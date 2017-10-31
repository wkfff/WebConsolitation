using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class UltraChartBrick : UserControl
    {
        #region Поля

        private string temporaryUrlPrefix = "../../";

        private DataTable chartDt = new DataTable();
        private Font defaultChartFont = new Font("Verdana", 8);
        private Font titleChartFont = new Font("Verdana", 10);
        private string dataFormatString;
        private string dataItemCaption;
        private ChartColorModel colorModel;
        private Unit width;
        private Unit height;
        private bool browserSizeAdapting = true;
        private string invalidDataMessage = "Нет данных";
        private bool x2AxisVisible = false;

        #endregion

        #region Свойства

        public DataTable DataTable
        {
            set { chartDt = value; }
            get { return chartDt; }
        }

        public string DataFormatString
        {
            set { dataFormatString = value; }
            get { return dataFormatString; }
        }

        public string DataItemCaption
        {
            set { dataItemCaption = value; }
            get { return dataItemCaption; }
        }

        public string TooltipFormatString
        {
            set { ChartControl.Tooltips.FormatString = value; }
            get { return ChartControl.Tooltips.FormatString; }
        }

        public bool SwapRowAndColumns
        {
            set { ChartControl.Data.SwapRowsAndColumns = value; }
            get { return ChartControl.Data.SwapRowsAndColumns; }
        }

        public bool ZeroAligned
        {
            set { ChartControl.Data.ZeroAligned = value; }
            get { return ChartControl.Data.ZeroAligned; }
        }

        public bool ColorSkinRowWise
        {
            set { ChartControl.ColorModel.Skin.ApplyRowWise = value; }
            get { return ChartControl.ColorModel.Skin.ApplyRowWise; }
        }

        public ChartColorModel ColorModel
        {
            set { colorModel = value; }
            get { return colorModel; }
        }

//        public string ChartSkinId
//        {
//            set { ChartControl.SkinID = value; }
//            get { return ChartControl.SkinID; }
//        }

        public Unit Width
        {
            set { width = value; }
            get { return width; }
        }

        public Unit Height
        {
            set { height = value; }
            get { return height; }
        }

        public UltraChart Chart
        {
            get { return ChartControl; }
        }

        public int XAxisExtent
        {
            get { return ChartControl.Axis.X.Extent; }
            set { ChartControl.Axis.X.Extent = value; }
        }

        public int YAxisExtent
        {
            get { return ChartControl.Axis.Y.Extent; }
            set { ChartControl.Axis.Y.Extent = value; }
        }

        public string TitleTop
        {
            get { return ChartControl.TitleTop.Text; }
            set { ChartControl.TitleTop.Text = value; }
        }

        public LegendAppearance Legend
        {
            get { return ChartControl.Legend; }
        }

        public string TemporaryUrlPrefix
        {
            get { return temporaryUrlPrefix; }
            set { temporaryUrlPrefix = value; }
        }

        public bool BrowserSizeAdapting
        {
            get { return browserSizeAdapting; }
            set { browserSizeAdapting = value; }
        }

        public string InvalidDataMessage
        {
            get { return invalidDataMessage; }
            set { invalidDataMessage = value; }
        }

        #endregion

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            ChartControl.Width = BrowserSizeAdapting ? CRHelper.GetChartWidth(width.Value) : width;
            ChartControl.Height = BrowserSizeAdapting ? CRHelper.GetChartHeight(height.Value) : height;
            
            ChartControl.Border.Thickness = 0;

            ChartControl.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChartInvalidDataReceived);
            ChartControl.DataBinding +=new EventHandler(ChartControl_DataBinding);
            ChartControl.FillSceneGraph += new FillSceneGraphEventHandler(ChartControl_FillSceneGraph);

            ChartControl.DeploymentScenario.ImageURL = String.Format("{0}TemporaryImages/ultraChartBrick#SEQNUM(500).png", temporaryUrlPrefix);
            ChartControl.DeploymentScenario.FilePath = String.Format("{0}TemporaryImages", temporaryUrlPrefix);

            SetChartAppearance();
            DataBind();
        }

        public virtual void DataBind()
        {
            ChartControl.DataBind();
        }

        protected virtual void SetChartAppearance()
        {
            ChartControl.Axis.X.Labels.Font = defaultChartFont;
            ChartControl.Axis.X.Labels.SeriesLabels.Font = defaultChartFont;
            
            ChartControl.Axis.Y.Labels.Font = defaultChartFont;
            ChartControl.Axis.Y.Labels.SeriesLabels.Font = defaultChartFont;

            ChartControl.Axis.X2.Labels.Font = defaultChartFont;
            ChartControl.Axis.X2.Labels.SeriesLabels.Font = defaultChartFont;

            ChartControl.Legend.Font = defaultChartFont;

            SetColorModel();

            SetTitleTop();
            SetItemCaption();
        }

        #region Обработчики диаграммы

        protected void ChartControl_DataBinding(object sender, EventArgs e)
        {
            ChartControl.DataSource = chartDt.Rows.Count > 0 ? chartDt : null;
        }

        protected virtual void ChartControl_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {

        }

        public void UltraChartInvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = invalidDataMessage;
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
        }

        #endregion

        #region Раскраска диаграммы

        private void SetColorModel()
        {
            switch (ColorModel)
            {
                case ChartColorModel.DefaultFixedColors:
                    {
                        ChartControl.ColorModel.ModelStyle = ColorModels.CustomLinear;
                        break;
                    }
                case ChartColorModel.ExtendedFixedColors:
                    {
                        ChartControl.ColorModel.ModelStyle = ColorModels.CustomSkin;
                        for (int i = 1; i < 10; i++)
                        {
                            Color color = GetCustomColor(i);
                            ChartControl.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color, 150));
                        }
                        
                        break;
                    }
                case ChartColorModel.PureRandom:
                    {
                        ChartControl.ColorModel.ModelStyle = ColorModels.PureRandom;
                        break;
                    }
                case ChartColorModel.IphoneColors:
                    {
                        ChartControl.ColorModel.ModelStyle = ColorModels.CustomSkin;
                        
                        for (int i = 1; i < 12; i++)
                        {
                            PaintElement pe = new PaintElement();
                            pe.ElementType = PaintElementType.Gradient;
                            pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                            pe.Fill = GetIphoneColor(i);
                            pe.FillStopColor = GetIphoneStopColor(i);
                            ChartControl.ColorModel.Skin.PEs.Add(pe);
                        }

                        break;
                    }
                case ChartColorModel.GreenRedColors:
                    {
                        ChartControl.ColorModel.ModelStyle = ColorModels.CustomSkin;

                        Color color1 = Color.LimeGreen;
                        Color color2 = Color.Red;

                        ChartControl.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
                        ChartControl.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));

                        ChartControl.Effects.Effects.Clear();
                        GradientEffect effect = new GradientEffect();
                        effect.Coloring = GradientColoringStyle.Darken;
                        effect.Enabled = true;
                        ChartControl.Effects.Enabled = true;
                        ChartControl.Effects.Effects.Add(effect);  
                        break;
                    }
            }
        }

        private static Color GetCustomColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.LimeGreen;
                    }
                case 2:
                    {
                        return Color.LightSkyBlue;
                    }
                case 3:
                    {
                        return Color.Gold;
                    }
                case 4:
                    {
                        return Color.Peru;
                    }
                case 5:
                    {
                        return Color.DarkOrange;
                    }
                case 6:
                    {
                        return Color.PeachPuff;
                    }
                case 7:
                    {
                        return Color.MediumSlateBlue;
                    }
                case 8:
                    {
                        return Color.ForestGreen;
                    }
                case 9:
                    {
                        return Color.HotPink;
                    }
            }
            return Color.White;
        }

        private static Color GetIphoneColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(110, 189, 241);
                    }
                case 2:
                    {
                        return Color.FromArgb(214, 171, 133);
                    }
                case 3:
                    {
                        return Color.FromArgb(141, 178, 105);
                    }
                case 4:
                    {
                        return Color.FromArgb(192, 178, 224);
                    }
                case 5:
                    {
                        return Color.FromArgb(245, 187, 102);
                    }
                case 6:
                    {
                        return Color.FromArgb(142, 164, 236);
                    }
                case 7:
                    {
                        return Color.FromArgb(217, 230, 117);
                    }
                case 8:
                    {
                        return Color.FromArgb(162, 154, 98);
                    }
                case 9:
                    {
                        return Color.FromArgb(143, 199, 219);
                    }
                case 10:
                    {
                        return Color.FromArgb(176, 217, 117);
                    }
                case 11:
                    {
                        return Color.Cyan;
                    }
                case 12:
                    {
                        return Color.Gold;
                    }
            }
            return Color.White;
        }

        private static Color GetIphoneStopColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(9, 135, 214);
                    }
                case 2:
                    {
                        return Color.FromArgb(138, 71, 10);
                    }
                case 3:
                    {
                        return Color.FromArgb(65, 124, 9);
                    }
                case 4:
                    {
                        return Color.FromArgb(44, 20, 91);
                    }
                case 5:
                    {
                        return Color.FromArgb(229, 140, 13);
                    }
                case 6:
                    {
                        return Color.FromArgb(11, 45, 160);
                    }
                case 7:
                    {
                        return Color.FromArgb(164, 184, 10);
                    }
                case 8:
                    {
                        return Color.FromArgb(110, 98, 8);
                    }
                case 9:
                    {
                        return Color.FromArgb(11, 100, 131);
                    }
                case 10:
                    {
                        return Color.FromArgb(102, 168, 9);
                    }
                case 11:
                    {
                        return Color.Cyan;
                    }
                case 12:
                    {
                        return Color.Gold;
                    }
            }
            return Color.White;
        }

        #endregion

        #region Настройка подписей диаграммы

        protected virtual void SetTitleTop()
        {
            if (ChartControl.TitleTop.Text == String.Empty)
            {
                ChartControl.TitleTop.Visible = false;
                return;
            }

            ChartControl.TitleTop.Visible = true;
            ChartControl.TitleTop.HorizontalAlign = StringAlignment.Center;
            ChartControl.TitleTop.Font = titleChartFont;
            ChartControl.TitleTop.Extent = 20;
        }

        protected virtual void SetItemCaption()
        {
            if (DataItemCaption == String.Empty)
            {
                ChartControl.TitleLeft.Visible = false;
                return;
            }

            ChartControl.TitleLeft.Visible = true;
            ChartControl.TitleLeft.Font = defaultChartFont;
            ChartControl.TitleLeft.Text = DataItemCaption;
            ChartControl.TitleLeft.HorizontalAlign = StringAlignment.Center;
            ChartControl.TitleLeft.Margins.Bottom = ChartControl.Axis.X.Extent;

            if (ChartControl.Legend.Visible && ChartControl.Legend.Location == LegendLocation.Bottom)
            {
                ChartControl.TitleLeft.Margins.Bottom += Convert.ToInt32(ChartControl.Height.Value * ChartControl.Legend.SpanPercentage / 100);
            }
        }

        #endregion
    }

    public enum ChartColorModel
    {
        DefaultFixedColors,
        ExtendedFixedColors,
        IphoneColors,
        PureRandom,
        GreenRedColors
    }
}