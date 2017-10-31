using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinChart;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert.Controls
{

    public partial class ChartAxisEditor : UserControl
    {
        private UltraChart _chart;
        private EventHandler _exitEditMode = null;
        private bool isMayHook;
        private int selectedCompositeLayer;

        public ChartAxisEditor()
        {
            InitializeComponent();

            this.cbXVisible.CheckedChanged += new EventHandler(cbXVisible_CheckedChanged);
            this.cbX2Visible.CheckedChanged += new EventHandler(cbX2Visible_CheckedChanged);
            this.cbYVisible.CheckedChanged += new EventHandler(cbYVisible_CheckedChanged);
            this.cbY2Visible.CheckedChanged += new EventHandler(cbY2Visible_CheckedChanged);
            this.cbZVisible.CheckedChanged += new EventHandler(cbZVisible_CheckedChanged);

            this.tbXExtent.Scroll += new EventHandler(tbXExtent_Scroll);
            this.tbXExtent.MouseUp += new MouseEventHandler(tbXExtent_MouseUp);

            this.tbX2Extent.Scroll += new EventHandler(tbX2Extent_Scroll);
            this.tbX2Extent.MouseUp += new MouseEventHandler(tbX2Extent_MouseUp);

            this.tbYExtent.Scroll += new EventHandler(tbYExtent_Scroll);
            this.tbYExtent.MouseUp += new MouseEventHandler(tbYExtent_MouseUp);

            this.tbY2Extent.Scroll += new EventHandler(tbY2Extent_Scroll);
            this.tbY2Extent.MouseUp += new MouseEventHandler(tbY2Extent_MouseUp);

            this.BackColorChanged += new EventHandler(ChartAxisEditor_BackColorChanged);
            this.VisibleChanged += new EventHandler(ChartAxisEditor_VisibleChanged);
        }

        /// <summary>
        /// Синхронизируем значения в контроле со значениями легенды диаграммы
        /// </summary>
        public void RefreshValues()
        {
            try
            {
                this.isMayHook = true;

                if (this.IsExistChart)
                {
                    this.InitAxisControls();
                    this.InitAxisVisibility();
                    this.InitAxisExtent();
                }
            }
            finally
            {
                this.isMayHook = false;
            }
        }

        #region Обработчики событий
        //Ось X
        void cbXVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                this.Chart.Axis.X.Visible = this.cbXVisible.Checked;
                this.DoExitEditMode(sender, e);
            }
        }

        void tbXExtent_Scroll(object sender, EventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                if (this.Chart.ChartType == ChartType.Composite)
                {
                    AxisItem axisX = CompositeChartUtils.GetCompositeAxisX(Chart, selectedCompositeLayer);
                    if (axisX != null)
                    {
                        axisX.Extent = this.tbXExtent.Value;
                    }
                }
                else
                {
                    this.Chart.Axis.X.Extent = this.tbXExtent.Value;
                }
                this.SetToolTip(this.tbXExtent);
            }
        }

        void tbXExtent_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                this.DoExitEditMode(sender, e);
            }
        }

        //Ось X2
        void cbX2Visible_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                this.Chart.Axis.X2.Visible = this.cbX2Visible.Checked;
                this.DoExitEditMode(sender, e);
            }
        }

        void tbX2Extent_Scroll(object sender, EventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                this.Chart.Axis.X2.Extent = this.tbX2Extent.Value;
                this.SetToolTip(this.tbX2Extent);
            }
        }

        void tbX2Extent_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                this.DoExitEditMode(sender, e);
            }
        }

        //Ось Y
        void cbYVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                this.Chart.Axis.Y.Visible = this.cbYVisible.Checked;
                this.DoExitEditMode(sender, e);
            }
        }

        void tbYExtent_Scroll(object sender, EventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                if (this.Chart.ChartType == ChartType.Composite)
                {
                    AxisItem axisY = CompositeChartUtils.GetCompositeAxisY(Chart, selectedCompositeLayer);
                    if (axisY != null)
                    {
                        axisY.Extent = this.tbYExtent.Value;
                    }
                }
                else
                {
                    this.Chart.Axis.Y.Extent = this.tbYExtent.Value;
                }
                this.SetToolTip(this.tbYExtent);
            }
        }

        void tbYExtent_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                this.DoExitEditMode(sender, e);
            }
        }

        //Ось Y2
        void cbY2Visible_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                this.Chart.Axis.Y2.Visible = this.cbY2Visible.Checked;
                this.DoExitEditMode(sender, e);
            }
        }

        void tbY2Extent_Scroll(object sender, EventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                if (this.Chart.ChartType == ChartType.Composite)
                {
                    AxisItem axisY2 = CompositeChartUtils.GetCompositeAxisY2(Chart, selectedCompositeLayer);
                    if (axisY2 != null)
                    {
                        axisY2.Extent = this.tbY2Extent.Value;
                    }
                }
                else
                {
                    this.Chart.Axis.Y2.Extent = this.tbY2Extent.Value;
                }
                this.SetToolTip(this.tbY2Extent);
            }
        }

        void tbY2Extent_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                this.DoExitEditMode(sender, e);
            }
        }

        //Ось Z
        void cbZVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsExistChart && !this.isMayHook)
            {
                this.Chart.Axis.Z.Visible = this.cbZVisible.Checked;
                this.DoExitEditMode(sender, e);
            }
        }

        /// <summary>
        /// При цветовой схеме "#5" (серая) чек боксы не приводятся к этому стилю, и становятся белыми,
        /// будем красить их в ручную
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChartAxisEditor_BackColorChanged(object sender, EventArgs e)
        {
            Color silver = Color.FromArgb(255, 237, 237, 237);

            this.cbXVisible.UseAppStyling = this.BackColor != silver;
            this.cbX2Visible.UseAppStyling = this.BackColor != silver;
            this.cbYVisible.UseAppStyling = this.BackColor != silver;
            this.cbY2Visible.UseAppStyling = this.BackColor != silver;
            this.cbZVisible.UseAppStyling = this.BackColor != silver;

            this.cbXVisible.BackColor = this.BackColor;
            this.cbX2Visible.BackColor = this.BackColor;
            this.cbYVisible.BackColor = this.BackColor;
            this.cbY2Visible.BackColor = this.BackColor;
            this.cbZVisible.BackColor = this.BackColor;
        }

        void ChartAxisEditor_VisibleChanged(object sender, EventArgs e)
        {
            //Если этого не сделаем, подсказки будут не видны
            this.toolTip.RemoveAll();
        }
        #endregion

        #region Вспомогательные методы
        /// <summary>
        /// Устанавливаем видимость доступных осей для данного типа диаграммы
        /// </summary>
        private void InitAxisControls()
        {
            if (this.IsExistChart)
            {
                //если доступна ось Z, значит это 3D диаграмма, так же есть 2D диаграммы у которых 
                //размер оси не редактируется, а значит вид контрола должен быть такой же 
                //как и для 3D диаграмм
                bool is3dChart = Common.InfragisticsUtils.IsAvaibleZAxis(this.Chart)
                                 || this.Chart.ChartType == ChartType.RadarChart;

                bool isComposite = this.Chart.ChartType == ChartType.Composite;

                //трек бары
                this.tbXExtent.Visible = !is3dChart;
                this.tbX2Extent.Visible = !is3dChart && !isComposite;
                this.tbYExtent.Visible = !is3dChart;
                this.tbY2Extent.Visible = !is3dChart;
                if (isComposite)
                {
                    this.tbYExtent.Enabled =
                        CompositeChartUtils.GetVisibleCompositeAxisY(this.Chart, selectedCompositeLayer);

                    this.tbY2Extent.Enabled =
                        CompositeChartUtils.GetVisibleCompositeAxisY2(this.Chart, selectedCompositeLayer);
                }
                //чек боксы
                this.cbXVisible.Visible = !isComposite;
                this.cbX2Visible.Visible = !is3dChart && !isComposite;
                this.cbYVisible.Visible = !isComposite;
                this.cbY2Visible.Visible = !is3dChart && !isComposite;
                //лейблы
                this.ulX2.Visible = !is3dChart && !isComposite;
                this.ulY2.Visible = !is3dChart;

                this.ulZ.Visible = is3dChart;
                this.cbZVisible.Visible = is3dChart;

                this.ulX.Enabled = this.Chart.ChartType != ChartType.RadarChart;
                this.cbXVisible.Enabled = this.Chart.ChartType != ChartType.RadarChart;
                this.ulZ.Enabled = this.Chart.ChartType != ChartType.RadarChart;
                this.cbZVisible.Enabled = this.Chart.ChartType != ChartType.RadarChart;
                
                //Устанавливаем расположение и размер контролов
                this.SetControlSizeAndLocation(is3dChart, isComposite);
            }
        }

        private void SetControlSizeAndLocation(bool is3dChart, bool isComposite)
        {
            if (is3dChart)
            {
                this.ulX.Location = new Point(15, this.ulX.Location.Y);
                this.cbXVisible.Location = new Point(15, this.cbXVisible.Location.Y);
                
                this.ulY.Location = new Point(45, this.ulY.Location.Y);
                this.cbYVisible.Location = new Point(45, this.cbYVisible.Location.Y);
                this.tbYExtent.Location = new Point(97, this.tbYExtent.Location.Y);
                this.ulY2.Location = new Point(167, this.ulY2.Location.Y);
                this.tbY2Extent.Location = new Point(144, this.tbY2Extent.Location.Y);
                this.Size = new Size(100, this.Size.Height);
            }
            else
            {
                if (isComposite)
                {
                    this.ulX.Location = new Point(20, this.ulX.Location.Y);

                    this.ulY.Location = new Point(60, this.ulY.Location.Y);
                    this.tbYExtent.Location = new Point(45, this.tbYExtent.Location.Y);

                    this.ulY2.Location = new Point(100, this.ulY2.Location.Y);
                    this.tbY2Extent.Location = new Point(85, this.tbY2Extent.Location.Y);
                    
                    this.Size = new Size(120, this.Size.Height);
                }
                else
                {
                    this.ulX.Location = new Point(29, this.ulX.Location.Y);
                    this.cbXVisible.Location = new Point(29, this.cbXVisible.Location.Y);

                    this.ulY.Location = new Point(123, this.ulY.Location.Y);
                    this.cbYVisible.Location = new Point(123, this.cbYVisible.Location.Y);
                    this.tbYExtent.Location = new Point(97, this.tbYExtent.Location.Y);
                    this.ulY2.Location = new Point(167, this.ulY2.Location.Y);
                    this.tbY2Extent.Location = new Point(144, this.tbY2Extent.Location.Y);
                    this.Size = new Size(194, this.Size.Height);
                }
            }
        }

        /// <summary>
        /// Проставляем видимости осей
        /// </summary>
        private void InitAxisVisibility()
        {
            this.cbXVisible.Checked = this.Chart.Axis.X.Visible;
            this.cbX2Visible.Checked = this.Chart.Axis.X2.Visible;
            this.cbYVisible.Checked = this.Chart.Axis.Y.Visible;
            this.cbY2Visible.Checked = this.Chart.Axis.Y2.Visible;
            this.cbZVisible.Checked = this.Chart.Axis.Z.Visible;
        }

        /// <summary>
        /// Инициализируем размер оси
        /// </summary>
        private void InitAxisExtent()
        {
            if (this.IsExistChart)
            {
                bool isAvaible = Common.InfragisticsUtils.IsAvailableAxisExtent(this.Chart);
                if (isAvaible)
                {
                    if (this.Chart.ChartType == ChartType.Composite)
                    {
                        if (Chart.CompositeChart.ChartAreas.Count != 0)
                        {
                            this.tbXExtent.Value = CompositeChartUtils.GetCompositeAxisX(Chart, selectedCompositeLayer) == null ?
                                0 : CompositeChartUtils.GetCompositeAxisX(Chart, selectedCompositeLayer).Extent;
                            this.tbYExtent.Value = CompositeChartUtils.GetCompositeAxisY(Chart, selectedCompositeLayer) == null ?
                                0 : CompositeChartUtils.GetCompositeAxisY(Chart, selectedCompositeLayer).Extent;
                            this.tbY2Extent.Value = CompositeChartUtils.GetCompositeAxisY2(Chart, selectedCompositeLayer) == null ?
                                0 : CompositeChartUtils.GetCompositeAxisY2(Chart, selectedCompositeLayer).Extent;
                        }
                    }
                    else
                    {
                        this.tbXExtent.Value = this.Chart.Axis.X.Extent;
                        this.tbX2Extent.Value = this.Chart.Axis.X2.Extent;
                        this.tbYExtent.Value = this.Chart.Axis.Y.Extent;
                        this.tbY2Extent.Value = this.Chart.Axis.Y2.Extent;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Выстваляет ТрекБарам подсказку равную ихним значениям
        /// </summary>
        /// <param name="trackBar"></param>
        private void SetToolTip(TrackBar trackBar)
        {
            this.toolTip.SetToolTip(trackBar, trackBar.Value.ToString());
        }

        private void DoExitEditMode(object sender, EventArgs e)
        {
            if (_exitEditMode != null)
            {
                _exitEditMode(sender, e);
            }
        }

        /// <summary>
        /// Существует ли диаграмма
        /// </summary>
        private bool IsExistChart
        {
            get { return this.Chart != null; }
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
        /// Событие происходи при завершении редактирования
        /// </summary>
        public event EventHandler ExitEditMode
        {
            add { _exitEditMode += value; }
            remove { _exitEditMode -= value; }
        }

        /// <summary>
        /// Выбранный слой композитной диаграммы
        /// </summary>
        public int SelectedCompositeLayer
        {
            get { return selectedCompositeLayer; }
            set { selectedCompositeLayer = value; }
        }
    }
}
