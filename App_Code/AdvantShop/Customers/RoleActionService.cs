using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Customers
{
    public class RoleActionService
    {
        #region Role Action

        public static RoleAction GetRoleActionById(int roleActionId)
        {
            var roleAction = SQLDataAccess.ExecuteReadOne<RoleAction>("Select * From Customers.RoleAction where RoleActionID=@RoleActionID", CommandType.Text,
                                                                             GetRoleActionFromReader,
                                                                             new SqlParameter("@RoleActionID", roleActionId));
            return roleAction;
        }

        private static RoleAction GetRoleActionFromReader(SqlDataReader reader)
        {
            return new RoleAction
            {
                RoleActionID = SQLDataHelper.GetInt(reader, "RoleActionID"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Key = (RoleActionKey)Enum.Parse(typeof(RoleActionKey), SQLDataHelper.GetString(reader, "Key")),
                Category = SQLDataHelper.GetString(reader, "Category"),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder")
            };
        }

        public static List<RoleAction> GetRoleActions()
        {
            List<RoleAction> roleActionList = SQLDataAccess.ExecuteReadList<RoleAction>("Select * From Customers.RoleAction Where Enabled='True' Order By Category, SortOrder", CommandType.Text,
                                                                            GetRoleActionFromReader);
            return roleActionList;
        }

        #endregion

        #region Customer Role Action

        public static List<RoleActionKey> GetCustomerRoleKeysByCustomerId(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadList<RoleActionKey>("Select RoleActionKey From Customers.CustomerRoleAction Where CustomerRoleAction.CustomerID=@CustomerID And Enabled='True'", CommandType.Text,
                                                                    reader => (RoleActionKey)Enum.Parse(typeof(RoleActionKey), SQLDataHelper.GetString(reader, "RoleActionKey")),
                                                                    new SqlParameter("@CustomerID", customerId));
        }

        public static void UpdateOrInsertCustomerRoleAction(Guid customerId, string roleActionKey, bool enabled)
        {
            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_UpdateInserCustomerRoleAction]", CommandType.StoredProcedure,
                                                new SqlParameter("@CustomerID", customerId),
                                                new SqlParameter("@RoleActionKey", roleActionKey),
                                                new SqlParameter("@Enabled", enabled));            
        }

        public static void DeleteCustomerRoleActions(Guid customerId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From Customers.CustomerRoleAction Where CustomerID = @CustomerID", CommandType.Text,
                                                new SqlParameter("@CustomerID", customerId));

            var cacheName = CacheNames.GetRoleActionsCacheObjectName(customerId.ToString());
            if (CacheManager.Contains(cacheName))
            {
                CacheManager.Remove(cacheName);
            }
        }


        /// <summary>
        /// Действия доступные данному пользователю
        /// </summary>
        /// <returns></returns>
        public static List<RoleAction> GetCustomerRoleActionsByCustomerId(Guid customerId)
        {
            var cacheName = CacheNames.GetRoleActionsCacheObjectName(customerId.ToString());
            if (CacheManager.Contains(cacheName))
            {
                var actions = CacheManager.Get<List<RoleAction>>(cacheName);
                if (actions != null)
                    return actions;
            }
            return GetCustomerRoleActionsByCustomerIdFromDB(customerId);
        }

        private static List<RoleAction> GetCustomerRoleActionsByCustomerIdFromDB(Guid customerId)
        {
            List<RoleAction> actions = SQLDataAccess.ExecuteReadList<RoleAction>("Select RoleActionKey, Enabled From Customers.CustomerRoleAction Where CustomerID = @CustomerID", CommandType.Text,
                                                                     GetCustomerRoleActionFromReader,
                                                                     new SqlParameter("@CustomerID", customerId));

            CacheManager.Insert(CacheNames.GetRoleActionsCacheObjectName(customerId.ToString()), actions);

            return actions;
        }

        public static bool HasCustomerRoleAction(Guid customerId, RoleActionKey key)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select count (RoleActionKey) From Customers.CustomerRoleAction Where CustomerID = @CustomerID And RoleActionKey = @Key and Enabled=1", CommandType.Text,
                                                                     new SqlParameter("@CustomerID", customerId),
                                                                     new SqlParameter("@Key", key.ToString())) > 0;
        }

        private static RoleAction GetCustomerRoleActionFromReader(SqlDataReader reader)
        {
            return new RoleAction
                        {
                            Key = (RoleActionKey)Enum.Parse(typeof(RoleActionKey), SQLDataHelper.GetString(reader, "RoleActionKey")),
                            Enabled = SQLDataHelper.GetBoolean(reader, "Enabled")
                        };
        }
        #endregion
    }
}