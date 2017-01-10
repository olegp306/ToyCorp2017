//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.SEO;
using AdvantShop.Trial;
using Resources;

namespace Social
{
    public partial class ShoppingCartPage : AdvantShopClientPage
    {
        protected CustomerGroup customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

        protected void Page_Load(object sender, EventArgs e)
        {
            lDemoWarning.Visible = Demo.IsDemoEnabled || TrialService.IsTrialEnabled;

            if (!IsPostBack)
            {
                if(Request["productid"].IsNotEmpty())
                {
                    int productId = Request["productid"].TryParseInt();
                    int amount = Request["amount"].TryParseInt(1);
                    if(productId != 0 && ProductService.IsProductEnabled(productId))
                    {
                        IList<EvaluatedCustomOptions> listOptions = null;
                        string selectedOptions = HttpUtility.UrlDecode(Request["AttributesXml"]);
                        try
                        {
                            listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions);
                        }
                        catch (Exception)
                        {
                            listOptions = null;
                        }

                        if (CustomOptionsService.DoesProductHaveRequiredCustomOptions(productId) && listOptions == null)
                        {
                            Response.Redirect(SettingsMain.SiteUrl + UrlService.GetLinkDB(ParamType.Product, productId));
                            return;
                        }

                        ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                            {
                                OfferId = ProductService.GetProduct(productId).Offers[0].OfferId,
                                Amount = amount,
                                ShoppingCartType = ShoppingCartType.ShoppingCart,
                                AttributesXml = listOptions != null ? selectedOptions : string.Empty,
                            });

                        Response.Redirect("shoppingcartsocial.aspx");
                    }
                }

                UpdateBasket();
                SetMeta(new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_ShoppingCart_ShoppingCart)), string.Empty);
            }
            //relatedProducts.ProductIds = ShoppingCartService.CurrentShoppingCart.Where(p => p.ItemType == ShoppingCartItem.EnumItemType.Product).Select(p => p.EntityId).ToList();
        }



        public void UpdateBasket()
        {
            var shpCart = ShoppingCartService.CurrentShoppingCart;

            if (shpCart.HasItems)
            {
                lblEmpty.Visible = false;
                //shoppingCartTable.Attributes.Add("style", "display:none");
            }
            else
            {
                //divEmptyCart.Attributes.Add("style", "display:none");
                dvOrderMerged.Visible = false;
                lblEmpty.Visible = true;
            }
        }
  
        protected void Page_PreRender(object sender, EventArgs e)
        {
            UpdateBasket();
            var shoppingCart = ShoppingCartService.CurrentShoppingCart;

            //pnlCouponCertificate.Visible = shoppingCart.HasItems && shoppingCart.Coupon == null && shoppingCart.Certificate == null
            //        && customerGroup.CustomerGroupId == CustomerGroupService.DefaultCustomerGroup;
        }

        protected void btnEnterCode_Click(object sender, EventArgs e)
        {
            if (customerGroup.CustomerGroupId != CustomerGroupService.DefaultCustomerGroup)
                return;

            //var cert = GiftCertificateService.GetCertificateByCode(txtCertificateCoupon.Text.Trim());
            //var coupon = CouponService.GetCouponByCode(txtCertificateCoupon.Text.Trim());

            //if (cert != null && cert.Paid && !cert.Used && cert.Enable)
            //{
            //    GiftCertificateService.AddCustomerCertificate(cert.CertificateId);
            //}
            //else if (coupon != null && (coupon.ExpirationDate == null || coupon.ExpirationDate > DateTime.Now) && (coupon.PossibleUses == 0 || coupon.PossibleUses > coupon.ActualUses) && coupon.Enabled)
            //{
            //    CouponService.AddCustomerCoupon(coupon.CouponID);
            //}
            Response.Redirect("shoppingcartsocial.aspx");
        }
    }
}