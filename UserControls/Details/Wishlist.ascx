<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Wishlist.ascx.cs" Inherits="UserControls.Details.Wishlist" %>
<a  href="<%= ExistInWishlist ? "wishlist.aspx" : "javascript:void(0);" %>"
    data-plugin="wishlist" 
    class="wishlist-link <%= ExistInWishlist ? "js-wishlist-added" : string.Empty %>" 
    data-offerid="<%= OfferId %>">
    <%= ExistInWishlist ? Resources.Resource.Client_Details_AlreadyInWishList : Resources.Resource.Client_Details_AddToWishList %>
</a>