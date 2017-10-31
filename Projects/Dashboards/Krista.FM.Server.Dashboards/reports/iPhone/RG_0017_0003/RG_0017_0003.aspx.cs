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
	/// Название: Социологические опросы по ЯНАО, детализация по электоральным предпочтениям
	/// Описание: Анализ данных социологических опросов населения Ямало-Ненецкого автономного округа
	/// Кубы: РЕГИОН_Данные опросов
	/// </summary>
    public partial class RG_0017_0003 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "RG_0017_0003";
		
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
			DataTable table = DataProvider.GetDataTableForChart("RG_0017_0003_last_period", DataProvidersFactory.PrimaryMASDataProvider);
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
			Title.InnerHtml = "Электоральные предпочтения";
			TitleSub.InnerHtml = String.Format(
				"по данным социологических опросов за {0} {1} года по {2}",
				paramMonth.ToLower(), paramYear, RegionsNamingHelper.ShortName(paramTerritory));

			DataTable table = DataProvider.GetDataTableForChart("RG_0017_0003_electorate", DataProvidersFactory.PrimaryMASDataProvider);
			ElectorateGrid.Controls.Add(GenerateHtmlTable(table));
		}

		private HtmlTable GenerateHtmlTable(DataTable table)
		{
			HtmlTable htmlTable = new HtmlTable();
			htmlTable.CellSpacing = 0;
			htmlTable.Attributes.Add("class", "htmlGrid");

			// заголовок
			HtmlTableRow htmlRowHeader = new HtmlTableRow();
			for (int i = 0; i < table.Columns.Count - 1; i++)
			{
				HtmlTableCell cell = new HtmlTableCell();
				cell.Attributes.Add("class", "header");
				cell.InnerHtml = String.Format("<div class=\"wrapper\"><div class=\"rotate\">{0}</div></div>",
					table.Columns[i].Caption
						.Replace("title", "&nbsp;")
						.Replace("Значение; ", String.Empty)
						.Replace("(Ямало-Ненецкий автономный округ ДАННЫЕ)", "Все")
						.Replace("муниципальный район", "МР")
						.Replace("Город", "г."));
				htmlRowHeader.Cells.Add(cell);
			}
			htmlTable.Rows.Add(htmlRowHeader);

			// тело
			for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
			{
				HtmlTableRow htmlRow = new HtmlTableRow();

				int maxValue = Int32.MinValue;
				int minValue = Int32.MaxValue;
				for (int columnIndex = 0; columnIndex < table.Columns.Count - 1; columnIndex++)
				{
					HtmlTableCell cell = new HtmlTableCell();

					if (columnIndex == 0)
					{
						cell.Attributes.Add("class", "photo");
						string title = table.Rows[rowIndex][columnIndex].ToString();
						string pkid = table.Rows[rowIndex][table.Columns.Count - 1].ToString();
						StringBuilder innerHtml = new StringBuilder();
						if (!pkid.IsEmpty() && pkid != "8" && pkid != "9")
						{
							innerHtml.AppendFormat("<img src=\"images\\party_{0}_{1}.png\" style=\"margin-top: 5px;\" /><br />", paramTerritoryId, pkid);
						}
						innerHtml.AppendFormat("{0}", title);
						cell.InnerHtml = innerHtml.ToString();
					}
					else
					{
						if (!CRHelper.DBValueIsEmpty(table.Rows[rowIndex][columnIndex]))
						{
							int value = CRHelper.DBValueConvertToInt32OrZero(table.Rows[rowIndex][columnIndex]);
							maxValue = Math.Max(maxValue, value);
							minValue = Math.Min(minValue, value);
							if (value != 0)
								cell.InnerHtml = value.ToString();
						}
					}

					htmlRow.Cells.Add(cell);
				}

				// звезды
				for (int columnIndex = 2; columnIndex < table.Columns.Count - 1; columnIndex++)
				{
					if (!CRHelper.DBValueIsEmpty(table.Rows[rowIndex][columnIndex]))
					{
						HtmlTableCell cell = htmlRow.Cells[columnIndex];
						int value = CRHelper.DBValueConvertToInt32OrZero(table.Rows[rowIndex][columnIndex]);
						if (value != 0)
						{
							if (value == maxValue && minValue + 1 != maxValue)
								cell.Style.Add("background", "url(../../../images/starYellowBB.png) no-repeat center 80%");
							else if (value == minValue)
								cell.Style.Add("background", "url(../../../images/starGrayBB.png) no-repeat center 80%");
						}
					}
				}

				htmlTable.Rows.Add(htmlRow);
			}
			
			return htmlTable;
		}

    }
	
}