<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="OrderConfirmationModule.aspx.cs" Inherits="ClientPages.OrderConfirmationModule" %>

<%@ MasterType VirtualPath="MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphGoogleAnalytics" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMain" runat="Server">
    <asp:Literal runat="server" ID="ltGaECommerce" />
    <div class="stroke">
        <div style="text-align: center;">
            <asp:Label ID="lblMessage" runat="server" Visible="False" ForeColor="red"></asp:Label>
            <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False"></asp:Label>
        </div>
        <h1 class="qo-header">
            <asp:Literal runat="server" ID="liPageHead"></asp:Literal></h1>
        <asp:Panel runat="server" ID="pnlContent">
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolderBottom" runat="Server">
    <% if (AdvantShop.Customers.CustomerContext.CurrentCustomer.EMail.Contains("@temp"))
       { %>
    <script type="text/javascript">
            $(function () {
                    var mp = $.advModal({
                        title: "<% = Resources.Resource.Client_OrderConfirmation_Attention %>",
                        buttons: [{ textBtn: "Ok",
                            func: function () {
                                if (UpdateCustomerEmail())
                                    mp.modalClose();
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
            <% } %>

    </script>
</asp:Content>
