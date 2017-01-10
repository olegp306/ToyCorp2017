using System;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Trial;
using Resources;

namespace Admin.UserControls.Settings
{
    public partial class GeneralSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resource.Admin_CommonSettings_InvalidGeneral;
        public bool FlagRedirect;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            FlagRedirect = false;

            if (!string.IsNullOrWhiteSpace(SettingsMain.LogoImageName))
            {
                pnlLogo.Visible = true;
                Logo.ImgSource = FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, true);
            }
            else
            {
                pnlLogo.Visible = false;
            }

            if (!string.IsNullOrWhiteSpace(SettingsMain.FaviconImageName))
            {
                pnlFavicon.Visible = true;
                Favicon.ImgSource = FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.FaviconImageName, true);
            }
            else
            {
                pnlFavicon.Visible = false;
            }

        }

        private void LoadData()
        {
            txtShopURL.Text = SettingsMain.SiteUrl;
            lSocialLinkVk.Text = SettingsMain.SiteUrl.Replace("www.", "").Replace("http://", "http://vk.").Replace("https://", "https://vk.");
            lSocialLinkFb.Text = SettingsMain.SiteUrl.Replace("www.", "").Replace("http://", "http://fb.").Replace("https://", "https://fb.");

            txtShopName.Text = SettingsMain.ShopName;
            txtImageAlt.Text = SettingsMain.LogoImageAlt;
            txtFormat.Text = SettingsMain.AdminDateFormat;
            txtShortFormat.Text = SettingsMain.ShortDateFormat;
            ddlCountry.DataBind();
            ddlCountry.SelectedValue = SettingsMain.SellerCountryId.ToString();
            txtPhone.Text = SettingsMain.Phone;
            txtCity.Text = SettingsMain.City;
            ckbEnableCheckConfirmCode.Checked = SettingsMain.EnableCaptcha;
            ckbEnableInplaceEditor.Checked = SettingsMain.EnableInplace;
            ckbDisplayToolBarBottom.Checked = SettingsDesign.DisplayToolBarBottom;

            ckbDisplayCityInTopPanel.Checked = SettingsDesign.DisplayCityInTopPanel;
            ckbDisplayCityBubble.Checked = SettingsDesign.DisplayCityBubble;

            ckbEnablePhoneMask.Checked = SettingsMain.EnablePhoneMask;

            int value = SettingsMain.SellerRegionId;
            if (SettingsMain.SellerRegionId != 0)
            {
                ddlRegion.DataBind();
                if (ddlRegion.Items.Count > 0)
                {
                    if (ddlRegion.Items.FindByValue(value.ToString()) != null)
                        ddlRegion.SelectedValue = value.ToString();
                }
                else
                    ddlRegion.Enabled = false;
            }
        }

        public bool SaveData()
        {
            if (!ValidateData())
                return false;

            if (SettingsMain.ShopName != txtShopName.Text)
            {
                TrialService.TrackEvent(TrialEvents.ChangeShopName, "");
            }

            if (SettingsMain.Phone != txtPhone.Text)
            {
                TrialService.TrackEvent(TrialEvents.ChangePhoneNumber, "");
            }

            if (SettingsMain.SiteUrl != txtShopURL.Text)
            {
                TrialService.TrackEvent(TrialEvents.ChangeDomain, "");
            }

            SettingsMain.SiteUrl = txtShopURL.Text.StartsWith("http://") || txtShopURL.Text.StartsWith("https://") ? txtShopURL.Text : "http://" + txtShopURL.Text;
            SettingsMain.ShopName = txtShopName.Text;
            SettingsMain.LogoImageAlt = txtImageAlt.Text;
            SettingsMain.AdminDateFormat = txtFormat.Text;
            SettingsMain.ShortDateFormat = txtShortFormat.Text;
            SettingsMain.SellerCountryId = ddlCountry.SelectedValue != string.Empty ? SQLDataHelper.GetInt(ddlCountry.SelectedValue) : 0;
            SettingsMain.Phone = txtPhone.Text;
            SettingsMain.City = txtCity.Text;
            SettingsMain.EnableCaptcha = ckbEnableCheckConfirmCode.Checked;
            SettingsMain.EnableInplace = ckbEnableInplaceEditor.Checked;

            SettingsDesign.DisplayToolBarBottom = ckbDisplayToolBarBottom.Checked;

            SettingsDesign.DisplayCityInTopPanel = ckbDisplayCityInTopPanel.Checked;
            SettingsDesign.DisplayCityBubble = ckbDisplayCityBubble.Checked;

            SettingsMain.EnablePhoneMask = ckbEnablePhoneMask.Checked;

            if (ddlRegion.Enabled)
            {
                SettingsMain.SellerRegionId = ddlRegion.SelectedValue != string.Empty ? SQLDataHelper.GetInt(ddlRegion.SelectedValue) : 0;
            }

            if (fuLogoImage.HasFile)
            {
                if (FileHelpers.CheckFileExtension(fuLogoImage.FileName, FileHelpers.eAdvantShopFileTypes.Image))
                {
                    FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
                    FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName));
                    var newFile = fuLogoImage.FileName.FileNamePlusDate("logo");
                    SettingsMain.LogoImageName = newFile;
                    fuLogoImage.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, newFile));
                    TrialService.TrackEvent(TrialEvents.ChangeLogo, "");
                }
                else
                {
                    ErrMessage = Resource.Admin_CommonSettings_InvalidLogoFormat;
                    return false;
                }
            }

            if (fuFaviconImage.HasFile)
            {
                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));

                if (FileHelpers.CheckFileExtension(fuFaviconImage.FileName, FileHelpers.eAdvantShopFileTypes.Favicon))
                {
                    FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.FaviconImageName));
                    var newFile = fuFaviconImage.FileName.FileNamePlusDate("favicon");
                    SettingsMain.FaviconImageName = newFile;
                    fuFaviconImage.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, newFile));
                }
                else
                {
                    ErrMessage = Resource.Admin_CommonSettings_InvalidFaviconFormat;
                    return false;
                }
            }

            LoadData();
            return true;
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(txtShopURL.Text))
            {
                return false;
            }
            if (txtShopURL.Text.Substring(txtShopURL.Text.Length - 1).Equals("/"))
            {
                txtShopURL.Text = txtShopURL.Text.Substring(0, txtShopURL.Text.Length - 1);
            }
            return true;
        }

        protected void DeleteLogo_Click(object sender, EventArgs e)
        {
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName));
            SettingsMain.LogoImageName = string.Empty;
            pnlLogo.Visible = false;
        }

        protected void DeleteFavicon_Click(object sender, EventArgs e)
        {
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.FaviconImageName));
            SettingsMain.FaviconImageName = string.Empty;
            pnlFavicon.Visible = false;
        }

        protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            FlagRedirect = true;
        }

        protected void sds_Init(object sender, EventArgs e)
        {
            ((SqlDataSource)sender).ConnectionString = Connection.GetConnectionString();
        }

        protected void ddlCountry_SelectedChanged(object sender, EventArgs e)
        {
            ddlRegion.DataBind();
            ddlRegion.Enabled = ddlRegion.Items.Count != 0;
        }
    }
}