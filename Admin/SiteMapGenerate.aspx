<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="false"
    CodeFile="SiteMapGenerate.aspx.cs" Inherits="Admin.SiteMapGenerate" %>

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
            <li class="neighbor-menu-item selected"><a href="SiteMapGenerate.aspx">
                <%= Resource.Admin_SiteMapGenerate_Header%></a></li>
            <li class="neighbor-menu-item"><a href="SiteMapGenerateXML.aspx">
                <%= Resource.Admin_SiteMapGenerateXML_Header%></a></li>
        </menu>
    </div>
    <div class="content-own">
        <center>
        <span class="AdminHead">
            <asp:Localize ID="Localize_Admin_SiteMapGenerate_Header" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerate_Header %>"></asp:Localize></span><br />
        <span class="AdminSubHead">
            <asp:Localize ID="Localize_Admin_SiteMapGenerate_SubHead" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerate_SubHead %>"></asp:Localize></span></center>
        <br />
        <br />
        <center>
        <table border="0" cellpadding="4" cellspacing="0" width="100%" class="catalog_link">
            <tr style="background-color: #eff0f1;" >
                <td style="text-align: right; width:50%; font-weight:bold;height:26px;">
                    <asp:Localize ID="Localize_Admin_SiteMapGenerate_ModDateSiteMap" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerate_ModDateSiteMap %>"></asp:Localize>
                </td>
                <td>
                    <asp:Label ID="lastMod" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right; width:50%; font-weight:bold;height:26px;">
                    <asp:Localize ID="Localize_Admin_SiteMapGenerate_LinkSiteMap" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerate_LinkSiteMap %>"></asp:Localize>
                </td>
                <td>
                    <%= ShowStrLinkToSiteMapFile() %>
                    (<a href="<%= ShowStrLinkToSiteMapFile() %>" target="_blank"><asp:Localize ID="Localize_Admin_SiteMapGenerate_LinkSiteMapGo" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerate_LinkSiteMapGo %>"></asp:Localize></a>)
                </td>
            </tr>
            <tr style="background-color: #eff0f1; font-weight:bold">
                <td style="text-align: right; width:50%; font-weight:bold;height:26px;">
                    <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerate_Schedule %>" ></asp:Localize>
                </td>
                <td>
                    <a href="CommonSettings.aspx#tabid=task"><asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_SiteMapGenerate_Set %>"></asp:Localize></a> 
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:Button ID="btnCreateMap" runat="server" OnClick="createMap_Click" Text="<%$ Resources:Resource,Admin_SiteMapGenerate_ButtonGenerate%>" />
        
        <br />
        <br />
        <asp:Label ID="lblError" runat="server" ForeColor="Blue" Visible="False"></asp:Label>
    </center>
    </div>
</asp:Content>
