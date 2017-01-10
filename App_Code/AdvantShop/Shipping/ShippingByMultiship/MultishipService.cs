using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AdvantShop.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository;
using Newtonsoft.Json;

namespace AdvantShop.Shipping
{
    public class MultishipService
    {
        private const string InitApiUrl = "https://multiship.ru/initApi";
        private const string OpenApiUrl = "https://multiship.ru/openAPI_v3";

       

        public static string GetSign(Dictionary<string, object> dict, string secretkey)
        {
            var preparedData = "";

            var items = dict.OrderBy(d => d.Key).Select(d => d.Value);

            foreach (var item in items)
            {
                if (item is Array)
                {
                    foreach (var subItem in (Array)item)
                    {
                        var subOrderItem = subItem as OrderItem;
                        if (subOrderItem != null)
                        {
                            preparedData += subOrderItem.ArtNo.Replace(",", ".") +
                                            subOrderItem.Price.ToString("F2").Replace(",", ".") + subOrderItem.Name +
                                            subOrderItem.Amount.ToString().Replace(",", ".");
                        }
                    }
                }
                else
                {
                    preparedData += item;
                }
            }

            return (preparedData + secretkey).Md5(false, Encoding.UTF8);
        }

        public static MultishipInitAnswer Register(string login, string password, string domain, ref string errorMessage)
        {
            var request = WebRequest.Create(InitApiUrl + "/init") as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var data = string.Format(
                "login={0}&password={1}&cmsName={2}&cmsVersion={3}&domain={4}&callbackUrl={5}",
                login, password, "AdVantShop.NET", "4.1", domain, "http://" + domain + "/callbacks");

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }
            
            var responseContent = "";
            using (var response = request.GetResponse() as HttpWebResponse)
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

            if (responseContent.IsNullOrEmpty() || responseContent.Contains("error"))
            {
                errorMessage = JsonConvert.DeserializeObject<MultishipInitError>(responseContent).error;
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<MultishipInitAnswer>(responseContent);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Debug.LogError(ex);
                return null;
            }
        }


        public static bool CreateOrder(Order order)
        {
            var multishipParams = ShippingMethodService.GetShippingParams(order.ShippingMethodId);

            var items = new List<string[]>();

            foreach (var item in order.OrderItems)
            {
                var product = ProductService.GetProduct(Convert.ToInt32(item.ProductID));
                if (product != null)
                {
                    var size = product.Size.Split('|');
                    for (int i = 0; i < item.Amount; i++)
                    {
                        items.Add(size);
                    }
                }
            }

            var heightAvg = Convert.ToInt32(multishipParams.ElementOrDefault(MultishipTemplate.HeightAvg));
            var widthAvg = Convert.ToInt32(multishipParams.ElementOrDefault(MultishipTemplate.WidthAvg));
            var lengthAvg = Convert.ToInt32(multishipParams.ElementOrDefault(MultishipTemplate.LengthAvg));

            var dimensions = MeasureHelper.GetDimensions(items, heightAvg, widthAvg, lengthAvg);

            var length = dimensions[0];
            var width = dimensions[1];
            var height = dimensions[2];

            var weight = order.OrderItems.Sum(x => x.Weight * x.Amount);
            weight = weight != 0 ? weight : Convert.ToInt32(multishipParams.ElementOrDefault(MultishipTemplate.WeightAvg));

            MultishipAdditionalData additionalData = null;
            try
            {
                if (order.OrderPickPoint != null && order.OrderPickPoint.AdditionalData.IsNotEmpty())
                {
                    additionalData =
                        JsonConvert.DeserializeObject<MultishipAdditionalData>(order.OrderPickPoint.AdditionalData);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }


            var dict = new Dictionary<string, object>()
            {
                {"order_date", order.OrderDate.ToUniversalTime()},

                {"order_weight", weight.ToString("F2").Replace(",", ".")},
                {"order_height", height},
                {"order_width", width},
                {"order_length", length},

                {"order_payment_method", "1"},
                {"order_delivery_cost", order.ShippingCost.ToString("F2").Replace(",", ".")},
                {"order_assessed_value", order.OrderItems.Sum(x => x.Price*x.Amount).ToString("F2").Replace(",", ".")},
                
                {"order_sender", multishipParams.ElementOrDefault(MultishipTemplate.SenderId)},
                {"order_requisite", multishipParams.ElementOrDefault(MultishipTemplate.RequisiteId)},
                {"order_warehouse", multishipParams.ElementOrDefault(MultishipTemplate.WarehouseId)},

                {"recipient_first_name", order.OrderCustomer.FirstName},
                {"recipient_last_name", ""},
                {"recipient_middle_name", ""},
                {"recipient_phone", order.OrderCustomer.MobilePhone.Replace("+", "").Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")},

                {"delivery_direction", additionalData != null ? additionalData.direction : 0},
                {"delivery_delivery", additionalData != null ? additionalData.delivery : 0},
                {"delivery_price", additionalData != null ? additionalData.price : 0},
                {"delivery_pickuppoint", order.OrderPickPoint != null ? order.OrderPickPoint.PickPointId : "1"},
                {"delivery_to_ms_warehouse", additionalData != null ? additionalData.to_ms_warehouse : 0},

                {"deliverypoint_city", order.ShippingContact.City},
                {"deliverypoint_index", order.ShippingContact.Zip},
                {"deliverypoint_street", order.ShippingContact.Address},

                {"order_num", order.OrderID},
                {"order_user_status_id", "-2"},
                {"recipient_comment", order.CustomerComment},
                {"client_id", multishipParams.ElementOrDefault(MultishipTemplate.ClientId)}
            };

            var dict2 = new Dictionary<string, object>(dict);
            dict2.Add("order_items", order.OrderItems.ToArray());

            var sign = GetSign(dict2, multishipParams.ElementOrDefault(MultishipTemplate.SecretKeyCreateOrder));
            
            var dataPost = string.Format("{0}&{1}&secret_key={2}",
                                    String.Join("&", dict.Select(x => x.Key + "=" + x.Value)),
                                    GetOrderItems(order.OrderItems),
                                    sign);
            

            var request = WebRequest.Create(OpenApiUrl + "/createOrder") as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
            request.ContentLength = bytes.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            var responseContent = "";
            using (var response = request.GetResponse() as HttpWebResponse)
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

            if (responseContent.IsNullOrEmpty() || responseContent.Contains("error") || !responseContent.Contains("status"))
            {
                Debug.LogError(responseContent);
                return false;
            }

            return true;
        }

        private static string GetOrderItems(IList<OrderItem> items)
        {
            var result = "";
            
            for(var i = 0; i < items.Count; i++)
            {
                result += (result != "" ? "&" : "") +
                          string.Format(
                              "order_items[{0}][orderitem_article]={1}&order_items[{0}][orderitem_cost]={2}&order_items[{0}][orderitem_name]={3}&order_items[{0}][orderitem_quantity]={4}",
                              i, items[i].ArtNo.Replace(",", "."), items[i].Price.ToString("F2").Replace(",", "."), items[i].Name, (items[i].Amount).ToString().Replace(",", "."));
            }

            return result;
        }
    }
}