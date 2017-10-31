using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	/// <summary>
	/// Класс-помощник для таблиц с показателями
	/// </summary>
	public class GridMark : GridBase
	{
		public string Mark { set; get; }
		public string MarkFormat { set; get; }
		public GridMark(UltraGridBrick grid, string mark) : base(grid)
		{
			MarkFormat = "N0";
			Mark = SKKHelper.WrapString(mark, 18, "<br />");
			HeaderHeight = 8 + 13*SKKHelper.CountStrings(Mark, "<br />");
		}
		
		public override void SetStyle()
		{
			base.SetStyle();
			SetRowLimitHeight();
		}

		public override void SetDataStyle()
		{
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = CRHelper.GetColumnWidth(200);
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			Band.HideColumns(1);
			Band.SetDefaultStyle(125, MarkFormat, false);
		}

		public override void SetDataRules()
		{
			// выделение жирновстью
			FontRowLevelRule levelRule = new FontRowLevelRule(Band.Columns.Count - 1);
			levelRule.AddFontLevel("0", Grid.BoldFont8pt);
			Grid.AddIndicatorRule(levelRule);

			// сдвиг текста согласно иерархии
			Grid.AddIndicatorRule(new PaddingRule(0, "Уровень", 10));
		}

		public override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
			headerLayout.AddCell(LabelCommon);
			headerLayout.AddCell(Mark);
			Grid.GridHeaderLayout.ApplyHeaderInfo();
		}
	}
}