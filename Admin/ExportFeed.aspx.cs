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
using AdvantShop.Catalog;
using AdvantShop.Controls;
using AdvantShop.Core.SQL;
using AdvantShop.ExportImport;
using AdvantShop.Helpers;
using Resources;

namespace Admin
{
    public partial class ExportFeedPage : AdvantShopAdminPage
    {
        SqlPaging _paging;
        InSetFieldFilter _selectionFilter;
        bool _inverseSelection;
        private const string ExportFeedNew = "ExportFeed.aspx";
        private int _catId = 0;

        public string ModuleName
        {
            get { return Request["moduleid"] ?? ""; }
        }

        public ExportFeedPage()
        {
            _inverseSelection = false;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", AdvantShop.Configuration.SettingsMain.ShopName, Resource.Admin_MasterPageAdminCatalog_Catalog));
            if (string.IsNullOrEmpty(ModuleName))
            {
                Response.Redirect("ExportFeed.aspx?ModuleId=YandexMarket");
                return;
            }

            YaHelp.Visible = (ModuleName == "YandexMarket");
            CsvHelp.Visible = (ModuleName == "CsvExport");

            if (!string.IsNullOrEmpty(Request["CatId"]))
            {
                Int32.TryParse(Request["CatId"], out _catId);
            }
            Category cat = CategoryService.GetCategory(_catId);
            if (cat != null)
            {
                lblCategoryName.Text = cat.Name;
                sn.BuildNavigationAdmin(_catId);
            }
            if (!Page.IsPostBack)
            {
                var flag = ExportFeedService.CheckCategoryHierical(ModuleName, _catId);
                pnlCategorySet.Enabled = !flag;
                chbFull.Checked = ExportFeedService.CheckCategory(ModuleName, _catId);
                pnlData.Enabled = !chbFull.Checked && !flag;
            }
            PageSubheader.Visible = true;
            ModuleNameLiteral.Text = ModuleName;

            if (!IsPostBack)
            {
                var rootCategory = CategoryService.GetCategory(0);
                var node2 = new TreeNode { Text = SelectCategory(0, ModuleName, rootCategory != null ? rootCategory.Name : Resource.Admin_m_Category_Root), Value = "0", Selected = true, NavigateUrl = ExportFeedNew + "?moduleid=" + ModuleName };

                tree2.Nodes.Add(node2);
                LoadChildCategories2(tree2.Nodes[0]);


                _paging = new SqlPaging
                    {
                        TableName = "[Catalog].[Product] left JOIN [Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] INNER JOIN Catalog.ProductCategories on ProductCategories.ProductId = [Product].[ProductID] and ProductCategories.Main=1",
                        ItemsPerPage = 100
                    };

                var f = new Field { Name = "Product.ProductId as ID", IsDistinct = true };
                _paging.AddField(f);

                f = new Field { Name = "Product.ArtNo" };
                _paging.AddField(f);

                f = new Field { Name = "Name" };
                _paging.AddField(f);

                f = new Field { Name = "(Select count(*) from Settings.ExportFeedSelectedProducts where ModuleName=@ModuleName and ExportFeedSelectedProducts.ProductID=Product.ProductId) as Cheaked" };
                _paging.AddField(f);

                var pf = new EqualFieldFilter { Value = _catId.ToString(), ParamName = "@Parent" };
                f = new Field { Name = "CategoryId", Filter = pf };
                _paging.AddField(f);

                f = new Field { Name = "SortOrder", Sorting = SortDirection.Ascending };
                _paging.AddField(f);

                _paging.AddParam(new SqlParam { ParameterName = "@ModuleName", Value = ModuleName });

                _paging.ExtensionWhere = ModuleName != "CsvExport" ? "and Offer.Price > 0 and ((Select Max(Amount) from Catalog.Offer where Offer.ProductID = Product.ProductID) > 0 or Product.AllowPreorder=1) and CategoryEnabled=1 and Enabled=1" : "";
                grid.ChangeHeaderImageUrl("arrowName", "images/arrowup.gif");

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
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {

            //-----Selection filter
            if (string.Compare(ddSelect.SelectedIndex.ToString(), "0") != 0)
            {

                if (string.Compare(ddSelect.SelectedIndex.ToString(), "2") == 0)
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

            if (!string.IsNullOrEmpty(txtArtNo.Text))
            {
                var nfilter = new CompareFieldFilter { Expression = txtArtNo.Text, ParamName = "@ArtNo" };
                _paging.Fields["ArtNo"].Filter = nfilter;
            }
            else
            {
                _paging.Fields["Name"].Filter = null;
            }

            //----Name filter
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                var nfilter = new CompareFieldFilter { Expression = txtName.Text, ParamName = "@Name" };
                _paging.Fields["Name"].Filter = nfilter;
            }
            else
            {
                _paging.Fields["Name"].Filter = null;
            }


            //---Sort filter
            if (!string.IsNullOrEmpty(txtSort.Text))
            {
                var nfilter = new CompareFieldFilter
                    {
                        Expression = txtSort.Text,
                        ParamName = "@SortOrder"
                    };
                _paging.Fields["SortOrder"].Filter = nfilter;
            }
            else
            {
                _paging.Fields["SortOrder"].Filter = null;
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

        protected void lbDeleteSelected_Click(object sender, EventArgs e)
        {
            if ((_selectionFilter != null) && (_selectionFilter.Values != null))
            {
                if (!_inverseSelection)
                {
                    foreach (var id in _selectionFilter.Values)
                    {
                        //CityService.DeleteCity(SQLDataHelper.GetInt(id));
                    }
                }
                else
                {
                    var itemsIds = _paging.ItemsIds<int>("Product.ProductId as ID");
                    foreach (var id in itemsIds.Where(id => !_selectionFilter.Values.Contains(id.ToString(CultureInfo.InvariantCulture))))
                    {
                        //CityService.DeleteCity(id);
                    }
                }
            }
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void grid_Sorting(object sender, GridViewSortEventArgs e)
        {
            var arrows = new Dictionary<string, string>
                {
                    {"Name", "arrowName"},
                    {"SortOrder", "arrowSortOrder"},
                    {"ArtNo", "arrowArtNo"},
                    {"Cheaked","arrowCheaked"}
                };
            const string urlArrowUp = "images/arrowup.gif";
            const string urlArrowDown = "images/arrowdown.gif";
            const string urlArrowGray = "images/arrowdownh.gif";


            Field csf = (from Field f in _paging.Fields.Values where f.Sorting.HasValue select f).First();
            Field nsf = _paging.Fields[e.SortExpression];

            if (nsf.Name.Equals(csf.Name))
            {
                csf.Sorting = csf.Sorting == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
                grid.ChangeHeaderImageUrl(arrows[csf.Name], (csf.Sorting == SortDirection.Ascending ? urlArrowUp : urlArrowDown));
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
            var parentCategories = CategoryService.GetParentCategories(_catId);
            parentCategories.Add(new Category { CategoryId = 0 });
            var nodes = tree2.Nodes;
            for (var i = parentCategories.Count - 1; i >= 0; i--)
            {
                var ii = i;
                var tn = (from TreeNode n in nodes where n.Value == parentCategories[ii].CategoryId.ToString() select n).SingleOrDefault();
                if (tn == null) continue;
                if (i == 0)
                {
                    tn.Select();
                    tn.Expand();
                }
                else
                {
                    tn.Expand();
                }
                nodes = tn.ChildNodes;
            }


            if (grid.UpdatedRow != null)
            {
                var flag = SQLDataHelper.GetBoolean(grid.UpdatedRow["Cheaked"]);

                if (flag)
                    ExportFeedService.InsertProduct(ModuleName, SQLDataHelper.GetInt(grid.UpdatedRow["ID"]));
                else
                    ExportFeedService.DeleteProduct(ModuleName, SQLDataHelper.GetInt(grid.UpdatedRow["ID"]));
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
                    if (Array.Exists(_selectionFilter.Values, c => c == data.Rows[intIndex]["ID"].ToString()))
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

        public void PopulateNode2(object sender, TreeNodeEventArgs e)
        {
            LoadChildCategories2(e.Node);
        }

        private void LoadChildCategories2(TreeNode node)
        {
            foreach (var c in CategoryService.GetChildCategoriesByCategoryId(SQLDataHelper.GetInt(node.Value), false))
            {
                var newNode = new TreeNode
                    {
                        Text = SelectCategory(c.CategoryId, ModuleName, c.Name),
                        Value = c.CategoryId.ToString(),
                        NavigateUrl = ExportFeedNew + "?moduleid=" + ModuleName + "&CatId=" + c.CategoryId
                    };
                if (c.HasChild)
                {
                    newNode.Expanded = false;
                    newNode.PopulateOnDemand = true;
                }
                else
                {
                    newNode.Expanded = true;
                    newNode.PopulateOnDemand = false;
                }
                node.ChildNodes.Add(newNode);
            }
        }

        protected void btnChange_OnClick(object sender, EventArgs e)
        {
            if (chbFull.Checked)
                ExportFeedService.InsertCategory(ModuleName, _catId);
            else
                ExportFeedService.DeleteCategory(ModuleName, _catId);

            Response.Redirect("ExportFeed.aspx?moduleid=" + ModuleName + "&CatId=" + _catId);
        }

        protected void lbSetActive_Click(object sender, EventArgs e)
        {
            if ((_selectionFilter != null) && (_selectionFilter.Values != null))
            {
                if (!_inverseSelection)
                {
                    foreach (var id in _selectionFilter.Values)
                    {
                        ExportFeedService.InsertProduct(ModuleName, SQLDataHelper.GetInt(id));
                    }
                }
                else
                {
                    var itemsIds = _paging.ItemsIds<int>("Product.ProductId as ID");
                    foreach (var id in itemsIds.Where(id => !_selectionFilter.Values.Contains(id.ToString(CultureInfo.InvariantCulture))))
                    {
                        ExportFeedService.InsertProduct(ModuleName, SQLDataHelper.GetInt(id));
                    }
                }
            }
        }

        protected void lbSetNotActive_Click(object sender, EventArgs e)
        {
            if ((_selectionFilter != null) && (_selectionFilter.Values != null))
            {
                if (!_inverseSelection)
                {
                    foreach (var id in _selectionFilter.Values)
                    {
                        ExportFeedService.DeleteProduct(ModuleName, SQLDataHelper.GetInt(id));
                    }
                }
                else
                {
                    var itemsIds = _paging.ItemsIds<int>("Product.ProductId as ID");
                    foreach (var id in itemsIds.Where(id => !_selectionFilter.Values.Contains(id.ToString(CultureInfo.InvariantCulture))))
                    {
                        ExportFeedService.DeleteProduct(ModuleName, SQLDataHelper.GetInt(id));
                    }
                }
            }
        }

        private string SelectCategory(int catId, string moduleName, string message)
        {
            if (ExportFeedService.CheckCategoryHierical(moduleName, catId) || ExportFeedService.CheckCategory(moduleName, catId))
                return "<span style='color: blue;font-weight:bold;'>" + message + "</span>";
            return message;
        }

        protected void btnResetExport_OnClick(object sender, EventArgs e)
        {
            ExportFeedService.DeleteModule(ModuleName);
            Response.Redirect("ExportFeed.aspx?moduleid=" + ModuleName + "&CatId=" + _catId);
        }
    }
}