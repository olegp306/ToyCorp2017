<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="BrandView.aspx.cs" Inherits="ClientPages.BrandView" EnableViewState="false" %>

<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.FilePath" %>
<%@ Import Namespace="AdvantShop.CMS" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner-thin">
            <div class="brand-header">
                <%= Resources.Resource.Client_Brands_Header %></div>
            <div class="brands-symbol">
                <div class="brands-abc">
                    <div>
                        <asp:Literal ID="lEngLetters" runat="server" EnableViewState="false"></asp:Literal></div>
                    <div>
                        <asp:Literal ID="lRusLetters" runat="server" EnableViewState="false"></asp:Literal>
                    </div>
                </div>
            </div>
            <div class="brands-country">
                <asp:DropDownList ID="ddlCountry" runat="server" DataTextField="Name" DataValueField="CountryID"
                    onchange="redirectBrandCountry()">
                </asp:DropDownList>
            </div>
            <div>
                <div class="brand-left">
                    <% if (brand.BrandLogo != null)
                       { %>
                    <div class="brands-logo">
                        <img class="manufacturer-picture" alt="<%=  HttpUtility.HtmlEncode(brand.Name) %>" src="<%= FoldersHelper.GetPath(FolderType.BrandLogo,brand.BrandLogo.PhotoName  ,false) %>" <%= InplaceEditor.Image.AttributesBrand(brand.BrandId) %> />
                    </div>
                    <% }else if(AdvantShop.Customers.CustomerContext.CurrentCustomer.IsAdmin || AdvantShop.Trial.TrialService.IsTrialEnabled){ %>
                         <div class="brands-logo">
                            <img class="manufacturer-picture js-inplace-image-visible-permanent" alt="<%= HttpUtility.HtmlEncode(brand.Name) %>" src="images/nophoto_xsmall.jpg" <%= InplaceEditor.Image.AttributesBrand(brand.BrandId) %>/>
                        </div>
                    <% } %>
                    <asp:Literal runat="server" ID="lCategories"></asp:Literal>
                </div>
                <div class="brand-right">
                    <div class="brands-country">
                        <span class="country">
                            <img alt="<%= brand.BrandCountry!= null ? brand.BrandCountry.Name : string.Empty  %>"
                                src='<%= UrlService.GetAbsoluteLink("images/Countries/" + (brand.BrandCountry !=null ? brand.BrandCountry.Iso2 + ".png": "_world.png"))%>' />
                            <%= brand.BrandCountry != null ? brand.BrandCountry.Name : string.Empty  %></span></div>
                    <div class="brands-name">
                        <h1  <%= InplaceEditor.Meta.Attribute(AdvantShop.SEO.MetaType.Brand, brand.BrandId) %>>
                            <%= metaInfo.H1 %></h1>
                    </div>
                    <div class="brands-brandurl" runat="server" id="divBrandSiteUrl">
                        <a href='<%= brand.BrandSiteUrl %>' target="_blank">
                            <%=Resources.Resource.Client_Brands_BrandSiteUrl%></a>
                    </div>
                    <div class="brands-descr" <%= InplaceEditor.Brand.Attribute(brand.BrandId, InplaceEditor.Brand.Field.Description)%>>
                        <% =brand.Description %>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
