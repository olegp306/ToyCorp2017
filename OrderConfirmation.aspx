<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="OrderConfirmation.aspx.cs" Inherits="ClientPages.OrderConfirmation" %>

<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>

<%@ Register TagPrefix="adv" TagName="Address" Src="UserControls/OrderConfirmation/StepAddress.ascx" %>
<%@ Register TagPrefix="adv" TagName="Shipping" Src="UserControls/OrderConfirmation/StepShipping.ascx" %>
<%@ Register TagPrefix="adv" TagName="Payment" Src="UserControls/OrderConfirmation/StepPayment.ascx" %>
<%@ Register TagPrefix="adv" TagName="Confirm" Src="UserControls/OrderConfirmation/StepConfirm.ascx" %>
<%@ Register TagPrefix="adv" TagName="Success" Src="UserControls/OrderConfirmation/StepSuccess.ascx" %>
<%@ Register TagPrefix="adv" TagName="Basket" Src="UserControls/OrderConfirmation/StepBasket.ascx" %>
<%@ Register TagPrefix="adv" TagName="Bonuses" Src="UserControls/OrderConfirmation/StepBonus.ascx" %>
<%@ Register TagPrefix="adv" TagName="BreadCrumbs" Src="~/UserControls/BreadCrumbs.ascx" %>

<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <% if (!string.Equals(Request["tab"], "FinalTab")){ %>
            <div class="full-cart-wrap">
                <h1 class="mainContantTitle">
                    <%= Resources.Resource.Client_ShoppingCart_ShoppingCart %></h1>
                <div class="cartSection">
                    <asp:Panel ID="pnlTopContent" runat="server">
                    </asp:Panel>
                    <asp:Literal ID="ltrlTopContent" runat="server"></asp:Literal>
                    <div id="cartWrapper" class="cart-wrapper">
                        <div id="dvOrderMerged" runat="server" visible="false" class="ShoppingCart_MergedOrder">
                            <asp:Localize ID="Localize_Client_ShoppingCart_ProductsInBasket" runat="server" Text="<%$ Resources:Resource, Client_ShoppingCart_ProductsInBasket %>"></asp:Localize>
                        </div>
                        <div data-plugin="cart">
                        </div>
                    </div>

                    <adv:StaticBlock runat="server" SourceKey="shoppingcart" />
                    <asp:Panel ID="pnlBottomContent" runat="server">
                    </asp:Panel>
                    <asp:Literal ID="ltrlBottomContent" runat="server"></asp:Literal>
                    <%--<asp:Label ID="lDemoWarning" runat="server" CssClass="warn" Text="<%$ Resources:Resource, Client_ShoppingCart_FakeShop %>" />--%>
                </div>
            </div>
            <%} %>
        </div>
    </div>
    
    
    
    
    

    <asp:Literal runat="server" ID="ltGaECommerce" />
    <h1 class="mainContantTitle">
        <%=Resource.Client_OrderConfirmation_OrderConfirmation%></h1>
    <adv:BreadCrumbs ID="breadCrumbs" runat="server" />
    </div>
    <div class="oc-wrapper of-zakaz">
        <asp:MultiView ID="mvOrderConfirm" runat="server" ActiveViewIndex="0">
            <asp:View ID="ViewCheckout" runat="server">
                <div class="oc-content">
                    <div class="tabsWrapper">
                        <%--<div class="tabsHeader">
                            <div class="container ">
                                <div class="tabItem active"><a href="#">Контактные данные</a></div>
                                <div class="tabItem two-tab"><a href="#">Доставка</a></div>
                                <div class="tabItem three-tab"><a href="#">Оплата</a></div>
                            </div>
                        </div>--%>
                        <div class="tabsContent">
                            <div class="tabsContentItem activeTab">
                                <div class="container">
                                    <adv:Address ID="Address" runat="server" />
                                    <%--<a href="#" style="margin-left: 208px;" class="btn btn-confirm btn-big greenButton" onclick="return switchTab('two-tab');">ШАГ 2</a>--%>
                                </div>
                            </div>
                            <div class="tabsContentItem activeTab">
                                <div class="container">
                                    <adv:Shipping ID="Shipping" runat="server" />
                                    <%--<a href="#" style="margin-left: 208px;" class="btn btn-confirm btn-big greenButton" onclick="return switchTab('three-tab');">ШАГ 3</a>--%>
                                </div>
                            </div>
                            <div class="tabsContentItem activeTab">
                                <div class="container">
                                    <adv:Payment ID="Payment" runat="server" />
                                    <div style="margin-left: 208px;">
                                        <div class="oc-total-price-bottom">
                                            <%= Resource.Client_OrderConfirmation_SumText%>:
                                            <asp:Label ID="lblTotalPrice" runat="server" CssClass="js-oc-total-sum-bottom oc-total-sum-bottom-price" />
                                        </div>
                                        <% if (SettingsOrderConfirmation.IsShowUserAgreementText)
                                            { %>
                                        <div class="oc-agreement">
                                            <input type="checkbox" runat="server" id="chkAgree" class="valid-required" />
                                            <label for="chkAgree"><%= SettingsOrderConfirmation.UserAgreementText%></label>
                                        </div>
                                        <% } %>
                                        <div class="oc-panel-wr">
                                            <adv:Button ID="btnConfirm" runat="server" Size="Big" Type="Confirm" CssClass="btn-continue greenButton"
                                                Text="Купить" OnClick="btnConfirm_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="container">
                                <div class="oc-info" style="margin-left: 208px;">
                                    Если у Вас возникли сложности с регистрацией заказа, то Вы можете оформить заказ по телефону: (383) 375-76-80.
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="container">
                        <%--<adv:Address ID="Address" runat="server" />
                    <adv:Shipping ID="Shipping" runat="server" />
                    <adv:Payment ID="Payment" runat="server" />--%>
                        <adv:Bonuses ID="Bonuses" runat="server" />
                        <adv:Confirm ID="Confirm" runat="server" />
                                
                        <div id="bonusplusbottom" runat="server" class="oc-bonusplus-bottom">
                            <%= Resource.Client_OrderConfirmation_BonusPlus%>: +<asp:Label runat="server" ID="lblBonusPlus" CssClass="oc-bonusplus-price-bottom" />
                        </div>
                    </div>
                    <div class="oc-cart-data" style="display: none;">
                        <div id="orderBasketBlock">
                            <adv:Basket ID="Basket" runat="server" />
                        </div>
                    </div>
                    <div class="clear"></div>
            </asp:View>
            <asp:View ID="ViewOrderConfirmationFinal" runat="server">
                <adv:Success ID="StepSuccess" runat="server" />
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
