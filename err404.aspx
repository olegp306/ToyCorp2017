<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeFile="err404.aspx.cs" Inherits="ClientPages.err404" EnableViewState="false" %>

<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>

<%@ Register TagPrefix="adv" TagName="MainPageProduct" Src="UserControls/Default/MainPageProduct.ascx" %>
<%@ Register TagPrefix="adv" TagName="News" Src="~/UserControls/Default/News.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
    <div class="stroke">
        <div class="content-owner">
            <div class="err-code">
                <div class="err-request"></div>
                <div style="clear: both;"></div>
            </div>
            <div class="err-recommend">
                <div class="err-request-text">
                    <div class="message-text">
                        <% = Resources.Resource.err404_Message%></div>
                </div>
                <% = Resources.Resource.err404_PossibleReasons%>:
                <ul>
                    <li>
                        <% = Resources.Resource.err404_PageWasDeleted %></li>
                    <li>
                        <% = Resources.Resource.err404_IncorrectLinkToThePage %></li>
                    <li>
                        <% = Resources.Resource.err404_IncorrectTypeOrAddress %></li>
                </ul>
                <div class="text-last">
                    <a href="<%= UrlService.GetAbsoluteLink("/") %>">
                        <% = Resources.Resource.err404_ReturnMessage%></a>
                </div>
            </div>
            <div style="clear: both;"></div>
        </div>
    </div>
    
    </div>
    <div>
    <!--Закрытие Container-->

    <%--<%= MainPageProducts%>--%>
    <adv:MainPageProduct ID="mainPageProduct" runat="server" Mode="Default" />
    <div class="newsSection">
        <div class="container">
            <adv:News ID="news" runat="server" />
        </div>
    </div>
</asp:Content>
