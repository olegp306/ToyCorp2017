//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Orders;

namespace AdvantShop.Catalog
{
    public static class ProductOnMain
    {
        public enum TypeFlag
        {
            None = 0,
            Bestseller = 1,
            New = 2,
            Discount = 3,
            OnSale=4,
            Recomended = 5
        }

        public static List<int> GetProductIdByType(TypeFlag type)
        {
            string sqlCmd;
            switch (type)
            {
                case TypeFlag.Bestseller:
                    sqlCmd = "select ProductId from Catalog.Product where Bestseller=1";
                    break;
                case TypeFlag.New:
                    sqlCmd = "select ProductId from Catalog.Product where New=1";
                    break;
                case TypeFlag.Discount:
                    sqlCmd = "select ProductId from Catalog.Product where Discount > 0";
                    break;
                case TypeFlag.OnSale:
                    sqlCmd = "select ProductId from Catalog.Product where OnSale=1";
                    break;
                case TypeFlag.Recomended:
                    sqlCmd = "select ProductId from Catalog.Product where Recomended=1";
                    break;
                default:
                    throw new NotImplementedException();
            }
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>(sqlCmd, CommandType.Text, "ProductId", new SqlParameter { ParameterName = "@type", Value = (int)type }).ToList();
        }

        public static DataTable GetProductsByType(TypeFlag type, int count)
        {
            string sqlCmd = "select Top(@count) Product.ProductId, Product.ArtNo, Name, BriefDescription, " +
                            "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) PhotoId From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID  or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type order by main desc, PhotoSortOrder) ELSE (Select TOP(1) PhotoId From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type order by main desc, PhotoSortOrder) END)  AS PhotoId, " +
                            "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type order by main desc, PhotoSortOrder) ELSE (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type order by main desc, PhotoSortOrder) END)  AS Photo, " +
                            "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID  or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type) ELSE (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type AND [Photo].[Main] = 1) END)  AS PhotoDesc, " +
                            "Discount, Ratio, RatioID, AllowPreOrder, Recomended, New, BestSeller, OnSale, UrlPath, " +
                            "ShoppingCartItemID, Price, " +
                            "(Select Max(Offer.Amount) from catalog.Offer Where ProductId=[Product].[ProductID]) as Amount," +
                            " Offer.OfferID, Offer.ColorID, MinAmount, " +
                            (SettingsCatalog.ComplexFilter ?
                            "(select [Settings].[ProductColorsToString]([Product].[ProductID])) as Colors":
                            "null as Colors") +
                            " from Catalog.Product " +
                            "LEFT JOIN [Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] and Offer.main=1 " +
                            "LEFT JOIN Catalog.Photo on Product.ProductID=Photo.ObjId and Type=@Type and Photo.main=1 " +
                            "LEFT JOIN [Catalog].[ShoppingCart] ON [Catalog].[ShoppingCart].[OfferID] = [Catalog].[Offer].[OfferID] AND [Catalog].[ShoppingCart].[ShoppingCartType] = @ShoppingCartType AND [ShoppingCart].[CustomerID] = @CustomerId " +
                            "Left JOIN [Catalog].[Ratio] on Product.ProductId=Ratio.ProductID and Ratio.CustomerId=@CustomerId " +
                            "where {0} and Enabled=1 and CategoryEnabled=1 and [Settings].[CountCategoriesByProduct](Product.ProductID) > 0 order by {1}";
            switch (type)
            {
                case TypeFlag.Bestseller:
                    sqlCmd = string.Format(sqlCmd, "Bestseller=1", "SortBestseller");
                    break;
                case TypeFlag.New:
                    sqlCmd = string.Format(sqlCmd, "New=1", "SortNew, [DateModified] DESC");
                    break;
                case TypeFlag.Discount:
                    sqlCmd = string.Format(sqlCmd, "Discount>0", "SortDiscount");
                    break;
                case TypeFlag.OnSale:
                    sqlCmd = string.Format(sqlCmd, "OnSale=1", "[DateModified] DESC");
                    break;
                case TypeFlag.Recomended:
                    sqlCmd = string.Format(sqlCmd, "Recomended=1", "[DateModified] DESC");
                    break;

                default:
                    throw new NotImplementedException();
            }
            return SQLDataAccess.ExecuteTable(sqlCmd, CommandType.Text,
                                              new SqlParameter {ParameterName = "@count", Value = count},
                                              new SqlParameter("@CustomerId", CustomerContext.CustomerId.ToString()),
                                              new SqlParameter("@Type", PhotoType.Product.ToString()),
                                              new SqlParameter("@ShoppingCartType", (int)ShoppingCartType.Compare));
        }
        public static DataTable GetPopularityManuallyProduct(int count)
        {
            string sqlCmd = "select Top(@count) Product.ProductId, Product.ArtNo, Name, BriefDescription, " +
                            "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) PhotoId From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID  or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type order by main desc, PhotoSortOrder) ELSE (Select TOP(1) PhotoId From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type order by main desc, PhotoSortOrder) END)  AS PhotoId, " +
                            "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type order by main desc, PhotoSortOrder) ELSE (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type order by main desc, PhotoSortOrder) END)  AS Photo, " +
                            "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID  or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type) ELSE (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type AND [Photo].[Main] = 1) END)  AS PhotoDesc, " +
                            "Discount, Ratio, RatioID, AllowPreOrder, Recomended, New, BestSeller, OnSale, UrlPath, " +
                            "ShoppingCartItemID, Price, " +
                            "(Select Max(Offer.Amount) from catalog.Offer Where ProductId=[Product].[ProductID]) as Amount," +
                            " Offer.OfferID, Offer.ColorID, MinAmount, " +
                            (SettingsCatalog.ComplexFilter ?
                            "(select [Settings].[ProductColorsToString]([Product].[ProductID])) as Colors" :
                            "null as Colors") +
                            " from Catalog.Product " +
                            "LEFT JOIN [Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] and Offer.main=1 " +
                            "LEFT JOIN Catalog.Photo on Product.ProductID=Photo.ObjId and Type=@Type and Photo.main=1 " +
                            "LEFT JOIN [Catalog].[ShoppingCart] ON [Catalog].[ShoppingCart].[OfferID] = [Catalog].[Offer].[OfferID] AND [Catalog].[ShoppingCart].[ShoppingCartType] = @ShoppingCartType AND [ShoppingCart].[CustomerID] = @CustomerId " +
                            "Left JOIN [Catalog].[Ratio] on Product.ProductId=Ratio.ProductID and Ratio.CustomerId=@CustomerId " +
                            "where  Enabled=1 and CategoryEnabled=1 and [Settings].[CountCategoriesByProduct](Product.ProductID) > 0 order by {0}";
            //switch (type)
            //{
            //    case TypeFlag.Bestseller:
            //        sqlCmd = string.Format(sqlCmd, "Bestseller=1", "SortBestseller");
            //        break;
            //    case TypeFlag.New:
            //        sqlCmd = string.Format(sqlCmd, "New=1", "SortNew, [DateModified] DESC");
            //        break;
            //    case TypeFlag.Discount:
            //        sqlCmd = string.Format(sqlCmd, "Discount>0", "SortDiscount");
            //        break;
            //    case TypeFlag.OnSale:
            //        sqlCmd = string.Format(sqlCmd, "OnSale=1", "[DateModified] DESC");
            //        break;
            //    case TypeFlag.Recomended:
            //        sqlCmd = string.Format(sqlCmd, "Recomended=1", "[DateModified] DESC");
            //        break;

            //    default:
            //        throw new NotImplementedException();
            //}

            sqlCmd = string.Format(sqlCmd, "[PopularityManually] DESC");

            return SQLDataAccess.ExecuteTable(sqlCmd, CommandType.Text,
                                              new SqlParameter { ParameterName = "@count", Value = count },
                                              new SqlParameter("@CustomerId", CustomerContext.CustomerId.ToString()),
                                              new SqlParameter("@Type", PhotoType.Product.ToString()),
                                              new SqlParameter("@ShoppingCartType", (int)ShoppingCartType.Compare));
        }



