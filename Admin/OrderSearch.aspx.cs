//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using Resources;
using AdvantShop.Core.SQL;

namespace Admin
{
    public partial class OrderSearch : AdvantShopAdminPage
    {
        private bool _inverseSelection;
        private SqlPaging _paging;
        private InSetFieldFilter _selectionFilter;

        private void MsgErr(bool clean)
        {
            if (clean)
            {
                Message.Visible = false;
                Message.Text = "";
            }
            else
            {
                Message.Visible = false;
            }
        }

        private void MsgErr(string messageText)
        {
            Message.Visible = true;
            Message.Text = "<br/>" + messageText;
        }


        protected void FillStatusFilter()
        {
            ddlStatusName.Items.Clear();
            ddlStatusName.Items.Add(new ListItem(Resource.Admin_Catalog_Any, string.Empty));
            foreach (var status in OrderService.GetOrderStatuses())
            {
                ddlStatusName.Items.Add(new ListItem(status.StatusName, status.StatusID.ToString()));
            }
        }

        protected void FillPaymentMethodFilter()
        {
            ddlPaymentMethod.Items.Clear();
            ddlPaymentMethod.Items.Add(new ListItem(Resource.Admin_Catalog_Any, "any"));
            foreach (string str in GetPaymentMethods())
            {
                ddlPaymentMethod.Items.Add(str);
            }
        }

        protected void FillShippingMethodFilter()
        {
            ddlShippingMethod.Items.Clear();
            ddlShippingMethod.Items.Add(new ListItem(Resource.Admin_Catalog_Any, "any"));
            foreach (string str in OrderService.GetShippingMethodNamesFromOrder())
            {
                ddlShippingMethod.Items.Add(str);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            MsgErr(true);
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_OrderSearch_SubHeader));

