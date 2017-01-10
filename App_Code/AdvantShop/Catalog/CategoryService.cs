//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using Resources;

namespace AdvantShop.Catalog
{
    public class CategoryService
    {
        public enum DisplayStyle
        {
            Tile, List
        }

        public struct CategoryPictures
        {
            public string Picture;
            public string MiniPicture;
        }

        /// <summary>
        /// Geting new ID for category contains all products without categories
        /// </summary>
        /// <returns>"-1" as default ID</returns>
        /// <remarks></remarks>
        public const int DefaultNonCategoryId = -1;

        /// <summary>
        /// return child category and product by parent categoryid
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static IList<CatalogItem> GetChildCategoriesAndProducts(int categoryId)
        {
            return SQLDataAccess.ExecuteReadList("[Catalog].[sp_GetCategoryContent]", CommandType.StoredProcedure,
                reader => new CatalogItem
                {
                    Id = SQLDataHelper.GetInt(reader, "ID"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                    ProductArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                    Type = (CatalogItemType)SQLDataHelper.GetInt(reader, "ItemType"),
                    ChildCount = SQLDataHelper.GetInt(reader, "ChildCount")
                },
                new SqlParameter("@categoryid", categoryId));
        }

        public static IEnumerable<int> GetProductIDs(int categoryId)
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>(
                "SELECT [ProductID] FROM [Catalog].[ProductCategories] WHERE [CategoryID] = @CategoryID",
                CommandType.Text,
                "ProductID",
                new SqlParameter("@CategoryID", categoryId));
        }

        /// <summary>
        /// Get list of products by categoryId
        /// </summary>
        /// <param name="categoryid"></param>
        /// <returns></returns>
        public static IList<Product> GetProductsByCategoryId(int categoryid)
        {
            return GetProductsByCategoryId(categoryid, false);
        }

