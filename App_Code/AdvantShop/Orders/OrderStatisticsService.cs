//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Orders
{
    public class OrderStatisticsService
    {
        private static float? _salesPlan;
        private static float? _profitPlan;

        public static float SalesPlan
        {
            get
            {
                if (_salesPlan != null)
                    return _salesPlan.Value;

                GetProfitPlan();

                return _salesPlan != null ? _salesPlan.Value : 0;
            }
            set { _salesPlan = value; }
        }

        public static float ProfitPlan
        {
            get
            {
                if (_profitPlan != null)
                    return _profitPlan.Value;
                GetProfitPlan();
                return _profitPlan != null ? _profitPlan.Value : 0;
            }
            set { _profitPlan = value; }
        }

        public static Dictionary<DateTime, float> GetOrdersSumByDays(DateTime minDate, DateTime maxDate)
        {
            var sums = new Dictionary<DateTime, float>();
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetSumByDays]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cmd.Parameters.AddWithValue("@MinDate", minDate);
                db.cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        sums.Add(SQLDataHelper.GetDateTime(reader, "Date"), SQLDataHelper.GetFloat(reader, "Sum"));
                    }
                db.cnClose();
                return sums;
            }
        }

        public static Dictionary<DateTime, float> GetOrdersProfitByDays(DateTime minDate, DateTime maxDate)
        {
            var sums = new Dictionary<DateTime, float>();
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetProfitByDays]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cmd.Parameters.AddWithValue("@MinDate", minDate);
                db.cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        sums.Add(SQLDataHelper.GetDateTime(reader, "Date"), SQLDataHelper.GetFloat(reader, "Profit"));
                    }
                db.cnClose();
                return sums;
            }
        }

        public static Dictionary<DateTime, int> GetOrdersCountByPeriod(DateTime minDate, DateTime maxDate)
        {
            var sums = new Dictionary<DateTime, int>();
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetCountByMonths]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cmd.Parameters.AddWithValue("@MinDate", minDate);
                db.cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        sums.Add(
                            new DateTime(SQLDataHelper.GetInt(reader, "Year"), SQLDataHelper.GetInt(reader, "Month"), 1),
                            SQLDataHelper.GetInt(reader, "Count"));
                    }
                db.cnClose();
                return sums;
            }
        }


        public static List<KeyValuePair<string, int>> GetTopPayments()
        {
            return SQLDataAccess.ExecuteReadList("[Order].[sp_GetPaymentRating]", CommandType.StoredProcedure,
                reader =>
                    new KeyValuePair<string, int>(SQLDataHelper.GetString(reader, "PaymentMethod"),
                                                  SQLDataHelper.GetInt(reader, "Rating")));
        }

        public static List<KeyValuePair<string, int>> GetTopShippings()
        {
            return SQLDataAccess.ExecuteReadList("[Order].[sp_GetShippingRating]", CommandType.StoredProcedure,
                reader =>
                    new KeyValuePair<string, int>(SQLDataHelper.GetString(reader, "ShippingMethod"),
                                                  SQLDataHelper.GetInt(reader, "Rating")));
        }

        public static List<KeyValuePair<string, int>> GetTopCities()
        {
            return SQLDataAccess.ExecuteReadList(
                "SELECT Top(10) [City], Count(*) as Rating " +
                "FROM [Order].[OrderContact] " +
	            "Group BY [City] " +
	            "Order By Rating Desc", 
                CommandType.Text,
                reader =>
                    new KeyValuePair<string, int>(SQLDataHelper.GetString(reader, "City"),
                                                  SQLDataHelper.GetInt(reader, "Rating")));
        }

        public static DataTable GetTopCustomersBySumPrice()
        {
            return SQLDataAccess.ExecuteTable(
                "Select Top(10) [CustomerID], [Email], " +
                "(Select top 1 [FirstName]+' '+[LastName] From [Order].[OrderCustomer] as c Where c.[CustomerID] = [OrderCustomer].[CustomerID]) as fio, " +
                "Sum([Order].[Sum]) as Summary " +
                "From [Order].[OrderCustomer] " +
                "Join [Order].[Order] On [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                "Group By [CustomerID], Email " +
                "Order By Summary Desc",
                CommandType.Text);
        }

        public static DataTable GetTopProductsByCount()
        {
            return SQLDataAccess.ExecuteTable(
                "Select Top(10) ProductID, Name, ArtNo, " +
                                "(Select [UrlPath] From [Catalog].[Product] Where [ProductId] = [OrderItems].[ProductID]) as UrlPath, " +
                                "Sum([Amount]) as Summary " +
                "From [Order].[OrderItems] " +
                "Group By [ProductID], [Name], [ArtNo] " +
                "Order By Summary Desc",
                CommandType.Text);
        }

        public static DataTable GetTopProductsBySum()
        {
            return SQLDataAccess.ExecuteTable(
                "Select Top(10) ProductID, Name, ArtNo, " +
                                "(Select [UrlPath] From [Catalog].[Product] Where [ProductId] = [OrderItems].[ProductID]) as UrlPath, " +
                                " Sum([Amount]*[Price]) as Summary " +
                "From [Order].[OrderItems] " +
                "Group By [ProductID], [Name], [ArtNo] " +
                "Order By Summary Desc",
                CommandType.Text);
        }

        public static KeyValuePair<float, float> GetMonthProgress()
        {
            return SQLDataAccess.ExecuteReadOne("[Order].[sp_GetOrdersMonthProgress]", CommandType.StoredProcedure,
                reader =>
                    new KeyValuePair<float, float>(SQLDataHelper.GetFloat(reader, "Sum"),
                                                   SQLDataHelper.GetFloat(reader, "Profit")));
        }

        public static void GetProfitPlan()
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Settings].[sp_GetLastProfitPlan]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                {
                    reader.Read();
                    SalesPlan = SQLDataHelper.GetFloat(reader, "SalesPlan");
                    ProfitPlan = SQLDataHelper.GetFloat(reader, "ProfitPlan");
                }
                db.cnClose();
            }
        }

        public static void SetProfitPlan(float sales, float profit)
        {
            SQLDataAccess.ExecuteNonQuery("[Settings].[sp_SetPlan]", CommandType.StoredProcedure,
                                            new SqlParameter("@SalesPlan", sales), 
                                            new SqlParameter("@ProfitPlan", profit));
            GetProfitPlan();
        }


        public static Dictionary<DateTime, float> GetOrdersSumGroupByDay(string group, DateTime minDate, DateTime maxDate,
                                                                    bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', SUM([Sum]) as 'Sum' " +
                "FROM [Order].[Order] " +
                "WHERE [OrderDate] > @MinDate and [OrderDate] <= @MaxDate " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool) onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0)",
                CommandType.Text,
                "Date", "Sum",
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate));
        }

        public static Dictionary<DateTime, float> GetOrdersCountGroupByDay(string group, DateTime minDate, DateTime maxDate,
                                                                        bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', Count([OrderID]) as Count " +
                "FROM [Order].[Order] " +
                "WHERE [OrderDate] > @MinDate and [OrderDate] <= @MaxDate " +

                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0)",
                CommandType.Text,
                "Date", "Count",
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate));
        }

        public static Dictionary<DateTime, float> GetOrdersRegGroupByDay(string group, DateTime minDate, DateTime maxDate, bool isRegistered,
                                                                        bool? onlyPayed = null, int? orderStatusId = null)
        {
            return SQLDataAccess.ExecuteReadDictionary<DateTime, float>(
                "Select DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) as 'Date', Count([Order].[OrderID]) as Count " +
                "FROM [Order].[Order] " +
                "Inner Join [Order].[OrderCustomer] On [Order].[OrderID] = [OrderCustomer].[OrderId] " +
                "Left Join [Customers].[Customer] On [Customer].[CustomerID] = [OrderCustomer].[CustomerID] " +
                "WHERE [OrderDate] > @MinDate and [OrderDate] <= @MaxDate " +
                
                (isRegistered ? "and [Customer].[Email] is not null " : "and [Customer].[Email] is null ") +
                (onlyPayed != null ? (" and [PaymentDate] is " + ((bool)onlyPayed ? "not" : "") + " null ") : "") +
                (orderStatusId != null ? " and [OrderStatusID] = " + orderStatusId : "") +

                "GROUP BY DATEADD(" + group + ", DATEDIFF(" + group + ", 0, [OrderDate]), 0) " +
                "Order By Date",
                CommandType.Text,
                "Date", "Count",
                new SqlParameter("@MinDate", minDate),
                new SqlParameter("@MaxDate", maxDate));
        }
    }
}