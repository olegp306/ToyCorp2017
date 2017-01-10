<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Rating.ascx.cs" Inherits="UserControls.RatingControl" %>
<div class="rating">
    <div class="<%= ReadOnly ? "rating-readonly" : string.Empty  %>" data-rating-objid="rating_<%=ProductId %>">
    </div>
    <input type="hidden" value="<%= Rating %>" data-rating-objid="rating_hidden_<%=ProductId %>" />
</div>
