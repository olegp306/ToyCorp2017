﻿<%@ Import Namespace="AdvantShop" %>

<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeFile="Check_BillUa.aspx.cs"
    Inherits="ClientPages.Check_BillUa" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 3.2//EN">
<html>
<head runat="server">
    <%--<link href="css/payment/stylech.css" rel="stylesheet" type="text/css" />--%>
    <title>
        Зразок заповнення платіжного доручення
    </title>
    <style>
		<!-- 
		body,DIV,table,THEAD,Tbody,TFOOT,TR,TH,TD,P { font-family:"Arial"; font-size:xx-small }
		 -->
	</style>
</head>
<script type="text/javascript">
    function position_this_window() {
        var x = (screen.availWidth - 820) / 2;
        window.resizeTo(820, 820);
        window.moveTo(Math.floor(x), 50);
    }

    var mapNumbers = {
        0: [2, 1, "нуль"],
        1: [0, 2, "один", "одна"],
        2: [1, 2, "два", "дві"],
        3: [1, 1, "три"],
        4: [1, 1, "чотири"],
        5: [2, 1, "п'ять"],
        6: [2, 1, "шість"],
        7: [2, 1, "сім"],
        8: [2, 1, "вісім"],
        9: [2, 1, "дев'ять"],
        10: [2, 1, "десять"],
        11: [2, 1, "одинадцять"],
        12: [2, 1, "дванадцять"],
        13: [2, 1, "тринадцять"],
        14: [2, 1, "чотирнадцять"],
        15: [2, 1, "п'ятнадцять"],
        16: [2, 1, "шістнадцять"],
        17: [2, 1, "сімнадцять"],
        18: [2, 1, "вісімнадцять"],
        19: [2, 1, "дев'ятнадцять"],
        20: [2, 1, "двадцять"],
        30: [2, 1, "тридцять"],
        40: [2, 1, "сорок"],
        50: [2, 1, "п'ятдесят"],
        60: [2, 1, "шістдесят"],
        70: [2, 1, "сімдесят"],
        80: [2, 1, "вісімдесят"],
        90: [2, 1, "дев'яносто"],
        100: [2, 1, "сто"],
        200: [2, 1, "двісті"],
        300: [2, 1, "триста"],
        400: [2, 1, "чотириста"],
        500: [2, 1, "п'ятсот"],
        600: [2, 1, "шістсот"],
        700: [2, 1, "сімсот"],
        800: [2, 1, "вісімсот"],
        900: [2, 1, "дев'ятсот"]
    };

    var mapOrders = [
        { _Gender: false, _arrStates: ["гривня", "гривні", "гривень"], _bAddZeroWord: true },
        { _Gender: false, _arrStates: ["тисяча", "тисячі", "тисяч"] },
        { _Gender: true, _arrStates: ["мільйон", "мільйона", "мільйонів"] },
        { _Gender: true, _arrStates: ["мільярд", "мільярда", "мільярдів"] },
        { _Gender: true, _arrStates: ["триліон", "триліона", "триліонів"] }
    ];

    var objKop = { _Gender: false, _arrStates: ["копійка", "копійки", "копійок"] };

    function Value(dVal, bGender) {
        var xVal = mapNumbers[dVal];
        if (xVal[1] == 1) {
            return xVal[2];
        } else {
            return xVal[2 + (bGender ? 0 : 1)];
        }
    }

    function From0To999(fValue, oObjDesc, fnAddNum, fnAddDesc) {
        var nCurrState = 2;
        if (Math.floor(fValue / 100) > 0) {
            var fCurr = Math.floor(fValue / 100) * 100;
            fnAddNum(Value(fCurr, oObjDesc._Gender));
            nCurrState = mapNumbers[fCurr][0];
            fValue -= fCurr;
        }

        if (fValue < 20) {
            if (Math.floor(fValue) > 0 || (oObjDesc._bAddZeroWord)) {
                fnAddNum(Value(fValue, oObjDesc._Gender));
                nCurrState = mapNumbers[fValue][0];
            }
        } else {
            var fCurr = Math.floor(fValue / 10) * 10;
            fnAddNum(Value(fCurr, oObjDesc._Gender));
            nCurrState = mapNumbers[fCurr][0];
            fValue -= fCurr;

            if (Math.floor(fValue) > 0) {
                fnAddNum(Value(fValue, oObjDesc._Gender));
                nCurrState = mapNumbers[fValue][0];
            }
        }

        fnAddDesc(oObjDesc._arrStates[nCurrState]);
    }

    function FloatToSamplesInWordsUkr(fAmount) {
        var fInt = Math.floor(fAmount + 0.005);
        var fDec = Math.floor(((fAmount - fInt) * 100) + 0.5);

        var arrRet = [];
        var iOrder = 0;
        var arrSouthands = [];
        for (; fInt > 0.9999; fInt /= 1000) {
            arrSouthands.push(Math.floor(fInt % 1000));
        }
        if (arrSouthands.length == 0) {
            arrSouthands.push(0);
        }

        function PushToRes(strVal) {
            arrRet.push(strVal);
        }
        for (var iSouth = arrSouthands.length - 1; iSouth >= 0; --iSouth) {
            From0To999(arrSouthands[iSouth], mapOrders[iSouth], PushToRes, PushToRes);
        }

        if (arrRet.length > 0) {
            // Capitalize first letter
            arrRet[0] = arrRet[0].match(/^(.)/)[1].toLocaleUpperCase() + arrRet[0].match(/^.(.*)$/)[1];
        }

        arrRet.push((fDec < 10) ? ("0" + fDec) : ("" + fDec));
        From0To999(fDec, objKop, function () { }, PushToRes);

        return arrRet.join(" ");
    }
