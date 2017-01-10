<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilterPrice.ascx.cs" Inherits="UserControls.Catalog.FilterPrice" %>
<%@ Import Namespace="AdvantShop.Repository.Currencies" %>
<!--noindex-->
<article class="block-uc-inside">
    <div class="title" data-expander-control="#filter-price">
        <%= Resources.Resource.Client_Catalog_PriceFilter %> (<%= CurrencyService.CurrentCurrency.Symbol %>)</div>
    <div class="content-price" id="filter-price">
        <div class="slider" data-current-min="<%= CurValMin.ToString("F0") %>" data-current-max="<%= CurValMax.ToString("F0") %>">
            <input autocomplete="off" type="text" class="min" value="<%= Min.ToString("F0") %>"/>
            <input autocomplete="off" type="text" class="max" value="<%= Max.ToString("F0") %>" />
        </div>
    </div>
</article>

<!--/noindex-->
