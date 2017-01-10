<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeFile="Check_SberBank.aspx.cs"
    Inherits="ClientPages.Check_SberBank" %>

<!DOCTYPE html PUBLIC "-//W3C//Dtd XHTML 1.0 transitional//EN" "http://www.w3.org/tr/xhtml1/Dtd/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/payment/billstyle.css" rel="stylesheet" type="text/css" />
    <title>
        <%=Resources.Resource.Client_Bill_Title%></title>
</head>
<script type="text/javascript">
    function position_this_window() {
        var x = (screen.availWidth - 700) / 2;
        window.resizeTo(750, 550);
        window.moveTo(Math.floor(x), 50);
    }
</script>
<body onload="position_this_window();window.print();">
    <form id="form1" runat="server">
    <div align="center">
        <table width="700" border="1" style="border: #000000 1px solid; text-align: left;
            font-weight: bold;" cellpadding="0" cellspacing="0">
            <tr>
                <td style="width: 220px; text-align: center; vertical-align: top;">
                    Платеж
                </td>
                <td>
                    <table border="0" cellpadding="4">
                        <tr>
                            <td colspan="2">
                                Получатель:
                                <asp:Label ID="lCompanyName" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td runat="server" id="pnlKpp">
                                КПП:
                                <asp:Label ID="lKPP" runat="server" CssClass="value" />
                            </td>
                            <td>
                                ИНН:
                                <asp:Label ID="lINN" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <%--<td>
                                Код ОКАТО:__________________ <asp:Label ID="Label1" runat="server"></asp:Label>
                            </td>--%>
                            <td colspan="2">
                                P/сч.:
                                <asp:Label ID="lTransactAccount" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                в:
                                <asp:Label ID="lBankName" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                БИК:
                                <asp:Label ID="lBIK" runat="server" CssClass="value"></asp:Label>
                            </td>
                            <td>
                                К/сч.:
                                <asp:Label ID="lCorrespondentAccount" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <%--<tr>
                            <td colspan="2">
                                Код бюджетной классификации (КБК):________________
                            </td>
                        </tr>--%>
                        <tr>
                            <td colspan="2">
                                Платеж:
                                <asp:Label ID="lPaymentDescr" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: left">
                                Плательщик:
                                <asp:Label ID="lPayer" runat="server" CssClass="value" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                Адрес плательщика:
                                <asp:Label ID="lPayerAddress" runat="server" CssClass="value" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                ИНН плательщика:<asp:Label ID="lPayerINN" runat="server" Text="" CssClass="value"></asp:Label>
                            </td>
                            <%--<td>
                                № л/сч. плательщика:__________________
                            </td>--%>
                        </tr>
                        <tr>
                            <td colspan="2">
                                Сумма:
                                <asp:Label ID="lWholeSum" runat="server" CssClass="value"></asp:Label>
                                руб.
                                <asp:Label ID="lSumFractPart" runat="server" CssClass="value"></asp:Label>
                                коп.&nbsp;&nbsp; Сумма оплаты услуг банка:______ руб. __ коп.
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                Подпись:_________________ Дата: "__"__________
                                <%=DateTime.Now.Year%>г.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 220px; text-align: center; vertical-align: top;">
                    Квитанция
                </td>
                <td>
                    <table border="0" cellpadding="4">
                        <tr>
                            <td colspan="2">
                                Получатель:
                                <asp:Label ID="lCompanyName2" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td runat="server" id="pnlKpp2">
                                КПП:
                                <asp:Label ID="lKPP2" runat="server" CssClass="value"></asp:Label>
                            </td>
                            <td>
                                ИНН:
                                <asp:Label ID="lINN2" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <%--<td>
                                Код ОКАТО:__________________ <asp:Label ID="Label1" runat="server"></asp:Label>
                            </td>--%>
                            <td colspan="2">
                                P/сч.:
                                <asp:Label ID="lTransactAccount2" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                в:
                                <asp:Label ID="lBankName2" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                БИК:
                                <asp:Label ID="lBIK2" runat="server" CssClass="value"></asp:Label>
                            </td>
                            <td>
                                К/сч.:
                                <asp:Label ID="lCorrespondentAccount2" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <%--<tr>
                            <td colspan="2">
                                Код бюджетной классификации (КБК):________________
                            </td>
                        </tr>--%>
                        <tr>
                            <td colspan="2">
                                Платеж:
                                <asp:Label ID="lPaymentDescr2" runat="server" CssClass="value"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="text-align: left">
                                Плательщик:
                                <asp:Label ID="lPayer2" runat="server" CssClass="value" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                Адрес плательщика:
                                <asp:Label ID="lPayerAddress2" runat="server" CssClass="value" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                ИНН плательщика:<asp:Label ID="lPayerINN2" runat="server" Text="" CssClass="value"></asp:Label>
                            </td>
                            <%--<td>
                                № л/сч. плательщика:__________________
                            </td>--%>
                        </tr>
                        <tr>
                            <td colspan="2">
                                Сумма:
                                <asp:Label ID="lWholeSum2" runat="server" CssClass="value"></asp:Label>
                                руб.
                                <asp:Label ID="lSumFractPart2" runat="server" CssClass="value"></asp:Label>
                                коп.&nbsp;&nbsp; Сумма оплаты услуг банка:______ руб. __ коп.
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                Подпись:_________________ Дата: "__"__________<%=DateTime.Now.Year%>г.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
