using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0002_0064 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();

        // выбранный период
        private CustomParam selectedPeriod;

        private DateTime date;
        private GridHeaderLayout headerLayout;
        private int endYear;
        private DataSet LoadData()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/OIV_0001_0001/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds;
        }
        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0064_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            string dimension = "";

            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            DateTime newDate = new DateTime(endYear, 1, 1);
            dimension = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", newDate, 1);
            UserParams.PeriodDimension.Value = dimension;

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            OutcomesGrid.DataBind();
            OutcomesGrid.Visible = false;
            //lbDescription.Text = string.Format("Структура органов исполнительной власти на 01.01.{0} года", endYear);
            IPadElementHeader1.Text = string.Format("Структура органов исполнительной власти на 01.01.{0} года", endYear + 1);
            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            SetStackChartAppearanceUnic();
            UltraChart1.Width = 750;
            UltraChart1.Height = 1000;

            foreach (DataRow row in dtChart.Rows)
            {
                if (!grbsNames.ContainsKey(row[1].ToString()))
                {
                    grbsNames.Add(row[0].ToString(), row[1].ToString());
                }
            }

            dtChart.Columns.RemoveAt(0);

            UltraChart1.Series.Clear();
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(3, dtChart));
            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.DataBind();

        }

        private Dictionary<String, String> grbsNames = new Dictionary<string, string>();

        void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    //CRHelper.SaveToErrorLog("До проверки = "+box.Series.Label);
                    if (box.Series != null/* &&
                        grbsNames.ContainsKey(box.Series.Label)*/)
                    {
                        //получаем ГРБС
                        //box.Series.Label = GetWarpedHint(grbsNames[box.Series.Label]);
                        //получаем Фактическая численность
                        box.Series.Label = "<b>" + GetWarpedHint(dtGrid.Rows[box.Row][0].ToString()) + "</b>";
                        box.Series.Label = box.Series.Label + "<br/>Фактическая численность&nbsp;<b>" + dtGrid.Rows[box.Row][2] + "</b>, чел.<br/>";
                        box.Series.Label = box.Series.Label + "Предельная штатная численность&nbsp;<b>" + dtGrid.Rows[box.Row][1] + "</b>, чел.<br/>";
                        if (dtGrid.Rows[box.Row][3].ToString() != "0")
                            box.Series.Label = box.Series.Label + "Вакансии&nbsp;<b>" + dtGrid.Rows[box.Row][3] + "</b>, чел.";
                        CRHelper.SaveToErrorLog(box.Series.Label);
                    }
                }
            }

        }

        private string GetWarpedHint(string hint)
        {
            string name = hint.Replace("\"", "'");
            if (name.Length > 30)
            {
                int k = 11;

                for (int j = 0; j < name.Length; j++)
                {
                    k++;
                    if (k > 30 && name[j] == ' ')
                    {
                        name = name.Insert(j, "<br/>");
                        k = 0;
                    }
                }
            }
            return name;
        }

        private void SetStackChartAppearanceUnic()
        {

            UltraChart1.ChartType = ChartType.StackBarChart;
            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 90;

            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>";
            //string.Format("<span style='font-family: Arial; font-size: 14pt'>ГРБС&nbsp;<SERIES_LABEL> <br />Фактическая численность&nbsp;<b><DATA_VALUE_ITEM:N0></b>&nbsp;чел.</span>");
            SetLabelsClipTextBehavior(UltraChart1.Axis.Y.Labels.SeriesLabels.Layout);

            //UltraChart1.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            //UltraChart1.Axis.X.TickmarkInterval = 50;
            //UltraChart1.Axis.X.TickmarkIntervalType = AxisIntervalType.Months;

            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.X.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.X.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.Black;



            UltraChart1.Axis.X.Labels.Visible = true;

            UltraChart1.Axis.X.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.Y.Labels.Visible = false;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;

            UltraChart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;


            UltraChart1.Axis.Y.Extent = 180;
            UltraChart1.Axis.X.Extent = 50;

            //UltraChart1.TitleLeft.Visible = true;
            //UltraChart1.TitleLeft.Text = "тыс.руб.";
            //UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            //UltraChart1.TitleLeft.FontColor = Color.White;
            //UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            //UltraChart1.TitleLeft.Margins.Bottom = 350;
            UltraChart1.Legend.Font = new Font("Verdana", 10);
            UltraChart1.Legend.SpanPercentage = 8;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
            /*  UltraChart1.Axis.X.NumericAxisType = NumericAxisType.Logarithmic;
              UltraChart1.Axis.X.LogBase = 3;
              UltraChart1.Axis.X.LogZero = 0.5;
              UltraChart1.Axis.Y.LineColor = Color.Transparent;*/
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.Green;

                    }
                case 2:
                    {

                        return Color.Red;
                    }
                default:
                    {
                        return Color.White;
                    }
            }
        }

        private void SetLabelsClipTextBehavior(AxisLabelLayoutAppearance layout)
        {
            layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            layout.BehaviorCollection.Clear();
            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();

            behavior.ClipText = false;
            behavior.Enabled = true;
            behavior.Trimming = StringTrimming.None;
            behavior.UseOnlyToPreventCollisions = false;
            layout.BehaviorCollection.Add(behavior);
        }

        #region Обработчики грида

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0064_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "ГРБС", dtGrid);
            string query2 = DataProvider.GetQueryText("FO_0002_0064_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "ГРБС", dtChart);

            DataSet ds = LoadData();
            DataTable dtDate = ds.Tables["date"];
            DataTable dtGrbs = ds.Tables["grbsTable"];

            Dictionary<String, String> grbsNames = new Dictionary<string, string>();
            foreach (DataRow grbsRow in dtGrbs.Rows)
            {
                if (grbsRow[1] != DBNull.Value)
                {
                    if (!grbsNames.ContainsKey(grbsRow[1].ToString()))
                    {
                        grbsNames.Add(grbsRow[1].ToString(), grbsRow["staffCountStartYear"].ToString());
                    }
                }
            }
            #region Склеивание строк
            int SumMinFin = 0;

            if (grbsNames.ContainsKey("Министерство  финансов и налоговой политики Новосибирской области"))
            {
                int value;
                if (Int32.TryParse(grbsNames["Министерство  финансов и налоговой политики Новосибирской области"].ToString(), out value))
                {
                    SumMinFin = value;
                }
                if (grbsNames.ContainsKey("Территориальные органы Министерства финансов и налоговой политики Новосибирской области"))
                {
                    if (Int32.TryParse(grbsNames["Территориальные органы Министерства финансов и налоговой политики Новосибирской области"].ToString(), out value))
                    { SumMinFin += value; }

                    if (!grbsNames.ContainsKey("Министерство финансов и налоговой политики Новосибирской области"))
                    {
                        grbsNames.Add("Министерство финансов и налоговой политики Новосибирской области", SumMinFin.ToString());
                    }
                }


            }
            SumMinFin = 0;
            if (grbsNames.ContainsKey("Министерство социального развития  Новосибирской области"))
            {
                int value;
                if (Int32.TryParse(grbsNames["Министерство социального развития  Новосибирской области"].ToString(), out value))
                {
                    SumMinFin = value;
                }
                if (grbsNames.ContainsKey("Территориальные органы Министерства социального развития Новосибирской области"))
                {
                    if (Int32.TryParse(grbsNames["Территориальные органы Министерства социального развития Новосибирской области"].ToString(), out value))
                    {
                        SumMinFin += value;
                    }
                    if (!grbsNames.ContainsKey("Министерство социального развития Новосибирской области"))
                    {
                        grbsNames.Add("Министерство социального развития Новосибирской области", SumMinFin.ToString());
                    }
                }
            }
            #endregion
            foreach (DataRow dtRow in dtGrid.Rows)
            {
                if (grbsNames.ContainsKey(dtRow[0].ToString()))
                {
                    int value;
                    if (Int32.TryParse(grbsNames[dtRow[0].ToString()], out value))
                    {
                        dtRow[1] = value;
                    }
                }
            }

            for (int i = dtGrid.Rows.Count - 1; i >= 0; i--)
            {
                if (dtGrid.Rows[i][0].ToString().ToLower() != "итого")
                {
                    if (!grbsNames.ContainsKey(dtGrid.Rows[i][0].ToString()))
                    {
                        dtGrid.Rows.Remove(dtGrid.Rows[i]);
                    }
                }
            }

            foreach (DataRow dtRow in dtChart.Rows)
            {

                if (grbsNames.ContainsKey(dtRow[0].ToString()))
                {
                    int value;
                    if (Int32.TryParse(grbsNames[dtRow[0].ToString()], out value))
                    {
                        dtRow[3] = value;
                    }
                }
            }
            for (int i = dtChart.Rows.Count - 1; i >= 0; i--)
            {
                if (!grbsNames.ContainsKey(dtChart.Rows[i][0].ToString()))
                {
                    dtChart.Rows.Remove(dtChart.Rows[i]);
                }
            }
            decimal sum1, sum2, sum3;
            sum1 = 0;
            sum2 = 0;
            sum3 = 0;
            decimal different;
            for (int i = 0; i < dtGrid.Rows.Count - 1; i++)
            {
                different = 0;
                object val1 = dtGrid.Rows[i]["Предельная штатная численность, чел."];
                object val2 = dtGrid.Rows[i]["Фактическая численность, чел."];
                if (val1 != DBNull.Value || val2 != DBNull.Value)
                {
                    different = (decimal)val1 - (decimal)val2;
                    sum1 = (decimal)val1 + sum1;
                    sum2 = (decimal)val2 + sum2;
                    sum3 = different + sum3;
                }
                dtGrid.Rows[i]["разница"] = different;
            }

            dtGrid.Rows[dtGrid.Rows.Count - 1]["Предельная штатная численность, чел."] = sum1;
            dtGrid.Rows[dtGrid.Rows.Count - 1]["Фактическая численность, чел."] = sum2;
            dtGrid.Rows[dtGrid.Rows.Count - 1]["разница"] = sum3;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                different = 0;
                object val1 = dtChart.Rows[i]["Предельная штатная численность"];
                object val2 = dtChart.Rows[i]["Фактическая численность, чел."];
                if (val1 != DBNull.Value || val2 != DBNull.Value)
                {
                    different = (decimal)val1 - (decimal)val2;

                }
                dtChart.Rows[i]["Отклонение фактической от предельной штатной численности, чел."] = different;
            }
            OutcomesGrid.DataSource = dtGrid;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            IPadElementHeader1.Text = string.Format("<span class='ServeText' style='font-size: 14pt'>Структура&nbsp;органов&nbsp;исполнительной&nbsp;власти&nbsp;за</span>&nbsp;<b><span class='DigitsValue'>{0}</span></b>&nbsp;<span class='ServeText' style='font-size: 14pt'>год</span>", endYear);
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = 200;
            e.Layout.Bands[0].Columns[2].Width = 200;
            e.Layout.Bands[0].Columns[3].Width = 110;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            headerLayout.AddCell("ГРБС");
            headerLayout.AddCell("Предельная штатная численность, чел.");
            headerLayout.AddCell("Фактическая численность, чел.");
            headerLayout.AddCell("Вакансии, чел");
            headerLayout.ApplyHeaderInfo();
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            //e.Row.Style.Height = 60;
            if (e.Row.Cells[0].Value.ToString().ToLower() != "итого")
            {
                SetCorner(e, 3);
            }
            else
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionCorner(e, 5, 0, true);

            //if (e.Row.Cells[1].Value.ToString() == "Всего")
            //{
            //    foreach (UltraGridCell cell in  e.Row.Cells)
            //    {
            //        cell.Style.Font.Bold = true;
            //    }
            //    e.Row.Cells[5].Style.Font.Size = 14;
            //}
            //e.Row.Cells[5].Style.Padding.Right = 6;


        }

        void SetCorner(RowEventArgs e, int i)
        {
            string img = "";
            decimal value1 = (decimal)e.Row.Cells[1].Value;
            decimal value2 = (decimal)e.Row.Cells[2].Value;
            //string title;
            if (value1 < value2)
            {
                img = "~/images/CornerRed.gif";
            }
            if (value1 > value2)
            { img = "~/images/CornerGreen.gif"; }
            if (img == "~/images/CornerRed.gif")
            {
                e.Row.Cells[i].Title = "Фактическая численность превышает предельную";
            }
            else
                if (img == "~/images/CornerGreen.gif")
                {
                    e.Row.Cells[i].Title = "Фактическая численность не превышает предельную";
                }
            e.Row.Cells[i].Style.BackgroundImage = img;

            e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top";
        }

        #endregion
    }
}
