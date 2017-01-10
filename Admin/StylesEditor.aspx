<%@ Page AutoEventWireup="true" CodeFile="StylesEditor.aspx.cs" Inherits="Admin.StylesEditor" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item"><a href="DesignConstructor.aspx">
                <%= Resource.Admin_MasterPageAdmin_DesignConstructor%></a></li>
            <li class="neighbor-menu-item"><a href="TemplateSettings.aspx">
                <%= Resource.Admin_MasterPageAdmin_TemplateSettings%></a></li>
             <li class="neighbor-menu-item selected"><a href="StylesEditor.aspx">
                <%= Resource.Admin_MasterPageAdmin_StylesEditor%></a></li>
        </menu>
    </div>
    <div class="content-own">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tbody>
                <tr>
                    <td style="width: 72px;">
                        <img src="images/orders_ico.gif" alt="" />
                    </td>
                    <td>
                        <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_MasterPageAdmin_StylesEditor %>"></asp:Label><br />
                        <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_TemplateSettings_SubHeader %>"></asp:Label>
                    </td>
                </tr>
            </tbody>
        </table>
        <div class="dvWarningNotify" style="margin-top: 10px; margin-bottom: 20px;">
            Внимание! Используйте дополнительные стили с осторожностью.<br /> Используйте файл, только если вы хорошо владеете навыками работы с CSS.
        </div>
        <div>
            <asp:Label ID="lblInfo" runat="server" ForeColor="Blue" CssClass="tpl-settings-result" />
        </div>
        <div style="width:800px; margin-top:15px;">
            <asp:TextBox Style="width:800px; height:600px; border: 1px solid #cbcbcb; border-radius: 5px; padding: 5px 5px 5px 10px;" runat="server" ID="txtStyle" TextMode="MultiLine" />
        </div>
        <div style="margin-top:20px; margin-bottom:20px;">
            <asp:Button CssClass="btn btn-middle btn-add" runat="server" ID="btnSave" OnClick="btnSaveClick"
                Text="<%$ Resources: Resource, Admin_StylesEditor_Save %>" />
        </div>
    </div>
</asp:Content>
