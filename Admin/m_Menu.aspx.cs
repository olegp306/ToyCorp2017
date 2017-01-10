//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop.CMS;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.Caching;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using Resources;

namespace Admin
{
    public partial class m_Menu : AdvantShopAdminPage
    {

        public enum eMenuMode
        {
            Edit,
            Create,
            Err
        }

        private eMenuMode _mode = eMenuMode.Create;
        private MenuService.EMenuType _type = MenuService.EMenuType.Top;
        private int _menuItemId = -1;

        private void MsgErr(bool clean)
        {
            if (clean)
            {
                lblError.Visible = false;
                lblError.Text = string.Empty;
            }
            else
            {
                lblError.Visible = false;
            }
        }

        private void MsgErr(string messageText)
        {
            lblError.Visible = true;
            lblError.Text = messageText + @"<br/>";
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_m_Category_AddCategory));

            if (!string.IsNullOrEmpty(Request["menuid"]))
            {
                if (Request["menuid"] == "Add")
                {
                    _mode = eMenuMode.Create;
                }
                else
                {
                    Int32.TryParse(Request["menuid"], out _menuItemId);
                    if (_menuItemId != -1) _mode = eMenuMode.Edit;
                }
                if (_menuItemId == -1) _mode = eMenuMode.Err;
            }
            else
            {
                _mode = eMenuMode.Err;
            }

            if (!string.IsNullOrEmpty(Request["type"]))
            {
                Enum.TryParse(Request["type"], true, out _type);
            }

            if (Request["mode"] != null)
            {
                if (Request["mode"] == "edit")
                {
                    _mode = eMenuMode.Edit;
                }
                else if (Request["mode"] == "create")
                {
                    _mode = eMenuMode.Create;
                }
                else
                {
                    _mode = eMenuMode.Err;
                }
            }
            else
            {
                _mode = eMenuMode.Err;
            }

