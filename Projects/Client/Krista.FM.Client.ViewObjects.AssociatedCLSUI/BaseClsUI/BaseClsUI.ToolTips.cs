using System;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.Common;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
    {
        private Timer _toolTipTimer;

        private UltraGridCell _lastCellUnderMouse = null;

        private CC.UltraGridEx activeGrid;
        private IEntity activeObject;
        private string activePresentationKey;


        private void toolTipTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // ������ �� ������?
                if (_lastCellUnderMouse == null)
                    return;

                // ������ �� �����?
                if ((_lastCellUnderMouse.Value == null) ||
                    (_lastCellUnderMouse.Value == DBNull.Value) || 
                    (Convert.ToString(_lastCellUnderMouse.Value) == String.Empty))
                    return;

                CellUIElement uiElem = (CellUIElement)_lastCellUnderMouse.GetUIElement();
                // ���� ����� �� ������ �������� ������ �������, �������
                if (uiElem == null) return;
                //Stopwatch sw = new Stopwatch();
                //sw.Start();
                #region ������
                


                // ���� ������ ������ ��� ������, �� ������ ��������, ������� ����� ������������� ��� � ������� ��� �����
                if (/*this.vo.ugeCls*/activeGrid.ColumnIsLookup(_lastCellUnderMouse.Column.Key))
                {
                    string sourceColumnName = CC.UltraGridEx.GetSourceColumnName(_lastCellUnderMouse.Column.Key);
                    object value = _lastCellUnderMouse.Row.Cells[sourceColumnName].Value;
                    if ((value == null) || (value == DBNull.Value))
                        return;
                    string lookupObjName = (string)_lastCellUnderMouse.Column.Tag;
					string lookupText = LookupManager.Instance.GetLookupValue(lookupObjName, true, Convert.ToInt32(value));
                    ShowCellToolTip(uiElem, lookupText);
                    return;
                }
                #endregion

                #region ����������� ������
                // �������� �� 
                string attrName = _lastCellUnderMouse.Column.Key;
                //CC.GridColumnsStates states = this.ugeCls_OnGetGridColumnsState(null);
                GridColumnsStates states = GetColumnStatesFromClsObject(activeObject, activeGrid, activePresentationKey);
                // ������� ��� ��������� ���� ����������������?
                if (!states.ContainsKey(attrName))
                    return;
                // �������� ���������?
                if (!states[attrName].IsReference)
                    return;

                // ������������ ��� � ������ ��������� ����� ID � �������� ��� ��������
                int clsID = Convert.ToInt32(_lastCellUnderMouse.Value);
                //sw.Stop();
                ShowCellToolTip(uiElem, GetReferenceAttributeRenaming(attrName, clsID));
                //Debug.WriteLine(String.Format("ToolTip showed. Build time: {0} ms", sw.ElapsedMilliseconds));
                return;
                #endregion
            }
            finally
            {
                _toolTipTimer.Stop();
                //Debug.WriteLine("ToolTipTimer stopped");
            }
        }


        private void ShowCellToolTip(CellUIElement cellUIElem, string toolTipText)
        {
            if (toolTipText == string.Empty)
                return;
            vo.ToolTip.ToolTipText = toolTipText;
            Point tooltipPos = new Point(cellUIElem.ClipRect.Left, cellUIElem.ClipRect.Bottom);
            vo.ToolTip.Show(/*vo.ugeCls*/activeGrid.ugData.PointToScreen(tooltipPos));
        }

        protected void ugeCls_OnMouseEnterGridElement(object sender, UIElementEventArgs e)
        {
            UltraGrid ug = (UltraGrid)sender;
            if (activeDetailGrid != null && ug == activeDetailGrid.ugData)
            {
                activeGrid = activeDetailGrid;
                activeObject = activeDetailObject;
                activePresentationKey = PresentationKey;
            }
            else if (ug == vo.ugeCls.ugData)
            {
                activeGrid = vo.ugeCls;
                activeObject = activeDataObj;
                activePresentationKey = DetailPresentationKey;
            }

            element = e.Element;
            _activeUIElementIsRow = e.Element is RowSelectorUIElement;

            if (_activeUIElementIsRow)
            {
                if (!ug.Focused)
                    ug.Focus();
                return;
            }

            if (e.Element is CellUIElement)
            {
                // ������ ���������� ������
                vo.ToolTip.Hide();
                // �������� ������ ������� ������������ ��������
                UltraGridCell cell = (UltraGridCell)e.Element.GetContext(typeof(UltraGridCell));
                // ���� ���� ����������� �� ������ ������ - ������������� ������ � ���������� ��
                if (cell != _lastCellUnderMouse)
                {
                    _lastCellUnderMouse = cell;
                    _toolTipTimer.Stop();
                    _toolTipTimer.Start();
                    //Debug.WriteLine("ToolTipTimer restarted");
                }
            }
        }

        protected void ugeCls_OnMouseLeaveGridElement(object sender, UIElementEventArgs e)
        {
            // ���� ������� - ������, ������ ������
            if (e.Element is CellUIElement)
            {
                vo.ToolTip.Hide();
                _toolTipTimer.Stop();
                _lastCellUnderMouse = null;
                //Debug.WriteLine("ToolTipTimer stopped");
            }
        }

    }
}