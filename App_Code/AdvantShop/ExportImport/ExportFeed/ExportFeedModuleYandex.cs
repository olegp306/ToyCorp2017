//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Repository.Currencies;
using AdvantShop.Statistic;

//test commit

namespace AdvantShop.ExportImport
{
    public class ExportFeedModuleYandex : ExportFeedModule
    {
        private string _currency;
        private string _description;
        private string _salesNotes;
        private bool _delivery;
        private bool _localDeliveryCost;
        private bool _properties;
        private bool _removeHTML;

        private List<ProductDiscount> _productDiscountModels = null;

        protected override string ModuleName
        {
            get { return "YandexMarket"; }
        }

        public static List<string> AvailableCurrencies = new List<string> { "RUR", "RUB", "USD", "BYR", "KZT", "EUR", "UAH" };
        public override void GetExportFeedString(string filenameAndPath)
        {
            var tempfile = filenameAndPath + "_temp";

            try
            {
                _currency = ExportFeed.GetModuleSetting(ModuleName, "Currency");
                _description = ExportFeed.GetModuleSetting(ModuleName, "DescriptionSelection");
                _salesNotes = ExportFeed.GetModuleSetting(ModuleName, "SalesNotes");
                _delivery = ExportFeed.GetModuleSetting(ModuleName, "Delivery").TryParseBool();
                _localDeliveryCost = ExportFeed.GetModuleSetting(ModuleName, "LocalDeliveryCost").TryParseBool();
                _properties = ExportFeed.GetModuleSetting(ModuleName, "Properties").TryParseBool();
                _removeHTML = ExportFeed.GetModuleSetting(ModuleName, "RemoveHTML").TryParseBool();

                var discountModule = AttachedModules.GetModules<IDiscount>().FirstOrDefault();
                if (discountModule != null)
                {
                    var classInstance = (IDiscount)Activator.CreateInstance(discountModule);
                    _productDiscountModels = classInstance.GetProductDiscountsList();
                }
                
                var shopName = ExportFeed.GetModuleSetting(ModuleName, "ShopName").Replace("#STORE_NAME#", SettingsMain.ShopName);
                var companyName = ExportFeed.GetModuleSetting(ModuleName, "CompanyName").Replace("#STORE_NAME#", SettingsMain.ShopName);
                FileHelpers.DeleteFile(tempfile);
                using (var outputFile = new FileStream(tempfile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
                    using (var writer = XmlWriter.Create(outputFile, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteDocType("yml_catalog", null, "shops.dtd", null);
                        writer.WriteStartElement("yml_catalog");
                        writer.WriteAttributeString("date", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        writer.WriteStartElement("shop");

                        writer.WriteStartElement("name");
                        writer.WriteRaw(shopName);
                        writer.WriteEndElement();

                        writer.WriteStartElement("company");
                        writer.WriteRaw(companyName);
                        writer.WriteEndElement();

                        writer.WriteStartElement("url");
                        writer.WriteRaw(ShopUrl);
                        writer.WriteEndElement();

                        writer.WriteStartElement("currencies");
                        var currencies = GetCurrencies().Where(item => AvailableCurrencies.Contains(item.Iso3)).ToList();
                        ProcessCurrency(currencies, _currency, writer);
                        writer.WriteEndElement();

                        CommonStatistic.TotalRow = GetCategoriesCount(ModuleName) + GetProdutsCount(ModuleName);
                        writer.WriteStartElement("categories");
                        foreach (var categoryRow in GetCategories(ModuleName))
                        {
                            ProcessCategoryRow(categoryRow, writer);
                            CommonStatistic.RowPosition++;
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("offers");

                        foreach (var offerRow in GetProduts(ModuleName))
                        {
                            ProcessProductRow(offerRow, writer);
                            CommonStatistic.RowPosition++;
                        }
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();

                        writer.Flush();
                        writer.Close();
                    }
                }

                FileHelpers.DeleteFile(filenameAndPath);
                File.Move(tempfile, filenameAndPath);

            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private static void ProcessCurrency(List<Currency> currencies, string currency, XmlWriter writer)
        {
            if (currencies == null) return;
            var defaultCurrency = currencies.FirstOrDefault(item => item.Iso3 == currency);
            if (defaultCurrency == null) return;
            ProcessCurrencyRow(new Currency
                {
                    CurrencyID = defaultCurrency.CurrencyID,
                    Value = 1,
                    Iso3 = defaultCurrency.Iso3
                }, writer);

            foreach (var curRow in currencies.Where(item => item.Iso3 != currency))
            {
                curRow.Value = curRow.Value / defaultCurrency.Value;
                ProcessCurrencyRow(curRow, writer);
            }
        }

        private static void ProcessCurrencyRow(Currency currency, XmlWriter writer)
        {
            writer.WriteStartElement("currency");
            writer.WriteAttributeString("id", currency.Iso3);

            var nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";
            writer.WriteAttributeString("rate", Math.Round(currency.Value, 2).ToString(nfi));
            writer.WriteEndElement();
        }

        private static void ProcessCategoryRow(ExportFeedCategories row, XmlWriter writer)
        {
            writer.WriteStartElement("category");
            writer.WriteAttributeString("id", row.Id.ToString(CultureInfo.InvariantCulture));
            if (row.ParentCategory != 0)
            {
                writer.WriteAttributeString("parentId", row.ParentCategory.ToString(CultureInfo.InvariantCulture));
            }
            writer.WriteRaw(row.Name.XmlEncode().RemoveInvalidXmlChars());
            writer.WriteEndElement();
        }

        private void ProcessProductRow(ExportFeedProduts row, XmlWriter writer)
        {
            if (string.IsNullOrWhiteSpace(row.BrandName)) ProcessSimpleModel(row, writer);
            else ProcessVendorModel(row, writer);
        }

        private string CreateLink(ExportFeedProduts row)
        {
            var sufix = string.Empty;
            if (!row.Main)
            {
                if (row.ColorId != 0)
                    sufix = "color=" + row.ColorId;
                if (row.SizeId != 0)
                {
                    if (string.IsNullOrEmpty(sufix))
                        sufix = "size=" + row.SizeId;
                    else
                        sufix += "&amp;size=" + row.SizeId;
                }
                sufix = !string.IsNullOrEmpty(sufix) ? "?" + sufix : sufix;
            }
            return ShopUrl.TrimEnd('/') + "/" + UrlService.GetLink(ParamType.Product, row.UrlPath, row.ProductId) + sufix;
        }

        private void ProcessSimpleModel(ExportFeedProduts row, XmlWriter writer)
        {
            //var tempUrl = (_shopUrl.EndsWith("/") ? _shopUrl.TrimEnd('/') : _shopUrl);
            writer.WriteStartElement("offer");
            writer.WriteAttributeString("id", row.ProductId.ToString().XmlEncode().RemoveInvalidXmlChars());
            writer.WriteAttributeString("group_id", row.ProductId.ToString());
            writer.WriteAttributeString("available", (row.Amount > 0).ToString().ToLower());

            writer.WriteStartElement("url");
            writer.WriteRaw(CreateLink(row));
            writer.WriteEndElement();


            float discount=0;
            if (_productDiscountModels != null)
            {
                var prodDiscount = _productDiscountModels.Find(d => d.ProductId == row.ProductId);
                if (prodDiscount != null)
                {
                    discount = prodDiscount.Discount;
                }
            }

            writer.WriteStartElement("price");
            var nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";
            writer.WriteRaw(Math.Round(CatalogService.CalculatePrice(row.Price, discount != 0 ? discount : row.Discount)).ToString(nfi));
            writer.WriteEndElement();

            writer.WriteStartElement("currencyId");
            writer.WriteRaw(_currency);
            writer.WriteEndElement();

            writer.WriteStartElement("categoryId");
            writer.WriteRaw(row.ParentCategory.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            if (!string.IsNullOrEmpty(row.Photos))
            {
                var temp = row.Photos.Split(',').Take(9);
                foreach (var item in temp.Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    writer.WriteStartElement("picture");
                    writer.WriteRaw(GetImageProductPath(item));
                    writer.WriteEndElement();
                }
            }

            if (_delivery)
            {
                writer.WriteStartElement("delivery");
                writer.WriteRaw(_delivery.ToString().ToLower());
                writer.WriteEndElement();
            }

            if (_localDeliveryCost)
            {
                writer.WriteStartElement("local_delivery_cost");
                writer.WriteRaw(Math.Round(row.ShippingPrice).ToString(nfi));
                writer.WriteEndElement();
            }

            writer.WriteStartElement("name");
            writer.WriteRaw(row.Name.XmlEncode().RemoveInvalidXmlChars() + (!string.IsNullOrWhiteSpace(row.SizeName) ? " " + row.SizeName : string.Empty) + (!string.IsNullOrWhiteSpace(row.ColorName) ? " " + row.ColorName : string.Empty));
            writer.WriteEndElement();

            writer.WriteStartElement("description");
            string desc = SQLDataHelper.GetString(_description == "full" ? row.Description : row.BriefDescription);

            if (_removeHTML)
                desc = StringHelper.RemoveHTML(desc);

            writer.WriteRaw(desc.XmlEncode().RemoveInvalidXmlChars());

            writer.WriteEndElement();

            if (!string.IsNullOrWhiteSpace(row.SalesNote))
            {
                writer.WriteStartElement("sales_notes");
                writer.WriteRaw(row.SalesNote.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }
            else if (!string.IsNullOrWhiteSpace(_salesNotes))
            {
                writer.WriteStartElement("sales_notes");
                writer.WriteRaw(_salesNotes.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (row.ManufacturerWarranty)
            {
                writer.WriteStartElement("manufacturer_warranty");
                writer.WriteRaw(row.ManufacturerWarranty.ToString().ToLower());
                writer.WriteEndElement();
            }

            if (row.Adult)
            {
                writer.WriteStartElement("adult");
                writer.WriteRaw(row.Adult.ToString().ToLower());
                writer.WriteEndElement();
            }

            if (!string.IsNullOrWhiteSpace(row.ColorName))
            {
                writer.WriteStartElement("param");
                writer.WriteAttributeString("name", SettingsCatalog.ColorsHeader.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteRaw(row.ColorName.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (!string.IsNullOrWhiteSpace(row.SizeName))
            {
                writer.WriteStartElement("param");
                writer.WriteAttributeString("name", SettingsCatalog.SizesHeader.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteRaw(row.SizeName.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (_properties)
            {
                foreach (var prop in PropertyService.GetPropertyValuesByProductId(row.ProductId))
                {
                    if (prop.Property.Name.IsNotEmpty() && prop.Value.IsNotEmpty())
                    {
                        writer.WriteStartElement("param");
                        writer.WriteAttributeString("name", prop.Property.Name.XmlEncode().RemoveInvalidXmlChars());
                        writer.WriteRaw(prop.Value.XmlEncode().RemoveInvalidXmlChars());
                        writer.WriteEndElement();
                    }
                }
            }
            
            writer.WriteEndElement();
        }

        private void ProcessVendorModel(ExportFeedProduts row, XmlWriter writer)
        {

            var list = new List<string>() {"a", "b"};

            writer.WriteStartElement("offer");
            writer.WriteAttributeString("id", row.ArtNo.XmlEncode().RemoveInvalidXmlChars());
            writer.WriteAttributeString("group_id", row.ProductId.ToString());

            writer.WriteAttributeString("available", (row.Amount > 0).ToString().ToLower());

            writer.WriteAttributeString("type", "vendor.model");

            writer.WriteStartElement("url");
            writer.WriteRaw(CreateLink(row));
            writer.WriteEndElement();

            float discount = 0;
            if (_productDiscountModels != null)
            {
                var prodDiscount = _productDiscountModels.Find(d => d.ProductId == row.ProductId);
                if (prodDiscount != null)
                {
                    discount = prodDiscount.Discount;
                }
            }

            writer.WriteStartElement("price");
            var nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";
            writer.WriteRaw(Math.Round(CatalogService.CalculatePrice(row.Price, discount != 0 ? discount : row.Discount)).ToString(nfi));
            writer.WriteEndElement();

            writer.WriteStartElement("currencyId");
            writer.WriteRaw(_currency);
            writer.WriteEndElement();

            writer.WriteStartElement("categoryId");
            writer.WriteRaw(row.ParentCategory.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();


            if (!string.IsNullOrEmpty(row.Photos))
            {
                var temp = row.Photos.Split(',').Take(9);
                foreach (var item in temp.Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    writer.WriteStartElement("picture");
                    writer.WriteRaw(GetImageProductPath(item));
                    writer.WriteEndElement();
                }
            }

            if (_delivery)
            {
                writer.WriteStartElement("delivery");
                writer.WriteRaw(_delivery.ToString().ToLower());
                writer.WriteEndElement();
            }

            if (_localDeliveryCost)
            {
                writer.WriteStartElement("local_delivery_cost");
                writer.WriteRaw(Math.Round(row.ShippingPrice).ToString(nfi));
                writer.WriteEndElement();
            }


            writer.WriteStartElement("vendor");
            writer.WriteRaw(row.BrandName.XmlEncode().RemoveInvalidXmlChars());
            writer.WriteEndElement();

            writer.WriteStartElement("vendorCode");
            writer.WriteRaw(row.ArtNo.XmlEncode().RemoveInvalidXmlChars());
            writer.WriteEndElement();

            writer.WriteStartElement("model");
            writer.WriteRaw(row.Name.XmlEncode().RemoveInvalidXmlChars() + (!string.IsNullOrWhiteSpace(row.SizeName) ? " " + row.SizeName.XmlEncode().RemoveInvalidXmlChars() : string.Empty) + (!string.IsNullOrWhiteSpace(row.ColorName) ? " " + row.ColorName.XmlEncode().RemoveInvalidXmlChars() : string.Empty));
            writer.WriteEndElement();

            writer.WriteStartElement("description");
            string desc = SQLDataHelper.GetString(_description == "full" ? row.Description : row.BriefDescription);

            if (_removeHTML)
                desc = StringHelper.RemoveHTML(desc);

            writer.WriteRaw(desc.XmlEncode().RemoveInvalidXmlChars());

            writer.WriteEndElement();

            if (!string.IsNullOrWhiteSpace(row.SalesNote))
            {
                writer.WriteStartElement("sales_notes");
                writer.WriteRaw(row.SalesNote.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }
            else if (!string.IsNullOrWhiteSpace(_salesNotes))
            {
                writer.WriteStartElement("sales_notes");
                writer.WriteRaw(_salesNotes.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (row.ManufacturerWarranty)
            {
                writer.WriteStartElement("manufacturer_warranty");
                writer.WriteRaw(row.ManufacturerWarranty.ToString().ToLower());
                writer.WriteEndElement();
            }

            if (row.Adult)
            {
                writer.WriteStartElement("adult");
                writer.WriteRaw(row.Adult.ToString().ToLower());
                writer.WriteEndElement();
            }

            if (!string.IsNullOrWhiteSpace(row.ColorName))
            {
                writer.WriteStartElement("param");
                writer.WriteAttributeString("name", SettingsCatalog.ColorsHeader.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteRaw(row.ColorName.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (!string.IsNullOrWhiteSpace(row.SizeName))
            {
                writer.WriteStartElement("param");
                writer.WriteAttributeString("name", SettingsCatalog.SizesHeader.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteRaw(row.SizeName.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            if (_properties)
            {
                foreach (var prop in PropertyService.GetPropertyValuesByProductId(row.ProductId))
                {
                    if (prop.Property.Name.IsNotEmpty() && prop.Value.IsNotEmpty())
                    {
                        writer.WriteStartElement("param");
                        writer.WriteAttributeString("name", prop.Property.Name.XmlEncode().RemoveInvalidXmlChars());
                        writer.WriteRaw(prop.Value.XmlEncode().RemoveInvalidXmlChars());
                        writer.WriteEndElement();
                    }
                }
            }

            writer.WriteEndElement();
        }
    }
}