using System;

using Krista.FM.ServerLibrary;
using Krista.FM.Common;

namespace Krista.FM.Server.Scheme
{
    public sealed class SchemeStub : MarshalByRefObject, ISchemeStub
    {
        #region ����

        /// <summary>
        /// ���������� ��������� �����.
        /// </summary>
        private IServer server;

        /// <summary>
        /// ���������� ��������� �����.
        /// </summary>
        private IScheme scheme;

        /// <summary>
        /// ������������ �����
        /// </summary>
        private string name;

        /// <summary>
        /// ���� � ����������������� ����� �����
        /// </summary>
        private string path;

        #endregion ����

        /// <summary>
        /// ����������� �������
        /// </summary>
        /// <param name="server"></param>
        /// <param name="name">������������ �����</param>
        /// <param name="path">���� � ����������������� ����� �����</param>
        public SchemeStub(IServer server, string name, string path)
        {
            this.server = server;
            this.name = name;
            this.path = path;
        }


        /// <summary>
        /// ��������� �����. �������� ������������ � ����� ������ ��������������.
        /// ��� ����, ����� ������� ������������ ����� ������������ � ����� ���������� ������� ����� Open.
        /// </summary>
        public void Startup()
        {
            try
            {
                UnityStarter.Initialize();

                SchemeClass startScheme = (SchemeClass)Resolver.Get<IScheme>();

                startScheme.Server = server;
                startScheme.Name = name;

                if (startScheme.Initialize(this.path))
                    scheme = startScheme;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// ��������� � ��������� �����
        /// </summary>
        public void Shutdown()
        {
            try
            {
                if (scheme != null)
                {
                    scheme.Dispose();
                    scheme = null;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("������ ������������ ����� {0}: {1}", name, e));
            }
        }

        public string Connect()
        {
            return ((SchemeClass)scheme).Connect();
        }

        public void Disconnect()
        {
            ((SchemeClass)scheme).Disconnect();
        }

        /// <summary>
        /// �������� ������ �����
        /// </summary>
        public IScheme Scheme
        {
            get { return scheme; }
        }
    }
}