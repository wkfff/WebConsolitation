using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinChart;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class Chart3DTransform : UserControl
    {
        private UltraChart _chart;

        private EventHandler _valueChanged = null;
        private EventHandler _exitEditMode = null;

        public Chart3DTransform()
        {
            InitializeComponent();
            this.tbPerspective.MouseUp += new MouseEventHandler(tbPerspective_MouseUp);
            this.tbScale.MouseUp += new MouseEventHandler(tbScale_MouseUp);
            this.tbX.MouseUp += new MouseEventHandler(tbX_MouseUp);
            this.tbY.MouseUp += new MouseEventHandler(tbY_MouseUp);
            this.VisibleChanged += new EventHandler(Chart3DTransform_VisibleChanged);
        }

        #region Обработчики событий
        void tbPerspective_MouseUp(object sender, MouseEventArgs e)
        {
            this.OnExitEditMode(sender, e);
        }

        void tbScale_MouseUp(object sender, MouseEventArgs e)
        {
            this.OnExitEditMode(sender, e);
        }

        void tbX_MouseUp(object sender, MouseEventArgs e)
        {
            this.OnExitEditMode(sender, e);
        }

        void tbY_MouseUp(object sender, MouseEventArgs e)
        {
            this.OnExitEditMode(sender, e);
        }

        void Chart3DTransform_VisibleChanged(object sender, EventArgs e)
        {
            //Если этого не сделаем, подсказки будут не видны
            this.toolTip.RemoveAll();
        }
        #endregion

        /// <summary>
        /// Синхронизирует значение контрола, со значенеями диаграммы
        /// </summary>
        public void RefreshValues()
        {
            View3DAppearance view3D = this.ChartView3D;

            this.tbPerspective.Value = (int)view3D.Perspective;
            this.SetToolTip(this.tbPerspective);

            this.tbScale.Value = (int)view3D.Scale;
            this.SetToolTip(this.tbScale);

            this.tbX.Value = CorrectRotationValue((int)view3D.XRotation);
            this.SetToolTip(this.tbX);

            this.tbY.Value = CorrectRotationValue((int)view3D.YRotation)* -1;
            this.SetToolTip(this.tbY);
        }

        private int CorrectRotationValue(int value)
        {
            if (Math.Abs(value) > 180)
            {
                if (value >= 0)
                {
                    return value % 180 - 180;
                }
                else
                {
                    return value % 180 + 180;
                }
            }
            else
            {
                return value;
            }
             
        }

        private void tbPerspective_Scroll(object sender, EventArgs e)
        {
            if (this.IsExistChartView3D)
            {
                this.ChartView3D.Perspective = this.tbPerspective.Value;
                this.SetToolTip(this.tbPerspective);
                this.OnValueChanged(sender, e);
            }
        }

        private void tbScale_Scroll(object sender, EventArgs e)
        {
            if (this.IsExistChartView3D)
            {
                this.ChartView3D.Scale = this.tbScale.Value;
                this.SetToolTip(this.tbScale);
                this.OnValueChanged(sender, e);
            }
        }

        private void tbX_Scroll(object sender, EventArgs e)
        {
            if (this.IsExistChartView3D)
            {
                this.ChartView3D.XRotation = this.tbX.Value;
                this.SetToolTip(this.tbX);
                this.OnValueChanged(sender, e);
            }
        }

        private void tbY_Scroll(object sender, EventArgs e)
        {
            if (this.IsExistChartView3D)
            {
                this.ChartView3D.YRotation = this.tbY.Value * -1;
                this.SetToolTip(this.tbY);
                this.OnValueChanged(sender, e);
            }
        }

        /// <summary>
        /// Сброс настроек в значения по умолчанию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ubReset_Click(object sender, EventArgs e)
        {
            if (this.IsExistChartView3D)
            {
                View3DAppearance view3D = this.ChartView3D;
                view3D.Perspective = 50;
                view3D.Scale = 65;
                view3D.XRotation = 144;
                view3D.YRotation = 12;

                this.RefreshValues();
                this.OnExitEditMode(sender, e);
            }
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            if (_valueChanged != null)
            {
                _valueChanged(sender, e);
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
        /// Выстваляет ТрекБарам подсказку равную ихним значениям
        /// </summary>
        /// <param name="trackBar"></param>
        private void SetToolTip(TrackBar trackBar)
        {
            string toolTip = (trackBar == this.tbY) ? (trackBar.Value * -1).ToString() : trackBar.Value.ToString();
            this.toolTip.SetToolTip(trackBar, toolTip);
        }

        /// <summary>
        /// Диаграмма
        /// </summary>
        public UltraChart Chart
        {
            get { return _chart; }
            set 
            {
                if (_chart != null)
                    //Если диаграмма уже была, удалим старый обработчик редактирования 3D вида
                    //_chart.Chart3DTransformed -= _chart_Chart3DTransformed;
                    _chart.Paint -= _chart_Paint;

                _chart = value;

                if (_chart != null)
                    //Повесим новый обработчик, на редактирование 3D вида
                    //_chart.Chart3DTransformed += new ChartTransform3DEventHandler(_chart_Chart3DTransformed);
                    //!!!Т.к. нужный обработчик оказался "мертвым", приходится синхронизировать 
                    //значения в контроле со значениями диаграммы на отрисовке
                    _chart.Paint += new PaintEventHandler(_chart_Paint);
            }
        }

        void _chart_Paint(object sender, PaintEventArgs e)
        {
            this.RefreshValues();
        }

        /*void _chart_Chart3DTransformed(object sender, ChartTransform3DEventArgs e)
        {
            this.RefreshValues();
        }*/

        /// <summary>
        /// Возвращает 3D редактор активной диаграммы
        /// </summary>
        private View3DAppearance ChartView3D
        {
            get { return this.Chart != null ? this.Chart.Transform3D : null; }
        }

        /// <summary>
        /// Есть ли у диаграммы 3D редактор
        /// </summary>
        private bool IsExistChartView3D
        {
            get { return this.ChartView3D != null; }
        }

        /// <summary>
        /// Событие происходи при редактировании любого из 3D параметров диаграммы
        /// </summary>
        public event EventHandler ValueChanged
        {
            add { _valueChanged += value; }
            remove { _valueChanged -= value; }
        }

        /// <summary>
        /// Событие происходи при завершении редактирования любого из 3D параметров диаграммы
        /// </summary>
        public event EventHandler ExitEditMode
        {
            add { _exitEditMode += value; }
            remove { _exitEditMode -= value; }
        }
    }
}
