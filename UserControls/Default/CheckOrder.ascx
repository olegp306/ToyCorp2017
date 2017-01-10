<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckOrder.ascx.cs"
    Inherits="UserControls.Default.CheckOrder" %>
<!--noindex-->
<article class="block-uc" data-plugin="expander" id="checkorder">
    <h3 class="title" data-expander-control="#check-status"><%=Resources.Resource.Client_UserControls_StatusComment_Head%></h3>
    <div class="content" id="check-status">
        <div class="status-input">
            <adv:AdvTextBox ID="txtNumber" runat="server" Placeholder='<%$ Resources:Resource, Client_UserControls_StatusComment_Number%>' MaxLength="20" DefaultButtonID="btncheckorder" />
            <div id="orderStatus" class="checkorder"></div>
        </div>
        <div class="btn-status-check">
            <adv:Button runat="server" ID="btncheckorder" Size="Middle" Type="Confirm" Text='<%$ Resources:Resource, Client_UserControls_StatusComment_Check%>' />
        </div>
    </div>
</article>
<!--/noindex-->
