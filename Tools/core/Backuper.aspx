<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Backuper.aspx.cs" Inherits="Tools.core.Backuper"
    MasterPageFile="MasterPage.master" %>

<asp:Content runat="server" ID="cntHead" ContentPlaceHolderID="head">
    <title>AdvantShop.NET Core Tools - Compare Code Masks</title>
    <script type="text/javascript" src="../../js/jq/jquery-1.7.1.min.js"></script>
</asp:Content>
<asp:Content runat="server" ID="cntMain" ContentPlaceHolderID="main">
    <div style="width: 100%; font-size: 16px;">
        <table style="border-collapse: collapse; width: 100%; padding: 0px; margin: 0px;">
            <tr>
                <td style="border-bottom: 1px solid black; text-align: center;">
                    <h1>
                        Backuper</h1>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="width: 50%; margin: auto; text-align: center; padding: 10px; margin-top: 0;">
                        <a href="updater.aspx" target="_blank">Updater</a>&nbsp;&nbsp;&nbsp;&nbsp;<a href="iscustom.aspx"
                            target="_blank">Check for changes</a>&nbsp;&nbsp;&nbsp;&nbsp;<a href="updaterfromfile.aspx"
                                target="_blank">Update from file</a>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="">
                    <fieldset style="width: 50%; margin: auto; padding: 10px; line-height: 18px; margin-top: 20px;">
                        <legend>
                            <h2>
                                Message</h2>
                        </legend>
                        <asp:Panel ID="pnlMessage" runat="server">
                            Some message
                        </asp:Panel>
                    </fieldset>
                    <div style="height: 30px;">
                        <div id="divStatus" style="padding: 10px; text-align: center; font-size: 18px;">
                            <asp:Panel ID="pnlStatusBackup" runat="server" Style="display: none;">
                                <asp:Image ID="imgWait" runat="server" ImageUrl="../img/loading.gif" />
                                Backup...
                            </asp:Panel>
                            <asp:Panel ID="pnlStatusComplete" runat="server" Style="display: none;">
                                Backup completed.
                            </asp:Panel>
                        </div>
                    </div>
                    <div id="divLinks" runat="server" style="width: 50%; margin: auto; padding: 10px;
                        line-height: 18px;">
                        <asp:LinkButton ID="lnkFileSql" runat="server" OnClick="lnkFileSql_Click"></asp:LinkButton>&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="lnkFileCode" runat="server" OnClick="lnkFileCode_Click"></asp:LinkButton>
                    </div>
                    <div style="width: 50%; margin: auto; padding: 10px; line-height: 18px;">
                        <input type="button" id="btn_BackupSql" onclick="startCreateBaseBackup()" value="Create database backup" />
                        <input type="button" id="btn_BackupCode" onclick="startCreateSourceBackup()" value="Create source backup" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        function startCreateBaseBackup() {
            $.ajax({
                type: "POST",
                url: "backuper.aspx/btnBackupSql_Click",
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (response) {
                    $('#<%= lnkFileSql.ClientID %>').html(JSON.parse(response).d);
                },
                beforeSend: function () {
                    $("#<%=pnlStatusBackup.ClientID %>").show();
                    $("#<%=pnlStatusComplete.ClientID %>").hide();
                },
                complete: function () {
                    $("#<%=pnlStatusBackup.ClientID %>").hide();
                    $("#<%=pnlStatusComplete.ClientID %>").show();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $("#<%=pnlStatusBackup.ClientID %>").hide();
                    $("#<%=pnlStatusComplete.ClientID %>").hide();
                }
            });
        }
        function startCreateSourceBackup() {
            $.ajax({
                type: "POST",
                url: "backuper.aspx/btnBackupCode_Click",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $('#<%= lnkFileCode.ClientID %>').html(JSON.parse(response).d);
                },
                beforeSend: function () {
                    $("#<%=pnlStatusBackup.ClientID %>").show();
                    $("#<%=pnlStatusComplete.ClientID %>").hide();
                },
                complete: function () {
                    $("#<%=pnlStatusBackup.ClientID %>").hide();
                    $("#<%=pnlStatusComplete.ClientID %>").show();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.responseText);
                    $("#<%=pnlStatusBackup.ClientID %>").hide();
                    $("#<%=pnlStatusComplete.ClientID %>").hide();
                }
            });
        }
    </script>
</asp:Content>
