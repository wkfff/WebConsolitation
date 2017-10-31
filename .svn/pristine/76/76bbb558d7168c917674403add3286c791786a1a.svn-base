using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.ProcessorLibrary;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.MobileReports.Common;
using Krista.FM.Common.Xml;


namespace Krista.FM.Server.DataPumps
{
    // Модуль функций, вызываемых во время выполнения этапов закачки (инициализация, общие функции обработки,
    // сопоставления и расчета кубов).

    /// <summary>
    /// Базовый класс для всех закачек.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject, IDataPumpModule
    {
        #region Методы этапов закачки


        #region Общие методы

        /// <summary>
        /// Инициализация массивов служебных классификаторов, если они не были установлены в закачке
        /// </summary>
        protected void InitAuxiliaryClss()
        {
            if (hierarchyClassifiers == null)
                hierarchyClassifiers = usedClassifiers;

            if (associateClassifiers == null)
                associateClassifiers = usedClassifiers;

            // массив associateClassifiersEx не инициализируется, т.к. если он не установлен,
            // то классификаторов (по источникам, отличающимся от источников закачки) нет и их сопоставлять не нужно

            if (cubeClassifiers == null)
                cubeClassifiers = usedClassifiers;

            // массив versionClassifiers не инициализируется, т.к. если он не установлен,
            // то классификаторов, требующих установки версии, нет

            if (cubeFacts == null)
                cubeFacts = usedFacts;
        }

        #endregion Общие методы


        #region Предварительный просмотр

        /// <summary>
        /// Предварительный просмотр данных закачки. 
        /// Переопределяется в потомках для выполнения действий по предпросмотру данных.
        /// </summary>
        protected virtual void DirectPreviewData()
        {
            //WriteEventIntoPreviewDataProtocol(
            //    PreviewDataEventKind.dpeStart, "На данном этапе никаких действий не выполняется.");
        }

        /// <summary>
        /// Предварительный просмотр данных закачки. 
        /// Выполняет подготовительные и завершающие действия. Вызывается главным потоком
        /// </summary>
        private void PreviewData()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
            {
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "Инициализация и подготовка данных для предпросмотра...", string.Empty, true);
                WriteToTrace("Инициализация и подготовка данных для предпросмотра...", TraceMessageKind.Information);

                PreparePreviewData();

                // Пишем в протокол о начале закачки
                WriteEventIntoPreviewDataProtocol(PreviewDataEventKind.dpeStart, "Старт предпросмотра.");

                // Проверка прав на запуск закачки
                LogicalCallContextData l = LogicalCallContextData.GetContext();
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                DirectPreviewData();
            }
            catch (ThreadAbortException)
            {
                threadIsAbort = true;
            }
            catch (Exception ex)
            {
                finishedWithErrors = true;
                WriteEventIntoPreviewDataProtocol(PreviewDataEventKind.dpeCriticalError, "Ошибка выполнения закачки", ex);
                //WriteStringToErrorFile(ex.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // Пропускаем все следующие этапы
                        SetStagesStateAfterStage(PumpProcessStates.PumpData, StageState.Skipped);
                        this.StagesQueue[PumpProcessStates.PreviewData].StageCurrentState = StageState.FinishedWithErrors;
                        WriteEventIntoPreviewDataProtocol(PreviewDataEventKind.dpeFinishedWithErrors,
                            "Операция прервана пользователем. Все следующие этапы пропущены.");
                    }
                    else
                    {
                        if (!finishedWithErrors)// && this.PumpedSources.Count > 0)
                        {
                            this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = StageState.SuccefullFinished;
                            WriteEventIntoPreviewDataProtocol(PreviewDataEventKind.dpeSuccefullFinished,
                                "Закачка успешно завершена. " + MakeDataSourcesVault());
                        }
                        else
                        {
                            // Пропускаем все следующие этапы
                            SetStagesStateAfterStage(PumpProcessStates.PumpData, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.PreviewData].StageCurrentState = StageState.FinishedWithErrors;
                            WriteEventIntoPreviewDataProtocol(PreviewDataEventKind.dpeFinishedWithErrors,
                                "Закачка завершена с ошибками. Все следующие этапы пропущены. " + MakeDataSourcesVault());
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteToTrace(ex.ToString(), TraceMessageKind.CriticalError);
                    //WriteStringToErrorFile(ex.ToString());
                }
                finally
                {
                    //WriteStopMessageToUsersProtocol();
                    DisposeProperties();
                }

                endStageEvent.Set();
            }
        }

        #endregion Предварительный просмотр


        #region Закачка данных

        /// <summary>
        /// Закачка данных. 
        /// Переопределяется в потомках для выполнения действий по закачке данных.
        /// </summary>
        protected virtual void DirectPumpData()
        {
            WriteEventIntoDataPumpProtocol(
                DataPumpEventKind.dpeStart, "На данном этапе никаких действий не выполняется.");
        }

        protected virtual void MarkClsAsInvalidate()
        {
            foreach (IClassifier cls in this.CubeClassifiers)
                scheme.Processor.InvalidateDimension(cls, "Krista.FM.Server.Scheme.Classes.Classifier",
                    InvalidateReason.ClassifierChanged, cls.OlapName);

            for (int i = 0; i < dimensionsForProcess.GetLength(0); i += 2)
                scheme.Processor.InvalidateDimension(
                    dimensionsForProcess[i],
                    "Krista.FM.Server.Scheme.Classes.Classifier",
                    InvalidateReason.ClassifierChanged,
                    dimensionsForProcess[i + 1]);
        }

        private void SendMessage(string message, MessageImportance mi, MessageType mt, string transferLink)
        {
            MessageWrapper messageWrapper = new MessageWrapper();
            messageWrapper.Subject = message;
            messageWrapper.MessageType = mt;
            messageWrapper.MessageImportance = mi;
            messageWrapper.SendAll = true;
            messageWrapper.TransferLink = transferLink;

            scheme.MessageManager.SendMessage(messageWrapper);
        }

        /// <summary>
        /// Закачка данных. 
        /// Выполняет подготовительные и завершающие действия. Вызывается главным потоком
        /// </summary>
        private void PumpData()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
			{
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "Инициализация и подготовка данных для закачки...", string.Empty, true);
                WriteToTrace("Инициализация и подготовка данных для закачки...", TraceMessageKind.Information);

                PreparePumpData();

				// Пишем в протокол о начале закачки
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт закачки.");

                // Проверка прав на запуск закачки
                LogicalCallContextData l = LogicalCallContextData.GetContext();
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                // инициализируем служебные массивы (установка иерархии, сопоставление, расчет измерений)
                InitAuxiliaryClss();

                DirectPumpData();

                // Удаляем строки "Неизвестные данные" из пустых классификаторов по закачанным источникам
                //DeleteUnusedClsFixedRows();

                // Запись в протокол о начале установки иерархии
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт установки иерархии на этапе закачки.");
                // Установка иерархии
                ClsHierarchySetting();

                // пометить измененные в закачке классификаторы как требующие расчета
                MarkClsAsInvalidate();

                // отправляем сообщение об успешном завершении этапа
                SendMessage(string.Format("Процесс закачки ({0} - этап закачки данных) успешно завершен", this.ProgramIdentifier),
                    MessageImportance.Regular, MessageType.PumpMessage, this.PumpID.ToString());
            }
            catch (ThreadAbortException)
            {
                threadIsAbort = true;
            }
            catch (Exception ex)
            {
                finishedWithErrors = true;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, "Ошибка выполнения закачки", ex);
                //WriteStringToErrorFile(ex.ToString());

                // отправляем сообщение о завершении этапа c ошибками
                SendMessage(string.Format("Процесс закачки ({0} - этап закачки данных) завершен с ошибками", this.ProgramIdentifier),
                    MessageImportance.Importance, MessageType.PumpMessage, this.PumpID.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // Пропускаем все следующие этапы
                        SetStagesStateAfterStage(PumpProcessStates.ProcessData, StageState.Skipped);
                        this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = StageState.FinishedWithErrors;
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishedWithErrors, 
                            "Операция прервана пользователем. Все следующие этапы пропущены.");
                    }
                    else
                    {
                        if (this.PumpedSources.Count == 0)
                        {
                            // Пропускаем все следующие этапы
                            SetStagesStateAfterStage(PumpProcessStates.ProcessData, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = StageState.SuccefullFinished;
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccefullFinished, "Закачка успешно завершена.");
                        }
                        else if (!finishedWithErrors)
                        {
                            this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = StageState.SuccefullFinished;
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccefullFinished, 
                                "Закачка успешно завершена. " + MakeDataSourcesVault());
                        }
                        else
                        {
                            // Пропускаем все следующие этапы
                            SetStagesStateAfterStage(PumpProcessStates.ProcessData, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = StageState.FinishedWithErrors;
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishedWithErrors, 
                                "Закачка завершена с ошибками. Все следующие этапы пропущены. " + MakeDataSourcesVault());
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteToTrace(ex.ToString(), TraceMessageKind.CriticalError);
                    //WriteStringToErrorFile(ex.ToString());
                }
                finally
                {
                    //WriteStopMessageToUsersProtocol();
                    DisposeProperties();
                }

                endStageEvent.Set();
            }
        }

        #endregion Закачка данных


        #region Обработка данных

        /// <summary>
        /// Обработать закачанные данные. 
        /// Переопределяется в потомках для выполнения действий по обработке данных.
        /// </summary>
        protected virtual void DirectProcessData()
        {
            WriteEventIntoProcessDataProtocol(
                ProcessDataEventKind.pdeInformation, "На данном этапе никаких действий не выполняется.");
        }

        /// <summary>
        /// Обработать закачанные данные. 
        /// Выполняет подготовительные и завершающие действия. Вызывается главным потоком
        /// </summary>
        private void ProcessData()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
			{
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "Инициализация и подготовка данных для обработки...", string.Empty, true);
                WriteToTrace("Инициализация и подготовка данных для обработки...", TraceMessageKind.Information);

				PrepareProcessData();

                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart, "Старт обработки данных.");

                // Проверка прав на запуск закачки
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                // инициализируем служебные массивы (установка иерархии, сопоставление, расчет измерений)
                InitAuxiliaryClss();

                DirectProcessData();

                // Запись в протокол о начале установки иерархии
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart, "Старт установки иерархии на этапе обработки.");
                // Установка иерархии
                ClsHierarchySetting();

                // пометить измененные в закачке классификаторы как требующие расчета
                MarkClsAsInvalidate();

                // нужно очистить контекст представления, иначе будут ошибки с неправильной интерпретацией клс во время закачки
                // в случае, если запущена закачка нескольких источников
                if (this.VersionClassifiers != null)
                    foreach (IClassifier cls in this.VersionClassifiers)
                        ClearPresentationContext(cls);

                // отправляем сообщение об успешном завершении этапа
                SendMessage(string.Format("Процесс закачки ({0} - этап обработки данных) успешно завершен", this.ProgramIdentifier),
                    MessageImportance.Regular, MessageType.PumpProcessMessage, this.PumpID.ToString());
            }
            catch (ThreadAbortException)
            {
                threadIsAbort = true;
            }
            catch (Exception ex)
            {
                finishedWithErrors = true;
                WriteToTrace("ERROR: " + ex.ToString(), TraceMessageKind.Error);
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeError, 
                    string.Format("Ошибка выполнения обработки данных: {0}", ex.Message));
                //WriteStringToErrorFile(ex.ToString());

                // отправляем сообщение о завершении этапа c ошибками
                SendMessage(string.Format("Процесс закачки ({0} - этап обработки данных) завершен с ошибками", this.ProgramIdentifier),
                    MessageImportance.Importance, MessageType.PumpProcessMessage, this.PumpID.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // Пропускаем все следующие этапы
                        SetStagesStateAfterStage(PumpProcessStates.AssociateData, StageState.Skipped);
                        this.StagesQueue[PumpProcessStates.ProcessData].StageCurrentState = StageState.FinishedWithErrors;
                        WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeFinishedWithErrors,
                            "Обработка данных завершена с ошибками: операция прервана пользователем.");
                    }
                    else
                    {
                        if (!finishedWithErrors)
                        {
                            this.StagesQueue[PumpProcessStates.ProcessData].StageCurrentState = StageState.SuccefullFinished;
                            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeSuccefullFinished, "Обработка данных завершена.");
                        }
                        else
                        {
                            // Пропускаем все следующие этапы
                            SetStagesStateAfterStage(PumpProcessStates.AssociateData, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.ProcessData].StageCurrentState = StageState.FinishedWithErrors;
                            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeFinishedWithErrors,
                                "Обработка данных завершена с ошибками.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteToTrace(ex.ToString(), TraceMessageKind.CriticalError);
                }
                finally
                {
                    //WriteStopMessageToUsersProtocol();
                    DisposeProperties();
                }

                endStageEvent.Set();
            }
        }

        #endregion Обработка данных


        #region Сопоставление данных

        /// <summary>
        /// Получение источника (который отличается от источника закачки) сопоставляемого классификатора.
        /// Переопределяется в потомках.
        /// </summary>
        /// <param name="pumpSourceID">ID источника закачки</param>
        /// <returns>ID источника классификатора</returns> 
        protected virtual int GetClsSourceID(int pumpSourceID)
        {
            return sourceID;
        }

        // Формирование списка ID источников классификаторов
        private List<int> GetClsDataSourcesList(Dictionary<int, string> dataSources)
        {
            List<int> clsDataSourcesList = new List<int>();
            foreach (int dataSourceId in dataSources.Keys)
            {
                int clsSourceID = GetClsSourceID(dataSourceId);
                if (clsSourceID > 0 && !clsDataSourcesList.Contains(clsSourceID))
                    clsDataSourcesList.Add(clsSourceID);
            }
            return clsDataSourcesList;
        }

        // Cопоставление классификаторов, формирующихся по источникам, отличающимся от источников закачки
        private void AssociateClssEx(Dictionary<int, string> dataSources)
        {
            if (associateClassifiersEx == null)
                return;
            List<int> clsDataSources = GetClsDataSourcesList(dataSources);
            try
            {
                for (int i = 0; i < clsDataSources.Count; i++)
                {
                    SetDataSource(clsDataSources[i]);
                    DoBridgeCls(clsDataSources[i], string.Format("источник {0} из {1}", i + 1, clsDataSources.Count),
                        this.AssociateClassifiersEx);
                }
            }
            finally
            {
                clsDataSources.Clear();
            }
        }

        /// <summary>
        /// Сопоставить данные.
        /// Переопределяется в потомках для выполнения действий по сопоставлению данных.
        /// </summary>
        protected virtual void DirectAssociateData()
        {
            Dictionary<int, string> dataSources = null;

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessData].IsExecuted)
            {
                dataSources = this.PumpedSources;
            }
            else
            {
                dataSources = GetAllPumpedDataSources();
            }

            if (dataSources.Count == 0)
            {
                this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                    BridgeOperationsEventKind.boeError, "Нет данных", "Нет данных",
                    "Ошибка при установке иерархии данных: Нет источников, закачанные данные из которых " +
                    "могли бы быть обработаны.", this.PumpID, -1);
                WriteToTrace("ERROR: Нет источников, закачанные данные из которых " +
                    "могли бы быть обработаны.", TraceMessageKind.Error);
            }
            else
            {
                int i = 1;
                // сопоставляем классификаторы, формирующиеся по источникам закачки
                foreach (KeyValuePair<int, string> dataSource in dataSources)
                {
                    SetDataSource(dataSource.Key);
                    DoBridgeCls(dataSource.Key,
                        string.Format("источник {0} из {1}", i, dataSources.Count), this.AssociateClassifiers);
                    i++;
                }
                // сопоставляем классификаторы, формирующиеся по источникам, отличающимся от источников закачки
                AssociateClssEx(dataSources);
            }
        }

        /// <summary>
        /// Сопоставить данные.
        /// Выполняет подготовительные и завершающие действия. Вызывается главным потоком
        /// </summary>
        private void AssociateData()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
            {
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "Инициализация и подготовка данных для сопоставления...", string.Empty, true);
                WriteToTrace("Инициализация и подготовка данных для сопоставления...", TraceMessageKind.Information);

                PrepareAssociateData();

                this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                    BridgeOperationsEventKind.boeStart, "Нет данных", "Нет данных",
                    "Старт сопоставления данных.", this.PumpID, -1);

                // Проверка прав на запуск закачки
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                // инициализируем служебные массивы (установка иерархии, сопоставление, расчет измерений)
                InitAuxiliaryClss();

                DirectAssociateData();

                // отправляем сообщение об успешном завершении этапа
                SendMessage(string.Format("Процесс закачки ({0} - этап сопоставления) успешно завершен", this.ProgramIdentifier),
                    MessageImportance.Regular, MessageType.PumpAssociateMessage, this.PumpID.ToString());
            }
            catch (ThreadAbortException)
            {
                threadIsAbort = true;
            }
            catch (Exception ex)
            {
                finishedWithErrors = true;
                WriteToTrace("ERROR: " + ex.ToString(), TraceMessageKind.Error);
                this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                    BridgeOperationsEventKind.boeError, "Нет данных", "Нет данных",
                    string.Format("Ошибка выполнения сопоставления данных: {0}", ex.Message),
                    this.PumpID, this.SourceID);
                //WriteStringToErrorFile(ex.ToString());

                // отправляем сообщение о завершении этапа c ошибками
                SendMessage(string.Format("Процесс закачки ({0} - этап сопоставления) завершен с ошибками", this.ProgramIdentifier),
                    MessageImportance.Importance, MessageType.PumpAssociateMessage, this.PumpID.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // Пропускаем все следующие этапы
                        SetStagesStateAfterStage(PumpProcessStates.ProcessCube, StageState.Skipped);
                        this.StagesQueue[PumpProcessStates.AssociateData].StageCurrentState = StageState.FinishedWithErrors;
                        this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                            BridgeOperationsEventKind.boeFinishedWithError, "Нет данных", "Нет данных",
                            "Сопоставление данных завершено с ошибками: операция прервана пользователем.",
                            this.PumpID, -1);
                    }
                    else
                    {
                        if (!finishedWithErrors)
                        {
                            this.StagesQueue[PumpProcessStates.AssociateData].StageCurrentState = StageState.SuccefullFinished;
                            this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                                BridgeOperationsEventKind.boeSuccefullFinished, "Нет данных", "Нет данных",
                                "Сопоставление данных завершено.",
                                this.PumpID, -1);
                        }
                        else
                        {
                            // Пропускаем все следующие этапы
                            SetStagesStateAfterStage(PumpProcessStates.ProcessCube, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.AssociateData].StageCurrentState = StageState.FinishedWithErrors;
                            this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                                BridgeOperationsEventKind.boeFinishedWithError, "Нет данных", "Нет данных",
                                "Сопоставление данных завершено с ошибками.",
                                this.PumpID, -1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteToTrace(ex.ToString(), TraceMessageKind.CriticalError);
                }
                finally
                {
                    //WriteStopMessageToUsersProtocol();
                    DisposeProperties();
                }

                endStageEvent.Set();
            }
        }

        #endregion Сопоставление данных


        #region Расчет кубов

        // список (objectKey1, cubeName1, ...)
        // нужен для вызова расчета кубов отдельно, если по ним нет сущностей
        public string[] cubesForProcess = new string[] { };
        public string[] dimensionsForProcess = new string[] { };

        /// <summary>
        /// Рассчитать кубы.
        /// Переопределяется в потомках для выполнения действий по расчету кубов.
        /// </summary>
        protected virtual void DirectProcessCube()
        {
            // На всякий случей делаем сохранение контекста...
            LogicalCallContextData callContext = LogicalCallContextData.GetContext();

            Guid batchGuid = Guid.Empty;
            try
            {
                // Создаем новый пакет для расчета
                batchGuid = scheme.Processor.CreateBatch();

                // Сохраняем идентификатор пакета в базе данных в таблице PumpHistory для текущей операции закачки
                this.PumpRegistryElement.PumpHistoryCollection[this.pumpID].BatchID = batchGuid.ToString();

                string startInfoMessage = String.Format("Старт отправки многомерных объектов на расчет. Идентификатор пакета \"{0}\"", batchGuid);
                
                WriteToTrace(startInfoMessage, TraceMessageKind.Information);

                this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                    this.ProgramIdentifier,
                    MDProcessingEventKind.mdpeSuccefullFinished,
                    startInfoMessage,
                    batchGuid.ToString(), batchGuid.ToString(),
                    OlapObjectType.Database, batchGuid.ToString());


                if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted ||
                    this.StagesQueue[PumpProcessStates.ProcessData].IsExecuted ||
                    this.StagesQueue[PumpProcessStates.AssociateData].IsExecuted)
                {
                    // в особо одаренных случаях (спасибо неиссякаемым креативам отдела викуси) 
                    // закачка используется не как закачка а как средство переноса данных (как же все это заебало)
                    // в таком случае источников нет и вызываем расчет кубов с сурсайди -1
                    if (this.PumpedSources.Keys.Count == 0)
                    {
                        ProcessCubes(this.CubeFacts, this.CubeClassifiers, batchGuid);
                    }
                    else
                    {
                        foreach (int dataSourceID in this.PumpedSources.Keys)
                        {
                            SetDataSource(dataSourceID);
                            // расчет кубов для определенного источника
                            ProcessCubes(this.CubeFacts, this.CubeClassifiers, batchGuid);
                        }
                    }
                }
                else
                {
                    //  Расчет кубов для всех источников закачки (SourceID = -1)
                    ProcessCubes(this.CubeFacts, this.CubeClassifiers, batchGuid);
                }

                // Отправляем пакет на расчет
                scheme.Processor.ProcessManager.StartBatch(batchGuid);

                string infoMessage = "Многомерные объекты  успешно отправлены на расчет";
                WriteToTrace(infoMessage, TraceMessageKind.Information);
                this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                    this.ProgramIdentifier,
                    MDProcessingEventKind.mdpeSuccefullFinished,
                    infoMessage,
                    batchGuid.ToString(), batchGuid.ToString(),
                    OlapObjectType.Database, batchGuid.ToString());

                // генерация отчетов для айпадов
                WriteToTrace("Начало генерации отчетов", TraceMessageKind.Information);
                GenerateIPadReports(batchGuid);
                WriteToTrace("Завершение генерации отчетов", TraceMessageKind.Information);
            }
            catch (ThreadAbortException)
            {
                if (batchGuid != Guid.Empty)
                {
                    scheme.Processor.RevertBatch(batchGuid);
                }

                string errorMessage = "Отправка многомерных объектов на расчет закончена с ошибками: операция прервана пользователем";
                this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                    this.ProgramIdentifier,
                    MDProcessingEventKind.mdpeFinishedWithError,
                    errorMessage, 
                    batchGuid.ToString(), batchGuid.ToString(), 
                    OlapObjectType.Database, batchGuid.ToString());
                WriteToTrace(errorMessage, TraceMessageKind.Error);
            }
            catch (Exception ex)
            {
                if (batchGuid != Guid.Empty)
                {
                    scheme.Processor.RevertBatch(batchGuid);
                }

                string errorMessage = String.Format(
                    "Отправка многомерных объектов на расчет закончена с ошибками: {0}",
                    Krista.Diagnostics.KristaDiagnostics.ExpandException(ex));
                
                this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                    this.ProgramIdentifier,
                    MDProcessingEventKind.mdpeFinishedWithError,
                    errorMessage,
                    batchGuid.ToString(), batchGuid.ToString(),
                    OlapObjectType.Database, batchGuid.ToString());
                
                WriteToTrace(errorMessage, TraceMessageKind.Error);
            }
            finally
            {
                LogicalCallContextData.SetContext(callContext);
            }
        }

        #region генерация отчетов айпада

        private void GenerateGetParams(string fileName, ref string bootloadServiceUri,
            ref string serverURL, ref string reportsHostUrl)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            try
            {
                XmlNode clsNode = doc.SelectSingleNode("PumpGenerateParams/params");
                bootloadServiceUri = clsNode.Attributes["bootloadServiceUri"].Value;
                serverURL = clsNode.Attributes["serverURL"].Value;
                reportsHostUrl = clsNode.Attributes["reportsHostUrl"].Value;
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref doc);
            }
        }

        SnapshotStartParams GenerateGetStartParams(string fileName, string serverURL, string reportsHostUrl)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            try
            {
                string innerXml = doc.InnerXml;
                return new SnapshotStartParams(serverURL, innerXml, reportsHostUrl);
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref doc);
            }
        }

        // генерация отчетов для айпада
        private const string PUMP_GENERATE_FOLDER = "PumpGenerateReports";
        private const string PUMP_GENERATE_PARAMS_FILENAME = "PumpGenerateParams.xml";
        private void GenerateIPadReports(Guid batchGuid)
        {
            DirectoryInfo dir = new DirectoryInfo(string.Format("{0}\\{1}", GetCurrentDir().FullName, PUMP_GENERATE_FOLDER));
            string pumpFileName = string.Format("{0}.xml", this.ProgramIdentifier);
            if (dir.GetFiles(pumpFileName, SearchOption.TopDirectoryOnly).GetLength(0) == 0)
            {
                WriteToTrace(string.Format("файл {0} не найден", pumpFileName), TraceMessageKind.Warning);
                return;
            }
            else
                WriteToTrace(string.Format("файл со списком генерируемых отчетов: {0}", pumpFileName), TraceMessageKind.Information);

            try
            {
                // генерацию вызываем только после завершения обработки пакета
                BatchState bs = scheme.Processor.GetBatchState(batchGuid);
                while ((bs != BatchState.Complited) && (bs != BatchState.ComplitedWithError) && (bs != BatchState.Deleted))
                {
                    Thread.Sleep(1000 * 30);
                    bs = scheme.Processor.GetBatchState(batchGuid);
                }
            }
            catch (Exception exc)
            {
                WriteToTrace(string.Format("Ошибка при получении состояния пакета: {0} ({1})", exc.Message, exc.StackTrace), TraceMessageKind.Warning);
            }

            pumpFileName = string.Format("{0}\\{1}", dir, pumpFileName);
            string pumpGenerateParamsFileName = string.Format("{0}\\{1}", dir, PUMP_GENERATE_PARAMS_FILENAME);
            if (dir.GetFiles(PUMP_GENERATE_PARAMS_FILENAME, SearchOption.TopDirectoryOnly).GetLength(0) == 0)
            {
                WriteToTrace(string.Format("файл {0} не найден", pumpGenerateParamsFileName), TraceMessageKind.Warning);
                return;
            }

            string bootloadServiceUri = string.Empty;
            string serverURL = string.Empty;
            string reportsHostUrl = string.Empty;
            GenerateGetParams(pumpGenerateParamsFileName, ref bootloadServiceUri, ref serverURL, ref reportsHostUrl);
            WriteToTrace(string.Format("Параметры генерации: bootloadServiceUri={0}; serverURL={1}; reportsHostUrl={2};",
                bootloadServiceUri, serverURL, reportsHostUrl), TraceMessageKind.Information);

            SnapshotStartParams ssp = GenerateGetStartParams(pumpFileName, serverURL, reportsHostUrl);
            ISnapshotService snapshotService = (ISnapshotService)Activator.GetObject(typeof(ISnapshotService), bootloadServiceUri);
            try
            {
                snapshotService.DoSnapshot(ssp);
            }
            catch (Exception ex)
            {
                this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                    MDProcessingEventKind.mdpeError,
                    string.Format("Ошибка при генерации отчетов: {0}", ex.Message), string.Empty);
            }
        }

        #endregion генерация отчетов айпада

        /// <summary>
        /// Рассчитать кубы.
        /// Выполняет подготовительные и завершающие действия. Вызывается главным потоком
        /// </summary>
        private void ProcessCube()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
            {
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "Инициализация и подготовка данных для расчета кубов...", string.Empty, true);
                WriteToTrace("Инициализация и подготовка данных для расчета кубов...", TraceMessageKind.Information);

                PrepareProcessCube();

                //this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                //    MDProcessingEventKind.mdpeStart,
                //    "Старт расчета кубов.", "Нет данных", this.PumpID, -1);

                // Проверка прав на запуск закачки
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                // инициализируем служебные массивы (установка иерархии, сопоставление, расчет измерений)
                InitAuxiliaryClss();

                DirectProcessCube();
            }
            catch (ThreadAbortException)
            {
                threadIsAbort = true;
            }
            catch (Exception ex)
            {
                finishedWithErrors = true;
                WriteToTrace("ERROR: " + ex.ToString(), TraceMessageKind.Error);
                this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                    MDProcessingEventKind.mdpeError,
                    string.Format("Ошибка выполнения расчета кубов: {0}", ex.Message), "Нет данных");
                //WriteStringToErrorFile(ex.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // Пропускаем все следующие этапы
                        SetStagesStateAfterStage(PumpProcessStates.CheckData, StageState.Skipped);
                        this.StagesQueue[PumpProcessStates.ProcessCube].StageCurrentState = StageState.FinishedWithErrors;
                        this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                            MDProcessingEventKind.mdpeFinishedWithError,
                            "Расчет кубов завершен с ошибками: операция прервана пользователем.", "Нет данных");
                    }
                    else
                    {
                        if (!finishedWithErrors)
                        {
                            this.StagesQueue[PumpProcessStates.ProcessCube].StageCurrentState = StageState.SuccefullFinished;
                            //this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                            //    MDProcessingEventKind.mdpeSuccefullFinished,
                            //    "Расчет кубов завершен.", "Нет данных", this.PumpID, -1);
                        }
                        else
                        {
                            // Пропускаем все следующие этапы
                            SetStagesStateAfterStage(PumpProcessStates.CheckData, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.ProcessCube].StageCurrentState = StageState.FinishedWithErrors;
                            this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                                MDProcessingEventKind.mdpeFinishedWithError,
                                "Расчет кубов завершен с ошибками.", "Нет данных");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteToTrace(ex.ToString(), TraceMessageKind.CriticalError);
                }
                finally
                {
                    //WriteStopMessageToUsersProtocol();
                    DisposeProperties();
                }

                endStageEvent.Set();
            }
        }

        #endregion Расчет кубов


        #region Проверка данных

        /// <summary>
        /// Проверить закачанные данные.
        /// Переопределяется в потомках для выполнения действий по проверке закачанных данных.
        /// </summary>
        protected virtual void DirectCheckData()
        {
            this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                ReviseDataEventKind.pdeInformation,
                "На данном этапе никаких действий не выполняется.", this.PumpID, -1);
        }

        /// <summary>
        /// Проверить закачанные данные.
        /// Выполняет подготовительные и завершающие действия. Вызывается главным потоком
        /// </summary>
        private void CheckData()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
            {
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "Инициализация и подготовка данных для проверки...", string.Empty, true);
                WriteToTrace("Инициализация и подготовка данных для проверки...", TraceMessageKind.Information);

                PrepareCheckData();

                this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                    ReviseDataEventKind.pdeStart,
                    "Старт проверки данных.", this.PumpID, -1);

                // Проверка прав на запуск закачки
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                DirectCheckData();

                // отправляем сообщение об успешном завершении этапа
                SendMessage(string.Format("Процесс закачки ({0} - этап проверки данных) успешно завершен", this.ProgramIdentifier),
                    MessageImportance.Regular, MessageType.PumpCheckDataMessage, this.PumpID.ToString());
            }
            catch (ThreadAbortException)
            {
                threadIsAbort = true;
            }
            catch (Exception ex)
            {
                finishedWithErrors = true;
                WriteToTrace("ERROR: " + ex.ToString(), TraceMessageKind.Error);
                this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                    ReviseDataEventKind.pdeFinishedWithErrors,
                    string.Format("Ошибка выполнения проверки данных: {0}", ex.Message),
                    this.PumpID, this.SourceID);
                //WriteStringToErrorFile(ex.ToString());

                // отправляем сообщение о завершении этапа c ошибками
                SendMessage(string.Format("Процесс закачки ({0} - этап проверки данных) завершен с ошибками", this.ProgramIdentifier),
                    MessageImportance.Importance, MessageType.PumpCheckDataMessage, this.PumpID.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // Пропускаем все следующие этапы
                        this.StagesQueue[PumpProcessStates.CheckData].StageCurrentState = StageState.FinishedWithErrors;
                        this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                            ReviseDataEventKind.pdeFinishedWithErrors,
                            "Проверка данных завершена с ошибками: операция прервана пользователем.", this.PumpID, -1);
                    }
                    else
                    {
                        if (!finishedWithErrors)
                        {
                            this.StagesQueue[PumpProcessStates.CheckData].StageCurrentState = StageState.SuccefullFinished;
                            this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                                ReviseDataEventKind.pdeSuccefullFinished,
                                "Проверка данных завершена.", this.PumpID, -1);
                        }
                        else
                        {
                            // Пропускаем все следующие этапы
                            this.StagesQueue[PumpProcessStates.CheckData].StageCurrentState = StageState.FinishedWithErrors;
                            this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                                ReviseDataEventKind.pdeFinishedWithErrors,
                                "Проверка данных завершена с ошибками.", this.PumpID, -1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteToTrace(ex.ToString(), TraceMessageKind.CriticalError);
                }
                finally
                {
                    //WriteStopMessageToUsersProtocol();
                    DisposeProperties();
                }

                endStageEvent.Set();
            }
        }

        #endregion Проверка данных

        #endregion Методы этапов закачки


        #region Функции выполнения начальных действий этапов

        /// <summary>
        /// Пишет сообщение о старте закачки в протокол действий пользователей
        /// </summary>
        private void WriteStartMessageToUsersProtocol()
        {
            if (this.StagesQueue.GetLastExecutedQueueElement() == null)
            {
                this.UsersOperationProtocol.WriteEventIntoUsersOperationProtocol(
                    UsersOperationEventKind.uoeStartWorking_RefUserName,
                    string.Format("Запущена {0}", this.PumpRegistryElement.Name));
            }
        }

        /// <summary>
        /// Добавляет ACL на указанный каталог для указанного пользователя.
        /// </summary>
        /// <param name="dir">Каталог</param>
        /// <param name="account">Пользователь</param>
        /// <param name="rights">Права файловой системы</param>
        /// <param name="controlType">Права доступа</param>
        private void AddDirectorySecurity(DirectoryInfo dir, string account, FileSystemRights rights, 
            AccessControlType controlType)
        {
            if (dir == null)
                return;

            // Получаем объект DirectorySecurity, представляющий текущие настройки безопасности
            DirectorySecurity dSecurity = dir.GetAccessControl();

            // Сначала установим доступ к самому каталогу.
            FileSystemAccessRule fsAccess = new FileSystemAccessRule(account, rights, InheritanceFlags.None,
                PropagationFlags.NoPropagateInherit, controlType);

            bool allOK;
            dSecurity.ModifyAccessRule(AccessControlModification.Set, fsAccess, out allOK);
            if (!allOK)
            {
                throw new ApplicationException("Невозможно установить доступ к " + dir + " для " + account);
            }

            // Устанавливаем правила наследования доступа
            FileSystemAccessRule inheritanceRule = new FileSystemAccessRule(account, rights,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly,
                controlType);
            dSecurity.ModifyAccessRule(AccessControlModification.Add, inheritanceRule, out allOK);
            if (!allOK)
            {
                throw new ApplicationException(
                    "Невозможно добавить правило наследования доступа на " + dir + " для " + account);
            }

            // Устанавливаем новые параметры доступа
            dir.SetAccessControl(dSecurity);
        }

        /// <summary>
        /// Открывает пользователю полный доступ к папке и всем подпапкам
        /// </summary>
        /// <param name="userName">Имя пользователя, без указания имени машины</param>
        /// <param name="dir">Папка</param>
        private void SetDirectoriesRights(string userName, DirectoryInfo dir)
        {
            string account = Environment.MachineName + "\\" + userName;

            AddDirectorySecurity(dir, account, FileSystemRights.FullControl, AccessControlType.Allow);

            DirectoryInfo[] subDirs = dir.GetDirectories("*", SearchOption.AllDirectories);
            foreach (DirectoryInfo subDir in subDirs)
            {
                AddDirectorySecurity(subDir, account, FileSystemRights.FullControl, AccessControlType.Allow);
            }

            // Если нужно выполнять приложения, не забудем открыть доступ к системной папке на чтение и 
            // исполнение приложений
            AddDirectorySecurity(new DirectoryInfo(Environment.SystemDirectory), account,
                FileSystemRights.FullControl, AccessControlType.Allow);
        }

        /// <summary>
        /// Инициализация свойств класса закачки
        /// </summary>
        private void InitProperties()
        {               
            // Добавляем запись истории закачки
            this.pumpID = AddPumpHistoryElement(this.ProgramIdentifier,
                string.Format("{0}. {1}", this.PumpRegistryElement.Name, PumpProcessStatesToRusString(this.State)));
            if (this.pumpID < 0)
            {
                throw new Exception("Ошибка при добавлении записи истории закачки");
            }

            // Получение адреса каталога источника
            this.RootDir = new DirectoryInfo(this.PumpRegistryElement.DataSourcesUNCPath);
            if (this.RootDir == null)
            {
                throw new Exception("Ошибка при получении корневого каталога источника");
            }

            //SetDirectoriesRights(Environment.UserName, this.RootDir);
            
            // Если не найден каталог источника, то пишем об этом в лог
            if (!this.RootDir.Exists)
            {
                throw new Exception(
                    string.Format("Каталог с данными {0} не найден", this.RootDir.FullName));
            }

            // Нужно ли перемещать файлы в архив
            this.UseArchive = Convert.ToBoolean(GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "ucbMoveFilesToArchive", "false"));

            // Удалять закачанные ранее данные из того же источника
            this.DeleteEarlierData = Convert.ToBoolean(GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "ucbDeleteEarlierData", "false"));

            // Признак закачки заключительных оборотов
            this.FinalOverturn = IsFinalOverturn();
        }

        /// <summary>
        /// Выполняет подготовительные действия перед предпросмотром данных
        /// </summary>
        /// <returns>Результат</returns>
        private void PreparePreviewData()
        {
            //WriteStartMessageToUsersProtocol();

            InitProperties();
        }

        /// <summary>
        /// Выполняет подготовительные действия перед закачкой
        /// </summary>
        /// <returns>Результат</returns>
        private void PreparePumpData()
        {
            //WriteStartMessageToUsersProtocol();

            if (this.StagesQueue[PumpProcessStates.PreviewData].IsExecuted) return;

            InitProperties();
        }

        /// <summary>
        /// Выполняет подготовительные действия перед обработкой данных
        /// </summary>
        /// <returns>Результат</returns>
        private void PrepareProcessData()
        {
            //WriteStartMessageToUsersProtocol();

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted) return;

            InitProperties();
        }

        /// <summary>
        /// Выполняет подготовительные действия перед сопоставлением данных
        /// </summary>
        /// <returns>Результат</returns>
        private void PrepareAssociateData()
        {
            //WriteStartMessageToUsersProtocol();

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessData].IsExecuted) return;

            InitProperties();
        }

        /// <summary>
        /// Выполняет подготовительные действия перед расчетом кубов
        /// </summary>
        /// <returns>Результат</returns>
        private void PrepareProcessCube()
        {
            //WriteStartMessageToUsersProtocol();

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.AssociateData].IsExecuted) return;

            InitProperties();
        }

        /// <summary>
        /// Выполняет подготовительные действия перед проверкой данных
        /// </summary>
        /// <returns>Результат</returns>
        private void PrepareCheckData()
        {
            //WriteStartMessageToUsersProtocol();

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.AssociateData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessCube].IsExecuted) return;

            InitProperties();
        }

        #endregion Функции выполнения начальных действий этапов


        #region Функции выполнения завершающих действий этапов

        /// <summary>
        /// Записывает сообщение об окончании закачки в протокол действий пользователей
        /// </summary>
        private void WriteStopMessageToUsersProtocol()
        {
            try
            {
                if (this.StagesQueue.GetNextExecutableQueueElement() == null)
                {
                    this.UsersOperationProtocol.WriteEventIntoUsersOperationProtocol(
                        UsersOperationEventKind.uoeStartWorking_RefUserName,
                        string.Format("{0} завершена.", this.PumpRegistryElement.Name));
                }
            }
            catch (Exception ex)
            {
                WriteToTrace("Ошибка при записи в протокол действий пользователя: " + ex.ToString(), TraceMessageKind.Error);
            }
        }

        /// <summary>
        /// Удаляет строки "Неизвестные данные" из всех классификаторов this.UsedClassifiers, в которых
        /// нет данных по закачанным источникам.
        /// </summary>
        private void DeleteUnusedClsFixedRows()
        {
            WriteToTrace("Старт удаления строк \"Неизвестные данные\" из пустых классфикаторов.", TraceMessageKind.Information);

            foreach (KeyValuePair<int, string> dataSource in this.PumpedSources)
            {
                for (int j = 0; j < this.UsedClassifiers.GetLength(0); j++)
                {
                    if (GetClsRecordsAmount(this.UsedClassifiers[j], -1, dataSource.Key, string.Empty) == 0)
                    {
                        DeleteTableData(this.UsedClassifiers[j], -1, dataSource.Key, "ID < 0");
                    }
                }
            }

            WriteToTrace("Удаление строк \"Неизвестные данные\" из пустых классфикаторов закончено.", TraceMessageKind.Information);
        }

        #endregion Функции выполнения завершающих действий этапов


        #region Функции установки иерархии классификаторов

        /// <summary>
        /// Устанавливает иерархию классификаторов
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="message">Сообщение прогресса</param>
        /// <param name="cls">Массив IClassifier</param>
        protected void SetClsStandardHierarchy(string message, IClassifier[] cls)
        {
            int lng = cls.GetLength(0);

            for (int i = 0; i < lng; i++)
            {
                if (cls[i] == null) continue;

                string semantic = cls[i].FullCaption;

                try
                {
                    SetProgress(lng, i + 1,
                        string.Format("Установка иерархии {0} ({1})...", semantic, message),
                        string.Format("Классификатор {0} из {1}", i + 1, lng), true);
                    WriteToTrace(string.Format("Установка иерархии {0}...", semantic), TraceMessageKind.Information);

                    DataSet ds = null;
                    cls[i].DivideAndFormHierarchy(this.SourceID, this.PumpID, ref ds);

                    WriteToTrace(string.Format("Иерархия {0} установлена.", semantic), TraceMessageKind.Information);
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeError, string.Format(
                            "Установка иерархии классификатора {0} по данным источника {1}, ID {2} закончена с ошибками.",
                            semantic, GetSourcePathBySourceID(this.SourceID), this.SourceID), ex);
                }
            }
        }

        protected void SetPresentationContexts()
        {
            if (this.VersionClassifiers != null)
                foreach (IClassifier cls in this.VersionClassifiers)
                    SetPresentationContext(cls);
        }

        protected void ClearPresentationContexts()
        {
            // нужно очистить контекст представления, иначе будут ошибки с неправильной интерпретацией клс во время последующей закачки
            // в случае, если запущена закачка нескольких источников
            if (this.VersionClassifiers != null)
                foreach (IClassifier cls in this.VersionClassifiers)
                    ClearPresentationContext(cls);
        }

        /// <summary>
        /// Установка иерархии классификаторов после закачки данных. 
        /// Может переопределяться в потомках для выполнения индивидуальных действий по установке иерархии.
        /// </summary>
        protected virtual void DirectClsHierarchySetting()
        {
            int i = 1;
            foreach (KeyValuePair<int, string> dataSource in this.PumpedSources)
            {
                try
                {
                    SetDataSource(dataSource.Key);

                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeInformation, string.Format(
                        "Старт установки иерархии по данным источника {0} (ID {1}).",
                        GetSourcePathBySourceID(dataSource.Key), dataSource.Key));

                    // устанавливаем представления
                    SetPresentationContexts();
                    try
                    {
                        SetClsStandardHierarchy(
                            string.Format("источник {0} из {1}", i, this.PumpedSources.Count), this.HierarchyClassifiers);
                    }
                    finally
                    {
                        // очищаем представления
                        ClearPresentationContexts();
                    }

                    WriteEventIntoDataPumpProtocol(
                         DataPumpEventKind.dpeInformation, string.Format(
                         "Установка иерархии по данным источника {0}, ID {1} успешно закончена.",
                         GetSourcePathBySourceID(dataSource.Key), dataSource.Key));

                    i++;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeError, string.Format(
                            "Установка иерархии по данным источника {0}, ID {1} закончена с ошибками.",
                            GetSourcePathBySourceID(dataSource.Key), dataSource.Key), ex);
                }
            }
        }

        /// <summary>
        /// Установка иерархии классификаторов после закачки данных. 
        /// Выполняет подготовительные и завершающие действия. Вызывается на этапе закачки.
        /// </summary>
        private void ClsHierarchySetting()
        {
            bool success = true;
            bool aborted = false;

            try
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт установки иерархии.");

                // Установка иерархии
                if (this.PumpedSources.Count > 0)
                	DirectClsHierarchySetting();
            }
            catch (ThreadAbortException)
            {
                aborted = true;
            }
            catch
            {
                success = false;
            }
            finally
            {
                if (aborted)
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeFinishedWithErrors, 
                        "Установка иерархии завершена с ошибками: операция прервана пользователем.");
                }
                else
                {
                    if (success)
                    {
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeSuccefullFinished, "Установка иерархии завершена.");
                    }
                    else
                    {
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeFinishedWithErrors, "Установка иерархии завершена с ошибками.");
                    }
                }
            }
        }

        #endregion Функции установки иерархии классификаторов


        #region Функции сопоставления данных

        /// <summary>
        /// Сопоставляет классификатор
        /// </summary>
        /// <param name="cls">Объект классификатора</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="allowDigits">Учитывать цифровые символы</param>
        /// <param name="reAssociate">true - автоматически сопоставлять все записи; false - сопоставлять только несопоставленные</param>
        protected void AssociateCls(IClassifier cls, int sourceID, bool allowDigits, bool reAssociate)
        {
            if (cls == null) return;

            //foreach (IBridgeAssociation ass in cls.Associations.Values)
            //{
            //    if (ass.AssociationClassType != AssociationClassTypes.Bridge) continue;
            foreach (IEntityAssociation ass in cls.Associations.Values)
            {
                if (!(ass is IBridgeAssociation))
                    continue;

                string assObjectKey = ass.RoleBridge.ObjectKey;
                int? bridgeClsSourceID = Scheme.DataVersionsManager.DataVersions.FindCurrentVersion(assObjectKey);
                if (bridgeClsSourceID == null)
                {
                    bridgeClsSourceID = sourceID;
                    this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                        BridgeOperationsEventKind.boeWarning, "Нет данных", "Нет данных", string.Format(
                        "Сопоставление {0} ({1}) закончено с ошибками: не найдена текущая версия сопоставимого классификатора.", ass.FullCaption, assObjectKey),
                        this.PumpID, sourceID);
                }

                try
                {
                    ((IBridgeAssociation)ass).Associate(sourceID, Convert.ToInt32(bridgeClsSourceID), this.PumpID, allowDigits, reAssociate);
                }
                catch (ThreadAbortException)
                {
                    this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                        BridgeOperationsEventKind.boeFinishedWithError, "Нет данных", "Нет данных", string.Format(
                        "Сопоставление {0} закончено с ошибками: операция прервана пользователем.", ass.FullCaption),
                        this.PumpID, sourceID);
                    WriteToTrace(string.Format(
                        "Сопоставление {0} закончено с ошибками: операция прервана пользователем.", ass.FullCaption), 
                        TraceMessageKind.Error);
                    throw;
                }
                catch (Exception ex)
                {
                    WriteToTrace(string.Format(
                        "Сопоставление {0} закончено с ошибками: {1}",
                        ass.FullCaption, ex.ToString()), TraceMessageKind.Error);
                }
            }
        }

        /// <summary>
        /// Сопоставляет классификаторы
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        protected void DoBridgeCls(int sourceID, string message, IClassifier[] cls)
        {
            DoBridgeCls(sourceID, message, cls, false, false);
        }

        /// <summary>
        /// Сопоставляет классификаторы
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="message"></param>
        /// <param name="cls"></param>
        /// <param name="allowDigits">Учитывать цифровые символы</param>
        /// <param name="reAssociate">true - автоматически сопоставлять все записи; false - сопоставлять только несопоставленные</param>
        protected void DoBridgeCls(int sourceID, string message, IClassifier[] cls, bool allowDigits, bool reAssociate)
        {
            string msg = string.Format(
                "Старт сопоставления данных источника {0} (ID {1}).",
                GetSourcePathBySourceID(sourceID), sourceID);
            this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                BridgeOperationsEventKind.dpeStartDataSourceProcessing, "Нет данных", "Нет данных", msg, this.PumpID, sourceID);
            WriteToTrace(msg, TraceMessageKind.Information);

            // Сопоставление данных              

            int lng = cls.GetLength(0);
            for (int i = 0; i < lng; i++)
            {
                if (cls[i] == null) continue;

                string semantic = cls[i].FullCaption;

                SetProgress(lng, i + 1,
                    string.Format("Сопоставление данных {0} ({1})...", semantic, message),
                    string.Format("Классификатор {0} из {1}", i + 1, lng), true);
                WriteToTrace(string.Format("Сопоставление данных {0}...", semantic), TraceMessageKind.Information);
                if (!cls[i].Attributes.ContainsKey("SourceID"))
                    sourceID = -1;

                AssociateCls(cls[i], sourceID, allowDigits, reAssociate);
                WriteToTrace(string.Format("Данные {0} сопоставлены.", semantic), TraceMessageKind.Information);
            }

            msg = string.Format(
                "Сопоставление данных источника {0} (ID {1}) закончено.",
                GetSourcePathBySourceID(sourceID), sourceID);
            this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                BridgeOperationsEventKind.dpeSuccessfullFinishDataSourceProcess, "Нет данных", "Нет данных", msg, 
                this.PumpID, sourceID);
            WriteToTrace(msg, TraceMessageKind.Information);
        }

        #endregion Функции сопоставления данных


        #region Функции расчета кубов

        /// <summary>
        /// Расчет кубов.
        /// </summary>
        /// <param name="fct">Список таблиц фактов.</param>
        /// <param name="cls">Список классификаторов.</param>
        /// <param name="batchGuid">Идентификатор пакета.</param>
        private bool ProcessCubes(IFactTable[] fct, IClassifier[] cls, Guid batchGuid)
        {
            // Наполняем пакет объектами
            for (int i = 0; i < cls.GetLength(0); i++)
            {
                if (cls[i] != null)
                {
                    WriteToTrace(String.Format("Отправка измерения \"{0}\" на расчет...", cls[i].OlapName), TraceMessageKind.Information);
                    
                    scheme.Processor.InvalidateDimension(
                        cls[i], 
                        ProgramIdentifier,
                        InvalidateReason.ClassifierChanged, 
                        cls[i].OlapName,
                        this.PumpID, this.SourceID,
                        batchGuid);

                    WriteToTrace(String.Format("Измерение \"{0}\" успешно отправлено на расчет", cls[i].OlapName), TraceMessageKind.Information);
                }
            }

            for (int i = 0; i < fct.GetLength(0); i++)
            {
                if (fct[i] != null)
                {
                    WriteToTrace(String.Format("Отправка секции куба \"{0}\" на расчет...", fct[i].OlapName), TraceMessageKind.Information);

                    List<string> partitionsIdList = fct[i].GetPartitionsNameBySourceID(this.SourceID);

                    if (partitionsIdList.Count == 0)
                    {
                        scheme.Processor.InvalidatePartition(
                            fct[i],
                            this.ProgramIdentifier,
                            InvalidateReason.DataPump,
                            fct[i].OlapName,
                            this.PumpID, this.SourceID,
                            batchGuid);

                        WriteToTrace(String.Format("Секция куба \"{0}\" успешно отправлена на расчет.", fct[i].OlapName), TraceMessageKind.Information);
                    }
                    else
                    {
                        foreach (string partitionId in partitionsIdList)
                        {
                            scheme.Processor.InvalidatePartition(
                                fct[i],
                                this.ProgramIdentifier,
                                InvalidateReason.DataPump,
                                fct[i].OlapName, partitionId,
                                this.PumpID, this.SourceID,
                                batchGuid);

                            WriteToTrace(String.Format("Секция куба \"{0}\\{1}\" успешно отправлена на расчет.", fct[i].OlapName, partitionId), TraceMessageKind.Information);
                        }
                    }
                }
            }

            for (int i = 0; i < dimensionsForProcess.GetLength(0); i += 2)
            {
                WriteToTrace(String.Format("Отправка измерения \"{0}\" на расчет...", dimensionsForProcess[i + 1]), TraceMessageKind.Information);
                scheme.Processor.InvalidateDimension(
                    dimensionsForProcess[i],
                    ProgramIdentifier,
                    InvalidateReason.ClassifierChanged,
                    dimensionsForProcess[i + 1],
                    batchGuid);
                WriteToTrace(String.Format("Измерение \"{0}\" успешно отправлено на расчет", dimensionsForProcess[i + 1]), TraceMessageKind.Information);
            }

            for (int i = 0; i < cubesForProcess.GetLength(0); i += 2)
            {
                WriteToTrace(String.Format("Отправка куба \"{0}\" на расчет...", cubesForProcess[i + 1]), TraceMessageKind.Information);
                scheme.Processor.InvalidatePartition(
                    cubesForProcess[i],
                    ProgramIdentifier,
                    InvalidateReason.DataPump,
                    cubesForProcess[i + 1],
                    batchGuid);
                WriteToTrace(String.Format("Куб \"{0}\" успешно отправлен на расчет", cubesForProcess[i + 1]), TraceMessageKind.Information);
            }

            return true;
        }

        #endregion Функции расчета кубов


        #region Функции удаления данных

        /// <summary>
        /// Формирует строку ограничения запроса по перечисленным параметрам
        /// </summary>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="constr">Дополнительное ограничение</param>
        /// <returns>Строка ограничения</returns>
        private string MakeConstraintString(int pumpID, int sourceID, string constr)
        {
            string result = string.Empty;

            if (pumpID >= 0) result = string.Format(" and PUMPID = {0}", pumpID);
            if (sourceID >= 0) result += string.Format(" and SOURCEID = {0}", sourceID);
            if (constr != string.Empty) result += string.Format(" and {0}", constr);

            return result;
        }

        /// <summary>
        /// Возвращает количество записей указанного классификатора по указанному источнику
        /// </summary>
        /// <param name="db">Объект ДБ</</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="constr">Ограничение выборки</param>
        /// <returns>Количество записей</returns>
        protected int GetClsRecordsAmount(IClassifier cls, int pumpID, int sourceID, string constr)
        {
            if (cls == null) return 0;

            // Формирование строки запроса
            string whereStr = MakeConstraintString(pumpID, sourceID, constr);

            if (cls.IsDivided && cls.Levels.HierarchyType == HierarchyType.ParentChild)
            {
                return Convert.ToInt32(this.DB.ExecQuery(
                    string.Format("select count(id) from {0} " +
                        "where ID >= 0 and ((SourceID <> -ParentID or ParentID is null) or " +
                        "ID <> CubeParentID or CubeParentID is null) {1}", 
                        cls.FullDBName, whereStr),
                    QueryResultTypes.Scalar));
            }
            else
            {
                return Convert.ToInt32(this.DB.ExecQuery(
                    string.Format("select count(id) from {0} where ID >= 0 {1}",
                        cls.FullDBName, whereStr),
                    QueryResultTypes.Scalar));
            }
        }

        /// <summary>
        /// Проверяет, есть ли записи в базе в указанных классификаторах по указанному источнику
        /// </summary>
        /// <param name="db">Объект ДБ</</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="constr">Ограничение выборки</param>
        /// <returns>Есть записи в указанных классификаторах по данному источнику или нет</returns>
        protected bool CheckClsRecordsAmount(IClassifier[] cls, int pumpID, int sourceID, 
            string constr)
        {
            WriteToTrace("Проверка наличия в базе данных классификаторов.", TraceMessageKind.Information);
            SetProgress(-1, -1, "Проверка наличия в базе данных классификаторов...", string.Empty, true);

            for (int i = 0; i < cls.GetLength(0); i++)
            {
                if (cls[i] != null)
                {
                    if (GetClsRecordsAmount(cls[i], pumpID, sourceID, constr) > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Возвращает количество записей указанной таблицы фактов по указанному источнику
        /// </summary>
        /// <param name="db">Объект ДБ</</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="constr">Ограничение выборки</param>
        /// <returns>Количество записей</returns>
        protected int GetFactRecordsAmount(IFactTable fct, int pumpID, int sourceID, string constr)
        {
            if (fct == null) return 0;

            // Формирование строки запроса
            string whereStr = MakeConstraintString(pumpID, sourceID, constr);

            return Convert.ToInt32(this.DB.ExecQuery(
                string.Format("select count(id) from {0} where ID >= 0 {1}",
                    fct.FullDBName, whereStr),
                QueryResultTypes.Scalar));
        }

        /// <summary>
        /// Проверяет, есть ли записи в базе в указанных таблицах фактов по указанному источнику
        /// </summary>
        /// <param name="db">Объект ДБ</</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="constr">Ограничение выборки</param>
        /// <returns>Есть записи в указанных классификаторах по данному источнику или нет</returns>
        protected bool CheckFactRecordsAmount(IFactTable[] fct, int pumpID, int sourceID, string constr)
        {
            WriteToTrace("Проверка наличия в базе данных фактов.", TraceMessageKind.Information);
            SetProgress(-1, -1, "Проверка наличия в базе данных фактов...", string.Empty, true);

            for (int i = 0; i < fct.GetLength(0); i++)
            {
                if (fct[i] != null)
                {
                    if (GetFactRecordsAmount(fct[i], pumpID, sourceID, constr) > 0)
                    {
                        return true;
                    }
                }
            }

            WriteToTrace("Данные фактов отсутствуют.", TraceMessageKind.Information);
            SetProgress(-1, -1, "Данные фактов отсутствуют.", string.Empty, true);

            return false;
        }

        /// <summary>
        /// Удаляет данные
        /// </summary>
        /// <param name="db">Объект ДБ</param>
        /// <param name="obj">Объект БД</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="constr">Строка ограничения</param>
		protected void DeleteTableData(IEntity obj, int pumpID, int sourceID, string constr)
        {
            string semantic = obj.FullCaption;

            // Формирование строки запроса
            string whereStr = MakeConstraintString(pumpID, sourceID, constr);
            if (whereStr != string.Empty)
            {
                whereStr = "where " + whereStr.Remove(0, 4);
            }

            WriteToTrace(string.Format("Удаление данных {0}...", semantic), TraceMessageKind.Information);
            SetProgress(-1, -1, string.Format(
                "Источник {0}. Удаление данных {1}...", GetShortSourcePathBySourceID(sourceID), semantic),
                string.Empty, true);
            
            string queryStr = string.Format("delete from {0} {1}", obj.FullDBName, whereStr);
            int recCount = (int)this.DB.ExecQuery(queryStr, QueryResultTypes.NonQuery);

            WriteEventIntoDeleteDataProtocol(
                DeleteDataEventKind.ddeInformation,
                string.Format("Удалены данные {0} ({1} строк).", semantic, recCount), true);
        }

        /// <summary>
        /// Удаляет данные по текущим pumpID, sourceID
        /// </summary>
        /// <param name="obj">Объект БД</param>
        /// <param name="constr">Строка ограничения</param>
		protected void DeleteTableData(IEntity obj, string constr)
        {
            DeleteTableData(obj, this.PumpID, this.SourceID, constr);
        }

        /// <summary>
        /// Удаляет данные таблиц фактов
        /// </summary>
        /// <param name="fct">Список таблиц фактов</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="constr">Ограничение</param>
        public void DirectDeleteFactData(IFactTable[] fct, int pumpID, int sourceID, string constr)
        {
            for (int i = 0; i < fct.GetLength(0); i++)
            {
                if (fct[i] != null)
                {
                    DeleteTableData(fct[i], pumpID, sourceID, constr);
                }
            }
        }

        private void ClearClsHierarchy(IClassifier cls, int pumpID, int sourceID, string constr)
        {
            string query = string.Format("Update {0} set ParentId = {1} ", cls.FullDBName, "''");
            string whereStr = MakeConstraintString(pumpID, sourceID, constr);
            if (whereStr != string.Empty)
                whereStr = "where " + whereStr.Remove(0, 4);
            query += whereStr;
            this.DB.ExecQuery(query, QueryResultTypes.NonQuery);
        }

        /// <summary>
        /// Удаляет данные таблиц классификаторов
        /// </summary>
        /// <param name="cls">Список таблиц классификаторов</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="constr">Ограничение</param>
        protected void DirectDeleteClsData(IClassifier[] cls, int pumpID, int sourceID, string constr)
        {
            for (int i = 0; i < cls.GetLength(0); i++)
            {
                if (cls[i] == null)
                    continue;
                // иногда из за иерархии вылезает оракловая ошибка (глюк оракла) 
                // ORA-00600: код внутр. ошибки, аргументы: [13001], [], [], [], [], [], [], [].
                // при очистке иерархии глюк пропадает
                if (this.ServerDBMSName == DBMSName.Oracle)
                {
                    ICollection<string> col = cls[i].Attributes.Keys;
                    if (col.Contains("ParentID"))
                        ClearClsHierarchy(cls[i], pumpID, sourceID, constr);
                }
                DeleteTableData(cls[i], pumpID, sourceID, constr);
            }
        }

        /// <summary>
        /// Удаляет данные протоколов
        /// </summary>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        protected void DirectDeleteProtocolData(int pumpID, int sourceID)
        {
            SetProgress(0, 0, "Удаление данных протоколов...", string.Empty, true);
            WriteToTrace("Удаление данных протоколов...", TraceMessageKind.Information);

            this.DataPumpProtocol.DeleteProtocolData(ModulesTypes.PreviewDataModule, sourceID, pumpID);
            this.DataPumpProtocol.DeleteProtocolData(ModulesTypes.DataPumpModule, sourceID, pumpID);
            this.ProcessDataProtocol.DeleteProtocolData(ModulesTypes.ProcessDataModule, sourceID, pumpID);
            this.AssociateDataProtocol.DeleteProtocolData(ModulesTypes.BridgeOperationsModule, sourceID, pumpID);
            //this.ProcessCubeProtocol.DeleteProtocolData(ModulesTypes.MDProcessingModule, sourceID, pumpID);
            //патчим для закачек 86н
            if (!ProgramIdentifier.StartsWith("BusGovRuPump."))
                this.ProcessCubeProtocol.DeleteProtocolData(ModulesTypes.MDProcessingModule, sourceID, pumpID);
            this.CheckDataProtocol.DeleteProtocolData(ModulesTypes.ReviseDataModule, sourceID, pumpID);
            this.DeleteDataProtocol.DeleteProtocolData(ModulesTypes.DeleteDataModule, sourceID, pumpID);

            SetProgress(0, 0, "Данные протоколов удалены.", string.Empty, true);
            WriteToTrace("Данные протоколов удалены.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Удалить данные истории закачки по ИД закачки
        /// </summary>
        /// <param name="pumpID">ИД закачки (-1 - удалить все)</param>
        protected void ClearPumpHistory(int pumpID)
        {
            WriteToTrace("Удаление данных логов...", TraceMessageKind.Information);

            string err = string.Empty;

            IPumpHistoryCollection phc = this.PumpRegistryElement.PumpHistoryCollection;
            if (phc != null)
            {
                if (pumpID > 0)
                {
                    err = phc.RemoveAt(pumpID);
                }
                else
                {
                    foreach (IPumpHistoryElement phe in phc)
                    {
                        phc.RemoveAt(phe.ID);
                    }
                }
            }

            if (err != string.Empty)
            {
                throw new Exception(err);
            }

            WriteToTrace("Данные логов удалены.", TraceMessageKind.Information);
        }

        /// <summary>
        /// Удаляет указанные записи из таблицы соответствия истории закачанным источникам
        /// </summary>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        private void ClearDataSources2PumpHistory(int pumpID, int sourceID)
        {
            if (sourceID > 0)
            {
                this.DB.ExecQuery(
                    "delete from DATASOURCES2PUMPHISTORY where REFDATASOURCES = ?",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("REFDATASOURCES", sourceID));
            }
            else if (pumpID > 0)
            {
                this.DB.ExecQuery(
                    "delete from DATASOURCES2PUMPHISTORY where REFPUMPHISTORY = ?",
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("REFPUMPHISTORY", pumpID));
            }
        }

        /// <summary>
        /// Удалить данные закачки по ИД закачки и/или источника (-1 - игнорировать)
        /// </summary>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        /// <param name="constr">Ограничение выборки</param>
        protected virtual void DirectDeleteData(int pumpID, int sourceID, string constr)
        {
            // Выполнение запросов
            DirectDeleteFactData(this.UsedFacts, pumpID, sourceID, constr);
            DirectDeleteClsData(this.UsedClassifiers, pumpID, sourceID, constr);

            ClearDataSources2PumpHistory(pumpID, sourceID);
        }

        /// <summary>
        /// Удалить данные закачки по ИД закачки и/или источника (-1 - игнорировать)
        /// </summary>
        /// <param name="db">Объект БД</param>
        /// <param name="pumpID">ИД закачки</param>
        /// <param name="sourceID">ИД источника</param>
        protected virtual void DirectDeleteData(int pumpID, int sourceID)
        {
            try
            {
                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeStart, pumpID, sourceID,
                    "Старт удаления данных.", null);

                // Выполнение запросов
                DirectDeleteFactData(this.UsedFacts, pumpID, sourceID, string.Empty);
                DirectDeleteClsData(this.UsedClassifiers, pumpID, sourceID, string.Empty);

                ClearDataSources2PumpHistory(pumpID, sourceID);

                SetProgress(-1, -1, "Данные удалены.", string.Empty, true);

                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeSuccefullFinished, pumpID, sourceID, 
                    "Удаление данных успешно завершено.", null);
            }
            catch (ThreadAbortException)
            {
                WriteEventIntoDeleteDataProtocol(
                    DeleteDataEventKind.ddeFinishedWithErrors, pumpID, sourceID, "Операция прервана пользователем.", null);
                throw;
            }
            catch (Exception ex)
            {
                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeFinishedWithErrors, pumpID, sourceID,
                    "Удаление данных завершено с ошибками", ex);
                throw;
            }
        }

        /// <summary>
        /// Удаляет зкачанные данные
        /// </summary>
        protected virtual void DeletePumpedData()
        {
            if (CheckClsRecordsAmount(this.UsedClassifiers, -1, this.SourceID, string.Empty) ||
                CheckFactRecordsAmount(this.UsedFacts, -1, this.SourceID, string.Empty))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, 
                    "Данные из текущего источника уже были закачаны. Закачанные данные будут удалены.");

                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.dpeStartDataSourceProcessing,
                    string.Format("Старт удаления данных источника ID {0}.", this.SourceID), true);

                DirectDeleteData(-1, this.SourceID, string.Empty);

                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.dpeSuccessfullFinishDataSourceProcess,
                    string.Format("Удаление данных источника ID {0} успешно завершено.", this.SourceID));

                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Закачанные данные удалены.", true);
            }
        }

        /// <summary>
        /// Удаляет ранее зкачанные данные
        /// </summary>
        protected virtual void DeleteEarlierPumpedData()
        {
            if (this.DeleteEarlierData)
            {
                DeletePumpedData();
            }
        }

        /// <summary>
        /// Удалить данные фактов и классификаторов
        /// </summary>
        /// <param name="constr">Ограничение на удаление данных</param>
        /// <param name="comment">Пояснение к операции удаления (дописывается к сообщению в лог)</param>
        /// <param name="deleteCls">Удалять и классификаторы</param>
        protected void DeleteData(string constr, string comment, bool deleteCls)
        {
            try
            {
                if (CheckFactRecordsAmount(this.UsedFacts, -1, this.SourceID, constr))
                {
                    WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeStart,
                        string.Format("Старт удаления данных. {0}", comment), true);

                    DirectDeleteFactData(this.UsedFacts, -1, this.SourceID, constr);

                    if (deleteCls)
                    {
                        DirectDeleteClsData(this.UsedClassifiers, -1, this.SourceID, constr);
                    }

                    SetProgress(-1, -1, "Данные удалены", string.Empty, true);

                    WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeSuccefullFinished,
                        "Удаление данных успешно завершено.", true);
                }
            }
            catch (ThreadAbortException)
            {
                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeFinishedWithErrors, "Операция прервана пользователем.");
                throw;
            }
            catch (Exception ex)
            {
                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeFinishedWithErrors, "Удаление данных завершено с ошибками", ex);
                throw;
            }
        }

        /// <summary>
        /// Удалить данные фактов
        /// </summary>
        /// <param name="constr">Ограничение на удаление данных</param>
        /// <param name="comment">Пояснение к операции удаления (дописывается к сообщению в лог)</param>
        protected void DeleteData(string constr, string comment)
        {
            DeleteData(constr, comment, false);
        }

        /// <summary>
        /// Удалить данные фактов
        /// </summary>
        /// <param name="constr">Ограничение на удаление данных</param>
        protected void DeleteData(string constr)
        {
            DeleteData(constr, string.Empty);
        }

        #endregion Функции удаления данных
    }
}