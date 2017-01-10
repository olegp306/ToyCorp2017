//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    public class IntellectMoneyMainProtocol : PaymentMethod
    {
        private const string Separator = "::";

        public string EshopId { get; set; }
        public string RecipientCurrency { get; set; }
        public string Preference { get; set; }
        public float CurrencyValue { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.IntellectMoneyMainProtocol; }
        }
        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }
        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl; }
        }
        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.ReturnUrl | UrlStatus.CancelUrl; }
        }
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {IntellectMoneyMainProtocolTemplate.EshopId , EshopId},
                               {IntellectMoneyMainProtocolTemplate.RecipientCurrency  , RecipientCurrency},
                               {IntellectMoneyMainProtocolTemplate.Preference , Preference},
                               {IntellectMoneyMainProtocolTemplate.CurrencyValue  , CurrencyValue.ToString()}
                           };
            }
            set
            {
                EshopId = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.EshopId);
                RecipientCurrency = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.RecipientCurrency);
                Preference = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.Preference);
                float decVal = 0;
                CurrencyValue = value.ContainsKey(IntellectMoneyMainProtocolTemplate.CurrencyValue) && float.TryParse(value[IntellectMoneyMainProtocolTemplate.CurrencyValue], out decVal) ? decVal : 1;
            }
        }

        public static Dictionary<string, string> GetCurrencies()
        {
            return Currencies;
        }
        public static readonly Dictionary<string, string> Currencies = new Dictionary<string, string>
                                                                           {
                                                                               {"RUR", "Российские рубли"},
                                                                               {"UAH","Украинские гривны"},
                                                                               {"USD", "Доллары США"},
                                                                               {"EUR", "Евро"},
                                                                           };
        public static Dictionary<string, string> GetPaymentSystems()
        {
            return PaymentSystems;
        }
        public static readonly Dictionary<string, string> PaymentSystems = new Dictionary<string, string>
                                                                           {
                                                                               {"", "Не выбран"},
                                                                               {"inner", "Оплата с кошелька Rbk Money"},
                                                                               {"bankCard", "Банковская карта Visa/MasterCard"},
                                                                               {"exchangers","Электронные платежные системы"},
                                                                               {"prepaidcard","Предоплаченная карта RBK Money"},
                                                                               {"transfers","Системы денежных переводов"},
                                                                               {"terminals","Платёжные терминалы"},
                                                                               {"iFree","SMS"},
                                                                               {"bank","Банковский платёж"},
                                                                               {"postRus","Почта России"},
                                                                               {"atm","Банкоматы"},
                                                                               {"yandex","Яндекс"},
                                                                               {"ibank","Интернет банкинг"},
                                                                               {"euroset","Евросеть"}
                                                                           };

        public override void ProcessForm(Order order)
        {
            var paymentNo = order.OrderID.ToString();
            var sum = (order.Sum / CurrencyValue).ToString("F2").Replace(",", ".");
            new PaymentFormHandler
            {
                Url = "https://merchant.intellectmoney.ru/ru/",
                InputValues = new Dictionary<string, string>
                                      {
                                          {"eshopId", EshopId },
                                          {"orderId", paymentNo},
                                          {"serviceName", "Order #" +order.OrderID},
                                          {"recipientAmount", sum},
                                          {"recipientCurrency", RecipientCurrency},
                                          {"preference", Preference},
                                          {"user_email", order.OrderCustomer.Email}
                                      }
            }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            var paymentNo = order.OrderID.ToString();
            var sum = (order.Sum / CurrencyValue).ToString("F2").Replace(",", ".");
            return new PaymentFormHandler
             {
                 Url = "https://merchant.intellectmoney.ru/ru/",
                 Page = page,
                 InputValues = new Dictionary<string, string>
                                      {
                                          {"eshopId", EshopId },
                                          {"orderId", paymentNo},
                                          {"serviceName", "Order #" +order.OrderID},
                                          {"recipientAmount", sum},
                                          {"recipientCurrency", RecipientCurrency},
                                          {"preference", Preference},
                                          {"user_email", order.OrderCustomer.Email}
                                      }
             }.ProcessRequest();
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            if (!CheckData(req))
            {
                Debug.LogError(req.ServerVariables["ALL_RAW"]);
                return NotificationMessahges.InvalidRequestData;
            }
                

            var paymentNumber = req["orderId"];
            int orderID = 0;
            if (int.TryParse(paymentNumber, out orderID) && OrderService.GetOrder(orderID) != null)
            {
                OrderService.PayOrder(orderID, true);
                return NotificationMessahges.SuccessfullPayment(paymentNumber);
            }
            return NotificationMessahges.Fail;
        }

        private static bool CheckData(HttpRequest req)
        {
            return !new[]
                        {
                            "eshopId",
                            "paymentId",
                            "orderId",
                            "eshopAccount",
                            "serviceName",
                            "recipientAmount",
                            "recipientCurrency",
                            "paymentStatus",
                            "userName",
                            "userEmail",
                            "paymentData",
                            "secretKey",
                            "hash"
                        }.Any(field => string.IsNullOrEmpty(req[field]))
                               &&
                               (req["eshopId"] + Separator +
                               req["paymentId"] + Separator +
                                req["orderId"] + Separator +
                                req["eshopAccount"] + Separator +
                                req["serviceName"] + Separator +
                                req["recipientAmount"] + Separator +
                                req["recipientCurrency"] + Separator +
                                req["paymentStatus"] + Separator +
                                req["userName"] + Separator +
                                req["userEmail"] + Separator +
                                req["paymentData"] + Separator +
                                req["secretKey"]).Md5() == req["hash"];
        }
    }
}