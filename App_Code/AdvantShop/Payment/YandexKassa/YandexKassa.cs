//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.SaasData;

namespace AdvantShop.Payment
{
    public class YandexKassa : PaymentMethod
    {
        public string ShopId { get; set; }
        public string ScId { get; set; }
        public float CurrencyValue { get; set; }
        public bool DemoMode { get; set; }
        public string YaPaymentType { get; set; }
        public string Password { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.YandexKassa; }
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
            get { return UrlStatus.FailUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public override string NotificationUrl
        {
            get
            {
                return SaasDataService.IsSaasEnabled
                           ? "https://demo.advantshop.net/yandexkassa/" + StringHelper.EncodeTo64(base.NotificationUrl)
                           : base.NotificationUrl.Replace("http://", "https://");
            }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {YandexKassaTemplate.ShopID, ShopId},
                               {YandexKassaTemplate.ScID, ScId},
                               {YandexKassaTemplate.CurrencyValue, CurrencyValue.ToString(CultureInfo.InvariantCulture)},
                               {YandexKassaTemplate.DemoMode, DemoMode.ToString()},
                               {YandexKassaTemplate.YaPaymentType, YaPaymentType},
                               {YandexKassaTemplate.Password, Password}
                           };
            }
            set
            {
                ShopId = value.ElementOrDefault(YandexKassaTemplate.ShopID);
                ScId = value.ElementOrDefault(YandexKassaTemplate.ScID);
                YaPaymentType = value.ElementOrDefault(YandexKassaTemplate.YaPaymentType);
                Password = value.ElementOrDefault(YandexKassaTemplate.Password);
                DemoMode = value.ElementOrDefault(YandexKassaTemplate.DemoMode).TryParseBool();
                float decVal;
                CurrencyValue = value.ContainsKey(YandexKassaTemplate.CurrencyValue) &&
                                float.TryParse(value[YandexKassaTemplate.CurrencyValue], NumberStyles.Float, CultureInfo.InvariantCulture, out decVal)
                                    ? decVal
                                    : 1;
            }
        }

        public override void ProcessForm(Order order)
        {
            new PaymentFormHandler
            {
                Url = DemoMode ? "https://demomoney.yandex.ru/eshop.xml" : "https://money.yandex.ru/eshop.xml",
                InputValues =
                {
                    {"shopId", ShopId},
                    {"scid", ScId},
                    {"sum", (order.Sum/CurrencyValue).ToString("F2").Replace(",", ".")},
                    {"customerNumber", order.OrderCustomer.CustomerID.ToString().Normalize()},
                    {"orderNumber", order.OrderID.ToString(CultureInfo.InvariantCulture).Normalize()},
                    {"shopSuccessURL", HttpUtility.UrlEncode(SuccessUrl)},
                    {"shopFailURL", HttpUtility.UrlEncode(FailUrl)},
                    {"cps_email", order.OrderCustomer.Email ?? string.Empty},
                    {"paymentType", YaPaymentType},
                    {
                        "cps_phone",
                        order.OrderCustomer.MobilePhone.IsNotEmpty() &&
                        order.OrderCustomer.MobilePhone.All(char.IsDigit) &&
                        order.OrderCustomer.MobilePhone.Length <= 15
                            ? order.OrderCustomer.MobilePhone
                            : string.Empty
                    },
                    {"cms_name", "AdVantShop.NET"}
                }
            }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            return new PaymentFormHandler
            {
                Url = DemoMode ? "https://demomoney.yandex.ru/eshop.xml" : "https://money.yandex.ru/eshop.xml",
                Page = page,
                Method = FormMethod.POST,
                InputValues =
                {
                    {"shopId", ShopId},
                    {"scid", ScId},
                    {"sum", (order.Sum/CurrencyValue).ToString("F2").Replace(",", ".")},
                    {"customerNumber", order.OrderCustomer.CustomerID.ToString().Normalize()},
                    {"orderNumber", order.OrderID.ToString(CultureInfo.InvariantCulture).Normalize()},
                    {"shopSuccessURL", HttpUtility.UrlEncode(SuccessUrl)},
                    {"shopFailURL", HttpUtility.UrlEncode(FailUrl)},
                    {"cps_email", order.OrderCustomer.Email ?? string.Empty},
                    {"paymentType", YaPaymentType},
                    {
                        "cps_phone",
                        order.OrderCustomer.MobilePhone.IsNotEmpty() &&
                        order.OrderCustomer.MobilePhone.All(char.IsDigit) &&
                        order.OrderCustomer.MobilePhone.Length <= 15
                            ? order.OrderCustomer.MobilePhone
                            : string.Empty
                    },
                    {"cms_name", "AdVantShop.NET"}
                }
            }.ProcessRequest();
        }

