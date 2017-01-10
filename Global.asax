<%@ Application Language="C#" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Core" %>
<%@ Import Namespace="AdvantShop.Diagnostics" %>
<script runat="server" Language="C#">
    
    private void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
        // Application_PreRequestHandlerExecute
    }

    public void Application_Start(object sender, EventArgs e)
    {
        ApplicationService.StartApplication(HttpContext.Current);
    }

    public void Application_BeginRequest(object sender, EventArgs e)
    {
        // Nothing here.
    }

    public void Application_End(object sender, EventArgs e)
    {
        // Code that runs on application shutdown
    }

    public void Application_Error(object sender, EventArgs e)
    {
        // Get variable
        Exception ex = Server.GetLastError();
        Debug.LogError(ex, ex is HttpException && ((HttpException)(ex)).GetHttpCode() != 400);

        // Clear the error from the server
        if (ex is HttpException && ((HttpException)(ex)).GetHttpCode() == 400)
            Server.ClearError();	
    }

    public void Session_Start(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(SettingsGeneral.AbsoluteUrl))
        {
            SettingsGeneral.SetAbsoluteUrl(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpContext.Current.Request.ApplicationPath);
        }

        // Code that runs when a new session is started
        SessionServices.StartSession(HttpContext.Current);
    }

    public void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a new session is ended
    }

</script>
