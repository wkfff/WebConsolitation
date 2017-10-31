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
    // ������ ������� ���������� ������� ������� (�������� ������, ������������������� ������� � ��.)

    /// <summary>
    /// ������� ����� ��� ���� �������.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {
        #region ����

        /// <summary>
        /// ������� ����� �������
        /// </summary>
        private Thread mainThread;
        /// <summary>
        /// ����� ����� �������
        /// </summary>
        private Thread stageThread;
        /// <summary>
        /// ����� �������� ��������� ����� �������
        /// </summary>
        private Thread stageWaitThread;
        private bool autoSuicide = false;
        private PumpProcessStates prevState = PumpProcessStates.Prepared;

        /// <summary>
        /// ����� ������������� ����� ������ ��������� ������� �������� ������ � ���������� ��������� �������
        /// (����� ���� �� �������� ����������, � ������ � ������ ��������� �������� ��������� �������)
        /// </summary>
        private AutoResetEvent stateChangedEvent = new AutoResetEvent(true);
        /// <summary>
        /// ����� ������������� ���������� ������ ����� � ����������� �������� ������ �������
        /// (����� ����� ���������� ����� ���� ����� ������ ���������� � ����� ������� ������ �������� � ��� 
        /// �� �������� ������)
        /// </summary>
        private AutoResetEvent endStageEvent = new AutoResetEvent(true);
        /// <summary>
        /// ����� ������������� ������ ������ ����� � ������� �������� ��������� �����
        /// (����� ����� �������� �� ��������� ������ ������ �����)
        /// </summary>
        private AutoResetEvent startStageEvent = new AutoResetEvent(true);
        /// <summary>
        /// ����� ������������� ����� ��������� ������� � ��� ��������� - ����� ��������� �� ���� �������� ��
        /// ����� ���������, �.�. � ���� ������ ��� �� ����� ����������
        /// </summary>
        private AutoResetEvent stateChangeProcessingEvent = new AutoResetEvent(false);
        /// <summary>
        /// ����� ������������� ���������� ������ �������� ���������� ����� � ������� ���������� ����� ��� 
        /// ����������� ���� ������� 
        /// </summary>
        private AutoResetEvent endWaitThreadEventEvent = new AutoResetEvent(true);

        #endregion ����


        #region ����� �������

        /// <summary>
        /// ������������� ����� � ���������, ������� � ����������
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
        /// ������������� ����� � ��������� �� � ���������� �����
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

        #endregion ����� �������


        #region ������� ���������� �������

        /// <summary>
        /// ����������� ������� ���� ������� �� ���������
        /// </summary>
        /// <param name="state">������� ����</param>
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
        /// �������-����, ��������� ���������� �������� �������
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

                WriteToTrace(string.Format("������ ������ �������� ���������� ����� ������� {0}.", Thread.CurrentThread.Name),
                    TraceMessageKind.Information);

                endWaitThreadEventEvent.Reset();

                // ������� ���������� ����� �������
                endStageEvent.WaitOne();

                // ���� ������� �������� !!!
                // ������ � �������
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
                    // ������������ ������
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
        /// ������� ��������� ������ �������
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

                WriteToTrace(string.Format("������ �� �������� ��������� {0}.", GetCurrentPumpMutexName()),
                    TraceMessageKind.Information);

                // ������ ������� ����� ���������� ��������
                mutex = GetCurrentPumpMutex();
                mutex.WaitOne();

                WriteToTrace("������� ����� ������� ������� ���������.", TraceMessageKind.Information);

                while (exitFlag)
                {
                    // ���� ���� �� ����� ����������� ��������� ��������� �������
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

                    // ������������� ��������� �������
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
                        // ������ ���������
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

                            // ���������� ����
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

                                // ������ ������� ����� �������
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
        /// ��������� ������� ����� �������
        /// </summary>
        /// <param name="state">����</param>
        private void StartStage(PumpProcessStates state)
        {
            prevState = state;

            startStageEvent.Reset();

            if (stageThread == null)
            {
                WriteToTrace("�������� ������ ����� �������.", TraceMessageKind.Information);

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

                // ��������� ����� ����� �������
                stageThread.Name = string.Format("{0}.{1} MainThread", this.ProgramIdentifier, state);
                WriteToTrace(string.Format("������ ������ ����� ������� {0}.", stageThread.Name), 
                    TraceMessageKind.Information);
                //stageThread.Priority = ThreadPriority.BelowNormal;
                stageThread.Start();

                // ����, ���� �� ���������� ����� ����� �������
                startStageEvent.WaitOne();

                // ������� ����, ������� ����� ������� ���������� �������� �������, 
                // � ����� ��������� ��������� ������� �� ������
                stageWaitThread = new Thread(new ThreadStart(this.WaitThreadFunction));
                stageWaitThread.Start();
            }
        }

        #endregion ������� ���������� �������
    }
}