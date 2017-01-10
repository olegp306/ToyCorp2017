//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Configuration;
using AdvantShop.Core.Attributes;

namespace AdvantShop.ExportImport
{
    public enum EncodingsEnum
    {
        [StringName("Windows-1251")]
        Windows1251,
        [StringName("UTF-8")]
        Utf8,
        [StringName("UTF-16")]
        Utf16,
        [StringName("KOI8-R")]
        Koi8R
    }

    public enum SeparatorsEnum
    {
        [StringName(",")]
        [ResourceKey("Admin_ImportCsv_Comma")]
        CommaSeparated,
        [StringName("\t")]
        [ResourceKey("Admin_ImportCsv_Tab")]
        TabSeparated,
        [StringName(";")]
        [ResourceKey("Admin_ImportCsv_Semicolon")]
        SemicolonSeparated,
        [StringName("")]
        [ResourceKey("Admin_ImportCsv_Custom")]
        Custom,
    }

    public class CsvSettings
    {
        public static string CsvSeparator
        {
            get { return SettingsGeneral.CsvSeparator; }
            set { SettingsGeneral.CsvSeparator = value; }
        }

        public static string CsvEnconing
        {
            get { return SettingsGeneral.CsvEnconing; }
            set { SettingsGeneral.CsvEnconing = value; }
        }

        public static string CsvColumSeparator
        {
            get { return SettingsGeneral.CsvColumSeparator; }
            set { SettingsGeneral.CsvColumSeparator = value; }
        }

        public static string CsvPropertySeparator
        {
            get { return SettingsGeneral.CsvPropertySeparator; }
            set { SettingsGeneral.CsvPropertySeparator = value; }
        }

        public static bool CsvExportNoInCategory
        {
            get { return SettingsGeneral.CsvExportNoInCategory; }
            set { SettingsGeneral.CsvExportNoInCategory = value; }
        }
    }
}