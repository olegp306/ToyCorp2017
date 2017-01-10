//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Customers
{
    public class RecentlyViewService
    {

        /// <summary>
        /// Загружает данные о просмотрах для текущего пользователя, удаляет старые, неактивыне товары и товары без категорий
        /// </summary>
        /// <remarks></remarks>
        public static List<RecentlyViewRecord> LoadViewDataByCustomer(Guid customerId, int rowsCount)
        {
            return SQLDataAccess.ExecuteReadList<RecentlyViewRecord>("[Customers].[sp_GetRecentlyView]", CommandType.StoredProcedure,
                                        ReadViewRecordFromReader,
                                        new SqlParameter("@CustomerId", customerId),
                                        new SqlParameter("@rowsCount", rowsCount),
                                        new SqlParameter("@Type", PhotoType.Product.ToString())
                                        );
        }

        /// <summary>
        /// Метод загружает данные о просмотрах для  пользователя из полученного SqlReader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static RecentlyViewRecord ReadViewRecordFromReader(SqlDataReader reader)
        {
            return new RecentlyViewRecord
                          {
                              ProductID = SQLDataHelper.GetInt(reader["ProductID"]),
                              ViewTime = SQLDataHelper.GetDateTime(reader["ViewDate"]),
                              ImgPath = SQLDataHelper.GetString(reader["Photo"]),
                              PhotoDesc = SQLDataHelper.GetString(reader["PhotoDesc"]),
                              Name = SQLDataHelper.GetString(reader["Name"]),
                              Price = SQLDataHelper.GetFloat(reader["Price"]),
                              Discount = SQLDataHelper.GetFloat(reader["Discount"]),
                              UrlPath = SQLDataHelper.GetString(reader["UrlPath"]),
                              Ratio = SQLDataHelper.GetFloat(reader["Ratio"]),
                              RatioID = SQLDataHelper.GetInt(reader["RatioID"]),
                              MultiPrice = SQLDataHelper.GetFloat(reader["MultiPrice"]),
                          };
        }

        public static void SetRecentlyView(Guid customerId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery("if (SELECT Count(ViewDate) FROM [Customers].[RecentlyViewsData] WHERE (CustomerID=@CustomerId) AND (ProductID=@ProductId)) > 0 " +
                                           "begin UPDATE [Customers].[RecentlyViewsData] SET ViewDate = GetDate() WHERE (CustomerID = @CustomerId) AND (ProductID = @ProductId) end " +
                                           "else begin INSERT INTO [Customers].[RecentlyViewsData](CustomerID, ProductID, ViewDate) VALUES (@CustomerId, @ProductId, GetDate()) end",
                                                        CommandType.Text,
                                                        new SqlParameter("@CustomerId", customerId),
                                                        new SqlParameter("@ProductId", productId)
                                                        );
        }

        public static void DeleteExpired()
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Customers].[RecentlyViewsData] where GetDate() > DATEADD(week, 1, ViewDate)", CommandType.Text);
        }
    }
}