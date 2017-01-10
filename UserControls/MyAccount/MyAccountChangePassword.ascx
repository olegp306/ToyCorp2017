<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyAccountChangePassword.ascx.cs"
    Inherits="UserControls.MyAccount.ChangePassword" %>

<ul class="form form-vr myacccount-pass-form">
    <li class="title">
        <div class="param-name">
            <%= Resources.Resource.Client_MyAccount_ChangePassword %>
        </div>
        <div class="param-value">
        </div>
    </li>
    <li>
        <div class="param-name">
            <label for="txtNewPassword">
                <%= Resources.Resource.Client_MyAccount_NewPassword %></label></div>
        <div class="param-value">
            <adv:AdvTextBox ID="txtNewPassword" ValidationType="CompareSource" runat="server"
                TextMode="Password" autocomplete="off" DefaultButtonID="btnChangePassword" /></div>
    </li>
    <li>
        <div class="param-name">
            <label for="txtNewPassword">
                <%= Resources.Resource.Client_MyAccount_NewPasswordAgain %></label></div>
        <div class="param-value">
            <adv:AdvTextBox ID="txtNewPasswordConfirm" ValidationType="Compare" runat="server"
                TextMode="Password"  DefaultButtonID="btnChangePassword" /></div>
    </li>
    <li>
        <div class="param-name">
        </div>
        <div class="param-value">
            <adv:Button ID="btnChangePassword" Size="Big" Type="Submit" runat="server" OnClick="btnChangePassword_Click"
                Text="<%$ Resources:Resource, Client_MyAccount_DoChange%>" />
        </div>
    </li>
</ul>
