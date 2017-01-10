//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using System.IO;
using System.Web.Services;

namespace Tools.core
{
    public partial class Backuper : System.Web.UI.Page
    {
        private static readonly string _codeFile = "App_Data\\dak_code.zip";
        private static readonly string _sqlFile = HttpContext.Current.Server.MapPath("~/App_Data/bak.sql");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (File.Exists(HttpContext.Current.Server.MapPath("~/" + _codeFile)))
            {
                lnkFileCode.Text = @"source archiv (" + new FileInfo(HttpContext.Current.Server.MapPath("~/" + _codeFile)).LastWriteTime + @")";
            }

            if (File.Exists(_sqlFile))
            {
                lnkFileSql.Text = @"base backup (" + new FileInfo(_sqlFile).LastWriteTime + @")";
            }
        }

        protected void lnkFileSql_Click(object sender, EventArgs e)
        {
            Page.Response.Clear();
            Page.Response.AppendHeader("content-disposition", "attachment; filename=\"adv_sql_backup.sql\"");
            Page.Response.TransmitFile(_sqlFile);
            Page.Response.Flush();
            Page.Response.End();
        }

        protected void lnkFileCode_Click(object sender, EventArgs e)
        {
            Page.Response.Clear();
            Page.Response.AppendHeader("content-disposition", "attachment; filename=\"adv_code_backup.sql\"");
            Page.Response.TransmitFile(HttpContext.Current.Server.MapPath("~/" + _codeFile));
            Page.Response.Flush();
            Page.Response.End();
        }

        [WebMethod]
        public static string btnBackupSql_Click()
        {
            Advantshop_Tools.UpdaterService.CreateBaseBackup();
            if (File.Exists(_sqlFile))
            {
                return @"base backup (" + new FileInfo(_sqlFile).LastWriteTime + @")";
            }
            return string.Empty;
        }

        [WebMethod]
        public static string btnBackupCode_Click()
        {
            Advantshop_Tools.UpdaterService.CreateCodeBackup();
            System.Threading.Thread.Sleep(300);
            if (File.Exists(HttpContext.Current.Server.MapPath("~/" + _codeFile)))
            {
                return @"source archiv (" + new FileInfo(HttpContext.Current.Server.MapPath("~/" + _codeFile)).LastWriteTime + @")";
            }
            return string.Empty;
        }
    }
}