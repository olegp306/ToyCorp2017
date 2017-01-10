<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CompareCodeMasks.aspx.cs"
    Inherits="Tools.core.CompareCodeMasks" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AdvantShop.NET Core Tools - Compare Code Masks</title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Button ID="btnCreateMask" runat="server" OnClick="btnCreateMask_OnClick" Text="Create Mask" /><asp:Button
        ID="btnUpdate" runat="server" OnClick="btnUpdateClick" Text="Update Code" /><br />
    <asp:Label ID="lblError" runat="server"></asp:Label>
    <asp:Literal ID="ltrlReport" runat="server"></asp:Literal>
    </form>
</body>
</html>