            if (!IsPostBack)
            {
                lblBigHead.Text = _type == MenuService.EMenuType.Top
                                      ? Resource.Admin_MenuManager_TopMenu
                                      : Resource.Admin_MenuManager_BottomMenu;

                if (_mode == eMenuMode.Edit)
                {
                    btnAdd.Text = Resource.Admin_m_Category_Save;
                    lblSubHead.Text = Resource.Admin_MenuManager_EditMenuItem;

                    if (!LoadMenuItem(_menuItemId))
                        return;
                }
                else if (_mode == eMenuMode.Create)
                {
                    txtName.Text = string.Empty;
                    txtName.Focus();

                    txtSortOrder.Text = @"0";

                    btnAdd.Text = Resource.Admin_m_Category_Add;
                    lblSubHead.Text = Resource.Admin_MenuManager_CreateMenuItem;
                }

                if (_mode == eMenuMode.Create && _menuItemId == 0)
                {
                    lParent.Text = Resource.Admin_MenuManager_RootItem;
                    pnlIcon.Visible = false;
                }
                else if (_mode == eMenuMode.Create)
                {
                    var mParentItem = MenuService.GetMenuItemById(_menuItemId, _type);
                    lParent.Text = mParentItem != null ? mParentItem.MenuItemName : Resource.Admin_m_Category_No;
                    hParent.Value = mParentItem != null ? Convert.ToString(mParentItem.MenuItemID) : string.Empty;
                    pnlIcon.Visible = false;
                }

                if (_menuItemId == 0 && _mode == eMenuMode.Edit)
                {
                    lParent.Text = Resource.Admin_m_Category_No;
                    lbParentChange.Visible = false;
                }
                else if (_mode == eMenuMode.Edit)
                {
                    var mItem = MenuService.GetMenuItemById(_menuItemId, _type);

                    if (mItem != null && mItem.MenuItemParentID != 0)
                    {
                        var pItem = MenuService.GetMenuItemById(mItem.MenuItemParentID, _type);
                        lParent.Text = pItem != null ? pItem.MenuItemName
                                           : Resource.Admin_MenuManager_RootItem;
                    }
                    else
                    {
                        lParent.Text = Resource.Admin_MenuManager_RootItem;
                    }
                    hParent.Value = Convert.ToString(mItem.MenuItemParentID);
                }

                var node = new TreeNode { Text = Resource.Admin_m_Category_Root, Value = @"0", Selected = true };
                tree.Nodes.Add(node);

                LoadChildItems(tree.Nodes[0]);

                TreeNodeCollection nodes = tree.Nodes[0].ChildNodes;
                //var isFirstNode = true;
                foreach (var parentItems in MenuService.GetParentMenuItems(String.IsNullOrEmpty(hParent.Value) ? 0 : Convert.ToInt32(hParent.Value), _type))
                {
                    TreeNode tn = (nodes.Cast<TreeNode>().Where(n => n.Value == parentItems.ToString(CultureInfo.InvariantCulture))).SingleOrDefault();

                    if (tn != null)
                    {
                        //tn.Selected = isFirstNode;
                        //isFirstNode = false;
                        tn.Expand();
                        nodes = tn.ChildNodes;
                    }
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                //Product = 0,
                //Category = 1,
                //StaticPage = 2,
                //News = 3,
                //Brand = 4,
                //Custom = 5

                if (rblLinkType.SelectedValue == "0" && gridProduct.SelectProductId != 0)
                {
                    txtUrl.Text = UrlService.GetLinkDB(ParamType.Product, gridProduct.SelectProductId);
                    hfParamId.Value = gridProduct.SelectProductId.ToString(CultureInfo.InvariantCulture);
                }
                else if (rblLinkType.SelectedValue == "1" && gridCategory.SelectCategoryId != 0)
                {
                    txtUrl.Text = UrlService.GetLinkDB(ParamType.Category, gridCategory.SelectCategoryId);
                    hfParamId.Value = gridCategory.SelectCategoryId.ToString(CultureInfo.InvariantCulture);
                }
                else if (rblLinkType.SelectedValue == "2" && gridAux.SelectAuxId != 0)
                {
                    txtUrl.Text = UrlService.GetLinkDB(ParamType.StaticPage, gridAux.SelectAuxId);
                    hfParamId.Value = gridAux.SelectAuxId.ToString(CultureInfo.InvariantCulture);
                }
                else if (rblLinkType.SelectedValue == "3" && gridNews.SelectNewsId != 0)
                {
                    txtUrl.Text = UrlService.GetLinkDB(ParamType.News, gridNews.SelectNewsId);
                    hfParamId.Value = gridNews.SelectNewsId.ToString(CultureInfo.InvariantCulture);
                }
                else if (rblLinkType.SelectedValue == "4" && gridBrand.SelectBrandId != 0)
                {
                    txtUrl.Text = UrlService.GetLinkDB(ParamType.Brand, gridBrand.SelectBrandId);
                }

                //lParent.Text = tree.SelectedNode.Text;
                //hParent.Value = tree.SelectedNode.Value;
            }
            if (rblLinkType.SelectedValue == "5")
            {
                lbChooseUrl.Attributes.Add("style", "display:none;");
            }
        }

        protected void tree_SelectedNodeChange(object sender, EventArgs e)
        {
            mpeTree.Show();
            lParent.Text = tree.SelectedNode.Text;
            hParent.Value = tree.SelectedNode.Value;
        }

        protected void btnUpdateParent_Click(object sender, EventArgs e)
        {
            mpeTree.Hide();
            lParent.Text = tree.SelectedNode.Text;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (_mode == eMenuMode.Edit)
            {
                if (lblError.Visible == false)
                {
                    SaveMenuItem();

                    if (lblError.Visible == false)
                    {
                        CommonHelper.RegCloseScript(this, "'Menu.aspx?MenuID=" + (String.IsNullOrEmpty(hParent.Value) ? "0" : hParent.Value) + "&type=" + _type + "';");
                    }
                }
            }
            else if (_mode == eMenuMode.Create)
            {
                int itemId = CreateMenuItem();
                if (itemId != 0 && lblError.Visible == false)
                {
                    CommonHelper.RegCloseScript(this, "'Menu.aspx?MenuID=" + (String.IsNullOrEmpty(hParent.Value) ? "0" : hParent.Value) + "&type=" + _type + "';");
                }
            }
        }

