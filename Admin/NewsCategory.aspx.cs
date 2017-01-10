//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.News;
using Resources;

namespace Admin
{
    public partial class NewsCategories : AdvantShopAdminPage
    {
        private bool _inverseSelection;
        private SqlPaging _paging;
        private InSetFieldFilter _selectionFilter;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            lblError.Visible = false;

            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_Properties_ListPropreties));

            if (!IsPostBack)
            {
                _paging = new SqlPaging { TableName = "[Settings].[NewsCategory]" };

                _paging.AddFieldsRange(new List<Field>
                    {
                        new Field {Name = "NewsCategoryID as ID", IsDistinct = true},
                        new Field {Name = "Name"},
                        new Field {Name = "SortOrder", Sorting = SortDirection.Ascending},
                        new Field {Name = "UrlPath"},
                    });
                grid.ChangeHeaderImageUrl("arrowSortOrder", "images/arrowup.gif");

                _paging.ItemsPerPage = 10;

                pageNumberer.CurrentPageIndex = 1;
                _paging.CurrentPageIndex = 1;
                ViewState["Paging"] = _paging;
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
                    string[] arrids = strIds.Split(' ');

                    var ids = new string[arrids.Length];
                    _selectionFilter = new InSetFieldFilter { IncludeValues = true };
                    for (int idx = 0; idx <= ids.Length - 1; idx++)
                    {
                        int t = int.Parse(arrids[idx]);
                        if (t != -1)
                        {
                            ids[idx] = t.ToString();
                        }
                        else
                        {
                            _selectionFilter.IncludeValues = false;
                            _inverseSelection = true;
                        }
                    }
                    _selectionFilter.Values = ids;
                    //_InverseSelection = If(ids(0) = -1, True, False)
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            //-----Selection filter
            if (String.CompareOrdinal(ddSelect.SelectedIndex.ToString(CultureInfo.InvariantCulture), "0") != 0)
            {
                if (String.CompareOrdinal(ddSelect.SelectedIndex.ToString(CultureInfo.InvariantCulture), "2") == 0)
                {
                    if (_selectionFilter != null)
                    {
                        _selectionFilter.IncludeValues = !_selectionFilter.IncludeValues;
                    }
                    else
                    {
                        _selectionFilter = null; //New InSetFieldFilter()
                        //_SelectionFilter.IncludeValues = True
                    }
                }
                _paging.Fields["ID"].Filter = _selectionFilter;
            }
            else
            {
                _paging.Fields["ID"].Filter = null;
            }

            //----Name filter
            _paging.Fields["Name"].Filter = !string.IsNullOrEmpty(txtName.Text)
                                                ? new CompareFieldFilter { Expression = txtName.Text, ParamName = "@Name" }
                                                : null;

            _paging.Fields["UrlPath"].Filter = !string.IsNullOrEmpty(txtUrlPath.Text)
                                                   ? new CompareFieldFilter { Expression = txtName.Text, ParamName = "@UrlPath" }
                                                   : null;

            _paging.Fields["SortOrder"].Filter = !string.IsNullOrEmpty(txtSort.Text)
                                                     ? new EqualFieldFilter { Value = txtSort.Text, ParamName = "@Sort" }
                                                     : null;

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

        protected void lbDeleteSelected_Click(object sender, EventArgs e)
        {
            if ((_selectionFilter != null) && (_selectionFilter.Values != null))
            {

                if (!_inverseSelection)
                {
                    foreach (var id in _selectionFilter.Values)
                    {
                        NewsService.DeleteNewsCategory(SQLDataHelper.GetInt(id));
                    }
                }
                else
                {
                    var itemsIds = _paging.ItemsIds<int>("NewsCategoryID as ID");
                    foreach (var newscCatId in itemsIds.Where(ncId => !_selectionFilter.Values.Contains(ncId.ToString(CultureInfo.InvariantCulture))))
                    {
                        NewsService.DeleteNewsCategory(newscCatId);
                    }

                }
            }
        }


        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCategory")
            {
                NewsService.DeleteNewsCategory(SQLDataHelper.GetInt(e.CommandArgument));
            }

            if (e.CommandName == "AddCategory")
            {
                try
                {
                    GridViewRow footer = grid.FooterRow;
                    if (!UrlService.IsAvailableUrl(ParamType.NewsCategory, ((TextBox)footer.FindControl("txtNewUrlPath")).Text))
                    {
                        lblError.Text = Resource.Admin_SynonymExist;
                        lblError.Visible = true;
                        return;
                    }

                    var temp = new NewsCategory
                        {
                            Name = ((TextBox)footer.FindControl("txtNewName")).Text,
                            UrlPath = ((TextBox)footer.FindControl("txtNewUrlPath")).Text,
                            SortOrder = ((TextBox)footer.FindControl("txtNewSortOrder")).Text.TryParseInt()
                        };
                    NewsService.InsertNewsCategory(temp);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
                grid.ShowFooter = false;
            }

            if (e.CommandName == "CancelAdd")
            {
                grid.ShowFooter = false;
            }
        }

        protected void grid_Sorting(object sender, GridViewSortEventArgs e)
        {
            var arrows = new Dictionary<string, string>
                {
                    { "Name", "arrowName" }, 
                    { "SortOrder", "arrowSortOrder" },
                    { "UrlPath", "arrowUrlPath" }, 
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

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (grid.UpdatedRow != null)
            {
                try
                {
                    var temp = new NewsCategory
                        {
                            NewsCategoryID = grid.UpdatedRow["ID"].TryParseInt(),
                            Name = grid.UpdatedRow["Name"],
                            UrlPath = grid.UpdatedRow["UrlPath"],
                            SortOrder = grid.UpdatedRow["SortOrder"].TryParseInt()
                        };

                    if (!UrlService.IsAvailableUrl(temp.NewsCategoryID, ParamType.NewsCategory, temp.UrlPath))
                    {
                        lblError.Text = Resource.Admin_SynonymExist;
                        lblError.Visible = true;
                        return;
                    }

                    NewsService.UpdateNewsCategory(temp);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

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
        }

        protected void grid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
        }

        private void grid_DataBound(object sender, EventArgs e)
        {
            if (grid.ShowFooter)
            {
                grid.FooterRow.FindControl("txtNewName").Focus();
            }
        }
    }
}