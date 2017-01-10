//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.SQL2;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.SEO;
using Newtonsoft.Json;
using Resources;

namespace ClientPages
{
    public partial class ProductList_Page : AdvantShopClientPage
    {
        protected ProductOnMain.TypeFlag _typeFlag = ProductOnMain.TypeFlag.None;
        protected string PageName;
        protected int ProductsCount;

        private SqlPaging _paging;

        protected void Page_Load(object sender, EventArgs e)
        {
            _paging = new SqlPaging();
            _paging.From("[Catalog].[Product]");
            _paging.Inner_Join("[Catalog].[ProductExt]  ON [Product].[ProductID] = [ProductExt].[ProductID]");
            _paging.Left_Join("[Catalog].[Photo]  ON [Photo].[PhotoId] = [ProductExt].[PhotoId]");
            _paging.Left_Join("[Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]");

            _paging.Left_Join("[Catalog].[ShoppingCart] ON [ShoppingCart].[OfferID] = [Offer].[OfferID] AND [ShoppingCart].[ShoppingCartType] = 3 AND [ShoppingCart].[CustomerID] = {0}", CustomerContext.CustomerId);
            _paging.Left_Join("[Catalog].[Ratio] on Product.ProductId= Ratio.ProductID and Ratio.CustomerId=[ShoppingCart].[CustomerID]");


            _paging.Select(
                "[Product].[ProductID]",
                "CountPhoto",
                "Photo.PhotoId",
                "PhotoName AS Photo",
                "Photo.Description as PhotoDesc",
                "BriefDescription",
                "Product.ArtNo",
                "Name",
                "Recomended",
                "Bestseller",
                "New",
                "OnSale",
                "[Product].Discount",
                "Offer.OfferID",
                "MaxAvailable AS Amount",
                "MinAmount",
                "MaxAmount",
                "Enabled",
                "AllowPreOrder",
                "Ratio",
                "RatioID",
                "ShoppingCartItemId",
                "UrlPath",
                "Offer.ColorID",
                "null as AdditionalPhoto"
                );

            if (SettingsCatalog.ComplexFilter)
            {
                _paging.Select(
                    "Colors",
                    "NotSamePrices as MultiPrices",
                    "MinPrice as Price"
                    );
            }
            else
            {
                _paging.Select(
                    "null as Colors",
                    "0 as MultiPrices",
                    "Price"
                    );
            }

            if (string.IsNullOrEmpty(Request["type"]) || !Enum.TryParse(Request["type"], true, out _typeFlag) ||
                _typeFlag == ProductOnMain.TypeFlag.None)
            {
                Error404();
            }

            switch (_typeFlag)
            {
                case ProductOnMain.TypeFlag.Bestseller:
                    PageName = Resource.Client_ProductList_AllBestSellers;
                    SetMeta(new MetaInfo(SettingsMain.ShopName + " - " + Resource.Client_Bestsellers_Header), string.Empty);
                    break;
                case ProductOnMain.TypeFlag.New:
                    PageName = Resource.Client_ProductList_AllNew;
                    SetMeta(new MetaInfo(SettingsMain.ShopName + " - " + Resource.Client_New_Header), string.Empty);
                    break;
                case ProductOnMain.TypeFlag.Discount:
                    PageName = Resource.Client_ProductList_AllDiscount;
                    SetMeta(new MetaInfo(SettingsMain.ShopName + " - " + Resource.Client_Discount_Header), string.Empty);
                    break;
                case ProductOnMain.TypeFlag.OnSale:
                    PageName = Resource.Client_ProductList_OnSale;
                    SetMeta(new MetaInfo(SettingsMain.ShopName + " - " + Resource.Client_OnSale_Header), string.Empty);
                    break;
                case ProductOnMain.TypeFlag.Recomended:
                    PageName = Resource.Client_ProductList_Recomended;
                    SetMeta(new MetaInfo(SettingsMain.ShopName + " - " + Resource.Client_Recomended_Header), string.Empty);
                    break;
            }

            ProductsCount = ProductOnMain.GetProductCountByType(_typeFlag);
            pnlSort.Visible = ProductsCount > 0;
            productView.Visible = ProductsCount > 0;

            breadCrumbs.Items.AddRange(new BreadCrumbs[]
                {
                    new BreadCrumbs()
                        {
                            Name = Resource.Client_MasterPage_MainPage,
                            Url = UrlService.GetAbsoluteLink("/")
                        },
                    new BreadCrumbs() {Name = PageName, Url = null}
                });

            BuildSorting();
            BuildFilter();
        }

