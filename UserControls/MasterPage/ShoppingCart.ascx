<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShoppingCart.ascx.cs"
    Inherits="UserControls.MasterPage.ShoppingCartControl" %>
<div class="minicart" data-plugin="cart" data-cart-options="{type:'mini', typeSite: '<%=TypeSite %>'}">
    <span class="minicart-text"><%= Resources.Resource.Client_ShoppingCart_ShoppingCart%>: <span data-cart-count="true" class="minicart-count"><%= Count %></span></span>
</div>