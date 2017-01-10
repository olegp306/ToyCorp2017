<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductPropertiesView.ascx.cs"
    Inherits="UserControls.Details.ProductPropertiesView" %>
<%@ Import Namespace="AdvantShop.CMS" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<div id="propertiesDetails">
    <asp:ListView runat="server" ID="lvProperties" ItemPlaceholderID="liPlaceholder">
        <LayoutTemplate>
            <ul class="properties">
                <li runat="server" id="liPlaceholder" />
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <%# RenderGroupHeader((PropertyGroup) Eval("Property.Group"))%>
            <li class="properties-row <%# LineCounter%2 == 0 ? string.Empty :  "properties-row-nth" %>">
                <div class="param-name">
                    <%# Eval("Property.Name")%>
                </div>
                <div class="param-value">
                    <% if (ShowInPlaceEditor)
                       { %>
                    <div class="inplace-property-wrap">
                        <input type="text" autocomplete="off" value="<%# Eval("Value").ToString().Replace("\"", "&quot;")%>"
                            <%# InplaceEditor.Property.AttributeEdit((int)Eval("PropertyValueId"), (int)Eval("PropertyId"), ProductId) %> />
                        <%# InplaceEditor.Property.RenderDeleteButton() %>
                    </div>
                    <% }
                       else
                       { %>
                    <%# Eval("Value")%>
                    <% } %>
                </div>
            </li>
        </ItemTemplate>
    </asp:ListView>
</div>
<%if (ShowInPlaceEditor)
  { %>
<ul class="properties">
    <li  class="properties-row">
        <div class="param-name">
            <span class="input-wrap">
                <input id="inplacePropertyName" autocomplete="off" class="inplace-property" placeholder="<%= Resources.Resource.UserControl_ProductPropertiesView_PropertyName %>" <%= InplaceEditor.Property.AttributeAdd(ProductId, InplaceEditor.Property.Field.Name) %>></span>
        </div>
        <div class="param-value">
            <span class="input-wrap">
                <input id="inplacePropertyValue" autocomplete="off" class="inplace-propertyvalue" placeholder="<%= Resources.Resource.UserControl_ProductPropertiesView_PropertyValue %>" <%= InplaceEditor.Property.AttributeAdd(ProductId, InplaceEditor.Property.Field.Value) %>></span>
        </div>
    </li>
</ul>
<%}%>

