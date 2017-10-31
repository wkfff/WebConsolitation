using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

using Krista.FM.Common;
using Krista.FM.Server.Scheme;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Scheme.Services.OLAP;

namespace Krista.FM.Server.Scheme
{
    /// <summary>
    /// ����� ��� ����� ���������� � ������� ����������.
    /// </summary>
    internal class ServerSystemInfo : SystemInfo
    {
        private DataTable collectedInfo;
        /// <summary>
        /// 
        /// </summary>
        private SchemeClass scheme;

        public ServerSystemInfo()
            : base("������")
        { }

        /// <summary>
        /// �������� ���������� � ��������� �� �� ���������� �������.
        /// </summary>
        /// <returns>������� ���������� ��������� ����������.</returns>
        protected override DataTable CollectInfo()
        {
            scheme = SchemeClass.Instance;

            collectedInfo = base.CollectInfo();

            LogicalCallContextData usercontext = LogicalCallContextData.GetContext();
            try
            {
                Krista.FM.Server.Common.SessionContext.SetSystemContext();

                CollectServerInfo("���������� � �������");
                CollectDataPumpInfo("������� ������");
            }
            catch(Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                LogicalCallContextData.SetContext(usercontext);
            }

            CheckRows(collectedInfo);
            return collectedInfo;
        }

        #region ���� ���������� � �������

        private void CollectServerInfo(string category)
        {
            try
            {
                // ����� ��������� �������
                GlobalInfo(category);

                // ���������� � �������
                CollectServerVersionsInfo(category);

                // ��������� �������
                CollectServerparameters(category);

                // ����������� ��
                CollectDWHInfo(category);

                // ����������� ��
                CollectOLAPInfo(category);
            }
            catch(Exception e)
            {
                Trace.TraceError("������ ��� ��������� ���������" + e.ToString());
            }
        }

        /// <summary>
        /// ����� ��������� �������
        /// </summary>
        /// <param name="category"></param>
        private void GlobalInfo(string category)
        {
            AddRow(category, "ProcessID", "ID ��������", String.Format("{0}", System.Diagnostics.Process.GetCurrentProcess().Id), null);
            AddRow(category, "G�", "����� ������ GC (����� ������ ������)", (System.Runtime.GCSettings.IsServerGC) ? "���������" : "����������", null);
            AddRow(category, "CLR", "������ CLR (������ .NET Framework)", String.Format("{0}", Environment.Version), null);
            AddRow(category, "MachineName", "��� ������", Environment.MachineName, null);
            AddRow(category, "UserDomainName", "��� ������", Environment.UserDomainName, null);
            AddRow(category, "UserName", "������� ��� ������� �������", Environment.UserName, null);
            AddRow(category, "ServerBaseDirectory", "������� �������", AppDomain.CurrentDomain.BaseDirectory, null);
            AddRow(category, "ServerAppName", "��� ����������", AppDomain.CurrentDomain.SetupInformation.ApplicationName, null);
            AddRow(category, "Machine", "������", scheme.Server.Machine, null);
            AddRow(category, "SchemeName", "��� �����", scheme.Name, null);
            AddRow(category, "MultiServerMode", "MultiServerMode", scheme.MultiServerMode.ToString(), null);
        }

        /// <summary>
        /// ��������� ����������� ��
        /// </summary>
        /// <param name="category"></param>
        private void CollectDWHInfo(string category)
        {
            Guid DWHID = AddRow(category, "DWHID", "��������� ����������� ��", String.Format("����: {0}\n��: {1}", scheme.SchemeDWH.ServerVersion, scheme.SchemeDWH.DataBaseName), null);

			AddRow(category, "ServerName", "������", (scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleClient || scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleDataAccess || scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.MSOracleDataAccess) ? string.Empty : scheme.SchemeDWH.ServerName, DWHID);
            AddRow(category, "FactoryName", "��� ����������", scheme.SchemeDWH.FactoryName, DWHID);
            AddRow(category, "DBVersion", "������ �������", scheme.SchemeDWH.ServerVersion, DWHID);
            AddRow(category, "DBName", "��� (�����)", scheme.SchemeDWH.DataBaseName, DWHID);
            AddRow(category, "Version", "������ ���� ������", scheme.SchemeDWH.DatabaseVersion, DWHID);
        }

