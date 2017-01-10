//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using Resources;

namespace Admin
{
    public partial class Menu : AdvantShopAdminPage
    {
        private int _menuId = 0;
        private bool _needReloadTree;
        private MenuService.EMenuType _menuType = MenuService.EMenuType.Top;
        public MenuService.EMenuType MenuType
        {
            get { return _menuType; }
            set { _menuType = value; }
        }

        private InSetFieldFilter _selectionFilter;
        private bool _inverseSelection;
        private SqlPaging _paging;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_MenuManager_TopMenu));

            Int32.TryParse(Request["menuid"], out _menuId);
            _needReloadTree = SQLDataHelper.GetBoolean(ViewState["updateTree"]);

            hlEditCategory.Visible = _menuId > 0;
            lblSeparator.Visible = _menuId > 0;
            hlDeleteCategory.Visible = _menuId > 0;

            if (!string.IsNullOrWhiteSpace(Request["type"]))
                Enum.TryParse(Request["type"], true, out _menuType);

            var menuitem = new AdvMenuItem();
            switch (_menuType)
            {
                case MenuService.EMenuType.Top:
                    menuitem = MenuService.GetMenuItemById(_menuId, _menuType);

                    lblHead.Text = menuitem == null
                                       ? Resource.Admin_MenuManager_TopMenu
                                       : string.Format("{0} - {1}", Resource.Admin_MenuManager_TopMenu, menuitem.MenuItemName);
                    lblSubHead.Text = Resource.Admin_MenuManager_SubHeaderTop;

                    Page.Title = menuitem == null
                                     ? Resource.Admin_MenuManager_TopMenu
                                     : string.Format("{0} - {1}", Resource.Admin_MenuManager_TopMenu, menuitem.MenuItemName);

                    break;
                case MenuService.EMenuType.Bottom:
                    menuitem = MenuService.GetMenuItemById(_menuId, _menuType);

                    lblHead.Text = menuitem == null
                                       ? Resource.Admin_MenuManager_BottomMenu
                                       : string.Format("{0} - {1}", Resource.Admin_MenuManager_BottomMenu, menuitem.MenuItemName);
                    lblSubHead.Text = Resource.Admin_MenuManager_SubHeaderBottom;

                    Page.Title = menuitem == null
                                     ? Resource.Admin_MenuManager_BottomMenu
                                     : string.Format("{0} - {1}", Resource.Admin_MenuManager_BottomMenu, menuitem.MenuItemName);
                    break;
            }

            btnAdd.OnClientClick = "open_window('m_Menu.aspx?MenuID=" + _menuId + "&mode=create&type=" + _menuType + "', 750, 640);return false;";
            hlEditCategory.NavigateUrl = "javascript:open_window(\'m_Menu.aspx?MenuID=" + _menuId + "&mode=edit&type=" + _menuType + "\', 750, 640)";
            hlDeleteCategory.Attributes["data-confirm"] =
                string.Format(Resource.Admin_MasterPageAdminCatalog_MenuConfirmation, menuitem != null ? menuitem.MenuItemName : string.Empty);

            if (!IsPostBack)
            {
                switch (_menuType)
                {
                    case MenuService.EMenuType.Top:
                        _paging = new SqlPaging
                            {
                                TableName = "[CMS].[MainMenu]",
                                ItemsPerPage = 10
                            };
                        break;
                    case MenuService.EMenuType.Bottom:
                        _paging = new SqlPaging
                            {
                                TableName = "[CMS].[BottomMenu]",
                                ItemsPerPage = 10
                            };
                        break;
                }

                _paging.AddFieldsRange(new List<Field>
                    {
                        new Field
                            {
                                Name = "MenuItemID as ID",
                                IsDistinct = true
                            },
                        new Field {Name = "MenuItemParentID" },
                        new Field {Name = "MenuItemName"},
                        new Field {Name = "Enabled"},
                        new Field {Name = "Blank"},
                        new Field {Name = "SortOrder",Sorting = SortDirection.Ascending}
                    });

                if (_menuId != 0)
                {
                    var filter = new EqualFieldFilter { ParamName = "@MenuItemParentID", Value = _menuId.ToString() };
                    _paging.Fields["MenuItemParentID"].Filter = filter;
                }
                else
                {
                    var filter = new NullFieldFilter { ParamName = "@MenuItemParentID", Null = true };
                    _paging.Fields["MenuItemParentID"].Filter = filter;
                }

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

                    _selectionFilter = new InSetFieldFilter();
                    if (arrids.Contains("-1"))
                    {
                        _selectionFilter.IncludeValues = false;
                        _inverseSelection = true;
                    }
                    else
                    {
                        _selectionFilter.IncludeValues = true;
                    }
                    _selectionFilter.Values = arrids.Where(id => id != "-1").ToArray();
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack || _needReloadTree)
            {
                tree.Nodes.Clear();
                treeBottom.Nodes.Clear();

                tree.Nodes.Add(new ButtonTreeNodeMenu
                    {
                        NavigateUrl = "Menu.aspx?type=Top",
                        Text = Resource.Admin_MenuManager_TopMenu,
                        Value = "0",
                        TreeView = tree,
                        MenuType = MenuService.EMenuType.Top
                    });
                treeBottom.Nodes.Add(new ButtonTreeNodeMenu
                    {
                        NavigateUrl = "Menu.aspx?type=Bottom",
                        Text = Resource.Admin_MenuManager_BottomMenu,
                        Value = @"0",
                        TreeView = treeBottom,
                        MenuType = MenuService.EMenuType.Bottom
                    });

                LoadRootMenuItems(tree.Nodes[0].ChildNodes, MenuService.EMenuType.Top);
                LoadRootMenuItems(treeBottom.Nodes[0].ChildNodes, MenuService.EMenuType.Bottom);

                if (_menuId == 0)
                {
                    if (_menuType == MenuService.EMenuType.Top && tree.Nodes.Count > 0)
                    {
                        tree.Nodes[0].Select();
                    }
                    else if (_menuType == MenuService.EMenuType.Bottom && treeBottom.Nodes.Count > 0)
                    {
                        treeBottom.Nodes[0].Select();
                    }
                }
                else
                {
                    TreeNodeCollection nodes;
                    switch (_menuType)
                    {
                        case MenuService.EMenuType.Top:
                            nodes = tree.Nodes[0].ChildNodes;
                            break;
                        case MenuService.EMenuType.Bottom:
                            nodes = treeBottom.Nodes[0].ChildNodes;
                            break;
                        default:
                            nodes = new TreeNodeCollection();
                            break;
                    }

                    bool isFirstNode = true;
                    foreach (var parentMenuItem in MenuService.GetParentMenuItems(_menuId, _menuType))
                    {
                        var tn =
                            (from TreeNode n in nodes where n.Value == parentMenuItem.ToString() select n).
                                SingleOrDefault();

                        if (tn != null)
                        {
                            tn.Selected = isFirstNode;
                            isFirstNode = false;
                            tn.Expand();
                            nodes = tn.ChildNodes;
                        }
                    }
                }
            }

            if (grid.UpdatedRow != null)
            {
                if (_menuType == MenuService.EMenuType.Top)
                {
                    CacheManager.RemoveByPattern(CacheNames.GetMainMenuCacheObjectName());
                }
                else if (_menuType == MenuService.EMenuType.Bottom)
                {
                    var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                    if (CacheManager.Contains(cacheName))
                        CacheManager.Remove(cacheName);
                }

                int sortOrder = 0;
                if (Int32.TryParse(grid.UpdatedRow["SortOrder"], out sortOrder))
                {
                    var mItem = MenuService.GetMenuItemById(SQLDataHelper.GetInt(grid.UpdatedRow["ID"]), _menuType);
                    mItem.MenuItemName = grid.UpdatedRow["MenuItemName"];
                    mItem.Blank = SQLDataHelper.GetBoolean(grid.UpdatedRow["Blank"]);
                    mItem.SortOrder = sortOrder;
                    mItem.Enabled = SQLDataHelper.GetBoolean(grid.UpdatedRow["Enabled"]);
                    MenuService.UpdateMenuItem(mItem, _menuType);
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

        private void LoadChildMenuItems(TreeNode node, MenuService.EMenuType mItemType)
        {
            foreach (var c in MenuService.GetChildMenuItemsByParentId(SQLDataHelper.GetInt(node.Value), mItemType))
            {
                var newNode = new ButtonTreeNodeMenu
                    {
                        Text = c.Enabled ? c.MenuItemName : string.Format("<span style=\"color:grey;\">{0}</span>", c.MenuItemName),
                        MessageToDel =
                            Server.HtmlEncode(string.Format(
                                Resource.Admin_MasterPageAdminCatalog_MenuConfirmation, c.MenuItemName)),
                        Value = c.MenuItemID.ToString(),
                        NavigateUrl = "Menu.aspx?MenuID=" + c.MenuItemID + "&type=" + mItemType,
                        TreeView = tree,
                        MenuType = mItemType,
                        Selected = c.MenuItemID == _menuId
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

        private void LoadRootMenuItems(TreeNodeCollection treeNodeCollection, MenuService.EMenuType mItemType)
        {
            foreach (var c in MenuService.GetChildMenuItemsByParentId(0, mItemType))
            {
                var newNode = new ButtonTreeNodeMenu
                    {
                        Text = c.MenuItemName,
                        MessageToDel =
                            Server.HtmlEncode(string.Format(
                                Resource.Admin_MasterPageAdminCatalog_MenuConfirmation, c.MenuItemName)),
                        Value = c.MenuItemID.ToString(),
                        NavigateUrl = "Menu.aspx?MenuId=" + c.MenuItemID + "&type=" + mItemType,
                        TreeView = tree,
                        MenuType = mItemType,
                        Selected = c.MenuItemID == _menuId
                    };

                if (c.HasChild)
                {
                    newNode.Expanded = false;
                    newNode.PopulateOnDemand = true;
                }
                treeNodeCollection.Add(newNode);
            }
        }

        protected void tree_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            LoadChildMenuItems(e.Node, MenuService.EMenuType.Top);
        }

        protected void treeBottom_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            LoadChildMenuItems(e.Node, MenuService.EMenuType.Bottom);
        }

        protected void tree_TreeNodeCommand(object sender, CommandEventArgs e)
        {
            try
            {
                if (e.CommandName.StartsWith("DeleteMenuItem"))
                {

                    int menuId = 0;
                    if (!String.IsNullOrEmpty(tree.SelectedValue))
                    {
                        menuId = SQLDataHelper.GetInt(tree.SelectedValue);
                    }
                    if (e.CommandName.Contains("#"))
                    {
                        menuId = SQLDataHelper.GetInt(e.CommandName.Substring(e.CommandName.IndexOf("#") + 1));
                    }

                    if (menuId != 0)
                    {
                        foreach (var id in MenuService.GetAllChildIdByParent(menuId, MenuService.EMenuType.Top))
                        {
                            MenuService.DeleteMenuItemById(id, MenuService.EMenuType.Top);
                        }
                        CacheManager.RemoveByPattern(CacheNames.GetMainMenuCacheObjectName());
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected void treeBottom_TreeNodeCommand(object sender, CommandEventArgs e)
        {
            try
            {
                if (e.CommandName.StartsWith("DeleteMenuItem"))
                {
                    int menuId = 0;
                    if (!String.IsNullOrEmpty(treeBottom.SelectedValue))
                    {
                        menuId = SQLDataHelper.GetInt(treeBottom.SelectedValue);
                    }
                    if (e.CommandName.Contains("#"))
                    {
                        menuId = SQLDataHelper.GetInt(e.CommandName.Substring(e.CommandName.IndexOf("#") + 1));
                    }

                    if (menuId != 0)
                    {
                        foreach (var id in MenuService.GetAllChildIdByParent(menuId, MenuService.EMenuType.Bottom))
                        {
                            MenuService.DeleteMenuItemById(id, MenuService.EMenuType.Bottom);
                        }

                        var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                        if (CacheManager.Contains(cacheName))
                            CacheManager.Remove(cacheName);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        protected string RenderSplitter()
        {
            var str = new StringBuilder();
            str.Append("<td class=\'splitter\'  onclick=\'togglePanel();return false;\' >");
            str.Append("<div class=\'leftPanelTop\'></div>");
            switch (Resource.Admin_Catalog_SplitterLang)
            {
                case "rus":
                    str.Append("<div id=\'divHide\' class=\'left_hide_rus\'></div>");
                    str.Append("<div id=\'divShow\' class=\'left_show_rus\'></div>");
                    break;
                case "eng":
                    str.Append("<div id=\'divHide\' class=\'left_hide_en\'></div>");
                    str.Append("<div id=\'divShow\' class=\'left_show_en\'></div>");
                    break;
            }
            str.Append("</td>");
            return str.ToString();
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
                        foreach (var item in MenuService.GetAllChildIdByParent(SQLDataHelper.GetInt(id), _menuType))
                            MenuService.DeleteMenuItemById(item, _menuType);
                    }
                }
                else
                {
                    var itemsIds = _paging.ItemsIds<int>("MenuItemID as ID");
                    foreach (var mItemId in itemsIds.Where(mId => !_selectionFilter.Values.Contains(mId.ToString())))
                    {
                        foreach (var id in MenuService.GetAllChildIdByParent(mItemId, _menuType))
                        {
                            MenuService.DeleteMenuItemById(id, _menuType);
                        }
                    }
                }
                if (_menuType == MenuService.EMenuType.Top)
                {
                    CacheManager.RemoveByPattern(CacheNames.GetMainMenuCacheObjectName());
                }
                if (_menuType == MenuService.EMenuType.Bottom)
                {
                    var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                    if (CacheManager.Contains(cacheName))
                    {
                        CacheManager.Remove(cacheName);
                    }
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (ddSelect.SelectedIndex != 0)
            {
                if (ddSelect.SelectedIndex == 2)
                {
                    if (_selectionFilter != null)
                    {
                        _selectionFilter.IncludeValues = !_selectionFilter.IncludeValues;
                    }
                }
                _paging.Fields["ID"].Filter = _selectionFilter;
            }
            else
            {
                _paging.Fields["ID"].Filter = null;
            }

            _paging.Fields["MenuItemName"].Filter = !string.IsNullOrEmpty(txtNameFilter.Text) ? new CompareFieldFilter { Expression = txtNameFilter.Text, ParamName = "@MenuItemName" } : null;
            _paging.Fields["Blank"].Filter = ddlBlank.SelectedIndex != 0 ? new CompareFieldFilter { Expression = ddlBlank.SelectedIndex == 1 ? "1" : "0", ParamName = "@Blank" } : null;
            _paging.Fields["Enabled"].Filter = ddlEnabled.SelectedIndex != 0 ? new CompareFieldFilter { Expression = ddlEnabled.SelectedIndex == 1 ? "1" : "0", ParamName = "@enabled" } : null;
            pageNumberer.CurrentPageIndex = 1;
            _paging.CurrentPageIndex = 1;
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            btnFilter_Click(sender, e);
            grid.ChangeHeaderImageUrl(null, null);
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteItem")
            {
                if (_menuType == MenuService.EMenuType.Top)
                {
                    CacheManager.RemoveByPattern(CacheNames.GetMainMenuCacheObjectName());
                }
                else if (_menuType == MenuService.EMenuType.Bottom)
                {
                    var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                    if (CacheManager.Contains(cacheName))
                        CacheManager.Remove(cacheName);
                }

                foreach (var id in MenuService.GetAllChildIdByParent(SQLDataHelper.GetInt(e.CommandArgument), _menuType))
                {
                    MenuService.DeleteMenuItemById(id, _menuType);
                }
            }
        }

        protected void grid_Sorting(object sender, GridViewSortEventArgs e)
        {
            var arrows = new Dictionary<string, string>
                {
                    {"MenuItemName", "arrowMenuName"},
                    {"Blank", "arrowBlank"},
                    {"Enabled", "arrowEnabled"},
                    {"SortOrder","arrowSortOrder"},
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

        protected void pn_SelectedPageChanged(object sender, EventArgs e)
        {
            _paging.CurrentPageIndex = pageNumberer.CurrentPageIndex;
        }

        //public string GetPicLink(int mItemId)
        //{
        //    string url = String.Empty;

        //    UrlService.GetAdminLink()
        //    return paramID != 0 && !String.IsNullOrEmpty(url = AdvantShop.Core.UrlRewriter.RouteService1.GetUrlAdminStringByParamID(paramID))
        //               ? String.Format("<a href=\"{0}\" class=\"editbtn showtooltip\" title=\"{1}\"><img src=\"admin/images/list.gif\" style=\"border: none;\"/></a>", url, Resource.Admin_MasterPageAdminMenu_EditPage)
        //               : "&nbsp;&nbsp;&nbsp;";
        //}

        protected void hlDeleteCategory_Click(object sender, EventArgs e)
        {
            foreach (var id in MenuService.GetAllChildIdByParent(_menuId, _menuType))
            {
                MenuService.DeleteMenuItemById(id, _menuType);
            }

            if (_menuType == MenuService.EMenuType.Top)
            {
                CacheManager.RemoveByPattern(CacheNames.GetMainMenuCacheObjectName());
                Response.Redirect("Menu.aspx?type=Top");
            }
            else if (_menuType == MenuService.EMenuType.Bottom)
            {
                var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                if (CacheManager.Contains(cacheName))
                    CacheManager.Remove(cacheName);
                Response.Redirect("Menu.aspx?type=Bottom");
            }
        }
    }
}