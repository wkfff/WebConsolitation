using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Infragistics.Win;
using System.Diagnostics;
using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.Components
{
    public partial class UltraGridEx
    {
        private int[] maxHeight;

        public void AfterCreateChildElements(Infragistics.Win.UIElement parent)
        {
            if (_createUIElement != null)
                _createUIElement(this, parent);
            else
                if (parent is HeaderUIElement)
                {
                    Infragistics.Win.UltraWinGrid.HeaderBase aHeader = ((HeaderUIElement)parent).Header;

                    if (aHeader.Column == null)
                        return;
                    string caption = aHeader.Column.Header.Caption;
                    if (caption != string.Empty)
                    {
                        Infragistics.Win.CheckBoxUIElement cbWrapWordsUIElement = null;
                        if (aHeader.Column.DataType == typeof(String) || (aHeader.Column.Tag != null && String.Equals(aHeader.Column.Tag.ToString(), "NeedCheckBox")))
                        {
                            cbWrapWordsUIElement = (CheckBoxUIElement)parent.GetDescendant(typeof(ButtonUIElement));

                            if (cbWrapWordsUIElement == null)
                            {
                                // создаем чекбокс
                                cbWrapWordsUIElement = new CheckBoxUIElement(parent);
                            }

                            Infragistics.Win.UltraWinGrid.ColumnHeader aColumnHeader =
                                (Infragistics.Win.UltraWinGrid.ColumnHeader)cbWrapWordsUIElement.GetAncestor(typeof(HeaderUIElement))
                                .GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                            if (aColumnHeader.Tag == null)
                                //If the tag was nothing, this is probably the first time this 
                                //Header is being displayed, so default to Indeterminate
                                aColumnHeader.Tag = aHeader.Column.CellMultiLine == DefaultableBoolean.True
                                                    ? CheckState.Checked
                                                    : CheckState.Unchecked;
                            else
                                cbWrapWordsUIElement.CheckState = (CheckState)aColumnHeader.Tag;

                            cbWrapWordsUIElement.ElementClick += new UIElementEventHandler(aButtonUIElement_ElementClick);

                            parent.ChildElements.Add(cbWrapWordsUIElement);

                            // передвигаем все элементы в элементе заголовка на ширину чекбокса
                            cbWrapWordsUIElement.Rect = new Rectangle(parent.Rect.X + 3, parent.Rect.Y + ((parent.Rect.Height - cbWrapWordsUIElement.CheckSize.Height) / 2), cbWrapWordsUIElement.CheckSize.Width, cbWrapWordsUIElement.CheckSize.Height);
                        }
                        TextUIElement aTextUIElement = (TextUIElement)parent.GetDescendant(typeof(TextUIElement));

                        // если нету текстового заголовка, то выходим
                        if (aTextUIElement == null)
                            return;
                        // получаем элемент для отображения сортировки
                        SortIndicatorUIElement aSortIndicatorElement = (SortIndicatorUIElement)parent.GetDescendant(typeof(SortIndicatorUIElement));
                        // получаем элемент кнопки фильтра
                        FilterDropDownButtonUIElement aFilterUIElement = (FilterDropDownButtonUIElement)parent.GetDescendant(typeof(FilterDropDownButtonUIElement));


                        int textLengh = 0;
                        if (aSortIndicatorElement != null && cbWrapWordsUIElement != null)
                            textLengh = 36;
                        else if (aSortIndicatorElement == null && cbWrapWordsUIElement != null)
                            textLengh = 20;
                        else if (aSortIndicatorElement == null && cbWrapWordsUIElement == null)
                            textLengh = 20;
                        else if (aSortIndicatorElement != null && cbWrapWordsUIElement == null)
                            textLengh = 36;

                        // Push the TextUIElement to the right a little to make 
                        // room for the CheckBox. 3 pixels of padding are used again. 
                        if (cbWrapWordsUIElement != null)
                            aTextUIElement.Rect = new Rectangle(cbWrapWordsUIElement.Rect.Right + 3, aTextUIElement.Rect.Y, parent.Rect.Width - (cbWrapWordsUIElement.Rect.Right - parent.Rect.X) - textLengh, aTextUIElement.Rect.Height);
                        else
                            aTextUIElement.Rect = new Rectangle(parent.Rect.X + 3, aTextUIElement.Rect.Y, parent.Rect.Width - textLengh, aTextUIElement.Rect.Height);

                        #region херня по нормализации надписей в заголовках колонок грида

                        int fullHeight = GetStringHeight(caption, this._ugData.Font, aTextUIElement.Rect.Width) + 10;
                        float fontHeight = this._ugData.Font.GetHeight();
                        int linesCount = (int)(fullHeight / fontHeight);

                        UltraGridBand band = aHeader.Column.Band;
                        int bandIndex = band.Index;

                        if ((maxHeight[bandIndex] < linesCount))
                        {
                            maxHeight[bandIndex] = linesCount;

                            if (maxHeight[bandIndex] > 10)
                                maxHeight[bandIndex] = 10;

                            //UltraGridBand band = aHeader.Column.Band;
                            if (band.ColHeaderLines != maxHeight[bandIndex])
                            {
                                band.ColHeaderLines = maxHeight[bandIndex];
                                band.NotifyPropChange(Infragistics.Win.UltraWinGrid.PropertyIds.ColHeaderLines);
                            }
                        }
                        int minWidth = GetMinimalColWidth(caption, aHeader.Column.Width, aTextUIElement.Rect.Width, this._ugData.Font);
                        if (aHeader.Column.MinWidth != minWidth)
                            aHeader.Column.MinWidth = minWidth;

                        aTextUIElement.WrapText = true;

                        #endregion

                        if (aSortIndicatorElement != null)
                            aSortIndicatorElement.Rect = new Rectangle(aTextUIElement.Rect.Right + 3, aSortIndicatorElement.Rect.Y, 13, aSortIndicatorElement.Rect.Height);

                        if (aFilterUIElement != null)
                            if (aSortIndicatorElement != null)
                                aFilterUIElement.Rect = new Rectangle(aSortIndicatorElement.Rect.Right + 3, aFilterUIElement.Rect.Y, 13, aFilterUIElement.Rect.Height);
                            else
                                aFilterUIElement.Rect = new Rectangle(aTextUIElement.Rect.Right + 3, aFilterUIElement.Rect.Y, 13, aFilterUIElement.Rect.Height);

                        string columnName = aHeader.Column.Key;
                        int columnWidth = aHeader.Column.Width;
                        if (ServerFilterEnabled)
                            this.ugFilter.DisplayLayout.Bands[0].Columns[columnName].Width = columnWidth;
                    }
                }
        }

        /// <summary>
        /// получение минимальной ширины колонки исходя из расчета 10 строк заголовка
        /// </summary>
        /// <returns></returns>
        private int GetMinimalColWidth(string columnCaption, int columnWidth, int captionWidth, Font font)
        {
            int lineSymbolsCount = columnCaption.Length / 10;
            if ((columnCaption.Length % 10) != 0)
                lineSymbolsCount++;
            string tmpStr = string.Empty.PadLeft(lineSymbolsCount, 'm');
            int lineWidth = this.GetStringWidth(tmpStr, font);
            return columnWidth - captionWidth + lineWidth;
        }

        public int GetStringHeight(string measuredString, Font font, int rectangleWidth)
        {
            Graphics g = Graphics.FromHwnd(this.Handle);
            SizeF sizeF = g.MeasureString(measuredString, font, rectangleWidth);
            Size rect = Size.Round(sizeF);

            return rect.Height;
        }


        public int GetStringWidth(string measuredString, Font font)
        {
            Graphics g = Graphics.FromHwnd(this.Handle);
            SizeF sizeF = g.MeasureString(measuredString, font);
            Size rect = Size.Round(sizeF);
            return rect.Width;
        }


        /// <summary>
        /// выставляем параметры для колонки на перенос слов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void aButtonUIElement_ElementClick(object sender, UIElementEventArgs e)
        {
            CheckBoxUIElement checkBox = (CheckBoxUIElement)e.Element;

            Infragistics.Win.UltraWinGrid.HeaderBase header = ((HeaderUIElement)checkBox.Parent).Header;

            Infragistics.Win.UltraWinGrid.ColumnHeader aColumnHeader = (Infragistics.Win.UltraWinGrid.ColumnHeader)checkBox.GetAncestor(typeof(HeaderUIElement)).GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

            aColumnHeader.Tag = checkBox.CheckState;

            UltraGridColumn column = header.Column;
            string columnName = column.Key;

            if (column.DataType == typeof(string))
            {
            // ставим свойство на перенос слов на следующую строку
            if (checkBox.CheckState == CheckState.Checked)
            {
                column.CellMultiLine = DefaultableBoolean.True;
            }
            else
            {
                column.CellMultiLine = DefaultableBoolean.False;
            }
            if (_UIElementClick != null)
                _UIElementClick(sender, e);
            int columnWidth = column.Width;
            column.PerformAutoResize(PerformAutoSizeType.None);
            column.Width = columnWidth;
        }
        else if (column.DataType == typeof(bool))
        {
            bool check = checkBox.CheckState == CheckState.Checked;
            foreach (UltraGridRow row in this.ugData.Rows)
            {
                row.Cells[columnName].Value = check;
                row.Update();
            }
        }
        }

        [DebuggerStepThrough()]
        public bool BeforeCreateChildElements(Infragistics.Win.UIElement parent)
        {
            return false;
        }
    }
}
