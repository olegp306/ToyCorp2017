<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeFile="err400.aspx.cs" Inherits="ClientPages.err400" EnableViewState="false" %>

<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
    <div class="stroke">
        <div class="content-owner">
            <div class="err-code">
                <div class="err-request-text">
                    <div class="message-title">
                        <% = Resources.Resource.err400_Title%></div>
                    <div class="message-text">
                        <% = Resources.Resource.err400_Message%></div>
                </div>
            </div>
            <div class="err-recommend">
                <div class="split-line">
                </div>
                <div class="text-last">
                    <a href="<%= UrlService.GetAbsoluteLink("/") %>">
                        <% = Resources.Resource.err404_ReturnMessage%></a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
