<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true"
    CodeFile="CustomerSearch.aspx.cs" Inherits="Admin.CustomerSearch" Title="Untitled Page" %>

<%@ Import Namespace="AdvantShop.Customers" %>
<%@ Import Namespace="Resources" %>

<%@ MasterType VirtualPath="~/Admin/MasterPageAdmin.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Head" runat="Server">
    <script type="text/javascript">
        function setupTooltips() {
            $(".showtooltip").tooltip({
                showURL: false
            });
        }
        function keyhook(ev) {
            ev = ev || window.event;
            var code = ev.keyCode;
            if (code == 27) {
                $find('calendar').hide();
                $find('calendar2').hide();
            }
        }
        document.onkeydown = keyhook;
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
                        var r = confirm("<%= Resources.Resource.Admin_CustomersSearch_Confirm%>");
                        if (r) __doPostBack('<%=lbDeleteSelected1.UniqueID%>', '');
                        break;
                    case "changeGroup":
                        document.getElementById('<%=lbChangeCustomerGroup.ClientID%>').click();
                }
            });
            initgrid();
        });

        function ChangeCustomerGroup() {
            if ($("#commandSelect option:selected").val() == "changeGroup") {
                $("#<%= ddlChangeCustomerGroup.ClientID %>").show();
            } else {
                $("#<%= ddlChangeCustomerGroup.ClientID %>").hide();
            }
        }
    </script>
