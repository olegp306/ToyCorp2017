<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepBonus.ascx.cs" Inherits="UserControls.OrderConfirmation.StepBonus" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>

<div class="oc-block">
    <div id="bonuses">
        <% if (Card != null) { %>
        <div class="order-b-title"><%= Resource.Client_StepBonus_GetDiscount%></div>
        <div class="order-b-content">
            <label><input type="checkbox" id="chkBonus" runat="server" /> <%= Resource.Client_StepBonus_ByBonusCard%> <asp:Literal runat="server" ID="liBonusAmount"/> </label>
        </div>
        <% } else { %>
        <div class="order-b-title"><%= Resource.Client_StepBonus_Bonuses%></div>
        <div class="order-b-content">
            <div class="bonus-item">
                <label class="bonus-choice"> <input type="radio" id="card_no" runat="server" name="card_sel" checked="True"/> <%= Resource.Client_StepBonus_NoBonusCard%></label>
            </div>
            <div class="bonus-item">
                <label class="bonus-choice"> <input type="radio" id="card_yes" runat="server" name="card_sel" /> <%= Resource.Client_StepBonus_IHaveBonusCard%></label>
                <div class="bonus-content bonus-hidden">
                    <div class="bonus-content-item">
                        <div class="bonus-i-b"><%= Resource.Client_Bonuses_CardNumber %></div>
                        <div class="bonus-i-v">
                            <adv:AdvTextBox runat="server" ID="txtCardNumber" width="120px" /> 
                            <adv:Button runat="server" ID="btnBonusConfirm" CssClass="bonus-confirm" Size="Middle" Type="Confirm" Text="<%$ Resources: Resource, Client_StepBonus_Aplly%>"/>
                        </div>
                    </div>
                    <%= Resource.Client_Bonuses_Or %>
                    <div class="bonus-content-item">
                        <div class="bonus-i-b"><%= Resource.Client_Bonuses_PhoneNumber %></div>
                        <div class="bonus-i-v">
                            <adv:AdvTextBox runat="server" ID="txtPhoneCardNumber" width="120px" CssClass="mask-phone mask-inp" /> 
                            <adv:Button runat="server" ID="btnBonusConfirmPhone" CssClass="bonus-confirm" Size="Middle" Type="Confirm" Text="<%$ Resources: Resource, Client_StepBonus_Aplly%>"/>
                        </div>
                    </div>
                </div>                
            </div>
            <div class="bonus-item">
                <label class="bonus-choice">
                    <input type="radio" id="card_want" runat="server" name="card_sel" /> <%= Resource.Client_StepBonus_IWantBonusCard%>
                    <asp:Literal runat="server" ID="liBonusesForNewCard" Visible="False"/>
                </label>
                <div class="bonus-content bonus-hidden">
                    <ul class="form form-vr">
                        <li>
                            <div class="param-name">
                                <label for="txtBonusLastName">
                                    <%= Resource.Client_Bonuses_LastName %></label></div>
                            <div class="param-value">
                                <adv:AdvTextBox ID="txtBonusLastName" ValidationType="Required" ValidationGroup="mabonus" runat="server" />
                            </div>
                        </li>
                        <li>
                            <div class="param-name">
                                <label for="txtBonusFirstName">
                                    <%= Resource.Client_Bonuses_FirstName %></label></div>
                            <div class="param-value">
                                <adv:AdvTextBox ID="txtBonusFirstName" ValidationType="Required" ValidationGroup="mabonus" runat="server" />
                            </div>
                        </li>
                        <% if (SettingsOrderConfirmation.IsShowPatronymic) { %>
                        <li>
                            <div class="param-name">
                                <label for="txtBonusSecondName">
                                    <%= Resource.Client_Bonuses_SecondName %></label></div>
                            <div class="param-value">
                                <adv:AdvTextBox ID="txtBonusSecondName" ValidationType="Required" ValidationGroup="mabonus" runat="server" />
                            </div>
                        </li>
                        <% } %>
                        <li>
                            <div class="param-name">
                                <label for="Gender">
                                    <%= Resource.Client_Bonuses_Gender %></label></div>
                            <div class="param-value">
                                <label><input type="radio" name="BonusGender" value="0" checked="checked" /> <%= Resource.Client_Bonuses_Male%></label>
                                <label><input type="radio" name="BonusGender" value="1" /> <%= Resource.Client_Bonuses_Female%></label>
                            </div>
                        </li>
                        <li>
                            <div class="param-name">
                                <label for="txtBonusDate">
                                    <%= Resource.Client_Bonuses_Date %></label></div>
                            <div class="param-value">
                                <adv:AdvTextBox ID="txtBonusDate" ValidationType="Required" ValidationGroup="mabonus" CssClass="mask-date mask-inp" runat="server" />
                            </div>
                        </li>
                        <li>
                            <div class="param-name">
                                <label for="txtBonusPhone">
                                    <%= Resource.Client_Bonuses_Phone %></label></div>
                            <div class="param-value">
                                <adv:AdvTextBox ID="txtBonusPhone" ValidationType="Required" ValidationGroup="mabonus" CssClass="mask-phone mask-inp" runat="server" />
                            </div>
                        </li>
                    </ul>
                    <adv:Button runat="server" ID="btnAddBonusCard" Size="Middle" Type="Confirm" ValidationGroup="mabonus" Text="<%$ Resources: Resource, Client_Bonuses_GetBonusCard %>"/>
                </div>
            </div>
        </div>
        <div id="bonus-confirm" style="display: none">
            <div class="bonus-confirm-content">
                <div class="bonus-confirm-error"></div>
                <div><%= Resource.Client_Bonuses_ConfirmText%></div>
                <span class="input-wrap bonus-content-code">
                      <input onkeyup="defaultButtonClick('btnBonusConfirmCode', event)" type="text" />
                </span>
                <adv:Button runat="server" ID="btnBonusConfirmCode" CssClass="bonus-confirm-code" Size="Middle" Type="Confirm" Text="<%$ Resources: Resource, Client_Bonuses_Confirm %>"/>
            </div>
        </div>
        <% } %>
    </div>
</div>