<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.AdminSearch" %>

using System;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Statistic;
using Newtonsoft.Json;

namespace Admin.HttpHandlers
{
    public class AdminSearch : AdminHandler, IHttpHandler
    {

        public enum eAdminSearch
        {
            Product = 0,
            Order = 1,
            Customer = 2
        }

        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            context.Response.Cache.SetLastModified(DateTime.UtcNow);

            string type = context.Request["type"].ToString();
            string q = context.Request["q"].ToString();
            string result = string.Empty;

            eAdminSearch searchType;

            bool resultParse = Enum.TryParse(type, true, out searchType);

            if (resultParse == true)
            {
                switch (searchType)
                {
                    case eAdminSearch.Product:
                        result = GetProducts(q);
                        break;
                    case eAdminSearch.Order:
                        result = GetOrders(q);
                        break;
                    case eAdminSearch.Customer:
                        result = GetCustomers(q);
                        break;
                }
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(result);
            context.Response.End();
        }

        private string GetProducts(string q)
        {
            string result = string.Empty;

            var productIds = AdvantShop.FullSearch.LuceneSearch.Search(q).AggregateString('/');

            var productNames = ProductService.GetForAutoCompleteByIdsInAdmin(productIds);

            if (productNames.Count != 0)
            {
                productNames = productNames.Distinct().Take(10).ToList();

                for (int i = 0; i < productNames.Count; i++)
                {
                    result += (productNames[i] + "\n");
                }
            }

            return result;
        }

        private string GetOrders(string q)
        {

            var result = new StringBuilder();
            var orders = AdvantShop.Orders.OrderService.GetOrdersForAutocomplete(q);

            for (int i = 0; i < orders.Count; i++)
            {
                result.AppendFormat("<a href=\"vieworder.aspx?orderid={0}\">", orders[i].OrderID);
                
                result.Append("№" + orders[i].OrderID + " - ");

                if (orders[i].LastName.IsNotEmpty())
                {
                    result.Append(" " + orders[i].LastName);
                }
                
                if (orders[i].FirstName.IsNotEmpty())
                {
                    result.Append(" " + orders[i].FirstName);
                }

                if (orders[i].Email.IsNotEmpty())
                {
                    result.Append(", " + orders[i].Email);
                }

                if (orders[i].MobilePhone.IsNotEmpty())
                {
                    result.Append(", " + orders[i].MobilePhone);
                }
                
                result.Append("</a>");
                result.Append( "\n");
            }

            return result.ToString();
        }

        private string GetCustomers(string q)
        {
            var result = new StringBuilder();

            var customers = AdvantShop.Customers.CustomerService.GetCustomersForAutocomplete(q);

            for (int i = 0; i < customers.Count; i++)
            {
                result.AppendFormat("<a href=\"ViewCustomer.aspx?CustomerID={0}\">", customers[i].Id);

                if (customers[i].LastName.IsNotEmpty())
                {
                    result.Append(customers[i].LastName + " ");
                }

                if (customers[i].FirstName.IsNotEmpty())
                {
                    result.Append(customers[i].FirstName);
                }

                result.Append("</a>");
                result.Append("\n");
            }
            
            return result.ToString();
        }

    }
}