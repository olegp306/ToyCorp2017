<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="ShippingMethod.aspx.cs" Inherits="Admin.EditShippingMethod" %>
<%@ Reference Control="~/Admin/UserControls/ShippingMethods/MasterControl.ascx" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="Resources" %>
<%@ Register Src="~/Admin/UserControls/ShippingMethods/MasterControl.ascx" TagName="ShippingMethod" TagPrefix="adv" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Head" runat="Server">
    <script type="text/javascript">
        $(function () {
            $(".ShippingType").hover(function () {
                    $(this).addClass("ptHovered");
                },
                function () {
                    $(this).removeClass("ptHovered");
                });

            $.advModal({
                title: "<%= Resources.Resource.Admin_ShippingMethod_Adding %>",
                control: $("#<%= btnAddMethod.ClientID%>"),
                htmlContent: $("#<%= modal.ClientID%>"),
                beforeOpen: clearModal,
                clickOut: false
            });
        });

        function Ok() { }

        function clearModal() {

            $("#<%= txtName.ClientID %>").val("");
            $("#<%= txtDescription.ClientID %>").val("");
            $("#<%= txtSortOrder.ClientID %>").val("");
            $("#<%= txtName.ClientID %>").focus();
        }
        function showElement(span) {
            var method_id = $("input:hidden", $(span)).val();
            if ($("hfShipping" + method_id).length == 0)
                location = "ShippingMethod.aspx?ShippingMethodID=" + method_id;
        }   
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
    <div id="divModal" style="display: none;">
        <asp:Panel ID="modal" runat="server">
            <div style="background-color: white; padding-top: 10px; padding-bottom: 10px; text-align: center;">
                <table width="470px;">
                    <tr>
                        <td style="width: 150px;">
                            <asp:Label ID="Label1" runat="server" Text="<%$  Resources:Resource, Admin_ShippingMethods_Name %>"></asp:Label><span
                                class="required">*</span>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtName" Width="300"></asp:TextBox>
                            <asp:RequiredFieldValidator Display="Dynamic" ID="RequiredFieldValidator1" runat="server"
                                ControlToValidate="txtName" EnableClientScript="true" Style="display: inline;"
                                ErrorMessage='<%$ Resources: Resource, Admin_ShippingMethod_NameRequired %>'></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="<%$  Resources:Resource, Admin_ShippingMethods_Type %>"></asp:Label><span
                                class="required">*</span>
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlType" DataTextField="name" DataValueField="value"
                                Width="300px" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="<%$  Resources:Resource, Admin_ShippingMethod_Description %>"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtDescription" TextMode="MultiLine" Width="300"
                                Height="100"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top; padding: 5px 0 0 0;">
                            <asp:Label ID="Label4" runat="server" Text="<%$  Resources:Resource, Admin_ShippingMethod_SortOrder %>"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtSortOrder" Width="300"></asp:TextBox>
                            <asp:RegularExpressionValidator Display="Dynamic" ID="RegularExpressionValidator1"
                                runat="server" ControlToValidate="txtSortOrder" EnableClientScript="true" ValidationExpression="[0-9]*"
                                ErrorMessage="<%$ Resources: Resource, Admin_SortOrder_MustBeNumeric %>"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align: right;">
                            <asp:LinkButton runat="server" Width="130" ID="btnOk" Text="<%$ Resources: Resource, Admin_ShippingMethod_Create %>"
                                OnClick="btnOk_Click" OnClientClick="return Page_ClientValidate();" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </div>
    <script>
        $("form").on("submit", function(e) {
            var a = e;
        });

    </script>
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item"><a href="CommonSettings.aspx">
                <%= Resource.Admin_MasterPageAdmin_Settings%></a></li>
            <li class="neighbor-menu-item"><a href="CheckoutFields.aspx">
                <%= Resource.Admin_MasterPageAdmin_CheckoutFields%></a></li>
            <li class="neighbor-menu-item"><a href="PaymentMethod.aspx">
                <%= Resource.Admin_MasterPageAdmin_PaymentMethod%></a></li>
            <li class="neighbor-menu-item selected"><a href="ShippingMethod.aspx">
                <%= Resource.Admin_MasterPageAdmin_ShippingMethod%></a></li>
            <li class="neighbor-menu-item"><a href="Country.aspx">
                <%= Resource.Admin_MasterPageAdmin_Countries%></a></li>
            <li class="neighbor-menu-item"><a href="Currencies.aspx">
                <%= Resource.Admin_MasterPageAdmin_Currency%></a></li>
            <li class="neighbor-menu-item"><a href="Taxes.aspx">
                <%= Resource.Admin_MasterPageAdmin_Taxes%></a></li>
            <li class="neighbor-menu-item"><a href="MailFormat.aspx">
                <%= Resource.Admin_MasterPageAdmin_MailFormat%></a></li>
            <li class="neighbor-menu-item"><a href="LogViewer.aspx">
                <%= Resource.Admin_MasterPageAdmin_BugTracker%></a></li>
            <li class="neighbor-menu-item"><a href="301Redirects.aspx">
                <%= Resource.Admin_MasterPageAdmin_301Redirects%></a></li>
        </menu>
    </div>
    <div class="content-own">
        <table style="width: 100%; table-layout: fixed;" cellpadding="0" cellspacing="0">
            <tbody>
                <tr>
                    <td style="width: 72px;">
                        <img src="images/customers_ico.gif" alt="" />
                    </td>
                    <td>
                        <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_Header %>"></asp:Label><br />
                        <asp:Label ID="lblShippingMethod" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_SubHeader %>"></asp:Label>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="lblMessage" Visible="false" ForeColor="Blue"></asp:Label>
                    </td>
                    <td>
                        <div class="btns-main">
                            <asp:LinkButton CssClass="btn btn-middle btn-add" ID="btnAddMethod" runat="server"
                                Text="<%$ Resources:Resource, Admin_Add %>" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; width: 100%" colspan="4">
                        <table cellpadding="0px" cellspacing="0px" style="width: 100%;">
                            <tr>
                                <td style="vertical-align: top; width: 225px;">
                                    <ul class="tabs" id="tabs-headers">
                                        <asp:Repeater runat="server" ID="rptTabs">
                                            <ItemTemplate>
                                                <li id="Li1" runat="server" onclick="javascript:showElement(this)" class='<%# (int)Eval("ShippingMethodID") == ShippingMethodID ? "selected" : "" %>'
                                                    style="">
                                                    <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Eval("ShippingMethodID") %>' />
                                                    <asp:Label ForeColor='<%# (bool)Eval("Enabled") ? Color.Black : Color.Gray %>' ID="Literal4"
                                                        runat="server" Text='<%# Eval("Name") %>' />
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </ul>
                                </td>
                                <td class="tabContainer" id="tabs-contents">
                                    <asp:Label ID="lblError" runat="server" CssClass="mProductLabelInfo" ForeColor="Red"
                                        Visible="False" Font-Names="Verdana" Font-Size="14px" EnableViewState="false"></asp:Label>
                                    <asp:Panel runat="server" ID="pnMethods">
                                        <asp:Panel runat="server" ID="pnEmpty" Visible="False">
                                            <span>
                                                <% = Resource.Admin_ShippingMethod_EmptyShipping %></span>
                                        </asp:Panel>
                                        <adv:ShippingMethod runat="server" ID="ucFeedEx" ShippingType="FedEx" OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucUsps" ShippingType="Usps" OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucUPS" ShippingType="UPS" OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucFixedRate" ShippingType="FixedRate" OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucFreeShipping" ShippingType="FreeShipping"
                                            OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucByWeight" ShippingType="ShippingByWeight"
                                            OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucEdost" ShippingType="Edost" OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucByShippingCost" ShippingType="ShippingByShippingCost"
                                            OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucByOrderPrice" ShippingType="ShippingByOrderPrice"
                                            OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucShippingByRangeWeightAndDistance" ShippingType="ShippingByRangeWeightAndDistance"
                                            OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucShippingNovaPoshta" ShippingType="ShippingNovaPoshta"
                                            OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucSelfDelivery" ShippingType="SelfDelivery"
                                            OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucCdek" ShippingType="Cdek" OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucMultiship" ShippingType="Multiship" OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucByProductAmount" ShippingType="ShippingByProductAmount"
                                            OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucByEmsPost" ShippingType="ShippingByEmsPost"
                                            OnSaved="ShippingMethod_Saved" />
                                        <adv:ShippingMethod runat="server" ID="ucCheckoutRu" ShippingType="CheckoutRu" OnSaved="ShippingMethod_Saved" />
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</asp:Content>