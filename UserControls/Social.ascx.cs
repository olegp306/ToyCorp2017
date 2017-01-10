using AdvantShop.Configuration;
using AdvantShop.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace UserControls
{
    public partial class Social : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = SettingsDesign.EnableSocialShareButtons;


            if (SettingsSocial.SocialShareCustomEnabled)
            {
                mvSocial.SetActiveView(vSocialCustom);
            }
            else
            {
                switch (Culture.Language)
                {
                    case Culture.SupportLanguage.English:
                        mvSocial.SetActiveView(vSocialDefault_En);
                        break;
                    case Culture.SupportLanguage.Russian:
                        mvSocial.SetActiveView(vSocialDefault_Ru);
                        break;
                }
            }
        }
    }
}