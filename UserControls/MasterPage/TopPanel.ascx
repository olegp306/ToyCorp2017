<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TopPanel.ascx.cs" Inherits="UserControls.MasterPage.TopPanel" %>
<%@ Import Namespace="Resources" %>

<div class="topSidebarCity">
    <div class="top-panel-item stretch-item" id="pnlCity" runat="server">
        <%= Resource.Client_MasterPage_LocationYourCity %>: <span class="js-location-call js-change-city-name js-location-replacement js-bubble-town top-panel-change-city-name" data-location-mask="#city#"><%= CurrentTown %></span>
    </div>
</div>
<div class="top-panel-currency top-panel-item stretch-item" runat="server" id="pnlCurrency">
    <asp:DropDownList ID="ddlCurrency" runat="server" onchange="ChangeCurrency(this);" />
</div>
<div class="topSidebarPhone">
    <div class="phone js-phone js-location-replacement" data-location-mask="#phone#" <%= ""/*InplaceEditor.Phone.Attribute() */%>>
        <%= AdvantShop.Repository.CityService.GetPhone()%>
    </div>
</div>
<div class="topSideSignIn">
    <div class="top-panel-login top-panel-item stretch-item" id="pnlLogin">
        <a runat="server" id="aLogin" rel="nofollow" class="tpl tpl-signin"><%= Resource.Client_MasterPage_Authorization %></a>
        <a runat="server" id="aMyAccount" class="tpl tpl-signin"><%= Resource.Client_MasterPage_MyAccount %></a>
        <%--Admin link --%>
        <a id="pnlAdmin" runat="server" class="tpl tpl-admin"><%= Resource.Client_MasterPage_Administration%></a>
        <a id="aCreateTrial" href="javascript:void(0);" runat="server" class="tpl tpl-admin trialAdmin">
            <%= Resource.Client_MasterPage_Administration%></a>
        <%--/Admin link --%>
        <a runat="server" id="aRegister" rel="nofollow" class="tpl tpl-reg"><%= Resource.Client_MasterPage_Registration %></a>
        <asp:LinkButton ID="lbLogOut" CssClass="tpl tpl-reg" runat="server"
            OnClick="btnLogout_Click"><%= Resource.Client_MasterPage_LogOut %></asp:LinkButton>
    </div>
</div>
<div class="top-panel-wishlist top-panel-item stretch-item" runat="server" id="pnlWishList">
    <%= Resource.Client_MasterPage_WishList%>: <a href="wishlist.aspx" class="wishlist-head-link"><%= WishlistCount %></a>
</div>


