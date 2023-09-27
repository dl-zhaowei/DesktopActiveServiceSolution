using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace DesktopActiveService.Helper
{
    internal class AppConfigHelper
    {
        public const string AppSettingsKey_ExecuteCleanTempPathService = "ExecuteCleanTempPathService";

        public const string SectionName_CleanTempPathSttings = "CleanTempPathSttings";
        public const string CleanTempPathSttings_ExcutionInterval = "ExcutionInterval";
        public const string CleanTempPathSttings_KeepTime = "KeepTime";

        public static string GetAppSettingValueByKey(string key)
        {
            var hasKey = ConfigurationManager.AppSettings.AllKeys.Contains(key);
            if (!hasKey)
                throw new SettingsPropertyNotFoundException($"Not found settings property '{key}'!");
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetSettingsValueByKey(string section, string key)
        {
            var configSection = ConfigurationManager.GetSection(section) as NameValueCollection;
            if (configSection is null)
                throw new SettingsPropertyNotFoundException($"Not found section '{section}'!");

            var configKeyValue = configSection.Get(key);
            if (configKeyValue is null)
                throw new SettingsPropertyNotFoundException($"Not found settings property '{key}'!");

            return configKeyValue;
        }
    }
}
