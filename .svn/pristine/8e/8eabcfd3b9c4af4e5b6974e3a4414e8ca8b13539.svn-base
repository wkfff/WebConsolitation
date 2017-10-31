using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_0001._0280
{
	public partial class Default : CustomReportPage
	{
		private CustomParam way_last_year { get { return (UserParams.CustomParam("way_last_year")); } }
		private CustomParam Pokaz { get { return (UserParams.CustomParam("Pokaz")); } }
		private CustomParam Lastdate { get { return (UserParams.CustomParam("lastdate")); } }
		private Int32 screen_width { get { return (int)Session["width_size"] - 50; } }
		private static double maxChartValue;
		private static double minChartValue;
		// параметр запроса для региона
		private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }


		private string _GetString_(string s)
		{
			string res = "";
			int i = 0;
			for (i = s.Length - 1; s[i] != ','; i--)
			{
				res = s[i] + res;
			};

			return res;
		}


		// Если по оси Y отсечки идут в миилионах, то заменить 6 нулей на млн.
		protected void FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
		{
			foreach (Primitive primitive in e.SceneGraph)
			{
				if (primitive is Text)
				{
					Text text = primitive as Text;
					string textValue = text.GetTextString();
					double numValue = 0;
					if (Double.TryParse(textValue, out numValue))
					{
						if (((numValue > 1000000) | (numValue == 0)) & (text.bounds.X < 50))
						{
							text.labelStyle.FontSizeBestFit = true;
							if (numValue != 0)
							{
								text.SetTextString(textValue.Substring(0, textValue.Length - 8) + " млн.");
							}
						}
					}
				}
			}
		}


		private void GridActiveRow(UltraWebGrid Grid, int index, bool active)
		{
			// получаем выбранную строку
			UltraGridRow row = Grid.Rows[index];
			// устанавливаем ее активной, если необходимо
			if (active)
			{
				row.Activate();
				row.Activated = true;
				row.Selected = true;
			}
		}


		private String ELV(String s)
		{
			int i = s.Length;
			string res = "";
			while (s[--i] != ']') ;
			while (s[--i] != '[')
			{
				res = s[i] + res;
			}
			return res;
		}


		public static void setChartErrorFont(Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
		{
			e.Text = "В настоящий момент данные отсутствуют";

			e.LabelStyle.FontColor = Color.LightGray;
			e.LabelStyle.VerticalAlign = StringAlignment.Center;
			e.LabelStyle.HorizontalAlign = StringAlignment.Center;
			e.LabelStyle.Font = new Font("Verdana", 30);
		}


		private void setFont(int typ, Label lab)
		{
			lab.Font.Name = "arial";
			lab.Font.Size = typ;
			if (typ == 14) { lab.Font.Bold = true; };
			if (typ == 10) { lab.Font.Bold = true; };
			if (typ == 18) { lab.Font.Bold = true; };
			if (typ == 16)
			{
				lab.Font.Bold = true;

				lab.Font.Size = FontUnit.Medium;
			};
		}


		public DataTable GetDSForChart(string sql)
		{
			DataTable dt = new DataTable();
			string s = DataProvider.GetQueryText(sql);
			DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(s, "Показатель", dt);
			return dt;
		}


		private String getLastDate(String way_ly)
		{
			way_last_year.Value = way_ly;
			CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("last_date"));
			return cs.Axes[1].Positions[0].Members[0].ToString();
		}


		private void conf_Grid(int sizePercent, UltraWebGrid grid, string cellFormat)
		{
			grid.Bands[0].Columns[0].Header.Style.Wrap = true;
			grid.Bands[0].Columns[0].CellStyle.Wrap = true;
			for (int i = 1; i < grid.Columns.Count; i++)
			{
				grid.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
				grid.Bands[0].Columns[i].CellStyle.Wrap = true;
				CRHelper.FormatNumberColumn(grid.Bands[0].Columns[i], cellFormat);
				grid.Bands[0].Columns[i].Header.Style.Wrap = true;
			}
			grid.DisplayLayout.CellClickActionDefault = CellClickAction.RowSelect;
			grid.DisplayLayout.HeaderTitleModeDefault = CellTitleMode.Always;
			grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;
			grid.DisplayLayout.GroupByBox.Hidden = true;
			grid.DisplayLayout.NoDataMessage = "Нет данных";
			grid.DisplayLayout.NoDataMessage = "Нет данных";
		}


		private void conf_Chart(int sizePercent, Infragistics.WebUI.UltraWebChart.UltraChart chart, bool leg, Infragistics.UltraChart.Shared.Styles.ChartType typ)
		{
			chart.Width = screen_width * sizePercent / 100;
			chart.SplineAreaChart.ChartText.Add(new Infragistics.UltraChart.Resources.Appearance.ChartTextAppearance(chart, -2, -2, true, new Font("arial", 8), Color.Black, "<DATA_VALUE:#>", StringAlignment.Far, StringAlignment.Center, 0));
			if (leg)
			{
				chart.Legend.Visible = true;
				chart.Legend.Location = Infragistics.UltraChart.Shared.Styles.LegendLocation.Bottom;
				chart.Legend.SpanPercentage = 30;

			}
		}


		private string GetString_(string s)
		{
			string res = "";
			int i = 0;
			for (i = s.Length - 1; s[i] != ','; i--) ;
			for (int j = 0; j < i; j++)
			{
				res += s[j];
			}
			return res;
		}


		string BN = "";
		protected override void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				System.Web.HttpBrowserCapabilities myBrowserCaps = Request.Browser;
				BN = ((System.Web.Configuration.HttpCapabilitiesBase)myBrowserCaps).Browser.ToUpper();
				// Коэффициенты для подгонки высоты гридов
				double kH = 1.0;
				if (BN == "IE")
				{
					kH = 0.99;
				}
				if (BN == "APPLEMAC-SAFARI")
				{
					kH = 1.0;
				}
				if (BN == "FIREFOX")
				{
					kH = 1.445;
				}
				RegionSettingsHelper.Instance.SetWorkingRegion(RegionSettings.Instance.Id);
				baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

				WebAsyncRefreshPanel1.AddLinkedRequestTrigger(GL);
				WebAsyncRefreshPanel3.AddLinkedRequestTrigger(GR);


				Lastdate.Value = ELV(getLastDate("ПРЕДПРИНИМАТЕЛИ БЕЗ ОБРАЗОВАНИЯ ЮРИДИЧЕСКОГО ЛИЦА"));
				GR.DataBind();
				GL.DataBind();
				Pokaz.Value = GL.Rows[0].Cells[0].Text;
				CL.DataBind();
				CC.DataBind();


				lbPageTitle.Text = String.Format("Деятельность предпринимателей без образования юридического лица ({0})",
					UserComboBox.getLastBlock(baseRegion.Value));
				Page.Title = lbPageTitle.Text;
				lbSubTitle.Text = "Анализ динамики и структуры основных показателей деятельности предпринимателей без образования юридического лица в муниципальном образовании";

				GridActiveRow(GR, 0, true);
				GridActiveRow(GL, 0, true);

				CellSet CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GLE"));

				for (int i = 0; i < CLS.Cells.Count; i++)
				{
					GL.Columns[i + 1].Header.Caption += ", " + CLS.Cells[i].Value.ToString().ToLower();

				}

				CLS = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText("GRE"));

				for (int i = 0; i < CLS.Cells.Count; i++)
				{
					GR.Rows[i].Cells[0].Text += ", " + CLS.Cells[i].Value.ToString().ToLower();

				}
				Pokaz.Value = GR.Rows[0].Cells[0].Text;
				CR.DataBind();
				GL.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.20 * kH);
				GR.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.27);
				GL.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.67);
				GR.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.32);

				CL.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33 - 5);
				CC.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33 - 5);
				CR.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33 - 5);
				CL.Height = CRHelper.GetChartHeight(325);
				CC.Height = CRHelper.GetChartHeight(325);
				CR.Height = CRHelper.GetChartHeight(325);
				CR.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
				CR.Axis.X.Extent = 20;
			}

		}


		protected void TopGrid_InitializeLayout(object sender, LayoutEventArgs e)
		{
			e.Layout.GroupByBox.Hidden = true;
			e.Layout.HeaderStyleDefault.Wrap = true;
			e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
			e.Layout.AllowSortingDefault = AllowSorting.No;
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

			conf_Grid(66, GL, "N2");
			double GW = screen_width / 100;
			double width_coef = 1.0;
			if (BN == "IE")
			{
			}
			if (BN == "FIREFOX")
			{
				width_coef = 0.9825;
			}
			if (BN == "APPLEMAC-SAFARI")
			{
			}
			e.Layout.Bands[0].Columns[0].Width = (int)(GW * 15);
			for (int i = 1; i < 6; ++i)
			{
				e.Layout.Bands[0].Columns[i].Width = (int)(GW * 10 * width_coef);
			}
			for (int i = 0; i < e.Layout.Bands[0].Columns.Count; ++i)
			{
				e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
			}
			e.Layout.Bands[0].Columns[0].Header.Caption = "Год";
			LTL.Width = Unit.Empty;
		}


		protected void TopGrid_ActiveRowChange(object sender, RowEventArgs e)
		{
			Pokaz.Value = e.Row.Cells[0].Text;
			CL.DataBind();
			CC.DataBind();
		}


		protected void GR_ActiveRowChange(object sender, RowEventArgs e)
		{
			Pokaz.Value = e.Row.Cells[0].Text;
			CR.DataBind();
		}


		protected void GL_DataBinding(object sender, EventArgs e)
		{
			LTR.Text = "Основные показатели деятельности ПБОЮЛ в " + Lastdate.Value + " году";
			GL.DataSource = GetDSForChart("gridleft");
		}


		protected void GR_DataBinding(object sender, EventArgs e)
		{
			LTL.Text = "Объем оборота торговли, работ и услуг ПБОЮЛ";
			GR.DataSource = GetDSForChart("gridright");
		}


		protected void GR_InitializeLayout(object sender, LayoutEventArgs e)
		{
			conf_Grid(33, GR, "N0");
			double GW = screen_width / 100;
			double Coef = 1;
			if (BN == "IE")
			{
				Coef = 0.99;
			}
			if (BN == "FIREFOX")
			{
				Coef = 0.9;
			}
			if (BN == "APPLEMAC-SAFARI")
			{
				Coef = 0.95;
			}
			e.Layout.Bands[0].Columns[0].Width = (int)(GW * 20 * Coef);
			e.Layout.Bands[0].Columns[1].Width = (int)(GW * 10 * Coef);
			e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
			e.Layout.AllowSortingDefault = AllowSorting.No;

			LTR.Width = Unit.Empty;
		}


		protected void CL_DataBinding(object sender, EventArgs e)
		{
			LBL.Text = "Структура оборота розничной торговли в " + Pokaz.Value + " году, %";
			CL.DataSource = GetDSForChart("chartleft");

			CL.Tooltips.FormatString = "<ITEM_LABEL>, рубль" + " " + "<b><DATA_VALUE:N0></b>";
			int size = 33;

			conf_Chart(size, CL, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart);
		}


		protected void CL_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
		{
			setChartErrorFont(e);
		}


		protected void CC_DataBinding(object sender, EventArgs e)
		{
			LBC.Text = "Структура оборота оптовой торговли в " + Pokaz.Value + " году, %";
			CC.DataSource = GetDSForChart("chartceter");

			CC.Tooltips.FormatString = "<ITEM_LABEL>, рубль" + " " + "<b><DATA_VALUE:N0></b>";
			int size = 33;

			conf_Chart(size, CC, true, Infragistics.UltraChart.Shared.Styles.ChartType.PieChart);
		}


		protected void CC_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
		{
			setChartErrorFont(e);
		}


		protected void CR_DataBinding(object sender, EventArgs e)
		{
			LBR.Text = "Динамика показателя " + '"' + GetString_(Pokaz.Value) + '"' + ", " + _GetString_(Pokaz.Value);
			CR.Tooltips.FormatString = "<b><DATA_VALUE:###,##0.##></b>, " + _GetString_(Pokaz.Value);
			Pokaz.Value = GetString_(Pokaz.Value);
			DataTable chart_table = GetDSForChart("chartright");
			CR.DataSource = chart_table;
			int size = 33;
			if (BN == "IE")
			{
				size = 33;
			}
			if (BN == "FIREFOX")
			{
				size = 34;
			}
			if (BN == "APPLEMAC-SAFARI")
			{
				size = 33;

			}
			CR.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
			LineAppearance lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
			CR.AreaChart.LineAppearances.Add(lineAppearance);
			// установка верхнего предела по оси Y диаграммы вручную
			GetExtimChartValues(chart_table, out minChartValue, out maxChartValue);
			CR.Axis.Y.RangeType = Infragistics.UltraChart.Shared.Styles.AxisRangeType.Custom;
			CR.Axis.Y.RangeMin = 0;
			CR.Axis.Y.RangeMax = maxChartValue * 1.1;
		}


		protected void GetExtimChartValues(DataTable chart_table, out double min, out double max)
		{
			ArrayList list = new ArrayList(chart_table.Rows[0].ItemArray);
			list.RemoveAt(0);
			list.Sort();
			max = Convert.ToDouble(list[list.Count - 1].ToString());
			min = Convert.ToDouble(list[0].ToString());
		}


		protected void CR_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
		{
			setChartErrorFont(e);
		}
	}
}
