//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.SaasData;
using AdvantShop.Trial;
using Google.GData.Analytics;

namespace AdvantShop.SEO
{
    public static class GoogleAnalyticsService
    {
        private const string DataFeedUrl = "https://www.google.com/analytics/feeds/data?key=";

        public static DateTime GetLastModifiedDate()
        {
            return SettingsSEO.GoogleAnalyticsCachedDate;
        }


        public static Dictionary<DateTime, GoogleAnalyticsData> GetData()
        {
            if (!SettingsSEO.GoogleAnalyticsApiEnabled || SettingsSEO.GoogleAnalyticsUserName.IsNullOrEmpty() ||
                SettingsSEO.GoogleAnalyticsPassword.IsNullOrEmpty() || SettingsSEO.GoogleAnalyticsAPIKey.IsNullOrEmpty())
            {
                return null;
            }

            if (SettingsSEO.GoogleAnalyticsCachedDate == DateTime.MinValue || SettingsSEO.GoogleAnalyticsCachedDate.AddHours(1) <= DateTime.Now)
            {
                var service = new AnalyticsService("WebApp");
                service.setUserCredentials(SettingsSEO.GoogleAnalyticsUserName, SettingsSEO.GoogleAnalyticsPassword);

                if (TrialService.IsTrialEnabled || SaasDataService.IsSaasEnabled)
                {
                    if (GetTotalVisitors(service) > 100)
                    {
                        TrialService.TrackEvent(TrialEvents.GetFirstThouthandVisitors, string.Empty);
                    }
                }

                var data = GetVisitsData(service);
                if (data != null)
                    return data;
            }


            // if something is wrong return cached data
            if (SettingsSEO.GoogleAnalyticsCachedData.IsNullOrEmpty())
                return null;

            try
            {
                var cachedData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<DateTime, GoogleAnalyticsData>>(SettingsSEO.GoogleAnalyticsCachedData);
                return cachedData;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, false);
                return null; // if there is no any data
            }
        }

        private static int GetTotalVisitors(AnalyticsService service)
        {
            var totalVisitorsQuerry = new DataQuery(DataFeedUrl + SettingsSEO.GoogleAnalyticsAPIKey)
                {
                    Ids = "ga:" + SettingsSEO.GoogleAnalyticsAccountID,
                    Metrics = "ga:visitors",
                    Dimensions = "ga:year",
                    GAStartDate = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd"),
                    GAEndDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    StartIndex = 1
                };

            try
            {
                var totalVisitors = service.Query(totalVisitorsQuerry);
                return totalVisitors.Aggregates.Metrics[0].Value.TryParseInt();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, false);
            }
            return 0;
        }


        private static Dictionary<DateTime, GoogleAnalyticsData> GetVisitsData(AnalyticsService service)
        {
            var data = new Dictionary<DateTime, GoogleAnalyticsData>();

            var query = new DataQuery(DataFeedUrl + SettingsSEO.GoogleAnalyticsAPIKey)
            {
                Ids = "ga:" + SettingsSEO.GoogleAnalyticsAccountID,
                Metrics = "ga:visitors,ga:visits,ga:pageviews",
                Dimensions = "ga:date",
                GAStartDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"),
                GAEndDate = DateTime.Now.ToString("yyyy-MM-dd"),
                StartIndex = 1
            };

            // try to get actual data from google

            try
            {
                var dataFeedVisits = service.Query(query);
                if (dataFeedVisits != null)
                {
                    foreach (DataEntry entry in dataFeedVisits.Entries)
                    {
                        data.Add(DateTime.ParseExact(entry.Title.Text.Split('=').LastOrDefault(), "yyyyMMdd", CultureInfo.InvariantCulture),
                                 new GoogleAnalyticsData
                                 {
                                     Visitors = entry.Metrics[0].Value.TryParseInt(),
                                     Visits = entry.Metrics[1].Value.TryParseInt(),
                                     PageViews = entry.Metrics[2].Value.TryParseInt(),
                                 });
                    }
                }

                // saving and return data if all is ok
                SettingsSEO.GoogleAnalyticsCachedData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                SettingsSEO.GoogleAnalyticsCachedDate = DateTime.Now;
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, false);
            }
            return null;
        }
    }
}