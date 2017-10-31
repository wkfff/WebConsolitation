using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using CC = Krista.FM.Client.Components;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common;

using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.Design;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinMaskedEdit;
using System.Threading;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI
    {
        protected virtual void ugeCls_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (e.Tool == null) return;
            ButtonTool btn = e.Tool as ButtonTool;
            if (btn == null)
                return;
            ToolBarOperations(btn.Key);
        }

        protected virtual void ToolBarOperations(string toolKey)
        {
            switch (toolKey)
            {
                // собственно осталась только одна кнопка, которая пока не вынесена в общий тулбар
                // Кнопка ращепления
                case "SetHierarchy":
                    SetHierarchy();
                    break;
                case "btnReserveRow":
                    if (clsClassType == ClassTypes.clsFactData ||
                        clsClassType == ClassTypes.clsFixedClassifier)
                        return;
                    HierarchyInfo hi = vo.ugeCls.HierarchyInfo;
                    // параметры по изменению диапазона ID записей
                    bool reserveChildRows = false;
                    bool reserveAllRows = false;
                    bool reserveRows = false;
                    if (frmSetReserveRows.ReserveRowsMode(ref reserveRows, ref reserveAllRows, ref reserveChildRows, hi.LevelsCount))
                    {
                        if (reserveAllRows)
                        {
                            ChangeAllRowsDiapazonID(reserveRows);
                        }
                        else
                        {
                            if (vo.ugeCls.ugData.Selected.Rows.Count == 0)
                            {
                                MessageBox.Show(parentWindow, "Записи для изменения диапазона не выбраны", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            // получаем список ID выделенных записей
                            List<int> selectedNonDeletedRowsID = new List<int>();
                            if (!reserveChildRows)
                            {
                                List<int> selectedRowsID;
                                UltraGridHelper.GetSelectedIDs(vo.ugeCls.ugData, out selectedRowsID);
                                // получаем список ID тех записей, которые не помечены на удаление
                                foreach (int id in selectedRowsID)
                                {
                                    if (dsObjData.Tables[0].Select(String.Format("ID = {0}", id)).Length > 0)
                                    {
                                        if ((reserveRows && id < developerRowDiapazon) || (!reserveRows && id >= developerRowDiapazon))
                                            selectedNonDeletedRowsID.Add(id);
                                    }
                                }
                            }
                            else
                            {
                                foreach (UltraGridRow selectedParentRow in vo.ugeCls._ugData.Selected.Rows)
                                {
                                    int id = Convert.ToInt32(selectedParentRow.Cells["ID"].Value);
                                    if (dsObjData.Tables[0].Select(String.Format("ID = {0}", id)).Length > 0)
                                    {
                                        if ((reserveRows && id < developerRowDiapazon) || (!reserveRows && id >= developerRowDiapazon))
                                            selectedNonDeletedRowsID.Add(id);
                                        GetChildRowsID(selectedParentRow, selectedNonDeletedRowsID, reserveRows);
                                    }
                                }
                            }
                            // если нету не ввыделенных удаленных записей, то ниче не делаем
                            if (selectedNonDeletedRowsID.Count <= 0)
                                return;

                            this.Workplace.OperationObj.Text = "Обработка данных";
                            this.Workplace.OperationObj.StartOperation();
                            try
                            {
                                // получаем список измененных ID
                                int[] newIds = ((IClassifier)this.ActiveDataObj).ReverseRowsRange(selectedNonDeletedRowsID.ToArray());
                                // во всех выделенных записях изменяем ID
                                ChangeRowsDiapazonID(selectedNonDeletedRowsID, newIds, reserveChildRows, reserveRows, hi.ParentRefClmnName);
                            }
                            finally
                            {
                                this.Workplace.OperationObj.StopOperation();
                            }
                        }
                    }
                    break;

                case "btnTestFunction":
                    CC.UltraGridHelper.FindChildRow(this.vo.ugeCls._ugData.ActiveRow, "ID", 4313);
                    break;
                // Если это кнопка показа данных
                case "ShowDependedData":
                    if (vo.ugeCls.ugData.ActiveRow != null)
                    {
                        this.Workplace.OperationObj.Text = "Поиск зависимых данных...";
                        this.Workplace.OperationObj.StartOperation();

                        int allChildRowCount = 0;
                        try
                        {
                            // Считаем подчиненные записи в этом же объекте.
                            GetChildRowsCount(vo.ugeCls.ugData.ActiveRow, ref allChildRowCount);
                        }
                        finally
                        {
                            Workplace.OperationObj.StopOperation();
                            // Показывем зависимые данные.
                            FrmDependedData.ShowDependedData(ActiveDataObj, UltraGridHelper.GetActiveID(vo.ugeCls.ugData),
                                DirectChildRowCount(vo.ugeCls.ugData.ActiveRow), allChildRowCount, DependedDataSearchType.User, GetFileName(), (Form)Workplace);
                        }
                    }
                    else
                    {
                        MessageBox.Show(parentWindow, "Не выбрана запись для поиска", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;
                // Кнопка слияния дубликатов.
                case "btnMergingDuplicates":
                    // Если выбрана одна запись, 
                    if (vo.ugeCls.ugData.Selected.Rows.Count == 1)
                        // то показываем форму выбора дубликатов.
                        frmMergingDuplicates.ChooseDuplicatesFormShow(Convert.ToInt32(vo.ugeCls.ugData.Selected.Rows[0].Cells["ID"].Value), this);
                    break;
                // Кнопка видимости деталей.
                case "btnDetailVisible":
                    vo.spMasterDetail.Panel2Collapsed = !vo.spMasterDetail.Panel2Collapsed;
                    if (vo.utcDetails.Visible)
                        vo.ugeCls.utmMain.Tools["btnDetailVisible"].SharedProps.ToolTipText = "Скрыть детали";
                    else
                        vo.ugeCls.utmMain.Tools["btnDetailVisible"].SharedProps.ToolTipText = "Показать детали";
                    break;
                case "btnTopLevelRow":
                    AddNewTemplate(null, true);
                    break;
                case "btnChildRow":
                    AddNewTemplate(UltraGridHelper.GetActiveRowCells(vo.ugeCls.ugData), false);
                    break;
            }
        }


        private SetBoolDelegate showProgress;

        private void SetHierarchy()
        {
            if (!vo.ugeCls.HierarchyInfo.isDivideCode)
                return;
            if (CurrentDataSourceID == NullDataSource && HasDataSources())
            {
                MessageBox.Show(parentWindow, "Установку иерархии выполнить невозможно, источник данных не выбран.", "Установка иерархии", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmSetHierarchyMode.DivideAndFormHierarchyMode mode = frmSetHierarchyMode.DivideAndFormHierarchyMode.FullHierarchy;
            bool isHierarchyClassifier = vo.ugeCls.HierarchyInfo.LevelsCount > 1;
            bool isSetFullHierarchy = true;
            if (frmSetHierarchyMode.SelectSettingHierarchyMode(isHierarchyClassifier, ref mode, parentWindow))
            {
                switch (mode)
                {
                    case frmSetHierarchyMode.DivideAndFormHierarchyMode.DivideOnly:
                        Workplace.OperationObj.Text = "Ращепление кода";
                        Workplace.OperationObj.StartOperation();
                        try
                        {
                            IClassifier classifier = (IClassifier)ActiveDataObj;
                            if (CurrentDataSourceID > 0)
                                classifier.DivideClassifierCode(CurrentDataSourceID);
                            else
                                classifier.DivideClassifierCode(-1);
                        }
                        finally
                        {
                            Workplace.OperationObj.StopOperation();
                        }
                        return;
                    case frmSetHierarchyMode.DivideAndFormHierarchyMode.FullHierarchy:
                        break;
                    case frmSetHierarchyMode.DivideAndFormHierarchyMode.WhereNotSetHierarchy:
                        isSetFullHierarchy = false;
                        break;
                }
                if (showProgress == null)
                    showProgress = new SetBoolDelegate(DivideAndFormHierarchy);

                DivideAndFormHierarchy(isSetFullHierarchy);

                if (mode != frmSetHierarchyMode.DivideAndFormHierarchyMode.NoDivideNoFormHierarchy)
                    LoadData(vo.ugeCls, null);
            }
        }

        /// <summary>
        /// добавление нового элемента в репозиторий
        /// </summary>
        private void AddNewTemplate(UltraGridRow activeRow, bool toTopLevel)
        {
            if (!toTopLevel && activeRow != null)
            {
                UltraGridRow row = activeRow.ChildBands[0].Band.AddNew();
                row.Update();
            }
            else
            {
                UltraGridRow addRow = vo.ugeCls.ugData.DisplayLayout.Bands[0].AddNew();
                addRow.Update();
            }
        }

        /// <summary>
        /// Возвращает количество прямых подчиненных записей.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private int DirectChildRowCount(UltraGridRow row)
        {
            int rowsCount = 0;
            if (row.ChildBands != null)
            {
                rowsCount = row.ChildBands[0].Rows.Count;
            }
            return rowsCount;
        }

        /// <summary>
        /// Обходим подчиненные записи и считаем количество.
        /// </summary>
        /// <param name="row">С какой строки обходим.</param>
        /// <param name="childRowsCount">Количество подчиненных.</param>
        private void GetChildRowsCount(UltraGridRow row, ref int childRowsCount)
        {
            if (row.ChildBands != null)
                foreach (UltraGridChildBand band in row.ChildBands)
                {
                    foreach (UltraGridRow childRow in band.Rows)
                    {
                        GetChildRowsCount(childRow, ref childRowsCount);
                        childRowsCount++;
                    }
                }
        }

        private void DivideAndFormHierarchy(bool setFullHierarchy)
        {
            this.Workplace.OperationObj.Text = "Обработка данных";
            this.Workplace.OperationObj.StartOperation();
            try
            {
                IClassifier classifier = (IClassifier)ActiveDataObj;
                int countRowsDisint = 0;
                if (CurrentDataSourceID >= 0)
                    // если установлен источник, то устанавливаем иерархию только по этому источнику
                    countRowsDisint = classifier.DivideAndFormHierarchy(CurrentDataSourceID, setFullHierarchy);
                else
                    // если нет, то по всему классификатору
                    countRowsDisint = classifier.DivideAndFormHierarchy(-1, setFullHierarchy);
            }
            finally
            {
                this.Workplace.OperationObj.StopOperation();
            }
        }
    }
}
