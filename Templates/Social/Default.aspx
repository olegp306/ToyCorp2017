<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="ClientPages.Default" EnableViewState="false" %>

<%@ Register Src="~/UserControls/StaticBlock.ascx" TagName="StaticBlock" TagPrefix="adv" %>

<%@ Register TagPrefix="adv" TagName="MainPageProduct" Src="~/UserControls/Default/MainPageProduct.ascx" %>
<%@ Register TagPrefix="adv" TagName="News" Src="~/UserControls/Default/News.ascx" %>
<%@ Register TagPrefix="adv" TagName="Carousel" Src="~/UserControls/Default/Carousel.ascx" %>
<%@ Register TagPrefix="adv" TagName="GiftCertificate" Src="~/UserControls/Default/GiftCertificate.ascx" %>
<%@ Register TagPrefix="adv" TagName="CheckOrder" Src="~/UserControls/Default/CheckOrder.ascx" %>
<%@ Register TagPrefix="adv" TagName="Voting" Src="~/UserControls/Default/VotingUC.ascx" %>
<%@ Register TagPrefix="adv" TagName="MenuCatalogAlternative" Src="~/UserControls/MasterPage/MenuCatalogAlternative.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <asp:MultiView runat="server" ID="mvDefaultPage">
        <Views>
            <asp:View runat="server" ID="defaultView">
                <adv:Carousel runat="server" ID="carousel" CssSlider="flexslider-main" />
                <%= MainPageProductsAfterCarousel%>
                <adv:StaticBlock ID="sbTextOnMain" runat="server" SourceKey="TextOnMain" />
                <%= MainPageProducts%>
                <adv:MainPageProduct ID="mainPageProduct" runat="server" Mode="TwoColumns" />
                <adv:StaticBlock ID="sbTextOnMain2" runat="server" SourceKey="TextOnMain2" />
                <!--shadow_split-->
                <div class="shadow-split">
                </div>
                <!--end_shadow_split-->
                <!--blocks-->
                <div class="container-default-blocks">
                    <!--new-block-->
                    <adv:News ID="news" runat="server" />
                    <!--end_new-block-->
                    <!--voting-->
                    <adv:Voting runat="server" ID="voting" />
                    <!--end_voting-->
                    <!--check_status-->
                    <div class="block-uc-merge">
                        <adv:CheckOrder ID="checkOrder" runat="server" />
                        <adv:GiftCertificate ID="giftCertificate" runat="server" Mode="Default" />
                    </div>
                    <!--end_check_status-->
                    <!--social-->
                    <div class="block-uc social-big">
                        <adv:StaticBlock ID="staticBlockVk" runat="server" SourceKey="Vk" DisableInplaceEditor="true" />
                    </div>
                    <!--end_social-->
                </div>
                <!--end_blocks-->
                <br class="clear" />
                <div class="split-light-blue">
                </div>
            </asp:View>
        </Views>
    </asp:MultiView>
</asp:Content>
