//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using System.Web.Security;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;

namespace AdvantShop.Security
{
    public class AuthorizeService
    {
        private const string Spliter = ":";
        public static Customer GetAuthenticatedCustomer()
        {
            if (HttpContext.Current == null) return null;
            try
            {
                var formsCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (formsCookie != null)
                {
                    var formsAuthenticationTicket = FormsAuthentication.Decrypt(formsCookie.Value);
                    if (formsAuthenticationTicket != null)
                    {
                        var token = formsAuthenticationTicket.Name;
                        var words = token.Split(new[] {Spliter}, StringSplitOptions.RemoveEmptyEntries);
                        if (words.Length != 2) return null;
                        var email = words[0];
                        var passHash = words[1];
                        return string.IsNullOrEmpty(email)
                                   ? null
                                   : CustomerService.GetCustomerByEmailAndPassword(email, passHash, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, false);
            }
            return null;
        }

        public static bool SignIn(string email, string password, bool isHash, bool createPersistentCookie)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return false;

            CustomerContext.IsDebug = Secure.IsDebugAccount(email, password);

            if (CustomerContext.IsDebug)
            {
                Secure.AddUserLog("sa", true, true);
                return true;
            }

            var oldCustomerId = CustomerContext.CurrentCustomer.Id;
            var customer = CustomerService.GetCustomerByEmailAndPassword(email, password, isHash);
            if (customer == null) return false;

            Secure.AddUserLog(customer.EMail, true, customer.IsAdmin);
            ShoppingCartService.MergeShoppingCarts(oldCustomerId, customer.Id);
            CustomerContext.SetCustomerCookie(customer.Id);
            FormsAuthentication.SetAuthCookie(email + Spliter + customer.Password, createPersistentCookie);
            return true;
        }

        public static void SignOut()
        {
            CustomerContext.DeleteCustomerCookie();
            FormsAuthentication.SignOut();
        }
    }
}