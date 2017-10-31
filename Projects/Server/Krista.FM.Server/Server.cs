using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server
{
    /// <summary>
    /// �������� �������� ��������� ������ IServer.
    /// �������� � ���� ������ �� ������� ���� ������������� �� �������
    /// </summary>
    public sealed class ServerClass : DisposableObject, IServer
    {
        #region ����

        /// <summary>
        /// �������� ��� ������� �����.
        /// </summary>
        private SchemeStub stub;

        /// <summary>
        /// ���� �������.
        /// </summary>
        private int serverPort;

        /// <summary>
        /// ����� ��� �������
        /// </summary>
        private string webServiceUrl;
        
        /// <summary>
        /// ���� � �������� ����������� ����. 
        /// </summary>
        private string repositoryPath;

        #endregion ����

        #region ������� ������

        /// <summary>
        /// ����������� ���������� ������
        /// </summary>
        internal ServerClass()
        {
            LogicalCallContextData.SetAuthorization("SYSTEM");

            try
            {
                Trace.TraceEvent(TraceEventType.Information, "{0}", DateTime.Now);
                Trace.TraceEvent(TraceEventType.Information, "ID ��������: {0}", Process.GetCurrentProcess().Id);
                Trace.TraceEvent(TraceEventType.Information, "����� ������ GC: {0}", GCSettings.IsServerGC ? "���������" : "����������");


                if (!CheckOSVersion())
                    return;

                if (!CheckServerAssemblyVersions())
                    return;

                webServiceUrl = GetConfigurationParameter("WebServiceUrl");
                ServerPort = Convert.ToInt16(GetConfigurationParameter("ServerPort"));

                if (!InitializeSchemes())
                {
                    Trace.TraceEvent(TraceEventType.Error, "������ ��� ������������� ����", "ServerClass");
                }
            }
            catch (Exception e)
            {
                // � ��� ������ ���������� �� ������ ��������
                Trace.TraceEvent(TraceEventType.Critical, "��� � ������������ ������� ServerClass: {0}", e);
                //throw (new Exception(e.Message));
            }
        }

        /// <summary>
        /// ��������� ����������� ���������� ������
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                Trace.TraceEvent(TraceEventType.Verbose, "~{0}({1})", GetType().FullName, disposing);
            }
            catch { /* ������ ��� ���������� */ }

            lock (this)
            {
                if (disposing)
                {
                    try
                    {
                        stub.Shutdown();
                    }
                    catch (Exception e)
                    {
                        Trace.TraceEvent(TraceEventType.Critical, e.ToString());
                    }
                }
            }
            base.Dispose(disposing);
        }

        #endregion ������� ������

        #region �������������

        /// <summary>
        /// �������� ������ ������������ �������
        /// </summary>
        /// <returns>true - ������������ ������� ��������������</returns>
        private static bool CheckOSVersion()
        {
            Trace.TraceEvent(TraceEventType.Information, "������ ������������ �������: {0}", Environment.OSVersion);
            Trace.TraceEvent(TraceEventType.Information, "���������� �����������: {0}", Environment.ProcessorCount);
            Trace.TraceEvent(TraceEventType.Information, "������ CLR: {0}", Environment.Version);
            Trace.TraceEvent(TraceEventType.Information, "��� ������: {0}", Environment.MachineName);
            Trace.TraceEvent(TraceEventType.Information, "��� ������: {0}", Environment.UserDomainName);
            Trace.TraceEvent(TraceEventType.Information, "������� ��� ������� �������: {0}", Environment.UserName);
            return true;
        }

        /// <summary>
        /// �������� ������ ������
        /// </summary>
        /// <returns></returns>
        private static bool CheckServerAssemblyVersions()
        {
            string serverLibraryVersion = AppVersionControl.GetServerLibraryVersion();

            string baseVesion = AppVersionControl.GetAssemblyBaseVersion(serverLibraryVersion);
            Trace.TraceEvent(TraceEventType.Information, "������� ������ ������� {0}", baseVesion);

            Dictionary<string, string> badAssemblies = new Dictionary<string, string>();

            AppVersionControl.CheckAssemblyVersions(baseVesion, "Krista.FM.Common.dll", badAssemblies, true);
            AppVersionControl.CheckAssemblyVersions(baseVesion, "Krista.FM.Server.*.dll", badAssemblies, true);

            if (badAssemblies.Count > 0)
            {
                Trace.TraceEvent(TraceEventType.Critical, "���������� {0} ������ � ������� ������������ �� �������.", badAssemblies.Count);
                return false;
            }

            return true;
        }

        /// <summary>
        /// ��������� ������������� ����, ������� �� ����� �����������
		/// </summary>
		/// <returns>
		/// ���������� ������ ��� ���� � ����������� �� ����������� �������������.
		/// ��������� �������� ������ ������ �������� � ����
		/// </returns>
        private bool InitializeSchemes()
        {
            // XML ��������� �����������
            string RepositorySettings = "RepositorySettings.xml";

            // �������� ���� � ����������� �� �������� ����������
            repositoryPath = GetConfigurationParameter("RepositoryPath");

            string[] qualifiedPath = repositoryPath.Split('\\');
            bool isRepositoryAbove2_3_0 = !qualifiedPath[qualifiedPath.Length - 1].Contains("Repository");

            Trace.TraceEvent(TraceEventType.Information, "���� � ����������� ����� " + repositoryPath, "ServerClass");

            if (isRepositoryAbove2_3_0)
            {
                InitializeScheme(repositoryPath + "\\Packages\\SchemeConfiguration.xml");
            }
            else
            {
                // ��������� � ��������� ���� �������� �����������
                string errMsg;
                XmlDocument xmlDoc = Validator.LoadValidated(repositoryPath + "\\" + RepositorySettings,
                    "ServerConfiguration.xsd", "xmluml", out errMsg);
                if (xmlDoc == null)
                {
                    Trace.TraceEvent(TraceEventType.Error, "������ ��� �������� �������� �����������\n{0}", errMsg);
                    return false;
                }

                // ������������ ������ ���� �����������
                XmlNodeList xmlSchemesList = xmlDoc.SelectNodes("/ServerConfiguration/Repository/Schemes/Scheme");
                foreach (XmlNode xmlScheme in xmlSchemesList)
                {
                    InitializeScheme(repositoryPath + "\\" + xmlScheme.Attributes["privatePath"].Value);
                    break; // ��������� ��������� ������ ����� �����
                }
            }

            return true;
        }

        /// <summary>
        /// ������������� ��������� �����.
        /// </summary>
        /// <param name="schemeFileName">������ ���� � XML-����� �������� �����</param>
        /// <returns>
        /// ���������� ������ ��� ���� � ����������� �� ����������� ������������� �����.
        /// ��������� �������� ������ ������ �������� � ����
        /// </returns>
        private bool InitializeScheme(string schemeFileName)
        {
            string schemeName = "UnassignedName";
            try
            {
                string errMsg;

                // ��� ������ �������� �� ����������
                XmlDocument xmlDoc = Validator.LoadValidated(schemeFileName, "ServerConfiguration.xsd", "xmluml", out errMsg);
                if (xmlDoc == null)
                {
                    Trace.TraceEvent(TraceEventType.Error, "������ ��� �������� �������� �����{0}{1}", Environment.NewLine, errMsg);
                    return false;
                }

                // �������� ��� �����
                XmlNode xmlNode = xmlDoc.SelectSingleNode("/ServerConfiguration/Package/@name");
                schemeName = xmlNode.Value;

                // ��������� � ��������� ������������� ���� �������
                stub = new SchemeStub(this, schemeName, schemeFileName);
                stub.Startup();
                //if (stub.Scheme != null)
                //    stub.Scheme.Server = this;
            }
            catch (Exception e)
            {
                Trace.TraceEvent(TraceEventType.Error, "������ ������������� �����{0}{1}{2}", schemeName, Environment.NewLine, e.ToString());
                return false;
            }
            return true;
        }

        #endregion �������������

        #region ��������������� �������

        private bool IsConnected(LogicalCallContextData context)
        {
            if(context["SessionID"] == null)
                return false;

            string schemeName = Convert.ToString(context["SchemeName"]);
            if (!String.IsNullOrEmpty(schemeName))
            {
                if (stub.Scheme.Name == schemeName)
                    return true;

            }
            return false;
        }

        private void Disconnect(LogicalCallContextData context)
        {
            if (context == null)
                return;

            if (!IsConnected(context))
                return;
            
            Trace.TraceEvent(TraceEventType.Verbose, "ServerClass.Disconnect()", "Server");
            if (stub.Scheme != null)
            {
                stub.Disconnect();
            }
            
        }

        #endregion ��������������� �������

        #region ���������� ���������� IServer

        void IServer.Activate()
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "Server";
            Trace.TraceEvent(TraceEventType.Verbose, "������ ������� �����������");
            return;
        }

        public void Disconnect()
        {
            Disconnect(LogicalCallContextData.GetContext());
        }

        public ISchemeStub Connect()
        {
            if (!Authentication.IsSystemRole())
                throw new Exception("������������ ���������� ��� ����������� � ���������� �����.");

            // �������� ���� �� �����
            if (stub == null)
                throw new Exception("����� �������� � �������! ��. ��� �������.");

            return stub;
        }

        public void Connect(ISchemeStub schemeStub, out IScheme schemeObject)
        {
            // TODO: ��������� �������� ������������ �� ������� ����������� � �����-���� �����
            // � ���� �� ���������, �� ������������� �������� ��� ����������
            Disconnect(LogicalCallContextData.GetContext());

            // TODO: ������� ������ ������ ���������� � ��������� �� �������� � �������� ������������
            Trace.TraceEvent(TraceEventType.Verbose, "Scheme.Connect()");
            ((SchemeStub)schemeStub).Connect();

            schemeObject = ((SchemeStub)schemeStub).Scheme;
        }
        
        /*
        /// <summary>
        /// ����������� � ����� � ������ schemeName
        /// </summary>
        /// <param name="schemeName">��� ����� � ������� ����� ������������</param>
        /// <param name="schemeObject">���� ����������� ��������, �� scheme �������� ��������� �� ������ �����</param>
        /// <returns>
        /// ���������� ������ ��� ���� � ����������� �� ���������� ����������� � �����.
        /// ��������� �������� ������ ������ �������� � ���� ������� ����������
        /// </returns>
        public bool Connect(string schemeName, out IScheme scheme)
        {
            string errStr = String.Empty;
            return Connect(schemeName, out scheme, AuthenticationType.atUndefined, String.Empty, String.Empty, ref errStr);
        }
        */

        public bool Connect(string schemeName, out IScheme scheme, AuthenticationType authType, string login,
            string pwdHash, ref string errStr)
        {
            return Connect(out scheme, authType, login, pwdHash, ref errStr, String.Empty);
        }
        
        public bool Connect(out IScheme scheme, AuthenticationType authType, string login,
            string pwdHash, ref string errStr)
        {
            return Connect(out scheme, authType, login, pwdHash, ref errStr, String.Empty);
        }

        public bool Connect(string schemeName, out IScheme scheme, AuthenticationType authType, string login,
            string pwdHash, ref string errStr, string clientServerLibraryVersion)
        {
            return Connect(out scheme, authType, login, pwdHash, ref errStr, clientServerLibraryVersion);
        }

        public bool Connect(out IScheme scheme, AuthenticationType authType, string login,
            string pwdHash, ref string errStr, string clientServerLibraryVersion)
        
        {
            Trace.TraceEvent(TraceEventType.Information, "������������ {0} �������� ����������� � ����� ��� ������ {1}. ������ ������������� {2}", Authentication.UserDate, login, authType);

            if (stub.Scheme == null)
            {
                errStr = "����� �������� � �������! ��. ��� �������.";
                scheme = null;
                return false;
            }

            scheme = null;

            // ���� ����� - ������ ������ ����� � ������������ �����������
            if (!String.IsNullOrEmpty(clientServerLibraryVersion))
            {
                string clientBaseVersion = AppVersionControl.GetAssemblyBaseVersion(clientServerLibraryVersion);
                string serverLibraryVersion = AppVersionControl.GetServerLibraryVersion();
                string serverBaseVersion = AppVersionControl.GetAssemblyBaseVersion(serverLibraryVersion);
                if (serverBaseVersion != clientBaseVersion)
                {
                    errStr = String.Concat(
                        String.Format("���������� �������� ������ ����� ������ '{0}'", AppVersionControl.ServerLibraryAssemblyName),
                        Environment.NewLine,
                        Environment.NewLine,
                        String.Format("������: {0}", serverLibraryVersion),
                        Environment.NewLine,
                        String.Format("������: {0}", clientServerLibraryVersion),
                        Environment.NewLine,
                        Environment.NewLine,
                        "����������� ����������"
                    );
                    return false;
                }
            }


            LogicalCallContextData userContext = LogicalCallContextData.GetContext();
            ISchemeStub ss;
            try
            {
                if (userContext["SchemeName"] == null)
                    userContext["SchemeName"] = stub.Scheme.Name;

                SessionContext.SetSystemContext();
                ss = Connect();
            }
            finally
            {
                LogicalCallContextData.SetContext(userContext);
            }

            if (ss == null)
                return false;

            Connect(ss, out scheme);

            Trace.TraceEvent(TraceEventType.Verbose, "�������������� �������");
            errStr = String.Empty;
            // ���� ����� - ��������������� �������
            switch (authType)
            {
                case AuthenticationType.atWindows:
                    // �������� - ���������� ����� �������������� �������������
                    scheme.UsersManager.CheckCurrentUser();
                    scheme.UsersManager.AuthenticateUser(login, ref errStr);
                    break;
                case AuthenticationType.adPwdSHA512:
                    scheme.UsersManager.AuthenticateUser(login, pwdHash, ref errStr);
                    break;
            }
            // ���� ������ �� ���������������� - ��������� ����������
            if (!String.IsNullOrEmpty(errStr))
            {
                stub.Disconnect();
                scheme = null;
                Trace.TraceError(errStr);
            }
            else
                Trace.TraceVerbose(String.Format("������ ����������������. ID ������ = {0}", SessionContext.SessionId));
            
            return String.IsNullOrEmpty(errStr);
        }

        /// <summary>
        /// ���� � ����������� ������� ����������
        /// </summary>
        public string RepositoryPath
        {
            get { return repositoryPath; }
        }

        /// <summary>
        /// ����� ��� �������
        /// </summary>
        public string WebServiceUrl
        {
            get { return webServiceUrl; }
        }

        /// <summary>
        /// ���������� ������ ���� �������� ���� �� �������
        /// </summary>
        public ICollection SchemeList
        {
            get
            {
                ArrayList sl = new ArrayList();
                sl.Add(stub.Scheme == null ? "����� �������� � �������! ��. ����." : stub.Scheme.Name);
                return sl;
            }
        }

        public string Machine 
        { 
            get 
            { 
                return Environment.MachineName; 
            } 
        }

        public int ServerPort
        {
            get { return serverPort; }
            set
            {
                if (serverPort == 0)
                {
                    serverPort = value;
                }
                else
                    throw new Exception("�������� ServerPort �� �������������.");
            }
        }

        /// <summary>
        /// ���������� ���������������� �������� ������� ����������.
        /// </summary>
        /// <param name="key">���������� ���� ���������.</param>
        /// <returns>�������� ���������.</returns>
        public string GetConfigurationParameter(string key)
        {
            string value = ConfigurationManager.AppSettings[key];

            #region ��������� �������� ��� ��������� ���������� ����������
            if (value == null)
            {
                if (key == "ProductName")
                {
                    Assembly assembly = AppVersionControl.GetServerLibraryAssembly();
                    object[] attr = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if (attr.Length > 0)
                        return ((AssemblyProductAttribute)attr[0]).Product;
                    else
                        return null;
                }
                if (key == "SupportServiceInfo")
                {
                    Assembly assembly = AppVersionControl.GetServerLibraryAssembly();
                    object[] attr = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    if (attr.Length > 0)
                        return ((AssemblyCompanyAttribute)attr[0]).Company;
                    else
                        return null;
                }
                if (key == "AssemblyBaseVersion")
                {
                   return AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion());
                }
            }
            #endregion

            return value;
        }

        #endregion ���������� IServer

        // ������ ����� ���� �����
        public override object InitializeLifetimeService()
        {          
            return null;
        }
    }
}
