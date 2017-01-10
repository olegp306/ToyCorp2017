//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.SEO;

namespace ClientPages
{
    public partial class ModulePage : AdvantShopClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(Request["ModuleId"]))
            //{
            //    Response.Redirect("");
            //}


            var module = AttachedModules.GetModules<IClientPageModule>().FirstOrDefault(
                item => ((IClientPageModule)Activator.CreateInstance(item, null)).ModuleStringId == Request["ModuleId"]);

            if (module != null)
            {
                var moduleObject = (IClientPageModule)Activator.CreateInstance(module, null);
                
                var userControl =
                    (this).LoadControl(
                        UrlService.GetAbsoluteLink(string.Format("/Modules/{0}/{1}", moduleObject.ModuleStringId,
                                                                 moduleObject.ClientPageControlFileName)));

                if (userControl != null)
                {
                    pnlContent.Controls.Add(userControl);
                }

                SetMeta(new MetaInfo
                    {
                        Title = moduleObject.PageTitle,
                        MetaDescription = moduleObject.MetaDescription,
                        MetaKeywords = moduleObject.MetaKeyWords
                    }, "");
            }
            else
            {
                Response.Redirect(UrlService.GetAdminAbsoluteLink(""));
            }
        }
    }
}