//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Mails
{
    public class MailFormat
    {
        public int MailFormatID { get; set; }

        public string FormatName { get; set; }

        public string FormatSubject { get; set; }

        public string FormatText { get; set; }

        public MailType FormatType { get; set; }

        public int SortOrder { get; set; }

        public bool Enable { get; set; }

        public DateTime AddDate { get; set; }

        public DateTime ModifyDate { get; set; }
    }
}
