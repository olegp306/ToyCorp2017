﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true"
    CodeFile="Properties.aspx.cs" Inherits="Admin.Properties" %>

<%@ Import Namespace="Resources" %>
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

        $(document).ready(function () {
            document.onkeydown = keyboard_navigation;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(Darken);
            prm.add_endRequest(Clear);
            initgrid();
            $("ineditcategory").tooltip();
        });

        $(document).ready(function () {
            $("#commandButton").click(function (e) {
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
                        var r = confirm("<%= Resource.Admin_Catalog_Confirm%>");
                        if (r) __doPostBack('<%=lbDeleteSelected.UniqueID%>', '');
                        break;
                    case "setInFilter":
                        var r = confirm("<%= Resource.Admin_Properties_UseInFilterConfirm%>");
                        if (r) __doPostBack('<%=lbSetInFilter.UniqueID%>', '');
                        break;
                    case "setNotInFilter":
                        var r = confirm("<%= Resource.Admin_Properties_NotUseInFilterConfirm%>");
                        if (r) __doPostBack('<%=lbSetNotInFilter.UniqueID%>', '');
                        break;

                    case "changeGroup":
                        showModalGroups(e);
                        break;
                }
            });
        });

    </script>
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
                            <img src="images/ajax-loader.gif" alt="" />
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
            <li class="neighbor-menu-item"><a href="Catalog.aspx">
                <%= Resource.Admin_MasterPageAdmin_CategoryAndProducts %></a></li>
            <li class="neighbor-menu-item dropdown-menu-parent"><a href="ProductsOnMain.aspx?type=New">
                <%= Resource.Admin_MasterPageAdminCatalog_FirstPageProducts%></a>
                <div class="dropdown-menu-wrap">
                    <ul class="dropdown-menu">
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=Bestseller">
                            <%= Resource.Admin_MasterPageAdminCatalog_BestSellers %>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=New">
                            <%= Resource.Admin_MasterPageAdminCatalog_New %>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=Discount">
                            <%= Resource.Admin_MasterPageAdminCatalog_Discount %>
                        </a></li>
                    </ul>
                </div>
            </li>
            <li class="neighbor-menu-item dropdown-menu-parent selected"><a href="Properties.aspx">
                <%= Resource.Admin_MasterPageAdmin_Directory%></a>
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
            <li class="neighbor-menu-item"><a href="ExportCSV.aspx">
                <%= Resource.Admin_MasterPageAdmin_Export%></a></li>
            <li class="neighbor-menu-item"><a href="ImportCSV.aspx">
                <%= Resource.Admin_MasterPageAdmin_Import%></a></li>
            <li class="neighbor-menu-item"><a href="Brands.aspx">
                <%= Resource.Admin_MasterPageAdmin_Brands%></a></li>
            <li class="neighbor-menu-item"><a href="Reviews.aspx">
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
    <div class="content-own">
        <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td style="width: 72px;"><img src="images/orders_ico.gif" alt="" /></td>
            <td>
                <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_Properties_Header %>"></asp:Label><br />
                <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_Properties_ListPropreties %>"></asp:Label>
            </td>
            <td style="vertical-align: bottom; padding-right: 10px"></td>
        </tr>
        </table>

        <ul class="two-column">
            <li class="two-column-item prop-groups">
                
                <div class="justify">
                    <h2 class="justify-item products-header"><%= Resource.Admin_PropertyGroups_Header %></h2>
                    <div class="justify-item products-header-controls">
                        <a href="m_PropertyGroup.aspx" class="showtooltip  products-header-controls" onclick="open_window('m_PropertyGroup.aspx', 750, 640); return false;">
                            <img src="images/gplus.gif" onmouseover="this.src='images/bplus.gif'" onmouseout="this.src='images/gplus.gif';"  title="<%= Resource.Admin_PropertyGroups_AddNew %>" /></a>
                    </div>
                </div>

                <asp:ListView runat="server" ID="lvGroups" ItemPlaceholderID="item" OnItemCommand="lvGroups_OnItemCommand">
                <LayoutTemplate>
                    <div class="prop-groups">
                        <div id="item" runat="server"></div>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class='prop-group-item<%# (int)Eval("PropertyGroupId") == GroupId ? " active" : "" %>'>
                        <a class="p-group" href='Properties.aspx<%# (int)Eval("PropertyGroupId") != 0 ? "?groupId=" + Eval("PropertyGroupId") : "" %>'><%# Eval("Name") %></a>
                            
                        <div class="group-options" runat="server" Visible='<%# (int)Eval("PropertyGroupId") == GroupId && (int)Eval("PropertyGroupId") != 0%>'>
                            <a id="editgroup" href="#" onclick="open_window('m_PropertyGroup.aspx?groupId=<%#Eval("PropertyGroupId") %>', 750, 640); return false;">
                                <img title="<%= Resource.Admin_Edit %>" src="images/editbtn.gif" />
                            </a>
                            <asp:LinkButton id="delgroup" runat="server" CommandName="DeleteGroup" CommandArgument='<%#Eval("PropertyGroupId") %>'
                                            CssClass="deletebtn valid-confirm"
                                            data-confirm="<%$ Resources:Resource, Admin_ConfirmDeleting %>"  />
                        </div>
                    </div>
                </ItemTemplate>
                </asp:ListView>

            </li>
            <li class="two-column-item">
                
            <div>
                <div class="btns-main">
                    <asp:Button CssClass="btn btn-middle btn-add" ID="btnAddProperty" runat="server" Text="<%$ Resources:Resource, Admin_Properties_Add %>" ValidationGroup="0" />
                </div>
                <div style="height: 10px">
                </div>
                <table style="width: 99%;" class="massaction">
                    <tr>
                        <td>
                            <span class="admin_catalog_commandBlock"><span style="display: inline-block; margin-right: 3px;">
                                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_Command %>"></asp:Localize>
                            </span><span style="display: inline-block">
                                <select id="commandSelect">
                                    <optgroup label="<%= Resource.Admin_Properties_Select %>">
                                        
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
                                        
                                    </optgroup>
                                    <optgroup label="<%= Resource.Admin_Properties_Edit %>">

                                        <option value="deleteSelected">
                                            <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DeleteSelected %>"></asp:Localize>
                                        </option>
                                        <option value="setInFilter">
                                            <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_ShowInFilter %>"></asp:Localize>
                                        </option>
                                        <option value="setNotInFilter">
                                            <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DontShowInFilter %>"></asp:Localize>
                                        </option>
                                        <option value="changeGroup">
                                            <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources:Resource, Admin_PropertyGroups_ChangeGroup %>"></asp:Localize>
                                        </option>
                                    </optgroup>
                                </select>
                                <input id="commandButton" type="button" value="GO" style="font-size: 10px; width: 38px; height: 20px;" />
                                <asp:LinkButton ID="lbDeleteSelected" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DeleteSelected %>"
                                    OnClick="lbDeleteSelected_Click" />
                                <asp:LinkButton ID="lbSetInFilter" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_SetActive %>"
                                    OnClick="lbSetInFilter_Click" />
                                <asp:LinkButton ID="lbSetNotInFilter" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_SetDeactive %>"
                                    OnClick="lbSetNotInFilter_Click" />
                            </span><span class="selecteditems" style="vertical-align: baseline"><span style="padding: 0px 3px 0px 3px;">|</span><span id="selectedIdsCount" class="bold">0</span>&nbsp;<span><%=Resource.Admin_Catalog_ItemsSelected%></span></span>
                            </span>
                        </td>
                        <td align="right" class="selecteditems">
                            <asp:UpdatePanel ID="upCounts" runat="server" UpdateMode="Conditional">
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                                </Triggers>
                                <ContentTemplate>
                                    <%=Resource.Admin_Catalog_Total%>
                                    <span class="bold">
                                        <asp:Label ID="lblFound" CssClass="foundrecords" runat="server" Text="" /></span>&nbsp;<%=Resource.Admin_Catalog_RecordsFound%>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                        <td style="width: 8px;"></td>
                    </tr>
                </table>
                <asp:Panel runat="server" ID="filterPanel" DefaultButton="btnFilter">
                    <table class="filter" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="height: 5px;" colspan="9"></td>
                        </tr>
                        <tr>
                            <td style="width: 60px; text-align: center;">
                                <div style="width: 60px; height: 1px;">
                                </div>
                                <asp:DropDownList ID="ddSelect" TabIndex="10" CssClass="dropdownselect" runat="server"
                                    Width="60">
                                    <asp:ListItem Selected="True" Text="<%$ Resources:Resource, Admin_Catalog_Any %>" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" />
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox CssClass="filtertxtbox" ID="txtName" runat="server" TabIndex="12" Width="99%" />
                            </td>
                            <td style="width: 110px; text-align: center;">
                                <div style="width: 110px; height: 1px;">
                                </div>
                                <asp:DropDownList ID="ddlUseInFilter" TabIndex="13" CssClass="dropdownselect" runat="server">
                                    <asp:ListItem Selected="True" Text="<%$ Resources:Resource, Admin_Catalog_Any %>"
                                        Value="Any" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" Value="1" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" Value="0" />
                                </asp:DropDownList>
                            </td>
                            <td style="width: 110px; text-align: center;">
                                <div style="width: 110px; height: 1px;">
                                </div>
                                <asp:DropDownList ID="ddlUseInDetails" TabIndex="13" CssClass="dropdownselect" runat="server">
                                    <asp:ListItem Selected="True" Text="<%$ Resources:Resource, Admin_Catalog_Any %>"
                                        Value="Any" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" Value="1" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" Value="0" />
                                </asp:DropDownList>
                            </td>
                            <td style="width: 110px; text-align: center;">
                                <div style="width: 110px; height: 1px;">
                                </div>
                                <asp:DropDownList ID="ddlUseInBrief" TabIndex="13" CssClass="dropdownselect" runat="server">
                                    <asp:ListItem Selected="True" Text="<%$ Resources:Resource, Admin_Catalog_Any %>"
                                        Value="Any" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" Value="1" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" Value="0" />
                                </asp:DropDownList>
                            </td>
                            <td style="width: 110px;">
                                <div style="width: 110px; height: 1px;">
                                </div>
                                <asp:TextBox CssClass="filtertxtbox" ID="txtSortOrder" Width="90%" runat="server"
                                    TabIndex="14" />
                            </td>
                            <td style="width: 110px; text-align: center;">
                                <div style="width: 110px; height: 1px;">
                                </div>
                                <asp:TextBox CssClass="filtertxtbox" ID="txtProductsCount" Width="90%" runat="server"
                                    TabIndex="14" />
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <div style="width: 50px; height: 1px;">
                                </div>
                                <asp:Button ID="btnFilter" runat="server" CssClass="btn" Width="50" OnClientClick="javascript:FilterClick();"
                                    TabIndex="23" Text="<%$ Resources:Resource, Admin_Catalog_Filter %>" OnClick="btnFilter_Click" />
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <div style="width: 50px; height: 1px;">
                                </div>
                                <asp:Button ID="btnReset" runat="server" CssClass="btn" Width="50" OnClientClick="javascript:ResetFilter();"
                                    TabIndex="24" Text="<%$ Resources:Resource, Admin_Catalog_Reset %>" OnClick="btnReset_Click" />
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 5px;" colspan="9"></td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="pageNumberer" EventName="SelectedPageChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbDeleteSelected" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="lbSetInFilter" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="lbSetNotInFilter" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnGo" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnAddProperty" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ddRowsPerPage" EventName="SelectedIndexChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <adv:AdvGridView ID="grid" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                            CellPadding="0" CellSpacing="0" Confirmation="<%$ Resources:Resource, Admin_Properties_Confirmation %>"
                            CssClass="tableview" Style="cursor: pointer" DataFieldForEditURLParam="" DataFieldForImageDescription=""
                            DataFieldForImagePath="" EditURL="" GridLines="None" OnRowCommand="grid_RowCommand"
                            OnSorting="grid_Sorting" OnRowDeleting="grid_RowDeleting" ShowFooter="false">
                            <Columns>
                                <asp:TemplateField AccessibleHeaderText="ID" Visible="false">
                                    <EditItemTemplate>
                                        <asp:Label ID="Label0" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="checkboxcolumn" ItemStyle-Width="60px" HeaderStyle-Width="60px"
                                    HeaderStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="checkBoxCheckAll" CssClass="checkboxcheckall" runat="server" onclick="javascript:SelectVisible(this.checked);" />
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <%# ((bool)Eval("IsSelected"))? "<input type='checkbox' class='sel' checked='checked' />": "<input type='checkbox' class='sel' />"%>
                                        <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Eval("ID") %>' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <div style="width: 60px; font-size: 0px">
                                        </div>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="Name" HeaderStyle-HorizontalAlign="Left">
                                    <HeaderTemplate>
                                        <asp:LinkButton ID="lbOrderName" runat="server" CommandName="Sort" CommandArgument="Name">
                                            <%=Resource.Admin_Catalog_Name%>
                                            <asp:Image ID="arrowName" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                        </asp:LinkButton>
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="txtNameBind" runat="server" Text='<%# Eval("Name") %>' Width="99%"></asp:TextBox>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox ID="txtNewName" runat="server" CssClass="add" Text='' Width="99%"></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="UseInFilter" ItemStyle-Width="100" HeaderStyle-Width="100"
                                    ItemStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:LinkButton ID="lbOrderUseInFilter" runat="server" CommandName="Sort" CommandArgument="UseInFilter">
                                            <%=Resource.Admin_Properties_UseFilter%>
                                            <asp:Image ID="arrowUseInFilter" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                        </asp:LinkButton>
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="ckbUseInFilter" runat="server" Checked='<%# Eval("UseInFilter") %>' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:CheckBox ID="chkNewUse" runat="server" CssClass="add" Checked="True" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="UseInDetails" ItemStyle-Width="100" HeaderStyle-Width="100"
                                    ItemStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:LinkButton ID="lbOrderUseInDetails" runat="server" CommandName="Sort" CommandArgument="UseInDetails">
                                            <%=Resource.Admin_Properties_UseInDetails%>
                                            <asp:Image ID="arrowUseInDetails" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                        </asp:LinkButton>
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="ckbUseInDetails" runat="server" Checked='<%# Eval("UseInDetails") %>' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:CheckBox ID="chkNewUseInDetails" runat="server" CssClass="add" Checked="True" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="UseInBrief" ItemStyle-Width="100" HeaderStyle-Width="100"
                                    ItemStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:LinkButton ID="lbOrderUseInBrief" runat="server" CommandName="Sort" CommandArgument="UseInBrief">
                                            <%=Resource.Admin_Properties_UseInBrief%>
                                            <asp:Image ID="arrowUseInBrief" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                        </asp:LinkButton>
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox ID="ckbUseInBrief" runat="server" Checked='<%# Eval("UseInBrief") %>' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:CheckBox ID="chkNewUseInBrief" runat="server" CssClass="add" Checked="True" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="SortOrder" ItemStyle-Width="100" HeaderStyle-Width="100">
                                    <HeaderTemplate>
                                        <asp:LinkButton ID="lbOrderSortOrder" runat="server" CommandName="Sort" CommandArgument="SortOrder">
                                            <%=Resource.Admin_Properties_SortOrder%>
                                            <asp:Image ID="arrowSortOrder" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                        </asp:LinkButton>
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="txtSortOrderBind" Width="95%" Text='<%# Eval("SortOrder") %>'></asp:TextBox>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:TextBox runat="server" ID="txtNewSortOrder" Width="95%" CssClass="add" Text=''></asp:TextBox>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="ProductsCount" ItemStyle-Width="100" HeaderStyle-Width="100">
                                    <HeaderTemplate>
                                        <asp:LinkButton ID="lbProductsCount" runat="server" CommandName="Sort" CommandArgument="ProductsCount">
                                            <%=Resource.Admin_Properties_ProductsCount%>
                                            <asp:Image ID="arrowProductsCount" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                        </asp:LinkButton>
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:Label runat="server" ID="lblProductsCount" Text='<%# Eval("ProductsCount") %>'></asp:Label>
                                    </EditItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="50px" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center"
                                    FooterStyle-HorizontalAlign="Center">
                                    <EditItemTemplate>
                                        <%--<img src="images/editbtn.gif" class="editbtn showtooltip" style="border: none;" alt='' title="<%= Resource.Admin_MasterPageAdminCatalog_Edit %>" />--%>
                                        <a runat="server" href='#' onclick='<%# "open_window(\"m_Property.aspx?propertyId=" + Eval("ID").ToString() + "\", 750, 640); return false;"%>'
                                            class="editbtn showtooltip" style="display: inline-block;" title="<%$ Resources:Resource, Admin_MasterPageAdminCatalog_Edit %>">
                                            <img src="images/editbtn.gif" style="border: none;" alt='' />
                                        </a>

                                        <a id="A1" runat="server" href='<%# "PropertyValues.aspx?PropertyID=" + Eval("ID") %>'
                                            class="editbtn showtooltip" style="display: inline-block;" title="<%$ Resources:Resource, Admin_Properties_EditValues %>">
                                            <img src="images/list.gif" style="border: none;" alt='' />
                                        </a>                                        
                                        <input id="btnUpdate" name="btnUpdate" class="updatebtn showtooltip" type="image"
                                            src="images/updatebtn.png" onclick="<%#ClientScript.GetPostBackEventReference(grid, "Update$" + Container.DataItemIndex)%>; return false;"
                                            style="display: none" title="<%=Resource.Admin_MasterPageAdminCatalog_Update %>" />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:ImageButton ID="ibAddProperty" runat="server" ImageUrl="images/addbtn.gif" CommandName="AddProperty"
                                            ToolTip="<%$ Resources:Resource, Admin_Property_Add  %>" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="30px" HeaderStyle-Width="30px" ItemStyle-HorizontalAlign="Left"
                                    FooterStyle-HorizontalAlign="Left">
                                    <EditItemTemplate>
                                        <asp:LinkButton ID="buttonDelete" runat="server" 
                                            CssClass="deletebtn showtooltip valid-confirm" CommandName="DeleteProperty" CommandArgument='<%# Eval("ID")%>'
                                            data-confirm="<%$ Resources:Resource, Admin_Properties_Confirmation %>"
                                            ToolTip='<%$ Resources:Resource, Admin_MasterPageAdminCatalog_Delete %>' />
                                        <input id="btnCancel" name="btnCancel" class="cancelbtn showtooltip" type="image"
                                            src="images/cancelbtn.png" onclick="row_canceledit($(this).parent().parent()[0]); return false;"
                                            style="display: none" title="<%=Resource.Admin_MasterPageAdminCatalog_Cancel %>" />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <asp:ImageButton ID="ibCancelAdd" runat="server" ImageUrl="images/cancelbtn.png"
                                            CommandName="CancelAdd" ToolTip="<%$ Resources:Resource, Admin_Property_CancelAdd  %>" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <FooterStyle BackColor="#ccffcc" />
                            <HeaderStyle CssClass="header" />
                            <RowStyle CssClass="row1 propertiesRow_25 readonlyrow" />
                            <AlternatingRowStyle CssClass="row2 propertiesRow_25_alt readonlyrow" />
                            <EmptyDataTemplate>
                                <div style="text-align: center; margin-top: 20px; margin-bottom: 20px;">
                                    <%=Resource.Admin_Catalog_NoRecords%>
                                </div>
                            </EmptyDataTemplate>
                        </adv:AdvGridView>
                        <table class="results2">
                            <tr>
                                <td style="width: 157px; padding-left: 6px;">
                                    <%=Resource.Admin_Catalog_ResultPerPage%>:&nbsp;<asp:DropDownList ID="ddRowsPerPage"
                                        runat="server" OnSelectedIndexChanged="btnFilter_Click" CssClass="droplist" AutoPostBack="true">
                                        <asp:ListItem>10</asp:ListItem>
                                        <asp:ListItem Selected="True">20</asp:ListItem>
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
                                            <%=Resource.Admin_Catalog_PageNum%>&nbsp;<asp:TextBox ID="txtPageNum" runat="server"
                                                Width="30" /></span>
                                        <asp:Button ID="btnGo" runat="server" CssClass="btn" Text="<%$ Resources:Resource, Admin_Catalog_GO %>"
                                            OnClick="linkGO_Click" />
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                        
                        <div style="display: none">
                            <input  type="hidden" id="hfgroupId" class="hfgroupId" runat="server"  />
                            <asp:Button runat="server" ID="btnChangeGroup" CssClass="btn-change-group"  OnClick="btnChangeGroup_Click" Text="change" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <input type="hidden" id="SelectedIds" name="SelectedIds" />
            </div>
            <div class="change-group-b" style="display: none">
                <asp:DropDownList runat="server" ID="ddlAllGroups" DataTextField="Name" DataValueField="PropertyGroupId" Width="230px" />
            </div>
            <script type="text/javascript">
                function setupTooltips() {
                    $(".showtooltip").tooltip({
                        showURL: false
                    });
                }
                Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () { setupTooltips(); });
            </script>
            </li>
        </ul>
    </div>
</asp:Content>
