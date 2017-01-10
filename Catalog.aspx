<%@ Page Language="C#" MasterPageFile="MasterPage.master" EnableEventValidation="false"
    EnableViewState="false" AutoEventWireup="true" CodeFile="Catalog.aspx.cs" Inherits="ClientPages.Catalog_Page" %>

<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.FilePath" %>
<%@ Import Namespace="AdvantShop.CMS" %>
<%@ Register Src="~/UserControls/Catalog/ProductView.ascx" TagName="ProductView" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/ProductViewChanger.ascx" TagName="ProductViewChanger" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/CategoryView.ascx" TagName="CategoryView" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/CatalogView.ascx" TagName="CatalogView" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/FilterProperty.ascx" TagName="FilterProperty" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/FilterBrand.ascx" TagName="FilterBrand" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/FilterSize.ascx" TagName="FilterSize" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/FilterColor.ascx" TagName="FilterColor" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/FilterPrice.ascx" TagName="FilterPrice" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/FilterExtra.ascx" TagName="FilterExtra" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/RecentlyView.ascx" TagName="RecentlyView" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/BreadCrumbs.ascx" TagName="BreadCrumbs" TagPrefix="adv" %>
<%@ Register Src="~/UserControls/StaticBlock.ascx" TagName="StaticBlock" TagPrefix="adv" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div>
        <h1 class="mainContantTitle" <%= InplaceEditor.Meta.Attribute(AdvantShop.SEO.MetaType.Category, Category.ID) %>>
            <asp:Literal ID="lblCategoryName" runat="server" /></h1>
        <adv:BreadCrumbs ID="breadCrumbs" runat="server" />
        <div class="sort-variant sortOrderList">
            <%--<p><%=Resources.Resource.Client_Catalog_SortBy%>:</p>--%>
            <asp:DropDownList ID="ddlSort" runat="server" onChange="ApplyFilter(null, true, false);" />
            <div class="sortOrderList">
                <p><%=Resources.Resource.Client_Catalog_SortBy%>:</p>
                <ul>
                    <li>
                        <a class="NoSort" id="SortByAddingDate" href="javascript:void(0);">Новизне</a>
                    </li>
                    <li>
                        <a class="NoSort" id="SortByPrice" href="javascript:void(0);">Цене</a>
                    </li>
                    <li>
                        <a class="NoSort" id="SortByPopularity" href="javascript:void(0);">Популярности</a>
                    </li>
                    <%-- <li class="DescRating">
                        <a class="NoSort" id="SortByRating" href="javascript:void(0);">Рейтингу</a>
                    </li>--%>
                </ul>
            </div>
        </div>
        <div class="col-left col-left-padding">
            <div class="categoryListNavigation">
                <adv:CatalogView runat="server" ID="catalogView" />
            </div>
            <%if (SettingsDesign.FilterVisibility && (filterBrand.Visible || filterPrice.Visible || filterProperty.Visible || filterColor.Visible || filterSize.Visible) && ProductsCount > 1)
                {%>
            <div class="filterOnMain">
                <h3 class="filterTitle">МЫ ПОМОЖЕМ ВАМ ПОДОБРАТЬ ИГРУШКУ</h3>
                <article class="block-uc" id="filter">
                    <div class="content" id="filter-content">
                        <adv:FilterPrice runat="server" ID="filterPrice" />
                        <adv:FilterBrand runat="server" ID="filterBrand" />
                        <adv:FilterColor runat="server" ID="filterColor" />
                        <adv:FilterSize runat="server" ID="filterSize" />
                        <adv:FilterProperty runat="server" ID="filterProperty" />
                        <adv:FilterExtra runat="server" ID="filterExtra" />
                        <div class="aplly-price">
                            <adv:Button runat="server" Size="Small" Type="Confirm" Text="ПОИСК"
                                OnClientClick="ApplyFilter(null, true, false);" CssClass="filter-button"></adv:Button>
                        </div>
                    </div>
                </article>
            </div>
            <% } %>
        </div>
        <div class="col-right">
            <%--<% if (Category.Picture != null)
               {%>
            <div class="c-banner">
                <img class="js-inplace-image-visible-permanent" <%= InplaceEditor.Image.AttributesCategory(Category.ID, InplaceEditor.Image.Field.CategoryBig) %> src="<%= FoldersHelper.GetImageCategoryPath(CategoryImageType.Big , Category.Picture.PhotoName , false) %>"
                    alt="<%= HttpUtility.HtmlEncode(Category.Name) %>" />
            </div>
            <%} %>--%>

            <div style="display: none;">
                <%if ((!string.IsNullOrEmpty(Category.BriefDescription) || IsAdmin) && paging.CurrentPage == 1)
                    {%>
                <div class="c-description" <%= InplaceEditor.Category.Attribute(Category.ID, InplaceEditor.Category.Field.BriefDescription) %>><%= Category.BriefDescription%></div>
                <% } %>
                <adv:CategoryView ID="categoryView" runat="server" />
                <% if (productView.HasProducts)
                    {%>
                <div class="str-sort" runat="server" id="pnlSort">
                    <div class="count-search">
                        <asp:Literal runat="server" ID="lTotalItems" />
                    </div>

                    <div class="clear">
                    </div>
                </div>
                <% } %>
            </div>

            <%if ((!string.IsNullOrEmpty(Category.Description) || IsAdmin) && paging.CurrentPage == 1)
                {%>
            <div class="c-briefdescription" style="margin-top: 0px !important;" <%= InplaceEditor.Category.Attribute(Category.ID, InplaceEditor.Category.Field.Description) %>><%= Category.Description%></div>
            <% } %>

            <adv:ProductView ID="productView" runat="server" />

            <adv:StaticBlock ID="sbCatalogBottom" runat="server" SourceKey="CatalogBottom" />
        </div>
    </div>
    </div>
        <div class="paginationSection">
            <adv:AdvPaging runat="server" ID="paging" DisplayShowAll="True" />
        </div>
</asp:Content>
