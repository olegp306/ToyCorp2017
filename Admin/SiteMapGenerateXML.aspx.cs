//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.ExportImport;
using Resources;

namespace Admin
{
    partial class SiteMapGenerateXML : AdvantShopAdminPage
    {
        // Leave empty if you don't need subfolders
        private const string strInitSubPath = "";
        //private const string strInitSubPath = "sitemap/";

        private string strInitFileVirtualPath = "../" + strInitSubPath;
        private string strInitFileName = "sitemap.xml".ToLower();
        private string _strPhysicalFilePath = string.Empty;
        private string _strPhysicalTargetFolder = string.Empty;

        public SiteMapGenerateXML()
        {
            Load += Page_Load;
            Init += Page_Init;
        }

        private void MsgErr(bool clean)
        {
            if (clean)
            {
                lblErr.Visible = false;
                lblErr.Text = string.Empty;
            }
            else
            {
                lblErr.Visible = false;
            }
        }

        private void MsgErr(string messageText)
        {
            lblErr.Visible = true;
            lblErr.Text = @"<br/>" + messageText;

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            _strPhysicalTargetFolder = Server.MapPath(strInitFileVirtualPath);
            _strPhysicalFilePath = _strPhysicalTargetFolder + strInitFileName;
        }

        public string ShowStrLinkToSiteMapFile()
        {
            return SettingsMain.SiteUrl + "/" + strInitSubPath + strInitFileName;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_SiteMapGenerate_Header));
            RefreshFileDateInfo();
        }

        private void RefreshFileDateInfo()
        {
            lastMod.Text = File.Exists(_strPhysicalFilePath) ? AdvantShop.Localization.Culture.ConvertDate((new FileInfo(_strPhysicalFilePath)).LastWriteTime) : @"---";
        }

        protected void btnCreateMap_Click(object sender, EventArgs e)
        {
            try
            {
                MsgErr(true);

                // Directory
                if (!Directory.Exists(_strPhysicalTargetFolder))
                {
                    Directory.CreateDirectory(_strPhysicalTargetFolder);
                }

                // Old files
                if (File.Exists(_strPhysicalTargetFolder))
                {
                    File.Delete(_strPhysicalTargetFolder);
                }

                // Save new file 
                //SiteMapService.GenerateSiteMap(_strPhysicalFilePath, StrInitFileVirtualPath);

                var temp = new ExportXmlMap(_strPhysicalFilePath, strInitFileVirtualPath);
                temp.Create();

                // Refresh label info
                RefreshFileDateInfo();

            }
            catch (Exception ex)
            {
                MsgErr(ex.Message + " at btnCreateMap_Click");
                AdvantShop.Diagnostics.Debug.LogError(ex);
            }
        }
    }
}