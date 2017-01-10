//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;
using AdvantShop.Orders;
using AdvantShop.Localization;
using AdvantShop.Diagnostics;

namespace AdvantShop.Payment
{
    public class IntellectMoney : PaymentMethod
    {
        public string MerchantId { get; set; }
        public string SecretKey { get; set; }


        public override PaymentType Type
        {
            get { return PaymentType.IntellectMoney; }
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
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>   
                             {
                                 {IntellectMoneyTemplate.MerchantId, MerchantId},
                                 {IntellectMoneyTemplate.SecretKey, SecretKey}
                             };

            }
            set
            {
                MerchantId = value.ElementOrDefault(IntellectMoneyTemplate.MerchantId);
                SecretKey = value.ElementOrDefault(IntellectMoneyTemplate.SecretKey);
            }
        }

        public override void ProcessForm(Order order)
        {
            new PaymentFormHandler
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = "https://Merchant.IntellectMoney.ru/",
                InputValues = new Dictionary<string, string>
                                      {
                                          {"lmi_payee_purse", MerchantId},
                                          {"LMI_PAYMENT_AMOUNT", order.Sum.ToString("F2").Replace(",", ".")},
                                          {"LMI_PAYMENT_DESC", GetOrderDescription(order.Number) },
                                          {"LMI_PAYMENT_NO", order.OrderID.ToString() },
                                          {"lmi_result_url", NotificationUrl },
                                          {"lmi_success_url", SuccessUrl },
                                          {"lmi_success_method", "1"}, // POST
                                          {"lmi_fail_url", FailUrl },
                                          {"lmi_fail_method", "1"}, // POST
                                          //{"lmi_expire_date", System.DateTime.Now.AddDays(10).ToString("yyyyMMdd hh:MM:ss")}, // ���� �������� ���
                                          //{"lmi_sim_mode", Sandbox ? "1" : "0"},
                                          //{"EMAIL", order.OrderCustomer.Email},
                                      }
            }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            return new PaymentFormHandler
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = "https://Merchant.IntellectMoney.ru/",
                InputValues = new Dictionary<string, string>
                                      {
                                          {"lmi_payee_purse", MerchantId},
                                          {"LMI_PAYMENT_AMOUNT", order.Sum.ToString("F2").Replace(",", ".")},
                                          {"LMI_PAYMENT_DESC", GetOrderDescription(order.Number) },
                                          {"LMI_PAYMENT_NO", order.OrderID.ToString() },
                                          {"lmi_result_url", NotificationUrl },
                                          {"lmi_success_url", SuccessUrl },
                                          {"lmi_success_method", "1"}, // POST
                                          {"lmi_fail_url", FailUrl },
                                          {"lmi_fail_method", "1"}, // POST
                                          //{"lmi_expire_date", System.DateTime.Now.AddDays(10).ToString("yyyyMMdd hh:MM:ss")}, // ���� �������� ���
                                          //{"lmi_sim_mode", Sandbox ? "1" : "0"},
                                          //{"EMAIL", order.OrderCustomer.Email},
                                      }
            }.ProcessRequest();
        }

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;
            int orderID = 0;

            // ����� ���������������� �������
            if (req["LMI_PREREQUEST"].IsNullOrEmpty() && req["LMI_PREREQUEST"] == "1")
            {
                context.Response.Write("YES");
                context.Response.End();
                return string.Empty;
            }

            if (CheckFields(req) && int.TryParse(req["LMI_PAYMENT_NO"], out orderID))
            {
                Order order = OrderService.GetOrder(orderID);
                if (order != null)
                {
                    OrderService.PayOrder(orderID, true);

                    context.Response.Write("YES");
                    context.Response.End();
                    return string.Empty;
                    //return NotificationMessahges.SuccessfullPayment(orderID.ToString());
                }
            }

            if (context.Request.Url.AbsolutePath.EndsWith(".aspx"))
                return "����� ������� �������.";

            return NotificationMessahges.InvalidRequestData;
        }

        private bool CheckFields(HttpRequest req)
        {
            return !new[]
                        {
                            "LMI_PAYEE_PURSE",
                            "LMI_PAYMENT_AMOUNT",
                            "LMI_PAYMENT_NO",
                            "LMI_MODE",
                            "LMI_SYS_INVS_NO",
                            "LMI_SYS_TRANS_NO",
                            "LMI_SYS_TRANS_DATE",
                            "LMI_PAYER_PURSE",
                            "LMI_PAYER_WM"
                        }.Any(field => string.IsNullOrEmpty(req[field]))
                       &&
                       (req["LMI_PAYEE_PURSE"] +
                        req["LMI_PAYMENT_AMOUNT"] +
                        req["LMI_PAYMENT_NO"] +
                        req["LMI_MODE"] +
                        req["LMI_SYS_INVS_NO"] +
                        req["LMI_SYS_TRANS_NO"] +
                        req["LMI_SYS_TRANS_DATE"] +
                        SecretKey +
                        req["LMI_PAYER_PURSE"] +
                        req["LMI_PAYER_WM"])
                           .Md5(true) == req["LMI_HASH"];
        }
    }
}