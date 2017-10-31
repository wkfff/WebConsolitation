using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;

namespace Krista.FM.Server.Dashboards.MinSportSupport
{
	public class GridFull : GridCommon
	{
		private const int markColumnsCount = 4;
		private const int markHiddenColumnsCount = 5;
		
		private Dictionary<string, object[]> marks = new Dictionary<string, object[]>();
		// 0 - uid для мемберов в mdx
		// 1 - показатель "досмотрено"
		// 2 - показатель "выявлено"
		// 3 - стартовый номер колонки в гриде

		private Collection<LocalRuleBase> rules = new Collection<LocalRuleBase>();

		// гарантирует полное отображение по горизонтали (когда нельзя доверять кукисам)
		public bool SafeMode { set; get; }

		public GridFull(UltraGridBrick grid) 
			: base(grid)
		{
			HeaderHeight = 167;	
			
		}

		/// <summary>
		/// установить стиль внешнего вида элемента
		/// </summary>
		public override void SetStyle()
		{
			base.SetStyle();
			SetMaxWidth(SafeMode);
			SetHeight(SportHelper.defaultHeightFull);
		}

		/// <summary>
		/// настройка внешнего вида данных
		/// </summary>
		public override void SetDataStyle()
		{
			Band.Columns[0].CellStyle.Wrap = true;
			Band.Columns[0].Width = CRHelper.GetColumnWidth(200); 
			Band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

			/*Band.HideColumns(2 + markHiddenColumnsCount * marks.Count);
			Band.SetDefaultStyle(90, "N3", false);

			Band.Columns[1].SetColumnWidth(1.1);
			Band.Columns[(int)marks["goods"].GetValue(3) + 2].SetColumnWidth(1.3); // "+1" as balance to magic

			// объем грузов
			CRHelper.FormatNumberColumn(Band.Columns[(int)marks["gvolume"].GetValue(3) + 0], "N3");
			CRHelper.FormatNumberColumn(Band.Columns[(int)marks["gvolume"].GetValue(3) + 1], "N3");
			Band.Columns[(int)marks["gvolume"].GetValue(3) + 0].SetColumnWidth(1.2);
			Band.Columns[(int)marks["gvolume"].GetValue(3) + 2].SetColumnWidth(1.2);*/
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

			// сдвиг текста согласно иерархии
			Grid.AddIndicatorRule(new PaddingRule(0, "УровеньИерархии", 10));

			/*
			foreach (KeyValuePair<string, object[]> mark in marks)
			{
				// наибольший/наименьший показатель
				StarIndicatorRule ruleStar = new StarIndicatorRule(2 + (int)mark.Value[3], String.Format("Выявлено_{0}_%ранг", mark.Value[0]));
				ruleStar.IndicatorBestWorseInit();
				Grid.AddIndicatorRule(ruleStar);

				// малые проценты
				Grid.AddIndicatorRule(new ReplaceValueColumnRule(2 + (int)mark.Value[3], "", String.Format("Выявлено_{0}_%доп", mark.Value[0])));
				
				// рост/снижение
				GrowRateScaleRule ruleUpDown = 
					new GrowRateScaleRule(
						3 + (int)mark.Value[3],
						String.Format("рост/снижение_{0}_флаг", mark.Value[0]),
						String.Format("рост/снижение_{0}_си", mark.Value[0]),
						String.Format("рост/снижение_{0}_формат", mark.Value[0]));
				if (mark.Key.ToLower().Contains("gvolume"))
				{
					ruleUpDown.IndicatorUpDownInit("т", "N3");
				}
				else
				{
					ruleUpDown.IndicatorUpDownInit();
				}
				Grid.AddIndicatorRule(ruleUpDown);
			}*/

		}

