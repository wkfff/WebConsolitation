using System;
using System.Collections.Generic;
using System.Data;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TemplatesUI.Gui
{
    public class SelectRowsClsUI : DataClsUI
    {
        private readonly HashSet<int> codes;
        private readonly string columnName;

        internal SelectRowsClsUI(IEntity entity, HashSet<int> codes, string columnName)
            : base(entity)
        {
            this.codes = codes;
            this.columnName = columnName;
        }

        public HashSet<int> Codes
        {
            get { return codes; }
        }

        public override void InitializeUI()
        {
            base.InitializeUI();

            vo.ugeCls.OnCellChange += UgeClsOnOnCellChange;
        }

        private void UgeClsOnOnCellChange(object sender, CellEventArgs cellEventArgs)
        {
            if (cellEventArgs.Cell.Column.Key == "SelectedRow")
            {
                if (cellEventArgs.Cell.Text == "True")
                {
                    codes.Add(Convert.ToInt32(cellEventArgs.Cell.Row.Cells[columnName].Value));
                }
                else
                {
                    codes.Remove(Convert.ToInt32(cellEventArgs.Cell.Row.Cells[columnName].Value));
                }
            }
        }

        public override void UpdateToolbar()
        {
            vo.utbToolbarManager.Visible = false;
            vo.ugeCls.utmMain.Visible = false;
        }

        protected override void OnNeedLoadChildRows(object sender, int parentID)
        {
            IDataUpdater upd = null;
            try
            {
                string filterStr;
                upd = GetActiveUpdater(parentID, out filterStr);
                DataTable childRows = new DataTable();
               
                DataTable allRows = dsObjData.Tables[0];
                upd.Fill(ref childRows);

                LookupManager.Instance.InitLookupsCash(ActiveDataObj, dsObjData);
                allRows.BeginLoadData();
                foreach (DataRow row in childRows.Rows)
                {
                    DataRow addedRow = allRows.Rows.Add(row.ItemArray);
                    addedRow["SelectedRow"] = codes.Contains(Convert.ToInt32(row[columnName]));
                    addedRow.AcceptChanges();
                }

                allRows.EndLoadData();
            }
            finally
            {
                if (upd != null)
                    upd.Dispose();
            }
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);

            DataColumn selectedRowColumn = new DataColumn();
            selectedRowColumn.ColumnName = "SelectedRow";
            selectedRowColumn.DefaultValue = false;
            selectedRowColumn.DataType = typeof(bool);
            dsObjData.Tables[0].Columns.Add(selectedRowColumn);

            foreach (DataRow row in dsObjData.Tables[0].Rows)
            {
                row["SelectedRow"] = codes.Contains(Convert.ToInt32(row[columnName]));
            }

            vo.ugeCls.DataSource = dsObjData;

            vo.ugeCls.IsReadOnly = false;
            vo.ugeCls.AllowEditRows = true;
            vo.ugeCls.ugData.DisplayLayout.AddNewBox.Hidden = true;

            foreach (UltraGridBand band in vo.ugeCls.ugData.DisplayLayout.Bands)
            {
                foreach (var column in band.Columns)
                {
                    column.CellActivation = Activation.NoEdit;
                }

                band.Columns["SelectedRow"].Hidden = false;
                band.Columns["SelectedRow"].Header.VisiblePosition = 0;
                band.Columns["SelectedRow"].CellActivation = Activation.AllowEdit;
                band.Columns["SelectedRow"].Header.Caption = string.Empty;
            }
        }
    }
}
