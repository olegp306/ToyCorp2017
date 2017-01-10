<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Kupivkredit.ascx.cs" Inherits="Admin.UserControls.PaymentMethods.KupivkreditControl" %>
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
            <asp:Localize runat="server" Text="Идентификатор партнера"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtPartnerId" Width="250" ></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Идентификатор партнера" /><asp:Label
                    runat="server" ID="msgPartnerId" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize1" runat="server" Text="Секретный ключ"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtSecretKey" Width="250" ></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Секретный ключ" /><asp:Label
                    runat="server" ID="msgSecretKey" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
         <td class="columnName">
            <asp:Localize ID="Localize2" runat="server" Text="Тестовый режим"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
         <td class="columnVal">
            <asp:CheckBox runat="server" ID="chkSandbox" />
        </td>
          <td class="columnDescr">
            <asp:Image ID="Image1" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png" ToolTip="" /><asp:Label runat="server" ID="msgSandbox" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr> 
    <tr>
         <td class="columnName">
            <asp:Localize ID="Localize3" runat="server" Text="Минимальная цена"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
         <td class="columnVal">
            <asp:TextBox runat="server" ID="txtMinimumPrice" Width="250" Text="3000" ></asp:TextBox>
        </td>
          <td class="columnDescr">
            <asp:Image runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Кнопка &quot;Купить в кредит&quot; будет отображаться у товаров превышающих минимальную цену (желательно > 3000 руб.)" /><asp:Label
                    runat="server" ID="msgMinimumPrice" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr> 
      <tr>
         <td class="columnName">
            <asp:Localize ID="Localize4" runat="server" Text="Сумма первого платежа(%)"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
         <td class="columnVal">
            <asp:TextBox runat="server" ID="txtFirstPayment" Width="250" Text="10" ></asp:TextBox>
        </td>
          <td class="columnDescr">
            <asp:Image ID="Image2" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="% от стоимости товара составит первый платеж" /><asp:Label
                    runat="server" ID="msgFirstPayment" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr> 
</table>
<div class="dvSubHelp2">
    <asp:Image ID="Image7" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
    <a href="http://www.advantshop.net/help/pages/buy-in-credit" target="_blank">Инструкция. Установка кнопки «Купи в кредит»</a>
</div>