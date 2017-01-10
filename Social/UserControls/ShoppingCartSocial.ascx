<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShoppingCartSocial.ascx.cs"
    Inherits="Social.UserControls.ShoppingCart" %>
<div class="top-panel-cart minicart" data-plugin="cart" data-cart-options="{type:'mini', typeSite: 'social'}">
    <%= Resources.Resource.Client_ShoppingCart_ShoppingCart %>: <span data-cart-count="true" class="minicart-count"><%= Count %></span>
</div>
       

	