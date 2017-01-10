<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeFile="MyAccount.aspx.cs" Inherits="ClientPages.MyAccount" %>
<%@ Import Namespace="AdvantShop.CMS" %>
<%@ Register TagPrefix="adv" TagName="MyAccountCommonInformation" Src="~/UserControls/MyAccount/MyAccountCommonInformation.ascx" %>
<%@ Register TagPrefix="adv" TagName="MyAccountAddressBook" Src="~/UserControls/MyAccount/MyAccountAddressBook.ascx" %>
<%@ Register TagPrefix="adv" TagName="MyAccountOrderHistory" Src="~/UserControls/MyAccount/MyAccountOrderHistory.ascx" %>
<%@ Register TagPrefix="adv" TagName="MyAccountChangePassword"  Src="~/UserControls/MyAccount/MyAccountChangePassword.ascx" %>
<%@ Register TagPrefix="adv" TagName="MyAccountBonusSystem"  Src="~/UserControls/MyAccount/MyAccountBonusSystem.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <h1>
                <%=Resources.Resource.Client_MyAccount_Header%></h1>
            <div class="tabs tabs-vr" data-plugin="tabs">
                <div class="tabs-headers">
                    <div class="tab-header" id="commoninf" data-tabs-header="true">
                        <span class="tab-inside">
                            <%= Resources.Resource.Client_MyAccount_CommonInf %></span><span class="right"></span></div>
                    <div class="tab-header" id="addressbook"  data-tabs-header="true">
                        <span class="tab-inside">
                            <%= Resources.Resource.Client_MyAccount_AddressBook %></span><span class="right"></span></div>
                    <div class="tab-header" id="orderhistory"  data-tabs-header="true" onclick="showOrdersHistory('#orderHistoryForm', '#orderDetailsForm')">
                        <span class="tab-inside">
                            <%= Resources.Resource.Client_MyAccount_OrderHistory %></span><span class="right"></span></div>
                    <div class="tab-header" id="changepassword"  data-tabs-header="true">
                        <span class="tab-inside">
                            <%= Resources.Resource.Client_MyAccount_ChangePassword %></span><span class="right"></span></div>
                    <div class="tab-header" id="bonusTab" runat="server"  data-tabs-header="true">
                        <span class="tab-inside">
                            <%= Resources.Resource.Client_MyAccount_Bonuses %></span><span class="right"></span></div>
                    <asp:Literal runat="server" ID="liDopTabs"></asp:Literal>
                    <%--Сюда могут быть добавлены дополнительные закладки через code behind--%>
                    <div class="myaccount-phone">
                        <div class="fb">
                            <%= Resources.Resource.Client_MyAccount_AnyQuestion %></div>
                        <div class="ph js-location-replacement" data-location-mask="#phone#" <%= InplaceEditor.Phone.Attribute() %>>
                            <%= AdvantShop.Repository.CityService.GetPhone()%>
                        </div>
                    </div>
                </div>
                <div class="tabs-contents" id="tabscontents" runat="server">
                    <div class="tab-content" data-tabs-content="true">
                        <adv:MyAccountCommonInformation ID="MyAccountCommonInformation" runat="server" />
                    </div>
                    <div class="tab-content" data-tabs-content="true">
                        <adv:MyAccountAddressBook ID="MyAccountAddressBook" runat="server" />
                    </div>
                    <div class="tab-content" data-tabs-content="true">
                        <adv:MyAccountOrderHistory ID="MyAccountOrderHistory" runat="server" />
                    </div>
                    <div class="tab-content" data-tabs-content="true">
                        <adv:MyAccountChangePassword ID="MyAccountChangePassword" runat="server" />
                    </div>
                    <div id="bonusContent" runat="server" class="tab-content" data-tabs-content="true">
                        <adv:MyAccountBonusSystem ID="MyAccountBonusSystem" runat="server" />
                    </div>
                    <%--Сюда могут быть добавлены дополнительные закладки через code behind--%>
                </div>
                <asp:Label ID="lblMessage" runat="server" Visible="False" ForeColor="red"></asp:Label>
            </div>
            <input type="hidden" class="tabid" id="tabid" runat="server"/>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolderBottom" runat="server">
    <% if (AdvantShop.Customers.CustomerContext.CurrentCustomer.EMail.Contains("@temp"))
       { %>
    <script type="text/javascript">
        $(function () {
            var mp = $.advModal({
                title: "<% = Resources.Resource.Client_OrderConfirmation_Attention %>",
                buttons: [{ textBtn: "Ok",
                    func: function () {
                        if (UpdateCustomerEmail()) {
                            mp.modalClose();
                            window.location.reload();
                        }
                        return false;
                    },
                    classBtn: "group-email btn-action"
                }],
                htmlContent: "<% = Resources.Resource.Client_OrderConfirmation_EnterContactEmail %><br /><br /><div style='position:relative'>Email:<br /><input type=\"text\" id=\"customerEmail\" class=\"valid-newemail group-email\"/></div>",
                clickOut: false,
                cross: false,
                closeEsc: false
            });
            mp.modalShow();
            validateControlsPos();
        });
    </script>
    <% } %>
</asp:Content>


