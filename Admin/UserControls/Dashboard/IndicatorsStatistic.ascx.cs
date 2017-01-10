using System;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Localization;
using AdvantShop.Statistic;

namespace Admin.UserControls.Dashboard
{
    public partial class Admin_UserControls_IndicatorsStatistic : System.Web.UI.UserControl
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            lblSaleToday.Text = Convert.ToString(StatisticService.GetOrdersCountByDate(now));
            lSumToday.Text = CatalogService.GetStringPrice(StatisticService.GetOrdersSumByDate(now));

            lblSaleYesterday.Text = Convert.ToString(StatisticService.GetOrdersCountByDate(now.AddDays(-1)));
            lSumYesterday.Text = CatalogService.GetStringPrice(StatisticService.GetOrdersSumByDate(now.AddDays(-1)));

            lblSaleWeek.Text = Convert.ToString(
                    StatisticService.GetOrdersCountByDateRange(
                        now.StartOfWeek(Culture.Language == Culture.SupportLanguage.English
                                            ? DayOfWeek.Sunday
                                            : DayOfWeek.Monday), now));
            lSumWeek.Text =
                CatalogService.GetStringPrice(
                    StatisticService.GetOrdersSumByDateRange(
                        now.StartOfWeek(Culture.Language == Culture.SupportLanguage.English
                                            ? DayOfWeek.Sunday
                                            : DayOfWeek.Monday), now));

            lblSaleMounth.Text = Convert.ToString(StatisticService.GetOrdersCountByDateRange(new DateTime(now.Year, now.Month, 1), now));
            lSumMonth.Text = CatalogService.GetStringPrice(StatisticService.GetOrdersSumByDateRange(new DateTime(now.Year, now.Month, 1), now));

            lblSale.Text = Convert.ToString(StatisticService.GetOrdersCountByDateRange(now.AddYears(-100), now));
            lSaleSum.Text = CatalogService.GetStringPrice(StatisticService.GetOrdersSumByDateRange(now.AddYears(-100), now));

            lblTotalProducts.Text = Convert.ToString(StatisticService.GetProductsCount());
        }
    }
}