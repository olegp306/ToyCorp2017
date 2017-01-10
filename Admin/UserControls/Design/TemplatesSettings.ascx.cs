//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Design;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Trial;
using Resources;

namespace Admin.UserControls.Design
{
    public partial class TemplatesSettings : System.Web.UI.UserControl
    {
        protected const string ThemePicturePath = "http://modules.advantshop.net/template/getthemepicture/?id={0}";
        protected const string _default = TemplateService.DefaultTemplateId;

        #region Errors
        private void MsgErr(bool clear)
        {
            lblError.Visible = false;
            lblError.Text = string.Empty;
        }

        private void MsgErr(string messageText)
        {
            lblError.Visible = true;
            lblError.Text += messageText;
        }
        #endregion
    
        protected void Page_PreRender(object sender, EventArgs e)
        {
            MsgErr(true);
        
            LoadData();
            lTrialMode.Visible = TrialService.IsTrialEnabled;
        }


        private void LoadData()
        {
            var templates = TemplateService.GetTemplates();       

            DataListTemplates.DataSource = templates.Items;
            DataListTemplates.DataBind();
        }

        protected void dlItems_ItemCommand(object source, ListViewCommandEventArgs e)
        {
            string stringId = e.CommandArgument.ToString();

            if (e.CommandName == "Add")
            {
                var message = TemplateService.GetTemplateArchiveFromRemoteServer(stringId);

                if (!message.IsNullOrEmpty())
                {
                    MsgErr(true);
                    MsgErr(message);
                }
            }

            if (e.CommandName == "Delete")
            {
                UninstallTemplate(stringId);
                SettingsDesign.ChangeTemplate(_default);
                CacheManager.Clean();
            }

            if (e.CommandName == "ApplyTheme")
            {
                SettingsDesign.ChangeTemplate(stringId);
                CacheManager.Clean();
            }
        }

        private void UninstallTemplate(string templateName)
        {
            if (templateName == _default)
                return;

            MsgErr(true);

            try
            {
                SettingsDesign.ChangeTemplate(_default);
                FileHelpers.DeleteDirectory(Server.MapPath("~/Templates/" + templateName));

                MsgErr(Resource.Admin_Templates_UninstallSuccess);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                MsgErr(Resource.Admin_Templates_UninstallFail);
            }
        }

        protected string RenderTemplatePicture(string previewImage, string templateId)
        {
            if (!string.IsNullOrEmpty(previewImage))
                return string.Format("<img src=\"{0}\" />", previewImage);

            return string.Format("<img src=\"{0}\" />", string.Format(ThemePicturePath, templateId));
        }
    }
}