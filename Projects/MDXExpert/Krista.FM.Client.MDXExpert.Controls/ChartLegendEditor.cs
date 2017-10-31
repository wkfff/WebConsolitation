using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Win.UltraWinChart;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class ChartLegendEditor : UserControl
    {
        private UltraChart _chart;

        private EventHandler _exitEditMode = null;

        private bool isMayHook;

        public ChartLegendEditor()
        {
            InitializeComponent();

            this.cbVisible.CheckedChanged += new EventHandler(cbVisible_CheckedChanged);
            this.ucLocation.ValueChanged += new EventHandler(ucLocation_ValueChanged);
            this.tbSpan.Scroll += new EventHandler(tbSpan_Scroll);
            this.tbSpan.MouseUp += new MouseEventHandler(tbSpan_MouseUp);
            this.BackColorChanged += new EventHandler(ChartLegendEditor_BackColorChanged);
            this.VisibleChanged += new EventHandler(ChartLegendEditor_VisibleChanged);
        }

        /// <summary>
        /// При цветовой схеме "#5" (серая) чек боксы не приводятся к этому стилю, и становятся белыми,
        /// будем красить их в ручную
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChartLegendEditor_BackColorChanged(object sender, EventArgs e)
        {
            Color silver = Color.FromArgb(255, 237, 237, 237);
            if (this.BackColor == silver)
            {
                this.cbVisible.UseAppStyling = false;
            }
            else
            {
                this.cbVisible.UseAppStyling = true;
            }
            this.cbVisible.BackColor = this.BackColor;
        }

        /// <summary>
        /// Синхронизируем значения в контроле со значениями легенды диаграммы
        /// </summary>
        public void RefreshValues()
        {
            if (this.IsExistLegend)
            {
                isMayHook = true;
                if (this.Chart.ChartType == ChartType.Composite)
                {
                    if (this.Chart.CompositeChart.Legends.Count != 0)
                    {
                        this.cbVisible.Checked = this.Chart.CompositeChart.Legends[0].Visible;
                        if (this.Chart.Parent != null)
                        {
                            this.ucLocation.SelectedIndex =
                                (int) ((ICompositeLegendParams) Chart.Parent.Parent.Parent).CompositeLegendLocation;
                            this.tbSpan.Value =
                                ((ICompositeLegendParams) Chart.Parent.Parent.Parent).CompositeLegendExtent;
                        }
                    }
                    else
                    {
                        this.cbVisible.Checked = false;
                    }
                }
                else
                {
                    this.cbVisible.Checked = this.Legend.Visible;
                    this.ucLocation.SelectedIndex = (int)this.Legend.Location;
                    this.tbSpan.Value = this.Legend.SpanPercentage;
                }
                isMayHook = false;

                this.SetToolTip(this.tbSpan);
            }
        }

        /// <summary>
        /// Видимость легенды
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsExistLegend)
            {
                if (this.Chart.ChartType == ChartType.Composite)
                {
                    if (this.Chart.CompositeChart.Legends.Count != 0)
                    {
                        this.Chart.CompositeChart.Legends[0].Visible = this.cbVisible.Checked;
                        this.Chart.InvalidateLayers();
                    }
                }
                else
                {
                    this.Legend.Visible = this.cbVisible.Checked;
                }
                this.DoExitEditMode(sender, e);
            }
        }

        /// <summary>
        /// Расположение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ucLocation_ValueChanged(object sender, EventArgs e)
        {
            if (this.IsExistLegend)
            {
                if (this.Chart.ChartType == ChartType.Composite)
                {
                    if (!isMayHook)
                        ((ICompositeLegendParams)Chart.Parent.Parent.Parent).CompositeLegendLocation = (Infragistics.UltraChart.Shared.Styles.LegendLocation)this.ucLocation.SelectedIndex;
                }
                else
                {
                    this.Legend.Location = (Infragistics.UltraChart.Shared.Styles.LegendLocation)this.ucLocation.SelectedIndex;
                }
                this.DoExitEditMode(sender, e);
            }
        }

        /// <summary>
        /// Размер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tbSpan_Scroll(object sender, EventArgs e)
        {
            if (this.IsExistLegend)
            {
                if (this.Chart.ChartType == ChartType.Composite)
                {
                    ((ICompositeLegendParams)Chart.Parent.Parent.Parent).CompositeLegendExtent = this.tbSpan.Value;
                }
                else
                {
                    this.Legend.SpanPercentage = this.tbSpan.Value;
                }
                this.SetToolTip(this.tbSpan);
            }
        }

        void tbSpan_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsExistLegend)
            {
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

        void ChartLegendEditor_VisibleChanged(object sender, EventArgs e)
        {
            //Если этого не сделаем, подсказки будут не видны
            this.toolTip.RemoveAll();
        }

        /// <summary>
        /// Выстваляет ТрекБарам подсказку равную ихним значениям
        /// </summary>
        /// <param name="trackBar"></param>
        private void SetToolTip(TrackBar trackBar)
        {
            this.toolTip.SetToolTip(trackBar, trackBar.Value.ToString() + "%");
        }

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
        /// Возвращает легенду активной диаграммы
        /// </summary>
        private LegendAppearance Legend
        {
            get { return this.Chart != null ? this.Chart.Legend : null; }
        }

        /// <summary>
        /// Есть ли у диаграммы легенда
        /// </summary>
        private bool IsExistLegend
        {
            get { return this.Legend != null; }
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
