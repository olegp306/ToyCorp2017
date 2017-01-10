using System;
using AdvantShop.Configuration;
using AdvantShop.Helpers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;

namespace AdvantShop.BonusSystem
{
    public class BonusSystem
    {
        public enum EBonusType
        {
            ByProductsCostWithShipping = 0,
            ByProductsCost = 1
        }

        public static string ApiKey
        {
            get { return SQLDataHelper.GetString(SettingProvider.Items["BonusSystem.ApiKey"]); }
            set { SettingProvider.Items["BonusSystem.ApiKey"] = value; }
        }

        public static bool IsActive
        {
            get
            {
                var bonusModule = AttachedModules.GetModules<IBonusSystem>();
                return bonusModule != null && bonusModule.Count > 0;
            }
        }

        public static EBonusType BonusType
        {
            get { return (EBonusType) Convert.ToInt32(SettingProvider.Items["BonusSystem.BonusType"]); }
            set { SettingProvider.Items["BonusSystem.BonusType"] = ((int) value).ToString(); }
        }

        public static float BonusFirstPercent
        {
            get { return BonusSystemService.GetBonusDefaultPercent(); }
        }

        public static float MaxOrderPercent
        {
            get { return SQLDataHelper.GetFloat(SettingProvider.Items["BonusSystem.MaxOrderPercent"]); }
            set { SettingProvider.Items["BonusSystem.MaxOrderPercent"] = value.ToString(); }
        }

        public static float BonusesForNewCard
        {
            get { return SQLDataHelper.GetFloat(SettingProvider.Items["BonusSystem.BonusesForNewCard"]); }
            set { SettingProvider.Items["BonusSystem.BonusesForNewCard"] = value.ToString(); }
        }


        public static bool UseOrderId
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["BonusSystem.UseOrderId"]); }
            set { SettingProvider.Items["BonusSystem.UseOrderId"] = value.ToString(); }
        }
    }
}