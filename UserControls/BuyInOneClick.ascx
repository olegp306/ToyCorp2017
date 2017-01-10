<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BuyInOneClick.ascx.cs"
    Inherits="UserControls.BuyInOneClick" EnableViewState="false" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="AdvantShop.Orders" %>
<div id="divBuyInOneClick" class="<%= PageEnum == BuyInOneclickPage.details || PageEnum == BuyInOneclickPage.orderconfirmation ? "buy-one-click-wrap" : "buy-one-click-wrap-alt" %>">
    <a id="lBtnBuyInOneClick" class="<%= PageEnum == BuyInOneclickPage.details || PageEnum == BuyInOneclickPage.orderconfirmation ? "btn-buy-one-click" : "btn btn-add btn-big" %>"
        href="javascript:void(0)" data-page="<%= PageEnum.ToString() %>" data-buyoneclick-offerid="<%= OfferID %>">
        <%= Resource.Client_BuyInOneclick_Button %></a>
</div>
<div style="display: none;">
    <div id="modalBuyInOneClick" class="modal-buy-in-one">
        <asp:HiddenField ID="hfProductId" runat="server" />
        <ul class="form form-vr" id="modalBuyInOneClickForm">
            <li>
                <p class="headtext">
                    <%= SettingsOrderConfirmation.BuyInOneClickFirstText%></p>
            </li>
            <% if (SettingsOrderConfirmation.IsShowBuyInOneClickName) { %>
            <li>
                <div class="param-name one-click-label">
                    <label for="txtBuyOneClickName">
                        <%= SettingsOrderConfirmation.BuyInOneClickName %></label></div>
                <div class="param-value">
                    <adv:AdvTextBox ID="txtBuyOneClickName" CssClassWrap="one-click-input-wrap" 
                                    runat="server" ValidationGroup="buyInOneClick" DefaultButtonID="btnAddChangeContact"/>
                </div>
            </li>
            <% } if (SettingsOrderConfirmation.IsShowBuyInOneClickEmail) { %>
            <li>
                <div class="param-name one-click-label">
                    <label for="txtBuyOneClickEmail">
                        <%=SettingsOrderConfirmation.BuyInOneClickEmail%></label></div>
                <div class="param-value">
                    <adv:AdvTextBox ID="txtBuyOneClickEmail" CssClassWrap="one-click-input-wrap" 
                                    runat="server" ValidationGroup="buyInOneClick" DefaultButtonID="btnAddChangeContact"/>
                </div>
            </li>
            <% } if (SettingsOrderConfirmation.IsShowBuyInOneClickPhone) { %>
            <li>
                <div class="param-name one-click-label">
                    <label for="txtBuyOneClickPhone">
                        <%=SettingsOrderConfirmation.BuyInOneClickPhone%></label></div>
                <div class="param-value">
                    <adv:AdvTextBox ID="txtBuyOneClickPhone" CssClassWrap="one-click-input-wrap" 
                        runat="server" ValidationGroup="buyInOneClick" DefaultButtonID="btnAddChangeContact"/>
                </div>
            </li>
            <% } if (SettingsOrderConfirmation.IsShowBuyInOneClickComment) { %>
            <li>
                <div class="param-name one-click-label">
                    <label for="txtBuyOneClickComment">
                        <%= SettingsOrderConfirmation.BuyInOneClickComment %></label></div>
                <div class="param-value">
                    <adv:AdvTextBox ID="txtBuyOneClickComment" CssClassWrap="one-click-textarea-wrap" TextMode="Multiline"
                        runat="server" ValidationGroup="buyInOneClick"/>
                </div>
            </li>
            <% } %>
            <li>
                <span style="color:red" id="errorOneClick"></span>
            </li>
            <li class="form-footer">
                <div class="param-name one-click-label">
                </div>
                <div class="param-value">
                    <adv:Button ID="btnBuyInOneClick" Size="Big" Type="Submit" CssClass="btn-save-pass-my"
                        runat="server" Text=' <%$Resources: Resource,Client_BuyInOneClick_WaitForTheCall%>'
                        ValidationGroup="buyInOneClick" />
                </div>
            </li>
        </ul>
        <div style="display: none;" id="modalBuyInOneClickFinal">
            <div class="finaltext">
                <%= SettingsOrderConfirmation.BuyInOneClickFinalText %></div>
            <div class="btn-final-one-click-wrap">
                <adv:Button ID="btnBuyInOneClickOk" Size="Big" Type="Submit" runat="server" Text=' Ok'
                    ValidationGroup="buyInOneClick" />
            </div>
        </div>
    </div>
</div>