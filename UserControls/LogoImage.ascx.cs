//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Trial;
using AdvantShop.CMS;


namespace UserControls
{
    public partial class LogoImage : System.Web.UI.UserControl
    {
        #region  Properties
        public override sealed bool Visible { get; set; }
        public bool EnableHref { get; set; }
        public string ImgAlt { get; set; }
        public string ImgSource { get; set; }
        public string ImgHref { get; set; }
        public string CssClassHref { get; set; }
        public string CssClassImage { get; set; }
        public string Title { get; set; }
        #endregion

        public LogoImage()
        {
            ImgSource = FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, false);
            EnableHref = true;
            Visible = true;
            ImgAlt = HttpUtility.HtmlEncode(SettingsMain.LogoImageAlt);
        }

        public string RenderHtml()
        {
            string resStrHtml = string.Empty;
            const string aTag = "<a href=\"{0}\" {1} {2}>{3}</a>";
            const string imgTag = "<img id=\"logo\" src=\"{0}\" {1} {2} {3}/>";


            //Source

            if (SettingsMain.LogoImageName.IsNullOrEmpty() && InplaceEditor.CanUseInplace(RoleActionKey.DisplayCommonSettings))
            {
                ImgSource = "images/nophoto-logo.png";
            }
            else if (SettingsMain.LogoImageName.IsNullOrEmpty())
            {
                return string.Empty;
            }

            //Alt
            string alt = !string.IsNullOrEmpty(ImgAlt) ? string.Format("alt=\"{0}\"", ImgAlt) : string.Empty;
            //CssClass
            string cssClass = string.Format("class=\"{0}{1}\"",
                (!string.IsNullOrEmpty(CssClassImage) ? CssClassImage : string.Empty),
                InplaceEditor.CanUseInplace(RoleActionKey.DisplayCommonSettings)? " js-inplace-image-visible-permanent" : string.Empty);
            //Result
            resStrHtml = string.Format(imgTag, ImgSource, alt, cssClass, AdvantShop.CMS.InplaceEditor.Image.AttributesLogo());
            //---------------------------------------------------------------------------

            //Creating href tag----------------------------------------------------------
            if (EnableHref && !string.IsNullOrEmpty(ImgHref))
            {
                //Href
                string href = UrlService.GetAbsoluteLink(ImgHref);
                //Title
                string title = !string.IsNullOrEmpty(Title) ? string.Format("title=\"{0}\"", Title) : string.Empty;
                //CssClass
                cssClass = !string.IsNullOrEmpty(CssClassHref) ? string.Format("class=\"{0}\"", CssClassHref) : string.Empty;
                //Href
                string image = resStrHtml;
                //Result
                resStrHtml = string.Format(aTag, href, title, cssClass, image);
            }
            //----------------------------------------------------------------------------

            return resStrHtml;
        }

    }
}