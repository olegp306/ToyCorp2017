<%@ WebHandler Language="C#" Class="GetSearch" %>

using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.FullSearch;
using System.Linq;

public class GetSearch : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        
        if (string.IsNullOrWhiteSpace(context.Request["q"]))
        {
            context.Response.Write("\n");
            context.Response.End();
            return;
        }
        var productIds = LuceneSearch.Search(context.Request["q"]).AggregateString('/');

        var productNames = ProductService.GetForAutoCompleteByIds(productIds);
               
        if (productNames.Count == 0)
        {
            context.Response.Write("\n");
            context.Response.End();
            return;
        }

        productNames = productNames.Distinct().Take(10).ToList();

        for (int i = 0; i < productNames.Count; i++)
        {
            context.Response.Write(productNames[i] + "\n");
        }
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}