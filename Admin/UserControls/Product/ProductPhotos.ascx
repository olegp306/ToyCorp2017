<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductPhotos.ascx.cs"
    Inherits="Admin.UserControls.Products.ProductPhotos" %>
<%@ Import Namespace="AdvantShop.FilePath" %>
<script type="text/javascript">
    function CreateHistory(hist) {
        $.historyLoad(hist);
    }

    var timeOut;
    function Darken() {
        timeOut = setTimeout('document.getElementById("inprogress").style.display = "block";', 1000);
    }

    function Clear() {
        clearTimeout(timeOut);
        document.getElementById("inprogress").style.display = "none";

        $("input.sel").each(function (i) {
            if (this.checked) $(this).parent().parent().addClass("selectedrow");
        });

        initgrid();
    }

    $(document).ready(function () {
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_beginRequest(Darken);
        prm.add_endRequest(Clear);
        initgrid();
    });

    $(document).ready(function () {
        $("#commandButton").click(function () {
            var command = $("#commandSelect").val();

            switch (command) {
            case "selectAll":
                SelectAll(true);
                break;
            case "unselectAll":
                SelectAll(false);
                break;
            case "selectVisible":
                SelectVisible(true);
                break;
            case "unselectVisible":
                SelectVisible(false);
                break;
            case "deleteSelected":
                var r = confirm("<%= Resources.Resource.Admin_Catalog_Confirm%>");
                if (r) __doPostBack('<%=lbDeleteSelected.UniqueID%>', '');
                break;
            }
        });
    });
</script>
<table class="table-p">
    <tr>
        <td class="formheader" colspan="3">
            <h2>
                <%=Resources.Resource.Admin_m_Product_Photos%>
            </h2>
            <span id="fuPhotoError" style="color: Red; font-weight: bold; display: none;">
                <%=Resources.Resource.Admin_m_Product_SelectPhoto%></span>
        </td>
    </tr>
    <tr class="formheaderfooter">
        <td colspan="3"></td>
    </tr>
    <tr>
        <td style="width:75px;">
            <%=Resources.Resource.Admin_m_Product_Path%>
        </td>
        <td style="width:330px;">
            <div class="upload-container">
                <div class="upload">
                    <input type='file' name='files' id='file1' />
                    <img src="../admin/images/deletebtn.png" alt="Remove picture." />
                </div>
            </div>
        </td>
        <td style="padding-left:5px;">
            <input type="button" name="btnUploadPhoto" class="btn btn-middle btn-add"
                value="<%=Resources.Resource.Admin_m_Product_Upload%>" onclick="ajaxFileUpload()" />
        </td>
    </tr>
