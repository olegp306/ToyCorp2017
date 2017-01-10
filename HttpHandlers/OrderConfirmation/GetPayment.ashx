<%@ WebHandler Language="C#" Class="GetPayment" %>

using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping;
using Newtonsoft.Json;

public class GetPayment : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        Culture.InitializeCulture();
        context.Response.ContentType = "application/json";

        var paymentId = context.Request["paymentId"].TryParseInt();
        var shippingId = context.Request["shippingId"].TryParseInt();

        var distance = context.Request["distance"].TryParseInt();
        var pickupId = context.Request["pickpointId"].TryParseInt();
        var pickpointStringId = context.Request["pickpointStringId"]?? string.Empty;
        var pickpointAddress = context.Request["pickpointAddress"] ?? string.Empty;
        var additionalData = context.Request["additionalData"] ?? string.Empty;
        var pickRate = context.Request["pickRate"].TryParseFloat();
        
        
        var pageData = OrderConfirmationService.Get(CustomerContext.CurrentCustomer.Id);
        if (pageData == null)
        {
            return;
        }
        
        var orderConfirmData = pageData.OrderConfirmationData;
        var shoppingCart = ShoppingCartService.CurrentShoppingCart;

        var shippingManager = new ShippingManager();
        var shippingRates = shippingManager.GetShippingRates(orderConfirmData.ShippingContact.CountryId,
                                                            orderConfirmData.ShippingContact.Zip,
                                                            orderConfirmData.ShippingContact.City,
                                                            orderConfirmData.ShippingContact.RegionName,
                                                            shoppingCart, distance, pickupId);
        ShippingManager.CurrentShippingRates = shippingRates;

        var shippingRate = shippingRates.Find(rate => rate.Id == shippingId);
        
        if (shippingRate == null && shippingRates.Count != 0)
        {
            shippingRate = shippingRates.FirstOrDefault();
        }

        if (shippingRate == null)
        {
            orderConfirmData.SelectedShippingItem = new ShippingItem();
            orderConfirmData.SelectedPaymentItem = new PaymentItem();
            OrderConfirmationService.Update(pageData);

            context.Response.Write(JsonConvert.SerializeObject(new PaymentModel()));
            return;
        }

        orderConfirmData.Distance = distance;
        orderConfirmData.SelectedShippingItem = ShippingMethodService.GetListShippingItem(shippingRate, pickupId,
            pickpointAddress, additionalData, orderConfirmData.ShippingContact.City);


        if (shippingRate.Type == ShippingType.CheckoutRu && orderConfirmData.SelectedShippingItem.Ext == null)
        {
            orderConfirmData.SelectedShippingItem.Ext = new ShippingOptionEx
            {
                AdditionalData = additionalData,
                Type = ExtendedType.CashOnDelivery
            };
        }
        else if (shippingRate.Type == ShippingType.CheckoutRu && orderConfirmData.SelectedShippingItem.Ext != null)
        {
            orderConfirmData.SelectedShippingItem.Rate = shippingRate.Ext.AdditionalData.Contains("mail;") ? shippingRate.Rate : pickRate;
        }
            
        
        if (orderConfirmData.SelectedShippingItem.Ext != null && !string.IsNullOrEmpty(pickpointStringId))
        {
            orderConfirmData.SelectedShippingItem.Ext.PickpointId = pickpointStringId;
        }

        var showCertificate = SettingsOrderConfirmation.EnableGiftCertificateService &&
                              shoppingCart.Certificate != null &&
                              shoppingCart.TotalPrice - shoppingCart.TotalDiscount + shippingRate.Rate <= 0;

        var returnPayment = PaymentService.LoadMethods(shippingRate.MethodId, shippingRate.Ext, showCertificate, false);
        var paymentMethods = PaymentService.UseGeoMapping(returnPayment, orderConfirmData.BillingContact.Country,
                                                            orderConfirmData.BillingContact.City);

        if (paymentId == 0 && orderConfirmData.SelectedPaymentItem.PaymenMethodtId != 0)
        {
            paymentId = orderConfirmData.SelectedPaymentItem.PaymenMethodtId;
        }

        var selectedPaymentMethod = paymentMethods.Find(p => p.PaymentMethodId == paymentId);
        if (selectedPaymentMethod == null && paymentMethods.Count > 0)
        {
            selectedPaymentMethod = paymentMethods.FirstOrDefault();
        }
        
        orderConfirmData.SelectedPaymentItem = selectedPaymentMethod ?? new PaymentItem();
        orderConfirmData.CheckSum = shoppingCart.GetHashCode();

        OrderConfirmationService.Update(pageData);

        var obj = new PaymentModel
        {
            SelectedPaymentId = selectedPaymentMethod != null ? selectedPaymentMethod.PaymentMethodId : 0,
            PaymentMethods = paymentMethods.Select(item => new PaymentMethodModel
            {
                Id = item.PaymentMethodId,
                Type = item.Type.ToString().ToLower(),
                Img = String.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{1}\" />",
                    PaymentIcons.GetPaymentIcon(item.Type, item.IconFileName.PhotoName, item.Name),
                    HttpUtility.HtmlEncode(item.Name)),
                Name = item.Name,
                Description = item.Description
            }).ToList(),
            IsValid =
                orderConfirmData.SelectedShippingItem.Id != 0 &&
                orderConfirmData.SelectedPaymentItem.PaymenMethodtId != 0,
            ShowAddress = ShippingMethodService.ShowAddressField(orderConfirmData.UserType, shippingRate),
            ShowCustomFields = ShippingMethodService.ShowCustomField(orderConfirmData.UserType, shippingRate),
            PaymentType = orderConfirmData.SelectedPaymentItem.Type.ToString().ToLower(),
        };

        context.Response.Write(JsonConvert.SerializeObject(obj));
    }

    public bool IsReusable
    {
        get { return false; }
    }
}