<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Cdek.ascx.cs" Inherits="Admin.UserControls.ShippingMethods.CdekControl" %>
<table border="0" cellpadding="2" cellspacing="0" style="width: 99%; margin-left: 5px; margin-top: 5px">
    <tr class="rowPost">
        <td colspan="3" style="height: 34px">
            <h4 style="display: inline; font-size: 12pt">
                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethods_HeadSettings %>"></asp:Localize></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize21" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_Cdek_AuthLogin %>"></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtAuthLogin" Width="250" ValidationGroup="5"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image1" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="<%$ Resources:Resource, Admin_ShippingMethod_Cdek_AuthLogin_Description %>" /><asp:Label
                    runat="server" ID="msgAuthLogin" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_Cdek_AuthPassword %>"></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtAuthPassword" Width="250" ValidationGroup="5"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image2" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="<%$ Resources:Resource, Admin_ShippingMethod_Cdek_AuthPassword_Description %>" /><asp:Label
                    runat="server" ID="msgAuthPassword" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize12" runat="server" Text="Город склада"></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtCityFrom" Width="250" ValidationGroup="5"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image5" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Город склада продавца" /><asp:Label
                    runat="server" ID="msgCityFrom" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize11" runat="server" Text="Наценка на доставку"></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtAdditionalPrice" Width="250" ValidationGroup="5"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image4" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Дополнительная наценка на доставку, например, за дополнительные услуги" /><asp:Label
                    runat="server" ID="msgAdditionalPrice" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_Edost_CreateCash %>"></asp:Localize>
        </td>
        <td class="columnVal">
            <asp:CheckBox ID="chbcreateCOD" runat="server" ValidationGroup="5" /><asp:HiddenField
                ID="hfCod" runat="server" />
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image6" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="<%$ Resources:Resource, Admin_ShippingMethod_Edost_CreateCash %>" /><asp:Label
                    runat="server" ID="Label5" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize3" runat="server" Text="Активные тарифы"></asp:Localize><span
                class="required">*</span>
        </td>
        <td class="columnVal">
            <asp:ListView ID="lvTariffs" runat="server" ItemPlaceholderID="itemPlaceholderID">
                <LayoutTemplate>
                    <table>
                        <tr runat="server" id="itemPlaceholderID">
                        </tr>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:CheckBox ID="ckbIsActive" runat="server" data-tariffid='<%#Eval("tariffId") %>'
                                Checked='<%# Eval("active") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblName" runat="server" Text='<%#Eval("name") %>'></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </td>
        <td class="columnDescr">
            <asp:Image ID="Image3" runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="Активные тарифы доставки" /><asp:Label runat="server" ID="msglvTariffs"
                    Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
</table>
<table border="0" cellpadding="2" cellspacing="0" style="width: 99%; margin-left: 5px; margin-top: 5px">
    <tr class="rowPost">
        <td colspan="3" style="height: 34px">
            <h4 style="display: inline; font-size: 12pt">
                <asp:Localize ID="Localize9" runat="server" Text="Форма вызова курьера"></asp:Localize></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize4" runat="server" Text="Дата и время вызова курьера"></asp:Localize><span
                class="required">*</span>
        </td>
        <td>
            <div style="height: 34px;">
                <div class="dp" style="width: 260px;">
                    <asp:Label ID="Label4" runat="server" Text="День" Width="90px"></asp:Label><asp:TextBox
                        ID="txtDate" Width="140px" runat="server" TabIndex="12" />
                    <%--   <img class="icon-calendar" src="./images/Calendar_scheduleHS.png" alt="" />--%>
                </div>
            </div>
            <div style="height: 34px;">
                <asp:Label ID="Label3" runat="server" Text="Время" Width="90px"></asp:Label>
                <asp:TextBox ID="txtTimeFrom" runat="server" Width="62px"></asp:TextBox>
                <ajaxToolkit:MaskedEditExtender runat="server" ID="MaskedEditExtender1" TargetControlID="txtTimeFrom"
                    MaskType="Time" Mask="99:99" />
                -
                <asp:TextBox ID="txtTimeTo" runat="server" Width="62px"></asp:TextBox>
                <ajaxToolkit:MaskedEditExtender runat="server" ID="MaskedEditExtender2" TargetControlID="txtTimeTo"
                    MaskType="Time" Mask="99:99" />
            </div>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize5" runat="server" Text="Город интернет магазина"></asp:Localize><span
                class="required">*</span>
        </td>
        <td>
            <asp:TextBox ID="txtSenderCity" runat="server" Width="230px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize6" runat="server" Text="Адрес интернет магазина"></asp:Localize><span
                class="required">*</span>
        </td>
        <td>
            <div style="height: 34px;">
                <asp:Label ID="lblSenderStreet" runat="server" Width="90px" Text="Улица"></asp:Label><asp:TextBox
                    ID="txtSenderStreet" runat="server" Width="140px"></asp:TextBox>
            </div>
            <div style="height: 34px;">
                <asp:Label ID="Label1" runat="server" Width="90px" Text="Дом"></asp:Label><asp:TextBox
                    ID="txtSenderHouse" runat="server" Width="140px"></asp:TextBox>
            </div>
            <div style="height: 34px;">
                <asp:Label ID="Label2" runat="server" Width="90px" Text="Квартира/офис"></asp:Label><asp:TextBox
                    ID="txtSenderFlat" runat="server" Width="140px"></asp:TextBox>
            </div>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize10" runat="server" Text="Имя контактного лица"></asp:Localize><span
                class="required">*</span>
        </td>
        <td>
            <asp:TextBox ID="txtSenderName" runat="server" Width="230px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize7" runat="server" Text="Телефон контактного лица"></asp:Localize><span
                class="required">*</span>
        </td>
        <td>
            <asp:TextBox ID="txtSenderPhone" runat="server" Width="230px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize8" runat="server" Text="Общий вес, в граммах"></asp:Localize><span
                class="required">*</span>
        </td>
        <td>
            <asp:TextBox ID="txtSenderWeght" runat="server" Width="230px"></asp:TextBox>
        </td>
    </tr>
</table>
<asp:Button ID="btnCallCourier" runat="server" OnClick="btnCallCourier_OnClick" Text="Вызвать курьера" CausesValidation="False" />
<asp:Label ID="lblCallCourier" runat="server"></asp:Label>
<br />
<br />
<div class="dvSubHelp2">
    <asp:Image ID="Image7" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
    <a href="http://www.advantshop.net/help/pages/install-sdek" target="_blank">Инструкция. Подключение модуля доставки СДЭК</a>
</div>
