<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LicCheck.aspx.cs" Inherits="ClientPages.LicCheck" EnableViewState="false"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" Text="Введите ключ"></asp:Label>
        <asp:TextBox runat="server" ID="txtKey" Width="400"></asp:TextBox>
        <asp:Button runat="server" ID="btnCheck" Text="Check license key" OnClick="btnCheck_Click" />
        <asp:HyperLink runat="server" ID="hlGo" NavigateUrl="Default.aspx">На главную</asp:HyperLink>
        <asp:Label runat="server" ID="lblMsg"></asp:Label>
    </div>
    </form>
</body>
</html>
