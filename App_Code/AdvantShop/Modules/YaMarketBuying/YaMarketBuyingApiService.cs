//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using Newtonsoft.Json;

namespace AdvantShop.Modules
{
    public class YaMarketByuingApiService
    {
        public const string ApiUrl = "yamarket/api";

        private const string MarketApiUrl = "https://api.partner.market.yandex.ru/v2/campaigns/{0}/orders/{1}/status.{2}";


        #region Help methods

        private static bool CheckAuthorizationToken(string auth)
        {
            return auth.IsNotEmpty() && YaMarketBuyingSettings.AuthToken.IsNotEmpty() && YaMarketBuyingSettings.AuthToken == auth;
        }

        private static string GetByType(string type, YaMarketRegion region)
        {
            if (region == null)
                return string.Empty;

            if (region.type == type)
                return region.name;

            return GetByType(type, region.parent);
        }

        private static void WriteError(string error)
        {
            var context = HttpContext.Current;

            context.Response.Status = "400 Bad Request";
            context.Response.Write(error);
            context.Response.End();
        }

        private static void WriteUnauthorized(string error)
        {
            var context = HttpContext.Current;

            context.Response.Status = "401 Unauthorized";
            context.Response.Write(error);
            context.Response.End();
        }


        #endregion

        #region POST cart order/accept order/status

        public static bool RewritePath(string rawUrl, ref string newUrl)
        {
            if (string.IsNullOrEmpty(rawUrl) || !rawUrl.Contains(ApiUrl))
                return false;

            var url = rawUrl.Split("?").FirstOrDefault();

            var auth = HttpContext.Current.Request.Headers["Authorization"];
            if (!CheckAuthorizationToken(auth))
            {
                WriteUnauthorized("Invalid AuthToken");
            }

            var json = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();

            Debug.LogError("Yamarket start: " + url);

            var method = url.Replace(ApiUrl, "").Trim(new[] { '/' });
            if (method.Contains("cart"))
            {
                Cart(json);
            }
            else if (method.Contains("order/accept"))
            {
                Accept(json);
            }
            else if (method.Contains("order/status"))
            {
                Status(json);
            }

            return true;
        }

