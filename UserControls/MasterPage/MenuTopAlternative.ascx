<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MenuTopAlternative.ascx.cs" Inherits="UserControls.MasterPage.MenuTopAlternative"
    EnableViewState="false" %>
<%@ Register Src="~/UserControls/MasterPage/Search.ascx" TagName="Search" TagPrefix="adv" %>
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