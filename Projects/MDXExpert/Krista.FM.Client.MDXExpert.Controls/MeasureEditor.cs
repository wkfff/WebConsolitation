using System;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class MeasureEditor : UserControl
    {
        private PivotTotal _editingMeasure;
        /// <summary>
        /// Признак что обработчики выполнять не надо
        /// </summary>
        private bool isMayHook = false;

        //события
        private EventHandler _exitEditMode = null;

        public MeasureEditor()
        {
            InitializeComponent();

            this.BackColorChanged += new EventHandler(MeasureEditor_BackColorChanged);
            this.ucbFormatType.ValueChanged += new EventHandler(ucbFormatType_ValueChanged);
            this.ucbUnitDisplayType.ValueChanged += new EventHandler(ucbUnitDisplayType_ValueChanged);
            this.ucbValueAlignment.ValueChanged += new EventHandler(ucbValueAlignment_ValueChanged);
            this.uchThousandDelemiter.CheckedChanged += new EventHandler(uchThousandDelemiter_CheckedChanged);
            this.uteDigitCount.KeyDown += new KeyEventHandler(uteDigitCount_KeyDown);
            this.uteDigitCount.AfterExitEditMode += new EventHandler(uteDigitCount_AfterExitEditMode);
            this.uteDigitCount.EditorSpinButtonClick += new Infragistics.Win.UltraWinEditors.SpinButtonClickEventHandler(uteDigitCount_EditorSpinButtonClick);
        }

        /// <summary>
        /// Тип
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ucbFormatType_ValueChanged(object sender, EventArgs e)
        {
            if (!this.IsExistEditingMeasure || this.isMayHook)
                return;

            this.EditingMeasure.Format.FormatType = (FormatType)this.ucbFormatType.SelectedIndex;
            this.SetControlState();
            this.OnExitEditMode(sender, e);
        }

        /// <summary>
        /// У меры некоторые свойства доступны/не доступны только при опеределенных типах формата
        /// </summary>
        private void SetControlState()
        {
            if (!this.IsExistEditingMeasure)
                return;

            ValueFormat measureFormat = this.EditingMeasure.Format;

            this.ulDigitCount.Enabled = measureFormat.IsDigitCountEnable;
            this.uteDigitCount.Enabled = measureFormat.IsDigitCountEnable;

            this.ulUnitDisplayType.Enabled = measureFormat.IsUnitDisplayTypeEnable;
            this.ucbUnitDisplayType.Enabled = measureFormat.IsUnitDisplayTypeEnable;

            this.uchThousandDelemiter.Enabled = measureFormat.IsThousandDelimiterEnable;
        }

        /// <summary>
        /// Место отображение едениц измерения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ucbUnitDisplayType_ValueChanged(object sender, EventArgs e)
        {
            if (!this.IsExistEditingMeasure || this.isMayHook)
                return;

            this.EditingMeasure.Format.UnitDisplayType = (UnitDisplayType)this.ucbUnitDisplayType.SelectedIndex;
            this.OnExitEditMode(sender, e);
        }

        /// <summary>
        /// Выравнивание
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ucbValueAlignment_ValueChanged(object sender, EventArgs e)
        {
            if (!this.IsExistEditingMeasure || this.isMayHook)
                return;

            this.EditingMeasure.Format.ValueAlignment = (StringAlignment)this.ucbValueAlignment.SelectedIndex;
            this.OnExitEditMode(sender, e);
        }

        /// <summary>
        /// Показывать разделитель разрядов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uchThousandDelemiter_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.IsExistEditingMeasure || this.isMayHook)
                return;

            this.EditingMeasure.Format.ThousandDelimiter = this.uchThousandDelemiter.Checked;
            this.OnExitEditMode(sender, e);
        }

        /// <summary>
        /// Количество чисел после запятой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uteDigitCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (!this.IsExistEditingMeasure || this.isMayHook)
                    return;

                string value;
                if (CommonUtils.IsValidIntValue(this.uteDigitCount.Text,
                    this.EditingMeasure.Format.DigitCount, 0, 20, false,
                    "MDXExpert-PropertyGrid-DigitCount.", out value))
                {
                    //Если значение удовлятворяет всем условиям, присваиваем его объектной модели, и
                    //обновляем PropertyGrid
                    this.EditingMeasure.Format.DigitCount = byte.Parse(value);
                    this.OnExitEditMode(sender, e);
                }
                this.uteDigitCount.Text = value;
            }
        }

        /// <summary>
        /// Количество чисел после запятой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void uteDigitCount_AfterExitEditMode(object sender, EventArgs e)
        {
            if (!this.IsExistEditingMeasure || this.isMayHook)
                return;

            string value;
            if (CommonUtils.IsValidIntValue(this.uteDigitCount.Text,
                this.EditingMeasure.Format.DigitCount, 0, 20, true,
                "MDXExpert-PropertyGrid-DigitCount.", out value))
            {
                //Если значение удовлятворяет всем условиям, присваиваем его объектной модели, и
                //обновляем PropertyGrid
                this.EditingMeasure.Format.DigitCount = byte.Parse(value);
                this.OnExitEditMode(sender, e);
            }
            this.uteDigitCount.Text = value;
        }

        void uteDigitCount_EditorSpinButtonClick(object sender, Infragistics.Win.UltraWinEditors.SpinButtonClickEventArgs e)
        {
            byte value;
            if (byte.TryParse(this.uteDigitCount.Text, out value))
            {
                if (e.ButtonType == Infragistics.Win.UltraWinEditors.SpinButtonItem.NextItem)
                {
                    if (value < 20)
                        value++;
                }
                else
                {
                    if (value > 0)
                        value--;
                }
                this.EditingMeasure.Format.DigitCount = value;
                this.uteDigitCount.Text = value.ToString();
                this.OnExitEditMode(sender, e);
            }
        }

        /// <summary>
        /// При цветовой схеме "#5" (серая) чек боксы не приводятся к этому стилю, и становятся белыми,
        /// будем красить их в ручную
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MeasureEditor_BackColorChanged(object sender, EventArgs e)
        {
            Color silver = Color.FromArgb(255, 237, 237, 237);
            if (this.BackColor == silver)
            {
                this.uchThousandDelemiter.UseAppStyling = false;
            }
            else
            {
                this.uchThousandDelemiter.UseAppStyling = true;
            }
            this.uchThousandDelemiter.BackColor = this.BackColor;
        }

        /// <summary>
        /// Синхронизируем значения в контроле со значениями PivotData
        /// </summary>
        public void RefreshValues()
        {
            if (this.IsExistEditingMeasure)
            {
                try
                {
                    this.isMayHook = true;

                    this.ucbFormatType.SelectedIndex = (int)this.EditingMeasure.Format.FormatType;
                    this.ucbValueAlignment.SelectedIndex = (int)this.EditingMeasure.Format.ValueAlignment;
                    this.ucbUnitDisplayType.SelectedIndex = (int)this.EditingMeasure.Format.UnitDisplayType;
                    this.uchThousandDelemiter.Checked = this.EditingMeasure.Format.ThousandDelimiter;
                    this.uteDigitCount.Text = this.EditingMeasure.Format.DigitCount.ToString();

                    this.SetControlState();
                }
                finally
                {
                    this.isMayHook = false;
                }
            }
        }

        private void OnExitEditMode(object sender, EventArgs e)
        {
            if (_exitEditMode != null)
            {
                _exitEditMode(sender, e);
            }
        }

        /// <summary>
        /// Редактируемая мера
        /// </summary>
        public PivotTotal EditingMeasure
        {
            get { return _editingMeasure; }
            set 
            { 
                _editingMeasure = value;
                this.RefreshValues();
            }
        }

        /// <summary>
        /// Существует ли редактируемая мера
        /// </summary>
        public bool IsExistEditingMeasure
        {
            get { return this.EditingMeasure != null; }
        }

        /// <summary>
        /// Событие происходи при завершении редактирования
        /// </summary>
        public event EventHandler ExitEditMode
        {
            add { _exitEditMode += value; }
            remove { _exitEditMode -= value; }
        }
    }
}
