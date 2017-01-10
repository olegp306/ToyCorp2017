//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AdvantShop.Helpers;

namespace AdvantShop.Customers
{
    [Serializable]
    public class Customer
    {
        public Customer()
        {
            Id = Guid.Empty;
            EMail = string.Empty;
            Password = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Patronymic = string.Empty;
            Phone = string.Empty;
            CustomerGroupId = CustomerGroupService.DefaultCustomerGroup;
        }

        public static Customer GetFromSqlDataReader(SqlDataReader reader)
        {
            var customer = new Customer
            {
                Id = SQLDataHelper.GetGuid(reader, "CustomerID"),
                CustomerGroupId = SQLDataHelper.GetInt(reader, "CustomerGroupId", 0),
                EMail = SQLDataHelper.GetString(reader, "EMail"),
                FirstName = SQLDataHelper.GetString(reader, "FirstName"),
                LastName = SQLDataHelper.GetString(reader, "LastName"),
                Patronymic = SQLDataHelper.GetString(reader, "Patronymic"),
                RegistrationDateTime = SQLDataHelper.GetDateTime(reader, "RegistrationDateTime"),
                Phone = SQLDataHelper.GetString(reader, "Phone"),
                Password = SQLDataHelper.GetString(reader, "Password"),
                CustomerRole = (Role)SQLDataHelper.GetInt(reader, "CustomerRole"),
                BonusCardNumber = SQLDataHelper.GetNullableLong(reader, "BonusCardNumber")
            };

            return customer;
        }

        public Guid Id { get; set; }

        public int CustomerGroupId { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }        

        public string Phone { get; set; }

        public DateTime RegistrationDateTime { get; set; }
        
        public string EMail { get; set; }

        public bool SubscribedForNews { get; set; }

        public long? BonusCardNumber { get; set; }

        public Role CustomerRole { get; set; }

        private CustomerGroup _customerGroup;
        public CustomerGroup CustomerGroup
        {
            get { return _customerGroup ?? (_customerGroup = CustomerGroupService.GetCustomerGroup(CustomerGroupId)); }
        }

        private List<CustomerContact> _contacts;
        public List<CustomerContact> Contacts
        {
            get { return _contacts ?? (_contacts = CustomerService.GetCustomerContacts(Id)); }
        }

        public void ReLoadContacts()
        {
            _contacts = CustomerService.GetCustomerContacts(Id);
        }

        public bool IsAdmin 
        { 
            get { return CustomerRole == Role.Administrator; } 
        }

        public bool IsModerator
        {
            get { return CustomerRole == Role.Moderator; }
        }

        public bool HasRoleAction(RoleActionKey key)
        {
            return RoleActionService.HasCustomerRoleAction(Id, key);
        }

        public bool RegistredUser 
        { 
            get { return EMail != string.Empty; } 
        }

        public bool IsVirtual { get; set; }
    }
}