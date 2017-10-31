using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	/// <summary>
	/// таблица по группам товаров
	/// </summary>
	public class GridGroups : GridBase
	{

		public GridGroups(UltraGridBrick grid) 
			: base(grid)
		{
			Grid = grid;
		}

		/// <summary>
		/// настройка внешнего вида 
		/// </summary>
		public override void SetStyle()
		{
			SetFullAutoSizes();
		}

		/// <summary>
		/// настройка внешнего вида данных
		/// </summary>
		public override void SetDataStyle()
		{
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = CRHelper.GetColumnWidth(60);
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
			
			Band.HideColumns(16);

			Band.SetDefaultStyle(90, "N3", false);
			
			Band.Columns[1].SetColumnWidth(0.9);
			Band.Columns[2].SetColumnWidth(0.9);
			Band.Columns[3].SetColumnWidth(0.5);
			Band.Columns[5].SetColumnWidth(0.5);
			Band.Columns[10].SetColumnWidth(0.7);
			Band.Columns[12].SetColumnWidth(0.7);
		}

		/// <summary>
		/// установить одинаковую ширину для всех колонок
		/// </summary>
		public void SetConstColumnWidth()
		{
			Band.SetConstWidth(90, false);
		}

		/// <summary>
		/// настройка индикаторов
		/// </summary>
		public override void SetDataRules()
		{
			// правило для первой строки
			FontRowLevelRule levelRule = new FontRowLevelRule(Band.Columns.Count - 1);
			levelRule.AddFontLevel("0", Grid.BoldFont8pt);
			Grid.AddIndicatorRule(levelRule);

			// правило для 1 столбца
			Grid.AddIndicatorRule(new HintRule(0, "ГруппыОписание"));

			// правила наибольшее/наименьшее значение
			StarIndicatorRule ruleStar;

			ruleStar = new StarIndicatorRule(4, "*ед., (ранг)");
			ruleStar.IndicatorBestWorseInit();
			Grid.AddIndicatorRule(ruleStar);

			ruleStar = new StarIndicatorRule(6, "*ед.Сравн, (ранг)");
			ruleStar.IndicatorBestWorseInit();
			Grid.AddIndicatorRule(ruleStar);

			ruleStar = new StarIndicatorRule(11, "*т., (ранг)");
			ruleStar.IndicatorBestWorseInit();
			Grid.AddIndicatorRule(ruleStar);

			ruleStar = new StarIndicatorRule(13, "*т.Сравн, (ранг)");
			ruleStar.IndicatorBestWorseInit();
			Grid.AddIndicatorRule(ruleStar);

			// правила для малых процентов
			Grid.AddIndicatorRule(new ReplaceValueColumnRule(4, "", "*ед., %доп"));
			Grid.AddIndicatorRule(new ReplaceValueColumnRule(6, "", "ПриостановленоСравн, ед%доп"));
			Grid.AddIndicatorRule(new ReplaceValueColumnRule(11, "", "*т., %доп"));
			Grid.AddIndicatorRule(new ReplaceValueColumnRule(13, "", "ПриостановленоСравн, т%доп"));

			// правила рост/снижение
			GrowRateScaleRule ruleUpDown;

			ruleUpDown = new GrowRateScaleRule(7, "*рост/снижение, ед. (флаг)", "*рост/снижение, ед. (СИ)", "*рост/снижение, ед. (формат)");
			ruleUpDown.IndicatorUpDownInit();
			Grid.AddIndicatorRule(ruleUpDown);
			
			ruleUpDown = new GrowRateScaleRule(14, "*рост/снижение, т. (флаг)", "*рост/снижение, т. (СИ)", "*рост/снижение, т. (формат)");
			ruleUpDown.IndicatorUpDownInit("т", "N3"); 
			Grid.AddIndicatorRule(ruleUpDown);
		}

		/// <summary>
		/// заголовки таблицы
		/// </summary>
		public override void SetDataHeader()
		{
			
			GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
			GridHeaderCell groupCell_1;
			GridHeaderCell groupCell_2;
			GridHeaderCell groupCell_3;

			headerLayout.AddCell("Группы товаров", 4);

			groupCell_1 = headerLayout.AddCell("Число партий грузов");
			groupCell_2 = groupCell_1.AddCell("Досмотрено");
			groupCell_2.AddCell("Текущий период").AddCell("абс");
			groupCell_2.AddCell("Период для сравнения").AddCell("абс");
			groupCell_2 = groupCell_1.AddCell("Приостановлено");
			groupCell_3 = groupCell_2.AddCell("Текущий период");
			groupCell_3.AddCell("абс");
			groupCell_3.AddCell("%");
			groupCell_3 = groupCell_2.AddCell("Период для сравнения");
			groupCell_3.AddCell("абс");
			groupCell_3.AddCell("%");
			groupCell_2.AddCell("Рост/<br />снижение абс.<br />показателя", 2);

			groupCell_1 = headerLayout.AddCell("Объем партий грузов");
			groupCell_2 = groupCell_1.AddCell("Досмотрено");
			groupCell_2.AddCell("Текущий период").AddCell("т");
			groupCell_2.AddCell("Период для сравнения").AddCell("т"); 
			groupCell_2 = groupCell_1.AddCell("Приостановлено");
			groupCell_3 = groupCell_2.AddCell("Текущий период");
			groupCell_3.AddCell("абс");
			groupCell_3.AddCell("%");
			groupCell_3 = groupCell_2.AddCell("Период для сравнения");
			groupCell_3.AddCell("абс");
			groupCell_3.AddCell("%");
			groupCell_2.AddCell("Рост/<br />снижение абс.<br />показателя", 2);

			headerLayout.AddNumericCells(0);

			Grid.GridHeaderLayout.ApplyHeaderInfo();
		}

		/// <summary>
		/// установить данные
		/// </summary>
		/// <param name="queryName"></param>
		public override void SetData(string queryName)
		{
			Query = new Query(queryName);
			QueryTemplate template;

			// [Measures].[ед., %]
			template = new QueryTemplate("skk_part_percent");
			template.AddRule("member_base", "[Measures].[ед., %]");
			template.AddRule("member_plus", "[Measures].[*ед., %доп]");
			template.AddRule("op1", "[Measures].[Приостановлено, ед.#]");
			template.AddRule("op2", "[Measures].[Досмотрено, ед.#]");
			Query.Templates.Add("skk_pattern_count_percent", template);

			// [Measures].[*ед., (ранг)]
			template = new QueryTemplate("skk_part_rank");
			template.AddRule("member", "[Measures].[*ед., (ранг)]");
			template.AddRule("value", "[Measures].[ед., %]");
			template.AddRule("set", "[Группы ]");
			Query.Templates.Add("skk_pattern_count_rank", template);

			// [Measures].[ПриостановленоСравн, ед%]
			template = new QueryTemplate("skk_part_percent");
			template.AddRule("member_base", "[Measures].[ПриостановленоСравн, ед%]");
			template.AddRule("member_plus", "[Measures].[ПриостановленоСравн, ед%доп]");
			template.AddRule("op1", "[Measures].[*сравн. Приостановлено, ед.#]");
			template.AddRule("op2", "[Measures].[ДосмотреноСравн, ед.#]");
			Query.Templates.Add("skk_pattern_count_percent_cmp", template);

			// [Measures].[*ед.Сравн, (ранг)]
			template = new QueryTemplate("skk_part_rank");
			template.AddRule("member", "[Measures].[*ед.Сравн, (ранг)]");
			template.AddRule("value", "[Measures].[ПриостановленоСравн, ед%]");
			template.AddRule("set", "[Группы ]");
			Query.Templates.Add("skk_pattern_count_rank_cmp", template);

			// [Measures].[рост/снижение, ед.]
			template = new QueryTemplate("skk_part_updown");
			template.AddRule("member_base", "[Measures].[рост/снижение, ед.]");
			template.AddRule("member_sign", "[Measures].[*рост/снижение, ед. (флаг)]");
			template.AddRule("member_unit", "[Measures].[*рост/снижение, ед. (СИ)]");
			template.AddRule("member_format", "[Measures].[*рост/снижение, ед. (формат)]");
			template.AddRule("op1", "[Measures].[Приостановлено, ед.#]");
			template.AddRule("op2", "[Measures].[*сравн. Приостановлено, ед.#]");
			Query.Templates.Add("skk_pattern_count_updown", template);
			

			// [Measures].[т., %]
			template = new QueryTemplate("skk_part_percent");
			template.AddRule("member_base", "[Measures].[т., %]");
			template.AddRule("member_plus", "[Measures].[*т., %доп]");
			template.AddRule("op1", "[Measures].[Приостановлено, т.]");
			template.AddRule("op2", "[Measures].[Досмотрено, т.]");
			Query.Templates.Add("skk_pattern_volume_percent", template);
			
			// [Measures].[*т., (ранг)]
			template = new QueryTemplate("skk_part_rank");
			template.AddRule("member", "[Measures].[*т., (ранг)]");
			template.AddRule("value", "[Measures].[т., %]");
			template.AddRule("set", "[Группы ]");
			Query.Templates.Add("skk_pattern_volume_rank", template);

			// [Measures].[ПриостановленоСравн, т%]
			template = new QueryTemplate("skk_part_percent");
			template.AddRule("member_base", "[Measures].[ПриостановленоСравн, т%]");
			template.AddRule("member_plus", "[Measures].[ПриостановленоСравн, т%доп]");
			template.AddRule("op1", "[Measures].[*сравн. Приостановлено, т.]");
			template.AddRule("op2", "[Measures].[ДосмотреноСравн, т.]");
			Query.Templates.Add("skk_pattern_volume_percent_cmp", template);

			// [Measures].[*т.Сравн, (ранг)]
			template = new QueryTemplate("skk_part_rank");
			template.AddRule("member", "[Measures].[*т.Сравн, (ранг)]");
			template.AddRule("value", "[Measures].[ПриостановленоСравн, т%]");
			template.AddRule("set", "[Группы ]");
			Query.Templates.Add("skk_pattern_volume_rank_cmp", template);
			
			// [Measures].[рост/снижение, т.]
			template = new QueryTemplate("skk_part_updown");
			template.AddRule("member_base", "[Measures].[рост/снижение, т.]");
			template.AddRule("member_sign", "[Measures].[*рост/снижение, т. (флаг)]");
			template.AddRule("member_unit", "[Measures].[*рост/снижение, т. (СИ)]");
			template.AddRule("member_format", "[Measures].[*рост/снижение, т. (формат)]");
			template.AddRule("op1", "[Measures].[Приостановлено, т.]");
			template.AddRule("op2", "[Measures].[*сравн. Приостановлено, т.]");
			Query.Templates.Add("skk_pattern_volume_updown", template);

			base.SetData();
		}
	}
}
