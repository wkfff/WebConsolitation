using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.SkkSupport
{
	public class GridBorder : GridBase
	{
		private Dictionary<GridBorderLabelParam, string> labelsParam = new Dictionary<GridBorderLabelParam, string>();
		private Dictionary<GridBorderLabelCount, string> labelsCount = new Dictionary<GridBorderLabelCount, string>();

		private string labelParam;
		private string labelBorder;
		private string labelCount;

		public GridBorder(UltraGridBrick grid) 
			: base(grid)
		{
			labelBorder = "<br />Участок границы<br /><br />";
			labelParam = String.Empty;
			labelCount = String.Empty;
			DeleteColumns = 1;

			// заполнить словарики
			labelsParam.Add(GridBorderLabelParam.Terra, "Территория");
			labelsParam.Add(GridBorderLabelParam.Transport, "Вид сообщения");
			labelsCount.Add(GridBorderLabelCount.Goods, "Досмотрено партий грузов");
			labelsCount.Add(GridBorderLabelCount.People, "Досмотрено лиц");
			labelsCount.Add(GridBorderLabelCount.Transport, "Досмотрено ТС");
		}

		public void SetLabels(GridBorderLabelParam param, GridBorderLabelCount count)
		{
			labelParam = labelsParam[param];
			labelCount = labelsCount[count];
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
			Band.SetDefaultStyle(175, "N0", true);
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
		}

		/// <summary>
		/// настройка индикаторов
		/// </summary>
		public override void SetDataRules()
		{
			// empty
		}

		/// <summary>
		/// заголовки таблицы
		/// </summary>
		public override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
			headerLayout.AddCell(labelParam);
			headerLayout.AddCell(labelBorder);
			headerLayout.AddCell(labelCount);
			Grid.GridHeaderLayout.ApplyHeaderInfo();
		}
		
	}

	public enum GridBorderLabelParam
	{
		Terra, Transport
	}

	public enum GridBorderLabelCount
	{
		Goods, People, Transport
	}

}
