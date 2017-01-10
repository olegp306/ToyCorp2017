<%@ Page Language="C#" AutoEventWireup="true" CodeFile="m_ProductVideos.aspx.cs"
    MasterPageFile="m_MasterPage.master" Inherits="Admin.m_ProductVideos" ValidateRequest="false" %>

<asp:Content ID="contentHead" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        #alink, #aplayer
        {
            color: #027DC2;
            border-bottom: 1px dashed;
            text-decoration: none;
        }
        #preview
        {
            margin: 10px 0;
            text-align: center;
        }
    </style>
</asp:Content>
<asp:Content ID="contentScript" ContentPlaceHolderID="cphScript" runat="server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            $('#alink').click(function (e) {
                $('.player').hide();
                $('.link').show();
                e.preventDefault();
            });
            $('#aplayer').click(function (e) {
                $('.link').hide();
                $('.player').show();
                e.preventDefault();
            });
            if ($(".txtplayer").val().length > 0) {
                $('.link').hide();
                $('.player').show();
            }
        });
    </script>
</asp:Content>
<asp:Content ID="contentCenter" ContentPlaceHolderID="cphCenter" runat="server">
    <div style="margin-top:10px;">
        <center>
            <asp:Label ID="lblCustomer" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_m_ProductVideos_Header %>"></asp:Label>
            <br />
            <asp:Label ID="lblCustomerName" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_m_ProductVideos_SubHeader %>"></asp:Label>
        </center>
        <asp:Panel ID="pnlAdd" runat="server" Height="84px" Width="100%">
            <center>
                <asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="False"></asp:Label>&nbsp;</center>
            <table border="0" cellpadding="2" cellspacing="0" width="100%">
                <tr>
                    <td style="width:36%; text-align:right; padding:5px 5px;">
                        <%= Resources.Resource.Admin_m_ProductVideos_Name%>
                    </td>
                    <td style="vertical-align: middle; text-align: left; padding:5px 0px;">
                        <asp:TextBox ID="txtName" runat="server" Width="450" Text="" MaxLength="255" CssClass="niceTextBox shortTextBoxClass" />
                    </td>
                </tr>
                <tr style="background-color: #eff0f1;">
                    <td style="padding:5px 5px;">
                        &nbsp;
                    </td>
                    <td style="padding:5px 0px;">
                        <div id="preview" style="margin: 0 auto; width: 560px;" runat="server" visible="false"></div>
                        <div>
                            <%= Resources.Resource.Admin_m_ProductVideos_Enter %> <a href="#" id="alink"><%= Resources.Resource.Admin_m_ProductVideos_LinkToTheVideo %></a>
                            <%= Resources.Resource.Admin_m_ProductVideos_Or %> <a href="#" id="aplayer">
                                <%= Resources.Resource.Admin_m_ProductVideos_PlayerCode %></a>
                        </div>
                    </td>
                </tr>
                <tr style="background-color:#eff0f1;">
                    <td style="text-align:right; padding:5px 5px;">
                        <span class="link"><%= Resources.Resource.Admin_m_ProductVideos_Link %>: </span><span class="player" style="display: none;">
                            <%= Resources.Resource.Admin_m_ProductVideos_PlayerCode %>
                        </span>
                    </td>
                    <td style="vertical-align: middle; text-align: left; padding:5px 0px;">
                        <div class="link">
                            <asp:TextBox ID="txtVideoLink" runat="server" Text="" Width="450px" CssClass="niceTextBox shortTextBoxClass" />
                        </div>
                        <div class="player" style="display: none;">
                            <asp:TextBox ID="txtPlayerCode" runat="server" TextMode="MultiLine" Height="60" Width="450"
                                Text="" CssClass="txtplayer niceTextBox" />
                        </div>
                    </td>
                </tr>
                <tr style="padding: 3px;">
                    <td style="text-align:right; padding:5px 5px;">
                        <%= Resources.Resource.Admin_m_ProductVideos_VideoDescription%>
                    </td>
                    <td style="vertical-align: middle; text-align: left;">
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Height="60" Width="450" Text="" CssClass="niceTextBox" />
                    </td>
                </tr>
                <tr style="background-color: #eff0f1;">
                    <td style="text-align:right; padding:5px 5px;">
                        <%= Resources.Resource.Admin_m_Product_Order%>
                    </td>
                    <td style="vertical-align: middle; text-align: left; padding:5px 0px;">
                        <asp:TextBox ID="txtSortOrder" runat="server" Text="0" CssClass="niceTextBox shortTextBoxClass" />
                    </td>
                </tr>
            </table>
            <div style="text-align: center; margin: 15px 0;">
                <asp:Button ID="btnOK" runat="server" Text="<%$ Resources:Resource, Admin_m_ProductVideos_Ok %>"
                    CssClass="btn btn-middle btn-add" OnClick="btnOK_Click" />
            </div>
            <br />
        </asp:Panel>
    </div>
</asp:Content>
