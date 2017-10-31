using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.iPadBricks.Sport_0001
{
	/// <summary>
	/// Класс глобальных настроек и общих методов
	/// </summary>
	public static class Helper
	{
		public static string DISEASES = "58,59";
		public static int PARAGRAPH_LENGTH = 40;

		public static int YEAR_LAST = 2011;
		public static int YEAR_PREV = 2010;

		public static decimal PROVIDE_AVG_GYMN = 400m;
		public static decimal PROVIDE_NRM_GYMN = 3.5m;
		public static decimal PROVIDE_AVG_STAD = 540m;
		public static decimal PROVIDE_NRM_STAD = 19.5m;
		public static decimal PROVIDE_AVG_SWIM = 200000m;
		public static decimal PROVIDE_NRM_SWIM = 750m;


		public static string BestRankFile()
		{
			return "../../../images/starYellowBB.png";
		}

		public static string BestRank()
		{
			return String.Format("<img src=\"{0}\" height=\"16\" style=\"margin-bottom:-1px;\"/>&nbsp;", BestRankFile());
		}

		public static string BestRankAbs(int left = 0)
		{
			return String.Format("<img src=\"{0}\" style=\"position: absolute; left:{1}px;\"/>", BestRankFile(), left);
		}

		public static string WorstRankFile()
		{
			return "../../../images/starGrayBB.png";
		}

		public static string WorstRank()
		{
			return String.Format("<img src=\"{0}\" height=\"18\" style=\"margin-bottom:-2px;\"/>&nbsp;", WorstRankFile());
		}

		public static string WorstRankAbs(int left = 0)
		{
			return String.Format("<img src=\"{0}\" style=\"position: absolute; left:{1}px;\"/>", WorstRankFile(), left);
		}

		public static string UpArrowFile(bool inversed = false)
		{
			if (!inversed)
				return "../../../images/arrowGreenUpBB.png";
			else
				return "../../../images/arrowRedUpBB.png";
		}

		public static string DnArrowFile(bool inversed = false)
		{
			if (!inversed)
				return "../../../images/arrowRedDownBB.png";
			else
				return "../../../images/arrowGreenDownBB.png";
		}

		public static string MaxFile()
		{
			return "../../../images/Max.png";
		}

		public static string MinFile()
		{
			return "../../../images/Min.png";
		}

		public static string MaxAbs(int left = 0, int top = 0)
		{
			return String.Format("<img src=\"{0}\" style=\"position: absolute; left:{1}px; top:{2}px;\"/>", MaxFile(), left, top);
		}

		public static string MinAbs(int left = 0, int top = 0)
		{
			return String.Format("<img src=\"{0}\" style=\"position: absolute; left:{1}px; top:{2}px;\"/>", MinFile(), left, top);
		}

		public static string CellIndicatorStyle(int offsetLeft = 2)
		{
			return String.Format("background-repeat: no-repeat; background-position: {0}px; center", offsetLeft);
		}

		public static Color GetRecreationColor(RecreationType type)
		{
			switch (type)
			{
				case RecreationType.Gymnasium:
					return Color.FromArgb(unchecked((int) 0xffffab00));
				case RecreationType.Stadium:
					return Color.FromArgb(unchecked((int) 0xff3fff7f));
				case RecreationType.Swimming:
					return Color.FromArgb(unchecked((int) 0xff009cac));
				case RecreationType.Other:
					return Color.FromArgb(unchecked((int) 0xffd292ff));
			}
			return Color.White;
		}

		public static PaintElement GetRecreationPE(RecreationType type)
		{
			return CRHelper.GetFillPaintElement(GetRecreationColor(type), 150);
		}
	}

	public enum RecreationType
	{
		Gymnasium = 1,
		Stadium,
		Swimming,
		Other
	}

	/// <summary>
	/// Упорядоченный список
	/// </summary>
	public class OrderedData
	{
		public List<OrderedValue> Data { private set; get; }
		public bool Inversed { set; get; }
		public int Precision { set; get; }
		public int MaxRank { private set; get; }
		public int Count { get { return Data.Count; } }
		
		public int NonEmptyCount
		{
			get
			{
				int count = 0;
				foreach (OrderedValue value in Data)
				{
					if (!value.IsEmpty)
					{
						count++;
					}
				}
				return count;
			}
		}

		public OrderedValue this[int index]
		{
			get { return Data[index]; }
		}

		public OrderedValue this[string id]
		{
			get { return Data.Find(value => value.ID.Equals(id)); }
		}
		
		public OrderedData()
		{
			Data = new List<OrderedValue>();
			Inversed = false;
			MaxRank = 0;
			Precision = 4;
		}

		public OrderedData(DataTable dataTable, int columnIndex) 
			: this()
		{
			AddFromTable(dataTable, columnIndex);
		}
		
		public void Sort()
		{
			Data.Sort();
			if (Inversed)
			{
				Data.Reverse();
			}
			RemoveEmpty();
			CalcRank();
		}

		public void Clear()
		{
			Data.Clear();
		}

		public void RemoveEmpty()
		{
			int i = 0;
			while (i < Data.Count)
			{
				OrderedValue value = Data[i];
				if (value.IsEmpty)
				{
					Data.Remove(value);
				}
				else
				{
					i++;
				}
			}
		}

		public void Add(OrderedValue value)
		{
			Data.Add(value);
		}

		public void AddFromTable(DataTable dataTable, int columnIndex)
		{
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string id = dataTable.Rows[i][0].ToString();
				object value = dataTable.Rows[i][columnIndex];
				if (!CRHelper.DBValueIsEmpty(value))
				{
					Add(new OrderedValue(id, CRHelper.DBValueConvertToDecimalOrZero(value)));
				}
				else
				{
					Add(new OrderedValue(id));
				}
			}
		}

		/// <summary>
		/// Вывод всего списка (для отладки)
		/// </summary>
		public override string ToString()
		{
			StringBuilder text = new StringBuilder();
			int i = 0;
			foreach (OrderedValue value in Data)
			{
				text.AppendFormat(
					"{0} ({1}) {2}: {3}{4}{5}",
					Convert.ToInt32(i).ToString().PadRight(3),
					Convert.ToInt32(value.Rank).ToString().PadRight(3),
					value.ID.PadRight(50),
					value.IsEmpty ? "empty" : value.Value.ToString(),
					value.Note,
					Environment.NewLine);
				i++;
			}
			return text.ToString();
		}

		public int GetRank(string id)
		{
			foreach (OrderedValue cur in Data)
			{
				if (cur.ID.Equals(id))
				{
					return cur.Rank;
				}
			}
			return 0;
		}

		public string GetDataName(int index)
		{
			index = GetIndex(index);
			return Data[index].ID;
		}

		public decimal GetDataValue(int index)
		{
			index = GetIndex(index);
			return Data[index].Value;
		}

		private int GetIndex(int index)
		{
			if (index > 0)
			{
				//максимальный по счету элемент
				index--;
			}
			else if (index < 0)
			{
				// минимальный по счету элемент
				index = GetLastNonEmptyIndex() + index;
			}

			if (index >= Data.Count)
				throw new Exception("OrderedData.GetIndex(): index >= Data.Count");
			if (index < 0)
				throw new Exception("OrderedData.GetIndex(): index < 0");

			while (index > 0 && Data[index].IsEmpty)
			{
				index--;
			}

			if (Data[index].IsEmpty)
				throw new Exception("OrderedData.GetIndex(): Data[index].IsEmpty");

			return index;
		}

		private int GetLastNonEmptyIndex()
		{
			int i = Data.Count;
			while (i > 0 && Data[i - 1].IsEmpty)
			{
				i--;
			}
			return i;
		}

		private void CalcRank()
		{
			// Empty-элементов быть не должно
			// необходимо выполнять ClearEmpty и Sort перед вычислением ранга

			int rank = 1;
			int rankEqual = 0;
			OrderedValue prev = null;
			foreach (OrderedValue cur in Data)
			{
				if (cur.IsEmpty)
				{
					throw new Exception("Empty item in CalcRank() function");
				}

				if (prev != null)
				{
					if ((!Inversed && cur.Value.CompareTo(prev.Value, Precision) < 0)
					    || (Inversed && cur.Value.CompareTo(prev.Value, Precision) > 0))
					{
						rank = rank + rankEqual + 1;
						rankEqual = 0;
					}
					else
					{
						rankEqual++;
					}
				}

				cur.SetRank(rank);

				prev = cur;
			}
			MaxRank = rank;
		}
	}

	/// <summary>
	/// Элемент упорядоченного списка
	/// </summary>
	public class OrderedValue : IComparable<OrderedValue>
	{
		public string ID { private set; get; }
		public bool IsEmpty { private set; get; }
		public decimal Value { private set; get; }
		public decimal ExtraValue { private set; get; }
		public int Rank { private set; get; }

		/// <summary>
		/// примечание, если вдруг потребуется хранить что-то еще
		/// </summary>
		public string Note { set; get; }


		public OrderedValue(string id, decimal value)
		{
			ID = id;
			Value = value;
			ExtraValue = 0;
			IsEmpty = false;
			Rank = 0;
		}

		public OrderedValue(string id, double value)
			: this(id, Convert.ToDecimal(value))
		{
		}

		public OrderedValue(string id, decimal value, decimal extraValue)
			: this(id, value)
		{
			ExtraValue = extraValue;
		}

		public OrderedValue(string id, double value, double extraValue)
			: this(id, Convert.ToDecimal(value), Convert.ToDecimal(extraValue))
		{
		}

		public OrderedValue(string id)
		{
			ID = id;
			Value = 0;
			Rank = 0;
			IsEmpty = true;
		}

		public void SetValue(decimal value)
		{
			IsEmpty = false;
			Value = value;
			Rank = 0;
		}

		public void SetExtraValue(decimal value)
		{
			ExtraValue = value;
		}

		public void SetEmpty()
		{
			IsEmpty = true;
			Value = 0;
			Rank = 0;
		}

		public void SetRank(int rank)
		{
			Rank = rank;
		}

		public int CompareTo(OrderedValue other)
		{
			if (IsEmpty && other.IsEmpty)
				return String.Compare(ID, other.ID, true);
			if (IsEmpty)
				return 1;
			if (other.IsEmpty)
				return -1;
			if (Value < other.Value)
				return 1;
			if (Value > other.Value)
				return -1;
			return String.Compare(ID, other.ID, true);
		}

	}

	public abstract class ChartWrapper : UltraChartWrapper
	{
		protected Color DefaultColor { set; get; }
		protected Color DefaultColorDark { set; get; }
		protected Font DefaultFont { set; get; }
		protected Font DefaultFontSmall { set; get; }

		protected string TitleTop { set; get; }
		protected string TitleLeft { set; get; }
		protected string TitleRight { set; get; }

		protected string ToolTipFormatString { set; get; }

		protected ChartWrapper(UltraChartItem chartItem) 
			: base(chartItem)
		{
			DefaultColor = Color.FromArgb(unchecked((int)0xffD1D1D1));
			DefaultColorDark = Color.FromArgb(unchecked((int)0xff666666));
			DefaultFont = new Font("Verdana", 10);
			DefaultFontSmall = new Font("Verdana", 8);

			TitleTop = String.Empty;
			TitleLeft = String.Empty;
			TitleRight = String.Empty;
			ToolTipFormatString = "<DATA_VALUE:N2>";
		}

		protected override void SetStyle()
		{
			ChartControl.BrowserSizeAdapting = false;
			ChartControl.Width = 740;
			ChartControl.Height = 350;
			ChartControl.Chart.BorderWidth = 0;

			ChartControl.Chart.TitleTop.Text = TitleTop;
			ChartControl.Chart.TitleTop.Visible = !String.IsNullOrEmpty(TitleTop);
			ChartControl.Chart.TitleTop.Font = DefaultFont;
			ChartControl.Chart.TitleTop.FontColor = DefaultColor;
			ChartControl.Chart.TitleTop.HorizontalAlign = StringAlignment.Center;
			ChartControl.Chart.TitleTop.VerticalAlign = StringAlignment.Near;
			ChartControl.Chart.TitleTop.Extent = 15 * TitleTop.Split("\n").Length;

			ChartControl.Chart.TitleLeft.Text = TitleLeft;
			ChartControl.Chart.TitleLeft.Visible = !String.IsNullOrEmpty(TitleLeft);
			ChartControl.Chart.TitleLeft.Font = DefaultFont;
			ChartControl.Chart.TitleLeft.FontColor = DefaultColor;
			ChartControl.Chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
			ChartControl.Chart.TitleLeft.Extent = 20 * TitleLeft.Split("\n").Length + 10;

			ChartControl.Chart.TitleRight.Text = TitleRight;
			ChartControl.Chart.TitleRight.Visible = !String.IsNullOrEmpty(TitleRight);
			ChartControl.Chart.TitleRight.Font = DefaultFont;
			ChartControl.Chart.TitleRight.FontColor = DefaultColor;
			ChartControl.Chart.TitleRight.HorizontalAlign = StringAlignment.Center;
			ChartControl.Chart.TitleRight.Extent = 20 * TitleRight.Split("\n").Length + 10;

			ChartControl.Chart.Tooltips.FormatString = String.Format(
				"<span style='font-family: Arial; font-size: 14pt;'>{0}</span>", ToolTipFormatString);
		}

	}

	public abstract class CompositeChartWrapper : ChartWrapper
	{
		protected int X_Extent { set; get; }
		protected string X_FormatString { set; get; }
		protected int Y_Extent { set; get; }
		protected string Y_FormatString { set; get; }
		protected int Y2_Extent { set; get; }
		protected string Y2_FormatString { set; get; }
		protected string Text1_FormatString { set; get; }
		protected string Text2_FormatString { set; get; }
		protected Color BoxColor { set; get; }
		protected Color IconColor { set; get; }
		protected double LeftMarginPercent { set; get; }
		protected double RightMarginPercent { set; get; }
		protected Rectangle LegendBounds { set; get; }

		protected CompositeChartWrapper(UltraChartItem chartItem)
			: base(chartItem)
		{
			X_Extent = 45;
			X_FormatString = "<ITEM_LABEL>";
			LeftMarginPercent = 0;
			RightMarginPercent = 0;
			Y_Extent = 60;
			Y_FormatString = "<DATA_VALUE:N2>";
			Y2_Extent = 60;
			Y2_FormatString = "<DATA_VALUE:N2>";
			Text1_FormatString = "<DATA_VALUE:N2>";
			Text2_FormatString = "<DATA_VALUE:N2>";
			IconColor = Color.White;
			LegendBounds = new Rectangle(10, 90, 80, 10);
		}
		
		protected override void SetStyle()
		{
			base.SetStyle();

			ChartControl.Chart.ChartType = ChartType.Composite;
			
			ChartArea area = new ChartArea();
			ChartControl.Chart.CompositeChart.ChartAreas.Add(area);
			area.Border.Thickness = 0;

			AxisItem axisX = new AxisItem();
			axisX.Key = "X";
			axisX.OrientationType = AxisNumber.X_Axis;
			axisX.SetLabelAxisType = SetLabelAxisType.GroupBySeries;
			axisX.DataType = AxisDataType.String;
			axisX.LineThickness = 1;
			axisX.LineColor = DefaultColor;
			axisX.Extent = X_Extent;
			axisX.MajorGridLines.Visible = false;
			axisX.Labels.Visible = true;
			axisX.Labels.Orientation = TextOrientation.Horizontal;
			axisX.Labels.Font = DefaultFont;
			axisX.Labels.FontColor = DefaultColor;
			axisX.Labels.ItemFormatString = X_FormatString;
			axisX.Labels.LabelStyle.Dy = 5;

			AxisItem axisY = new AxisItem();
			axisY.Key = "Y";
			axisY.OrientationType = AxisNumber.Y_Axis;
			axisY.DataType = AxisDataType.Numeric;
			axisY.LineThickness = 1;
			axisY.LineColor = DefaultColor;
			axisY.Extent = Y_Extent;
			axisY.MajorGridLines.Color = DefaultColorDark;
			axisY.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
			axisY.Labels.HorizontalAlign = StringAlignment.Far;
			axisY.Labels.ItemFormatString = Y_FormatString;
			axisY.Labels.Font = DefaultFontSmall;
			axisY.Labels.FontColor = DefaultColor;
			axisY.Labels.LabelStyle.Dx = -10;
			axisY.TickmarkStyle = AxisTickStyle.Percentage;
			axisY.TickmarkPercentage = 10;
			axisY.RangeType = AxisRangeType.Custom;
			
			AxisItem axisX2 = new AxisItem();
			axisX2.Key = "X2";
			axisX2.Visible = false;
			axisX2.OrientationType = AxisNumber.X2_Axis;
			axisX2.SetLabelAxisType = SetLabelAxisType.ContinuousData;
			axisX2.DataType = AxisDataType.String;
			axisX2.Margin.Near.MarginType = LocationType.Percentage;
			axisX2.Margin.Near.Value = LeftMarginPercent;
			axisX2.Margin.Far.MarginType = LocationType.Percentage;
			axisX2.Margin.Far.Value = RightMarginPercent;

			AxisItem axisY2 = new AxisItem();
			axisY2.Key = "Y2";
			axisY2.OrientationType = AxisNumber.Y2_Axis;
			axisY2.DataType = AxisDataType.Numeric;
			axisY2.LineThickness = 1;
			axisY2.LineColor = DefaultColor;
			axisY2.Extent = Y2_Extent;
			axisY2.MajorGridLines.Color = DefaultColorDark;
			axisY2.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
			axisY2.Labels.HorizontalAlign = StringAlignment.Far;
			axisY2.Labels.ItemFormatString = Y2_FormatString;
			axisY2.Labels.Font = DefaultFontSmall;
			axisY2.Labels.FontColor = DefaultColor;
			axisY2.Labels.LabelStyle.Dx = 10;
			axisY2.TickmarkStyle = AxisTickStyle.Percentage;
			axisY2.TickmarkPercentage = 10;
			axisY2.RangeType = AxisRangeType.Custom;

			area.Axes.Add(axisX);
			area.Axes.Add(axisY);
			area.Axes.Add(axisX2);
			area.Axes.Add(axisY2);

			ChartLayerAppearance layer1 = new ChartLayerAppearance();
			layer1.ChartType = ChartType.ColumnChart;
			((ColumnChartAppearance)layer1.ChartTypeAppearance).ColumnSpacing = 0;
			((ColumnChartAppearance)layer1.ChartTypeAppearance).SeriesSpacing = 0;
			((ColumnChartAppearance)layer1.ChartTypeAppearance).ChartText.Add(
				new ChartTextAppearance
				{
					Column = -2,
					Row = -2,
					VerticalAlign = StringAlignment.Far,
					HorizontalAlign = StringAlignment.Center,
					ItemFormatString = Text1_FormatString,
					ChartTextFont = DefaultFont,
					FontColor = DefaultColor,
					Visible = Text1_FormatString != String.Empty
				});
			layer1.ChartArea = area;
			layer1.AxisX = axisX;
			layer1.AxisY = axisY;
			layer1.LegendItem = LegendItemType.Series;
			ChartControl.Chart.CompositeChart.ChartLayers.Add(layer1);

			ChartLayerAppearance layer2 = new ChartLayerAppearance();
			layer2.ChartType = ChartType.SplineChart;
			LineAppearance lineAppearance = new LineAppearance();
			lineAppearance.LineStyle.StartStyle = LineCapStyle.RoundAnchor;
			lineAppearance.LineStyle.EndStyle = LineCapStyle.RoundAnchor;
			lineAppearance.IconAppearance.Icon = SymbolIcon.Character;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Large;
			lineAppearance.IconAppearance.Character = '◉';
			lineAppearance.IconAppearance.CharacterFont = new Font("Comic Sans MS", 24, FontStyle.Bold);
			lineAppearance.IconAppearance.PE.ElementType = PaintElementType.CustomBrush;
			lineAppearance.IconAppearance.PE.CustomBrush = new SolidBrush(IconColor);
			lineAppearance.SplineTension = 0.3f;
			lineAppearance.Thickness = 5;
			((LineChartAppearance)layer2.ChartTypeAppearance).LineAppearances.Add(lineAppearance);
			((LineChartAppearance)layer2.ChartTypeAppearance).ChartText.Add(
				new ChartTextAppearance
				{
					Column = -2,
					Row = -2,
					VerticalAlign = StringAlignment.Far,
					HorizontalAlign = StringAlignment.Center,
					ItemFormatString = Text2_FormatString,
					ChartTextFont = DefaultFont,
					FontColor = DefaultColor,
					Visible = Text2_FormatString != String.Empty
				});
			layer2.ChartArea = area;
			layer2.AxisX = axisX2;
			layer2.AxisY = axisY2;
			layer2.LegendItem = LegendItemType.Series;
			ChartControl.Chart.CompositeChart.ChartLayers.Add(layer2);
			
			CompositeLegend compositeLegend = new CompositeLegend();
			compositeLegend.Border.Color = Color.FromArgb(unchecked((int)0xFF151515));
			compositeLegend.Border.Thickness = 3;
			compositeLegend.PE.ElementType = PaintElementType.SolidFill;
			compositeLegend.PE.Fill = Color.FromArgb(unchecked((int)0xFF151515));
			compositeLegend.BoundsMeasureType = MeasureType.Percentage;
			compositeLegend.Bounds = LegendBounds;
			compositeLegend.LabelStyle.Font = DefaultFont;
			compositeLegend.LabelStyle.FontColor = DefaultColor;
			compositeLegend.ChartLayers.Add(layer1);
			compositeLegend.ChartLayers.Add(layer2);
			ChartControl.Chart.CompositeChart.Legends.Add(compositeLegend);
		}

		protected override void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
		{
			Rectangle rect = new Rectangle();
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is Polyline)
				{
					Polyline point = (Polyline) primitive;
					
					if (point.Path != null && point.Path.ToLower().Contains("legend"))
					{
						point.Visible = false;
						rect = new Rectangle();
						rect.X = point.points[0].point.X;
						rect.Y = point.points[0].point.Y;
						//rect.Width = point.points[point.points.Length - 1].point.X - rect.X;
						//rect.Height = point.points[point.points.Length - 1].point.Y - rect.Y;
						rect.X -= 7;
						rect.Y -= 9;
						rect.Width = 24;
						rect.Height = 20;
					}
				}
			}
			
			e.SceneGraph.Add(new Text(rect, "◉", new LabelStyle { Font = new Font("Comic Sans MS", 24, FontStyle.Bold), FontColor = IconColor }));
		}
	}

	public class ChartBoxesWrapper : ChartWrapper
	{
		protected Color ColorBox { set; get; }
		protected ItemLabelType LabelType { set; get; }

		protected const int LINE_SIZE = 50;
		protected const int BLOCKS_PER_LINE = 5;
		protected const int BLOCK_SIZE = 9;

		public enum ItemLabelType
		{
			None, Area, Year
		}

		public ChartBoxesWrapper(UltraChartItem chartItem)
			: base(chartItem)
		{
			ColorBox = Color.Red;
			LabelType = ItemLabelType.None;
		}

		protected override void SetStyle()
		{
			// настройка общего стиля
			ToolTipFormatString = "<ITEM_LABEL>";
			base.SetStyle();

			// настройка индивидуального стиля

			ChartControl.Width = 360;
			ChartControl.Height = ChartControl.Chart.TitleTop.Extent + 28 + LINE_SIZE * Table.Rows.Count;
			ChartControl.ZeroAligned = true;
			ChartControl.LegendVisible = false;

			ChartControl.Chart.ChartType = ChartType.BarChart;

			ChartControl.Chart.Axis.X.Visible = false;

			ChartControl.Chart.Axis.Y.Visible = true;
			ChartControl.Chart.Axis.Y.Extent = 100;
			ChartControl.Chart.Axis.Y.MajorGridLines.Visible = false;
			ChartControl.Chart.Axis.Y.LineThickness = 0;
			ChartControl.Chart.Axis.Y.Labels.Visible = true;
			ChartControl.Chart.Axis.Y.Labels.Layout.Padding = 10;
			ChartControl.Chart.Axis.Y.Labels.LabelStyle.Dy = 2;
			ChartControl.Chart.Axis.Y.Labels.FontSizeBestFit = false;
			ChartControl.Chart.Axis.Y.Labels.Font = new Font("Verdana", 10, FontStyle.Bold);
			ChartControl.Chart.Axis.Y.Labels.FontColor = DefaultColor;
			ChartControl.Chart.Axis.Y.Labels.ItemFormatString = "<ITEM_LABEL>";
			ChartControl.Chart.Axis.Y.Labels.SeriesLabels.Visible = false;

		}

		protected override void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
		{
			Collection<Box> itemsList = new Collection<Box>();
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];

				if (primitive is Box)
				{
					Box item = (Box)primitive;
					if (item.DataPoint != null)
					{
						itemsList.Add(item);
					}
				}
			}

			foreach (Box box in itemsList)
			{
				box.Visible = false;

				double value = Convert.ToDouble(box.Value);

				box.rect.Y = box.rect.Y + (LINE_SIZE - BLOCKS_PER_LINE * BLOCK_SIZE) / 2;
				box.rect.Width = Convert.ToInt32(Math.Ceiling(value / BLOCKS_PER_LINE) * BLOCK_SIZE);
				box.rect.Height = BLOCKS_PER_LINE * BLOCK_SIZE;

				DrawBoxes(e.SceneGraph, box.rect.X, box.rect.Y, value);

				e.SceneGraph.Add(
					new Text(
						new Point(box.rect.Right + 10, box.rect.Top + box.rect.Height / 2),
						value.ToString("N2"),
						new LabelStyle
						{
							Font = new Font("Arial", 12, FontStyle.Bold),
							FontColor = Color.White
						}
					));

				box.DataPoint.Label = String.Format(
					"&nbsp;{0}&nbsp;<br />&nbsp;{1}&nbsp;<br />&nbsp;<b>{2}</b>&nbsp;чел.&nbsp;",
					box.Series.Label,
					LabelType == ItemLabelType.Area 
						? RegionsNamingHelper.FullName(box.DataPoint.Label)
						: LabelType == ItemLabelType.Year
							? box.DataPoint.Label + " год"
							: box.DataPoint.Label,
					String.Format("{0:N2}", box.Value));
			}

		}

		private void DrawBoxes(Infragistics.UltraChart.Core.SceneGraph sceneGraph, int xStart, int yStart, double value)
		{
			for (int rowIndex = 0; rowIndex < Math.Ceiling(value) / BLOCKS_PER_LINE; rowIndex++)
			{
				for (int columnIndex = 0; columnIndex < BLOCKS_PER_LINE; columnIndex++)
				{
					Box box = new Box(new Point(xStart + rowIndex * BLOCK_SIZE, yStart + columnIndex * BLOCK_SIZE), BLOCK_SIZE, BLOCK_SIZE);
					box.PE.ElementType = PaintElementType.SolidFill;
					box.PE.Fill = ColorBox;
					box.PE.FillOpacity = 250;

					if (rowIndex * BLOCKS_PER_LINE + columnIndex >= Convert.ToInt32(Math.Floor(value)))
					{
						if (rowIndex * BLOCKS_PER_LINE + columnIndex > Convert.ToInt32(Math.Floor(value))
							|| Math.Floor(value).ToString("N2") == value.ToString("N2"))
						{
							break;
						}
						box.PE.FillOpacity = Convert.ToByte(50 + (value - Math.Floor(value)) * 150);
					}
					sceneGraph.Add(box);
				}
			}
		}

	}

	public class SporterPieChart : UltraChartWrapper
	{
		private double mainValue;

		public SporterPieChart(UltraChartItem chart, string temporaryUrlPrefix, double value)
			: base(chart)
		{
			TemporaryUrlPrefix = temporaryUrlPrefix;
			mainValue = value;
		}

		protected override void SetStyle()
		{
			ChartControl.BrowserSizeAdapting = false;
			ChartControl.Width = 30;
			ChartControl.Height = 30;
			ChartControl.Chart.ChartType = ChartType.PieChart;

			ChartControl.LegendVisible = false;

			ChartControl.Chart.BackColor = Color.Black;

			ChartControl.Chart.PieChart.RadiusFactor = 100;
			ChartControl.Chart.PieChart.StartAngle = 270;
			ChartControl.Chart.PieChart.Labels.Visible = false;
			ChartControl.Chart.PieChart.Labels.LeaderLinesVisible = false;

			// цвета
			ChartControl.Chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
			ChartControl.Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(Color.LawnGreen, 100));
			ChartControl.Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(Color.LightSkyBlue, 200));
			ChartControl.Chart.Effects.Effects.Clear();
			GradientEffect effect = new GradientEffect();
			effect.Coloring = GradientColoringStyle.Darken;
			effect.Enabled = true;
			ChartControl.Chart.Effects.Enabled = true;
			ChartControl.Chart.Effects.Effects.Add(effect);

		}

		protected override void SetData()
		{
			DataTable table = new DataTable();
			table.Columns.Add(new DataColumn("column0", typeof(string)));
			table.Columns.Add(new DataColumn("column1", typeof(double)));

			DataRow row = table.NewRow();
			row[0] = "0";
			row[1] = mainValue;
			table.Rows.Add(row);

			row = table.NewRow();
			row[0] = "1";
			row[1] = 1 - mainValue;
			table.Rows.Add(row);

			ChartControl.Chart.DataSource = table;
			ChartControl.Chart.DataBind();
		}

		public void SaveTo(string filename)
		{
			string serverPath = String.Format("{0}{1}", HttpContext.Current.Server.MapPath("~/TemporaryImages/"), filename);
			if (File.Exists(serverPath))
				return;

			MemoryStream stream = new MemoryStream();
			ChartControl.Chart.SaveTo(stream, ImageFormat.Png);
			Bitmap bmp = new Bitmap(stream);
			Bitmap copyBmp = bmp.Clone(new Rectangle(1, 1, bmp.Width - 1, bmp.Height - 1), bmp.PixelFormat);
			copyBmp.Save(serverPath);
		}
	}

	public class RecreationPieChart : UltraChartWrapper
	{
		private decimal mainValue;
		private Color color;

		public RecreationPieChart(UltraChartItem chart, string temporaryUrlPrefix, decimal value, Color color)
			: base(chart)
		{
			TemporaryUrlPrefix = temporaryUrlPrefix;
			mainValue = value > 1 ? 1 : value;
			this.color = color;
		}

		protected override void SetStyle()
		{
			ChartControl.BrowserSizeAdapting = false;
			ChartControl.Width = 30;
			ChartControl.Height = 30;
			ChartControl.Chart.ChartType = ChartType.PieChart;

			ChartControl.LegendVisible = false;

			ChartControl.Chart.BackColor = Color.Black;

			ChartControl.Chart.PieChart.RadiusFactor = 100;
			ChartControl.Chart.PieChart.StartAngle = 270;
			ChartControl.Chart.PieChart.Labels.Visible = false;
			ChartControl.Chart.PieChart.Labels.LeaderLinesVisible = false;

			// цвета
			ChartControl.Chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
			ChartControl.Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color, 200));
			ChartControl.Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(Color.WhiteSmoke, 125));
			ChartControl.Chart.Effects.Effects.Clear();
			GradientEffect effect = new GradientEffect();
			effect.Coloring = GradientColoringStyle.Darken;
			effect.Enabled = false;
			ChartControl.Chart.Effects.Enabled = true;
			ChartControl.Chart.Effects.Effects.Add(effect);

		}

		protected override void SetData()
		{
			DataTable table = new DataTable();
			table.Columns.Add(new DataColumn("column0", typeof(string)));
			table.Columns.Add(new DataColumn("column1", typeof(double)));

			DataRow row = table.NewRow();
			row[0] = "0";
			row[1] = mainValue;
			table.Rows.Add(row);

			row = table.NewRow();
			row[0] = "1";
			row[1] = 1 - mainValue;
			table.Rows.Add(row);

			ChartControl.Chart.DataSource = table;
			ChartControl.Chart.DataBind();
		}

		public void SaveTo(string filename)
		{
			string serverPath = String.Format("{0}{1}", HttpContext.Current.Server.MapPath("~/TemporaryImages/"), filename);
			if (File.Exists(serverPath))
				return;

			MemoryStream stream = new MemoryStream();
			ChartControl.Chart.SaveTo(stream, ImageFormat.Png);
			Bitmap bmp = new Bitmap(stream);
			Bitmap copyBmp = bmp.Clone(new Rectangle(1, 1, bmp.Width - 1, bmp.Height - 1), bmp.PixelFormat);
			copyBmp.Save(serverPath);
		}
	}
}