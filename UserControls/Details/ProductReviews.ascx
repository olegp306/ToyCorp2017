<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductReviews.ascx.cs" Inherits="UserControls.Details.ProductReviews" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Customers" %>
<div class="reviews" data-plugin="reviews" data-reviews-options="{entityId:<%= EntityId %>, entityType: <%= (int)EntityType %>, moderate: <%= SettingsCatalog.ModerateReviews.ToString().ToLower() %>}" 
 data-userName="<%=CustomerContext.CurrentCustomer.RegistredUser ? CustomerContext.CurrentCustomer.FirstName + " " + CustomerContext.CurrentCustomer.LastName : string.Empty %>" data-userEmail="<%= CustomerContext.CurrentCustomer.EMail %>">
    <asp:ListView runat="server" ID="lvReviews">
        <ItemTemplate>
            <div class="review-item hReview">
                <div>
                    <span class="type item">product</span>
                    <span class="dtreviewed"><%# ((DateTime)Eval("AddDate")).ToString("yyyy-MM-dd")%></span>
                    <span class="permalink"><%= SettingsMain.SiteUrl + Request.RawUrl%></span>
                    <div class="author">
                        <span class="reviewer"><%# Eval("Name") %></span><span class="date">
                            <%# ((DateTime)Eval("AddDate")).ToString(SettingsMain.ShortDateFormat + " - HH:mm")%></span>
                    </div>
                    <div class="message description">
                        <%# Eval("Text")%>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:ListView>
</div>