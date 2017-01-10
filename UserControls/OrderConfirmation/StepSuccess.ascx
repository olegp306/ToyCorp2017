<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepSuccess.ascx.cs" Inherits="UserControls.OrderConfirmation.StepSuccess" %>
<%@ Import Namespace="AdvantShop.Modules" %>
<%@ Import Namespace="AdvantShop.Orders" %>
<%@ Import Namespace="Resources" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<script type="text/javascript">
    $(function () {
        getPaymentButton('<%=OrderID%>', 'btnPaymentFunctionality');
    });
</script>
<div class="step-success">
    <div class="static-block">
        <%= SbOrderSuccessTopText%>
    </div>
    <div>
        <span id="btnPaymentFunctionality"></span>
    </div>
    <div class="oc-print-b">
        <a class="oc-print" href="javascript:void(0)" onclick="javascript:open_printable_version('PrintOrder.aspx?OrderNumber=<%=Order.Number %>'); return false;">
            <%= Resource.Client_OrderConfirmation_PrintOrder %></a>
    </div>
    <adv:StaticBlock runat="server" SourceKey="ordersuccess" />
    <%= ModulesRenderer.RenderIntoOrderConfirmationFinalStep(OrderService.GetOrder(OrderID))%>
    <asp:Literal runat="server" ID="lSuccessScript"></asp:Literal>
</div>
