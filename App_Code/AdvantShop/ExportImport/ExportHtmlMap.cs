//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Resources;

namespace AdvantShop.ExportImport
{
    public class ExportHtmlMap
    {
        private readonly string _filenameAndPath;
        private readonly Encoding _encoding;
        private string _prefUrl;
        private StreamWriter _sw;

        public ExportHtmlMap(string filenameAndPath, string prefUrl, Encoding encoding)
        {
            _filenameAndPath = filenameAndPath;
            _prefUrl = prefUrl;
            _encoding = encoding;
        }

        public bool Create()
        {
            try
            {
                var path = Path.GetDirectoryName(_filenameAndPath);
                if (path == null) return false;

                FileHelpers.CreateDirectory(path);
                FileHelpers.DeleteFile(_filenameAndPath);

                _prefUrl = _prefUrl.Contains("http://") || _prefUrl.Contains("https://") ? _prefUrl : "http://" + _prefUrl;

                using (var fs = new FileStream(_filenameAndPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    //using (_sw = new StreamWriter(_filenameAndPath, false, _encoding))
                    using (_sw = new StreamWriter(fs, _encoding))
                    {
                        _sw.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>" + _prefUrl + " - " + Resource.Admin_SiteMapGenerate_Header + "</title></head><body><div>");

                        _sw.WriteLine("<b><a href='{0}'>{0}</a></b><br/><br/>", SettingsMain.SiteUrl);
                        CreateAux();
                        CreateCategory();
                        if (SettingsDesign.NewsVisibility)
                            CreateNews();
                        CreateBrands();

                        _sw.WriteLine("</div></body></html>");

                        _sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, TaskManager.TaskManagerInstance().GetTasks());
                return false;
            }
            return true;
        }

        /// <summary>
        /// write aux to file directly
        /// </summary>
        private void CreateAux()
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "SELECT [StaticPageID], [PageName],[UrlPath] FROM [CMS].[StaticPage] WHERE [IndexAtSiteMap] = 1 and enabled=1 ORDER BY [SortOrder]";
                db.cnOpen();
                using (var read = db.cmd.ExecuteReader())
                {
                    bool tempHaveItem = read.HasRows;

                    if (tempHaveItem)
                        _sw.WriteLine("<b>" + Resource.Client_Sitemap_StaticPages + " </b> <ul>");

                    while (read.Read())
                        _sw.WriteLine("<li><a href='{0}'>{1}</a></li>", _prefUrl + UrlService.GetLink(ParamType.StaticPage, SQLDataHelper.GetString(read["UrlPath"]), SQLDataHelper.GetInt(read["StaticPageID"])), read["PageName"]);

                    if (tempHaveItem)
                        _sw.WriteLine("</ul>");
                }
            }
        }

