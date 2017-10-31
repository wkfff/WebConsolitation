using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumpManagement
{
	/// <summary>
	/// Коллекция элементов реестра закачек
	/// </summary>
    public class PumpHistoryCollection : DisposableObject, IPumpHistoryCollection, ICollection, IEnumerable
    {
        #region Поля

        // Список элементов
		private SortedList list;
		// Схема
		private IScheme scheme;
		// Ссылка на реестр закачек
		int pumpRegistryID;

        #endregion Поля


        #region Инициализация

        /// <summary>
		/// Конструктор объекта
		/// </summary>
        public PumpHistoryCollection(IScheme scheme, int PumpRegistryID)
		{
            this.scheme = scheme;

			pumpRegistryID = PumpRegistryID;
			list = new SortedList(100);

			Initialize();
		}

		/// <summary>
		/// Инициализация коллекции из базы данных
		/// </summary>
		private void Initialize()
		{
            IDatabase db = scheme.SchemeDWH.DB;

			try
			{
				DataTable dt = (DataTable)db.ExecQuery(
                    "select H.ID, H.ProgramIdentifier, H.ProgramConfig, H.SystemVersion, H.ProgramVersion, H.PumpDate, " +
                    "	H.StartedBy, H.RefPumpRegistry, H.Comments, H.BatchID, H.UserName, H.UserHost, H.SessionID " +
                    "from PumpHistory H " +
					"where H.RefPumpRegistry = ?",
					QueryResultTypes.DataTable,
					db.CreateParameter("PumpRegistryID", pumpRegistryID));

				foreach (DataRow row in dt.Rows)
				{
					PumpHistoryElement item = (PumpHistoryElement)CreateElement((string)row[1]);
					item.ID = Convert.ToInt32(row[0]);
					if (!row.IsNull(2)) item.ProgramConfig = (string)row[2];
					item.SystemVersion = (string)row[3];
					item.ProgramVersion = (string)row[4];
					item.PumpDate = Convert.ToDateTime(row[5]);
					item.StartedBy = Convert.ToInt32(row[6]);
					//item.DataSource = Convert.ToInt32(row[7]);
					if (!row.IsNull(8))
                        item.Description = (string)row[8];
                    if (!row.IsNull(9))
                        item.BatchID = (string)row[9];
                    if (!row.IsNull(10))
                        item.UserName = (string)row[10];
                    if (!row.IsNull(11))
                        item.UserHost = (string)row[11];
                    if (!row.IsNull(12))
                        item.SessionID = (string)row[12];
                    list.Add(item.ID, item);
				}
			}
			finally
			{
				db.Dispose();
			}
        }

        #endregion Инициализация


        #region Реализация IPumpHistoryCollection

        /// <summary>
		/// Возвращает количество елементов в коллекции
		/// </summary>
		public int Count
		{
			get { return list.Count; }
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public int Add(object value)
		{
			int result = -1;
			PumpHistoryElement his = (PumpHistoryElement)value;
            IDatabase db = scheme.SchemeDWH.DB;
			try
			{
				his.ID = (int)db.GetGenerator("g_pumphistory");
				db.ExecQuery(
					"insert into PumpHistory (ID, ProgramIdentifier, ProgramConfig, SystemVersion, ProgramVersion, " +
					"PumpDate, StartedBy, RefPumpRegistry, Comments, BatchID, UserName, UserHost, SessionID) " +
					"values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
					QueryResultTypes.NonQuery,
					db.CreateParameter("ID", his.ID),
					db.CreateParameter("ProgramIdentifier", his.ProgramIdentifier),
					db.CreateParameter("ProgramConfig", his.ProgramConfig, DbType.AnsiString),
					db.CreateParameter("SystemVersion", his.SystemVersion),
					db.CreateParameter("ProgramVersion", his.ProgramVersion),
					db.CreateParameter("PumpDate", his.PumpDate, DbType.DateTime),
					db.CreateParameter("StartedBy", his.StartedBy),
					db.CreateParameter("RefPumpRegistry", pumpRegistryID),
                    db.CreateParameter("Comments", his.Description, DbType.AnsiString),
                    db.CreateParameter("BatchID", his.BatchID, DbType.AnsiString),
                    db.CreateParameter("UserName", his.UserName, DbType.AnsiString),
                    db.CreateParameter("UserHost", his.UserHost, DbType.AnsiString),
                    db.CreateParameter("SessionID", his.SessionID, DbType.AnsiString));
				list.Add(his.ID, his);

				result = his.ID;
			}
			catch (Exception ex)
			{
                Trace.WriteLine(ex, "PumpHistoryCollection");
                throw;
			}
			finally
			{
				db.Dispose();
			}

			return result;
		}

		/// <summary>
		/// Создает элемент коллекции
		/// </summary>
		/// <returns>Созданный элемент</returns>
        public IPumpHistoryElement CreateElement(string programIdentifier)
		{
            PumpHistoryElement elem = new PumpHistoryElement(scheme);
            elem.ProgramIdentifier = programIdentifier;
            return (IPumpHistoryElement)elem;
		}

		/// <summary>
		/// Индексатор возвращает элемент реестра закачек с указанным ключом,
		/// если ключа нет, то возвращает null
		/// </summary>
		public IPumpHistoryElement this[int key]
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				if (!list.Contains(key))
					return null;
				return (IPumpHistoryElement)list[key];
			}
		}

        /// <summary>
        /// Удалить запись истории
        /// </summary>
        /// <param name="index">ИД записи</param>
        /// <returns>Сооббщение об ошибке</returns>
        public string RemoveAt(int index)
        {
            IDatabase db = scheme.SchemeDWH.DB;

            try
            {
                // Удаляем данные из таблицы связи
                db.ExecQuery("delete from datasources2pumphistory where refpumphistory = ?",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("refpumphistory", index));

                // Удаляем данные из таблицы истории
                db.ExecQuery("delete from pumphistory where id = ?",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("id", index));

                // Удаляем элемент списка
                if (list.ContainsKey(index)) list.Remove(index);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                db.Dispose();
            }
        }

		#endregion Реализация IPumpHistoryCollection


		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public bool IsSynchronized
		{
			get { return this.list.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return this.list.SyncRoot; }
		}

		#endregion


		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		#endregion
	}
}
