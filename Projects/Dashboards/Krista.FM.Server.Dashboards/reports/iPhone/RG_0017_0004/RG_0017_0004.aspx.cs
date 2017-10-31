using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
	/// <summary>
	/// Название: Социологические опросы по ЯНАО, детализация по доверию институтам власти
	/// Описание: Анализ данных социологических опросов населения Ямало-Ненецкого автономного округа
	/// Кубы: РЕГИОН_Данные опросов
	/// </summary>
    public partial class RG_0017_0004 : CustomReportPage
    {
    	public string TEMPORARY_URL_PREFIX = "../../..";
		public string REPORT_ID = "RG_0017_0004";
		
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
			DataTable table = DataProvider.GetDataTableForChart("RG_0017_0004_last_period", DataProvidersFactory.PrimaryMASDataProvider);
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
			Title.InnerHtml = "Индекс доверия институтам власти";
			TitleSub.InnerHtml = String.Format(
				"по данным социологических опросов за {0} {1} года по {2}",
				paramMonth.ToLower(), paramYear, RegionsNamingHelper.ShortName(paramTerritory));

			DataTable table = DataProvider.GetDataTableForChart("RG_0017_0004_power", DataProvidersFactory.PrimaryMASDataProvider);
			ElectorateGrid.Controls.Add(GenerateHtmlTable(table));
		}

		private HtmlTable GenerateHtmlTable(DataTable table)
		{
			HtmlTable htmlTable = new HtmlTable();
			htmlTable.CellSpacing = 0;
			htmlTable.Attributes.Add("class", "htmlGrid");

			// заголовок
			HtmlTableRow htmlRowHeader = new HtmlTableRow();
			for (int i = 0; i < table.Columns.Count; i++)
			{
				HtmlTableCell cell = new HtmlTableCell();
				cell.Attributes.Add("class", "header");
				cell.InnerHtml = String.Format("<div class=\"wrapper\"><div class=\"rotate\">{0}</div></div>",
					table.Columns[i].Caption
						.Replace("title", "&nbsp;")
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
				for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
				{
					HtmlTableCell cell = new HtmlTableCell();

					if (columnIndex == 0)
					{
						cell.Attributes.Add("class", "photo");
						cell.InnerHtml = table.Rows[rowIndex][columnIndex].ToString()
							.Replace("Правоохранительные", "Право-<br />охранительные");
					}
					else
					{
						if (!CRHelper.DBValueIsEmpty(table.Rows[rowIndex][columnIndex]))
						{
							int value = CRHelper.DBValueConvertToInt32OrZero(table.Rows[rowIndex][columnIndex]);
							maxValue = Math.Max(maxValue, value);
							minValue = Math.Min(minValue, value);
							cell.InnerHtml = value.ToString();
							if (value < 0)
								cell.Attributes.Add("class", "ShadowRed");
							else if (value > 0)
								cell.Attributes.Add("class", "ShadowGreen");
						}
					}

					htmlRow.Cells.Add(cell);
				}

				// звезды
				for (int columnIndex = 2; columnIndex < table.Columns.Count; columnIndex++)
				{
					if (!CRHelper.DBValueIsEmpty(table.Rows[rowIndex][columnIndex]))
					{
						HtmlTableCell cell = htmlRow.Cells[columnIndex];
						int value = CRHelper.DBValueConvertToInt32OrZero(table.Rows[rowIndex][columnIndex]);
						if (value == maxValue)
							cell.Style.Add("background", "url(../../../images/starYellowBB.png) no-repeat center 80%");
						else if (value == minValue)
							cell.Style.Add("background", "url(../../../images/starGrayBB.png) no-repeat center 80%");
					}
				}

				htmlTable.Rows.Add(htmlRow);
			}
			
			return htmlTable;
		}

    }
	
}