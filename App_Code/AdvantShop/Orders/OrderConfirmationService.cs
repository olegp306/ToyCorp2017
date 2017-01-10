//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using Newtonsoft.Json;

namespace AdvantShop.Orders
{
    public class OrderConfirmationService
    {

        public static int OrderID
        {
            get
            {
                return HttpContext.Current.Session["OrderID"] != null ? (int) HttpContext.Current.Session["OrderID"] : 0;
            }
            set 
            {
                if (value == 0)
                {
                    HttpContext.Current.Session.Remove("OrderID");
                }
                else
                {
                    HttpContext.Current.Session["OrderID"] = value;
                }
            }
        }

        public static bool IsExist(Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "Select Count(CustomerId) from [Order].OrderConfirmation where CustomerId=@CustomerId", CommandType.Text,
                new SqlParameter("@CustomerId", customerId)) > 0;
        }

        public static OrderConfirmation Get(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadOne("Select * from [Order].OrderConfirmation where CustomerId=@CustomerId",
                CommandType.Text, GetFromReader,
                new SqlParameter("@CustomerId", customerId));
        }

        public static void Add(OrderConfirmation item)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Insert into [Order].OrderConfirmation ([CustomerId],[OrderConfirmationData],LastUpdate) values (@CustomerId,@OrderConfirmationData,GetDate())",
                CommandType.Text,
                new SqlParameter("@CustomerId", item.CustomerId),
                new SqlParameter("@OrderConfirmationData", JsonConvert.SerializeObject(item.OrderConfirmationData)));
        }

        public static void Update(OrderConfirmation item)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Order].OrderConfirmation set OrderConfirmationData=@OrderConfirmationData,LastUpdate=GetDate() where CustomerId=@CustomerId",
                CommandType.Text,
                new SqlParameter("@CustomerId", item.CustomerId),
                new SqlParameter("@OrderConfirmationData", JsonConvert.SerializeObject(item.OrderConfirmationData)));
        }

        public static void Delete(Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].OrderConfirmation Where CustomerId=@CustomerId",
                CommandType.Text, new SqlParameter("@CustomerId", customerId));
        }

        public static void DeleteExpired(DateTime olderThan)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete from [Order].OrderConfirmation Where LastUpdate<@olderThan",
                CommandType.Text, new SqlParameter("@olderThan", olderThan));
        }

        private static OrderConfirmation GetFromReader(SqlDataReader reader)
        {
            return new OrderConfirmation
            {
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                OrderConfirmationData =
                    JsonConvert.DeserializeObject<OrderConfirmationData>(SQLDataHelper.GetString(reader, "OrderConfirmationData")),
                LastUpdate = SQLDataHelper.GetDateTime(reader, "LastUpdate", DateTime.MinValue)
            };
        }
    }
}