<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilterPrice.ascx.cs" Inherits="Social.UserControls.FilterPrice" %>
<%@ Import Namespace="AdvantShop.Repository.Currencies" %>
<!--noindex-->
<div class="block-content-part">
    <div class="title block-subhead">
        <%= Resources.Resource.Client_Catalog_PriceFilter %>
        (
        <%= CurrencyService.CurrentCurrency.Symbol %>
        )</div>
    <div class="content content-price">
        <div class="slider">
            <span class="min">
                <%= Min %></span> <span class="max">
                    <%= Max %></span>
        </div>
        <br class="clear" />
    </div>
</div>
<input type="hidden" id="sliderCurentMin" value="<%= CurValMin %>" />
<input type="hidden" id="sliderCurentMax" value="<%= CurValMax %>" />
<!--/noindex-->
