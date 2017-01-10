<%@ WebHandler Language="C#" Class="BitPayService" %>

using System;
using System.Net;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Text;
using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Payment;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;

public class BitPayService : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest (HttpContext context) 
    {
        context.Response.ContentType = "application/json";

        int orderId = 0;
        if (context.Request["orderId"].IsNullOrEmpty() || !Int32.TryParse(context.Request["orderId"], out orderId))
        {
            context.Response.Write(JsonConvert.SerializeObject(new { error = "error" }));
            return; 
        }

        var order = OrderService.GetOrder(orderId);
        if (order != null)
        {
            var payment = (BitPay)PaymentService.GetPaymentMethodByType(PaymentType.BitPay);

            if (payment != null)
            {
                string message = string.Format("&price={0}&currency={1}&orderID={2}&itemDesc={3}&physical=true",
                                    order.Sum.ToString("F2").Replace(",", "."),
                                    payment.Currency, order.OrderID, string.Format(Resources.Resource.Payment_OrderDescription, order.Number));

                try
                {
                    var request = (HttpWebRequest)WebRequest.Create("https://bitpay.com/api/invoice/");
                    request.Method = "POST";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(payment.ApiKey));
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Proxy = null;
                    request.Credentials = CredentialCache.DefaultCredentials;

                    var requestStream = request.GetRequestStream();

                    using (var sw = new StreamWriter(request.GetRequestStream(), Encoding.UTF8))
                        sw.Write(message);

                    string response = "";
                    using (var sr = new StreamReader(request.GetResponse().GetResponseStream()))
                        response = sr.ReadToEnd();
                    
                    var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
                    if (obj.ContainsKey("url"))
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { url = obj["url"], error = "" }));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex); 
                }
            }
        }
        
        context.Response.Write(JsonConvert.SerializeObject(new { error = "error" }));
    }
 
    public bool IsReusable 
    {
        get { return false; }
    }

}