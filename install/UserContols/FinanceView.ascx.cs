using System;
using System.Collections.Generic;
using System.Globalization;
using AdvantShop.Configuration;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace ClientPages
{
    public partial class install_UserContols_FinanceView : AdvantShop.Controls.InstallerStep
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            divBankSettings.Visible = AdvantShop.Core.AdvantshopConfigService.GetActivityCommonSetting("banksettings");
        }

        public new void LoadData()
        {
            txtPlan.Text = String.Format("{0:## ##0.00}", OrderStatisticsService.SalesPlan);
            txtPlaPribl.Text = String.Format("{0:## ##0.00}", OrderStatisticsService.ProfitPlan);
            txtCompanyName.Text = SettingsBank.CompanyName;
            txtCompanyAddress.Text = SettingsBank.Address;
            txtInn.Text = SettingsBank.INN;
            txtKPP.Text = SettingsBank.KPP;
            txtRachetniChet.Text = SettingsBank.RS;
            txtBankName.Text = SettingsBank.BankName;
            txtKorrecpChet.Text = SettingsBank.KS;
            txtBik.Text = SettingsBank.BIK;

            chbCheakProductCount.Checked = SettingsOrderConfirmation.AmountLimitation;
            txtMinOrderPrice.Text = String.Format("{0:## ##0.00}", SettingsOrderConfirmation.MinimalOrderPrice);
            txtMinPriceGift.Text = String.Format("{0:## ##0.00}", SettingsOrderConfirmation.MinimalPriceCertificate);
            txtMaxPriceGift.Text = String.Format("{0:## ##0.00}", SettingsOrderConfirmation.MaximalPriceCertificate);

            if (!string.IsNullOrWhiteSpace(SettingsBank.StampImageName))
            {
                imgPechat.ImageUrl = FoldersHelper.GetPath(FolderType.Pictures, SettingsBank.StampImageName, true);
            }
            else
            {
                imgPechat.Visible = false;
            }

        }

        public new void SaveData()
        {
            float sales;
            float profit;
            StringHelper.GetMoneyFromString(txtPlan.Text, out sales);
            StringHelper.GetMoneyFromString(txtPlaPribl.Text, out profit);
            OrderStatisticsService.SetProfitPlan(sales, profit);

            SettingsOrderConfirmation.AmountLimitation = chbCheakProductCount.Checked;

            float minimalOrderPrice;
            float minimalPriceCertificate;
            float maximalPriceCertificate;

            StringHelper.GetMoneyFromString(txtMinOrderPrice.Text, out minimalOrderPrice);
            SettingsOrderConfirmation.MinimalOrderPrice = minimalOrderPrice;

            StringHelper.GetMoneyFromString(txtMinPriceGift.Text, out minimalPriceCertificate);
            SettingsOrderConfirmation.MinimalPriceCertificate = minimalPriceCertificate;

            StringHelper.GetMoneyFromString(txtMaxPriceGift.Text, out maximalPriceCertificate);
            SettingsOrderConfirmation.MaximalPriceCertificate = maximalPriceCertificate;

            SettingsBank.Address = txtCompanyAddress.Text;
            SettingsBank.CompanyName = txtCompanyName.Text;
            SettingsBank.INN = txtInn.Text;
            SettingsBank.KPP = txtKPP.Text;
            SettingsBank.RS = txtRachetniChet.Text;
            SettingsBank.BankName = txtBankName.Text;
            SettingsBank.KS = txtKorrecpChet.Text;
            SettingsBank.BIK = txtBik.Text;

            if (fuPechat.HasFile)
            {
                FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.Pictures));
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsBank.StampImageName));
                SettingsBank.StampImageName = fuPechat.FileName;
                fuPechat.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, fuPechat.FileName));
            }
        }

        public new bool Validate()
        {
            List<ValidElement> validList = new List<ValidElement>();
            validList.Add(new ValidElement()
            {
                Control = txtPlan,
                ErrContent = ErrContent,
                ValidType = ValidType.Money,
                Message = Resources.Resource.Install_UserContols_FinanceView_Err_Plan
            });
            validList.Add(new ValidElement()
            {
                Control = txtPlaPribl,
                ErrContent = ErrContent,
                ValidType = ValidType.Money,
                Message = Resources.Resource.Install_UserContols_FinanceView_Err_PlaPribl
            });
            validList.Add(new ValidElement()
            {
                Control = txtMinOrderPrice,
                ErrContent = ErrContent,
                ValidType = ValidType.Money,
                Message = Resources.Resource.Install_UserContols_FinanceView_Err_MinOrderPrice
            });
            validList.Add(new ValidElement()
            {
                Control = txtMaxPriceGift,
                ErrContent = ErrContent,
                ValidType = ValidType.Money,
                Message = Resources.Resource.Install_UserContols_FinanceView_Err_MaxPriceGift
            });
            validList.Add(new ValidElement()
            {
                Control = txtMinPriceGift,
                ErrContent = ErrContent,
                ValidType = ValidType.Money,
                Message = Resources.Resource.Install_UserContols_FinanceView_Err_MinPriceGift
            });
            return ValidationHelper.Validate(validList);
        }
    }
}