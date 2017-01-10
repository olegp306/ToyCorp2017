<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VotingUC.ascx.cs" Inherits="UserControls.Default.VotingUC" %>
<%@ Import Namespace="Resources" %>
<!--noindex-->
<article class="block-uc" data-plugin="expander" id="vote">
    <h3 class="title" data-expander-control="#voiting-content">
        <%=Resource.Client_UserControls_VotingUC_Voting%><span class="control"></span></h3>
    <div class="content" id="voiting-content">
        <div data-plugin="vote">
        </div>
    </div>
</article>
<!--/noindex-->
