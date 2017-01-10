//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

namespace AdvantShop.SaasData
{
    public class SaasData
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int ProductsCount { get; set; }
        public int PhotosCount { get; set; }
        public bool HaveExcel { get; set; }
        public bool Have1C { get; set; }
        public bool HaveExportFeeds { get; set; }
        public bool HavePriceRegulating { get; set; }
        public bool HaveBankIntegration { get; set; }
        public bool IsWork { get; set; }

        public int LeftDay { get; set; }
        public DateTime LastUpdate { get; set; }

        public decimal Money { get; set; }
        public decimal Bonus { get; set; }

        public bool IsCorrect { get; set; }

        public bool IsWorkingNow
        {
            get
            {
                return (IsWork && (Money >= 0));
            }
        }

        public SaasData()
        {
            Name = string.Empty;
            Price = 0;
            ProductsCount = 0;
            PhotosCount = 0;
            HaveExcel = false;
            Have1C = false;
            HaveExportFeeds = false;
            HavePriceRegulating = false;
            HaveBankIntegration = false;
            IsWork = false;
            LeftDay = 0;
            LastUpdate = DateTime.Now.AddDays(-7);
            Money = 0;
            Bonus = 0;
            IsCorrect = true;
        }

        public SaasData(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                IsCorrect = false;
                return;
            }

            if (parameters.ContainsKey(SaasDataTemplate.Error))
            {
                IsCorrect = false;
                return;
            }
            //throw new Exception("error with get saas");

            Name = parameters.ElementOrDefault(SaasDataTemplate.Name);
            ProductsCount = parameters.ElementOrDefault(SaasDataTemplate.ProductsCount).TryParseInt();
            PhotosCount = parameters.ElementOrDefault(SaasDataTemplate.PhotosCount).TryParseInt();
            HaveExcel = parameters.ElementOrDefault(SaasDataTemplate.HaveExcel).TryParseBool();
            Have1C = parameters.ElementOrDefault(SaasDataTemplate.Have1C).TryParseBool();
            HaveExportFeeds = parameters.ElementOrDefault(SaasDataTemplate.HaveExportFeeds).TryParseBool();
            HavePriceRegulating = parameters.ElementOrDefault(SaasDataTemplate.HavePriceRegulating).TryParseBool();
            HaveBankIntegration = parameters.ElementOrDefault(SaasDataTemplate.HaveBankIntegration).TryParseBool();

            IsWork = parameters.ElementOrDefault(SaasDataTemplate.IsWork).TryParseBool();
            LeftDay = parameters.ElementOrDefault(SaasDataTemplate.LeftDay).TryParseInt();
            Money = parameters.ElementOrDefault(SaasDataTemplate.Money) != null ? parameters.ElementOrDefault(SaasDataTemplate.Money).Replace(".", ",").TryParseDecimal() : 0;
            Bonus = parameters.ElementOrDefault(SaasDataTemplate.Bonus) != null ? parameters.ElementOrDefault(SaasDataTemplate.Bonus).Replace(".", ",").TryParseDecimal() : 0;
            LastUpdate = parameters.ElementOrDefault(SaasDataTemplate.LastUpdate).TryParseDateTime(DateTime.MinValue, CultureInfo.InvariantCulture, DateTimeStyles.None);
            IsCorrect = true;
        }
    }
}
