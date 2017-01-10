<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepLogin.ascx.cs" Inherits="UserControls.OrderConfirmation.StepLogin" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<%@ Register TagPrefix="adv" TagName="LoginOpenID" Src="~/UserControls/LoginOpenID.ascx" %>
<div class="form-c">
    <div class="title">
        <%=Resources.Resource.Client_OrderConfirmation_HaveAccount%></div>
    <ul class="form form-auth">
        <li>
            <adv:AdvTextBox ValidationType="Login" Placeholder="Email" runat="server" ID="txtAuthLogin" />
        </li>
        <li>
            <adv:AdvTextBox ValidationType="Required" Placeholder="<%$ Resources:Resource, Client_MasterPage_Password %>"
                ID="txtAuthPWD" runat="server" TextMode="Password" DefaultButtonID="btnAuthGO" />
        </li>
    </ul>
    <div>
        <adv:Button ID="btnAuthGO" runat="server" Type="Action" Size="Middle" Text="<%$ Resources:Resource, Client_MasterPage_SignIn %>"
            OnClick="btnAuthGO_Click"></adv:Button>
        <a href="fogotPassword.aspx" class="link-forget">
            <%=Resources.Resource.Client_MasterPage_FogotPassword%></a>
    </div>
    <%--<adv:LoginOpenID runat="server" PageToRedirect="orderconfirmation.aspx"/>--%>
</div>
<div class="form-addon">
    <div class="form-addon-text">
        <div class="title">
            <%=Resources.Resource.Client_OrderConfirmation_NewBuyer%></div>
        <div class="new-user-text">
            <adv:StaticBlock ID="staticBlockReg" runat="server" SourceKey="loginRegBlock" />
        </div>
        <div class="btn-new-users">
            <adv:Button ID="btnGoWithReg" OnClick="btnGoWithReg_Click" Type="Action" Size="Middle"
                runat="server" Text="<%$Resources:Resource,Client_MasterPage_Registration%>"
                DisableValidation="True"></adv:Button>
            <br />
            <br />
            <adv:Button Text="<%$ Resources:Resource,Client_OrderConfirmation_OrderConfirmationWithoutReg%>"
                ID="bntGoQReg" Size="Middle" Type="Confirm" runat="server" OnClick="bntGoQReg_Click"
                DisableValidation="True" />
        </div>
        <br class="clear" />
    </div>
</div>
