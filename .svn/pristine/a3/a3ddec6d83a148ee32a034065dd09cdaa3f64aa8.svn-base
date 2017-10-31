using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinMaskedEdit;
using System.ComponentModel;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;
using System.Windows.Forms;

namespace Krista.FM.Client.Components
{
    public partial class UltraGridEx
    {
        private Infragistics.Win.ToolTip toolTipValue = null;
        public Infragistics.Win.ToolTip ToolTip
        {
            get
            {
                if (null == this.toolTipValue)
                {
                    this.toolTipValue = new Infragistics.Win.ToolTip(this);
                    this.toolTipValue.DisplayShadow = true;
                    this.toolTipValue.AutoPopDelay = 0;
                    this.toolTipValue.InitialDelay = 0;
                }
                return this.toolTipValue;
            }
        }

        private UltraGridCell lastActiveCellWithLookupEditor = null;

        private static EditAsType DataTypeToEditAsValue(Type sourceType)
        {
            switch (sourceType.FullName)
            {
                case "System.Int64":
                case "System.UInt64":
                    return EditAsType.Long;
                case "System.Byte":
                case "System.SByte":
                case "System.Int32":
                case "System.UInt32":
                case "System.Int16":
                case "System.UInt16":
                    return EditAsType.Integer;
                case "System.Double":
                    return EditAsType.Double;
                case "System.Decimal":
                    return EditAsType.Currency;
                case "System.DateTime":
                    return EditAsType.DateTime;
                default:
                    return EditAsType.String;
            }

        }

        public void _ugData_BeforeEnterEditMode(object sender, CancelEventArgs e)
        {
            #region Эмуляция редактора ячейки (для лукапов)
            // нужно ли проводить действия специфические для лукапов
            if (!LookupsEventsDefined())
                return;

            UltraGrid ug = (UltraGrid)sender;
            if (ug.ActiveCell == null)
                return;

            // редактируется ли колонка?
            UltraGridCell cell = ug.ActiveCell;
            UltraGridColumn clmn = cell.Column;
            // проверки возможности редактирования ячейки
            //#warning Проверить достаточность этого условия (определения возможности редактирования ячейки)
            if (!(cell.Row.Activation == Activation.AllowEdit))
                return;
            if (!(cell.Column.CellActivation == Activation.AllowEdit))
                return;

            // колонка - лукап?
            if (!LookupColumnsNames.Contains(clmn.Key))
                return;

            // начинаем манипулировать со встроенным редактором
            CellUIElement objCellUIElement = (CellUIElement)cell.GetUIElement(ug.ActiveRowScrollRegion, ug.ActiveColScrollRegion);
            if (objCellUIElement == null) { return; }

            int left = objCellUIElement.RectInsideBorders.Location.X + ug.Location.X;
            int top = objCellUIElement.RectInsideBorders.Location.Y + ug.Location.Y;
            int width = objCellUIElement.RectInsideBorders.Width;
            int height = objCellUIElement.RectInsideBorders.Height;

            // настраиваем редактор
            // ..сбрасываем настройки
            umeLookupValue.Reset();
            // .. начинаем косить под редактор ячейки
            EditorButton btn = (EditorButton)umeLookupValue.ButtonsRight[0];
            btn.Text = String.Empty;
            umeLookupValue.BorderStyle = UIElementBorderStyle.None;
            umeLookupValue.AutoSize = false;
            UltraGridHelper.SetLikelyEditButtonStyleAppearance(btn.Appearance);
            btn.Appearance.Image = ilSmall.Images[0];
            //#warning После отладки лукапов убрать: меняем цвет редактора на светло-зеленый
            //umeLookupValue.Appearance.BackColor = System.Drawing.Color.LightGreen;
            umeLookupValue.Appearance.BackColor = cell.Column.CellAppearance.BackColor;

            string sourceColumnName = GetSourceColumnName(clmn.Key);
            // маска
            if (gridColumnsStates != null)
            {
                GridColumnState gcs = GridColumnsStates[sourceColumnName];
                if (!String.IsNullOrEmpty(gcs.Mask))
                {
                    #warning проверить поведение редактора на сложных масках (которые в гриде дополняются пробелами)
                    umeLookupValue.DisplayMode = MaskMode.IncludeLiteralsWithPadding;
                    umeLookupValue.EditAs = EditAsType.UseSpecifiedMask;
                    umeLookupValue.InputMask = gcs.Mask;
                }
                else
                {
                    umeLookupValue.DisplayMode = MaskMode.Raw;
                    umeLookupValue.EditAs = DataTypeToEditAsValue(clmn.Band.Columns[sourceColumnName].DataType);
                }
            }

            umeLookupValue.SetBounds(left, top, width, height, BoundsSpecified.All);

            umeLookupValue.Value = cell.Row.Cells[sourceColumnName].Value;

            // пытаемся получить параметры кнопки ячейки
            UIElement cellBtnUIElem = objCellUIElement.GetDescendant(typeof(EditButtonUIElement));
            if (cellBtnUIElem != null)
                // .. и установить ей ширину
                btn.Width = cellBtnUIElem.Rect.Width;

            // показываем редактор
            umeLookupValue.Visible = true;
            umeLookupValue.Focus();
            umeLookupValue.BringToFront();

            // запоминаем ячейку для последующей синхронизации значений
            lastActiveCellWithLookupEditor = cell;

            e.Cancel = true;
            #endregion
        }

        private void CloseLookupEditor()
        {
            umeLookupValue_Leave(this.umeLookupValue, EventArgs.Empty);
        }

        private void umeLookupValue_Leave(object sender, EventArgs e)
        {
            UltraMaskedEdit ume = (UltraMaskedEdit)sender;
            ume.Visible = false;

            #warning Здесь должна быть проверка на правильность значения?
            if (lastActiveCellWithLookupEditor != null)
            {
                // запоминаем ячейку 
                UltraGridCell curCell = lastActiveCellWithLookupEditor;
                // сбрасываем текущую
                lastActiveCellWithLookupEditor = null;
                // если значение не менялось, обновлять строку не нужно, но т.к. значения редактора
                // и грида имеют разные типы, придется извратится
                string sourceColumnName = GetSourceColumnName(curCell.Column.Key);
                object oldValue = curCell.Row.Cells[sourceColumnName].Value;
                object newValue = ume.Value;
                // проверяем значения на пустоту
                bool oldValueIsNull = (oldValue == null) || (oldValue == DBNull.Value);
                bool newValueIsNull = (Convert.ToString(newValue) == String.Empty) || (newValue == null);
                // если оба значения пустые - выходим
                if (oldValueIsNull && newValueIsNull)
                    return;
                // если новое пустое, а старое нет - присваимваем старому DBNull.Value
                if (newValueIsNull && (!oldValueIsNull))
                {
                    curCell.Row.Cells[sourceColumnName].Value = DBNull.Value;
                    return;
                }
                // если новое не пустое, а старое пустое - присваиваем старому с приведением типа
                newValue = Convert.ChangeType(newValue, curCell.Row.Band.Columns[sourceColumnName].DataType);
                if (!newValueIsNull && oldValueIsNull)
                {
                    curCell.Row.Cells[sourceColumnName].Value = newValue;
                    return;
                }
                // если оба не пустые  - приводим тип нового значения к старому
                // пытаемся сравнить и меняем значение в случае несовпадения
                if (((IComparable)oldValue).CompareTo(newValue) != 0)
                    curCell.Row.Cells[sourceColumnName].Value = newValue;
            }
        }
    }
}
