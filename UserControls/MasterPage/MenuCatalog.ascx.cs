//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;


namespace UserControls.MasterPage
{
    public partial class MenuCatalog : System.Web.UI.UserControl
    {
        #region Fields

        private int _selectedCategoryId;

        private const int ItemsPerCol = 9;
        private const string MenuCatalogCacheKey = "MenuCatalog";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            var productId = Request["productid"].TryParseInt();
            var currentCategory = 0;
            if (productId != 0)
            {
                var firstCategory = ProductService.GetCategoriesByProductId(productId).FirstOrDefault();
                if (firstCategory != null)
                    currentCategory = firstCategory.CategoryId;
            }
            else
            {
                currentCategory= Request["categoryid"].TryParseInt();
            }

            if (currentCategory != 0)
            {
                var rootcats = CategoryService.GetParentCategories(currentCategory);
                if (rootcats.Count > 0)
                {
                    _selectedCategoryId = rootcats.Last().CategoryId;
                }
            }
            searchBlock.Visible = SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.CatalogMenu;
        }

        protected string GetMenu()
        {
            var result = new StringBuilder();

            var menuItems = GetCategoryMenuItems();

            var rootIndex = 0;
            foreach (var rootCategory in menuItems.SubCategories)
            {
                //ѕункт в главном меню
                result.AppendFormat("<div class=\"{0}\"><div class=\"tree-item-inside\">",
                    rootCategory.CategoryId == _selectedCategoryId ? "tree-item-selected" : "tree-item");

                result.AppendFormat("<a href=\"{0}\" class=\"{1}\">{3}{2}</a>",
                    UrlService.GetLink(ParamType.Category, rootCategory.UrlPath, rootCategory.CategoryId),
                    rootCategory.HasChild ? "tree-item-link tree-parent" : "tree-item-link", rootCategory.Name,
                    rootCategory.IconPath.IsNotEmpty() ? string.Format("<img class='menu-icon' src='{0}' />", FoldersHelper.GetImageCategoryPath(CategoryImageType.Icon, rootCategory.IconPath, false)) : "");

                if (rootCategory.HasChild)
                {
                    //Ќачало подменю
                    result.Append("<div class=\"tree-submenu\"><div class=\"container\">\r\n");

                    if (rootCategory.DisplaySubCategoriesInMenu)
                    {
                        //раздел категорий
                        result.Append("<div class=\"tree-submenu-category\">");

                        int j = 0;
                        foreach (var subCategory in rootCategory.SubCategories)
                        {
                            //колонка категорий
                            result.Append("<div class=\"tree-submenu-column\">");

                            //1 уровень
                            result.AppendFormat("<span class=\"title-column\"><a href=\"{0}\">{1}</a></span>",
                                UrlService.GetLink(ParamType.Category, subCategory.UrlPath, subCategory.CategoryId),
                                subCategory.Name);

                            result.AppendFormat("<div class=\"tree-submenu-children\">");

                            //2 уровень
                            foreach (var childrenCategory in subCategory.SubCategories)
                            {
                                result.AppendFormat("<a href=\"{0}\">{1}</a>",
                                    UrlService.GetLink(ParamType.Category, childrenCategory.UrlPath, childrenCategory.CategoryId),
                                    childrenCategory.Name);
                            }

                            if (subCategory.SubCategories.Count > 10)
                            {
                                result.AppendFormat("<a href=\"{0}\">{1}</a>",
                                    UrlService.GetLink(ParamType.Category, subCategory.UrlPath, subCategory.CategoryId),
                                    Resources.Resource.Client_MasterPage_ViewMore);
                            }
                            result.AppendFormat("</div>");

                            // олонка категорий закрываетс€
                            result.Append("</div>");

                            var columns = rootCategory.DisplayBrandsInMenu ? 4 : 5;
                            if (j++ % columns == columns - 1)
                            {
                                result.Append("<br class=\"clear\" />");
                            }
                        }
                        //раздел категорий закрываетс€
                        result.Append("</div>\r\n");

                        //раздел производителей
                        if (rootCategory.DisplayBrandsInMenu)
                        {
                            result.Append(RenderBrands(rootCategory.Brands));
                        }
                    }
                    else
                    {
                        //раздел категорий
						result.Append("<div class=\"tree-submenu-category\">");
                        
                        int i = 0;
                        foreach (var subCategory in rootCategory.SubCategories)
                        {
                            //колонка категорий
                            if (i % ItemsPerCol == 0)
                            {
                                result.Append("<div class=\"tree-submenu-column\">");
                            }
                            result.AppendFormat("<a href=\"{0}\">{1}</a>",
                                UrlService.GetLink(ParamType.Category, subCategory.UrlPath, subCategory.CategoryId),
                                subCategory.Name);

                            // олонка категорий закрываетс€
                            if (i % ItemsPerCol == ItemsPerCol - 1 || i == rootCategory.SubCategories.Count - 1)
                            {
                                result.Append("</div>");
                            }
                            i++;
                        }
                        //раздел категорий закрываетс€
                        result.Append("</div>\r\n");

                        //раздел производителей
                        if (rootCategory.DisplayBrandsInMenu)
                        {
                            result.Append(RenderBrands(rootCategory.Brands));
                        }
                    }

                    //ѕодменю закрываетс€

                    result.AppendFormat("</div>{0}</div>",
                        rootCategory.CategoryPicturePath.IsNotEmpty()
                            ? string.Format("<div class=\"tree-submenu-image\"><img src='{0}' /></div>",
                                FoldersHelper.GetImageCategoryPath(CategoryImageType.Big, rootCategory.CategoryPicturePath, false))
                            : "");
                }

                //ѕункт в главном меню закрываетс€
                result.AppendFormat("</div></div>");

                //spliter
                if (rootIndex++ != menuItems.SubCategories.Count - 1)
                {
                    result.AppendFormat("<div class=\"tree-item-split\"></div>");
                }
            }

            return result.ToString();
        }

