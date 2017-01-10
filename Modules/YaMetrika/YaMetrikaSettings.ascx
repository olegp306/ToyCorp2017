<%@ Control Language="C#" AutoEventWireup="true" CodeFile="YaMetrikaSettings.ascx.cs"
    Inherits="Advantshop.Modules.YaMetrika.UserControls.Admin_YaMetrikaSettings" %>
<div style="text-align: center;">
    <table border="0" cellpadding="2" cellspacing="0">
        <tr class="rowsPost">
            <td colspan="2" style="height: 34px;">
                <span class="spanSettCategory">
                    <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: YaMetrika_Header %>" /></span>
                <asp:Label ID="lblMessage" runat="server" Visible="False" Style="float: right;"></asp:Label>
                <hr color="#C2C2C4" size="1px" />
            </td>
        </tr>
        <tr class="rowsPost">
            <td style="width: 150px; text-align: left;">
                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: YaMetrika_CounterId%>"></asp:Localize>
            </td>
            <td>
                <asp:TextBox ID="txtCounterId" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr class="rowsPost">
            <td style="width: 150px; text-align: left;">
                <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: YaMetrika_Counter%>"></asp:Localize>
            </td>
            <td>
                <asp:TextBox ID="txtCounter" runat="server" Height="200px" Width="700px" TextMode="MultiLine" />
            </td>
        </tr>
        <tr>
            <td colspan="2" style="padding: 10px 0 0 0">
                <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: YaMetrika_Events%>"></asp:Localize>
                <br />
                <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources: YaMetrika_OrderEvent%>"></asp:Localize>
                
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="<%$ Resources: YaMetrika_Save%>" />
            </td>
        </tr>
    </table>
</div>
