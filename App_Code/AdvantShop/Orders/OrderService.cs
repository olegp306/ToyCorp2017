//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using AdvantShop.BonusSystem;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using Resources;

namespace AdvantShop.Orders
{
    public class OrderService
    {
        private const int LenghtStringOrderId = 6;
        private const string CharsNs = "0123456789ABDEFHJK0123456789MNQRSTWXYZ";

        #region Statuses

        public static int DefaultOrderStatus
        {
            get
            {
                return SQLDataAccess.ExecuteScalar<int>(
                    "SELECT OrderStatusID FROM [Order].[OrderStatus] WHERE [IsDefault] = 'True'",
                    CommandType.Text);
            }
        }

        public static int CanceledOrderStatus
        {
            get
            {
                return SQLDataAccess.ExecuteScalar<int>(
                    "SELECT OrderStatusID FROM [Order].[OrderStatus] WHERE [IsCanceled] = 'True'",
                    CommandType.Text);
            }
        }

        //todo Vladimir пересмотреть и перенести в грид
        public static bool StatusCanBeDeleted(int statusId)
        {
            if (statusId == DefaultOrderStatus)
                return false;
            return GetOrderCountByStatusId(statusId) <= 0;
        }


        public static List<OrderStatus> GetOrderStatuses()
        {
            return SQLDataAccess.ExecuteReadList("SELECT * FROM [Order].OrderStatus Order By SortOrder", CommandType.Text,
                                                 GetOrderStatusFromReader);
        }

        private static OrderStatus GetOrderStatusFromReader(SqlDataReader reader)
        {
            return new OrderStatus
                {
                    StatusID = SQLDataHelper.GetInt(reader, "OrderStatusID"),
                    StatusName = SQLDataHelper.GetString(reader, "StatusName"),
                    Command = (OrderStatusCommand)SQLDataHelper.GetInt(reader, "CommandID"),
                    IsCanceled = SQLDataHelper.GetBoolean(reader, "IsCanceled"),
                    IsDefault = SQLDataHelper.GetBoolean(reader, "IsDefault"),
                    Color = SQLDataHelper.GetString(reader, "Color"),
                    SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
                };
        }

        public static string GetStatusName(int idStatus)
        {
            return SQLDataAccess.ExecuteScalar<string>(
                "SELECT StatusName FROM [Order].[OrderStatus]  WHERE OrderStatusID = @OrderStatusID",
                CommandType.Text, new SqlParameter("OrderStatusID", idStatus));
        }

        public static void ChangeOrderStatus(int orderId, int statusId, bool updateModules = true)
        {
            int? command = ChangeOrderStatusInDb(orderId, statusId);
            if (SettingsOrderConfirmation.DecrementProductsCount)
            {
                if (command != null)
                {
                    if (command == (int)OrderStatusCommand.Increment)
                    {
                        IncrementProductsCountAccordingOrder(orderId);
                    }
                    else if (command == (int)OrderStatusCommand.Decrement)
                    {
                        DecrementProductsCountAccordingOrder(orderId);
                    }
                }
            }

            if (BonusSystem.BonusSystem.IsActive)
            {
                var status = GetOrderStatus(statusId);
                if (status != null && status.IsCanceled)
                {
                    BonusSystemService.CancelPurchase(GetOrderNumberById(orderId), orderId);
                }
            }

            if (updateModules)
            {
                var modules = AttachedModules.GetModules<IOrderChanged>();
                if (modules.Any())
                {
                    var status = GetOrderStatus(statusId);
                    var order = GetOrder(orderId);

                    foreach (var module in modules)
                    {
                        var instance = (IOrderChanged)Activator.CreateInstance(module, null);
                        instance.DoOrderChangeStatus(order);
                    }
                }
            }
        }

