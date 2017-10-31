using System;
using System.Data;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumpManagement
{
	/// <summary>
	/// Элемент реестра закачек.
	/// </summary>
    public class PumpHistoryElement : DisposableObject, IPumpHistoryElement
    {
        #region Поля

        private int _ID = -1;
		private string programIdentifier;
		private string programConfig;
		private string systemVersion;
		private string programVersion;
        private DateTime pumpDate;
		private int startedBy;
		private string description;
        private string batchID;
        private IScheme scheme;
        private string userName;
        private string userHost;
        private string sessionID;

        #endregion Поля


        #region Инициализация

        /// <summary>
		/// Конструктор объекта
		/// </summary>
        public PumpHistoryElement(IScheme scheme)
		{
            this.scheme = scheme;
			description = string.Empty;
			programConfig = string.Empty;
            batchID = string.Empty;
            userName = string.Empty;
            userHost = string.Empty;
            sessionID = string.Empty;
        }

        #endregion Инициализация


        #region Реализация IPumpHistoryElement

        #region Реализация методов

        /// <summary>
		/// Удаляет все данные закаченные по этой закачке
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void DeleteData()
		{
			
		}

		#endregion Реализация методов


		#region Реализация свойств

		/// <summary>
		/// ID записи элемента реестра закачек в базе данных
		/// </summary>
		public int ID
		{
			get { return _ID; }
			set
			{
                if (_ID == -1)
                {
                    _ID = value;
                }
			}
		}

		/// <summary>
		/// Идентификатор программы закачки. Максимальная длина 38 символов.
		/// </summary>
		public string ProgramIdentifier
		{
			get { return programIdentifier; }
			set
			{
				programIdentifier = value;
			}
		}

		/// <summary>
		/// Версия системы в формате XX.XX.XX
		/// </summary>
		public string SystemVersion
		{
			get { return systemVersion; }
			set
			{
				systemVersion = value;
			}
		}

		/// <summary>
		/// Версия программы(модуля) закачки в формате XX.XX.XX
		/// </summary>
		public string ProgramVersion
		{
			get { return programVersion; }
			set
			{
				programVersion = value;
			}
		}

		/// <summary>
		/// Дата и время когда была запущена закачка
		/// </summary>
        public DateTime PumpDate
		{
			get { return pumpDate; }
			set
			{
				pumpDate = value;
			}
		}

		/// <summary>
		/// Запущена пользователем или планировщиком по расписанию
		/// </summary>
		public int StartedBy
		{
			get { return startedBy; }
			set
			{
				startedBy = value;
			}
		}

		/// <summary>
		/// Текстовое описание элемента реестра закачек. Максимальная длина 2048 символов.
		/// </summary>
		public string Description
		{
			get { return description; }
			set
			{
				description = value;
			}
		}

		/// <summary>
		/// Конфигурационные параметры для закачки (в формате XML)
		/// </summary>
		public string ProgramConfig
		{
			get { return programConfig; }
			set
			{
				programConfig = value;
			}
		}

        /// <summary>
        /// Источники, закачанные по текущей записи истории
        /// </summary>
        public DataTable DataSources
        {
            get
            {
                Database dbs = (Database)scheme.SchemeDWH.DB;

                try
                {
                    return dbs.ExecQuery(
                        "select d.* from DATASOURCES d where d.ID in " +
                        "(select d2p.REFDATASOURCES from DATASOURCES2PUMPHISTORY d2p where d2p.REFPUMPHISTORY = ?)",
                        QueryResultTypes.DataTable,
                        dbs.CreateParameter("REFPUMPHISTORY", this.ID)) as DataTable;
                }
                finally
                {
                    dbs.Close();
                }
            }
        }

        /// <summary>
        /// Guid пакета обработки кубов
        /// </summary>
        public string BatchID
        {
            get { return batchID; }
            set
            {
                using (IDatabase db = scheme.SchemeDWH.DB)
                    db.ExecQuery("update PumpHistory set BatchID = ? where ID = ?", QueryResultTypes.NonQuery,
                        db.CreateParameter("BatchID", value, DbType.AnsiString),
                        db.CreateParameter("ID", ID));
                batchID = value;
            }
        }

        // имя пользователя
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
            }
        }

        // машина пользователя
        public string UserHost
        {
            get { return userHost; }
            set
            {
                userHost = value;
            }
        }

        // идентификатор сессии
        public string SessionID
        {
            get { return sessionID; }
            set
            {
                sessionID = value;
            }
        }

        #endregion Реализация свойств

		#endregion Реализация IPumpHistoryElement
	}
}
