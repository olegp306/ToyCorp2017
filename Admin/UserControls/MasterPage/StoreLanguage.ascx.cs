//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Localization;
using System.Globalization;
using Resources;

namespace Admin.UserControls.MasterPage
{
    public partial class StoreLanguage : System.Web.UI.UserControl
    {
        protected void lnkEnglishLanguage_Click(object sender, EventArgs e)
        {
            Culture.Language = Culture.SupportLanguage.English;
            Response.Redirect(Request.RawUrl);
        }

        protected void lnkRussianLanguage_Click(object sender, EventArgs e)
        {
            Culture.Language = Culture.SupportLanguage.Russian;
            Response.Redirect(Request.RawUrl);
        }

        protected void lnkUkrainianLanguage_Click(object sender, EventArgs e)
        {
            Culture.Language = Culture.SupportLanguage.Ukrainian;
            Response.Redirect(Request.RawUrl);
        }
        
        protected string RenderLanguageImage()
        {
            string result = "<img src=\"images/new_admin/lang/" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + ".jpg\" class=\"lang-selected\" alt=\"{0}\"/>";

            switch (Culture.Language)
            {
                case Culture.SupportLanguage.English:
                    return string.Format(result, Resource.Global_Language_English);
                case Culture.SupportLanguage.Russian:
                    return string.Format(result, Resource.Global_Language_Russian);
                case Culture.SupportLanguage.Ukrainian:
                    return string.Format(result, Resource.Global_Language_Ukrainian);
                default:
                    return string.Empty;
            }
        }
    }
}