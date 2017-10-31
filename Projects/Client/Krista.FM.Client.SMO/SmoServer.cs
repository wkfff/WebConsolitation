using System;
using System.Collections.Generic;
using System.ComponentModel;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public class SmoServer : ServerManagedObject<IServer>, IServer
    {
        public SmoServer(IServer serverControl)
            : base(serverControl)
        {
        }

        #region IServer Members

        [DisplayName("��� ������ (Machine)")]
        [Description("��� ������ �� ������� ��������� ������ ����������")]
        public string Machine
        {
            get { return serverControl.Machine; }
        }

        [Browsable(false)]
        public System.Collections.ICollection SchemeList
        {
            get { return serverControl.SchemeList; }
        }

        [Browsable(false)]
        public bool Connect(string schemeName, out IScheme scheme, AuthenticationType authType, string login,
            string pwdHash, ref string errStr)
        {
            throw new NotSupportedException("�� ��������������");
        }

        [Browsable(false)]
        public bool Connect(string schemeName, out IScheme scheme, AuthenticationType authType, string login,
            string pwdHash, ref string errStr, string clientServerLibraryVersion)
        {
            throw new NotSupportedException("�� ��������������");
        }

        public bool Connect(out IScheme scheme, AuthenticationType authType, string login, string pwdHash, ref string errStr)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Connect(out IScheme scheme, AuthenticationType authType, string login, string pwdHash, ref string errStr, string clientServerLibraryVersion)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [Browsable(false)]
        public ISchemeStub Connect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [DisplayName("����� ���-������� (WebServiceUrl)")]
        [Description("����� ���-�������")]
        public string WebServiceUrl
        {
            get { return serverControl.GetConfigurationParameter("WebServiceUrl"); }
        }

        [DisplayName("���� � ����������� (RepositoryPath)")]
        [Description("���� � ����������� ������� ����������")]
        public string RepositoryPath
        {
            get { return serverControl.GetConfigurationParameter("RepositoryPath"); }
        }

        [Browsable(false)]
        public void Activate()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [Browsable(false)]
        public void Disconnect()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [Browsable(false)]
        public void Startup(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [Browsable(false)]
        public void Open(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [Browsable(false)]
        public void Shutdown(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [DisplayName("���� ������� (ServerPort)")]
        [Description("���� �������")]
        public int ServerPort
        {
            get
            {
                return Convert.ToInt32(serverControl.GetConfigurationParameter("ServerPort"));
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        [Browsable(false)]
        public int NextFreePort
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string GetConfigurationParameter(string key)
        {
            return serverControl.GetConfigurationParameter(key);
        }

        public object GetConfigurationSection(string key)
        {
            return serverControl.GetConfigurationSection(key);
        }

        #endregion
    }
}
