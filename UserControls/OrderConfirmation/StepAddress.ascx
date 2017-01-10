<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepAddress.ascx.cs" Inherits="UserControls.OrderConfirmation.StepAddress" %>
<%@ Register TagPrefix="adv" TagName="LoginOpenID" Src="~/UserControls/LoginOpenID.ascx" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>

<div id="DivNoReg" runat="server">

    <div class="oc-text-left"><%= Resource.Client_OrderConfirmation_Customer %></div>
    <div class="usertypes-b checkbox">
            <input type="radio" name="usertype" checked="True" value="0" runat="server" id="rbNewCustomer" />
            <label class="usertypes-b-label" for="rbNewCustomer"><%=Resource.Client_OrderConfirmation_NewBuyer%></label>
            
            <input type="radio" name="usertype" value="1" runat="server" id="rbbOldCustomer" />
            <label class="usertypes-b-label" for="rbbOldCustomer"><%=Resource.Client_OrderConfirmation_HaveAccount%></label>
        <a href="javascript:void(0);" class="js-oc-social" id="signInSocial" runat="server"><%= Resource.Client_OrderConfirmation_SingInSocial %></a>
    </div>

    <div class="signin oc-block">
        <div class="oc-text-left"> Контактные данные </div>
        <ul class="form form-vr">
            <li>
                <div class="param-value">
                    <adv:AdvTextBox ValidationType="Login" runat="server" ID="txtLoginEmail" ValidationGroup="login" Placeholder="E-mail" />
                </div>
            </li>
            <li>
                <div class="param-value">
                    <adv:AdvTextBox ValidationType="Required" ValidationGroup="login"
                        ID="txtLoginPassword" runat="server" TextMode="Password" DefaultButtonID="btnLogin" Placeholder="Пароль" />
                </div>
            </li>
            <li>
                <div class="param-value">
                    <adv:Button ID="btnLogin" runat="server" Type="Action" Size="Middle" Text="<%$ Resources:Resource, Client_MasterPage_SignIn %>" ValidationGroup="login"
                        OnClick="btnLogin_Click" />
                    <a href="fogotPassword.aspx" class="link-forget"><%=Resource.Client_MasterPage_FogotPassword%></a>
                </div>
            </li>
        </ul>
    </div>

    <div class="newcustomer oc-block">
        <div id="dvLoginPanel" runat="server">
            <div class="oc-text-left">Контактные данные</div>
            <ul class="form form-vr">
                <li>
                    <div class="param-value">
                        <adv:AdvTextBox ValidationType="Email" ID="txtEmail" runat="server"  placeholder="E-mail"/>
                    </div>
                </li>
                <li id="tblLoginTable" runat="server" style="display: none">
                    <div class="param-name">
                        <label for="txtPassword">
                            <%= Resource.Client_Registration_Password %>:</label>
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ID="txtPassword" runat="server" TextMode="Password" />
                    </div>
                </li>
                <li>
                    <div class="param-value">
                        <adv:AdvTextBox ValidationType="Required" ID="txtFirstName" runat="server" MaxLength="100" placeholder="Имя"/>
                    </div>
                
                <% if (SettingsOrderConfirmation.IsShowLastName)
                   {%>
                
                    <div class="param-value" style="margin-left: 40px;">
                        <adv:AdvTextBox ValidationType="Required" ID="txtLastName" runat="server"  MaxLength="100" placeholder="Фамилия"/>
                    </div>
                </li>
                <% } if (SettingsOrderConfirmation.IsShowPatronymic)
                   {%>
                <li>
                    <div class="param-name">
                        <label for="txtPatronymic">
                            <%= Resource.Client_Registration_Patronymic%>:</label>
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ValidationType="None" ID="txtPatronymic" runat="server"  MaxLength="100"/>
                    </div>
                </li>
                <% } if (SettingsOrderConfirmation.IsShowPhone)
                   {%>
                <li>
                    <div class="param-value">
                        <adv:AdvTextBox ValidationType="Required" ID="txtPhone" runat="server"  MaxLength="100" placeholder="Номер телефона"/>
                    </div>
                </li>
                <% } if (!SettingsDesign.DisplayCityInTopPanel)
                   { %>

                <% if (SettingsOrderConfirmation.IsShowCountry)
                   { %>
                <li>
                    <div class="param-name">
                        <%= Resource.Client_Registration_Country %>:
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ValidationType="Required" ID="txtCountry" runat="server" MaxLength="70" />
                    </div>
                </li>
                <% } %>

                <% if (SettingsOrderConfirmation.IsShowState)
                   { %>
                <li>
                    <div class="param-name">
                        <%= Resource.Client_Registration_State %>:
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ValidationType="Required" ID="txtRegion" runat="server" MaxLength="70" />
                    </div>
                </li>
                <% } %>
                <% if (SettingsOrderConfirmation.IsShowCity)
                   { %>
                <li>
                    <div class="param-name">
                        <%= Resource.Client_OrderConfirmationt_City %>:
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ValidationType="Required" ID="txtCity" runat="server" CssClass="autocompleteCity" MaxLength="70" />
                    </div>
                </li>
                <% } %>
                <% } %>
                <li>
                    <div class="param-value" style="width: 400px;">
                        <label>
                            <input type="checkbox" id="chknewcustomer" runat="server" />
                            <%=Resource.Client_OrderConfirmation_Register%></label>
                        <adv:StaticBlock ID="sbOrderRegHint" runat="server" SourceKey="OrderRegHint" />
                    </div>
                </li>
                <li class="newcustomer-hidden">
                    <div class="param-value">
                        <adv:AdvTextBox ValidationType="CompareSource" ID="txtNewPassword" runat="server" TextMode="Password" placeholder="Пароль"/>
                    </div>
                </li>
                <li class="newcustomer-hidden">
                    <div class="param-value">
                        <adv:AdvTextBox ValidationType="Compare" ID="txtPasswordConfirm" runat="server" TextMode="Password" placeholder="Пароль (ещё раз)"/>
                    </div>
                </li>
            </ul>
        </div>
    </div>

    <div hidden>
        <div class="js-oc-social-content oc-social-content">
            <adv:LoginOpenID ID="LoginOpenID" runat="server" PageToRedirect="orderconfirmation.aspx" />
        </div>
    </div>
