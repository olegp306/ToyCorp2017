<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CountersSettings.ascx.cs" Inherits="Admin.UserControls.Settings.CountersSettings" %>
<table border="0" cellpadding="2" cellspacing="0" style="width: 930px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_GoogleAnalytics%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width:300px;">
            <label class="form-lbl" for="<%= chbGoogleAnalytics.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_GoogleAnalyticsEnabled%></label>
        </td>
        <td>
            <asp:CheckBox ID="chbGoogleAnalytics" runat="server" CssClass="checkly-align" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= chkGaUseDemografic.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_GoogleAnalyticsUseDemografic%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkGaUseDemografic" CssClass="checkly-align" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtGoogleAnalytics.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_GoogleAnalyticsNumer%></label>
        </td>
        <td>
            UA-<asp:TextBox ID="txtGoogleAnalytics" runat="server" CssClass="niceTextBox shortTextBoxClass" />
            <div style="margin: 5px 0;">
                <asp:Literal ID="Literal1" runat="server" Text="<%$Resources:Resource, Admin_CommonSettings_GAEventsCategory %>" />
                <br />
                <asp:Literal ID="Literal3" runat="server" Text="<%$Resources:Resource, Admin_CommonSettings_GoogleAnalyticsEvents %>" />
            </div>
        </td>
    </tr>
</table>
<div class="dvSubHelp" style="margin-bottom:0px;">
    <asp:Image ID="Image1" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
    <a href="http://www.advantshop.net/help/pages/install-google-analytics" target="_blank">Инструкция. Установка счетчика статистики Google Analytics</a>
</div>
<div class="dvSubHelp" style="margin-top:10px;">
    <asp:Image ID="Image3" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
    <a href="http://www.advantshop.net/help/pages/google-analytics-goals" target="_blank">Инструкция. Как настроить цели в Google Analytics</a>
</div>
<table border="0" cellpadding="2" cellspacing="0" style="width: 930px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory spanSettCategorySub">
                Google Analytics Api
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width:300px;">
            <label class="form-lbl" for="<%= chbGoogleAnalyticsApi.ClientID %>">Google Analytics Api</label>
        </td>
        <td>
            <asp:CheckBox ID="chbGoogleAnalyticsApi" runat="server" CssClass="checkly-align" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtGoogleAnalyticsAccountID.ClientID %>">AccountID</label>
        </td>
        <td>
            <asp:TextBox ID="txtGoogleAnalyticsAccountID" runat="server" CssClass="niceTextBox shortTextBoxClass" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtGoogleAnalyticsUserName.ClientID %>">User Name</label>
        </td>
        <td>
            <asp:TextBox ID="txtGoogleAnalyticsUserName" runat="server" CssClass="niceTextBox shortTextBoxClass" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtGoogleAnalyticsPassword.ClientID %>">Password</label>
        </td>
        <td>
            <asp:TextBox ID="txtGoogleAnalyticsPassword" runat="server" CssClass="niceTextBox shortTextBoxClass" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtGoogleAnalyticsAPIKey.ClientID %>">API Key</label>
        </td>
        <td>
            <asp:TextBox ID="txtGoogleAnalyticsAPIKey" runat="server" CssClass="niceTextBox textBoxLong" />
        </td>
    </tr>
</table>
<div class="dvSubHelp">
    <asp:Image ID="Image2" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
    <a href="http://www.advantshop.net/help/pages/google-analytics-api" target="_blank">Инструкция. Подключение к Google Analytics API</a>
</div>
<table border="0" cellpadding="2" cellspacing="0" style="width: 930px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory spanSettCategorySub">
                <%= Resources.Resource.Admin_CommonSettings_GTM%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width:300px;">
            <label class="form-lbl" for="<%= chbUseGTM.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_UseGTM%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chbUseGTM" CssClass="checkly-align" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtGTMContainerID.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_GTM_ContainerID%></label>
        </td>
        <td>
            <asp:TextBox ID="txtGTMContainerID" runat="server" CssClass="niceTextBox shortTextBoxClass" />
            <div style="margin: 5px 0;">
                <asp:Literal ID="Literal2" runat="server" Text="<%$Resources:Resource, Admin_CommonSettings_GoogleAnalyticsEvents %>" />
            </div>
        </td>
    </tr>
</table>
<table border="0" cellpadding="2" cellspacing="0" style="width: 930px;">
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory spanSettCategorySub">
                <%= Resources.Resource.Admin_CommonSettings_ConversionTracking%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width:300px;">
            <label class="form-lbl" for="<%= txtOrderSuccessScript.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_OrderSuccessScript%></label>
        </td>
        <td>
            <asp:TextBox ID="txtOrderSuccessScript" runat="server" CssClass="niceTextBox shortTextBoxClass" TextMode="MultiLine" Width="600" Height="200px"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost">
        <td></td>
        <td>
            <span class="subSaveNotify">
                <%= Resources.Resource.Admin_CommonSettings_OrderSuccessScriptVariables%>
            </span>
            <span class="subSaveNotify">
                Чтобы сохранить изменения, нажмите оранжевую кнопку "Сохранить" вверху.
            </span>
        </td>
    </tr>
</table>
<div class="dvSubHelp">
    <asp:Image ID="Image4" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
    <a href="http://www.advantshop.net/help/pages/cv-final-page-script" target="_blank">Инструкция. Скрипт на странице успешного оформления заказа</a>
</div>