<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeFile="Subscribe.aspx.cs" Inherits="ClientPages.Subscribe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <h1>
                <%=Resources.Resource.Client_Subscribe_Header%></h1>
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0" EnableTheming="True">
            <asp:View ID="ViewDataCollecting" runat="server">
                <div class="form-c">
                    <div class="title">
                        <%= Resources.Resource.Client_Subscribe_EmailForSubscribe %></div>
                    <ul class="form">
                        <li>
                            <adv:AdvTextBox ID="txtEmail" runat="server" ValidationType="Email" /></li>
                    </ul>
                    <div>
                        <adv:Button ID="btnSubscribe" runat="server" Type="Action" Size="Middle" Text="<%$ Resources:Resource, Client_Subscribe_Subscribe %>"
                            OnClick="btnSubscribe_Click"></adv:Button>
                        <a id="aDeactive" runat="server" href="subscribedeactivate.aspx" title="<%$ Resources:Resource, Client_Subscribe_DeactiveSubscribe %>">
                            <%=Resources.Resource.Client_Subscribe_CancelingSubscribe%></a>
                    </div>
                </div>
            </asp:View>
            <asp:View ID="ViewEmailSend" runat="server">
                <div class="form-c">
                    <asp:Label ID="lblInfo" runat="server" Visible="False" Text=""></asp:Label>
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
