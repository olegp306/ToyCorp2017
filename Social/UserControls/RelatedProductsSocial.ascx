<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RelatedProductsSocial.ascx.cs"
    Inherits="Social.UserControls.RelatedProducts" %>
<%@ Register TagPrefix="adv" TagName="Rating" Src="~/UserControls/Rating.ascx" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<div class="pv-tile carousel-default">
    <asp:ListView runat="server" ID="lvRelatedProducts" ItemPlaceholderID="liPlaceHolder">
        <LayoutTemplate>
            <ul class="jcarousel">
                <li runat="server" id="liPlaceHolder"></li>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <table class="p-table">
                    <tr>
                        <td class="img-middle">
                            <div class="pv-photo" onclick="location.href='<%# "social/detailssocial.aspx?productId=" + Eval("ProductId") %>'">
                                <%# RenderPictureTag(SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("Name")))%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <a href="<%# "social/detailssocial.aspx?productId=" + Eval("ProductId") %>"
                                    class="link-pv-name">
                                    <%# Eval("Name") %></a>
                            </div>
                            <%# SQLDataHelper.GetString(Eval("Name")).Length < 24 ? string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) : string.Empty %>
                            <adv:Rating ID="Rating1" runat="server" />
                            <div class="price-container">
                                <%# RenderPrice(SQLDataHelper.GetFloat(Eval("MainPrice")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                            </div>
                            <div class="pv-btns">
                                <adv:Button ID="btnAdd" CssSpan="btn-add-icon" runat="server" Type="Add" Size="Middle"
                                    Text='<%$ Resources:Resource, Client_Catalog_Add %>' Href='<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetInt(Eval("ProductId"))) %>'
                                    Visible='<%# SQLDataHelper.GetFloat(Eval("MainPrice")) > 0 && SQLDataHelper.GetFloat(Eval("TotalAmount")) > 0 %>'
                                    Rel='<%# "productid:" + Eval("ProductID") %>' />
                                <adv:Button ID="btnAction" runat="server" Type="Action" Size="Small" Text='<%$ Resources:Resource, Client_Catalog_OrderByRequest %>'
                                    Href='<%# "sendrequestonproduct.aspx?productid=" + Eval("productId") %>' Visible='<%# (!(SQLDataHelper.GetFloat(Eval("MainPrice")) > 0 && SQLDataHelper.GetInt(Eval("TotalAmount")) > 0) && SQLDataHelper.GetBoolean(Eval("AllowPreOrder"))) %>' Target="_blank" />
                                <adv:Button ID="btnBuy" runat="server" Type="Buy" Size="Middle" Text='<%$ Resources:Resource, Client_More %>' Href='<%# "social/detailssocial.aspx?productId=" + Eval("ProductId") %>' />
                            </div>
                        </td>
                    </tr>
                </table>
            </li>
        </ItemTemplate>
    </asp:ListView>
</div>
