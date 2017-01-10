<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.ExportOrderExcel" %>

using System;
using System.Web;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport.Excel;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace Admin.HttpHandlers
{
    public class ExportOrderExcel : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            var order = OrderService.GetOrder(SQLDataHelper.GetInt(context.Request["OrderID"]));
            if (order != null)
            {
                try
                {
                    string strPath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
                    FileHelpers.CreateDirectory(strPath);

                    var filename = String.Format("Order{0}.xls", order.Number);
                    var wrt = new ExcelSingleOrderWriter();
                    wrt.Generate(strPath + filename, order);
                    
                    CommonHelper.WriteResponseXls(strPath + filename, filename);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("Error on creating xls document");
                }
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Error on creating xls document");
            }
        }
    }
}