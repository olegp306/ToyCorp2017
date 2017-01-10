<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" CodeFile="CompareProducts.aspx.cs"
    AutoEventWireup="true" Inherits="ClientPages.CompareProducts" EnableViewState="false" %>

<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">

    <div class="compareproduct-container compareproduct-container-processing js-compareproduct-container" <%= CurrentProducts.Count == 0 ? "style=\"display:none;\"" : "" %>>
        <div class="clearfix">
            <div class="compareproduct-products-col">
                <div class="compareproduct-products-wrapper">
                    <ul class="compareproduct-product js-compareproduct-block js-compareproduct-block-products">
                        <li class="compareproduct-product-row js-compareproduct-block-row" data-row-index="0">
                            <asp:ListView runat="server" ID="lvProducts">
                                <ItemTemplate>
                                    <div class="compareproduct-product-item js-compareproduct-product-item" data-compare-product-id="<%# Eval("Offer.ProductId")%>">
                                        <div class="compareproduct-product-item-wrap">
                                            <div class="compareproduct-product-name">
                                                <a href="<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("Offer.Product.UrlPath")), SQLDataHelper.GetInt(Eval("Offer.ProductID"))) %>" class="compareproduct-product-name-link"><%# Eval("Offer.Product.Name") %></a>
                                            </div>
                                            <div class="compareproduct-product-sku"><%# Eval("Offer.ArtNo") %></div>
                                            <figure class="compareproduct-product-pic-wrap" style="height: <%= ImageSmallHeight%>px">
                                                <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("Offer.ProductId")), SQLDataHelper.GetString(Eval("Offer.Photo.PhotoName")), SQLDataHelper.GetString(Eval("Offer.Product.UrlPath")))%>
                                            </figure>
                                            <div class="price-container">
                                                <div class="price">
                                                    <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("Offer.ProductId")), SQLDataHelper.GetFloat(Eval("Offer.Price")), SQLDataHelper.GetFloat(Eval("Offer.Product.Discount")))%>
                                                </div>
                                            </div>

                                            <div class="compareproduct-product-btns">
                                                <%--                                                <adv:Button data-cart-add-productid='<%# Eval("Offer.ProductID") %>' data-cart-amount='<%# Eval("Offer.Product.MinAmount") %>' runat="server" Size="XSmall" Type="Buy"
                                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("Offer.Product.UrlPath").ToString(),SQLDataHelper.GetInt(Eval("Offer.ProductID"))) %>'
                                                    Text='<%# BuyButtonText %>' Visible='<%# SQLDataHelper.GetFloat(Eval("Offer.Price")) > 0 && SQLDataHelper.GetFloat(Eval("Offer.Amount")) > 0 %>' />--%>

                                                <adv:Button data-cart-add-productid='<%# Eval("Offer.ProductId") %>' data-cart-amount='<%# Eval("Offer.Product.MinAmount") %>'
                                                    runat="server" Type="Add" Size="Small" Text='<%# BuyButtonText %>'
                                                    Href='<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("Offer.Product.UrlPath")), Convert.ToInt32(Eval("Offer.ProductId"))) %>'
                                                    Visible='<%# DisplayBuyButton && SQLDataHelper.GetDecimal(Eval("Price")) > 0 && SQLDataHelper.GetDecimal(Eval("Offer.Amount")) > 0 %>'
                                                    data-color-id='<%# Eval("Offer.ColorID") %>' />
                                                <adv:Button runat="server" Type="Action" Size="Small" Text='<%# PreOrderButtonText %>'
                                                    Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("OfferId") %>'
                                                    Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetInt(Eval("Offer.Amount")) <= 0 || SQLDataHelper.GetFloat(Eval("Price")) == 0)  && SQLDataHelper.GetBoolean(Eval("Offer.Product.AllowPreorder")) %>' />
                                            </div>
                                            <a href="javascript:void(0);" class="compareproduct-product-remove" data-compare-product-id="<%# Eval("Offer.ProductId")%>" data-cart-remove="<%# Eval("ShoppingCartItemId") %>">Удалить</a>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:ListView>
                        </li>
                        <% int index = 1; %>
                        <% foreach (var prop in CurrentProperties)
                           { %>
                        <li class="compareproduct-product-row js-compareproduct-block-row" data-row-index="<%= index++ %>">
                            <% foreach (var product in CurrentProducts)
                               { %>
                            <div class="compareproduct-product-item js-compareproduct-product-item" data-compare-product-id="<%= product.ProductId%>">
                                <%= product.ProductPropertyValues.Where(p=> p.PropertyId == prop.PropertyId).Select(p=>p.Value).AggregateString(',')  %>
                            </div>
                            <% } %>
                        </li>
                        <% } %>
                    </ul>
                </div>
            </div>
            <div class="compareproduct-properties-col">
                <asp:ListView runat="server" ID="lvProperties" ItemPlaceholderID="liProperty">
                    <LayoutTemplate>
                        <ul class="compareproduct-properties js-compareproduct-block js-compareproduct-block-properties">
                            <li class="compareproduct-properties-row js-compareproduct-block-row" data-row-index="0">
                                <div class="compareproduct-properties-item">
                                </div>
                            </li>
                            <li runat="server" id="liProperty" />
                        </ul>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <li class="compareproduct-properties-row js-compareproduct-block-row" data-row-index="<%# Container.DataItemIndex+1 %>">
                            <div class="compareproduct-properties-item">
                                <%# Eval("Name") %>
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:ListView>
            </div>
        </div>
    </div>

    <div class="js-compareproduct-empty compareproduct-empty" <%= CurrentProducts.Count == 0 ? "style=\"display:block;\"" : "" %>>
        <%= Resources.Resource.Client_CompareProducts_Empty %>
    </div>

</asp:Content>
