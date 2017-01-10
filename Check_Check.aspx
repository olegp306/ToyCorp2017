<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Check_Check.aspx.cs" Inherits="ClientPages.Check_Check" %>
<%@ Import Namespace="AdvantShop.Helpers" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=Resources.Resource.Client_Check_Title%></title>
    <style type="text/css">
        body
        {
            font-family: Verdana, Arial, Helvetica, Sans-serif;
            font-size: 11px;
            margin: 10px;
            padding: 10px;
        }
    </style>
    <script type="text/javascript">
        function position_this_window() {
            window.resizeTo(670, 700);
            var x = (screen.availWidth - 670) / 2;
            window.moveTo(Math.floor(x), 50);
        }
    </script>
</head>
<body onload="position_this_window();window.print();">
    <form id="form1" runat="server">
    <br />
    <table cellspacing="0" cellpadding="0" style="width: 600px; background-color: #ffffff;"
        summary="Order details">
        <tr>
            <td>
                <table cellspacing="0" cellpadding="0" style="width: 100%; border: none;" summary="Summary">
                    <tr>
                        <td style="width: 100%;">
                            <table cellspacing="0" cellpadding="2" width="100%" summary="Details">
                                <tr>
                                    <td valign="top">
                                        <strong style="font-size: 28px; text-transform: uppercase;">Invoice</strong>
                                        <br />
                                        <br />
                                        <strong>Date:</strong>
                                        <asp:Label ID="lOrderDate" runat="server"></asp:Label><br />
                                        <strong>Order id:</strong>
                                        <asp:Label ID="lOrderId" runat="server"></asp:Label><br />
                                        <strong>Order status:</strong> Queued<br />
                                        <strong>Payment method:</strong><br />
                                        Check (manual processing)<br />
                                        <strong>Delivery method:</strong><br />
                                        <asp:Label ID="lShippingMethod" runat="server"></asp:Label>
                                    </td>
                                    <td valign="bottom" align="right">
                                        <strong>
                                            <asp:Label ID="lCompanyName" runat="server"></asp:Label></strong><br />
                                        <asp:Label ID="lAddress" runat="server"></asp:Label>,
                                        <asp:Label ID="lCity" runat="server"></asp:Label><br />
                                        <asp:Label ID="lState" runat="server"></asp:Label>,
                                        <asp:Label ID="lCountry" runat="server" Text="Label"></asp:Label><br />
                                        CALL US:
                                        <asp:Label ID="lCompanyPhone" runat="server"></asp:Label><br />
                                        International:
                                        <asp:Label ID="lInterPhone" runat="server"></asp:Label><br />
                                        Fax:
                                        <asp:Label ID="lCompanyFax" runat="server"></asp:Label><br />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <hr style="border: 0px none; border-bottom: 2px solid #58595b; margin: 2px 0px 17px 0px;
                    padding: 0px; height: 0px;" />
                <table cellspacing="0" cellpadding="0" style="width: 45%; border: 0px none; margin-bottom: 15px;"
                    summary="Address">
                    <tr>
                        <td nowrap="nowrap">
                            <strong>Name:</strong>
                        </td>
                        <td>
                            <asp:Label ID="lName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong>Phone:</strong>
                        </td>
                        <td>
                            <asp:Label ID="lPhone" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <strong>E-mail:</strong>
                        </td>
                        <td>
                            <asp:Label ID="lEmail" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table cellspacing="0" cellpadding="0" style="width: 100%; border: 0px none; margin-bottom: 30px;"
                    summary="Addresses">
                    <tr>
                        <td style="width: 45%; height: 25px;">
                            <strong>Billing Address</strong>
                        </td>
                        <td width="10%">
                            &nbsp;
                        </td>
                        <td style="width: 45%; height: 25px;">
                            <strong>Shipping Address</strong>
                        </td>
                    </tr>
                    <tr>
                        <td height="4">
                            <img src="images/spacer.gif" style="height: 2px; width: 100%; background: #58595b none;"
                                alt="" />
                        </td>
                        <td>
                            <img height="2" src="images/spacer.gif" width="1" alt="" />
                        </td>
                        <td height="4">
                            <img src="images/spacer.gif" style="height: 2px; width: 100%; background: #58595b none;"
                                alt="" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table cellspacing="0" cellpadding="0" style="width: 100%; border: none;" summary="Billing Address">
                                <tr>
                                    <td>
                                        <strong>Address:</strong>
                                    </td>
                                    <td>
                                        <asp:Label ID="lBillingAddress" runat="server"></asp:Label><br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>City:</strong>
                                    </td>
                                    <td>
                                        <asp:Label ID="lBillingCity" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>State:</strong>
                                    </td>
                                    <td>
                                        <asp:Label ID="lBillingState" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Country:</strong>
                                    </td>
                                    <td>
                                        <asp:Label ID="lBillingCountry" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Zip/Postal code:</strong>
                                    </td>
                                    <td>
                                        <asp:Label ID="lBillingZip" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <table cellspacing="0" cellpadding="0" width="100%" border="0" summary="Shipping Address">
                                <tr>
                                    <td>
                                        <strong>Address:</strong>
                                    </td>
                                    <td>
                                        <asp:Label ID="lShippingAddress" runat="server"></asp:Label><br />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>City:</strong>
                                    </td>
                                    <td>
                                        <asp:Label ID="lShippingCity" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>State:</strong>
                                    </td>
                                    <td>
                                        <asp:Label ID="lShippingState" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Country:</strong>
                                    </td>
                                    <td>
                                        <asp:Label ID="lShippingCountry" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong>Zip/Postal code:</strong>
                                    </td>
                                    <td>
                                        <asp:Label ID="lShippingZip" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <p style="font-size: 14px; font-weight: bold; text-align: center">
                    Products ordered</p>
                <table cellspacing="0" cellpadding="3" width="100%" border="1" summary="Products">
                    <tr>
                        <th width="60" bgcolor="#cccccc">
                            #
                        </th>
                        <th bgcolor="#cccccc">
                            Product
                        </th>
                        <th nowrap="nowrap" width="100" bgcolor="#cccccc" align="center">
                            Item price
                        </th>
                        <th width="60" bgcolor="#cccccc">
                            Quantity
                        </th>
                        <th width="60" bgcolor="#cccccc">
                            Total
                        </th>
                    </tr>
                    <asp:Repeater runat="server" ID="rptOrderItems" DataSource="<%# Order.OrderItems %>">
                    <ItemTemplate>
                    <tr>
                    <td align="center"><asp:Literal runat="server" Text='<%# Container.ItemIndex %>'></asp:Literal></td>
                    <td><b><NOBR><asp:Literal runat="server" Text='<%# Eval("Name") %>'></asp:Literal></NOBR></b></td>
                    <td align="right" nowrap="nowrap"><span class="currency"><asp:Literal ID="Literal1" runat="server" Text='<%# EvalPrice((float) Eval("Price")) %>'></asp:Literal> </span>&nbsp;&nbsp;</td>
                    <td align="center"><asp:Literal ID="Literal2" runat="server" Text='<%# Eval("Amount") %>'></asp:Literal></td>
                    <td align="right" nowrap="nowrap" style="padding-right: 5px;"><span class="currency"><asp:Literal ID="Literal3" runat="server" Text='<%# EvalPrice((SQLDataHelper.GetFloat(Eval("Price")) * SQLDataHelper.GetFloat(Eval("Amount"))))  %>'></asp:Literal> </span></td>
                    </tr>
                    </ItemTemplate>
                    </asp:Repeater>
                    
                </table>

                <table cellspacing="0" cellpadding="0" width="100%" border="0" summary="Total">
                    <tr>
                        <td style="padding-right: 3px; height: 20px; width: 100%; text-align: right;">
                            <strong>Subtotal:</strong>
                        </td>
                        <td style="white-space: nowrap; padding-right: 5px; height: 20px; text-align: right;">
                            <span class="currency">
                                <asp:Label ID="lSubTotal" runat="server"></asp:Label></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-right: 3px; height: 20px; width: 100%; text-align: right;">
                            <strong>Shipping cost:</strong>
                        </td>
                        <td style="white-space: nowrap; padding-right: 5px; height: 20px; text-align: right;">
                            <span class="currency">
                                <asp:Label ID="lShippingCost" runat="server"></asp:Label></span>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-right: 3px; height: 20px; width: 100%; text-align: right;">
                            <strong>Discount:</strong>
                        </td>
                        <td style="white-space: nowrap; padding-right: 5px; height: 20px; text-align: right;">
                            <span class="currency">
                                <asp:Label ID="lDiscount" runat="server"></asp:Label></span>
                        </td>
                    </tr>
                    <asp:Literal ID="literalTaxCost" runat="server"></asp:Literal>
                    <tr>
                        <td height="4" colspan="2">
                            <img src="images/spacer.gif" style="height: 2px; width: 100%; background: #58595b none;"
                                alt="" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-right: 3px; height: 25px; background: #cccccc none; width: 100%;
                            text-align: right;">
                            <strong>Total:</strong>
                        </td>
                        <td style="white-space: nowrap; padding-right: 5px; height: 25px; background: #cccccc none;
                            text-align: right;">
                            <strong><span class="currency">
                                <asp:Label ID="lTotal" runat="server"></asp:Label></span></strong>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td style="text-align: center; padding-top: 30px; font-size: 12px;">
                Thank you for your purchase!
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
