<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OpenidParagrafView.ascx.cs"
    Inherits="ClientPages.install_UserContols_OpenidParagrafView" %>
<h1>
    <% = Resources .Resource .Install_UserContols_OpenidParagrafView_h1 %></h1>
<h2>
    <%= Resources .Resource .Install_UserContols_OpenidParagrafView_h2 %>
</h2>
<asp:Label runat="server" ID="lblError" ForeColor="Red"></asp:Label>
<div class="group">
    <p>
        <%= Resources .Resource .Install_UserContols_OpenidParagrafView_Email %>
    </p>
    <div class="str">
        <asp:TextBox runat="server" CssClass="txt valid-required valid-login" ID="txtLogin"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <%= Resources .Resource .Install_UserContols_OpenidParagrafView_Pass %>
    </p>
    <div class="str">
        <asp:TextBox runat="server" CssClass="txt valid-required valid-comparesource" ID="txtPass"
            TextMode="Password"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% =Resources .Resource .Install_UserContols_OpenidParagrafView_RepeatPass %>
    </p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-compare" runat="server" ID="txtPassAgain" TextMode="Password"></asp:TextBox>
    </div>
</div>
<h2>
    <% = Resources.Resource.Install_UserContols_OpenidParagrafView_h2_2%>
</h2>
<fieldset class="group simple" runat="server" id="fieldsetYandex">
    <legend>
        <asp:CheckBox runat="server" ID="chbYandex" /><label for="<%=chbYandex.ClientID %>"><img
            src="../install/images/icons/yandex.png" />Yandex</label></legend>
</fieldset>
<fieldset class="group simple" runat="server" id="fieldsetMailru">
    <legend>
        <asp:CheckBox runat="server" ID="chbMailru" />
        <label for="<%= chbMailru.ClientID %>">
            <img src="../install/images/icons/mail_ru.png" />
            Mail.ru</label></legend>
</fieldset>
<fieldset class="group" runat="server" id="fieldsetGoogle">
    <legend>
        <asp:CheckBox runat="server" ID="chbGoogle" />
        <label for="<%=chbGoogle.ClientID %>">
            <img src="../install/images/icons/google.png" />
            Google</label></legend>
    <div class="block-options">
        <p>
            Client Id
        </p>
        <div class="str">
            <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtGoogleClientID"></asp:TextBox>
        </div>
        <p>
            Application Secret
        </p>
        <div class="str">
            <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtGoogleClientSecret"></asp:TextBox>
        </div>
         <p>
            Redirect URL
        </p>
        <div class="str">
            <asp:Label runat="server" ID="lblGoogleRedirectUrl" />
        </div>
    </div>
</fieldset>
<fieldset class="group" runat="server" id="fieldsetFacebook">
    <legend>
        <asp:CheckBox runat="server" ID="chbFacebook" />
        <label for="<%=chbFacebook.ClientID %>">
            <img src="../install/images/icons/facebook.png" />
            Facebook
        </label>
    </legend>
    <div class="block-options">
        <p>
            Client Id
        </p>
        <div class="str">
            <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtFacebookClientId"></asp:TextBox>
        </div>
        <p>
            Application Secret
        </p>
        <div class="str">
            <asp:TextBox runat="server" CssClass="txt valid-required" ID="txtFacebookApplicationSecret"></asp:TextBox>
        </div>
    </div>
</fieldset>
<fieldset class="group" runat="server" id="fieldsetVk">
    <legend>
        <asp:CheckBox runat="server" ID="chbVk" />
        <label for="<%= chbVk.ClientID %>">
            <img src="../install/images/icons/vk.png" />
            Вконтакте</label>
    </legend>
    <div class="block-options">
        <p>
            <asp:Literal ID="ltVkappid" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_HeadOAuthVKappid %>"></asp:Literal>
        </p>
        <div class="str">
            <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtVKAppId"></asp:TextBox>
        </div>
        <p>
            <asp:Literal ID="ltHeadOAuthVKSecretKey" runat="server" Text="<%$ Resources: Resource, Admin_CommonSettings_HeadOAuthVKSecretKey %>"></asp:Literal>
        </p>
        <div class="str">
            <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtVKSecret"></asp:TextBox>
        </div>
    </div>
</fieldset>

