//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;
using AdvantShop.Core.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.SEO;
using AdvantShop.Statistic;
using CsvHelper;
using Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AdvantShop.ExportImport
{
    public class CsvExport
    {
        private const int MaxCellLength = 60000;
        private readonly string _path;
        private readonly string _encodeType;
        private readonly string _delimetr;
        private readonly string _columSeparator;
        private readonly string _propertySeparator;
        private readonly List<ProductFields> _fieldMapping;
        private readonly bool _csvExportNoInCategory;

        private CsvExport(string path, string encodeType, string delimetr, string columSeparator, string propertySeparator, List<ProductFields> fieldMapping, bool csvExportNoInCategory)
        {
            _path = path;
            _encodeType = encodeType;
            _delimetr = delimetr;
            _columSeparator = columSeparator;
            _propertySeparator = propertySeparator;
            _fieldMapping = fieldMapping;
            _csvExportNoInCategory = csvExportNoInCategory;
        }

        public static CsvExport Factory(string path, string encodeType, string delimetr, string columSeparator, string propertySeparator, List<ProductFields> fieldMapping, bool csvExportNoInCategory = false)
        {
            return new CsvExport(path, encodeType, delimetr, columSeparator, propertySeparator, fieldMapping, csvExportNoInCategory);
        }

        private CsvWriter InitWriter()
        {
            var writer = new CsvWriter(new StreamWriter(_path, false, Encoding.GetEncoding(_encodeType)));
            writer.Configuration.Delimiter = _delimetr;
            return writer;
        }

        public void SaveProductsToCsv(ProductCsvFilterModel filterModel)
        {
            using (var writer = InitWriter())
            {
                WriteHeader(writer);
                var products = ProductService.GetCsvProducts(filterModel);
                if (products == null) return;

                foreach (var product in products)
                {
                    if (!CommonStatistic.IsRun) return;

                    if (_fieldMapping.Contains(ProductFields.Description) && product.Description.Length > MaxCellLength)
                    {
                        CommonStatistic.WriteLog(string.Format(Resource.Admin_ExportCsv_TooLargeDescription, product.Name, product.ArtNo));
                        CommonStatistic.TotalErrorRow++;
                        continue;
                    }

                    if (_fieldMapping.Contains(ProductFields.BriefDescription) && product.BriefDescription.Length > MaxCellLength)
                    {
                        CommonStatistic.WriteLog(string.Format(Resource.Admin_ExportCsv_TooLargeBriefDescription, product.Name, product.ArtNo));
                        CommonStatistic.TotalErrorRow++;
                        continue;
                    }
                    WriteItem(writer, product);
                    CommonStatistic.RowPosition++;
                }
            }
        }

        private void WriteHeader(ICsvWriter writer)
        {
            foreach (var item in _fieldMapping)
                writer.WriteField(item.StrName());
            writer.NextRecord();
        }

        private void WriteItem(ICsvWriter writer, Product product)
        {
            var meta = MetaInfoService.GetMetaInfo(product.ID, MetaType.Product) ??
                        new MetaInfo(0, 0, MetaType.Product, string.Empty, string.Empty, string.Empty, string.Empty);
            foreach (var item in _fieldMapping)
            {
                if (item == ProductFields.Sku)
                    writer.WriteField(product.ArtNo);
                if (item == ProductFields.Name)
                    writer.WriteField(product.Name);

                if (item == ProductFields.ParamSynonym)
                    writer.WriteField(product.UrlPath);

                if (item == ProductFields.Category)
                    writer.WriteField((CategoryService.GetCategoryStringByProductId(product.ProductId, _columSeparator)));

                if (item == ProductFields.Sorting)
                    writer.WriteField((CategoryService.GetCategoryStringByProductId(product.ProductId, _columSeparator, true)));

                if (item == ProductFields.Enabled)
                    writer.WriteField(product.Enabled ? "+" : "-");

                if (!product.HasMultiOffer)
                {
                    var offer = product.Offers.FirstOrDefault() ?? new Offer();
                    if (item == ProductFields.Price)
                        writer.WriteField(offer.Price.ToString("F2"));
                    if (item == ProductFields.PurchasePrice)
                        writer.WriteField(offer.SupplyPrice.ToString("F2"));
                    if (item == ProductFields.Amount)
                        writer.WriteField(offer.Amount.ToString(CultureInfo.InvariantCulture));

                    if (item == ProductFields.MultiOffer)
                        writer.WriteField(string.Empty);
                }
                else
                {
                    if (item == ProductFields.Price)
                        writer.WriteField(string.Empty);
                    if (item == ProductFields.PurchasePrice)
                        writer.WriteField(string.Empty);
                    if (item == ProductFields.Amount)
                        writer.WriteField(string.Empty);
                    if (item == ProductFields.MultiOffer)
                        writer.WriteField(OfferService.OffersToString(product.Offers, _columSeparator, _propertySeparator));
                }

                if (item == ProductFields.Unit)
                    writer.WriteField(product.Unit);
                if (item == ProductFields.ShippingPrice)
                    writer.WriteField(product.ShippingPrice.ToString("F2"));
                if (item == ProductFields.Discount)
                    writer.WriteField(product.Discount.ToString("F2"));
                if (item == ProductFields.Weight)
                    writer.WriteField(product.Weight.ToString("F2"));
                if (item == ProductFields.Size)
                    writer.WriteField(product.Size.Replace("|", " x "));

                if (item == ProductFields.BriefDescription)
                    writer.WriteField(product.BriefDescription);
                if (item == ProductFields.Description)
                    writer.WriteField(product.Description);

                if (item == ProductFields.Title)
                    writer.WriteField(meta.Title);
                if (item == ProductFields.H1)
                    writer.WriteField(meta.H1);
                if (item == ProductFields.MetaKeywords)
                    writer.WriteField(meta.MetaKeywords);
                if (item == ProductFields.MetaDescription)
                    writer.WriteField(meta.MetaDescription);
                if (item == ProductFields.Markers)
                    writer.WriteField(ProductService.MarkersToString(product, _columSeparator));
                if (item == ProductFields.Photos)
                    writer.WriteField(PhotoService.PhotoToString(product.ProductPhotos, _columSeparator, _propertySeparator));
                if (item == ProductFields.Videos)
                    writer.WriteField(ProductVideoService.VideoToString(product.ProductVideos, _columSeparator));
                if (item == ProductFields.Properties)
                    writer.WriteField(PropertyService.PropertiesToString(product.ProductPropertyValues, _columSeparator, _propertySeparator));

                if (item == ProductFields.Producer)
                    writer.WriteField(BrandService.BrandToString(product.BrandId));

                if (item == ProductFields.OrderByRequest)
                    writer.WriteField(product.AllowPreOrder ? "+" : "-");

                if (item == ProductFields.SalesNotes)
                    writer.WriteField(product.SalesNote);

                if (item == ProductFields.Related)
                    writer.WriteField(ProductService.LinkedProductToString(product.ProductId, RelatedType.Related, _columSeparator));

                if (item == ProductFields.Alternative)
                    writer.WriteField(ProductService.LinkedProductToString(product.ProductId, RelatedType.Alternative, _columSeparator));

                if (item == ProductFields.CustomOption)
                    writer.WriteField(CustomOptionsService.CustomOptionsToString(CustomOptionsService.GetCustomOptionsByProductId(product.ProductId)));

                if (item == ProductFields.Gtin)
                    writer.WriteField(product.Gtin);

                if (item == ProductFields.GoogleProductCategory)
                    writer.WriteField(product.GoogleProductCategory);

                if (item == ProductFields.Adult)
                    writer.WriteField(product.Adult ? "+" : "-");

                if (item == ProductFields.ManufacturerWarranty)
                    writer.WriteField(product.ManufacturerWarranty ? "+" : "-");
            }
            writer.NextRecord();
        }

        public void Process(bool inBackGround = true, ProductCsvFilterModel model = null)
        {
            if (model == null)
                model = new ProductCsvFilterModel()
                {
                    ModuleName = "CsvExport",
                    ExportNoInCategory = _csvExportNoInCategory
                };

            CommonStatistic.TotalRow =
                ProductService.GetCsvProdutsCount(model);

            CommonStatistic.StartNew(() =>
            {
                try
                {
                    SaveProductsToCsv(model);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    CommonStatistic.WriteLog(ex.Message);
                }
                CommonStatistic.IsRun = false;
            }, inBackGround);
        }
    }
}