        /// <summary>
        /// Get list of products by categoryId
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="inDepth">param set use recurse or not</param>
        /// <returns></returns>
        public static IList<Product> GetProductsByCategoryId(int categoryId, bool inDepth)
        {
            var query = inDepth
                ? "SELECT * FROM [Catalog].[Product] INNER JOIN [Catalog].[ProductCategories] on ProductCategories.ProductID = Product.ProductID WHERE [ProductCategories].CategoryID  IN (SELECT id FROM [Settings].[GetChildCategoryByParent](@categoryId)) AND [Product].[Enabled] = 1  AND [Product].[CategoryEnabled] = 1"
                : "SELECT * FROM [Catalog].[Product] INNER JOIN [Catalog].[ProductCategories] on ProductCategories.ProductID = Product.ProductID WHERE [ProductCategories].CategoryID = @categoryId AND [Product].[Enabled] = 1 AND [Product].[CategoryEnabled] = 1";

            var prouducts = SQLDataAccess.ExecuteReadList(query, CommandType.Text,
                reader => new Product
                {
                    ProductId = SQLDataHelper.GetInt(reader, "ProductId"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                    BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription", null),
                    Description = SQLDataHelper.GetString(reader, "Description", null),
                    Discount = SQLDataHelper.GetFloat(reader, "Discount"),
                    //ShippingPrice = SQLDataHelper.GetFloat(reader, "ShippingPrice"),
                    Size = SQLDataHelper.GetString(reader, "Size"),
                    Weight = SQLDataHelper.GetFloat(reader, "Weight"),
                    Ratio = SQLDataHelper.GetDouble(reader, "Ratio"),
                    Enabled = SQLDataHelper.GetBoolean(reader, "Enabled", true),
                    Recomended = SQLDataHelper.GetBoolean(reader, "Recomended"),
                    New = SQLDataHelper.GetBoolean(reader, "New"),
                    BestSeller = SQLDataHelper.GetBoolean(reader, "Bestseller"),
                    OnSale = SQLDataHelper.GetBoolean(reader, "OnSale")
                },
                new SqlParameter("@categoryId", categoryId));

            return prouducts ?? new List<Product>();
        }

        /// <summary>
        /// return child categories by parent categoryId
        /// </summary>
        /// <param name="categoryId"></param>
        /// /// <param name="hasProducts"></param>
        /// <returns></returns>
        public static IList<Category> GetChildCategoriesByCategoryId(int categoryId, bool hasProducts)
        {
            return SQLDataAccess.ExecuteReadList(
                "[Catalog].[sp_GetChildCategoriesByParentID]", CommandType.StoredProcedure,
                reader =>
                {
                    var category = new Category
                    {
                        CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                        Name = SQLDataHelper.GetString(reader, "Name"),
                        //Picture = SQLDataHelper.GetString(reader, "Picture"),
                        SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                        ParentCategoryId = SQLDataHelper.GetInt(reader, "ParentCategory"),
                        ProductsCount = SQLDataHelper.GetInt(reader, "Products_Count"),
                        TotalProductsCount =
                            SQLDataHelper.GetInt(reader, "Total_Products_Count"),
                        Description = SQLDataHelper.GetString(reader, "Description"),
                        BriefDescription =
                            SQLDataHelper.GetString(reader, "BriefDescription"),
                        Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                        DisplayStyle = SQLDataHelper.GetString(reader, "DisplayStyle"),
                        //MiniPicture = SQLDataHelper.GetString(reader, "MiniPicture"),
                        UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                        ParentsEnabled = SQLDataHelper.GetBoolean(reader, "HirecalEnabled")
                    };
                    var childCounts = SQLDataHelper.GetInt(reader, "ChildCategories_Count");
                    category.HasChild = childCounts > 0;
                    category.Picture = new Photo(0, category.CategoryId, PhotoType.CategoryBig) { PhotoName = SQLDataHelper.GetString(reader, "Picture") };
                    category.MiniPicture = new Photo(0, category.CategoryId, PhotoType.CategorySmall) { PhotoName = SQLDataHelper.GetString(reader, "MiniPicture") };
                    return category;
                },
                new SqlParameter("@ParentCategoryID", categoryId), new SqlParameter("@hasProducts", hasProducts),
                new SqlParameter("@bigType", PhotoType.CategoryBig.ToString()),
                new SqlParameter("@smallType", PhotoType.CategorySmall.ToString()));
        }

        /// <summary>
        /// return child categories by parent categoryId
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static IList<Category> GetChildCategoriesByCategoryIdForMenu(int categoryId)
        {
            return SQLDataAccess.ExecuteReadList(
                "[Catalog].[sp_GetChildCategoriesByParentIDForMenu]",
                CommandType.StoredProcedure,
                reader =>
                {
                    var category = new Category
                    {
                        CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                        Name = SQLDataHelper.GetString(reader, "Name"),
                        //Picture = SQLDataHelper.GetString(reader, "Picture"),
                        SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                        ParentCategoryId = SQLDataHelper.GetInt(reader, "ParentCategory"),
                        ProductsCount = SQLDataHelper.GetInt(reader, "Products_Count"),
                        BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription"),
                        Description = SQLDataHelper.GetString(reader, "Description"),
                        Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                        DisplayStyle = SQLDataHelper.GetString(reader, "DisplayStyle"),
                        UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                        DisplayBrandsInMenu = SQLDataHelper.GetBoolean(reader, "DisplayBrandsInMenu"),
                        DisplaySubCategoriesInMenu = SQLDataHelper.GetBoolean(reader, "DisplaySubCategoriesInMenu"),
                    };
                    var childCounts = SQLDataHelper.GetInt(reader, "ChildCategories_Count");
                    category.HasChild = childCounts > 0;
                    return category;
                },
                new SqlParameter("@CurrentCategoryID", categoryId));
        }

        /// <summary>
        /// return child categories by parent categoryId
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static IList<Category> GetChildCategoriesByCategoryIdForMenu(int categoryId, Boolean hasProducts)
        {
            if (hasProducts)
            {
                return GetChildCategoriesByCategoryIdForMenu(categoryId).Where(c => c.ProductsCount > 0).ToList();
            }
            else
            {
                return GetChildCategoriesByCategoryIdForMenu(categoryId);
            }
        }

        public static IEnumerable<int> GetChildIDsHierarchical(int catId)
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>("[Catalog].[sp_GetChildCategoriesIDHierarchical]",
                                                                    CommandType.StoredProcedure,
                                                                    "CategoryID",
                                                                    new SqlParameter("@parent", catId)
                                                                    );
        }

        /// <summary>
        /// return child categories and NonCategory
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static IList<Category> GetChildCategoriesAndNonCategory(int categoryId)
        {
            var res = (List<Category>)GetChildCategoriesByCategoryId(categoryId, false);
            // Adding category witch  contents all products without categories
            var nonCategory = new Category
            {
                CategoryId = DefaultNonCategoryId,
                Name = Resource.Admin_CategoryService_ProductsWithoutCategories,
                Picture = null,
                ProductsCount = 0,
                ParentCategoryId = DefaultNonCategoryId,
                Enabled = true
            };
            res.Add(nonCategory);
            return res;
        }


