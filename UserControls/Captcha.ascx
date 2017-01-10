<%@ Control Language="C#" CodeFile="Captcha.ascx.cs" Inherits="UserControls.Captcha" ClientIDMode="AutoID" %>
<div class="captcha-wrap">
    <div class="captha-img">
    <input type="hidden" runat="server" id="hfBase64" />
    <input type="hidden" class="valid-captchasource" runat="server" id="hfSource" />
    <img src='<%="httphandlers/captcha/getimg.ashx?captchatext=" + HttpUtility.UrlEncode(Base64Text)%>' alt="" />
    </div>
    <div class="captcha-txt">
        <adv:AdvTextBox ValidationType="Captcha" CssClassWrap="input-wrap-captcha" CssClass="captcha-input" ID="txtValidCode" runat="server" />
    </div>
</div>
