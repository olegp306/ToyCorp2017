<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FreeShipping.ascx.cs"
    Inherits="Admin.UserControls.ShippingMethods.FreeShippingControl" %>
<table border="0" cellpadding="2" cellspacing="0" style="width: 99%; margin-left: 5px;
    margin-top: 5px;">
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_ShippingTerm %>"></asp:Localize><span
                class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtDeliveryTime" Width="250" ValidationGroup="5"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image1" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="<%$ Resources:Resource, Admin_ShippingMethod_ShippingTerm %>" /><asp:Label runat="server" ID="Label1" Visible="false"
                    ForeColor="Red"></asp:Label>
        </td>
    </tr>
</table>
