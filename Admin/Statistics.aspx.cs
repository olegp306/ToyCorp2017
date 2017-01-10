//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using Resources;

namespace Admin
{
    public partial class Statistics : AdvantShopAdminPage
    {
        private DateTime _dateFrom;
        private DateTime _dateTo;

        private enum EGroupDateBy
        {
            Day,
            Week,
            Month
        }
        private EGroupDateBy _groupBy;
        private string _groupFormatString;

        private int? _statusId;
        private bool? _paied;

        private readonly float _currencyValue = CurrencyService.CurrentCurrency.Value;

        protected void Page_PreRender(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_Statistics_Header));

            Filter();

            LoadOrdersSumGraph();
            LoadOrdersCountGraph();
            LoadOrderRegGraph();

            LoadTopPayments();
            LoadTopShippings();
            LoadTopCities();

            LoadTopCustomers();
            LoadTopProducts();
            LoadTopProductsBySum();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {

        }

        private void Filter()
        {
            if (txtDateFrom.Text.IsNullOrEmpty() || txtDateTo.Text.IsNullOrEmpty())
            {
                _dateFrom = DateTime.Now.AddMonths(-1);
                _dateTo = DateTime.Now;

                txtDateFrom.Text = _dateFrom.ToString("dd.MM.yyyy");
                txtDateTo.Text = _dateTo.ToString("dd.MM.yyyy");
            }
            else
            {
                _dateFrom = txtDateFrom.Text.TryParseDateTime();
                _dateTo = txtDateTo.Text.TryParseDateTime();
            }

            if (groupbyDay.Checked)
            {
                _groupBy = EGroupDateBy.Day;
                _groupFormatString = "dd";
            }
            else if (groupbyWeek.Checked)
            {
                _groupBy = EGroupDateBy.Week;
                _groupFormatString = "wk";
            }
            else if (groupbyMounth.Checked)
            {
                _groupBy = EGroupDateBy.Month;
                _groupFormatString = "mm";
            }
            else
            {
                _groupBy = EGroupDateBy.Day;
                _groupFormatString = "dd";
                groupbyDay.Checked = true;
            }

            _statusId = ddlStatuses.SelectedIndex > 0 ? ddlStatuses.SelectedValue.TryParseInt() : default(int?);

            ddlStatuses.DataSource = OrderService.GetOrderStatuses();
            ddlStatuses.DataBind();
            ddlStatuses.Items.Insert(0, new ListItem(Resource.Admin_Catalog_Any, "0"));

            if (_statusId != null && ddlStatuses.Items.FindByValue(((int)_statusId).ToString()) != null)
            {
                ddlStatuses.SelectedValue = ((int)_statusId).ToString();
            }
            
            switch (ddlPayed.SelectedIndex)
            {
                case 0:
                    _paied = null;
                    break;
                case 1:
                    _paied = true;
                    break;
                case 2:
                    _paied = false;
                    break;
            }
        }

        private void LoadOrdersSumGraph()
        {
            orderGraph.Attributes["data-chart"] = RenderOrdersSumGraph(_dateFrom, _dateTo);
            orderGraph.Attributes["data-chart-options"] =
                string.Format("{{xaxis : {{ mode: 'time', timeformat: '%d %b', min: {0}, max: {1}}} }}",
                    GetTimestamp(_dateFrom), GetTimestamp(_dateTo));
        }

        private void LoadOrdersCountGraph()
        {
            orderCountGraph.Attributes["data-chart"] = RenderOrdersCountGraph(_dateFrom, _dateTo);
            orderCountGraph.Attributes["data-chart-options"] =
                string.Format("{{xaxis : {{ mode: 'time', timeformat: '%d %b', min: {0}, max: {1}}} }}",
                    GetTimestamp(_dateFrom), GetTimestamp(_dateTo));
        }

        private void LoadOrderRegGraph()
        {
            orderRegGraph.Attributes["data-chart"] = RenderOrdersRegGraph(_dateFrom, _dateTo);
            orderRegGraph.Attributes["data-chart-options"] =
                string.Format("{{xaxis : {{ mode: 'time', timeformat: '%d %b', min: {0}, max: {1}}} }}",
                    GetTimestamp(_dateFrom), GetTimestamp(_dateTo));
        }

        private void LoadTopPayments()
        {
            paymentsPie.Attributes["data-chart"] = RenderTopPayments();
            paymentsPie.Attributes["data-chart-options"] =
                "{series: { pie: { show: true,radius: 1,innerRadius: 0.5,label: {show: true,radius: 3/4,formatter: function(label, series) {return '<div style=\"font-size:14px;text-align:center;color:white;\">' + Math.round(series.percent) + '%</div>';},background: { opacity: 0 }}, offset:{top: 0,left: -70}}}}";
        }

        private void LoadTopShippings()
        {
            shippingsPie.Attributes["data-chart"] = RenderTopShippings();
            shippingsPie.Attributes["data-chart-options"] =
                "{series: { pie: { show: true,radius: 1,innerRadius: 0.5,label: {show: true,radius: 3/4,formatter: function(label, series) {return '<div style=\"font-size:14px;text-align:center;color:white;\">' + Math.round(series.percent) + '%</div>';},background: { opacity: 0 }}, offset:{top: 0,left: -70}}}}";
        }

