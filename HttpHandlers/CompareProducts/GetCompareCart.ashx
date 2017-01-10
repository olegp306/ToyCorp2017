<%@ WebHandler Language="C#" Class="GetCompareCart" %>

using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using Newtonsoft.Json;

public class GetCompareCart : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        var compare = from item in ShoppingCartService.CurrentCompare
                      select new
                                 {
                                     item.Offer.Product.ID,
                                     item.Offer.Product.Name,
                                     Link = UrlService.GetLink(ParamType.Product, item.Offer.Product.UrlPath, item.Offer.Product.ID)
                                 };
        
        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(compare));
    }

    public bool IsReusable
    {
        get { return true;}
    }
}