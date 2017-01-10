<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="Product.aspx.cs" Inherits="Admin.ProductPage" EnableViewStateMac="false" %>
<%@ Import Namespace="Resources" %>
<%@ Register Src="~/Admin/UserControls/Product/RightNavigation.ascx" TagName="RightNavigation" TagPrefix="adv" %>
<%@ Register Src="../UserControls/Catalog/SiteNavigation.ascx" TagName="SiteNavigation" TagPrefix="uc1" %>
<%@ Register Src="~/Admin/UserControls/Product/ProductProperties.ascx" TagName="ProductProperties" TagPrefix="adv" %>
<%@ Register Src="~/Admin/UserControls/Product/ProductPhotos.ascx" TagName="ProductPhotos" TagPrefix="adv" %>
<%@ Register Src="~/Admin/UserControls/Product/ProductVideos.ascx" TagName="ProductVideos" TagPrefix="adv" %>
<%@ Register Src="~/Admin/UserControls/Product/RelatedProducts.ascx" TagName="RelatedProducts" TagPrefix="adv" %>
<%@ Register Src="~/Admin/UserControls/Product/ProductCustomOption.ascx" TagName="ProductCustomOption" TagPrefix="adv" %>
<%@ Register Src="~/Admin/UserControls/Product/ProductOffers.ascx" TagName="ProductOffers" TagPrefix="adv" %>
<%@ Register Src="~/Admin/UserControls/PopupGridBrand.ascx" TagName="PopupGridBrand" TagPrefix="adv" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_Head" runat="Server">
    <title>
        <%=GetPageTitle()%></title>
    <link rel="stylesheet" href="~/Admin/css/slimbox2.css" type="text/css" media="screen"
        id="slimboxStyle" runat="server" />
    <script type="text/javascript" src="js/slimbox2.js"></script>
    <script type="text/javascript" src="js/ajaxfileupload.js"></script>
    <script type="text/javascript">

        function focusoninput(sender) {
            $(sender).parent().find("td:last input").focus();
            $(sender).parent().find("td:last textarea").focus();
        }

        function removeunloadhandler(a) {
            window.onbeforeunload = null;
        }

        function endRequest() {
            window.onbeforeunload = beforeunload;
            $(".photoinput").val("");
        }

        var skip = false;
        var dirty = false;

        function beforeunload(e) {
            if (!skip) {
                if ($("img.floppy:visible, img.exclamation:visible").length > 0) {
                    var evt = window.event || e;
                    evt.returnValue = '<%=Resources.Resource.Admin_Product_LosingChanges%>';
                }
            } else {
                skip = false;
            }
        }

        function addbeforeunloadhandler() {
            window.onbeforeunload = beforeunload;
        }

        $(document).ready(function () {
            var szCookieString = document.cookie;
            if ($.cookie("isVisibleRightPanel") != "false") {
                showRightPanel();
            }

            window.onbeforeunload = beforeunload;

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(endRequest);

            $("#<%= btnSave.ClientID %>").click(function () { skip = true; });

            window.onbeforeunload = beforeunload;
        });

        function showRightPanel() {
            document.getElementById("rightPanel").style.display = "block";
            document.getElementById("right_divHide").style.display = "block";
            document.getElementById("right_divShow").style.display = "none";
        }

        function toggleRightPanel() {
            if ($.cookie("isVisibleRightPanel") == "true") {
                $("div:.rightPanel").hide("fast");
                $("div:.right_hide_rus").hide("fast");
                $("div:.right_show_rus").show("fast");
                $("div:.right_hide_en").hide("fast");
                $("div:.right_show_en").show("fast");
                $.cookie("isVisibleRightPanel", "false", { expires: 7 });
            } else {
                $("div:.rightPanel").show("fast");
                $("div:.right_show_rus").hide("fast");
                $("div:.right_hide_rus").show("fast");
                $("div:.right_show_en").hide("fast");
                $("div:.right_hide_en").show("fast");
                $.cookie("isVisibleRightPanel", "true", { expires: 7 });
            }
        }

    </script>
    <style type="text/css">
        h2
        {
            margin: 1px 0;
            font-size: 11pt;
            font-family: Verdana, Geneva, 'DejaVu Sans' , sans-serif;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphMain" runat="Server">
    <asp:UpdateProgress runat="server" ID="uprogress">
        <ProgressTemplate>
            <div id="inprogress">
                <div id="curtain" class="opacitybackground">
                    &nbsp;
                </div>
                <div class="loader">
                    <table width="100%" style="font-weight: bold; text-align: center;">
                        <tbody>
                            <tr>
                                <td align="center">
                                    <img src="images/ajax-loader.gif" alt="" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" style="color: #0D76B8;">
                                    <asp:Localize ID="Localize_Admin_Product_PleaseWait" runat="server" Text="<%$ Resources:Resource, Admin_Product_PleaseWait %>"
                                        EnableViewState="false"></asp:Localize>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <input type="hidden" name="tabid" id="tabid" class="tabid" runat="server" />
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item selected"><a href="Catalog.aspx">
                <%= Resource.Admin_MasterPageAdmin_CategoryAndProducts %></a></li>
            <li class="neighbor-menu-item dropdown-menu-parent"><a href="ProductsOnMain.aspx?type=New">
                <%= Resource.Admin_MasterPageAdminCatalog_FirstPageProducts%></a>
                <div class="dropdown-menu-wrap">
                    <ul class="dropdown-menu">
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=New">
                            <%= Resources.Resource.Admin_MasterPageAdminCatalog_New %>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=Bestseller">
                            <%= Resources.Resource.Admin_MasterPageAdminCatalog_BestSellers %>
                        </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="ProductsOnMain.aspx?type=Discount">
                            <%= Resources.Resource.Admin_MasterPageAdminCatalog_Discount %>
                        </a></li>
                    </ul>
                </div>
            </li>
            <li class="neighbor-menu-item dropdown-menu-parent"><a href="Properties.aspx">
                <%= Resources.Resource.Admin_MasterPageAdmin_Directory%></a>
                <div class="dropdown-menu-wrap">
                    <ul class="dropdown-menu">
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Properties.aspx">
                            Свойства товаров </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Colors.aspx">
                            Справочник цветов </a></li>
                        <li class="dropdown-menu-item m-item"><a class="main-menu-subitem-lnk" href="Sizes.aspx">
                            Справочник размеров </a></li>
                    </ul>
                </div>
            </li>
            <li class="neighbor-menu-item"><a href="ExportCSV.aspx">
                <%= Resource.Admin_MasterPageAdmin_Export%></a></li>
            <li class="neighbor-menu-item"><a href="ImportCSV.aspx">
                <%= Resource.Admin_MasterPageAdmin_Import%></a></li>
            <li class="neighbor-menu-item"><a href="Brands.aspx">
                <%= Resource.Admin_MasterPageAdmin_Brands%></a></li>
            <li class="neighbor-menu-item"><a href="Reviews.aspx">
                <%= Resource.Admin_MasterPageAdmin_Reviews%></a></li>
        </menu>
    </div>
    <div class="content-own">
        <div id="notify" style="position: absolute; top: 5px; right: 5px; width: 350px;">
            </div>
        <table style="width: 100%;">
            <tr>
                <td style="width: 10px;">
                </td>
                <td style="vertical-align: top; width: 100%; padding: 0 5px 0 0;">
                    <div style="width: 800px; font-size: 0px;">
                    </div>
                    <table style="width: 100%;" cellpadding="0" cellspacing="0">
                        <tr>
                            <td width="135" rowspan="2">
                                <asp:UpdatePanel ID="upPhoto" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="productPhotos" EventName="MainPhotoUpdate" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <asp:Literal ID="ltPhoto" runat="server" Text='<%# HtmlProductImage()%>' EnableViewState="false"></asp:Literal>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                            <td valign="top" style="padding-left: 10px;">
                                <asp:Label ID="lProductName" CssClass="AdminHead" runat="server" EnableViewState="false"></asp:Label>
                                <asp:Label ID="lbIsProductActive" runat="server" EnableViewState="false"></asp:Label><br />
                                <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_Product_SubHeader %>"
                                    EnableViewState="false"></asp:Label><br />
                            </td>
                            <td align="right" valign="bottom" style="padding-bottom: 5px; width: 400px; padding-right: 3px;" rowspan="2">
                                <div style="display:inline-block">
                                    <a target="_blank" class="Link" runat="server" id="aToClient" enableviewstate="false" href="#">
                                        <%=Resource. Admin_Product_Link_ShowInAdmin %>
                                    </a>
                                    <br />
                                    <br />
                                    <div data-plugin="help" data-help-options="{position:'left bottom'}" class="help-block">
                                        <a href="javascript:void(0);" runat="server" ID="btnCopyProduct" class="btn btn-middle btn-action js-help-icon" style="margin-right:5px;"
                                                onclick="encodeFormData(); copyProduct(event);"><%=Resources.Resource.Admin_Product_CopyProduct %></a>
	                                    <article class="bubble help js-help">
		                                    <header class="help-header">
			                                    Копия товара
		                                    </header>
		                                    <div class="help-content">
			                                    Сделать полную копию товара с новым артикулом. Функция помогает ускорить наполнение каталога, создавая копии товара.
		                                    </div>
	                                    </article>
                                    </div>
                                    <asp:Button CssClass="btn btn-middle btn-add" ID="btnSave" runat="server" EnableViewState="false" 
                                        OnClick="btnSave_Click" OnClientClick="encodeFormData(); saveProperties();" onmousedown="window.onbeforeunload=null;" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td valign="bottom" style="padding-left: 10px;">
                                <span style="font-weight: bold;">
                                    <asp:Localize ID="Localize_Admin_Catalog_CategoryLocation" runat="server" Text="<%$ Resources:Resource, Admin_Catalog_CategoryLocation %>"
                                        EnableViewState="false"></asp:Localize></span><br />
                                <uc1:SiteNavigation ID="sn" runat="server" EnableViewState="false" />
                                <asp:Label ID="lMessage" runat="server" ForeColor="Red" Visible="false" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr style="height: 7px;">
                            <td colspan="3">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <table id="tabs">
                                    <tr>
                                        <td style="width: 225px;">
                                            <ul id="tabs-headers">
                                                <li id="general">
                                                    <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Resource, Admin_m_Product_Main %>"
                                                        EnableViewState="false" />
                                                    <asp:Image ID="imgExcl1" ImageUrl="images/excl.gif" runat="server" Visible="false"
                                                        CssClass="exclamation" EnableViewState="false" />
                                                    <img id="itab1floppy" name="itab1floppy" style="display: none" class="floppy" src="images/floppy.gif" />
                                                </li>
                                                <li id="cat" style="<%= (AddingNewProduct ? "display:none;": String.Empty)%>"><span>
                                                    <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Resource, Admin_Product_Categories%>"
                                                        EnableViewState="false" /></span> </li>
                                                <li id="price">
                                                    <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Resource, Admin_Product_PriceAvailability  %>"
                                                        EnableViewState="false" />
                                                    <asp:Image ID="imgExcl2" ImageUrl="images/excl.gif" runat="server" Visible="false"
                                                        CssClass="exclamation" />
                                                    <img id="itab2floppy" name="itab2floppy" style="display: none" class="floppy" src="images/floppy.gif" />
                                                </li>
                                                <li id="photos" style="<%= (AddingNewProduct ? "display:none;": String.Empty)%>"><span>
                                                    <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Resource, Admin_m_Product_Photos %>"
                                                        EnableViewState="false" /></span> </li>
                                                <li id="videos" style="<%= (AddingNewProduct ? "display:none;": String.Empty)%>"><span>
                                                    <asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Resource, Admin_m_Product_Videos %>"
                                                        EnableViewState="false" /></span> </li>
                                                <li id="descr">
                                                    <asp:Literal ID="Literal9" runat="server" Text="<%$ Resources:Resource, Admin_Product_Description %>"
                                                        EnableViewState="false" />
                                                    <input type="hidden" id="tab4floppy" name="tab4floppy" value="false" /><img id="itab4floppy"
                                                        name="itab4floppy" style="display: none" class="floppy" src="images/floppy.gif" />
                                                </li>
                                                <li id="prop" style="<%= (AddingNewProduct ? "display:none;": String.Empty)%>"><span>
                                                    <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Resource, Admin_Product_ProductProperties %>"
                                                        EnableViewState="false" /></span> </li>
                                                <li id="customopt" style="<%= (AddingNewProduct ? "display:none;": String.Empty)%>">
                                                    <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Resource, Admin_Product_CustomOptions%>"
                                                        EnableViewState="false" />
                                                    <asp:Image ID="imgExcl6" ImageUrl="images/excl.gif" runat="server" Visible="false"
                                                        CssClass="exclamation" EnableViewState="false" />
                                                    <img id="itab6floppy" name="itab6floppy" style="display: none" class="floppy" src="images/floppy.gif" />
                                                </li>
                                                <li id="seo">
                                                    <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Resource, Admin_Product_SEO  %>"
                                                        EnableViewState="false" />
                                                    <asp:Image ID="imgExcl7" ImageUrl="images/excl.gif" runat="server" Visible="false"
                                                        CssClass="exclamation" EnableViewState="false" />
                                                    <img id="itab7floppy" name="itab7floppy" style="display: none" class="floppy" src="images/floppy.gif" />
                                                </li>
                                                <li id="related" style="<%= (AddingNewProduct ? "display:none;": String.Empty)%>"><span>
                                                    <asp:Literal ID="lRelatedProduct" runat="server" Text="" EnableViewState="false" /></span>
                                                </li>
                                                <li id="alt" style="<%= (AddingNewProduct ? "display:none;": String.Empty)%>"><span>
                                                    <asp:Literal ID="lAlternativeProduct" runat="server" Text="" EnableViewState="false" /></span>
                                                </li>
                                                <asp:ListView ID="lvTabTitles" runat="server" ItemPlaceholderID="itemPlaceholderID">
                                                    <LayoutTemplate>
                                                        <li id="itemPlaceholderID" runat="server"></li>
                                                    </LayoutTemplate>
                                                    <ItemTemplate>
                                                        <li id='<%# "tabid_" + Eval("TabTitleId") %>'>
                                                            <asp:Literal ID="Literal9" runat="server" Text='<%#Eval("Title") %>' EnableViewState="false" />
                                                        </li>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                            </ul>
                                        </td>
                                        <td id="tabs-contents">
                                            <asp:Label ID="lblError" runat="server" CssClass="mProductLabelInfo" ForeColor="Red"
                                                Visible="False" Font-Names="Verdana" Font-Size="14px" EnableViewState="false"></asp:Label>
                                            <!-- Main -->
                                            <div class="tab-content">
                                                <table class="table-p">
                                                    <tr>
                                                        <td class="formheader">
                                                            <h2><%=Resources.Resource.Admin_m_Product_Main%></h2>
                                                        </td>
                                                    </tr>
                                                    <tr class="formheaderfooter">
                                                        <td>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <table class="table-p">
                                                    <tr>
                                                        <td style="width:210px;">
                                                            ID
                                                        </td>
                                                        <td style="vertical-align: middle; height: 29px;">
                                                            <asp:Label ID="lblProductId" runat="server" EnableViewState="false"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td onclick="focusoninput(this)">
                                                            <%=Resources.Resource.Admin_Product_StockNumber%>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtStockNumber" CssClass="toencode niceTextBox shortTextBoxClass" runat="server" Width="226px" />
                                                            <asp:Label runat="server" ID="lStockNumberError" ForeColor="Red" Visible="false" EnableViewState="false" />
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td onclick="focusoninput(this)">
                                                            <%=Resources.Resource.Admin_Product_Name%>
                                                            <span style="color: Red">*</span>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtName" CssClass="toencode niceTextBox shortTextBoxClass" runat="server" Width="90%" />
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtName"
                                                                EnableClientScript="false" Display="Dynamic" ValidationGroup="1" ErrorMessage='<%$ Resources:Resource,Admin_m_Product_RequiredField %>'> </asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td onclick="focusoninput(this)">
                                                            <%=Resources.Resource.Admin_m_Product_UrlSynonym%>
                                                            <span style="color:Red">*</span>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtSynonym" CssClass="toencode niceTextBox shortTextBoxClass" runat="server" Width="360px" /><br />
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtSynonym"
                                                                EnableClientScript="false" Display="Dynamic" ValidationGroup="1" ErrorMessage='<%$ Resources:Resource,Admin_m_Product_RequiredField %>'> </asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtSynonym"
                                                                EnableClientScript="false" Display="Dynamic" ValidationGroup="1" ErrorMessage='<%$ Resources:Resource,Admin_m_Category_SynonymInfo %>'
                                                                ValidationExpression="^[a-zA-Z0-9_-]*$"></asp:RegularExpressionValidator>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td>
                                                            <%=Resources.Resource.Admin_m_Product_Active%>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="chkEnabled" runat="server" Checked="true" CssClass="checkly-align" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formheader" colspan="2">
                                                            <h2><%=Resources.Resource.Admin_Product_Additional%></h2>
                                                        </td>
                                                    </tr>
                                                    <tr class="formheaderfooter">
                                                        <td colspan="2">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td onclick="focusoninput(this)">
                                                            <asp:Label ID="Label12" runat="server" Text="<%$ Resources:Resource, Admin_m_Product_ProductBrand %>"></asp:Label>
                                                        </td>
                                                        <td style="vertical-align: middle;">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:UpdatePanel runat="server">
                                                                            <Triggers>
                                                                                <asp:AsyncPostBackTrigger ControlID="popUpBrand" />
                                                                                <asp:AsyncPostBackTrigger ControlID="ibRemoveBrand" />
                                                                            </Triggers>
                                                                            <ContentTemplate>
                                                                                <asp:Label runat="server" ID="lBrand" EnableViewState="false"></asp:Label>
                                                                                <asp:ImageButton runat="server" ID="ibRemoveBrand" ImageUrl="~/Admin/images/remove.jpg"
                                                                                    Style="margin-left: 2px; margin-right: 2px;" OnClick="ibRemoveBrand_Click" EnableViewState="false" />
                                                                            </ContentTemplate>
                                                                        </asp:UpdatePanel>
                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton runat="server" OnClientClick="ShowModalPopupBrand();return false"
                                                                            Text="<%$Resources:Resource, Admin_Product_Select %>" Style="margin-left: 5px"
                                                                            EnableViewState="false"></asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td onclick="focusoninput(this)">
                                                            <%=Resources.Resource.Admin_m_Product_ProductWeight%>
                                                        </td>
                                                        <td style="vertical-align: middle; height: 29px">
                                                            <asp:TextBox ID="txtWeight" runat="server" Width="50px" Text="0" CssClass="toencode niceTextBox shortTextBoxClass" />
                                                            <%=Resources.Resource.Admin_Product_Kg%>
                                                            <asp:RangeValidator ID="RangeValidator1" runat="server" ControlToValidate="txtWeight"
                                                                ValidationGroup="1" Display="Dynamic" EnableClientScript="false" ErrorMessage='<%$ Resources:Resource,Admin_Product_EnterValidNumber %>'
                                                                MaximumValue="3000000" MinimumValue="0" Type="Double"> </asp:RangeValidator>
                                                            <asp:RequiredFieldValidator ID="RangeValidator7" runat="server" ControlToValidate="txtWeight"
                                                                ValidationGroup="1" Display="Dynamic" EnableClientScript="false" ErrorMessage='<%$ Resources:Resource,Admin_Product_EnterValidNumber %>'> </asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td onclick="focusoninput(this)">
                                                            <%=Resources.Resource.Admin_m_Product_Size%>
                                                        </td>
                                                        <td style="vertical-align: middle; height: 29px">
                                                            <asp:TextBox ID="txtSizeLength" runat="server" Width="50px" Text="0" CssClass="toencode niceTextBox shortTextBoxClass" />&nbsp;x
                                                            <asp:TextBox ID="txtSizeWidth" runat="server" Width="50px" Text="0" CssClass="toencode niceTextBox shortTextBoxClass" />&nbsp;x
                                                            <asp:TextBox ID="txtSizeHeight" runat="server" Width="50px" Text="0" CssClass="toencode niceTextBox shortTextBoxClass" />&nbsp;<span><% = Resources.Resource.Admin_Product_SizeDimension%></span>
                                                            <asp:RangeValidator ID="RangeValidator2" runat="server" ControlToValidate="txtSizeLength"
                                                                ValidationGroup="3" Display="Dynamic" EnableClientScript="true" ErrorMessage='<%$ Resources:Resource,Admin_Product_EnterValidNumber %>'
                                                                MaximumValue="3000000" MinimumValue="0" Type="Double"> </asp:RangeValidator>
                                                            <asp:RangeValidator ID="RangeValidator5" runat="server" ControlToValidate="txtSizeWidth"
                                                                ValidationGroup="3" Display="Dynamic" EnableClientScript="true" ErrorMessage='<%$ Resources:Resource,Admin_Product_EnterValidNumber %>'
                                                                MaximumValue="3000000" MinimumValue="0" Type="Double"> </asp:RangeValidator>
                                                            <asp:RangeValidator ID="RangeValidator6" runat="server" ControlToValidate="txtSizeHeight"
                                                                ValidationGroup="3" Display="Dynamic" EnableClientScript="true" ErrorMessage='<%$ Resources:Resource,Admin_Product_EnterValidNumber %>'
                                                                MaximumValue="3000000" MinimumValue="0" Type="Double"> </asp:RangeValidator>
                                                            <asp:Label ID="lSize" runat="server" Visible="false" Text="Error size" EnableViewState="false"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr class="formheaderfooter">
                                                        <td colspan="2">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formheader" colspan="2">
                                                            <h2><%=Resources.Resource.Admin_Product_Markers%></h2>
                                                        </td>
                                                    </tr>
                                                    <tr class="formheaderfooter">
                                                        <td colspan="2">
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td>
                                                            <%=Resources.Resource.Admin_Product_BestSeller%>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="chkBestseller" runat="server" CssClass="checkly-align" />
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td>
                                                            <%=Resources.Resource.Admin_Product_Recomended%>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="chkRecommended" runat="server" CssClass="checkly-align" />
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td>
                                                            <%=Resources.Resource.Admin_Product_New%>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="chkNew" runat="server" CssClass="checkly-align" />
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td>
                                                            <%=Resources.Resource.Admin_Product_OnSale%>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox ID="chkOnSale" runat="server" CssClass="checkly-align" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:Label runat="server" ID="lblMarkersDisabled" Text="<%$ Resources:Resource, Admin_Product_CanNotChange %>"
                                                                EnableViewState="false"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <!-- Categories -->
                                            <div class="tab-content">
                                                <asp:UpdatePanel ID="UpdatePanel8" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="lnAddLink" EventName="Click" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnDelLink" EventName="Click" />
                                                        <asp:AsyncPostBackTrigger ControlID="btnMainLink" EventName="Click" />
                                                        <asp:AsyncPostBackTrigger ControlID="LinksProductTree" EventName="TreeNodePopulate" />
                                                    </Triggers>
                                                    <ContentTemplate>
                                                        <table class="table-p">
                                                            <tr>
                                                                <td colspan="3" class="formheader">
                                                                    <h2>
                                                                        <asp:Localize ID="Localize_Admin_Product_CategoriesContainsProducts" runat="server"
                                                                            Text="<%$ Resources:Resource, Admin_Product_CategoriesContainsProducts %>" EnableViewState="false"></asp:Localize></h2>
                                                                </td>
                                                            </tr>
                                                            <tr class="formheaderfooter">
                                                                <td colspan="3">
                                                                </td>
                                                            </tr>
                                                        </table> 
                                                        <table class="table-p" style="table-layout: fixed;">
                                                            <tr>
                                                                <td style="vertical-align:top;">
                                                                    <div style="min-width: 285px; height: 300px; overflow: scroll; background-color: White; margin-top:10px; margin-bottom:10px; border:1px solid gray; border-radius:3px;" id="divScroll">
                                                                        <asp:TreeView ID="LinksProductTree" ForeColor="Black" SelectedNodeStyle-BackColor="Blue"
                                                                            EnableClientScript="true" PopulateNodesFromClient="true" runat="server" ShowLines="True"
                                                                            ExpandImageUrl="images/loading.gif" BackColor="White" OnTreeNodePopulate="PopulateNode"
                                                                            EnableViewState="true">
                                                                            <SelectedNodeStyle BackColor="Yellow" CssClass="selectedNodeClass" />
                                                                        </asp:TreeView>
                                                                    </div>
                                                                </td>
                                                                <td style="width:80px; text-align:center; padding-bottom:45px;">
                                                                    <asp:Button ID="lnAddLink" runat="server" Text=">>>" OnClick="lnAddLink_Click" EnableViewState="false"
                                                                        CssClass="btn btn-middle btn-add" style="padding-left: 15px; padding-right: 15px;" />
                                                                    <br />
                                                                    <br />
                                                                    <br />
                                                                    <asp:Button ID="btnDelLink" runat="server" Text="<<<" OnClick="btnDelLink_Click" EnableViewState="false"
                                                                        CssClass="btn btn-middle btn-action" style="padding-left: 15px; padding-right: 15px;" />
                                                                </td>
                                                                <td style="vertical-align:top;">
                                                                    <div style="min-width: 285px; height: 300px; overflow-x: auto; margin-top:10px; border:1px solid gray; border-radius:3px;">
                                                                        <asp:ListBox ID="ListlinkBox" runat="server" Height="283px" CssClass="TableContainer">
                                                                        </asp:ListBox>
                                                                    </div>
                                                                    <div style="margin-top:10px; text-align: right;">
                                                                        <asp:Button ID="btnMainLink" runat="server" CssClass="btn btn-middle btn-add"
                                                                            Text="<%$ Resources: Resource, Admin_Product_MainLink %>" OnClick="btnMainLink_Click" EnableViewState="false" />
                                                                        <div data-plugin="help" class="help-block">
                                                                            <div class="help-icon js-help-icon"></div>
                                                                            <article class="bubble help js-help">
                                                                                <header class="help-header">
                                                                                    Сделать категорию основной
                                                                                </header>
                                                                                <div class="help-content">
                                                                                    Выделите категорию и нажмите "сделать основной", если нужно чтобы навигация ("Хлебные крошки"), строились по этой категории.<br />
                                                                                    <br />
                                                                                    При переходе в карточку товара, навигация будет построена от основной категории.
                                                                                </div>
                                                                            </article>
                                                                        </div>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                        <script type="text/javascript">
                                                            var scrollTopcategories = 0, timerScroll;

                                                            var observeScroll = function () {
                                                                $("#divScroll").on('scroll', function () {

                                                                    var blockWithScroll = $(this);

                                                                    if (timerScroll != null) {
                                                                        clearTimeout(timerScroll);
                                                                    }

                                                                    timerScroll = setTimeout(function () {
                                                                        scrollTopcategories = blockWithScroll.scrollTop();
                                                                    }, 300);

                                                                });
                                                            };


                                                            observeScroll();

                                                            Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
                                                                $("#divScroll").scrollTop(scrollTopcategories);

                                                                observeScroll();
                                                            });
                                                        </script>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                            <!-- Prices -->
                                            <div class="tab-content">
                                                <table class="table-p">
                                                    <tr>
                                                        <td class="formheader" colspan="2">
                                                            <h2><%=Resources.Resource.Admin_Product_PriceAvailability%></h2>
                                                        </td>
                                                    </tr>
                                                    <tr class="formheaderfooter">
                                                        <td colspan="2">
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <adv:ProductOffers ID="productOffers" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="width: 30%; height: 26px;">
                                                        </td>
                                                        <td style="vertical-align: middle; height: 26px;">
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formheader" colspan="2">
                                                            <h2><%=Resources.Resource.Admin_Product_Other%></h2>
                                                        </td>
                                                    </tr>
                                                    <tr class="formheaderfooter">
                                                        <td colspan="2">
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td onclick="focusoninput(this)">
                                                            <%=Resources.Resource.Admin_m_Product_DeliveryCost%>
                                                        </td>
                                                        <td>
                                                            &nbsp;<asp:TextBox ID="txtShippingPrice" runat="server" Text="0" Width="88px" CssClass="niceTextBox shortTextBoxClass2" />
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Стоимость доставки единицы товара
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Данный параметр используется для расчета стоимости доставки в методе "Доставка в зависимости от стоимости доставки товара"<br />
                                                                        <br />
                                                                        Так же, этот параметр выгружается в Яндекс.Маркет в поле стоимость доставки товара (тэг local_delivery_cost).<br />
                                                                        <br />
                                                                        Если параметр не нужен, укажите значение 0.
                                                                    </div>
                                                                </article>
                                                            </div>
                                                            <asp:RangeValidator ID="rvShippingPrice" runat="server" ControlToValidate="txtShippingPrice"
                                                                ValidationGroup="2" Display="Dynamic" EnableClientScript="true" ErrorMessage='<%$ Resources:Resource,Admin_Product_EnterValidNumber %>'
                                                                MaximumValue="1000000000" MinimumValue="0" Type="Double"> </asp:RangeValidator>
                                                            <asp:RequiredFieldValidator ID="RangeValidator11" runat="server" ControlToValidate="txtShippingPrice"
                                                                ValidationGroup="1" Display="Dynamic" EnableClientScript="true" ErrorMessage='<%$ Resources:Resource,Admin_Product_EnterValidNumber %>' />
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td onclick="focusoninput(this)">
                                                            <%=Resources.Resource.Admin_m_Product_Discount%>
                                                        </td>
                                                        <td>
                                                            &nbsp;<asp:TextBox ID="txtDiscount" runat="server" Text="0" Width="88px" CssClass="niceTextBox shortTextBoxClass2"/>&nbsp;%
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Скидка
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Скидка на цену товара, указывается только на этот товар, на всё вариации.<br />
                                                                        <br />
                                                                        Скидка указывается в процентах.
                                                                    </div>
                                                                </article>
                                                            </div>
                                                            <asp:RangeValidator ID="RangeValidator3" runat="server" ControlToValidate="txtDiscount"
                                                                ValidationGroup="2" Display="Dynamic" EnableClientScript="true" ErrorMessage='<%$ Resources: Resource, Admin_Product_EnterValidNumber  %>'
                                                                MaximumValue="100" MinimumValue="0" Type="Double"> </asp:RangeValidator>
                                                            <asp:RequiredFieldValidator ID="RangeValidator8" runat="server" ControlToValidate="txtDiscount"
                                                                ValidationGroup="2" Display="Dynamic" EnableClientScript="true" ErrorMessage='<%$ Resources: Resource, Admin_Product_EnterValidNumber  %>'>
                                                            </asp:RequiredFieldValidator>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td onclick="focusoninput(this)">
                                                            <%=Resources.Resource.Admin_Product_Unit%>
                                                        </td>
                                                        <td>
                                                            &nbsp;<asp:TextBox ID="txtUnit" runat="server" Width="88px" CssClass="niceTextBox shortTextBoxClass2"/>
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Единицы измерения
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Единицы измерения товара.<br />
                                                                        <br />
                                                                        Например: <br />
                                                                        штуки (шт.), литры (л.), упаковка (упак.) и т.д.
                                                                    </div>
                                                                </article>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td>
                                                            <%=Resources.Resource.Admin_m_Product_OrderByRequest%>
                                                        </td>
                                                        <td>
                                                            &nbsp;<asp:CheckBox ID="chkAllowPreOrder" runat="server" Checked="false" CssClass="checkly-align" />
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Под заказ
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Опция определяет, если товара нет в наличии, показывать ли в этом случае кнопку "Под заказ".
                                                                    </div>
                                                                </article>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td style="width: 30%; height: 29px; vertical-align: middle;">
                                                            <asp:Label ID="Label15" runat="server" Text="<%$ Resources:Resource, Admin_m_Product_AmountLimitation %>"
                                                                EnableViewState="false"></asp:Label>
                                                        </td>
                                                        <td style="vertical-align: middle; height: 29px">
                                                            &nbsp;<asp:TextBox ID="txtMinAmount" runat="server" CssClass="niceTextBox shortTextBoxClass3"/>
                                                            <asp:RangeValidator ID="RangeValidator13" runat="server" ControlToValidate="txtMinAmount"
                                                                ValidationGroup="2" Display="Dynamic" EnableClientScript="false" ErrorMessage='<%$ Resources: Resource, Admin_Product_EnterValidNumber %>'
                                                                MaximumValue="100000" MinimumValue="0" Type="Double" />
                                                            -
                                                            <asp:TextBox ID="txtMaxAmount" runat="server" CssClass="niceTextBox shortTextBoxClass3"/>
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Минимальное и максимальное количество 
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Определяет минимальное и максимальное возможное количество товара в заказе. <br />
                                                                        <br />
                                                                        Например: 2-10. Означает, что купить можно минимум 2 штуки, но не более 10 за 1 заказ.<br />
                                                                        <br />
                                                                        Если ограничение не нужно, например, на верхнюю границу, оставьте поле пустым.
                                                                    </div>
                                                                </article>
                                                            </div>
                                                            <asp:RangeValidator ID="RangeValidator14" runat="server" ControlToValidate="txtMaxAmount"
                                                                ValidationGroup="2" Display="Dynamic" EnableClientScript="false" ErrorMessage='<%$ Resources: Resource, Admin_Product_EnterValidNumber %>'
                                                                MaximumValue="100000" MinimumValue="0" Type="Double" />
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td style="width: 30%; height: 29px; vertical-align: middle;">
                                                            <asp:Label ID="Label16" runat="server" Text="<%$ Resources:Resource, Admin_m_Product_AmountMultiplicity%>"
                                                                EnableViewState="false"></asp:Label>
                                                        </td>
                                                        <td style="vertical-align: middle; height: 29px">
                                                            &nbsp;<asp:TextBox ID="txtMultiplicity" runat="server" Text="1" Width="88px" CssClass="niceTextBox shortTextBoxClass2" />
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Кратность количества
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Важная опция, если товар можно купить порциями, которые меньше единицы.<br /><br />
                                                                        Например: Метры, вы можете указать кратность, 0.2, если продаете, например ткань, кусочками по 20 см.
                                                                    </div>
                                                                </article>
                                                            </div>
                                                            <asp:RangeValidator ID="RangeValidator15" runat="server" ControlToValidate="txtMultiplicity"
                                                                ValidationGroup="2" Display="Dynamic" EnableClientScript="true" ErrorMessage='<%$ Resources: Resource, Admin_Product_EnterValidNumber %>'
                                                                MaximumValue="100000" MinimumValue="0" Type="Double" />
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtMultiplicity"
                                                                ValidationGroup="2" Display="Dynamic" EnableClientScript="true" ErrorMessage='<%$ Resources: Resource, Admin_Product_EnterValidNumber %>' />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formheader" colspan="2">
                                                            <h2><%=Resource.Admin_Product_Export%></h2>
                                                        </td>
                                                    </tr>
                                                    <tr class="formheaderfooter">
                                                        <td colspan="2">
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td style="width: 30%; height: 29px; vertical-align: middle;">
                                                            <asp:Label ID="Label18" runat="server" Text='<%$ Resources: Resource, Admin_Product_Yandex_SalesNote %>'
                                                                EnableViewState="false"></asp:Label>
                                                        </td>
                                                        <td style="vertical-align: middle; height: 29px">
                                                            &nbsp;<asp:TextBox ID="txtSalesNote" runat="server" Text="" CssClass="niceTextBox textBoxClass" />
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Заметки для продажи
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Элемент "sales_notes", используется только для Яндекс.Маркета<br />
                                                                        <br />
                                                                        Элемент используется для отражения информации о минимальной сумме заказа, минимальной партии товара или необходимости предоплаты, а также для описания акций, скидок и распродаж.<br />
                                                                        <br />
                                                                        Допустимая длина текста в элементе — 50 символов.<br />
                                                                        <br />
                                                                        Необязательный элемент.<br />
                                                                        <br />
                                                                        Например: Необходима предоплата.
                                                                    </div>
                                                                </article>
                                                            </div>
                                                            <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtSalesNote"
                                                                ID="RegularExpressionValidator2" ValidationExpression="^[\s\S]{0,50}$" runat="server"
                                                                ErrorMessage='<%$ Resources: Resource, Admin_Product_Yandex_MaxSalesNote %>'></asp:RegularExpressionValidator>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td style="width: 30%; height: 29px; vertical-align: middle;">
                                                            <asp:Label ID="Label8" runat="server" Text='<%$ Resources: Resource, Admin_Product_Gtin %>'
                                                                EnableViewState="false"></asp:Label>
                                                        </td>
                                                        <td style="vertical-align: middle; height: 29px">
                                                            &nbsp;<asp:TextBox ID="txtGtin" runat="server" Text="" CssClass="niceTextBox textBoxClass" />
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Google GTIN
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Используется только для Google Merchant Center<br />
                                                                        <br />
                                                                        Подробнее: <br /><a href="https://support.google.com/merchants/answer/160161?hl=ru#valid" target="_blank">
                                                                        Google GTIN на support.google.com</a>
                                                                    </div>
                                                                </article>
                                                            </div>
                                                            <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtGtin" ID="RegularExpressionValidator3"
                                                                ValidationExpression="^[\s\S]{0,50}$" runat="server" ErrorMessage='<%$ Resources: Resource, Admin_Product_Yandex_MaxSalesNote %>'></asp:RegularExpressionValidator>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td style="width: 30%; height: 29px; vertical-align: middle;">
                                                            <asp:Label ID="Label9" runat="server" Text='<%$ Resources: Resource, Admin_Product_GoogleBase_GoogleProductCategory %>'
                                                                EnableViewState="false"></asp:Label>
                                                        </td>
                                                        <td style="vertical-align: middle; height: 29px">
                                                            &nbsp;<asp:TextBox ID="txtGoogleProductCategory" runat="server" Text="" CssClass="niceTextBox textBoxClass" />
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Классификация товаров
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Этот атрибут определяет категорию товара в соответствии с классификацией Google.<br />
                                                                        <br />
                                                                        Используется только для Google Merchant Center<br />
                                                                        <br />
                                                                        Подробнее: <br /><a href="https://support.google.com/merchants/answer/160081?hl=ru" target="_blank">
                                                                        Google Product Category на support.google.com</a>
                                                                    </div>
                                                                </article>
                                                            </div>
                                                            <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtGoogleProductCategory"
                                                                ID="RegularExpressionValidator4" ValidationExpression="^[\s\S]{0,500}$" runat="server"
                                                                ErrorMessage='<%$ Resources: Resource, Admin_Product_Yandex_MaxSalesNote %>'></asp:RegularExpressionValidator>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td style="width: 30%; height: 29px; vertical-align: middle;">
                                                            <asp:Label ID="Label20" runat="server" Text='<%$ Resources: Resource, Admin_Product_Yandex_Adult %>'
                                                                EnableViewState="false"></asp:Label>
                                                        </td>
                                                        <td style="vertical-align: middle; height: 29px">
                                                            &nbsp;<asp:CheckBox runat="server" ID="chbAdult" CssClass="checkly-align"/>
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Товар для взрослых
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Элемент "adult"<br />
                                                                        <br />
                                                                        Элемент обязателен для обозначения товара, имеющего отношение к удовлетворению сексуальных потребностей, либо иным образом эксплуатирующего интерес к сексу.<br />
                                                                        <br />
                                                                        Необязательный элемент.<br />
                                                                        <br />
                                                                        Используется как для Яндекс.Маркет, так и Google Merchant Center.
                                                                    </div>
                                                                </article>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr class="rowsPost row-interactive">
                                                        <td style="width: 30%; height: 29px; vertical-align: middle;">
                                                            <asp:Label ID="Label21" runat="server" Text='<%$ Resources: Resource, Admin_Product_Yandex_ManufacturerWarranty %>'
                                                                EnableViewState="false"></asp:Label>
                                                        </td>
                                                        <td style="vertical-align: middle; height: 29px">
                                                            &nbsp;<asp:CheckBox runat="server" ID="chbManufacturerWarranty" CssClass="checkly-align"/>
                                                            <div data-plugin="help" class="help-block">
                                                                <div class="help-icon js-help-icon"></div>
                                                                <article class="bubble help js-help">
                                                                    <header class="help-header">
                                                                        Гарантия производителя
                                                                    </header>
                                                                    <div class="help-content">
                                                                        Элемент "manufacturer_warranty"<br />
                                                                        <br />
                                                                        Элемент предназначен для отметки товаров, имеющих официальную гарантию производителя.<br />
                                                                        <br />
                                                                        Необязательный элемент.<br />
                                                                    </div>
                                                                </article>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <!-- Photos -->
                                            <div class="tab-content">
                                                <adv:ProductPhotos runat="server" ID="productPhotos" OnMainPhotoUpdate="productPhotos_OnMainPhotoUpdate"
                                                    OnPhotoMessage="productPhotos_OnPhotoMessage" />
                                            </div>
                                            <!-- Videos -->
                                            <div class="tab-content">
                                                <adv:ProductVideos runat="server" ID="productVideos" />
                                            </div>
                                            <!-- Description -->
                                            <div class="tab-content">
                                                <table class="table-p">
                                                    <tbody>
                                                        <tr>
                                                            <td class="formheader">
                                                                <h2>
                                                                    <%=Resources.Resource.Admin_Product_Description%></h2>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <CKEditor:CKEditorControl ID="fckDescription" runat="server" BasePath="~/ckeditor/"
                                                                    Height="400px" Width="100%" EnableViewState="False" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formheader">
                                                                <h2>
                                                                    <%=Resources.Resource.Admin_Product_ShortDescription%></h2>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <CKEditor:CKEditorControl runat="server" ID="fckBriefDescription" BasePath="~/ckeditor/"
                                                                    Height="400px" Width="100%" EnableViewState="false" />
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                            <!-- Properties -->
                                            <div class="tab-content">
                                                <adv:ProductProperties ID="productProperties" runat="server" />
                                            </div>
                                            <!-- Custom options -->
                                            <div class="tab-content">
                                                <adv:ProductCustomOption ID="productCustomOption" runat="server" />
                                            </div>
                                            <!-- Meta -->
                                            <div class="tab-content">
                                                <asp:Label ID="lResult" runat="server" CssClass="mProductLabelInfo" ForeColor="Blue"
                                                    Text="<%$ Resources:Resource, Admin_m_Product_Saved %>" Visible="false" EnableViewState="false"></asp:Label>
                                                <table class="table-p">
                                                    <tbody>
                                                        <tr class="rowPost">
                                                            <td class="formheader" colspan="2">
                                                                <h2><%=Resources.Resource.Admin_Product_SEO%></h2>
                                                            </td>
                                                        </tr>
                                                        <tr class="formheaderfooter">
                                                            <td >
                                                            </td>
                                                        </tr>
                                                        <tr class="rowsPost row-interactive">
                                                            <td style="width:220px;">
                                                                <%=Resources.Resource.Admin_Catalog_UseDefaultMeta%>
                                                            </td>
                                                            <td>
                                                                <asp:CheckBox runat="server" ID="chbDefaultMeta" Checked="true" CssClass="checkly-align" />
                                                                <div data-plugin="help" class="help-block">
                                                                    <div class="help-icon js-help-icon"></div>
                                                                    <article class="bubble help js-help">
                                                                        <header class="help-header">
                                                                            Использовать Meta по умолчанию
                                                                        </header>
                                                                        <div class="help-content">
                                                                            Если опция <b>включена</b>, SEO настройки будут взяты из общих настроек магазина.
                                                                            <br />
                                                                            <br />
                                                                            Если опиця <b>выключена</b>, SEO настроки для товара будут взяты с формы ниже.
                                                                        </div>
                                                                    </article>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr class="rowsPost row-interactive">
                                                            <td onclick="focusoninput(this)">
                                                                <%=Resources.Resource.Admin_m_Product_HeadTitle%>
                                                            </td>
                                                            <td style="vertical-align: middle; height: 26px;">
                                                                <asp:TextBox ID="txtTitle" runat="server" CssClass="niceTextBox textBoxClass"/>
                                                            </td>
                                                        </tr>
                                                        <tr class="rowsPost row-interactive">
                                                            <td onclick="focusoninput(this)">
                                                                H1
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtH1" runat="server" CssClass="niceTextBox textBoxClass"/>
                                                            </td>
                                                        </tr>
                                                        <tr class="rowsPost row-interactive">
                                                            <td onclick="focusoninput(this)">
                                                                <%=Resources.Resource.Admin_m_Product_MetaKeywords%>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtMetaKeywords" runat="server" CssClass="niceTextBox textArea2Lines" TextMode="MultiLine"/>
                                                            </td>
                                                        </tr>
                                                        <tr class="rowsPost row-interactive">
                                                            <td onclick="focusoninput(this)">
                                                                <%=Resources.Resource.Admin_m_Product_MetaDescription%>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtMetaDescription" runat="server" CssClass="niceTextBox textArea2Lines" TextMode="MultiLine"/>
                                                            </td>
                                                        </tr>
                                                        <tr class="rowsPost">
                                                            <td></td>
                                                            <td>
                                                                <span class="subSaveNotify">
                                                                    <%=Resources.Resource.Admin_m_Product_UseGlobalVariables2%>
                                                                </span>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <div style="display:none;">
                                                                    <asp:Label CssClass="PaymentMethod_description" ID="Label37" runat="server" Text="<%$ Resources:Resource, Admin_m_Product_UseGlobalVariables %>"></asp:Label>
                                                                </div> 
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2">
                                                                <div class="dvSubHelp">
                                                                    <asp:Image ID="Image6" runat="server" ImageUrl="~/Admin/images/messagebox_help.png" AlternateText="" />
                                                                    <a href="http://www.advantshop.net/help/pages/settings_of_seo_module" target="_blank">Инструкция. Настройка мета по умолчанию для магазина</a>
                                                                </div>       
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                            <!-- Alternative -->
                                            <div class="tab-content">
                                                <adv:RelatedProducts runat="server" ID="relatedProducts" RelatedType="0"></adv:RelatedProducts>
                                            </div>
                                            <div class="tab-content">
                                                <adv:RelatedProducts runat="server" ID="alternativeProducts" RelatedType="1"></adv:RelatedProducts>
                                            </div>
                                            <!-- TabModules -->
                                            <asp:ListView ID="lvTabBodies" runat="server" ItemPlaceholderID="itemPlaceholderID">
                                                <LayoutTemplate>
                                                    <div id="itemPlaceholderID" runat="server">
                                                    </div>
                                                </LayoutTemplate>
                                                <ItemTemplate>
                                                    <div class="tab-content">
                                                        <asp:HiddenField ID="hfTabTitleId" runat="server" Value='<%#Eval("TabTitleId") %>' />
                                                        <table class="table-p">
                                                            <tbody>
                                                                <tr>
                                                                    <td class="formheader">
                                                                        <h2>
                                                                            <%# Eval("Title") %></h2>
                                                                        <br />
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <CKEditor:CKEditorControl ID="fckTabBody" runat="server" BasePath="~/ckeditor/" Height="400px"
                                                                            Width="100%" EnableViewState="False" Text='<%# Eval("Body") %>' />
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:ListView>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
                <%=AdvantShop.Helpers.HtmlHelper.RenderSplitter()%>
                <td class="rightNavigation">
                    <div id="rightPanel" class="rightPanel">
                        <adv:RightNavigation runat="server" ID="rightNavigation"></adv:RightNavigation>
                    </div>
                </td>
                <td style="width: 10px;">
                </td>
            </tr>
        </table>
    </div>
    <adv:PopupGridBrand ID="popUpBrand" runat="server"></adv:PopupGridBrand>
    <script type="text/javascript">

        function setupTooltips() {
            $(".showtooltip").tooltip({
                showURL: false
            });

            $(".imgtooltip[abbr]").tooltip({
                delay: 10,
                showURL: false,
                bodyHandler: function () {
                    return $("<img/>").attr("src", $(this).attr("abbr"));
                }
            });
        }

        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () { setupTooltips(); });

        function fillUrl() {
            var text = $('#<%=txtName.ClientID %>').val();
            var url = $('#<%=txtSynonym.ClientID %>').val();
            if ((text != "") & (url == "")) {
                $('#<%=txtSynonym.ClientID %>').val(translite(text));
            }
        }

        $(function () {
            if ($('#<%= chbDefaultMeta.ClientID %>').is(":checked")) {
                $('#<%=txtTitle.ClientID %>').attr("disabled", "disabled");
                $('#<%=txtH1.ClientID %>').attr("disabled", "disabled");
                $('#<%=txtMetaDescription.ClientID %>').attr("disabled", "disabled");
                $('#<%=txtMetaKeywords.ClientID %>').attr("disabled", "disabled");
            }
        });

        $('#<%= chbDefaultMeta.ClientID %>').click(function () {
            if ($('#<%= chbDefaultMeta.ClientID %>').is(":checked")) {
                $('#<%=txtTitle.ClientID %>').val("");
                $('#<%=txtH1.ClientID %>').val("");
                $('#<%=txtMetaDescription.ClientID %>').val("");
                $('#<%=txtMetaKeywords.ClientID %>').val("");

                $('#<%=txtTitle.ClientID %>').attr("disabled", "disabled");
                $('#<%=txtH1.ClientID %>').attr("disabled", "disabled");
                $('#<%=txtMetaDescription.ClientID %>').attr("disabled", "disabled");
                $('#<%=txtMetaKeywords.ClientID %>').attr("disabled", "disabled");

            } else {
                $('#<%=txtTitle.ClientID %>').removeAttr("disabled");
                $('#<%=txtH1.ClientID %>').removeAttr("disabled");
                $('#<%=txtMetaDescription.ClientID %>').removeAttr("disabled");
                $('#<%=txtMetaKeywords.ClientID %>').removeAttr("disabled");
            }
        });


        $('#<%=txtSynonym.ClientID %>').focus(fillUrl);
    </script>
</asp:Content>
