using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
	/// <summary>
	/// Название:	График размещения государственных и муниципальных заказов
	/// Описание:	Мониторинг размещения заказов по данным общероссийского сайта для размещения информации о размещении заказов
	/// Тип:		Оригинальный
	/// БД:			Реляционная: fmserv\mssql2005
	/// </summary>
    public partial class OOS_0001_0001 : CustomReportPage
    {
    	public readonly string TEMPORARY_URL_PREFIX = "../../..";
		public readonly string REPORT_ID = "OOS_0001_0001";

		private const int CALENDAR_MONTH_DISPERSION = 3;
		
		// параметры запросов
		private CustomParam customParamDate;
		private CustomParam customParamDateStart;
		private CustomParam customParamDateEnd;
		private CustomParam customParamTerritory;

		private int paramTerritoryId;
		private string paramTerritory;
		private DateTime paramDate;

		private Dictionary<int, int> Cube2Relational_CastTerra =
			new Dictionary<int, int>
				{
					{82, 59514}
				};

		private Dictionary<int, string[]> TypePurch =
 			new Dictionary<int, string[]>
 				{
 					{0, new []{"", ""}}, //Значение не указано
					{1, new []{"конкурс", "ShadowContest"}}, //Открытый конкурс
					{2, new []{"аукцион", "ShadowTender"}}, //Открытый аукцион
					{3, new []{"аукцион", "ShadowTender"}}, //Открытый аукцион в электронной форме
					{4, new []{"котировка", "ShadowQuote"}}, //Запрос котировок
					{5, new []{"отбор", "ShadowChoice"}}, //Предварительный отбор и запрос котировок при чрезвычайных ситуациях
					{6, new []{"", ""}}, //Единственный поставщик
					{7, new []{"", ""}}, //Размещение заказа на товарных биржах
					{8, new []{"интерес", "ShadowInterest"}}, //Сообщение о заинтересованности в проведении конкурса
 				};


		protected override void Page_PreLoad(object sender, EventArgs e)
		{
			base.Page_PreLoad(sender, e);
			
			// Инициализация параметров запроса
			customParamDate = UserParams.CustomParam("param_date");
			customParamDateStart = UserParams.CustomParam("param_date_start");
			customParamDateEnd = UserParams.CustomParam("param_date_end");
			customParamTerritory = UserParams.CustomParam("param_territory");
		}

    	protected override void Page_Load(object sender, EventArgs e)
        {
			base.Page_Load(sender, e);
			
			// значения по умолчанию
			paramDate = DateTime.Now.Date;
			paramTerritory = UserParams.StateArea.Value;
			paramTerritory = "Ямало-Ненецкий автономный округ";
			paramTerritoryId = Int32.Parse(CustomParams.GetSubjectIdByName(paramTerritory));

			if (!Cube2Relational_CastTerra.ContainsKey(paramTerritoryId))
				throw new Exception(
					String.Format(
						"В словаре Cube2Relational_CastTerra не найдено сопоставление территории с реляционной базой: {0}({1})",
						paramTerritory, paramTerritoryId));

			// параметры
    		customParamDate.Value = paramDate.ToString("yyyyMMdd");
			customParamTerritory.Value = Convert.ToString(Cube2Relational_CastTerra[paramTerritoryId]);

			// TouchElementBounds
			string filename = String.Format("{0}TouchElementBounds.xml", HttpContext.Current.Server.MapPath("~/TemporaryImages/"));
			StringBuilder xml = new StringBuilder();
			xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<touchElements>\n");
			xml.AppendFormat("<element id=\"OOS_0001_0002_{0}\" bounds=\"x=0;y=0;width=768;height=0\" openMode=\"\"/>\n", paramTerritoryId);
			xml.Append("</touchElements>");
			File.WriteAllText(filename, xml.ToString(), Encoding.UTF8);

			CurrentMonth.Value = paramDate.ToString("yyyy-M");
			using (IDatabase database = GetDataBase())
			{
				CreatePurchases(database);
    			CreateCalendar(database);
				CreateTopPurchases(database);
			}
        }

		#region Календарь

		private void CreateCalendar(IDatabase database)
		{
			const int monthWidth = 648;
			const int monthMargin = 10;
			CalendarHeader.Text = "Календарь размещения заказов";

			customParamDateStart.Value = String.Format("{0:yyyyMM}00", paramDate.AddMonths(-CALENDAR_MONTH_DISPERSION));
			customParamDateEnd.Value = String.Format("{0:yyyyMM}00", paramDate.AddMonths(CALENDAR_MONTH_DISPERSION + 1));
			DataTable dataTable = (DataTable)database.ExecQuery(DataProvider.GetQueryText("OOS_0001_0001_calendar"), QueryResultTypes.DataTable);

			if (dataTable.Rows.Count == 0)
			{
				CalendarBody.InnerHtml = "<span>Нет данных</span>";
				return;
			}
			MonthWidth.Value = Convert.ToString(monthWidth + (monthMargin*2));

			int rowIndex = 0;
			CalendarBody.Style.Add("margin-left", "auto");
			CalendarBody.Style.Add("margin-right", "auto");
			CalendarBody.Style.Add("overflow-x", "scroll");
			CalendarBody.Style.Add("-webkit-overflow-scrolling", "touch");
			HtmlGenericControl calendar = new HtmlGenericControl("div");
			calendar.Attributes.Add("id", "calendar");
			int width = 0;
			for (int i = -CALENDAR_MONTH_DISPERSION; i <= CALENDAR_MONTH_DISPERSION; i++)
			{
				if (String.Format("{0:yyyyMM}00", paramDate.AddMonths(i)).CompareTo(dataTable.Rows[dataTable.Rows.Count - 1][0].ToString()) >= 0)
					break;

				HtmlGenericControl month = new HtmlGenericControl("div");
				month.Controls.Add(CreateMonth(paramDate.AddMonths(i), dataTable, ref rowIndex));
				month.Attributes.Add("id", String.Format("month_{0}", paramDate.AddMonths(i).ToString("yyyy-M")));
				month.Style.Add("width", String.Format("{0}px", monthWidth));
				month.Style.Add("margin", String.Format("0 {0}px", monthMargin));
				month.Style.Add("display", "inline-block");
				month.Style.Add("vertical-align", "top");

				calendar.Controls.Add(month);

				width += monthWidth + (monthMargin * 2);
			}

			calendar.Style.Add("width", String.Format("{0}px", width));
			CalendarBody.Controls.Add(calendar);

		}

		private HtmlControl CreateMonth(DateTime month, DataTable dataTable, ref int rowIndex)
		{
			int totalDays = DateTime.DaysInMonth(month.Year, month.Month);

			HtmlTable table = new HtmlTable();
			table.CellSpacing = 0;
			table.Attributes.Add("class", "Calendar");
			table.Style.Add("width", "100%");
			HtmlTableRow row;
			HtmlTableCell cell;
			
			// смещение первого дня месяца в неделе
			DateTime monthDay = new DateTime(month.Year, month.Month, 1);
			int currentDay = (int)(monthDay.DayOfWeek) - 1;
			currentDay = -(currentDay == -1 ? 6 : currentDay) + 1;

			// кол-во недель в месяце
			int weeks = Convert.ToInt32(Math.Ceiling((-(currentDay - 1) + totalDays) / 7m));
			
			// заголовок месяца
			row = new HtmlTableRow();
			cell = new HtmlTableCell("th");
			cell.ColSpan = 7;
			if (paramDate.Year == month.Year && paramDate.Month == month.Month)
				cell.Attributes.Add("class", "MonthTitleActive");
			else
				cell.Attributes.Add("class", "MonthTitle");
			cell.InnerHtml = month.ToString("MMMM yyyy").ToUpperFirstSymbol();
			row.Cells.Add(cell);
			table.Rows.Add(row);

			// заголовки дней
			row = new HtmlTableRow();
			foreach (string day in new[] { "пн", "вт", "ср", "чт", "пт", "сб", "вс" })
			{
				cell = new HtmlTableCell("th");
				cell.Style.Add("width", "14.2857%");
				cell.InnerHtml = String.Format("<div class=\"BorderDashed\">{0}</div>", day);

				row.Cells.Add(cell);
			}
			table.Rows.Add(row);

			// ячейки дней
			for (int weekIndex = 0; weekIndex < weeks; weekIndex++)
			{
				row = new HtmlTableRow();
				for (int dayIndex = 0; dayIndex < 7; dayIndex++)
				{
					cell = new HtmlTableCell();
					cell.InnerHtml = "&nbsp;";
					row.Cells.Add(cell);
				}
				table.Rows.Add(row);
			}

			// дни месяца
			for (int weekIndex = 0; weekIndex < weeks; weekIndex++)
			{
				for (int dayIndex = 0; dayIndex < 7; dayIndex++)
			    {
					cell = table.Rows[2 + weekIndex].Cells[dayIndex];
			        if ((currentDay > 0) && (currentDay <= totalDays))
			        {
			            // реальные дни
			            StringBuilder innerHtml = new StringBuilder();
			            innerHtml.AppendFormat("<div class=\"CalendarDay\">{0}</div>", currentDay);

			            if (monthDay.ToString("yyyyMMdd") == dataTable.Rows[rowIndex][0].ToString())
			            {
			                innerHtml.Insert(0, String.Format("<a href=\"webcommand?showPinchReport=OOS_0001_0002_{0}&date={1}\">", paramTerritoryId, monthDay.ToString("yyyyMMdd")));
			                innerHtml.AppendFormat("<div class=\"CalendarCount\">{0}</div>", CRHelper.DBValueConvertToInt32OrZero(dataTable.Rows[rowIndex][1]));
			                string price = String.Empty;
			                if (!CRHelper.DBValueIsEmpty(dataTable.Rows[rowIndex][2]))
			                {
			                    price = String.Format("{0}", FormatCurrency(CRHelper.DBValueConvertToDecimalOrZero(dataTable.Rows[rowIndex][2]), 0));
			                }
			                innerHtml.AppendFormat("<div class=\"CalendarPrice\">{0}</div>", price);
			                innerHtml.Append("</a>");
			                if (rowIndex < dataTable.Rows.Count - 1)
			                {
			                    rowIndex++;
			                }
			            }
			            else
			            {
			                innerHtml.AppendFormat("<div class=\"CalendarCount\">&nbsp;</div>");
			                innerHtml.AppendFormat("<div class=\"CalendarPrice\">&nbsp;</div>");
			            }
			            cell.InnerHtml = innerHtml.ToString();

			            // оформление дня
			            string classes = "Day BorderTop BorderLeft";
			            if (dayIndex == 6) classes += " BorderRight";
			            if (weekIndex == weeks - 1) classes += " BorderBottom";
			            if (paramDate.Equals(monthDay)) classes += " Today";
						cell.Attributes.Add("class", classes);
			            monthDay = monthDay.AddDays(1);
			        }
			        else if (currentDay > 0)
			        {
						string classes = "BorderTop";
						if ((currentDay == totalDays + 1) && (dayIndex != 0)) classes += " BorderLeft";
						cell.Attributes.Add("class", classes);
			        }
			        currentDay++;
			    }
			}

			//width = dayTHWidth + dayTDWidth * weeks;
			return table;
		}

		#endregion 

		#region Объявленные закупки
		
		private void CreatePurchases(IDatabase database)
		{
			PurchasesHeader.Text = 
				String.Format(
					"В {0} и МО на&nbsp;<span class=\"DigitsValueLarge\">{1}</span>&nbsp;размещается заказов", 
					RegionsNamingHelper.ShortName(paramTerritory),
					paramDate.ToString("dd.MM.yyyy")
					);

			DataTable dataTable = (DataTable)database.ExecQuery(DataProvider.GetQueryText("OOS_0001_0001_declared_purchases"), QueryResultTypes.DataTable);
		
			StringBuilder html = new StringBuilder();
			html.Append("<table class=\"htmlGrid\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"width: 750px; margin: 0 auto;\">");
			html.Append("<tr>");
			html.Append("<th width=\"400\">&nbsp;</td>");
			html.Append("<th style=\"border-right: 1px solid #777;\" width=\"90\">&nbsp;</th>");
			html.Append("<th style=\"border-right: 1px solid #777;\" width=\"120\">Количество</td>");
			html.Append("<th width=\"150\">Сумма, млн. руб</td>");
			html.Append("</tr>");
			for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
			{
				DataRow row = dataTable.Rows[rowIndex];
				int typePurchID = CRHelper.DBValueConvertToInt32OrZero(row["TypePurchID"]);

				html.Append("<tr>");

				html.AppendFormat("<td style=\"text-align: left; padding-left: 10px; {1}\">{0}</td>",
					row["TypePurch"],
					rowIndex < dataTable.Rows.Count - 1 ? "border-bottom: 1px solid #777;" : String.Empty);

				html.AppendFormat("<td class=\"TypePurch {2}\" style=\"border-right: 1px solid #777; {1}\">{0}</td>",
					TypePurch[typePurchID][0],
					rowIndex < dataTable.Rows.Count - 1 ? "border-bottom: 1px solid #777;" : String.Empty,
					TypePurch[typePurchID][1]);

				html.AppendFormat("<td style=\"border-right: 1px solid #777; {1}\"><span class=\"DigitsValue\">{0:N0}</span></td>", 
					CRHelper.DBValueConvertToInt32OrZero(row["Count"]),
					rowIndex < dataTable.Rows.Count - 1 ? "border-bottom: 1px solid #777;" : String.Empty);
				
				html.AppendFormat("<td style=\"text-align: right; padding-right: 10px; {1}\"><span class=\"DigitsValue\">{0}</span></td>",
					!CRHelper.DBValueIsEmpty(row["Price"]) && typePurchID != 5 ? String.Format("{0:N3}", CRHelper.DBValueConvertToDecimalOrZero(row["Price"]) / 1000000) : String.Empty,
					rowIndex < dataTable.Rows.Count - 1 ? "border-bottom: 1px solid #777;" : String.Empty);
				
				html.Append("</tr>");
			}
			html.Append("</table>");
			PurchasesBody.InnerHtml = html.ToString();
		}
		
		#endregion 
		
		#region Топ закупок

		private void CreateTopPurchases(IDatabase database)
		{
			TopPurchasesHeader.Text = String.Format("Из них 10 самых крупных");

			DataTable dtData = (DataTable)database.ExecQuery(DataProvider.GetQueryText("OOS_0001_0001_top_purchases"), QueryResultTypes.DataTable);
			StringBuilder html = new StringBuilder();
			foreach (DataRow row in dtData.Rows)
			{
				int typePurchID = CRHelper.DBValueConvertToInt32OrZero(row["TypePurchID"]);
				DataItem item = 
					new DataItem
				    {
						MethodID = typePurchID,
						Method = TypePurch[typePurchID][0],
				        BudgetLevel = Convert.ToString(row["BudgetLevel"]),
				        Terra = Convert.ToString(row["Terra"]),
						Customer = Convert.ToString(row["Customer"]).ToUpperFirstSymbol(),
				        Purchase = Convert.ToString(row["Purchase"]).Trim().TrimEnd('.').ToUpperFirstSymbol(),
				        Price = CRHelper.DBValueIsEmpty(row["Price"]) ? (decimal?)null : Convert.ToDecimal(row["Price"]),
				        Link = Convert.ToString(row["Link"]),
				        RefPublicDate = Convert.ToInt32(row["RefPublicDate"]),
				        RefGiveDate = Convert.ToInt32(row["RefGiveDate"]),
				        RefConsiderDate = Convert.ToInt32(row["RefConsiderDate"]),
				        RefMatchDate = Convert.ToInt32(row["RefMatchDate"]),
				        RefResultDate = Convert.ToInt32(row["RefResultDate"])
				    };

				html.Append("<div class=\"PurWrapper\">");

				html.AppendFormat("<div class=\"PurMethod {1}\">{0}</div>", item.Method.ToUpperFirstSymbol(), TypePurch[item.MethodID][1]);
				
				html.AppendFormat("<div class=\"PurBoldWhite\">{0}__" +
				                  "<span class=\"PurLink\"><a href=\"{1}\">Подробнее на сайте zakupki.gov.ru >></a></span></div>", item.Purchase, item.Link);
				
				html.Append("<div>");
				html.AppendFormat("Источник финансирования:&nbsp;");
				switch (item.BudgetLevel)
				{
				    case "Уровень субъекта РФ":
						html.AppendFormat("<span class=\"PurBoldWhite\">Бюджет субъекта РФ</span>");
				        break;
				    case "Муниципальный уровень":
						html.AppendFormat("<span class=\"PurBoldWhite\">Бюджет муниципального образования ({0})</span>", item.Terra);
				        break;
				}
				html.Append("</div>");

				html.AppendFormat("<div>Заказчик:&nbsp;<span class=\"PurBoldWhite\">{0}</span></div>", item.Customer);

				if (item.Price != null)
					html.AppendFormat(
						"<div>Начальная максимальная цена контракта:&nbsp;" +
					    "<span class=\"PurBoldWhite {1}\" style=\"color: #fff !important;\">{0}</span></div>", 
						FormatCurrency(item.Price.Value, 3),
						TypePurch[typePurchID][1]);

				string stage = String.Empty;
				int currentDate = Convert.ToInt32(paramDate.ToString("yyyyMMdd"));
				switch (item.MethodID)
				{
					case 1:
						if (currentDate >= item.RefPublicDate && currentDate < item.RefGiveDate)
							stage = "Подача заявок";
						else if (currentDate == item.RefGiveDate)
							stage = "Вскрытие конвертов с заявками";
						else if (currentDate > item.RefGiveDate && currentDate < item.RefResultDate)
							stage = "Рассмотрение заявок";
						else if (currentDate == item.RefResultDate)
							stage = "Подведение итогов";
						break;
					case 2:
					case 3:
						if (currentDate >= item.RefPublicDate && currentDate <= item.RefGiveDate)
							stage = "Подача заявок";
						else if (currentDate > item.RefGiveDate && currentDate <= item.RefConsiderDate)
							stage = "Рассмотрение первых частей заявок";
						else if (currentDate == item.RefMatchDate)
							stage = "Начало аукциона";
						break;
					case 4:
						if (currentDate >= item.RefPublicDate && currentDate < item.RefGiveDate)
							stage = "Подача заявок";
						else if (currentDate == item.RefGiveDate)
							stage = "Подведение итогов";
						break;
					case 5:
						if (currentDate >= item.RefPublicDate && currentDate < item.RefGiveDate)
							stage = "Подача заявок";
						else if (currentDate == item.RefGiveDate)
							stage = "Подведение итогов отбора";
						break;
					case 8:
						if (currentDate >= item.RefPublicDate && currentDate < item.RefGiveDate)
							stage = "Подача заявок";
						else if (currentDate == item.RefGiveDate)
							stage = "Подведение итогов";
						break;
				}
				html.AppendFormat("<div>Стадия:&nbsp;<span class=\"PurBoldWhite\">{0}</span></div>", stage);

				if (item.RefPublicDate != -1)
					html.AppendFormat("<div>Публикация извещения:&nbsp;<span class=\"PurBoldWhite\">{0}</span></div>", DataItem.FormatDateValue(item.RefPublicDate));
				
				if (item.RefGiveDate != -1)
				{
					html.Append("<div>");
					string date = String.Format("<span class=\"PurBoldWhite\">{0}</span>", DataItem.FormatDateValue(item.RefGiveDate));
					switch (item.MethodID)
				    {
				        case 1:
				        case 8:
				            html.AppendFormat("Вскрытие заявок:&nbsp;{0}", date);
				            break;
				        case 4:
				        case 5:
				            html.AppendFormat("Рассмотрение котировочных заявок:&nbsp;{0}", date);
				            break;
				        case 2:
						case 3:
				            html.AppendFormat("Окончание подачи заявок:&nbsp;{0}", date);
				            break;
				    }
					html.Append("</div>");
				}

				if (item.RefConsiderDate != -1)
				{
					html.Append("<div>");
					string date = String.Format("<span class=\"PurBoldWhite\">{0}</span>", DataItem.FormatDateValue(item.RefConsiderDate));
				    switch (item.MethodID)
				    {
				        case 1:
				            html.AppendFormat("Рассмотрение заявок:&nbsp;{0}", date);
				            break;
				        case 2:
						case 3:
				            html.AppendFormat("Рассмотрение первых частей заявок:&nbsp;{0}", date);
				            break;
				    }
					html.Append("</div>");
				}

				if (item.RefMatchDate != -1)
				{
					html.Append("<div>");
					string date = String.Format("<span class=\"PurBoldWhite\">{0}</span>", DataItem.FormatDateValue(item.RefMatchDate));
				    switch (item.MethodID)
				    {
				        case 2:
						case 3:
				            html.AppendFormat("Проведение аукциона в электронной форме:&nbsp;{0}", date);
				            break;
				    }
					html.Append("</div>");
				}

				if (item.RefResultDate != -1)
				{
					html.Append("<div>");
					string date = String.Format("<span class=\"PurBoldWhite\">{0}</span>", DataItem.FormatDateValue(item.RefResultDate));
				    switch (item.MethodID)
				    {
				        case 1:
				            html.AppendFormat("Подведение итогов:&nbsp;{0}", date);
				            break;
				    }
					html.Append("</div>");
				}
				
				html.Append("</div>");
				
			}
			TopPurchasesBody.InnerHtml = html.ToString();
		}

		#endregion

		private static string FormatCurrency(decimal currency, int digits)
		{
			string format = String.Format("N{0}", digits);
			if (currency < 1000)
				return String.Format("{0} руб", currency.ToString(format));
			if (currency < 1000000)
				return String.Format("{0} тыс. руб", (currency/1000m).ToString(format));
			if (currency < 10000000000)
				return String.Format("{0} млн. руб", (currency / 1000000m).ToString(format));
			return String.Format("{0} млрд. руб", (currency / 1000000000m).ToString(format));
		}

		private static IDatabase GetDataBase()
		{
			try
			{
				HttpSessionState sessionState = HttpContext.Current.Session;
				LogicalCallContextData cnt =
					sessionState[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
				if (cnt != null)
					LogicalCallContextData.SetContext(cnt);
				IScheme scheme = (IScheme)sessionState[ConnectionHelper.SCHEME_KEY_NAME];
				return scheme.SchemeDWH.DB;
			}
			finally
			{
				CallContext.SetData("Authorization", null);
			}
		}
    }

	public class DataItem
	{
		public string Method { set; get; }
		public int MethodID { set; get; }
		public string BudgetLevel { set; get; }
		public string Terra { set; get; }
		public string Customer { set; get; }
		public string Purchase { set; get; }
		public decimal? Price { set; get; }
		public string Link { set; get; }
		public int RefPublicDate { set; get; }
		public int RefGiveDate { set; get; }
		public int RefConsiderDate { set; get; }
		public int RefMatchDate { set; get; }
		public int RefResultDate { set; get; }

		public static string FormatDateValue(int date)
		{
			string str = date.ToString();
			return String.Format("{0}.{1}.{2}", str.Substring(6, 2), str.Substring(4, 2), str.Substring(0, 4));
		}
	}
}