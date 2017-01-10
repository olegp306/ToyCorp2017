<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true"
    CodeFile="Module.aspx.cs" Inherits="Admin.Module" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Head" runat="Server">
    <script type="text/javascript">
        function showElement(span) {
            var method_id = $("input:hidden", $(span)).val();
            location = "module.aspx?module=<%= Server.UrlEncode(Request["module"]) %>&currentcontrolindex=" + method_id;
        }
    </script>
    <link href="css/new_admin/modules-settings.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="content-own">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td colspan="3" style="vertical-align: middle; padding-bottom: 15px;">
                    <img src="images/orders_ico.gif" alt="" style="float: left; margin-right: 10px;" />
                    <div style="float: left;">
                        <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_Module_Header %>"></asp:Label><asp:Label
                            ID="lblIsActive" CssClass="lblIsActive" runat="server" Style="margin-left: 30px;
                            font-size: 18px;"></asp:Label><br />
                        <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_Module_SubHeader %>"></asp:Label>
                        <div style="margin-top: 10px;">
                            <label><input type="checkbox" id="ckbActiveModule" runat="server" class="ckbActiveModule"
                                data-modulestringid="" />&nbsp;<asp:Label ID="lblActiveModule" runat="server" Text="<%$ Resources:Resource, Admin_Module_ModuleActive%>"></asp:Label></label></div>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; width: 100%" colspan="3">
                    <table cellpadding="0px" cellspacing="0px" style="width: 100%;">
                        <tr>
                            <td style="vertical-align: top; width: 225px;">
                                <ul class="tabs" id="tabs-headers">
                                    <asp:Repeater runat="server" ID="rptTabs">
                                        <ItemTemplate>
                                            <li id="Li1" runat="server" onclick="javascript:showElement(this)" class='<%# Container.ItemIndex == CurrentControlIndex ? "selected" : "" %>'>
                                                <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Container.ItemIndex %>' />
                                                <asp:Label ID="Literal4" runat="server" Text='<%# Eval("NameTab") %>' />
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ul>
                            </td>
                            <td class="tabContainer" id="tabs-contents">
                                <asp:Panel ID="pnlBody" runat="server" Style="width: 100%">
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:Label runat="server" ID="lblInfo"></asp:Label>
    </div>
</asp:Content>
