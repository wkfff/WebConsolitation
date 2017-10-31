using System;
using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Users;


namespace Krista.FM.Server.DataPumpManagement
{
	/// <summary>
	/// Коллекция элементов реестра закачек
	/// </summary>
	public class PumpRegistryCollection : DisposableObject, IPumpRegistryCollection
	{
        #region Поля
        
        // Список элементов
		private SortedList list;
		// Схема
		private IScheme scheme;

        #endregion Поля


        #region Инициализация

        /// <summary>
		/// Конструктор объекта
		/// </summary>
        public PumpRegistryCollection(IScheme scheme)
		{
            this.scheme = scheme;
			list = new SortedList(100);
        }

        #endregion Инициализация


        #region Общие функции

        /// <summary>
        /// Регистрация закачки в системе безопасности
        /// </summary>
        /// <param name="progID">ИД закачки</param>
        /// <param name="description">Описание</param>
		/// <param name="usersManager"></param>
		public static void RegisterPumpProgram(string progID, string description, UsersManager usersManager)
        {
            if (usersManager != null)
            {
                usersManager.RegisterSystemObject(progID, description, SysObjectsTypes.DataPump);
            }
        }

        /// <summary>
        /// Удаление регистрации закачки в системе безопасности
        /// </summary>
        /// <param name="progID">ИД закачки</param>
		/// <param name="usersManager"></param>
		public static void UnregisterPumpProgram(string progID, UsersManager usersManager)
        {
            if (usersManager != null)
            {
                usersManager.UnregisterSystemObject(progID);
            }
        }

		private PumpRegistryElement GetRegistryElement(DataRow row)
		{
			PumpRegistryElement item = (PumpRegistryElement)CreateElement();

			item.ID = Convert.ToInt32(row[0]);
			item.SupplierCode = Convert.ToString(row[1]);
			item.DataCode = Convert.ToString(row[2]);
			item.ProgramIdentifier = Convert.ToString(row[3]);
			//if (!row.IsNull(4)) item.ProgramConfig = Convert.ToString(row[4]);
			if (!row.IsNull(5)) item.Description = Convert.ToString(row[5]);
			if (!row.IsNull(6)) item.Name = Convert.ToString(row[6]);
			if (!row.IsNull(7)) item.PumpProgram = Convert.ToString(row[7]);

			item.Initialize();
			item.RequestPumpHistory();
			return item;
		}

		#endregion Общие функции


        #region Реализация IPumpRegistryCollection

		/// <summary>
		/// Создает элемент реестра закачек
		/// </summary>
		/// <returns>Созданный элемент</returns>
		public IPumpRegistryElement CreateElement()
		{
			return new PumpRegistryElement(scheme);
		}

		/// <summary>
		/// Индексатор возвращает элемент реестра закачек с указанным ключом,
		/// если ключа нет, то возвращает null. key - ProgramIdentifier закачки.
		/// </summary>
		public IPumpRegistryElement this[string key]
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				if (list.ContainsKey(key))
					return (IPumpRegistryElement) list[key];

				using (IDatabase db = scheme.SchemeDWH.DB)
				{
					DataTable dt = DataPumpInfo.PumpRegistryDataTable(db, String.Format("ProgramIdentifier = '{0}'", key));
					if (dt.Rows.Count == 0)
						return null;

					IPumpRegistryElement pre = GetRegistryElement(dt.Rows[0]);
					list.Add(pre.ProgramIdentifier, pre);

					return pre;
				}
			}
		}

		#endregion Реализация IPumpRegistryCollection
	}
}
