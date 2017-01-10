//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.SEO;

namespace AdvantShop.Catalog
{

    public struct BrandProductCount
    {
        public int InCategoryCount { set; get; }
        public int InChildsCategoryCount { set; get; }
    }

    public class BrandService
    {
        public static List<char> GetEngBrandChars()
        {
            return new List<char>(){ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                                         'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        }

        public static List<char> GetRusBrandChars()
        {
            return new List<char>(){ 'а', 'б', 'в', 'г', 'д', 'е', 'Є', 'ж', 'з', 'и','й', 'к', 'л', 'м', 'н', 'о',
                                         'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'э', 'ю', '€'};
        }

        #region Get Add Update Delete

        public static void DeleteBrand(int brandId)
        {
            DeleteBrandLogo(brandId);
            SQLDataAccess.ExecuteNonQuery("Delete From Catalog.Brand Where BrandID=@BrandID", CommandType.Text,
                                            new SqlParameter { ParameterName = "@BrandId", Value = brandId });
        }


        public static int AddBrand(Brand brand)
        {
            brand.BrandId = SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                "[Catalog].[sp_AddBrand]",
                CommandType.StoredProcedure,
                new SqlParameter("@BrandName", brand.Name),
                new SqlParameter("@BrandDescription", brand.Description ?? (object)DBNull.Value),
                new SqlParameter("@BrandBriefDescription", brand.BriefDescription ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", brand.Enabled),
                new SqlParameter("@SortOrder", brand.SortOrder),
                new SqlParameter("@CountryID", brand.CountryId == 0 ? (object)DBNull.Value : brand.CountryId),
                new SqlParameter("@UrlPath", brand.UrlPath),
                new SqlParameter("@BrandSiteUrl", brand.BrandSiteUrl.IsNotEmpty() ? brand.BrandSiteUrl : (object)DBNull.Value)
                ));

            if (brand.BrandId == 0)
                return 0;

            // ---- Meta
            if (brand.Meta != null)
            {
                brand.Meta.ObjId = brand.BrandId;
                MetaInfoService.SetMeta(brand.Meta);
            }
            return brand.BrandId;
        }

        public static void UpdateBrand(Brand brand)
        {

            SQLDataAccess.ExecuteNonQuery(
                "[Catalog].[sp_UpdateBrandById]",
                CommandType.StoredProcedure,
                new SqlParameter("@BrandID", brand.BrandId),
                new SqlParameter("@BrandName", brand.Name),
                new SqlParameter("@BrandDescription", brand.Description ?? (object)DBNull.Value),
                new SqlParameter("@BrandBriefDescription", brand.BriefDescription ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", brand.Enabled),
                new SqlParameter("@SortOrder", brand.SortOrder),
                new SqlParameter("@CountryID", brand.CountryId == 0 ? (object)DBNull.Value : brand.CountryId),
                new SqlParameter("@UrlPath", brand.UrlPath),
                new SqlParameter("@BrandSiteUrl", brand.BrandSiteUrl.IsNotEmpty() ? brand.BrandSiteUrl : (object)DBNull.Value)
                );

            MetaInfoService.SetMeta(brand.Meta);
        }

        private static Brand GetBrandFromReader(SqlDataReader reader)
        {
            return new Brand
                       {
                           BrandId = SQLDataHelper.GetInt(reader, "BrandId"),
                           Name = SQLDataHelper.GetString(reader, "BrandName"),
                           Description = SQLDataHelper.GetString(reader, "BrandDescription", string.Empty),
                           BriefDescription = SQLDataHelper.GetString(reader, "BrandBriefDescription", string.Empty),
                           //BrandLogo = SQLDataHelper.GetString(reader, "BrandLogo"),
                           Enabled = SQLDataHelper.GetBoolean(reader, "Enabled", true),
                           SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                           CountryId = SQLDataHelper.GetInt(reader, "CountryID", 0),
                           UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                           BrandSiteUrl = SQLDataHelper.GetString(reader, "BrandSiteUrl")
                       };
        }

        public static Brand GetBrandById(int brandId)
        {
            return SQLDataAccess.ExecuteReadOne<Brand>("Select * From Catalog.Brand where BrandID=@BrandID", CommandType.Text,
                                                            GetBrandFromReader, new SqlParameter("@BrandID", brandId));
        }

        public static List<Brand> GetBrandsNames()
        {
            return SQLDataAccess.ExecuteReadList<Brand>("Select BrandName, BrandID from Catalog.Brand order by SortOrder", CommandType.Text,
                 (reader) => new Brand
                                {
                                    Name = SQLDataHelper.GetString(reader, "BrandName"),
                                    BrandId = SQLDataHelper.GetInt(reader, "BrandID")
                                });
        }

        public static List<Brand> GetBrands()
        {
            return SQLDataAccess.ExecuteReadList<Brand>("Select * from Catalog.Brand order by BrandName", CommandType.Text, GetBrandFromReader);
        }

        public static List<Brand> GetBrands(bool haveProducts)
        {
            string cmd = haveProducts
                             ? @"SELECT * FROM [Catalog].[Brand] left join Catalog.Photo on Photo.ObjId=Brand.BrandID and Type=@Type  Where (SELECT COUNT(ProductID) From [Catalog].[Product] Where [Product].[BrandID]=Brand.[BrandID]) > 0 and enabled=1 ORDER BY [SortOrder], [BrandName]"
                             : "Select * from Catalog.Brand left join Catalog.Photo on Photo.ObjId=Brand.BrandID and Type=@Type where enabled=1 Order by [SortOrder], [BrandName]";
            return SQLDataAccess.ExecuteReadList<Brand>(cmd, CommandType.Text, reader => new Brand
                                                                                             {
                                                                                                 BrandId = SQLDataHelper.GetInt(reader, "BrandId"),
                                                                                                 Name = SQLDataHelper.GetString(reader, "BrandName"),
                                                                                                 Description = SQLDataHelper.GetString(reader, "BrandDescription", string.Empty),
                                                                                                 BriefDescription = SQLDataHelper.GetString(reader, "BrandBriefDescription", string.Empty),
                                                                                                 BrandLogo = new Photo(SQLDataHelper.GetInt(reader, "PhotoId"), SQLDataHelper.GetInt(reader, "ObjId"), PhotoType.Brand) { PhotoName = SQLDataHelper.GetString(reader, "PhotoName") },
                                                                                                 Enabled = SQLDataHelper.GetBoolean(reader, "Enabled", true),
                                                                                                 SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                                                                                                 CountryId = SQLDataHelper.GetInt(reader, "CountryID", 0),
                                                                                                 UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                                                                                                 BrandSiteUrl = SQLDataHelper.GetString(reader, "BrandSiteUrl")
                                                                                             }
                                                        , new SqlParameter("@Type", PhotoType.Brand.ToString()));
        }

        #endregion

        #region ProductLinks

        public static void DeleteProductLink(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalig.Product Set BrandID=Null Where ProductID=@ProductID", CommandType.Text,
                                            new SqlParameter { ParameterName = "@ProductID", Value = productId });
        }

        public static void AddProductLink(int productId, int brandId)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalog.Product Set BrandID=@BrandID Where ProductID=@ProductID", CommandType.Text,
                                            new SqlParameter { ParameterName = "@ProductID", Value = productId },
                                            new SqlParameter { ParameterName = "@BrandId", Value = brandId });
        }

