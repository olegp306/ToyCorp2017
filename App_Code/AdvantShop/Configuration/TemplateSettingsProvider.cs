//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Design;
using AdvantShop.Diagnostics;

namespace AdvantShop.Configuration
{
    public class TemplateSettingsProvider
    {
        public const string TemplateFileConfigName = "template.config";

        public sealed class TemplateSettingIndexer
        {
            public string this[string name]
            {
                get { return GetSettingValue(name); }
                set { SetSettingValue(name, value); }
            }
        }

        private static TemplateSettingIndexer _staticIndexer;
        public static TemplateSettingIndexer Items
        {
            get { return _staticIndexer ?? (_staticIndexer = new TemplateSettingIndexer()); }
        }

        #region Get/Set settings value

        public static string GetSettingValue(string strName)
        {
            string strRes = null;

            string strCacheName = CacheNames.GetTemplateSettingsCacheObjectName(SettingsDesign.Template, strName);
            if (CacheManager.Contains(strCacheName))
            {
                strRes = CacheManager.Get<string>(strCacheName);
            }
            else
            {
                var settings = GetTemplateSettings();
                var setting = settings.FirstOrDefault(s => s.Name == strName);
                if (setting != null)
                {
                    strRes = setting.Value;
                }
            }

            return strRes;
        }

        public static bool SetSettingValue(string strName, string strValue)
        {
            var boolReult = SetTemplateSetting(strName, strValue);

            // Add into cahce
            if (boolReult)
            {
                string strCacheName = CacheNames.GetTemplateSettingsCacheObjectName(SettingsDesign.Template, strName);
                CacheManager.Insert(strCacheName, strValue);
            }

            return boolReult;
        }     
   
        #endregion

        #region Get/Set settings service

        public static List<TemplateSetting> GetTemplateSettings()
        {
            var settings = new List<TemplateSetting>();

            string templateConfigFile = SettingsGeneral.AbsolutePath
                                            + (SettingsDesign.Template != TemplateService.DefaultTemplateId ? ("templates\\" + SettingsDesign.Template + "\\") : "App_Data\\")
                                            + TemplateFileConfigName;

            if (!File.Exists(templateConfigFile))
                return settings;

            try
            {
                var doc = XDocument.Load(templateConfigFile);

                foreach (var elSetting in doc.Root.Elements().Elements("Setting"))
                {
                    var setting = new TemplateSetting
                                    {
                                        Name = elSetting.Attribute("Name").Value,
                                        Value = elSetting.Element("Value").Value
                                    };
                    settings.Add(setting);

                    // Add to cache
                    string strCacheName = CacheNames.GetTemplateSettingsCacheObjectName(SettingsDesign.Template, setting.Name);
                    CacheManager.Insert(strCacheName, setting.Value);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return settings;
        }

        public static bool SetTemplateSetting(string strName, string strValue)
        {
            string templateConfigFile = SettingsGeneral.AbsolutePath
                                            + (SettingsDesign.Template != TemplateService.DefaultTemplateId ? ("templates\\" + SettingsDesign.Template + "\\") : "App_Data\\")
                                            + TemplateFileConfigName;

            if (!File.Exists(templateConfigFile))
                return false;

            try
            {
                var doc = XDocument.Load(templateConfigFile);

                var setting = doc.Root.Elements().Elements("Setting").FirstOrDefault(s => s.Attribute("Name").Value == strName);
                if (setting != null)
                {
                    setting.Element("Value").Value = strValue;
                    doc.Save(templateConfigFile);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return false;
        }

        /// <summary>
        /// Get localized template settings
        /// </summary>
        /// <returns></returns>
        public static TemplateSettingBox GetTemplateSettingsBox()
        {
            var settingsBox = new TemplateSettingBox() {  Settings = new List<TemplateSetting>() };


            string templateConfigFile = SettingsGeneral.AbsolutePath
                                            + (SettingsDesign.Template != TemplateService.DefaultTemplateId ? ("templates\\" + SettingsDesign.Template + "\\") : "App_Data\\")
                                            + TemplateFileConfigName;

            if (!File.Exists(templateConfigFile))
            {
                settingsBox.Message = Resource.Admin_TemplateSettings_ConfigNotExist;
                return settingsBox;
            }

            try
            {
                var settings = settingsBox.Settings;
                var resourceManager = new ResourceManager(typeof(Resource));

                var doc = XDocument.Load(templateConfigFile);

                foreach (var elSection in doc.Root.Elements())
                {
                    string sectionName = resourceManager.GetString("Admin_TemplateSettings_" + elSection.Attribute("Title").Value).Default(elSection.Attribute("Title").Value);

                    if (elSection.Attribute("Hidden") != null && Convert.ToBoolean(elSection.Attribute("Hidden").Value))
                        continue;

                    foreach (var elSetting in elSection.Elements())
                    {
                        var setting = new TemplateSetting
                                        {
                                            Name = elSetting.Attribute("Name").Value,
                                            Type = elSetting.Attribute("Type").Value,
                                            Value = elSetting.Element("Value").Value,
                                            SectionName = sectionName
                                        };

                        setting.Title = elSetting.Attribute("Title") != null
                                                ? resourceManager.GetString("Admin_TemplateSettings_" + elSetting.Attribute("Title").Value).Default(elSetting.Attribute("Title").Value)
                                                : resourceManager.GetString("Admin_TemplateSettings_" + setting.Name).Default(setting.Name);

                        var options = new List<TemplateOptionSetting>();
                        foreach (var elOption in elSetting.Elements("option"))
                        {
                            string title = elOption.Attribute("Title").Value;

                            options.Add(new TemplateOptionSetting
                            {
                                Title = resourceManager.GetString("Admin_TemplateSettings_" + title).Default(title),
                                Value = elOption.Attribute("Value").Value
                            });
                        }
                        setting.Options = options;
                        settings.Add(setting);
                    }
                }
            }
            catch (Exception ex)
            {
                settingsBox.Message = Resource.Admin_TemplateSettings_ErrorReadConfig;
                Debug.LogError(ex);
            }

            return settingsBox;
        }
        
        #endregion
    }
}