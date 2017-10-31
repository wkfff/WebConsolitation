using System;
using System.Data.SqlTypes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.OLAP.BatchOperations
{
    /// <summary>
    /// Операция выполняемая в рамках пакета.
    /// </summary>
    public abstract class BatchOperationAbstract
    {
        protected DateTime startTime = SqlDateTime.MinValue.Value;
        protected DateTime finishTime = SqlDateTime.MinValue.Value;
        protected Guid batchId;
        protected IMDProcessingProtocol protocol;

        protected BatchOperationAbstract(Guid batchId, IMDProcessingProtocol protocol)
        {
            this.batchId = batchId;
            this.protocol = protocol;
        }

        /// <summary>
        /// Наименование операции.
        /// </summary>
        public abstract string Name { get; }
        
        /// <summary>
        /// Запускает операцию на выполнение.
        /// </summary>
        public abstract string Execute();
        
        /// <summary>
        /// Время начала операции.
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime;}
        }
        
        /// <summary>
        /// Время завершения операции.
        /// </summary>
        public DateTime FinishTime
        {
            get { return finishTime; }
        }

        /// <summary>
        /// Время выполнения операции.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get { return finishTime - startTime; }
        }

        public Guid BatchID
        {
            get { return batchId; }
        }
    }
}
