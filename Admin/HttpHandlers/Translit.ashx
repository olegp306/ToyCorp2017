<%@ WebHandler Language="C#" Class="Translit" %>

using System.Web;
using AdvantShop.Helpers;
using Newtonsoft.Json;

public class Translit : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var temp = context.Request["source"];
        var tempNew = StringHelper.TransformUrl(StringHelper.Translit(temp));
        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(tempNew));
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