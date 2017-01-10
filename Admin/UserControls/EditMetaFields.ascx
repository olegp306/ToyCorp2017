<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditMetaFields.ascx.cs" Inherits="Admin.UserControls.EditMetaFields" %>
<asp:HiddenField ID="hfMetaId" runat="server" />
<table border="0" cellpadding="2" cellspacing="0">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <h4 style="display: inline; font-size: 10pt;">
                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_HeadSeo%>"></asp:Localize></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
        <td>
            <span>
                <%=Resources.Resource.Admin_MetaTitle%></span>
        </td>
        <td>
            <asp:TextBox ID="txtPageTitle" runat="server" Width="400px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <span>H1</span>
        </td>
        <td>
            <asp:TextBox ID="txtH1" runat="server" Width="400px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <span>
                <%=Resources.Resource.Admin_MetaKeyWords%></span>
        </td>
        <td>
            <asp:TextBox ID="txtMetaKeywords" runat="server" Width="400px" Height="85px" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            <span>
                <%=Resources.Resource.Admin_MetaDescription%></span>
        </td>
        <td>
            <asp:TextBox ID="txtMetaDescription" runat="server" Width="400px" Height="85px" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td></td>
        <td valign="top" width="400">
            <asp:Localize ID="Localize2" Text="<%$ Resources: Resource, Admin_NewsCategory_UseGlobalVariables %>"
                runat="server"></asp:Localize>
        </td>
    </tr>
</table>
