using System;
using System.Web;
using System.Xml.Linq;
using AdvantShop.Configuration;
using AdvantShop.Customers;

public partial class Api_1c_init_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var login = Request["login"];
        var pass = Request["password"];

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(pass))
        {
            Response("error auth");
            return;
        }

        var customer = CustomerService.GetCustomerByEmailAndPassword(login, pass, false);

        if (customer == null || (!customer.IsAdmin && !customer.IsModerator))
        {
            Response("error auth");
            return;
        }

        var apikey = SettingsApi.ApiKey;
        var siteUrl = SettingsMain.SiteUrl.TrimEnd('/');

        var doc = new XDocument(
            new XElement("settings",
                new XElement("check_apikey_url", siteUrl + "/api/1c/chekapikey.ashx?apikey=" + apikey),
                new XElement("import_products_url", siteUrl + "/api/1c/importproducts.ashx?apikey=" + apikey),
                new XElement("import_photos_url", siteUrl + "/api/1c/importphotos.ashx?apikey=" + apikey),
                new XElement("export_products_url", siteUrl + "/api/1c/exportproducts.ashx?apikey=" + apikey),
                new XElement("export_deletedproducts_url", siteUrl + "/api/1c/deletedproducts.ashx?apikey=" + apikey),
                new XElement("export_orders_url", siteUrl + "/api/1c/exportorders.ashx?apikey=" + apikey),
                new XElement("export_deletedorders_url", siteUrl + "/api/1c/deletedorders.ashx?apikey=" + apikey),
                new XElement("change_orderstatus_url", siteUrl + "/api/1c/changeorderstatus.ashx?apikey=" + apikey)
                )
            );

        var xml = doc.ToString();

        Response(xml);
    }

    private void Response(string message)
    {
        var context = HttpContext.Current;
        context.Response.ContentType = "text/plain";
        context.Response.Write(message);
        context.Response.End();
    }
}