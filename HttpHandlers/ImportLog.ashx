<%@ WebHandler Language="C#" Class="ImportLog" %>

using System;
using System.Web;
using System.Data;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

public class LineTemplate : ITemplate
{

    private readonly ListItemType _templateType;

    public LineTemplate(ListItemType type)
    {
        _templateType = type;
    }

    private static void TemplateControlDataBinding(object sender, EventArgs e)
    {
        var lc = (Literal)sender;
        var container = (RepeaterItem)lc.NamingContainer;
        lc.Text += DataBinder.Eval(container.DataItem, "message") + "\r\n";
    }

    #region ITemplate Members

    public void InstantiateIn(Control container)
    {
        var lc = new Literal();

        switch (_templateType)
        {
            case ListItemType.Item:
                lc.DataBinding += TemplateControlDataBinding;
                break;
        }

        container.Controls.Add(lc);
    }

    #endregion
}
public class ImportLog : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/download";
        context.Response.AppendHeader("Content-Disposition", "attachment; filename=ImportLog.txt");

        string connstr = ConfigurationManager.ConnectionStrings["AdvantConnectionString"].ConnectionString;
        var logTbl = new DataTable();
        using (var conn = new SqlConnection(connstr))
        {

            var adapter = new SqlDataAdapter("SELECT id, message FROM [Catalog].[ImportLog]", conn);
            conn.Open();
            adapter.Fill(logTbl);
            conn.Close();
        }

        var result = new StringBuilder();
        var writer = new StringWriter(result);
        var htmlWriter = new HtmlTextWriter(writer);

        var rtr = new Repeater
                      {
                          ItemTemplate = new LineTemplate(ListItemType.Item),
                          DataSource = logTbl
                      };
        rtr.DataBind();

        rtr.RenderControl(htmlWriter);

        context.Response.Write(result);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}