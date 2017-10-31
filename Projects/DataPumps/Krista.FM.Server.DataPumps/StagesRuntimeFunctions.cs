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
    // ������ �������, ���������� �� ����� ���������� ������ ������� (�������������, ����� ������� ���������,
    // ������������� � ������� �����).

    /// <summary>
    /// ������� ����� ��� ���� �������.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject, IDataPumpModule
    {
        #region ������ ������ �������


        #region ����� ������

        /// <summary>
        /// ������������� �������� ��������� ���������������, ���� ��� �� ���� ����������� � �������
        /// </summary>
        protected void InitAuxiliaryClss()
        {
            if (hierarchyClassifiers == null)
                hierarchyClassifiers = usedClassifiers;

            if (associateClassifiers == null)
                associateClassifiers = usedClassifiers;

            // ������ associateClassifiersEx �� ����������������, �.�. ���� �� �� ����������,
            // �� ��������������� (�� ����������, ������������ �� ���������� �������) ��� � �� ������������ �� �����

            if (cubeClassifiers == null)
                cubeClassifiers = usedClassifiers;

            // ������ versionClassifiers �� ����������������, �.�. ���� �� �� ����������,
            // �� ���������������, ��������� ��������� ������, ���

            if (cubeFacts == null)
                cubeFacts = usedFacts;
        }

        #endregion ����� ������


        #region ��������������� ��������

        /// <summary>
        /// ��������������� �������� ������ �������. 
        /// ���������������� � �������� ��� ���������� �������� �� ������������� ������.
        /// </summary>
        protected virtual void DirectPreviewData()
        {
            //WriteEventIntoPreviewDataProtocol(
            //    PreviewDataEventKind.dpeStart, "�� ������ ����� ������� �������� �� �����������.");
        }

        /// <summary>
        /// ��������������� �������� ������ �������. 
        /// ��������� ���������������� � ����������� ��������. ���������� ������� �������
        /// </summary>
        private void PreviewData()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
            {
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "������������� � ���������� ������ ��� �������������...", string.Empty, true);
                WriteToTrace("������������� � ���������� ������ ��� �������������...", TraceMessageKind.Information);

                PreparePreviewData();

                // ����� � �������� � ������ �������
                WriteEventIntoPreviewDataProtocol(PreviewDataEventKind.dpeStart, "����� �������������.");

                // �������� ���� �� ������ �������
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
                WriteEventIntoPreviewDataProtocol(PreviewDataEventKind.dpeCriticalError, "������ ���������� �������", ex);
                //WriteStringToErrorFile(ex.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // ���������� ��� ��������� �����
                        SetStagesStateAfterStage(PumpProcessStates.PumpData, StageState.Skipped);
                        this.StagesQueue[PumpProcessStates.PreviewData].StageCurrentState = StageState.FinishedWithErrors;
                        WriteEventIntoPreviewDataProtocol(PreviewDataEventKind.dpeFinishedWithErrors,
                            "�������� �������� �������������. ��� ��������� ����� ���������.");
                    }
                    else
                    {
                        if (!finishedWithErrors)// && this.PumpedSources.Count > 0)
                        {
                            this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = StageState.SuccefullFinished;
                            WriteEventIntoPreviewDataProtocol(PreviewDataEventKind.dpeSuccefullFinished,
                                "������� ������� ���������. " + MakeDataSourcesVault());
                        }
                        else
                        {
                            // ���������� ��� ��������� �����
                            SetStagesStateAfterStage(PumpProcessStates.PumpData, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.PreviewData].StageCurrentState = StageState.FinishedWithErrors;
                            WriteEventIntoPreviewDataProtocol(PreviewDataEventKind.dpeFinishedWithErrors,
                                "������� ��������� � ��������. ��� ��������� ����� ���������. " + MakeDataSourcesVault());
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

        #endregion ��������������� ��������


        #region ������� ������

        /// <summary>
        /// ������� ������. 
        /// ���������������� � �������� ��� ���������� �������� �� ������� ������.
        /// </summary>
        protected virtual void DirectPumpData()
        {
            WriteEventIntoDataPumpProtocol(
                DataPumpEventKind.dpeStart, "�� ������ ����� ������� �������� �� �����������.");
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
        /// ������� ������. 
        /// ��������� ���������������� � ����������� ��������. ���������� ������� �������
        /// </summary>
        private void PumpData()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
			{
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "������������� � ���������� ������ ��� �������...", string.Empty, true);
                WriteToTrace("������������� � ���������� ������ ��� �������...", TraceMessageKind.Information);

                PreparePumpData();

				// ����� � �������� � ������ �������
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "����� �������.");

                // �������� ���� �� ������ �������
                LogicalCallContextData l = LogicalCallContextData.GetContext();
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                // �������������� ��������� ������� (��������� ��������, �������������, ������ ���������)
                InitAuxiliaryClss();

                DirectPumpData();

                // ������� ������ "����������� ������" �� ������ ��������������� �� ���������� ����������
                //DeleteUnusedClsFixedRows();

                // ������ � �������� � ������ ��������� ��������
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "����� ��������� �������� �� ����� �������.");
                // ��������� ��������
                ClsHierarchySetting();

                // �������� ���������� � ������� �������������� ��� ��������� �������
                MarkClsAsInvalidate();

                // ���������� ��������� �� �������� ���������� �����
                SendMessage(string.Format("������� ������� ({0} - ���� ������� ������) ������� ��������", this.ProgramIdentifier),
                    MessageImportance.Regular, MessageType.PumpMessage, this.PumpID.ToString());
            }
            catch (ThreadAbortException)
            {
                threadIsAbort = true;
            }
            catch (Exception ex)
            {
                finishedWithErrors = true;
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeCriticalError, "������ ���������� �������", ex);
                //WriteStringToErrorFile(ex.ToString());

                // ���������� ��������� � ���������� ����� c ��������
                SendMessage(string.Format("������� ������� ({0} - ���� ������� ������) �������� � ��������", this.ProgramIdentifier),
                    MessageImportance.Importance, MessageType.PumpMessage, this.PumpID.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // ���������� ��� ��������� �����
                        SetStagesStateAfterStage(PumpProcessStates.ProcessData, StageState.Skipped);
                        this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = StageState.FinishedWithErrors;
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishedWithErrors, 
                            "�������� �������� �������������. ��� ��������� ����� ���������.");
                    }
                    else
                    {
                        if (this.PumpedSources.Count == 0)
                        {
                            // ���������� ��� ��������� �����
                            SetStagesStateAfterStage(PumpProcessStates.ProcessData, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = StageState.SuccefullFinished;
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccefullFinished, "������� ������� ���������.");
                        }
                        else if (!finishedWithErrors)
                        {
                            this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = StageState.SuccefullFinished;
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccefullFinished, 
                                "������� ������� ���������. " + MakeDataSourcesVault());
                        }
                        else
                        {
                            // ���������� ��� ��������� �����
                            SetStagesStateAfterStage(PumpProcessStates.ProcessData, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.PumpData].StageCurrentState = StageState.FinishedWithErrors;
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishedWithErrors, 
                                "������� ��������� � ��������. ��� ��������� ����� ���������. " + MakeDataSourcesVault());
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

        #endregion ������� ������


        #region ��������� ������

        /// <summary>
        /// ���������� ���������� ������. 
        /// ���������������� � �������� ��� ���������� �������� �� ��������� ������.
        /// </summary>
        protected virtual void DirectProcessData()
        {
            WriteEventIntoProcessDataProtocol(
                ProcessDataEventKind.pdeInformation, "�� ������ ����� ������� �������� �� �����������.");
        }

        /// <summary>
        /// ���������� ���������� ������. 
        /// ��������� ���������������� � ����������� ��������. ���������� ������� �������
        /// </summary>
        private void ProcessData()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
			{
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "������������� � ���������� ������ ��� ���������...", string.Empty, true);
                WriteToTrace("������������� � ���������� ������ ��� ���������...", TraceMessageKind.Information);

				PrepareProcessData();

                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart, "����� ��������� ������.");

                // �������� ���� �� ������ �������
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                // �������������� ��������� ������� (��������� ��������, �������������, ������ ���������)
                InitAuxiliaryClss();

                DirectProcessData();

                // ������ � �������� � ������ ��������� ��������
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeStart, "����� ��������� �������� �� ����� ���������.");
                // ��������� ��������
                ClsHierarchySetting();

                // �������� ���������� � ������� �������������� ��� ��������� �������
                MarkClsAsInvalidate();

                // ����� �������� �������� �������������, ����� ����� ������ � ������������ �������������� ��� �� ����� �������
                // � ������, ���� �������� ������� ���������� ����������
                if (this.VersionClassifiers != null)
                    foreach (IClassifier cls in this.VersionClassifiers)
                        ClearPresentationContext(cls);

                // ���������� ��������� �� �������� ���������� �����
                SendMessage(string.Format("������� ������� ({0} - ���� ��������� ������) ������� ��������", this.ProgramIdentifier),
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
                    string.Format("������ ���������� ��������� ������: {0}", ex.Message));
                //WriteStringToErrorFile(ex.ToString());

                // ���������� ��������� � ���������� ����� c ��������
                SendMessage(string.Format("������� ������� ({0} - ���� ��������� ������) �������� � ��������", this.ProgramIdentifier),
                    MessageImportance.Importance, MessageType.PumpProcessMessage, this.PumpID.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // ���������� ��� ��������� �����
                        SetStagesStateAfterStage(PumpProcessStates.AssociateData, StageState.Skipped);
                        this.StagesQueue[PumpProcessStates.ProcessData].StageCurrentState = StageState.FinishedWithErrors;
                        WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeFinishedWithErrors,
                            "��������� ������ ��������� � ��������: �������� �������� �������������.");
                    }
                    else
                    {
                        if (!finishedWithErrors)
                        {
                            this.StagesQueue[PumpProcessStates.ProcessData].StageCurrentState = StageState.SuccefullFinished;
                            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeSuccefullFinished, "��������� ������ ���������.");
                        }
                        else
                        {
                            // ���������� ��� ��������� �����
                            SetStagesStateAfterStage(PumpProcessStates.AssociateData, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.ProcessData].StageCurrentState = StageState.FinishedWithErrors;
                            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeFinishedWithErrors,
                                "��������� ������ ��������� � ��������.");
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

        #endregion ��������� ������


        #region ������������� ������

        /// <summary>
        /// ��������� ��������� (������� ���������� �� ��������� �������) ��������������� ��������������.
        /// ���������������� � ��������.
        /// </summary>
        /// <param name="pumpSourceID">ID ��������� �������</param>
        /// <returns>ID ��������� ��������������</returns> 
        protected virtual int GetClsSourceID(int pumpSourceID)
        {
            return sourceID;
        }

        // ������������ ������ ID ���������� ���������������
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

        // C������������ ���������������, ������������� �� ����������, ������������ �� ���������� �������
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
                    DoBridgeCls(clsDataSources[i], string.Format("�������� {0} �� {1}", i + 1, clsDataSources.Count),
                        this.AssociateClassifiersEx);
                }
            }
            finally
            {
                clsDataSources.Clear();
            }
        }

        /// <summary>
        /// ����������� ������.
        /// ���������������� � �������� ��� ���������� �������� �� ������������� ������.
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
                    BridgeOperationsEventKind.boeError, "��� ������", "��� ������",
                    "������ ��� ��������� �������� ������: ��� ����������, ���������� ������ �� ������� " +
                    "����� �� ���� ����������.", this.PumpID, -1);
                WriteToTrace("ERROR: ��� ����������, ���������� ������ �� ������� " +
                    "����� �� ���� ����������.", TraceMessageKind.Error);
            }
            else
            {
                int i = 1;
                // ������������ ��������������, ������������� �� ���������� �������
                foreach (KeyValuePair<int, string> dataSource in dataSources)
                {
                    SetDataSource(dataSource.Key);
                    DoBridgeCls(dataSource.Key,
                        string.Format("�������� {0} �� {1}", i, dataSources.Count), this.AssociateClassifiers);
                    i++;
                }
                // ������������ ��������������, ������������� �� ����������, ������������ �� ���������� �������
                AssociateClssEx(dataSources);
            }
        }

        /// <summary>
        /// ����������� ������.
        /// ��������� ���������������� � ����������� ��������. ���������� ������� �������
        /// </summary>
        private void AssociateData()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
            {
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "������������� � ���������� ������ ��� �������������...", string.Empty, true);
                WriteToTrace("������������� � ���������� ������ ��� �������������...", TraceMessageKind.Information);

                PrepareAssociateData();

                this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                    BridgeOperationsEventKind.boeStart, "��� ������", "��� ������",
                    "����� ������������� ������.", this.PumpID, -1);

                // �������� ���� �� ������ �������
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                // �������������� ��������� ������� (��������� ��������, �������������, ������ ���������)
                InitAuxiliaryClss();

                DirectAssociateData();

                // ���������� ��������� �� �������� ���������� �����
                SendMessage(string.Format("������� ������� ({0} - ���� �������������) ������� ��������", this.ProgramIdentifier),
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
                    BridgeOperationsEventKind.boeError, "��� ������", "��� ������",
                    string.Format("������ ���������� ������������� ������: {0}", ex.Message),
                    this.PumpID, this.SourceID);
                //WriteStringToErrorFile(ex.ToString());

                // ���������� ��������� � ���������� ����� c ��������
                SendMessage(string.Format("������� ������� ({0} - ���� �������������) �������� � ��������", this.ProgramIdentifier),
                    MessageImportance.Importance, MessageType.PumpAssociateMessage, this.PumpID.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // ���������� ��� ��������� �����
                        SetStagesStateAfterStage(PumpProcessStates.ProcessCube, StageState.Skipped);
                        this.StagesQueue[PumpProcessStates.AssociateData].StageCurrentState = StageState.FinishedWithErrors;
                        this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                            BridgeOperationsEventKind.boeFinishedWithError, "��� ������", "��� ������",
                            "������������� ������ ��������� � ��������: �������� �������� �������������.",
                            this.PumpID, -1);
                    }
                    else
                    {
                        if (!finishedWithErrors)
                        {
                            this.StagesQueue[PumpProcessStates.AssociateData].StageCurrentState = StageState.SuccefullFinished;
                            this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                                BridgeOperationsEventKind.boeSuccefullFinished, "��� ������", "��� ������",
                                "������������� ������ ���������.",
                                this.PumpID, -1);
                        }
                        else
                        {
                            // ���������� ��� ��������� �����
                            SetStagesStateAfterStage(PumpProcessStates.ProcessCube, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.AssociateData].StageCurrentState = StageState.FinishedWithErrors;
                            this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                                BridgeOperationsEventKind.boeFinishedWithError, "��� ������", "��� ������",
                                "������������� ������ ��������� � ��������.",
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

        #endregion ������������� ������


        #region ������ �����

        // ������ (objectKey1, cubeName1, ...)
        // ����� ��� ������ ������� ����� ��������, ���� �� ��� ��� ���������
        public string[] cubesForProcess = new string[] { };
        public string[] dimensionsForProcess = new string[] { };

        /// <summary>
        /// ���������� ����.
        /// ���������������� � �������� ��� ���������� �������� �� ������� �����.
        /// </summary>
        protected virtual void DirectProcessCube()
        {
            // �� ������ ������ ������ ���������� ���������...
            LogicalCallContextData callContext = LogicalCallContextData.GetContext();

            Guid batchGuid = Guid.Empty;
            try
            {
                // ������� ����� ����� ��� �������
                batchGuid = scheme.Processor.CreateBatch();

                // ��������� ������������� ������ � ���� ������ � ������� PumpHistory ��� ������� �������� �������
                this.PumpRegistryElement.PumpHistoryCollection[this.pumpID].BatchID = batchGuid.ToString();

                string startInfoMessage = String.Format("����� �������� ����������� �������� �� ������. ������������� ������ \"{0}\"", batchGuid);
                
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
                    // � ����� ��������� ������� (������� ������������ ��������� ������ ������) 
                    // ������� ������������ �� ��� ������� � ��� �������� �������� ������ (��� �� ��� ��� �������)
                    // � ����� ������ ���������� ��� � �������� ������ ����� � �������� -1
                    if (this.PumpedSources.Keys.Count == 0)
                    {
                        ProcessCubes(this.CubeFacts, this.CubeClassifiers, batchGuid);
                    }
                    else
                    {
                        foreach (int dataSourceID in this.PumpedSources.Keys)
                        {
                            SetDataSource(dataSourceID);
                            // ������ ����� ��� ������������� ���������
                            ProcessCubes(this.CubeFacts, this.CubeClassifiers, batchGuid);
                        }
                    }
                }
                else
                {
                    //  ������ ����� ��� ���� ���������� ������� (SourceID = -1)
                    ProcessCubes(this.CubeFacts, this.CubeClassifiers, batchGuid);
                }

                // ���������� ����� �� ������
                scheme.Processor.ProcessManager.StartBatch(batchGuid);

                string infoMessage = "����������� �������  ������� ���������� �� ������";
                WriteToTrace(infoMessage, TraceMessageKind.Information);
                this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                    this.ProgramIdentifier,
                    MDProcessingEventKind.mdpeSuccefullFinished,
                    infoMessage,
                    batchGuid.ToString(), batchGuid.ToString(),
                    OlapObjectType.Database, batchGuid.ToString());

                // ��������� ������� ��� �������
                WriteToTrace("������ ��������� �������", TraceMessageKind.Information);
                GenerateIPadReports(batchGuid);
                WriteToTrace("���������� ��������� �������", TraceMessageKind.Information);
            }
            catch (ThreadAbortException)
            {
                if (batchGuid != Guid.Empty)
                {
                    scheme.Processor.RevertBatch(batchGuid);
                }

                string errorMessage = "�������� ����������� �������� �� ������ ��������� � ��������: �������� �������� �������������";
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
                    "�������� ����������� �������� �� ������ ��������� � ��������: {0}",
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

        #region ��������� ������� ������

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

        // ��������� ������� ��� ������
        private const string PUMP_GENERATE_FOLDER = "PumpGenerateReports";
        private const string PUMP_GENERATE_PARAMS_FILENAME = "PumpGenerateParams.xml";
        private void GenerateIPadReports(Guid batchGuid)
        {
            DirectoryInfo dir = new DirectoryInfo(string.Format("{0}\\{1}", GetCurrentDir().FullName, PUMP_GENERATE_FOLDER));
            string pumpFileName = string.Format("{0}.xml", this.ProgramIdentifier);
            if (dir.GetFiles(pumpFileName, SearchOption.TopDirectoryOnly).GetLength(0) == 0)
            {
                WriteToTrace(string.Format("���� {0} �� ������", pumpFileName), TraceMessageKind.Warning);
                return;
            }
            else
                WriteToTrace(string.Format("���� �� ������� ������������ �������: {0}", pumpFileName), TraceMessageKind.Information);

            try
            {
                // ��������� �������� ������ ����� ���������� ��������� ������
                BatchState bs = scheme.Processor.GetBatchState(batchGuid);
                while ((bs != BatchState.Complited) && (bs != BatchState.ComplitedWithError) && (bs != BatchState.Deleted))
                {
                    Thread.Sleep(1000 * 30);
                    bs = scheme.Processor.GetBatchState(batchGuid);
                }
            }
            catch (Exception exc)
            {
                WriteToTrace(string.Format("������ ��� ��������� ��������� ������: {0} ({1})", exc.Message, exc.StackTrace), TraceMessageKind.Warning);
            }

            pumpFileName = string.Format("{0}\\{1}", dir, pumpFileName);
            string pumpGenerateParamsFileName = string.Format("{0}\\{1}", dir, PUMP_GENERATE_PARAMS_FILENAME);
            if (dir.GetFiles(PUMP_GENERATE_PARAMS_FILENAME, SearchOption.TopDirectoryOnly).GetLength(0) == 0)
            {
                WriteToTrace(string.Format("���� {0} �� ������", pumpGenerateParamsFileName), TraceMessageKind.Warning);
                return;
            }

            string bootloadServiceUri = string.Empty;
            string serverURL = string.Empty;
            string reportsHostUrl = string.Empty;
            GenerateGetParams(pumpGenerateParamsFileName, ref bootloadServiceUri, ref serverURL, ref reportsHostUrl);
            WriteToTrace(string.Format("��������� ���������: bootloadServiceUri={0}; serverURL={1}; reportsHostUrl={2};",
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
                    string.Format("������ ��� ��������� �������: {0}", ex.Message), string.Empty);
            }
        }

        #endregion ��������� ������� ������

        /// <summary>
        /// ���������� ����.
        /// ��������� ���������������� � ����������� ��������. ���������� ������� �������
        /// </summary>
        private void ProcessCube()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
            {
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "������������� � ���������� ������ ��� ������� �����...", string.Empty, true);
                WriteToTrace("������������� � ���������� ������ ��� ������� �����...", TraceMessageKind.Information);

                PrepareProcessCube();

                //this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                //    MDProcessingEventKind.mdpeStart,
                //    "����� ������� �����.", "��� ������", this.PumpID, -1);

                // �������� ���� �� ������ �������
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                // �������������� ��������� ������� (��������� ��������, �������������, ������ ���������)
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
                    string.Format("������ ���������� ������� �����: {0}", ex.Message), "��� ������");
                //WriteStringToErrorFile(ex.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // ���������� ��� ��������� �����
                        SetStagesStateAfterStage(PumpProcessStates.CheckData, StageState.Skipped);
                        this.StagesQueue[PumpProcessStates.ProcessCube].StageCurrentState = StageState.FinishedWithErrors;
                        this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                            MDProcessingEventKind.mdpeFinishedWithError,
                            "������ ����� �������� � ��������: �������� �������� �������������.", "��� ������");
                    }
                    else
                    {
                        if (!finishedWithErrors)
                        {
                            this.StagesQueue[PumpProcessStates.ProcessCube].StageCurrentState = StageState.SuccefullFinished;
                            //this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                            //    MDProcessingEventKind.mdpeSuccefullFinished,
                            //    "������ ����� ��������.", "��� ������", this.PumpID, -1);
                        }
                        else
                        {
                            // ���������� ��� ��������� �����
                            SetStagesStateAfterStage(PumpProcessStates.CheckData, StageState.Skipped);
                            this.StagesQueue[PumpProcessStates.ProcessCube].StageCurrentState = StageState.FinishedWithErrors;
                            this.ProcessCubeProtocol.WriteEventIntoMDProcessingProtocol(
                                MDProcessingEventKind.mdpeFinishedWithError,
                                "������ ����� �������� � ��������.", "��� ������");
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

        #endregion ������ �����


        #region �������� ������

        /// <summary>
        /// ��������� ���������� ������.
        /// ���������������� � �������� ��� ���������� �������� �� �������� ���������� ������.
        /// </summary>
        protected virtual void DirectCheckData()
        {
            this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                ReviseDataEventKind.pdeInformation,
                "�� ������ ����� ������� �������� �� �����������.", this.PumpID, -1);
        }

        /// <summary>
        /// ��������� ���������� ������.
        /// ��������� ���������������� � ����������� ��������. ���������� ������� �������
        /// </summary>
        private void CheckData()
        {
            bool finishedWithErrors = false;
            bool threadIsAbort = false;

            try
            {
                endStageEvent.Reset();

                startStageEvent.Set();

                SetProgress(0, 0, "������������� � ���������� ������ ��� ��������...", string.Empty, true);
                WriteToTrace("������������� � ���������� ������ ��� ��������...", TraceMessageKind.Information);

                PrepareCheckData();

                this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                    ReviseDataEventKind.pdeStart,
                    "����� �������� ������.", this.PumpID, -1);

                // �������� ���� �� ������ �������
                this.Scheme.UsersManager.CheckPermissionForSystemObject(
                    Convert.ToString(this.ProgramIdentifier), (int)DataPumpOperations.StartPump, true);

                InitDBObjects();

                DirectCheckData();

                // ���������� ��������� �� �������� ���������� �����
                SendMessage(string.Format("������� ������� ({0} - ���� �������� ������) ������� ��������", this.ProgramIdentifier),
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
                    string.Format("������ ���������� �������� ������: {0}", ex.Message),
                    this.PumpID, this.SourceID);
                //WriteStringToErrorFile(ex.ToString());

                // ���������� ��������� � ���������� ����� c ��������
                SendMessage(string.Format("������� ������� ({0} - ���� �������� ������) �������� � ��������", this.ProgramIdentifier),
                    MessageImportance.Importance, MessageType.PumpCheckDataMessage, this.PumpID.ToString());
            }
            finally
            {
                try
                {
                    if (threadIsAbort)
                    {
                        // ���������� ��� ��������� �����
                        this.StagesQueue[PumpProcessStates.CheckData].StageCurrentState = StageState.FinishedWithErrors;
                        this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                            ReviseDataEventKind.pdeFinishedWithErrors,
                            "�������� ������ ��������� � ��������: �������� �������� �������������.", this.PumpID, -1);
                    }
                    else
                    {
                        if (!finishedWithErrors)
                        {
                            this.StagesQueue[PumpProcessStates.CheckData].StageCurrentState = StageState.SuccefullFinished;
                            this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                                ReviseDataEventKind.pdeSuccefullFinished,
                                "�������� ������ ���������.", this.PumpID, -1);
                        }
                        else
                        {
                            // ���������� ��� ��������� �����
                            this.StagesQueue[PumpProcessStates.CheckData].StageCurrentState = StageState.FinishedWithErrors;
                            this.CheckDataProtocol.WriteEventIntoReviseDataProtocol(
                                ReviseDataEventKind.pdeFinishedWithErrors,
                                "�������� ������ ��������� � ��������.", this.PumpID, -1);
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

        #endregion �������� ������

        #endregion ������ ������ �������


        #region ������� ���������� ��������� �������� ������

        /// <summary>
        /// ����� ��������� � ������ ������� � �������� �������� �������������
        /// </summary>
        private void WriteStartMessageToUsersProtocol()
        {
            if (this.StagesQueue.GetLastExecutedQueueElement() == null)
            {
                this.UsersOperationProtocol.WriteEventIntoUsersOperationProtocol(
                    UsersOperationEventKind.uoeStartWorking_RefUserName,
                    string.Format("�������� {0}", this.PumpRegistryElement.Name));
            }
        }

        /// <summary>
        /// ��������� ACL �� ��������� ������� ��� ���������� ������������.
        /// </summary>
        /// <param name="dir">�������</param>
        /// <param name="account">������������</param>
        /// <param name="rights">����� �������� �������</param>
        /// <param name="controlType">����� �������</param>
        private void AddDirectorySecurity(DirectoryInfo dir, string account, FileSystemRights rights, 
            AccessControlType controlType)
        {
            if (dir == null)
                return;

            // �������� ������ DirectorySecurity, �������������� ������� ��������� ������������
            DirectorySecurity dSecurity = dir.GetAccessControl();

            // ������� ��������� ������ � ������ ��������.
            FileSystemAccessRule fsAccess = new FileSystemAccessRule(account, rights, InheritanceFlags.None,
                PropagationFlags.NoPropagateInherit, controlType);

            bool allOK;
            dSecurity.ModifyAccessRule(AccessControlModification.Set, fsAccess, out allOK);
            if (!allOK)
            {
                throw new ApplicationException("���������� ���������� ������ � " + dir + " ��� " + account);
            }

            // ������������� ������� ������������ �������
            FileSystemAccessRule inheritanceRule = new FileSystemAccessRule(account, rights,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly,
                controlType);
            dSecurity.ModifyAccessRule(AccessControlModification.Add, inheritanceRule, out allOK);
            if (!allOK)
            {
                throw new ApplicationException(
                    "���������� �������� ������� ������������ ������� �� " + dir + " ��� " + account);
            }

            // ������������� ����� ��������� �������
            dir.SetAccessControl(dSecurity);
        }

        /// <summary>
        /// ��������� ������������ ������ ������ � ����� � ���� ���������
        /// </summary>
        /// <param name="userName">��� ������������, ��� �������� ����� ������</param>
        /// <param name="dir">�����</param>
        private void SetDirectoriesRights(string userName, DirectoryInfo dir)
        {
            string account = Environment.MachineName + "\\" + userName;

            AddDirectorySecurity(dir, account, FileSystemRights.FullControl, AccessControlType.Allow);

            DirectoryInfo[] subDirs = dir.GetDirectories("*", SearchOption.AllDirectories);
            foreach (DirectoryInfo subDir in subDirs)
            {
                AddDirectorySecurity(subDir, account, FileSystemRights.FullControl, AccessControlType.Allow);
            }

            // ���� ����� ��������� ����������, �� ������� ������� ������ � ��������� ����� �� ������ � 
            // ���������� ����������
            AddDirectorySecurity(new DirectoryInfo(Environment.SystemDirectory), account,
                FileSystemRights.FullControl, AccessControlType.Allow);
        }

        /// <summary>
        /// ������������� ������� ������ �������
        /// </summary>
        private void InitProperties()
        {               
            // ��������� ������ ������� �������
            this.pumpID = AddPumpHistoryElement(this.ProgramIdentifier,
                string.Format("{0}. {1}", this.PumpRegistryElement.Name, PumpProcessStatesToRusString(this.State)));
            if (this.pumpID < 0)
            {
                throw new Exception("������ ��� ���������� ������ ������� �������");
            }

            // ��������� ������ �������� ���������
            this.RootDir = new DirectoryInfo(this.PumpRegistryElement.DataSourcesUNCPath);
            if (this.RootDir == null)
            {
                throw new Exception("������ ��� ��������� ��������� �������� ���������");
            }

            //SetDirectoriesRights(Environment.UserName, this.RootDir);
            
            // ���� �� ������ ������� ���������, �� ����� �� ���� � ���
            if (!this.RootDir.Exists)
            {
                throw new Exception(
                    string.Format("������� � ������� {0} �� ������", this.RootDir.FullName));
            }

            // ����� �� ���������� ����� � �����
            this.UseArchive = Convert.ToBoolean(GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "ucbMoveFilesToArchive", "false"));

            // ������� ���������� ����� ������ �� ���� �� ���������
            this.DeleteEarlierData = Convert.ToBoolean(GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "ucbDeleteEarlierData", "false"));

            // ������� ������� �������������� ��������
            this.FinalOverturn = IsFinalOverturn();
        }

        /// <summary>
        /// ��������� ���������������� �������� ����� �������������� ������
        /// </summary>
        /// <returns>���������</returns>
        private void PreparePreviewData()
        {
            //WriteStartMessageToUsersProtocol();

            InitProperties();
        }

        /// <summary>
        /// ��������� ���������������� �������� ����� ��������
        /// </summary>
        /// <returns>���������</returns>
        private void PreparePumpData()
        {
            //WriteStartMessageToUsersProtocol();

            if (this.StagesQueue[PumpProcessStates.PreviewData].IsExecuted) return;

            InitProperties();
        }

        /// <summary>
        /// ��������� ���������������� �������� ����� ���������� ������
        /// </summary>
        /// <returns>���������</returns>
        private void PrepareProcessData()
        {
            //WriteStartMessageToUsersProtocol();

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted) return;

            InitProperties();
        }

        /// <summary>
        /// ��������� ���������������� �������� ����� �������������� ������
        /// </summary>
        /// <returns>���������</returns>
        private void PrepareAssociateData()
        {
            //WriteStartMessageToUsersProtocol();

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessData].IsExecuted) return;

            InitProperties();
        }

        /// <summary>
        /// ��������� ���������������� �������� ����� �������� �����
        /// </summary>
        /// <returns>���������</returns>
        private void PrepareProcessCube()
        {
            //WriteStartMessageToUsersProtocol();

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.AssociateData].IsExecuted) return;

            InitProperties();
        }

        /// <summary>
        /// ��������� ���������������� �������� ����� ��������� ������
        /// </summary>
        /// <returns>���������</returns>
        private void PrepareCheckData()
        {
            //WriteStartMessageToUsersProtocol();

            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.AssociateData].IsExecuted ||
                this.StagesQueue[PumpProcessStates.ProcessCube].IsExecuted) return;

            InitProperties();
        }

        #endregion ������� ���������� ��������� �������� ������


        #region ������� ���������� ����������� �������� ������

        /// <summary>
        /// ���������� ��������� �� ��������� ������� � �������� �������� �������������
        /// </summary>
        private void WriteStopMessageToUsersProtocol()
        {
            try
            {
                if (this.StagesQueue.GetNextExecutableQueueElement() == null)
                {
                    this.UsersOperationProtocol.WriteEventIntoUsersOperationProtocol(
                        UsersOperationEventKind.uoeStartWorking_RefUserName,
                        string.Format("{0} ���������.", this.PumpRegistryElement.Name));
                }
            }
            catch (Exception ex)
            {
                WriteToTrace("������ ��� ������ � �������� �������� ������������: " + ex.ToString(), TraceMessageKind.Error);
            }
        }

        /// <summary>
        /// ������� ������ "����������� ������" �� ���� ��������������� this.UsedClassifiers, � �������
        /// ��� ������ �� ���������� ����������.
        /// </summary>
        private void DeleteUnusedClsFixedRows()
        {
            WriteToTrace("����� �������� ����� \"����������� ������\" �� ������ ��������������.", TraceMessageKind.Information);

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

            WriteToTrace("�������� ����� \"����������� ������\" �� ������ �������������� ���������.", TraceMessageKind.Information);
        }

        #endregion ������� ���������� ����������� �������� ������


        #region ������� ��������� �������� ���������������

        /// <summary>
        /// ������������� �������� ���������������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="message">��������� ���������</param>
        /// <param name="cls">������ IClassifier</param>
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
                        string.Format("��������� �������� {0} ({1})...", semantic, message),
                        string.Format("������������� {0} �� {1}", i + 1, lng), true);
                    WriteToTrace(string.Format("��������� �������� {0}...", semantic), TraceMessageKind.Information);

                    DataSet ds = null;
                    cls[i].DivideAndFormHierarchy(this.SourceID, this.PumpID, ref ds);

                    WriteToTrace(string.Format("�������� {0} �����������.", semantic), TraceMessageKind.Information);
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeError, string.Format(
                            "��������� �������� �������������� {0} �� ������ ��������� {1}, ID {2} ��������� � ��������.",
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
            // ����� �������� �������� �������������, ����� ����� ������ � ������������ �������������� ��� �� ����� ����������� �������
            // � ������, ���� �������� ������� ���������� ����������
            if (this.VersionClassifiers != null)
                foreach (IClassifier cls in this.VersionClassifiers)
                    ClearPresentationContext(cls);
        }

        /// <summary>
        /// ��������� �������� ��������������� ����� ������� ������. 
        /// ����� ���������������� � �������� ��� ���������� �������������� �������� �� ��������� ��������.
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
                        "����� ��������� �������� �� ������ ��������� {0} (ID {1}).",
                        GetSourcePathBySourceID(dataSource.Key), dataSource.Key));

                    // ������������� �������������
                    SetPresentationContexts();
                    try
                    {
                        SetClsStandardHierarchy(
                            string.Format("�������� {0} �� {1}", i, this.PumpedSources.Count), this.HierarchyClassifiers);
                    }
                    finally
                    {
                        // ������� �������������
                        ClearPresentationContexts();
                    }

                    WriteEventIntoDataPumpProtocol(
                         DataPumpEventKind.dpeInformation, string.Format(
                         "��������� �������� �� ������ ��������� {0}, ID {1} ������� ���������.",
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
                            "��������� �������� �� ������ ��������� {0}, ID {1} ��������� � ��������.",
                            GetSourcePathBySourceID(dataSource.Key), dataSource.Key), ex);
                }
            }
        }

        /// <summary>
        /// ��������� �������� ��������������� ����� ������� ������. 
        /// ��������� ���������������� � ����������� ��������. ���������� �� ����� �������.
        /// </summary>
        private void ClsHierarchySetting()
        {
            bool success = true;
            bool aborted = false;

            try
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "����� ��������� ��������.");

                // ��������� ��������
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
                        "��������� �������� ��������� � ��������: �������� �������� �������������.");
                }
                else
                {
                    if (success)
                    {
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeSuccefullFinished, "��������� �������� ���������.");
                    }
                    else
                    {
                        WriteEventIntoDataPumpProtocol(
                            DataPumpEventKind.dpeFinishedWithErrors, "��������� �������� ��������� � ��������.");
                    }
                }
            }
        }

        #endregion ������� ��������� �������� ���������������


        #region ������� ������������� ������

        /// <summary>
        /// ������������ �������������
        /// </summary>
        /// <param name="cls">������ ��������������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="allowDigits">��������� �������� �������</param>
        /// <param name="reAssociate">true - ������������� ������������ ��� ������; false - ������������ ������ ����������������</param>
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
                        BridgeOperationsEventKind.boeWarning, "��� ������", "��� ������", string.Format(
                        "������������� {0} ({1}) ��������� � ��������: �� ������� ������� ������ ������������� ��������������.", ass.FullCaption, assObjectKey),
                        this.PumpID, sourceID);
                }

                try
                {
                    ((IBridgeAssociation)ass).Associate(sourceID, Convert.ToInt32(bridgeClsSourceID), this.PumpID, allowDigits, reAssociate);
                }
                catch (ThreadAbortException)
                {
                    this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                        BridgeOperationsEventKind.boeFinishedWithError, "��� ������", "��� ������", string.Format(
                        "������������� {0} ��������� � ��������: �������� �������� �������������.", ass.FullCaption),
                        this.PumpID, sourceID);
                    WriteToTrace(string.Format(
                        "������������� {0} ��������� � ��������: �������� �������� �������������.", ass.FullCaption), 
                        TraceMessageKind.Error);
                    throw;
                }
                catch (Exception ex)
                {
                    WriteToTrace(string.Format(
                        "������������� {0} ��������� � ��������: {1}",
                        ass.FullCaption, ex.ToString()), TraceMessageKind.Error);
                }
            }
        }

        /// <summary>
        /// ������������ ��������������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        protected void DoBridgeCls(int sourceID, string message, IClassifier[] cls)
        {
            DoBridgeCls(sourceID, message, cls, false, false);
        }

        /// <summary>
        /// ������������ ��������������
        /// </summary>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="message"></param>
        /// <param name="cls"></param>
        /// <param name="allowDigits">��������� �������� �������</param>
        /// <param name="reAssociate">true - ������������� ������������ ��� ������; false - ������������ ������ ����������������</param>
        protected void DoBridgeCls(int sourceID, string message, IClassifier[] cls, bool allowDigits, bool reAssociate)
        {
            string msg = string.Format(
                "����� ������������� ������ ��������� {0} (ID {1}).",
                GetSourcePathBySourceID(sourceID), sourceID);
            this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                BridgeOperationsEventKind.dpeStartDataSourceProcessing, "��� ������", "��� ������", msg, this.PumpID, sourceID);
            WriteToTrace(msg, TraceMessageKind.Information);

            // ������������� ������              

            int lng = cls.GetLength(0);
            for (int i = 0; i < lng; i++)
            {
                if (cls[i] == null) continue;

                string semantic = cls[i].FullCaption;

                SetProgress(lng, i + 1,
                    string.Format("������������� ������ {0} ({1})...", semantic, message),
                    string.Format("������������� {0} �� {1}", i + 1, lng), true);
                WriteToTrace(string.Format("������������� ������ {0}...", semantic), TraceMessageKind.Information);
                if (!cls[i].Attributes.ContainsKey("SourceID"))
                    sourceID = -1;

                AssociateCls(cls[i], sourceID, allowDigits, reAssociate);
                WriteToTrace(string.Format("������ {0} ������������.", semantic), TraceMessageKind.Information);
            }

            msg = string.Format(
                "������������� ������ ��������� {0} (ID {1}) ���������.",
                GetSourcePathBySourceID(sourceID), sourceID);
            this.AssociateDataProtocol.WriteEventIntoBridgeOperationsProtocol(
                BridgeOperationsEventKind.dpeSuccessfullFinishDataSourceProcess, "��� ������", "��� ������", msg, 
                this.PumpID, sourceID);
            WriteToTrace(msg, TraceMessageKind.Information);
        }

        #endregion ������� ������������� ������


        #region ������� ������� �����

        /// <summary>
        /// ������ �����.
        /// </summary>
        /// <param name="fct">������ ������ ������.</param>
        /// <param name="cls">������ ���������������.</param>
        /// <param name="batchGuid">������������� ������.</param>
        private bool ProcessCubes(IFactTable[] fct, IClassifier[] cls, Guid batchGuid)
        {
            // ��������� ����� ���������
            for (int i = 0; i < cls.GetLength(0); i++)
            {
                if (cls[i] != null)
                {
                    WriteToTrace(String.Format("�������� ��������� \"{0}\" �� ������...", cls[i].OlapName), TraceMessageKind.Information);
                    
                    scheme.Processor.InvalidateDimension(
                        cls[i], 
                        ProgramIdentifier,
                        InvalidateReason.ClassifierChanged, 
                        cls[i].OlapName,
                        this.PumpID, this.SourceID,
                        batchGuid);

                    WriteToTrace(String.Format("��������� \"{0}\" ������� ���������� �� ������", cls[i].OlapName), TraceMessageKind.Information);
                }
            }

            for (int i = 0; i < fct.GetLength(0); i++)
            {
                if (fct[i] != null)
                {
                    WriteToTrace(String.Format("�������� ������ ���� \"{0}\" �� ������...", fct[i].OlapName), TraceMessageKind.Information);

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

                        WriteToTrace(String.Format("������ ���� \"{0}\" ������� ���������� �� ������.", fct[i].OlapName), TraceMessageKind.Information);
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

                            WriteToTrace(String.Format("������ ���� \"{0}\\{1}\" ������� ���������� �� ������.", fct[i].OlapName, partitionId), TraceMessageKind.Information);
                        }
                    }
                }
            }

            for (int i = 0; i < dimensionsForProcess.GetLength(0); i += 2)
            {
                WriteToTrace(String.Format("�������� ��������� \"{0}\" �� ������...", dimensionsForProcess[i + 1]), TraceMessageKind.Information);
                scheme.Processor.InvalidateDimension(
                    dimensionsForProcess[i],
                    ProgramIdentifier,
                    InvalidateReason.ClassifierChanged,
                    dimensionsForProcess[i + 1],
                    batchGuid);
                WriteToTrace(String.Format("��������� \"{0}\" ������� ���������� �� ������", dimensionsForProcess[i + 1]), TraceMessageKind.Information);
            }

            for (int i = 0; i < cubesForProcess.GetLength(0); i += 2)
            {
                WriteToTrace(String.Format("�������� ���� \"{0}\" �� ������...", cubesForProcess[i + 1]), TraceMessageKind.Information);
                scheme.Processor.InvalidatePartition(
                    cubesForProcess[i],
                    ProgramIdentifier,
                    InvalidateReason.DataPump,
                    cubesForProcess[i + 1],
                    batchGuid);
                WriteToTrace(String.Format("��� \"{0}\" ������� ��������� �� ������", cubesForProcess[i + 1]), TraceMessageKind.Information);
            }

            return true;
        }

        #endregion ������� ������� �����


        #region ������� �������� ������

        /// <summary>
        /// ��������� ������ ����������� ������� �� ������������� ����������
        /// </summary>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="constr">�������������� �����������</param>
        /// <returns>������ �����������</returns>
        private string MakeConstraintString(int pumpID, int sourceID, string constr)
        {
            string result = string.Empty;

            if (pumpID >= 0) result = string.Format(" and PUMPID = {0}", pumpID);
            if (sourceID >= 0) result += string.Format(" and SOURCEID = {0}", sourceID);
            if (constr != string.Empty) result += string.Format(" and {0}", constr);

            return result;
        }

        /// <summary>
        /// ���������� ���������� ������� ���������� �������������� �� ���������� ���������
        /// </summary>
        /// <param name="db">������ ��</</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="constr">����������� �������</param>
        /// <returns>���������� �������</returns>
        protected int GetClsRecordsAmount(IClassifier cls, int pumpID, int sourceID, string constr)
        {
            if (cls == null) return 0;

            // ������������ ������ �������
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
        /// ���������, ���� �� ������ � ���� � ��������� ��������������� �� ���������� ���������
        /// </summary>
        /// <param name="db">������ ��</</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="constr">����������� �������</param>
        /// <returns>���� ������ � ��������� ��������������� �� ������� ��������� ��� ���</returns>
        protected bool CheckClsRecordsAmount(IClassifier[] cls, int pumpID, int sourceID, 
            string constr)
        {
            WriteToTrace("�������� ������� � ���� ������ ���������������.", TraceMessageKind.Information);
            SetProgress(-1, -1, "�������� ������� � ���� ������ ���������������...", string.Empty, true);

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
        /// ���������� ���������� ������� ��������� ������� ������ �� ���������� ���������
        /// </summary>
        /// <param name="db">������ ��</</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="constr">����������� �������</param>
        /// <returns>���������� �������</returns>
        protected int GetFactRecordsAmount(IFactTable fct, int pumpID, int sourceID, string constr)
        {
            if (fct == null) return 0;

            // ������������ ������ �������
            string whereStr = MakeConstraintString(pumpID, sourceID, constr);

            return Convert.ToInt32(this.DB.ExecQuery(
                string.Format("select count(id) from {0} where ID >= 0 {1}",
                    fct.FullDBName, whereStr),
                QueryResultTypes.Scalar));
        }

        /// <summary>
        /// ���������, ���� �� ������ � ���� � ��������� �������� ������ �� ���������� ���������
        /// </summary>
        /// <param name="db">������ ��</</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="constr">����������� �������</param>
        /// <returns>���� ������ � ��������� ��������������� �� ������� ��������� ��� ���</returns>
        protected bool CheckFactRecordsAmount(IFactTable[] fct, int pumpID, int sourceID, string constr)
        {
            WriteToTrace("�������� ������� � ���� ������ ������.", TraceMessageKind.Information);
            SetProgress(-1, -1, "�������� ������� � ���� ������ ������...", string.Empty, true);

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

            WriteToTrace("������ ������ �����������.", TraceMessageKind.Information);
            SetProgress(-1, -1, "������ ������ �����������.", string.Empty, true);

            return false;
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        /// <param name="db">������ ��</param>
        /// <param name="obj">������ ��</param>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="constr">������ �����������</param>
		protected void DeleteTableData(IEntity obj, int pumpID, int sourceID, string constr)
        {
            string semantic = obj.FullCaption;

            // ������������ ������ �������
            string whereStr = MakeConstraintString(pumpID, sourceID, constr);
            if (whereStr != string.Empty)
            {
                whereStr = "where " + whereStr.Remove(0, 4);
            }

            WriteToTrace(string.Format("�������� ������ {0}...", semantic), TraceMessageKind.Information);
            SetProgress(-1, -1, string.Format(
                "�������� {0}. �������� ������ {1}...", GetShortSourcePathBySourceID(sourceID), semantic),
                string.Empty, true);
            
            string queryStr = string.Format("delete from {0} {1}", obj.FullDBName, whereStr);
            int recCount = (int)this.DB.ExecQuery(queryStr, QueryResultTypes.NonQuery);

            WriteEventIntoDeleteDataProtocol(
                DeleteDataEventKind.ddeInformation,
                string.Format("������� ������ {0} ({1} �����).", semantic, recCount), true);
        }

        /// <summary>
        /// ������� ������ �� ������� pumpID, sourceID
        /// </summary>
        /// <param name="obj">������ ��</param>
        /// <param name="constr">������ �����������</param>
		protected void DeleteTableData(IEntity obj, string constr)
        {
            DeleteTableData(obj, this.PumpID, this.SourceID, constr);
        }

        /// <summary>
        /// ������� ������ ������ ������
        /// </summary>
        /// <param name="fct">������ ������ ������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="pumpID">�� �������</param>
        /// <param name="constr">�����������</param>
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
        /// ������� ������ ������ ���������������
        /// </summary>
        /// <param name="cls">������ ������ ���������������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="pumpID">�� �������</param>
        /// <param name="constr">�����������</param>
        protected void DirectDeleteClsData(IClassifier[] cls, int pumpID, int sourceID, string constr)
        {
            for (int i = 0; i < cls.GetLength(0); i++)
            {
                if (cls[i] == null)
                    continue;
                // ������ �� �� �������� �������� ��������� ������ (���� ������) 
                // ORA-00600: ��� �����. ������, ���������: [13001], [], [], [], [], [], [], [].
                // ��� ������� �������� ���� ���������
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
        /// ������� ������ ����������
        /// </summary>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
        protected void DirectDeleteProtocolData(int pumpID, int sourceID)
        {
            SetProgress(0, 0, "�������� ������ ����������...", string.Empty, true);
            WriteToTrace("�������� ������ ����������...", TraceMessageKind.Information);

            this.DataPumpProtocol.DeleteProtocolData(ModulesTypes.PreviewDataModule, sourceID, pumpID);
            this.DataPumpProtocol.DeleteProtocolData(ModulesTypes.DataPumpModule, sourceID, pumpID);
            this.ProcessDataProtocol.DeleteProtocolData(ModulesTypes.ProcessDataModule, sourceID, pumpID);
            this.AssociateDataProtocol.DeleteProtocolData(ModulesTypes.BridgeOperationsModule, sourceID, pumpID);
            //this.ProcessCubeProtocol.DeleteProtocolData(ModulesTypes.MDProcessingModule, sourceID, pumpID);
            //������ ��� ������� 86�
            if (!ProgramIdentifier.StartsWith("BusGovRuPump."))
                this.ProcessCubeProtocol.DeleteProtocolData(ModulesTypes.MDProcessingModule, sourceID, pumpID);
            this.CheckDataProtocol.DeleteProtocolData(ModulesTypes.ReviseDataModule, sourceID, pumpID);
            this.DeleteDataProtocol.DeleteProtocolData(ModulesTypes.DeleteDataModule, sourceID, pumpID);

            SetProgress(0, 0, "������ ���������� �������.", string.Empty, true);
            WriteToTrace("������ ���������� �������.", TraceMessageKind.Information);
        }

        /// <summary>
        /// ������� ������ ������� ������� �� �� �������
        /// </summary>
        /// <param name="pumpID">�� ������� (-1 - ������� ���)</param>
        protected void ClearPumpHistory(int pumpID)
        {
            WriteToTrace("�������� ������ �����...", TraceMessageKind.Information);

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

            WriteToTrace("������ ����� �������.", TraceMessageKind.Information);
        }

        /// <summary>
        /// ������� ��������� ������ �� ������� ������������ ������� ���������� ����������
        /// </summary>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
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
        /// ������� ������ ������� �� �� ������� �/��� ��������� (-1 - ������������)
        /// </summary>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
        /// <param name="constr">����������� �������</param>
        protected virtual void DirectDeleteData(int pumpID, int sourceID, string constr)
        {
            // ���������� ��������
            DirectDeleteFactData(this.UsedFacts, pumpID, sourceID, constr);
            DirectDeleteClsData(this.UsedClassifiers, pumpID, sourceID, constr);

            ClearDataSources2PumpHistory(pumpID, sourceID);
        }

        /// <summary>
        /// ������� ������ ������� �� �� ������� �/��� ��������� (-1 - ������������)
        /// </summary>
        /// <param name="db">������ ��</param>
        /// <param name="pumpID">�� �������</param>
        /// <param name="sourceID">�� ���������</param>
        protected virtual void DirectDeleteData(int pumpID, int sourceID)
        {
            try
            {
                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeStart, pumpID, sourceID,
                    "����� �������� ������.", null);

                // ���������� ��������
                DirectDeleteFactData(this.UsedFacts, pumpID, sourceID, string.Empty);
                DirectDeleteClsData(this.UsedClassifiers, pumpID, sourceID, string.Empty);

                ClearDataSources2PumpHistory(pumpID, sourceID);

                SetProgress(-1, -1, "������ �������.", string.Empty, true);

                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeSuccefullFinished, pumpID, sourceID, 
                    "�������� ������ ������� ���������.", null);
            }
            catch (ThreadAbortException)
            {
                WriteEventIntoDeleteDataProtocol(
                    DeleteDataEventKind.ddeFinishedWithErrors, pumpID, sourceID, "�������� �������� �������������.", null);
                throw;
            }
            catch (Exception ex)
            {
                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeFinishedWithErrors, pumpID, sourceID,
                    "�������� ������ ��������� � ��������", ex);
                throw;
            }
        }

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected virtual void DeletePumpedData()
        {
            if (CheckClsRecordsAmount(this.UsedClassifiers, -1, this.SourceID, string.Empty) ||
                CheckFactRecordsAmount(this.UsedFacts, -1, this.SourceID, string.Empty))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, 
                    "������ �� �������� ��������� ��� ���� ��������. ���������� ������ ����� �������.");

                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.dpeStartDataSourceProcessing,
                    string.Format("����� �������� ������ ��������� ID {0}.", this.SourceID), true);

                DirectDeleteData(-1, this.SourceID, string.Empty);

                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.dpeSuccessfullFinishDataSourceProcess,
                    string.Format("�������� ������ ��������� ID {0} ������� ���������.", this.SourceID));

                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "���������� ������ �������.", true);
            }
        }

        /// <summary>
        /// ������� ����� ��������� ������
        /// </summary>
        protected virtual void DeleteEarlierPumpedData()
        {
            if (this.DeleteEarlierData)
            {
                DeletePumpedData();
            }
        }

        /// <summary>
        /// ������� ������ ������ � ���������������
        /// </summary>
        /// <param name="constr">����������� �� �������� ������</param>
        /// <param name="comment">��������� � �������� �������� (������������ � ��������� � ���)</param>
        /// <param name="deleteCls">������� � ��������������</param>
        protected void DeleteData(string constr, string comment, bool deleteCls)
        {
            try
            {
                if (CheckFactRecordsAmount(this.UsedFacts, -1, this.SourceID, constr))
                {
                    WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeStart,
                        string.Format("����� �������� ������. {0}", comment), true);

                    DirectDeleteFactData(this.UsedFacts, -1, this.SourceID, constr);

                    if (deleteCls)
                    {
                        DirectDeleteClsData(this.UsedClassifiers, -1, this.SourceID, constr);
                    }

                    SetProgress(-1, -1, "������ �������", string.Empty, true);

                    WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeSuccefullFinished,
                        "�������� ������ ������� ���������.", true);
                }
            }
            catch (ThreadAbortException)
            {
                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeFinishedWithErrors, "�������� �������� �������������.");
                throw;
            }
            catch (Exception ex)
            {
                WriteEventIntoDeleteDataProtocol(DeleteDataEventKind.ddeFinishedWithErrors, "�������� ������ ��������� � ��������", ex);
                throw;
            }
        }

        /// <summary>
        /// ������� ������ ������
        /// </summary>
        /// <param name="constr">����������� �� �������� ������</param>
        /// <param name="comment">��������� � �������� �������� (������������ � ��������� � ���)</param>
        protected void DeleteData(string constr, string comment)
        {
            DeleteData(constr, comment, false);
        }

        /// <summary>
        /// ������� ������ ������
        /// </summary>
        /// <param name="constr">����������� �� �������� ������</param>
        protected void DeleteData(string constr)
        {
            DeleteData(constr, string.Empty);
        }

        #endregion ������� �������� ������
    }
}