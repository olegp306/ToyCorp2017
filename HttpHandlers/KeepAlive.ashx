<%@ WebHandler Language="C#" Class="KeepAlive" %>

using System.Web;

public class KeepAlive : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        context.Response.Write("Hello from AdvantShop");
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}