		/// <summary>
		/// заголовки таблицы
		/// </summary>
		public override void SetDataHeader()
		{
			GridHeaderLayout headerLayout = Grid.GridHeaderLayout;
			GridHeaderCell groupCell_1;
			/*GridHeaderCell groupCell_2;
			GridHeaderCell groupCell_3;
			GridHeaderCell groupCell_4;*/
			
			headerLayout.AddCell("Территория", 2);

			groupCell_1 = headerLayout.AddCell("Спортивные сооружения");
			groupCell_1.AddCell("Всего", 1);
			groupCell_1.AddCell("Стадионы", 1);
			groupCell_1.AddCell("Плоскостные спортсооружения", 1);
			groupCell_1.AddCell("Залы", 1);
            groupCell_1.AddCell("Бассейны", 1);
            headerLayout.AddCell("Кадры", 2);
            headerLayout.AddCell("Численность занимающихся", 2);
            headerLayout.AddCell("из них женщины", 2);
            headerLayout.AddCell("Численность занимающихся в клубах, в том числе по месту жительства", 2);
            headerLayout.AddCell("Количество учащихся отнесенных к спецмедгруппе", 2);

			/*groupCell_1 = headerLayout.AddCell("Досмотр лиц на наличие признаков инфекционных заболеваний");
			groupCell_1.AddCell("Досмотрено", 4);
			groupCell_2 = groupCell_1.AddCell("Выявлено");
			groupCell_2.AddCell("абс", 3);
			groupCell_2.AddCell("%", 3);
			groupCell_2.AddCell("Рост/<br />снижение абс.<br />показателя", 3);

			groupCell_1 = headerLayout.AddCell("Досмотр грузов");
			groupCell_1.AddCell("Досмотрено партий грузов – всего, абс.", 4);
			groupCell_1.AddCell("Доля досмотренных партий грузов, товаров 1-11 групп раздела II Единого перечня товаров, %", 4);
			groupCell_1.AddCell("Рост/<br />снижение показателя", 4);

			groupCell_2 = groupCell_1.AddCell("Досмотр подконтрольных грузов, товаров 1-11 групп раздела II Единого перечня товаров");

			groupCell_3 = groupCell_2.AddCell("Число партий грузов");
			groupCell_3.AddCell("Досмотрено", 2);
			groupCell_4 = groupCell_3.AddCell("Приостановлено");
			groupCell_4.AddCell("абс");
			groupCell_4.AddCell("%");
			groupCell_4.AddCell("Рост/<br />снижение абс.<br />показателя");

			groupCell_3 = groupCell_2.AddCell("Объем партий грузов");
			groupCell_3.AddCell("Досмотрено,<br />т", 2); 
			groupCell_4 = groupCell_3.AddCell("Приостановлено");
			groupCell_4.AddCell("абс");
			groupCell_4.AddCell("%");
			groupCell_4.AddCell("Рост/<br />снижение абс.<br />показателя");*/

			headerLayout.AddNumericCells(0);
			
			Grid.GridHeaderLayout.ApplyHeaderInfo();
		}

