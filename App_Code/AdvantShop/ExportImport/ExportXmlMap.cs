//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.SEO;

namespace AdvantShop.ExportImport
{
    public class ExportXmlMap
    {
        private readonly string _strPhysicalTargetFolder;
        private readonly string _filenameAndPath;

        private const string DefaultChangeFreq = "daily";

        private const float DefaultPriority = 0.5f;
        private const int MaxUrlCount = 50000;
        private const int StepFileLength = 30000;


        public ExportXmlMap(string filenameAndPath, string strPhysicalTargetFolder)
        {
            _filenameAndPath = filenameAndPath;
            _strPhysicalTargetFolder = strPhysicalTargetFolder;
        }

        public void Create()
        {
            try
            {
                DeleteOldFiles();
                GenerateSiteMap();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, TaskManager.TaskManagerInstance().GetTasks());
            }
        }

        private void DeleteOldFiles()
        {
            var path = Path.GetDirectoryName(_filenameAndPath);
            if (path != null)
            {
                var dir = new DirectoryInfo(path);
                foreach (var item in dir.GetFiles())
                {
                    if (item.Name.Contains("sitemap") && item.Name.Contains(".xml"))
                    {
                        FileHelpers.DeleteFile(item.FullName);
                    }
                }
            }
        }

        private int GetCount()
        {
            var totalCount = 0;
            totalCount += SQLDataAccess.ExecuteScalar<int>("SELECT Count([CategoryId]) FROM [Catalog].[Category] WHERE [Enabled] = 1 and HirecalEnabled=1 and CategoryID <> 0", CommandType.Text);
            totalCount += SQLDataAccess.ExecuteScalar<int>("SELECT Count([Product].[ProductID]) " +
                    " FROM [Catalog].[Product] INNER JOIN [Catalog].[ProductCategories] ON [Catalog].[Product].[ProductID] = [Catalog].[ProductCategories].[ProductID]" +
                    " INNER JOIN [Catalog].[Category] ON [Catalog].[Category].[CategoryID] = [Catalog].[ProductCategories].[CategoryID] AND [Catalog].[Category].[Enabled] = 1 " +
                    " WHERE [Product].[Enabled] = 1 and [Product].[CategoryEnabled]=1 and (select Count(CategoryID) from Catalog.ProductCategories where ProductID=[Product].[ProductID]) <> 0;"
                    , CommandType.Text);
            if (SettingsDesign.NewsVisibility)
                totalCount += SQLDataAccess.ExecuteScalar<int>("SELECT Count([NewsID]) FROM [Settings].[News]", CommandType.Text);
            totalCount += SQLDataAccess.ExecuteScalar<int>("SELECT Count([StaticPageID]) FROM [CMS].[StaticPage] where IndexAtSiteMap=1 and enabled=1", CommandType.Text);
            totalCount += SQLDataAccess.ExecuteScalar<int>("SELECT Count([BrandID]) FROM [Catalog].[Brand] where enabled=1", CommandType.Text);
            return totalCount;
        }

        private IEnumerable<SiteMapData> GetData()
        {
            string sqlcommand = "SELECT [CategoryId] as Id, [UrlPath], 'category' as Type, GetDate() as Lastmod FROM [Catalog].[Category] WHERE [Enabled] = 1 and HirecalEnabled =1 and CategoryID <> 0";
            sqlcommand += " union ";
            sqlcommand += @" SELECT [Product].[ProductID] as Id , [Product].[UrlPath], 'product' as Type ,[Product].[DateModified] as Lastmod FROM [Catalog].[Product] " +
                            "INNER JOIN [Catalog].[ProductCategories] ON [Catalog].[Product].[ProductID] = [Catalog].[ProductCategories].[ProductID] " +
                            "INNER JOIN [Catalog].[Category] ON [Catalog].[Category].[CategoryID] = [Catalog].[ProductCategories].[CategoryID] " +
                            "AND [Catalog].[Category].[Enabled] = 1 WHERE [Product].[Enabled] = 1 and [Product].[CategoryEnabled] = 1 and (select Count(CategoryID) from Catalog.ProductCategories where ProductID=[Product].[ProductID])<> 0 ";
            if (SettingsDesign.NewsVisibility)
            {
                sqlcommand += " union ";
                sqlcommand +=
                    "SELECT [NewsID] as Id, News.[UrlPath], 'news' as Type , [AddingDate] as LastMod FROM [Settings].[News]";
            }
            sqlcommand += " union ";
            sqlcommand += "SELECT [StaticPageID] as Id, [UrlPath], 'page' as Type, [ModifyDate] as Lastmod FROM [CMS].[StaticPage] where IndexAtSiteMap=1 and enabled=1";
            sqlcommand += " union ";
            sqlcommand += "SELECT [BrandID] as Id, [UrlPath], 'brand' as Type, GetDate() as Lastmod FROM [Catalog].[Brand] where enabled=1";

            return SQLDataAccess.ExecuteReadList(sqlcommand, CommandType.Text, GetSiteMapDataFromReader);
        }

        private void GenerateSiteMap()
        {
            var totalCount = GetCount();
            var data = GetData();
            if (totalCount > MaxUrlCount)
            {
                int intervals = totalCount / StepFileLength;
                if (totalCount % StepFileLength > 0)
                    intervals += 1;
                CreateMultipleXml(intervals, _filenameAndPath, data);
            }
            else
            {
                CreateSimpleXml(_filenameAndPath, data);
            }
        }

