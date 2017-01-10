using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.SaasData;
using AdvantShop.Statistic;
using CsvHelper;
using Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AdvantShop.ExportImport
{
    public class CsvImport
    {
        private readonly string _fullPath;
        private readonly bool _disablebProduct;
        private readonly bool _hasHeadrs;
        private static bool _skipOriginalPhoto;
        private Dictionary<string, int> _fieldMapping;
        private readonly string _separators;
        private readonly string _encodings;
        private readonly string _columSeparator;
        private readonly string _propertySeparator;


        private CsvImport(string filePath, bool hasHeadrs, bool disablebProduct, string separators, string encodings, Dictionary<string, int> fieldMapping, string columSeparator, string propertySeparator, bool skipOriginalPhoto)
        {
            _fullPath = filePath;
            _hasHeadrs = hasHeadrs;
            _disablebProduct = disablebProduct;
            _fieldMapping = fieldMapping;
            _encodings = encodings;
            _separators = separators;
            _columSeparator = columSeparator;
            _propertySeparator = propertySeparator;
            _skipOriginalPhoto = skipOriginalPhoto;
        }

        public static CsvImport Factory(string filePath, bool hasHeadrs, bool disablebProduct, string separators, string encodings, Dictionary<string, int> fieldMapping, string columSeparator, string propertySeparator, bool skipOriginalPhoto = false)
        {
            return new CsvImport(filePath, hasHeadrs, disablebProduct, separators, encodings, fieldMapping, columSeparator, propertySeparator, skipOriginalPhoto);
        }

        public static CsvImport Factory(string filePath, bool hasHeadrs, bool skipOriginalPhoto = false)
        {
            return new CsvImport(filePath, hasHeadrs, false, null, null, null, null, null, skipOriginalPhoto);
        }

        private CsvReader InitReader(bool? hasHeaderRecord = null)
        {
            var reader = new CsvReader(new StreamReader(_fullPath, Encoding.GetEncoding(_encodings ?? EncodingsEnum.Utf8.StrName())));
            reader.Configuration.Delimiter = _separators ?? SeparatorsEnum.SemicolonSeparated.StrName();
            if (hasHeaderRecord.HasValue)
                reader.Configuration.HasHeaderRecord = (bool)hasHeaderRecord;
            else
                reader.Configuration.HasHeaderRecord = _hasHeadrs;
            return reader;
        }

        public List<string[]> ReadFirst2()
        {
            var list = new List<string[]>();
            using (var csv = InitReader())
            {
                int count = 0;
                while (csv.Read())
                {
                    if (count == 2)
                        break;

                    if (csv.CurrentRecord != null)
                        list.Add(csv.CurrentRecord);
                    count++;
                }
            }
            return list;
        }

        public void Process(bool inBackGround = true)
        {
            CommonStatistic.StartNew(() =>
            {
                try
                {
                    _process();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    CommonStatistic.WriteLog(ex.Message);
                }
                CommonStatistic.IsRun = false;
            }, inBackGround);
        }

        private void _process()
        {
            if (_fieldMapping == null)
                MapFileds();

            if (_fieldMapping == null)
            {
                throw new Exception("can mapping colums");
            }

            if (_disablebProduct)
            {
                ProductService.DisableAllProducts();
            }

            
            CommonStatistic.TotalRow = GetRowCount();
            var somePostProcessing = _fieldMapping.ContainsKey(ProductFields.Related.StrName()) || _fieldMapping.ContainsKey(ProductFields.Alternative.StrName());

            if (somePostProcessing)
            {
                CommonStatistic.TotalRow *= 2;
            }

            ProcessRow(false, _columSeparator, _propertySeparator);
            if (somePostProcessing && CommonStatistic.IsRun)
                ProcessRow(true, _columSeparator, _propertySeparator);

            CommonStatistic.IsRun = false;

            CategoryService.RecalculateProductsCountManual();
            CategoryService.SetCategoryHierarchicallyEnabled(0);
            LuceneSearch.CreateAllIndexInBackground();
            ProductService.PreCalcProductParamsMassInBackground();

            CacheManager.Clean();
            FileHelpers.DeleteFilesFromImageTempInBackground();
            FileHelpers.DeleteFile(_fullPath);

        }

        private void MapFileds()
        {
            _fieldMapping = new Dictionary<string, int>();
            using (var csv = InitReader(false))
            {
                csv.Read();
                for (var i = 0; i < csv.CurrentRecord.Length; i++)
                {
                    if (csv.CurrentRecord[i] == ProductFields.None.StrName()) continue;
                    if (!_fieldMapping.ContainsKey(csv.CurrentRecord[i]))
                        _fieldMapping.Add(csv.CurrentRecord[i], i);
                }
            }
        }

        private long GetRowCount()
        {
            long count = 0;
            using (var csv = InitReader())
            {
                while (csv.Read())
                    count++;
            }
            return count;
        }

        private void ProcessRow(bool onlyPostProcess, string columSeparator, string propertySeparator)
        {
            if (!File.Exists(_fullPath)) return;
            using (var csv = InitReader())
            {
                while (csv.Read())
                {
                    if (!CommonStatistic.IsRun)
                    {
                        csv.Dispose();
                        FileHelpers.DeleteFile(_fullPath);
                        return;
                    }
                    try
                    {
                        
                        var productInStrings = PrepareRow(csv);
                        if (productInStrings == null) continue;

                        if (!onlyPostProcess)
                            UpdateInsertProduct(productInStrings, columSeparator, propertySeparator);
                        else
                            PostProcess(productInStrings, columSeparator);

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
        }

        private Dictionary<ProductFields, object> PrepareRow(ICsvReader csv)
        {
            // Step by rows
            var productInStrings = new Dictionary<ProductFields, object>();

            foreach (ProductFields productField in Enum.GetValues(typeof(ProductFields)))
            {
                switch (productField.Status())
                {
                    case ProductFieldStatus.String:
                        GetString(productField, csv, productInStrings);
                        break;
                    case ProductFieldStatus.StringRequired:
                        GetStringRequired(productField, csv, productInStrings);
                        break;
                    case ProductFieldStatus.NotEmptyString:
                        GetStringNotNull(productField, csv, productInStrings);
                        break;
                    case ProductFieldStatus.Float:
                        if (!GetDecimal(productField, csv, productInStrings))
                            return null;
                        break;
                    case ProductFieldStatus.Int:
                        if (!GetInt(productField, csv, productInStrings))
                            return null;
                        break;
                }
            }
            return productInStrings;
        }

        private bool GetString(ProductFields rEnum, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var nameField = rEnum.StrName();
            if (_fieldMapping.ContainsKey(nameField))
                productInStrings.Add(rEnum, TrimAnyWay(csv[_fieldMapping[nameField]]));
            return true;
        }

        private bool GetStringNotNull(ProductFields rEnum, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                productInStrings.Add(rEnum, tempValue);
            return true;
        }

        private bool GetStringRequired(ProductFields rEnum, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var tempValue = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (!string.IsNullOrEmpty(tempValue))
                productInStrings.Add(rEnum, tempValue);
            else
            {
                LogInvalidData(string.Format(Resource.Admin_ImportCsv_CanNotEmpty, ProductFields.Name.ResourceKey(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetDecimal(ProductFields rEnum, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var shippingPrice = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(shippingPrice))
                shippingPrice = "0";
            float tmp;
            if (float.TryParse(shippingPrice, out  tmp))
            {
                productInStrings.Add(rEnum, tmp);
            }
            else if (float.TryParse(shippingPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
            {
                productInStrings.Add(rEnum, tmp);
            }
            else
            {
                LogInvalidData(string.Format(Resource.Admin_ImportCsv_MustBeNumber, rEnum.ResourceKey(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private bool GetInt(ProductFields rEnum, ICsvReaderRow csv, IDictionary<ProductFields, object> productInStrings)
        {
            var nameField = rEnum.StrName();
            if (!_fieldMapping.ContainsKey(nameField)) return true;
            var amount = TrimAnyWay(csv[_fieldMapping[nameField]]);
            if (string.IsNullOrEmpty(amount))
                amount = "0";
            int tmp;
            if (int.TryParse(amount, out  tmp))
            {
                productInStrings.Add(rEnum, tmp);
            }
            else
            {
                LogInvalidData(string.Format(Resource.Admin_ImportCsv_MustBeNumber, rEnum.ResourceKey(), CommonStatistic.RowPosition + 2));
                return false;
            }
            return true;
        }

        private static string TrimAnyWay(string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Trim();
        }

        private static void LogInvalidData(string message)
        {
            CommonStatistic.WriteLog(message);
            CommonStatistic.TotalErrorRow++;
            CommonStatistic.RowPosition++;
        }

        private static bool useMultiThreadImport = false;

        public static void UpdateInsertProduct(Dictionary<ProductFields, object> productInStrings, string columSeparator, string propertySeparator)
        {
            if (useMultiThreadImport)
            {
                var added = false;
                while (!added)
                {
                    int workerThreads;
                    int asyncIoThreads;
                    ThreadPool.GetAvailableThreads(out workerThreads, out asyncIoThreads);
                    if (workerThreads != 0)
                    {
                        //ThreadPool.QueueUserWorkItem(UpdateInsertProductWorker, productInStrings);
                        Task.Factory.StartNew(() => UpdateInsertProductWorker(columSeparator, propertySeparator, productInStrings));
                        added = true;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            else
            {
                UpdateInsertProductWorker(columSeparator, propertySeparator, productInStrings);
            }
        }

        private static void UpdateInsertProductWorker(string columSeparator, string propertySeparator, Dictionary<ProductFields, object> productInStrings)
        {
            try
            {
                bool addingNew;
                Product product = null;
                if (productInStrings.ContainsKey(ProductFields.Sku) && productInStrings[ProductFields.Sku].AsString().IsNullOrEmpty())
                    throw new Exception("SKU can not be empty");

                var artNo = productInStrings.ContainsKey(ProductFields.Sku) ? productInStrings[ProductFields.Sku].AsString() : string.Empty;
                if (string.IsNullOrEmpty(artNo))
                {
                    addingNew = true;
                }
                else
                {
                    product = ProductService.GetProduct(artNo);
                    addingNew = product == null;
                }

                if (addingNew)
                {
                    product = new Product { ArtNo = string.IsNullOrEmpty(artNo) ? null : artNo, Multiplicity = 1 };
                }

                if (productInStrings.ContainsKey(ProductFields.Name))
                    product.Name = productInStrings[ProductFields.Name].AsString();
                else
                    product.Name = product.Name ?? string.Empty;

                if (productInStrings.ContainsKey(ProductFields.Enabled))
                {
                    product.Enabled = productInStrings[ProductFields.Enabled].AsString().Trim().Equals("+");
                }

                if (productInStrings.ContainsKey(ProductFields.OrderByRequest))
                    product.AllowPreOrder = productInStrings[ProductFields.OrderByRequest].AsString().Trim().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.Discount))
                    product.Discount = productInStrings[ProductFields.Discount].AsFloat();

                if (productInStrings.ContainsKey(ProductFields.Weight))
                    product.Weight = productInStrings[ProductFields.Weight].AsFloat();

                if (productInStrings.ContainsKey(ProductFields.Size))
                    product.Size = GetSizeForBdFormat(productInStrings[ProductFields.Size].AsString());

                if (productInStrings.ContainsKey(ProductFields.BriefDescription))
                    product.BriefDescription = productInStrings[ProductFields.BriefDescription].AsString();

                if (productInStrings.ContainsKey(ProductFields.Description))
                    product.Description = productInStrings[ProductFields.Description].AsString();

                if (productInStrings.ContainsKey(ProductFields.SalesNotes))
                    product.SalesNote = productInStrings[ProductFields.SalesNotes].AsString();

                if (productInStrings.ContainsKey(ProductFields.Gtin))
                    product.Gtin = productInStrings[ProductFields.Gtin].AsString();

                if (productInStrings.ContainsKey(ProductFields.GoogleProductCategory))
                    product.GoogleProductCategory = productInStrings[ProductFields.GoogleProductCategory].AsString();

                if (productInStrings.ContainsKey(ProductFields.Adult))
                    product.Adult = productInStrings[ProductFields.Adult].AsString().Trim().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.ManufacturerWarranty))
                    product.ManufacturerWarranty = productInStrings[ProductFields.ManufacturerWarranty].AsString().Trim().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.ShippingPrice))
                    product.ShippingPrice = productInStrings[ProductFields.ShippingPrice].AsFloat();

                if (productInStrings.ContainsKey(ProductFields.Unit))
                    product.Unit = productInStrings[ProductFields.Unit].AsString();

                if (productInStrings.ContainsKey(ProductFields.MultiOffer))
                {
                    OfferService.OffersFromString(product, productInStrings[ProductFields.MultiOffer].AsString(), columSeparator, propertySeparator);
                }
                else
                {
                    OfferService.OfferFromFields(product, productInStrings.ContainsKey(ProductFields.Price) ? productInStrings[ProductFields.Price].AsFloat() : (float?)null,
                                                        productInStrings.ContainsKey(ProductFields.PurchasePrice) ? productInStrings[ProductFields.PurchasePrice].AsFloat() : (float?)null,
                                                  productInStrings.ContainsKey(ProductFields.Amount) ? productInStrings[ProductFields.Amount].AsFloat() : (float?)null);
                }

                if (productInStrings.ContainsKey(ProductFields.ParamSynonym))
                {
                    var prodUrl = productInStrings[ProductFields.ParamSynonym].AsString().IsNotEmpty()
                                      ? productInStrings[ProductFields.ParamSynonym].AsString()
                                      : product.ArtNo;
                    product.UrlPath = UrlService.GetAvailableValidUrl(product.ID, ParamType.Product, prodUrl);
                }
                else
                {
                    product.UrlPath = product.UrlPath ??
                                      UrlService.GetAvailableValidUrl(product.ID, ParamType.Product,
                                      product.ArtNo ?? product.Name.Substring(0, product.Name.Length - 1 < 50 ? product.Name.Length - 1 : 50));

                }

                product.Meta.ObjId = product.ProductId;

                if (productInStrings.ContainsKey(ProductFields.Title))
                    product.Meta.Title = productInStrings[ProductFields.Title].AsString();
                else
                    product.Meta.Title = product.Meta.Title ?? SettingsSEO.ProductMetaTitle;

                if (productInStrings.ContainsKey(ProductFields.H1))
                    product.Meta.H1 = productInStrings[ProductFields.H1].AsString();
                else
                    product.Meta.H1 = product.Meta.H1 ?? SettingsSEO.ProductMetaH1;

                if (productInStrings.ContainsKey(ProductFields.MetaKeywords))
                    product.Meta.MetaKeywords = productInStrings[ProductFields.MetaKeywords].AsString();
                else
                    product.Meta.MetaKeywords = product.Meta.MetaKeywords ?? SettingsSEO.ProductMetaKeywords;

                if (productInStrings.ContainsKey(ProductFields.MetaDescription))
                    product.Meta.MetaDescription = productInStrings[ProductFields.MetaDescription].AsString();
                else
                    product.Meta.MetaDescription = product.Meta.MetaDescription ?? SettingsSEO.ProductMetaDescription;

                if (productInStrings.ContainsKey(ProductFields.Markers))
                    ProductService.MarkersFromString(product, productInStrings[ProductFields.Markers].AsString(), columSeparator);

                if (productInStrings.ContainsKey(ProductFields.Producer))
                    product.BrandId = BrandService.BrandFromString(productInStrings[ProductFields.Producer].AsString());

                if (!addingNew)
                {
                    ProductService.UpdateProduct(product, false);
                    CommonStatistic.TotalUpdateRow++;
                }
                else
                {
                    if (!(SaasDataService.IsSaasEnabled && ProductService.GetProductsCount() >= SaasDataService.CurrentSaasData.ProductsCount))
                    {
                        ProductService.AddProduct(product, false);
                        CommonStatistic.TotalAddRow++;
                    }
                    else
                    {
                        Log(CommonStatistic.RowPosition + ": " + "Превышен лимит по количеству товаров");
                        CommonStatistic.TotalErrorRow++;
                    }
                }

                if (product.ProductId > 0)
                    OtherFields(productInStrings, product.ProductId, columSeparator, propertySeparator);
            }
            catch (OutOfMemoryException e)
            {
                Debug.LogError(e);
                CommonStatistic.TotalErrorRow++;
                Log(CommonStatistic.RowPosition + ": " + "Неверный формат файла " + productInStrings[ProductFields.Photos].AsString());
            }
            catch (Exception e)
            {
                CommonStatistic.TotalErrorRow++;
                Log(CommonStatistic.RowPosition + ": " + e.Message);
                Debug.LogError(e);
            }

            productInStrings.Clear();
            CommonStatistic.RowPosition++;
        }


        private static void OtherFields(IDictionary<ProductFields, object> fields, int productId, string columSeparator, string propertySeparator)
        {
            //Category
            if (fields.ContainsKey(ProductFields.Category))
            {
                string sorting = string.Empty;
                if (fields.ContainsKey(ProductFields.Sorting))
                {
                    sorting = fields[ProductFields.Sorting].AsString();
                }
                var parentCategory = fields[ProductFields.Category].AsString();
                CategoryService.SubParseAndCreateCategory(parentCategory, productId, columSeparator, sorting);
            }

            //photo
            if (fields.ContainsKey(ProductFields.Photos))
            {
                string photos = fields[ProductFields.Photos].AsString();
                if (!string.IsNullOrEmpty(photos))
                    PhotoService.PhotoFromString(productId, photos, columSeparator, propertySeparator, _skipOriginalPhoto);
            }

            //video
            if (fields.ContainsKey(ProductFields.Videos))
            {
                string videos = fields[ProductFields.Videos].AsString();
                ProductVideoService.VideoFromString(productId, videos, columSeparator);
            }

            //Properties
            if (fields.ContainsKey(ProductFields.Properties))
            {
                string properties = fields[ProductFields.Properties].AsString();
                PropertyService.PropertiesFromString(productId, properties, columSeparator, propertySeparator);
            }

            if (fields.ContainsKey(ProductFields.CustomOption))
            {
                string customOption = fields[ProductFields.CustomOption].AsString();
                CustomOptionsService.CustomOptionsFromString(productId, customOption);
            }
        }

        private static void Log(string message)
        {
            CommonStatistic.WriteLog(message);
        }

        private static string GetSizeForBdFormat(string str)
        {
            if (string.IsNullOrEmpty(str)) return "0|0|0";

            var listSymb = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '.' };
            string res = string.Empty;
            var list = new List<string>();
            foreach (char t in str)
            {
                if (listSymb.Contains(t))
                {
                    res += t;
                }
                else
                {
                    if (!string.IsNullOrEmpty(res))
                    {
                        list.Add(res.Trim());
                        res = string.Empty;
                    }
                }
            }
            if (!string.IsNullOrEmpty(res))
                list.Add(res.Trim());

            res = list.AggregateString('|');

            return res;
        }

        public static void PostProcess(Dictionary<ProductFields, object> productInStrings, string columSeparator)
        {
            if (productInStrings.ContainsKey(ProductFields.Sku))
            {
                var artNo = productInStrings[ProductFields.Sku].AsString();
                int productId = ProductService.GetProductId(artNo);

                //relations
                if (productInStrings.ContainsKey(ProductFields.Related))
                {
                    var linkproducts = productInStrings[ProductFields.Related].AsString();
                    ProductService.LinkedProductFromString(productId, linkproducts, RelatedType.Related, columSeparator);
                }

                //relations
                if (productInStrings.ContainsKey(ProductFields.Alternative))
                {
                    var linkproducts = productInStrings[ProductFields.Alternative].AsString();
                    ProductService.LinkedProductFromString(productId, linkproducts, RelatedType.Alternative, columSeparator);
                }
            }
            CommonStatistic.RowPosition++;
        }

    }

    public static class CsvExt
    {
        public static string AsString(this object val)
        {
            var t = val as string;
            return t ?? "";
        }

        public static float AsFloat(this object val)
        {
            if (val is float)
                return (float)val;
            return 0F;
        }
    }
}