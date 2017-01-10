<%@ Control Language="C#" AutoEventWireup="true" CodeFile="YandexKassa.ascx.cs" Inherits="Admin.UserControls.PaymentMethods.YandexKassaControl" %>
<table border="0" cellpadding="2" cellspacing="0" style="margin-left: 5px; margin-top: 5px;">
    <tr class="rowPost">
        <td colspan="3" style="height: 34px;">
            <h4 style="display: inline; font-size: 12pt;">
                <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethods_HeadSettings %>"></asp:Localize></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Yandex_ShopID %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtShopID" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Label runat="server" ID="msgShopID" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Yandex_ScID %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtScID" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Label runat="server" ID="msgScID" Visible="false" ForeColor="Red"></asp:Label>
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
            <asp:Label runat="server" ID="msgCurrencyValue" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize2" runat="server" Text="Способ оплаты"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:DropDownList runat="server" ID="ddlPaymentType">
                <asp:ListItem Text="Со счета в Яндекс.Деньгах" Value="PC" />
                <asp:ListItem Text="С банковской карты" Value="AC" />
                <asp:ListItem Text="Со счета мобильного телефона" Value="MC" />
                <asp:ListItem Text="По коду через терминал" Value="GP" />
                <asp:ListItem Text="Оплата через Сбербанк: оплата по SMS или Сбербанк Онлайн" Value="SB" />
				<asp:ListItem Text="Оплата из кошелька в системе WebMoney" Value="WM" />
				<asp:ListItem Text="Оплата через мобильный терминал (mPOS)" Value="MP" />
				<asp:ListItem Text="Оплата через Альфа-Клик" Value="AB" />
				<asp:ListItem Text="Оплата через MasterPass" Value="МА" />
				<asp:ListItem Text="Оплата через Промсвязьбанк" Value="PB" />
            </asp:DropDownList>
        </td>
        <td class="columnDescr">
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_YandexKassa_Password %>"></asp:Localize><span class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtPassword" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Label runat="server" ID="msgPassword" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Yandex_DemoMode %>"></asp:Localize>
        </td>
        <td class="columnVal">
            <asp:CheckBox runat="server" ID="cbDemoMode"/>
        </td>
        <td class="columnDescr">
            <asp:Label runat="server" ID="msgCertificate" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
</table>
<div class="dvSubHelp2">
    <asp:Image ID="Image1" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
    <a href="http://www.advantshop.net/help/pages/connect-yandex-kassa" target="_blank">Инструкция. Подключение платежного модуля "Касса от Яндекс.Денег"</a>
</div>