        private void BuildSorting()
        {
            var sort = ESortOrder.NoSorting;
            
            if (!string.IsNullOrEmpty(Request["sort"]))
            {
                Enum.TryParse(Request["sort"], true, out sort);
            }
            
            foreach (ESortOrder enumItem in Enum.GetValues(typeof(ESortOrder)))
            {
                ddlSort.Items.Add(new ListItem
                {
                    Text = enumItem.GetLocalizedName(),
                    Value = enumItem.ToString(),
                    Selected = sort == enumItem
                });
            }

            switch (sort)
            {
                case ESortOrder.AscByAddingDate:
                    _paging.OrderBy("DateAdded as DateAddedSort ASC");
                    break;
                case ESortOrder.DescByAddingDate:
                    _paging.OrderBy("DateAdded as DateAddedSort DESC");
                    break;

                case ESortOrder.AscByName:
                    _paging.OrderBy("Name as NameSort ASC");
                    break;

                case ESortOrder.DescByName:
                    _paging.OrderBy("Name as NameSort DESC");
                    break;

                case ESortOrder.AscByPrice:
                    _paging.OrderBy("[Offer].Price - [Offer].Price * [Product].Discount / 100 as PriceTemp ASC");
                    break;

                case ESortOrder.DescByPrice:
                    _paging.OrderBy("[Offer].Price - [Offer].Price * [Product].Discount / 100 as PriceTemp DESC");
                    break;

                case ESortOrder.AscByRatio:
                    _paging.OrderBy("Ratio as RatioSort ASC");
                    break;

                case ESortOrder.DescByRatio:
                    _paging.OrderBy("Ratio as RatioSort DESC");
                    break;

                case ESortOrder.AscByPopularity:
                    _paging.OrderBy("Popularity as PopularitySort ASC");
                    break;
                case ESortOrder.DescByPopularity:
                    _paging.OrderBy("Popularity as PopularitySort DESC");
                    break;

                default:
                    switch (_typeFlag)
                    {
                        case ProductOnMain.TypeFlag.Bestseller:
                            _paging.OrderBy("SortBestseller as Sort ASC");
                            break;
                        case ProductOnMain.TypeFlag.New:
                            _paging.OrderBy("SortNew as Sort ASC, DateModified DESC");
                            break;
                        case ProductOnMain.TypeFlag.Discount:
                            _paging.OrderBy("SortDiscount as Sort ASC");
                            break;
                        case ProductOnMain.TypeFlag.OnSale:
                            _paging.OrderBy("DateModified DESC");
                            break;
                        case ProductOnMain.TypeFlag.Recomended:
                            _paging.OrderBy("SortRecomended as Sort ASC, DateModified DESC");
                            break;
                    }
                    break;
            }
        }

