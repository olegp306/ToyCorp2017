<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductView.ascx.cs" Inherits="UserControls.Catalog.ProductView"
    EnableViewState="false" %>
<%@ Register Src="~/UserControls/Rating.ascx" TagName="Rating" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/CompareControl.ascx" TagName="CompareControl"
    TagPrefix="adv" %>
<%@ Register TagPrefix="adv" TagName="SizeColorPickerCatalog" Src="~/UserControls/Catalog/SizeColorPickerCatalog.ascx" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Import Namespace="AdvantShop.CMS" %>
<%@ Import Namespace="Resources" %>
<asp:MultiView runat="server" ID="mvProducts">
    <Views>
        <asp:View runat="server" ID="viewTile">
            <asp:ListView runat="server" ID="lvTile" ItemPlaceholderID="tilePlaceHolder">
                <LayoutTemplate>
                    <div class="pv-tile scp">
                        <div runat="server" id="tilePlaceHolder">
                        </div>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="pv-item scp-item" data-productid="<%# Eval("ProductId") %>" style='width: <%= ImageWidth %>px;' data-display-previews='<%= enablePhotoPreviews %>'>
                        <div class="pv-photo-wrap" style="height: <%= ImageHeightSmall %>px; width: <%= ImageWidth %>px;">
                            <div class="goodItemThumb">
                                <div class="pv-photo">
                                    <%# RenderPictureTag(SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("AdditionalPhoto")), UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetInt(Eval("PhotoId")), SQLDataHelper.GetInt(Eval("ProductId")))%>
                                </div>
                                <div class="goodItemInCart">
                                    <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>'
                                        runat="server" Type="Add" Size="Small" Text='<%# BuyButtonText %>'
                                        Href='<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))) %>'
                                        Visible='<%# DisplayBuyButton && SQLDataHelper.GetDecimal(Eval("Price")) > 0 && SQLDataHelper.GetDecimal(Eval("Amount")) > 0 %>'
                                        data-color-id='<%# Eval("ColorID") %>' />
                                </div>
                            </div>
                            <%# RenderLabels(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                        </div>
                        <div class="goodItemDesc">
                            <div class="pv-info">
                                <div class="pv-div-link compare-<%# Eval("OfferId") %>">
                                    <a href="<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))) %>"
                                        class="link-pv-name">
                                        <%# Eval("Name") %></a>
                                </div>
                                <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>'
                                    Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%#ColorImageWidth %>" />
                                <adv:Rating ID="Rating1" runat="server" ProductId='<%# Convert.ToInt32(Eval("ProductID")) %>'
                                    ShowRating='<%# EnableRating %>' Rating='<%# Convert.ToDouble(Eval("Ratio")) %>'
                                    ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                <div class="price-container">
                                    <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")), SQLDataHelper.GetFloat(Eval("MultiPrices")), false)%>
                                </div>


                                <adv:CompareControl ID="CompareControl" runat="server" Visible='<%# EnableCompare && SQLDataHelper.GetInt(Eval("OfferId"))> 0%>'
                                    OfferId='<%# SQLDataHelper.GetInt(Eval("OfferId")) %>' IsSelected='<%# Eval("ShoppingCartItemID") != DBNull.Value%>' />
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div class="no-items">
                        <%= Resource.Client_Catalog_NoItemsFound  %>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </asp:View>
        <asp:View runat="server" ID="viewList">
            <asp:ListView runat="server" ID="lvList" ItemPlaceholderID="listPlaceholder">
                <LayoutTemplate>
                    <div class="pv-list scp">
                        <div runat="server" id="listPlaceholder">
                        </div>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <div class="pv-item scp-item" data-productid="<%# Eval("ProductId") %>" <%# SQLDataHelper.GetInt(Eval("CountPhoto")) >= 3 && enablePhotoPreviews == "true" ? "style=\"min-height:" + 3 * (ImageHeightXSmall + 17) + "px;\"" : "" %> data-display-previews='<%= enablePhotoPreviews %>'>
                        <div class="pv-photo-c">
                            <figure>
                                <div class="pv-photo" style='width: <%= ImageWidth%>px;'>
                                    <%# RenderPictureTag(SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("AdditionalPhoto")), UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetInt(Eval("PhotoId")), SQLDataHelper.GetInt(Eval("ProductId")))%>
                                </div>
                                <%# RenderLabels(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                            </figure>
                        </div>
                        <div class="pv-info">
                            <a href="<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))) %>"
                                class="link-pv-name compare-<%#Eval("OfferId") %>">
                                <%# Eval("Name") %></a>
                            <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>'
                                Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>' ImageHeight="<%# ColorImageHeight %>" ImageWidth="<%#ColorImageWidth %>" />
                            <adv:Rating runat="server" ProductId='<%# Convert.ToInt32(Eval("ProductID")) %>'
                                ShowRating='<%# EnableRating %>' Rating='<%# Convert.ToDouble(Eval("Ratio")) %>'
                                ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                            <asp:Label ID="lblNoAmount" runat="server" CssClass="not-available" Text='<%$ Resources:Resource, Client_Catalog_NotAvailable%>'
                                Visible='<%#SQLDataHelper.GetDecimal(Eval("Amount")) <= 0 %>'></asp:Label>

                            <div class="descr" data-plugin="inplace" data-inplace-params="{id: <%# Eval("ProductId") %>, type: <%= "'" + InplaceEditor.ObjectType.Product + "'"  %>, prop: <%= "'" +  InplaceEditor.Product.Field.BriefDescription + "'"%>}"><%# Eval("BriefDescription") %></div>

                            <div class="price-container">
                                <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")), SQLDataHelper.GetFloat(Eval("MultiPrices")), true)%>
                            </div>
                            <div class="pv-btns">
                                <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>'
                                    runat="server" Type="Add" Size="Small" Text='<%# BuyButtonText %>'
                                    Href='<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))) %>'
                                    Visible='<%# DisplayBuyButton && SQLDataHelper.GetDecimal(Eval("Price")) > 0 && SQLDataHelper.GetDecimal(Eval("Amount")) > 0 %>' />
                                <adv:Button runat="server" Type="Action" Size="Small" Text='<%# PreOrderButtonText %>'
                                    Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("OfferID") %>'
                                    Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetInt(Eval("Amount")) <= 0 || SQLDataHelper.GetFloat(Eval("Price")) == 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                <adv:Button runat="server" Type="Buy" Size="Small" Text='<%# MoreButtonText %>'
                                    Href='<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))) %>'
                                    Visible='<%# DisplayMoreButton %>' />
                                <adv:CompareControl ID="CompareControl" runat="server" Visible='<%# EnableCompare %>'
                                    OfferId='<%# SQLDataHelper.GetInt(Eval("OfferId")) %>' IsSelected='<%# Eval("ShoppingCartItemId") != DBNull.Value%>' />
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div class="no-items">
                        <%= Resource.Client_Catalog_NoItemsFound  %>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </asp:View>
        <asp:View runat="server" ID="viewTable">
            <asp:ListView runat="server" ID="lvTable" ItemPlaceholderID="tablePlaceHolder">
                <LayoutTemplate>
                    <table class="pv-table">
                        <tr class="head">
                            <th class="icon"></th>
                            <th class="p-name">
                                <asp:Literal runat="server" Text="<%$ Resources:Resource, Client_UserControls_ProductView_Name%>" />
                            </th>
                            <th class="rating"></th>
                            <th class="pv-price">
                                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Resource, Client_UserControls_ProductView_Price%>" />
                            </th>
                            <th class="btns"></th>
                        </tr>
                        <tr runat="server" id="tablePlaceHolder" />
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr class="pv-item">
                        <td class="icon" <%# RenderPictureTag(SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("AdditionalPhoto")), UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")),  SQLDataHelper.GetInt(Eval("PhotoId")), SQLDataHelper.GetInt(Eval("ProductId")))%>>
                            <%# !string.IsNullOrEmpty(Eval("Photo").ToString()) ? "<div class=\"photo\"></div>" : string.Empty%>
                            <adv:CompareControl Type="Icon" ID="CompareControl" runat="server" Visible='<%# EnableCompare %>'
                                OfferId='<%# Eval("OfferId") %>' IsSelected='<%# Eval("ShoppingCartItemID") != DBNull.Value%>' />
                        </td>
                        <td>
                            <a href="<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))) %>"
                                class="link-pv-name compare-<%#Eval("OfferId") %>">
                                <%# Eval("Name") %></a>
                        </td>
                        <td class="rating">
                            <adv:Rating runat="server" ProductId='<%# Convert.ToInt32(Eval("ProductID")) %>'
                                ShowRating='<%# EnableRating %>' Rating='<%# Convert.ToDouble(Eval("Ratio")) %>'
                                ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                        </td>
                        <td class="pv-price">
                            <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")), SQLDataHelper.GetFloat(Eval("MultiPrices")), true)%>
                            <%# RenderLabels(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")), 1)%>
                        </td>
                        <td class="btns">
                            <adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>'
                                runat="server" Type="Add" Size="Small" Text='<%# BuyButtonText %>'
                                Href='<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))) %>'
                                Visible='<%# DisplayBuyButton &&  SQLDataHelper.GetDecimal(Eval("Price")) > 0 && SQLDataHelper.GetDecimal(Eval("Amount")) > 0 %>' />
                            <adv:Button runat="server" Type="Action" Size="Small" Text='<%# PreOrderButtonText %>'
                                Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("OfferID") %>'
                                Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetInt(Eval("Amount")) <= 0  || SQLDataHelper.GetFloat(Eval("Price")) == 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                            <adv:Button runat="server" Type="Buy" Size="Small" Text='<%# MoreButtonText %>'
                                Href='<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))) %>'
                                Visible="<%# DisplayMoreButton %>" />
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <div class="no-items">
                        <%= Resource.Client_Catalog_NoItemsFound  %>
                    </div>
                </EmptyDataTemplate>
            </asp:ListView>
        </asp:View>
    </Views>
</asp:MultiView>
