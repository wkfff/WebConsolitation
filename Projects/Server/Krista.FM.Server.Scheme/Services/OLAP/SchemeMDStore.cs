using System;
using System.Data.OleDb;

using Krista.FM.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Services.OLAP
{
    /// <summary>
	/// Объект реализующий доступ к многомерной базе данных.
	/// </summary>
    //[System.Diagnostics.DebuggerStepThrough()]
    internal class SchemeMDStore : DisposableObject, ISchemeMDStore
	{
       
		// Строка подключения.
		private OlapConnectionString connectionString;

		// Версия сервера, необходима для того,
		// чтобы определить к AS2000 мы подключились или к SSAS2005.
		private string serverVersion;

        /// <summary>
        /// Объект для доступа к многомерной базе данных.
        /// </summary>
        private OlapDatabase olapDatabase;


        /// <summary>
        /// Конструктор объекта.
        /// </summary>
        private SchemeMDStore()
		{
		}

        /// <summary>
        /// Экземпляр класса.
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
		/// Инициализирует строку с версией сервера.
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
					"Ошибка подключения к многомерной базе через OleDb: {0}", e.Message);
			    return false;
			}
			finally
			{
				connection.Dispose();
			}

		    return true;
		}


        /// <summary>
		/// Разбирает строку с версией сервера. Если разобрать не удалось - генерирует исключение.
		/// </summary>
		/// <returns>true, если подключены к SSAS2005</returns>
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
				string.Format("Неизвестная версия сервера: {0}", serverVersion));
		}

        /// <summary>
        /// Инициализация многомерного хранилища
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
                Trace.TraceError("Инициализация многомерной базы завершена с ошибкой: {0}", e.Message);
                return false;
            }
            finally
            {
                Trace.Unindent();
            }
        }

        /// <summary>
        /// Объект для доступа к многомерной базе данных.
        /// </summary>
        internal OlapDatabase OlapDatabase
        {
            get { return olapDatabase; }
        }

        /// <summary>
        /// Объект для доступа к многомерной базе данных.
        /// </summary>
        IOlapDatabase ISchemeMDStore.OlapDatabase
        {
            get { return olapDatabase; }
        }

        /// <summary>
        /// Имя сервера многомерной бызы
        /// </summary>
        public string ServerName
        {
            get { return connectionString.DataSource; }
        }

        /// <summary>
        /// Имя многомерной бызы
        /// </summary>
        public string CatalogName
        {
            get { return connectionString.InitialCatalog; }
        }

        /// <summary>
        /// Имя реляционной базы, на которую настроена многомерная
        /// </summary>
        public string OlapDataSourceName
        {
            get { return olapDatabase.OlapDataSourceName; }
        }

        #region ISchemeMDStore Members

        /// <summary>
        /// Версия многоменной базы
        /// </summary>
        public string DatabaseVersion
        {
            get { return olapDatabase.DatabaseVersion; }
        }

        /// <summary>
        /// Имя сервера реляционной базы, на которую настроена многомерная
        /// </summary>
        public string OlapDataSourceServer
        {
            get { return olapDatabase.OlapDataSourceServer; }
        }

        #endregion
    }
}