        private void BuildFilter()
        {
            _paging.Where("Enabled={0}", true);
            _paging.Where("AND CategoryEnabled={0}", true);
            _paging.Where("AND Offer.Main={0} AND Offer.Main IS NOT NULL", true);

            switch (_typeFlag)
            {
                case ProductOnMain.TypeFlag.Bestseller:
                    _paging.Where("AND Bestseller={0}", true);
                    break;
                case ProductOnMain.TypeFlag.New:
                    _paging.Where("AND New={0}", true);
                    break;
                case ProductOnMain.TypeFlag.Discount:
                    _paging.Where("AND Discount > {0}", 0);
                    break;
                case ProductOnMain.TypeFlag.OnSale:
                    _paging.Where("AND OnSale={0}", true);
                    break;
                case ProductOnMain.TypeFlag.Recomended:
                    _paging.Where("AND Recomended={0}", true);
                    break;
            }
            
            filterPrice.CategoryId = 0;
            filterPrice.InDepth = true;
            if (!string.IsNullOrEmpty(Request["pricefrom"]) || !string.IsNullOrEmpty(Request["priceto"]))
            {
                var pricefrom = Request["pricefrom"].TryParseInt(0);
                var priceto = Request["priceto"].TryParseInt(int.MaxValue);

                filterPrice.CurValMin = pricefrom;
                filterPrice.CurValMax = priceto;
                _paging.Where("and Exists( select 1 from [Catalog].[Offer] where Offer.Price - Offer.Price * Discount / 100 >= {0} ", pricefrom * CurrencyService.CurrentCurrency.Value);
                _paging.Where("AND Offer.Price - Offer.Price * Discount / 100 <={0} and Offer.ProductId = [Product].[ProductID])", priceto * CurrencyService.CurrentCurrency.Value);
            }
            else
            {
                filterPrice.CurValMin = 0;
                filterPrice.CurValMax = int.MaxValue;
            }
            
            filterBrand.CategoryId = 0;
            filterBrand.InDepth = true;
            filterBrand.WorkType = _typeFlag;
            if (!string.IsNullOrEmpty(Request["brand"]))
            {
                var brandIds = Request["brand"].Split(',').Select(item => item.TryParseInt()).Where(id => id != 0).ToList();
                filterBrand.SelectedBrandIDs = brandIds;
                _paging.Where("AND BrandID IN ({0})", brandIds.ToArray());
            }
            else
            {
                filterBrand.SelectedBrandIDs = new List<int>();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            _paging.CurrentPageIndex = paging.CurrentPage != 0 ? paging.CurrentPage : 1;
            _paging.ItemsPerPage = paging.CurrentPage != 0 ? SettingsCatalog.ProductsPerPage : int.MaxValue;
            var totalCount = _paging.TotalRowsCount;
            paging.TotalPages = _paging.PageCount(totalCount);

            if (totalCount != 0 && paging.TotalPages < paging.CurrentPage || paging.CurrentPage < 0)
            {
                Error404();
                return;
            }

            if (Request["ajax"] == "1")
            {
                Response.Clear();
                Response.ContentType = "application/json";

                var prices =
                    _paging.GetCustomData(
                        "min(Price - Price * discount/100) as PriceFrom, max(Price - Price * discount/100) as PriceTo",
                        string.Empty,
                        reader => new
                        {
                            From = SQLDataHelper.GetFloat(reader, "PriceFrom"),
                            To = SQLDataHelper.GetFloat(reader, "PriceTo"),
                        }, false).First();

                var res = JsonConvert.SerializeObject(new
                {
                    ProductsCount = totalCount,
                    AvaliblePriceFrom = Math.Floor(prices.From / CurrencyService.CurrentCurrency.Value),
                    AvaliblePriceTo = Math.Ceiling(prices.To / CurrencyService.CurrentCurrency.Value),
                    AvalibleBrands = filterBrand.AvalibleBrandIDs,
                });
                Response.Write(res);
                Response.End();
                return;
            }

            var data = _paging.PageItems;
            productView.DataSource = data;
            productView.DataBind();

            bool exluderingFilters = SettingsCatalog.ExluderingFilters;
            filterBrand.AvalibleBrandIDs = exluderingFilters
                ? _paging.GetCustomData("BrandID", " AND BrandID is not null", reader => SQLDataHelper.GetInt(reader, "BrandID"), true)
                : null;
        }
    }
}