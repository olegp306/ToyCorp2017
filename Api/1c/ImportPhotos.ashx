<%@ WebHandler Language="C#" Class="ImportPhotos" %>

using System;
using System.IO;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.OneC;
using Newtonsoft.Json;

public class ImportPhotos : IHttpHandler
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

        if (context.Request.Files.Count == 0)
        {
            context.Response.Write(JsonConvert.SerializeObject(new OneCResponse("error", "Missing .zip file")));
            return;
        }

        try
        {
            var oneCFolder = FoldersHelper.GetPathAbsolut(FolderType.OneCTemp);
            var zipfile = oneCFolder + "import.zip";
            var importDirectory = oneCFolder + "import/";

            FileHelpers.CreateDirectory(oneCFolder);
            FileHelpers.CreateDirectory(importDirectory);

            foreach (string fileName in context.Request.Files)
            {
                var postedFile = context.Request.Files[fileName];
                if (postedFile == null)
                    break;

                postedFile.SaveAs(zipfile);
                FileHelpers.UnZipFile(zipfile, importDirectory);

                foreach (var file in Directory.GetFiles(importDirectory))
                {
                    var fileUpload = file.Replace("1c_temp/import", "upload_images");

                    File.Delete(fileUpload);
                    File.Move(file, fileUpload);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            context.Response.Write(JsonConvert.SerializeObject(new OneCResponse("error", "Error: " + ex.Message)));
            return;
        }

        context.Response.Write(JsonConvert.SerializeObject(new OneCResponse(){status = "ok"}));
    }

    public bool IsReusable
    {
        get { return false; }
    }
}