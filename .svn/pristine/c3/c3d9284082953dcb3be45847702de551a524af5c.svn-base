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
    /// Класс со всякими вспомогательными функциями для доступа к данным через UDL-файлы
    /// </summary>
    public static class UDLFileDataAccess
    {
        /// <summary>
        /// Закрывает ADODB-соединение если оно открыто
        /// </summary>
        /// <param name="Con">ADODB-соединение</param>
        public static void CloseAdoConnection(ADODB.Connection Con)
        {
            if ((Con != null) & (Con.State != (int)ObjectStateEnum.adStateClosed)) Con.Close();
        }

        /// <summary>
        /// Возвращает строку подключения для UDL-файла и тип подклюения (OleDB или ODBC)
        /// </summary>
        /// <param name="UdlFilePath">Путь к UDL-файлу</param>
        /// <param name="ConnectionString">Строка подключения</param>
        /// <param name="IsOleDB">Тип соединения</param>
        /// <returns>Возвращает пустую строку если все прошло успешно и расшифровку ошибки в противном случае</returns>
        public static string GetConnectionStringFromUdl(string UdlFilePath, ref string ConnectionString,
            ref bool isOleDB)
        {
            string ErrStr = string.Empty;
            ADODB.Connection Con = null;
            const string MSOleDBForODBCProvider = "MSDASQL.DLL";
            try
            {
                // создаем временное ADODB connection
                Con = new ADODB.ConnectionClass();
                // пытаемся его открыть
                Con.Open("File name=" + UdlFilePath, "", "", 0);
                if (Con.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                {
                    // Получаем тип OleDB-провайдера
                    ADODB.Property tmpProp = Con.Properties["Provider Name"];
                    string ProviderName = tmpProp.Value.ToString();
                    // Если это не "Microsoft OleDB provider for ODBC Drivers"
                    if (ProviderName != MSOleDBForODBCProvider)
                    {
                        // ... возвращаем строку подключения из UDL-файла
                        isOleDB = true;
                        ConnectionString = Con.ConnectionString;
                    }
                    else
                    {
                        // если нет - возвращаем строку для ODBC
                        isOleDB = false;
                        // она хранится в специальном свойстве
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
                // закрываем соединение в любом случае
                CloseAdoConnection(Con);
            }
            return ErrStr;
        }

        /// <summary>
        /// Возвращает инициализированное (но не открытое) соединение типа IDbConnection по UDL-файлу
        /// работает и для OleDB провайдеров и для ODBC
        /// </summary>
        /// <param name="UdlFilePath">Путь к UDL-файлу</param>
        /// <param name="con">соединение</param>
        /// <returns>Возвращает пустую строку если все прошло успешно и расшифровку ошибки в противном случае</returns>
        public static string GetConnectionFromUdl(string UdlFilePath, ref IDbConnection con, ref bool isOleDB)
        {
            string ErrStr = string.Empty;
            string ConnectionString = string.Empty;

            // Освободим ссылку на соединение
            con = null;

            // Получаем строку подключения из UDL-файла и тип провайдера
            ErrStr = GetConnectionStringFromUdl(UdlFilePath, ref ConnectionString, ref isOleDB);
            if (ErrStr == string.Empty)
            {
                // В зависимости от типа инициализируем интерфейс IDbConnection
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
                // устанавлиаем строку подключения
                con.ConnectionString = ConnectionString;
            }
            return ErrStr;
        }

        /// <summary>
        ///  Получение IDataAdapter из UDL (в зависимости от типа подключения)
        /// </summary>
        /// <param name="UdlFilePath">Путь к UDL-файлу</param>
        /// <param name="QueryStr">Строка запроса</param>
        /// <param name="DataAdapter">ссылка на экземпляр IDataAdapter</param>
        /// <returns>Возвращает пустую строку если все прошло успешно и расшифровку ошибки в противном случае</returns>
        public static string GetDataAdapterFromUdl(string UdlFilePath, string QueryStr, ref IDataAdapter DataAdapter)
        {
            string ErrStr = string.Empty;
            string ConnectionString = string.Empty;
            bool IsOleDb = true;

            // освободим ссылку на DataAdapter
            DataAdapter = null;

            ErrStr = GetConnectionStringFromUdl(UdlFilePath, ref ConnectionString, ref IsOleDb);
            if (ErrStr == string.Empty)
            {
                // в зависимости от типа подключения создаем соответсвующий датаадаптер
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
        /// Возвращает значение параметра коннекта из строки подключения
        /// </summary>
        /// <param name="con">Коннект</param>
        /// <param name="paramName">Имя параметра</param>
        /// <returns>Значение параметра</returns>
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