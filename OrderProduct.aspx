<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeFile="OrderProduct.aspx.cs" Inherits="ClientPages.OrderProduct" Title="Untitled Page"
    EnableViewState="false" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <h1><%= Resources.Resource.Client_OrderProduct_Header %></h1>
    <br />
    <span class="ContentText">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </span>
    <a href="." ><%= Resources.Resource.Client_OrderProduct_BackToMainPage %></a>
</asp:Content>
