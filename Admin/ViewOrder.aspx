<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true"
    CodeFile="ViewOrder.aspx.cs" Inherits="Admin.ViewOrder" %>

<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Import Namespace="AdvantShop.Orders" %>
<%@ Import Namespace="Resources" %>
<%@ Register Src="UserControls/Order/OrdersSearch.ascx" TagName="OrdersSearch" TagPrefix="adv" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item selected"><a href="OrderSearch.aspx">
                <%= Resource.Admin_MasterPageAdmin_Orders%></a></li>
            <li class="neighbor-menu-item"><a href="OrderStatuses.aspx">
                <%= Resource.Admin_MasterPageAdmin_OrderStatuses%></a></li>
            <li class="neighbor-menu-item"><a href="OrderByRequest.aspx">
                <%= Resource.Admin_MasterPageAdmin_OrderByRequest%></a></li>
            <li class="neighbor-menu-item"><a href="ExportOrdersExcel.aspx">
                <%= Resource.Admin_MasterPageAdmin_OrdersExcelExport%></a></li>
            <li class="neighbor-menu-item"><a href="Export1C.aspx">
                <%= Resource.Admin_MasterPageAdmin_1CExport%></a></li>
        </menu>
        <div class="panel-add">
            <a href="EditOrder.aspx?OrderID=addnew" class="panel-add-lnk">
                <%= Resource.Admin_MasterPageAdmin_Add %>
                <%= Resource.Admin_MasterPageAdmin_Order %></a>
        </div>
    </div>
    <ul class="two-column">
        <li class="two-column-item">
            <adv:OrdersSearch ID="OrdersSearch" runat="server" />
        </li>
        <li class="two-column-item">
            <div id="notify" style="position: absolute; top: 5px; right: 5px; width: 350px;">
            </div>
            <ul class="justify order-dashboard-row">
                <li class="justify-item">
                    <asp:Panel ID="pnlOrderNumber" runat="server" CssClass="order-main">
                        <div class="order-main-number">
                            <%= Resource.Admin_ViewOrder_ItemNum %>
                            <asp:Label ID="lblOrderId" runat="server"></asp:Label>
                        </div>
                        <%=Resource.Admin_ViewOrder_Date %>
                        <asp:Label ID="lblOrderDate" runat="server"></asp:Label>
                        <div>
                            <%= Resource.Admin_ViewOrder_Number %>
                            <asp:Label ID="lblOrderNumber" runat="server"></asp:Label>
                        </div>
                    </asp:Panel>
                    <div class="select-order-status">
                        <%= Resource.Admin_ViewOrder_OrderStatus %>
                        <asp:DropDownList ID="ddlViewOrderStatus" runat="server" DataTextField="StatusName"
                            DataValueField="StatusID" CssClass="ddlViewOrderStatus" />
                        <div>
                            <a id="lnkSendMail" class="vieworder-send-mail" href="javascript:void(0)" onclick="<%= string.Format("sendMailOrderStatus({0})",  OrderId)%>">
                                <%= Resources.Resource.Admin_ViewOrder_SendConfirmation %></a>
                        </div>
                        
                        <div id="divUseIn1C" runat="server" style="padding: 10px 0 5px 0">
                            <label><%=Resource.Admin_ViewOrder_UseIn1C%> <asp:CheckBox ID="chkUseIn1C" runat="server" CssClass="useIn1c" /></label>
                            <div id="divStatus1C" runat="server" Visible="False" style="padding: 10px 0 5px 0">
                                <%= Resource.Admin_ViewOrder_StatusIn1C %>: <asp:Label runat="server" ID="lbl1CStatus" />
                            </div>
                        </div>
                    </div>
                </li>
                <li class="justify-item">
                    <div class="dropdown-order-do">
                        <div class="dropdown-arrow-neutral dropdown-menu-parent icon-cogwheel">
                            <%= Resource.Admin_ViewOrder_Action%>
                            <div class="dropdown-menu-wrap">
                                <ul class="dropdown-menu">
                                    <li class="dropdown-menu-item">
                                        <asp:HyperLink ID="lnkEditOrder" runat="server"><%= Resource.Admin_Edit%></asp:HyperLink></li>
                                    <li class="dropdown-menu-item"><a href="#" onclick="javascript:open_printable_version('../PrintOrder.aspx?OrderNumber=<%= OrderNumber %>&order=details');return false;">
                                        <%=Resource.Admin_ViewOrder_PrintOrder%></a></li>
                                    <li class="dropdown-menu-item">
                                        <asp:HyperLink ID="lnkExportToExcel" runat="server"><%= Resource.Admin_ViewOrder_ExportToExcel %></asp:HyperLink></li>
                                    <li class="dropdown-menu-item">
                                        <asp:LinkButton ID="lnkDelete" runat="server" OnClick="lnkDelete_Click" CssClass="valid-confirm"
                                            data-confirm="<%$ Resources:Resource, Admin_ViewOrder_Confirmation %>"><%= Resource.Admin_Delete %></asp:LinkButton>
                                    <li id="liMultiship" runat="server" visible="False" class="dropdown-menu-item"><a
                                        href="javascript:void(0)" class="multiship-create-order" data-value="<%= OrderId%>">
                                        <%= Resource.Admin_ViewOrder_CreateMultishipOrder%></a> </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="order-status-checkout">
                        <div data-plugin="radiolist" class="radiolist radiolist-big radiolist-checkout">
                            <%= RenderPaidButtons() %>
                        </div>
                    </div>
                </li>
            </ul>
            <div class="order-buyer-name">
                <div class="list-order-data-caption">
                    <%= Resource.Admin_ViewOrder_Customer %>
                </div>
                <ul class="list-order-data">
                    <li class="list-order-data-row">
                        <div class="list-order-data-name">
                            <%= Resource.Admin_ViewOrder_CustomerName%>:
                        </div>
                        <div class="list-order-data-value">
                            <asp:HyperLink ID="lnkCustomerName" runat="server" Target="_blank"></asp:HyperLink>
                            <asp:Label ID="lblCustomerName" runat="server"></asp:Label>
                        </div>
                    </li>
                    <li class="list-order-data-row">
                        <div class="list-order-data-name">
                            <%= Resource.Admin_ViewOrder_Email %>:
                        </div>
                        <div class="list-order-data-value">
                            <asp:HyperLink ID="lnkCustomerEmail" runat="server" CssClass="order-email" Target="_blank"></asp:HyperLink>
                            <asp:Label ID="lblCustomerEmail" runat="server"></asp:Label>
                        </div>
                    </li>
                    <li class="list-order-data-row">
                        <div class="list-order-data-name">
                            <%= SettingsOrderConfirmation.CustomerPhoneField %>
                        </div>
                        <div class="list-order-data-value">
                            <asp:Label ID="lblCustomerPhone" runat="server"></asp:Label>
                        </div>
                    </li>
                </ul>
            </div>
            <ul class="order-list-address">
                <li class="order-list-address-item">
                    <div class="list-order-data-caption">
                        <%= Resource.Admin_ViewOrder_ShippingAddress %>
                    </div>
                    <% if (order != null && order.ShippingContact != null)
                       { %>
                    <ul class="list-order-data">
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= Resource.Admin_ViewOrder_Country %>
                            </div>
                            <div class="list-order-data-value">
                                <asp:Label ID="lblShippingCountry" runat="server"></asp:Label>
                            </div>
                        </li>
                        <% if (order.ShippingContact.Zone.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= Resource.Admin_ViewOrder_Zone %>
                            </div>
                            <div class="list-order-data-value">
                                <asp:Label ID="lblShippingRegion" runat="server"></asp:Label>
                            </div>
                        </li>
                        <% } if (order.ShippingContact.City.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= Resource.Admin_ViewOrder_City %>
                            </div>
                            <div class="list-order-data-value">
                                <asp:Label ID="lblShippingCity" runat="server"></asp:Label>
                            </div>
                        </li>
                        <% } if (order.ShippingContact.Zip.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%=Resource.Admin_ViewOrder_Zip %>
                            </div>
                            <div class="list-order-data-value">
                                <asp:Label ID="lblShippingZipCode" runat="server"></asp:Label>
                            </div>
                        </li>
                        <% } if (order.ShippingContact.Address.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= Resource.Admin_ViewOrder_Address %>
                            </div>
                            <div class="list-order-data-value">
                                <asp:Label ID="lblShippingAddress" runat="server"></asp:Label>
                                <asp:HyperLink ID="lnkShippingAddressOnMap" runat="server" Target="_blank"><img class="order-map" src="images/new_admin/cliparts/map.jpg"
                                    alt="" /></asp:HyperLink>
                            </div>
                        </li>
                        <% }
                           if (order.ShippingContact.CustomField1.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= SettingsOrderConfirmation.CustomShippingField1 %>
                            </div>
                            <div class="list-order-data-value">
                                <%= order.ShippingContact.CustomField1%>
                            </div>
                        </li>
                        <% } if (order.ShippingContact.CustomField1.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= SettingsOrderConfirmation.CustomShippingField2 %>
                            </div>
                            <div class="list-order-data-value">
                                <%= order.ShippingContact.CustomField2%>
                            </div>
                        </li>
                        <% } if (order.ShippingContact.CustomField1.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= SettingsOrderConfirmation.CustomShippingField3 %>
                            </div>
                            <div class="list-order-data-value">
                                <%= order.ShippingContact.CustomField3%>
                            </div>
                        </li>
                        <% } %>
                    </ul>
                    <% } %>
                    <asp:Label ID="lblCheckoutAdressNotice" runat="server" Visible="false" Text="Для корректной работы Checkout.ru, адрес доставки должен начинаться с названия улицы. Например 'Ленинский проспект' или 'Промышленная улица'" ForeColor="blue"></asp:Label>
                </li>
                <li class="order-list-address-item">
                    <div class="list-order-data-caption">
                        <%= Resource.Admin_ViewOrder_BillingAddress %>
                    </div>
                    <% if (order != null && order.BillingContact != null)
                       { %>
                    <ul class="list-order-data">
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= Resource.Admin_ViewOrder_Country %>
                            </div>
                            <div class="list-order-data-value">
                                <asp:Label ID="lblBuyerCountry" runat="server"></asp:Label>
                            </div>
                        </li>
                        <% if (order.BillingContact.Zone.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= Resource.Admin_ViewOrder_Zone %>
                            </div>
                            <div class="list-order-data-value">
                                <asp:Label ID="lblBuyerRegion" runat="server"></asp:Label>
                            </div>
                        </li>
                        <% } if (order.BillingContact.City.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= Resource.Admin_ViewOrder_City %>
                            </div>
                            <div class="list-order-data-value">
                                <asp:Label ID="lblBuyerCity" runat="server"></asp:Label>
                            </div>
                        </li>
                        <% } if (order.BillingContact.Zip.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%=Resource.Admin_ViewOrder_Zip %>
                            </div>
                            <div class="list-order-data-value">
                                <asp:Label ID="lblBuyerZip" runat="server"></asp:Label>
                            </div>
                        </li>
                        <% } if (order.BillingContact.Address.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= Resource.Admin_ViewOrder_Address %>
                            </div>
                            <div class="list-order-data-value">
                                <asp:Label ID="lblBuyerAddress" runat="server"></asp:Label>
                                <asp:HyperLink ID="lnkBuyerAddressOnMap" runat="server" Target="_blank"><img class="order-map" src="images/new_admin/cliparts/map.jpg"
                                    alt="" /></asp:HyperLink>
                            </div>
                        </li>
                        <% }
                           if (order.BillingContact.CustomField1.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= SettingsOrderConfirmation.CustomShippingField1 %>
                            </div>
                            <div class="list-order-data-value">
                                <%= order.BillingContact.CustomField1%>
                            </div>
                        </li>
                        <% } if (order.BillingContact.CustomField1.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= SettingsOrderConfirmation.CustomShippingField2 %>
                            </div>
                            <div class="list-order-data-value">
                                <%= order.BillingContact.CustomField2%>
                            </div>
                        </li>
                        <% } if (order.BillingContact.CustomField1.IsNotEmpty())
                           { %>
                        <li class="list-order-data-row">
                            <div class="list-order-data-name">
                                <%= SettingsOrderConfirmation.CustomShippingField3 %>
                            </div>
                            <div class="list-order-data-value">
                                <%= order.BillingContact.CustomField3%>
                            </div>
                        </li>
                        <% } %>
                    </ul>
                    <% } %>
                </li>
                <li class="order-list-address-item">
                    <div class="list-order-data-caption">
                        <% if (ShippingTypeIsCdek)
                           { %>
                        <div class="dropdown-menu-parent icon-cogwheel">
                            <%= Resource.Admin_ViewOrder_ShippingMethodName %>
                            <div class="dropdown-menu-wrap">
                                <ul class="dropdown-menu">
                                    <li class="dropdown-menu-item"><a href="javascript:void(0)" class="cdek-send-order"
                                        data-value="<%= OrderId %>" data-tariff="<%= OrderPickPoint != null ? OrderPickPoint.AdditionalData : string.Empty %>"
                                        data-pickpoint="<%= OrderPickPoint != null ? OrderPickPoint.PickPointId : string.Empty %>">
                                        <%= Resource.Admin_ViewOrder_CreateCdekOrder %></a> </li>
                                    <li class="dropdown-menu-item"><a href="javascript:void(0)" class="cdek-reportstatus-order"
                                        data-value="<%= OrderId %>" onclick="getCdekOrderReportStatus('<%= OrderId %>')">Скачать отчет «Статусы заказов»</a> </li>
                                    <li class="dropdown-menu-item"><a href="javascript:void(0)" class="cdek-reportinfo-order"
                                        onclick="getCdekOrderReportInfo('<%= OrderId %>')">Скачать отчет «Информация по заказам»</a> </li>
                                    <li class="dropdown-menu-item"><a href="javascript:void(0)" class="cdek-printform-order"
                                        onclick="getCdekOrderPrintform('<%= OrderId %>')">Скачать печатную форму квитанции к заказу</a> </li>
                                    <li class="dropdown-menu-item"><a href="javascript:void(0)" class="cdek-callcustomer-order" data-value="<%= OrderId %>">Прозвон получателя</a> </li>
                                    <li class="dropdown-menu-item"><a href="javascript:void(0)" class="cdek-delete-order" data-value="<%= OrderId %>">Удаленить заказ из системы СДЭК</a> </li>
                                </ul>
                            </div>
                        </div>
                        <% }
                           else if (ShippingTypeIsCheckout)
                           { %>
                        <div class="dropdown-menu-parent icon-cogwheel">
                            <%= Resource.Admin_ViewOrder_ShippingMethodName %>
                            <div class="dropdown-menu-wrap">
                                <ul class="dropdown-menu">
                                    <li class="dropdown-menu-item"><a href="javascript:void(0)" class="checkout-send-order"
                                        data-value="<%= OrderId %>" data-tariff="<%= OrderPickPoint != null ? OrderPickPoint.AdditionalData : string.Empty %>"
                                        data-pickpoint="<%= OrderPickPoint != null ? OrderPickPoint.PickPointId : string.Empty %>">
                                        <%= Resource.Admin_ViewOrder_CreateCheckoutOrder %></a> </li>
                                </ul>
                            </div>
                        </div>
                        <% }
                           else
                           { %>
                        <%= Resource.Admin_ViewOrder_ShippingMethodName %>
                        <% } %>
                    </div>
                    <div>
                        <asp:Label ID="lblShippingMethodName" runat="server"></asp:Label>
                    </div>
                    <div class="list-order-data-caption order-payment-method">
                        <div class="dropdown-menu-parent icon-cogwheel">
                            <%= Resource.Admin_ViewOrder_PaymentMethodName %>
                            <div class="dropdown-menu-wrap">
                                <ul class="dropdown-menu">
                                    <li class="dropdown-menu-item">
                                        <a href="<%= "../billing.aspx?orderid=" + OrderId + "&hash=" + OrderService.GetBillingLinkHash(order) %>" target="_blank">
                                            <%= Resource.Admin_ViewOrder_GoToPayment %></a>
                                    </li>
                                    <li id="liSendBillingLink" runat="server" class="dropdown-menu-item">
                                        <a href="javascript:void(0)" class="send-billing-link" data-value="<%= OrderId%>"><%= Resource.Admin_ViewOrder_SendBillingLink %></a>
                                    </li>
                                </ul>
                            </div>
                        </div>

                    </div>
                    <div>
                        <asp:Label ID="lblPaymentMethodName" runat="server"></asp:Label>
                    </div>
                </li>
            </ul>

            <div class="order-table-wrap">
                <asp:ListView ID="lvOrderItems" runat="server" ItemPlaceholderID="itemPlaceholderID">
                    <LayoutTemplate>
                        <table class="table-ui-simple">
                            <caption>
                                <asp:Label runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_OrderContent %>"></asp:Label></caption>
                            <thead>
                                <tr>
                                    <th>
                                        <asp:Label runat="server" Text="<%$ Resources: Resource, Admin_ViewOrder_Product %>"></asp:Label>
                                    </th>
                                    <th class="table-ui-simple-align-center">
                                        <asp:Label ID="Label2" runat="server" Text="<%$ Resources: Resource, Admin_ViewOrder_PriceForUnit %>"></asp:Label>
                                    </th>
                                    <th class="table-ui-simple-align-center">
                                        <asp:Label runat="server" Text="<%$ Resources: Resource, Admin_ViewOrder_ItemAmount %>"></asp:Label>
                                    </th>
                                    <th class="table-ui-simple-align-center">
                                        <asp:Label runat="server" Text="<%$ Resources: Resource, Admin_ViewOrder_ItemCost %>"></asp:Label>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr runat="server" id="itemPlaceholderID">
                                </tr>
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <a href='<%# UrlService.GetAdminAbsoluteLink("product.aspx?productid=" + Eval("ProductID")) %>'
                                    class="order-item-photo" target="_blank">
                                    <%# Eval("ProductID") != null ? RenderPicture(SQLDataHelper.GetInt(Eval("ProductID")), SQLDataHelper.GetNullableInt(Eval("PhotoID"))) : ""%></a>
                                <div class="order-item-info">
                                    <div class="order-item-name">
                                        <a href='<%# UrlService.GetAdminAbsoluteLink("product.aspx?productid=" + Eval("ProductID")) %>'
                                            class="order-item-lnk" target="_blank">
                                            <%#Eval("Name") %></a>
                                    </div>
                                    <div>
                                        <%#Eval("ArtNo") %>
                                    </div>
                                    <div class="order-item-options">
                                        <div>
                                            <%# !string.IsNullOrEmpty(Convert.ToString(Eval("Color"))) ? "<span>" + SettingsCatalog.ColorsHeader + ":</span> " + Eval("Color") : string.Empty %>
                                        </div>
                                        <div>
                                            <%# !string.IsNullOrEmpty(Convert.ToString(Eval("Size"))) ? "<span>" + SettingsCatalog.SizesHeader + ":</span> " + Eval("Size") : string.Empty%>
                                        </div>
                                        <div>
                                            <%# RenderSelectedOptions((IList<EvaluatedCustomOptions>)Eval("SelectedOptions")) %>
                                        </div>
                                    </div>
                                </div>
                            </td>
                            <td class="table-ui-simple-align-center table-ui-simple-bold">
                                <%# CatalogService.GetStringPrice(Convert.ToSingle(Eval("Price")), 1, CurrencyCode, CurrencyValue) %>
                            </td>
                            <td class="table-ui-simple-align-center">
                                <%# Convert.ToSingle(Eval("Amount")) %>
                            </td>
                            <td class="table-ui-simple-align-center table-ui-simple-bold">
                                <%# CatalogService.GetStringPrice(Convert.ToSingle(Eval("Price")), Convert.ToSingle(Eval("Amount")), CurrencyCode, CurrencyValue)%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
                <asp:ListView ID="lvOrderCertificates" runat="server" ItemPlaceholderID="itemPlaceholderID">
                    <LayoutTemplate>
                        <table class="table-ui-simple">
                            <caption>
                                <asp:Label runat="server" Text="<%$ Resources:Resource,Admin_ViewOrder_OrderContent %>"></asp:Label></caption>
                            <thead>
                                <tr>
                                    <th></th>
                                    <th class="table-ui-simple-align-center">
                                        <asp:Label runat="server" Text="<%$ Resources: Resource, Admin_ViewOrder_CertificateCode %>"></asp:Label>
                                    </th>
                                    <th class="table-ui-simple-align-center">
                                        <asp:Label runat="server" Text="<%$Resources: Resource, Admin_ViewOrder_GiftCertificateSum %>"></asp:Label>
                                    </th>
                                    <th class="table-ui-simple-align-center">
                                        <asp:Label runat="server" Text="<%$Resources: Resource, Admin_ViewOrder_GiftCertificateApplyInOrder %>"></asp:Label>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr runat="server" id="itemPlaceholderID">
                                </tr>
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%= Resource. Admin_ViewOrder_Certificate %>
                            </td>
                            <td class="table-ui-simple-align-center table-ui-simple-bold">
                                <%# Eval("CertificateCode") %>
                            </td>
                            <td class="table-ui-simple-align-center">
                                <%# CatalogService.GetStringPrice(Convert.ToSingle(Eval("Sum")), 1, CurrencyCode, CurrencyValue)%>
                            </td>
                            <td class="table-ui-simple-align-center table-ui-simple-bold">
                                <%# Eval("ApplyOrderNumber") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
            </div>
            <div class="order-result-wrap clearfix">
                <div class="order-comment">
                    <div class="order-comment-title">
                        <%= Resource.Admin_ViewOrder_UserComment %>
                    </div>
                    <div>
                        <asp:Label ID="lblUserComment" runat="server" Text="<%$ Resources: Resource, Admin_OrderSearch_NoComment %>">
                        </asp:Label>
                    </div>
                </div>
                <ul class="order-result-list">
                    <li class="order-result-list-row">
                        <div class="order-result-name">
                            <%= Resource.Admin_ViewOrder_ItemCost2 %>:
                        </div>
                        <div class="order-result-value">
                            <asp:Label ID="lblTotalOrderPrice" runat="server"></asp:Label>
                        </div>
                    </li>
                    <li class="order-result-list-row" runat="server" id="trDiscount">
                        <div class="order-result-name">
                            <%= Resource.Admin_ViewOrder_ItemDiscount %>:
                            <asp:Label ID="lblOrderDiscountPercent" runat="server" CssClass="order-result-discount"></asp:Label>
                        </div>
                        <div class="order-result-value">
                            <asp:Label ID="lblOrderDiscount" runat="server"></asp:Label>
                        </div>
                    </li>
                    <li class="order-result-list-row" runat="server" id="trBonuses">
                        <div class="order-result-name">
                            <%= Resource.Admin_ViewOrder_Bonuses%>:
                        </div>
                        <div class="order-result-value">
                            <asp:Label ID="lblOrderBonuses" runat="server"></asp:Label>
                        </div>
                    </li>
                    <li class="order-result-list-row" runat="server" id="trCertificatePrice" visible="False">
                        <div class="order-result-name">
                            <%=Resource. Admin_ViewOrder_Certificate %>:
                        </div>
                        <div class="order-result-value">
                            <asp:Label ID="lblCertificatePrice" runat="server"></asp:Label>
                        </div>
                    </li>
                    <li class="order-result-list-row" runat="server" id="trCoupon" visible="False">
                        <div class="order-result-name">
                            <%=Resource. Admin_ViewOrder_Coupon%>:
                        </div>
                        <div class="order-result-value">
                            <asp:Label ID="lblCoupon" runat="server"></asp:Label>
                        </div>
                    </li>
                    <li class="order-result-list-row">
                        <div class="order-result-name">
                            <%=Resource.Admin_ViewOrder_ShippingPrice %>:
                        </div>
                        <div class="order-result-value">
                            <asp:Label ID="lblShippingPrice" runat="server"></asp:Label>
                        </div>
                    </li>
                    <asp:ListView ID="lvTaxes" runat="server" ItemPlaceholderID="itemPlaceholderID">
                        <LayoutTemplate>
                            <li runat="server" id="itemPlaceholderID"></li>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <li class="order-result-list-row">
                                <div class="order-result-name">
                                    <%# (Convert.ToBoolean(Eval("TaxShowInPrice")) ? Resource.Core_TaxServices_Include_Tax : "") + " " + Eval("TaxName")%>:
                                </div>
                                <div class="order-result-value">
                                    <%#(Convert.ToBoolean(Eval("TaxShowInPrice")) ? "" : "+") + CatalogService.GetStringPrice(Convert.ToSingle(Eval("TaxSum")), CurrencyValue, CurrencyCode)%>
                                </div>
                            </li>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <li class="order-result-list-row">
                                <div class="order-result-name">
                                    <%= Resource.Admin_ViewOrder_Taxes %>
                                </div>
                                <div class="order-result-value">
                                    <%= CatalogService.GetStringPrice(0, CurrencyValue, CurrencyCode) %>
                                </div>
                            </li>
                        </EmptyDataTemplate>
                    </asp:ListView>
                    <li class="order-result-list-row" runat="server" id="liPaymentPrice">
                        <div class="order-result-name">
                            <%=Resource.Admin_ViewOrder_PaymentPrice %>:
                        </div>
                        <div class="order-result-value">
                            <asp:Label ID="lblPaymentPrice" runat="server"></asp:Label>
                        </div>
                    </li>
                    <li class="order-result-list-row">
                        <div class="order-result-name">
                            <%= Resource.Admin_ViewOrder_TotalPrice %>:
                        </div>
                        <div class="order-result-value">
                            <asp:Label ID="lblTotalPrice" runat="server"></asp:Label>
                        </div>
                    </li>

                </ul>
                <br class="clear">
                <div class="bonus-table" id="bonusCardBlock" runat="server" visible="False">
                    <div class="bonus-table-header"><%= Resource.Admin_ViewOrder_BonusCard %></div>
                    <ul class="order-result-list">
                        <li class="order-result-list-row">
                            <div class="order-result-name">
                                <%= Resource.Admin_ViewOrder_BonusCardNumber%>:
                            </div>
                            <div class="order-result-value">
                                <asp:Label ID="lblBonusCardNumber" runat="server" />
                            </div>
                        </li>
                        <li class="order-result-list-row">
                            <div class="order-result-name">
                                <%= Resource.Admin_ViewOrder_BonusCardAmount%>:
                            </div>
                            <div class="order-result-value">
                                <asp:Label ID="lblBonusCardAmount" runat="server"></asp:Label>
                            </div>
                        </li>
                    </ul>
                </div>
            </div>



            <ul class="justify order-comment-wrap">
                <li class="justify-item order-comment-item-wrap">
                    <div class="order-comment-title">
                        <asp:Label ID="lblStatusComment" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_StatusComment %>">
                        </asp:Label>
                    </div>
                    <div class="textarea-wrap order-comment-text">
                        <asp:TextBox ID="txtStatusComment" runat="server" TextMode="MultiLine" CssClass="editableTextBoxInViewOrder"
                            data-field-type="statusComment">
                        </asp:TextBox>
                    </div>
                </li>
                <li class="justify-item order-comment-item-wrap">
                    <div class="order-comment-title">
                        <asp:Label ID="lblAdminOrderComment" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_AdminOrderComment %>">
                        </asp:Label>
                    </div>
                    <div class="textarea-wrap order-comment-text">
                        <asp:TextBox ID="txtAdminOrderComment" runat="server" TextMode="MultiLine" CssClass="editableTextBoxInViewOrder"
                            data-field-type="adminOrderComment"></asp:TextBox>
                    </div>
                </li>
            </ul>
        </li>
    </ul>
</asp:Content>
