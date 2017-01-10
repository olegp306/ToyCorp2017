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
    public partial class err404 : AdvantShopClientPage //  : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resources.Resource.err404_Title)),
                    string.Empty);
            CommonHelper.DisableBrowserCache();
            Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = 404;
            Response.Status = "404 Not Found";
        }
    }
}