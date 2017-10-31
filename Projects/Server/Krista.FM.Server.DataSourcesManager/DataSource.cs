using System;
using System.Data;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataSourcesManager
{
	/// <summary>
	/// Источник данных
	/// </summary>
	internal class DataSource : DisposableObject, IDataSource
	{
		private int _ID = -1;
		private string supplierCode;
		private string dataCode;
        private string dataName;
        private ParamKindTypes parametersType;
		private DataSourceManager dataSourceManager;
		private string name = String.Empty;
        private string terrirory = String.Empty;
		private int year = 0;
		private int month = 0;
		private string variant = String.Empty;
		private int quarter = 0;
	    private int locked = 0;
	    private int deleted = 0;


		public DataSource(DataSourceManager dataSourceManager)
		{
			if (dataSourceManager == null)
				throw new ArgumentNullException("dataSourceManager");
			this.dataSourceManager = dataSourceManager;
		}

        private void CheckWriteAccess()
        {
            if (_ID != -1)
                throw new InvalidOperationException("Свойство нельзя изменять.");
        }

		#region Реализация интерфейса IDataSource

		#region Реализация методов

		/// <summary>
		/// Удаляет все данные закаченные по этому источнику
		/// </summary>
		public void DeleteData()
		{
			// Для всех выполненных закачек выполнить метод удаления операции закачки	
		}
        
        /// <summary>
        /// Блокировка(закрытие) источника.
        /// </summary>
        public void LockDataSource()
        {
            locked = 1;
            UpdateDataSourceState();
            WriteIntoProtocol(DataSourceEventKind.ceSourceLock, String.Format("Источник ID={0} закрыт от изменений", ID));
        }

        /// <summary>
        /// Открытие источника.
        /// </summary>
        public void UnlockDataSource()
        {
            locked = 0;
            UpdateDataSourceState();
            WriteIntoProtocol(DataSourceEventKind.ceSourceUnlock, String.Format("Источник ID={0} открыт для изменений", ID));
        }

        /// <summary>
        /// Удаляет данные по источнику и ставит источнику признак удаления.
        /// </summary>
        /// <param name="dependedObjects">Таблица зависимых объектов.</param>
        public DataTable RemoveWithData(DataTable dependedObjects)
        {
            IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;
            DataTable dtResult = CreateDtResult();
            try
            {
                db.BeginTransaction();
                // Проходим по зависимым объектам и удаляем записи по источнику.
                foreach (DataRow row in dependedObjects.Rows)
                {
                    // Сначала по таблицам фактов.
                    if ((row["ObjectType"].ToString()).Equals("Таблица фактов"))
                    {
                        DataTable innerResult = ProcessLockedVariant(db, row["FullDBName"].ToString());
                        foreach (DataRow innerRow in innerResult.Rows)
                        {
                            AddIfNoExistsResultRow(ref dtResult, innerRow);
                       }
                    }
                }

                // Потом по всем остальным.
                foreach (DataRow row in dependedObjects.Rows)
                {
                    if (!(row["ObjectType"].ToString()).Equals("Таблица фактов"))
                    {
                        DataTable innerResult = ProcessDeleteChildRecord(db, row["FullDBName"].ToString());
                        foreach (DataRow innerRow in innerResult.Rows)
                        {
                            AddIfNoExistsResultRow(ref dtResult, innerRow);
                        }
                    }
                }
                if (dtResult.Rows.Count > 0)
                {
                    db.Rollback();
                    db.Dispose();
                    return dtResult;
                } 

                // Ставим источнику признак удаления.
                deleted = 1;
                db.ExecQuery(string.Format("update HUB_DataSources set DELETED = ? where ID = ?"),
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("Deleted", deleted),
                    db.CreateParameter("ID", ID));

                db.Commit();
                WriteIntoProtocol(DataSourceEventKind.ceSourceDelete, String.Format("Источник ID={0} удален", ID));
                return null;
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw new Exception("Ошибка при удалении источника данных", ex);
            }
            finally
            {
                db.Dispose();
            }
        }

        // Добавляет в таблицу результатов строку, если такой еще не было.
        private static void AddIfNoExistsResultRow(ref DataTable dtResult, DataRow innerRow)
        {
            foreach (DataRow row in dtResult.Rows)
            {
                if (row["FullDBName"].ToString() == innerRow["FullDBName"].ToString() &&
                    row["ID"].ToString() == innerRow["ID"].ToString() )
                {
                    return;
                }
                
            }
            dtResult.ImportRow(innerRow);
        }

	    private DataTable ProcessDeleteChildRecord(IDatabase db, string clsName)
	    {
            // Ищем классификатор по имени.
	        IClassifier cls = null;
	        foreach (IClassifier item in this.dataSourceManager.Scheme.Classifiers.Values)
	        {
	            if (item.FullDBName.Equals(clsName))
	            {
	                cls = item;
	                break;
	            }
	        }

	        // Выбираем записи, которые должны быть удалены
	        DataTable dtRefKdID = (DataTable)db.ExecQuery(string.Format
                  ("select ID from {0} where sourceID = ?", cls.FullDBName),
                  QueryResultTypes.DataTable, db.CreateParameter("ID", ID));

            DataTable dtResult = CreateDtResult();

	        // Проверяем для каждого ID
            foreach (DataRow refKdIDRow in dtRefKdID.Rows)
	        {
                // Просматриваем ассоциации.
                foreach (IEntityAssociation item in cls.Associated.Values)
	            {
                    // Считаем зависимые по другим источникам
                    DataTable dtOtherSourceData = new DataTable();
                    if (item.RoleData.ClassType != ClassTypes.Table)
                    {
                        dtOtherSourceData = (DataTable)(db.ExecQuery
                            (String.Format(
                            "select ID from {0} where {1} = ? and sourceID <> ?", item.RoleData.FullDBName, item.RoleDataAttribute.Name),
                            QueryResultTypes.DataTable,
                            db.CreateParameter("ID", Convert.ToInt32(refKdIDRow[0])),
                            db.CreateParameter("sourceID", ID)));
                    }

                    // Если они есть
	                foreach (DataRow otherSourceDataRow  in dtOtherSourceData.Rows)
	                {
                        // Заносим имя классификатора и ID записи в черный список
                        DataRow resultRow = dtResult.NewRow();
                        resultRow["FullCaption"] = item.RoleData.FullCaption;
                        resultRow["FullDBName"] = item.RoleData.FullDBName;
                        resultRow["ID"] = otherSourceDataRow[0];
                        resultRow["ObjectType"] = item.RoleData.GetObjectType();
                        dtResult.Rows.Add(resultRow);
	                }

                    // Если в зависимых таблица фактов
                    if (item.RoleData is IFactTable)
                    {
                        // перед удалением проверяем, не ссылается ли она на заблокированный вариант
                        DataTable innerResult = ProcessLockedVariant(db, item.RoleData.FullDBName);
                        foreach (DataRow innerRow in innerResult.Rows)
                        {
                            dtResult.ImportRow(innerRow);
                        }

                    }
                    if (item.RoleData.ClassType != ClassTypes.Table)
                    {
                        if (dtResult.Rows.Count == 0)
                        {
                            // Пытаемся удалить данные
                            db.ExecQuery(
                                String.Format("delete from {0} where {1} = ? and sourceID = ?",
                                              item.RoleData.FullDBName, item.RoleDataAttribute.Name),
                                QueryResultTypes.NonQuery,
                                db.CreateParameter("RefKd", Convert.ToInt32(refKdIDRow[0])),
                                db.CreateParameter("sourceID", ID));
                        }
                    }
	            }
	        }
            if (dtResult.Rows.Count == 0)
            // Удаляем записи по источнику
            {
                db.ExecQuery(String.Format("delete from {0} where sourceID = ?", cls.FullDBName),
                             QueryResultTypes.NonQuery,
                             db.CreateParameter("sourceID", ID));
            }
            return dtResult;
	    }

	    /// <summary>
        /// Обработка исключения с заблокированным вариантом.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="row"></param>
	    private DataTable ProcessLockedVariant(IDatabase db, string factTableName)
	    {
            // Ищем таблицу по имени.
	        IFactTable factTable = null;
	        foreach (IFactTable item in this.dataSourceManager.Scheme.FactTables.Values)
	        {
	            if (item.FullDBName.Equals(factTableName))
	            {
	                factTable = item;
	                break;
	            }
	        }

	        DataTable dtResult = CreateDtResult();

	        //Просматриваем ассоциации таблицы фактов
	        foreach (IAssociation association in factTable.Associations.Values)
	        {
	            // Ищем в них вариант
	            if (association.RoleBridge is IVariantDataClassifier)
	            {
	                IEntity variantCls = association.RoleBridge;
	                // Выбираем ID записей варианта, на которые ссылается таблица
	                DataTable dtRefVariantID = (DataTable)db.ExecQuery(string.Format
                       ("select distinct {0} from {1} where sourceID = ?", association.RoleDataAttribute.Name, factTable.FullDBName),
                       QueryResultTypes.DataTable, db.CreateParameter("ID", ID));

                    // Считаем сколько из этих записей заблокированы
	                foreach (DataRow dataRow in dtRefVariantID.Rows)
	                {
                        int count = Convert.ToInt32(db.ExecQuery
                            (String.Format("select count (*) from {0} where ID = ? and variantcompleted = 1", variantCls.FullDBName),
                            QueryResultTypes.Scalar, db.CreateParameter("ID", Convert.ToInt32(dataRow[0]))));
                        if (count > 0)
                        {
                            // Заносим имя варианта и ID записи в черный список
                            DataRow resultRow = dtResult.NewRow();
                            resultRow["FullCaption"] = variantCls.FullCaption;
                            resultRow["FullDBName"] = variantCls.FullDBName;
                            resultRow["ID"] = dataRow[0];
                            resultRow["ObjectType"] = variantCls.GetObjectType();
                            dtResult.Rows.Add(resultRow);
                        }
	                }
	            }
	        }
            // Если ошибок не было
            if (dtResult.Rows.Count == 0)
            {
                // Пытаемся удалить данные.
                db.ExecQuery(String.Format("delete from {0} where sourceID = ?", factTable.FullDBName),
                     QueryResultTypes.NonQuery,
                     db.CreateParameter("sourceID", ID));
            }
            return dtResult;
	    }

	    private static DataTable CreateDtResult()
	    {
	        DataTable dtResult = new DataTable();
	        DataColumn colFullCaption = new DataColumn("FullCaption", Type.GetType("System.String"));
	        DataColumn colFullDBName = new DataColumn("FullDBName", Type.GetType("System.String"));
	        DataColumn colID = new DataColumn("ID", Type.GetType("System.Int32"));
            DataColumn colObjectType = new DataColumn("ObjectType", Type.GetType("System.String"));
	        dtResult.Columns.Add(colFullCaption);
	        dtResult.Columns.Add(colFullDBName);
	        dtResult.Columns.Add(colID);
	        dtResult.Columns.Add(colObjectType);
	        return dtResult;
	    }

        /// <summary>
        /// Производит поиск данного источника данных в коллекции
        /// </summary>
        /// <returns></returns>
        public int? FindInDatabase()
        {
            return dataSourceManager.DataSources.FindDataSource(this);
        }

        /// <summary>
        /// Сохраняет данный источник данных в коллекции и в базе данных
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            return dataSourceManager.DataSources.Add(this);
        }

	    public void ConfirmDataSource()
	    {
            IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;
	        try
	        {
                UpdateDataSourceConfirmed(db, 1);
                WriteIntoProtocol(DataSourceEventKind.ceSourceLock, String.Format("Источник ID={0} переведен в состояние Утвержден", ID));
	        }
	        finally
	        {
	            db.Dispose();
	        }
	    }

	    public void UnConfirmDataSource()
	    {
            IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;
            try
            {
                UpdateDataSourceConfirmed(db, 0);
                WriteIntoProtocol(DataSourceEventKind.ceSourceLock, String.Format("Источник ID={0} переведен в состояние Не проверен", ID));
            }
            finally
            {
                db.Dispose();
            }
	    }

	    #endregion Реализация методов


		#region Реализация свойств

		/// <summary>
		/// ID источника данных
		/// </summary>
		public int ID 
		{ 
			get { return _ID; }
			set { _ID = value; }
		}

		/// <summary>
		/// Код поставщика данных
		/// </summary>
		public string SupplierCode 
		{ 
			get { return supplierCode; }
			set {
                CheckWriteAccess();
                supplierCode = value; 
            }
		}

		/// <summary>
		/// Порядковый номер поступивщей информации
		/// </summary>
		public string DataCode	
		{ 
			get	{ return dataCode; }
            set
            {
                CheckWriteAccess();
                dataCode = value;
            }
		}

        /// <summary>
        /// Наименование поступающей имформации
        /// </summary>
        public String DataName
        {
            get { return dataName; }
            set
            {
                CheckWriteAccess();
                dataName = value;
            }
        }
        
        /// <summary>
		/// Вид параметров источника данных
		/// </summary>
		public ParamKindTypes ParametersType
		{ 
			get	{ return parametersType; }
            set
            {
                CheckWriteAccess();
                parametersType = value;
            }
		}

		/// <summary>
		/// Вид параметров: наименование бюджета
		/// </summary>
		public string BudgetName
		{ 
			get	{ return name; }
            set
            {
                CheckWriteAccess();
                name = value;
            }
		}

        /// <summary>
        /// Вид параметров: Территория
        /// </summary>
        public string Territory
        {
            get { return terrirory; }
            set
            {
                CheckWriteAccess();
                terrirory = value;
            }
        }

        /// <summary>
		/// Вид параметров: год
		/// </summary>
		public int Year 
		{ 
			get	{ return year; }
            set
            {
                CheckWriteAccess();
                year = value;
            }
		}

		/// <summary>
		/// Вид параметров: месяц
		/// </summary>
		public int Month
		{ 
			get	{ return month; }
            set
            {
                CheckWriteAccess();
                month = value;
            }
		}

		/// <summary>
		/// Вид параметров: вариант
		/// </summary>
		public string Variant 
		{ 
			get	{ return variant; }
            set
            {
                CheckWriteAccess();
                variant = value;
            }
		}

		/// <summary>
		/// Вид параметров: квартал
		/// </summary>
		public int Quarter
		{
			get { return quarter; }
            set
            {
                CheckWriteAccess();
                quarter = value;
            }
		}

	    /// <summary>
	    /// Признак блокировки источника.
	    /// </summary>
        public int Locked
	    {
            get { return locked; }
	    }

	    /// <summary>
	    /// Признак удаления источника.
	    /// </summary>
	    public int Deleted
	    {
            get { return deleted; }
	    }
        
		#endregion Реализация свойств

		#endregion Реализация интерфейса IDataSource

        
        private void UpdateDataSourceState()
        {
            IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;
            try
            {
                UpdateDataSourceState(db);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Изменяет поле состояния источника в базе данных.
        /// </summary>
        /// <param name="db">Объект доступа к базе данных.</param>
        private void UpdateDataSourceState(IDatabase db)
        {
            db.ExecQuery(String.Format("update HUB_DataSources set locked = ? where ID = ?"),
                QueryResultTypes.NonQuery,
                db.CreateParameter("Locked", locked),
                db.CreateParameter("SourceID", ID));
        }

        /// <summary>
        /// Изменяет поле признака утвержден или нет
        /// </summary>
        /// <param name="db">Объект доступа к базе данных.</param>
        private void UpdateDataSourceConfirmed(IDatabase db, int confirmed)
        {
            db.ExecQuery(String.Format("update HUB_DataSources set confirmed = ? where ID = ?"),
                QueryResultTypes.NonQuery,
                db.CreateParameter("Locked", confirmed),
                db.CreateParameter("SourceID", ID));
        }

        /// <summary>
        /// Запись события в протокол действий пользователя
        /// </summary>
        /// <param name="kind">Тип события</param>
        /// <param name="eventMsg">Текст сообщения</param>
        private void WriteIntoProtocol(DataSourceEventKind kind, string eventMsg)
        {
            IDataSourceProtocol log = null;
            try
            {
               log = (IDataSourceProtocol)dataSourceManager.Scheme.GetProtocol("Krista.FM.Server.Scheme.dll");
               log.WriteEventIntoDataSourceProtocol(kind, this.ID, eventMsg);
            }
            finally
            {
                if (log != null)
                    log.Dispose();
            }
        }
	}
}