		/// <summary>
		/// установить данные
		/// </summary>
		public override void SetData(string queryName)
		{
			Query = new QueryWorker(queryName);
			//QueryTemplate template;

			/*foreach (KeyValuePair<string, object[]> mark in marks)
			{
				// наибольший/наименьший показатель
				// правило заполнения таблички
				rules.Add(new LocalRuleMinMax(2 + (int)mark.Value[3], String.Format("Выявлено_{0}_%ранг", mark.Value[0])));

				// Выявлено_{0}_#, Досмотрено_{0}_#, Выявлено_{0}_Сравн#, Досмотрено_{0}_Сравн#
				template = new QueryTemplate("skk_part_base_members");
				template.AddRule("type", mark.Value[0].ToString());
				template.AddRule("mark_view", mark.Value[1].ToString());
				template.AddRule("mark_deny", mark.Value[2].ToString());
				Query.Templates.Add(String.Format("skk_pattern_{0}_members", mark.Key), template);
				
				// [Measures].[Выявлено%]
				template = new QueryTemplate("skk_part_percent");
				template.AddRule("member_base", String.Format("[Measures].[Выявлено_{0}_%]", mark.Value[0]));
				template.AddRule("member_plus", String.Format("[Measures].[Выявлено_{0}_%доп]", mark.Value[0]));
				template.AddRule("op1", String.Format("[Measures].[Выявлено_{0}_#]", mark.Value[0]));
				template.AddRule("op2", String.Format("[Measures].[Досмотрено_{0}_#]", mark.Value[0]));
				Query.Templates.Add(String.Format("skk_pattern_{0}_percent", mark.Key), template);

				// [Measures].[Выявлено% (ранг)]
				template = new QueryTemplate("skk_part_rank_empty");
				template.AddRule("member", String.Format("[Measures].[Выявлено_{0}_%ранг]", mark.Value[0]));
				Query.Templates.Add(String.Format("skk_pattern_{0}_rank", mark.Key), template);
				
				// [Measures].[рост/снижение]
				template = new QueryTemplate("skk_part_updown");
				template.AddRule("member_base", String.Format("[Measures].[рост/снижение_{0}]", mark.Value[0]));
				template.AddRule("member_sign", String.Format("[Measures].[рост/снижение_{0}_флаг]", mark.Value[0]));
				template.AddRule("member_unit", String.Format("[Measures].[рост/снижение_{0}_си]", mark.Value[0]));
				template.AddRule("member_format", String.Format("[Measures].[рост/снижение_{0}_формат]", mark.Value[0]));
				template.AddRule("op1", String.Format("[Measures].[Выявлено_{0}_#]", mark.Value[0]));
				template.AddRule("op2", String.Format("[Measures].[Выявлено_{0}_Сравн#]", mark.Value[0]));
				Query.Templates.Add(String.Format("skk_pattern_{0}_updown", mark.Key), template);

				// select
				template = new QueryTemplate("skk_part_tables_select_members");
				template.AddRule("type", mark.Value[0].ToString());
				Query.Templates.Add(String.Format("skk_pattern_{0}_select", mark.Key), template);
				
				// select hidden
				template = new QueryTemplate("skk_part_tables_select_hidden_members");
				template.AddRule("type", mark.Value[0].ToString());
				Query.Templates.Add(String.Format("skk_pattern_{0}_select_hidden", mark.Key), template);
			}*/

			base.SetData();
			
			//ApplyLocalRules();
			
		}

		private void ApplyLocalRules()
		{
			foreach (LocalRuleBase rule in rules)
			{
				rule.Start(Grid.DataTable);
			}

			foreach (DataRow row in Grid.DataTable.Rows)
			{
				foreach (LocalRuleBase rule in rules)
				{
					rule.CheckValue(row, Grid.DataTable.Rows.IndexOf(row));
				}
			}

			foreach (LocalRuleBase rule in rules)
			{
				rule.Finish(Grid.DataTable);
			}
		}

	}

	public abstract class LocalRuleBase
	{
		protected int column = -1;

		protected LocalRuleBase(int column)
		{
			this.column = column;
		}

		public abstract void CheckValue(DataRow row, int index);
		public abstract void Start(DataTable data);
		public abstract void Finish(DataTable data);
	}

	public class LocalRuleMinMax : LocalRuleBase
	{
		private int columnRank = -1;
		private string nameRank = String.Empty;

		private double max = Double.MinValue; 
		private double min = Double.MaxValue;

		public LocalRuleMinMax(int column, string nameRank) : base(column)
		{
			this.nameRank = nameRank;
		}
		
		public override void Start(DataTable data)
		{
			// находим номера нужных колонок
			int i = 0;
			while ((i < data.Rows[0].ItemArray.Length) && (columnRank == -1))
			{
				string columnCaption = data.Columns[i].Caption;
				if (columnCaption.Contains(nameRank))
				{
					columnRank = i;
					break;
				}
				i++;
			}
		}

		public override void CheckValue(DataRow row, int index)
		{
			if (column == -1 || columnRank == -1)
			{
				return;
			}

			double value;
			if (Double.TryParse(row[column].ToString(), out value))
			{
				value = Math.Round(value, 4);
				if (value > max)
				{
					max = value;
				}
				else if (value < min)
				{
					min = value;
				}
			}
		}

		public override void Finish(DataTable data)
		{
			if ((min == Double.MaxValue) && (max == Double.MinValue))
				return;
			if (min == max)
				return; 
			if (min == 0)
				min = Double.MaxValue;
			
			foreach (DataRow row in data.Rows)
			{
				double value;
				if (Double.TryParse(row[column].ToString(), out value))
				{
					value = Math.Round(value, 4);
					if (value == max)
					{
						row[columnRank] = 1;
					}
					if (value == min)
					{
						row[columnRank] = -1;
					}
				}
			}
		}
	}

}
