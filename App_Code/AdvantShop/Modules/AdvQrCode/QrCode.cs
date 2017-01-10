using System.IO;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Modules.Interfaces;
using System.Globalization;

namespace AdvantShop.Modules
{
    public class AdvQrCode : IModuleDetails
    {
        public string ModuleStringId
        {
            get { return "advqrcode"; }
        }

        public string ModuleName
        {
            get
            {
                switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                {
                    case "ru":
                        return "QR код";

                    case "en":
                        return "QR code";

                    default:
                        return "QR code";
                }
            }
        }

        public List<IModuleControl> ModuleControls
        {
            get
            {
                return new List<IModuleControl> ();
               // return new List<IModuleControl> { new QrcodeSetting() };
            }
        }

        public bool HasSettings
        {
            get { return false; }
        }

        public bool CheckAlive()
        {
            return ModulesRepository.IsInstallModule(ModuleStringId) &&
                File.Exists(HttpContext.Current.Server.MapPath("~/bin/Gma.QrCodeNet.Encoding.dll")) &&
                File.Exists(HttpContext.Current.Server.MapPath("~/Modules/AdvQrCode/GetQrCodeByUrl.ashx"));
        }

        public bool InstallModule()
        {
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/bin/Gma.QrCodeNet.Encoding.dll")))
            {
                File.Copy(HttpContext.Current.Server.MapPath("~/App_Code/AdvantShop/Modules/AdvQrCode/Gma.QrCodeNet.Encoding.dll"),
                    HttpContext.Current.Server.MapPath("~/bin/Gma.QrCodeNet.Encoding.dll"));
            }

            return File.Exists(HttpContext.Current.Server.MapPath("~/bin/Gma.QrCodeNet.Encoding.dll"));
        }

        public bool UpdateModule()
        {
            return true;
        }

        public bool UninstallModule()
        {
            if (File.Exists(HttpContext.Current.Server.MapPath("~/bin/Gma.QrCodeNet.Encoding.dll")))
            {
                File.Delete(HttpContext.Current.Server.MapPath("~/bin/Gma.QrCodeNet.Encoding.dll"));
            }
            return true;
        }

        public string RenderToRightColumn()
        {
            var url = "http://" + HttpContext.Current.Request.Url.Authority + HttpContext.Current.Request.RawUrl;
            return "<img src =\"Modules/AdvQrCode/GetQrCodeByUrl.ashx?url=" + HttpUtility.UrlEncode(url) + "\" alt=\"qrcode\">";
        }

        public string RenderToProductInformation()
        {
            return string.Empty;
        }

        public string RenderToProductInformation(int productId)
        {
            return string.Empty;
        }

        public string RenderToBottom()
        {
            return string.Empty;
        }

        //private class QrcodeSetting : IModuleControl
        //{
        //    #region Implementation of IModuleControl

        //    public string NameTab
        //    {
        //        get
        //        {
        //            switch (CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
        //            {
        //                case "ru":
        //                    return "Настройки";

        //                case "en":
        //                    return "Settings";

        //                default:
        //                    return "Settings";
        //            }
        //        }
        //    }

        //    public string File
        //    {
        //        get { return "AdvQrCodeModule.ascx"; }
        //    }

        //    #endregion
        //}
    }

}