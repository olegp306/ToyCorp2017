//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Payment
{
    public class PayPalExpressCheckout : PaymentMethod
    {
        public static readonly List<string> AvaliableCurrs = new List<string> {
            "AUD",
            "BRL",
            "CAD", 
            "CZK", 
            "DKK", 
            "EUR", 
            "HKD", 
            "HUF", 
            "ILS", 
            "JPY", 
            "MYR",
            "MXN", 
            "NOK", 
            "NZD", 
            "PHP",
            "PLN", 
            "GBP",
            "RUB",
            "SGD", 
            "SEK", 
            "CHF",
            "TWD",
            "THB",
            "TRY",
            "USD"};

        public override PaymentType Type
        {
            get { return PaymentType.PayPalExpressCheckout; }
        }
        public override ProcessType ProcessType
        {
            get { return ProcessType.ServerRequest; }
        }
        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler | NotificationType.ReturnUrl; }
        }
        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public string User { get; set; }
        public string Password { get; set; }
        public string Signature { get; set; }

        public float CurrencyValue { get; set; }
        public string CurrencyCode { get; set; }
        public bool ShowTaxAndShipping { get; set; }
        public bool Sandbox { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {PayPalExpressCheckoutTemplate.User, User},
                               {PayPalExpressCheckoutTemplate.Password, Password},
                               {PayPalExpressCheckoutTemplate.Signature, Signature},

                               {PayPalExpressCheckoutTemplate.CurrencyCode, CurrencyCode},
                               {PayPalExpressCheckoutTemplate.CurrencyValue, CurrencyValue.ToString(CultureInfo.CurrentCulture)},
                               {PayPalExpressCheckoutTemplate.Sandbox, Sandbox.ToString()}
                           };
            }
            set
            {
                User = !value.ContainsKey(PayPalExpressCheckoutTemplate.User)
                    ? string.Empty
                    : value[PayPalExpressCheckoutTemplate.User];

                Password = !value.ContainsKey(PayPalExpressCheckoutTemplate.Password)
                    ? string.Empty
                    : value[PayPalExpressCheckoutTemplate.Password];

                Signature = !value.ContainsKey(PayPalExpressCheckoutTemplate.Signature)
                    ? string.Empty
                    : value[PayPalExpressCheckoutTemplate.Signature];

                if (!value.ContainsKey(PayPalExpressCheckoutTemplate.CurrencyCode))
                {
                    CurrencyCode = "USD";
                    var dollar = CurrencyService.Currency("USD");
                    CurrencyValue = dollar != null ? dollar.Value : 1;
                }
                else
                {
                    CurrencyCode = value[PayPalExpressCheckoutTemplate.CurrencyCode];
                    if (!value.ContainsKey(PayPalExpressCheckoutTemplate.CurrencyValue))
                        CurrencyValue = 1;
                    else
                    {
                        CurrencyValue = value[PayPalExpressCheckoutTemplate.CurrencyValue].TryParseFloat(1);
                    }
                }
                bool boolVal;

                if (value.ContainsKey(PayPalExpressCheckoutTemplate.Sandbox) && bool.TryParse(value[PayPalExpressCheckoutTemplate.Sandbox], out boolVal))
                    Sandbox = boolVal;
            }
        }

        public override string ProcessServerRequest(Order order)
        {
            var token = IdentityTransaction(order);
            if (!string.IsNullOrEmpty(token))
            {
                // AddOrderToken(order.OrderID, token);
                return string.Format("https://www.{0}paypal.com/webscr?cmd=_express-checkout&token=", Sandbox ? "sandbox." : string.Empty) + HttpUtility.UrlEncode(token);
            }

            return string.Empty;
        }

        public override string ProcessResponse(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Request["token"]) && !string.IsNullOrEmpty(context.Request["PayerID"]))
            {
                var currencyCode = string.Empty;
                var totalOrder = string.Empty;
                var orderNumber = string.Empty;

                var requestGetDetailsUrl =
                    string.Format("https://api-3t.{0}paypal.com/nvp", Sandbox ? "sandbox." : string.Empty) +
                    "?USER=" + User +
                    "&PWD=" + Password +
                    "&SIGNATURE=" + Signature +
                    "&METHOD=GetExpressCheckoutDetails" +
                    "&VERSION=106.0" +
                    "&RETURNURL=" + HttpUtility.UrlEncode(this.NotificationUrl) +
                    "&CANCELURL=" + HttpUtility.UrlEncode(this.CancelUrl) +
                    "&LOCALECODE=" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName +
                    "&TOKEN=" + context.Request["token"] +
                    "&BUTTONSOURCE=AdVantShop_Cart";

                WebRequest requestGetDetails = WebRequest.Create(requestGetDetailsUrl);

                requestGetDetails.Method = "GET";

                var stringRequestDetails =
                    (new StreamReader(requestGetDetails.GetResponse().GetResponseStream())).ReadToEnd();

                if (!string.IsNullOrEmpty(stringRequestDetails))
                {
                    foreach (var param in HttpUtility.UrlDecode(stringRequestDetails).Split(new[] { '&' }))
                    {
                        var pair = param.Split(new[] { '=' });
                        if (pair.Count() == 2 && string.Equals(pair[0], "CURRENCYCODE"))
                        {
                            currencyCode = pair[1];
                        }
                        else if (pair.Count() == 2 && string.Equals(pair[0], "AMT"))
                        {
                            totalOrder = pair[1];
                        }
                        else if (pair.Count() == 2 && string.Equals(pair[0], "INVNUM"))
                        {
                            orderNumber = pair[1];
                        }
                    }
                }

                Debug.LogError("---" + currencyCode + "---" + totalOrder + "---" + orderNumber);

                Order order = null;
                if (!string.IsNullOrEmpty(orderNumber))
                {
                    order = OrderService.GetOrderByNumber(orderNumber);
                    if (order == null)
                    {
                        return NotificationMessahges.Fail;
                    }
                }

                var requestUrl =
                    string.Format("https://api-3t.{0}paypal.com/nvp", Sandbox ? "sandbox." : string.Empty) +
                    "?USER=" + User +
                    "&PWD=" + Password +
                    "&SIGNATURE=" + Signature +
                    "&METHOD=DoExpressCheckoutPayment" +
                    "&VERSION=106.0" +
                    "&TOKEN=" + context.Request["token"] +
                    "&PAYERID=" + context.Request["PayerID"] +
                    "&PAYMENTREQUEST_0_AMT=" + totalOrder +
                    "&PAYMENTREQUEST_0_CURRENCYCODE=" + currencyCode +
                    "&PAYMENTREQUEST_0_PAYMENTACTION=Sale" +
                    "&BUTTONSOURCE=AdVantShop_Cart";
                if (order != null && order.ShippingContact != null)
                {
                    var country = CountryService.GetCountryByName(order.ShippingContact.Country);

                    requestUrl +=
                        "&PAYMENTREQUEST_0_SHIPTONAME=" + HttpUtility.UrlEncode(order.ShippingContact.Name) +
                        "&PAYMENTREQUEST_0_SHIPTOSTREET=" + HttpUtility.UrlEncode(order.ShippingContact.Address) +
                        "&PAYMENTREQUEST_0_SHIPTOCITY=" + HttpUtility.UrlEncode(order.ShippingContact.City) +
                        "&PAYMENTREQUEST_0_SHIPTOSTATE=" + HttpUtility.UrlEncode(order.ShippingContact.Zone) +
                        "&PAYMENTREQUEST_0_SHIPTOZIP=" + HttpUtility.UrlEncode(order.ShippingContact.Zip) +
                        "&PAYMENTREQUEST_0_SHIPTOCOUNTRYCODE=" + HttpUtility.UrlEncode(country.Iso2);
                }
                for (int m = 0; m < order.OrderItems.Count; ++m)
                {
                    requestUrl += string.Format( 
                        "&L_PAYMENTREQUEST_0_NAME{0}={1}&L_PAYMENTREQUEST_0_DESC{0}={2}&L_PAYMENTREQUEST_0_AMT{0}={3}&L_PAYMENTREQUEST_0_QTY{0}={4}&L_PAYMENTREQUEST_0_ITEMCATEGORY{0}=Physical",
                        m,
                        HttpUtility.UrlEncode(order.OrderItems[m].Name),
                        HttpUtility.UrlEncode(string.Empty),
                        HttpUtility.UrlEncode(order.OrderItems[m].Price.ToString().Replace(",", ".")),
                        HttpUtility.UrlEncode(order.OrderItems[m].Amount.ToString()));
                }
                if (order.TotalDiscount != 0)
                {
                    requestUrl += string.Format(
                    "&L_PAYMENTREQUEST_0_NAME{0}={1}&L_PAYMENTREQUEST_0_DESC{0}={2}&L_PAYMENTREQUEST_0_AMT{0}={3}&L_PAYMENTREQUEST_0_QTY{0}={4}&L_PAYMENTREQUEST_0_ITEMCATEGORY{0}=Physical",
                    order.OrderItems.Count,
                    HttpUtility.UrlEncode(Resources.Resource.Admin_PaymentMethod_PayPalExpressCheckout_OrderDiscount),
                    HttpUtility.UrlEncode(string.Empty),
                    HttpUtility.UrlEncode(Math.Round((-1 * Math.Round(order.TotalDiscount, 2)), 2).ToString().Replace(",", ".")),
                    HttpUtility.UrlEncode("1"));
                }

                var request = WebRequest.Create(requestUrl);

                request.Method = "GET";

                var str = (new StreamReader(request.GetResponse().GetResponseStream())).ReadToEnd();

                Debug.LogError("---" + str + "---");

                if (!string.IsNullOrEmpty(str))
                {
                    foreach (var param in HttpUtility.UrlDecode(str).Split(new[] { '&' }))
                    {
                        var pair = param.Split(new[] { '=' });
                        if (pair.Count() == 2 && string.Equals(pair[0], "ACK"))
                        {
                            if (string.Equals(pair[1], "Success") && order != null)
                            {
                                OrderService.PayOrder(order.OrderID, true);
                                return NotificationMessahges.SuccessfullPayment(order.Number);
                            }
                            break;
                        }
                    }
                }
            }

            return NotificationMessahges.Fail;
        }

        private string IdentityTransaction(Order order)
        {
            if (order.Payed)
            {
                return string.Empty;
            }

            var resultToken = string.Empty;

            var shippingcost = Math.Round(order.ShippingCost * CurrencyValue, 2);
            var discount = (-1 * Math.Round(order.TotalDiscount * CurrencyValue, 2));
            var taxcost = order.Taxes.Where(t=> !t.TaxShowInPrice).Sum(t=> Math.Round(t.TaxSum * CurrencyValue, 2));
            var orderItemsSum = Math.Round(order.OrderItems.Sum(item => item.Price * item.Amount * CurrencyValue),2);
            discount += Math.Round(order.Sum * CurrencyValue, 2) - Math.Round(shippingcost + taxcost + orderItemsSum + discount, 2);

            var requestUrl = string.Format("https://api-3t.{0}paypal.com/nvp", Sandbox ? "sandbox." : string.Empty) +
                             "?USER=" + User +
                             "&PWD=" + Password +
                             "&SIGNATURE=" + Signature +
                             "&HDRIMG=" + SettingsGeneral.AbsoluteUrl + "\\" + FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, false) +
                             "&METHOD=SetExpressCheckout" +
                             "&VERSION=106.0" +
                             "&RETURNURL=" + HttpUtility.UrlEncode(this.SuccessUrl) +
                             "&CANCELURL=" + HttpUtility.UrlEncode(this.CancelUrl) +
                             "&LOCALECODE=" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName +
                             "&PAYMENTREQUEST_0_AMT=" + Math.Round(order.Sum * CurrencyValue, 2).ToString().Replace(",", ".") +
                             "&PAYMENTREQUEST_0_ITEMAMT=" + (orderItemsSum + Math.Round(discount, 2)).ToString().Replace(",", ".") +
                             "&PAYMENTREQUEST_0_CURRENCYCODE=" + CurrencyCode +
                             "&PAYMENTREQUEST_0_SHIPPINGAMT=" + shippingcost.ToString().Replace(",", ".") +
                             "&PAYMENTREQUEST_0_TAXAMT=" + taxcost.ToString().Replace(",", ".") +
                             "&PAYMENTREQUEST_0_PAYMENTACTION=SALE" +
                             "&PAYMENTREQUEST_0_INVNUM=" + order.Number +
                             "&PAYMENTREQUEST_0_NOTIFYURL=" + HttpUtility.UrlEncode(this.NotificationUrl) +
                             "&BUTTONSOURCE=AdVantShop_Cart";

            if (order.ShippingContact != null)
            {
                var country = CountryService.GetCountryByName(order.ShippingContact.Country);

                requestUrl +=
                    "&PAYMENTREQUEST_0_SHIPTONAME=" + HttpUtility.UrlEncode(order.ShippingContact.Name) +
                    "&PAYMENTREQUEST_0_SHIPTOSTREET=" + HttpUtility.UrlEncode(order.ShippingContact.Address) +
                    "&PAYMENTREQUEST_0_SHIPTOCITY=" + HttpUtility.UrlEncode(order.ShippingContact.City) +
                    "&PAYMENTREQUEST_0_SHIPTOSTATE=" + HttpUtility.UrlEncode(order.ShippingContact.Zone) +
                    "&PAYMENTREQUEST_0_SHIPTOZIP=" + HttpUtility.UrlEncode(order.ShippingContact.Zip) +
                    "&PAYMENTREQUEST_0_SHIPTOCOUNTRYCODE=" + HttpUtility.UrlEncode(country.Iso2);
            }

            for (int m = 0; m < order.OrderItems.Count; ++m)
            {
                requestUrl += string.Format(
                    "&L_PAYMENTREQUEST_0_NAME{0}={1}&L_PAYMENTREQUEST_0_DESC{0}={2}&L_PAYMENTREQUEST_0_AMT{0}={3}&L_PAYMENTREQUEST_0_QTY{0}={4}&L_PAYMENTREQUEST_0_ITEMCATEGORY{0}=Physical",
                    m,
                    HttpUtility.UrlEncode(order.OrderItems[m].Name),
                    HttpUtility.UrlEncode(string.Empty),
                    HttpUtility.UrlEncode(Math.Round(order.OrderItems[m].Price * CurrencyValue, 2).ToString().Replace(",", ".")),
                    HttpUtility.UrlEncode(order.OrderItems[m].Amount.ToString()));
            }

            if (order.TotalDiscount != 0)
            {
                requestUrl += string.Format(
                "&L_PAYMENTREQUEST_0_NAME{0}={1}&L_PAYMENTREQUEST_0_DESC{0}={2}&L_PAYMENTREQUEST_0_AMT{0}={3}&L_PAYMENTREQUEST_0_QTY{0}={4}&L_PAYMENTREQUEST_0_ITEMCATEGORY{0}=Physical",
                order.OrderItems.Count,
                HttpUtility.UrlEncode(Resources.Resource.Admin_PaymentMethod_PayPalExpressCheckout_OrderDiscount),
                HttpUtility.UrlEncode(string.Empty),
                HttpUtility.UrlEncode(Math.Round(discount, 2).ToString().Replace(",", ".")),
                HttpUtility.UrlEncode("1"));
            }

            WebRequest request = WebRequest.Create(requestUrl);

            request.Method = "GET";

            var str = (new StreamReader(request.GetResponse().GetResponseStream())).ReadToEnd();

            Debug.LogError(requestUrl);
            if (!string.IsNullOrEmpty(str))
            {
                foreach (var responsePair in HttpUtility.UrlDecode(str).Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var pair = responsePair.Split(new[] { '=' });
                    if (pair.Count() > 1 && string.Equals(pair[0].ToLower(), "token"))
                    {
                        resultToken = pair[1];
                    }
                }
            }

            return resultToken;
        }

        private void AddOrderToken(int orderId, string token)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO [ModulePayment].[PayPalExpressCheckout] ([OrderId],[Token]) VALUES (@OrderId, @Token)",
                CommandType.Text,
                new SqlParameter("@OrderId", orderId),
                new SqlParameter("@Token", token));
        }

        private string GetOrderToken(int orderId)
        {
            return SQLDataAccess.ExecuteScalar<string>(
                 "SELECT [Token] FROM [ModulePayment].[PayPalExpressCheckout] WHERE [OrderId] = @OrderId",
                 CommandType.Text,
                 new SqlParameter("@OrderId", orderId));
        }

        private int GetOrderByToken(string token)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                 "SELECT [OrderId] FROM [ModulePayment].[PayPalExpressCheckout] WHERE [Token] = @Token",
                 CommandType.Text,
                 new SqlParameter("@Token", token));
        }

        private void DeleteOrderToken(int orderId, string token)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [ModulePayment].[PayPalExpressCheckout] WHERE [OrderId] = @OrderId AND [Token] = @Token",
                CommandType.Text,
                new SqlParameter("@OrderId", orderId),
                new SqlParameter("@Token", token));
        }
    }
}