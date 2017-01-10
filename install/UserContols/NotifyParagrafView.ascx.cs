using System;
using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Helpers;

namespace ClientPages
{
    public partial class install_UserContols_NotifyParagrafView : AdvantShop.Controls.InstallerStep
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public new void LoadData()
        {
            txtOrderEmail.Text = SettingsMail.EmailForOrders;
            txtEmailProductDiscuss.Text = SettingsMail.EmailForProductDiscuss;
            txtEmailRegReport.Text = SettingsMail.EmailForRegReport;
            txtFeedbackEmail.Text = SettingsMail.EmailForFeedback;

            txtEmailSMTP.Text = SettingsMail.SMTP;
            txtEmailLogin.Text = SettingsMail.Login;
            txtEmailPassword.Text = SettingsMail.Password;
            txtEmail.Text = SettingsMail.From;
            chkEnableSSL.Checked = SettingsMail.SSL;
            txtEmailPort.Text = SettingsMail.Port.ToString();
        }

        public new void SaveData()
        {
            SettingsMail.EmailForOrders = txtOrderEmail.Text;
            SettingsMail.EmailForProductDiscuss = txtEmailProductDiscuss.Text;
            SettingsMail.EmailForRegReport = txtEmailRegReport.Text;
            SettingsMail.EmailForFeedback = txtFeedbackEmail.Text;

            SettingsMail.SMTP = txtEmailSMTP.Text;
            SettingsMail.Login = txtEmailLogin.Text;
            SettingsMail.Password = txtEmailPassword.Text;
            SettingsMail.From = txtEmail.Text;
            SettingsMail.Port = txtEmailPort.Text.TryParseInt();
            SettingsMail.SSL = chkEnableSSL.Checked;
        }

        public new bool Validate()
        {
            var validList = new List<ValidElement>();

            validList.Add(new ValidElement
            {
                Control = txtEmailRegReport,
                ErrContent = ErrContent,
                ValidType = ValidType.Email,
                Message = Resources.Resource.Install_UserContols_NotifyParagrafView_NeedRegReport
            });
            validList.Add(new ValidElement
            {
                Control = txtOrderEmail,
                ErrContent = ErrContent,
                ValidType = ValidType.Email,
                Message = Resources.Resource.Install_UserContols_NotifyParagrafView_NeedOrder
            });
            validList.Add(new ValidElement
            {
                Control = txtEmailProductDiscuss,
                ErrContent = ErrContent,
                ValidType = ValidType.Email,
                Message = Resources.Resource.Install_UserContols_NotifyParagrafView_NeedProductDiscuss
            });
            //validList.Add(new ValidElement { Control = txtEmailProductQuestion, ErrContent = ErrContent, ValidType = ValidType.Email, Message = Resources.Resource.Install_UserContols_NotifyParagrafView_NeedProductQuestion });
            validList.Add(new ValidElement
            {
                Control = txtFeedbackEmail,
                ErrContent = ErrContent,
                ValidType = ValidType.Email,
                Message = Resources.Resource.Install_UserContols_NotifyParagrafView_NeedFeedBack
            });
            validList.Add(new ValidElement
            {
                Control = txtEmailSMTP,
                ErrContent = ErrContent,
                ValidType = ValidType.Required,
                Message = Resources.Resource.Install_UserContols_NotifyParagrafView_NeedSMTP
            });
            validList.Add(new ValidElement
            {
                Control = txtEmailLogin,
                ErrContent = ErrContent,
                ValidType = ValidType.Required,
                Message = Resources.Resource.Install_UserContols_NotifyParagrafView_NeedLogin
            });
            validList.Add(new ValidElement
            {
                Control = txtEmailPassword,
                ErrContent = ErrContent,
                ValidType = ValidType.Required,
                Message = Resources.Resource.Install_UserContols_NotifyParagrafView_NeedPass
            });
            validList.Add(new ValidElement
            {
                Control = txtEmailPort,
                ErrContent = ErrContent,
                ValidType = ValidType.Number,
                Message = Resources.Resource.Install_UserContols_NotifyParagrafView_NeedPort
            });
            validList.Add(new ValidElement
            {
                Control = txtEmail,
                ErrContent = ErrContent,
                ValidType = ValidType.Email,
                Message = Resources.Resource.Install_UserContols_NotifyParagrafView_NeedEmail
            });

            return ValidationHelper.Validate(validList);
        }
    }
}