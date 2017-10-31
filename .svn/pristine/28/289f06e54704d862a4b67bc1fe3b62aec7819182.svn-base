using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // Модуль шаблонов наиболее употребимых функций этапа обработки

    /// <summary>
    /// Базовый класс для всех закачек.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {
        #region Общие функции обработки источников данных

        /// <summary>
        /// Функция запроса данных из базы
        /// </summary>
        protected virtual void QueryDataForProcess()
        {

        }

        /// <summary>
        /// Функция обработки закачанных данных
        /// </summary>
        protected virtual void ProcessDataSource()
        {

        }

        /// <summary>
        /// Функция сохранения закачанных данных в базу
        /// </summary>
        protected virtual void UpdateProcessedData()
        {

        }

        /// <summary>
        /// Функция выполнения завершающих действий закачки
        /// </summary>
        protected virtual void ProcessFinalizing()
        {

        }

        /// <summary>
        /// Действия, выполняемые после обработки данных
        /// </summary>
        protected virtual void AfterProcessDataAction()
        {

        }

        /// <summary>
        /// Проверяет текущий истоник на соответствие параметрам закачки
        /// </summary>
        /// <param name="year">Год</param>
        /// <param name="month">Месяц</param>
        private bool CheckDataSource(IDataSource ds, int year, int month)
        {
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
            {
                if (year > 0 && ds.Year != year) return false;

                if (ds.ParametersType == ParamKindTypes.YearMonth ||
                    ds.ParametersType == ParamKindTypes.YearMonthVariant)
                {
                    if (month > 0 && ds.Month != month)
                        return false;
                }
            }

            return true;
        }

        #endregion Общие функции обработки источников данных

        private string GetProcessParamsDescription(int year, int month)
        {
            if (year <= 0)
                return string.Empty;
            if (month <= 0)
                return string.Format("(Год: {0})", year);
            else
                return string.Format("(Год: {0}, Месяц: {1})", year, CommonRoutines.MonthByNumber[month - 1]);
        }

        /// <summary>
        /// Функция обработки закачанны источников, если они есть, или всех, если закачанных нет.
        /// </summary>
        /// <param name="year">Параметр "Год"</param>
        /// <param name="month">Параметр "Месяц"</param>
        protected void ProcessDataSourcesTemplate(int year, int month, string message)
        {
            string processParamsDescription = GetProcessParamsDescription(year, month);
            // Пишем в протокол о начале обработки
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart, 
                string.Format("{0} {1} - Старт обработки.", message, processParamsDescription));

            // Если это не первый этап закачки, то обрабатываем все закачанные за текущий сеанс данные
            // иначе считываем хмл-параметры и обрабатываем все.
            Dictionary<int, string> dataSources = null;
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
            {
                dataSources = GetAllPumpedDataSources();
            }
            else
            {
                // А есть ли данные для обработки?
                if (this.PumpID < 0 || this.PumpedSources.Count == 0)
                {
                    WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeCriticalError, 
                        string.Format("{0} - Нет данных.", message));
                    return;
                }

                dataSources = this.PumpedSources;
            }

            SortDataSources(ref dataSources);

            // Признак, были ли обработаны данные
            bool wasProcessed = false;
            foreach (KeyValuePair<int, string> dataSource in dataSources)
            {
                if (!CheckDataSource(GetDataSourceBySourceID(dataSource.Key), year, month)) continue;

                wasProcessed = true;
                SetDataSource(dataSource.Key);

                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.dpeStartDataSourceProcessing, 
                    string.Format("{0} - Старт обработки по данным источника ID {1}.", message, this.SourceID));

                BeginTransaction();

                try
                {
                    SetPresentationContexts();
                    try
                    {
                        QueryDataForProcess();

                        // добавление версий объектов
                        WriteToTrace("Добавление версий классификаторов...", TraceMessageKind.Information);
                        SetClsVersion();
                        WriteToTrace("Добавление версий классификаторов окончено.", TraceMessageKind.Information);

                        ProcessDataSource();
                    }
                    finally
                    {
                        ClearPresentationContexts();
                    }

                    UpdateProcessedData();

                    CommitTransaction();
                    
                    this.DataSourcesProcessingResult.AddToPumpedSources(this.SourceID, string.Empty);

                    AfterProcessDataAction();

                    WriteEventIntoProcessDataProtocol(ProcessDataEventKind.dpeSuccessfullFinishDataSourceProcess,
                        string.Format("{0} по данным источника ID {1} успешно завершено.", message, this.SourceID));
                }
                catch (ThreadAbortException)
                {
                    AfterProcessDataAction();

                    RollbackTransaction();

                    throw;
                }
                catch (Exception ex)
                {
                    AfterProcessDataAction();

                    WriteEventIntoProcessDataProtocol(ProcessDataEventKind.dpeFinishDataSourceProcessingWithError,
                        string.Format("{0} по данным источника ID {1} завершено с ошибками", message, this.SourceID), ex);

                    RollbackTransaction();
                }
                finally
                {
                    ProcessFinalizing();
                    this.DataSource = null;
                    CollectGarbage();
                }
            }

            // Если данные не были обработаны, то скорее всего их нет
            // Такое возможно, когда этап обработки запускается отдельно с индивидуальными параметрами
            if (!wasProcessed)
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning,
                    string.Format("{0} не выполнено, так как отсутствуют данные по источнику (Год: {1}, Месяц: {2}).", message, year, month));

            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeSuccefullFinished,
                string.Format("{0} {1} завершено.", message, processParamsDescription));
        }

        /// <summary>
        /// Функция обработки закачанны источников, если они есть, или всех, если закачанных нет.
        /// </summary>
        /// <param name="year">Параметр "Год"</param>
        /// <param name="month">Параметр "Месяц"</param>
        protected void ProcessDataSourcesTemplate(string message)
        {
            ProcessDataSourcesTemplate(0, 0, message);
        }
    }
}
