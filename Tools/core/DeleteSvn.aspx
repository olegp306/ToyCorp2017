<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DeleteSvn.aspx.cs" Inherits="Tools.core.DeleteSvn"
    MasterPageFile="MasterPage.master" %>

<asp:Content runat="server" ID="cntHead" ContentPlaceHolderID="head">
    <title>AdvantShop.NET Tools</title>
</asp:Content>
<asp:Content runat="server" ID="cntMain" ContentPlaceHolderID="main">
    <fieldset style="margin-bottom: 22px; padding: 15px 15px 15px 15px;">
        <div style="margin-bottom: 22px;">
            <asp:Button ID="btnDeleteSvn" runat="server" Text="Clean *.svn files" OnClick="btnDeleteSvn_Click" />
        </div>
        <div style="margin-bottom: 22px;">
            <asp:CheckBox ID="chboxDeleteFiles" runat="server" Text="Delete files" />
        </div>
        <div style="font-family: Courier New; color: Blue; font-weight: bold; font-size: 11pt;
            margin-bottom: 11px;">
            <asp:Label ID="lCompleted" runat="server" EnableViewState="false" Visible="false">Cleanup successfuly completed</asp:Label>
        </div>
        <div>
            <asp:Label ID="lResultHeader" runat="server" Visible="false" EnableViewState="false"
                Font-Bold="true">Result:</asp:Label>
        </div>
        <div style="font-family: Courier New; font-size: 10pt;">
            <div style="text-align: left;">
                <asp:Literal ID="lResult" runat="server" EnableViewState="false" Text="" />
            </div>
        </div>
    </fieldset>
</asp:Content>
