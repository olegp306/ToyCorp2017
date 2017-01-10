//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Orders;

namespace AdvantShop.Customers
{
    [Serializable]
    public class CustomerContact
    {
        public Guid CustomerContactID { get; set; }

        public Guid CustomerGuid { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public int? RegionId { get; set; }

        public string RegionName { get; set; }

        public string Address { get; set; }

        public string Zip { get; set; }

        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        
        public CustomerContact()
        {
            CustomerContactID = Guid.Empty;
            CustomerGuid = Guid.Empty;
            Name = string.Empty;
            Country = string.Empty;
            RegionName = string.Empty;
            City = string.Empty;
            Address = string.Empty;
            Zip = string.Empty;
            CustomField1 = string.Empty;
            CustomField2 = string.Empty;
            CustomField3 = string.Empty;
        }

        public static explicit operator OrderContact(CustomerContact contact)
        {
            return new OrderContact
            {
                Name = contact.Name,
                Country = contact.Country,
                Zone = contact.RegionName.IsNullOrEmpty() ? null : contact.RegionName,
                City = contact.City,
                Zip = contact.Zip.IsNullOrEmpty() ? null : contact.Zip,
                Address = contact.Address,
                CustomField1 = contact.CustomField1,
                CustomField2 = contact.CustomField2,
                CustomField3 = contact.CustomField3
            };
        }
    }
}