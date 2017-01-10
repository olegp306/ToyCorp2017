//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Globalization;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Repository;
using AdvantShop.SEO;
using Resources;
using Image = System.Drawing.Image;

namespace Admin
{
    public partial class m_Brand : AdvantShopAdminPage
    {
        protected int BrandId
        {
            get
            {
                int id = 0;
                int.TryParse(Request["id"], out id);
                return id;
            }
        }

        private void MsgErr(bool clean)
        {
            if (clean)
            {
                lblError.Visible = false;
                lblError.Text = "";
            }
            else
            {
                lblError.Visible = false;
            }
        }

        private void MsgErr(string messageText)
        {
            lblError.Visible = true;
            lblError.Text = @"<br/>" + messageText;
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            if (!Valid()) return;

            if (BrandId != 0)
            {
                SaveBrand();
            }
            else
            {
                CreateBrand();
            }
            //
            // Close window
            //
            if (lblError.Visible == false)
            {
                CommonHelper.RegCloseScript(this, string.Empty);
            }
        }

        protected void btnDeleteLogo_Click(object sender, EventArgs e)
        {
            if (BrandId != 0)
            {
                BrandService.DeleteBrandLogo(BrandId);
                pnlLogo.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_m_News_Header));
            FCKDescription.Language = FCKDescription.Language = CultureInfo.CurrentCulture.ToString();

