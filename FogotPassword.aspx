<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeFile="FogotPassword.aspx.cs" Inherits="ClientPages.FogotPassword" Title="Untitled Page" %>

<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <h1>
                <%=Resources.Resource.Client_FogotPassword_Header%></h1>
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="ViewDataCollecting" runat="server">
                    <div class="overfl">
                        <div class="form-c">
                            <div class="title">
                                <%=Resources.Resource.Client_FogotPassword_EnterEmail%>
                            </div>
                            <ul class="form">
                                <li>
                                    <adv:AdvTextBox ValidationType="Login" SpanClass="input-auth" Placeholder="Email"
                                        runat="server" ID="txtEmail" />
                                </li>
                            </ul>
                            <div>
                                <adv:Button ID="btnSendPasswordByEmail" runat="server" Type="Action" Size="Middle"
                                    Text="<%$ Resources:Resource, Client_FogotPassword_Recover %>" OnClick="btnSendPasswordByEmail_Click"></adv:Button>
                                <br />
                                <br />
                            </div>
                        </div>
                        <div class="form-addon">
                            <div class="form-addon-text">
                                <div class="new-user-text">
                                    <adv:StaticBlock ID="staticBlockReg" runat="server" SourceKey="recoverPassword" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="ViewEmailSend" runat="server">
                    <div class="overfl">
                        <div class="form-c">
                            <%=Resources.Resource.Client_FogotPassword_MessageWasSent%>
                            <div class="form-padding">
                                <adv:Button runat="server" Type="Action" Size="Middle" Text='<%$ Resources:Resource, Client_FogotPassword_ToMainPage %>'
                                    Href="." />
                                <br />
                                <br />
                            </div>
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="ViewEmailSendError" runat="server">
                    <div class="overfl">
                        <div class="form-c">
                            <%=Resources.Resource.Client_FogotPassword_EmailNotFound%>
                            <div class="form-padding">
                                <adv:Button ID="Button1" runat="server" Type="Action" Size="Middle" Text='<%$ Resources:Resource, Client_FogotPassword_ToMainPage %>'
                                    Href="." />
                                <br />
                                <br />
                            </div>
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="ViewRecovery" runat="server">
                    <div class="overfl">
                        <div class="form-c">
                            <div class="title">
                                <%=Resources.Resource.Client_FogotPassword_ChangePassword%>
                            </div>
                            <ul class="form">
                                <li>
                                    <adv:AdvTextBox ValidationType="CompareSource" SpanClass="input-auth" TextMode="Password"
                                        Placeholder='<%$Resources:Resource, Client_FogotPassword_NewPassword%>' runat="server"
                                        ID="txtNewPassword" DefaultButtonID="btnChangePassword" />
                                </li>
                                <li>
                                    <adv:AdvTextBox ValidationType="Compare" SpanClass="input-auth" TextMode="Password"
                                        Placeholder='<%$Resources:Resource, Client_FogotPassword_NewPasswordAgain%>'
                                        runat="server" ID="txtNewPasswordConfirm" DefaultButtonID="btnChangePassword" />
                                </li>
                            </ul>
                            <div>
                                <adv:Button ID="btnChangePassword" runat="server" Type="Action" Size="Middle" Text='<%$ Resources:Resource, Client_FogotPassword_ChangePassword %>'
                                    OnClick="btnChangePassword_Click"></adv:Button>
                                <br />
                                <br />
                            </div>
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="ViewRecoveryError" runat="server">
                    <div class="form-c">
                        <%=Resources.Resource.Client_FogotPassword_Error%>
                        <div class="form-padding">
                            <adv:Button runat="server" Type="Action" Size="Middle" Text='<%$ Resources:Resource, Client_FogotPassword_ToMainPage %>'
                                Href='.' />
                            <br />
                            <br />
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="ViewPasswordChanged" runat="server">
                    <div class="overfl">
                        <div class="form-c">
                            <%=Resources.Resource.Client_FogotPassword_PasswordChanged%>
                            <div class="form-padding">
                                <adv:Button runat="server" Type="Action" Size="Middle" Text='<%$ Resources:Resource, Client_FogotPassword_ToMainPage %>'
                                    Href='.' />
                                <br />
                                <br />
                            </div>
                        </div>
                    </div>
                </asp:View>
            </asp:MultiView>
        </div>
    </div>
</asp:Content>
