using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Design;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Permission;
using AdvantShop.Repository;
using AdvantShop.SaasData;
using AdvantShop.Trial;
using Resources;

namespace ClientPages
{
    public partial class install_UserContols_ShopinfoView : AdvantShop.Controls.InstallerStep
    {
        public bool ActiveLic;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Demo.IsDemoEnabled || TrialService.IsTrialEnabled)
            {
                divKey.Visible = false;
            }
        }

        protected void sds_Init(object sender, EventArgs e)
        {
            ((SqlDataSource)sender).ConnectionString = Connection.GetConnectionString();
        }

        public new void LoadData()
        {
            if (!string.IsNullOrWhiteSpace(SettingsMain.LogoImageName))
                imgLogo.ImageUrl = "../" + FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, true);
            else
                imgLogo.Visible = false;

            if (!string.IsNullOrWhiteSpace(SettingsMain.FaviconImageName))
                imgFavicon.ImageUrl = "../" +
                                      FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.FaviconImageName, true);
            else
                imgFavicon.Visible = false;

            if (!(Demo.IsDemoEnabled || TrialService.IsTrialEnabled))
            {
                txtKey.Text = SettingsLic.LicKey;
            }

            txtShopName.Text = SettingsMain.ShopName;
            txtUrl.Text = SettingsMain.SiteUrl;
            //txtTitle.Text = SettingsSEO.DefaultMetaTitle;
            //txtMetadescription.Text = SettingsSEO.DefaultMetaDescription;
            //txtMetakeywords.Text = SettingsSEO.DefaultMetaKeywords;

            ddlCountry.DataBind();
            ddlCountry.SelectedValue = SettingsMain.SellerCountryId.ToString();
            txtCity.Text = SettingsMain.City;

            int value = SettingsMain.SellerRegionId;

            if (SettingsMain.SellerRegionId != 0)
            {
                var region = RegionService.GetRegion(SettingsMain.SellerRegionId);
                if (region != null)
                    txtRegion.Text = region.Name;
            }

            txtPhone.Text = SettingsMain.Phone;
            txtDirector.Text = SettingsBank.Director;
            txtAccountant.Text = SettingsBank.Accountant;
            txtManager.Text = SettingsBank.Manager;
        }

        public new void SaveData()
        {
            if (!(Demo.IsDemoEnabled || TrialService.IsTrialEnabled))
            {
                SettingsLic.LicKey = txtKey.Text;
            }

            if (!TemplateService.IsExistTemplate(SettingsDesign.Template))
                SettingsDesign.ChangeTemplate(TemplateService.DefaultTemplateId);

            SettingsMain.ShopName = txtShopName.Text;
            SettingsMain.SiteUrl = txtUrl.Text;
            SettingsMain.LogoImageAlt = txtShopName.Text;
            if (fuLogo.HasFile)
            {
                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.Pictures));
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName));
                SettingsMain.LogoImageName = fuLogo.FileName;
                fuLogo.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, fuLogo.FileName));
            }

            if (fuFavicon.HasFile)
            {
                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.Pictures));
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures,
                    SettingsMain.FaviconImageName));
                SettingsMain.FaviconImageName = fuFavicon.FileName;
                fuLogo.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, fuFavicon.FileName));
            }

            //SettingsSEO.DefaultMetaTitle = txtTitle.Text;
            //SettingsSEO.DefaultMetaDescription = txtMetadescription.Text;
            //SettingsSEO.DefaultMetaKeywords = txtMetakeywords.Text;
            var countryId = 0;
            int.TryParse(ddlCountry.SelectedValue, out countryId);
            SettingsMain.SellerCountryId = countryId;

            var regionId = RegionService.GetRegionIdByName(txtRegion.Text);
            SettingsMain.SellerRegionId = regionId;

            SettingsMain.City = txtCity.Text;
            SettingsMain.Phone = txtPhone.Text;
            SettingsBank.Director = txtDirector.Text;
            SettingsBank.Accountant = txtAccountant.Text;
            SettingsBank.Manager = txtManager.Text;
        }

        public new bool Validate()
        {
            if (!(Demo.IsDemoEnabled || TrialService.IsTrialEnabled || SaasDataService.IsSaasEnabled))
            {
                try
                {
                    ActiveLic = PermissionAccsess.ActiveLic(txtKey.Text, txtUrl.Text, txtShopName.Text,
                        SettingsGeneral.SiteVersion, SettingsGeneral.SiteVersionDev);
                    SettingsLic.ActiveLic = ActiveLic;
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex, "Error at license check at installer");
                }
                if (!ActiveLic)
                {
                    lblError.Text = Resources.Resource.Install_UserContols_ShopinfoView_Err_WrongKey;
                }
            }

            if (fuLogo.HasFile)
            {
                if(!FileHelpers.CheckFileExtension(Path.GetExtension(fuLogo.FileName), FileHelpers.eAdvantShopFileTypes.Image))
                {
                    lblError.Text = Resource.Admin_CommonSettings_InvalidLogoFormat;
                    return false;
                }
            }

            if (fuFavicon.HasFile)
            {
                if(!FileHelpers.CheckFileExtension(Path.GetExtension(fuFavicon.FileName), FileHelpers.eAdvantShopFileTypes.Favicon))
                {
                    lblError.Text = Resource.Admin_CommonSettings_InvalidFaviconFormat;
                    return false;
                }
            }


            var validList = new List<ValidElement>
            {
                new ValidElement()
                {
                    Control = txtShopName,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_ShopinfoView_Err_NeedName
                },
                new ValidElement()
                {
                    Control = txtUrl,
                    ErrContent = ErrContent,
                    ValidType = ValidType.Required,
                    Message = Resources.Resource.Install_UserContols_ShopinfoView_Err_NeedUrl
                }
            };

            return ValidationHelper.Validate(validList);
        }
    }
}