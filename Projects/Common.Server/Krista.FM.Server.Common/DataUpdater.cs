using System;
using System.Data;
using System.Data.Common;

using Krista.FM.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Common
{
	[Serializable]
	public delegate void AfterFillDataTableEventDelegate(DataTable dataTable);

	[Serializable]
	public delegate void AfterFillDataSetEventDelegate(DataSet dataSet, string tableName);

	[Serializable]
    public delegate bool BeforeUpdateEventDelegate(ref DataTable dataTable);

    [Serializable]
    public delegate void AfterUpdateEventDelegate();

    [Serializable]
    public delegate bool InsteadUpdateEventDelegate(IDatabase db, DataRow dataRow);

    [ExceptionLoggingContextAttribute(@"C:\exceptions.log")]
    public class DataUpdater : DisposableObject, IDataUpdater
    {
        private static object g_identifier = 0;
		private int identifier = -1;

        //static PerformanceCounter counter;

        private DbProviderFactory factory;
        private IDbDataAdapter dataAdapter;
        internal DbCommandBuilder commandBuilder;
        // Транзакция в которой производится обновление данных в БД
        private IDbTransaction transaction;

		public event AfterFillDataTableEventDelegate OnAfterFillDataTable;
		public event AfterFillDataSetEventDelegate OnAfterFillDataSet;
		//public event BeforeUpdateEventDelegate OnBeforeUpdate;
        public event AfterUpdateEventDelegate OnAfterUpdate;
        public event InsteadUpdateEventDelegate OnInsteadUpdate;

        private bool sessionContext;


        public DataUpdater(DbProviderFactory factory)
        {
			lock (g_identifier)
			{
				g_identifier = identifier = Convert.ToInt32(g_identifier) + 1;
				Trace.TraceVerbose("new {0}({1}) {2} {3}", GetType().FullName, identifier, Authentication.UserName, DateTime.Now);
				//counter.Increment();
			}

			this.factory = factory;
            dataAdapter = factory.CreateDataAdapter();
            commandBuilder = factory.CreateCommandBuilder();
            if (commandBuilder != null) 
                commandBuilder.DataAdapter = (DbDataAdapter)dataAdapter;
        }

        public DataUpdater(IDbDataAdapter da, DbCommandBuilder cb, DbProviderFactory factory, bool sessionContext)
            : this(factory)
        {
            this.sessionContext = sessionContext;
			if (this.sessionContext) 
				SessionContext.RegisterObject(this);

            dataAdapter = da;
            commandBuilder = cb;

			if (cb != null) commandBuilder.DataAdapter = (DbDataAdapter)dataAdapter;
        }

        public DataUpdater(IDbDataAdapter da, DbCommandBuilder cb, IDatabase db)
            : this(da, cb, ((Database)db).Factory, true)
        {

        }

        protected override void Dispose(bool disposing)
        {
			Trace.TraceVerbose("~{0}({1} {2}) {3} {4}", GetType().FullName, disposing, identifier, Authentication.UserName, DateTime.Now);
            // Синхронизируем потоки для запрета одновременного вызова Dispose/Close
            lock (this)
            {
                if (disposing)
                {
                    // Здесь еще можно обращаться к полям, ссылающимся 
                    // на другие объекты - это безопасно для кода, так как для
                    // этих объектов метод Finalize еще не вызван
                    if (this.commandBuilder != null)
                        this.commandBuilder.Dispose();

                    if (this.dataAdapter.SelectCommand != null)
                        this.dataAdapter.SelectCommand.Dispose();

                    if (this.dataAdapter.InsertCommand != null)
                        this.dataAdapter.InsertCommand.Dispose();

                    if (this.dataAdapter.UpdateCommand != null)
                        this.dataAdapter.UpdateCommand.Dispose();

                    if (this.dataAdapter.DeleteCommand != null)
                        this.dataAdapter.DeleteCommand.Dispose();

                    ((DbDataAdapter)this.dataAdapter).Dispose();
                    //counter.Decrement();

                    if (this.sessionContext) SessionContext.UnregisterObject(this);
                }
                else
                {
                }

                // Здесь должно выполняться уничножение/закрытие неупровляемых ресурсов
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Транзакция в которой производится обновление данных в БД
        /// </summary>
		public IDbTransaction Transaction
		{
			get { return this.transaction; }
			set
			{
				this.transaction = value;
				if (this.dataAdapter.SelectCommand != null)
					this.dataAdapter.SelectCommand.Transaction = this.transaction;
				if (this.dataAdapter.UpdateCommand != null)
					this.dataAdapter.UpdateCommand.Transaction = this.transaction;
				if (this.dataAdapter.DeleteCommand != null)
					this.dataAdapter.DeleteCommand.Transaction = this.transaction;
				if (this.dataAdapter.InsertCommand != null)
					this.dataAdapter.InsertCommand.Transaction = this.transaction;
			}
		}

        internal IDbCommand SelectCommand
        {
            get { return this.dataAdapter.SelectCommand; }
            set 
			{ 
				this.dataAdapter.SelectCommand = value;
				this.dataAdapter.SelectCommand.Transaction = this.transaction;
			}
        }

        internal IDbCommand InsertCommand
        {
            get { return this.dataAdapter.InsertCommand; }
            set 
			{ 
				this.dataAdapter.InsertCommand = value;
				this.dataAdapter.InsertCommand.Transaction = this.transaction;
			}
        }

        internal IDbCommand UpdateCommand
        {
            get { return this.dataAdapter.UpdateCommand; }
            set 
			{ 
				this.dataAdapter.UpdateCommand = value;
				this.dataAdapter.UpdateCommand.Transaction = this.transaction;
			}
        }

        internal IDbCommand DeleteCommand
        {
            get { return this.dataAdapter.DeleteCommand; }
            set 
			{ 
				this.dataAdapter.DeleteCommand = value;
				this.dataAdapter.DeleteCommand.Transaction = this.transaction;
			}
        }

        #region IDataUpdater Members

        public int Fill(ref DataTable dataTable)
        {
            int rowsAffected;

            dataTable.Clear();
            rowsAffected = ((DbDataAdapter)dataAdapter).Fill(dataTable);
			
			if (OnAfterFillDataTable != null)
			{
				OnAfterFillDataTable(dataTable);
			}
            
			if (dataTable.DataSet == null)
                dataTable.RemotingFormat = SerializationFormat.Binary;
            return rowsAffected;
        }

        public int Fill(ref DataSet dataSet)
        {
            return Fill(ref dataSet, String.Empty);
        }

        public int Fill(ref DataSet dataSet, string tableName)
        {
            int rowsAffected;

            if (dataSet == null)
                throw new ArgumentNullException("dataSet");

            if (!String.IsNullOrEmpty(tableName))
                rowsAffected = ((DbDataAdapter)dataAdapter).Fill(dataSet, tableName);
            else
                rowsAffected = ((DbDataAdapter)dataAdapter).Fill(dataSet);

			if (OnAfterFillDataTable != null)
			{
				OnAfterFillDataSet(dataSet, tableName);
			}

			//SetNullBlobData(dataSet);

            // по умолчанию датасет сериализуется в XML, это очень медленно на больших объемах 
            // данных и увеличивает сетевой трафик. Включаем бинарную сериализацию.
            // Согласно показаниям профайлера на маленьких датасетах (до тысячи записей)
            // прирост скорости составил ~20%
            dataSet.RemotingFormat = SerializationFormat.Binary;

            return rowsAffected;
        }

        /// <summary>
        /// Обращает в null блоб поля в датасете.
        /// </summary>
        /// <param name="dataSet">Датасет для обработки.</param>
        private void SetNullBlobData(DataSet dataSet)
        {
            foreach (DataColumn column in dataSet.Tables[0].Columns)
            {
                if (column.DataType.ToString() == "System.Byte[]")
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        row[column] = null;
                    }
                    dataSet.AcceptChanges();
                }
            }
        }

		/// <summary>
		/// Запросить у подключения транзакцию и начать ее.
		/// Вызывается только для объектов работющих без активной транзакции (например интерфейсы пользователя)
		/// </summary>
		/// <param name="transCon">указатель на подключение, открытое для получения транзакции</param>
		internal void StartTransaction(ref IDbConnection transCon)
		{
			transCon = null;
			if (this.dataAdapter.UpdateCommand != null)
				transCon = this.dataAdapter.UpdateCommand.Connection;
			else
				if (this.dataAdapter.SelectCommand != null)
					transCon = this.dataAdapter.SelectCommand.Connection;
				else
					if (this.dataAdapter.InsertCommand != null)
						transCon = this.dataAdapter.InsertCommand.Connection;
					else
						if (this.dataAdapter.DeleteCommand != null)
							transCon = this.dataAdapter.DeleteCommand.Connection;
			if (transCon == null)
				throw new InvalidProgramException("Невозможно открыть транзакцию");

            transCon = new DbConnectionWrapper((DbConnection)transCon);
			if (transCon.State != ConnectionState.Open) 
                transCon.Open();

			Transaction = transCon.BeginTransaction();
		}

		/// <summary>
		/// Закрыть активную транзакцию, освободить ресурсы
		/// </summary>
		/// <param name="transCon">указатель на подключение, открытое для получения транзакции</param>
		/// <param name="commit">принять или откатить изменения</param>
		internal void CloseAndDisposeTransaction(ref IDbConnection transCon, bool commit)
		{
			if (Transaction != null)
			{
				if (commit)
					Transaction.Commit();
				else
					Transaction.Rollback();

				Transaction.Dispose();
				Transaction = null;
				transCon.Close();
				transCon = null;
			}
		}

		public int Update(ref DataSet dataSet)
        {
            int rowsAffected = -1;

            if (dataSet == null)
                throw new ArgumentNullException("Параметр dataSet не должен быть пуст");

			// установлена ли глобальная транзакция?
			bool inLocalTransaction = (Transaction == null);

			// если нет - открываем локальную
			// для корректной очистки ресурсов запоминаем открытое для получения транзакции подключение
			IDbConnection transCon = null;
			if (inLocalTransaction) 
                StartTransaction(ref transCon);

			try
			{
                // пытаемся сохранить изменения
				
				if (OnInsteadUpdate != null)
				{
					IDatabase db;
					if (inLocalTransaction)
						db = new Database(transCon, Transaction, factory);
					else
						db = new Database(Transaction.Connection, Transaction, factory);

					try
					{
						DataTable dt = dataSet.GetChanges().Tables[0];
						if (dt != null)
						{
							foreach (DataRow row in dt.Rows)
							{
								if (OnInsteadUpdate(db, row))
								{
									row.AcceptChanges();
									rowsAffected++;
								}
							}
						}
					}
					finally
					{
						db.Dispose();
					}
				}
				else
					rowsAffected = ((DbDataAdapter)this.dataAdapter).Update(dataSet);

                // сохраняем изменения, закрываем локальную транзакцию (если она была открыта)
				if (inLocalTransaction) 
                    CloseAndDisposeTransaction(ref transCon, true);

                if (OnAfterUpdate != null)
                {
                    OnAfterUpdate();
                }
			}
			catch (Exception e)
			{
				// откатываем изменения, закрываем локальную транзакицию (если она была открыта) и подключение
				if (inLocalTransaction) 
                    CloseAndDisposeTransaction(ref transCon, false);
				
                // отдаем исключение на дальнейшую обработку
                throw new Exception(e.Message, e);
            }
			// возвращаем количество обработанных записей, в случае исключения до сюда дело не дойдет, ну да и не надо
			return rowsAffected;
        }

        public int Update(ref DataSet dataSet, string tableName)
        {
			int rowsAffected;

			if (dataSet == null)
				throw new ArgumentNullException("Параметр dataSet не должен быть пуст");

			bool inLocalTransaction = (Transaction == null);
			IDbConnection transCon = null;
			if (inLocalTransaction) 
                StartTransaction(ref transCon);
			
            try
			{
				rowsAffected = ((DbDataAdapter)this.dataAdapter).Update(dataSet, tableName);
				
                if (inLocalTransaction) 
                    CloseAndDisposeTransaction(ref transCon, true);
            
                if (OnAfterUpdate != null)
                {
                    OnAfterUpdate();
                }
            }
            catch (Exception e)
            {
				if (inLocalTransaction) 
                    CloseAndDisposeTransaction(ref transCon, false);

                throw new Exception(e.Message, e);
            }
			return rowsAffected;
        }

        /// <summary>
        /// Записывает измененные данные обратно в БД.
        /// </summary>
        /// <param name="dataTable">Таблица содержащая измененные строки</param>
        /// <returns>Количество обработанных строк</returns>
        /// <remarks>
        /// 
        /// </remarks>
        public int Update(ref DataTable dataTable)
        {
			int rowsAffected = -1;

			bool inLocalTransaction = (Transaction == null);
			IDbConnection transCon = null;

            if (inLocalTransaction) 
                StartTransaction(ref transCon);

            try
            {
                /*try
                {
                    if (OnBeforeUpdate != null)
                        if (!OnBeforeUpdate(ref dataTable))
                            return 0;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString(), Authentication.UserDate);
                    throw;
                }*/

                if (OnInsteadUpdate != null)
                {
                    IDatabase db;
                    if (inLocalTransaction)
                        db = new Database(transCon, Transaction, factory);
                    else
                        db = new Database(Transaction.Connection, Transaction, factory);

                    try
                    {
                        DataTable dt = dataTable.GetChanges();
                        if (dt != null)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                if (OnInsteadUpdate(db, row))
                                {
                                    rowsAffected++;
                                }
                            }
                            dt.AcceptChanges();
                        }
                    }
                    finally
                    {
                        db.Dispose();
                    }
                }
                else
                {
                    if (dataAdapter is System.Data.SqlClient.SqlDataAdapter)
                        ((System.Data.SqlClient.SqlDataAdapter) dataAdapter).RowUpdated += RowUpdated;
                    
                    rowsAffected = ((DbDataAdapter) dataAdapter).Update(dataTable);
                }

                if (OnAfterUpdate != null)
                {
                    OnAfterUpdate();
                }

                return rowsAffected;
            }
            catch (Exception e)
            {
                if (inLocalTransaction)
                    CloseAndDisposeTransaction(ref transCon, false);

                throw new ServerException(e.Message, e);
            }
            finally
            {
                if (inLocalTransaction)
                    CloseAndDisposeTransaction(ref transCon, true);
            }
        }

        private void RowUpdated(object sender, System.Data.SqlClient.SqlRowUpdatedEventArgs e)
        {
            if (e.RecordsAffected == 0)
            {
                Trace.TraceError(
                    String.Format(
                        "В таблице {0} при обновлении записи с ID {1} возникло исключение оптимистичного параллелизма.",
                        e.Row.Table, e.Row[0]));
                e.Status = UpdateStatus.SkipCurrentRow;
            }
        }

        #endregion
    }
}
