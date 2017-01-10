<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SearchSocial.ascx.cs"
    Inherits="Social.UserControls.Search" %>
<span class="search">
    <adv:AdvTextBox runat="server" DefaultButtonID="btnGoSearch" ID="txtSearch" CssClass="search-text autocompleteSearch" Placeholder="<%$ Resources:Resource, Client_MasterPage_Search %>" />
    <adv:Button runat="server" ID="btnGoSearch" CssClass="btn-search" OnClientClick="searchNowSocial();" />
</span>