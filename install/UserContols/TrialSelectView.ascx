<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TrialSelectView.ascx.cs"
    Inherits="ClientPages.install_UserContols_TrialSelectView" %>

<h1><asp:Label runat="server" ID="lblHead" Text=""></asp:Label></h1>
<div class="group">
    <div class="express" id="divExpress" runat="server">
        <asp:RadioButton runat="server" ID="rbExpressInstall" Text="<%$Resources:Resource, Install_UserContols_TrialSelectView_Express%>"
            Checked="true" GroupName="installType" />
        <br />
        <% = Resources.Resource.Install_UserContols_TrialSelectView_ExpressDecsription %>
        <br />
        <br />
        <asp:RadioButton runat="server" ID="rbFullInstall" Text="<%$Resources:Resource, Install_UserContols_TrialSelectView_Full%>" GroupName="installType" />
        <br />
        <% = Resources.Resource.Install_UserContols_TrialSelectView_FullDecsription %>
        
    </div>
    <div id="divNoWriteAccess" runat="server" Visible="False">
        <div class="group" runat="server" id="divHasNoWriteAccess">
            <p>
                <%= Resources.Resource.Install_UserContols_ShopinfoView_HasNoWriteAccess%></p>
            <p>
                <%= Resources.Resource.Install_UserContols_ShopinfoView_HasNoWriteAccess_Desc%></p>
            <br/>
            <a class="check_other_folders" id="check_folders" onclick="javascript:void(0);"><%= Resources.Resource.Install_UserContols_TrialSelectView_AccsessRights_CheckOtherFolders %></a>
            <div id="load_content">
                <img src="images/ajax-loader.gif"/>
            </div>
            <div id="access_info" style="margin-top: 5px;"></div>
        </div>
    </div>
</div>
