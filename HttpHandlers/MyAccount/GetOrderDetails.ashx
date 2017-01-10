<%@ WebHandler Language="C#" Class="GetOrderDetails" %>

using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Web.SessionState;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping;
using AdvantShop.Taxes;
using Newtonsoft.Json;
using AdvantShop;
using Resources;

public class GetOrderDetails : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();

        if (context.Request["ordernumber"].IsNullOrEmpty())
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }

        if (!CustomerContext.CurrentCustomer.RegistredUser)
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }

        var order = OrderService.GetOrderByNumber(context.Request["ordernumber"]);

        if (order.OrderCustomer == null ||
            order.OrderCustomer.CustomerID != CustomerContext.CurrentCustomer.Id)
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }
        var productsPrice = order.OrderItems.Sum(item => item.Amount * item.Price);
        string couponPrice = string.Empty, couponPersent = string.Empty;

        if (order.Coupon != null)
        {
            couponPrice = order.Coupon.Value != 0
                              ? string.Format("-{0} ({1})",
                                              CatalogService.GetStringPrice(order.TotalDiscount,
                                                                            order.OrderCurrency.CurrencyValue,
                                                                            order.OrderCurrency.CurrencyCode),
                                              order.Coupon.Code)
                              : string.Empty;

            couponPersent = order.Coupon.Type == CouponType.Percent
                                ? CatalogService.FormatPriceInvariant(order.Coupon.Value)
                                : string.Empty;
        }
        
        var billingAddress = CustomerService.ConvertToLinedAddress(
            new CustomerContact
                {
                    Name = order.BillingContact.Name,
                    Address = order.BillingContact.Address,
                    City = order.BillingContact.City,
                    Country = order.BillingContact.Country,
                    RegionName = string.IsNullOrEmpty(order.BillingContact.Zone)
                                     ? "-"
                                     : order.BillingContact.Zone,
                    Zip = string.IsNullOrEmpty(order.BillingContact.Zip)
                              ? "-"
                              : order.BillingContact.Zip,
                });
        
        var shippingInfo = CustomerService.ConvertToLinedAddress(
            new CustomerContact
                {
                    Name = order.ShippingContact.Name,
                    Address = order.ShippingContact.Address,
                    City = order.ShippingContact.City,
                    Country = order.ShippingContact.Country,
                    RegionName = string.IsNullOrEmpty(order.ShippingContact.Zone)
                                     ? "-"
                                     : order.ShippingContact.Zone,
                    Zip = string.IsNullOrEmpty(order.ShippingContact.Zip)
                              ? "-"
                              : order.ShippingContact.Zip,
                });
        
        context.Response.ContentType = "application/json";
        var items = new object[order.OrderItems.Count];
        for (int i = 0; i < order.OrderItems.Count; i++)
        {
            string photoPath = "";
            string url = string.Empty;
            //if (order.OrderItems[i].ItemType == EnumItemType.Product)
            //{
            var productID = order.OrderItems[i].ProductID;
            if (productID != null)
            {
                var product = ProductService.GetProduct((int)productID);
                photoPath = product != null && product.Photo.IsNotEmpty()
                                ? FoldersHelper.GetImageProductPath(ProductImageType.XSmall, product.Photo, false)
                                : "images/nophoto_xsmall.jpg";
                if (product != null)
                {
                    url = UrlService.GetLinkDB(ParamType.Product, (int)productID);
                }
            }
            //}
            //else
            //{
            //    photoPath = "images/giftcertificate/certifacate_xsmall.jpg";
            //}

            items[i] = new
            {
                //Name = order.OrderItems[i].ItemType == EnumItemType.Product ? order.OrderItems[i].Name : Resources.Resource.Client_GiftCertificate,
                order.OrderItems[i].Name,
                Price = CatalogService.GetStringPrice(
                    order.OrderItems[i].Price,
                    order.OrderCurrency.CurrencyValue,
                    order.OrderCurrency.CurrencyCode),
                Amount = order.OrderItems[i].Amount,
                ArtNo = order.OrderItems[i].ArtNo,
                Id = order.OrderItems[i].ProductID,
                TotalPrice = CatalogService.GetStringPrice(
                    order.OrderItems[i].Amount * order.OrderItems[i].Price,
                    order.OrderCurrency.CurrencyValue,
                    order.OrderCurrency.CurrencyCode),
                Photo = photoPath,
                Url = url,
                CustomOptions = string.Format("{0}{1}{2}", order.OrderItems[i].Color != null ? SettingsCatalog.ColorsHeader + ": " + order.OrderItems[i].Color + "<br/>" : "",
                                order.OrderItems[i].Size != null ? SettingsCatalog.SizesHeader + ": " + order.OrderItems[i].Size + "<br/>" : "",
                                OrderService.RenderSelectedOptions(order.OrderItems[i].SelectedOptions))
            };
        }

        var isEnabledPayment = true;
        object[] payments = null;
        var onclickEvent = string.Empty;
        if (order.OrderStatusId != OrderService.CanceledOrderStatus)
        {
            var allPaymentstemp = PaymentService.GetAllPaymentMethods(true).Where(payment => payment.Type != PaymentType.GiftCertificate);
            var allPayments = new List<PaymentMethod>();
            foreach (var method in allPaymentstemp)
            {
                if (order.ShippingMethodId != 0 && ShippingMethodService.IsPaymentNotUsed(order.ShippingMethodId, method.PaymentMethodId))
                    continue;
                allPayments.Add(method);
            }
            
            allPayments = PaymentService.UseGeoMapping(allPayments, order.BillingContact.Country, order.BillingContact.City);
            isEnabledPayment = allPayments.Any(item => item.PaymentMethodId == order.PaymentMethodId);
            payments = new object[allPayments.Count()];
            for (var i = 0; i < allPayments.Count(); ++i)
            {
                payments[i] = new
                {
                    id = allPayments.ElementAt(i).PaymentMethodId,
                    name = allPayments.ElementAt(i).Name
                };
            }

            onclickEvent = isEnabledPayment
                ? OrderService.ProcessOrder(order, PaymentService.PageWithPaymentButton.myaccount)
                : OrderService.ProcessOrder(order, PaymentService.PageWithPaymentButton.myaccount, allPayments.Any() ? allPayments.ElementAt(0) : null);
        }

        var orderDetails = new
        {
            Email = CustomerContext.CurrentCustomer.EMail,
            order.OrderID,
            order.Number,
            order.OrderStatus.StatusName,
            OrderItems = items,
            BillingName = order.BillingContact.Name,
            billingAddress,
            ShippingName = order.ShippingContact.Name,
            shippingInfo,
            order.ArchivedShippingName,
            order.PaymentMethodId,
            order.PaymentMethodName,
            TotalDiscountPrice = CatalogService.GetStringPrice(
                order.TotalDiscount,
                order.OrderCurrency.CurrencyValue,
                order.OrderCurrency.CurrencyCode),
            ProductsPrice =  CatalogService.GetStringPrice(productsPrice, order.OrderCurrency.CurrencyValue, order.OrderCurrency.CurrencyCode),
            TotalDiscount = order.OrderDiscount,
            CertificatePrice = order.Certificate != null
                                    ? CatalogService.GetStringPrice(order.Certificate.Price, order.OrderCurrency.CurrencyValue,order.OrderCurrency.CurrencyCode)
                                    : string.Empty,
            TotalPrice = CatalogService.GetStringPrice(order.Sum, order.OrderCurrency.CurrencyValue, order.OrderCurrency.CurrencyCode),
            couponPrice,
            couponPersent,
            ShippingPrice = CatalogService.GetStringPrice(order.ShippingCost, order.OrderCurrency.CurrencyValue, order.OrderCurrency.CurrencyCode),
            PaymentPrice = order.PaymentCost != 0 ? 
                              (order.PaymentCost > 0 ? "+" : "") + CatalogService.GetStringPrice(order.PaymentCost, order.OrderCurrency.CurrencyValue, order.OrderCurrency.CurrencyCode)
                            : string.Empty,
            PaymentPriceText = order.PaymentCost != 0 ? Resource.Client_OrderConfirmation_PaymentCost : Resource.Client_OrderConfirmation_PaymentDiscount,
            order.CustomerComment,
            Payments = payments,
            order.PaymentDetails,
            PaymentButton = onclickEvent.Contains("</form>") ? onclickEvent.Substring(onclickEvent.IndexOf("</form>") + 7, onclickEvent.Length - (onclickEvent.IndexOf("</form>") + 7)) : onclickEvent,
            PaymentForm = onclickEvent.Contains("</form>") ? onclickEvent.Substring(0, onclickEvent.IndexOf("</form>") + 7) : "",
            Canceled = order.OrderStatusId == OrderService.CanceledOrderStatus,
            Payed = order.Payed,
            Bonus = CatalogService.GetStringPrice(order.BonusCost, order.OrderCurrency.CurrencyValue, order.OrderCurrency.CurrencyCode),
            TaxesNames = order.Taxes.Select(tax=>tax.TaxName).AggregateString(','),
            TaxesPrice = CatalogService.GetStringPrice(order.TaxCost, order.OrderCurrency.CurrencyValue, order.OrderCurrency.CurrencyCode),
        };

        context.Response.Write(JsonConvert.SerializeObject(orderDetails));
    }

    public bool IsReusable
    {
        get { return false; }
    }
}