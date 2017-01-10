//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Web;
using AdvantShop.Orders;
using AdvantShop.Localization;

namespace AdvantShop.Payment
{
    public class OnPay : PaymentMethod
    {
        public string FormPay { get; set; }
        public string SendMethod { get; set; }
        public bool CheckMd5 { get; set; }
        public string SecretKey { get; set; }
        public string CurrencyLabel { get; set; }
        public float CurrencyValue { get; set; }

        public static Dictionary<string, string> GetCurrencies()
        {
            return Currencies;
        }
        public static readonly Dictionary<string, string> Currencies = new Dictionary<string, string>
                                                                           {
                                                                               {"EUR", "Банковский перевод EUR"},
                                                                               {"LIE", "Visa MasterCard EUR (LiqPay)"},
                                                                               {"LIQ", "Visa MasterCard RUR (LiqPay)"},
                                                                               {"LIU", "Visa MasterCard UAH (LiqPay)"},
                                                                               {"LIZ", "Visa MasterCard USD (LiqPay)"},
                                                                               {"LRU", "Liberty Reserve, LRUSD"},
                                                                               {"MCZ", "Вывод на карту MC Loyalbank в долл"},
                                                                               {"MMR", "Moneymail.ru"},
                                                                               {"PPL", "PayPal"},
                                                                               {"RUR", "Рублевый счет"},
                                                                               {"USD", "Банковский перевод USD"},
                                                                               {"VCZ", "Вывод на VISA долл"},
                                                                               {"WMB", "Webmoney WMB "},
                                                                               {"WME", "Webmoney WME"},
                                                                               {"WMR", "Webmoney WMR"},
                                                                               {"WMU", "Webmoney WMU"},
                                                                               {"WMZ", "Webmoney WMZ"},
                                                                               {"Y05", "Яндекс Карта 500 руб (1)"},
                                                                               {"YC1", "Яндекс Карта 1000 руб (0)"},
                                                                               {"YC3", "Яндекс Карта 3000 руб (0)"},
                                                                               {"YC5", "Яндекс Карта 5000 руб (0)"},
                                                                               {"YCX", "Яндекс Карта 10000 руб (0)"},
                                                                               {"YDM", "Яндекс.Деньги вывод"}
                                                                           };


