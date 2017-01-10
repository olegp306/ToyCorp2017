<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="PaymentFailUrl.aspx.cs" Inherits="ClientPages.PaymentFailUrl" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <adv:StaticBlock ID="StaticBlock1" runat="server" SourceKey="PaymentFail" />
            <asp:HyperLink ID="Hyperlink1" NavigateUrl="." runat="server" Text="<%$ Resources: Resource, Client_ShoppingCart_ToMain%>"></asp:HyperLink>
        </div>
    </div>
</asp:Content>