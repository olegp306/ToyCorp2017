//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AdvantShop.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Shipping.ShippingCdek;
using Newtonsoft.Json;
using System.Web;

namespace AdvantShop.Shipping
{
    public class Cdek : IShippingMethod
    {
        #region Constants

        private const string _urlNewOrders = "http://gw.edostavka.ru:11443/new_orders.php";
        private const string _urlNewSchedule = "http://gw.edostavka.ru:11443/new_schedule.php";
        private const string _urlCallCourier = "http://gw.edostavka.ru:11443/call_courier.php";
        private const string _urlDeleteOrders = "http://gw.edostavka.ru:11443/delete_orders.php";
        private const string _urlOrdersPrint = "http://gw.edostavka.ru:11443/orders_print.php";
        private const string _urlOrdersStatusReport = "http://gw.edostavka.ru:11443/status_report_h.php";
        private const string _urlOrdersInfoReport = "http://gw.edostavka.ru:11443/info_report.php";


        private const string _urlCalculatePrice = "http://api.edostavka.ru/calculator/calculate_price_by_json.php";
        private const string _urlGetListOfCityPoints = "http://gw.edostavka.ru:11443/pvzlist.php";

        public static readonly List<CdekTariff> tariffs = new List<CdekTariff>
            {
                new CdekTariff{tariffId = 1, name = "Экспресс лайт дверь-дверь", mode = "Д-Д"},
                new CdekTariff{tariffId = 3, name = "Супер-экспресс до 18", mode = "Д-Д"},
                new CdekTariff{tariffId = 4, name = "Рассылка", mode = "Д-Д"},
                new CdekTariff{tariffId = 5, name = "Экономичный экспресс склад-склад", mode = "С-С"},
                new CdekTariff{tariffId = 7, name = "Международный экспресс документы", mode = "Д-Д"},
                new CdekTariff{tariffId = 8, name = "Международный экспресс грузы", mode = "Д-Д"},
                new CdekTariff{tariffId = 10, name = "Экспресс лайт склад-склад", mode = "С-С"},
                new CdekTariff{tariffId = 11, name = "Экспресс лайт склад-дверь", mode = "С-Д"},
                new CdekTariff{tariffId = 12, name = "Экспресс лайт дверь-склад", mode = "Д-С"},
                new CdekTariff{tariffId = 15, name = "Экспресс тяжеловесы склад-склад", mode = "С-С"},

                new CdekTariff{tariffId = 16, name = "Экспресс тяжеловесы склад-дверь", mode = "С-Д"},
                new CdekTariff{tariffId = 17, name = "Экспресс тяжеловесы дверь-склад", mode = "Д-С"},
                new CdekTariff{tariffId = 18, name = "Экспресс тяжеловесы дверь-дверь", mode = "Д-Д"},
                new CdekTariff{tariffId = 57, name = "Супер-экспресс до 9", mode = "Д-Д"},

                new CdekTariff{tariffId = 58, name = "Супер-экспресс до 10", mode = "Д-Д"},
                new CdekTariff{tariffId = 59, name = "Супер-экспресс до 12", mode = "Д-Д"},
                new CdekTariff{tariffId = 60, name = "Супер-экспресс до 14", mode = "Д-Д"},
                new CdekTariff{tariffId = 61, name = "Супер-экспресс до 16", mode = "Д-Д"},
                new CdekTariff{tariffId = 62, name = "Магистральный экспресс склад-склад", mode = "С-С"},
                new CdekTariff{tariffId = 63, name = "Магистральный супер-экспресс склад-склад", mode = "С-С"},

                new CdekTariff{tariffId = 66, name = "Блиц-экспресс 01", mode = "Д-Д"},
                new CdekTariff{tariffId = 67, name = "Блиц-экспресс 02", mode = "Д-Д"},
                new CdekTariff{tariffId = 68, name = "Блиц-экспресс 03", mode = "Д-Д"},
                new CdekTariff{tariffId = 69, name = "Блиц-экспресс 04", mode = "Д-Д"},
                new CdekTariff{tariffId = 70, name = "Блиц-экспресс 05", mode = "Д-Д"},
                new CdekTariff{tariffId = 71, name = "Блиц-экспресс 06", mode = "Д-Д"},
                new CdekTariff{tariffId = 72, name = "Блиц-экспресс 07", mode = "Д-Д"},
                new CdekTariff{tariffId = 73, name = "Блиц-экспресс 08", mode = "Д-Д"},
                new CdekTariff{tariffId = 74, name = "Блиц-экспресс 09", mode = "Д-Д"},
                new CdekTariff{tariffId = 75, name = "Блиц-экспресс 10", mode = "Д-Д"},
                new CdekTariff{tariffId = 76, name = "Блиц-экспресс 11", mode = "Д-Д"},
                new CdekTariff{tariffId = 77, name = "Блиц-экспресс 12", mode = "Д-Д"},
                new CdekTariff{tariffId = 78, name = "Блиц-экспресс 13", mode = "Д-Д"},
                new CdekTariff{tariffId = 79, name = "Блиц-экспресс 14", mode = "Д-Д"},
                new CdekTariff{tariffId = 80, name = "Блиц-экспресс 15", mode = "Д-Д"},
                new CdekTariff{tariffId = 81, name = "Блиц-экспресс 16", mode = "Д-Д"},
                new CdekTariff{tariffId = 82, name = "Блиц-экспресс 17", mode = "Д-Д"},
                new CdekTariff{tariffId = 83, name = "Блиц-экспресс 18", mode = "Д-Д"},
                new CdekTariff{tariffId = 84, name = "Блиц-экспресс 19", mode = "Д-Д"},
                new CdekTariff{tariffId = 85, name = "Блиц-экспресс 20", mode = "Д-Д"},
                new CdekTariff{tariffId = 86, name = "Блиц-экспресс 21", mode = "Д-Д"},
                new CdekTariff{tariffId = 87, name = "Блиц-экспресс 22", mode = "Д-Д"},
                new CdekTariff{tariffId = 88, name = "Блиц-экспресс 23", mode = "Д-Д"},
                new CdekTariff{tariffId = 89, name = "Блиц-экспресс 24", mode = "Д-Д"},
                //для интернет магазинов
                new CdekTariff{tariffId = 136, name = "Посылка склад-склад", mode = "С-С"},
                new CdekTariff{tariffId = 137, name = "Посылка склад-дверь", mode = "С-Д"},
                //new CdekTariff{tariffId = 140, name = "Возврат склад-склад", mode = "С-С"},
                //new CdekTariff{tariffId = 141, name = "Возврат склад-дверь", mode = "С-Д"},
                //new CdekTariff{tariffId = 142, name = "Возврат дверь-склад", mode = "Д-С"}
            };


