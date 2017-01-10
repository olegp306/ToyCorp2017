<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ToolbarBottom.ascx.cs"
    Inherits="UserControls.MasterPage.ToolbarBottom" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Register Src="~/UserControls/Rating.ascx" TagName="Rating" TagPrefix="adv" %>
<!--noindex-->
<div class="toolbar-bottom js-toolbar-bottom">
    <div class="toolbar-bottom-content clearfix">
        <div runat="server" id="toolbarBottomRecently" class="toolbar-bottom-link toolbar-bottom-left js-toolbar-bottom-recently">
            <span class="toolbar-bottom-count js-toolbar-bottom-count">
                <%=  RecentlyCount %></span> <a href="javascript:void(0);" class="toolbar-bottom-link-text">
                    <%= Resource.Client_UserControls_RecentlyViews_RecentlyViewed %></a>
            <asp:ListView runat="server" ID="lvToolbarBottomRecentlyView" ItemPlaceholderID="toolbarBottomRecentlyPlacehodler">
                <LayoutTemplate>
                    <div class="toolbar-bottom-submenu-wrap">
                        <div class="toolbar-bottom-submenu">
                            <ul class="toolbar-bottom-submenu-list">
                                <li runat="server" id="toolbarBottomRecentlyPlacehodler"></li>
                            </ul>
                        </div>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <li class="toolbar-bottom-submenu-row">
                        <figure class="toolbar-bottom-submenu-photo">
                            <%# RenderPictureTag(SQLDataHelper.GetString(Eval("ImgPath")), UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))), SQLDataHelper.GetString(Eval("PhotoDesc")), SQLDataHelper.GetString(Eval("Name")))%>
                        </figure>
                        <div class="toolbar-bottom-submenu-info">
                            <div class="toolbar-bottom-submenu-name">
                                <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID")) ) %>">
                                    <%# Eval("Name") %></a></div>
                            <div class="toolbar-bottom-submenu-price">
                                <%# RenderPriceTag(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")), SQLDataHelper.GetFloat(Eval("MultiPrice")))%></div>
                            <adv:Rating ID="ucRating" runat="server" ProductId='<%# Convert.ToInt32(Eval("ProductID")) %>'
                                ShowRating='<%# EnableRating %>' Rating='<%# Convert.ToDouble(Eval("Ratio")) %>'
                                ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                        </div>
                    </li>
                </ItemTemplate>
            </asp:ListView>
        </div>
        <div runat="server" id="toolbarBottomCompare" class="toolbar-bottom-left">
            <a href="compareproducts.aspx" class="toolbar-bottom-link js-toolbar-bottom-compare">
                <span class="toolbar-bottom-count js-toolbar-bottom-count">
                    <%= CompareCount %></span><span class="toolbar-bottom-link-text"><%= Resource.Client_Catalog_CompareCart %></span>
            </a>
        </div>
        <div runat="server" id="toolbarBottomWishlist" class="toolbar-bottom-left">
            <a href="wishlist.aspx" class="toolbar-bottom-link js-toolbar-bottom-wishlist"><span
                class="toolbar-bottom-count js-toolbar-bottom-count">
                <%= WishlistCount %></span><span class="toolbar-bottom-link-text"><%= Resource.Client_Wishlist %></span>
            </a>
        </div>
        <div runat="server" id="toolbarBottomInplace" class="toolbar-bottom-block toolbar-bottom-left">
            <span class="toolbar-bottom-i-checkbox-text">
                <%= Resource.Client_Editing_Mode %></span>
            <label class="toolbar-bottom-i-checkbox js-toolbar-bottom-i-checkbox">
                <input name="inplaceEnable" id="inplaceEnable" class="toolbar-bottom-i-checkbox-native js-toolbar-bottom-inplace"
                    type="checkbox" value="" <%= AdvantShop.Configuration.SettingsMain.EnableInplace ? "checked=\"checked\"" : "" %> />
                <i class="toolbar-bottom-i-checkbox-control js-toolbar-bottom-i-checkbox-control"></i>
                <span class="toolbar-bottom-i-text-on"><%= Resource.Client_Editing_Mode_On%></span>
                <span class="toolbar-bottom-i-text-off"><%= Resource.Client_Editing_Mode_Off%></span>
            </label>
        </div>
        <%if (AdvantShop.Configuration.SettingsDesign.ShoppingCartVisibility)
          {%>
        <a href="<%= ShowConfirmButton ? "orderconfirmation.aspx" : "shoppingcart.aspx" %>"
            class="btn btn-middle toolbar-bottom-action toolbar-bottom-right js-toolbar-bottom-confirm <%= CartCount == 0 ? "btn-disabled": "" %>">
            <%= Resource.Client_ShoppingCart_DrawUp %></a> <a href="shoppingcart.aspx" class="toolbar-bottom-link toolbar-bottom-right js-toolbar-bottom-cart"
                data-toolbar-bottom-count="<%= CartCount %>"><span class="toolbar-bottom-count js-toolbar-bottom-count">
                    <%= CartCount %></span> <span class="toolbar-bottom-link-text">
                        <%= Resource.Client_ShoppingCart_ShoppingCart %></span></a>
        <% } %>
    </div>
</div>
<!--/noindex-->