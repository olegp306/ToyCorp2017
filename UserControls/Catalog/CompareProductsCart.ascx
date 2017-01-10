<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CompareProductsCart.ascx.cs"
    Inherits="UserControls.Catalog.CompareProductsCart" %>
<!--noindex-->
<div id="compare" class="expander block-uc">
    <span class="title expand ex-control">
        <%= Resources.Resource.Client_UserControls_CompareProducts_CompareProducts %><span
            class="control"></span></span>
    <div class="content ex-content">
        <div class="compare-list">
            <div class="compare-no">
                <%= Resources.Resource.Client_CompareProductsCart_NoProducts%></div>
        </div>
        <div class="btn-compare">
            <adv:Button runat="server" Type="Confirm" Size="small" OnClientClick="window.open('compareproducts.aspx')"
                Text="<%$ Resources:Resource, Client_UserControls_CompareProducts_ViewCart %>" />
        </div>
    </div>
</div>
<!--/noindex-->
