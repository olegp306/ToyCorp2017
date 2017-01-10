<%@ Page Language="C#" AutoEventWireup="true" CodeFile="cnhelper.aspx.cs" Inherits="Tools.cnhelper" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>AdvantShop.NET Tools - SQL Connection String Builder</title>
    <style type="text/css" >
        .Header1 {font-family: Tahoma; font-weight: bold;}
        .ContentDiv {font:0.75em 'Lucida Grande', sans-serif;}
        .Label {font-family: Tahoma; font-size: 16px; color: #666666;}
        .clsText {border:1px solid #DDDDDD; padding:3px; font-size:14px;}
        .clsExtend {padding:6px;}
        .label-box {border-color:#DBDBDB; border-style:solid; border-width:1px 1px 1px 1px; color:#666666; display:none; font-size:14px; line-height:1.45em; padding:0.85em 10px 0.85em 10px; text-transform:lowercase; width: 735px; display: block;}
        .label-box.good {background-color:#D3F9BF; border-color:#E1EFDB;}
        .label-box.error {background-color:#FFCFCF; background-image:none; border-color:#E5A3A3; color:#801B1B; padding-left:10px;}
        .btn {background:url("img/bg-btn.gif") repeat-x scroll 0 0 #DDDDDD; border-color:#DDDDDD #DDDDDD #CCCCCC; border-style:solid; border-width:1px; color:#333333; cursor:pointer; font:11px/14px "Lucida Grande",sans-serif; margin:0; overflow:visible; padding:4px 8px 5px; width:auto;}
        .btn-m {background-position:0 -200px; font-size:15px; line-height:20px !important; padding:5px 15px 6px;}
        .btn-m:hover, .btn-m:focus {background-position:0 -206px;}
        #tableParams tr {height: 26px;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <span style="font-family: Tahoma; font-weight: bold;">AdvantShop.NET tools</span> - <asp:HyperLink ID="HyperLink6" runat="server" 
        ForeColor="Green" NavigateUrl="Default.aspx" Text="Back to main page"></asp:HyperLink>
    <br /><br />
    <div class="ContentDiv">
        <span class="Label">Connection string:</span><br /><br />
        <table id="tableParams">
            <tr>
                <td>Data Source:</td>
                <td><asp:TextBox ID="txtDataSource" runat="server" CssClass="clsText" Width="250px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Initial Catalog:</td>
                <td><asp:TextBox ID="txtInitialCatalog" runat="server" CssClass="clsText" Width="250px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>User ID:</td>
                <td><asp:TextBox ID="txtUserID" runat="server" CssClass="clsText" Width="250px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Password:</td>
                <td><asp:TextBox ID="txtPassword" runat="server" CssClass="clsText" Width="250px"></asp:TextBox></td>
            </tr>
            <tr>
                <td>Connect Timeout:</td>
                <td><asp:TextBox ID="txtConnectTimeout" runat="server" CssClass="clsText" Width="250px"></asp:TextBox></td>
            </tr>
            <tr>
                <td><label for="chkPersistSecurityInfo">Persist Security Info:</label></td>
                <td>
                    <asp:CheckBox ID="chkPersistSecurityInfo" runat="server" Text="" /> <label for="chkPersistSecurityInfoValue">Set value:</label> <asp:CheckBox ID="chkPersistSecurityInfoValue" runat="server" Text="" />
                </td>
            </tr>
            <tr>
                <td><label for="chkIntegratedSecurity">Use Integrated Security:</label></td>
                <td>
                    <asp:CheckBox ID="chkIntegratedSecurity" runat="server" Text="" /> <label for="chkIntegratedSecurityValue">Set value:</label> <asp:CheckBox ID="chkIntegratedSecurityValue" runat="server" Text="" />
                </td>
            </tr>
        </table>
        <br />
        <asp:Button ID="btnGoExec" runat="server" Text="Build" Width="105px"  CssClass="btn btn-m" OnClick="btnGoExec_Click"/>
        <br /><br />
        <asp:TextBox ID="txtCnResultText" runat="server" CssClass="clsText" Height="144px" TextMode="MultiLine" Width="750px"></asp:TextBox><br />
        <br />
        <asp:Button ID="btnTestConnection" runat="server" Text="test cn" CssClass="btn btn-m" Width="105px"  OnClick="btnTestConnection_Click"/>
        <br /><br />
        <asp:Literal ID="Message" runat="server"></asp:Literal><br />
        <br />
    </div>
    <hr />
    </form>
</body>
</html>

