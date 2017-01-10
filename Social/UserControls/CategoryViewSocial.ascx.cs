//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.FilePath;

namespace Social.UserControls
{
    public partial class CategoryView: UserControl
    {
        public int CategoryID { set; get; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var categories = CategoryService.GetChildCategoriesByCategoryId(CategoryID, false).Where(cat=> cat.Enabled);
            lvCategory.DataSource = categories;
            lvCategory.DataBind();
        }

        protected string RenderCategoryImage(string imageUrl, int categoryId, string urlPath, string categoryName)
        {
            if (imageUrl.IsNullOrEmpty())
                return string.Empty;
            return string.Format("<a href=\"{0}\"><img src=\"{1}\" class=\"cat-image\" alt=\"{2}\" /></a>",
                                 "social/catalogsocial.aspx?categoryid=" + categoryId,
                                 FoldersHelper.GetImageCategoryPath(CategoryImageType.Small , imageUrl, false),
                                 categoryName);
        }
    }
}