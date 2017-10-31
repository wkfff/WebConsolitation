using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	// обертка над таблицей показателя (досмотрено/приостановлено)

	public class GridHelpMarkGoods : GridHelpMarkTransportBase
	{
		public GridHelpMarkGoods()
		{
			LabelViewed = "Общее число<br />досмотренных<br />партий грузов, ед.";
			LabelDeny = "Доля<br />досмотренных<br />партий грузов<br />1-11 групп грузов,<br />товаров раздела II<br />Единого перечня<br />товаров, %";
		}

		public override void SetData(string queryName)
		{
			DataTable dtGrid1 = new Query(queryName).GetDataTable();
			NumberRows(dtGrid1);

			dtGrid1.Columns[3].Caption = "%";

			// посчитать второй столбец как процент
			foreach (DataRow row in dtGrid1.Rows)
			{
				double all;
				double value;
				if (!Double.TryParse(row[2].ToString(), out all))
				{
					row[3] = String.Empty;
					continue;
				}
				if (!Double.TryParse(row[3].ToString(), out value))
				{
					continue;
				}
				if (value > 0)
				{
					row[3] = value/all;
				}
			}

			SplitData(dtGrid1);
		}
	}

	public class GridHelpMarkPeople : GridHelpMarkTransportBase
	{
		public GridHelpMarkPeople()
		{
			LabelViewed = "Число лиц,<br />досмотренных на<br />наличие признаков<br />инфекционных<br />заболеваний,<br />человек";
			LabelDeny = "Число выявленных<br />больных/<br />подозрительных на<br />инфекционные<br />заболевания,<br />человек";
		}
	}

	public class GridHelpMarkTransport : GridHelpMarkTransportBase
	{
		public GridHelpMarkTransport()
		{
			LabelViewed = "Общее число<br />досмотренных<br />транспортных<br />средств, ед.";
			LabelDeny = "Число ТС, пропуск<br />которых был<br />приостановлен<br />(временно<br />запрещен), ед.";
		}
	}

	public abstract class GridHelpMarkTransportBase : GridHelpBase
	{
		public GridHelpMark Grid1 { protected set; get; }
		public GridHelpMark Grid2 { protected set; get; }
		
		public string LabelViewed { set; get; }
		public string LabelDeny { set; get; }

		public override void Init(Page mainControl, string queryName)
		{
			page = mainControl;

			Grid1 = new GridHelpMark((UltraGridBrick) page.LoadControl("../../../Components/UltraGridBrick.ascx"));
			Grid2 = new GridHelpMark((UltraGridBrick) page.LoadControl("../../../Components/UltraGridBrick.ascx"));

			Grid1.LabelCommon = "Субъект Российской<br />Федерации";
			Grid2.LabelCommon = Grid1.LabelCommon;
			Grid1.LabelViewed = LabelViewed;
			Grid2.LabelViewed = LabelViewed;
			Grid1.LabelDeny = LabelDeny;
			Grid2.LabelDeny = LabelDeny;

			Grid1.SetStyle();
			Grid2.SetStyle();

			SetData(queryName);
		}

		public override HtmlGenericControl GetItem()
		{
			HtmlGenericControl item = new HtmlGenericControl("div");
			HtmlGenericControl subitem;

			if (ParamOneGrid)
			{
				// pdf export
				subitem = new HtmlGenericControl("div");
				subitem.Style.Add("float", "left");
				subitem.Controls.Add(Grid1.Grid);
				item.Controls.Add(subitem);

				subitem = new HtmlGenericControl("div");
				subitem.Style.Add("clear", "both");
				item.Controls.Add(subitem);
			}
			else
			{
				// html view

				subitem = new HtmlGenericControl("div");
				subitem.Style.Add("float", "left");
				subitem.Style.Add("margin-right", "5px");
				subitem.Controls.Add(Grid1.Grid);
				item.Controls.Add(subitem);

				subitem = new HtmlGenericControl("div");
				subitem.Style.Add("float", "left");
				subitem.Style.Add("margin-left", "5px");
				subitem.Controls.Add(Grid2.Grid);
				item.Controls.Add(subitem);

				subitem = new HtmlGenericControl("div");
				subitem.Style.Add("clear", "both");
				item.Controls.Add(subitem);
			}
			return item;
		}

		public virtual void SetData(string queryName)
		{
			DataTable dtGrid1 = new Query(queryName).GetDataTable();
			NumberRows(dtGrid1);

			if (ParamOneGrid)
			{
				// pdf export
				Grid1.Grid.DataTable = dtGrid1;
			}
			else 
			{
				// html view
				SplitData(dtGrid1);
			}
		}

		protected void NumberRows(DataTable dtGrid1)
		{
			// перенумеровать строки
			int index = 0;
			foreach (DataRow row in dtGrid1.Rows)
			{
				row[1] = row[0];
				row[0] = (index == 0) ? String.Empty : index.ToString();
				index++;
			}
		}

		protected void SplitData(DataTable dtGrid1)
		{
			// разделить на 2 таблицы
			DataTable dtGrid2 = dtGrid1.Clone();
			int middle = Convert.ToInt32(Math.Ceiling((double)dtGrid1.Rows.Count / 2));
			int count = dtGrid1.Rows.Count - middle;
			for (int i = 0; i < count; i++)
			{
				dtGrid2.ImportRow(dtGrid1.Rows[middle]);
				dtGrid1.Rows.RemoveAt(middle);
			}

			// привязать данные
			Grid1.Grid.DataTable = dtGrid1;
			Grid2.Grid.DataTable = dtGrid2;
		}

	}

	public class GridHelpMark : GridBase
	{
		public GridHelpMark(UltraGridBrick grid)
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
			Band.Columns[0].Width = 30;
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			Band.Columns[1].CellStyle.Wrap = true;
			Band.Columns[1].Width = 150;
			Band.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			Band.HideColumns(1);
			Band.SetDefaultStyle(120, "N0", 2);

			// т.к. содержащиеся символы % портят ширину в автонастройке
			Band.Columns[2].Width = 120;
			Band.Columns[3].Width = 120;
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
			headerLayout.AddCell("№<br />п/п");
			headerLayout.AddCell(LabelCommon);
			headerLayout.AddCell(LabelViewed);
			headerLayout.AddCell(LabelDeny);
			Grid.GridHeaderLayout.ApplyHeaderInfo();
		}
	}
}
