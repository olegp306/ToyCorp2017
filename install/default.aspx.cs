using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.SaasData;
using AdvantShop.Trial;
using Resources;

namespace ClientPages
{
    public partial class _default : Page
    {
        public enum ActiveDiv
        {
            None = 0,
            TrialSelect = 1,
            Shopinfo = 2,
            Finance = 3,
            Payment = 4,
            Shipping = 5,
            OpenidParagraf = 6,
            NotifyParagraf = 7,
            Final = 8,

        }

        public struct MenuItem
        {
            public string MenuName;
            public ActiveDiv Div;
            public string StyleClass;
        }

        public readonly List<MenuItem> MenuItems = new List<MenuItem>();

        private bool _hasWriteAccess = true;

        private ActiveDiv _currentDiv = ActiveDiv.None;

        public ActiveDiv CurrentDiv
        {
            get
            {
                if (_currentDiv != ActiveDiv.None) return _currentDiv;
                if (string.IsNullOrEmpty(Request["step"]))
                {
                    if (!_hasWriteAccess)
                    {
                        _currentDiv = ActiveDiv.TrialSelect;
                        TrialSelectView.HasWriteAccess = false;
                        return _currentDiv;
                    }
                    _currentDiv = TrialService.IsTrialEnabled ? ActiveDiv.TrialSelect : ActiveDiv.Shopinfo;
                    return _currentDiv;
                }

                if (Enum.IsDefined(typeof (ActiveDiv), Request["step"]))
                    _currentDiv = (ActiveDiv) Enum.Parse(typeof (ActiveDiv), Request["step"], true);

                return _currentDiv;
            }
        }

        protected override void InitializeCulture()
        {
            AdvantShop.Localization.Culture.InitializeCulture();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            MenuItems.AddRange(new List<MenuItem>
            {
                new MenuItem
                {
                    MenuName = Resource.Install_Default_LeftMenu_AboutShop,
                    Div = ActiveDiv.Shopinfo,
                    StyleClass = "shop-info"
                },
                new MenuItem
                {
                    MenuName = Resource.Install_Default_LeftMenu_Finance,
                    Div = ActiveDiv.Finance,
                    StyleClass = "finance"
                },
                new MenuItem
                {
                    MenuName = Resource.Install_Default_LeftMenu_Payment,
                    Div = ActiveDiv.Payment,
                    StyleClass = "payment"
                },
                new MenuItem
                {
                    MenuName = Resource.Install_Default_LeftMenu_Shipping,
                    Div = ActiveDiv.Shipping,
                    StyleClass = "shipping"
                },
                new MenuItem
                {
                    MenuName = Resource.Install_Default_LeftMenu_Security,
                    Div = ActiveDiv.OpenidParagraf,
                    StyleClass = "openid paragraf"
                },
                new MenuItem
                {
                    MenuName = Resource.Install_Default_LeftMenu_Notification,
                    Div = ActiveDiv.NotifyParagraf,
                    StyleClass = "notify paragraf"
                },
                new MenuItem
                {
                    MenuName = Resource.Install_Default_LeftMenu_FinalStep,
                    Div = ActiveDiv.Final,
                    StyleClass = "final"
                }
            });
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (File.Exists(SettingsGeneral.InstallFilePath))
                Response.Redirect("~/");

            _hasWriteAccess = CheckAppDataFolder();

            ShopinfoView.ErrContent = errContent;
            FinanceView.ErrContent = errContent;
            PaymentView.ErrContent = errContent;
            ShippingView.ErrContent = errContent;
            OpenidParagrafView.ErrContent = errContent;
            NotifyParagrafView.ErrContent = errContent;

            if (!IsPostBack)
                SetActiveView();

            if (CurrentDiv != ActiveDiv.Final)
            {
                btnNext.Visible = _hasWriteAccess;
                btnBack.Visible = CurrentDiv != ActiveDiv.Shopinfo && CurrentDiv != ActiveDiv.TrialSelect;
                btnGoToShop.Visible = false;
            }
            else
            {
                btnNext.Visible = false;
                btnBack.Visible = false;
                btnGoToShop.Visible = true;
            }
        }

