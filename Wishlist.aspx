<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" CodeFile="Wishlist.aspx.cs"
    AutoEventWireup="true" Inherits="ClientPages.Wishlist" EnableViewState="true" %>

<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <h1>
                <asp:Localize ID="Localize_Client_Wishlist" runat="server" Text="<%$ Resources:Resource, Client_Wishlist %>" /></h1>
            <asp:ListView runat="server" ID="lvList" ItemPlaceholderID="listPlaceholder">
                <LayoutTemplate>
                    <div class="pv-list">
                        <div runat="server" id="listPlaceholder">
                        </div>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="pv-item">
                        <div class="pv-photo-c">
                            <figure>
                                <div class="pv-photo">
                                    <%# RenderPictureTag(SQLDataHelper.GetString(Eval("Offer.Photo") != null ? Eval("Offer.Photo.PhotoName") : ""), SQLDataHelper.GetString(Eval("Offer.Product.Name")), UrlService.GetLink(ParamType.Product, Eval("Offer.Product.UrlPath").ToString(), SQLDataHelper.GetInt(Eval("Offer.ProductId"))))%>
                                </div>
                                <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Offer.Product.Recomended")), SQLDataHelper.GetBoolean(Eval("Offer.Product.OnSale")), SQLDataHelper.GetBoolean(Eval("Offer.Product.Bestseller")), SQLDataHelper.GetBoolean(Eval("Offer.Product.New")), SQLDataHelper.GetFloat(Eval("Offer.Product.Discount")))%>
                            </figure>
                        </div>
                        <div class="pv-info">
                            <a href="<%# UrlService.GetLink(ParamType.Product, Eval("Offer.Product.UrlPath").ToString(), SQLDataHelper.GetInt(Eval("Offer.ProductId"))) %>"
                                class="link-pv-name">
                                <%# Eval("Offer.Product.Name")%></a>
                            <div class="sku">
                                <%# Eval("Offer.ArtNo")%></div>
                            <div>
                                <%# Eval("Offer.Color")!= null ? AdvantShop.Configuration.SettingsCatalog.ColorsHeader + ": " + Eval("Offer.Color.ColorName") : string.Empty %></div>
                            <div>
                                <%# Eval("Offer.Size") != null ? AdvantShop.Configuration.SettingsCatalog.SizesHeader + ": " + Eval("Offer.Size.SizeName") : string.Empty%></div>
                            <%# CatalogService.RenderSelectedOptions(Eval("AttributesXml").ToString()) %>
                            <%# !string.IsNullOrWhiteSpace(Eval("Offer.Product.BriefDescription").ToString()) ? "<div class=\"descr\">" + Eval("Offer.Product.BriefDescription") + "</div>" : string.Empty%>
                            <div class="price-container">
                                <%# RenderPriceTag(SQLDataHelper.GetFloat(Eval("Offer.Price")), SQLDataHelper.GetFloat(Eval("Offer.Product.Discount")))%>
                            </div>
                            <div class="pv-btns">
                                <adv:Button ID="btnAdd" runat="server" Type="Add" Size="Small" Text='<%# BuyButtonText %>'
                                    OnClientClick='<%# "$(\"#hfWishListItemID\").val(" + SQLDataHelper.GetInt(Eval("ShoppingCartItemId")) + ");__doPostBack(\"" + lbAddTocart.UniqueID + "\",\"\")" %>'
                                    Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Offer.Price")) > 0 && SQLDataHelper.GetFloat(Eval("Offer.Amount")) > 0 %>' />
                                <adv:Button ID="btnOrderByRequest" runat="server" Type="Action" Size="Small" Text='<%# PreOrderButtonText %>'
                                    Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("Offer.OfferId") %>' 
                                    Visible='<%# DisplayPreOrderButton && ((SQLDataHelper.GetFloat(Eval("Offer.Price")) > 0  && SQLDataHelper.GetInt(Eval("Offer.Amount")) == 0) && SQLDataHelper.GetBoolean(Eval("Offer.CanOrderByRequest"))) %>' />
                                <adv:Button ID="btnBuy" runat="server" Type="Buy" Size="Small" Text='<%# MoreButtonText %>'
                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("Offer.Product.UrlPath").ToString(), SQLDataHelper.GetInt(Eval("Offer.Product.ProductId"))) %>'
                                    Visible='<%# DisplayMoreButton %>' />
                            </div>
                        </div>
                        <div class="wishlist-cross">
                            <a class="cross" onclick="<%# "$('#hfWishListItemID').val(" + SQLDataHelper.GetInt(Eval("ShoppingCartItemId")) + ");__doPostBack('"+  lbDelete.UniqueID + "', '');" %>"
                                href="javascript:void(0);"></a>
                        </div>
                    </div>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div class="no-items">
                        <%= Resources.Resource.Client_WishList_NoRecords %>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
            <br class="clear" />
        </div>
    </div>
    <br />
    <asp:HiddenField runat="server" ID="hfWishListItemID" />
    <asp:LinkButton runat="server" ID="lbDelete" OnClick="btnDeleteClick"></asp:LinkButton>
    <asp:LinkButton runat="server" ID="lbAddTocart" OnClick="btnAddToCartClick"></asp:LinkButton>
</asp:Content>