        public static readonly List<AddedService> addedServise = new List<AddedService>
            {
                new AddedService{Code = 30, Name = "Примерка на дому", Description = "Курьер доставляет покупателю несколько единиц товара (одежда, обувь и пр.) для примерки. Время ожидания курьера в этом случае составляет 30 минут."},
                new AddedService{Code=36, Name="Частичная доставка", Description = "Во время доставки товара покупатель может отказаться от одной или нескольких позиций, и выкупить только часть заказа"},
                new AddedService{Code=37, Name="Осмотр вложения", Description = "Проверка покупателем содержимого заказа до его оплаты (вскрытие посылки)."},
                new AddedService{Code=3, Name="Доставка в выходной день",Description = "Осуществление доставки заказа в выходные и нерабочие дни"},
                new AddedService{Code=16, Name="Забор в городе отправителя",Description = "Дополнительная услуга забора груза в городе отправителя, при условии что тариф доставки с режимом «от склада»"},
                new AddedService{Code=17, Name="Доставка в городе получателя",Description = "Дополнительная услуга доставки груза в городе получателя, при условии что тариф доставки с режимом «до склада» (только для тарифов «Магистральный», «Магистральный супер-экспресс»)"},
                new AddedService{Code=2, Name = "Страхование",Description = "Обеспечение страховой защиты посылки. Размер дополнительного сбора страхования вычисляется от размера объявленной стоимости отправления. Важно: Услуга начисляется автоматически для всех заказов ИМ, не разрешена для самостоятельной передачи в тэге AddService."}
            };


        #endregion

        private string AuthLogin { get; set; }
        private string AuthPassword { get; set; }
        private string ActiveTariffs { get; set; }

        //**
        public string CountryFrom { get; set; }
        public string ZipFrom { get; set; }
        public string StateFrom { get; set; }
        public string CityFrom { get; set; }
        public string AddressFrom { get; set; }

        //***
        public string CountryTo { get; set; }
        public string ZipTo { get; set; }
        public string StateTo { get; set; }
        public string CityTo { get; set; }
        public string AddressTo { get; set; }

        public string MethodName { get; set; }

        public float TotalPrice { get; set; }

        public float AdditionalPrice { get; set; }

