using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace AdvantShop.Payment
{
    public class Qiwi : PaymentMethod
    {
        public string ProviderId { get; set; }
        public string RestId { get; set; }
        public string Password { get; set; }
        public string PasswordNotify { get; set; }
        public string ProviderName { get; set; }
        public string CurrencyCode { get; set; }
        public float CurrencyValue { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.QIWI; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.ServerRequest; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
        }


        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {QiwiTemplate.ProviderID, ProviderId},
                               {QiwiTemplate.RestID, RestId},
                               {QiwiTemplate.Password, Password},
                               {QiwiTemplate.PasswordNotify, PasswordNotify},
                               {QiwiTemplate.ProviderName, ProviderName},
                               {QiwiTemplate.CurrencyCode, CurrencyCode},
                               {QiwiTemplate.CurrencyValue, CurrencyValue.ToString()}
                           };
            }
            set
            {
                ProviderId = value.ElementOrDefault(QiwiTemplate.ProviderID);
                RestId = value.ElementOrDefault(QiwiTemplate.RestID);
                Password = value.ElementOrDefault(QiwiTemplate.Password);
                PasswordNotify = value.ElementOrDefault(QiwiTemplate.PasswordNotify);
                ProviderName = value.ElementOrDefault(QiwiTemplate.ProviderName);
                CurrencyCode = value.ElementOrDefault(QiwiTemplate.CurrencyCode);

                float decVal;
                CurrencyValue = value.ContainsKey(QiwiTemplate.CurrencyValue) &&
                                float.TryParse(value[QiwiTemplate.CurrencyValue], out decVal)
                                    ? decVal
                                    : 1;
            }
        }

        public override string ProcessServerRequest(Order order)
        {
            int retriesNum = 0;
            string result = "";
            var qiwiAnswer = new QiwiTemplate.QIWIAnswer();

            string orderStrId;

            do
            {
                // если заказ уже есть в системе qiwi, но был изменен на стороне магазина, подменяем id на id_номерпопытки
                orderStrId = retriesNum > 0
                    ? string.Format("{0}_{1}", order.OrderID, retriesNum)
                    : order.OrderID.ToString();

                var webRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://w.qiwi.com/api/v2/prv/{0}/bills/{1}",
                            Parameters[QiwiTemplate.ProviderID], orderStrId));
                webRequest.Headers["Authorization"] = "Basic " +
                                                      Convert.ToBase64String(
                                                          Encoding.UTF8.GetBytes(
                                                              (Parameters.ContainsKey(QiwiTemplate.RestID)
                                                                  ? Parameters[QiwiTemplate.RestID]
                                                                  : Parameters[QiwiTemplate.ProviderID])
                                                              + ":" + Parameters[QiwiTemplate.Password]));

                webRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
                webRequest.PreAuthenticate = true;
                webRequest.Method = "PUT";
                webRequest.Accept = "text/json";

                string data = string.Format("user={0}&amount={1}&ccy={2}&comment={3}&lifetime={4}",
                    HttpUtility.UrlEncode("tel:+" + (order.PaymentDetails != null ? order.PaymentDetails.Phone : "")),
                    (order.Sum/CurrencyValue).ToString("F2").Replace(",", "."),
                    Parameters[QiwiTemplate.CurrencyCode],
                    order.OrderID.ToString(),
                    HttpUtility.UrlEncode(DateTime.Now.AddDays(45).ToString("yyyy-MM-ddTHH:mm:ss"))
                    );

                var bytes = Encoding.UTF8.GetBytes(data);
                webRequest.ContentLength = bytes.Length;
                using (var requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }

                try
                {
                    using (var response = webRequest.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            if (stream != null)
                                using (var reader = new StreamReader(stream))
                                {
                                    result = reader.ReadToEnd();
                                }
                        }
                    }
                }
                catch (WebException e)
                {
                    using (var eResponse = e.Response)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                        {
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    result = reader.ReadToEnd();
                                }
                        }
                    }
                }
                retriesNum++;
                if (result.IsNotEmpty())
                    qiwiAnswer = JsonConvert.DeserializeObject<QiwiTemplate.QIWIAnswer>(result);

            } while (result.IsNotEmpty() && qiwiAnswer.response.result_code == (int)QiwiTemplate.CreateCode.BillIsExist && retriesNum < 10);

            if (result.IsNotEmpty())
            {
                if (qiwiAnswer.response.bill != null && qiwiAnswer.response.result_code == (int)QiwiTemplate.CreateCode.Sucsses && qiwiAnswer.response.bill.status == "waiting")
                {
                    return string.Format(
                        "https://w.qiwi.com/order/external/main.action?shop={0}&transaction={1}&successUrl={2}&failUrl={3}",
                        Parameters[QiwiTemplate.ProviderID], 
                        orderStrId,
                        HttpUtility.UrlEncode(SuccessUrl),
                        HttpUtility.UrlEncode(FailUrl)
                        );
                }
                else
                {
                    Debug.LogError(result);
                }
            }

            return string.Empty;
        }

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;
            int orderId = 0;

            string result;

            string orderStrId = req["bill_id"] ?? string.Empty;
            if (orderStrId.Contains("_TEST_"))
                orderStrId = orderStrId.Replace("_TEST_", string.Empty);
            else
                orderStrId = orderStrId.Contains("_") ? orderStrId.Split(new[] { '_' })[0] : orderStrId;

            if (req.Headers["Authorization"] == "Basic " + Convert.ToBase64String(
                Encoding.UTF8.GetBytes(Parameters[QiwiTemplate.ProviderID] + ":" + 
                (Parameters[QiwiTemplate.PasswordNotify].IsNotEmpty() ? Parameters[QiwiTemplate.PasswordNotify] : Parameters[QiwiTemplate.Password]))) && 
                int.TryParse(orderStrId, out orderId) && req["status"] == "paid")
            {
                Order order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    OrderService.PayOrder(orderId, true);
                    result = "<?xml version=\"1.0\"?><result><result_code>0</result_code></result>";
                }
                else
                {
                    result = "<?xml version=\"1.0\"?><result><result_code>300</result_code></result>";
                }
            }
            else
            {
                result = "<?xml version=\"1.0\"?><result><result_code>150</result_code></result>";
            }

            context.Response.Clear();
            context.Response.ContentType = "text/xml";
            context.Response.Write(result);
            context.Response.End();
            return result;
        }
    }
}