            if (IsPostBack) return;
            ddlCountry.Items.Clear();
            ddlCountry.Items.Add(new ListItem(Resource.Client_Brands_AllCoutries, "0"));
            var list = CountryService.GetAllCountryIdAndName().OrderBy(x => x.Name).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                ddlCountry.Items.Add(new ListItem(list[i].Name, list[i].CountryId.ToString(CultureInfo.InvariantCulture)));
            }
            if (BrandId != 0)
            {
                btnOK.Text = Resource.Admin_m_News_Save;
                LoadBrandById(BrandId);
            }
            else
            {
                btnOK.Text = Resource.Admin_m_News_Add;
                pnlLogo.Visible = false;
                chkEnabled.Checked = true;
                imgLogo.ImageUrl = string.Empty;
                txtName.Text = string.Empty;
                txtBrandSiteUrl.Text = string.Empty;
                txtSortOrder.Text = @"0";
                txtHeadTitle.Text = @"#STORE_NAME# - #BRAND_NAME#";
                txtMetaKeys.Text = @"#STORE_NAME# - #BRAND_NAME#";
                txtMetaDescription.Text = @"#STORE_NAME# - #BRAND_NAME#";
                FCKDescription.Text = "";
                FCKBriefDescription.Text = "";
            }
        }

        protected bool Valid()
        {

            txtURL.Text = txtURL.Text.Replace("\'", "");
            if (string.IsNullOrEmpty(txtURL.Text))
            {
                MsgErr(Resource.Admin_m_News_NoID);
                return false;
            }
            if (!UrlService.IsAvailableUrl(BrandId, ParamType.Brand, txtURL.Text))
            {
                MsgErr(Resource.Admin_SynonymExist);
                return false;
            }
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MsgErr(Resource.Admin_m_News_NoTitle);
                return false;
            }

            int sort = 0;
            if (!int.TryParse(txtSortOrder.Text, out sort))
            {
                MsgErr(Resource.Admin_m_Brand_WrongSorting);
                return false;
            }
            if (FileUpload1.HasFile && !FileHelpers.CheckFileExtension(FileUpload1.FileName, FileHelpers.eAdvantShopFileTypes.Image))
            {
                MsgErr(Resource.Admin_ErrorMessage_WrongImageExtension);
                return false;
            }
            MsgErr(true); // Clean
            return true;
        }

        protected void SaveBrand()
        {
            try
            {
                var brand = new Brand
                    {
                        BrandId = BrandId,
                        Name = txtName.Text.Trim(),
                        //BrandLogo = logo,
                        Description = FCKDescription.Text,
                        BriefDescription = FCKBriefDescription.Text,
                        Enabled = chkEnabled.Checked,
                        UrlPath = txtURL.Text.Trim(),
                        SortOrder = txtSortOrder.Text.TryParseInt(),
                        CountryId = SQLDataHelper.GetInt(ddlCountry.SelectedValue),
                        BrandSiteUrl = txtBrandSiteUrl.Text,
                        Meta = new MetaInfo
                            {
                                ObjId = BrandId,
                                Type = MetaType.Brand,
                                Title = txtHeadTitle.Text,
                                H1 = txtH1.Text,
                                MetaKeywords = txtMetaKeys.Text,
                                MetaDescription = txtMetaDescription.Text
                            },
                    };

                BrandService.UpdateBrand(brand);
                if (!FileUpload1.HasFile) return;
                PhotoService.DeletePhotos(BrandId, PhotoType.Brand);

                var tempName = PhotoService.AddPhoto(new Photo(0, BrandId, PhotoType.Brand) { OriginName = FileUpload1.FileName });
                if (!string.IsNullOrWhiteSpace(tempName))
                {
                    using (var image = Image.FromStream(FileUpload1.FileContent))
                    {
                        FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.BrandLogo, tempName),
                                                        SettingsPictureSize.BrandLogoWidth,
                                                        SettingsPictureSize.BrandLogoHeight, image);
                    }
                }
            }
            catch (Exception ex)
            {
                MsgErr(ex.Message + " SaveBrand main");
                Debug.LogError(ex);
            }
        }

        protected void CreateBrand()
        {
            try
            {
                var brand = new Brand
                    {
                        Name = txtName.Text,
                        //BrandLogo = logo,
                        Description = FCKDescription.Text,
                        BriefDescription = FCKBriefDescription.Text,
                        Enabled = chkEnabled.Checked,
                        UrlPath = txtURL.Text,
                        SortOrder = txtSortOrder.Text.TryParseInt(),
                        CountryId = SQLDataHelper.GetInt(ddlCountry.SelectedValue),
                        BrandSiteUrl = txtBrandSiteUrl.Text,
                        Meta = new MetaInfo
                            {
                                Type = MetaType.Brand,
                                MetaDescription = txtMetaDescription.Text,
                                Title = txtHeadTitle.Text,
                                MetaKeywords = txtMetaKeys.Text,
                                H1 = txtH1.Text
                            }
                    };

                var tempbrandId = BrandService.AddBrand(brand);
                if (FileUpload1.HasFile)
                {
                    var tempName = PhotoService.AddPhoto(new Photo(0, tempbrandId, PhotoType.Brand) { OriginName = FileUpload1.FileName });
                    if (!string.IsNullOrWhiteSpace(tempName))
                    {
                        using (Image image = Image.FromStream(FileUpload1.FileContent))
                        {
                            FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.BrandLogo, tempName), SettingsPictureSize.BrandLogoWidth, SettingsPictureSize.BrandLogoHeight, image);
                        }
                    }
                }

                if (lblError.Visible == false)
                {
                    txtName.Text = string.Empty;
                    FCKDescription.Text = string.Empty;
                    FCKBriefDescription.Text = string.Empty;
                    chkEnabled.Checked = true;
                }

                // close
            }
            catch (Exception ex)
            {
                MsgErr(ex.Message + " CreateBrand main");
                Debug.LogError(ex);
            }
        }

        protected void LoadBrandById(int brandId)
        {
            Brand brand = BrandService.GetBrandById(brandId);

            if (brand == null)
            {
                MsgErr(Resource.Admin_m_Brand_ErrorLoadingBrand);
                return;
            }
            txtURL.Text = brand.UrlPath;

            txtName.Text = brand.Name;

            if (brand.BrandLogo != null)
            {
                lblLogo.Text = brand.BrandLogo.PhotoName;
                pnlLogo.Visible = true;
                imgLogo.ImageUrl = FoldersHelper.GetPath(FolderType.BrandLogo, brand.BrandLogo.PhotoName, true);
                imgLogo.ToolTip = brand.BrandLogo.PhotoName;
            }
            else
            {
                lblLogo.Text = @"No picture";
                pnlLogo.Visible = false;
            }

            txtSortOrder.Text = brand.SortOrder.ToString(CultureInfo.InvariantCulture);
            FCKDescription.Text = brand.Description;
            FCKBriefDescription.Text = brand.BriefDescription;
            chkEnabled.Checked = brand.Enabled;
            txtHeadTitle.Text = brand.Meta.Title;
            txtH1.Text = brand.Meta.H1;
            txtMetaKeys.Text = brand.Meta.MetaKeywords;
            txtMetaDescription.Text = brand.Meta.MetaDescription;
            txtBrandSiteUrl.Text = brand.BrandSiteUrl;
            ddlCountry.SelectedValue = brand.CountryId.ToString(CultureInfo.InvariantCulture);
        }
    }
}