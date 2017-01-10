using System;
using AdvantShop.Helpers;
using AdvantShop.Repository;

namespace UserControls.MyAccount
{
    public partial class AddressBook : System.Web.UI.UserControl
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            cboCountry.DataSource = CountryService.GetAllCountries();
            cboCountry.DataBind();

            if (cboCountry.Items.FindByValue(IpZoneContext.CurrentZone.CountryId.ToString()) != null)
                cboCountry.SelectedValue = IpZoneContext.CurrentZone.CountryId.ToString();
        }

        protected void llbAddressBook_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.AbsolutePath +
                              QueryHelper.ChangeQueryParam(Request.Url.Query, "View", "AddressBook"));
        }
    }
}