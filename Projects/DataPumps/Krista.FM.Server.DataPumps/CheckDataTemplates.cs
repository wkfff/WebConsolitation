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
    // ������ �������� �������� ����������� ������� ����� ���������

    /// <summary>
    /// ������� ����� ��� ���� �������.
    /// </summary>
    public abstract partial class DataPumpModuleBase : DisposableObject
    {

        #region ����� ������� �������� ���������� ������

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

        #endregion ����� ������� �������� ���������� ������

        protected void CheckDataSourcesTemplate(int year, int month, string message)
        {
            string checkParamsDescription = GetProcessParamsDescription(year, month);
            // ����� � �������� � ������ ���������
            CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeStart,
                string.Format("{0} {1} - ����� ��������.", message, checkParamsDescription), this.PumpID, this.SourceID);

            // ���� ��� �� ������ ���� �������, �� ������������ ��� ���������� �� ������� ����� ������
            // ����� ��������� ���-��������� � ������������ ���.
            Dictionary<int, string> dataSources = null;
            if (!this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
            {
                dataSources = GetAllPumpedDataSources();
            }
            else
            {
                // � ���� �� ������ ��� ���������?
                if (this.PumpID < 0 || this.PumpedSources.Count == 0)
                {
                    CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeCriticalError,
                        string.Format("{0} - ��� ������.", message), this.PumpID, this.SourceID);
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
                    string.Format("{0} - ����� �������� �� ������ ��������� ID {1}.", message, this.SourceID), this.PumpID, this.SourceID);

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
                        string.Format("{0} �� ������ ��������� ID {1} ������� ���������.", message, this.SourceID), this.PumpID, this.SourceID);
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
                        string.Format("{0} �� ������ ��������� ID {1} ��������� � ��������: {2}", message, this.SourceID, ex), this.PumpID, this.SourceID);
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
                string.Format("{0} {1} ���������.", message, checkParamsDescription), this.PumpID, this.SourceID);
        }

        protected void CheckDataSourcesTemplate(string message)
        {
            CheckDataSourcesTemplate(0, 0, message);
        }
    }
}
