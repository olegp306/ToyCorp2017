//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace ClientPages
{
    public partial class Catalog_Page : AdvantShopClientPage
    {
        private int _categoryId;
        protected Category Category;
        protected int ProductsCount;
        protected bool Indepth;
        protected bool IsAdmin = CustomerContext.CurrentCustomer.IsAdmin;
        private SqlPaging _paging;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(Request["categoryid"]) || !Int32.TryParse(Request["categoryid"], out _categoryId))
            {
                Error404();
            }

            Category = CategoryService.GetCategory(_categoryId);
            if (Category == null || Category.Enabled == false || Category.ParentsEnabled == false)
            {
                Error404();
                return;
            }

            //Indepth = Request["indepth"] == "1" || Category.DisplayChildProducts;
            Indepth = true;

            _paging = new SqlPaging();
            _paging.From("[Catalog].[Product]");
            _paging.Left_Join("[Catalog].[ProductExt]  ON [Product].[ProductID] = [ProductExt].[ProductID]");
            _paging.Left_Join("[Catalog].[Photo]  ON [Photo].[PhotoId] = [ProductExt].[PhotoId]");
            _paging.Left_Join("[Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]");

            _paging.Left_Join("[Catalog].[ShoppingCart] ON [ShoppingCart].[OfferID] = [Offer].[OfferID] AND [ShoppingCart].[ShoppingCartType] = 3 AND [ShoppingCart].[CustomerID] = {0}", CustomerContext.CustomerId);
            _paging.Left_Join("[Catalog].[Ratio] on Product.ProductId= Ratio.ProductID and Ratio.CustomerId=[ShoppingCart].[CustomerID]");
            // Для сотрировки по популярности
            _paging.Left_Join(@"(Select [Order].[OrderItems].[ProductId], COUNT([Order].[OrderItems].[ProductId])as Popularity  From [Order].[OrderItems],[Catalog].[Product]
where [Order].[OrderItems].[ProductId]=[Catalog].[Product].[ProductId]
Group by  [Order].[OrderItems].[ProductId])as PopTable
ON PopTable.ProductId=[Catalog].[Product].[ProductId]");


            if (!Indepth)
                _paging.Inner_Join("[Catalog].[ProductCategories] on [ProductCategories].[CategoryID] = {0} and  ProductCategories.ProductId = [Product].[ProductID]", _categoryId);

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
                        "PopularityManually",
                        "DateAdded",
                        "ISNULL (Popularity , 0)as Popularity"
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

            ProductsCount = Indepth ? Category.ProductsCount : CategoryService.GetEnabledProductsCountInCategory(Category.ID, Indepth);

            categoryView.CategoryID = _categoryId;
            //categoryView.Visible = Category.DisplayStyle == "True" || ProductsCount == 0;
            categoryView.Visible = false;

            pnlSort.Visible = ProductsCount > 0;
            productView.Visible = ProductsCount > 0;
            catalogView.CategoryID = _categoryId;

            filterProperty.CategoryId = _categoryId;

            filterBrand.CategoryId = _categoryId;
            filterBrand.InDepth = Indepth;
            filterBrand.Visible = SettingsCatalog.ShowProducerFilter;

            filterSize.CategoryId = _categoryId;
            filterSize.InDepth = Indepth;
            filterSize.Visible = SettingsCatalog.ShowSizeFilter;

            filterColor.CategoryId = _categoryId;
            filterColor.InDepth = Indepth;
            filterColor.Visible = SettingsCatalog.ShowColorFilter;

            filterPrice.CategoryId = _categoryId;
            filterPrice.InDepth = Indepth;
            filterPrice.Visible = SettingsCatalog.ShowPriceFilter;


            lblCategoryName.Text = _categoryId != 0 ? Category.Meta.H1 : Resource.Client_MasterPage_Catalog;

            breadCrumbs.Items =
                CategoryService.GetParentCategories(_categoryId).Select(parent => new BreadCrumbs
                {
                    Name = parent.Name,
                    Url = UrlService.GetLink(ParamType.Category, parent.UrlPath, parent.CategoryId)
                }).Reverse().ToList();

            if (_categoryId == 0)
            {
                breadCrumbs.Items.Add(new BreadCrumbs
                {
                    Name = Resource.Client_MasterPage_Catalog,
                    Url = UrlService.GetAbsoluteLink("catalog")
                });
            }

            breadCrumbs.Items.Insert(0, new BreadCrumbs
            {
                Name = Resource.Client_MasterPage_MainPage,
                Url = UrlService.GetAbsoluteLink("/")
            });

            //Добавим новые meta
            MetaInfo newMetaInfo = new MetaInfo();
            newMetaInfo = Category.Meta;

            StringBuilder newTitle = new StringBuilder("", 200);
            string polTitle = "";
            string vozrastTitle = "";
            string brandTitle = "";

            //если в фильтре указан бренд
            if (!string.IsNullOrEmpty(Request["brand"]))
                brandTitle = BrandService.GetBrandById(int.Parse(Request["brand"])).Name;

            // если в фильтре выбраны пол (prop = 5083 для девочек, prop = 5084 для мальчиков) или
            //возраст( prop = 5087   0 - 12 мес, prop = 5086   1 - 3 года, prop = 5075   4 - 5 лет, prop = 5081  6 - 8 лет, prop = 5082  9 - 11 лет, prop = 5085  более 12 лет
            if (!string.IsNullOrEmpty(Request["prop"]))
            {
                var filterCol = Request["prop"].Split('-');
                foreach (var val in filterCol)
                {
                    switch (val)
                    {

                        case "5083":
                            polTitle = PropertyService.GetPropertyValueById(int.Parse(val)).Value;
                            break;
                        case "5084":
                            polTitle = PropertyService.GetPropertyValueById(int.Parse(val)).Value;
                            break;

                        case "5087":
                            vozrastTitle = PropertyService.GetPropertyValueById(int.Parse(val)).Value;
                            break;
                        case "5086":
                            vozrastTitle = PropertyService.GetPropertyValueById(int.Parse(val)).Value;
                            break;
                        case "5075":
                            vozrastTitle = PropertyService.GetPropertyValueById(int.Parse(val)).Value;
                            break;
                        case "5081":
                            vozrastTitle = PropertyService.GetPropertyValueById(int.Parse(val)).Value;
                            break;
                        case "5082":
                            vozrastTitle = PropertyService.GetPropertyValueById(int.Parse(val)).Value;
                            break;
                        case "5085":
                            vozrastTitle = PropertyService.GetPropertyValueById(int.Parse(val)).Value;
                            break;
                    }

                }
            }

            //если  пол или возраст указаны в фильтре
            if (polTitle != "" || vozrastTitle != "")
            {
                newTitle.Append("Игрушки ");
                if (polTitle != "")
                    newTitle.Append(polTitle);
                if (vozrastTitle != "")
                    newTitle.Append((newTitle.ToString() == "") ? vozrastTitle : ", " + vozrastTitle);
                if (brandTitle != "")
                    newTitle.Append(" от " + brandTitle);
            }
            //если пол и возраст не указаны в фильтре
            else
            {
                newTitle.Append(Category.Name);
                if (brandTitle != "")
                    newTitle.Append(" " + brandTitle);
            }
            newMetaInfo.Title = newTitle + " купить в интернет-магазине Корпорация Игрушек";
            newMetaInfo.MetaDescription = "Интернет-магазин Корпорация Игрушек представляет: " + newTitle.ToString() + " и еще более 5,5 тысяч видов товаров по оптовым ценам. Порадуйте своего ребенка!";
            newMetaInfo.MetaKeywords = (polTitle != "" || vozrastTitle != "") ? "Игрушки " + newTitle.ToString() : newTitle.ToString();

            var metaInfo = SetMeta(newMetaInfo, Category.Name, page: paging.CurrentPage);
            //var metaInfo = SetMeta(Category.Meta, Category.Name, page: paging.CurrentPage);
            lblCategoryName.Text = metaInfo.H1;

            BuildSorting();
            BuildFilter();
        }

        private void BuildSorting()
        {
            var sort = Category.Sorting;
            if (!string.IsNullOrEmpty(Request["sort"]))
            {
                Enum.TryParse(Request["sort"], true, out sort);
            }

            foreach (ESortOrder enumItem in Enum.GetValues(typeof(ESortOrder)))
            {
                if (!SettingsCatalog.EnableProductRating &&
                    (enumItem == ESortOrder.DescByRatio || enumItem == ESortOrder.AscByRatio))
                {
                    continue;
                }

                ddlSort.Items.Add(new ListItem
                {
                    Text = enumItem.GetLocalizedName(),
                    Value = enumItem.ToString(),
                    Selected = sort == enumItem
                });
            }

            _paging.OrderBy(
            "(CASE WHEN Price=0 THEN 0 ELSE 1 END) as TempSort DESC",
            "AmountSort AS TempAmountSort DESC");

            switch (sort)
            {
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
                case ESortOrder.AscByAddingDate:
                    _paging.OrderBy("DateAdded as DateAddedSort ASC");
                    break;
                case ESortOrder.DescByAddingDate:
                    _paging.OrderBy("DateAdded as DateAddedSort DESC");
                    break;
                //case ESortOrder.AscByPopularity:
                //    _paging.OrderBy("Popularity as PopularitySort ASC");
                //    break;
                //case ESortOrder.DescByPopularity:
                //    _paging.OrderBy("Popularity as PopularitySort DESC");
                //    break;

                case ESortOrder.AscByPopularity:
                    _paging.OrderBy("PopularityManually as PopularitySort ASC");
                    break;
                case ESortOrder.DescByPopularity:
                    _paging.OrderBy("PopularityManually as PopularitySort DESC");
                    break;
            }

            _paging.OrderBy(!Indepth ? "[ProductCategories].[SortOrder] ASC" : "");
        }

        private void BuildFilter()
        {
            _paging.Where("Enabled={0}", true);
            _paging.Where("AND CategoryEnabled={0}", true);

            if (Indepth)
            {
                _paging.Where("AND Exists( select 1 from [Catalog].[ProductCategories] INNER JOIN [Settings].[GetChildCategoryByParent]({0}) AS hCat ON hCat.id = [ProductCategories].[CategoryID] and  ProductCategories.ProductId = [Product].[ProductID])", _categoryId);
            }

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

            _paging.Where("AND Offer.Main={0} AND Offer.Main IS NOT NULL", true);


            if (!string.IsNullOrEmpty(Request["size"]))
            {
                var sizeIds = Request["size"].Split(',').Select(item => item.TryParseInt()).Where(id => id != 0).ToList();
                filterSize.SelectedSizesIDs = sizeIds;
                _paging.Where("and Exists( select 1 from [Catalog].[Offer] where Offer.[SizeID] IN ({0}) and Offer.ProductId = [Product].[ProductID])", sizeIds.ToArray());
            }
            else
            {
                filterSize.SelectedSizesIDs = new List<int>();
            }

            if (!string.IsNullOrEmpty(Request["color"]))
            {
                var colorIds = Request["color"].Split(',').Select(item => item.TryParseInt()).Where(id => id != 0).ToList();
                filterColor.SelectedColorsIDs = colorIds;
                _paging.Where("and Exists( select 1 from [Catalog].[Offer] where Offer.[ColorID] IN ({0}) and Offer.ProductId = [Product].[ProductID]  and Offer.[Amount] > 0)", colorIds.ToArray());

                if (SettingsCatalog.ComplexFilter)
                {
                    _paging.Select(
                        string.Format(
                                "(select Top 1 PhotoName from catalog.Photo inner join catalog.offer on Photo.objid=offer.productid and Type='product'" +
                                " where offer.productid=product.productid and Photo.ColorID in({0}) order by Photo.PhotoSortOrder, Photo.Main)" +
                                " as AdditionalPhoto",
                                colorIds.AggregateString(',')));
                }
                else
                {
                    _paging.Select("null as AdditionalPhoto");
                }

            }
            else
            {
                filterColor.SelectedColorsIDs = new List<int>();
                _paging.Select("null as AdditionalPhoto");
            }

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

            if (!string.IsNullOrEmpty(Request["prop"]))
            {
                var selectedPropertyIDs = new List<int>();
                var filterCollection = Request["prop"].Split('-');

                CheckSelectCategory();

                foreach (var val in filterCollection)
                {
                    var tempListIds = new List<int>();
                    foreach (int id in val.Split(',').Select(item => item.TryParseInt()).Where(id => id != 0))
                    {
                        tempListIds.Add(id);
                        selectedPropertyIDs.Add(id);
                    }
                    if (tempListIds.Count > 0)
                        _paging.Where("AND Exists( select 1 from [Catalog].[ProductPropertyValue] where [Product].[ProductID] = [ProductID] and PropertyValueID IN ({0}))", tempListIds.ToArray());
                }
                filterProperty.SelectedPropertyIDs = selectedPropertyIDs;
            }
            else
            {
                filterProperty.SelectedPropertyIDs = new List<int>();
            }


            var rangeIds = new Dictionary<int, KeyValuePair<float, float>>();
            var rangeQueries =
                Request.QueryString.AllKeys.Where(
                    p => p != null && p.StartsWith("prop_") && (p.EndsWith("_min") || p.EndsWith("_max"))).ToList();

            foreach (var rangeQuery in rangeQueries)
            {
                if (rangeQuery.EndsWith("_max"))
                    continue;

                var propertyId = rangeQuery.Split('_')[1].TryParseInt();
                if (propertyId == 0)
                    continue;

                var min = Request.QueryString[rangeQuery].TryParseFloat();
                var max = Request.QueryString[rangeQuery.Replace("min", "max")].TryParseFloat();

                rangeIds.Add(propertyId, new KeyValuePair<float, float>(min, max));
            }

            if (rangeIds.Count > 0)
            {
                foreach (var i in rangeIds.Keys)
                {
                    _paging.Where("AND Exists( select 1 from [Catalog].[ProductPropertyValue] ");
                    _paging.Where("inner Join [Catalog].[PropertyValue] on [PropertyValue].[PropertyValueID] = [ProductPropertyValue].[PropertyValueID]");
                    _paging.Where("where [Product].[ProductID] = [ProductID] and PropertyId = {0}", i);
                    _paging.Where("And RangeValue >= {0}", rangeIds[i].Key);
                    _paging.Where("And RangeValue <= {0})", rangeIds[i].Value);
                }

            }
            filterProperty.SelectedRangePropertyIDs = rangeIds;

            switch (Request["available"])
            {
                case "1":
                    filterExtra.AvailableSelected = true;
                    break;
                case "0":
                    filterExtra.AvailableSelected = false;
                    break;
                default:
                    filterExtra.AvailableSelected = SettingsCatalog.AvaliableFilterSelected;
                    break;
            }

            if (filterExtra.AvailableSelected)
            {
                _paging.Where("AND MaxAvailable>{0}", 0);
            }


            switch (Request["preorder"])
            {
                case "1":
                    filterExtra.PreOrderSelected = true;
                    break;
                case "0":
                    filterExtra.PreOrderSelected = false;
                    break;
                default:
                    filterExtra.PreOrderSelected = SettingsCatalog.PreorderFilterSelected;
                    break;
            }

            if (filterExtra.PreOrderSelected)
            {
                _paging.Where("AND AllowPreOrder={0}", true);
                _paging.Where("AND MaxAvailable={0}", false);
            }

        }

        private void CheckSelectCategory()
        {
            var CategoryId = AdvantShop.Catalog.CategoryService.GetCategoryIdByName("Каталог");
            List<Category> filterCategories = new List<Category>();

            filterCategories = AdvantShop.Catalog.CategoryService.GetChildCategoriesByCategoryIdForMenu(CategoryId).ToList();
            var categoryUrls = filterCategories.Select(url => url.UrlPath);

            foreach (var category in filterCategories)
            {
                if (Request["prop"].ToString().Contains(category.UrlPath))
                {
                    filterProperty.SelectedCategoryId = category.CategoryId;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            _paging.ItemsPerPage = paging.CurrentPage != 0 ? SettingsCatalog.ProductsPerPage : int.MaxValue;
            _paging.CurrentPageIndex = paging.CurrentPage != 0 ? paging.CurrentPage : 1;

            var totalCount = _paging.TotalRowsCount;

            lTotalItems.Text = string.Format(Resource.Client_Catalog_ItemsFound, totalCount);

            paging.TotalPages = _paging.PageCount(totalCount);

            if ((paging.TotalPages < paging.CurrentPage && paging.CurrentPage > 1) || paging.CurrentPage < 0)
            {
                Error404();
                return;
            }


            // if we get request from ajax filter
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
                        }, false
                        ).First();

                var res = JsonConvert.SerializeObject(new
                {
                    ProductsCount = totalCount,
                    AvalibleProperties = filterProperty.AvaliblePropertyIDs,
                    AvalibleBrands = filterBrand.AvalibleBrandIDs,
                    AvaliblePriceFrom = Math.Floor(prices.From / CurrencyService.CurrentCurrency.Value),
                    AvaliblePriceTo = Math.Ceiling(prices.To / CurrencyService.CurrentCurrency.Value),
                });
                Response.Write(res);
                Response.End();
                return;
            }

            var data = _paging.PageItems;
           
            productView.DataSource = data;
            productView.DataBind();


            bool exluderingFilters = SettingsCatalog.ExluderingFilters;

            filterProperty.AvaliblePropertyIDs = exluderingFilters
                                                     ? _paging.GetCustomData("PropertyValueID",
                                                                             " AND PropertyValueID is not null",
                                                                             reader =>
                                                                             SQLDataHelper.GetInt(reader, "PropertyValueID"),
                                                                             true,
                                                                             "Left JOIN [Catalog].[ProductPropertyValue] ON [Product].[ProductID] = [ProductPropertyValue].[ProductID]") : null;
            filterBrand.AvalibleBrandIDs = exluderingFilters
                                               ? _paging.GetCustomData("BrandID",
                                                                       " AND BrandID is not null",
                                                                       reader => SQLDataHelper.GetInt(reader, "BrandID"), true) : null;

            if (SettingsCatalog.ShowSizeFilter)
            {
                filterSize.AvalibleSizesIDs = exluderingFilters
                    ? _paging.GetCustomData("sizeOffer.SizeID", " AND sizeOffer.SizeID is not null",
                                            reader => SQLDataHelper.GetInt(reader, "SizeID"), true,
                                            "Left JOIN [Catalog].[Offer] as sizeOffer ON [Product].[ProductID] = [sizeOffer].[ProductID]")
                    : null;
            }

            if (SettingsCatalog.ShowColorFilter)
            {
                filterColor.AvalibleColorsIDs = exluderingFilters
                    ? _paging.GetCustomData("colorOffer.ColorID", " AND colorOffer.ColorID is not null",
                                            reader => SQLDataHelper.GetInt(reader, "ColorID"), true,
                                            "Left JOIN [Catalog].[Offer] as colorOffer ON [Product].[ProductID] = [colorOffer].[ProductID]")
                    : null;
            }

            if (GoogleTagManager.Enabled)
            {
                var tagManager = ((AdvantShopMasterPage)Master).TagManager;
                tagManager.PageType = GoogleTagManager.ePageType.category;
                tagManager.CatCurrentId = Category.ID;
                tagManager.CatCurrentName = Category.Name;
                tagManager.CatParentId = Category.ParentCategory.ID;
                tagManager.CatParentName = Category.ParentCategory.Name;

                tagManager.ProdIds = new List<string>();
                foreach (DataRow row in data.Rows)
                {
                    tagManager.ProdIds.Add((string)row["ArtNo"]);
                }
            }
        }
    }
}