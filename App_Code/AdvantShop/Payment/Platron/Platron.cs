//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    public class Platron : PaymentMethod
    {
        private const string Url = "https://www.platron.ru/payment.php";
        private const string Separator = ";";

        private const string ResultFormat =
            @"<?xml version='1.0' encoding='utf-8'?>
                <response>
	                <pg_salt>{0}</pg_salt>
	                <pg_status>{1}</pg_status>
	                <pg_description>{2}</pg_description>
	                <pg_sig>{3}</pg_sig>
                </response>";


        public string MerchantId { get; set; }
        public string Currency { get; set; }
        public string PaymentSystem { get; set; }
        public float CurrencyValue { get; set; }
        public string SecretKey { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.Platron; }
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
            get { return UrlStatus.CancelUrl | UrlStatus.FailUrl | UrlStatus.NotificationUrl | UrlStatus.ReturnUrl; }
        }
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {PlatronTemplate.MerchantId , MerchantId},
                               {PlatronTemplate.Currency , Currency},
                               {PlatronTemplate.PaymentSystem , PaymentSystem},
                               {PlatronTemplate.CurrencyValue  , CurrencyValue.ToString()},
                               {PlatronTemplate.SecretKey, SecretKey}
                           };
            }
            set
            {
                MerchantId = value.ElementOrDefault(PlatronTemplate.MerchantId);
                Currency = value.ElementOrDefault(PlatronTemplate.Currency);
                PaymentSystem = value.ElementOrDefault(PlatronTemplate.PaymentSystem);
                float decVal = 0;
                CurrencyValue = value.ContainsKey(PlatronTemplate.CurrencyValue) && float.TryParse(value[PlatronTemplate.CurrencyValue], out decVal) ? decVal : 1;
                SecretKey = value.ElementOrDefault(PlatronTemplate.SecretKey);
            }
        }

        public static Dictionary<string, string> GetCurrencies()
        {
            return Currencies;
        }
        public static readonly Dictionary<string, string> Currencies = new Dictionary<string, string>
                                                                           {
                                                                               {"RUR", "Российские рубли"},
                                                                               {"USD", "Доллары США"},
                                                                               {"EUR", "Евро"},
                                                                           };
        public static Dictionary<string, string> GetPaymentSystems()
        {
            return PaymentSystems;
        }
        public static readonly Dictionary<string, string> PaymentSystems = new Dictionary<string, string>
                                                                           {
                                                                               {"WEBMONEYR", "ЭПС WebMoney, R-кошельки"},
                                                                               {"WEBMONEYZ", "ЭПС WebMoney, Z-кошельки"},
                                                                               {"WEBMONEYE", "ЭПС WebMoney, E-кошельки"},
                                                                               {"WEBMONEYRBANK","ЭПС WebMoney, R-кошельки с перечислением на расчетный счет в банке"},
                                                                               {"YANDEXMONEY","ЭПС Яндекс.Деньги"},
                                                                               {"MONEYMAILRU","ЭПС деньги@mail.ru"},
                                                                               {"RBKMONEY","ЭПС RbkMoney"},
                                                                               {"TRANSCRED","Кредитные карты через процессинг Транскредит банка"},
                                                                               {"RAIFFEISEN","Кредитные карты через процессинг Райффайзен банка"},
                                                                               {"EUROSET","EUROSET"},
                                                                               {"ELECSNET","Терминалы Элекснет"},
                                                                               {"OSMP","Терминалы ОСМП / QIWI"},
                                                                               {"OSMP-II","Терминалы ОСМП / QIWI с активационным платежом"},
                                                                               {"BEELINEMK","Счет на телефоне Билайн"},
                                                                               {"UNIKASSA","Терминалы Уникасса"},
                                                                               {"COMEPAY","Терминалы ComePay"},
                                                                               {"PINPAY","Терминалы PinPay Express"},
                                                                               {"MOBW","Мобильный кошелек ОСМП / QIWI "},
                                                                               {"CONTACT","Система приёма платежей «Контакт»"},
                                                                               {"MASTERBANK","Банкоматы МастерБанка"},
                                                                               {"CASH","Наличные (включает EUROSET, ELECSNET, OSMP, OSMP-II, UNIKASSA, COMEPAY, ALLOCARD, CONTACT, MASTERBANK, PINPAY)"}
                                                                           };

        public override void ProcessForm(Order order)
        {
            var paymentNo = order.OrderID.ToString();
            var sum = (order.Sum / CurrencyValue).ToString("F2").Replace(",", ".");
            var tempRandomString = new Guid().ToString();
            new PaymentFormHandler
                {
                    Url = Url,
                    InputValues = new Dictionary<string, string>
                                      {
                                          {"pg_amount", sum},
                                          {"pg_currency", Currency},
                                          {"pg_description",Resources.Resource.Client_OrderConfirmation_PayOrder + " #" +order.OrderID},
                                          {"pg_merchant_id", MerchantId},
                                          {"pg_order_id", paymentNo},
                                          {"pg_salt", tempRandomString},
                                          {"pg_sig", GetSignature("payment.php"+Separator
                                                                   + sum + Separator
                                                                   + Currency + Separator
                                                                   + Resources.Resource.Client_OrderConfirmation_PayOrder + " #" + order.OrderID + Separator
                                                                   + MerchantId + Separator
                                                                   +paymentNo +Separator
                                                                   +tempRandomString )}
                                      }
                }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            var paymentNo = order.OrderID.ToString();
            var sum = (order.Sum / CurrencyValue).ToString("F2").Replace(",", ".");
            var tempRandomString = new Guid().ToString();
            return new PaymentFormHandler
              {
                  Url = Url,
                  Page = page,
                  InputValues = new Dictionary<string, string>
                                      {
                                          {"pg_amount", sum},
                                          {"pg_currency", Currency},
                                          {"pg_description",Resources.Resource.Client_OrderConfirmation_PayOrder + " #" +order.OrderID},
                                          {"pg_merchant_id", MerchantId},
                                          {"pg_order_id", paymentNo},
                                          {"pg_salt", tempRandomString},
                                          {"pg_sig", GetSignature("payment.php"+Separator 
                                                                   + sum + Separator
                                                                   + Currency + Separator
                                                                   + Resources.Resource.Client_OrderConfirmation_PayOrder + " #" + order.OrderID + Separator
                                                                   + MerchantId + Separator
                                                                   + paymentNo + Separator
                                                                   + tempRandomString )}
                                      }
              }.ProcessRequest();
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            //if (!CheckData(req))
            //    return InvalidRequestData;
            var paymentNumber = req["pg_order_id"];
            try
            {
                int orderID = 0;
                if (int.TryParse(paymentNumber, out orderID) && OrderService.GetOrder(orderID) != null)
                {
                    if (!string.IsNullOrWhiteSpace(req["pg_result"]) && req["pg_result"].Trim() == "1")
                    {
                        OrderService.PayOrder(orderID, true);
                        return SuccessfullPayment(paymentNumber);
                    }
                    else if (string.IsNullOrWhiteSpace( req["pg_refund_type"]))
                    {
                        return string.Empty;
                    }
                }

                
            }
            catch { }
            return RejectedResponse;
        }

        protected string InvalidRequestData
        {
            get
            {
                const string desc = "Order not found";
                const string status = "error";
                return FormatNotificationResponse(desc, status);
            }
        }
        protected string RejectedResponse
        {
            get
            {
                const string desc = "Order not found";
                const string status = "rejected";
                return FormatNotificationResponse(desc, status);
            }
        }
        protected string SuccessfullPayment(string orderNumber)
        {
            const string desc = "Order payed";
            const string status = "ok";
            return FormatNotificationResponse(desc, status);

        }
        protected string FormatNotificationResponse(string desc, string status)
        {
            var salt = new Guid().ToString();
            return string.Format(ResultFormat, salt, status, desc,
                                 GetSignature(HttpContext.Current.Request.UrlReferrer + Separator + desc + Separator +
                                              salt + Separator + status));
        }

        private string GetSignature(string fields)
        {
            return (fields + Separator + SecretKey).Md5(false, Encoding.UTF8);
        }

        private bool CheckData(HttpRequest req)
        {
            //return !new[]
            //            {
            //                "pg_salt",
            //                "pg_order_id",
            //                "pg_payment_id",
            //                "pg_payment_system",
            //                "pg_amount",
            //                "pg_currency",
            //                "pg_net_amount",
            //                "pg_ps_amount",
            //                "pg_ps_currency",
            //                "pg_ps_full_amount",
            //                "pg_payment_date",
            //                "pg_can_reject",
            //                "pg_result",
            //                "pg_sig"
            //            }.Any(field => string.IsNullOrEmpty(req[field]))
            //       &&
            //       (req["pg_salt"] + Separator +
            //        req["pg_order_id"] + Separator +
            //        req["pg_payment_id"] + Separator +
            //        req["pg_payment_system"] + Separator +
            //        req["pg_amount"] + Separator +
            //        req["pg_ps_amount"] + Separator +
            //        req["pg_net_amount"] + Separator +
            //        SecretKey + Separator +
            //        req["pg_ps_currency"] + Separator +
            //        req["pg_ps_full_amount"] + Separator +
            //        req["pg_payment_date"] + Separator +
            //        req["pg_can_reject"] + Separator +
            //        req["pg_result"])
            //           .Md5() == req["pg_sig"];

            if (string.IsNullOrWhiteSpace(req["pg_sig"]))
            {
                return false;
            }

            var parameters = new Dictionary<string, string>
                {
                    {"pg_salt",req["pg_salt"]},
                    {"pg_order_id",req["pg_order_id"]},
                    {"pg_payment_id",req["pg_payment_id"]},
                    {"pg_payment_system",req["pg_payment_system"]},
                    {"pg_amount",req["pg_amount"]},
                    {"pg_currency",req["pg_currency"]},
                    {"pg_net_amount",req["pg_net_amount"]},
                    {"pg_ps_amount",req["pg_ps_amount"]},
                    {"pg_ps_currency",req["pg_ps_currency"]},
                    {"pg_ps_full_amount",req["pg_ps_full_amount"]},
                    {"pg_payment_date",req["pg_payment_date"]},
                    {"pg_can_reject",req["pg_can_reject"]},
                    {"pg_result",req["pg_result"]}
                }.OrderBy(pair => pair.Key);
            var stringForSig = string.Empty;
            for (int i = 0; i < parameters.Count(); ++i)
            {
                if (!string.IsNullOrWhiteSpace(parameters.ElementAt(i).Value))
                {
                    stringForSig += parameters.ElementAt(i).Value + Separator;
                }
            }
            stringForSig += SecretKey;

            return string.Equals(stringForSig.Md5(), req["pg_sig"]);
        }
    }
}