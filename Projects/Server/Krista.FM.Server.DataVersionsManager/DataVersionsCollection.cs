using System;
using Krista.FM.ServerLibrary;
using System.Data;
using System.Runtime.CompilerServices;
using Krista.FM.Common;
using System.Collections;

namespace Krista.FM.Server.DataVersionsManager
{
    /// <summary>
    /// Коллекция версий классификаторов
    /// </summary>
    public class DataVersionsCollection : DisposableObject, IDataVersionsCollection, ICollection
    {
        /// <summary>
        /// Менеджер управления версиями классификаторов
        /// </summary>
        private readonly DataVersionManager manager;

        private readonly object syncRoot;

        public DataVersionsCollection(DataVersionManager manager)
        {
            this.manager = manager;

            this.syncRoot = new object();
        }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public DataVersionsCollection()
        {
            
        }

        #region IDataVersionsCollection Members
        
        public IDataVersion Create()
        {
            return new DataVersion();
        }

        /// <summary>
        /// Индексатор, версию определяем как guid объекта и ID источника
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="sourceID"></param>
        /// <returns></returns>
        public IDataVersion this[string objectKey, int sourceID]
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                IDataVersion item = null;
                IDatabase db = manager.Scheme.SchemeDWH.DB;

                try
                {
                    DataTable dt = (DataTable)db.ExecQuery("Select * from OBJECTVERSIONS where SOURCEID = ? and OBJECTKEY = ?",
                                                           QueryResultTypes.DataTable,
                                                           db.CreateParameter("SOURCEID", sourceID),
                                                           db.CreateParameter("OBJECTKEY", objectKey));

                    if (dt.Rows.Count != 0)
                    {
                        DataRow row = dt.Rows[0];
                        item = Create();
                        item.ID = Convert.ToInt32(row[0]);
                        item.SourceID = Convert.ToInt32(row[1]);
                        item.ObjectKey = Convert.ToString(row[2]);
                        item.PresentationKey = Convert.ToString(row[3]);
                        item.Name = Convert.ToString(row[4]);
                        item.IsCurrent = Convert.ToBoolean(row[5]);
                    }
                }
                finally
                {
                    db.Dispose();
                }

                return item;
            }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int Add(object value)
        {
            IDatabase db = manager.Scheme.SchemeDWH.DB;
            try
            {
                DataVersion dataVersion = (DataVersion)value;
                if (!manager.DataVersions.Containts(dataVersion))
                {
                    AddNew(db, dataVersion);
                    return dataVersion.ID;
                }
                else
                {
                    return dataVersion.ID;
                }
            }
            finally
            {
                db.Dispose();
            }

            return -1;
        }