</script>
<body onload="position_this_window();window.print();">
<form id="form1" runat="server">
    <table cellspacing="0" cols="36" border="0" width="768px">
	    <colgroup width="21"></colgroup>
	    <colgroup width="13"></colgroup>
	    <colgroup width="8"></colgroup>
	    <colgroup span="17" width="21"></colgroup>
	    <colgroup width="7"></colgroup>
	    <colgroup width="14"></colgroup>
	    <colgroup span="2" width="21"></colgroup>
	    <colgroup width="8"></colgroup>
	    <colgroup width="13"></colgroup>
	    <colgroup width="21"></colgroup>
	    <colgroup width="1"></colgroup>
	    <colgroup width="20"></colgroup>
	    <colgroup span="7" width="21"></colgroup>
	    <tbody><tr>
		    <td style="border-top: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000" colspan="36" height="17" align="center" valign="top"><b><font size="2">Зразок заповнення платіжного доручення</font></b></td>
		    </tr>
	    <tr>
		    <td style="border-left: 1px solid #000000" height="9" align="left"><font face="Times New Roman"><br></font></td>
		    <td align="left" colspan="34"><br></td>
		    <td style="border-right: 1px solid #000000" align="left"><br></td>
	    </tr>
	    <tr>
		    <td style="border-left: 1px solid #000000" colspan="5" height="43" align="left" valign="middle"><font face="Times New Roman" size="2">Одержувач</font></td>
		    <td colspan="12" align="center" valign="middle">
                <b><font size="2">                
                    <asp:Literal ID="liCompanyName" runat="server" /> </font></b>                
            </td>
		    <td align="left" colspan="18"><br></td>
		    <td style="border-right: 1px solid #000000" align="left"><br></td>
	    </tr>
	    <tr>
		    <td style="border-left: 1px solid #000000" colspan="4" rowspan="2" height="30" align="left" valign="middle"><font face="Times New Roman" size="2">Код</font></td>
		    <td align="left"><font face="Times New Roman"><br></font></td>
		    <td style="border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000" colspan="9" rowspan="2" align="center" valign="middle" sdval="35880538" sdnum="1033;">
                <b><font size="2">
                    <asp:Literal ID="liCompanyCode" runat="server" /> </font></b>
            </td>
		    <td align="left" colspan="8"><br></td>
		    <td colspan="11" align="center" valign="middle"><font face="Times New Roman" size="2">КРЕДИТ рах. N</font></td>
		    <td align="left" colspan="2"><br></td>
		    <td style="border-right: 1px solid #000000" align="left"><br></td>
	    </tr>
	    <tr>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td style="border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000" colspan="11" rowspan="2" align="center" valign="middle" sdnum="1033;0;0">
                    <b><font size="2"><asp:Literal ID="liCredit" runat="server" /></font></b>
            </td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td style="border-right: 1px solid #000000" align="left"><br></td>
	    </tr>
	    <tr>
		    <td style="border-left: 1px solid #000000" colspan="8" height="18" align="left"><font face="Times New Roman" size="2">Банк одержувача</font></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td colspan="4" align="center"><font face="Times New Roman" size="2">Код банку</font></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td style="border-right: 1px solid #000000" align="left"><br></td>
	    </tr>
	    <tr>
		    <td style="border-left: 1px solid #000000" colspan="3" height="6" align="left"><font face="Times New Roman"><br></font></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td style="border-top: 1px solid #000000; border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000" colspan="4" rowspan="2" align="center" valign="middle" sdnum="1033;">
                    <b><font size="2"><asp:Literal ID="liBankCode" runat="server" /></font></b>
            </td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000" colspan="11" rowspan="2" align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td style="border-right: 1px solid #000000" align="left"><br></td>
	    </tr>
	    <tr>
		    <td style="border-left: 1px solid #000000" colspan="16" height="28" align="left">
                <b><font size="2"><asp:Literal ID="liBankName" runat="server" /> </font></b>
            </td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td style="border-right: 1px solid #000000" align="left"><br></td>
	    </tr>
	    <tr>
		    <td style="border-bottom: 1px solid #000000; border-left: 1px solid #000000" height="11" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000;" colspan="34"><br></td>
		    <td style="border-bottom: 1px solid #000000; border-right: 1px solid #000000" align="left"><br></td>
	    </tr>
	    <tr>
		    <td height="45" align="left" colspan="37"><br></td>
	    </tr>	
	    <tr>
		    <td style="border-bottom: 1px solid #000000" colspan="36" height="28" align="left" valign="middle">
                <b><font size="4">
                    <asp:Literal ID="liOrderNum" runat="server" /></font></b></td>
	    </tr>
	    <tr>
		    <td height="15" align="left" colspan="36"><br></td>
	    </tr>
	    <tr>
		    <td colspan="6" height="23" align="left" valign="middle"><font size="2">Постачальник:</font></td>
		    <td colspan="30" align="left" valign="top">
                <b><font size="2"> 
                    <asp:Literal ID="liCompanyName2" runat="server" /> </font></b></td>
		    </tr>
	    <tr>
		    <td height="88" align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td colspan="30" align="left" valign="top">
                <font size="2">
                    <asp:Literal ID="liCompanyEssencials" runat="server" /> </font></td>
	    </tr>
	    <tr>
		    <td colspan="6" height="18" align="left" valign="middle"><font size="2">Покупець:</font></td>
		    <td colspan="30" align="left" valign="top">
                <b><font size="2">
                    <asp:Literal ID="liBuyerInfo" runat="server" /> </font></b></td>
	    </tr>
	    <tr>
		    <td height="9" align="left" colspan="36"><br></td>
	    </tr>
	    <tr>
		    <td colspan="6" height="18" align="left"><font size="2">Договір:</font></td>
		    <td colspan="30" align="left"><font size="2"><br></font></td>
		    </tr>
	    <tr>
		    <td height="9" align="left" colspan="36"><br></td>
	    </tr>    

        <asp:Repeater ID="rprOrrderItems" runat="server">
            <HeaderTemplate>
                <tr>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="2" height="34" align="center" valign="middle" bgcolor="#FCFAEB"><b><font size="2">№</font></b></td>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="19" align="center" valign="middle" bgcolor="#FCFAEB"><b><font size="2">Товар</font></b></td>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="4" align="center" valign="middle" bgcolor="#FCFAEB"><b><font size="2">Кіл-сть</font></b></td>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="3" align="center" valign="middle" bgcolor="#FCFAEB"><b><font size="2">Од.</font></b></td>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="4" align="center" valign="middle" bgcolor="#FCFAEB"><b><font size="2">Ціна з ПДВ</font></b></td>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000" colspan="4" align="center" valign="middle" bgcolor="#FCFAEB"><b><font size="2">Сума з ПДВ</font></b></td>
		        </tr>

            </HeaderTemplate>
            <ItemTemplate>
                <tr>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="2" height="16" align="center" valign="top" sdval="1" sdnum="1033;0;0">
                        <%# Container.ItemIndex + 1%>
		            </td>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="19" align="left" valign="top">
                        <%# Eval("Name") %>
		            </td>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="4" align="RIGHT" valign="top" sdval="2" sdnum="1033;0;0">
                        <%# Eval("Amount") %>
		            </td>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="3" align="left" valign="top">
                        <%# GetUnit(AdvantShop.Helpers.SQLDataHelper.GetInt(Eval("ProductId"))) %>&nbsp;
		            </td>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="4" align="RIGHT" valign="top" sdval="544" sdnum="1033;0;0.00">
                        <%# GetPrice(AdvantShop.Helpers.SQLDataHelper.GetFloat(Eval("Price")), Order.OrderCurrency)%>
		            </td>
		            <td style="border-top: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000" colspan="4" align="RIGHT" valign="top" bgcolor="#FFFFFF" sdval="1088" sdnum="1033;0;0.00">
                        <%# GetPrice(AdvantShop.Helpers.SQLDataHelper.GetFloat(Eval("Price")) * AdvantShop.Helpers.SQLDataHelper.GetFloat(Eval("Amount")), Order.OrderCurrency)%>
		            </td>
		        </tr>
            </ItemTemplate>
        </asp:Repeater>
                <% if (Order != null && Order.ShippingCost != 0)
                   { %>
                <tr>
                    <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="2" height="16" align="center" valign="top" sdval="1" sdnum="1033;0;0">
                        <%= Order.OrderItems.Count + 1 %>
                    </td>
                    <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="19" align="left" valign="top">Послуги з доставки
                    </td>
                    <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="4" align="RIGHT" valign="top" sdval="2" sdnum="1033;0;0">1
                    </td>
                    <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="3" align="left" valign="top">&nbsp</td>
                    <td style="border-top: 1px solid #000000; border-left: 1px solid #000000" colspan="4" align="RIGHT" valign="top" sdval="544" sdnum="1033;0;0.00">
                        <%= GetPrice(Order.ShippingCost, Order.OrderCurrency)%>
                    </td>
                    <td style="border-top: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000" colspan="4" align="RIGHT" valign="top" bgcolor="#FFFFFF" sdval="1088" sdnum="1033;0;0.00">
                        <%= GetPrice(Order.ShippingCost, Order.OrderCurrency)%>
                    </td>
                </tr>
                <% } %>

	    <tr>
		    <td style="border-top: 1px solid #000000" height="9" align="left" colspan="36"><br></td>
	    </tr>
        <tr id="trDiscount" runat="server">
		    <td height="16" align="left" colspan="28"><br></td>	
		    <td align="RIGHT" valign="top" colspan="4"><b><font size="2">Знижка:</font></b></td>
		    <td colspan="4" align="RIGHT" valign="top" sdval="1448" sdnum="1033;0;0.00">
                <b><font size="2"><asp:Literal ID="liDiscount" runat="server" /> </font></b>
		    </td>
		</tr>
	    <tr>
		    <td height="16" align="left" colspan="28"><br></td>	
		    <td align="RIGHT" valign="top" colspan="4"><b><font size="2">Всього:</font></b></td>
		    <td colspan="4" align="RIGHT" valign="top" sdval="1448" sdnum="1033;0;0.00">
                <b><font size="2"><asp:Literal ID="liTotal" runat="server" /> </font></b>
		    </td>
		</tr>
	    <tr id="trTaxSum" runat="server">
		    <td height="16" align="left" colspan="25"><br></td>
		    <td align="RIGHT" valign="top" colspan="7"><b><font size="2">У тому числі ПДВ:</font></b></td>
		    <td colspan="4" align="RIGHT" valign="top" sdval="241.333333333333" sdnum="1033;0;0.00">
                <b><font size="2">
                    <asp:Literal ID="liTaxSum" runat="server" /></font></b></td>
		</tr>
	    <tr>
		    <td height="9" align="left" colspan="36"><br></td>
	    </tr>
	    <tr>
		    <td colspan="36" height="15" align="left">
                <asp:Literal ID="liTotalCount" runat="server" /></td>
		</tr>
	    <tr id="trTax" runat="server" visible="false">
		    <td colspan="35" height="35" align="left" valign="top">
                <b><font size="2"><span id="summTotal"></span> 
                <br>У т.ч. ПДВ: <span id="tax"></span> <br></font></b>                
		    </td>
		    <td align="left"><br></td>
	    </tr>
	    <tr>
		    <td style="border-bottom: 1px solid #000000" height="9" colspan="36"><br></td>
	    </tr>
	    <tr>
		    <td height="15" colspan="36"><br></td>
	    </tr>
	    <tr>
		    <td height="16" align="left" colspan="18"><br></td>
		    <td align="left" colspan="2"><b><font size="2">Виписав(ла):</font></b></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td style="border-bottom: 1px solid #000000" align="left"><br></td>
		    <td align="left"><br></td>
	    </tr>
    </tbody>
    </table>
    <asp:HiddenField ID="hfTotal" runat="server" />
    <asp:HiddenField ID="hfTax" runat="server" Value="0" />
    <script type="text/javascript">
        var summ = document.getElementById("summTotal");
        var total = parseFloat(document.getElementById("hfTotal").value.replace(" ", "").replace(",", "."));

        summ.innerHTML = FloatToSamplesInWordsUkr(total);

        // tax
        var tax = document.getElementById("tax");
        var totalTax = parseFloat(document.getElementById("hfTax").value.replace(" ", "").replace(",", "."));

        if (totalTax != 0) {
            tax.innerHTML = FloatToSamplesInWordsUkr(totalTax);
        }
    </script>
    </form>
</body>
</html>
