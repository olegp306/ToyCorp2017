<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductPhotoView.ascx.cs"
    Inherits="UserControls.Details.ProductPhotoView" %>
<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Customers" %>
<%@ Import Namespace="AdvantShop.FilePath" %>
<%@ Import Namespace="AdvantShop.CMS" %>
<%@ Import Namespace="Resources" %>
<div class="d-photo" runat="server" id="pnlPhoto">
    <div class="d-c-photo">
        <div class="preview-image">
            <table>
                <tr>
                    <td class="preview-cell" style="height: <%= SettingsPictureSize.MiddleProductImageHeight %>px">
                        <% if (SettingsDesign.EnableZoom)
                           {%>
                        <a id="zoom" href="<%= FoldersHelper.GetImageProductPath(ProductImageType.Big, MainPhoto.PhotoName, false) %>"
                            class="zoom cloud-zoom cloud-zoom-progress" rel="{zoomWidth:<%= SettingsPictureSize.MiddleProductImageWidth %>, zoomHeight:<%= SettingsPictureSize.MiddleProductImageHeight%>}" style="width: <%= SettingsPictureSize.MiddleProductImageWidth%>px;" data-has-photo="<%= Product.ProductPhotos.Count > 0 ? "true" : "false"%>">
                        <% }
                           else
                           {%>
                        <a id="zoom" href="<%= FoldersHelper.GetImageProductPath(ProductImageType.Big, MainPhoto.PhotoName, false) %>" style="height: <%= SettingsPictureSize.MiddleProductImageHeight%>px; width: <%= SettingsPictureSize.MiddleProductImageWidth%>px;" data-has-photo="<%= Product.ProductPhotos.Count > 0 ? "true" : "false"%>">
                            <% } %>
                            <span class="pic-details-wrap" style="position: relative; display: inline-block;">
                                <img id="preview-img" src="<%= FoldersHelper.GetImageProductPath(ProductImageType.Middle, MainPhoto.PhotoName, false) %><%= InplaceEditor.CanUseInplace(RoleActionKey.DisplayCatalog)  ? "?"+ (new Random().Next()):"" %>" 
                                     class="<%= InplaceEditor.CanUseInplace(RoleActionKey.DisplayCatalog) ? "js-inplace-image-visible-permanent" : "" %>" 
                                    <%= InplaceEditor.Image.AttributesProduct(MainPhoto.PhotoId, Product.ProductId,ProductImageType.Middle, true, MainPhoto.PhotoName.IsNotEmpty(), MainPhoto.PhotoName.IsNotEmpty()) %>
                                    <%= string.Format("alt=\"{0}\" title=\"{0}\"", HttpUtility.HtmlEncode(MainPhoto.Description.IsNotEmpty()? MainPhoto.Description : Product.Name)) %> /></span>
                            <%= CatalogService.RenderLabels(Product.Recomended, Product.OnSale, Product.BestSeller, Product.New, Product.Discount, customLabels: Labels, warranty:Product.ManufacturerWarranty)%>
                       </a>
                    </td>
                </tr>
            </table>
            <% if (Product.ProductPhotos.Count > 0)
               { %>
            <div class="zoom-c">
                <img src="images/icons/zoom.png" alt="" class="zoom-link" id="icon-zoom" />
                <a class="link-zoom" href="<%= FoldersHelper.GetImageProductPath(ProductImageType.Big, MainPhoto.PhotoName, false) %>" id="link-fancybox">
                    <% = Resource.Client_Details_Zoom %></a>
            </div>
            <% } %>
        </div>
        <div id="carouselDetails" runat="server">
            <div id="flexsliderDetails" class="flexslider-carousel flexslider-carousel-small"
                data-plugin="flexslider" data-flexslider-autobind="false" data-flexslider-options="{ controlNav:false, animation: 'slide', minItems: 1, maxItems:4, itemWidth: <%=SettingsPictureSize.XSmallProductImageWidth %>, itemMargin: 10, move:1, animationLoop:false, slideshow:false }">
                <asp:ListView runat="server" ID="lvPhotos" ItemPlaceholderID="liItemPlaceholder">
                    <LayoutTemplate>
                        <ul class="slides">
                            <li runat="server" id="liItemPlaceholder" />
                        </ul>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <li data-color-id="<%# Eval("ColorID") %>" data-photo-id="<%# Eval("PhotoId") %>">
                            <div class="flexslider-carousel-item-wrap"><figure class="flexslider-carousel-item <%# (bool)Eval("Main") ? "selected" : string.Empty %>"
                                    style="<%= "width:" + SettingsPictureSize.XSmallProductImageWidth + "px;"%>; <%= "height:" + SettingsPictureSize.XSmallProductImageHeight + "px;"%>" data-imagepicker-caller="true"
                                    data-imagepicker-group="details" data-imagepicker-img="{
                                       middle : {src: '<%# FoldersHelper.GetImageProductPath(ProductImageType.Middle, Eval("PhotoName").ToString(), false) %>', place : '#preview-img', title: '<%# GetStringEncoded(Eval("Description").ToString().IsNotEmpty() ? Eval("Description").ToString() : string.Format("{0} - {1} {2}", Product.Name, Resource.ClientPage_AltText, Eval("PhotoId"))) %>'},
                                       big : {src: '<%# FoldersHelper.GetImageProductPath(ProductImageType.Big, Eval("PhotoName").ToString(), false) %>', place : '#zoom', title: '<%# GetStringEncoded(Eval("Description").ToString().IsNotEmpty() ? Eval("Description").ToString() : string.Format("{0} - {1} {2}", Product.Name, Resource.ClientPage_AltText, Eval("PhotoId"))) %>'}
                                    }"><img src="<%# FoldersHelper.GetImageProductPath(ProductImageType.XSmall, Eval("PhotoName").ToString(), false) %><%= CustomerContext.CurrentCustomer.IsAdmin || AdvantShop.Trial.TrialService.IsTrialEnabled  ? "?"+ (new Random().Next()):"" %>"
                                        <%# string.Format("alt=\"{0}\" title=\"{0}\"", HttpUtility.HtmlEncode(Eval("Description").ToString().IsNotEmpty() ? Eval("Description").ToString() : string.Format("{0} - {1} {2}", Product.Name, Resource.ClientPage_AltText, Eval("PhotoId")))) %> /></figure></div>
                        </li>
                    </ItemTemplate>
                </asp:ListView>
            </div>
        </div>
    </div>
</div>

<div class="d-photo" id="pnlNoPhoto" runat="server">
    <div class="d-c-photo">
        <div class="preview-image">
            <figure class="preview-image-nophoto" style="width: <%=SettingsPictureSize.MiddleProductImageWidth %>px; height: <%= SettingsPictureSize.MiddleProductImageHeight %>px">
                <img class="inplace-controls-pos-outside-bottom <%= InplaceEditor.CanUseInplace(RoleActionKey.DisplayCatalog) ? "js-inplace-image-visible-permanent" : "" %>" id="preview-img" src="images/nophoto.jpg" alt="" <%= InplaceEditor.Image.AttributesProduct(-1, Product.ProductId,ProductImageType.Middle, true, false, false) %> />
                <%= CatalogService.RenderLabels(Product.Recomended, Product.OnSale, Product.BestSeller, Product.New, Product.Discount)%>
            </figure>
        </div>
    </div>
</div>
