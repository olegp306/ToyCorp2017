<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CompareControl.ascx.cs"
    Inherits="UserControls.Catalog.CompareUserControl" %>
<%@ Import Namespace="Resources" %>

<div class="compare-wrap compare-type-<%=Type.ToString().ToLower() %> <%=IsSelected ? "compare-selected-type-" + Type.ToString().ToLower() : "" %> <%= CssClassContainer%>">
    <input class="compare-checkbox" 
        data-plugin="compare" 
        data-compare-animation-obj="<%= AnimationObj %>"
        type="checkbox" 
        id="<%= "chk_" + OfferId %>" <%= IsSelected ? "checked=checked" : "" %>
        value="<%=OfferId %>" 
        data-compare-options='<%= GetOptions() %>' />
    <label class="compare-label" for="<%= "chk_" + OfferId %>">
        <%= IsSelected ? Resource.Client_Catalog_AlreadyCompare + " (<a href='compareproducts.aspx' target='_blank'>" + Resource.Client_Compare_View + "</a>)" :  "<a href=\"\">" + Resource.Client_Catalog_Compare + "</a>"%>
    </label>
</div>


