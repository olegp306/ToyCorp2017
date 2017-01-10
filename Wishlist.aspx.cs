using System;
using System.Linq;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using AdvantShop.Controls;
using AdvantShop.SEO;
using Resources;

namespace ClientPages
{

    public partial class Wishlist : AdvantShopClientPage
    {
        protected CustomerGroup customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;


        protected bool DisplayBuyButton = SettingsCatalog.DisplayBuyButton;
        protected bool DisplayMoreButton = SettingsCatalog.DisplayMoreButton;
        protected bool DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton;

        protected string BuyButtonText = SettingsCatalog.BuyButtonText;
        protected string MoreButtonText = SettingsCatalog.MoreButtonText;
        protected string PreOrderButtonText = SettingsCatalog.PreOrderButtonText;

        protected float DiscountByTime = DiscountByTimeService.GetDiscountByTime();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SettingsDesign.WishListVisibility)
                Response.Redirect("~/");

            SetMeta(new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_Wishlist_Header)),
                    string.Empty);
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            lvList.DataSource = ShoppingCartService.CurrentWishlist;
            lvList.DataBind();
        }

        protected void btnDeleteClick(object sender, EventArgs e)
        {
            int itemId = hfWishListItemID.Value.TryParseInt();
            var wishList = ShoppingCartService.CurrentWishlist;
            if (wishList.Any(item => item.ShoppingCartItemId == itemId))
            {
                ShoppingCartService.DeleteShoppingCartItem(itemId);
            }
        }

        protected void btnAddToCartClick(object sender, EventArgs e)
        {
            int itemId = hfWishListItemID.Value.TryParseInt();
            var wishListItem = ShoppingCartService.CurrentWishlist.Find(item => item.ShoppingCartItemId == itemId);
            if (wishListItem != null)
            {

                ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                    {
                        OfferId = wishListItem.OfferId,
                        Amount = 1,
                        ShoppingCartType = ShoppingCartType.ShoppingCart,
                        AttributesXml = wishListItem.AttributesXml,
                    });
                Response.Redirect("shoppingcart.aspx");
            }
        }

        protected string RenderPictureTag(string urlPhoto, string productName, string urlPath)
        {

            return string.Format("<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" /></a>", urlPath,
                                 urlPhoto.IsNotEmpty()
                                     ? FoldersHelper.GetImageProductPath(ProductImageType.Small, urlPhoto, false)
                                     : "images/nophoto_small.jpg", productName);

        }

        protected string RenderPriceTag(float price, float discount)
        {
            float productDiscount = discount != 0 ? discount : DiscountByTime;
            return CatalogService.RenderPrice(price, productDiscount, false, customerGroup);
        }
    }
}