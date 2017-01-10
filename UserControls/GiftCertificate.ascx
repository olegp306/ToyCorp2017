<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftCertificate.ascx.cs"
    Inherits="UserControls.GiftCertificatePrint" %>
<%@ Register TagPrefix="adv" TagName="Logo" Src="~/UserControls/LogoImage.ascx" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>
<%if (isModal)
  { %>
  <div style="display: none;">
<div id="modalGiftCertificate">
    <% } %>
    <div class="certificate pie">
        <div class="header">
            <div class="logo-wrap">
                <adv:Logo runat="server" ID="Logo" ImgHref='/' />
            </div>
            <div class="code-wrap">
                <div class="text">
                    <%= Resource.Client_GiftCertificate_Code%>:</div>
                <div class="code">
                    <asp:Label ID="lblCertificateCode" runat="server"></asp:Label></div>
            </div>
            <div class="clear"></div>
        </div>
        <div class="section pie">
            <div class="cert-data">
                <div class="name">
                    <img src="images/giftcertificate/header_<%=SettingsMain.Language %>.png" alt="" /></div>
                <div class="persons">
                    <div class="person-to">
                        <img src="images/giftcertificate/to_<%=SettingsMain.Language %>.png" alt="" /><asp:Label ID="lblToName" runat="server" /></div>
                    <div class="person-from">
                        <img src="images/giftcertificate/from_<%=SettingsMain.Language %>.png" alt="" /><asp:Label ID="lblFromName" runat="server" /></div>
                    <br class="clear" />
                </div>
                <div class="message">
                    <asp:Label ID="lblMessage" runat="server" />
                </div>
                <div class="cert-price">
                    <asp:Label ID="lblSum" runat="server"></asp:Label>
                </div>
                <div class="heighter"></div>
                <br class="clear"/>
            </div>
            <div class="use">
                <%= Resource.Client_GiftCertificate_CanBeUsed%>
                <span class="site-use">
                    <%= SettingsMain.SiteUrl %></span>
            </div>
        </div>
        <div class="bow-wrap">
            <div class="bow">
            </div>
        </div>
    </div>
    <%if (isModal)
      { %>
</div>
</div>
<% }%>