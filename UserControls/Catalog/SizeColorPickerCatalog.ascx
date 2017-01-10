<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SizeColorPickerCatalog.ascx.cs" Inherits="UserControls.Catalog.SizeColorPicker" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<div data-productid="<%= ProductId %>"
    data-part="sizeColorPickerCatalog"
    data-colors="<%= Colors %>"
    data-color-id-active="<%= DefaultColorID %>"
    data-color-image-width="<%= ImageWidth %>"
    data-color-image-height="<%= ImageHeight %>"
    >
</div>