        private void CreateCategory()
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "SELECT [CategoryID], [Name], [ParentCategory],[UrlPath] FROM [Catalog].[Category] WHERE [Enabled] = 1 and HirecalEnabled=1 and ParentCategory=0 and CategoryID<>0 AND [Products_Count] > 0 ORDER BY [SortOrder]";
                db.cnOpen();
                using (var read = db.cmd.ExecuteReader())
                {
                    bool tempHaveItem = read.HasRows;

                    if (tempHaveItem)
                        _sw.WriteLine("<b>" + Resource.Client_Sitemap_Catalog + "</b><ul>");

                    while (read.Read())
                    {
                        if (!read["CategoryID"].ToString().Trim().Equals("0"))
                        {
                            _sw.WriteLine("<li><a href='{0}'>{1}</a>", _prefUrl + UrlService.GetLink(ParamType.Category, SQLDataHelper.GetString(read["UrlPath"]), SQLDataHelper.GetInt(read["CategoryID"])), read["Name"]);
                            GetSubCategories((int)read["CategoryID"]);
                            GetProducts((int)read["CategoryID"]);
                            _sw.WriteLine("</li>");
                        }
                    }

                    if (tempHaveItem)
                        _sw.WriteLine("</ul>");
                }
            }
        }

        private void GetSubCategories(int categoryId)
        {
            _sw.WriteLine("<ul>");
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "SELECT [CategoryID], [Name], [ParentCategory],[UrlPath] FROM [Catalog].[Category] WHERE [Enabled] = 1 and HirecalEnabled=1 and ParentCategory=@categoryId AND [Products_Count] > 0 ORDER BY [SortOrder]";
                db.cmd.Parameters.AddWithValue("@categoryId", categoryId);
                db.cnOpen();
                using (var read = db.cmd.ExecuteReader())
                {
                    while (read.Read())
                    {
                        _sw.WriteLine("<li>");
                        _sw.WriteLine("<a href='{0}'>{1}</a>", _prefUrl + UrlService.GetLink(ParamType.Category, SQLDataHelper.GetString(read["UrlPath"]), SQLDataHelper.GetInt(read["CategoryID"])), HttpUtility.HtmlEncode(read["Name"]));
                        GetSubCategories((int)read["CategoryID"]);
                        GetProducts((int)read["CategoryID"]);
                        _sw.WriteLine("</li>");
                    }
                }
            }
            _sw.WriteLine("</ul>");
        }

        private void GetProducts(int categoryId)
        {
            _sw.WriteLine("<ul>");
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "SELECT Product.[ProductID] as ProductID, Product.[Name] as Name,ProductCategories.CategoryID as ParentCategory,[UrlPath]  FROM [Catalog].[ProductCategories]" +
                                     " INNER JOIN [Catalog].[Product] ON [Product].ProductID = ProductCategories.ProductID WHERE CategoryEnabled =1 and [Enabled] = 1 and ProductCategories.Main = 1 " +
                                     "and (select Count(CategoryID) from Catalog.ProductCategories where ProductID=[Product].[ProductID])<> 0 and CategoryID=@categoryId ";
                db.cmd.Parameters.AddWithValue("@categoryId", categoryId);
                db.cnOpen();
                using (var read = db.cmd.ExecuteReader())
                    while (read.Read())
                        _sw.WriteLine("<li><a href='{0}'>{1}</a></li>", _prefUrl + UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(read["UrlPath"]), SQLDataHelper.GetInt(read["ProductID"])), HttpUtility.HtmlEncode(read["Name"]));
            }
            _sw.WriteLine("</ul>");
        }

        private void CreateNews()
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "SELECT [NewsID], [Title], [AddingDate],[UrlPath] FROM [Settings].[News] ORDER BY AddingDate DESC";
                db.cnOpen();
                using (var read = db.cmd.ExecuteReader())
                {
                    bool tempHaveItem = read.HasRows;

                    if (tempHaveItem)
                        _sw.WriteLine("<b>" + Resource.Client_Sitemap_News + " </b> <ul>");

                    while (read.Read())
                        _sw.WriteLine("<li><a href='{0}'>{1}</a></li>", _prefUrl + UrlService.GetLink(ParamType.News, SQLDataHelper.GetString(read["UrlPath"]), SQLDataHelper.GetInt(read["NewsID"])), read["AddingDate"] + " :: " + read["Title"]);

                    if (tempHaveItem)
                        _sw.WriteLine("</ul>");
                }
            }
        }

        private void CreateBrands()
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "SELECT [BrandName], [BrandID], [UrlPath] FROM [Catalog].[Brand] Where enabled=1 ORDER BY BrandName";
                db.cnOpen();
                using (var read = db.cmd.ExecuteReader())
                {
                    bool tempHaveItem = read.HasRows;

                    if (tempHaveItem)
                        _sw.WriteLine("<b>" + Resource.Client_Sitemap_Brands + " </b> <ul>");

                    while (read.Read())
                        _sw.WriteLine("<li><a href='{0}'>{1}</a></li>",
                                      _prefUrl +
                                      UrlService.GetLink(ParamType.Brand, SQLDataHelper.GetString(read["UrlPath"]),
                                                         SQLDataHelper.GetInt(read["BrandID"])), read["BrandName"]);

                    if (tempHaveItem)
                        _sw.WriteLine("</ul>");
                }
            }
        }

    }
}