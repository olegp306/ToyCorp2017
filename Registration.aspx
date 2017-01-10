<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeFile="Registration.aspx.cs" Inherits="ClientPages.Registration" %>

<%@ Register TagPrefix="adv" TagName="LoginOpenID" Src="~/UserControls/LoginOpenID.ascx" %>
<%@ Register TagPrefix="adv" TagName="CaptchaControl" Src="UserControls/Captcha.ascx" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>

<%@ Import Namespace="AdvantShop.BonusSystem" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <h1>
                <%= Resource. Client_Registration_Registration %></h1>
            <div id="dvDemoDataUserNotification" runat="server" visible="false" class="OrderConfirmation_NotifyLable">
                <%=Resource.Client_OrderConfirmation_WithDemoMode%>
            </div>
            <ul id="ulUserRegistarionValidation" runat="server" visible="false" class="ulValidFaild">
                <li>Error1</li>
                <li>Error2</li>
            </ul>
            <div class="form-c">
                <asp:Label ID="lblMessage" runat="server" Visible="False" ForeColor="red"></asp:Label>
                <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False"></asp:Label>
                <ul class="form">
                    <li>
                        <div class="param-name">
                            <label for="txtFirstName">
                                <%=SettingsOrderConfirmation.CustomerFirstNameField%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:advtextbox validationtype="Required" id="txtFirstName" runat="server" validationgroup="reg" />
                        </div>
                    </li>
                    <% if (SettingsOrderConfirmation.IsShowLastName)
                       { %>
                    <li>
                        <div class="param-name">
                            <label for="txtLastName">
                                <%=Resource.Client_Registration_Surname%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:advtextbox validationtype="Required" id="txtLastName" runat="server" validationgroup="reg" />
                        </div>
                    </li>
                    <% } if (SettingsOrderConfirmation.IsShowPatronymic)
                       { %>
                    <li>
                        <div class="param-name">
                            <label for="txtLastName">
                                <%=Resource.Client_Registration_Patronymic%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:advtextbox validationtype="None" id="txtPatronymic" runat="server" validationgroup="reg" />
                        </div>
                    </li>
                    <% } %>
                    <li>
                        <div class="param-name">
                            <label for="txtEmail">
                                E-Mail:</label>
                        </div>
                        <div class="param-value">
                            <adv:advtextbox validationtype="NewEmail" id="txtEmail" runat="server" defaultbuttonid="btnRegister" validationgroup="reg" />
                        </div>
                    </li>
                    <li>
                        <div class="param-name">
                            <label for="txtPassword">
                                <%=Resource.Client_Registration_Password%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:advtextbox validationtype="CompareSource" id="txtPassword" runat="server" textmode="Password" validationgroup="reg" />
                        </div>
                    </li>
                    <li>
                        <div class="param-name">
                            <label for="txtPasswordConfirm">
                                <%=Resource.Client_Registration_PasswordAgain%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:advtextbox validationtype="Compare" id="txtPasswordConfirm" runat="server" textmode="Password" validationgroup="reg" />
                        </div>
                    </li>
                    <% if (SettingsOrderConfirmation.IsShowPhone || BonusSystem.IsActive)
                       { %>
                    <li>
                        <div class="param-name">
                            <label for="txtPhone">
                                <%=SettingsOrderConfirmation.CustomerPhoneField%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:advtextbox validationtype="Required" id="txtPhone" runat="server" validationgroup="reg" />
                        </div>
                    </li>
                    <% } %>
                </ul>

                <% if (BonusSystem.IsActive)
                   { %>
                <div id="bonuses">
                    <% if (Card != null)
                       { %>
                    <div class="order-b-title"><%= Resource.Client_StepBonus_Bonuses %></div>
                    <div class="order-b-content">
                        <%= Resource.Client_StepBonus_BonusesApplied %>
                    </div>
                    <% }
                       else
                       { %>
                    <div class="order-b-title"><%= Resource.Client_StepBonus_Bonuses %></div>
                    <div class="order-b-content">
                        <div class="bonus-item">
                            <label class="bonus-choice">
                                <input type="radio" id="card_no" runat="server" name="card_sel" checked="True" />
                                <%= Resource.Client_StepBonus_NoBonusCard %></label>
                        </div>
                        <div class="bonus-item">
                            <label class="bonus-choice">
                                <input type="radio" id="card_yes" runat="server" name="card_sel" />
                                <%= Resource.Client_StepBonus_IHaveBonusCard %></label>
                            <div class="bonus-content bonus-hidden">
                                <div class="bonus-content-item">
                                    <div class="bonus-i-b"><%= Resource.Client_Bonuses_CardNumber %></div>
                                    <div class="bonus-i-v">
                                        <adv:advtextbox runat="server" id="txtCardNumber" width="120px" />
                                        <adv:button runat="server" id="btnBonusConfirm" cssclass="bonus-confirm" size="Middle" type="Confirm" text="<%$ Resources: Resource, Client_StepBonus_Aplly%>" />
                                    </div>
                                </div>
                                <%= Resource.Client_Bonuses_Or %>
                                <div class="bonus-content-item">
                                    <div class="bonus-i-b"><%= Resource.Client_Bonuses_PhoneNumber %></div>
                                    <div class="bonus-i-v">
                                        <adv:advtextbox runat="server" id="txtPhoneCardNumber" width="120px" cssclass="mask-phone mask-inp" />
                                        <adv:button runat="server" id="btnBonusConfirmPhone" cssclass="bonus-confirm" size="Middle" type="Confirm" text="<%$ Resources: Resource, Client_StepBonus_Aplly%>" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="bonus-item">
                            <label class="bonus-choice">
                                <input type="radio" id="card_want" runat="server" name="card_sel" />
                                <%= Resource.Client_StepBonus_IWantBonusCard %>
                                <asp:Literal runat="server" ID="liBonusesForNewCard" Visible="False" />
                            </label>
                            <div class="bonus-content bonus-hidden">
                                <ul class="form form-vr">
                                    <li>
                                        <div class="param-name">
                                            <label for="txtBonusLastName">
                                                <%= Resource.Client_Bonuses_LastName %></label>
                                        </div>
                                        <div class="param-value">
                                            <adv:advtextbox id="txtBonusLastName" validationtype="Required" validationgroup="mabonus" runat="server" />
                                        </div>
                                    </li>
                                    <li>
                                        <div class="param-name">
                                            <label for="txtBonusFirstName">
                                                <%= Resource.Client_Bonuses_FirstName %></label>
                                        </div>
                                        <div class="param-value">
                                            <adv:advtextbox id="txtBonusFirstName" validationtype="Required" validationgroup="mabonus" runat="server" />
                                        </div>
                                    </li>
                                    <% if (SettingsOrderConfirmation.IsShowPatronymic)
                                       { %>
                                    <li>
                                        <div class="param-name">
                                            <label for="txtBonusSecondName">
                                                <%= Resource.Client_Bonuses_SecondName %></label>
                                        </div>
                                        <div class="param-value">
                                            <adv:advtextbox id="txtBonusSecondName" validationtype="Required" validationgroup="mabonus" runat="server" />
                                        </div>
                                    </li>
                                    <% } %>
                                    <li>
                                        <div class="param-name">
                                            <label for="Gender">
                                                <%= Resource.Client_Bonuses_Gender %></label>
                                        </div>
                                        <div class="param-value">
                                            <label>
                                                <input type="radio" name="BonusGender" value="0" checked="checked" />
                                                <%= Resource.Client_Bonuses_Male %></label>
                                            <label>
                                                <input type="radio" name="BonusGender" value="1" />
                                                <%= Resource.Client_Bonuses_Female %></label>
                                        </div>
                                    </li>
                                    <li>
                                        <div class="param-name">
                                            <label for="txtBonusDate">
                                                <%= Resource.Client_Bonuses_Date %></label>
                                        </div>
                                        <div class="param-value">
                                            <adv:advtextbox id="txtBonusDate" validationtype="Required" validationgroup="mabonus" cssclass="mask-date mask-inp" runat="server" />
                                        </div>
                                    </li>
                                    <li>
                                        <div class="param-name">
                                            <label for="txtBonusPhone">
                                                <%= Resource.Client_Bonuses_Phone %></label>
                                        </div>
                                        <div class="param-value">
                                            <adv:advtextbox id="txtBonusPhone" validationtype="Required" validationgroup="mabonus" cssclass="mask-phone mask-inp" runat="server" />
                                        </div>
                                    </li>
                                </ul>
                                <adv:button runat="server" id="btnAddBonusCard" size="Middle" type="Confirm" validationgroup="mabonus" text="<%$ Resources: Resource, Client_Bonuses_GetBonusCard %>" />
                            </div>
                        </div>
                    </div>
                    <div id="bonus-confirm" style="display: none">
                        <div class="bonus-confirm-content">
                            <div class="bonus-confirm-error"></div>
                            <div><%= Resource.Client_Bonuses_ConfirmText %></div>
                            <span class="input-wrap bonus-content-code">
                                <input onkeyup="defaultButtonClick('btnBonusConfirmCode', event)" type="text" />
                            </span>
                            <adv:button runat="server" id="btnBonusConfirmCode" cssclass="bonus-confirm-code-reg" size="Middle" type="Confirm" text="<%$ Resources: Resource, Client_Bonuses_Confirm %>" />
                        </div>
                    </div>
                    <% } %>
                </div>
                <% } %>

                <ul class="form" style="padding: 5px 0 0 0">
                    <li runat="server" id="NewsSubscription">
                        <div class="param-name"></div>
                        <div class="param-value">
                            <asp:CheckBox ID="chkSubscribed4News" runat="server" Text="<%$ Resources: Resource, Client_Registration_NewsSubscribe%>" />
                        </div>
                    </li>
                    <li runat="server" id="liCaptcha">
                        <div class="param-name">
                            <label for="txtValidCode">
                                <%=Resource.Client_Details_Code%>:</label>
                        </div>
                        <div class="param-value">
                            <adv:captchacontrol id="dnfValid" runat="server" defaultbuttonid="btnRegister" validationgroup="reg" />
                        </div>
                    </li>
                    <% if (SettingsOrderConfirmation.IsShowUserAgreementText)
                       { %>
                    <li>
                        <div class="param-name"></div>
                        <div class="param-value">
                            <input type="checkbox" runat="server" id="chkAgree" class="valid-required" />
                            <label for="chkAgree">
                                <%= Resource.Client_Registration_Agree %></label>
                        </div>
                    </li>
                    <% } %>
                    <li>
                        <div class="param-name"></div>
                        <div class="param-value">
                            <br />
                            <adv:button id="btnRegister" type="Action" size="Middle" runat="server" text="<%$ Resources:Resource, Client_Registration_Reg %>"
                                onclick="btnRegister_Click" validationgroup="reg" />
                        </div>
                    </li>
                </ul>

            </div>
            <div class="form-addon">
                <div class="form-addon-text">
                    <adv:staticblock id="staticBlock" runat="server" sourcekey="textOnReg" />
                </div>
                <adv:loginopenid runat="server" pagetoredirect="registration.aspx" />
            </div>
            <br class="clear" />
        </div>
    </div>
</asp:Content>
