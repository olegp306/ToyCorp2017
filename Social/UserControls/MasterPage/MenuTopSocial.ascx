<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MenuTopSocial.ascx.cs" Inherits="UserControls_MenuTop_Social"
    EnableViewState="false" %>
<%@ Register Src="~/Social/UserControls/SearchSocial.ascx" TagName="SearchSocial" TagPrefix="adv" %>
<div class="main-menu">
    <%= GetHtml() %>
</div>
    <adv:SearchSocial runat="server" ID="searchBlock" />
