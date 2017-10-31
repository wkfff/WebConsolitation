using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;

using Microsoft.AnalysisServices;

using Krista.FM.Server.ProcessorLibrary;
using System.ComponentModel;


namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// Оболочка для нетипизированного датасета. Не умеет синхронизировать работу нескольких пользователей.
    /// Код выдран из типизированного датасета. Сам типизированный датасет использовать не удалось,
    /// т.к. для его использования необходимо иметь иписание типа и на клиенте и на сервере.
    /// </summary>
    class DataSetAccumulatorWrapper
    {
        private DataSet dsAccumulator = new DataSet("Accumulator");
        private AccumulatorDataTable accumulator;
        private BatchDataTable batch;
        private DataRelation relationBatch_Accumulator;

        public DataSetAccumulatorWrapper()
        {
            accumulator = new AccumulatorDataTable();
            dsAccumulator.Tables.Add(accumulator);
            batch = new BatchDataTable();
            dsAccumulator.Tables.Add(batch);
            relationBatch_Accumulator = new DataRelation("Batch_Accumulator",
                new DataColumn[] {batch.IdColumn},
                new DataColumn[] {accumulator.RefBatchColumn}, false);
            dsAccumulator.Relations.Add(relationBatch_Accumulator);
        }

        public class AccumulatorDataTable : DataTable
        {
            private DataColumn columnId;
            private DataColumn columnObjectType;
            private DataColumn columnDatabaseId;
            private DataColumn columnCubeId;
            private DataColumn columnMeasureGroupId;
            private DataColumn columnObjectId;
            private DataColumn columnObjectName;
            private DataColumn columnCubeName;
            private DataColumn columnProcessType;
            private DataColumn columnRefBatch;
            private DataColumn columnState;
            private DataColumn columnLastProcessed;
            private DataColumn columnRecordStatus;
            private DataColumn columnProcessReason;
            private DataColumn columnRefUser;
            private DataColumn columnAdditionTime;
            private DataColumn columnErrorMessage;
            private DataColumn columnOptimizationMember;
            private DataColumn columnUserName;            

            public AccumulatorDataTable()
            {
                this.TableName = "Accumulator";
                this.BeginInit();
                this.InitClass();                
                this.EndInit();
            }

            private void InitClass()
            {
                columnId = new System.Data.DataColumn("Id", typeof(int), null, System.Data.MappingType.Element);
                Columns.Add(this.columnId);
                columnObjectType = new System.Data.DataColumn("ObjectType", typeof(Krista.FM.Server.ProcessorLibrary.OlapObjectType), null, System.Data.MappingType.Element);
                Columns.Add(this.columnObjectType);
                columnDatabaseId = new System.Data.DataColumn("DatabaseId", typeof(string), null, System.Data.MappingType.Element);
                Columns.Add(this.columnDatabaseId);
                columnCubeId = new System.Data.DataColumn("CubeId", typeof(string), null, System.Data.MappingType.Element);
                Columns.Add(this.columnCubeId);
                columnMeasureGroupId = new System.Data.DataColumn("MeasureGroupId", typeof(string), null, System.Data.MappingType.Element);
                Columns.Add(this.columnMeasureGroupId);
                columnObjectId = new System.Data.DataColumn("ObjectId", typeof(string), null, System.Data.MappingType.Element);
                Columns.Add(this.columnObjectId);
                columnObjectName = new System.Data.DataColumn("ObjectName", typeof(string), null, System.Data.MappingType.Element);
                Columns.Add(this.columnObjectName);
                columnCubeName = new System.Data.DataColumn("CubeName", typeof(string), null, System.Data.MappingType.Element);
                Columns.Add(this.columnCubeName);
                columnProcessType = new System.Data.DataColumn("ProcessType", typeof(Microsoft.AnalysisServices.ProcessType), null, System.Data.MappingType.Element);
                Columns.Add(this.columnProcessType);
                columnRefBatch = new System.Data.DataColumn("RefBatch", typeof(int), null, System.Data.MappingType.Element);
                Columns.Add(this.columnRefBatch);
                columnState = new System.Data.DataColumn("State", typeof(Microsoft.AnalysisServices.AnalysisState), null, System.Data.MappingType.Element);
                Columns.Add(this.columnState);
                columnLastProcessed = new System.Data.DataColumn("LastProcessed", typeof(System.DateTime), null, System.Data.MappingType.Element);
                Columns.Add(this.columnLastProcessed);
                columnRecordStatus = new System.Data.DataColumn("RecordStatus", typeof(Krista.FM.Server.ProcessorLibrary.RecordStatus), null, System.Data.MappingType.Element);
                Columns.Add(this.columnRecordStatus);
                columnProcessReason = new System.Data.DataColumn("ProcessReason", typeof(Krista.FM.Server.ProcessorLibrary.InvalidateReason), null, System.Data.MappingType.Element);
                Columns.Add(this.columnProcessReason);
                columnRefUser = new System.Data.DataColumn("RefUser", typeof(int), null, System.Data.MappingType.Element);
                Columns.Add(this.columnRefUser);
                columnAdditionTime = new System.Data.DataColumn("AdditionTime", typeof(System.DateTime), null, System.Data.MappingType.Element);
                Columns.Add(this.columnAdditionTime);
                columnErrorMessage = new System.Data.DataColumn("ErrorMessage", typeof(string), null, System.Data.MappingType.Element);
                Columns.Add(this.columnErrorMessage);
                columnOptimizationMember = new System.Data.DataColumn("OptimizationMember", typeof(bool), null, System.Data.MappingType.Element);
                Columns.Add(this.columnOptimizationMember);
                columnUserName = new System.Data.DataColumn("UserName", typeof(string), null, System.Data.MappingType.Element);
                Columns.Add(this.columnUserName);
                Constraints.Add(new System.Data.UniqueConstraint("Constraint1", new System.Data.DataColumn[] {
                                this.columnId}, true));
                columnId.AllowDBNull = false;
                columnId.Unique = true;
                columnId.Caption = "Ключ";
                columnObjectType.AllowDBNull = false;
                columnObjectType.Caption = "Тип объекта";
                columnDatabaseId.AllowDBNull = false;
                columnDatabaseId.MaxLength = 132;
                columnCubeId.MaxLength = 132;
                columnMeasureGroupId.MaxLength = 132;
                columnObjectId.AllowDBNull = false;
                columnObjectId.MaxLength = 132;
                columnObjectName.AllowDBNull = false;
                columnObjectName.Caption = "Имя";
                columnObjectName.MaxLength = 132;
                columnCubeName.Caption = "Имя куба";
                columnCubeName.MaxLength = 132;
                columnProcessType.AllowDBNull = false;
                columnRefBatch.Caption = "Пакет";
                columnState.AllowDBNull = false;
                columnState.Caption = "Состояние";
                columnRecordStatus.AllowDBNull = false;
                columnRecordStatus.Caption = "Статус записи";
                columnProcessReason.AllowDBNull = false;
                columnRefUser.AllowDBNull = false;
                columnAdditionTime.AllowDBNull = false;
                columnAdditionTime.Caption = "Время добавления";
                columnErrorMessage.MaxLength = 255;
                columnOptimizationMember.AllowDBNull = false;
                columnOptimizationMember.Caption = "Из накопителя";
            }

            public AccumulatorRow FindById(int Id)
            {
                return ((AccumulatorRow)(Rows.Find(new object[] { Id })));
            }
            
            public System.Data.DataColumn IdColumn
            {
                get { return this.columnId; }                
            }
            
            public System.Data.DataColumn ObjectTypeColumn
            {
                get { return this.columnObjectType; }                
            }
            
            public System.Data.DataColumn DatabaseIdColumn
            {
                get { return this.columnDatabaseId; }                
            }
            
            public System.Data.DataColumn CubeIdColumn
            {
                get { return this.columnCubeId; }                
            }
            
            public System.Data.DataColumn MeasureGroupIdColumn
            {
                get { return this.columnMeasureGroupId; }                
            }
            
            public System.Data.DataColumn ObjectIdColumn
            {
                get { return this.columnObjectId; }                
            }
            
            public System.Data.DataColumn ObjectNameColumn
            {
                get { return this.columnObjectName; }                
            }
            
            public System.Data.DataColumn CubeNameColumn
            {
                get { return this.columnCubeName; }                
            }
            
            public System.Data.DataColumn ProcessTypeColumn
            {
                get { return this.columnProcessType; }                
            }
            
            public System.Data.DataColumn RefBatchColumn
            {
                get { return this.columnRefBatch; }                
            }
            
            public System.Data.DataColumn StateColumn
            {
                get { return this.columnState; }                
            }
            
            public System.Data.DataColumn LastProcessedColumn
            {
                get { return this.columnLastProcessed; }                
            }
            
            public System.Data.DataColumn RecordStatusColumn
            {
                get { return this.columnRecordStatus; }                
            }
            
            public System.Data.DataColumn ProcessReasonColumn
            {
                get { return this.columnProcessReason; }                
            }
            
            public System.Data.DataColumn RefUserColumn
            {
                get { return this.columnRefUser; }                
            }
            
            public System.Data.DataColumn AdditionTimeColumn
            {
                get { return this.columnAdditionTime; }                
            }
            
            public System.Data.DataColumn ErrorMessageColumn
            {
                get { return this.columnErrorMessage; }                
            }
            
            public System.Data.DataColumn OptimizationMemberColumn
            {
                get { return this.columnOptimizationMember; }                
            }
            
            public System.Data.DataColumn UserNameColumn
            {
                get { return this.columnUserName; }                
            }
            
            public int Count
            {
                get { return Rows.Count; }                
            }
            
            public AccumulatorRow this[int index]
            {
                //TODO: Создавать новый AccumulatorRow
                get { return ((AccumulatorRow)(Rows[index])); }                
            }

            //internal DataTable TableAccumulator
            //{
            //    get { return tableAccumulator; }
            //}

            //public void AcceptChanges()
            //{
            //    tableAccumulator.AcceptChanges();
            //}

            //public void RejectChanges()
            //{
            //    tableAccumulator.RejectChanges();
            //}
                        
            protected override System.Data.DataRow NewRowFromBuilder(System.Data.DataRowBuilder builder)
            {
                return new AccumulatorRow(builder);
            }
            
            protected override System.Type GetRowType()
            {
                return typeof(AccumulatorRow);
            }

            /// <summary>
            /// Применяется для добавления записей, не привязанных к пакету (т.е. ссылка на пакет - null).
            /// </summary>
            /// <param name="ObjectTypeName"></param>
            /// <param name="Id"></param>
            /// <param name="Name"></param>
            /// <param name="CubeName"></param>
            /// <param name="ProcessType"></param>
            /// <param name="batchId"></param>
            /// <param name="State"></param>
            /// <param name="LastProcessed"></param>
            /// <param name="RecordStatus"></param>
            /// <param name="ErrorMessage"></param>
            /// <param name="ProcessReason"></param>
            /// <param name="UserName"></param>
            /// <param name="AdditionTime"></param>
            /// <param name="FromAccumulator"></param>
            /// <returns></returns>
            private AccumulatorRow AddAccumulatorRow2(
                        int accumulatorId,
                        Krista.FM.Server.ProcessorLibrary.OlapObjectType OlapObjectType,
                        string DatabaseId,
                        string CubeId,
                        string MeasureGroupId,
                        string ObjectId,
                        string Name,
                        string CubeName,
                        Microsoft.AnalysisServices.ProcessType ProcessType,
                        Microsoft.AnalysisServices.AnalysisState State,
                        System.DateTime LastProcessed,
                        Krista.FM.Server.ProcessorLibrary.RecordStatus RecordStatus,
                        Krista.FM.Server.ProcessorLibrary.InvalidateReason ProcessReason,
                        int RefUser,
                        System.DateTime AdditionTime,
                        bool OptimizationMember,
                        string userName)
            {
                AccumulatorRow rowAccumulatorRow = ((AccumulatorRow)(this.NewRow()));
                rowAccumulatorRow.ItemArray = new object[] {
                        accumulatorId,
                        OlapObjectType,
                        DatabaseId,
                        CubeId,
                        MeasureGroupId,
                        ObjectId,
                        Name,
                        CubeName,
                        ProcessType,
                        null,
                        State,
                        LastProcessed,
                        RecordStatus,
                        ProcessReason,
                        RefUser,
                        AdditionTime,
                        null,    
                        OptimizationMember,
                        userName
                };
                this.Rows.Add(rowAccumulatorRow);
                return rowAccumulatorRow;
            }

            public void Add(int accumulatorId, IProcessableObjectInfo objectInfo, RecordStatus recordStatus,
                InvalidateReason processReason, int refUser, string userName)
            {
                AddAccumulatorRow2(
                    accumulatorId,
                    objectInfo.ObjectType,
                    objectInfo.DatabaseId,
                    objectInfo.CubeId,
                    objectInfo.MeasureGroupId,
                    objectInfo.ObjectID,
                    objectInfo.ObjectName,
                    objectInfo.CubeName,
                    objectInfo.ProcessType,
                    objectInfo.State,
                    objectInfo.LastProcessed,
                    recordStatus,
                    processReason,
                    refUser,
                    DateTime.Now,
                    true,
                    userName);
            }

            private RecordStatus OptimizeAccumulatorRow(IProcessableObjectInfo objectInfo, AccumulatorRow accRow)
            {
                if (accRow != null)
                {
                    int compareResult = ProcessorUtils.CompareProcessTypes(accRow.ProcessType, objectInfo.ProcessType);
                    if (compareResult >= 0)
                    {
                        return RecordStatus.CanceledByOptimization;
                    }
                    else
                    {
                        accRow.RecordStatus = RecordStatus.CanceledByOptimization;
                        return RecordStatus.Waiting;
                    }
                }
                return RecordStatus.Waiting;
            }

            public AccumulatorRow[] FindRows(int batchId)
            {
                return (AccumulatorRow[])Select(string.Format("RefBatch = '{0}'", batchId), "RefBatch, ObjectType DESC");
            }

            public AccumulatorRow[] FindRows(int batchId, string id)
            {
                return (AccumulatorRow[])Select(
                    string.Format("BatchId = '{0}' and Id = '{1}'", batchId, id), "BatchId, Id");
            }

            public AccumulatorRow[] FindRows(RecordStatus recordStatus, string objectTypeName, string name)
            {
                return (AccumulatorRow[])Select(
                    string.Format("RecordStatus = '{0}' and ObjectTypeName = '{1}' and Name = '{2}'",
                    (int)recordStatus, objectTypeName, name), "RecordStatus, ObjectTypeName, Name");
            }

            public AccumulatorRow[] FindRows(OlapObjectType objectType, string cubeId, string measureGroupId, string objectId)
            {
                if (OlapObjectType.Dimension == objectType)
                {
                    //Ищем измерение
                    return (AccumulatorRow[])Select(string.Format("ObjectId = '{0}'", objectId), "ObjectId");
                }
                else
                {
                    //Ищем раздел куба (partition)
                    return (AccumulatorRow[])Select(string.Format(
                        "CubeId = '{0}' and MeasureGroupId = '{1}' and ObjectId = '{2}'", cubeId, measureGroupId, objectId),
                        "CubeId, MeasureGroupId, ObjectId");
                }
            }

            public AccumulatorRow[] FindRows(OlapObjectType objectType, string cubeId, string measureGroupId, string objectId, RecordStatus recordStatus)
            {
                if (OlapObjectType.Dimension == objectType)
                {
                    // Ищем измерение.
                    return (AccumulatorRow[])Select(string.Format("ObjectId = '{0}' and RecordStatus = '{1}'", objectId, (int)recordStatus),
                        "ObjectId, RecordStatus");
                }
                else
                {
                    // Ищем раздел куба (partition).
                    return (AccumulatorRow[])Select(string.Format(
                        "CubeId = '{0}' and MeasureGroupId = '{1}' and ObjectId = '{2}' and RecordStatus = '{3}'",
                        cubeId, measureGroupId, objectId, (int)recordStatus),
                        "CubeId, MeasureGroupId, ObjectId, RecordStatus");
                }
            }

            public AccumulatorRow FindRow(int batchId, string id)
            {
                AccumulatorRow[] rows = FindRows(batchId, id);
                if (rows.Length > 0) return rows[0];
                return null;
            }

            public AccumulatorRow FindRow(RecordStatus recordStatus, string objectTypeName, string objectName)
            {
                AccumulatorRow[] rows = FindRows(recordStatus, objectTypeName, objectName);
                if (rows.Length > 0) return rows[0];
                return null;
            }

            public AccumulatorRow FindRow(OlapObjectType objectType, string cubeId, string measureGroupId, string objectId)
            {
                AccumulatorRow[] rows = FindRows(objectType, cubeId, measureGroupId, objectId);
                if (rows.Length > 0) return rows[0];
                return null;
            }

            public AccumulatorRow FindRow(
                OlapObjectType objectType, string cubeId, string measureGroupId, string objectId, RecordStatus recordStatus)
            {
                AccumulatorRow[] rows = FindRows(objectType, cubeId, measureGroupId, objectId, recordStatus);
                if (rows.Length > 0) return rows[0];
                return null;
            }


            public void Optimize(OlapObjectType objectType, string cubeId, string measureGroupId, string objectId)
            {
                AccumulatorRow[] waitingRecords = FindRows(objectType, cubeId, measureGroupId, objectId, RecordStatus.Waiting);

                // В любой момент до начала оптимизации не должно быть больше двух записей в состоянии ожидания.
                Debug.Assert(waitingRecords.Length <= 2, "Найдено больше двух записей в режиме ожидания!");
                // В любой момент после оптимизации должна быть одна и только одна запись в состоянии ожидания.
                if (waitingRecords.Length == 2)
                {
                    int compareResult = ProcessorUtils.CompareProcessTypes(waitingRecords[0].ProcessType, waitingRecords[1].ProcessType);
                    if (compareResult >= 0)
                    {
                        waitingRecords[1].RecordStatus = RecordStatus.CanceledByOptimization;
                    }
                    else
                    {
                        waitingRecords[0].RecordStatus = RecordStatus.CanceledByOptimization;
                    }
                }
            }

            public void SetBatchId(int batchId, int accumulatorId)
            {
                AccumulatorRow accRow = this.FindById(accumulatorId);
                accRow.RefBatch = batchId;
            }

            public bool SetRecordStatus(int accumulatorId, RecordStatus recordStatus, string errorMessage)
            {
                return SetRecordStatus(FindById(accumulatorId), recordStatus, errorMessage);
            }

            public bool SetRecordStatus(int accumulatorId, RecordStatus recordStatus)
            {
                return SetRecordStatus(accumulatorId, recordStatus, null);
            }

            public bool SetRecordStatus(
                OlapObjectType objectType, string cubeId, string measureGroupId, string objectId, RecordStatus oldRecordStatus, 
                RecordStatus newRecordStatus, string errorMessage)
            {
                return SetRecordStatus(FindRow(objectType, cubeId, measureGroupId, objectId, oldRecordStatus), newRecordStatus, errorMessage);
            }

            public bool SetRecordStatus(AccumulatorRow accRow, RecordStatus recordStatus, string errorMessage)
            {
                if (accRow != null)
                {
                    accRow.RecordStatus = recordStatus;
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        accRow.SetErrorMessageNull();
                    }
                    else
                    {
                        accRow.ErrorMessage = errorMessage;
                    }
                    return true;
                }
                return false;
            }
        }
                
        public class BatchDataTable : DataTable
        {
            //private DataTable tableBatch;

            private DataColumn columnId;
            private DataColumn columnRefUser;
            private DataColumn columnAdditionTime;
            private DataColumn columnBatchState;
            private DataColumn columnSessionId;
            private DataColumn columnUserName;            

            public BatchDataTable()
            {
                this.TableName = "Batch";
                this.BeginInit();
                this.InitClass();
                this.EndInit();
            }

            private void InitClass()
            {
                columnId = new System.Data.DataColumn("Id", typeof(int), null, System.Data.MappingType.Element);
                Columns.Add(this.columnId);
                columnRefUser = new System.Data.DataColumn("RefUser", typeof(int), null, System.Data.MappingType.Element);
                Columns.Add(this.columnRefUser);
                columnAdditionTime = new System.Data.DataColumn("AdditionTime", typeof(System.DateTime), null, System.Data.MappingType.Element);
                Columns.Add(this.columnAdditionTime);
                columnBatchState = new System.Data.DataColumn("BatchState", typeof(Krista.FM.Server.ProcessorLibrary.BatchState), null, System.Data.MappingType.Element);
                Columns.Add(this.columnBatchState);
                columnSessionId = new System.Data.DataColumn("SessionId", typeof(string), null, System.Data.MappingType.Element);
                Columns.Add(this.columnSessionId);
                columnUserName = new System.Data.DataColumn("UserName", typeof(string), null, System.Data.MappingType.Element);
                Columns.Add(this.columnUserName);
                Constraints.Add(new System.Data.UniqueConstraint("Constraint21", new System.Data.DataColumn[] {
                                this.columnId}, true));
                columnId.AllowDBNull = false;
                columnId.Unique = true;
                columnRefUser.AllowDBNull = false;
                columnRefUser.Caption = "UserName";
                columnAdditionTime.AllowDBNull = false;
                columnAdditionTime.Caption = "Время добавления";
                columnBatchState.AllowDBNull = false;
                columnSessionId.MaxLength = 132;
            }
            
            public System.Data.DataColumn IdColumn
            {
                get { return this.columnId; }                
            }
            
            public System.Data.DataColumn RefUserColumn
            {
                get { return this.columnRefUser; }                
            }
            
            public System.Data.DataColumn AdditionTimeColumn
            {
                get { return this.columnAdditionTime; }                
            }
            
            public System.Data.DataColumn BatchStateColumn
            {
                get { return this.columnBatchState; }                
            }
            
            public System.Data.DataColumn SessionIdColumn
            {
                get { return this.columnSessionId; }                
            }
            
            public System.Data.DataColumn UserNameColumn
            {
                get { return this.columnUserName; }                
            }            
            
            public int Count
            {
                get { return Rows.Count; }                
            }
            
            public BatchRow this[int index]
            {
                //TODO: Создавать новый объкт BatchRow
                get { return ((BatchRow)(Rows[index])); }                
            }

            public BatchRow FindById(int Id)
            {
                return ((BatchRow)(Rows.Find(new object[] {Id})));
            }

            public void DeleteBatch(int batchId)
            {
                BatchRow batchRow = this.FindById(batchId);
                if (batchRow != null)
                {
                    batchRow.Delete();
                }
            }

            public void StartBatch(int batchId, string sessionId)
            {
                BatchRow batchRow = this.FindById(batchId);
                batchRow.BatchState = BatchState.Running;
                batchRow.SessionId = sessionId;
            }

            public string CancelBatch(int batchId)
            {
                BatchRow batchRow = this.FindById(batchId);
                batchRow.BatchState = BatchState.Canceled;
                string sessionId = batchRow.SessionId;
                batchRow.SetSessionIdNull();
                return sessionId;
            }

            public int ComplitBatch(string sessionId)
            {
                BatchRow batchRow = FindBySessionId(sessionId);
                batchRow.SetSessionIdNull();
                batchRow.BatchState = BatchState.Complited;
                return batchRow.Id;
            }

            public BatchRow FindBySessionId(string sessionId)
            {
                if (string.IsNullOrEmpty(sessionId)) return null;

                BatchRow[] batchRows = (BatchRow[])Select(string.Format("SessionId = '{0}'", sessionId), "SessionId");
                if (batchRows.Length > 0)
                {
                    return batchRows[0];
                }
                return null;
            }
            
            //internal DataTable TableBatch
            //{
            //    get { return tableBatch; }
            //}

            //public void AcceptChanges()
            //{
            //    tableBatch.AcceptChanges();
            //}

            //public void RejectChanges()
            //{
            //    tableBatch.RejectChanges();
            //}
            
            protected override System.Data.DataRow NewRowFromBuilder(System.Data.DataRowBuilder builder)
            {
                return new BatchRow(builder);
            }
            
            protected override System.Type GetRowType()
            {
                return typeof(BatchRow);
            }
        }
        
        public class AccumulatorRow : System.Data.DataRow
        {
            private AccumulatorDataTable tableAccumulator;
            
            internal AccumulatorRow(System.Data.DataRowBuilder rb)
                : base(rb)
            {
                this.tableAccumulator = ((AccumulatorDataTable)(this.Table));
                //this.tableAccumulator = new AccumulatorDataTable(this.Table);
            }
            
            public int Id
            {
                get { return ((int)(this[this.tableAccumulator.IdColumn])); }                
                set { this[this.tableAccumulator.IdColumn] = value; }                
            }
            
            public Krista.FM.Server.ProcessorLibrary.OlapObjectType ObjectType
            {
                get
                {
                    return ((Krista.FM.Server.ProcessorLibrary.OlapObjectType)(this[this.tableAccumulator.ObjectTypeColumn]));
                }
                set
                {
                    this[this.tableAccumulator.ObjectTypeColumn] = value;
                }
            }
            
            public string DatabaseId
            {
                get { return ((string)(this[this.tableAccumulator.DatabaseIdColumn])); }                
                set { this[this.tableAccumulator.DatabaseIdColumn] = value; }                
            }
            
            public string CubeId
            {
                get
                {
                    if (this.IsCubeIdNull())
                    {
                        return null;
                    }
                    else
                    {
                        return ((string)(this[this.tableAccumulator.CubeIdColumn]));
                    }
                }
                set { this[this.tableAccumulator.CubeIdColumn] = value; }                
            }
            
            public string MeasureGroupId
            {
                get
                {
                    if (this.IsMeasureGroupIdNull())
                    {
                        return null;
                    }
                    else
                    {
                        return ((string)(this[this.tableAccumulator.MeasureGroupIdColumn]));
                    }
                }
                set { this[this.tableAccumulator.MeasureGroupIdColumn] = value; }                
            }
            
            public string ObjectId
            {
                get { return ((string)(this[this.tableAccumulator.ObjectIdColumn])); }                
                set { this[this.tableAccumulator.ObjectIdColumn] = value; }                
            }
            
            public string ObjectName
            {
                get { return ((string)(this[this.tableAccumulator.ObjectNameColumn])); }                
                set { this[this.tableAccumulator.ObjectNameColumn] = value; }                
            }
            
            public string CubeName
            {
                get
                {
                    if (this.IsCubeNameNull())
                    {
                        return null;
                    }
                    else
                    {
                        return ((string)(this[this.tableAccumulator.CubeNameColumn]));
                    }
                }
                set { this[this.tableAccumulator.CubeNameColumn] = value; }                
            }
            
            public Microsoft.AnalysisServices.ProcessType ProcessType
            {
                get
                {
                    return ((Microsoft.AnalysisServices.ProcessType)(this[this.tableAccumulator.ProcessTypeColumn]));
                }
                set
                {
                    this[this.tableAccumulator.ProcessTypeColumn] = value;
                }
            }
            
            public int RefBatch
            {
                get
                {
                    try
                    {
                        return ((int)(this[this.tableAccumulator.RefBatchColumn]));
                    }
                    catch (System.InvalidCastException e)
                    {
                        throw new System.Data.StrongTypingException("The value for column \'RefBatch\' in table \'Accumulator\' is DBNull.", e);
                    }
                }
                set
                {
                    this[this.tableAccumulator.RefBatchColumn] = value;
                }
            }
            
            public Microsoft.AnalysisServices.AnalysisState State
            {
                get
                {
                    return ((Microsoft.AnalysisServices.AnalysisState)(this[this.tableAccumulator.StateColumn]));
                }
                set { this[this.tableAccumulator.StateColumn] = value; }                
            }
            
            public System.DateTime LastProcessed
            {
                get
                {
                    try
                    {
                        return ((System.DateTime)(this[this.tableAccumulator.LastProcessedColumn]));
                    }
                    catch (System.InvalidCastException e)
                    {
                        throw new System.Data.StrongTypingException("The value for column \'LastProcessed\' in table \'Accumulator\' is DBNull.", e);
                    }
                }
                set { this[this.tableAccumulator.LastProcessedColumn] = value; }                
            }
            
            public Krista.FM.Server.ProcessorLibrary.RecordStatus RecordStatus
            {
                get
                {
                    return ((Krista.FM.Server.ProcessorLibrary.RecordStatus)(this[this.tableAccumulator.RecordStatusColumn]));
                }
                set { this[this.tableAccumulator.RecordStatusColumn] = value; }                
            }
            
            public Krista.FM.Server.ProcessorLibrary.InvalidateReason ProcessReason
            {
                get
                {
                    return ((Krista.FM.Server.ProcessorLibrary.InvalidateReason)(this[this.tableAccumulator.ProcessReasonColumn]));
                }
                set { this[this.tableAccumulator.ProcessReasonColumn] = value; }                
            }
            
            public int RefUser
            {
                get { return ((int)(this[this.tableAccumulator.RefUserColumn])); }                
                set { this[this.tableAccumulator.RefUserColumn] = value; }                
            }
            
            public System.DateTime AdditionTime
            {
                get { return ((System.DateTime)(this[this.tableAccumulator.AdditionTimeColumn])); }                
                set { this[this.tableAccumulator.AdditionTimeColumn] = value; }                
            }
            
            public string ErrorMessage
            {
                get
                {
                    try
                    {
                        return ((string)(this[this.tableAccumulator.ErrorMessageColumn]));
                    }
                    catch (System.InvalidCastException e)
                    {
                        throw new System.Data.StrongTypingException("The value for column \'ErrorMessage\' in table \'Accumulator\' is DBNull.", e);
                    }
                }
                set { this[this.tableAccumulator.ErrorMessageColumn] = value; }                
            }
            
            public bool OptimizationMember
            {
                get { return ((bool)(this[this.tableAccumulator.OptimizationMemberColumn])); }                
                set { this[this.tableAccumulator.OptimizationMemberColumn] = value; }                
            }
            
            public string UserName
            {
                get
                {
                    if (this.IsUserNameNull())
                    {
                        return null;
                    }
                    else
                    {
                        return ((string)(this[this.tableAccumulator.UserNameColumn]));
                    }
                }
                set { this[this.tableAccumulator.UserNameColumn] = value; }                
            }
            
            public BatchRow BatchRow
            {
                get { return ((BatchRow)(this.GetParentRow(this.Table.ParentRelations["Batch_Accumulator"]))); }                
                set { this.SetParentRow(value, this.Table.ParentRelations["Batch_Accumulator"]); }                
            }
            
            public bool IsCubeIdNull()
            {
                return this.IsNull(this.tableAccumulator.CubeIdColumn);
            }
            
            public void SetCubeIdNull()
            {
                this[this.tableAccumulator.CubeIdColumn] = System.Convert.DBNull;
            }
            
            public bool IsMeasureGroupIdNull()
            {
                return this.IsNull(this.tableAccumulator.MeasureGroupIdColumn);
            }
            
            public void SetMeasureGroupIdNull()
            {
                this[this.tableAccumulator.MeasureGroupIdColumn] = System.Convert.DBNull;
            }
            
            public bool IsCubeNameNull()
            {
                return this.IsNull(this.tableAccumulator.CubeNameColumn);
            }
                        
            public void SetCubeNameNull()
            {
                this[this.tableAccumulator.CubeNameColumn] = System.Convert.DBNull;
            }
                        
            public bool IsRefBatchNull()
            {
                return this.IsNull(this.tableAccumulator.RefBatchColumn);
            }
                        
            public void SetRefBatchNull()
            {
                this[this.tableAccumulator.RefBatchColumn] = System.Convert.DBNull;
            }
                        
            public bool IsLastProcessedNull()
            {
                return this.IsNull(this.tableAccumulator.LastProcessedColumn);
            }
                        
            public void SetLastProcessedNull()
            {
                this[this.tableAccumulator.LastProcessedColumn] = System.Convert.DBNull;
            }
                        
            public bool IsErrorMessageNull()
            {
                return this.IsNull(this.tableAccumulator.ErrorMessageColumn);
            }
                        
            public void SetErrorMessageNull()
            {
                this[this.tableAccumulator.ErrorMessageColumn] = System.Convert.DBNull;
            }
                        
            public bool IsUserNameNull()
            {
                return this.IsNull(this.tableAccumulator.UserNameColumn);
            }
                        
            public void SetUserNameNull()
            {
                this[this.tableAccumulator.UserNameColumn] = System.Convert.DBNull;
            }
        }

        public class BatchRow : System.Data.DataRow
        {
            private BatchDataTable tableBatch;
                        
            internal BatchRow(System.Data.DataRowBuilder rb)
                : base(rb)
            {
                this.tableBatch = ((BatchDataTable)(this.Table));
                //this.tableBatch = new BatchDataTable(this.Table);
            }
            
            public int Id
            {
                get { return ((int)(this[this.tableBatch.IdColumn])); }                
                set { this[this.tableBatch.IdColumn] = value; }                
            }
            
            public int RefUser
            {
                get { return ((int)(this[this.tableBatch.RefUserColumn])); }                
                set { this[this.tableBatch.RefUserColumn] = value; }                
            }
            
            public System.DateTime AdditionTime
            {
                get { return ((System.DateTime)(this[this.tableBatch.AdditionTimeColumn])); }                
                set { this[this.tableBatch.AdditionTimeColumn] = value; }                
            }
            
            public Krista.FM.Server.ProcessorLibrary.BatchState BatchState
            {
                get
                {
                    return ((Krista.FM.Server.ProcessorLibrary.BatchState)(this[this.tableBatch.BatchStateColumn]));
                }
                set { this[this.tableBatch.BatchStateColumn] = value; }                
            }
            
            public string SessionId
            {
                get
                {
                    if (this.IsSessionIdNull())
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return ((string)(this[this.tableBatch.SessionIdColumn]));
                    }
                }
                set { this[this.tableBatch.SessionIdColumn] = value; }                
            }
            
            public string UserName
            {
                get
                {
                    try
                    {
                        return ((string)(this[this.tableBatch.UserNameColumn]));
                    }
                    catch (System.InvalidCastException e)
                    {
                        throw new System.Data.StrongTypingException("The value for column \'UserName\' in table \'Batch\' is DBNull.", e);
                    }
                }
                set { this[this.tableBatch.UserNameColumn] = value; }                
            }
            
            public bool IsSessionIdNull()
            {
                return this.IsNull(this.tableBatch.SessionIdColumn);
            }
            
            public void SetSessionIdNull()
            {
                this[this.tableBatch.SessionIdColumn] = System.Convert.DBNull;
            }
            
            public bool IsUserNameNull()
            {
                return this.IsNull(this.tableBatch.UserNameColumn);
            }
            
            public void SetUserNameNull()
            {
                this[this.tableBatch.UserNameColumn] = System.Convert.DBNull;
            }
            
            public AccumulatorRow[] GetAccumulatorRows()
            {
                return ((AccumulatorRow[])(base.GetChildRows(this.Table.ChildRelations["Batch_Accumulator"])));
            }
        }

        public int[] GetBatchRows(int batchId)
        {
            AccumulatorRow[] accRows = accumulator.FindRows(batchId);
            int[] accKeys = new int[accRows.Length];
            for (int i = 0; i < accRows.Length; i++)
            {
                accKeys[i] = accRows[i].Id;
            }
            return accKeys;
        }

        public int GetBatchIndex(int batchId)
        {
            DataRow dataRow = batch.FindById(batchId);
            if (dataRow != null)
            {
                return batch.Rows.IndexOf(dataRow);
            }
            return -1;
        }

        public DataSet DataSetAccumulator
        {
            get { return dsAccumulator; }
        }

        public AccumulatorDataTable Accumulator
        {
            get { return accumulator; }
        }

        public BatchDataTable Batch
        {
            get { return batch; }
        }

        public void AcceptChanges()
        {
            dsAccumulator.AcceptChanges();
        }

        public void RejectChanges()
        {
            dsAccumulator.RejectChanges();
        }
    }
}
