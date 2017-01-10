//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;

namespace Tools.core
{
    public partial class DeleteSvn : System.Web.UI.Page
    {
        protected void btnDeleteSvn_Click(object sender, System.EventArgs e)
        {
            string rootCategory = Server.MapPath("~/");

            if (Directory.Exists(rootCategory))
            {
                DeleteAllSVN(rootCategory);
            }

        }

        protected void DeleteAllSVN(string dirName)
        {

            var list = new List<string>();
            list.AddRange(Directory.GetDirectories(dirName, ".svn", SearchOption.AllDirectories));

            var stringResult = new System.Text.StringBuilder();
            Boolean summRes = false;

            // Process

            try
            {

                foreach (var fileName in list)
                {

                    if (chboxDeleteFiles.Checked)
                    {
                        stringResult.AppendFormat("Deleted: '{0}'<br/>", fileName);
                        Directory.Delete(fileName, true);
                    }
                    else
                    {
                        stringResult.AppendFormat("Found: '{0}'<br/>", fileName);
                    }

                }

                summRes = true;

            }
            catch (Exception ex)
            {
                stringResult.AppendFormat("<b>Error: '{0}'</b><br/>", ex.Message);
            }

            // Summary result

            lCompleted.Visible = true;

            if (summRes)
            {
                if (chboxDeleteFiles.Checked)
                {
                    lCompleted.Text = "Cleanup successfuly completed";
                }
                else
                {
                    lCompleted.Text = "Analysis successfully completed";
                }

            }
            else
            {
                lCompleted.Text = "Cleanup completed with errors";
            }

            lResultHeader.Visible = true;
            lResult.Text = stringResult.ToString();

        }
    }
}