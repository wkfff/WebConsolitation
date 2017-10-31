using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using System.Collections;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.GlobalConsts
{
    public class GlobalConstsManager: DisposableObject, IGlobalConstsManager
    {
        private GlobalConstsCollection consts;

        private IScheme scheme;

        public GlobalConstsManager(IScheme scheme)
        {
            if (scheme == null)
                throw new Exception("Не задан интерфейс схемы");
            this.scheme = scheme;
            consts = (GlobalConstsCollection)Consts;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // освобождаем управляемые ресурсы
                consts.Dispose();
            }
            // освобождаем неуправляемые ресурсы
            // ...
        }

        public IGlobalConstsCollection Consts
        {
            get 
            {
                if (consts == null)
                {
                    consts = new GlobalConstsCollection(scheme);
                }
                return (IGlobalConstsCollection)consts; 
            }
        }

        public DataTable GetDataTable()
        {
            return Consts.GetData(GlobalConstsTypes.Configuration);
        }

        public void ResetCache()
        {
            consts = null;
        }
    }

    public class GlobalConstsCollection: DisposableObject, IGlobalConstsCollection
    {
        private IScheme _scheme;

        private DataTable constsTable;

        private const string selectSQLQuery = "select id, Name, Caption, Description, Value, ConstValueType, ConstCategory, ConstType from GlobalConsts";

        internal static string _insertParamsSQL = "insert into GlobalConsts" +
            " (ID, Name, Caption, Description, Value, ConstValueType, ConstCategory, ConstType)" +
            " values (?, ?, ?, ?, ?, ?, ?, ?)";

        private static string _updateParamsSQL = "update GlobalConsts set ID = ?, Name = ?, " +
            "Caption = ?, Description = ?, Value = ?, ConstValueType = ?, ConstCategory = ?, ConstType = ? " +
            "where ID = ?";

        private static string _deleteParamsSQL = "delete from GlobalConsts where ID = ?";

        private const string generatorName = "g_GlobalConsts";

        public GlobalConstsCollection(IScheme scheme)
        {
            _scheme = scheme;
            constsTable = new DataTable();
            InternalLoadData(string.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public DataTable GetData(GlobalConstsTypes constType)
        {
            return constsTable;
        }

        // напрямую заполняем таблицу данными
        public void GetData(GlobalConstsTypes constType, ref DataTable data)
        {
            data = this.constsTable.Clone();
            foreach (DataRow row in this.constsTable.Rows)
            {
                if (row.RowState == DataRowState.Unchanged)
                    if (Convert.ToInt32(row["CONSTTYPE"]) == (int)constType)
                        data.Rows.Add(row.ItemArray);
            }
            data.AcceptChanges();
        }

        internal DataRow GetConstData(string constName)
        {
            return GetConstRow(constName);
        }

        public void ApplyChanges(ref DataTable changes)
        {
            IDataUpdater duConsts = null;
            Database db = (Database)_scheme.SchemeDWH.DB;
            try
            {
                duConsts = InitEditParamsDataAdapter(db);
                if (changes != null)
                {
                    duConsts.Update(ref constsTable);
                    constsTable.AcceptChanges();
                }
            }
            finally
            {
                if (db != null)
                    db.Dispose();
                if (duConsts != null)
                    duConsts.Dispose();
            }
        }

        public bool ContainCaption(string caption, ref int id)
        {
            DataRow[] rows = constsTable.Select(String.Format("CAPTION = '{0}'", caption));
            if (rows.Length > 0)
            {
                id = Convert.ToInt32(rows[0]["ID"]);
                return true;
            }
            id = -1;
            return false;
        }

        private DataRow GetConstRow(string constName)
        {
            DataRow[] rows = constsTable.Select(String.Format("NAME = '{0}'", constName));
            if (rows.Length > 0)
                return rows[0];
            else
                return null;
        }

        private void InternalLoadData(string filter)
        {
            using (IDataUpdater duConsts = GetDataUpdater(selectSQLQuery))
            {
                duConsts.Fill(ref constsTable);
            }
        }

        internal static void AppendParamsCommandParameters(Database db, IDbCommand command, bool includeIDConstraintParameter)
        {
            IDbDataParameter prm = null;
            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            ((DbParameter)prm).IsNullable = true;
            command.Parameters.Add(prm);
            // Name
            prm = db.CreateParameter("Name", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "Name";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("Caption", DataAttributeTypes.dtString, 1000);
            prm.SourceColumn = "Caption";
            //prm.IsNullable = true;
            command.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("Description", DataAttributeTypes.dtString, 2000);
            prm.SourceColumn = "Description";
            command.Parameters.Add(prm);
            // Value
            prm = db.CreateParameter("Value", DataAttributeTypes.dtString, 2000);
            prm.SourceColumn = "Value";
            command.Parameters.Add(prm);
            // ConstValueType
            prm = db.CreateParameter("ConstValueType", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ConstValueType";
            command.Parameters.Add(prm);
            // ConstCategory
            prm = db.CreateParameter("ConstCategory", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ConstCategory";
            command.Parameters.Add(prm);
            // ConstType
            prm = db.CreateParameter("ConstType", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ConstType";
            command.Parameters.Add(prm);

            if (includeIDConstraintParameter)
            {
                // ID
                prm = db.CreateParameter("IDConstraint", DataAttributeTypes.dtInteger, 10);
                prm.SourceColumn = "ID";
                command.Parameters.Add(prm);
            }
        }

        private IDataUpdater InitEditParamsDataAdapter(Database db)
        {
            IDbDataAdapter adapter = db.GetDataAdapter();
            // команда вставки данных
            adapter.InsertCommand = db.Connection.CreateCommand();
            AppendParamsCommandParameters(db, adapter.InsertCommand, false);
            adapter.InsertCommand.CommandText = db.GetQuery(_insertParamsSQL, adapter.InsertCommand.Parameters);

            // команда обновления данных
            adapter.UpdateCommand = db.Connection.CreateCommand();
            AppendParamsCommandParameters(db, adapter.UpdateCommand, true);
            adapter.UpdateCommand.CommandText = db.GetQuery(_updateParamsSQL, adapter.UpdateCommand.Parameters);

            // команда удаления данных
            adapter.DeleteCommand = db.Connection.CreateCommand();
            IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            adapter.DeleteCommand.Parameters.Add(prm);
            adapter.DeleteCommand.CommandText = db.GetQuery(_deleteParamsSQL, adapter.DeleteCommand.Parameters);

            DataUpdater upd = new DataUpdater(adapter, null, db);
            upd.Transaction = db.Transaction;
            return (IDataUpdater)upd;
        }

        private IDataUpdater GetDataUpdater(string query)
        {
            IDataUpdater updater;
            IDatabase db = _scheme.SchemeDWH.DB;
            try
            {
                updater = db.GetDataUpdater(query);
            }
            finally
            {
                db.Dispose();
            }
            return updater;
        }

        #region методы для неуправляемого кода

        public IGlobalConst ConstByName(string constName)
        {
            if (GetConstRow(constName) == null)
                throw new Exception(String.Format("Константа с идентификатором {0} коллекции не существует", constName));
            return (IGlobalConst)new GlobalConst(this, constName);
        }

        public IGlobalConst AddNew(string constName)
        {
            if (GetConstRow(constName) != null)
                throw new Exception(String.Format("Константа с идентификатором {0} уже присутствует в коллекции", constName));
            DataRow constRow = constsTable.Rows.Add(null, null);
            constRow["NAME"] = constName;
            IDatabase db = this._scheme.SchemeDWH.DB;
            try
            {
                constRow["ID"] = db.GetGenerator(generatorName);
            }
                finally
            {
                db.Dispose();
            }
            return (IGlobalConst)new GlobalConst(this, constName);
        }

        public IGlobalConst AddNew(int ID, string constName)
        {
            if (GetConstRow(constName) != null)
                throw new Exception(String.Format("Константа с идентификатором {0} уже присутствует в коллекции", constName));
            DataRow constRow = constsTable.Rows.Add(null, null);
            constRow["NAME"] = constName;
            constRow["ID"] = ID;
            return (IGlobalConst)new GlobalConst(this, constName);
        }

        public int GetGeneratorValue()
        {
            int genValue = 0;
            IDatabase db = this._scheme.SchemeDWH.DB;
            try
            {
                genValue = db.GetGenerator(generatorName);
            }
            finally
            {
                db.Dispose();
            }
            return genValue;
        }

        public void ApplyChanges()
        {
            DataTable dt = constsTable.GetChanges();
            ApplyChanges(ref dt);
        }

        public void CancelChanges()
        {
            constsTable.RejectChanges();
        }

        #endregion

        #region реализация интерфейса IGlobalConstsCollection

        public bool ContainsKey(string key)
        {
            return GetConstRow(key) != null;
        }

        public void Add(string key, IGlobalConst globalConst)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Remove(string key)
        {
            DataRow row = GetConstRow(key);
            if (row != null)
            {
                row.Delete();
                return true;
            }
            return false;
        }

        public bool TryGetValue(string key, out IGlobalConst globalConst)
        {
            globalConst = null;
            if (GetConstRow(key) != null)
            {
                globalConst = (IGlobalConst) new GlobalConst(this, key);
                return true;
            }
            return false;
        }

        public IGlobalConst this[string key]
        {
            get
            {
                if (GetConstRow(key) != null)
                    return (IGlobalConst) new GlobalConst(this, key);
                else
                    throw new KeyNotFoundException(String.Format("Ключ \"{0}\" не найден в коллекции \"{1}\"", key, this.ToString()));
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        ICollection<string> IDictionary<string, IGlobalConst>.Keys
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ICollection<IGlobalConst> Values
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public virtual void Add(KeyValuePair<string, IGlobalConst> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool ICollection<KeyValuePair<string, IGlobalConst>>.IsReadOnly
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public virtual void Clear()
        {
            foreach (DataRow row in this.constsTable.Rows)
            {
                row.Delete();
            }
        }

        public bool Contains(KeyValuePair<string, IGlobalConst> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public virtual void CopyTo(KeyValuePair<string, IGlobalConst>[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get { return this.constsTable.Rows.Count; }
        }

        public virtual bool Remove(KeyValuePair<string, IGlobalConst> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IEnumerator<KeyValuePair<string, IGlobalConst>> IEnumerable<KeyValuePair<string, IGlobalConst>>.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    [Serializable]
    public class GlobalConst : DisposableObject, IGlobalConst
    {
        private GlobalConstsCollection constsCollection;

        private string constName;

        public GlobalConst(GlobalConstsCollection constsCollection, string name)
        {
            this.constsCollection = constsCollection;
            constName = name;
        }

        #region реализация интерфейса IGlobalConst

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        /// <param name="disposing">вызван пользователем или сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                constsCollection = null;
            }
            base.Dispose(disposing);
        }

        public int ID
        {
            get { return Convert.ToInt32(constsCollection.GetConstData(constName)["ID"]); }
            //set { constsCollection.GetConstData(constName)["ID"] = value; }
        }

        public string Name
        {
            get { return constsCollection.GetConstData(constName)["NAME"].ToString(); }
            set
            {
                if (this.Name == value)
                    return;
                if (constsCollection.ContainsKey(value))
                    throw new Exception("Константа с таким идентификатором уже присутствует в коллекции");
                constsCollection.GetConstData(constName)["NAME"] = value; 
            }
        }

        public string Caption
        {
            get { return constsCollection.GetConstData(constName)["CAPTION"].ToString(); }
            set
            {
                if (this.Caption == value)
                    return;
                int id = -1;
                if (constsCollection.ContainCaption(value, ref id))
                    throw new Exception("Константа с таким именем уже присутствует в коллекции");
                constsCollection.GetConstData(constName)["CAPTION"] = value; 
            }
        }

        public string Description
        {
            get { return constsCollection.GetConstData(constName)["DESCRIPTION"].ToString(); }
            set { constsCollection.GetConstData(constName)["DESCRIPTION"] = value; }
        }

        public object Value
        {
            get { return constsCollection.GetConstData(constName)["VALUE"]; }
            set { constsCollection.GetConstData(constName)["VALUE"] = value; }
        }

        public DataAttributeTypes ConstValueType
        {
            get { return (DataAttributeTypes)constsCollection.GetConstData(constName)["CONSTVALUETYPE"]; }
            set { constsCollection.GetConstData(constName)["CONSTVALUETYPE"] = (int)value; }
        }

        public GlobalConstCategories ConstCactegory
        {
            get { return (GlobalConstCategories)constsCollection.GetConstData(constName)["CONSTCATEGORY"]; }
            set { constsCollection.GetConstData(constName)["CONSTCATEGORY"] = (int)value; }
        }

        public GlobalConstsTypes ConstType
        {
            get { return (GlobalConstsTypes)constsCollection.GetConstData(constName)["CONSTTYPE"]; }
            set 
            {
                // смотрим, что бы константа не была конфигурационной...
                /*if (value == GlobalConstsTypes.Configuration)
                    throw new Exception("Константы такого типа не могут быть добавлены");*/
                constsCollection.GetConstData(constName)["CONSTTYPE"] = (int)value; 
            }
        }

        #endregion
    }
}
