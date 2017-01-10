//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.Orders
{
    public enum ShoppingCartType
    {
        /// <summary>
        /// Shopping cart
        /// </summary>
        ShoppingCart = 1,

        /// <summary>
        /// Wishlist
        /// </summary>
        Wishlist = 2,

        /// <summary>
        /// Compare product
        /// </summary>
        Compare = 3
    }

    public static class ShoppingCartService
    {
        public static ShoppingCart CurrentShoppingCart
        {
            get { return GetShoppingCart(ShoppingCartType.ShoppingCart); }
        }

        public static ShoppingCart CurrentCompare
        {
            get { return GetShoppingCart(ShoppingCartType.Compare); }
        }

        public static ShoppingCart CurrentWishlist
        {
            get { return GetShoppingCart(ShoppingCartType.Wishlist); }
        }


        public static ShoppingCart GetShoppingCart(ShoppingCartType shoppingCartType)
        {
            return GetShoppingCart(shoppingCartType, CustomerContext.CustomerId);
        }

        public static ShoppingCart GetShoppingCart(ShoppingCartType shoppingCartType, Guid customerId)
        {
            var templist =
                SQLDataAccess.ExecuteReadList(
                    "SELECT * FROM Catalog.ShoppingCart WHERE ShoppingCartType = @ShoppingCartType and CustomerId = @CustomerId",
                    CommandType.Text, GetFromReader,
                    new SqlParameter("@ShoppingCartType", (int) shoppingCartType),
                    new SqlParameter("@CustomerId", customerId));

            var shoppingCart = new ShoppingCart();
            shoppingCart.AddRange(templist);
            return shoppingCart;
        }

        public static ShoppingCart GetAllShoppingCarts(Guid customerId)
        {
            var shoppingCart = new ShoppingCart();

            foreach (ShoppingCartType shoppingCartType in Enum.GetValues(typeof(ShoppingCartType)))
            {
                shoppingCart.AddRange(GetShoppingCart(shoppingCartType, customerId));
            }

            return shoppingCart;
        }

        ///// <summary>
        ///// Gets a shopping cart item
        ///// </summary>
        ///// <param name = "itemId"></param>
        ///// <returns></returns>
        //public static ShoppingCartItem GetShoppingCartItem(int itemId)
        //{
        //    if (itemId < 0)
        //    {
        //        return null;
        //    }

        //    return SQLDataAccess.ExecuteReadOne
        //        ("SELECT * FROM Catalog.ShoppingCart WHERE ItemId = @ItemId", CommandType.Text, GetFromReader,
        //            new SqlParameter { ParameterName = "@ItemId", Value = itemId }
        //        );
        //}

        //private static float GetShoppingCartItemAmount(Guid customerId, ShoppingCartItem item)
        //{
        //    return SQLDataAccess.ExecuteScalar<float>
        //         (" SELECT amount FROM [Catalog].[ShoppingCart] " +
        //          " WHERE [CustomerId] = @CustomerId AND " +
        //                " [OfferId] = @OfferId AND " +
        //                " [ShoppingCartType] = @ShoppingCartType AND " +
        //                " [AttributesXml] = @AttributesXml",
        //             CommandType.Text,
        //             new SqlParameter { ParameterName = "@CustomerId", Value = customerId },
        //             new SqlParameter { ParameterName = "@OfferId", Value = item.OfferId },
        //             new SqlParameter { ParameterName = "@AttributesXml", Value = item.AttributesXml ?? String.Empty },
        //             new SqlParameter { ParameterName = "@ShoppingCartType", Value = (int)item.ShoppingCartType }
        //         );
        //}

        public static void AddShoppingCartItem(ShoppingCartItem item)
        {
            var customerId = CustomerContext.CustomerId;
            AddShoppingCartItem(item, customerId);
        }

        public static void AddShoppingCartItem(ShoppingCartItem item, Guid customerId)
        {
            item.CustomerId = customerId;

            var shoppingCartItem = GetExistsShoppingCartItem(customerId, item.OfferId, item.AttributesXml, item.ShoppingCartType);
            if (shoppingCartItem != null)
            {
                shoppingCartItem.Amount += item.Amount;
                UpdateShoppingCartItem(shoppingCartItem);
            }
            else
            {
                InsertShoppingCartItem(item);
            }
        }

        public static ShoppingCartItem GetExistsShoppingCartItem(Guid customerId, int offerId, string attributesXml, ShoppingCartType shoppingCartType)
        {
            return
                SQLDataAccess.ExecuteReadOne(
                    "SELECT * FROM [Catalog].[ShoppingCart] WHERE [CustomerId] = @CustomerId  AND [OfferId] = @OfferId AND [ShoppingCartType] = @ShoppingCartType AND [AttributesXml] = @AttributesXml",
                    CommandType.Text,
                    (reader) => new ShoppingCartItem
                    {
                        ShoppingCartItemId = SQLDataHelper.GetInt(reader, "ShoppingCartItemId"),
                        ShoppingCartType = (ShoppingCartType) SQLDataHelper.GetInt(reader, "ShoppingCartType"),
                        OfferId = SQLDataHelper.GetInt(reader, "OfferID"),
                        CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                        AttributesXml = SQLDataHelper.GetString(reader, "AttributesXml"),
                        Amount = SQLDataHelper.GetFloat(reader, "Amount"),
                        CreatedOn = SQLDataHelper.GetDateTime(reader, "CreatedOn"),
                        UpdatedOn = SQLDataHelper.GetDateTime(reader, "UpdatedOn"),
                    },
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@OfferId", offerId),
                    new SqlParameter("@AttributesXml", attributesXml ?? String.Empty),
                    new SqlParameter("@ShoppingCartType", (int) shoppingCartType));
        }

        /// <summary>
        /// insert new shoppingCartItem, CreatedOn and UpdatedOn must get on sql GetDate()
        /// </summary>
        /// <param name = "item"></param>
        public static void InsertShoppingCartItem(ShoppingCartItem item)
        {
            item.ShoppingCartItemId = SQLDataAccess.ExecuteScalar<int>(
                "INSERT INTO Catalog.ShoppingCart (ShoppingCartType, CustomerId, OfferId, AttributesXml, Amount, CreatedOn, UpdatedOn) " +
                "VALUES (@ShoppingCartType, @CustomerId, @OfferId, @AttributesXml, @Amount, GetDate(), GetDate()); Select SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@ShoppingCartType", (int) item.ShoppingCartType),
                new SqlParameter("@CustomerId", item.CustomerId),
                new SqlParameter("@OfferId", item.OfferId),
                new SqlParameter("@AttributesXml", item.AttributesXml ?? String.Empty),
                new SqlParameter("@Amount", item.Amount));
        }


        public static void UpdateShoppingCartItem(ShoppingCartItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [Catalog].[ShoppingCart] SET [ShoppingCartType] = @ShoppingCartType, [CustomerId] = @CustomerId, [OfferId] = @OfferId, [AttributesXml] = @AttributesXml, [UpdatedOn] = GetDate(), [Amount] = @Amount WHERE [ShoppingCartItemId] = @ShoppingCartItemId",
                CommandType.Text,
                new SqlParameter("@ShoppingCartType", (int) item.ShoppingCartType),
                new SqlParameter("@ShoppingCartItemId", item.ShoppingCartItemId),
                new SqlParameter("@CustomerId", item.CustomerId),
                new SqlParameter("@OfferId", item.OfferId),
                new SqlParameter("@AttributesXml", item.AttributesXml ?? String.Empty),
                new SqlParameter("@Amount", item.Amount));
        }

        public static void ClearShoppingCart(ShoppingCartType shoppingCartType)
        {
            ClearShoppingCart(shoppingCartType, CustomerContext.CustomerId);
        }

        public static void ClearShoppingCart(ShoppingCartType shoppingCartType, Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.ShoppingCart WHERE ShoppingCartType = @ShoppingCartType and CustomerId = @CustomerId",
                CommandType.Text,
                new SqlParameter("@ShoppingCartType", (int) shoppingCartType),
                new SqlParameter("@CustomerId", customerId));
        }

        public static void DeleteExpiredShoppingCartItems(DateTime olderThan)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM Catalog.ShoppingCart WHERE CreatedOn<@olderThan",
                CommandType.Text, new SqlParameter("@olderThan", olderThan));
        }

        public static void DeleteShoppingCartItem(int itemId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM Catalog.ShoppingCart WHERE ShoppingCartItemId = @ShoppingCartItemId", CommandType.Text,
                new SqlParameter("@ShoppingCartItemId", itemId));
        }

        public static void MergeShoppingCarts(Guid oldCustomerId, Guid currentCustomerId)
        {
            if (oldCustomerId == currentCustomerId) return;
            foreach (var item in GetAllShoppingCarts(oldCustomerId))
            {
                AddShoppingCartItem(item, currentCustomerId);
            }
        }

        private static ShoppingCartItem GetFromReader(SqlDataReader reader)
        {
            return new ShoppingCartItem
            {
                ShoppingCartItemId = SQLDataHelper.GetInt(reader, "ShoppingCartItemId"),
                ShoppingCartType = (ShoppingCartType)SQLDataHelper.GetInt(reader, "ShoppingCartType"),
                OfferId = SQLDataHelper.GetInt(reader, "OfferID"),
                CustomerId = SQLDataHelper.GetGuid(reader, "CustomerId"),
                AttributesXml = SQLDataHelper.GetString(reader, "AttributesXml"),
                Amount = SQLDataHelper.GetFloat(reader, "Amount"),
                CreatedOn = SQLDataHelper.GetDateTime(reader, "CreatedOn"),
                UpdatedOn = SQLDataHelper.GetDateTime(reader, "UpdatedOn"),
            };
        }
    }
}