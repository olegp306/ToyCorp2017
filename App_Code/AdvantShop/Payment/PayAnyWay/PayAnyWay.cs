//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    public class PayAnyWay : PaymentMethod
    {
        public string MerchantId { get; set; }
        public string CurrencyLabel { get; set; }
        public float CurrencyValue { get; set; }
        public string Signature { get; set; }
        public bool TestMode { get; set; }
        public string UnitId { get; set; }
        public string LimitIds { get; set; }


        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.NotificationUrl | UrlStatus.ReturnUrl | UrlStatus.FailUrl; }
        }
        public override PaymentType Type
        {
            get { return PaymentType.PayAnyWay; }
        }
        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {PayAnyWayTemplate.MerchantId, MerchantId},
                               {PayAnyWayTemplate.CurrencyLabel, CurrencyLabel},
                               {PayAnyWayTemplate.CurrencyValue, CurrencyValue.ToString()},
                               {PayAnyWayTemplate.Signature, Signature},
                               {PayAnyWayTemplate.TestMode, TestMode.ToString()},
                               {PayAnyWayTemplate.UnitId, UnitId},
                               {PayAnyWayTemplate.LimitIds, LimitIds},
                           };
            }
            set
            {
                if (value.ContainsKey(PayAnyWayTemplate.MerchantId))
                    MerchantId = value[PayAnyWayTemplate.MerchantId];
                Signature = value.ElementOrDefault(PayAnyWayTemplate.Signature);
                CurrencyLabel = !value.ContainsKey(PayAnyWayTemplate.CurrencyLabel)
                                    ? "RUB"
                                    : value[PayAnyWayTemplate.CurrencyLabel];
                float decVal = 0;
                CurrencyValue = value.ContainsKey(PayAnyWayTemplate.CurrencyValue) &&
                                float.TryParse(value[PayAnyWayTemplate.CurrencyValue], out decVal)
                                    ? decVal
                                    : 1;
                bool boolVal;
                TestMode = !bool.TryParse(value.ElementOrDefault(PayAnyWayTemplate.TestMode), out boolVal) || boolVal;

                UnitId = value.ElementOrDefault(PayAnyWayTemplate.UnitId);
                LimitIds = value.ElementOrDefault(PayAnyWayTemplate.LimitIds);
            }
        }

        private PaymentFormHandler RendPaymentFormHandler(Order order)
        {
            var sum = (order.Sum * CurrencyValue).ToString("F2").Replace(",", ".");

            var payment = new PaymentFormHandler
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = "https://www.moneta.ru/assistant.htm",
                InputValues = new Dictionary<string, string>
                {
                    {"MNT_ID", MerchantId},
                    {"MNT_TRANSACTION_ID", order.OrderID.ToString()},
                    {"MNT_CURRENCY_CODE", CurrencyLabel},
                    {"MNT_AMOUNT", sum},
                    {"MNT_TEST_MODE", TestMode ? "1" : "0"},
                    {"MNT_DESCRIPTION", GetOrderDescription(order.Number)},
                    {
                        "MNT_SIGNATURE",
                        string.Format("{0}{1}{2}{3}{4}{5}", MerchantId, order.OrderID.ToString(), sum, CurrencyLabel,
                            TestMode ? "1" : "0", Signature).Md5()
                    },
                    {"MNT_SUCCESS_URL", SuccessUrl},
                    {"MNT_FAIL_URL", FailUrl},
                }
            };

            if (!string.IsNullOrEmpty(UnitId))
            {
                payment.InputValues.Add("paymentSystem.unitId", UnitId);
            }

            if (!string.IsNullOrEmpty(LimitIds))
            {
                payment.InputValues.Add("paymentSystem.limitIds", LimitIds);
            }

            return payment;
        }

        public override void ProcessForm(Order order)
        {
            RendPaymentFormHandler(order).Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            return RendPaymentFormHandler(order).ProcessRequest();
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;

            //if (!CheckData(req))
            //    return "FAIL";

            int orderId;
            if (int.TryParse(req["MNT_TRANSACTION_ID"], out orderId))
            {
                var order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    string sum = (order.Sum * CurrencyValue).ToString("F2").Replace(",", ".");
                    //if (req["MNT_AMOUNT"] == sum && !order.Payed)
                    if (!order.Payed)
                        OrderService.PayOrder(orderId, true);

                    return "SUCCESS";
                }
            }
            return "FAIL";
        }

        public bool CheckData(HttpRequest req)
        {
            var fields = new string[]
                             {
                                 "MNT_ID",
                                 "MNT_TRANSACTION_ID",
                                 "MNT_OPERATION_ID",
                                 "MNT_AMOUNT",
                                 "MNT_CURRENCY_CODE",
                                 "MNT_TEST_MODE",
                                 "MNT_SIGNATURE"
                             };

            return (!fields.Any(val => string.IsNullOrEmpty(req[val]))
                && fields.Aggregate<string, StringBuilder, string>(new StringBuilder(), (str, field) => str.Append(field == "MNT_ID" ? MerchantId : field == "MNT_CURRENCY_CODE" ? CurrencyLabel : field == "MNT_SIGNATURE" ? Signature : req[field]), Strings.ToString).Md5(true) != req["MNT_SIGNATURE"]);
        }

    }
}