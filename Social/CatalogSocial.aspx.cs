//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.CMS;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using Resources;

namespace Social
{
    public partial class CatalogPage: AdvantShopClientPage
    {
        private int _categoryId;
        protected Category category;
        protected int ProductsCount;
        protected string MainPageText;

        private SqlPaging _paging;

        protected void Page_Load(object sender, EventArgs e)
        {
            _paging = new SqlPaging
                {
                    TableName =
                        "[Catalog].[Product] LEFT JOIN [Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] inner join Catalog.ProductCategories on ProductCategories.ProductId = [Product].[ProductID] Left JOIN [Catalog].[ProductPropertyValue] ON [Product].[ProductID] = [ProductPropertyValue].[ProductID] LEFT JOIN [Catalog].[ShoppingCart] ON [Catalog].[ShoppingCart].[OfferID] = [Catalog].[Offer].[OfferID] AND [Catalog].[ShoppingCart].[ShoppingCartType] = 3 AND [ShoppingCart].[CustomerID] = @CustomerId Left JOIN [Catalog].[Ratio] on Product.ProductId= Ratio.ProductID and Ratio.CustomerId=@CustomerId"
                };
            _paging.AddFieldsRange(
                new List<Field>
                    {
                        new Field {Name = "[Product].[ProductID]", IsDistinct = true},
                        //new Field {Name = "PhotoName AS Photo"},
                        new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select Count(PhotoName) From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type) ELSE (Select Count(PhotoName) From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type) END)  AS CountPhoto"},
                    new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type Order By main desc, [Photo].[PhotoSortOrder]) ELSE (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type AND [Photo].[Main] = 1) END)  AS Photo"},
                    new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type Order By main desc, [Photo].[PhotoSortOrder]) ELSE (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type AND [Photo].[Main] = 1) END)  AS PhotoDesc"},
                     
                        new Field {Name = "(select [Settings].[ProductColorsToString]([Product].[ProductID])) as Colors"},
                        //new Field {Name = "[Photo].[Description] AS PhotoDesc"},
                        new Field {Name = "[ProductCategories].[CategoryID]", NotInQuery=true},
                        new Field {Name = "BriefDescription"},
                        new Field {Name = "Product.ArtNo"},
                        new Field {Name = "Name"},
                        new Field {Name = "(CASE WHEN Price=0 THEN 0 ELSE 1 END) as TempSort", Sorting=SortDirection.Descending},
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
                        new Field {Name = "[ProductCategories].[SortOrder]"},
                        new Field {Name = "[ShoppingCart].[CustomerID]", NotInQuery=true},
                        new Field {Name = "BrandID", NotInQuery=true},
                        new Field {Name = "Offer.ProductID as Size_ProductID", NotInQuery=true},
                        new Field {Name = "Offer.ProductID as Color_ProductID", NotInQuery=true},
                        new Field {Name = "Offer.ProductID as Price_ProductID", NotInQuery=true},
                        new Field {Name = "Offer.ColorID"},
                        new Field {Name = "CategoryEnabled", NotInQuery=true},
                    });

            if (SettingsCatalog.ComplexFilter)
            {
                _paging.AddFieldsRange(new List<Field>
                    {
                        new Field {Name = "(select max (price) - min (price) from catalog.offer where offer.productid=product.productid) as MultiPrices"},
                        new Field {Name = "(select min (price) from catalog.offer where offer.productid=product.productid) as Price"},
                    });
            }
            else
            {
                _paging.AddFieldsRange(new List<Field>
                    {
                        new Field {Name = "0 as MultiPrices"},
                        new Field {Name = "Price"},
                    });
            }

            _paging.AddParam(new SqlParam { ParameterName = "@CustomerId", Value = CustomerContext.CustomerId.ToString() });
            _paging.AddParam(new SqlParam { ParameterName = "@Type", Value = PhotoType.Product.ToString() });

            if (string.IsNullOrEmpty(Request["categoryid"]) || !Int32.TryParse(Request["categoryid"], out _categoryId))
            {
                _categoryId = 0;

                var sbMainPage = StaticBlockService.GetPagePartByKeyWithCache("MainPageSocial");
                if (sbMainPage != null && sbMainPage.Enabled)
                    MainPageText = sbMainPage.Content;
            }

            if (!string.IsNullOrEmpty(MainPageText))
            {
                SetMeta(null, string.Empty);
                return;
            }

            category = CategoryService.GetCategory(_categoryId);
            if (category == null || category.Enabled == false || category.ParentsEnabled == false)
            {
                Error404();
                return;
            }

            ProductsCount = category.GetProductCount();

            categoryView.CategoryID = _categoryId;
            categoryView.Visible = true;
            pnlSort.Visible = ProductsCount > 0;
            productView.Visible = ProductsCount > 0;

            lblCategoryName.Text = _categoryId != 0 ? category.Name : Resource.Client_MasterPage_Catalog;
            //lblCategoryDescription.Text = category.Description;

            //imgCategoryImage.ImageUrl = string.IsNullOrEmpty(category.Picture) ? "" : string.Format("{0}", ImageFolders.GetImageCategoryPath(false, category.Picture));

            breadCrumbs.Items =
                CategoryService.GetParentCategories(_categoryId).Select(parent => new BreadCrumbs
                    {
                        Name = parent.Name,
                        Url = "social/catalogsocial.aspx?categoryid=" + parent.CategoryId
                    }).Reverse().ToList();
            breadCrumbs.Items.Insert(0, new BreadCrumbs
                {
                    Name = Resource.Client_MasterPage_MainPage,
                    Url = UrlService.GetAbsoluteLink("social/catalogsocial.aspx")
                });

            SetMeta(category.Meta, category.Name);
        
            if (category.DisplayChildProducts)
            {
                var cfilter = new InChildCategoriesFieldFilter
                    {
                        CategoryId = _categoryId.ToString(),
                        ParamName = "@CategoryID"
                    };
                _paging.Fields["[ProductCategories].[CategoryID]"].Filter = cfilter;
            }
            else
            {
                var cfilter = new EqualFieldFilter { Value = _categoryId.ToString(), ParamName = "@catalog" };
                _paging.Fields["[ProductCategories].[CategoryID]"].Filter = cfilter;
            }

            _paging.Fields["Enabled"].Filter = new EqualFieldFilter { Value = "1", ParamName = "@enabled" }; ;
            _paging.Fields["CategoryEnabled"].Filter = new EqualFieldFilter { Value = "1", ParamName = "@CategoryEnabled" };

            var logicalFilter = new LogicalFilter { ParamName = "@Main", HideInCustomData = true };
            logicalFilter.AddFilter(new EqualFieldFilter { Value = "1", ParamName = "@Main1", HideInCustomData = true });
            logicalFilter.AddLogicalOperation("OR");
            logicalFilter.AddFilter(new NullFieldFilter { Null = true, ParamName = "@Main2", HideInCustomData = true });
            _paging.Fields["Offer.Main"].Filter = logicalFilter;

            BuildSorting();
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
                    _paging.Fields["Ratio"].Sorting = SortDirection.Ascending;
                    break;

                case ESortOrder.DescByRatio:
                    _paging.Fields["Ratio"].Sorting = SortDirection.Descending;
                    break;

                default:
                    _paging.Fields["[ProductCategories].[SortOrder]"].Sorting = SortDirection.Ascending;
                    break;
            }
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MainPageText))
                return;

            _paging.ItemsPerPage = paging.CurrentPage != 0 ? SettingsCatalog.ProductsPerPage : int.MaxValue;
            _paging.CurrentPageIndex = paging.CurrentPage != 0 ? paging.CurrentPage : 1;

            productView.ViewMode = productViewChanger.CatalogViewMode;
            lTotalItems.Text = string.Format(Resource.Client_Catalog_ItemsFound, _paging.TotalRowsCount);

            paging.TotalPages = _paging.PageCount;
            productView.DataSource = _paging.PageItems;
            productView.DataBind();
        }
    }
}