        private static int? ChangeOrderStatusInDb(int orderId, int statusId)
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("[Order].[sp_GetChangeOrderStatus]", CommandType.StoredProcedure,
                                                         new SqlParameter("@OrderID", orderId),
                                                         new SqlParameter("@OrderStatusID", statusId)));
        }

        public static int AddOrderStatus(OrderStatus status)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Order].[sp_AddOrderStatus]", CommandType.StoredProcedure,
                                                 new SqlParameter("@OrderStatusID", status.StatusID),
                                                 new SqlParameter("@StatusName", status.StatusName),
                                                 new SqlParameter("@CommandID", (int)status.Command),
                                                 new SqlParameter("@IsDefault", status.IsDefault),
                                                 new SqlParameter("@IsCanceled", status.IsCanceled),
                                                 new SqlParameter("@Color", status.Color.IsNotEmpty() ? status.Color : (object)DBNull.Value),
                                                 new SqlParameter("@SortOrder", status.SortOrder)
                                                 );
        }

        public static void UpdateOrderStatus(OrderStatus status)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateOrderStatus]", CommandType.StoredProcedure,
                                                 new SqlParameter("@OrderStatusID", status.StatusID),
                                                 new SqlParameter("@StatusName", status.StatusName),
                                                 new SqlParameter("@CommandID", (int)status.Command),
                                                 new SqlParameter("@IsDefault", status.IsDefault),
                                                 new SqlParameter("@IsCanceled", status.IsCanceled),
                                                 new SqlParameter("@Color", status.Color.IsNotEmpty() ? status.Color : (object)DBNull.Value),
                                                 new SqlParameter("@SortOrder", status.SortOrder)
                                                 );
        }


        public static bool DeleteOrderStatus(int orderStatusId)
        {
            if (!StatusCanBeDeleted(orderStatusId))
                return false;

            return SQLDataAccess.ExecuteScalar<int>(
                "[Order].[sp_DeleteOrderStatus]",
                CommandType.StoredProcedure,
                new SqlParameter("@OrderStatusID", orderStatusId)) == 1;
        }

        public static OrderStatus GetOrderStatus(int orderStatusId)
        {
            return
                    SQLDataAccess.ExecuteReadOne(
                        "SELECT * FROM [Order].[OrderStatus] WHERE [OrderStatusID] = @OrderStatusID",
                        CommandType.Text,
                        GetOrderStatusFromReader,
                        new SqlParameter("@OrderStatusID", orderStatusId));
        }

        #endregion

        #region OrderHistory

        public static OrderHistory GetOrderHistoryFromReader(SqlDataReader reader)
        {
            return new OrderHistory
                       {
                           OrderID = SQLDataHelper.GetInt(reader, "OrderID"),
                           OrderNumber = SQLDataHelper.GetString(reader, "Number"),
                           ShippingMethod = SQLDataHelper.GetString(reader, "ShippingMethod"),
                           ShippingMethodName = SQLDataHelper.GetString(reader, "ShippingMethodName"),
                           ArchivedPaymentName = SQLDataHelper.GetString(reader, "PaymentMethodName"),
                           PaymentMethodID = SQLDataHelper.GetInt(reader, "PaymentMethodID"),
                           Status = SQLDataHelper.GetString(reader, "StatusName"),
                           StatusID = SQLDataHelper.GetInt(reader, "OrderStatusID"),
                           Sum = SQLDataHelper.GetFloat(reader, "Sum"),
                           OrderDate = SQLDataHelper.GetDateTime(reader, "OrderDate"),
                           Payed = SQLDataHelper.GetNullableDateTime(reader, "PaymentDate") != null,
                           ProductsHtml = string.Empty,
                           CurrencyCode = SQLDataHelper.GetString(reader, "CurrencyCode"),
                           CurrencyValue = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                           CurrencySymbol = SQLDataHelper.GetString(reader, "CurrencySymbol"),
                           IsCodeBefore = SQLDataHelper.GetBoolean(reader, "IsCodeBefore")
                       };
        }



        public static IList<OrderHistory> GetCustomerOrderHistory(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadList<OrderHistory>(
                "[Order].[sp_GetCustomerOrderHistory]",
                CommandType.StoredProcedure,
                GetOrderHistoryFromReader,
                new SqlParameter { Value = customerId.ToString(), ParameterName = "@CustomerID" });
        }

        #endregion

        #region OrderItems

        private static OrderItem GetOrderItemFromReader(IDataReader reader)
        {
            return new OrderItem
                       {
                           OrderItemID = SQLDataHelper.GetInt(reader, "OrderItemID"),
                           Name = SQLDataHelper.GetString(reader, "Name"),
                           Price = SQLDataHelper.GetFloat(reader, "Price"),
                           Amount = SQLDataHelper.GetFloat(reader, "Amount"),
                           ProductID = SQLDataHelper.GetInt(reader, "ProductID"),
                           ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                           SupplyPrice = SQLDataHelper.GetFloat(reader, "SupplyPrice"),
                           Weight = SQLDataHelper.GetFloat(reader, "Weight"),
                           Color = SQLDataHelper.GetString(reader, "Color", null),
                           Size = SQLDataHelper.GetString(reader, "Size", null),
                           IsCouponApplied = SQLDataHelper.GetBoolean(reader, "IsCouponApplied"),
                           PhotoID = SQLDataHelper.GetNullableInt(reader, "PhotoID"),
                           DecrementedAmount = SQLDataHelper.GetFloat(reader, "DecrementedAmount"),
                       };
        }

        public static List<OrderItem> GetOrderItems(int orderId)
        {
            var result = new List<OrderItem>();

            using (var da = new SQLDataAccess())
            {
                da.cmd.CommandText = "[Order].[sp_GetOrderItems]";
                da.cmd.CommandType = CommandType.StoredProcedure;

                da.cmd.Parameters.Clear();
                da.cmd.Parameters.AddWithValue("@OrderID", orderId);

                da.cn.Open();

                using (SqlDataReader reader = da.cmd.ExecuteReader())

                    while (reader.Read())
                    {
                        result.Add(GetOrderItemFromReader(reader));
                    }

                da.cmd.CommandText = "[Order].[sp_GetSelectedOptionsByOrderItemId]";
                da.cmd.CommandType = CommandType.StoredProcedure;

                foreach (OrderItem orditm in result)
                {
                    da.cmd.Parameters.Clear();
                    da.cmd.Parameters.AddWithValue("@OrderItemId", orditm.OrderItemID);
                    var evlist = new List<EvaluatedCustomOptions>();
                    using (var reader = da.cmd.ExecuteReader())
                        while (reader.Read())
                        {
                            var ev = new EvaluatedCustomOptions
                                         {
                                             CustomOptionId = SQLDataHelper.GetInt(reader, "CustomOptionId"),
                                             CustomOptionTitle = SQLDataHelper.GetString(reader, "CustomOptionTitle"),
                                             OptionId = SQLDataHelper.GetInt(reader, "OptionId"),
                                             OptionPriceBc = SQLDataHelper.GetFloat(reader, "OptionPriceBC"),
                                             OptionPriceType =
                                                 (OptionPriceType)SQLDataHelper.GetInt(reader, "OptionPriceType"),
                                             OptionTitle = SQLDataHelper.GetString(reader, "OptionTitle")
                                         };

                            evlist.Add(ev);
                        }


                    orditm.SelectedOptions = evlist;
                }
                da.cnClose();
            }
            return result;
        }

        private static void UpdateOrderedItem(int orderId, OrderItem item)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateOrderItem]", CommandType.StoredProcedure,
                                          new SqlParameter("@OrderItemID", item.OrderItemID),
                                          new SqlParameter("@OrderID", orderId),
                                          new SqlParameter("@Name", item.Name),
                                          new SqlParameter("@Price", item.Price),
                                          new SqlParameter("@Amount", item.Amount),
                                          new SqlParameter("@ProductID", item.ProductID),
                                          new SqlParameter("@ArtNo", item.ArtNo),
                                          new SqlParameter("@SupplyPrice", item.SupplyPrice),
                                          new SqlParameter("@Weight", item.Weight),
                                          new SqlParameter("@IsCouponApplied", item.IsCouponApplied),
                                          new SqlParameter("@Color", item.Color ?? (object)DBNull.Value),
                                          new SqlParameter("@Size", item.Size ?? (object)DBNull.Value),
                                          new SqlParameter("@DecrementedAmount", item.DecrementedAmount),
                                          new SqlParameter("@PhotoID", item.PhotoID != 0 && item.PhotoID != null ? item.PhotoID : (object)DBNull.Value));

        }

        private static void AddOrderedItem(int orderId, OrderItem item)
        {
            item.OrderItemID = SQLDataAccess.ExecuteScalar<int>("[Order].[sp_AddOrderItem]", CommandType.StoredProcedure,
                                                                new SqlParameter("@OrderID", orderId),
                                                                new SqlParameter("@Name", item.Name),
                                                                new SqlParameter("@Price", item.Price),
                                                                new SqlParameter("@Amount", item.Amount),
                                                                new SqlParameter("@ProductID", item.ProductID ?? (object)DBNull.Value),
                                                                new SqlParameter("@ArtNo", item.ArtNo),
                                                                new SqlParameter("@SupplyPrice", item.SupplyPrice),
                                                                new SqlParameter("@Weight", item.Weight),
                                                                new SqlParameter("@IsCouponApplied", item.IsCouponApplied),
                                                                new SqlParameter("@Color", item.Color ?? (object)DBNull.Value),
                                                                new SqlParameter("@Size", item.Size ?? (object)DBNull.Value),
                                                                new SqlParameter("@DecrementedAmount", item.DecrementedAmount),
                                                                new SqlParameter("@PhotoID", item.PhotoID != 0 && item.PhotoID != null ? item.PhotoID : (object)DBNull.Value)
                                                                );
            if (item.SelectedOptions != null)
            {
                foreach (EvaluatedCustomOptions evco in item.SelectedOptions)
                {
                    SQLDataAccess.ExecuteNonQuery("[Order].[sp_AddOrderCustomOptions]", CommandType.StoredProcedure,
                                                  new SqlParameter("@CustomOptionId", evco.CustomOptionId),
                                                  new SqlParameter("@OptionId", evco.OptionId),
                                                  new SqlParameter("@CustomOptionTitle", evco.CustomOptionTitle),
                                                  new SqlParameter("@OptionTitle", evco.OptionTitle),
                                                  new SqlParameter("@OptionPriceBC", evco.OptionPriceBc),
                                                  new SqlParameter("@OptionPriceType", evco.OptionPriceType),
                                                  new SqlParameter("@OrderItemID", item.OrderItemID));
                }
            }
        }

        private static void AddUpdateOrderedItem(int orderId, OrderItem item)
        {
            IList<OrderItem> items = GetOrderItems(orderId);
            if (items.Any(orderItem => orderItem.OrderItemID == item.OrderItemID))
            {
                UpdateOrderedItem(orderId, item);
            }
            else
                AddOrderedItem(orderId, item);
        }

        public static bool AddUpdateOrderItems(IList<OrderItem> items, int orderId)
        {
            foreach (OrderItem orderItem in items)
            {
                AddUpdateOrderedItem(orderId, orderItem);
            }
            RefreshTotal(orderId);
            return false;
        }

        public static bool AddUpdateOrderItems(IList<OrderItem> items, IList<OrderItem> olditems, int orderId)
        {
            List<OrderItem> itemsToDelete = new List<OrderItem>();
            foreach (OrderItem oldOrderItem in olditems)
            {
                bool isfound = items.Any(orderItem => orderItem.OrderItemID == oldOrderItem.OrderItemID);

                if (!isfound)
                {
                    oldOrderItem.Amount = 0;
                    AddUpdateOrderedItem(orderId, oldOrderItem);
                    itemsToDelete.Add(oldOrderItem);
                }
            }

            if (itemsToDelete.Count > 0)
            {
                IncrementProductsCountAccordingOrder(orderId);
                foreach (var item in itemsToDelete)
                {
                    DeleteOrderItem(item.OrderItemID, orderId);
                }
            }

            foreach (OrderItem orderItem in items)
            {
                AddUpdateOrderedItem(orderId, orderItem);
            }
            RefreshTotal(orderId);
            return false;
        }


        public static void DeleteOrderItem(int id, int orderId)
        {

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[OrderItems] WHERE [OrderItemID] = @OrderItemID",
                                          CommandType.Text,
                                          new SqlParameter("@OrderID", orderId),
                                          new SqlParameter("@OrderItemID", id));
        }

        public static void ClearOrderItems(int orderId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[OrderItems] WHERE [OrderID] = @OrderID",
                                          CommandType.Text, new SqlParameter("@OrderID", orderId));
        }

        #endregion

        public static Order GetOrderFromReader(IDataReader reader)
        {
            return new Order
            {
                OrderID = SQLDataHelper.GetInt(reader, "OrderID"),
                Number = SQLDataHelper.GetString(reader, "Number"),
                OrderStatusId = SQLDataHelper.GetInt(reader, "OrderStatusID"),
                StatusComment = SQLDataHelper.GetString(reader, "StatusComment"),
                AdditionalTechInfo = SQLDataHelper.GetString(reader, "AdditionalTechInfo"),
                AdminOrderComment = SQLDataHelper.GetString(reader, "AdminOrderComment"),
                Sum = SQLDataHelper.GetFloat(reader, "Sum"),
                ShippingCost = SQLDataHelper.GetFloat(reader, "ShippingCost"),
                PaymentCost = SQLDataHelper.GetFloat(reader, "PaymentCost"),
                OrderDiscount = SQLDataHelper.GetFloat(reader, "OrderDiscount"),
                TaxCost = SQLDataHelper.GetFloat(reader, "TaxCost"),
                OrderDate = SQLDataHelper.GetDateTime(reader, "OrderDate"),
                ShippingContactID = SQLDataHelper.GetInt(reader, "ShippingContactID"),
                BillingContactID = SQLDataHelper.GetInt(reader, "BillingContactID"),
                SupplyTotal = SQLDataHelper.GetFloat(reader, "SupplyTotal"),
                ShippingMethodId = SQLDataHelper.GetInt(reader, "ShippingMethodID"),
                PaymentMethodId = SQLDataHelper.GetInt(reader, "PaymentMethodId"),
                AffiliateID = SQLDataHelper.GetInt(reader, "AffiliateID"),
                CustomerComment = SQLDataHelper.GetString(reader, "CustomerComment"),
                Decremented = SQLDataHelper.GetBoolean(reader, "Decremented"),
                PaymentDate =
                    SQLDataHelper.GetDateTime(reader, "PaymentDate") == DateTime.MinValue
                        ? null
                        : (DateTime?)SQLDataHelper.GetDateTime(reader, "PaymentDate"),
                ArchivedPaymentName = SQLDataHelper.GetString(reader, "PaymentMethodName"),
                ArchivedShippingName = SQLDataHelper.GetString(reader, "ShippingMethodName"),

                GroupName = SQLDataHelper.GetString(reader, "GroupName"),
                GroupDiscount = SQLDataHelper.GetFloat(reader, "GroupDiscount"),
                Certificate = SQLDataHelper.GetString(reader, "CertificateCode").IsNotEmpty()
                                    ? new OrderCertificate
                                        {
                                            Code = SQLDataHelper.GetString(reader, "CertificateCode"),
                                            Price = SQLDataHelper.GetFloat(reader, "CertificatePrice")
                                        }
                                    : null,
                Coupon = SQLDataHelper.GetString(reader, "CouponCode").IsNotEmpty()
                                ? new OrderCoupon
                                    {
                                        Type = (CouponType)SQLDataHelper.GetInt(reader, "CouponType"),
                                        Code = SQLDataHelper.GetString(reader, "CouponCode"),
                                        Value = SQLDataHelper.GetFloat(reader, "CouponValue")
                                    }
                                : null,
                BonusCost = SQLDataHelper.GetFloat(reader, "BonusCost"),
                UseIn1C = SQLDataHelper.GetBoolean(reader, "UseIn1C"),
                ModifiedDate = SQLDataHelper.GetDateTime(reader, "ModifiedDate")
            };
        }

        public static OrderAutocomplete GetOrderForAutocompleteFromReader(IDataReader reader)
        {
            return new OrderAutocomplete
            {
                OrderID = SQLDataHelper.GetInt(reader, "OrderID"),
                Number = SQLDataHelper.GetString(reader, "Number"),
                FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                LastName = SQLDataHelper.GetString(reader, "LastName"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                MobilePhone = SQLDataHelper.GetString(reader, "MobilePhone")
            };
        }


        public static List<Order> GetAllOrders()
        {
            return SQLDataAccess.ExecuteReadList<Order>("SELECT * FROM [Order].[Order]", CommandType.Text, GetOrderFromReader);
        }

        public static List<OrderAutocomplete> GetOrdersForAutocomplete(string query)
        {
            return SQLDataAccess.ExecuteReadList<OrderAutocomplete>("SELECT [Order].[OrderID], Number, FirstName, LastName, Email, MobilePhone FROM [Order].[Order] INNER JOIN [Order].[OrderCustomer] ON [Order].[OrderID] = [OrderCustomer].[OrderID] WHERE [Number] LIKE '%' + @q + '%'" +
                                                                                                                                                                           "OR [Email] LIKE '%' + @q + '%'" +
                                                                                                                                                                           "OR [FirstName] LIKE '%' + @q + '%'" +
                                                                                                                                                                           "OR [LastName] LIKE '%' + @q + '%'" +
                                                                                                                                                                           "OR [MobilePhone] LIKE '%' + @q + '%'",
                                                                                                                                                                            CommandType.Text, GetOrderForAutocompleteFromReader, new SqlParameter("@q", query));
        }

        public static List<Order> GetLastOrders(int number)
        {
            return SQLDataAccess.ExecuteReadList<Order>("SELECT TOP(@count) * FROM [Order].[Order] order by orderdate desc", CommandType.Text, GetOrderFromReader, new SqlParameter("@count", number));
        }

        public static List<string> GetShippingMethods()
        {
            List<string> result = SQLDataAccess.ExecuteReadList("SELECT Name FROM [Order].[ShippingMethod]", CommandType.Text, reader => SQLDataHelper.GetString(reader, "Name").Trim());
            return result;
        }

        public static List<string> GetShippingMethodNamesFromOrder()
        {
            List<string> result = SQLDataAccess.ExecuteReadList("SELECT distinct ShippingMethodName FROM [Order].[Order]", CommandType.Text, reader => SQLDataHelper.GetString(reader, "ShippingMethodName").Trim());
            return result;
        }

        public static void DeleteOrder(int id)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_DeleteOrder]", CommandType.StoredProcedure, new SqlParameter { ParameterName = "@OrderID", Value = id });
            Modules.ModulesRenderer.OrderDeleted(id);

            if (Settings1C.Enabled)
                SQLDataAccess.ExecuteNonQuery(
                    "Insert Into [Order].[DeletedOrders] ([OrderId],[DateTime]) Values (@OrderId, Getdate())", CommandType.Text,
                    new SqlParameter("@OrderId", id));
        }

        public static int AddOrder(Order order)
        {
            AddOrderMain(order);
            if (order.OrderID != 0)
            {
                AddOrderCustomer(order.OrderID, order.OrderCustomer);

                if (order.BillingContact != null && order.BillingContact != order.ShippingContact)
                {
                    AddOrderContacts(order.OrderID, order.ShippingContact, order.BillingContact);
                }
                else
                {
                    AddOrderContacts(order.OrderID, order.ShippingContact);
                    order.BillingContact = order.ShippingContact;
                }

                AddOrderCurrency(order.OrderID, order.OrderCurrency);

                if (order.PaymentDetails != null)
                    AddPaymentDetails(order.OrderID, order.PaymentDetails);

                if (order.OrderPickPoint != null)
                    AddUpdateOrderPickPoint(order.OrderID, order.OrderPickPoint);

                if (order.OrderItems != null)
                    foreach (var row in order.OrderItems)
                    {
                        AddUpdateOrderedItem(order.OrderID, row);
                    }

                if (order.OrderCertificates != null)
                    foreach (var certificate in order.OrderCertificates)
                    {
                        certificate.OrderId = order.OrderID;
                        GiftCertificateService.AddCertificate(certificate);
                    }

                RefreshTotal(order.OrderID);
            }
            return order.OrderID;
        }

        private static void AddOrderCurrency(int orderId, OrderCurrency orderCurrency)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO [Order].[OrderCurrency] (OrderID, CurrencyCode, CurrencyNumCode, CurrencyValue, CurrencySymbol, IsCodeBefore ) VALUES (@OrderID, @CurrencyCode, @CurrencyNumCode, @CurrencyValue, @CurrencySymbol, @IsCodeBefore)",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@CurrencyCode", orderCurrency.CurrencyCode),
                new SqlParameter("@CurrencyNumCode", orderCurrency.CurrencyNumCode),
                new SqlParameter("@CurrencyValue", orderCurrency.CurrencyValue),
                new SqlParameter("@CurrencySymbol", orderCurrency.CurrencySymbol),
                new SqlParameter("@IsCodeBefore", orderCurrency.IsCodeBefore));
        }

        public static void AddUpdateOrderPickPoint(int orderId, OrderPickPoint pickPoint)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"if (select count(orderid) from [Order].[OrderPickPoint] where orderid=@orderid) = 0
                begin 
                    INSERT INTO [Order].[OrderPickPoint] (OrderID, PickPointId, PickPointAddress, AdditionalData) VALUES (@OrderID, @PickPointId, @PickPointAddress, @AdditionalData) 
                end
                else
                begin
                    Update [Order].[OrderPickPoint] set PickPointId=@PickPointId, PickPointAddress=@PickPointAddress, AdditionalData=@AdditionalData Where OrderID=@OrderID
                end",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@PickPointId", pickPoint.PickPointId),
                new SqlParameter("@PickPointAddress", pickPoint.PickPointAddress ?? string.Empty),
                new SqlParameter("@AdditionalData", pickPoint.AdditionalData ?? string.Empty));
        }

        public static void DeleteOrderPickPoint(int orderID)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from [Order].[OrderPickPoint] where OrderID= @OrderID",
               CommandType.Text,
               new SqlParameter("@OrderID", orderID));
        }


        /// <summary>
        /// Adds order billing and shipping contacts
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shippingContact"></param>
        /// <param name="billingContact"></param>
        private static bool AddOrderContacts(int orderId, OrderContact shippingContact, OrderContact billingContact)
        {
            bool res = true;
            res &= (AddOrderContact(orderId, shippingContact, OrderContactType.ShippingContact) != 0);
            res &= (AddOrderContact(orderId, billingContact, OrderContactType.BillingContact) != 0);
            return res;
        }

        /// <summary>
        /// Adds order contacts with equals shipping and billing contacts
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="onlyContact"></param>
        private static bool AddOrderContacts(int orderId, OrderContact onlyContact)
        {
            int contactID = AddOrderContact(orderId, onlyContact, OrderContactType.ShippingContact);
            UpdateOrderContactId(orderId, contactID, OrderContactType.BillingContact);
            return contactID != 0;
        }

        private static int AddOrderContact(int orderId, OrderContact contact, OrderContactType type)
        {
            contact.OrderContactId = SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                "INSERT INTO [Order].[OrderContact] ([Name],[Country],[Zone],[City],[Zip],[Address],[CustomField1],[CustomField2],[CustomField3]) " +
                " VALUES (@Name,@Country,@Zone,@City,@Zip,@Address,@CustomField1,@CustomField2,@CustomField3); " +
                "SELECT scope_identity();",
                CommandType.Text,
                new SqlParameter("@Name", contact.Name ?? string.Empty),
                new SqlParameter("@Country", contact.Country ?? string.Empty),
                new SqlParameter("@Zone", contact.Zone ?? string.Empty),
                new SqlParameter("@City", contact.City ?? string.Empty),
                new SqlParameter("@Zip", contact.Zip ?? string.Empty),
                new SqlParameter("@Address", contact.Address ?? string.Empty),
                new SqlParameter("@CustomField1", contact.CustomField1 ?? string.Empty),
                new SqlParameter("@CustomField2", contact.CustomField2 ?? string.Empty),
                new SqlParameter("@CustomField3", contact.CustomField3 ?? string.Empty)
                ));

            UpdateOrderContactId(orderId, contact.OrderContactId, type);
            return contact.OrderContactId;
        }

        public static void UpdateOrderContactId(int orderId, int contactId, OrderContactType type)
        {
            SQLDataAccess.ExecuteNonQuery(
                string.Format("UPDATE [Order].[Order] SET [{0}] = @ContactID WHERE [OrderID] = @OrderID",
                              type == OrderContactType.ShippingContact ? "ShippingContactID" : "BillingContactID"),
                CommandType.Text, new SqlParameter("@OrderID", orderId), new SqlParameter("@ContactID", contactId));
        }

        private static void AddOrderCustomer(int orderId, OrderCustomer orderCustomer)
        {
            SQLDataAccess.ExecuteNonQuery(
                "INSERT INTO [Order].[OrderCustomer] ([OrderId],[CustomerID],[CustomerIP],[FirstName],[LastName],[Email],[MobilePhone], [Patronymic]) " +
                " VALUES (@OrderId,@CustomerID,@CustomerIP,@FirstName,@LastName,@Email,@MobilePhone, @Patronymic)",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@CustomerID", orderCustomer.CustomerID),
                new SqlParameter("@CustomerIP", orderCustomer.CustomerIP),
                new SqlParameter("@FirstName", orderCustomer.FirstName ?? string.Empty),
                new SqlParameter("@LastName", orderCustomer.LastName ?? string.Empty),
                new SqlParameter("@Patronymic", orderCustomer.Patronymic ?? string.Empty),
                new SqlParameter("@Email", orderCustomer.Email ?? string.Empty),
                new SqlParameter("@MobilePhone", orderCustomer.MobilePhone ?? string.Empty));
        }

        private static void AddOrderMain(Order ord)
        {
            ord.OrderID = SQLDataAccess.ExecuteScalar<int>(
                        "INSERT INTO [Order].[Order] " +
                            "([Number], [ShippingMethodID], [PaymentMethodID], [AffiliateID], " +
                             "[OrderDate], [PaymentDate], [CustomerComment], [StatusComment], " +
                             "[AdditionalTechInfo],[AdminOrderComment], [ShippingCost],[PaymentCost], [OrderStatusID], " +
                             "[ShippingMethodName],[PaymentMethodName], [GroupName], [GroupDiscount], [OrderDiscount], " +
                             "[CertificateCode], [CertificatePrice], [CouponCode], [CouponType], [CouponValue], [BonusCost], [ModifiedDate],[UseIn1C]) " +
                        "VALUES " +
                            "(@Number, @ShippingMethodID, @PaymentMethodID, @AffiliateID, " +
                             "@OrderDate, null, @CustomerComment, @StatusComment, " +
                             "@AdditionalTechInfo, @AdminOrderComment, @ShippingCost,@PaymentCost, @OrderStatusID, " +
                             "@ShippingMethodName, @PaymentMethodName, @GroupName, @GroupDiscount,@OrderDiscount, " +
                             "@CertificateCode, @CertificatePrice, @CouponCode, @CouponType, @CouponValue, @BonusCost, Getdate(),@UseIn1C); " +
                        "SELECT scope_identity();",
                CommandType.Text,

                new SqlParameter("@Number", ord.Number),
                new SqlParameter("@ShippingMethodID", ord.ShippingMethodId != 0 ? ord.ShippingMethodId : (object)DBNull.Value),
                new SqlParameter("@PaymentMethodID", ord.PaymentMethodId != 0 ? ord.PaymentMethodId : (object)DBNull.Value),
                new SqlParameter("@ShippingMethodName", ord.ArchivedShippingName ?? string.Empty),
                new SqlParameter("@PaymentMethodName", ord.PaymentMethodName ?? string.Empty),
                new SqlParameter("@OrderStatusID", ord.OrderStatusId),
                new SqlParameter("@AffiliateID", ord.AffiliateID),
                new SqlParameter("@ShippingCost", ord.ShippingCost),
                new SqlParameter("@PaymentCost", ord.PaymentCost),
                new SqlParameter("@OrderDate", ord.OrderDate),
                new SqlParameter("@CustomerComment", ord.CustomerComment ?? string.Empty),
                new SqlParameter("@StatusComment", ord.StatusComment ?? string.Empty),
                new SqlParameter("@AdditionalTechInfo", ord.AdditionalTechInfo ?? string.Empty),
                new SqlParameter("@AdminOrderComment", ord.AdminOrderComment ?? string.Empty),
                new SqlParameter("@GroupName", ord.GroupName ?? CustomerGroupService.GetCustomerGroup(1).GroupName),
                new SqlParameter("@GroupDiscount", ord.GroupDiscount),
                new SqlParameter("@OrderDiscount", ord.OrderDiscount),
                new SqlParameter("@CertificatePrice", ord.Certificate != null ? (object)ord.Certificate.Price : DBNull.Value),
                new SqlParameter("@CertificateCode", ord.Certificate != null ? (object)ord.Certificate.Code : DBNull.Value),
                new SqlParameter("@CouponCode", ord.Coupon != null ? (object)ord.Coupon.Code : DBNull.Value),
                new SqlParameter("@CouponType", ord.Coupon != null ? (object)ord.Coupon.Type : DBNull.Value),
                new SqlParameter("@CouponValue", ord.Coupon != null ? (object)ord.Coupon.Value : DBNull.Value),
                new SqlParameter("@BonusCost", ord.BonusCost),
                new SqlParameter("@UseIn1C", 1)
                );
        }

        public static void AddPaymentDetails(int orderId, PaymentDetails details)
        {
            if (details != null)
                SQLDataAccess.ExecuteNonQuery("[Order].[sp_AddPaymentDetails]", CommandType.StoredProcedure,
                                                new SqlParameter("@OrderID", orderId),
                                                new SqlParameter("@CompanyName", details.CompanyName ?? string.Empty),
                                                new SqlParameter("@INN", details.INN ?? string.Empty),
                                                new SqlParameter("@Phone", details.Phone ?? string.Empty));
        }

        public static void UpdatePaymentDetails(int orderId, PaymentDetails details)
        {
            if (details != null)
                SQLDataAccess.ExecuteNonQuery("if(select count (orderid) from [Order].[PaymentDetails] where OrderID=@OrderID) > 0 " +
                                              " Update [Order].[PaymentDetails] Set CompanyName=@CompanyName, INN=@INN, phone=@phone Where OrderID=@OrderID" +
                                              " else " +
                                              " insert into [Order].[PaymentDetails] (OrderID, CompanyName,  INN, phone) values(@OrderID, @CompanyName, @INN, @phone) ", CommandType.Text,
                                                new SqlParameter("@OrderID", orderId),
                                                new SqlParameter("@CompanyName", details.CompanyName ?? string.Empty),
                                                new SqlParameter("@INN", details.INN ?? string.Empty),
                                                new SqlParameter("@Phone", details.Phone ?? string.Empty));
        }

        public static void UpdateNumber(int id, string number)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateOrderNumber]", CommandType.StoredProcedure, new SqlParameter("@OrderID", id), new SqlParameter("@Number", number));
        }

        public static void UpdateAdminOrderComment(int id, string adminOrderComment)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateOrderAdminOrderComment]", CommandType.StoredProcedure, new SqlParameter("@OrderID", id),
                                        new SqlParameter("@AdminOrderComment", adminOrderComment ?? string.Empty));
        }

        public static void UpdateStatusComment(int id, string statusComment)
        {
            SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdateOrderStatusComment]", CommandType.StoredProcedure,
                                            new SqlParameter("@OrderID", id), new SqlParameter("@StatusComment", statusComment ?? string.Empty));
        }

        public static StatusInfo GetStatusInfo(string orderNum)
        {
            return SQLDataAccess.ExecuteReadOne("[Order].[sp_GetOrderStatusInfo]", CommandType.StoredProcedure,
                reader => new StatusInfo()
                {
                    StatusComment = SQLDataHelper.GetString(reader, "StatusComment"),
                    StatusName = SQLDataHelper.GetString(reader, "StatusName")
                },
                new SqlParameter("@OrderNum", orderNum));
        }

        public static void PayOrder(int orderId, bool pay, bool updateModules = true)
        {
            var order = GetOrder(orderId);
            if (order == null)
                throw new Exception("Can't pay empty order");

            if ((pay && order.Payed) || (!pay && !order.Payed))
                return;

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[Order] SET [PaymentDate] = @PaymentDate, ModifiedDate = Getdate() WHERE [OrderID] = @OrderID", CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@PaymentDate", pay ? DateTime.Now : (object)DBNull.Value));

            if (pay)
            {
                Mails.SendMail.SendMailNow(SettingsMail.EmailForOrders,
                    string.Format(Resource.Admin_Order_PaymentEmailSubject, order.OrderID, CatalogService.GetStringPrice(order.Sum)),
                    string.Format(Resource.Admin_Order_PaymentEmailBody, order.OrderID, CatalogService.GetStringPrice(order.Sum), order.ArchivedPaymentName, SettingsMain.SiteUrl),
                    true);
            }
            else
            {
                Mails.SendMail.SendMailNow(SettingsMail.EmailForOrders,
                    string.Format(Resource.Admin_Order_PaymentCancelEmailSubject, order.OrderID, CatalogService.GetStringPrice(order.Sum)),
                    string.Format(Resource.Admin_Order_PaymentCancelEmailBody, order.OrderID, CatalogService.GetStringPrice(order.Sum, order.OrderCurrency), SettingsMain.SiteUrl),
                    true);
            }

            foreach (var certificate in GiftCertificateService.GetOrderCertificates(orderId))
            {
                GiftCertificateService.SendCertificateMails(certificate);
            }

            if (pay && BonusSystem.BonusSystem.IsActive)
            {
                BonusSystemService.Confirm(GetOrderNumberById(orderId), orderId);
            }

            if (updateModules)
            {
                ModulesRenderer.OrderUpdated(orderId);
            }
        }

        public static PaymentDetails GetPaymentDetails(int orderId)
        {
            return SQLDataAccess.ExecuteReadOne("[Order].[sp_GetPaymentDetails]", CommandType.StoredProcedure,
                reader => new PaymentDetails
                {
                    CompanyName = SQLDataHelper.GetString(reader, "CompanyName"),
                    INN = SQLDataHelper.GetString(reader, "INN"),
                    Phone = SQLDataHelper.GetString(reader, "Phone")
                },
                new SqlParameter("@OrderID", orderId));
        }

        public static Order GetOrder(int orderId)
        {
            return SQLDataAccess.ExecuteReadOne<Order>(
                "SELECT * FROM [Order].[Order] WHERE [OrderID] = @OrderID",
                CommandType.Text,
                GetOrderFromReader,
                new SqlParameter("@OrderID", orderId));
        }

        public static int GetOrderStatusId(int orderId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT [OrderStatusID] FROM [Order].[Order] WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId));
        }

        public static int GetOrderIdByNumber(string number)
        {
            int retId = 0;
            if (!string.IsNullOrEmpty(number))
            {
                retId = SQLDataAccess.ExecuteScalar<int>("[Order].[sp_GetOrderIdByNumber]", CommandType.StoredProcedure, new SqlParameter("@Number", number));
            }
            return retId;
        }

        public static string GetOrderNumberById(int orderId)
        {
            return SQLDataAccess.ExecuteScalar<string>(
             "SELECT [Number] FROM [Order].[Order] WHERE [OrderId] = @OrderId",
             CommandType.Text,
             new SqlParameter("@OrderId", orderId));
        }

        public static int GetCountOrder(string number)
        {
            int retCount = 0;

            if (!string.IsNullOrEmpty(number))
            {
                retCount = SQLDataAccess.ExecuteScalar<int>("[Order].[sp_GetCountOrderByNumber]", CommandType.StoredProcedure, new SqlParameter("@Number", number));
            }

            return retCount;
        }

        public static string GenerateNumber(int id)
        {
            string retNum;
            var random = new Random();

            do
            {
                retNum = string.Format("{0}-{1}-{2}", CreateKey(random, 3), CreateKey(random, 4), GetStringOrderId(id.ToString()));
            } while (GetCountOrder(retNum) != 0);

            return retNum;
        }

        private static string GetStringOrderId(string id)
        {
            return id.Length < LenghtStringOrderId
                       ? string.Format("{0}{1}", Space(LenghtStringOrderId - id.Length).Replace(" ", "0"), id)
                       : id;
        }

        private static string Space(int count)
        {
            var res = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                res.Append(' ');
            }
            return res.ToString();
        }

        private static string CreateKey(Random random, int numBytes)
        {
            string retStr = string.Empty;
            for (int i = 0; i < numBytes; i++)
            {
                retStr += CharsNs[(int)((CharsNs.Length) * random.NextDouble())];
            }
            return retStr;
        }

        public static List<Order> GetOrdersByStatusId(int statusId)
        {
            return SQLDataAccess.ExecuteReadList<Order>(
                "SELECT * FROM [Order].[Order] WHERE [OrderStatusID] = @OrderStatusID",
                CommandType.Text,
                GetOrderFromReader,
                new SqlParameter("@OrderStatusID", statusId));
        }

        public static List<Order> GetOrdersFor1C(DateTime from, DateTime to, bool onlyUseIn1C)
        {
            var query = "SELECT * FROM [Order].[Order] WHERE ";
            var queryParams = new List<SqlParameter>();

            if (onlyUseIn1C)
            {
                query += "[UseIn1C] = 1 and ";
            }

            query += "([OrderDate] >= @From and [OrderDate] <= @To or [ModifiedDate] >= @From and [ModifiedDate] <= @To)";
            queryParams.Add(new SqlParameter("@From", from));
            queryParams.Add(new SqlParameter("@To", to));

            return SQLDataAccess.ExecuteReadList(query, CommandType.Text, GetOrderFromReader, queryParams.ToArray());
        }

        public static List<int> GetDeletedOrders(DateTime? from, DateTime? to)
        {
            var query = "SELECT OrderId FROM [Order].[DeletedOrders]";
            var queryParams = new List<SqlParameter>();

            if (from != null && to != null)
            {
                query += " Where [DateTime] >= @From and [DateTime] <= @To";
                queryParams.Add(new SqlParameter("@From", from));
                queryParams.Add(new SqlParameter("@To", to));
            }

            return SQLDataAccess.ExecuteReadList(query, CommandType.Text, reader => SQLDataHelper.GetInt(reader, "OrderId"), queryParams.ToArray());
        }

        public static void SerializeToXml(List<Order> orders, TextWriter baseWriter, bool isAdvanced = false)
        {
            using (var writer = XmlWriter.Create(baseWriter, new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Orders");
                foreach (Order order in orders)
                {
                    SerializeToXml(order, writer, isAdvanced);
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
        }

        public static void SerializeToXml(Order order, TextWriter baseWriter)
        {
            SerializeToXml(new List<Order> { order }, baseWriter);
        }

        private static void SerializeToXml(Order order, XmlWriter writer, bool isAdvanced = false)
        {
            OrderCustomer customer = order.OrderCustomer;

            var totalDiscount = order.TotalDiscount;

            writer.WriteStartElement("Order");
            writer.WriteAttributeString("OrderID", order.OrderID.ToString());
            writer.WriteAttributeString("CustomerEmail", customer != null && customer.Email.IsNotEmpty() ? customer.Email : string.Empty);
            writer.WriteAttributeString("OfferType", "Obsolete");
            writer.WriteAttributeString("Date", order.OrderDate.ToString());

            writer.WriteAttributeString("Comments", order.CustomerComment);
            writer.WriteAttributeString("DiscountPercent", order.OrderDiscount.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("DiscountValue", totalDiscount.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("ShippingCost", order.ShippingCost.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("CustomerIP", customer != null ? order.OrderCustomer.CustomerIP : string.Empty);
            writer.WriteAttributeString("ShippingMethod", order.ArchivedShippingName);
            writer.WriteAttributeString("BillingMethod", order.PaymentMethodName);
            writer.WriteAttributeString("Canceled", order.OrderStatus.IsCanceled.ToString());

            if (isAdvanced)
            {
                //writer.WriteAttributeString("OrderStatus", Settings1C.GetStatusName(order.OrderStatus.StatusID));
                writer.WriteAttributeString("TaxCost", (order.Taxes != null ? order.Taxes.Sum(tax => tax.TaxSum) : 0).ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("TaxInPrice", (order.Taxes != null && order.Taxes.Count > 0 ? Convert.ToInt32(order.Taxes.FirstOrDefault().TaxShowInPrice) : 0).ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("ModifiedDate", order.ModifiedDate.ToString());
            }

            var orderItemsCount = order.OrderItems.Count;
            var orderItemsSum = order.OrderItems.Sum(x => x.Amount * x.Price);
            var bonusOnProduct = (int)(order.BonusCost / orderItemsCount);
            var paymentCostOnProduct = (int)(order.PaymentCost / orderItemsCount);
            var discountSum = 0f;
            var bonusSum = 0;
            var paymentSum = 0;
            var count = 0;


            if (orderItemsCount != 0)
            {
                writer.WriteStartElement("Products");
                foreach (var item in order.OrderItems)
                {
                    writer.WriteStartElement("Product");
                    writer.WriteAttributeString("ID", item.ArtNo ?? item.ProductID.ToString());
                    writer.WriteAttributeString("Цвет", item.Color);
                    writer.WriteAttributeString("Размер", item.Size);
                    writer.WriteAttributeString("Amount", item.Amount.ToString());
                    writer.WriteAttributeString("Price", item.Price.ToString("F2", CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("Currency", string.Empty);

                    if (isAdvanced)
                    {
                        var discountOnProduct = (float)Math.Round(item.Amount * item.Price / orderItemsSum * totalDiscount, 2);

                        writer.WriteAttributeString("Discount",
                            (orderItemsCount != count + 1 ? discountOnProduct : totalDiscount - discountSum).ToString("F2", CultureInfo.InvariantCulture));

                        writer.WriteAttributeString("Bonus",
                            (orderItemsCount != count + 1 ? bonusOnProduct : order.BonusCost - bonusSum).ToString(CultureInfo.InvariantCulture));

                        writer.WriteAttributeString("Payment",
                            (orderItemsCount != count + 1 ? paymentCostOnProduct : order.PaymentCost - paymentSum).ToString(CultureInfo.InvariantCulture));

                        count++;
                        discountSum += discountOnProduct;
                        bonusSum += bonusOnProduct;
                        paymentSum += paymentCostOnProduct;
                    }
                    if (item.ProductID != null)
                    {
                        var product = ProductService.GetProduct((int)item.ProductID);
                        writer.WriteAttributeString("Unit", product == null ? "" : product.Unit);
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            if (customer != null)
            {
                writer.WriteStartElement("Customer");

                writer.WriteAttributeString("Surname", customer.LastName ?? string.Empty);
                writer.WriteAttributeString("Name", customer.FirstName ?? string.Empty);
                writer.WriteAttributeString("Email", customer.Email ?? string.Empty);
                writer.WriteAttributeString("CustomerType", string.Empty);
                var contact = order.ShippingContact ?? new OrderContact();
                writer.WriteAttributeString("City", string.Empty);
                writer.WriteAttributeString("Address", string.Empty);
                writer.WriteAttributeString("Zip", string.Empty);

                //for 1c, because this filds 1c are waiting
                writer.WriteAttributeString("Phone", customer.MobilePhone);
                writer.WriteAttributeString("Fax", string.Empty);

                writer.WriteAttributeString("ShippingName", order.ShippingContact != null ? order.ShippingContact.Name : "");
                writer.WriteAttributeString("BillingName", order.BillingContact != null ? order.BillingContact.Name : "");
                writer.WriteAttributeString("ContactName", contact.Name);

                //for 1c, because this filds 1c are waiting
                writer.WriteAttributeString("ShippingEmail", string.Empty);
                writer.WriteAttributeString("BillingEmail", string.Empty);

                //writer.WriteAttributeString("BillingEmail", order.BillingContact.Name);

                writer.WriteAttributeString("ShippingCountry", order.ShippingContact != null ? order.ShippingContact.Country : "");
                writer.WriteAttributeString("BillingCountry", order.BillingContact != null ? order.BillingContact.Country : "");
                writer.WriteAttributeString("ShippingZone", order.ShippingContact != null ? order.ShippingContact.Zone : "");
                writer.WriteAttributeString("BillingZone", order.BillingContact != null ? order.BillingContact.Zone : "");

                writer.WriteAttributeString("ShippingCity", order.ShippingContact != null ? order.ShippingContact.City : "");
                writer.WriteAttributeString("BillingCity", order.BillingContact != null ? order.BillingContact.City : "");
                writer.WriteAttributeString("ShippingAddress", order.ShippingContact != null ? order.ShippingContact.Address : "");
                writer.WriteAttributeString("BillingAddress", order.BillingContact != null ? order.BillingContact.Address : "");
                writer.WriteAttributeString("ShippingZip", order.ShippingContact != null ? order.ShippingContact.Zip : "");
                writer.WriteAttributeString("BillingZip", order.BillingContact != null ? order.BillingContact.Zip : "");
                //for 1c, because this filds 1c are waiting
                writer.WriteAttributeString("ShippingPhone", string.Empty);
                writer.WriteAttributeString("BillingPhone", string.Empty);
                writer.WriteAttributeString("ShippingFax", string.Empty);
                writer.WriteAttributeString("BillingFax", string.Empty);

                writer.WriteEndElement();

            }

            writer.WriteEndElement();
        }

        public static IList<OrderPriceDiscount> GetOrderPricesDiscounts()
        {
            var result = new List<OrderPriceDiscount>();

            if (SettingsOrderConfirmation.EnableDiscountModule)
            {
                var str = CacheNames.GetOrderPriceDiscountCacheObjectName();
                if (CacheManager.Contains(str))
                {
                    result = CacheManager.Get<List<OrderPriceDiscount>>(str);
                    if (result != null)
                        return result;
                }

                result = new List<OrderPriceDiscount>();

                using (var da = new SQLDataAccess())
                {
                    da.cmd.CommandText = "SELECT  PriceRange, PercentDiscount FROM [Order].OrderPriceDiscount ORDER BY PriceRange";
                    da.cmd.CommandType = CommandType.Text;

                    da.cmd.Parameters.Clear();

                    da.cnOpen();

                    using (SqlDataReader reader = da.cmd.ExecuteReader())

                        while (reader.Read())
                        {
                            var opd = new OrderPriceDiscount
                                          {
                                              PercentDiscount = SQLDataHelper.GetDouble(reader, "PercentDiscount"),
                                              PriceRange = SQLDataHelper.GetFloat(reader, "PriceRange")
                                          };
                            result.Add(opd);
                        }

                    da.cnClose();
                    CacheManager.Insert(str, result, 10);
                }
            }
            return result;
        }

        public static float GetDiscount(float price)
        {
            return GetDiscount(GetOrderPricesDiscounts(), price);
        }

        public static float GetDiscount(IList<OrderPriceDiscount> table, float price)
        {
            return table == null
                       ? 0
                       : (float)
                         table.Where(dr => dr.PriceRange < price).OrderBy(dr => dr.PriceRange).DefaultIfEmpty(
                             new OrderPriceDiscount { PercentDiscount = 0 }).Last().PercentDiscount;
        }

        public static bool? IsDecremented(int orderId)
        {
            return SQLDataAccess.ExecuteScalar<bool>("[Order].[sp_IsDecremented]", CommandType.StoredProcedure, new SqlParameter("@OrderID", orderId));
        }

        public static bool RefreshTotal(int orderId)
        {
            Order order = GetOrder(orderId);

            float totalPrice = 0;
            float totalProductsPrice = 0;
            float totalDiscount = 0;
            float supplyTotal = 0;
            float bonusPrice = order.BonusCost;

            if (order.OrderItems.Any())
            {
                totalProductsPrice = order.OrderItems.Sum(item => item.Price * item.Amount);
                supplyTotal = order.OrderItems.Sum(item => item.SupplyPrice * item.Amount);
            }
            else if (order.OrderCertificates.Any())
            {
                totalProductsPrice = order.OrderCertificates.Sum(item => item.Sum);
            }

            totalDiscount += order.OrderDiscount > 0 ? order.OrderDiscount * totalProductsPrice / 100 : 0;

            if (order.Certificate != null)
            {
                totalDiscount += order.Certificate.Price != 0 ? order.Certificate.Price : 0;
            }

            if (order.Coupon != null)
            {
                switch (order.Coupon.Type)
                {
                    case CouponType.Fixed:
                        var productsPrice =
                            order.OrderItems.Where(item => item.IsCouponApplied).Sum(p => p.Price * p.Amount);
                        totalDiscount += productsPrice >= order.Coupon.Value ? order.Coupon.Value : productsPrice;
                        break;
                    case CouponType.Percent:
                        totalDiscount +=
                            order.OrderItems.Where(item => item.IsCouponApplied)
                                 .Sum(item => order.Coupon.Value * item.Price / 100 * item.Amount);
                        break;
                }
            }

            if (order.BonusCost > 0)
            {
                bonusPrice = BonusSystemService.GetBonusCost(totalProductsPrice - totalDiscount + order.ShippingCost, totalProductsPrice - totalDiscount, order.BonusCost);
                totalDiscount += bonusPrice;
            }


            Dictionary<TaxElement, float> taxes;
            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                taxes = TaxServices.CalculateCertificateTaxes(order.OrderCertificates.Sum(cert => cert.Sum));
            }
            else
            {
                taxes = TaxServices.CalculateTaxes(totalProductsPrice - totalDiscount + order.ShippingCost);
            }

            if (taxes.Count > 0)
            {
                TaxServices.ClearOrderTaxes(order.OrderID);
                TaxServices.SetOrderTaxes(order.OrderID,
                                          taxes.Select(
                                              tax =>
                                              new OrderTax()
                                                  {
                                                      TaxID = tax.Key.TaxId,
                                                      TaxName = tax.Key.Name,
                                                      TaxShowInPrice = tax.Key.ShowInPrice,
                                                      TaxSum = tax.Value,
                                                      TaxRate = tax.Key.Rate
                                                  }).ToList());
            }

            float taxTotal = taxes.Sum(tv => tv.Value);
            float taxNotInPrice = taxes.Where(tax => !tax.Key.ShowInPrice).Sum(tv => tv.Value);

            totalPrice = totalProductsPrice - totalDiscount + order.ShippingCost + taxNotInPrice + order.PaymentCost;
            if (totalPrice < 0) totalPrice = 0;


            order.DiscountCost = totalDiscount;

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[Order] SET [Sum] = @Sum, [TaxCost] = @TaxCost, [SupplyTotal] = @SupplyTotal, [BonusCost] = @BonusCost, [DiscountCost] = @DiscountCost WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@Sum", totalPrice),
                new SqlParameter("@TaxCost", taxTotal),
                new SqlParameter("@SupplyTotal", supplyTotal),
                new SqlParameter("@BonusCost", bonusPrice),
                new SqlParameter("@DiscountCost", totalDiscount));

            return true;
        }

        public static int GetLastOrderId()
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT TOP 1 OrderID FROM [Order].[Order] order by OrderDate desc", CommandType.Text);
        }

        public static bool UpdateOrderContacts(int orderId, OrderContact shippingContact, OrderContact billingContact)
        {
            bool res = true;
            ClearOrderContacts(orderId);
            if (shippingContact == billingContact)
                res &= AddOrderContacts(orderId, shippingContact);
            else
                res &= AddOrderContacts(orderId, shippingContact, billingContact);
            if (res)
                return RefreshTotal(orderId);

            return false;
        }

        private static void ClearOrderContacts(int orderId)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"with temp (ShippingContactID, BillingContactID) as 
                                                (SELECT ShippingContactID, BillingContactID FROM [Order].[Order] WHERE [OrderID] = @OrderID)
                                                DELETE FROM [Order].[OrderContact] 
                                                WHERE [OrderContactID] = (select top(1) ShippingContactID from temp)
                                                OR  [OrderContactID] = (select top(1) BillingContactID from temp); 
                                                UPDATE [Order].[Order] SET ShippingContactID = 0, BillingContactID = 0 WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId));
        }

        public static void UpdateOrderMain(Order order)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Order].[Order]
                       SET [Number] = @Number
                          ,[ShippingMethodID] = @ShippingMethodID
                          ,[PaymentMethodID] = @PaymentMethodID
                          ,[AffiliateID] = @AffiliateID
                          ,[OrderDiscount] = @OrderDiscount
                          ,[CustomerComment] = @CustomerComment
                          ,[StatusComment] = @StatusComment
                          ,[AdditionalTechInfo] = @AdditionalTechInfo
                          ,[AdminOrderComment] = @AdminOrderComment
                          ,[Decremented] = @Decremented
                          ,[ShippingCost] = @ShippingCost
                          ,[PaymentCost] = @PaymentCost
                          ,[TaxCost] = @TaxCost
                          ,[SupplyTotal] = @SupplyTotal
                          ,[Sum] = @Sum
                          ,[OrderStatusID] = @OrderStatusID
                          ,[ShippingMethodName] = @ShippingMethodName
                          ,[PaymentMethodName] = @PaymentMethodName
                          ,[GroupName] = @GroupName
                          ,[GroupDiscount] = @GroupDiscount
                          ,[OrderDate] = @OrderDate
                          ,[CertificateCode] = @CertificateCode
                          ,[CertificatePrice] = @CertificatePrice
                          ,[UseIn1C] = @UseIn1C
                          ,[ModifiedDate] = Getdate()
                     WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@Number", order.Number),
                new SqlParameter("@ShippingMethodID", order.ShippingMethodId == 0 ? (object)DBNull.Value : order.ShippingMethodId),
                new SqlParameter("@PaymentMethodID", order.PaymentMethodId == 0 ? (object)DBNull.Value : order.PaymentMethodId),
                new SqlParameter("@AffiliateID", order.AffiliateID),
                new SqlParameter("@OrderDiscount", order.OrderDiscount),
                new SqlParameter("@CustomerComment", order.CustomerComment ?? string.Empty),
                new SqlParameter("@StatusComment", order.StatusComment ?? string.Empty),
                new SqlParameter("@AdditionalTechInfo", order.AdditionalTechInfo ?? string.Empty),
                new SqlParameter("@AdminOrderComment", order.AdminOrderComment ?? string.Empty),
                new SqlParameter("@Decremented", order.Decremented),
                new SqlParameter("@ShippingCost", order.ShippingCost),
                new SqlParameter("@PaymentCost", order.PaymentCost),
                new SqlParameter("@TaxCost", order.TaxCost),
                new SqlParameter("@SupplyTotal", order.SupplyTotal),
                new SqlParameter("@Sum", order.Sum),
                new SqlParameter("@OrderStatusID", order.OrderStatusId),
                new SqlParameter("@OrderID", order.OrderID),
                new SqlParameter("@ShippingMethodName", order.ArchivedShippingName),
                new SqlParameter("@PaymentMethodName", order.PaymentMethodName),
                new SqlParameter("@GroupName", order.GroupName),
                new SqlParameter("@GroupDiscount", order.GroupDiscount),
                new SqlParameter("@OrderDate", order.OrderDate),
                new SqlParameter("@CertificateCode", order.Certificate != null ? (object)order.Certificate.Code : DBNull.Value),
                new SqlParameter("@CertificatePrice", order.Certificate != null ? (object)order.Certificate.Price : DBNull.Value),
                new SqlParameter("@UseIn1C", order.UseIn1C));
        }

        public static void DecrementProductsCountAccordingOrder(int ordId)
        {
            if (Settings1C.Enabled)
                return;

            SQLDataAccess.ExecuteNonQuery("[Order].[sp_DecrementProductsCountAccordingOrder]",
                                            CommandType.StoredProcedure,
                                            new SqlParameter("@orderId", ordId));

            foreach (var orderitem in GetOrderItems(ordId).Where(orderitem => orderitem.ProductID.HasValue))
            {
                ProductService.PreCalcProductParams((int)orderitem.ProductID);
            }
        }

        public static void IncrementProductsCountAccordingOrder(int ordId)
        {
            if (Settings1C.Enabled)
                return;

            SQLDataAccess.ExecuteNonQuery("[Order].[sp_IncrementProductsCountAccordingOrder]",
                                            CommandType.StoredProcedure,
                                            new SqlParameter("@orderId", ordId));

            foreach (var orderitem in GetOrderItems(ordId).Where(orderitem => orderitem.ProductID.HasValue))
            {
                ProductService.PreCalcProductParams((int)orderitem.ProductID);
            }
        }

        public static OrderCustomer GetOrderCustomer(int orderID)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Order].[OrderCustomer] WHERE [OrderID] = @OrderID",
                CommandType.Text,
                reader =>
                new OrderCustomer
                    {
                        CustomerID = SQLDataHelper.GetGuid(reader, "CustomerID"),
                        CustomerIP = SQLDataHelper.GetString(reader, "CustomerIP"),
                        FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                        Patronymic = SQLDataHelper.GetString(reader, "Patronymic"),
                        LastName = SQLDataHelper.GetString(reader, "LastName"),
                        Email = SQLDataHelper.GetString(reader, "Email"),
                        OrderID = orderID,
                        MobilePhone = SQLDataHelper.GetString(reader, "MobilePhone"),
                    }, new SqlParameter("@OrderID", orderID));
        }

        public static OrderCustomer GetOrderCustomer(string orderNumber)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Order].[OrderCustomer] WHERE [OrderID] = (Select OrderID from [Order].[Order] Where Number=@Number)",
                CommandType.Text,
                reader =>
                new OrderCustomer
                {
                    CustomerID = SQLDataHelper.GetGuid(reader, "CustomerID"),
                    CustomerIP = SQLDataHelper.GetString(reader, "CustomerIP"),
                    FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                    LastName = SQLDataHelper.GetString(reader, "LastName"),
                    Patronymic = SQLDataHelper.GetString(reader, "Patronymic"),
                    Email = SQLDataHelper.GetString(reader, "Email"),
                    OrderID = SQLDataHelper.GetInt(reader, "OrderID"),
                    MobilePhone = SQLDataHelper.GetString(reader, "MobilePhone"),
                }, new SqlParameter("@Number", orderNumber));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetOrderCustomersEmails()
        {
            return SQLDataAccess.ExecuteReadColumn<string>(
                "SELECT distinct [Email] FROM [Order].[OrderCustomer]",
                CommandType.Text,
                "Email");
        }

        public static OrderCurrency GetOrderCurrency(int orderId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Order].[OrderCurrency] WHERE [OrderID] = @OrderID",
                CommandType.Text,
                reader =>
                new OrderCurrency
                    {
                        CurrencyCode = SQLDataHelper.GetString(reader, "CurrencyCode"),
                        CurrencyNumCode = SQLDataHelper.GetInt(reader, "CurrencyNumCode"),
                        CurrencyValue = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                        CurrencySymbol = SQLDataHelper.GetString(reader, "CurrencySymbol"),
                        IsCodeBefore = SQLDataHelper.GetBoolean(reader, "IsCodeBefore")
                    }, new SqlParameter("@OrderID", orderId));
        }

        public static OrderPickPoint GetOrderPickPoint(int orderId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Order].[OrderPickPoint] WHERE [OrderID] = @OrderID",
                CommandType.Text,
                reader =>
                new OrderPickPoint
                {
                    OrderId = orderId,
                    PickPointId = SQLDataHelper.GetString(reader, "PickPointId"),
                    PickPointAddress = SQLDataHelper.GetString(reader, "PickPointAddress"),
                    AdditionalData = SQLDataHelper.GetString(reader, "AdditionalData", ""),
                }, new SqlParameter("@OrderID", orderId));
        }

        public static OrderContact GetOrderContact(int contactId)
        {
            return SQLDataAccess.ExecuteReadOne(
                "SELECT * FROM [Order].[OrderContact] WHERE [OrderContactID] = @OrderContactID",
                CommandType.Text, GetOrderContactFromReader,
                new SqlParameter("@OrderContactID", contactId));
        }

        public static OrderContact GetOrderContactFromReader(SqlDataReader reader)
        {
            return new OrderContact
            {
                OrderContactId = SQLDataHelper.GetInt(reader, "OrderContactID"),
                Address = SQLDataHelper.GetString(reader, "Address"),
                City = SQLDataHelper.GetString(reader, "City"),
                Country = SQLDataHelper.GetString(reader, "Country"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Zone = SQLDataHelper.GetString(reader, "Zone"),
                Zip = SQLDataHelper.GetString(reader, "Zip"),
                CustomField1 = SQLDataHelper.GetString(reader, "CustomField1"),
                CustomField2 = SQLDataHelper.GetString(reader, "CustomField2"),
                CustomField3 = SQLDataHelper.GetString(reader, "CustomField3"),
            };
        }

        public static void UpdateOrderCurrency(int orderId, string currencyCode, float currencyValue)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[OrderCurrency] SET [CurrencyCode] = @CurrencyCode, [CurrencyValue] = @CurrencyValue WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId),
                new SqlParameter("@CurrencyCode", currencyCode),
                new SqlParameter("@CurrencyValue", currencyValue));
        }

        public static void UpdateOrderCustomer(OrderCustomer customer)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Order].[OrderCustomer] SET [CustomerID] = @CustomerID, [FirstName] = @FirstName, LastName=@LastName, Email=@Email, MobilePhone=@MobilePhone, Patronymic=@Patronymic WHERE [OrderID] = @OrderID",
                CommandType.Text,
                new SqlParameter("@OrderID", customer.OrderID),
                new SqlParameter("@CustomerID", customer.CustomerID),
                new SqlParameter("@FirstName", customer.FirstName),
                new SqlParameter("@LastName", customer.LastName),
                new SqlParameter("@Patronymic", customer.Patronymic),
                new SqlParameter("@Email", customer.Email),
                new SqlParameter("@MobilePhone", customer.MobilePhone));
        }

        public static Order GetOrderByNumber(string orderNumber)
        {
            return GetOrder(GetOrderIdByNumber(orderNumber));
        }

        public static string ProcessOrder(Order order, PaymentService.PageWithPaymentButton page,
                                            PaymentMethod customPaymentMethod = null)
        {
            if (order == null)
                return string.Empty;

            var paymentMethod = customPaymentMethod ?? order.PaymentMethod;
            if (paymentMethod == null)
                return string.Empty;

            if (paymentMethod.ProcessType == ProcessType.FormPost)
            {
                return paymentMethod.ProcessFormString(order, page);
            }

            if (paymentMethod.ProcessType == ProcessType.ServerRequest)
            {
                string href = paymentMethod.ProcessServerRequest(order);

                if (page == PaymentService.PageWithPaymentButton.myaccount)
                {
                    return Button.RenderHtml(Resource.Client_OrderConfirmation_PayOrder,
                        Button.eType.Confirm, Button.eSize.Small,
                        href: href);
                }
                else
                {
                    return Button.RenderHtml(Resource.Client_OrderConfirmation_PayOrder,
                        Button.eType.Submit, Button.eSize.Middle,
                        href: href);
                }
            }

            if (paymentMethod.ProcessType == ProcessType.Javascript)
            {
                var pm = paymentMethod.Type.ToEnum<PaperPaymentType>();
                var buttonTextDictionary = new Dictionary<PaperPaymentType, string>
                {
                    {PaperPaymentType.NonPaperMethod, Resource.Client_OrderConfirmation_PayNonPaperMethod},
                    {PaperPaymentType.SberBank, Resource.Client_OrderConfirmation_PrintLuggage},
                    {PaperPaymentType.Bill, Resource.Client_OrderConfirmation_PrintBill},
                    {PaperPaymentType.Check, Resource.Client_OrderConfirmation_PrintCheck},
                    {PaperPaymentType.BillUa, Resource.Client_OrderConfirmation_PrintBill}
                };

                var buttonText =
                    PaymentService.GetCreditPaymentMethods()
                        .Any(x => x.PaymentMethodId == paymentMethod.PaymentMethodId)
                        ? Resource.Client_OrderConfirmation_CreditProcessing
                        : buttonTextDictionary[pm];

                if (page == PaymentService.PageWithPaymentButton.myaccount)
                {
                    return paymentMethod.ProcessJavascript(order) + " " +
                           Button.RenderHtml(buttonText, Button.eType.Confirm, Button.eSize.Small,
                               onClientClick: paymentMethod.ProcessJavascriptButton(order));
                }
                else
                {
                    return paymentMethod.ProcessJavascript(order) + " " +
                           Button.RenderHtml(buttonText, Button.eType.Submit, Button.eSize.Middle,
                               onClientClick: paymentMethod.ProcessJavascriptButton(order));
                }
            }
            return string.Empty;
        }

        public static int GetOrderCountByStatusId(object statusId)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM [Order].[Order] WHERE [OrderStatusID] = @StatusID",
                CommandType.Text,
                new SqlParameter("@StatusID", statusId));
        }

        public static void CancelOrder(int orderID)
        {
            ChangeOrderStatus(orderID, CanceledOrderStatus);
            UpdateStatusComment(orderID, Resource.Client_MyAccount_UserCanceledOrder);
        }

        public static OrderStatus GetOrderStatusByName(string statusName)
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "SELECT * FROM [Order].[OrderStatus] WHERE LOWER ([StatusName]) = LOWER (@StatusName)",
                    CommandType.Text,
                    reader =>
                    new OrderStatus
                    {
                        StatusID = SQLDataHelper.GetInt(reader, "OrderStatusID"),
                        StatusName = SQLDataHelper.GetString(reader, "StatusName"),
                        Command = (OrderStatusCommand)SQLDataHelper.GetInt(reader, "CommandID"),
                        IsDefault = SQLDataHelper.GetBoolean(reader, "IsDefault"),
                        IsCanceled = SQLDataHelper.GetBoolean(reader, "IsCanceled")
                    },
                    new SqlParameter("@StatusName", statusName));
        }

        private static string GetProductPhoto(int? productId, int? photoId)
        {
            if (photoId != null)
            {
                var photo = PhotoService.GetPhoto((int)photoId);
                if (photo != null)
                    return string.Format("<img src='{0}' />",
                                         SettingsMain.SiteUrl.Trim('/') + '/' +
                                         FoldersHelper.GetImageProductPath(ProductImageType.Small, photo.PhotoName,
                                                                           false));
            }

            if (productId != null)
            {
                var p = ProductService.GetProduct((int)productId);
                if (p != null && p.Photo.IsNotEmpty())
                {
                    return string.Format("<img src='{0}' />",
                                         SettingsMain.SiteUrl.Trim('/') + '/' +
                                         FoldersHelper.GetImageProductPath(ProductImageType.Small, p.Photo, false));
                }
            }

            return "&nbsp;";
        }


        public static string GenerateHtmlOrderTable(IList<OrderItem> orderItems, Currency currency, float productsPrice, float orderDiscountPercent,
                                                        OrderCoupon coupon, OrderCertificate certificate, float totalDiscount, float shippingPrice,
                                                        float paymentPrice, float taxesTotal, float bonusPrice, float newBonus)
        {
            var htmlOrderTable = new StringBuilder();

            htmlOrderTable.Append("<table style=\'width:100%; border-collapse:collapse;\' cellspacing=\'3\' cellpadding=\'3\'>");
            htmlOrderTable.Append("<tr style='border-bottom: 1px #ccc solid'>");
            htmlOrderTable.AppendFormat("<td style=\'width:100px; text-align: center;\'>&nbsp;</td>");
            htmlOrderTable.AppendFormat("<td style=\'width:100px; text-align: center;\'>{0}</td>", Resource.Client_OrderConfirmation_SKU);
            htmlOrderTable.AppendFormat("<td>{0}</td>", Resource.Client_OrderConfirmation_Name);
            htmlOrderTable.AppendFormat("<td style=\'width:90px; text-align:center;\'>{0}</td>", Resource.Client_OrderConfirmation_Price);
            htmlOrderTable.AppendFormat("<td style=\'width:80px; text-align: center;\' >{0}</td>", Resource.Client_OrderConfirmation_Count);
            htmlOrderTable.AppendFormat("<td style=\'width:100px;\'>{0}</td>", Resource.Client_OrderConfirmation_Cost);
            htmlOrderTable.Append("</tr>");

            // Добавление заказанных товаров
            foreach (var item in orderItems)
            {
                if (item.ProductID.HasValue)
                {
                    htmlOrderTable.Append("<tr style='border-bottom: 1px #ccc solid'>");
                    if (item.ProductID != null)
                    {
                        Photo photo;
                        if (item.PhotoID.HasValue && item.PhotoID != 0 && (photo = PhotoService.GetPhoto((int)item.PhotoID)) != null)
                        {
                            htmlOrderTable.AppendFormat("<td style=\'text-align: center;\'><img src='{0}' /></td>",
                                                        StringHelper.MakeASCIIUrl(SettingsMain.SiteUrl.Trim('/')) + '/' + FoldersHelper.GetImageProductPath(ProductImageType.XSmall, photo.PhotoName, false));
                        }
                        else
                        {
                            htmlOrderTable.AppendFormat("<td>&nbsp;</td>");
                        }
                    }
                    htmlOrderTable.AppendFormat("<td style=\'text-align: center;\' >{0}</td>", item.ArtNo);
                    htmlOrderTable.AppendFormat("<td>{0} {1} {2} {3}</td>", item.Name,
                                                item.Color.IsNotEmpty() ? "<div>" + SettingsCatalog.ColorsHeader + ": " + item.Color + "</div>" : "",
                                                item.Size.IsNotEmpty() ? "<div>" + SettingsCatalog.SizesHeader + ": " + item.Size + "</div>" : "",
                                                RenderSelectedOptions(item.SelectedOptions));
                    htmlOrderTable.AppendFormat("<td style=\'text-align: center;\'>{0}</td>", CatalogService.GetStringPrice(item.Price, currency));
                    htmlOrderTable.AppendFormat("<td style=\'text-align: center;\'>{0}</td>", item.Amount);
                    htmlOrderTable.AppendFormat("<td>{0}</td>", CatalogService.GetStringPrice(item.Price * item.Amount, currency));
                    htmlOrderTable.Append("</tr>");
                }
            }

            // Стоимость заказа
            htmlOrderTable.Append("<tr>");
            htmlOrderTable.AppendFormat("<td colspan=\'5\' style=\'text-align:right;\'><b>{0}:</b></td>", Resource.Client_OrderConfirmation_OrderCost);
            htmlOrderTable.AppendFormat("<td><b>{0}</b></td>", CatalogService.GetStringPrice(productsPrice, currency));
            htmlOrderTable.Append("</tr>");

            if (orderDiscountPercent != 0)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td colspan=\'5\' style=\'text-align:right;\'><b>{0}:</b></td>", Resource.Client_OrderConfirmation_Discount);
                htmlOrderTable.AppendFormat("<td><b>-{0}</b></td>", CatalogService.GetStringDiscountPercent(productsPrice, orderDiscountPercent, false));
                htmlOrderTable.Append("</tr>");
            }

            if (bonusPrice != 0)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td colspan=\'5\' style=\'text-align:right;\'><b>{0}:</b></td>", Resource.Client_OrderConfirmation_Bonuses);
                htmlOrderTable.AppendFormat("<td><b>-{0}</b></td>", CatalogService.GetStringPrice(bonusPrice));
                htmlOrderTable.Append("</tr>");
            }

            if (certificate != null)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td colspan=\'5\' style=\'text-align:right;\'><b>{0}:</b></td>", Resource.Client_OrderConfirmation_Certificate);
                htmlOrderTable.AppendFormat("<td><b>-{0}</b></td>", CatalogService.GetStringPrice(certificate.Price, currency));
                htmlOrderTable.Append("</tr>");
            }
            if (coupon != null)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td colspan=\'5\' style=\'text-align:right;\'><b>{0}:</b></td>", Resource.Client_OrderConfirmation_Coupon);
                if (coupon.Type == CouponType.Fixed)
                {
                    htmlOrderTable.AppendFormat("<td><b>-{0}</b></td>",
                                                CatalogService.GetStringPrice(coupon.Value, currency));
                }
                else
                {
                    htmlOrderTable.AppendFormat("<td><b>-{0}%</b></td>", CatalogService.FormatPriceInvariant(coupon.Value));
                }
                htmlOrderTable.Append("</tr>");
            }

            // Стоимость доставки
            if (shippingPrice != 0)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td colspan=\'5\' style=\'text-align:right;\'><b>{0}:</b></td>", Resource.Client_OrderConfirmation_DeliveryCost);
                htmlOrderTable.AppendFormat("<td><b>{0}</b></td>", CatalogService.GetStringPrice(shippingPrice, currency));
                htmlOrderTable.Append("</tr>");
            }

            if (paymentPrice != 0)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td colspan=\'5\' style=\'text-align:right;\'><b>{0}:</b></td>", paymentPrice > 0 ? Resource.Client_OrderConfirmation_PaymentCost : Resource.Client_OrderConfirmation_PaymentDiscount);
                htmlOrderTable.AppendFormat("<td><b>{0}</b></td>", CatalogService.GetStringPrice(paymentPrice, currency));
                htmlOrderTable.Append("</tr>");
            }

            var taxes = TaxServices.CalculateTaxes(productsPrice - totalDiscount - bonusPrice + shippingPrice);
            foreach (var tax in taxes)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td colspan=\'5\' style=\'text-align:right;\'><b>{0}:</b></td>",
                                            (tax.Key.ShowInPrice ? Resource.Core_TaxServices_Include_Tax : "") + " " + tax.Key.Name);
                htmlOrderTable.AppendFormat("<td><b>{0}</b></td>",
                                            (tax.Key.ShowInPrice ? "" : "+") +
                                            CatalogService.GetStringPrice(tax.Value, currency));
                htmlOrderTable.Append("</tr>");
            }

            var total = productsPrice - totalDiscount - bonusPrice + shippingPrice + taxes.Where(tax => !tax.Key.ShowInPrice).Sum(tax => tax.Value) + paymentPrice;
            if (total < 0) total = 0;

            // Итого
            htmlOrderTable.Append("<tr>");
            htmlOrderTable.AppendFormat("<td colspan=\'5\' style=\'text-align:right;\'><b>{0}:</b></td>", Resource.Client_OrderConfirmation_Total);
            htmlOrderTable.AppendFormat("<td><b>{0}</b></td>", CatalogService.GetStringPrice(total, currency));
            htmlOrderTable.Append("</tr>");


            if (newBonus > 0)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td colspan=\'5\' style=\'text-align:right;\'><b>{0}:</b></td>", Resource.Client_OrderConfirmation_NewBonus);
                htmlOrderTable.AppendFormat("<td><b>{0}</b></td>", newBonus.ToString("F2"));
                htmlOrderTable.Append("</tr>");
            }

            htmlOrderTable.Append("</table>");

            return htmlOrderTable.ToString();
        }

        public static string GenerateHtmlOrderCertificateTable(IList<GiftCertificate> orderCetificates, Currency currency, float paymentPrice, float taxesTotal)
        {
            var htmlOrderTable = new StringBuilder();

            htmlOrderTable.Append("<table width=\'100%\' border=\'0\' cellspacing=\'1\' cellpadding=\'2\'>");
            htmlOrderTable.Append("<tr>");
            htmlOrderTable.AppendFormat("<td style=\'border: 1px #000 solid;\'>&nbsp;</td>");
            htmlOrderTable.AppendFormat("<td style=\'border: 1px #000 solid;\'>{0}</td>", Resource.Client_OrderConfirmation_Certificate);
            htmlOrderTable.AppendFormat("<td style=\'border: 1px #000 solid;\'>{0}</td>", Resource.Client_OrderConfirmation_Price);
            htmlOrderTable.Append("</tr>");

            // Добавление заказанных сертификатов
            foreach (var item in orderCetificates)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td style=\'border: 1px #000 solid;\'>&nbsp;</td>");
                htmlOrderTable.AppendFormat("<td style=\'border: 1px #000 solid;\'>{0}</td>", item.CertificateCode);
                htmlOrderTable.AppendFormat("<td style=\'border: 1px #000 solid;\'>{0}</td>", CatalogService.GetStringPrice(item.Sum, currency));
                htmlOrderTable.Append("</tr>");
            }

            // Налоги

            var taxes = TaxServices.CalculateCertificateTaxes(orderCetificates.Sum(cert => cert.Sum));
            float taxesExcluded = taxes.Where(tax => tax.Key.ShowInPrice).Sum(tax => tax.Value);

            foreach (var tax in taxes)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td colspan=\'2\' style=\'text-align:right; border: 1px #000 solid;\'><b>{0}:</b></td>",
                                            (tax.Key.ShowInPrice ? Resource.Core_TaxServices_Include_Tax : "") + " " + tax.Key.Name);
                htmlOrderTable.AppendFormat("<td style=\'width:200px; border: 1px #000 solid;\'><b>{0}</b></td>", (tax.Key.ShowInPrice ? "" : "+") + CatalogService.GetStringPrice(tax.Value, currency));
                htmlOrderTable.Append("</tr>");
            }


            if (paymentPrice != 0)
            {
                htmlOrderTable.Append("<tr>");
                htmlOrderTable.AppendFormat("<td colspan=\'2\' style=\'text-align:right; border: 1px #000 solid;\'><b>{0}:</b></td>", paymentPrice > 0 ? Resource.Client_OrderConfirmation_PaymentCost : Resource.Client_OrderConfirmation_PaymentDiscount);
                htmlOrderTable.AppendFormat("<td style=\'width:200px; border: 1px #000 solid;\'><b>{0}</b></td>", CatalogService.GetStringPrice(paymentPrice, currency));
                htmlOrderTable.Append("</tr>");
            }

            // Итого
            htmlOrderTable.Append("<tr>");
            htmlOrderTable.AppendFormat("<td colspan=\'2\' style=\'text-align:right; border: 1px #000 solid;\'><b>{0}:</b></td>", Resource.Client_OrderConfirmation_Total);
            htmlOrderTable.AppendFormat("<td style=\'width:200px; border: 1px #000 solid;\'><b>{0}</b></td>", CatalogService.GetStringPrice(orderCetificates.Sum(cert => cert.Sum) + paymentPrice + taxesExcluded, currency));
            htmlOrderTable.Append("</tr>");

            htmlOrderTable.Append("</table>");

            return htmlOrderTable.ToString();
        }

        public static string RenderSelectedOptions(IEnumerable<EvaluatedCustomOptions> evlist)
        {
            if (evlist == null || !evlist.Any())
                return String.Empty;

            var res = new StringBuilder("<div class=\"customoptions\">");

            foreach (EvaluatedCustomOptions evco in evlist)
            {
                res.Append(evco.CustomOptionTitle);
                res.Append(": ");
                res.Append(evco.OptionTitle);
                res.Append(" ");
                if (evco.OptionPriceBc > 0)
                {
                    res.Append("+");
                    res.Append(CatalogService.GetStringPrice(evco.OptionPriceBc));
                }
                res.Append("<br />");
            }

            res.Append("</div>");

            return res.ToString();
        }

        public static bool IsPaidOrder(int orderId)
        {
            return Convert.ToInt32(
                SQLDataAccess.ExecuteScalar(
                "Select COUNT([PaymentDate]) FROM [Order].[Order] WHERE OrderID = @OrderID AND [PaymentDate] is not null",
                CommandType.Text,
                new SqlParameter("@OrderID", orderId))) > 0;
        }

        public static string GetBillingLinkHash(Order order)
        {
            if (order == null || order.OrderCustomer == null)
                return string.Empty;

            return (order.OrderID + order.Number + order.OrderCustomer.CustomerID).Md5(false).ToString();
        }

        public static void ChangeUseIn1C(int orderId, bool useIn1C)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [Order].[Order] Set UseIn1C = @UseIn1C, ModifiedDate = Getdate() Where OrderId=@OrderId",
                CommandType.Text,
                new SqlParameter("@OrderId", orderId),
                new SqlParameter("@UseIn1C", useIn1C));
        }

        #region Order status 1C

        public static void AddOrUpdateOrderStatus1C(OrderStatus1C status)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"If ((Select Count(OrderId) From [Order].[OrderStatus1C] Where OrderId = @OrderId) > 0) 
                begin 
                    Update [Order].[OrderStatus1C] Set Status1C=@Status1C, OrderId1C=@OrderId1C, OrderDate=@OrderDate Where OrderId=@OrderId
                end 
                Else 
                begin 
                    Insert Into [Order].[OrderStatus1C] ([OrderId],[Status1C],[OrderId1C],[OrderDate]) Values (@OrderId, @Status1C, @OrderId1C, @OrderDate); 
                end ",
                CommandType.Text,
                new SqlParameter("@OrderId", status.OrderId),
                new SqlParameter("@Status1C", status.Status1C),
                new SqlParameter("@OrderId1C", status.OrderId1C),
                new SqlParameter("@OrderDate", status.OrderDate));
        }

        public static OrderStatus1C GetStatus1C(int orderId)
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "Select * From [Order].[OrderStatus1C] Where OrderId = @OrderId", CommandType.Text,
                    reader => new OrderStatus1C()
                    {
                        OrderId = SQLDataHelper.GetInt(reader, "OrderId"),
                        Status1C = SQLDataHelper.GetString(reader, "Status1C"),
                        OrderId1C = SQLDataHelper.GetString(reader, "OrderId1C"),
                        OrderDate = SQLDataHelper.GetString(reader, "OrderDate"),
                    },
                    new SqlParameter("@OrderId", orderId));
        }

        #endregion
    }
}