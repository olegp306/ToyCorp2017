<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VersionLabel.ascx.cs" Inherits="UserControls.Default.VersionLabel" EnableViewState="false" %>
<!-- Current version --- staring --- -->
<%  if (this.Visible)
    {%>
<div class="block" style="margin-top: -4px;">
    <div class="block_header">
        <div class="block_name">
            <%=Resources.Resource.Client_MasterPage_Version%></div>
    </div>
    <div class="block_middle">
        <div class="block_content" style="font-size: 11px; text-align: center;">
            <b>
                <%=Resources.Resource.Client_MasterPage_CurrentVersion%>:</b>
            <%= AdvantShop.Configuration .SettingsGeneral.SiteVersion%>
            <br />
            <b>uptime:</b>
            <br /><%= GetUptimeString()%>
        </div>
    </div>
    <div class="block_footer">
    </div>
</div>
<% }%>
<!-- Current version --- ending --- -->