</div>

<div id="DivReg" runat="server" class="contacts-reg-b">
    <div id="contactsDivOc" class="contactsDiv"></div>
    <adv:Button ID="btnAddAddress" runat="server" Type="Submit" Size="Middle" CssClass="btn-add-adr-my"
        Text="<%$ Resources:Resource, Client_OrderConfirmation_Add %>" />

    <asp:HiddenField ID="hfOcContactShippingId" runat="server" />
    <asp:HiddenField ID="hfOcContactBillingId" runat="server" />
    <div style="display: none">
        <div id="modal">
            <div class="oc-text-left"> Контактные данные </div>
            <ul class="form form-vr">
                <li>
                    <div class="param-name">
                        <label for="txtContactNameOc">
                            <%=Resource.Client_MyAccount_FIO%></label>
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ID="txtContactNameOc" ValidationType="Required" ValidationGroup="address"
                            DefaultButtonID="btnAddChangeContactOc" runat="server" MaxLength="150"></adv:AdvTextBox>
                    </div>
                </li>
                <% if (SettingsOrderConfirmation.IsShowCountry)
                   {%>
                <li>
                    <div class="param-name">
                        <label for="cboCountryOcs">
                            <%=Resource.Client_MyAccount_Country%></label>
                    </div>
                    <div class="param-value">
                        <adv:AdvDropDownList ID="cboCountryOc" DataTextField="Name" DataValueField="CountryID"
                            runat="server" ValidationType="Required" ValidationGroup="address">
                        </adv:AdvDropDownList>
                    </div>
                </li>
                <% } if (SettingsOrderConfirmation.IsShowState)
                   {%>
                <li>
                    <div class="param-name">
                        <label for="txtContactZoneOc">
                            <%=Resource.Client_MyAccount_Region%></label>
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ID="txtContactZoneOc" ValidationType="Required" ValidationGroup="address"
                            runat="server" CssClass="autocompleteRegion" MaxLength="70" />
                    </div>
                </li>
                <% } if (SettingsOrderConfirmation.IsShowCity)
                   {%>
                <li>
                    <div class="param-name">
                        <label for="txtContactCityOc">
                            <%=Resource.Client_MyAccount_City%></label>
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ID="txtContactCityOc" ValidationType="Required" ValidationGroup="address"
                            runat="server" CssClass="autocompleteCity" MaxLength="70" />
                    </div>
                </li>
                <% } if (SettingsOrderConfirmation.IsShowAddress)
                   {%>
                <li>
                    <div class="param-name">
                        <label for="txtContactAddressOc">
                            <%=Resource.Client_MyAccount_Address%></label>
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ID="txtContactAddressOc" SpanClass="adress-mainindex" ValidationGroup="address"
                            ValidationType="Required" runat="server" TextMode="Multiline" MaxLength="255" />
                    </div>
                </li>
                <% } if (SettingsOrderConfirmation.IsShowZip)
                   {%>
                <li>
                    <div class="param-name">
                        <label for="txtContactZipOc">
                            <%= Resource.Client_MyAccount_Postcode %></label>
                    </div>
                    <div class="param-value">
                        <adv:AdvTextBox ID="txtContactZipOc" ValidationGroup="address" DefaultButtonID="btnAddChangeContactOc"
                            runat="server" MaxLength="70" />
                    </div>
                </li>
                <% } %>
                <li>
                    <div class="param-name">
                    </div>
                    <div class="param-value">
                        <adv:Button ID="btnAddChangeContactOc" Size="Big" Type="Submit" ValidationGroup="address"
                            CssClass="btn-save-pass-my" runat="server" Text="<%$ Resources:Resource, Client_MyAccount_Add %>" />
                    </div>
                </li>
            </ul>
            <asp:HiddenField ID="hfContactIdOc" runat="server" />
        </div>
    </div>
    <script type="text/javascript">
        $(function () {
            getContactsForOC("#contactsDivOc", '<%= PageData != null && PageData.ShippingContact != null ? PageData.ShippingContact.CustomerContactID.ToString() : "0"%>');
        });
    </script>
