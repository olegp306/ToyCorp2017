//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.News;
using AdvantShop.SEO;
using Resources;

namespace Admin
{
    public partial class m_News : AdvantShopAdminPage
    {
        protected int NewsId
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
                lblError.Text = string.Empty;
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

            if (NewsId != 0)
            {
                SaveNews();
            }
            else
            {
                CreateNews();
            }

            // Close window
            if (lblError.Visible == false)
            {
                CommonHelper.RegCloseScript(this, string.Empty);
            }
        }

        protected void btnDeleteImage_Click(object sender, EventArgs e)
        {
            if (NewsId != 0)
            {
                NewsService.DeleteNewsImage(NewsId);
                pnlImage.Visible = false;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_m_News_Header));
            CKEditorControlAnnatation.Language = FCKTextToPublication.Language = CultureInfo.CurrentCulture.ToString();
            lblImageInfo.Text = string.Format("* {0} {1}x{2}px", Resource.Admin_m_News_ResultImageSize,
                                              SettingsPictureSize.NewsImageWidth, SettingsPictureSize.NewsImageHeight);

            if (!IsPostBack)
            {
                if (NewsId != 0)
                {
                    btnOK.Text = Resource.Admin_m_News_Save;
                    LoadNewsById(NewsId);

                }
                else
                {
                    txtStringID.Text = @"news-" + NewsService.GetLastId();
                    btnOK.Text = Resource.Admin_m_News_Add;
                    txtDate.Text = DateTime.Now.ToShortDateString();
                    txtTime.Text = DateTime.Now.ToString("HH:mm");
                    pnlImage.Visible = false;

                    Image1.ImageUrl = string.Empty;
                    txtTitle.Text = string.Empty;
                    txtHeadTitle.Text = string.Empty;
                    txtMetaKeys.Text = string.Empty;
                    txtMetaDescription.Text = string.Empty;
                    FCKTextToPublication.Text = string.Empty;
                }
            }
        }

        private bool Valid()
        {
            MsgErr(true);
            DateTime temp;
            if (!DateTime.TryParse(txtDate.Text, out  temp))
            {
                MsgErr(Resource.Admin_m_News_WrongDateFormat);
                return false;
            }

            txtStringID.Text = txtStringID.Text.Replace("\'", "");
            if (string.IsNullOrEmpty(txtStringID.Text))
            {
                MsgErr(Resource.Admin_m_News_NoID);
                return false;
            }

            if (!UrlService.IsAvailableUrl(NewsId, ParamType.News, txtStringID.Text))
            {
                MsgErr(Resource.Admin_SynonymExist);
                return false;
            }

            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                MsgErr(Resource.Admin_m_News_NoTitle);
                return false;
            }

            if (string.IsNullOrEmpty(FCKTextToPublication.Text))
            {
                MsgErr(Resource.Admin_m_News_NoMessageText);
                return false;
            }

            if (string.IsNullOrEmpty(CKEditorControlAnnatation.Text))
            {
                MsgErr(Resource.Admin_m_News_NoAnnotation);
                return false;
            }

            if (dboNewsCategory.SelectedValue.TryParseInt() < 1)
            {
                MsgErr(Resource.Admin_m_News_NoCategory);
                return false;
            }

            return true;
        }

        private void SaveNews()
        {
            MsgErr(true); // Clean
            try
            {
                var news = new NewsItem
                    {
                        NewsID = NewsId,
                        AddingDate = SQLDataHelper.GetDateTime(txtDate.Text + " " + txtTime.Text),
                        Title = txtTitle.Text,
                        //Picture = file,
                        TextToPublication = FCKTextToPublication.Text,
                        TextAnnotation = CKEditorControlAnnatation.Text,
                        TextToEmail = string.Empty,//rbNo.Checked ? String.Empty : FCKTextToEmail.Text,
                        NewsCategoryID = dboNewsCategory.SelectedValue.TryParseInt(),
                        ShowOnMainPage = chkOnMainPage.Checked,
                        UrlPath = txtStringID.Text,
                        //MetaId = SQLDataHelper.GetInt(hfMetaId.Text),
                        Meta = new MetaInfo
                            {
                                ObjId = NewsId,
                                Title = txtHeadTitle.Text,
                                H1 = txtH1.Text,
                                MetaKeywords = txtMetaKeys.Text,
                                MetaDescription = txtMetaDescription.Text,
                                Type = MetaType.News
                            }
                    };

                news.UrlPath = txtStringID.Text;
                NewsService.UpdateNews(news);
                if (FileUpload1.HasFile)
                {
                    if (!FileHelpers.CheckFileExtension(FileUpload1.FileName, FileHelpers.eAdvantShopFileTypes.Image))
                    {
                        MsgErr(Resource.Admin_ErrorMessage_WrongImageExtension);
                        return;
                    }
                    PhotoService.DeletePhotos(NewsId, PhotoType.News);

                    var tempName = PhotoService.AddPhoto(new Photo(0, NewsId, PhotoType.News) { OriginName = FileUpload1.FileName });
                    if (!string.IsNullOrWhiteSpace(tempName))
                    {
                        using (var image = Image.FromStream(FileUpload1.FileContent))
                            FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.News, tempName), SettingsPictureSize.NewsImageWidth, SettingsPictureSize.NewsImageHeight, image);
                    }
                }
            }
            catch (Exception ex)
            {
                MsgErr(ex.Message + " SaveNews main");
                Debug.LogError(ex);
            }
        }

        private void CreateNews()
        {

            MsgErr(true); // Clean

            try
            {
                //const string format = "yyyy-MM-dd";
                var news = new NewsItem
                    {
                        AddingDate = SQLDataHelper.GetDateTime(txtDate.Text + " " + txtTime.Text),
                        Title = txtTitle.Text,
                        //Picture = file,
                        TextToPublication = FCKTextToPublication.Text,
                        TextAnnotation = CKEditorControlAnnatation.Text,
                        TextToEmail = String.Empty,
                        NewsCategoryID = dboNewsCategory.SelectedValue.TryParseInt(),
                        ShowOnMainPage = chkOnMainPage.Checked,
                        UrlPath = txtStringID.Text,
                        Meta = new MetaInfo
                            {
                                MetaDescription = txtMetaDescription.Text,
                                Title = txtHeadTitle.Text,
                                MetaKeywords = txtMetaKeys.Text,
                                H1 = txtH1.Text,
                                Type = MetaType.News
                            }
                    };
                var id = NewsService.InsertNews(news);
                if (FileUpload1.HasFile)
                {
                    var tempName = PhotoService.AddPhoto(new Photo(0, id, PhotoType.News) { OriginName = FileUpload1.FileName });
                    if (!string.IsNullOrWhiteSpace(tempName))
                    {
                        using (Image image = Image.FromStream(FileUpload1.FileContent))
                            FileHelpers.SaveResizePhotoFile(FoldersHelper.GetPathAbsolut(FolderType.News, tempName), SettingsPictureSize.NewsImageWidth, SettingsPictureSize.NewsImageHeight, image);
                    }
                }
                if (lblError.Visible == false)
                {
                    txtTitle.Text = string.Empty;
                    FCKTextToPublication.Text = string.Empty;
                }
                // close
            }
            catch (Exception ex)
            {
                MsgErr(ex.Message + " CreateNews main");
                Debug.LogError(ex);
            }
        }

        private void LoadNewsById(int newsId)
        {
            NewsItem news = NewsService.GetNewsById(newsId);
            if (news == null)
            {
                MsgErr("News with this ID not exist");
                return;
            }
            txtStringID.Text = news.UrlPath;
            txtDate.Text = news.AddingDate.ToShortDateString();
            txtTime.Text = news.AddingDate.ToString("HH:mm");
            txtTitle.Text = news.Title;

            if (news.Picture != null)
            {
                lblImage.Text = news.Picture.PhotoName;
                pnlImage.Visible = true;
                Image1.ImageUrl = FoldersHelper.GetPath(FolderType.News, news.Picture.PhotoName, true);
                Image1.ToolTip = news.Picture.PhotoName;
            }
            else
            {
                lblImage.Text = @"No picture";
                pnlImage.Visible = false;
            }

            FCKTextToPublication.Text = news.TextToPublication;
            CKEditorControlAnnatation.Text = news.TextAnnotation;
            chkOnMainPage.Checked = news.ShowOnMainPage;
            //FCKTextToEmail.Text = news.TextToEmail;

            //hfMetaId.Text = news.MetaId.ToString();

            news.Meta = MetaInfoService.GetMetaInfo(newsId, MetaType.News) ??
                        new MetaInfo(0, 0, MetaType.News, string.Empty, string.Empty, string.Empty, string.Empty);
            txtHeadTitle.Text = news.Meta.Title;
            txtH1.Text = news.Meta.H1;
            txtMetaKeys.Text = news.Meta.MetaKeywords;
            txtMetaDescription.Text = news.Meta.MetaDescription;

            dboNewsCategory.DataBind();
            dboNewsCategory.SelectedValue = news.NewsCategoryID.ToString(CultureInfo.InvariantCulture);
        }

        protected void SqlDataSource1_Init(object sender, EventArgs e)
        {
            SqlDataSource1.ConnectionString = Connection.GetConnectionString();
        }
    }
}