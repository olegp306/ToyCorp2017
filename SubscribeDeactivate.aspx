<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeFile="SubscribeDeactivate.aspx.cs" Inherits="ClientPages.SubscribeDeactivate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <h1>
                <%=Resources.Resource.Client_SubscribeDeactivate_Header%></h1>
           <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0" EnableTheming="True">
                <asp:View ID="viewDataCollecting" runat="server">
                    <div class="form-c">
                        <div class="title">
                            <%= Resources.Resource.Client_SubscribeDeactivate_Email%>
                        </div>
                        <ul class="form">
                            <li>
                                <adv:AdvTextBox ID="txtEmailAdress" runat="server" ValidationType="Email" /></li>
                            <li><span class="param-name">
                                <%=Resources.Resource.Client_SubscribeDeactivate_Reason %></span> <span class="param-value-textarea">
                                    <adv:AdvTextBox ID="txtDeactivateReason" runat="server" TextMode="MultiLine" />
                                </span></li>
                        </ul>
                        <div>
                            <adv:Button ID="btnDeactivate" runat="server" Type="Action" Size="Middle" Text="<%$ Resources:Resource, Client_SubscribeDeactivate_Delete %>"
                                OnClick="btnDeactivate_Click"></adv:Button>
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="viewMessage" runat="server">
                    <div class="form-c">
                        <asp:Label ID="lblError" runat="server" Visible="False" Font-Bold="True" ForeColor="Red"></asp:Label>
                        <asp:Label ID="lblInfo" runat="server" Visible="False"></asp:Label>
                        <asp:Label ID="lblActivated" runat="server" Visible="False"></asp:Label>
                        <div class="form-padding">
                            <adv:Button ID="Button1" runat="server" Type="Action" Size="Middle" Text='<%$ Resources:Resource, Client_FogotPassword_ToMainPage %>'
                                Href="." />
                        </div>
                    </div>
                </asp:View>
            </asp:MultiView>
        </div>
    </div>
</asp:Content>
