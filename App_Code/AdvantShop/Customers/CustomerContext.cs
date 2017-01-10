//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Helpers;
using AdvantShop.Security;

namespace AdvantShop.Customers
{
    public static class CustomerContext
    {
        private const string CustomerContextKey = "CustomerContext";

        private static string CustomerCookieName
        {
            get { return string.Format("{0}_customer", HttpUtility.UrlEncode(SettingsMain.SiteUrl)); }
        }

        private static Customer GetCurrentCustomer()
        {
            Customer customer = null;
            if (HttpContext.Current != null)
            {
                var cachedCustomer = HttpContext.Current.Items[CustomerContextKey] as Customer;
                if (cachedCustomer != null) return cachedCustomer;

                if (IsDebug)
                {
                    customer = new Customer
                               {
                                   CustomerRole = Role.Administrator,
                                   IsVirtual = true
                               };
                }

                //registered user
                if (customer == null)
                {
                    customer = AuthorizeService.GetAuthenticatedCustomer();
                }

                //load guest customer
                if (customer == null)
                {
                    var customerCookie = CommonHelper.GetCookie(CustomerCookieName);
                    if (customerCookie != null && !String.IsNullOrEmpty(customerCookie.Value))
                    {
                        Guid customerGuid;
                        if (Guid.TryParse(customerCookie.Value, out customerGuid) &&
                            !CustomerService.ExistsCustomer(customerGuid))
                        {
                            customer = new Customer {Id = customerGuid, CustomerRole = Role.Guest};
                        }
                    }
                }

                //create guest if not exists
                if (customer == null)
                {
                    var customerId = Guid.NewGuid();
                    while (CustomerService.ExistsCustomer(customerId))
                        customerId = Guid.NewGuid();

                    customer = new Customer { Id = customerId, CustomerRole = Role.Guest };
                    SetCustomerCookie(customer.Id);
                }
                
                HttpContext.Current.Items[CustomerContextKey] = customer;
            }
            return customer;
        }

        public static bool IsDebug
        {
            get
            {
                return HttpContext.Current.Session != null
                    && HttpContext.Current.Session["isDebug"] != null
                    && (bool)HttpContext.Current.Session["isDebug"];
            }
            set
            {
                if (HttpContext.Current.Session == null) return;
                HttpContext.Current.Session["isDebug"] = value;
            }
        }

        public static Customer CurrentCustomer
        {
            get { return GetCurrentCustomer(); }
        }

        public static Guid CustomerId
        {
            get { return CurrentCustomer.Id; }
        }

        public static void SetCustomerCookie(Guid userId)
        {
            CommonHelper.SetCookie(CustomerCookieName, userId.ToString(), new TimeSpan(90, 0, 0, 0), true);
        }

        public static void DeleteCustomerCookie()
        {
            CommonHelper.DeleteCookie(CustomerCookieName);
        }
    }
}