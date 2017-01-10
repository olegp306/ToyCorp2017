//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using Resources;

namespace Admin
{
    public partial class StylesEditor : AdvantShopAdminPage
    {
        private string Path;

        protected void Page_Load(object sender, EventArgs e)
        {
            Path = Server.MapPath("~/css/extra.css");
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_MasterPageAdmin_StylesEditor));
            MsgErr(true);
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!File.Exists(Path))
            {
                using (File.Create(Path))
                {
                    //nothing here, just  create file
                }
            }

            using (TextReader reader = new StreamReader(Path))
            {
                txtStyle.Text = reader.ReadToEnd();
            }
        }

        protected void btnSaveClick(object sender, EventArgs e)
        {
            try
            {
                using (TextWriter writer = new StreamWriter(Path, false))
                {
                    writer.Write(txtStyle.Text);
                }
                MsgErr(Resource.Admin_StylesEditor_FileSaved, true);
            }
            catch (Exception ex)
            {
                MsgErr("Error: " + ex.Message, false);
            }
        }

        private void MsgErr(bool boolClean)
        {
            if (boolClean)
            {
                lblInfo.Visible = false;
                lblInfo.Text = string.Empty;
            }
            else
            {
                lblInfo.Visible = false;
            }
        }

        private void MsgErr(string strMessageText, bool isSucces)
        {
            const string strSuccesFormat = "<div class=\"label-box-admin good\" style=\"margin-top:20px; margin-bottom:20px; width: 735px;\">{0} // at {1}</div>";
            const string strFailFormat = "<div class=\"label-box-admin error\" style=\"margin-top:20px; margin-bottom:20px; width: 735px;\">{0} // at {1}</div>";

            lblInfo.Visible = true;

            if (isSucces)
            {
                lblInfo.Text = string.Format(strSuccesFormat, strMessageText, DateTime.Now.ToString());
            }
            else
            {
                lblInfo.Text = string.Format(strFailFormat, strMessageText, DateTime.Now.ToString());
            }

        }
    }
}