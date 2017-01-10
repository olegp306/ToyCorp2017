using System;
using System.IO;
using System.Net;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.BonusSystem
{
    public class BonusSystemService
    {
        #region Fields

        private const string BonusSystemUrl = "https://my.active-client.ru/rest/";
        private const string BonusCardCacheKey = "BonusSystem.BonusCard_";
        private const string BonusFirstPercentCacheKey = "BonusSystem.BonusFirstPercent";

        #endregion

        #region Private methods

        private static T MakeRequest<T>(string url, string data = null, string method = "POST", string contentType = "text/json") where T : class
        {
            try
            {
                var request = WebRequest.Create(BonusSystemUrl + url) as HttpWebRequest;
                request.Method = method;
                request.Headers["Authentication"] = BonusSystem.ApiKey;
                request.ContentType = contentType;

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

                return JsonConvert.DeserializeObject<T>(responseContent);
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
                                    Debug.LogError(error);
                                }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return null;
        }

        #endregion

        #region Bonus card api methods

        public static BonusCard GetCard(long? cardId)
        {
            if (cardId == null)
                return null;

            return GetCard((long)cardId);
        }

        public static BonusCard GetCard(long cardId)
        {
            var cacheKey = BonusCardCacheKey + cardId;

            if (CacheManager.Contains(cacheKey))
                return CacheManager.Get<BonusCard>(cacheKey);

            var cardResponse = MakeRequest<BonusCardResponse>("api/cards/" + cardId, method: "GET");

            if (cardResponse != null && cardResponse.Status == 200)
            {
                CacheManager.Insert(cacheKey, cardResponse.Data, 1);
                return cardResponse.Data;
            }
            return null;
        }

        public static BonusCard GetCardByPhone(string phone)
        {
            if (phone.IsNullOrEmpty())
                return null;

            var cardResponse = MakeRequest<BonusCardResponse>("api/cards/" + phone + "/byphone", method: "GET");

            if (cardResponse != null && cardResponse.Status == 200)
            {
                return cardResponse.Data;
            }

            return null;
        }

        public static BonusCardResponse AddCard(BonusCard card)
        {
            var cardResponse = MakeRequest<BonusCardResponse>("api/cards/create", JsonConvert.SerializeObject(card));
            return cardResponse;
        }

        public static BonusCardResponse UpdateCard(BonusCard card)
        {
            CacheManager.RemoveByPattern(BonusCardCacheKey);

            var cardResponse = MakeRequest<BonusCardResponse>("api/cards/", JsonConvert.SerializeObject(card), "PUT");
            return cardResponse;
        }

        /// <summary>
        /// Списание бонусов
        /// </summary>
        /// <param name="cardNumber">Номер карты</param>
        /// <param name="sum">Сумма, из которой расчитывались бонусы</param>
        /// <param name="bonuses">Сколько бонусов списать</param>
        /// <param name="orderNumber">Номер заказа</param>
        /// <param name="orderId"></param>
        public static void MakeBonusPurchase(long cardNumber, float sum, float bonuses, string orderNumber, int orderId)
        {
            CacheManager.RemoveByPattern(BonusCardCacheKey);

            var data = new
            {
                CardNumber = cardNumber,
                PurchaseAmount = sum.ToString().Replace(",", "."),
                BonusAmount = bonuses.ToString().Replace(",", "."),
                Comment = "Заказ № " + (BonusSystem.UseOrderId ? orderId.ToString() : orderNumber) + " в магазине " + SettingsMain.SiteUrlPlain,
                DocumentId = orderNumber
            };

            MakeRequest<BonusPurchaseResponse>("api/purchases", JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Запрос смс кода по номеру карты
        /// </summary>
        /// <param name="cardNumber">Номер карты</param>
        /// <returns></returns>
        public static BonusSmsCodeResponse GetSmsCode(long cardNumber)
        {
            return MakeRequest<BonusSmsCodeResponse>("api/cards/" + cardNumber + "/smscodebycardnum", method: "GET");
        }

        /// <summary>
        /// Запрос смс кода по телефону
        /// </summary>
        /// <param name="phone">Номер телефона</param>
        /// <returns></returns>
        public static BonusSmsCodeResponse GetSmsCode(string phone)
        {
            return MakeRequest<BonusSmsCodeResponse>("api/cards/" + phone + "/smscodebyphone", method: "GET");
        }

        /// <summary>
        /// Проверка занят ли телефон
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsPhoneExist(string phone)
        {
            var response = MakeRequest<BonusPhoneExistResponse>("api/cards/" + phone + "/exist", method: "GET");

            if (response != null && response.Status == 200 && response.Data != null)
                return Convert.ToBoolean(response.Data.Exist);

            return false;
        }

        /// <summary>
        /// Подтверждаем, что заказ оплачен
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool Confirm(string orderNumber, int orderId)
        {
            CacheManager.RemoveByPattern(BonusCardCacheKey);

            var response = MakeRequest<BonusConfirmResponse>("api/purchases/confirm/" +  (BonusSystem.UseOrderId ? orderId.ToString() : orderNumber) , method: "GET");
            return response != null && response.Status == 200 && response.Data == orderNumber;
        }

        /// <summary>
        /// Получить продажу
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static BonusPurchase GetPurchase(string orderNumber, int orderId)
        {
            var response = MakeRequest<BonusPurchaseResponse>("api/purchases/" + (BonusSystem.UseOrderId ? orderId.ToString() : orderNumber), method: "GET");

            return response != null && response.Status == 200 ? response.Data : null;
        }

        /// <summary>
        /// Отмена продажи
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static void CancelPurchase(string orderNumber, int orderId)
        {
            MakeRequest<BonusPurchaseResponse>(
                "api/purchases/" + (BonusSystem.UseOrderId ? orderId.ToString() : orderNumber), 
                JsonConvert.SerializeObject(new { Comment = "Отмена заказа № " + (BonusSystem.UseOrderId ? orderId.ToString() : orderNumber) + " в магазине " + SettingsMain.SiteUrlPlain }), 
                method: "DELETE");
        }

        /// <summary>
        /// Обновление продажи при редатировании заказа
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="sum"></param>
        /// <param name="bonuses"></param>
        /// <param name="orderNumber"></param>
        /// <param name="orderId"></param>
        public static void UpdatePurchase(long cardNumber, float sum, float bonuses, string orderNumber, int orderId)
        {
            CacheManager.RemoveByPattern(BonusCardCacheKey);

            var orderIdentifier = BonusSystem.UseOrderId ? orderId.ToString() : orderNumber;

            var data = new
            {
                CardNumber = cardNumber,
                PurchaseAmount = sum.ToString().Replace(",", "."),
                BonusAmount = bonuses.ToString().Replace(",", "."),
                Comment = "Изменение заказа № " + orderIdentifier + " в магазине " + SettingsMain.SiteUrlPlain,
                DocumentId = orderIdentifier
            };

            MakeRequest<BonusPurchaseResponse>("api/purchases/" + orderIdentifier, JsonConvert.SerializeObject(data), "PUT");
        }

        /// <summary>
        /// Процент бонусов по умолчанию
        /// </summary>
        /// <returns></returns>
        public static float GetBonusDefaultPercent()
        {
            if (CacheManager.Contains(BonusFirstPercentCacheKey))
                return CacheManager.Get<float>(BonusFirstPercentCacheKey);

            var response = MakeRequest<BonusGradeResponse>("api/grades/defaultgrade", method: "GET");

            float percent = response != null ? response.Data : 0;

            CacheManager.Insert(BonusFirstPercentCacheKey, percent);
            return percent;
        }


        public static bool IsActive()
        {
            var cardResponse = MakeRequest<BonusCardResponse>("api/cards/1", method: "GET");
            return cardResponse != null;
        }

        #endregion

        /// <summary>
        /// Расчет стоимости бонуса
        /// </summary>
        /// <param name="totaOrderlPrice">Стоимость товаров со скидками и доставкой</param>
        /// <param name="productsPrice">Стоимость товаров со скидками </param>
        /// <param name="bonusAmount">Бонусы</param>
        /// <returns></returns>
        public static float GetBonusCost(float totaOrderlPrice, float productsPrice, float bonusAmount)
        {
            var sumPrice = BonusSystem.BonusType == BonusSystem.EBonusType.ByProductsCostWithShipping
                    ? totaOrderlPrice
                    : productsPrice;

            var bonusPrice = sumPrice > bonusAmount ? bonusAmount : sumPrice;

            if (BonusSystem.MaxOrderPercent == 100 || (bonusPrice * 100 / sumPrice) <= BonusSystem.MaxOrderPercent)
                return bonusPrice;

            return sumPrice * BonusSystem.MaxOrderPercent / 100;
        }

        /// <summary>
        /// Расчет стоимости бонусов, которые будут начислены на карту
        /// </summary>
        /// <param name="totalPrice"></param>
        /// <param name="productsPriceWhitDiscount"></param>
        /// <param name="bonusPercent"></param>
        /// <returns></returns>
        public static float GetBonusPlusCost(float totalPrice, float productsPriceWhitDiscount, float bonusPercent)
        {
            var sumPrice = BonusSystem.BonusType == BonusSystem.EBonusType.ByProductsCostWithShipping
                    ? totalPrice
                    : productsPriceWhitDiscount;

            return sumPrice * bonusPercent / 100;
        }
    }
}