<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BuyInTimeModule.ascx.cs" Inherits="Advantshop.Modules.UserControls.BuyInTime.Admin_BuyInTimeModule" %>
<div style="text-align: center;">
    <table border="0" cellpadding="2" cellspacing="0">
        <tr class="rowsPost">
            <td colspan="2" style="height: 34px;">
                <span class="spanSettCategory">
                    <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources: BuyInTime_Header%>" /></span>
                <asp:Label ID="lblMessage" runat="server" Visible="False" Style="float: right;"></asp:Label>
                <hr color="#C2C2C4" size="1px" />
            </td>
        </tr>
        <tr class="rowsPost">
            <td style="width: 170px; text-align: left;">
                <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources: BuyInTime_ActionContent%>"></asp:Localize>
            </td>
            <td>
                <CKEditor:CKEditorControl ID="ckeActionTitle" BasePath="~/ckeditor/" runat="server" Height="170px" Width="700px" />
                <div style="margin-bottom: 20px;">
                    <asp:Localize runat="server" Text="<%$ Resources: BuyInTime_ActionContent_Hint%>" /></div>
            </td>
        </tr>
        <tr class="rowsPost">
            <td style="width: 170px; text-align: left;">
                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources: BuyInTime_ActionLabelContent%>"></asp:Localize>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtLabelCode" Width="100%" />
            </td>
        </tr>
        <tr class="rowsPost">
            <td style="width: 170px; text-align: left; vertical-align: top; padding: 20px 0 0 0">
                <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: BuyInTime_CurrentAction%>" />
            </td>
            <td style="padding: 20px 0 0 0">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                    <ContentTemplate>

                        <asp:ListView ID="rprProducts" runat="server" OnItemCommand="rprProducts_ItemCommand" ItemPlaceholderID="trPlaceholderID">
                            <LayoutTemplate>
                                <table class="table-ui">
                                    <thead>
                                        <th>
                                            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources: BuyInTime_Name%>" /></th>
                                        <th>
                                            <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources: BuyInTime_Discount%>" /></th>
                                        <th>
                                            <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources: BuyInTime_DateStart%>" /></th>
                                        <th>
                                            <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources: BuyInTime_DateExpired%>" /></th>
                                        <th>
                                            <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources: BuyInTime_Status%>" /></th>
                                        <th>
                                            <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources: BuyInTime_SortOrder%>" /></th>
                                        <th>&nbsp;</th>
                                        <th>&nbsp;</th>
                                    </thead>
                                    <tbody>
                                        <tr id="trPlaceholderID" runat="server"></tr>
                                    </tbody>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%# Eval("Name")%>
                                    </td>
                                    <td>
                                        <%# Eval("DiscountInTime")%>
                                    </td>
                                    <td>
                                        <%# Eval("DateStart")%>
                                    </td>
                                    <td>
                                        <%# Eval("DateExpired")%>
                                    </td>
                                    <td>
                                        <%# RenderStatus((DateTime)Eval("DateStart"), (DateTime)Eval("DateExpired"),(bool)Eval("IsRepeat"),(int)Eval("DaysRepeat"))%>
                                    </td>
                                    <td>
                                        <%# Eval("SortOrder")%>
                                    </td>
                                    <td>
                                        <a href='<%# "javascript:open_window(\"../Modules/BuyInTime/BuyInTimeModuleAddEdit.aspx?Id=" + Eval("Id") +"\",900,700)"%>'>
                                            <asp:Image runat="server" ImageUrl="~/Modules/BuyInTime/images/editbtn.gif" EnableViewState="false" />
                                        </a>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="lb" runat="server" CommandArgument='<%#Eval("Id")%>' CommandName="DeleteItem" OnClientClick='<%# "return confirm(\"" + GetLocalResourceObject("BuyInTime_DeleteAction") + "\");" %>'>
                                            <asp:Image ID="Image1" runat="server" ImageUrl="~/Modules/BuyInTime/images/remove.jpg" EnableViewState="false" />
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <EmptyItemTemplate>
                                <tr>
                                    <td colspan="5">
                                        <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources: BuyInTime_NoAction%>" />
                                    </td>
                                </tr>
                            </EmptyItemTemplate>
                        </asp:ListView>

                    </ContentTemplate>
                </asp:UpdatePanel>
                <div style="margin: 15px 0">
                    <a href='javascript:open_window("../Modules/BuyInTime/BuyInTimeModuleAddEdit.aspx",1000,800)'>
                        <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources: BuyInTime_AddAction%>" /></a>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="<%$ Resources: BuyInTime_Save%>" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label runat="server" ID="lblErr" Visible="False"></asp:Label>
            </td>
        </tr>
    </table>
</div>
