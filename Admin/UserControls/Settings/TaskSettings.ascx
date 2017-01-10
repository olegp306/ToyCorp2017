<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TaskSettings.ascx.cs" Inherits="Admin.UserControls.Settings.TaskSettings" %>
<%@ Import Namespace="AdvantShop.Core.Scheduler" %>
<script type="text/javascript">
    var type = "<%=TimeIntervalType.Days.ToString() %>";

    function ChangeDL(obj, tr) {
        if ($(obj).val() == type) {
            $('#' + tr).show();
        }
        else {
            $('#' + tr).hide();
        }
    }

    $(function () {
        ChangeDL('#<% = ddlTypeHtml.ClientID  %>', 'trHtml');
        ChangeDL('#<% = ddlTypeXml.ClientID  %>', 'trXml');
        ChangeDL('#<% = ddlTypeYandex.ClientID  %>', 'trYandex');
    });
</script>
<table border="0" cellpadding="2" cellspacing="0" style="width: 450px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_Scheduling%>
            </span>
            <br />
            <span class="subTitleNotify">
                Здесь вы можете задать правила работы задач, которые исполняются по расписанию в указанное время.
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <div class="dvWarningNotify">
                <%= Resources.Resource.Admin_TaskSettings_Warning%>
            </div>
        </td>
    </tr>
</table>
<table border="0" cellpadding="2" cellspacing="0" style="width: 450px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_SiteMapHtmlUpdating%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 155px;">
            <label class="form-lbl" for="<%= chbEnabledHtml.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_Enabled%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" Checked="True" ID="chbEnabledHtml" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtTimeIntervalHtml.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_StartInterval%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtTimeIntervalHtml" CssClass="niceTextBox shortTextBoxClass3">1</asp:TextBox>&nbsp;
            <asp:DropDownList runat="server" ID="ddlTypeHtml" onchange="javascript:ChangeDL(this,'trHtml')"></asp:DropDownList>
        </td>
    </tr>
    <tr id="trHtml" class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtHoursHtml.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_StartTime%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtHoursHtml" CssClass="niceTextBox shortTextBoxClass3" Text="0"/><span class="paramUnit"><%= Resources.Resource.Admin_CommonSettings_Hours%></span>&nbsp;
            <asp:TextBox runat="server" ID="txtMinutesHtml" CssClass="niceTextBox shortTextBoxClass3" Text="0"/><span class="paramUnit"><%= Resources.Resource.Admin_CommonSettings_Minutes%></span>
        </td>
    </tr>
</table>
<br />
<table border="0" cellpadding="2" cellspacing="0" style="width: 450px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_SiteMapXMLUpdating%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 155px;">
            <label class="form-lbl" for="<%= chbEnabledXml.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_Enabled%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" Checked="True" ID="chbEnabledXml" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtTimeIntervalXml.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_StartInterval%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtTimeIntervalXml" CssClass="niceTextBox shortTextBoxClass3" Text="1"/>&nbsp;
            <asp:DropDownList runat="server" ID="ddlTypeXml" onchange="javascript:ChangeDL(this,'trXml')"></asp:DropDownList>
        </td>
    </tr>
    <tr id="trXml" class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtHoursXml.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_StartTime%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtHoursXml" CssClass="niceTextBox shortTextBoxClass3" Text="0" /><span class="paramUnit"><%= Resources.Resource. Admin_CommonSettings_Hours%></span>&nbsp;
            <asp:TextBox runat="server" ID="txtMinutesXml" CssClass="niceTextBox shortTextBoxClass3" Text="0"/><span class="paramUnit"><%= Resources.Resource. Admin_CommonSettings_Minutes%></span>
        </td>
    </tr>
</table>
<br />
<table border="0" cellpadding="2" cellspacing="0" style="width: 450px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <%= Resources.Resource.Admin_CommonSettings_YMLyandexFileUpdating%>
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 155px;">
            <label class="form-lbl" for="<%= chbEnabledYandex.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_Enabled%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" Checked="True" ID="chbEnabledYandex" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtTimeIntervalYandex.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_StartInterval%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtTimeIntervalYandex" CssClass="niceTextBox shortTextBoxClass3" Text="1"/>&nbsp;
            <asp:DropDownList runat="server" ID="ddlTypeYandex" onchange="javascript:ChangeDL(this,'trYandex')"></asp:DropDownList>
        </td>
    </tr>
    <tr id="trYandex" class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtHoursYandex.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_StartTime%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtHoursYandex" CssClass="niceTextBox shortTextBoxClass3" Text="0" /><span class="paramUnit"><%= Resources.Resource.Admin_CommonSettings_Hours%></span>&nbsp;
            <asp:TextBox runat="server" ID="txtMinutesYandex" CssClass="niceTextBox shortTextBoxClass3" Text="0"/><span class="paramUnit"><%= Resources.Resource.Admin_CommonSettings_Minutes%></span>
        </td>
    </tr>
</table>
<br/>
<table border="0" cellpadding="2" cellspacing="0" style="width: 450px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                GoogleBase
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 155px;">
            <label class="form-lbl" for="<%= chbEnabledGoogleBase.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_Enabled%></label>
        </td>
        <td>
            <asp:CheckBox runat="server" Checked="True" ID="chbEnabledGoogleBase" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtTimeIntervalGoogleBase.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_StartInterval%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtTimeIntervalGoogleBase" CssClass="niceTextBox shortTextBoxClass3" Text="1"/>&nbsp;
            <asp:DropDownList runat="server" ID="ddlTypeGoogleBase" onchange="javascript:ChangeDL(this,'trGoogleBase')"></asp:DropDownList>
        </td>
    </tr>
    <tr id="trGoogleBase" class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= txtHoursGoogleBase.ClientID %>" class="Label"><%= Resources.Resource.Admin_CommonSettings_StartTime%></label>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtHoursGoogleBase" CssClass="niceTextBox shortTextBoxClass3" Text="0" /><span class="paramUnit"><%= Resources.Resource.Admin_CommonSettings_Hours%></span>&nbsp;
            <asp:TextBox runat="server" ID="txtMinutesGoogleBase" CssClass="niceTextBox shortTextBoxClass3" Text="0"/><span class="paramUnit"><%= Resources.Resource.Admin_CommonSettings_Minutes%></span>
        </td>
    </tr>
</table>
<div class="dvSubHelp" style="margin-bottom:0px;">
    <asp:Image ID="Image1" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
    <a href="http://www.advantshop.net/help/pages/background-tasks" target="_blank">Инструкция. Запуск задач по расписанию</a>
</div>