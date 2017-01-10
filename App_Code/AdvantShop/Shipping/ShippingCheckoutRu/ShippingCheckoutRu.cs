using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Linq;
using AdvantShop.Payment;
using AdvantShop.Shipping.CheckoutRu;
using AdvantShop.Orders;


namespace AdvantShop.Shipping
{
    public class ShippingCheckoutRu : IShippingMethod
    {
        private readonly Dictionary<DeliveryType, string> _deliveryTypeName = new Dictionary<DeliveryType, string>
        {
            {DeliveryType.Postamat, "Постоматы"},
            {DeliveryType.Pvz, "Пункты выдачи"},
            {DeliveryType.Express, "Курьерская доставка"},
            {DeliveryType.Mail, "Доставка почтой"}
        };


        private readonly Dictionary<int, string> _deliveryCompanies = new Dictionary<int, string>{
            {1, "DPD"},
            {2, "PickPoint"},
            {3, "Собственная доставка магазина"},
            {4, "Hermes"},
            {5, "СПСР-Экспресс"},
            {6, "DPD"},
            {7, "DPD"},
            {8, "ShopLogistics"},
            {9, "Boxberry"},
            {10, "DPD"},
            {11, "Почта России"}
          };

        private const string Url = "http://platform.checkout.ru/service/";
        protected string ClientId { get; set; }
        protected bool Grouping { get; set; }
        public float Extracharge { get; set; }
        public ExtrachargeType ExtrachargeType { get; set; }


