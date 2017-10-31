using System;
using System.Collections.Generic;
using System.Data;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	// диаграммы по участкам границы
	
	public class ChartBordersGoods : ChartBordersBase
	{
		public ChartBordersGoods(UltraChart chart)
			: base(chart)
		{
			TextBottom = "единиц";
			TextHints = "Досмотрено партий грузов";
			BottomExtent = 40;
			LabelsExtent = 28;
		}
	}

	public class ChartBordersPeople : ChartBordersBase
	{
		public ChartBordersPeople(UltraChart chart)
			: base(chart)
		{
			TextBottom = "человек";
			TextHints = "Досмотрено лиц";
			BottomExtent = 50;
			LabelsExtent = 28;
		}
	}

	public class ChartBordersTransport : ChartBordersBase
	{
		public ChartBordersTransport(UltraChart chart)
			: base(chart)
		{
			TextBottom = "единиц";
			TextHints = "Досмотрено ТС";
			BottomExtent = 40;
			LabelsExtent = 28;
		}
	}

	public abstract class ChartBordersBase : ChartBase
	{

		protected Dictionary<string, string> ChartBorderVal2Percent;
		protected Dictionary<string, string> ChartBorderName2Val;

		protected ChartBordersBase(UltraChart chart) 
			: base(chart)
		{
			ChartBorderVal2Percent = new Dictionary<string, string>();
			ChartBorderName2Val = new Dictionary<string, string>();
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle()
		{
			SetStyle(SKKHelper.default2ItemsWidth, SKKHelper.defaultChartBordersHeight);
		}

		/// <summary>
		/// настройка внешнего вида
		/// </summary>
		public override void SetStyle(double width, double height)
		{
			base.SetStyle(width, height);

			Chart.ChartType = ChartType.BarChart;

			SetCommonHorizontalAxis(BottomExtent, 180);
			SetCommonHorizontalText(Chart.BarChart.ChartText, MarkFormat);
			
			CRHelper.FillCustomColorModelLight(Chart, SKKHelper.maxBordersCount, false);
			
			Chart.TitleBottom.Visible = true;
			Chart.TitleBottom.Text = TextBottom;
			Chart.Tooltips.FormatString = "<ITEM_LABEL>";
			
			Chart.FillSceneGraph += FillSceneGraph;
		}

		/// <summary>
		/// установить данные
		/// </summary>
		public override void SetData(string queryName)
		{
			DataTable dtChart = (new Query(queryName)).GetDataTable();
			
			DataPostProcessing(dtChart); 
		}

		/// <summary>
		/// постобработка данных, обязательно должна выполняться
		/// </summary>
		protected virtual void DataPostProcessing(DataTable dtChart)
		{
			if (dtChart.Rows.Count > 0)
			{
				SetExtraData(dtChart);

				Chart.Series.Clear();
				Chart.Series.Add(CRHelper.GetNumericSeries(1, dtChart));

				FitHeight(dtChart.Rows.Count);
			}
			else
			{
				SetChartHeight(SKKHelper.defaultChartHeight);
			}
		}

		/// <summary>
		/// установить доп данные (%)
		/// </summary>
		protected void SetExtraData(DataTable dtChart)
		{
			double max = 0;
			foreach (DataRow row in dtChart.Rows)
			{
				double value;
				double percent;

				if (!Double.TryParse(row[1].ToString(), out value))
				{
					value = 0;
				}
				if (!Double.TryParse(row[2].ToString(), out percent))
				{
					percent = 0;
				}

				if (value > max)
				{
					max = value;
				}

				string valueStr = value.ToString(MarkFormat);
				if (!ChartBorderName2Val.ContainsKey(row[0].ToString()))
				{
					ChartBorderName2Val.Add(row[0].ToString(), valueStr);
				}
				if ((value > 0) && !ChartBorderVal2Percent.ContainsKey(valueStr))
				{
					ChartBorderVal2Percent.Add(valueStr, String.Format("{0:P2}", percent));
				}
			}
		}

		/// <summary>
		/// подгон размеров под данные
		/// </summary>
		protected virtual void FitHeight(int countRows)
		{
			if (countRows < SKKHelper.maxBordersCount)
			{
				double oldHeight = ChartHeight;
				double newHeight = oldHeight + BottomExtent - 50 - (SKKHelper.maxBordersCount - countRows) * minRowHeight;
				double oldRealHeight = Chart.Height.Value;
				SetChartHeight(newHeight);
				double newRealHeight = Chart.Height.Value;

				// общая высота
				if (newHeight < SKKHelper.defaultChartHeight)
				{
					SetChartHeight(SKKHelper.defaultChartHeight);
					oldRealHeight = Chart.Height.Value;
				}
				else
				{
					SetChartHeight(oldHeight);
				}

				// отступ снизу
				Chart.TitleBottom.Extent += Convert.ToInt32(oldRealHeight - newRealHeight);
			}
		}

		/// <summary>
		/// нет данных
		/// </summary>
		public override void InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
		{
			base.InvalidDataReceived(sender, e);
			e.Text = TitleNoData;
		}

		/// <summary>
		/// доп обработка
		/// </summary>
		private void FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];

				if (primitive is Text && primitive.Path == null)
				{
					// подписи на диаграмме
					Text text = (Text)primitive;
					if (ChartBorderVal2Percent.ContainsKey(text.GetTextString()))
					{
						string percent = ChartBorderVal2Percent[text.GetTextString()];
						text.SetTextString(String.Format("{0} ({1})" , text.GetTextString(), percent));
						text.labelStyle.Dx = 60 + (percent.Length-5) * 8;
					}
				}

				if (primitive is Box)
				{
					Box box = (Box)primitive;
					if (box.DataPoint != null)
					{
						if (ChartBorderName2Val.ContainsKey(box.DataPoint.Label))
						{
							string value = ChartBorderName2Val[box.DataPoint.Label];
							string percent = ChartBorderVal2Percent.ContainsKey(value) ? ChartBorderVal2Percent[value] : String.Empty;

							box.DataPoint.Label =
								String.Format("&nbsp;{0}{1},&nbsp;\n&nbsp;{2}: <b>{3}{4}</b>&nbsp;\n&nbsp;<b>{5}</b>&nbsp;",
									box.DataPoint.Label,
									box.DataPoint.Label.ToLower().Contains("не указан") ? "" : " участок",
									TextHints,
									value,
									MarkSI == String.Empty ? String.Empty : " " + MarkSI,
									percent
									);
						}
					}
				}
			}			
		}


	}
}
