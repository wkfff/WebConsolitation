using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Common;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables;
using Krista.FM.ServerLibrary;

using Infragistics.Win;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid.ExcelExport;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Association
{
    public partial class AssociationUI : BaseViewObj
    {

        UltraGridRow CurrentClsBridgeRow = null;

        #region ���������� ������� �������������
        /// <summary>
        ///  ���������� ������� �� ������ ������������� � �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region ���������� ������� �������������
        /// <summary>
        ///  ���������� ������� �� ������ ������������� � �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void utbmAssociate_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            bool refreshDataCls = false;
            bool refreshBridgeCls = false;

            ButtonTool tool = (ButtonTool)e.Tool;
            string messageText = string.Empty;

            UltraGrid gridClsData = attClsData.UltraGridExComponent.ugData;
            UltraGrid gridClsBridge = attClsBridge.UltraGridExComponent.ugData;

            bool needSaveData = false;
            List<int> addedIDs = null;

            switch (tool.Key)
            {
                // ������������� ������, ������������ 2 �������� � ������ ������ ������
                case "HandAssociate":
                    if (clsDataGrid.ActiveRow == null || clsBridgeGrid.ActiveRow == null)
                    {
                        MessageBox.Show("�� ������� ������ ��� �������������", "��������������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int index = 0;
                    // �������� ���������� ������ � �������������� ��������������
                    if (clsDataGrid.Selected.Rows.Count == 0 && clsDataGrid.ActiveRow != null)
                        clsDataGrid.ActiveRow.Selected = true;
                    DataRow[] clsDataRows = new DataRow[clsDataGrid.Selected.Rows.Count];
                    DataTable clsDataTable = attClsData.GetClsDataSet().Tables[0];
                    foreach (UltraGridRow selectedRow in clsDataGrid.Selected.Rows)
                    {
                        clsDataRows[index] = clsDataTable.Select(string.Format("ID = {0}", selectedRow.Cells["ID"].Value))[0];
                        index++;
                    }
                    // �������� �������� ������ � ������������ ��������������
                    DataTable clsBridgeTable = attClsBridge.GetClsDataSet().Tables[0];
                    DataRow[] clsBridgeRows = clsBridgeTable.Select(string.Format("ID = {0}", clsBridgeGrid.ActiveRow.Cells["ID"].Value));
                    // ������������ ������
                    HandMasterForSingleRecord master = new HandMasterForSingleRecord(curentAssociation, this.Workplace, clsDataRows, clsBridgeRows[0]);
                    try
                    {
                        if (master.ShowDialog() == DialogResult.OK)
                        {
                            //needSaveData = true;
                            foreach (DataRow row in clsDataRows)
                            {
                                row[associationName] = clsBridgeRows[0]["ID"];
                                row.AcceptChanges();
                            }
                        }
                    }
                    finally
                    {
                        master.Dispose();
                    }
                    break;
                // ���������� ����� ������ �������������� ������ � ������������
                case "AddToBridge":
                    if (clsDataGrid.ActiveRow == null)
                    {
                        MessageBox.Show("�� ������� ������ ��� �������������", "��������������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (clsDataGrid.Selected.Rows.Count == 0 && clsDataGrid.ActiveRow != null)
                        clsDataGrid.ActiveRow.Selected = true;

                    int id = -1;
                    addedIDs = new List<int>();
                    foreach (UltraGridRow row in gridClsData.Selected.Rows)
                    {
                        clsDataGrid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                        try
                        {
                            id = curentAssociation.CopyAndAssociateRow(Convert.ToInt32(row.Cells["ID"].Value));
                            row.Cells[curentAssociation.FullDBName].Value = id;
                            addedIDs.Add(id);
                        }
                        finally
                        {
                            clsDataGrid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                        }
                        row.Update();
                    }
                    // ��������� ������������ ����� ����, ������� ��� ����������� ����������� �������� ��� ��������������
                    if (gridClsData.Selected.Rows.Count > 0)
                    {
                        clsBridgeGrid.Selected.Rows.Clear();
                        clsBridgeGrid.ActiveRow = null;
                        refreshBridgeCls = true;
                        needSaveData = true;
                    }
                    break;
                // ������ ��������������� �������������
                case "AssociateMaster":
                    if (AssociateMaster.Associate(curentAssociation, attClsData.CurrentSourceID, this.Workplace))
                    {
                        refreshDataCls = true;
                    }
                    break;
                // ������������ ������������� �� �������������� ������
                case "CreateBridge":
                    messageText = String.Format("������ ������������ ������������� �������������� '{0}.{1}' �� �������������� ������ '{2}.{3}'?",
                        CommonMethods.GetDataObjSemanticRus(this.Workplace.ActiveScheme.Semantics, curentAssociation.RoleBridge), curentAssociation.RoleBridge.Caption,
                        CommonMethods.GetDataObjSemanticRus(this.Workplace.ActiveScheme.Semantics, curentAssociation.RoleData), curentAssociation.RoleData.Caption);
                    if (MessageBox.Show(messageText, "������������ ������������� ��������������", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        this.Workplace.OperationObj.Text = "��������� ������";
                        this.Workplace.OperationObj.StartOperation();
                        try
                        {
                            // ��������� ������������ ������������� �� �������� �������������� ������
                            curentAssociation.FormBridgeClassifier(attClsData.CurrentSourceID);
                            // ��������� � ������������� �������������� ��������
                            ((IClassifier)curentAssociation.RoleBridge).DivideClassifierCode(attClsData.CurrentSourceID);
                            refreshDataCls = true;
                            refreshBridgeCls = true;
                        }
                        finally
                        {
                            this.Workplace.OperationObj.StopOperation();
                        }
                    }
                    break;
                // ������� ������������� (������ �� ������������ �������������)
                case "ClearAllBridgeRef":
                    if (attClsData.CurrentSourceID >= 0)
                    {
                        string dataSourceName = Workplace.ActiveScheme.DataSourceManager.GetDataSourceName(attClsData.CurrentSourceID);
                        messageText =
                            String.Format(
                                "�������� ������������� �������������� '{0}' �� ��������� ������ '{1}, SourceID = {2}'?",
                                curentAssociation.RoleData.Caption, dataSourceName,
                                attClsData.CurrentSourceID);
                    }
                    else
                        messageText = String.Format("�������� ������������� �������������� '{0}'?",
                            curentAssociation.RoleData.Caption);
                    if (MessageBox.Show(messageText, "������� �������������", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        // �������� �� ���� �������, ����������� � ������ �������� -1 (������ �� ������������)
                        this.Workplace.OperationObj.Text = "������� �������������";
                        this.Workplace.OperationObj.StartOperation();
                        try
                        {
                            curentAssociation.ClearAssociationReference(attClsData.CurrentSourceID);
                            refreshDataCls = true;
                        }
                        finally
                        {
                            this.Workplace.OperationObj.StopOperation();
                        }
                    }
                    break;
                // ������� ������� ������ �������������� ������ �� �������������
                case "ClearCurentBridgeRef":
                    if (clsDataGrid.Selected.Rows.Count == 0 && clsDataGrid.ActiveRow != null)
                        clsDataGrid.ActiveRow.Selected = true;
                    foreach (UltraGridRow row in gridClsData.Selected.Rows)
                    {
                        clsDataGrid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                        try
                        {
                            row.Cells[curentAssociation.FullDBName].Value = -1;
                        }
                        finally
                        {
                            clsDataGrid.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                        }

                        row.Update();
                        needSaveData = true;
                    }
                    break;
                // ����� ������, �� ������� �����������
                case "ShowBridgeRow":
                    if (clsDataGrid.ActiveRow == null)
                        break;
                    // ���� ������ ������ �� �����, �� ������� ���������� ��������� ������ (����, ���������)
                    if (CurrentClsBridgeRow != null)
                    {
                        //CurrentClsBridgeRow.Appearance.ResetBackColor();
                        foreach (UltraGridCell cell in CurrentClsBridgeRow.Cells)
                        {
                            if (cell.Appearance.BackColor != Color.YellowGreen)
                            {
                                cell.Appearance.ResetBackColor2();
                            }
                            else
                                cell.Appearance.ResetBackColor();
                        }
                        CurrentClsBridgeRow.Selected = false;
                        CurrentClsBridgeRow.Refresh();
                    }
                    CurrentClsBridgeRow = null;
                    // ���� ���� �������� ����� �� ������, �� ������� ���������
                    clsBridgeGrid.Selected.Rows.Clear();
                    // ���� ������, � ������� ������������ ������� ������ �������������� ������
                    if (clsDataGrid.ActiveRow.Cells[curentAssociation.FullDBName].Value != DBNull.Value)
                        if (Convert.ToInt32(clsDataGrid.ActiveRow.Cells[curentAssociation.FullDBName].Value) >= 0)
                        {
                            this.Workplace.OperationObj.Text = "����� ������";
                            this.Workplace.OperationObj.StartOperation();
                            // ���� ������ � ��������������� ID
                            try
                            {
                                CurrentClsBridgeRow = CC.UltraGridHelper.FindRow(clsBridgeGrid, "ID", clsDataGrid.ActiveRow.Cells[curentAssociation.FullDBName].Value.ToString());
                            }
                            finally
                            {
                                this.Workplace.OperationObj.StopOperation();
                            }
                        }
                    // ���� ����� ������, �������� ��
                    if (CurrentClsBridgeRow != null)
                    {
                        //CurrentClsBridgeRow.Appearance.BackColor = Color.YellowGreen;
                        foreach (UltraGridCell cell in CurrentClsBridgeRow.Cells)
                        {
                            if (cell.Appearance.BackColor != Color.Empty)
                            {
                                cell.Appearance.BackColor2 = Color.YellowGreen;
                                cell.Appearance.AlphaLevel = 250;
                                cell.Appearance.BackHatchStyle = BackHatchStyle.SmallCheckerBoard;
                            }
                            else
                                cell.Appearance.BackColor = Color.YellowGreen;
                        }
                        CurrentClsBridgeRow.Selected = true;
                        CurrentClsBridgeRow.Activate();
                        CurrentClsBridgeRow.ExpandAncestors();
                    }
                    return;
                //break;
                case "GetAssociationExcelReport":
                    GetExcelReport();
                    return;
                case "AddToBridgeAll":
                    this.Workplace.OperationObj.Text = "������� ��������������� ������";
                    this.Workplace.OperationObj.StartOperation();
                    try
                    {
                        if (curentAssociation as IBridgeAssociationReport != null)
                        {
                            ((IBridgeAssociationReport) curentAssociation).CopyAndAssociateAllRow(
                                attClsData.CurrentSourceID);

                            refreshDataCls = true;
                            refreshBridgeCls = true;
                        }
                    }
                    finally
                    {
                        Workplace.OperationObj.StopOperation();
                    }
                    break;
            }
            // ��������� ���������
            this.Workplace.OperationObj.Text = "���������� ������";
            this.Workplace.OperationObj.StartOperation();
            try
            {
                if (needSaveData)
                    attClsData.SaveChanges();
            }
            finally
            {
                this.Workplace.OperationObj.StopOperation();
            }
            // ��������� ������ ���������������
            if (refreshDataCls)
                attClsData.RefreshAttachedData();
            if (refreshBridgeCls)
                attClsBridge.RefreshAttachedData();

            if (addedIDs != null)
                FindAndSelectRows(addedIDs);

            gridClsData = null;
            gridClsBridge = null;
            // �������� ���������� �������, ������ ������...
            curentAssociation.RefreshRecordsCount();
            SetAssociateCount();
            SetFilter();
            HideNoAssociationRef();
        }

        #endregion

        /// <summary>
        /// ����� � ��������� �������
        /// </summary>
        /// <param name="ids"></param>
        private void FindAndSelectRows(List<int> ids)
        {
            clsBridgeGrid.BeginUpdate();
            foreach (int id in ids)
            {
                UltraGridRow row = CC.UltraGridHelper.FindGridRow(clsBridgeGrid, "ID", id);
                if (row != null)
                {
                    row.Selected = true;
                    row.Activate();
                }
            }
            clsBridgeGrid.EndUpdate();
        }


        #endregion
    }
}
