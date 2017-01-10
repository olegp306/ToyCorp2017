<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="ExportCsv.aspx.cs" Inherits="Admin.ExportCsv" %>
<%@ Import Namespace="AdvantShop.Core.Extensions" %>
<%@ Import Namespace="AdvantShop.ExportImport" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item"><a href="Catalog.aspx">
                <%= Resource.Admin_MasterPageAdmin_CategoryAndProducts %></a> </li>
            <li class="neighbor-menu-item dropdown-menu-parent"><a href="ProductsOnMain.aspx?type=New">
                <%= Resource.Admin_MasterPageAdminCatalog_FirstPageProducts%></a>
                <div class="dropdown-menu-wrap">
                    <ul class="dropdown-menu">
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=Bestseller">
                            <%= Resources.Resource.Admin_MasterPageAdminCatalog_BestSellers %>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=New">
                            <%= Resources.Resource.Admin_MasterPageAdminCatalog_New %>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=Discount">
                            <%= Resources.Resource.Admin_MasterPageAdminCatalog_Discount %>
                        </a></li>
                    </ul>
                </div>
            </li>
            <li class="neighbor-menu-item dropdown-menu-parent"><a href="Properties.aspx">
                <%= Resources.Resource.Admin_MasterPageAdmin_Directory%></a>
                <div class="dropdown-menu-wrap">
                    <ul class="dropdown-menu">
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Properties.aspx">
                            <%= Resource.Admin_MasterPageAdmin_ProductProperties%>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Colors.aspx">
                            <%= Resource.Admin_MasterPageAdmin_ColorDictionary%>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Sizes.aspx">
                            <%= Resource.Admin_MasterPageAdmin_SizeDictionary%>
                        </a></li>
                    </ul>
                </div>
            </li>
            <li class="neighbor-menu-item selected"><a href="ExportCSV.aspx">
                <%= Resource.Admin_MasterPageAdmin_Export%></a></li>
            <li class="neighbor-menu-item"><a href="ImportCSV.aspx">
                <%= Resource.Admin_MasterPageAdmin_Import%></a></li>
            <li class="neighbor-menu-item"><a href="Brands.aspx">
                <%= Resource.Admin_MasterPageAdmin_Brands%></a></li>
            <li class="neighbor-menu-item"><a href="Reviews.aspx">
                <%= Resource.Admin_MasterPageAdmin_Reviews%></a></li>
        </menu>
        <div class="panel-add">
            <%= Resource.Admin_MasterPageAdmin_Add %>
            <a href="Product.aspx?categoryid=<%=Request["categoryid"] ?? "0"  %>" class="panel-add-lnk">
                <%= Resource.Admin_MasterPageAdmin_Product %></a>, <a href="#" onclick="open_window('m_Category.aspx?CategoryID=<%=Request["categoryid"] ?? "0" %>&mode=create', 750, 640); return false;"
                    class="panel-add-lnk">
                    <%= Resource.Admin_MasterPageAdmin_Category %></a>
        </div>
    </div>
    <div class="content-own">
        <div id="mainDiv" runat="server">
            <div>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td style="width: 72px;">
                            <img src="images/orders_ico.gif" alt="" />
                        </td>
                        <td>
                            <asp:Label ID="lblAdminHead" runat="server" CssClass="AdminHead" Text="<%$ Resources:Resource, Admin_ExportExcel_Catalog %>"></asp:Label><br />
                            <asp:Label ID="lblAdminSubHead" runat="server" CssClass="AdminSubHead" Text="<%$ Resources:Resource, Admin_ExportExcel_CatalogDownload %>"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <div id="divSomeMessage" runat="server">
                    <span><%=Resource.Admin_ImportCsv_YouCanExport%></span><br />
                    <span><%=Resource.Admin_ImportCsv_YouCanChoose%></span>
                </div>
                <div id="divAction" runat="server" style="margin-bottom:10px">
                    <div class="dvSubHelp" id="CsvHelp" runat="server">
                        <asp:Image ID="Image2" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
                        <a href="http://www.advantshop.net/help/pages/import-csv" target="_blank">Инструкция. Импорт и экспорт данных в формате CSV (Excel)</a>
                    </div>
                    <br />
                    <div style="font-weight:bold; margin-bottom:5px; font-size:14px;">
                        Параметры выгрузки
                    </div>
                    <table class="export-settings">
                        <tr>
                            <td>
                                <span><%=Resource.Admin_Csv_ChooseCategory%>:</span>
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblCategoriesCount"></asp:Label>
                                <a href="ExportFeed.aspx?ModuleId=CsvExport" style="float:right" target="_blank"><%=Resource.Admin_Csv_Choose %></a>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                  <span><%=Resource.Admin_Csv_ExportNoInCategory%>:</span>
                            </td>
                            <td>
                                <asp:CheckBox runat="server" ID="chbCsvExportNoInCategory" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span><%=Resource.Admin_ImportCsv_ChoseSeparator%>:</span>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlSeparetors" style="display: inline-block" />
                                &nbsp;
                                <asp:TextBox ID="txtCustomSeparator" runat="server" class="niceTextBox shortTextBoxClass2"
                                    MaxLength="5" Style="display:none;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span><%=Resource.Admin_Csv_ColumSeparator%>:</span>
                            </td>
                            <td>
                                <asp:TextBox ID="txtColumSeparator" runat="server" MaxLength="5" class="niceTextBox shortTextBoxClass2" Text=";"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span><%=Resource.Admin_Csv_PropertySeparator%>:</span>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPropertySeparator" runat="server" MaxLength="5" class="niceTextBox shortTextBoxClass2" Text=":"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span><%=Resource.Admin_ImportCsv_ChoseEncoding%>:</span>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlEncoding" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span><%=Resource.Admin_ExportCsv_ExportCategorySort%>:</span>
                            </td>
                            <td>
                                <asp:CheckBox runat="server" ID="ChbCategorySort" />
                            </td>
                        </tr>
                        <tr style="background-color: #FFF">
                            <td colspan="2">&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <span style="font-weight:bold; font-size:14px;"><%=Resource.Admin_ExportCsv_ChoseFields%></span>
                            </td>
                        </tr>
                        <tr style="height:60px;">
                            <td>
                                &nbsp;
                            </td>
                            <td style="padding-right:20px;">
                                <input type="button" class="btn btn-middle btn-action" style="margin-right:5px;"
                                    value="<%=  Resource.Admin_ExportCsv_Deselect %>" onclick="ChangeBtState('deselect')" />&nbsp;
                                <input type="button" class="btn btn-middle btn-add" 
                                    value="<%= Resource.Admin_ExportCsv_Select %>" onclick="ChangeBtState('select')" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="choseDiv" runat="server" class="overflow">
                </div>
                <center>
                <div id="OutDiv" runat="server" visible="false" style="margin-bottom: 5px">
                    <table>
                        <tr>
                            <td>
                                <div class="progressDiv">
                                    <div class="progressbarDiv" id="textBlock">
                                    </div>
                                    <div id="InDiv" class="progressInDiv">
                                        &nbsp;
                                    </div>
                                </div>
                                <br />
                                <br />
                                <%=Resource.Admin_ImportXLS_ProductsWithError %> : <span id="errorBlock" class=""></span>
                                <br/>
                                <%=Resource.Admin_CommonStatictic_CurrentProcess%> : <a id="lCurrentProcess"></a>
                            </td>
                        </tr>
                    </table>
                    <div id="divScript" runat="server">
                        <script type="text/javascript">
                            var _timerId = -1;
                            var _stopLinkId = "#<%= linkCancel.ClientID %>";

                            $(document).ready(function () {
                                $.fjTimer({
                                    interval: 100,
                                    repeat: true,
                                    tick: function (counter, timerId) {
                                        _timerId = timerId;
                                        jQuery.ajax({
                                            url: "HttpHandlers/CommonStatisticData.ashx",
                                            dataType: "json",
                                            cache: false,
                                            success: function (data) {
                                                var processed;
                                                if (data.Total != 0) {
                                                    processed = Math.round(data.Processed / data.Total * 100);
                                                } else {
                                                    processed = 0;
                                                }

                                                $("#textBlock").html(processed + "% (" + data.Processed + "/" + data.Total + ")");
                                                $("#InDiv").css("width", processed + "%");

                                                $("#errorBlock").html(data.Error);
                                                $("#lCurrentProcess").html(data.CurrentProcessName);
                                                $("#lCurrentProcess").attr("href", data.CurrentProcess);

                                                if (!data.IsRun) {
                                                    stopTimer();
                                                    if (data.Error != 0)
                                                        $("#hlDownloadImportLog").css("display", "inline");
                                                    $("#hlStart").css("display", "inline");
                                                    $("#lblRes").css("display", "inline");
                                                    $("#downloadFile").css("display", "inline");
                                                    if (data.Error == 0) {
                                                        $("#lblRes").html("<% =  Resource.Admin_ImportXLS_UpdoadingSuccessfullyCompleted %>");
                                                    }
                                                    else {
                                                        $("#lblRes").html("<% =  Resource.Admin_ImportXLS_UpdoadingCompletedWithErrors %>");
                                                        $("#lblRes").css("color", "red");
                                                    }
                                                    $("#<%= linkCancel.ClientID %>").css("display", "none");
                                                }
                                            }
                                        });
                                    }
                                });

                                $(_stopLinkId).click(function () {
                                    if (_timerId != -1) {
                                        stopTimer();
                                    }
                                });
                            });

                            function stopTimer() {
                                clearInterval(_timerId);
                            }
                        </script>
                    </div>
                </div>
            </center>
                <div id="divbtnAction" runat="server" style="margin-top:25px; margin-left:25px;">
                    <table>
                        <tr>
                            <td style="width:200px;">
                                <asp:Button CssClass="btn btn-middle btn-add" runat="server" ID="btnDownload" OnClick="btnDownload_Click"
                                    Text="<%$ Resources: Resource, Admin_ExportExcel_Export %>" />
                            </td>
                            <td>&nbsp;<asp:Literal ID="ltLink" runat="server"></asp:Literal>
                            </td>
                        </tr>
                    </table>
                </div>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="linkCancel" EventName="Click" />
                    </Triggers>
                    <ContentTemplate>
                        <div style="text-align: center;">
                            <asp:LinkButton ID="linkCancel" runat="server" Text="<%$ Resources:Resource, Admin_ImportXLS_CancelImport%>"
                                OnClick="linkCancel_Click"></asp:LinkButton>
                            <span id="lblRes" style="display: none; font-weight: bold; color: blue"></span>
                            <br />
                            <a id="hlDownloadImportLog" style="display: none" class='Link' href="HttpHandlers/DownloadLog.ashx">
                                <%= Resource.Admin_ImportXLS_DownloadImportLog%></a><br />
                            <a id="downloadFile" style="display: none" class='Link' href='../price_temp/<% = ExtStrFileName %> '>
                                <%= Resource.Admin_ExportExcel_DownloadFile%></a><br />
                            <a id="hlStart" style="display: none" class='Link' href="ExportCsv.aspx">
                                <%= Resource.Admin_ExportCsv_ExportAgain%></a><br />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <asp:Label ID="lError" runat="server" ForeColor="Red" Font-Bold="true" Visible="false" EnableViewState="false"></asp:Label>
            </div>
            <asp:DropDownList runat="server" ID="ddlProduct" Style="display: none" />
            <script type="text/javascript">
                function Change(obj) {
                    var itemTd = $(obj).closest("td");
                    var tr = itemTd.closest("tr");
                    var tds = tr.children("td");
                    var idx = tds.index(itemTd);
                    tr.next("tr").find("td:eq(" + idx + ") span").text($("<% = '#' + ddlProduct.ClientID  %> [value='" + $(obj).val() + "']").text());
                }
                function ChangeState(obj) {
                    window.location = 'ExportCsv.aspx?state=' + $(obj).val();
                }
                function ChangeBtState(obj) {
                    var flag = $("<% = '#' + ChbCategorySort.ClientID%>").prop('checked');
                    window.location = 'ExportCsv.aspx?state=' + obj + '&<% = CategorySort%>=' + flag;
                }
            </script>
        </div>
        <div id="notInTariff" runat="server" visible="false" class="AdminSaasNotify">
            <center>
            <h2>
                <%= Resource.Admin_DemoMode_NotAvailableFeature%>
            </h2>
        </center>
        </div>
    </div>
    <script type="text/javascript">
        var temp = '<% =  SeparatorsEnum.Custom.StrName()%>';

        function Change() {
            if ($("#<% = ddlSeparetors.ClientID %> option:selected'").val() == temp) {
                $('#<% = txtCustomSeparator.ClientID %>').show();
            } else {
                $('#<% = txtCustomSeparator.ClientID %>').hide();
            }
        }

        $(document).ready(function () {
            Change();
            $("#<% = ddlSeparetors.ClientID %>").on('change', Change);
        });
    </script>

</asp:Content>
