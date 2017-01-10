<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepPayment.ascx.cs" Inherits="UserControls.OrderConfirmation.StepPayment" %>
<%@ Import Namespace="Resources" %>
<%@ Register TagPrefix="adv" TagName="PaymentMethods" Src="~/UserControls/OrderConfirmation/PaymentMethods.ascx" %>

<div class="payments-info oc-block">
    <adv:PaymentMethods runat="server" ID="pm" />

    <div id="pnlInfoForSberBank" runat="server" class="payment-details">
        <div class="oc-subtitle">
            <label for="txtINN2">
                <%=Resource.Client_OrderConfirmation_INN%></label>
        </div>
        <div class="param-name">
            <adv:AdvTextBox CssClassWrap="input-company" ValidationType="None" ID="txtINN2" runat="server" />
        </div>
    </div>

    <div id="pnlInfoForBill" runat="server" class="payment-details">
        <div class="oc-subtitle">
            <label for="txtCompanyName">
                <%=Resource.Client_OrderConfirmation_OrganizationName%></label>
        </div>
        <div class="oc-sub-payment-content">
            <div class="param-name">
                <adv:AdvTextBox CssClassWrap="input-company" ValidationType="None" ID="txtCompanyName" runat="server" />
            </div>
        </div>
        <div class="oc-subtitle">
            <label for="txtINN">
                <%=Resource.Client_OrderConfirmation_INN%></label>
        </div>
        <div class="oc-sub-payment-content">
            <div class="param-name">
                <adv:AdvTextBox CssClassWrap="input-company" ValidationType="None" ID="txtINN" runat="server" />
            </div>
        </div>
    </div>

    <div id="pnlPhoneForQiwi" runat="server" class="payment-details">
        <div class="oc-subtitle">
            <label for="txtPhone">
                <%=Resource.Client_OrderConfirmation_Phone%>:</label>
        </div>
        <div class="param-name" style="position: relative">
            +<adv:AdvTextBox CssClassWrap="input-company" ValidationType="Phone" ID="txtPhone" runat="server" />
            <br />
            <br />
            <%=Resource.Client_OrderConfirmation_PhoneComment%>
        </div>
    </div>
</div>