        #endregion


        public static List<Brand> GetBrandsByProductOnMain(ProductOnMain.TypeFlag type)
        {
            var subCmd = string.Empty;
            switch (type)
            {
                case ProductOnMain.TypeFlag.New:
                    subCmd = "New=1";
                    break;
                case ProductOnMain.TypeFlag.Bestseller:
                    subCmd = "Bestseller=1";
                    break;
                case ProductOnMain.TypeFlag.Discount:
                    subCmd = "Discount>0";
                    break;
                case ProductOnMain.TypeFlag.OnSale:
                    subCmd = "OnSale=1";
                    break;
                case ProductOnMain.TypeFlag.Recomended:
                    subCmd = "Recomended=1";
                    break;
            }
            string cmd =
                "Select Brand.BrandID, Brand.BrandName, UrlPath, Brand.SortOrder from Catalog.Brand where BrandID in (select BrandID from Catalog.Product where " + subCmd + " ) and enabled=1 order by Brand.BrandName";
            return SQLDataAccess.ExecuteReadList<Brand>(cmd, CommandType.Text,
                                                         reader => new Brand
                                                         {
                                                             BrandId = SQLDataHelper.GetInt(reader, "BrandID"),
                                                             Name = SQLDataHelper.GetString(reader, "BrandName"),
                                                             UrlPath = SQLDataHelper.GetString(reader, "UrlPath")
                                                         });
        }

