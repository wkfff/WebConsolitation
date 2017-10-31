using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Handbooks
{
    public class SelectRowsClsUI : DataClsUI
    {
        internal SelectRowsClsUI(IEntity entity, int year)
            : base(entity)
        {
            CurrentDataSourceYear = year;
        }

        public override void UpdateToolbar()
        {
            vo.utbToolbarManager.Visible = false;
            vo.ugeCls.utmMain.Visible = false;
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);
            dsObjData.Tables[0].Columns.Add("SelectedRow", typeof(bool));
            foreach (DataRow row in dsObjData.Tables[0].Rows)
            {
                row["SelectedRow"] = false;
            }

            vo.ugeCls.DataSource = dsObjData;

            vo.ugeCls.IsReadOnly = false;
            vo.ugeCls.AllowEditRows = true;
            vo.ugeCls.ugData.DisplayLayout.AddNewBox.Hidden = true;

            foreach (UltraGridBand band in vo.ugeCls.ugData.DisplayLayout.Bands)
            {
                foreach (var column in band.Columns)
                {
                    column.Hidden = true;
                    column.CellActivation = Activation.NoEdit;
                }
                band.Columns["CodeStr_Remasked"].Hidden = false;
                band.Columns["Name"].Hidden = false;
                band.Columns["SelectedRow"].Hidden = false;
                band.Columns["SelectedRow"].Header.VisiblePosition = 0;
                band.Columns["SelectedRow"].CellActivation = Activation.AllowEdit;
                band.Columns["SelectedRow"].Header.Caption = string.Empty;
            }
        }
    }
}
