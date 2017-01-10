<%@ WebHandler Language="C#" Class="ImportProducts" %>

using System;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.OneC;
using AdvantShop.Statistic;
using Newtonsoft.Json;
using System.IO;

public class ImportProducts : IHttpHandler
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

        if (context.Request.Files.Count == 0 || (context.Request.Files[0] != null && !context.Request.Files[0].FileName.EndsWith(".csv")))
        {
            context.Response.Write(JsonConvert.SerializeObject(new OneCResponse("error", "Missing .csv file")));
            return;
        }

        try
        {
            var oneCFolder = FoldersHelper.GetPathAbsolut(FolderType.OneCTemp);
            var importDirectory = oneCFolder + "import/";

            FileHelpers.CreateDirectory(oneCFolder);
            FileHelpers.CreateDirectory(importDirectory);

            foreach (string fileName in context.Request.Files)
            {
                var postedFile = context.Request.Files[fileName];
                if (postedFile == null)
                    break;

                var importCsvFile = importDirectory + postedFile.FileName;

                postedFile.SaveAs(importCsvFile);

                CommonStatistic.Init();
                CommonStatistic.CurrentProcess = "1c import products";
                CommonStatistic.CurrentProcessName = "1c import products";

                if (File.Exists(CommonStatistic.FileLog))
                    File.WriteAllText(CommonStatistic.FileLog, String.Empty);
                
                CommonStatistic.IsRun = true;
                
                CsvImport.Factory(importCsvFile, false, false, ";", "UTF-8", null, "&&", ":", true).Process(false);
                
                CommonStatistic.IsRun = false;

                context.Response.Write(JsonConvert.SerializeObject(new OneCResponse() { status = "ok", errors = CommonStatistic.ReadLog() }));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            
            context.Response.Write(JsonConvert.SerializeObject(new OneCResponse("error", "Error: " + ex.Message)));
            return;
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}