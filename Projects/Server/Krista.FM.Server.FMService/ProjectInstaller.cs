using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace Krista.FM.Server.FMService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void InitializeInstaller()
        {
            try
            {
                serviceInstaller.ServiceName = InstallParametrs.Instance().ServiceName;

                serviceInstaller.ServicesDependedOn = InstallParametrs.Instance().GetServicesDependedOn();
            }
            catch (Exception e)
            {
            	throw new Exception("Ошибка инициализации инсталятора", e);
            }
        }

		public override void Install(IDictionary stateSaver)
        {
			Context.LogMessage("Установка службы...");
			Context.LogMessage(String.Format("ServiceName={0}", serviceInstaller.ServiceName));
			
			base.Install(stateSaver);

			// Code maybe written for installation of an application.
			Context.LogMessage("Установка службы завершена");
        }

		public override void Uninstall(IDictionary savedState)
        {
			Context.LogMessage("Удаление службы...");
			Context.LogMessage(String.Format("ServiceName={0}", serviceInstaller.ServiceName));

			base.Uninstall(savedState);
			
			Context.LogMessage("Служба удалена.");
        }
    }
}