//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.IO;
using System.Web;
using System.Net;
using System.Threading;

using AdvantShop.Configuration;
using AdvantShop.SaasData;
using Newtonsoft.Json;
using AdvantShop.Diagnostics;  
using AdvantShop.Modules.Interfaces;
using AdvantShop.Helpers;


namespace AdvantShop.Modules
{
    public class ModulesService
    {
        //private const string RequestUrlGetModules = "http://modules.advantshop.net/modules/GetModules/{0}";
        //private const string RequestUrlGetModuleArchive = "http://modules.advantshop.net/modules/GetModule?lickey={0}&moduleId={1}";

        private const string RequestUrlGetModules = "http://modules.advantshop.net/DownloadableContent/GetDlcs?id={0}&dlctype=Module&storeversion={1}";
        private const string RequestUrlGetModuleArchive = "http://modules.advantshop.net/DownloadableContent/GetDlc?lickey={0}&dlcId={1}&storeversion={2}";

        #region Process modules from remote server

        public static ModuleBox GetModules()
        {
            var modules = GetModulesFromRemoteServer() ?? new ModuleBox();

            var isExistsOnlineModules = modules.Items.Count > 0;

            foreach (var type in AttachedModules.GetModules())
            {
                var moduleInst = (IModule)Activator.CreateInstance(type, null);
                Module curModule;
                if (isExistsOnlineModules && (curModule = modules.Items.FirstOrDefault(item => item.StringId.ToLower() == moduleInst.ModuleStringId.ToLower())) != null)
                {
                    curModule.IsInstall = moduleInst.CheckAlive() && 
                                        ModulesRepository.IsInstallModule(moduleInst.ModuleStringId);
                    curModule.HasSettings = moduleInst.HasSettings;
                    curModule.Enabled = ModulesRepository.IsActiveModule(moduleInst.ModuleStringId);
                }
                else
                {
                    modules.Items.Add(new Module()
                    {
                        Name = moduleInst.ModuleName,
                        StringId = moduleInst.ModuleStringId,
                        Version = Resources.Resource.ModulesService_ModuleInDebug,
                        IsInstall =
                            moduleInst.CheckAlive() && ModulesRepository.IsInstallModule(moduleInst.ModuleStringId),
                        Price = 0,
                        IsLocalVersion = true,
                        Active = true,
                        HasSettings = moduleInst.HasSettings,
                        Enabled = ModulesRepository.IsActiveModule(moduleInst.ModuleStringId)
                    });
                }
            }

            var existModules = ModulesRepository.GetModulesFromDb();
            foreach (var module in modules.Items)
            {
                Module currentModule;
                if (existModules.Count > 0 &&
                    (currentModule = existModules.FirstOrDefault(item => item.StringId == module.StringId)) != null)
                {
                    module.CurrentVersion = currentModule.Version;
                }
            }
            return modules;
        }

        private static ModuleBox GetModulesFromRemoteServer()
        {
            var modules = new ModuleBox();

            try
            {
                var request = WebRequest.Create(string.Format(RequestUrlGetModules, 
                    SaasDataService.IsSaasEnabled ? SettingsGeneral.CurrentSaasId : SettingsLic.LicKey, 
                    SettingsGeneral.SiteVersionDev));
                request.Method = "GET";

                using (var dataStream = request.GetResponse().GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        var responseFromServer = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(responseFromServer))
                        {
                            modules = JsonConvert.DeserializeObject<ModuleBox>(responseFromServer);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return modules;
        }

        public static string GetModuleArchiveFromRemoteServer(string moduleId)
        {
            var zipFileName = HttpContext.Current.Server.MapPath("~/App_Data/" + Guid.NewGuid() + ".Zip");
            try
            {
                new WebClient().DownloadFile(
                    string.Format(RequestUrlGetModuleArchive, SettingsLic.LicKey, moduleId, SettingsGeneral.SiteVersionDev),
                    zipFileName
                   );

                if (!FileHelpers.UnZipFile(zipFileName, HttpContext.Current.Server.MapPath("~/")))
                {
                    return "error on UnZipFile";
                }
                FileHelpers.DeleteFile(zipFileName);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return "error on UnZipFile";
            }

            return string.Empty;
        }

        #endregion

        #region Public methods to process modules

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleStringId"></param>
        /// <returns></returns>
        public static bool InstallModule(string moduleStringId)
        {
            return InstallModule(moduleStringId, "0");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleStringId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool InstallModule(string moduleStringId, string version)
        {
            var moduleInst = AttachedModules.GetModules().FirstOrDefault(
            item =>
            ((IModule)Activator.CreateInstance(item, null)).ModuleStringId.ToLower() == moduleStringId.ToLower());

            if (moduleInst != null)
            {
                var module = ((IModule)Activator.CreateInstance(moduleInst, null));
                if (module.InstallModule())
                {
                    
                    ModulesRepository.InstallModuleToDb(
                        new Module
                        {
                            StringId = module.ModuleStringId,
                            Name = module.ModuleName,
                            DateModified = DateTime.Now,
                            DateAdded = DateTime.Now,
                            Version = version,
                            Active = false,
                            HasSettings = module.HasSettings
                        });
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleStringId"></param>
        /// <returns></returns>
        public static bool UninstallModule(string moduleStringId)
        {
            var moduleInst = AttachedModules.GetModules().FirstOrDefault(
                    item =>
                    ((IModule)Activator.CreateInstance(item, null)).ModuleStringId.ToLower() == SQLDataHelper.GetString(moduleStringId).ToLower());

            if (moduleInst != null)
            {
                var module = ((IModule)Activator.CreateInstance(moduleInst, null));
                if (module.UninstallModule())
                {
                    ModulesRepository.UninstallModuleFromDb(module.ModuleStringId);
                }
                else
                {
                    return false;
                }
            }

            FileHelpers.DeleteDirectory(HttpContext.Current.Server.MapPath("~/App_Code/Advantshop/Modules/" + moduleStringId));
            FileHelpers.DeleteDirectory(HttpContext.Current.Server.MapPath("~/Modules/" + moduleStringId));
            
            return true;
        }

        public static string GetModuleStringIdByUrlPath(string urlPath)
        {
            if (urlPath.IsNullOrEmpty())
                return string.Empty;

            var moduleInst = AttachedModules.GetModules<IClientPageModule>().FirstOrDefault(
                        item =>
                        ((IClientPageModule)Activator.CreateInstance(item, null)).UrlPath.ToLower() == SQLDataHelper.GetString(urlPath).ToLower());

            if (moduleInst != null)
            {
                var module = ((IModule)Activator.CreateInstance(moduleInst, null));
                return module.ModuleStringId;
            }

            return string.Empty;
        }
        #endregion

        #region Call core methods for modules

        public static void SendModuleMail(string subject, string message, string email, bool isBodyHtml)
        {
            Mails.SendMail.SendMailNow(email, subject, message, isBodyHtml);
        }

        #endregion
    }
}