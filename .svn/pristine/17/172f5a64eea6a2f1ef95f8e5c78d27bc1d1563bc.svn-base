using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Contexts;
using System.Threading;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.Common;

namespace Krista.FM.Server.OLAP.Processor
{    
    /// <summary>
    /// Типизированный датасет. Не умеет синхронизировать работу нескольких пользователей.
    /// </summary>
    public partial class DataSetAccumulator
    {
        partial class AccumulatorDataTable
        {
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
                return (AccumulatorRow[])Select(string.Format("RefBatch = '{0}'", batchId), "RefBatch");
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

            public AccumulatorRow[] FindRows(string cubeId, string measureGroupId, string objectId)
            {
                if (string.IsNullOrEmpty(cubeId) && string.IsNullOrEmpty(measureGroupId))
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

            public AccumulatorRow FindRow(string cubeId, string measureGroupId, string objectId)
            {
                AccumulatorRow[] rows = FindRows(cubeId, measureGroupId, objectId);
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

            public bool SetRecordStatus(int accumulatorId, RecordStatus recordStatus)
            {
                return SetRecordStatus(FindById(accumulatorId), recordStatus);
            }

            public bool SetRecordStatus(
                string cubeId, string measureGroupId, string objectId, RecordStatus recordStatus)
            {
                return SetRecordStatus(FindRow(cubeId, measureGroupId, objectId), recordStatus);
            }

            public bool SetRecordStatus(AccumulatorRow accRow, RecordStatus recordStatus)
            {   
                if (accRow != null)
                {
                    accRow.RecordStatus = recordStatus;
                    return true;
                }
                return false;
            }
        }

        partial class BatchDataTable
        {
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
        }        
        
        public int[] GetBatchRows(int batchId)
        {
            AccumulatorRow[] accRows = Accumulator.FindRows(batchId);
            int[] accKeys = new int[accRows.Length];
            for (int i = 0; i < accRows.Length; i++)
            {
                accKeys[i] = accRows[i].Id;
            }
            return accKeys;
        }                      

        public int GetBatchIndex(int batchId)
        {
            DataRow dataRow = Batch.FindById(batchId);
            if (dataRow != null)
            {
                return Batch.Rows.IndexOf(dataRow);
            }
            return -1;
        }        
    }
}