</table>
<br />
<table class="table-p">
    <tr style="background-color: #eff0f1;">
        <td style="vertical-align: bottom; height: 24px;" valign="bottom" colspan="2">
            <asp:Label ID="Label47" runat="server" Text="<%$ Resources:Resource, Admin_m_Product_PhotoDescription %>"
                Font-Bold="False"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="height: 68px;" valign="bottom" colspan="2">
            <asp:TextBox ID="txtPhotoDescription" runat="server" style="width:523px; height:63px; border: 1px solid #cbcbcb; border-radius: 5px; padding: 5px 5px 5px 10px;" 
                TextMode="MultiLine" CssClass="photoinput toencode"></asp:TextBox>
        </td>
    </tr>
    <tr style="height: 40px;">
        <td colspan="2">
            <asp:Label ID="Label20" runat="server" Text="<%$ Resources:Resource, Admin_m_Product_CurrentImages %>"
                EnableViewState="false"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Label ID="lPhotoMessage" runat="server" ForeColor="Red" Visible="false" EnableViewState="false"></asp:Label>
            <asp:LinkButton ID="lnkUpdatePhoto" runat="server" OnClick="lnkUpdatePhoto_Click"
                EnableViewState="false" />
            <div>
                <table style="width: 100%;" class="massaction">
                    <tr>
                        <td>
                            <span class="admin_catalog_commandBlock"><span style="display: inline-block; margin-right: 3px;">
                                <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_Command %>"></asp:Localize>
                            </span><span style="display: inline-block;">
                                <select id="commandSelect">
                                    <option value="selectAll">
                                        <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_SelectAll %>"
                                            EnableViewState="false"></asp:Localize>
                                    </option>
                                    <option value="unselectAll">
                                        <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_UnselectAll %>"
                                            EnableViewState="false"></asp:Localize>
                                    </option>
                                    <option value="selectVisible">
                                        <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_SelectVisible %>"
                                            EnableViewState="false"></asp:Localize>
                                    </option>
                                    <option value="unselectVisible">
                                        <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_UnselectVisible %>"
                                            EnableViewState="false"></asp:Localize>
                                    </option>
                                    <option value="deleteSelected">
                                        <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DeleteSelected %>"
                                            EnableViewState="false"></asp:Localize>
                                    </option>
                                </select>
                                <input id="commandButton" type="button" value="GO" style="font-size: 10px; width: 38px; height: 20px;" />
                                <asp:LinkButton ID="lbDeleteSelected" Style="display: none" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_DeleteSelected %>"
                                    OnClick="lbDeleteSelected_Click" />
                            </span><span class="selecteditems" style="vertical-align: baseline"><span style="padding: 0px 3px 0px 3px;">|</span><span id="selectedIdsCount" class="bold">0</span>&nbsp;<span><%=Resources.Resource.Admin_Catalog_ItemsSelected%></span></span></span>
                        </td>
                        <td align="right" class="selecteditems">
                            <asp:UpdatePanel ID="upCounts" runat="server">
                                <Triggers>
                                </Triggers>
                                <ContentTemplate>
                                    <%=Resources.Resource.Admin_Catalog_Total%>
                                    <span class="bold">
                                        <asp:Label ID="lblFound" CssClass="foundrecords" runat="server" Text="" /></span>&nbsp;<%=Resources.Resource.Admin_Catalog_RecordsFound%>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                        <td style="width: 8px;"></td>
                    </tr>
                </table>
                <br />
                <div>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="pageNumberer" EventName="SelectedPageChanged" />
                            <asp:AsyncPostBackTrigger ControlID="lbDeleteSelected" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnGo" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="ddRowsPerPage" EventName="SelectedIndexChanged" />
                        </Triggers>
                        <ContentTemplate>
                            <adv:AdvGridView ID="grid" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                CellPadding="2" CellSpacing="0" Confirmation="<%$ Resources:Resource, Admin_Currencies_QDelete %>"
                                CssClass="tableview" Style="cursor: pointer" DataFieldForEditURLParam="" DataFieldForImageDescription="Description"
                                EditURL="" GridLines="None" OnRowCommand="grid_RowCommand" DataFieldForImagePath="PhotoName"
                                OnSorting="grid_Sorting" OnRowDeleting="grid_RowDeleting" OnRowDataBound="grid_RowDataBound"
                                ShowFooter="false" TooltipImgCellIndex="2">
                                <Columns>
                                    <asp:TemplateField AccessibleHeaderText="ID" Visible="false">
                                        <EditItemTemplate>
                                            <asp:Label ID="Label0" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="Label01" runat="server" Text='<%# Bind("ID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-CssClass="checkboxcolumn" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="checkBoxCheckAll" CssClass="checkboxcheckall" runat="server" onclick="javascript:SelectVisible(this.checked);" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <%# ((bool)Eval("IsSelected"))? "<input type='checkbox' class='sel' checked='checked' />": "<input type='checkbox' class='sel' />"%>
                                            <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Eval("ID") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField AccessibleHeaderText="PhotoName" HeaderStyle-HorizontalAlign="Center"
                                        ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            <%= Resources.Resource.Admin_m_Product_Image %>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <img src='<%# FoldersHelper.GetImageProductPath(ProductImageType.XSmall, Eval("PhotoName").ToString(), true) %>' alt='<%# Eval("Description") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField AccessibleHeaderText="Description" HeaderStyle-HorizontalAlign="Left">
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lbDescription" runat="server" CommandName="Sort" CommandArgument="Description">
                                                <%= Resources.Resource.Admin_Product_Description %>
                                                <asp:Image ID="arrowDescription" CssClass="arrow" runat="server" ImageUrl="~/admin/images/arrowdownh.gif" />
                                            </asp:LinkButton>
                                        </HeaderTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtDescription" runat="server" Text='<%# Eval("Description") %>'
                                                Width="99%"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lCode" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField AccessibleHeaderText="ColorID">
                                        <HeaderTemplate>

                                            <asp:LinkButton ID="lbColorID" runat="server" CommandName="Sort" CommandArgument="ColorID">
                                                <%= Resources.Resource.Admin_Product_Color %>
                                                <asp:Image ID="arrowColorID" CssClass="arrow" runat="server" ImageUrl="~/admin/images/arrowdownh.gif" />
                                            </asp:LinkButton>
                                        </HeaderTemplate>
                                        <EditItemTemplate>
                                            <asp:DropDownList ID="ddlColorID" runat="server" Width="100px" DataTextField="ColorName"
                                                DataValueField="ColorID" DataSourceID="sdsColors" SelectedValue='<%# Eval("ColorID") != DBNull.Value ? Eval("ColorID") : "null" %>' />
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField AccessibleHeaderText="PhotoSortOrder">
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lbPhotoSortOrder" runat="server" CommandName="Sort" CommandArgument="PhotoSortOrder">
                                                <%= Resources.Resource.Admin_m_Product_Order %>
                                                <asp:Image ID="arrowPhotoSortOrder" CssClass="arrow" runat="server" ImageUrl="~/admin/images/arrowdownh.gif" />
                                            </asp:LinkButton>
                                        </HeaderTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="txtPhotoSortOrder" runat="server" Text='<%# Eval("PhotoSortOrder") %>'
                                                Width="99%"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lPhotoSortOrder" runat="server" Text='<%# Bind("PhotoSortOrder") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField AccessibleHeaderText="Main" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>
                                            <asp:LinkButton ID="lbMain" runat="server" CommandName="Sort" CommandArgument="Main">
                                                <%= Resources.Resource.Admin_m_Product_Default %>
                                                <asp:Image ID="arrowMain" CssClass="arrow" runat="server" ImageUrl="~/admin/images/arrowdownh.gif" />
                                            </asp:LinkButton>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="cbMain" runat="server" Checked='<%# Eval("Main")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="center" FooterStyle-HorizontalAlign="Center">
                                        <EditItemTemplate>
                                            <input id="btnUpdate" name="btnUpdate" class="updatebtn showtooltip" type="image"
                                                src="images/updatebtn.png" onclick="<%# this.Page.ClientScript.GetPostBackEventReference(grid, "Update$" + Container.DataItemIndex)%>; return false;"
                                                style="display: none" title='<%= Resources.Resource.Admin_MasterPageAdminCatalog_Update%>' />
                                            <asp:LinkButton ID="buttonDelete" runat="server"
                                                CssClass="deletebtn showtooltip valid-confirm" CommandName="Delete" CommandArgument='<%# Eval("ID")%>'
                                                data-confirm="<%$ Resources:Resource, Admin_Product_ConfirmDeletingPhoto %>" 
                                                ToolTip='<%$ Resources:Resource, Admin_MasterPageAdminCatalog_Delete %>' />
                                            <input id="btnCancel" name="btnCancel" class="cancelbtn showtooltip" type="image"
                                                src="images/cancelbtn.png" onclick="row_canceledit($(this).parent().parent()[0]); return false;"
                                                style="display: none" title="<%=Resources.Resource.Admin_MasterPageAdminCatalog_Cancel %>" />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <FooterStyle BackColor="#ccffcc" />
                                <HeaderStyle CssClass="header" />
                                <RowStyle CssClass="row1 propertiesRow_25 readonlyrow" />
                                <AlternatingRowStyle CssClass="row2 propertiesRow_25_alt readonlyrow" />
                                <EmptyDataTemplate>
                                    <center style="margin-top: 20px; margin-bottom: 20px;">
                                        <%=Resources.Resource.Admin_m_Product_NoFoto %>
                                    </center>
                                </EmptyDataTemplate>
                            </adv:AdvGridView>
                            <input type="hidden" id="SelectedIds" name="SelectedIds" />
                            <br />
                            <table class="results2">
                                <tr>
                                    <td style="width: 157px; padding-left: 6px;">
                                        <%=Resources.Resource.Admin_Catalog_ResultPerPage%>:&nbsp;<asp:DropDownList ID="ddRowsPerPage"
                                            runat="server" OnSelectedIndexChanged="btnFilter_Click" CssClass="droplist" AutoPostBack="true">
                                            <asp:ListItem>10</asp:ListItem>
                                            <asp:ListItem>20</asp:ListItem>
                                            <asp:ListItem>50</asp:ListItem>
                                            <asp:ListItem>100</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td align="center">
                                        <adv:PageNumberer CssClass="PageNumberer" ID="pageNumberer" runat="server" DisplayedPages="7"
                                            UseHref="false" UseHistory="false" OnSelectedPageChanged="pn_SelectedPageChanged" />
                                    </td>
                                    <td style="width: 157px; text-align: right; padding-right: 12px">
                                        <asp:Panel ID="goToPage" runat="server" DefaultButton="btnGo" EnableViewState="false">
                                            <span style="color: #494949">
                                                <%=Resources.Resource.Admin_Catalog_PageNum%>&nbsp;<asp:TextBox ID="txtPageNum" runat="server"
                                                    Width="30" /></span>
                                            <asp:Button ID="btnGo" runat="server" CssClass="btn" Text="<%$ Resources:Resource, Admin_Catalog_GO %>"
                                                OnClick="linkGO_Click" EnableViewState="false" />
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </td>
    </tr>
