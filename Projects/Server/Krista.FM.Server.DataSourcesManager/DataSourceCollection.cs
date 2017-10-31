using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Diagnostics;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataSourcesManager
{
    /// <summary>
    /// Коллекция источников данных из базы
    /// </summary>
    internal class DataSourceCollection : DisposableObject, IDataSourceCollection, ICollection
    {
        // Родительский объект
        private readonly DataSourceManager dataSourceManager;
        // родительский элемент
        private readonly IPumpHistoryElement pumpHistoryElement;
        private object syncRoot;

        /// <summary>
        /// Конструктор
        /// </summary>
        public DataSourceCollection(DataSourceManager dataSourceManager, IPumpHistoryElement pumpHistoryElement)
        {
            if (dataSourceManager == null)
                throw new ArgumentNullException("dataSourceManager");
            this.dataSourceManager = dataSourceManager;
            this.pumpHistoryElement = pumpHistoryElement;
            this.syncRoot = new object();
        }


        #region Реализация интерфейса IDataSourceCollection

        /// <summary>
        /// Определяет содержит ли коллекция указанный ключ
        /// </summary>
        /// <param name="key">ID источника данных</param>
        /// <returns>true если содержит указанный ключ; иначе false</returns>
        public bool Contains(int key)
        {
            Database db = (Database)dataSourceManager.Scheme.SchemeDWH.DB;
            try
            {
                int count;

                if (this.pumpHistoryElement == null)
                    count = Convert.ToInt32(db.ExecQuery("select Count(*) from DataSources where ID = ?", QueryResultTypes.Scalar, db.CreateParameter("ID", key)));
                else
                    count = Convert.ToInt32(db.ExecQuery(
                        "select Count(*) from DataSources S join DataSources2PumpHistory H on (S.ID = H.RefDataSources) where H.RefPumpHistory = ? and S.ID = ?",
                        QueryResultTypes.Scalar, db.CreateParameter("RefPumpHistory", pumpHistoryElement.ID), db.CreateParameter("ID", key)));

                return count != 0;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Ищет источник данных по его параметрам
        /// </summary>
        /// <param name="obj">Объект источника</param>
        /// <returns>ИД источника (null - не найден)</returns>
        public int? FindDataSource(object obj)
        {
            int? result = null;

            Database db = (Database)dataSourceManager.Scheme.SchemeDWH.DB;
            try
            {
                DataSource ds = (DataSource)obj;

                switch (ds.ParametersType)
                {
                    case ParamKindTypes.Budget:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and UPPER(NAME) = UPPER(?) and YEAR = ? and DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType),
                            db.CreateParameter("NAME", ds.BudgetName),
                            db.CreateParameter("YEAR", ds.Year)));
                        break;

                    case ParamKindTypes.Year:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and YEAR = ? and DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType),
                            db.CreateParameter("YEAR", ds.Year)));
                        break;

                    case ParamKindTypes.YearMonth:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and YEAR = ? and MONTH = ? and DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType),
                            db.CreateParameter("YEAR", ds.Year),
                            db.CreateParameter("MONTH", ds.Month)));
                        break;

                    case ParamKindTypes.YearMonthVariant:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and YEAR = ? and MONTH = ? and UPPER(VARIANT) = UPPER(?) and " +
                            "DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType),
                            db.CreateParameter("YEAR", ds.Year),
                            db.CreateParameter("MONTH", ds.Month),
                            db.CreateParameter("VARIANT", ds.Variant)));
                        break;

                    case ParamKindTypes.YearQuarter:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and YEAR = ? and QUARTER = ? and DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType),
                            db.CreateParameter("YEAR", ds.Year),
                            db.CreateParameter("QUARTER", ds.Quarter)));
                        break;

                    case ParamKindTypes.YearQuarterMonth:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and YEAR = ? and QUARTER = ? and MONTH = ? and DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType),
                            db.CreateParameter("YEAR", ds.Year),
                            db.CreateParameter("QUARTER", ds.Quarter),
                            db.CreateParameter("MONTH", ds.Month)));
                        break;

                    case ParamKindTypes.YearVariant:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and YEAR = ? and UPPER(VARIANT) = UPPER(?) and DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType),
                            db.CreateParameter("YEAR", ds.Year),
                            db.CreateParameter("VARIANT", ds.Variant)));
                        break;

                    case ParamKindTypes.YearTerritory:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and YEAR = ? and UPPER(TERRITORY) = UPPER(?) and DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType),
                            db.CreateParameter("YEAR", ds.Year),
                            db.CreateParameter("TERRITORY", ds.Territory)));
                        break;

                    case ParamKindTypes.WithoutParams:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType)));
                        break;

                    case ParamKindTypes.Variant:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and UPPER(VARIANT) = UPPER(?) and DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType),
                            db.CreateParameter("VARIANT", ds.Variant)));
                        break;

                    case ParamKindTypes.YearVariantMonthTerritory:
                        result = Convert.ToInt32(db.ExecQuery(
                            "select ID from DATASOURCES " +
                            "where UPPER(SUPPLIERCODE) = UPPER(?) and DATACODE = ? and " +
                            "KINDSOFPARAMS = ? and YEAR = ? and UPPER(VARIANT) = UPPER(?) and MONTH = ? and " +
                            "UPPER(TERRITORY) = UPPER(?) and DELETED <> 1",
                            QueryResultTypes.Scalar,
                            db.CreateParameter("SUPPLIERCODE", ds.SupplierCode),
                            db.CreateParameter("DATACODE", Convert.ToInt32(ds.DataCode)),
                            db.CreateParameter("KINDSOFPARAMS", (int)ds.ParametersType),
                            db.CreateParameter("YEAR", ds.Year),
                            db.CreateParameter("VARIANT", ds.Variant),
                            db.CreateParameter("MONTH", ds.Month),
                            db.CreateParameter("TERRITORY", ds.Territory)));
                        break;
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                db.Dispose();
            }

            if (result == 0) return null;

            return result;
        }

        /// <summary>
        /// Определяет, содержит ли коллекция указанный источник (среди всех источников)
        /// </summary>
        /// <param name="obj">Объект источника</param>
        /// <returns>ИД источника, -1 если не найден</returns>
        public bool Contains(object obj)
        {
            return FindDataSource(obj) != null;
        }

        /// <summary>
        /// Возвращает коллекцию ключей (ID источников)
        /// </summary>
        public ICollection Keys
        {
            get
            {
                ArrayList keyList = new ArrayList();
                Database db = (Database)dataSourceManager.Scheme.SchemeDWH.DB;
                try
                {
                    DataTable dt;
                    if (this.pumpHistoryElement == null)
                        dt = (DataTable)db.ExecQuery("select ID from DataSources", QueryResultTypes.DataTable);
                    else
                        dt = (DataTable)db.ExecQuery(
                            "select S.ID from DataSources S join DataSources2PumpHistory H on (S.ID = H.RefDataSources) where H.RefPumpHistory = ?",
                            QueryResultTypes.DataTable, db.CreateParameter("RefPumpHistory", pumpHistoryElement.ID));

                    foreach (DataRow row in dt.Rows)
                        keyList.Add(Convert.ToInt32(row[0]));

                    return keyList;
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// Возвращает количество елементов в коллекции
        /// </summary>
        public int Count
        {
            get
            {
                Database db = (Database)dataSourceManager.Scheme.SchemeDWH.DB;
                try
                {
                    int count;

                    if (this.pumpHistoryElement == null)
                        count = Convert.ToInt32(db.ExecQuery("select Count(*) from DataSources", QueryResultTypes.Scalar));
                    else
                        count = Convert.ToInt32(db.ExecQuery(
                            "select Count(*) from DataSources S join DataSources2PumpHistory H on (S.ID = H.RefDataSources) where H.RefPumpHistory = ?",
                            QueryResultTypes.Scalar, db.CreateParameter("RefPumpHistory", pumpHistoryElement.ID)));

                    return count;
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        /// <summary>
        /// Добавляет новый источник данных в базу данных.
        /// </summary>
        /// <param name="ds">Новый источник данных</param>
        /// <param name="db">Объект для доступа к БД</param>
        /// <remarks>
        /// Просто добавляет новую запись источника, без привязки к опрерации закачки
        /// </remarks>
        private static void AddNew(DataSource ds, IDatabase db)
        {
            // Добавляем в базу
            ds.ID = db.GetGenerator("g_DataSources");
            db.ExecQuery(
                "insert into HUB_DataSources " +
                "(ID, SupplierCode, DataCode, DataName, KindsOfParams, Year, Name, Month, Variant, Quarter, Territory, Locked, Deleted) " +
                "values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                QueryResultTypes.NonQuery,
                db.CreateParameter("ID", ds.ID),
                db.CreateParameter("SupplierCode", ds.SupplierCode),
                db.CreateParameter("DataCode", Convert.ToInt32(ds.DataCode)),
                db.CreateParameter("DataName", ds.DataName),
                db.CreateParameter("KindsOfParams", (int)ds.ParametersType),
                db.CreateParameter("Year", ds.Year),
                db.CreateParameter("Name", ds.BudgetName),
                db.CreateParameter("Month", ds.Month),
                db.CreateParameter("Variant", ds.Variant),
                db.CreateParameter("Quarter", ds.Quarter),
                db.CreateParameter("Territory", ds.Territory),
                db.CreateParameter("Locked", ds.Locked),
                db.CreateParameter("Deleted", ds.Deleted));
        }

        /// <summary>
        /// Добавляет источник в базу данных с привязкой к истории
        /// </summary>
        /// <param name="value">DataSource источник данных</param>
        /// <returns>ИД источника</returns>
        /// <param name="phe"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int Add(Object value, IPumpHistoryElement phe)
        {
            DataSource ds = (DataSource)value;
            IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;

            try
            {
                db.BeginTransaction();

                if (!dataSourceManager.DataSources.Contains(ds.ID))
                {
                    AddNew(ds, db);
                    this.dataSourceManager.Scheme.Processor.InvalidateDimension(
                        SystemSchemeObjects.SystemDataSources_ENTITY_KEY, "Krista.FM.Server.DataSourcesManager",
                        Krista.FM.Server.ProcessorLibrary.InvalidateReason.ClassifierChanged,
                        "Источники данных");
                }

                if (phe != null)
                {
                    // Вставляем связь с операцией закачки
                    // Но сначала проверим, нет ли такой записи
                    int count = Convert.ToInt32(db.ExecQuery(
                        "select count (RefDataSources) from DataSources2PumpHistory " +
                        "where RefDataSources = ? and RefPumpHistory = ?",
                        QueryResultTypes.Scalar,
                        db.CreateParameter("RefDataSources", ds.ID),
                        db.CreateParameter("RefPumpHistory", phe.ID)));
                    if (count == 0)
                    {
                        db.ExecQuery(
                            "insert into DataSources2PumpHistory (RefDataSources, RefPumpHistory) values (?, ?)",
                            QueryResultTypes.NonQuery,
                            db.CreateParameter("RefDataSources", ds.ID),
                            db.CreateParameter("RefPumpHistory", phe.ID));
                    }
                }

                db.Commit();

                return ds.ID;
            }
            catch(Exception e)
            {
                db.Rollback();
                throw new Exception(e.Message);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Добавляет новый источник в базу данных, с привязкой или без к операции закачки
        /// </summary>
        /// <param name="value">DataSource источник данных</param>
        /// <returns>ID добавленного источника</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int Add(object value)
        {
            using (IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB)
            {
                DataSource ds = (DataSource)value;
                if (!dataSourceManager.DataSources.Contains(ds))
                {
                    AddNew(ds, db);
					try
					{
						dataSourceManager.Scheme.Processor.InvalidateDimension(
							SystemSchemeObjects.SystemDataSources_ENTITY_KEY, "Krista.FM.Server.DataSourcesManager",
							Krista.FM.Server.ProcessorLibrary.InvalidateReason.ClassifierChanged,
							"Источники данных");
					}
					catch (InvalidateOlapObjectException e)
                	{
                		Trace.TraceError("При установке признака необходимости расчета для измерения \"Источники данных\" произошла ошибка: {0}", e.Message);
						return ds.ID;
                	}
                	return ds.ID;
                }
                return -1;
            }
        }

        /// <summary>
        /// Создает объект источника данных
        /// </summary>
        /// <returns>Созданный объект</returns>
        public IDataSource CreateElement()
        {
            return new DataSource(dataSourceManager);
        }

        /// <summary>
        /// Удаляет источник из базы
        /// </summary>
        /// <param name="index">ИД источника</param>
        /// <returns>Строка ошибки</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string RemoveAt(int index)
        {
            IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;
            try
            {
                db.BeginTransaction();

                db.ExecQuery("delete from HUB_DataSources where ID = ?",
                    QueryResultTypes.NonQuery, db.CreateParameter("ID", index));

                db.Commit();

                return string.Empty;
            }
            catch (Exception ex)
            {
                db.Rollback();
                return ex.ToString();
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Индексатор возвращает источник данных с указанным ключом (ID),
        /// если ключа нет, то возвращает null
        /// </summary>
        public IDataSource this[int key]
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                DataSource item = null;
                IDatabase db = dataSourceManager.Scheme.SchemeDWH.DB;

                try
                {
                    DataTable dt;
                    if (this.pumpHistoryElement != null)
                    {
                        dt = (DataTable)db.ExecQuery(
                            "select S.ID, S.SupplierCode, S.DataCode, S.DataName, S.KindsOfParams, S.Name, " +
                            "S.Year, S.Month, S.Variant, S.Quarter, S.Territory " +
                            "from DataSources S join DataSources2PumpHistory H on (S.ID = H.RefDataSources) " +
                            "where H.RefPumpHistory = ? and S.ID = ?",
                            QueryResultTypes.DataTable,
                            db.CreateParameter("RefPumpHistory", pumpHistoryElement.ID),
                            db.CreateParameter("ID", key));
                    }
                    else
                    {
                        dt = (DataTable)db.ExecQuery(
                            "select S.ID, S.SupplierCode, S.DataCode, S.DataName, S.KindsOfParams, S.Name, " +
                            "S.Year, S.Month, S.Variant, S.Quarter, S.Territory from DataSources S " +
                            "where S.ID = ?",
                            QueryResultTypes.DataTable,
                            db.CreateParameter("ID", key));
                    }

                    if (dt.Rows.Count == 1)
                    {
                        DataRow row = dt.Rows[0];
                        item = (DataSource)CreateElement();
                        item.SupplierCode = Convert.ToString(row[1]);
                        item.DataCode = Convert.ToString(row[2]);
                        item.DataName = Convert.ToString(row[3]);
                        item.ParametersType = (ParamKindTypes)Convert.ToInt32(row[4]);
                        if (!row.IsNull(5)) item.BudgetName = Convert.ToString(row[5]);
                        if (!row.IsNull(6)) item.Year = Convert.ToInt32(row[6]);
                        if (!row.IsNull(7)) item.Month = Convert.ToInt32(row[7]);
                        if (!row.IsNull(8)) item.Variant = Convert.ToString(row[8]);
                        if (!row.IsNull(9)) item.Quarter = Convert.ToInt32(row[9]);
                        if (!row.IsNull(10)) item.Territory = Convert.ToString(row[10]);
                        item.ID = Convert.ToInt32(row[0]);
                    }
                    return item;
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        #endregion Реализация интерфейса IDataSourceCollection


        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //this.list.CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this.SyncRoot; }
        }

        #endregion


        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            throw new Exception("Метод GetEnumerator() не реализован. Для доступа к элементам источника сначала нужно получить коллекцию ключей Key, а потом обращаться по ключу к нужному элементу.");
        }

        #endregion
    }
}
