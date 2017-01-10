//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.SaasData;
using Newtonsoft.Json;

namespace AdvantShop.Trial
{
    public class TrialService
    {
        private const string UrlTrialInfo = "http://modules.advantshop.net/Trial/GetParams/{0}";
        private const string UrlTrialEvents = "http://cap.advantshop.net/Achivements/Event/LogEvent?licKey={0}&eventName={1}&eventParams={2}";

        private const string UrlGetAchievements = "http://cap.advantshop.net/Achivements/Shop/GetLicenseAchievement?licKey={0}";
        private const string UrlAchievementsGetPoints = "http://cap.advantshop.net/Achivements/Shop/GetLicensePoints?licKey={0}";
        private const string UrlGetAchievementsDescription = "http://cap.advantshop.net/Achivements/Shop/GetAchievementsDescription?licKey={0}";
        private const string UrlGetAchievementsPopUp = "http://cap.advantshop.net/Achivements/Shop/GetAchievementsPopUp?licKey={0}";


        private static DateTime _lastUpdate;

        private static DateTime _trialTillCached = DateTime.MinValue;

        public static bool IsTrialEnabled
        {
            get { return ModeConfigService.IsModeEnabled(ModeConfigService.Modes.TrialMode); }
        }


        public static DateTime TrialPeriodTill
        {
            get
            {
                if (DateTime.Now > _lastUpdate.AddHours(1))
                {
                    try
                    {
                        var request = WebRequest.Create(string.Format(UrlTrialInfo, SettingsLic.LicKey));
                        request.Method = "GET";

                        using (var dataStream = request.GetResponse().GetResponseStream())
                        {
                            using (var reader = new StreamReader(dataStream))
                            {
                                var responseFromServer = reader.ReadToEnd();
                                if (!string.IsNullOrEmpty(responseFromServer))
                                {
                                    _trialTillCached = JsonConvert.DeserializeObject<DateTime>(responseFromServer);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                    _lastUpdate = DateTime.Now;
                }
                return _trialTillCached;
            }
        }

        public static void TrackEvent(TrialEvents trialEvent, string eventParams)
        {
            if (IsTrialEnabled || SaasDataService.IsSaasEnabled)
            {
                new System.Threading.Tasks.Task(() =>
                    {
                        try
                        {
                            new WebClient().DownloadString(
                                string.Format(UrlTrialEvents, SettingsLic.LicKey, trialEvent.ToString(),
                                              HttpUtility.UrlEncode(eventParams)));
                            UpdateAchievements();
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex, false);
                        }
                    }).Start();

                if (HttpContext.Current.Session["TrialEvents"] == null)
                {
                    HttpContext.Current.Session["TrialEvents"] = new List<KeyValuePair<TrialEvents, string>>();
                }
                ((List<KeyValuePair<TrialEvents, string>>)HttpContext.Current.Session["TrialEvents"]).Add(
                    new KeyValuePair<TrialEvents, string>(trialEvent, eventParams));

            }
        }

        public static void UpdateAchievements()
        {
            try
            {
                SettingsMain.Achievements = new WebClient() { Encoding = Encoding.UTF8 }
                    .DownloadString(string.Format(UrlGetAchievements, SettingsLic.LicKey))
                    .Replace("#SHOP_URL_LINK#", UrlService.GetAbsoluteLink("/"))
                    .Replace("#SHOP_URL#", SettingsMain.SiteUrl);

                SettingsMain.AchievementsPoints = new WebClient() { Encoding = Encoding.UTF8 }
                    .DownloadString(string.Format(UrlAchievementsGetPoints, SettingsLic.LicKey));

                SettingsMain.AchievementsDescription = JsonConvert.DeserializeObject<string>(new WebClient() { Encoding = Encoding.UTF8 }
                    .DownloadString(string.Format(UrlGetAchievementsDescription, SettingsLic.LicKey)).Replace("#LICENCE_KEY#", SettingsLic.LicKey));


                SettingsMain.AchievementsPopUp = JsonConvert.DeserializeObject<string>(new WebClient() { Encoding = Encoding.UTF8 }
                    .DownloadString(string.Format(UrlGetAchievementsPopUp, SettingsLic.LicKey)));
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, false);
            }

        }

        public static List<AchievementLevel> GetAchievements()
        {
            UpdateAchievements();

            try
            {
                return JsonConvert.DeserializeObject<List<AchievementLevel>>(SettingsMain.Achievements);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, false);
                return new List<AchievementLevel>();
            }
        }

        public static Achievement GetAchievementHelp(int id)
        {

            List<AchievementLevel> achievementLevels = TrialService.GetAchievements();

            return achievementLevels.SelectMany(level => level.Achievements.Where(ach => ach.Id == id)).FirstOrDefault();
        }
    }
}