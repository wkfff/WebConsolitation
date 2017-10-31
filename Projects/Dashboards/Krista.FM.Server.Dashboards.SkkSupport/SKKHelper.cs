using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
    public static class SKKHelper
    {
    	public const int defaultYear = 2010;
		public const string defaultMonth = "������";

    	public const string noDataBordersByTransport = "��� ������, �.�. ��� ���������� ���� ��������� ��� ������� ��������, ������������� �� ������� ��";
		public const string noDataBordersByTerra = "��� ������, �.�. ����� ��������� ���������� �� �������� �� ���� ������� ������� ��";

		public const int scrollWidth = 17;
		public const int defRowsCount = 11;
    	public const int maxBordersCount = 15;
		public const int maxStringLength = 25;
		public const int minPageWidth = 1210;
		public const int ExportPageWidth = 1100;
		public const int reservedHeight = 260;
		public const int paragraphLength = 25;
		public const int paragraphLengthSymbols = 5;

		public static int defaultMapWidth = Convert.ToInt32(CustomReportConst.minScreenWidth * (0.95));
		public const int defaultMapHeight = 500;

		public const int defaultGridHeight = 400;
		public const int defaultChartHeight = 340;
		public const int defaultChartCompareHeight = 200;
		public const int defaultChartBordersHeight = 410;
    	public const int defaultChartNoDataHeight = 120;

		public static int defaultHelpItemWidth = 800;
		public static int defaultHeightFull = Convert.ToInt32(CustomReportConst.minScreenHeight - reservedHeight);
    	public static int default1ItemsWidth = Convert.ToInt32(CustomReportConst.minScreenWidth * (0.63));
		public static int default2ItemsWidth = Convert.ToInt32(CustomReportConst.minScreenWidth * (0.48));
		
		private const int comboYearWidth = 115;
		private const int comboBaseWidth = 276;

    	#region ��������� ���������� �������

		public static MemberAttributesDigest DigestInit(string queryID)
		{
			return
				new MemberAttributesDigest(
					DataProvidersFactory.PrimaryMASDataProvider,
					queryID,
					Query.queriesPath
					);
		}

    	public static ParamDate ParamDate_Init()
		{
			return new ParamDate("skk_queries_param_date_default");
		}

		public static void ComboCurrentYear_Init(this CustomMultiCombo combo, int firstYear, int lastYear, int selectedYear)
		{
			combo.Title = "���";
			combo.Width = comboYearWidth;
			combo.MultiSelect = false;
			combo.ShowSelectedValue = true;
			combo.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastYear));
			combo.Set�heckedState(selectedYear.ToString(), true);
		}

		public static void ComboCurrentMonth_Init(this CustomMultiCombo combo, string selectedMonth)
		{
			combo.Title = "�����(�)";
			combo.Width = comboBaseWidth;
			combo.MultiSelect = true;
			combo.ShowSelectedValue = true;
			combo.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
			combo.Set�heckedState(CRHelper.ToUpperFirstSymbol(selectedMonth), true);
		}

		public static MemberAttributesDigest DigestDirection_Init()
		{
			return DigestInit("skk_queries_param_direction");
		}

		public static void ComboDirection_Init(this CustomMultiCombo combo, MemberAttributesDigest digest)
		{
			combo.Title = "�����������";
			combo.Width = comboBaseWidth;
			combo.MultiSelect = false;
			combo.ParentSelect = false;
			combo.ShowSelectedValue = true;
			combo.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(digest.UniqueNames, digest.MemberLevels));
		}

    	public static MemberAttributesDigest DigestBorder_Init()
    	{
    		return DigestInit("skk_queries_param_border");
		}

    	public static void ComboBorder_Init(this CustomMultiCombo combo, MemberAttributesDigest digest, bool TotalIsPresent = true)
		{
			combo.Title = "������� ���.������� ��";
			combo.Width = comboBaseWidth + 6 + comboBaseWidth / 2;
			combo.MultiSelect = false;
			combo.ParentSelect = false;
			combo.ShowSelectedValue = true;
			Dictionary<string, int> dict = CustomMultiComboDataHelper.FillMemberUniqueNameList(digest.UniqueNames, digest.MemberLevels);
			if (!TotalIsPresent)
				dict.Remove("�����");
			combo.FillDictionaryValues(dict);
		}

		public static MemberAttributesDigest DigestPoint_Init()
		{
			return DigestInit("skk_queries_param_point");
		}

		public static void ComboPoint_Init(this CustomMultiCombo combo, MemberAttributesDigest digest)
		{
			combo.Title = "����� ��������";
			combo.Width = comboBaseWidth + 6 + comboBaseWidth;
			combo.MultiSelect = false;
			combo.ParentSelect = false;
			combo.ShowSelectedValue = true;
			combo.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(digest.UniqueNames, digest.MemberLevels));
		}

		public static MemberAttributesDigest DigestTerra_Init()
		{
			return DigestInit("skk_queries_param_terra");
		}

		public static void ComboTerra_Init(this CustomMultiCombo combo, MemberAttributesDigest digest)
		{
			combo.Title = "����������";
			combo.Width = comboBaseWidth + 6 + comboBaseWidth / 2;
			combo.MultiSelect = false;
			combo.ParentSelect = true;
			combo.ShowSelectedValue = true;
			combo.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(digest.UniqueNames, digest.MemberLevels));
			combo.Set�heckedState("��", true);
		}

		public static MemberAttributesDigest DigestTransport_Init()
		{
			return DigestInit("skk_queries_param_transport");
		}

		public static void ComboTransport_Init(this CustomMultiCombo combo, MemberAttributesDigest digest)
		{
			combo.Title = "��� ���������";
			combo.Width = comboBaseWidth;
			combo.MultiSelect = false;
			combo.ParentSelect = false;
			combo.ShowSelectedValue = true;
			combo.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(digest.UniqueNames, digest.MemberLevels));
		}

		public static MemberAttributesDigest DigestMark_Init()
		{
			return DigestInit("skk_queries_param_mark");
		}

		public static void ComboMark_Init(this CustomMultiCombo combo, MemberAttributesDigest digest)
		{
			combo.Title = "����������";
			combo.Width = comboBaseWidth + 6 + comboBaseWidth;
			combo.MultiSelect = false;
			combo.ParentSelect = false;
			combo.ShowSelectedValue = true;
			combo.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(digest.UniqueNames, digest.MemberLevels));
		}

		public static void CheckDetailPP_Init(this CheckBox checkBox)
		{
			checkBox.Text = "�������������� �� ������� ��������";
			checkBox.Width = comboBaseWidth;
			checkBox.Checked = false;
		}

		public static void CheckDetailPP_PostInit(this CheckBox checkBox)
		{
			checkBox.InputAttributes.Add("onclick", "document.getElementById('RefreshButton').className='Button';");
		}

		public static string CheckDetailPP_GetDetailTerra(this CheckBox checkBox)
		{
			return checkBox.Checked ? "SELF_BEFORE_AFTER" : "SELF_AND_BEFORE";
		}

		public static string CheckDetailPP_GetDetailBorder(this CheckBox checkBox)
		{
			return checkBox.Checked ? "SELF_AND_AFTER" : "SELF";
		}

		public static string CheckDetailPP_GetDetailTransport(this CheckBox checkBox)
		{
			return checkBox.Checked ? "true" : "false";
		}

    	#endregion

		#region ��������� ���������� �������

		public static string GetUniqueMonths(Collection<string> monthsArray, string year)
		{
			string months = String.Empty;
			foreach (string month in monthsArray)
			{
				months += String.Format("[�������].[���_�����].[���].[{0}].[{1}], ", year, month.ToLower());
			}
			if (months.Length > 0)
			{
				months = months.Remove(months.Length - 2, 2);
			}
			return months;
		}

		public static string GetAblativeMonths(Collection<string> monthsArray)
		{
			string months = String.Empty;
			foreach (string month in monthsArray)
			{
				months += CRHelper.RusMonthTvorit(CRHelper.MonthNum(month.ToLower())) + ", ";
			}
			if (months.Length > 0)
			{
				months = months.Remove(months.Length - 2, 2);
			}
			return months;
		}

		public static void ChangeSelectedPeriod(CustomMultiCombo combo)
		{
			if (String.IsNullOrEmpty(combo.SelectedValuesString.Trim()))
			{
				combo.Set�heckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(1)), true);
			}
		}

		public static void ChangeSelectedPeriodCompare(CustomMultiCombo combo)
		{
			if (String.IsNullOrEmpty(combo.SelectedValuesString.Trim()))
			{
				combo.Set�heckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(1)), true);
			}
		}

    	#endregion

		#region �������� � ���������

    	/// <summary>
    	/// ������ ������, ������ �� ���������
    	/// </summary>
    	public static void SetDefaultStyle(this UltraGridBand band, int defaultWidth, string defaultFormat, bool setFirstColumn)
    	{
    		int start = setFirstColumn ? 0 : 1;
			band.SetDefaultStyle(defaultWidth, defaultFormat, start);
		}

		/// <summary>
		/// ������ ������, ������ �� ���������
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
		/// ���������� ��������� ������ � ���� �������
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
		/// �������� ������ ������� �� ���������
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
			if (columnName.ToLower().Contains("����") && columnName.ToLower().Contains("��������"))
			{
				return 95;
			}
			return defaultWidth;
		}

    	#endregion

		#region �������� � �����������

		public static void SetLogarithmicAxis(this AxisAppearance axis, double maxValue)
		{
			axis.TickmarkStyle = AxisTickStyle.Percentage;
			axis.TickmarkPercentage = 100 / (Math.Log10(maxValue) + 1);
			axis.LogBase = 10;
			axis.LogZero = 0.000001;
			axis.NumericAxisType = NumericAxisType.Logarithmic;
			axis.RangeType = AxisRangeType.Custom;
			axis.RangeMax = maxValue;
			axis.RangeMin = 0.1;
		}

		public static double SetLogarithmicScale(this AxisAppearance axis, double maxValue)
		{
			if (maxValue <= 0)
			{
				maxValue = 1;
			}

			double result = Math.Pow(10, Math.Ceiling(Math.Log10(maxValue)));
			axis.TickmarkPercentage = 100 / (Math.Log10(result) + 1);
			axis.RangeMax = result;

			return result;
		}

    	#endregion

		#region ��������� ��������

		public static void ExportExcelSetStyle(ReportExcelExporter report)
		{
			report.SheetColumnCount = 15;
			report.HeaderCellHeight = 20;
			report.GridColumnWidthScale = 1.3;
			report.RowsAutoFitEnable = true;
		}

		public static void ExportPdfSetStyle(ReportPDFExporter report)
		{
			report.HeaderCellHeight = 20;
		}

    	#endregion

		#region javascript-���������

		/// <summary>
		/// 
		/// </summary>
		public static string JavaScript_GetVScroll(UltraGridBrick grid)
		{
			return String.Format(
					"SetGridScrollBar('{0}', '{1}', '{2}');\n",
					"G_" + grid.Grid.ClientID.Replace("_", "x"),
					grid.Grid.ClientID.Replace("_", "x") + "_main",
					scrollWidth);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void JavaScript_SetVScrollOnLoad(this HtmlGenericControl tag, string jsCode)
		{
			StringBuilder scriptString = new StringBuilder();
			scriptString.Append("\n<script type='text/javascript'><!--\n");
			scriptString.AppendFormat("window.onload = function(){{\ngetSize();\n{0}\n}};\n", jsCode);
			scriptString.Append(GetJSVerticalScrollBar());
			scriptString.Append("\n//--></script>\n");
			tag.InnerHtml = scriptString.ToString();
		}

		/// <summary>
		/// �������� javascript, ���������� ������ ����� ��� ������������ ������ ���������
		/// </summary>
		public static void JavaScript_SetVerticalScrollBar(this HtmlGenericControl tag, UltraGridBrick grid)
		{
			StringBuilder scriptString = new StringBuilder();
			scriptString.Append("\n<script type='text/javascript'><!--\n");
			scriptString.AppendFormat(
					"window.onload = function(){{\ngetSize();\nSetGridScrollBar('{0}', '{1}', '{2}');\n}}\n",
					"G_" + grid.Grid.ClientID.Replace("_", "x"),
					grid.Grid.ClientID.Replace("_", "x") + "_main",
					scrollWidth);
			scriptString.Append(GetJSVerticalScrollBar());
			scriptString.Append("\n//--></script>\n");

			tag.InnerHtml = scriptString.ToString();
		}

		/// <summary>
		/// ����� javascript-�������, ������������� ������ ��������
		/// </summary>
		private static string GetJSVerticalScrollBar()
		{
			StringBuilder scriptString = new StringBuilder();
			scriptString.Append(
@"function SetGridScrollBar(data, main, scrollWidth)
{
	var data_obj = ig_csom.getElementById(data);
	var main_obj = ig_csom.getElementById(main);
	main_obj.style.width = parseInt(data_obj.style.width) + parseInt(scrollWidth) + 'px';
}");
			return scriptString.ToString();
		}

    	#endregion

		#region ��������� �����������

		public static void IndicatorBestWorseInit(this StarIndicatorRule rule)
		{
			rule.BestRankImg = "~/images/max_pc.png"; 
			rule.BestRankHint = "���������� ��������";
			rule.WorseRankImg = "~/images/min_pc.png";
			rule.WorseRankHint = "���������� ��������";
		}

		public static void IndicatorUpDownInit(this GrowRateScaleRule rule)
		{
			rule.HintUp = "���� � ������� ��� ���������";
			rule.HintDown = "�������� � ������� ��� ���������";
			rule.ImageUp = "~/images/arrowRedUpBB.png";
			rule.ImageDown = "~/images/arrowGreenDownBB.png";
		}
		
		public static void IndicatorUpDownInit(this GrowRateScaleRule rule, string defaultSI, string defaultFormat)
		{
			rule.IndicatorUpDownInit();
			rule.SI = defaultSI;
			rule.Format = defaultFormat;
		}

		#endregion

		#region ��������������� �������

		public static bool IsMozilla()
		{
			return 
				HttpContext.Current.Request.Browser.Browser.ToLower().Contains("firefox");
		}

		/// <summary>
		/// ������� ������ �� ������ �� ��������� �����
		/// </summary>
		public static string WrapString(string text, int maxLength, string eoln)
		{
			string result = String.Empty;

			// �������� ������� � ������������
			string newEoln = eoln.Replace(" ", "_");
			text = text.Replace(eoln, newEoln);
			
			string[] lines = text.Replace(newEoln, " "+newEoln+" ").Split(new[]{" "}, StringSplitOptions.RemoveEmptyEntries);
			int length = 0;
			foreach (string line in lines)
			{
				if (line.Equals(newEoln) || (length + 1 + line.Length > maxLength))
				{
					length = 0;
					result += newEoln;
				}
				else
				{
					length ++;
					result += " ";
				}

				if (!line.Equals(newEoln))
				{
					length += line.Length;
					result += line;
				}

			}

			return result.Replace(newEoln, eoln);
		}

		/// <summary>
		/// ����� �������� ����� � ������
		/// </summary>
		public static int CountStrings(string text, string eoln)
		{
			MatchCollection matches = Regex.Matches(text, eoln);
			return matches.Count+1;
		}

		public static string AddParagraph()
		{
			return String.Format("<span style='padding-left: {0}px;'></span>", paragraphLength);
		}

		public static string AddParagraphSpaces()
		{
			StringBuilder text = new StringBuilder();
			for (int i = 0; i < paragraphLengthSymbols; i++)
			{
				text.Append((char) 160);
			}
			return text.ToString();
		}
		
		#endregion

		#region ����������, ���������� �����

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

		public enum DirectionType
		{
			In, Out
		}
	}
}
