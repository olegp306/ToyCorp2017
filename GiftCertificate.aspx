<%@ Page Language="C#" MasterPageFile="MasterPage.master" CodeFile="GiftCertificate.aspx.cs"
    Inherits="ClientPages.GiftCertificate_Page" EnableViewState="false" %>

<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Payment" %>
<%@ Import Namespace="Resources" %>
<%@ Register Src="UserControls/Captcha.ascx" TagName="CaptchaControl" TagPrefix="adv" %>
<%@ Register TagPrefix="adv" TagName="GiftCertificate" Src="~/UserControls/GiftCertificate.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">

    <div class="stroke">
        <div class="content-owner">
            <h1>
                <% = Resource.Client_GiftCertificate_Header %></h1>
            <div class="form-c">
                <ul class="form form-vr">
                    <li>
                        <div class="param-name">
                            <label for="txtTo">
                                <%= Resource.Client_GiftCertificate_To%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:AdvTextBox ID="txtTo" runat="server" ValidationType="Required" />
                        </div>
                    </li>
                    <li>
                        <div class="param-name">
                            <label for="txtFrom">
                                <%= Resource.Client_GiftCertificate_From%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:AdvTextBox ID="txtFrom" runat="server" ValidationType="Required" />
                        </div>
                    </li>
                    <li>
                        <div class="param-name">
                            <label for="txtSum">
                                <%= Resource.Client_GiftCertificate_Sum%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:AdvTextBox ID="txtSum" runat="server" ValidationType="Money" />
                        </div>
                    </li>
                    <li>
                        <div class="param-name">
                            <label for="txtMessage">
                                <%= Resource.Client_GiftCertificate_Message%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:AdvTextBox ID="txtMessage" CssClassWrap="certificate-message" ValidationType="Required"
                                TextMode="MultiLine" runat="server"></adv:AdvTextBox>
                        </div>
                    </li>
                    <li>
                        <div class="param-name">
                            <label for="txtEmail">
                                <%= Resource.Client_GiftCertificate_RecipientEmail%>:
                            </label>
                        </div>
                        <div class="param-value">
                            <adv:AdvTextBox ID="txtEmail" ValidationType="Email" runat="server"></adv:AdvTextBox>
                        </div>
                    </li>
                    <li>
                        <div class="param-name">
                            <label for="txtEmailFrom">
                                <%= Resource.Client_GiftCertificate_SenderEmail%>:
                            </label>
                        </div>
                        <div class="param-value">
                            <adv:AdvTextBox ID="txtEmailFrom" ValidationType="Email" runat="server"></adv:AdvTextBox>
                        </div>
                    </li>
                    <li>
                        <div class="param-name">
                            <label for="rblPaymentMethods">
                                <%= Resource.Client_GiftCertificate_PaymentMethod%>:
                            </label>
                        </div>
                        <div class="param-value">
                            <asp:ListView ID="lvPaymentMethods" runat="server" ItemPlaceholderID="itemPlaceholderID">
                                <LayoutTemplate>
                                    <table>
                                        <body>
                                            <tr runat="server" id="itemPlaceholderID">
                                            </tr>
                                        </body>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <input id='<%# "rbPaymentMethod_"+ Eval("PaymentMethodID")%>' type="radio" name="rbListPaymentMetods"
                                                value='<%# Eval("PaymentMethodID") %>' data-payment-phone="<%# ((PaymentType)Eval("Type") == PaymentType.QIWI).ToString().ToLower() %>" />
                                        </td>
                                        <td>
                                            <label for='<%# "rbPaymentMethod_"+ Eval("PaymentMethodID")%>'>
                                                <%# Eval("Name")%></label>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:ListView>
                            <asp:HiddenField ID="hfPaymentMethod" runat="server" />
                        </div>
                    </li>
                    <li id="liPhone">
                        <div class="param-name">
                            <label for="txtPhone">
                                <%=Resource.Client_OrderConfirmation_Phone%>:
                            </label>
                        </div>
                        <div class="param-value">
                            <adv:AdvTextBox runat="server" ID="txtPhone" />
                        </div>
                    </li>

                    <li runat="server" id="liCaptcha">
                        <div class="param-name">
                            <label>
                                <%=Resource.Client_Details_Code%>:
                            </label>
                        </div>
                        <div class="param-value">
                            <adv:CaptchaControl ID="validShield" runat="server" />
                        </div>
                    </li>
                    <li>
                        <div class="param-name">
                        </div>
                        <div class="param-value">
                            <adv:Button ID="btnBuyGiftCertificate" Type="Submit" Size="Middle" runat="server"
                                Text="<%$ Resources:Resource, Client_GiftCertificate_Buy %>" OnClick="btnBuyGiftCertificate_Click"></adv:Button>
                            <a href="javascript:void(0)" id="printCert">
                                <%=Resource.Client_GiftCertificate_PreView%></a>
                        </div>
                    </li>
                </ul>
            </div>
            <div class="form-addon">
                <div class="form-addon-text">
                    <p>
                        <%=Resource.Client_GiftCertificate_Limits%>
                        <br />
                        <%=Resource.Client_GiftCertificate_MinimalPrice%>:
                                <%= CatalogService.GetStringPrice(SettingsOrderConfirmation.MinimalPriceCertificate) %><br />
                        <%=Resource.Client_GiftCertificate_MaximumPrice%>:
                                <%= CatalogService.GetStringPrice(SettingsOrderConfirmation.MaximalPriceCertificate) %><br />
                        <%=Resource.Client_GiftCertificate_MinimumOrderPrice%>:
                                <%= CatalogService.GetStringPrice(SettingsOrderConfirmation.MinimalOrderPrice) %><br />
                    </p>
                </div>
            </div>
            <script type="text/javascript">
                function ShowPhone(doShow) {
                    if (doShow == "true") {
                        $('#liPhone').show();
                    } else {
                        $('#liPhone').hide();
                    }
                }

                $("[data-payment-phone]").on("click", function () {
                    ShowPhone($(this).attr("data-payment-phone"));
                });

                $(function () {
                    ShowPhone($("[data-payment-phone] :checked").attr("data-payment-phone"));
                });


            </script>

        </div>
    </div>
    <adv:GiftCertificate ID="GiftCertificate" runat="server" isModal="true" />
</asp:Content>
