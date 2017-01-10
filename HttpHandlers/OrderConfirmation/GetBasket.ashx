<%@ WebHandler Language="C#" Class="GetBasket" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Taxes;
using Newtonsoft.Json;
using Resources;

public class GetBasket : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        context.Response.ContentType = "application/json";

        var shpCart = ShoppingCartService.CurrentShoppingCart;
        if (shpCart.Count == 0)
            return;

        var pageData = OrderConfirmationService.Get(CustomerContext.CurrentCustomer.Id);
        if (pageData == null)
            return;

        var confirmationData = pageData.OrderConfirmationData;

        confirmationData.UseBonuses = context.Request["bonuses"] == "true";
        
        var paymentId = context.Request["paymentId"].TryParseInt();
        if (paymentId != 0)
        {
            UpdatePayment(pageData, paymentId);
        }
        
        var productsPrice = shpCart.TotalPrice;
        var discountOnTotalPrice = shpCart.DiscountPercentOnTotalPrice;
        var totalDiscount = shpCart.TotalDiscount;
        var shippingPrice = confirmationData.SelectedShippingItem.Rate;

        var certificatePrice = "";
        var couponPrice = "";
        var discountOnTotal = "";
        var discountSumOnTotal = "";

        if (discountOnTotalPrice > 0)
        {
            discountOnTotal = string.Format("<span class=\"per\">-{0}%</span>", discountOnTotalPrice);
            discountSumOnTotal = CatalogService.GetStringPrice(productsPrice * discountOnTotalPrice / 100);
        }

        if (shpCart.Certificate != null)
        {
            certificatePrice = String.Format("-{0}",
                CatalogService.GetStringPrice(shpCart.Certificate.Sum, 1,
                    shpCart.Certificate.CertificateOrder.OrderCurrency.CurrencyCode,
                    shpCart.Certificate.CertificateOrder.OrderCurrency.CurrencyValue));
        }

        if (shpCart.Coupon != null)
        {
            if (shpCart.TotalPrice < shpCart.Coupon.MinimalOrderPrice)
            {
                couponPrice = String.Format(Resource.Client_OrderConfirmation_CouponMessage,
                    CatalogService.GetStringPrice(shpCart.Coupon.MinimalOrderPrice));
            }
            else
            {
                if (totalDiscount == 0)
                {
                    couponPrice = String.Format("-{0} ({1}) <img src='images/question_mark.png' title='{3}'> <a class=\"cross\"  data-cart-remove-cupon=\"true\" title=\"{2}\"></a>",
                                      CatalogService.GetStringPrice(0), shpCart.Coupon.Code,
                                      Resource.Client_ShoppingCart_DeleteCoupon,
                                      Resource.Client_ShoppingCart_CouponNotApplied);
                }
                else
                {
                    switch (shpCart.Coupon.Type)
                    {
                        case CouponType.Fixed:
                            couponPrice = String.Format("-{0} ({1})", CatalogService.GetStringPrice(totalDiscount), shpCart.Coupon.Code);
                            break;
                        case CouponType.Percent:
                            couponPrice = String.Format("-{0} ({1}%) ({2})", CatalogService.GetStringPrice(totalDiscount),
                                                                CatalogService.FormatPriceInvariant(shpCart.Coupon.Value), shpCart.Coupon.Code);
                            break;
                    }
                }
            }
        }

        if (confirmationData.SelectedPaymentItem.Type == PaymentType.CashOnDelivery && confirmationData.SelectedShippingItem.Ext != null && confirmationData.SelectedShippingItem.Ext.Type != ExtendedType.Cdek)
            shippingPrice = pageData.OrderConfirmationData.SelectedShippingItem.Ext.PriceCash;


        float bonusPrice = 0;
        float bonusPlusPrice = 0;

        if (BonusSystem.IsActive)
        {
            var bonusCard = BonusSystemService.GetCard(confirmationData.Customer.BonusCardNumber);
            if (bonusCard != null)
            {
                if (confirmationData.UseBonuses)
                {
                    bonusPrice = BonusSystemService.GetBonusCost(productsPrice + shippingPrice - totalDiscount, productsPrice - totalDiscount, bonusCard.BonusAmount);
                    totalDiscount += bonusPrice;
                }

                bonusPlusPrice = BonusSystemService.GetBonusPlusCost(productsPrice + shippingPrice - totalDiscount, productsPrice - totalDiscount, bonusCard.BonusPercent);
                confirmationData.BonusPlus = bonusPlusPrice;
            }
        }
        
        var taxesItems = TaxServices.CalculateTaxes(productsPrice - totalDiscount + shippingPrice);
        var taxesTotal = taxesItems.Where(tax => !tax.Key.ShowInPrice).Sum(item => item.Value);

        var paymentExtraCharge = confirmationData.SelectedPaymentItem.Extracharge;

        if (confirmationData.SelectedPaymentItem.ExtrachargeType == ExtrachargeType.Percent)
        {
            paymentExtraCharge = paymentExtraCharge * (productsPrice - totalDiscount + shippingPrice + taxesTotal) / 100;
        }

        var paymentCost = paymentExtraCharge == 0
            ? ""
            : (paymentExtraCharge > 0
                ? Resource.Client_OrderConfirmation_PaymentCost
                : Resource.Client_OrderConfirmation_PaymentDiscount);

        var totalPrice = productsPrice + shippingPrice + taxesTotal - totalDiscount + paymentExtraCharge;

        var obj = new
        {
            TotalOrderPrice = CatalogService.GetStringPrice(productsPrice),
            Discount = discountOnTotal,
            DiscountSum = discountSumOnTotal,
            Bonuses = bonusPrice > 0 ? CatalogService.GetStringPrice(bonusPrice) : string.Empty,
            BonusesPlus = bonusPlusPrice > 0 ? CatalogService.GetStringPrice(bonusPlusPrice) : string.Empty,
            ShippingPrice = shippingPrice != 0 ? CatalogService.GetStringPrice(shippingPrice): confirmationData.SelectedShippingItem.ZeroPriceMessage,
            PaymentCost = paymentCost,
            PaymentExtraCharge = (paymentExtraCharge > 0 ? "+" : "") + CatalogService.GetStringPrice(paymentExtraCharge),
            Taxes = BuildTaxTable(taxesItems, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Iso3),
            CouponPrice = couponPrice,
            CertificatePrice = certificatePrice,
            Total = CatalogService.GetStringPrice(totalPrice > 0 ? totalPrice : 0),
            IsValid =
                confirmationData.SelectedShippingItem.Id != 0 &&
                confirmationData.SelectedPaymentItem.PaymenMethodtId != 0
        };

        context.Response.Write(JsonConvert.SerializeObject(obj));
    }

    private void UpdatePayment(OrderConfirmation pageData, int paymentId)
    {
        var confirmationData = pageData.OrderConfirmationData;

        var shippingRate = ShippingManager.CurrentShippingRates.Find(rate => rate.Id == confirmationData.SelectedShippingItem.Id) ??
                           new ShippingItem();

        var shpCart = ShoppingCartService.CurrentShoppingCart;
        var showCertificate = SettingsOrderConfirmation.EnableGiftCertificateService &&
                              shpCart.Certificate != null &&
                              shpCart.TotalPrice - shpCart.TotalDiscount + shippingRate.Rate <= 0;

        var returnPayment = PaymentService.LoadMethods(shippingRate.MethodId, shippingRate.Ext, showCertificate, false);
        var paymentMethods = PaymentService.UseGeoMapping(returnPayment,
                                confirmationData.BillingContact.Country,
                                confirmationData.BillingContact.City);

        var selectedPaymentMethod = paymentMethods.Find(p => p.PaymentMethodId == paymentId);
        if (selectedPaymentMethod == null && paymentMethods.Count > 0)
        {
            selectedPaymentMethod = paymentMethods.FirstOrDefault();
        }

        confirmationData.SelectedPaymentItem = selectedPaymentMethod ?? new PaymentItem();
        OrderConfirmationService.Update(pageData);
    }

    private static string BuildTaxTable(Dictionary<TaxElement, float> taxes, float currentCurrencyRate, string currentCurrencyIso3)
    {
        var sb = new StringBuilder();
        foreach (var tax in taxes)
        {
            sb.AppendFormat("<div class=\"orderbasket-row\"><div class=\"orderbasket-row-price\"><div class=\"orderbasket-row-text\">{0} {1}:</div>",
                            (tax.Key.ShowInPrice ? Resource.Core_TaxServices_Include_Tax : ""),
                            tax.Key.Name);

            sb.AppendFormat("<div class=\"orderbasket-row-cost\">{0}{1}</div></div></div>",
                            (tax.Key.ShowInPrice ? "" : "+"),
                            CatalogService.GetStringPrice(tax.Value, currentCurrencyRate, currentCurrencyIso3));
        }
        return sb.ToString();
    }


    public bool IsReusable
    {
        get { return false; }
    }
}