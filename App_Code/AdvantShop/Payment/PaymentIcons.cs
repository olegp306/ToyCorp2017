//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.IO;
using AdvantShop.FilePath;

namespace AdvantShop.Payment
{
    public class PaymentIcons
    {
        public static string GetPaymentIcon(PaymentType type, string iconName, string name)
        {
            string folderPath = FoldersHelper.GetPath(FolderType.PaymentLogo, string.Empty, false);

            if (!string.IsNullOrWhiteSpace(iconName) && File.Exists(FoldersHelper.GetPathAbsolut(FolderType.PaymentLogo, iconName)))
            {
                return FoldersHelper.GetPath(FolderType.PaymentLogo, iconName, false);
            }

            if (name.Contains(Resources.Resource.Install_UserContols_PaymentView_CreditCard))
            {
                return folderPath + "plasticcard.gif";
            }
            if (name.Contains(Resources.Resource.Install_UserContols_PaymentView_ElectronMoney))
            {
                return folderPath + "emoney.gif";
            }
            if (name.Contains(Resources.Resource.Install_UserContols_PaymentView_Qiwi))
            {
                return folderPath + "qiwi.gif";
            }
            if (name.Contains(Resources.Resource.Install_UserContols_PaymentView_Euroset))
            {
                return folderPath + "euroset.gif";
            }
            return string.Format("{0}{1}_default.gif", folderPath, (int)type);
        }
    }
}