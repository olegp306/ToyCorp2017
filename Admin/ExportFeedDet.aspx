<%@ Page Title="ExportFeed" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="ExportFeedDet.aspx.cs" Inherits="Admin.ExportFeedDet" %>
<%@ Import Namespace="Resources" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item"><a href="Discount_PriceRange.aspx">
                <%= Resource.Admin_MasterPageAdmin_DiscountMethods%></a></li>
            <li class="neighbor-menu-item"><a href="Coupons.aspx">
                <%= Resource.Admin_MasterPageAdmin_Coupons%></a></li>
            <li class="neighbor-menu-item"><a href="Certificates.aspx">
                <%= Resource.Admin_MasterPageAdmin_Certificate%></a></li>
            <li class="neighbor-menu-item"><a href="SendMessage.aspx">
                <%= Resource.Admin_MasterPageAdmin_SendMessage%></a></li>
            <li class="neighbor-menu-item"><a href="Voting.aspx">
                <%= Resource.Admin_MasterPageAdmin_Voting%></a></li>
            <li class="neighbor-menu-item"><a href="SiteMapGenerate.aspx">
                <%= Resource.Admin_SiteMapGenerate_Header%></a></li>
            <li class="neighbor-menu-item"><a href="SiteMapGenerateXML.aspx">
                <%= Resource.Admin_SiteMapGenerateXML_Header%></a></li>
        </menu>
    </div>
    <div id="mainDiv" runat="server">
        <div style="margin-left: 20px; margin-right: 20px; padding-bottom:20px;">
            <div class="pageHeader">
                <span class="AdminHead">
                    <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_PageHeader %>' /></span>
                <span id="PageSubheader" visible="false" runat="Server">
                    <br />
                    <span class="AdminSubHead">
                        <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_PageSubHeader %>' />
                        <asp:Literal ID="ModuleNameLiteral" runat="Server" /></span>
                    <br />
                    <br />
                </span>
            </div>
            <br />
            <div class="ui-tabs">
                <ul class="ui-tabs-nav">
                    <li><a href="ExportFeed.aspx?moduleid=<%= ModuleName  %>">
                        <%=Resources.Resource.Admin_ExportFeed_ChooseProduct%></a></li>
                    <li class="ui-tabs-selected"><a href="javascript:void();">
                        <%=Resources.Resource.Admin_ExportFeed_ModuleSettings%></a></li>
                </ul>
                <div id="tabs-1" class="ui-tabs-panel">
                    <table border="0" cellpadding="2" cellspacing="0" style="width: 100%;">
                        <tr visible="false" id="datafeedNameRow" class="settingsRow" runat="Server">
                            <td class="parametrName">
                                <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedTitle %>' />
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:TextBox class="niceTextBox textBoxClass" ID="DatafeedTitleTextBox" runat="Server" />
                                </span>
                            </td>
                        </tr>
                        <tr visible="false" id="datafeedDescRow" class="settingsRowAlt" runat="Server">
                            <td class="parametrName">
                                <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedDescription %>' />
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:TextBox class="niceTextBox textBoxClass" ID="DatafeedDescriptionTextBox" runat="Server" />
                                </span>
                            </td>
                        </tr>
                        <tr visible="false" id="TrYandexCompany" class="settingsRow" runat="Server" style="height:60px;">
                            <td class="parametrName">
                                <asp:Literal ID="Literal1" runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_CompanyName %>' />
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:TextBox class="niceTextBox textBoxClass" ID="txtCompanyName" runat="Server" />
                                    <br />
                                    <span class="warning">
                                        <asp:Literal ID="Literal3" runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_ShopNameWarning %>' />
                                        <asp:Literal ID="companyName1" runat="Server" />.
                                    </span>
                                </span>
                            </td>
                        </tr>
                        <tr visible="false" id="TrYandexShop" class="settingsRowAlt" runat="Server" style="height:60px;">
                            <td class="parametrName">
                                <asp:Literal ID="Literal2" runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_ShopName %>' />
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:TextBox class="niceTextBox textBoxClass" ID="txtShopName" runat="Server" />
                                    <br />
                                    <span class="warning">
                                        <asp:Literal ID="Literal4" runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_ShopNameWarning %>' />
                                        <asp:Literal ID="companyName2" runat="Server" />.
                                    </span>
                                </span>
                            </td>
                        </tr>
                        <tr class="settingsRow ">
                            <td class="parametrName settingsRowWithDesc">
                                <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedFileName %>' /><br />
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:Literal ID="lShopUrl" runat="Server" /><asp:TextBox class="niceTextBox shortTextBoxClass" ID="FileNameTextBox" runat="Server" />.<asp:Literal
                                        ID="FileNameExtLiteral" runat="Server" />
                                </span>
                            </td>
                        </tr>
                        <tr class="settingsRowAlt">
                            <td class="parametrName settingsRowWithDesc">
                                <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedCurrencySelect %>' />
                                <span class="warning" id="currencyWarning" visible="false" runat="Server">
                                    <br />
                                    <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedCurrencyWarning %>' />
                                    <asp:Literal ID="MainCurrencyLiteral" runat="Server" />.
                                    <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedCurrencySelectWarning %>' />.
                                </span>
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:DropDownList Width="125px" ID="CurrencyListBox" runat="Server">
                                    </asp:DropDownList>
                                </span>
                            </td>
                        </tr>
                        <tr class="settingsRow">
                            <td class="parametrName">
                                <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedDescriptionSelect %>'></asp:Literal>
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:DropDownList Width="125px" ID="DescriptionSelectListBox" runat="Server">
                                        <asp:ListItem Value="short" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedDescriptionShort %>' />
                                        <asp:ListItem Value="full" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedDescriptionFull %>' />
                                    </asp:DropDownList>
                                </span>
                            </td>
                        </tr>
                        <tr class="settingsRowAlt">
                            <td class="parametrName">
                                <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedRecomendation %>' />:
                            </td>
                            <td class="parametrValue">
                                <div id="recomendationLiteral" class="parametrValueString" runat="Server">
                                    -
                                </div>
                            </td>
                        </tr>
                        <tr id="SalesNotes" class="settingsRow" visible="false" runat="Server">
                            <td class="parametrName">
                                <asp:Literal runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_FeedSalesNotes %>' />
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:TextBox class="niceTextBox textBoxClass" ID="SalesNotesTextBox" runat="Server" />
                                </span>
                            </td>
                        </tr>
                        <tr id="GoogleProductCategory" class="settingsRow" visible="false" runat="Server">
                            <td class="parametrName">
                                <asp:Literal ID="Literal6" runat="Server" Text='GoogleProductCategory' />
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:TextBox class="niceTextBox shortTextBoxClass" ID="GoogleProductCategoryTextBox" runat="Server" />
                                </span>
                            </td>
                        </tr>
                        <tr>
                            <td class="parametrName">
                                <asp:Literal ID="Literal7" runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_RemoveHTML%>' />
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:CheckBox runat="server" ID="chbRemoveHTML" CssClass="checkly-align"/>
                                </span>
                            </td>
                        </tr>
                        <tr id="Delivery" class="settingsRow" visible="false" runat="Server">
                            <td class="parametrName">
                                <%= Resources.Resource.Admin_ExportFeed_FeedDelivery%>
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:CheckBox runat="server" ID="chbDelivery" CssClass="checkly-align"/>
                                </span>
                            </td>
                        </tr>
                        <tr id="localDelivery" class="settingsRowAlt" visible="false" runat="Server">
                            <td class="parametrName">
                                <%= Resources.Resource.Admin_ExportFeed_LocalDelivery%>
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:CheckBox runat="server" ID="chbLocalDelivery" CssClass="checkly-align"/>
                                </span>
                            </td>
                        </tr>
                        <tr id="properties" visible="false" runat="Server" class="settingsRow">
                            <td class="parametrName">
                                <%= Resources.Resource.Admin_ExportFeed_FeedProperties%>
                            </td>
                            <td class="parametrValue">
                                <span class="parametrValueString">
                                    <asp:CheckBox runat="server" ID="cbProperties" CssClass="checkly-align"/>
                                </span>
                            </td>
                        </tr>
                        <tr class="settingsRowAlt controlClass">
                            <td colspan="2" style="text-align: center">
                                <div style="margin: 20px 0px 10px 0px;">
                                    <asp:Button runat="server" ID="btnSave" CssClass="btn btn-middle btn-add btn-admin-save"
                                        Text="<%$ Resources:Resource,Admin_ExportFeed_SaveSettings %>" OnClick="btnSave_Click" />
                                </div>
                                <div id="saveSuccess" style="color: Blue;" runat="server" visible="false">
                                    <asp:Literal ID="Literal8" runat="Server" Text='<%$ Resources:Resource, Admin_ExportFeed_SettingsSaved %>' />
                                </div>
                            </td>
                        </tr>
                    </table>
                    <div class="dvSubHelp" id="YaHelp" runat="server">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" />
                        <a href="http://www.advantshop.net/help/pages/export-yandex-market" target="_blank">Инструкция. Выгрузка товаров в Яндекс.Маркет</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="notInTariff" runat="server" visible="false" class="AdminSaasNotify">
        <center>
            <h2>
                <%=  Resource.Admin_DemoMode_NotAvailableFeature%>
            </h2>
        </center>
    </div>
</asp:Content>
