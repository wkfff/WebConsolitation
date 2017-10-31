using System;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;

using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win;

using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Association;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    public abstract partial class BaseClsUI : BaseViewObj, IInplaceClsView
	{
		private List<UltraGridRow> selectedRows = null;

		private List<int> selectedRowsID = null;

        private DataTable associationDataTable;


        private DataTable GetAssociationsTable()
        {
            DataTable dt = null;
            switch (clsClassType)
            {
				case ClassTypes.clsFactData:
				case ClassTypes.clsDataClassifier:
                    dt = ActiveDataObj.Associations.GetDataTable();
                    break;
                case ClassTypes.clsBridgeClassifier:
                    dt = ActiveDataObj.Associated.GetDataTable();
                    DataTable dt2 = ActiveDataObj.Associations.GetDataTable();
                    DataTableHelper.CopyDataTable(dt2, ref dt);
                    break;
            }
            return dt;
        }

        private void AssociationPageLoad(Infragistics.Win.UltraWinTabControl.UltraTab tab)
        {
            if (tab == null) return;
            if (tab.Index == 2)
                FillProtocols();
            if (tab.Index == 1)
            {
                vo.ugAssociations.InitializeRow += new InitializeRowEventHandler(ugAssociations_InitializeRow);
                vo.ugAssociations.ClickCellButton += new CellEventHandler(ugAssociations_ClickCellButton);

                RefreshRowsCountInAllAssociations();

                vo.udsAssociations.Rows.Clear();
                InitAssociationPage();
                vo.ugAssociations.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                vo.ugAssociations.DisplayLayout.Bands[0].Columns[1].Header.Caption = "Категория сопоствляемого";
                vo.ugAssociations.DisplayLayout.Bands[0].Columns[2].Header.Caption = "Сопоставляемый классификатор";
                vo.ugAssociations.DisplayLayout.Bands[0].Columns[3].Header.Caption = "Категория сопоставимого";
                vo.ugAssociations.DisplayLayout.Bands[0].Columns[4].Header.Caption = "Сопоставимый классификатор";
                vo.ugAssociations.DisplayLayout.Bands[0].Columns["AssociatedRecCnt"].Header.Caption = "Всего записей";
                vo.ugAssociations.DisplayLayout.Bands[0].Columns["UnassociatedrecCnt"].Header.Caption = "Не сопоставлено записей";
            }
        }

		private void utcDataCls_ActiveTabChanged(object sender, Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventArgs e)
        {
            // переход на страничку с сопоставлениями
            AssociationPageLoad(e.Tab);
        }

        private IAssociation GetAssociation(string associationName)
        {
            if (this.Workplace.ActiveScheme.Associations.ContainsKey(associationName))
                return (IAssociation)this.Workplace.ActiveScheme.Associations[associationName];
            return null;
        }

        /// <summary>
        /// заполняем грид данными по сопоставлениям текущего классификатора
        /// </summary>
		public void InitAssociationPage()
		{
			// заполняем список ассоциаций в которые входит объек
            if (associationDataTable == null)
                associationDataTable = GetAssociationsTable();
            else if (associationDataTable.Rows.Count == 0)
                associationDataTable = GetAssociationsTable();

            //vo.ugAssociations.DataSource = null;
            UltraGridColumn clmnAllRecords = null;
            UltraGridColumn clmnUnassociateRecords = null;
            if (!vo.ugAssociations.DisplayLayout.Bands[0].Columns.Exists("AssociatedRecCnt"))
                clmnAllRecords = vo.ugAssociations.DisplayLayout.Bands[0].Columns.Add("AssociatedRecCnt", "Всего записей");
            else
                clmnAllRecords = vo.ugAssociations.DisplayLayout.Bands[0].Columns["AssociatedRecCnt"];

            if (!vo.ugAssociations.DisplayLayout.Bands[0].Columns.Exists("UnassociatedrecCnt"))
                clmnUnassociateRecords = vo.ugAssociations.DisplayLayout.Bands[0].Columns.Add("UnassociatedrecCnt", "Не сопоставлено записей");
            else
                clmnUnassociateRecords = vo.ugAssociations.DisplayLayout.Bands[0].Columns["UnassociatedrecCnt"];

            if (!vo.ugAssociations.DisplayLayout.Bands[0].Columns.Exists("GoToAssociation"))
            {
                UltraGridColumn clmn = vo.ugAssociations.DisplayLayout.Bands[0].Columns.Add("GoToAssociation", "");
                UltraGridHelper.SetLikelyButtonColumnsStyle(clmn, -1);
                clmn.CellButtonAppearance.Image = vo.ilImages.Images[11];
                clmn.Header.VisiblePosition = 0;
            }

            //vo.ugAssociations.BeginUpdate();
            vo.ugAssociations.DataSource = associationDataTable;
            //vo.ugAssociations.EndUpdate();

            clmnAllRecords.Header.VisiblePosition = 7;
            clmnUnassociateRecords.Header.VisiblePosition = 8;
		}

        void ugAssociations_ClickCellButton(object sender, CellEventArgs e)
        {
			Workplace.SwitchTo("Классификаторы и таблицы", typeof(Navigation.AssociationNavigationListUI).FullName, e.Cell.Row.Cells[0].Value);
        }

        void ugAssociations_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["GoToAssociation"].ToolTipText = "Перейти на интерфейс сопоставления";

            IBridgeAssociation ass = (IBridgeAssociation)GetAssociation(e.Row.Cells[0].Value.ToString());
            if (clsClassType == ClassTypes.clsDataClassifier)
            {
                e.Row.Cells["AssociatedRecCnt"].Value = string.Format("{0} (По всем источникам {1})",
                    ass.GetRecordsCountByCurrentDataSource(CurrentDataSourceID), ((IBridgeAssociation)ass).GetAllRecordsCount());
                e.Row.Cells["UnassociatedrecCnt"].Value = string.Format("{0} (По всем источникам {1})",
                    ass.GetUnassociateRecordsCountByCurrentDataSource(CurrentDataSourceID), ((IBridgeAssociation)ass).GetAllUnassociateRecordsCount());
            }
            else
            {
                e.Row.Cells["AssociatedRecCnt"].Value = string.Format("{0}", ass.GetAllRecordsCount());
                e.Row.Cells["UnassociatedrecCnt"].Value = string.Format("{0}", ass.GetAllUnassociateRecordsCount());
            }
        }

		private bool GetActiveDataSource(ref int dataSourceID, ref string dataSourceName)
		{
			ComboBoxTool cbDataSources = (ComboBoxTool)vo.utbToolbarManager.Tools["cbDataSources"];
			bool finded = false;
			foreach (ValueListItem item in cbDataSources.ValueList.ValueListItems)
			{
				if (item.DisplayText == cbDataSources.Text)
				{
					dataSourceID = Convert.ToInt32(item.DataValue);
					dataSourceName = item.DisplayText;
					finded = true;
					break;
				}
			}
			return finded;
		}

        public void RefreshRowsCountInAllAssociations()
        {
            switch (clsClassType)
            {
                case ClassTypes.clsDataClassifier:
                    foreach (IAssociation ass in ActiveDataObj.Associations.Values)
                    {
                        if (ass is IBridgeAssociation)
                            ((IBridgeAssociation)ass).RefreshRecordsCount();
                    }
                    break;
                case ClassTypes.clsBridgeClassifier:
                    foreach (IEntityAssociation ass in ActiveDataObj.Associations.Values)
                    {
                        if (ass is IBridgeAssociation)
                            ((IBridgeAssociation)ass).RefreshRecordsCount();
                    }
                    foreach (IEntityAssociation ass in ActiveDataObj.Associated.Values)
                    {
                        if (ass is IBridgeAssociation)
                            ((IBridgeAssociation)ass).RefreshRecordsCount();
                    }
                    break;
            }
        }

		private void Associations_ButtonClick(object sender, CellEventArgs e)
		{
            
		}

		#region РеализациЯ Drag&Drop
		private UltraGridRow LastEnteredRow;

		/// <summary>
		/// Получить UltraGridRow от UIElement. Опрашиваются и потомки.
		/// </summary>
		/// <param name="elem">элемент</param>
		/// <returns>строка</returns>
		private UltraGridRow GetRowFromElement(UIElement elem)
		{
			UltraGridRow row = null;
			try
			{
				row = (UltraGridRow)elem.GetContext(typeof(UltraGridRow), true);
			}
			catch {	}
			return row;
		}

		/// <summary>
		/// Получить UltraGridRow по экранным координатам. Опрашиваются и потомки.
		/// </summary>
		/// <param name="X">Координата X (экранная)</param>
		/// <param name="Y">Координата Y (экранная)</param>
		/// <returns></returns>
		private UltraGridRow GetRowFromPos(int X, int Y)
		{ 
			Point pt = new Point(X, Y);
			pt = vo.ugeCls.ugData.PointToClient(pt);
			UIElement elem = vo.ugeCls.ugData.DisplayLayout.UIElement.ElementFromPoint(pt);
			return GetRowFromElement(elem);
		}

		/// <summary>
		/// Выделить строку (или вернуть в нормальное состояние)
		/// </summary>
		/// <param name="row">строка</param>
		/// <param name="colored">признак необходимости выделения</param>
		private void ColoredRow(UltraGridRow row, bool colored)
		{
			if (row == null) return;
			if (colored)
			{
				row.Appearance.AlphaLevel = 150;
				row.Appearance.BackColor2 = Color.Red;
				row.Appearance.BackHatchStyle = BackHatchStyle.Percent50;
			}
			else
			{
				row.Appearance.AlphaLevel = vo.ugeCls.ugData.DisplayLayout.Override.RowPreviewAppearance.AlphaLevel;
				row.Appearance.BackColor2 = vo.ugeCls.ugData.DisplayLayout.Override.RowPreviewAppearance.BackColor2;
				row.Appearance.BackHatchStyle = vo.ugeCls.ugData.DisplayLayout.Override.RowPreviewAppearance.BackHatchStyle;
			}
			try
			{
				row.GetUIElement().Invalidate(true);
			}
			catch { }
		}

		/// <summary>
		/// Процедура проверки допустимости перетаскивания на строку.
		/// Дописать.
		/// Должна возвращать DragDropEffects.None если трока равна перетаскиваемой
		/// или является дочерней (во избежание циклических ссылок)
		/// </summary>
		/// <param name="row">Строка-приемник</param>
		/// <returns>Допустимое действие</returns>
		private DragDropEffects CheckRow(UltraGridRow row, bool afterSetValues)
		{
            if (selectedRows.Contains(row))
                return DragDropEffects.None;
            if (selectedRows.Count > 0)
            {
                if (selectedRows[0].ParentRow == row)
                    return DragDropEffects.None;
                if (selectedRows[0].Cells[vo.ugeCls.HierarchyInfo.ParentRefClmnName].Value == DBNull.Value &&
                    row == null && !afterSetValues)
                    return DragDropEffects.None;
            }
            if (row == null)
                return DragDropEffects.Move;
			if (row.Band.Index >= vo.ugeCls.ugData.DisplayLayout.MaxBandDepth - 1)
				return DragDropEffects.None;
			if (CheckDropRow(row))
				return DragDropEffects.Move;
			return DragDropEffects.None;
		}

		/// <summary>
		///  Процедура проверки допустимости перетаскивания на строку. 
		///  Возвращает действие, которое должно производиться над перетаскиваемой записью
		/// </summary>
		/// <param name="dropRow">Запись, над которой стоим при перетаскивании</param>
		/// <returns>Тип действия над записью</returns>
		private bool CheckDropRow(UltraGridRow dropRow)
		{
            if (selectedRows.Contains(dropRow))
			    return false;
			if (dropRow.ParentRow != null)
			{
				if (CheckDropRow(dropRow.ParentRow))
					return true;
				return false;
			}
			return true;
		}

		void ugeCls_GridSelectionDrag(object sender, CancelEventArgs e)
		{
            // если нет прав на изменения иерархии, то выходим
            if (!allowChangeHierarchy) return;
			// для внедренных объектов тоже отключаем
			// для плоксого представления и фиксированных классификаторов Drag&Drop невозможен
		    UltraGridEx gridEx = sender as UltraGridEx;
            if (gridEx == null)
                return;
            if ((gridEx.ugData.DisplayLayout.ViewStyle == ViewStyle.SingleBand) || (clsClassType == ClassTypes.clsFixedClassifier))
				return;
            // так же отключаем для неирархических классификаторов
            if (gridEx.HierarchyInfo.ParentClmnName == string.Empty || gridEx.HierarchyInfo.ParentRefClmnName == string.Empty)
                return;
            gridEx.inDragDrop = true;
			try
			{
				if (!InInplaceMode)
				{
					// получаем активную строку
                    selectedRows = new List<UltraGridRow>();
                    foreach(UltraGridRow selectRow in gridEx.ugData.Selected.Rows)
                        selectedRows.Add(selectRow);
                    UltraGridHelper.GetSelectedIDs(gridEx.ugData, out selectedRowsID);
					// насинаем Drag&Drop
                    gridEx.ugData.DoDragDrop(vo.ugeCls.ugData.Selected.Rows, DragDropEffects.Move);
					e.Cancel = true;
				}
			}
			finally
			{
				// Возвращаем все в нормальное состояние, обнуляем указатели
				ColoredRow(LastEnteredRow, false);
				if (!InInplaceMode)
				{
					if (CheckRow(LastEnteredRow, true) == DragDropEffects.Move && !InInplaceMode)
                        if (LastEnteredRow == null)
                        {
                            if (CheckAllowAddNew() == AllowAddNew.Yes)
                            {
                                foreach (int id in selectedRowsID)
                                {
                                    vo.ugeCls.SetRowToStateByID(id, vo.ugeCls.ugData.Rows, UltraGridEx.LocalRowState.Modified);
                                }
                            }
                        }
                        else
                        {
                            foreach (int id in selectedRowsID)
                            {
                                vo.ugeCls.SetRowToStateByID(id, LastEnteredRow.ChildBands[0].Rows, UltraGridEx.LocalRowState.Modified);
                            }
                        }
				}
                vo.ugeCls.ugData.DisplayLayout.RefreshFilters();
                if (selectedRows != null)
                {
                    selectedRows.Clear();
                    selectedRowsID.Clear();
                }
				LastEnteredRow = null;
			}
		}

		void ugeCls_GridDragOver(object sender, DragEventArgs e)
		{
			// ПОлучаем строку над которой в данный момент находимся
			UltraGridRow tmpRow = GetRowFromPos(e.X, e.Y);

            if (!InInplaceMode)
            {
                if (e.Data.GetDataPresent("FileDrop"))
                    e.Effect = DragDropEffects.Copy;
                else
                {
                    // Если она не является перетаскиваемой и выделенной в данный момент
                    if ((tmpRow != LastEnteredRow) && (!selectedRows.Contains(tmpRow)))
                    {
                        // Снимаем выделение с предыдущей выделенной
                        ColoredRow(LastEnteredRow, false);
                        // Запоминаем строку
                        LastEnteredRow = tmpRow;
                        // Выделяем
                        ColoredRow(LastEnteredRow, true);
                        if (tmpRow != null)
                        {
                            int rowsCount = vo.ugeCls.ugData.ActiveRowScrollRegion.VisibleRows.Count;
                            if (tmpRow == vo.ugeCls.ugData.ActiveRowScrollRegion.VisibleRows[rowsCount - 3].Row ||
                                tmpRow == vo.ugeCls.ugData.ActiveRowScrollRegion.VisibleRows[rowsCount - 2].Row ||
                                tmpRow == vo.ugeCls.ugData.ActiveRowScrollRegion.VisibleRows[rowsCount - 1].Row)
                                vo.ugeCls.scrollDirection = UltraGridEx.ScrollDirections.down;
                            else
                                if (tmpRow == vo.ugeCls.ugData.ActiveRowScrollRegion.VisibleRows[0].Row ||
                                    tmpRow == vo.ugeCls.ugData.ActiveRowScrollRegion.VisibleRows[1].Row ||
                                    tmpRow == vo.ugeCls.ugData.ActiveRowScrollRegion.VisibleRows[2].Row)
                                    vo.ugeCls.scrollDirection = UltraGridEx.ScrollDirections.up;
                                else
                                    vo.ugeCls.scrollDirection = UltraGridEx.ScrollDirections.unknown;
                        }
                    }
                    // Проверяем допустимость перетаскивания
                    e.Effect = CheckRow(tmpRow, false);
                }
            }
		}

		void ugeCls_GridDragLeave(object sender, EventArgs e)
		{
			ColoredRow(LastEnteredRow, false);
            vo.ugeCls.inDragDrop = false;
		}

		void ugeCls_GridDragEnter(object sender, DragEventArgs e)
		{
            if (e.Data.GetDataPresent("FileDrop") && !InInplaceMode)
                e.Effect = DragDropEffects.Copy;
            else
            {
                UltraGridRow tmpRow = GetRowFromPos(e.X, e.Y);

                ColoredRow(LastEnteredRow, false);
                LastEnteredRow = tmpRow;
                ColoredRow(LastEnteredRow, true);

                if (!InInplaceMode)
                {
                    e.Effect = CheckRow(LastEnteredRow, false);
                }
            }
		}


		void ugeCls_GridDragDrop(object sender, DragEventArgs e)
		{
		    UltraGridEx gridEx = sender as UltraGridEx;
            if ((gridEx) == null)
		        return;

            if (e.Data.GetDataPresent("FileDrop") && !InInplaceMode)
            {
                string[] files = (string[])e.Data.GetData("FileDrop");
                List<string> dropFiles = new List<string>();
                foreach (string file in files)
                {
                    FileAttributes attr = File.GetAttributes(file);
                    // если это директория 
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        dropFiles.AddRange(Directory.GetFiles(file));
                    else
                        dropFiles.Add(file);
                }
                // добавляем файлы в задачу
                Workplace.OperationObj.Text = String.Format("Добавление файлa: {0}", "...");
                Workplace.OperationObj.StartOperation();
                try
                {
                    foreach (string fileName in dropFiles)
                    {
                        // добавление записи с документом...
                        // если документов в записи несколько, документ добавляем в первый
                        AddDocumentRow(fileName, vo.ugeCls);
                    }
                }
                finally
                {
                    Workplace.OperationObj.StopOperation();
                }
            }
            else
            {
                gridEx.ugData.BeginUpdate();
                try
                {
                    if (!InInplaceMode)
                    {
                        // Получаем элемент под курсором, он будет родителем для перетаскиваемой записи
                        UltraGridRow row = GetRowFromPos(e.X, e.Y);
                        // если кинули на саму себя - ничего не делаем
                        if (!selectedRows.Contains(row))
                        {

                            UltraGridRow parentRow = null;
                            if (selectedRows.Count > 0)
                            {
                                parentRow = selectedRows[0].ParentRow;
                            }

                            HierarchyInfo hi = gridEx.HierarchyInfo;
                            if (row == null)
                            {
                                if (!InInplaceMode)
                                    if (MessageBox.Show("Вы действительно хотите назначить группу записей корневым элементом?", "Установка иерархии", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        // Если кинули строку в "пустоту", то она будет корневой
                                        //vo.ugeCls.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                                        foreach (int selectID in selectedRowsID)
                                        {
                                            DataRow dataRow = dsObjData.Tables[0].Select(string.Format("ID = {0}", selectID))[0];
                                            if (AllowEditCurrentRow(dataRow))
                                                dataRow[hi.ParentRefClmnName] = DBNull.Value;
                                        }

                                        //gridEx.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                                    }
                            }
                            else
                            {
                                // если кинули на другую строку - делаем ее родительской
                                if (!InInplaceMode)
                                    if (MessageBox.Show("Вы действительно хотите назначить родительскую запись?", "Установка иерархии", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        //vo.ugeCls.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                                        /*foreach (UltraGridRow dragDropRow in selectedRows)
                                        {
                                            if (AllowEditCurrentRow(dragDropRow))
                                            {
                                                dragDropRow.Cells[hi.ParentRefClmnName].Value = row.Cells[hi.ParentClmnName].Value;
                                                dragDropRow.Update();
                                            }
                                        }*/
                                        foreach (int selectID in selectedRowsID)
                                        {
                                            DataRow dataRow = dsObjData.Tables[0].Select(string.Format("ID = {0}", selectID))[0];
                                            if (AllowEditCurrentRow(dataRow))
                                                dataRow[hi.ParentRefClmnName] = row.Cells[hi.ParentClmnName].Value;
                                        }

                                        gridEx.BurnChangesDataButtons(true);
                                        //gridEx.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                                    }
                            }
                            vo.ugeCls.BurnChangesDataButtons(true);
                            gridEx.ugData.Selected.Rows.Clear();
                            // мы все таки кинули одну строку на другую
                            gridEx.inDragDrop = false;
                        }
                    }
                }
                finally
                {
                    gridEx.ugData.EndUpdate();
                }
            }
		}


		#endregion Drag&Drop
	}
}