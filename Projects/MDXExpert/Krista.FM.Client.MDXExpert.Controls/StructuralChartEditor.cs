using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinChart;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class StructuralChartEditor : UserControl
    {
        private UltraChart _chart;

        private EventHandler _exitEditMode = null;

        public StructuralChartEditor()
        {
            InitializeComponent();

            this.tbLayerMinProc.Scroll += new EventHandler(tbLayerMinProc_Scroll);
            this.tbLayerMinProc.MouseUp += new MouseEventHandler(tbLayerMinProc_MouseUp);
            this.cbIsConcentricAppear.CheckedChanged += new EventHandler(cbIsConcentricAppear_CheckedChanged);
            this.BackColorChanged += new EventHandler(StructuralChartEditor_BackColorChanged);
            this.VisibleChanged += new EventHandler(StructuralChartEditor_VisibleChanged);
        }

        /// <summary>
        /// Синхронизируем значения в контроле со значениями диаграммы
        /// </summary>
        public void RefreshValues()
        {
            if (this.Chart != null)
            {
                this.cbIsConcentricAppear.Checked = this.GetConcentric(this.Chart);
                this.cbIsConcentricAppear.Enabled = this.IsAvaibleConcentricAppear(this.Chart);
                this.tbLayerMinProc.Value = Math.Min(this.GetOtherCategotyPercent(this.Chart), 
                    this.tbLayerMinProc.Maximum);
                this.SetToolTip(this.tbLayerMinProc);
            }
        }

        #region Допольнительные методы 
        /// <summary>
        /// Доступно ли для диаграммы концентрическое представление
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private bool IsAvaibleConcentricAppear(UltraChart chart)
        {
            return (this.Chart.ChartType == ChartType.PieChart|| 
                this.Chart.ChartType == ChartType.PieChart3D || 
                this.Chart.ChartType == ChartType.DoughnutChart || 
                this.Chart.ChartType == ChartType.DoughnutChart3D);
        }

        /// <summary>
        /// Получить включеность концентрического представления
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private bool GetConcentric(UltraChart chart)
        {
            if (chart != null)
            {
                switch (this.Chart.ChartType)
                {
                    case ChartType.DoughnutChart:
                    case ChartType.DoughnutChart3D:
                        {
                            return this.Chart.DoughnutChart.Concentric;
                        }
                    case ChartType.PieChart:
                    case ChartType.PieChart3D:
                        {
                            return this.Chart.PieChart.Concentric;
                        }
                }
            }
            return false;
        }

        /// <summary>
        /// Установить включенность концентрического представления
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="value"></param>
        private void SetConcentric(UltraChart chart, bool value)
        {
            if (chart != null)
            {
                switch (this.Chart.ChartType)
                {
                    case ChartType.DoughnutChart:
                    case ChartType.DoughnutChart3D:
                        {
                            this.Chart.DoughnutChart.Concentric = value;
                            break;
                        }
                    case ChartType.PieChart:
                    case ChartType.PieChart3D:
                        {
                            this.Chart.PieChart.Concentric = value;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Получить минимальный процент слоя у диаграммы
        /// </summary>
        /// <param name="chart"></param>
        /// <returns></returns>
        private int GetOtherCategotyPercent(UltraChart chart)
        {
            if (chart != null)
            {
                switch (chart.ChartType)
                {
                    case ChartType.DoughnutChart:
                    case ChartType.DoughnutChart3D:
                        return (int)chart.DoughnutChart.OthersCategoryPercent;
                    case ChartType.PieChart:
                    case ChartType.PieChart3D:
                        return (int)chart.PieChart.OthersCategoryPercent;
                    case ChartType.FunnelChart:
                        return (int)chart.FunnelChart.OthersCategoryPercent;
                    case ChartType.FunnelChart3D:
                        return (int)chart.FunnelChart3D.OthersCategoryPercent;
                    case ChartType.ConeChart3D:
                        return (int)chart.ConeChart3D.OthersCategoryPercent;
                    case ChartType.PyramidChart:
                        return (int)chart.PyramidChart.OthersCategoryPercent;
                    case ChartType.PyramidChart3D:
                        return (int)chart.PyramidChart3D.OthersCategoryPercent;
                }
            }
            return 0;
        }

        /// <summary>
        /// Установить диаграмме минимальный процент слоя
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="value"></param>
        private void SetOtherCategotyPercent(UltraChart chart, int value)
        {
            if ((chart != null) && (value >= 0) && (value <= 100)) 
            {
                switch (chart.ChartType)
                {
                    case ChartType.DoughnutChart:
                    case ChartType.DoughnutChart3D:
                        {
                            chart.DoughnutChart.OthersCategoryPercent = value;
                            break;
                        }
                    case ChartType.PieChart:
                    case ChartType.PieChart3D:
                        {
                            chart.PieChart.OthersCategoryPercent = value;
                            break;
                        }
                    case ChartType.FunnelChart:
                        {
                            chart.FunnelChart.OthersCategoryPercent = value;
                            break;
                        }
                    case ChartType.FunnelChart3D:
                        {
                            chart.FunnelChart3D.OthersCategoryPercent = value;
                            break;
                        }
                    case ChartType.ConeChart3D:
                        {
                            chart.ConeChart3D.OthersCategoryPercent = value;
                            break;
                        }
                    case ChartType.PyramidChart:
                        {
                            chart.PyramidChart.OthersCategoryPercent = value;
                            break;
                        }
                    case ChartType.PyramidChart3D:
                        {
                            chart.PyramidChart3D.OthersCategoryPercent = value;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Выстваляет ТрекБарам подсказку равную ихним значениям
        /// </summary>
        /// <param name="trackBar"></param>
        private void SetToolTip(TrackBar trackBar)
        {
            this.toolTip.SetToolTip(trackBar, trackBar.Value.ToString() + "%");
        }
        #endregion

        #region Обработчики событий
        /// <summary>
        /// При цветовой схеме "#5" (серая) чек боксы не приводятся к этому стилю, и становятся белыми,
        /// будем красить их в ручную
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StructuralChartEditor_BackColorChanged(object sender, EventArgs e)
        {
            Color silver = Color.FromArgb(255, 237, 237, 237);
            if (this.BackColor == silver)
            {
                this.cbIsConcentricAppear.UseAppStyling = false;
            }
            else
            {
                this.cbIsConcentricAppear.UseAppStyling = true;
            }
            this.cbIsConcentricAppear.BackColor = this.BackColor;
        }

        void StructuralChartEditor_VisibleChanged(object sender, EventArgs e)
        {
            //Если этого не сделаем, подсказки будут не видны
            this.toolTip.RemoveAll();
        }

        void tbLayerMinProc_Scroll(object sender, EventArgs e)
        {
            if (this.Chart != null)
            {
                this.SetOtherCategotyPercent(this.Chart, this.tbLayerMinProc.Value);
                this.SetToolTip(this.tbLayerMinProc);
            }
        }

        void tbLayerMinProc_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.Chart != null)
            {
                this.DoExitEditMode(sender, e);
            }
        }

        void cbIsConcentricAppear_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Chart != null)
            {
                this.SetConcentric(this.Chart, this.cbIsConcentricAppear.Checked);
                this.DoExitEditMode(sender, e);
            }
        }

        private void DoExitEditMode(object sender, EventArgs e)
        {
            if (_exitEditMode != null)
            {
                _exitEditMode(sender, e);
            }
        }
        #endregion

        /// <summary>
        /// Диаграмма
        /// </summary>
        public UltraChart Chart
        {
            get { return _chart; }
            set
            {
                _chart = value;
                this.RefreshValues();
            }
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
