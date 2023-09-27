using DesktopActiveService.Helper;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace DesktopActiveService
{
    public partial class CleanTempPathService : ServiceBase
    {
        public CleanTempPathService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            CleanTempPath();
        }

        protected override void OnStop()
        {
        }

        /// <summary>
        /// Clean temporary path
        /// </summary>
        internal void CleanTempPath()
        {
            // The excution interval in seconds
            var excutionInterval = AppConfigHelper.GetSettingsValueByKey(
                AppConfigHelper.SectionName_CleanTempPathSttings,
                AppConfigHelper.CleanTempPathSttings_ExcutionInterval);
            var intExcutionInterval = int.Parse(excutionInterval);

            // Keep time in seconds
            var keepTime = AppConfigHelper.GetSettingsValueByKey(
                AppConfigHelper.SectionName_CleanTempPathSttings,
                AppConfigHelper.CleanTempPathSttings_KeepTime);
            var intKeepTime = int.Parse(keepTime);

            // Execute condition
            var startTime = DateTime.UtcNow;
            bool executeCondition()
            {
                var hasExecutedTimes = (DateTime.UtcNow - startTime).TotalSeconds;
                intKeepTime = 0 < intKeepTime ? intKeepTime : 3600;
                return hasExecutedTimes <= intKeepTime;
            }

            // Clean temporary path is executed every 180 seconds.
            do
            {
                ProcessHelper.OpenFolderAndDoSomething(Path.GetTempPath(), CleanTempPathCore);
                Thread.Sleep(intExcutionInterval * 1000);
            }
            while (executeCondition());
        }

        /// <summary>
        /// Clean temporary path
        /// </summary>
        private void CleanTempPathCore()
        {
            // Get temporary directory
            var tempDirectory = Path.GetTempPath();

            // Focus
            SendKeys.SendWait("{HOME}");

            // Delete temporary directories
            var subDirectories = Directory.GetDirectories(tempDirectory);
            var subDirectorieList = subDirectories.ToList();
            subDirectorieList.Sort((x, y) => x.CompareTo(y));
            foreach (var subDirectory in subDirectorieList)
            {
                try
                {
                    Directory.Delete(subDirectory, true);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    if (!subDirectory.Equals(subDirectories.Last()))
                        SendKeys.SendWait("{DOWN}");
                    Thread.Sleep(1000);
                }
            }

            // Delete temporary files
            var subFiles = Directory.GetFiles(tempDirectory);
            var subFileList = subDirectories.ToList();
            subFileList.Sort((x, y) => x.CompareTo(y));
            foreach (var subFile in subFileList)
            {
                try
                {
                    File.Delete(subFile);
                }
                catch
                {
                    continue;
                }
                finally
                {
                    if (!subFile.Equals(subFiles.Last()))
                        SendKeys.SendWait("{DOWN}");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
