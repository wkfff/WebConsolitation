using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	public class GridUpDownTransport : GridUpDownBase
	{

		public GridUpDownTransport(UltraGridBrick grid) : base(grid)
		{
			HeaderHeight = 97;
		}

		public override void SetDataHeader()
		{
			LabelViewed = "Досмотрено ТС";
			LabelDeny = "Приостановлено ТС";
			base.SetDataHeader();
		}

	}

	public class GridUpDownPeople : GridUpDownBase
	{

		public GridUpDownPeople(UltraGridBrick grid) : base(grid)
		{
			HeaderHeight = 110;
		}

		public override void SetDataHeader()
		{
			LabelViewed = "Досмотрено лиц";
			LabelDeny = "Выявлено больных, лиц с подозрением на<br />инфекционные заболевания";
			base.SetDataHeader();
		}

	}

	public abstract class GridUpDownBase : GridBase
	{

		protected GridUpDownBase(UltraGridBrick grid) 
			: base(grid)
		{
			// empty
		}

		/// <summary>
		/// установить стиль внешнего вида элемента
		/// </summary>
		public override void SetStyle()
		{
			base.SetStyle();
			SetRowLimitHeight();
		}

		/// <summary>
		/// настройка внешнего вида данных
		/// </summary>
		public override void SetDataStyle()
		{
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = CRHelper.GetColumnWidth(200); 
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			Band.HideColumns(9);
			Band.SetDefaultStyle(85, "N2", false);
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

			// сдвиг текста согласно иерархии
			Grid.AddIndicatorRule(new PaddingRule(0, "УровеньИерархии", 10));

			// наибольший/наименьший показатель
			StarIndicatorRule ruleStar;

			ruleStar = new StarIndicatorRule(4, "Выявлено% (ранг)");
			ruleStar.IndicatorBestWorseInit();
			Grid.AddIndicatorRule(ruleStar);
			
			ruleStar = new StarIndicatorRule(6, "ВыявленоСравн% (ранг)");
			ruleStar.IndicatorBestWorseInit();
			Grid.AddIndicatorRule(ruleStar);
			
			// малые проценты
			Grid.AddIndicatorRule(new ReplaceValueColumnRule(4, "", "Выявлено%доп"));
			Grid.AddIndicatorRule(new ReplaceValueColumnRule(6, "", "ВыявленоСравн%доп"));

			// рост/снижение
			GrowRateScaleRule ruleUpDown = new GrowRateScaleRule(7, "*рост/снижение (флаг)", "*рост/снижение (си)", "*рост/снижение (формат)");
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

			headerLayout.AddCell(LabelCommon, 3);
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

			headerLayout.AddNumericCells(0);

			Grid.GridHeaderLayout.ApplyHeaderInfo();
		}

		/// <summary>
		/// установить данные
		/// </summary>
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

			// [Measures].[Выявлено% (ранг)]
			template = new QueryTemplate("skk_part_rank");
			template.AddRule("member", "[Measures].[Выявлено% (ранг)]");
			template.AddRule("value", "[Measures].[Выявлено%]");
			template.AddRule("set", "[Строки]");
			Query.Templates.Add("skk_pattern_rank", template);

			// [Measures].[ВыявленоСравн%]
			template = new QueryTemplate("skk_part_percent");
			template.AddRule("member_base", "[Measures].[ВыявленоСравн%]");
			template.AddRule("member_plus", "[Measures].[ВыявленоСравн%доп]");
			template.AddRule("op1", "[Measures].[ВыявленоСравн#]");
			template.AddRule("op2", "[Measures].[ДосмотреноСравн#]");
			Query.Templates.Add("skk_pattern_percent_cmp", template);

			// [Measures].[ВыявленоСравн% (ранг)]
			template = new QueryTemplate("skk_part_rank");
			template.AddRule("member", "[Measures].[ВыявленоСравн% (ранг)]");
			template.AddRule("value", "[Measures].[ВыявленоСравн%]");
			template.AddRule("set", "[Строки]");
			Query.Templates.Add("skk_pattern_rank_cmp", template);

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
