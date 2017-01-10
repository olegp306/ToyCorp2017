<%@ WebHandler Language="C#" Class="GetProductPhotos" %>

using System;
using System.Web;
using Newtonsoft.Json;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Catalog;
using System.Linq;

public class GetProductPhotos : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        if (context.Request["productId"].IsNullOrEmpty())
        {
            context.Response.Write(JsonConvert.SerializeObject(null));
            return;
        }

        int productId = context.Request["productId"].TryParseInt();

        var objs = PhotoService.GetPhotos(productId, PhotoType.Product).Select(photo => new
        {
            PathXSmall = AdvantShop.FilePath.FoldersHelper.GetImageProductPath(AdvantShop.FilePath.ProductImageType.XSmall, photo.PhotoName, false),
            PathSmall = AdvantShop.FilePath.FoldersHelper.GetImageProductPath(AdvantShop.FilePath.ProductImageType.Small, photo.PhotoName, false),
            PathMiddle = AdvantShop.FilePath.FoldersHelper.GetImageProductPath(AdvantShop.FilePath.ProductImageType.Middle, photo.PhotoName, false),
            photo.ColorID,
            photo.PhotoId,
            photo.Description,
            SettingsPictureSize.XSmallProductImageHeight,
            SettingsPictureSize.XSmallProductImageWidth,
            SettingsPictureSize.SmallProductImageHeight,
            SettingsPictureSize.SmallProductImageWidth,
            SettingsPictureSize.MiddleProductImageWidth,
            SettingsPictureSize.MiddleProductImageHeight
        }).ToList();

        context.Response.Write(JsonConvert.SerializeObject(objs));

    }

    public bool IsReusable
    {
        get { return false; }
    }
}