using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components
{
	public partial class UltraChartItem : UserControl
	{
		
		public UltraChart Chart
		{
			get { return ChartControl; }
		}
		
		public Font DefaultFont { set; get; }
		public Color DefaultColor { set; get; }
		
		public bool BrowserSizeAdapting { set; get; }
		public string InvalidDataMessage { set; get; }
		public Color InvalidDataColor { set; get; }

		#region Свойства диаграммы
		
		public Unit Width
		{
			set
			{
				ChartControl.Width = BrowserSizeAdapting ? CRHelper.GetChartWidth(value.Value) : value;
			}
			get { return ChartControl.Width; }
		}

		public Unit Height
		{
			set
			{
				ChartControl.Height = BrowserSizeAdapting ? CRHelper.GetChartHeight(value.Value) : value;
			}
			get { return ChartControl.Height; }
		}

		private string reportID = String.Empty;
		public string ReportID
		{
			set
			{
				reportID = String.Format("{0}_", value);
				SetImageUrl();
			}
			get { return reportID; }
		}

		private string temporaryUrlPrefix;
		public string TemporaryUrlPrefix
		{
			set
			{
				temporaryUrlPrefix = value;
				ChartControl.DeploymentScenario.FilePath = String.Format("{0}/TemporaryImages", temporaryUrlPrefix);
				SetImageUrl();
			}
			get { return temporaryUrlPrefix; }
		}

		private void SetImageUrl()
		{
			ChartControl.DeploymentScenario.ImageURL = String.Format("{0}/{1}{2}_#SEQNUM(100).png", ChartControl.DeploymentScenario.FilePath, ReportID, ID);
		}

		private ChartColorModel colorModel;
		public ChartColorModel ColorModel
		{
			set
			{
				colorModel = value;
				SetColorModel();
			}
			get { return colorModel; }
		}

		public string TitleTop
		{
			set
			{
				ChartControl.TitleTop.Text = value;
				SetTitleTop();
			}
			get { return ChartControl.TitleTop.Text; }
		}

		public string TitleLeft
		{
			set
			{
				ChartControl.TitleLeft.Text = value;
				SetTitleLeft();
			}
			get { return ChartControl.TitleLeft.Text; }
		}

		public string TitleRight
		{
			set
			{
				ChartControl.TitleRight.Text = value;
				SetTitleRight();
			}
			get { return ChartControl.TitleRight.Text; }
		}

		public bool LegendVisible
		{
			set { ChartControl.Legend.Visible = value; }
			get { return ChartControl.Legend.Visible; }
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

		#endregion

		public UltraChartItem()
		{
			DefaultFont = new Font("Verdana", 8);
			DefaultColor = Color.Black;

			BrowserSizeAdapting = true;
			
			InvalidDataMessage = "Нет данных";
			InvalidDataColor = Color.Black;
		}

		protected void Page_PreLoad(object sender, EventArgs e)
		{
			TemporaryUrlPrefix = "../..";
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			ChartControl.InvalidDataReceived += InvalidDataReceived;
		}
		
		public void SetDefaultStyle()
		{
			ChartControl.Width = 400;
			ChartControl.Height = 300;

			ChartControl.EnableViewState = false;
			ChartControl.Border.Thickness = 0;

			ChartControl.Axis.X.Labels.Font = DefaultFont;
			ChartControl.Axis.X.Labels.FontColor = DefaultColor;
			ChartControl.Axis.X.Labels.SeriesLabels.Font = DefaultFont;
			ChartControl.Axis.X.Labels.SeriesLabels.FontColor = DefaultColor;

			ChartControl.Axis.Y.Labels.Font = DefaultFont;
			ChartControl.Axis.Y.Labels.FontColor = DefaultColor;
			ChartControl.Axis.Y.Labels.SeriesLabels.Font = DefaultFont;
			ChartControl.Axis.Y.Labels.SeriesLabels.FontColor = DefaultColor;

			ChartControl.Tooltips.Font.Name = DefaultFont.Name; 
			ChartControl.Tooltips.Font.Size = new FontUnit(DefaultFont.Size);
			ChartControl.Tooltips.FontColor = Color.Black;

			ChartControl.Legend.Font = DefaultFont;
			ChartControl.Legend.FontColor = DefaultColor;

			SetColorModel();

			SetTitleTop();
			SetTitleLeft();
		}

		private void InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
		{
			e.Text = InvalidDataMessage;
			e.LabelStyle.FontColor = InvalidDataColor;
			e.LabelStyle.FontSizeBestFit = false;
			e.LabelStyle.HorizontalAlign = StringAlignment.Center;
			e.LabelStyle.VerticalAlign = StringAlignment.Center;
		}

		#region Раскраска диаграммы

		public void SetColorModel()
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
					return Color.LimeGreen;
				case 2:
					return Color.LightSkyBlue;
				case 3:
					return Color.Gold;
				case 4:
					return Color.Peru;
				case 5:
					return Color.DarkOrange;
				case 6:
					return Color.PeachPuff;
				case 7:
					return Color.MediumSlateBlue;
				case 8:
					return Color.ForestGreen;
				case 9:
					return Color.HotPink;
			}
			return Color.White;
		}

		private static Color GetIphoneColor(int i)
		{
			switch (i)
			{
				case 1: 
					return Color.FromArgb(110, 189, 241);
				case 2: 
					return Color.FromArgb(214, 171, 133);
				case 3: 
					return Color.FromArgb(141, 178, 105);
				case 4: 
					return Color.FromArgb(192, 178, 224);
				case 5: 
					return Color.FromArgb(245, 187, 102);
				case 6:
					return Color.FromArgb(142, 164, 236);
				case 7:
					return Color.FromArgb(217, 230, 117);
				case 8:
					return Color.FromArgb(162, 154, 98);
				case 9:
					return Color.FromArgb(143, 199, 219);
				case 10:
					return Color.FromArgb(176, 217, 117);
				case 11:
					return Color.Cyan;
				case 12:
					return Color.Gold;
			}
			return Color.White;
		}

		private static Color GetIphoneStopColor(int i)
		{
			switch (i)
			{
				case 1:	
					return Color.FromArgb(9, 135, 214);
				case 2:	
					return Color.FromArgb(138, 71, 10);
				case 3:
					return Color.FromArgb(65, 124, 9);
				case 4:
					return Color.FromArgb(44, 20, 91);
				case 5:
					return Color.FromArgb(229, 140, 13);
				case 6:
					return Color.FromArgb(11, 45, 160);
				case 7:
					return Color.FromArgb(164, 184, 10);
				case 8:
					return Color.FromArgb(110, 98, 8);
				case 9:
					return Color.FromArgb(11, 100, 131);
				case 10:
					return Color.FromArgb(102, 168, 9);
				case 11:
					return Color.Cyan;
				case 12:
					return Color.Gold;
			}
			return Color.White;
		}

		#endregion

		#region Подписи диаграммы

		private void SetTitleTop()
		{
			if (TitleTop == String.Empty)
			{
				ChartControl.TitleTop.Visible = false;
				return;
			}

			ChartControl.TitleTop.Visible = true;
			ChartControl.TitleTop.Font = DefaultFont;
			ChartControl.TitleTop.FontColor = DefaultColor;
			ChartControl.TitleTop.HorizontalAlign = StringAlignment.Center;
			ChartControl.TitleTop.VerticalAlign = StringAlignment.Near;
			ChartControl.TitleTop.Extent = 15 * TitleTop.Split("\n").Length;
		}

		private void SetTitleLeft()
		{
			if (TitleLeft == String.Empty)
			{
				ChartControl.TitleLeft.Visible = false;
				return;
			}

			ChartControl.TitleLeft.Visible = true;
			ChartControl.TitleLeft.Font = DefaultFont;
			ChartControl.TitleLeft.HorizontalAlign = StringAlignment.Center;
			ChartControl.TitleLeft.Margins.Bottom = ChartControl.Axis.X.Extent;
			ChartControl.TitleLeft.Extent = 20 * TitleLeft.Split("\n").Length + 10;

			if (ChartControl.Legend.Visible && ChartControl.Legend.Location == LegendLocation.Bottom)
			{
				ChartControl.TitleLeft.Margins.Bottom += Convert.ToInt32(ChartControl.Height.Value * ChartControl.Legend.SpanPercentage / 100);
			}
		}

		private void SetTitleRight()
		{
			if (TitleRight == String.Empty)
			{
				ChartControl.TitleRight.Visible = false;
				return;
			}

			ChartControl.TitleRight.Visible = true;
			ChartControl.TitleRight.Font = DefaultFont;
			ChartControl.TitleRight.HorizontalAlign = StringAlignment.Center;
			ChartControl.TitleRight.Margins.Bottom = ChartControl.Axis.X.Extent;
			ChartControl.TitleRight.Extent = 20 * TitleLeft.Split("\n").Length + 10;

			if (ChartControl.Legend.Visible && ChartControl.Legend.Location == LegendLocation.Bottom)
			{
				ChartControl.TitleRight.Margins.Bottom += Convert.ToInt32(ChartControl.Height.Value * ChartControl.Legend.SpanPercentage / 100);
			}
		}

		#endregion
	}
}