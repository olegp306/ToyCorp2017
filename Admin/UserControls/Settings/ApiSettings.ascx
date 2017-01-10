<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ApiSettings.ascx.cs" Inherits="Admin.UserControls.Settings.BankSettings" %>
<%@ Import Namespace="Resources" %>
<table border="0" cellpadding="2" cellspacing="0">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">API</span>
            <br />
            <span class="subTitleNotify">
                Настройка интеграции с дополнительными сервисами.
            </span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 180px;">
            <label class="form-lbl" for="<%= txtApiKey.ClientID %>"><%= Resource.Admin_UserControl_ApiSettings_ApiKey%></label>
        </td>
        <td>
            <asp:TextBox class="niceTextBox textBoxClass" Style="width:500px;" ID="txtApiKey" runat="server" />
            <div data-plugin="help" class="help-block">
                <div class="help-icon js-help-icon"></div>
                <article class="bubble help js-help">
                    <header class="help-header">
                        API ключ
                    </header>
                    <div class="help-content">
                        API ключ - Это параметр необходимый для обеспечения возможности подключения сторонних сервисов к магазину.<br />
                        <br />
                        Обратите внимание, если вы повторно сгенерируете ключ, все ссылки в которых он используется, так же необходимо обновить, включая те, что были указанны ранее в сторонних сервисах.
                    </div>
                </article>
            </div>
            <br/>
            <asp:LinkButton runat="server" ID="lbGenerateApiKey" Text="<%$ Resources: Resource, Admin_UserControl_ApiSettings_GenerateApiKey%>" 
                OnClick="lbGenerateApiKey_Click" />
        </td>
    </tr>
</table>

<table id="tb1cApi" runat="server" Visible="False" border="0" cellpadding="2" cellspacing="2">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">1C</span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resource.Admin_UserControl_ApiSettings_1C_Enabled%>
        </td>
        <td>
            <asp:CheckBox ID="chk1CEnabled" runat="server" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resource.Admin_UserControl_ApiSettings_1C_ExportOrdersType%>
        </td>
        <td>
            <asp:DropDownList ID="ddlExportOrdersType" runat="server">
                <asp:ListItem Text="<%$ Resources:Resource, Admin_UserControl_ApiSettings_1C_UseIn1CType %>" Value="0" />
                <asp:ListItem Text="<%$ Resources:Resource, Admin_UserControl_ApiSettings_1C_AllType %>" Value="1" />
            </asp:DropDownList>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resource.Admin_UserControl_ApiSettings_1C_ImportPhotos%>
        </td>
        <td>
            <asp:Label runat="server" ID="lblImportPhotosUrl" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 180px;">
            <%= Resource.Admin_UserControl_ApiSettings_1C_ImportProducts%>
        </td>
        <td>
            <asp:Label runat="server" ID="lblImportProductsUrl" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resource.Admin_UserControl_ApiSettings_1C_ExportProducts%>
        </td>
        <td>
            <asp:Label runat="server" ID="lblExportProductsUrl" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td style="width: 180px;">
            <%= Resource.Admin_UserControl_ApiSettings_1C_DeletedProducts%>
        </td>
        <td>
            <asp:Label runat="server" ID="lblDeletedProducts" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resource.Admin_UserControl_ApiSettings_1C_ExportOrders%>
        </td>
        <td>
            <asp:Label runat="server" ID="lblExportOrdersUrl" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resource.Admin_UserControl_ApiSettings_1C_DeletedOrders%>
        </td>
        <td>
            <asp:Label runat="server" ID="lblDeletedOrdersUrl" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <%= Resource.Admin_UserControl_ApiSettings_1C_ChangeOrderStatusUrl%>
        </td>
        <td>
            <asp:Label runat="server" ID="lblChangeOrderStatusUrl" />
        </td>
    </tr>
    <%--<tr class="rowPost">
        <td style="padding:10px 0px;">
            <b><%= Resource.Admin_UserControl_ApiSettings_1C_StatusIn1C%></b>
        </td>
        <td style="padding:10px 0px;">
            <b><%= Resource.Admin_UserControl_ApiSettings_1C_StatusInSite%></b>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ddlNotAssigned.ClientID %>"><%= Resource.Admin_UserControl_ApiSettings_1C_NotAssigned%></label>
        </td>
        <td>
            <asp:DropDownList runat="server" ID="ddlNotAssigned" DataTextField="StatusName" DataValueField="StatusId" />
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ddlAssigned.ClientID %>"><%= Resource.Admin_UserControl_ApiSettings_1C_Assigned%></label>
        </td>
        <td>
            <asp:DropDownList runat="server" ID="ddlAssigned" DataTextField="StatusName" DataValueField="StatusId"/>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ddlRezerv.ClientID %>"><%= Resource.Admin_UserControl_ApiSettings_1C_Rezerv%></label>
        </td>
        <td>
            <asp:DropDownList runat="server" ID="ddlRezerv" DataTextField="StatusName" DataValueField="StatusId"/>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ddlToShip.ClientID %>"><%= Resource.Admin_UserControl_ApiSettings_1C_ToShip%></label>
        </td>
        <td>
            <asp:DropDownList runat="server" ID="ddlToShip" DataTextField="StatusName" DataValueField="StatusId"/>
        </td>
    </tr>
    <tr class="rowsPost row-interactive">
        <td>
            <label class="form-lbl" for="<%= ddlClosed.ClientID %>"><%= Resource.Admin_UserControl_ApiSettings_1C_Closed%></label>
        </td>
        <td>
            <asp:DropDownList runat="server" ID="ddlClosed" DataTextField="StatusName" DataValueField="StatusId"/>
        </td>
    </tr>--%>
</table>

