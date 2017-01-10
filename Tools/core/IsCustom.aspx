<%@ Page Language="C#" AutoEventWireup="true" CodeFile="IsCustom.aspx.cs" Inherits="Tools.core.IsCustom"
    MasterPageFile="MasterPage.master" %>

<asp:Content runat="server" ID="cntHead" ContentPlaceHolderID="head">
    <title>AdvantShop.NET Core Tools - Is custom project</title>
    <script type="text/javascript" src="../../js/jq/jquery-1.7.1.min.js"></script>
</asp:Content>
<asp:Content runat="server" ID="cntMain" ContentPlaceHolderID="main">
    <table style="border-collapse: collapse; width: 100%; padding: 0px; margin: 0px;">
        <tr>
            <td style="border-bottom: 1px solid black; text-align: center;">
                <h1>
                    Check for changes</h1>
            </td>
        </tr>
        <tr>
            <td>
                <div style="width: 50%; margin: auto; text-align: center; padding-top: 5px; padding-bottom: 10px;">
                    <a href="backuper.aspx" target="_blank">Create backups</a>&nbsp;&nbsp;&nbsp;&nbsp;<a
                        href="updater.aspx" target="_blank">Updater</a>&nbsp;&nbsp;&nbsp;&nbsp;<a href="updaterfromfile.aspx"
                            target="_blank">Update from file</a>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div style="width: 50%; margin: auto; padding: 10px;">
                    <asp:CheckBox ID="ckbUpdateMasks" runat="server" Text="Update mask files before check"
                        Checked="True" /><br />
                    <br />
                    <input type="button" id="btnCompareCode" value="Check source" onclick="startProcessCodeMask()" />&nbsp;&nbsp;&nbsp;<input
                        type="button" id="btnCompareBase" value="Check base" onclick="startProcessBaseMask()" /><br />
                    <asp:Label ID="lblError" runat="server"></asp:Label>
                </div>
                <div style="height: 30px;">
                    <div id="divStatus" style="padding: 10px; text-align: center; font-size: 18px;">
                        <asp:Panel ID="pnlStatusBackup" runat="server" Style="display: none;">
                            <asp:Image ID="imgWait" runat="server" ImageUrl="../img/loading.gif" />
                            Check...
                        </asp:Panel>
                        <asp:Panel ID="pnlStatusComplete" runat="server" Style="display: none;">
                            Check completed.
                        </asp:Panel>
                    </div>
                </div>
                <fieldset style="width: 50%; margin: auto; padding: 10px;">
                    <legend>
                        <h2>
                            Report</h2>
                    </legend>
                    <div id="ltrlReport" runat="server" style="height: 500px; overflow: scroll;">
                        No reports</div>
                </fieldset>
                <div id="divLinks" runat="server" style="width: 50%; margin: auto; padding: 10px;
                    line-height: 18px;">
                    <asp:LinkButton ID="lnkFileSql" runat="server" OnClick="lnkFileSql_Click"></asp:LinkButton>&nbsp;&nbsp;&nbsp;
                    <asp:LinkButton ID="lnkFileCode" runat="server" OnClick="lnkFileCode_Click"></asp:LinkButton>
                </div>
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        function startProcessCodeMask() {
            $.ajax({
                type: "POST",
                url: "IsCustom.aspx/btnCompareCode_OnClick",
                data: JSON.stringify({ 'updateMasks': $('#<%= ckbUpdateMasks.ClientID %>').is(":checked") }),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (response) {
                    $('#<%= ltrlReport.ClientID %>').html(JSON.parse(response).d);
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
        function startProcessBaseMask() {
            $.ajax({
                type: "POST",
                url: "IsCustom.aspx/btnCompareBase_OnClick",
                data: JSON.stringify({ 'updateMasks': $('#<%= ckbUpdateMasks.ClientID %>').is(":checked") }),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (response) {
                    $('#<%= ltrlReport.ClientID %>').html(JSON.parse(response).d);
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
