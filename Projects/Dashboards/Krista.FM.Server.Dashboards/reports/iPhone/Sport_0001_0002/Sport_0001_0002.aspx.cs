﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.Sport_0001;
using BoxAnnotation = Infragistics.UltraGauge.Resources.BoxAnnotation;
using OrderedData = Krista.FM.Server.Dashboards.iPadBricks.Sport_0001.OrderedData;
using OrderedValue = Krista.FM.Server.Dashboards.iPadBricks.Sport_0001.OrderedValue;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
	/// <summary>
	/// Отчетность Минспорта (по 6 показателям) #20203
	/// оригинальный субъекта
	/// </summary>
    public partial class Sport_0001_0002 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "Sport_0001_0002";

		public string territoryID;
    	public string territory;
		public string territoryAreaName;
		public string territoryRegionName;
		public string territoryRegionShortName;

		// параметры запросов
		public CustomParam paramYearLast;
		public CustomParam paramYearPrev;
		public CustomParam paramColumn;
		public CustomParam paramRow;
		public CustomParam paramTerritory;

		// кешируемые переменные
		public MSQueryHelper sportData;
		public Dictionary<string, double> statData;
    	public DataTable populationRFTable;
		public DataTable populationFOTable;

		// частоиспользуемые переменные
		public Dictionary<string, double> MainValues { get; set; }


		public Sport_0001_0002()
		{
			MainValues = new Dictionary<string, double>();

		}

		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);

			// Инициализация параметров запроса
			paramYearLast = UserParams.CustomParam("param_year_last");
			paramYearPrev = UserParams.CustomParam("param_year_prev");
			paramColumn = UserParams.CustomParam("param_column");
			paramRow = UserParams.CustomParam("param_row");
			paramTerritory = UserParams.CustomParam("param_territory");
		}

    	protected override void Page_Load(object sender, EventArgs e)
        {
			base.Page_Load(sender, e);

			territory = String.Format("[Российская  Федерация].[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
			territoryAreaName = UserParams.StateArea.Value;
    		territoryRegionName = UserParams.Region.Value;
			territoryRegionShortName = UserParams.ShortRegion.Value;
    		territoryID = CustomParams.GetSubjectIdByName(territoryAreaName);

			paramYearLast.Value = Convert.ToString(Helper.YEAR_LAST);
			paramYearPrev.Value = Convert.ToString(Helper.YEAR_PREV);
			
			// stat data
			paramTerritory.Value = territory;
			DataTable dataTable = DataProvider.GetDataTableForChart("Sport_0001_0002_population", DataProvidersFactory.SecondaryMASDataProvider);
			statData = new Dictionary<string, double>();
			statData.Add("population_last", CRHelper.DBValueConvertToDoubleOrZero(dataTable.Rows[0][0]));
			statData.Add("populationFO_last", CRHelper.DBValueConvertToDoubleOrZero(dataTable.Rows[0][1]));
			statData.Add("populationRF_last", CRHelper.DBValueConvertToDoubleOrZero(dataTable.Rows[0][2]));

			// population data
			paramTerritory.Value = territory + ".parent.parent";
			populationRFTable = DataProvider.GetDataTableForChart("Sport_0001_0002_FM_subjects_population", DataProvidersFactory.SecondaryMASDataProvider);
			paramTerritory.Value = territory + ".parent";
			populationFOTable = DataProvider.GetDataTableForChart("Sport_0001_0002_FM_subjects_population", DataProvidersFactory.SecondaryMASDataProvider);
			
			// minsport data
			sportData = new MSQueryHelper();
			sportData.Territory = territory;
			sportData
				.AddMember("sporter_last", 1, 13, Helper.YEAR_LAST)
				.AddMember("sporter_prev", 1, 13, Helper.YEAR_PREV)
				.AddMember("staff_last", 1, 1, Helper.YEAR_LAST)
				.AddMember("staff_prev", 1, 1, Helper.YEAR_PREV)
				.AddMember("recreation_last", 46, 25, Helper.YEAR_LAST)
				.AddMember("recreation_prev", 46, 25, Helper.YEAR_PREV)
				.AddMember("gymnasium_last", 51, 25, Helper.YEAR_LAST)
				.AddMember("gymnasium_prev", 51, 25, Helper.YEAR_PREV)
				.AddMember("stadium_last", 48, 25, Helper.YEAR_LAST)
				.AddMember("stadium_prev", 48, 25, Helper.YEAR_PREV)
				.AddMember("swimingpool_last", 63, 25, Helper.YEAR_LAST)
				.AddMember("swimingpool_prev", 63, 25, Helper.YEAR_PREV)

				.AddMember("sporterRF_last", 1, 13, Helper.YEAR_LAST, 2)
				.AddMember("sporterFO_last", 1, 13, Helper.YEAR_LAST, 1)
				.AddMember("staffRF_last", 1, 1, Helper.YEAR_LAST, 2)
				.AddMember("staffFO_last", 1, 1, Helper.YEAR_LAST, 1)
				.AddMember("recreationRF_last", 46, 25, Helper.YEAR_LAST, 2)
				.AddMember("recreationFO_last", 46, 25, Helper.YEAR_LAST, 1)
				.AddMember("gymnasiumRF_last", 51, 25, Helper.YEAR_LAST, 2)
				.AddMember("gymnasiumFO_last", 51, 25, Helper.YEAR_LAST, 1)
				.AddMember("stadiumRF_last", 48, 25, Helper.YEAR_LAST, 2)
				.AddMember("stadiumFO_last", 48, 25, Helper.YEAR_LAST, 1)
				.AddMember("swimingpoolRF_last", 63, 25, Helper.YEAR_LAST, 2)
				.AddMember("swimingpoolFO_last", 63, 25, Helper.YEAR_LAST, 1)
				;
			sportData.ExecQuery(DataProvidersFactory.PrimaryMASDataProvider);
			
			BuildSporter();
			BuildStaff();
			BuildRecreation();

			// TouchElementBounds
			string filename = String.Format("{0}TouchElementBounds.xml", HttpContext.Current.Server.MapPath("~/TemporaryImages/"));
			StringBuilder xml = new StringBuilder();
			xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<touchElements>\n");
			xml.AppendFormat("<element id=\"Sport_0001_0006_{0}\" bounds=\"x=0;y=40;width=768;height=150\" openMode=\"\"/>\n", territoryID);
			xml.AppendFormat("<element id=\"Sport_0001_0009_{0}\" bounds=\"x=0;y=230;width=768;height=220\" openMode=\"\"/>\n", territoryID);
			xml.AppendFormat("<element id=\"Sport_0001_0012_{0}\" bounds=\"x=0;y=490;width=768;height=410\" openMode=\"\"/>\n", territoryID);
			xml.Append("</touchElements>");
			File.WriteAllText(filename, xml.ToString());

        }

		#region Численность занимающихся спортом

		/// <summary>
		/// Численность занимающихся спортом
		/// </summary>
		private void BuildSporter()
		{
			SporterHeader.Text = "Численность занимающихся спортом";
			SporterDetailTime.Attributes.Add("href", String.Format("webcommand?showPinchReport=Sport_0001_0006_{0}", territoryID));

			MainValues.Clear();
			MainValues.Add("x1", sportData.GetDouble("sporter_last"));
			MainValues.Add("x2", MainValues["x1"] / statData["population_last"]);
			MainValues.Add("x3", MainValues["x1"] - sportData.GetDouble("sporter_prev"));
			MainValues.Add("x4", Math.Abs(MainValues["x3"]) / sportData.GetDouble("sporter_prev"));

			SporterChartHelper sporterChartHelper = new SporterChartHelper(UltraChartSporter, this, MainValues["x1"]);
			sporterChartHelper.SetStyleAndData();
			
			StringBuilder text = new StringBuilder();
			text.AppendFormat(
				"В__#YEAR_LAST#__г. регулярно занимались спортом__{0}__чел.<br />",
				HtmlDigitsLarge(MainValues["x1"].ToString("N0")));
			if (Math.Abs(MainValues["x3"]) < 1)
				text.AppendFormat("#PARA#не изменилось по сравнению с #YEAR_PREV#__г.<br />"); 
			else
				text.AppendFormat(
					"#PARA#на__{0}__чел. {1}__чем в #YEAR_PREV#__г. ({2})<br />",
					HtmlDigits(Math.Abs(MainValues["x3"]).ToString("N0")),
					MainValues["x3"] > 0 ? "больше__#UP_GREEN#" : "меньше__#DN_RED#",
					HtmlDigits(String.Format("{0}{1}", MainValues["x3"] > 0 ? "+" : "-", MainValues["x4"].ToString("P2")))
					);

			// Доля занимающихся спортом
			text.Append(HtmlMarginTop(String.Format(
				"Доля занимающихся спортом__{0}__населения<br />",
				HtmlDigitsLarge(MainValues["x2"].ToString("P2"))
				)));
			text.Append(ReplaceRank(
				AreaType.RF,
				FillSporterOrderedData1(populationRFTable, AreaType.RF),
				Convert.ToDouble(sportData.GetDouble("sporterRF_last")/statData["populationRF_last"]).ToString("P2")
				));
			text.Append(ReplaceRank(
				AreaType.FO,
				FillSporterOrderedData1(populationFOTable, AreaType.FO),
				Convert.ToDouble(sportData.GetDouble("sporterFO_last") / statData["populationFO_last"]).ToString("P2")
				));
			SporterText.Text = ReplaceGeneral(text.ToString());
		}


		/// <summary>
		/// Доля занимающихся спортом
		/// </summary>
		private OrderedData FillSporterOrderedData1(DataTable fmTable, AreaType areaType)
		{
			paramRow.Value = "1";
			paramColumn.Value = "13";
			paramTerritory.Value = TerritoryByAreaType(areaType);
			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0002_MS_Subjects", DataProvidersFactory.PrimaryMASDataProvider);

			OrderedData orderedData = new OrderedData();
			for (int i = 0; i < fmTable.Rows.Count; i++)
			{
				string id = msTable.Rows[i][0].ToString();
				object msValue = msTable.Rows[i][1];
				object fmValue = fmTable.Rows[i][1];
				if (!CRHelper.DBValueIsEmpty(msValue) && !CRHelper.DBValueIsEmpty(fmValue))
				{
					double value = CRHelper.DBValueConvertToDoubleOrZero(msValue) / CRHelper.DBValueConvertToDoubleOrZero(fmValue);
					orderedData.Add(new OrderedValue(id, value));
				}
				else
				{
					orderedData.Add(new OrderedValue(id));
				}
			}
			orderedData.Sort();

			return orderedData;
		}

		#endregion

		#region Численность кадров

		/// <summary>
		/// Численность кадров
		/// </summary>
		private void BuildStaff()
		{
			StaffHeader.Text = "Численность кадров";
			StaffDetailTime.Attributes.Add("href", String.Format("webcommand?showPinchReport=Sport_0001_0009_{0}", territoryID));

			MainValues.Clear();
			MainValues.Add("x1", sportData.GetDouble("staff_last"));
			MainValues.Add("x2", MainValues["x1"] - sportData.GetDouble("staff_prev"));
			MainValues.Add("x3", Math.Abs(MainValues["x2"]) / sportData.GetDouble("staff_prev"));
			MainValues.Add("x4", sportData.GetDouble("sporter_last") / MainValues["x1"]);
			MainValues.Add("x5", 10000 * MainValues["x1"] / statData["population_last"]);

			StaffChartHelper staffChartHelper = new StaffChartHelper(this, UltraChartStaff);
			staffChartHelper.SetStyleAndData();
			
			StringBuilder text = new StringBuilder();
			text.Append(String.Format(
				"В__#YEAR_LAST#__г. численность работников спорта__{0}__чел.<br />#PARA#{1}<br />",
				HtmlDigitsLarge(MainValues["x1"].ToString("N0")),
				Math.Abs(MainValues["x2"]) < 1
					? "не изменилось по сравнению с #YEAR_PREV#__г."
					: String.Format("на__{0}__чел. {1}__чем в #YEAR_PREV#__г. ({2})",
						HtmlDigits(Math.Abs(MainValues["x2"]).ToString("N0")),	
						MainValues["x2"] > 0 ? "больше__#UP_GREEN#" : "меньше__#DN_RED#",
						HtmlDigits(String.Format("{0}{1}", MainValues["x2"] > 0 ? "+" : "-", MainValues["x3"].ToString("P2")))
				)));

			// Занимающихся спортом на одного работника
			text.Append(HtmlMarginTop(String.Format(
				"Занимающихся спортом на одного работника__{0}__чел.<br />",
				HtmlDigits(MainValues["x4"].ToString("N2"))
				)));
			text.Append(ReplaceRank(
				AreaType.RF,
				FillStaffOrderedData1(AreaType.RF),
				Convert.ToDouble(sportData.GetDouble("sporterRF_last") / sportData.GetDouble("staffRF_last")).ToString("N2"),
				"чел."
				));
			text.Append(ReplaceRank(
				AreaType.FO,
				FillStaffOrderedData1(AreaType.FO),
				Convert.ToDouble(sportData.GetDouble("sporterFO_last") / sportData.GetDouble("staffFO_last")).ToString("N2"),
				"чел."
				));
			
			// Число спортивных работников на 10__000 населения
			text.Append(HtmlMarginTop(String.Format(
				"Число спортивных работников на 10__тыс. населения__{0}__чел.<br />",
				HtmlDigits(MainValues["x5"].ToString("N2"))
				)));
			text.Append(ReplaceRank(
				AreaType.RF,
				FillStaffOrderedData2(populationRFTable, AreaType.RF),
				Convert.ToDouble(10000 * sportData.GetDouble("staffRF_last") / statData["populationRF_last"]).ToString("N2"),
				"чел."
				));
			text.Append(ReplaceRank(
				AreaType.FO,
				FillStaffOrderedData2(populationFOTable, AreaType.FO),
				Convert.ToDouble(10000 * sportData.GetDouble("staffFO_last") / statData["populationFO_last"]).ToString("N2"),
				"чел."
				));
			
			StaffText.Text = ReplaceGeneral(text.ToString());
		}

		/// <summary>
		/// Занимающихся спортом на одного работника
		/// </summary>
		private OrderedData FillStaffOrderedData1(AreaType areaType)
		{
			paramTerritory.Value = TerritoryByAreaType(areaType);
			
			paramRow.Value = "1";
			paramColumn.Value = "13";
			DataTable sporterTable = DataProvider.GetDataTableForChart("Sport_0001_0002_MS_Subjects", DataProvidersFactory.PrimaryMASDataProvider);

			paramRow.Value = "1";
			paramColumn.Value = "1";
			DataTable staffTable = DataProvider.GetDataTableForChart("Sport_0001_0002_MS_Subjects", DataProvidersFactory.PrimaryMASDataProvider);

			OrderedData orderedData = new OrderedData();

			for (int i = 0; i < sporterTable.Rows.Count; i++)
			{
				string id = sporterTable.Rows[i][0].ToString();
				object sporterValue = sporterTable.Rows[i][1];
				object staffValue = staffTable.Rows[i][1];
				if (!CRHelper.DBValueIsEmpty(sporterValue) && !CRHelper.DBValueIsEmpty(staffValue))
				{
					double value = CRHelper.DBValueConvertToDoubleOrZero(sporterValue) / CRHelper.DBValueConvertToDoubleOrZero(staffValue);
					orderedData.Add(new OrderedValue(id, value));
				}
				else
				{
					orderedData.Add(new OrderedValue(id));
				}
			}
			orderedData.Sort();
			
			return orderedData;
		}

		/// <summary>
		/// Число спортивных работников на 10__тыс. населения
		/// </summary>
		private OrderedData FillStaffOrderedData2(DataTable fmTable, AreaType areaType)
		{
			paramRow.Value = "1";
			paramColumn.Value = "1";
			paramTerritory.Value = TerritoryByAreaType(areaType);
			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0002_MS_Subjects", DataProvidersFactory.PrimaryMASDataProvider);

			OrderedData orderedData = new OrderedData();
			for (int i = 0; i < fmTable.Rows.Count; i++)
			{
				string id = msTable.Rows[i][0].ToString();
				object msValue = msTable.Rows[i][1];
				object fmValue = fmTable.Rows[i][1];
				if (!CRHelper.DBValueIsEmpty(msValue) && !CRHelper.DBValueIsEmpty(fmValue))
				{
					double value = 10000 * CRHelper.DBValueConvertToDoubleOrZero(msValue) / CRHelper.DBValueConvertToDoubleOrZero(fmValue);
					orderedData.Add(new OrderedValue(id, value));
				}
				else
				{
					orderedData.Add(new OrderedValue(id));
				}
			}
			orderedData.Sort();

			return orderedData;
		}

		#endregion

		#region Спортивные сооружения

		/// <summary>
		/// Спортивные сооружения
		/// </summary>
		private void BuildRecreation()
		{
			RecreationHeader.Text = "Спортивные сооружения";
			RecreationDetailTime.Attributes.Add("href", String.Format("webcommand?showPinchReport=Sport_0001_0012_{0}", territoryID));

			MainValues.Clear();
			MainValues.Add("x1", sportData.GetDouble("recreation_last"));
			MainValues.Add("x2", MainValues["x1"] - sportData.GetDouble("recreation_prev"));
			MainValues.Add("x3", Math.Abs(MainValues["x2"]) / sportData.GetDouble("recreation_prev"));
			
			StringBuilder text = new StringBuilder();
			text.AppendFormat(
				"В__#YEAR_LAST#__г. общее число спортивных сооружений__{0}<br />#PARA#{1}<br />",
				HtmlDigitsLarge(MainValues["x1"].ToString("N0")),
				Math.Abs(MainValues["x2"]) < 1
					? "не изменилось по сравнению с #YEAR_PREV#__г."
					: String.Format("на__{0}__{1}__чем в #YEAR_PREV#__г. ({2})",
						HtmlDigits(Math.Abs(MainValues["x2"]).ToString("N0")), 
						MainValues["x2"] > 0 ? "больше__#UP_GREEN#" : "меньше__#DN_RED#",
						HtmlDigits(String.Format("{0}{1}", MainValues["x2"] > 0 ? "+" : "-", MainValues["x3"].ToString("P2")))
						));
			
			text.Append(BuildRecreationType(RecreationType.Gymnasium));
			text.Append(BuildRecreationType(RecreationType.Stadium));
			text.Append(BuildRecreationType(RecreationType.Swimmingpool));
			
			RecreationText.Text = ReplaceGeneral(text.ToString());
		}

		private string BuildRecreationType(RecreationType recreationType)
		{
			string _id;
			string _name;
			double _avg;
			double _norma;
			string _measure;
			string _format;
			string _gaugeFill;

			switch (recreationType)
			{
				case RecreationType.Gymnasium:
					{
						paramRow.Value = "51";
						paramColumn.Value = "25";
						_id = "gymnasium";
						_name = "Спортзалов";
						_avg = Convert.ToDouble(Helper.PROVIDE_AVG_GYMN);
						_norma = Convert.ToDouble(Helper.PROVIDE_NRM_GYMN);
						_measure = "тыс.__м²";
						_format = "N2";
						_gaugeFill = "gauge_fill1.png";
						break;
					}
				case RecreationType.Stadium:
					{
						paramRow.Value = "48";
						paramColumn.Value = "25";
						_id = "stadium";
						_name = "Плоскостных сооружений";
						_avg = Convert.ToDouble(Helper.PROVIDE_AVG_STAD);
						_norma = Convert.ToDouble(Helper.PROVIDE_NRM_STAD);
						_measure = "тыс.__м²";
						_format = "N2";
						_gaugeFill = "gauge_fill3.png";
						break;
					}
				case RecreationType.Swimmingpool:
					{
						paramRow.Value = "63";
						paramColumn.Value = "25";
						_id = "swimingpool";
						_name = "Бассейнов";
						_avg = Convert.ToDouble(Helper.PROVIDE_AVG_SWIM);
						_norma = Convert.ToDouble(Helper.PROVIDE_NRM_SWIM);
						_measure = "м²";
						_format = "N0";
						_gaugeFill = "gauge_fill2.png";
						break;
					}
				default:
					return String.Empty;
			}
			
			MainValues.Clear();
			MainValues.Add("x1", sportData.GetDouble(String.Format("{0}_last", _id)));
			MainValues.Add("x2", MainValues["x1"] - sportData.GetDouble(String.Format("{0}_prev", _id)));
			MainValues.Add("x3", Math.Abs(MainValues["x2"]) / sportData.GetDouble(String.Format("{0}_prev", _id)));
			MainValues.Add("x4", 10 * _avg * MainValues["x1"] / statData["population_last"]);
			MainValues.Add("x5", MainValues["x4"] / _norma);

			GaugeWrapper gauge = new GaugeWrapper(this);
			gauge.NewGauge(MainValues["x4"], _norma, _format, String.Format("{0} на 10 тыс. чел.", _measure.Replace("__", " ")), _gaugeFill);

			StringBuilder text = new StringBuilder();
			
			text.Append(HtmlMarginTop(String.Format(
				"{0}__{1}<br />", HtmlTextBold(_name), HtmlDigits(MainValues["x1"].ToString("N0")))));
			
			text.AppendFormat("<div style=\"float: right;\">{0}</div>",gauge.GetGaugeImageTag());
			if (Math.Abs(MainValues["x2"]) < 1)
				text.AppendFormat("#PARA#не изменилось по сравнению с #YEAR_PREV#__г.<br />");
			else
				text.AppendFormat(
					"#PARA#на__{0}__{1}__чем в #YEAR_PREV#__г. ({2})<br />",
					HtmlDigits(Math.Abs(MainValues["x2"]).ToString("N0")),
					MainValues["x2"] > 0 ? "больше__#UP_GREEN#" : "меньше__#DN_RED#",
					HtmlDigits(String.Format("{0}{1}", MainValues["x2"] > 0 ? "+" : "-", MainValues["x3"].ToString("P2")))
					);

			text.AppendFormat(
				"Обеспеченность__{0}__{1} на 10__тыс.__чел. ({2}__от норматива)<br />",
				HtmlDigits(MainValues["x4"].ToString("N2")), _measure,
				HtmlDigits(MainValues["x5"].ToString("P2")));
			
			text.Append(HtmlMarginSmallTop(String.Empty));

			text.Append(ReplaceRank(
				AreaType.RF,
				FillRecreationOrderedData(populationRFTable, AreaType.RF, _avg, _norma),
				Convert.ToDouble(10 * _avg * sportData.GetDouble(_id+"RF_last") / statData["populationRF_last"]).ToString("N2"),
				String.Format("{0} на 10__тыс.__чел.", _measure)
				));

			text.Append(ReplaceRank(
				AreaType.FO,
				FillRecreationOrderedData(populationFOTable, AreaType.FO, _avg, _norma),
				Convert.ToDouble(10 * _avg * sportData.GetDouble(_id+"FO_last") / statData["populationFO_last"]).ToString("N2"),
				String.Format("{0} на 10__тыс.__чел.", _measure)
				));
			
			return text.ToString();
		}

    	private OrderedData FillRecreationOrderedData(DataTable fmTable, AreaType areaType, double avg, double norma)
		{
			paramTerritory.Value = TerritoryByAreaType(areaType);
			DataTable msTable = DataProvider.GetDataTableForChart("Sport_0001_0002_MS_Subjects", DataProvidersFactory.PrimaryMASDataProvider);

			OrderedData orderedData = new OrderedData();
			for (int i = 0; i < fmTable.Rows.Count; i++)
			{
				string id = msTable.Rows[i][0].ToString();
				object msValue = msTable.Rows[i][1];
				object fmValue = fmTable.Rows[i][1];
				if (!CRHelper.DBValueIsEmpty(msValue) && !CRHelper.DBValueIsEmpty(fmValue))
				{
					double value = 10 * avg * CRHelper.DBValueConvertToDoubleOrZero(msValue) / CRHelper.DBValueConvertToDoubleOrZero(fmValue) / norma;
					orderedData.Add(new OrderedValue(id, value));
				}
				else
				{
					orderedData.Add(new OrderedValue(id));
				}
			}
			orderedData.Sort();

			return orderedData;
		}

		#endregion
		
		#region Обработка данных
		
		/// <summary>
		/// Добавляет верхний отступ блоку
		/// </summary>
		private static string HtmlMarginTop(string text)
		{
			return String.Format("<div style=\"margin-top: 13px;\">{0}</div>", text);
		}

		/// <summary>
		/// Добавляет большой верхний отступ блоку
		/// </summary>
		private static string HtmlMarginSmallTop(string text)
		{
			return String.Format("<div style=\"margin-top: 5px;\">{0}</div>", text);
		}

		/// <summary>
		/// Оформляет вывод цифорвых значений
		/// </summary>
		private static string HtmlDigits(string text)
		{
			return String.Format("<span class=\"DigitsValue\">{0}</span>", text);
		}

		/// <summary>
		/// Оформляет вывод важных цифорвых значений
		/// </summary>
		private static string HtmlDigitsLarge(string text)
		{
			return String.Format("<span class=\"DigitsValueXLarge\">{0}</span>", text);
		}

		/// <summary>
		/// Оформляет вывод белого текста
		/// </summary>
		private static string HtmlTextBold(string text)
		{
			return String.Format("<span style=\"font-weight: bold;\">{0}</span>", text);
		}

		/// <summary>
		/// Заменяет основны переменные
		/// </summary>
		private string ReplaceGeneral(string text)
		{
			text = text 
				.Replace("__", "&nbsp;")
				.Replace("#PARA#", String.Format("<img src=\"../../../images/empty.gif\" width=\"{0}\" height=\"1\" />", Helper.PARAGRAPH_LENGTH))

				.Replace("#YEAR_LAST#", HtmlDigits(Convert.ToString(Helper.YEAR_LAST)))
				.Replace("#YEAR_PREV#", Convert.ToString(Helper.YEAR_PREV))

				.Replace("#GOOD#", "<img src=\"../../../images/starYellowBB.png\" height=\"16\" style=\"margin-bottom:-1px;\"/>")
				.Replace("#BAD#", "<img src=\"../../../images/starGrayBB.png\" height=\"18\" style=\"margin-bottom:-2px;\"/>")

				.Replace("#MAX#", "<img src=\"../../../images/max.png\" height=\"18\" style=\"margin-bottom:-3px;\"/>")
				.Replace("#MIN#", "<img src=\"../../../images/min.png\" height=\"18\" style=\"margin-bottom:-3px;\"/>")

				.Replace("#UP_GREEN#", "<img src=\"../../../images/arrowGreenUpBB.png\" style=\"margin-bottom:-2px;\"/>")
				.Replace("#DN_RED#", "<img src=\"../../../images/arrowRedDownBB.png\" style=\"margin-bottom:-2px;\"/>")
				;

			return text;
		}

		/// <summary>
		/// Возвращает форматированную строку с рангом
		/// </summary>
		private string ReplaceRank(AreaType area, OrderedData data, string value, string measure = "")
		{
			int rank = data.GetRank(territoryAreaName);
			return
				String.Format(
					"#PARA#ранг в {0}__{1}{2}__({3}{4}__по {0})<br />",
					area == AreaType.RF ? "РФ" : territoryRegionShortName,
					HtmlDigits(rank.ToString()),
					rank == 1 ? "#GOOD#" : rank == data.MaxRank ? "#BAD#" : String.Empty,
					HtmlDigits(value),
					measure == "" ? String.Empty : String.Format("__{0}", measure)
				);
		}

		private string TerritoryByAreaType(AreaType areaType)
		{
			if (areaType == AreaType.RF)
			{
				return territory + ".parent.parent";
			}
			if (areaType == AreaType.FO)
			{
				return territory + ".parent";
			}
			return territory;
		}

		#endregion
		
		private enum AreaType
		{
			RF, FO
		}
		
		private enum RecreationType
		{
			Gymnasium,
			Stadium,
			Swimmingpool
		}
    }

	public class SporterChartHelper : UltraChartWrapper
	{
		private Sport_0001_0002 page;
		private double mainValue;

		public SporterChartHelper(UltraChartItem chartItem, Sport_0001_0002 page, double value)
			: base(chartItem)
		{
			this.page = page;
			mainValue = value;
		}

		protected override void SetStyle()
		{
			ChartControl.TemporaryUrlPrefix = page.TEMPORARY_URL_PREFIX;

			ChartControl.BrowserSizeAdapting = false;
			ChartControl.Width = 260;
			ChartControl.Height = 130;
			ChartControl.Chart.ChartType = ChartType.PieChart;

			ChartControl.LegendVisible = false;
			ChartControl.TooltipFormatString = "&nbsp;<span style='font-family: Arial; font-size: 14pt;'><b><DATA_VALUE:N0></b>&nbsp;чел.&nbsp;<br />&nbsp;<b><PERCENT_VALUE:N2>%</b></span>&nbsp;";

			ChartControl.Chart.PieChart.RadiusFactor = 100;
			ChartControl.Chart.PieChart.StartAngle = 190;
			ChartControl.Chart.PieChart.Labels.Font = new Font("Verdana", 8, FontStyle.Bold);
			ChartControl.Chart.PieChart.Labels.FormatString = "<DATA_VALUE:N0> чел.\n<PERCENT_VALUE:N2>%";
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
			row[1] = page.statData["population_last"] - mainValue;
			table.Rows.Add(row);

			ChartControl.Chart.DataSource = table;
			ChartControl.Chart.DataBind();
		}

		protected override void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
		{
			Infragistics.UltraChart.Core.Primitives.Box box = new Infragistics.UltraChart.Core.Primitives.Box(new Rectangle(28, 30, 54, 52));
			box.PE.StrokeWidth = 0;
			box.PE.ElementType = PaintElementType.Image;
			box.PE.FillImage = new Bitmap(page.Server.MapPath("~/images/Minsport/sporter.png"));
			e.SceneGraph.Add(box);

			box = new Infragistics.UltraChart.Core.Primitives.Box(new Rectangle(180, 52, 50, 46));
			box.PE.StrokeWidth = 0;
			box.PE.ElementType = PaintElementType.Image;
			box.PE.FillImage = new Bitmap(page.Server.MapPath("~/images/Minsport/lazier.png"));
			e.SceneGraph.Add(box);
			
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];

				if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
				{
					Infragistics.UltraChart.Core.Primitives.Text text = primitive as Infragistics.UltraChart.Core.Primitives.Text;

					if (text.GetTextString().Contains(mainValue.ToString("N0")))
					{
						text.bounds = new Rectangle(0, 0, 120, text.bounds.Height);

					}
					else 
					{
						int left = Convert.ToInt32(ChartControl.Chart.Width.Value) - 120;
						int top = Convert.ToInt32(ChartControl.Height.Value) - text.bounds.Height;
						text.bounds = new Rectangle(left, top, 120, text.bounds.Height);
					}
				}

			}
		}
	}

	public class StaffChartHelper : UltraChartWrapper
	{
		private Sport_0001_0002 page;
		
		public StaffChartHelper(Sport_0001_0002 page, UltraChartItem chartItem) 
			: base(chartItem)
		{
			this.page = page;
		}

		protected override void SetStyle()
		{
			ChartControl.TemporaryUrlPrefix = page.TEMPORARY_URL_PREFIX;

			ChartControl.BrowserSizeAdapting = false;
			ChartControl.Width = 260;
			ChartControl.Height = 170;
			
			ChartControl.Chart.ChartType = ChartType.SplineChart;
			ChartControl.LegendVisible = false;
			ChartControl.SwapRowAndColumns = true;

			ChartControl.TooltipFormatString = "&nbsp;<span style='font-family: Arial; font-size: 14pt;'><b><DATA_VALUE:N0></b>&nbsp;чел.</span>&nbsp;";

			ChartControl.Chart.Axis.Y.Visible = false;
			ChartControl.Chart.Axis.Y.Margin.Near.Value = 10;
			ChartControl.Chart.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
			ChartControl.Chart.Axis.Y.Margin.Far.Value = 25;
			ChartControl.Chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;

			ChartControl.Chart.Axis.X.Visible = true;
			ChartControl.Chart.Axis.X.Extent = 25;
			ChartControl.Chart.Axis.X.MajorGridLines.Visible = false;
			ChartControl.Chart.Axis.X.Labels.Visible = true;
			ChartControl.Chart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
			ChartControl.Chart.Axis.X.Margin.Near.Value = 35;
			ChartControl.Chart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
			ChartControl.Chart.Axis.X.Margin.Far.Value = 25;
			ChartControl.Chart.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
			
			ChartControl.Chart.SplineChart.ChartText.Clear();
			ChartControl.Chart.SplineChart.ChartText.Add(
				new ChartTextAppearance
				{
					Visible = true,
					Column = -2,
					Row = -2,
					VerticalAlign = StringAlignment.Far,
					ItemFormatString = "<DATA_VALUE:N0> чел.\n\n",
					ChartTextFont = new Font("Verdana", 8, FontStyle.Bold),
					FontColor = Color.White
				});

			ChartControl.Chart.SplineChart.LineAppearances.Clear();
			LineAppearance lineAppearance = new LineAppearance();
			lineAppearance.LineStyle.StartStyle = LineCapStyle.RoundAnchor;
			lineAppearance.LineStyle.EndStyle = LineCapStyle.RoundAnchor;
			lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Large;
			lineAppearance.IconAppearance.Icon = SymbolIcon.Character;
			lineAppearance.IconAppearance.Character = '◉';
			lineAppearance.IconAppearance.CharacterFont = new Font("Comic Sans MS", 24, FontStyle.Bold);
			lineAppearance.IconAppearance.PE.ElementType = PaintElementType.CustomBrush;
			lineAppearance.IconAppearance.PE.CustomBrush = new SolidBrush(Color.FromArgb(unchecked((int)0xfffebe2e)));//f9dd1c
			lineAppearance.SplineTension = 0.3f;
			lineAppearance.Thickness = 5;
			ChartControl.Chart.SplineChart.LineAppearances.Add(lineAppearance);

			// цвета
			ChartControl.Chart.ColorModel.ModelStyle = ColorModels.CustomLinear;
			ChartControl.Chart.ColorModel.CustomPalette = new[] { Color.FromArgb(unchecked((int)0xfffebe2e)) };
			ChartControl.Chart.ColorModel.AlphaLevel = 165;
			
		}

		protected override void SetData()
		{
			page.paramRow.Value = "1";
			page.paramColumn.Value = "1";
			page.paramTerritory.Value = page.territory;
			DataTable table = DataProvider.GetDataTableForChart("Sport_0001_0002_staff_chart", DataProvidersFactory.PrimaryMASDataProvider);

			ChartControl.Chart.DataSource = table;
			ChartControl.Chart.DataBind();

		}

	}

	public class GaugeWrapper
	{
		private Sport_0001_0002 page;
		private double _value;
		private double _valueMax;
		private string _format;
		private string _measure;
		private string fileFill;
		private string fileName;
		private string filePath;
		private UltraGauge gauge;
		private LinearGauge linearGauge;

		private const int ScaleStartExtent = 3;
		private const int ScaleEndExtent = 77;
		private const int ScaleInnerExtent = 35;
		private const int ScaleOuterExtent = 85;

		public GaugeWrapper(Sport_0001_0002 page)
		{
			this.page = page;
		}

		public void NewGauge(double value, double valueMax, string format, string measure, string fileNameFill)
		{
			_value = value;
			_valueMax = valueMax;
			_format = format;
			_measure = measure;

			fileFill = fileNameFill;
			fileName = String.Format("{0}_gauge_{1}.png", page.REPORT_ID, Guid.NewGuid());
			filePath = page.Server.MapPath("~/TemporaryImages/" + fileName);

			BuildGauge();
		}

		/// <summary>
		/// Формирует картинку из гейджа
		/// </summary>
		public string GetGaugeImageTag()
		{
			gauge.SaveTo(filePath, GaugeImageType.Png, new Size((int)(gauge.Width.Value), (int)(gauge.Height.Value)));

			return String.Format("<img style=\"\" src=\"{0}/TemporaryImages/{1}\"/>", page.TEMPORARY_URL_PREFIX, fileName);
		}

		private void BuildGauge()
		{
			gauge = new UltraGauge();
			gauge.Width = 230;
			gauge.Height = 60;

			linearGauge = new LinearGauge();

			AddBackground();
			AddBackgroundScale();
			AddMainScale();
			AddValue();
			AddValueMax();
			AddTitleBottom(_measure);

			gauge.Gauges.Add(linearGauge);
		}

		private void AddMainScale()
		{
			double value = (_value > _valueMax) ? _valueMax : _value;

			LinearGaugeScale scale = new LinearGaugeScale();
			scale.EndExtent = ScaleEndExtent;
			scale.StartExtent = ScaleStartExtent;
			scale.Axes.Add(new NumericAxis(0, _valueMax, _valueMax / 5));

			LinearGaugeRange range = new LinearGaugeRange();
			range.StartValue = 0;
			range.EndValue = value;
			range.InnerExtent = ScaleInnerExtent;
			range.OuterExtent = ScaleOuterExtent;

			ImageBrushElement brush = new ImageBrushElement();
			brush.Image = new Bitmap(page.Server.MapPath(String.Format("~/images/Minsport/{0}", fileFill)));
			brush.ImageFit = ImageFit.Tile;
			range.BrushElements.Add(brush);
			scale.Ranges.Add(range);


			linearGauge.Scales.Add(scale);
		}

		private void AddBackground()
		{
			ImageBrushElement brush = new ImageBrushElement();
			brush.Image = new Bitmap(page.Server.MapPath("~/images/Minsport/gauge_bg.png"));
			brush.RelativeBounds = new Rectangle(0, 0, 180, 45);
			brush.RelativeBoundsMeasure = Measure.Pixels;
			linearGauge.BrushElements.Add(brush);
		}

		private void AddBackgroundScale()
		{
			LinearGaugeScale scale = new LinearGaugeScale();
			scale.StartExtent = ScaleStartExtent;
			scale.EndExtent = ScaleEndExtent;
			scale.InnerExtent = ScaleInnerExtent;
			scale.OuterExtent = ScaleOuterExtent;
			scale.BrushElements.Add(new SolidFillBrushElement(Color.FromArgb(unchecked((int)0xff232221))));
			linearGauge.Scales.Add(scale);
		}

		private void AddValue()
		{
			double value = _value > _valueMax ? _valueMax : _value;
			Color colorText = Color.White;

			BoxAnnotation annotation = new BoxAnnotation();
			int start = 1 + Convert.ToInt32((ScaleEndExtent - 2 * ScaleStartExtent) * value / _valueMax);
			if (value / _valueMax > 0.70)
			{
				start -= 16;
				colorText = Color.FromArgb(unchecked((int)0xff000000));

				if (_value.ToString("N2").Length > 4)
				{
					start -= 4 * (_value.ToString("N2").Length - 4);
				}
			}

			annotation.Bounds = new Rectangle(ScaleStartExtent + start, 25, 0, 0);
			annotation.BoundsMeasure = Measure.Percent;
			annotation.Label.Font = new Font("Arial", 12, FontStyle.Bold);
			annotation.Label.BrushElement = new SolidFillBrushElement(colorText);
			annotation.Label.FormatString = _value.ToString("N2");
			gauge.Annotations.Add(annotation);
		}

		private void AddValueMax()
		{
			BoxAnnotation annotation = new BoxAnnotation();
			annotation.Bounds = new Rectangle(ScaleEndExtent + 3, 25, 20, 0);
			annotation.BoundsMeasure = Measure.Percent;
			annotation.Label.Font = new Font("Arial", 12, FontStyle.Bold);
			annotation.Label.BrushElement = new SolidFillBrushElement(Color.White);
			annotation.Label.FormatString = _valueMax.ToString(_format);
			gauge.Annotations.Add(annotation);
		}

		private void AddTitleBottom(string text)
		{
			BoxAnnotation annotation = new BoxAnnotation();
			annotation.Bounds = new Rectangle(0, 77, 80, 0);
			annotation.BoundsMeasure = Measure.Percent;
			annotation.Label.Font = new Font("Arial", 10);
			annotation.Label.BrushElement = new SolidFillBrushElement(Color.FromArgb(unchecked((int)0xffD1D1D1)));
			annotation.Label.FormatString = text;
			gauge.Annotations.Add(annotation);
		}
	}

	public class MSQueryHelper
	{
		private const string cube = "[1-ФК]";
		private List<string> members;
		private List<string> select;
		private DataTable data;

		public string Territory { get; set; }


		public MSQueryHelper()
		{
			members = new List<string>();
			select = new List<string>();
		}

		public MSQueryHelper AddMember(string id, int row, int column, int year, int parentCount = 0)
		{
			string terra = Territory;
			for (int i = 0; i < parentCount; i++)
			{
				terra += ".parent";
			}

			select.Add(template_select.Replace("#ID#", id));
			members.Add(
				template_member
					.Replace("#ID#", id)
					.Replace("#ROW#", String.Format("{0}", row))
					.Replace("#COLUMN#", String.Format("{0}", column))
					.Replace("#YEAR#", String.Format("{0}", year))
					.Replace("#TERRITORY#", terra)
				);
			return this;
		}

		public void ExecQuery(DataProvider dataProvider)
		{
			string query = template_query
				.Replace("#MEMBERS#", String.Join("\n", members.ToArray()))
				.Replace("#SELECT#", String.Join(",\n\t\t", select.ToArray()))
				.Replace("#CUBE#", cube);

			data = new DataTable();
			dataProvider.GetDataTableForChart(query, "dummy", data);
		}

		public object GetValue(string id)
		{
			return data.Rows[0][id];
		}

		public double GetDouble(string id)
		{
			return CRHelper.DBValueConvertToDoubleOrZero(data.Rows[0][id]);
		}

		#region шаблоны для запроса

		private const string template_query = @"
-- Sport_0001_0002_base
with
    #MEMBERS#
select
    { 
        #SELECT# 
    } on columns
from #CUBE#";
		private const string template_select = @"[Measures].[#ID#]";
		private const string template_member = @"
    member [Measures].[#ID#] 
    as ' 
        (            
            filter 
            (
                [1ФК_Строки_сопоставимые].members,
                [1ФК_Строки_сопоставимые].[Строки_сопоставимые].currentMember.properties(""Код"") = ""#ROW#""
            ).item(0),
            filter 
            (
                [1ФК_Столбцы_сопоставимые].members,
                [1ФК_Столбцы_сопоставимые].[Столбцы_сопоставимые].currentMember.properties(""Код"") = ""#COLUMN#""
            ).item(0),
            [Год].[Год].[Все].[#YEAR#],
            [Территории_сопоставимый].[Территории_сопоставимый].[All].#TERRITORY#,
            [Measures].[Значение] 
        )'";

		#endregion

	}
	
}