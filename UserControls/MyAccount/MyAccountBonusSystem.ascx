<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyAccountBonusSystem.ascx.cs"
    Inherits="UserControls.MyAccount.MyAccountBonusSystem" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>

<div id="bonuses">
    <div class="subtitle">
        <%= Resource.Client_Bonuses_BonusCard %>
    </div>
    <% if (Card == null)
       { %>
    <div class="bonus-item">
        <label class="bonus-choice">
            <input type="radio" id="card_yes" runat="server" name="card_sel" />
            <%= Resource.Client_StepBonus_IHaveBonusCard %></label>
        <div class="bonus-content bonus-hidden">
            <div class="bonus-content-item">
                <div class="bonus-i-b"><%= Resource.Client_Bonuses_CardNumber %></div>
                <div class="bonus-i-v">
                    <adv:AdvTextBox runat="server" ID="txtCardNumber" Width="120px" />
                    <adv:Button runat="server" ID="btnBonusConfirmMa" CssClass="ma-bonus-confirm" Size="Middle" Type="Confirm" Text="<%$ Resources: Resource, Client_StepBonus_Aplly%>" />
                </div>
            </div>
            <%= Resource.Client_Bonuses_Or %>
            <div class="bonus-content-item">
                <div class="bonus-i-b"><%= Resource.Client_Bonuses_PhoneNumber %></div>
                <div class="bonus-i-v">
                    <adv:AdvTextBox runat="server" ID="txtPhoneCardNumber" Width="120px" CssClass="mask-phone mask-inp" />
                    <adv:Button runat="server" ID="btnBonusConfirmPhoneMa" CssClass="ma-bonus-confirm" Size="Middle" Type="Confirm" Text="<%$ Resources: Resource, Client_StepBonus_Aplly%>" />
                </div>
            </div>
        </div>
    </div>
    <div class="bonus-item">
        <label class="bonus-choice">
            <input type="radio" id="card_want" runat="server" name="card_sel" checked="True" />
            <%= Resource.Client_StepBonus_IWantBonusCard %></label>
        <div class="bonus-content">
            <% } %>

            <div style='<%= (Card == null) ? "dispaly:block": "display:none"%>'>
                <div id="cardinfo">
                    <ul class="form form-vr">
                        <li>
                            <div class="param-name">
                                <label for="txtBonusLastName">
                                    <%= Resource.Client_Bonuses_LastName%></label>
                            </div>
                            <div class="param-value">
                                <adv:AdvTextBox ID="txtBonusLastName" ValidationType="Required" runat="server" ValidationGroup="mabonus" />
                            </div>
                        </li>
                        <li>
                            <div class="param-name">
                                <label for="txtBonusFirstName">
                                    <%= Resource.Client_Bonuses_FirstName%></label>
                            </div>
                            <div class="param-value">
                                <adv:AdvTextBox ID="txtBonusFirstName" ValidationType="Required" runat="server" ValidationGroup="mabonus" />
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
                                <adv:AdvTextBox ID="txtBonusSecondName" ValidationType="Required" runat="server" ValidationGroup="mabonus" />
                            </div>
                        </li>
                        <% } %>
                        <li>
                            <div class="param-name">
                                <label>
                                    <%= Resource.Client_Bonuses_Gender%></label>
                            </div>
                            <div class="param-value gender">
                                <label>
                                    <input type="radio" id="genderMale" runat="server" name="BonusGender" value="0" />
                                    <%= Resource.Client_Bonuses_Male%></label>
                                <label>
                                    <input type="radio" id="genderFemale" runat="server" name="BonusGender" value="1" />
                                    <%= Resource.Client_Bonuses_Female%></label>
                            </div>
                        </li>
                        <li>
                            <div class="param-name">
                                <label for="txtBonusDate">
                                    <%= Resource.Client_Bonuses_Date%></label>
                            </div>
                            <div class="param-value">
                                <adv:AdvTextBox ID="txtBonusDate" ValidationType="Required" CssClass="mask-date" runat="server" ValidationGroup="mabonus" />
                            </div>
                        </li>
                        <li>
                            <div class="param-name">
                                <label for="txtBonusPhone">
                                    <%= Resource.Client_Bonuses_Phone%></label>
                            </div>
                            <div class="param-value">
                                <adv:AdvTextBox ID="txtBonusPhone" ValidationType="Required" CssClass="mask-phone" runat="server" ValidationGroup="mabonus" />
                            </div>
                        </li>
                        <li>
                            <div class="param-name">
                            </div>
                            <div class="param-value">
                                <adv:Button ID="btnMaAddBonusCard" Size="Big" Type="Submit" runat="server" ValidationGroup="mabonus" />
                            </div>
                        </li>
                    </ul>
                </div>
            </div>

            <% if (Card == null)
               { %>
        </div>
    </div>
    <% } %>

    <div id="bonus-confirm" style="display: none">
        <div class="bonus-confirm-content">
            <div class="bonus-confirm-error"></div>
            <div><%= Resource.Client_Bonuses_ConfirmText%></div>
            <div class="bonus-content-code">
                <adv:AdvTextBox runat="server" />
            </div>
            <adv:Button runat="server" ID="btnBonusConfirmCode" CssClass="bonus-confirm-code" Size="Middle" Type="Confirm"
                Text="<%$ Resources: Resource, Client_Bonuses_Confirm %>" />
        </div>
    </div>
</div>

<% if (Card != null)
   { %>
<ul class="form form-vr">
    <li>
        <div class="param-name">
            <label for="txtFirstName">
                <%= Resource.Client_Bonuses_CardNumber%>:</label>
        </div>
        <div class="param-value">
            <%= Card.CardNumber %>
        </div>
    </li>
    <li>
        <div class="param-name">
            <label for="txtLastName">
                <%= Resource.Client_Bonuses_BonusAmount%>:</label>
        </div>
        <div class="param-value">
            <%= Card.BonusAmount %>
        </div>
    </li>
    <li>
        <div class="param-name">
            <label for="txtSecondName">
                <%= Resource.Client_Bonuses_BonusPercent%>:</label>
        </div>
        <div class="param-value">
            <%= Card.BonusPercent %>%
        </div>
    </li>
    <li>
        <a class="edit-card-info" href="javascript:void(0)"><%= Resource.Client_Bonuses_EditInfo%></a>
    </li>
</ul>
<% } %>