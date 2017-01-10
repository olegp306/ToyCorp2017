//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace UserControls.Default
{
    public partial class VersionLabel : System.Web.UI.UserControl
    {
        private bool _visible = true;
        public override bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        protected string GetUptimeString()
        {
            TimeSpan ts = AdvantShop.Diagnostics.ApplicationUptime.GetUptime();
            return String.Format("{0} days, {1} hours, {2} minutes", ts.Days, ts.Hours, ts.Minutes);
        }
    }
}