        private void LoadTopCities()
        {
            orderCitiesPie.Attributes["data-chart"] = RenderTopCities();
            orderCitiesPie.Attributes["data-chart-options"] =
                "{series: { pie: { show: true,radius: 1,innerRadius: 0.5,label: {show: true,radius: 3/4,formatter: function(label, series) {return '<div style=\"font-size:14px;text-align:center;color:white;\">' + Math.round(series.percent) + '%</div>';},background: { opacity: 0 }}, offset:{top: 0,left: -70}}}}";
        }

        private void LoadTopCustomers()
        {
            lvCustomers.DataSource = OrderStatisticsService.GetTopCustomersBySumPrice();
            lvCustomers.DataBind();
        }

        private void LoadTopProducts()
        {
            lvProducts.DataSource = OrderStatisticsService.GetTopProductsByCount();
            lvProducts.DataBind();
        }

        private void LoadTopProductsBySum()
        {
            lvProductsBySum.DataSource = OrderStatisticsService.GetTopProductsBySum();
            lvProductsBySum.DataBind();
        }

        #region Render

        private string RenderOrdersSumGraph(DateTime dateFrom, DateTime dateTo)
        {
            var listSum = OrderStatisticsService.GetOrdersSumGroupByDay(_groupFormatString, dateFrom, dateTo, _paied, _statusId);

            var data = "";
            switch (_groupBy)
            {
                case EGroupDateBy.Day:
                    data = GetByDays(listSum, dateFrom, dateTo);
                    break;
                case EGroupDateBy.Week:
                    data = GetByWeeks(listSum, dateFrom, dateTo);
                    break;
                case EGroupDateBy.Month:
                    data = GetByMonths(listSum, dateFrom, dateTo);
                    break;
            }

            return String.Format("[{{label: '{0}', data:[{1}]}}]", Resource.Admin_Default_Orders, data);
        }

        private string RenderOrdersCountGraph(DateTime dateFrom, DateTime dateTo)
        {
            var listSum = OrderStatisticsService.GetOrdersCountGroupByDay(_groupFormatString, dateFrom, dateTo, _paied, _statusId);

            var data = "";
            switch (_groupBy)
            {
                case EGroupDateBy.Day:
                    data = GetByDays(listSum, dateFrom, dateTo);
                    break;
                case EGroupDateBy.Week:
                    data = GetByWeeks(listSum, dateFrom, dateTo);
                    break;
                case EGroupDateBy.Month:
                    data = GetByMonths(listSum, dateFrom, dateTo);
                    break;
            }

            return String.Format("[{{label: '{0}', data:[{1}]}}]", Resource.Admin_Statistics_OrdersByCount, data);
        }

        private string RenderOrdersRegGraph(DateTime dateFrom, DateTime dateTo)
        {
            var listSumForReg = OrderStatisticsService.GetOrdersRegGroupByDay(_groupFormatString, dateFrom, dateTo, true, _paied, _statusId);
            var listSumForUnReg = OrderStatisticsService.GetOrdersRegGroupByDay(_groupFormatString, dateFrom, dateTo, false, _paied, _statusId);

            var dataReg = "";
            var dataUnReg = "";
            switch (_groupBy)
            {
                case EGroupDateBy.Day:
                    dataReg = GetByDays(listSumForReg, dateFrom, dateTo);
                    dataUnReg = GetByDays(listSumForUnReg, dateFrom, dateTo);
                    break;
                case EGroupDateBy.Week:
                    dataReg = GetByWeeks(listSumForReg, dateFrom, dateTo);
                    dataUnReg = GetByWeeks(listSumForUnReg, dateFrom, dateTo);
                    break;
                case EGroupDateBy.Month:
                    dataReg = GetByMonths(listSumForReg, dateFrom, dateTo);
                    dataUnReg = GetByMonths(listSumForUnReg, dateFrom, dateTo);
                    break;
            }

            return String.Format("[{{label: '{0}', data:[{1}]}}, {{label: '{2}', data:[{3}]}}]",
                Resource.Admin_Statistics_Registered, dataReg,
                Resource.Admin_Statistics_UnRegistered, dataUnReg);
        }

        private string RenderTopPayments()
        {
            var payments = OrderStatisticsService.GetTopPayments();

            if (payments.Count >= 10)
            {
                var paymentStat = payments.Take(9).ToList();
                paymentStat.Add(new KeyValuePair<string, int>(Resource.Admin_Statistics_Others, payments.Skip(9).Sum(x => x.Value)));
                payments = paymentStat;
            }

            return String.Format("[{0}]",
                payments.Aggregate("",
                    (current, payment) =>
                        current +
                        string.Format("{{ label: \"{0}\",  data:{1}}},", payment.Key, payment.Value)));
        }

