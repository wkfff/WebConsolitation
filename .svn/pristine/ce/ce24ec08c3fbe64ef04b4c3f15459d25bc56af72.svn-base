using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Web.UI.WebControls.Image;
using System.Xml;
using System.IO;
using Dundas.Maps.WebControl;
using System.Collections;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Core;
using Infragistics.WebUI.UltraWebChart;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FW_0003_0001_Horizontal : CustomReportPage
    {
        private GridHeaderLayout headerLayout;
        private DateTime date;
        private Collection<string> moCollection = new Collection<string>();
        private int maxActive = 0;

        private double maxSqare = 0;
        private int maxCount = 0;

        private DataTable dtChart1 = new DataTable(); 
        private DataTable dtChart2 = new DataTable();

        private DataSet LoadData()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/iphone/FW_0003_0001/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //DataSet ds = LoadData();
            //DataTable dtDate = ds.Tables["date"];
            //DataTable dtForestry = ds.Tables["forestry"];

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FW_0003_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);
            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            

            date = CRHelper.PeriodDayFoDate(UserParams.PeriodCurrentDate.Value);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", date.AddDays(-1), 5);  
            
            DataTable dtForestry = new DataTable();
            query = DataProvider.GetQueryText("FW_0003_0001_data");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtForestry);

           // date = Convert.ToDateTime(dtDate.Rows[0]["curretnDate"]);

            foreach (DataRow row in dtForestry.Rows)
            {
                string mo = row["stateId"].ToString();
                if (!(moCollection.Contains(mo)))
                {
                    moCollection.Add(mo);
                }
            }

            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            WebGrid1.InitializeRow += new InitializeRowEventHandler(WebGrid1_InitializeRow);
            DataTable source = dtForestry;
            //source.Columns.RemoveAt(0);

            for (int i = source.Rows.Count - 1; i > source.Rows.Count - 7; i--)
            {
                source.Rows[i][0] = String.Format("В {0} г. на {1:dd.MM}", date.AddYears(source.Rows.Count - i - 7).Year, date);
            }

            for (int i = 0; i < source.Rows.Count - 7; i++)
            {
                if (source.Rows[i][4] != DBNull.Value &&
                    source.Rows[i][4].ToString() != String.Empty)
                {
                    int active = Convert.ToInt32(source.Rows[i][4]);
                    if (active > maxActive)
                    {
                        maxActive = active;
                    }
                }
            }

            DataRow totalRow = source.Rows[source.Rows.Count - 7];

            int total = 0;
            if (totalRow[4] != DBNull.Value &&
                totalRow[4].ToString() != String.Empty)
            {
                int active = Convert.ToInt32(totalRow[4]);
                total += active;                
            }
            if (totalRow[6] != DBNull.Value &&
                totalRow[6].ToString() != String.Empty)
            {
                total += Convert.ToInt32(totalRow[6]);
            }

            if (total < 1)
            {
                mainDiv.Style.Add("background-image", "url(../../../images/ForestTree.png)");
            }
            else if (total < 10)
            {
                mainDiv.Style.Add("background-image", "url(../../../images/ForestFireMiddle.png)");
            }
            else
            {
                mainDiv.Style.Add("background-image", "url(../../../images/flame.png)");
            }
            source.AcceptChanges();

            SetupGrid(UltraWebGrid1, source);
            UltraWebGrid1.DisplayLayout.Bands[0].HeaderLayout.Clear();
            SetupGrid(WebGrid1, source);
            
            lbDescription.Text = String.Format("Ежедневная сводная ведомость лесных пожаров в лесном фонде Ханты-Мансийского авт. округа – Югры в разрезе территориальных отделов по состоянию на 9:00 местного времени&nbsp;<span class='DigitsValue'>{0}</span>", date.AddDays(1).ToShortDateString());

            DataRow rowTotal = dtForestry.Rows[dtForestry.Rows.Count - 7];

            Label1.Text = String.Format("действующие пожары<br/>на 9:00 {0:dd.MM}:&nbsp;<div style='position: relative; top: -22px; text-align: center; left: 123px;  width: 40px; height: 23px; background-image: url(../../../images/jawRed.png); background-position: left top; background-repeat: no-repeat'>{1:N0}</div>", date.AddDays(1), rowTotal["Действует пожаров "]);
            Label2.Text = String.Format("в т.ч. локализовано<br/>за {0:dd.MM}:&nbsp;<div style='position: relative; top: -22px; text-align: center; left: 80px;  width: 40px; height: 23px; background-image: url(../../../images/jawYellow.png); background-position: left top; background-repeat: no-repeat'>{1:N0}</div>", date, rowTotal["Локализовано пожаров "]);
            Label3.Text = String.Format("ликвидировано<br/>с начала года:&nbsp;<div style='position: relative; top: -22px; text-align: center; left: 133px;  width: 40px; height: 23px; background-image: url(../../../images/jawGreen.png); background-position: left top; background-repeat: no-repeat'>{1:N0}</div>", date, rowTotal["Ликвидировано пожаров "]);

            SetMapSettings(DundasMap1, "Охваченная пожаром\nлесная площадь, га", "#FROMVALUE{N0} - #TOVALUE{N0}");

            FillMapData(DundasMap1, source);
            //string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/FW_0003_0001/") + "TouchElementBounds.xml";
            //Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/FW_0003_0001/"));
            //File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"FO_0003_0002_{0}\" bounds=\"x=0;y=120;width=768;height=260\" openMode=\"incomes\"/><element id=\"FO_0003_0004_{0}\" bounds=\"x=0;y=380;width=768;height=230\" openMode=\"outcomes\"/><element id=\"FO_0003_0005_{0}\" bounds=\"x=0;y=610;width=768;height=100\" openMode=\"rests\"/><element id=\"FO_0003_0006_{0}\" bounds=\"x=0;y=710;width=768;height=130\" openMode=\"\"/></touchElements>", CustomParams.GetSubjectIdByName(UserParams.StateArea.Value)));

            DataTable dtMax = new DataTable();
            query = DataProvider.GetQueryText("FW_0003_0001_max");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtMax);

            if (dtMax.Rows[0][0] != DBNull.Value &&
                dtMax.Rows[0][0].ToString() != String.Empty)
            {               
                maxSqare = Convert.ToDouble(dtMax.Rows[0][0].ToString());
            }

            if (dtMax.Rows[0][1] != DBNull.Value &&
                dtMax.Rows[0][1].ToString() != String.Empty)
            {
                maxCount = Convert.ToInt32(dtMax.Rows[0][1].ToString());
            }

            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            SetColumnChartAppearance(UltraChart1, dtChart1);
                       
            query = DataProvider.GetQueryText("FW_0003_0001_chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart1);

            SetCustomSkin(UltraChart1);
            UltraChart1.Series.Clear();
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtChart1));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(2, dtChart1));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(3, dtChart1));
            UltraChart1.DataBind();

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", date.AddDays(-1), 5);     
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", date.AddDays(-2), 5);

            UltraChart2.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            SetColumnChartAppearance(UltraChart2, dtChart2);

            query = DataProvider.GetQueryText("FW_0003_0001_chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart2);

            SetCustomSkin2(UltraChart2);
            UltraChart2.Series.Clear();
            UltraChart2.Series.Add(CRHelper.GetNumericSeries(1, dtChart2));
            UltraChart2.Series.Add(CRHelper.GetNumericSeries(2, dtChart2));
            UltraChart2.Series.Add(CRHelper.GetNumericSeries(3, dtChart2));
            UltraChart2.DataBind();

            IPadElementHeader1.Text = String.Format("Эффективность тушения пожаров за отчетные сутки", date.AddDays(1).ToShortDateString());
            IPadElementHeader2.Text = String.Format("Эффективность тушения пожаров за&nbsp;<span class='DigitsValueXLarge'>{0}</span>", date.AddDays(-1).ToShortDateString());
        }

        private void SetColumnChartAppearance(UltraChart chart, DataTable source)
        {
            chart.Width = 1010;
            chart.Height = 250;

            chart.ChartType = ChartType.ColumnChart;
            chart.ColumnChart.ColumnSpacing = 0;
            chart.ColumnChart.SeriesSpacing = 0;

            chart.Data.SwapRowsAndColumns = true;

            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.X.Labels.Orientation = TextOrientation.Custom;
            chart.Axis.X.Labels.OrientationAngle = 0;

            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br/><DATA_VALUE:N2></span>";
            
            chart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.X.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.X.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            chart.Axis.X.MinorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.Y.MinorGridLines.Color = Color.FromArgb(150, 150, 150);

            chart.Axis.X.Labels.FontSizeBestFit = false;
            chart.Axis.X.Labels.Font = new Font("Arial", 12, FontStyle.Bold);
            chart.Axis.X.Labels.FontColor = Color.White;

            chart.Axis.X.Labels.Visible = true;
            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.Axis.X.Labels.VerticalAlign = StringAlignment.Near;

            chart.Axis.Y.Labels.FontSizeBestFit = false;
            chart.Axis.Y.Labels.Font = new Font("Verdana", 10);
            chart.Axis.Y.Labels.Visible = true;

            chart.Axis.Y.Extent = 25;
            chart.Axis.X.Extent = 15;

            chart.Axis.Y.RangeType = AxisRangeType.Custom;
            chart.Axis.Y.RangeMax = maxSqare + maxSqare / 5;
            chart.Axis.Y.RangeMin = 0;

            chart.Legend.Visible = false;

            chart.Legend.Font = new Font("Verdana", 10);
            chart.Legend.SpanPercentage = 18;
            SetLabelsClipTextBehavior(chart.Axis.X.Labels.Layout);

            chart.BackColor = Color.Transparent;
            chart.Border.Color = Color.Transparent;

            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Text = "Га";
            //UltraChart1.TitleLeft.Margins.Bottom = 25;
            chart.TitleLeft.Orientation = TextOrientation.Horizontal;
            chart.TitleLeft.HorizontalAlign = StringAlignment.Near;
            chart.TitleLeft.VerticalAlign = StringAlignment.Near;
            chart.TitleLeft.FontColor = Color.White; //Color.FromArgb(209, 209, 209);
            chart.TitleLeft.Font = new Font("Verdana", 10);
        }

        private static void SetCustomSkin(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = false;
            chart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 3; i++)
            {
                PaintElement pe = new PaintElement();

                Color color = Color.White;
                Color stopColor = Color.White;

                switch (i)
                {
                    case 1:
                        {
                            color = Color.FromArgb(247, 10, 2);
                            break;
                        }
                    case 2:
                        {
                            color = Color.FromArgb(183, 152, 40);
                            break;
                        }
                    case 3:
                        {
                            color = Color.FromArgb(47, 182, 55);
                            break;
                        }
                }

                pe.Fill = color;
                pe.FillStopColor = color;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.Hatch = FillHatchStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                chart.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private static void SetCustomSkin2(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = false;
            chart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 3; i++)
            {
                PaintElement pe = new PaintElement();

                Color color = Color.White;
                Color stopColor = Color.White;

                switch (i)
                {
                    case 1:
                        {
                            color = Color.FromArgb(232, 116, 112);
                            break;
                        }
                    case 2:
                        {
                            color = Color.FromArgb(220, 200, 130);
                            break;
                        }
                    case 3:
                        {
                            color = Color.FromArgb(120, 190, 124);
                            break;
                        }
                }

                pe.Fill = color;
                pe.FillStopColor = color;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.Hatch = FillHatchStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                chart.ColorModel.Skin.PEs.Add(pe);
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

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        int nativeWidth = box.rect.Width;

                        Text text = new Text();
                        text.labelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
                        text.PE.Fill = Color.White;

                        int yPos = box.rect.Y - 45;
                        text.bounds = new Rectangle(box.rect.X, yPos, nativeWidth, 50);

                        int value = Convert.ToInt32(dtChart1.Rows[0][box.Column + 4]);
                        string fireDescription = value == 1 ? "пожар" : value > 1 && value < 5 ? "пожара" : "пожаров";

                        text.SetTextString(String.Format("{0:N0} {1}\n{2:N1} Га", value, fireDescription, box.Value));
                        text.labelStyle.HorizontalAlign = StringAlignment.Center;
                        e.SceneGraph.Add(text);

                        box.rect.Width = (int)(nativeWidth / maxCount * value);
                        box.rect.X = box.rect.X + (nativeWidth - box.rect.Width) / 2;
                        box.DataPoint.Label = String.Format("\"", value);
                    }
                }
            }                 
        }

        void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        int nativeWidth = box.rect.Width;

                        Text text = new Text();
                        text.labelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
                        text.PE.Fill = Color.White;

                        int yPos = box.rect.Y - 45;
                        text.bounds = new Rectangle(box.rect.X, yPos, nativeWidth, 50);

                        int value = Convert.ToInt32(dtChart2.Rows[0][box.Column + 4]);
                        string fireDescription = value == 1 ? "пожар" : value > 1 && value < 5 ? "пожара" : "пожаров";

                        text.SetTextString(String.Format("{0:N0} {1}\n{2:N1} Га", value, fireDescription, box.Value));
                        text.labelStyle.HorizontalAlign = StringAlignment.Center;
                        e.SceneGraph.Add(text);

                        box.rect.Width = (int)(nativeWidth / maxCount * value);
                        box.rect.X = box.rect.X + (nativeWidth - box.rect.Width) / 2;
                        box.DataPoint.Label = String.Format("\"", value);
                    }
                }
            }
        }

        public void SetMapSettings(MapControl dundasMap, string legendTitle, string format)
        {
            dundasMap.Width = 1060;
            dundasMap.Height = 500;
            dundasMap.Shapes.Clear();

            dundasMap.ShapeFields.Add("Name");
            dundasMap.ShapeFields["Name"].Type = typeof(string);
            dundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            dundasMap.ShapeFields.Add("Complete");
            dundasMap.ShapeFields["Complete"].Type = typeof(double);
            dundasMap.ShapeFields["Complete"].UniqueIdentifier = false;

            AddMapLayer(dundasMap, CRHelper.MapShapeType.Areas);

            dundasMap.Meridians.Visible = false;
            dundasMap.Parallels.Visible = false;
            dundasMap.ZoomPanel.Visible = false;
            dundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            dundasMap.NavigationPanel.Visible = false;
            dundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            dundasMap.Viewport.EnablePanning = true;
            dundasMap.Viewport.OptimizeForPanning = false;
            dundasMap.Viewport.BackColor = Color.Black;

            // добавляем легенду
            Legend legend = new Legend("CompleteLegend");
            legend.Visible = true;
            legend.Margins.Top = 40;
            legend.Dock = PanelDockStyle.Right;
            legend.BackColor = Color.FromArgb(75, 255, 255, 255);
            legend.BackSecondaryColor = Color.Black;
            legend.BackGradientType = GradientType.TopBottom;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Black;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.FromArgb(192, 192, 192);
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.TitleColor = Color.White;
            legend.Title = legendTitle;
            legend.AutoFitMinFontSize = 7;
            //legend.
            dundasMap.Legends.Clear();
            dundasMap.Legends.Add(legend);

            // добавляем легенду
            legend = new Legend("SymbolLegend");
            legend.Visible = true;
            legend.Margins.Top = 40;
            legend.Margins.Left = 730;
            legend.Dock = PanelDockStyle.Left;
            legend.BackColor = Color.FromArgb(75, 255, 255, 255);
            legend.BackSecondaryColor = Color.Black;
            legend.BackGradientType = GradientType.TopBottom;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Black;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.FromArgb(192, 192, 192);
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.TitleColor = Color.White;
            legend.Title = "Количество пожаров";

            if (maxActive == 0)
            {
                legend.Visible = false;
            }
            else if (maxActive == 1)
            {
                legend.Items.Add(GetLegendItem(1, "1"));
            }
            else if (maxActive == 2)
            {
                legend.Items.Add(GetLegendItem(1, "1"));
                legend.Items.Add(GetLegendItem(2, "2"));
            }
            else if (maxActive == 3)
            {
                legend.Items.Add(GetLegendItem(1, "1"));
                legend.Items.Add(GetLegendItem(2, "2"));
                legend.Items.Add(GetLegendItem(3, "3"));
            }
            else if (maxActive == 4)
            {
                legend.Items.Add(GetLegendItem(1, "1"));
                legend.Items.Add(GetLegendItem(2, "2 - 3"));
                legend.Items.Add(GetLegendItem(3, "4"));
            }
            else if (maxActive == 5)
            {
                legend.Items.Add(GetLegendItem(1, "1"));
                legend.Items.Add(GetLegendItem(2, "2 - 3"));
                legend.Items.Add(GetLegendItem(3, "4 - 5"));
            }
            else if (maxActive == 6)
            {
                legend.Items.Add(GetLegendItem(1, "1 - 2"));
                legend.Items.Add(GetLegendItem(2, "3 - 4"));
                legend.Items.Add(GetLegendItem(3, "5 - 6"));
            }
            else
            {
                legend.Items.Add(GetLegendItem(1, String.Format("1 - {0:N0}", Math.Round(maxActive / 3.0))));
                legend.Items.Add(GetLegendItem(2, String.Format("{0:N0} - {1:N0}", Math.Round(maxActive / 3.0) + 1, Math.Round(maxActive / 3.0) * 2)));
                legend.Items.Add(GetLegendItem(3, String.Format("{0:N0} - {1:N0}", Math.Round(maxActive / 3.0) * 2 + 1, maxActive)));
            }

            legend.AutoFitMinFontSize = 7;
            dundasMap.Legends.Add(legend);

            // добавляем правила раскраски
            dundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Complete";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 3;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Orange;
            rule.MiddleColor = Color.Coral;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = format;
            dundasMap.ShapeRules.Add(rule);
        }

        private static LegendItem GetLegendItem(int scale, string text)
        {
            LegendItem item = new LegendItem();

            LegendCell cell = new LegendCell();
            cell.CellType = LegendCellType.Image;
            cell.Image = "../../../images/bigFire.gif";
            cell.ImageSize = new Size(100 * scale, 100 * scale);
            item.Cells.Add(cell);

            cell = new LegendCell();
            cell.CellType = LegendCellType.Text;
            cell.Text = text;
            item.Cells.Add(cell);
            return item;
        }

        private int GetSymbolScale(int value)
        {
            int scale = 1;

            if (maxActive < 4)
            {
                return value;
            }
            else if (maxActive == 4)
            {
                switch (value)
                {
                    case 1:
                        return 1;
                    case 2:
                    case 3:
                        return 2;
                    case 4:
                        return 3;
                }
            }
            else if (maxActive == 5)
            {
                switch (value)
                {
                    case 1:
                        return 1;
                    case 2:
                    case 3:
                        return 2;
                    case 4:
                    case 5:
                        return 3;
                }
            }
            else if (maxActive == 6)
            {
                switch (value)
                {
                    case 1:
                    case 2:
                        return 1;
                    case 3:
                    case 4:
                        return 2;
                    case 5:
                    case 6:
                        return 3;
                }
            }
            else
            {
                if (value < Math.Round(maxActive / 3.0))
                {
                    return 1;
                }
                if (value > Math.Round(maxActive / 3.0) * 2 + 1)
                {
                    return 3;
                }
                return 3;

            }
            return scale;
        }

        private void AddMapLayer(MapControl map, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath("../../../maps/Субъекты/ХМАО/Лесхоз_ХМАО.shp");
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "NAME", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденные формы</returns>
        public static ArrayList FindMapShape(MapControl map, string patternValue)
        {
            patternValue = patternValue.Replace("ЗП \"Малая Сосьва\"", "Малая Сосьва").Replace("Берёзовское", "Березовское");
            ArrayList shapeList = new ArrayList();

            foreach (Shape shape in map.Shapes)
            {
                if (GetShapeName(shape) == patternValue)
                {
                    shapeList.Add(shape);
                }
            }

            return shapeList;
        }

        public static string GetShapeName(Shape shape)
        {
            string shapeName = shape.Name;
            shapeName = shape.Name.Split(' ')[0];

            //shapeName = shapeName.Replace("муниципальный район", "МР");            
            return shapeName;
        }

        public void FillMapData(MapControl dundasMap, DataTable dtMap)
        {
            int dataColumnIndex = 5;
            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[dataColumnIndex] != DBNull.Value && row[dataColumnIndex].ToString() != string.Empty)
                {
                    string subject = row[0].ToString();
                    double value = Convert.ToDouble(row[dataColumnIndex]);
                    int fireCount = Convert.ToInt32(row[4]);
                    ArrayList shapeList = FindMapShape(dundasMap, subject);
                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);
                        shape["Name"] = subject;
                        shape["Complete"] = Convert.ToDouble(row[dataColumnIndex]);

                        string fireDescription = fireCount == 1 ? "пожар" : fireCount > 1 && fireCount < 5 ? "пожара" : "пожаров";

                        shape.Text = string.Format("{0}\n{1:N0} {2}\n{3:N0} га", shapeName, fireCount, fireDescription, value);
                        shape.Font = new Font(shape.Font.Name, 10);
                        shape.TextColor = Color.White;

                        Dundas.Maps.WebControl.Symbol symbol = new Dundas.Maps.WebControl.Symbol();
                        symbol.Name = shape.Name + dundasMap.Symbols.Count;
                        symbol.ParentShape = shape.Name;
                        symbol.Layer = shape.Layer;
                        symbol.Image = "../../../images/bigFire.gif";
                        symbol.Offset.X = -50;
                        int scale = GetSymbolScale(fireCount);

                        if (scale == 1)
                        {
                            symbol.Height = 14;
                            symbol.Width = 10;
                        }
                        else if (scale == 2)
                        {
                            symbol.Height = 27;
                            symbol.Width = 19;
                        }
                        else if (scale == 3)
                        {
                            symbol.Height = 40;
                            symbol.Width = 28;
                        }
                        dundasMap.Symbols.Add(symbol);
                    }
                }
            }
        }

        void WebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Hidden = true;
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().Contains("Итого"))
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 3;
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }
            else if (e.Row.Cells[0].Value.ToString().Contains(" г. на "))
            {
                e.Row.Cells[4].ColSpan = 6;
            }           
        }

        private void SetupGrid(UltraWebGrid grid, DataTable dt)
        {
            grid.Width = Unit.Empty;
            grid.Height = Unit.Empty;
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayout);

            grid.DisplayLayout.HeaderStyleDefault.BackColor = Color.Transparent;
            grid.DisplayLayout.RowStyleDefault.BackColor = Color.Transparent;
            grid.DisplayLayout.FrameStyle.BackColor = Color.Transparent;

            grid.DataSource = dt;
            grid.DataBind();
        }

        void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderClickActionDefault = HeaderClickAction.NotSet;
            e.Layout.CellClickActionDefault = CellClickAction.NotSet;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.White;

            //e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.White;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "N1");

            e.Layout.Bands[0].Columns[0].Width = 180;
            e.Layout.Bands[0].Columns[1].Width = 80;
            e.Layout.Bands[0].Columns[2].Width = 103;
            e.Layout.Bands[0].Columns[3].Width = 103;
            e.Layout.Bands[0].Columns[4].Width = 80;

            e.Layout.Bands[0].Columns[5].Width = 104;
            e.Layout.Bands[0].Columns[6].Width = 80;
            e.Layout.Bands[0].Columns[7].Width = 104;
            e.Layout.Bands[0].Columns[8].Width = 80;
            e.Layout.Bands[0].Columns[9].Width = 104;

            e.Layout.Bands[0].Columns[10].Hidden = true;

            e.Layout.Bands[0].HeaderStyle.BorderColor = Color.FromArgb(60, 60, 60);

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[5].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[6].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[7].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[8].CellStyle.BorderDetails.WidthLeft = 3;

            e.Layout.Bands[0].Columns[9].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[10].CellStyle.BorderDetails.WidthLeft = 3;

            headerLayout = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout.AddCell("Территориальный отдел – лесничество");

            GridHeaderCell cell = headerLayout.AddCell("Всего с начала года");
            cell.AddCell("кол-во");
            GridHeaderCell childCell = cell.AddCell("охваченная площадь");
            childCell.AddCell("лес (га)");
            childCell.AddCell("нелес (га)");

            cell = headerLayout.AddCell(String.Format("<table style='margin-left: 175px'><tr><td>Из них за {0}&nbsp;</td><td><a href='webcommand?showPinchReport=FW_0003_0002_Horizontal'><img src='../../../images/detail.png'></a></td></tr></table>", date.ToShortDateString()));

            childCell = cell.AddCell("действует");
            childCell.AddCell("кол-во");
            childCell.AddCell("лес (га)");

            childCell = cell.AddCell("в т.ч. локализовано");
            childCell.AddCell("кол-во");
            childCell.AddCell("лес (га)");

            childCell = cell.AddCell("ликвидировано");
            childCell.AddCell("кол-во");
            childCell.AddCell("лес (га)");

            headerLayout.ApplyHeaderInfo();
        }
    }
}
