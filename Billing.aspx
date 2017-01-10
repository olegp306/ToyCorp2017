<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Billing.aspx.cs" Inherits="ClientPages.Billing" EnableViewState="false" %>
<%@ Register TagPrefix="adv" TagName="Favicon" Src="~/UserControls/MasterPage/Favicon.ascx" %>
<%@ Register TagPrefix="adv" TagName="MenuTop" Src="~/UserControls/MasterPage/MenuTop.ascx" %>
<%@ Register TagPrefix="adv" TagName="Search" Src="~/UserControls/MasterPage/Search.ascx" %>
<%@ Register TagPrefix="adv" TagName="ShoppingCart" Src="~/UserControls/MasterPage/ShoppingCart.ascx" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<%@ Register TagPrefix="adv" TagName="Logo" Src="~/UserControls/LogoImage.ascx" %>

<%@ Import Namespace="AdvantShop.Design" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Import Namespace="AdvantShop.Payment" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<!DOCTYPE html>
<html style="min-height: 100%">
<head>
    <title><%= PageTitle%></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <meta name="robots" content="noindex" />
    <meta name="robots" content="nofollow" />
    <link rel="stylesheet" type="text/css" href="css/styles.css?p=20151228" />
    <link rel="stylesheet" type="text/css" href="css/styles-extra.css" />
    <link rel="stylesheet" type="text/css" href="css/theme.css">
    <link rel="stylesheet" href="<%= "design/" + DesignService.GetDesign("theme") + "/css/styles.css" %>" id="themecss" />
    <link rel="stylesheet" href="<%= "design/colors/" + DesignService.GetDesign("colorscheme") + "/css/styles.css" %>" id="colorcss" />
    <script type="text/javascript" src="js/jq/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="js/string.format-1.0.js"></script>
    <script type="text/javascript" src="js/jspage/billing/billing.js"></script>
    <script type="text/javascript" src="<%= "js/localization/" + SettingsMain.Language + "/lang.js"%>"></script>
    <adv:Favicon ID="Favicon" runat="server" />
</head>
<body>
    <div class="container">
        <header id="header">
                <adv:Logo ID="Logo" ImgHref='/' runat="server" />
                <div class="center-cell">
                </div>
                <div class="contact-cell">
                    <div class="contact-inside">
                        <div>
                            <div class="phone">
                                <%= AdvantShop.Repository.CityService.GetPhone()%>
                            </div>
                        </div>
                    </div>
                </div>
            </header>

        <div class="stroke">
            <div class="oc-billing">
                <h2><%= PageTitle%></h2>
                <div class="oc-billing-payments">
                    <asp:ListView ID="lvPaymentMethod" runat="server" ItemPlaceholderID="itemPlaceHolder">
                        <LayoutTemplate>
                            <div class="payment-methods">
                                <div runat="server" id="itemPlaceHolder" />
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div class="method-item js-vis-item" data-payment='<%# Eval("PaymentMethodID ") %>' data-type='<%# Eval("Type").ToString().ToLower() %>'>
                                <div class="checkbox">
                                    <input type="radio" name="paymentchk" />
                                </div>
                                <div class="shipping-img">
                                    <img src='<%# PaymentIcons.GetPaymentIcon((PaymentType)Eval("Type"), Eval("IconFileName.PhotoName") as string , Eval("Name").ToString()) %>'
                                        <%# string.Format("alt=\"{0}\" title=\"{0}\"", HttpUtility.HtmlEncode(Eval("Name"))) %> />
                                </div>
                                <div class="method-info">
                                    <div class="method-name">
                                        <%#Eval("Name") %>
                                    </div>
                                    <div class="method-descr">
                                        <%#Eval("Description") %>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <SelectedItemTemplate>
                            <div class="method-item js-vis-item" data-payment='<%# Eval("PaymentMethodID ") %>' data-type='<%# Eval("Type").ToString().ToLower() %>'>
                                <div class="checkbox">
                                    <input type="radio" name="paymentchk" checked="checked" />
                                </div>
                                <div class="shipping-img">
                                    <img src='<%# PaymentIcons.GetPaymentIcon((PaymentType)Eval("Type"), Eval("IconFileName.PhotoName") as string , Eval("Name").ToString()) %>'
                                        <%# string.Format("alt=\"{0}\" title=\"{0}\"", HttpUtility.HtmlEncode(Eval("Name"))) %> />
                                </div>
                                <div class="method-info">
                                    <div class="method-name">
                                        <%#Eval("Name") %>
                                    </div>
                                    <div class="method-descr">
                                        <%#Eval("Description") %>
                                    </div>
                                </div>
                            </div>
                        </SelectedItemTemplate>
                        <EmptyDataTemplate>
                            <div class="payment-methods">
                                <span class="oc-no-way">
                                    <asp:Literal ID="lblNoShipping" runat="server" Text="<%$ Resources:Resource, Client_OrderConfirmation_NoPaymentMethod %>" /></span>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </div>
                <div style="float: left; margin: 5px 0 0 5px">
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
                                        <div>
                                            <%# SQLDataHelper.GetString(Eval("Name")) %>
                                        </div>
                                        <%# !string.IsNullOrEmpty(Convert.ToString(Eval("Color"))) ? SettingsCatalog.ColorsHeader + ": " + Eval("Color") : string.Empty %>
                                        <%# !string.IsNullOrEmpty(Convert.ToString(Eval("Size"))) ? SettingsCatalog.SizesHeader + ": " + Eval("Size") : string.Empty%>
                                        <%# RenderSelectedOptions((IList<EvaluatedCustomOptions>)Eval("SelectedOptions")) %>
                                    </div>

                                    <div class="orderbasket-item-cost">
                                        <%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Amount")), CurrencyCode, CurrencyValue)%>
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

                            <div id="deliveryRow" runat="server" visible="False" class="orderbasket-row">
                                <div class="orderbasket-row-price">
                                    <div class="orderbasket-row-text">
                                        <%= Resource.Client_OrderConfirmation_DeliveryCost%>:
                                    </div>
                                    <div class="orderbasket-row-cost">
                                        <asp:Label ID="lblShippingPrice" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <asp:ListView ID="lvTaxes" runat="server" ItemPlaceholderID="itemPlaceholderID">
                                <LayoutTemplate>
                                    <div runat="server" id="itemPlaceholderID"></div>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <div class="orderbasket-row">
                                        <div class="orderbasket-row-price">
                                            <div class="orderbasket-row-text">
                                                <%# (Convert.ToBoolean(Eval("TaxShowInPrice")) ? Resource.Core_TaxServices_Include_Tax : "") + " " + Eval("TaxName")%>:
                                            </div>
                                            <div class="orderbasket-row-cost">
                                                <%#(Convert.ToBoolean(Eval("TaxShowInPrice")) ? "" : "+") + CatalogService.GetStringPrice(Convert.ToSingle(Eval("TaxSum")), CurrencyValue, CurrencyCode)%>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:ListView>

                            <div id="paymentExtraChargeRow" runat="server" class="orderbasket-row" visible="False">
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
                    </div>
                </div>

                <div class="clear"></div>
                <div class="billing-pay" data-order="<%=OrderId%>">
                    <div id="btnPay"></div>
                </div>
                <div class="billing-form"></div>
            </div>
        </div>
    </div>
    <div id="theme-container">
        <div class="theme-left">
        </div>
        <div class="theme-right">
        </div>
    </div>
</body>
</html>
