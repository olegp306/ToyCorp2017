﻿<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true"
    CodeFile="about.aspx.cs" Inherits="Admin.About" Title="AdVantShop.NET - About us" %>

<%@ Import Namespace="AdvantShop.Trial" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <table style="width: 100%; height: 300px">
        <tr>
            <td>
                <p style="text-align: center;"><span style="font-size: 24px;">Ad<b>Vant</b>Shop.NET Ultimate</span></p>
                <p style="text-align: center;"><em>Best software in your hands</em></p>
                <p style="text-align: center;"><%=AdvantShop.Configuration.SettingsGeneral.SiteVersion%></p>
            </td>
        </tr>
        <tr>
            <td style="text-align: center">
                <div id="share" data-event="<%= TrialEvents.ShareInSocialNetwork %>">
                    <a href="http://vk.com/share.php?url=http://www.advantshop.net" target="_blank" data-network="vk" title="Поделиться ВКонтакте"><img src="http://cap.advantshop.net/files/AchievementInstuctions/12.1.png" class="social" /></a>
                    <a href="https://www.facebook.com/sharer/sharer.php?src=sp&u=http://www.advantshop.net" target="_blank" data-network="fb" title='Поделиться в Facebook'><img src="http://cap.advantshop.net/files/AchievementInstuctions/12.2.png"  class="social"  /></a>
                    <a href="http://www.odnoklassniki.ru/dk?st.cmd=addShare&st._surl=http://www.advantshop.net" target="_blank" data-network="od" title='Поделиться в Одноклассниках'><img src="http://cap.advantshop.net/files/AchievementInstuctions/12.3.png"  class="social" /></a>
                    <a href="https://twitter.com/intent/tweet?status=AdVantShop.NET Ultimate" target="_blank" data-network="tw" title='Поделиться в Twitter'><img src="http://cap.advantshop.net/files/AchievementInstuctions/12.4.png"  class="social"  /></a>
                </div>
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
