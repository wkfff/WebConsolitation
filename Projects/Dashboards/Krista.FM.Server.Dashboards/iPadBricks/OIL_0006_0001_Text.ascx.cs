using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class OIL_0006_0001_Text : UserControl
    {
        public int ChartWidth = 750;
        public int ChartHeight = 235;
        private double avg;
        private string shortFO;

        DataTable dtChart;
        private DataTable dtAvg;
        private DataTable dtLimit;

        private Dictionary<string, double> limitDictionary;

        private string reportCode;
        public string ReportCode
        {
            get
            {
                return reportCode;
            }
            set
            {
                reportCode = value;
            }
        }

        private string reportPrefix;
        public string ReportPrefix
        {
            get
            {
                return reportPrefix;
            }
            set
            {
                reportPrefix = value;
            }
        }

        public int ControlHeight
        {
            get
            {
                return Convert.ToInt32(UltraChart.Height.Value + 110);
            }
        }

        private DateTime reportDate;
        public DateTime ReportDate
        {
            get
            {
                return reportDate;
            }
            set
            {
                reportDate = value;
            }
        }

        private DateTime lastDate;
        public DateTime LastDate
        {
            get
            {
                return lastDate;
            }
            set
            {
                lastDate = value;
            }
        }

        private string oilName = "Бензин марки АИ-80";
        public string OilName
        {
            get
            {
                return oilName;
            }
            set
            {
                oilName = value;
            }
        }

        private string foName = "Российская Федерация";
        public string FoName
        {
            get
            {
                return foName;
            }
            set
            {
                foName = value;
            }
        }

        private string foId = "0";
        public string FoId
        {
            get
            {
                return foId;
            }
            set
            {
                foId = value;
            }
        }

        private string oilId = "";

        public string OilId
        {
            get
            {
                return oilId;
            }
            set
            {
                oilId = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UltraChart.ChartType = ChartType.StackBarChart;

            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            
            UltraChart.Data.SwapRowsAndColumns = true;

            UltraChart.Axis.Y.Extent = 180;
            UltraChart.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;

            UltraChart.Axis.X.RangeType = AxisRangeType.Custom;
            UltraChart.Axis.X.RangeMax = 45;
            UltraChart.Axis.X.RangeMin = 20;
            UltraChart.Axis.X.Extent = 20;
            
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.X.Labels.Visible = true;

            UltraChart.Data.ZeroAligned = false;
            UltraChart.Legend.Visible = false;

            UltraChart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL></span>";
            UltraChart.Tooltips.Font.Name = "Verdana";
            UltraChart.Tooltips.Font.Size = 12;

            UltraChart.Axis.X.MajorGridLines.Visible = false;
            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Text = "Руб.";
            UltraChart.TitleBottom.Font = new Font("Verdana", 8);
            UltraChart.TitleBottom.FontColor = Color.FromArgb(209,209,209);
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Far;
            UltraChart.TitleBottom.Margins.Left = UltraChart.Axis.Y.Extent;
            
            CustomParams.MakeOilParams(oilId, "id");

            IncomesHeader.MultitouchReport = String.Format("oil_0006_0004{0}_fo={1}", reportPrefix, foId);
            IncomesHeader.Text = oilName;

            SetColorModel();
            AvgDataBind();
            LimitDataBind();
            ChartDataBind();
        }

        private void AvgDataBind()
        {
            string query = DataProvider.GetQueryText(ReportCode + "_avg");
            dtAvg = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtAvg);

            if (dtAvg.Rows.Count > 0)
            {
                avg = GetRowValue(dtAvg.Rows[0], "Среднее");
            }
        }

        private void LimitDataBind()
        {
            string query = DataProvider.GetQueryText(ReportCode + "_limit");
            dtLimit = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtLimit);

            limitDictionary = new Dictionary<string,double>();

            foreach (DataRow row in dtLimit.Rows)
            {
                double limit = GetRowValue(row, "Предел");
                if (limit != Double.MinValue)
                {
                    limitDictionary.Add(row[0].ToString(), limit);
                }
            }
        }

        private void ChartDataBind()
        {
            string query = DataProvider.GetQueryText(ReportCode + "_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtChart);

            shortFO = RegionsNamingHelper.ShortName(CustomParam.CustomParamFactory("fullRegionName").Value);
            string foComment = shortFO == "РФ"
                                   ? "субъектах РФ"
                                   : String.Format("субъектах РФ, входящих в состав {0},", shortFO);

            if (dtChart.Rows.Count > 0)
            {
                UltraChart.Width = ChartWidth;

                UltraChart.Height = dtChart.Rows.Count * 38 + 66;

                if (dtChart.Columns.Count > 0)
                {
                    dtChart.Columns.RemoveAt(0);
                }

                UltraChart.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }

                UltraChart.DataBind();

                UltraChart.Visible = true;
                Label1.Text = String.Format("На&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;г. рост цен на&nbsp;<span class='DigitsValue'>{2}</span>&nbsp;к {1:dd.MM.yyyy}&nbsp;г. и\\или превышение пороговых цен, установленных Минэнерго России на 31.12.2011, наблюдается в следующих субъектах РФ и компаниях",
                    reportDate, lastDate, CRHelper.ToLowerFirstSymbol(oilName));
            }
            else
            {
                UltraChart.Visible = false;
                Label1.Text = String.Format("На&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;г. в {3}&nbsp;<span class='DigitsValue'>не наблюдается</span>&nbsp;роста цен к {1:dd.MM.yyyy}&nbsp;г. и\\или превышение пороговых цен, установленных Минэнерго России на 31.12.2011 на&nbsp;<span class='DigitsValue'>{2}</span>",
                    reportDate, lastDate, CRHelper.ToLowerFirstSymbol(oilName), foComment);
            }
        }

        private void SetColorModel()
        {
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LightBlue;
            Color color2 = Color.Red;
            Color color3 = Color.Green;

            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            UltraChart.ColorModel.Skin.ApplyRowWise = false;

            UltraChart.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            UltraChart.Effects.Enabled = true;
            UltraChart.Effects.Effects.Add(effect);
        }

        private static string GetShortRegionName(string fullName)
        {
            fullName = fullName.Replace("автономный округ", "АО");
            fullName = fullName.Replace("автономная область", "АО");
            fullName = fullName.Replace("Чувашская республика-Чувашия", "Республика Чувашия");
            fullName = fullName.Replace("Республика Адыгея (Адыгея)", "Республика Адыгея");
            return fullName;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            double xMax = xAxis.MapMaximum;
            double yMax = yAxis.MapMaximum;
            double yMin = yAxis.MapMinimum;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text)primitive;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new Font("Verdana", 8);
                    axisText.labelStyle.WrapText = false;
                }
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text text = (Text)primitive;
                    if (text.Row == -1)
                    {
                        text.bounds.Width = 170;

                        text.labelStyle.HorizontalAlign = StringAlignment.Far;

                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.FontSizeBestFit = false;
                        text.labelStyle.Font = new Font("Verdana", 8);
                        text.labelStyle.WrapText = true;

                        text.SetTextString(GetShortRegionName(text.GetTextString()));
                    }
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string serieName = GetRowStringValue(dtChart.Rows[box.Row], "Наименование").Replace("\"", "'");
                        double lastValue = GetRowValue(dtChart.Rows[box.Row], "На начало года");
                        double increaseValue = GetRowValue(dtChart.Rows[box.Row], "Прирост");
                        double decreaseValue = GetRowValue(dtChart.Rows[box.Row], "Снижение");

                        if (lastValue != Double.MinValue && increaseValue != Double.MinValue && decreaseValue != Double.MinValue)
                        {
                            lastValue = lastValue + decreaseValue;

                            double currValue = lastValue + increaseValue - decreaseValue;

                            double deviationValue = increaseValue == 0 ? -decreaseValue : increaseValue;
                            double deviationPercent = currValue == 0 ? 0 : deviationValue / lastValue;
                            double limit = GetSubjectLimit(serieName);

                            string limitString = String.Empty; 
                            if (limit > 0)
                            {
                                limitString = currValue > limit
                                                  ? String.Format("\n<b><span style='color:red;'>Превышен</span></b>&nbsp;порог Минэнерго России\n31.12.2011:&nbsp;<b>{0:N2}</b>&nbsp;руб.", limit)
                                                  : String.Format("\nПорог Минэнерго России &nbsp;\n31.12.2011:&nbsp;<b>{0:N2}</b>&nbsp;руб.", limit);
                            }

                            string devaitonString = deviationValue >= 0 ? "Рост" : "Снижение";

                            string growString = deviationValue == 0
                                                    ? String.Format("Цена не изменилась")
                                                    : String.Format("{2}&nbsp;<b>{0:N2}</b>&nbsp;руб. (<b>{3}{1:P2}</b>)", deviationValue, deviationPercent, devaitonString, deviationValue >= 0 ? "+" : String.Empty);

                            box.DataPoint.Label = String.Format("{0}\n{1}\n{2:dd.MM.yyyy}:&nbsp;<b>{3:N2}</b>&nbsp;руб.\n{4:dd.MM.yyyy}:&nbsp;<b>{5:N2}</b>&nbsp;руб.\n{6}{7}",
                                        serieName, oilName, lastDate, lastValue, reportDate, currValue, growString, limitString);

                            if (box.Column == 0)
                            {
                                int textWidth = 160;

                                Text text = new Text();
                                text.bounds = new Rectangle(Convert.ToInt32(xMax - textWidth), box.rect.Top, textWidth, box.rect.Height);
                                text.labelStyle.Font = new Font("Verdana", 9);
                                text.labelStyle.FontColor = Color.White;

                                if (deviationValue == 0)
                                {
                                    text.SetTextString("Цена не изменилась");
                                }
                                else
                                {
                                    text.SetTextString(String.Format("{2}{0:N2} руб. ({1:P2})", deviationValue,
                                                                     deviationPercent,
                                                                     deviationValue >= 0 ? "+" : String.Empty));
                                }
                                
                                e.SceneGraph.Add(text);

                                if (limit != 0 && currValue > limit)
                                {
                                    Ellipse ellipse = new Ellipse(new Point((int)xMax - 10, box.rect.Top + box.rect.Height / 2), 10);
                                    ellipse.PE.Fill = Color.Red;
                                    e.SceneGraph.Add(ellipse);
                                }
                            }
                        }
                    }
                }
            }

            if (avg != Double.MinValue)
            {
                int textWidth = 130;
                int textHeight = 30;

                Line line = new Line();
                line.p1 = new Point((int)xAxis.Map(avg), (int)yMin + textHeight);
                line.p2 = new Point((int)xAxis.Map(avg), (int)yMax);
                line.PE.Stroke = Color.White;
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                e.SceneGraph.Add(line);

               Text text = new Text();
                text.bounds = new Rectangle(Convert.ToInt32(xAxis.Map(avg) - textWidth * 0.5), (int)yMin + textHeight - 5, textWidth, textHeight);
                text.labelStyle.Font = new Font("Verdana", 9);
                text.labelStyle.FontColor = Color.White;
                text.labelStyle.HorizontalAlign = StringAlignment.Center;
                text.SetTextString(String.Format("Среднее по {1}\n{0:N2} руб.", avg, shortFO));
                e.SceneGraph.Add(text);
            }
        }

        private double GetSubjectLimit(string name)
        {
            foreach (string key in limitDictionary.Keys)
            {
                if (name.Contains(key))
                {
                    return limitDictionary[key];
                }
            }

            return Double.MaxValue;
        }

        private static double GetRowValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
                {
                    return Convert.ToDouble(row[columnName]);
                }
            }

            return Double.MinValue;
        }

        private static string GetRowStringValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
                {
                    return row[columnName].ToString();
                }
            }

            return String.Empty;
        }
    }
}