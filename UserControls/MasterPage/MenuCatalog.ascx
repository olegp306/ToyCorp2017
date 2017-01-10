<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MenuCatalog.ascx.cs" EnableViewState="false"
    Inherits="UserControls.MasterPage.MenuCatalog" %>
<%@ Register Src="~/UserControls/MasterPage/Search.ascx" TagName="Search" TagPrefix="adv" %>

<div class="tree" id="tree">
                <nav class="tree-menu"><%=GetMenu()%></nav>
                <adv:Search runat="server" ID="searchBlock" />
</div>