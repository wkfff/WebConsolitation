using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web.UI.HtmlControls;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
	/// <summary>
	/// Название: Социологические опросы по ЯНАО, оригинальный
	/// Описание: Анализ данных социологических опросов населения Ямало-Ненецкого автономного округа
	/// Кубы: РЕГИОН_Данные опросов 
	/// </summary>
    public partial class RG_0017_0001 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "RG_0017_0001";
		
		// параметры запросов
		private CustomParam customParamPeriod;
		private CustomParam customParamTerritory;

		private int paramYear;
		private string paramMonth;
		private int paramTerritoryId;
		private string paramTerritory;


		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			
			// Инициализация параметров запроса
			customParamPeriod = UserParams.CustomParam("param_period");
			customParamTerritory = UserParams.CustomParam("param_territory");
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
			DataTable table = DataProvider.GetDataTableForChart("RG_0017_0001_last_period", DataProvidersFactory.PrimaryMASDataProvider);
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
			Title.InnerHtml = "Политические показатели";
			TitleSub.InnerHtml = String.Format(
				"по данным социологических опросов за {0} {1} года по {2}",
				paramMonth.ToLower(), paramYear, RegionsNamingHelper.ShortName(paramTerritory));

			Header_Politician.Text = "Индекс доверия политикам";
			Header_Politician.MultitouchReport = String.Format("RG_0017_0002_{0}", paramTerritoryId);
			GeneratePolitician();

			Header_Electorate.Text = "Электоральные предпочтения";
			Header_Electorate.MultitouchReport = String.Format("RG_0017_0003_{0}", paramTerritoryId);
			GenerateElectorate();

			Header_Power.Text = "Индекс доверия институтам власти";
			Header_Power.MultitouchReport = String.Format("RG_0017_0004_{0}", paramTerritoryId);
			GeneratePower();

			Header_Government.Text = "Индексы доверия местной власти";
			Header_Government.MultitouchReport = String.Format("RG_0017_0005_{0}", paramTerritoryId);
			GenerateGovernment();

			// TouchElementBounds
			string filename = String.Format("{0}TouchElementBounds.xml", HttpContext.Current.Server.MapPath("~/TemporaryImages/"));
			StringBuilder xml = new StringBuilder();
			xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<touchElements>\n");
			xml.AppendFormat("<element id=\"RG_0017_0002_{0}\" bounds=\"x=0;y=110;width=768;height=400\" openMode=\"\"/>\n", paramTerritoryId);
			xml.AppendFormat("<element id=\"RG_0017_0003_{0}\" bounds=\"x=0;y=565;width=768;height=160\" openMode=\"\"/>\n", paramTerritoryId);
			xml.AppendFormat("<element id=\"RG_0017_0004_{0}\" bounds=\"x=0;y=775;width=768;height=190\" openMode=\"\"/>\n", paramTerritoryId);
			xml.AppendFormat("<element id=\"RG_0017_0005_{0}\" bounds=\"x=0;y=1020;width=768;height=110\" openMode=\"\"/>\n", paramTerritoryId);
			xml.Append("</touchElements>");
			File.WriteAllText(filename, xml.ToString());
		}

		private void GeneratePolitician()
		{
			DataTable table = DataProvider.GetDataTableForChart("RG_0017_0001_politician", DataProvidersFactory.PrimaryMASDataProvider);
			table.PrimaryKey = new[] {table.Columns[0]};
			
			// Путина с Медведевым на первые места

			DataRow rowPutin = table.Rows.Find("В.В. Путин");
			DataRow rowPutinNew = table.NewRow();
			rowPutinNew.ItemArray = rowPutin.ItemArray;
			table.Rows.Remove(rowPutin);

			DataRow rowMedvedev = table.Rows.Find("Д.А. Медведев");
			DataRow rowMedvedevNew = table.NewRow();
			rowMedvedevNew.ItemArray = rowMedvedev.ItemArray;
			table.Rows.Remove(rowMedvedev);

			table.Rows.InsertAt(rowMedvedevNew, 0);
			if (Convert.ToDouble(rowPutinNew["Индекс"]) >= Convert.ToDouble(rowMedvedevNew["Индекс"]))
				table.Rows.InsertAt(rowPutinNew, 0);
			else
				table.Rows.InsertAt(rowPutinNew, 1);
			

			for (int i = 0; i < table.Rows.Count; i++)
			{
				DataRow row = table.Rows[i];
				
				HtmlGenericControl div = new HtmlGenericControl("div");
				div.ID = String.Format("Politician_{0}", i);

				div.Style.Add("display", "inline-block");
				div.Style.Add("width", "130px");
				div.Style.Add("margin", "10px");
				div.Style.Add("text-align", "center");

				StringBuilder html = new StringBuilder();
				html.AppendFormat("<img src=\"images\\pol_{0}_{1}.jpg\" style=\"margin: 5px 0; border: 1px solid #777777;\" /><br />", paramTerritoryId, row["pkid"]);
				html.AppendFormat("<span class=\"TextWhite TextLarge\">{0}</span><br />", row["title"]);
				html.AppendFormat("<span class=\"TextWhite TextXXLarge {0}\">{1}</span><br />",
					(decimal)row["Индекс"] < 0 ? "ShadowRed" : (decimal)row["Индекс"] > 0 ? "ShadowGreen" : String.Empty, 
					row["Индекс"]);
				div.InnerHtml = html.ToString();

				StringBuilder tooltip = new StringBuilder();
				tooltip.AppendFormat("&nbsp;<b>{0}</b>&nbsp;—&nbsp;<br />&nbsp;&nbsp;&nbsp;&nbsp;{1}&nbsp;<br />", row["ФИО"], row["Должность"]);
				tooltip.AppendFormat("&nbsp;<span style=\"color: green;\">Доверяю&nbsp;—&nbsp;<b>{0}%</b></span>&nbsp;опрошенных&nbsp;<br />", row["Доверяют"]);
				tooltip.AppendFormat("&nbsp;<span style=\"color: red;\">Не доверяю&nbsp;—&nbsp;<b>{0}%</b></span>&nbsp;опрошенных&nbsp;<br />", row["Не доверяют"]);
				tooltip.AppendFormat("&nbsp;Индекс доверия&nbsp;<b>{0}</b>&nbsp;", row["Индекс"]);
				TooltipHelper.AddToolTip(div, String.Format("<span style=\"font-family: Arial; font-size: 14pt;\">{0}</span>", tooltip), Page);

				Body_Politician.Controls.Add(div);
			}
		}

		private void GenerateElectorate()
		{
			DataTable table = DataProvider.GetDataTableForChart("RG_0017_0001_electorate", DataProvidersFactory.PrimaryMASDataProvider);

			TagCloud_Electorate.startFontSize = 8;
			TagCloud_Electorate.groupCount = 5;
			TagCloud_Electorate.fontStep = 3;
			TagCloud_Electorate.floatStyle = FloatStyle.None;
			TagCloud_Electorate.displayStyle = "inline-block";
			TagCloud_Electorate.whiteSpaceStyle = "nowrap";

			Dictionary<string, Tag> tags = new Dictionary<string, Tag>();
			foreach (DataRow row in table.Rows)
			{
				string imageTag = String.Empty;
				string imageFile = String.Format("party_{0}_{1}.png", paramTerritoryId, row["pkid"]);
				string imageFolder = HttpContext.Current.Request.PhysicalPath;
				imageFolder = String.Format("{0}\\images\\", imageFolder.Remove(imageFolder.LastIndexOf("\\")));
				
				if (File.Exists(imageFolder + imageFile))
				{
					imageTag = String.Format("<img src=\"images/{0}\" style=\"padding-left: 5px; vertical-align: middle;\"/>", imageFile);
				}
				
				Tag tag = new Tag();
				tag.weight = Convert.ToInt32(row["Значение"]);
				tag.key = String.Format("{0} (<span style=\"color: white; font-weight: bold;\">{1:P0}</span>){2}", row["title"], Convert.ToDecimal(row["Значение"]) / 100, imageTag);

				StringBuilder tooltip = new StringBuilder();
				tooltip.AppendFormat("&nbsp;<b>{0}</b>&nbsp;<br />", row["title"]);
				tooltip.AppendFormat("&nbsp;<b>{0}%</b>&nbsp;опрошенных&nbsp;", row["Значение"]);
				tag.toolTip = String.Format("<span style=\"font-family: Arial; font-size: 14pt;\">{0}</span>", tooltip);
				tags.Add(tag.key, tag);
			}

			TagCloud_Electorate.ForeColor = Color.White;
			TagCloud_Electorate.Render(tags);
		}
		
		private void GeneratePower()
		{
			ChartPower power = new ChartPower(UltraChartPower);
			power.TemporaryUrlPrefix = TEMPORARY_URL_PREFIX;
			power.ReportID = REPORT_ID;
			power.Table = DataProvider.GetDataTableForChart("RG_0017_0001_power", DataProvidersFactory.PrimaryMASDataProvider);
			power.SetStyleAndData();
		}

		private void GenerateGovernment()
		{
			Dictionary<string, object[]> data = 
				new Dictionary<string, object[]>
			    {
			      {"Индекс восприятия уровня распространенности коррупции", 
					  new[]{"Индекс восприятия уровня&nbsp;<br />&nbsp;распространенности коррупции", "Не распространена", "Распространена"}},
				  {"Индекс доверия главам", 
					  new[]{"Индекс доверия главам&nbsp;<br />&nbsp;муниципальных образований", "Доверяю", "Не доверяю"}},
				  {"Индекс доверия Думе", 
					  new[]{"Индекс доверия Думе&nbsp;<br />&nbsp;городского округа / муниципального района", "Доверяю", "Не доверяю"}},
				  {"Индекс доверия председателю Думы", 
					  new[]{"Индекс доверия председателю Думы&nbsp;<br />&nbsp;городского округа / муниципального района", "Доверяю", "Не доверяю"}}
			    };

			DataTable table = DataProvider.GetDataTableForChart("RG_0017_0001_government", DataProvidersFactory.PrimaryMASDataProvider);
			for (int i = 0; i < table.Rows.Count; i++)
			{
				DataRow row = table.Rows[i];

				HtmlGenericControl div = new HtmlGenericControl("div");
				div.ID = String.Format("Government_{0}", i);

				div.Style.Add("display", "inline-block");
				div.Style.Add("width", "180px");
				div.Style.Add("margin", "5px");
				div.Style.Add("text-align", "center");
				div.Style.Add("vertical-align", "top");

				string id = row["title"].ToString();
				string positiveStyle = "ShadowGreen";
				string negativeStyle = "ShadowRed";
				if ( i==0 )
				{
					positiveStyle = "ShadowRed";
					negativeStyle = "ShadowGreen";
				}
				StringBuilder html = new StringBuilder();
				html.AppendFormat("<span class=\"TextWhite TextXXLarge {0}\">{1}</span><br />",
					(decimal)row["Индекс"] < 0 ? negativeStyle : (decimal)row["Индекс"] > 0 ? positiveStyle : String.Empty, 
					row["Индекс"]);
				html.AppendFormat("<span class=\"TextWhite TextNormal\">{0}</span><br />", data[id][0].ToString().Replace("&nbsp;<br />&nbsp;", " ")); 
				div.InnerHtml = html.ToString();
				
				StringBuilder tooltip = new StringBuilder();
				tooltip.AppendFormat("&nbsp;<b>{0}</b>&nbsp;<br />", data[id][0]);
				tooltip.AppendFormat("&nbsp;<span style=\"color: green;\">{0}&nbsp;—&nbsp;<b>{1}%</b></span>&nbsp;опрошенных&nbsp;<br />", data[id][1], row[data[id][1].ToString()]);
				tooltip.AppendFormat("&nbsp;<span style=\"color: red;\">{0}&nbsp;—&nbsp;<b>{1}%</b></span>&nbsp;опрошенных&nbsp;<br />", data[id][2], row[data[id][2].ToString()]);
				tooltip.AppendFormat("&nbsp;Затрудняюсь ответить&nbsp;—&nbsp;<b>{0}%</b>&nbsp;опрошенных&nbsp;<br />", row["Затрудняюсь ответить"]);
				tooltip.AppendFormat("&nbsp;Индекс&nbsp;<b>{0}</b>&nbsp;<br />", row["Индекс"]);
				TooltipHelper.AddToolTip(div, String.Format("<span style=\"font-family: Arial; font-size: 14pt;\">{0}</span>", tooltip), Page);
				
				Body_Government.Controls.Add(div);
			}
		}

		private class ChartPower : UltraChartWrapper
		{
			private string ToolTipFormatString { set; get; }
			private int RangeFull { set; get; }
			private Font FontText { set; get; }
			private Color FontColor { set; get; }
			private Color ColorNegative { set; get; }
			private Color ColorPositive { set; get; }

			public ChartPower(UltraChartItem chartItem)
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

				RangeFull = Convert.ToInt32(Table.Rows[0]["Максимум общий"]);
			
				SetStyle();

				ChartControl.LegendVisible = false;
				ChartControl.BrowserSizeAdapting = false;
				ChartControl.ZeroAligned = true;
				ChartControl.Width = 755;
				ChartControl.Height = 180;
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
					int valPositive = Convert.ToInt32(Table.Rows[i]["Доверяю"]);
					int valNegative = Convert.ToInt32(Table.Rows[i]["Не доверяю"]);

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

					box.DataPoint.Label = String.Format(
						"&nbsp;<b>{0}</b>&nbsp;<br />" +
						"&nbsp;<span style='color: green;'>Доверяю&nbsp;—&nbsp;<b>{1}%</b></span>&nbsp;опрошенных&nbsp;<br />" +
						"&nbsp;<span style='color: red;'>Не доверяю&nbsp;—&nbsp;<b>{2}%</b></span>&nbsp;опрошенных&nbsp;<br />" +
						"&nbsp;Индекс доверия&nbsp;<b>{3}</b>&nbsp;",
						box.DataPoint.Label, valPositive, valNegative, valIndex
						);
				}
			}
		}

    }

}