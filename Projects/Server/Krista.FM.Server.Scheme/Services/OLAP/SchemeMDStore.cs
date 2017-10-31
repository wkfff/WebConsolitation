using System;
using System.Data.OleDb;

using Krista.FM.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Services.OLAP
{
    /// <summary>
	/// ������ ����������� ������ � ����������� ���� ������.
	/// </summary>
    //[System.Diagnostics.DebuggerStepThrough()]
    internal class SchemeMDStore : DisposableObject, ISchemeMDStore
	{
       
		// ������ �����������.
		private OlapConnectionString connectionString;

		// ������ �������, ���������� ��� ����,
		// ����� ���������� � AS2000 �� ������������ ��� � SSAS2005.
		private string serverVersion;

        /// <summary>
        /// ������ ��� ������� � ����������� ���� ������.
        /// </summary>
        private OlapDatabase olapDatabase;


        /// <summary>
        /// ����������� �������.
        /// </summary>
        private SchemeMDStore()
		{
		}

        /// <summary>
        /// ��������� ������.
        /// </summary>
        public static SchemeMDStore Instance
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return Singleton<SchemeMDStore>.Instance; }
        }

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    if (olapDatabase != null)
					{
                        olapDatabase.Dispose();
					}
                }
            }
			base.Dispose(disposing);
        }

		/// <summary>
		/// �������������� ������ � ������� �������.
		/// </summary>
		private bool InitServerVersion()
		{
			OleDbConnection connection = new OleDbConnection(connectionString.OleDbConnectionString);
			try
			{
				connection.Open();
				serverVersion = connection.ServerVersion;
			}
			catch (Exception e)
			{
				Trace.TraceError(
					"������ ����������� � ����������� ���� ����� OleDb: {0}", e.Message);
			    return false;
			}
			finally
			{
				connection.Dispose();
			}

		    return true;
		}


        /// <summary>
		/// ��������� ������ � ������� �������. ���� ��������� �� ������� - ���������� ����������.
		/// </summary>
		/// <returns>true, ���� ���������� � SSAS2005</returns>
        public bool IsAS2005()
		{
			if (serverVersion.StartsWith("8."))
			{
				return false;
			}
			if (serverVersion.StartsWith("9."))
			{
				return true;
			}
			if (serverVersion.StartsWith("10."))
			{
				return true;
			}
            if (serverVersion.StartsWith("11."))
            {
                return true;
            }
			throw new ApplicationException(
				string.Format("����������� ������ �������: {0}", serverVersion));
		}

        /// <summary>
        /// ������������� ������������ ���������
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
			Trace.Indent();
			try
            {
                connectionString = new OlapConnectionString();
                connectionString.ReadConnectionString(SchemeClass.Instance.BaseDirectory + "\\" + "MAS.UDL");
                if (!InitServerVersion())
                    return false;

				if (IsAS2005())
				{
                    olapDatabase = new OlapDatabase2005(connectionString);
                }
				else
				{
                    olapDatabase = new OlapDatabase2000(connectionString);
                }
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("������������� ����������� ���� ��������� � �������: {0}", e.Message);
                return false;
            }
            finally
            {
                Trace.Unindent();
            }
        }

        /// <summary>
        /// ������ ��� ������� � ����������� ���� ������.
        /// </summary>
        internal OlapDatabase OlapDatabase
        {
            get { return olapDatabase; }
        }

        /// <summary>
        /// ������ ��� ������� � ����������� ���� ������.
        /// </summary>
        IOlapDatabase ISchemeMDStore.OlapDatabase
        {
            get { return olapDatabase; }
        }

        /// <summary>
        /// ��� ������� ����������� ����
        /// </summary>
        public string ServerName
        {
            get { return connectionString.DataSource; }
        }

        /// <summary>
        /// ��� ����������� ����
        /// </summary>
        public string CatalogName
        {
            get { return connectionString.InitialCatalog; }
        }

        /// <summary>
        /// ��� ����������� ����, �� ������� ��������� �����������
        /// </summary>
        public string OlapDataSourceName
        {
            get { return olapDatabase.OlapDataSourceName; }
        }

        #region ISchemeMDStore Members

        /// <summary>
        /// ������ ����������� ����
        /// </summary>
        public string DatabaseVersion
        {
            get { return olapDatabase.DatabaseVersion; }
        }

        /// <summary>
        /// ��� ������� ����������� ����, �� ������� ��������� �����������
        /// </summary>
        public string OlapDataSourceServer
        {
            get { return olapDatabase.OlapDataSourceServer; }
        }

        #endregion
    }
}
