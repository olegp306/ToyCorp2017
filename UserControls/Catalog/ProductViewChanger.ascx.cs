using System;
using AdvantShop.Configuration;
using AdvantShop.Helpers;

namespace UserControls.Catalog
{
    public partial class ProductViewChanger : System.Web.UI.UserControl
    {

        public SettingsCatalog.ProductViewMode CatalogViewMode { get; set; }
        public SettingsCatalog.ProductViewMode SearchViewMode { get; set; }
        public SettingsCatalog.ProductViewPage CurrentPage { get; set; }
        public static int CurrentViewMode { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session["ViewMode"]!=null)
            {
                CatalogViewMode = (SettingsCatalog.ProductViewMode)SQLDataHelper.GetInt(Session["ViewMode"]);
                SearchViewMode = (SettingsCatalog.ProductViewMode)SQLDataHelper.GetInt(Session["ViewMode"]);
            }else
            {
                CatalogViewMode = SettingsCatalog.DefaultCatalogView;
                SearchViewMode = SettingsCatalog.DefaultSearchView;
            }

            if ((CurrentPage == SettingsCatalog.ProductViewPage.Catalog && !SettingsCatalog.EnabledCatalogViewChange) || ((CurrentPage == SettingsCatalog.ProductViewPage.Search && !SettingsCatalog.EnabledSearchViewChange)))
            {
                Visible = false;
            }
        }

        protected bool IsSelectedView(SettingsCatalog.ProductViewMode view)
        {
            return (CurrentPage == SettingsCatalog.ProductViewPage.Catalog && CatalogViewMode == view) ||
                   (CurrentPage == SettingsCatalog.ProductViewPage.Search && SearchViewMode == view);
        }

        protected void lbTiles_Click(object sender, EventArgs e)
        {
            Session["ViewMode"] = SettingsCatalog.ProductViewMode.Tiles;
            CatalogViewMode = SettingsCatalog.ProductViewMode.Tiles;
            SearchViewMode = SettingsCatalog.ProductViewMode.Tiles;
        }

        protected void lbList_Click(object sender, EventArgs e)
        {
            Session["ViewMode"] = SettingsCatalog.ProductViewMode.List;
            CatalogViewMode = SettingsCatalog.ProductViewMode.List;
            SearchViewMode = SettingsCatalog.ProductViewMode.List;
        }

        protected void lbTable_Click(object sender, EventArgs e)
        {
            Session["ViewMode"] = SettingsCatalog.ProductViewMode.Table;
            CatalogViewMode = SettingsCatalog.ProductViewMode.Table;
            SearchViewMode = SettingsCatalog.ProductViewMode.Table;
        }
    }
}
