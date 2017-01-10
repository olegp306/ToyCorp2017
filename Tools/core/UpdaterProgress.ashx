<%@ WebHandler Language="C#" Class="Advantshop_Tools.UpdaterProgress" %>

using System.Web;

namespace Advantshop_Tools
{
    public class UpdaterProgress : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(Advantshop_Tools.UpdaterStatus.Status);

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
