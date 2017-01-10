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

namespace AdvantShop.Customers
{
    public class CustomerService
    {
        public static List<string> GetCustomersIDs()
        {
            List<string> result = SQLDataAccess.ExecuteReadList<string>("SELECT [CustomerID] FROM [Customers].[Customer]",
                                                                CommandType.Text, reader => SQLDataHelper.GetGuid(reader, "CustomerID").ToString());
            return result;
        }

        //todo vladimir не надо вытаскивать всего кастомера
        public static string GetCustomerEmailById(Guid custId)
        {
            var result = SQLDataAccess.ExecuteReadOne<string>("[Customers].[sp_GetCustomerByID]", CommandType.StoredProcedure,
                                                                 reader => SQLDataHelper.GetString(reader, "Email"), new SqlParameter("@CustomerID", custId));
            return result;
        }

        public static int DeleteCustomer(Guid customerId)
        {
            var customerEmail = GetCustomerEmailById(customerId);
            SubscriptionService.Unsubscribe(customerEmail);

            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_DeleteCustomer]", CommandType.StoredProcedure, new SqlParameter("@CustomerID", customerId));
            return 0;
        }

        public static int DeleteContact(Guid contactId)
        {
            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_DeleteCustomerContact]", CommandType.StoredProcedure,
                                          new SqlParameter { ParameterName = "@ContactID", Value = contactId });
            return 0;
        }

        public static int GetCustomerGroupId(Guid customerId)
        {
            return
                SQLDataHelper.GetInt(
                    SQLDataAccess.ExecuteScalar(
                        "SELECT [CustomerGroupId] FROM [Customers].[Customer] WHERE [CustomerID] = @CustomerID",
                        CommandType.Text, new SqlParameter { ParameterName = "@CustomerID", Value = customerId }),
                    CustomerGroupService.DefaultCustomerGroup);
        }

        public static Customer GetCustomer(Guid customerId)
        {
            return
                SQLDataAccess.ExecuteReadOne<Customer>(
                    "SELECT * FROM [Customers].[Customer] WHERE [CustomerID] = @CustomerID",
                    CommandType.Text,
                    Customer.GetFromSqlDataReader,
                    new SqlParameter { ParameterName = "@CustomerID", Value = customerId });
        }

        public static List<Customer> GetCustomersbyRole(Role role)
        {
            return SQLDataAccess.ExecuteReadList<Customer>("SELECT * FROM [Customers].[Customer] WHERE [CustomerRole] = @CustomerRole",
                                                           CommandType.Text,
                                                           Customer.GetFromSqlDataReader,
                                                           new SqlParameter("@CustomerRole", (int)role));
        }

        public static List<Customer> GetCustomersForAutocomplete(string query)
        {
            return SQLDataAccess.ExecuteReadList<Customer>("SELECT * FROM [Customers].[Customer] WHERE [FirstName] like '%' + @q + '%'" +
                                                                                                 "OR  [LastName] like '%' + @q + '%'" +
                                                                                                 "OR [Phone] like '%' + @q + '%'" +
                                                                                                 "OR [Email] like '%' + @q + '%'",
                                                           CommandType.Text,
                                                           Customer.GetFromSqlDataReader,
                                                           new SqlParameter("@q", query));
        }

        public static bool ExistsCustomer(Guid customerId)
        {
            return SQLDataAccess.ExecuteScalar<int>
                ("SELECT COUNT([CustomerID]) FROM [Customers].[Customer] WHERE [CustomerID] = @CustomerID",
                    CommandType.Text,
                    new SqlParameter { ParameterName = "@CustomerID", Value = customerId }) > 0;
        }

        public static Customer GetCustomerByEmail(string email)
        {
            return SQLDataAccess.ExecuteReadOne<Customer>(
                "[Customers].[sp_GetCustomerByEmail]", CommandType.StoredProcedure,
                Customer.GetFromSqlDataReader, new SqlParameter { ParameterName = "@Email", Value = email });
        }

        public static Customer GetCustomerByRole(Role role)
        {
            return SQLDataAccess.ExecuteReadOne<Customer>(
                "Select top(1) * from Customers.Customer where CustomerRole=@CustomerRole", CommandType.Text,
                Customer.GetFromSqlDataReader, new SqlParameter { ParameterName = "@CustomerRole", Value = role });
        }


        public static Customer GetCustomerByOpenAuthIdentifier(string identifier)
        {
            return SQLDataAccess.ExecuteReadOne<Customer>(
                "[Customers].[sp_GetCustomerByOpenAuthIdentifier]", CommandType.StoredProcedure,
                Customer.GetFromSqlDataReader, new SqlParameter { ParameterName = "@Identifier", Value = identifier });
        }

        public static CustomerContact GetContactFromSqlDataReader(SqlDataReader reader)
        {
            var contact = new CustomerContact
            {
                CustomerContactID = SQLDataHelper.GetGuid(reader, "ContactID"),
                Address = SQLDataHelper.GetString(reader, "Address"),
                City = SQLDataHelper.GetString(reader, "City"),
                Country = SQLDataHelper.GetString(reader, "Country"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Zip = SQLDataHelper.GetString(reader, "Zip"),
                RegionName = SQLDataHelper.GetString(reader, "Zone"),
                CountryId = SQLDataHelper.GetInt(reader, "CountryID"),
                RegionId = SQLDataHelper.GetNullableInt(reader, "RegionID"),
                CustomerGuid = SQLDataHelper.GetGuid(reader, "CustomerID")
            };

            return contact;
        }

        public static CustomerContact GetCustomerContact(string contactId)
        {
            var contact = SQLDataAccess.ExecuteReadOne<CustomerContact>(
                "SELECT * FROM [Customers].[Contact] WHERE [ContactID] = @id",
                CommandType.Text,
                GetContactFromSqlDataReader,
                new SqlParameter { ParameterName = "@id", Value = contactId });

            return contact;
        }

        public static List<CustomerContact> GetCustomerContacts(Guid customerId)
        {
            return SQLDataAccess.ExecuteReadList<CustomerContact>(
                "[Customers].[sp_GetCustomerContact]",
                CommandType.StoredProcedure, GetContactFromSqlDataReader,
                new SqlParameter { ParameterName = "@CustomerID", Value = customerId });
        }

        public static IList<Customer> GetCustomers()
        {
            return SQLDataAccess.ExecuteReadList<Customer>("SELECT * FROM [Customers].[Customer]", CommandType.Text, Customer.GetFromSqlDataReader);
        }

        public static List<string> GetCustomersEmails()
        {
            return SQLDataAccess.ExecuteReadColumn<string>("SELECT Email FROM [Customers].[Customer]", CommandType.Text, "Email");
        }

        public static Guid AddContact(CustomerContact contact, Guid customerId)
        {
            var id = SQLDataAccess.ExecuteScalar("[Customers].[sp_AddCustomerContact]",
                CommandType.StoredProcedure,
                new SqlParameter("@CustomerID", customerId),
                new SqlParameter("@Name", contact.Name),
                new SqlParameter("@Country", contact.Country),
                new SqlParameter("@City", contact.City),
                new SqlParameter("@Zone", contact.RegionName),
                new SqlParameter("@Address", contact.Address),
                new SqlParameter("@Zip", contact.Zip),
                new SqlParameter("@CountryID", contact.CountryId),
                new SqlParameter("@RegionID",
                    contact.RegionId.HasValue && contact.RegionId > 0 ? contact.RegionId : (object)DBNull.Value));
            return SQLDataHelper.GetGuid(id);
        }

        public static void UpdateContact(CustomerContact contact)
        {
            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_UpdateCustomerContact]", CommandType.StoredProcedure,
                new SqlParameter("@ContactID", contact.CustomerContactID),
                new SqlParameter("@Name", contact.Name),
                new SqlParameter("@Country", contact.Country),
                new SqlParameter("@City", contact.City),
                new SqlParameter("@Zone", contact.RegionName),
                new SqlParameter("@Address", contact.Address),
                new SqlParameter("@Zip", contact.Zip),
                new SqlParameter("@CountryID", contact.CountryId),
                new SqlParameter("@RegionID",
                    contact.RegionId.HasValue && contact.RegionId > 0 ? contact.RegionId : (object)DBNull.Value));
        }

        public static bool UpdateCustomer(Customer customer)
        {
            if (customer == null || customer.EMail.IsNullOrEmpty())
                return false;

            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_UpdateCustomerInfo]", CommandType.StoredProcedure,
                new SqlParameter("@CustomerID", customer.Id),
                new SqlParameter("@FirstName", customer.FirstName ?? string.Empty),
                new SqlParameter("@LastName", customer.LastName ?? string.Empty),
                new SqlParameter("@Phone", customer.Phone ?? string.Empty),
                new SqlParameter("@Email", customer.EMail),
                new SqlParameter("@CustomerGroupId",
                    customer.CustomerGroupId == 0 ? (object)DBNull.Value : customer.CustomerGroupId),
                new SqlParameter("@CustomerRole", customer.CustomerRole),
                new SqlParameter("@BonusCardNumber", customer.BonusCardNumber ?? (object)DBNull.Value));

            if (SubscriptionService.IsSubscribe(customer.EMail) != customer.SubscribedForNews)
            {

                if (customer.SubscribedForNews)
                {
                    SubscriptionService.Subscribe(customer.EMail);
                }
                else
                {
                    SubscriptionService.Unsubscribe(customer.EMail);
                }
            }

            return true;
        }

        public static int UpdateCustomerEmail(Guid id, string email)
        {
            SQLDataAccess.ExecuteNonQuery("Update Customers.Customer Set Email = @Email Where CustomerID = @CustomerID",
                                            CommandType.Text, new SqlParameter("@CustomerID", id), new SqlParameter("@Email", email));
            return 0;
        }

        public static Customer GetCustomerByEmailAndPassword(string email, string password, bool isHash)
        {
            return SQLDataAccess.ExecuteReadOne<Customer>("[Customers].[sp_GetCustomerByEmailAndPassword]",
                                                                       CommandType.StoredProcedure, Customer.GetFromSqlDataReader,
                                                                       new SqlParameter { ParameterName = "@Email", Value = email },
                                                                       new SqlParameter { ParameterName = "@Password", Value = isHash ? password : SecurityHelper.GetPasswordHash(password) });
        }

        public static string ConvertToLinedAddress(CustomerContact cc)
        {
            string address = string.Empty;

            if (!String.IsNullOrEmpty(cc.Country.Trim()))
            {
                address += cc.Country + ", ";
            }

            if (cc.RegionName.Trim() != "-")
            {
                address += cc.RegionName + ", ";
            }

            if (!String.IsNullOrEmpty(cc.City.Trim()))
            {
                address += cc.City + ", ";
            }

            if (cc.Zip.Trim() != "-")
            {
                address += cc.Zip + ", ";
            }

            if (!String.IsNullOrEmpty(cc.Address.Trim()))
            {
                address += cc.Address + ", ";
            }

            return address;
        }

        public static bool ExistsEmail(string strUserEmail)
        {
            if (string.IsNullOrEmpty(strUserEmail))
            {
                return false;
            }

            bool boolRes = SQLDataAccess.ExecuteScalar<int>("SELECT COUNT(CustomerID) FROM [Customers].[Customer] WHERE [Email] = @Email;", CommandType.Text, new SqlParameter("@Email", strUserEmail)) > 0;

            return boolRes;
        }

        public static void ChangePassword(Guid customerId, string strNewPassword, bool isPassHashed)
        {
            SQLDataAccess.ExecuteNonQuery("[Customers].[sp_ChangePassword]", CommandType.StoredProcedure,
                                                new SqlParameter { ParameterName = "@CustomerID", Value = customerId },
                                                new SqlParameter { ParameterName = "@Password", Value = isPassHashed ? strNewPassword : SecurityHelper.GetPasswordHash(strNewPassword) }
                                                );
        }

        public static Guid InsertNewCustomer(Customer customer)
        {
            if (CheckCustomerExist(customer.EMail))
            {
                return Guid.Empty;
            }
            var temp = SQLDataAccess.ExecuteScalar("[Customers].[sp_AddCustomer]", CommandType.StoredProcedure,
                                                new SqlParameter("@CustomerGroupID", customer.CustomerGroupId),
                                                new SqlParameter("@Password", SecurityHelper.GetPasswordHash(customer.Password)),
                                                new SqlParameter("@FirstName", customer.FirstName ?? string.Empty),
                                                new SqlParameter("@LastName", customer.LastName ?? string.Empty),
                                                new SqlParameter("@Patronymic", customer.Patronymic ?? string.Empty),
                                                new SqlParameter("@Phone", string.IsNullOrEmpty(customer.Phone) ? (object)DBNull.Value : customer.Phone),
                                                new SqlParameter("@RegistrationDateTime", DateTime.Now),
                                                new SqlParameter("@Email", customer.EMail),
                                                new SqlParameter("@CustomerRole", customer.CustomerRole),
                                                new SqlParameter("@BonusCardNumber", customer.BonusCardNumber ?? (object)DBNull.Value)
                                                ).ToString();

            if (customer.SubscribedForNews)
            {
                SubscriptionService.Subscribe(customer.EMail);
            }

            customer.Id = new Guid(temp);
            return customer.Id;
        }

        public static string GetContactId(CustomerContact contact)
        {
            var res = SQLDataHelper.GetNullableGuid(SQLDataAccess.ExecuteScalar("[Customers].[sp_GetContactIDByContent]", CommandType.StoredProcedure,
                                         new SqlParameter("@Name", contact.Name),
                                         new SqlParameter("@Country", contact.Country),
                                         new SqlParameter("@City", contact.City),
                                         new SqlParameter("@Zone", contact.RegionName ?? ""),
                                         new SqlParameter("@Zip", contact.Zip ?? ""),
                                         new SqlParameter("@Address", contact.Address),
                                         new SqlParameter("@CustomerID", contact.CustomerGuid)
                                         ));
            return res == null ? null : res.ToString();
        }

        public static bool CheckCustomerExist(string email)
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT COUNT([CustomerID]) FROM [Customers].[Customer] WHERE [Email] = @Email",
                CommandType.Text, new SqlParameter("@Email", email)) != 0;
        }

        public static bool AddOpenIdLinkCustomer(Guid customerGuid, string identifier)
        {
            return SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                @"Insert Into [Customers].[OpenIdLinkCustomer] (CustomerID, OpenIdIdentifier) Values (@CustomerID, @OpenIdIdentifier)",
                CommandType.Text,
                new SqlParameter("@CustomerID", customerGuid),
                new SqlParameter("@OpenIdIdentifier", identifier))) != 0;
        }

        public static bool IsExistOpenIdLinkCustomer(string identifier)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                @"SELECT COUNT([CustomerID]) FROM [Customers].[OpenIdLinkCustomer] WHERE [OpenIdIdentifier] = @OpenIdIdentifier",
                CommandType.Text,
                new SqlParameter("@OpenIdIdentifier", identifier)) != 0;
        }

        public static void ChangeCustomerGroup(string customerId, int customerGroupId)
        {
            SQLDataAccess.ExecuteNonQuery("Update [Customers].[Customer] Set CustomerGroupId = @CustomerGroupId WHERE CustomerID = @CustomerID",
                                            CommandType.Text, new SqlParameter("@CustomerID", customerId), new SqlParameter("@CustomerGroupId", customerGroupId));
        }
    }
}