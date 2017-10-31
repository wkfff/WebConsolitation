using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Microsoft.AnalysisServices;

using Krista.FM.Server.ProcessorLibrary;

namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    public partial class AccumulatorViewer : UserControl
    {   
        private int batchId = -1;
        private IAccumulatorManager accumulatorManager;
        private IProcessManager processManager;
        
        private DataSetOlapBaseUtils dataSetOlapBaseUtils;

        private BindingSource biSourceAccumulator = new BindingSource();
        private BindingSource biSourceBatch = new BindingSource();
        private BindingSource biSourceBatch_Accumulator = new BindingSource();

        public AccumulatorViewer()
        {
            InitializeComponent();
            gridAccumulator.RowTemplate.DefaultCellStyle.BackColor = Color.Transparent;
            gridAccumulator.AutoGenerateColumns = false;
        }

        public void Connect(IAccumulatorManager accumulatorManager, IProcessManager processManager, DataSetOlapBaseUtils dataSetOlapBaseUtils)
        {
            this.accumulatorManager = accumulatorManager; 
            this.processManager = processManager;
            this.dataSetOlapBaseUtils = dataSetOlapBaseUtils;
            if (null != this.accumulatorManager && null != this.processManager && null != this.dataSetOlapBaseUtils)
            {                
                InitBindingSources();
                InitGrids();
            }            
        }

        private void InitBindingSources()
        {
            biSourceAccumulator.SuspendBinding();
            try
            {
                biSourceAccumulator.Sort = string.Empty;
                biSourceAccumulator.DataSource = null;
                biSourceAccumulator.DataSource = accumulatorManager.DSAccumulator;
                biSourceAccumulator.DataMember = "Accumulator";
                biSourceAccumulator.Sort = "RECORDSTATUS, OBJECTTYPE, ADDITIONTIME DESC, PROCESSTYPE";
            }
            finally
            {
                biSourceAccumulator.ResumeBinding();
            }
            biSourceBatch.SuspendBinding();
            try
            {                
                biSourceBatch.DataSource = null;
                biSourceBatch.DataSource = accumulatorManager.DSAccumulator;
                biSourceBatch.DataMember = "Batch";
                biSourceBatch_Accumulator.DataSource = null;
                biSourceBatch_Accumulator.DataSource = biSourceBatch;
                biSourceBatch_Accumulator.DataMember = "Batch_Accumulator";
            }
            finally
            {
                biSourceBatch.ResumeBinding();
            }            
        }

        private void RefreshGrids()
        {
            InitBindingSources();
        }

        private void InitGrids()
        {            
            gridAccumulator.DataSource = biSourceAccumulator;            
            batchDetailViewer.Connect(biSourceBatch_Accumulator, false);
        }        

        private bool BatchCreated()
        {
            return batchId != -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<int> GetWaitingRowsKeys(out Dictionary<int, string> partitions, out Dictionary<int, string> dimensions)
        {            
            partitions = new Dictionary<int, string>();
            dimensions = new Dictionary<int, string>();            
            for (int i = 0; i < gridAccumulator.SelectedRows.Count; i++)
            {
                //Добавляем только записи в состоянии ожидания.
                if (!gridAccumulator.SelectedRows[i].IsNewRow &&
                    (RecordStatus)gridAccumulator.SelectedRows[i].Cells[columnRecordStatus.Index].Value == RecordStatus.waiting)
                {                    
                    if ((OlapObjectType)gridAccumulator.SelectedRows[i].Cells[columnObjectType.Index].Value == OlapObjectType.partition)
                    {
                        partitions.Add(
                            (int)gridAccumulator.SelectedRows[i].Cells[columnId.Index].Value,
                            (string)gridAccumulator.SelectedRows[i].Cells[columnObjectId.Index].Value);
                    }
                    else
                    {
                        dimensions.Add(
                            (int)gridAccumulator.SelectedRows[i].Cells[columnId.Index].Value,
                            (string)gridAccumulator.SelectedRows[i].Cells[columnObjectId.Index].Value);
                    }
                }
            }
            List<int> allWaitingKeys = new List<int>(partitions.Keys);
            allWaitingKeys.AddRange(dimensions.Keys);
            return allWaitingKeys;
        }

        private void AnalyseWaitingRows(IEnumerable<string> waitingRowsIds)
        {
            foreach (string objectId in waitingRowsIds)
            {
                List<DataRow> dimensions = dataSetOlapBaseUtils.GetCubeDimensions(objectId);
                //foreach (DataRow dimension in dimensions)
                //{
                //    dimension.
                //}
            }
        }

        private void stripBtnAddToBatch_Click(object sender, EventArgs e)
        {
            Dictionary<int, string> partitions; 
            Dictionary<int, string> dimensions;
            IEnumerable<int> allWaitingKeys = GetWaitingRowsKeys(out partitions, out dimensions);
            if (partitions.Count > 0 || dimensions.Count > 0)
            {
                if (!BatchCreated())
                {
                    batchId = accumulatorManager.CreateBatch(allWaitingKeys);
                    RefreshGrids();
                    biSourceBatch.Position = accumulatorManager.GetBatchIndex(batchId);
                    batchDetailViewer.Connect();
                }
                else
                {
                    accumulatorManager.AddBatchDetail(batchId, allWaitingKeys);
                    RefreshGrids();
                }               
            }
        }

        private void stripBtnResetBatch_Click(object sender, EventArgs e)
        {
            DeleteBatch();
        }

        private void DeleteBatch()
        {   
            batchDetailViewer.Disconnect();            
            accumulatorManager.DeleteBatch(batchId);
            batchId = -1;
        }

        private void stripBtnProcessBatch_Click(object sender, EventArgs e)
        {
            //if (BatchCreated())
            //{
            //    IAccumulatorRow[] nonWaitingRows = batchRow.GetNonWaitingAccumulatorRows();
            //    if (nonWaitingRows.Length > 0)
            //    {
            //        MessageBox.Show("Пакет содержит записи, удаленные оптимизацией");
            //    }
            //    else
            //    {
            //        processManager.StartBatch(batchId);
            //        batchDetailViewer.Disconnect();
            //        batchId = -1;                
            //    }
            //}
            if (BatchCreated())
            {
                processManager.StartBatch(batchId);
                batchDetailViewer.Disconnect();
                batchId = -1;                
            }            
        }

        public bool Connected
        {
            get { return accumulatorManager != null; }
        }        

        private void stripBtnRefresh_Click(object sender, EventArgs e)
        {
            accumulatorManager.RefreshTableAccumulator();
            RefreshGrids();
        }        

        private void gridAccumulator_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            ViewerUtils.grid_RowPrePaint(gridAccumulator, e,
                ViewerUtils.GetRecordStatusBackBrush(
                ViewerUtils.GetRecordStatus(gridAccumulator, e.RowIndex, columnRecordStatus.Index)));
        }

        /// <summary>
        /// Генерирует строку фильтра в зависимости от состояния фильтровых кнопок.
        /// </summary>
        /// <returns>Строка фильтра</returns>
        private string GetFilter()
        {
            string filter = string.Empty;
            if (stripBtnShowWaiting.Checked)
            {
                if (stripBtnShowErrors.Checked)
                {
                    filter = string.Format("RecordStatus = '{0}' or RecordStatus = '{1}' or RecordStatus = '{2}'",
                        (int)RecordStatus.waiting, (int)RecordStatus.inProcess, (int)RecordStatus.complitedWithErrors);
                }
                else
                {
                    filter = string.Format("RecordStatus = '{0}' or RecordStatus = '{1}'",
                        (int)RecordStatus.waiting, (int)RecordStatus.inProcess);
                }
            }
            else
            {
                if (stripBtnShowErrors.Checked)
                {
                    filter = string.Format("RecordStatus = '{0}'", (int)RecordStatus.complitedWithErrors);
                }
                else
                {
                    filter = string.Empty;
                }
            }
            return filter;
        }

        private void stripBtnShowActual_CheckStateChanged(object sender, EventArgs e)
        {
            biSourceAccumulator.Filter = GetFilter();
        }
    }    
}