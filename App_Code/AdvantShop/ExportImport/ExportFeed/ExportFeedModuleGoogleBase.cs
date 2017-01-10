//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using AdvantShop.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Statistic;

namespace AdvantShop.ExportImport
{
    public class ExportFeedModuleGoogleBase : ExportFeedModule
    {
        private string _currency;
        private string _description;
        private string _googleProductCategory;
        private bool _removeHTML;
        const string GoogleBaseNamespace = "http://base.google.com/ns/1.0";
        private List<ProductDiscount> _productDiscountModels = null;


        protected override string ModuleName
        {
            get { return "GoogleBase"; }
        }

        public override void GetExportFeedString(string filenameAndPath)
        {

            _description = ExportFeed.GetModuleSetting(ModuleName, "DescriptionSelection");
            _currency = ExportFeed.GetModuleSetting(ModuleName, "Currency");
            _googleProductCategory = ExportFeed.GetModuleSetting(ModuleName, "GoogleProductCategory");
            _removeHTML = ExportFeed.GetModuleSetting(ModuleName, "RemoveHTML").TryParseBool();

            var discountModule = AttachedModules.GetModules<IDiscount>().FirstOrDefault();
            if (discountModule != null)
            {
                var classInstance = (IDiscount)Activator.CreateInstance(discountModule);
                _productDiscountModels = classInstance.GetProductDiscountsList();
            }

            var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

            using (var stream = new FileStream(filenameAndPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    // source http://www.google.com/support/merchants/bin/answer.py?answer=188494&expand=GB
                    writer.WriteStartDocument();

                    writer.WriteStartElement("rss");
                    writer.WriteAttributeString("version", "2.0");
                    writer.WriteAttributeString("xmlns", "g", null, GoogleBaseNamespace);
                    writer.WriteStartElement("channel");
                    writer.WriteElementString("title", ExportFeed.GetModuleSetting(ModuleName, "DatafeedTitle"));
                    writer.WriteElementString("link", ShopUrl);
                    writer.WriteElementString("description", ExportFeed.GetModuleSetting(ModuleName, "DatafeedDescription"));

                    CommonStatistic.TotalRow = GetProdutsCount(ModuleName);
                    foreach (var productRow in GetProduts(ModuleName))
                    {
                        ProcessProductRow(productRow, writer);
                        CommonStatistic.RowPosition++;
                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
        }

        private void ProcessProductRow(ExportFeedProduts row, XmlWriter writer)
        {
            writer.WriteStartElement("item");

            #region Основные сведения о товарах

            //id
            writer.WriteElementString("g", "id", GoogleBaseNamespace, row.OfferId.ToString(CultureInfo.InvariantCulture));

            //title [title]
            writer.WriteStartElement("title");
            var title = row.Name.XmlEncode().RemoveInvalidXmlChars();
            //title should be not longer than 70 characters
            if (title.Length > 70)
                title = title.Substring(0, 70);
            writer.WriteCData(title);
            writer.WriteEndElement();

            //description
            
            var desc = _description == "full" ? row.Description : row.BriefDescription;
            if (_removeHTML)
                desc = StringHelper.RemoveHTML(desc);

            if (desc.IsNotEmpty())
            {
                writer.WriteStartElement("description");
                writer.WriteCData(desc.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteEndElement();
            }

            //google_product_category http://www.google.com/support/merchants/bin/answer.py?answer=160081
            var googleProductCategory = row.GoogleProductCategory;
            if (string.IsNullOrEmpty(googleProductCategory))
                googleProductCategory = _googleProductCategory;
            writer.WriteStartElement("g", "google_product_category", GoogleBaseNamespace);
            writer.WriteCData(googleProductCategory.XmlEncode().RemoveInvalidXmlChars());
            writer.WriteEndElement();

            //product_type
            var localPath = string.Empty;
            var cats = CategoryService.GetParentCategories(row.ParentCategory).Reverse()
                                      .Select(cat => new
                                      {
                                          Name = cat.Name,
                                          Url = UrlService.GetLink(ParamType.Category, cat.UrlPath, cat.ID)
                                      }).ToList();
            for (var i = 0; i < cats.Count; i++)
            {
                var cat = cats[i];
                localPath = localPath + cat.Name;
                if (i == cats.Count - 1) continue;
                localPath = localPath + " > ";
            }
            writer.WriteStartElement("g", "product_type", GoogleBaseNamespace);
            writer.WriteCData(localPath.XmlEncode().RemoveInvalidXmlChars());
            writer.WriteEndElement();

            if (row.Adult)
                writer.WriteElementString("g", "adult", GoogleBaseNamespace, row.Adult.ToString());


            //link
            writer.WriteElementString("link", ShopUrl.TrimEnd('/') + "/" + UrlService.GetLink(ParamType.Product, row.UrlPath, row.ProductId));

            //image link
            if (!string.IsNullOrEmpty(row.Photos))
            {
                var temp = row.Photos.Split(',');
                for (var i = 0; i < temp.Length; i++)
                    writer.WriteElementString("g", i == 0 ? "image_link" : "additional_image_link", GoogleBaseNamespace, GetImageProductPath(temp[i]));
            }

            //condition 
            writer.WriteElementString("g", "condition", GoogleBaseNamespace, "new");
            #endregion


            #region наличие и цена
            //availability
            const string availability = "in stock";
            writer.WriteElementString("g", "availability", GoogleBaseNamespace, availability);


            float discount = 0;
            if (_productDiscountModels != null)
            {
                var prodDiscount = _productDiscountModels.Find(d => d.ProductId == row.ProductId);
                if (prodDiscount != null)
                {
                    discount = prodDiscount.Discount;
                }
            }

            writer.WriteElementString("g", "price", GoogleBaseNamespace, Math.Round(CatalogService.CalculatePrice(row.Price, discount != 0 ? discount : row.Discount)).ToString());

            #endregion

            #region Уникальные идентификаторы товаров
            //GTIN 
            var gtin = row.Gtin;
            if (!string.IsNullOrEmpty(gtin))
            {
                writer.WriteStartElement("g", "gtin", GoogleBaseNamespace);
                writer.WriteCData(gtin);
                writer.WriteFullEndElement(); // g:gtin
            }

            //brand 
            if (!string.IsNullOrEmpty(row.BrandName))
            {
                writer.WriteStartElement("g", "brand", GoogleBaseNamespace);
                writer.WriteCData(row.BrandName.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteFullEndElement(); // g:brand
            }

            //mpn [mpn]
            if (!string.IsNullOrEmpty(row.ArtNo))
            {
                writer.WriteStartElement("g", "mpn", GoogleBaseNamespace);
                writer.WriteCData(row.ArtNo.XmlEncode().RemoveInvalidXmlChars());
                writer.WriteFullEndElement(); // g:mpn
            }
            #endregion

            #region Варианты товара
            if (!(row.ColorName.IsNullOrEmpty() || row.SizeName.IsNullOrEmpty()))
            {
                //item_group_id
                writer.WriteElementString("g", "item_group_id", GoogleBaseNamespace, row.ProductId.ToString());
                //color
                writer.WriteElementString("g", "color", GoogleBaseNamespace, row.ColorName.XmlEncode().RemoveInvalidXmlChars());
                //color
                writer.WriteElementString("g", "size", GoogleBaseNamespace, row.SizeName.XmlEncode().RemoveInvalidXmlChars());
            }

            #endregion

            #region Tax & Shipping
            #endregion

            writer.WriteElementString("g", "expiration_date", GoogleBaseNamespace, DateTime.Now.AddDays(28).ToString("yyyy-MM-dd"));
            writer.WriteEndElement();
        }
    }
}