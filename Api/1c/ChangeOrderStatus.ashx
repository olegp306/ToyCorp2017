<%@ WebHandler Language="C#" Class="ChangeOrderStatus" %>

using System;
using System.IO;
using System.Text;
using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.OneC;
using AdvantShop.Orders;
using CsvHelper;
using Newtonsoft.Json;

public class ChangeOrderStatus : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        
        var apikey = context.Request["apikey"];

        if (!Settings1C.Enabled || string.IsNullOrWhiteSpace(apikey) || string.IsNullOrWhiteSpace(SettingsApi.ApiKey) ||
            apikey != SettingsApi.ApiKey)
        {
            context.Response.Write(JsonConvert.SerializeObject(new OneCResponse("error", "Check apikey")));
            return;
        }

        var packageid = context.Request["packageid"];
        if (string.IsNullOrWhiteSpace(packageid))
        {
            context.Response.Write(JsonConvert.SerializeObject(new OneCResponse("error", "Wrong packageid")));
            return;
        }
        
        try
        {
            var oneCFolder = FoldersHelper.GetPathAbsolut(FolderType.OneCTemp);
            FileHelpers.CreateDirectory(oneCFolder);

            var errors = "";
            var warnings = "";

            if (context.Request.Files.Count == 0)
            {
                context.Response.Write(JsonConvert.SerializeObject(new OneCResponse("error", "Missing .csv file") { packageid = packageid }));
                return;
            }

            foreach (string fileName in context.Request.Files)
            {
                var postedFile = context.Request.Files[fileName];
                if (postedFile == null)
                    break;

                if (Path.GetExtension(postedFile.FileName) != ".csv")
                    break;

                var csvFilePath = oneCFolder + postedFile.FileName;

                postedFile.SaveAs(csvFilePath);

                using (var reader = new CsvReader(new StreamReader(csvFilePath, Encoding.GetEncoding("UTF-8"))))
                {
                    reader.Configuration.Delimiter = ";";
                    reader.Configuration.HasHeaderRecord = true;

                    int i = 1;
                    
                    while (reader.Read())
                    {
                        try
                        {
                            var orderIdString = reader[0];
                            
                            var orderStatus1C = new OrderStatus1C()
                            {
                                OrderId = orderIdString.TryParseInt(),
                                Status1C = reader[1],
                                OrderId1C = reader[2],
                                OrderDate = reader[3]
                            };

                            var order = OrderService.GetOrder(orderStatus1C.OrderId);
                            if (order != null)
                            {
                                OrderService.AddOrUpdateOrderStatus1C(orderStatus1C);

                                if (string.IsNullOrWhiteSpace(orderStatus1C.Status1C))
                                {
                                    warnings += string.Format("Статус заказа #{0} пустая строка; ", orderIdString);
                                }

                                if (orderStatus1C.Status1C.Trim().ToLower() == "удален")
                                {
                                    OrderService.ChangeOrderStatus(orderStatus1C.OrderId, OrderService.CanceledOrderStatus);
                                }
                            }
                            else
                            {
                                errors += string.Format("Заказ #{0} не найден; ", orderIdString);
                            }
                        }
                        catch (Exception ex)
                        {
                            errors += string.Format("Ошибка в {0} строке {1}; ", i, ex.Message);
                            Debug.LogError(ex);
                        }

                        i++;
                    }
                }
            }

            context.Response.Write(
                JsonConvert.SerializeObject(new OneCResponse()
                {
                    packageid = packageid,
                    status = "ok",
                    errors = errors,
                    warnings = warnings
                }));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            
            context.Response.Write(
                JsonConvert.SerializeObject(new OneCResponse() {packageid = packageid, status = "error", errors = "Error: " + ex.Message}));
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}