using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Helpers;
using SquishIt.Framework;

namespace AdvantShop.Core
{
    public class JsCssTool
    {
        private const string CacheKeyPrefix = "squishit_";

        public static void ReCreateIfNotExist()
        {
            if (!FileHelpers.IsDirectoryHaveFiles(SettingsGeneral.AbsolutePath + "/combine"))
            {
                CacheManager.RemoveByPattern(CacheKeyPrefix);
            }
        }

        public static void Clear()
        {
            FileHelpers.DeleteFilesFromPath(SettingsGeneral.AbsolutePath + "/combine");
        }

        public static string MiniCss(IList<string> paths, string filename = "combined.css")
        {
            var outputfile = "~/combine/" + filename + "?#";

            FileHelpers.CreateDirectory(SettingsGeneral.AbsolutePath + "/combine");
            var temp = Bundle.Css();
            foreach (var item in paths)
                temp.Add(item);
            return temp.WithMinifier(new SquishIt.Framework.Minifiers.CSS.MsMinifier()).Render(outputfile); // .ForceRelease()
        }

        public static string MiniJs(IList<string> paths, string filename = "combined.js")
        {
            var outputfile = "~/combine/" + filename + "?#";

            FileHelpers.CreateDirectory(SettingsGeneral.AbsolutePath + "/combine");
            var temp = Bundle.JavaScript();
            foreach (var item in paths)
                temp.Add(item);
            return temp.WithMinifier(new SquishIt.Framework.Minifiers.JavaScript.MsMinifier()).Render(outputfile); // .ForceRelease()
        }
    }
}