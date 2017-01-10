//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{

    public class WalletOneCheckout : PaymentMethod
    {
        public override PaymentType Type
        {
            get { return PaymentType.WalletOneCheckout; }
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
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public string MerchantId { get; set; }
        public string SecretKey { get; set; }
        public string ExpiredDate { get; set; }


        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {WalletOneCheckoutTemplate.MerchantId, MerchantId},
                               {WalletOneCheckoutTemplate.SecretKey, SecretKey}
                           };
            }
            set
            {
                if (value.ContainsKey(WalletOneCheckoutTemplate.MerchantId))
                    MerchantId = value[WalletOneCheckoutTemplate.MerchantId];
                if (value.ContainsKey(WalletOneCheckoutTemplate.SecretKey))
                    SecretKey = value[WalletOneCheckoutTemplate.SecretKey];
            }
        }

        public override void ProcessForm(Order order)
        {
            string sum = Math.Round(order.Sum * order.OrderCurrency.CurrencyValue, 2).ToString().Replace(",", ".");

            var merchantParams = new Dictionary<string, string>
            {
                {"WMI_CULTURE_ID", "ru-RU"},
                {"WMI_CURRENCY_ID", order.OrderCurrency.CurrencyNumCode.ToString()},
                {"WMI_DESCRIPTION", GetOrderDescription(order.Number)},
                {"WMI_FAIL_URL", FailUrl},
                {"WMI_MERCHANT_ID", MerchantId},
                {"WMI_PAYMENT_AMOUNT", sum},
                {"WMI_PAYMENT_NO", order.OrderID.ToString()},
                {"WMI_RECIPIENT_LOGIN", order.OrderCustomer.Email},
                {"WMI_SUCCESS_URL", SuccessUrl},
            };

            var signatureData = new StringBuilder();
            foreach (string key in merchantParams.Keys)
            {
                signatureData.Append(merchantParams[key]);
            }

            // Формирование значения параметра WMI_SIGNATURE
            Byte[] bytes = Encoding.GetEncoding(1251).GetBytes(signatureData + SecretKey);
            Byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string signature = Convert.ToBase64String(hash);
            merchantParams.Add("WMI_SIGNATURE", signature);

            new PaymentFormHandler
            {
                FormName = "_xclick",
                Method = FormMethod.GET,
                Url = "https://merchant.w1.ru/checkout/default.aspx",
                InputValues = merchantParams
            }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {

            string sum = Math.Round(order.Sum * order.OrderCurrency.CurrencyValue, 2).ToString().Replace(",", ".");

            var merchantParams = new Dictionary<string, string>
            {
                {"WMI_CULTURE_ID", "ru-RU"},
                {"WMI_CURRENCY_ID", order.OrderCurrency.CurrencyNumCode.ToString()},
                {"WMI_DESCRIPTION", GetOrderDescription(order.Number)},
                {"WMI_FAIL_URL", FailUrl},
                {"WMI_MERCHANT_ID", MerchantId},
                {"WMI_PAYMENT_AMOUNT", sum},
                {"WMI_PAYMENT_NO", order.OrderID.ToString()},
                {"WMI_RECIPIENT_LOGIN", order.OrderCustomer.Email},
                {"WMI_SUCCESS_URL", SuccessUrl},
            };

            var signatureData = new StringBuilder();
            foreach (string key in merchantParams.Keys)
            {
                signatureData.Append(merchantParams[key]);
            }

            // Формирование значения параметра WMI_SIGNATURE
            Byte[] bytes = Encoding.GetEncoding(1251).GetBytes(signatureData + SecretKey);
            Byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string signature = Convert.ToBase64String(hash);
            merchantParams.Add("WMI_SIGNATURE", signature);

            return new PaymentFormHandler
            {
                FormName = "_xclick",
                Method = FormMethod.GET,
                Url = "https://merchant.w1.ru/checkout/default.aspx",
                InputValues = merchantParams
            }.ProcessRequest();
        }

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;
            if (ValidateRequest(req))
            {
                int orderID = 0;
                if (int.TryParse(req["WMI_PAYMENT_NO"], out orderID))
                {
                    if (req["WMI_ORDER_STATE"].ToLower() == "ACCEPTED".ToLower())
                    {
                        Order order = OrderService.GetOrder(orderID);
                        if (order != null)
                        {
                            OrderService.PayOrder(orderID, true);
                            return "WMI_RESULT=OK&WMI_DESCRIPTION=Order successfully processed";
                        }
                    }
                }
                return "WMI_RESULT=RETRY&WMI_DESCRIPTION=Invalid Order ID";
            }
            return "WMI_RESULT=RETRY&WMI_DESCRIPTION=Invalid WMI_SIGNATURE";
        }

        private bool ValidateRequest(HttpRequest req)
        {

            var merchantParams = new Dictionary<string, string>();

            foreach (string key in req.Form.AllKeys)
            {
                if (key.ToLower() != "WMI_SIGNATURE".ToLower() && key.ToLower() != "PaymentMethodID".ToLower())
                {
                    merchantParams.Add(key, req.Form[key]);
                }
            }

            var signatureData = new StringBuilder();
            foreach (string key in merchantParams.Keys.OrderBy(s => s))
            {
                signatureData.Append(merchantParams[key]);
            }

            // Формирование значения параметра WMI_SIGNATURE
            Byte[] bytes = Encoding.GetEncoding(1251).GetBytes(signatureData + SecretKey);
            Byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);
            string signature = Convert.ToBase64String(hash);

            return signature == req["WMI_SIGNATURE"];
        }
    }
}