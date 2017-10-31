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
    public partial class TV_0001_0001 : CustomReportPage
    {
        private DateTime reportDate;

        private DataTable dtActions = new DataTable();
        private DataTable dtEvents = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //lbTitle.Font.Bold = false;
            Plan.ForeColor = Color.White;
            Fact.ForeColor = Color.White;
            lbExecutedFact.ForeColor = Color.White;
            lbExecutedPaln.ForeColor = Color.White;
            lbExecutedFact.Style.Add("font-size", "18px;");
            lbExecutedPaln.Style.Add("font-size", "18px;");
            
            reportDate = new DateTime(2010, 6, 1);

            string query = DataProvider.GetQueryText("TV_0001_0001_actionsDescription");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���", dtActions);

            BindExecutedAll();
            BindActionsDescription();

            ConfigureActionChart(UltraChart11, 0);
            ConfigureActionChart(UltraChart12, 1);
            ConfigureActionChart(UltraChart21, 2);
            ConfigureActionChart(UltraChart22, 3);
            ConfigureActionChart(UltraChart23, 4);
            ConfigureActionChart(UltraChart24, 5);
            ConfigureActionChart(UltraChart25, 6);
            ConfigureActionChart(UltraChart26, 7);
            ConfigureActionChart(UltraChart27, 8);
            ConfigureActionChart(UltraChart28, 9);
            ConfigureActionChart(UltraChart31, 10);
            ConfigureActionChart(UltraChart32, 11);

            Label11.Text = GetActionPercentText(0);
            Label12.Text = GetActionPercentText(1);
            Label21.Text = GetActionPercentText(2);
            Label22.Text = GetActionPercentText(3);
            Label23.Text = GetActionPercentText(4);
            Label24.Text = GetActionPercentText(5);
            Label25.Text = GetActionPercentText(6);
            Label26.Text = GetActionPercentText(7);
            Label27.Text = GetActionPercentText(8);
            Label28.Text = GetActionPercentText(9);
            Label31.Text = GetActionPercentText(10);
            Label32.Text = GetActionPercentText(11);
            
            UltraChart11.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart11_FillSceneGraph);
            UltraChart12.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart12_FillSceneGraph);
            UltraChart21.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart21_FillSceneGraph);
            UltraChart22.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart22_FillSceneGraph);
            UltraChart23.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart23_FillSceneGraph);
            UltraChart24.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart24_FillSceneGraph);
            UltraChart25.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart25_FillSceneGraph);
            UltraChart26.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart26_FillSceneGraph);
            UltraChart27.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart27_FillSceneGraph);
            UltraChart28.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart28_FillSceneGraph);
            UltraChart31.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart31_FillSceneGraph);
            UltraChart32.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart32_FillSceneGraph);

            query = DataProvider.GetQueryText("TV_0001_0001_eventsDescription");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���", dtEvents);
            BindEventsDescription();

            query = DataProvider.GetQueryText("TV_0001_0001_chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���", dtChart);

            UltraChart1.ChartType = ChartType.StackBarChart;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Width = 220;
            UltraChart1.Height = 193;
            UltraChart1.Legend.SpanPercentage = 35;
            UltraChart1.Axis.X.Extent = 0;
            UltraChart1.Axis.X.LineColor = Color.FromArgb(192, 192, 192);
            UltraChart1.Axis.Y.LineColor = Color.FromArgb(192, 192, 192);
            UltraChart1.Axis.X2.Extent = 0;
            UltraChart1.Axis.Y.Labels.Visible = false;
            UltraChart1.Axis.Y.Extent = 35;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N3> ���.���.";

            UltraChart1.Series.Clear();
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(2, dtChart));
            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;
            //UltraChart1.DataSource = dtChart;
            UltraChart1.DataBind();
            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.Style.Add("margin-right", "-5px");
            UltraChart1.Style.Add("margin-top", "-2px");

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.InitializeLayout += new Infragistics.WebUI.UltraWebGrid.InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.DataBind();
        }

        private string GetActionPercentText(int i)
        {
            string result = String.Empty;
            DataRow row = dtActions.Rows[i];

            DateTime dateStartPlan;
            DateTime dateEndPlan;
            DateTime dateStartFact;
            DateTime dateEndFact;

            if (dtActions.Rows[i]["����������� ������� ����������"].ToString() != "0")
            {
                result += String.Format("{0:N0}%<br/>", row["����������� ������� ����������"]);
            }
            
            if (Convert.ToDouble(row["����������� ������� ����������"]) < Convert.ToDouble(row["�������� ������� ����������"]))
            {
                result += "<img src=\"../../../images/bell.png\">";
            }
            if (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) &&
                dateEndFact.Year > 2008)
            {
                if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan))
                {
                    // � ���� ������ ������� ����� �����
                    if (reportDate > dateEndFact &&
                        dateEndFact > dateEndPlan)
                    {
                        result += "<img src=\"../../../images/clock_red.png\">";
                    }
                }
            }
            if (LessFinance(row, GetActionStatus(i)) != String.Empty)
            {
                result += "<img src=\"../../../images/money.png\">";
            }
            return result;
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

        #region ������
        void UltraChart11_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "1.1");
        }

        void UltraChart12_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "1.2");
        }

        void UltraChart21_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "2.1");
        }

        void UltraChart22_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "2.2");
        }

        void UltraChart23_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "2.3");
        }

        void UltraChart24_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "2.4");
        }

        void UltraChart25_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "2.5");
        }

        void UltraChart26_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "2.6");
        }

        void UltraChart27_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "2.7");
        }

        void UltraChart28_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "2.8");
        }

        void UltraChart31_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "3.1");
        }

        void UltraChart32_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "3.2");
        }
        #endregion

        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                if (text.GetTextString().Contains("����"))
                {
                    text.SetTextString("����");
                }
                else
                {
                    if (text.GetTextString().Contains("����"))
                    {
                        text.SetTextString("����");
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
                        if (box.DataPoint.Label == "���� ")
                        {
                            box.DataPoint.Label = String.Format("����������� ���������� ����������� �� {0} ���", box.Series.Label);
                        }
                        else
                        {
                            box.DataPoint.Label = String.Format("���������� ���������������� � ������ {0} ����", box.Series.Label);
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
            ch.Caption = "����������� ������";

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "����", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "����", "");

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
            multiHeaderPos += 2;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "������������ ���������";

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "����", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "����", "");

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
            multiHeaderPos += 2;
            ch.RowLayoutColumnInfo.SpanX = 2;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "�����";

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "����", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "����", "");

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
            string query = DataProvider.GetQueryText("TV_0001_0001_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���", dtGrid);
            UltraWebGrid1.DataSource = dtGrid;
        }

        #region �����������
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
            //chart.PieChart.RadiusFactor = 110;
           // chart.Style.Add("padding", "0px");
          //  chart.Style.Add("margin", "-3px");
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                Color color = Color.Gray;
                Color colorEnd = Color.Gray;
                PaintElement pe = new PaintElement();
                switch (GetActionStatus(chartIndex))
                {
                    case "�� ����������":
                        {
                            color = Color.Red;
                            colorEnd = Color.Red;
                            break;
                        }
                    case "���������� � ����":
                        {
                            color = Color.FromArgb(70, 118, 5);
                            colorEnd = Color.FromArgb(70, 118, 5);
                            break;
                        }
                    case "���������� ������� ���������������� �����":
                        {
                            color = Color.FromArgb(70, 118, 5);
                            colorEnd = Color.FromArgb(70, 118, 5);
                            break;
                        }
                    case "���������":
                        {
                            color = Color.Gray;
                            colorEnd = Color.Gray;
                            break;
                        }
                    case "����������� �����������":
                        {
                            color = Color.Orange;
                            colorEnd = Color.Orange;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = colorEnd;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.Horizontal;
                pe.FillOpacity = 150;
                chart.ColorModel.Skin.PEs.Add(pe);
            }
            //chart.Style.Add("margin-top", " -10px");

            DataTable actionDataTable = new DataTable();
            actionDataTable.Columns.Add("name", typeof(string));
            actionDataTable.Columns.Add("value", typeof(double));
            object[] fictiveValue = { GetActionHint(chartIndex), 100 };
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

            string status = "���������";
            

            DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan);
            DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartPlan);

            string realisationTime = String.Format("���� ����������: ��������������� � {0:dd.MM.yyy} �� {1:dd.MM.yyy}", dateStartPlan, dateEndPlan);

            // ���� ���� ����������
            if (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) &&
                dateEndFact.Year > 2008)
            {
                if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan))
                {
                    // � ���� � ���� ������ �������
                    if (reportDate > dateEndPlan &&
                        reportDate > dateEndFact)
                    {
                        status = "���������� � ����";
                    }
                }
            }

            // ���� ���� ����������
            if (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) &&
                dateEndFact.Year > 2008)
            {
                if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan))
                {
                    // � ���� ������ ������� ����� �����
                    if (reportDate > dateEndFact &&
                        dateEndFact > dateEndPlan)
                    {
                        status = "���������� ������� ���������������� �����";
                    }
                }
            }

            // ��� ����� ����������
            if (!DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) ||
                (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) && dateEndFact.Year < 2009))
            {
                if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan))
                {
                    // � ���� ����� �������
                    if (reportDate > dateEndPlan)
                    {
                        status = "�� ����������";
                    }
                }

                if (DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartFact))
                {
                    // � ���� ����� �������
                    if (reportDate > dateStartFact)
                    {
                        status = "����������� �����������";
                    }
                }
            }
            return status;
        }

        private bool ActionEnded(int i)
        {
            DateTime dateFactEnd;
            return DateTime.TryParse(dtActions.Rows[i]["���� ���������� ����������� ���������"].ToString(), out dateFactEnd) &&
                   dateFactEnd.Year > 2008;
        }

        //private bool StartFactDateGreaterCurDate(int i)
        //{
        //    DateTime dateFactEnd;
        //    return dtActions.Rows[i]["���� ���������� ����������� ������"] != DBNull.Value &&
        //           DateTime.TryParse(dtActions.Rows[i]["���� ���������� ����������� ������"].ToString(), out dateFactEnd) &&
        //           dateFactEnd.Year > 2008 &&
        //           dateFactEnd > reportDate;
        //}

        private bool EndPlanDateGreaterCurDate(int i)
        {
            DateTime datePlanEnd;
            return dtActions.Rows[i]["���� ���������� �������� ���������"] != DBNull.Value &&
                   DateTime.TryParse(dtActions.Rows[i]["���� ���������� �������� ���������"].ToString(), out datePlanEnd) &&
                   (datePlanEnd > reportDate);
        }
                
        private bool EndPlanDateGreaterEndFactDate(int i)
        {
            DateTime dateFactEnd;
            DateTime datePlanEnd;
            return dtActions.Rows[i]["���� ���������� �������� ���������"] != DBNull.Value &&
                   DateTime.TryParse(dtActions.Rows[i]["���� ���������� �������� ���������"].ToString(), out datePlanEnd) &&
                   dtActions.Rows[i]["���� ���������� ����������� ���������"] != DBNull.Value &&
                   DateTime.TryParse(dtActions.Rows[i]["���� ���������� ����������� ���������"].ToString(), out dateFactEnd) &&
                   dateFactEnd.Year > 2008 &&
                   datePlanEnd > dateFactEnd;
        }

        private bool StartPlanDateGreaterCurDate(int i)
        {
            DateTime datePlanEnd;
            return dtActions.Rows[i]["���� ���������� �������� ������"] != DBNull.Value &&
                   DateTime.TryParse(dtActions.Rows[i]["���� ���������� �������� ������"].ToString(), out datePlanEnd) &&
                   datePlanEnd > reportDate;
        }

        private string GetActionHint(int i)
        {
            #region ����
            //string name = dtActions.Rows[i]["���"].ToString().Replace("'", "'");
            //if (name.Length > 40)
            //{
            //    int k = 0;
            //    for (int j = 0; j < name.Length; j++)
            //    {
            //        k++;
            //        if (k > 40 && name[j] == ' ')
            //        {
            //            name = name.Insert(j, "<br/>");
            //            k = 0;
            //        }
            //    }
            //}

            //string executor = dtActions.Rows[i]["�����������"].ToString().Replace("'", "'");
            //if (executor.Length > 40)
            //{
            //    int k = 0;
            //    for (int j = 0; j < executor.Length; j++)
            //    {
            //        k++;
            //        if (k > 37 && executor[j] == ' ')
            //        {
            //            executor = executor.Insert(j, "<br/>");
            //            k = 0;
            //        }
            //    }
            //}

            //string realisationTime = "<b>���� ����������</b><br/>";
            //DateTime dateEnd;
            //DateTime dateStart;

            //if (dtActions.Rows[i]["���� ���������� ����������� ������"] != DBNull.Value &&
            //    dtActions.Rows[i]["���� ���������� ����������� ���������"] != DBNull.Value &&
            //    DateTime.TryParse(dtActions.Rows[i]["���� ���������� ����������� ������"].ToString(), out dateStart) &&
            //    DateTime.TryParse(dtActions.Rows[i]["���� ���������� ����������� ���������"].ToString(), out dateEnd) &&
            //    dateEnd.Year > 2008)
            //{
            //    realisationTime = String.Format("<b>���� ����������</b>&nbsp;c {0:dd.MM.yyyy} �� {1:dd.MM.yyyy}<br/>", dateStart, dateEnd);
            //    if (GetActionStatus(i) == "���������� ������� ���������������� �����")
            //    {
            //        DateTime dateEndPlan;
            //        DateTime dateStartPlan;
            //        DateTime.TryParse(dtActions.Rows[i]["���� ���������� �������� ������"].ToString(), out dateStartPlan);
            //        DateTime.TryParse(dtActions.Rows[i]["���� ���������� �������� ���������"].ToString(), out dateEndPlan);
            //        realisationTime = String.Format("<b>���� ����������</b><br/>&nbsp;&nbsp;&nbsp;����������� c {0:dd.MM.yyyy} �� {1:dd.MM.yyyy}<br/>&nbsp;&nbsp;&nbsp;�������� c {2:dd.MM.yyyy} �� {3:dd.MM.yyyy}<br/>", dateStart, dateEnd, dateStartPlan, dateEndPlan);
            //    }
            //}
            //else
            //{
            //    if (dtActions.Rows[i]["���� ���������� �������� ������"] != DBNull.Value &&
            //       dtActions.Rows[i]["���� ���������� �������� ���������"] != DBNull.Value &&
            //       DateTime.TryParse(dtActions.Rows[i]["���� ���������� �������� ������"].ToString(), out dateStart) &&
            //       DateTime.TryParse(dtActions.Rows[i]["���� ���������� �������� ���������"].ToString(), out dateEnd))
            //    {
            //        realisationTime +=
            //            String.Format("&nbsp;&nbsp;&nbsp;�������� c {0:dd.MM.yyyy} �� {1:dd.MM.yyyy}<br/>", dateStart, dateEnd);
            //    }
            //    if (dtActions.Rows[i]["���� ���������� ��������� ������"] != DBNull.Value &&
            //       dtActions.Rows[i]["���� ���������� ��������� ���������"] != DBNull.Value &&
            //       DateTime.TryParse(dtActions.Rows[i]["���� ���������� ��������� ������"].ToString(), out dateStart) &&
            //       DateTime.TryParse(dtActions.Rows[i]["���� ���������� ��������� ���������"].ToString(), out dateEnd))
            //    {
            //        realisationTime +=
            //            String.Format("&nbsp;&nbsp;&nbsp;��������� c {0:dd.MM.yyyy} �� {1:dd.MM.yyyy}<br/>", dateStart, dateEnd);
            //    }
            //}

            //string status = String.Format("<b>������:</b> {0}", GetActionStatus(i));
            //string imp�rtantly = "������";
            //switch (dtActions.Rows[i]["������ ������� �������"].ToString())
            //{
            //    case "1":
            //        {
            //            imp�rtantly = "������";
            //            break;
            //        }
            //    case "2":
            //        {
            //            imp�rtantly = "�������";
            //            break;
            //        }
            //    case "3":
            //        {
            //            imp�rtantly = "�������";
            //            break;
            //        }
            //}

            //string executedPercent = String.Empty;

            //if (dtActions.Rows[i]["����������� ������� ����������"].ToString() != "0" ||
            //    dtActions.Rows[i]["�������� ������� ����������"].ToString() != "0")
            //{
            //    executedPercent = "<b>������� ����������:</b>&nbsp;";
            //    if (dtActions.Rows[i]["����������� ������� ����������"].ToString() != "0")
            //    {
            //        executedPercent = String.Format("{0}���� {1:N0}%", executedPercent, dtActions.Rows[i]["����������� ������� ����������"]);
            //    }
            //    if (dtActions.Rows[i]["����������� ������� ����������"].ToString() != "100")
            //    {
            //        executedPercent = String.Format("{0}&nbsp;(���� {1:N0}%)", executedPercent, dtActions.Rows[i]["�������� ������� ����������"]);
            //    }
            //}
            //return String.Format("{0}<br/>{1}<br/>{2}{3}<br/><b>�����������:</b> {4}<br/><b>��������:</b> {5}<br/>{6}", dtActions.Rows[i]["�����������"].ToString().Replace("�����������", "<b>�����������</b>"), name, realisationTime, status, executor, imp�rtantly, executedPercent);
            #endregion

            DateTime dateStartPlan;
            DateTime dateEndPlan;
            DateTime dateStartFact;
            DateTime dateEndFact;

            DataRow row = dtActions.Rows[i];
            string status = "���������";
            string indication = "<img src='../../../images/date.png'>&nbsp;";

            DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan);
            DateTime.TryParse(row["���� ���������� �������� ������"].ToString(), out dateStartPlan);

            string realisationTime = String.Format("<span style='color: black;'><b>���� ����������:</b></span>&nbsp;<br/>�������� � {0:dd.MM.yyy} �� {1:dd.MM.yyy}", dateStartPlan, dateEndPlan);

            // ���� ���� ����������
            if (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) &&
                dateEndFact.Year > 2008)
            {
                if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan))
                {
                    // � ���� � ���� ������ �������
                    if (reportDate > dateEndPlan &&
                        reportDate > dateEndFact)
                    {
                        indication = "<img src='../../../images/accept.png'>&nbsp;";
                        status = "���������� � ����";

                        DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartFact);
                        realisationTime = String.Format("<span style='color: black;'><b>���� ����������:</b></span><br/>� {0:dd.MM.yyy} �� {1:dd.MM.yyy}", dateStartFact, dateEndFact);
                    }
                }
            }

            // ���� ���� ����������
            if (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) &&
                dateEndFact.Year > 2008)
            {
                if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan))
                {
                    // � ���� ������ ������� ����� �����
                    if (reportDate > dateEndFact &&
                        dateEndFact > dateEndPlan)
                    {
                        indication = "<img src='../../../images/accept.png'>&nbsp;";
                        status = "���������� ������� ���������������� �����";

                        DateTime.TryParse(row["���� ���������� �������� ������"].ToString(), out dateStartPlan);
                        DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan);
                        DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartFact);

                        realisationTime = String.Format("<span style='color: black;'><b>���� ����������:</b></span>&nbsp;<span style='color: red;'>�������&nbsp;</span><img src='../../../images/clock_red.png'><br/>����������� c {0:dd.MM.yyyy} �� {1:dd.MM.yyyy}<br/>��������������� c {2:dd.MM.yyyy} �� {3:dd.MM.yyyy}", dateStartFact, dateEndFact, dateStartPlan, dateEndPlan);

                    }
                }
            }

            // ��� ����� ����������
            if (!DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) ||
                (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEndFact) && dateEndFact.Year < 2009))
            {
                if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEndPlan))
                {
                    // � ���� ����� �������
                    if (reportDate > dateEndPlan)
                    {
                        indication = "<img src='../../../images/cancel.png'>&nbsp;";
                        status = "�� ����������";

                        DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartPlan);
                        realisationTime = String.Format("<span style='color: black;'><b>���� ����������:</b></span><br/>��������������� � {0:dd.MM.yyy} �� {1:dd.MM.yyy}", dateStartPlan, dateEndPlan);
                    }
                }

                if (DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartFact))
                {
                    // � ���� ����� �������
                    if (reportDate > dateStartFact)
                    {
                        indication = "<img src='../../../images/inProc.png'>&nbsp;";
                        status = "����������� �����������";
                        DateTime.TryParse(row["���� ���������� ����������� ������"].ToString(), out dateStartPlan);
                        realisationTime = String.Format("<span style='color: black;'><b>���� ����������:</b></span><br/>��������������� � {0:dd.MM.yyy} �� {1:dd.MM.yyy}<br/>����������� � {2:dd.MM.yyy}", dateStartPlan, dateEndPlan, dateStartFact);
                    }
                }
            }

            string financing = "���";
            if (!ZeroFinancing(row))
            {
                financing = GetFinancing(row);
            }

            return String.Format("{7}<span style='color: black;'><b>{0}</b></span>&nbsp;<span style='color: black;'><b>{1}</b></span><br/><span style='color: black'><b>�����������:</b></span>&nbsp;{2} <span style='color: black'><b><br/>������:</b></span>&nbsp;{8}<br/>{3}<br/><span style='color: black'><b>% ����������:</b></span>&nbsp;{9}���� {4:N0}% (���� {5:N0}%)<br/><b>��������������, ���.���.:&nbsp;</b>{10}&nbsp;{6}",
                row["�����������"], GetWarpedHint(row["���"].ToString()), GetWarpedHint(row["�����������"].ToString()), realisationTime, row["����������� ������� ����������"], row["�������� ������� ����������"],
                financing, indication, status.ToLower(), Convert.ToDouble(row["����������� ������� ����������"]) < Convert.ToDouble(row["�������� ������� ����������"]) ? "<span style='color: red;'><b>�������������</b>&nbsp;</span><img src='../../../images/bell.png'>&nbsp;" : "", LessFinance(row, status));
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

        private void BindActionsDescription()
        {
            //for (int i = 0; i < dtActions.Rows.Count; i++)
            //{
            //    DateTime dateEnd;
            //    DateTime dateStart;

            //    // �� ����� ����, ���� ���� ����������� ���� ���������, ��� ������ �� ������ �������.
            //    if (!EndFactDateGreaterCurDate(i))
            //    {
            //        endedActionFact++;
            //    }
            //    // �� ����� ����, ���� ���� ����������� ���� ������, ��� ������ �� ������ �������.
            //    // ����� ��������� ���� ���������, ��� ��������� ����.
            //    else if (StartFactDateGreaterCurDate(i))
            //    {
            //        inProcActionFact++;
            //    }

            //    if (EndPlanDateGreaterCurDate(i))
            //    {
            //        endedActionPlan++;
            //    }

                

            //    if (DateTime.TryParse(dtActions.Rows[i]["���� ���������� �������� ������"].ToString(), out dateStart) &&
            //        DateTime.TryParse(dtActions.Rows[i]["���� ���������� �������� ���������"].ToString(), out dateEnd))
            //    {
            //        if ((dateEnd > reportDate) && (dateStart < reportDate))
            //        {
            //            inProcActionPlan++;
            //        }
            //    }

            //    if (DateTime.TryParse(dtActions.Rows[i]["���� ���������� ����������� ������"].ToString(), out dateStart))
            //    {
            //        if (dateStart < reportDate)
            //        {
            //            ramedActionFact++;
            //        }
            //    }

            //    if (DateTime.TryParse(dtActions.Rows[i]["���� ���������� �������� ������"].ToString(), out dateStart))
            //    {
            //        if (dateEnd < reportDate)
            //        {
            //            ramedActionPlan++;
            //        }
            //    }
            //}

            //lbActionsValues.Text = String.Format("���������: {0} ({1}); � �������� ���������� {2} ({3}); �� ����������� {4} ({5})",
            //                                     endedActionFact, endedActionPlan, inProcActionFact, inProcActionPlan, ramedActionFact, ramedActionPlan);
        }

        private bool StartFactDateGreaterCurDate(int i)
        {
            DateTime dateStart;
            return DateTime.TryParse(dtActions.Rows[i]["���� ���������� ����������� ������"].ToString(), out dateStart) &&
                   dateStart > reportDate;
        }

        private bool EndFactDateGreaterCurDate(int i)
        {
            DateTime dateEnd;
            return DateTime.TryParse(dtActions.Rows[i]["���� ���������� ����������� ���������"].ToString(), out dateEnd) &&
                   dateEnd.Year > 2008 && 
                   dateEnd > reportDate;
        }


        private void BindExecutedAll()
        {
            string query = DataProvider.GetQueryText("TV_0001_0001_executedAll");
            DataTable dtEcecutedAll = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtEcecutedAll);
            lbExecutedFact.Text = String.Format(" {0:N0}%", dtEcecutedAll.Rows[0][1]);
            lbExecutedPaln.Text = String.Format(" {0:N0}%", dtEcecutedAll.Rows[0][2]);
           // GaugePlaceHolder.Controls.Add(GetGauge(Convert.ToDouble(dtEcecutedAll.Rows[0][1]), Convert.ToDouble(dtEcecutedAll.Rows[0][2])));
            SetUpGaudes((LinearGauge)ugNoTarget.Gauges[0], Convert.ToDouble(dtEcecutedAll.Rows[0][1]));
            SetUpGaudes((LinearGauge)UltraGauge1.Gauges[0], Convert.ToDouble(dtEcecutedAll.Rows[0][2]));
        }
        #endregion

        #region �������� �������

        private void BindEventsDescription()
        {
            //foreach (DataRow row in dtActions.Rows)
            //{
            //    DateTime dateEnd;
            //    DateTime dateStart;
            //    if (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEnd) &&
            //        dateEnd.Year > 2008)
            //    {
            //        if (dateEnd.Year == reportDate.Year)
            //        {
            //            startYearEventFact++;
            //        }
            //    }
            //    if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEnd))
            //    {
            //        if (dateEnd.Year == reportDate.Year)
            //        {
            //            startYearEventPlan++;
            //        }
            //    }
            //    if (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEnd) &&
            //        dateEnd.Year > 2008)
            //    {
            //        if (dateEnd.Year >= 2009)
            //        {
            //            startProjectEventFact++;
            //        }
            //    }
            //    if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEnd))
            //    {
            //        if (dateEnd.Year == reportDate.Year)
            //        {
            //            startProjectEventPlan++;
            //        }
            //    }
            //    if (DateTime.TryParse(row["���� ���������� ����������� ���������"].ToString(), out dateEnd) &&
            //        dateEnd.Year > 2008)
            //    {
            //        if (dateEnd.Year == reportDate.Year &&
            //            dateEnd.Month == reportDate.AddMonths(-1).Month)
            //        {
            //            thisMonthEventFact++;
            //        }
            //    }
            //    if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEnd))
            //    {
            //        if (dateEnd.Year == reportDate.Year &&
            //            dateEnd.Month == reportDate.Month)
            //        {
            //            thisMonthEventPlan++;
            //        }
            //    }
            //    if (DateTime.TryParse(row["���� ���������� �������� ���������"].ToString(), out dateEnd))
            //    {
            //        if (dateEnd.Year == reportDate.AddMonths(1).Year &&
            //            dateEnd.Month == reportDate.AddMonths(1).Month)
            //        {
            //            nextMonthEventPlan++;
            //        }
            //    }
                
            //}

            //ramedEventFact = startProjectEventFact - startProjectEventPlan;

            //KeyEventsFromStartYear.Text = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;� ������ 2010 ���� {0} ({1}); � ������ ������� {2} ({3})<br/>�� ���<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;���������� � ���� 2010 ���� {4} ({5}); �� ���������� {6}<br/>���������� �������� �������, ��������� � ���� 2010 ����: {7}", 
            //            startYearEventFact, startYearEventPlan, startProjectEventFact, startProjectEventPlan, thisMonthEventFact, thisMonthEventPlan, ramedEventFact, nextMonthEventPlan);
        }
        #endregion

        #region �����

        private static UltraGauge GetGauge(double markerValueFact, double markerValuePlan)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = 300;
            gauge.Height = 40;
            gauge.DeploymentScenario.FilePath = "../../../TemporaryImages";
            gauge.DeploymentScenario.ImageURL = "../../../TemporaryImages/gauge_imfrf02_01_#SEQNUM(100).png";

            // ����������� �����
            LinearGauge linearGauge = new LinearGauge();
            linearGauge.CornerExtent = 10;
            linearGauge.MarginString = "1, 10, 1, 10, Pixels";

            // �������� �������� ����� 
            double endValue = (Math.Max(100, markerValueFact));
            endValue = (Math.Max(endValue, markerValuePlan));

            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = 98;
            scale.StartExtent = 1;
            scale.OuterExtent = 98;
            scale.InnerExtent = 52;

            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(0, 255, 255, 255);
            gradientBrush.StartColor = Color.FromArgb(120, 255, 255, 255);
            scale.BrushElements.Add(gradientBrush);
            linearGauge.Scales.Add(scale);
            AddMainScale(endValue, linearGauge, markerValueFact, markerValuePlan);
            AddMajorTickmarkScale(endValue, linearGauge);
            AddGradient(linearGauge);

            linearGauge.Margin.Top = 1;
            linearGauge.Margin.Bottom = 1;

            gauge.Gauges.Add(linearGauge);
            return gauge;
        }

        private const int ScaleStartExtent = 5;
        private const int ScaleEndExtent = 97;

        private static void AddMajorTickmarkScale(double endValue, LinearGauge linearGauge)
        {
            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.MajorTickmarks.EndWidth = 2;
            scale.MajorTickmarks.StartWidth = 2;
            scale.MajorTickmarks.EndExtent = 40;
            scale.MajorTickmarks.StartExtent = 25;
            scale.MajorTickmarks.BrushElements.Add(new SolidFillBrushElement(Color.White));
            scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 10));
            linearGauge.Scales.Add(scale);
        }

        private static void AddGradient(LinearGauge linearGauge)
        {
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(150, 150, 150);
            gradientBrush.StartColor = Color.FromArgb(10, 255, 255, 255);
            gradientBrush.GradientStyle = Gradient.BackwardDiagonal;
            linearGauge.BrushElements.Add(gradientBrush);
        }

        private static void AddMainScale(double endValue, LinearGauge linearGauge, double markerValueFact, double markerValuePlan)
        {
            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 5));

            AddMainScaleRangeFact(scale, markerValueFact);
            AddMainScaleRangePlan(scale, markerValuePlan);
            SetMainScaleLabels(scale);
            linearGauge.Scales.Add(scale);
        }

        private static void SetMainScaleLabels(LinearGaugeScale scale)
        {

            scale.Labels.ZPosition = LinearTickmarkZPosition.AboveMarkers;
            scale.Labels.Extent = 20;
            Font font = new Font("Arial", 9);
            scale.Labels.Font = font;
            scale.Labels.EqualFontScaling = false;
            SolidFillBrushElement solidFillBrushElement = new SolidFillBrushElement(Color.White);
            solidFillBrushElement.RelativeBoundsMeasure = Measure.Percent;
            Rectangle rect = new Rectangle(0, 0, 80, 0);
            solidFillBrushElement.RelativeBounds = rect;
            scale.Labels.BrushElements.Add(solidFillBrushElement);
            scale.Labels.Shadow.Depth = 2;
            scale.Labels.Shadow.BrushElements.Add(new SolidFillBrushElement());
        }

        private static void AddMainScaleRangeFact(LinearGaugeScale scale, double markerValue)
        {
            LinearGaugeRange range = new LinearGaugeRange();
            range.EndValue = markerValue;
            range.StartValue = 0;
            range.OuterExtent = 45;
            range.InnerExtent = 10;
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(1, 51, 75);
            gradientBrush.StartColor = Color.FromArgb(8, 218, 164);
            gradientBrush.GradientStyle = Gradient.Vertical;
            range.BrushElements.Add(gradientBrush);
            scale.Ranges.Add(range);
        }

        private static void AddMainScaleRangePlan(LinearGaugeScale scale, double markerValue)
        {
            LinearGaugeRange range = new LinearGaugeRange();
            range.EndValue = markerValue;
            range.StartValue = 0;
            range.OuterExtent = 90;
            range.InnerExtent = 55;
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(1, 12, 150);
            gradientBrush.StartColor = Color.FromArgb(8, 146, 218);
            gradientBrush.GradientStyle = Gradient.Vertical;
            range.BrushElements.Add(gradientBrush);
            scale.Ranges.Add(range);
        }

        #endregion

        private static string GetFinancing(DataRow row)
        {
            return
                String.Format(
                    "<br/>�����: ����&nbsp;<span style='color: black;'><b>{0:N2}</b>&nbsp;</span>  (����&nbsp;<span style='color: black;'><b>{1:N2}</b></span>)<br/>�� ������-���� 2010 ����: ����&nbsp;<span style='color: black;'><b>{2:N2}</b>&nbsp;</span>  (����&nbsp;<span style='color: black;'><b>{3:N2}</b></span>)<br/>�� ���� 2010 ����: ����&nbsp;<span style='color: black;'><b>{4:N2}</b>&nbsp;</span>  (����&nbsp;<span style='color: black;'><b>{5:N2}</b></span>)",
                    row["�������������� ���� �����"], row["�������������� ���� �����"],
                    row["�������������� ���� ������� ���"], row["�������������� ���� ������� ���"],
                    row["�������������� ���� ������� �����"], row["�������������� ���� ������� �����"]);

        }

        private static string LessFinance(DataRow row, string status)
        {
            bool less = (status != "���������" &&
                         Convert.ToDouble(row["�������������� ���� ������� ���"].ToString()) <
                         Convert.ToDouble(row["�������������� ���� ������� ���"].ToString()));
            return less ? String.Format("<span style='color: red;'><b>�������������</b>&nbsp;</span><img src='../../../images/money.png'>") : String.Empty;
        }

        private static bool ZeroFinancing(DataRow row)
        {
            return (Convert.ToDouble(row["�������������� ���� �����"].ToString()) == 0 &&
                    Convert.ToDouble(row["�������������� ���� �����"].ToString()) == 0 &&
                    Convert.ToDouble(row["�������������� ���� ������� ���"].ToString()) == 0 &&
                    Convert.ToDouble(row["�������������� ���� ������� ���"].ToString()) == 0 &&
                    Convert.ToDouble(row["�������������� ���� ������� �����"].ToString()) == 0 &&
                    Convert.ToDouble(row["�������������� ���� ������� �����"].ToString()) == 0);
        }

        private void SetUpGaudes(LinearGauge gaude, double currentValue)
        {
            gaude.Scales[0].Markers[0].Value = currentValue;
            double topValue = 100;
            gaude.Scales[0].Axes[0].SetEndValue(topValue);
            gaude.Scales[0].Labels.Frequency = topValue / 4;
            gaude.Scales[0].Labels.Font = new Font("Verdana", 8);
            LinearGaugeNeedle needle = new LinearGaugeNeedle();
            needle.StartExtent = 2;
            needle.MidExtent = 5;
            needle.EndExtent = 9;
            needle.StartWidth = 9;
            needle.MidWidth = 9;
            needle.EndWidth = 0;
            //needle.Value = avgValue / 1000;
            SolidFillBrushElement fill = new SolidFillBrushElement();
            fill.Color = Color.Gray;
            //            fill.Color = Color.White;
            needle.BrushElements.Add(fill);

            gaude.Scales[0].Markers.Add(needle);

            needle = new LinearGaugeNeedle();
            needle.StartExtent = 2;
            needle.MidExtent = 5;
            needle.EndExtent = 9;
            needle.StartWidth = 9;
            needle.MidWidth = 9;
            needle.EndWidth = 0;
            //needle.Value = minValue / 1000;
            fill = new SolidFillBrushElement();
            fill.Color = Color.Gray;
            //            fill.Color = Color.White;
            needle.BrushElements.Add(fill);

            gaude.Scales[0].Markers.Add(needle);
            //if (rest > 0)
            //{
            //    needle = new LinearGaugeNeedle();
            //    needle.StartExtent = 10;
            //    needle.MidExtent = 30;
            //    needle.EndExtent = 50;
            //    needle.StartWidth = 2;
            //    needle.EndWidth = 2;
            //    needle.MidWidth = 2;
            //    needle.Value = rest / 1000;
            //    HatchBrushElement hatch = new HatchBrushElement();
            //    hatch.HatchStyle = HatchStyle.DarkVertical;
            //    hatch.ForeColor = Color.Gray;
            //    //                hatch.ForeColor = Color.White;
            //    needle.BrushElements.Add(hatch);
            //    gaude.Scales[0].Markers.Add(needle);
            //}
            //else
            //{
            //    lbRest.ForeColor = Color.Red;
            //}

            //Infragistics.UltraGauge.Resources.BoxAnnotation ba = new Infragistics.UltraGauge.Resources.BoxAnnotation();
            //ba.Bounds = new Rectangle(5, (int)gaude.Scales[0].Axes[0].Map(avgValue / 1000, 135, 11), 15, 10);
            //ba.Label.FormatString = "����";
            //ba.Label.Font = new Font("Arial", 8);
            //ba.Label.BrushElements.Add(new SolidFillBrushElement(Color.Gray));
            ////            ba.Label.BrushElements.Add(new SolidFillBrushElement(Color.White));
            //gaude.GaugeComponent.Annotations.Add(ba);

            //ba = new BoxAnnotation();
            //ba.Bounds = new Rectangle(7, (int)gaude.Scales[0].Axes[0].Map(minValue / 1000, 135, 11), 15, 10);
            //ba.Label.FormatString = "���";
            //ba.Label.Font = new Font("Arial", 8);
            //ba.Label.BrushElements.Add(new SolidFillBrushElement(Color.Gray));
            ////            ba.Label.BrushElements.Add(new SolidFillBrushElement(Color.White));
            //gaude.GaugeComponent.Annotations.Add(ba);
        }

    }
}
