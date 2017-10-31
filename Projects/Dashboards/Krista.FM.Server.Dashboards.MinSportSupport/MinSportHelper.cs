using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.MinSportSupport
{
    public static class SportHelper
    {
    	public const int defaultYear = 2010;
		public const string defaultMonth = "январь";

		public const int scrollWidth = 17;
		public const int defRowsCount = 11;
    	public const int maxBordersCount = 15;
		public const int maxStringLength = 25;
		public const int minPageWidth = 1210;
		public const int ExportPageWidth = 1100;
		public const int reservedHeight = 260;
		public const int paragraphLength = 25;

		public static int defaultMapWidth = Convert.ToInt32(CustomReportConst.minScreenWidth * (0.95));
		public const int defaultMapHeight = 500;

		public const int defaultGridHeight = 400;
		public const int defaultChartHeight = 340;
		public const int defaultChartCompareHeight = 200;
		public const int defaultChartBordersHeight = 410;
    	public const int defaultChartNoDataHeight = 120;

		public static int defaultHeightFull = Convert.ToInt32(CustomReportConst.minScreenHeight - reservedHeight);
    	public static int default1ItemsWidth = Convert.ToInt32(CustomReportConst.minScreenWidth * (0.63));
		public static int default2ItemsWidth = Convert.ToInt32(CustomReportConst.minScreenWidth * (0.48));
		
		private const int comboYearWidth = 115;
		private const int comboBaseWidth = 276;

    	#region Настройка параметров отчетов

		public static MemberAttributesDigest DigestInit(string queryID)
		{
			return
				new MemberAttributesDigest(
					DataProvidersFactory.PrimaryMASDataProvider,
					queryID,
					QueryWorker.queriesPath
					);
		}


		public static void ComboCurrentYear_Init(this CustomMultiCombo combo, int firstYear, int lastYear, int selectedYear)
		{
			combo.Title = "Год";
			combo.Width = comboYearWidth;
			combo.MultiSelect = false;
			combo.ShowSelectedValue = true;
			combo.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastYear));
			combo.SetСheckedState(selectedYear.ToString(), true);
		}

		public static MemberAttributesDigest DigestTerra_Init()
		{
            return DigestInit("minsport_queries_param_territory");
		}

        public static MemberAttributesDigest DigestFederalDistrict_Init()
        {
            return DigestInit("minsport_queries_param_territory_federal_district");
        }

        public static MemberAttributesDigest DigestYear_Init()
        {
            return DigestInit("minsport_queries_param_year");
        }

        public static void ComboTerra_Init(this CustomMultiCombo combo, MemberAttributesDigest digest)
		{
			combo.Title = "Территория";
			combo.Width = comboBaseWidth + 6 + comboBaseWidth / 2;
			combo.MultiSelect = false;
			combo.ParentSelect = true;
			combo.ShowSelectedValue = true;
			combo.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(digest.UniqueNames, digest.MemberLevels));
			combo.SetСheckedState("РФ", true);
		}

        public static void ComboTerritoryForConverter_Init(this CustomMultiCombo combo, MemberAttributesDigest digest)
        {
            combo.Title = "Территория";
            combo.Width = comboBaseWidth + 6 + comboBaseWidth / 2;
            combo.MultiSelect = true;
            combo.MultipleSelectionType = MultipleSelectionType.SimpleMultiple;
            combo.ShowSelectedValue = true;
            combo.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(digest.UniqueNames, digest.MemberLevels));
            combo.SetСheckedState("РФ", true);
        }

        public static void ComboYear_Init(this CustomMultiCombo combo, MemberAttributesDigest digest)
        {
            combo.Title = "Год";
            combo.Width = comboBaseWidth + 6 + comboBaseWidth / 2;
            combo.MultiSelect = false;
            combo.ParentSelect = true;
            combo.ShowSelectedValue = true;
            combo.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(digest.UniqueNames, digest.MemberLevels));
            //combo.SetСheckedState("РФ", true);
        }

		

    	#endregion

		#region Операции с таблицами

    	/// <summary>
    	/// формат вывода, ширина по умолчанию
    	/// </summary>
    	public static void SetDefaultStyle(this UltraGridBand band, int defaultWidth, string defaultFormat, bool setFirstColumn)
    	{
    		int start = setFirstColumn ? 0 : 1;
			band.SetDefaultStyle(defaultWidth, defaultFormat, start);
		}

		/// <summary>
		/// формат вывода, ширина по умолчанию
		/// </summary>
		public static void SetDefaultStyle(this UltraGridBand band, int defaultWidth, string defaultFormat, int startColumn)
		{
			for (int i = startColumn; i < band.Columns.Count; i = i + 1)
			{
				string columnCaption = band.Columns[i].Header.Caption;

				string formatString = GetColumnFormat(columnCaption, defaultFormat);
				CRHelper.FormatNumberColumn(band.Columns[i], formatString);

				int columnWidth = GetColumnWidth(columnCaption, defaultWidth);
				band.Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);

				band.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
			}
		}

		/// <summary>
		/// установить указанную ширину у всех колонок
		/// </summary>
		public static void SetConstWidth(this UltraGridBand band, int defaultWidth, bool setFirstColumn)
		{
			int start = setFirstColumn ? 0 : 1;
			for (int i = start; i < band.Columns.Count; i = i + 1)
			{
				band.Columns[i].Width = CRHelper.GetColumnWidth(defaultWidth);
			}
		}

		/// <summary>
		/// изменить ширину колонки на множитель
		/// </summary>
		public static void SetColumnWidth(this UltraGridColumn column, double scaleWidth)
		{
			if (!column.Width.IsEmpty && column.Width.Value > 0)
			{
				column.Width = Convert.ToInt32(column.Width.Value*scaleWidth);
			}
		}

		public static string GetColumnFormat(string columnName, string defaultFormat)
		{
			if (columnName.ToLower().Contains("%"))
			{
				return "P2";
			}
			if (columnName.ToLower().Contains("#"))
			{
				return "N0";
			}
			return defaultFormat;
		}

		public static int GetColumnWidth(string columnName, int defaultWidth)
		{
			if (columnName.ToLower().Contains("%"))
			{
				return 75;
			}
			if (columnName.ToLower().Contains("рост") && columnName.ToLower().Contains("снижение"))
			{
				return 95;
			}
			return defaultWidth;
		}

    	#endregion

		#region Вспомогательные функции

		public static bool IsMozilla()
		{
			return 
				HttpContext.Current.Request.Browser.Browser.ToLower().Contains("firefox");
		}

        /// <summary>
        /// Выполнить MDX запрос
        /// </summary>
        public static DataTable ExecQuery(DataProvider dataProvider, string query)
        {
            var data = new DataTable();
            dataProvider.GetDataTableForChart(query, "dummy", data);
            return data;
        }

        /// <summary>
        /// Формирование имени файла экспорта по правилам ЕМИСС 
        /// </summary>
        public static string FormattionFileName(string parcelCode, string factorCode)
        {
            return String.Format("{0}-{1}-{2}-1-{3}.xls", XmlWorker.GetDepartmentCode(),
                                 String.Format("{0:00000000}", Convert.ToInt32(parcelCode)),
                                 factorCode,
                                 String.Format("{0:yyyyMMdd}", DateTime.Now));
        }

		#endregion

        #region Форматирование html 

        public static HtmlGenericControl StyleSimple(HtmlGenericControl html)
        {
            html.Style.Add("margin", "5px 0");
            return html;
        }

        public static HtmlGenericControl StyleError(HtmlGenericControl html)
        {
            html.Style.Add("font-size", "10pt");
            html.Style.Add("font-color", "red");
            return html;
        }

        public static StringBuilder Text(string text)
        {
            return new StringBuilder(text);
        }

        public static HtmlGenericControl ToHTML(StringBuilder text)
        {
            var item = new HtmlGenericControl("div") { InnerHtml = text.ToString() };
            return item;
        }

        public static HtmlGenericControl StyleMainTitle(HtmlGenericControl html)
        {
            html.Style.Add("margin", "10px 0");
            html.Style.Add("text-align", "center");
            html.Style.Add("font-size", "12pt");
            html.Style.Add("font-weight", "bold");
            return html;
        }

        #endregion

        #region Расширения, упрощающие жизнь

        public static void AddRewrite(this Dictionary<string, string> dictionary, string key, string value)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary.Remove(key);
			}
			dictionary.Add(key, value);
		}

		public static void RemoveAnyway(this Dictionary<string, string> dictionary, string key)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary.Remove(key);
			}
		}

    	#endregion

        public static string GetTerritoryShortNameForMap(string fullName)
        {
            string upperName = fullName.ToUpper();

            if (upperName.Contains("ЦЕНТРАЛ")) return "ЦФО";
            if (upperName.Contains("ЮЖНЫЙ")) return "ЮФО";
            if (upperName.Contains("УРАЛЬС")) return "УрФО";
            if (upperName.Contains("ДАЛЬНЕВ")) return "ДФО";
            if (upperName.Contains("СЕВЕРО-З")) return "СЗФО";
            if (upperName.Contains("СЕВЕРО-К")) return "СКФО";
            if (upperName.Contains("ПРИВОЛЖ")) return "ПФО";
            if (upperName.StartsWith("СИБИРС")) return "СФО";
            if (upperName.Contains("РОССИЙСК")) return "РФ";

            return fullName;
        }
    }
}