            if (!IsPostBack)
            {
                FillStatusFilter();
                FillPaymentMethodFilter();
                FillShippingMethodFilter();

                _paging = new SqlPaging
                {
                    TableName =
                        "[Order].[Order] " +
                        "LEFT JOIN [Order].[OrderCustomer] ON [Order].[OrderID]=[OrderCustomer].[OrderID] " +
                        "LEFT JOIN [Order].[OrderStatus] ON [OrderStatus].[OrderStatusID]=[Order].[OrderStatusID] " +
                        "LEFT JOIN [Order].[OrderCurrency] ON [Order].[OrderID] = [OrderCurrency].[OrderID] " +
                        "LEFT JOIN [Order].[OrderContact] ON [Order].[BillingContactID] = [OrderContact].[OrderContactID] " +
                        "LEFT JOIN [Order].[PaymentMethod] ON [Order].[PaymentMethodID] = [Order].[PaymentMethod].[PaymentMethodID] " +
                        "LEFT JOIN [Order].[ShippingMethod] ON [Order].[ShippingMethodID] = [Order].[ShippingMethod].ShippingMethodID",
                    ItemsPerPage = 10
                };

                _paging.AddFieldsRange(
                    new Field("[Order].OrderID as ID") { IsDistinct = true },
                    new Field("StatusName"),
                    new Field("[Order].[OrderStatusID] as OrderStatusID"),
                    new Field("PaymentDate"),
                    new Field("PaymentMethodName"),
                    new Field("ShippingMethodName"),
                    new Field("FirstName + ' ' + LastName as BuyerName"),
                    new Field("OrderContact.Name as BillingName"),
                    new Field("PaymentMethod.Name as PaymentMethod"),
                    new Field("ShippingMethod.Name as ShippingMethod"),
                    new Field("Sum"),
                    new Field("OrderDate") { Sorting = SortDirection.Descending },
                    new Field("CurrencyValue"),
                    new Field("CurrencyCode"),
                    new Field("CustomerID"),
                    new Field("Color"));


                _paging.ItemsPerPage = 10;

                pageNumberer.CurrentPageIndex = 1;
                _paging.CurrentPageIndex = 1;
                ViewState["Paging"] = _paging;

                grid.ChangeHeaderImageUrl("arrowOrderID", "images/arrowdown.gif");

                if (Request["filter"] != null)
                {
                    if (Request["filter"] == "lastmonth")
                    {
                        txtDateFrom.Text = DateTime.Now.Date.AddDays(-1).ToString("dd.MM.yyyy");
                        txtDateTo.Text = DateTime.Now.Date.AddMonths(1).ToString("dd.MM.yyyy");
                    }
                    if (Request["filter"] == "today")
                    {
                        txtDateFrom.Text = DateTime.Now.Date.ToString("dd.MM.yyyy");
                        txtDateTo.Text = DateTime.Now.Date.ToString("dd.MM.yyyy");

                    }
                    if (Request["filter"] == "yesterday")
                    {
                        txtDateFrom.Text = DateTime.Now.Date.AddDays(-1).ToString("dd.MM.yyyy");
                        txtDateTo.Text = DateTime.Now.Date.AddDays(-1).ToString("dd.MM.yyyy");
                    }
                    btnFilter_Click(sender, e);
                }


                if (Request["search"].IsNotEmpty())
                {
                    var order = OrderService.GetOrder(Request["search"].TryParseInt()) ?? OrderService.GetOrderByNumber(Request["search"]);
                    if (order != null)
                    {
                        Response.Redirect("ViewOrder.aspx?orderID=" + order.OrderID);
                        return;
                    }

                    txtBuyerName.Text = Request["search"];

                    btnFilter_Click(null, null);
                }


                ddlStatus.DataBind();
            }
            else
            {
                _paging = (SqlPaging)(ViewState["Paging"]);
                _paging.ItemsPerPage = SQLDataHelper.GetInt(ddRowsPerPage.SelectedValue);

                if (_paging == null)
                {
                    throw (new Exception("Paging lost"));
                }

                string strIds = Request.Form["SelectedIds"];

                if (!string.IsNullOrEmpty(strIds))
                {
                    strIds = strIds.Trim();
                    var arrids = strIds.Split(' ');

                    var ids = new string[arrids.Length];

                    _selectionFilter = new InSetFieldFilter { IncludeValues = true };
                    for (int idx = 0; idx <= ids.Length - 1; idx++)
                    {
                        var t = arrids[idx];
                        if (t.Replace(" ", "") != "-1")
                        {
                            ids[idx] = t;
                        }
                        else
                        {
                            _selectionFilter.IncludeValues = false;
                            _inverseSelection = true;
                        }
                    }

                    _selectionFilter.Values = ids;
                }
            }

