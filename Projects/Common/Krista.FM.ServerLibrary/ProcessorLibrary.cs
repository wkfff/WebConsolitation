using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.AnalysisServices;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.ProcessorLibrary
{
    /// <summary>
    /// Фасад менеджера расчета кубов.
    /// </summary>
	public interface IProcessor : IDisposable
	{
		/// <summary>
		/// Рассчет куба. В зависимости от типа многомерной базы (определяется автоматически) используется либо интерфейс DSO (AS2000), либо интерфейс AMO (SSAS2005)
		/// </summary>
		/// <param name="processTypes">Как расчитывать.</param>
		/// <param name="cubeName">Что рассчитывать.</param>
        void ProcessCube(IEntity entity, ProcessTypes processTypes, string cubeName);

        /// <summary>
        /// Создает новый пакет расчета объектов.
        /// </summary>
        /// <returns>ID пакета.</returns>
        Guid CreateBatch();

        /// <summary>
        /// Возвращает состояние пакета.
        /// Метод необходим для определения результата асинхронной операции обработки пакета.
        /// </summary>
        /// <param name="batchId">ID пакета.</param>
        /// <returns>Состояние пакета.</returns>
	    BatchState GetBatchState(Guid batchId);

        /// <summary>
        /// Удаляет пакет находящийся в состоянии создания.
        /// </summary>
        /// <remarks>
        /// Метод должен вызываться если в процессе формирования пакета произошла ошибка.
        /// Если пакет имеет состояние отличное от "Создан", то метод вернет исключение.
        /// </remarks>
        /// <param name="batchId">ID пакета.</param>
        void RevertBatch(Guid batchId);

        void InvalidatePartition(string objectKey, string moduleName, InvalidateReason invalidateReason, string cubeName);
        void InvalidatePartition(string objectKey, string moduleName, InvalidateReason invalidateReason, string cubeName, string partitionName);
        void InvalidateDimension(string objectKey, string moduleName, InvalidateReason invalidateReason, string dimensionName);

        void InvalidatePartition(string objectKey, string moduleName, InvalidateReason invalidateReason, string cubeName, Guid batchGuid);
        void InvalidatePartition(string objectKey, string moduleName, InvalidateReason invalidateReason, string cubeName, string partitionName, Guid batchGuid);
        void InvalidateDimension(string objectKey, string moduleName, InvalidateReason invalidateReason, string dimensionName, Guid batchGuid);

        void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName);
        void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName, string partitionName);
        void InvalidateDimension(IEntity entity, string moduleName, InvalidateReason invalidateReason, string dimensionName);

        void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName, Guid batchGuid);
        void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName, string partitionName, Guid batchGuid);
        void InvalidateDimension(IEntity entity, string moduleName, InvalidateReason invalidateReason, string dimensionName, Guid batchGuid);

        void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName, int pumpID, int sourceID, Guid batchGuid);
        void InvalidatePartition(IEntity entity, string moduleName, InvalidateReason invalidateReason, string cubeName, string partitionName, int pumpID, int sourceID, Guid batchGuid);
        void InvalidateDimension(IEntity entity, string moduleName, InvalidateReason invalidateReason, string dimensionName, int pumpID, int sourceID, Guid batchGuid);
        void InvalidateDimension(IEntity entity, string moduleName, InvalidateReason invalidateReason, string dimensionName, int pumpID, int sourceID, Guid batchGuid, ProcessType processType);

        IOlapDBWrapper OlapDBWrapper { get; }        
        IProcessManager ProcessManager { get; }
        IOlapDatabaseGenerator OlapDatabaseGenerator { get; }
	}

    /// <summary>
    /// Режим расчета для SSAS 2005
    /// </summary>
    public enum ProcessMode
    {
        /// <summary>
        /// Параллельный режим расчета 
        /// </summary>
        ParallelMode = 0,
        /// <summary>
        /// Последовательный режим расчета
        /// </summary>
        SequentialMode = 1
    }

    /// <summary>
    /// Режим управления тракзакциями для SSAS 2005
    /// </summary>
    public enum TransactionMode
    {
        /// <summary>
        /// Параллельный режим расчета 
        /// </summary>
        OneTransaction = 0,
        /// <summary>
        /// Последовательный режим расчета
        /// </summary>
        SeparateTransaction = 1
    }

    /// <summary>
    /// Интерфейс, представляющий многомерную базу в виде датасета с тремя таблицами: измерения, кубы, разделы кубов.
    /// </summary>
    public interface IOlapDBWrapper : IDisposable
    {
		/// <summary>
		/// Возвращает многомерные объекты их кэша.
		/// </summary>
		/// <param name="filter">Фильтр отбора многомерных объектов.</param>
        DataTable GetPartitions(string filter);
        
		/// <summary>
		/// Возвращает все пакеты диспетчера обработки многомерных объектов.
		/// </summary>
		DataTable BatchesView { get; }

        /// <summary>
        /// Возвращает количество пакетов в очереди на обработку.
        /// </summary>
        int BatchQueueCount { get; }

		/// <summary>
		/// Возвращает количество ошибок в многомерной базе.
		/// </summary>
		int GetDatabaseErrorsCount();

		/// <summary>
		/// Возвращает таблицу ошибок в многомерной базе.
		/// </summary>
		DataTable GetDatabaseErrors();

        void RefreshOlapObjects();
        void RefreshOlapObjects(RefreshOlapDataSetOptions refreshOptions);
        string DatabaseId { get; }
        string DatabaseName { get; }
        
        /// <summary>
        /// Возвращает идентификатор пакета для объекта с заданным id.
        /// </summary>        
        string GetBatchGuid(string objectId);
        
        void UpdateValues(DataSet changedValues);
        void UpdateValues(ref DataTable changedValues);
        void UpdateBatchValues(ref DataTable changedValues);
    }

    ///// <summary>
    ///// Интрефейс, обслуживающий накопитель - т.е. хранилище с запросами на расчет кубов и измерений.
    ///// </summary>
    //public interface IAccumulatorManager : IDisposable
    //{        
    //    IEnumerable<int> AddRange(IProcessableObjectInfo objectInfo);
    //    IEnumerable<int> AddRangeAndProcess(IProcessableObjectInfo objectInfo);
    //    IEnumerable<int> AddRange(IEnumerable<IProcessableObjectInfo> objectInfoList);
    //    IEnumerable<int> AddRangeAndProcess(IEnumerable<IProcessableObjectInfo> objectInfoList);
    //    IEnumerable<int> AddRange(DataSet dsSelectedObjects);
    //    IEnumerable<int> AddRangeAndProcess(DataSet dsSelectedObjects);
    //    void SetRecordStatus(OlapObjectType objectType, string cubeId, string measureGroupId, string objectId, RecordStatus oldRecordStatus, RecordStatus newRecordStatus);
    //    void SetRecordStatus(OlapObjectType objectType, string cubeId, string measureGroupId, string objectId, RecordStatus oldRecordStatus, RecordStatus newRecordStatus, string errorMessage);
    //    void RefreshTableAccumulator();

    //    /// <summary>
    //    /// Создает пустой пакет.
    //    /// </summary>
    //    /// <returns>Идентификатор пакета.</returns>
    //    int CreateBatch();

    //    /// <summary>
    //    /// Создает пакет и включает в него те записи из накопителя, идентификаторы которых были переданы.
    //    /// </summary>
    //    /// <param name="accumulatorKeyList">Список идентификаторов записей из накопителя.</param>
    //    /// <returns>Идентификатор пакета.</returns>
    //    int CreateBatch(IEnumerable<int> accumulatorKeyList);

    //    void AddBatchDetail(int batchId, int accumulatorKey);
    //    void AddBatchDetail(int batchId, IEnumerable<int> accumulatorKeyList);
    //    bool StartBatch(int batchId, string sessionId);
    //    string CancelBatch(int batchId);
    //    int ComplitBatch(string sessionId);
    //    void DeleteBatch(int batchId);

    //    int GetBatchIndex(int batchId);
    //    int[] GetBatchRows(int batchId);

    //    DataSet DSAccumulator { get; }
    //}

    public interface IProcessManager : IDisposable
    {
        void StartBatch(Guid batchGuid);
        void StopBatch(Guid batchGuid);

        bool Paused { get; set; }

        ProcessMode ProcessMode { get; set; }
        TransactionMode TransactionMode { get; set; }
    }

    public interface IProcessableObjectInfo
    {
        string DatabaseId { get; }
        string DatabaseName { get; }        
        string CubeId { get; }
        string CubeName { get; }        
        string MeasureGroupId { get; }
        string MeasureGroupName { get; }
        string ObjectID { get; }
        string ObjectName { get; }
        string FullName { get; set; }
        string ObjectKey { get; set; }
        Microsoft.AnalysisServices.ProcessType ProcessType { get; set; }
        OlapObjectType ObjectType { get; }
        Microsoft.AnalysisServices.AnalysisState State { get; set; }
        DateTime LastProcessed { get; set; }
        string ProcessResult { get; set; }
        RecordStatus RecordStatus { get; set; }
        string Revision { get; set; }
        string BatchOperations { get; set; }
    }

    /// <summary>
    /// Интерфейс для генерации тех объектов многомерной базы, которых нет в схеме сервера.
    /// </summary>
    public interface IOlapDatabaseGenerator
    {
        void GenerateMRRDimensions(string serverName, string databaseName, string dataSourceName,
            out string logString, out int totalCount, out int errorCount);
    }

    public enum RecordStatus
    {
        Waiting = 0,
        ComplitedWithErrors = 1,
        InBatch = 2,
        InProcess = 3,        
        CanceledByUser = 4,
        CanceledByOptimization = 5,
        ComplitedSuccessful = 6,
    }

    public enum InvalidateReason
    {
        WriteBack = 0,
        ClassifierChanged = 1,
        AssociationChanged = 2,
        UserPleasure = 3,
        DataPump = 4,
    }

    /// <summary>
    /// Состояния пакетов.
    /// </summary>
    public enum BatchState
    {
        /// <summary>
        /// Создан (начальное состояние).
        /// </summary>
        Created = 0,
        
        /// <summary>
        /// Ожидание.
        /// </summary>
        Waiting = 1,

        /// <summary>
        /// Выполняется.
        /// </summary>
        Running = 2,

        /// <summary>
        /// Завершен успешно (конечное состояние).
        /// </summary>
        Complited = 3,
        
        /// <summary>
        /// Отложен.
        /// </summary>
        Canceled = 4,

        /// <summary>
        /// Завершен с ошибкой (конечное состояние).
        /// </summary>
        ComplitedWithError = 5,

        /// <summary>
        /// Удален (конечное состояние).
        /// </summary>
        Deleted = 6
    }

    public enum OlapObjectType
    {
        Database,
        Cube,
        MeasureGroup,
        Partition,
        Dimension,
    }

    public enum RefreshOlapDataSetOptions
    {
        OnlyIfDatasetIsEmpty,
        Always,
    }

    public enum BatchStartPriority
    {
        Auto,
        Immediately,
    }

    public enum SetNeedProcessOptions
    {
        Auto,
        FixedValue,
    }

    /// <summary>
    /// Режим распределения объектов по пакетам
    /// </summary>
    public enum ProcessBatchMode
    {
        /// <summary>
        /// Каждый объект в отдельном пакете
        /// </summary>
        Separate,
        /// <summary>
        /// Все объекты вместе - одним пакетом
        /// </summary>
        Together
    }

    /// <summary>
    /// Режим обработки объектов
    /// </summary>
    public enum ProcessObjectMode
    {
        /// <summary>
        /// Выделенные
        /// </summary>
        Selected,
        /// <summary>
        /// Все требующие расчета
        /// </summary>
        AllNeedProcess
    }

    public static class ProcessorEnumsConverter
    {
        public static string GetBatchStateName(BatchState batchState)
        {
            switch (batchState)
            {
                case BatchState.Created: return "Создан";
                case BatchState.Waiting: return "Ожидание";
                case BatchState.Running: return "Выполняется";
                case BatchState.Complited: return "Завершен успешно";
                case BatchState.Canceled: return "Отложен";
                case BatchState.ComplitedWithError: return "Завершен с ошибкой";
                case BatchState.Deleted: return "Удален";
                default: return String.Empty;
            }
        }

        public static string GetBatchPriorityName(BatchStartPriority priority)
        {
            switch (priority)
            {
                case BatchStartPriority.Auto: return "Низкий";
                case BatchStartPriority.Immediately: return "Высокий";
                default: return String.Empty;
            }
        }
    }
}
