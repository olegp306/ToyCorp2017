//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using AdvantShop.Orders;
using AdvantShop.Payment.AlfabankSoap;

namespace AdvantShop.Payment
{
    public class Alfabank : PaymentMethod
    {
        public int ShopId { get; set; }
        
        public string Login { get; set; }
        
        public string Password { get; set; }

        public bool IsSandBox { get; set; }
        
        public override PaymentType Type
        {
            get { return PaymentType.Alfabank; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.ServerRequest; }
        }
        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
        }
        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.NotificationUrl; }
        }
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                                {
                                   {"alfabank_shopid", ShopId.ToString(CultureInfo.InvariantCulture)},
                                   {"alfabank_login", Login},
                                   {"alfabank_password", Password},
                                   {"alfabank_issandbox", IsSandBox.ToString()}
                               };
            }
            set
            {
                ShopId = value.ElementOrDefault("alfabank_shopid").TryParseInt();
                Login = value.ElementOrDefault("alfabank_login");
                Password = value.ElementOrDefault("alfabank_password");
                IsSandBox = value.ElementOrDefault("alfabank_issandbox").TryParseBool();
            }
        }

        public Alfabank()
        {
            // Чтобы по умолчанию был отладочный режим.
            IsSandBox = true;
        }

        public override string ProcessServerRequest(Order order)
        {
            using (var svc = new AlfabankService2(Login, Password, IsSandBox))
            {
                var orderCustomer = order.GetOrderCustomer();

                var result = svc.register_online(
                    new register_online
                    {
                        order = new OrderID
                        {
                            shop_id = ShopId, // Идентификатор магазина в платёжной системе, берем из настроек способа оплаты
                            number = order.Number // Номер заказа
                        },

                        cost = new Amount
                        {
                            amount = order.Sum, // Сумма для списания
                            currency = "RUB" // Валюта для списания
                        },

                        customer = new CustomerInfo
                        {
                            id = orderCustomer.CustomerID.ToString("N"), // Идентификатор заказчика в магазине
                            phone = orderCustomer.MobilePhone, // Телефон заказчика
                            name = string.Format("{0} {1}", orderCustomer.FirstName, orderCustomer.LastName), // ФИО заказчика
                            email = orderCustomer.Email // email
                        },

                        description = new OrderInfo
                        {
                            shopref = string.Empty, // внутренний параметр ИМ, который возвращается в одноименном параметре ответа на запросы
                            paytype = "card", // код типа способа оплаты (card)
                            descr = string.Empty // XML c описанием заказа
                        },

                        postdata = new[]
                        {
                            new PostEntry {name = "ReturnURLOk", value = SuccessUrl}, // адрес возврата, на который будет перенаправлен браузер пользователя в случае успешной оплаты
                            new PostEntry {name = "ReturnURLFault", value = FailUrl} // адрес возврата, на который будет перенаправлен браузер пользователя в случае несостоявшейся оплаты
                        }
                    }).retval;

                return string.Format("{0}?session={1}", result.redirect_url, result.session);
            }
        }
    }
}