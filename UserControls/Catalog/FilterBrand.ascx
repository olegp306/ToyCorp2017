<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilterBrand.ascx.cs" Inherits="UserControls.Catalog.FilterBrand" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<!--noindex-->
<article class="block-uc-inside" data-plugin="expander">
    <div class="title" data-expander-control="#brand-filter">
        <% =Resources.Resource.Client_Catalog_Brands%>
    </div>
    <div class="content" id="brand-filter" hidden>
        <div class="chb-list brandList">
        <asp:ListView runat="server" ID="lvBrands">
            <ItemTemplate>
                <div>
                    <input type="checkbox" id="<%# "brand_" + Eval("BrandID")%>" <%#AvalibleBrandIDs != null && !AvalibleBrandIDs.Contains(SQLDataHelper.GetInt(Eval("BrandID"))) ? "disabled=\"disabled\"" : string.Empty %>
                        <%#SelectedBrandIDs != null && SelectedBrandIDs.Contains(SQLDataHelper.GetInt(Eval("BrandID"))) ? "checked=\"checked\"" : string.Empty %>
                        value="<%# Eval("BrandID")%>" />
                    <label for="<%# "brand_" + Eval("BrandID")%>">
                        <%# Eval("Name")%></label>
                </div>
            </ItemTemplate>
        </asp:ListView>
        </div>
    </div>
</article>
<!--/noindex-->