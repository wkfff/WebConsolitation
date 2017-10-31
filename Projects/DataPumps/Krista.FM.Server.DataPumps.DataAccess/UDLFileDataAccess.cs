using ADODB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Text;

using Krista.FM.Common;


namespace Krista.FM.Server.DataPumps.DataAccess
{
    /// <summary>
    /// ����� �� ������� ���������������� ��������� ��� ������� � ������ ����� UDL-�����
    /// </summary>
    public static class UDLFileDataAccess
    {
        /// <summary>
        /// ��������� ADODB-���������� ���� ��� �������
        /// </summary>
        /// <param name="Con">ADODB-����������</param>
        public static void CloseAdoConnection(ADODB.Connection Con)
        {
            if ((Con != null) & (Con.State != (int)ObjectStateEnum.adStateClosed)) Con.Close();
        }

        /// <summary>
        /// ���������� ������ ����������� ��� UDL-����� � ��� ���������� (OleDB ��� ODBC)
        /// </summary>
        /// <param name="UdlFilePath">���� � UDL-�����</param>
        /// <param name="ConnectionString">������ �����������</param>
        /// <param name="IsOleDB">��� ����������</param>
        /// <returns>���������� ������ ������ ���� ��� ������ ������� � ����������� ������ � ��������� ������</returns>
        public static string GetConnectionStringFromUdl(string UdlFilePath, ref string ConnectionString,
            ref bool isOleDB)
        {
            string ErrStr = string.Empty;
            ADODB.Connection Con = null;
            const string MSOleDBForODBCProvider = "MSDASQL.DLL";
            try
            {
                // ������� ��������� ADODB connection
                Con = new ADODB.ConnectionClass();
                // �������� ��� �������
                Con.Open("File name=" + UdlFilePath, "", "", 0);
                if (Con.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                {
                    // �������� ��� OleDB-����������
                    ADODB.Property tmpProp = Con.Properties["Provider Name"];
                    string ProviderName = tmpProp.Value.ToString();
                    // ���� ��� �� "Microsoft OleDB provider for ODBC Drivers"
                    if (ProviderName != MSOleDBForODBCProvider)
                    {
                        // ... ���������� ������ ����������� �� UDL-�����
                        isOleDB = true;
                        ConnectionString = Con.ConnectionString;
                    }
                    else
                    {
                        // ���� ��� - ���������� ������ ��� ODBC
                        isOleDB = false;
                        // ��� �������� � ����������� ��������
                        tmpProp = Con.Properties["Extended Properties"];
                        ConnectionString = tmpProp.Value.ToString();
                    }
                }
            }
            catch (System.Exception obj)
            {
                ErrStr = obj.ToString();
            }
            finally
            {
                // ��������� ���������� � ����� ������
                CloseAdoConnection(Con);
            }
            return ErrStr;
        }

        /// <summary>
        /// ���������� ������������������ (�� �� ��������) ���������� ���� IDbConnection �� UDL-�����
        /// �������� � ��� OleDB ����������� � ��� ODBC
        /// </summary>
        /// <param name="UdlFilePath">���� � UDL-�����</param>
        /// <param name="con">����������</param>
        /// <returns>���������� ������ ������ ���� ��� ������ ������� � ����������� ������ � ��������� ������</returns>
        public static string GetConnectionFromUdl(string UdlFilePath, ref IDbConnection con, ref bool isOleDB)
        {
            string ErrStr = string.Empty;
            string ConnectionString = string.Empty;

            // ��������� ������ �� ����������
            con = null;

            // �������� ������ ����������� �� UDL-����� � ��� ����������
            ErrStr = GetConnectionStringFromUdl(UdlFilePath, ref ConnectionString, ref isOleDB);
            if (ErrStr == string.Empty)
            {
                // � ����������� �� ���� �������������� ��������� IDbConnection
                if (isOleDB)
                {
                    OleDbConnection OleDbCon = new OleDbConnection();
                    con = (IDbConnection)OleDbCon;
                }
                else
                {
                    OdbcConnection OdbcCon = new OdbcConnection();
                    con = (IDbConnection)OdbcCon;
                }
                // ������������ ������ �����������
                con.ConnectionString = ConnectionString;
            }
            return ErrStr;
        }

        /// <summary>
        ///  ��������� IDataAdapter �� UDL (� ����������� �� ���� �����������)
        /// </summary>
        /// <param name="UdlFilePath">���� � UDL-�����</param>
        /// <param name="QueryStr">������ �������</param>
        /// <param name="DataAdapter">������ �� ��������� IDataAdapter</param>
        /// <returns>���������� ������ ������ ���� ��� ������ ������� � ����������� ������ � ��������� ������</returns>
        public static string GetDataAdapterFromUdl(string UdlFilePath, string QueryStr, ref IDataAdapter DataAdapter)
        {
            string ErrStr = string.Empty;
            string ConnectionString = string.Empty;
            bool IsOleDb = true;

            // ��������� ������ �� DataAdapter
            DataAdapter = null;

            ErrStr = GetConnectionStringFromUdl(UdlFilePath, ref ConnectionString, ref IsOleDb);
            if (ErrStr == string.Empty)
            {
                // � ����������� �� ���� ����������� ������� �������������� �����������
                if (IsOleDb)
                {
                    OleDbDataAdapter OleDbDA = new OleDbDataAdapter(QueryStr, ConnectionString);
                    DataAdapter = (IDataAdapter)OleDbDA;
                }
                else
                {
                    OdbcDataAdapter OdbcDA = new OdbcDataAdapter(QueryStr, ConnectionString);
                    DataAdapter = (IDataAdapter)OdbcDA;
                }
            }
            return ErrStr;
        }

        /// <summary>
        /// ���������� �������� ��������� �������� �� ������ �����������
        /// </summary>
        /// <param name="con">�������</param>
        /// <param name="paramName">��� ���������</param>
        /// <returns>�������� ���������</returns>
        public static string GetConnectionParam(IDbConnection con, string paramName)
        {
            string[] strArray = con.ConnectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            int count = strArray.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (strArray[i].ToUpper().StartsWith(paramName.ToUpper()))
                    return strArray[i].Remove(0, paramName.Length + 1);
            }
            return string.Empty;
        }
    }
}