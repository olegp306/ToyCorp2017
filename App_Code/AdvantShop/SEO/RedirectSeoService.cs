//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.SEO
{
    public static class RedirectSeoService
    {
        private const string RedirectsCahcheName = "redirectsCache";

        private static void CleanRedirectsCahche()
        {
            CacheManager.Remove(RedirectsCahcheName);
        }

        public static RedirectSeo GetRedirectSeoById(int id)
        {
            return SQLDataAccess.ExecuteReadOne<RedirectSeo>("SELECT * FROM [Settings].[Redirect] WHERE ID = @ID",
                                                                                CommandType.Text, GetRedirectSeoFromReader,
                                                                                new SqlParameter { ParameterName = "@ID", Value = id });
        }

        public static IEnumerable<RedirectSeo> GetRedirectsSeo()
        {
            return SQLDataAccess.ExecuteReadIEnumerable<RedirectSeo>("SELECT * FROM [Settings].[Redirect]", CommandType.Text, GetRedirectSeoFromReader);
        }

        public static RedirectSeo GetByInputUrl(string relativeUrl, string absoluteUrl)
        {
            List<RedirectSeo> redirects;
            if (CacheManager.Contains(RedirectsCahcheName))
            {
                redirects = CacheManager.Get<List<RedirectSeo>>(RedirectsCahcheName);
            }
            else
            {
                redirects = GetRedirectsSeo().ToList();
                CacheManager.Insert(RedirectsCahcheName, redirects, 60);
            }
            var result = redirects.FirstOrDefault(r => ImitateSqlLike(relativeUrl, r.RedirectFrom.ToLower()) || ImitateSqlLike(absoluteUrl, r.RedirectFrom.ToLower()));
            return result != null
                ? new RedirectSeo
                {
                    ID = result.ID,
                    ProductArtNo = result.ProductArtNo,
                    RedirectFrom = result.RedirectFrom.Replace("*", "").Replace("*", ""),
                    RedirectTo = result.RedirectTo
                } : null;

            //return SQLDataAccess.ExecuteReadOne<RedirectSeo>("SELECT ID, Replace(RedirectFrom, '*', '') as RedirectFrom, RedirectTo, ProductArtNo  FROM [Settings].[Redirect] where @relativeUrl like Replace(RedirectFrom, '*', '%') or @absoluteUrl like Replace(RedirectFrom, '*', '%')", CommandType.Text,
            //    GetRedirectSeoFromReader, new SqlParameter("@relativeUrl", relativeUrl.Trim()), new SqlParameter("@absoluteUrl", absoluteUrl.Trim()));
        }

        private static bool ImitateSqlLike(string input, string source)
        {
            source = source.Trim('?');
            if (source.Contains('*'))
            {
                var cleanSource = source.Replace("*", "");
                if (source.IndexOf('*') == 0)
                    return input.EndsWith(cleanSource);
                if (source.IndexOf('*') == source.Length - 1)
                    return input.StartsWith(cleanSource);
                return input.Contains(cleanSource);
            }
            return input == source;
        }

        private static RedirectSeo GetRedirectSeoFromReader(SqlDataReader reader)
        {
            return new RedirectSeo
            {
                ID = SQLDataHelper.GetInt(reader, "ID"),
                RedirectFrom = SQLDataHelper.GetString(reader, "RedirectFrom"),
                RedirectTo = SQLDataHelper.GetString(reader, "RedirectTo"),
                ProductArtNo = SQLDataHelper.GetString(reader, "ProductArtNo")
            };
        }

        public static RedirectSeo GetRedirectsSeoByRedirectFrom(string redirectFrom)
        {
            return SQLDataAccess.ExecuteReadOne<RedirectSeo>(
                "SELECT * FROM [Settings].[Redirect] WHERE RedirectFrom = @RedirectFrom",
                CommandType.Text,
                GetRedirectSeoFromReader,
                new SqlParameter("@RedirectFrom", redirectFrom));
        }

        public static void AddRedirectSeo(RedirectSeo redirectSeo)
        {
            CleanRedirectsCahche();
            redirectSeo.ID =
                SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Settings].[Redirect] ([RedirectFrom], [RedirectTo], [ProductArtNo]) VALUES (@RedirectFrom, @RedirectTo, @ProductArtNo); SELECT SCOPE_IDENTITY();",
                    CommandType.Text,
                    new SqlParameter("@RedirectFrom", redirectSeo.RedirectFrom),
                    new SqlParameter("@RedirectTo", redirectSeo.RedirectTo),
                    new SqlParameter("@ProductArtNo", redirectSeo.ProductArtNo)
                    );
        }

        public static void UpdateRedirectSeo(RedirectSeo redirectSeo)
        {
            CleanRedirectsCahche();
            SQLDataAccess.ExecuteNonQuery("UPDATE [Settings].[Redirect] SET RedirectFrom = @RedirectFrom, RedirectTo = @RedirectTo, ProductArtNo = @ProductArtNo WHERE ID = @ID", CommandType.Text,
                                                new SqlParameter("@ID", redirectSeo.ID),
                                                new SqlParameter("@RedirectFrom", redirectSeo.RedirectFrom),
                                                new SqlParameter("@RedirectTo", redirectSeo.RedirectTo),
                                                new SqlParameter("@ProductArtNo", redirectSeo.ProductArtNo)
                                         );
        }

        public static void DeleteRedirectSeo(int id)
        {
            CleanRedirectsCahche();
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Settings].[Redirect] WHERE ID = @ID", CommandType.Text,
                                          new SqlParameter { ParameterName = "@ID", Value = id });
        }
    }
}