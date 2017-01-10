using System;
using System.Linq;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Helpers;
using AdvantShop.News;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Trial;
using Resources;

namespace Admin.UserControls.Settings
{
    public partial class CatalogSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidCatalog;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            ddlDefaultCurrency.DataSource = SqlDataSource2;
            ddlDefaultCurrency.DataTextField = "Name";
            ddlDefaultCurrency.DataValueField = "CurrencyIso3";
            ddlDefaultCurrency.DataBind();

            ddlDefaultCurrency.SelectedValue = SettingsCatalog.DefaultCurrencyIso3;
            cbAllowToChangeCurrency.Checked = SettingsCatalog.AllowToChangeCurrency;

            txtProdPerPage.Text = SettingsCatalog.ProductsPerPage.ToString();
            cbEnableProductRating.Checked = SettingsCatalog.EnableProductRating;
            cbEnablePhotoPreviews.Checked = SettingsCatalog.EnablePhotoPreviews;
            cbEnableCompareProducts.Checked = SettingsCatalog.EnableCompareProducts;
            cbEnableCatalogViewChange.Checked = SettingsCatalog.EnabledCatalogViewChange;
            cbEnableSearchViewChange.Checked = SettingsCatalog.EnabledSearchViewChange;

            cbExluderingFilters.Checked = SettingsCatalog.ExluderingFilters;

            chkShowPriceFilter.Checked = SettingsCatalog.ShowPriceFilter;
            chkShowProducerFilter.Checked = SettingsCatalog.ShowProducerFilter;
            chkShowSizeFilter.Checked = SettingsCatalog.ShowSizeFilter;
            chkShowColorFilter.Checked = SettingsCatalog.ShowColorFilter;
            cbComplexFilter.Checked = SettingsCatalog.ComplexFilter;

            txtSizesHeader.Text = SettingsCatalog.SizesHeader;
            txtColorsHeader.Text = SettingsCatalog.ColorsHeader;
            

            txtColorPictureWidthCatalog.Text = SettingsPictureSize.ColorIconWidthCatalog.ToString();
            txtColorPictureHeightCatalog.Text = SettingsPictureSize.ColorIconHeightCatalog.ToString();
            txtColorPictureWidthDetails.Text = SettingsPictureSize.ColorIconWidthDetails.ToString();
            txtColorPictureHeightDetails.Text = SettingsPictureSize.ColorIconHeightDetails.ToString();
            ddlCatalogView.SelectedValue = ((int)SettingsCatalog.DefaultCatalogView).ToString();
            ddlSearchView.SelectedValue = ((int)SettingsCatalog.DefaultSearchView).ToString();

            txtBlockOne.Text = SettingsCatalog.RelatedProductName;
            txtBlockTwo.Text = SettingsCatalog.AlternativeProductName;

            txtBuyButtonText.Text = SettingsCatalog.BuyButtonText;
            txtMoreButtonText.Text = SettingsCatalog.MoreButtonText;
            txtPreOrderButtonText.Text = SettingsCatalog.PreOrderButtonText;

            cbDisplayBuyButton.Checked = SettingsCatalog.DisplayBuyButton;
            cbDisplayMoreButton.Checked = SettingsCatalog.DisplayMoreButton;
            cbDisplayPreOrderButton.Checked = SettingsCatalog.DisplayPreOrderButton;

            cbAvaliableFilterEnabled.Checked = SettingsCatalog.AvaliableFilterEnabled;
            cbAvaliableFilterSelected.Checked = SettingsCatalog.AvaliableFilterSelected;
            cbPreorderFilterEnabled.Checked = SettingsCatalog.PreorderFilterEnabled;
            cbPreorderFilterSelected.Checked = SettingsCatalog.PreorderFilterSelected;

            ckbShowCategoriesInBottomMenu.Checked = SettingsCatalog.DisplayCategoriesInBottomMenu;

            cbShowProductsCount.Checked = SettingsCatalog.ShowProductsCount;

