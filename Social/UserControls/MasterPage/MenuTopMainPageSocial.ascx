<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MenuTopMainPageSocial.ascx.cs"
    Inherits="UserControls_MasterPage_MenuTopMainPageSocial" EnableViewState="false" %>
<%@ Register TagPrefix="adv" TagName="MenuCatalogAlternative" Src="~/Social/UserControls/MasterPage/MenuCatalogAlternativeSocial.ascx" %>
<%@ Register TagPrefix="adv" TagName="MenuTopAlternative" Src="~/Social/UserControls/MasterPage/MenuTopAlternativeSocial.ascx" %>
<div class="table-emul">
    <div class="col-left">
        <menu class="catalog-menu-root">
            <li><a href="social/catalogsocial.aspx" class="categories-root pie"><%= Resources.Resource.Client_Catalog_SocialCategories %></a>
                <adv:MenuCatalogAlternative ID="menuCatalogAlternative" runat="server" />
            </li>
        </menu>
    </div>
    <div class="col-right">
        <adv:MenuTopAlternative ID="menuTopAlternative" runat="server" />
    </div>
</div>