        private string RenderBrands(List<Brand> brands)
        {
            if (brands.Count == 0)
                return string.Empty;

            var result = new StringBuilder();

            result.Append("<div class=\"tree-submenu-brand\">");
            result.AppendFormat("<div class=\"title-column\">{0}</div>",
                Resources.Resource.Client_MasterPage_Brands);

            if (brands.Count <= 10)
            {
                result.Append("<div class=\"tree-submenu-column\">");
                foreach (var brand in brands)
                {
                    result.AppendFormat("<a href=\"{0}\">{1}</a>",
                        UrlService.GetLink(ParamType.Brand, brand.UrlPath, brand.BrandId), brand.Name);
                }
                result.AppendFormat("</div>");
            }
            else
            {
                int border = brands.Count / 2 + brands.Count % 2;

                //first column
                result.Append("<div class=\"tree-submenu-column\">");
                for (int i = 0; i < border; i++)
                {
                    result.AppendFormat("<a href=\"{0}\">{1}</a>",
                        UrlService.GetLink(ParamType.Brand, brands[i].UrlPath, brands[i].BrandId),
                        brands[i].Name);
                }
                result.AppendFormat("</div>");

                //second column
                result.Append("<div class=\"tree-submenu-column\">");
                for (int i = border; i < brands.Count; i++)
                {
                    result.AppendFormat("<a href=\"{0}\">{1}</a>",
                        UrlService.GetLink(ParamType.Brand, brands[i].UrlPath, brands[i].BrandId),
                        brands[i].Name);
                }
                result.AppendFormat("</div>");
            }
            result.AppendFormat("</div>");

            return result.ToString();
        }

        private CategoryMenuItem GetCategoryMenuItems()
        {
            if (CacheManager.Contains(MenuCatalogCacheKey))
                return CacheManager.Get<CategoryMenuItem>(MenuCatalogCacheKey);

            var menuItems = new CategoryMenuItem() {SubCategories = GetRootSubcategories()};

            foreach (var subCategory in menuItems.SubCategories)
            {
                if (subCategory.HasChild)
                {
                    subCategory.SubCategories = GetSubcategories(subCategory.CategoryId);

                    if (subCategory.DisplaySubCategoriesInMenu)
                    {
                        foreach (var subChildrenCategories in subCategory.SubCategories)
                        {
                            subChildrenCategories.SubCategories = GetSubcategories(subChildrenCategories.CategoryId);
                        }
                    }

                    if (subCategory.DisplayBrandsInMenu)
                    {
                        subCategory.Brands = BrandService.GetBrandsByCategoryID(subCategory.CategoryId, true);
                    }
                }
            }

            CacheManager.Insert(MenuCatalogCacheKey, menuItems);
            return menuItems;
        }

        private List<CategoryMenuItem> GetSubcategories(int categoryId)
        {
            return
                CategoryService.GetChildCategoriesByCategoryId(categoryId, true)
                    .Where(cat => cat.Enabled)
                    .Select(c => new CategoryMenuItem()
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        UrlPath = c.UrlPath,
                        HasChild = c.HasChild,
                        DisplayBrandsInMenu = c.DisplayBrandsInMenu,
                        DisplaySubCategoriesInMenu = c.DisplaySubCategoriesInMenu,
                    }).ToList();
        }

        private List<CategoryMenuItem> GetRootSubcategories()
        {
            return
                CategoryService.GetChildCategoriesByCategoryIdForMenu(0)
                    .Select(c => new CategoryMenuItem()
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        UrlPath = c.UrlPath,
                        HasChild = c.HasChild,
                        DisplayBrandsInMenu = c.DisplayBrandsInMenu,
                        DisplaySubCategoriesInMenu = c.DisplaySubCategoriesInMenu,
                        IconPath = c.Icon != null ? c.Icon.PhotoName : "",
                        CategoryPicturePath = c.Picture != null ? c.Picture.PhotoName : ""
                    }).ToList();
        }
    }
}
