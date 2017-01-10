<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true"
    CodeFile="ProductsOnMain.aspx.cs" Inherits="Admin.ProductsOnMain" ValidateRequest="false" %>

<%@ Import Namespace="Resources" %>
<%@ Register TagPrefix="adv" TagName="PopupTree" Src="~/Admin/UserControls/PopupTreeView.ascx" %>
<%@ Register TagPrefix="adv" TagName="MainPageProduct" Src="~/Admin/UserControls/MainPageProduct.ascx" %>
<%@ Register TagPrefix="adv" TagName="CatalogPart" Src="~/Admin/UserControls/Catalog/CatalogPart.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Head" runat="Server">
    <script type="text/javascript">

        function CreateHistory(hist) {
            $.historyLoad(hist);
        }

        var timeOut;
        function Darken() {
            timeOut = setTimeout('document.getElementById("inprogress").style.display = "block";', 1000);
        }

        function Clear() {
            clearTimeout(timeOut);
            document.getElementById("inprogress").style.display = "none";

            $("input.sel").each(function (i) {
                if (this.checked) $(this).parent().parent().addClass("selectedrow");
            });

            initgrid();
        }

        function removeunloadhandler(a) {
            window.onbeforeunload = null;
        }

        function ATreeView_Select(sender, arg) {
            $("a.selectedtreenode").removeClass("selectedtreenode");
            $(sender).addClass("selectedtreenode");
            document.getElementById("TreeView_SelectedValue").value = arg;
            document.getElementById("TreeView_SelectedNodeText").value = sender.innerHTML;
            return false;
        }

        $(document).ready(function () {
            document.onkeydown = keyboard_navigation;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(Darken);
            prm.add_endRequest(Clear);
            initgrid();
            $("ineditcategory").tooltip();
        });

        function togglePanel() {
            if ($.cookie("isVisiblePanel") == "true" || $.cookie("isVisiblePanel") == "") {
                $("div:.leftPanel").hide("fast");
                $("div:#divHide").hide("fast");
                $("div:#divShow").show("fast");
                $.cookie("isVisiblePanel", "false", { expires: 7 });
            } else {
                $("div:.leftPanel").show("fast");
                $("div:#divHide").show("fast");
                $("div:#divShow").hide("fast");
                $.cookie("isVisiblePanel", "true", { expires: 7 });
            }
        }

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
                        var r = confirm("<%= Resources.Resource.Admin_Catalog_Confirm%>");
                        if (r) window.__doPostBack('<%=lbDeleteSelected.UniqueID%>', '');
                        break;
                }
            });
        });

    </script>
    <style type="text/css">
        .style1 {
            height: 24px;
        }

        .style2 {
            width: 8px;
            height: 24px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
    <div id="inprogress" style="display: none;">
        <div id="curtain" class="opacitybackground">
            &nbsp;
        </div>
        <div class="loader">
            <table width="100%" style="font-weight: bold; text-align: center;">
                <tbody>
                    <tr>
                        <td align="center">
                            <img src="images/ajax-loader.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="color: #0D76B8;">
                            <asp:Localize ID="Localize_Admin_Properties_PleaseWait" runat="server" Text="<%$ Resources:Resource, Admin_Properties_PleaseWait %>"></asp:Localize>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item"><a href="Catalog.aspx" class="neighbor-menu-lnk">
                <%= Resource.Admin_MasterPageAdmin_CategoryAndProducts %></a></li>
            <li class="neighbor-menu-item selected dropdown-menu-parent"><a href="ProductsOnMain.aspx?type=New"
                class="neighbor-menu-lnk">
                <%= Resource.Admin_MasterPageAdminCatalog_FirstPageProducts%></a>
                <div class="dropdown-menu-wrap">
                    <ul class="dropdown-menu">
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=Bestseller">
                            <%= Resources.Resource.Admin_MasterPageAdminCatalog_BestSellers %>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=New">
                            <%= Resources.Resource.Admin_MasterPageAdminCatalog_New %>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=Discount">
                            <%= Resources.Resource.Admin_MasterPageAdminCatalog_Discount %>
                        </a></li>
                    </ul>
                </div>
            </li>
            <li class="neighbor-menu-item dropdown-menu-parent"><a href="Properties.aspx">
                <%= Resources.Resource.Admin_MasterPageAdmin_Directory%></a>
                <div class="dropdown-menu-wrap">
                    <ul class="dropdown-menu">
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Properties.aspx">
                            <%= Resource.Admin_MasterPageAdmin_ProductProperties%>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Colors.aspx">
                            <%= Resource.Admin_MasterPageAdmin_ColorDictionary%>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Sizes.aspx">
                            <%= Resource.Admin_MasterPageAdmin_SizeDictionary%>
                        </a></li>
                    </ul>
                </div>
            </li>
            <li class="neighbor-menu-item"><a href="ExportCSV.aspx" class="neighbor-menu-lnk">
                <%= Resource.Admin_MasterPageAdmin_Export%></a></li>
            <li class="neighbor-menu-item"><a href="ImportCSV.aspx" class="neighbor-menu-lnk">
                <%= Resource.Admin_MasterPageAdmin_Import%></a></li>
            <li class="neighbor-menu-item"><a href="Brands.aspx" class="neighbor-menu-lnk">
                <%= Resource.Admin_MasterPageAdmin_Brands%></a></li>
            <li class="neighbor-menu-item"><a href="Reviews.aspx" class="neighbor-menu-lnk">
                <%= Resource.Admin_MasterPageAdmin_Reviews%></a></li>
        </menu>
        <div class="panel-add">
            <%= Resource.Admin_MasterPageAdmin_Add %>
            <a href="Product.aspx?categoryid=<%=Request["categoryid"] ?? "0" %>" class="panel-add-lnk">
                <%= Resource.Admin_MasterPageAdmin_Product %></a>, <a href="#" onclick="open_window('m_Category.aspx?CategoryID=<%=Request["categoryid"] ?? "0" %>&mode=create', 750, 640); return false;"
                    class="panel-add-lnk">
                    <%= Resource.Admin_MasterPageAdmin_Category %></a>
        </div>
    </div>
    <ul class="two-column">
        <li class="two-column-item">
            <div class="panel-toggle">
                <adv:CatalogPart ID="CatalogPart1" runat="server" />
                <div class="justify">
                    <h2 class="justify-item catalog-tree-header">
                        <%= Resource.Admin_Catalog_Categories %>
                    </h2>
                    <div class="justify-item catalog-tree-controls">
                        <img id="ineditcategory" class="showtooltip" onclick="open_window('m_Category.aspx?CategoryID=0&mode=create', 750, 640); return false;"
                            onmouseover="this.src='images/bplus.gif'" title="<%= Resource.Admin_MasterPageAdminCatalog_AddNewCategory %>"
                            onmouseout="this.src='images/gplus.gif';" src="images/gplus.gif" />
                        <asp:ImageButton ID="ibRecalculate" CssClass="showtooltip" runat="server" ImageUrl="images/groundarrow.gif"
                            ToolTip="<%$ Resources:Resource, Admin_MasterPageAdminCatalog_Recalculate%>"
                            onmouseout="this.src='images/groundarrow.gif';" OnClick="ibRecalculate_Click" />
                        <input type="image" src="images/gudarrow.gif" class="showtooltip" onclick="open_window('m_CategorySortOrder.aspx', 750, 640); return false;"
                            title="<%= Resource.Admin_MasterPageAdminCatalog_SortOrder %>" onmouseover="this.src='images/budarrow.gif';"
                            onmouseout="this.src='images/gudarrow.gif';" />
                    </div>
                </div>
                <adv:CommandTreeView SkipLinkText="" CollapseImageUrl="images/new_admin/treeview/arrow-collapse.png"
                    ExpandImageUrl="images/new_admin/treeview/arrow-expand.png" CssClass="treeview"
                    ID="tree" runat="server" NodeWrap="False" ShowLines="False" OnTreeNodeCommand="tree_TreeNodeCommand"
                    OnTreeNodePopulate="tree_TreeNodePopulate">
                    <ParentNodeStyle ImageUrl="images/new_admin/treeview/folder.jpg" />
                    <SelectedNodeStyle ImageUrl="images/new_admin/treeview/folder-alt.jpg" Font-Bold="true" />
                    <NodeStyle ImageUrl="images/new_admin/treeview/folder.jpg" />
                </adv:CommandTreeView>
            </div>
        </li>
        <li class="two-column-item">
            <div>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tbody>
                        <tr>
                            <td style="width: 72px;">
                                <img src="images/orders_ico.gif" alt="" />
                            </td>
                            <td>
                                <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text=""></asp:Label><br />
                                <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_ProductsList %>"></asp:Label>
                                <asp:Label ID="lMessage" Style="float: left;" runat="server" ForeColor="Red" Visible="false"
                                    EnableViewState="false" />
                            </td>
                            <td style="vertical-align: bottom; padding-right: 10px">
                                <div class="btns-main">
                                    <asp:Button CssClass="btn btn-middle btn-add" ID="btnAddProduct" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_AddProduct %>" ValidationGroup="0" OnClick="btnAddProduct_Click"
                                        OnClientClick="document.body.style.overflowX='hidden';_TreePostBack=true;removeunloadhandler();" />
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div style="width: 100%">
                    <table style="width: 99%;" class="massaction">
                        <tr>
                            <td class="style1">
                                <span class="admin_catalog_commandBlock"><span style="display: inline-block; margin-right: 3px;">
                                    <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_Command %>"></asp:Localize>
                                </span><span style="display: inline-block;">
                                    <select id="commandSelect">
                                        <option value="selectAll">
                                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_SelectAll %>"></asp:Localize>
                                        </option>
                                        <option value="unselectAll">
                                            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_UnselectAll %>"></asp:Localize>
                                        </option>
                                        <option value="selectVisible">
                                            <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_SelectVisible %>"></asp:Localize>
                                        </option>
                                        <option value="unselectVisible">
                                            <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_UnselectVisible %>"></asp:Localize>
                                        </option>
                                        <option value="deleteSelected">
                                            <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DeleteSelected %>"></asp:Localize>
                                        </option>
                                    </select>
                                    <input id="commandButton" type="button" value="GO" style="font-size: 10px; width: 38px; height: 20px;" />
                                    <asp:LinkButton ID="lbDeleteSelected" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DeleteSelected %>"
                                        OnClick="lbDeleteSelected_Click" />
                                </span><span class="selecteditems" style="vertical-align: baseline"><span style="padding: 0px 3px 0px 3px;">|</span><span id="selectedIdsCount" class="bold">0</span>&nbsp;<span><%=Resources.Resource.Admin_Catalog_ItemsSelected%></span></span>
                                </span>
                            </td>
                            <td align="right" class="style1" style="text-align: right;">
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
                            <td class="style2"></td>
                        </tr>
                    </table>
                    <div style="border: 1px #c9c9c7 solid; width: 100%">
                        <asp:Panel runat="server" ID="filterPanel" DefaultButton="btnFilter">
                            <table class="filter" cellpadding="2" cellspacing="0">
                                <tr style="height: 5px;">
                                    <td colspan="5"></td>
                                </tr>
                                <tr>
                                    <td style="width: 70px; text-align: center;">
                                        <asp:DropDownList ID="ddSelect" TabIndex="10" CssClass="dropdownselect" runat="server"
                                            Width="65">
                                            <asp:ListItem Selected="True" Text="<%$ Resources:Resource, Admin_Catalog_Any %>" />
                                            <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" />
                                            <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" />
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <div style="width: 200px; font-size: 0px; height: 0px;">
                                        </div>
                                        <asp:TextBox CssClass="filtertxtbox" ID="txtName" Width="99%" runat="server" TabIndex="12" />
                                    </td>
                                    <td style="width: 150px;">
                                        <div style="width: 150px; font-size: 0px; height: 0px;">
                                        </div>
                                        <asp:TextBox CssClass="filtertxtbox" ID="txtSortOrder" Width="99%" runat="server"
                                            TabIndex="12" />
                                    </td>
                                    <td style="width: 90px;">
                                        <div style="width: 90px; font-size: 0px; height: 0px;">
                                        </div>
                                        <center>
                                                <asp:Button ID="btnFilter" runat="server" CssClass="btn" OnClientClick="javascript:FilterClick();"
                                                    TabIndex="23" Text="<%$ Resources:Resource, Admin_Catalog_Filter %>" OnClick="btnFilter_Click" />
                                                <asp:Button ID="btnReset" runat="server" CssClass="btn" OnClientClick="javascript:ResetFilter();"
                                                    TabIndex="24" Text="<%$ Resources:Resource, Admin_Catalog_Reset %>" OnClick="btnReset_Click" />
                                            </center>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 5px;" colspan="5"></td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="pageNumberer" EventName="SelectedPageChanged" />
                                <asp:AsyncPostBackTrigger ControlID="lbDeleteSelected" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnGo" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="ddRowsPerPage" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="btnAddProduct" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="popTree" EventName="TreeNodeSelected" />
                                <asp:AsyncPostBackTrigger ControlID="popTree" EventName="Hiding" />
                            </Triggers>
                            <ContentTemplate>
                                <input type="hidden" id="TreeView_SelectedValue" name="TreeView_SelectedValue" />
                                <input type="hidden" id="TreeView_SelectedNodeText" name="TreeView_SelectedNodeText" />
                                <adv:PopupTree runat="server" ID="popTree" OnTreeNodeSelected="popTree_Selected"
                                    Type="CategoryProduct" HeaderText="<%$ Resources:Resource, Admin_CatalogLinks_ParentCategory %>" />
                                <adv:AdvGridView ID="grid" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                    CellPadding="2" CellSpacing="0" CssClass="tableview" Style="cursor: pointer"
                                    DataFieldForEditURLParam="" DataFieldForImageDescription="" DataFieldForImagePath=""
                                    EditURL="" GridLines="None" OnRowCommand="grid_RowCommand" OnSorting="grid_Sorting"
                                    ShowFooter="false">
                                    <Columns>
                                        <asp:TemplateField AccessibleHeaderText="ID" Visible="false">
                                            <EditItemTemplate>
                                                <asp:Label ID="Label0" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="Label01" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="Label02" runat="server" Text='0'></asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-CssClass="checkboxcolumn" HeaderStyle-Width="70px" ItemStyle-Width="70px"
                                            HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <div style="width: 40px; height: 0px; font-size: 0px;">
                                                </div>
                                                <asp:CheckBox ID="checkBoxCheckAll" CssClass="checkboxcheckall" runat="server" onclick="javascript:SelectVisible(this.checked);" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <%# ((bool)Eval("IsSelected"))? "<input type='checkbox' class='sel' checked='checked' />": "<input type='checkbox' class='sel' />"%>
                                                <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Eval("ID") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="ArtNo" HeaderStyle-HorizontalAlign="Left">
                                            <HeaderTemplate>
                                                <div style="width: 150px; font-size: 0px; height: 0px;">
                                                </div>
                                                <asp:LinkButton ID="lbArtNo" runat="server" CommandName="Sort" CommandArgument="ArtNo">
                                                    <%=Resources.Resource.Admin_Catalog_StockNumber%>
                                                    <asp:Image ID="arrowArtNo" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                                </asp:LinkButton>
                                            </HeaderTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lArtNo" runat="server" Text='<%# Bind("ArtNo") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lArtNo2" runat="server" Text='<%# Bind("ArtNo") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="Name" HeaderStyle-HorizontalAlign="Left">
                                            <HeaderTemplate>
                                                <div style="width: 150px; font-size: 0px; height: 0px;">
                                                </div>
                                                <asp:LinkButton ID="lbName" runat="server" CommandName="Sort" CommandArgument="Name">
                                                    <%=Resources.Resource.Admin_Product_Name%>
                                                    <asp:Image ID="arrowName" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                                </asp:LinkButton>
                                            </HeaderTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="lName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lName2" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField AccessibleHeaderText="Sort" HeaderStyle-HorizontalAlign="Left"
                                            HeaderStyle-Width="150">
                                            <HeaderTemplate>
                                                <div style="width: 150px; font-size: 0px; height: 0px;">
                                                </div>
                                                <asp:LinkButton ID="lbSort" runat="server" CommandName="Sort" CommandArgument="Sort">
                                                    <%=Resources.Resource.Admin_Catalog_SortOrder%>
                                                    <asp:Image ID="arrowSort" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                                </asp:LinkButton>
                                            </HeaderTemplate>
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtSort" runat="server" Text='<%# Eval("Sort") %>' Width="99%"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lSort" runat="server" Text='<%# Bind("Sort") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-Width="90px" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="center"
                                            FooterStyle-HorizontalAlign="Center">
                                            <EditItemTemplate>
                                                <a id="cmdlink" runat="server" href='<%# "Product.aspx?productid=" + Eval("ID").ToString() %>'
                                                    class="editbtn showtooltip" title='<%$ Resources:Resource,Admin_MasterPageAdminCatalog_Edit%>'>
                                                    <img src="images/editbtn.gif" style="border: none;" alt="" /></a>
                                                <input id="btnUpdate" name="btnUpdate" class="updatebtn showtooltip" type="image"
                                                    src="images/updatebtn.png" onclick="<%#ClientScript.GetPostBackEventReference(grid, "Update$" + Container.DataItemIndex)%>; return false;"
                                                    style="display: none" title='<%= Resources.Resource.Admin_MasterPageAdminCatalog_Update%>' />
                                                <asp:LinkButton ID="buttonDelete" runat="server"
                                                    CssClass="deletebtn showtooltip valid-confirm" CommandName="DeleteProduct" CommandArgument='<%# Eval("ID")%>'
                                                    data-confirm="<%$ Resources:Resource, Admin_Catalog_Confirmation %>"
                                                    ToolTip='<%$ Resources:Resource, Admin_MasterPageAdminCatalog_DeleteFromlList %>' />
                                                <input id="btnCancel" name="btnCancel" class="cancelbtn showtooltip" type="image"
                                                    src="images/cancelbtn.png" onclick="row_canceledit($(this).parent().parent()[0]); return false;"
                                                    style="display: none" title="<%=Resources.Resource.Admin_MasterPageAdminCatalog_Cancel %>" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <FooterStyle BackColor="#ccffcc" />
                                    <HeaderStyle CssClass="header" />
                                    <RowStyle CssClass="row1 propertiesRow_25 readonlyrow" />
                                    <AlternatingRowStyle CssClass="row2 propertiesRow_25_alt readonlyrow" />
                                    <EmptyDataTemplate>
                                        <center style="margin-top: 20px; margin-bottom: 20px;">
                                                <%=Resources.Resource.Admin_Catalog_NoRecords%>
                                            </center>
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
                                                <asp:ListItem Selected="true">20</asp:ListItem>
                                                <asp:ListItem>50</asp:ListItem>
                                                <asp:ListItem>100</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="center">
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
        </li>
    </ul>
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
                var catId = $(this).attr("catId");
                var catName = $(this).attr("catName");
                if (catId != '0') {
                    var cnt = "<div class='hoverPanel'><div style='margin-bottom:5px;width:190px'><a onmouseover='this.style.color = \"#FF7F27\"' onmouseout='this.style.color = \"#000000\"' onclick=\"open_window('m_Category.aspx?CategoryID=" + catId + "&mode=create',750,640); return false;\"> <div class=\"hoverPanel_img_container\"><img onmouseout=\"this.src = 'images/glplus.gif';\" onmouseover=\"this.src = 'images/blplus.gif';\" src=\"images/glplus.gif\" alt=\"\"/></div>";
                    cnt += "<span><%= Resources.Resource.Admin_MasterPageAdminCatalog_FPanel_AddCategory %></span></a></div>";
                    cnt += "<div style='margin-bottom:5px;'><a onmouseover='this.style.color = \"#FF7F27\"' onmouseout='this.style.color = \"#000000\"' onclick=\"open_window('m_Category.aspx?CategoryID=" + catId + "&mode=edit',750,640); return false;\"> <div class=\"hoverPanel_img_container\"><img onmouseout=\"this.src = 'images/gpencil.gif';\" onmouseover=\"this.src = 'images/bpencil.gif';\" src=\"images/gpencil.gif\" alt=\"\"/></div>";
                    cnt += "<span><%= Resources.Resource.Admin_MasterPageAdminCatalog_FPanel_EditCategory %></span></a></div>";
                    cnt += "<div><a onmouseover='this.style.color = \"#FF7F27\"' onmouseout='this.style.color = \"#000000\"' onclick=\"if(confirm('" + catName + "')){__doPostBack('<%= tree.UniqueID %>','c$DeleteCategory#" + catId + "')} return false;\"> <div class=\"hoverPanel_img_container\"><img onmouseout=\"this.src = 'images/gcross.gif';\" onmouseover=\"this.src = 'images/bcross.gif';\" src=\"images/gcross.gif\" alt=\"\"/></div>";
                    cnt += "<span><%= Resources.Resource.Admin_MasterPageAdminCatalog_FPanel_DeleteCategory %></span></a></div></div>";
                }
                else {
                    var cnt = "<div class='hoverPanel'><div style='margin-bottom:5px;width:190px'><a onmouseover='this.style.color = \"#FF7F27\"' onmouseout='this.style.color = \"#000000\"' onclick=\"open_window('m_Category.aspx?CategoryID=" + catId + "&mode=create',750,640); return false;\"> <div class=\"hoverPanel_img_container\"><img onmouseout=\"this.src = 'images/glplus.gif';\" onmouseover=\"this.src = 'images/blplus.gif';\" src=\"images/glplus.gif\" alt=\"\"/></div>";
                    cnt += "<span><%= Resources.Resource.Admin_MasterPageAdminCatalog_FPanel_AddCategory %></span></a></div>";
                    cnt += "<div style='margin-bottom:5px;'><a onmouseover='this.style.color = \"#FF7F27\"' onmouseout='this.style.color = \"#000000\"' onclick=\"open_window('m_Category.aspx?CategoryID=" + catId + "&mode=edit',750,640); return false;\"> <div class=\"hoverPanel_img_container\"><img onmouseout=\"this.src = 'images/gpencil.gif';\" onmouseover=\"this.src = 'images/bpencil.gif';\" src=\"images/gpencil.gif\" alt=\"\"/></div>";
                    cnt += "<span><%= Resources.Resource.Admin_MasterPageAdminCatalog_FPanel_EditCategory %></span></a></div></div>";
                }

                $(this).qtip({
                    content: cnt,
                    position: { corner: { target: 'bottomLeft', tooltip: "topLeft" }, adjust: { screen: true } },
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

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () { setupTooltips(); });

        function ATreeView_Select(sender, arg) {
            $("a.selectedtreenode").removeClass("selectedtreenode");
            $(sender).addClass("selectedtreenode");
            document.getElementById("TreeView_SelectedValue").value = arg;
            document.getElementById("TreeView_SelectedNodeText").value = sender.innerHTML;
            return false;
        }
    </script>
</asp:Content>
