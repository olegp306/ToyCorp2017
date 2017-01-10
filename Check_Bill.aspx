<%@ Import Namespace="AdvantShop" %>

<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeFile="Check_Bill.aspx.cs" Inherits="ClientPages.Check_Bill" %>

<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.FilePath" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Import Namespace="AdvantShop.Repository.Currencies" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="css/payment/stylech.css" rel="stylesheet" type="text/css" />
    <title>
        <%=Resources.Resource.Client_Bill2_Title%></title>
</head>
<script type="text/javascript">
    function position_this_window() {
        var x = (screen.availWidth - 770) / 2;
        window.resizeTo(770, 770);
        window.moveTo(Math.floor(x), 50);
    }
</script>
<script language="JavaScript" type="text/javascript">

    /* ----------------------------
    Сумма прописью на JavaScript
    Автор: Mad Max
    ---------------------------- */

    var money;
    var price;
    var rub, kop;
    var litera = sotny = desatky = edinicy = minus = "";
    var k = 0, i, j;

    N = ["", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять",
"", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать",
"", "десять", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто",
"", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот",
"тысяч", "тысяча", "тысячи", "тысячи", "тысячи", "тысяч", "тысяч", "тысяч", "тысяч", "тысяч",
"миллионов", "миллион", "миллиона", "миллиона", "миллиона", "миллионов", "миллионов", "миллионов", "миллионов", "миллионов",
"миллиардов", "миллиард", "миллиарда", "миллиарда", "миллиарда", "миллиардов", "миллиардов", "миллиардов", "миллиардов", "миллиардов"];

    var M = new Array(10);
    for (j = 0; j < 10; ++j)
        M[j] = new Array(N.length);

    for (i = 0; i < N.length; i++)
        for (j = 0; j < 10; j++)
            M[j][i] = N[k++];

    var R = new Array("рублей", "рубль", "рубля", "рубля", "рубля", "рублей", "рублей", "рублей", "рублей", "рублей");
    var K = new Array("копеек", "копейка", "копейки", "копейки", "копейки", "копеек", "копеек", "копеек", "копеек", "копеек");

    function num2str(money, target) {
        rub = "", kop = "";
        money = money.replace(",", ".");

        if (isNaN(money)) { document.getElementById(target).innerHTML = "Не числовое значение"; return }
        if (money.substr(0, 1) == "-") { money = money.substr(1); minus = "минус " }
        else minus = "";
        money = Math.round(money * 100) / 100 + "";

        if (money.indexOf(".") != -1) {
            rub = money.substr(0, money.indexOf("."));
            kop = money.substr(money.indexOf(".") + 1);
            if (kop.length == 1) kop += "0";
        }
        else rub = money;

        if (rub.length > 12) { document.getElementById(target).innerHTML = "Слишком большое число"; return }

        ru = propis(price = rub, R);
        ko = propis(price = kop, K);
        ko != "" ? res = ru + " " + ko : res = ru;
        ru == "Ноль " + R[0] && ko != "" ? res = ko : 0;
        kop == 0 ? res += " ноль " + K[0] : 0;
        //document.getElementById(target).innerHTML = ru.substr(0,1).toUpperCase()
        document.getElementById(target).innerHTML = (minus + ru).substr(0, 1).toUpperCase() + (minus + ru).substr(1);
    }

    function propis(price, D) {
        litera = "";
        for (i = 0; i < price.length; i += 3) {
            sotny = desatky = edinicy = "";
            if (n(i + 2, 2) > 10 && n(i + 2, 2) < 20) {
                edinicy = " " + M[n(i + 1, 1)][1] + " " + M[0][i / 3 + 3];
                i == 0 ? edinicy += D[0] : 0;
            }
            else {
                edinicy = M[n(i + 1, 1)][0];
                (edinicy == "один" && (i == 3 || D == K)) ? edinicy = "одна" : 0;
                (edinicy == "два" && (i == 3 || D == K)) ? edinicy = "две" : 0;
                i == 0 && edinicy != "" ? 0 : edinicy += " " + M[n(i + 1, 1)][i / 3 + 3];
                edinicy == " " ? edinicy = "" : (edinicy == " " + M[n(i + 1, 1)][i / 3 + 3]) ? 0 : edinicy = " " + edinicy;
                i == 0 ? edinicy += " " + D[n(i + 1, 1)] : 0;
                (desatky = M[n(i + 2, 1)][2]) != "" ? desatky = " " + desatky : 0;
            }
            (sotny = M[n(i + 3, 1)][3]) != "" ? sotny = " " + sotny : 0;
            if (price.substr(price.length - i - 3, 3) == "000" && edinicy == " " + M[0][i / 3 + 3]) edinicy = "";
            litera = sotny + desatky + edinicy + litera;
        }
        if (litera == " " + R[0]) return "ноль" + litera;
        else return litera.substr(1);
    }

    function n(start, len) {
        if (start > price.length) return 0;
        else return Number(price.substr(price.length - start, len));
    }

