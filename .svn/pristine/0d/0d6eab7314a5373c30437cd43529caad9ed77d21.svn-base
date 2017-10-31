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

        #region Обработчик тулбара сопоставления
        /// <summary>
        ///  Обработчик нажатия на кнопки сопоставления в тулбаре
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region Обработчик тулбара сопоставления
        /// <summary>
        ///  Обработчик нажатия на кнопки сопоставления в тулбаре
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
                // Сопоставление ручное, сопоставляет 2 выбраные в разных гридах записи
                case "HandAssociate":
                    if (clsDataGrid.ActiveRow == null || clsBridgeGrid.ActiveRow == null)
                    {
                        MessageBox.Show("Не выбраны записи для сопоставления", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int index = 0;
                    // получаем выделенные записи в сопоставляемом классификаторе
                    if (clsDataGrid.Selected.Rows.Count == 0 && clsDataGrid.ActiveRow != null)
                        clsDataGrid.ActiveRow.Selected = true;
                    DataRow[] clsDataRows = new DataRow[clsDataGrid.Selected.Rows.Count];
                    DataTable clsDataTable = attClsData.GetClsDataSet().Tables[0];
                    foreach (UltraGridRow selectedRow in clsDataGrid.Selected.Rows)
                    {
                        clsDataRows[index] = clsDataTable.Select(string.Format("ID = {0}", selectedRow.Cells["ID"].Value))[0];
                        index++;
                    }
                    // получаем активную запись в сопоставимом классификаторе
                    DataTable clsBridgeTable = attClsBridge.GetClsDataSet().Tables[0];
                    DataRow[] clsBridgeRows = clsBridgeTable.Select(string.Format("ID = {0}", clsBridgeGrid.ActiveRow.Cells["ID"].Value));
                    // сопоставляем записи
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
                // добавление одной записи классификатора данных в сопоставимый
                case "AddToBridge":
                    if (clsDataGrid.ActiveRow == null)
                    {
                        MessageBox.Show("Не выбраны записи для сопоставления", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    // изменения производятся через базу, поэтому для отображения результатов рефрешим оба классификатора
                    if (gridClsData.Selected.Rows.Count > 0)
                    {
                        clsBridgeGrid.Selected.Rows.Clear();
                        clsBridgeGrid.ActiveRow = null;
                        refreshBridgeCls = true;
                        needSaveData = true;
                    }
                    break;
                // мастер автоматического сопоставления
                case "AssociateMaster":
                    if (AssociateMaster.Associate(curentAssociation, attClsData.CurrentSourceID, this.Workplace))
                    {
                        refreshDataCls = true;
                    }
                    break;
                // Формирование сопоставимого по классификатору данных
                case "CreateBridge":
                    messageText = String.Format("Начать формирование сопоставимого классификатора '{0}.{1}' по классификатору данных '{2}.{3}'?",
                        CommonMethods.GetDataObjSemanticRus(this.Workplace.ActiveScheme.Semantics, curentAssociation.RoleBridge), curentAssociation.RoleBridge.Caption,
                        CommonMethods.GetDataObjSemanticRus(this.Workplace.ActiveScheme.Semantics, curentAssociation.RoleData), curentAssociation.RoleData.Caption);
                    if (MessageBox.Show(messageText, "Формирование сопоставимого классификатора", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        this.Workplace.OperationObj.Text = "Обработка данных";
                        this.Workplace.OperationObj.StartOperation();
                        try
                        {
                            // формируем сопоставимый классификатор по текущему классификатору данных
                            curentAssociation.FormBridgeClassifier(attClsData.CurrentSourceID);
                            // установим у сопоставимого классификатора иерархию
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
                // Очистка сопоставления (ссылок на сопоставимый классификатор)
                case "ClearAllBridgeRef":
                    if (attClsData.CurrentSourceID >= 0)
                    {
                        string dataSourceName = Workplace.ActiveScheme.DataSourceManager.GetDataSourceName(attClsData.CurrentSourceID);
                        messageText =
                            String.Format(
                                "Очистить сопоставление классификатора '{0}' по источнику данных '{1}, SourceID = {2}'?",
                                curentAssociation.RoleData.Caption, dataSourceName,
                                attClsData.CurrentSourceID);
                    }
                    else
                        messageText = String.Format("Очистить сопоставление классификатора '{0}'?",
                            curentAssociation.RoleData.Caption);
                    if (MessageBox.Show(messageText, "Очистка сопоставления", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        // проходим по всем записям, проставляем в ссылки значение -1 (запись не сопоставлена)
                        this.Workplace.OperationObj.Text = "Очистка сопоставления";
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
                // Очистка текущей записи классификатора данных от сопоставления
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
                // Поиск записи, по которой сопоставили
                case "ShowBridgeRow":
                    if (clsDataGrid.ActiveRow == null)
                        break;
                    // если искали запись до этого, то очищаем предыдущую найденную запись (цвет, выделение)
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
                    // если были выделены какие то записи, то убираем выделение
                    clsBridgeGrid.Selected.Rows.Clear();
                    // ищем запись, к которой сопоставлена текущая запись классификатора данных
                    if (clsDataGrid.ActiveRow.Cells[curentAssociation.FullDBName].Value != DBNull.Value)
                        if (Convert.ToInt32(clsDataGrid.ActiveRow.Cells[curentAssociation.FullDBName].Value) >= 0)
                        {
                            this.Workplace.OperationObj.Text = "Поиск данных";
                            this.Workplace.OperationObj.StartOperation();
                            // Ищем запись с соответствующим ID
                            try
                            {
                                CurrentClsBridgeRow = CC.UltraGridHelper.FindRow(clsBridgeGrid, "ID", clsDataGrid.ActiveRow.Cells[curentAssociation.FullDBName].Value.ToString());
                            }
                            finally
                            {
                                this.Workplace.OperationObj.StopOperation();
                            }
                        }
                    // если нашли запись, помечаем ее
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
                    this.Workplace.OperationObj.Text = "Перенос несопоставленых данных";
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
            // сохраняем изменения
            this.Workplace.OperationObj.Text = "Сохранение данных";
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
            // обновляем данные классификаторов
            if (refreshDataCls)
                attClsData.RefreshAttachedData();
            if (refreshBridgeCls)
                attClsBridge.RefreshAttachedData();

            if (addedIDs != null)
                FindAndSelectRows(addedIDs);

            gridClsData = null;
            gridClsBridge = null;
            // получаем количество записей, ставим фильтр...
            curentAssociation.RefreshRecordsCount();
            SetAssociateCount();
            SetFilter();
            HideNoAssociationRef();
        }

        #endregion

        /// <summary>
        /// поиск и выделение записей
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
