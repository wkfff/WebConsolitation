using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // Модуль функций управления этапами закачки (потоками этапов, последовательностью вызовов и др.)

    /// <summary>
    /// Базовый класс для всех закачек.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {
        #region Поля

        /// <summary>
        /// Главный поток закачки
        /// </summary>
        private Thread mainThread;
        /// <summary>
        /// Поток этапа закачки
        /// </summary>
        private Thread stageThread;
        /// <summary>
        /// Поток ожидания окончания этапа закачки
        /// </summary>
        private Thread stageWaitThread;
        private bool autoSuicide = false;
        private PumpProcessStates prevState = PumpProcessStates.Prepared;

        /// <summary>
        /// Эвент синхронизации цикла опроса состояния закачки главного потока с установкой состояния закачки
        /// (чтобы цикл не вертелся бесконечно, а только в момент установки свойства состояния закачки)
        /// </summary>
        private AutoResetEvent stateChangedEvent = new AutoResetEvent(true);
        /// <summary>
        /// Эвент синхронизации завершения потока этапа с завершением главного потока закачки
        /// (чтобы поток завершения этапа ждал этого самого завершения и чтобы закачка успела записать в лог 
        /// до удаления сессии)
        /// </summary>
        private AutoResetEvent endStageEvent = new AutoResetEvent(true);
        /// <summary>
        /// Эвент синхронизации старта потока этапа с потоком ожидания окончания этапа
        /// (чтобы поток ожидания не стартовал раньше потока этапа)
        /// </summary>
        private AutoResetEvent startStageEvent = new AutoResetEvent(true);
        /// <summary>
        /// Эвент синхронизации смены состояния закачки и его обработки - чтобы состояние не было изменено во
        /// время обработки, т.к. в этом случае оно не будет обработано
        /// </summary>
        private AutoResetEvent stateChangeProcessingEvent = new AutoResetEvent(false);
        /// <summary>
        /// Эвент синхронизации завершения потока ожидания завершения этапа с началом следующего этапа или 
        /// завершением всей закачки 
        /// </summary>
        private AutoResetEvent endWaitThreadEventEvent = new AutoResetEvent(true);

        #endregion Поля


        #region Общие функции

        /// <summary>
        /// Устанавливает этапы в состояние, начиная с указанного
        /// </summary>
        protected void SetStagesStateAfterStage(PumpProcessStates state, StageState stageState)
        {
            if (state <= PumpProcessStates.PreviewData)
                this.StagesQueue[PumpProcessStates.PreviewData].StageCurrentState = stageState;
            if (state <= PumpProcessStates.PumpData)
                this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = stageState;
            if (state <= PumpProcessStates.ProcessData)
                this.StagesQueue[PumpProcessStates.ProcessData].StageCurrentState = stageState;
            if (state <= PumpProcessStates.AssociateData)
                this.StagesQueue[PumpProcessStates.AssociateData].StageCurrentState = stageState;
            if (state <= PumpProcessStates.ProcessCube)
                this.StagesQueue[PumpProcessStates.ProcessCube].StageCurrentState = stageState;
            if (state <= PumpProcessStates.CheckData)
                this.StagesQueue[PumpProcessStates.CheckData].StageCurrentState = stageState;
        }

        /// <summary>
        /// Устанавливает этапы в состояние до с указанного этапа
        /// </summary>
        protected void SetStagesStateBeforeStage(PumpProcessStates state, StageState stageState)
        {
            if (state >= PumpProcessStates.PreviewData)
                this.StagesQueue[PumpProcessStates.PreviewData].StageCurrentState = stageState;
            if (state >= PumpProcessStates.PumpData)
                this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = stageState;
            if (state >= PumpProcessStates.ProcessData)
                this.StagesQueue[PumpProcessStates.ProcessData].StageCurrentState = stageState;
            if (state >= PumpProcessStates.AssociateData)
                this.StagesQueue[PumpProcessStates.AssociateData].StageCurrentState = stageState;
            if (state >= PumpProcessStates.ProcessCube)
                this.StagesQueue[PumpProcessStates.ProcessCube].StageCurrentState = stageState;
            if (state >= PumpProcessStates.CheckData)
                this.StagesQueue[PumpProcessStates.CheckData].StageCurrentState = stageState;
        }

        #endregion Общие функции


        #region Функции управления потоком

        /// <summary>
        /// Переключает текущий этап закачки на следующий
        /// </summary>
        /// <param name="state">Текущий этап</param>
        private void SwitchPumpProcessState()
        {
            if (this.State == PumpProcessStates.Aborted)
            {
                return;
            }

            if (prevState == PumpProcessStates.DeleteData || prevState == PumpProcessStates.CheckData)
            {
                this.State = PumpProcessStates.Finished;
                return;
            }

            for (int i = (int)prevState + 1; i <= (int)PumpProcessStates.CheckData; i++)
            {
                PumpProcessStates nextState = (PumpProcessStates)i;

                if (this.StagesQueue[nextState].StageInitialState == StageState.Blocked)
                {
                    //prevState = nextState;
                }
                else if (this.StagesQueue[nextState].StageInitialState == StageState.Skipped ||
                    this.StagesQueue[nextState].StageCurrentState == StageState.Skipped)
                {
                    //prevState = nextState;
                    this.ServerSideDataPumpProgress.OnStageSkipped(nextState);
                }
                else
                {
                    prevState = this.State;
                    this.State = nextState;
                    return;
                }
            }

            this.State = PumpProcessStates.Finished;
        }

        /// <summary>
        /// Функция-нить, ожидающая завершения процесса закачки
        /// </summary>
        private void WaitThreadFunction()
        {
            try
            {
                if (Thread.CurrentThread.Name == null)
                {
                    Thread.CurrentThread.Name = string.Format(
                        "{0}.{1} WaitThread", this.ProgramIdentifier, this.State);
                }

                WriteToTrace(string.Format("Запуск потока ожидания завершения этапа закачки {0}.", Thread.CurrentThread.Name),
                    TraceMessageKind.Information);

                endWaitThreadEventEvent.Reset();

                // Ожидаем завершения этапа закачки
                endStageEvent.WaitOne();

                // Этап закачки завершен !!!
                // Запись в очередь
                IStagesQueueElement sqe = this.StagesQueue[prevState];
                if (sqe != null)
                {
                    if (sqe.StageCurrentState != StageState.FinishedWithErrors &&
                        sqe.StageCurrentState != StageState.SuccefullFinished)
                    {
                        sqe.StageCurrentState = StageState.SuccefullFinished;
                    }

                    sqe.IsExecuted = true;
                    sqe.EndTime = DateTime.Now;

                    this.DataPumpProgress.ProgressCurrentPos = 0;
                    this.DataPumpProgress.ProgressMaxPos = 0;
                    this.DataPumpProgress.ProgressMessage = string.Empty;
                    this.DataPumpProgress.ProgressText = string.Empty;

                    this.ServerSideDataPumpProgress.OnPumpProcessStateChanged(prevState, this.State);
                    this.ServerSideDataPumpProgress.OnStageFinished(this.State);
                }

                if (this.State != PumpProcessStates.Paused && this.State != PumpProcessStates.Running)
                {
                    stageThread = null;
                }

                if (autoSuicide)
                {
                    this.State = PumpProcessStates.Finished;
                }
                else if (!locked)
                {
                    // Переключение этапов
                    SwitchPumpProcessState();
                }
            }
            catch (Exception ex)
            {
                WriteToTrace(ex.ToString(), TraceMessageKind.CriticalError, "DataPumpModuleBase");
                this.State = PumpProcessStates.Finished;
            }
            finally
            {
                endWaitThreadEventEvent.Set();
            }
        }

        /// <summary>
        /// Функция основного потока закачки
        /// </summary>
        private void MainThreadFunction()
        {
            Mutex mutex = null;

            try
            {
                if (Thread.CurrentThread.Name == null)
                {
                    Thread.CurrentThread.Name = this.ProgramIdentifier + " MainThread";
                }

                WriteToTrace(string.Format("Запрос на владение мьютексом {0}.", GetCurrentPumpMutexName()),
                    TraceMessageKind.Information);

                // Делаем текущий поток владельцем мьютекса
                mutex = GetCurrentPumpMutex();
                mutex.WaitOne();

                WriteToTrace("Главный поток закачки владеет мьютексом.", TraceMessageKind.Information);

                while (exitFlag)
                {
                    // Ждем пока не будет установлено состояние программы закачки
                    stateChangedEvent.WaitOne();
                    stateChangeProcessingEvent.Reset();

                    if (this.StagesQueue.ContainsStage(this.State))
                    {
                        if (this.StagesQueue[this.State].StageInitialState == StageState.Blocked ||
                            this.StagesQueue[this.State].StageInitialState == StageState.OutOfQueue ||
                            this.StagesQueue[this.State].StageInitialState == StageState.Skipped)
                        {
                            stateChangeProcessingEvent.Set();
                            SwitchPumpProcessState();
                            continue;
                        }
                    }

                    // Устанавливаем параметры очереди
                    IStagesQueueElement sqe = this.StagesQueue[this.State];
                    if (sqe != null)
                    {
                        sqe.StartTime = DateTime.Now;
                        sqe.StageCurrentState = StageState.InProgress;
                        this.ServerSideDataPumpProgress.OnStageStarted(this.State);
                    }
                    //WriteToTrace(this.DataPumpInfo.PumpProcessStatesToString(this.State), TraceMessageKind.Information);

                    try
                    {
                        // Анализ состояния
                        switch (this.State)
                        {
                            case PumpProcessStates.Prepared:
                                this.ServerSideDataPumpProgress.OnPumpProcessStateChanged(prevState, this.State);

                                break;

                            case PumpProcessStates.Running:
                                if (stageThread != null)
                                {
                                    this.ServerSideDataPumpProgress.OnPumpProcessStateChanged(prevState, this.State);
                                    this.ServerSideDataPumpProgress.OnStageResumed(prevState);
                                }

                                break;

                            case PumpProcessStates.Paused:
                                if (stageThread != null)
                                {
                                    this.ServerSideDataPumpProgress.OnPumpProcessStateChanged(prevState, this.State);
                                    this.ServerSideDataPumpProgress.OnStagePaused(prevState);
                                }

                                break;

                            case PumpProcessStates.Finished:
                                this.ServerSideDataPumpProgress.OnPumpProcessStateChanged(prevState, this.State);
                                exitFlag = false;

                                //mutex.ReleaseMutex();

                                break;

                            case PumpProcessStates.Aborted:
                                if (stageThread != null)
                                {
                                    stageThread.Abort();
                                    endWaitThreadEventEvent.WaitOne();
                                }

                                this.ServerSideDataPumpProgress.OnPumpProcessStateChanged(prevState, this.State);
                                this.ServerSideDataPumpProgress.OnStageStopped(prevState);
                                exitFlag = false;

                                //mutex.ReleaseMutex();

                                break;

                            // Пропустить этап
                            case PumpProcessStates.Skip:
                                IStagesQueueElement prevSqe = this.StagesQueue[prevState];
                                if (prevSqe != null)
                                {
                                    prevSqe.IsExecuted = true;
                                    prevSqe.EndTime = DateTime.Now;
                                    prevSqe.StageCurrentState = StageState.Skipped;
                                }

                                this.ServerSideDataPumpProgress.OnPumpProcessStateChanged(prevState, this.State);
                                this.ServerSideDataPumpProgress.OnStageSkipped(prevState);

                                stageThread.Abort();

                                endWaitThreadEventEvent.WaitOne();

                                //mutex.ReleaseMutex();

                                break;

                            default:
                                this.ServerSideDataPumpProgress.OnPumpProcessStateChanged(prevState, this.State);

                                // Запуск функции этапа закачки
                                StartStage(this.State);

                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToTrace(ex.ToString(), TraceMessageKind.CriticalError, "DataPumpModuleBase");
                        exitFlag = false;
                    }

                    stateChangedEvent.Reset();
                    stateChangeProcessingEvent.Set();
                }
            }
            catch (Exception ex)
            {
                WriteToTrace(ex.ToString(), TraceMessageKind.CriticalError, "DataPumpModuleBase");
            }
            finally
            {
                try
                {
                    if (mutex != null) mutex.ReleaseMutex();
                }
                catch { }
            }
        }

        /// <summary>
        /// Запускает функцию этапа закачки
        /// </summary>
        /// <param name="state">Этап</param>
        private void StartStage(PumpProcessStates state)
        {
            prevState = state;

            startStageEvent.Reset();

            if (stageThread == null)
            {
                WriteToTrace("Создание потока этапа закачки.", TraceMessageKind.Information);

                switch (state)
                {
                    case PumpProcessStates.PreviewData:
                        stageThread = new Thread(new ThreadStart(this.PreviewData));
                        break;

                    case PumpProcessStates.PumpData:
                        stageThread = new Thread(new ThreadStart(this.PumpData));
                        break;

                    case PumpProcessStates.ProcessData:
                        stageThread = new Thread(new ThreadStart(this.ProcessData));
                        break;

                    case PumpProcessStates.AssociateData:
                        stageThread = new Thread(new ThreadStart(this.AssociateData));
                        break;

                    case PumpProcessStates.ProcessCube:
                        stageThread = new Thread(new ThreadStart(this.ProcessCube));
                        break;

                    case PumpProcessStates.CheckData:
                        stageThread = new Thread(new ThreadStart(this.CheckData));
                        break;
                }

                // Запускаем поток этапа закачки
                stageThread.Name = string.Format("{0}.{1} MainThread", this.ProgramIdentifier, state);
                WriteToTrace(string.Format("Запуск потока этапа закачки {0}.", stageThread.Name), 
                    TraceMessageKind.Information);
                //stageThread.Priority = ThreadPriority.BelowNormal;
                stageThread.Start();

                // Ждем, пока не запустится поток этапа закачки
                startStageEvent.WaitOne();

                // Создаем нить, которая будет ожидать завершения процесса закачки, 
                // а затем выгружать программу закачки из памяти
                stageWaitThread = new Thread(new ThreadStart(this.WaitThreadFunction));
                stageWaitThread.Start();
            }
        }

        #endregion Функции управления потоком
    }
}