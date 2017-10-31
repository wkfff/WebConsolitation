using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme
{
    /// <summary>
    /// Коллекция классификаторов
    /// </summary>
    internal class ConversionTableCollection : DisposableObject, IConversionTableCollection, ICollection
    {
        private static ConversionTableCollection instance;

        private ConversionTableCollection()
        {
        }

        /// <summary>
        /// Экземпляр объекта.
        /// </summary>
        public static ConversionTableCollection Instance
        {
            [DebuggerStepThrough]
            get
            {
                if (instance == null)
                {
                    lock (typeof(ConversionTableCollection))
                    {
                        instance = new ConversionTableCollection();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Возвращает коллекцию ключей (ID таблиц перекодировок)
        /// toBase
        /// </summary>
        public ICollection Keys
        {
            get
            {
                ArrayList keyList = new ArrayList();
                Database db = null;
                try
                {
                    db = (Database)SchemeDWH.Instance.DB;
                    DataTable dt = (DataTable)db.ExecQuery(
                        "select AssociationKey, RuleName, AssociationName from MetaConversionTablesCatalog",
                        QueryResultTypes.DataTable);
                    foreach (DataRow row in dt.Rows)
                        keyList.Add(String.Format("{0}.{1}", Convert.ToString(row[0]), Convert.ToString(row[1])));
                    return keyList;
                }
                finally
                {
                    if (db != null)
                        db.Dispose();
                }
            }
        }

        /// <summary>
        /// Возвращает количество елементов в коллекции
        /// toBase
        /// </summary>
        public int Count
        {
            get
            {
                Database db = null;
                try
                {
                    db = (Database)SchemeDWH.Instance.DB;
                    return Convert.ToInt32(db.ExecQuery("select Count(*) from MetaConversionTable", QueryResultTypes.Scalar)); 
                }
                finally
                {
                    if (db != null)
                        db.Dispose();
                }
            }
        }

        /// <summary>
        /// Выделяет из полного имени таблицы перекодировок имя ассоциации и имя правила сопоставления 
        /// </summary>
        /// <param name="key">Полное имя таблицы перекодировок</param>
        /// <param name="associationName">Наименование ассоциации</param>
        /// <param name="ruleName">Наименование правила сопоставления</param>
        private static void GetNameParts(string key, out string associationName, out string ruleName)
        {
            string[] parts = key.Split('.');
            if (parts.GetLength(0) < 2)
                throw new Exception(String.Format("Указано некорректное имя таблицы перекодировки: {0}", key));

            associationName = parts[0];
            ruleName = parts[1];
        }

        /// <summary>
        /// Определяет, содержит ли коллекция указанную таблицу перекодировок
        /// </summary>
        /// <param name="association">Объект таблицы перекодировок</param>
        /// <param name="ruleName"></param>
        /// <returns>true если найден, иначе false</returns>
        public bool ContainsKey(IAssociation association, string ruleName)
        {
            Database db = (Database)SchemeDWH.Instance.DB;
            try
            {
                int count = Convert.ToInt32(db.ExecQuery(
                    "select count(ID) from MetaConversionTable where RefAssociation = ? and AssociateRule = ?",
                    QueryResultTypes.Scalar,
                    db.CreateParameter("RefAssociation", association.ID),
                    db.CreateParameter("Rule", ruleName)));
                return count > 0;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Определяет, содержит ли коллекция указанную таблицу перекодировок
        /// </summary>
        /// <param name="key">Объект таблицы перекодировок</param>
        /// <returns>true если найден, иначе false</returns>
        public bool ContainsKey(int key)
        {
            Database db = null;
            try
            {
                db = (Database)SchemeDWH.Instance.DB;
                int count = Convert.ToInt32(db.ExecQuery(
                    "select count(ID) from MetaConversionTable where ID = ?",
                    QueryResultTypes.Scalar, db.CreateParameter("ID", key)));
                return count > 0;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Добавляет новую таблицу перекодировок в базу данных
        /// </summary>
        /// <param name="сonversionTable">ConversionTable таблица перекодировок</param>
        /// <param name="db">Объект для доступа к БД</param>
        /// <remarks>
        /// Просто добавляет новую таблицу перекодировок
        /// </remarks>
        private static void AddNew(ConversionTable сonversionTable, IDatabase db)
        {
            сonversionTable.AttachToDB(db);
        }

        /// <summary>
        /// Добавляет новую таблицу перекодировок в базу данных
        /// </summary>
        /// <param name="value">ConversionTable таблица перекодировок</param>
        /// <returns>ID добавленной таблицы перекодировок</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int Add(object value)
        {
            IDatabase db = null;
            try
            {
                db = SchemeDWH.Instance.DB;
                ConversionTable сonversionTable = (ConversionTable)value;
                if (!this.ContainsKey(сonversionTable.ID))
                    AddNew(сonversionTable, db);
                return сonversionTable.ID;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        /// <summary>
        /// Индексатор возвращает таблицу перекодировок с указанным именем
        /// </summary>
        public IConversionTable this[string key]
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                string associationName;
                string ruleName;

                GetNameParts(key, out associationName, out ruleName);

                return this[(IAssociation)SchemeClass.Instance.Associations[associationName], ruleName];
            }
        }

        /// <summary>
        /// Индексатор возвращает таблицу перекодировок с указанным именем
        /// </summary>
        public IConversionTable this[IAssociation association, string ruleName]
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                Database db = null;
                try
                {
                    BridgeAssociation bridgeAssociation = (BridgeAssociation)association;

                    db = (Database)SchemeDWH.Instance.DB;
                    DataTable dt = (DataTable)db.ExecQuery(
                        "select ID from MetaConversionTable where RefAssociation = ? and AssociateRule = ?",
                        QueryResultTypes.DataTable,
                        db.CreateParameter("RefAssociation", bridgeAssociation.ID),
                        db.CreateParameter("Rule", ruleName));

                    ConversionTable conversionTable;
                    if (dt.Rows.Count > 0)
                    {
                        conversionTable = new ConversionTable(bridgeAssociation, ruleName);
                        conversionTable.ID = Convert.ToInt32(dt.Rows[0][0]);
                        conversionTable.SemanticName = "Semantic";
                    }
                    else
                    {
                        conversionTable = new ConversionTable(bridgeAssociation, ruleName);
                        conversionTable.ID = -1;
                        conversionTable.SemanticName = "Semantic";
                    }
                    return conversionTable;
                }
                finally
                {
                    if (db != null)
                        db.Dispose();
                }
            }
        }

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
            throw new Exception("Метод GetEnumerator() не реализован. Для доступа к элементам сначала нужно получить коллекцию ключей Key, а потом обращаться по ключу к нужному элементу.");
        }

        #endregion

        #region ICollection2DataTable Members

        public DataTable GetDataTable()
        {
            DataColumn[] columns = new DataColumn[7];
            columns[0] = new DataColumn("Name", typeof(String));
            columns[1] = new DataColumn("DataSemantic", typeof(String));
            columns[2] = new DataColumn("DataCaption", typeof(String));
            columns[3] = new DataColumn("BridgeSemantic", typeof(String));
            columns[4] = new DataColumn("BridgeCaption", typeof(String));
            columns[5] = new DataColumn("RuleName", typeof(String));
            columns[6] = new DataColumn("ObjectKey", typeof(String));

            DataTable dt = new DataTable(GetType().Name);
            dt.Columns.AddRange(columns);

            foreach (string itemName in this.Keys)
            {
                string associationName, ruleName;
                GetNameParts(itemName, out associationName, out ruleName);
                
                DataRow row = dt.NewRow();
                IAssociation association = (IAssociation)SchemeClass.Instance.Associations[associationName];
                if (!((IBridgeAssociation) association).AssociateRules.ContainsKey(ruleName)) continue;

                row[0] = associationName;
                row[1] = ((Entity)association.RoleData).SemanticCaption;
                row[2] = association.RoleData.Caption;
                row[3] = ((Entity)association.RoleBridge).SemanticCaption;
                row[4] = association.RoleBridge.Caption;
                row[5] = ((IBridgeAssociation)association).AssociateRules[ruleName].Name;
                row[6] = ruleName;
                /*                try
                                    {
                                        IConversionTable ct = this[itemName];
                                    }
                                    catch (Exception e)
                                    {
                                        row.RowError = e.Message;
                                    }*/
                dt.Rows.Add(row);
            }

            return dt;
        }

        #endregion
    }
}