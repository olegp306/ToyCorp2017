<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Multiship.ascx.cs" Inherits="Admin.UserControls.ShippingMethods.MultishipControl" %>

<table border="0" cellpadding="2" cellspacing="0" style="width: 99%; margin-left: 5px;
    margin-top: 5px">
    <tr class="rowPost">
        <td colspan="3" style="height: 34px">
            <h4 style="display: inline; font-size: 12pt">
                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethods_HeadSettings %>"></asp:Localize></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <% if (!IsActive) {%>
       <tr>
        <td colspan="3">
            Инструкция:<br><br>
            1) Зарегистрируйтесь на сайте <a href="https://multiship.ru/" target="blank">multiship.ru</a><br><br>
            2) Введите email, пароль и домен, которые Вы указали при регистрации<br><br>
            3) Нажмите кнопку "Сохранить параметры"<br><br><br>
        </td>
    </tr>
       <tr>
        <td style="width: 130px;vertical-align: middle;">
            <asp:Localize ID="Localize3" runat="server" Text="Email"></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtEmail" Width="250" ValidationGroup="5"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image1" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Email от аккаунта на сайте multiship.ru" /><asp:Label
                    runat="server" ID="Label1" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="width: 130px;vertical-align: middle;">
            <asp:Localize ID="Localize5" runat="server" Text="Пароль"></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" Width="250" ValidationGroup="5"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image3" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Пароль от аккаунта на сайте multiship.ru" /><asp:Label
                    runat="server" ID="Label2" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="width: 130px;vertical-align: middle;">
            <asp:Localize ID="Localize7" runat="server" Text="Домен (www.site.ru)"></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtDomain" Width="250" ValidationGroup="5"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image5" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Домен сайта: www.site.ru. Должен совпадать с тем, что указано в multiship.ru 'Настройки -> магазины'" /><asp:Label
                    runat="server" ID="Label3" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <% } else {%>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize8" runat="server" Text="Статус:"></asp:Localize>
        </td>
        <td class="columnVal status-msmodule">
            <asp:Label runat="server" ID="lblStatus" /> <a href="javascript:void(0)">Деактивировать</a>
            <asp:HiddenField runat="server" ID="hfIsActive"/>
            <asp:HiddenField runat="server" ID="hfMSObject"/>
        </td>
        <td class="columnDescr">
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize2" runat="server" Text="Город, из которого осуществляется доставка"></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtCityFrom" Width="250" ValidationGroup="5" Text="Москва"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image2" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Город, из которого осуществляется доставка" /><asp:Label
                    runat="server" ID="msgCityFrom" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize9" runat="server" Text="ID магазинов, созданных в системе MultiShip"></asp:Localize>
        </td>
        <td class="columnVal">
            <asp:DropDownList runat="server" ID="ddlSenders" />
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image4" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Id магазина в системе MultiShip" /><asp:Label
                    runat="server" ID="Label4" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="3">
            Если у товаров не указан размер или вес, то будут применены следущие параметры<br><br>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize6" runat="server" Text="Средний вес"></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtWeight" Width="50px" Text="5" /> кг.
        </td>
        <td class="columnDescr">
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize4" runat="server" Text="Длина, высота, ширина (см) "></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtLengthAvg" Width="50px" Text="10" /> 
            <asp:TextBox runat="server" ID="txtHeightAvg" Width="50px" Text="10" /> 
            <asp:TextBox runat="server" ID="txtWidthAvg" Width="50px" Text="10" /> см.
        </td>
        <td class="columnDescr">
        </td>
    </tr>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.status-msmodule a').on("click", function() {
                $('.status-msmodule span').text('Изменен. Нажмите кнопку сохранить');
                $('.status-msmodule input').val('False');
            });
        });
    </script>
    <% } %>

</table>
<asp:Label runat="server" ID="lError" ForeColor="Red" Visible="False" style="display:block; margin-bottom: 10px;" />
<br/>
<br/>

