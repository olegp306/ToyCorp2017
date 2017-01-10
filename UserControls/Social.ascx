<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Social.ascx.cs" Inherits="UserControls.Social" %>
<asp:MultiView runat="server" ID="mvSocial">
    <asp:View runat="server" ID="vSocialDefault_Ru">
         <div class="share42init"></div>
        <script type="text/javascript" src="js/plugins/share42/share42.js"></script> 
    </asp:View>
    <asp:View runat="server" ID="vSocialDefault_En">
        <!-- AddThis Button BEGIN -->
        <div class="addthis_toolbox addthis_default_style addthis_32x32_style">
        <a class="addthis_button_facebook"></a>
        <a class="addthis_button_twitter"></a>
        <a class="addthis_button_pinterest_share"></a>
        <a class="addthis_button_google_plusone_share"></a>
        <a class="addthis_button_compact"></a><a class="addthis_counter addthis_bubble_style"></a>
        </div>
        <script type="text/javascript" defer>var addthis_config = { "data_track_addressbar": true };</script>
        <script type="text/javascript" src="//s7.addthis.com/js/300/addthis_widget.js#pubid=ra-4e2582ed5550ffee" defer></script>
        <!-- AddThis Button END -->
    </asp:View>
    <asp:View runat="server" ID="vSocialCustom">
        <%= AdvantShop.Configuration.SettingsSocial.SocialShareCustomCode %>
    </asp:View>
</asp:MultiView>