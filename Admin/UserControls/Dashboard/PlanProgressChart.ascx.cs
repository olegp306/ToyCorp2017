using System;
using AdvantShop.Orders;

namespace Admin.UserControls.Dashboard
{
    public partial class PlanProgressChart : System.Web.UI.UserControl
    {
        protected double planPercent;
        protected float sales;

        protected void Page_Load(object sender, EventArgs e)
        {
            var plannedSales = OrderStatisticsService.SalesPlan;
            sales = OrderStatisticsService.GetMonthProgress().Key;
            planPercent = Math.Round(sales / (plannedSales / 100));
        }
    }
}