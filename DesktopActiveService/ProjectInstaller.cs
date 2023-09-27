using DesktopActiveService.Const;
using System.ComponentModel;
using System.ServiceProcess;

namespace DesktopActiveService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();
            Install();
        }

        public void Install()
        {
            // 示例代码
            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            // 设置属性
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.ServiceName = ServiceCommand.ServiceName;

            // 添加到 Installers 集合中
            Installers.AddRange(
                new System.Configuration.Install.Installer[] {
                    serviceProcessInstaller,
                    serviceInstaller });
        }
    }
}