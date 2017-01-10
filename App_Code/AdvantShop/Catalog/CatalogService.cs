//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using AdvantShop.Repository.Currencies;
using Resources;
using AdvantShop.Customers;
using AdvantShop.CMS;
using AdvantShop.Configuration;
namespace AdvantShop.Catalog
{
    public enum ESortOrder
    {
        DescByPopularity = 0,
        DescByAddingDate = 1,
        AscByName = 2,
        DescByName = 3,
        AscByPrice = 4,
        DescByPrice = 5,
        AscByRatio = 6,
        DescByRatio = 7,
        NoSorting = 8,
        AscByPopularity = 9,
        AscByAddingDate = 10
    }


    public class CatalogService
    {
        #region  GetStringPrice

        public static string GetStringPrice(float price, bool isMainPrice = false)
        {
            return GetStringPrice(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, 0, 1, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, null, false, isMainPrice);
        }

        public static string GetStringPrice(float price, bool isWrap, bool isMainPrice = false)
        {
            return GetStringPrice(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, 0, 1, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, null, isWrap, isMainPrice);
        }

        public static string GetStringPrice(float price, float currentCurrencyRate, string currentCurrencyIso3)
        {
            return GetStringPrice(price, 1, currentCurrencyIso3, currentCurrencyRate);
        }

        public static string GetStringPrice(float price, Currency currency)
        {
            return GetStringPrice(price, currency.Value, currency.Symbol, 0, 1, currency.IsCodeBefore, currency.PriceFormat, null, false);
        }

        public static string GetStringPrice(float price, float discount)
        {
            return GetStringPrice(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, discount, 1, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, null, false);
        }

        public static string GetStringPrice(float price, float discount, float amount)
        {
            return GetStringPrice(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, discount, amount, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, null, false);
        }

        public static string GetStringPrice(float price, float discount, float amount, string zeroPriceMsg)
        {
            return GetStringPrice(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, discount, amount, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, zeroPriceMsg, false);
        }

        public static string GetStringPrice(float price, float qty, string currencyCode, float currencyRate)
        {
            Currency cur = CurrencyService.Currency(currencyCode);
            if (cur == null)
                return GetStringPrice(price, currencyRate, currencyCode, 0, qty, false, CurrencyService.DefaultPriceFormat, null, false);
            return GetStringPrice(price, currencyRate, cur.Symbol, 0, qty, cur.IsCodeBefore, cur.PriceFormat, null, false);
        }

        private static string GetStringPrice(float price, float currentCurrencyRate, string currentCurrencyCode, float discount, float amount, bool isCodeBefore, string priceFormat, string zeroPriceMsg, bool isWrap, bool isMainPrice = false)
        {
            if ((price == 0 || amount == 0) && !String.IsNullOrEmpty(zeroPriceMsg))
            {
                return zeroPriceMsg;
            }

            string strPriceRes;
            if (discount == 0)
            {
                strPriceRes = String.IsNullOrEmpty(priceFormat) ? Math.Round((price * amount) / currentCurrencyRate, 2).ToString() : String.Format("{0:" + priceFormat + "}", Math.Round((price * amount) / currentCurrencyRate, 2));
            }
            else
            {
                float dblTemp = (price * amount) / currentCurrencyRate;
                strPriceRes = String.IsNullOrEmpty(priceFormat) ? Math.Round(dblTemp - ((dblTemp / 100) * discount), 2).ToString() : String.Format("{0:" + priceFormat + "}", Math.Round(dblTemp - ((dblTemp / 100) * discount), 2));
            }

            string strCurrencyFormat = isWrap
                                           ? (isCodeBefore
                                                  ? "<span class=\"curr\">{1}</span> <span class=\"price-num\">{0}</span>"
                                                  : "<span class=\"price-num\">{0}</span> <span class=\"curr\">{1}</span>")
                                           : isMainPrice
                                                 ? (isCodeBefore ? "{1} <span class='price'>{0}</span>" : "<span class='price'>{0}</span> {1}")
                                                 : (isCodeBefore ? "{1} {0}" : "{0} {1}");

            return String.Format(strCurrencyFormat, strPriceRes, currentCurrencyCode);
        }

        public static string GetStringDiscountPercent(float price, float discount, bool boolAddMinus)
        {
            return GetStringDiscountPercent(price, discount, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, boolAddMinus);
        }

        public static string GetStringDiscountPercent(float price, float discount, float currentCurrencyRate, string currentCurrencyCode, bool boolAddMinus)
        {
            return GetStringDiscountPercent(price, discount, currentCurrencyRate, currentCurrencyCode, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, boolAddMinus);
        }

        public static string GetStringDiscountPercent(float price, float discount, float currentCurrencyRate, string currentCurrencyCode, bool isCodeBefore, string priceFormat, bool boolAddMinus)
        {
            var strFormat = String.Empty;
            var dblRes = Math.Round(((price / 100) * discount) / currentCurrencyRate, 2);

            string strFormatedPrice = priceFormat == "" ? dblRes.ToString() : String.Format("{0:" + priceFormat + "}", dblRes);

            if (boolAddMinus)
            {
                strFormat = "-";
            }

            if (isCodeBefore)
            {
                strFormat += "{1}{0} ({2}%)";
            }
            else
            {
                strFormat += "{0}{1} ({2}%)";
            }

            return String.Format(strFormat, strFormatedPrice, currentCurrencyCode, discount);
        }