            if (!IsPostBack && !string.IsNullOrEmpty(Request["status"]) && ddlStatusName.Items.FindByValue(Request["status"].ToLower()) != null)
            {
                ddlStatusName.SelectedValue = Request["status"].ToLower();
                btnFilter_Click(new object(), new EventArgs());
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            DataTable data = _paging.PageItems;
            while (data.Rows.Count < 1 && _paging.CurrentPageIndex > 1)
            {
                _paging.CurrentPageIndex--;
                data = _paging.PageItems;
            }

            var clmn = new DataColumn("IsSelected", typeof(bool)) { DefaultValue = _inverseSelection };
            data.Columns.Add(clmn);
            if ((_selectionFilter != null) && (_selectionFilter.Values != null))
            {
                for (int i = 0; i <= data.Rows.Count - 1; i++)
                {
                    int intIndex = i;
                    if (Array.Exists(_selectionFilter.Values, c => c == (data.Rows[intIndex]["ID"]).ToString()))
                    {
                        data.Rows[i]["IsSelected"] = !_inverseSelection;
                    }
                }
            }

            if (data.Rows.Count < 1)
            {
                goToPage.Visible = false;
            }

            grid.DataSource = data;
            grid.DataBind();

            pageNumberer.PageCount = _paging.PageCount;
            lblFound.Text = _paging.TotalRowsCount.ToString();
            lblTotal.Text = CatalogService.GetStringPrice(_paging.GetCustomData("Sum ([order].[Sum]) as totalPrice", "",
                            reader => SQLDataHelper.GetFloat(reader, "totalPrice")).FirstOrDefault());
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            var flogical = new LogicalFilter();
            //-----Selection filter
            if (ddSelect.SelectedIndex != 0)
            {
                if (ddSelect.SelectedIndex == 2)
                {
                    if (_selectionFilter != null)
                    {
                        _selectionFilter.IncludeValues = !_selectionFilter.IncludeValues;
                    }
                    else
                    {
                        _selectionFilter = null;
                    }
                }
                flogical.AddFilter(_selectionFilter);
            }

            //----OrderID filter
            if (!string.IsNullOrEmpty(txtOrderID.Text))
            {
                flogical.AddLogicalOperation("AND");
                flogical.AddFilter(new EqualFieldFilter { Value = txtOrderID.Text.TryParseInt().ToString(), ParamName = "@OrderId" });
            }


            _paging.Fields["ID"].Filter = flogical.FilterCount() > 0 ? flogical : null;

            //----Status filter
            if (!string.IsNullOrEmpty(ddlStatusName.SelectedValue))
            {
                if (ddlStatusName.SelectedIndex != 0)
                {
                    _paging.Fields["OrderStatusID"].Filter = new EqualFieldFilter()
                    {
                        ParamName = "@StatusId",
                        Value = ddlStatusName.SelectedValue
                    };
                }
                else
                {
                    _paging.Fields["OrderStatusID"].Filter = null;
                }
            }
            else
            {
                _paging.Fields["OrderStatusID"].Filter = null;
            }


            //----Sum filter
            var sumFilter = new RangeFieldFilter { ParamName = "@Sum" };

            try
            {
                int priceFrom = 0;
                sumFilter.From = int.TryParse(txtSumFrom.Text, out priceFrom) ? priceFrom : 0;
            }
            catch (Exception)
            {
            }

            try
            {
                int priceTo = 0;
                sumFilter.To = int.TryParse(txtSumTo.Text, out priceTo) ? priceTo : int.MaxValue;
            }
            catch (Exception)
            {
            }

            _paging.Fields["Sum"].Filter = sumFilter.From.HasValue || sumFilter.To.HasValue ? sumFilter : null;


            //---- Name filter
            _paging.Fields["BuyerName"].Filter = !string.IsNullOrEmpty(txtBuyerName.Text) ? new CompareFieldFilter { ParamName = "@BuyerName", Expression = txtBuyerName.Text } : null;

            //---- Billing Name filter
            //_paging.Fields["BillingName"].Filter = !string.IsNullOrEmpty(txtBillingName.Text) ? new CompareFieldFilter { ParamName = "@BillingName", Expression = txtBillingName.Text } : null;

            _paging.Fields["PaymentMethod"].Filter = ddlPaymentMethod.SelectedValue != "any"
                                                         ? new EqualFieldFilter
                                                             {
                                                                 ParamName = "@PaymentMethod",
                                                                 Value = ddlPaymentMethod.SelectedValue
                                                             }
                                                         : null;
            _paging.Fields["ShippingMethodName"].Filter = ddlShippingMethod.SelectedValue != "any"
                                                              ? new EqualFieldFilter
                                                                  {
                                                                      ParamName = "@ShippingMethodName",
                                                                      Value = ddlShippingMethod.SelectedValue
                                                                  }
                                                              : null;

            _paging.Fields["PaymentDate"].Filter = ddlPayed.SelectedValue != "any"
                                                       ? ddlPayed.SelectedValue == "yes"
                                                             ? new NullFieldFilter { ParamName = "@PaymentDate", Null = false }
                                                             : new NullFieldFilter { ParamName = "@PaymentDate", Null = true }
                                                       : null;
            //---OrderDate filter
            var dfilter = new DateTimeRangeFieldFilter { ParamName = "@dateOrdSearch" };
            if (!string.IsNullOrEmpty(txtDateFrom.Text))
            {
                DateTime? d = null;
                try
                {
                    d = DateTime.Parse(txtDateFrom.Text);
                }
                catch (Exception)
                {
                }
                if (d.HasValue)
                {
                    var dt = new DateTime(d.Value.Year, d.Value.Month, d.Value.Day, 0, 0, 0, 0);
                    dfilter.From = dt;
                }
            }

            if (!string.IsNullOrEmpty(txtDateTo.Text))
            {
                DateTime? d = null;
                try
                {
                    d = DateTime.Parse(txtDateTo.Text);
                }
                catch (Exception)
                {
                }
                if (d.HasValue)
                {
                    var dt = new DateTime(d.Value.Year, d.Value.Month, d.Value.Day, 23, 59, 59, 99);
                    dfilter.To = dt;
                }
            }

            if (dfilter.From.HasValue || dfilter.To.HasValue)
            {
                _paging.Fields["OrderDate"].Filter = dfilter;
            }
            else
            {
                _paging.Fields["OrderDate"].Filter = null;
            }
            pageNumberer.CurrentPageIndex = 1;
            _paging.CurrentPageIndex = 1;

        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            btnFilter_Click(sender, e);
            grid.ChangeHeaderImageUrl(null, null);
        }

