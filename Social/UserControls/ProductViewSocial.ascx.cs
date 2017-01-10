//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.FilePath;

namespace Social.UserControls
{
    public partial class ProductView : UserControl
    {

        public object DataSource { set; get; }
        public SettingsCatalog.ProductViewMode ViewMode { set; get; }
        public bool HasProducts { private set; get; }

        protected bool EnableRating = SettingsCatalog.EnableProductRating;
        protected bool EnableCompare = SettingsCatalog.EnableCompareProducts;
        protected CustomerGroup customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;
        protected int ImageWidth = SettingsPictureSize.SmallProductImageWidth;
        protected int ImageHeightSmall = SettingsPictureSize.SmallProductImageHeight;
        protected int ImageHeightXSmall = SettingsPictureSize.XSmallProductImageHeight;

        protected int ColorImageHeight = SettingsPictureSize.ColorIconHeightCatalog;
        protected int ColorImageWidth = SettingsPictureSize.ColorIconWidthCatalog;

        protected string RenderPictureTag(string urlPhoto, string productName, string urlPath)
        {
            string strFormat = string.Empty;

            switch (ViewMode)
            {
                case SettingsCatalog.ProductViewMode.Tiles:
                    strFormat = string.Format("<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\"  class=\"scp-img p-photo\" /></a>", urlPath,
                                              FoldersHelper.GetImageProductPath(ProductImageType.Small, urlPhoto, false), productName);
                    break;
                case SettingsCatalog.ProductViewMode.List:
                    strFormat = string.Format("<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\"  class=\"scp-img p-photo\" /></a>", urlPath,
                                              FoldersHelper.GetImageProductPath(ProductImageType.Small, urlPhoto, false), productName);
                    break;
                case SettingsCatalog.ProductViewMode.Table:
                    if (urlPhoto.IsNotEmpty())
                    {
                        strFormat = string.Format("abbr=\"{0}\"",
                                                  FoldersHelper.GetImageProductPath(ProductImageType.Small, urlPhoto, false));
                    }
                    break;
            }
            return strFormat;
        }

        protected string RenderPriceTag(float price, float discount, float multiPrice)
        {
            if (multiPrice == 0)
            {
                return CatalogService.RenderPrice(price, discount, false, customerGroup);
            }
            else
            {
                return Resources.Resource.Client_Catalog_From + " " + CatalogService.RenderPrice(price, discount, false, customerGroup);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            switch (ViewMode)
            {
                case SettingsCatalog.ProductViewMode.Tiles:
                    mvProducts.SetActiveView(viewTile);
                    lvTile.DataSource = DataSource;
                    lvTile.DataBind();
                    HasProducts = lvTile.Items.Any();
                    break;
                case SettingsCatalog.ProductViewMode.List:
                    mvProducts.SetActiveView(viewList);
                    lvList.DataSource = DataSource;
                    lvList.DataBind();
                    HasProducts = lvList.Items.Any();
                    break;

                case SettingsCatalog.ProductViewMode.Table:
                    mvProducts.SetActiveView(viewTable);
                    lvTable.DataSource = DataSource;
                    lvTable.DataBind();
                    HasProducts = lvTable.Items.Any();
                    break;
            }
        }
    }
}