//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Web;
using System.Web.Services;
using Advantshop_Tools;

namespace Tools.core
{
    public partial class IsCustom : System.Web.UI.Page
    {
        private static readonly string ShopCodeMaskFile = HttpContext.Current.Server.MapPath("~/App_Data/shopCodeMaskFile.txt");
        private static readonly string ShopBaseMaskFile = HttpContext.Current.Server.MapPath("~/App_Data/shopBaseMaskFile.txt");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (File.Exists(ShopCodeMaskFile))
            {
                lnkFileCode.Text = @"code mask (" + new FileInfo(ShopCodeMaskFile).LastWriteTime + @")";
            }

            if (File.Exists(ShopBaseMaskFile))
            {
                lnkFileSql.Text = @"base mask (" + new FileInfo(ShopBaseMaskFile).LastWriteTime + @")";
            }
        }
    
        protected void btnCompareBase_OnClick(object sender, EventArgs e)
        {
            var report = UpdaterService.CompareBaseVersions(ckbUpdateMasks.Checked);

            ltrlReport.InnerHtml = !string.IsNullOrEmpty(report) ? report : "<span style=\"color:green;\">Versions of the code are the same</span>";

            if (File.Exists(ShopBaseMaskFile))
            {
                lnkFileSql.Text = @"base mask (" + new FileInfo(ShopBaseMaskFile).LastWriteTime + @")";
            }
        }

        protected void lnkFileSql_Click(object sender, EventArgs e)
        {
            //UpdaterService.CreateBaseMaskFile();
            Page.Response.Clear();
            Page.Response.AppendHeader("content-disposition", "attachment; filename=\"shopBaseMaskFile.txt\"");
            Page.Response.TransmitFile(ShopBaseMaskFile);
            Page.Response.Flush();
            Page.Response.End();
        }

        protected void lnkFileCode_Click(object sender, EventArgs e)
        {
            //UpdaterService.CreateCodeMaskFile();
            Page.Response.Clear();
            Page.Response.AppendHeader("content-disposition", "attachment; filename=\"shopCodeMaskFile.txt\"");
            Page.Response.TransmitFile(ShopCodeMaskFile);
            Page.Response.Flush();
            Page.Response.End();
        }

        [WebMethod]
        public static string btnCompareCode_OnClick(bool updateMasks)
        {
            var report = UpdaterService.CompareCodeVersions(updateMasks);

            return !string.IsNullOrEmpty(report)
                       ? report
                       : "<span style=\"color:green;\">Versions of the code are the same</span>";
        }

        [WebMethod]
        public static string btnCompareBase_OnClick(bool updateMasks)
        {
            var report = UpdaterService.CompareBaseVersions(updateMasks);

            return !string.IsNullOrEmpty(report)
                       ? report
                       : "<span style=\"color:green;\">Versions of the database are the same</span>";
        }
    }
}