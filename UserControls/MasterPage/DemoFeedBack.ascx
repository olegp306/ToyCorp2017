<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DemoFeedBack.ascx.cs"
    Inherits="UserControls.MasterPage.DemoFeedback" %>
<%@ Register TagPrefix="adv" TagName="CaptchaControl" Src="~/UserControls/Captcha.ascx" %>
<a href="javascript:void(0);" class="link-feedback <%=GetCssClass() %>"></a>
<div id="feedBack" class="feedback">
    <div class="close-feedback">
    </div>
    <div class="feedback-wrap pie">
        <ul class="form form-vr">
            <li>
                <div class="param-name">
                    <label for="txtSenderName">
                        <%=Resources.Resource.Client_Feedback_Name%>:</label></div>
                <div class="param-value">
                    <adv:AdvTextBox ID="txtSenderName" runat="server" ValidationType="Required" ValidationGroup="DemoFeedBack" />
                </div>
            </li>
            <li>
                <div class="param-name">
                    <label for="txtEmail">
                        Email:</label></div>
                <div class="param-value">
                    <adv:AdvTextBox ID="txtEmail" runat="server" ValidationType="Email" ValidationGroup="DemoFeedBack" />
                </div>
            </li>
            <li>
                <div class="param-name">
                    <label for="txtMessage">
                        <%=Resources.Resource.Client_Feedback_MessageText%></label></div>
                <div class="param-value-textarea  param-value">
                    <adv:AdvTextBox ID="txtMessage" TextMode="MultiLine" runat="server" ValidationType="Required"
                        ValidationGroup="DemoFeedBack"></adv:AdvTextBox>
                </div>
            </li>
            <li runat="server" id="liCaptcha">
                <div class="param-name">
                    <label for="txtMessage">
                        <%=Resources.Resource.Client_Feedback_Code%></label></div>
                <div class="param-value-textarea  param-value">
                    <adv:CaptchaControl runat="server" ID="feedBackCaptcha" ValidationGroup="DemoFeedBack" />
                </div>
            </li>
            <li>
                <div class="param-name">
                </div>
                <div class="param-value">
                    <adv:Button ID="btnSend" Type="Submit" Size="Middle" runat="server" Text="<%$ Resources:Resource, Client_Feedback_Send %>"
                        ValidationGroup="DemoFeedBack" OnClick="btnSend_Click"></adv:Button>
                </div>
            </li>
        </ul>
    </div>
</div>
