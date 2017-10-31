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

        #region Общие функции проверки источников данных

        protected virtual void QueryDataForCheck()
        {
            QueryData();
        }

        protected virtual void CheckDataSource()
        {

        }

        protected virtual void UpdateCheckedData()
        {
            UpdateData();
        }

        protected virtual void CheckFinalizing()
        {
            PumpFinalizing();
        }

        protected virtual void AfterCheckDataAction()
        {

        }

        #endregion Общие функции проверки источников данных

        protected void CheckDataSourcesTemplate(int year, int month, string message)
        {
            string checkParamsDescription = GetProcessParamsDescription(year, month);
            // Пишем в протокол о начале обработки
            CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeStart,
                string.Format("{0} {1} - Старт проверки.", message, checkParamsDescription), this.PumpID, this.SourceID);

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
                    CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeCriticalError,
                        string.Format("{0} - Нет данных.", message), this.PumpID, this.SourceID);
                    return;
                }
                dataSources = this.PumpedSources;
            }
            SortDataSources(ref dataSources);
            foreach (KeyValuePair<int, string> dataSource in dataSources)
            {
                if (!CheckDataSource(GetDataSourceBySourceID(dataSource.Key), year, month))
                    continue;
                SetDataSource(dataSource.Key);
                CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.dpeStartDataSourceProcessing,
                    string.Format("{0} - Старт проверки по данным источника ID {1}.", message, this.SourceID), this.PumpID, this.SourceID);

                BeginTransaction();

                try
                {
                    QueryDataForCheck();
                    CheckDataSource();
                    UpdateCheckedData();
                    CommitTransaction();
                    this.DataSourcesProcessingResult.AddToPumpedSources(this.SourceID, string.Empty);
                    AfterCheckDataAction();
                    CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.dpeSuccessfullFinishDataSourceProcess,
                        string.Format("{0} по данным источника ID {1} успешно завершено.", message, this.SourceID), this.PumpID, this.SourceID);
                }
                catch (ThreadAbortException)
                {
                    AfterCheckDataAction();
                    RollbackTransaction();
                    throw;
                }
                catch (Exception ex)
                {
                    AfterCheckDataAction();
                    CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.dpeFinishDataSourceProcessingWithError,
                        string.Format("{0} по данным источника ID {1} завершено с ошибками: {2}", message, this.SourceID, ex), this.PumpID, this.SourceID);
                    RollbackTransaction();
                }
                finally
                {
                    ProcessFinalizing();
                    this.DataSource = null;
                    CollectGarbage();
                }
            }
            CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeSuccefullFinished,
                string.Format("{0} {1} завершено.", message, checkParamsDescription), this.PumpID, this.SourceID);
        }

        protected void CheckDataSourcesTemplate(string message)
        {
            CheckDataSourcesTemplate(0, 0, message);
        }
    }
}
