<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CleanUp.aspx.cs" Inherits="Tools.core.CleanUp" MasterPageFile="MasterPage.master" %>

<asp:Content runat="server" ID="cntHead" ContentPlaceHolderID="head">
    <title>AdvantShop.NET Core Tools - Cleanup</title>
    <style type="text/css">
        .Header1{font-family:Tahoma;font-weight:700}
        .ContentDiv{font:.75em 'Lucida Grande' ,sans-serif}
        .Label{font-family:Tahoma;font-size:16px;color:#666}
        .clsText{border:1px solid #DDD;padding:3px;font-size:14px}
        .clsText_faild{border:1px solid #E5A3A3;padding:3px;font-size:14px;background-color:#FFCFCF}
        .label-box{border-color:#DBDBDB;border-style:solid;border-width:1px;color:#666;display:none;font-size:14px;line-height:1.45em;padding:.85em 10px;text-transform:lowercase;width:735px;display:block}
        .label-box.good{background-color:#D3F9BF;border-color:#E1EFDB}
        .label-box.error{background-color:#FFCFCF;background-image:none;border-color:#E5A3A3;color:#801B1B;padding-left:10px}
        .btn{background:url(../img/bg-btn.gif) repeat-x scroll 0 0 #DDD;border-color:#DDD #DDD #CCC;border-style:solid;border-width:1px;color:#333;cursor:pointer;font:11px/14px "Lucida Grande" ,sans-serif;margin:0;overflow:visible;padding:4px 8px 5px;width:auto}
        .btn-m{background-position:0 -200px;font-size:15px;line-height:20px!important;padding:5px 15px 6px}
        .btn-m:hover,.btn-m:focus{background-position:0 -206px}
        .fieldsetData{margin-bottom:22px;padding:12px 15px 15px;font-family:Tahoma}
        .tableActualData{font-size:14px;margin-top:5px}
    </style>
</asp:Content>
<asp:Content runat="server" ID="cntmain" ContentPlaceHolderID="main">
    <fieldset style="margin-bottom: 22px; padding: 15px 15px 15px 15px;">
        <div style="margin-bottom: 22px;">
            <asp:Button ID="btnCleanUpPictureFolder" runat="server" Text="Clean product pictures folder"
                OnClick="btnCleanUpPictureFolder_Click" />
        </div>
        <div style="margin-bottom: 22px;">
            <asp:CheckBox ID="chboxDeleteFiles" runat="server" Text="Delete files" />
        </div>
        <div style="font-family: Courier New; color: Blue; font-weight: bold; font-size: 11pt;
            margin-bottom: 11px;">
            <asp:Label ID="lCompleted" runat="server" EnableViewState="false" Visible="false">Cleanup successfuly completed</asp:Label>
        </div>
        <div>
            <asp:Label ID="lResultHeader" runat="server" Visible="false" EnableViewState="false">Deleted files:</asp:Label>
        </div>
        <div style="font-family: Courier New; font-size: 10pt;">
            <div style="text-align: left;">
                <asp:Literal ID="lResult" runat="server" EnableViewState="false" Text="" />
            </div>
        </div>
    </fieldset>
    <fieldset style="margin-bottom: 22px; padding: 15px 15px 15px 15px;">
        <div style="margin-bottom: 22px;">
            <asp:Button ID="btnCleanUpBD" runat="server" Text="Cleanup DB" OnClick="btnCleanUpBD_Click" />
        </div>
        <div style="margin-bottom: 22px;">
            <asp:CheckBox ID="chboxMakeNull" runat="server" Text="Delete wrong data" />
        </div>
        <div style="font-family: Courier New; color: Blue; font-weight: bold; font-size: 11pt;
            margin-bottom: 11px;">
            <asp:Label ID="lDBCleanupCompleted" runat="server" EnableViewState="false" Visible="false">Cleanup successfuly completed</asp:Label>
        </div>
        <div style="font-family: Courier New; font-size: 10pt;">
            <div style="text-align: left;">
                <asp:Literal ID="lDBResult" runat="server" EnableViewState="false" Text="" />
            </div>
        </div>
    </fieldset>
    <fieldset style="margin-bottom: 22px; padding: 15px 15px 15px 15px;">
        <div style="margin-bottom: 22px;">
            Folder '~/pictures_deleted/' current size:
            <asp:Label ID="lblFolderSize" runat="server" EnableViewState="false" />
            <asp:Button ID="Button3" runat="server" Text="Get FolderSize" OnClick="GetDeletedFolderSize" /><br /><br />
            <asp:Button ID="Button2" runat="server" Text="Clear '~/pictures_deleted' folder" OnClick="CleanDeletedFolder" />
        </div>

    </fieldset>
</asp:Content>
