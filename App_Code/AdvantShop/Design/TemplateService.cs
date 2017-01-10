using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;

using AdvantShop.Configuration;
using Newtonsoft.Json;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.DownloadableContent;

namespace AdvantShop.Design
{
    public class TemplateService
    {
        //private const string RequestUrlGetTemplates = "http://modules.advantshop.net/template/gettemplates/?lickey={0}";
        //private const string RequestUrlGetTemplateArchive = "http://modules.advantshop.net/template/gettemplate?lickey={0}&templateId={1}";
        public const string DefaultTemplateId = "_default";


        private const string RequestUrlGetTemplates = "http://modules.advantshop.net/DownloadableContent/GetDlcs?id={0}&dlctype=Template";
        private const string RequestUrlGetTemplateArchive = "http://modules.advantshop.net/DownloadableContent/GetDlc?lickey={0}&dlcId={1}";

        public static DownloadableContentBox GetTemplates()
        {
            var templates = GetTemplatesFromRemoteServer();

            templates.Items.Add(new DownloadableContentObject()
                {
                    StringId = DefaultTemplateId,
                    Name = Resources.Resource.Admin_Templates_DefaultTemplate,
                    IsInstall = true,
                    Active = true,
                    Icon = "../images/design/preview.png"
                });

            if (Directory.Exists(SettingsGeneral.AbsolutePath + "Templates"))
            {
                foreach (var templateFolder in Directory.GetDirectories(SettingsGeneral.AbsolutePath + "Templates"))
                {
                    if (File.Exists(templateFolder + "\\MasterPage.master"))
                    {
                        var stringId = templateFolder.Split('\\').Last();
                        var curTemplate = templates.Items.Find(t => t.StringId == stringId.ToLower());

                        if (curTemplate != null)
                        {
                            curTemplate.IsInstall = true;
                            curTemplate.Active = true;

                        }
                        else
                        {
                            templates.Items.Add(new DownloadableContentObject
                                {
                                    StringId = stringId,
                                    Name = stringId,
                                    IsInstall = true,
                                    Icon = string.Format("../Templates/{0}/images/design/preview.png", stringId),
                                    Active = true
                                });
                        }
                    }
                }
            }

            templates.Items = templates.Items.OrderBy(t => t.Name).ThenByDescending(t => t.IsInstall).ToList();

            var resultTemplateBox = new DownloadableContentBox()
                                    {
                                        Message = templates.Message,
                                        Items = new List<DownloadableContentObject>()
                                    };

            resultTemplateBox.Items.Add(templates.Items.FirstOrDefault(t => t.StringId == SettingsDesign.Template));
            resultTemplateBox.Items.AddRange(templates.Items.Where(t => t.StringId != SettingsDesign.Template));

            return resultTemplateBox;
        }

        private static DownloadableContentBox GetTemplatesFromRemoteServer()
        {
            var templateBox = new DownloadableContentBox() { Items = new List<DownloadableContentObject>() };

            try
            {
                var request = WebRequest.Create(string.Format(RequestUrlGetTemplates, SettingsLic.LicKey));
                request.Method = "GET";

                using (var dataStream = request.GetResponse().GetResponseStream())
                using (var reader = new StreamReader(dataStream))
                {
                    var responseFromServer = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(responseFromServer))
                    {
                        templateBox = JsonConvert.DeserializeObject<DownloadableContentBox>(responseFromServer);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return templateBox;
        }

        public static string GetTemplateArchiveFromRemoteServer(string templateId)
        {
            var zipFileName = HttpContext.Current.Server.MapPath("~/App_Data/" + Guid.NewGuid() + ".Zip");
            try
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadFile(string.Format(RequestUrlGetTemplateArchive, SettingsLic.LicKey, templateId),
                                                    zipFileName);
                }

                if (!FileHelpers.UnZipFile(zipFileName, HttpContext.Current.Server.MapPath("~/Templates/")))
                {
                    return "error on UnZipFile";
                }

                FileHelpers.DeleteFile(zipFileName);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return "error on download or unzip";
            }

            return string.Empty;
        }

        public static bool IsExistTemplate(string templateId)
        {
            if (SettingsDesign.Template == DefaultTemplateId)
                return true;
            return File.Exists(SettingsGeneral.AbsolutePath + "Templates\\" + templateId + "\\MasterPage.master");
        }
    }
}