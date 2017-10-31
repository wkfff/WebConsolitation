using System.Data;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.SkkSupport
{

	public class GridCountTransport : GridCountBase
	{

		public GridCountTransport(UltraGridBrick grid)
			: base(grid)
		{
			LabelCommon = "Вид сообщения";
			LabelViewed = "Количество пунктов пропуска, в которых осуществлялся досмотр транспортных средств";
			LabelDeny = "Количество пунктов пропуска, в которых запрещался въезд/выезд транспортных средств";
		}

	}

	public class GridCountPeople : GridCountBase
	{

		public GridCountPeople(UltraGridBrick grid)
			: base(grid)
		{
			LabelCommon = "Вид сообщения";
			LabelViewed = "Количество пунктов пропуска, в которых осуществлялся досмотр лиц на наличие признаков инфекционных заболеваний";
			LabelDeny = "Количество пунктов пропуска, в которых были выявлены больные и/или лица с подозрением на инфекционные заболевания";
		}

	}

	public class GridCountGoods : GridCountBase
	{

		public GridCountGoods(UltraGridBrick grid)
			: base(grid)
		{
			LabelCommon = "Вид сообщения";
			LabelViewed = "Количество пунктов пропуска, в которых осуществлялся досмотр грузов";
			LabelDeny = "Количество пунктов пропуска, в которых запрещался ввоз/вывоз грузов";
		}

	}

	public abstract class GridCountBase : GridBase
	{
		protected GridCountBase(UltraGridBrick grid) 
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
			Band.Columns[0].Width = CRHelper.GetColumnWidth(120);
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			Band.HideColumns(1);
			Band.SetDefaultStyle(175, "N0", false);
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
		}

		/// <summary>
		/// заголовки таблицы
		/// </summary>
		public override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
			headerLayout.AddCell(LabelCommon);
			headerLayout.AddCell(LabelViewed);
			headerLayout.AddCell(LabelDeny);
			Grid.GridHeaderLayout.ApplyHeaderInfo();
		}

		/// <summary>
		/// установить данные
		/// </summary>
		/// <param name="queryName"></param>
		public override void SetData(string queryName)
		{
			base.SetData(queryName);

			DataTable dtGrid = Grid.DataTable;
			if (dtGrid.Rows.Count > 1)
			{
				if (dtGrid.Rows[0][0].ToString().ToLower().Contains("все"))
					dtGrid.Rows[0][0] = "ВСЕГО";

				if (dtGrid.Rows.Count == 2)
					dtGrid.Rows[0].Delete();
			}
		}
	}
}