        public void RemoveObject(string objectKey)
        {
            IDatabase db = manager.Scheme.SchemeDWH.DB;
            try
            {
                db.BeginTransaction();

                db.ExecQuery("delete from ObjectVersions where ObjectKey = ?",
                    QueryResultTypes.NonQuery, db.CreateParameter("ObjectKey", objectKey));

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Создание версии классификатора
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dataVersion"></param>
        private void AddNew(IDatabase db, IDataVersion dataVersion)
        {
            dataVersion.ID = db.GetGenerator("g_ObjectVersions");
            if (String.IsNullOrEmpty(dataVersion.PresentationKey))
                db.ExecQuery("insert into OBJECTVERSIONS (ID, SOURCEID, OBJECTKEY, NAME, ISCURRENT) values(?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", dataVersion.ID),
                    db.CreateParameter("SOURCEID", dataVersion.SourceID),
                    db.CreateParameter("OBJECTKEY", dataVersion.ObjectKey),
                    db.CreateParameter("NAME", dataVersion.Name),
                    db.CreateParameter("ISCURRENT", Convert.ToInt32(dataVersion.IsCurrent)));
            else
                db.ExecQuery("insert into OBJECTVERSIONS " +
                    "(ID, SOURCEID, OBJECTKEY, PRESENTATIONKEY, NAME, ISCURRENT) "+
                    "values(?, ?, ?, ?, ?, ?)",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("ID", dataVersion.ID),
                    db.CreateParameter("SOURCEID", dataVersion.SourceID),
                    db.CreateParameter("OBJECTKEY", dataVersion.ObjectKey),
                    db.CreateParameter("PRESENTATIONKET", dataVersion.PresentationKey),
                    db.CreateParameter("NAME", dataVersion.Name),
                    db.CreateParameter("ISCURRENT", Convert.ToInt32(dataVersion.IsCurrent)));

            /*IEntity entity = (IEntity) manager.Scheme.RootPackage.FindEntityByName(dataVersion.ObjectKey);
            if (entity != null && entity is IBridgeClassifier)
            {
                manager.Scheme.Associations.CreateItem(entity, entity, AssociationClassTypes.BridgeBridge);
            }*/
        }
        
        /// <summary>
        /// Поиск версии классификатора
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Containts(object value)
        {
            return FindDataVersion(value) != null;
        }

        /// <summary>
        /// Поиск версии классификатора. Версия определяется objectKey объекта и источником
        /// </summary>
        /// <param name="value"></param>
        /// <returns>ID версии в базе, null - nothing</returns>
        public int? FindDataVersion(object value)
        {
            int? result;

            IDatabase db = manager.Scheme.SchemeDWH.DB;
            try
            {
                DataVersion dataVersion = (DataVersion)value;

                result = Convert.ToInt32(db.ExecQuery(
                    "select ID from OBJECTVERSIONS " +
                        "where UPPER(OBJECTKEY) = UPPER(?) and UPPER(SOURCEID) = ?",
                        QueryResultTypes.Scalar,
                        db.CreateParameter("OBJECTKEY", dataVersion.ObjectKey),
                        db.CreateParameter("SOURCEID", dataVersion.SourceID)));

            }
            finally
            {
                db.Dispose();
            }

            return (result == 0) ? null : result;
        }

        public int? FindCurrentVersion(string objectKey)
        {
            int? result;

            IDatabase db = manager.Scheme.SchemeDWH.DB;
            try
            {
                result = Convert.ToInt32(db.ExecQuery(
                    "select SourceID from OBJECTVERSIONS " +
                    "where UPPER(OBJECTKEY) = UPPER(?) and UPPER(ISCURRENT) = 1",
                    QueryResultTypes.Scalar,
                    db.CreateParameter("OBJECTKEY", objectKey)));
            }
            finally
            {
                db.Dispose();
            }

            return result;
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get 
            { 
                IDatabase db = manager.Scheme.SchemeDWH.DB;
                try
                {
                    return Convert.ToInt32(db.ExecQuery(
                        "select count(*) from OBJECTVERSIONS", 
                        QueryResultTypes.Scalar
                           ));
                }
                finally
                {
                    db.Dispose();
                }
         }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return syncRoot; }
        }

        #endregion

        #region IDataVersionsCollection Members

        /// <summary>
        /// Дополнительная процедура при удалении версии структуры классификатора
        /// Для версий в поле представления указываем значение по умолчанию
        /// </summary>
        /// <param name="key"></param>
        public void RemovePresentation(string key)
        {
            IDatabase db = manager.Scheme.SchemeDWH.DB;

            try
            {
                db.ExecQuery("update objectversions set presentationkey = default where presentationkey = ?",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("PresentationKey", key));
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Дополнительная процедура при удалении источника данных.
        /// Удаляем все версии, где используется этот источник
        /// </summary>
        /// <param name="SourceID"></param>
        public void RemoveSource(int SourceID)
        {
            IDatabase db = manager.Scheme.SchemeDWH.DB;

            try
            {
                db.ExecQuery("delete from objectversions t where t.sourceid = ?",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("SourceID", Convert.ToString(SourceID)));
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Обновление версии структуры для существующей версии классификатора
        /// </summary>
        /// <param name="presentationKey"></param>
        public void UpdatePresentation(string presentationKey, string sourceID, string activeObjectKey, bool isCurrent)
        {
            IDatabase db = manager.Scheme.SchemeDWH.DB;

            try
            {
                db.ExecQuery("update objectversions set presentationkey = ?, iscurrent = ? where sourceid = ? and objectKey = ?",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("PresentationKey", Convert.ToString(presentationKey)),
                    db.CreateParameter("IsCurrent", Convert.ToInt32(isCurrent)),
                    db.CreateParameter("SourceID", Convert.ToString(sourceID)),
                    db.CreateParameter("ObjectKey", Convert.ToString(activeObjectKey)));
            }
            finally
            {
                db.Dispose();
            }
        }

        #endregion
    }
}
