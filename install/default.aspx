<%@ Page Language="C#" AutoEventWireup="true" CodeFile="default.aspx.cs" Inherits="ClientPages._default"
    EnableViewState="false" %>

<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Register Src="UserContols/ShopinfoView.ascx" TagName="ShopinfoView" TagPrefix="adv" %>
<%@ Register Src="UserContols/FinanceView.ascx" TagName="FinanceView" TagPrefix="adv" %>
<%@ Register Src="UserContols/PaymentView.ascx" TagName="PaymentView" TagPrefix="adv" %>
<%@ Register Src="UserContols/ShippingView.ascx" TagName="ShippingView" TagPrefix="adv" %>
<%@ Register Src="UserContols/OpenidParagrafView.ascx" TagName="OpenidParagrafView"
    TagPrefix="adv" %>
<%@ Register Src="UserContols/NotifyParagrafView.ascx" TagName="NotifyParagrafView"
    TagPrefix="adv" %>
<%@ Register Src="UserContols/FinalView.ascx" TagName="FinalView" TagPrefix="adv" %>
<%@ Register Src="UserContols/TrialSelectView.ascx" TagName="TrialSelectView" TagPrefix="adv" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>
        <% = Resources.Resource.Install_Default_Title%></title>
    <link type="text/css" rel="stylesheet" href="css/style.css" />
    <link type="text/css" rel="stylesheet" href="../css/jq/jquery.autocomplete.css" />
    <link type="text/css" rel="stylesheet" href="../css/validator.css" />
    <script type="text/javascript" src="../js/localization/<%= SettingsMain.Language%>/lang.js"></script>
    <script type="text/javascript" src="../js/jq/jquery-1.7.1.min.js"></script>
</head>
<body>
    <form id="form" runat="server">
    <table class="container footer-img">
        <tr>
            <td class="header">
                <img src="images/logo.png" alt="AdvantShop.NET" class="logo"/>
                <div class="header-text">
                    <span class="options"><%= Resources.Resource.Installer_Setting %></span><br />
                    <span class="version"><%= Resources.Resource.Installer_Version %></span>
                </div>
            </td>
        </tr>
        <tr>
            <td class="content">
                <div class="column-left">
                    <div class="column-split-top">
                    </div>
                    <div class="steps">
                        <% = GetLeftMenu() %>
                    </div>
                </div>
                <div class="column-right">
                    <asp:MultiView runat="server" ID="views">
                        <asp:View runat="server" ID="ViewTrialSelect">
                            <adv:TrialSelectView ID="TrialSelectView" runat="server" />
                        </asp:View>
                        <asp:View runat="server" ID="ViewShopinfo">
                            <adv:ShopinfoView ID="ShopinfoView" runat="server" />
                        </asp:View>
                        <asp:View runat="server" ID="ViewFinance">
                            <adv:FinanceView ID="FinanceView" runat="server" />
                        </asp:View>
                        <asp:View runat="server" ID="ViewPayment">
                            <adv:PaymentView ID="PaymentView" runat="server" />
                        </asp:View>
                        <asp:View runat="server" ID="ViewShipping">
                            <adv:ShippingView ID="ShippingView" runat="server" />
                        </asp:View>
                        <asp:View runat="server" ID="ViewOpenidParagraf">
                            <adv:OpenidParagrafView ID="OpenidParagrafView" runat="server" />
                        </asp:View>
                        <asp:View runat="server" ID="ViewNotifyParagraf">
                            <adv:NotifyParagrafView ID="NotifyParagrafView" runat="server" />
                        </asp:View>
                        <asp:View runat="server" ID="ViewFinal">
                            <adv:FinalView ID="FinalView" runat="server" />
                        </asp:View>
                    </asp:MultiView>
                    <div runat="server" id="errContent" class="err-content">
                    </div>
                    <div class="step-btns">
                        <adv:Button runat="server" ID="btnBack" OnClick="BackClick" CssClass="btn-back" Text="<%$Resources:Resource, Installer_Back %>" ValidationGroup="none" />
                        <adv:Button runat="server" ID="btnNext" OnClick="NextClick" CssClass="btn-next" Text="<%$Resources:Resource, Installer_Next %>" />
                        <asp:LinkButton  Text="<%$Resources:Resource, Installer_Finish %>" runat="server" ID="btnGoToShop" Visible="false" OnClick="GoShopClick"
                            CssClass="btn-next"></asp:LinkButton>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="column-split-bottom">
            </td>
        </tr>
        <tr>
            <td class="footer">
                <%= Resources.Resource.Client_MasterPage_Copyright%>
            </td>
        </tr>
    </table>
    <div class="footer-bg">
    </div>
    <div class="footer-img">
    </div>
    <div class="top-img">
    </div>
    </form>
    <script type="text/javascript" src="../js/jq/jquery.autocomplete.js"></script>
    <script type="text/javascript" src="../js/jq/jquery.validate.js"></script>
    <script type="text/javascript" src="../js/validateInit.js"></script>
    <script type="text/javascript" src="js/wizard.js"></script>
    <script type="text/javascript" src="../js/doPostBack.js"></script>
</body>
</html>