        public override string ProcessResponse(HttpContext context)
        {
            var typeRequest = TypeRequestYandex.checkOrder;
            var processingResult = ProcessingResult.ErrorParsing;
            var invoiceId = string.Empty;

            try
            {
                ProcessingMd5(context, ref processingResult, ref typeRequest, ref invoiceId);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                processingResult = ProcessingResult.Exception;
            }

            var result = RendAnswer(typeRequest, processingResult, invoiceId);
            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/xml";
            context.Response.Write(result);
            context.Response.End();
            return result;
        }

        private string RendAnswer(TypeRequestYandex typeRequest, ProcessingResult processingResult, string invoiceId)
        {
            return string.Format(
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?><{0}Response performedDatetime=\"{1}\" code=\"{2}\" invoiceId=\"{3}\" shopId=\"{4}\"/>",
                typeRequest, DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fzzz"), (int)processingResult, invoiceId, ShopId);
        }

        private bool IsCheckFields(Dictionary<string, string> parameters, TypeRequestYandex typeRequest)
        {
            decimal orderSumAmount;

            if (parameters["shopId"].Equals(ShopId, StringComparison.InvariantCultureIgnoreCase) &&
                parameters["invoiceId"].IsNotEmpty() && parameters["invoiceId"].All(char.IsDigit) &&
                parameters["orderNumber"].IsNotEmpty() && parameters["orderNumber"].All(char.IsDigit) &&
                parameters["orderSumAmount"].IsNotEmpty() &&
                decimal.TryParse(parameters["orderSumAmount"], NumberStyles.Float, CultureInfo.InvariantCulture, out orderSumAmount))
            {
                var ord = OrderService.GetOrder(parameters["orderNumber"].TryParseInt());

                if (ord != null &&
                    // Если это запрос "Уведомление о переводе", которые могут повторяться несколько раз (упомянуто в документации),
                    // тогда неважно заказ был уже отмечен оплаченным или уже отменен
                    (typeRequest == TypeRequestYandex.paymentAviso || (!ord.Payed && ord.OrderStatusId != OrderService.CanceledOrderStatus)) &&
                    ord.OrderCustomer.CustomerID.ToString().Normalize().Equals(parameters["customerNumber"], StringComparison.InvariantCultureIgnoreCase) &&
                    orderSumAmount >= Math.Round((decimal)(ord.Sum / CurrencyValue), 2))
                {
                    return true;
                }
            }
            return false;
        }

        #region NVP/MD5

        private void ProcessingMd5(HttpContext context, ref ProcessingResult processingResult,
            ref TypeRequestYandex typeRequest, ref string invoiceId)
        {
            var parameters = ReadParametersMd5(context, ref typeRequest);

            invoiceId = parameters.ContainsKey("invoiceId") ? parameters["invoiceId"] : string.Empty;

            if (IsCheckMd5(parameters))
            {
                if (IsCheckFields(parameters, typeRequest))
                {
                    if (typeRequest == TypeRequestYandex.paymentAviso)
                        OrderService.PayOrder(parameters["orderNumber"].TryParseInt(), true);

                    processingResult = ProcessingResult.Success;
                }
                else
                {
                    processingResult = typeRequest == TypeRequestYandex.checkOrder
                        ? ProcessingResult.TranslationFailure
                        : ProcessingResult.ErrorParsing;

                }
            }
            else
                processingResult = ProcessingResult.ErrorAuthorize;
        }

        private Dictionary<string, string> ReadParametersMd5(HttpContext context, ref TypeRequestYandex typeRequest)
        {
            if (context.Request["action"].IsNotEmpty())
            {
                if (context.Request["action"].Equals("checkOrder", StringComparison.InvariantCultureIgnoreCase))
                    typeRequest = TypeRequestYandex.checkOrder;
                else if (context.Request["action"].Equals("paymentAviso", StringComparison.InvariantCultureIgnoreCase))
                    typeRequest = TypeRequestYandex.paymentAviso;
            }
            return context.Request.Params.AllKeys.ToDictionary(key => key, key => context.Request[key]);
        }

        private bool IsCheckMd5(Dictionary<string, string> parameters)
        {
            return parameters["md5"].ToLower() ==
                   string.Format("{0};{1};{2};{3};{4};{5};{6};{7}",
                       parameters["action"],
                       parameters["orderSumAmount"],
                       parameters["orderSumCurrencyPaycash"],
                       parameters["orderSumBankPaycash"],
                       parameters["shopId"],
                       parameters["invoiceId"],
                       parameters["customerNumber"],
                       Password).Md5(false);
        }

        #endregion

        private enum TypeRequestYandex
        {

            //Do not change the register
            checkOrder,
            paymentAviso
        }

        private enum ProcessingResult : int
        {
            Success = 0,
            ErrorAuthorize = 1,
            TranslationFailure = 100,
            ErrorParsing = 200,
            Exception = 1000
        }
    }
}