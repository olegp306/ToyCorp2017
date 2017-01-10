//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using AdvantShop.Controls;
using AdvantShop.News;
using System.Collections.Generic;
using AdvantShop.CMS;
using AdvantShop.Core.UrlRewriter;
using Resources;
using AdvantShop.SEO;

namespace ClientPages
{
    public partial class NewsView : AdvantShopClientPage
    {
        protected string SPhoto;
        protected int NewsID = 0;
        protected MetaInfo metaInfo;
        protected NewsItem NewsItem;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Page.Request["newsid"]))
            {
                this.Error404();
                return;
            }

            int tempId;
            Int32.TryParse(Page.Request["newsid"], out tempId);

            NewsItem = NewsService.GetNewsById(tempId);

            if (NewsItem == null)
            {
                this.Error404();
                return;
            }

            metaInfo = SetMeta(NewsItem.Meta, NewsItem.Title);

            var category = NewsService.GetNewsCategoryById(NewsItem.NewsCategoryID);
            ucBreadCrumbs.Items = new List<BreadCrumbs>
                {
                    new BreadCrumbs
                        {
                            Name = Resource.Client_MasterPage_MainPage,
                            Url = UrlService.GetAbsoluteLink("/")
                        },

                    new BreadCrumbs
                        {
                            Name = Resource.Client_News_News,
                            Url = "news"
                        },
                    new BreadCrumbs
                        {
                            Name = category.Name,
                            Url = category.UrlPath
                        }
                };

            if (GoogleTagManager.Enabled)
            {
                var tagManager = ((AdvantShopMasterPage) Master).TagManager;
                tagManager.PageType = GoogleTagManager.ePageType.info;
            }
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            lvNewsCategories.DataSource = NewsService.GetNewsCategories().Where(item => item.CountNews > 0);
            lvNewsCategories.DataBind();
        }
    }
}