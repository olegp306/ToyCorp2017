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
    public class ExportFeedModuleShoppingCom : ExportFeedModule
    {
        private string _description;

        public ExportFeedModuleShoppingCom()
        {
            //GetSelectedCategories("ShoppingCom");
        }

        protected override string ModuleName
        {
            get { return "ShoppingCom"; }
        }

        public override void GetExportFeedString(string file)
        {
            _description = ExportFeed.GetModuleSetting(ModuleName, "DescriptionSelection");

            using (var s = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var memoryBuffer = new StreamWriter(s, Encoding.UTF8))
                {
                    memoryBuffer.Write("MPN");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("Manufacturer Name");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("UPC");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("Product Name");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("Product Description");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("  Product Price  ");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("Product URL");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("Image URL ");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("Shopping.com Categorization");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("Stock Availability");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("Stock Description");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write(" Ground Shipping  ");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write(" Weight ");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write("Zip Code");
                    memoryBuffer.Write(",");

                    memoryBuffer.Write(" Condition ");
                    memoryBuffer.Write("\n");

                    CommonStatistic.TotalRow =  GetProdutsCount(ModuleName);
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
            memoryBuffer.Write("\"");
            //MPN
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //Manufacturer name
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //UPC
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //Product Name
            memoryBuffer.Write(row.Name);
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //Product Description
            string desc = _description == "full" ? row.Description : row.BriefDescription;

            memoryBuffer.Write(!string.IsNullOrEmpty(desc) ? desc : Resource.ExportFeed_NoDescription);
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //Product Price
            var nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";
            memoryBuffer.Write(CatalogService.CalculatePrice(row.Price, row.Discount).ToString(nfi));
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //Product URL
            memoryBuffer.Write(ShopUrl.TrimEnd('/') + "/" + UrlService.GetLink(ParamType.Product, row.UrlPath, row.ProductId));
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");
            memoryBuffer.Write("\"");
            //Image URL
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
            //Shopping.com Categorization
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
            //Stock Availability
            //memoryBuffer.Write(SQLDataHelper.GetBoolean(row["Enabled"]));
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //Stock Description
            memoryBuffer.Write("shopping");
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //Ground Shipping
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //Weight
            //memoryBuffer.Write(SQLDataHelper.GetDecimal(row["Weight"]));
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //Zip Code
            memoryBuffer.Write("\"");

            memoryBuffer.Write(",");

            memoryBuffer.Write("\"");
            //Condition
            memoryBuffer.Write("New");
            memoryBuffer.Write("\"");

            memoryBuffer.Write("\n");
        }
    }
}