        #endregion

        #region GetStringPrice Inplace

        public static string GetStringPriceInplace(float price, InplaceEditor.Offer.Field field, int offerId)
        {
            return GetStringPriceInplace(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, 0, 1, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, null, field, offerId);
        }

        private static string GetStringPriceInplace(float price, float currentCurrencyRate, string currentCurrencyCode, float discount, float amount, bool isCodeBefore, string priceFormat, string zeroPriceMsg, InplaceEditor.Offer.Field field, int offerId = 0)
        {
            if ((price == 0 || amount == 0) && !String.IsNullOrEmpty(zeroPriceMsg))
            {
                return zeroPriceMsg;
            }

            string strPriceRes;
            if (discount == 0)
            {
                strPriceRes = String.IsNullOrEmpty(priceFormat) ? Math.Round((price * amount) / currentCurrencyRate, 2).ToString() : String.Format("{0:" + priceFormat + "}", Math.Round((price * amount) / currentCurrencyRate, 2));
            }
            else
            {
                float dblTemp = (price * amount) / currentCurrencyRate;
                strPriceRes = String.IsNullOrEmpty(priceFormat) ? Math.Round(dblTemp - ((dblTemp / 100) * discount), 2).ToString() : String.Format("{0:" + priceFormat + "}", Math.Round(dblTemp - ((dblTemp / 100) * discount), 2));
            }

            string strCurrencyFormat = isCodeBefore
                ? "<div class=\"curr\">{1}</div> <div class=\"price-num\" {2}>{0}</div>"
                : "<div class=\"price-num\" {2}>{0}</div> <div class=\"curr\">{1}</div>";

            return String.Format(strCurrencyFormat, strPriceRes, currentCurrencyCode, InplaceEditor.Offer.AttributePriceDetails(offerId, field));
        }

        #endregion

        //todo Vladimir точно нужна эта функция?
        public static float CalculatePrice(float price, float discount)
        {
            return (price - ((price / 100) * discount));
        }

        public static float CalculateProductPrice(float price, float productDiscount, CustomerGroup customerGroup, IList<EvaluatedCustomOptions> customOptions)
        {
            float customOptionPrice = 0;
            if (customOptions != null)
            {
                customOptionPrice = CustomOptionsService.GetCustomOptionPrice(price, customOptions);
            }

            float groupDiscount = customerGroup == null ? 0 : customerGroup.GroupDiscount;
            float finalDiscount = productDiscount > groupDiscount ? productDiscount : groupDiscount;

            return (price + customOptionPrice) * ((100 - finalDiscount) / 100);
        }

        public static string FormatPriceInvariant(object price)
        {
            return String.Format("{0:##,##0.##}", price);
        }

        public static string RenderLabels(bool recommended, bool sales, bool best, bool news, float discount, int labelCount = 5, List<string> customLabels = null, bool warranty = false)
        {
            var labels = new StringBuilder();
            labels.Append("<span class=\"label-p\">");

            if (warranty && labelCount-- > 0)
                labels.AppendFormat("<span class=\"warranty\">{0}</span>", Resource.Client_Catalog_LabelWarranty);
            if (discount > 0 && labelCount-- > 0)
                labels.AppendFormat("<span class=\"disc\">{0} {1}%</span>", Resource.Client_Catalog_Discount, FormatPriceInvariant(discount));
            if (recommended && labelCount-- > 0)
                labels.AppendFormat("<span class=\"recommend\">{0}</span>", Resource.Client_Catalog_LabelRecomend);
            if (sales && labelCount-- > 0)
                labels.AppendFormat("<span class=\"sales\">{0}</span>", Resource.Client_Catalog_LabelSales);
            if (best && labelCount-- > 0)
                labels.AppendFormat("<span class=\"best\">{0}</span>", Resource.Client_Catalog_LabelBest);
            if (news && labelCount > 0)
                labels.AppendFormat("<span class=\"new\">{0}</span>", Resource.Client_Catalog_LabelNew);

            if (customLabels != null)
                foreach (var customLabel in customLabels)
                    labels.Append(customLabel);

            labels.Append("</span>");

            return labels.ToString();
        }

