<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NotifyParagrafView.ascx.cs"
    Inherits="ClientPages.install_UserContols_NotifyParagrafView" %>
<h1>
    <%= Resources .Resource .Install_UserContols_NotifyParagrafView_h1 %></h1>
<h2>
    <% = Resources .Resource .Install_UserContols_NotifyParagrafView_h2 %></h2>
<div class="group">
    <p>
        <% = Resources .Resource .Install_UserContols_NotifyParagrafView_RegReport %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-email" runat="server" ID="txtEmailRegReport"></asp:TextBox></div>
</div>
<div class="group">
    <p>
        <%= Resources .Resource .Install_UserContols_NotifyParagrafView_Order %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-email" runat="server" ID="txtOrderEmail"></asp:TextBox></div>
</div>
<div class="group">
    <p>
        <% = Resources.Resource.Install_UserContols_NotifyParagrafView_ProductDiscuss%></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-email" runat="server" ID="txtEmailProductDiscuss"></asp:TextBox></div>
</div>
<%--<div class="group">
    <p>
        <% = Resources .Resource.Install_UserContols_NotifyParagrafView_ProductQuestion %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-email" runat="server" ID="txtEmailProductQuestion"></asp:TextBox></div>
</div>--%>
<div class="group">
    <p>
        <% = Resources .Resource .Install_UserContols_NotifyParagrafView_Feedback %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-email" runat="server" ID="txtFeedbackEmail"></asp:TextBox></div>
</div>
<h2>
    <% = Resources .Resource .Install_UserContols_NotifyParagrafView_h2_2 %></h2>
<div class="group">
    <p>
        <% = Resources. Resource .Install_UserContols_NotifyParagrafView_SMTP %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtEmailSMTP"></asp:TextBox></div>
</div>
<div class="group">
    <p>
        <% = Resources. Resource .Install_UserContols_NotifyParagrafView_Login %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtEmailLogin"></asp:TextBox></div>
</div>
<div class="group">
    <p>
        <%= Resources. Resource . Install_UserContols_NotifyParagrafView_Pass %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-required" runat="server" ID="txtEmailPassword"></asp:TextBox></div>
</div>
<div class="group">
    <p>
        <% = Resources. Resource . Install_UserContols_NotifyParagrafView_Port %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-number" runat="server" ID="txtEmailPort"></asp:TextBox></div>
</div>
<div class="group">
    <p>
        <%= Resources. Resource .Install_UserContols_NotifyParagrafView_Email %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-email" runat="server" ID="txtEmail"></asp:TextBox></div>
</div>
<div class="group">
    <asp:CheckBox runat="server" ID="chkEnableSSL" Text="<%$ Resources:Resource,Install_UserContols_NotifyParagrafView_SSL %>" />
</div>
