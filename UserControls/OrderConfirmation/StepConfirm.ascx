<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepConfirm.ascx.cs" Inherits="UserControls.OrderConfirmation.StepConfirm" %>
<%@ Register TagPrefix="adv" TagName="captchacontrol" Src="~/UserControls/Captcha.ascx" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Customers" %>
<%@ Import Namespace="AdvantShop.Orders" %>
<%@ Import Namespace="Resources" %>


<% if (SettingsOrderConfirmation.DisplayPromoTextbox && ShoppingCartService.CurrentShoppingCart.Coupon == null && ShoppingCartService.CurrentShoppingCart.Certificate == null && CustomerContext.CurrentCustomer.CustomerGroup.CustomerGroupId == CustomerGroupService.DefaultCustomerGroup)
   {%>
<div class="oc-comment oc-block" id="ocCoupon" >
    <div class="order-b-title">
        <%= Resource.Client_ShoppingCart_CouponCode %>
    </div>
    <div class="param-name order-b-content">
        <adv:AdvTextBox ID="txtCertificateCoupon" runat="server" TextMode="Text" CssClassWrap="coupon-txt" DefaultButtonID="btnRegUserConfirm"/> 
        <adv:Button runat="server" ID="btnCouponApply" Type="Confirm" Size="Middle" data-cart-apply-cert-cupon="#txtCertificateCoupon" Text="<%$ Resources:Resource, Client_ShoppingCart_Apply  %>" />
    </div>
</div>
<% }%>



<% if (SettingsOrderConfirmation.IsShowUserComment)
   { %>
<div class="oc-comment oc-block">
    <div class="order-b-title">
        <%= Resource.Client_OrderConfirmation_Comment %>
    </div>
    <div class="param-name">
        <adv:AdvTextBox ID="txtComments" runat="server" CssClassWrap="order-comment" TextMode="MultiLine"
            DefaultButtonID="btnRegUserConfirm"></adv:AdvTextBox>
    </div>
</div>
<% } %>
<%if (SettingsMain.EnableCaptcha)
  {%>
<div id="trCaptcha" class="oc-captcha" runat="server">
    <div class="oc-subtitle">
        <%=Resource.Client_Registration_ConfCode%>
    </div>
    <div class="param-name">
        <adv:captchacontrol ID="dnfValid" runat="server" DefaultButtonID="btnRegUserConfirm" />
    </div>
</div>
<% } %>
