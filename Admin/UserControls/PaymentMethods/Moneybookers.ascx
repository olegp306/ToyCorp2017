﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Moneybookers.ascx.cs"
    Inherits="Admin.UserControls.PaymentMethods.MoneybookersControl" %>
<table border="0" cellpadding="2" cellspacing="0" style="width: 99%; margin-left: 5px;
    margin-top: 5px;">
    <tr class="rowPost">
        <td colspan="3" style="height: 34px;">
            <h4 style="display: inline; font-size: 12pt;">
                <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethods_HeadSettings %>"></asp:Localize></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
         <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Moneybookers_PayToEmai %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
         <td class="columnVal">
            <asp:TextBox runat="server" ID="txtPayToEmai" Width="250"></asp:TextBox>
        </td>
          <td class="columnDescr">
            <asp:Image runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png" ToolTip="<%$ Resources:Resource, Admin_PaymentMethod_Moneybookers_PayToEmai_Description %>" /><asp:Label runat="server" ID="msgPayToEmai" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
         <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_CurrencyCode %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
         <td class="columnVal">
            <asp:TextBox runat="server" ID="txtCurrencyCode" Width="250"></asp:TextBox>
        </td>
          <td class="columnDescr">
            <asp:Image runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png" ToolTip="<%$ Resources:Resource, Admin_PaymentMethod_Moneybookers_CurrencyCode_Description %>" /><asp:Label runat="server" ID="msgCurrencyCode" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
         <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_CurrencyValue %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
         <td class="columnVal">
            <asp:TextBox runat="server" ID="txtCurrencyValue" Width="250"></asp:TextBox>
        </td>
          <td class="columnDescr">
            <asp:Image runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png" ToolTip="<%$ Resources:Resource, Admin_PaymentMethod_CurrencyValue_Description %>" /><asp:Label runat="server" ID="msgCurrencyValue" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
         <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Moneybookers_SecretWord %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
         <td class="columnVal">
            <asp:TextBox runat="server" ID="txtSecretWord" Width="250"></asp:TextBox>
        </td>
          <td class="columnDescr">
            <asp:Image runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png" ToolTip="<%$ Resources:Resource, Admin_PaymentMethod_Moneybookers_SecretWord_Description %>" /><asp:Label runat="server" ID="msgSecretWord" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
         <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Sandbox %>"></asp:Localize>
        </td>
         <td class="columnVal">
            <asp:CheckBox runat="server" ID="chkSandbox"></asp:CheckBox>
        </td>
          <td class="columnDescr">
            <asp:Image runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png" ToolTip="<%$ Resources:Resource, Admin_PaymentMethod_Sandbox_Description %>" /><asp:Label runat="server" ID="msgSandbox" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
</table>
