//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.CMS;
using AdvantShop.Customers;

namespace UserControls.Catalog
{
    public partial class CategoryView : UserControl
    {
        public int CategoryID { set; get; }
        protected bool DisplayProductsCount = SettingsCatalog.ShowProductsCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            var categories = CategoryService.GetChildCategoriesByCategoryId(CategoryID, false).Where(cat => cat.Enabled && cat.ParentsEnabled);
            lvCategory.DataSource = categories;
            lvCategory.DataBind();
        }

        protected string RenderCategoryImage(string imageUrl, int categoryId, string urlPath, string categoryName)
        {
            var result = string.Empty;


            if (imageUrl.IsNullOrEmpty() && !CustomerContext.CurrentCustomer.IsAdmin)
                return result;


            result = string.Format("<a class=\"cat-lnk\" href=\"{0}\"><img src=\"{1}\" class=\"cat-image {4}\" alt=\"{2}\" title=\"{2}\" {3}/></a>",
                                 UrlService.GetLink(ParamType.Category, urlPath, SQLDataHelper.GetInt(categoryId)),
                                 imageUrl.IsNullOrEmpty() ? "images/nophoto_small.jpg" : FoldersHelper.GetImageCategoryPath(CategoryImageType.Small, imageUrl, false),
                                 HttpUtility.HtmlEncode(categoryName),
                                 InplaceEditor.Image.AttributesCategory(categoryId, InplaceEditor.Image.Field.CategorySmall),
                                 InplaceEditor.CanUseInplace(RoleActionKey.DisplayCatalog) ? "js-inplace-image-visible-permanent" : "");


            return result;
        }
    }
}