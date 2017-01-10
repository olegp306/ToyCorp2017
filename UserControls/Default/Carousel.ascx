<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Carousel.ascx.cs" Inherits="UserControls.Default.Carousel" %>
<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="AdvantShop.CMS" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.FilePath" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="AdvantShop.Customers" %>
<!--slider-->
<div class="flexslider<%= CssSlider.IsNotEmpty() ? " " + CssSlider : "" %>" 
    <%= (CustomerContext.CurrentCustomer.IsAdmin || AdvantShop.Trial.TrialService.IsTrialEnabled) &&  !SettingsMain.EnableInplace && CarouselsCount == 0 ? "hidden"  : "" %> 
    data-plugin="flexslider" 
    data-flexslider-options="<%= "{animation:'" + SettingsDesign.CarouselAnimation + "', animationSpeed:" + SettingsDesign.CarouselAnimationSpeed + ", slideshowSpeed:" + SettingsDesign.CarouselAnimationDelay + "}" %>">
    <ul class="slides">
        <% foreach (var item in CarouselService.GetAllCarouselsMainPage()) {%>
        <li>
            <a href="<%=item.URL %>">
            <img <%= InplaceEditor.Image.AttributesCarousel(item.CarouselID) %> class="<%= InplaceEditor.CanUseInplace(RoleActionKey.DisplayCarousel) ? "js-inplace-image-visible-permanent" : "" %>" src="<%= File.Exists(FoldersHelper.GetPathAbsolut(FolderType.Carousel, item.Picture.PhotoName)) 
                                    ? FoldersHelper.GetPath(FolderType.Carousel, item.Picture.PhotoName , false  )
                                    : "images/nophoto_carousel_"+ SettingsMain.Language +".jpg" %>"
                alt="<%= HttpUtility.HtmlEncode(item.Picture.Description)%>"  <%= InplaceEditor.Image.AttributesControls(true, true, true) %> /></a></li>
        <%}%>
        <%if (CarouselsCount == 0 && (CustomerContext.CurrentCustomer.IsAdmin || AdvantShop.Trial.TrialService.IsTrialEnabled))
          { %>
        <li>
            <img id="carouselNoPhoto" <%= InplaceEditor.Image.AttributesCarousel(-1) %> src="images/nophoto_carousel_<%= SettingsMain.Language %>.jpg" alt="" <%= InplaceEditor.Image.AttributesControls(true, false, false) %> />
            <span class="inplace-slider-control js-inplace-slider-emulate"></span>
        </li>
        <%} %>
    </ul>
</div>
<!--end_slider-->