        protected string GetLeftMenu()
        {
            var strHtml = new StringBuilder();
            const string tempalte = "<div class='step{0}' onclick=\"{3}\"><div class='{1}'>{2}</div></div>";
            for (int i = 0; i < MenuItems.Count; i++)
            {
                if (MenuItems[i].Div < CurrentDiv)
                    strHtml.Append(string.Format(tempalte, " post", MenuItems[i].StyleClass, MenuItems[i].MenuName,
                        "location.href = '" + UrlService.GetAbsoluteLink("install/default.aspx") + "?step=" +
                        MenuItems[i].Div + "'"));
                else if (MenuItems[i].Div == CurrentDiv)
                    strHtml.Append(string.Format(tempalte, " active", MenuItems[i].StyleClass, MenuItems[i].MenuName,
                        "return false;"));
                else
                    strHtml.Append(string.Format(tempalte, string.Empty, MenuItems[i].StyleClass, MenuItems[i].MenuName,
                        "return false;"));
            }
            return strHtml.ToString();
        }

        protected void NextClick(object sender, EventArgs e)
        {
            if (CurrentDiv != ActiveDiv.Final)
            {
                switch (CurrentDiv)
                {
                    case ActiveDiv.TrialSelect:
                        if (TrialSelectView.IsExpressInstall())
                            GoShopClick(sender, e);
                        break;
                    case ActiveDiv.Shopinfo:
                        if (!ShopinfoView.Validate()) return;
                        ShopinfoView.SaveData();
                        if (!(Demo.IsDemoEnabled || TrialService.IsTrialEnabled || SaasDataService.IsSaasEnabled))
                        {
                            if (!ShopinfoView.ActiveLic)
                            {
                                ShopinfoView.LoadData();
                                return;
                            }
                        }
                        break;
                    case ActiveDiv.Finance:
                        if (!FinanceView.Validate()) return;
                        FinanceView.SaveData();
                        break;
                    case ActiveDiv.Payment:
                        if (!PaymentView.Validate()) return;
                        PaymentView.SaveData();
                        break;
                    case ActiveDiv.Shipping:
                        if (!ShippingView.Validate()) return;
                        ShippingView.SaveData();
                        break;
                    case ActiveDiv.OpenidParagraf:
                        if (!OpenidParagrafView.Validate()) return;
                        OpenidParagrafView.SaveData();
                        break;
                    case ActiveDiv.NotifyParagraf:
                        if (!NotifyParagrafView.Validate()) return;
                        NotifyParagrafView.SaveData();
                        break;
                    case ActiveDiv.Final:
                        return;
                }

                var temp = (int) CurrentDiv;
                temp++;
                Response.Redirect(UrlService.GetAbsoluteLink("install/default.aspx") + "?step=" +
                                  ((ActiveDiv) temp).ToString());
            }
        }

        protected void BackClick(object sender, EventArgs e)
        {
            if (CurrentDiv != ActiveDiv.Shopinfo)
            {
                var temp = (int) CurrentDiv;
                temp--;
                Response.Redirect(UrlService.GetAbsoluteLink("install/default.aspx") + "?step=" +
                                  ((ActiveDiv) temp).ToString());
            }
        }

        protected void GoShopClick(object sender, EventArgs e)
        {
            FileHelpers.CreateFile(SettingsGeneral.InstallFilePath);
            Response.Redirect("~/");
        }

        private void SetActiveView()
        {
            switch (CurrentDiv)
            {

                case ActiveDiv.TrialSelect:
                    views.SetActiveView(ViewTrialSelect);
                    break;
                case ActiveDiv.Shopinfo:
                    ShopinfoView.LoadData();
                    views.SetActiveView(ViewShopinfo);
                    break;
                case ActiveDiv.Finance:
                    FinanceView.LoadData();
                    views.SetActiveView(ViewFinance);
                    break;
                case ActiveDiv.Payment:
                    PaymentView.LoadData();
                    views.SetActiveView(ViewPayment);
                    break;
                case ActiveDiv.Shipping:
                    ShippingView.LoadData();
                    views.SetActiveView(ViewShipping);
                    break;
                case ActiveDiv.OpenidParagraf:
                    OpenidParagrafView.LoadData();
                    views.SetActiveView(ViewOpenidParagraf);
                    break;
                case ActiveDiv.NotifyParagraf:
                    NotifyParagrafView.LoadData();
                    views.SetActiveView(ViewNotifyParagraf);
                    break;
                case ActiveDiv.Final:
                    views.SetActiveView(ViewFinal);
                    break;
            }
        }

        private bool CheckAppDataFolder()
        {
            bool allowWrite = false;

            string fullPath = HttpContext.Current.Server.MapPath("~/App_Data/");
            string testFileName = fullPath + "testFile";

            try
            {
                File.Create(testFileName).Close();
                File.Delete(testFileName);
                allowWrite = true;
            }
            catch (Exception)
            {
            }

            return allowWrite;
        }
    }
}