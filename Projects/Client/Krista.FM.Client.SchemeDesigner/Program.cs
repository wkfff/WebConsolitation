using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common.Services;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeDesigner
{
    static class Program
    {
        static IScheme ConnectToScheme(ref bool differentVersionsMode, ref string connectionErrorMessage)
        {
            string ServerName = "";
            string SchemeName = "";
            string Login = "";
            string Password = "";
            IScheme scheme = null;
            AuthenticationType _authType = AuthenticationType.atWindows;

            // Настройка среды .NET Remoting
            RemotingConfiguration.Configure(AppDomain.CurrentDomain.FriendlyName + ".config", false);

            while (true)
            {
                // показываем окно с диалогом подключения к схеме
                if (frmLogon.ShowLogonForm(ref ServerName, ref SchemeName, ref Login, ref Password,
                    ref scheme, ref differentVersionsMode, ref _authType, ref connectionErrorMessage))
                {
                    if (scheme != null || differentVersionsMode)
                    {
                        // если успешно подключились - создаем воркплайс
                        return scheme;
                        // ... и выходим из цикла
                    }
                    else
                    {
                        // если нет - предлагаем поключится заново
                        if (MessageBox.Show("Не удалось подключиться к схеме. Повторить?", "Ошибка подключения",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No) break;
                    }
                }
                // если пользователь не захотел подключаться - выходим из цикла
                else break;
            }
            // в текущей конфигурации без схемы мы работать не можем
            return null;
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            // русифицируем используемые компоненты Infragistics
            InfragisticsRusification.LocalizeAll();

            // Назначаем обработчик необработанных исключений нити Workplace
        	Application.ThreadException += UnhandledExceptionHandler.OnThreadException;

            ResourceService.InitializeService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Krista.FM.Client.Common.Resources.Loader.Initialize();

            bool differentVersionsMode = false;
            string connectionErrorMessage = null;
            IScheme scheme = ConnectToScheme(ref differentVersionsMode, ref connectionErrorMessage);

            if (scheme != null)
            {
                UnhandledExceptionHandler.Scheme = scheme;
                SchemeDesigner schemeDesigner = new SchemeDesigner(scheme, differentVersionsMode, connectionErrorMessage);
                if (schemeDesigner.InitializeSchemeDesigner())
                {
                    Application.Run(schemeDesigner);
                }
            }
        }
    }
}