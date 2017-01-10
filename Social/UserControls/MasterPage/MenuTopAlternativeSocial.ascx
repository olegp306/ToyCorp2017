<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MenuTopAlternativeSocial.ascx.cs" Inherits="UserControls_MenuTopAlternativeSocial"
    EnableViewState="false" %>
<%@ Register Src="~/Social/UserControls/SearchSocial.ascx" TagName="Search" TagPrefix="adv" %>
<div class="tree" id="tree">
    <div class="left">
        <div class="right">
            <div class="center">
                <div class="tree-menu"><%=GetMenu()%></div>
                <adv:Search runat="server" ID="searchBlock" />
            </div>
        </div>
    </div>
</div>