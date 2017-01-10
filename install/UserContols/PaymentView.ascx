<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PaymentView.ascx.cs" Inherits="ClientPages.install_UserContols_PaymentView" %>
<h1>
    <% = Resources.Resource.Install_UserContols_PaymentView_h1%></h1>
<asp:MultiView runat="server" ID="mvPayment">
    <asp:View runat="server" ID="vNew">
        <fieldset class="group simple">
            <legend>
                <asp:CheckBox runat="server" ID="chbCash" Checked="True" Text="<%$ Resources:Resource, Install_UserContols_PaymentView_Cash %>" /></legend>
        </fieldset>
        <fieldset class="group simple" runat="server" id="divFizBank">
            <legend>
                <asp:CheckBox runat="server" ID="chbFizBank" Checked="True" Text="<%$ Resources:Resource, Install_UserContols_PaymentView_BankTransferFiz %>" /></legend>
        </fieldset>
        <fieldset class="group simple" runat="server" id="divUrBank">
            <legend>
                <asp:CheckBox runat="server" ID="chbUrBank" Checked="True" Text="<%$ Resources:Resource, Install_UserContols_PaymentView_BankTransferUr %>" /></legend>
        </fieldset>
        <fieldset class="group" runat="server" id="divCreditCard">
            <legend>
                <asp:CheckBox runat="server" ID="chbCreditCard" Checked="False" Text="<%$ Resources:Resource, Install_UserContols_PaymentView_CreditCard %>" /></legend>
            <div class="block-options">
                <p class="do">
                    <%= Resources.Resource.Install_UserContols_PaymentView_Agregator%></p>
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbRobokassaCreditcard" GroupName="CreaditCard"
                    onclick="ShowMethod('RobokassaCreditCard')" Text="Robokassa" Checked="true" />
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbAssistCreditcard" GroupName="CreaditCard" onclick="ShowMethod('AssistCreditCard')"
                    Text="Assist" />
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbPlatronCreditcard" GroupName="CreaditCard"
                    onclick="ShowMethod('PlatronCreditCard')" Text="Platron" />
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbZPaymentCreditcard" GroupName="CreaditCard"
                    onclick="ShowMethod('ZpaymentCreditCard')" Text="Zpayment" />
                <div class="list-methods" id="credit">
                    <div id="RobokassaCreditCard">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_RobokassaCreditCardLogin%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtLoginRobokassaCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_RobokassaCreditCardPass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtPassRobokassaCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="AssistCreditCard" style="display: none">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <%= Resources.Resource.Install_UserContols_PaymentView_AssistCreditCard_ShopID%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtShopIdAssistCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_AssistCreditCard_Login%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtLoginAssistCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_AssistCreditCard_Pass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtPassAssistCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="PlatronCreditCard" style="display: none">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <span>
                                        <% = Resources.Resource.Install_UserContols_PaymentView_PlatronCreditCard_IdSeller%></span>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtSellerIdPlatronCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <span>
                                        <% = Resources.Resource.Install_UserContols_PaymentView_PlatronCreditCard_Payment%></span>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtPaySystemCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <span>
                                        <% = Resources.Resource.Install_UserContols_PaymentView_PlatronCreditCard_PaymentPass%></span>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtPayPassCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="ZpaymentCreditCard" style="display: none">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_ZpaymentCreditCard_Poket%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtPayPoketCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_ZpaymentCreditCard_UserPass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtPassZpaymentCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_ZpaymentCreditCard_SecretKey%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtSecretKeyZpaymentCreditcard"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </fieldset>
        <fieldset class="group" runat="server" id="divElectronMoney">
            <legend>
                <asp:CheckBox runat="server" ID="chbElectronMoney" Checked="False" Text="<%$ Resources:Resource,Install_UserContols_PaymentView_ElectronMoney %>" /></legend>
            <div class="block-options">
                <p class="do">
                    <% = Resources.Resource.Install_UserContols_PaymentView_Agregator%>
                </p>
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbRobokassaElectronMoney" GroupName="ElectronMoney"
                    onclick="ShowMethod('RobokassaElectronMoney')" Text="Robokassa" Checked="true" />
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbAssistElectronMoney" GroupName="ElectronMoney"
                    onclick="ShowMethod('AssistElectronMoney')" Text="Assist" />
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbPlatronElectronMoney" GroupName="ElectronMoney"
                    onclick="ShowMethod('PlatronElectronMoney')" Text="Platron" />
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbZPaymentElectronMoney" GroupName="ElectronMoney"
                    onclick="ShowMethod('ZpaymentElectronMoney')" Text="Zpayment" />
                <div class="list-methods" id="electron">
                    <div id="RobokassaElectronMoney">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_RobokassaCreditCardLogin%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtLoginRobokassaElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_RobokassaCreditCardPass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtPassRobokassaElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="AssistElectronMoney" style="display: none">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_AssistCreditCard_ShopID%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtShopIdAssistElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <%= Resources.Resource.Install_UserContols_PaymentView_AssistCreditCard_Login%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtLoginAssistElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_AssistCreditCard_Pass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtPassAssistElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="PlatronElectronMoney" style="display: none">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_PlatronCreditCard_IdSeller%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtSellerIdPlatronElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <%= Resources.Resource.Install_UserContols_PaymentView_PlatronCreditCard_Payment%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtPaySystemElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_PlatronCreditCard_PaymentPass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtPayPassElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="ZpaymentElectronMoney" style="display: none">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_ZpaymentCreditCard_Poket%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtPayPoketElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_ZpaymentCreditCard_UserPass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtPassZpaymentElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <%  = Resources.Resource.Install_UserContols_PaymentView_ZpaymentCreditCard_SecretKey %>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtSecretKeyZpaymentElectronMoney"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </fieldset>
        <fieldset class="group" runat="server" id="divTerminals">
            <legend>
                <asp:CheckBox runat="server" ID="chbTerminals" Checked="False" Text="<%$ Resources:Resource, Install_UserContols_PaymentView_Terminals %>" /></legend>
            <div class="block-options">
                <p class="do">
                    <% = Resources.Resource.Install_UserContols_PaymentView_Agregator%></p>
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbRobokassaTerminals" GroupName="Terminals" onclick="ShowMethod('RobokassaTerminals')"
                    Text="Robokassa" Checked="true" />
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbAssistTerminals" GroupName="Terminals" onclick="ShowMethod('AssistTerminals')"
                    Text="Assist" />
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbPlatronTerminals" GroupName="Terminals" onclick="ShowMethod('PlatronTerminals')"
                    Text="Platron" />
                <asp:RadioButton CssClass="paymentVariant" runat="server" ID="rbZPaymentTerminals" GroupName="Terminals" onclick="ShowMethod('ZpaymentTerminals')"
                    Text="Zpayment" />
                <div class="list-methods" id="terminal">
                    <div id="RobokassaTerminals">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <%= Resources.Resource.Install_UserContols_PaymentView_RobokassaCreditCardLogin%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtLoginRobokassaTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <%= Resources.Resource.Install_UserContols_PaymentView_RobokassaCreditCardPass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtPassRobokassaTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="AssistTerminals" style="display: none">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_AssistCreditCard_ShopID%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtShopIdAssistTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_AssistCreditCard_Login%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtLoginAssistTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_AssistCreditCard_Pass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtPassAssistTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="PlatronTerminals" style="display: none">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_PlatronCreditCard_IdSeller%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtSellerIdPlatronTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <%= Resources.Resource.Install_UserContols_PaymentView_PlatronCreditCard_Payment%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtPaySystemTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_PlatronCreditCard_PaymentPass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtPayPassTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="ZpaymentTerminals" style="display: none">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_ZpaymentCreditCard_Poket%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtPayPoketTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_ZpaymentCreditCard_UserPass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtPassZpaymentTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_ZpaymentCreditCard_SecretKey%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtSecretKeyZpaymentTerminals"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </fieldset>
        <fieldset class="group" runat="server" id="divIphone">
            <legend>
                <asp:CheckBox runat="server" ID="chbIPhone" Checked="False" Text="<%$ Resources:Resource, Install_UserContols_PaymentView_ByIPnone %>" /></legend>
            <div class="list-methods">
                <div id="divIPhone" class="block-options">
                    <p class="do">
                        <% = Resources.Resource.Install_UserContols_PaymentView_Agregator%>
                    </p>
                    <asp:RadioButton runat="server" ID="RadioButton9" GroupName="IPhone" Text="Robokassa"
                        Checked="true" />
                    <div id="RobokassaIphone">
                        <table class="subform">
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_RobokassaCreditCardLogin%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtLoginRobokassaIPhone"></asp:TextBox></div>
                                </td>
                            </tr>
                            <tr>
                                <td class="param-name">
                                    <% = Resources.Resource.Install_UserContols_PaymentView_RobokassaCreditCardPass%>
                                </td>
                                <td>
                                    <div class="str">
                                        <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtPassRobokassaIPhone"></asp:TextBox></div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </fieldset>
        <p>
            <%= Resources.Resource.Install_UserContols_PaymentView_Warning%>
        </p>
    </asp:View>
    <asp:View runat="server" ID="vExistPayment">
        <div class="group">
            <% = Resources.Resource.Install_UserContols_PaymentView_IsExist%>
        </div>
    </asp:View>