        protected void pn_SelectedPageChanged(object sender, EventArgs e)
        {
            _paging.CurrentPageIndex = pageNumberer.CurrentPageIndex;
        }

        protected void linkGO_Click(object sender, EventArgs e)
        {
            int pagen;
            try
            {
                pagen = int.Parse(txtPageNum.Text);
            }
            catch (Exception)
            {
                pagen = -1;
            }
            if (pagen >= 1 && pagen <= _paging.PageCount)
            {
                pageNumberer.CurrentPageIndex = pagen;
                _paging.CurrentPageIndex = pagen;
            }
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                OrderService.DeleteOrder(int.Parse(e.CommandArgument.ToString()));
            }
        }

        protected void grid_Sorting(object sender, GridViewSortEventArgs e)
        {
            var arrows = new Dictionary<string, string>
                {
                    {"ID", "arrowOrderID"},
                    {"StatusName", "arrowStatusName"},
                    {"BillingName", "arrowBillingName"},
                    {"PaymentMethod", "arrowPaymentMethod"},
                    {"ShippingMethodName", "arrowShippingMethod"},
                    {"PaymentDate","arrowPay"},
                    {"Sum", "arrowSum"},
                    {"OrderDate", "arrowOrderDate"}
                };

            const string urlArrowUp = "images/arrowup.gif";
            const string urlArrowDown = "images/arrowdown.gif";
            const string urlArrowGray = "images/arrowdownh.gif";

            Field csf = (from Field f in _paging.Fields.Values where f.Sorting.HasValue select f).First();
            Field nsf = _paging.Fields[e.SortExpression];

            if (nsf.Name.Equals(csf.Name))
            {
                csf.Sorting = csf.Sorting == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
                grid.ChangeHeaderImageUrl(arrows[csf.Name],
                                          (csf.Sorting == SortDirection.Ascending ? urlArrowUp : urlArrowDown));
            }
            else
            {
                csf.Sorting = null;

                grid.ChangeHeaderImageUrl(arrows[csf.Name], urlArrowGray);

                nsf.Sorting = SortDirection.Ascending;
                grid.ChangeHeaderImageUrl(arrows[nsf.Name], urlArrowUp);
            }

            pageNumberer.CurrentPageIndex = 1;
            _paging.CurrentPageIndex = 1;
        }

        protected void DSStatus_Init(object sender, EventArgs e)
        {
            DSStatus.ConnectionString = Connection.GetConnectionString();
        }

        public static List<string> GetPaymentMethods()
        {
            var result = new List<string>();
            try
            {
                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandText = "SELECT distinct Name FROM [Order].[PaymentMethod]";
                    db.cmd.CommandType = CommandType.Text;
                    db.cnOpen();

                    using (var reader = db.cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(SQLDataHelper.GetString(reader, "Name").Trim());
                        }
                    }
                    db.cnClose();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return result;
        }

        protected string RenderSum(double sum, double rate, string currency)
        {
            return CatalogService.GetStringPrice((float)sum, (float)rate, currency);
        }

