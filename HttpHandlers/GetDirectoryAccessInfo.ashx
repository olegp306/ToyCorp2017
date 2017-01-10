<%@ WebHandler Language="C#" Class="GetDirectoryAccessInfo" %>

using System;
using System.IO;
using System.Text;
using System.Web;
using Resources;

public class GetDirectoryAccessInfo : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/html";

        //context.Response.Write("qweasd");
        context.Response.Write(GetAccessInfoHtml());
    }
    
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    private static string GetAccessInfoHtml()
    {
        var testData = new string[] {"~/", 
                                    "~/admin/css/",
                                    "~/admin/js/",
                                    "~/App_Data/", 
                                    "~/App_Data/errlog/", 
                                    "~/App_Data/Lucene/", 
                                    "~/App_Data/notepad/",
                                    "~/css/",
                                    "~/design/",
                                    "~/exports/",
                                    "~/js/",
                                    "~/Modules/",
                                    "~/pictures/",
                                    "~/pictures/category/",
                                    "~/pictures/category/small/",
                                    "~/pictures/news/", 
                                    "~/pictures/product/",
                                    "~/pictures/product/big/",
                                    "~/pictures/product/middle/",
                                    "~/pictures/product/small/",
                                    "~/pictures/product/xsmall/",
                                    "~/price_temp/", 
                                    "~/Templates/",
                                    "~/upload_images/",
                                    "~/userfiles/", 
                                    "~/userfiles/file/", 
                                    "~/userfiles/flash/", 
                                    "~/userfiles/image/"};

        var sb = new StringBuilder();

        sb.AppendFormat("<span style='font-size: 14px; font-weight: bold;'>{0}: '{1}'</span>",
            Resource.Install_UserContols_TrialSelectView_AccsessRights_ActualDate,
            DateTime.Now.ToString());
        
        sb.AppendFormat("<table style='margin-top:5px; border-collapse:separate;'><tr><td>{0}</td><td style='width:85px;'>{1}</td><td style='width:75px;'>{2}</td><td style='width:75px;'>{3}</td><td style='width:75px;'>{4}</td><td style='width:75px;'>{5}</td></tr>",
            Resource.Install_UserContols_TrialSelectView_AccsessRights_Path,
            Resource.Install_UserContols_TrialSelectView_AccsessRights_IsExist,
            Resource.Install_UserContols_TrialSelectView_AccsessRights_Create,
            Resource.Install_UserContols_TrialSelectView_AccsessRights_Read,
            Resource.Install_UserContols_TrialSelectView_AccsessRights_Modify,
            Resource.Install_UserContols_TrialSelectView_AccsessRights_Delete
            );
        
        foreach (string strTestData in testData)
        {
            sb.Append("<tr>");
            sb.AppendFormat("<td><span style='font-weight: bold;'>{0}</span></td>", strTestData);
            string fullName = HttpContext.Current.Server.MapPath(strTestData);
            if (Directory.Exists(fullName))
            {
                sb.Append("<td><span style='color:green;'>Exist</span></td>");
                var dir = new DirectoryInfo(fullName);
                sb.Append(CheckDirectory(dir));
            }
            else
            {
                sb.Append("<td><span style='color:red;'>Not exist</span></td>");
                sb.Append("<td><span style='color:red'>False</span></td>");
                sb.Append("<td><span style='color:red'>False</span></td>");
                sb.Append("<td><span style='color:red'>False</span></td>");
                sb.Append("<td><span style='color:red'>False</span></td>");
            }
            sb.Append("</tr>");
        }
        sb.Append("</table>");

        return sb.ToString();
    }
    
    private static string CheckDirectory(DirectoryInfo directory)
    {
        var sb = new StringBuilder();
        
        string testFileName = directory.FullName + "testFile";

        try
        {
            File.Create(testFileName).Close();
            sb.Append("<td><span style='color:green;'>True</span></td>");
        }
        catch (Exception)
        {
            sb.Append("<td><span style='color:red;'>False</span></td>");
        }

        var tf = new FileInfo(testFileName);
        BinaryWriter bw = null;
        bool allowread = false;
        try
        {
            bw = new BinaryWriter(tf.Open(FileMode.Open));
            allowread = true;
            sb.Append("<td><span style='color:green;'>True</span></td>");
        }
        catch (Exception)
        {
            sb.Append("<td><span style='color:red;'>False</span></td>");
        }

        if (allowread)
        {
            bw.Write("test string");
            sb.Append("<td><span style='color:green;'>True</span></td>");
        }
        else
            sb.Append("<td><span style='color:red;'>False</span></td>");

        if (bw != null)
        {
            bw.Close();
        }

        try
        {
            tf.Delete();
            sb.Append("<td><span style='color:green;'>True</span></td>");
        }
        catch (Exception)
        {
            sb.Append("<td><span style='color:red;'>False</span></td>");
        }
        return sb.ToString();
    }
}