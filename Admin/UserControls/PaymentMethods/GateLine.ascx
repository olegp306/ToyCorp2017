﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GateLine.ascx.cs" Inherits="Admin.UserControls.PaymentMethods.GateLineControl" %>

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
            <asp:Localize ID="LocalizeSite" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_GateLine_Site %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
         <td class="columnVal">
            <asp:TextBox runat="server" ID="txtSite" Width="250"></asp:TextBox>
        </td>
          <td class="columnDescr">
            <asp:Image ID="ImageSite" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png" 
                ToolTip="<%$ Resources:Resource, Admin_PaymentMethod_GateLine_Site_Description %>" /><asp:Label runat="server" ID="msgSite" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
         <td class="columnName">
            <asp:Localize ID="LocalizePassword" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_GateLine_Password %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
         <td class="columnVal">
            <asp:TextBox runat="server" ID="txtPassword" Width="250"></asp:TextBox>
        </td>
          <td class="columnDescr">
            <asp:Image ID="ImagePassword" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png" 
                ToolTip="<%$ Resources:Resource, Admin_PaymentMethod_GateLine_Password_Description %>" /><asp:Label runat="server" ID="msgPassword" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
         <td class="columnName">
            <asp:Localize ID="LocalizeTestMode" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_GateLine_TestMode %>"></asp:Localize>
        </td>
         <td class="columnVal">
            <asp:CheckBox runat="server" ID="chkTestMode" Checked="False" />
        </td>
          <td class="columnDescr">
            <asp:Image ID="ImageTestMode" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png" 
                ToolTip="<%$ Resources:Resource, Admin_PaymentMethod_GateLine_TestMode_Description %>" /><asp:Label runat="server" ID="msgTestMode" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
</table>