        protected void lbDeleteSelected_Click(object sender, EventArgs e)
        {
            if ((_selectionFilter != null) && (_selectionFilter.Values != null))
            {
                if (!_inverseSelection)
                {
                    foreach (var id in _selectionFilter.Values)
                    {
                        OrderService.DeleteOrder(SQLDataHelper.GetInt(id));
                    }
                }
                else
                {
                    var itemsIds = _paging.ItemsIds<int>("[Order].OrderID as ID");
                    foreach (int id in itemsIds.Where(iId => !_selectionFilter.Values.Contains(iId.ToString())))
                    {
                        OrderService.DeleteOrder(id);
                    }
                }
            }
        }


        protected void lbSetPay_Click(object sender, EventArgs e)
        {
            if ((_selectionFilter != null) && (_selectionFilter.Values != null))
            {
                if (!_inverseSelection)
                {
                    foreach (var id in _selectionFilter.Values)
                    {
                        OrderService.PayOrder(SQLDataHelper.GetInt(id), true);
                    }
                }
                else
                {
                    var itemsIds = _paging.ItemsIds<int>("[Order].OrderID as ID");
                    foreach (int id in itemsIds.Where(oId => !_selectionFilter.Values.Contains(oId.ToString())))
                    {
                        OrderService.PayOrder(SQLDataHelper.GetInt(id), true);
                    }
                }
            }
        }

        protected void lbSetNotPay_Click(object sender, EventArgs e)
        {
            if ((_selectionFilter != null) && (_selectionFilter.Values != null))
            {
                if (!_inverseSelection)
                {
                    foreach (var id in _selectionFilter.Values)
                    {
                        OrderService.PayOrder(SQLDataHelper.GetInt(id), false);
                    }
                }
                else
                {
                    var itemsIds = _paging.ItemsIds<int>("[Order].OrderID as ID");
                    foreach (int id in itemsIds.Where(oId => !_selectionFilter.Values.Contains(oId.ToString())))
                    {
                        OrderService.PayOrder(SQLDataHelper.GetInt(id), false);
                    }
                }
            }
        }

        protected void lbChangeStatus_Click(object sender, EventArgs e)
        {
            if ((_selectionFilter != null) && (_selectionFilter.Values != null))
            {
                if (!_inverseSelection)
                {
                    foreach (var id in _selectionFilter.Values)
                    {
                        var oldStatus = OrderService.GetOrderStatusId(int.Parse(id));
                        OrderService.ChangeOrderStatus(int.Parse(id), int.Parse(ddlStatus.SelectedValue));
                        if (oldStatus != int.Parse(ddlStatus.SelectedValue))
                        {
                            OrderService.UpdateStatusComment(int.Parse(id), string.Empty);
                        }
                    }
                }
                else
                {
                    var itemsIds = _paging.ItemsIds<int>("[Order].OrderID as ID");
                    foreach (int id in itemsIds.Where(oId => !_selectionFilter.Values.Contains(oId.ToString())))
                    {
                        var oldStatus = OrderService.GetOrderStatusId(id);
                        OrderService.ChangeOrderStatus(id, int.Parse(ddlStatus.SelectedValue));
                        if (oldStatus != int.Parse(ddlStatus.SelectedValue))
                        {
                            OrderService.UpdateStatusComment(id, string.Empty);
                        }
                    }
                }

                IEnumerable<string> ids = _inverseSelection
                                              ? OrderService.GetAllOrders().Select(o => o.OrderID.ToString()).Where(
                                                  oId => !_selectionFilter.Values.Contains(oId))
                                              : _selectionFilter.Values.ToList();
            }
        }
        protected string SelectedCount()
        {
            if (_inverseSelection)
                return lblFound.Text;
            if (_selectionFilter != null && _selectionFilter.Values != null)
                return _selectionFilter.Values.Length.ToString();
            return "0";
        }

        protected void grid_OnDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((LinkButton)e.Row.FindControl("buttonDelete")).Attributes["data-confirm"] =
                    string.Format(Resource.Admin_OrderSearch_DeleteOrder, ((DataRowView)e.Row.DataItem)["Id"]);
            }
        }
    }
}