<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UniSenderSettings.ascx.cs"
    Inherits="Advantshop.UserControls.Modules.Admin_UniSenderSettings" %>
<table border="0" cellpadding="2" cellspacing="0">
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: UniSender_Header%>" /></span>
            <asp:Label ID="lblMessage" runat="server" Visible="False" Style="float: right;"></asp:Label>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: UniSender_Id %>"></asp:Localize>
            <br />
        </td>
        <td>
            &nbsp;<asp:TextBox ID="txtUniSenderId" runat="server" Width="250px"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: UniSender_FromName %>"></asp:Localize>
            <br />
        </td>
        <td>
            &nbsp;<asp:TextBox ID="txtFromName" runat="server" Width="250px"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: UniSender_FromEmail %>"></asp:Localize>
            <br />
        </td>
        <td>
            &nbsp;<asp:TextBox ID="txtFromEmail" runat="server" Width="250px"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="3" style="height: 34px;">
        </td>
    </tr>
    <tr class="rowPost">
        <td colspan="3" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Label ID="lblUniSenderRightHead" runat="server" Text="<%$ Resources: UniSender_Subscribers %>"
                    Style="float: right;"></asp:Label>
                <div class="clear">
                </div>
            </span>
            <hr style="color: #C2C2C4; height: 1px;" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: UniSender_ListSubscribers %>"></asp:Localize>
            <br />
        </td>
        <td>
            &nbsp;<asp:DropDownList ID="ddlUniSenderListsReg" runat="server" DataValueField="Id"
                DataTextField="Title" Width="250px">
            </asp:DropDownList>
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize runat="server" Text="<%$ Resources: UniSender_ListOrderCustomers %>"></asp:Localize>
            <br />
        </td>
        <td>
            &nbsp;<asp:DropDownList ID="ddlUniSenderListsOrderCustomers" runat="server" DataValueField="Id"
                DataTextField="Title" Width="250px">
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="<%$ Resources: UniSender_Save%>" />
        </td>
    </tr>
</table>
