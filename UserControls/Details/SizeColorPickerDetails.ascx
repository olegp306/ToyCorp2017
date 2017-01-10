<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SizeColorPickerDetails.ascx.cs" Inherits="UserControls.Details.SizeColorPicker_Details" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<div data-part="sizeColorPickerDetails" <%= ManuallyInit ? "data-manual-init=\"true\"" : "" %>  
     data-productId="<%= Product.ProductId %>" 
     data-orderbyrequest="<%= Product.AllowPreOrder %>"
     data-sizeHeader="<%= SettingsCatalog.SizesHeader %>" 
     data-colorHeader="<%= SettingsCatalog.ColorsHeader %>"
     data-color-image-width="<%= ImageWidth %>"
     data-color-image-height="<%= ImageHeight %>"
     data-selected-offer-id="<%= SelectedOfferId %>">
</div>
