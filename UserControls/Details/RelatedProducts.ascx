<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RelatedProducts.ascx.cs"
    Inherits="UserControls.Details.RelatedProducts" %>
<%@ Register TagPrefix="adv" TagName="Rating" Src="~/UserControls/Rating.ascx" %>
<%@ Register TagPrefix="adv" TagName="SizeColorPickerCatalog" Src="~/UserControls/Catalog/SizeColorPickerCatalog.ascx" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<div class="pv-tile carousel-default">
    <asp:ListView runat="server" ID="lvRelatedProducts" ItemPlaceholderID="liPlaceHolder">
        <%--<LayoutTemplate>
            <ul class="jcarousel">
                <li runat="server" id="liPlaceHolder"></li>
            </ul>
        </LayoutTemplate>--%>
        <ItemTemplate>
            <%--<li style="width: <%= ImageMaxWidth%>px;">--%>
                <div class="pv-item scp-item" data-productid="<%# Eval("productId") %>" style="width: <%= ImageMaxWidth%>px" data-display-previews='<%= enablePhotoPreviews %>'>
                    <table class="p-table">
                        <tr>
                            <td class="img-middle" style="height: <%= ImageMaxHeight%>px;">
                                <%--UrlService.GetAbsoluteLink for IE--%>
                                <div class="goodItemThumb">
                                    <div class="pv-photo">
                                        <%# RenderPictureTag(SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetString(Eval("PhotoDesc")))%></div>
                                    <div class="goodItemInCart">
                                        <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add"
                                            Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                            Text='<%#BuyButtonText  %>' Visible='<%# DisplayBuyButton && ProductService.IsExists(SQLDataHelper.GetInt(Eval("ProductID"))) %>' />
                                    </div>
                                </div>
                                <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="goodItemDesc">
                                    <div class="pv-div-link">
                                        <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductId"))) %>"
                                            class="link-pv-name">
                                            <%# Eval("Name") %></a>
                                    </div>
                                    <div class="price-container">
                                        <div class="price">
                                            <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("MainPrice")), SQLDataHelper.GetFloat(Eval("Discount")), 0)%>
                                        </div>
                                    </div>

                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            <%--</li>--%>
        </ItemTemplate>
    </asp:ListView>
</div>
