<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true" 
         CodeFile="CartInfo.aspx.cs"  Inherits="Advantshop.Modules.UserControls.CartInfo" %>
<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
<div style="padding: 15px;">
    <div style="padding: 10px 0;">
        <a href="Module.aspx?module=AbandonedCarts">← Вернуться к списку всех корзин</a>
    </div>

     <div class="order-buyer-name">
        <div class="list-order-data-caption">
            <%= Resource.Admin_ViewOrder_Customer %>
        </div>
        <ul id="customerInfo" runat="server" class="list-order-data">
            <li class="list-order-data-row">
                <div class="list-order-data-name">
                    <%= Resource.Admin_ViewOrder_CustomerName%>:
                </div>
                <div class="list-order-data-value">
                    <asp:HyperLink ID="lnkCustomerName" runat="server" Target="_blank" />
                    <asp:Label ID="lblCustomerName" runat="server" />
                </div>
            </li>
            <li class="list-order-data-row">
                <div class="list-order-data-name">
                    <%= Resource.Admin_ViewOrder_Email %>
                </div>
                <div class="list-order-data-value">
                    <asp:HyperLink ID="lnkCustomerEmail" runat="server" CssClass="order-email" Target="_blank"></asp:HyperLink>
                    <asp:Label ID="lblCustomerEmail" runat="server" />
                </div>
            </li>
            <li class="list-order-data-row">
                <div class="list-order-data-name">
                    <%= Resource.Admin_ViewOrder_Telephone%>
                </div>
                <div class="list-order-data-value">
                    <asp:Label ID="lblCustomerPhone" runat="server" />
                </div>
            </li>
        </ul>
        <div id="customerInfoNotExist" runat="server" Visible="False">
            Нет данных, потому что покупатель не начинал оформление заказа.
        </div>
    </div>
    <ul class="order-list-address">
        <li class="order-list-address-item">
             <% if (ShippingContact != null)
                { %>
            <div class="list-order-data-caption">
                <%= Resource.Admin_ViewOrder_ShippingAddress %>
            </div>
            <ul class="list-order-data">
                <li class="list-order-data-row">
                    <div class="list-order-data-name">
                        <%= Resource.Admin_ViewOrder_Country %>
                    </div>
                    <div class="list-order-data-value">
                        <asp:Label ID="lblShippingCountry" runat="server" />
                    </div>
                </li>
                <% if (ShippingContact.RegionName.IsNotEmpty())
                    { %>
                <li class="list-order-data-row">
                    <div class="list-order-data-name">
                        <%= Resource.Admin_ViewOrder_Zone %>
                    </div>
                    <div class="list-order-data-value">
                        <asp:Label ID="lblShippingRegion" runat="server" />
                    </div>
                </li>
                <% } if (ShippingContact.City.IsNotEmpty())
                    { %>
                <li class="list-order-data-row">
                    <div class="list-order-data-name">
                        <%= Resource.Admin_ViewOrder_City %>
                    </div>
                    <div class="list-order-data-value">
                        <asp:Label ID="lblShippingCity" runat="server" />
                    </div>
                </li>
                <% } if (ShippingContact.Zip.IsNotEmpty())
                    { %>
                <li class="list-order-data-row">
                    <div class="list-order-data-name">
                        <%=Resource.Admin_ViewOrder_Zip %>
                    </div>
                    <div class="list-order-data-value">
                        <asp:Label ID="lblShippingZipCode" runat="server" />
                    </div>
                </li>
                <% } if (ShippingContact.Address.IsNotEmpty())
                     { %>
                <li class="list-order-data-row">
                    <div class="list-order-data-name">
                        <%= Resource.Admin_ViewOrder_Address %>
                    </div>
                    <div class="list-order-data-value">
                        <asp:Label ID="lblShippingAddress" runat="server" />
                    </div>
                </li>
                <% } %>
            </ul>
            <% } %>
        </li>
        <li id="divShipPayments" runat="server" class="order-list-address-item">
            <div class="list-order-data-caption">
                <%= Resource.Admin_ViewOrder_ShippingMethodName %>
            </div>
            <div>
                <asp:Label ID="lblShippingMethodName" runat="server" />
            </div>
            <div class="list-order-data-caption order-payment-method">
                <%= Resource.Admin_ViewOrder_PaymentMethodName %>
            </div>
            <div>
                <asp:Label ID="lblPaymentMethodName" runat="server" />
            </div>
        </li>
    </ul>
    
    
    <div class="order-table-wrap">
        <asp:ListView ID="lvOrderItems" runat="server" ItemPlaceholderID="itemPlaceholderID">
            <LayoutTemplate>
                <table class="table-ui-simple">
                    <caption>
                        <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_OrderContent %>" /></caption>
                    <thead>
                        <tr>
                            <th>
                                <asp:Label ID="Label2" runat="server" Text="<%$ Resources: Resource, Admin_ViewOrder_Product %>" />
                            </th>
                            <th class="table-ui-simple-align-center">
                                <asp:Label ID="Label3" runat="server" Text="<%$ Resources: Resource, Admin_ViewOrder_PriceForUnit %>" />
                            </th>
                            <th class="table-ui-simple-align-center">
                                <asp:Label ID="Label4" runat="server" Text="<%$ Resources: Resource, Admin_ViewOrder_ItemAmount %>" />
                            </th>
                            <th class="table-ui-simple-align-center">
                                <asp:Label ID="Label5" runat="server" Text="<%$ Resources: Resource, Admin_ViewOrder_ItemCost %>" />
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr runat="server" id="itemPlaceholderID">
                        </tr>
                    </tbody>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <a href='<%# UrlService.GetAbsoluteLink("details.aspx?productid=" + Eval("Offer.ProductID")) %>'
                            class="order-item-photo" target="_blank">
                            <%# Eval("Offer.ProductID") != null ? RenderPicture(SQLDataHelper.GetInt(Eval("Offer.ProductID")), SQLDataHelper.GetNullableInt(Eval("Offer.Photo") != null ? Eval("Offer.Photo.PhotoID") : null)) : ""%></a>
                        <div class="order-item-info">
                            <div class="order-item-name">
                                <a href='<%# UrlService.GetAbsoluteLink("details.aspx?productid=" + Eval("Offer.ProductID")) %>' class="order-item-lnk" target="_blank">
                                    <%#Eval("Offer.Product.Name") %></a>
                            </div>
                            <div>
                                <%#Eval("Offer.ArtNo") %>
                            </div>
                            <div class="order-item-options">
                                <div>
                                    <%# !string.IsNullOrEmpty(Convert.ToString(Eval("Offer.Color"))) ? "<span>" + AdvantShop.Configuration.SettingsCatalog.ColorsHeader + ":</span> " + Eval("Offer.Color") : string.Empty %>
                                </div>
                                <div>
                                    <%# !string.IsNullOrEmpty(Convert.ToString(Eval("Offer.Size"))) ? "<span>" + AdvantShop.Configuration.SettingsCatalog.SizesHeader + ":</span> " + Eval("Offer.Size") : string.Empty%>
                                </div>
                                <div>
                                    <%# RenderSelectedOptions((IList<EvaluatedCustomOptions>)Eval("EvaluatedCustomOptions")) %>
                                </div>
                            </div>
                        </div>
                    </td>
                    <td class="table-ui-simple-align-center table-ui-simple-bold">
                        <%# CatalogService.GetStringPrice(Convert.ToSingle(Eval("Price")), 0, 1) %>
                    </td>
                    <td class="table-ui-simple-align-center">
                        <%# Convert.ToSingle(Eval("Amount")) %>
                    </td>
                    <td class="table-ui-simple-align-center table-ui-simple-bold">
                        <%# CatalogService.GetStringPrice(Convert.ToSingle(Eval("Price")), 0, Convert.ToSingle(Eval("Amount")))%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
    </div>


</div>
</asp:Content>