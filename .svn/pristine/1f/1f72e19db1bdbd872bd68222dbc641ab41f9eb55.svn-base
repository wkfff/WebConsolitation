using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Remoting;
using System.Security.Permissions;
using System.ServiceProcess;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.FMService
{
    public partial class FMService : ServiceBase
    {
        private IServer Server;


        public FMService()
        {
            InitializeComponent();
        }

		[SecurityPermission(SecurityAction.LinkDemand)]
		protected override void OnStart(string[] args)
        {
            try
            {
                // путь к файлу с параметрами запуска
                InstallParametrs.SetParametersBaseDirectory(AppDomain.CurrentDomain.BaseDirectory);

                // Основная служба
                ServiceController sc = new ServiceController(InstallParametrs.Instance().ServiceName);

                // Зависимости
                foreach (ServiceController scc in sc.ServicesDependedOn)
                {
                    if (scc.Status != ServiceControllerStatus.StartPending &&
                        scc.Status != ServiceControllerStatus.Running)
                        scc.Start();

                    // запуск зависимой службы
                    scc.WaitForStatus(ServiceControllerStatus.Running);
                }

                // URL объекта сервера приложений.
                string URL = String.Format(CultureInfo.InvariantCulture, "tcp://{0}:{1}/FMServer/Server.rem", Environment.MachineName, ConfigurationManager.AppSettings["ServerPort"]);

                // Настройка среды .NET Remoting
                RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);

                // Необходимо чтобы серверный объект создавался и настраивался при запуске
                // сервера приложений, а не при первом обращении клиента. 
                // Именно это здесь и делаем - создаем удаленный объект средствами .NET Remoting
                Server = (IServer)Activator.GetObject(typeof(IServer), URL);
                Trace.TraceVerbose("Proxi server object created");

                Trace.TraceInformation("Инициализация сервера");
                // Прокси объект сервера создан. Теперь надо произвести первое обащение
                // к удаленному объекту для того, чтобы он реально был создан на стороне сервера,
                // :) т.е. здесь же в текущем домене :)
                // Собственно делаем первое обащение к объекту сервера
                Trace.Indent();
                Server.Activate();
                Trace.Unindent();
                

                // А вот теперь серверный объект создан и ни куда не денется от нас,
                // если конечно мы ему корректно настроим таймаут
                Trace.TraceInformation("Инициализация сервера завершена");
            }
            catch (Exception e)
            {
                Trace.TraceError("Необработанная ошибка во время инициализации сервера: {0}", e.ToString());
            	throw;
            }
        }

        protected override void OnStop()
        {
			Server = null;
            Trace.TraceVerbose("OnStop");

			foreach (TraceListener listener in System.Diagnostics.Trace.Listeners)
			{
				listener.Flush();
				listener.Close();
			}
		}
    }
}
