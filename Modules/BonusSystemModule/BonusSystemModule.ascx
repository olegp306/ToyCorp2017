<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BonusSystemModule.ascx.cs" Inherits="Advantshop.Modules.BonusSystem.Admin_BonusSystemModule" %>
<div style="text-align: center;">
    <table border="0" cellpadding="2" cellspacing="0">
        <tr class="rowsPost">
            <td colspan="2" style="height: 34px;">
                <span class="spanSettCategory">
                    <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: BonusSystem_Header%>" /></span>
                <hr color="#C2C2C4" size="1px" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblMessage" runat="server" Visible="False" Style="float: right;"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" AssociatedControlID="txtApiKey" Text="ApiKey" /></td>
            <td style="height: 24px;">
                <asp:TextBox runat="server" ID="txtApiKey" Width="500px" />
                <div id="divTrial" runat="server">
                    <asp:Label runat="server" ID="txtApiKeyDemo" Text="**************************************************" />
                    <br />
                    <a href="javascript:void(0);" onclick="$('.newKey').show()">Изменить ключ</a>
                    <br />
                    <asp:TextBox runat="server" ID="txtNewKey" CssClass="newKey" Style="display: none; margin: 10px 0" Width="500px" />
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Localize2" AssociatedControlID="ddlBonusType" runat="server" Text="<%$ Resources: BonusSystem_ChargeBonuses%>" />
            </td>
            <td>
                <asp:DropDownList ID="ddlBonusType" runat="server">
                    <asp:ListItem Text="<%$ Resources: BonusSystem_OrderCost %>" Value="0" />
                    <asp:ListItem Text="<%$ Resources: BonusSystem_ProductsCost %>" Value="1" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="width: 200px">
                <asp:Label ID="Label1" AssociatedControlID="txtMaxOrderPercent" runat="server" Text="<%$ Resources: BonusSystem_MaxOrderPercent%>" />
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtMaxOrderPercent" />
                %
            </td>
        </tr>
        <tr>
            <td style="width: 200px">
                <asp:Label ID="Localize3" AssociatedControlID="lblBonusFirstPercent" runat="server" Text="<%$ Resources: BonusSystem_DefaultPercent%>" />
            </td>
            <td>
                <asp:Label runat="server" ID="lblBonusFirstPercent" />
                %
            </td>
        </tr>
        <tr>
            <td style="width: 200px">
                <asp:Label ID="Label2" AssociatedControlID="txtBonusesForNewCard" runat="server" Text="<%$ Resources: BonusSystem_BonusesForNewCard%>" />
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtBonusesForNewCard" Text="0" />
                <div style="padding: 5px 0">
                    <asp:Localize runat="server" Text="<%$ Resources: BonusSystem_BonusesForNewCard_Hint%>" />
                </div>
            </td>
        </tr>

        <tr>
            <td style="width: 200px">
                <asp:Label ID="Label3" AssociatedControlID="ckbUseOrderId" runat="server" Text="<%$ Resources: BonusSystem_UseOrderId%>" />
            </td>
            <td>
                <asp:CheckBox ID="ckbUseOrderId" runat="server" />
            </td>
        </tr>

        <tr>
            <td colspan="2">
                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="<%$ Resources: BonusSystem_Save%>" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label runat="server" ID="lblErr" Visible="False"></asp:Label>
            </td>
        </tr>
    </table>
</div>
