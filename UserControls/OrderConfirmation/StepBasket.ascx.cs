using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using Resources;

namespace UserControls.OrderConfirmation
{
    public partial class StepBasket : System.Web.UI.UserControl
    {
        public AdvantShop.Orders.OrderConfirmation PageData { get; set; }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (PageData == null)
                return;

            LoadShoppingCart();
            OrderConfirmationService.Update(PageData);

            buyInOneClick.Visible = SettingsOrderConfirmation.BuyInOneClick && SettingsOrderConfirmation.BuyInOneClickInOrderConfirmation;
        }

        private void LoadShoppingCart()
        {

            var orderConfirmData = PageData.OrderConfirmationData;

            var shpCart = ShoppingCartService.CurrentShoppingCart;

            if (shpCart.Count > 0 && shpCart.Count <= 5)
            {
                lvOrderList.DataSource = shpCart;
                lvOrderList.DataBind();
                lvOrderList.Visible = true;
            }
            else
            {
                lvOrderList.Visible = false;
            }

            var productsPrice = shpCart.TotalPrice;
            var discountOnTotalPrice = shpCart.DiscountPercentOnTotalPrice;
            var totalDiscount = shpCart.TotalDiscount;
            var shippingPrice = orderConfirmData.SelectedShippingItem.Rate;

            if (discountOnTotalPrice > 0)
            {
                discountRow.Visible = true;
                liDiscountPercent.Text = string.Format("<span class=\"per\">-{0}%</span>", discountOnTotalPrice);
                lblOrderDiscount.Text = "-" + CatalogService.GetStringPrice(productsPrice * discountOnTotalPrice / 100);
            }
            else
            {
                discountRow.Visible = false;
            }

            if (shpCart.Certificate != null)
            {
                certificateRow.Visible = true;
                lblCertificatePrice.Text = String.Format("-{0}",
                    CatalogService.GetStringPrice(shpCart.Certificate.Sum, 1,
                        shpCart.Certificate.CertificateOrder.OrderCurrency.CurrencyCode,
                        shpCart.Certificate.CertificateOrder.OrderCurrency.CurrencyValue));
            }

            if (shpCart.Coupon != null)
            {
                couponRow.Visible = true;
                if (shpCart.TotalPrice < shpCart.Coupon.MinimalOrderPrice)
                {
                    lblCouponPrice.Text = String.Format(Resource.Client_OrderConfirmation_CouponMessage,
                        CatalogService.GetStringPrice(shpCart.Coupon.MinimalOrderPrice));
                }
                else
                {
                    if (totalDiscount == 0)
                    {
                        lblCouponPrice.Text = String.Format("-{0} ({1}) <img src='images/question_mark.png' title='{3}'> <a class=\"cross\"  data-cart-remove-cupon=\"true\" title=\"{2}\"></a>",
                                      CatalogService.GetStringPrice(0), shpCart.Coupon.Code,
                                      Resource.Client_ShoppingCart_DeleteCoupon,
                                      Resource.Client_ShoppingCart_CouponNotApplied);
                    }
                    else
                    {
                        switch (shpCart.Coupon.Type)
                        {
                            case CouponType.Fixed:
                                lblCouponPrice.Text = String.Format("-{0} ({1})",
                                    CatalogService.GetStringPrice(totalDiscount), shpCart.Coupon.Code);
                                break;
                            case CouponType.Percent:
                                lblCouponPrice.Text = String.Format("-{0} ({1}%) ({2})",
                                    CatalogService.GetStringPrice(totalDiscount),
                                    CatalogService.FormatPriceInvariant(shpCart.Coupon.Value), shpCart.Coupon.Code);
                                break;
                        }
                    }
                }
            }

            if (BonusSystem.IsActive)
            {
                var bonusCard = BonusSystemService.GetCard(orderConfirmData.Customer.BonusCardNumber);
                if (bonusCard != null)
                {
                    if (orderConfirmData.UseBonuses)
                    {
                        var bonusPrice = BonusSystemService.GetBonusCost(productsPrice + shippingPrice - totalDiscount, productsPrice - totalDiscount, bonusCard.BonusAmount);
                        totalDiscount += bonusPrice;

                        lblOrderBonuses.Text = "-" + CatalogService.GetStringPrice(bonusPrice);
                        bonusesRow.Visible = true;
                    }

                    var bonusPlusPrice = BonusSystemService.GetBonusPlusCost(productsPrice + shippingPrice - totalDiscount, productsPrice - totalDiscount, bonusCard.BonusPercent);
                    if (bonusPlusPrice > 0)
                    {
                        liBonusPlus.Text = "+" + CatalogService.GetStringPrice(bonusPlusPrice);
                        bonusPlus.Visible = true;
                    }
                    orderConfirmData.BonusPlus = bonusPlusPrice;
                }
            }



            if (orderConfirmData.SelectedPaymentItem.Type == PaymentType.CashOnDelivery && orderConfirmData.SelectedShippingItem.Ext != null)
                shippingPrice = orderConfirmData.SelectedShippingItem.Ext.PriceCash;
            
            var taxesItems = TaxServices.CalculateTaxes(productsPrice - totalDiscount + shippingPrice);
            var taxesTotal = taxesItems.Where(tax=> !tax.Key.ShowInPrice).Sum(item => item.Value);

            var paymentExtraCharge = orderConfirmData.SelectedPaymentItem.Extracharge;
            paymentExtraChargeRow.Visible = paymentExtraCharge != 0;

            if (orderConfirmData.SelectedPaymentItem.ExtrachargeType == ExtrachargeType.Percent)
            {
                paymentExtraCharge = paymentExtraCharge * (productsPrice - totalDiscount + shippingPrice + taxesTotal) / 100;
            }

            lPaymentCost.Text = paymentExtraCharge > 0
                ? Resource.Client_OrderConfirmation_PaymentCost
                : Resource.Client_OrderConfirmation_PaymentDiscount;

            var totalPrice = productsPrice - totalDiscount + shippingPrice + paymentExtraCharge + taxesTotal;


            lblProductsPrice.Text = CatalogService.GetStringPrice(productsPrice);
            lblShippingPrice.Text = shippingPrice != 0 ? "+" + CatalogService.GetStringPrice(shippingPrice) : orderConfirmData.SelectedShippingItem.ZeroPriceMessage;
            lblPaymentExtraCharge.Text = (paymentExtraCharge > 0 ? "+" : "") +
                                         CatalogService.GetStringPrice(paymentExtraCharge);

            //todo tax
            if (taxesItems.Count > 0)
            {
                literalTaxCost.Text = BuildTaxTable(taxesItems, CurrencyService.CurrentCurrency.Value,
                    CurrencyService.CurrentCurrency.Iso3);
            }

            lblTotalPrice.Text = CatalogService.GetStringPrice(totalPrice > 0 ? totalPrice : 0);

            orderConfirmData.Sum = totalPrice > 0 ? totalPrice : 0;
            orderConfirmData.CheckSum = ShoppingCartService.CurrentShoppingCart.GetHashCode();
        }


        private static string BuildTaxTable(Dictionary<TaxElement, float> taxes, float currentCurrencyRate, string currentCurrencyIso3)
        {
            var sb = new StringBuilder();
            foreach (var tax in taxes)
            {
                sb.Append("<div class=\"orderbasket-row\">");

                sb.AppendFormat("<div class=\"orderbasket-row-price\"> <div class=\"orderbasket-row-text\">{0} {1}:</div> ",
                                (tax.Key.ShowInPrice ? Resource.Core_TaxServices_Include_Tax : ""),
                                tax.Key.Name);

                sb.AppendFormat("<div class=\"orderbasket-row-cost\">{0}{1}</div></div>",
                                (tax.Key.ShowInPrice ? "" : "+"),
                                CatalogService.GetStringPrice(tax.Value, currentCurrencyRate, currentCurrencyIso3));

                sb.Append("</div>");

            }
            return sb.ToString();
        }
    }
}