        private void CreateMultipleXml(int intervals, string strFinalFilePath, IEnumerable<SiteMapData> data)
        {
            CreateXmlMap(intervals, strFinalFilePath);
            CreateXmlFiles(intervals, strFinalFilePath, data);
        }

        /// <summary>
        /// create xml file of all catalog
        /// </summary>
        private void CreateXmlFiles(int intervals, string strFinalFilePath, IEnumerable<SiteMapData> data)
        {
            string fname = strFinalFilePath.Replace(".xml", "");
            for (int i = 0; i < intervals; i++)
            {
                string filePath = string.Format("{0}_{1}.xml", fname, i);
                var temp = data.Skip(StepFileLength * i).Take(StepFileLength);
                WriteFile(filePath, temp, i == 0);
            }
        }


        /// <summary>
        /// create xml mapping
        /// </summary>
        /// <param name="intervals"></param>
        /// <param name="strFinalFilePath"></param>
        private void CreateXmlMap(int intervals, string strFinalFilePath)
        {
            string fname = strFinalFilePath.Replace(".xml", "");
            using (var outputFile = new StreamWriter(strFinalFilePath, false, new UTF8Encoding(false)))
            {
                using (var writer = XmlWriter.Create(outputFile))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("sitemapindex", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    for (int i = 0; i < intervals; i++)
                    {
                        string filePath = string.Format("{0}_{1}.xml", fname, i);
                        writer.WriteStartElement("sitemap");

                        writer.WriteStartElement("loc");
                        writer.WriteString(SettingsMain.SiteUrl + "/" + filePath.Split('\\').Last());
                        writer.WriteEndElement();

                        writer.WriteStartElement("lastmod");
                        writer.WriteString(DateTime.Now.ToString("yyyy-MM-dd"));
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        private void CreateSimpleXml(string fileName, IEnumerable<SiteMapData> data)
        {
            WriteFile(fileName, data);
        }


        private void WriteFile(string filePath, IEnumerable<SiteMapData> data, bool isFirst = true)
        {
            using (var outputFile = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (var writer = XmlWriter.Create(outputFile))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    if (isFirst)
                    {
                        // adding link to main page
                        WriteLine(new SiteMapData()
                        {
                            Loc = SettingsMain.SiteUrl,
                            Lastmod = DateTime.Now,
                            Changefreq = DefaultChangeFreq,
                            Priority = DefaultPriority
                        }, writer);
                    }

                    foreach (var item in data)
                    {
                        WriteLine(item, writer);
                    }
                }
            }
        }

        /// <summary>
        /// write kine to xml
        /// </summary>
        /// <param name="item"></param>
        /// <param name="writer"></param>
        private void WriteLine(SiteMapData item, XmlWriter writer)
        {
            writer.WriteStartElement("url");
            // url -------------

            writer.WriteStartElement("loc");
            writer.WriteString(item.Loc);
            writer.WriteEndElement();

            writer.WriteStartElement("lastmod");
            writer.WriteString(item.Lastmod.ToString("yyyy-MM-dd"));
            writer.WriteEndElement();

            writer.WriteStartElement("changefreq");
            writer.WriteString(item.Changefreq);
            writer.WriteEndElement();

            writer.WriteStartElement("priority");
            writer.WriteString(item.Priority.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            // url -------------
            writer.WriteEndElement();
        }

        /// <summary>
        /// return data from reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static SiteMapData GetSiteMapDataFromReader(SqlDataReader reader)
        {
            var prefUrl = SettingsMain.SiteUrl + "/";
            var siteMapData = new SiteMapData
            {
                Changefreq = DefaultChangeFreq,
                Priority = DefaultPriority
            };

            if (SQLDataHelper.GetString(reader, "Type").ToLower() == "category")
            {
                siteMapData.Loc = prefUrl + UrlService.GetLink(ParamType.Category, SQLDataHelper.GetString(reader, "UrlPath"), SQLDataHelper.GetInt(reader, "Id"));
                siteMapData.Lastmod = DateTime.Now;
            }
            else if (SQLDataHelper.GetString(reader, "Type").ToLower() == "product")
            {
                siteMapData.Loc = prefUrl + UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(reader, "UrlPath"), SQLDataHelper.GetInt(reader, "Id"));
                siteMapData.Lastmod = SQLDataHelper.GetDateTime(reader, "Lastmod");
            }
            else if (SQLDataHelper.GetString(reader, "Type").ToLower() == "news")
            {
                siteMapData.Loc = prefUrl + UrlService.GetLink(ParamType.News, SQLDataHelper.GetString(reader, "UrlPath"), SQLDataHelper.GetInt(reader, "Id"));
                siteMapData.Lastmod = SQLDataHelper.GetDateTime(reader, "Lastmod");
            }
            else if (SQLDataHelper.GetString(reader, "Type").ToLower() == "page")
            {
                siteMapData.Loc = prefUrl + UrlService.GetLink(ParamType.StaticPage, SQLDataHelper.GetString(reader, "UrlPath"), SQLDataHelper.GetInt(reader, "Id"));
                siteMapData.Lastmod = SQLDataHelper.GetDateTime(reader, "Lastmod");
            }

            else if (SQLDataHelper.GetString(reader, "Type").ToLower() == "brand")
            {
                siteMapData.Loc = prefUrl + UrlService.GetLink(ParamType.Brand, SQLDataHelper.GetString(reader, "UrlPath"), SQLDataHelper.GetInt(reader, "Id"));
                siteMapData.Lastmod = DateTime.Now;
            }

            return siteMapData;
        }

    }
}