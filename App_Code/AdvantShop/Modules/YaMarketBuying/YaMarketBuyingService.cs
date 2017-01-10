//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------


using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Helpers;

namespace AdvantShop.Modules
{
    public class YaMarketByuingService
    {
        public static bool InstallModule()
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Module.YaMarketShippings') AND type in (N'U'))" +
                "Begin " +
                @"CREATE TABLE Module.YaMarketShippings
	                (
	                Id int NOT NULL IDENTITY (1, 1),
	                ShippingMethodId int NOT NULL,
	                Type nvarchar(25) NOT NULL,
	                MinDate int NOT NULL,
	                MaxDate int NOT NULL
	                )  ON [PRIMARY]

                ALTER TABLE Module.YaMarketShippings ADD CONSTRAINT
	                PK_YaMarketShippings PRIMARY KEY CLUSTERED 
	                (Id) WITH(STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

                ALTER TABLE Module.YaMarketShippings ADD CONSTRAINT
	                FK_YaMarketShippings_ShippingMethod FOREIGN KEY (ShippingMethodId) REFERENCES [Order].ShippingMethod
	                (ShippingMethodID) ON UPDATE  NO ACTION 
	                 ON DELETE  CASCADE  " +
                "End",
                CommandType.Text);

            ModulesRepository.ModuleExecuteNonQuery(
                "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Module.YaMarketOrder') AND type in (N'U'))" +
                "Begin " +
                @"CREATE TABLE Module.YaMarketOrder
                    (
                    MarketOrderId int NOT NULL,
                    OrderId int NOT NULL,
                    Status [nvarchar](max) NOT NULL
                    )  ON [PRIMARY] " +
                "End",
                CommandType.Text);

            YaMarketBuyingSettings.AuthToken = "";
            YaMarketBuyingSettings.Payments = "";
            YaMarketBuyingSettings.Outlets = "";

            YaMarketBuyingSettings.AuthClientId = "";
            YaMarketBuyingSettings.AuthTokenToMarket = "";
            YaMarketBuyingSettings.Login = "";
            YaMarketBuyingSettings.CampaignId = "";

            YaMarketBuyingSettings.UpaidStatusId = 0;
            YaMarketBuyingSettings.ProcessingStatusId = 0;
            YaMarketBuyingSettings.DeliveryStatusId = 0;
            YaMarketBuyingSettings.DeliveredStatusId = 0;

            return true;
        }


        public static List<YaMarketShipping> GetShippings()
        {
            return ModulesRepository.ModuleExecuteReadList("Select * From Module.YaMarketShippings", CommandType.Text,
                reader => new YaMarketShipping()
                {
                    Id = SQLDataHelper.GetInt(reader, "Id"),
                    ShippingMethodId = SQLDataHelper.GetInt(reader, "ShippingMethodId"),
                    Type = SQLDataHelper.GetString(reader, "Type"),
                    MinDate = SQLDataHelper.GetInt(reader, "MinDate"),
                    MaxDate = SQLDataHelper.GetInt(reader, "MaxDate")
                });
        }

        public static void AddShipping(YaMarketShipping shipping)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "INSERT INTO [Module].[YaMarketShippings] ([ShippingMethodId],[Type],[MinDate],[MaxDate]) VALUES(@ShippingMethodId,@Type,@MinDate,@MaxDate)", 
                CommandType.Text,
                new SqlParameter("@ShippingMethodId", shipping.ShippingMethodId),
                new SqlParameter("@Type", shipping.Type),
                new SqlParameter("@MinDate", shipping.MinDate),
                new SqlParameter("@MaxDate", shipping.MaxDate));
        }

        public static void DeleteShippings()
        {
            ModulesRepository.ModuleExecuteNonQuery("Delete From [Module].[YaMarketShippings]", CommandType.Text);
        }

        public static YaOrder GetOrder(int marketOrderId)
        {
            return ModulesRepository.ModuleExecuteReadOne(
                "Select * From Module.YaMarketOrder Where MarketOrderId = @MarketOrderId", CommandType.Text,
                reader => new YaOrder()
                {
                    MarketOrderId = SQLDataHelper.GetInt(reader, "MarketOrderId"),
                    OrderId = SQLDataHelper.GetInt(reader, "OrderId"),
                    Status = SQLDataHelper.GetString(reader, "Status"),
                },
                new SqlParameter("@MarketOrderId", marketOrderId));
        }

        public static int GetMarketOrderId(int orderId)
        {
            return ModulesRepository.ModuleExecuteScalar<int>(
                "Select MarketOrderId From Module.YaMarketOrder Where OrderId = @OrderId", CommandType.Text,
                new SqlParameter("@OrderId", orderId));
        }

        public static void AddOrder(YaOrder order)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "Insert Into Module.YaMarketOrder (MarketOrderId,OrderId,Status) VALUES (@MarketOrderId, @OrderId, @Status)", CommandType.Text,
                new SqlParameter("@MarketOrderId", order.MarketOrderId),
                new SqlParameter("@OrderId", order.OrderId),
                new SqlParameter("@Status", order.Status ?? string.Empty));
        }

        public static void UpdateOrder(YaOrder order)
        {
            ModulesRepository.ModuleExecuteNonQuery(
                "Update Module.YaMarketOrder Set Status=@Status Where MarketOrderId=@MarketOrderId and OrderId=@OrderId", CommandType.Text,
                new SqlParameter("@MarketOrderId", order.MarketOrderId),
                new SqlParameter("@OrderId", order.OrderId),
                new SqlParameter("@Status", order.Status ?? string.Empty));
        }

        public static DataTable GetHistory()
        {
            return ModulesRepository.ModuleExecuteTable(
                "Select [Order].OrderId, MarketOrderId, Status, OrderDate, Sum "+
                "From [Module].[YaMarketOrder] Left Join [Order].[Order] On [YaMarketOrder].[OrderId] = [Order].[OrderId]", 
                CommandType.Text);
        }
    }
}