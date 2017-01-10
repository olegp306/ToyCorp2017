<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ModulesManager.aspx.cs" Inherits="Admin.ModulesManager"
    MasterPageFile="MasterPageAdmin.master" %>

<%@ Import Namespace="AdvantShop.Trial" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="AdvantShop" %>
<asp:Content ID="contentMain" runat="server" ContentPlaceHolderID="cphMain">
    <div id="inprogress" style="display: none">
        <div id="curtain" class="opacitybackground">
            &nbsp;
        </div>
        <div class="loader">
            <table width="100%" style="font-weight: bold; text-align: center;">
                <tbody>
                    <tr>
                        <td align="center">
                            <img src="images/ajax-loader.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="color: #0D76B8;">
                            <asp:Localize ID="Localize_Admin_Properties_PleaseWait" runat="server" Text="<%$ Resources:Resource, Admin_Properties_PleaseWait %>"></asp:Localize>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="content-own">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tbody>
                <tr>
                    <td style="width: 72px;">
                        <img src="images/orders_ico.gif" alt="" />
                    </td>
                    <td>
                        <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_ModuleManager_Header %>"></asp:Label><br />
                        <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_ModuleManager_SubHeader %>"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="lTrialMode" runat="server" Text="<%$ Resources:Resource, Admin_Module_TrialMode %>" ForeColor="Red"></asp:Label>
                    </td>
                    <td style="vertical-align: bottom; padding-right: 10px">
                        <div style="float: right; padding-right: 10px">
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        <br />
        <script type="text/javascript">
            function progressShow() {
                document.getElementById('inprogress').style.display = 'block';
            }
            function progressHide() {
                document.getElementById('inprogress').style.display = 'none';
            }
        </script>
        <%--<iframe src="ModulesManagerInside.aspx" frameborder="0" height="690" width="100%" hspace="0" scrolling="auto"></iframe>--%>
        <%--        <asp:ScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1"
            ScriptMode="Release">
        </asp:ScriptManager>--%>
        <div style="height: 690px; width: 100%; overflow: auto;">
            <asp:ListView runat="server" ID="lvModulesManager" ItemPlaceholderID="pl" OnItemCommand="lvModules_ItemCommand"
                Visible="True">
                <LayoutTemplate>
                    <div>
                        <ul class="modules-list">
                            <asp:PlaceHolder runat="server" ID="pl" />
                        </ul>
                    </div>
                </LayoutTemplate>
                <ItemTemplate>
                    <li class="modules-item-item">
                        <div class="modules-item-pic-cell">
                            <div class="modules-item-pic-wrap">
                                <a class="modules-item-pic-lnk inset-shadow" href="javascript:void(0);" onclick="<%# !string.IsNullOrEmpty(Convert.ToString(Eval("DetailsLink"))) ? "parent.open('"+ Eval("DetailsLink") +"', 'AdvantshopDetails')"  : "return false;" %>">
                                    <img alt="<%# Eval("Name") %>" src="<%# !string.IsNullOrEmpty(Convert.ToString(Eval("Icon"))) ? Eval("Icon").ToString() : "images/new_admin/modules/nophoto.jpg" %>"
                                        class="modules-item-pic">
                                </a>
                            </div>
                            <div id="Div1" runat="server" visible='<%# !string.IsNullOrEmpty(Convert.ToString(Eval("DetailsLink"))) %>'
                                class="modules-item-more">
                                <a href="javascript:void(0);" onclick="<%#!string.IsNullOrEmpty(Convert.ToString(Eval("DetailsLink"))) ? "parent.open('"+ Eval("DetailsLink") +"', 'AdvantshopDetails')" : "return false;" %>"
                                    class="modules-item-more-lnk">
                                    <%= Resource.Admin_Module_More %></a></div>
                        </div>
                        <div class="modules-item-info">
                            <div class="modules-item-title">
                                <a class="modules-item-title-lnk" target="_blank" href="<%# !string.IsNullOrEmpty(Convert.ToString(Eval("DetailsLink"))) ? Eval("DetailsLink") : "javascript:void(0);" %>">
                                    <%# Eval("Name") %>
                                </a>
                                <div style="color: #3F3F3F; font-size: 14px; font-weight: normal; margin-top: 10px;"
                                    runat="server" visible='<%# Convert.ToBoolean(Eval("IsInstall")) %>'>
                                    <asp:Label ID="lblActiveModuleInfo" runat="server" Text="<%$ Resources:Resource, Admin_Module_ModuleActive %>"></asp:Label>&nbsp;
                                    <input type="checkbox" id="ckbActiveModule" runat="server" checked='<%# Convert.ToBoolean( Eval("Enabled")) %>'
                                        class="ckbActiveModule" data-modulestringid='<%# Eval("StringId") %>' />
                                </div>
                                <asp:HiddenField ID="hfLastVersion" runat="server" Value='<%# Eval("Version") %>' />
                                <asp:HiddenField ID="hfId" runat="server" Value='<%# Eval("Id") %>' />
                            </div>
                            <div class="modules-item-descr">
                                <%# Eval("BriefDescription") %>
                            </div>
                            <div class="justify">
                                <div class="justify-item">
                                    <div id="Div2" runat="server" visible='<%# !Convert.ToBoolean(Eval("IsInstall")) %>'
                                        class="modules-item-module-price">
                                        <%#  Convert.ToDecimal(Eval("Price")) != 0 ? String.Format("{0:##,##0.##}", Eval("Price")) + " " + Eval("Currency") : Resource.Admin_Modules_FreeCost%></div>
                                    <a id="A1" class="btn btn-middle btn-action" onclick='<%# "location.assign(\"" + "Module.aspx?module=" + Eval("StringId") + "\")" %>'
                                        runat="server" href="javascript:void(0)" visible='<%#Convert.ToBoolean(Eval("HasSettings")) && Convert.ToBoolean(Eval("IsInstall")) %>'>
                                        <%= Resource.Admin_ModulesManager_Settings %></a>
                                </div>
                                <div class="justify-item">
                                    <input type="button" onclick='<%# "progressShow();" + string.Format("installModule(\"{0}\",\"{1}\",\"{2}\")", Eval("StringId"), Eval("Id"),Eval("Version") ) %>'
                                        runat="server" id="btnInstall" class="btn btn-middle btn-submit" value='<%$ Resources:Resource, Admin_ModulesManager_Install %>'
                                        visible='<%#!TrialService.IsTrialEnabled && !Convert.ToBoolean(Eval("IsInstall")) && Convert.ToBoolean(Eval("Active")) %>' />
                                    <a id="A2" runat="server" visible='<%# Convert.ToBoolean(Eval("IsInstall")) && !(Convert.ToBoolean(Eval("Active")) && !Convert.ToString(Eval("Version")).Equals(Convert.ToString(Eval("CurrentVersion")))) %>'
                                        class="btn btn-middle btn-disabled" href="javascript:void(0);">
                                        <%= Resource.Admin_ModulesManager_Installed %></a>
                                    <asp:Button OnClientClick="progressShow();" ID="btnInstallLastVersion" runat="server"
                                        CssClass="btn btn-middle btn-update" CommandArgument='<%# Eval("StringId") %>'
                                        CommandName="InstallLastVersion" Text='<%$ Resources : Resource , Admin_Modules_Update%>'
                                        Visible='<%#!TrialService.IsTrialEnabled &&  Convert.ToBoolean(Eval("IsInstall")) && Convert.ToBoolean(Eval("Active")) && !Convert.ToString(Eval("Version")).Equals(Convert.ToString(Eval("CurrentVersion"))) %>' />
                                    <a id="A3" class="btn btn-middle btn-action" runat="server" href='<%# Eval("DetailsLink") %>'
                                        visible='<%#!TrialService.IsTrialEnabled &&  !Convert.ToBoolean(Eval("IsInstall")) && !Convert.ToBoolean(Eval("Active")) %>'
                                        target="_blank">
                                        <%= Resource.Admin_ModulesManager_Buy %></a>
                                </div>
                            </div>
                            <asp:LinkButton CssClass="module-delete" runat="server" ID="btnDelete" CausesValidation="false"
                                CommandArgument='<%# Eval("StringId") %>' CommandName="Uninstall" Visible='<%# !TrialService.IsTrialEnabled && Convert.ToBoolean(Eval("IsInstall")) %>'>
                            <img src="images/deletebtn.png" onclick="if(confirm('<%= Resource.Admin_ThemesSettings_Confirmation %>')){ progressShow(); }else{ return false;}" alt='<%= Resource.Admin_ModulesManager_Delete %>' />
                            </asp:LinkButton>
                        </div>
                    </li>
                </ItemTemplate>
            </asp:ListView>
            <div class="modules-paging">
                <adv:AdvPaging runat="server" ID="paging" DisplayArrows="false" DisplayPrevNext="false"
                    DisplayShowAll="false" />
            </div>
        </div>
    </div>
    <script>
        $(function () {

            $(document).on('keydown.pagenumber', function (e) {
                //37 - left arrow
                //39 - right arrow
                if (e.ctrlKey === true && e.keyCode === 37) {
                    if ($("#paging-prev").length)
                        document.location = $("#paging-prev").attr("href");
                } else if (e.ctrlKey === true && e.keyCode === 39) {
                    if ($("#paging-next").length)
                        document.location = $("#paging-next").attr("href");
                }
            });
        });

        $(window).load(function () {
            progressHide();
        });
    </script>
</asp:Content>