        // POST /cart
        protected static void Cart(string json)
        {
            var yaCart = JsonConvert.DeserializeObject<YaCart>(json);
            if (yaCart == null || yaCart.cart == null)
                return;

            var yaResponse = new YaMarketCartResponse();

            try
            {
                var shoppingCart = new ShoppingCart();

                foreach (var marketItem in yaCart.cart.items)
                {
                    var offer = OfferService.GetOffer(Convert.ToInt32(marketItem.offerId));
                    var isEnabled = offer != null && offer.Product.Enabled;

                    yaResponse.items.Add(new YaMarketItem(marketItem)
                    {
                        price = isEnabled ? offer.Price : 0,
                        count = isEnabled ? offer.Amount : 0,
                        delivery = isEnabled
                    });

                    if (isEnabled)
                    {
                        shoppingCart.Add(new ShoppingCartItem()
                        {
                            OfferId = offer.OfferId,
                            Amount = marketItem.count,
                            AttributesXml = string.Empty,
                            ShoppingCartType = ShoppingCartType.ShoppingCart,
                        });
                    }
                }

                var city = GetByType("CITY", yaCart.cart.delivery.region) ?? string.Empty;
                var region = GetByType("REGION", yaCart.cart.delivery.region) ??
                             (GetByType("SUBJECT_FEDERATION", yaCart.cart.delivery.region) ?? string.Empty);
                var country = GetByType("COUNTRY", yaCart.cart.delivery.region) ?? "Россия";
                var countryId = CountryService.GetCountryByName(country).CountryId;

                var shippingManager = new ShippingManager();
                var shippingRates = shippingManager.GetShippingRates(countryId, "", city, region, shoppingCart, 0);
                var yaMarketShippings = YaMarketByuingService.GetShippings();


                foreach (var shippingRate in shippingRates.OrderBy(x => x.Rate))
                {
                    var shipping = yaMarketShippings.FirstOrDefault(x => x.ShippingMethodId == shippingRate.MethodId);
                    if (shipping == null)
                        continue;

                    var delivery = new YaMarketDeliveryResponse()
                    {
                        id = shippingRate.Id.ToString(),
                        type = shipping.Type,
                        serviceName = shippingRate.MethodNameRate,
                        price = shippingRate.Rate,
                        dates = new YaMarketDate()
                        {
                            fromDate = DateTime.Now.AddDays(shipping.MinDate).ToString("dd-MM-yyyy"), // todo
                            toDate = DateTime.Now.AddDays(shipping.MaxDate).ToString("dd-MM-yyyy")
                        }
                    };

                    if (delivery.type == "PICKUP" && YaMarketBuyingSettings.Outlets.IsNotEmpty())
                    {
                        delivery.outlets = new List<YaMarketOutlet>();
                        foreach (var outlet in YaMarketBuyingSettings.Outlets.Split(';'))
                        {
                            delivery.outlets.Add(new YaMarketOutlet() { id = Convert.ToInt32(outlet) });
                        }
                    }

                    yaResponse.deliveryOptions.Add(delivery);
                }

                foreach (var payment in YaMarketBuyingSettings.Payments.Split(';'))
                {
                    yaResponse.paymentMethods.Add(payment);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            var context = HttpContext.Current;
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(new { cart = yaResponse }));
            context.Response.End();
        }


        // POST /order/accept
        protected static void Accept(string json)
        {
            var yaOrder = JsonConvert.DeserializeObject<YaMarketOrderRequest>(json);
            if (yaOrder == null || yaOrder.order == null)
                return;

            Order order = null;

            try
            {
                var adminComment = "";

                adminComment = "Заказ номер: " + yaOrder.order.id + (yaOrder.order.fake ? "(тестовый)" : "") + "\r\n";

                if (yaOrder.order.paymentType.IsNotEmpty())
                    adminComment += "Тип оплаты заказа: " +
                                    (yaOrder.order.paymentType == "PREPAID"
                                        ? "предоплата"
                                        : "постоплата при получении заказа") + "\r\n";

                if (yaOrder.order.paymentMethod.IsNotEmpty())
                {
                    adminComment += "Способ оплаты заказа: ";
                    switch (yaOrder.order.paymentMethod)
                    {
                        case "YANDEX":
                            adminComment += "оплата при оформлении";
                            break;
                        case "SHOP_PREPAID":
                            adminComment += "предоплата напрямую магазину (только для Украины)";
                            break;
                        case "CASH_ON_DELIVERY":
                            adminComment += "наличный расчет при получении заказа";
                            break;
                        case "CARD_ON_DELIVERY":
                            adminComment += "оплата банковской картой при получении заказа";
                            break;
                    }
                }

                adminComment += "\r\n";

                var orderContact = new OrderContact();
                var shippingCost = 0f;
                var shippingMethodName = "";

                if (yaOrder.order.delivery != null)
                {
                    adminComment += string.Format("Доставка: {0}, стоимость доставки: {1}, даты: {2} до {3}\r\n",
                        yaOrder.order.delivery.serviceName, yaOrder.order.delivery.price ?? 0,
                        yaOrder.order.delivery.dates.fromDate, yaOrder.order.delivery.dates.toDate);

                    orderContact = new OrderContact
                    {
                        Address =
                            yaOrder.order.delivery.address.street + " " + yaOrder.order.delivery.address.house + " " +
                            yaOrder.order.delivery.address.subway + " " + yaOrder.order.delivery.address.block + " " +
                            yaOrder.order.delivery.address.floor,
                        City = yaOrder.order.delivery.address.city,
                        Country = yaOrder.order.delivery.address.country,
                        Name = string.Empty,
                        Zip = yaOrder.order.delivery.address.postcode ?? string.Empty,
                        Zone = string.Empty
                    };

                    if (yaOrder.order.delivery.price != null)
                        shippingCost = (float)yaOrder.order.delivery.price;

                    shippingMethodName = yaOrder.order.delivery.serviceName;
                }

                var orderItems = (from item in yaOrder.order.items
                                  let offer = OfferService.GetOffer(Convert.ToInt32(item.offerId))
                                  where offer != null
                                  let product = offer.Product
                                  select new OrderItem()
                                  {
                                      Name = product.Name,
                                      Price = item.price,
                                      Amount = item.count,
                                      SupplyPrice = product.Offers[0].SupplyPrice,
                                      ProductID = product.ProductId,
                                      ArtNo = product.ArtNo,
                                      IsCouponApplied = false,
                                      Weight = product.Weight
                                  }).ToList();

                var orderCurrency = yaOrder.order.currency == "RUR"
                    ? (CurrencyService.GetAllCurrencies(true)
                        .FirstOrDefault(x => x.Iso3 == yaOrder.order.currency || x.Iso3 == "RUB") ??
                       CurrencyService.GetAllCurrencies(true).FirstOrDefault())
                    : (CurrencyService.GetAllCurrencies(true).FirstOrDefault(x => x.Iso3 == yaOrder.order.currency) ??
                       CurrencyService.GetAllCurrencies(true).FirstOrDefault());

                order = new Order()
                {
                    AdminOrderComment = adminComment,
                    CustomerComment = yaOrder.order.notes,
                    OrderCustomer = new OrderCustomer()
                    {
                        Email = "YaMarket@Order.ru",
                        CustomerIP = "127.0.0.1"
                    },
                    OrderItems = orderItems,
                    OrderCurrency = orderCurrency,
                    ShippingContact = orderContact,
                    BillingContact = orderContact,
                    ShippingCost = shippingCost,
                    ArchivedShippingName = shippingMethodName,
                    OrderStatusId = OrderService.DefaultOrderStatus,
                    OrderDate = DateTime.Now,
                    Number = OrderService.GenerateNumber(1),
                };

                order.OrderID = OrderService.AddOrder(order);
                order.Number = OrderService.GenerateNumber(order.OrderID);
                OrderService.UpdateNumber(order.OrderID, order.Number);
                OrderService.ChangeOrderStatus(order.OrderID, OrderService.DefaultOrderStatus);


                if (order.OrderID != 0)
                {
                    YaMarketByuingService.AddOrder(new YaOrder()
                    {
                        MarketOrderId = yaOrder.order.id.TryParseInt(),
                        OrderId = order.OrderID,
                        Status = string.Format("[{0}] Создан заказ {1}", DateTime.Now.ToString("g"), order.OrderID)
                    });

                    try
                    {
                        var orderTable = OrderService.GenerateHtmlOrderTable(order.OrderItems, order.OrderCurrency,
                            orderItems.Sum(x => x.Price * x.Amount), 0, null, null, 0, 0, 0, 0, 0, 0);

                        var mailTemplate = new BuyInOneClickMailTemplate(order.OrderID.ToString(), "", "", "", orderTable);
                        mailTemplate.BuildMail();

                        SendMail.SendMailNow(SettingsMail.EmailForOrders, "Заказ через Яндекс.Маркет", mailTemplate.Body,
                            true);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            /*
             * Если магазин считает запрос, поступающий от Яндекс.Маркета, некорректным, 
             * магазин должен вернуть статус ответа 400 с описанием причины ошибки в теле ответа. 
             * Такие ответы будут анализироваться на предмет нарушений и недоработок API со стороны Яндекс.Маркета.
             * 
             */
            var orderResponse = new YaMarketOrderResponse()
            {
                order = new YaMarketOrderAccept()
                {
                    accepted = order != null && order.OrderID != 0,
                    id = yaOrder.order.id
                }
            };

            var context = HttpContext.Current;
            context.Response.ContentType = "application/json";
            context.Response.Write(JsonConvert.SerializeObject(orderResponse));
            context.Response.End();
        }


        // POST order/status
        protected static void Status(string json)
        {
            try
            {
                var yaOrderStatus = JsonConvert.DeserializeObject<YaMarketOrderStatusRequest>(json);
                if (yaOrderStatus == null || yaOrderStatus.order == null)
                    return;

                var marketOrderId = yaOrderStatus.order.id.TryParseInt();

                var yaOrder = YaMarketByuingService.GetOrder(marketOrderId);
                if (yaOrder == null)
                {
                    WriteError("нет заказа с данным id");
                    return;
                }

                var order = OrderService.GetOrder(yaOrder.OrderId);
                if (order == null)
                {
                    WriteError("нет заказа с данным id");
                    return;
                }


                var status = string.Format("[{0}] Статус: ", DateTime.Now.ToString("g"));

                switch (yaOrderStatus.order.status)
                {
                    case "UNPAID":
                        status += "заказ оформлен, но еще не оплачен (если выбрана оплата при оформлении)";
                        if (YaMarketBuyingSettings.UpaidStatusId != 0)
                        {
                            OrderService.ChangeOrderStatus(order.OrderID, YaMarketBuyingSettings.UpaidStatusId);
                            order.OrderStatusId = YaMarketBuyingSettings.UpaidStatusId;
                        }
                        break;
                    case "PROCESSING":
                        status += "заказ можно выполнять";
                        if (YaMarketBuyingSettings.ProcessingStatusId != 0)
                        {
                            OrderService.ChangeOrderStatus(order.OrderID, YaMarketBuyingSettings.ProcessingStatusId);
                            order.OrderStatusId = YaMarketBuyingSettings.ProcessingStatusId;
                        }
                        break;
                    case "CANCELLED":
                        status += "заказ отменен: ";
                        OrderService.ChangeOrderStatus(order.OrderID, OrderService.CanceledOrderStatus);
                        order.OrderStatusId = OrderService.CanceledOrderStatus;

                        if (yaOrderStatus.order.substatus.IsNotEmpty())
                        {
                            switch (yaOrderStatus.order.substatus)
                            {
                                case "RESERVATION_EXPIRED":
                                    status += "покупатель не завершил оформление зарезервированного заказа вовремя";
                                    break;
                                case "USER_NOT_PAID":
                                    status += "покупатель не оплатил заказ (для типа оплаты PREPAID)";
                                    break;
                                case "USER_UNREACHABLE":
                                    status += "не удалось связаться с покупателем";
                                    break;
                                case "USER_CHANGED_MIND":
                                    status += "покупатель отменил заказ по собственным причинам";
                                    break;
                                case "USER_REFUSED_DELIVERY":
                                    status += "покупателя не устраивают условия доставки";
                                    break;
                                case "USER_REFUSED_PRODUCT":
                                    status += "покупателю не подошел товар";
                                    break;
                                case "SHOP_FAILED":
                                    status += "магазин не может выполнить заказ";
                                    break;
                                case "USER_REFUSED_QUALITY":
                                    status += "покупателя не устраивает качество товара";
                                    break;
                                case "REPLACING_ORDER":
                                    status += "покупатель изменяет состав заказа";
                                    break;
                                case "PROCESSING_EXPIRED":
                                    status += "магазин не обработал заказ вовремя";
                                    break;
                            }
                        }
                        break;
                }

                var html = new StringBuilder();

                var paymentMethod = "";
                switch (yaOrderStatus.order.paymentMethod)
                {
                    case "YANDEX":
                        paymentMethod = "оплата при оформлении";
                        break;
                    case "SHOP_PREPAID":
                        paymentMethod = "предоплата напрямую магазину (только для Украины)";
                        break;
                    case "CASH_ON_DELIVERY":
                        paymentMethod = "наличный расчет при получении заказа";
                        break;
                    case "CARD_ON_DELIVERY":
                        paymentMethod = "оплата банковской картой при получении заказа";
                        break;
                }

                html.AppendFormat("<div>Заказ #{0} изменил свой статус.</div>", yaOrder.OrderId);
                html.AppendFormat("<div>{0}</div>", status);
                html.AppendFormat("<div>Дата оформления заказа: {0}</div>", yaOrderStatus.order.creationDate);
                html.AppendFormat("<div>Валюта: {0}</div>", yaOrderStatus.order.currency == "RUR" ? "российский рубль" : "украинская гривна");
                html.AppendFormat("<div>Сумма заказа без учета доставки: {0}</div>", yaOrderStatus.order.itemsTotal);
                html.AppendFormat("<div>Сумма заказа с учетом доставки: {0}</div>", yaOrderStatus.order.total);
                html.AppendFormat("<div>Тип оплаты заказа: {0}</div>", yaOrderStatus.order.paymentType == "PREPAID" ? "предоплата" : "постоплата при получении заказа");
                html.AppendFormat("<div>Способ оплаты заказа: {0}</div>", paymentMethod);
                html.AppendFormat("<div>Тестовый: {0}</div>", yaOrderStatus.order.fake ? "Да" : "Нет");
                html.AppendFormat("<div>Комментарий к заказу: {0}</div>", yaOrderStatus.order.notes);

                if (yaOrderStatus.order.buyer != null)
                {
                    html.Append("<div>Пользователь</div>");
                    html.AppendFormat("<div>Идентификатор покупателя: {0}</div>", yaOrderStatus.order.buyer.id);
                    html.AppendFormat("<div>Имя покупателя: {0}</div>", yaOrderStatus.order.buyer.firstName);
                    html.AppendFormat("<div>Номер телефона: {0}</div>", yaOrderStatus.order.buyer.phone);
                    html.AppendFormat("<div>Email: {0}</div>", yaOrderStatus.order.buyer.email);
                    html.AppendFormat("<div>Фамилия Отчество: {0} {1}</div>", yaOrderStatus.order.buyer.lastName, yaOrderStatus.order.buyer.middleName);

                    order.OrderCustomer.FirstName = yaOrderStatus.order.buyer.firstName;
                    order.OrderCustomer.MobilePhone = yaOrderStatus.order.buyer.phone;
                    order.OrderCustomer.Email = yaOrderStatus.order.buyer.email;
                    order.OrderCustomer.LastName = (yaOrderStatus.order.buyer.lastName ?? string.Empty) +
                                                   (yaOrderStatus.order.buyer.middleName ?? string.Empty);

                    OrderService.UpdateOrderCustomer(order.OrderCustomer);
                }

                if (yaOrderStatus.order.delivery != null)
                {
                    html.Append("<div>Доставка</div>");
                    html.AppendFormat("<div>Метод: {0}</div>", yaOrderStatus.order.delivery.serviceName);
                    html.AppendFormat("<div>Стоимость: {0}</div>", yaOrderStatus.order.delivery.price);
                    html.AppendFormat("<div>Время: {0} до {1}</div>", yaOrderStatus.order.delivery.dates.fromDate, yaOrderStatus.order.delivery.dates.toDate);

                    if (yaOrderStatus.order.delivery.outletId != 0)
                        html.AppendFormat("<div>Id пункта самовывоза: {0}</div>", yaOrderStatus.order.delivery.outletId);

                    if (yaOrderStatus.order.delivery.address != null)
                    {
                        html.Append("<div>Адрес</div>");
                        html.AppendFormat("<div>Страна: {0}</div>", yaOrderStatus.order.delivery.address.country);
                        html.AppendFormat("<div>Город: {0}</div>", yaOrderStatus.order.delivery.address.city);
                        html.AppendFormat("<div>Номер дома: {0}</div>", yaOrderStatus.order.delivery.address.house);

                        if (yaOrderStatus.order.delivery.address.postcode.IsNotEmpty())
                            html.AppendFormat("<div>Почтовый индекс: {0}</div>", yaOrderStatus.order.delivery.address.postcode);

                        if (yaOrderStatus.order.delivery.address.street.IsNotEmpty())
                            html.AppendFormat("<div>Улица: {0}</div>", yaOrderStatus.order.delivery.address.street);

                        if (yaOrderStatus.order.delivery.address.subway.IsNotEmpty())
                            html.AppendFormat("<div>Станция метро: {0}</div>", yaOrderStatus.order.delivery.address.subway);

                        if (yaOrderStatus.order.delivery.address.block.IsNotEmpty())
                            html.AppendFormat("<div>Номер корпуса либо строения: {0}</div>", yaOrderStatus.order.delivery.address.block);

                        if (yaOrderStatus.order.delivery.address.entrance.IsNotEmpty())
                            html.AppendFormat("<div>Номер подъезда: {0}</div>", yaOrderStatus.order.delivery.address.entrance);

                        if (yaOrderStatus.order.delivery.address.entryphone.IsNotEmpty())
                            html.AppendFormat("<div>Код домофона: {0}</div>", yaOrderStatus.order.delivery.address.entryphone);

                        if (yaOrderStatus.order.delivery.address.floor.IsNotEmpty())
                            html.AppendFormat("<div>Этаж: {0}</div>", yaOrderStatus.order.delivery.address.floor);

                        if (yaOrderStatus.order.delivery.address.apartment.IsNotEmpty())
                            html.AppendFormat("<div>Номер квартиры либо офиса: {0}</div>", yaOrderStatus.order.delivery.address.apartment);

                        if (yaOrderStatus.order.delivery.address.recipient.IsNotEmpty())
                            html.AppendFormat("<div>ФИО получателя заказа: {0}</div>", yaOrderStatus.order.delivery.address.recipient);

                        if (yaOrderStatus.order.delivery.address.phone.IsNotEmpty())
                            html.AppendFormat("<div>Номер телефона получателя заказа: {0}</div>", yaOrderStatus.order.delivery.address.phone);
                    }
                }


                var result = html.ToString();

                order.AdminOrderComment = result.Replace("<div>", "").Replace("</div>", "\r\n");
                OrderService.UpdateOrderMain(order);

                YaMarketByuingService.UpdateOrder(new YaOrder()
                {
                    MarketOrderId = yaOrder.MarketOrderId,
                    OrderId = order.OrderID,
                    Status = yaOrder.Status + "\r\n------\r\n" + result.Replace("<div>", "").Replace("</div>", "\r\n")
                });

                SendMail.SendMailNow(SettingsMail.EmailForOrders, "Заказ через Яндекс.Маркет. Изменение статуса заказа", result, true);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                WriteError(ex.StackTrace);
                return;
            }


            var context = HttpContext.Current;
            context.Response.ContentType = "application/json";
            context.Response.Write("OK");
            context.Response.End();
        }

        #endregion

        #region Status

        private static bool MakeRequest(string api, string data)
        {
            try
            {
                var request = WebRequest.Create(api) as HttpWebRequest;
                request.Method = "PUT";
                request.ContentType = "application/json";
                request.Headers["Authorization"] =
                    string.Format("OAuth oauth_token=\"{0}\", oauth_client_id=\"{1}\"",
                        YaMarketBuyingSettings.AuthTokenToMarket, YaMarketBuyingSettings.AuthClientId);

                if (data.IsNotEmpty())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                }

                var responseContent = "";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = reader.ReadToEnd();
                            }
                    }
                }
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                {
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    Debug.LogError(error + " **** " + data);
                                }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
            return true;
        }

        public static void ChangeStatus(IOrderStatus status, IOrder order)
        {
            if (status.IsCanceled)
                ChangeMarketStatusToCanceled(order);

            if (status.StatusID == YaMarketBuyingSettings.DeliveryStatusId)
                ChangeMarketStatusToDelivery(order);

            if (status.StatusID == YaMarketBuyingSettings.DeliveredStatusId)
                ChangeMarketStatusToDelivered(order);
        }

        protected static void ChangeMarketStatusToCanceled(IOrder order)
        {
            var status = new YaStatus() { order = new YaStatusOrder() {status = "CANCELLED", substatus = "USER_UNREACHABLE" }};
            var orderId = YaMarketByuingService.GetMarketOrderId(order.OrderID);

            if (orderId == 0)
                return;

            MakeRequest(string.Format(MarketApiUrl, YaMarketBuyingSettings.CampaignId, orderId, "json"),
                JsonConvert.SerializeObject(status, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }

        protected static void ChangeMarketStatusToDelivery(IOrder order)
        {
            var status = new YaStatus() { order = new YaStatusOrder(){ status = "DELIVERY" }};
            var orderId = YaMarketByuingService.GetMarketOrderId(order.OrderID);

            if (orderId == 0)
                return;

            MakeRequest(string.Format(MarketApiUrl, YaMarketBuyingSettings.CampaignId, orderId, "json"),
                JsonConvert.SerializeObject(status, new JsonSerializerSettings(){NullValueHandling = NullValueHandling.Ignore}));
        }

        protected static void ChangeMarketStatusToDelivered(IOrder order)
        {
            var status = new YaStatus() {order = new YaStatusOrder(){ status = "DELIVERED" }};
            var orderId = YaMarketByuingService.GetMarketOrderId(order.OrderID);

            if (orderId == 0)
                return;

            MakeRequest(string.Format(MarketApiUrl, YaMarketBuyingSettings.CampaignId, orderId, "json"),
                JsonConvert.SerializeObject(status, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }

        #endregion
    }
}