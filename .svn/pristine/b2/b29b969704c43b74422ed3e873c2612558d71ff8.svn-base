using System;
using System.Data;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Text = Infragistics.UltraChart.Core.Primitives.Text;
using System.Drawing;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FW_0001_0001 : CustomReportPage
    {
        private DateTime reportDate;

        private DataTable dtActions = new DataTable();
        private DataTable dtEvents = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();
                
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            Title.Font.Bold = false;


            reportDate = new DateTime(2010, 6, 1);

            string query = DataProvider.GetQueryText("FW_0001_0001_actionsDescription");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Имя", dtActions);

            ConfigureActionChart(UltraChart11, 0);
            ConfigureActionChart(UltraChart12, 1);
            ConfigureActionChart(UltraChart13, 2);
            ConfigureActionChart(UltraChart14, 3);
            ConfigureActionChart(UltraChart15, 4);
            ConfigureActionChart(UltraChart16, 5);

            Label11.Text = "1,19";
            Label12.Text = "1,20";
            Label21.Text = "1,22";
            Label22.Text = "1,08";
            Label23.Text = "1,05";
            Label24.Text = "0,82";


            UltraChart11.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart11_FillSceneGraph);
            UltraChart12.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart12_FillSceneGraph);
            UltraChart13.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart13_FillSceneGraph);
            UltraChart14.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart14_FillSceneGraph);
            UltraChart15.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart15_FillSceneGraph);
            UltraChart16.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart16_FillSceneGraph);

            ConfigureActionChart(UltraChart21, 0);
            ConfigureActionChart(UltraChart22, 0);
            ConfigureActionChart(UltraChart23, 0);
            ConfigureActionChart(UltraChart24, 0);
            ConfigureActionChart(UltraChart25, 0);
            ConfigureActionChart(UltraChart26, 0);

            UltraChart21.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart21_FillSceneGraph);
            UltraChart22.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart22_FillSceneGraph);
            UltraChart23.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart23_FillSceneGraph);
            UltraChart24.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart24_FillSceneGraph);
            UltraChart25.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart25_FillSceneGraph);
            UltraChart26.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart26_FillSceneGraph);

            query = DataProvider.GetQueryText("FW_0001_0001_chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Имя", dtChart);

            UltraChart1.ChartType = ChartType.StackBarChart;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Width = 230;
            UltraChart1.Height = 100;
            UltraChart1.Legend.SpanPercentage = 31;
            UltraChart1.Legend.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Extent = 0;
            UltraChart1.Axis.X.LineColor = Color.FromArgb(192, 192, 192);
            UltraChart1.Axis.Y.LineColor = Color.FromArgb(192, 192, 192);
            UltraChart1.Axis.X2.Extent = 0;
            UltraChart1.Axis.Y.Labels.Visible = false;
            UltraChart1.Axis.Y.Extent = 35;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N3> млн.руб.";

            UltraChart1.Series.Clear();
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(2, dtChart));
            UltraChart1.Data.SwapRowsAndColumns = true;            
            //UltraChart1.DataSource = dtChart;
            UltraChart1.DataBind();
            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.Style.Add("margin-right", "-5px");
            UltraChart1.Style.Add("margin-top", "-2px");

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 2:
                        {
                            color = Color.FromArgb(110, 189, 241);
                            stopColor = Color.FromArgb(9, 135, 214);
                            break;
                        }
                    case 1:
                        {
                            color = Color.FromArgb(192, 178, 224);
                            stopColor = Color.FromArgb(44, 20, 91);
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }            

            //UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.InitializeLayout += new Infragistics.WebUI.UltraWebGrid.InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.DataBind();
        }

        private static void AddText(FillSceneGraphEventArgs e, string textValue)
        {
            Label lb = new Label();
            Text text = new Text(new Point(14, 28), textValue);
            LabelStyle style = new LabelStyle();

            style.Font = new Font("Arial", 12, FontStyle.Bold);
            //style.Font.Bold = true;
            style.FontColor = Color.White;
            text.SetLabelStyle(style);
            e.SceneGraph.Add(text);
        }

        #region номера
        void UltraChart11_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "М1");
        }

        void UltraChart12_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "М2");
        }

        void UltraChart13_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "М3");
        }

        void UltraChart14_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "М4");
        }

        void UltraChart15_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "М5");
        }

        void UltraChart16_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "М6");
        }

        void UltraChart21_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "И1");
        }

        void UltraChart22_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "И2");
        }

        void UltraChart23_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "И3");
        }

        void UltraChart24_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "И4");
        }

        void UltraChart25_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "И5");
        }

        void UltraChart26_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "И6");
        }
        #endregion

        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                if (text.GetTextString().Contains("Факт"))
                {
                    text.SetTextString("Факт");
                }
                else
                {
                    if (text.GetTextString().Contains("План"))
                    {
                        text.SetTextString("План");
                    }
                }
            }
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Box)
                {
                    Infragistics.UltraChart.Core.Primitives.Box box = (Infragistics.UltraChart.Core.Primitives.Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.DataPoint.Label == "План ")
                        {
                            box.DataPoint.Label = String.Format("Планируемое финансовое обеспечение на {0} год", box.Series.Label);
                        }
                        else
                        {
                            box.DataPoint.Label = String.Format("Фактически профинансировано с начала {0} года", box.Series.Label);
                        }
                    }
                }
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "Областной бюджет";

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "План", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Факт", "");

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
            multiHeaderPos += 2;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Внебюджетные источники";

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "План", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Факт", "");

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
            multiHeaderPos += 2;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Всего";

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "План", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Факт", "");

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            for (int i = 1; i < 7; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = 76;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }
            e.Layout.Bands[0].Columns[0].Width = 64;

        }

        void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FW_0001_0001_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Год", dtGrid);
            UltraWebGrid1.DataSource = dtGrid;
        }

        #region Мероприятия
        private void ConfigureActionChart(UltraChart chart, int chartIndex)
        {
            chart.Width = 53;
            chart.Height = 53;
            chart.ChartType = ChartType.PieChart;
            chart.Border.Thickness = 0;
            chart.Axis.Y.Extent = 5;
            chart.Axis.X.Extent = 5;
            chart.Tooltips.FormatString = "<SERIES_LABEL>";
            chart.Legend.Visible = false;
            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            chart.PieChart.Labels.Visible = false;
            chart.PieChart.Labels.LeaderLineThickness = 0;

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            chart.ColorModel.Skin.PEs.Clear();

            Color color = Color.Gray;
            Color colorEnd = Color.Gray;
            PaintElement pe = new PaintElement();
            switch (chartIndex)
            {
                case 5:
                    {
                        color = Color.Red;
                        colorEnd = Color.Red;
                        break;
                    }
                default :
                    {
                        color = Color.FromArgb(70, 118, 5);
                        colorEnd = Color.FromArgb(70, 118, 5);
                        break;
                    }
            }

            pe.Fill = color;
            pe.FillStopColor = colorEnd;
            pe.ElementType = PaintElementType.Gradient;
            pe.FillGradientStyle = GradientStyle.Horizontal;
            pe.FillOpacity = 150;
            chart.ColorModel.Skin.PEs.Add(pe);

            //chart.Style.Add("margin-top", " -10px");

            DataTable actionDataTable = new DataTable();
            actionDataTable.Columns.Add("name", typeof(string));
            actionDataTable.Columns.Add("value", typeof(double));
            object[] fictiveValue = { String.Empty, 100 };
            actionDataTable.Rows.Add(fictiveValue);
            chart.DataSource = actionDataTable;
            chart.DataBind();
        }

        private string GetActionStatus(int i)
        {
            DateTime dateStartPlan;
            DateTime dateEndPlan;
            DateTime dateStartFact;
            DateTime dateEndFact;

            DataRow row = dtActions.Rows[i];

            string status = "Ожидается";


            DateTime.TryParse(row["Срок реализации плановый окончание"].ToString(), out dateEndPlan);
            DateTime.TryParse(row["Срок реализации фактический начало"].ToString(), out dateStartPlan);

            string realisationTime = String.Format("Срок реализации: запланированный с {0:dd.MM.yyy} по {1:dd.MM.yyy}", dateStartPlan, dateEndPlan);

            // Есть факт завершения
            if (DateTime.TryParse(row["Срок реализации фактический окончание"].ToString(), out dateEndFact) &&
                dateEndFact.Year > 2008)
            {
                if (DateTime.TryParse(row["Срок реализации плановый окончание"].ToString(), out dateEndPlan))
                {
                    // и план и факт раньше текущей
                    if (reportDate > dateEndPlan &&
                        reportDate > dateEndFact)
                    {
                        status = "Состоялось в срок";
                    }
                }
            }

            // Есть факт завершения
            if (DateTime.TryParse(row["Срок реализации фактический окончание"].ToString(), out dateEndFact) &&
                dateEndFact.Year > 2008)
            {
                if (DateTime.TryParse(row["Срок реализации плановый окончание"].ToString(), out dateEndPlan))
                {
                    // и факт раньше текущей позже плана
                    if (reportDate > dateEndFact &&
                        dateEndFact > dateEndPlan)
                    {
                        status = "Состоялось позднее запланированного срока";
                    }
                }
            }

            // нет факта завершения
            if (!DateTime.TryParse(row["Срок реализации фактический окончание"].ToString(), out dateEndFact) ||
                (DateTime.TryParse(row["Срок реализации фактический окончание"].ToString(), out dateEndFact) && dateEndFact.Year < 2009))
            {
                if (DateTime.TryParse(row["Срок реализации плановый окончание"].ToString(), out dateEndPlan))
                {
                    // и план позже текущей
                    if (reportDate > dateEndPlan)
                    {
                        status = "Не состоялось";
                    }
                }

                if (DateTime.TryParse(row["Срок реализации фактический начало"].ToString(), out dateStartFact))
                {
                    // и план позже текущей
                    if (reportDate > dateStartFact)
                    {
                        status = "Мероприятие реализуется";
                    }
                }
            }
            return status;
        }

        #endregion




    }
}
