namespace DesktopActiveService.Const
{
    internal class ServiceCommand
    {
        public const string ServiceName = "DesktopActiveService";
        public const string StopService = "sc stop {0}";
        public const string DeleteService = "sc delete {0}";
        public const string OpenInstallutilExeDiretory = "cd C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319";
        public const string CreateService = "InstallUtil.exe /i {0}";
        public const string StartService = "/c net start {0}";
    }
}
