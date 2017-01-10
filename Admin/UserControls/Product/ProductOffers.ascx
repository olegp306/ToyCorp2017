<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductOffers.ascx.cs" Inherits="Admin.UserControls.Products.ProductOffers" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="True">
    <ContentTemplate>
        <table>
            <tr>
                <td style="height: 29px; vertical-align: middle;">
                    <%=Resources.Resource.Admin_Product_VaryByColorAndSize%>
                    <asp:CheckBox ID="chkMultiOffer" runat="server" Checked="false" CssClass="checkly-align"
                        AutoPostBack="true" OnCheckedChanged="chkMultiOffer_Click" />
                    <div data-plugin="help" class="help-block">
                        <div class="help-icon js-help-icon"></div>
                        <article class="bubble help js-help">
                            <header class="help-header">
                                Цены и наличие
                            </header>
                            <div class="help-content">
                                Данная опция определяет - одна цена у товара, или меняется в зависимости от цвета и размера.
                            </div>
                        </article>
                    </div>
                </td>
            </tr>
        </table>
        <br />
        <asp:MultiView runat="server" ActiveViewIndex="0" ID="mvOffers">
            <asp:View runat="server" ID="viewSingleOffer">
                <table width="500px" class="table-offer">
                    <tr>
                        <th>
                            <%=Resources.Resource.Admin_Product_Price %>
                        </th>
                        <th>
                            <%=Resources.Resource.Admin_Product_SupplyPrice %>
                        </th>
                        <th>
                            <%=Resources.Resource.Admin_Product_Amount %>
                        </th>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtPrice" runat="server" Width="160px" Text="0" CssClass="niceTextBox shortTextBoxClass3"/>
                            <asp:RangeValidator ID="rvPrice" runat="server" ControlToValidate="txtPrice" ValidationGroup="2" Display="Dynamic"
                                EnableClientScript="true" ErrorMessage='<%$ Resources:Resource,Admin_Product_EnterValidNumber %>'
                                MaximumValue="1000000000" MinimumValue="0" Type="Double" />
                            <asp:RequiredFieldValidator ID="RangeValidator9" runat="server" ControlToValidate="txtPrice" ValidationGroup="2"
                                Display="Dynamic" EnableClientScript="true" ErrorMessage='<%$ Resources:Resource,Admin_Product_EnterValidNumber %>' />
                        </td>
                        <td>
                            <asp:TextBox ID="txtSupplyPrice" runat="server" Width="160px" Text="0" CssClass="niceTextBox shortTextBoxClass3"/>
                            <asp:RangeValidator ID="rvSupplyPrice" runat="server" ControlToValidate="txtSupplyPrice" ValidationGroup="2"
                                Display="Dynamic" EnableClientScript="true" ErrorMessage='*' MaximumValue="1000000000" MinimumValue="0"
                                Type="Double" />
                            <asp:RequiredFieldValidator ID="RangeValidator10" runat="server" ControlToValidate="txtSupplyPrice" ValidationGroup="2"
                                Display="Dynamic" EnableClientScript="true" ErrorMessage='*' />
                        </td>
                        <td>
                            <asp:TextBox ID="txtAmount" runat="server" Width="160px" Text="1" CssClass="niceTextBox shortTextBoxClass3"/>
                            <asp:RangeValidator ID="RangeValidator4" runat="server" ControlToValidate="txtAmount" ValidationGroup="2"
                                Display="Dynamic" EnableClientScript="true" ErrorMessage='*' MaximumValue="100000" MinimumValue="-100000"
                                Type="Double" />
                            <asp:RequiredFieldValidator ID="RangeValidator12" runat="server" ControlToValidate="txtAmount" ValidationGroup="2"
                                Display="Dynamic" EnableClientScript="true" ErrorMessage='*' />
                        </td>
                    </tr>
                </table>
            </asp:View>
            <asp:View runat="server" ID="viewMultiOffer">
                <table class="table-offer">
                    <tr>
                        <th>
                            <%=Resources.Resource.Admin_Product_Main %>
                        </th>
                        <th>
                            <%=Resources.Resource.Admin_Product_StockNumber%>
                        </th>
                        <th>
                            <%=SettingsCatalog.SizesHeader%>
                        </th>
                        <th>
                            <%=SettingsCatalog.ColorsHeader%>
                        </th>
                        <th>
                            <%=Resources.Resource.Admin_Product_Price %>
                        </th>
                        <th>
                            <%=Resources.Resource.Admin_Product_SupplyPrice %>
                        </th>
                        <th>
                            <%=Resources.Resource.Admin_Product_Amount %>
                        </th>
                    </tr>
                    <asp:ListView ID="lvOffers" runat="server" OnItemCommand="lvOffers_ItemCommand">
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="6">
                                    <%=Resources.Resource.Admin_Product_NoOffres %>
                                </td>
                            </tr>
                        </EmptyDataTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:RadioButton ID="cbMultiMain" runat="server" CssClass="cbMain" Checked='<%# Eval("Main") %>' />
                                </td>
                                <td>
                                    <asp:HiddenField runat="server" ID="hfOfferID" Value='<%# Eval("OfferID") %>' />
                                    <asp:TextBox ID="txtMultySKU" runat="server" Width="100px" Text='<%# Eval("ArtNo") %>' CssClass="niceTextBox shortTextBoxClass3" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtMultySKU"
                                        Display="Dynamic" EnableClientScript="true" ErrorMessage='*' />
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlMultiSize" runat="server" Width="100px" DataTextField="SizeName" DataValueField="SizeID"
                                        DataSourceID="sdsSizes" SelectedValue='<%# Eval("SizeID") ?? "null" %>' />
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlMultiColor" runat="server" Width="100px" DataTextField="ColorName" DataValueField="ColorID"
                                        DataSourceID="sdsColors" SelectedValue='<%# Eval("ColorID") ?? "null"  %>' />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMultiPrice" runat="server" Width="100px" Text='<%# Eval("Price") %>' CssClass="niceTextBox shortTextBoxClass3" />
                                    <asp:RangeValidator ID="RangeValidator16" runat="server" ControlToValidate="txtMultiPrice" ValidationGroup="2"
                                        Display="Dynamic" EnableClientScript="true" ErrorMessage='*' MaximumValue="1000000000" MinimumValue="0"
                                        Type="Double" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtMultiPrice"
                                        ValidationGroup="2" Display="Dynamic" EnableClientScript="true" ErrorMessage='*' />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMultiSupplyPrice" runat="server" Width="100px" Text='<%# Eval("SupplyPrice") %>' CssClass="niceTextBox shortTextBoxClass3" />
                                    <asp:RangeValidator ID="RangeValidator17" runat="server" ControlToValidate="txtMultiSupplyPrice" ValidationGroup="2"
                                        Display="Dynamic" EnableClientScript="true" ErrorMessage='*' MaximumValue="1000000000" MinimumValue="0"
                                        Type="Double" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtMultiSupplyPrice"
                                        ValidationGroup="2" Display="Dynamic" EnableClientScript="true" ErrorMessage='*' />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtMultiAmount" runat="server" Width="100px" Text='<%# Eval("Amount") %>' CssClass="niceTextBox shortTextBoxClass3" />
                                    <asp:RangeValidator ID="RangeValidator18" runat="server" ControlToValidate="txtMultiAmount" ValidationGroup="2"
                                        Display="Dynamic" EnableClientScript="true" ErrorMessage='*' MaximumValue="100000" MinimumValue="-100000"
                                        Type="Double" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtMultiAmount"
                                        ValidationGroup="2" Display="Dynamic" EnableClientScript="true" ErrorMessage='*' />
                                </td>
                                <td>
                                    <asp:ImageButton runat="server" ImageUrl="~/Admin/images/deletebtn.png" CommandName="DeleteOffer" CommandArgument='<%# Eval("OfferID") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:ListView>
                </table>
                <asp:Label runat="server" ID="lErrorOffer" Text="*" ForeColor="Red" Visible="false" />
                <div style="margin-top:10px;">
                    <asp:Button ID="lbNewOffer" runat="server" CssClass="btn btn-middle btn-add"
                        Text="<%$ Resources:Resource, Admin_Product_NewOffer %>" OnClick="lbNewOffer_Click" />
                </div>
            </asp:View>
        </asp:MultiView>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:SqlDataSource runat="server" ID="sdsColors" SelectCommand="Select 'null' as ColorID, '––––' as ColorName, -1000 as SortOrder Union Select cast(ColorID as nvarchar(10)), ColorName, SortOrder From Catalog.Color order by  SortOrder, ColorName"
    OnInit="sds_Init"></asp:SqlDataSource>
<asp:SqlDataSource runat="server" ID="sdsSizes" SelectCommand="Select 'null' as SizeID, '––––' as SizeName, -1000 as SortOrder  Union Select cast(SizeID as nvarchar(10)), SizeName, SortOrder From Catalog.Size order by SortOrder, SizeName"
    OnInit="sds_Init"></asp:SqlDataSource>
<script type="text/javascript">
    $('body').on("click", ".cbMain>input", function () {
        if ($(this).is(":checked")) {
            $(".cbMain>input").removeAttr("checked");
            $(this).attr("checked", "checked");
        }
    });
</script>
