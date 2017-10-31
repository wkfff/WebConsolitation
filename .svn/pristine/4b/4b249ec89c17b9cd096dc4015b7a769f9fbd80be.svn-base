using System;
using System.Data;

namespace Krista.FM.ServerLibrary
{
    /// <summary>
    /// Тип результата возвращаемого запросом
    /// </summary>
    public enum QueryResultTypes
    {
        NonQuery,
        DataTable,
        DataSet,
        Scalar,
        StoredProcedure
    }

    /// <summary>
    /// Интерфейс для работы с отсоединенными ADO.NET компонентами
    /// </summary>
    public interface IDataUpdater : IDisposable
    {
        /// <summary>
        /// Заполняет DataTable данными из базы данных
        /// </summary>
        /// <returns>Количество закаченных строк</returns>
        int Fill(ref DataTable dataTable);

        int Fill(ref DataSet dataSet);

        int Fill(ref DataSet dataSet, string tableName);

        /// <summary>
        /// Записывает измененные данные обратно в БД
        /// </summary>
        /// <returns>Количество обработанных строк</returns>
        int Update(ref DataTable dataTable);

        int Update(ref DataSet dataSet);

        int Update(ref DataSet dataSet, string tableName);
    }

    /// <summary>
    /// Интерфейс доступа к данным
    /// </summary>
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// Выполнение запроса с получением результатов
        /// </summary>
        /// <param name="queryText">Текст SQL-запроса</param>
        /// <param name="queryResultType">Тип результата возвращаемого запросом</param>
        /// <param name="parameters">Набор параметров запроса</param>
        /// <returns>Результат выполнения запроса</returns>
        Object ExecQuery(String queryText, QueryResultTypes queryResultType, params IDbDataParameter[] parameters);
        /// <summary>
        /// Выполнение запроса с получением результатов
        /// </summary>
        /// <param name="queryText">Текст SQL-запроса</param>
        /// <param name="queryResultType">Тип результата возвращаемого запросом</param>
        /// <param name="maxRecordCountInResult">Максимальное количество записей, возвращаемое запросом</param>
        /// <param name="parameters">Набор параметров запроса</param>
        /// <returns>Результат выполнения запроса</returns>
        Object ExecQuery(String queryText, QueryResultTypes queryResultType, int? maxRecordCountInResult, params IDbDataParameter[] parameters);
        /// <summary>
        /// Выборка данных с помощью запроса SELECT
        /// </summary>
        /// <param name="queryText">текст запроса</param>
        /// <param name="maxRecordCountInResult">максимальное количество записей</param>
        /// <param name="parameters">параметры запроса</param>
        /// <returns>DataTable с данными</returns>
        DataTable SelectData(String queryText, int? maxRecordCountInResult, params IDbDataParameter[] parameters);

        /// <summary>
        /// Запуск транзакции
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Запуск транзакции в заданном режиме
        /// </summary>
        /// <param name="il">Режим транзакции</param>
        void BeginTransaction(IsolationLevel il);

        /// <summary>
        /// Завершение транзакции
        /// </summary>
        void Commit();

        /// <summary>
        /// Откат транзакции
        /// </summary>
        void Rollback();

        /// <summary>
        /// Получение следующего значения генератора
        /// </summary>
        /// <param name="generatorName">Имя генератора</param>
        /// <returns>Значение генератора</returns>
        int GetGenerator(string generatorName);

        IDataUpdater GetDataUpdater(string queryText);
        IDataUpdater GetDataUpdater(string queryText, string insertText, string updateText, string deleteText);
        IDataUpdater GetDataUpdater(string tableName, IDataAttributeCollection attributes);
        IDataUpdater GetDataUpdater(string tableName, IDataAttributeCollection attributes,
            string selectFilter, int? maxRecordCountInSelect, IDbDataParameter[] selectFilterParameters);

        /// <summary>
        /// Создает параметр с указанным именем и значением. Тип параметра определяется по типу значения
        /// </summary>
        /// <param name="name">Имя параметра. Должно бать уникальным и начинаться с буквы</param>
        /// <param name="value">Значение параметра</param>
        /// <returns>Созданный параметр</returns>
        IDbDataParameter CreateParameter(string name, object value);

        /// <summary>
        /// Создает параметр с указанным именем, значением и типом.
        /// </summary>
        /// <param name="name">Имя параметра. Должно бать уникальным и начинаться с буквы</param>
        /// <param name="value">Значение параметра</param>
        /// <param name="dbType">Тип параметра</param>
        /// <returns></returns>
        /// <returns>Созданный параметр</returns>
        IDbDataParameter CreateParameter(string name, object value, DbType dbType);

        /*		System.Data.IDbConnection Connection { get; set; }
		
                bool Connected { get; set; }
		
                string ErrorDescription { get; }
		
                string LastError { get; }
		
                string Name { get; }
		
                bool ShowFullSQLError { get; set; }*/

        /// <summary>
        /// Создать параметр-блоб
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="direction">Направление</param>
        /// <returns>Созданный параметр</returns>
        IDbDataParameter CreateBlobParameter(string name, ParameterDirection direction);

        /// <summary>
        /// Создать параметр-блоб
        /// </summary>
        /// <param name="name">Имя параметра</param>
        /// <param name="direction">Направление</param>
        /// <param name="parameterData">Данные</param>
        /// <returns>Созданный параметр</returns>
        IDbDataParameter CreateBlobParameter(string name, ParameterDirection direction, byte[] parameterData);
    }
}