        /// <summary>
        /// ��������� ����������� ���� 
        /// </summary>
        /// <param name="category"></param>
        private void CollectOLAPInfo(string category)
        {
            Guid MDStoreID = AddRow(category, "MDInfo", "��������� ����������� ����", String.Format("����: {0}\n����������� ����: {1}", scheme.SchemeMDStore.IsAS2005() ? "SSAS2005" : "SSAS2000", scheme.SchemeMDStore.CatalogName), null);

            AddRow(category, "MDStoreName", "������", SchemeMDStore.Instance.ServerName, MDStoreID);
            AddRow(category, "MDStoreVersion", "������ �������", SchemeMDStore.Instance.OlapDatabase.ServerVersion, MDStoreID);
            AddRow(category, "MDName", "���", SchemeMDStore.Instance.CatalogName, MDStoreID);
            AddRow(category, "MDStoreRName", "����������� ����", SchemeMDStore.Instance.OlapDataSourceName, MDStoreID);
            AddRow(category, "MDStoreRServer", "������ ����������� ����", SchemeMDStore.Instance.OlapDataSourceServer, MDStoreID);
            
            if (!String.IsNullOrEmpty(SchemeMDStore.Instance.OlapDatabase.DatabaseVersion))
                AddRow(category, "VersionMD", "������ ����������� ����", SchemeMDStore.Instance.OlapDatabase.DatabaseVersion, MDStoreID);

            AddRow(category, "MDXUniqueNameStyle", "�������� ��������� ���������� ����", SchemeMDStore.Instance.OlapDatabase.ConnectionString.MDXUniqueNameStyle, MDStoreID);
            GetCubesRevision(category, MDStoreID);
        }

        private void GetCubesRevision(string category, Guid mdStoreId)
        {
            DataTable cubestable = SchemeClass.Instance.Processor.OlapDBWrapper.GetPartitions("objecttype = 1");
            Guid CubesID = AddRow(category, "CubesInfo", "������ �����",
                                      String.Format("����� ����� - {0}", cubestable.Rows.Count), mdStoreId);
            foreach (DataRow row in cubestable.Rows)
            {

                if (row["Revision"] != null)
                {
                    AddRow(category, "Cube", row["ObjectName"].ToString(), row["Revision"].ToString(), CubesID);
                }
            }
        }

        /// <summary>
        /// ��������� �������
        /// </summary>
        /// <param name="category"></param>
        private void CollectServerparameters(string category)
        {
            Guid parametrID = AddRow(category, "ServerParameters", "��������� �������", String.Format("����� ���������� - {0}", System.Configuration.ConfigurationManager.AppSettings.Count), null);
            foreach (string parametr in System.Configuration.ConfigurationManager.AppSettings.Keys)
            {
                AddRow(category, parametr, parametr, scheme.Server.GetConfigurationParameter(parametr), parametrID);
            }
        }

        private void CollectServerVersionsInfo(string category)
        {
            AddRow(category, "ServerLibraryVersion", "������� ������ �������", AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion()), null);
            Guid versionID = AddRow(category, "ServerAssemblyes", "������ ������",
                                    String.Format("����� ������ - {0}",
                                                  scheme.UsersManager.GetServerAssemblyesInfo(
                                                      AppVersionControl.ServerAssemblyesSearchMaskDll).Count + scheme.UsersManager.GetServerAssemblyesInfo(
                                                      AppVersionControl.ServerAssemblyesSearchMaskExe).Count), null);

            CollectVersionInfo(category, AppVersionControl.ServerAssemblyesSearchMaskDll, versionID);
            CollectVersionInfo(category, AppVersionControl.ServerAssemblyesSearchMaskExe, versionID);
            CollectVersionInfo(category, "Krista.Diagnostics.dll", versionID);
        }

        private void CollectVersionInfo(string category, string filter, object parentID)
        {
            Dictionary<string, string> commonVer = AppVersionControl.GetAssemblyesVersions(filter);
            foreach (string commonVersion in commonVer.Keys)
            {
               AddRow(category, "Version", commonVersion, commonVer[commonVersion], parentID);
            }
        }
        
        #endregion

        private void CollectDataPumpInfo(string category)
        {
            string filter = "Krista.FM.Server.DataPump*.dll";

            Guid versionsID = AddRow(category, "DataPumpAssemblyVersions", "������ ������", String.Format("����� ������ - {0}", AppVersionControl.GetAssemblyesVersions(filter).Count), null); 
            CollectVersionInfo(category, filter, versionsID);
        }
    }
}
