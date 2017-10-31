using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

using Microsoft.AnalysisServices;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.OLAP.Processor
{
    public sealed class OlapDBWrapper2005 : OlapDBWrapper
    {
        private static volatile OlapDBWrapper2005 instance;
        private static readonly object syncRoot = new object();
        

        public static OlapDBWrapper2005 GetInstance(IOlapDatabase olapDatabase, IScheme scheme)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new OlapDBWrapper2005(olapDatabase, scheme);
                    }
                }
            }
            return instance;
        }

        private OlapDBWrapper2005(IOlapDatabase olapDatabase, IScheme scheme)
            : base(scheme, olapDatabase)
        {
            FillDataSet();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DatabaseObject.Dispose();                
            }            
            base.Dispose(disposing);
        }

        private Database DatabaseObject
        {
            get { return (Database)olapDatabase.DatabaseObject; }
        }

        /// <summary>
        /// Проверяет наличие продключения к базе данных, 
        /// в случаи потери соединения восстанавливает его.
        /// </summary>
        protected override void CheckConnection()
        {
            olapDatabase.CheckConnection();
        }

        protected override Dictionary<string, IProcessableObjectInfo> GetCubes()
        {
            Dictionary<string, IProcessableObjectInfo> objectToProcessList = new Dictionary<string, IProcessableObjectInfo>();
            if (this.DatabaseObject != null)
            {
                foreach (Cube item in this.DatabaseObject.Cubes)
                {
                    DateTime lastProcessed;
                    AnalysisState state = item.State;

                    switch (state)
                    {
                        case AnalysisState.PartiallyProcessed:
                        case AnalysisState.Processed:
                            lastProcessed = item.LastProcessed;
                            break;
                        case AnalysisState.Unprocessed:
                        default:
                            lastProcessed = SqlDateTime.MinValue.Value;
                            break;
                    }

					if (!item.Annotations.Contains("FullName"))
					{
						RegisterDatabaseError("Не задан FullName", item.ID, item.Name, OlapObjectType.Cube);
						continue;
					}

					if (!item.Annotations.Contains("ObjectKey"))
					{
						RegisterDatabaseError("Не задан ObjectKey", item.ID, item.Name, OlapObjectType.Cube);
					}

					ProcessableObjectInfo processableObjectInfo = new ProcessableObjectInfo(
                        item.Annotations["FullName"].Value.InnerText,
                        item.Annotations.Contains("ObjectKey") ? item.Annotations["ObjectKey"].Value.InnerText : Guid.Empty.ToString(),
                        OlapObjectType.Cube, DatabaseId, DatabaseName, 
                        item.ID, item.Name, 
                        ProcessType.ProcessFull, state, lastProcessed,
                        RecordStatus.Waiting, string.Empty, false);

                    if (objectToProcessList.ContainsKey(processableObjectInfo.ObjectID))
                    {
                        string message = String.Format(
                            "Объекты \"{0}\" и \"{1}\" имеют одинаковый ObjectID = {2}",
                            processableObjectInfo.ObjectName,
                            objectToProcessList[processableObjectInfo.ObjectID].ObjectName,
                            processableObjectInfo.ObjectID);
                        Trace.TraceError(message);
                        throw new Exception(message);
                    }
                    objectToProcessList.Add(processableObjectInfo.ObjectID, processableObjectInfo);
                }                
            }
            return objectToProcessList;
        }

        protected override Dictionary<string, IProcessableObjectInfo> GetDimensions()
        {
            Dictionary<string, IProcessableObjectInfo> objectToProcessList = new Dictionary<string, IProcessableObjectInfo>();
            if (this.DatabaseObject != null)
            {
                foreach (Dimension item in this.DatabaseObject.Dimensions)
                {
                    DateTime lastProcessed;
                    item.Refresh();

                    switch (item.State)
                    {
                        case AnalysisState.PartiallyProcessed:
                        case AnalysisState.Processed:
                            lastProcessed = item.LastProcessed;
                            break;
                        case AnalysisState.Unprocessed:
                        default:
                            lastProcessed = SqlDateTime.MinValue.Value;
                            break;
                    }

					if (!item.Annotations.Contains("FullName"))
					{
						RegisterDatabaseError("Не задан FullName", item.ID, item.Name, OlapObjectType.Dimension);
						continue;
					}

					if (!item.Annotations.Contains("ObjectKey"))
					{
						RegisterDatabaseError("Не задан ObjectKey", item.ID, item.Name, OlapObjectType.Dimension);
					}

					ProcessableObjectInfo processableObjectInfo = new ProcessableObjectInfo(
                        item.Annotations["FullName"].Value.InnerText,
                        item.Annotations.Contains("ObjectKey") ? item.Annotations["ObjectKey"].Value.InnerText : Guid.Empty.ToString(),
                        OlapObjectType.Dimension, DatabaseId, DatabaseName,
                        item.ID, item.Name,
                        ProcessType.ProcessFull, item.State, lastProcessed,
                        RecordStatus.Waiting, string.Empty, false);

                    if (objectToProcessList.ContainsKey(processableObjectInfo.ObjectID))
                    {
                        string message = String.Format(
                            "Объекты \"{0}\" и \"{1}\" имеют одинаковый ObjectID = {2}",
                            processableObjectInfo.ObjectName,
                            objectToProcessList[processableObjectInfo.ObjectID].ObjectName,
                            processableObjectInfo.ObjectID);
                        Trace.TraceError(message);
                        throw new Exception(message);
                    }
                    objectToProcessList.Add(processableObjectInfo.ObjectID, processableObjectInfo);
                }
            }
            return objectToProcessList;
        }

        protected override List<CubeDimensionLink> GetCubeDimensionLinkList()
        {
            List<CubeDimensionLink> cubeDimensionLinkList = new List<CubeDimensionLink>();
            if (this.DatabaseObject != null)
            {
                foreach (Cube cube in this.DatabaseObject.Cubes)
                {
                    foreach (CubeDimension dimension in cube.Dimensions)
                    {
                        cubeDimensionLinkList.Add(new CubeDimensionLink(cube.ID, dimension.Dimension.ID));
                    }
                }
            }
            return cubeDimensionLinkList;
        }

        public override Dictionary<string, IProcessableObjectInfo> GetCubePartitions(string cubeId)
        {
            Dictionary<string, IProcessableObjectInfo> objectToProcessList = new Dictionary<string, IProcessableObjectInfo>();
            if (this.DatabaseObject != null)
            {
                Cube cube = this.DatabaseObject.Cubes.Find(cubeId);
                
                if (cube != null)
                {
                    foreach (MeasureGroup mg in cube.MeasureGroups)
                    {
                        if (!mg.IsLinked)
                        {
                            foreach (Partition partition in mg.Partitions)
                            {
                                partition.Refresh();

                                DateTime lastProcessed;

                                switch (partition.State)
                                {
                                    case AnalysisState.PartiallyProcessed:
                                    case AnalysisState.Processed:
                                        lastProcessed = partition.LastProcessed;
                                        break;
                                    case AnalysisState.Unprocessed:
                                    default:
                                        lastProcessed = SqlDateTime.MinValue.Value;
                                        break;
                                }

                                ProcessableObjectInfo processableObjectInfo = new ProcessableObjectInfo(
                                    mg.Annotations["FullName"].Value.InnerText,
                                    mg.Annotations.Contains("ObjectKey") ? mg.Annotations["ObjectKey"].Value.InnerText : Guid.Empty.ToString(),
                                    OlapObjectType.Partition, DatabaseId, DatabaseName,
                                    cube.ID, cube.Name, mg.ID, mg.Name, partition.ID, partition.Name,
                                    ProcessType.ProcessFull, partition.State, lastProcessed,
                                    mg.Annotations.Contains("Revision") ? mg.Annotations["Revision"].Value.InnerText : String.Empty,
                                    mg.Annotations.Contains("BatchOperations") ? mg.Annotations["BatchOperations"].Value.InnerText : String.Empty);

                                if (objectToProcessList.ContainsKey(processableObjectInfo.ObjectID))
                                {
                                    string message = String.Format(
                                        "Объекты \"{0}\" и \"{1}\" имеют одинаковый ObjectID = {2}",
                                        processableObjectInfo.ObjectName,
                                        objectToProcessList[processableObjectInfo.ObjectID].ObjectName,
                                        processableObjectInfo.ObjectID);
                                    Trace.TraceError(message);
                                    throw new Exception(message);
                                }
                                objectToProcessList.Add(processableObjectInfo.ObjectID, processableObjectInfo);
                            }                            
                        }
                    }
                }
            }
            return objectToProcessList;
        }

        public override Dictionary<string, IProcessableObjectInfo> GetCubeDimensions(string cubeId)
        {
            return GetCubeDimensions(cubeId, new Dictionary<string, IProcessableObjectInfo>());            
        }        

        public override Dictionary<string, IProcessableObjectInfo> GetCubeDimensions(
            string cubeId, Dictionary<string, IProcessableObjectInfo> dimensionList)
        {            
            if (this.DatabaseObject != null)
            {
                Cube cube = this.DatabaseObject.Cubes.Find(cubeId);
                if (cube != null)
                {
                    foreach (CubeDimension cubeDimension in cube.Dimensions)
                    {
                        if (!dimensionList.ContainsKey(cubeDimension.Dimension.ID))
                        {
                            ProcessableObjectInfo processableObjectInfo = new ProcessableObjectInfo(
                                String.Empty, Guid.Empty.ToString(),
                                OlapObjectType.Dimension, DatabaseId, DatabaseName,
                                cubeDimension.Dimension.ID, cubeDimension.Dimension.Name, ProcessType.ProcessFull);
                            dimensionList.Add(processableObjectInfo.ObjectID, processableObjectInfo);
                        }                        
                    }
                }
            }
            return dimensionList;
        }        

        private Partition GetPartition(string cubeId, string measureGroupId, string objectId)
        {
            Cube cube = this.DatabaseObject.Cubes.Find(cubeId);
            if (null != cube)
            {
                MeasureGroup measureGroup = cube.MeasureGroups.Find(measureGroupId);
                if (null != measureGroup)
                {
                    return measureGroup.Partitions.Find(objectId);
                }
            }
            return null;
        }

        internal override void RefreshState(OlapObjectType objectType, string cubeId, string measureGroupId, string objectId)
        {
            DataRow dataRow = GetDataRow(objectId);
            if (null != dataRow)
            {
                DateTime lastProcessed;
                AnalysisState analysisState =
                    GetState(objectType, cubeId, measureGroupId, objectId, out lastProcessed);
                SetState(dataRow, analysisState, lastProcessed);
            }
        }

        protected override AnalysisState GetState(OlapObjectType objectType,
            string cubeId, string measureGroupId, string objectId, out DateTime lastProcessed)
        {
            ProcessableMajorObject processableObject = null; 
           
            switch (objectType)
            {                
                case OlapObjectType.Cube:
                    processableObject = this.DatabaseObject.Cubes.Find(objectId);
                    break;                
                case OlapObjectType.Partition:                    
                    processableObject = GetPartition(cubeId, measureGroupId, objectId);
                    break;
                case OlapObjectType.Dimension:
                    processableObject = this.DatabaseObject.Dimensions.Find(objectId);
                    break;
                default:
                    break;
            }
            if (null != processableObject)
            {
                processableObject.Refresh(true);

                lastProcessed = processableObject.LastProcessed;
                return processableObject.State;
            }
            else
            {
                lastProcessed = SqlDateTime.MinValue.Value;
                return AnalysisState.Unprocessed;
            }
        }

        public override string DatabaseId
        {
            get { return this.DatabaseObject.ID; }
        }

        public override string DatabaseName
        {
            get { return this.DatabaseObject.Name; }
        }

        protected override bool ObjectExist(OlapObjectType objectType, string objectId)
        {
            switch (objectType)
            {
                case OlapObjectType.Cube:                    
                    return DatabaseObject.Cubes.Contains(objectId);
                case OlapObjectType.Dimension:
                    return DatabaseObject.Dimensions.Contains(objectId);
                default:
                    return false;
            }
        }

        protected override bool ObjectExist(OlapObjectType objectType, string objectId, string parentId)
        {            
            Cube cube = DatabaseObject.Cubes.Find(parentId);            
            if (null != cube)
            {
                //Родитель - куб, и этот куб существует в многомерной базе.
                switch (objectType)
                {
                    case OlapObjectType.MeasureGroup:
                        return cube.MeasureGroups.Contains(objectId);
                    case OlapObjectType.Partition:
                        foreach (MeasureGroup item in cube.MeasureGroups)
                        {
                            if (item.Partitions.Contains(objectId))
                            {
                                return true;
                            }
                        }
                        return false;
                    default:
                        break;
                }
            }
            else
            {
                //Возможно родитель - группа мер. Пытаемся найти эту группу мер.                
                foreach (Cube item in DatabaseObject.Cubes)
                {
                    if (item.MeasureGroups.Contains(parentId))
                    {
                        return ObjectExist(objectType, objectId, item.ID);                        
                    }
                }                
            }
            return false;           
        }        
    }
}
