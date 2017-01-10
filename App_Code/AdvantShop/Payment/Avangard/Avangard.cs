//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    public class Avangard : PaymentMethod
    {
        public override PaymentType Type
        {
            get { return PaymentType.Avangard; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.ServerRequest; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl | NotificationType.Handler; }
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
                               {AvangardTemplate.ShopId, ShopId},
                               {AvangardTemplate.ShopPassword, ShopPassword},
                               {AvangardTemplate.AvSign, AvSign}
                           };
            }
            set
            {
                if (value.ContainsKey(AvangardTemplate.ShopId))
                    ShopId = value[AvangardTemplate.ShopId];
                ShopPassword = value.ElementOrDefault(AvangardTemplate.ShopPassword);
                AvSign = value.ElementOrDefault(AvangardTemplate.AvSign);
            }
        }

        public string ShopId { get; set; }

        public string ShopPassword { get; set; }

        public string AvSign { get; set; }

        public override string ProcessServerRequest(Order order)
        {
            var responseTicket = GetTicket(order);
            if (responseTicket.ResponseCode != 0)
            {
                LogToFile("заказ " + order.OrderID + " пришел пустой тикет, " + responseTicket.ResponseMessage);
                return string.Empty;
            }

            PaymentService.SaveOrderpaymentInfo(order.OrderID, this.PaymentMethodId, AvangardTemplate.Ticket, responseTicket.Ticket);
            PaymentService.SaveOrderpaymentInfo(order.OrderID, this.PaymentMethodId, AvangardTemplate.OkCode, responseTicket.OkCode);
            PaymentService.SaveOrderpaymentInfo(order.OrderID, this.PaymentMethodId, AvangardTemplate.FailureCode, responseTicket.FailureCode);

            LogToFile("заказ " + order.OrderID + " получили коды операций и тикет. Тикет:" + responseTicket.Ticket + ", OkCode:" + responseTicket.OkCode + ", FailureCode:" + responseTicket.FailureCode);

            return string.Format("https://www.avangard.ru/iacq/pay?ticket={0}", responseTicket.Ticket);
        }

        public override string ProcessResponse(HttpContext context)
        {
            LogToFile("обрабатываем ответ от банка после оплаты" );
            string failNotification = "<span style=\"color:red;font-size:14px;\">Оплата не проведена: Отказ банка – эмитента карты. Ошибка в процессе оплаты, указаны неверные данные карты.</span>";
            HttpRequest req = context.Request;

            if (!string.IsNullOrEmpty(req["result_code"]))
            {
                var additionalInfo = PaymentService.GetOrderIdByPaymentIdAndCode(this.PaymentMethodId, req["result_code"]);
                if (additionalInfo.Name == AvangardTemplate.FailureCode)
                {
                    LogToFile("заказ " + additionalInfo.OrderId + " получили FailureCode от банка:" + additionalInfo.Value);
                    return failNotification;
                }

                var order = OrderService.GetOrder(additionalInfo.OrderId);
                if (order != null)
                {
                    LogToFile("заказ " + additionalInfo.OrderId + " получили OkCode от банка:" + additionalInfo.Value + " выставляем статус оплачено");
                    OrderService.PayOrder(order.OrderID, true);
                    context.Response.StatusCode = 202;
                    context.Response.Status = "202 Accepted";

                    return "<span style=\"color:green; font-size:14px; \">Оплата прошла успешно.</span>";
                    //NotificationMessahges.SuccessfullPayment(order.OrderID.ToString());
                }
                return failNotification;
            }
            else
            {
                LogToFile(" пришел пустой result_code");
                LogToFile("Пытаемся проверить signature");

                if (string.IsNullOrEmpty(req["status_code"]) || string.IsNullOrEmpty(req["shop_id"]) ||
                    string.IsNullOrEmpty(req["order_number"]) || string.IsNullOrEmpty(req["amount"]))
                {
                    LogToFile(
                        string.Format(
                            "Недостаточно данных для построения signature: result_code - {0}, shop_id - {1}, order_number - {2}, amount - {3}",
                            req["status_code"], req["shop_id"],
                            req["order_number"], req["amount"]));
                    return failNotification;
                }

                if (AvSign.IsNullOrEmpty())
                {
                    LogToFile("Пустой пароль av_sign");
                    return failNotification;
                }

                var sign = (AvSign.Md5(true) + (req["shop_id"] + req["order_number"] + req["amount"]).Md5(true)).Md5(true);

                if (sign != req["signature"])
                {
                    LogToFile(
                        string.Format(
                            "Не совпадает signature: signature - {0}, shop_id - {1}, order_number - {2}, amount - {3}, av_sign - {4}",
                            req["signature"], req["shop_id"],
                            req["order_number"], req["amount"], AvSign));
                    return failNotification;
                }

                var order = OrderService.GetOrder(req["order_number"].TryParseInt());
                if (order != null)
                {
                    if (req["status_code"] == ((int) OrderStstus.Payed).ToString())
                    {
                        LogToFile("заказ " + order.OrderID + "оплачен");
                        OrderService.PayOrder(order.OrderID, true);

                        context.Response.StatusCode = 202;
                        context.Response.Status = "202 Accepted";

                        return "<span style=\"color:green; font-size:14px; \">Оплата прошла успешно.</span>";
                    }
                    else
                    {
                        return failNotification;
                    }
                }
                return failNotification;
            }
        }

        private AvangardResponse GetTicket(Order order)
        {
            var result = new AvangardResponse();

            string sum = (Math.Round(order.Sum) * 100).ToString();

            var requestXmlString =
                "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" +
                "<NEW_ORDER>" +
                "<SHOP_ID>{0}</SHOP_ID>" +
                "<SHOP_PASSWD>{1}</SHOP_PASSWD>" +
                "<AMOUNT>{2}</AMOUNT>" +
                "<ORDER_NUMBER>{3}</ORDER_NUMBER>" +
                "<ORDER_DESCRIPTION>{4}</ORDER_DESCRIPTION>" +
                "<LANGUAGE>{5}</LANGUAGE>" +
                "<BACK_URL>{6}</BACK_URL>" +
                "<CLIENT_NAME>{7}</CLIENT_NAME>" +
                "<CLIENT_ADDRESS>{8}</CLIENT_ADDRESS>" +
                "<CLIENT_EMAIL>{9}</CLIENT_EMAIL>" +
                "<CLIENT_PHONE>{10}</CLIENT_PHONE>" +
                "<CLIENT_IP>{11}</CLIENT_IP>" +
                "</NEW_ORDER >";

            //System.Net.HttpWebRequest adds the header 'HTTP header "Expect: 100-Continue"' to every request unless you explicitly ask it not to by setting this static property to false:
            ServicePointManager.Expect100Continue = false;

            var postData = string.Format(requestXmlString,
               ShopId,
               ShopPassword,
               sum,
               order.OrderID.ToString(),
               Translit(GetOrderDescription(order.Number)),
               CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
               this.SuccessUrl,
               Translit(order.OrderCustomer.FirstName + " " + order.OrderCustomer.LastName),
               Translit(order.BillingContact.Country + "," + order.BillingContact.City + "," + order.BillingContact.Address),
               order.OrderCustomer.Email,
               order.OrderCustomer.MobilePhone,
               order.OrderCustomer.CustomerIP
               );

            LogToFile("заказ " + order.OrderID.ToString() + ", отправляем данные " + postData);

            WebRequest request = WebRequest.Create("https://www.avangard.ru/iacq/h2h/reg?xml=" + HttpUtility.UrlEncode(postData));
            request.Method = "GET";

            using (var reader = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.GetEncoding("windows-1251")))
            {
                var responseFromServer = reader.ReadToEnd();

                using (var xmlReader = XmlReader.Create(new StringReader(responseFromServer)))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            switch (xmlReader.Name)
                            {
                                case "id":
                                    int id;
                                    if (xmlReader.Read() && int.TryParse(xmlReader.Value, out id))
                                    {
                                        result.Id = id;
                                    }
                                    break;
                                case "ticket":
                                    if (xmlReader.Read())
                                    {
                                        result.Ticket = xmlReader.Value;
                                    }
                                    break;
                                case "ok_code":
                                    if (xmlReader.Read())
                                    {
                                        result.OkCode = xmlReader.Value;
                                    }
                                    break;
                                case "failure_code":
                                    if (xmlReader.Read())
                                    {
                                        result.FailureCode = xmlReader.Value;
                                    }
                                    break;
                                case "response_code":
                                    int responseCode;
                                    if (xmlReader.Read() && int.TryParse(xmlReader.Value, out responseCode))
                                    {
                                        result.ResponseCode = responseCode;
                                    }
                                    break;
                                case "response_message":
                                    if (xmlReader.Read())
                                    {
                                        result.ResponseMessage = xmlReader.Value;
                                    }
                                    break;
                            }
                        }
                    }
                }

                reader.Close();
            }

            return result;
        }

        private string Translit(string input)
        {
            var dictionary = new Dictionary<string, string>
                {
                    {"а","a"},
                    {"б","b"},
                    {"в","v"},
                    {"г","g"},
                    {"д","d"},
                    {"е","e"},
                    {"ё","jo"},
                    {"ж","zh"},
                    {"з","z"},
                    {"и","i"},
                    {"й","j"},
                    {"к","k"},
                    {"л","l"},
                    {"м","m"},
                    {"н","n"},
                    {"о","o"},
                    {"п","p"},
                    {"р","r"},
                    {"с","s"},
                    {"т","t"},
                    {"у","u"},
                    {"ф","f"},
                    {"х","h"},
                    {"ц","c"},
                    {"ч","ch"},
                    {"ш","sh"},
                    {"щ","shh"},
                    {"ъ",""},
                    {"ы","y"},
                    {"ь","'"},
                    {"э","je"},
                    {"ю","ju"},
                    {"я","ja"}
                };



            var output = string.Empty;
            input = input.ToLower();
            for (int i = 0; i < input.Length; i++)
            {
                output += dictionary.ContainsKey(input[i].ToString())
                              ? dictionary[input[i].ToString()].ToString()
                              : input[i].ToString();
            }
            return output;
        }

        private void LogToFile(string logMessage)
        {
            try
            {
                var fullFilePath = HttpContext.Current.Server.MapPath("~/App_Data/avangardLog.txt");

                if (!File.Exists(fullFilePath))
                {
                    File.Create(fullFilePath);
                }

                using (var streamWriter = new StreamWriter(fullFilePath, true))
                {
                    streamWriter.WriteLine(DateTime.Now.ToString() + " " + logMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("не удалось залогировать событие: " + logMessage + " Ошибка: " + ex);
            }
            
        }

        enum OrderStstus
        {
            NotFound = 0,
            Processing = 1,
            Discarded = 2,
            Payed = 3,
            PartialRefund = 4,
            Refund = 5
        }

    }
}