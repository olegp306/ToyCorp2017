<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShippingByRangeWeightAndDistance.ascx.cs" Inherits="Admin.UserControls.ShippingMethods.ShippingByRangeWeightAndDistanceControl" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<table border="0" cellpadding="2" cellspacing="0" style="width: 99%; margin-left: 5px; margin-top: 5px;">
    <tr class="rowPost">
        <td colspan="3" style="height: 34px;">
            <h4 style="display: inline; font-size: 12pt;">
                <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethods_HeadSettings %>"></asp:Localize></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <span><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_BasePrice %></span>
        </td>
        <td class="columnVal" style="width: 500px">
            <table border="0" cellpadding="2" cellspacing="0">
                <tr>
                    <td><%= Resources.Resource. Admin_ShippingMethod_ShippingByRangeWeightAndDistance_UpTo %>:
                        <asp:TextBox runat="server" ID="txtAmount" Width="60" ValidationGroup="byPrice" />
                        <%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_Kg %>
                    </td>
                    <td><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_Price %>:
                        <asp:TextBox runat="server" ID="txtPrice" Width="60" ValidationGroup="byPrice" />
                    </td>
                    <td><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_WeightPerUnit %>
                        <asp:CheckBox runat="server" ID="chbShippingPerUnit" />
                    </td>

                    <td style="text-align: center">
                        <asp:Button runat="server" ID="btnAdd" OnClick="btnAdd_Click" Text="<%$ Resources:Resource, Admin_ShippingMethod_ShippingByOrderPrice_Add %>"
                            ValidationGroup="byPrice" />
                    </td>
                </tr>
                <asp:Repeater runat="server" ID="rWeightLimits" OnItemCommand="rWeightLimits_Delete">
                    <ItemTemplate>
                        <tr>
                            <td style="border-right-style: solid; border-right-width: 1px; border-right-color: black;"><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_UpTo %>: <%#  Eval("Amount") %> <%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_Kg %>
                            </td>
                            <td style="border-right-style: solid; border-right-width: 1px; border-right-color: black; text-align: center"><%# CatalogService.GetStringPrice((float)Eval("Price")) %>
                            </td>
                            <td style="border-right-style: solid; border-right-width: 1px; border-right-color: black; text-align: center">
                                <%# !(bool)Eval("PerUnit") ? "" : Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_WeightPerUnit %>
                            </td>
                            <td style="text-align: center">
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../../images/remove.jpg" ToolTip="<%$ Resources:Resource, Admin_ShippingMethod_ShippingByOrderPrice_Delete %>"
                                    CommandName="DeleteWeightLimits" CommandArgument='<%# Container.ItemIndex %>' CausesValidation="false" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <br />
        </td>
        <td class="columnDescr"></td>
    </tr>
    <tr>
        <td class="columnName"><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_ConsideringTheDistance %>
        </td>
        <td class="columnName">
            <asp:CheckBox runat="server" ID="chbUseDistance" />
        </td>
        <td class="columnDescr"></td>
    </tr>
    <tr>
        <td class="columnName"><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_DepensOnDistance %>
        </td>
        <td class="columnVal" style="width: 80%">
            <table border="0" cellpadding="2" cellspacing="0">
                <tr>
                    <td><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_UpTo %>:
                        <asp:TextBox runat="server" ID="txtDistanse" Width="60" ValidationGroup="byPrice2" /><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_Km %>
                    </td>
                    <td><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_Price %>:
                        <asp:TextBox runat="server" ID="txtDistansePrice" Width="60" ValidationGroup="byPrice2" />

                    </td>
                    <td><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_PerUnit %>
                        <asp:CheckBox runat="server" ID="chbPerUnit" />
                    </td>
                    <td style="text-align: center">
                        <asp:Button runat="server" ID="btnAddD" OnClick="btnAddD_Click" Text="<%$ Resources:Resource, Admin_ShippingMethod_ShippingByOrderPrice_Add %>"
                            ValidationGroup="byPrice2" />
                    </td>
                </tr>
                <asp:Repeater runat="server" ID="rDistanceLimits" OnItemCommand="rDistanceLimits_Delete">
                    <ItemTemplate>
                        <tr>
                            <td style="border-right-style: solid; border-right-width: 1px; border-right-color: black;"><%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_UpTo %>: <%#  Eval("Amount") %> <%= Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_Km %>
                            </td>
                            <td style="border-right-style: solid; border-right-width: 1px; border-right-color: black; text-align: center"><%# CatalogService.GetStringPrice((float)Eval("Price")) %>
                            </td>
                            <td style="border-right-style: solid; border-right-width: 1px; border-right-color: black; text-align: center">
                                <%# !(bool)Eval("PerUnit") ? "" : Resources.Resource.Admin_ShippingMethod_ShippingByRangeWeightAndDistance_PerUnit %>
                            </td>
                            <td style="text-align: center">
                                <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="../../images/remove.jpg" ToolTip="<%$ Resources:Resource, Admin_ShippingMethod_ShippingByOrderPrice_Delete %>"
                                    CommandName="DeleteWeightLimits" CommandArgument='<%# Container.ItemIndex %>' CausesValidation="false" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </td>
        <td class="columnDescr"></td>
    </tr>
    <tr>
        <td colspan="2">&nbsp; 
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_ShippingTerm %>"></asp:Localize><span
                class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtDeliveryTime" Width="250" ValidationGroup="5"></asp:TextBox>
        </td>
        <td class="columnDescr">&nbsp;
        </td>
    </tr>
</table>
<br />
<br />