</asp:MultiView>
<script type="text/javascript">
    $(function () {

        $(".paymentVariant input[type=radio]:checked").click();

        var robokassaCredit = $("#RobokassaCreditCard input");
        var robokassaElectron = $("#RobokassaElectronMoney input");
        var robokassaTerminals = $("#RobokassaTerminals input");
        var robokassaIphone = $("#RobokassaIphone input");
        var robokassa = [robokassaCredit, robokassaElectron, robokassaTerminals, robokassaIphone];

        processArray(robokassa, null, function (idx) {
            $(this).data("idx", idx);
        });

        var assistCredit = $("#AssistCreditCard input");
        var assistElectron = $("#AssistElectronMoney input");
        var assistTerminals = $("#AssistTerminals input");

        var assist = [assistCredit, assistElectron, assistTerminals];

        processArray(assist, null, function (idx) {
            $(this).data("idx", idx);
        });


        var platronCredit = $("#PlatronCreditCard input");
        var platronElectron = $("#PlatronElectronMoney input");
        var platronTerminals = $("#PlatronTerminals input");

        var platron = [platronCredit, platronElectron, platronTerminals];

        processArray(platron, null, function (idx) {
            $(this).data("idx", idx);
        });

        var zpaymentCredit = $("#ZpaymentCreditCard input");
        var zpaymentElectron = $("#ZpaymentElectronMoney input");
        var zpaymentTerminals = $("#ZpaymentTerminals input");

        var zpayment = [zpaymentCredit, zpaymentElectron, zpaymentTerminals];

        processArray(zpayment, null, function (idx) {
            $(this).data("idx", idx);
        });


        $("fieldset.group div.list-methods > div input").blur(function () {
            var el = $(this);

            if (!el.val().length || el.data("idx") == null) return true;

            var currentId = el.closest("div.list-methods").children("div:visible").attr("id").toLowerCase();
            var idxCurrent = el.data("idx");
            var elText = el.val();

            if (currentId.indexOf("robokassa") > -1) {
                processArray(robokassa, function () {
                    if (!$(this[idxCurrent]).val().length) {
                        $(this[idxCurrent]).val(elText);
                        $("form").validate().element($(this[idxCurrent]));
                    };
                });
                return true;
            } else if (currentId.indexOf("assist") > -1) {
                processArray(assist, function () {
                    if (!$(this[idxCurrent]).val().length) {
                        $(this[idxCurrent]).val(elText);
                        $("form").validate().element($(this[idxCurrent]));
                    };
                });
                return true;
            } else if (currentId.indexOf("platron") > -1) {
                processArray(platron, function () {
                    if (!$(this[idxCurrent]).val().length) {
                        $(this[idxCurrent]).val(elText);
                        $("form").validate().element($(this[idxCurrent]));
                    };
                });
                return true;
            } else if (currentId.indexOf("zpayment") > -1) {
                processArray(zpayment, function () {
                    if (!$(this[idxCurrent]).val().length) {
                        $(this[idxCurrent]).val(elText);
                        $("form").validate().element($(this[idxCurrent]));
                    };
                });
                return true;
            };
            return true;
        });
    });

    function processArray(arr, funcParent, funcChild) {
        $.each(arr, function (idxP, elP) {

            if (funcParent) funcParent.call(this, idxP, elP);

            $(this).each(function (idx, el) {
                if (funcChild) funcChild.call(this, idx, el);
            });
        });
    }
</script>
