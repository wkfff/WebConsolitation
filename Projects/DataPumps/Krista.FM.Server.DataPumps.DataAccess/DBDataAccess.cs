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
    /// Наименование СУБД
    /// </summary>
    public enum DBMSName
    {
        /// <summary>
        /// dBase (*.dbf)
        /// </summary>
        dBase,

        /// <summary>
        /// Interbase и его клоны
        /// </summary>
        Interbase,

        /// <summary>
        /// Oracle
        /// </summary>
        Oracle
    }


    /// <summary>
    /// Драйверы ODBC
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
    /// Класс с функциями для доступа к данным формата DBF
    /// </summary>
    public class DBDataAccess : DisposableObject
    {
        #region Поля

        // Список для хранения созданных источников
        private ArrayList dsnList = new ArrayList();

        #endregion Поля


        #region Константы

        // Add a system DSN
        const int ODBC_ADD_SYS_DSN = 4;
        // Configure a system DSN
        const int ODBC_CONFIG_SYS_DSN = 5;
        // Remove a system DSN
        const int ODBC_REMOVE_SYS_DSN = 6;
        // Название драйвера, с помощью которого подключаемся к каталогу с файлами DBF
        const string DBF_DRIVER_NAME = "Microsoft dBase Driver (*.dbf)";

        #endregion Константы


        /// <summary>
        /// Освобождение ресурсов
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
        /// Создает источник данных ODBС, настроенный на указанный каталог
        /// </summary>
        /// <param name="dbfDir">Каталог с файлами DBF</param>
        /// <param name="con">Соединение</param>
        /// <param name="sourceName">Имя созданного источника</param>
        /// <returns>Возвращает пустую строку, если все прошло успешно и расшифровку ошибки в противном случае</returns>
        private string CreateSYSDSN(string dbfDir, ref IDbConnection con, out string sourceName)
        {
            sourceName = string.Empty;

            try
            {
                // Генерим случайное имя для нового источника
                string tmpName = Guid.NewGuid().ToString();
                tmpName = tmpName.Replace("-", "");

                // Добавляем источник в DSN
                string connectionStr = string.Format(
                    "DSN={0};Description=Временный источник данных;Exclusive=1;ReadOnly=1;FIL=dBase 5.0;" +
                    "DefaultDir={1};CollatingSequence=ASCII;Deleted=01;PageTimeout=5;Statistics=0;UserCommitSync=Yes;",
                    tmpName, dbfDir);
                SQLConfigDataSource(0, ODBC_ADD_SYS_DSN, DBF_DRIVER_NAME, connectionStr);

                // Строка подключения
                connectionStr = connectionStr + "DriverId=533";
                OdbcConnection OdbcCon = new OdbcConnection(connectionStr);
                con = (IDbConnection)OdbcCon;

                // Запоминаем источник, чтобы потом замочить
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
        /// Удаляет указанный источник
        /// </summary>
        /// <param name="sourceName">Имя источника</param>
        /// <returns>Строка ошибки</returns>
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
        /// Удаляет все созданные временные источники ODBC
        /// </summary>
        /// <returns>Строка ошибки</returns>
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
        /// Создает подключение к источнику
        /// </summary>
        /// <param name="dir">Источник</param>
        /// <returns>Объект БД</returns>
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
        /// Создает подключение к файлу dbf через Microsoft FoxPro VFP Driver (*.dbf)
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns>Объект БД</returns>
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
                    "Ошибка при создании соединения с источником {0}.", dir));
            }

            db = new Database(conn, System.Data.Common.DbProviderFactories.GetFactory("System.Data.Odbc"), false);
        }
    }
}
