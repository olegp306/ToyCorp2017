using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Security;
using System;
using AdvantShop.Trial;
using System.Globalization;
using AdvantShop.Orders;

namespace UserControls.MasterPage
{
    public partial class TopPanel : System.Web.UI.UserControl
    {
        protected string CurrentTown = IpZoneContext.CurrentZone.City;
        protected string WishlistCount = string.Empty;


        protected void Page_Load(object sender, EventArgs e)
        {
            var curentCustomer = CustomerContext.CurrentCustomer;

            aLogin.Visible = aRegister.Visible = !curentCustomer.RegistredUser;
            aRegister.HRef = UrlService.GetAbsoluteLink("registration.aspx");
            aLogin.HRef = UrlService.GetAbsoluteLink("login.aspx");
            lbLogOut.Visible = aMyAccount.Visible = curentCustomer.RegistredUser;
            aMyAccount.HRef = UrlService.GetAbsoluteLink("myaccount.aspx");
            pnlAdmin.Visible = (curentCustomer.CustomerRole == Role.Administrator || curentCustomer.CustomerRole == Role.Moderator || TrialService.IsTrialEnabled);
            pnlAdmin.HRef = UrlService.GetAbsoluteLink("admin/default.aspx");

            aCreateTrial.Visible = Demo.IsDemoEnabled;

            pnlCurrency.Visible = SettingsCatalog.AllowToChangeCurrency;
            foreach (Currency row in CurrencyService.GetAllCurrencies(true))
            {
                ddlCurrency.Items.Add(new ListItem(row.Name, row.Iso3));
            }
            ddlCurrency.SelectedValue = CurrencyService.CurrentCurrency.Iso3;

            pnlCity.Visible = SettingsDesign.DisplayCityInTopPanel;

            int wishCount = ShoppingCartService.CurrentWishlist.Count;
            WishlistCount = string.Format("{0} {1}", wishCount == 0 ? "" : wishCount.ToString(CultureInfo.InvariantCulture),
                                          Strings.Numerals(wishCount, Resources.Resource.Client_MasterPage_WishList_Empty,
                                                           Resources.Resource.Client_MasterPage_WishList_1Product,
                                                           Resources.Resource.Client_MasterPage_WishList_2Products,
                                                           Resources.Resource.Client_MasterPage_WishList_5Products));

            pnlWishList.Visible = SettingsDesign.WishListVisibility && !pnlCity.Visible;

        }

        public void btnLogout_Click(object sender, EventArgs e)
        {
            AuthorizeService.SignOut();
            Response.Redirect("~/");
        }
    }
}