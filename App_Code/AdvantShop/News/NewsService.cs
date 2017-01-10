//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.SEO;
using System;

namespace AdvantShop.News
{
    public class NewsService
    {
        public static void DeleteNewsCategory(int newsCategoryId)
        {
            foreach (var id in GetNewsByCategoryID(newsCategoryId).Select(news => news.NewsID))
                DeleteNews(id);
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[NewsCategory] WHERE NewsCategoryID=@ID",
                                          CommandType.Text, new SqlParameter { ParameterName = "@ID", Value = newsCategoryId });
        }

        public static void DeleteNews(int newsId)
        {
            CacheManager.Remove(CacheNames.GetNewsForMainPage());
            DeleteNewsImage(newsId);
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[News] WHERE NewsID=@ID", CommandType.Text, new SqlParameter { ParameterName = "@ID", Value = newsId });
        }

        public static List<NewsItem> GetNews()
        {
            List<NewsItem> list = SQLDataAccess.ExecuteReadList<NewsItem>("SELECT * FROM [Settings].[News] ORDER BY [AddingDate], [NewsID] DESC", CommandType.Text, GetNewsFromReader);
            return list;
        }

        public static int GetNewsCount()
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT count(NewsID) FROM [Settings].[News]", CommandType.Text);
        }

        public static void DeleteNewsImage(int newsId)
        {
            PhotoService.DeletePhotos(newsId, PhotoType.News);
        }

        public static int GetLastId()
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("SELECT max([NewsID])+1 from [Settings].[News]", CommandType.Text));
        }

        public static IEnumerable<NewsCategory> GetNewsCategories()
        {
            return SQLDataAccess.ExecuteReadIEnumerable(
                "SELECT *, (Select Count(NewsID) FROM [Settings].[News] WHERE NewsCategoryID = [Settings].[NewsCategory].[NewsCategoryID]) as CountNews FROM [Settings].[NewsCategory]",
                CommandType.Text,
                reader => new NewsCategory
                              {
                                  NewsCategoryID = SQLDataHelper.GetInt(reader, "NewsCategoryID"),
                                  Name = SQLDataHelper.GetString(reader, "Name"),
                                  SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                                  CountNews = SQLDataHelper.GetInt(reader, "CountNews"),
                                  UrlPath = SQLDataHelper.GetString(reader, "UrlPath")
                              });
        }

        public static NewsCategory GetNewsCategoryById(int id)
        {
            return SQLDataAccess.ExecuteReadOne<NewsCategory>(
                    "SELECT * FROM [Settings].[NewsCategory] where NewsCategoryID=@NewsCategoryID",
                    CommandType.Text,
                    reader => new NewsCategory
                                  {
                                      NewsCategoryID = SQLDataHelper.GetInt(reader, "NewsCategoryID"),
                                      Name = SQLDataHelper.GetString(reader, "Name"),
                                      SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                                      UrlPath = SQLDataHelper.GetString(reader, "UrlPath")
                                  },
                                  new SqlParameter { ParameterName = "@NewsCategoryID", Value = id }
                                  );
        }

        public static IEnumerable<NewsItem> GetNewsByCategoryID(int categoryID)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<NewsItem>(
                    "SELECT * FROM [Settings].[News] WHERE NewsCategoryID=@NewsCategoryID ORDER BY [AddingDate], [NewsID] DESC",
                    CommandType.Text,
                    GetNewsFromReader,
                    new SqlParameter("@NewsCategoryID", categoryID));
        }

        // get from cache by name of function if it in cache or from db
        public static List<NewsItem> GetNewsForMainPage()
        {
            List<NewsItem> list;
            if (CacheManager.Contains(CacheNames.GetNewsForMainPage()))
                list = CacheManager.Get<List<NewsItem>>(CacheNames.GetNewsForMainPage());
            else
            {
                list = GetNewsForMainPageFromDb();
                CacheManager.Insert(CacheNames.GetNewsForMainPage(), list);
            }
            return list;
        }

        private static List<NewsItem> GetNewsForMainPageFromDb()
        {
            List<NewsItem> list = SQLDataAccess.ExecuteReadList<NewsItem>("SELECT TOP (@count) * FROM [Settings].[News] WHERE [ShowOnMainPage] = \'True\' ORDER BY [AddingDate] DESC, NewsID DESC",
                                                                CommandType.Text, GetNewsFromReader,
                                                                new SqlParameter { ParameterName = "@count", Value = SettingsNews.NewsMainPageCount });
            return list;
        }

        public static int InsertNewsCategory(NewsCategory newsCategory)
        {
            var id = SQLDataAccess.ExecuteScalar<int>("Insert into [Settings].[NewsCategory] ([Name],[SortOrder],[UrlPath]) values (@Name,@SortOrder,@UrlPath); Select SCOPE_IDENTITY ();",
                                                        CommandType.Text,
                                                        new SqlParameter("@Name", newsCategory.Name),
                                                        new SqlParameter("@UrlPath", newsCategory.UrlPath),
                                                        new SqlParameter("@SortOrder", newsCategory.SortOrder));
            if (newsCategory.Meta != null)
            {
                if (!newsCategory.Meta.Title.IsNullOrEmpty() || !newsCategory.Meta.MetaKeywords.IsNullOrEmpty() || !newsCategory.Meta.MetaDescription.IsNullOrEmpty() || !newsCategory.Meta.H1.IsNullOrEmpty())
                {
                    newsCategory.Meta.ObjId = id;
                    MetaInfoService.SetMeta(newsCategory.Meta);
                }
            }
            return id;
        }

        public static void UpdateNewsCategory(NewsCategory newsCategory)
        {
            SQLDataAccess.ExecuteNonQuery("Update [Settings].[NewsCategory] set [Name]=@name,[UrlPath] = @UrlPath , [SortOrder] = @SortOrder where NewsCategoryID = @NewsCategoryID",
                                                       CommandType.Text,
                                                       new SqlParameter("@NewsCategoryID", newsCategory.NewsCategoryID),
                                                       new SqlParameter("@Name", newsCategory.Name),
                                                       new SqlParameter("@UrlPath", newsCategory.UrlPath),
                                                       new SqlParameter("@SortOrder", newsCategory.SortOrder));
            if (newsCategory.Meta != null)
            {
                if (newsCategory.Meta.Title.IsNullOrEmpty() && newsCategory.Meta.MetaKeywords.IsNullOrEmpty() && newsCategory.Meta.MetaDescription.IsNullOrEmpty() && newsCategory.Meta.H1.IsNullOrEmpty())
                {
                    if (MetaInfoService.IsMetaExist(newsCategory.NewsCategoryID, MetaType.NewsCategory))
                        MetaInfoService.DeleteMetaInfo(newsCategory.NewsCategoryID, MetaType.NewsCategory);
                }
                else
                    MetaInfoService.SetMeta(newsCategory.Meta);
            }
        }

        public static int InsertNews(NewsItem news)
        {
            CacheManager.Remove(CacheNames.GetNewsForMainPage());
            var id = SQLDataAccess.ExecuteScalar<int>("[Settings].[sp_AddNews]", CommandType.StoredProcedure,
                                                                     new[]
                                                                            {
                                                                                new SqlParameter("@NewsCategoryID", news.NewsCategoryID),
                                                                                new SqlParameter("@AddingDate", news.AddingDate),
                                                                                new SqlParameter("@Title", news.Title),
                                                                                //new SqlParameter("@Picture", news.Picture),
                                                                                new SqlParameter("@TextToPublication", news.TextToPublication),
                                                                                new SqlParameter("@TextToEmail", news.TextToEmail),
                                                                                new SqlParameter("@TextAnnotation", news.TextAnnotation),
                                                                                new SqlParameter("@ShowOnMainPage", news.ShowOnMainPage),
                                                                                new SqlParameter("@UrlPath", news .UrlPath  )
                                                                            });
            // ---- Meta
            if (news.Meta != null)
            {
                if (!news.Meta.Title.IsNullOrEmpty() || !news.Meta.MetaKeywords.IsNullOrEmpty() || !news.Meta.MetaDescription.IsNullOrEmpty() || !news.Meta.H1.IsNullOrEmpty())
                {
                    news.Meta.ObjId = id;
                    MetaInfoService.SetMeta(news.Meta);
                }
            }
            return id;
        }

        public static bool UpdateNews(NewsItem news)
        {
            CacheManager.Remove(CacheNames.GetNewsForMainPage());

            SQLDataAccess.ExecuteNonQuery("[Settings].[sp_UpdateNews]", CommandType.StoredProcedure,
                                            new[]
                                                        {
                                                            new SqlParameter("@NewsID", news.NewsID),
                                                            new SqlParameter("@NewsCategoryID", news.NewsCategoryID),
                                                            new SqlParameter("@AddingDate", news.AddingDate),
                                                            new SqlParameter("@Title", news.Title),
                                                            //new SqlParameter("@Picture", news.Picture),
                                                            new SqlParameter("@TextToPublication", news.TextToPublication),
                                                            new SqlParameter("@TextToEmail", news.TextToEmail),
                                                            new SqlParameter("@TextAnnotation", news.TextAnnotation),
                                                            new SqlParameter("@ShowOnMainPage", news.ShowOnMainPage),
                                                            new SqlParameter("@UrlPath", news.UrlPath)
                                                        }
                                        );
            if (news.Meta != null)
            {
                if (news.Meta.Title.IsNullOrEmpty() && news.Meta.MetaKeywords.IsNullOrEmpty() && news.Meta.MetaDescription.IsNullOrEmpty() && news.Meta.H1.IsNullOrEmpty())
                {
                    if (MetaInfoService.IsMetaExist(news.ID, MetaType.News))
                        MetaInfoService.DeleteMetaInfo(news.ID, MetaType.News);
                }
                else
                    MetaInfoService.SetMeta(news.Meta);
            }
            return true;
        }

        public static NewsItem GetNewsById(int newsId)
        {
            if (newsId == 0) return null;

            return SQLDataAccess.ExecuteReadOne<NewsItem>("SELECT * FROM [Settings].[News] WHERE NewsID=@NewsID",
                                                            CommandType.Text, GetNewsFromReader,
                                                            new SqlParameter { ParameterName = "@NewsID", Value = newsId });
        }


        private static NewsItem GetNewsFromReader(SqlDataReader reader)
        {
            return new NewsItem
            {
                NewsID = SQLDataHelper.GetInt(reader, "NewsID"),
                NewsCategoryID = SQLDataHelper.GetInt(reader, "NewsCategoryID"),
                Title = SQLDataHelper.GetString(reader, "Title"),
                //Picture = SQLDataHelper.GetString(reader, "Picture"),
                TextToPublication = SQLDataHelper.GetString(reader, "TextToPublication"),
                TextToEmail = SQLDataHelper.GetString(reader, "TextToEmail"),
                TextAnnotation = SQLDataHelper.GetString(reader, "TextAnnotation"),
                ShowOnMainPage = SQLDataHelper.GetBoolean(reader, "ShowOnMainPage"),
                AddingDate = SQLDataHelper.GetDateTime(reader, "AddingDate"),
                UrlPath = SQLDataHelper.GetString(reader, "UrlPath")
            };
        }

        public static void SendNews(string txtTitle, string text)
        {
            foreach (var moduleType in AttachedModules.GetModules<ISendMails>())
            {
                var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                if (ModulesRepository.IsActiveModule(moduleObject.ModuleStringId))
                {
                    moduleObject.SendMails(txtTitle, text, MailRecipientType.Subscriber);
                }
            }
        }

        public static void SetNewsOnMainPage(int newsId, bool showOnMainPage)
        {
            SQLDataAccess.ExecuteNonQuery("Update [Settings].[News] Set ShowOnMainPage = @ShowOnMainPage WHERE NewsID=@NewsID",
                                          CommandType.Text,
                                          new SqlParameter { ParameterName = "@NewsID", Value = newsId },
                                          new SqlParameter { ParameterName = "@ShowOnMainPage", Value = showOnMainPage }
                                          );
        }
        public static void ChangeCategoryNews(int newsId, int newsCategoryId)
        {
            SQLDataAccess.ExecuteNonQuery("Update [Settings].[News] Set NewsCategoryID = @NewsCategoryID WHERE NewsID = @NewsID",
                                           CommandType.Text,
                                           new SqlParameter { ParameterName = "@NewsID", Value = newsId },
                                           new SqlParameter { ParameterName = "@NewsCategoryID", Value = newsCategoryId }
                                           );
        }
    }
}