</asp:Content>
<asp:Content ID="ContentCustomerSearch" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item selected"><a href="CustomerSearch.aspx">
                <%= Resource.Admin_MasterPageAdmin_Buyers%></a></li>
            <li class="neighbor-menu-item"><a href="CustomersGroups.aspx">
                <%= Resource.Admin_MasterPageAdmin_CustomersGroups%></a></li>
            <li class="neighbor-menu-item"><a href="Subscription.aspx">
                <%= Resource.Admin_MasterPageAdmin_SubscribeList%></a></li>
        </menu>
        <div class="panel-add">
            <a href="CreateCustomer.aspx" class="panel-add-lnk"><%= Resource.Admin_MasterPageAdmin_Add %> <%= Resource.Admin_MasterPageAdmin_Customer %></a>
        </div>
    </div>
    <div class="content-own">
        <div style="text-align: center;">
            <table cellpadding="0" cellspacing="0" width="100%">
                <tbody>
                    <tr>
                        <td style="width: 72px;">
                            <img src="images/customers_ico.gif" alt="" />
                        </td>
                        <td>
                            <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_CustomerSearch_Header %>"></asp:Label><br />
                            <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_CustomerSearch_SubHeader %>"></asp:Label>
                        </td>
                        <td>
                            <div class="btns-main">
                                <asp:Button CssClass="btn btn-middle btn-add" ID="btnCreateCustomer" runat="server" Text="<%$ Resources:Resource, Admin_CreateCustomer_Header %>" ValidationGroup="0" Visible="true" OnClick="btnCreateCustomer_Click" />
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div>
            <br />
            <table style="width: 100%;" class="massaction">
                <tr>
                    <td>
                        <span class="admin_catalog_commandBlock"><span style="display: inline-block; margin-right: 3px;">
                            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_Command %>"></asp:Localize>
                        </span><span style="display: inline-block">
                            <select id="commandSelect" onchange="ChangeCustomerGroup();">
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
                                <option value="changeGroup">
                                    <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources:Resource, Admin_Customers_ChangeGroup %>"></asp:Localize>
                                </option>
                            </select>
                            <asp:DropDownList ID="ddlChangeCustomerGroup" CssClass="dropdownselect" runat="server"
                                DataSourceID="sdsGroup" DataTextField="Name" DataValueField="CustomerGroupId"
                                OnDataBound="ddlCustomerGroup_DataBound" Style="display: none;">
                            </asp:DropDownList>
                            <input id="commandButton" type="button" value="GO" style="font-size: 10px; width: 38px; height: 20px;" />
                            <asp:LinkButton ID="lbDeleteSelected1" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_CustomersSearch_DeleteSelected %>"
                                OnClick="lbDeleteSelected1_Click" />
                            <asp:LinkButton ID="lbChangeCustomerGroup" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DeleteSelected %>"
                                OnClick="lbChangeCustomerGroup_Click" />
                        </span><span class="selecteditems" style="vertical-align: baseline"><span style="padding: 0px 3px 0px 3px;">|</span><span id="selectedIdsCount" class="bold">0</span>&nbsp;<span><%=Resources.Resource.Admin_Catalog_ItemsSelected%></span></span>
                        </span>
                    </td>
                    <td align="right" class="selecteditems">
                        <asp:UpdatePanel ID="upCounts" runat="server">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                            </Triggers>
                            <ContentTemplate>
                                <asp:Localize ID="Localize_Admin_Catalog_Total" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_Total %>"></asp:Localize>
                                <span class="bold">
                                    <asp:Label ID="lblFound" CssClass="foundrecords" runat="server" Text="" /></span>&nbsp;<asp:Localize
                                        ID="Localize8" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_RecordsFound %>"></asp:Localize>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                    <td style="width: 8px;"></td>
                </tr>
            </table>
            <div>
                <asp:Panel runat="server" ID="filterPanel" DefaultButton="btnFilter">
                    <table class="filter" style="border-collapse: collapse;" border="0" cellpadding="0" cellspacing="0">
                        <tr style="height: 5px;">
                            <td colspan="7"></td>
                        </tr>
                        <tr>
                            <td style="width: 70px; text-align: center;">
                                <div style="width: 70px; font-size: 0px;">
                                </div>
                                <asp:DropDownList ID="ddSelect" TabIndex="10" CssClass="dropdownselect" runat="server"
                                    Width="65px">
                                    <asp:ListItem Selected="True" Text="<%$ Resources:Resource, Admin_Catalog_Any %>" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" />
                                </asp:DropDownList>
                            </td>
                            <td style="width: 250px;">
                                <div style="height: 0px; width: 200px; font-size: 0px;">
                                </div>
                                <asp:TextBox CssClass="filtertxtbox" ID="txtSearchLastname" Width="99%" runat="server"
                                    TabIndex="12" />
                            </td>
                            <td style="width: 250px;">
                                <div style="height: 0px; width: 200px; font-size: 0px;">
                                </div>
                                <asp:TextBox CssClass="filtertxtbox" ID="txtSearchFirstName" Width="99%" runat="server"
                                    TabIndex="12" />
                            </td>
                            <td>
                                <div style="height: 0px; width: 200px; font-size: 0px;">
                                </div>
                                <asp:TextBox CssClass="filtertxtbox" ID="txtSearchEmail" Width="99%" runat="server"
                                    TabIndex="12" />
                            </td>
                            <td style="width: 100px; text-align: center;">
                                <div style="width: 100px; height: 0px; font-size: 0px;">
                                </div>
                                <asp:SqlDataSource ID="sdsGroup" runat="server" OnInit="sds_Init" SelectCommand="SELECT CustomerGroupId, (GroupName + ' - ' + CAST(GroupDiscount AS nvarchar(20)) + '%') as [Name] FROM [Customers].[CustomerGroup] ORDER BY GroupDiscount"></asp:SqlDataSource>
                                <asp:DropDownList ID="ddlCustomerGroup" CssClass="dropdownselect" runat="server"
                                    DataSourceID="sdsGroup" DataTextField="Name" DataValueField="CustomerGroupId"
                                    OnDataBound="ddlCustomerGroup_DataBound">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <div style="width: 100px; height: 0px; font-size: 0px;">
                                </div>
                                <asp:DropDownList ID="ddlCustomerRole" runat="server" Style="width: 155px;">
                                </asp:DropDownList>
                            </td>
                            <td style="width: 180px;">
                                <div style="height: 0px; width: 150px; font-size: 0px;">
                                </div>
                                <table cellpadding="1px;" cellspacing="0px;" style="margin-left: 5px;">
                                    <tr>
                                        <td style="text-align: left;">
                                            <%=Resources.Resource.Admin_Catalog_From%>:
                                        </td>
                                        <td style="width: 110px;">
                                            <div class="dp">
                                                <asp:TextBox CssClass="filtertxtbox" ID="txtDateFrom" Width="80" runat="server" TabIndex="12" />
                                                <img class="icon-calendar" src="images/Calendar_scheduleHS.png" alt="" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="text-align: left;">
                                            <%=Resources.Resource.Admin_Catalog_To%>:
                                        </td>
                                        <td style="width: 88px;">
                                            <div class="dp">
                                                <asp:TextBox CssClass="filtertxtbox" ID="txtDateTo" Width="80" runat="server" TabIndex="12" />
                                                <img class="icon-calendar" src="images/Calendar_scheduleHS.png" alt="" />
                                            </div>
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                            </td>
                            <td style="width: 69px; text-align: center;">
                                <div style="height: 0px; width: 69px; font-size: 0px;">
                                </div>
                                <center>
                                <asp:Button ID="btnFilter" runat="server" CssClass="btn" OnClientClick="javascript:FilterClick();"
                                    TabIndex="23" Text="<%$ Resources:Resource, Admin_Catalog_Filter %>" OnClick="btnFilter_Click" />
                                <asp:Button ID="btnReset" runat="server" CssClass="btn" OnClientClick="javascript:ResetFilter();"
                                    TabIndex="24" Text="<%$ Resources:Resource, Admin_Catalog_Reset %>" OnClick="btnReset_Click" />
                            </center>
                            </td>
                        </tr>
                        <tr style="height: 5px;">
                            <td colspan="7"></td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="pageNumberer" EventName="SelectedPageChanged" />
                        <asp:AsyncPostBackTrigger ControlID="lbDeleteSelected1" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnFilter" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="advCustomers" EventName="Sorting" />
                        <asp:AsyncPostBackTrigger ControlID="btnReset" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnGo" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ddRowsPerPage" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="advCustomers" EventName="RowCommand" />
                    </Triggers>
                    <ContentTemplate>
                        <adv:AdvGridView ID="advCustomers" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                            CellPadding="0" CellSpacing="0" Confirmation="<%$ Resources:Resource, Admin_CustomersSearch_Confirmation %>"
                            CssClass="tableview" Style="cursor: pointer" DataFieldForImageDescription=""
                            DataFieldForImagePath="" EditURL="" GridLines="None" OnRowCommand="agv_RowCommand"
                            OnSorting="advCustomers_Sorting" OnRowDataBound="grid_RowDataBound">
                            <Columns>
                                <asp:TemplateField AccessibleHeaderText="ID" Visible="false" HeaderStyle-Width="70px">
                                    <ItemTemplate>
                                        <asp:Label ID="Label01" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-CssClass="checkboxcolumn" ItemStyle-Width="70px" HeaderStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <div style="height: 0px; width: 70px; font-size: 0px;">
                                        </div>
                                        <asp:CheckBox ID="checkBoxCheckAll" CssClass="checkboxcheckall" runat="server" onclick="javascript:SelectVisible(this.checked);" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <%#(bool)Eval("IsSelected") ? "<input type='checkbox' class='sel' checked='checked' />" : "<input type='checkbox' class='sel' />"%>
                                        <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Eval("ID") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="Lastname" ItemStyle-HorizontalAlign="Left"
                                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="250">
                                    <HeaderTemplate>
                                        <div style="height: 0px; width: 200px; font-size: 0px;">
                                        </div>
                                        <asp:LinkButton ID="lbOrderLastname" runat="server" CommandName="Sort" CommandArgument="Lastname">
                                            <%=Resources.Resource.Admin_CustomerSearch_Surname1%>
                                            <asp:Image ID="arrowLastname" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                        </asp:LinkButton>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblLastname" runat="server" Text='<%# Eval("Lastname") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="Firstname" ItemStyle-HorizontalAlign="Left"
                                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="250">
                                    <HeaderTemplate>
                                        <div style="height: 0px; width: 200px; font-size: 0px;">
                                        </div>
                                        <asp:LinkButton ID="lbOrderFirstname" runat="server" CommandName="Sort" CommandArgument="Firstname">
                                            <%=Resources.Resource.Admin_CustomerSearch_Name1%>
                                            <asp:Image ID="arrowFirstname" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                        </asp:LinkButton>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblFirstname" runat="server" Text='<%# Eval("Firstname") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="Email" ItemStyle-HorizontalAlign="Left"
                                    HeaderStyle-HorizontalAlign="Left">
                                    <HeaderTemplate>
                                        <div style="height: 0px; width: 200px; font-size: 0px;">
                                        </div>
                                        <asp:LinkButton ID="lbEmail" runat="server" CommandName="Sort" CommandArgument="Email">
                                            <%=Resources.Resource.Admin_CustomerSearch_Email1%>
                                            <asp:Image ID="arrowEmail" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                        </asp:LinkButton>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField AccessibleHeaderText="CustomerGroup" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center"
                                    FooterStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <% = Resources.Resource.Admin_Customer_GroupName%>
                                    </HeaderTemplate>
                                    <EditItemTemplate>
                                        <asp:DropDownList ID="ddlCustomerGroup" runat="server">
                                        </asp:DropDownList>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField AccessibleHeaderText="CustomerRole" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Center"
                                    FooterStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <%--<% = Resources.Resource.Admin_CustomerRole%>--%> Role
                                    </HeaderTemplate> 
                                    <EditItemTemplate>
                                    <%# GetRoleName(Eval("CustomerRole").ToString())  %>
                                    </EditItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField AccessibleHeaderText="RegDate" ItemStyle-HorizontalAlign="Left"
                                    HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="180">
                                    <HeaderTemplate>
                                        <div style="height: 0px; width: 150px; font-size: 0px;">
                                        </div>
                                        <asp:LinkButton ID="lbRegDate" runat="server" CommandName="Sort" CommandArgument="RegistrationDateTime">
                                            <%=Resources.Resource.Admin_CustomerSearch_RegDate1%>
                                            <asp:Image ID="arrowRegistrationDateTime" CssClass="arrow" runat="server" ImageUrl="images/arrowdownh.gif" />
                                        </asp:LinkButton>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="lblRegDate" runat="server" Text='<%#AdvantShop.Localization.Culture.ConvertDate((DateTime)Eval("RegistrationDateTime"))%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="center" ItemStyle-Width="90px">
                                    <EditItemTemplate>
                                        <input id="btnUpdate" name="btnUpdate" class="updatebtn showtooltip" type="image"
                                            src="images/updatebtn.png" onclick="<%#ClientScript.GetPostBackEventReference(advCustomers, "Update$" + Container.DataItemIndex)%>; return false;"
                                            style="display: none; border: none;" title='<%= Resources.Resource.Admin_MasterPageAdminCatalog_Update%>' />
                                        <input id="btnCancel" name="btnCancel" class="cancelbtn showtooltip" type="image"
                                            src="images/cancelbtn.png" onclick="row_canceledit($(this).parent().parent()[0]); return false;"
                                            style="display: none; border: none;" title="<%=Resources.Resource.Admin_MasterPageAdminCatalog_Cancel %>" />
                                        <a href='<%#"ViewCustomer.aspx?CustomerID=" + Eval("ID")%>' class="showtooltip"
                                            title="<%= Resources.Resource.Admin_CustomersSearch_Edit %>">
                                            <img src="images/editbtn.gif" style="border: none;" /></a>
                                        <asp:LinkButton ID="buttonDelete" runat="server"
                                            CssClass="deletebtn showtooltip valid-confirm" CommandName="DeleteAll"
                                            data-confirm="<%$ Resources:Resource, Admin_CustomersSearch_Confirmation %>"
                                            ToolTip="<%$ Resources:Resource, Admin_CustomersSearch_Delete %>"
                                            CommandArgument='<%# Eval("ID") %>' Visible='<%# (Guid)Eval("ID") != CustomerContext.CustomerId %>' />
                                    </EditItemTemplate>
                                    <ItemTemplate>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="header" />
                            <RowStyle CssClass="row1 readonlyrow" />
                            <AlternatingRowStyle CssClass="row2 readonlyrow" />
                            <EmptyDataTemplate>
                                <center style="margin-top: 20px; margin-bottom: 20px;">
                                <asp:Localize ID="Localize_Admin_Catalog_NoRecords" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_NoRecords %>"></asp:Localize>
                            </center>
                            </EmptyDataTemplate>
                        </adv:AdvGridView>
                        <div style="border-top: 1px #c9c9c7 solid;">
                        </div>
                        <table class="results2">
                            <tr>
                                <td style="width: 157px; padding-left: 6px;">
                                    <asp:Localize ID="Localize_Admin_Catalog_ResultPerPage" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_ResultPerPage %>"></asp:Localize>:&nbsp;<asp:DropDownList
                                        ID="ddRowsPerPage" runat="server" OnSelectedIndexChanged="btnFilter_Click" CssClass="droplist"
                                        AutoPostBack="true">
                                        <asp:ListItem>10</asp:ListItem>
                                        <asp:ListItem>20</asp:ListItem>
                                        <asp:ListItem>50</asp:ListItem>
                                        <asp:ListItem>100</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td align="center">
                                    <adv:PageNumberer CssClass="PageNumberer" ID="pageNumberer" runat="server" DisplayedPages="7"
                                        UseHref="false" OnSelectedPageChanged="pn_SelectedPageChanged" UseHistory="false" />
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
                        <center>
                        <div style="text-align: left;">
                            <asp:Label ID="Message" runat="server" Font-Bold="True" ForeColor="Red" Visible="False"></asp:Label><br />
                        </div>
                    </center>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <input type="hidden" id="SelectedIds" name="SelectedIds" />
        </div>
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
                                <asp:Localize ID="Localize_Admin_Catalog_PleaseWait" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_PleaseWait %>"></asp:Localize>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <br />
        <script type="text/javascript">
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_pageLoaded(function () { setupTooltips(); });
            prm.add_beginRequest(function () { Darken(); });
            prm.add_endRequest(function () { Clear(); });
        </script>
    </div>
</asp:Content>