        public static DataTable GetAdminProductsByType(TypeFlag type, int count)
        {
            string sqlCmd;
            switch (type)
            {
                case TypeFlag.Bestseller:
                    sqlCmd = "select Top(@count) Product.ProductId, Name from Catalog.Product where Bestseller=1 order by SortBestseller";
                    break;
                case TypeFlag.New:
                    sqlCmd = "select Top(@count) Product.ProductId, Name from Catalog.Product where New=1 order by SortNew";
                    break;
                case TypeFlag.Discount:
                    sqlCmd = "select Top(@count) Product.ProductId, Name from Catalog.Product where Discount > 0 order by SortDiscount";
                    break;
                case TypeFlag.OnSale:
                    sqlCmd = "select Top(@count) Product.ProductId, Name from Catalog.Product where OnSale=1 order by SortOnSale";
                    break;
                case TypeFlag.Recomended:
                    sqlCmd = "select Top(@count) Product.ProductId, Name from Catalog.Product where Recomended=1 order by SortOnRecomended";
                    break;

                default:
                    throw new NotImplementedException();
            }
            return SQLDataAccess.ExecuteTable(sqlCmd, CommandType.Text, new SqlParameter { ParameterName = "@count", Value = count });
        }

        public static int GetProductCountByType(TypeFlag type)
        {
            string sqlCmd = "select Count(ProductId) from Catalog.Product where Enabled=1 and CategoryEnabled=1 and {0}";
            switch (type)
            {
                case TypeFlag.Bestseller:
                    sqlCmd = string.Format(sqlCmd, "bestseller=1");
                    break;
                case TypeFlag.New:
                    sqlCmd = string.Format(sqlCmd, "new=1");
                    break;
                case TypeFlag.Discount:
                    sqlCmd = string.Format(sqlCmd, "Discount > 0");
                    break;
                case TypeFlag.OnSale:
                    sqlCmd = string.Format(sqlCmd, "OnSale = 1");
                    break;
                case TypeFlag.Recomended:
                    sqlCmd = string.Format(sqlCmd, "Recomended = 1");
                    break;
                default:
                    throw new NotImplementedException();
            }
            return SQLDataAccess.ExecuteScalar<int>(sqlCmd, CommandType.Text);
        }

