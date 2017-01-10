<%@ WebHandler Language="C#" Class="_1C_Csv" %>

using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.Helpers;

public class _1C_Csv : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        var username = context.Request["username"];
        var password = context.Request["password"];
        var customer = CustomerService.GetCustomerByEmailAndPassword(username, password, false);
        if (customer != null)
        {
            context.Response.Write("login wrong");
            return;
        }

        if (context.Request.Files.Count == 1)
        {
            var pf = context.Request.Files[0];
            FileHelpers.CreateDirectory(context.Server.MapPath("~/1c_temp"));
            var zipfile = context.Server.MapPath("~/1c_temp/import.zip");
            var impDir = context.Server.MapPath("~/1c_temp/import/");
            FileHelpers.CreateDirectory(impDir);
            //FileHelpers.DeleteDirectory(impDir, false);
            pf.SaveAs(zipfile);
            FileHelpers.UnZipFile(zipfile, impDir);
            foreach (var file in Directory.GetFiles(impDir).Where(file => !file.EndsWith(".csv") && !file.EndsWith(".xml")))
            {
                File.Delete(file.Replace("1c_temp\\import", "upload_images"));
                File.Move(file, file.Replace("1c_temp\\import", "upload_images"));
            }

            var files = Directory.GetFiles(impDir).Where(x => x.EndsWith(".csv"));
            foreach (var file in files)
            {
                CsvImport.Factory(file, false).Process();
            }
        }
        else
        {
            context.Response.Write("missing file");
        }
        context.Response.Write("ok");
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}