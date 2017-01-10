using System;
using AdvantShop.Catalog;
using AdvantShop.Orders;
using Resources;

namespace Admin.UserControls.Settings
{
    public partial class ProfitSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resource.Admin_CommonSettings_InvalidProfit;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            txtSalesPlan.Text = CatalogService.FormatPriceInvariant(OrderStatisticsService.SalesPlan);
            txtProfitPlan.Text =  CatalogService.FormatPriceInvariant(OrderStatisticsService.ProfitPlan);

        }

        public bool SaveData()
        {
            bool isValid = true;

            float sales = 0;
            float profit = 0;

            if (float.TryParse(txtSalesPlan.Text.Replace(" ", ""), out sales) && sales > 0 && float.TryParse(txtProfitPlan.Text.Replace(" ", ""), out profit) && profit > 0)
            {
                OrderStatisticsService.SetProfitPlan(sales, profit);
            }
            else
            {
                ErrMessage += Resource.Admin_CommonSettings_ProfitError;
                isValid = false;
            }
        
            LoadData();
            return isValid;
        }
    }
}