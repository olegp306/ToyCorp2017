<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AliveTest.aspx.cs" Inherits="Tools.core.AliveTest"
    MasterPageFile="MasterPage.master" %>

<asp:Content runat="server" ID="cntHead" ContentPlaceHolderID="head">
    <title>AdvantShop.NET Core Tools - Access rights test</title>
    <style type="text/css">
        .Header1{font-family:Tahoma;font-weight:700}
        .ContentDiv{font:.75em 'Lucida Grande' ,sans-serif}
        .Label{font-family:Tahoma;font-size:16px;color:#666}
        .clsText{border:1px solid #DDD;padding:3px;font-size:14px}
        .clsText_faild{border:1px solid #E5A3A3;padding:3px;font-size:14px;background-color:#FFCFCF}
        .label-box{border-color:#DBDBDB;border-style:solid;border-width:1px;color:#666;display:none;font-size:14px;line-height:1.45em;padding:.85em 10px;text-transform:lowercase;width:735px;display:block}
        .label-box.good{background-color:#D3F9BF;border-color:#E1EFDB}
        .label-box.error{background-color:#FFCFCF;background-image:none;border-color:#E5A3A3;color:#801B1B;padding-left:10px}
        .btn{background:url(../img/bg-btn.gif) repeat-x scroll 0 0 #DDD;border-color:#DDD #DDD #CCC;border-style:solid;border-width:1px;color:#333;cursor:pointer;font:11px/14px "Lucida Grande" ,sans-serif;margin:0;overflow:visible;padding:4px 8px 5px;width:auto}
        .btn-m{background-position:0 -200px;font-size:15px;line-height:20px!important;padding:5px 15px 6px}
        .btn-m:hover,.btn-m:focus{background-position:0 -206px}
        .fieldsetData{margin-bottom:22px;padding:12px 15px 15px;font-family:Tahoma}
        .tableActualData{font-size:14px;margin-top:5px}
    </style>
</asp:Content>
<asp:Content runat="server" ID="cntMain" ContentPlaceHolderID="main">
    <fieldset style="margin-bottom: 22px; padding: 15px 15px 15px 15px;">
        <div>
            <span class="Label">User path:</span>
            <br />
            <br />
            ~/
            <asp:TextBox ID="txtUserFolderToTest" runat="server" Width="515px" CssClass="clsText"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="btnRunTestStandard" runat="server" CssClass="btn btn-m" Text="Test user folder"
                Width="200px" OnClick="btnRunTestUserFolder_Click" />
            <br />
            <br />
            <asp:Button ID="btnRunTestUserFolder" runat="server" CssClass="btn btn-m" Text="Test standard folders"
                Width="200px" OnClick="btnRunTestStandard_Click" />
        </div>
    </fieldset>
    <asp:Repeater ID="StatusRepeater" runat="server">
        <HeaderTemplate>
            <fieldset class="fieldsetData">
                <span style="font-size: 18px; font-weight: bold;">
                    <% = _strActualData %></span>
                <table class="tableActualData">
                    <tbody>
                        <tr>
                            <td>
                                Path
                            </td>
                            <td style="width: 70px;">
                                is exits
                            </td>
                            <td style="width: 80px;">
                                files count
                            </td>
                            <td style="width: 50px;">
                                create
                            </td>
                            <td style="width: 50px;">
                                read
                            </td>
                            <td style="width: 50px;">
                                modify
                            </td>
                            <td style="width: 50px;">
                                delete
                            </td>
                        </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <span style="font-size: 14px; font-weight: bold;">
                        <%# Eval("Name")%>&nbsp;&nbsp;</span>
                </td>
                <td>
                    <%# (bool)Eval("Exist") ? "<span style='color:green;'>Exist</span>" : "<span style='color:red;'>Not exist</span>"%>
                </td>
                <td>
                    <%# Eval("FilesCount") %>
                </td>
                <td>
                    <%# (bool)Eval("AllowCreate") ? "<span style='color:green'>" + Eval("AllowCreate") + "</span>" : "<span style='color:red'>" + Eval("AllowCreate") + " </span>"%>
                </td>
                <td>
                    <%# (bool)Eval("AllowRead") ? "<span style='color:green'>" + Eval("AllowRead") + "</span>" : "<span style='color:red'>" + Eval("AllowRead") + " </span>"%>
                </td>
                <td>
                    <%# (bool)Eval("AllowWrite") ? "<span style='color:green'>" + Eval("AllowWrite") + "</span>" : "<span style='color:red'>" + Eval("AllowWrite") + " </span>"%>
                </td>
                <td>
                    <%# (bool)Eval("AllowDelete") ? "<span style='color:green'>" + Eval("AllowDelete") + "</span>" : "<span style='color:red'>" + Eval("AllowDelete") + " </span>"%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody> </table> </fieldset>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>
