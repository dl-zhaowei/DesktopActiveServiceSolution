using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace DesktopActiveService.Helper
{
    internal class ProcessHelper
    {
        #region Open folder and dosomething

        // Import Windows API functions
        // 导入Windows API函数
        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(uint esFlags);

        // defconstant
        const uint ES_CONTINUOUS = 0x80000000;
        const uint ES_SYSTEM_REQUIRED = 0x00000001;

        public static void OpenFolderAndDoSomething(string folderPath, Action doSomething)
        {
            // Set thread execution state to prevent computer screen lock and sleep
            // 设置线程执行状态，防止计算机锁屏和休眠
            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);

            Process process = new Process();
            process.StartInfo.FileName = folderPath;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.Start();
            doSomething.Invoke();
            process.Close();

            // Restore the default thread execution state
            // 恢复默认线程执行状态
            SetThreadExecutionState(ES_CONTINUOUS);
        }

        #endregion

        #region Install service

        public static bool ExecuteCommandsAsAdmin(string[] commands)
        {
            try
            {
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Verb = "runas",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                    }
                };

                process.Start();

                // 获取标准输入流，并将要执行的指令写入
                using (var sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        foreach (var command in commands)
                        {
                            sw.WriteLine(command);
                            Thread.Sleep(2 * 1000);
                        }
                    }
                }
                process.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Get install path

        public static string GetInstallPath()
        {
#if DEBUG
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.Location;
#else
            var programFilesFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var manufacturer = GetManufacturerFromAssembly();
            var productName = GetProductNameFromAssembly();

            // 完整的安装路径
            var installationPath = Path.Combine(programFilesFolder, manufacturer, productName);
            return installationPath;
#endif
        }

#if !DEBUG
        private static string GetManufacturerFromAssembly()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var productAttribute = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            return productAttribute?.Product ?? "Manufacturer";
        }

        private static string GetProductNameFromAssembly()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var titleAttribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>();
            return titleAttribute?.Title ?? "ProductName";
        }
#endif

        #endregion

        #region Search windows service

        public static bool SearchWindowsService(string serviceName)
        {
            var services = ServiceController.GetServices();
            var targetService = Array.Find(services,
                service => service.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            return targetService != null;
        }

        public static ServiceControllerStatus GetWindowsServiceStatus(string serviceName)
        {
            var services = ServiceController.GetServices();
            var targetService = Array.Find(services,
                service => service.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            return targetService.Status;
        }

        #endregion
    }
}