        public ShoppingCart ShoppingCart { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public string CountryTo { get; set; }
        public string CityTo { get; set; }
        public string ZipTo { get; set; }
        public string StateTo { get; set; }

        public ShippingCheckoutRu(Dictionary<string, string> parameters)
        {
            ClientId = parameters.ElementOrDefault(ShippingCheckoutRuTemplate.ClientId);
            Grouping = Convert.ToBoolean(parameters.ElementOrDefault(ShippingCheckoutRuTemplate.Grouping));
            ExtrachargeType = (ExtrachargeType)parameters.ElementOrDefault(ShippingCheckoutRuTemplate.ExtrachargeType).TryParseInt();
            Extracharge = parameters.ElementOrDefault(ShippingCheckoutRuTemplate.Extracharge).TryParseFloat();
        }

        public List<ShippingOption> GetShippingOptions()
        {
            var listShippingOptions = new List<ShippingOption>();
            var ticket = GetTicket();
            var placeId = GetPlaceId(ticket, CityTo);

            var costs = GetCosts(ticket, placeId);

            if (Grouping)
            {
                foreach (var deliveryTypeName in _deliveryTypeName)
                {
                    listShippingOptions.AddRange(
                        GetShippingOptionsHelper(costs.GetDeliveries(deliveryTypeName.Key), deliveryTypeName.Key, deliveryTypeName.Value));
                }
            }
            else
            {
                foreach (var deliveryTypeName in _deliveryTypeName)
                {
                    foreach (var deliveryCompany in _deliveryCompanies)
                    {
                        listShippingOptions.AddRange(
                            GetShippingOptionsHelper(costs.GetDeliveries(deliveryTypeName.Key, deliveryCompany.Key), deliveryTypeName.Key, string.Format("{0} - {1}", deliveryCompany.Value, deliveryTypeName.Value)));
                    }
                }
            }

            return listShippingOptions;
        }

        private List<ShippingOption> GetShippingOptionsHelper(List<CheckoutDeliveryModel> deliveryCost, DeliveryType deliveryType, string deliveryName)
        {
            var listShippingOptions = new List<ShippingOption>();

            if (!deliveryCost.Any())
            {
                return new List<ShippingOption>();
            }

            if (deliveryType == DeliveryType.Postamat || deliveryType == DeliveryType.Pvz)
            {
                var shippingPoints = new List<ShippingPoint>();
                int index = 1;
                foreach (var delivery in deliveryCost)
                {
                    shippingPoints.Add(
                        new ShippingPoint
                        {
                            Id = index,
                            Code = delivery.Code,
                            Address = delivery.Address,
                            Description = delivery.AdditionalInfo,
                            AdditionalData = string.Format("{0};{1};{2};{3}",
                                delivery.DeliveryId,
                                delivery.CheckoutDeliveryType.ToString(),
                                delivery.MinDeliveryTerm,
                                delivery.MaxDeliveryTerm),
                            Rate = ProcessRate(delivery.Cost)
                        });

                    index++;
                }

                listShippingOptions.Add(
                    new ShippingOption
                    {
                        Name = deliveryName,
                        Rate = shippingPoints[0].Rate,
                        //DeliveryTime = deliveryCost.minDeliveryTerm,
                        ShippingPoints = shippingPoints,
                        Extend = new ShippingOptionEx
                        {
                            AdditionalData = deliveryType.ToString(),
                            //епик говеная заглушка, сделать норм объекты для компаний доставки
                            Type = string.Equals(deliveryName, "Boxberry") ? ExtendedType.Boxberry : ExtendedType.Pickpoint
                        }
                    });
            }
            else if (deliveryType == DeliveryType.Express || deliveryType == DeliveryType.Mail)
            {
                listShippingOptions.Add(
                    new ShippingOption
                    {
                        Name = deliveryName,
                        Rate = ProcessRate(deliveryCost[0].Cost),
                        DeliveryTime = deliveryCost[0].MinDeliveryTerm.ToString(),
                        ShippingPoints = null,
                        Extend = new ShippingOptionEx
                        {
                            AdditionalData = string.Format("{0};{1};{2};{3}",
                              deliveryCost[0].DeliveryId,
                              deliveryCost[0].CheckoutDeliveryType.ToString(),
                              deliveryCost[0].MinDeliveryTerm,
                              deliveryCost[0].MaxDeliveryTerm),
                            Type = ExtendedType.CashOnDelivery
                        }
                    });

            }

            return listShippingOptions;


        }

        public float GetRate()
        {
            //throw new NotImplementedException();
            return 300;
        }

        public CheckoutResponse CreateOrder(Order order)
        {
            if (order == null)
            {
                return new CheckoutResponse { error = true, errorMessage = "Не найден заказ" };
            }

            var additionalData = order.OrderPickPoint.AdditionalData.Split(new[] { ';' });

            var ticket = GetTicket();

            var placeId = GetPlaceId(ticket, order.ShippingContact.City);
            var streetId = GetStreetFias(ticket, placeId, order.ShippingContact.Address);

            if (string.IsNullOrEmpty(streetId) && (string.Equals(additionalData[1], DeliveryType.Express.ToString()) || string.Equals(additionalData[1], DeliveryType.Mail.ToString())))
            {
                return new CheckoutResponse { error = true, errorMessage = "Неверный адрес, не удается получить код улицы" };
            }

            var zip = 0;
            if (!Int32.TryParse(order.ShippingContact.Zip, out zip) && (string.Equals(additionalData[1], DeliveryType.Express.ToString()) || string.Equals(additionalData[1], DeliveryType.Mail.ToString())))
            {
                return new CheckoutResponse { error = true, errorMessage = "Неверный почтовый индекс" };
            }

            var createorder = new
            {
                apiKey = ClientId,
                order = new
                    {
                        goods = order.OrderItems.Select(orderItem => new
                        {
                            payCost = orderItem.Price.ToString("F2").Replace(",", "."),
                            assessedCost = orderItem.Price.ToString("F2").Replace(",", "."),
                            code = orderItem.ArtNo,
                            name = orderItem.Name,
                            quantity = orderItem.Amount < 1 ? 1 : Convert.ToInt32(orderItem.Amount),
                            variantCode = string.Empty,
                            weight = orderItem.Weight.ToString().Replace(",", ".")
                        }).ToList(),
                        user = new
                        {
                            email = order.OrderCustomer.Email,
                            fullname = order.OrderCustomer.LastName + " " + order.OrderCustomer.FirstName,
                            phone = order.OrderCustomer.MobilePhone
                        },
                        comment = order.CustomerComment,
                        shopOrderId = order.OrderID,
                        paymentMethod = order.PaymentMethod.Type == PaymentType.Cash ? "cash" : "prepay",
                        delivery = new
                        {
                            deliveryId = additionalData[0], //order.OrderPickPoint.PickPointId,
                            placeFiasId = placeId,
                            courierOptions = new List<string> { "none" },
                            addressExpress = additionalData[1] == DeliveryType.Express.ToString() || additionalData[1] == DeliveryType.Mail.ToString() ? new { postindex = order.ShippingContact.Zip, streetFiasId = streetId, house = order.ShippingContact.Address } : null,
                            addressPvz = additionalData[1] != DeliveryType.Express.ToString() && additionalData[1] != DeliveryType.Mail.ToString() ? order.OrderPickPoint.PickPointAddress : null,
                            cost = Math.Round(order.ShippingCost, 2).ToString("F2").Replace(",", "."),
                            type = additionalData[1].ToLower(),
                            minTerm = additionalData[2],
                            maxTerm = additionalData[3]
                        }
                    }
            };
            return PostRequestGetObject<CheckoutResponse>(
                 "order/create",
                 Newtonsoft.Json.JsonConvert.SerializeObject(createorder),
                 "POST");
        }

        private string GetTicket()
        {
            var response = PostRequestGetObject<CheckoutTicket>("login/ticket/" + ClientId);
            if (response != null)
            {
                return response.ticket;
            }
            return string.Empty;
        }

        private string GetPlaceId(string ticket, string city)
        {
            var response = PostRequestGetObject<CheckoutCitySuggestions>(string.Format(
                "checkout/getPlacesByQuery?ticket={0}&place={1}", ticket, city));

            if (response.suggestions.Count > 0)
            {
                return response.suggestions[0].id;
            }

            return string.Empty;
        }

        protected string GetStreetFias(string ticket, string placeId, string streetname)
        {
            var response = PostRequestGetObject<CheckoutStreetSuggestions>(string.Format("checkout/getStreetsByQuery?ticket={0}&placeId={1}&street={2}", ticket, placeId, streetname.Split(new[] { ',', ' ' })[0]));

            if (response.suggestions.Count > 0)
            {
                return response.suggestions[0].id;
            }

            return string.Empty;
        }

        protected CheckoutCost GetCosts(string ticket, string placeId)
        {
            if (OrderItems == null)
            {
                var str =
                    string.Format(
                        "checkout/calculation?ticket={0}&placeId={1}&totalSum={2}&assessedSum={3}&totalWeight={4}&itemsCount={5}",
                        ticket,
                        placeId,
                        ShoppingCart.TotalPrice.ToString("F2").Replace(",", "."),
                        ShoppingCart.TotalPrice.ToString("F2").Replace(",", "."),
                        ShoppingCart.TotalShippingWeight.ToString().Replace(",", "."),
                        Math.Ceiling(ShoppingCart.TotalItems));
                return PostRequestGetObject<CheckoutCost>(str);
            }
            else
            {
                var str =
                  string.Format(
                      "checkout/calculation?ticket={0}&placeId={1}&totalSum={2}&assessedSum={3}&totalWeight={4}&itemsCount={5}",
                      ticket,
                      placeId,
                      OrderItems.Sum(item => item.Price * item.Amount).ToString("F2").Replace(",", "."),
                      OrderItems.Sum(item => item.Price * item.Amount).ToString("F2").Replace(",", "."),
                      OrderItems.Sum(item => item.Weight * item.Amount).ToString().Replace(",", "."),
                      Math.Ceiling(ShoppingCart.TotalItems));
                return PostRequestGetObject<CheckoutCost>(str);
            }
        }

        private T PostRequestGetObject<T>(string urlPath, string data = "", string requestMethod = "GET")
        {
            var request = WebRequest.Create(Url + urlPath);
            request.Method = requestMethod;

            if (!string.IsNullOrEmpty(data))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(data);

                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
            }

            using (var response = request.GetResponse())
            {
                var responseString = (new StreamReader(response.GetResponseStream())).ReadToEnd();

                return typeof(T) != typeof(string) ? Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseString) : (T)(object)responseString;
            }
        }

        private float ProcessRate(float rate)
        {
            if (ExtrachargeType == ExtrachargeType.Percent)
                return rate + rate * Extracharge / 100;
            return rate + Extracharge;
        }
    }

}