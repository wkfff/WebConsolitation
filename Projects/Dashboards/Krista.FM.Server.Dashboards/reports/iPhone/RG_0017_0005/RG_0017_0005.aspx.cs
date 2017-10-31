using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
	/// <summary>
	/// Название: Социологические опросы по ЯНАО, детализация по доверию местной власти
	/// Описание: Анализ данных социологических опросов населения Ямало-Ненецкого автономного округа
	/// Кубы: РЕГИОН_Данные опросов 
	/// </summary>
    public partial class RG_0017_0005 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "RG_0017_0005";
		
		// параметры запросов
		private CustomParam customParamPeriod;
		private CustomParam customParamTerritory;
		private CustomParam customParamMark;
		private CustomParam customParamPositive;
		private CustomParam customParamNegative;
		

		private int paramYear;
		private string paramMonth;
		private int paramTerritoryId;
		private string paramTerritory;
		private string paramPositive;
		private string paramNegative;

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			
			// Инициализация параметров запроса
			customParamPeriod = UserParams.CustomParam("param_period");
			customParamTerritory = UserParams.CustomParam("param_territory");

			customParamMark = UserParams.CustomParam("param_mark");
			customParamPositive = UserParams.CustomParam("param_positive");
			customParamNegative = UserParams.CustomParam("param_negative");
		}

    	protected override void Page_Load(object sender, EventArgs e)
        {
			base.Page_Load(sender, e);
			SetupParams();
    		GenerateReport();

        }
		
		private void SetupParams()
		{
			// значения по умолчанию
			paramYear = 2010;
			paramMonth = "январь";
			paramTerritory = UserParams.StateArea.Value;
			paramTerritoryId = Int32.Parse(CustomParams.GetSubjectIdByName(paramTerritory));
			
			// территория
			customParamTerritory.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
			
			// последний месяц-год, на который есть данные
			DataTable table = DataProvider.GetDataTableForChart("RG_0017_0005_last_period", DataProvidersFactory.PrimaryMASDataProvider);
			try
			{
				string period = table.Rows[0][0].ToString();
				// [].[].[Данные всех периодов].[2011].[Полугодие 2].[Квартал 4].[Октябрь]
				
				MatchCollection matches = Regex.Matches(period, @"\[([^\]]*)\]");
				if (matches.Count > 3)
					paramYear = Convert.ToInt32(matches[3].Groups[1].Value);
				if (matches.Count > 6)
					paramMonth = matches[6].Groups[1].Value;

				customParamPeriod.Value = period;
			}
			catch (Exception)
			{
				throw new Exception("Необходимые данные в БД не найдены");
			}
			
		}

		private void GenerateReport()
		{
			Title.InnerHtml = "Индекс доверия местной власти";
			TitleSub.InnerHtml = String.Format(
				"по данным социологических опросов за {0} {1} года по {2}",
				paramMonth.ToLower(), paramYear, RegionsNamingHelper.ShortName(paramTerritory));

			Header1.Text = "Индекс восприятия уровня распространенности коррупции";
			customParamMark.Value = "[СоцОпрос__Показатели].[СоцОпрос__Показатели].[Все показатели].[Индекс восприятия уровня распространенности коррупции]";
			customParamPositive.Value = paramPositive = "Не распространена";
			customParamNegative.Value = paramNegative = "Распространена";
			GenerateGovernment(UltraChartGovernment1);

			Header2.Text = "Индекс доверия главам муниципальных образований";
			customParamMark.Value = "[СоцОпрос__Показатели].[СоцОпрос__Показатели].[Все показатели].[Индекс доверия главам]";
			customParamPositive.Value = paramPositive = "Доверяю";
			customParamNegative.Value = paramNegative = "Не доверяю";
			GenerateGovernment(UltraChartGovernment2);

			Header3.Text = "Индекс доверия Думе<br />городского округа / муниципального района";
			customParamMark.Value = "[СоцОпрос__Показатели].[СоцОпрос__Показатели].[Все показатели].[Индекс доверия Думе]";
			customParamPositive.Value = paramPositive = "Доверяю";
			customParamNegative.Value = paramNegative = "Не доверяю";
			GenerateGovernment(UltraChartGovernment3);

			Header4.Text = "Индекс доверия председателю Думы<br />городского округа / муниципального района";
			customParamMark.Value = "[СоцОпрос__Показатели].[СоцОпрос__Показатели].[Все показатели].[Индекс доверия председателю Думы]";
			customParamPositive.Value = paramPositive = "Доверяю";
			customParamNegative.Value = paramNegative = "Не доверяю";
			GenerateGovernment(UltraChartGovernment4);
		}

		private void GenerateGovernment(UltraChartItem chartItem)
		{
			ChartSymmetry chartGov = new ChartSymmetry(chartItem);
			chartGov.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
			chartGov.ReportID = REPORT_ID;
			chartGov.TextPositive = paramPositive;
			chartGov.TextNegative = paramNegative;
			chartGov.Table = DataProvider.GetDataTableForChart("RG_0017_0005_government", DataProvidersFactory.PrimaryMASDataProvider);
			chartGov.SetStyleAndData();
		}
		
		private class ChartSymmetry : UltraChartWrapper
		{
			private string ToolTipFormatString { set; get; }
			private int RangeFull { set; get; }
			private Font FontText { set; get; }
			private Color FontColor { set; get; }
			public string TextPositive { set; get; }
			private Color ColorPositive { set; get; }
			public string TextNegative { set; get; }
			private Color ColorNegative { set; get; }

			public ChartSymmetry(UltraChartItem chartItem)
				: base(chartItem)
			{
				DefaultColor = Color.FromArgb(unchecked((int) 0xffD1D1D1));
				DefaultColorDark = Color.FromArgb(unchecked((int) 0xff666666));
				DefaultFont = new Font("Verdana", 10);
				DefaultFontSmall = new Font("Verdana", 8);

				ToolTipFormatString = "<ITEM_LABEL>";

				FontText = new Font("Arial", 13);
				FontColor = Color.FromArgb(255, 218, 218, 218);
				ColorNegative = Color.FromArgb(255, 216, 0, 0);
				ColorPositive = Color.ForestGreen;
			}

			public new void SetStyleAndData()
			{
				Table.InvertRowsOrder();
				foreach (DataRow row in Table.Rows)
				{
					row[0] = row[0].ToString()
						.Replace("title", "&nbsp;")
						.Replace("(Ямало-Ненецкий автономный округ ДАННЫЕ)", "Все")
						.Replace("муниципальный район", "МР")
						.Replace("Город", "г.");
				}

				RangeFull = Convert.ToInt32(Table.Rows[0]["Максимум общий"]);
				
				SetStyle();

				ChartControl.LegendVisible = false;
				ChartControl.BrowserSizeAdapting = false;
				ChartControl.ZeroAligned = true;
				ChartControl.Width = 755;
				ChartControl.Height = 25 * Table.Rows.Count + 28;
				ChartControl.Chart.BorderWidth = 0;
				ChartControl.Chart.Tooltips.FormatString = String.Format(
					"<span style='font-family: Arial; font-size: 14pt;'>{0}</span>", ToolTipFormatString);

				ChartControl.Chart.ChartType = ChartType.BarChart;
				
				ChartControl.Chart.Axis.X.Visible = true;
				ChartControl.Chart.Axis.X.Extent = 0;
				ChartControl.Chart.Axis.X.LineThickness = 0;
				ChartControl.Chart.Axis.X.MajorGridLines.Visible = false;
				ChartControl.Chart.Axis.X.Labels.Visible = false;
				ChartControl.Chart.Axis.X.Labels.SeriesLabels.Visible = false;
				ChartControl.Chart.Axis.X.RangeType = AxisRangeType.Custom;
				ChartControl.Chart.Axis.X.RangeMin = 0;
				ChartControl.Chart.Axis.X.RangeMax = RangeFull;

				ChartControl.Chart.Axis.Y.Visible = true;
				ChartControl.Chart.Axis.Y.Extent = 0;
				ChartControl.Chart.Axis.Y.LineThickness = 0;
				ChartControl.Chart.Axis.Y.MajorGridLines.Visible = false;
				ChartControl.Chart.Axis.Y.Labels.Visible = false;
				ChartControl.Chart.Axis.Y.Labels.SeriesLabels.Visible = false;

				// привязка данных
				NumericSeries series = CRHelper.GetNumericSeries(1, Table);
				series.Label = Table.Columns[1].Caption;
				ChartControl.Chart.Series.Add(series);
			}

			protected override void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
			{
				Collection<Box> boxes = new Collection<Box>();
				for (int i = 0; i < e.SceneGraph.Count; i++)
				{
					Primitive primitive = e.SceneGraph[i];
					
					if (primitive is Box)
					{
						Box item = (Box)primitive;
						if (item.DataPoint != null)
						{
							boxes.Add(item);
						}
					}
				}

				for (int i = 0; i < boxes.Count; i++)
				{
					Box box = boxes[i];
					
					double xScale = Convert.ToDouble(box.rect.Width) / RangeFull;
					int xAvg = box.rect.Left + box.rect.Width / 2;

					string valTitle = Table.Rows[i][0].ToString();
					int valIndex = Convert.ToInt32(Table.Rows[i]["Индекс"]);
					int valUnknown = Convert.ToInt32(Table.Rows[i]["Затрудняюсь ответить"]);
					int valPositive = Convert.ToInt32(Table.Rows[i][TextPositive]);
					int valNegative = Convert.ToInt32(Table.Rows[i][TextNegative]);

					// не доверяют
					int widthNegative = Convert.ToInt32(valNegative * xScale);
					Box boxNegative = new Box(new Point(xAvg - widthNegative, box.rect.Y), widthNegative, box.rect.Height);
					boxNegative.PE.ElementType = PaintElementType.SolidFill;
					boxNegative.PE.Fill = ColorNegative;
					boxNegative.PE.FillOpacity = 250;
					e.SceneGraph.Add(boxNegative);

					// доверяют
					int widthPositive = Convert.ToInt32(valPositive * xScale);
					Box boxPositive = new Box(new Point(xAvg, box.rect.Y), widthPositive, box.rect.Height);
					boxPositive.PE.ElementType = PaintElementType.SolidFill;
					boxPositive.PE.Fill = ColorPositive;
					boxPositive.PE.FillOpacity = 250;
					e.SceneGraph.Add(boxPositive);
					
					// текст
					Text text = new Text();
					text.SetTextString(String.Format("{0} ({1})", valTitle, valIndex));
					text.bounds = new Rectangle(box.rect.X, box.rect.Y, box.rect.Width, box.rect.Height);
					text.labelStyle.HorizontalAlign = StringAlignment.Center;
					text.labelStyle.Font = FontText;
					text.PE.ElementType = PaintElementType.CustomBrush;
					text.PE.CustomBrush = new SolidBrush(FontColor);
					e.SceneGraph.Add(text);

					// тултип
					box.Visible = false;
					box.rect.X = xAvg - widthNegative;
					box.rect.Width = widthNegative + widthPositive;

					StringBuilder tooltip = new StringBuilder();
					tooltip.AppendFormat("&nbsp;<b>{0}</b>&nbsp;<br />", box.DataPoint.Label);
					tooltip.AppendFormat("&nbsp;<span style='color: green;'>{0}&nbsp;—&nbsp;<b>{1}%</b></span>&nbsp;опрошенных&nbsp;<br />", TextPositive, valPositive);
					tooltip.AppendFormat("&nbsp;<span style='color: red;'>{0}&nbsp;—&nbsp;<b>{1}%</b></span>&nbsp;опрошенных&nbsp;<br />", TextNegative, valNegative);
					tooltip.AppendFormat("&nbsp;Затрудняюсь ответить&nbsp;—&nbsp;<b>{0}%</b>&nbsp;опрошенных&nbsp;<br />", valUnknown);
					tooltip.AppendFormat("&nbsp;Индекс&nbsp;<b>{0}</b>&nbsp;", valIndex);
					box.DataPoint.Label = tooltip.ToString();
				}
			}
		}

    }

}