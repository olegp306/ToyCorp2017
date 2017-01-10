//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Helpers;
using AdvantShop.Controls;
using AdvantShop.SEO;
using AdvantShop.Configuration;

namespace ClientPages
{
    public partial class err400 : AdvantShopClientPage //  : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resources.Resource.err400_Title)),
                    string.Empty);
            CommonHelper.DisableBrowserCache();
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = 400;
            Response.Status = "400 Bad Request";
        }
    }
}