using System;
using AdvantShop.Configuration;
using AdvantShop.Trial;

namespace Admin.UserControls.Settings
{
    public partial class SEOSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidSEO;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            txtProductsHeadTitle.Text = SettingsSEO.ProductMetaTitle;
            txtProductsMetaKeywords.Text = SettingsSEO.ProductMetaKeywords;
            txtProductsMetaDescription.Text = SettingsSEO.ProductMetaDescription;
            txtProductsH1.Text = SettingsSEO.ProductMetaH1;
            txtProductsAdditionalDescription.Text = SettingsSEO.ProductAdditionalDescription;

            txtCategoriesHeadTitle.Text = SettingsSEO.CategoryMetaTitle;
            txtCategoriesMetaKeywords.Text = SettingsSEO.CategoryMetaKeywords;
            txtCategoriesMetaDescription.Text = SettingsSEO.CategoryMetaDescription;
            txtCategoriesMetaH1.Text = SettingsSEO.CategoryMetaH1;

            txtNewsHeadTitle.Text = SettingsSEO.NewsMetaTitle;
            txtNewsMetaKeywords.Text = SettingsSEO.NewsMetaKeywords;
            txtNewsMetaDescription.Text = SettingsSEO.NewsMetaDescription;
            txtNewsH1.Text = SettingsSEO.NewsMetaH1;

            txtStaticPageHeadTitle.Text = SettingsSEO.StaticPageMetaTitle;
            txtStaticPageMetaKeywords.Text = SettingsSEO.StaticPageMetaKeywords;
            txtStaticPageMetaDescription.Text = SettingsSEO.StaticPageMetaDescription;
            txtStaticPageH1.Text = SettingsSEO.StaticPageMetaH1;

            txtTitle.Text = SettingsSEO.DefaultMetaTitle;
            txtMetaKeys.Text = SettingsSEO.DefaultMetaKeywords;
            txtMetaDescription.Text = SettingsSEO.DefaultMetaDescription;


            txtBrandTitle.Text = SettingsSEO.BrandMetaTitle;
            txtBrandMetaKeywords.Text = SettingsSEO.BrandMetaKeywords;
            txtBrandMetaDescription.Text = SettingsSEO.BrandMetaDescription;

            txtCustomMetaString.Text = SettingsSEO.CustomMetaString;

        }

        public bool SaveData()
        {
            SettingsSEO.ProductMetaTitle = txtProductsHeadTitle.Text;
            SettingsSEO.ProductMetaKeywords = txtProductsMetaKeywords.Text;
            SettingsSEO.ProductMetaDescription = txtProductsMetaDescription.Text;
            SettingsSEO.ProductMetaH1 = txtProductsH1.Text;
            SettingsSEO.ProductAdditionalDescription = txtProductsAdditionalDescription.Text;

            SettingsSEO.CategoryMetaTitle = txtCategoriesHeadTitle.Text;
            SettingsSEO.CategoryMetaKeywords = txtCategoriesMetaKeywords.Text;
            SettingsSEO.CategoryMetaDescription = txtCategoriesMetaDescription.Text;
            SettingsSEO.CategoryMetaH1 = txtCategoriesMetaH1.Text;

            SettingsSEO.NewsMetaTitle = txtNewsHeadTitle.Text;
            SettingsSEO.NewsMetaKeywords = txtNewsMetaKeywords.Text;
            SettingsSEO.NewsMetaDescription = txtNewsMetaDescription.Text;
            SettingsSEO.NewsMetaH1 = txtNewsH1.Text;

            SettingsSEO.StaticPageMetaTitle = txtStaticPageHeadTitle.Text;
            SettingsSEO.StaticPageMetaKeywords = txtStaticPageMetaKeywords.Text;
            SettingsSEO.StaticPageMetaDescription = txtStaticPageMetaDescription.Text;
            SettingsSEO.StaticPageMetaH1 = txtStaticPageH1.Text;

            SettingsSEO.DefaultMetaTitle = txtTitle.Text;
            SettingsSEO.DefaultMetaKeywords = txtMetaKeys.Text;
            SettingsSEO.DefaultMetaDescription = txtMetaDescription.Text;

            SettingsSEO.BrandMetaTitle = txtBrandTitle.Text;
            SettingsSEO.BrandMetaKeywords = txtBrandMetaKeywords.Text;
            SettingsSEO.BrandMetaDescription = txtBrandMetaDescription.Text;

            SettingsSEO.CustomMetaString = txtCustomMetaString.Text;
            LoadData();

            return true;
        }
    }
}