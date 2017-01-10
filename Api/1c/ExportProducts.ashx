<%@ WebHandler Language="C#" Class="ExportProducts" %>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.OneC;
using Newtonsoft.Json;

public class ExportProducts : IHttpHandler
{
    private const string StrFileName = "products";
    private const string StrFileExt = ".csv";

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

        var productFields = new List<ProductFields>()
        {
            ProductFields.Sku,
            ProductFields.Name,
            ProductFields.ParamSynonym,
            ProductFields.Category,
            ProductFields.Sorting,
            ProductFields.Enabled,
            ProductFields.Price,
            ProductFields.PurchasePrice,
            ProductFields.Amount,
            ProductFields.MultiOffer,
            ProductFields.Unit,
            ProductFields.Discount,
            ProductFields.ShippingPrice,
            ProductFields.Weight,
            ProductFields.Size,
            ProductFields.BriefDescription,
            ProductFields.Description,
            ProductFields.Title,
            ProductFields.MetaKeywords,
            ProductFields.MetaDescription,
            ProductFields.H1,
            ProductFields.Photos,
            ProductFields.Videos,
            ProductFields.Markers,
            ProductFields.Properties,
            ProductFields.Producer,
            ProductFields.OrderByRequest,
            ProductFields.SalesNotes,
            ProductFields.Related,
            ProductFields.Alternative,
            ProductFields.CustomOption,
            ProductFields.Gtin,
            ProductFields.GoogleProductCategory,
            ProductFields.Adult
        };

        try
        {
            var strFilePath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(strFilePath);

            foreach (var file in Directory.GetFiles(strFilePath).Where(f => f.Contains(StrFileName)).ToList())
            {
                FileHelpers.DeleteFile(file);
            }

            var extStrFileName = (StrFileName + StrFileExt).FileNamePlusDate();
            var strFullPath = strFilePath + extStrFileName;

            var filterModel = new ProductCsvFilterModel()
            {
                ModuleName = "CsvExport1C",
                AllProducts = true,
                ExportNoInCategory = true
            };

            DateTime from;
            DateTime to;
            if (!String.IsNullOrWhiteSpace(context.Request["from"]) && !String.IsNullOrWhiteSpace(context.Request["to"]))
            {
                if (DateTime.TryParseExact(context.Request["from"], "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out from) &&
                    DateTime.TryParseExact(context.Request["to"], "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out to))
                {
                    filterModel.From = from;
                    filterModel.To = to.AddDays(1);
                }
            }

            CsvExport.Factory(strFullPath, 
                                CsvSettings.CsvEnconing, ";", "&&", ":", productFields, true)
                                .Process(false, filterModel);

            CommonHelper.WriteResponseFile(strFullPath, extStrFileName);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            context.Response.Write(JsonConvert.SerializeObject(new OneCResponse("error", "Error: " + ex.Message)));
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}