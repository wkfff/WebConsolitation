using System;
using System.Collections.Generic;
using System.Data;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataSourcesManager
{
	/// <summary>
	/// Менеджер источников данных, реестра закачек и программ закачек
	/// </summary>
	public class DataSourceManager : DisposableObject, IDataSourceManager
	{
	    public static readonly string DefaultCurrentVersionName = string.Format("ФО\\0022 Классификаторы - {0}", DateTime.Now.Year);

	    // Ссылка на объект схемы
		private IScheme scheme;
		// Коллекция элементов источников данных
		private DataSourceCollection dataSourceCollection;
        // Список поставщиков данных
        private DataSupplierCollection dataSuppliers;


        /// <summary>
		/// Конструктор объекта
		/// </summary>
		/// <param name="scheme">Ссылка на интерфейс объекта схемы</param>
		public DataSourceManager(IScheme scheme)
		{
			if (scheme == null)
				throw new ArgumentNullException("scheme");
			
			this.scheme = scheme;

            // Инициализация списка поставщиков данных
            dataSuppliers = new DataSupplierCollection(this);

			dataSourceCollection = new DataSourceCollection(this, null);
		}

        /// <summary>
        /// Путь к файлу с настройками поставщиков данных
        /// </summary>
        public string DataSupliersFilePath
        {
            get { return scheme.BaseDirectory + "\\DataSources.xml"; }
        }


		#region Реализация интерфейса IDataSourceManager

		/// <summary>
		/// Путь к каталогу с исходными данными
		/// </summary>
		public string BaseDirectory
		{
			get { return scheme.BaseDirectory + "\\DataSources"; }
		}

        /// <summary>
        /// Путь к каталогу архива
        /// </summary>
        public string ArchiveDirectory
        {
            get { return scheme.BaseDirectory + "\\Archive"; }
        }

		public IDataSourceCollection DataSources
		{
			get { return dataSourceCollection; }
		}

		/// <summary>
		/// Интерфейс для доступа к объектам схемы
		/// </summary>
		public IScheme Scheme
		{
			get { return this.scheme; }
		}

        /// <summary>
        /// Список поставщиков данных
        /// </summary>
        public IDataSupplierCollection DataSuppliers
        {
            get { return dataSuppliers; }
        }

        /// <summary>
        /// получение данных по источникам
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataSourcesInfo()
        {
            IDatabase db = null;
            DataTable dt = null;
            try
            {
                db = scheme.SchemeDWH.DB;
                dt = (DataTable)db.ExecQuery(
                    "select ID, SupplierCode, DataCode, DataName, KindsOfParams, Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted, Confirmed from DataSources order by ID",
                    QueryResultTypes.DataTable);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// Получение данных по источникам
        /// </summary>
        /// <param name="dataSourceKinds">Набор видов поступающей информации, 
        /// чтобы на клиенте можно было задать только параметр источника</param>
        /// <returns></returns>
        public DataTable GetDataSourcesInfo(string dataSourceKinds)
        {
            IDatabase db = null;
            DataTable dt = null;

            string restrictionExpr = GetRestrictionExpr(dataSourceKinds);
            try
            {
                db = scheme.SchemeDWH.DB;
                dt = (DataTable)db.ExecQuery(String.Format(
                    "select ID, SupplierCode, DataCode, DataName, KindsOfParams, Name, Year, Month, Variant, Quarter, Territory, Locked, Deleted, Confirmed from DataSources {0} order by ID",
                    restrictionExpr),
                    QueryResultTypes.DataTable);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// Орнаничевающее выборку выражение
        /// </summary>
        /// <param name="dataSourceKinds"></param>
        /// <returns></returns>
        private string GetRestrictionExpr(string dataSourceKinds)
        {
            // пустой запрос
            if (String.IsNullOrEmpty(dataSourceKinds))
                return " WHERE 1 = -1";

            string restriction = String.Empty;
            string[] parts = dataSourceKinds.Split(';');

            for (int i = 0; i < parts.Length; i++)
            {
                if (i == 0)
                    restriction = String.Format(" WHERE (SupplierCode = '{0}' AND DataCode = {1:N}) ", 
                        parts[i].Split('\\')[0], parts[i].Split('\\')[1]);
                else
                    restriction += String.Format(" OR (SupplierCode = '{0}' AND DataCode = {1:N}) ",
                         parts[i].Split('\\')[0], parts[i].Split('\\')[1]);
            }

            return restriction;
        }

        /// <summary>
        /// Определяет описание и параметры источника данных
        /// </summary>
        /// <param name="SourceID">ID источника данных</param>
        /// <returns>Описание и параметры источника данных</returns>
        public string GetDataSourceName(int SourceID)
        {
            if (SourceID == 0)
            {
                return DefaultCurrentVersionName;
            }

            IDatabase DB = scheme.SchemeDWH.DB;
            try
            {
                string dsName = "Источник не найден";
                DataTable dt = (DataTable)DB.ExecQuery("select ID, SupplierCode, DataCode, DataName, KindsOfParams, Name, Year, Month, Variant, Quarter, Territory from DataSources where ID = ?",
                    QueryResultTypes.DataTable, DB.CreateParameter("ID", SourceID));
                if (dt.Rows.Count > 0)
                {
                    string paramsName = String.Empty;
                    try
                    {
                        DataRow row = dt.Rows[0];
                        switch ((ParamKindTypes)Convert.ToInt32(row[4]))
                        {
                            case ParamKindTypes.Budget:
                                paramsName = String.Format("{0} {1}", row[5], row[6]);
                                break;
                            case ParamKindTypes.YearTerritory:
                                paramsName = String.Format("{0} {1}", row[6], row[10]);
                                break;
                            case ParamKindTypes.Year:
                                paramsName = String.Format("{0}", row[6]);
                                break;
                            case ParamKindTypes.YearMonth:
                                paramsName = String.Format("{0}-{1:D2}", row[6], Convert.ToInt32(row[7]));
                                break;
                            case ParamKindTypes.YearMonthVariant:
                                paramsName = String.Format("{0}-{1:D2} вариант: {2}", row[6], Convert.ToInt32(row[7]), row[8]);
                                break;
                            case ParamKindTypes.YearQuarter:
                                paramsName = String.Format("{0} квартал {1}", row[6], row[9]);
                                break;
                            case ParamKindTypes.YearQuarterMonth:
                                paramsName = String.Format("{0} квартал: {1} месяц: {2:D2}", row[6], row[9], Convert.ToInt32(row[7]));
                                break;
                            case ParamKindTypes.YearVariant:
                                paramsName = String.Format("{0} вариант: {1}", row[6], row[8]);
                                break;
                            case ParamKindTypes.WithoutParams:
                                paramsName = String.Empty;
                                break;
                            case ParamKindTypes.Variant:
                                paramsName = String.Format("вариант: {0}", row[8]);
                                break;
                            case ParamKindTypes.YearVariantMonthTerritory:
                                paramsName = String.Format("{0} вариант: {1} месяц: {2:D2} территория: {3}", row[6], row[8], Convert.ToInt32(row[7]), row[10]);
                                break;
                        }
                    }
                    catch (Exception exp)
                    {
                        paramsName = "Ошибка: " + exp.Message;
                    }
                    dsName = String.Format("{0}\\{1:D4} {2} {3}", dt.Rows[0][1], Convert.ToInt32(dt.Rows[0][2]), dt.Rows[0][3], paramsName != String.Empty ? "- " + paramsName : String.Empty);
                }
                return dsName;
            }
            finally
            {
                DB.Dispose();
            }
        }

        /// <summary>
        /// Возвращает список источников по которым сформирован объект
        /// </summary>
        /// <param name="tableName">имя представления в базе данных. Должно получаться из свойства ICommonObject.FullDBName</param>
        /// <returns>Key - ID источника данных; Value - описание источника</returns>
        public Dictionary<int, string> GetDataSourcesNames(string tableName)
        {
            return GetDataSourcesNames(tableName, null);
        }

        /// <summary>
        /// Возвращает список источников по которым сформирован объект
        /// </summary>
        /// <param name="tableName">имя представления в базе данных. Должно получаться из свойства ICommonObject.FullDBName</param>
        /// <returns>Key - ID источника данных; Value - описание источника</returns>
        public Dictionary<int, string> GetDataSourcesNames(string tableName, string whereClauseCondition)
        {
            Dictionary<int, string> list = new Dictionary<int, string>();
            
            string whereClause = String.Empty;
            if (!String.IsNullOrEmpty(whereClauseCondition))
                whereClause = " where " + whereClauseCondition;

            using (IDatabase db = Scheme.SchemeDWH.DB)
            {
                IDbCommand cmd = ((Database)db).InitCommand(null);
                cmd.CommandText = String.Format("select distinct SourceID from {0}{1}", tableName, whereClause);
                //conn.Open();
                IDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string sourceName = "Нет источника";
                    if (!dr.IsDBNull(0))
                    {
                        sourceName = Scheme.DataSourceManager.GetDataSourceName(Convert.ToInt32(dr.GetValue(0)));
                        list.Add(Convert.ToInt32(dr.GetValue(0)), sourceName);
                    }
                    else
                        list.Add(-1, sourceName);
                }
                return list;
            }
        }

        /// <summary>
        /// Визвращает IDataUpdater доступный только для чтения для получения всех источников данных
        /// </summary>
        public IDataUpdater DataSourcesDataUpdater
        {
            get
            {
                IDatabase DB = Scheme.SchemeDWH.DB;
                IDataUpdater du = DB.GetDataUpdater("select ID, SupplierCode, DataCode, DataName, KindsOfParams, Name, Year, Month, Variant, Quarter from DataSources");
                DB.Dispose();
                return du;
            }
        }

        #endregion Реализация интерфейса IDataSourceManager
    }
}