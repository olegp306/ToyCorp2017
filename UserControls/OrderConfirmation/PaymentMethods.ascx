<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PaymentMethods.ascx.cs" Inherits="UserControls.OrderConfirmation.PaymentMethods" %>
<%@ Import Namespace="AdvantShop.Payment" %>

<div data-plugin="vis" data-vis-options="{visible:7, textControlShow: '<%= Resources.Resource.Client_OrderConfirmation_MorePayment%>', textControlHide:'<%= Resources.Resource.Client_OrderConfirmation_CollapsePayment %>', itemExtraSelector: function(){ return $(this).find('.checkbox input[type=radio]:checked').length > 0 }}">
    <asp:ListView ID="lvPaymentMethod" runat="server" ItemPlaceholderID="itemPlaceHolder">
        <LayoutTemplate>
            <div class="oc-text-left">Способы оплаты</div>
            <div class="payment-bg">
            <div class="payment-methods">
                <div runat="server" id="itemPlaceHolder" />
            </div>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <div class="method-item js-vis-item" data-payment='<%# Eval("PaymentMethodID ") %>' data-type='<%# Eval("Type").ToString().ToLower() %>'>
                <div class="checkbox">
                    <input type="radio" name="paymentchk" id="<%# Eval("PaymentMethodID ") %>"/>
                    <label for="<%# Eval("PaymentMethodID ") %>"></label>
                </div>
                <div class="shipping-img">
                    <img src='<%# PaymentIcons.GetPaymentIcon((PaymentType)Eval("Type"), Eval("IconFileName.PhotoName") as string , Eval("Name").ToString()) %>'
                        <%# string.Format("alt=\"{0}\" title=\"{0}\"", HttpUtility.HtmlEncode(Eval("Name"))) %> />
                </div>
                <div class="method-info">
                    <div class="method-name">
                        <%#Eval("Name") %>
                    </div>
                    <div class="method-descr">
                        <%#Eval("Description") %>
                    </div>
                </div>
            </div>
        </ItemTemplate>
        <SelectedItemTemplate>
            <div class="method-item js-vis-item" data-payment='<%# Eval("PaymentMethodID ") %>' data-type='<%# Eval("Type").ToString().ToLower() %>'>
                <div class="checkbox">
                    <input type="radio" name="paymentchk" checked="checked" id="<%# Eval("PaymentMethodID ") %>"/>
                    <label for="<%# Eval("PaymentMethodID ") %>"></label>
                </div>
                <div class="shipping-img">
                    <img src='<%# PaymentIcons.GetPaymentIcon((PaymentType)Eval("Type"), Eval("IconFileName.PhotoName") as string , Eval("Name").ToString()) %>'
                        <%# string.Format("alt=\"{0}\" title=\"{0}\"", HttpUtility.HtmlEncode(Eval("Name"))) %> />
                </div>
                <div class="method-info">
                    <div class="method-name">
                        <%#Eval("Name") %>
                    </div>
                    <div class="method-descr">
                        <%#Eval("Description") %>
                    </div>
                </div>
            </div>
        </SelectedItemTemplate>
        <EmptyDataTemplate>
            <div class="payment-methods">
                <span class="oc-no-way">
                    <asp:Literal ID="lblNoShipping" runat="server" Text="<%$ Resources:Resource, Client_OrderConfirmation_NoPaymentMethod %>" /></span>
            </div>
        </EmptyDataTemplate>
    </asp:ListView>
    <a class="js-vis-control vis-control oc-vis-control" href="javascript:void(0);"><%= Resources.Resource.Client_OrderConfirmation_MorePayment%></a>
</div>
<asp:HiddenField ID="hfPaymentMethodId" runat="server" />