        private bool LoadMenuItem(int menuId)
        {
            try
            {
                var mItem = MenuService.GetMenuItemById(menuId, _type);
                if (mItem == null)
                {
                    MsgErr("Error at LoadMenuItem");
                    return false;
                }

                txtName.Text = mItem.MenuItemName;
                txtSortOrder.Text = mItem.SortOrder.ToString(CultureInfo.InvariantCulture);

                //txtUrl.Text = mItem.MenuItemUrlType != EMenuItemUrlType.Custom
                //    ? UrlService.GetLinkDB((ParamType)mItem.MenuItemUrlType, Convert.ToInt32(mItem.MenuItemUrlPath))
                //    : mItem.MenuItemUrlPath;
                txtUrl.Text = mItem.MenuItemUrlPath;

                hfParamId.Value = mItem.MenuItemUrlType != EMenuItemUrlType.Custom ? mItem.MenuItemUrlPath : "0";

                ckbBlank.Checked = mItem.Blank;
                ckbNofollow.Checked = mItem.NoFollow;
                ckbEnabled.Checked = mItem.Enabled;
                ddlShowMode.SelectedValue = Convert.ToInt32(mItem.ShowMode).ToString(CultureInfo.InvariantCulture);
                rblLinkType.SelectedValue = Convert.ToInt32(mItem.MenuItemUrlType).ToString(CultureInfo.InvariantCulture);

                if (File.Exists(Server.MapPath("~") + "\\pictures\\icons\\" + mItem.MenuItemIcon))
                {
                    lblIconFileName.Text = mItem.MenuItemIcon;
                    pnlIcon.Visible = true;

                    imgIcon.ImageUrl = UrlService.GetAbsoluteLink("/pictures/icons/" + mItem.MenuItemIcon);
                    imgIcon.ToolTip = mItem.MenuItemIcon;
                }
                else
                {
                    lblIconFileName.Text = @"No picture";
                    pnlIcon.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MsgErr(ex.Message + " at LoadMenuItem");
                Debug.LogError(ex, "LoadMenuItem");
                return false;
            }
            return true;
        }

        private void SaveMenuItem()
        {
            if (_mode == eMenuMode.Err)
            {
                return;
            }
            if (!ValidateData())
            {
                return;
            }
            var url = string.Empty;
            if (txtUrl.Text.Contains("www."))
            {
                url = txtUrl.Text.Contains("http://") || txtUrl.Text.Contains("https://") ? txtUrl.Text : "http://" + txtUrl.Text;
            }
            else
            {
                url = txtUrl.Text;
            }

            lblError.Text = String.Empty;
            var mItem = new AdvMenuItem
                {
                    MenuItemID = _menuItemId,
                    MenuItemName = txtName.Text,
                    MenuItemParentID = string.IsNullOrEmpty(hParent.Value) ? 0 : Convert.ToInt32(hParent.Value),
                    MenuItemUrlPath = url,
                    SortOrder = Convert.ToInt32(txtSortOrder.Text),
                    Blank = ckbBlank.Checked,
                    Enabled = ckbEnabled.Checked,
                    MenuItemUrlType = (EMenuItemUrlType)Convert.ToInt32(rblLinkType.SelectedValue),
                    ShowMode = (EMenuItemShowMode)Convert.ToInt32(ddlShowMode.SelectedValue),
                    NoFollow = ckbNofollow.Checked
                };

            if (IconFileUpload.HasFile)
            {
                PhotoService.DeletePhotos(_menuItemId, PhotoType.MenuIcon);
                using (IconFileUpload.FileContent)
                {
                    var tempName = PhotoService.AddPhoto(new Photo(0, _menuItemId, PhotoType.MenuIcon) { OriginName = IconFileUpload.FileName });
                    if (!string.IsNullOrWhiteSpace(tempName))
                    {
                        IconFileUpload.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.MenuIcons, tempName));
                    }
                    mItem.MenuItemIcon = tempName;
                }
            }
            else
            {
                mItem.MenuItemIcon = pnlIcon.Visible ? imgIcon.ToolTip : null;
            }