        public ShoppingCart ShoppingCart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public Cdek(Dictionary<string, string> parameters)
        {
            AuthLogin = parameters.ElementOrDefault(CdekTemplate.AuthLogin);
            AuthPassword = parameters.ElementOrDefault(CdekTemplate.AuthPassword);
            ActiveTariffs = parameters.ElementOrDefault(CdekTemplate.ActiveTariffs);
            CityFrom = parameters.ElementOrDefault(CdekTemplate.CityFrom);

            float additionalPrice = 0;
            if (Single.TryParse(parameters.ElementOrDefault(CdekTemplate.AdditionalPrice), out additionalPrice))
            {
                AdditionalPrice = additionalPrice;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float GetRate()
        {
            throw new Exception("GetRate method isnot avalible for Cdek");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ShippingOption> GetShippingOptions()
        {

            if (string.IsNullOrEmpty(CityTo) || string.IsNullOrEmpty(CityFrom))
            {
                return new List<ShippingOption>();
            }

            var cdekCityToId = GetCdekCityId(CityTo);
            var cdekCityFromId = GetCdekCityId(CityFrom);

            var listShippingOptions = new List<ShippingOption>();

            var dateExecute = DateTime.Now.Date.ToString("yyyy-MM-dd");

            var goods = new List<CdekGoods>();

            foreach (var item in ShoppingCart)
            {

                var currentProduct = ProductService.GetProduct(item.Offer.ProductId);
                if (currentProduct != null)
                {
                    var sizes = currentProduct.Size.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (sizes.Count() == 3)
                    {

                        var length = Math.Round(Convert.ToSingle(sizes[0]) / 10);
                        var width = Math.Round(Convert.ToSingle(sizes[1]) / 10);
                        var height = Math.Round(Convert.ToSingle(sizes[2]) / 10);

                        goods.Add(new CdekGoods
                        {
                            weight =
                                (currentProduct.Weight * item.Amount) == 0
                                    ? "0.001"
                                    : (currentProduct.Weight * item.Amount).ToString().Replace(",", "."),
                            length = length > 0 ? length.ToString() : "1",
                            width = width > 0 ? width.ToString() : "1",
                            height = height > 0 ? height.ToString() : "1",
                        });
                    }
                    else
                    {

                        goods.Add(new CdekGoods
                        {
                            weight =
                                (currentProduct.Weight * item.Amount) == 0
                                    ? "0.001"
                                    : (currentProduct.Weight * item.Amount).ToString().Replace(",", "."),
                            length = "1",
                            width = "1",
                            height = "1",
                        });
                    }
                }
            }

            var listShippingPoints = new List<ShippingPoint>();
            int index = 0;
            foreach (var cdekPoint in GetListOfCityPoints(cdekCityToId))
            {
                listShippingPoints.Add(new ShippingPoint
                {
                    Id = index,
                    Code = cdekPoint.Code,
                    Address = cdekPoint.Address,
                    Description = cdekPoint.Note + " " + cdekPoint.WorkTime
                });
                ++index;
            }

            foreach (var cdekTarrifId in ActiveTariffs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var cdekTariff = tariffs.FirstOrDefault(item => string.Equals(item.tariffId.ToString(), cdekTarrifId));
                if (cdekTariff == null)
                {
                    continue;
                }

                var jsonData = JsonConvert.SerializeObject(
                    new
                    {
                        version = "1.0",
                        dateExecute = dateExecute,
                        authLogin = AuthLogin,
                        secure = GetMd5Hash(MD5.Create(), dateExecute + "&" + AuthPassword),
                        senderCityId = cdekCityFromId.ToString(),
                        receiverCityId = cdekCityToId.ToString(),
                        tariffId = cdekTariff.tariffId,
                        goods = goods
                    });

                var responseResult = PostRequestGetString(_urlCalculatePrice, jsonData, "application/json");

                var cdekAnswer = JsonConvert.DeserializeObject<CdekResponse>(responseResult);
                if (cdekAnswer.error == null)
                {
                    var shippingOptionRate = cdekAnswer.result.priceByCurrency + AdditionalPrice;
                    var shippingOption = new ShippingOption
                    {
                        Rate = shippingOptionRate < 0 ? 0 : shippingOptionRate,
                        Name = MethodName,
                        DeliveryTime = cdekAnswer.result.deliveryPeriodMin + "-" + cdekAnswer.result.deliveryPeriodMax + " дней",
                        ShippingPoints = cdekTariff.mode.EndsWith("-Д") ? null : listShippingPoints,
                        Extend = new ShippingOptionEx
                        {
                            AdditionalData = cdekAnswer.result.tariffId.ToString(),
                            Type = ExtendedType.Cdek
                        }
                    };
                    listShippingOptions.Add(shippingOption);
                }
                else
                {
                    var errors = cdekAnswer.error.Aggregate("", (current, error) => current + (error.code + " " + error.text));
                    Debug.LogError(new Exception("Cdek: " + errors), false);
                }
            }

            return listShippingOptions;
        }

        #region********************************* Cdek Api Methods *********************************

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="tariffId"></param>
        /// <param name="pickpointId"></param>
        /// <returns></returns>
        public CdekStatusAnswer SendNewOrders(Order order, int tariffId, string pickpointId)
        {
            if (order == null || order.OrderCustomer == null || order.ShippingContact.Address == null)
            {
                return new CdekStatusAnswer
                    {
                        Status = false,
                        Message = "Ошибка при добавление заказа в систему СДЭК"
                    };
            }

            var cdekSendCityCode = GetCdekCityId(CityFrom);
            var cdekRecCityCode = GetCdekCityId(order.ShippingContact.City);
            var dateExecute = order.OrderDate.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss");

            var resultXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            resultXml += string.Format("<DeliveryRequest Number=\"{0}\" Date=\"{1}\" Account=\"{2}\" Secure=\"{3}\" OrderCount=\"1\">",
                order.OrderID,
                dateExecute,
                AuthLogin,
                GetMd5Hash(MD5.Create(), dateExecute + "&" + AuthPassword));

            //Order tag
            resultXml += string.Format(
             "<Order Number=\"{0}\" SendCityCode=\"{1}\" RecCityCode=\"{2}\" SendCityPostCode=\"\" RecCityPostCode=\"\" RecipientName=\"{3}\" RecipientEmail=\"{4}\" Phone=\"{5}\" TariffTypeCode=\"{6}\" Comment=\"{7}\" SellerName=\"{8}\" DeliveryRecipientCost=\"{9}\">",
             order.Number,
             cdekSendCityCode,
             cdekRecCityCode,
             order.OrderCustomer.FirstName + " " + order.OrderCustomer.LastName,
             order.OrderCustomer.Email,
             order.OrderCustomer.MobilePhone,
             tariffId,
             order.CustomerComment,
             Configuration.SettingsMain.ShopName,
             order.Payed ? 0 : order.ShippingCost
             );

            //Address tag
            resultXml += string.Format("<Address Street=\"{0}\" House=\"0\" Flat=\"0\" PvzCode=\"{1}\"/>", order.OrderPickPoint.PickPointAddress, pickpointId);

            var itemsPrice = order.OrderItems.Sum(item => item.Price * item.Amount);
            var difference = (order.Sum - order.ShippingCost) - itemsPrice;

            float itemsPriceWithDifferenceSum = 0;

            //Package tag
            resultXml += string.Format("<Package Number=\"{0}\" BarCode=\"{0}\" Weight=\"{1}\">", 1, order.OrderItems.Sum(item => (item.Weight * 1000) * item.Amount));
            int index = 1;
            foreach (var orderItem in order.OrderItems)
            {
                var relationSum = Math.Round(difference / 100 * (orderItem.Price / (itemsPrice / 100)), 2);

                itemsPriceWithDifferenceSum += (orderItem.Price + (float)relationSum) * orderItem.Amount;

                resultXml += string.Format(
                    "<Item WareKey=\"{0}\" Cost=\"{1}\" Payment=\"{2}\" Weight=\"{3}\" Amount=\"{4}\" Comment=\"{5}\"/>",
                    orderItem.ArtNo,
                    orderItem.Price + relationSum + (index == order.OrderItems.Count ? Math.Round(order.Sum - order.ShippingCost) - itemsPriceWithDifferenceSum : 0),
                    order.PaymentMethod.Type == PaymentType.Cash || order.PaymentMethod.Type == PaymentType.CashOnDelivery || !order.Payed
                        ? orderItem.Price + relationSum + (index == order.OrderItems.Count ? Math.Round(order.Sum - order.ShippingCost) - itemsPriceWithDifferenceSum : 0)
                        : 0,
                    (orderItem.Weight * 1000),
                    orderItem.Amount,
                    HttpUtility.UrlEncode(orderItem.Name.Replace("\"", "'")));
                index++;
            }

            resultXml += "</Package>";
            resultXml += "</Order>";
            resultXml += "</DeliveryRequest>";

            var responceString = PostRequestGetString(_urlNewOrders, "xml_request=" + resultXml, "application/x-www-form-urlencoded");
            
            if (responceString.Contains("ERR_ORDER_DUBL_EXISTS"))
            {
                return new CdekStatusAnswer
                    {
                        Status = false,
                        Message = "Заказ уже существует в системе"
                    };
            }
            else if (responceString.Contains("ERR_INVALID_WEIGHT"))
            {
                return new CdekStatusAnswer
                {
                    Status = false,
                    Message = "Значение веса должно быть больше 0"
                };
            }
            else if (responceString.Contains("ERR_"))
            {
                return new CdekStatusAnswer
                {
                    Status = false,
                    Message = "Ошибка добавления заказа" 
                };
            }
            else
            {
                return new CdekStatusAnswer
                    {
                        Status = true,
                        Message = "Заказ добавлен в систему СДЭК"
                    };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReportOrderStatuses()
        {
            var dateExecute = DateTime.UtcNow.Date.ToString("yyyy-MM-ddThh:mm:ss");
            var resultXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            resultXml += string.Format(
                "<StatusReport Date=\"{0}\" Account=\"{1}\" Secure=\"{2}\" ShowHistory=\"{3}\">",
                dateExecute,
                AuthLogin,
                GetMd5Hash(MD5.Create(), dateExecute + "&" + AuthPassword),
                1);

            resultXml += "</StatusReport>";

            var responceString = PostRequestGetString(_urlNewOrders, "xml_request=" + resultXml, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        public CdekStatusAnswer ReportOrderStatuses(Order order)
        {
            var dateExecute = DateTime.UtcNow.Date.ToString("yyyy-MM-ddThh:mm:ss");
            var resultXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            resultXml += string.Format(
                "<StatusReport Date=\"{0}\" Account=\"{1}\" Secure=\"{2}\" ShowHistory=\"{3}\">",
                dateExecute,
                AuthLogin,
                GetMd5Hash(MD5.Create(), dateExecute + "&" + AuthPassword),
                1);

            resultXml += string.Format("<Order DispatchNumber=\"{0}\" Number=\"{1}\" Date=\"{2}\" />",
                order.OrderID,
                order.Number,
                order.OrderDate.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"));

            resultXml += "</StatusReport>";

            var responceString = PostRequestGetString(_urlOrdersStatusReport, "xml_request=" + resultXml, "application/x-www-form-urlencoded");

            CdekOrderStatusInfo result;

            using (var reader = new StringReader(responceString))
            {
                var serializer = new XmlSerializer(typeof(CdekOrderStatusInfo));
                result = (CdekOrderStatusInfo)serializer.Deserialize(reader);
            }

            return new CdekStatusAnswer
                {
                    Message = result != null && result.Orders != null
                        ? "Информация о статусах получена"
                        : string.Format("Заказ {0} в системе не найден", order.Number),
                    Status = result != null && result.Orders != null,
                    Object = CreateOrderStatusReport(result)
                };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        public CdekStatusAnswer ReportOrdersInfo(Order order)
        {
            var dateExecute = DateTime.UtcNow.Date.ToString("yyyy-MM-ddThh:mm:ss");
            var resultXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            resultXml += string.Format(
                "<InfoRequest Date=\"{0}\" Account=\"{1}\" Secure=\"{2}\">",
                dateExecute,
                AuthLogin,
                GetMd5Hash(MD5.Create(), dateExecute + "&" + AuthPassword));

            resultXml += string.Format(
                "<Order DispatchNumber=\"{0}\" Number=\"{1}\" Date=\"{2}\" />",
                string.Empty,//order.OrderID,
                order.Number,
                order.OrderDate.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"));

            resultXml += "</InfoRequest>";

            var responceString = PostRequestGetString(_urlOrdersInfoReport, "xml_request=" + resultXml, "application/x-www-form-urlencoded");

            if (responceString.Contains("ERR_INVALID_NUMBER"))
            {
                return new CdekStatusAnswer
                {
                    Message = string.Format("Заказ {0} в системе не найден", order.Number),
                    Status = false,
                    Object = CreateOrderInfoReport(null)
                };
            }

            CdekOrderInfoReport result;

            using (var reader = new StringReader(responceString))
            {
                var serializer = new XmlSerializer(typeof(CdekOrderInfoReport));
                result = (CdekOrderInfoReport)serializer.Deserialize(reader);
            }

            return new CdekStatusAnswer
            {
                Message = result != null && result.Orders != null
                    ? "Информация о заказе получена"
                    : string.Format("Заказ {0} в системе не найден", order.Number),
                Status = result != null && result.Orders != null,
                Object = CreateOrderInfoReport(result)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReportOrdersInfo()
        {
            var dateExecute = DateTime.UtcNow.Date.ToString("yyyy-MM-ddThh:mm:ss");
            var resultXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            resultXml += string.Format(
                "<InfoRequest Date=\"{0}\" Account=\"{1}\" Secure=\"{2}\">",
                dateExecute,
                AuthLogin,
                GetMd5Hash(MD5.Create(), dateExecute + "&" + AuthPassword));

            resultXml += "</InfoRequest>";

            var responceString = PostRequestGetString(_urlNewOrders, "xml_request=" + resultXml, "application/x-www-form-urlencoded");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        public string PrintFormOrder(Order order)
        {
            var dateExecute = DateTime.UtcNow.Date.ToString("yyyy-MM-ddThh:mm:ss");
            var resultXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            resultXml += string.Format(
                "<OrdersPrint Date=\"{0}\" Account=\"{1}\" Secure=\"{2}\" OrderCount=\"1\">",
                dateExecute,
                AuthLogin,
                GetMd5Hash(MD5.Create(), dateExecute + "&" + AuthPassword));

            resultXml += string.Format("<Order DispatchNumber=\"{0}\" Number=\"{1}\" Date=\"{2}\"/>",
                string.Empty,
                order.Number,
            order.OrderDate.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"));

            resultXml += "</OrdersPrint>";

            var printFormFilePath =
                string.Format(Configuration.SettingsGeneral.AbsolutePath + "price_temp\\");
            var fileName = string.Format("cdekPrintFormOrder{0}.pdf", order.OrderID);

            using (var responseStream = PostRequestGetStream(_urlOrdersPrint, "xml_request=" + resultXml, "application/x-www-form-urlencoded"))
            {
                using (var memoryStream = new MemoryStream())
                {
                    responseStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    var responseString = (new StreamReader(memoryStream)).ReadToEnd();
                    if (responseString.Contains("ERR_INVALID_NUMBER"))
                    {
                        memoryStream.Dispose();
                        responseStream.Dispose();
                        return CreatePrintFormNotOrder(order.Number);
                    }

                    using (var filestream = new FileStream(printFormFilePath + fileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        memoryStream.Position = 0;
                        memoryStream.CopyTo(filestream);
                    }
                }
            }

            return fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        public CdekStatusAnswer DeleteOrder(Order order)
        {
            var dateExecute = DateTime.UtcNow.Date.ToString("yyyy-MM-ddThh:mm:ss");
            var resultXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            resultXml += string.Format(
                "<DeleteRequest Number=\"{0}\" Date=\"{1}\" Account=\"{2}\" Secure=\"{3}\" OrderCount=\"1\">",
                order.OrderID,
                dateExecute,
                AuthLogin,
                GetMd5Hash(MD5.Create(), dateExecute + "&" + AuthPassword));

            resultXml += string.Format("<Order Number=\"{0}\"/>", order.Number);

            resultXml += "</DeleteRequest>";

            var responceString = PostRequestGetString(_urlDeleteOrders, "xml_request=" + resultXml, "application/x-www-form-urlencoded");
            if (responceString.Contains("ERR_ORDER_NOTFIND"))
            {
                return new CdekStatusAnswer { Message = string.Format("Заказ {0} не найден в системе СДЭК", order.Number), Status = false };
            }
            else
            {
                return new CdekStatusAnswer { Message = string.Format("Заказ {0} удален из системы СДЭК", order.Number), Status = true };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        public CdekStatusAnswer CallCustomer(Order order)
        {
            var dateExecute = DateTime.UtcNow.Date.ToString("yyyy-MM-ddThh:mm:ss");
            var resultXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            resultXml += string.Format(
                "<ScheduleRequest Date=\"{0}\" Account=\"{1}\" Secure=\"{2}\" OrderCount=\"1\">",
                dateExecute,
                AuthLogin,
                GetMd5Hash(MD5.Create(), dateExecute + "&" + AuthPassword));

            resultXml += string.Format(
                "<Order DispatchNumber=\"{0}\" Number=\"{1}\" Date=\"{2}\"/>",
                string.Empty,//order.OrderID,
                order.Number,
                order.OrderDate.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss"));

            resultXml += string.Format(
                "<Attempt ID=\"{0}\" Date=\"{1}\"/>",
                order.OrderID,
                DateTime.Now.AddDays(1).ToString("yyyy-MM-ddThh:mm:ss"));

            resultXml += "</ScheduleRequest>";

            var responceString = PostRequestGetString(_urlNewSchedule, "xml_request=" + resultXml, "application/x-www-form-urlencoded");

            if (responceString.Contains("ERR_INVALID_NUMBER"))
            {
                return new CdekStatusAnswer { Message = string.Format("Заказ {0} не найден в системе СДЭК", order.Number), Status = false };
            }
            else
            {
                return new CdekStatusAnswer { Message = string.Format("Прозвон получателя создан в заказе {0}", order.Number), Status = true };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeBegin"></param>
        /// <param name="timeEnd"></param>
        /// <param name="cityName"></param>
        /// <param name="street"></param>
        /// <param name="house"></param>
        /// <param name="flat"></param>
        /// <param name="phone"></param>
        /// <param name="name"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public CdekStatusAnswer CallCourier(DateTime date, DateTime timeBegin, DateTime timeEnd, string cityName, string street, string house, string flat, string phone, string name, string weight)
        {
            var dateExecute = DateTime.UtcNow.Date.ToString("yyyy-MM-ddThh:mm:ss");
            var resultXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            resultXml += string.Format(
                "<CallCourier Date=\"{0}\" Account=\"{1}\" Secure=\"{2}\" CallCount=\"1\">",
                dateExecute,
                AuthLogin,
                GetMd5Hash(MD5.Create(), dateExecute + "&" + AuthPassword));

            resultXml += string.Format(
                "<Call Date=\"{0}\" TimeBeg=\"{1}\" TimeEnd=\"{2}\" SendCityCode=\"{3}\" SendPhone=\"{4}\" SenderName=\"{5}\" Weight=\"{6}\" >",
                date.ToString("yyyy-MM-ddThh:mm:ss"),
                timeBegin.ToString("yyyy-MM-ddTHH:mm:ss"),
                timeEnd.ToString("yyyy-MM-ddTHH:mm:ss"),
                GetCdekCityId(cityName),
                phone,
                name,
                weight);

            resultXml += string.Format("<Address Street=\"{0}\" House=\"{1}\" Flat=\"{2}\" />",
                street,
                house,
                flat);

            resultXml += "</Call></CallCourier>";

            var serializer = new XmlSerializer(typeof(CdekXmlResponse));
            var result = new CdekXmlResponse();

            try
            {
                result = (CdekXmlResponse)serializer.Deserialize(
                    new StreamReader(PostRequestGetStream(_urlCallCourier, "xml_request=" + resultXml, "application/x-www-form-urlencoded")));
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                {
                    using (var eStream = eResponse.GetResponseStream())
                    {
                        if (eStream != null)
                            using (var reader = new StreamReader(eStream))
                            {
                                result = (CdekXmlResponse)serializer.Deserialize(reader);
                            }
                    }
                }
            }

            return new CdekStatusAnswer { Message = result.CallCourier.Msg, Status = string.IsNullOrEmpty(result.CallCourier.ErrorCode) };
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cdekCityId"></param>
        /// <returns></returns>
        private List<CdekPvz> GetListOfCityPoints(int cdekCityId)
        {
            ServicePointManager.Expect100Continue = false;
            var request = WebRequest.Create(_urlGetListOfCityPoints + "?cityid=" + cdekCityId);
            request.Method = "GET";

            var result = string.Empty;
            using (var response = request.GetResponse())
            {
                result = (new StreamReader(response.GetResponseStream())).ReadToEnd();
            }

            var listPvz = new List<CdekPvz>();

            using (var xmlReader = XmlReader.Create(new StringReader(result)))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() && string.Equals(xmlReader.Name, "Pvz"))
                    {
                        var cdekPvz = new CdekPvz
                            {
                                Code = xmlReader.GetAttribute("Code"),
                                Name = xmlReader.GetAttribute("Name"),
                                CityCode = xmlReader.GetAttribute("CityCode"),
                                City = xmlReader.GetAttribute("City"),
                                WorkTime = xmlReader.GetAttribute("WorkTime"),
                                Address = xmlReader.GetAttribute("Address"),
                                Phone = xmlReader.GetAttribute("Phone"),
                                Note = xmlReader.GetAttribute("Note"),
                            };
                        listPvz.Add(cdekPvz);

                        while (xmlReader.Read() && !string.Equals(xmlReader.Name, "Pvz"))
                        {
                            if (string.Equals(xmlReader.Name, "Pvz"))
                            {
                                cdekPvz.WeightLimit = new CdekPvzWeightLimit
                                    {
                                        WeightMin = xmlReader.GetAttribute("WeightMin"),
                                        WeightMax = xmlReader.GetAttribute("WeightMax")
                                    };
                            }
                        }
                    }
                }
            }

            return listPvz;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private string PostRequestGetString(string url, string data, string contentType)
        {
            var request = WebRequest.Create(url);
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(data);

            request.ContentType = contentType;
            request.ContentLength = byteArray.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }
            using (var response = request.GetResponse())
            {
                return (new StreamReader(response.GetResponseStream())).ReadToEnd();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private Stream PostRequestGetStream(string url, string data, string contentType)
        {
            var request = WebRequest.Create(url);
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(data);

            request.ContentType = contentType;
            request.ContentLength = byteArray.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            return request.GetResponse().GetResponseStream();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        private int GetCdekCityId(string cityName)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT TOP 1 [Id] FROM [Shipping].[CdekCities] WHERE CityName = @CityName",
                CommandType.Text,
                new SqlParameter("@CityName", cityName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cdekOrderStatusInfo"></param>
        /// <returns></returns>
        private string CreateOrderStatusReport(CdekOrderStatusInfo cdekOrderStatusInfo)
        {
            var filePath = string.Format(Configuration.SettingsGeneral.AbsolutePath + "price_temp\\");
            var fileName = string.Format("cdekOrderStatusInfoReport_{0}.txt", DateTime.Now.ToShortDateString());

            using (var writer = new StreamWriter(filePath + fileName))
            {
                if (cdekOrderStatusInfo.Orders == null || !cdekOrderStatusInfo.Orders.Any())
                {
                    writer.WriteLine("Заказ не найден в системе СДЭК");
                    writer.Dispose();
                    return fileName;
                }

                writer.WriteLine("Отчет «Статусы заказов» {0} - {1}",
                    cdekOrderStatusInfo.DateFirst,
                    cdekOrderStatusInfo.DateLast);

                writer.WriteLine("\tНомер акта приема-передачи: " + cdekOrderStatusInfo.Orders[0].ActNumber);

                writer.WriteLine("\tНомер отправления клиента: " + cdekOrderStatusInfo.Orders[0].Number);

                writer.WriteLine("\tНомер отправления СДЭК (присваивается при импорте заказов): " + cdekOrderStatusInfo.Orders[0].DispatchNumber);

                writer.WriteLine("\tДата доставки: " + cdekOrderStatusInfo.Orders[0].DeliveryDate);

                writer.WriteLine("\tПолучатель при доставке: " + cdekOrderStatusInfo.Orders[0].RecipientName);

                writer.WriteLine("Текущий статус заказа");

                writer.WriteLine("\tCтатус: {0} {1} {2}. Город изменения статуса {3} {4}",
                    cdekOrderStatusInfo.Orders[0].Status.Code,
                    cdekOrderStatusInfo.Orders[0].Status.Date,
                    cdekOrderStatusInfo.Orders[0].Status.Description,
                    cdekOrderStatusInfo.Orders[0].Status.CityCode,
                    cdekOrderStatusInfo.Orders[0].Status.CityName);

                writer.WriteLine("История изменения статусов");

                foreach (var state in cdekOrderStatusInfo.Orders[0].Status.State)
                {
                    writer.WriteLine("\t{0} {1} {2}. Города изменения статуса {3} {4}",
                        state.Code,
                        state.Date,
                        state.Description,
                        state.CityCode,
                        state.CityName);
                }

                writer.WriteLine("Текущий дополнительный статус {0} {1} {2}",
                    cdekOrderStatusInfo.Orders[0].Reason.Code,
                    cdekOrderStatusInfo.Orders[0].Reason.Date,
                    cdekOrderStatusInfo.Orders[0].Reason.Description);

                writer.WriteLine("Текущая причина задержки {0} {1} {2}",
                    cdekOrderStatusInfo.Orders[0].DelayReason.Code,
                    cdekOrderStatusInfo.Orders[0].DelayReason.Date,
                    cdekOrderStatusInfo.Orders[0].DelayReason.Description);

                if (cdekOrderStatusInfo.Orders[0].Call != null)
                {
                    if (cdekOrderStatusInfo.Orders[0].Call.CallGood != null)
                    {
                        writer.WriteLine("История прозвонов получателя");
                        foreach (var callGood in cdekOrderStatusInfo.Orders[0].Call.CallGood.Good)
                        {
                            writer.WriteLine("\tУдачный прозвон {0}, дата доставки {1}",
                                callGood.Date,
                                callGood.DateDeliv);
                        }
                    }

                    if (cdekOrderStatusInfo.Orders[0].Call.CallFail != null)
                    {
                        writer.WriteLine("История неудачных прозвонов");
                        foreach (var callFail in cdekOrderStatusInfo.Orders[0].Call.CallFail.Fail)
                        {
                            writer.WriteLine("\tНеудачный прозвон {0}, Причина неудачного прозвона {1} {2}",
                                callFail.Date,
                                callFail.ReasonCode,
                                callFail.ReasonDescription);
                        }
                    }

                    if (cdekOrderStatusInfo.Orders[0].Call.CallDelay != null)
                    {
                        writer.WriteLine("История переносов прозвона");
                        foreach (var callDelay in cdekOrderStatusInfo.Orders[0].Call.CallDelay.Delay)
                        {
                            writer.WriteLine("\tПеренос прозвона {0}, Дата, на которую перенесен прозвон {1}",
                                callDelay.Date,
                                callDelay.DateNext);
                        }
                    }
                }
            }

            return fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cdekOrderInfoReport"></param>
        /// <returns></returns>
        private string CreateOrderInfoReport(CdekOrderInfoReport cdekOrderInfoReport)
        {
            var filePath = string.Format(Configuration.SettingsGeneral.AbsolutePath + "price_temp\\");
            var fileName = string.Format("cdekOrderInfoReport_{0}.txt", DateTime.Now.ToShortDateString());

            using (var writer = new StreamWriter(filePath + fileName))
            {
                if (cdekOrderInfoReport == null || cdekOrderInfoReport.Orders == null || !cdekOrderInfoReport.Orders.Any())
                {
                    writer.WriteLine("Заказ не найден в системе СДЭК");
                    writer.Dispose();
                    return fileName;
                }

                writer.WriteLine("Отчет «Информация по заказам»");

                writer.WriteLine("Заказ");

                writer.WriteLine("\tДата, в которую был передан заказ в базу СДЭК: " + cdekOrderInfoReport.Orders[0].Date);

                writer.WriteLine("\tНомер отправления клиента: " + cdekOrderInfoReport.Orders[0].Number);

                writer.WriteLine("\tНомер отправления СДЭК (присваивается при импорте заказов): " + cdekOrderInfoReport.Orders[0].DispatchNumber);

                writer.WriteLine("\tКод типа тарифа: " + cdekOrderInfoReport.Orders[0].TariffTypeCode);

                writer.WriteLine("\tРасчетный вес (в граммах): " + cdekOrderInfoReport.Orders[0].Weight);

                writer.WriteLine("\tСтоимость услуги доставки, руб: " + cdekOrderInfoReport.Orders[0].DeliverySum);

                writer.WriteLine("\tДата последнего изменения суммы по услуге доставки: " + cdekOrderInfoReport.Orders[0].DateLastChange);

                writer.WriteLine("\tГород отправителя {0} {1}, почтовый индекс {2}",
                    cdekOrderInfoReport.Orders[0].SendCity.Code,
                    cdekOrderInfoReport.Orders[0].SendCity.Name,
                    cdekOrderInfoReport.Orders[0].SendCity.PostCode);

                writer.WriteLine("\tГород получателя {0} {1}, почтовый индекс {2}",
                  cdekOrderInfoReport.Orders[0].RecCity.Code,
                  cdekOrderInfoReport.Orders[0].RecCity.Name,
                  cdekOrderInfoReport.Orders[0].RecCity.PostCode);


                var addedService =
                    addedServise.FirstOrDefault(
                        item => item.Code == cdekOrderInfoReport.Orders[0].AddedService.ServiceCode);
                writer.WriteLine("\tДополнительные услуги к заказам: {0} руб. {1} {2}",
                    cdekOrderInfoReport.Orders[0].AddedService.Sum,
                    cdekOrderInfoReport.Orders[0].AddedService.ServiceCode,
                    addedService != null ? addedService.Name : string.Empty);
            }

            return fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        private string CreatePrintFormNotOrder(string orderNumber)
        {
            var filePath = string.Format(Configuration.SettingsGeneral.AbsolutePath + "price_temp\\");
            var fileName = string.Format("cdekPrintFormOrder_{0}.txt", DateTime.Now.ToShortDateString());

            using (var writer = new StreamWriter(filePath + fileName))
            {
                writer.WriteLine("Заказ {0} не найден в системе СДЭК", orderNumber);
            }
            return fileName;
        }
    }
}