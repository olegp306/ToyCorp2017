using System.Threading;
using AdvantShop.Localization;

namespace AdvantShop.Core.Extensions
{
    public static class Threads
    {
        static public Thread SetCulture(this Thread val, string lang = "")
        {
            var culture = Culture.CurrentCulture(lang);
            val.CurrentCulture = culture;
            val.CurrentUICulture = culture;
            return val;
        }
    }
}