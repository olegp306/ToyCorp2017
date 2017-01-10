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
    public class ExportFeedModuleYahooShopping : ExportFeedModule
    {
        private string _description;

        protected override string ModuleName
        {
            get { return "YahooShopping"; }
        }

        public override void GetExportFeedString(string  file)
        {
            _description = ExportFeed.GetModuleSetting(ModuleName, "DescriptionSelection");
            
            using (var s = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var memoryBuffer = new StreamWriter(s, Encoding.UTF8))
                {
                    memoryBuffer.Write("code");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("name");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("description");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("price");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("product-url");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("merchant-site-category");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("medium");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("image-url");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("upc");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("manufacturer");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("manufacturer-part-no");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("msrp");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("in-stock");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("shipping-price");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("shipping-weight");
                    memoryBuffer.Write("\t");

                    memoryBuffer.Write("\n");

                    CommonStatistic.TotalRow = GetProdutsCount(ModuleName);
                    foreach (var productRow in GetProduts(ModuleName))
                    {
                        ProcessProductRow(productRow, memoryBuffer);
                        CommonStatistic.RowPosition++;
                    }

                    memoryBuffer.Flush();
                }
            }
        }

        private void ProcessProductRow(ExportFeedProduts row, StreamWriter memoryBuffer)
        {
            memoryBuffer.Write(row.ProductId.ToString());
            memoryBuffer.Write("\t");

            memoryBuffer.Write(row.Name);
            memoryBuffer.Write("\t");

            string desc = _description == "full" ? row.Description : row.BriefDescription;

            memoryBuffer.Write(!string.IsNullOrEmpty(desc) ? desc : Resource.ExportFeed_NoDescription);
            memoryBuffer.Write("\t");

            var nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";
            memoryBuffer.Write(CatalogService.CalculatePrice(row.Price, row.Discount).ToString(nfi));
            memoryBuffer.Write("\t");

            memoryBuffer.Write(ShopUrl.TrimEnd('/') + "/" + UrlService.GetLink(ParamType.Product, row.UrlPath, row.ProductId));
            memoryBuffer.Write("\t");

            var categorizationBuffer = new StringBuilder();
            Category category = CategoryService.GetCategory(row.ParentCategory);
            categorizationBuffer.Insert(0, category.Name);
            while (category.ParentCategoryId != 0)
            {
                category = CategoryService.GetCategory(category.ParentCategoryId);
                categorizationBuffer.Insert(0, category.Name + " >> ");
            }
            memoryBuffer.Write(categorizationBuffer.ToString());
            memoryBuffer.Write("\t");

            memoryBuffer.Write("None");
            memoryBuffer.Write("\t");
            //Image URL
            if (!string.IsNullOrEmpty(row.Photos))
            {
                var temp = row.Photos.Split(',');
                var item = temp.FirstOrDefault();
                if (!string.IsNullOrEmpty(item))
                    memoryBuffer.Write(GetImageProductPath(item));
            }
            memoryBuffer.Write("\t");

            memoryBuffer.Write("\t");

            memoryBuffer.Write("\t");

            memoryBuffer.Write("\t");

            memoryBuffer.Write(row.Price.ToString(nfi));
            memoryBuffer.Write("\t");

            //memoryBuffer.Write(SQLDataHelper.GetBoolean(row["Enabled"]));
            //memoryBuffer.Write("\t");

            //memoryBuffer.Write(SQLDataHelper.GetDecimal(row["ShippingPrice"]).ToString(nfi));
            //memoryBuffer.Write("\t");

            //memoryBuffer.Write(SQLDataHelper.GetDecimal(row["Weight"]));
            //memoryBuffer.Write("\n");
        }
    }
}