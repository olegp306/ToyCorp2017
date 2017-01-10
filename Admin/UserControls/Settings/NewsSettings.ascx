<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewsSettings.ascx.cs" Inherits="Admin.UserControls.Settings.NewsSettings" %>
<table border="0" cellpadding="2" cellspacing="0" style="width:550px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                Новости
            </span>
            <br />
            <span class="subTitleNotify">
                Прочие настройки для новостей
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
</table>
<br />
<table border="0" cellpadding="2" cellspacing="0" style="width:550px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_HeadNewsOther%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width:180px;">
            <label class="form-lbl" for="<%= txtNewsPerPage.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_NewsPerPage%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtNewsPerPage" class="niceTextBox shortTextBoxClass3"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtNewsMainPageCount.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_MainPageNews%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtNewsMainPageCount" class="niceTextBox shortTextBoxClass3"></asp:TextBox>
        </td>
    </tr>
</table>
<span class="subSaveNotify">
    Чтобы сохранить изменения, нажмите оранжевую кнопку "Сохранить" вверху.
</span>