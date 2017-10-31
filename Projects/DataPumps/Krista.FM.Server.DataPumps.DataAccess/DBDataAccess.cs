using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.DataAccess
{
    /// <summary>
    /// ������������ ����
    /// </summary>
    public enum DBMSName
    {
        /// <summary>
        /// dBase (*.dbf)
        /// </summary>
        dBase,

        /// <summary>
        /// Interbase � ��� �����
        /// </summary>
        Interbase,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle
    }


    /// <summary>
    /// �������� ODBC
    /// </summary>
    public enum ODBCDriverName
    {
        /// <summary>
        /// Microsoft dBase Driver (*.dbf)
        /// </summary>
        Microsoft_dBase_Driver,

        /// <summary>
        /// Microsoft dBase VFP Driver (*.dbf)
        /// </summary>
        Microsoft_dBase_VFP_Driver,

        /// <summary>
        /// Microsoft FoxPro VFP Driver (*.dbf)
        /// </summary>
        Microsoft_FoxPro_VFP_Driver
    }


    /// <summary>
    /// ����� � ��������� ��� ������� � ������ ������� DBF
    /// </summary>
    public class DBDataAccess : DisposableObject
    {
        #region ����

        // ������ ��� �������� ��������� ����������
        private ArrayList dsnList = new ArrayList();

        #endregion ����


        #region ���������

        // Add a system DSN
        const int ODBC_ADD_SYS_DSN = 4;
        // Configure a system DSN
        const int ODBC_CONFIG_SYS_DSN = 5;
        // Remove a system DSN
        const int ODBC_REMOVE_SYS_DSN = 6;
        // �������� ��������, � ������� �������� ������������ � �������� � ������� DBF
        const string DBF_DRIVER_NAME = "Microsoft dBase Driver (*.dbf)";

        #endregion ���������


        /// <summary>
        /// ������������ ��������
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearSYSDSN();
            }

            base.Dispose(disposing);
        }

        [DllImport("odbccp32.dll")]
        private static extern bool SQLConfigDataSource(int hwndParent, int fRequest, string Driver, string Attributes);

        /// <summary>
        /// ������� �������� ������ ODB�, ����������� �� ��������� �������
        /// </summary>
        /// <param name="dbfDir">������� � ������� DBF</param>
        /// <param name="con">����������</param>
        /// <param name="sourceName">��� ���������� ���������</param>
        /// <returns>���������� ������ ������, ���� ��� ������ ������� � ����������� ������ � ��������� ������</returns>
        private string CreateSYSDSN(string dbfDir, ref IDbConnection con, out string sourceName)
        {
            sourceName = string.Empty;

            try
            {
                // ������� ��������� ��� ��� ������ ���������
                string tmpName = Guid.NewGuid().ToString();
                tmpName = tmpName.Replace("-", "");

                // ��������� �������� � DSN
                string connectionStr = string.Format(
                    "DSN={0};Description=��������� �������� ������;Exclusive=1;ReadOnly=1;FIL=dBase 5.0;" +
                    "DefaultDir={1};CollatingSequence=ASCII;Deleted=01;PageTimeout=5;Statistics=0;UserCommitSync=Yes;",
                    tmpName, dbfDir);
                SQLConfigDataSource(0, ODBC_ADD_SYS_DSN, DBF_DRIVER_NAME, connectionStr);

                // ������ �����������
                connectionStr = connectionStr + "DriverId=533";
                OdbcConnection OdbcCon = new OdbcConnection(connectionStr);
                con = (IDbConnection)OdbcCon;

                // ���������� ��������, ����� ����� ��������
                dsnList.Add(tmpName);
                sourceName = tmpName;

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// ������� ��������� ��������
        /// </summary>
        /// <param name="sourceName">��� ���������</param>
        /// <returns>������ ������</returns>
        public string DeleteSYSDSN(string sourceName)
        {
            try
            {
                SQLConfigDataSource(0, ODBC_REMOVE_SYS_DSN, DBF_DRIVER_NAME, string.Format("DSN={0}", sourceName));
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// ������� ��� ��������� ��������� ��������� ODBC
        /// </summary>
        /// <returns>������ ������</returns>
        public string ClearSYSDSN()
        {
            try
            {
                for (int i = dsnList.Count - 1; i >= 0; i--)
                {
                    DeleteSYSDSN((string)dsnList[i]);
                    dsnList.RemoveAt(i);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// ������� ����������� � ���������
        /// </summary>
        /// <param name="dir">��������</param>
        /// <returns>������ ��</returns>
        public Database ConnectToDataSource(string dir)
        {
            string sourceName = string.Empty;
            if (!Directory.Exists(dir)) return null;

            Database result = null;
            IDbConnection conn = null;

            CreateSYSDSN(dir, ref conn, out sourceName);
            if (conn == null) return null;

            result = new Database(conn, System.Data.Common.DbProviderFactories.GetFactory("System.Data.Odbc"), false);

            return result;
        }

        /// <summary>
        /// ������� ����������� � ����� dbf ����� Microsoft FoxPro VFP Driver (*.dbf)
        /// </summary>
        /// <param name="file">����</param>
        /// <returns>������ ��</returns>
        public void ConnectToDataSource(ref Database db, string dir, ODBCDriverName odbcDriverName)
        {
            if (db != null)
            {
                db.Close();
                db = null;
            }

            IDbConnection conn = null;

            switch (odbcDriverName)
            {
                case ODBCDriverName.Microsoft_dBase_Driver:
                    conn = (IDbConnection)new OdbcConnection(string.Format(
                        "DRIVER=Microsoft dBase Driver (*.dbf);DriverID=533;FIL=dBase 5.0;DefaultDir={0}", dir));
                    break;

                case ODBCDriverName.Microsoft_dBase_VFP_Driver:
                    conn = (IDbConnection)new OdbcConnection(string.Format(
                        "DRIVER=Microsoft dBase VFP Driver (*.dbf);Exclusive=No;SourceType=DBF;SourceDB={0}", dir));
                    break;

                case ODBCDriverName.Microsoft_FoxPro_VFP_Driver:
                    conn = (IDbConnection)new OdbcConnection(string.Format(
                        "DRIVER=Microsoft FoxPro VFP Driver (*.dbf);Exclusive=No;SourceType=DBF;SourceDB={0}", dir));
                    break;
            }

            if (conn == null)
            {
                throw new DataSourceIsCorruptException(string.Format(
                    "������ ��� �������� ���������� � ���������� {0}.", dir));
            }

            db = new Database(conn, System.Data.Common.DbProviderFactories.GetFactory("System.Data.Odbc"), false);
        }
    }
}
