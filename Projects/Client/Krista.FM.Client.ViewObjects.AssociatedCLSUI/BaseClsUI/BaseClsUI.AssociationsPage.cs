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
                vo.ugAssociations.DisplayLayout.Bands[0].Columns[1].Header.Caption = "��������� ��������������";
                vo.ugAssociations.DisplayLayout.Bands[0].Columns[2].Header.Caption = "�������������� �������������";
                vo.ugAssociations.DisplayLayout.Bands[0].Columns[3].Header.Caption = "��������� �������������";
                vo.ugAssociations.DisplayLayout.Bands[0].Columns[4].Header.Caption = "������������ �������������";
                vo.ugAssociations.DisplayLayout.Bands[0].Columns["AssociatedRecCnt"].Header.Caption = "����� �������";
                vo.ugAssociations.DisplayLayout.Bands[0].Columns["UnassociatedrecCnt"].Header.Caption = "�� ������������ �������";
            }
        }

		private void utcDataCls_ActiveTabChanged(object sender, Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventArgs e)
        {
            // ������� �� ��������� � ���������������
            AssociationPageLoad(e.Tab);
        }

        private IAssociation GetAssociation(string associationName)
        {
            if (this.Workplace.ActiveScheme.Associations.ContainsKey(associationName))
                return (IAssociation)this.Workplace.ActiveScheme.Associations[associationName];
            return null;
        }

        /// <summary>
        /// ��������� ���� ������� �� �������������� �������� ��������������
        /// </summary>
		public void InitAssociationPage()
		{
			// ��������� ������ ���������� � ������� ������ �����
            if (associationDataTable == null)
                associationDataTable = GetAssociationsTable();
            else if (associationDataTable.Rows.Count == 0)
                associationDataTable = GetAssociationsTable();

            //vo.ugAssociations.DataSource = null;
            UltraGridColumn clmnAllRecords = null;
            UltraGridColumn clmnUnassociateRecords = null;
            if (!vo.ugAssociations.DisplayLayout.Bands[0].Columns.Exists("AssociatedRecCnt"))
                clmnAllRecords = vo.ugAssociations.DisplayLayout.Bands[0].Columns.Add("AssociatedRecCnt", "����� �������");
            else
                clmnAllRecords = vo.ugAssociations.DisplayLayout.Bands[0].Columns["AssociatedRecCnt"];

            if (!vo.ugAssociations.DisplayLayout.Bands[0].Columns.Exists("UnassociatedrecCnt"))
                clmnUnassociateRecords = vo.ugAssociations.DisplayLayout.Bands[0].Columns.Add("UnassociatedrecCnt", "�� ������������ �������");
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
			Workplace.SwitchTo("�������������� � �������", typeof(Navigation.AssociationNavigationListUI).FullName, e.Cell.Row.Cells[0].Value);
        }

        void ugAssociations_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["GoToAssociation"].ToolTipText = "������� �� ��������� �������������";

            IBridgeAssociation ass = (IBridgeAssociation)GetAssociation(e.Row.Cells[0].Value.ToString());
            if (clsClassType == ClassTypes.clsDataClassifier)
            {
                e.Row.Cells["AssociatedRecCnt"].Value = string.Format("{0} (�� ���� ���������� {1})",
                    ass.GetRecordsCountByCurrentDataSource(CurrentDataSourceID), ((IBridgeAssociation)ass).GetAllRecordsCount());
                e.Row.Cells["UnassociatedrecCnt"].Value = string.Format("{0} (�� ���� ���������� {1})",
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

		#region ���������� Drag&Drop
		private UltraGridRow LastEnteredRow;

		/// <summary>
		/// �������� UltraGridRow �� UIElement. ������������ � �������.
		/// </summary>
		/// <param name="elem">�������</param>
		/// <returns>������</returns>
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
		/// �������� UltraGridRow �� �������� �����������. ������������ � �������.
		/// </summary>
		/// <param name="X">���������� X (��������)</param>
		/// <param name="Y">���������� Y (��������)</param>
		/// <returns></returns>
		private UltraGridRow GetRowFromPos(int X, int Y)
		{ 
			Point pt = new Point(X, Y);
			pt = vo.ugeCls.ugData.PointToClient(pt);
			UIElement elem = vo.ugeCls.ugData.DisplayLayout.UIElement.ElementFromPoint(pt);
			return GetRowFromElement(elem);
		}

		/// <summary>
		/// �������� ������ (��� ������� � ���������� ���������)
		/// </summary>
		/// <param name="row">������</param>
		/// <param name="colored">������� ������������� ���������</param>
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
		/// ��������� �������� ������������ �������������� �� ������.
		/// ��������.
		/// ������ ���������� DragDropEffects.None ���� ����� ����� ���������������
		/// ��� �������� �������� (�� ��������� ����������� ������)
		/// </summary>
		/// <param name="row">������-��������</param>
		/// <returns>���������� ��������</returns>
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
		///  ��������� �������� ������������ �������������� �� ������. 
		///  ���������� ��������, ������� ������ ������������� ��� ��������������� �������
		/// </summary>
		/// <param name="dropRow">������, ��� ������� ����� ��� ��������������</param>
		/// <returns>��� �������� ��� �������</returns>
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
            // ���� ��� ���� �� ��������� ��������, �� �������
            if (!allowChangeHierarchy) return;
			// ��� ���������� �������� ���� ���������
			// ��� �������� ������������� � ������������� ��������������� Drag&Drop ����������
		    UltraGridEx gridEx = sender as UltraGridEx;
            if (gridEx == null)
                return;
            if ((gridEx.ugData.DisplayLayout.ViewStyle == ViewStyle.SingleBand) || (clsClassType == ClassTypes.clsFixedClassifier))
				return;
            // ��� �� ��������� ��� �������������� ���������������
            if (gridEx.HierarchyInfo.ParentClmnName == string.Empty || gridEx.HierarchyInfo.ParentRefClmnName == string.Empty)
                return;
            gridEx.inDragDrop = true;
			try
			{
				if (!InInplaceMode)
				{
					// �������� �������� ������
                    selectedRows = new List<UltraGridRow>();
                    foreach(UltraGridRow selectRow in gridEx.ugData.Selected.Rows)
                        selectedRows.Add(selectRow);
                    UltraGridHelper.GetSelectedIDs(gridEx.ugData, out selectedRowsID);
					// �������� Drag&Drop
                    gridEx.ugData.DoDragDrop(vo.ugeCls.ugData.Selected.Rows, DragDropEffects.Move);
					e.Cancel = true;
				}
			}
			finally
			{
				// ���������� ��� � ���������� ���������, �������� ���������
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
			// �������� ������ ��� ������� � ������ ������ ���������
			UltraGridRow tmpRow = GetRowFromPos(e.X, e.Y);

            if (!InInplaceMode)
            {
                if (e.Data.GetDataPresent("FileDrop"))
                    e.Effect = DragDropEffects.Copy;
                else
                {
                    // ���� ��� �� �������� ��������������� � ���������� � ������ ������
                    if ((tmpRow != LastEnteredRow) && (!selectedRows.Contains(tmpRow)))
                    {
                        // ������� ��������� � ���������� ����������
                        ColoredRow(LastEnteredRow, false);
                        // ���������� ������
                        LastEnteredRow = tmpRow;
                        // ��������
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
                    // ��������� ������������ ��������������
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
                    // ���� ��� ���������� 
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        dropFiles.AddRange(Directory.GetFiles(file));
                    else
                        dropFiles.Add(file);
                }
                // ��������� ����� � ������
                Workplace.OperationObj.Text = String.Format("���������� ����a: {0}", "...");
                Workplace.OperationObj.StartOperation();
                try
                {
                    foreach (string fileName in dropFiles)
                    {
                        // ���������� ������ � ����������...
                        // ���� ���������� � ������ ���������, �������� ��������� � ������
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
                        // �������� ������� ��� ��������, �� ����� ��������� ��� ��������������� ������
                        UltraGridRow row = GetRowFromPos(e.X, e.Y);
                        // ���� ������ �� ���� ���� - ������ �� ������
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
                                    if (MessageBox.Show("�� ������������� ������ ��������� ������ ������� �������� ���������?", "��������� ��������", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        // ���� ������ ������ � "�������", �� ��� ����� ��������
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
                                // ���� ������ �� ������ ������ - ������ �� ������������
                                if (!InInplaceMode)
                                    if (MessageBox.Show("�� ������������� ������ ��������� ������������ ������?", "��������� ��������", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
                            // �� ��� ���� ������ ���� ������ �� ������
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