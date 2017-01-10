<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepBasket.ascx.cs" Inherits="UserControls.OrderConfirmation.StepBasket" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Import Namespace="Resources" %>
<%@ Register TagPrefix="adv" TagName="BuyInOneClick" Src="~/UserControls/BuyInOneClick.ascx" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>

<div class="orderbasket">
    <div class="orderbasket-row">
        <div class="orderbasket-header"><%= Resource.Client_OrderConfirmation_BasketHeader%></div>
    </div>
    <asp:ListView ID="lvOrderList" runat="server" ItemPlaceholderID="itemPlaceHolder">
        <LayoutTemplate>
            <div class="orderbasket-items">
                <div id="itemPlaceHolder" runat="server"></div>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <div class="orderbasket-item">
                <div class="orderbasket-item-info">
                    <div><a href="<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("Offer.Product.UrlPath")), SQLDataHelper.GetInt( Eval("Offer.ProductId"))) %>" target="_blank">
                        <%# SQLDataHelper.GetString(Eval("Offer.Product.Name")) %></a></div>
                    <div class="amount"><%= Resource.Client_Details_Amount %>: <%# SQLDataHelper.GetString(Eval("Amount")) + " " + SQLDataHelper.GetString(Eval("Offer.Product.Unit")) %></div>
                    <%# Eval("Offer.Color") != null ? SettingsCatalog.ColorsHeader + ": " + SQLDataHelper.GetString(Eval("Offer.Color.ColorName")) + "<br />" : "" %>
                    <%# Eval("Offer.Size") != null ? SettingsCatalog.SizesHeader + ": " + SQLDataHelper.GetString(Eval("Offer.Size.SizeName")) + "<br />" : ""%>
                    <%# CatalogService.RenderSelectedOptions(SQLDataHelper.GetString(Eval("AttributesXml")))%>
                </div>

                <div class="orderbasket-item-cost">
                    <%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Price")), 0F, SQLDataHelper.GetFloat(Eval("Amount")))%>
                </div>
            </div>
        </ItemTemplate>
    </asp:ListView>
    
    <div class="js-oc-summary">

        <div class="orderbasket-row">
            <div class="orderbasket-row-price">
                <div class="orderbasket-row-text">
                    <%= Resource.Client_OrderConfirmation_OrderCost %>:
                </div>
                <div class="orderbasket-row-cost">
                    <asp:Label ID="lblProductsPrice" runat="server" />
                </div>
            </div>
        </div>

        <div id="certificateRow" runat="server" visible="false" class="orderbasket-row">
            <div class="orderbasket-row-price">
                <div class="orderbasket-row-text">
                    <%= Resource.Client_OrderConfirmation_Certificate%>:
                </div>
                <div class="orderbasket-row-cost">
                    <asp:Label ID="lblCertificatePrice" runat="server" />
                </div>
            </div>
        </div>

        <div id="couponRow" runat="server" visible="false" class="orderbasket-row">
            <div class="orderbasket-row-price">
                <div class="orderbasket-row-text">
                    <%= Resource.Client_OrderConfirmation_Coupon%>:
                </div>
                <div class="orderbasket-row-cost">
                    <asp:Label ID="lblCouponPrice" runat="server" />
                </div>
            </div>
        </div>

        <div id="discountRow" runat="server" visible="false" class="orderbasket-row">
            <div class="orderbasket-row-price">
                <div class="orderbasket-row-text">
                    <%= Resource.Client_OrderConfirmation_Discount %> (<asp:Literal runat="server" ID="liDiscountPercent" />):
                </div>
                <div class="orderbasket-row-cost">
                    <asp:Label ID="lblOrderDiscount" runat="server" />
                </div>
            </div>
        </div>
        
        <div id="bonusesRow" runat="server" visible="false" class="orderbasket-row">
            <div class="orderbasket-row-price">
                <div class="orderbasket-row-text">
                    <%= Resource.Client_OrderConfirmation_Bonuses%>:
                </div>
                <div class="orderbasket-row-cost">
                    <asp:Label ID="lblOrderBonuses" runat="server" />
                </div>
            </div>
        </div>

        <div id="deliveryRow" runat="server" class="orderbasket-row">
            <div class="orderbasket-row-price">
                <div class="orderbasket-row-text">
                    <%= Resource.Client_OrderConfirmation_DeliveryCost%>:
                </div>
                <div class="orderbasket-row-cost">
                    <asp:Label ID="lblShippingPrice" runat="server" />
                </div>
            </div>
        </div>
        <asp:Literal ID="literalTaxCost" runat="server"></asp:Literal>
        <div id="paymentExtraChargeRow" runat="server" class="orderbasket-row">
            <div class="orderbasket-row-price">
                <div class="orderbasket-row-text">
                    <asp:Literal runat="server" ID="lPaymentCost" />:
                </div>
                <div class="orderbasket-row-cost">
                    <asp:Label ID="lblPaymentExtraCharge" runat="server" />
                </div>
            </div>
        </div>
    </div>

    <div class="orderbasket-row-ex">
        <div class="orderbasket-result">
            <div class="orderbasket-result-text">
                <%= Resource.Client_OrderConfirmation_Total%>:
            </div>
            <div class="orderbasket-result-cost total-price">
                <asp:Label ID="lblTotalPrice" CssClass="js-oc-total-sum-basket" runat="server" />
            </div>
        </div>
    </div>
    <div id="bonusPlus" runat="server" Visible="False" class="orderbasket-row">
        <div class="orderbasket-row-price">
            <div class="orderbasket-row-text">
                <%= Resource.Client_OrderConfirmation_BonusPlusBasket%>:
            </div>
            <div class="orderbasket-row-cost">
                <asp:Label ID="liBonusPlus" runat="server" />
            </div>
        </div>
    </div>
    <div class="orderbasket-row">
        <a href="shoppingcart.aspx" class="orderbasket-cart-link"><%= Resource.Client_OrderConfirmation_GoToCart%></a>
    </div>
    <div class="orderbasket-row">
        <div class="orderbasket-buy-one-click">
            <adv:BuyInOneClick ID="buyInOneClick" runat="server" />
            <div class="orderbasket-buy-one-click-text">
                <adv:StaticBlock runat="server" ID="sbStepBasketBuyOneClick" SourceKey="orderBasketBuyOneClick" />
            </div>
        </div>
    </div>
</div>
