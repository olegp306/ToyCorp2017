//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;

namespace Tools.core
{
    public partial class CompareCodeMasks : System.Web.UI.Page
    {
        private static readonly string ShopCodeMaskFile = HttpContext.Current.Server.MapPath("~/App_Data/shopMaskFile.txt");
        private static readonly string RootDirectory = HttpContext.Current.Server.MapPath("~/");

        private static readonly List<string> ExclusionFoldersAndFiles = new List<string>{
            ".svn\\",
            "exports\\",
            "Export\\",
            "pictures\\",
            "pictures_default\\",
            "pictures_elbuz\\",
            "pictures_en\\",
            "picturesextra\\",
            "price_download\\",
            "price_temp\\",
            "upload_images\\",
            "images\\",
            "userfiles\\",
            "Lucene",
            "App_Data\\",
            "App_WebReferences",
            "ckeditor\\",
            "Web.ConnectionStrings.config",
            "Web.AppSettings.config",
            "Yamarket.xml",
            "robots.txt",
            "sitemap.html",
            "sitemap.xml"};

        protected void Page_Init(object sender, EventArgs e)
        {

        }

        protected void btnCreateMask_OnClick(object sender, EventArgs e)
        {
            if (CreateCodeMaskFile())
            {
                ltrlReport.Text = CompareCodeMaskFiles();
            }
        }

        private bool CreateCodeMaskFile()
        {
            try
            {
                using (var outputFile = new StreamWriter(ShopCodeMaskFile))
                {
                    foreach (var advFileName in Directory.GetFiles(RootDirectory, "*.*", SearchOption.AllDirectories))
                    {
                        var filiName = advFileName.Replace(HttpContext.Current.Request.PhysicalApplicationPath, "");

                        var isExclusion = ExclusionFoldersAndFiles.Any(exclusion => filiName.Contains(exclusion));
                        if (isExclusion) continue;

                        using (HashAlgorithm hashAlg = new SHA1Managed())
                        {
                            using (Stream file = new FileStream(advFileName, FileMode.Open, FileAccess.Read))
                            {
                                byte[] hash = hashAlg.ComputeHash(file);
                                outputFile.WriteLine(filiName + ";" + BitConverter.ToString(hash));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                return false;
            }
            return true;
        }

        private string CompareCodeMaskFiles()
        {
            var result = string.Empty;

            try
            {
                using (var streamReaderThisMaskFile = new StreamReader(ShopCodeMaskFile))
                {
                    var request = WebRequest.Create(string.Format("http://localhost:20728/AdvUpdateService/HttpHandlers/GetCodeMaskFileByVersion.ashx?version=3.0.0.1&license=0"));
                    request.Method = "POST";

                    var response = request.GetResponse();
                    if (response != null)
                    {
                        using (var streamReaderEtalonMaskFile = new StreamReader(response.GetResponseStream()))// todo: get file from remoute server
                        {
                            while (streamReaderThisMaskFile.Peek() >= 0 && streamReaderEtalonMaskFile.Peek() >= 0)
                            {
                                var stringThisMaskFile = streamReaderThisMaskFile.ReadLine();
                                var stringEtalonMaskFile = streamReaderEtalonMaskFile.ReadLine();

                                var isExclusion = ExclusionFoldersAndFiles.Any(exclusion => stringThisMaskFile.Contains(exclusion) || stringEtalonMaskFile.Contains(exclusion));
                                if (isExclusion) continue;

                                if (stringEtalonMaskFile != stringThisMaskFile)
                                {
                                    result += "discrepancy in : " + stringThisMaskFile + "<br/>";
                                }
                            }
                        }
                    }
                    else
                    {
                        return "not etalon file";
                    }
                }
            }
            catch (Exception ex)
            {
                result = "Error on matching: " + ex;
            }

            return string.IsNullOrEmpty(result) ? "Full match" : result;
        }

        protected void btnUpdateClick(object sender, EventArgs e)
        {

        }
    }
}