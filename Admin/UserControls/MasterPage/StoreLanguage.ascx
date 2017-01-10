<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StoreLanguage.ascx.cs"
    Inherits="Admin.UserControls.MasterPage.StoreLanguage" %>
<div class="top-part-right dropdown-menu-parent dropdown-arrow-light">
    <%= RenderLanguageImage() %>
    <div class="dropdown-menu-wrap">
        <ul class="dropdown-menu">
            <li class="dropdown-menu-item">
                <asp:LinkButton runat="server" ID="lnkEnglishLanguage" OnClick="lnkEnglishLanguage_Click" CausesValidation="False">
                    <img src="images/new_admin/lang/en.jpg" alt="<%= Resources.Resource.Global_Language_English %>" class="flag"/>
                    <%= Resources.Resource.Global_Language_English%>
                </asp:LinkButton>
            </li>
            <li class="dropdown-menu-item">
                <asp:LinkButton runat="server" ID="lnkRussianLanguage" OnClick="lnkRussianLanguage_Click" CausesValidation="False">
                    <img src="images/new_admin/lang/ru.jpg" alt='<%= Resources.Resource.Global_Language_Russian %>' class="flag"/>
                    <%= Resources.Resource.Global_Language_Russian %>
                </asp:LinkButton>
            </li>
            <li class="dropdown-menu-item">
                <asp:LinkButton runat="server" ID="lnkUkrainianLanguage" OnClick="lnkUkrainianLanguage_Click" CausesValidation="False">
                    <img src="images/new_admin/lang/uk.jpg" alt='<%= Resources.Resource.Global_Language_Ukrainian %>' class="flag"/>
                    <%= Resources.Resource.Global_Language_Ukrainian %>
                </asp:LinkButton>
            </li>
        </ul>
    </div>
</div>
