<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LoginOpenID.ascx.cs" Inherits="UserControls.LoginOpenID" %>
<div class="auth-social">
    <div class="title">
        <%= Resources.Resource.Client_Login_LoginVia %></div>
    <div>
        <a href="javascript:void(0)" id="lnkbtnGoogle" runat="server" onserverclick="lnkbtnGoogleClick">
            <img src="images/buttons/gl.png" alt="Google" title="Google" /></a> <a href="javascript:void(0)"
                id="lnkbtnYandex" runat="server" onserverclick="lnkbtnYandexClick">
                <img src="images/buttons/ya.png" alt="Yandex" title="Yandex" /></a> <%--<a href="javascript:void(0)"
                    id="lnkbtnTwitter" runat="server" onserverclick="lnkbtnTwitterClick">
                    <img src="images/buttons/tw.png" alt="Twitter" title="Twitter" /></a>--%>
        <a href="javascript:void(0)" id="lnkbtnFacebook" runat="server" onserverclick="lnkbtnFacebookClick">
            <img src="images/buttons/fb.png" alt="Facebook" title="Facebook" /></a> <a href="javascript:void(0)"
                id="lnkbtnVk" runat="server" onserverclick="lnkbtnVkClick">
                <img src="images/buttons/vk.png" alt="Вконтакте" title="Вконтакте" /></a>
        <a href="javascript:void(0)" id="lnkbtnMail" runat="server">
            <img src="images/buttons/ml.png" alt="Mail.ru" title="Mail.ru" /></a> <a href="javascript:void(0)"
                id="lnkbtnOdnoklassniki" runat="server"  onserverclick="lnkbtnOdnoklassnikiClick">
                <img src="images/buttons/od.png" alt="Одноклассники" title="Одноклассники" /></a>
        <br />
    </div>
</div>
<div style="display: none;" runat="server" id="pnlMail">
    <div id="modalMail" class="modalDiv">
        <br />
        <adv:AdvTextBox ValidationType="None" ID="txtOauthUserId" DefaultButtonID="btnOpenID"
            runat="server" />
        <br />
        <br />
        <div style="text-align: right;">
            <adv:Button ID="btnOpenID" runat="server" OnClick="lnkbtnMailClick" Type="Confirm"
                Size="Middle" Text="OK" OnClientClick="if(!$('#txtOauthUserId').val().length) return false;"
                DisableValidation="True" />
        </div>
    </div>
</div>