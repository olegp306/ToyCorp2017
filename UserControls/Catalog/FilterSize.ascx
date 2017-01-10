<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilterSize.ascx.cs" Inherits="UserControls.Catalog.FilterSize" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<!--noindex-->
<article class="block-uc-inside" data-plugin="expander">
    <h4 class="title" data-expander-control="#size-filter">
        <% =SettingsCatalog.SizesHeader%>
    </h4>
    <div class="content" id="size-filter" >
        <div class="chb-list size-filter">
            <asp:ListView runat="server" ID="lvSizes">
                <ItemTemplate>
                    <div>
                        <input type="checkbox" id="<%# "size_" + Eval("SizeID")%>" <%# AvalibleSizesIDs != null && !AvalibleSizesIDs.Contains(SQLDataHelper.GetInt(Eval("SizeID"))) ? "disabled=\"disabled\"" : string.Empty %>
                            <%# SelectedSizesIDs != null && SelectedSizesIDs.Contains(SQLDataHelper.GetInt(Eval("SizeID"))) ? "checked=\"checked\"" : string.Empty %>
                            value="<%# Eval("SizeID")%>" />
                        <label for="<%# "size_" + Eval("SizeID")%>">
                            <%# Eval("SizeName")%></label>
                    </div>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </div>
</article>
<!--/noindex-->