</table>

<asp:SqlDataSource runat="server" ID="sdsColors" SelectCommand="Select '0' as ColorID, '––––' as ColorName, -1000 as SortOrder 
        Union
        Select cast(Color.ColorID as nvarchar(10)), ColorName, SortOrder from Catalog.Photo inner join Catalog.Color on Color.ColorID=Photo.ColorID where objId=@productid and type='Product' 
        union 
        Select cast(Color.ColorID as nvarchar(10)), ColorName, SortOrder  From Catalog.Color inner join catalog.Offer on offer.ColorID=Color.Colorid
        where productid=@productid order by ColorName"
    OnInit="sds_Init">
    <SelectParameters>
        <asp:QueryStringParameter Name="productid" QueryStringField="ProductID" Type="Int32" DefaultValue="" />
    </SelectParameters>
</asp:SqlDataSource>

<script type="text/javascript">
    $(document).ready(function () {
        var currentImage = 1;
        $("input[name='files']").live("change", function () {
            var pathToRemoveIcon = "../admin/images/deletebtn.png";
            currentImage = currentImage + 1;
            var htmlToAppend = '<div class="upload"><input type="file" name="files" id="file' + currentImage + '" /> <img src="' + pathToRemoveIcon + '" alt="Remove picture." /></div>';
            $('.upload-container').append(htmlToAppend);
        });
        $(".upload img").live("click", function () {
            if ($(this).parent().siblings().length > 0) {
                $(this).parent().remove();
            }
        });
    });
    
    function ajaxFileUpload() {
        $("#fuPhotoError").hide();
        if ($("#fuPhoto").val() == "") {
            $("#fuPhotoError").fadeIn();
            return false;
        }

        var uplist = $("input[name=files]");
        var arrId = [];
        for (var i = 0; i < uplist.length; i++) {
            if (uplist[i].value) {
                arrId.push(uplist[i].id);
            }
        }
        
        $.ajaxFileUpload(
            {
                url: 'httphandlers/uploadphoto.ashx?ProductID=<%= Request["productid"] %>&description=' + encodeURIComponent($("#<%=txtPhotoDescription.ClientID %>").val()),
                secureuri: false,
                fileElementId: arrId,
                dataType: 'json',
                success: function (data, status) {
                    if (typeof (data.error) != 'undefined') {
                        if (data.error != '') {
                            alert(data.error);
                        } else {
                            $("div.upload").slice(1).remove();
                            <%= Page.ClientScript.GetPostBackEventReference(lnkUpdatePhoto, "") %>;
                        }
                    }
                },
                error: function (data, status, e) {
                    alert(e);
                }
            }
        );
        return false;
    }
</script>