        /// <summary>
        /// add category
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="updateCache"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static int AddCategory(Category cat, bool updateCache, SQLDataAccess db)
        {
            db.cmd.CommandText = "[Catalog].[sp_AddCategory]";
            db.cmd.CommandType = CommandType.StoredProcedure;

            db.cmd.Parameters.Clear();
            db.cmd.Parameters.AddWithValue("@Description", cat.Description ?? (object)DBNull.Value);
            db.cmd.Parameters.AddWithValue("@BriefDescription", cat.BriefDescription ?? (object)DBNull.Value);
            db.cmd.Parameters.AddWithValue("@Name", cat.Name);
            db.cmd.Parameters.AddWithValue("@ParentCategory", cat.ParentCategoryId);

            db.cmd.Parameters.AddWithValue("@SortOrder", cat.SortOrder);
            db.cmd.Parameters.AddWithValue("@Enabled", cat.Enabled);
            db.cmd.Parameters.AddWithValue("@DisplayStyle", cat.DisplayStyle ?? String.Empty);
            db.cmd.Parameters.AddWithValue("@DisplayChildProducts", cat.DisplayChildProducts);
            db.cmd.Parameters.AddWithValue("@DisplayBrandsInMenu", cat.DisplayBrandsInMenu);
            db.cmd.Parameters.AddWithValue("@DisplaySubCategoriesInMenu", cat.DisplaySubCategoriesInMenu);
            db.cmd.Parameters.AddWithValue("@UrlPath", cat.UrlPath);
            db.cmd.Parameters.AddWithValue("@Sorting", cat.Sorting);

            db.cnOpen();
            var id = SQLDataHelper.GetInt(db.cmd.ExecuteScalar());
            db.cnClose();
            if (updateCache)
            {
                CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(cat.ParentCategoryId));
                CacheManager.RemoveByPattern("MenuCatalog");

                if (cat.ParentCategoryId == 0)
                {
                    var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                    if (CacheManager.Contains(cacheName))
                        CacheManager.Remove(cacheName);
                }
            }
            return id;
        }

        /// <summary>
        /// add category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="updateCache"></param>
        /// <param name="setHierarchicallyEnabled"></param>
        /// <returns></returns>
        public static int AddCategory(Category category, bool updateCache, bool setHierarchicallyEnabled = true)
        {
            int id;
            using (var db = new SQLDataAccess())
            {
                id = AddCategory(category, updateCache, db);
                if (id == -1)
                    return -1;
                // ---- Meta
                if (setHierarchicallyEnabled)
                    SetCategoryHierarchicallyEnabled(id);

                if (category.Meta != null)
                {
                    if (!category.Meta.Title.IsNullOrEmpty() || !category.Meta.MetaKeywords.IsNullOrEmpty() ||
                        !category.Meta.MetaDescription.IsNullOrEmpty() || !category.Meta.H1.IsNullOrEmpty())
                    {
                        category.Meta.ObjId = id;
                        MetaInfoService.SetMeta(category.Meta);
                    }
                }
            }
            return id;
        }

        /// <summary>
        /// get parent categories by child category
        /// </summary>
        /// <param name="childCategoryId">child categoryid</param>
        /// <returns></returns>
        public static IList<Category> GetParentCategories(int childCategoryId)
        {
            return SQLDataAccess.ExecuteReadList("[Catalog].[sp_GetParentCategories]", CommandType.StoredProcedure,
                reader => new Category
                {
                    CategoryId = SQLDataHelper.GetInt(reader, "id"),
                    Name = SQLDataHelper.GetString(reader, "name"),
                    UrlPath = SQLDataHelper.GetString(reader, "url")
                },
                new SqlParameter("@ChildCategoryId", childCategoryId));
        }

        /// <summary>
        /// delete category by categoryId
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="updateCache">refresh cache</param>
        /// <returns>return list of file namme image</returns>
        private static IEnumerable<int> DeleteCategory(int categoryId, bool updateCache)
        {
            if (categoryId == 0)
                throw new Exception("deleting Root catregory");

            if (updateCache)
            {
                CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(categoryId));
                CacheManager.RemoveByPattern("MenuCatalog");

                var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                if (CacheManager.Contains(cacheName))
                    CacheManager.Remove(cacheName);
            }

