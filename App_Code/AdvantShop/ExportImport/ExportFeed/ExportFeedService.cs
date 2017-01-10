using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core;
using AdvantShop.Core.SQL;

namespace AdvantShop.ExportImport
{

    public class ExportFeedService
    {
        public static bool CheckCategory(string modulename, int catId)
        {

            if (String.IsNullOrWhiteSpace(modulename)) return false;
            return SQLDataAccess.ExecuteScalar<int>("Select count(*) from Settings.ExportFeedSelectedCategories where ModuleName=@ModuleName and CategoryID=@CategoryID",
                                                    CommandType.Text,
                                                    new SqlParameter("@ModuleName", modulename),
                                                    new SqlParameter("@CategoryID", catId)) > 0;
        }

        public static bool CheckCategoryHierical(string modulename, int catId)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "Select count(*) from Settings.ExportFeedSelectedCategories where ModuleName=@ModuleName " +
                    " and CategoryID in (Select id from [Settings].[GetParentsCategoryByChild](@CategoryID) union select 0) and CategoryID<> @CategoryID",
                    CommandType.Text, new SqlParameter("@ModuleName", modulename), new SqlParameter("@CategoryID", catId)) > 0;
        }

        public static void InsertCategory(string modulename, int catId)
        {

            if (String.IsNullOrWhiteSpace(modulename)) return;
            if (CheckCategory(modulename, catId)) return;
            SQLDataAccess.ExecuteScalar<int>("Insert into Settings.ExportFeedSelectedCategories (ModuleName, CategoryID) VALUES (@ModuleName, @CategoryID)",
                                                   CommandType.Text,
                                                   new SqlParameter("@ModuleName", modulename),
                                                   new SqlParameter("@CategoryID", catId));
        }

        public static void DeleteCategory(string modulename, int catId)
        {
            if (String.IsNullOrWhiteSpace(modulename)) return;
            SQLDataAccess.ExecuteScalar<int>("Delete from Settings.ExportFeedSelectedCategories where ModuleName=@ModuleName and CategoryID=@CategoryID",
                                                   CommandType.Text,
                                                   new SqlParameter("@ModuleName", modulename),
                                                   new SqlParameter("@CategoryID", catId));
        }


        public static bool CheckProduct(string modulename, int proId)
        {

            if (String.IsNullOrWhiteSpace(modulename)) return false;
            return SQLDataAccess.ExecuteScalar<int>("Select count(*) from Settings.ExportFeedSelectedProducts where ModuleName=@ModuleName and ProductID=@ProductID",
                                                    CommandType.Text,
                                                    new SqlParameter("@ModuleName", modulename),
                                                    new SqlParameter("@ProductID", proId)) > 0;
        }

        public static void InsertProduct(string modulename, int proId)
        {
            if (CheckProduct(modulename, proId)) return;
            if (String.IsNullOrWhiteSpace(modulename)) return;
            SQLDataAccess.ExecuteScalar<int>("Insert into Settings.ExportFeedSelectedProducts (ModuleName, ProductID) VALUES (@ModuleName, @ProductID)",
                                                   CommandType.Text,
                                                   new SqlParameter("@ModuleName", modulename),
                                                   new SqlParameter("@ProductID", proId));
        }

        public static void DeleteProduct(string modulename, int proId)
        {
            if (String.IsNullOrWhiteSpace(modulename)) return;
            SQLDataAccess.ExecuteScalar<int>("Delete from Settings.ExportFeedSelectedProducts where ModuleName=@ModuleName and ProductID=@ProductID",
                                                   CommandType.Text,
                                                   new SqlParameter("@ModuleName", modulename),
                                                   new SqlParameter("@ProductID", proId));
        }

        public static void DeleteModule(string modulename)
        {
            if (String.IsNullOrWhiteSpace(modulename)) return;
            SQLDataAccess.ExecuteScalar<int>("Delete from Settings.ExportFeedSelectedProducts where ModuleName=@ModuleName",
                                                   CommandType.Text, new SqlParameter("@ModuleName", modulename));
            SQLDataAccess.ExecuteScalar<int>("Delete from Settings.ExportFeedSelectedCategories where ModuleName=@ModuleName",
                                                   CommandType.Text, new SqlParameter("@ModuleName", modulename));
        }

        public static int GetCsvCategoriesCount(string moduleName)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select count(CategoryID) from [Settings].[ExportFeedSelectedCategories] where moduleName=@moduleName ", CommandType.Text, new SqlParameter("@moduleName", moduleName));
        }
    }
}