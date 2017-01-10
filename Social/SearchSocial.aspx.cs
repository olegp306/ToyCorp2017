//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.FullSearch;
using AdvantShop.Repository.Currencies;

namespace Social
{
    public partial class SearchPage : AdvantShopClientPage
    {
        protected string SearchTerm = string.Empty;
        ESortOrder _sort = ESortOrder.NoSorting;

        private SqlPaging _paging;


        protected void Page_Load(object sender, EventArgs e)
        {
            _paging = new SqlPaging
                {
                    TableName =
                        "[Catalog].[Product] LEFT JOIN [Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] AND [Offer].[Main] = 1 LEFT JOIN [Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type='Product' AND Photo.[Main] = 1 inner join Catalog.ProductCategories on ProductCategories.ProductId = [Product].[ProductID] Left JOIN [Catalog].[ProductPropertyValue] ON [Product].[ProductID] = [ProductPropertyValue].[ProductID] LEFT JOIN [Catalog].[ShoppingCart] ON [Catalog].[ShoppingCart].[OfferId] = [Catalog].[Offer].[OfferID] AND [Catalog].[ShoppingCart].[ShoppingCartType] = 3 AND [ShoppingCart].[CustomerID] = @CustomerId Left JOIN [Catalog].[Ratio] on Product.ProductId= Ratio.ProductID and Ratio.CustomerId=@CustomerId"
                };
            _paging.AddParam(new SqlParam { ParameterName = "@CustomerId", Value = CustomerContext.CustomerId.ToString() });
            _paging.AddParam(new SqlParam { ParameterName = "@Type", Value = PhotoType.Product.ToString() });

            _paging.AddFieldsRange(
                new List<Field>
                    {
                       new Field {Name = "[Product].[ProductID]", IsDistinct = true},
                        new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select Count(PhotoName) From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type) ELSE (Select Count(PhotoName) From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type) END)  AS CountPhoto"},
                        new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type Order By main desc, [Photo].[PhotoSortOrder]) ELSE (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type AND [Photo].[Main] = 1) END)  AS Photo"},
                        new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type Order By main desc, [Photo].[PhotoSortOrder]) ELSE (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type AND [Photo].[Main] = 1) END)  AS PhotoDesc"},
                        new Field {Name = "[ProductCategories].[CategoryID]", NotInQuery=true},
                        new Field {Name = "BriefDescription"},
                        new Field {Name = "Product.ArtNo"},
                        new Field {Name = "Name"},
                    
                        new Field {Name = "Recomended"},
                        new Field {Name = "Bestseller"},
                        new Field {Name = "New"},
                        new Field {Name = "OnSale"},
                        new Field {Name = "Discount"},
                        new Field {Name = "Offer.Main", NotInQuery=true},
                        new Field {Name = "Offer.OfferID"},
                        new Field {Name = "Offer.Amount"},
                        new Field {Name = "(CASE WHEN Offer.Amount=0 OR Offer.Amount < IsNull(MinAmount,0) THEN 0 ELSE 1 END) as TempAmountSort", Sorting=SortDirection.Descending},
                        new Field {Name = "MinAmount"},
                        new Field {Name = "MaxAmount"},
                        new Field {Name = "Enabled"},
                        new Field {Name = "AllowPreOrder"},
                        new Field {Name = "Ratio"},
                        new Field {Name = "RatioID"},
                        new Field {Name = "DateModified"},
                        new Field {Name = "ShoppingCartItemId"},
                        new Field {Name = "UrlPath"},

                        new Field {Name = "[ShoppingCart].[CustomerID]", NotInQuery=true},
                        new Field {Name = "BrandID", NotInQuery=true},
                        new Field {Name = "Offer.ProductID as Size_ProductID", NotInQuery=true},
                        new Field {Name = "Offer.ProductID as Color_ProductID", NotInQuery=true},
                        new Field {Name = "Offer.ProductID as Price_ProductID", NotInQuery=true},
                        new Field {Name = "Offer.ColorID"},
                        new Field {Name = "CategoryEnabled", NotInQuery=true},
                        new Field {Name = "null as AdditionalPhoto"}
                    });

            if (SettingsCatalog.ComplexFilter)
            {
                _paging.AddFieldsRange(new List<Field>
                {
                    new Field {Name = "(select [Settings].[ProductColorsToString]([Product].[ProductID])) as Colors"},
                    new Field {Name = "(select max (price) - min (price) from catalog.offer where offer.productid=product.productid) as MultiPrices"},
                    new Field {Name = "(select min (price) from catalog.offer where offer.productid=product.productid) as Price"},
                });
            }
            else
            {
                _paging.AddFieldsRange(new List<Field>
                {
                    new Field {Name = "null as Colors"},
                    new Field {Name = "0 as MultiPrices"},
                    new Field {Name = "Price"},
                });
            }

            _paging.Fields["[Product].[ProductID]"].Filter = new CountProductInCategory();
            _paging.Fields["Enabled"].Filter = new EqualFieldFilter { ParamName = "@enabled", Value = "1" };
            _paging.Fields["CategoryEnabled"].Filter = new EqualFieldFilter { ParamName = "@CategoryEnabled", Value = "1" };

            BuildSorting();
            BuildFilter();

            SetMeta(null, string.Empty);
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
                case ESortOrder.AscByName:
                    _paging.Fields["Name"].Sorting = SortDirection.Ascending;
                    break;

                case ESortOrder.DescByName:
                    _paging.Fields["Name"].Sorting = SortDirection.Descending;
                    break;

                case ESortOrder.AscByPrice:
                    _paging.ExtendedSorting = "Price - Price * Discount / 100";
                    _paging.ExtendedSortingDirection = SortDirection.Ascending;
                    break;

                case ESortOrder.DescByPrice:
                    _paging.ExtendedSorting = "Price - Price * Discount / 100";
                    _paging.ExtendedSortingDirection = SortDirection.Descending;
                    break;

                case ESortOrder.AscByRatio:
                    _paging.Fields["ShowRatio"].Sorting = SortDirection.Descending;
                    _paging.Fields["Ratio"].Sorting = SortDirection.Ascending;
                    break;

                case ESortOrder.DescByRatio:
                    _paging.Fields["ShowRatio"].Sorting = SortDirection.Descending;
                    _paging.Fields["Ratio"].Sorting = SortDirection.Descending;
                    break;
            }
        }

        private void BuildFilter()
        {

            foreach (Category c in (CategoryService.GetChildCategoriesByCategoryId(0, true).Where(p => p.Enabled)))
            {
                ddlCategory.Items.Add(new ListItem { Text = c.Name, Value = c.CategoryId.ToString(), Selected = Request["category"] == c.CategoryId.ToString() });
            }

            _paging.Fields["[ProductCategories].[CategoryID]"].Filter = new InChildCategoriesFieldFilter
                {
                    CategoryId = ddlCategory.SelectedValue,
                    ParamName = "@CategoryID"
                };


            if (Page.Request["name"].IsNotEmpty())
            {
                var name = HttpUtility.UrlDecode(Page.Request["name"]).Trim();
                txtName.Text = name;
                var productIds = LuceneSearch.Search(txtName.Text).AggregateString('/');
                _paging.TableName += " inner join (select item, sort from [Settings].[ParsingBySeperator](@source,'/') ) as dtt on Product.ProductId=convert(int, dtt.item) ";
                _paging.AddParam(new SqlParam { ParameterName = "@source", Value = productIds });
                if (_sort == ESortOrder.NoSorting)
                {
                    _paging.Fields.Add("sort", new Field("sort"));
                    _paging.Fields["sort"].Sorting = SortDirection.Ascending;
                }


                SearchTerm = HttpUtility.HtmlEncode(name);
            }

            filterPrice.CategoryId = 0;
            filterPrice.InDepth = true;

            if (!string.IsNullOrEmpty(Request["pricefrom"]) || !string.IsNullOrEmpty(Request["priceto"]))
            {
                int pricefrom = Request["pricefrom"].TryParseInt(0);
                int priceto = Request["priceto"].TryParseInt(int.MaxValue);

                filterPrice.CurValMin = pricefrom;
                filterPrice.CurValMax = priceto;

                _paging.Fields["Price_ProductID"].Filter = new PriceFieldFilter
                {
                    ParamName = "@priceRange",
                    From = pricefrom * CurrencyService.CurrentCurrency.Value,
                    To = priceto * CurrencyService.CurrentCurrency.Value,
                    CategoryId = 0,
                    GetSubCategoryes = true
                };
            }
            else
            {
                filterPrice.CurValMin = 0;
                filterPrice.CurValMax = int.MaxValue;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            _paging.CurrentPageIndex = paging.CurrentPage != 0 ? paging.CurrentPage : 1;
            _paging.ItemsPerPage = SettingsCatalog.ProductsPerPage;

            vProducts.DataSource = _paging.PageItems;
            vProducts.ViewMode = productViewChanger.SearchViewMode;
            vProducts.DataBind();
            paging.TotalPages = _paging.PageCount;
            int itemsCount = _paging.TotalRowsCount;
            lItemsCount.Text = string.Format("{0} {1}", itemsCount.ToString(), Strings.Numerals(itemsCount, Resources.Resource.Client_Searsh_NoResults,
                                                                                                Resources.Resource.Client_Searsh_1Result,
                                                                                                Resources.Resource.Client_Searsh_2Results,
                                                                                                Resources.Resource.Client_Searsh_5Results));
            pnlSort.Visible = itemsCount > 0;
        }
    }
}