//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web.Services;
using Advantshop_Tools;

namespace Tools.core
{
    public partial class Updater : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            var versInf = UpdaterService.GetLastVersionInformation();
            lblLastVersion.Text = versInf.lastVersion;

            if (!string.IsNullOrEmpty(versInf.versionHistory))
            {
                lblVersionInformation.InnerHtml = versInf.versionHistory;
            }
            else
            {
                lblVersionInformation.InnerHtml = "No information";
                divMoreInf.Visible = false;
            }
        }

        [WebMethod]
        public static string btnUpdate_Click(bool updateMasks)
        {
            UpdaterStatus.IsProcess = true;

            //1. проверка на возможность обновления
            UpdaterStatus.Status = "Check source ...";
            //System.Threading.Thread.Sleep(2000);
            var compareCodeInf = UpdaterService.CompareCodeVersions(updateMasks);
            if (!string.IsNullOrEmpty(compareCodeInf))
            {
                UpdaterStatus.Status = "Not completed";
                return compareCodeInf;
            }

            UpdaterStatus.Status = "Check database ...";
            //System.Threading.Thread.Sleep(2000);
            var compareBaseInf = UpdaterService.CompareBaseVersions(updateMasks);
            if (!string.IsNullOrEmpty(compareBaseInf))
            {
                UpdaterStatus.Status = "Not completed";
                return compareBaseInf;
            }

            //2. Создание бэкапов
            UpdaterStatus.Status = "Create database backup ...";
            //System.Threading.Thread.Sleep(2000);
            UpdaterService.CreateBaseBackup();

            UpdaterStatus.Status = "Create source backup ...";
            //System.Threading.Thread.Sleep(2000);
            UpdaterService.CreateCodeBackup();

            //3. скачваем последнюю версию двига и обновляем
            UpdaterStatus.Status = "Create source backup ...";
            //System.Threading.Thread.Sleep(2000);
            UpdaterService.UpdateAvantshop();


            UpdaterStatus.IsProcess = false;
            return string.Empty;
        }
    }
}