//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Payment
{

    public class PayPal : PaymentMethod
    {

        public static readonly List<string> AvaliableCurrs = new List<string> {
                                                              "AUD", "CAD", "CZK", "DKK", "EUR", "HKD", "HUF", "ILS", "JPY"
                                                              , "MXN", "NOK", "NZD", "PLN", "GBP", "SGD", "SEK", "CHF",
                                                              "USD"
                                                          };

        public override PaymentType Type
        {
            get { return PaymentType.PayPal; }
        }
        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }
        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler | NotificationType.ReturnUrl; }
        }
        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }
        public string EMail { get; set; }

        public string PDTCode { get; set; }
        //public string ReturnUrl { get; set; }
        //public string CancelUrl { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencyCode { get; set; }
        public bool ShowTaxAndShipping { get; set; }
        public bool Sandbox { get; set; }
        private const string Command = "_xclick";
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {PayPalTemplate.EMail, EMail},
                               {PayPalTemplate.PDTCode, PDTCode},
                               //{PayPalTemplate.ReturnUrl, ReturnUrl},
                               //{PayPalTemplate.CancelUrl, CancelUrl},
                               {PayPalTemplate.CurrencyCode, CurrencyCode},
                               {PayPalTemplate.CurrencyValue, CurrencyValue.ToString(CultureInfo.InvariantCulture)},
                               {PayPalTemplate.ShowTaxAndShipping, ShowTaxAndShipping.ToString()},
                               {PayPalTemplate.Sandbox, Sandbox.ToString()}
                           };
            }
            set
            {
                if (value.ContainsKey(PayPalTemplate.EMail))

                    EMail = value[PayPalTemplate.EMail];
                PDTCode = !value.ContainsKey(PayPalTemplate.PDTCode) ? "" : value[PayPalTemplate.PDTCode];
                //ReturnUrl = !value.ContainsKey(PayPalTemplate.ReturnUrl) ? SettingsMain.SiteUrl : value[PayPalTemplate.ReturnUrl];
                //CancelUrl = !value.ContainsKey(PayPalTemplate.CancelUrl) ? SettingsMain.SiteUrl : value[PayPalTemplate.CancelUrl];

                if (!value.ContainsKey(PayPalTemplate.CurrencyCode))
                {
                    CurrencyCode = "USD";
                    var dollar = CurrencyService.Currency("USD");
                    CurrencyValue = dollar != null ? dollar.Value : 1;
                }
                else
                {
                    CurrencyCode = value[PayPalTemplate.CurrencyCode];
                    if (!value.ContainsKey(PayPalTemplate.CurrencyValue))
                        CurrencyValue = 1;
                    else
                    {
                        float val;
                        CurrencyValue = float.TryParse(value[PayPalTemplate.CurrencyValue], out val) ? val : 1;
                    }
                }
                bool boolVal;
                if (value.ContainsKey(PayPalTemplate.ShowTaxAndShipping) && bool.TryParse(value[PayPalTemplate.ShowTaxAndShipping], out boolVal))
                    ShowTaxAndShipping = boolVal;
                if (value.ContainsKey(PayPalTemplate.Sandbox) && bool.TryParse(value[PayPalTemplate.Sandbox], out boolVal))
                    Sandbox = boolVal;
            }
        }

        private string GetUrl()
        {
            return string.Format("https://{0}/cgi-bin/webscr", Sandbox ? "www.sandbox.paypal.com" : "www.paypal.com");
        }

        public override void ProcessForm(Order order)
        {

            int index = 0;
            if (!string.IsNullOrEmpty(order.BillingContact.Name)) index = order.BillingContact.Name.IndexOf(" ");
            string first_name = string.Empty;
            string last_name = string.Empty;
            if (index > 0)
            {
                first_name = order.BillingContact.Name.Substring(0, index).Trim();
                last_name = order.BillingContact.Name.Substring(index + 1).Trim();
            }
            else
                first_name = order.BillingContact.Name.Trim();

            new PaymentFormHandler
                {
                    FormName = "_xclick",
                    Method = FormMethod.POST,
                    Url = GetUrl(),
                    InputValues = new Dictionary<string, string>
                                      {
                                          {"cmd", Command},
                                          {"business", EMail},
                                          {"charset", "utf-8"},
                                          {"currency_code", CurrencyCode},
                                          {"item_name", string.Format("Order #{0}", order.OrderID)},
                                          {"invoice", order.Number},
                                          {"amount", CurrencyService.ConvertCurrency(ShowTaxAndShipping ? order.Sum - order.TaxCost - order.ShippingCost : order.Sum, CurrencyValue,order.OrderCurrency.CurrencyValue).ToString("F2").Replace(",", ".")},
                                          {"tax", CurrencyService.ConvertCurrency(ShowTaxAndShipping ? order.TaxCost : 0, CurrencyValue,order.OrderCurrency.CurrencyValue).ToString("F2").Replace(",", ".")},
                                          {"shipping", CurrencyService.ConvertCurrency(ShowTaxAndShipping ? order.ShippingCost : 0, CurrencyValue,order.OrderCurrency.CurrencyValue).ToString("F2").Replace(",", ".")},
                                          {"address1", (order.BillingContact.Address ?? string.Empty).Replace("\n", "")},
                                          {"city", order.BillingContact.City ?? string.Empty},
                                          {"country", AdvantShop.Repository.CountryService.GetIso2(order.BillingContact.Country ?? string.Empty) ?? string.Empty},
                                          {"lc", AdvantShop.Repository.CountryService.GetIso2(order.BillingContact.Country ?? string.Empty) ?? string.Empty},
                                          //{"email", order.BillingContact.Email ?? string.Empty},
                                          {"email", order.OrderCustomer .Email ?? string.Empty},
                                          {"first_name", first_name ?? string.Empty},
                                          {"last_name", last_name ?? string.Empty},
                                          {"zip", order.BillingContact.Zip ?? string.Empty},
                                          {"state", order.BillingContact.Zone ?? string.Empty},
                                          {"return", SuccessUrl},
                                          {"notify_url", NotificationUrl},
                                          {"cancel_return", CancelUrl}
                                      }
                }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {

            int index = 0;
            if (!string.IsNullOrEmpty(order.BillingContact.Name)) index = order.BillingContact.Name.IndexOf(" ");
            string first_name = string.Empty;
            string last_name = string.Empty;
            if (index > 0)
            {
                first_name = order.BillingContact.Name.Substring(0, index).Trim();
                last_name = order.BillingContact.Name.Substring(index + 1).Trim();
            }
            else
                first_name = order.BillingContact.Name.Trim();

            return new PaymentFormHandler
               {
                   FormName = "_xclick",
                   Method = FormMethod.POST,
                   Url = GetUrl(),
                   Page = page,
                   InputValues = new Dictionary<string, string>
                                      {
                                          {"cmd", Command},
                                          {"business", EMail},
                                          {"charset", "utf-8"},
                                          {"currency_code", CurrencyCode},
                                          {"item_name", string.Format("Order #{0}", order.OrderID)},
                                          {"invoice", order.Number},
                                          {"amount", CurrencyService.ConvertCurrency(ShowTaxAndShipping ? order.Sum - order.TaxCost - order.ShippingCost : order.Sum, CurrencyValue,order.OrderCurrency.CurrencyValue).ToString("F2").Replace(",", ".")},
                                          {"tax", CurrencyService.ConvertCurrency(ShowTaxAndShipping ? order.TaxCost : 0, CurrencyValue,order.OrderCurrency.CurrencyValue).ToString("F2").Replace(",", ".")},
                                          {"shipping", CurrencyService.ConvertCurrency(ShowTaxAndShipping ? order.ShippingCost : 0, CurrencyValue,order.OrderCurrency.CurrencyValue).ToString("F2").Replace(",", ".")},
                                          {"address1", (order.BillingContact.Address ?? string.Empty).Replace("\n", "")},
                                          {"city", order.BillingContact.City ?? string.Empty},
                                          {"country", Repository.CountryService.GetIso2(order.BillingContact.Country ?? string.Empty) ?? string.Empty},
                                          {"lc", Repository.CountryService.GetIso2(order.BillingContact.Country ?? string.Empty) ?? string.Empty},
                                          //{"email", order.BillingContact.Email ?? string.Empty},
                                          {"email", order.OrderCustomer .Email ?? string.Empty},
                                          {"first_name", first_name ?? string.Empty},
                                          {"last_name", last_name ?? string.Empty},
                                          {"zip", order.BillingContact.Zip ?? string.Empty},
                                          {"state", order.BillingContact.Zone ?? string.Empty},
                                          {"return", SuccessUrl},
                                          {"notify_url", NotificationUrl},
                                          {"cancel_return", CancelUrl}
                                      }
               }.ProcessRequest();
        }

        public override string ProcessResponse(HttpContext context)
        {

            //if (Sandbox)
            //    return NotificationMessahges.TestMode;
            //if (string.IsNullOrEmpty(context.Request["tx"]))
            //    return NotificationMessahges.InvalidRequestData;
            //var tx = context.Request["tx"];
            try
            {
                bool IsValidRequest = false;
                string orderNumber = string.Empty;
                string mc_gross = string.Empty;

                if (!string.IsNullOrEmpty(GetPrm(context, "verify_sign")))
                {
                    string strFormValues = Encoding.ASCII.GetString(context.Request.BinaryRead(context.Request.ContentLength));
                    string strNewValue;

                    // Create the request back
                    var req = (HttpWebRequest)WebRequest.Create(GetUrl());

                    // Set values for the request back
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    strNewValue = strFormValues + "&cmd=_notify-validate";
                    req.ContentLength = strNewValue.Length;

                    // Write the request back IPN strings
                    using (StreamWriter stOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
                        stOut.Write(strNewValue);

                    // send the request, read the response
                    using (HttpWebResponse strResponse = (HttpWebResponse)req.GetResponse())
                    {
                        using (Stream IPNResponseStream = strResponse.GetResponseStream())
                        {
                            Encoding encode = Encoding.GetEncoding("utf-8");
                            using (StreamReader readStream = new StreamReader(IPNResponseStream, encode))
                            {
                                char[] read = new char[256];
                                // Reads 256 characters at a time.
                                int count = readStream.Read(read, 0, 256);
                                while (count > 0)
                                {
                                    // Dumps the 256 characters to a string and displays the string to the console.
                                    String IPNResponse = new String(read, 0, count);
                                    count = readStream.Read(read, 0, 256);
                                    // if IPN response was VERIFIED..perform VERIFIED handling
                                    //for this example - send email of raw IPN string
                                    if (IPNResponse == "VERIFIED")
                                        IsValidRequest = true;
                                }
                                //tidy up, close streams
                                readStream.Close();
                                strResponse.Close();
                            }
                        }
                    }
                    orderNumber = GetPrm(context, "invoice");
                    mc_gross = GetPrm(context, "mc_gross");
                }
                else if (!string.IsNullOrEmpty(GetPrm(context, "tx")))
                {
                    var req = (HttpWebRequest)WebRequest.Create(GetUrl());
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";

                    string formContent = string.Format("cmd=_notify-synch&at={0}&tx={1}", PDTCode, GetPrm(context, "tx"));
                    req.ContentLength = formContent.Length;

                    using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
                        sw.Write(formContent);

                    string response;
                    using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
                        response = HttpUtility.UrlDecode(sr.ReadToEnd());

                    var success = response.Split('\n').Select(l => l.Trim()).First();
                    if (success != null && success.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        IsValidRequest = true;

                        orderNumber =
                            response.Split('\n').FirstOrDefault(
                                line => line.Contains('=') && line.Trim().Substring(0, line.IndexOf('=')) == "invoice");

                        mc_gross =
                            response.Split('\n').FirstOrDefault(
                                line => line.Contains('=') && line.Trim().Substring(0, line.IndexOf('=')) == "mc_gross");

                        if (!string.IsNullOrEmpty(orderNumber))
                            orderNumber = orderNumber.Substring(orderNumber.IndexOf('=') + 1);

                        if (!string.IsNullOrEmpty(mc_gross))
                            mc_gross = mc_gross.Substring(mc_gross.IndexOf('=') + 1);
                    }
                }

                if (IsValidRequest && !string.IsNullOrEmpty(orderNumber) && !string.IsNullOrEmpty(mc_gross))
                {
                    Order order = OrderService.GetOrder(OrderService.GetOrderIdByNumber(orderNumber));
                    if (order != null)
                    {
                        var provider = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.InvariantCulture.LCID);
                        provider.NumberFormat.NumberDecimalSeparator = ".";
                        float Summ;
                        float.TryParse(mc_gross, System.Globalization.NumberStyles.AllowDecimalPoint, provider, out Summ);
                        if (Summ < Math.Round(CurrencyService.ConvertCurrency(order.Sum, CurrencyValue, order.OrderCurrency.CurrencyValue), 2))
                            return NotificationMessahges.Fail;

                        string fileName = System.IO.Path.GetFileName(context.Request.FilePath);
                        NotificationType ntFile = Payment.NotificationType.None;
                        switch (fileName.ToLower())
                        {
                            case "paymentnotification.ashx":
                                ntFile = Payment.NotificationType.Handler;
                                break;
                            case "paymentreturnurl.aspx":
                                ntFile = Payment.NotificationType.ReturnUrl;
                                break;
                        }

                        if (((this.NotificationType & ntFile) == Payment.NotificationType.Handler) || ((this.NotificationType & ntFile) == Payment.NotificationType.ReturnUrl))
                            OrderService.PayOrder(order.OrderID, true);

                        return NotificationMessahges.SuccessfullPayment(orderNumber);
                    }
                }

                return NotificationMessahges.Fail;

                //string formContent = string.Format("cmd=_notify-validate");
                //req.ContentLength = formContent.Length;

                //using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
                //    sw.Write(formContent);

                //string response;
                //using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
                //    response = HttpUtility.UrlDecode(sr.ReadToEnd());

                //var success = response.Split('\n').Select(l => l.Trim()).First();
                //if (success != null && success.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                //{
                //    var orderNumber =
                //        response.Split('\n').FirstOrDefault(
                //            line => line.Contains('=') && line.Trim().Substring(0, line.IndexOf('=')) == "invoice");
                //    if (!string.IsNullOrEmpty(orderNumber))
                //        OrderService.PayOrder(OrderService.GetOrderIdByNumber(orderNumber), true);

                //    return NotificationMessahges.SuccessfullPayment(orderNumber);
                //}
                //return NotificationMessahges.Fail;
            }
            catch (Exception ex)
            {
                return NotificationMessahges.LogError(ex);
            }
            //TODO ORDER PAYMENT TEST
        }

        private string GetPrm(HttpContext context, string sName)
        {
            string sValue;
            if (context != null && !string.IsNullOrEmpty(sName))
            {
                sValue = context.Request.Form[sName];
                if (string.IsNullOrEmpty(sValue)) sValue = context.Request.QueryString[sName];
                if (string.IsNullOrEmpty(sValue)) sValue = string.Empty;
            }
            else sValue = string.Empty;

            return sValue;
        }
    }
}