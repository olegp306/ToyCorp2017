//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Customers
{
    public class CustomerGroupService
    {
        public static int DefaultCustomerGroup = 1;

        public static CustomerGroup GetCustomerGroup(int customerGroupId)
        {
            var customerGroup = SQLDataAccess.ExecuteReadOne<CustomerGroup>("SELECT * FROM [Customers].[CustomerGroup] WHERE CustomerGroupId = @CustomerGroupId",
                                                                                      CommandType.Text, GetCustomerGroupFromReader, new SqlParameter("@CustomerGroupId", customerGroupId));
            return customerGroup;
        }

        public static List<CustomerGroup> GetCustomerGroupList()
        {
            List<CustomerGroup> customerGroupList = SQLDataAccess.ExecuteReadList<CustomerGroup>("SELECT * FROM [Customers].[CustomerGroup] order by GroupDiscount", CommandType.Text, GetCustomerGroupFromReader);
            return customerGroupList;
        }

        public static List<int> GetCustomerGroupListIds()
        {
            List<int> customerGroupListIds = SQLDataAccess.ExecuteReadList<int>("SELECT [CustomerGroupId] FROM [Customers].[CustomerGroup]", CommandType.Text,
                                                                           reader => SQLDataHelper.GetInt(reader, "CustomerGroupId"));
            return customerGroupListIds;
        }

        private static CustomerGroup GetCustomerGroupFromReader(SqlDataReader reader)
        {
            return new CustomerGroup
            {
                CustomerGroupId = SQLDataHelper.GetInt(reader, "CustomerGroupId"),
                GroupName = SQLDataHelper.GetString(reader, "GroupName"),
                GroupDiscount = SQLDataHelper.GetFloat(reader, "GroupDiscount")
            };
        }

        public static void AddCustomerGroup(CustomerGroup customerGroup)
        {
            customerGroup.CustomerGroupId = SQLDataAccess.ExecuteScalar<int>("INSERT INTO [Customers].[CustomerGroup] ([GroupName], [GroupDiscount]) VALUES (@GroupName, @GroupDiscount); SELECT SCOPE_IdENTITY();",
                                                                                CommandType.Text,
                                                                                new SqlParameter("@GroupName", customerGroup.GroupName),
                                                                                new SqlParameter("@GroupDiscount", customerGroup.GroupDiscount));
        }

        public static void UpdateCustomerGroup(CustomerGroup customerGroup)
        {
            SQLDataAccess.ExecuteNonQuery(" UPDATE [Customers].[CustomerGroup] SET [GroupName] = @GroupName, [GroupDiscount] = @GroupDiscount " +
                                          " WHERE CustomerGroupId = @CustomerGroupId", CommandType.Text,
                                          new SqlParameter("@CustomerGroupId", customerGroup.CustomerGroupId),
         new SqlParameter("@GroupName", customerGroup.GroupName),
                                          new SqlParameter("@GroupDiscount", customerGroup.GroupDiscount));
        }

        public static void DeleteCustomerGroup(int customerGroupId)
        {
            if (customerGroupId != DefaultCustomerGroup)
            {
                SQLDataAccess.ExecuteNonQuery("UPDATE [Customers].[Customer] set CustomerGroupId = @NewCustomerGroupId Where CustomerGroupId=@OldCustomerGroupId",
                                                CommandType.Text, 
                                                new SqlParameter("@OldCustomerGroupId", customerGroupId),
                                                new SqlParameter("@NewCustomerGroupId", DefaultCustomerGroup));

                SQLDataAccess.ExecuteNonQuery("DELETE FROM [Customers].[CustomerGroup] WHERE CustomerGroupId = @CustomerGroupId",
                                                CommandType.Text, new SqlParameter("@CustomerGroupId", customerGroupId));
            }
        }
    }
}