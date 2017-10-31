using System;
using System.Collections.Generic;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Services.OLAP
{
    /// <summary>
    /// Класс для доступа к многомерной базе данных.
    /// </summary>
    internal abstract class OlapDatabase : IDisposable, IOlapDatabase
    {
        /// <summary>
        /// Строка подключения.
        /// </summary>
        protected OlapConnectionString connectionString;

        /// <summary>
        /// Версия многомерной базы.
        /// </summary>
        protected string databaseVersion;

        /// <summary>
        ///  Определение версии многомерной базы
        /// </summary>
        protected abstract bool InitializeDatabaseVersion();

        /// <summary>
        /// Определяет, проверять или нет версию многомерной базы
        /// </summary>
        /// <returns></returns>
        protected static bool NeedCheckDatabasVersion()
        {            
            return (SchemeClass.Instance.Server.GetConfigurationParameter("NotInitializeDatabaseVersion") != null) ? true : false;       
        }
        /// <summary>
        /// Инициализация экземпляра объекта.
        /// </summary>
        /// <param name="connectionString">Строка подключения.</param>
        protected OlapDatabase(OlapConnectionString connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Возвращает объект доступа к серверу Analisys Service.
        /// </summary>
        public abstract Object ServerObject { get; }

        /// <summary>
        /// Возвращает объект доступа к базе данных.
        /// </summary>
        public abstract Object DatabaseObject { get; }

        /// <summary>
        /// Имя реляционной базы, на которую настроена многомерная.
        /// </summary>
        public abstract string OlapDataSourceName { get; }

        /// <summary>
        /// Сервер реляционной базы, на которую настроена многомерка
        /// </summary>
        public abstract string OlapDataSourceServer { get; }

        /// <summary>
        /// Выполняется подключение к базе данных.
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// Закрывает подключение к базе данных.
        /// </summary>
        public abstract void CloseConnection();

        /// <summary>
        /// Проверяет наличие продключения к базе данных, 
        /// в случаи потери соединения восстанавливает его.
        /// </summary>
        public abstract void CheckConnection();

        public abstract void Dispose();

        /// <summary>
        /// Версия сервера Analisys Services.
        /// </summary>
        public abstract string ServerVersion { get; }

        /// <summary>
        /// Версия многоменной базы.
        /// </summary>
        public string DatabaseVersion
        {
            get { return databaseVersion; }
        }

        /// <summary>
        /// Пишет в лог информацию об OLAP сервере.
        /// </summary>		
        protected static void WriteServerInfo(string serverName, string serverVersion, string repositoryVersion)
        {
            Trace.TraceInformation(String.Format(
                "Сервер: {0}, версия сервера: {1} {2}",
                serverName, serverVersion, repositoryVersion));
        }

        public OlapConnectionString ConnectionString
        {
            get { return connectionString; }
        }
    }
}
