//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.IO;
using AdvantShop.FilePath;

namespace AdvantShop.Shipping
{
    public class ShippingIcons
    {
        public static string GetShippingIcon(ShippingType type, string iconName, string namefragment)
        {
            string folderPath = FoldersHelper.GetPath(FolderType.ShippingLogo, string.Empty, false);

            if ((type == ShippingType.eDost || type == ShippingType.Cdek || type == ShippingType.Multiship || type == ShippingType.CheckoutRu) &&
                iconName.IsNullOrEmpty())
            {
                namefragment = namefragment.ToLower();
                if (namefragment.Contains("ems"))
                    return folderPath + "7_ems.gif";
                if (namefragment.Contains("почта россии") || namefragment.Contains("доставка почтой"))
                    return folderPath + "7_pochtarussia.gif";
                if (namefragment.Contains("спср экспресс") || namefragment.Contains("спср"))
                    return folderPath + "7_spsrExpress.gif";
                if (namefragment.Contains("сдэк"))
                    return folderPath + "7_cdek.gif";
                if (namefragment.Contains("dhl"))
                    return folderPath + "7_dhl.gif";
                if (namefragment.Contains("ups"))
                    return folderPath + "7_ups.gif";
                if (namefragment.Contains("желдорэкспедиция"))
                    return folderPath + "7_trainroadExpedition.gif";
                if (namefragment.Contains("автотрейдинг"))
                    return folderPath + "7_autotraiding.gif";
                if (namefragment.Contains("пэк"))
                    return folderPath + "7_pek.gif";
                if (namefragment.Contains("деловые линии"))
                    return folderPath + "7_delovielinies.gif";
                if (namefragment.Contains("мегаполис"))
                    return folderPath + "7_megapolis.gif";
                if (namefragment.Contains("гарантпост"))
                    return folderPath + "7_garantpost.gif";
                if (namefragment.Contains("pony"))
                    return folderPath + "7_ponyexpress.gif";
                if (namefragment.Contains("pickpoint"))
                    return folderPath + "7_pickpoint.gif";
                if (namefragment.Contains("boxberry"))
                    return folderPath + "7_boxberry.gif";
                if (namefragment.Contains("энергия"))
                    return folderPath + "7_energia.gif";
                if (namefragment.Contains("hermes"))
                    return folderPath + "7_hermes.gif";
                if (namefragment.Contains("dpd"))
                    return folderPath + "7_dpd.gif";
                if (namefragment.Contains("shoplogistics"))
                    return folderPath + "7_shoplogistics.gif";

                if (namefragment.Contains("shoplogistics"))
                    return folderPath + "7_shoplogistics.gif";

                if (namefragment.Contains("пункты выдачи") || namefragment.Contains("постоматы"))
                    return folderPath + "1.gif";

                if (namefragment.Contains("курьерская доставка"))
                    return folderPath + "2.gif";

                if (iconName.IsNotEmpty() && File.Exists(FoldersHelper.GetPathAbsolut(FolderType.ShippingLogo, iconName)))
                    return folderPath + iconName;

                return folderPath + "7_default.gif";
            }

            if (iconName.IsNotEmpty() && File.Exists(FoldersHelper.GetPathAbsolut(FolderType.ShippingLogo, iconName)))
                return folderPath + iconName;

            return string.Format("{0}{1}.gif", folderPath, (int) type);
        }
    }
}