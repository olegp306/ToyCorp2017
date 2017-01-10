<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilterColor.ascx.cs" Inherits="UserControls.Catalog.FilterColor" %>
<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<!--noindex-->
<article class="block-uc-inside" data-plugin="expander">
    <div class="title" data-expander-control="#color-filter">
        <% =SettingsCatalog.ColorsHeader%>
    </div>
    <div class="content" id="color-filter">
        <div class="chb-list color-filter">
            <asp:ListView runat="server" ID="lvColors">
                <ItemTemplate>
                        <label class="pie filter-color <%# !Eval("ColorCode").ToString().IsNotEmpty() ? "filter-color-none" : ""%>" 
                            style="<%#"width:" +  ColorImageWidth +"px;height:" + ColorImageHeight + "px;" + (Eval("IconFileName") != null ? "background-image:url('pictures/color/catalog/" + Eval("IconFileName.PhotoName")+ "')" :  "background:" +  Eval("ColorCode"))%>" 
                            title="<%# Eval("ColorName")%>">
                            <input class="filter-color-input-hidden" type="checkbox" id="<%# "color_" + Eval("ColorID")%>"
                                <%#SelectedColorsIDs != null && SelectedColorsIDs.Contains(SQLDataHelper.GetInt(Eval("ColorID"))) ? "checked=\"checked\"" : string.Empty %>
                                value="<%# Eval("ColorID")%>" />
                </label>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </div>
</article>
<!--/noindex-->
