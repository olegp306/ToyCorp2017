<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true"
    CodeFile="Statistics.aspx.cs" Inherits="Admin.Statistics" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
    <div id="inprogress" style="display: none;">
        <div id="curtain" class="opacitybackground">
            &nbsp;
        </div>
        <div class="loader">
            <table width="100%" style="font-weight: bold; text-align: center;">
                <tbody>
                    <tr>
                        <td style="text-align: center;">
                            <img src="images/ajax-loader.gif" alt="" />
                        </td>
                    </tr>
                    <tr>
                        <td style="color: #0D76B8; text-align: center;">
                            <asp:Localize ID="Localize_Admin_Properties_PleaseWait" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_PleaseWait %>"></asp:Localize>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item"><a href="Discount_PriceRange.aspx">
                <%= Resource.Admin_MasterPageAdmin_DiscountMethods%></a></li>
            <li class="neighbor-menu-item selected"><a href="Coupons.aspx">
                <%= Resource.Admin_MasterPageAdmin_Coupons%></a></li>
            <li class="neighbor-menu-item"><a href="Certificates.aspx">
                <%= Resource.Admin_MasterPageAdmin_Certificate%></a></li>
            <li class="neighbor-menu-item"><a href="SendMessage.aspx">
                <%= Resource.Admin_MasterPageAdmin_SendMessage%></a></li>
            <li class="neighbor-menu-item"><a href="ExportFeed.aspx?ModuleId=YandexMarket">
                <%= Resources.Resource.Admin_MasterPageAdmin_YandexMarket %></a></li>
            <li class="neighbor-menu-item"><a href="ExportFeed.aspx?ModuleId=GoogleBase">
                <%= Resources.Resource.Admin_MasterPageAdmin_GoogleBase %></a></li>
            <li class="neighbor-menu-item dropdown-menu-parent"><a href="Voting.aspx">
                <%= Resource.Admin_MasterPageAdmin_Voting%></a>
                <div class="dropdown-menu-wrap">
                    <ul class="dropdown-menu">
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Voting.aspx"><%= Resources.Resource.Admin_MasterPageAdmin_Voting %>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="VotingHistory.aspx"><%= Resources.Resource.Admin_MasterPageAdmin_VotingHistory %>
                        </a></li>
                    </ul>
                </div>
            </li>
            <li class="neighbor-menu-item selected"><a href="Statistics.aspx">
                <%= Resources.Resource.Admin_Statistics_Header %></a></li>
            <li class="neighbor-menu-item"><a href="SiteMapGenerate.aspx">
                <%= Resource.Admin_SiteMapGenerate_Header%></a></li>
            <li class="neighbor-menu-item"><a href="SiteMapGenerateXML.aspx">
                <%= Resource.Admin_SiteMapGenerateXML_Header%></a></li>
        </menu>
    </div>
    <div class="content-own">
        <div>
            <table cellpadding="0" cellspacing="0" width="100%">
                <tbody>
                    <tr>
                        <td style="width: 72px;">
                            <img src="images/orders_ico.gif" alt="" />
                        </td>
                        <td>
                            <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_Statistics_Header %>"></asp:Label><br />
                            <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_Statistics_SubHeader %>"></asp:Label>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div>
                
            </div>
            <div style="margin: 10px 0">
                
                <div style="padding: 15px 0;">
                    
                    <table cellpadding="3px">
                        <tr>
                            <td>
                                <%= Resource.Admin_Statistics_DateFrom %>
                            </td>
                            <td></td>
                            <td>
                                <%= Resource.Admin_Statistics_DateTo %>
                            </td>
                            <td>
                                <%= Resource.Admin_Statistics_Paid %>
                            </td>
                            <td>
                                 <%= Resource.Admin_Statistics_Status %>
                            </td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>
                                <div class="dp"><asp:TextBox runat="server" ID="txtDateFrom" /></div>                                
                            </td>
                            <td>-</td>
                            <td>
                                <div class="dp"><asp:TextBox runat="server" ID="txtDateTo" /></div>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlPayed">
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Any %>" Value="any" Selected="True" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_Yes %>" Value="yes" />
                                    <asp:ListItem Text="<%$ Resources:Resource, Admin_Catalog_No %>" Value="no" />
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlStatuses" DataTextField="StatusName" DataValueField="StatusID" />
                            </td>
                            <td>
                                <asp:Button runat="server" ID="btnFilter" Text="<%$ Resources:Resource, Admin_Statistics_Apply %>" OnClick="btnFilter_Click" CssClass="filter-btn" />
                            </td>
                        </tr>
                    </table>
                </div>
                
                <div style="width: 600px; float: left">
                    <div class="clearfix">
                        <h2 class="chart-orders-title"><%= Resource.Admin_Default_Orders %></h2>
                        <div class="chart-orders-period">
                            <div data-plugin="radiolist" class="radiolist">
                                <label>
                                    <input type="radio" id="groupbyDay" runat="server" value="day" />
                                    <%= Resource.Admin_Statistics_Day %>
                                </label>
                                <label>
                                    <input type="radio" id="groupbyWeek" runat="server" value="week" />
                                    <%= Resource.Admin_Statistics_Week %>
                                </label>
                                <label>
                                    <input type="radio" id="groupbyMounth" runat="server" value="mounth" />
                                    <%= Resource.Admin_Statistics_Month %>
                                </label>
                            </div>
                        </div>
                    </div>
                    <article class="chart-block">
                        <div id="orderGraph" runat="server" style="width: 100%; height: 200px;" data-plugin="chart"></div>
                    </article>
                    
                    <div class="clearfix">
                        <h2 class="chart-orders-title"><%= Resource.Admin_Statistics_OrdersByCount %></h2>
                   </div>
                    <article class="chart-block">
                        <div id="orderCountGraph" runat="server" style="width: 100%; height: 200px;" data-plugin="chart"></div>
                    </article>
                    
                    <div class="clearfix">
                        <h2 class="chart-orders-title"><%= Resource.Admin_Statistics_OrdersByCustomersType %></h2>
                   </div>
                    <article class="chart-block">
                        <div id="orderRegGraph" runat="server" style="width: 100%; height: 200px;" data-plugin="chart"></div>
                    </article>
                </div>
                
                <div style="float: left;margin: 0 0 0 20px;">
                    <div style="position: relative">
                        <div class="clearfix">
                            <h2 class="chart-orders-title"><%= Resource.Admin_Statistics_Payments %></h2>
                        </div>
                        <div id="paymentsPie" runat="server" style="width: 530px;height: 230px" data-plugin="chart"></div>
                    </div>
                    
                    <div>
                        <div class="clearfix">
                            <h2 class="chart-orders-title"><%= Resource.Admin_Statistics_Shippings %></h2>
                        </div>
                        <div id="shippingsPie" runat="server" style="width: 530px;height: 230px" data-plugin="chart"></div>
                    </div>
                    
                    <div>
                        <div class="clearfix">
                            <h2 class="chart-orders-title"><%= Resource.Admin_Statistics_OrdersByCities %></h2>
                        </div>
                        <div id="orderCitiesPie" runat="server" style="width: 530px;height: 230px" data-plugin="chart"></div>
                    </div>
                </div>
                
                <div class="clear"></div>
                
                <div style="margin: 15px 0">
                    
                    <div class="stat-b-item" style="width: 330px;">
                        <div class="clearfix">
                            <h2 class="chart-orders-title"><%= Resource.Admin_Statistics_TopCustomersBySum %></h2>
                        </div>
                        <asp:ListView runat="server" ID="lvCustomers">
                            <LayoutTemplate>
                                <table class="table-ui">
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, Admin_Statistics_CustomerName%>" />
                                        </td>
                                        <td>
                                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, Admin_Statistics_SummPrice%>" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="itemPlaceholder"></tr>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><b><%# Container.DataItemIndex + 1%></b></td>
                                    <td><%# !string.IsNullOrEmpty((string)Eval("Email")) ? Eval("Email") : Eval("fio") %></td>
                                    <td style="width: 90px"><%# Convert.ToInt64(Eval("Summary")).ToString("F2") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                    
                    <div class="stat-b-item">
                        <div class="clearfix">
                            <h2 class="chart-orders-title"><%= Resource.Admin_Statistics_TopProductsByAmount %></h2>
                        </div>
                        <asp:ListView runat="server" ID="lvProducts">
                            <LayoutTemplate>
                                <table class="table-ui">
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, Admin_Statistics_Name%>" />
                                        </td>
                                        <td style="width: 50px">
                                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, Admin_Statistics_ProductsAmount%>" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="itemPlaceholder"></tr>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><b><%# Container.DataItemIndex + 1%></b></td>
                                    <td>
                                        <%# RenderLink(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetString(Eval("ArtNo"))) %>
                                    </td>
                                    <td><%# Convert.ToInt32(Eval("Summary")) %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>
                    
                    <div class="stat-b-item">
                        <div class="clearfix">
                            <h2 class="chart-orders-title"><%= Resource.Admin_Statistics_TopProductsByAmount %></h2>
                        </div>
                        <asp:ListView runat="server" ID="lvProductsBySum">
                            <LayoutTemplate>
                                <table class="table-ui">
                                    <tr>
                                        <td></td>
                                        <td>
                                            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: Resource, Admin_Statistics_Name%>" />
                                        </td>
                                        <td>
                                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: Resource, Admin_Statistics_SummPrice%>" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="itemPlaceholder"></tr>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><b><%# Container.DataItemIndex + 1%></b></td>
                                    <td>
                                        <%# RenderLink(SQLDataHelper.GetInt(Eval("ProductId")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetString(Eval("ArtNo"))) %>
                                    </td>
                                    <td><%# Convert.ToInt64(Eval("Summary")).ToString("F2") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </div>

                </div>

            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".radiolist input").on("change", function () {
                $(".filter-btn").click();
            });
        });
    </script>
</asp:Content>
