//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;

namespace UserControls
{
    public partial class NewsSubscription : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Visible = SettingsDesign.NewsSubscriptionVisibility;
        }


        protected void btnSubmit_Click(object sender, System.EventArgs e)
        {
            Page.Response.Redirect("~/subscribe.aspx?emailtosubscribe=" + txtEmail.Text);
        }
    }
}
