<%@ WebHandler Language="C#" Class="SaveCustomerData" %>

using System.Web;
using System.Web.SessionState;
using AdvantShop.Customers;
using AdvantShop.Orders;

public class SaveCustomerData : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        context.Response.ContentType = "application/json";

        var pageData = OrderConfirmationService.Get(CustomerContext.CurrentCustomer.Id);
        if (pageData == null)
        {
            return;
        }

        pageData.OrderConfirmationData.Customer.EMail = context.Request["email"];
        pageData.OrderConfirmationData.Customer.FirstName = context.Request["firstname"];
        pageData.OrderConfirmationData.Customer.LastName = context.Request["lastname"];
        pageData.OrderConfirmationData.Customer.Patronymic = context.Request["patronymic"];
        pageData.OrderConfirmationData.Customer.Phone = context.Request["phone"];
       
        OrderConfirmationService.Update(pageData);
     
    }


    public bool IsReusable
    {
        get { return false; }
    }
}
