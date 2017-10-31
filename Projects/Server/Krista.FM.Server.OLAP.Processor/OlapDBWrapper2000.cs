using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

using DSO;

using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;
using Microsoft.AnalysisServices;
using Cube=DSO.Cube;
using CubeDimension=DSO.CubeDimension;
using Database=DSO.Database;
using Dimension=DSO.Dimension;
using Partition=DSO.Partition;


namespace Krista.FM.Server.OLAP.Processor
{
    /// <summary>
    /// Обёртка для доступа к объектам многомерной базы данных Analisys Service 2000.
    /// </summary>
    public sealed class OlapDBWrapper2000 : OlapDBWrapper
    {
        private static volatile OlapDBWrapper2000 instance;
        private static readonly object syncRoot = new object();

        private string databaseId;

        [System.Diagnostics.DebuggerStepThrough]
        public static OlapDBWrapper2000 GetInstance(IOlapDatabase olapDatabase, IScheme scheme)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new OlapDBWrapper2000(olapDatabase, scheme);
                    }
                }
            }
            return instance;
        }

        private OlapDBWrapper2000(IOlapDatabase olapDatabase, IScheme scheme)
            : base(scheme, olapDatabase)
        {
            FillDataSet();
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

		/// <summary>
		/// Возвращает CustomProperties с заданным именем.
		/// Если свойства нет, то возникает исключение.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="propertyName">Имя свойства</param>
		/// <returns></returns>
		private static string GetProperty(Properties properties, string propertyName)
		{
			return (string)properties[propertyName].Value;
		}

		/// <summary>
        /// Возвращает CustomProperties с заданным именем.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="defaultValue">Значение для свойства по умолчанию.</param>
        /// <returns></returns>
        private static string GetProperty(Properties properties, string propertyName, string defaultValue)
        {
            try
            {
                return (string)properties[propertyName].Value;
            }
            catch (Exception)
            {
                return defaultValue;                
            }
        }

        protected override Dictionary<string, IProcessableObjectInfo> GetCubes()
        {
            Dictionary<string, IProcessableObjectInfo> objectToProcessList = new Dictionary<string, IProcessableObjectInfo>();
            if (DatabaseObject != null)
            {
                foreach (Cube item in DatabaseObject.Cubes)
                {
                    Microsoft.AnalysisServices.AnalysisState state;
                    DateTime lastProcessed;
                    try
                    {
                        state = ProcessorUtils.ConvertToAS2005State(item.State);
                        switch (state)
                        {
                            case Microsoft.AnalysisServices.AnalysisState.PartiallyProcessed:
                            case Microsoft.AnalysisServices.AnalysisState.Processed:
                                lastProcessed = item.LastProcessed;
                                break;
                            case Microsoft.AnalysisServices.AnalysisState.Unprocessed:
                            default:
                                lastProcessed = SqlDateTime.MinValue.Value;
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError("{0}", Krista.Diagnostics.KristaDiagnostics.ExpandException(e));
                        state = Microsoft.AnalysisServices.AnalysisState.Unprocessed;
                        lastProcessed = SqlDateTime.MinValue.Value;
                    }

					if (GetProperty(item.CustomProperties, "FullName", String.Empty) == String.Empty)
					{
						RegisterDatabaseError("Не задан FullName", String.Empty, item.Name, OlapObjectType.Cube);
						continue;
					}

					if (GetProperty(item.CustomProperties, "ObjectKey", String.Empty) == String.Empty)
					{
						RegisterDatabaseError("Не задан ObjectKey", String.Empty, item.Name, OlapObjectType.Cube);
					}

					ProcessableObjectInfo processableObjectInfo = new ProcessableObjectInfo(
                        GetProperty(item.CustomProperties, "FullName"),
                        GetProperty(item.CustomProperties, "ObjectKey", String.Empty),
                        OlapObjectType.Cube, DatabaseId, DatabaseName,
                        GetProperty(item.CustomProperties, "ID", item.Name), item.Name,
                        Microsoft.AnalysisServices.ProcessType.ProcessFull, state, lastProcessed,
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
            if (DatabaseObject != null)
            {
                foreach (Dimension item in DatabaseObject.Dimensions)
                {
                    Microsoft.AnalysisServices.AnalysisState state;
                    state = ProcessorUtils.ConvertToAS2005State(item.State);
                    DateTime lastProcessed;
                    switch (state)
                    {
                        case Microsoft.AnalysisServices.AnalysisState.PartiallyProcessed:
                        case Microsoft.AnalysisServices.AnalysisState.Processed:
                            lastProcessed = item.LastProcessed;
                            break;
                        case Microsoft.AnalysisServices.AnalysisState.Unprocessed:
                        default:
                            lastProcessed = SqlDateTime.MinValue.Value;
                            break;
                    }

					if (GetProperty(item.CustomProperties, "FullName", String.Empty) == String.Empty)
					{
						RegisterDatabaseError("Не задан FullName", String.Empty, item.Name, OlapObjectType.Dimension);
						continue;
					}

					if (GetProperty(item.CustomProperties, "ObjectKey", String.Empty) == String.Empty)
					{
						RegisterDatabaseError("Не задан ObjectKey", String.Empty, item.Name, OlapObjectType.Dimension);
					}

					ProcessableObjectInfo processableObjectInfo = new ProcessableObjectInfo(
                        GetProperty(item.CustomProperties, "FullName"),
                        GetProperty(item.CustomProperties, "ObjectKey", String.Empty),
                        OlapObjectType.Dimension, DatabaseId, DatabaseName,
                        GetProperty(item.CustomProperties, "ID", item.Name), item.Name,
                        Microsoft.AnalysisServices.ProcessType.ProcessFull, state, lastProcessed,
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
            if (DatabaseObject != null)
            {
                foreach (Cube cube in DatabaseObject.Cubes)
                {   
                    foreach (Dimension cubeDimension in cube.Dimensions)
                    {
                        Dimension dimension = (Dimension)DatabaseObject.Dimensions.Item(cubeDimension.Name);
                        if (null != dimension)
                        {
                            cubeDimensionLinkList.Add(new CubeDimensionLink(
                                GetProperty(cube.CustomProperties, "ID", cube.Name),
                                GetProperty(dimension.CustomProperties, "ID", dimension.Name)));
                        }                        
                    }
                }
            }
            return cubeDimensionLinkList;
        }

        /// <summary>
        /// Возвращает куб с заданным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор куба.</param>
        /// <returns>Найденный куб.</returns>
        private Cube GetCubeById(string id)
        {
            foreach (Cube item in DatabaseObject.Cubes)
            {
                string itemId = GetProperty(item.CustomProperties, "ID", null);
                if (!string.IsNullOrEmpty(itemId) && itemId.Equals(id))
                {
                    return item;
                } 
            }
            return null;
        }

        /// <summary>
        /// Возвращает измерение с заданным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор измерения.</param>
        /// <returns>Найденное измерение.</returns>
        private Dimension GetDimensionById(string id)
        {
            foreach (Dimension item in DatabaseObject.Dimensions)
            {
                string itemId = GetProperty(item.CustomProperties, "ID", null);
                if (!string.IsNullOrEmpty(itemId) && itemId.Equals(id))
                {
                    return item;
                } 
            }
            return null;
        }

        /// <summary>
        /// Возвращает список разделов куба. Обращается напрямую к многомерной базе.
        /// </summary>
        /// <param name="cubeId">Идентификатор куба</param>
        /// <returns>Список разделов куба.</returns>
        public override Dictionary<string, IProcessableObjectInfo> GetCubePartitions(string cubeId)
        {
            Dictionary<string, IProcessableObjectInfo> objectToProcessList = new Dictionary<string, IProcessableObjectInfo>();
            if (DatabaseObject != null)
            {
                //Cube cube = (Cube)database.Cubes.Item(cubeId);
                Cube cube = GetCubeById(cubeId);
                if (cube != null && !cube.IsVirtual)
                {
                    foreach (Partition partition in cube.Partitions)
                    {
                        Microsoft.AnalysisServices.AnalysisState state;
                        state = ProcessorUtils.ConvertToAS2005State(partition.State);
                        DateTime lastProcessed;
                        switch (state)
                        {
                            case Microsoft.AnalysisServices.AnalysisState.PartiallyProcessed:
                            case Microsoft.AnalysisServices.AnalysisState.Processed:
                                lastProcessed = partition.LastProcessed;
                                break;
                            case Microsoft.AnalysisServices.AnalysisState.Unprocessed:
                            default:
                                lastProcessed = SqlDateTime.MinValue.Value;
                                break;
                        }
                        ProcessableObjectInfo processableObjectInfo = new ProcessableObjectInfo(
                            GetProperty(cube.CustomProperties, "FullName", String.Empty),
                            GetProperty(cube.CustomProperties, "ObjectKey", String.Empty),
                            OlapObjectType.Partition, DatabaseId, DatabaseName,
                            cubeId, cube.Name,
                            GetProperty(cube.CustomProperties, "MeasureGroupID", cube.Name), cube.Name,
                            GetProperty(partition.CustomProperties, "ID", partition.Name), partition.Name,
                            Microsoft.AnalysisServices.ProcessType.ProcessFull, state, lastProcessed,
                            GetProperty(cube.CustomProperties, "Revision", String.Empty),
                            GetProperty(cube.CustomProperties, "BatchOperations", String.Empty));

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
                if (cube == null)
                {
                    Trace.TraceWarning("Не найден куб по его идентификатору \"{0}\".", cubeId);
                }
            }
            return objectToProcessList;
        }

        public override Dictionary<string, IProcessableObjectInfo> GetCubeDimensions(string cubeId)
        {
            return GetCubeDimensions(cubeId, new Dictionary<string, IProcessableObjectInfo>());
        }        

        /// <summary>
        /// Добавляет в общий список измерений измерения куба, если их там еще нет.
        /// </summary>
        /// <param name="cubeId">Идентификатор куба.</param>
        /// <param name="dimensionList">Список измерений, куда добавлять.</param>
        /// <returns>Обновленный список измерений.</returns>
        public override Dictionary<string, IProcessableObjectInfo> GetCubeDimensions(
            string cubeId, Dictionary<string, IProcessableObjectInfo> dimensionList)
        {            
            if (DatabaseObject != null)
            {                
                //Cube cube = (Cube)database.Cubes.Item(cubeId);
                Cube cube = GetCubeById(cubeId);
                if (cube != null)
                {
                    foreach (CubeDimension cubeDimension in cube.Dimensions)
                    {
                        if (null == GetByName(dimensionList, cubeDimension.Name))
                        {
                            //Получаем общее измерение - оно необходимо, чтобы получить идентификатор.
                            Dimension dimension = (Dimension)DatabaseObject.Dimensions.Item(cubeDimension.Name);
                            ProcessableObjectInfo processableObjectInfo = new ProcessableObjectInfo(
                                String.Empty, Guid.Empty.ToString(),
                                OlapObjectType.Dimension, DatabaseId, DatabaseName,
                                GetProperty(dimension.CustomProperties, "ID", dimension.Name), dimension.Name,
                                Microsoft.AnalysisServices.ProcessType.ProcessFull);
                            dimensionList.Add(processableObjectInfo.ObjectID, processableObjectInfo);
                        }                        
                    }
                }
            }
            return dimensionList;
        }

        private Partition GetPartition(string cubeId, string objectId)
        {
            try
            {
                Cube cube = (Cube)this.DatabaseObject.Cubes.Item(cubeId);
                if (null != cube)
                {
                    return (Partition)cube.Partitions.Item(objectId);
                }
                return null;
            }
            catch (Exception)
            {
                return null;                
            }
            
        }

        internal override void RefreshState(OlapObjectType objectType, string cubeId, string measureGroupId, string objectId)
        {
            DataRow dataRow = GetDataRow(objectId);
            if (null != dataRow)
            {
                DateTime lastProcessed;
                AnalysisState analysisState =
                    GetState(objectType, cubeId, measureGroupId, measureGroupId, out lastProcessed);
                SetState(dataRow, analysisState, lastProcessed);
            }
        }

        protected override Microsoft.AnalysisServices.AnalysisState GetState(OlapObjectType objectType,
            string cubeId, string measureGroupId, string objectId, out DateTime lastProcessed)
        {            
            switch (objectType)
            {
                case OlapObjectType.Cube:
                    Cube cube = (Cube)DatabaseObject.Cubes.Item(objectId);
                    lastProcessed = cube.LastProcessed;
                    return ProcessorUtils.ConvertToAS2005State(cube.State);                    
                case OlapObjectType.Partition:
                    Partition partition = GetPartition(cubeId, objectId);
                    lastProcessed = partition.LastProcessed;
                    return ProcessorUtils.ConvertToAS2005State(partition.State);                    
                case OlapObjectType.Dimension:
                    Dimension dimension = (Dimension)DatabaseObject.Dimensions.Item(objectId);
                    lastProcessed = dimension.LastProcessed;
                    return ProcessorUtils.ConvertToAS2005State(dimension.State);                    
                default:
                    lastProcessed = SqlDateTime.MinValue.Value;
                    return Microsoft.AnalysisServices.AnalysisState.Unprocessed;                    
            }            
        }

        public override string DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(databaseId))
                {
                    databaseId = GetProperty(DatabaseObject.CustomProperties, "ID", DatabaseName);
                }
                return databaseId;
            }
        }

        public override string DatabaseName
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.DatabaseObject.Name; }
        }

        protected override bool ObjectExist(OlapObjectType objectType, string objectId)
        {
            switch (objectType)
            {                
                case OlapObjectType.Cube:
                    return null != GetCubeById(objectId);
                case OlapObjectType.Dimension:
                    return null != GetDimensionById(objectId);
                default:
                    return false;
            }
        }        

        protected override bool ObjectExist(OlapObjectType objectType, string objectId, string parentId)
        {            
            Cube cube = GetCubeById(parentId);
            if (null != cube)
            {
                foreach (Partition item in cube.Partitions)
                {
                    string itemId = GetProperty(item.CustomProperties, "ID", null);
                    if (!string.IsNullOrEmpty(itemId) && itemId.Equals(objectId))
                    {
                        return true;
                    }
                }
            }            
            return false;            
        }
    }
}