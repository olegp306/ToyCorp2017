<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Menu.aspx.cs" Inherits="Admin.Menu" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Head" runat="Server">
    <style type="text/css">
        #ineditcategory
        {
            height: 16px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
    <div id="inprogress" style="display: none;">
        <div id="curtain" class="opacitybackground">
            &nbsp;</div>
        <div class="loader">
            <table width="100%" style="font-weight: bold; text-align: center;">
                <tbody>
                    <tr>
                        <td style="text-align: center;">
                            <img src="images/ajax-loader.gif" alt="" />
                        </td>
                    </tr>
                    <tr>
                        <td style="color: #0D76B8; text-align: center;">
                            <asp:Localize ID="Localize_Admin_Properties_PleaseWait" runat="server" Text="<%$ Resources:Resource, Admin_Properties_PleaseWait %>"></asp:Localize>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item selected"><a href="Menu.aspx">
                <%= Resource.Admin_MasterPageAdmin_MainMenu%></a></li>
            <li class="neighbor-menu-item"><a href="NewsAdmin.aspx">
                <%= Resource.Admin_MasterPageAdmin_NewsMenuRoot%></a></li>
            <li class="neighbor-menu-item"><a href="NewsCategory.aspx">
                <%= Resource.Admin_MasterPageAdmin_NewsCategory%></a></li>
            <li class="neighbor-menu-item"><a href="Carousel.aspx">
                <%= Resource.Admin_MasterPageAdmin_Carousel%></a></li>
            <li class="neighbor-menu-item"><a href="StaticPages.aspx">
                <%= Resource.Admin_MasterPageAdmin_AuxPagesMenuItem%></a></li>
            <li class="neighbor-menu-item"><a href="StaticBlocks.aspx">
                <%= Resource.Admin_MasterPageAdmin_PageParts%></a></li>
        </menu>
        <div class="panel-add">
            <%= Resource.Admin_MasterPageAdmin_Add %>
            <a href="#" onclick="open_window('m_news.aspx', 750, 640); return false;" class="panel-add-lnk">
                <%= Resource.Admin_MasterPageAdmin_News %></a>, <a href="StaticPage.aspx" class="panel-add-lnk">
                    <%= Resource.Admin_MasterPageAdmin_StaticPage %></a>
        </div>
    </div>
    <div class="content-own">
        <table border="0" width="100%" id="table2" cellspacing="0" cellpadding="0">
            <tr>
                <td style="vertical-align: top;">
                    <div id="leftPanel" class="leftPanel dvLeftPanel">
                        <table border="0" cellspacing="0" cellpadding="0" class="catalog_part catelog_list">
                            <tr style="height: 28px;">
                                <td style="width: 30px;">
                                    <img src="images/folder.gif" alt="" />
                                </td>
                                <td style="width: 137px;" class="catalog_label">
                                    <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_MenuManager_TopMenu %>"></asp:Localize>
                                </td>
                                <td style="width: 75px; text-align: right;">
                                    <asp:ImageButton ID="ibtnAddInRoot" runat="server" ImageUrl="images/gplus.gif" onmouseover="this.src='images/bplus.gif'"
                                        onmouseout="this.src='images/gplus.gif';" ToolTip='<%$ Resources:Resource,Admin_MenuManager_CreateItem %>'
                                        OnClientClick="open_window('m_Menu.aspx?MenuID=0&mode=create&type=Top', 750, 640);return false;" />
                                </td>
                            </tr>
                        </table>
                        <div class="catalog_part catelog_listContent">
                            <adv:CommandTreeView ID="tree" runat="server" NodeWrap="True" ShowLines="true" CssClass="AdminTree_MainClass"
                                OnTreeNodeCommand="tree_TreeNodeCommand" OnTreeNodePopulate="tree_TreeNodePopulate">
                                <ParentNodeStyle Font-Bold="False" />
                                <SelectedNodeStyle ForeColor="#027dc2" Font-Bold="true" HorizontalPadding="0px" VerticalPadding="0px" />
                                <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" HorizontalPadding="5px"
                                    NodeSpacing="0px" VerticalPadding="0px" />
                            </adv:CommandTreeView>
                        </div>
                        <table border="0" cellspacing="0" cellpadding="0" class="catalog_part catelog_list">
                            <tr style="height: 28px;">
                                <td style="width: 30px;">
                                    <img src="images/folder.gif" alt="" />
                                </td>
                                <td style="width: 137px;" class="catalog_label">
                                    <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources:Resource, Admin_MenuManager_BottomMenu %>"></asp:Localize>
                                </td>
                                <td style="width: 75px; text-align: right">
                                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/gplus.gif" onmouseover="this.src='images/bplus.gif'"
                                        onmouseout="this.src='images/gplus.gif';" ToolTip='<%$ Resources:Resource,Admin_MenuManager_CreateItem %>'
                                        OnClientClick="open_window('m_Menu.aspx?MenuID=0&mode=create&type=Bottom', 750, 640);return false;" />
                                </td>
                            </tr>
                        </table>
                        <div class="catalog_part catelog_listContent">
                            <adv:CommandTreeView ID="treeBottom" runat="server" NodeWrap="True" ShowLines="true"
                                CssClass="AdminTree_MainClass" OnTreeNodeCommand="treeBottom_TreeNodeCommand"
                                OnTreeNodePopulate="treeBottom_TreeNodePopulate">
                                <ParentNodeStyle Font-Bold="False" />
                                <SelectedNodeStyle ForeColor="#027dc2" Font-Bold="true" HorizontalPadding="0px" VerticalPadding="0px" />
                                <NodeStyle Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" HorizontalPadding="5px"
                                    NodeSpacing="0px" VerticalPadding="0px" />
                            </adv:CommandTreeView>
                        </div>
                    </div>
                </td>
                <%=RenderSplitter()%>
                <td style="width: 100%; vertical-align: top; padding: 0px 0 0 10px;">
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <tbody>
                            <tr>
                                <td style="width: 72px;">
                                    <img src="images/orders_ico.gif" alt="" />
                                </td>
                                <td colspan="2">
                                    <div>
                                        <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource,  Admin_MenuManager_TopMenu %>"></asp:Label>
                                        &nbsp;&nbsp;
                                        <asp:HyperLink ID="hlEditCategory" CssClass="blueLink" runat="server" Text="<%$ Resources:Resource, Admin_MasterPageAdminCatalog_FPanel_EditMenu %>" />
                                        <asp:Label ID="lblSeparator" runat="server" Text=" | "></asp:Label>
                                        <asp:LinkButton ID="hlDeleteCategory" CssClass="blueLink valid-confirm" runat="server" 
                                            Text="<%$ Resources:Resource, Admin_MasterPageAdminCatalog_FPanel_DeleteMenu %>"
                                            OnClick="hlDeleteCategory_Click"></asp:LinkButton>
                                    </div>
                                    <div>
                                        <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_MenuManager_SubHeaderTop %>"></asp:Label>
                                    </div>
                                </td>
                                <td style="vertical-align:bottom;">
                                    <div class="btns-main">
                                        <asp:Button CssClass="btn btn-middle btn-add" ID="btnAdd" runat="server" Text="<%$ Resources:Resource, Admin_HeadCmdInsertMenu %>" />
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div style="width:100%; margin-top:10px;">
                        <div>
                            <table style="width: 100%;" class="massaction">
                                <tr>
                                    <td>
                                        <span class="admin_catalog_commandBlock"><span style="display: inline-block; margin-right: 3px;">
                                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_Command %>"></asp:Localize>
                                        </span><span style="display: inline-block">
                                            <select id="commandSelect">
                                                <option value="selectAll">
                                                    <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_SelectAll %>"></asp:Localize>
                                                </option>
                                                <option value="unselectAll">
                                                    <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_UnselectAll %>"></asp:Localize>
                                                </option>
                                                <option value="selectVisible">
                                                    <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_SelectVisible %>"></asp:Localize>
                                                </option>
                                                <option value="unselectVisible">
                                                    <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_UnselectVisible %>"></asp:Localize>
                                                </option>
                                                <option value="deleteSelected">
                                                    <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DeleteSelected %>"></asp:Localize>
                                                </option>
                                            </select>
                                            <input id="commandButton" type="button" value="GO" style="font-size: 10px; width: 38px;
                                                height: 20px;" />
                                            <asp:LinkButton ID="lbDeleteSelected" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DeleteSelected %>"
                                                OnClick="lbDeleteSelected_Click" />
                                        </span><span class="selecteditems" style="vertical-align: baseline"><span style="padding: 0px 3px 0px 3px;">
                                            |</span><span id="selectedIdsCount" class="bold">0</span>&nbsp;<span><%=Resources.Resource.Admin_Catalog_ItemsSelected%></span></span>
                                        </span>
                                        <asp:Label ID="lblError" runat="server" Visible="false"></asp:Label>
                                    </td>
                                    <td class="selecteditems" style="text-align: right;">
                                        <asp:UpdatePanel ID="upCounts" runat="server">
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                                                <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                                            </Triggers>
                                            <ContentTemplate>
                                                <%=Resources.Resource.Admin_Catalog_Total%>
                                                <span class="bold">
                                                    <asp:Label ID="lblFound" CssClass="foundrecords" runat="server" Text="" /></span>&nbsp;<%=Resources.Resource.Admin_Catalog_RecordsFound%>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                            <div>
                                <asp:Panel runat="server" ID="filterPanel" DefaultButton="btnFilter">
                                    <table class="filter" cellpadding="0" cellspacing="0">
                                        <tr style="height: 5px;">
                                            <td colspan="6">
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 60px; text-align: center;">
                                                <asp:DropDownList ID="ddSelect" TabIndex="10" CssClass="dropdownselect" runat="server"
                                                    Width="55">
                                                    <asp:ListItem Selected="True" Text="<%$ Resources:Resource, Admin_Catalog_Any %>" />
                                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" />
                                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" />
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox CssClass="filtertxtbox" ID="txtNameFilter" Width="99%" runat="server"
                                                    TabIndex="12" />
                                            </td>
                                            <td style="width: 150px;">
                                                <asp:DropDownList ID="ddlEnabled" TabIndex="10" CssClass="dropdownselect" runat="server">
                                                    <asp:ListItem Selected="True" Text="<%$ Resources:Resource, Admin_Catalog_Any %>" />
                                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" />
                                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" />
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 150px;">
                                                <asp:DropDownList ID="ddlBlank" TabIndex="10" CssClass="dropdownselect" runat="server">
                                                    <asp:ListItem Selected="True" Text="<%$ Resources:Resource, Admin_Catalog_Any %>" />
                                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" />
                                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" />
                                                </asp:DropDownList>
                                            </td>
                                            <td style="width: 160px; padding-right: 0px; text-align: right; white-space: nowrap">
                                                <div style="width: 160px; height: 0px; font-size: 0px;">
                                                </div>
                                            </td>
                                            <td style="width: 100px; text-align: center;">
                                                <asp:Button ID="btnFilter" runat="server" CssClass="btn" OnClientClick="javascript:FilterClick();"
                                                    TabIndex="23" Text="<%$ Resources:Resource, Admin_Catalog_Filter %>" OnClick="btnFilter_Click" />
                                                <asp:Button ID="btnReset" runat="server" CssClass="btn" OnClientClick="javascript:ResetFilter();"
                                                    TabIndex="24" Text="<%$ Resources:Resource, Admin_Catalog_Reset %>" OnClick="btnReset_Click" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="height: 5px;" colspan="6">
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnAdd" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="btnGo" EventName="Click" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <adv:AdvGridView ID="grid" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                            CellPadding="0" CellSpacing="0" Confirmation="<%$ Resources:Resource, Admin_NewsAdmin_Confirmation %>"
                                            CssClass="tableview" Style="cursor: pointer" GridLines="None" OnRowCommand="grid_RowCommand"
                                            OnSorting="grid_Sorting" ShowFooter="false" ShowFooterWhenEmpty="true">
                                            <Columns>
                                                <asp:TemplateField AccessibleHeaderText="ID" Visible="false">
                                                    <EditItemTemplate>
                                                        <asp:Label ID="Label0" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label01" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-CssClass="checkboxcolumn" HeaderStyle-HorizontalAlign="center"
                                                    HeaderStyle-Width="60" ItemStyle-Width="60px">
                                                    <HeaderTemplate>
                                                        <div style="width: 60; height: 0px; font-size: 0px">
                                                        </div>
                                                        <asp:CheckBox ID="checkBoxCheckAll" CssClass="checkboxcheckall" runat="server" onclick="javascript:SelectVisible(this.checked);" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <%# ((bool)Eval("IsSelected"))? "<input type='checkbox' class='sel' checked='checked' />": "<input type='checkbox' class='sel' />"%>
                                                        <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Eval("ID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="MenuItemName" HeaderStyle-HorizontalAlign="Left">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="lblMenuName" runat="server" CommandName="Sort" CommandArgument="MenuItemName">
                                                            <%= Resources.Resource.Admin_MenuManager_Name %>
                                                            <asp:Image ID="arrowMenuName" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" /></asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtMenuName" runat="server" Text='<%# Eval("MenuItemName") %>' Width="99%"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lMenuName" runat="server" Text='<%# Bind("MenuItemName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="Enabled" HeaderStyle-HorizontalAlign="Left"
                                                    HeaderStyle-Width="150" ItemStyle-Width="150px">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="lblEnabled" runat="server" CommandName="Sort" CommandArgument="Enabled">
                                                            <%= Resources.Resource.Admin_MenuManager_Enabled %>
                                                            <asp:Image ID="arrowEnabled" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" /></asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <EditItemTemplate>
                                                        <asp:CheckBox ID="cbEnabledEdit" runat="server" Checked='<%# Eval("Enabled") %>'>
                                                        </asp:CheckBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbEnabled" runat="server" Checked='<%# Eval("Enabled") %>'></asp:CheckBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="Blank" HeaderStyle-HorizontalAlign="Left"
                                                    HeaderStyle-Width="150" ItemStyle-Width="150px">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="lblBlank" runat="server" CommandName="Sort" CommandArgument="Blank">
                                                            <%= Resources.Resource.Admin_MenuManager_InNewTab %>
                                                            <asp:Image ID="arrowBlank" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" /></asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <EditItemTemplate>
                                                        <asp:CheckBox ID="cbBlankEdit" runat="server" Checked='<%# Eval("Blank") %>'></asp:CheckBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cbBlank" runat="server" Checked='<%# Eval("Blank") %>'></asp:CheckBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField AccessibleHeaderText="SortOrder" HeaderStyle-HorizontalAlign="Left"
                                                    HeaderStyle-Width="160px" ItemStyle-Width="160px">
                                                    <HeaderTemplate>
                                                        <asp:LinkButton ID="lblSortOrder" runat="server" CommandName="Sort" CommandArgument="SortOrder">
                                                            <%= Resources.Resource.Admin_MenuManager_SortOrder %>
                                                            <asp:Image ID="arrowSortOrder" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" /></asp:LinkButton>
                                                    </HeaderTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtSortOrderEdit" runat="server" Text='<%# Eval("SortOrder") %>'>
                                                        </asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtSortOrder" runat="server" Text='<%# Eval("SortOrder") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Left" FooterStyle-HorizontalAlign="Center">
                                                    <EditItemTemplate>
                                                        <%# "<a href=\"javascript:open_window('m_Menu.aspx?MenuID=" + Eval("ID") + "&mode=edit&type=" + MenuType + "',750,600);\" class = 'editbtn showtooltip' title=\"" + Resources.Resource.Admin_MasterPageAdminMenu_EditLinks + "\" ><img src='images/editbtn.gif' style='border: none;' /></a>"%>
                                                        <%--<%# GetPicLink(SQLDataHelper.GetInt(Eval("ParamID")))%>--%>
                                                        <input id="btnUpdate" name="btnUpdate" class="updatebtn showtooltip" type="image"
                                                            src="images/updatebtn.png" onclick="<%#ClientScript.GetPostBackEventReference(grid, "Update$" + Container.DataItemIndex)%>;return false;"
                                                            style="display: none" title="<%=Resources.Resource.Admin_MasterPageAdminCatalog_Update %>" />
                                                        <asp:LinkButton ID="buttonDelete" runat="server"
                                                            CssClass="deletebtn showtooltip valid-confirm" CommandName="DeleteItem" CommandArgument='<%# Eval("ID")%>'
                                                            data-confirm="<%$Resources:Resource, Admin_MenuManager_DeleteItem %>"
                                                            ToolTip='<%$ Resources:Resource, Admin_MasterPageAdminCatalog_Delete %>' />
                                                        <input id="btnCancel" name="btnCancel" class="cancelbtn showtooltip" type="image"
                                                            src="images/cancelbtn.png" onclick="row_canceledit($(this).parent().parent()[0]);return false;"
                                                            style="display: none" title="<%=Resources.Resource.Admin_MasterPageAdminCatalog_Cancel %>" />
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle CssClass="header" />
                                            <RowStyle CssClass="row1 propertiesRow_25 readonlyrow" />
                                            <AlternatingRowStyle CssClass="row2 propertiesRow_25_alt readonlyrow" />
                                            <EmptyDataTemplate>
                                                <div style="text-align: center; margin-top: 20px; margin-bottom: 20px;">
                                                    <%=Resources.Resource.Admin_Catalog_NoRecords%>
                                                </div>
                                            </EmptyDataTemplate>
                                        </adv:AdvGridView>
                                        <div style="border-top: 1px #c9c9c7 solid;">
                                        </div>
                                        <table class="results2">
                                            <tr>
                                                <td style="width: 157px; padding-left: 6px;">
                                                    <%=Resources.Resource.Admin_Catalog_ResultPerPage%>:&nbsp;<asp:DropDownList ID="ddRowsPerPage"
                                                        runat="server" OnSelectedIndexChanged="btnFilter_Click" CssClass="droplist" AutoPostBack="true">
                                                        <asp:ListItem>10</asp:ListItem>
                                                        <asp:ListItem>20</asp:ListItem>
                                                        <asp:ListItem>50</asp:ListItem>
                                                        <asp:ListItem>100</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td style="text-align: center;">
                                                    <adv:PageNumberer CssClass="PageNumberer" ID="pageNumberer" runat="server" DisplayedPages="7"
                                                        UseHref="false" UseHistory="false" OnSelectedPageChanged="pn_SelectedPageChanged" />
                                                </td>
                                                <td style="width: 157px; text-align: right; padding-right: 12px">
                                                    <asp:Panel ID="goToPage" runat="server" DefaultButton="btnGo">
                                                        <span style="color: #494949">
                                                            <%=Resources.Resource.Admin_Catalog_PageNum%>&nbsp;<asp:TextBox ID="txtPageNum" runat="server"
                                                                Width="30" /></span>
                                                        <asp:Button ID="btnGo" runat="server" CssClass="btn" Text="<%$ Resources:Resource, Admin_Catalog_GO %>"
                                                            OnClick="linkGO_Click" />
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <input type="hidden" id="SelectedIds" name="SelectedIds" />
                        </div>
                    </div>
                    <div class="dvSubHelp">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
                        <a href="http://www.advantshop.net/help/pages/static-menu" target="_blank">Инструкция. Создание пунктов главного меню</a>
                    </div>
                </td>
            </tr>
        </table>
        <script type="text/javascript">
            var base$TreeView_PopulateNodeDoCallBack = this.TreeView_PopulateNodeDoCallBack;
            var base$TreeView_ProcessNodeData = this.TreeView_ProcessNodeData;
            this.TreeView_ProcessNodeData = function (result, context) {
                //alert( "after load " );
                hide_wait_for_node(context.node);
                var r = base$TreeView_ProcessNodeData(result, context);
                setupHoverPanel();
                return r;
            };
            this.TreeView_PopulateNodeDoCallBack = function (context, param) {
                //alert( "before load " );
                show_wait_for_node(context.node);
                return base$TreeView_PopulateNodeDoCallBack(context, param);
            };

            function setupTooltips() {
                $(".showtooltip").tooltip({
                    showURL: false
                });
            }

            function setupHoverPanel() {
                $(".newToolTip").each(function () {
                    if ($(this).data('qtip')) {
                        return true;
                    }
                    var MenuId = $(this).attr("menuId");
                    var MenuName = $(this).attr("menuName");
                    var MenuType = $(this).attr("menuType");

                    var cnt = "<div class='hoverPanel'><div style='margin-bottom:5px;width:190px'><a onmouseover='this.style.color = \"#FF7F27\"' onmouseout='this.style.color = \"#000000\"' onclick=\"open_window('m_Menu.aspx?MenuID=" + MenuId + "&mode=create&type=" + MenuType + "',750,640); return false;\"> <div class=\"hoverPanel_img_container\"><img onmouseout=\"this.src = 'images/glplus.gif';\" onmouseover=\"this.src = 'images/blplus.gif';\" src=\"images/glplus.gif\" alt=\"\"/></div>";
                    cnt += "<span><%= Resources.Resource.Admin_MasterPageAdminCatalog_FPanel_AddMenu %></span></a></div>";
                    if (MenuId != 0) {
                        cnt += "<div style='margin-bottom:5px;'><a onmouseover='this.style.color = \"#FF7F27\"' onmouseout='this.style.color = \"#000000\"' onclick=\"open_window('m_Menu.aspx?MenuID=" + MenuId + "&mode=edit&type=" + MenuType + "',750,640); return false;\"> <div class=\"hoverPanel_img_container\"><img onmouseout=\"this.src = 'images/gpencil.gif';\" onmouseover=\"this.src = 'images/bpencil.gif';\" src=\"images/gpencil.gif\" alt=\"\"/></div>";
                        cnt += "<span><%= Resources.Resource.Admin_MasterPageAdminCatalog_FPanel_EditMenu %></span></a></div>";
                    }
                    if (MenuType == 'Top') {
                        cnt += "<div><a onmouseover='this.style.color = \"#FF7F27\"' onmouseout='this.style.color = \"#000000\"' onclick=\"if(confirm('" + MenuName + "')){__doPostBack('<%= tree.UniqueID %>','c$DeleteMenuItem#" + MenuId + "')} return false;\"> <div class=\"hoverPanel_img_container\"><img onmouseout=\"this.src = 'images/gcross.gif';\" onmouseover=\"this.src = 'images/bcross.gif';\" src=\"images/gcross.gif\" alt=\"\"/></div>";
                    } else {
                        cnt += "<div><a onmouseover='this.style.color = \"#FF7F27\"' onmouseout='this.style.color = \"#000000\"' onclick=\"if(confirm('" + MenuName + "')){__doPostBack('<%= treeBottom.UniqueID %>','c$DeleteMenuItem#" + MenuId + "')} return false;\"> <div class=\"hoverPanel_img_container\"><img onmouseout=\"this.src = 'images/gcross.gif';\" onmouseover=\"this.src = 'images/bcross.gif';\" src=\"images/gcross.gif\" alt=\"\"/></div>";
                    }
                    cnt += "<span><%= Resources.Resource.Admin_MasterPageAdminCatalog_FPanel_DeleteMenu %></span></a></div></div>";



                    $(this).qtip({
                        content: cnt,
                        position: { corner: { target: 'bottomLeft', tooltip: "topLeft" }, adjust: { screen: true} },
                        //раскомментировать в случае падения производительности скриптов на странице
                        hide: { fixed: true, delay: 100 /*,effect: function () { $(this).stop(true, true).hide(); }*/ },
                        show: { solo: true, delay: 600 /*,effect: function () { $(this).stop(true, true).show(); }*/ }
                    });

                    $(this).mouseover(function () {
                        $(this).addClass("AdminTree_HoverNodeStyle");
                    });

                    $(this).mouseout(function () {
                        $(this).removeClass("AdminTree_HoverNodeStyle");
                    });
                    return true;
                });
            }

            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () { setupTooltips(); setupHoverPanel(); });

            function hide_wait_for_node(node) {
                if (node.wait_img) {
                    node.removeChild(node.wait_img);
                }
            }

            function show_wait_for_node(node) {
                var wait_img = document.createElement("IMG");
                wait_img.src = "images/loader.gif";
                wait_img.border = 0;
                node.wait_img = wait_img;
                node.appendChild(wait_img);
            }

            function esc_press(D) {
                D = D || window.event;
                var A = D.keyCode;
                if (A == 27) {
                    HideModalPopupNews();
                    HideModalPopupProduct();
                    HideModalPopupCategory();
                    HideModalPopupAux();
                }
            }
            document.onkeydown = esc_press;

            $(document).ready(function () {
                $("#commandButton").click(function () {
                    var command = $("#commandSelect").val();

                    switch (command) {
                        case "selectAll":
                            SelectAll(true);
                            break;
                        case "unselectAll":
                            SelectAll(false);
                            break;
                        case "selectVisible":
                            SelectVisible(true);
                            break;
                        case "unselectVisible":
                            SelectVisible(false);
                            break;
                        case "deleteSelected":
                            var r = confirm("<%= Resources.Resource.Admin_MenuManager_Confirm%>");
                            if (r) document.getElementById('<%=lbDeleteSelected.ClientID%>').click();
                            //if (r) __doPostBack('<%=lbDeleteSelected.UniqueID%>', '');
                            break;
                    }
                });
            });

            function togglePanel() {
                if (findCookie("isVisiblePanel") == "true" || findCookie("isVisiblePanel") == "") {
                    $("div:.leftPanel").hide("fast");
                    $("div:#divHide").hide("fast");
                    $("div:#divShow").show("fast");
                    addCookie("isVisiblePanel", "false", 7);
                } else {
                    $("div:.leftPanel").show("fast");
                    $("div:#divHide").show("fast");
                    $("div:#divShow").hide("fast");
                    addCookie("isVisiblePanel", "true", 7);
                }

            }

            $(document).ready(function () {
                if (findCookie("isVisiblePanel") != "false") {
                    showPanel();
                }
            });

            function findCookie(szName) {
                var i = 0;
                var nStartPosition = 0;
                var nEndPosition = 0;
                var szCookieString = document.cookie;

                while (i <= szCookieString.length) {
                    nStartPosition = i;
                    nEndPosition = nStartPosition + szName.length;

                    if (szCookieString.substring(nStartPosition, nEndPosition) == szName) {
                        nStartPosition = nEndPosition + 1;
                        nEndPosition = document.cookie.indexOf(";", nStartPosition);

                        if (nEndPosition < nStartPosition)
                            nEndPosition = document.cookie.length;

                        return document.cookie.substring(nStartPosition, nEndPosition);
                    }
                    i++;
                }
                return "";
            }

            function showPanel() {
                document.getElementById("leftPanel").style.display = "block";
                document.getElementById("divHide").style.display = "block";
                document.getElementById("divShow").style.display = "none";
            }

            function addCookie(szName, szValue, dtDaysExpires) {
                var dtExpires = new Date();
                var dtExpiryDate;

                dtExpires.setTime(dtExpires.getTime() + dtDaysExpires * 24 * 60 * 60 * 1000);

                dtExpiryDate = dtExpires.toGMTString();

                document.cookie = szName + "=" + szValue + "; expires=" + dtExpiryDate;
            }

            var timeOut;
            function Darken() {
                timeOut = setTimeout('document.getElementById("inprogress").style.display = "block";', 1000);
            }

            function Clear() {
                clearTimeout(timeOut);

                $("input.sel").each(function (i) {
                    if (this.checked) $(this).parent().parent().addClass("selectedrow");
                });

                initgrid();
            }

            $(document).ready(function () {
                document.onkeydown = keyboard_navigation;
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                prm.add_beginRequest(Darken);
                prm.add_endRequest(Clear);
                initgrid();
                $("ineditcategory").tooltip();
            });
        </script>
    </div>
</asp:Content>