        private string RenderTopShippings()
        {
            var shippings = OrderStatisticsService.GetTopShippings();

            if (shippings.Count >= 10)
            {
                var pshippingStat = shippings.Take(9).ToList();
                pshippingStat.Add(new KeyValuePair<string, int>(Resource.Admin_Statistics_Others, shippings.Skip(9).Sum(x => x.Value)));
                shippings = pshippingStat;
            }

            return String.Format("[{0}]",
                shippings.Aggregate("",
                    (current, shipping) =>
                        current + string.Format("{{ label: \"{0}\",  data:{1}}},", shipping.Key, shipping.Value)));
        }

        private string RenderTopCities()
        {
            var shippings = OrderStatisticsService.GetTopCities();

            if (shippings.Count >= 10)
            {
                var pshippingStat = shippings.Take(9).ToList();
                pshippingStat.Add(new KeyValuePair<string, int>(Resource.Admin_Statistics_Others, shippings.Skip(9).Sum(x => x.Value)));
                shippings = pshippingStat;
            }

            return String.Format("[{0}]",
                shippings.Aggregate("",
                    (current, shipping) =>
                        current + string.Format("{{ label: \"{0}\",  data:{1}}},", shipping.Key, shipping.Value)));
        }

        #endregion

        #region Help methods

        private static long GetTimestamp(DateTime date)
        {
            TimeSpan span = (date - new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (long)(span.TotalSeconds * 1000);
        }

        private string GetByDays(Dictionary<DateTime, float> list, DateTime startDate, DateTime endDate)
        {
            var resultList = new List<string>();
            var tempDate = DateTime.MinValue;

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            foreach (var profit in list)
            {
                if (tempDate != DateTime.MinValue)
                {
                    var dayOffset = (profit.Key - tempDate).Days;
                    for (var i = 1; i < dayOffset; i++)
                    {
                        resultList.Add(string.Format("[{0},{1}]", GetTimestamp(tempDate.AddDays(i)), 0));
                    }
                }
                else
                {
                    var dayOffset = (profit.Key - startDate).Days;
                    for (var i = 1; i < dayOffset; i++)
                    {
                        resultList.Add(string.Format("[{0},{1}]", GetTimestamp(startDate.AddDays(i)), 0));
                    }
                }

                resultList.Add(string.Format("[{0},{1}]", GetTimestamp(profit.Key), (profit.Value / _currencyValue).ToString("F2").Replace(",", ".")));
                tempDate = profit.Key;
            }

            if (tempDate == DateTime.MinValue)
                tempDate = startDate;

            var endDayOffset = (endDate - tempDate).Days;
            for (var i = 1; i <= endDayOffset; i++)
            {
                resultList.Add(string.Format("[{0},'{1}']", GetTimestamp(tempDate.AddDays(i)), 0));
            }

            return String.Join(",", resultList);
        }

        private string GetByWeeks(Dictionary<DateTime, float> list, DateTime startDate, DateTime endDate)
        {
            var resultList = new List<string>();

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            var nextDate = startDate;
            var prevDate = DateTime.MinValue;

            while (nextDate <= endDate)
            {
                var value = list.Where(x => x.Key > prevDate && x.Key <= nextDate).Sum(x => x.Value);
                resultList.Add(string.Format("[{0},{1}]", GetTimestamp(nextDate), (value / _currencyValue).ToString("F2").Replace(",", ".")));

                prevDate = nextDate;
                nextDate = nextDate.DayOfWeek != 0
                            ? nextDate.AddDays(7 - (int) nextDate.DayOfWeek + 1)
                            : nextDate.AddDays(7);
            }

            var lastValue = list.Where(x => x.Key > prevDate && x.Key <= endDate).Sum(x => x.Value);
            resultList.Add(string.Format("[{0},{1}]", GetTimestamp(endDate), (lastValue / _currencyValue).ToString("F2").Replace(",", ".")));
            
            return String.Join(",", resultList);
        }

        private string GetByMonths(Dictionary<DateTime, float> list, DateTime startDate, DateTime endDate)
        {
            var resultList = new List<string>();

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);

            var nextDate = startDate;
            var prevDate = DateTime.MinValue;

            while (nextDate <= endDate)
            {
                var value = list.Where(x => x.Key > prevDate && x.Key <= nextDate).Sum(x => x.Value);
                resultList.Add(string.Format("[{0},{1}]", GetTimestamp(nextDate), (value / _currencyValue).ToString("F2").Replace(",", ".")));

                prevDate = nextDate;
                nextDate = nextDate.AddMonths(1);
                if (nextDate.Day != 1)
                    nextDate = new DateTime(nextDate.Year, nextDate.Month, 1);
            }

            var lastValue = list.Where(x => x.Key > prevDate && x.Key <= endDate).Sum(x => x.Value);
            resultList.Add(string.Format("[{0},{1}]", GetTimestamp(endDate), (lastValue / _currencyValue).ToString("F2").Replace(",", ".")));

            return String.Join(",", resultList);
        }

        #endregion

        protected string RenderLink(int productId, string urlPath, string name, string artno)
        {
            if (urlPath.IsNullOrEmpty() || productId == 0)
                return string.Format("{0} [{1}]", name, artno);

            return string.Format("<a href=\"../{0}\">{1} [{2}]</a>",
                UrlService.GetLink(ParamType.Product, urlPath, productId), name, artno);
        }
    }
}