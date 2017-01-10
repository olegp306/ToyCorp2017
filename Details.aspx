<%@ Page Language="C#" MasterPageFile="MasterPage.master" EnableViewState="false"
    CodeFile="Details.aspx.cs" Inherits="ClientPages.Details" %>

<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.FilePath" %>
<%@ Import Namespace="AdvantShop.Customers" %>
<%@ Import Namespace="AdvantShop.CMS" %>
<%@ Import Namespace="Resources" %>

<%@ Register TagPrefix="adv" TagName="CustomOptions" Src="~/UserControls/Details/CustomOptions.ascx" %>
<%@ Register TagPrefix="adv" TagName="ProductPhotoView" Src="~/UserControls/Details/ProductPhotoView.ascx" %>
<%@ Register TagPrefix="adv" TagName="ProductVideoView" Src="~/UserControls/Details/ProductVideoView.ascx" %>
<%@ Register TagPrefix="adv" TagName="ProductPropertiesView" Src="~/UserControls/Details/ProductPropertiesView.ascx" %>
<%@ Register TagPrefix="adv" TagName="ProductBriefPropertiesView" Src="~/UserControls/Details/ProductBriefPropertiesView.ascx" %>
<%@ Register TagPrefix="adv" TagName="RelatedProducts" Src="~/UserControls/Details/RelatedProducts.ascx" %>
<%@ Register TagPrefix="adv" TagName="ProductReviews" Src="~/UserControls/Details/ProductReviews.ascx" %>
<%@ Register TagPrefix="adv" TagName="BuyInOneClick" Src="~/UserControls/BuyInOneClick.ascx" %>
<%@ Register TagPrefix="adv" TagName="BreadCrumbs" Src="~/UserControls/BreadCrumbs.ascx" %>
<%@ Register TagPrefix="adv" TagName="Rating" Src="~/UserControls/Rating.ascx" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<%@ Register TagPrefix="adv" TagName="CompareControl" Src="~/UserControls/Catalog/CompareControl.ascx" %>
<%@ Register TagPrefix="adv" TagName="SizeColorPicker" Src="~/UserControls/Details/SizeColorPickerDetails.ascx" %>
<%@ Register TagPrefix="adv" TagName="Social" Src="~/UserControls/Social.ascx" %>
<%@ Register TagPrefix="adv" TagName="Wishlist" Src="~/UserControls/Details/Wishlist.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <script type="text/javascript" src="//vk.com/js/api/share.js?11" charset="windows-1251"></script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke det-page">
        <h2 class="details-category"><%= CategoryService.GetCategory(CurrentProduct.CategoryId).Name %></h2>
        <div class="crumbs-thin">
            <adv:BreadCrumbs runat="server" ID="breadCrumbs" />
        </div>
        <div class="block-d">
            <adv:ProductPhotoView runat="server" ID="productPhotoView" />
            <div class="d-info hproduct">
                <h1 class="product-name fn" <%= InplaceEditor.Meta.Attribute(AdvantShop.SEO.MetaType.Product, CurrentProduct.ID) %>>
                    <%= metaInfo.H1 %></h1>
                <%--<h3 class="brand-art"><%= CurrentProduct.Brand!=null ? CurrentProduct.Brand.Name: "" %></h3>
                <h3 class="brand-art"><%= CurrentProduct.ArtNo %></h3> --%>
                <div id="hrefAdmin" runat="server" visible="false">
                    <a target="_blank" class="details-admin" href="<%= "admin/product.aspx?productid=" + Request["productid"] %>">
                        <% = Resource.Client_Details_Link_ShowInClient %></a>
                </div>
                <div id="pnlPrice" runat="server">
                    <div class="price-c">
                        <% if (CustomerContext.CurrentCustomer.IsAdmin || AdvantShop.Trial.TrialService.IsTrialEnabled)
                           { %>

                        <div id="priceWrap" data-inplace-update="priceDetails">
                            <%= CatalogService.RenderPriceInplace(CurrentOffer.Price, CurrentProduct.CalculableDiscount, true, CustomerContext.CurrentCustomer.CustomerGroup, CustomOptionsService.SerializeToXml(productCustomOptions.CustomOptions, productCustomOptions.SelectedOptions), CurrentOffer.OfferId) %>
                        </div>
                        <% }
                           else
                           { %>
                        <div id="priceWrap">
                            <%= CatalogService.RenderPrice(CurrentOffer.Price, CurrentProduct.CalculableDiscount, true, CustomerContext.CurrentCustomer.CustomerGroup, CustomOptionsService.SerializeToXml(productCustomOptions.CustomOptions, productCustomOptions.SelectedOptions)) %>
                        </div>
                        <% } %>

                        <div class="fpayment">
                            <asp:Label CssClass="first-payment" ID="lblFirstPayment" runat="server" />
                        </div>

                        <asp:Label ID="lblProductBonus" runat="server" />

                        <asp:HiddenField ID="hfFirstPaymentPercent" runat="server" />
                    </div>
                    <div class="prop-str">
                        <div class="param-value inplace-indicator-offset">
                            <asp:Literal ID="lAvailiableAmount" runat="server" />
                        </div>
                    </div>
                </div>
                <adv:Rating ID="rating" runat="server" />
                <div class="prop">
                     <div class="prop-str">
                        <span class="param-name">
                            <%= Resource.Client_Details_SKU %>:</span>
                            <div class="param-value" id="skuValue">
                                <%= DisplaySku %>
                            </div>

                    </div>
                    <div class="prop-str" id="pnlBrand" runat="server">
                        <span class="param-name">
                            <%--<%= Resource.Client_Details_Brand %>:</span><span class="param-value"> <a href="<%= UrlService.GetLink(ParamType.Brand,CurrentProduct.Brand.UrlPath ,CurrentProduct.BrandId) %>">
                                <%= CurrentProduct.Brand.Name %></a></span>--%>
                            <%= Resource.Client_Details_Brand %>:</span><span class="param-value"> <a href="<%= String.Format("categories/catalog?prop={0}", GetCustomBrandID()) %>">
                                <%= CurrentProduct.Brand.Name %></a></span>
                    </div>
                    <div class="category">
                        <%= CurrentProduct.ProductCategories.Count > 0 ? CurrentProduct.ProductCategories[0].Name : string.Empty %>
                    </div>
                    <div class="prop-str" id="pnlSize" runat="server">
                        <span class="param-name">
                            <%= Resource.Client_Details_Size%>:</span><span class="param-value"><%= CurrentProduct.Size.Replace("|", " x ") %>
                            </span>
                    </div>
                    <asp:Panel CssClass="prop-str" ID="pnlWeight" runat="server">
                        <span class="param-name">
                            <%= Resource.Client_Details_Weight%>:</span>
                        <div class="param-value" <%= InplaceEditor.Product.AttibuteWeight(CurrentProduct.ID)%>><%= CurrentProduct.Weight %><span class="js-weight-unit"> <%= Resource.Client_Details_KG %></span></div>
                    </asp:Panel>
                    <adv:ProductBriefPropertiesView ID="productBriefPropertiesView" runat="server" />
                    <div class="prop-str" runat="server" id="divAmount">
                        <span class="param-name param-name-txt">
                            <%= Resource.Client_Details_Amount %>:</span> <span class="param-value"><span class="input-wrap">
                                <%=RenderSpinBox()%></span></span>
                    </div>
                    <asp:Panel CssClass="prop-str" runat="server" ID="divUnit">
                        <div class="param-name"><%= Resource.Client_Details_Unit %>:</div>
                        <div class="param-value" <%= InplaceEditor.Product.AttibuteUnit(CurrentProduct.ID)%>><%= CurrentProduct.Unit %></div>
                    </asp:Panel>
                    <adv:SizeColorPicker runat="server" ID="sizeColorPicker" ManuallyInit="true" />
                    <adv:CustomOptions ID="productCustomOptions" runat="server" />
                </div>
                <asp:Literal runat="server" ID="liProductInformation" />
                <div class="btns-d">
                    <adv:Button ID="btnAdd" runat="server" Size="Big" Text='<%$ Resources:Resource, Client_Details_Add %>' CssClass="greenButton"
                        ValidationGroup="cOptions" />
                    <adv:Button ID="btnAddCredit" runat="server" Type="Buy" Size="Big" CssClass="btn-credit"
                        Text='<%$ Resources:Resource, Client_Details_AddCredit %>' ValidationGroup="cOptions" />
                    <asp:Label ID="lblFirstPaymentNote" CssClass="first-payment-note" runat="server">*Первый взнос</asp:Label>
                    <adv:Button ID="btnOrderByRequest" runat="server" Size="Big" Type="Action" Text='<%$ Resources:Resource, Client_Catalog_OrderByRequest %>'
                        DisableValidation="true" OnClientClick="redirectToPreOrder($(this));" />
                    <adv:BuyInOneClick ID="BuyInOneClick" runat="server" />
                </div>
                
                <div class="clear"></div>
                <div>
                    <adv:Social runat="server" ID="Social" />
                </div>
                
                <div class="clear"></div>
                <div id="dShipping" class="details-delivery" runat="server">
                    <asp:Literal runat="server" ID="liShipping" />
                </div>
                <br />
                <adv:StaticBlock ID="sbDescriptionDetails" runat="server" SourceKey="DescriptionDetails" />
            </div>
        </div>
        <adv:StaticBlock runat="server" ID="details2" SourceKey="details-2" />
    </div>
    </div><!--Закрываем Container-->

    <div class="tabs tabs-hr" data-plugin="tabs">
        <div class="related-alternative-wrap">
            <div class="tabs-headers">
                <div data-tabs-header="true" class="tab-header <%= string.IsNullOrEmpty(CurrentProduct.Description) && string.IsNullOrEmpty(liAdditionalDescription.Text) 
                    && !InplaceEditor.CanUseInplace(RoleActionKey.DisplayCatalog) ? "tab-hidden" : "tab-visible"%>"
                    id="tab-descr">
                    <span class="tab-inside">
                        <%= Resource.Client_Details_Description %></span>
                </div>
                <div data-tabs-header="true" class="tab-header <%= !productPropertiesView.HasProperties && !CustomerContext.CurrentCustomer.IsAdmin ? "tab-hidden" : ""%>"
                    id="tab-property">
                    <span class="tab-inside">
                        <%= Resource.Client_Details_Properties %></span>
                </div>
                <div data-tabs-header="true" class="tab-header" id="tab-video">
                    <span class="tab-inside">
                        <%= Resource.Client_Details_Video %></span>
                </div>
                <% if (SettingsCatalog.AllowReviews)
                   { %>
                <div data-tabs-header="true" class="tab-header" id="tab-review">
                    <span class="tab-inside">
                        <%= Resource.Client_Details_Reviews %>
                        <asp:Literal ID="lReviewsCount" runat="server" /></span>
                </div>
                <% } %>
                <asp:ListView ID="lvTabsTitles" runat="server" ItemPlaceholderID="itemPlaceholderID">
                    <LayoutTemplate>
                        <div runat="server" id="itemPlaceholderID">
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <div data-tabs-header="true" class="tab-header" id="<%# "tab-" + Eval("TabTitleId") %>">
                            <span class="tab-inside">
                                <%# Eval("Title")%></span>
                        </div>
                    </ItemTemplate>
                </asp:ListView>
                <div class="tabs-border">
                </div>
            </div>
        </div>
        <div class="tabs-contents">
            <div class="related-alternative-wrap">
                <div data-tabs-content="true" class="tab-content description">
                    <div <%= InplaceEditor.Product.AttibuteDescription(CurrentProduct.ID)%>>
                        <% = CurrentProduct.Description %>
                    </div>
                    <asp:Literal runat="server" ID="liAdditionalDescription" />
                </div>
                <div data-tabs-content="true" class="tab-content">
                    <adv:ProductPropertiesView ID="productPropertiesView" runat="server" />
                </div>
                <div data-tabs-content="true" class="tab-content">
                    <adv:ProductVideoView ID="ProductVideoView" runat="server" />
                </div>
                <% if (SettingsCatalog.AllowReviews)
                   { %>
                <div data-tabs-content="true" class="tab-content">
                    <adv:ProductReviews ID="productReviews" runat="server" />
                </div>
                <% } %>
                <asp:ListView ID="lvTabsBodies" runat="server" ItemPlaceholderID="itemPlaceholderID">
                    <LayoutTemplate>
                        <div runat="server" id="itemPlaceholderID">
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <div data-tabs-content="true" class="tab-content">
                            <%#Eval("Body") %>
                        </div>
                    </ItemTemplate>
                </asp:ListView>
            </div>
        </div>
    </div>
    <div class="relatedProductsSection" <%= !alternativeProducts.HasProducts ? "style=\" display:none;\"" : ""%>>
        <div class="container">
            <h3 class="addSectionTitle">
                <%= SettingsCatalog.AlternativeProductName %></h3>
            <adv:RelatedProducts runat="server" ID="alternativeProducts" RelatedType="Alternative" />
        </div>
    </div>
    <div class="relatedProductsSection" <%= !relatedProducts.HasProducts ? "style=\" display:none;\"" : ""%>>
        <div class="container">
            <h3 class="addSectionTitle">
                <%= SettingsCatalog.RelatedProductName %></h3>
            <adv:RelatedProducts runat="server" ID="relatedProducts" RelatedType="Related" />
        </div>
    </div>
    <input type="hidden" data-page="details" id="hfProductId" name="hfProductId" value="<%= ProductId %>" />
    <script>
        function redirectToPreOrder(btn) {
            var base = $('base').attr('href'),
                offerid = btn.attr('data-offerid'),
                amount = $('#txtAmount').val(),
                options = $('#customOptionsHidden_' + btn.attr('data-productid')).val();

            window.location.href = base + 'sendrequestonproduct.aspx?offerid=' + offerid + '&amount=' + amount + '&options=' + options;
        }
    </script>
</asp:Content>
