<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyAccountOrderHistory.ascx.cs"
    Inherits="UserControls.MyAccount.MyAccountOrderHistory" %>
<script type="text/javascript">
    $(document).ready(function () {
        getOrderHistory("#orderHistoryForm", "#orderDetailsForm");
    });
</script>
<div id="orderHistoryForm" class="containerDiv">
</div>
<div id="orderDetailsForm" class="containerDiv">
</div>
