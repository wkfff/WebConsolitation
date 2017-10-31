using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.AnalysisServices;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;

using Database=Krista.FM.Server.Common.Database;

namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// Обёртка для доступа к объектам многомерной базы данных.
    /// </summary>
    public abstract class OlapDBWrapper : DisposableObject, IOlapDBWrapper
    {
        protected IScheme scheme;
        protected IOlapDatabase olapDatabase;

        private static string RelationName_ParentChild = "Parent_Child";
        private static string RelationName_CubesCubeDimensionLinks = "Cubes_CubeDimensionLinks";
        private static string RelationName_DimensionsCubeDimensionLinks = "Dimensions_CubeDimensionLinks";
        //private static string RelationName_BatchOlapObjects = "Batch_OlapObjects";

		private DataTable dataTableErrors = new DataTable("Errors");
		private static DataColumn column_DatabaseErrors_Error;
		private static DataColumn column_DatabaseErrors_ObjectType;
		private static DataColumn column_DatabaseErrors_ObjectId;
		private static DataColumn column_DatabaseErrors_ObjectName;

		private DataSet dataSetOlapBase = new DataSet();

		//Таблица OlapObjects
        private static DataColumn columnID;
        private static DataColumn columnObjectType;
        private static DataColumn columnObjectId;
        private static DataColumn columnObjectName;
        private static DataColumn columnParentId;
        private static DataColumn columnParentName;
        private static DataColumn columnUsed;
        private static DataColumn columnNeedProcess;
        private static DataColumn columnState;
        private static DataColumn columnLastProcessed;
        private static DataColumn columnRefBatchID;
        private static DataColumn columnProcessType;
        private static DataColumn columnProcessResult;
        private static DataColumn columnSynchronized;
        private static DataColumn columnFullName;
        private static DataColumn columnObjectKey;

        private static DataColumn columnStateName;
        private static DataColumn columnSelected;
        private static DataColumn columnRecordStatus;
        private static DataColumn columnRevision;
        private static DataColumn columnBatchOperations;

        //Таблица Batch
        private static DataColumn column_Batch_Id;
        private static DataColumn column_Batch_BatchId;
        private static DataColumn column_Batch_RefUser;
        private static DataColumn column_Batch_UserName;
        private static DataColumn column_Batch_AdditionTime;
        private static DataColumn column_Batch_BatchState;
        private static DataColumn column_Batch_BatchStateName;
        private static DataColumn column_Batch_SessionId;
        private static DataColumn column_Batch_Priority;
        private static DataColumn column_Batch_PriorityName;


        private const string sqlSelectOlapObjects =
            "select ID, OBJECTTYPE, OBJECTID, OBJECTNAME, PARENTID, PARENTNAME, USED, NEEDPROCESS, " +
            "STATE, LASTPROCESSED, REFBATCHID, PROCESSTYPE, PROCESSRESULT, SYNCHRONIZED, FullName, ObjectKey, REVISION, batchoperations " +
            "from OlapObjects " +
            "order by OBJECTTYPE, PARENTNAME, OBJECTNAME";

        private const string sqlUpdateOlapObjects = "update OlapObjects set" +
            " OBJECTTYPE = ?, OBJECTID = ?, OBJECTNAME = ?, PARENTID = ?, PARENTNAME = ?, USED = ?" +
            ", NEEDPROCESS = ?, STATE = ?, LASTPROCESSED = ?, REFBATCHID = ?, PROCESSTYPE = ?" +
            ", PROCESSRESULT = ?, SYNCHRONIZED = ?, FullName = ?, ObjectKey = ?, REVISION = ?, BatchOperations = ? where ID = ?";

        private const string sqlInsertOlapObjects = "insert into OlapObjects" +
            " (ID, OBJECTTYPE, OBJECTID, OBJECTNAME, PARENTID, PARENTNAME, USED, NEEDPROCESS, STATE," +
            " LASTPROCESSED, REFBATCHID, PROCESSTYPE, PROCESSRESULT, SYNCHRONIZED, FullName, ObjectKey, REVISION, batchoperations)" +
            " values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

        private const string sqlDeleteOlapObjects = "delete from OlapObjects where ID = ?";

        private const string sqlSelectBatch =
            "select ID, REFUSER, ADDITIONTIME, BATCHSTATE, SESSIONID, BATCHID, PRIORITY" +
            " from batch order by ADDITIONTIME DESC, BATCHSTATE, REFUSER";

        private const string sqlUpdateBatch =
            "update batch set REFUSER = ?, ADDITIONTIME = ?, BATCHSTATE = ?, SESSIONID = ?, BATCHID = ?" +
            ", PRIORITY = ? where ID = ?";

        private const string sqlInsertBatch =
            "insert into batch " +
            "(ID, REFUSER, ADDITIONTIME, BATCHSTATE, SESSIONID, BATCHID, PRIORITY) " +
            "values (?, ?, ?, ?, ?, ?, ?)";

        private const string sqlDeleteBatch = "delete from batch where ID = ?";
        private static Mutex mutexOlapObjects = new Mutex();
        private static Mutex mutexBatch = new Mutex();
        internal static Mutex mutexProcessBatch = new Mutex();

        //Все кубы многомерной базы.
        protected Dictionary<string, IProcessableObjectInfo> cubes;
        //Все измерения многомерной базы.
        protected Dictionary<string, IProcessableObjectInfo> dimensions;
        //Все разделы всех кубов многоменой базы.
        protected Dictionary<string, IProcessableObjectInfo> partitions = new Dictionary<string,IProcessableObjectInfo>();        
                
        ///// <summary>
        ///// Поле CubeId используется как часть составного ключа и поэтому не может быть null.
        ///// Поэтому, для тех элементов, для которых не задан конкретный куб (измерения и собственно кубы),
        ///// будем использовать эту константу в качестве идентификатора и имени куба.
        ///// </summary>        
        //private static readonly string defaultCubeId = "Куб не задан";

        protected class CubeDimensionLink
        {
            private readonly string cubeId;
            private readonly string dimensionId;

            public CubeDimensionLink(string cubeId, string dimensionId)
            {
                this.cubeId = cubeId;
                this.dimensionId = dimensionId;
            }

            public string CubeId
            {
                get { return cubeId; }
            }

            public string DimensionId
            {
                get { return dimensionId;  }
            }
        }

        protected OlapDBWrapper()
        { }

        protected OlapDBWrapper(IScheme scheme, IOlapDatabase olapDatabase)
        {
            this.scheme = scheme;
            this.olapDatabase = olapDatabase;
            GenerateDataSetSchema();

            ValidateBatchesInDatabase();
        }

        internal IOlapDatabase OlapDatabase
        {
            get { return olapDatabase; }
        }
        
        /// <summary>
        /// Проверяет наличие продключения к базе данных, 
        /// в случаи потери соединения восстанавливает его.
        /// </summary>
        protected abstract void CheckConnection();

        protected abstract Dictionary<string, IProcessableObjectInfo> GetCubes();
        protected abstract Dictionary<string, IProcessableObjectInfo> GetDimensions();
        protected abstract List<CubeDimensionLink> GetCubeDimensionLinkList();

        /// <summary>
        /// Проверяет существование куба или измерения с заданным идентификатором.
        /// </summary>
        /// <param name="objectType">Тип объекта (куб или измерение).</param>
        /// <param name="objectId">Идентификатор объекта.</param>
        /// <returns>True, если объект существует.</returns>
        protected abstract bool ObjectExist(OlapObjectType objectType, string objectId);
        
        /// <summary>
        /// Проверяет существование группы мер или раздела с заданным идентификатором в кубе.
        /// </summary>
        /// <param name="objectType">Тип объекта, существование которого хотим проверить.</param>
        /// <param name="objectId">Идентификатор объекта, сущесвование которого хотим проверить.</param>
        /// <param name="parentId">Идентификатор родительского объекта.</param>
        /// <returns></returns>
        protected abstract bool ObjectExist(OlapObjectType objectType, string objectId, string parentId);

        private DataTable dataTableOlapObjects;        
        private DataTable dataTableCubeDimensionLink;
        private DataTable dataTableBatch;

        private void GenerateDataSetSchema()
        {
            dataSetOlapBase.Reset();
            dataSetOlapBase.RemotingFormat = SerializationFormat.Binary;
            dataSetOlapBase.BeginInit();
            try
            {
                dataTableOlapObjects = dataSetOlapBase.Tables.Add("OlapObjects");
                dataTableCubeDimensionLink = dataSetOlapBase.Tables.Add("CubeDimensionLinks");
                dataTableBatch = dataSetOlapBase.Tables.Add("Batch");
                //dataTableOlapObjects = new DataTable("OlapObjects");
                //dataTableCubeDimensionLink = new DataTable("CubeDimensionLinks");
                //dataTableBatch = new DataTable("Batch");

                InitObjectTable(dataTableOlapObjects);                
                InitLinkTable(dataTableCubeDimensionLink);
                InitBatchTable(dataTableBatch);
                
				InitKeys();
                InitRelations();

            	InitDatabaseErrorsTable(dataTableErrors);
			}
            finally
            {
                dataSetOlapBase.EndInit();
            }            
        }

        private void ClearOlapObjects()
        {
            dataTableCubeDimensionLink.Clear();
            dataTableOlapObjects.Clear();

            if (null != cubes)
            {
                cubes.Clear();
                cubes = null;
            }

            if (null != dimensions)
            {
                dimensions.Clear();
                dimensions = null;
            }            

            partitions.Clear();
        }

        public abstract string DatabaseId
        {
            get;
        }

        public abstract string DatabaseName
        {
            get;
        }

        #region получение OlapObjectsDataUpdater

        private DataUpdater GetOlapObjectsDataUpdater(Database db)
        {
            IDbDataAdapter adapter = db.GetDataAdapter();
            adapter.SelectCommand = db.Connection.CreateCommand();
            adapter.SelectCommand.CommandText = sqlSelectOlapObjects;

            adapter.InsertCommand = db.Connection.CreateCommand();
            AppendInsertOlapObjectsCommand(db, adapter.InsertCommand);
            adapter.InsertCommand.CommandText = db.GetQuery(sqlInsertOlapObjects, adapter.InsertCommand.Parameters);

            adapter.UpdateCommand = db.Connection.CreateCommand();
            AppendUpdateOlapObjectsCommand(db, adapter.UpdateCommand);
            adapter.UpdateCommand.CommandText = db.GetQuery(sqlUpdateOlapObjects, adapter.UpdateCommand.Parameters);

            // команда удаления данных
            adapter.DeleteCommand = db.Connection.CreateCommand();
            IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            adapter.DeleteCommand.Parameters.Add(prm);
            adapter.DeleteCommand.CommandText = db.GetQuery(sqlDeleteOlapObjects, adapter.DeleteCommand.Parameters);

            DataUpdater upd = new DataUpdater(adapter, null, db);
            upd.Transaction = db.Transaction;
            return upd;

            /*

            DataUpdater du;

            DbProviderFactory factory = DbProviderFactories.GetFactory(scheme.SchemeDWH.FactoryName);
            DbDataAdapter da = (DbDataAdapter)db.GetDataAdapter();
            DbCommandBuilder cb = factory.CreateCommandBuilder();

            da.SelectCommand = (DbCommand)db.Connection.CreateCommand();
            da.SelectCommand.CommandText = sqlSelectOlapObjects;

            cb.DataAdapter = da;
            DbCommand insertCommand = cb.GetInsertCommand();

            DbCommand updateCommand = cb.GetUpdateCommand();
            string query = updateCommand.CommandText;
            int idxFirstAND = query.IndexOf(") AND (");
            int idxID = query.IndexOf("(ID = ");
            string prmName = query.Substring(idxID + 6, idxFirstAND - idxID - 6).Replace(":", string.Empty);
            updateCommand.CommandText = query.Substring(0, idxFirstAND) + "))";
            List<IDbDataParameter> newParams = new List<IDbDataParameter>();
            foreach (IDbDataParameter prm in updateCommand.Parameters)
            {
                newParams.Add(prm);
                if (string.Compare(prm.ParameterName.Replace(":", string.Empty), prmName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    break;
                }
            }
            updateCommand.Parameters.Clear();
            foreach (IDbDataParameter param in newParams)
            {
                updateCommand.Parameters.Add(param);
            }

            DbCommand deleteCommand = cb.GetDeleteCommand();
            query = deleteCommand.CommandText;
            idxFirstAND = query.IndexOf(") AND (");
            idxID = query.IndexOf("(ID = :");
            prmName = query.Substring(idxID + 6, idxFirstAND - idxID - 6).Replace(":", string.Empty); ;
            deleteCommand.CommandText = query.Substring(0, idxFirstAND) + "))";
            newParams = new List<IDbDataParameter>();
            foreach (IDbDataParameter prm in deleteCommand.Parameters)
            {
                newParams.Add(prm);
                if (string.Compare(prm.ParameterName.Replace(":", string.Empty), prmName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    break;
                }
            }
            deleteCommand.Parameters.Clear();
            foreach (IDbDataParameter param in newParams)
            {
                deleteCommand.Parameters.Add(param);
            }

            //cb.RefreshSchema();

            da.InsertCommand = insertCommand;
            da.UpdateCommand = updateCommand;
            da.DeleteCommand = deleteCommand;

            du = new DataUpdater(da, null, factory, true);

            return du;
             * */
        }

        internal static void AppendInsertOlapObjectsCommand(Database db, IDbCommand command)
        {
            IDbDataParameter prm;
            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            command.Parameters.Add(prm);
            // Name
            prm = db.CreateParameter("OBJECTTYPE", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "OBJECTTYPE";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("OBJECTID", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "OBJECTID";
            command.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("OBJECTNAME", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "OBJECTNAME";
            command.Parameters.Add(prm);
            // название файла
            prm = db.CreateParameter("PARENTID", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "PARENTID";
            command.Parameters.Add(prm);
            // ссылка на родителя
            prm = db.CreateParameter("PARENTNAME", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "PARENTNAME";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("USED", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "USED";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("NEEDPROCESS", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "NEEDPROCESS";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("STATE", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "STATE";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("LASTPROCESSED", DataAttributeTypes.dtDateTime, 10);
            prm.SourceColumn = "LASTPROCESSED";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("REFBATCHID", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "REFBATCHID";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("PROCESSTYPE", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "PROCESSTYPE";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("PROCESSRESULT", DataAttributeTypes.dtString, 2000);
            prm.SourceColumn = "PROCESSRESULT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("SYNCHRONIZED", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "SYNCHRONIZED";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("FullName", DataAttributeTypes.dtString, 64);
            prm.SourceColumn = "FullName";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("ObjectKey", DataAttributeTypes.dtString, 36);
            prm.SourceColumn = "ObjectKey";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("REVISION", DataAttributeTypes.dtString, 10);
            prm.SourceColumn = "REVISION";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("batchoperations", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "batchoperations";
            command.Parameters.Add(prm);
        }

        internal static void AppendUpdateOlapObjectsCommand(Database db, IDbCommand command)
        {
            IDbDataParameter prm;
            // Name
            prm = db.CreateParameter("OBJECTTYPE", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "OBJECTTYPE";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("OBJECTID", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "OBJECTID";
            command.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("OBJECTNAME", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "OBJECTNAME";
            command.Parameters.Add(prm);
            // название файла
            prm = db.CreateParameter("PARENTID", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "PARENTID";
            command.Parameters.Add(prm);
            // ссылка на родителя
            prm = db.CreateParameter("PARENTNAME", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "PARENTNAME";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("USED", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "USED";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("NEEDPROCESS", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "NEEDPROCESS";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("STATE", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "STATE";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("LASTPROCESSED", DataAttributeTypes.dtDateTime, 10);
            prm.SourceColumn = "LASTPROCESSED";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("REFBATCHID", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "REFBATCHID";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("PROCESSTYPE", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "PROCESSTYPE";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("PROCESSRESULT", DataAttributeTypes.dtString, 2000);
            prm.SourceColumn = "PROCESSRESULT";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("SYNCHRONIZED", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "SYNCHRONIZED";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("FullName", DataAttributeTypes.dtString, 64);
            prm.SourceColumn = "FullName";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("ObjectKey", DataAttributeTypes.dtString, 36);
            prm.SourceColumn = "ObjectKey";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("REVISION", DataAttributeTypes.dtString, 10);
            prm.SourceColumn = "REVISION";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("batchoperations", DataAttributeTypes.dtString, 255);
            prm.SourceColumn = "batchoperations";
            command.Parameters.Add(prm);

            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            command.Parameters.Add(prm);
        }

        #endregion

        #region BatchDataUpdater

        private DataUpdater GetBatchDataUpdater(Database db)
        {
            IDbDataAdapter adapter = db.GetDataAdapter();
            adapter.SelectCommand = db.Connection.CreateCommand();
            adapter.SelectCommand.CommandText = sqlSelectBatch;

            adapter.InsertCommand = db.Connection.CreateCommand();
            AppendInsertBatchCommand(db, adapter.InsertCommand);
            adapter.InsertCommand.CommandText = db.GetQuery(sqlInsertBatch, adapter.InsertCommand.Parameters);

            adapter.UpdateCommand = db.Connection.CreateCommand();
            AppendUpdateBatchCommand(db, adapter.UpdateCommand);
            adapter.UpdateCommand.CommandText = db.GetQuery(sqlUpdateBatch, adapter.UpdateCommand.Parameters);

            // команда удаления данных
            adapter.DeleteCommand = db.Connection.CreateCommand();
            IDbDataParameter prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            adapter.DeleteCommand.Parameters.Add(prm);
            adapter.DeleteCommand.CommandText = db.GetQuery(sqlDeleteBatch, adapter.DeleteCommand.Parameters);

            DataUpdater upd = new DataUpdater(adapter, null, db);
            upd.Transaction = db.Transaction;
            return upd;

            /*
            DataUpdater du;

            DbProviderFactory factory = DbProviderFactories.GetFactory(scheme.SchemeDWH.FactoryName);
            DbDataAdapter da = (DbDataAdapter)db.GetDataAdapter();
            DbCommandBuilder cb = factory.CreateCommandBuilder();

            da.SelectCommand = (DbCommand)db.Connection.CreateCommand();
            da.SelectCommand.CommandText = sqlSelectBatch;

            cb.DataAdapter = da;
            DbCommand insertCommand = cb.GetInsertCommand();

            DbCommand updateCommand = cb.GetUpdateCommand();
            string query = updateCommand.CommandText;
            int idxFirstAND = query.IndexOf(") AND (");
            int idxID = query.IndexOf("(ID = :");
            string prmName = query.Substring(idxID + 6, idxFirstAND - idxID - 6).Replace(":", string.Empty);
            updateCommand.CommandText = query.Substring(0, idxFirstAND) + "))";
            List<IDbDataParameter> newParams = new List<IDbDataParameter>();
            foreach (IDbDataParameter prm in updateCommand.Parameters)
            {
                newParams.Add(prm);
                if (string.Compare(prm.ParameterName.Replace(":", string.Empty), prmName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    break;
                }
            }
            updateCommand.Parameters.Clear();
            foreach (IDbDataParameter param in newParams)
            {
                updateCommand.Parameters.Add(param);
            }

            DbCommand deleteCommand = cb.GetDeleteCommand();
            query = deleteCommand.CommandText;
            idxFirstAND = query.IndexOf(") AND (");
            idxID = query.IndexOf("(ID = :");
            prmName = query.Substring(idxID + 6, idxFirstAND - idxID - 6).Replace(":", string.Empty);
            deleteCommand.CommandText = query.Substring(0, idxFirstAND) + "))";
            newParams = new List<IDbDataParameter>();
            foreach (IDbDataParameter prm in deleteCommand.Parameters)
            {
                newParams.Add(prm);
                if (string.Compare(prm.ParameterName.Replace(":", string.Empty), prmName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    break;
                }
            }
            deleteCommand.Parameters.Clear();
            foreach (IDbDataParameter param in newParams)
            {
                deleteCommand.Parameters.Add(param);
            }

            //cb.RefreshSchema();

            da.InsertCommand = insertCommand;
            da.UpdateCommand = updateCommand;
            da.DeleteCommand = deleteCommand;

            du = new DataUpdater(da, null, factory, true);

            return du;
             * */
        }

        internal static void AppendInsertBatchCommand(Database db, IDbCommand command)
        {
            IDbDataParameter prm;
            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            command.Parameters.Add(prm);
            // Name
            prm = db.CreateParameter("REFUSER", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "REFUSER";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("ADDITIONTIME", DataAttributeTypes.dtDateTime, 10);
            prm.SourceColumn = "ADDITIONTIME";
            command.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("BATCHSTATE", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "BATCHSTATE";
            command.Parameters.Add(prm);
            // название файла
            prm = db.CreateParameter("SESSIONID", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "SESSIONID";
            command.Parameters.Add(prm);
            // ссылка на родителя
            prm = db.CreateParameter("BATCHID", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "BATCHID";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("PRIORITY", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "PRIORITY";
            command.Parameters.Add(prm);
        }

        internal static void AppendUpdateBatchCommand(Database db, IDbCommand command)
        {
            IDbDataParameter prm;
            // Name
            prm = db.CreateParameter("REFUSER", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "REFUSER";
            command.Parameters.Add(prm);
            // Caption
            prm = db.CreateParameter("ADDITIONTIME", DataAttributeTypes.dtDateTime, 10);
            prm.SourceColumn = "ADDITIONTIME";
            command.Parameters.Add(prm);
            // Description
            prm = db.CreateParameter("BATCHSTATE", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "BATCHSTATE";
            command.Parameters.Add(prm);
            // название файла
            prm = db.CreateParameter("SESSIONID", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "SESSIONID";
            command.Parameters.Add(prm);
            // ссылка на родителя
            prm = db.CreateParameter("BATCHID", DataAttributeTypes.dtString, 132);
            prm.SourceColumn = "BATCHID";
            command.Parameters.Add(prm);

            prm = db.CreateParameter("PRIORITY", DataAttributeTypes.dtInteger, 1);
            prm.SourceColumn = "PRIORITY";
            command.Parameters.Add(prm);
            // ID
            prm = db.CreateParameter("ID", DataAttributeTypes.dtInteger, 10);
            prm.SourceColumn = "ID";
            command.Parameters.Add(prm);
        }

        #endregion

        /// <summary>
        /// Выполняет проверку пакетов находящихся в незавершенном состоянии.
        /// </summary>
        /// <remarks>
        /// Если пакет находится в состоянии выполнения, то переводит его в состояние "Выполнен с ошибками".
        /// Если пакет в состоянии создания, то такой пакет просто удаляется.
        /// </remarks>
        public void ValidateBatchesInDatabase()
        {
            IMDProcessingProtocol protocol = (IMDProcessingProtocol)this.scheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);
            Database db = (Database)scheme.SchemeDWH.DB;
            DataUpdater updaterOlapObjects = GetOlapObjectsDataUpdater(db);
            DataUpdater updaterBatch = GetBatchDataUpdater(db);//(DataUpdater)db.GetDataUpdater(sqlSelectBatch);
            dataTableOlapObjects.BeginLoadData();
            try
            {
                DataTable dtOlapObjects = new DataTable();
                DataTable dtBatches = new DataTable();
                
                //Загружем записи из хранилища.
                updaterOlapObjects.Fill(ref dtOlapObjects);
                updaterBatch.Fill(ref dtBatches);

                // Удаляем пакеты находящиеся в состоянии "Создан"
                foreach (DataRow batchRow in dtBatches.Select(String.Format("BatchState = {0}", (int)BatchState.Created)))
                {
                    foreach (DataRow olapObjectRow in dtOlapObjects.Select(String.Format("RefBatchId = '{0}'", batchRow["BatchId"])))
                    {
                        olapObjectRow["RefBatchId"] = DBNull.Value;
                    }

                    batchRow.Delete();
                }

                // Выполняющиеся пакеты переводим в состояние "Выполнен с ошибкой"
                foreach (DataRow batchRow in dtBatches.Select(String.Format("BatchState = {0}", (int)BatchState.Running)))
                {
                    foreach (DataRow olapObjectRow in dtOlapObjects.Select(String.Format("RefBatchId = '{0}'", batchRow["BatchId"])))
                    {
                        olapObjectRow["RefBatchId"] = DBNull.Value;
                    }
                    
                    batchRow["BatchState"] = (int)BatchState.ComplitedWithError;
                    
                    protocol.WriteEventIntoMDProcessingProtocol(
                        "Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeWarning,
                        String.Format("Обработка пакета была прервана вследствии незапланированного останова службы"),
                        Convert.ToString(batchRow["BatchId"]),
                        Convert.ToString(batchRow["BatchId"]),
                        OlapObjectType.Database,
                        Convert.ToString(batchRow["BatchId"]));
                }

                // Пакеты, находящиеся в состоянии "Ожидание" и "Отложен" переводим в состояние "Удален"
                foreach (DataRow batchRow in dtBatches.Select(String.Format("BatchState IN ({0}, {1})", (int)BatchState.Waiting, (int)BatchState.Canceled)))
                {
                    foreach (DataRow olapObjectRow in dtOlapObjects.Select(String.Format("RefBatchId = '{0}'", batchRow["BatchId"])))
                    {
                        olapObjectRow["RefBatchId"] = DBNull.Value;
                    }

                    batchRow["BatchState"] = (int)BatchState.Deleted;

                    protocol.WriteEventIntoMDProcessingProtocol(
                        "Krista.FM.Server.OLAP.Processor",
                        MDProcessingEventKind.mdpeWarning,
                        String.Format("Пакет был удален вследствии останова службы."),
                        Convert.ToString(batchRow["BatchId"]),
                        Convert.ToString(batchRow["BatchId"]),
                        OlapObjectType.Database,
                        Convert.ToString(batchRow["BatchId"]));
                }

                // Для всех объектов очищаем ссылки на пакеты
                foreach (DataRow olapObjectRow in dtOlapObjects.Select("RefBatchId is not null"))
                {
                    olapObjectRow["RefBatchId"] = DBNull.Value;
                }

                //Сохраняем изменения в хранилище.
                DataTable changes = dtOlapObjects.GetChanges();
                if (changes != null)
                    updaterOlapObjects.Update(ref changes);
                changes = dtBatches.GetChanges();
                if (changes != null)
                    updaterBatch.Update(ref changes);

                //Принимаем изменения.
                dtOlapObjects.AcceptChanges();
                dtBatches.AcceptChanges();
            }
            finally
            {
                DisposeDB(ref db);
                DisposeUpdater(ref updaterOlapObjects);
                DisposeUpdater(ref updaterBatch);
                protocol.Dispose();
            }
        }

        private void FillOlapObjects()
        {

            // Если в режиме мульти-сервер, то ничего не делаем
            if (scheme.MultiServerMode)
            {
                return;
            }

            DateTime startTime = DateTime.Now;
            Trace.TraceInformation("Запущено обновление OlapObjects: {0}", startTime);
            mutexOlapObjects.WaitOne();
            Database kristaDB = (Database)scheme.SchemeDWH.DB;
            DataUpdater updaterOlapObjects = GetOlapObjectsDataUpdater(kristaDB);
            dataTableOlapObjects.BeginLoadData();
            try
            {
                //Загружем записи из хранилища.
                updaterOlapObjects.Fill(ref dataTableOlapObjects);

                //Добавляем запись самой многомерной базы данных.
                AddOlapObject("Database", Guid.NewGuid().ToString(), OlapObjectType.Database, DatabaseId, DatabaseName, string.Empty, string.Empty,
                    AnalysisState.Unprocessed, SqlDateTime.MinValue.Value, string.Empty, string.Empty);

                cubes = GetCubes();
                foreach (KeyValuePair<string, IProcessableObjectInfo> cube in cubes)
                {
                    //Получаем разделы куба.
                    Dictionary<string, IProcessableObjectInfo> currentCubePartitions = GetCubePartitions(cube.Value.ObjectID);

                    foreach (KeyValuePair<string, IProcessableObjectInfo> item in currentCubePartitions)
                    {
                        //Добавляем разделы куба в общий список разделов.
                        partitions.Add(item.Key, item.Value);

                        //DataRow[] rows = dataTableOlapObjects.Select(String.Format("ObjectId = '{0}'", item.Value.MeasureGroupId));
                        //if (rows.GetLength(0) == 0) 
                        if (null == dataTableOlapObjects.Rows.Find(item.Value.MeasureGroupId))
                        {
                            //Создаем объект для группы мер.
                            ProcessableObjectInfo measureGroupInfo = new ProcessableObjectInfo(
                                item.Value.FullName, item.Value.ObjectKey,
                                OlapObjectType.MeasureGroup, DatabaseId, DatabaseName,
                                item.Value.CubeId, item.Value.CubeName,
                                null, null,
                                item.Value.MeasureGroupId, item.Value.MeasureGroupName, ProcessType.ProcessFull);
                            //Добавляем группу мер в таблицу "OlapObjects".
                            AddProcessableObjectInfo(measureGroupInfo);
                        }


                        //Добавляем раздел куба в таблицу "OlapObjects".
                        AddProcessableObjectInfo(item.Value);
                        cube.Value.FullName = item.Value.FullName;
                        cube.Value.Revision = item.Value.Revision;
                        cube.Value.BatchOperations = item.Value.BatchOperations;
                    }

                    //Теперь добавляем сам куб в таблицу "OlapObjects".
                    AddProcessableObjectInfo(cube.Value);
                }
                dimensions = GetDimensions();
                AddProcessableObjectInfoList(dimensions);

                //Мы добавили в хранилище новые объекты многомерной базы,
                //а также пометили как синхронизированные уже существующие записи.
                //Теперь пройдемся по всем объектам, помеченным как синхронизированные и убедимся,
                //что они действительно существуют в многомерной базе.
                //Если объект не существует - снимем признак синхронизированности.
                foreach (DataRow dataRow in dataTableOlapObjects.Select("Synchronized = 'true'"))
                {
                    OlapObjectType objectType = (OlapObjectType)dataRow[columnObjectType];
                    switch (objectType)
                    {
                        case OlapObjectType.Partition:
                            dataRow[columnSynchronized] = partitions.ContainsKey((string)dataRow[columnObjectId]);
                            //dataRow[columnSynchronized] = ObjectExist(objectType, (string)dataRow[columnObjectId]);
                            break;
                        case OlapObjectType.Dimension:
                            dataRow[columnSynchronized] = dimensions.ContainsKey((string)dataRow[columnObjectId]);
                            //dataRow[columnSynchronized] = ObjectExist(objectType, (string)dataRow[columnObjectId]);
                            break;
                        default:
                            break;
                    }
                }

                // Несоглассованные объекты теперь можно удалить
                foreach (DataRow dataRow in dataTableOlapObjects.Select("Synchronized <> 'true'"))
                    dataTableOlapObjects.Rows.Remove(dataRow);

                AddCubeDimensionLinkList(dataTableCubeDimensionLink, GetCubeDimensionLinkList());

                InitialFillOlapObjectsCalcColumns(ref dataTableOlapObjects);

                //Сохраняем изменения в хранилище.
                DataTable dtChanges = dataTableOlapObjects.GetChanges();
                if (dtChanges != null)
                    updaterOlapObjects.Update(ref dtChanges);

                //Принимаем изменения.
                dataTableOlapObjects.AcceptChanges();
                dataTableCubeDimensionLink.AcceptChanges();

                Trace.TraceVerbose("Время обновления OlapObjects: {0}", DateTime.Now - startTime);
            }
            finally
            {
                mutexOlapObjects.ReleaseMutex();
                DisposeDBAndUpdater(ref kristaDB, ref updaterOlapObjects);
                dataTableOlapObjects.EndLoadData();
            }
        }

        private void FillBatch()
        {
            // Если в режиме мульти-сервер, то ничего не делаем
            if (scheme.MultiServerMode)
            {
               return;
            }

            dataTableBatch.RemotingFormat = SerializationFormat.Binary;
            dataTableBatch.BeginLoadData();
            mutexBatch.WaitOne();
            try
            {
				using (Database kristaDB = (Database)scheme.SchemeDWH.DB)
				using (DataUpdater updaterBatch = (DataUpdater)kristaDB.GetDataUpdater(sqlSelectBatch))
				{
					updaterBatch.Fill(ref dataTableBatch);
				}
                FillBatchLookupNames(ref dataTableBatch);
                dataTableBatch.AcceptChanges();
            }
            finally
            {
                mutexBatch.ReleaseMutex();
                dataTableBatch.EndLoadData();
            }            
        }

        private void FillBatchLookupNames(ref DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                // Имена пользователей
                if (!(row[column_Batch_RefUser] is DBNull))
                {
                    row[column_Batch_UserName] = scheme.UsersManager.GetUserNameByID((int)row[column_Batch_RefUser]);
                }

                // Русские имена состояний пакета
                if (!(row[column_Batch_BatchState] is DBNull))
                {
                    row[column_Batch_BatchStateName] = ProcessorEnumsConverter.GetBatchStateName((BatchState)row[column_Batch_BatchState]);
                }

                // Русские имена приоритетов пакета
                if (!(row[column_Batch_Priority] is DBNull))
                {
                    row[column_Batch_PriorityName] = ProcessorEnumsConverter.GetBatchPriorityName((BatchStartPriority)row[column_Batch_Priority]);
                }
            }
        }

        /// <summary>
        /// Заполниет вычисляемые поля в строке.
        /// </summary>
        /// <param name="row">Обрабатываемая строка.</param>
        private static void FillCalculatedColumns(DataRow row)
        {
            if (row[columnState] != null)
            {
                row[columnStateName] = GetStateName((AnalysisState)row[columnState]);
            }
        }

        /// <summary>
        /// Первичное заполнение вычисляемых полей.
        /// </summary>
        /// <param name="table">Обрабатываемая таблица.</param>
        private static void InitialFillOlapObjectsCalcColumns(ref DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                FillCalculatedColumns(row);
                row[columnRecordStatus] = RecordStatus.Waiting;
            }
        }

        private static void InitObjectTable(DataTable dataTable)
        {
            dataTable.RemotingFormat = SerializationFormat.Binary;
            columnID = dataTable.Columns.Add("ID", typeof (int));
            columnObjectType = dataTable.Columns.Add("ObjectType", typeof(OlapObjectType));
            columnObjectId = dataTable.Columns.Add("ObjectId", typeof(string));
            columnObjectName = dataTable.Columns.Add("ObjectName", typeof(string));
            columnParentId = dataTable.Columns.Add("ParentId", typeof(string));
            columnParentName = dataTable.Columns.Add("ParentName", typeof(string));

            columnUsed = dataTable.Columns.Add("Used", typeof(bool));
            columnUsed.DefaultValue = true;
            columnNeedProcess = dataTable.Columns.Add("NeedProcess", typeof(bool));
            columnNeedProcess.DefaultValue = false;
            
            columnState = dataTable.Columns.Add("State", typeof(AnalysisState));            
            columnLastProcessed = dataTable.Columns.Add("LastProcessed", typeof(DateTime));
            columnLastProcessed.AllowDBNull = true;
            columnRefBatchID = dataTable.Columns.Add("RefBatchId", typeof(string));
            columnRefBatchID.AllowDBNull = true;

            //columnIsActual = dataTable.Columns.Add("IsActual", typeof(bool));
            //columnIsActual.DefaultValue = true;            
            
            columnProcessType = dataTable.Columns.Add("ProcessType", typeof(ProcessType));
            //columnProcessType.DefaultValue = ProcessType.ProcessFull;
            columnProcessType.AllowDBNull = true;

            columnProcessResult = dataTable.Columns.Add("ProcessResult", typeof(string));
            columnProcessResult.AllowDBNull = true;
            columnSynchronized = dataTable.Columns.Add("Synchronized", typeof(bool));
            columnSynchronized.DefaultValue = true;
            columnFullName = dataTable.Columns.Add("FullName", typeof(string));
            columnFullName.DefaultValue = true;
            columnObjectKey = dataTable.Columns.Add("ObjectKey", typeof(string));
            columnObjectKey.DefaultValue = true;
            columnRevision = dataTable.Columns.Add("Revision", typeof(string));
            columnRevision.DefaultValue = true;
            columnBatchOperations = dataTable.Columns.Add("BatchOperations", typeof(string));
            columnBatchOperations.DefaultValue = true;

            // Поля, отсутствующие в хранилище, но присутствующие в датасете для удобства работы.

            // Разыменовка состояния объекта.
            columnStateName = dataTable.Columns.Add("StateName", typeof(string));
            // Поле для отметки того что запись выбрана пользователем в интерфейсе.
            columnSelected = dataTable.Columns.Add("Selected", typeof(bool));
            columnSelected.DefaultValue = false;
            // Признак состояния записи - свободна, в пакете, рассчитывается.
            columnRecordStatus = dataTable.Columns.Add("RecordStatus", typeof(RecordStatus));
            columnRecordStatus.AllowDBNull = true;
        }

        private static void InitLinkTable(DataTable dataTable)
        {
            dataTable.RemotingFormat = SerializationFormat.Binary;
            dataTable.Columns.Add("RefCube", typeof(string));
            dataTable.Columns.Add("RefDimension", typeof(string));
        }
        
        private static void InitBatchTable(DataTable dataTable)
        {
            dataTable.RemotingFormat = SerializationFormat.Binary;
            column_Batch_Id = dataTable.Columns.Add("ID", typeof(int));
            column_Batch_RefUser = dataTable.Columns.Add("RefUser", typeof(int));
            column_Batch_AdditionTime = dataTable.Columns.Add("AdditionTime", typeof(DateTime));
            column_Batch_BatchState = dataTable.Columns.Add("BatchState", typeof(BatchState));
            column_Batch_SessionId = dataTable.Columns.Add("SessionId", typeof(string));
            column_Batch_BatchId = dataTable.Columns.Add("BatchId", typeof(string));
            column_Batch_Priority = dataTable.Columns.Add("Priority", typeof(BatchStartPriority));

            column_Batch_UserName = dataTable.Columns.Add("UserName", typeof(string));
            column_Batch_BatchStateName = dataTable.Columns.Add("BatchStateName", typeof(string));
            column_Batch_PriorityName = dataTable.Columns.Add("PriorityName", typeof(string));
        }

		private static void InitDatabaseErrorsTable(DataTable dataTable)
		{
			dataTable.RemotingFormat = SerializationFormat.Binary;
			column_DatabaseErrors_Error = dataTable.Columns.Add("Error", typeof(string));
			column_DatabaseErrors_ObjectType = dataTable.Columns.Add("ObjectType", typeof(OlapObjectType));
			column_DatabaseErrors_ObjectId = dataTable.Columns.Add("ObjectId", typeof(string));
			column_DatabaseErrors_ObjectName = dataTable.Columns.Add("ObjectName", typeof(string));
		}

        private static void AddCubeDimensionLink(DataTable dataTable, CubeDimensionLink item)
        {
            dataTable.Rows.Add(
                item.CubeId,
                item.DimensionId
                );
        }

        private static void AddCubeDimensionLinkList(DataTable dataTable, List<CubeDimensionLink> items)
        {
            dataTable.BeginLoadData();
            try
            {
                foreach (CubeDimensionLink item in items)
                {
                    AddCubeDimensionLink(dataTable, item);
                }
            }
            finally
            {
                dataTable.EndLoadData();
            }
        }
            
        /// <summary>
        /// Добавляет запись в таблицу OlapObjects
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="objectType"></param>
        /// <param name="objectId"></param>
        /// <param name="objectName"></param>
        /// <param name="parentId"></param>
        /// <param name="parentName"></param>
        /// <param name="analysisState"></param>
        /// <param name="lastProcessed"></param>
        private void AddOlapObject(string fullName, string objectKey, OlapObjectType objectType, string objectId, string objectName, string parentId,
            string parentName, AnalysisState analysisState, DateTime lastProcessed, string revision, string batchOperations)
        {
            DataRow dataRow = GetOlapObjectsRow(objectId);
            
            //Если записи с таким идентификаторм еще нет - то добавляем ее.
            if (null == dataRow)
            {
                using (Database db = (Database)scheme.SchemeDWH.DB)
                {
                    dataRow = dataTableOlapObjects.NewRow();
                    dataRow[columnID] = NextObjectID(db);
                    dataRow[columnFullName] = fullName;
                    dataRow[columnObjectKey] = objectKey;
                    dataRow[columnObjectType] = objectType;
                    dataRow[columnObjectId] = objectId;
                    dataRow[columnObjectName] = objectName;
                    dataRow[columnParentId] = parentId;
                    dataRow[columnParentName] = parentName;
                    dataRow[columnState] = analysisState;
                    //dataRow[columnStateName] = GetStateName(analysisState);
                    dataRow[columnLastProcessed] = lastProcessed;
                    dataRow[columnRefBatchID] = DBNull.Value;
                    dataRow[columnRecordStatus] = RecordStatus.Waiting;
                    dataRow[columnProcessType] = ProcessType.ProcessFull;
                    dataRow[columnRevision] = revision;
                    dataRow[columnBatchOperations] = batchOperations;
                    FillCalculatedColumns(dataRow);
                    //if (analysisState != AnalysisState.Unprocessed)
                    //{
                    //    dataRow[columnProcessResult] = String.Format("Рассчитан {0}", lastProcessed);
                    //}
                    dataTableOlapObjects.Rows.Add(dataRow);
                }
            }
            //Если объект уже существует в многомерной базе - помечаем его как синхронизированный.
            else
            {
                dataRow[columnParentName] = parentName;
                dataRow[columnSynchronized] = true;
                dataRow[columnObjectName] = objectName;
                dataRow[columnFullName] = fullName;
                dataRow[columnObjectKey] = objectKey;
                dataRow[columnState] = analysisState;
                dataRow[columnRevision] = revision;
                dataRow[columnBatchOperations] = batchOperations;
                //dataRow[columnStateName] = GetStateName(analysisState);
                dataRow[columnLastProcessed] = lastProcessed;
                if (analysisState != AnalysisState.Unprocessed)
                {
                    //dataRow[columnProcessResult] = String.Format("Рассчитан {0}", lastProcessed);
                }
                else
                {
                    if (Convert.ToString(dataRow[columnProcessResult]).StartsWith("Рассчитан"))
                    {
                        dataRow[columnProcessResult] = String.Empty;
                    }
                }
                FillCalculatedColumns(dataRow);
            }
        }

        /// <summary>
        /// Добавляет запись в таблицу Batch.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="batchGuid"></param>
        /// <param name="refUser"></param>
        /// <param name="additionTime"></param>
        /// <param name="batchState"></param>
        /// <param name="sessionId"></param>
        /// <param name="priority"></param>
        private void AddBatch(int id, Guid batchGuid, int refUser, DateTime additionTime, BatchState batchState, string sessionId, BatchStartPriority priority)
        {
            DataRow dataRow = dataTableBatch.Rows.Find(batchGuid);
            if (null == dataRow)
            {
                dataRow = dataTableBatch.NewRow();
                dataRow[column_Batch_Id] = id;
                dataRow[column_Batch_BatchId] = batchGuid.ToString();
                dataRow[column_Batch_RefUser] = refUser;
                dataRow[column_Batch_UserName] = scheme.UsersManager.GetUserNameByID(refUser);
                dataRow[column_Batch_AdditionTime] = additionTime;
                dataRow[column_Batch_BatchState] = batchState;
                dataRow[column_Batch_BatchStateName] = ProcessorEnumsConverter.GetBatchStateName(batchState);
                dataRow[column_Batch_SessionId] = sessionId;
                dataRow[column_Batch_Priority] = priority;
                dataRow[column_Batch_PriorityName] = ProcessorEnumsConverter.GetBatchPriorityName(priority);
                dataTableBatch.Rows.Add(dataRow);
            }
        }

        private void AddProcessableObjectInfo(IProcessableObjectInfo item)
        {
            switch (item.ObjectType)
            {
                case OlapObjectType.Database:
                    break;
                case OlapObjectType.Cube:
                    AddOlapObject(item.FullName, item.ObjectKey, item.ObjectType, item.ObjectID, item.ObjectName, item.DatabaseId, item.DatabaseName,
                        item.State, item.LastProcessed, item.Revision, item.BatchOperations);                    
                    break;                
                case OlapObjectType.MeasureGroup:
                    AddOlapObject(item.FullName, item.ObjectKey, item.ObjectType, item.ObjectID, item.ObjectName, item.CubeId, item.CubeName,
                        item.State, item.LastProcessed, string.Empty, string.Empty);                    
                    break;
                case OlapObjectType.Partition:
                    ////Если AS 2000, то раздел подчиняется напрямую кубу, иначе, раздел подчиняется группе мер.
                    //if (item.CubeId.Equals(item.MeasureGroupId, StringComparison.OrdinalIgnoreCase))
                    //{
                    //    AddOlapObject(item.ObjectType, item.ObjectID, item.ObjectName, item.CubeId, item.CubeName,
                    //        item.State, item.LastProcessed);
                    //}
                    //else
                    //{
                    //    AddOlapObject(item.ObjectType, item.ObjectID, item.ObjectName, item.MeasureGroupId, item.MeasureGroupName,
                    //        item.State, item.LastProcessed);
                    //}
                    AddOlapObject(item.FullName, item.ObjectKey, item.ObjectType, item.ObjectID, item.ObjectName, item.MeasureGroupId, item.MeasureGroupName,
                            item.State, item.LastProcessed, string.Empty, item.BatchOperations);
                    break;
                case OlapObjectType.Dimension:
                    AddOlapObject(item.FullName, item.ObjectKey, item.ObjectType, item.ObjectID, item.ObjectName, item.DatabaseId, item.DatabaseName,
                        item.State, item.LastProcessed, string.Empty, string.Empty);
                    break;
                default:
                    break;
            }            
        }

        /// <summary>
        /// Добавляет в указанную таблицу список объектов.
        /// </summary>
        /// <param name="items">Список объектов, которые необходимо добавить.</param>
        /// <returns>True, если все добавляемые объекты были актуальны, False в противном случае.</returns>
        private bool AddProcessableObjectInfoList(Dictionary<string, IProcessableObjectInfo> items)
        {
            bool isActual = true;
            foreach (KeyValuePair<string, IProcessableObjectInfo> item in items)
            {                
                AddProcessableObjectInfo(item.Value);
            }
            return isActual;
        }        
        
        private static string GetStateName(AnalysisState analysisState)
        {
            switch (analysisState)
            {
                case AnalysisState.PartiallyProcessed:
                    return "Частично рассчитан";                    
                case AnalysisState.Processed:
                    return "Рассчитан";                    
                default:
                case AnalysisState.Unprocessed:
                    return "Не рассчитан";                    
            }
        }

        private void InitKeys()
        {
            dataTableOlapObjects.PrimaryKey = new DataColumn[] { columnObjectId };
            dataTableCubeDimensionLink.PrimaryKey =
                new DataColumn[] {
                    dataTableCubeDimensionLink.Columns["RefCube"],
                    dataTableCubeDimensionLink.Columns["RefDimension"],
                };
            dataTableBatch.PrimaryKey = new DataColumn[] { column_Batch_BatchId };
        }

        private void InitRelations()
        {
            if (dataSetOlapBase != null)
            {
                dataSetOlapBase.Relations.Add("Parent_Child", columnObjectId, columnParentId, false);

                dataSetOlapBase.Relations.Add("Cubes_CubeDimensionLinks", columnObjectId,
                    dataSetOlapBase.Tables["CubeDimensionLinks"].Columns["RefCube"], false);

                dataSetOlapBase.Relations.Add("Dimensions_CubeDimensionLinks", columnObjectId,
                    dataSetOlapBase.Tables["CubeDimensionLinks"].Columns["RefDimension"], false);

                dataSetOlapBase.Relations.Add("Batch_OlapObjects", column_Batch_BatchId, columnRefBatchID, false);
            }            
        }

        /// <summary>
        /// Обновление серверного кеша.
        /// </summary>
        public void RefreshOlapObjects()
        {
            RefreshOlapObjects(RefreshOlapDataSetOptions.OnlyIfDatasetIsEmpty);
        }

        /// <summary>
        /// Обновление серверного кеша.
        /// </summary>
        /// <param name="refreshOptions">Режим обновления.</param>
        public void RefreshOlapObjects(RefreshOlapDataSetOptions refreshOptions)
        {
            olapDatabase.CheckConnection();

            switch (refreshOptions)
            {
                default:
                case RefreshOlapDataSetOptions.OnlyIfDatasetIsEmpty:
                    //if (dataSetOlapBase.Tables["OlapObjects"].Rows.Count == 0)
                    if (dataTableOlapObjects.Rows.Count == 0)
                    {
                        ClearOlapObjects();
                        FillOlapObjects();
                    }
                    break;
                case RefreshOlapDataSetOptions.Always:
                    ClearOlapObjects();
                    FillOlapObjects();
                    break;
            }            
        }

        protected void FillDataSet()
        {
            RefreshOlapObjects();
            FillBatch();
        }
        
        //public DataSet DataSetOlapBase
        //{
        //    [System.Diagnostics.DebuggerStepThrough()]
        //    get { return dataSetOlapBase; }
        //}

        public DataTable Partitions
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return dataTableOlapObjects; }
        }

        public DataTable GetPartitions(string filter)
        {
            DataTable clone;
            try
            {
                mutexOlapObjects.WaitOne();
                clone = dataTableOlapObjects.Clone();
                DataRow[] rows = dataTableOlapObjects.Select(filter);
                foreach (DataRow row in rows)
                {
                    clone.LoadDataRow(row.ItemArray, LoadOption.OverwriteChanges);
                }
            }
            finally
            {
                mutexOlapObjects.ReleaseMutex();
            }
            return clone;
        }

        public DataTable BatchesView
        {
            [System.Diagnostics.DebuggerStepThrough]
            get
            {
                DataTable clone;
                try
                {
                    mutexBatch.WaitOne();
                    clone = dataTableBatch.Copy();
                }
                finally
                {
                    mutexBatch.ReleaseMutex();
                }
                return clone;
            }
        }

        /// <summary>
        /// Возвращает количество пакетов в очереди на обработку.
        /// </summary>
        public int BatchQueueCount 
        {
            get
            {
                return dataTableBatch.Select(String.Format(
                    "BatchState = {0} or BatchState = {1} or BatchState = {2}",
                    (int)BatchState.Waiting, (int)BatchState.Running, (int)BatchState.Canceled)).GetLength(0);
            }
        }

        internal ProcessableObjectInfo GetCubeByPartitionId(string partitionId)
        {
            DataRow partitionRow = GetOlapObjectsRow(partitionId);
            DataRow measureGroupDataRow = partitionRow.GetParentRow(RelationName_ParentChild);
            DataRow cubeDataRow;
            if ((OlapObjectType)measureGroupDataRow[columnObjectType] == OlapObjectType.MeasureGroup)
            {
                //Если родитель - группа мер, то вытаскиваем еще и куб.
                cubeDataRow = measureGroupDataRow.GetParentRow(RelationName_ParentChild);
            }
            else
            {
                //Если родитель куб - то и делать больше ничего не надо.
                cubeDataRow = measureGroupDataRow;
            }
            return new ProcessableObjectInfo(
                Convert.ToString(cubeDataRow[columnFullName]),
                Convert.ToString(cubeDataRow[columnObjectKey]),
                (OlapObjectType)cubeDataRow[columnObjectType], 
                DatabaseId, 
                DatabaseName,
                Convert.ToString(cubeDataRow[columnObjectId]),
                Convert.ToString(cubeDataRow[columnObjectName]),
                (ProcessType)cubeDataRow[columnProcessType],
                (AnalysisState)cubeDataRow[columnState],
                (DateTime)cubeDataRow[columnLastProcessed],
                (RecordStatus)cubeDataRow[columnRecordStatus],
                Convert.ToString(cubeDataRow[columnProcessResult]),
                Convert.ToBoolean(cubeDataRow[columnNeedProcess]));
        }

        /// <summary>
        /// Конструирует объект типа ProcessableObjectInfo на основе записи таблицы OlapObjects
        /// </summary>
        /// <param name="olapObjectsDataRow"></param>
        /// <returns></returns>
        internal ProcessableObjectInfo GetProcessableObjectFromDataRow(DataRow olapObjectsDataRow)
        {
            OlapObjectType objectType = (OlapObjectType)olapObjectsDataRow[columnObjectType];
            switch (objectType)
            {
                case OlapObjectType.Database:                
                case OlapObjectType.MeasureGroup:
                    break;
                case OlapObjectType.Partition:
                    DataRow cubeDataRow;
                    DataRow measureGroupDataRow = olapObjectsDataRow.GetParentRow(RelationName_ParentChild);                    
                    if ((OlapObjectType)measureGroupDataRow[columnObjectType] == OlapObjectType.MeasureGroup)
                    {
                        //Если родитель - группа мер, то вытаскиваем еще и куб.
                        cubeDataRow = measureGroupDataRow.GetParentRow(RelationName_ParentChild);
                    }
                    else
                    {
                        //Если родитель куб - то и делать больше ничего не надо.
                        cubeDataRow = measureGroupDataRow;
                    }
                    return new ProcessableObjectInfo(
                        Convert.ToString(olapObjectsDataRow[columnFullName]),
                        Convert.ToString(olapObjectsDataRow[columnObjectKey]),
                        (OlapObjectType)olapObjectsDataRow[columnObjectType], 
                        DatabaseId, 
                        DatabaseName,
                        Convert.ToString(cubeDataRow[columnObjectId]), 
                        Convert.ToString(cubeDataRow[columnObjectName]),
                        Convert.ToString(measureGroupDataRow[columnObjectId]), 
                        Convert.ToString(measureGroupDataRow[columnObjectName]),
                        Convert.ToString(olapObjectsDataRow[columnObjectId]), 
                        Convert.ToString(olapObjectsDataRow[columnObjectName]), 
                        (ProcessType)olapObjectsDataRow[columnProcessType], 
                        (AnalysisState)olapObjectsDataRow[columnState],
                        olapObjectsDataRow[columnLastProcessed] is DBNull ? SqlDateTime.MinValue.Value : (DateTime)olapObjectsDataRow[columnLastProcessed],
                        (RecordStatus)olapObjectsDataRow[columnRecordStatus], 
                        olapObjectsDataRow[columnProcessResult] is DBNull ? String.Empty : Convert.ToString(olapObjectsDataRow[columnProcessResult]),
                        Convert.ToBoolean(olapObjectsDataRow[columnNeedProcess]),
                        Convert.ToString(olapObjectsDataRow[columnBatchOperations]));
                case OlapObjectType.Cube:
                case OlapObjectType.Dimension:
                    return new ProcessableObjectInfo(
                        Convert.ToString(olapObjectsDataRow[columnFullName]),
                        Convert.ToString(olapObjectsDataRow[columnObjectKey]),
                        (OlapObjectType)olapObjectsDataRow[columnObjectType], 
                        DatabaseId, 
                        DatabaseName,
                        Convert.ToString(olapObjectsDataRow[columnObjectId]), 
                        Convert.ToString(olapObjectsDataRow[columnObjectName]), 
                        (ProcessType)olapObjectsDataRow[columnProcessType],
                        (AnalysisState)olapObjectsDataRow[columnState],
                        olapObjectsDataRow[columnLastProcessed] is DBNull ? SqlDateTime.MinValue.Value : (DateTime)olapObjectsDataRow[columnLastProcessed],
                        (RecordStatus)olapObjectsDataRow[columnRecordStatus],
                        olapObjectsDataRow[columnProcessResult] is DBNull ? String.Empty : Convert.ToString(olapObjectsDataRow[columnProcessResult]),
                        Convert.ToBoolean(olapObjectsDataRow[columnNeedProcess]));
                default:
                    break;
            }
            return null;            
        }

        internal IEnumerable<IProcessableObjectInfo> GetProcessableObjectFromDataRows(IEnumerable<DataRow> rows)
        {
            List<IProcessableObjectInfo> items = new List<IProcessableObjectInfo>();
            foreach (DataRow dataRow in rows)
	        {
                items.Add(GetProcessableObjectFromDataRow(dataRow));
	        }
            return items;
        }

        protected IProcessableObjectInfo GetProcessableObjectInfo(OlapObjectType objectType, string name, DataTable dataTable)
        {
            DataRow[] rows = dataTable.Select(
                string.Format("ObjectType = {0} and ObjectName = '{1}'", (int)objectType, name), "ObjectName");            
            if (rows.Length > 0)
            {   
                return GetProcessableObjectFromDataRow(rows[0]);
            }
            return null;
        }

        public IProcessableObjectInfo GetProcessableObjectInfo(OlapObjectType objectType, string objectId, string parentId)
        {
            DataRow dataRow = GetDataRow(objectId);
            if (null != dataRow)
            {                
                return GetProcessableObjectFromDataRow(dataRow);
            }
            return null;
        }

        public IProcessableObjectInfo GetProcessableObjectInfo(OlapObjectType objectType, string objectId)
        {
            return GetProcessableObjectInfo(objectType, objectId, DatabaseId);
        }
        
        /// <summary>
        /// Возвращает объект в списке с заданным ID
        /// </summary>
        /// <param name="objectList">Список объектов.</param>
        /// <param name="id">Идентификатор для поиска</param>
        /// <returns></returns>
        protected static IProcessableObjectInfo GetById(List<IProcessableObjectInfo> objectList, string id)
        {
            foreach (IProcessableObjectInfo processableObjectInfo in objectList)
            {
                if (processableObjectInfo.ObjectID.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    return processableObjectInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// Возвращает объект в списке с заданным именем.
        /// </summary>
        /// <param name="objectList">Список объектов.</param>
        /// <param name="name">Имя для поиска.</param>
        /// <returns></returns>
        protected static IProcessableObjectInfo GetByName(Dictionary<string, IProcessableObjectInfo> objectList, string name)
        {
            foreach (KeyValuePair<string, IProcessableObjectInfo> item in objectList)
            {
                if (item.Value.ObjectName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return item.Value;
                }
            }
            return null;
        }

        public abstract Dictionary<string, IProcessableObjectInfo> GetCubePartitions(string cubeId);

        public abstract Dictionary<string, IProcessableObjectInfo> GetCubeDimensions(string cubeId);

        public abstract Dictionary<string, IProcessableObjectInfo> GetCubeDimensions(string cubeId, Dictionary<string, IProcessableObjectInfo> dimensionList);

        public virtual Dictionary<string, IProcessableObjectInfo> GetCubeDimensions(List<string> cubeIdList)
        {
            Dictionary<string, IProcessableObjectInfo> dimensionList = new Dictionary<string, IProcessableObjectInfo>();
            foreach (string cubeId in cubeIdList)
            {
                dimensionList = GetCubeDimensions(cubeId, dimensionList);
            }
            return dimensionList;
        }

        public virtual void InvalidatePartition(string cubeId, string measureGroupId, string objectId)
        {
            DataRow dataRow = GetDataRow(objectId);
            SetActual(dataRow, false);
        }

        /// <summary>
        /// Получает состояние объекта (расчитан\не расчитан) из многомерной базы.
        /// </summary>
        /// <param name="objectType">Тип объекта (куб, раздел куба, измерение).</param>
        /// <param name="cubeId">Идентификатор куба.</param>
        /// <param name="measureGroupId">Идентификатор группы мер.</param>
        /// <param name="objectId">Идентификатор объекта.</param>
        /// <param name="lastProcessed">Время последнего расчета.</param>
        /// <returns>Состояние объекта (расчитан\не расчитан).</returns>
        protected abstract AnalysisState GetState(
            OlapObjectType objectType, string cubeId, string measureGroupId, string objectId, out DateTime lastProcessed);

        protected DataTable GetTable(OlapObjectType objectType)
        {
            return dataTableOlapObjects;            
        }        

        protected DataRow[] SelectDataRows(OlapObjectType objectType, string cubeId, string measureGroupId, string objectId)
        {
            DataTable dataTable = GetTable(objectType);
            if (null != dataTable)
            {
                return dataTable.Select(string.Format("CubeId='{0}', MeasureGroupId='{1}', ObjectId='{2}'",
                    cubeId, measureGroupId, objectId));
            }
            return new DataRow[] { };
        }

        protected DataRow GetDataRow(string objectId)
        {            
            return dataTableOlapObjects.Rows.Find(objectId);            
        }        

        /// <summary>
        /// Запрашивает состояние объекта из многомерной базы и обновляет его в Датасете.
        /// </summary>
        /// <param name="objectType">Тип объекта (куб, раздел куба, измерение).</param>
        /// <param name="cubeId">Идентификатор куба.</param>
        /// <param name="measureGroupId">Идентификатор группы мер.</param>
        /// <param name="objectId">Идентификатор объекта.</param>
        internal virtual void RefreshState(OlapObjectType objectType, string cubeId, string measureGroupId, string objectId)
        {            
        }

        /// <summary>
        /// Установка состояния многомерного объекта.
        /// </summary>
        /// <param name="dataRow">Запись объекта.</param>
        /// <param name="analysisState">Состояние.</param>
        /// <param name="lastProcessed">Дата расчета.</param>
        internal static void SetState(DataRow dataRow, AnalysisState analysisState, DateTime lastProcessed)
        {            
            if (null != dataRow)
            {
                dataRow["State"] = analysisState;
                //dataRow["StateName"] = GetStateName(analysisState);
                dataRow["LastProcessed"] = lastProcessed;
                FillCalculatedColumns(dataRow);
            }
        }

        private static void SetActual(DataRow dataRow, bool isActual)
        {
            if (null != dataRow)
            {
                dataRow["IsActual"] = isActual;                
            }
        }

        public void InvalidateCube(string cubeId)
        {
            Dictionary<string, IProcessableObjectInfo> partitionsList = GetCubePartitions(cubeId);
            foreach (KeyValuePair<string, IProcessableObjectInfo> item in partitionsList)
            {
                InvalidatePartition(cubeId, item.Value.MeasureGroupId, item.Value.ObjectID);
            }            
            DataRow dataRow = GetDataRow(cubeId);
            SetActual(dataRow, false);
        }        

        internal DataRow GetOlapObjectsRow(string objectId)
        {
            return dataTableOlapObjects.Rows.Find(objectId);
        }
        
        internal DataRow[] GetOlapObjectsRowsByBatchGuid(string batchGuid)
        {
            return dataTableOlapObjects.Select(String.Format("RefBatchID = '{0}'", batchGuid));
        }

        internal DataRow GetBatchRow(string batchGuid)
        {
            return dataTableBatch.Rows.Find(batchGuid);
        }

        internal BatchState GetBatchState(string batchGuid)
        {
            DataRow row = GetBatchRow(batchGuid);
            if (row != null)
            {
                return (BatchState) row[column_Batch_BatchState];
            }
            else
            {
                throw new ServerException(String.Format("Пакет с идентификатором \"{0}\" не найден.", batchGuid));
            }
        }

        /// <summary>
        /// Возвращает идентификатор куба по его имени.
        /// </summary>
        /// <returns></returns>
        public string GetCubeIdByName(string cubeName)
        {
            DataRow[] rows = dataTableOlapObjects.Select(
                string.Format("ObjectType = {0} and ObjectName = '{1}'", (int)OlapObjectType.Cube, cubeName));
            if (rows.Length > 0)
            {
                return (string)rows[0][columnObjectId];
            }
            Trace.TraceWarning("Не найдено ни одного куба с именем \"{0}\"", cubeName);
            return string.Empty;
        }

        /// <summary>
        /// Возвращает именя куба по его идентификатору.
        /// </summary>
        /// <returns></returns>
        public string GetCubeNameById(string cubeId)
        {
            DataRow[] rows = dataTableOlapObjects.Select(
                string.Format("ObjectType = {0} and ObjectId = '{1}'", (int)OlapObjectType.Cube, cubeId));
            if (rows.Length > 0)
            {
                return (string)rows[0][columnObjectName];
            }
            Trace.TraceWarning("Не найдено ни одного куба с ID = \"{0}\"", cubeId);
            return string.Empty;
        }

        /// <summary>
        /// Возвращает идентификатор секции куба по ее имени.
        /// </summary>
        /// <returns></returns>
        // TODO: По имени раздел куба получить нельзя - нужно еще имя куба!!!
        public string GetPartitionIdByName(string cubeName, string partitionName)
        {
            // TODO: Здесь сделано допущение, что ParentName (имя группы мер) соответствует имени куба
            DataRow[] rows = dataTableOlapObjects.Select(
                string.Format("ObjectType = '{0}' and ParentName = '{1}' and ObjectName = '{2}'",
                (int)OlapObjectType.Partition, cubeName, partitionName));
            if (rows.Length > 0)
            {
                return (string)rows[0][columnObjectId];
            }
            Trace.TraceWarning("Не найдено ни одной секции куба с именем \"{0}\\{1}\"", cubeName, partitionName);
            return string.Empty;
        }

        /// <summary>
        /// Возвращает идентификатор куба по его имени.
        /// </summary>
        /// <returns></returns>
        public string GetPartitionNameById(string partitionId)
        {
            DataRow[] rows = dataTableOlapObjects.Select(
                string.Format("ObjectType = '{0}' and ObjectID = '{1}'", (int)OlapObjectType.Partition, partitionId));
            if (rows.Length > 0)
            {
                return (string)rows[0][columnObjectName];
            }
            Trace.TraceWarning("Не найдено ни одной секции куба с ID = \"{0}\"", partitionId);
            return string.Empty;
        }

        /// <summary>
        /// Возвращает идентификатор измерения по его имени.
        /// </summary>
        /// <returns></returns>
        public string GetDimensionIdByName(string dimensionName)
        {
            DataRow[] rows = dataTableOlapObjects.Select(
                string.Format("ObjectType = '{0}' and ObjectName = '{1}'", (int)OlapObjectType.Dimension, dimensionName));

            if (rows.Length > 0)
            {
                return (string)rows[0][columnObjectId];
            }
            Trace.TraceWarning("Не найдено ни одного изменения с именем \"{0}\"", dimensionName);
            return string.Empty;
        }

        /// <summary>
        /// Возвращает список идентификаторов многомерных объектов по их FullName.
        /// </summary>
        /// <param name="olapObjectType">Тип объектов.</param>
        /// <param name="fullName">FullName объекта в схеме.</param>
        /// <returns>Список идентификаторов многомерных объектов.</returns>
        public List<string> GetOlapObjectsIdByFullName(OlapObjectType olapObjectType, string fullName)
        {
            List<string> objectsIdList = new List<string>();
            DataRow[] rows = dataTableOlapObjects.Select(String.Format(
                "ObjectType = '{0}' and FullName = '{1}'", (int)olapObjectType, fullName));
            if (rows.Length == 0)
            {
                Trace.TraceWarning("Не найдено ни одного объекта {0} c FullName равным \"{1}\"", olapObjectType, fullName);
            }

            foreach (DataRow row in rows)
            {
                objectsIdList.Add(Convert.ToString(row["ObjectId"]));
            }
            return objectsIdList;
        }

        /// <summary>
        /// Возвращает список идентификаторов многомерных объектов по их ObjectKey.
        /// </summary>
        /// <param name="olapObjectType">Тип объектов.</param>
        /// <returns>Список идентификаторов многомерных объектов.</returns>
        /// <param name="objectKey"></param>
        public List<string> GetOlapObjectsIdByObjectKey(OlapObjectType olapObjectType, string objectKey)
        {
            List<string> objectsIdList = new List<string>();
            DataRow[] rows = dataTableOlapObjects.Select(String.Format(
                "ObjectType = '{0}' and ObjectKey = '{1}'", (int)olapObjectType, objectKey));
            if (rows.Length == 0)
            {
                Trace.TraceWarning("Не найдено ни одного объекта {0} c ObjectKey равным \"{1}\"", olapObjectType, objectKey);
            }

            foreach (DataRow row in rows)
            {
                objectsIdList.Add(Convert.ToString(row["ObjectId"]));
            }
            return objectsIdList;
        }


        /// <summary>
        /// Возвращает список разделов куба.
        /// </summary>
        /// <param name="cubeId">Идентификатор куба</param>
        /// <returns>Список разделов куба.</returns>
        internal DataRow[] GetCubePartitionsRows(string cubeId)
        {
            List<DataRow> partitionsRows = new List<DataRow>();
            DataRow cubeRow = GetOlapObjectsRow(cubeId);
            if (null != cubeRow)
            {
                DataRow[] measureGroupRows = cubeRow.GetChildRows(RelationName_ParentChild);
                foreach (DataRow measureGroupRow in measureGroupRows)
                {
                    DataRow[] partitionRows = measureGroupRow.GetChildRows(RelationName_ParentChild);
                    foreach (DataRow partitionRow in partitionRows)
                    {
                        partitionsRows.Add(partitionRow);
                    }
                }
            }
            return partitionsRows.ToArray();
        }

        /// <summary>
        /// Возвращает список разделов куба.
        /// </summary>
        /// <param name="cubeId">Идентификатор куба</param>
        /// <returns>Список разделов куба.</returns>
        public Dictionary<string, IProcessableObjectInfo> DS_GetCubePartitions(string cubeId)
        {
            Dictionary<string, IProcessableObjectInfo> partitionList = new Dictionary<string, IProcessableObjectInfo>();
            foreach (DataRow partitionRow in GetCubePartitionsRows(cubeId))
            {
                IProcessableObjectInfo objectInfo = GetProcessableObjectFromDataRow(partitionRow);
                partitionList.Add(objectInfo.ObjectID, objectInfo);
            }                    
            return partitionList;
        }

        /// <summary>
        /// Возвращает список измерений куба.
        /// </summary>
        /// <param name="cubeId">Идентификатор куба.</param>
        /// <returns>Список измерений куба.</returns>
        //public Dictionary<string, IProcessableObjectInfo> DS_GetCubeDimensions(string cubeId)
        //{
        //    Dictionary<string, IProcessableObjectInfo> dimensionList = new Dictionary<string, IProcessableObjectInfo>();
        //    DataRow cubeRow = GetOlapObjectsRow(cubeId);
        //    if (null != cubeRow)
        //    {
        //        DataRow[] links = cubeRow.GetChildRows(RelationName_CubesCubeDimensionLinks);
        //        for (int i = 0; i < links.Length; i++)
        //        {
        //            DataRow dataRow = GetOlapObjectsRow((string)links[i]["RefDimension"]);
        //            if (null != dataRow)
        //            {
        //                IProcessableObjectInfo objectInfo = GetProcessableObjectFromDataRow(dataRow);
        //                dimensionList.Add(objectInfo.ObjectID, objectInfo);
        //            }
        //        }
        //    }
        //    return dimensionList;
        //}
        public Dictionary<string, IProcessableObjectInfo> DS_GetCubeDimensions(string cubeId)
        {
            Dictionary<string, IProcessableObjectInfo> dimensionList = new Dictionary<string, IProcessableObjectInfo>();
            DataRow cubeRow = GetOlapObjectsRow(cubeId);
            if (null != cubeRow)
            {
                //DataRow[] links = dataTableCubeDimensionLink.Select(String.Format("RefCube = '{0}'", cubeRow[columnObjectId]));
                DataRow[] links = cubeRow.GetChildRows(RelationName_CubesCubeDimensionLinks);
                for (int i = 0; i < links.Length; i++)
                {
                    DataRow dataRow = GetOlapObjectsRow((string)links[i]["RefDimension"]);
                    if (null != dataRow)
                    {
                        IProcessableObjectInfo objectInfo = GetProcessableObjectFromDataRow(dataRow);
                        dimensionList.Add(objectInfo.ObjectID, objectInfo);
                    }
                }
            }
            return dimensionList;
        }        

        /// <summary>
        /// Возвращает список кубов, использующих данное измерение.
        /// </summary>
        /// <param name="dimensionId">Идентификатор измерения.</param>
        /// <returns>Список кубов.</returns>
        public Dictionary<string, IProcessableObjectInfo> DS_GetDimensionCubes(string dimensionId)
        {
            Dictionary<string, IProcessableObjectInfo> cubeList = new Dictionary<string, IProcessableObjectInfo>();
            DataRow dimensionRow = GetOlapObjectsRow(dimensionId);
            if (null != dimensionRow)
            {
                //DataRow[] links = dataTableCubeDimensionLink.Select(String.Format("RefDimension = '{0}'", dimensionRow[columnObjectId]));
                DataRow[] links = dimensionRow.GetChildRows(RelationName_DimensionsCubeDimensionLinks);
                foreach (DataRow dataRow in links)
                {
                    DataRow cube = GetOlapObjectsRow((string)dataRow["RefCube"]);
                    if (null != cube)
                    {
                        IProcessableObjectInfo objectInfo = GetProcessableObjectFromDataRow(cube);
                        cubeList.Add(objectInfo.ObjectID, objectInfo);                        
                    }
                }
            }
            return cubeList;
        }        

        public List<IProcessableObjectInfo> GetProcessableObjectList(List<DataRow> dataRowList)
        {
            List<IProcessableObjectInfo> processableObjectList = new List<IProcessableObjectInfo>(dataRowList.Count);
            foreach (DataRow dataRow in dataRowList)
            {
                processableObjectList.Add(GetProcessableObjectFromDataRow(dataRow));
            }
            return processableObjectList;
        }

        public void UpdateValues(ref DataTable changedValues)
        {
            if (changedValues != null && changedValues.Rows.Count > 0)
            {
                mutexOlapObjects.WaitOne();
                dataTableOlapObjects.BeginLoadData();
                try
                {
                    using (Database db = (Database)scheme.SchemeDWH.DB)
                    using (DataUpdater updater = GetOlapObjectsDataUpdater(db))
                    {
                        //Включаем изменения в основной датасет.
                        dataTableOlapObjects.Merge(changedValues);

                        //Сохраняем изменения в хранилище.
                        //DataTable changes = dataTableOlapObjects.GetChanges();
                        //foreach (DataRow row in changes.Rows)
                        //{
                        //    if (row.RowState == DataRowState.Modified)
                        //    {
                        //        object result = db.ExecQuery(
                        //            "UPDATE OLAPOBJECTS SET OBJECTTYPE = ?, OBJECTID = ?, OBJECTNAME = ?, PARENTID = ?, PARENTNAME = ?, USED = ?, NEEDPROCESS = ?, STATE = ?, LASTPROCESSED = ?, REFBATCHID = ?, PROCESSTYPE = ?, PROCESSRESULT = ?, SYNCHRONIZED = ? WHERE ID = ?",
                        //            QueryResultTypes.NonQuery,
                        //            db.CreateParameter("OBJECTTYPE", row["OBJECTTYPE"]),
                        //            db.CreateParameter("OBJECTID", row["OBJECTID"]),
                        //            db.CreateParameter("OBJECTNAME", row["OBJECTNAME"]),
                        //            db.CreateParameter("PARENTID", row["PARENTID"]),
                        //            db.CreateParameter("PARENTNAME", row["PARENTNAME"]),
                        //            db.CreateParameter("USED", row["USED"]),
                        //            db.CreateParameter("NEEDPROCESS", row["NEEDPROCESS"]),
                        //            db.CreateParameter("STATE", row["STATE"]),
                        //            db.CreateParameter("LASTPROCESSED", row["LASTPROCESSED"]),
                        //            db.CreateParameter("REFBATCHID", row["REFBATCHID"]),
                        //            db.CreateParameter("PROCESSTYPE", row["PROCESSTYPE"]),
                        //            db.CreateParameter("PROCESSRESULT", row["PROCESSRESULT"]),
                        //            db.CreateParameter("SYNCHRONIZED", row["SYNCHRONIZED"]),
                        //            db.CreateParameter("ID", row["ID"]));
                        //    }
                        //}

                        //Сохраняем изменения в хранилище.
                        DataTable dtChanges = dataTableOlapObjects.GetChanges();
                        if (dtChanges != null)
                            updater.Update(ref dtChanges);

                        //Принимаем изменения.
                        dataTableOlapObjects.AcceptChanges();
                    }
                }
                finally
                {
                    dataTableOlapObjects.EndLoadData();
                    mutexOlapObjects.ReleaseMutex();
                }
            }
        }

        public void UpdateValues(DataSet changedValues)
        {
            if (null != changedValues && null != changedValues.Tables["OlapObjects"] &&
                changedValues.Tables["OlapObjects"].Rows.Count > 0)
            {
                dataTableOlapObjects.BeginLoadData();
                Database kristaDB = (Database)scheme.SchemeDWH.DB;
                DataUpdater updater = GetOlapObjectsDataUpdater(kristaDB);
                mutexOlapObjects.WaitOne();
                try
                {
                    //Включаем изменения в основной датасет.
                    dataSetOlapBase.Merge(changedValues.Tables["OlapObjects"]);

                    DataSet ds = dataSetOlapBase.GetChanges();

                    //Сохраняем изменения в хранилище.
                    if (ds != null)
                        updater.Update(ref ds);

                    //Принимаем изменения.
                    dataSetOlapBase.AcceptChanges();
                }
                finally
                {
                    if (updater != null && !updater.Disposed)
                    {
                        updater.Dispose();
                    }
                    if (kristaDB != null && !kristaDB.Disposed)
                    {
                        kristaDB.Dispose();
                    }
                    mutexOlapObjects.ReleaseMutex();
                    dataTableOlapObjects.EndLoadData();
                }
            }
        }

        public void UpdateBatchValues(ref DataTable changedValues)
        {
            if (changedValues != null && changedValues.Rows.Count > 0)
            {
                mutexBatch.WaitOne();
                dataTableBatch.BeginLoadData();
                try
                {
                    using (Database db = (Database)scheme.SchemeDWH.DB)
                    using (DataUpdater updater = GetBatchDataUpdater(db))
                    {
                        // Включаем изменения в основной датасет.
                        dataTableBatch.Merge(changedValues);

                        // Для удаленных пакетов очищаем ссылки объектов на них.
                        foreach (DataRow row in changedValues.Rows)
                        {
                            if (((BatchState)row[column_Batch_BatchState.ColumnName, DataRowVersion.Current]) == BatchState.Deleted
                                && ((BatchState)row[column_Batch_BatchState.ColumnName, DataRowVersion.Original]) == BatchState.Waiting)
                            {
                                ClearBatchGuid(Convert.ToString(row[column_Batch_BatchId.ColumnName]));
                            }
                        }

                        FillBatchLookupNames(ref dataTableBatch);

                        // Сохраняем изменения в хранилище.
                        DataTable dtChanges = dataTableBatch;
                        if (dtChanges != null)
                            updater.Update(ref dtChanges);

                        // Принимаем изменения.
                        dataTableBatch.AcceptChanges();
                        
                        ProcessService.newRequestEvent.Set();
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("При обновлении пакетов произошла ошибка: {0}", e.Message);
                    dataTableBatch.RejectChanges();
                    throw new ServerException(e.Message, e);
                }
                finally
                {
                    dataTableBatch.EndLoadData();
                    mutexBatch.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// Если объект находится в пакете, то возвращает true.
        /// </summary>
        /// <param name="objectId">Идентификатор пакета.</param>
        /// <returns></returns>
        internal bool ObjectBlocked(string objectId)
        {
            return !string.IsNullOrEmpty(GetBatchGuid(objectId));
        }

        /// <summary>
        /// Возвращает идентификатор пакета для объекта с заданным id.
        /// </summary>
        /// <param name="objectId">Идентификатор объекта.</param>
        /// <returns>Идентификатор пакета.</returns>
        public string GetBatchGuid(string objectId)
        {
            DataRow dataRow = GetOlapObjectsRow(objectId);
            if (null != dataRow)
            {
                if (dataRow[columnRefBatchID] == DBNull.Value)
                {
                    return string.Empty;
                }                
                return (string)dataRow[columnRefBatchID];
            }
            return string.Empty;
        }



        /// <summary>
        /// Проставляет для заданных записей признак необходимости расчета.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="options"></param>
        /// <param name="fixedValue"></param>
        /// <returns></returns>
        public IEnumerable<IProcessableObjectInfo> SetNeedProcess(IProcessableObjectInfo item, SetNeedProcessOptions options, bool fixedValue)
        {
            List<IProcessableObjectInfo> items = new List<IProcessableObjectInfo>();
            items.Add(item);
            return SetNeedProcess(items, options, fixedValue);
        }

        /// <summary>
        /// Проставляет для заданных записей признак необходимости расчета.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="options"></param>
        /// <param name="fixedValue"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<IProcessableObjectInfo> SetNeedProcess(IEnumerable<IProcessableObjectInfo> items, SetNeedProcessOptions options, bool fixedValue)
        {
            List<IProcessableObjectInfo> usedList = new List<IProcessableObjectInfo>();

            Database db = (Database)scheme.SchemeDWH.DB;
            DataUpdater updaterOlapObjects = GetOlapObjectsDataUpdater(db);
            mutexOlapObjects.WaitOne();
            try
            {
                db.BeginTransaction();
				updaterOlapObjects.Transaction = db.Transaction;
                try
                {
                    foreach (IProcessableObjectInfo item in items)
                    {
                        DataRow dataRow = GetOlapObjectsRow(item.ObjectID);
                        if (null != dataRow)
                        {
                            bool needProcess = fixedValue;
                            if (options == SetNeedProcessOptions.Auto)
                            {
                                if (!Convert.ToBoolean(dataRow[columnNeedProcess]))
                                {
                                    needProcess = Convert.ToBoolean(dataRow[columnUsed]);
                                }
                            }

                            dataRow[columnNeedProcess] = needProcess;

                            if (Convert.ToBoolean(dataRow[columnUsed]) && needProcess)
                            {
                                usedList.Add(item);
                            }
                        }
                    }
                    DataTable dtChanges = dataTableOlapObjects.GetChanges();
                    if (dtChanges != null)
                        updaterOlapObjects.Update(ref dtChanges);
                    
                    //Завершаем транзакцию.
					db.Commit();
                    updaterOlapObjects.Transaction = null;

                    dataTableOlapObjects.AcceptChanges();

                    foreach (IProcessableObjectInfo item in usedList)
                    {
                        Trace.TraceVerbose("{0} Необходим расчет объекта: {1} \"{2}\"", Authentication.UserDate, item.ObjectType, item.ObjectName);
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    
					db.Rollback();
					updaterOlapObjects.Transaction = null;

                    dataTableOlapObjects.RejectChanges();
                    
                    throw new ServerException(e.Message, e);
                }
            }
            finally
            {
                mutexOlapObjects.ReleaseMutex();
				DisposeDBAndUpdater(ref db, ref updaterOlapObjects);
			}
            return usedList;
        }

        public void SetProcessType(IProcessableObjectInfo item, int processType)
        {
            Database db = (Database)scheme.SchemeDWH.DB;
            DataUpdater updaterOlapObjects = GetOlapObjectsDataUpdater(db);
            mutexOlapObjects.WaitOne();
            try
            {
                db.BeginTransaction();
                updaterOlapObjects.Transaction = db.Transaction;
                try
                {
                    DataRow dataRow = GetOlapObjectsRow(item.ObjectID);
                    if (null != dataRow)
                    {
                        dataRow[columnProcessType] = processType;
                    }
                    DataTable dtChanges = dataTableOlapObjects.GetChanges();
                    if (dtChanges != null)
                        updaterOlapObjects.Update(ref dtChanges);

                    //Завершаем транзакцию.
                    db.Commit();
                    updaterOlapObjects.Transaction = null;

                    dataTableOlapObjects.AcceptChanges();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);

                    db.Rollback();
                    updaterOlapObjects.Transaction = null;

                    dataTableOlapObjects.RejectChanges();

                    throw new ServerException(e.Message, e);
                }
            }
            finally
            {
                mutexOlapObjects.ReleaseMutex();
                DisposeDBAndUpdater(ref db, ref updaterOlapObjects);
            }
        }

        /// <summary>
        /// Сбрасывает состояние для всех секций кобов, в которые входит измерение dimensionId.
        /// </summary>
        /// <param name="dimensionId">Идентификатор измерения.</param>
        internal void ResetUsedPartitionsInDimension(string dimensionId)
        {
            DataRow row = GetOlapObjectsRow(dimensionId);
            DataRow[] rows = row.GetChildRows(RelationName_DimensionsCubeDimensionLinks);
            foreach (DataRow dataRow in rows)
            {
                foreach (DataRow partitionRow in GetCubePartitionsRows(Convert.ToString(dataRow["RefCube"])))
                {
                    SetState(partitionRow, AnalysisState.Unprocessed, SqlDateTime.MinValue.Value);
                    //partitionRow["NeedProcess"] = false;
                }
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        private static int NextBatchId(Database db)
        {
            return db.GetGenerator("g_Batch");
        }

        [System.Diagnostics.DebuggerStepThrough]
        private static int NextObjectID(Database db)
        {
            return db.GetGenerator("g_OlapObjects");
        }

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void ClearBatchGuid(string batchGuid)
        {
            if (!String.IsNullOrEmpty(batchGuid))
            {
                DataRow[] dataRows = GetOlapObjectsRowsByBatchGuid(batchGuid);
                if (dataRows.Length > 0)
                {
                    Database db = (Database)scheme.SchemeDWH.DB;
                    DataUpdater updater = GetOlapObjectsDataUpdater(db);
                    mutexOlapObjects.WaitOne();                    
                    try
                    {
                        db.BeginTransaction();
                        updater.Transaction = db.Transaction;

                        foreach (DataRow dataRow in dataRows)
                        {
                            dataRow[columnRefBatchID] = DBNull.Value;
                            dataRow[columnRecordStatus] = RecordStatus.Waiting;
                        }
                        DataTable dtChanges = dataTableOlapObjects.GetChanges();
                        if (dtChanges != null)
                            updater.Update(ref dtChanges);

                        db.Commit();

                        dataTableOlapObjects.AcceptChanges();

                        IMDProcessingProtocol protocol = null;
                        try
                        {
                            protocol = (IMDProcessingProtocol)this.scheme.GetProtocol(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name);
                            protocol.WriteEventIntoMDProcessingProtocol(
                                "Интерфейс кубов", MDProcessingEventKind.mdpeInformation,
                                "Пакет удален пользователем",
                                batchGuid, batchGuid, OlapObjectType.Cube, batchGuid);

                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("При записи в протокол произошла ошибка: {0}", e.Message);
                        }
                        finally
                        {
                            if (protocol != null)
                                protocol.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("При очистке ссылок на пакет произошла ошибка: {0}", e.Message);
                        db.Rollback();
                        dataTableOlapObjects.RejectChanges();
                        throw new ServerException(e.Message, e);
                    }
                    finally
                    {
                        mutexOlapObjects.ReleaseMutex();
                        DisposeDBAndUpdater(ref db, ref updater);
                    }
                }
            }
        }

        /// <summary>
        /// Создает новый пакет расчета объектов.
        /// </summary>
        /// <returns>ID пакета.</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		internal Guid CreateBatch()
        {
            using (Database db = (Database)scheme.SchemeDWH.DB)
            using (DataUpdater updaterBatch = (DataUpdater)db.GetDataUpdater(sqlSelectBatch))
            {
                mutexBatch.WaitOne();
                try
                {
                    int id = NextBatchId(db);
                    Guid batchGuid = Guid.NewGuid();

					db.BeginTransaction();
                    updaterBatch.Transaction = db.Transaction;

                    //Добавляем сам пакет.
                    AddBatch(id, batchGuid, (int)Authentication.UserID, DateTime.Now, BatchState.Created, null, BatchStartPriority.Auto);

                    DataTable changes = dataTableBatch.GetChanges();
                    if (changes != null)
                        updaterBatch.Update(ref changes);

                    //Завершаем транзакцию.
                    db.Commit();
					updaterBatch.Transaction = null;
                    dataTableBatch.AcceptChanges();
                    Trace.TraceVerbose("{0} Создан пакет: {{{1}}}", Authentication.UserDate, batchGuid);

                    return batchGuid;
                }
                catch (Exception e)
                {
                    Trace.TraceError("При создании пакета произошла ошибка: {0}", e);
                    
					db.Rollback();
					updaterBatch.Transaction = null;
					
					dataTableBatch.RejectChanges();

                    throw new ServerException(e.Message, e);
                }
                finally
                {
                    mutexBatch.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// У записей OlapObjects обновляет поля статуса и очищает идентификатор пакета.
        /// Удаляет пакет.
        /// </summary>
        /// <param name="batchGuid"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		internal void DeleteBatch(Guid batchGuid)
        {
            Database db = (Database)scheme.SchemeDWH.DB;
            DataUpdater updaterBatch = (DataUpdater)db.GetDataUpdater(sqlSelectBatch);
            DataUpdater updaterOlapObjects = GetOlapObjectsDataUpdater(db);
            mutexBatch.WaitOne();
            mutexOlapObjects.WaitOne();
            try
            {
                db.BeginTransaction();
                updaterBatch.Transaction = db.Transaction;
                updaterOlapObjects.Transaction = db.Transaction;
                try
                {
                    DataRow[] dataRows = GetOlapObjectsRowsByBatchGuid(batchGuid.ToString());
                    foreach (DataRow dataRow in dataRows)
                    {
                        dataRow[columnRefBatchID] = DBNull.Value;
                        //dataRow[columnRecordStatus] = item.RecordStatus;
                        //dataRow[columnState] = item.State;
                        //dataRow[columnStateName] = GetStateName(item.State);
                        //dataRow[columnLastProcessed] = item.LastProcessed;
                        //dataRow[columnProcessResult] = item.ProcessResult;
                        //if (item.RecordStatus == RecordStatus.ComplitedSuccessful)
                        //{
                        //    dataRow[columnNeedProcess] = false;
                        //}
                    }
                    DataTable dtChanges = dataTableOlapObjects.GetChanges();
                    if (dtChanges != null)
                        updaterOlapObjects.Update(ref dtChanges);

                    //Меняем статус у самого пакета.
                    DataRow batchRow = GetBatchRow(batchGuid.ToString());
                    batchRow.Delete();
                    dtChanges = dataTableBatch.GetChanges();
                    if (dtChanges != null)
                        updaterBatch.Update(ref dtChanges);

                    //Завершаем транзакцию.
                    db.Commit();

                    dataTableOlapObjects.AcceptChanges();
                    dataTableBatch.AcceptChanges();

                    Trace.TraceVerbose("{0} Пакет удален: {{{1}}}", Authentication.UserDate, batchGuid);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    updaterBatch.Transaction.Rollback();

                    dataTableOlapObjects.RejectChanges();
                    dataTableBatch.RejectChanges();

                    throw new ServerException(e.Message, e);
                }
            }
            finally
            {
                mutexOlapObjects.ReleaseMutex();
                mutexBatch.ReleaseMutex();
                DisposeDBAndUpdater(ref db, ref updaterOlapObjects, ref updaterBatch);
            }
        }

        internal void AddToBatch(string objectId, BatchStartPriority batchStartOptions, Guid batchGuid, BatchStartPriority priority)
        {
            List<string> objectIdList = new List<string>();
            objectIdList.Add(objectId);
            AddToBatch(objectIdList, batchStartOptions, batchGuid, priority);
        }

		[MethodImpl(MethodImplOptions.Synchronized)]
		internal void AddToBatch(IEnumerable<string> objectIdList, BatchStartPriority batchStartOptions, Guid batchGuid, BatchStartPriority priority)
        {
            bool batchIsEmpty = true;
            
            Database db = (Database)scheme.SchemeDWH.DB;
            DataUpdater updaterBatch = GetBatchDataUpdater(db);
            DataUpdater updaterOlapObjects = GetOlapObjectsDataUpdater(db);
            mutexBatch.WaitOne();
            mutexOlapObjects.WaitOne();
            try
            {
				db.BeginTransaction();
				updaterBatch.Transaction = db.Transaction;
				updaterOlapObjects.Transaction = db.Transaction;
                try
                {
                    foreach (string objectId in objectIdList)
                    {
                        if (!ObjectBlocked(objectId))
                        {
                            //Проставляем идентификатор пакета у записи.
                            DataRow dataRow = GetOlapObjectsRow(objectId);
                            dataRow[columnRefBatchID] = batchGuid;
                            dataRow[columnRecordStatus] = RecordStatus.InBatch;
                            batchIsEmpty = false;
                            Trace.TraceVerbose("{0} Объект \"{1}\" добавлен в пакет {{{2}}}", Authentication.UserDate, dataRow[columnObjectName], batchGuid);
                        }
                    }

                    DataRow batchRow = GetBatchRow(batchGuid.ToString());
                    if (priority == BatchStartPriority.Immediately)
                    {
                        batchRow[column_Batch_Priority] = priority;
                        batchRow[column_Batch_PriorityName] = ProcessorEnumsConverter.GetBatchPriorityName(priority);
                    }

                    if (batchIsEmpty)
                    {
                        //throw new Exception("Пакет пуст!");
                    }

                    //Сохраняем запись в таблице "OlapObjects".
                    DataTable dtChanges = dataTableOlapObjects.GetChanges();
                    if (dtChanges != null)
                        updaterOlapObjects.Update(ref dtChanges);

                    //Сохраняем пакет.
                    dtChanges = dataTableBatch.GetChanges();
                    if (dtChanges != null)
                        updaterBatch.Update(ref dtChanges);

                    //Завершаем транзакцию.
					db.Commit();
					updaterOlapObjects.Transaction = null;
                    updaterBatch.Transaction = null;

                    dataTableOlapObjects.AcceptChanges();
                    dataTableBatch.AcceptChanges();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    db.Rollback();
					updaterOlapObjects.Transaction = null;
					updaterBatch.Transaction = null;

                    dataTableOlapObjects.RejectChanges();
                    dataTableBatch.RejectChanges();

                    throw new ServerException(e.Message, e);
                }
            }
            finally
            {
                mutexOlapObjects.ReleaseMutex();
                mutexBatch.ReleaseMutex();
                DisposeDBAndUpdater(ref db, ref updaterOlapObjects, ref updaterBatch);
            }

            /*if (batchStartOptions == BatchStartPriority.Immediately &&
                batchCreated && batchGuid != Guid.Empty)
            {
                scheme.Processor.ProcessManager.StartBatch(batchGuid);
            }*/
        }

        /// <summary>
        /// Помещает пакет в очередь ожидания расчета.
        /// </summary>
        /// <param name="batchGuid"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<IProcessableObjectInfo> StartBatch(Guid batchGuid, string sessionId)
        {
            List<IProcessableObjectInfo> items = new List<IProcessableObjectInfo>();
            Database db = (Database)scheme.SchemeDWH.DB;
            DataUpdater updaterBatch = GetBatchDataUpdater(db);
            
            DataUpdater updaterOlapObjects = GetOlapObjectsDataUpdater(db);
            mutexBatch.WaitOne();
            mutexOlapObjects.WaitOne();
            try
            {
				db.BeginTransaction();
				updaterBatch.Transaction = db.Transaction;
                updaterOlapObjects.Transaction = db.Transaction;
                try
                {
                    //Меняем статус у записей в таблице OlapObjects.
                    DataRow[] dataRows = GetOlapObjectsRowsByBatchGuid(batchGuid.ToString());
                    foreach (DataRow dataRow in dataRows)
                    {
                        dataRow[columnRecordStatus] = RecordStatus.Waiting;
                        items.Add(GetProcessableObjectFromDataRow(dataRow));
                    }
                    DataTable dtChanges = dataTableOlapObjects.GetChanges();
                    if (dtChanges != null)
                        updaterOlapObjects.Update(ref dtChanges);

                    //Меняем статус у самого пакета.
                    DataRow batchRow = GetBatchRow(batchGuid.ToString());

                    // Если пакет пуст, то удаляем его
                    if (items.Count == 0)
                    {
                        batchRow.Delete();
                        Trace.TraceVerbose("{0} Пакет \"{1}\" удален, т.к. он пустой", Authentication.UserDate, batchGuid);
                    }
                    else
                    {
                        batchRow[column_Batch_BatchState] = BatchState.Waiting;
                        batchRow[column_Batch_BatchStateName] = ProcessorEnumsConverter.GetBatchStateName(BatchState.Waiting);
                        batchRow[column_Batch_SessionId] = sessionId;
                    }
                    dtChanges = dataTableBatch.GetChanges();
                    if (dtChanges != null)
                        updaterBatch.Update(ref dtChanges);

                    //Завершаем транзакцию.
					db.Commit();
					updaterOlapObjects.Transaction = null;
                    updaterBatch.Transaction = null;

                    dataTableOlapObjects.AcceptChanges();
                    dataTableBatch.AcceptChanges();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    db.Rollback();
					updaterOlapObjects.Transaction = null;
					updaterBatch.Transaction = null;

                    dataTableOlapObjects.RejectChanges();
                    dataTableBatch.RejectChanges();

                    throw new ServerException(e.Message, e);
                }
            }
            finally
            {
                mutexOlapObjects.ReleaseMutex();
                mutexBatch.ReleaseMutex();
                DisposeDBAndUpdater(ref db, ref updaterOlapObjects, ref updaterBatch);
            }
            return items;
        }

        /// <summary>
        /// Помечает пакет, что он в данный момент расчитывается.
        /// </summary>
        /// <param name="batchGuid"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<IProcessableObjectInfo> StartProcessBatch(Guid batchGuid, string sessionId)
        {
            Trace.TraceVerbose("{0} Пакет {{{1}}} взят на расчет", Authentication.UserDate, batchGuid);

            List<IProcessableObjectInfo> items = new List<IProcessableObjectInfo>();            
            Database db = (Database)scheme.SchemeDWH.DB;
            DataUpdater updaterBatch = GetBatchDataUpdater(db);
            DataUpdater updaterOlapObjects = GetOlapObjectsDataUpdater(db);
            mutexBatch.WaitOne();
            mutexOlapObjects.WaitOne();
            try
            {
				db.BeginTransaction();
				updaterBatch.Transaction = db.Transaction;
				updaterOlapObjects.Transaction = db.Transaction;
                try
                {
                    //Меняем статус у записей в таблице OlapObjects.
                    DataRow[] dataRows = GetOlapObjectsRowsByBatchGuid(batchGuid.ToString());
                    foreach (DataRow dataRow in dataRows)
                    {
                        dataRow[columnRecordStatus] = RecordStatus.InProcess;
                        items.Add(GetProcessableObjectFromDataRow(dataRow));
                    }
                    DataTable dtChanges = dataTableOlapObjects.GetChanges();
                    if (dtChanges != null)
                        updaterOlapObjects.Update(ref dtChanges);

                    //Меняем статус у самого пакета.
                    DataRow batchRow = GetBatchRow(batchGuid.ToString());
                    batchRow[column_Batch_BatchState] = BatchState.Running;
                    batchRow[column_Batch_BatchStateName] = ProcessorEnumsConverter.GetBatchStateName(BatchState.Running);
                    if (batchRow[column_Batch_SessionId] == DBNull.Value)
                    {
                        batchRow[column_Batch_SessionId] = sessionId;
                    }
                    dtChanges = dataTableBatch.GetChanges();
                    if (dtChanges != null)
                        updaterBatch.Update(ref dtChanges);

                    //Завершаем транзакцию.
                    db.Commit();
					updaterOlapObjects.Transaction = null;
					updaterBatch.Transaction = null;

                    dataTableOlapObjects.AcceptChanges();
                    dataTableBatch.AcceptChanges();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    db.Rollback();
					updaterOlapObjects.Transaction = null;
					updaterBatch.Transaction = null;

                    dataTableOlapObjects.RejectChanges();
                    dataTableBatch.RejectChanges();

                    throw new ServerException(e.Message, e);
                }
            }
            finally
            {
                mutexOlapObjects.ReleaseMutex();
                mutexBatch.ReleaseMutex();
                DisposeDBAndUpdater(ref db, ref updaterOlapObjects, ref updaterBatch);
            }            
            return items;
        }

        /// <summary>
        /// У записей OlapObjects обновляет поля статуса и очищает идентификатор пакета.
        /// У пакета меняет статус.
        /// </summary>
        /// <param name="batchGuid"></param>
        /// <param name="batchState"></param>
        /// <param name="items"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ComplitBatch(Guid batchGuid, BatchState batchState, IEnumerable<IProcessableObjectInfo> items)
        {
            Trace.TraceVerbose("{0} Расчет пакета {{{1}}} завершен", Authentication.UserDate, batchGuid);

            Database db = (Database)scheme.SchemeDWH.DB;
            DataUpdater updaterBatch = GetBatchDataUpdater(db);
            DataUpdater updaterOlapObjects = GetOlapObjectsDataUpdater(db);
            mutexBatch.WaitOne();
            mutexOlapObjects.WaitOne();
            try
            {
				db.BeginTransaction();
				updaterBatch.Transaction = db.Transaction;
				updaterOlapObjects.Transaction = db.Transaction;
                try
                {   
                    foreach (IProcessableObjectInfo item in items)
                    {
                        DataRow dataRow = GetOlapObjectsRow(item.ObjectID);
                        if (null != dataRow)
                        {   
                            if ((item.State == AnalysisState.Processed || item.State == AnalysisState.PartiallyProcessed) &&
                                string.IsNullOrEmpty(item.ProcessResult))
                            {
                                //item.ProcessResult = String.Format("Рассчитан {0}", item.LastProcessed);
                            }

                            dataRow[columnRefBatchID] = DBNull.Value;
                            dataRow[columnRecordStatus] = item.RecordStatus;
                            dataRow[columnState] = item.State;
                            //dataRow[columnStateName] = GetStateName(item.State);
                            dataRow[columnLastProcessed] = item.LastProcessed;
                            dataRow[columnProcessResult] = item.ProcessResult;
                            if (item.RecordStatus == RecordStatus.ComplitedSuccessful)
                            {
                                dataRow[columnNeedProcess] = false;
                            }
                            FillCalculatedColumns(dataRow);
                        }
                    }
                    DataTable dtChanges = dataTableOlapObjects.GetChanges();
                    if (dtChanges != null)
                        updaterOlapObjects.Update(ref dtChanges);

                    //Меняем статус у самого пакета.
                    DataRow batchRow = GetBatchRow(batchGuid.ToString());
                    batchRow[column_Batch_BatchState] = batchState;
                    batchRow[column_Batch_BatchStateName] = ProcessorEnumsConverter.GetBatchStateName(batchState);
                    
                    dtChanges = dataTableBatch.GetChanges();
                    if (dtChanges != null)
                        updaterBatch.Update(ref dtChanges);

                    //Завершаем транзакцию.
                    db.Commit();
					updaterOlapObjects.Transaction = null;
					updaterBatch.Transaction = null;

                    dataTableOlapObjects.AcceptChanges();
                    dataTableBatch.AcceptChanges();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    db.Rollback();
					updaterOlapObjects.Transaction = null;
					updaterBatch.Transaction = null;

                    dataTableOlapObjects.RejectChanges();
                    dataTableBatch.RejectChanges();

                    throw new ServerException(e.Message, e);
                }
            }
            finally
            {
                mutexOlapObjects.ReleaseMutex();
                mutexBatch.ReleaseMutex();
                DisposeDBAndUpdater(ref db, ref updaterOlapObjects, ref updaterBatch);
            }
        }

        #region Вспомогательные методы для работы с объектами Database и DataUpdater
        private static void DisposeDB(ref Database kristaDB)
        {
            if (null != kristaDB && !kristaDB.Disposed)
            {
                kristaDB.Dispose();
                kristaDB = null;
            }
        }

        private static void DisposeUpdater(ref DataUpdater updater)
        {
            if (null != updater && !updater.Disposed)
            {
                updater.Dispose();
                updater = null;
            }
        }

        private static void DisposeDBAndUpdater(ref Database kristaDB, ref DataUpdater updater1, ref DataUpdater updater2)
        {
            DisposeUpdater(ref updater1);
            DisposeUpdater(ref updater2);
            DisposeDB(ref kristaDB);
        }

        private static void DisposeDBAndUpdater(ref Database kristaDB, ref DataUpdater updater)
        {
            DisposeUpdater(ref updater);
            DisposeDB(ref kristaDB);
        }
        #endregion Вспомогательные методы для работы с объектами Database и DataUpdater

		#region Ошибки многомерной базы данных

		protected void RegisterDatabaseError(string errorText, string objectId, string objectName, OlapObjectType objectType)
		{
			DataRow row = dataTableErrors.NewRow();
			row[column_DatabaseErrors_Error] = errorText;
			row[column_DatabaseErrors_ObjectId] = objectId;
			row[column_DatabaseErrors_ObjectName] = objectName;
			row[column_DatabaseErrors_ObjectType] = objectType;
			dataTableErrors.Rows.Add(row);

			Trace.TraceError("{0}, {1}, {2}, {3}", errorText, objectName, objectType, objectId);
		}

		public int GetDatabaseErrorsCount()
		{
			return dataTableErrors.Rows.Count;
		}

		public DataTable GetDatabaseErrors()
		{
			return dataTableErrors;
		}

		#endregion Ошибки многомерной базы данных
	}
}