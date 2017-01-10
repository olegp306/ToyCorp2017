<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BubbleZone.ascx.cs"
    Inherits="UserControls.MasterPage.BubbleZone" %>
<div class="bubble bubble-town js-bubble-zone" data-plugin="bubble" data-bubble-options="{clickOutClose:true, position: 'center bottom',  eventTypeShow: 'click.Bubble', eventTypeHide: 'click.Bubble', isBackgroundEnabled: true}" id="bubbleZone">
    <%= Resources.Resource.Client_MasterPage_LocationYourCity %> - <%= CurrentTown %>,<br />
    <%= Resources.Resource.Client_MasterPage_LocationGuessed %>?
    <div class="bubble-town-buttons">
        <a href="javascript:void(0);" class="js-bubble-town-ok btn btn-middle btn-confirm"><%= Resources.Resource.Client_MasterPage_LocationYes %></a>
        <a href="javascript:void(0);" class="js-bubble-town-no btn btn-middle btn-action"><%= Resources.Resource.Client_MasterPage_LocationNo %></a>
    </div>
</div>
