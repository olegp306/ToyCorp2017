//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.News;
using AdvantShop.SEO;
using AdvantShop.CMS;
using AdvantShop.Core.UrlRewriter;
using Resources;

namespace ClientPages
{
    public partial class News : AdvantShopClientPage
    {
        private SqlPaging _paging;

        protected void Page_Load(object sender, EventArgs e)
        {
            bool categoryIdIsNum = false;
            int categoryId = -1;
            if (!string.IsNullOrEmpty(Request["newscategoryid"]) &&
                Int32.TryParse(Request["newscategoryid"], out categoryId))
            {
                categoryIdIsNum = categoryId != -1;
            }

            _paging = new SqlPaging
                {
                    TableName = "Settings.News left join Catalog.Photo on Photo.objId=News.NewsID and Type=@Type",
                    ItemsPerPage = SettingsNews.NewsPerPage
                };

            _paging.AddField(new Field {Name = "NewsID"});

            _paging.AddField(new Field {Name = "AddingDate", Sorting = SortDirection.Descending});

            _paging.AddField(new Field {Name = "Title"});

            _paging.AddField(new Field {Name = "PhotoName as Picture"});

            _paging.AddField(new Field {Name = "TextToPublication"});

            _paging.AddField(new Field {Name = "TextToEmail"});

            _paging.AddField(new Field {Name = "TextAnnotation"});

            _paging.AddField(new Field {Name = "UrlPath"});

            _paging.AddParam(new SqlParam {ParameterName = "@Type", Value = PhotoType.News.ToString()});

            if (categoryIdIsNum)
            {
                var f = new Field {Name = "NewsCategoryID", NotInQuery = true};
                var filter = new EqualFieldFilter
                    {
                        ParamName = "@NewsCategoryID",
                        Value = categoryId.ToString(CultureInfo.InvariantCulture)
                    };
                f.Filter = filter;
                _paging.AddField(f);
            }

           
            var category = NewsService.GetNewsCategoryById(categoryId);
            
            if (category != null)
            {
                ucBreadCrumbs.Items = new List<BreadCrumbs>
                    {
                        new BreadCrumbs
                            {
                                Name = Resources.Resource.Client_News_News,
                                Url = UrlService.GetAbsoluteLink("news")
                            },
                        new BreadCrumbs
                            {
                                Name = category.Name,
                                Url =
                                    UrlService.GetLink(ParamType.NewsCategory, category.UrlPath, category.NewsCategoryID)
                            }
                    };
            }
            else
            {
                ucBreadCrumbs.Visible = false;
            }

            MetaInfo meta = new MetaInfo
            {
                Type = MetaType.News,
                Title = SettingsNews.NewsMetaTitle,
                MetaKeywords = SettingsNews.NewsMetaKeywords,
                MetaDescription = SettingsNews.NewsMetaDescription,
                H1 = SettingsNews.NewsMetaH1
            };

            if (category != null && category.Meta != null)
            {
                meta = category.Meta;
            }
            
            meta = SetMeta(meta, category != null ? category.Name: string.Empty, page: paging.CurrentPage);
            header.Text = category != null ? meta.H1 : Resource.Client_News_News;

            if (GoogleTagManager.Enabled)
            {
                var tagManager = ((AdvantShopMasterPage)Master).TagManager;
                tagManager.PageType = GoogleTagManager.ePageType.info;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            _paging.ItemsPerPage = paging.CurrentPage != 0 ? SettingsNews.NewsPerPage : int.MaxValue;
            _paging.CurrentPageIndex = paging.CurrentPage != 0 ? paging.CurrentPage : 1;
            paging.TotalPages = _paging.PageCount;

            if (paging.TotalPages < paging.CurrentPage || paging.CurrentPage < 0)
            {
                Error404();
                return;
            }

            lvNews.DataSource = _paging.PageItems;
            lvNews.DataBind();
            paging.TotalPages = _paging.PageCount;


            lvNewsCategories.DataSource = NewsService.GetNewsCategories().Where(item => item.CountNews > 0);
            lvNewsCategories.DataBind();
        }
    }
}