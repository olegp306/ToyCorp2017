//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Linq;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Configuration
{
    public class SettingsCatalog
    {

        public enum ProductViewMode
        {
            Tiles = 0,
            List = 1,
            Table = 2
        }

        public enum ProductViewPage
        {
            Catalog = 0,
            Search = 1
        }

        public static int ProductsPerPage
        {
            get { return int.Parse(SettingProvider.Items["ProductsPerPage"]); }
            set { SettingProvider.Items["ProductsPerPage"] = value.ToString(); }
        }

        public static string DefaultCurrencyIso3
        {
            get
            {
                return SettingProvider.Items["DefaultCurrencyISO3"];
                //(CurrencyService.Currency(SQLDataHelper.GetString(SettingProvider.Items["DefaultCurrencyISO3"])) ??
                // CurrencyService.GetAllCurrencies().FirstOrDefault()).Iso3;
            }
            set { SettingProvider.Items["DefaultCurrencyISO3"] = value; }
        }

        public static bool AllowToChangeCurrency
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["AllowToChangeCurrency"]); }
            set { SettingProvider.Items["AllowToChangeCurrency"] = value.ToString(); }
        }


        public static ProductViewMode DefaultCatalogView
        {
            get { return (ProductViewMode)int.Parse(SettingProvider.Items["DefaultCatalogView"]); }
            set { SettingProvider.Items["DefaultCatalogView"] = ((int)value).ToString(); }
        }

        public static ProductViewMode DefaultSearchView
        {
            get { return (ProductViewMode)int.Parse(SettingProvider.Items["DefaultSearchView"]); }
            set { SettingProvider.Items["DefaultSearchView"] = ((int)value).ToString(); }
        }

        public static bool EnableProductRating
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnableProductRating"]); }
            set { SettingProvider.Items["EnableProductRating"] = value.ToString(); }
        }

        public static bool EnablePhotoPreviews
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnablePhotoPreviews"]); }
            set { SettingProvider.Items["EnablePhotoPreviews"] = value.ToString(); }
        }


        public static bool ShowProductsCount
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ShowProductsCount"]); }
            set { SettingProvider.Items["ShowProductsCount"] = value.ToString(); }
        }

        public static bool EnableCompareProducts
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnableCompareProducts"]); }
            set { SettingProvider.Items["EnableCompareProducts"] = value.ToString(); }
        }



        public static bool ShowProductsWithZeroAmount
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ShowProductsWithZeroAmount"]); }
            set { SettingProvider.Items["ShowProductsWithZeroAmount"] = value.ToString(); }
        }

        public static bool EnabledCatalogViewChange
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnableCatalogViewChange"]); }
            set { SettingProvider.Items["EnableCatalogViewChange"] = value.ToString(); }
        }

        public static bool EnabledSearchViewChange
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnableSearchViewChange"]); }
            set { SettingProvider.Items["EnableSearchViewChange"] = value.ToString(); }
        }

        public static bool CompressBigImage
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["CompressBigImage"]); }
            set { SettingProvider.Items["CompressBigImage"] = value.ToString(); }
        }

        public static string RelatedProductName
        {
            get { return SettingProvider.Items["RelatedProductName"]; }
            set { SettingProvider.Items["RelatedProductName"] = value; }
        }

        public static string AlternativeProductName
        {
            get { return SettingProvider.Items["AlternativeProductName"]; }
            set { SettingProvider.Items["AlternativeProductName"] = value; }
        }

        public static bool AllowReviews
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["AllowReviews"]); }
            set { SettingProvider.Items["AllowReviews"] = value.ToString(); }
        }

        public static bool ModerateReviews
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ModerateReviewed"]); }
            set { SettingProvider.Items["ModerateReviewed"] = value.ToString(); }
        }

        public static bool ComplexFilter
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ComplexFilter"]); }
            set { SettingProvider.Items["ComplexFilter"] = value.ToString(); }
        }

        public static string SizesHeader
        {
            get { return SQLDataHelper.GetString(SettingProvider.Items["SizesHeader"]); }
            set { SettingProvider.Items["SizesHeader"] = value; }
        }


        public static string ColorsHeader
        {
            get { return SQLDataHelper.GetString(SettingProvider.Items["ColorsHeader"]); }
            set { SettingProvider.Items["ColorsHeader"] = value; }
        }


        public static bool AvaliableFilterEnabled
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["AvaliableFilterEnabled"]); }
            set { SettingProvider.Items["AvaliableFilterEnabled"] = value.ToString(); }
        }


        public static bool AvaliableFilterSelected
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["AvaliableFilterSelected"]); }
            set { SettingProvider.Items["AvaliableFilterSelected"] = value.ToString(); }
        }

        public static bool PreorderFilterEnabled
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["PreorderFilterEnabled"]); }
            set { SettingProvider.Items["PreorderFilterEnabled"] = value.ToString(); }
        }

        public static bool PreorderFilterSelected
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["PreorderFilterSelected"]); }
            set { SettingProvider.Items["PreorderFilterSelected"] = value.ToString(); }
        }
        public static bool ExluderingFilters
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ExluderingFilters"]); }
            set { SettingProvider.Items["ExluderingFilters"] = value.ToString(); }
        }

        public static string GetRelatedProductName(int relatedType)
        {
            return (relatedType == 0) ? RelatedProductName : AlternativeProductName;
        }


        public static string BuyButtonText
        {
            get { return SettingProvider.Items["BuyButtonText"]; }
            set { SettingProvider.Items["BuyButtonText"] = value; }
        }

        public static string MoreButtonText
        {
            get { return SettingProvider.Items["MoreButtonText"]; }
            set { SettingProvider.Items["MoreButtonText"] = value; }
        }

        public static string PreOrderButtonText
        {
            get { return SettingProvider.Items["PreOrderButtonText"]; }
            set { SettingProvider.Items["PreOrderButtonText"] = value; }
        }

        public static bool DisplayBuyButton
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["DisplayBuyButton"]); }
            set { SettingProvider.Items["DisplayBuyButton"] = value.ToString(); }
        }

        public static bool DisplayMoreButton
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["DisplayMoreButton"]); }
            set { SettingProvider.Items["DisplayMoreButton"] = value.ToString(); }
        }

        public static bool DisplayPreOrderButton
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["DisplayPreOrderButton"]); }
            set { SettingProvider.Items["DisplayPreOrderButton"] = value.ToString(); }
        }

        public static bool DisplayCategoriesInBottomMenu
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["DisplayCategoriesInBottomMenu"]); }
            set { SettingProvider.Items["DisplayCategoriesInBottomMenu"] = value.ToString(); }
        }

        public static bool ShowStockAvailability
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ShowStockAvailability"]); }
            set { SettingProvider.Items["ShowStockAvailability"] = value.ToString(); }
        }

        public static bool ShowColorFilter
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ShowColorFilter"]); }
            set { SettingProvider.Items["ShowColorFilter"] = value.ToString(); }
        }

        public static bool ShowSizeFilter
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ShowSizeFilter"]); }
            set { SettingProvider.Items["ShowSizeFilter"] = value.ToString(); }
        }

        public static bool DisplayWeight
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["DisplayWeight"]); }
            set { SettingProvider.Items["DisplayWeight"] = value.ToString(); }
        }

        public static bool DisplayDimensions
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["DisplayDimensions"]); }
            set { SettingProvider.Items["DisplayDimensions"] = value.ToString(); }
        }

        public static bool ShowProducerFilter
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ShowProducerFilter"]); }
            set { SettingProvider.Items["ShowProducerFilter"] = value.ToString(); }
        }

        public static bool ShowPriceFilter
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ShowPriceFilter"]); }
            set { SettingProvider.Items["ShowPriceFilter"] = value.ToString(); }
        }
    }
}