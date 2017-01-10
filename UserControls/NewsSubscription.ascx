<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NewsSubscription.ascx.cs"
    Inherits="UserControls.NewsSubscription" %>
<!--noindex-->
<div class="block-uc" data-plugin="expander">
    <h3 class="title" data-expander-control="#newsSubscription">
        <%=Resources.Resource.Client_MasterPage_NewsSubscription%></h3>
    <div class="content" id="newsSubscription">
        <div class="status-input">
            <adv:AdvTextBox ID="txtEmail" runat="server" Placeholder="Email" />
            <span id="orderStatus"></span>
        </div>
        <div class="btn-status-check">
            <adv:Button runat="server" ID="btnSubscribe" Size="Middle" Type="Confirm" Text='<%$ Resources:Resource, Client_MasterPage_Subscribe %>'
                OnClick="btnSubmit_Click" />
            <a href="subscribedeactivate.aspx" title="<%= Resources.Resource.Client_MasterPage_NewsUnsubscription %>">
                <%= Resources.Resource.Client_MasterPage_Unsubscribe %></a>
        </div>
    </div>
</div>
<!--/noindex-->
