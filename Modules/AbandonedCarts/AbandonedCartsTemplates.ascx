<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AbandonedCartsTemplates.ascx.cs" Inherits="Advantshop.Modules.UserControls.AbandonedCartsTemplates" %>
<div>
    <div style="margin: 0 0 10px 0">
        <a href='javascript:open_window("../Modules/AbandonedCarts/AddEditTemplate.aspx",830,600)'>
            <asp:Localize runat="server" Text="<%$ Resources: AddNewTemplate %>"></asp:Localize>
        </a>
    </div>
    <asp:ListView ID="lvTemplates" runat="server" ItemPlaceholderID="trPlaceholderID"  OnItemCommand="lvTemplates_ItemCommand">
        <LayoutTemplate>
            <table class="table-ui">
                <thead>
                    <th><asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Name%>" /></th>
                    <th style="width: 150px;"><asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: Active%>" /></th>
                    <th style="width: 150px;"><asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: SendingTime%>" /></th>
                    <th style="width: 60px;">&nbsp;</th>
                </thead>
                <tbody>
                    <tr id="trPlaceholderID" runat="server"></tr>
                </tbody>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <%# Eval("Name") %>
                </td>
                <td>
                    <%# (bool)Eval("Active") ? "Да" : "Нет" %>
                </td>
                <td>
                    <%# Eval("SendingTime") %>
                </td>
                <td>
                    <a href='<%# "javascript:open_window(\"../Modules/AbandonedCarts/AddEditTemplate.aspx?Id=" + Eval("Id")+"\",830,600)"%>'>
                        <asp:Image runat="server" ImageUrl="~/Modules/AbandonedCarts/images/editbtn.gif" EnableViewState="false" />
                    </a> <asp:LinkButton ID="lb" runat="server" CommandArgument='<%#Eval("Id")%>' CommandName="DeleteItem">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Modules/BuyInTime/images/remove.jpg" EnableViewState="false" />
                    </asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
</div>
