<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GiftCertificate.ascx.cs" Inherits="UserControls.Default.GiftCertificate" EnableViewState="false" %>
<!--noindex-->
<div class="block-uc block-certificate" onclick="location.href='giftcertificate.aspx'">
    <img src="images/giftcertificate/certifacate_bow.jpg" class="certificate-img" alt="<%= Resources.Resource.Client_MasterPage_Certificate%>" />
    <div class="block-certificate-txt">
        <asp:Label runat="server" Text="<%$ Resources:Resource, Client_MasterPage_Certificates%>"></asp:Label></div>
</div>
<!--/noindex-->