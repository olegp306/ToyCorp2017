<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NotifyEmailsSettings.ascx.cs" Inherits="Admin.UserControls.Settings.NotifyEmailsSettings" %>
<table border="0" cellpadding="2" cellspacing="0" style="width:650px;">
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_NotifyEmails%>
            </span>
            <br />
            <span class="subTitleNotify">
                В данном разделе задаются email адреса, на которые будут отправлены уведомления о событиях.
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width:230px;">
            <label class="form-lbl" for="<%= txtEmailRegReport.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_EmailForReports%></label>
        </td>
        <td>
            <asp:TextBox ID="txtEmailRegReport" runat="server" class="niceTextBox textBoxClass"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtOrderEmail.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_EmailForOrders%></label>
        </td>
        <td>
            <asp:TextBox ID="txtOrderEmail" runat="server" class="niceTextBox textBoxClass"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtEmailProductDiscuss.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_EmailForComment%></label>
        </td>
        <td>
            <asp:TextBox ID="txtEmailProductDiscuss" runat="server" class="niceTextBox textBoxClass"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtFeedbackEmail.ClientID %>"><%= Resources.Resource.Admin_CommonSettings_EmailForFeedBack%></label>
        </td>
        <td>
            <asp:TextBox ID="txtFeedbackEmail" runat="server" class="niceTextBox textBoxClass"></asp:TextBox>
        </td>
    </tr>
</table>
<span class="subSaveNotify">
    Вы можете указать 2 и более получателей, через символ точки с запятой. <br />
    Например: email@email.ru; email222@email.ru<br /><br />
    Чтобы сохранить изменения, нажмите оранжевую кнопку "Сохранить" вверху.
</span>
