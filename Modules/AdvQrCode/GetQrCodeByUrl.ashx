<%@ WebHandler Language="C#" Class="GetQrCodeByUrl" %>



using System;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

public class GetQrCodeByUrl : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "image/png";

        if (string.IsNullOrEmpty(context.Request["url"]))
        {
            return;
        }

        var url = HttpUtility.UrlDecode(context.Request["url"]);
        QrCode qrCode;
        var encoder = new QrEncoder(ErrorCorrectionLevel.M);
        encoder.TryEncode(url.Split(new[] { "?" }, StringSplitOptions.None)[0], out qrCode);

        var renderer = new GraphicsRenderer(new FixedModuleSize(2, QuietZoneModules.Two), System.Drawing.Brushes.Black, System.Drawing.Brushes.White);
        using (var ms = new MemoryStream())
        {
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);

            context.Response.BinaryWrite(ms.ToArray());
            context.Response.Flush();
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}