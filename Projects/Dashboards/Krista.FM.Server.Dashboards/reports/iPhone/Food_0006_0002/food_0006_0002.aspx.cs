using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections;
using Infragistics.UltraChart.Core;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Food_0006_0002 : CustomReportPage
    {

		private string shortFoodName;
		private DateTime lastDate;
		private DateTime currDate;
        private DateTime yearDate;
		private static object currCostHMAO;
        private static object lastCostHMAO;
        private static object yearCostHMAO;
        private string unit;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
			
			string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/Food_0006_0002/") + "TouchElementBounds.xml";
			Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/Food_0006_0002/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"Food_0006_0003_Food={0}\" bounds=\"x=0;y=400;width=768;height=100\" openMode=\"incomes\"/><element id=\"Food_0006_0004_Food={0}\" bounds=\"x=0;y=500;width=768;height=230\" openMode=\"outcomes\"/></touchElements>", HttpContext.Current.Session["CurrentFoodID"]));

            string multitouchIcon = String.Empty;
            multitouchIcon = "<img src='../../../images/detail.png'>";
            detalizationIconDiv.InnerHtml = String.Format("<table><tr><td><a href='webcommand?showPinchReport=Food_0006_0003_Food={1}'>{0}</a></td>", multitouchIcon, HttpContext.Current.Session["CurrentFoodID"]);
            detalizationIconDiv.InnerHtml += String.Format("<td><a href='webcommand?showPinchReport=Food_0006_0004_Food={1}'><img src='../../../images/lock.png'></a></td></tr></table>", multitouchIcon, HttpContext.Current.Session["CurrentFoodID"]);

            shortFoodName = CRHelper.GetLastBlock(UserParams.Food.Value);

			InitializeDate();
			
			Label1.Text = MakeText();

            Image1.ImageUrl = String.Format("../../../images/Продукты iPad/{0}.png", HttpContext.Current.Session["CurrentFoodID"]);

			#region Диаграмма 1

			UltraChart1.Width = 760;
			UltraChart1.Height = 300;
			UltraChart1.ChartType = ChartType.AreaChart;

			UltraChart1_AddApearences();

			UltraChart1.AreaChart.NullHandling = NullHandling.DontPlot;
			UltraChart1.Axis.X.Extent = 60;
			UltraChart1.Axis.Y.Extent = 25;

			UltraChart1.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
			UltraChart1.Axis.X.Margin.Near.Value = 3;
			UltraChart1.Axis.X.Margin.Far.Value = 2;
			UltraChart1.Axis.Y.Margin.Near.Value = 7;
			UltraChart1.Axis.Y.Margin.Far.Value = 3;

			UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><SERIES_LABEL>г.\n<ITEM_LABEL>\n<b><DATA_VALUE:N2></b> руб.</span>";

			UltraChart1.Axis.Y.Labels.Visible = true;
			UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.Visible = true;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = String.Format("руб. за {0}", unit);
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.TitleLeft.FontColor = Color.White;
            UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = 125;

			UltraChart1.Legend.Visible = false;

			UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
			UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
			UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

			UltraChart1.DataBind();

			#endregion

			#region Диаграмма 2

			UltraChart2.Width = 760;
			UltraChart2.Height = 450;

            UltraChart2.ChartType = ChartType.ColumnChart;
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomLinear;
			UltraChart2.Border.Thickness = 0;

			UltraChart2.Axis.X.Extent = 175;
			UltraChart2.Axis.X.Labels.Visible = true;
			UltraChart2.Axis.X.Labels.SeriesLabels.Visible = false;

            UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 10);
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart2.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart2.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart2.Axis.X.MinorGridLines.Color = Color.Black;

            UltraChart2.Axis.X.Labels.WrapText = true;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart2.Axis.X.Labels.VerticalAlign = StringAlignment.Center;
			
            UltraChart2.Axis.Y.Extent = 25;

			UltraChart2.Axis.Y.Labels.Font = new Font("Verdana", 10);
			UltraChart2.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
			UltraChart2.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
			UltraChart2.Axis.Y.MinorGridLines.Color = Color.Black;
			UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart2.Axis.Y.RangeType = AxisRangeType.Custom;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Text = String.Format("руб. за {0}", unit);
            UltraChart2.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart2.TitleLeft.FontColor = Color.White;
            UltraChart2.TitleLeft.VerticalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Margins.Bottom = 250;
            
            UltraChart2.Legend.Visible = false;

			UltraChart2.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL>\n<b><DATA_VALUE:N2></b> руб.</span>";
			UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
			UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
			UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);

			UltraChart2.DataBind();

			#endregion

            UltraChart1.Style.Add("margin-top", "-10px");
            UltraChart2.Style.Add("margin-top", "-10px");

		}

		#region Инициализация

        private void InitializeDate()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0006_0002_incomes_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            UserParams.PeriodFirstYear.Value = dtDate.Rows[0]["ДанныеНа"].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[1]["ДанныеНа"].ToString();
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[2]["ДанныеНа"].ToString();
            unit = dtDate.Rows[3]["ДанныеНа"].ToString();

            currDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodCurrentDate.Value, 3);
            lastDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodLastDate.Value, 3);
            yearDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodFirstYear.Value, 3);
        }

		#endregion

		#region Текст

		private string MakeText()
		{
			string query = DataProvider.GetQueryText("Food_0006_0002_text");
			DataTable dtText = new DataTable();
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Текст", dtText);
			if (dtText.Rows.Count == 0)
                return String.Format("на&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy} г.</span>&nbsp;данные о цене на продукт питания «{0}» отсутствуют.", shortFoodName, currDate);
			DataRow row = dtText.Rows[0];
			currCostHMAO = row["Цена на текущую дату"];
			lastCostHMAO = row["Цена на прошлую дату"];
            yearCostHMAO = row["Цена на начало года"];
            object delta = row["Изменение цены"];
            object grown = row["Темп прироста"];
            object deltaYear = row["Изменение цены к началу года"];
            object grownYear = row["Темп прироста к началу года"];
            string result = String.Format(
                "На&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy} г.</span>&nbsp;средняя розничная цена по ЯНАО на продукт питания «{0}» за {3}:&nbsp;<span class='DigitsValueXLarge'>{2:N2}</span>&nbsp;руб.",
				shortFoodName, currDate, currCostHMAO, unit);
            result += "<br/><table>";
            if (lastCostHMAO != DBNull.Value)
			{
                string img = String.Empty;
				if (Convert.ToDouble(delta) > 0)
				{
                    img = "рост&nbsp;&nbsp;&nbsp;<img src='../../../images/ArrowRedUpIPad.png'/>&nbsp;&nbsp;&nbsp;с";
				}
				else if (Convert.ToDouble(delta) < 0)
				{
                    img = "снижение&nbsp;<img src='../../../images/ArrowGreenDownIPad.png'/>&nbsp;с";
				}
                if (Convert.ToDouble(delta) != 0)
                    result += String.Format("<tr valign=\"bottom\"><td width=\"75px\"></td><td>{1}</td><td>&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy} г.</span>&nbsp;</td><td align=\"right\">&nbsp;<span class='DigitsValue'>{2}</span>&nbsp;руб.</td><td align=\"right\">&nbsp;<span class='DigitsValue'>{3}</span></td></tr>", lastDate, img, CRHelper.CurrencyToStringWithSign(delta), CRHelper.PercentToStringWithSign(grown));
                else
                    result += String.Format("<tr valign=\"bottom\"><td width=\"75px\"></td><td>изменений&nbsp;с</td><td>&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy} г.</span>&nbsp;</td><td colspan=\"2\">не&nbsp;было.</td><td></td></tr>", lastDate);
            }
            if (yearCostHMAO != DBNull.Value)
            {
                string img = String.Empty;
                if (Convert.ToDouble(deltaYear) > 0)
                {
                    img = "рост&nbsp;&nbsp;&nbsp;<img src='../../../images/ArrowRedUpIPad.png'/>&nbsp;&nbsp;&nbsp;с";
                }
                else if (Convert.ToDouble(deltaYear) < 0)
                {
                    img = "снижение&nbsp;<img src='../../../images/ArrowGreenDownIPad.png'/>&nbsp;с";
                }
                if (Convert.ToDouble(deltaYear) != 0)
                    result += String.Format("<tr valign=\"bottom\"><td></td><td>{1}</td><td>&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy} г.</span>&nbsp;</td><td align=\"right\">&nbsp;<span class='DigitsValue'>{2}</span>&nbsp;руб.</td><td align=\"right\">&nbsp;<span class='DigitsValue'>{3}</span></td></tr>", yearDate, img, CRHelper.CurrencyToStringWithSign(deltaYear), CRHelper.PercentToStringWithSign(grownYear));
                else
                    result += String.Format("<tr valign=\"bottom\"><td></td><td>изменений&nbsp;с</td><td>&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy} г.</span>&nbsp;</td><td colspan=\"2\">не&nbsp;было.</td><td></td></tr>", yearDate);
            }
            result += "</table>";

			return result;
		}

		#endregion

		#region Диграмма 1

		private void UltraChart1_DataBinding(object sender, EventArgs e)
		{
			DataTable dtChart = new DataTable();
			string query = DataProvider.GetQueryText("Food_0006_0002_chart1");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtChart);

			foreach (DataRow row in dtChart.Rows)
			{
				row["Дата"] = String.Format("{0:dd.MM.yy}", CRHelper.DateByPeriodMemberUName(row["Дата"].ToString(), 3));
			}

			dtChart.Columns.RemoveAt(0);

			UltraChart1.Data.SwapRowsAndColumns = true;

			UltraChart1.DataSource = dtChart;
		}

		private void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is PointSet)
				{
					PointSet pointSet = (PointSet)primitive;
					foreach (DataPoint point in pointSet.points)
					{
						point.hitTestRadius = 20;
						if (point.DataPoint == null)
						{
							point.Visible = false;
						}
					}
				}
			}
		}

		private void UltraChart1_AddApearences()
		{
			UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
			UltraChart1.ColorModel.Skin.PEs.Clear();
			PaintElement pe = new PaintElement();
			pe.Fill = Color.Indigo;
			pe.FillStopColor = Color.Indigo;
			pe.StrokeWidth = 0;
			pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
			pe.FillOpacity = 255;
			pe.FillStopOpacity = 150;
			UltraChart1.ColorModel.Skin.PEs.Add(pe);
			LineAppearance lineAppearance = new LineAppearance();
			lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
			lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
			lineAppearance.IconAppearance.PE = pe;
			UltraChart1.AreaChart.LineAppearances.Add(lineAppearance);
		}

		#endregion

		#region Диаграмма 2

		protected void UltraChart2_DataBinding(object sender, EventArgs e)
		{
			DataTable dtChart = new DataTable();
			string query = DataProvider.GetQueryText("Food_0006_0002_chart2");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "МО", dtChart);

			foreach (DataRow row in dtChart.Rows)
			{
				row["МО"] = row["МО"].ToString().Replace(" район", " р-н");
			}

            SortTable(ref dtChart, dtChart.Columns["МО"], dtChart.Columns["Цена МО"]);

            UltraChart2.Axis.Y.RangeMin = Convert.ToDouble(dtChart.Rows[0]["Цена МО"]) * 0.95;
            UltraChart2.Axis.Y.RangeMax = Convert.ToDouble(dtChart.Rows[dtChart.Rows.Count - 1]["Цена МО"]) * 1.05;

            UltraChart2.Data.SwapRowsAndColumns = true;
			UltraChart2.DataSource = dtChart.DefaultView;
		}

		protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
		{
			for (int i = 0; i < e.SceneGraph.Count; i++)
			{
				Primitive primitive = e.SceneGraph[i];
				if (primitive is Box)
				{
                    Box box = primitive as Box;
                    if (box.Value != null)
                    {
                        double hmao, mo;
                        if (Double.TryParse(currCostHMAO.ToString(), out hmao) && Double.TryParse(box.Value.ToString(), out mo))
                        {
                            box.PE.ElementType = PaintElementType.SolidFill;
                            if (mo < hmao)
                            {
                                box.PE.Fill = Color.Green;
                            }
                            else if (mo > hmao)
                            {
                                box.PE.Fill = Color.Red;
                            }
                            else
                            {
                                box.PE.Fill = Color.Yellow;
                            }
                        }
                    }
				}
			}
            UltraChart2_AddLineWithTitle(Convert.ToDouble(currCostHMAO), "ЯНАО", Color.Yellow, e);
		}

        protected void UltraChart2_AddLineWithTitle(double value, string region, Color color, FillSceneGraphEventArgs e)
        {
            string formatString = "{0}: {1:N2} руб.";
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];
            Point p1 = new Point((int)xAxis.MapMinimum, (int)yAxis.Map(value));
            Point p2 = new Point((int)xAxis.MapMaximum, (int)yAxis.Map(value));
            Line line = new Line(p1, p2);
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = color;
            line.PE.StrokeWidth = 2;
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.labelStyle.Orientation = TextOrientation.Horizontal;
            text.labelStyle.Font = new System.Drawing.Font("Arial", 10, FontStyle.Regular);
            text.labelStyle.FontColor = color;

            text.labelStyle.HorizontalAlign = StringAlignment.Near;
            text.labelStyle.VerticalAlign = StringAlignment.Near;

            Size size = new Size(500, 15);
            Point p;

            p = new Point(p1.X + 50, p1.Y - 20);

            text.bounds = new System.Drawing.Rectangle(p, size);

            text.SetTextString(String.Format(formatString, region, value));

            e.SceneGraph.Add(text);
        }

		#endregion

        private void SortTable(ref DataTable table, DataColumn keyColumn, DataColumn sortColumn)
        {
            if (table == null || table.Rows.Count == 0)
                return;
            object[] sortArray = new object[table.Rows.Count];
            object[] keysArray = new object[table.Rows.Count];
            int n = 0;
            foreach (DataRow row in table.Rows)
            {
                keysArray[n] = row[keyColumn];
                sortArray[n] = row[sortColumn];
                ++n;
            }

            Array.Sort(sortArray, keysArray);

            DataTable newTable = table.Clone();

            for (int i = 0; i < keysArray.Length; ++i)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row[keyColumn] == keysArray[i])
                    {
                        newTable.ImportRow(row);
                        break;
                    }
                }
            }

            table = newTable;

        }

	}

}
