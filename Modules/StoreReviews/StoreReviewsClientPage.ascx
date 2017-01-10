<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StoreReviewsClientPage.ascx.cs"
    Inherits="Advantshop.Modules.UserControls.StoreReviews.StoreReviewsClientPage" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Modules" %>
<link rel="stylesheet" href="modules/storereviews/module-css/storereviews.css" />
<h1>
    <asp:Literal runat="server" ID="ltHeader" Text='<%$ Resources: StoreReviews_StoreReviews%>' /> 
</h1>
<div class="shop-reviews">
    <asp:ListView ID="lvStoreReviews" runat="server" ItemPlaceholderID="itemPlaceHolderId"
        OnPagePropertiesChanging="lvStoreReviews_PagePropertiesChanging" OnPreRender="lvStoreReviews_PreRender">
        <LayoutTemplate>
            <div runat="server" id="itemPlaceHolderId">
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <div class="shop-reviews-wrap pie" data-sr-item="<%# Eval("Id") %>" data-sr-parentid="<%# Eval("ParentId") %>">
                <asp:Panel CssClass="shop-reviews-row" runat="server" Visible='<%# Convert.ToDouble(Eval("Rate").ToString()) > 0 && ShowRatio %>'>
                    <div class="shop-reviews-rating" data-sr-rating="<%# Convert.ToDouble(Eval("Rate")) %>">
                    </div>
                </asp:Panel>
                <div class="shop-reviews-row">
                    <asp:Label CssClass="shop-reviews-name" ID="lblEmail" runat="server" Text='<%# Eval("ReviewerName") %>'></asp:Label>
                    <asp:Label CssClass="shop-reviews-date" ID="lblDate" runat="server" Text='<%# ((DateTime)Eval("DateAdded")).ToString("dd MMMM yyyy, HH:mm") %>'></asp:Label>
                </div>
                <div class="shop-reviews-row shop-reviews-text">
                    <asp:Label ID="ltReview" runat="server" Text='<%# Eval("Review") %>'></asp:Label>
                </div>
                <div data-sr-form-btn="true">
                    <a href="javascript:void(0);" data-sr-reply="true">
                        <%= GetLocalResourceObject("StoreReviews_Answer") %></a>
                </div>
                <%#  (bool)Eval("HasChild") ? RenderChild((List<StoreReview>)Eval("ChildrenReviews")) : ""%>
            </div>
        </ItemTemplate>
    </asp:ListView>
    <div class="shop-reviews-paging" data-sr-paging="true">
        <asp:DataPager ID="dp" runat="server" PagedControlID="lvStoreReviews" PageSize="2"
            ViewStateMode="Enabled" OnPagePropertiesChanging="dp_PagePropertiesChanging">
            <Fields>
                <asp:TemplatePagerField>
                    <PagerTemplate>
                        <span class="shop-reviews-key">Ctrl + &larr; </span>
                    </PagerTemplate>
                </asp:TemplatePagerField>
                <asp:NextPreviousPagerField ShowFirstPageButton="False" ShowNextPageButton="False"
                    ShowLastPageButton="False" ShowPreviousPageButton="True" PreviousPageText="<%$ Resources: StoreReviews_Previous%>" />
                <asp:NumericPagerField CurrentPageLabelCssClass="shop-reviews-selected" NumericButtonCssClass="shop-reviews-page" />
                <asp:NextPreviousPagerField ShowFirstPageButton="False" ShowNextPageButton="True"
                    ShowLastPageButton="False" ShowPreviousPageButton="False" NextPageText="<%$ Resources: StoreReviews_Next%>" />
                <asp:TemplatePagerField>
                    <PagerTemplate>
                        <span class="shop-reviews-key">Ctrl + &rarr;</span>
                    </PagerTemplate>
                </asp:TemplatePagerField>
            </Fields>
        </asp:DataPager>
    </div>
</div>
<div class="shop-reviews-form-wrap">
    <div class="shop-reviews-form-title">
        <%= GetLocalResourceObject("StoreReviews_FormTitle")%></div>
    <ul class="shop-reviews-form">
        <% if (ShowRatio)
           { %>
        <li class="shop-reviews-form-row">
            <div class="shop-reviews-form-rate">
            </div>
            <div id="hint">
            </div>
            <input type="hidden" runat="server" id="hfScope" name="hfScope" value="0" />
        </li>
        <% } %>
        <li class="shop-reviews-form-row">
            <label for="txtReviewerName">
                <%= GetLocalResourceObject("StoreReviews_FormName")%></label>
            <div class="shop-reviews-form-input">
                <div class="input-wrap">
                    <asp:TextBox ID="txtReviewerName" data-plugin="validelem" data-validelem-group="StoreReviews"
                        data-validelem-methods="['required']" runat="server" /></div>
            </div>
        </li>
        <li class="shop-reviews-form-row">
            <label for="txtEmail">
                <%= GetLocalResourceObject("StoreReviews_ReviewerEmail") %></label>
            <div class="shop-reviews-form-input">
                <div class="input-wrap">
                    <asp:TextBox ID="txtEmail" runat="server" data-plugin="validelem" data-validelem-group="StoreReviews"
                        data-validelem-methods="['required', 'email']"></asp:TextBox></div>
            </div>
        </li>
        <li class="shop-reviews-form-row">
            <label for="txtReview">
                <%= GetLocalResourceObject("StoreReviews_Review")%></label>
            <div class="shop-reviews-form-input">
                <div class="textarea-wrap">
                    <asp:TextBox ID="txtReview" runat="server" TextMode="MultiLine" data-validelem-group="StoreReviews"
                        data-plugin="validelem" data-validelem-methods="['required']"></asp:TextBox></div>
            </div>
        </li>
        <li runat="server" id="liError" class="shop-reviews-li-error"></li>
        <li class="shop-reviews-form-row">
            <asp:LinkButton CssClass="btn btn-submit btn-middle" ID="btn" runat="server" OnClick="btnClick"
                Text='<%$Resources: StoreReviews_Send  %>' data-validelem-btn="StoreReviews"></asp:LinkButton>
        </li>
    </ul>
</div>
<script src="modules/StoreReviews/module-js/module-localization/<%=SettingsMain.Language %>.js"></script>
<script src="modules/StoreReviews/module-js/module-rate/jquery.ratyStoreReviews.js"></script>
<script src="modules/StoreReviews/module-js/module-validate/validate.js"></script>
<script src="modules/StoreReviews/module-js/StoreReviews.js"></script>
