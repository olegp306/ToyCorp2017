<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LastOrders.ascx.cs" Inherits="Admin.UserControls.Dashboard.Admin_UserControls_LastOrders" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<div class="table-ui-wrap">
    <asp:ListView ID="lvLastOrders" runat="server" ItemPlaceholderID="itemPlaceholderID" OnItemCommand="lvLastOrders_ItemCommand">
        <LayoutTemplate>
            <table class="table-ui table-last-orders">
                <thead>
                    <tr>
                        <th colspan="2" class="table-ui-align-left">
                            <asp:Label runat="server" Text='<%$ Resources: Resource, Admin_Dashboard_OrderStatus %>'></asp:Label>
                        </th>
                        <th>
                            <asp:Label ID="Label2" runat="server" Text='<%$ Resources: Resource, Admin_Dashboard_OrderID %>'></asp:Label>
                        </th>
                        <th class="table-ui-align-left">
                            <asp:Label ID="Label3" runat="server" Text='<%$ Resources: Resource, Admin_Dashboard_OrderBuyer %>'></asp:Label>
                        </th>
                        <th>
                            <asp:Label ID="Label4" runat="server" Text='<%$ Resources: Resource, Admin_Dashboard_OrderDate %>'></asp:Label>
                        </th>
                        <th class="table-ui-cost">
                            <asp:Label ID="Label5" runat="server" Text='<%$ Resources: Resource, Admin_Dashboard_OrderSum %>'></asp:Label>
                        </th>
                        <th class="table-ui-row-btn">
                            <asp:Label ID="Label6" runat="server" Text='<%$ Resources: Resource, Admin_Dashboard_OrderCommands %>'></asp:Label>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr id="itemPlaceholderID" runat="server">
                    </tr>
                </tbody>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr data-redirect='<%# "vieworder.aspx?OrderID=" + Eval("OrderID") %>'>
                <td class="table-order-status" <%# "style=\"background: #"+ Eval("OrderStatus.Color") +"\"" %>>
                </td>
                <td>
                    <span>
                        <%# Eval("OrderStatus.StatusName")%>
                    </span>
                </td>
                <td class="table-ui-align-center">
                    <%# Eval("OrderID") %>
                </td>
                <td>
                    <%#Eval("OrderCustomer.FirstName") + " " + Eval("OrderCustomer.LastName")%>
                </td>
                <td class="table-ui-align-center">
                    <%# AdvantShop.Localization.Culture.ConvertDate((DateTime)Eval("OrderDate")) %>
                </td>
                <td class="table-ui-align-center">
                    <%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Sum")) < 0 ? 0 : SQLDataHelper.GetFloat(Eval("Sum")), SQLDataHelper.GetFloat(Eval("OrderCurrency.CurrencyValue")), SQLDataHelper.GetString(Eval("OrderCurrency.CurrencyCode")))%>
                </td>
                <td class="table-ui-align-center">
                    <a href='<%# "vieworder.aspx?OrderID=" + Eval("OrderID") %>'>
                        <img src="images/editbtn.gif" alt="" /></a>
                    <asp:LinkButton runat="server" ID="buttonDelete" runat="server" CommandName="DeleteOrder" CommandArgument='<%# Eval("OrderID") %>'
                        CssClass="showtooltip lnk-del-order" ToolTip='<%$ Resources:Resource, Admin_OrderSearch_Delete%>' 
                        data-message='<%# string.Format(Resources.Resource.Admin_OrderSearch_DeleteOrder, Eval("OrderID")) %>'>
                        <img src="images/deletebtn.png" alt="" />
                    </asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
</div>