        /// <summary>
        /// Render product price
        /// </summary>
        /// <param name="productPrice">product price</param>
        /// <param name="discount">total discount price</param>
        /// <param name="showDiscount">display discount</param>
        /// <param name="customerGroup">customer group</param>
        /// <param name="customOptions">custom options</param>
        /// <param name="isWrap">currency wrap</param>
        /// <returns></returns>
        public static string RenderPrice(float productPrice, float discount, bool showDiscount, CustomerGroup customerGroup,
                                            string customOptions = null, bool isWrap = false)
        {
            if (productPrice == 0)
            {
                return String.Format("<div class='price-wrap'>{0}</div>", Resource.Client_Catalog_ContactWithUs);
            }

            float priceWithCustomOptions = CalculateProductPrice(productPrice, 0, null, CustomOptionsService.DeserializeFromXml(customOptions));

            var priceWithDiscount = CalculateProductPrice(productPrice, discount, customerGroup, CustomOptionsService.DeserializeFromXml(customOptions));

            if (discount == 0 || !showDiscount)
            {
                return String.Format("<div class='price-wrap'>{0}</div>", GetStringPrice(priceWithDiscount, isWrap));
            }

            var groupDiscount = customerGroup.CustomerGroupId == 0 ? 0 : customerGroup.GroupDiscount;
            var finalDiscount = discount > groupDiscount ? discount : groupDiscount;

            return
                String.Format(
                    "<div class=\"price-old\">{0}</div><div class=\"price-wrap\">{1}</div><div class=\"price-benefit\">{2} {3} {4} {5}% </div>",
                    GetStringPrice(priceWithCustomOptions),
                    GetStringPrice(priceWithDiscount, isMainPrice: true),
                    Resource.Client_Catalog_Discount_Benefit,
                    GetStringPrice(priceWithCustomOptions - priceWithDiscount),
                    Resource.Client_Catalog_Discount_Or,
                    FormatPriceInvariant(finalDiscount));
        }

        /// <summary>
        /// Render price with inplace editor
        /// </summary>
        /// <param name="productPrice">product price</param>
        /// <param name="discount">total discount price</param>
        /// <param name="showDiscount">display discount</param>
        /// <param name="customerGroup">customer group</param>
        /// <param name="customOptions">custom options</param>
        /// <param name="offerId">offer id</param>
        /// <returns></returns>
        public static string RenderPriceInplace(float productPrice, float discount, bool showDiscount,
                                    CustomerGroup customerGroup, string customOptions = null, int offerId = 0)
        {

            if (productPrice == 0)
            {
                return String.Format("<div class=\'price-wrap\' {1}>{0}</div>", Resource.Client_Catalog_ContactWithUs,
                                        InplaceEditor.Offer.AttributePriceDetails(offerId, InplaceEditor.Offer.Field.Price));
            }

            float priceWithCustomOptions = CalculateProductPrice(productPrice, 0, null, CustomOptionsService.DeserializeFromXml(customOptions));

            var priceWithDiscount = CalculateProductPrice(productPrice, discount, customerGroup, CustomOptionsService.DeserializeFromXml(customOptions));

            if (discount == 0 || !showDiscount)
            {
                return String.Format("<div class=\'price-wrap inplace-indicator-offset {1}\'>{0}</div>",
                    GetStringPriceInplace(priceWithDiscount, InplaceEditor.Offer.Field.Price, offerId),
                    !SettingsMain.EnableInplace ? "inplace-indicator-offset-off" : "");
            }

            var groupDiscount = customerGroup.CustomerGroupId == 0 ? 0 : customerGroup.GroupDiscount;
            var finalDiscount = Math.Max(discount, groupDiscount);

            return
                String.Format(
                    "<div class=\"price-old inplace-indicator-offset\">{0}</div><div class=\"price-wrap\">{1}</div><div class=\"price-benefit\">{2} {3} {4} {5}% </div>",
                    GetStringPriceInplace(priceWithCustomOptions, InplaceEditor.Offer.Field.Price, offerId),
                    GetStringPrice(priceWithDiscount, isMainPrice: true),
                    Resource.Client_Catalog_Discount_Benefit,
                    GetStringPrice(priceWithCustomOptions - priceWithDiscount),
                    Resource.Client_Catalog_Discount_Or,
                    FormatPriceInvariant(finalDiscount));
        }

        public static string RenderBonusPrice(float bonusPercent, float productPrice, float discount,
                                                CustomerGroup customerGroup, string customOptions = null)
        {
            var priceWithDiscount = CalculateProductPrice(productPrice, discount, customerGroup, CustomOptionsService.DeserializeFromXml(customOptions));

            if (productPrice <= 0 || bonusPercent <= 0 || priceWithDiscount <= 0)
            {
                return string.Empty;
            }

            return String.Format("<div class=\"bonus-price\">" + Resource.Client_Bonuses_BonusesOnCard + "</div>",
                                    GetStringPrice(priceWithDiscount * bonusPercent / 100));
        }

        public static string RenderSelectedOptions(string xml)
        {
            if (String.IsNullOrEmpty(xml))
                return String.Empty;

            var result = new StringBuilder("<div class=\"customoptions\">");

            foreach (var item in CustomOptionsService.DeserializeFromXml(xml))
            {
                result.Append(item.CustomOptionTitle + ": " + item.OptionTitle);
                if (item.OptionPriceBc != 0)
                {
                    result.Append(" ");
                    if (item.OptionPriceBc > 0)
                        result.Append("+" +
                                      (item.OptionPriceType == OptionPriceType.Fixed
                                          ? GetStringPrice(item.OptionPriceBc)
                                          : item.OptionPriceBc + " %"));
                }
                result.Append("<br />");
            }

            result.Append("</div>");

            return result.ToString();
        }
    }
}