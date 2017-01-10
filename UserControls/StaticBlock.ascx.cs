//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.CMS;

namespace UserControls
{
    public partial class StaticBlock : System.Web.UI.UserControl
    {
        public string SourceKey { get; set; }
        public string CssClass { get; set; }
        public bool DisableInplaceEditor { get; set; }

        protected AdvantShop.CMS.StaticBlock sb;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(SourceKey))
            {
                sb = StaticBlockService.GetPagePartByKeyWithCache(SourceKey);
            }

            this.Visible = sb != null && sb.Enabled;
        }

        protected string GetContent()
        {
            var result = "";

            if (sb != null)
            {
                result = sb.Content;
            }

            return result;
        }
    }
}