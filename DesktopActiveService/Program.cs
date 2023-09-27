using DesktopActiveService.Const;
using DesktopActiveService.Helper;
using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Windows.Forms;

namespace DesktopActiveService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun = null;

            try
            {
                ServicesToRun = GetExecuteServices();
                InstallService();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to run DesktopActiveService. \nCause: {ex.Message}");
            }
            finally
            {
                ServiceBase.Run(ServicesToRun);
            }
        }

        /// <summary>
        /// Get execute services from App.config
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static ServiceBase[] GetExecuteServices()
        {
            var servicesToRun = new List<ServiceBase>();

            var wetherExecuteCleanTempPathService = WetherExecuteService(
                AppConfigHelper.AppSettingsKey_ExecuteCleanTempPathService);
            if (wetherExecuteCleanTempPathService)
            {
                var serviceItem = new CleanTempPathService();
                servicesToRun.Add(serviceItem);
            }

            if (servicesToRun.Count == 0)
                throw new Exception(ServiceMessage.NoServicesNeedToBeInstalled);

            return servicesToRun.ToArray();
        }

        /// <summary>
        /// Determine whether to execute by service name in App.config
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private static bool WetherExecuteService(string serviceName)
        {
            var wetherExecuteServiceValue = AppConfigHelper.GetAppSettingValueByKey(serviceName);
            bool.TryParse(wetherExecuteServiceValue, out var wetherExecute);

            return wetherExecute;
        }

        /// <summary>
        /// Install DesktopActiveService into the winodows service list
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static void InstallService()
        {
            // Search service
            var commands = new List<string>();
            var hasService = ProcessHelper.SearchWindowsService(ServiceCommand.ServiceName);
            if (!hasService)
            {
                // Install service
                var intallPath = ProcessHelper.GetInstallPath();
                commands.Add(string.Format(ServiceCommand.OpenInstallutilExeDiretory));
                commands.Add(string.Format(ServiceCommand.CreateService, intallPath));
                var executeStatus = ProcessHelper.ExecuteCommandsAsAdmin(commands.ToArray());
                var hasTargetService = ProcessHelper.SearchWindowsService(ServiceCommand.ServiceName);
                if (!executeStatus || !hasTargetService)
                {
                    throw new Exception(ServiceMessage.ServicesInstallFailed);
                }
            }

            //var serviceStatus = ProcessHelper.GetWindowsServiceStatus(ServiceCommand.ServiceName);
            //if (serviceStatus != ServiceControllerStatus.Running)
            //{
            //    // Start service
            //    commands.Clear();
            //    commands.Add(string.Format(ServiceCommand.StartService, ServiceCommand.ServiceName));
            //    var executeStatus = ProcessHelper.ExecuteCommandsAsAdmin(commands.ToArray());
            //    if (!executeStatus)
            //    {
            //        throw new Exception(ServiceMessage.ServicesStartFailed);
            //    }
            //}

            //serviceStatus = ProcessHelper.GetWindowsServiceStatus(ServiceCommand.ServiceName);
            //if (serviceStatus != ServiceControllerStatus.Running)
            //{
            //    throw new Exception(ServiceMessage.ServicesStartFailed);
            //}
        }
    }
}
