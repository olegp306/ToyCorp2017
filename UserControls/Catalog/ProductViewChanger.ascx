<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductViewChanger.ascx.cs"
    Inherits="UserControls.Catalog.ProductViewChanger" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>
<ul class="views">
    <!--tile-->
    <% if (IsSelectedView(SettingsCatalog.ProductViewMode.Tiles))
       {
    %>
    <li class="selected" title="<%= Resource.Client_Catalog_Tiles %>"><span class="vtiles">
    </span></li>
    <%
        }
       else
       {%>
    <li>
        <asp:LinkButton runat="server" ID="lbTiles" OnClick="lbTiles_Click" ToolTip='<%$Resources:Resource, Client_Catalog_Tiles%>' CssClass="vtiles"></asp:LinkButton></li>
    <%} %>
    <!--list-->
    <% if (IsSelectedView(SettingsCatalog.ProductViewMode.List))
       {
    %>
    <li class="selected" title="<%= Resource.Client_Catalog_List %>"><span class="vlist">
    </span></li>
    <%
        }
       else
       {%>
    <li>
        <asp:LinkButton runat="server" ID="lbList" OnClick="lbList_Click" ToolTip='<%$Resources:Resource, Client_Catalog_List%>' CssClass="vlist"></asp:LinkButton></li>
    <%} %>
    <!--table-->
    <% if (IsSelectedView(SettingsCatalog.ProductViewMode.Table))
       {
    %>
    <li class="selected" title="<%= Resource.Client_Catalog_Table %>"><span class="vtable">
    </span></li>
    <%
        }
       else
       {%>
    <li>
        <asp:LinkButton runat="server" ID="lbTable" OnClick="lbTable_Click" ToolTip='<%$Resources:Resource, Client_Catalog_Table%>'  CssClass="vtable"></asp:LinkButton></li>
    <%} %>
</ul>