        #region BrandCategory
        public static List<Brand> GetBrandsByCategoryID(int categoryId, bool inDepth)
        {
            string cmd = inDepth
                        ? "Select Brand.BrandID, Brand.BrandName, UrlPath, Brand.SortOrder from Catalog.Brand where  BrandID in ( select BrandID from Catalog.Product inner join Catalog.ProductCategories on ProductCategories.ProductID= Product.ProductID and ProductCategories.CategoryID in (select id from Settings.GetChildCategoryByParent(@CategoryID) where Product.Enabled = 1 and Product.CategoryEnabled=1) ) and Enabled=1 Order by Brand.SortOrder,Brand.BrandName"
                        : "Select Brand.BrandID, Brand.BrandName, UrlPath, Brand.SortOrder from Catalog.Brand where  BrandID in ( select BrandID from Catalog.Product inner join Catalog.ProductCategories on ProductCategories.ProductID= Product.ProductID and ProductCategories.CategoryID = @CategoryID and Product.Enabled = 1 and Product.CategoryEnabled=1) and Enabled=1 Order by Brand.SortOrder, Brand.BrandName";
            return SQLDataAccess.ExecuteReadList<Brand>(cmd, CommandType.Text,
                                                         reader => new Brand
                                                                       {
                                                                           BrandId = SQLDataHelper.GetInt(reader, "BrandID"),
                                                                           Name = SQLDataHelper.GetString(reader, "BrandName"),
                                                                           UrlPath = SQLDataHelper.GetString(reader, "UrlPath")
                                                                       },
                                                        new SqlParameter { ParameterName = "@CategoryID", Value = categoryId });
        }

        /// <summary>
        /// ¬озвращает словарь: id категории - число товаров с брендом
        /// </summary>
        /// <param name="brandId"></param>
        /// <returns></returns>
        public static Dictionary<int, BrandProductCount> GetCategoriesByBrand(int brandId)
        {
            return SQLDataAccess.ExecuteReadDictionary<int, BrandProductCount>("[Catalog].[sp_GetCategoriesByBrand]", CommandType.StoredProcedure,
                "CategoryID", reader => new BrandProductCount
                {
                    InCategoryCount = SQLDataHelper.GetInt(reader, "ProductCount"),
                    InChildsCategoryCount = SQLDataHelper.GetInt(reader, "ProductCountChilds")
                }, new SqlParameter("@BrandID", brandId));
        }
        #endregion


        #region Is Enabled

        public static bool IsBrandEnabled(int brandId)
        {
            var res = SQLDataAccess.ExecuteScalar<bool>(
                "SELECT [Enabled] FROM [Catalog].[Brand] WHERE [BrandId] = @brandId", CommandType.Text,
                new SqlParameter { ParameterName = "@brandId", Value = brandId });

            return res;
        }

        #endregion

        #region Products Count
        /// <summary>
        /// get products count
        /// </summary>
        /// <returns></returns>
        public static int GetProductsCount(int brandId)
        {
            var res = SQLDataAccess.ExecuteScalar<int>(
                "SELECT Count([ProductID]) FROM [Catalog].[Product] Where BrandID=@BrandID", CommandType.Text,
                new SqlParameter { ParameterName = "@BrandID", Value = brandId });
            return res;
        }

        #endregion

        #region image
        public static void DeleteBrandLogo(int brandId)
        {
            PhotoService.DeletePhotos(brandId, PhotoType.Brand);
        }

        #endregion


        public static int GetBrandIdByName(string brandName)
        {
            return SQLDataAccess.ExecuteScalar<int>("select BrandID from Catalog.Brand where LOWER (BrandName)=@BrandName", CommandType.Text,
                                                    new SqlParameter { ParameterName = "@BrandName", Value = brandName.ToLower() });
        }

        public static string BrandToString(int brandId)
        {
            var brand = GetBrandById(brandId);
            return brand != null ? brand.Name : string.Empty;
        }

        public static int BrandFromString(string brand)
        {
            int brandId;
            if (string.IsNullOrWhiteSpace(brand))
            {
                brandId = 0;
            }
            else if ((brandId = GetBrandIdByName(brand)) == 0)
            {
                var tempBrand = new Brand
                {
                    Enabled = true,
                    Name = brand,
                    Description = brand,
                    UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Brand, brand),
                    Meta = null
                };
                brandId = AddBrand(tempBrand);
            }

            return brandId;
        }
    }
}