            MenuService.UpdateMenuItem(mItem, _type);
            if (_type == MenuService.EMenuType.Top)
            {
                CacheManager.RemoveByPattern(CacheNames.GetMainMenuCacheObjectName());
                CacheManager.RemoveByPattern(CacheNames.GetMainMenuAuthCacheObjectName());
            }
            else if (_type == MenuService.EMenuType.Bottom)
            {
                var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                if (CacheManager.Contains(cacheName))
                    CacheManager.Remove(cacheName);

                var cacheAuthName = CacheNames.GetBottomMenuAuthCacheObjectName();
                if (CacheManager.Contains(cacheAuthName))
                    CacheManager.Remove(cacheAuthName);
            }
        }

        private int CreateMenuItem()
        {
            ValidateData();

            var mItem = new AdvMenuItem
                {
                    MenuItemName = txtName.Text,
                    MenuItemParentID = string.IsNullOrEmpty(hParent.Value) ? 0 : Convert.ToInt32(hParent.Value),
                    MenuItemUrlPath = txtUrl.Text,
                    Enabled = ckbEnabled.Checked,
                    Blank = ckbBlank.Checked,
                    SortOrder = Convert.ToInt32(txtSortOrder.Text),
                    MenuItemUrlType = (EMenuItemUrlType)Convert.ToInt32(rblLinkType.SelectedValue),
                    ShowMode = (EMenuItemShowMode)Convert.ToInt32(ddlShowMode.SelectedValue),
                    NoFollow = ckbNofollow.Checked
                };

            if (_type == MenuService.EMenuType.Top)
            {
                CacheManager.RemoveByPattern(CacheNames.GetMainMenuCacheObjectName());
                CacheManager.RemoveByPattern(CacheNames.GetMainMenuAuthCacheObjectName());
            }
            else if (_type == MenuService.EMenuType.Bottom)
            {
                var cacheName = CacheNames.GetBottomMenuCacheObjectName();
                if (CacheManager.Contains(cacheName))
                    CacheManager.Remove(cacheName);

                var cacheAuthName = CacheNames.GetBottomMenuAuthCacheObjectName();
                if (CacheManager.Contains(cacheAuthName))
                    CacheManager.Remove(cacheAuthName);
            }
            mItem.MenuItemID = MenuService.AddMenuItem(mItem, _type);
            _menuItemId = mItem.MenuItemID;
            if (IconFileUpload.HasFile)
            {
                using (IconFileUpload.FileContent)
                {
                    var tempName = PhotoService.AddPhoto(new Photo(0, _menuItemId, PhotoType.MenuIcon) { OriginName = IconFileUpload.FileName });
                    if (!string.IsNullOrWhiteSpace(tempName))
                    {
                        IconFileUpload.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.MenuIcons, tempName));
                    }
                    mItem.MenuItemIcon = tempName;
                }
            }
            else
            {
                mItem.MenuItemIcon = pnlIcon.Visible ? imgIcon.ToolTip : null;
            }

            MenuService.UpdateMenuItem(mItem, _type);

            return mItem.MenuItemID;
        }

        protected void btnDeleteIcon_Click(object sender, EventArgs e)
        {
            if (_mode != eMenuMode.Edit) return;

            try
            {
                MenuService.DeleteMenuItemIconById(_menuItemId, _type,
                                                   Server.MapPath("~") + "\\pictures\\icons\\" + lblIconFileName.Text);
                pnlIcon.Visible = false;
            }
            catch (Exception ex)
            {
                MsgErr(ex.Message + " at DeleteImage");
                Debug.LogError(ex, "DeleteImage");
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                return false;
            }
            if (string.IsNullOrEmpty(hParent.Value))
            {
                return false;
            }
            if (rblLinkType.SelectedValue != "5" && string.IsNullOrEmpty(txtUrl.Text))
            {
                return false;
            }
            if (string.IsNullOrEmpty(txtSortOrder.Text))
            {
                return false;
            }
            return true;
        }

        protected void PopulateNode(object sender, TreeNodeEventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["type"]))
            {
                Enum.TryParse(Request["type"], true, out _type);
            }
            LoadChildItems(e.Node);
        }

        private void LoadChildItems(TreeNode node)
        {
            var childMenuItems = MenuService.GetChildMenuItemsByParentId(Convert.ToInt32(node.Value), _type);

            foreach (var c in childMenuItems)
            {
                var newNode = new TreeNode
                    {
                        Text = c.MenuItemName,
                        Value = c.MenuItemID.ToString(),
                        Selected = hParent.Value == c.MenuItemID.ToString()
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

        protected void lbParentChange_Click(object sender, EventArgs e)
        {
            mpeTree.Show();
        }

    }
}