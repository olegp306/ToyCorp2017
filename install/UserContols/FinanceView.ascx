<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FinanceView.ascx.cs" Inherits="ClientPages.install_UserContols_FinanceView" %>
<h1>
    <%= Resources .Resource .Install_UserContols_FinanceView_h1 %></h1>
<h2>
    <% = Resources .Resource .Install_UserContols_FinanceView_h2 %></h2>
<div class="group">
    <p>
        <% = Resources .Resource .Install_UserContols_FinanceView_Plan %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-money" runat="server" ID="txtPlan"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources .Resource .Install_UserContols_FinanceView_PlanPrib %></p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-money" runat="server" ID="txtPlaPribl"></asp:TextBox>
    </div>
</div>
<div class="group">
    <div class="str">
        <asp:CheckBox runat="server" ID="chbCheakProductCount" Text="<%$ Resources :Resource , Install_UserContols_FinanceView_CheckAvalible %>" />
    </div>
</div>
<div class="group">
    <p>
        <% = Resources .Resource .Install_UserContols_FinanceView_MinSum %>
    </p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-money" runat="server" ID="txtMinOrderPrice"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources .Resource .Install_UserContols_FinanceView_MaxSumCertificate %>
    </p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-money" runat="server" ID="txtMaxPriceGift"></asp:TextBox>
    </div>
</div>
<div class="group">
    <p>
        <% = Resources .Resource . Install_UserContols_FinanceView_MinSumCertificate %>
    </p>
    <div class="str">
        <asp:TextBox CssClass="txt valid-money" runat="server" ID="txtMinPriceGift"></asp:TextBox>
    </div>
</div>
<div id="divBankSettings" runat="server">
    <h2>
        <% = Resources .Resource . Install_UserContols_FinanceView_h2_2 %></h2>
    <div class="group">
        <p>
            <% = Resources .Resource .Install_UserContols_FinanceView_CompanyName  %></p>
        <div class="str">
            <asp:TextBox CssClass="txt" runat="server" ID="txtCompanyName"></asp:TextBox></div>
    </div>
    <div class="group">
        <p>
            <% = Resources .Resource .Install_UserContols_FinanceView_Address  %></p>
        <div class="str">
            <asp:TextBox CssClass="txt" runat="server" ID="txtCompanyAddress"></asp:TextBox></div>
    </div>
    <div class="group">
        <p>
            <% = Resources .Resource .Install_UserContols_FinanceView_INN %></p>
        <div class="str">
            <asp:TextBox CssClass="txt" runat="server" ID="txtInn"></asp:TextBox>
        </div>
    </div>
    <div class="group">
        <p>
            <% = Resources .Resource .Install_UserContols_FinanceView_KPP %></p>
        <div class="str">
            <asp:TextBox CssClass="txt" runat="server" ID="txtKPP"></asp:TextBox>
        </div>
    </div>
    <div class="group">
        <p>
            <% = Resources .Resource .Install_UserContols_FinanceView_RachetniChet %></p>
        <div class="str">
            <asp:TextBox CssClass="txt" runat="server" ID="txtRachetniChet"></asp:TextBox>
        </div>
    </div>
    <div class="group">
        <p>
            <% = Resources .Resource .Install_UserContols_FinanceView_BankName %></p>
        <div class="str">
            <asp:TextBox CssClass="txt" runat="server" ID="txtBankName"></asp:TextBox>
        </div>
    </div>
    <div class="group">
        <p>
            <% = Resources .Resource .Install_UserContols_FinanceView_KorrecpChet %></p>
        <div class="str">
            <asp:TextBox CssClass="txt" runat="server" ID="txtKorrecpChet"></asp:TextBox>
        </div>
    </div>
    <div class="group">
        <p>
            <% = Resources .Resource .Install_UserContols_FinanceView_Bik %></p>
        <div class="str">
            <asp:TextBox CssClass="txt" runat="server" ID="txtBik"></asp:TextBox>
        </div>
    </div>
    <div class="group">
        <p>
            <% = Resources .Resource .Install_UserContols_FinanceView_CompanyPechat %></p>
        <div class="container-logo">
            <asp:Image runat="server" ID="imgPechat" />
        </div>
        <asp:FileUpload runat="server" ID="fuPechat" CssClass="file-upload" />
    </div>
</div>
