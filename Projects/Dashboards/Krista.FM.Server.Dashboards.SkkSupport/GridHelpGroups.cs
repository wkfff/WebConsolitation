using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	// обертка над таблицей показателя (досмотрено/приостановлено)

	public abstract class GridHelpGroupsCommon : GridHelpBase
	{
		protected SKKHelper.DirectionType Direction { set; get; }

		public override void Init(Page mainControl, string queryName)
		{
			page = mainControl;
			Grid = new GridHelpGroups((UltraGridBrick)page.LoadControl("../../../Components/UltraGridBrick.ascx"));

			(Grid as GridHelpGroups).Direction = Direction;
			Grid.SetStyle();
			Grid.DeleteColumns = 1;
			Grid.SetData(queryName);
		}
	}

	public class GridHelpGroupsInCommon : GridHelpGroupsCommon
	{
		public GridHelpGroupsInCommon()
		{
			Direction = SKKHelper.DirectionType.In;
		}
	}

	public class GridHelpGroupsOutCommon : GridHelpGroupsCommon
	{
		public GridHelpGroupsOutCommon()
		{
			Direction = SKKHelper.DirectionType.Out;
		}
	}

	public class GridHelpGroups : GridBase
	{
		public SKKHelper.DirectionType Direction { set; get; }

		public GridHelpGroups(UltraGridBrick grid)
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
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = 205; 
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			Band.HideColumns(3);
			Band.SetDefaultStyle(80, "N0", 1);
			
			// тонны
			CRHelper.FormatNumberColumn(Band.Columns[2], "N3");
			CRHelper.FormatNumberColumn(Band.Columns[5], "N3");
			Band.Columns[2].Width = 95;
			Band.Columns[5].Width = 95;
		}

		/// <summary>
		/// настройка индикаторов
		/// </summary>
		public override void SetDataRules()
		{
			// правило жирности
			FontRowLevelRule levelRule = new FontRowLevelRule(Band.Columns.Count - 1);
			levelRule.AddFontLevel("0", Grid.BoldFont8pt);
			Grid.AddIndicatorRule(levelRule);
			
			// наибольший/наименьший показатель
			StarIndicatorRule ruleStar;
			ruleStar = new StarIndicatorRule(4, "Выявлено%ранг");
			ruleStar.IndicatorBestWorseInit();
			Grid.AddIndicatorRule(ruleStar);

			ruleStar = new StarIndicatorRule(6, "Выявлено%ранг2");
			ruleStar.IndicatorBestWorseInit();
			Grid.AddIndicatorRule(ruleStar);
		}

		/// <summary>
		/// заголовки таблицы
		/// </summary>
		public override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
			GridHeaderCell groupCell_1;
			GridHeaderCell groupCell_2;

			headerLayout.AddCell("Группы товаров<br />Раздела II<br />Единого перечня товаров");
			switch (Direction)
			{
				case SKKHelper.DirectionType.In:
					groupCell_1 = headerLayout.AddCell("Досмотрено<br />при ввозе в РФ");
					break;
				case SKKHelper.DirectionType.Out:
					groupCell_1 = headerLayout.AddCell("Досмотрено<br />при вывозе из РФ");
					break;
				default:
					groupCell_1 = headerLayout.AddCell(String.Empty);
					break;
			}
			groupCell_1.AddCell("Число партий").AddCell("единиц");
			groupCell_1.AddCell("Объем").AddCell("тонн");
			switch (Direction)
			{
				case SKKHelper.DirectionType.In:
					groupCell_1 = headerLayout.AddCell("Запрещен ввоз грузов и товаров");
					break;
				case SKKHelper.DirectionType.Out:
					groupCell_1 = headerLayout.AddCell("Запрещен вывоз грузов и товаров");
					break;
				default:
					groupCell_1 = headerLayout.AddCell(String.Empty);
					break;
			}
			groupCell_2 = groupCell_1.AddCell("Число партий");
			groupCell_2.AddCell("единиц");
			groupCell_2.AddCell("%");
			groupCell_2 = groupCell_1.AddCell("Объем");
			groupCell_2.AddCell("тонн");
			groupCell_2.AddCell("%");
			Grid.GridHeaderLayout.ApplyHeaderInfo();
		}

		public override void SetData(string queryName)
		{
			Query = new Query(queryName);
			QueryTemplate template;
			
			// [Measures].[Выявлено%ранг]
			template = new QueryTemplate("skk_part_rank");
			template.AddRule("member", "[Measures].[Выявлено%ранг]");
			template.AddRule("value", "[Measures].[Выявлено%]");
			template.AddRule("set", "[Группы]");
			Query.Templates.Add("skk_pattern_rank", template);

			// [Measures].[Выявлено%ранг2]
			template = new QueryTemplate("skk_part_rank");
			template.AddRule("member", "[Measures].[Выявлено%ранг2]");
			template.AddRule("value", "[Measures].[Выявлено2%]");
			template.AddRule("set", "[Группы]");
			Query.Templates.Add("skk_pattern_rank2", template);

			base.SetData();
			DataTable table = Grid.DataTable;
			
			foreach (DataRow row in table.Rows)
			{
				int braceIndex = row[0].ToString().IndexOf("(");
				if (braceIndex >= 0)
				{
					row[0] = String.Format(
						"<b>{0}</b> {1}",
						row[0].ToString().Substring(0, braceIndex-1),
						row[0].ToString().Substring(braceIndex));
				}
			}

		}
		
	}
}
