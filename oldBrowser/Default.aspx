<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="ClientPages.ie6_Default" %>
<%@ Register TagPrefix="adv" TagName="LiveCounter" Src="~/UserControls/MasterPage/LiveinternetCounter.ascx" %>
<%@ Import Namespace="AdvantShop.SEO" %>

<!DOCTYPE html>
<html>
<head>
    <base href="<%= Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + (!Request.ApplicationPath.EndsWith("/") ? "/" : string.Empty) %>" />
    <title></title>
    <link type="text/css" rel="stylesheet" href="oldBrowser/css/styles.css" />
</head>
<body>
    <adv:LiveCounter runat="server" />
    <%= new GoogleAnalyticsString().GetGoogleAnalyticsString()%>
    <form id="form" runat="server">
        <div class="wrap">
            <div class="main" id="current-ex2">
                <div class="qtcontent">
                    <h1>
                        <%=Resources.Resource.IE6_Caution %></h1>
                    <p>
                        <%=Resources.Resource.IE6_About %>
                    </p>
                    <p>
                        <%=Resources.Resource.IE6_Recommended %>
                    </p>
                    <br />
                    <table class="brows">
                        <tr>
                            <td>
                                <a target="_blank" href="http://www.microsoft.com/windows/Internet-explorer/default.aspx">
                                    <img src="oldBrowser/images/ie.jpg" alt="Internet Explorer" /></a>
                            </td>
                            <td>
                                <a target="_blank" href="http://www.opera.com/download/">
                                    <img src="oldBrowser/images/op.jpg" alt="Opera Browser" /></a>
                            </td>
                            <td>
                                <a target="_blank" href="http://www.mozilla.com/firefox/">
                                    <img src="oldBrowser/images/mf.jpg" alt="Mozilla Firefox" /></a>
                            </td>
                            <td>
                                <a target="_blank" href="http://www.google.com/chrome">
                                    <img src="oldBrowser/images/gc.jpg" alt="Google Chrome" /></a>
                            </td>
                        </tr>
                        <tr class="brows_name">
                            <td>
                                <a target="_blank" href="http://www.microsoft.com/windows/Internet-explorer/default.aspx">Internet Explorer</a>
                            </td>
                            <td>
                                <a target="_blank" href="http://www.opera.com/download/">Opera
                                Browser</a>
                            </td>
                            <td>
                                <a target="_blank" href="http://www.mozilla.com/firefox/">Mozilla
                                Firefox</a>
                            </td>
                            <td>
                                <a target="_blank" href="http://www.google.com/chrome">Google
                                Chrome</a>
                            </td>
                        </tr>
                    </table>
                    <h2>
                        <%=Resources.Resource.IE6_Why %></h2>
                    <p>
                        <%=Resources.Resource.IE6_OldBrowser %>
                    </p>
                    <p>
                        <%=Resources.Resource.IE6_PortableVersion %>
                    </p>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