        public override PaymentType Type
        {
            get { return PaymentType.OnPay; }
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
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl; }
        }
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>   
                             {
                                 {OnPayTemplate.FormPay, FormPay},
                                 {OnPayTemplate.SendMethod, SendMethod},
                                 {OnPayTemplate.CheckMd5, CheckMd5.ToString()},
                                 {OnPayTemplate.SecretKey, SecretKey},
                                 {OnPayTemplate.CurrencyLabel, CurrencyLabel},
                                 {OnPayTemplate.CurrencyValue, CurrencyValue.ToString()}
                             };

            }
            set
            {
                CurrencyLabel = !value.ContainsKey(OnPayTemplate.CurrencyLabel)
                                   ? "RUR"
                                   : value[OnPayTemplate.CurrencyLabel];
                FormPay = value.ElementOrDefault(OnPayTemplate.FormPay);
                bool checkedMd5;
                CheckMd5 = !bool.TryParse(value.ElementOrDefault(OnPayTemplate.CheckMd5), out checkedMd5) || checkedMd5;
                SendMethod = value.ElementOrDefault(OnPayTemplate.SendMethod);
                SecretKey = value.ElementOrDefault(OnPayTemplate.SecretKey);
                float decVal;
                CurrencyValue = value.ContainsKey(OnPayTemplate.CurrencyValue) &&
                                float.TryParse(value[OnPayTemplate.CurrencyValue], out decVal)
                                    ? decVal
                                    : 1;
            }
        }

        public override void ProcessForm(Order order)
        {
            string sum = System.Math.Round((order.Sum * CurrencyValue), 1).ToString().Replace(",", ".");
            if (CheckMd5)
            {
                new PaymentFormHandler
                {
                    FormName = "_xclick",
                    Method = FormMethod.GET,
                    Url = "http://secure.onpay.ru/pay/" + FormPay,
                    InputValues = new Dictionary<string, string>
                                      {
                                          {"url_success", SuccessUrl},
                                          {"pay_mode", "fix"},
                                          {"price", sum},
                                          {"currency", CurrencyLabel},
                                          {"pay_for", order.OrderID.ToString()},
                                          {"price_final", "true"},
                                          {
                                             "md5",
                                             ("fix" + sum + ";" + CurrencyLabel + ";" + order.Number + ";yes;" + SecretKey).Md5(true)
                                             }
                                      }
                }.Post();
            }

            else
            {
                new PaymentFormHandler
                {
                    FormName = "_xclick",
                    Method = FormMethod.GET,
                    Url = "http://secure.onpay.ru/pay/" + FormPay,
                    InputValues = new Dictionary<string, string>
                                      {
                                          {"url_success", SuccessUrl},
                                          {"pay_mode", "fix"},
                                          {"price", sum},
                                          {"currency", CurrencyLabel},
                                          {"pay_for", order.OrderID.ToString()}
                                      }
                }.Post();
            }
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            string sum = System.Math.Round((order.Sum * CurrencyValue), 1).ToString().Replace(",", ".");
            if (CheckMd5)
            {
                return new PaymentFormHandler
                 {
                     FormName = "_xclick",
                     Method = FormMethod.GET,
                     Page = page,
                     Url = "http://secure.onpay.ru/pay/" + FormPay,
                     InputValues = new Dictionary<string, string>
                                      {
                                          {"url_success", SuccessUrl},
                                          {"pay_mode", "fix"},
                                          {"price", sum},
                                          {"currency", CurrencyLabel},
                                          {"pay_for", order.OrderID.ToString()},
                                          {"price_final", "true"},
                                          {
                                             "md5",
                                             ("fix" + sum + ";" + CurrencyLabel + ";" + order.Number + ";yes;" + SecretKey).Md5(true)
                                             }
                                      }
                 }.ProcessRequest();
            }

            else
            {
                return new PaymentFormHandler
                 {
                     FormName = "_xclick",
                     Method = FormMethod.GET,
                     Page = page,
                     Url = "http://secure.onpay.ru/pay/" + FormPay,
                     InputValues = new Dictionary<string, string>
                                      {
                                          {"url_success", SuccessUrl},
                                          {"pay_mode", "fix"},
                                          {"price", sum},
                                          {"currency", CurrencyLabel},
                                          {"pay_for", order.OrderID.ToString()}
                                      }
                 }.ProcessRequest();
            }
        }

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;
            int orderID = 0;

            // пришел запрос с проверкой, необходимо отдать xml-ответ
            if (req["type"].IsNotEmpty() && req["type"] == "check")
            {
                context.Response.Write(GetCheckResponseXML(req));
                context.Response.End();
                return "";
            }

            // пришел запрос об оплате
            if (req["type"].IsNotEmpty() && req["type"] == "pay")
            {
                if (int.TryParse(req["pay_for"], out orderID))
                {
                    Order order = OrderService.GetOrder(orderID);
                    if (order != null)
                    {
                        OrderService.PayOrder(orderID, true);

                        context.Response.Write(GetPayResponseXML(0, orderID, req["order_amount"], req["onpay_id"], "OK", req["order_currency"]));
                        context.Response.End();
                        return "";
                    }
                }

                context.Response.Write(GetPayResponseXML(2, orderID, req["order_amount"], req["onpay_id"], "Error", req["order_currency"]));
                context.Response.End();
                return "";
            }


            if (CheckFields(req) && int.TryParse(req["pay_for"], out orderID))
            {
                Order order = OrderService.GetOrder(orderID);
                if (order != null)
                {
                    OrderService.PayOrder(orderID, true);
                    return NotificationMessahges.SuccessfullPayment(orderID.ToString());
                }
            }
            return "";
        }

        private string GetCheckResponseXML(HttpRequest req)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<result>");
            sb.AppendLine("<code>" + 0 + "</code>");
            sb.AppendLine("<pay_for>" + req["pay_for"] + "</pay_for>");
            sb.AppendLine("<comment>" + "OK" + "</comment>");
            sb.AppendLine("<md5>" + string.Format("check;{0};{1};{2};{3};{4}", req["pay_for"], req["order_amount"], req["order_currency"], 0, SecretKey).Md5(true) + "</md5>");
            sb.AppendLine("</result>");
            return sb.ToString();
        }

        /// <summary>
        /// Возвращает xml при запросе об оплате
        /// </summary>
        /// <param name="iCode">0 - ok, 2 -error</param>
        /// <param name="pay_for">orderId из запроса</param>
        /// <param name="order_amount">кол-во</param>
        /// <param name="onpay_id">onpay_id</param>
        /// <param name="comment">OK - все хорошо, иначе ошибка</param>
        /// <param name="order_currency">order_currency</param>
        /// <returns></returns>
        private string GetPayResponseXML(int iCode, int pay_for, string order_amount, string onpay_id, string comment, string order_currency)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<result>");
            sb.AppendLine("<code>" + iCode + "</code>");
            sb.AppendLine("<comment>" + comment + "</comment>");
            sb.AppendLine("<onpay_id>" + onpay_id + "</onpay_id>");
            sb.AppendLine("<pay_for>" + pay_for + "</pay_for>");
            sb.AppendLine("<order_id>" + pay_for + "</order_id>");
            sb.AppendLine("<md5>" + string.Format("pay;{0};{1};{2};{3};{4};{5};{6}", pay_for, onpay_id, pay_for, order_amount, order_currency, iCode, SecretKey).Md5(true) + "</md5>");
            sb.AppendLine("</result>");
            return sb.ToString();
        }

        private bool CheckFields(HttpRequest req)
        {
            if (CheckMd5)
            {
                if (string.IsNullOrEmpty(req["price"]) || string.IsNullOrEmpty(req["pay_for"]) || string.IsNullOrEmpty(req["md5"]))
                    return false;
                if (req["md5"].ToLower() !=
                     (req["pay_mode"] + req["price"] + ";" + req["currency"] + ";" + req["pay_for"] + ";yes;" + SecretKey).Md5(true))
                    return false;
                return true;
            }
            else
            {
                return !(string.IsNullOrEmpty(req["pay_for"]));  // string.IsNullOrEmpty(req["price"]) || 
            }
        }
    }
}