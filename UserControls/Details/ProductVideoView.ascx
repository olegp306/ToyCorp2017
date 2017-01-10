<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductVideoView.ascx.cs" Inherits="UserControls.Details.ProductVideoView" %>
<% if (hasVideos)
   { %>
   <div data-plugin="videos" data-productId="<%=ProductID %>"></div>
<% } %>