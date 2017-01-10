<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ChangePrice.ascx.cs" Inherits="Admin.UserControls.ChangePrice" %>
<asp:UpdatePanel ID="up" runat="server">
    <ContentTemplate>
        <asp:Label ID="lbHeader" Text="<%$ Resources: Resource, Admin_ChangePrice_Header %>"
            runat="server" Font-Bold="true"></asp:Label>
        <br />
        <asp:DropDownList ID="ddlAction" runat="server">
        </asp:DropDownList>
        <asp:Label ID="Label1" runat="server" Text="<%$  Resources: Resource, Admin_ChangePrice_By %>"></asp:Label>
        <asp:TextBox ID="txtValue" runat="server" Width="50px"></asp:TextBox>
        <asp:DropDownList ID="ddlPercent" runat="server" OnInit="ddlPercent_Init">
            <asp:ListItem Selected="True" Value="false"></asp:ListItem>
            <asp:ListItem Value="true"></asp:ListItem>
        </asp:DropDownList>
        <asp:Button ID="btnGo" runat="server" OnClick="btnGo_Click" Text="<%$  Resources: Resource, Admin_ChangePrice_GO %>" />
        <br />
        <br />
        <asp:Label runat="server" Visible="false" ID="lblMessage" Font-Bold="true" ForeColor="#0000ff"></asp:Label>
    </ContentTemplate>
</asp:UpdatePanel>