            trialClearShopHeader.Visible = trialClearShop.Visible = TrialService.IsTrialEnabled;
        }


        public bool SaveData()
        {
            if (!ValidateData())
                return false;

            var iso3 = SettingsCatalog.DefaultCurrencyIso3;
            SettingsCatalog.DefaultCurrencyIso3 = ddlDefaultCurrency.SelectedValue;

            if (SettingsCatalog.DefaultCurrencyIso3 != iso3)
                CurrencyService.CurrentCurrency = CurrencyService.Currency(SettingsCatalog.DefaultCurrencyIso3);

            SettingsCatalog.AllowToChangeCurrency = cbAllowToChangeCurrency.Checked;

            SettingsCatalog.ProductsPerPage = SQLDataHelper.GetInt(txtProdPerPage.Text);
            SettingsCatalog.EnableProductRating = cbEnableProductRating.Checked;
            SettingsCatalog.EnablePhotoPreviews = cbEnablePhotoPreviews.Checked;
            SettingsCatalog.EnableCompareProducts = cbEnableCompareProducts.Checked;
            SettingsCatalog.EnabledCatalogViewChange = cbEnableCatalogViewChange.Checked;
            SettingsCatalog.EnabledSearchViewChange = cbEnableSearchViewChange.Checked;
            SettingsCatalog.DefaultCatalogView = (SettingsCatalog.ProductViewMode)SQLDataHelper.GetInt(ddlCatalogView.SelectedValue);
            SettingsCatalog.DefaultSearchView = (SettingsCatalog.ProductViewMode)SQLDataHelper.GetInt(ddlSearchView.SelectedValue);
            SettingsCatalog.DisplayCategoriesInBottomMenu = ckbShowCategoriesInBottomMenu.Checked;

            SettingsCatalog.ExluderingFilters = cbExluderingFilters.Checked;

            SettingsCatalog.ShowPriceFilter = chkShowPriceFilter.Checked;
            SettingsCatalog.ShowProducerFilter = chkShowProducerFilter.Checked;
            SettingsCatalog.ShowSizeFilter = chkShowSizeFilter.Checked;
            SettingsCatalog.ShowColorFilter  = chkShowColorFilter.Checked;
            SettingsCatalog.ComplexFilter = cbComplexFilter.Checked;
            
            SettingsCatalog.SizesHeader = txtSizesHeader.Text;
            SettingsCatalog.ColorsHeader = txtColorsHeader.Text;
            
            SettingsPictureSize.ColorIconWidthCatalog = txtColorPictureWidthCatalog.Text.TryParseInt();
            SettingsPictureSize.ColorIconHeightCatalog = txtColorPictureHeightCatalog.Text.TryParseInt();
            SettingsPictureSize.ColorIconWidthDetails = txtColorPictureWidthDetails.Text.TryParseInt();
            SettingsPictureSize.ColorIconHeightDetails = txtColorPictureHeightDetails.Text.TryParseInt();

            SettingsCatalog.RelatedProductName = txtBlockOne.Text;
            SettingsCatalog.AlternativeProductName = txtBlockTwo.Text;

            SettingsCatalog.BuyButtonText = txtBuyButtonText.Text;
            SettingsCatalog.MoreButtonText = txtMoreButtonText.Text;
            SettingsCatalog.PreOrderButtonText = txtPreOrderButtonText.Text;

            SettingsCatalog.DisplayBuyButton = cbDisplayBuyButton.Checked;
            SettingsCatalog.DisplayMoreButton = cbDisplayMoreButton.Checked;
            SettingsCatalog.DisplayPreOrderButton = cbDisplayPreOrderButton.Checked;

            SettingsCatalog.AvaliableFilterEnabled = cbAvaliableFilterEnabled.Checked;
             SettingsCatalog.AvaliableFilterSelected = cbAvaliableFilterSelected.Checked;
            SettingsCatalog.PreorderFilterEnabled = cbPreorderFilterEnabled.Checked;
            SettingsCatalog.PreorderFilterSelected = cbPreorderFilterSelected.Checked;


            SettingsCatalog.ShowProductsCount = cbShowProductsCount.Checked;

            LoadData();
            return true;
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(ddlDefaultCurrency.SelectedValue))
            {
                ErrMessage = "";
                return false;
            }

            if (string.IsNullOrEmpty(txtProdPerPage.Text))
            {
                ErrMessage = "";
                return false;
            }

            int ti;
            if (!int.TryParse(txtProdPerPage.Text, out ti))
            {
                ErrMessage = Resource.Admin_CommonSettings_NoNumberPerPage;
                return false;
            }
            return true;
        }

        protected void SqlDataSource2_Init(object sender, EventArgs e)
        {
            SqlDataSource2.ConnectionString = Connection.GetConnectionString();
        }

        protected void btnDoindex_Click(object sender, EventArgs e)
        {
            AdvantShop.FullSearch.LuceneSearch.CreateAllIndexInBackground();
            lbDone.Visible = true;
        }

        protected void bntClearShop_OnClick(object sender, EventArgs e)
        {
            if (TrialService.IsTrialEnabled)
            {
                foreach (var category in CategoryService.GetCategories().Where(category => category.ID != 0))
                {
                    CategoryService.DeleteCategoryAndPhotos(category.ID);
                }

                foreach (var productId in ProductService.GetAllProductIDs())
                {
                    ProductService.DeleteProduct(productId, false);
                }

                foreach (var property in PropertyService.GetAllProperties())
                {
                    PropertyService.DeleteProperty(property.PropertyId);
                }

                foreach (var brand in BrandService.GetBrands())
                {
                    BrandService.DeleteBrand(brand.ID);
                }


                foreach (var paymentId in PaymentService.GetAllPaymentMethodIDs())
                {
                    PaymentService.DeletePaymentMethod(paymentId);
                }

                foreach (var shippingId in ShippingMethodService.GetAllShippingMethodIds())
                {
                    ShippingMethodService.DeleteShippingMethod(shippingId);
                }

                foreach (var news in NewsService.GetNews())
                {
                    NewsService.DeleteNews(news.ID);
                }

                foreach (var newsCstegory in NewsService.GetNewsCategories())
                {
                    NewsService.DeleteNewsCategory(newsCstegory.NewsCategoryID);
                }

                CategoryService.RecalculateProductsCountManual();
                CategoryService.SetCategoryHierarchicallyEnabled(0);
                CacheManager.Clean();

                TrialService.TrackEvent(TrialEvents.DeleteTestData, string.Empty);
            }
        }

    }
}