        public static void AddProductByType(int productId, int sortOrder, TypeFlag type)
        {
            string sqlCmd;
            switch (type)
            {
                case TypeFlag.Bestseller:
                    sqlCmd = "Update Catalog.Product set SortBestseller=@Sort, Bestseller=1 where ProductId=@productId";
                    break;
                case TypeFlag.New:
                    sqlCmd = "Update Catalog.Product set SortNew=@Sort, New=1 where ProductId=@productId";
                    break;
                case TypeFlag.Discount:
                    sqlCmd = "Update Catalog.Product set SortDiscount=@Sort where ProductId=@productId";
                    break;
                case TypeFlag.OnSale:
                    sqlCmd = "Update Catalog.Product set SortOnSale=@Sort, OnSale=1 where ProductId=@productId";
                    break;
                case TypeFlag.Recomended:
                    sqlCmd = "Update Catalog.Product set SortRecomended=@Sort, Recomended=1 where ProductId=@productId";
                    break;
                default:
                    throw new NotImplementedException();
            }
            SQLDataAccess.ExecuteNonQuery(sqlCmd, CommandType.Text,
                                            new SqlParameter { ParameterName = "@productId", Value = productId },
                                            new SqlParameter { ParameterName = "@Sort", Value = sortOrder }
                                            );
        }

