//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Diagnostics;
using Resources;

namespace Admin
{
    public partial class LogViewer : AdvantShopAdminPage
    {
        protected Debug.ErrType CurrentView = Debug.ErrType.ErrHttp;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            lblErr.Text = string.Empty;
            if (!string.IsNullOrEmpty(Request["errtype"]))
            {
                Enum.TryParse(Request["errtype"], true, out CurrentView);
            }

            hlErr404.NavigateUrl = "LogViewer.aspx?ErrType=" + Debug.ErrType.Err404.ToString();
            if (CurrentView == Debug.ErrType.Err404)
                hlErr404.Font.Bold = true;

            hlErr500.NavigateUrl = "LogViewer.aspx?ErrType=" + Debug.ErrType.Err500.ToString();
            if (CurrentView == Debug.ErrType.Err500)
                hlErr500.Font.Bold = true;

            hlErrHttp.NavigateUrl = "LogViewer.aspx?ErrType=" + Debug.ErrType.ErrHttp.ToString();
            if (CurrentView == Debug.ErrType.ErrHttp)
                hlErrHttp.Font.Bold = true;

            string str = string.Empty;
            switch (CurrentView)
            {
                case Debug.ErrType.Err404:
                    str = Resource.Admin_BugTrackerError404_aspx;
                    break;
                case Debug.ErrType.Err500:
                    str = Resource.Admin_MasterPageAdmin_BugTracker_Internal;
                    break;
                case Debug.ErrType.ErrHttp:
                    str = Resource.Admin_MasterPageAdmin_BugTracker_Other;
                    break;
            }

            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, str));
            LoadData();
        }

        private bool LoadData()
        {
            var listLog = new List<LogEntry>();
            try
            {
                if (File.Exists(Debug.ErrFile[CurrentView]))
                {
                    using (var csv = new CsvHelper.CsvReader(new StreamReader(Debug.ErrFile[CurrentView], Encoding.UTF8, true)))
                    {
                        csv.Configuration.Delimiter = Debug.CharSeparate;
                        csv.Configuration.HasHeaderRecord = false;
                        while (csv.Read())
                        {
                            string[] r = csv.CurrentRecord;
                            if (r.Length >= 4)
                            {
                                var item = new LogEntry
                                    {
                                        TimeStamp = r[0],
                                        Level = r[1],
                                        Message = r[2],
                                        ErrorMessage = r[3]
                                    };
                                listLog.Add(item);
                            }
                            else
                            {
                                lblErr.Text = @"Log file wrong format";
                                return false;
                            }
                        }
                    }
                    listLog.Reverse();
                    grid.DataSource = listLog;
                    grid.DataBind();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                lblErr.Text = ex.Message;
                return false;
            }
        }
    }
}
