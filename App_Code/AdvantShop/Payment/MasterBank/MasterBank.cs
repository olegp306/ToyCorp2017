//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for WebMiney
    /// </summary>
    public class MasterBank : PaymentMethod
    {
        public string Terminal { get; set; }

        public string SecretWord { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.MasterBank; }
        }
        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }
        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
        }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {MasterBankTemplate.Terminal, Terminal},
                               {MasterBankTemplate.SecretWord, SecretWord}
                           };
            }
            set
            {
                Terminal = value.ElementOrDefault(MasterBankTemplate.Terminal);
                SecretWord = value.ElementOrDefault(MasterBankTemplate.SecretWord);
            }
        }
        public override void ProcessForm(Order order)
        {

            //1.	AMOUNT (����� � ������. ����������� ������ � �����)
            //2.	ORDER (���������� ����� ������. ������ ���� ����������. �������������� ��� ���������� �������. ��������� ������ ����� ������� 6-32 ��������.)
            //3.	MERCH_URL (URL, ������� ������������� �� ������ ������ � �������. ���� �� �����, ������� �� ���� �������� ���������)
            //4.	TERMINAL (��� ���������, ������������� ������)
            //5.	COUNTRY (������. ����������� ����������, ���� �������� ��������� �� � ������)
            //6.	TIMESTAMP (����� ���������� �������� � GMT (-4 ���� �� �����������). ������ YYYYMMDDHHMMSS)
            //7.	SIGN (�������� �������. ��������� �� ���������: md5(TERMINAL. TIMESTAMP.ORDER.AMOUNT.<��������� �����>) ����� ����� ����������� � �������� ������������)

            var orderIdString = string.Empty;
            for (int i = 0; i < 6 - order.OrderID.ToString().Length; i++)
            {
                orderIdString += "0";
            }
            orderIdString += order.OrderID;
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            new PaymentFormHandler
                {
                    Url = "https://pay.masterbank.ru/acquiring",
                    InputValues = new Dictionary<string, string>
                                      {
                                          {"AMOUNT", order.Sum.ToString()},
                                          {"ORDER", orderIdString},
                                          {"MERCH_URL", SuccessUrl},
                                          {"TERMINAL", Terminal},
                                          {"TIMESTAMP", timestamp},
                                          {"SIGN", GetMd5Hash(MD5.Create(),Terminal + timestamp + orderIdString + order.Sum.ToString() + SecretWord)}
                                      }
                }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            //1.	AMOUNT (����� � ������. ����������� ������ � �����)
            //2.	ORDER (���������� ����� ������. ������ ���� ����������. �������������� ��� ���������� �������. ��������� ������ ����� ������� 6-32 ��������.)
            //3.	MERCH_URL (URL, ������� ������������� �� ������ ������ � �������. ���� �� �����, ������� �� ���� �������� ���������)
            //4.	TERMINAL (��� ���������, ������������� ������)
            //5.	COUNTRY (������. ����������� ����������, ���� �������� ��������� �� � ������)
            //6.	TIMESTAMP (����� ���������� �������� � GMT (-4 ���� �� �����������). ������ YYYYMMDDHHMMSS)
            //7.	SIGN (�������� �������. ��������� �� ���������: md5(TERMINAL. TIMESTAMP.ORDER.AMOUNT.<��������� �����>) ����� ����� ����������� � �������� ������������)

            var orderIdString = string.Empty;
            for (int i = 0; i < 6 - order.OrderID.ToString().Length; i++)
            {
                orderIdString += "0";
            }
            orderIdString += order.OrderID;

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            return new PaymentFormHandler
             {
                 Url = "https://pay.masterbank.ru/acquiring",
                 Page = page,
                 InputValues = new Dictionary<string, string>
                                      {
                                          {"AMOUNT", order.Sum.ToString().Replace(',','.')},
                                          {"ORDER", orderIdString},
                                          {"MERCH_URL", SuccessUrl},
                                          {"TERMINAL", Terminal},
                                          {"TIMESTAMP", timestamp },
                                          {"SIGN", GetMd5Hash(MD5.Create(), Terminal + timestamp + orderIdString + order.Sum.ToString().Replace(',','.') + SecretWord)}
                                      }
             }.ProcessRequest();
        }

        public override string ProcessResponse(HttpContext context)
        {
            //1.	RESULT (��������� ��������. 0 � �������� 2 � ��������� 3 � ����������� �������� )
            //2.	RC (��� ������ ISO8583)
            //3.	CURRENCY (������)
            //4.	ORDER
            //5.	RRN (����� �������� � �������� �������)
            //6.	INT_REF (���������� ��� ��������)
            //7.	AUTHCODE (��� �����������. ����� �������������)
            //8.	PAN (��������������� ����� �����)
            //9.	TRTYPE (��� ��������. 0 � ����������� (��������� ������ ������������), 21 � ���������� �������, 24 � �������.)
            //10.	TIMESTAMP
            //11.	SIGN (������������ ��� ������������ �������)
            //12.	AMOUNT
            var req = context.Request;

            if (!CheckData(req))
                return NotificationMessahges.InvalidRequestData;

            var paymentNumber = req["ORDER"];

            if (string.IsNullOrEmpty(req["AMOUNT"]) || string.IsNullOrEmpty(req["ORDER"]) || string.IsNullOrEmpty(req["RRN"]) || string.IsNullOrEmpty(req["INT_REF"]))
            {
                return NotificationMessahges.Fail;
            }

            int orderID;
            if (int.TryParse(paymentNumber, out orderID))
            {
                var order = OrderService.GetOrder(orderID);
                if (order != null)
                {
                    SaveResponceParametr(orderID, req["AMOUNT"], req["ORDER"], req["RRN"], req["INT_REF"]);
                    return NotificationMessahges.SuccessfullPayment(order.Number);
                }
            }
            return NotificationMessahges.Fail;
        }

        public bool CheckData(HttpRequest req)
        {
            return true;
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public bool ProcessCloseRollbackOperation(int orderId, string type)
        {
            var rollbackParams = SQLDataAccess.ExecuteReadOne<RollbackParams>(
                "Select * From [ModulePayment].[MasterBank] Where OrderId = @OrderId",
                CommandType.Text,
                reader => new RollbackParams
                {
                    orderId = SQLDataHelper.GetInt(reader, "OrderId"),
                    order = SQLDataHelper.GetString(reader, "StringOrderId"),
                    amount = SQLDataHelper.GetString(reader, "Amount"),
                    rrn = SQLDataHelper.GetString(reader, "RRN"),
                    int_ref = SQLDataHelper.GetString(reader, "INT_REF")
                },
                new SqlParameter("@OrderId", orderId));
            if (rollbackParams == null)
            {
                return false;
            }

            var post_data = "AMOUNT={0}&ORDER={1}&RRN={2}&INT_REF={3}&TIMESTAMP={4}&TERMINAL={5}&SIGN={6}";
            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            WebRequest request = WebRequest.Create("https://pay.masterbank.ru/acquiring/" + type + "?");

            byte[] data = Encoding.UTF8.GetBytes(string.Format(post_data,
                rollbackParams.amount,
                rollbackParams.order,
                rollbackParams.rrn,
                rollbackParams.int_ref,
                timeStamp,
                Terminal,
                GetMd5Hash(MD5.Create(), Terminal + timeStamp + rollbackParams.order + rollbackParams.amount + SecretWord)
                ));

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            request.Method = "POST";
            request.GetResponse();
            if (string.Equals(type, "close"))
            {
                OrderService.PayOrder(rollbackParams.orderId, true);
            }
            else if (string.Equals(type, "rollback")) { }

            SQLDataAccess.ExecuteNonQuery(
              "Delete From [ModulePayment].[MasterBank] Where OrderId = @OrderId",
              CommandType.Text,
              new SqlParameter("@OrderId", orderId));

            return true;
        }

        private void SaveResponceParametr(int orderId, string Amount, string order, string rrn, string int_ref)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert Into [ModulePayment].[MasterBank] ([OrderId],[Amount],[StringOrderId],[RRN],[INT_REF]) VALUES (@OrderId,@Amount,@StringOrderId,@RRN,@INT_REF)",
                CommandType.Text,
                new SqlParameter("@OrderId", orderId),
                new SqlParameter("@Amount", Amount),
                new SqlParameter("@StringOrderId", order),
                new SqlParameter("@RRN", rrn),
                new SqlParameter("@INT_REF", int_ref));
        }
    }
}