            return SQLDataAccess.ExecuteReadIEnumerable("[Catalog].[sp_DeleteCategoryWithSubCategoies]",
                                                            CommandType.StoredProcedure,
                                                            reader => SQLDataHelper.GetInt(reader, "CategoryID"),
                                                            new SqlParameter("@id", categoryId));
        }

        /// <summary>
        /// get all categories
        /// </summary>
        /// <returns></returns>
        public static List<Category> GetCategories()
        {
            return
                SQLDataAccess.ExecuteReadList(
                    "SELECT [CategoryID], [Name], [ParentCategory], [SortOrder] FROM [Catalog].[Category] Where CategoryId <> 0",
                    CommandType.Text,
                    reader => new Category
                    {
                        CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                        Name = SQLDataHelper.GetString(reader, "Name"),
                        ParentCategoryId = SQLDataHelper.GetInt(reader, "ParentCategory"),
                        SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                    });
        }


        /// <summary>
        /// get category by categoryId from cache or db if cache null
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static Category GetCategory(int categoryId)
        {
            Category res;
            string strCacheName = CacheNames.GetCategoryCacheObjectName(categoryId);

            if (CacheManager.Contains(strCacheName))
            {
                res = CacheManager.Get<Category>(strCacheName);
                if (res != null)
                    return res;
            }
            // Return from db
            res = GetCategoryFromDbByCategoryId(categoryId);

            // Insert to cahce
            if (res != null)
                CacheManager.Insert(strCacheName, res);
            else
                CacheManager.Remove(strCacheName);

            return res;
        }

        public static Category GetCategoryFromReader(SqlDataReader reader)
        {
            return new Category
            {
                CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                //Picture = SQLDataHelper.GetString(reader, "Picture"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                ParentCategoryId = SQLDataHelper.GetInt(reader, "ParentCategory"),
                ProductsCount = SQLDataHelper.GetInt(reader, "Products_Count"),
                Description = SQLDataHelper.GetString(reader, "Description"),
                BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                DisplayStyle = SQLDataHelper.GetString(reader, "DisplayStyle"),
                DisplayChildProducts = SQLDataHelper.GetBoolean(reader, "DisplayChildProducts"),
                DisplayBrandsInMenu = SQLDataHelper.GetBoolean(reader, "DisplayBrandsInMenu"),
                DisplaySubCategoriesInMenu = SQLDataHelper.GetBoolean(reader, "DisplaySubCategoriesInMenu"),
                //MiniPicture = SQLDataHelper.GetString(reader, "MiniPicture"),
                TotalProductsCount = SQLDataHelper.GetInt(reader, "Total_Products_Count"),
                UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                ParentsEnabled = SQLDataHelper.GetBoolean(reader, "HirecalEnabled"),
                Sorting = (ESortOrder)SQLDataHelper.GetInt(reader, "Sorting")
            };
        }

        /// <summary>
        /// get category by categoryId from DB
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static Category GetCategoryFromDbByCategoryId(int categoryId)
        {
            var c = SQLDataAccess.ExecuteReadOne(@"SELECT * FROM [Catalog].[Category] WHERE CategoryId = @CategoryId",
                                                                CommandType.Text, GetCategoryFromReader, new SqlParameter("@CategoryId", categoryId));
            return c;
        }

        /// <summary>
        /// update category
        /// </summary>
        /// <param name="category"></param>
        /// <param name="updateCache">refresh cache</param>
        /// <returns></returns>
        public static bool UpdateCategory(Category category, bool updateCache)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateCategory]", CommandType.StoredProcedure,
                                        new SqlParameter("@CategoryID", category.CategoryId),
                                        new SqlParameter("@Name", category.Name),
                                        new SqlParameter("@ParentCategory", category.ParentCategoryId),
                                        new SqlParameter("@Description", category.Description),
                                        new SqlParameter("@BriefDescription", category.BriefDescription),
                                        new SqlParameter("@Enabled", category.Enabled),
                                        new SqlParameter("@DisplayStyle", category.DisplayStyle),
                                        new SqlParameter("@displayChildProducts", category.DisplayChildProducts),
                                        new SqlParameter("@DisplayBrandsInMenu", category.DisplayBrandsInMenu),
                                        new SqlParameter("@DisplaySubCategoriesInMenu", category.DisplaySubCategoriesInMenu),
                                        new SqlParameter("@SortOrder", category.SortOrder),
                //new SqlParameter("@Picture", !string.IsNullOrEmpty(category.Picture) ? category.Picture : ((object)DBNull.Value)),
                //new SqlParameter("@MiniPicture", !string.IsNullOrEmpty(category.MiniPicture) ? category.MiniPicture : ((object)DBNull.Value)),
                                        new SqlParameter("@UrlPath", category.UrlPath),
                                        new SqlParameter("@Sorting", category.Sorting)
                                        );
            if (category.Meta != null)
            {
                if (category.Meta.Title.IsNullOrEmpty() && category.Meta.MetaKeywords.IsNullOrEmpty() && category.Meta.MetaDescription.IsNullOrEmpty() && category.Meta.H1.IsNullOrEmpty())
                {
                    if (MetaInfoService.IsMetaExist(category.CategoryId, MetaType.Category))
                        MetaInfoService.DeleteMetaInfo(category.CategoryId, MetaType.Category);
                }
                else
                    MetaInfoService.SetMeta(category.Meta);
            }

            SetCategoryHierarchicallyEnabled(category.CategoryId);

            // Work with cache
            if (updateCache)
            {
                CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(category.CategoryId));
                CacheManager.RemoveByPattern("MenuCatalog");

                if (category.ParentCategoryId == 0)
                {
                    var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                    if (CacheManager.Contains(cacheName))
                        CacheManager.Remove(cacheName);
                }
            }
            return true;
        }

        public static bool UpdateCategorySortOrder(string name, int sortOrder, int cateoryId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Catalog.Category set name=@name, SortOrder=@SortOrder where CategoryID = @CategoryID",
                CommandType.Text,
                new SqlParameter("@name", name),
                new SqlParameter("@SortOrder", sortOrder),
                new SqlParameter("@CategoryID", cateoryId));

            CacheManager.RemoveByPattern("MenuCatalog");

            var cacheName = CacheNames.GetBottomMenuCacheObjectName();
            if (CacheManager.Contains(cacheName))
                CacheManager.Remove(cacheName);

            return true;
        }

        /// <summary>
        /// Warning!! Very heavy function! recalculate product in categories
        /// </summary>
        public static void RecalculateProductsCountManual()
        {
            new Task(() =>
            {
                SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_RecalculateProductsCount]", CommandType.StoredProcedure);
                ClearCategoryCache();
            }).Start();

        }
        /// <summary>
        /// clear all categories in cache
        /// </summary>
        public static void ClearCategoryCache()
        {
            foreach (DictionaryEntry e in CacheManager.CacheObject)
            {
                if (SQLDataHelper.GetString(e.Key).StartsWith(CacheNames.GetCategoryCacheObjectPrefix()))
                {
                    CacheManager.Remove(SQLDataHelper.GetString(e.Key));
                }
            }
        }

        /// <summary>
        /// get total count of products
        /// </summary>
        /// <returns></returns>
        public static int GetTolatCounTofProducts()
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNTofTotalProducts]", CommandType.StoredProcedure);
        }

        /// <summary>
        /// get total count of products without categoies
        /// </summary>
        /// <returns></returns>
        public static int GetTolatCounTofProductsWithoutCategories()
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNTofProductsWithoutCategories]", CommandType.StoredProcedure);
        }

        /// <summary>
        /// get total count of products in categoies
        /// </summary>
        /// <returns></returns>
        public static int GetTolatCounTofProductsInCategories()
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNTofProductsInCategories]", CommandType.StoredProcedure);
        }


        //****************************************************************
        //TODO may be remove to exportimport
        //****************************************************************
        /// <summary>
        /// Converts category with given ID to xml format
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Xml formatted category</returns>
        /// <remarks></remarks>
        public static XmlDocument ConvertToXml(int id)
        {
            Category category = GetCategory(id);
            return ConvertToXml(category);
        }

        /// <summary>
        /// Converts given category  to xml format
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Xml formatted category</returns>
        /// <remarks></remarks>
        public static XmlDocument ConvertToXml(Category category)
        {
            var result = new XmlDocument();
            if (category == null)
            {
                return null;
            }
            XmlNode catXml = result.CreateElement("Category");
            XmlElement child = result.CreateElement("CategoryID");
            child.InnerText = category.CategoryId.ToString();
            catXml.AppendChild(child);
            child = result.CreateElement("ParentCategoryId");
            child.InnerText = category.ParentCategoryId.ToString();
            catXml.AppendChild(child);
            child = result.CreateElement("Name");
            child.InnerText = category.Name;
            catXml.AppendChild(child);
            child = result.CreateElement("Enabled");
            child.InnerText = category.Enabled.ToString();
            catXml.AppendChild(child);

            // '''??
            if (!String.IsNullOrEmpty(category.DisplayStyle))
            {
                child = result.CreateElement("DisplayStyle");
                child.InnerText = category.DisplayStyle;
                catXml.AppendChild(child);
            }

            // '''??
            if (category.ProductsCount > 0)
            {
                child = result.CreateElement("ProductsCount");
                child.InnerText = category.ProductsCount.ToString();
                catXml.AppendChild(child);
            }

            // '''??
            if (category.MiniPicture != null)
                if (!String.IsNullOrEmpty(category.MiniPicture.PhotoName))
                {
                    child = result.CreateElement("MiniPicture");
                    child.InnerText = category.MiniPicture.PhotoName;
                    catXml.AppendChild(child);
                }

            if (category.Picture != null)
                if (!String.IsNullOrEmpty(category.Picture.PhotoName))
                {
                    child = result.CreateElement("Picture");
                    child.InnerText = category.Picture.PhotoName;
                    catXml.AppendChild(child);
                }

            if (!String.IsNullOrEmpty(category.Description))
            {
                child = result.CreateElement("Description");
                child.InnerText = category.Description;
                catXml.AppendChild(child);
            }
            if (category.Meta != null)
            {
                XmlElement metaXml = result.CreateElement("Meta");
                if (!String.IsNullOrEmpty(category.Meta.Title))
                {
                    child = result.CreateElement("Title");
                    child.InnerText = category.Meta.Title;
                    metaXml.AppendChild(child);
                }
                if (!String.IsNullOrEmpty(category.Meta.MetaDescription))
                {
                    child = result.CreateElement("MetaDescription");
                    child.InnerText = category.Meta.MetaDescription;
                    metaXml.AppendChild(child);
                }
                if (!String.IsNullOrEmpty(category.Meta.MetaKeywords))
                {
                    child = result.CreateElement("MetaKeywords");
                    child.InnerText = category.Meta.MetaKeywords;
                    metaXml.AppendChild(child);
                }
                if (!String.IsNullOrEmpty(category.Meta.H1))
                {
                    child = result.CreateElement("H1");
                    child.InnerText = category.Meta.H1;
                    metaXml.AppendChild(child);
                }
            }
            if (category.ProductIDs != null && category.ProductIDs.Count > 0)
            {
                XmlElement products = result.CreateElement("Products");
                foreach (var product in category.ProductIDs)
                {
                    var productXml = result.CreateElement("Product");
                    productXml.InnerText = product.ToString();
                    products.AppendChild(productXml);
                }
                if (products.HasChildNodes)
                    catXml.AppendChild(products);
            }
            result.AppendChild(catXml);
            return result;
        }

        public static int GetEnabledProductsCountInCategory(int catId, bool displayChildProducts)
        {
            var comand = displayChildProducts
                            ? "SELECT Count(Product.ProductID) FROM [Catalog].[Product] INNER JOIN [Catalog].[ProductCategories] on ProductCategories.ProductID = Product.ProductID WHERE [ProductCategories].CategoryID  IN (SELECT id FROM [Settings].[GetChildCategoryByParent](@categoryId)) AND [Product].[Enabled] = 1 AND [Product].[CategoryEnabled] = 1"
                            : "SELECT Count(Product.ProductID) FROM [Catalog].[Product] INNER JOIN [Catalog].[ProductCategories] on ProductCategories.ProductID = Product.ProductID WHERE [ProductCategories].CategoryID = @categoryId AND [Product].[Enabled] = 1 AND [Product].[CategoryEnabled] = 1";
            return SQLDataAccess.ExecuteScalar<int>(comand, CommandType.Text, new SqlParameter("@categoryId", catId));
        }

        /// <summary>
        /// delete relationship beetween category and product
        /// </summary>
        /// <param name="prodId"></param>
        /// <param name="categId"></param>
        /// <returns></returns>
        public static void DeleteCategoryAndLink(int prodId, int categId)
        {
            ProductService.DeleteProductLink(prodId, categId);
        }

        /// <summary>
        /// delete category and photo of category
        /// </summary>
        /// <param name="categoryId"></param>
        public static void DeleteCategoryAndPhotos(int categoryId)
        {
            foreach (var id in DeleteCategory(categoryId, true))
            {
                PhotoService.DeletePhotos(id, PhotoType.CategoryBig);
                PhotoService.DeletePhotos(id, PhotoType.CategorySmall);
            }

            var cacheName = CacheNames.GetBottomMenuCacheObjectName();
            if (CacheManager.Contains(cacheName))
                CacheManager.Remove(cacheName);
        }

        /// <summary>
        /// delete all relationships with products
        /// </summary>
        /// <param name="catId"></param>
        public static void DeleteCategoryLink(int catId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteCategoryLinks]", CommandType.StoredProcedure, new SqlParameter("@CategoryID", catId));
            ClearCategoryCache();
        }

        public static bool IsExistCategory(int catId)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNTCategoryByID]", CommandType.StoredProcedure, new SqlParameter("@CategoryID", catId)) > 0;
        }


        public static KeyValuePair<float, float> GetPriceRange(int categoryId, bool inDepth)
        {
            return SQLDataAccess.ExecuteReadOne(
                @"[Catalog].[sp_GetPriceRange]",
                CommandType.StoredProcedure,
                reader =>
                new KeyValuePair<float, float>(
                    SQLDataHelper.GetFloat(reader, "minprice"),
                    SQLDataHelper.GetFloat(reader, "maxprice")),
                new SqlParameter("@categoryId", categoryId),
                new SqlParameter("@useDepth", inDepth)
                );
        }

        public static int? GetChildCategoryIdByName(int parentId, string name)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT CategoryID FROM [Catalog].[Category] WHERE [Name] = @Name AND [ParentCategory] = @ParentID",
                CommandType.Text,
                 reader => SQLDataHelper.GetNullableInt(reader, "CategoryID"),
                new SqlParameter("@Name", name),
                new SqlParameter("@ParentID", parentId));
        }

        public static int GetCategoryIdByName(string name)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT Top 1 CategoryID FROM [Catalog].[Category] WHERE [Name] = @Name",
                CommandType.Text,
                new SqlParameter("@Name", name));
        }
        public static int GetCategoryIdByUrlPath(string urlPath)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT Top 1 CategoryID FROM [Catalog].[Category] WHERE [UrlPath] = @UrlPath",
                CommandType.Text,
                new SqlParameter("@UrlPath", urlPath));
        }

        public static string GetCategoryStringByProductId(int productId, string separator, bool onlySort = false)
        {
            var categoryDict = SQLDataAccess.ExecuteReadDictionary<int, int>(
                "Select CategoryID, SortOrder from Catalog.ProductCategories where ProductID=@id order by Main Desc", CommandType.Text,
                "CategoryID", "SortOrder",
                new SqlParameter("@id", productId));

            if (onlySort)
            {
                var resSort = categoryDict.Select(
                    pair =>
                    String.Format("{0}", pair.Value)).AggregateString(separator);
                return resSort;
            }

            var res = categoryDict.Select(
                pair =>
                String.Format("[{0}]", GetParentCategoriesAsString(pair.Key))).AggregateString(',');
            return res;
        }

        private static string GetParentCategoriesAsString(int childCategoryId)
        {
            var res = new StringBuilder();
            var categoies = GetParentCategories(childCategoryId);
            for (var i = categoies.Count - 1; i >= 0; i--)
            {
                if (i != categoies.Count - 1)
                {
                    res.Append(" >> ");
                }
                res.Append(categoies[i].Name);
            }
            return res.ToString();
        }

        public static void SubParseAndCreateCategory(string strCategory, int productId, string separator = ";", string sorting = "")
        {
            //
            // strCategory "[Техника >> Игровые приставки >> PlayStation],[....]"
            //
            if (strCategory.IsNullOrEmpty()) return;

            string[] sortCat = sorting.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            ProductService.DeleteAllProductLink(productId);

            bool isMainCategory = true;
            var indexCat = -1;
            var cats = strCategory.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var categoryStr in cats)
            {
                var categoryString = categoryStr.Trim();
                if (categoryString == ",") continue;
                if (categoryString.IsNullOrEmpty()) continue;

                indexCat++;
                var productSortOrder = 0;
                if (sortCat.Length > indexCat)
                {
                    productSortOrder = sortCat[indexCat].TryParseInt();
                }

                int parentId = 0;
                var categories = categoryString.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries).Select(name => name.Trim()).ToArray();

                if (categories.Any(c => c.IsNullOrEmpty())) continue;

                int index = 0;
                foreach (var categoryName in categories)
                {
                    var cat = GetChildCategoryIdByName(parentId, categoryName);
                    if (!cat.HasValue)
                    {
                        parentId = AddCategory(new Category
                        {
                            Name = categoryName,
                            ParentCategoryId = parentId,
                            SortOrder = 0,
                            Enabled = true,
                            DisplayChildProducts = false,
                            UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Category, categoryName),
                            DisplayStyle = DisplayStyle.List.ToString()
                        }, false, false);
                    }
                    else
                    {
                        parentId = cat.Value;
                    }

                    if (index == categories.Length - 1)
                    {
                        ProductService.AddProductLink(productId, parentId, productSortOrder, false);
                        if (isMainCategory)
                        {
                            ProductService.SetMainLink(productId, parentId);
                            isMainCategory = false;
                        }
                    }
                    index++;
                }
            }
        }

        public static void SetActive(int categoryId, bool active)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Update [Catalog].[Category] Set Enabled = @Enabled Where CategoryID = @CategoryID",
                 CommandType.Text,
                 new SqlParameter("@CategoryID", categoryId),
                 new SqlParameter("@Enabled", active));
        }

        /// <summary>
        /// for elbuz import
        /// </summary>
        /// <param name="strCategory"></param>
        /// <returns></returns>
        public static int SubParseAndCreateCategory(string strCategory)
        {
            int categoryId = -1;
            //
            // strCategory "[Техника >> Игровые приставки >> PlayStation]-10;[....]" (10 - порядок сортировки товара в категории(* необязательно))
            //
            foreach (string strT in strCategory.Split(new[] { ';' }))
            {
                var st = strT;
                st = st.Replace("[", "");
                st = st.Replace("]", "");
                int parentId = 0;
                string[] temp = st.Split(new[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i <= temp.Length - 1; i++)
                {
                    string name = temp[i].Trim();
                    if (!String.IsNullOrEmpty(name))
                    {
                        var cat = GetChildCategoryIdByName(parentId, name);
                        if (cat.HasValue)
                        {
                            parentId = cat.Value;
                            categoryId = cat.Value;
                        }
                        else
                        {
                            parentId = AddCategory(new Category
                            {
                                Name = name,
                                ParentCategoryId = parentId,
                                //Picture = string.Empty,
                                SortOrder = 0,
                                Enabled = true,
                                DisplayChildProducts = false,
                                UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Category, name),
                                DisplayStyle = DisplayStyle.List.ToString(),
                                Meta = MetaInfoService.GetDefaultMetaInfo(MetaType.Category, name)
                            }, false);
                        }
                    }
                    if (i == temp.Length - 1)
                    {
                        categoryId = parentId;
                    }
                }
            }
            return categoryId;
        }

        public static int GetHierarchyProductsCount(int categoryId)
        {
            return SQLDataAccess.ExecuteScalar<int>(@" SELECT COUNT([TProduct].[ProductId]) FROM [Catalog].[Product] AS [TProduct] " +
                      "WHERE (SELECT COUNT([OfferId]) FROM [Catalog].[Offer] WHERE [Price] > 0 AND [Amount] > 0 AND [OfferListId] = 6 AND [ProductId] = [TProduct].[ProductId]) > 0 " +
                      "AND (SELECT TOP(1) [Catalog].[ProductCategories].[CategoryId] FROM [Catalog].[ProductCategories] JOIN [Catalog].[Category] on [Category].[CategoryId] = [ProductCategories].[CategoryId] WHERE [ProductCategories].[ProductId] = [TProduct].[ProductId] AND [Enabled] = 1 AND [CategoryEnabled] = 1) IN (SELECT [ID] FROM [Settings].[GetChildCategoryByParent] (@CategoryId)) " +
                      "AND [Enabled] = 1 AND [CategoryEnabled] = 1", CommandType.Text, new SqlParameter("@CategoryID", categoryId));
        }

        public static bool FullEnabled(int categoryId)
        {
            bool fullEnabled = true;

            while (categoryId != 0)
            {
                var category = GetCategory(categoryId);
                categoryId = category.ParentCategoryId;

                fullEnabled &= category.Enabled;
            }

            return fullEnabled;
        }

        public static void SetCategoryHierarchicallyEnabled(int categoryId)
        {
            new Task(() => SQLDataAccess.ExecuteNonQuery("[Catalog].[SetCategoryHierarchicallyEnabled]", CommandType.StoredProcedure,
                new SqlParameter("@CatParent", categoryId))).Start();
            
        }
    }
}