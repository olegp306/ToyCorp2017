//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;

namespace AdvantShop.Statistic
{
    public class StatisticService
    {
        #region Search statistic

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="description"></param>
        /// <param name="resultCount"></param>
        /// <param name="searchTerm"></param>
        /// <param name="customerId"></param>
        public static void AddSearchStatistic(string request, string searchTerm, string description, int resultCount, Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(@"IF (SELECT COUNT(ID) FROM [Statistic].[SearchStatistic] WHERE SearchTerm = @SearchTerm AND Description = @Description AND CustomerID = @CustomerID) = 0 Begin
                INSERT INTO [Statistic].[SearchStatistic] ([Request],[ResultCount],[Date],[SearchTerm],[Description],[CustomerID]) VALUES (@Request, @Resultcount, GETDATE(), @SearchTerm, @Description,@CustomerID) end ",
                CommandType.Text,
                new SqlParameter("@Request", request),
                new SqlParameter("@Resultcount", resultCount),
                new SqlParameter("@SearchTerm", searchTerm),
                new SqlParameter("@Description", description),
                new SqlParameter("@CustomerID", customerId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DataTable GetHistorySearchStatistic(int numRows)
        {
            return SQLDataAccess.ExecuteTable(
                "SELECT TOP(@NumRows) * FROM [Statistic].[SearchStatistic] ORDER BY Date DESC",
                CommandType.Text,
                new SqlParameter("@NumRows", numRows));
        }

        /// <summary>
        /// Statistic by frequency search
        /// </summary>
        /// <returns></returns>
        public static DataTable GetFrequencySearchStatistic(DateTime date)
        {
            return SQLDataAccess.ExecuteTable(
                    "SELECT [Request], COUNT([Request]) AS numOfRequest, [ResultCount],[SearchTerm],[Description] FROM [Statistic].[SearchStatistic] WHERE [Date] >= Convert(date, @Date) GROUP BY [Request],[ResultCount],[SearchTerm],[Description] ORDER BY numOfRequest DESC",
                    CommandType.Text,
                    new SqlParameter("@Date", date));
        }
        #endregion

        #region Common statistic

        public static int GetOrdersCountByDateRange(DateTime fromDate, DateTime toDate)
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "SELECT COUNT(OrderID) FROM [Order].[Order] WHERE [OrderDate] IS NOT NULL AND Convert(date, @FromDate) <= Convert(date, [OrderDate]) AND Convert(date, [OrderDate]) <=  Convert(date, @ToDate)",
                CommandType.Text,
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)));
        }

        public static Int64 GetOrdersSumByDateRange(DateTime fromDate, DateTime toDate)
        {
            return Convert.ToInt64(SQLDataAccess.ExecuteScalar(
                "SELECT isnull(Sum([order].[sum]), 0) FROM [Order].[Order] WHERE [OrderDate] IS NOT NULL AND Convert(date, @FromDate) <= Convert(date, [OrderDate]) AND Convert(date, [OrderDate]) <=  Convert(date, @ToDate)",
                CommandType.Text,
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)));
        }



        public static int GetOrdersCountByDate(DateTime date)
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "SELECT COUNT(OrderID) FROM [Order].[Order]  WHERE Convert(date, [OrderDate]) = Convert(date, @Date)",
                CommandType.Text,
                new SqlParameter("@Date", date)));
        }


        public static int GetOrdersSumByDate(DateTime date)
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "SELECT isnull(Sum([order].[sum]), 0) FROM [Order].[Order] WHERE [OrderDate] IS NOT NULL AND Convert(date, [OrderDate]) = Convert(date, @Date)",
                CommandType.Text,
                new SqlParameter("@Date", date)));
        }


        public static int GetOrdersCount()
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "SELECT COUNT(OrderID) FROM [Order].[Order]",
                CommandType.Text));
        }


        public static int GetProductsCount()
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "SELECT COUNT(ProductID) FROM [Catalog].[Product]",
                CommandType.Text));
        }

        /// <summary>
        /// get orders with order status @default order status@
        /// </summary>
        /// <returns>orders count</returns>
        public static int GetLastOrdersCount()
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "SELECT COUNT(OrderID) FROM [Order].[Order] WHERE [OrderStatusID] = (SELECT [OrderStatusID] FROM [Order].[OrderStatus] WHERE [IsDefault] = 1)",
                CommandType.Text));
        }


        /// <summary>
        /// get reviews count
        /// </summary>
        /// <returns></returns>
        public static int GetReviewsCount()
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "SELECT COUNT(ReviewID) FROM [CMS].[Review]",
                CommandType.Text));
        }

        /// <summary>
        /// get last reviews count
        /// </summary>
        /// <returns></returns>
        public static int GetLastReviewsCount()
        {
            return Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                "SELECT COUNT(ReviewID) FROM [CMS].[Review] WHERE [Checked] = 0",
                CommandType.Text));
        }

        #endregion

    }
}