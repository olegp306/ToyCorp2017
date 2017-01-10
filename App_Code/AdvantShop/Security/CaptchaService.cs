using System.Collections.Generic;

namespace AdvantShop.Security
{
    public static class CaptchaService
    {
        private static readonly List<string> IPWithCaptha = new List<string>();

        public static void AddIPtoList(string ip)
        {
            if (!IPWithCaptha.Contains(ip))
            {
                IPWithCaptha.Add(ip);
            }
        }


        public static bool IsIPinList(string ip)
        {
            return IPWithCaptha.Contains(ip);
        }

        public static void RemoveIPfromList(string ip)
        {
            IPWithCaptha.Remove(ip);
        }

    }
}