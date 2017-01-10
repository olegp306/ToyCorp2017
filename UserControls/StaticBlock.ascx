<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StaticBlock.ascx.cs" Inherits="UserControls.StaticBlock"
    EnableViewState="false" %>
<div class="static-block <%= CssClass %><%= DisableInplaceEditor ? " inplace-disabled" : "" %>" <%= AdvantShop.CMS.InplaceEditor.StaticBlock.Attribute(sb.StaticBlockId) %>>
    <%= GetContent() %>
</div>