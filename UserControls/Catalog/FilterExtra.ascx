<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilterExtra.ascx.cs" Inherits="UserControls.Catalog.UserControls_FilterExtra" %>
<!--noindex-->
<article class="block-uc-inside">
    <div class="title" data-expander-control="#aAvaliable"><%= Resources.Resource.Client_Catalog_Availability %></div>
    <div class="content" id="extra-filter">
        <div class="chb-list">
            <div id="divAvaliable" runat="server">
                <input type="checkbox" id="cbAvailable" <%=  AvailableSelected ? "checked=\"checked\"" : string.Empty %>  />
                <label for="cbAvailable"><%= Resources.Resource.Client_Catalog_InStock %></label>
            </div>
            <div id="divPreorder" runat="server">
                <input type="checkbox" id="cbPreorder" <%=  PreOrderSelected ? "checked=\"checked\"" : string.Empty %> />
                <label for="cbPreorder"><%= Resources.Resource.Client_Catalog_ForPreOrder %></label>
            </div>
        </div>
    </div>
</article>

<!--/noindex-->
