using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.SkkSupport
{

	public class GridCmpTransport : GridCmpBase
	{
		public GridCmpTransport(UltraGridBrick grid)
			: base(grid)
		{
			LabelViewed = "Досмотрено ТС";
			LabelDeny = "Приостановлено ТС";
		}

	}

	public class GridCmpPeople : GridCmpBase
	{
		public GridCmpPeople(UltraGridBrick grid)
			: base(grid)
		{
			LabelViewed = "Досмотрено лиц";
			LabelDeny = "Выявлено больных, лиц с подозрением на<br />инфекционные заболевания";
		}

	}

	public abstract class GridCmpBase : GridBase
	{
		protected GridCmpBase(UltraGridBrick grid) 
			: base(grid)
		{
			// empty
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
			Band.HideColumns(6);
			Band.SetDefaultStyle(90, "N2", true);
		}

		/// <summary>
		/// настройка индикаторов
		/// </summary>
		public override void SetDataRules()
		{
			// правила для малых процентов
			Grid.AddIndicatorRule(new ReplaceValueColumnRule(3, "", "Выявлено%доп"));
			Grid.AddIndicatorRule(new ReplaceValueColumnRule(5, "", "ВыявленоСравн%доп"));

			// применить правило для 4 столбца
			GrowRateScaleRule ruleUpDown = new GrowRateScaleRule(6, "*рост/снижение (флаг)", "*рост/снижение (си)", "*рост/снижение (формат)");
			ruleUpDown.IndicatorUpDownInit(); 
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

			groupCell_1 = headerLayout.AddCell(LabelViewed);
			groupCell_1.AddCell("Текущий период").AddCell("абс");
			groupCell_1.AddCell("Период для сравнения").AddCell("абс");
			groupCell_1 = headerLayout.AddCell(LabelDeny);
			groupCell_2 = groupCell_1.AddCell("Текущий период");
			groupCell_2.AddCell("абс");
			groupCell_2.AddCell("%");
			groupCell_2 = groupCell_1.AddCell("Период для сравнения");
			groupCell_2.AddCell("абс");
			groupCell_2.AddCell("%");
			groupCell_1.AddCell("Рост/<br />снижение абс.<br />показателя", 2);

			headerLayout.AddNumericCells(1);

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

			// [Measures].[Выявлено%]
			template = new QueryTemplate("skk_part_percent");
			template.AddRule("member_base", "[Measures].[Выявлено%]");
			template.AddRule("member_plus", "[Measures].[Выявлено%доп]");
			template.AddRule("op1", "[Measures].[Выявлено#]");
			template.AddRule("op2", "[Measures].[Досмотрено#]");
			Query.Templates.Add("skk_pattern_percent", template);

			// [Measures].[ВыявленоСравн%]
			template = new QueryTemplate("skk_part_percent");
			template.AddRule("member_base", "[Measures].[ВыявленоСравн%]");
			template.AddRule("member_plus", "[Measures].[ВыявленоСравн%доп]");
			template.AddRule("op1", "[Measures].[ВыявленоСравн#]");
			template.AddRule("op2", "[Measures].[ДосмотреноСравн#]");
			Query.Templates.Add("skk_pattern_percent_cmp", template);

			// [Measures].[рост/снижение]
			template = new QueryTemplate("skk_part_updown");
			template.AddRule("member_base", "[Measures].[рост/снижение]");
			template.AddRule("member_sign", "[Measures].[*рост/снижение (флаг)]");
			template.AddRule("member_unit", "[Measures].[*рост/снижение (си)]");
			template.AddRule("member_format", "[Measures].[*рост/снижение (формат)]");
			template.AddRule("op1", "[Measures].[Выявлено#]");
			template.AddRule("op2", "[Measures].[ВыявленоСравн#]");
			Query.Templates.Add("skk_pattern_updown", template);
			
			base.SetData();
		}

	}

}