</div>

<div id="DivRegWithoutAddress" runat="server" class="oc-block">
    <div class="oc-text-left"> Контактные данные </div>
    <ul class="form form-vr">
        <li>
            <!--<div class="param-name">
                E-mail:
            </div>-->
            <div class="param-value" style="margin-bottom: 10px;">
                <asp:Literal runat="server" ID="liRegEmail" />
                <adv:AdvTextBox ValidationType="NewEmail" ID="txtRegEmail" runat="server" />
            </div>
        </li>
        <li>
            <div class="param-value">
                <adv:AdvTextBox ValidationType="Required" ID="txtRegFirstName" runat="server" Placeholder="Имя" />
            </div>
        </li>
        <% if (SettingsOrderConfirmation.IsShowLastName)
           {%>
        <li>
            <div class="param-value">
                <adv:AdvTextBox ValidationType="Required" ID="txtRegLastName" runat="server" Placeholder="Фамилия" />
            </div>
        </li>
        <% } if (SettingsOrderConfirmation.IsShowPatronymic)
           {%>
        <li>
            <div class="param-name">
                <label for="txtRegPatronymic">
                    <%= Resource.Client_Registration_Patronymic%>:</label>
            </div>
            <div class="param-value">
                <adv:AdvTextBox ValidationType="None" ID="txtRegPatronymic" runat="server" />
            </div>
        </li>
        <% } if (SettingsOrderConfirmation.IsShowPhone)
           {%>
        <li>
            <div class="param-value">
                <adv:AdvTextBox ValidationType="Required" ID="txtRegPhone" runat="server" Placeholder="Номер телефона" />
            </div>
        </li>
        <% } if (!SettingsDesign.DisplayCityInTopPanel)
           { %>
        <li>
            <div class="param-name">
                <%= Resource.Client_OrderConfirmationt_City %>:
            </div>
            <div class="param-value">
                <adv:AdvTextBox ValidationType="Required" ID="txtRegCity" runat="server" />
            </div>
        </li>
        <% } %>
    </ul>
</div>
