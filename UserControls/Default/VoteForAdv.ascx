<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VoteForAdv.ascx.cs" Inherits="UserControls.Default.VoteForAdv" %>
<!-- Current version --- staring --- -->
<%  if (this.Visible)
    {%>
<div class="block" style="margin-top: -4px;">
    <div class="block_header">
        <div class="block_name">
            <%=Resources.Resource.UserControl_VoteForAdv_Like%></div>
    </div>
    <div class="block_middle">
        <div class="block_content" style="font-size: 11px; text-align: center;">
            <div id="fb-root">
            </div>
            <script>
                (function (d, s, id) {
                    var js, fjs = d.getElementsByTagName(s)[0];
                    if (d.getElementById(id)) { return; }
                    js = d.createElement(s); js.id = id;
                    js.src = "//connect.facebook.net/ru_RU/all.js#xfbml=1";
                    fjs.parentNode.insertBefore(js, fjs);
                } (document, 'script', 'facebook-jssdk'));
            </script>
            <div class="fb-like" data-send="false" data-layout="button_count" data-width="140"
                data-show-faces="true">
            </div>            
        </div>
    </div>
    <div class="block_footer">
    </div>
</div>
<% }%>
<!-- Current version --- ending --- -->
