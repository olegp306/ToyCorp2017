<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="ExportFeed.aspx.cs" Inherits="Admin.ExportFeedPage" %>
<%@ Import Namespace="Resources" %>
<%@ Register TagPrefix="uc1" TagName="SiteNavigation" Src="~/UserControls/Catalog/SiteNavigation.ascx" %>
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
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(Darken);
            prm.add_endRequest(Clear);
            initgrid();
        });

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
                    case "setActive":
                        document.getElementById('<%=lbSetActive.ClientID%>').click();
                        break;
                    case "setNotActive":
                        document.getElementById('<%=lbSetNotActive.ClientID%>').click();
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
            <li class="neighbor-menu-item"><a href="Discount_PriceRange.aspx">
                <%= Resource.Admin_MasterPageAdmin_DiscountMethods%></a></li>
            <li class="neighbor-menu-item"><a href="Coupons.aspx">
                <%= Resource.Admin_MasterPageAdmin_Coupons%></a></li>
            <li class="neighbor-menu-item"><a href="Certificates.aspx">
                <%= Resource.Admin_MasterPageAdmin_Certificate%></a></li>
            <li class="neighbor-menu-item"><a href="SendMessage.aspx">
                <%= Resource.Admin_MasterPageAdmin_SendMessage%></a></li>
            <li class="neighbor-menu-item"><a href="ExportFeed.aspx?ModuleId=YandexMarket">
                <%= Resources.Resource.Admin_MasterPageAdmin_YandexMarket %></a></li>
            <li class="neighbor-menu-item"><a href="ExportFeed.aspx?ModuleId=GoogleBase">
                <%= Resources.Resource.Admin_MasterPageAdmin_GoogleBase %></a></li>
            <li class="neighbor-menu-item dropdown-menu-parent"><a href="Voting.aspx">
                <%= Resource.Admin_MasterPageAdmin_Voting%></a>
                <div class="dropdown-menu-wrap">
                    <ul class="dropdown-menu">
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Voting.aspx"><%= Resources.Resource.Admin_MasterPageAdmin_Voting %>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="VotingHistory.aspx"><%= Resources.Resource.Admin_MasterPageAdmin_VotingHistory %>
                        </a></li>
                    </ul>
                </div>
            </li>
            <li class="neighbor-menu-item"><a href="Statistics.aspx">
                <%= Resources.Resource.Admin_Statistics_Header %></a></li>
            <li class="neighbor-menu-item"><a href="SiteMapGenerate.aspx">
                <%= Resource.Admin_SiteMapGenerate_Header%></a></li>
            <li class="neighbor-menu-item"><a href="SiteMapGenerateXML.aspx">
                <%= Resource.Admin_SiteMapGenerateXML_Header%></a></li>
        </menu>
    </div>
    <div class="content-own">
        <div class="pageHeader">
            <span class="AdminHead">
                <asp:Literal ID="Literal1" runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_PageHeader %>' /></span>
            <span id="PageSubheader" visible="false" runat="Server">
                <br />
                <span class="AdminSubHead">
                    <asp:Literal ID="Literal2" runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_PageSubHeader %>' />
                    <asp:Literal ID="ModuleNameLiteral" runat="Server" /></span>
                <br />
                <br />
            </span>
        </div>
        <br />
        <div class="ui-tabs">
            <ul class="ui-tabs-nav">
                <li class="ui-tabs-selected"><a href="javascript:void();">
                    <%=Resources.Resource.Admin_ExportFeed_ChooseProduct%></a></li>
                <% if (ModuleName != "CsvExport")
                   { %>
                <li><a href="ExportFeedDet.aspx?moduleid=<%= ModuleName %>">
                    <%=Resources.Resource.Admin_ExportFeed_ModuleSettings %></a></li>
                <% } %>
            </ul>
            <div id="tabs-1" class="ui-tabs-panel">
                <table width="100%">
                    <tr>
                        <td style="height: 400px; width: 300px; vertical-align: top;">
                            <div class="" style="width: 300px; padding: 10px; overflow-x: auto;">
                                <asp:TreeView ID="tree2" ForeColor="Black" SelectedNodeStyle-BackColor="Blue" PopulateNodesFromClient="true"
                                    runat="server" ShowLines="True" ExpandImageUrl="images/loading.gif" OnTreeNodePopulate="PopulateNode2">
                                    <SelectedNodeStyle BackColor="Yellow" />
                                </asp:TreeView>
                                <table style="margin-top: 10px">
                                    <tr>
                                        <td>
                                            <div style="height: 10px; width: 10px; background-color: blue;">
                                            </div>
                                        </td>
                                        <td>
                                            <%=Resources.Resource.Admin_ExportFeed_ExportFullCategory%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div style="height: 10px; width: 10px; background-color: black;">
                                            </div>
                                        </td>
                                        <td>
                                            <%=Resources.Resource.Admin_ExportFeed_OnlyProduct%>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                        <td style="vertical-align: top">
                            <div style="width: 100%">
                                <div>
                                    <table cellpadding="0" cellspacing="0" width="100%">
	                                    <tbody>
		                                    <tr>
			                                    <td style="width: 72px;">
				                                    <img src="images/orders_ico.gif" alt="" />
			                                    </td>
			                                    <td colspan="2">
                                                    <div>
					                                    <asp:Label ID="lblCategoryName" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_MasterPageAdminCatalog_lblMain %>"></asp:Label>
				                                    </div>
				                                    <div>
					                                    <span class="AdminSubHead" style="display: inline-block;">
                                                            <%=Resources.Resource.Admin_MasterPageAdminCatalog_lblSubMain%>
                                                        </span>
                                                        <asp:Panel runat="server" ID="pnlCategorySet">
                                                            <label style="margin-top: 10px; display: inline-block;">
                                                                <asp:CheckBox AutoPostBack="True" runat="server" ID="chbFull" OnCheckedChanged="btnChange_OnClick" CssClass="checkly-align" style="margin-right:3px;" />
                                                                <%= Resources.Resource.Admin_ExportFeed_ExportCurrentCategory %>
                                                                <div data-plugin="help" class="help-block">
                                                                    <div class="help-icon js-help-icon"></div>
                                                                    <article class="bubble help js-help">
                                                                        <header class="help-header">
                                                                            Выгружать все товары
                                                                        </header>
                                                                        <div class="help-content">
                                                                            Отметьте эту галочку, если нужно выгружать все товары данной категории и всех подкатегорий.<br />
                                                                            <br />
                                                                            Если галочка снята, каждый товар, который нужно экспортировать, нужно будет отметить вручную.
                                                                        </div>
                                                                    </article>
                                                                </div>
                                                            </label>
                                                        </asp:Panel>
				                                    </div>
			                                    </td>
			                                    <td style="vertical-align:bottom;">
				                                     <div class="btns-main">
                                                        <asp:Button class="btn btn-middle btn-action" runat="server" ID="btnResetExport" Text="<%$ Resources:Resource, Admin_ExportFeed_ResetButton %>" OnClick="btnResetExport_OnClick" />&nbsp;
                                                        <a class="btn btn-middle btn-add" href="<%= ModuleName == "CsvExport" ? "ExportCsv.aspx" : "ExportFeedProgress.aspx?ModuleId=" + ModuleName + "&start=yes" %>"><%= Resource.Admin_ExportFeed_ExportButton %></a>
                                                    </div>
			                                    </td>
		                                    </tr>
	                                    </tbody>
                                    </table>
                                    <div id="siteNavigationBlock" style="margin-top:15px; margin-bottom:15px" runat="server">
                                        <span style="font-weight: bold;">
                                            <asp:Localize ID="Localize_Admin_Catalog_CategoryLocation" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_CategoryLocation %>"></asp:Localize>
                                        </span>
                                        <br />
                                        <uc1:SiteNavigation ID="sn" runat="server" />
                                        <asp:Label ID="lMessage" Style="float: left;" runat="server" ForeColor="Red" Visible="false" EnableViewState="false" />
                                    </div>
                                </div>
                                <asp:Panel ID="pnlData" runat="server">
                                    <table style="width: 99%;" class="massaction">
                                        <tr>
                                            <td style="height: 24px;">
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
                                                        <option value="setActive">
                                                            <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources:Resource, Admin_ExportFeed_SetExport %>"></asp:Localize>
                                                        </option>
                                                        <option value="setNotActive">
                                                            <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources:Resource, Admin_ExportFeed_SetNotExport %>"></asp:Localize>
                                                        </option>
                                                    </select>
                                                    <input id="commandButton" type="button" value="GO" style="font-size: 10px; width: 38px; height: 20px;" />
                                                    <asp:LinkButton ID="lbDeleteSelected" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DeleteSelected %>"
                                                        OnClick="lbDeleteSelected_Click" />
                                                    <asp:LinkButton ID="lbSetActive" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_SetActive %>"
                                                        OnClick="lbSetActive_Click" />
                                                    <asp:LinkButton ID="lbSetNotActive" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_SetDeactive %>"
                                                        OnClick="lbSetNotActive_Click" />
                                                </span><span class="selecteditems" style="vertical-align: baseline"><span style="padding: 0px 3px 0px 3px;">|</span><span id="selectedIdsCount" class="bold">0</span>&nbsp;<span><%=Resources.Resource.Admin_Catalog_ItemsSelected%></span></span></span></td>
                                            <td style="text-align: right; height: 24px;">
                                                <asp:UpdatePanel ID="upCounts" runat="server" UpdateMode="Conditional">
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
                                            <td style="width: 8px; height: 24px;"></td>
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
                                                        <asp:TextBox CssClass="filtertxtbox" ID="txtArtNo" Width="99%" runat="server" TabIndex="12" />
                                                    </td>
                                                    <td>
                                                        <div style="width: 200px; font-size: 0px; height: 0px;">
                                                        </div>
                                                        <asp:TextBox CssClass="filtertxtbox" ID="txtName" Width="99%" runat="server" TabIndex="12" />
                                                    </td>
                                                    <td style="width: 170px;">
                                                        <div style="width: 170px; font-size: 0px; height: 0px;">
                                                        </div>
                                                        <asp:TextBox CssClass="filtertxtbox" ID="txtSort" Width="99%" runat="server" TabIndex="12" />
                                                    </td>
                                                    <td style="width: 70px; text-align: center;">
                                                        <asp:DropDownList ID="ddlExport" TabIndex="10" CssClass="dropdownselect" runat="server"
                                                            Width="65">
                                                            <asp:ListItem Selected="True" Text="<%$ Resources:Resource, Admin_Catalog_Any %>" />
                                                            <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" />
                                                            <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td style="width: 90px;">
                                                        <div style="width: 90px; font-size: 0px; height: 0px;">
                                                        </div>
                                                        <div style="text-align: center;">
                                                            <asp:Button ID="btnFilter" runat="server" CssClass="btn" OnClientClick="javascript:FilterClick();"
                                                                TabIndex="23" Text="<%$ Resources:Resource, Admin_Catalog_Filter %>" OnClick="btnFilter_Click" />
                                                            <asp:Button ID="btnReset" runat="server" CssClass="btn" OnClientClick="javascript:ResetFilter();"
                                                                TabIndex="24" Text="<%$ Resources:Resource, Admin_Catalog_Reset %>" OnClick="btnReset_Click" />
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="height: 5px;" colspan="5"></td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="pageNumberer" EventName="SelectedPageChanged" />
                                                <asp:AsyncPostBackTrigger ControlID="lbDeleteSelected" EventName="Click" />
                                                <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                                                <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                                                <asp:AsyncPostBackTrigger ControlID="btnGo" EventName="Click" />
                                                <asp:AsyncPostBackTrigger ControlID="ddRowsPerPage" EventName="SelectedIndexChanged" />
                                            </Triggers>
                                            <ContentTemplate>
                                                <adv:AdvGridView ID="grid" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                    CellPadding="2" CellSpacing="0" CssClass="tableview" Style="cursor: pointer"
                                                    DataFieldForEditURLParam="" DataFieldForImageDescription="" DataFieldForImagePath=""
                                                    EditURL="" GridLines="None" OnRowCommand="grid_RowCommand" OnSorting="grid_Sorting"
                                                    ShowFooterWhenEmpty="true" ShowHeaderWhenEmpty="true">
                                                    <Columns>
                                                        <asp:TemplateField AccessibleHeaderText="ID" Visible="false">
                                                            <EditItemTemplate>
                                                                <asp:Label ID="Label0" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                                            </EditItemTemplate>
                                                            <ItemTemplate>
                                                                <asp:Label ID="Label01" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                                            </ItemTemplate>
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
                                                                    <%= Resources.Resource.Admin_ExportFeed_Grid_ArtNo%>
                                                                    <asp:Image ID="arrowArtNo" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                                                </asp:LinkButton>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lArtNo" runat="server" Text='<%# Bind("ArtNo") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField AccessibleHeaderText="Name" HeaderStyle-HorizontalAlign="Left">
                                                            <HeaderTemplate>
                                                                <div style="width: 150px; font-size: 0px; height: 0px;">
                                                                </div>
                                                                <asp:LinkButton ID="lbName" runat="server" CommandName="Sort" CommandArgument="Name">
                                                                    <%= Resources.Resource.Admin_ExportFeed_Grid_Name%>
                                                                    <asp:Image ID="arrowName" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                                                </asp:LinkButton>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField AccessibleHeaderText="SortOrder" HeaderStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="170px" HeaderStyle-Width="170px">
                                                            <HeaderTemplate>
                                                                <div style="width: 170px; font-size: 0px; height: 0px;">
                                                                </div>
                                                                <asp:LinkButton ID="lbSortOrder" runat="server" CommandName="Sort" CommandArgument="SortOrder">
                                                                    <%= Resources.Resource.Admin_ExportFeed_Grid_SortOrder%>
                                                                    <asp:Image ID="arrowSortOrder" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                                                </asp:LinkButton>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:Label ID="lSortOrder" runat="server" Text='<%# Bind("SortOrder") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField AccessibleHeaderText="Cheaked" HeaderStyle-HorizontalAlign="Left"
                                                            ItemStyle-Width="170px" HeaderStyle-Width="170px">
                                                            <HeaderTemplate>
                                                                <div style="width: 170px; font-size: 0px; height: 0px;">
                                                                </div>
                                                                <asp:LinkButton ID="lbCheaked" runat="server" CommandName="Sort" CommandArgument="Cheaked">
                                                                    <%= Resources.Resource.Admin_ExportFeed_Grid_Export%>
                                                                    <asp:Image ID="arrowCheaked" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                                                </asp:LinkButton>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox runat="server" Checked='<%# ((int)Eval("Cheaked"))>0 %>' />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField ItemStyle-Width="90px" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="center"
                                                            FooterStyle-HorizontalAlign="Center">
                                                            <EditItemTemplate>
                                                                <input id="btnUpdate" name="btnUpdate" class="updatebtn showtooltip" type="image"
                                                                    src="images/updatebtn.png" onclick="<%#ClientScript.GetPostBackEventReference(grid, "Update$" + Container.DataItemIndex)%>; return false;"
                                                                    style="display: none" title='<%= Resources.Resource.Admin_MasterPageAdminCatalog_Update%>' />
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
                                                        <div style="text-align: center; margin-top: 20px; margin-bottom: 20px;">
                                                            <asp:Localize ID="Localize_Admin_Catalog_NoRecords" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_NoRecords %>"></asp:Localize>
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
                                                                <asp:ListItem Selected="True">100</asp:ListItem>
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
                                </asp:Panel>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="dvSubHelp" id="YaHelp" runat="server">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
                <a href="http://www.advantshop.net/help/pages/export-yandex-market" target="_blank">Инструкция. Выгрузка товаров в Яндекс.Маркет</a>
            </div>
            <div class="dvSubHelp" id="CsvHelp" runat="server">
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
                <a href="http://www.advantshop.net/help/pages/export-part-csv" target="_blank">Инструкция. Выгрузка каталога в csv файл по категориям</a>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function setupTooltips() {
            $(".showtooltip").tooltip({
                showURL: false
            });
        }
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () { setupTooltips(); });
    </script>
</asp:Content>
