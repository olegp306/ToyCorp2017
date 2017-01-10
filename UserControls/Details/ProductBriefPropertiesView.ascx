<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductBriefPropertiesView.ascx.cs" Inherits="UserControls.Details.ProductBriefPropertiesView" %>
<asp:ListView runat="server" ID="lvBriefProperties" ItemPlaceholderID="liPlaceholder">
    <ItemTemplate>
        <div class="prop-str">
            <span class="param-name">
                <%# Eval("Property.Name")%></span>
            <div class="param-value">
                <%# Eval("Value")%></div>
        </div>
    </ItemTemplate>
</asp:ListView>
