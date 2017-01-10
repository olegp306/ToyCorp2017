using System.Linq;
using System.Web.UI;
using System.Text;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Core.Caching;
using AdvantShop.FilePath;

namespace UserControls.MasterPage
{
    public partial class MenuCatalogAlternative : UserControl
    {
        private const int ItemsPerCol = 10;
        private const string MenuCatalogCacheKey = "MenuCatalog_Alternative";

        protected string GetMenu()
        {
            if (CacheManager.Contains(MenuCatalogCacheKey))
                return CacheManager.Get<string>(MenuCatalogCacheKey);

            var result = new StringBuilder();

            foreach (var rootItem in CategoryService.GetChildCategoriesByCategoryIdForMenu(0).Where(cat => cat.Enabled))
            {
                //Пункт в главном меню
                result.AppendFormat("<li class=\"item{0}\"><a href=\"{1}\" class=\"lnk-item\">{3}<span class='tbl tbl-text'>{2}</span></a>", rootItem.HasChild ? " parent" : "",
                                    UrlService.GetLink(ParamType.Category, rootItem.UrlPath, rootItem.CategoryId), rootItem.Name,
                                    rootItem.Icon != null && rootItem.Icon.PhotoName.IsNotEmpty()
                                                    ? string.Format("<span class='tbl'><img src='{0}' class='menu-icon' /></span>",
                                                                    FoldersHelper.GetImageCategoryPath(CategoryImageType.Icon, rootItem.Icon.PhotoName, false))
                                                    : "");

                if (rootItem.HasChild)
                {
                    result.AppendLine("<div class=\"tree-submenu\">");

                    var children = CategoryService.GetChildCategoriesByCategoryId(rootItem.CategoryId, false).Where(cat => cat.Enabled).ToList();

                    if (rootItem.DisplaySubCategoriesInMenu)
                    {
                        //раздел категорий
                        result.Append("<div class=\"tree-submenu-category\">");
                        for (int i = 0; i < children.Count; ++i)
                        {
                            //колонка категорий
                            result.Append("<div class=\"tree-submenu-column\">");

                            //1 уровень
                            result.AppendFormat("<span class=\"title-column\"><a href=\"{0}\">{1}</a></span>",
                                                UrlService.GetLink(ParamType.Category, children[i].UrlPath,
                                                                   children[i].CategoryId), children[i].Name);
                            result.AppendFormat("<div class=\"tree-submenu-children\">");
                            //2 уровень
                            var subchildren = CategoryService.GetChildCategoriesByCategoryId(children[i].CategoryId, false).Where(cat => cat.Enabled).ToList();

                            for (int j = 0; j < subchildren.Count && j < 10; j++)
                            {
                                result.AppendFormat("<a href=\"{0}\">{1}</a>", UrlService.GetLink(ParamType.Category, subchildren[j].UrlPath, subchildren[j].CategoryId), subchildren[j].Name);
                            }
                            if (subchildren.Count > 10)
                            {
                                result.AppendFormat("<a href=\"{0}\">{1}</a>", UrlService.GetLink(ParamType.Category, children[i].UrlPath, children[i].CategoryId), Resources.Resource.Client_MasterPage_ViewMore);
                            }
                            result.AppendFormat("</div>");

                            //Колонка категорий закрывается
                            result.Append("</div>");

                            int columns = rootItem.DisplayBrandsInMenu ? 3 : 4;
                            if (i % columns == columns - 1)
                            {
                                result.Append("<br class=\"clear\" />");
                            }
                        }
                        //раздел категорий закрывается
                        result.Append("</div>\r\n");

                        //раздел производителей
                        if (rootItem.DisplayBrandsInMenu)
                        {
                            var brands = BrandService.GetBrandsByCategoryID(rootItem.CategoryId, true);
                            if (brands.Count > 0)
                            {
                                result.Append("<div class=\"tree-submenu-brand\">");
                                result.AppendFormat("<div class=\"title-column\">{0}</div>", Resources.Resource.Client_MasterPage_Brands);

                                result.Append("<div class=\"tree-submenu-column\">");
                                foreach (Brand br in brands)
                                {
                                    result.AppendFormat("<a href=\"{0}\">{1}</a>", UrlService.GetLink(ParamType.Brand, br.UrlPath, br.BrandId), br.Name);
                                }
                                result.AppendFormat("</div>");

                                result.AppendFormat("</div>");
                            }
                        }
                    }
                    else
                    {
                        int columnsCount = 0;
                        //раздел категорий
                        result.Append("<div class=\"tree-submenu-category\">");
                        for (int i = 0; i < children.Count; ++i)
                        {
                            //колонка категорий
                            if (i % ItemsPerCol == 0)
                            {
                                result.Append("<div class=\"tree-submenu-column\">");
                            }
                            result.AppendFormat("<a href=\"{0}\">{1}</a>", UrlService.GetLink(ParamType.Category, children[i].UrlPath, children[i].CategoryId), children[i].Name);

                            //Колонка категорий закрывается
                            if (i % ItemsPerCol == ItemsPerCol - 1 || i == children.Count - 1)
                            {
                                columnsCount++;
                                result.Append("</div>");
                                int columns = rootItem.DisplayBrandsInMenu ? 3 : 4;
                                if (columnsCount % columns == 0)
                                {
                                    result.Append("<br /><br />");
                                }
                            }

                        }
                        //раздел категорий закрывается
                        result.Append("</div>\r\n");

                        //раздел производителей
                        if (rootItem.DisplayBrandsInMenu)
                        {
                            var brands = BrandService.GetBrandsByCategoryID(rootItem.CategoryId, true);
                            if (brands.Count > 0)
                            {
                                result.Append("<div class=\"tree-submenu-brand\">");
                                result.AppendFormat("<div class=\"title-column\">{0}</div>", Resources.Resource.Client_MasterPage_Brands);

                                result.Append("<div class=\"tree-submenu-column\">");
                                foreach (Brand br in brands)
                                {
                                    result.AppendFormat("<a href=\"{0}\">{1}</a>", UrlService.GetLink(ParamType.Brand, br.UrlPath, br.BrandId), br.Name);
                                }
                                result.AppendFormat("</div>");

                                result.AppendFormat("</div>");
                            }
                        }
                    }

                    //Подменю закрывается
                    result.AppendLine("</div>");
                }

                //Пункт в главном меню закрывается
                result.AppendLine("</li>");
            }

            var resultString = result.ToString();
            CacheManager.Insert(MenuCatalogCacheKey, resultString);
            return resultString;
        }
    }
}