</script>
<body onload="position_this_window();window.print();">
    <form id="form1" runat="server">
    <div>
        <div class="Vnimanie">
            &quot;Внимание! Оплата данного счета означает согласие с условиями поставки товара. Уведомление об оплате
            обязательно, в противном случае не гарантируется наличие товара на складе. Товар отпускается по факту
            прихода денег на р/с Поставщика, самовывозом, при наличии доверенности и паспорта.&quot;
        </div>
        <br />
        <div class="Obrazech">
            Образец заполнения платежного поручения</div>
        <br />
        <table class="TableRekvizit">
            <tr>
                <td rowspan="2" colspan="2">
                    <div>
                        <asp:Label ID="lblbank" runat="server" /></div>
                    <div class="BankPoluchatel">
                        Банк получателя</div>
                </td>
                <td>
                    БИК
                </td>
                <td style="border-bottom-width: 0;">
                    <asp:Label ID="lblbik" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    Сч. №
                </td>
                <td style="border-top-width: 0;">
                    <asp:Label ID="lCorrespondentAccount" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="INN">
                    ИНН
                    <asp:Label ID="lblinn" runat="server" />
                </td>
                <td class="KPP">
                    <div runat="server" id="divKPP">
                        КПП
                        <asp:Label ID="lblkpp" runat="server" />
                    </div>
                </td>
                <td rowspan="2">
                    Сч. №
                </td>
                <td rowspan="2">
                    <asp:Label ID="lTransactAccount" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div>
                        <asp:Label ID="lblcompanyname" runat="server" /></div>
                    <div class="Poluchatel">
                        Получатель</div>
                </td>
            </tr>
        </table>
        <br />
        <h1>
            Счет №
            <asp:Label ID="lblOrderID" runat="server" />
            от
            <asp:Label ID="lblDateTime" runat="server" /></h1>
        <br />
        <hr class="HrBlack" />
        <table style="width: 100%">
            <tr>
                <td>
                    Поставщик:
                </td>
                <td class="Rekviziti">
                    <asp:Label ID="lblProvider" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="padding-top: 10px;">
                    Покупатель:&nbsp;
                </td>
                <td class="Rekviziti" style="padding-top: 10px;">
                    <%--<asp:Label ID="lblbuyercompany" runat="server" />
          <asp:label ID="lblbuyerinn" runat="server" />
          <asp:Label ID="lblbuyeraddress" runat="server" />--%>
                    <asp:Label ID="lblBuyer" runat="server" />
                </td>
            </tr>
        </table>
        <br />
        <center>
            <table cellspacing="1" cellpadding="4" width="100%" bgcolor="#aaaaaa" border="0"
                class="Spisok">
                <tbody>
                    <tr class="Zag" bgcolor="white">
                        <td>
                            <font class="sc">№</font>
                        </td>
                        <td width="30%">
                            <font class="sc">
                                <%=Resources.Resource.Client_Bill2_JobName%></font>
                        </td>
                        <td>
                            <font class="sc">
                                <%=Resources.Resource.Client_Bill2_Count%></font>
                        </td>
                        <td>
                            <font class="sc">
                                <%=Resources.Resource.Client_Bill2_Price%></font>
                        </td>
                        <td>
                            <font class="sc">Сумма без скидки</font>
                        </td>
                        <td>
                            <font class="sc">Скидка</font>
                        </td>
                        <td width="15%">
                            <font class="sc">
                                <%=Resources.Resource.Client_Bill2_Sum%></font>
                        </td>
                    </tr>
                    <asp:Repeater runat="server" ID="rptOrderItems" DataSource="<%# Order.OrderItems %>">
                        <ItemTemplate>
                            <tr bgcolor="white">
                                <td>
                                    <font class="sc">
                                        <asp:Literal runat="server" Text="<%# Container.ItemIndex + 1%>" /></font>
                                </td>
                                <td>
                                    <font class="sc"><b>
                                        <asp:Literal runat="server" Text='<%# Eval("ArtNo") + ", " + Eval("Name") %>' />
                                    </b></font>
                                    <td>
                                        <font class="sc">
                                            <asp:Literal ID="Literal3" runat="server" Text='<%# Eval("Amount") %>' /></font>
                                    </td>
                                    <td>
                                        <font class="sc">
                                            <asp:Literal ID="Literal4" runat="server" Text='<%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Price")), Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode)%>' /></font>
                                    </td>
                                    <td>
                                        <font class="sc">
                                            <asp:Literal ID="Literal5" runat="server" Text='<%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Price")) *SQLDataHelper.GetFloat(Eval("Amount")) , Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode) %>' /></font>
                                    </td>
                                    <td>
                                        <font class="sc">
                                            <asp:Literal ID="Literal6" runat="server" Text='<%# Math.Round(0.0, 2).ToString("##,##0.00") + "%"%>' />
                                        </font>
                                    </td>
                                    <td style="text-align: right">
                                        <font class="sc">
                                            <asp:Literal ID="Literal7" runat="server" Text='<%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Price")) * SQLDataHelper.GetFloat(Eval("Amount")) , Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode) %>' /></font>
                                    </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                     <asp:Repeater runat="server" ID="rptOrderCertificates" DataSource="<%# Order.OrderCertificates %>">
                        <ItemTemplate>
                            <tr bgcolor="white">
                                <td>
                                    <font class="sc">
                                        <asp:Literal runat="server" Text="<%# Container.ItemIndex + 1%>" /></font>
                                </td>
                                <td>
                                    <font class="sc"><b>
                                        <asp:Literal ID="Literal8" runat="server" Text='<%# Resources.Resource.Client_GiftCertificate_Header + ": " + Eval("CertificateCode")%>' />
                                    </b></font>
                                    <td>
                                        <font class="sc">
                                            <asp:Literal ID="Literal3" runat="server" Text='1' /></font>
                                    </td>
                                    <td>
                                        <font class="sc">
                                            <asp:Literal ID="Literal4" runat="server" Text='<%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Sum")), Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode)%>' /></font>
                                    </td>
                                    <td>
                                        <font class="sc">
                                            <asp:Literal ID="Literal5" runat="server" Text='<%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Sum")), Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode) %>' /></font>
                                    </td>
                                    <td>
                                        <font class="sc">
                                            <asp:Literal ID="Literal6" runat="server" Text='<%# Math.Round(0.0, 2).ToString("##,##0.00") + "%"%>' />
                                        </font>
                                    </td>
                                    <td style="text-align: right">
                                        <font class="sc">
                                            <asp:Literal ID="Literal7" runat="server" Text='<%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Sum")) , Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode) %>' /></font>
                                    </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr bgcolor="white" id="trShipping" runat="server" visible="<%# Order.ShippingCost != 0 %>">
                        <td>
                            <font class="sc">
                                <%= rptOrderItems.Items.Count + 1%>
                            </font>
                        </td>
                        <td colspan="5">
                            <font class="sc"><b>
                                <nobr>Услуги по доставке</nobr>
                            </b></font>
                        </td>
                        <td style="text-align: right">
                            <font class="sc">
                                <asp:Literal ID="Literal2" runat="server" Text='<%# CatalogService.GetStringPrice(Order.ShippingCost , Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode)%>' /></font>
                        </td>
                    </tr>
                    <tr bgcolor="white" id="trPaymentCost" runat="server">
                        <td>
                            <font class="sc">
                                <%= rptOrderItems.Items.Count + (Order.ShippingCost != 0 ? 2 : 1)%>
                            </font>
                        </td>
                        <td colspan="5">
                            <font class="sc"><b>
                                <nobr>Наценка на метод оплаты</nobr>
                            </b></font>
                        </td>
                        <td style="text-align: right">
                            <font class="sc">
                                <%= CatalogService.GetStringPrice(Order.PaymentCost , Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode)%></font>
                        </td>
                    </tr>
                </tbody>
            </table>
        </center>
        <br />
        <table class="Rekviziti" cellspacing="0" cellpadding="5" width="100%" border="0">
            <tbody>
                <tr bgcolor="white">
                    <td align="right" width="82%">
                        <font class="sc"><b>
                            <%=Resources.Resource.Client_Bill2_Total%>:</b></font>
                    </td>
                    <td align="right" width="18%">
                        <font class="sc"><b>
                            <asp:Label ID="lbltotalprice" runat="server" /></b></font>
                    </td>
                </tr>
                <asp:Literal ID="literalTaxCost" runat="server" />
                <tr>
                    <td align="right" width="82%">
                        <font class="sc"><b>
                            <%=Resources.Resource.Client_Bill2_TotalDiscount%></b></font>
                    </td>
                    <td align="right" width="18%">
                        <font class="sc"><b>
                            <asp:Label ID="lTotalDiscount" runat="server" /></b></font>
                    </td>
                </tr>
                <tr bgcolor="white">
                    <td align="right" width="82%">
                        <font class="sc"><b>
                            <%=Resources.Resource.Client_Bill2_Paid%></b></font>
                    </td>
                    <td align="right" width="18%">
                        <font class="sc"><b>
                            <asp:Label ID="lbltotalpricetopay" runat="server" /></b></font>
                    </td>
                </tr>
            </tbody>
        </table>
        <br />
        <div>
            Всего наименований
            <% Response.Write(rptOrderItems.Items.Count + (Order.ShippingCost != 0 ? 1 : 0) + (Order.PaymentCost != 0 ? 1 : 0)); %>, на сумму
            <asp:Label ID="lbltotalprice2" runat="server" /></div>
        <div class="Rekviziti">
            <table style="font-weight: bold;" border="0">
                <tr>
                    <td>
                        <div class="cat" id="str">
                        </div>
                    </td>
                    <td>
                        &nbsp;<asp:Label ID="lbltotalkop" class="cat" runat="server" />
                    </td>
                </tr>
            </table>
            <hr class="HrBlack" />
            <table width="100%">
                <tbody>
                    <tr style="font-weight: bold;">
                        <td>
                            Руководитель
                        </td>
                        <td style="padding: 0 20px; text-align: center;">
                            ______________________
                        </td>
                        <td style="padding: 0 20px; text-align: center;">
                            <div style="height: 0px; position: relative; z-index: 0;">
                                <div style="position: absolute; top: 0px; left: 0px; width: 100%; text-align: center; z-index: 0;">
                                    <% if (SettingsBank.StampImageName.IsNotEmpty())
                                       {%>
                                    <img src="<%= FoldersHelper.GetPath(FolderType.Pictures, SettingsBank.StampImageName, false)%>" />
                                    <% } %>
                                </div>
                            </div>
                            <div style="position: relative; z-index: 1;">
                                ______________________</div>
                        </td>
                        <td style="padding: 0 20px; text-align: center; text-decoration: underline;">
                            <asp:Label ID="lblDirector" runat="server" />
                        </td>
                    </tr>
                    <tr style="font-size: 10px; text-align: center; height: 24px;">
                        <td>
                        </td>
                        <td>
                            должность
                        </td>
                        <td>
                            <div style="position: relative; z-index: 1;">
                                подпись</div>
                        </td>
                        <td>
                            расшифровка подписи
                        </td>
                    </tr>
                    <tr style="font-weight: bold;">
                        <td colspan="2">
                            Главный (старший) бухгалтер
                        </td>
                        <td style="padding: 0 20px; text-align: center;">
                            <div style="position: relative; z-index: 1;">
                                ______________________</div>
                        </td>
                        <td style="padding: 0 20px; text-align: center; text-decoration: underline;">
                            <asp:Label ID="lblAccountant" runat="server" />
                        </td>
                    </tr>
                    <tr style="font-size: 10px; text-align: center; height: 24px;">
                        <td>
                        </td>
                        <td>
                        </td>
                        <td>
                            <div style="position: relative; z-index: 1;">
                                подпись</div>
                        </td>
                        <td>
                            расшифровка подписи
                        </td>
                    </tr>
                    <tr style="font-weight: bold;">
                        <td>
                            Ответственный
                        </td>
                        <td style="padding: 0 20px; text-align: center;">
                            ______________________
                        </td>
                        <td style="padding: 0 20px; text-align: center;">
                            ______________________
                        </td>
                        <td style="padding: 0 20px; text-align: center; text-decoration: underline;">
                            <asp:Label ID="lblManager" runat="server" />
                        </td>
                    </tr>
                    <tr style="font-size: 10px; text-align: center; height: 24px;">
                        <td>
                        </td>
                        <td>
                            должность
                        </td>
                        <td>
                            подпись
                        </td>
                        <td>
                            расшифровка подписи
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