        public static void AddProductByType(int productId, TypeFlag type)
        {
            string sqlCmd;
            switch (type)
            {
                case TypeFlag.Bestseller:
                    sqlCmd = "Update Catalog.Product set SortBestseller=(Select min(SortBestseller)-10 from Catalog.Product), Bestseller=1 where ProductId=@productId";
                    break;
                case TypeFlag.New:
                    sqlCmd = "Update Catalog.Product set SortNew=(Select min(SortNew)-10 from Catalog.Product), New=1 where ProductId=@productId";
                    break;
                case TypeFlag.Discount:
                    sqlCmd = "Update Catalog.Product set SortDiscount=(Select min(SortDiscount)-10 from Catalog.Product) where ProductId=@productId";
                    break;
                case TypeFlag.OnSale:
                    sqlCmd = "Update Catalog.Product set SortOnSale=(Select min(SortOnSale)-10 from Catalog.Product), OnSale=1 where ProductId=@productId";
                    break;
                case TypeFlag.Recomended:
                    sqlCmd = "Update Catalog.Product set SortRecomended=(Select min(SortRecomended)-10 from Catalog.Product), Recomended=1 where ProductId=@productId";
                    break;
                default:
                    throw new NotImplementedException();
            }
            SQLDataAccess.ExecuteNonQuery(sqlCmd, CommandType.Text, new SqlParameter { ParameterName = "@productId", Value = productId });
        }

        public static void DeleteProductByType(int prodcutId, TypeFlag type)
        {
            string sqlCmd;
            switch (type)
            {
                case TypeFlag.Bestseller:
                    sqlCmd = "Update Catalog.Product set SortBestseller=0, Bestseller=0 where ProductId=@productId";
                    break;
                case TypeFlag.New:
                    sqlCmd = "Update Catalog.Product set SortNew=0, New=0 where ProductId=@productId";
                    break;
                case TypeFlag.Discount:
                    sqlCmd = "Update Catalog.Product set SortDiscount=0, Discount =0 where ProductId=@productId";
                    break;
                case TypeFlag.OnSale:
                    sqlCmd = "Update Catalog.Product set SortOnSale=0, OnSale=0 where ProductId=@productId";
                    break;
                case TypeFlag.Recomended:
                    sqlCmd = "Update Catalog.Product set SortRecomended=0, Recomended=0 where ProductId=@productId";
                    break;
                default:
                    throw new NotImplementedException();
            }

            SQLDataAccess.ExecuteNonQuery(sqlCmd, CommandType.Text,
                                            new SqlParameter { ParameterName = "@productId", Value = prodcutId }
                                            );
        }

        public static void UpdateProductByType(int productId, int sortOrder, TypeFlag type)
        {
            string sqlCmd;
            switch (type)
            {
                case TypeFlag.Bestseller:
                    sqlCmd = "Update Catalog.Product set SortBestseller=@sortOrder where ProductId=@productId and Bestseller=1";
                    break;
                case TypeFlag.New:
                    sqlCmd = "Update Catalog.Product set SortNew=@sortOrder where ProductId=@productId and New=1";
                    break;
                case TypeFlag.Discount:
                    sqlCmd = "Update Catalog.Product set SortDiscount=@sortOrder where ProductId=@productId";
                    break;
                case TypeFlag.OnSale:
                    sqlCmd = "Update Catalog.Product set SortOnSale=@sortOrder where ProductId=@productId and OnSale=1";
                    break;
                case TypeFlag.Recomended:
                    sqlCmd = "Update Catalog.Product set SortRecomended=@recomendedOrder where ProductId=@productId and Recomended=1";
                    break;
                default:
                    throw new NotImplementedException();
            }

            SQLDataAccess.ExecuteNonQuery(sqlCmd, CommandType.Text,
                                            new SqlParameter { ParameterName = "@productId", Value = productId },
                                            new SqlParameter { ParameterName = "@sortOrder", Value = sortOrder }
                                            );
        }
    }
}