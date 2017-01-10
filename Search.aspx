<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Search.aspx.cs" Inherits="ClientPages.Search" %>

<%@ Import Namespace="Resources" %>
<%@ Register Src="UserControls/Catalog/ProductView.ascx" TagName="ProductView" TagPrefix="adv" %>
<%@ Register TagPrefix="adv" TagName="FilterPrice" Src="~/UserControls/Catalog/FilterPrice.ascx" %>
<%@ Register TagPrefix="adv" TagName="RecentlyView" Src="~/UserControls/Catalog/RecentlyView.ascx" %>
<%@ Register TagPrefix="adv" TagName="ProductViewChanger" Src="~/UserControls/Catalog/ProductViewChanger.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="col-left">
            <article class="block-uc block-search" id="filter">
                <h3 class="title">
                    <%=Resource.Client_Search_SearchParams %></h3>
                <div class="content">
                    <div class="param-name">
                        <%=Resource.Client_Search_Find %>:</div>
                    <div class="param-value">
                        <adv:AdvTextBox runat="server" ID="txtName" DefaultButtonID="btnFind" CssClass="autocompleteSearch" />
                    </div>
                    <div class="param-name">
                        <%=Resource.Client_Search_InCategories %>:
                    </div>
                    <div class="param-value">
                        <asp:DropDownList ID="ddlCategory" runat="server">
                            <asp:ListItem Text="<%$ Resources:Resource, Client_Search_AllCategories %>" Value="0"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <adv:FilterPrice runat="server" ID="filterPrice" />
                    <div class="uc-search">
                        <adv:Button runat="server" ID="btnFind" Type="Submit" Size="Middle" OnClientClick="ApplySearch();"
                            Text="<%$ Resources:Resource, Client_Search_Find %>" />
                    </div>
                    <div class="declare">
                    </div>
                </div>
            </article>
            <adv:RecentlyView runat="server" />
        </div>
        <div class="col-right">
            <div class="page-name">
                <%= Resource.Client_Search_Found%>
                <strong>
                    <asp:Literal runat="server" ID="lItemsCount"></asp:Literal></strong>
                <% if (!String.IsNullOrWhiteSpace(SearchTerm))
                   {%>
                <%= Resource.Client_Search_OnQuery%>
                &quot;<%= SearchTerm%>&quot;
                <% } %>
            </div>
            <div class="str-sort" runat="server" id="pnlSort">
                <div class="sort-variant sort-variant-l">
                    <%= Resource.Client_Catalog_SortBy%>
                    <asp:DropDownList ID="ddlSort" runat="server" onChange="ApplySearch();">
                    </asp:DropDownList>
                </div>
                <adv:ProductViewChanger runat="server" ID="productViewChanger" CurrentPage="Search" />
                <br class="clear" />
            </div>
            <adv:ProductView ID="vProducts" runat="server" />
            <adv:AdvPaging runat="server" ID="paging" DisplayShowAll="True" />
        </div>
        <br class="clear" />
    </div>
</asp:Content>
