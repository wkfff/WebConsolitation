using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI
{
    class PlaningKDUI : DataClsUI
    {
        private const string code1 = "___1%";
        private const string code2 = "___2%";

        private int year;

        internal PlaningKDUI(IEntity entity, int year)
            : base(entity)
        {
            this.year = year;
        }

        public override void UpdateToolbar()
        {
            vo.utbToolbarManager.Visible = false;
            vo.ugeCls.utmMain.Visible = false;
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                dsObjData = (DataSet)db.ExecQuery(string.Format(@"select id, CodeStr, Name, ParentId from {0} where
                    (CodeStr like ? or CodeStr like ?) and
                    id > 0 and SourceID = (select id from DataSources where SupplierCode = 'ФО' and DataCode = 29 and DataName = 'Проект бюджета' and Year = {1})",
                    ActiveDataObj.FullDBName, year), 
                    QueryResultTypes.DataSet,
                    new DbParameterDescriptor("p0", code1),
                    new DbParameterDescriptor("p1", code2));
            }

            AddIntermediateResults(dsObjData.Tables[0]);

            vo.ugeCls.DataSource = dsObjData;
            vo.ugeCls.ServerFilterEnabled = false;
            SetDetailVisible(ActiveDataObj);
            UpdateToolbar();

            DataColumn newColumn = dsObjData.Tables[0].Columns.Add("SelectCode", typeof(Boolean));
            newColumn.Caption = string.Empty;
            newColumn.DefaultValue = false;

            newColumn = dsObjData.Tables[0].Columns.Add("IsParent", typeof(Boolean));
            newColumn.Caption = string.Empty;
            newColumn.DefaultValue = false;

            newColumn = dsObjData.Tables[0].Columns.Add("AddRow", typeof(Boolean));
            newColumn.Caption = string.Empty;
            newColumn.DefaultValue = false;

            foreach (DataRow row in dsObjData.Tables[0].Rows)
            {
                row["SelectCode"] = false;
                row["IsParent"] = false;
                row["AddRow"] = false;
            }

            vo.ugeCls.IsReadOnly = false;
            vo.ugeCls.AllowEditRows = true;
            vo.ugeCls.ugData.DisplayLayout.AddNewBox.Hidden = true;

            foreach (UltraGridBand band in vo.ugeCls.ugData.DisplayLayout.Bands)
            {
                band.Columns["SelectCode"].Header.VisiblePosition = 0;
                band.Columns["SelectCode"].CellActivation = Activation.AllowEdit;
                band.Columns["SelectCode"].Header.Caption = string.Empty;
                band.Columns["ID"].CellActivation = Activation.NoEdit;
                band.Columns["CodeStr"].CellActivation = Activation.NoEdit;
                band.Columns["Name"].CellActivation = Activation.NoEdit;
                band.Columns["ParentId"].CellActivation = Activation.NoEdit;
                band.Columns["IsParent"].Hidden = true;
                band.Columns["AddRow"].Hidden = true;
            }

            vo.ugeCls.ugData.CellChange += ugData_CellChange;
        }

        void ugData_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Text == "True" || e.Cell.Text == "False")
            {
                e.Cell.Value = Convert.ToBoolean(e.Cell.Text);
                e.Cell.Row.Update();
                if (Convert.ToBoolean(e.Cell.Value))
                {
                    object parentId = e.Cell.Row.Cells["ParentID"].Value;
                    SetParentRows(parentId);
                }
            }
        }
        
        private void SetParentRows(object parentId)
        {
            if (parentId == DBNull.Value)
                return;
            foreach (DataRow row in dsObjData.Tables[0].Select(string.Format("ID = {0}", parentId)))
            {
                row["IsParent"] = true;
                SetParentRows(row["ParentID"]);
            }
        }

        private void AddIntermediateResults(DataTable dtKd)
        {
            if (dtKd.Rows.Count == 0)
                return;

            dtKd.BeginLoadData();
            DataRow parentRow = dtKd.Select(string.Empty, "CodeStr ASC")[0];

            foreach (DataRow row in dtKd.Select(string.Format("ParentID = {0}", parentRow["ID"])))
            {
                string codeSub = row["CodeStr"].ToString().Substring(3, 3);
                if (codeSub == "101" || codeSub == "102" || codeSub == "103" || codeSub == "104" || codeSub == "105" ||
                    codeSub == "106" || codeSub == "107" || codeSub == "108" || codeSub == "109")
                    row["ParentID"] = -1;
                else
                    row["ParentID"] = -2;
            }

            // добавляем итоги по налоговым и неналоговым доходам
            DataRow newRow = dtKd.NewRow();
            newRow["CodeStr"] = string.Empty;
            newRow["Name"] = "Всего налоговых доходов";
            newRow["ParentID"] = parentRow["ID"];
            newRow["ID"] = -1;
            dtKd.Rows.Add(newRow);
            newRow = dtKd.NewRow();
            newRow["CodeStr"] = string.Empty;
            newRow["Name"] = "Всего неналоговых доходов";
            newRow["ParentID"] = parentRow["ID"];
            newRow["ID"] = -2;
            dtKd.Rows.Add(newRow);
            dtKd.EndLoadData();
        }
    }
}
