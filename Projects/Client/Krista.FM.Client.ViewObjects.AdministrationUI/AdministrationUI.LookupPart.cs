using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;
using Infragistics.Win.UltraWinGrid;
using System.Windows.Forms;
using Krista.FM.Client.Components;
using System.Drawing;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AdministrationUI
{
    public partial class AdministrationUI : BaseViewObj, IInplaceTasksPermissionsView
    {
        #region свойства, связанные с тултипами

        private UltraGridCell _lastCellUnderMouse = null;

        private Timer _toolTipTimer = null;

        public Timer ToolTipTimer
        {
            get
            {
                if (_toolTipTimer == null)
                {
                    _toolTipTimer = new Timer();
                    _toolTipTimer.Interval = 500;
                    _toolTipTimer.Tick += new EventHandler(toolTipTimer_Tick);
                }
                return _toolTipTimer;
            }
        }

        private Infragistics.Win.ToolTip toolTipValue = null;

        public Infragistics.Win.ToolTip ToolTip
        {
            get
            {
                if (null == this.toolTipValue)
                {
                    this.toolTipValue = new Infragistics.Win.ToolTip(vo);
                    this.toolTipValue.DisplayShadow = true;
                    this.toolTipValue.AutoPopDelay = 0;
                    this.toolTipValue.InitialDelay = 0;
                }
                return this.toolTipValue;
            }
        }

        #endregion


        #region лукапы


        bool newGrid_OnCheckLookupValue(object sender, string lookupName, object value)
        {
            return true;
        }

        string newGrid_OnGetLookupValue(object sender, string lookupName, bool needFeelValue, object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;
            DataRow row = null;
            int refID = Convert.ToInt32(value);
            switch (lookupName)
            {
                case "REGION":
                    if (regionsLookupCash.ContainsKey(refID))
                        return regionsLookupCash[refID];
                    row = GetRegionData(refID);
                    if (row != null)
                        return string.Format("{0}({1})", row["Name"], row["ID"]);
                    break;
            }

            return Convert.ToString(value);
        }

        private void toolTipTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // ячейка не задана?
                if (_lastCellUnderMouse == null)
                    return;

                // ячейка не пуста?
                if ((_lastCellUnderMouse.Value == null) ||
                    (_lastCellUnderMouse.Value == DBNull.Value) ||
                    (Convert.ToString(_lastCellUnderMouse.Value) == String.Empty))
                    return;

                CellUIElement uiElem = (CellUIElement)_lastCellUnderMouse.GetUIElement();
                // если каким то образм получили пустой элемент, выходим
                if (uiElem == null) return;

                #region Лукапы
                string sourceColumnName = string.Empty;
                // если ячейка служит для лукапа, то найдем значение, которое будем отображатьпод нею в тултипе или хинте
                if (vo.ugeAllList.ColumnIsLookup(_lastCellUnderMouse.Column.Key))
                {
                    sourceColumnName = UltraGridEx.GetSourceColumnName(_lastCellUnderMouse.Column.Key);
                    if (sourceColumnName.ToUpper() != "REFREGION")
                        return;
                    object value = _lastCellUnderMouse.Row.Cells[sourceColumnName].Value;
                    if ((value == null) || (value == DBNull.Value))
                        return;
                    string lookupText = GetReferenceAttributeRenaming(Convert.ToInt32(value), sourceColumnName);
                    ShowCellToolTip(uiElem, lookupText);
                    return;
                }
                #endregion

                #region Разыменовка ссылок
                // является ли 
                string attrName = _lastCellUnderMouse.Column.Key;

                GridColumnsStates states = GetUsersColumns();
                // колонка для аттрибута была инициализирована?
                if (!states.ContainsKey(attrName))
                    return;
                // аттрибут ссылочный?
                if (!states[attrName].IsReference)
                    return;

                // предполагаем что в ячейке находится некое ID и пытаемся его получить
                int refKD = Convert.ToInt32(_lastCellUnderMouse.Value);
                ShowCellToolTip(uiElem, GetReferenceAttributeRenaming(refKD, sourceColumnName));
                return;

                #endregion
            }
            finally
            {
                ToolTipTimer.Stop();
            }
        }

        private void ShowCellToolTip(CellUIElement cellUIElem, string toolTipText)
        {
            if (toolTipText == string.Empty)
                return;
            ToolTip.ToolTipText = toolTipText;
            Point tooltipPos = new Point(cellUIElem.ClipRect.Left, cellUIElem.ClipRect.Bottom);
            ToolTip.Show(vo.ugeAllList.ugData.PointToScreen(tooltipPos));
        }

        void ugeAllList_OnMouseLeaveGridElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            // если элемент - ячейка, прячем тултип
            if (e.Element is CellUIElement)
            {
                ToolTip.Hide();
                ToolTipTimer.Stop();
                _lastCellUnderMouse = null;
            }
        }

        void ugeAllList_OnMouseEnterGridElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {
            if (e.Element is CellUIElement)
            {
                // прячем предыдущий тултип
                ToolTip.Hide();
                // получаем ячейку которая соответсвует элементу
                UltraGridCell cell = (UltraGridCell)e.Element.GetContext(typeof(UltraGridCell));
                // если мышь передвинута на другую ячейку - перезапускаем таймер и запоминаем ее
                if (cell != _lastCellUnderMouse)
                {
                    _lastCellUnderMouse = cell;
                    ToolTipTimer.Stop();
                    ToolTipTimer.Start();
                }
            }
        }

        /// <summary>
        /// разыменовка для поля КД
        /// </summary>
        /// <param name="refKD"></param>
        /// <returns></returns>
        private string GetReferenceAttributeRenaming(int refID, string columnName)
        {
            string res = "Район не найден";
            DataRow row = null;
            switch (columnName.ToUpper())
            {
                case "REFREGION":
                    row = GetRegionData(refID);
                    if (row != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("ID: " + Convert.ToString(row["ID"]));
                        sb.AppendLine("Наименование: " + Convert.ToString(row["Name"]));
                        res = sb.ToString();
                    }
                    break;
            }
            return res;
        }

        private Dictionary<int, string> regionsLookupCash = new Dictionary<int, string>();

        private void FillCash()
        {
            string regionsCashQuery = "select ID, Name from d_Regions_Analysis";
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dt = (DataTable)db.ExecQuery(regionsCashQuery, QueryResultTypes.DataTable);
                foreach (DataRow row in dt.Rows)
                {
                    regionsLookupCash.Add(Convert.ToInt32(row["ID"]), string.Format("{0}({1})", row["Name"], row["ID"]));
                }
            }
        }

        private DataRow GetRegionData(int refRegion)
        {
            IDatabase db = null;
            try
            {
                db = Workplace.ActiveScheme.SchemeDWH.DB;
                string query = string.Format("select ID, Name from d_Regions_Analysis where ID = ?");
                IDbDataParameter param = new System.Data.OleDb.OleDbParameter("ID", refRegion);
                DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable, param);
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                return null;
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
        }

        #endregion

    }
}
