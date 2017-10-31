using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // Модуль функций инициализации и управления объектами данных (ДатаАдаптерами, ДатаСетами и др.)

    /// <summary>
    /// Базовый класс для всех закачек.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {
        #region Константы

        /// <summary>
        /// Время ожидания выполнения запроса IDbCommand (в секундах)
        /// </summary>
        public const int constCommandTimeout = 3600;

        /// <summary>
        /// Количество знаков, до которого будет догонятся значение элемента составного ключа кэша
        /// </summary>
        public const int constTotalWidthForComplexKey = 20;

        // максимальное кол - во записей в датасете при сохранении в базу
        public const int MAX_DS_RECORDS_AMOUNT = 10000;

        #endregion Константы


        #region Структуры, перечисления

        /// <summary>
        /// Структура с данными строки классификатора (для использования в кэшах классификаторов)
        /// </summary>
        public class ClsRowData
        {
            /// <summary>
            /// ИД записи
            /// </summary>
            public int ID;

            /// <summary>
            /// Код классификатора
            /// </summary>
            public string Code;

            /// <summary>
            /// Наименование классификатора
            /// </summary>
            public string Name;

            /// <summary>
            /// Значение поля по выбору (указывается при заполнении кэша)
            /// </summary>
            public string Field1;

            /// <summary>
            /// Значение поля по выбору (указывается при заполнении кэша)
            /// </summary>
            public string Field2;

            /// <summary>
            /// Значение поля по выбору (указывается при заполнении кэша)
            /// </summary>
            public string Field3;

            /// <summary>
            /// Конструктор
            /// </summary>
            public ClsRowData(int id, string code, string name, string field1, string field2, string field3)
            {
                this.ID = id;
                this.Code = code;
                this.Name = name;
                this.Field1 = field1;
                this.Field2 = field2;
                this.Field3 = field3;
            }
        }

        #endregion Структуры, перечисления


        #region Функции для работы с IDbDataAdapter

        /// <summary>
        /// Оболочка для адаптера
        /// </summary>
        private class DbDataAdapterWrapper : IDbDataAdapter
        {
            /// <summary>
            /// Внутренний адаптер
            /// </summary>
            private IDbDataAdapter dbDataAdapter;

            /// <summary>
            /// Объект сервера описывающий сущьность базы данных
            /// </summary>
            private IEntity entity;

            public DbDataAdapterWrapper(IDbDataAdapter dbDataAdapter, IEntity entity)
            {
                this.dbDataAdapter = dbDataAdapter;
                this.entity = entity;
            }

            #region Проверка пакетного обновления
            /*
            private bool IsSqlServer()
            {
                DbConnection cn = (DbConnection)GetConnection();
                return cn.GetType().FullName == "System.Data.SqlClient.SqlConnection";
            }

            private IDbConnection GetConnection()
            {
                IDbTransaction trans;
                return GetConnection(out trans);
            }

            private IDbConnection GetConnection(out IDbTransaction trans)
            {
                trans = null;
                if (dbDataAdapter.SelectCommand != null)
                {
                    trans = dbDataAdapter.SelectCommand.Transaction;
                    return dbDataAdapter.SelectCommand.Connection;
                }
                if (dbDataAdapter.InsertCommand != null)
                {
                    trans = dbDataAdapter.InsertCommand.Transaction;
                    return dbDataAdapter.InsertCommand.Connection;
                }
                if (dbDataAdapter.UpdateCommand != null)
                {
                    trans = dbDataAdapter.UpdateCommand.Transaction;
                    return dbDataAdapter.UpdateCommand.Connection;
                }
                if (dbDataAdapter.DeleteCommand != null)
                {
                    trans = dbDataAdapter.DeleteCommand.Transaction;
                    return dbDataAdapter.DeleteCommand.Connection;
                }
                return null;
            }

            private int UpdateSqlServer(DataTable dt, string tableName)
            {
                int processedRecord = -1;
                IDbTransaction trans;
                DbConnection cn = (DbConnection)GetConnection(out trans);
                using (SqlBulkCopy bc = new SqlBulkCopy((SqlConnection)cn, 
                    SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.KeepIdentity, 
                    (SqlTransaction)trans))
                {
                    bc.BatchSize = dt.Rows.Count;
                    bc.DestinationTableName = tableName;
                    try
                    {
                        bc.WriteToServer(dt);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                return processedRecord;
            }*/

            #endregion

            #region IDbDataAdapter Members

            public IDbCommand DeleteCommand
            {
                get { return dbDataAdapter.DeleteCommand; }
                set { dbDataAdapter.DeleteCommand = value; }
            }

            public IDbCommand InsertCommand
            {
                get { return dbDataAdapter.InsertCommand; }
                set { dbDataAdapter.InsertCommand = value; }
            }

            public IDbCommand SelectCommand
            {
                get { return dbDataAdapter.SelectCommand; }
                set { dbDataAdapter.SelectCommand = value; }
            }

            public IDbCommand UpdateCommand
            {
                get { return dbDataAdapter.UpdateCommand; }
                set { dbDataAdapter.UpdateCommand = value; }
            }

            #endregion

            #region IDataAdapter Members

            public int Fill(DataSet dataSet)
            {
                return dbDataAdapter.Fill(dataSet);
            }

            public DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
            {
                return dbDataAdapter.FillSchema(dataSet, schemaType);
            }

            public IDataParameter[] GetFillParameters()
            {
                return dbDataAdapter.GetFillParameters();
            }

            public MissingMappingAction MissingMappingAction
            {
                get { return dbDataAdapter.MissingMappingAction; }
                set { dbDataAdapter.MissingMappingAction = value; }
            }

            public MissingSchemaAction MissingSchemaAction
            {
                get { return dbDataAdapter.MissingSchemaAction; }
                set { dbDataAdapter.MissingSchemaAction = value; }
            }

            public ITableMappingCollection TableMappings
            {
                get { return dbDataAdapter.TableMappings; }
            }

            /// <summary>
            /// Сохраняет изменения в базу данных
            /// </summary>
            /// <param name="dataSet"></param>
            /// <returns>Количество обработанных строк</returns>
            /// <remarks>
            /// Перед сохранением производится дополнительная обработка данных, 
            /// которая примерно составляет 25% от общего времени.
            /// Большую часть времени из этих 25% составляют удаленные 
            /// вызовы к схеме для получения свойств атрибутов.
            /// </remarks>
            public int Update(DataSet dataSet)
            {
                if (dataSet.Tables.Count > 1)
                    throw new Exception(String.Format(
                        "Класс {0} рассчитан на обновление только одной таблицы в датасете.", this.ToString()));


                Stopwatch sw = new Stopwatch();
                sw.Start();

                DataTable dt = dataSet.Tables[0];

                // Получаем список обязательных полей со значениями по умолчанию
                Dictionary<string, object> notNullAttributes = new Dictionary<string, object>();
                foreach (IDataAttribute attribute in entity.Attributes.Values)
                {
                    string attributeName = attribute.Name;
                    object attributeDefaultValue = attribute.DefaultValue;
                    if (dt.Columns.Contains(attributeName) && attributeDefaultValue != null && !attribute.IsNullable)
                    {
                        notNullAttributes.Add(attributeName, attributeDefaultValue);
                    }
                }

                 // Устанавливаем значения по умолчанию для обязательных полей
                 if (notNullAttributes.Count > 0)
                 {
                    foreach (DataRow row in dt.Rows)
                    {
                        if ((row.RowState == DataRowState.Added) || (row.RowState == DataRowState.Modified))
                        {
                            row.BeginEdit();
                            foreach (KeyValuePair<string, object> item in notNullAttributes)
                            {
                                if (row[item.Key] == DBNull.Value)
                                    row[item.Key] = item.Value;
                            }
                            row.EndEdit();
                        }
                    }
                 }

                 sw.Stop();              
                 Trace.WriteLine(String.Format("Время дополнительной обработки : {0} мс", sw.ElapsedMilliseconds));
                 sw.Reset();
                 sw.Start();

                int affectedRowsCount = -1;
                //if (!IsSqlServer())
                    affectedRowsCount = dbDataAdapter.Update(dataSet);
                //else
                //    affectedRowsCount = UpdateSqlServer(dt, entity.FullDBName);

                 sw.Stop();
                 Trace.WriteLine(String.Format("Время сохранения изменений в БД: {0} мс", sw.ElapsedMilliseconds));
                 Trace.WriteLine(String.Format("Количество обработанных строк  : {0}", affectedRowsCount));
                    
                 return affectedRowsCount;
            }

            #endregion
        }

        /// <summary>
        /// Устанавлвает у всех коммандов датаадаптера указанную транзакцию
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="tr">Транзакция</param>
        private void SetDataAdapterTransaction(IDbDataAdapter da, IDbTransaction tr)
        {
            if (da.DeleteCommand != null)
                da.DeleteCommand.Transaction = tr;
            if (da.InsertCommand != null)
                da.InsertCommand.Transaction = tr;
            if (da.SelectCommand != null)
                da.SelectCommand.Transaction = tr;
            if (da.UpdateCommand != null)
                da.UpdateCommand.Transaction = tr;
        }

        /// <summary>
        /// Инициализирует ДатаАдаптер для работы с нашей базой
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="obj">Описание структуры данных в базе</param>
        /// <param name="readOnly">Только для чтения. Из запросов удаляются ID для фактов</param>
        /// <param name="restrict">Ограничение</param>
        /// <param name="tableMappingName">Название таблицы в датасете, куда будет производиться выборка.
        /// Пустая строка - название по умолчанию.</param>
        /// <param name="removeID">Удалять ID из запросов</param>
        public void InitLocalDataAdapter(ref IDbDataAdapter da, IEntity obj, bool readOnly, string restrict,
            string tableMappingName, bool removeID)
        {
            da = null;
            da = new DbDataAdapterWrapper(this.DB.GetDataAdapter(), obj);

            if (tableMappingName != string.Empty)
            {
                da.TableMappings.Clear();
                da.TableMappings.Add(obj.FullDBName, tableMappingName);
            }


            // Создаем копию коллекции на сервере путем клонирования объекта
            IDataAttributeCollection attributes = obj.Attributes.Clone() as IDataAttributeCollection;
            // При клонировании оригинальный объект автоматически помечается как заблокированный. 
            // Поэтому для дальнейшей корректной работы необходимо разблокировать этот объект.
            obj.Attributes.Unlock();

            // Удаляем системный атрибут
            //bool removed = attributes.Remove("RowType");
            if (attributes.ContainsKey("RowType"))
                attributes.Remove("RowType");

            // Удаляем все ссылки на сопоставимые для классификаторов данных
            if (obj.ClassType == ClassTypes.clsDataClassifier)
            {
                foreach (IEntityAssociation association in obj.Associations.Values)
                {
                    if (association.AssociationClassType == AssociationClassTypes.Bridge /*||
                        association.AssociationClassType == AssociationClassTypes.Link*/)
                        //removed = 
                            attributes.Remove(association.RoleDataAttribute.Name);
                }
            }

            bool removed = false;
            if (removeID)
            {
                removed = attributes.Remove("ID");
            }

            da.SelectCommand = this.DB.InitSelectCommand(this.DB.Transaction, obj.FullDBName, attributes, 
                restrict, null, null);
            if (!readOnly)
            {
                da.DeleteCommand = this.DB.InitDeleteCommand(this.DB.Transaction, obj.FullDBName);

                // для СклСервера убираем айди из инсерта для фактов, так как при вставке явного айди генерится исключение
                // ваще надо подумать, нужен ли айди при инсерте в таблице фактов (вероятно нужен для детали...)
                if ((serverDBMSName == DBMSName.SQLServer) && (obj is IFactTable))
                    if (!removed)
                        removed = attributes.Remove("ID");
                da.InsertCommand = this.DB.InitInsertCommand(this.DB.Transaction, obj.FullDBName, attributes);

                // Апдейту ИД нафиг не упал
                if (!removed)
                    removed = attributes.Remove("ID");
                da.UpdateCommand = this.DB.InitUpdateCommand(this.DB.Transaction, obj.FullDBName, attributes);
            }

            // Освобождаем созданную копию на сервере
            attributes.Dispose();
        }

        /// <summary>
        /// Инициализирует ДатаАдаптер для работы с нашей базой
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="obj">Описание структуры данных в базе</param>
        /// <param name="readOnly">Только для чтения. Из запросов удаляются ID для фактов</param>
        /// <param name="restrict">Ограничение</param>
        /// <param name="tableMappingName">Название таблицы в датасете, куда будет производиться выборка.
        /// Пустая строка - название по умолчанию.</param>
        public void InitLocalDataAdapter(ref IDbDataAdapter da, IEntity obj, bool readOnly, string restrict,
            string tableMappingName)
        {
            InitLocalDataAdapter(ref da, obj, readOnly, restrict, tableMappingName, false);
        }

        /// <summary>
        /// Инициализирует ДатаАдаптер (только для чтения)
        /// </summary>
        /// <param name="da">Объект БД</param>
        /// <param name="fct">ДатаАдаптер</param>
        /// <param name="query">Запрос</param>
        public void InitLocalDataAdapter(Database db, ref IDbDataAdapter da, string query)
        {
            da = null;
            da = db.GetDataAdapter();
            da.SelectCommand = db.Connection.CreateCommand();
            da.SelectCommand.Transaction = db.Transaction;
            da.SelectCommand.CommandTimeout = constCommandTimeout;
            da.SelectCommand.CommandText = query;
        }

        #endregion Функции для работы с IDbDataAdapter


        #region Функции для работы с DataSet

        /// <summary>
        /// Сохраняет данные датасета в базу через инсерты
        /// </summary>
        /// <param name="db">БД</param>
        /// <param name="table">Таблица</param>
        /// <param name="obj">Наименование таблицы в базе</param>
        /// <returns>Строка ошибки</returns>
        /*protected string UpdateDataTableByInsert(DataTable table, string tableName)
        {
            DataRow[] rows = table.Select(string.Empty, string.Empty, DataViewRowState.Added);
            string fields = string.Empty;

            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i].ColumnName.ToUpper() != "ID")
                {
                    fields += table.Columns[i].ColumnName + ", ";
                }
            }

            if (fields != string.Empty)
            {
                fields = fields.Remove(fields.Length - 2);
            }
            else
            {
                return "Нет полей";
            }

            string query = string.Empty;

            int count = rows.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                try
                {
                    string values = string.Empty;
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (table.Columns[j].ColumnName.ToUpper() == "ID")
                            continue;

                        string str = Convert.ToString(table.Rows[i][j]);
                        if (str == string.Empty)
                        {
                            str = "null";
                        }
                        if (table.Rows[i][j].GetType() == typeof(string))
                        {
                            values += string.Format("'{0}', ", str.Replace("'", "''"));
                        }
                        else
                        {
                            values += str.Replace(',', '.') + ", ";
                        }
                    }
                    if (values != string.Empty)
                    {
                        values = values.Remove(values.Length - 2);
                    }
                    else
                    {
                        return "Нет значений, строка " + i.ToString();
                    }

                    query = string.Format("insert into {0} ({1}) values ({2})", tableName, fields, values);

                    this.DB.ExecQuery(query, QueryResultTypes.NonQuery);
                }
                catch (Exception ex)
                {
                    return ex.Message + ", строка " + i.ToString() + "   " + query;
                }
            }

            return string.Empty;
        }*/

        /// <summary>
        /// Инициализация датасета
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="ds">Датасет</param>
        /// <param name="obj">Описание структуры данных в базе</param>
        /// <param name="readOnly">Только для чтения</param>
        /// <param name="restrict">Ограничение</param>
        /// <param name="tableMappingName">Название таблицы в датасете, куда будет производиться выборка.
        /// Пустая строка - название по умолчанию.</param>
        /// <param name="removeID">Удалять ID из запросов</param>
        public void InitDataSet(ref IDbDataAdapter da, ref DataSet ds, IEntity obj, bool readOnly,
            string restrict, string tableMappingName, bool removeID)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string traceMessage = string.Format("Запрос данных {0} (ограничение \"{1}\")...", obj.FullDBName, restrict);
            WriteToTrace(traceMessage, TraceMessageKind.Information);

            InitLocalDataAdapter(ref da, obj, readOnly, restrict, tableMappingName, removeID);
            ClearDataSet(ref ds);
            da.Fill(ds);

            sw.Stop();
            traceMessage = string.Format("Запрос данных окончен ({0} строк {1} мс) ", GetTotalRecCountForDataSet(ds), sw.ElapsedMilliseconds);
            WriteToTrace(traceMessage, TraceMessageKind.Information);
        }

        /// <summary>
        /// Инициализация датасета
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="ds">Датасет</param>
        /// <param name="obj">Описание структуры данных в базе</param>
        /// <param name="readOnly">Только для чтения</param>
        /// <param name="restrict">Ограничение</param>
        /// <param name="tableMappingName">Название таблицы в датасете, куда будет производиться выборка.
        /// Пустая строка - название по умолчанию.</param>
        public void InitDataSet(ref IDbDataAdapter da, ref DataSet ds, IEntity obj, bool readOnly,
            string restrict, string tableMappingName)
        {
            InitDataSet(ref da, ref ds, obj, readOnly, restrict, tableMappingName, false);
        }

        /// <summary>
        /// Инициализация датасета
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="ds">Датасет</param>
        /// <param name="obj">Описание структуры данных в базе</param>
        /// <param name="restrict">Ограничение</param>
        public void InitDataSet(ref IDbDataAdapter da, ref DataSet ds, IEntity obj, string restrict)
        {
            InitDataSet(ref da, ref ds, obj, false, restrict, string.Empty);
        }

        /// <summary>
        /// Инициализация датасета таблицы фактов
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="ds">Датасет</param>
        /// <param name="obj">Описание структуры данных в базе</param>
        /// <param name="restrict">Ограничение</param>
        public void InitFactDataSet(ref IDbDataAdapter da, ref DataSet ds, IFactTable obj)
        {
            InitDataSet(ref da, ref ds, obj, false, "1 = 0", string.Empty, true);
        }

        /// <summary>
        /// Инициализация датасета
        /// </summary>
        /// <param name="db">Объект БД</param>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="ds">Датасет</param>
        /// <param name="restrict">Ограничение</param>
        public void InitDataSet(Database db, ref IDbDataAdapter da, ref DataSet ds, string query)
        {
            WriteToTrace(string.Format(
                "Запрос данных DB \"{0}\", запрос \"{1}\"...", db.Connection.ConnectionString, query), TraceMessageKind.Information);

            InitLocalDataAdapter(db, ref da, query);
            ClearDataSet(ref ds);
            da.Fill(ds);

            WriteToTrace(string.Format("Запрос данных окончен ({0} строк).", GetTotalRecCountForDataSet(ds)), TraceMessageKind.Information);
        }

        /// <summary>
        /// Функция инициализации датасета классификатора. 
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="ds">Датасет</param>
        /// <param name="cls">Описание классификатора данных</param>
        /// <param name="readOnly">Только для чтения</param>
        /// <param name="tableMappingName">Название таблицы в датасете, куда будет производиться выборка.
        /// Пустая строка - название по умолчанию.</param>
        public void InitClsDataSet(ref IDbDataAdapter da, ref DataSet ds, IClassifier cls, bool readOnly,
            string tableMappingName, int sourceID)
        {
            string traceMessage = string.Format("Запрос данных {0}...", cls.FullDBName);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            WriteToTrace(traceMessage, TraceMessageKind.Information);

            string whereConstraint = String.Empty;
            if (cls.IsDivided && cls.Levels.HierarchyType == HierarchyType.ParentChild)
            {
                whereConstraint = 
                    string.Format("ID >= 0 and id < 1000000000 and ((SOURCEID <> -PARENTID or PARENTID is null) or " +
                    "ID <> CUBEPARENTID or CUBEPARENTID is null) and SOURCEID = {0}", sourceID);
            }
            else
            {
                whereConstraint = string.Format("ID >= 0 and SOURCEID = {0}", sourceID);
            }
            // сделать параметром - строка - ограничение
            // if (cls.Attributes.Keys.Contains("PumpID"))
            //    whereConstraint += " and (PumpId <> -1)";
            InitLocalDataAdapter(ref da, cls, readOnly, whereConstraint, tableMappingName);

            ClearDataSet(ref ds);
            da.Fill(ds);

            sw.Stop();
            traceMessage = string.Format("Запрос данных окончен ({0} строк {1} мс)", GetTotalRecCountForDataSet(ds), sw.ElapsedMilliseconds);
            WriteToTrace(traceMessage, TraceMessageKind.Information);
        }

        /// <summary>
        /// Функция инициализации датасета классификатора. 
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="ds">Датасет</param>
        /// <param name="cls">Описание классификатора данных</param>
        /// <param name="readOnly">Только для чтения</param>
        /// <param name="tableMappingName">Название таблицы в датасете, куда будет производиться выборка.
        /// Пустая строка - название по умолчанию.</param>
        public void InitClsDataSet(ref IDbDataAdapter da, ref DataSet ds, IClassifier cls, bool readOnly,
            string tableMappingName)
        {
            InitClsDataSet(ref da, ref ds, cls, readOnly, tableMappingName, this.SourceID);
        }

        /// <summary>
        /// Функция инициализации датасета классификатора. 
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="ds">Датасет</param>
        /// <param name="cls">Описание классификатора данных</param>
        public void InitClsDataSet(ref IDbDataAdapter da, ref DataSet ds, IClassifier cls)
        {
            InitClsDataSet(ref da, ref ds, cls, false, string.Empty);
        }

        /// <summary>
        /// Очищает датасет
        /// </summary>
        /// <param name="ds">Датасет</param>
        public void ClearDataSet(ref DataSet ds)
        {
            if (ds != null)
            {
                ds.Clear();
                ds.Dispose();
            }

            ds = new DataSet();
        }

        /// <summary>
        /// Очищает датасет и инициализирует его
        /// </summary>
        /// <param name="ds">Датасет</param>
        public void ClearDataSet(IDbDataAdapter da, ref DataSet ds)
        {
            ClearDataSet(ref ds);
            da.Fill(ds);
        }

        /// <summary>
        /// Очищает датасет таблицы и инициализирует его
        /// </summary>
        /// <param name="dt">Таблица</param>
        public void ClearDataSet(IDbDataAdapter da, DataTable dt)
        {
            DataSet ds = dt.DataSet;
            ClearDataSet(da, ref ds);
        }

        /// <summary>
        /// Проверяет все таблицы датасета на наличие изменений
        /// </summary>
        /// <param name="ds">Датасет</param>
        /// <returns>true - есть изменения хотя бы в одной таблице</returns>
        private bool CheckTablesForUpdates(DataSet ds)
        {
            if (ds == null) return false;

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                // ***
                // 22.03.2007 Борисов: вот эта вот проверка приводит к созданию в памяти копии
                // всех изменений таблицы. Оно нам надо на огромных таблицах фактов?
                
                //if (ds.Tables[i].GetChanges() != null)
                //{
                //    return true;
                //}
                // ***

                foreach (DataRow row in ds.Tables[i].Rows)
                {
                    if ((row.RowState == DataRowState.Added) || (row.RowState == DataRowState.Deleted) ||
                        (row.RowState == DataRowState.Modified))
                        return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Возвращает общее количество строк во всех таблицах датасета
        /// </summary>
        /// <param name="ds">Датасет</param>
        /// <returns>Количество строк</returns>
        protected int GetTotalRecCountForDataSet(DataSet ds)
        {
            if (ds == null) return 0;

            int result = 0;

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                result += ds.Tables[i].Rows.Count;
            }

            return result;
        }

        /// <summary>
        /// Сохраняет данные датасета
        /// </summary>
        /// <param name="da">ДатаАдаптер</param>
        /// <param name="ds">ДатаСет</param>
        /// <param name="obj">Объект таблицы фактов или классификатора</param>
		public void UpdateDataSet(IDbDataAdapter da, DataSet ds, IEntity obj)
        {
            if (da != null && ds != null)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                if (!CheckTablesForUpdates(ds)) return;

                string fullCaption = "<Неизвестный объект>";
                if (obj != null)
                {
                	fullCaption = obj.FullCaption;
                }

                string traceMessage = string.Format("Сохранение данных {0}...", fullCaption);
                SetProgress(-1, -1, traceMessage, string.Empty, true);
                WriteToTrace(traceMessage, TraceMessageKind.Information);

                try
                {
                    da.Update(ds);
                }
                catch (Exception ex)
                {
                    WriteToTrace(ex.ToString(), TraceMessageKind.Error);
                    throw new Exception(string.Format(
                        "Ошибка при сохранении данных {0} ({1})", fullCaption, ex.Message), ex);
                }

                sw.Stop();
                traceMessage = string.Format("Данные {0} сохранены ({1} строк {2} мс)",
                    fullCaption, GetTotalRecCountForDataSet(ds), sw.ElapsedMilliseconds);
                SetProgress(-1, -1, traceMessage, string.Empty, true);
                WriteToTrace(traceMessage, TraceMessageKind.Information);
            }
        }

        #endregion Функции для работы с DataSet


        #region Функции для работы со строками (поиск, закачка)

        /// <summary>
        /// Возвращает массив значений полей из массива отображения значений на имена полей
        /// </summary>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <returns>Массив значений</returns>
        protected object[] GetFieldValuesFromValuesMapping(object[] valuesMapping)
        {
            object[] result = new object[valuesMapping.GetLength(0) / 2];

            int count = valuesMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                result[i / 2] = valuesMapping[i + 1];
            }

            return result;
        }

        /// <summary>
        /// Ищет в таблице записи с заданными значениями
        /// </summary>
        /// <param name="dt">Таблица</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <returns>Строка</returns>
        protected DataRow[] FindRows(DataTable dt, object[] valuesMapping)
        {
            try
            {
                // Формируем строку ограничения
                string constr = string.Empty;

                int count = valuesMapping.GetLength(0);
                for (int i = 0; i < count; i += 2)
                {
                    if (valuesMapping[i] == null) continue;

                    string value = Convert.ToString(valuesMapping[i + 1]);
                    if (value == string.Empty)
                    {
                        value = "null";
                    }
                    else
                    {
                        value = string.Format("'{0}'", value.Replace("'", "''"));
                    }
                    constr = string.Format("{0} and ({1} = {2})", constr, valuesMapping[i], value);
                }
                if (constr != string.Empty)
                {
                    constr = constr.Remove(0, 4);
                }

                return dt.Select(constr);
            }
            catch
            {
                return new DataRow[0];
            }
        }

        /// <summary>
        /// Ищет в таблице запись с заданными значениями
        /// </summary>
        /// <param name="dt">Таблица</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <returns>Строка</returns>
        protected DataRow FindRow(DataTable dt, object[] valuesMapping)
        {
            DataRow[] rows = FindRows(dt, valuesMapping);
            if (rows.GetLength(0) == 0)
            {
                return null;
            }
            else
            {
                return rows[0];
            }
        }

        /// <summary>
        /// Возвращает ИД строки, найденной по указанному условию
        /// </summary>
        /// <param name="dt">Таблица для поиска</param>
        /// <param name="selectStr">Строка выборки</param>
        /// <param name="defaultValue">Значение по умолчанию. Возвращается, если стока не найдена</param>
        /// <returns>ИД строки</returns>
        protected int FindRowID(DataTable dt, string selectStr, int defaultValue)
        {
            DataRow[] rows = dt.Select(selectStr);
            if (rows.GetLength(0) == 0) return defaultValue;

            return GetIntCellValue(rows[0], "ID", defaultValue);
        }

        /// <summary>
        /// Ищет в таблице запись с заданными значениями
        /// </summary>
        /// <param name="dt">Таблица</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <param name="defaultValue">Значение по умолчанию. Возвращается, если стока не найдена</param>
        /// <returns>ИД строки</returns>
        protected int FindRowID(DataTable dt, object[] valuesMapping, int defaultValue)
        {
            DataRow row = FindRow(dt, valuesMapping);

            return GetIntCellValue(row, "ID", defaultValue);
        }

        /// <summary>
        /// Возвращает ИД записи или значение по умолчанию, если запись = null
        /// </summary>
        /// <param name="row">Запись</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>ИД</returns>
        protected int GetRowID(DataRow row, int defaultValue)
        {
            return GetIntCellValue(row, "ID", defaultValue);
        }

        /// <summary>
        /// Возвращает значение указанного поля строки, найденной по указанному условию
        /// </summary>
        /// <param name="dt">Таблица для поиска</param>
        /// <param name="selectStr">Строка выборки</param>
        /// <param name="fieldName">Название поля</param>
        /// <param name="defaultValue">Значение по умолчанию. Возвращается, если стока не найдена</param>
        /// <returns>Значение поля</returns>
        protected object FindRowFieldValue(DataTable dt, string selectStr, string fieldName, object defaultValue)
        {
            try
            {
                DataRow[] rows = dt.Select(selectStr);

                if (rows.GetLength(0) == 0 || !dt.Columns.Contains(fieldName))
                {
                    return defaultValue;
                }

                return rows[0][fieldName];
            }
            catch { }

            return defaultValue;
        }

        /// <summary>
        /// Ищет в таблице запись с заданными значениями. Значения из массива берутся начиная с указанного индекса
        /// </summary>
        /// <param name="dt">Таблица</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <param name="index">Индекс элемента, начиная с которого будет формироваться строка запроса</param>
        /// <returns>Строка</returns>
        protected DataRow FindRowFromIndex(DataTable dt, int index, object[] valuesMapping)
        {
            try
            {
                if (index > valuesMapping.GetLength(0)) return null;

                object[] obj = new object[0];
                Array.Resize(ref obj, valuesMapping.GetLength(0) - index);
                Array.Copy(valuesMapping, index, obj, 0, obj.GetLength(0));

                return FindRow(dt, obj);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Возвращает следующее значение генератора объекта
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <returns>Значение генератора</returns>
        [Obsolete("Медленный метод, требует обращения к прокси удаленного обеъкта")]
        public int GetGeneratorNextValue(IEntity obj)
        {
            return this.DB.GetGenerator(obj.GeneratorName);
        }

        public int GetGeneratorNextValue(string generatorName)
        {
            return this.DB.GetGenerator(generatorName);
        }

        /// <summary>
        /// Закачивает строку (даже если она есть в закачанных данных) с генерацией ИД
        /// </summary>
        /// <param name="dt">Таблица</param>
        /// <param name="obj">Объект классификатора/таблицы фактов</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <returns>ИД записи классификатора</returns>
		protected int PumpRow(DataTable dt, IEntity obj, object[] valuesMapping)
        {
            DataRow row = PumpRow(obj, dt, valuesMapping, true);

            if (row == null) return -1;

            return Convert.ToInt32(row["ID"]);
        }

        /// <summary>
        /// Закачивает строку (даже если она есть в закачанных данных) без генерации ИД
        /// </summary>
        /// <param name="dt">Таблица</param>
        /// <param name="obj">Объект классификатора/таблицы фактов</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <returns>ИД записи классификатора</returns>
        protected DataRow PumpRow(DataTable dt, object[] valuesMapping)
        {
            return PumpRow(null, dt, valuesMapping, false);
        }

        /// <summary>
        /// Закачивает строку (даже если она есть в закачанных данных)
        /// </summary>
        /// <param name="dt">Таблица</param>
        /// <param name="obj">Объект классификатора/таблицы фактов</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <returns>ИД записи классификатора</returns>
		protected DataRow PumpRow(IEntity obj, DataTable dt, object[] valuesMapping, bool generateID)
        {
            if (valuesMapping == null) return null;

            DataRow row = dt.NewRow();

            if (obj != null && generateID)
            {
                row["ID"] = GetGeneratorNextValue(obj);
            }

            if (dt.Columns.Contains("SOURCEID")) 
                row["SOURCEID"] = this.SourceID;

            if (dt.Columns.Contains("PUMPID")) 
                row["PUMPID"] = this.PumpID;

            if (dt.Columns.Contains("TASKID")) 
                row["TASKID"] = -1;

            CopyValuesToRow(row, valuesMapping);

            dt.Rows.Add(row);

            return row;
        }

        /// <summary>
        /// Закачивает строку (только если ее нет в закачанных данных) с генерацией ИД
        /// </summary>
        /// <param name="ds">Таблица классификатора</param>
        /// <param name="cls">Объект классификатора</param>
        /// <param name="isAdded">true - запись была добавлена</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <param name="valuesMappingForSearch">Список пар поле - значение для поиска записи. Если null, 
        /// берется valuesMapping</param>
        /// <returns>ИД записи классификатора</returns>
		protected DataRow PumpOriginalRow(DataTable dt, IEntity obj, object[] valuesMapping,
            object[] valuesMappingForSearch)
        {
            if (valuesMapping == null)
            {
                return null;
            }

            DataRow row = null;

            // Ищем строку
            if (valuesMappingForSearch != null)
            {
                row = FindRow(dt, valuesMappingForSearch);
            }
            else
            {
                row = FindRow(dt, valuesMapping);
            }

            if (row == null)
            {
                row = PumpRow(obj, dt, valuesMapping, true);
            }

            return row;
        }

        /// <summary>
        /// Закачивает строку (только если ее нет в закачанных данных)
        /// </summary>
        /// <param name="ds">Таблица классификатора</param>
        /// <param name="cls">Объект классификатора</param>
        /// <param name="isAdded">true - запись была добавлена</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <param name="valuesMappingForSearch">Список пар поле - значение для поиска записи. Если null, 
        /// берется valuesMapping</param>
        /// <returns>ИД записи классификатора</returns>
		protected int PumpOriginalRow(IEntity obj, DataTable dt, object[] valuesMapping,
            object[] valuesMappingForSearch)
        {
            return Convert.ToInt32(PumpOriginalRow(dt, obj, valuesMapping, valuesMappingForSearch)["ID"]);
        }

        /// <summary>
        /// Закачивает строку (только если ее нет в закачанных данных)
        /// </summary>
        /// <param name="ds">Таблица классификатора</param>
        /// <param name="cls">Объект классификатора</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <returns>ИД записи классификатора</returns>
		protected int PumpOriginalRow(DataSet ds, IEntity obj, object[] valuesMapping)
        {
            return PumpOriginalRow(obj, ds.Tables[0], valuesMapping, null);
        }

        /// <summary>
        /// Закачивает строку (только если ее нет в закачанных данных)
        /// </summary>
        /// <param name="ds">Таблица классификатора</param>
        /// <param name="cls">Объект классификатора</param>
        /// <param name="valuesMapping">Список пар поле - значение</param>
        /// <returns>ИД записи классификатора</returns>
		protected int PumpOriginalRow(DataSet ds, IEntity obj, object[] valuesMapping, 
            object[] valuesMappingForSearch)
        {
            return PumpOriginalRow(obj, ds.Tables[0], valuesMapping, valuesMappingForSearch);
        }

        /// <summary>
        /// Тип значения поля из списка соответствия полей
        /// </summary>
        protected enum MappedFieldKind
        {
            /// <summary>
            /// Название поля
            /// </summary>
            FieldName = 0,

            /// <summary>
            /// Константа 
            /// </summary>
            Constant = 1,

            /// <summary>
            /// Сумма значений полей
            /// </summary>
            FieldsSum = 2,

            /// <summary>
            /// Если значение первого поля null, то пишем константу после ";" 
            /// </summary>
            ConstForNull = 3
        }

        /// <summary>
        /// Определяет тип значения поля из списка соответствия полей (название поля, константа или сумма полей)
        /// </summary>
        /// <param name="mappedField">Поле
        /// Поле имеет следующий формат: 
        /// "somestring" - название поля в исходной базе;
        /// "константа в угловых скобках" - какая-то константа, воспринимается как значение поля классификатора данных;
        /// "somestring+somestring2" - поле somestring + поле somestring2;
        /// "somestring;somestring2" - если значение поля somestring = null, то константа somestring2</param>
        /// <param name="fieldsList">Список значений в соответствии с типом поля</param>
        /// <returns>Тип поля</returns>
        protected MappedFieldKind ParseFieldsMapping(string mappedField, out string[] fieldsList)
        {
            fieldsList = new string[5];
            // В угловых скобках находятся константы
            if (mappedField.StartsWith("<") && mappedField.EndsWith(">"))
            {
                fieldsList[0] = mappedField.Trim('<', '>');
                return MappedFieldKind.Constant;
            }
            else
            {
                // Со знаком "+" перечислены поля, значения которых нужно сложить
                if (mappedField.Contains("+"))
                {
                    fieldsList = mappedField.Split('+');
                    return MappedFieldKind.FieldsSum;
                }
                else
                {
                    // Если значение первого поля null, то закачиваем константу после знака ";"
                    if (mappedField.Contains(";"))
                    {
                        fieldsList = mappedField.Split(';');
                        return MappedFieldKind.ConstForNull;
                    }
                    else
                    {
                        fieldsList[0] = mappedField;
                        return MappedFieldKind.FieldName;
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает поле кода классификатора
        /// </summary>
        /// <param name="cls">Классификатор</param>
        /// <returns>Поле кода</returns>
        private const string CODE = "CODE";
        private const string CODE_STR = "CODESTR";
        private const string LONG_CODE = "LONGCODE";
        protected string GetClsCodeField(IClassifier cls)
        {
            foreach (KeyValuePair<string, IDataAttribute> attr in cls.Attributes)
            {
                if (attr.Value.Name.ToUpper() == CODE)
                    return CODE;
                else if (attr.Value.Name.ToUpper() == CODE_STR)
                    return CODE_STR;
                else if (attr.Value.Name.ToUpper() == LONG_CODE)
                    return LONG_CODE;
            }
            return string.Empty;
        }

        /// <summary>
        /// Копирует строку в указанную таблицу
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="dt">Таблица, куда копировать строку</param>
        protected void CopyRowToTable(DataRow row, DataTable dt)
        {
            if (row == null) return;

            DataRow newRow = dt.NewRow();
            CopyRowToRow(row, newRow);
            dt.Rows.Add(newRow);
        }

        /// <summary>
        /// Копирует строку в указанную таблицу
        /// </summary>
        /// <param name="sourceRow">Исходная строка</param>
        /// <param name="destRow">Строка, куда копировать</param>
        protected void CopyRowToRow(DataRow sourceRow, DataRow destRow)
        {
            if (sourceRow == null || destRow == null) return;

            for (int i = 0; i < destRow.Table.Columns.Count; i++)
            {
                if (sourceRow.Table.Columns.Contains(destRow.Table.Columns[i].ColumnName))
                {
                    destRow[i] = sourceRow[destRow.Table.Columns[i].ColumnName];
                }
            }
        }

        protected void CopyRowToRowNoId(DataRow sourceRow, DataRow destRow)
        {
            if (sourceRow == null || destRow == null)
                return;
            for (int i = 0; i < destRow.Table.Columns.Count; i++)
            {
                string columnName = destRow.Table.Columns[i].ColumnName;
                if (columnName.ToUpper() == "ID")
                    continue;
                if (sourceRow.Table.Columns.Contains(columnName))
                    destRow[i] = sourceRow[columnName];
            }
        }

        /// <summary>
        /// Устанавливает значения полей строки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="valuesMapping">Значения полей: пары поле-значение</param>
        protected void CopyValuesToRow(DataRow row, object[] valuesMapping)
        {
            DataTable dt = row.Table;

            for (int i = 0; i < valuesMapping.GetLength(0) - 1; i += 2)
            {
                if (valuesMapping[i] == null)
                {
                    continue;
                }

                string fieldName = Convert.ToString(valuesMapping[i]);

                if (dt.Columns.Contains(fieldName))
                {
                    if (valuesMapping[i + 1] != null)
                    {
                        row[fieldName] = valuesMapping[i + 1];
                    }
                    else
                    {
                        row[fieldName] = DBNull.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Возращает массив пар имя_поля-значение_поля для данной строки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="fieldNames">Массив имен полей</param>
        /// <returns>Массив пар имя_поля-значение_поля</returns>
        protected object[] GetFieldValuesMappingFromRow(DataRow row, string[] fieldNames)
        {
            if (row == null || fieldNames == null) return null;

            object[] result = new object[fieldNames.GetLength(0) * 2];

            int count = fieldNames.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                result[i * 2] = fieldNames[i];
                if (row.Table.Columns.Contains(fieldNames[i]))
                {
                    result[i * 2 + 1] = row[fieldNames[i]];
                }
            }

            return result;
        }

        /// <summary>
        /// Сравнивает две строки по указанным полям.
        /// </summary>
        /// <param name="row1">Первая строка</param>
        /// <param name="row2">Вторая строка</param>
        /// <param name="fieldNames">Массив имен полей.</param>
        /// <returns>true - значения строк совпадают</returns>
        protected bool CompareRows(DataRow row1, DataRow row2, string[] fieldNames)
        {
            if (row1 == null || row2 == null || fieldNames == null || fieldNames.GetLength(0) == 0) return false;

            bool fieldsPresented = false;

            int count = fieldNames.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (row1.Table.Columns.Contains(fieldNames[i]) && row2.Table.Columns.Contains(fieldNames[i]))
                {
                    fieldsPresented = true;
                    if (row1[fieldNames[i]] != row2[fieldNames[i]]) return false;
                }
            }

            if (!fieldsPresented) return false;

            return true;
        }

        #endregion Функции для работы со строками (поиск, закачка)


        #region Функции для работы с кэшами

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyFields">Поля со значениями ключа</param>
        /// <param name="valueField">Поле со значениями value кэша</param>
        protected void FillRowsCache(ref Dictionary<string, int> cache, DataTable dt, string[] keyFields, 
            string valueField)
        {
            if (dt == null || keyFields.GetLength(0) == 0)
                return;

            // Инициализируем размер кэша здесь, в соответствии 
            // с фактическим размером таблицы
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<string, int>(dt.Rows.Count);

            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState != DataRowState.Deleted)
                {
                    string key = GetComplexCacheKey(row, keyFields);
                    if (!cache.ContainsKey(key))
                    {
                        cache.Add(key, Convert.ToInt32(row[valueField]));
                    }
                }
            }
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache"> кэш </param>
        /// <param name="dt"> таблица </param>
        /// <param name="keyFields"> список ключевых полей </param>
        /// <param name="delimeter"> разделитель для ключа кэша </param>
        /// <param name="valueField"> Поле со значениями value кэша </param>
        protected void FillRowsCache(ref Dictionary<string, int> cache, DataTable dt, string[] keyFields, string delimeter, string valueField)
        {
            if (dt == null)
                return;
            // Инициализируем размер кэша здесь, в соответствии 
            // с фактическим размером таблицы
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<string, int>(dt.Rows.Count);
            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState == DataRowState.Deleted)
                    continue;
                string key = GetComplexCacheKey(row, keyFields, delimeter);
                if (!cache.ContainsKey(key))
                    cache.Add(key, Convert.ToInt32(row[valueField]));
            }
        }

        protected void FillRowsCache(ref Dictionary<string, string> cache, DataTable dt, string[] keyFields, string delimeter, string valueField)
        {
            if (dt == null)
                return;
            // Инициализируем размер кэша здесь, в соответствии 
            // с фактическим размером таблицы
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<string, string>(dt.Rows.Count);
            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState == DataRowState.Deleted)
                    continue;
                string key = GetComplexCacheKey(row, keyFields, delimeter);
                if (!cache.ContainsKey(key))
                    cache.Add(key, row[valueField].ToString());
            }
        }

        protected void FillRowsCache(ref Dictionary<string, DataRow> cache, DataTable dt, string[] keyFields, string delimeter)
        {
            if (dt == null)
                return;
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<string, DataRow>(dt.Rows.Count);
            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState == DataRowState.Deleted)
                    continue;
                string key = GetComplexCacheKey(row, keyFields, delimeter);
                if (!cache.ContainsKey(key))
                    cache.Add(key, row);
            }
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache"> кэш </param>
        /// <param name="dt"> таблица </param>
        /// <param name="keyFields"> список ключевых полей </param>
        /// <param name="delimeter"> разделитель для ключа кэша </param>
        /// <param name="valueField"> Поле со значениями value кэша </param>
        protected void FillRowsCache(ref Dictionary<string, int> cache, DataTable dt, string[] keyFields, string delimeter, string valueField, string emptyChar)
        {
            if (dt == null)
                return;
            // Инициализируем размер кэша здесь, в соответствии 
            // с фактическим размером таблицы
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<string, int>(dt.Rows.Count);
            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState == DataRowState.Deleted)
                    continue;
                string key = GetComplexCacheKey(row, keyFields, delimeter, emptyChar);
                if (!cache.ContainsKey(key))
                    cache.Add(key, Convert.ToInt32(row[valueField]));
            }
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        /// <param name="valueField">Поле со значениями value кэша</param>
        protected void FillRowsCache(ref Dictionary<string, int> cache, DataTable dt, string keyField, 
            string valueField)
        {
            FillRowsCache(ref cache, dt, new string[] { keyField }, valueField);
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        protected void FillRowsCache(ref Dictionary<string, int> cache, DataTable dt, string keyField)
        {
            FillRowsCache(ref cache, dt, keyField, "ID");
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyFields">Поле со значениями ключа</param>
        /// <param name="valueField">Поле со значениями value кэша</param>
        protected void FillRowsCache(ref Dictionary<string, string> cache, DataTable dt, string[] keyFields, 
            string valueField)
        {
            if (dt == null) return;

            // Инициализируем размер кэша здесь, в соответствии 
            // с фактическим размером таблицы
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<string, string>(dt.Rows.Count);

            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState != DataRowState.Deleted)
                {
                    string key = GetComplexCacheKey(row, keyFields);
                    if (!cache.ContainsKey(key))
                    {
                        cache.Add(key, Convert.ToString(row[valueField]));
                    }
                }
            }
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        /// <param name="valueField">Поле со значениями value кэша</param>
        protected void FillRowsCache(ref Dictionary<string, string> cache, DataTable dt, string keyField,
            string valueField)
        {
            FillRowsCache(ref cache, dt, new string[] { keyField }, valueField);
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="codesMapping">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        protected void FillRowsCache(Dictionary<string, ClsRowData> codesMapping, DataTable dt, string keyField,
            string field1Field, string Field1DefValue, string field2Field, string field2DefValue, 
            string field3Field, string field3DefValue, IClassifier cls)
        {
            if (dt == null || codesMapping == null)
            {
                return;
            }

            codesMapping.Clear();
            string codeField = GetClsCodeField(cls);

            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].RowState != DataRowState.Deleted && !dt.Rows[i].IsNull(keyField))
                    {
                        string key = Convert.ToString(dt.Rows[i][keyField]);
                        if (!codesMapping.ContainsKey(key))
                        {
                            codesMapping.Add(key, new ClsRowData(
                                Convert.ToInt32(dt.Rows[i]["ID"]),
                                GetStringCellValue(dt.Rows[i], codeField, string.Empty),
                                GetStringCellValue(dt.Rows[i], "NAME", string.Empty),
                                GetStringCellValue(dt.Rows[i], field1Field, Field1DefValue),
                                GetStringCellValue(dt.Rows[i], field2Field, field2DefValue),
                                GetStringCellValue(dt.Rows[i], field3Field, field3DefValue)));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="codesMapping">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        protected void FillRowsCache(Dictionary<string, ClsRowData> codesMapping, DataTable dt, string keyField,
            string field1Field, string field2Field, string field3Field, IClassifier cls)
        {
            FillRowsCache(codesMapping, dt, keyField, field1Field, string.Empty, field2Field, string.Empty,
                field3Field, string.Empty, cls);
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="codesMapping">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        protected void FillRowsCache(Dictionary<string, ClsRowData> codesMapping, DataTable dt, string keyField, 
            IClassifier cls)
        {
            FillRowsCache(codesMapping, dt, keyField, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, cls);
        }

        /// <summary>
        /// Заполняет список кэша записей. Ключ кэша состоит из значений нескольких полей
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поля со значениями ключа</param>
        protected void FillRowsCache(ref Dictionary<string, DataRow> cache, DataTable dt, string[] keyFields)
        {
            if (dt == null || keyFields.GetLength(0) == 0) return;

            // Инициализируем размер кэша здесь, в соответствии 
            // с фактическим размером таблицы
            if (cache != null) cache.Clear();
            cache = new Dictionary<string, DataRow>(dt.Rows.Count);

            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState == DataRowState.Deleted) continue;

                string key = GetComplexCacheKey(row, keyFields);
                if (!cache.ContainsKey(key))
                {
                    cache.Add(key, row);
                }
            }
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        protected void FillRowsCache(ref Dictionary<int, DataRow> cache, DataTable dt, string keyField)
        {
            if (dt == null) return;

            // Инициализируем размер кэша здесь, в соответствии 
            // с фактическим размером таблицы
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<int, DataRow>(dt.Rows.Count);

            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState != DataRowState.Deleted && !row.IsNull(keyField))
                {
                    int key = Convert.ToInt32(row[keyField]);
                    if (!cache.ContainsKey(key))
                    {
                        cache.Add(key, row);
                    }
                }
            }
        }


        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        /// <param name="valueField">Поле со значениями value кэша</param>
        protected void FillRowsCache(ref Dictionary<int, int> cache, DataTable dt, string keyField, string valueField)
        {
            if (dt == null) return;

            // Инициализируем размер кэша здесь, в соответствии 
            // с фактическим размером таблицы
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<int, int>(dt.Rows.Count);

            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState != DataRowState.Deleted && !row.IsNull(keyField))
                {
                    int key = Convert.ToInt32(row[keyField]);
                    if (!cache.ContainsKey(key))
                    {
                        cache.Add(key, Convert.ToInt32(row[valueField]));
                    }
                }
            }
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        protected void FillRowsCache(ref Dictionary<int, int> cache, DataTable dt, string keyField)
        {
            FillRowsCache(ref cache, dt, keyField, "ID");
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        /// <param name="valueField">Поле со значениями value кэша</param>
        protected void FillRowsCache(ref Dictionary<int, string> cache, DataTable dt, string keyField, 
            string[] valueField)
        {
            if (dt == null) return;

            // Инициализируем размер кэша здесь, в соответствии 
            // с фактическим размером таблицы
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<int, string>(dt.Rows.Count);

            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState != DataRowState.Deleted && !row.IsNull(keyField))
                {
                    int key = Convert.ToInt32(row[keyField]);
                    if (!cache.ContainsKey(key))
                    {
                        cache.Add(key, Convert.ToString(GetComplexCacheKey(row, valueField)));
                    }
                }
            }
        }

        /// <summary>
        /// Заполняет список кэша записей 
        /// </summary>
        /// <param name="cache">Список</param>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="keyField">Поле со значениями ключа</param>
        /// <param name="valueField">Поле со значениями value кэша</param>
        protected void FillRowsCache(ref Dictionary<int, string> cache, DataTable dt, string keyField,
            string valueField)
        {
            FillRowsCache(ref cache, dt, keyField, new string[] { valueField });
        }

        #endregion Функции для работы с кэшами


        #region Функции для работы с кэшированными строками

        /// <summary>
        /// Ищет строку классификатора в списке кодов.
        /// </summary>
        /// <param name="cache">Список кодов</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="key">Значение классификатора</param>
        /// <param name="defaultValue">Значение, возвращаемое при неудачном поиске</param>
        /// <returns>ИД записи</returns>
        protected int FindCachedRow(Dictionary<string, int> cache, string key, int defaultValue)
        {
            if (cache == null) return defaultValue;

            if (!cache.ContainsKey(key))
            {
                return defaultValue;
            }
            else
            {
                return cache[key];
            }
        }

        /// <summary>
        /// Ищет строку классификатора в списке кодов.
        /// </summary>
        /// <param name="cache">Список кодов</param>
        /// <param name="key">Значение классификатора</param>
        /// <param name="defaultValue">Значение, возвращаемое при неудачном поиске</param>
        /// <returns>ИД записи</returns>
        protected DataRow FindCachedRow(Dictionary<int, DataRow> cache, int key)
        {
            if (cache == null) return null;

            if (!cache.ContainsKey(key))
            {
                return null;
            }
            else
            {
                return cache[key];
            }
        }

        /// <summary>
        /// Возвращает ключ для кэша, состоящий из значений нескольких полей
        /// </summary>
        /// <param name="keyValues">Массив значений для формирования составного ключа</param>
        /// <returns>Ключ</returns>
        protected string GetComplexCacheKey(string[] keyValues)
        {
            string result = string.Empty;

            if (keyValues.GetLength(0) == 1)
            {
                result = keyValues[0];
            }
            else
            {
                int count = keyValues.GetLength(0);
                for (int j = 0; j < count; j++)
                {
                    result += keyValues[j].PadLeft(constTotalWidthForComplexKey, '0');
                }
            }

            return result;
        }

        /// <summary>
        /// Возвращает ключ для кэша, состоящий из значений нескольких полей
        /// </summary>
        /// <param name="keyValues">Массив значений для формирования составного ключа</param>
        /// <returns>Ключ</returns>
        protected string GetComplexCacheKey(object[] keyValues, int totalWidthForComplexKey)
        {
            string result = string.Empty;

            if (keyValues.GetLength(0) == 1)
            {
                if (keyValues[0] != null) result = Convert.ToString(keyValues[0]);
            }
            else
            {
                for (int j = 0; j < keyValues.GetLength(0); j++)
                {
                    if (keyValues[j] != null)
                        result += Convert.ToString(keyValues[j]).PadLeft(totalWidthForComplexKey, '0');
                }
            }

            return result;
        }

        /// <summary>
        /// Возвращает ключ для кэша, состоящий из значений нескольких полей
        /// </summary>
        /// <param name="keyValues">Массив значений для формирования составного ключа</param>
        /// <returns>Ключ</returns>
        protected string GetComplexCacheKey(object[] keyValues)
        {
            return GetComplexCacheKey(keyValues, constTotalWidthForComplexKey);
        }

        /// <summary>
        /// Формирует значение ключа для кэша
        /// </summary>
        /// <param name="row">Строка с записями</param>
        /// <param name="keyFields">Массив полей со значениями ключа</param>
        protected string GetComplexCacheKey(DataRow row, string[] keyFields)
        {
            if (row == null)
                return string.Empty;

            string key = string.Empty;

            if (keyFields.GetLength(0) == 1)
            {
                key = Convert.ToString(row[keyFields[0]]);
            }
            else
            {
                int count = keyFields.GetLength(0);
                for (int i = 0; i < count; i++)
                {
                    key += Convert.ToString(row[keyFields[i]]).PadLeft(constTotalWidthForComplexKey, '0');
                }
            }

            return key;
        }

        /// <summary>
        /// Формирует значение ключа для кэша
        /// </summary>
        /// <param name="row">Строка с записями</param>
        /// <param name="keyFields">Массив полей со значениями ключа</param>
        protected string GetComplexCacheKey(DataRow row, string[] keyFields, string delimeter)
        {
            if (row == null)
                return string.Empty;
            string key = string.Empty;
            foreach (string keyField in keyFields)
                key += Convert.ToString(row[keyField]) + delimeter;
            if (delimeter != string.Empty)
                key = key.Remove(key.Length - 1);
            return key;
        }

        /// <summary>
        /// Формирует значение ключа для кэша
        /// </summary>
        /// <param name="row">Строка с записями</param>
        /// <param name="keyFields">Массив полей со значениями ключа</param>
        protected string GetComplexCacheKey(DataRow row, string[] keyFields, string delimeter, string emptyChar)
        {
            if (row == null)
                return string.Empty;
            string key = string.Empty;
            foreach (string keyField in keyFields)
                key += Convert.ToString(row[keyField]).Replace(emptyChar, "") + delimeter;
            if (delimeter != string.Empty)
                key = key.Remove(key.Length - 1);
            return key;
        }

        /// <summary>
        /// Ищет строку классификатора в кэше с составным ключом
        /// </summary>
        /// <param name="cache">Список кодов</param>
        /// <param name="keyValues">Массив значений для формирования составного ключа</param>
        /// <param name="defaultValue">Значение, возвращаемое при неудачном поиске</param>
        /// <returns>ИД записи</returns>
        protected DataRow FindCachedRow(Dictionary<string, DataRow> cache, string[] keyValues)
        {
            if (cache == null) return null;

            string key = GetComplexCacheKey(keyValues);

            if (!cache.ContainsKey(key))
            {
                return null;
            }
            else
            {
                return cache[key];
            }
        }

        /// <summary>
        /// Ищет ИД строки классификатора в списке.
        /// </summary>
        /// <param name="cache">Список кодов</param>
        /// <param name="key">Значение классификатора</param>
        /// <param name="defaultValue">Значение, возвращаемое при неудачном поиске</param>
        /// <returns>ИД записи</returns>
        protected int FindCachedRowID(Dictionary<int, DataRow> cache, int key, int defaultValue)
        {
            DataRow row = FindCachedRow(cache, key);

            if (row != null)
            {
                return Convert.ToInt32(row["ID"]);
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Ищет ИД строки классификатора в кэше с составным ключом
        /// </summary>
        /// <param name="cache">Список кодов</param>
        /// <param name="row">Строка</param>
        /// <param name="keyValues">Массив значений для формирования составного ключа</param>
        /// <param name="defaultValue">Значение, возвращаемое при неудачном поиске</param>
        /// <returns>ИД записи</returns>
        protected int FindCachedRowID(Dictionary<string, DataRow> cache, string[] keyValues, 
            int defaultValue)
        {
            DataRow cachedRow = FindCachedRow(cache, keyValues);

            if (cachedRow != null)
            {
                return Convert.ToInt32(cachedRow["ID"]);
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Ищет значение ячейки строки классификатора в списке.
        /// </summary>
        /// <param name="cache">Список кодов</param>
        /// <param name="key">Значение классификатора</param>
        /// <param name="defaultValue">Значение, возвращаемое при неудачном поиске</param>
        /// <returns>ИД записи</returns>
        protected object FindCachedRowField(Dictionary<int, DataRow> cache, int key, string fieldName, object defaultValue)
        {
            DataRow row = FindCachedRow(cache, key);

            if (row != null)
            {
                return row[fieldName];
            }
            else
            {
                return defaultValue;
            }
        }

        /*
        /// <summary>
        /// Ищет строку классификатора в списке кодов.
        /// </summary>
        /// <param name="codesMapping">Список кодов</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="key">Значение классификатора</param>
        /// <param name="defaultValue">Значение, возвращаемое при неудачном поиске</param>
        /// <returns>ИД записи</returns>
        protected ClsRowData FindCachedRow(Dictionary<string, ClsRowData> codesMapping, string key, int defaultValue)
        {
            if (!codesMapping.ContainsKey(key))
            {
                return new ClsRowData(defaultValue, "0", constDefaultClsName, string.Empty, string.Empty, string.Empty);
            }
            else
            {
                return codesMapping[key];
            }
        }*/

        /// <summary>
        /// Ищет строку классификатора в списке кодов.
        /// </summary>
        /// <param name="cache">Список кодов</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="key">Значение классификатора</param>
        /// <param name="defaultValue">Значение, возвращаемое при неудачном поиске</param>
        /// <returns>ИД записи</returns>
        protected int FindCachedRow(Dictionary<int, int> cache, int key, int defaultValue)
        {
            if (!cache.ContainsKey(key))
            {
                return defaultValue;
            }
            else
            {
                return cache[key];
            }
        }

        /// <summary>
        /// Ищет строку классификатора в списке кодов.
        /// </summary>
        /// <param name="cache">Список кодов</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="key">Значение классификатора</param>
        /// <param name="defaultValue">Значение, возвращаемое при неудачном поиске</param>
        /// <returns>ИД записи</returns>
        protected string FindCachedRow(Dictionary<int, string> cache, int key, string defaultValue)
        {
            if (cache == null) return null;

            if (!cache.ContainsKey(key))
            {
                return defaultValue;
            }
            else
            {
                return cache[key];
            }
        }

        /// <summary>
        /// Ищет строку классификатора в списке кодов.
        /// </summary>
        /// <param name="cache">Список кодов</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="key">Значение классификатора</param>
        /// <param name="defaultValue">Значение, возвращаемое при неудачном поиске</param>
        /// <returns>ИД записи</returns>
        protected string FindCachedRow(Dictionary<string, string> cache, string key, string defaultValue)
        {
            if (cache == null) return null;

            if (!cache.ContainsKey(key))
            {
                return defaultValue;
            }
            else
            {
                return cache[key];
            }
        }

        #region Dictionary<string, int>
        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <returns>ИД записи</returns>
		protected int PumpCachedRow(Dictionary<string, int> cache, DataTable dt, IEntity obj, string key,
            object[] fieldsMapping)
        {
            int id = -1;

            if (cache == null)
            {
                return PumpOriginalRow(obj, dt, fieldsMapping, null);
            }
            else
            {
                if (!cache.ContainsKey(key))
                {
                    id = PumpRow(dt, obj, fieldsMapping);
                    cache.Add(key, id);
                }
                else
                {
                    id = cache[key];
                }
            }

            return id;
        }

        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// Ключ кэша записывается в указанное поле. Если fieldName является числовым полем, то 
        /// лидирующие нули ключа игнорируются.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша. Служит значением для поля fieldName</param>
        /// <param name="keyField">Имя поля ключа</param>
        /// <param name="valueField">Имя поля значения</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <returns>Значение записи кэша</returns>
		protected int PumpCachedRow(Dictionary<string, int> cache, DataTable dt, IEntity obj, string key,
            string keyField, string valueField, object[] fieldsMapping)
        {
            int value = -1;

            if (cache == null)
            {
                return PumpOriginalRow(obj, dt, fieldsMapping, null);
            }
            else
            {
                string tmpKey = key;
                DataColumn column = dt.Columns[keyField];
                if (column.DataType != typeof(string)) tmpKey = tmpKey.TrimStart('0').PadLeft(1, '0');

                if (!cache.ContainsKey(tmpKey))
                {
                    DataRow row = PumpRow(obj, dt, (object[])CommonRoutines.ConcatArrays(
                        new object[] { keyField, tmpKey }, fieldsMapping), true);

                    value = Convert.ToInt32(row[valueField]);
                    cache.Add(tmpKey, value);
                }
                else
                {
                    value = cache[tmpKey];
                }
            }

            return value;
        }

        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// Ключ кэша записывается в указанное поле. Если fieldName является числовым полем, то 
        /// лидирующие нули ключа игнорируются.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша. Служит значением для поля fieldName</param>
        /// <param name="keyField">Имя поля ключа</param>
        /// <param name="valueField">Имя поля значения</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <returns>Значение записи кэша</returns>
		protected int PumpCachedRow(Dictionary<string, int> cache, DataTable dt, IEntity obj, string key, string cacheKey,
            string keyField, string valueField, object[] fieldsMapping)
        {
            int value = -1;
            if (cache == null)
                return PumpOriginalRow(obj, dt, fieldsMapping, null);
            else
            {
                string tmpKey = key;
                DataColumn column = dt.Columns[keyField];
                if (column.DataType != typeof(string)) tmpKey = tmpKey.TrimStart('0').PadLeft(1, '0');

                if (!cache.ContainsKey(cacheKey))
                {
                    DataRow row = PumpRow(obj, dt, (object[])CommonRoutines.ConcatArrays(
                        new object[] { keyField, tmpKey }, fieldsMapping), true);

                    value = Convert.ToInt32(row[valueField]);
                    cache.Add(cacheKey, value);
                }
                else
                {
                    value = cache[cacheKey];
                }
            }
            return value;
        }

        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// Ключ кэша записывается в указанное поле. Если fieldName является числовым полем, то 
        /// лидирующие нули ключа игнорируются.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="mapping">Список пар поле - значение</param>
        /// <param name="key">Значение ключа кэша</param>
        /// <param name="valueField">Имя поля значения</param>
        /// <returns>Значение записи кэша</returns>
		protected int PumpCachedRow(Dictionary<string, int> cache, DataTable dt, IEntity obj, object[] mapping, string cacheKey, string valueField)
        {
            int value = -1;
            if (!cache.ContainsKey(cacheKey))
            {
                DataRow row = PumpRow(obj, dt, mapping, true);
                value = Convert.ToInt32(row[valueField]);
                cache.Add(cacheKey, value);
            }
            else
                value = cache[cacheKey];
            return value;
        }

        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// Ключ кэша записывается в указанное поле. Если fieldName является числовым полем, то 
        /// лидирующие нули ключа игнорируются.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша. Служит значением для поля fieldName</param>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <returns>ИД записи</returns>
		protected int PumpCachedRow(Dictionary<string, int> cache, DataTable dt, IEntity obj, string key,
            string fieldName, object[] fieldsMapping)
        {
            return PumpCachedRow(cache, dt, obj, key, fieldName, "ID", fieldsMapping);
        }

        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// Ключ кэша записывается в указанное поле. Если fieldName является числовым полем, то 
        /// лидирующие нули ключа игнорируются.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша. Служит значением для поля fieldName</param>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <returns>ИД записи</returns>
		protected int PumpCachedRow(Dictionary<string, int> cache, DataTable dt, IEntity obj, object key,
            string fieldName, object[] fieldsMapping)
        {
            return PumpCachedRow(cache, dt, obj, Convert.ToString(key), fieldName, "ID", fieldsMapping);
        }
        #endregion

        #region Dictionary<int, int>
        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// Ключ кэша записывается в указанное поле. Если fieldName является числовым полем, то 
        /// лидирующие нули ключа игнорируются.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша</param>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <returns>ИД записи</returns>
		protected int PumpCachedRow(Dictionary<int, int> cache, DataTable dt, IEntity obj, int key,
            string fieldName, object[] fieldsMapping)
        {
            return PumpCachedRow(cache, dt, obj, key, fieldName, "ID", fieldsMapping);
        }

        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// Ключ кэша записывается в указанное поле. Если fieldName является числовым полем, то 
        /// лидирующие нули ключа игнорируются.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша. Служит значением для поля fieldName</param>
        /// <param name="keyField">Имя поля ключа</param>
        /// <param name="valueField">Имя поля значения</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <returns>Значение записи кэша</returns>
		protected int PumpCachedRow(Dictionary<int, int> cache, DataTable dt, IEntity obj, int key,
            string keyField, string valueField, object[] fieldsMapping)
        {
            int value = -1;

            if (cache == null)
            {
                return PumpOriginalRow(obj, dt, fieldsMapping, null);
            }
            else
            {
                if (!cache.ContainsKey(key))
                {
                    if (string.IsNullOrEmpty(valueField))
                    {
                        value = PumpRow(dt, obj, fieldsMapping);
                        cache.Add(key, value);
                    }
                    else
                    {
                        DataRow row = PumpRow(obj, dt, (object[])CommonRoutines.ConcatArrays(
                            new object[] { keyField, key }, fieldsMapping), true);

                        value = Convert.ToInt32(row[valueField]);
                        cache.Add(key, value);
                    }
                }
                else
                {
                    value = cache[key];
                }
            }

            return value;
        }
        #endregion

        #region Dictionary<int, DataRow>
        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <returns>ИД записи</returns>
		protected int PumpCachedRow(Dictionary<int, DataRow> cache, DataTable dt, IEntity obj, int key,
            object[] fieldsMapping)
        {
            int id = -1;

            if (cache == null)
            {
                return PumpOriginalRow(obj, dt, fieldsMapping, null);
            }
            else
            {
                if (!cache.ContainsKey(key))
                {
                    DataRow row = PumpRow(obj, dt, fieldsMapping, true);
                    cache.Add(key, row);

                    id = Convert.ToInt32(row["ID"]);
                }
                else
                {
                    id = Convert.ToInt32(cache[key]["ID"]);
                }
            }

            return id;
        }
        #endregion

        #region Dictionary<string, DataRow>

        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <param name="rePump">true - если такая запись есть, то ее значения переписываются из fieldsMapping</param>
        /// <returns>ИД записи</returns>
		protected DataRow PumpCachedRow(Dictionary<string, DataRow> cache, DataTable dt, IEntity obj, string key,
            object[] fieldsMapping, bool rePump)
        {
            DataRow row = null;

            if (cache == null)
            {
                return PumpOriginalRow(dt, obj, fieldsMapping, null);
            }
            else
            {
                if (!cache.ContainsKey(key))
                {
                    row = PumpRow(obj, dt, fieldsMapping, true);
                    cache.Add(key, row);
                }
                else
                {
                    row = cache[key];

                    // Eсли такая запись есть, то ее значения переписываются из fieldsMapping
                    if (rePump)
                    {
                        CopyValuesToRow(row, fieldsMapping);
                    }
                }
            }

            return row;
        }

        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <param name="rePump">true - если такая запись есть, то ее значения переписываются из fieldsMapping</param>
        /// <returns>ИД записи</returns>
		protected int PumpCachedRow(Dictionary<string, DataRow> cache, IEntity obj, DataTable dt, string key,
            object[] fieldsMapping, bool rePump)
        {
            return Convert.ToInt32(PumpCachedRow(cache, dt, obj, key, fieldsMapping, rePump)["ID"]);
        }

        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <returns>ИД записи</returns>
		protected int PumpCachedRow(Dictionary<string, DataRow> cache, DataTable dt, IEntity obj, string key,
            object[] fieldsMapping)
        {
            return PumpCachedRow(cache, obj, dt, key, fieldsMapping, false);
        }

        /// <summary>
        /// Закачивает строку классификатора/фактов. Сначала ищет в кэше. Если найдено, то переписывает
        /// значения полей строки значениями из fieldsMapping
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="dt">Таблица классификатора/фактов</param>
        /// <param name="obj">IClassifier/IFactTable</param>
        /// <param name="key">Значение ключа кэша</param>
        /// <param name="fieldsMapping">Список пар поле - значение</param>
        /// <returns>ИД записи</returns>
		protected int RepumpCachedRow(Dictionary<string, DataRow> cache, DataTable dt, IEntity obj, string key,
            object[] fieldsMapping)
        {
            return PumpCachedRow(cache, obj, dt, key, fieldsMapping, true);
        }
        #endregion

        #endregion Функции для работы с кэшированными строками


        #region Функции для работы с ячейками таблиц

        /// <summary>
        /// Возвращает значение ячейки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение ячейки</returns>
        protected int GetIntCellValue(DataRow row, string column, object defaultValue)
        {
            try
            {
                if (row == null || column == string.Empty || !row.Table.Columns.Contains(column) ||
                    row.IsNull(column))
                {
                    return Convert.ToInt32(defaultValue);
                }
                else
                {
                    return Convert.ToInt32(row[column]);
                }
            }
            catch
            {
                return Convert.ToInt32(defaultValue);
            }
        }

        /// <summary>
        /// Возвращает значение ячейки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение ячейки</returns>
        protected int GetIntCellValue(DataRow row, int column, object defaultValue)
        {
            try
            {
                return GetIntCellValue(row, row.Table.Columns[column].ColumnName, defaultValue);
            }
            catch
            {
                return Convert.ToInt32(defaultValue);
            }
        }

        protected int GetIntCellValue(DataRow row, DataColumn clmn, int defaultValue)
        {
            try
            {
                return Convert.ToInt32(row[clmn]);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Возвращает значение ячейки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение ячейки</returns>
        protected double GetDoubleCellValue(DataRow row, string column, object defaultValue)
        {
            try
            {
                if (row == null || column == string.Empty || !row.Table.Columns.Contains(column) ||
                    row.IsNull(column))
                {
                    return Convert.ToDouble(defaultValue);
                }
                else
                {
                    return Convert.ToDouble(row[column]);
                }
            }
            catch
            {
                return Convert.ToDouble(defaultValue);
            }
        }

        /// <summary>
        /// Возвращает значение ячейки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение ячейки</returns>
        protected double GetDoubleCellValue(DataRow row, int column, object defaultValue)
        {
            try
            {
                return GetDoubleCellValue(row, row.Table.Columns[column].ColumnName, defaultValue);
            }
            catch
            {
                return Convert.ToDouble(defaultValue);
            }
        }

        protected decimal GetDecimalCellValue(DataRow row, string column, decimal defaultValue)
        {
            if (row == null || column == string.Empty || !row.Table.Columns.Contains(column) || row.IsNull(column))
                return defaultValue;
            else
                return Convert.ToDecimal(row[column]);
        }

        protected decimal GetDecimalCellValue(DataRow row, int column, decimal defaultValue)
        {
            return GetDecimalCellValue(row, row.Table.Columns[column].ColumnName, defaultValue);
        } 

        /// <summary>
        /// Возвращает значение ячейки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение ячейки</returns>
        protected string GetStringCellValue(DataRow row, string column, object defaultValue)
        {
            try
            {
                if (row == null || column == string.Empty || !row.Table.Columns.Contains(column) ||
                    row.IsNull(column))
                {
                    return Convert.ToString(defaultValue);
                }
                else
                {
                    string value = Convert.ToString(row[column]).Trim();
                    if (value == string.Empty)
                    {
                        return Convert.ToString(defaultValue);
                    }
                    else
                    {
                        return value;
                    }
                }
            }
            catch
            {
                return Convert.ToString(defaultValue);
            }
        }

        /// <summary>
        /// Возвращает значение ячейки
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение ячейки</returns>
        protected string GetStringCellValue(DataRow row, int column, object defaultValue)
        {
            try
            {
                return GetStringCellValue(row, row.Table.Columns[column].ColumnName, defaultValue);
            }
            catch
            {
                return Convert.ToString(defaultValue);
            }
        }

        /// <summary>
        /// Возвращает значение ячейки.
        /// Применяется в тех случаях, когда доподлинно не известно название столбца.
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="columns">Список столбцов</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение ячейки</returns>
        protected object GetMultiCellValue(DataRow row, string[] columns, object defaultValue)
        {
            try
            {
                string columnName = string.Empty;

                int count = columns.GetLength(0);
                for (int i = 0; i < count; i++)
                {
                    if (row.Table.Columns.Contains(columns[i]))
                    {
                        columnName = columns[i];
                        break;
                    }
                }
                if (columnName == string.Empty)
                {
                    return defaultValue;
                }

                return row[columnName];
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Возвращает значение ячейки.
        /// Если значение ячейки равно указанному или пустое, то возвращается значение по умолчанию.
        /// </summary>
        /// <param name="row">Строка</param>
        /// <param name="column">Столбец</param>
        /// <param name="relativeValue">Значение ячейки, при котором возращается значение по умолчанию</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение ячейки</returns>
        protected double GetDoubleCellRelativeValue(DataRow row, string column, double relativeValue, 
            double defaultValue)
        {
            try
            {
                if (row == null || column == string.Empty || !row.Table.Columns.Contains(column) ||
                    row.IsNull(column))
                {
                    return defaultValue;
                }
                if (Convert.ToDouble(row[column]) == relativeValue)
                {
                    return defaultValue;
                }
                else
                {
                    return Convert.ToDouble(row[column]);
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion Функции для работы с ячейками таблиц
    }
}