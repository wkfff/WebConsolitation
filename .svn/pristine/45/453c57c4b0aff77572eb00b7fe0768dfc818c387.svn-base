using System;
using System.Data;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Server.Dashboards.Core.DataProviders
{
    //часть класса DataProvider по установке и управлению подключением к кногомерке
    public partial class DataProvider
    {
        /// <summary>
        /// Попытка законнектится
        /// </summary>
        /// <returns>Флаг успешности</returns>        
        private bool TryConnect()
        {
            if (connection != null && 
                connection.State == ConnectionState.Open)
            {
                connection.Dispose();
                Trace.TraceVerbose("Подключение к {0} существовало, убили его", ConnectionString);
            }
            connection = new AdomdConnection(ConnectionString);
            Trace.TraceVerbose("Попытка подключения к {0}", ConnectionString);
            try
            {
                connection.Open();
                Trace.TraceVerbose("Успешное подключение к {0}", ConnectionString);
                return true;
            }
            catch (AdomdErrorResponseException e)
            {
                if (String.IsNullOrEmpty(e.Message))
                {
                    throw new Exception(string.Format("Не удалось подключиться к {0}", ConnectionString), e);
                }
                else
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Проверка подключения
        /// </summary>
        /// <returns>Флаг успешности</returns>        
        private bool CheckConnect()
        {
            lastError = string.Empty;
              
            //даже формально состояние сеанса не "открыто"
            if (connection == null || 
                connection.State != ConnectionState.Open) 
            {
                Trace.TraceVerbose("Состояние сеанса не 'открыто'");
                bool succes = TryConnect();
                Trace.TraceVerbose("Успех = {0}; Ошибки = {1}", succes, lastError);
                //попытка подключиться заново       
                return succes;
            }
            else //формально сеанс открыт, но...
            {
                Trace.TraceVerbose("Состояние сеанса 'открыто'");
                //избегаем попыток обмана с закэшированными данными
                try
                {
                    Trace.TraceVerbose("Избегаем попыток обмана с закэшированными данными");
                    string dataBase = connection.Database;
                }
                catch
                {
                    //сеанс устарел - попытка подключиться
                    Trace.TraceVerbose("Сеанс устарел - попытка подключиться");
                    bool succes = TryConnect();
                    Trace.TraceVerbose("Успех = {0}; Ошибки = {1}", succes, lastError);
                    return succes;
                }
            }
            Trace.TraceVerbose("Повторного подключения не требуется");
            return true;
        } 
       
        public void DisposeConnection()
        {
            if (connection != null &&
                connection.State == ConnectionState.Open)
            {
                connection.Dispose();
            }
        }
    }
}
