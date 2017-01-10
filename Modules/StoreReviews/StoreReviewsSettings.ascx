<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StoreReviewsSettings.ascx.cs"
    Inherits="Advantshop.UserControls.Modules.StoreReviews.Admin_StoreReviewsSettings" %>
<div>
    <table border="0" cellpadding="2" cellspacing="0">
        <tr class="rowsPost">
            <td colspan="2" style="height: 34px;">
                <span class="spanSettCategory">
                    <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: StoreReviews_Header%>" /></span>
                <asp:Label ID="lblMessage" runat="server" Visible="False" Style="float: right; margin-left: 10px;"></asp:Label>
                <hr color="#C2C2C4" size="1px" />
            </td>
        </tr>
<%--        <tr class="rowsPost">
            <td style="width: 400px; text-align: left;">
                <asp:Localize ID="Localize10" runat="server" Text="<%$ Resources: StoreReviews_Active%>"></asp:Localize>
            </td>
            <td>
                <asp:CheckBox runat="server" ID="ckbEnableStoreReviews" />
            </td>
        </tr>--%>
        <tr class="rowsPost">
            <td style="width: 400px; text-align: left;">
                <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: StoreReviews_ShowRatio%>"></asp:Localize>
            </td>
            <td>
                <asp:CheckBox runat="server" ID="chkShowRatio" Checked="True" />
            </td>
        </tr>
        <tr class="rowsPost">
            <td style="width: 400px; text-align: left;">
                <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: StoreReviews_PageSize%>"></asp:Localize>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtPageSize"></asp:TextBox>
            </td>
        </tr>
        <tr class="rowsPost">
            <td style="width: 400px; text-align: left;">
                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: StoreReviews_ActiveModerate%>"></asp:Localize>
            </td>
            <td>
                <asp:CheckBox runat="server" ID="ckbActiveModerate" />
            </td>
        </tr>
        
        <tr class="rowsPost">
            <td style="width: 400px; text-align: left;">
                <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: StoreReviews_PageTitle %>"></asp:Localize>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtPageTitle"></asp:TextBox>
            </td>
        </tr>
        
        <tr class="rowsPost">
            <td style="width: 400px; text-align: left;">
                <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources: StoreReviews_MetaKeyWords %>"></asp:Localize>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtMetaKeyWords"></asp:TextBox>
            </td>
        </tr>
        
        <tr class="rowsPost">
            <td style="width: 400px; text-align: left;">
                <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources: StoreReviews_MetaDescription %>"></asp:Localize>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtMetaDescription"></asp:TextBox>
            </td> 
        </tr>

        <tr class="rowsPost">
            <td style="width: 400px; text-align: left;">
                <asp:Localize ID="Localize4" runat="server" Text=""></asp:Localize>
            </td>
            <td>
                <asp:HyperLink href="../storereviews" runat="server" Text="<%$ Resources:StoreReviews_URL%>" Target="_blank" />
            </td>
        </tr>

        <tr>
            <td colspan="2">
                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="<%$ Resources:StoreReviews_Save %>" />
            </td>
        </tr>
    </table>
</div>
