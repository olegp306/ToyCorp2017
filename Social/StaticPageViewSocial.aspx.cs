//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop;
using AdvantShop.CMS;
using AdvantShop.Controls;

namespace Social
{
    public partial class StaticPageView : AdvantShopClientPage
    {
        protected StaticPage page;

        protected void Page_Load(object sender, EventArgs e)
        {
            int pageId = Page.Request["staticpageid"].TryParseInt();
            page = StaticPageService.GetStaticPage(pageId);
            if (pageId == 0 || page == null || (page != null && !page.Enabled))
            {
                Error404();
                return;
            }
            SetMeta(page.Meta, page.PageName);
        }
    }
}