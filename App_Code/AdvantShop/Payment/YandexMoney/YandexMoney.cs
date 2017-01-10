//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    public class YandexMoney : PaymentMethod
    {
        public string ShopId { get; set; }
        public string ScId { get; set; }
        public float CurrencyValue { get; set; }
        public string YaPaymentType { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.YandexMoney; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {YandexMoneyTemplate.ShopId, ShopId},
                    {YandexMoneyTemplate.ScId, ScId},
                    {YandexMoneyTemplate.CurrencyValue, CurrencyValue.ToString()},
                    {YandexMoneyTemplate.YaPaymentType, YaPaymentType}
                };
            }
            set
            {
                ShopId = value.ElementOrDefault(YandexMoneyTemplate.ShopId);
                ScId = value.ElementOrDefault(YandexMoneyTemplate.ScId);
                YaPaymentType = value.ElementOrDefault(YandexMoneyTemplate.YaPaymentType);

                float decVal;
                CurrencyValue = value.ContainsKey(YandexMoneyTemplate.CurrencyValue) &&
                                float.TryParse(value[YandexMoneyTemplate.CurrencyValue], out decVal)
                    ? decVal
                    : 1;
            }
        }

        public override void ProcessForm(Order order)
        {
            new PaymentFormHandler
            {
                Url = "http://money.yandex.ru/eshop.xml",
                InputValues =
                {
                    {"scid", ScId},
                    {"shopId", ShopId},
                    {"CustName", order.OrderCustomer.FirstName + order.OrderCustomer.LastName},
                    {"CustEMail", order.OrderCustomer.Email},
                    {"CustAddr", order.ShippingContact.Address},
                    {"OrderDetails", string.Join(",", order.OrderItems.Select(item => item.Name + " - " + item.Amount))},
                    {"Sum", (order.Sum/CurrencyValue).ToString("F2").Replace(",", ".")},
                    {"paymentType", YaPaymentType}
                }
            }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            return new PaymentFormHandler
            {
                Url = "http://money.yandex.ru/eshop.xml",
                Page = page,
                InputValues =
                {
                    {"scid", ScId},
                    {"shopId", ShopId},
                    {"CustName", order.OrderCustomer.FirstName + order.OrderCustomer.LastName},
                    {"CustEMail", order.OrderCustomer.Email},
                    {"CustAddr", order.ShippingContact.Address},
                    {"OrderDetails", string.Join(",", order.OrderItems.Select(item => item.Name + " - " + item.Amount))},
                    {"Sum", (order.Sum/CurrencyValue).ToString("F2").Replace(",", ".")},
                    {"paymentType", YaPaymentType}
                }
            }.ProcessRequest();
        }
    }
}