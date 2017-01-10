using System;
using System.Linq;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.UrlRewriter;

//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

public partial class UserControls_MenuCatalog_Social : System.Web.UI.UserControl
{
    private int _selectedCategoryId;
    private const int ItemsPerCol = 9;


    protected void Page_Load(object sender, EventArgs e)
    {
        int currentCategory;
        if (Int32.TryParse(Request["categoryid"], out currentCategory))
        {
            var rootcats = CategoryService.GetParentCategories(currentCategory);
            if (rootcats.Count > 0)
            {
                _selectedCategoryId = rootcats.Last().CategoryId;
            }
        }
    }


    public string GetMenu()
    {
        var cacheName = "MenuCatalogSocial" + _selectedCategoryId;
        //if (CacheManager.Contains(cacheName))
        //{
        //    return CacheManager.Get<string>(cacheName);
        //}

        var result = new StringBuilder();

        var rootCategories = CategoryService.GetChildCategoriesByCategoryIdForMenu(0).Where(cat => cat.Enabled).ToList();
        for (int rootIndex = 0; rootIndex < rootCategories.Count; ++rootIndex)
        {
            //ѕункт в главном меню
            result.AppendFormat("<div class=\"{0}\"><div class=\"tree-item-inside\">", rootCategories[rootIndex].CategoryId == _selectedCategoryId ? "tree-item-selected" : "tree-item");


            result.AppendFormat("<a href=\"{0}\" class=\"{1}\">{2}</a>", "social/catalogsocial.aspx?categoryID=" + rootCategories[rootIndex].CategoryId,
                                    rootCategories[rootIndex].HasChild ? "tree-item-link tree-parent" : "tree-item-link", rootCategories[rootIndex].Name);

            if (rootCategories[rootIndex].HasChild)
            {
                //Ќачало подменю
                result.Append("<div class=\"tree-submenu\">\r\n");
                var children = CategoryService.GetChildCategoriesByCategoryId(rootCategories[rootIndex].CategoryId, true).Where(cat => cat.Enabled).ToList();

                if (rootCategories[rootIndex].DisplaySubCategoriesInMenu)
                {
                    //раздел категорий
                    result.Append("<div class=\"tree-submenu-category\">");
                    for (int i = 0; i < children.Count; ++i)
                    {
                        //колонка категорий
                        result.Append("<div class=\"tree-submenu-column\">");

                        //1 уровень
                        result.AppendFormat("<span class=\"title-column\"><a href=\"{0}\">{1}</a></span>", "social/catalogsocial.aspx?categoryID=" + children[i].CategoryId, children[i].Name);
                        result.AppendFormat("<div class=\"tree-submenu-children\">");
                        //2 уровень
                        var subchildren = CategoryService.GetChildCategoriesByCategoryId(children[i].CategoryId, true).Where(cat => cat.Enabled).ToList();

                        for (int j = 0; j < subchildren.Count && j < 10; j++)
                        {
                            result.AppendFormat("<a href=\"{0}\">{1}</a>", "social/catalogsocial.aspx?categoryID=" + subchildren[j].CategoryId, subchildren[j].Name);
                        }
                        if (subchildren.Count > 10)
                        {
                            result.AppendFormat("<a href=\"{0}\">{1}</a>", "social/catalogsocial.aspx?categoryID=" + children[i].CategoryId, Resources.Resource.Client_MasterPage_ViewMore);
                        }
                        result.AppendFormat("</div>");

                        // олонка категорий закрываетс€
                        result.Append("</div>");

                        int columns = rootCategories[rootIndex].DisplayBrandsInMenu ? 4 : 5;
                        if (i % columns == columns - 1)
                        {
                            result.Append("<br class=\"clear\" />");
                        }
                    }
                    //раздел категорий закрываетс€
                    result.Append("</div>\r\n");

                    //раздел производителей
                    //if (rootCategories[rootIndex].DisplayBrandsInMenu)
                    //{
                    //    var brands = BrandService.GetBrandsByCategoryID(rootCategories[rootIndex].CategoryId, true);
                    //    if (brands.Count > 0)
                    //    {
                    //        result.Append("<div class=\"tree-submenu-brand\">");
                    //        result.AppendFormat("<div class=\"title-column\">{0}</div>", Resources.Resource.Client_MasterPage_Brands);

                    //        result.Append("<div class=\"tree-submenu-column\">");
                    //        foreach (Brand br in brands)
                    //        {
                    //            result.AppendFormat("<a href=\"{0}\">{1}</a>", UrlService.GetLink(ParamType.Brand, br.UrlPath, br.BrandId), br.Name);
                    //        }
                    //        result.AppendFormat("</div>");

                    //        result.AppendFormat("</div>");
                    //    }
                    //}
                }
                else
                {
                    //раздел категорий
                    result.Append("<div class=\"tree-submenu-category\">");
                    for (int i = 0; i < children.Count; ++i)
                    {
                        //колонка категорий
                        if (i % ItemsPerCol == 0)
                        {
                            result.Append("<div class=\"tree-submenu-column\">");
                        }
                        result.AppendFormat("<a href=\"{0}\">{1}</a>", "social/catalogsocial.aspx?categoryID=" + children[i].CategoryId, children[i].Name);

                        // олонка категорий закрываетс€
                        if (i % ItemsPerCol == ItemsPerCol - 1 || i == children.Count - 1)
                        {
                            result.Append("</div>");
                        }

                    }
                    //раздел категорий закрываетс€
                    result.Append("</div>\r\n");

                    //раздел производителей
                    //if (rootCategories[rootIndex].DisplayBrandsInMenu)
                    //{
                    //    var brands = BrandService.GetBrandsByCategoryID(rootCategories[rootIndex].CategoryId, true);
                    //    if (brands.Count > 0)
                    //    {
                    //        result.Append("<div class=\"tree-submenu-brand\">");
                    //        result.AppendFormat("<div class=\"title-column\">{0}</div>", Resources.Resource.Client_MasterPage_Brands);

                    //        result.Append("<div class=\"tree-submenu-column\">");
                    //        foreach (Brand br in brands)
                    //        {
                    //            result.AppendFormat("<a href=\"{0}\">{1}</a>", UrlService.GetLink(ParamType.Brand, br.UrlPath, br.BrandId), br.Name);
                    //        }
                    //        result.AppendFormat("</div>");

                    //        result.AppendFormat("</div>");
                    //    }
                    //}
                }
                //ѕодменю закрываетс€
                result.AppendFormat("</div>");
            }

            //ѕункт в главном меню закрываетс€
            result.AppendFormat("</div></div>");

            //spliter
            if (rootIndex != rootCategories.Count - 1)
            {
                result.AppendFormat("<div class=\"tree-item-split\"></div>");
            }
        }

        var resultString = result.ToString();
        CacheManager.Insert<string>(cacheName, resultString);
        return resultString;
    }
}
