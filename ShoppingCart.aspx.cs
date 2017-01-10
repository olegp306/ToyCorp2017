//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using System;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;
using AdvantShop.SEO;
using AdvantShop.Trial;
using Resources;

namespace ClientPages
{
    public partial class ShoppingCart_Page : AdvantShopClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lDemoWarning.Visible = Demo.IsDemoEnabled || TrialService.IsTrialEnabled;

            //BuyInOneClick.Visible = SettingsOrderConfirmation.BuyInOneClick;

            if (!IsPostBack)
            {
                if (Request["products"].IsNotEmpty())
                {
                    foreach (var item in Request["products"].Split(";"))
                    {
                        int offerId;
                        var newItem = new ShoppingCartItem(){ShoppingCartType = ShoppingCartType.ShoppingCart, CustomerId = CustomerContext.CustomerId};

                        var parts = item.Split("-");
                        if (parts.Length > 0 && (offerId = parts[0].TryParseInt(0)) != 0 && OfferService.GetOffer(offerId) != null)
                        {
                            newItem.OfferId = offerId;
                        }
                        else
                        {
                            continue;
                        }

                        if (parts.Length > 1)
                        {
                            newItem.Amount = parts[1].TryParseFloat();
                        }
                        else
                        {
                            newItem.Amount = 1;
                        }

                        var currentItem = ShoppingCartService.CurrentShoppingCart.FirstOrDefault(shpCartitem => shpCartitem.OfferId == newItem.OfferId);

                        if (currentItem != null)
                        {
                            currentItem.Amount = newItem.Amount;
                            ShoppingCartService.UpdateShoppingCartItem(currentItem);
                        }
                        else
                        {
                            ShoppingCartService.AddShoppingCartItem(newItem);
                        }
                    }
                    Response.Redirect("shoppingcart.aspx");
                    return;
                }

                UpdateBasket();
                SetMeta(new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_ShoppingCart_ShoppingCart)), string.Empty);

                if (GoogleTagManager.Enabled)
                {
                    var tagManager = ((AdvantShopMasterPage)Master).TagManager;
                    tagManager.PageType = GoogleTagManager.ePageType.cart;
                    tagManager.ProdIds = ShoppingCartService.CurrentShoppingCart.Select(item => item.Offer.ArtNo).ToList();
                    tagManager.TotalValue = ShoppingCartService.CurrentShoppingCart.TotalPrice;
                }
            }

            var showConfirmButtons = true;

            //подключение модуля
            foreach (var module in AttachedModules.GetModules<IRenderIntoShoppingCart>())
            {
                var moduleObject = (IRenderIntoShoppingCart)Activator.CreateInstance(module, null);

                ltrlBottomContent.Text = moduleObject.DoRenderToBottom();
                ltrlTopContent.Text = moduleObject.DoRenderToTop();

                if (!string.IsNullOrEmpty(moduleObject.ClientSideControlNameBottom))
                {
                    var userControl =
                        (this).LoadControl(moduleObject.ClientSideControlNameBottom);

                    if (userControl != null)
                    {
                        ((IUserControlInSc)userControl).ProductIds =
                            ShoppingCartService.CurrentShoppingCart.Select(p => p.Offer.ProductId).ToList();
                        pnlBottomContent.Controls.Add(userControl);
                    }
                }
                if (!string.IsNullOrEmpty(moduleObject.ClientSideControlNameTop))
                {
                    var userControl =
                        (this).LoadControl(moduleObject.ClientSideControlNameTop);

                    if (userControl != null)
                    {
                        ((IUserControlInSc)userControl).ProductIds =
                            ShoppingCartService.CurrentShoppingCart.Select(p => p.Offer.ProductId).ToList();
                        pnlTopContent.Controls.Add(userControl);
                    }
                }
                showConfirmButtons &= moduleObject.ShowConfirmButtons;
            }

            BuyInOneClick.Visible = showConfirmButtons && SettingsOrderConfirmation.BuyInOneClick;
            aCheckOut.Visible = showConfirmButtons;
        }

        public void UpdateBasket()
        {
            var shpCart = ShoppingCartService.CurrentShoppingCart;

            if (shpCart.HasItems)
            {
                lblEmpty.Visible = false;
            }
            else
            {
                dvOrderMerged.Visible = false;
                BuyInOneClick.Visible = false;
                lblEmpty.Visible = true;
            }
        }


        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            var shoppingCart = ShoppingCartService.CurrentShoppingCart;

            if (!shoppingCart.CanOrder)
            {
                UpdateBasket();
            }
            else
            {
                Response.Redirect("orderconfirmation.aspx");
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            UpdateBasket();
        }
    }
}