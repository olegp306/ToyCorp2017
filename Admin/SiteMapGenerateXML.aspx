<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="false"
    CodeFile="SiteMapGenerateXML.aspx.cs" Inherits="Admin.SiteMapGenerateXML" %>

<%@ Import Namespace="Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
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
            <li class="neighbor-menu-item"><a href="Statistics.aspx">
                <%= Resources.Resource.Admin_Statistics_Header %></a></li>
            <li class="neighbor-menu-item"><a href="SiteMapGenerate.aspx">
                <%= Resource.Admin_SiteMapGenerate_Header%></a></li>
            <li class="neighbor-menu-item selected"><a href="SiteMapGenerateXML.aspx">
                <%= Resource.Admin_SiteMapGenerateXML_Header%></a></li>
        </menu>
    </div>
    <div class="content-own">
        <center>
        <span class="AdminHead">
            <asp:Localize ID="Localize_Admin_SiteMapGenerateXML_Header" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerateXML_Header %>"></asp:Localize></span><br />
        <span class="AdminSubHead">
            <asp:Localize ID="Localize_Admin_SiteMapGenerateXML_SubHeader" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerateXML_SubHeader %>"></asp:Localize></span></center>
        <br />
        <br />
        <center>
        <table border="0" cellpadding="4" cellspacing="0" width="100%" class="catalog_link">
            <tr style="background-color:#eff0f1;">
                <td style="text-align:right; width:50%; font-weight:bold; height:26px;">
                    <asp:Localize ID="Localize_Admin_SiteMapGenerateXML_LastGenerationDate" runat="server"
                        Text="<%$ Resources:Resource, Admin_SiteMapGenerateXML_LastGenerationDate %>"></asp:Localize>
                </td>
                <td>
                    <asp:Label ID="lastMod" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align:right; font-weight:bold; height:26px;">
                    <asp:Localize ID="Localize_Admin_SiteMapGenerateXML_LinkToFile" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerateXML_LinkSiteMap %>"></asp:Localize>
                </td>
                <td>
                    <%= ShowStrLinkToSiteMapFile() %>
                    (<a href="<%= ShowStrLinkToSiteMapFile() %>" target="_blank"><asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerateXML_LinkSiteMapGo %>"></asp:Localize></a>)
                </td>
            </tr>
            <tr style="background-color: #eff0f1; font-weight:bold">
                <td style="text-align:right; width:50%; font-weight:bold; height:26px;">
                    <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerate_Schedule %>" ></asp:Localize>
                </td>
                <td>
                    <a href="CommonSettings.aspx#tabid=task"><asp:Localize ID="Localize3" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerate_Set %>"></asp:Localize></a> 
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:Button ID="btnCreateMap" runat="server" Text="<%$ Resources:Resource,Admin_SiteMapGenerateXML_Generate %>"
            OnClick="btnCreateMap_Click" />
        <br />
        <br />
        <asp:Label ID="lblErr" runat="server" Text="" ForeColor="Blue"></asp:Label>
    </center>
    </div>
</asp:Content>
