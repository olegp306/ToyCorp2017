<%@ Page Language="C#" MasterPageFile="MasterPageSocial.master" EnableEventValidation="false"
    EnableViewState="false" AutoEventWireup="true" CodeFile="CatalogSocial.aspx.cs"
    Inherits="Social.CatalogPage" %>

<%@ Import Namespace="AdvantShop.FilePath" %>
<%@ Register Src="~/Social/UserControls/ProductViewSocial.ascx" TagName="ProductView"
    TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Catalog/ProductViewChanger.ascx" TagName="ProductViewChanger"
    TagPrefix="adv" %>
<%@ Register Src="~/Social/UserControls/CategoryViewSocial.ascx" TagName="CategoryViewSocial"
    TagPrefix="adv" %>
<%@ Register Src="~/UserControls/BreadCrumbs.ascx" TagName="BreadCrumbs" TagPrefix="adv" %>
<%@ MasterType VirtualPath="MasterPageSocial.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <% if (!string.IsNullOrEmpty(MainPageText))
           { %>
           <%= MainPageText%>
        <% } else  { %>
            <% if (category.Picture != null)
               {%>
            <div class="c-banner">
                <img src="<%= FoldersHelper.GetImageCategoryPath(CategoryImageType.Big , category.Picture.PhotoName, false ) %>"
                    alt="<%= category.Name %>" />
            </div>
            <% } %>
            <h1>
                <asp:Literal ID="lblCategoryName" runat="server" /></h1>
            <adv:BreadCrumbs ID="breadCrumbs" runat="server" />
            <%if (!string.IsNullOrEmpty(category.Description))
              {%>
            <div class="c-description">
                <%= category.Description%>
            </div>
            <% } %>
            <adv:CategoryViewSocial ID="categoryView" runat="server" />
            <% if (productView.HasProducts)
               {%>
            <div class="str-sort" runat="server" id="pnlSort">
                <div class="count-search">
                    <asp:Literal runat="server" ID="lTotalItems" /></div>
                <adv:ProductViewChanger runat="server" ID="productViewChanger" CurrentPage="Catalog" />
                <div class="sort-variant">
                    <%=Resources.Resource.Client_Catalog_SortBy%>
                    <asp:DropDownList ID="ddlSort" runat="server" onChange="ApplyFilter(null, true, false);" />
                </div>
                <br class="clear" />
            </div>
            <% } %>
            <adv:ProductView ID="productView" runat="server" />
            <adv:AdvPaging runat="server" ID="paging" DisplayShowAll="True" />
        <%} %>
    </div>
    <br class="clear" />
</asp:Content>
