using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Configuration;

using Krista.FM.ServerLibrary;
using Krista.FM.Common;
using System.IO;
using System.Collections.Specialized;
using Krista.FM.Client.MobileReports.Common;
using System.Windows.Forms;

namespace Krista.FM.Client.MobileReports.Snapshot
{
    public class Program
    {
        private const string _URL = "tcp://{0}/FMServer/Server.rem";

        private IServer server;
        private MobileReports.Core.MobileReportsCore helper;

        private void DoSnapshot(string paramsPath)
        {
            try
            {
                string aplServerURL = String.Format(_URL, ConfigurationManager.AppSettings["SchemeServerName"]);
                string aplServerUser = ConfigurationManager.AppSettings["SchemeUserName"];
                string aplServerUserPswSHA512 = ConfigurationManager.AppSettings["SchemeUserPswSHA512"];
                bool isWindowsAuthentication = bool.Parse(ConfigurationManager.AppSettings["IsWindowsAuthentication"]);
                AuthenticationType authenticationType = isWindowsAuthentication ?
                    AuthenticationType.atWindows : AuthenticationType.adPwdSHA512;

                //Подключаемся к серверу приложений.
                server = (IServer)Activator.GetObject(typeof(IServer), aplServerURL);
                LogicalCallContextData.SetAuthorization(aplServerUser);
                ClientSession.CreateSession(SessionClientType.WindowsNetClient);
                string errStr = string.Empty;
                IScheme scheme = null;
                server.Connect(string.Empty, out scheme, authenticationType, aplServerUser,
                    aplServerUserPswSHA512, ref errStr);

                //загружаем xml со стартовыми параметрами
                XmlDocument document = new XmlDocument();
                document.Load(paramsPath);

                //создаем помошника обновления отчетов
                helper = new MobileReports.Core.MobileReportsCore(scheme);
                //начинаем процесс обновления
                helper.DeployData(document.SelectSingleNode(@"//deployingReports"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                //отключаемся от схемы
                this.Disconnect();
            }
            Console.ReadLine();
        }

        /// <summary>
        /// Уничтожение сессии, освобождение ресурсов
        /// </summary>
        private void Disconnect()
        {
            // явно уничтожаем сессию
            LogicalCallContextData lccd = LogicalCallContextData.GetContext();
            if ((lccd != null) && (lccd["Session"] != null))
            {
                ISession session = lccd["Session"] as ISession;
                session.Dispose();
            }
        }

        //[STAThread]
        static void Main(string[] args)
        {
            string param = string.Empty;
            if ((args != null) && (args.Length > 0))
                param = args[0];
            else
                param = Path.Combine(Application.StartupPath, "StartParams.xml");
            Program snapshot = new Program();
            snapshot.DoSnapshot(param);
        }
    }
}
