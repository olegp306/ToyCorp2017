//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Data;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.SEO;
using AdvantShop.Statistic;
using Newtonsoft.Json;
using Resources;
using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace ClientPages
{
    public partial class Search : AdvantShopClientPage
    {
        protected string SearchTerm = string.Empty;
        private ESortOrder _sort = ESortOrder.NoSorting;

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

            // Для сотрировки по популярности
            _paging.Left_Join(@"(Select [Order].[OrderItems].[ProductId], COUNT([Order].[OrderItems].[ProductId])as Popularity  From [Order].[OrderItems],[Catalog].[Product]
            where [Order].[OrderItems].[ProductId]=[Catalog].[Product].[ProductId]
            Group by  [Order].[OrderItems].[ProductId])as PopTable
            ON PopTable.ProductId=[Catalog].[Product].[ProductId]");
            
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

            BuildSorting();
            BuildFilter();

            var nmeta = new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_Search_AdvancedSearch));
            SetMeta(nmeta, string.Empty, page: paging.CurrentPage);
            txtName.Focus();
        }

        private void BuildSorting()
        {
            if (!string.IsNullOrEmpty(Request["sort"]))
            {
                Enum.TryParse(Request["sort"], true, out _sort);
            }


            foreach (ESortOrder enumItem in Enum.GetValues(typeof(ESortOrder)))
            {
                ddlSort.Items.Add(new ListItem
                    {
                        Text = enumItem.GetLocalizedName(),
                        Value = enumItem.ToString(),
                        Selected = _sort == enumItem
                    });
            }

            switch (_sort)
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
            }
        }

        private void BuildFilter()
        {
            _paging.Where("Enabled={0}", true);
            _paging.Where("AND CategoryEnabled={0}", true);

            foreach (var c in (CategoryService.GetChildCategoriesByCategoryId(0, true).Where(p => p.Enabled)))
            {
                ddlCategory.Items.Add(new ListItem
                {
                    Text = c.Name,
                    Value = c.CategoryId.ToString(),
                });
            }

            var listItem = ddlCategory.Items.FindByValue(Request["category"]);
            if (listItem != null)
            {
                ddlCategory.SelectedValue = listItem.Value;
            }

            _paging.Where("AND Exists( select 1 from [Catalog].[ProductCategories] INNER JOIN [Settings].[GetChildCategoryByParent]({0}) AS hCat ON hCat.id = [ProductCategories].[CategoryID] and  ProductCategories.ProductId = [Product].[ProductID])", ddlCategory.SelectedValue);



            if (!string.IsNullOrEmpty(Page.Request["name"]))
            {
                var name = HttpUtility.UrlDecode(Page.Request["name"]).Trim();
                txtName.Text = name;
                var productIds = LuceneSearch.Search(txtName.Text).AggregateString('/');
                _paging.Inner_Join("(select item, sort from [Settings].[ParsingBySeperator]({0},'/') ) as dtt on Product.ProductId=convert(int, dtt.item)", productIds);

                if (_sort == ESortOrder.NoSorting)
                {
                    _paging.OrderBy("dtt.sort ASC");
                }

                SearchTerm = HttpUtility.HtmlEncode(name);
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
        }

        protected void Page_Prerender(object sender, EventArgs e)
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

            // if we get request from ajax filter
            if (Request["ajax"] == "1")
            {
                var prices =
                    _paging.GetCustomData(
                        "min(Price - Price * discount/100) as PriceFrom, max(Price - Price * discount/100) as PriceTo",
                        string.Empty,
                        reader => new
                            {
                                From = SQLDataHelper.GetFloat(reader, "PriceFrom"),
                                To = SQLDataHelper.GetFloat(reader, "PriceTo")
                            }, false
                        ).First();
                Response.Clear();
                Response.ContentType = "application/json";

                var res = JsonConvert.SerializeObject(new
                    {
                        ProductsCount = totalCount,
                        AvaliblePriceFrom = Math.Floor(prices.From / CurrencyService.CurrentCurrency.Value),
                        AvaliblePriceTo = Math.Ceiling(prices.To / CurrencyService.CurrentCurrency.Value),
                    });
                Response.Write(res);
                Response.End();
                return;
            }

            if (!string.IsNullOrEmpty(Page.Request["name"]) && string.IsNullOrEmpty(Page.Request["ignorelog"]))
            {
                var url = Page.Request.Url.ToString();
                url = url.Substring(url.LastIndexOf("/"), url.Length - url.LastIndexOf("/"));
                StatisticService.AddSearchStatistic(
                    url,
                    Page.Request["name"],
                    string.Format(Resource.Client_Search_SearchIn,
                                  ddlCategory.SelectedItem.Text,
                                  filterPrice.CurValMin,
                                  string.IsNullOrEmpty(Request["priceto"]) ? "∞" : filterPrice.CurValMax.ToString()),
                    totalCount,
                    CustomerContext.CurrentCustomer.Id);
            }

            //filterPrice.Min = prices.Key / CurrencyService.CurrentCurrency.Value;
            //filterPrice.Max = prices.Value / CurrencyService.CurrentCurrency.Value;

            var data = _paging.PageItems;
            vProducts.DataSource = data;
            vProducts.ViewMode = productViewChanger.SearchViewMode;
            vProducts.DataBind();

            int itemsCount = totalCount;
            lItemsCount.Text = string.Format("{0} {1}", itemsCount,
                                             Strings.Numerals(itemsCount, Resource.Client_Searsh_NoResults,
                                                              Resource.Client_Searsh_1Result,
                                                              Resource.Client_Searsh_2Results,
                                                              Resource.Client_Searsh_5Results));
            pnlSort.Visible = itemsCount > 0;

            if (GoogleTagManager.Enabled)
            {
                var tagManager = ((AdvantShopMasterPage)Master).TagManager;
                tagManager.PageType = GoogleTagManager.ePageType.searchresults;
                tagManager.ProdIds = new List<string>();
                foreach (DataRow row in data.Rows)
                {
                    tagManager.ProdIds.Add((string)row["ArtNo"]);
                }
            }
        }
    }
}