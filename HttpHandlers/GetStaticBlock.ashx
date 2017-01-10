<%@ WebHandler Language="C#" Class="GetStaticBlock" %>

using System.Web;
using AdvantShop.CMS;
using AdvantShop;

public class GetStaticBlock : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string key = context.Request["Key"];
        if(key.IsNotEmpty())
        {
            var block = StaticBlockService.GetPagePartByKey(key);
            if (block != null)
            {
                context.Response.Write(block.Content);
            }
        }
    }
    
    public bool IsReusable
    {
        get { return true;}
    }
}