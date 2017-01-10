using System;
using AdvantShop.Configuration;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace Admin.UserControls.Settings
{
    public partial class BankSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidBank;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            pnlStamp.Visible = !string.IsNullOrWhiteSpace(SettingsBank.StampImageName);
        }

        protected string GetImageSource()
        {
            if (!string.IsNullOrWhiteSpace(SettingsBank.StampImageName))
            {
                return FoldersHelper.GetPath(FolderType.Pictures, SettingsBank.StampImageName, true);
            }
            return string.Empty;
        }

        private void LoadData()
        {
            txtINN.Text = SettingsBank.INN;
            txtRS.Text = SettingsBank.RS;
            txtDirector.Text = SettingsBank.Director;
            txtManager.Text = SettingsBank.Manager;
            txtHeadCounter.Text = SettingsBank.Accountant;
            txtBIK.Text = SettingsBank.BIK;
            txtBankName.Text = SettingsBank.BankName;
            txtKPP.Text = SettingsBank.KPP;
            txtKS.Text = SettingsBank.KS;
            txtCompanyName.Text = SettingsBank.CompanyName;
            txtAddress.Text = SettingsBank.Address;

        }
        public bool SaveData()
        {
            SettingsBank.INN = txtINN.Text;
            SettingsBank.RS = txtRS.Text;
            SettingsBank.Director = txtDirector.Text;
            SettingsBank.Manager = txtManager.Text;
            SettingsBank.Accountant = txtHeadCounter.Text;
            SettingsBank.BIK = txtBIK.Text;
            SettingsBank.BankName = txtBankName.Text;
            SettingsBank.KPP = txtKPP.Text;
            SettingsBank.KS = txtKS.Text;
            SettingsBank.CompanyName = txtCompanyName.Text;
            SettingsBank.Address = txtAddress.Text;

            if (fuStamp.HasFile)
            {
                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.ImageTemp));
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsBank.StampImageName));
                SettingsBank.StampImageName = fuStamp.FileName;
                fuStamp.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, fuStamp.FileName));
            }

            LoadData();
            return true;
        }

        private bool ValidateData()
        {
            return true;
        }

        protected void DeleteStamp_Click(object sender, EventArgs e)
        {
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsBank.StampImageName));
            SettingsBank.StampImageName = string.Empty;
        }
    }
}