using System;
using System.Data.SqlTypes;

using Microsoft.AnalysisServices;

using Krista.FM.Server.ProcessorLibrary;


namespace Krista.FM.Server.OLAP.Processor
{
    [System.Diagnostics.DebuggerStepThrough]
    public class ProcessableObjectInfo : MarshalByRefObject, IProcessableObjectInfo
    {
        private OlapObjectType objectType;
        private ProcessType processType = ProcessType.ProcessDefault;
        private string databaseId;
        private string databaseName;
        private string cubeId;
        private string cubeName;
        private string measureGroupId;
        private string measureGroupName;
        private string objectId;
        private string objectName;
        private string fullName;
        private string objectKey;
        private AnalysisState state;
        private DateTime lastProcessed;
        private string processResult = string.Empty;
        private RecordStatus recordStatus = RecordStatus.Waiting;
        private bool needProcess;
        private string revision;
        private string batchOperations;

        private void Init(string fullName, string objectKey, OlapObjectType objectType, string databaseId, string databaseName,
            string cubeId, string cubeName, string measureGroupId, string measureGroupName,
            string objectId, string objectName, ProcessType objectProcessType,
            AnalysisState state, DateTime lastProcessed, RecordStatus recordStatus, string processResult, bool needProcess,
            string revision, string batchOperations)
        {
            this.objectType = objectType;
            this.databaseId = databaseId;
            this.databaseName = databaseName;
            this.cubeId = cubeId;
            this.cubeName = cubeName;
            this.measureGroupId = measureGroupId;
            this.measureGroupName = measureGroupName;
            this.objectId = objectId;
            this.objectName = objectName;
            this.fullName = fullName;
            this.objectKey = objectKey;
            this.processType = objectProcessType;
            this.state = state;
            this.lastProcessed = lastProcessed;
            this.RecordStatus = recordStatus;
            this.ProcessResult = processResult;
            this.needProcess = needProcess;
            this.revision = revision;
            this.batchOperations = batchOperations;
        }

        public ProcessableObjectInfo(string fullName, string objectKey, OlapObjectType objectType, string databaseId, string databaseName,
            string cubeId, string cubeName, string measureGroupId, string measureGroupName,
            string objectId, string objectName, ProcessType objectProcessType, AnalysisState state, 
            DateTime lastProcessed, RecordStatus recordStatus, string processResult, bool needProcess, string batchOperations)
        {
            Init(fullName, objectKey, objectType, databaseId, databaseName, cubeId, cubeName, measureGroupId, measureGroupName,
                objectId, objectName, objectProcessType, state, lastProcessed, recordStatus, processResult, needProcess, string.Empty, batchOperations);
        }

        public ProcessableObjectInfo(string fullName, string objectKey, OlapObjectType objectType, string databaseId, string databaseName,
            string cubeId, string cubeName, string measureGroupId, string measureGroupName,
            string objectId, string objectName, ProcessType objectProcessType,
            AnalysisState state, DateTime lastProcessed, string revision, string batchOperations)
        {
            Init(fullName, objectKey, objectType, databaseId, databaseName, cubeId, cubeName, measureGroupId, measureGroupName,
                objectId, objectName, objectProcessType, state, lastProcessed, RecordStatus.Waiting, string.Empty, false, revision, batchOperations);
        }

        public ProcessableObjectInfo(string fullName, string objectKey, OlapObjectType objectType, string databaseId, string databaseName,
            string cubeId, string cubeName, string measureGroupId, string measureGroupName,
            string objectId, string objectName, ProcessType objectProcessType)
        {
            Init(fullName, objectKey, objectType, databaseId, databaseName, cubeId, cubeName, measureGroupId, measureGroupName,
                objectId, objectName, objectProcessType, AnalysisState.Unprocessed, SqlDateTime.MinValue.Value,
                RecordStatus.Waiting, string.Empty, false, string.Empty, String.Empty);
        }

        public ProcessableObjectInfo(string fullName, string objectKey, OlapObjectType objectType, string databaseId, string databaseName,
            string objectId, string objectName, ProcessType objectProcessType, AnalysisState state, 
            DateTime lastProcessed, RecordStatus recordStatus, string processResult, bool needProcess)
        {
            Init(fullName, objectKey, objectType, databaseId, databaseName, null, null, null, null, objectId, objectName, objectProcessType,
                state, lastProcessed, recordStatus, processResult, needProcess, string.Empty, String.Empty);
        }

        public ProcessableObjectInfo(string fullName, string objectKey, OlapObjectType objectType, string databaseId, string databaseName,
            string objectId, string objectName, ProcessType objectProcessType)
        {
            Init(fullName, objectKey, objectType, databaseId, databaseName, null, null, null, null, objectId, objectName, objectProcessType,
                AnalysisState.Unprocessed, SqlDateTime.MinValue.Value, RecordStatus.Waiting, string.Empty, false, string.Empty, String.Empty);
        }
                
        public ProcessType ProcessType
        {
            get { return processType; }
            set
            {
                if (ProcessType.ProcessAdd == value && OlapObjectType.Dimension == ObjectType)
                {
                    processType = ProcessType.ProcessUpdate;
                }
                else
                {
                    if (ProcessType.ProcessUpdate  == value && OlapObjectType.Partition == ObjectType)
                    {
                        processType = ProcessType.ProcessAdd;
                    }
                    else
                    {
                        processType = value;
                    }
                }
            }
        }
        
        public string ObjectID
        {
            get { return objectId; }
        }
                
        public string ObjectName
        {
            get { return objectName; }
        }

        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }

        public string ObjectKey
        {
            get { return objectKey; }
            set { objectKey = value; }
        }

        public OlapObjectType ObjectType
        {
            get { return objectType; }            
        }
        
        public string DatabaseId
        {
            get { return databaseId; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
        }
                
        public string CubeId
        {
            get { return cubeId; }            
        }
                
        public string MeasureGroupId
        {
            get { return measureGroupId; }            
        }

        public string MeasureGroupName
        {
            get { return measureGroupName; }
        }
                
        public string CubeName
        {
            get { return cubeName; }            
        }

        public AnalysisState State
        {
            get { return state; }
            set { state = value; }
        }

        public DateTime LastProcessed
        {
            get { return lastProcessed; }
            set { lastProcessed = value; }
        }

        public override string ToString()
        {
            return objectName;
        }

        public string ProcessResult
        {
            get { return processResult; }
            set { processResult = value; }
        }

        public RecordStatus RecordStatus
        {
            get { return recordStatus; }
            set { recordStatus = value; }
        }

        public bool NeedProcess
        {
            get { return needProcess; }
        }

        public string Revision
        {
            get { return revision; }
            set { revision = value; }
        }

        public string BatchOperations
        {
            get { return batchOperations; }
            set { batchOperations = value; }
        }
    }
}
