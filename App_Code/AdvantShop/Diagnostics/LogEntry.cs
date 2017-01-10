//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Diagnostics
{
    public class LogEntry
    {
        public string TimeStamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        //public string Details { get; set; }
        public string ErrorMessage { get; set; }
    }
}