using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinMaskedEdit;
using System.ComponentModel;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace Krista.FM.Client.Components
{
    public partial class UltraGridEx
    {
        #region поиск текста в гриде

        Dictionary<string, string> searchColumns;
        private UltraGridTextSearcher ultraGridTextSearcher;
        
        /// <summary>
        /// поиск текста в гриде. Отображение панельки с параметрами поиска
        /// </summary>
        private void SearchText()
        {
            ultraDockManager1.HostControl = ParentFormEx;
            searchColumns = new Dictionary<string, string>();
            searchColumns.Clear();
            foreach (UltraGridColumn column in ugData.DisplayLayout.Bands[0].Columns)
            {
                if (!column.Hidden && !string.IsNullOrEmpty(column.Header.Caption))
                    if (!searchColumns.ContainsKey(column.Header.Caption))
                    searchColumns.Add(column.Header.Caption, column.Key);
            }
            textSearcher1.ShowSearch(this, searchColumns);
            // установим родителя для док менеджера. для того, что бы не было отдельной иконки при переключении задач
            if (!string.IsNullOrEmpty(Caption))
                ultraDockManager1.ControlPanes[0].Text = Caption;
            ultraDockManager1.ControlPanes[0].Show();
            ultraDockManager1.ControlPanes[0].Activate();
        }

        private Form _parentForm;
        /// <summary>
        /// родительская форма для панельки с параметрами поиска
        /// </summary>
        internal Form ParentFormEx
        {
            get
            {
                if (_parentForm == null)
                {
                    _parentForm = ParentForm;
                    while (_parentForm.ParentForm != null)
                    {
                        _parentForm = _parentForm.ParentForm;
                    }
                }
                return _parentForm;
            }
        }

        SearchInfo _searchInfo;
        /// <summary>
        /// параметры поиска
        /// </summary>
        public SearchInfo SearchInfo
        {
            get
            {
                if (_searchInfo == null)
                    _searchInfo = new SearchInfo();

                return _searchInfo;
            }
        }

        /// <summary>
        /// Запуск поиска
        /// </summary>
        internal void Search()
        {
            if (ultraGridTextSearcher == null)
                ultraGridTextSearcher = new GroupUltraGridTextSearcher(ugData, SearchInfo);

            ultraGridTextSearcher.Search();
        }

        #endregion

        #region действия по горячим клавишам

        /// <summary>
        /// нажатия кнопок на клавиатуре дублируют некоторые функции тулбара
        /// и вообще некоторые функции по нажатию клавиш или их комбинаций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ugData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.Control && e.Shift)
            {
                InDebugMode = !InDebugMode;
                return;
            }

            if (!e.Alt && !e.Control && !e.Shift && e.KeyCode == Keys.Enter)
                EnterEditMode();

            if (e.Control && e.KeyCode == Keys.X)
            {
                if (!InDebugMode)
                    return;
                eventViewer.ClearEvents();
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.F5)
            {
                RefreshData();
            }

            // очищаем данные
            if (e.KeyCode == Keys.D && e.Control)
            {
                ClearCurrentData();
            }

            if (e.KeyCode == Keys.S && e.Control)
            {
                SaveChanges();
            }
            // добавляем новую запись на тот же уровень, что и текущая активная...
            if (e.KeyCode == Keys.Insert && !e.Alt && !e.Control && !e.Shift)
            {
                InsertRow();
                e.Handled = true;
            }

            // копируем полностью активную строку для дальнейшей вставки этой записи
            if ((e.KeyCode == Keys.Insert || e.KeyCode == Keys.C) && e.Control)
            {
                if (ugData.ActiveCell != null)
                    if (ugData.ActiveCell.IsInEditMode)
                        return;

                bool allowCopy = utmMain.Tools["CopyRow"].SharedProps.Enabled && utmMain.Tools["CopyRow"].SharedProps.Visible;
                if (allowCopy)
                    CopyRow();
            }

            // вставляем скопированную или вырезаную запись...
            if ((e.KeyCode == Keys.Insert && e.Shift) || (e.KeyCode == Keys.V && e.Control))
            {
                if (ugData.ActiveCell != null)
                    if (ugData.ActiveCell.IsInEditMode)
                        return;

                bool allowPaste = utmMain.Tools["PasteRow"].SharedProps.Enabled && utmMain.Tools["PasteRow"].SharedProps.Visible;
                if (allowPaste)
                    PasteRow();
            }

            if (e.KeyCode == Keys.Left && e.Alt)
            {
                if (ugData.ActiveRow != null)
                    if (ugData.ActiveRow.ParentRow != null)
                    {
                        ugData.ActiveRow.ParentRow.Selected = true;
                        ugData.ActiveRow.Selected = false;
                        ugData.ActiveRow.ParentRow.Activate();
                    }
            }

            if (e.KeyCode == Keys.Right && e.Alt)
            {
                if (ugData.ActiveRow != null)
                    if (ugData.ActiveRow.ChildBands != null)
                        if (ugData.ActiveRow.ChildBands[0].Rows.Count > 0)
                        {
                            ugData.ActiveRow.Selected = false;
                            ugData.ActiveRow.ChildBands[0].Rows[0].Selected = true;
                            ugData.ActiveRow.ChildBands[0].Rows[0].Activate();
                        }
            }

            if (e.KeyCode == Keys.Enter && e.Control)
            {
                if (ugData.ActiveCell != null)
                    if (ugData.ActiveCell.Column.Style == Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton)
                    {
                        if (_OnClickCellButton != null)
                        {
                            CellEventArgs arg = new CellEventArgs(ugData.ActiveCell);
                            _OnClickCellButton(ugData, arg);
                        }
                    }
                    else if (ugData.ActiveCell.ValueListResolved != null)
                    {
                        ugData.PerformAction(UltraGridAction.EnterEditMode);
                        Rectangle rect = ugData.ActiveCell.GetUIElement().Rect;
                        ugData.ActiveCell.Column.ValueList.DropDown(ugData.RectangleToScreen(rect),
                            ugData.ActiveCell.ValueListResolved.ItemCount - 1, string.Empty);
                    }
                    else if (ugData.ActiveCell.Column.Editor is EditorWithMask)
                    {
                        ugData.PerformAction(UltraGridAction.EnterEditMode);
                        if (((EditorWithMask)ugData.ActiveCell.Column.Editor).ButtonsRight.Count > 0)
                        {
                            DropDownEditorButton button = (DropDownEditorButton)((EditorWithMask)ugData.ActiveCell.Column.Editor).ButtonsRight[0];
                            button.DropDown();
                        }
                    }
            }
            // добавление подчиненной записи 
            if (e.Alt && e.KeyCode == Keys.Insert && !e.Control && !e.Shift)
            {
                InsertChildRow();
                e.Handled = true;
            }

            if (e.Control && e.KeyCode == Keys.W)
            {
                if (this.ugData.ActiveRow != null)
                {
                    if (this.ugData.ActiveCell == null)
                    {
                        UltraGridCell cell = FindFirstCellInRow(this.ugData.ActiveRow);
                        cell.Selected = true;
                        cell.Activate();
                        this.ugData.Selected.Rows.Clear();
                    }
                    else
                    {
                        this.ugData.ActiveCell = null;
                        this.ugData.Selected.Cells.Clear();
                        this.ugData.ActiveRow.Selected = true;
                    }
                }

            }

            if (e.KeyCode == Keys.E && e.Control)
            {
                this.ugData.PerformAction(UltraGridAction.ToggleEditMode);
            }
        }

        private UltraGridCell FindFirstCellInRow(UltraGridRow row)
        {
            foreach (UltraGridCell cell in row.Cells)
            {
                if (cell.Column.Header.VisiblePosition == 0)
                    return cell;
            }
            return null;
        }

        private void umeLookupValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        ((UltraMaskedEdit)sender).PerformAction(MaskedEditAction.PrevCharacter, false, false);
                        ((UltraMaskedEdit)sender).PerformAction(MaskedEditAction.SetPivot, false, false);
                        break;
                    case Keys.Down:
                        ((UltraMaskedEdit)sender).PerformAction(MaskedEditAction.NextCharacter, false, false);
                        ((UltraMaskedEdit)sender).PerformAction(MaskedEditAction.SetPivot, false, false);
                        break;
                }
                e.Handled = true;
            }
        }

        private void umeLookupValue_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            UltraMaskedEdit ume = (UltraMaskedEdit)sender;
            ume.ShowInkButton = ShowInkButton.Never;

            #region дублирование некоторых опций по нажатию определенных комбинаций кнопок

            // обновление данных
            if (e.KeyCode == Keys.F5)
            {
                CloseLookupEditor();
                RefreshData();
            }

            // удаление всех данных
            if (e.KeyCode == Keys.D && e.Control)
            {
                CloseLookupEditor();
                ClearCurrentData();
            }

            // сохранение измнений
            if (e.KeyCode == Keys.S && e.Control)
            {
                CloseLookupEditor();
                SaveChanges();
            }

            // выход из режима редактирования
            if (!(e.Alt && e.Control && e.Shift) && e.KeyCode == Keys.Escape)
                CloseLookupEditor();

            #endregion

            if (e.KeyCode == Keys.Tab && !e.Shift)
            {
                e.IsInputKey = true;
                CloseLookupEditor();
                this.ugData.PerformAction(UltraGridAction.NextCellByTab);
            }

            if (e.KeyCode == Keys.Enter && e.Control)
            {
                if (this.ugData.ActiveCell != null)
                    if (_OnClickCellButton != null)
                    {
                        CellEventArgs arg = new CellEventArgs(this.ugData.ActiveCell);
                        _ugData_ClickCellButton(this.ugData, arg);
                    }
            }

            if (e.Shift && e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
                CloseLookupEditor();
                ugData.PerformAction(UltraGridAction.PrevCellByTab);
            }

            if (e.Control && e.KeyCode == Keys.E)
                CloseLookupEditor();

            if (e.KeyCode == Keys.Insert && !e.Alt && !e.Control && !e.Shift)
            {
                InsertRow();
            }

            if (e.Alt && e.KeyCode == Keys.Insert && !e.Control && !e.Shift)
            {
                InsertChildRow();
            }
        }

        #endregion

        private void _ugData_BeforeCustomRowFilterDialog(object sender, BeforeCustomRowFilterDialogEventArgs e)
        {
            if ((e.CustomRowFiltersDialog != null) && (e.CustomRowFiltersDialog.Grid != null))
            {
                UltraGridHelper.CustomizeRowFilerDialog(e.CustomRowFiltersDialog, e.Column.Header.Caption);
            }

            e.CustomRowFiltersDialog.Grid.BeforeCellListDropDown += new CancelableCellEventHandler(Grid_BeforeCellListDropDown);
        }

        void Grid_BeforeCellListDropDown(object sender, CancelableCellEventArgs e)
        {
            // настройка перевода фильтра для булевских колонок
            if (e.Cell.Column.DataType != typeof(bool) && e.Cell.Column.Style != Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox)
                return;

            ValueList list = e.Cell.ValueListResolved as ValueList;
            if (list == null)
                return;
            foreach (ValueListItem val in list.ValueListItems)
            {
                if (val.DataValue is bool)
                {
                    if (Convert.ToBoolean(val.DataValue) == true)
                        val.DisplayText = "Да";
                    if (Convert.ToBoolean(val.DataValue) == false)
                        val.DisplayText = "Нет";
                }
                else if (val.DisplayText == "0" || val.DisplayText == "1")
                {
                    if (Convert.ToInt32(val.DataValue) == 1)
                        val.DisplayText = "Да";
                    if (Convert.ToInt32(val.DataValue) == 0)
                        val.DisplayText = "Нет";
                }
            }
        }

        private void _ugData_BeforeRowFilterDropDown(object sender, BeforeRowFilterDropDownEventArgs e)
        {
            if (e.ValueList == null)
                return;

            if (e.Column.DataType != typeof(bool) && e.Column.Style != Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox)
                return;

            foreach (ValueListItem val in e.ValueList.ValueListItems)
            {
                if (val.DataValue is bool)
                {
                    if (Convert.ToBoolean(val.DataValue) == true)
                        val.DisplayText = "Да";
                    if (Convert.ToBoolean(val.DataValue) == false)
                        val.DisplayText = "Нет";
                }
                else if (val.DisplayText == "0" || val.DisplayText == "1")
                {
                    if (Convert.ToInt32(val.DataValue) == 1)
                        val.DisplayText = "Да";
                    if (Convert.ToInt32(val.DataValue) == 0)
                        val.DisplayText = "Нет";
                }
            }
        }

        public bool inDragDrop = false;
        public enum ScrollDirections { up, down, unknown };
        public ScrollDirections scrollDirection = ScrollDirections.unknown;
        [DebuggerStepThrough()]
        private void timer_Tick(object sender, EventArgs e)
        {
            if (inDragDrop)
            {
                // если как то определить, что все таки вышли за пределы основной области грида, то скролировать
                switch (scrollDirection)
                {
                    case ScrollDirections.up:
                        ugData.ActiveRowScrollRegion.Scroll(RowScrollAction.LineUp);
                        break;
                    case ScrollDirections.down:
                        ugData.ActiveRowScrollRegion.Scroll(RowScrollAction.LineDown);
                        break;
                }
            }
        }
    }
}
