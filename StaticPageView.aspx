<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeFile="StaticPageView.aspx.cs" Inherits="ClientPages.StaticPageView" %>

<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="AdvantShop.CMS" %>

<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>

<%@ Register TagPrefix="adv" TagName="BreadCrumbs" Src="~/UserControls/BreadCrumbs.ascx" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<%@ Register Src="~/UserControls/Social.ascx" TagPrefix="adv" TagName="Social" %>

<%@ Register TagPrefix="adv" TagName="Rating" Src="~/UserControls/Rating.ascx" %>
<%@ Register TagPrefix="adv" TagName="SizeColorPickerCatalog" Src="~/UserControls/Catalog/SizeColorPickerCatalog.ascx" %>


<asp:Content runat="server" ID="contentHead" ContentPlaceHolderID="ContentPlaceHolderHeader">
    <script type="text/javascript" src="//vk.com/js/api/share.js?11" charset="windows-1251"></script>
    <script type="text/javascript">
        (function () {
            var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
            po.src = 'https://apis.google.com/js/plusone.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
        })();
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <h1 class="mainContantTitle" <%= InplaceEditor.Meta.Attribute(AdvantShop.SEO.MetaType.StaticPage, page.ID) %>>
            <%= metaInfo.H1 %></h1>
        <adv:BreadCrumbs runat="server" ID="ucBreadCrumbs" />

        <div class="overfl gray-bg">
            <div class="<%= hasSubPages ? "left-thin" : "content-owner" %>">

                <div class="news-descr" <%= InplaceEditor.StaticPage.Attribute(page.ID, InplaceEditor.StaticPage.Field.PageText) %>>
                    <%= page.PageText %>
                </div>
            </div>
            <div class="right-slim" runat="server" id="rightBlock">
                <div class="block-static">
                    <div class="title upper">
                        <%= page.PageName %>
                    </div>
                    <asp:ListView ID="lvSubPages" runat="server">
                        <LayoutTemplate>
                            <div class="content">
                                <ul class="list-news-cat">
                                    <li runat="server" id="itemPlaceHolder"></li>
                                </ul>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <li><a href="<%# UrlService.GetLink(ParamType.StaticPage, Eval("UrlPath").ToString(), Eval("id").ToString().TryParseInt()) %>">
                                <%#Eval("PageName")%></a></li>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>
        </div>
    </div>

    <div>
        <%--footter--%>
        <div class="specialGoods tabsWrapper">
    <div class="tabsHeader">
        <div class="container">
            <div class="specialGoodsTab tabItem active"><a href="#" class="newest">Новинки</a></div>
            <div class="specialGoodsTab tabItem"><a href="#" class="bestPrice">Лучшая цена</a></div>
            <div class="specialGoodsTab tabItem"><a href="#" class="popular">Популярный товар</a></div>
            <div class="specialGoodsTab tabItem"><a href="#" class="actions">Акции</a></div>
            <div class="specialGoodsTab tabItem"><a href="#" class="recomended">Распродажа</a></div>
        </div>
    </div>
        <asp:MultiView runat="server" ID="mvMainPageProduct">
            <Views>
                <asp:View runat="server" ID="viewDefault">
                    <div class="tabsContent">
                        <div class="container">
                            <%--Новинки--%>
                            <div class="tabsContentItem activeTab">
                                <div class="block-best" runat="server" id="pnlNew">
                                    <div class="pv-tile scp">
                                        <asp:ListView runat="server" ID="lvNew" ItemPlaceholderID="liItemPlaceholder">
                                            <ItemTemplate>
                                                <div class="pv-item scp-item" data-productid="<%# Eval("productId") %>" style="width: <%= ImageMaxWidth%>px" data-display-previews='<%= enablePhotoPreviews %>'>
                                                    <table class="p-table">
                                                        <tr>
                                                            <td class="img-middle" style="height: <%= ImageMaxHeight %>px">
                                                                <div class="goodItemThumb">
                                                                    <div class="pv-photo">
                                                                        <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetInt(Eval("PhotoId")))%>
                                                                    </div>
                                                                    <div class="goodItemInCart">
                                                                        <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add"
                                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                                            Text='<%#BuyButtonText  %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                                    </div>
                                                                </div>
                                                                <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div class="goodItemDesc">
                                                                    <div class="pv-div-link">
                                                                        <a href="<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                                            class="link-pv-name">
                                                                            <%# Eval("Name") %></a>
                                                                        <div class="pv-div-link-gradient"></div>
                                                                    </div>
                                                                    <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%# ColorImageWidth %>" />
                                                                    <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                                        ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                                        ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                                    <div class="price-container">
                                                                        <div class="price">
                                                                            <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </div>
                                </div>
                                <div class="new-title">
                                    <%-- <%= Resources.Resource.Client_Default_New %>--%>
                                    <a href="productlist.aspx?type=new">
                                        <%= Resources.Resource.Client_Default_AllNew%></a>
                                </div>
                            </div>
                            <%-- Блок товаров с лучшей ценой--%>
                            <div class="tabsContentItem">
                                <div class="block-best" runat="server" id="pnlDiscount">
                                    <div class="pv-tile scp">
                                        <asp:ListView runat="server" ID="lvDiscount" ItemPlaceholderID="liItemPlaceholder">
                                            <ItemTemplate>
                                                <div class="pv-item scp-item" data-productid="<%# Eval("productId") %>" style="width: <%= ImageMaxWidth%>px" data-display-previews='<%= enablePhotoPreviews %>'>
                                                    <table class="p-table">
                                                        <tr>
                                                            <td class="img-middle" style="height: <%= ImageMaxHeight %>px">
                                                                <div class="goodItemThumb">
                                                                    <div class="pv-photo">
                                                                        <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetInt(Eval("PhotoId")))%>
                                                                    </div>
                                                                    <div class="goodItemInCart">
                                                                        <adv:Button ID="Button1" data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add"
                                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                                            Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                                    </div>
                                                                </div>
                                                                <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div class="goodItemDesc">
                                                                    <div class="pv-div-link">
                                                                        <a href="<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                                            class="link-pv-name">
                                                                            <%# Eval("Name") %></a>
                                                                        <div class="pv-div-link-gradient"></div>
                                                                    </div>
                                                                    <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%# ColorImageWidth %>" />
                                                                    <adv:Rating ID="Rating1" runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                                        ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                                        ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                                    <div class="price-container">
                                                                        <div class="price">
                                                                            <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                                        </div>
                                                                    </div>

                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </div>
                                </div>
                                <div class="best-title">
                                    <%-- <%= Resources.Resource.Client_Default_BestSellers %>--%>
                                    <a href="productlist.aspx?type=onsale">
                                        <%= Resources.Resource.Client_Default_OnSale %></a>
                                </div>
                            </div>
                            <%--ПОПУЛЯРНЫЙ ТОВАР--%>

                            <div class="tabsContentItem">

                                <div class="block-best" runat="server" id="pnlBest">
                                    <div class="pv-tile scp">
                                        <asp:ListView runat="server" ID="lvBestSellers" ItemPlaceholderID="liItemPlaceholder">
                                            <ItemTemplate>
                                                <div class="pv-item scp-item" data-productid="<%# Eval("productId") %>" style="width: <%= ImageMaxWidth%>px" data-display-previews='<%= enablePhotoPreviews %>'>
                                                    <table class="p-table">
                                                        <tr>
                                                            <td class="img-middle" style="height: <%= ImageMaxHeight %>px">
                                                                <div class="goodItemThumb">
                                                                    <div class="pv-photo">
                                                                        <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetInt(Eval("PhotoId")))%>
                                                                    </div>
                                                                    <div class="goodItemInCart">
                                                                        <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add"
                                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                                            Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                                    </div>
                                                                </div>
                                                                <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div class="goodItemDesc">
                                                                    <div class="pv-div-link">
                                                                        <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                                            class="link-pv-name">
                                                                            <%# Eval("Name") %></a>
                                                                        <div class="pv-div-link-gradient"></div>
                                                                    </div>
                                                                    <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%# ColorImageWidth %>" />
                                                                    <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                                        ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                                        ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                                    <div class="price-container">
                                                                        <div class="price">
                                                                            <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </div>
                                </div>
                                <div class="best-title">
                                    <%-- <%= Resources.Resource.Client_Default_BestSellers %>--%>
                                    <a href="productlist.aspx?type=bestseller">
                                        <%= Resources.Resource.Client_Default_AllBestSellers %></a>
                                </div>
                            </div>
                            <%--АКЦИИ--%>
                            <div class="tabsContentItem">
                                <adv:StaticBlock ID="StaticBlock6" runat="server" SourceKey="promotionsOnMain" />
                            </div>

                            <%--РАСПРОДАЖА--%>
                            <div class="tabsContentItem">

                                <div class="block-recomended" runat="server" id="pnlRecomended">
                                    <div class="pv-tile scp">
                                        <asp:ListView runat="server" ID="lvRecomended" ItemPlaceholderID="liItemPlaceholder">
                                            <ItemTemplate>
                                                <div class="pv-item scp-item" data-productid="<%# Eval("productId") %>" style="width: <%= ImageMaxWidth%>px" data-display-previews='<%= enablePhotoPreviews %>'>
                                                    <table class="p-table">
                                                        <tr>
                                                            <td class="img-middle" style="height: <%= ImageMaxHeight %>px">
                                                                <div class="goodItemThumb">
                                                                    <div class="pv-photo">
                                                                        <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetInt(Eval("PhotoId")))%>
                                                                    </div>
                                                                    <div class="goodItemInCart">
                                                                        <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add"
                                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                                            Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                                    </div>
                                                                </div>
                                                                <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <div class="goodItemDesc">
                                                                    <div class="pv-div-link">
                                                                        <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                                            class="link-pv-name">
                                                                            <%# Eval("Name") %></a>
                                                                        <div class="pv-div-link-gradient"></div>
                                                                    </div>
                                                                    <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%# ColorImageWidth %>" />
                                                                    <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                                        ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                                        ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                                    <div class="price-container">
                                                                        <div class="price">
                                                                            <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </div>
                                </div>
                                <div class="best-title">
                                    <a href="productlist.aspx?type=recomended">
                                        <%= Resources.Resource.Client_Default_Recomended %></a>
                                </div>
                            </div>
                        </div>

                    </div>
                </asp:View>
                <asp:View runat="server" ID="viewAlternative">
                    <table class="container-special default-mode">
                        <tr>
                            <td class="block" runat="server" id="liBestsellers">
                                <div class="best-title">
                                    <%= Resources.Resource.Client_Default_BestSellers %>
                                    <a href="productlist.aspx?type=bestseller">
                                        <%= Resources.Resource.Client_Default_AllBestSellers %></a>
                                </div>
                                <asp:ListView runat="server" ID="lvBestSellersAlternative" ItemPlaceholderID="liItemPlaceholder">
                                    <LayoutTemplate>
                                        <ul class="p-list scp">
                                            <li runat="server" id="liItemPlaceholder"></li>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li class="p-block scp-item" data-productid="<%# Eval("productId")%>" data-display-previews='<%= enablePhotoPreviews %>'>
                                            <table class="p-table">
                                                <tr>
                                                    <td class="img-middle" style="height: <%= ImageMaxHeight%>px">
                                                        <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetInt(Eval("PhotoId")))%>
                                                        <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div class="pv-div-link">
                                                            <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                                class="link-pv-name">
                                                                <%# Eval("Name") %></a>
                                                            <%# string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) %>
                                                        </div>
                                                        <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%# ColorImageWidth %>" />
                                                        <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                            ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                            ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                        <div class="price-container">
                                                            <div class="price">
                                                                <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                            </div>
                                                        </div>
                                                        <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="XSmall" Type="Add"
                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                            Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                        <adv:Button runat="server" Type="Action" Size="XSmall" Text='<%# PreOrderButtonText %>'
                                                            Href='<%# "sendrequestonproduct.aspx?OfferID=" + Eval("OfferID") %>'
                                                            Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetInt(Eval("Amount")) <= 0 || SQLDataHelper.GetFloat(Eval("Price")) == 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                                        <adv:Button runat="server" Size="XSmall" Type="Buy" Text='<%# MoreButtonText %>'
                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                            Visible="<%# DisplayMoreButton %>" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </li>
                                    </ItemTemplate>
                                </asp:ListView>
                            </td>
                            <td class="block" runat="server" id="liNew">
                                <div class="new-title">
                                    <%= Resources.Resource.Client_Default_New %>
                                    <a href="productlist.aspx?type=new">
                                        <%= Resources.Resource.Client_Default_AllNew%></a>
                                </div>
                                <asp:ListView runat="server" ID="lvNewAlternative" ItemPlaceholderID="liItemPlaceholder">
                                    <LayoutTemplate>
                                        <ul class="p-list scp">
                                            <li runat="server" id="liItemPlaceholder"></li>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li class="p-block scp-item" data-productid="<%# Eval("productId")%>" data-display-previews='<%= enablePhotoPreviews %>'>
                                            <table class="p-table">
                                                <tr>
                                                    <td class="img-middle" style="height: <%= ImageMaxHeight%>px">
                                                        <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetInt(Eval("PhotoId")))%>
                                                        <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div class="pv-div-link">
                                                            <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                                class="link-pv-name">
                                                                <%# Eval("Name") %></a>
                                                            <%# string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) %>
                                                        </div>
                                                        <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%# ColorImageWidth %>" />
                                                        <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                            ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                            ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                        <div class="price-container">
                                                            <div class="price">
                                                                <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                            </div>
                                                        </div>
                                                        <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="XSmall" Type="Add"
                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                            Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                        <adv:Button runat="server" Type="Action" Size="XSmall" Text='<%# PreOrderButtonText %>'
                                                            Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("OfferID") %>'
                                                            Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetInt(Eval("Amount")) <= 0  || SQLDataHelper.GetFloat(Eval("Price")) == 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                                        <adv:Button runat="server" Size="XSmall" Type="Buy" Text='<%# MoreButtonText %>'
                                                            Href='<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                            Visible="<%# DisplayMoreButton %>" />

                                                    </td>
                                                </tr>
                                            </table>
                                        </li>
                                    </ItemTemplate>
                                </asp:ListView>
                            </td>
                            <td class="block" runat="server" id="liDiscount">
                                <div class="discount-title">
                                    <%= Resources.Resource.Client_Default_Discount %>
                                    <a href="productlist.aspx?type=discount">
                                        <%= Resources.Resource.Client_Default_AllDiscount %></a>
                                </div>
                                <asp:ListView runat="server" ID="lvDiscountAlternative" ItemPlaceholderID="liItemPlaceholder">
                                    <LayoutTemplate>
                                        <ul class="p-list scp">
                                            <li runat="server" id="liItemPlaceholder"></li>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li class="p-block scp-item" data-productid="<%# Eval("productId")%>" data-display-previews='<%= enablePhotoPreviews %>'>
                                            <table class="p-table">
                                                <tr>
                                                    <td class="img-middle" style="height: <%= ImageMaxHeight%>px">
                                                        <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetInt(Eval("PhotoId")))%>
                                                        <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div class="pv-div-link">
                                                            <a href="<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                                class="link-pv-name">
                                                                <%# Eval("Name") %></a>
                                                            <%# string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) %>
                                                        </div>
                                                        <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%# ColorImageWidth %>" />
                                                        <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                            ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                            ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                        <div class="price-container">
                                                            <div class="price">
                                                                <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                            </div>
                                                        </div>
                                                        <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="XSmall" Type="Add"
                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                            Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                        <adv:Button runat="server" Type="Action" Size="XSmall" Text='<%# PreOrderButtonText %>'
                                                            Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("OfferID") %>'
                                                            Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetInt(Eval("Amount")) <= 0 || SQLDataHelper.GetFloat(Eval("Price")) == 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                                        <adv:Button runat="server" Size="XSmall" Type="Buy" Text='<%# MoreButtonText %>'
                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                            Visible="<%# DisplayMoreButton %>" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </li>
                                    </ItemTemplate>
                                </asp:ListView>
                            </td>
                            <td class="block" runat="server" id="liRecomended">
                                <div class="best-title">
                                    <%= Resources.Resource.Client_Default_AllRecomended %>
                                    <a href="productlist.aspx?type=recomended">
                                        <%= Resources.Resource.Client_Default_Recomended %></a>
                                </div>
                                <asp:ListView runat="server" ID="lvSaleAlternative" ItemPlaceholderID="liItemPlaceholder">
                                    <LayoutTemplate>
                                        <ul class="p-list scp">
                                            <li runat="server" id="liItemPlaceholder"></li>
                                        </ul>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <li class="p-block scp-item" data-productid="<%# Eval("productId")%>" data-display-previews='<%= enablePhotoPreviews %>'>
                                            <table class="p-table">
                                                <tr>
                                                    <td class="img-middle" style="height: <%= ImageMaxHeight%>px">
                                                        <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetInt(Eval("PhotoId")))%>
                                                        <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div class="pv-div-link">
                                                            <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                                class="link-pv-name">
                                                                <%# Eval("Name") %></a>
                                                            <%# string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) %>
                                                        </div>
                                                        <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%# ColorImageWidth %>" />
                                                        <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                            ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                            ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                        <div class="price-container">
                                                            <div class="price">
                                                                <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discont")))%>
                                                            </div>
                                                        </div>
                                                        <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="XSmall" Type="Add"
                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                            Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                        <adv:Button runat="server" Type="Action" Size="XSmall" Text='<%# PreOrderButtonText %>'
                                                            Href='<%# "sendrequestonproduct.aspx?OfferID=" + Eval("OfferID") %>'
                                                            Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetInt(Eval("Amount")) <= 0 || SQLDataHelper.GetFloat(Eval("Price")) == 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                                        <adv:Button runat="server" Size="XSmall" Type="Buy" Text='<%# MoreButtonText %>'
                                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                            Visible="<%# DisplayMoreButton %>" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </li>
                                    </ItemTemplate>
                                </asp:ListView>
                            </td>
                        </tr>
                    </table>
                    <script type="text/javascript">
                        $("td:last-child", ".container-special").addClass("block-last");
                    </script>
                </asp:View>

            </Views>
        </asp:MultiView>
    </div>



</asp:Content>
