//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using AdvantShop.CMS;
using Resources;

namespace AdvantShop.Controls
{
    public class CommandTreeView : TreeView
    {
        private CommandEventHandler _treeNodeCommandEvent;

        public event CommandEventHandler TreeNodeCommand
        {
            add { _treeNodeCommandEvent = (CommandEventHandler)Delegate.Combine(_treeNodeCommandEvent, value); }
            remove { _treeNodeCommandEvent = (CommandEventHandler)Delegate.Remove(_treeNodeCommandEvent, value); }
        }

        private void OnTreeNodeCommand(string cmd)
        {
            if (_treeNodeCommandEvent != null)
                _treeNodeCommandEvent(this, new CommandEventArgs(cmd, null));
        }

        protected override void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.Substring(0, 2).Equals("c$"))
            {
                OnTreeNodeCommand(eventArgument.Substring(2, eventArgument.Length - 2));
            }
            else
            {
                base.RaisePostBackEvent(eventArgument);
            }
        }
    }

    [Flags]
    public enum TreeButtonStatus
    {
        None = 0x0,
        AddChild = 0x1,
        Edit = 0x2,
        Delete = 0x3
    }

    public class ButtonTreeNode : TreeNode
    {
        public TreeView TreeView { get; set; }

        public string MessageToDel { get; set; }
        public TreeButtonStatus ShowButtons { get; set; }
        public string CreateStr { get; set; }
        public string EditStr { get; set; }
        public string DeleteStr { get; set; }
        public ButtonTreeNode()
        {
            ShowButtons = TreeButtonStatus.AddChild | TreeButtonStatus.Edit | TreeButtonStatus.Delete;
        }
    }

    public class ButtonTreeNodeCatalog : ButtonTreeNode
    {

        protected override void RenderPreText(HtmlTextWriter writer)
        {
            base.RenderPreText(writer);

            writer.AddAttribute("class", "newToolTip");
            writer.AddAttribute("catId", Value);
            writer.AddAttribute("catName", MessageToDel);
            writer.RenderBeginTag("div");
        }

        protected override void RenderPostText(HtmlTextWriter writer)
        {
            base.RenderPostText(writer);
            if (Selected)
            {
                writer.Write("&nbsp;");
                if ((ShowButtons & TreeButtonStatus.AddChild) == TreeButtonStatus.AddChild)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, "images/glplus.gif");
                    writer.AddAttribute(HtmlTextWriterAttribute.Title,
                                        Resource.Admin_MasterPageAdminCatalog_FPanel_AddCategory);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "showtooltip");
                    writer.AddAttribute("onmouseover", "this.src = \'images/blplus.gif\';");
                    writer.AddAttribute("onmouseout", "this.src = \'images/glplus.gif\';");
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                        "open_window(\'m_Category.aspx?CategoryID=" + Value +
                                        "&mode=create\',750,640); return false;");
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }
                if ((ShowButtons & TreeButtonStatus.Edit) == TreeButtonStatus.Edit)
                {
                    writer.Write("&nbsp;");
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, "images/gpencil.gif");
                    writer.AddAttribute(HtmlTextWriterAttribute.Title,
                                        Resource.Admin_MasterPageAdminCatalog_FPanel_EditCategory);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "showtooltip");
                    writer.AddAttribute("onmouseover", "this.src = \'images/bpencil.gif\';");
                    writer.AddAttribute("onmouseout", "this.src = \'images/gpencil.gif\';");
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                        "open_window(\'m_Category.aspx?CategoryID=" + Value +
                                        "&mode=edit\',750,640); return false;");
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }
                if (Value != "0" && (ShowButtons & TreeButtonStatus.Delete) == TreeButtonStatus.Delete)
                {
                    writer.Write("&nbsp;");
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, "images/gcross.gif");
                    writer.AddAttribute(HtmlTextWriterAttribute.Title,
                                        Resource.Admin_MasterPageAdminCatalog_FPanel_DeleteCategory);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "showtooltip");
                    writer.AddAttribute("onmouseover", "this.src = \'images/bcross.gif\';");
                    writer.AddAttribute("onmouseout", "this.src = \'images/gcross.gif\';");
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                        "if(confirm(\'" + string.Format(Resource.Admin_MasterPageAdminCatalog_Confirmation, Text) + "\')){" + TreeView.Page.ClientScript.GetPostBackEventReference(TreeView, "c$DeleteCategory") + "} return false;");
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }
            }
            writer.RenderEndTag();
        }
    }

    public class ButtonTreeNodeMenu : ButtonTreeNode
    {
        public MenuService.EMenuType MenuType = MenuService.EMenuType.Top;

        protected override void RenderPreText(HtmlTextWriter writer)
        {
            base.RenderPreText(writer);

            writer.AddAttribute("class", "newToolTip");
            writer.AddAttribute("menuId", Value);
            writer.AddAttribute("menuName", MessageToDel);
            writer.AddAttribute("menuType", MenuType.ToString());
            writer.RenderBeginTag("div");
        }

        protected override void RenderPostText(HtmlTextWriter writer)
        {
            base.RenderPostText(writer);
            if (Selected)
            {
                if ((ShowButtons & TreeButtonStatus.AddChild) == TreeButtonStatus.AddChild)
                {
                    writer.Write("&nbsp;");
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, "images/glplus.gif");
                    writer.AddAttribute(HtmlTextWriterAttribute.Title,
                                        Resource.Admin_MenuManager_CreateItem);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "showtooltip");
                    writer.AddAttribute("onmouseover", "this.src = \'images/blplus.gif\';");
                    writer.AddAttribute("onmouseout", "this.src = \'images/glplus.gif\';");
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                        "open_window('m_Menu.aspx?MenuID=" + Value + "&mode=create&type=" + MenuType +
                                        "',750,600);return false;");
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }

                if (Value != "0")
                {
                    if ((ShowButtons & TreeButtonStatus.Edit) == TreeButtonStatus.Edit)
                    {
                        writer.Write("&nbsp;");
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, "images/gpencil.gif");
                        writer.AddAttribute(HtmlTextWriterAttribute.Title,
                                            Resource.Admin_MenuManager_EditItem);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "showtooltip");
                        writer.AddAttribute("onmouseover", "this.src = \'images/bpencil.gif\';");
                        writer.AddAttribute("onmouseout", "this.src = \'images/gpencil.gif\';");
                        writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                            "open_window('m_Menu.aspx?MenuID=" + Value + "&mode=edit&type=" + MenuType +
                                            "',750,600);return false;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag();
                    }
                    if ((ShowButtons & TreeButtonStatus.Delete) == TreeButtonStatus.Delete)
                    {
                        writer.Write("&nbsp;");
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, "images/gcross.gif");
                        writer.AddAttribute(HtmlTextWriterAttribute.Title,
                                            Resource.Admin_MenuManager_DeleteItem);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "showtooltip");
                        writer.AddAttribute("onmouseover", "this.src = \'images/bcross.gif\';");
                        writer.AddAttribute("onmouseout", "this.src = \'images/gcross.gif\';");
                        writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                            "if(confirm(\'" +
                                            string.Format(Resource.Admin_MenuManager_ConfirmationItem, Text) +
                                            "\')){" +
                                            TreeView.Page.ClientScript.GetPostBackEventReference(TreeView,
                                                                                                 "c$DeleteMenuItem") +
                                            "} return false;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag();
                    }
                }
            }
            writer.RenderEndTag();
        }
    }

    public class ButtonTreeNodeStaticPage : ButtonTreeNode
    {
        protected override void RenderPreText(HtmlTextWriter writer)
        {
            base.RenderPreText(writer);

            writer.AddAttribute("class", "newToolTip");
            writer.AddAttribute("catId", Value);
            writer.AddAttribute("catName", MessageToDel);
            writer.RenderBeginTag("div");
        }

        protected override void RenderPostText(HtmlTextWriter writer)
        {
            base.RenderPostText(writer);
            if (Selected)
            {
                if ((ShowButtons & TreeButtonStatus.AddChild) == TreeButtonStatus.AddChild)
                {
                    writer.Write("&nbsp;");
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
                    writer.AddAttribute(HtmlTextWriterAttribute.Src, "images/glplus.gif");
                    writer.AddAttribute(HtmlTextWriterAttribute.Title,
                                        Resource.Admin_StaticPage_Create);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "showtooltip");
                    writer.AddAttribute("onmouseover", "this.src = \'images/blplus.gif\';");
                    writer.AddAttribute("onmouseout", "this.src = \'images/glplus.gif\';");
                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                        "window.location='StaticPage.aspx?ParentID=" + Value +
                                        "&mode=create';return false;");
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }
                if (Value != "0")
                {
                    if ((ShowButtons & TreeButtonStatus.Edit) == TreeButtonStatus.Edit)
                    {
                        writer.Write("&nbsp;");
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, "images/gpencil.gif");
                        writer.AddAttribute(HtmlTextWriterAttribute.Title,
                                            Resource.Admin_StaticPage_AuxEdit);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "showtooltip");
                        writer.AddAttribute("onmouseover", "this.src = \'images/bpencil.gif\';");
                        writer.AddAttribute("onmouseout", "this.src = \'images/gpencil.gif\';");
                        writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                            "window.location='StaticPage.aspx?PageID=" + Value +
                                            "&mode=edit';return false;");

                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag();
                    }
                    if ((ShowButtons & TreeButtonStatus.Delete) == TreeButtonStatus.Delete)
                    {
                        writer.Write("&nbsp;");
                        writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, "images/gcross.gif");
                        writer.AddAttribute(HtmlTextWriterAttribute.Title,
                                            Resource.Admin_StaticPage_DeleteItem);
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "showtooltip");
                        writer.AddAttribute("onmouseover", "this.src = \'images/bcross.gif\';");
                        writer.AddAttribute("onmouseout", "this.src = \'images/gcross.gif\';");
                        writer.AddAttribute(HtmlTextWriterAttribute.Onclick,
                                            "if(confirm(\'" +
                                            string.Format(Resource.Admin_StaticPage_Confirmation, Text) +
                                            "\')){" +
                                            TreeView.Page.ClientScript.GetPostBackEventReference(TreeView,
                                                                                                 "c$DeleteStaticPage" + "#" + Value) +
                                            "} return false;");
                        writer.RenderBeginTag(HtmlTextWriterTag.Input);
                        writer.RenderEndTag();
                    }
                }
            }
            writer.RenderEndTag();
        }
    }
}