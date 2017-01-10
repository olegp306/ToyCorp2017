<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Updater.aspx.cs" Inherits="Tools.core.Updater"
    MasterPageFile="MasterPage.master" %>

<asp:Content runat="server" ID="cntHead" ContentPlaceHolderID="head">
    <title>AdvantShop.NET Core Tools - Advantshop updater</title>
    <script type="text/javascript" src="../../js/jq/jquery-1.7.1.min.js"></script>
</asp:Content>
<asp:Content runat="server" ID="cntMain" ContentPlaceHolderID="main">
    <table style="border-collapse: collapse; width: 100%; padding: 0px; margin: 0px;">
        <tr>
            <td style="border-bottom: 1px solid black; text-align: center;">
                <h1>
                    Updater</h1>
            </td>
        </tr>
        <tr>
            <td>
                <div style="width: 100%; margin: auto; text-align: center; padding-top: 5px; padding-bottom: 10px;">
                    <a href="backuper.aspx" target="_blank">Create backups</a>&nbsp;&nbsp;&nbsp;&nbsp;<a
                        href="iscustom.aspx" target="_blank">Check for changes</a>&nbsp;&nbsp;&nbsp;&nbsp;<a
                            href="updaterfromfile.aspx" target="_blank">Update from file</a>
                </div>
            </td>
        </tr>
    </table>
    <div style="width: 50%; font-size: 16px; margin: auto;">
        <table style="border-collapse: collapse; width: 100%; padding: 0px; margin: 0px;">
            <tr>
                <td>
                    <div style="margin-bottom: 30px; padding-top: 10px;">
                        <span style="font-weight: bold;">Текущая версия магазина:</span>
                        <asp:Label runat="server" ID="lblCurrentVersion"><%= AdvantShop.Configuration.SettingsGeneral.SiteVersion%></asp:Label>
                    </div>
                    <div style="">
                        <span style="font-weight: bold;">Последняя версия магазина:</span>
                        <asp:Label runat="server" ID="lblLastVersion"></asp:Label>
                        <br />
                        <div id="divMoreInf" runat="server">
                            <fieldset id="divMore" style="margin: auto; padding: 10px;">
                                <legend><span id="spanMore" style="cursor: pointer; text-decoration: underline;"
                                    onclick="showHideInformation()">Read more about the changes (Collapse)</span></legend>
                                <div runat="server" id="lblVersionInformation">
                                </div>
                            </fieldset>
                        </div>
                    </div>
                    <fieldset style="padding: 10px;">
                        <legend>
                            <h2 style="margin: 0px;">
                                Message</h2>
                        </legend>Рекомендуем сделать резервные копии исходного кода и базы данных.
                    </fieldset>
                    <div style="line-height: 18px;">
                        <br />
                        <asp:CheckBox ID="ckbUpdate" runat="server" Text="Я нахожусь в трезвом уме и твердой памяти" />
                        <br />
                        <asp:CheckBox ID="ckbUpdate1" runat="server" Text="Я сделал все необходимые резервные копии" />
                        <br />
                        <br />
                        <asp:CheckBox ID="ckbUpdateMasks" runat="server" Text="Update mask files before check" />
                        <br />
                        <br />
                        <input type="button" id="btn_update" onclick="startUpdate()" value="UPDATE" style="display: none;" />
                    </div>
                </td>
            </tr>
        </table>
        <fieldset style="padding: 10px;">
            <legend>
                <h2 style="margin: 0px;">
                    Статус обновления</h2>
            </legend>
            <div id="ltrlStatus" runat="server" style="height: 300px; overflow: scroll;">
                No report
            </div>
        </fieldset>
    </div>
    <script type="text/javascript">
        function showHideInformation() {
            if ($("#<%=lblVersionInformation.ClientID%>").is(":visible")) {
                $("#<%=lblVersionInformation.ClientID%>").hide();
                $("#spanMore").text("Read more about the changes (Expand)");

            } else {
                $("#<%=lblVersionInformation.ClientID%>").show();
                $("#spanMore").text("Read more about the changes (Collapse)");
            }
        }

        $("#<%=ckbUpdate.ClientID%>").click(function () {
            if ($("#<%= ckbUpdate1.ClientID%>").is(":checked") && $("#<%= ckbUpdate.ClientID%>").is(":checked")) {
                $("#btn_update").show();
            } else {
                $("#btn_update").hide();
            }
        });

        $("#<%=ckbUpdate1.ClientID%>").click(function () {
            if ($("#<%= ckbUpdate1.ClientID%>").is(":checked") && $("#<%= ckbUpdate.ClientID%>").is(":checked")) {
                $("#btn_update").show();
            } else {
                $("#btn_update").hide();
            }
        });

        function startUpdate() {
            if (!$("#<%=ckbUpdate.ClientID%>").is(":checked") || !$("#<%=ckbUpdate1.ClientID%>").is(":checked")) {
                return;
            }

            var intervalId = 0;

            $.ajax({
                type: "POST",
                url: "Updater.aspx/btnUpdate_Click",
                data: JSON.stringify({ 'updateMasks': $('#<%= ckbUpdateMasks.ClientID %>').is(":checked") }),
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (response) {
                    var str = JSON.parse(response).d;
                    if (str != "") {
                        $('#<%= ltrlStatus.ClientID %>').html(JSON.parse(response).d);
                    }
                },
                beforeSend: function () {
                    intervalId = setInterval(getStatus, 1000);
                },
                complete: function () {
                    clearInterval(intervalId);
                    $('#<%= ltrlStatus.ClientID %>').html("Comltete");
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //clearInterval(intervalId);
                    //alert(xhr.responseText);
                }
            });
        }

        function getStatus() {
            $.ajax({
                type: "GET",
                url: "UpdaterProgress.ashx",
                contentType: "application/json; charset=utf-8",
                dataType: "html",
                success: function (response) {
                    $('#<%= ltrlStatus.ClientID %>').html(response);
                },
                beforeSend: function () {

                },
                complete: function () {

                },
                error: function (xhr, ajaxOptions, thrownError) {
                    //alert(xhr.responseText);
                }
            });
        }

    </script>
</asp:Content>
