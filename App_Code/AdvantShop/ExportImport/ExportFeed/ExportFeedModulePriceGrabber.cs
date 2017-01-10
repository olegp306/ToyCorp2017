//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Statistic;
using Resources;


namespace AdvantShop.ExportImport
{
    public class ExportFeedModulePriceGrabber : ExportFeedModule
    {
        private string _descriptionSelecton;

        protected override string ModuleName
        {
            get { return "PriceGrabber"; }
        }

        public override void GetExportFeedString(string file)
        {
            _descriptionSelecton = ExportFeed.GetModuleSetting(ModuleName, "DescriptionSelection");
            using (var s = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var memoryBuffer = new StreamWriter(s, Encoding.UTF8))
                {
                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Unique Retailer SKU (RETSKU)");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Manufacturer Name");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Manufacturer Part Number (MPN)");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Product Title");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Categorization");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Product URL");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Image URL");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Detailed Description");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Selling Price");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Product Condition");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Availability");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("UPC");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Shipping Costs");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write(",");

                    memoryBuffer.Write("\"");
                    memoryBuffer.Write("Weight");
                    memoryBuffer.Write("\"");

                    memoryBuffer.Write("\n");

                    CommonStatistic.TotalRow = GetProdutsCount(ModuleName);
                    foreach (var productRow in GetProduts(ModuleName))
                    {
                        ProcessDataRow(productRow, memoryBuffer);
                        CommonStatistic.RowPosition++;
                    }

                    memoryBuffer.Flush();
                }
            }
        }

        private void ProcessDataRow(ExportFeedProduts row, StreamWriter memoryBuffer)
        {
            var tempId = row.ProductId;
            memoryBuffer.Write("\"");
            memoryBuffer.Write(tempId);
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            memoryBuffer.Write("");
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            memoryBuffer.Write(row.Name);
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            var categorizationBuffer = new StringBuilder();
            Category category = CategoryService.GetCategory(row.ParentCategory);
            categorizationBuffer.Insert(0, category.Name);
            while (category.ParentCategoryId != 0)
            {
                category = CategoryService.GetCategory(category.ParentCategoryId);
                categorizationBuffer.Insert(0, category.Name + " >> ");
            }
            memoryBuffer.Write(categorizationBuffer.ToString());
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            memoryBuffer.Write(ShopUrl.TrimEnd('/') + "/" + UrlService.GetLink(ParamType.Product, row.UrlPath, row.ProductId));
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            //memoryBuffer.Write("\"");
            //memoryBuffer.Write(GetShopUrl() + GetProductLink(tempId));
            //memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            
            if (!string.IsNullOrEmpty(row.Photos))
            {
                var temp = row.Photos.Split(',');
                var item = temp.FirstOrDefault();
                if (!string.IsNullOrEmpty(item))
                    memoryBuffer.Write(GetImageProductPath(item));
            }

            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            string desc = _descriptionSelecton == "full" ? row.Description : row.BriefDescription;

            memoryBuffer.Write(!string.IsNullOrEmpty(desc) ? desc : Resource.ExportFeed_NoDescription);
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            var nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";
            memoryBuffer.Write(CatalogService.CalculatePrice(row.Price, row.Discount).ToString(nfi));
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            memoryBuffer.Write("New");
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            //memoryBuffer.Write("\"");
            //memoryBuffer.Write(row["Enabled"]);
            //memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            //memoryBuffer.Write("\"");
            //memoryBuffer.Write(SQLDataHelper.GetDecimal(row["ShippingPrice"]).ToString(nfi));
            //memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            //memoryBuffer.Write("\"");
            //memoryBuffer.Write(SQLDataHelper.GetDecimal(row["Weight"]));
            //memoryBuffer.Write("\"");

            memoryBuffer.Write("\n");
        }
    }
}