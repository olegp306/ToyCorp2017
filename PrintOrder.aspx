<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintOrder.aspx.cs" Inherits="ClientPages.PrintOrder" %>

<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="Admin/css/AdminStyle.css" rel="stylesheet" type="text/css" />
    <title></title>
    <%if (MapType == "yandexmap")
      { %>
    <script src="http://api-maps.yandex.ru/2.0-stable/?load=package.standard&lang=ru-RU" type="text/javascript"></script>
    <%} %>

    <style type="text/css">
        body {
            font-family: Arial;
            font-size: 13px;
        }

        .center {
            text-align: center;
        }

        .print-contact {
            padding: 3px 0 0 20px;
        }

        .print-o-item {
            padding: 3px 0 0 0;
        }

        .print-o-title {
            font-weight: bold;
            display: inline-block;
        }

        .print-o-value {
            display: inline-block;
        }

        .pr-shipm {
            padding: 3px 0 10px 20px;
            font-weight: bold;
        }

        .grid-top {
            font-weight: bold;
            padding: 0 10px;
        }

        #printorder-gmap {
            padding: 10px 0 0 0;
        }

        #printorder-yamap {
            width: 450px;
            height: 350px;
            padding: 10px 0 0 0;
        }

        .print-sum-b {
            margin: 8px 0 0 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding: 15px;">
            <div class="center">
                <asp:Label ID="lblOrderID" CssClass="AdminHead" runat="server" />
                <br />
                <% if (ShowStatusInfo)
                   { %>
                (<%= Order.OrderStatus.StatusName%>)<br />
                <%} %>
                <br />
            </div>
            <b><%=Resource.Admin_ViewOrder_Date%></b>
            <asp:Label ID="lOrderDate" runat="server" />
            <br />
            <% if (ShowStatusInfo)
               { %>
            <div>
                <div class="print-o-title"><%=Resource.Admin_ViewOrder_Number%></div>
                <div class="print-o-value"><%= Order.Number%></div>
            </div>
            <% if (Order.StatusComment.IsNotEmpty())
               { %>
            <div>
                <div class="print-o-title"><%= Resource.Admin_ViewOrder_StatusComment %></div>
                <div class="print-o-value"><%= Order.StatusComment %></div>
            </div>
            <% }
               } %>
            <div>
                <div class="print-o-title"><%=Resource.Admin_ViewOrder_Email%></div>
                <div class="print-o-value"><%= Order.OrderCustomer.Email%></div>
            </div>
            <div>
                <div class="print-o-title"><%=Resource.Admin_ViewOrder_Telephone%></div>
                <div class="print-o-value"><%= Order.OrderCustomer.MobilePhone%></div>
            </div>
            <br />
            <table border="0" width="100%" cellspacing="0" cellpadding="0">
                <tr>
                    <td style="width: 34%; vertical-align: top" runat="server" id="trBilling">
                        <%= Resource.Admin_ViewOrder_Billing %>
                        <div class="print-contact">
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_Name%></div>
                                <div class="print-o-value"><%= Order.BillingContact.Name%></div>
                            </div>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_Country%></div>
                                <div class="print-o-value"><%= Order.BillingContact.Country%></div>
                            </div>
                            <% if (Order.BillingContact.Zone.IsNotEmpty())
                               { %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_Zone%></div>
                                <div class="print-o-value"><%= Order.BillingContact.Zone %></div>
                            </div>
                            <% } %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_City%></div>
                                <div class="print-o-value"><%= Order.BillingContact.City%></div>
                            </div>
                            <% if (Order.BillingContact.Zip.IsNotEmpty())
                               { %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_Zip%></div>
                                <div class="print-o-value"><%= Order.BillingContact.Zip%></div>
                            </div>
                            <% } %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_Address%></div>
                                <div class="print-o-value"><%= Order.BillingContact.Address%></div>
                            </div>

                            <% if (Order.BillingContact.CustomField1.IsNotEmpty())
                               { %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= SettingsOrderConfirmation.CustomShippingField1%></div>
                                <div class="print-o-value"><%= Order.BillingContact.CustomField1%></div>
                            </div>
                            <% } %>
                            <% if (Order.BillingContact.CustomField2.IsNotEmpty())
                               { %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= SettingsOrderConfirmation.CustomShippingField2%></div>
                                <div class="print-o-value"><%= Order.BillingContact.CustomField2%></div>
                            </div>
                            <% } %>

                            <% if (Order.BillingContact.CustomField3.IsNotEmpty())
                               { %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= SettingsOrderConfirmation.CustomShippingField3%></div>
                                <div class="print-o-value"><%= Order.BillingContact.CustomField3%></div>
                            </div>
                            <% } %>

                        </div>
                    </td>
                    <td style="width: 33%; vertical-align: top" runat="server" id="trShipping">
                        <%= Resource. Admin_ViewOrder_Shipping %>
                        <div class="print-contact">
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_Name%></div>
                                <div class="print-o-value"><%= Order.BillingContact.Name%></div>
                            </div>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_Country%></div>
                                <div class="print-o-value"><%= Order.ShippingContact.Country%></div>
                            </div>
                            <% if (Order.ShippingContact.Zone.IsNotEmpty())
                               { %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_Zone%></div>
                                <div class="print-o-value"><%= Order.ShippingContact.Zone%></div>
                            </div>
                            <% } %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_City%></div>
                                <div class="print-o-value"><%= Order.ShippingContact.City%></div>
                            </div>
                            <% if (Order.ShippingContact.Zip.IsNotEmpty())
                               { %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_Zip%></div>
                                <div class="print-o-value"><%= Order.ShippingContact.Zip%></div>
                            </div>
                            <% } %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= Resource.Admin_ViewOrder_Address%></div>
                                <div class="print-o-value"><%= Order.ShippingContact.Address%></div>
                            </div>
                            
                             <% if (Order.ShippingContact.CustomField1.IsNotEmpty())
                               { %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= SettingsOrderConfirmation.CustomShippingField1%>:</div>
                                <div class="print-o-value"><%= Order.ShippingContact.CustomField1%></div>
                            </div>
                            <% } %>
                            <% if (Order.ShippingContact.CustomField2.IsNotEmpty())
                               { %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= SettingsOrderConfirmation.CustomShippingField2%>:</div>
                                <div class="print-o-value"><%= Order.ShippingContact.CustomField2%></div>
                            </div>
                            <% } %>

                            <% if (Order.ShippingContact.CustomField3.IsNotEmpty())
                               { %>
                            <div class="print-o-item">
                                <div class="print-o-title"><%= SettingsOrderConfirmation.CustomShippingField3%>:</div>
                                <div class="print-o-value"><%= Order.ShippingContact.CustomField3%></div>
                            </div>
                            <% } %>
                        </div>
                    </td>
                    <td style="width: 32%; vertical-align: top">
                        <%= Resource.Admin_ViewOrder_ShippingMethod %>
                        <div class="pr-shipm">
                            <%= Order.ArchivedShippingName + (Order.OrderPickPoint != null ? "<br/>" + Order.OrderPickPoint.PickPointAddress : string.Empty)%>
                        </div>
                        <%= Resource.Admin_ViewOrder_PaymentType %>
                        <div class="pr-shipm">
                            <%= Order.PaymentMethodName %>
                        </div>
                    </td>
                </tr>
            </table>
            <% if (Request["order"] == "details" && ShowMap)
               {
                   if (MapType == "googlemap")
                   { %>
            <%= string.Format("<div id=\"printorder-gmap\"><img width=\"500\" height=\"300\" src=\"https://maps.googleapis.com/maps/api/staticmap?center={0}&zoom=16&size=500x300&sensor=false\"></img></div>",
                                    HttpUtility.UrlEncode(MapAdress))%>
            <% }
                   else
                   { %>
            <script type="text/javascript">
                var myMap;
                ymaps.ready(function () {
                    var coordinates;
                    var myGeocoder = ymaps.geocode("<%= MapAdress %>");
                    myGeocoder.then(
                        function (res) {
                            try {
                                coordinates = res.geoObjects.get(0).geometry.getCoordinates();
                                myMap = new ymaps.Map("printorder-yamap", {
                                    center: coordinates,
                                    zoom: 15,
                                    behaviors: ["default", "scrollZoom"]
                                });
                                var myPlacemark = new ymaps.Placemark(coordinates);
                                myMap.geoObjects.add(myPlacemark);
                                myMap.controls.add("mapTools").add("zoomControl").add("typeSelector").add("trafficControl");
                            } catch (e) {
                            } finally {
                                setTimeout(function () { window.print(); }, 1500);
                            }
                        }
                    );
                });
            </script>
            <div id="printorder-yamap"></div>
            <%  }
               } %>
            <br />
            <%= Resource.Admin_ViewOrder_OrderItem %>
            <br />
            <br />
            <asp:ListView ID="lvOrderItems" runat="server" ItemPlaceholderID="itemPlaceholderID">
                <LayoutTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="grid-main">
                        <thead>
                            <tr class="GridView_HeaderStyle">
                                <td class="grid-top">
                                    <asp:Literal runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemName %>" />
                                </td>
                                <td class="grid-top" style="width: 90px; text-align: center;">
                                    <asp:Literal runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Price %>" />
                                </td>
                                <td class="grid-top" style="width: 55px; text-align: center;">
                                    <asp:Literal runat="server" Text="<%$ Resources:Resource, Client_Bill2_Count %>" />
                                </td>
                                <td class="grid-top" style="width: 90px; text-align: center;">
                                    <asp:Literal runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemCost %>" />
                                </td>
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
                        <td class="grid-left">
                            <%# Eval("ArtNo") + ", " + Eval("Name")%>
                            <div class="order-item-options">
                                <%# Eval("Color") != null ? "<div><span>" + SettingsCatalog.ColorsHeader + "</span>: " + Eval("Color") + "</div>" : ""%>
                                <%# Eval("Size") != null ? "<div><span>" + SettingsCatalog.SizesHeader + "</span>: " + Eval("Size") + "<div />" : ""%>
                                <%#RenderSelectedOptions((IList<EvaluatedCustomOptions>)Eval("SelectedOptions"))%>
                            </div>
                        </td>
                        <td class="grid-even" style="text-align: center;">
                            <%#CatalogService.GetStringPrice((float)Eval("Price"), 1, OrdCurrency.CurrencyCode, OrdCurrency.CurrencyValue)%>
                        </td>
                        <td class="grid-even" style="text-align: center;">
                            <%#Eval("Amount")%>
                        </td>
                        <td class="grid-even" style="text-align: center;">
                            <%#CatalogService.GetStringPrice((float)Eval("Price"), (float)Eval("Amount"), OrdCurrency.CurrencyCode, OrdCurrency.CurrencyValue)%>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
            <asp:ListView ID="lvOrderGiftCertificates" runat="server" ItemPlaceholderID="itemPlaceholderID">
                <LayoutTemplate>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="grid-main">
                        <thead>
                            <tr class="GridView_HeaderStyle">
                                <th class="grid-top"></th>
                                <th class="grid-top">
                                    <asp:Literal runat="server" Text="<%$ Resources:Resource,Client_PrintOrder_GiftCertificateCode %>" />
                                </th>
                                <th class="grid-top" style="text-align: center;">
                                    <asp:Literal runat="server" Text="<%$ Resources:Resource,Client_PrintOrder_GiftCertificateSum %>" />
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
                        <td class="grid-left">
                            <asp:Label runat="server" Text="<%$ Resources:Resource,Client_PrintOrder_GiftCertificate %>" />
                        </td>
                        <td class="grid-left">
                            <%# Eval("CertificateCode") %>
                        </td>
                        <td class="grid-even">
                            <%#CatalogService.GetStringPrice((float)Eval("Sum"), 1, OrdCurrency.CurrencyCode, OrdCurrency.CurrencyValue)%>
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr>
                        <td class="grid-left_alt">
                            <asp:Label ID="Label6" runat="server" Text="<%$ Resources:Resource,Client_PrintOrder_GiftCertificate %>" />
                        </td>
                        <td class="grid-left_alt">
                            <%# Eval("CertificateCode") %>
                        </td>
                        <td class="grid-left_alt">
                            <%#CatalogService.GetStringPrice((float)Eval("Sum"), 1, OrdCurrency.CurrencyCode, OrdCurrency.CurrencyValue)%>
                        </td>
                    </tr>
                </AlternatingItemTemplate>
            </asp:ListView>
            <asp:Panel ID="pnlSummary" runat="server" CssClass="print-sum-b">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td style="text-align: right">
                            <asp:Label ID="Label8" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemCost2 %>" />:&nbsp;
                        </td>
                        <td style="width: 150px">
                            <asp:Label ID="lblProductPrice" runat="server" />
                        </td>
                    </tr>
                    <tr id="trDiscount" runat="server" visible="false">
                        <td style="text-align: right">
                            <asp:Label ID="Label9" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemDiscount %>" />:&nbsp;
                        </td>
                        <td style="width: 150px">
                            <asp:Label ID="lblOrderDiscount" runat="server" />
                        </td>
                    </tr>
                    <tr id="trBonus" runat="server" visible="false">
                        <td style="text-align: right">
                            <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Bonuses %>" />:&nbsp;
                        </td>
                        <td style="width: 150px">
                            <asp:Label ID="lblOrderBonus" runat="server" />
                        </td>
                    </tr>
                    <tr id="trCertificate" runat="server" visible="false">
                        <td style="text-align: right">
                            <asp:Label ID="Label12" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Certificate %>" />:&nbsp;
                        </td>
                        <td style="width: 150px">
                            <asp:Label ID="lblCertificate" runat="server" />
                        </td>
                    </tr>
                    <tr id="trCoupon" runat="server" visible="false">
                        <td style="text-align: right">
                            <asp:Label ID="Label16" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Coupon%>" />:&nbsp;
                        </td>
                        <td style="width: 150px">
                            <asp:Label ID="lblCoupon" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            <asp:Label ID="Label10" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ShippingPrice %>" />:&nbsp;
                        </td>
                        <td style="width: 150px">
                            <asp:Label ID="lblShippingPrice" runat="server" />
                        </td>
                    </tr>
                    <asp:Literal ID="literalTaxCost" runat="server"></asp:Literal>
                    <tr id="PaymentRow" runat="server">
                        <td style="text-align: right">
                            <asp:Label ID="Label6" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_PaymentExtracharge %>" />:&nbsp;
                        </td>
                        <td style="width: 150px">
                            <asp:Label ID="lblPaymentPrice" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right">
                            <b>
                                <asp:Label ID="Label11" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_TotalPrice %>" />:&nbsp;</b>
                        </td>
                        <td style="width: 150px">
                            <b>
                                <asp:Label ID="lblTotalPrice" runat="server" /></b>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <% if (Order.CustomerComment.IsNotEmpty())
               { %>
            <%= Resource.Client_PrintOrder_YourComment %>
            <div><%=Order.CustomerComment%> </div>
            <% } %>
        </div>
        <script type="text/javascript">
            function position_this_window() {
                var x = (screen.availWidth - 770) / 2;
                window.resizeTo(762, 662);
                window.moveTo(Math.floor(x), 50);
            }

            window.addEventListener('DOMContentLoaded', loaded, false);

            function loaded() {
                position_this_window();

                if (document.getElementById("printorder-map") == null) {
                    setTimeout(function () { window.print(); }, 1500);
                }
            };
        </script>
    </form>
</body>
</html>
