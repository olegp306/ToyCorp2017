<%@ Control Language="C#" AutoEventWireup="true" CodeFile="News.ascx.cs" Inherits="UserControls.Default.News"
    EnableViewState="false" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<!--noindex-->
<article class="block-uc" data-plugin="expander">
    <h3 class="addSectionTitle" data-expander-control="#expander-news">
        <%= Resources.Resource.Client_UserControls_News_News %></h3>
    <div class="content" id="expander-news">
        <asp:ListView runat="server" ID="lvNews">
            <ItemTemplate>
                <div class="news-item">
                    <a href="<%# UrlService.GetLink(ParamType.News, Eval("UrlPath").ToString() ,SQLDataHelper.GetInt(Eval("NewsID"))) %>"
                        class="newsItemTitle">
                        <%#Eval("Title")%></a>
                    <p class="newsItemArticle"><%#Eval("TextAnnotation")%></p>
                    <div class="news-date">
                        <%# AdvantShop.Localization.Culture.ConvertShortDate((DateTime)Eval("AddingDate"))%>
                    </div>
                </div>
            </ItemTemplate>
        </asp:ListView>
    </div>
    <a href="news" style="display:none;">
            <%= Resources.Resource.Client_UserControls_News_AllNews %></a>
</article>
<!--/noindex-->
