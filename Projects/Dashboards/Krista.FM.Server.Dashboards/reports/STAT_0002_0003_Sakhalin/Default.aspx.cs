using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0003_Sakhalin
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;

        private DataTable dtComments;
        
        private CustomParam Faces;
        private CustomParam Units;
        private CustomParam LastDate;
        private CustomParam periodYear;

        private static MemberAttributesDigest regionsDigest;

        private GridHeaderLayout headerLayout;
        /// <summary>
        /// Выбраны ли 
        /// федеральные округа
        /// </summary>
        /// 

        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int minScreenWidth
        {
            get { return IsSmallResolution ? 850 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        public bool AllFO
        {
            get { return regionsCombo.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            base.Page_PreLoad(sender, e);
            //CRHelper.SaveToUserAgentLog(String.Format("Базовый прелоад {0}", Environment.TickCount - start));

            start = Environment.TickCount;
            if (Faces == null)
            {
                Faces = UserParams.CustomParam("Faces");
            }

            if (Units == null)
            {
                Units = UserParams.CustomParam("Units");
            }

            if (LastDate == null)
            {
                LastDate = UserParams.CustomParam("LastDate");
            }
            if (periodYear == null)
            {
                periodYear = UserParams.CustomParam("periodYear");
            }
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 500);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.305);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
            UltraChart.ChartType = ChartType.StackColumnChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.Axis.X.Extent = 70;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 13;
            UltraChart.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart.Axis.X.Margin.Near.Value = 2;
            UltraChart.Axis.Y.Margin.Near.Value = 2;
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "млн.руб.";
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.Transform3D.XRotation = -245;
            UltraChart.Transform3D.YRotation = 0;
            UltraChart.Transform3D.ZRotation = 0;
            EmptyAppearance item = new EmptyAppearance();
            item.EnableLineStyle = true;
            item.EnablePoint = false;
            LineStyle style = new LineStyle();
            style.DrawStyle = LineDrawStyle.Dash;
            style.MidPointAnchors = false;
            item.LineStyle = style;
            UltraChart.LineChart.EmptyStyles.Add(item);
            UltraChart.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart_ChartDrawItem);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear; 

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);            
            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N1> млн. руб.";
            UltraChart1.Axis.X.Extent = 70;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 16;
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Margin.Near.Value = 2;
            UltraChart1.Axis.Y.Margin.Near.Value = 2;
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "млн.руб.";
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart1.Transform3D.XRotation = -245;
            UltraChart1.Transform3D.YRotation = 0;
            UltraChart1.Transform3D.ZRotation = 0;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 9; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            color = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            color = Color.Red;
                            break;
                        }
                    case 6:
                        {
                            color = Color.Blue;
                            break;
                        }
                    case 7:
                        {
                            color = Color.DarkViolet;
                            break;
                        }
                    case 8:
                        {
                            color = Color.OrangeRed;
                            break;
                        }
                    case 9:
                        {
                            color = Color.CornflowerBlue;
                            break;
                        }
                }
                pe.Fill = color;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin; 
            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 760);
            UltraChart2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.41);
            UltraChart2.ChartType = ChartType.DoughnutChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N2> млн. руб.\n<PERCENT_VALUE:N2>%";
            UltraChart2.ChartDrawItem +=new ChartDrawItemEventHandler(UltraChart2_ChartDrawItem);
            UltraChart2.Axis.X.Extent = 70;
            UltraChart2.Axis.X.Labels.Visible = true;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P2>";
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Legend.SpanPercentage = 20;
            UltraChart2.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart2.Axis.X.Margin.Near.Value = 2;
            UltraChart2.Axis.Y.Margin.Near.Value = 2;
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            CRHelper.CopyCustomColorModel(UltraChart, UltraChart2); 
            item.EnableLineStyle = true;
            item.EnablePoint = false;
            style.DrawStyle = LineDrawStyle.Dash;
            style.MidPointAnchors = false;
            item.LineStyle = style;
            UltraChart2.LineChart.EmptyStyles.Add(item);
            UltraChart3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart3.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50);
            UltraChart3.ChartType = ChartType.StackColumnChart;
            UltraChart3.Border.Thickness = 0;

            UltraChart3.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:N1> млн. руб.";
            UltraChart3.Axis.X.Extent = 70;
            UltraChart3.Axis.X.Labels.Visible = true;
            UltraChart3.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart3.Legend.Visible = true;
            UltraChart3.Legend.Location = LegendLocation.Bottom;
            UltraChart3.Legend.SpanPercentage = 16;
            UltraChart3.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Border.Thickness = 0;
            UltraChart3.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart3.Axis.X.Margin.Near.Value = 2;
            UltraChart3.Axis.Y.Margin.Near.Value = 2;
            UltraChart3.TitleLeft.Visible = true;
            UltraChart3.TitleLeft.Text = "млн.руб.";
            UltraChart3.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart3.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart3.Transform3D.XRotation = -245;
            UltraChart3.Transform3D.YRotation = 0;
            UltraChart3.Transform3D.ZRotation = 0;
            UltraChart3.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 9; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            color = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            color = Color.Red;
                            break;
                        }
                    case 6:
                        {
                            color = Color.Blue;
                            break;
                        }
                    case 7:
                        {
                            color = Color.DarkViolet;
                            break;
                        }
                    case 8:
                        {
                            color = Color.OrangeRed;
                            break;
                        }
                    case 9:
                        {
                            color = Color.CornflowerBlue;
                            break;
                        }
                }
                pe.Fill = color;
                UltraChart3.ColorModel.Skin.PEs.Add(pe);
            }
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            item.EnableLineStyle = true;
            item.EnablePoint = false;
            style.DrawStyle = LineDrawStyle.Dash;
            style.MidPointAnchors = false;
            item.LineStyle = style;
            UltraChart3.LineChart.EmptyStyles.Add(item);
            UltraChart3.ColorModel.Skin.ApplyRowWise = false;
            UltraChart3.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart.Data.SwapRowsAndColumns = false;
            //UltraGridExporter1.Visible = true;
            //UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            //UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            //UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            //UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            //UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            //UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
            //        <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            //UltraGridExporter1.PdfExporter.EndExport += new EventHandler
            //    <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);                        
            ////CRHelper.SaveToUserAgentLog(String.Format("Остальной прелоад {0}", Environment.TickCount - start));
            //UltraGridExporter1.MultiHeader = true;
            //UltraGridExporter1.HeaderChildCellHeight = 100;
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {

        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
         for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    double div = 1;
                    if (box.DataPoint != null)
                    {
                        if ((dtChart.Rows[0][box.Row + 1] != DBNull.Value)&&(dtChart.Rows[1][box.Row + 1] != DBNull.Value))
                        {
                            div = Convert.ToDouble(dtChart.Rows[box.Column][box.Row + 1]) / (Convert.ToDouble(dtChart.Rows[0][box.Row + 1]) + Convert.ToDouble(dtChart.Rows[1][box.Row + 1]));
                        }
                    string percent = string.Format("{0:P2}", div);
                    box.DataPoint.Label = string.Format("{0}\nна {1}\n{2} млн. руб. ({3})", box.DataPoint.Label.ToString(), box.Series.Label.ToString(), dtChart.Rows[box.Column][box.Row + 1].ToString(), percent);   
                    }
                }
            }  
        }
        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            //CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));

            start = Environment.TickCount;
            if (!Page.IsPostBack)
            {
                
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("STAT_0002_0003_Sakhalin_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);


                regionsCombo.Width = 300;
                regionsCombo.Title = "Территория";
                regionsCombo.MultiSelect = false;
                regionsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "STAT_0002_0003_Sakhalin_regions");
                regionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(regionsDigest.UniqueNames, regionsDigest.MemberLevels));
                regionsCombo.SetСheckedState("Сахалинская область", true);

                periodYear.Value = dtDate.Rows[0][0].ToString();

            }
            UserParams.Region.Value = regionsDigest.GetMemberUniqueName(regionsCombo.SelectedValue);
            Units.Value = "в рублях";
            Faces.Value = "Потребительские кредиты:";
            string firstyear = "2005";

            string query1 = DataProvider.GetQueryText("Dates1");
            DataTable dateGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query1, dateGrid);
            if (dateGrid.Rows.Count > 0)
            {
                LastDate.Value = dateGrid.Rows[dateGrid.Rows.Count - 1][4].ToString();
            }
            string region = string.Empty;
            switch (regionsCombo.SelectedValue)
            {
                case "Приморский край":
                    {
                        region = "Приморского края";
                        break;
                    }
                case "Хабаровский край":
                    {
                        region = "Хабаровского края";
                        break;
                    }
                case "Амурская область":
                    {
                        region = "Амурской области";
                        break;
                    }
                case "Камчатский край":
                    {
                        region = "Камчатского края";
                        break;
                    }
                case "Магаданская область":
                    {
                        region = "Магаданской области";
                        break;
                    }
                case "Сахалинская область":
                    {
                        region = "Сахалинской области";
                        break;
                    }
                case "Чукотский автономный округ":
                    {
                        region = "Чукотского автономного округа";
                        break;
                    }
                case "Республика Саха (Якутия)":
                    {
                        region = "Республики Саха (Якутия)";
                        break;
                    }
                case "Еврейская автономная область":
                    {
                        region = "Еврейской автономной области";
                        break;
                    }
            }
           ChartCaption.Text = String.Format("Структурная динамика выданных объемов потребительских и ипотечных кредитов, {0}", regionsCombo.SelectedValue);
           ChartCaption1.Text = String.Format("Соотношение выданных объемов потребительских кредитов {0} с другими субъектами Дальневосточного ФО", region);
           DateTime DateTime = CRHelper.DateByPeriodMemberUName(LastDate.Value.ToString(), 3);
           ChartCaption2.Text = String.Format("Структурное соотношение выданных объемов кредитов за {1:dd.MM.yy}, {0}", regionsCombo.SelectedValue, DateTime);
           ChartCaption3.Text = String.Format("Соотношение выданных объемов ипотечных кредитов {0} с другими субъектами Дальневосточного ФО", region);
           Page.Title = "Потребительские и ипотечные кредиты";
           Label1.Text = Page.Title;
           Label2.Text = "Анализ динамики процентных ставок и выданных объёмов по потребительским и ипотечным кредитам в субъектах Российской Федерации, входящих в состав Дальневосточного федерального округа (млн. руб.)";
           UserParams.PeriodYear.Value = "2011";
           UserParams.Region.Value = regionsDigest.GetMemberUniqueName(regionsCombo.SelectedValue);
           UserParams.StateArea.Value = regionsCombo.SelectedValue;
           
           headerLayout = new GridHeaderLayout(UltraWebGrid);
           UltraWebGrid.Bands.Clear();
           UltraWebGrid.DataBind();
           
           UltraChart.DataBind();
           UltraChart1.DataBind();
           UltraChart2.DataBind();
           UltraChart3.DataBind();
           string patternValue = UserParams.StateArea.Value;
           //int defaultRowIndex = 1;
           //UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
           //ActiveGridRow(row);
           //CRHelper.SaveToUserAgentLog(String.Format("Остльной лоад {0}", Environment.TickCount - start));       
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
                 
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            
                string query = DataProvider.GetQueryText("STAT_0002_0003_Sakhalin_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);
                query = DataProvider.GetQueryText("Dates1");
                DataTable dtGridDate = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);
                if (dtGrid.Rows.Count > 0)
                {
                    for (int i = 0; i < dtGrid.Rows.Count; i++)
                    {
                        DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtGridDate.Rows[i][4].ToString(), 3);
                        dtGrid.Rows[i][0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

             for (int k = 0; k < dtGrid.Rows.Count; k++)
                {
                    DataRow row = dtGrid.Rows[k];

                    for (int i = 1; i < row.ItemArray.Length - 1; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            if ((i == 1) || (i == 19)) row[i] = Convert.ToDouble(row[i]) / 1000000;
                        }
                    }
                }        
               
                int oldcount = dtGrid.Columns.Count - 1;
                DataColumn column = new DataColumn("Потребительские кредиты:;1", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Потребительские кредиты:;2", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Потребительские кредиты:;3", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Ипотечные кредиты:;1", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Ипотечные кредиты:;2", typeof(string));
                dtGrid.Columns.Add(column);
                column = new DataColumn("Ипотечные кредиты:;3", typeof(string));
                dtGrid.Columns.Add(column);
                for (int i = 0; i < dtGrid.Rows.Count; i++)
                {

                    dtGrid.Rows[i][oldcount + 1] = string.Format("{0:N2}",dtGrid.Rows[i][1]);//
                    dtGrid.Rows[i][oldcount + 2] = string.Format("{0:P2} - {1:P2}", dtGrid.Rows[i][8], dtGrid.Rows[i][9]);
                    dtGrid.Rows[i][oldcount + 3] = string.Format("{0:P2} - {1:P2}", dtGrid.Rows[i][11], dtGrid.Rows[i][12]);
                    dtGrid.Rows[i][oldcount + 4] = string.Format("{0:N2}", dtGrid.Rows[i][19]);//
                    dtGrid.Rows[i][oldcount + 5] = string.Format("{0:P2} - {1:P2}", dtGrid.Rows[i][29], dtGrid.Rows[i][30]);
                    dtGrid.Rows[i][oldcount + 6] = string.Format("{0:P2} - {1:P2}", dtGrid.Rows[i][32], dtGrid.Rows[i][33]);
                 
                } 

                for (int i = 0; i < oldcount; i++)
                {
                    dtGrid.Columns.RemoveAt(1);
                }
                dtGrid.AcceptChanges();
                UltraWebGrid.DataSource = dtGrid;            
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(80);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;



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
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(95);
                }


            }
            int count = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[0].Hidden = false;

            for (int k = 1; k < count; k++)
            {
                e.Layout.Bands[0].Columns[count - k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[count - k].CellStyle.Padding.Right = 5;
            }

            e.Layout.Bands[0].Columns[count - 6].Header.Caption = "Объёмы (млн.руб)";
            e.Layout.Bands[0].Columns[count - 5].Header.Caption = "Процентная ставка по выданным кредитам (средневзвеш.), % (в рублях)";
            e.Layout.Bands[0].Columns[count - 4].Header.Caption = "Процентная ставка по выданным кредитам (средневзвеш.), % (в иностранной валюте)";
            e.Layout.Bands[0].Columns[count - 3].Header.Caption = "Объёмы (млн.руб)";
            e.Layout.Bands[0].Columns[count - 2].Header.Caption = "Процентная ставка по выданным кредитам (средневзвеш.), % (в рублях)";
            e.Layout.Bands[0].Columns[count - 1].Header.Caption = "Процентная ставка по выданным кредитам (средневзвеш.), % (в иностранной валюте)";

            //headerLayout.AddCell(e.Layout.Bands[0].Columns[count - 6].Header.Caption);
            //headerLayout.AddCell(e.Layout.Bands[0].Columns[count - 5].Header.Caption);
            //headerLayout.AddCell(e.Layout.Bands[0].Columns[count - 4].Header.Caption);
            //headerLayout.AddCell(e.Layout.Bands[0].Columns[count - 3].Header.Caption);
            //headerLayout.AddCell(e.Layout.Bands[0].Columns[count - 2].Header.Caption);
            //headerLayout.AddCell(e.Layout.Bands[0].Columns[count - 1].Header.Caption);

            //ColumnHeader ch = new ColumnHeader(true);
            //ch.Caption = "Потребительские кредиты:";
            //ch.Title = "";
            //ch.RowLayoutColumnInfo.OriginY = 0;
            //ch.RowLayoutColumnInfo.OriginX = count - 6;
            //ch.RowLayoutColumnInfo.SpanX = 3;
            //ch.Style.Wrap = true;
            //e.Layout.Bands[0].HeaderLayout.Add(ch);

            //ch = new ColumnHeader(true);
            //ch.Caption = "Ипотечные кредиты:";
            //ch.Title = "";
            //ch.RowLayoutColumnInfo.OriginY = 0;
            //ch.RowLayoutColumnInfo.OriginX = count - 3;
            //ch.RowLayoutColumnInfo.SpanX = 3;
            //ch.Style.Wrap = true;
            //e.Layout.Bands[0].HeaderLayout.Add(ch);
            //e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            //Collection<string> cellsCaption = new Collection<string>();
            headerLayout.AddCell("Период");
            GridHeaderCell groupCell = headerLayout.AddCell("Потребительские кредиты:");
            groupCell.AddCell(e.Layout.Bands[0].Columns[count - 6].Header.Caption);
            groupCell.AddCell(e.Layout.Bands[0].Columns[count - 5].Header.Caption);
            groupCell.AddCell(e.Layout.Bands[0].Columns[count - 4].Header.Caption);

            GridHeaderCell groupCell2 = headerLayout.AddCell("Ипотечные кредиты:");
            groupCell2.AddCell(e.Layout.Bands[0].Columns[count - 3].Header.Caption);
            groupCell2.AddCell(e.Layout.Bands[0].Columns[count - 2].Header.Caption);
            groupCell2.AddCell(e.Layout.Bands[0].Columns[count - 1].Header.Caption);

            headerLayout.ApplyHeaderInfo();

        }
        

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {

        }



        #endregion

        #region Обработчики диаграмы

        DataTable dtChart = new DataTable();

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {


            

            string queryName = "Chart_query";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            query = DataProvider.GetQueryText("Dates");
            DataTable dtGridDate = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtGridDate);

            if (dtChart.Rows.Count > 0)
            {

                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtGridDate.Rows[i - 1][4].ToString(), 3);
                    dtChart.Columns[i].ColumnName = string.Format("{0:dd.MM.yy}", dateTime);
                }

            }
            for (int k = 0; k < dtChart.Rows.Count; k++)
            {
                DataRow row = dtChart.Rows[k];

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                    
                }
                
            }
            if (dtChart.Columns.Count > 10)
            {
                int newCount = 10;
                while (!(dtChart.Columns.Count == newCount + 1))
                {
                    dtChart.Columns.RemoveAt(1);
                }
            }

                dtChart.AcceptChanges();
                UltraChart.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
                        
            DataTable dtChart = new DataTable();

            string queryName = "Chart1_query";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            dtChart.Columns.RemoveAt(0);

            if (dtChart.Rows.Count > 0)
            {
                 for (int i = 0; i < dtChart.Rows.Count; i++)
                  {
                      DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtChart.Rows[i][0].ToString(), 3);
                      dtChart.Rows[i][0] = string.Format("{0:dd.MM.yy}", dateTime);
                  }
            }
              for (int k = 0; k < dtChart.Rows.Count; k++)
              {
                  DataRow row = dtChart.Rows[k];

                  for (int i = 1; i < row.ItemArray.Length; i++)
                  {
                      if (row[i] != DBNull.Value)
                      {
                          row[i] = Convert.ToDouble(row[i]) / 1000000;
                      }
                  }
              }

           UltraChart1.DataSource = dtChart;

        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {

            DataTable dtChart = new DataTable();

            string queryName = "Chart2_query";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            for (int k = 0; k < dtChart.Rows.Count; k++)
            {
                switch (k)
                {
                    case 0:
                        {
                            dtChart.Rows[k][0] = "Потребительские кредиты";
                            break;
                        }
                    case 1:
                        {
                            dtChart.Rows[k][0] = "Ипотечные кредиты";
                            break;
                        }
                }
            }
            for (int k = 0; k < dtChart.Rows.Count; k++)
            {
                DataRow row = dtChart.Rows[k];

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }

                }

            }
            UltraChart2.Data.SwapRowsAndColumns = false;
            UltraChart2.DataSource = dtChart;


        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {

            DataTable dtChart = new DataTable();

            string queryName = "Chart3_query";
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            dtChart.Columns.RemoveAt(0);
            if (dtChart.Rows.Count > 0)
            {
                for (int i = 0; i < dtChart.Rows.Count; i++)
                {

                    DateTime dateTime = CRHelper.DateByPeriodMemberUName(dtChart.Rows[i][0].ToString(), 3);
                    dtChart.Rows[i][0] = string.Format("{0:dd.MM.yy}", dateTime);
                }
            }
            for (int k = 0; k < dtChart.Rows.Count; k++)
            {
                DataRow row = dtChart.Rows[k];

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            UltraChart3.DataSource = dtChart;

        }

        void UltraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {

        
        }

        void UltraChart2_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart2.Legend.Location == LegendLocation.Top) || (UltraChart2.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart2.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = (UltraChart2.Legend.SpanPercentage * (int)UltraChart2.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart2.Legend.Margins.Left + UltraChart2.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }

        #endregion
        
        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {

            ActiveGridRow(e.Row);
        }
        #region Экспорт в Excel

        //private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        //{
        //    ActiveGridRow(UltraWebGrid.Rows[3]);
        //    e.CurrentWorksheet.Rows[0].Cells[0].Value = Page.Title;
        //    e.CurrentWorksheet.Rows[1].Cells[0].Value = "Анализ динамики процентных ставок и выданных объёмов по потребительским и ипотечным кредитам в субъектах Российской Федерации, входящих в состав Уральского федерального округа (млн. руб.)";
        //}

        //private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        //{
        //    e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex ].Header.Key.Split(';')[0].Trim();
        //}

        //private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        //{

        //    int columnCount = UltraWebGrid.Columns.Count;
        //    int width = 300;
        //    e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
        //    e.CurrentWorksheet.Columns[0].Width = width * 27;
        //    e.CurrentWorksheet.Columns[1].Width = width * 27;
        //    e.CurrentWorksheet.Columns[2].Width = width * 27;
        //    e.CurrentWorksheet.Columns[3].Width = width * 27;
        //    e.CurrentWorksheet.Columns[4].Width = width * 27;
        //    e.CurrentWorksheet.Columns[5].Width = width * 27;
        //    e.CurrentWorksheet.Columns[6].Width = width * 27;
        //    e.CurrentWorksheet.Columns[7].Width = width * 27;
        //    e.CurrentWorksheet.Columns[8].Width = width * 27;
        //    e.CurrentWorksheet.Columns[9].Width = width * 27;
        //    e.CurrentWorksheet.Columns[10].Width = width * 27;
        //    e.CurrentWorksheet.Columns[11].Width = width * 27;
        //    e.CurrentWorksheet.Columns[12].Width = width * 27;
        //    e.CurrentWorksheet.Columns[13].Width = width * 27;
        //    e.CurrentWorksheet.Columns[14].Width = width * 27;


        //    int columnCountt = UltraWebGrid.Columns.Count;
        //    for (int i = 1; i < columnCountt; i = i + 1)
        //    {
        //        for (int j = 5; j < 20; j++)
        //        {
        //            e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                 
        //        }
        //    }

        //    int columnCounttt = UltraWebGrid.Columns.Count;
        //    for (int i = 1; i < columnCounttt; i = i + 1)
        //    {
        //        e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00";
        //        e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
        //    }
        //    for (int i = 3; i < 5; i++)
        //    {
        //        e.CurrentWorksheet.Rows[i].Height = 20 * 35;
        //    }
        //    for (int k = 1; k < columnCounttt; k = k + 1)
        //    {
        //        e.CurrentWorksheet.Rows[3].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
        //        e.CurrentWorksheet.Rows[4].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
        //    }
        //}

        //private void ExcelExportButton_Click(object sender, EventArgs e)
        //{
        //    UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
        //    UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
        //    UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
            
        //}

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
            Worksheet sheet4 = workbook.Worksheets.Add("sheet4");
            Worksheet sheet5 = workbook.Worksheets.Add("sheet5");

            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.SheetColumnCount = 20;

            ReportExcelExporter1.GridColumnWidthScale = 1.5;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.6));
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.6));
            UltraChart1.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.35));
            UltraChart1.Legend.SpanPercentage = 20;
            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.35));
            UltraChart2.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.35));
            UltraChart3.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.6));
            UltraChart3.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.35));
            UltraChart3.Legend.SpanPercentage = 20;

            ReportExcelExporter1.Export(UltraChart, ChartCaption.Text, sheet2, 3);
            ReportExcelExporter1.Export(UltraChart1, ChartCaption1.Text, sheet3, 3);
            ReportExcelExporter1.Export(UltraChart2, ChartCaption2.Text, sheet4, 3);
            ReportExcelExporter1.Export(UltraChart3, ChartCaption3.Text, sheet5, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            ISection section4 = report.AddSection();
            ISection section5 = report.AddSection();

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart1.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.35));
            UltraChart1.Legend.SpanPercentage = 20;
            UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.35));
            UltraChart2.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.35));
            UltraChart3.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart3.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.35));
            UltraChart3.Legend.SpanPercentage = 20;

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart, ChartCaption.Text, section2);
            ReportPDFExporter1.Export(UltraChart1, ChartCaption1.Text, section3);
            ReportPDFExporter1.Export(UltraChart2, ChartCaption2.Text, section4);
            ReportPDFExporter1.Export(UltraChart3, ChartCaption3.Text, section5);
        }


        //private void PdfExportButton_Click(object sender, EventArgs e)
        //{
        //    UltraGridExporter1.MultiHeader = true;

        //    UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
        //    UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        //}

        //private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        //{
        //    IText title = e.Section.AddText();
        //    Font font = new Font("Verdana", 16);
        //    title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
        //    title.Style.Font.Bold = true;
        //    title.AddContent(Page.Title);

        //    title = e.Section.AddText();
        //    font = new Font("Verdana", 14);
        //    title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
        //    title.AddContent("Анализ динамики процентных ставок и выданных объёмов по потребительским и ипотечным кредитам в субъектах Российской Федерации, входящих в состав Уральского федерального округа (млн. руб.)");
        //}

        //private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        //{
        //    e.Section.AddPageBreak();

        //    UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
        //    UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
        //    UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
        //    UltraChart3.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
        //    IText title = e.Section.AddText();
        //    Font font = new Font("Verdana", 14);
        //    title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
        //    title.Style.Font.Bold = true;
        //    title.AddContent(ChartCaption2.Text);

        //    Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart2);
        //    e.Section.AddImage(img);

        //    e.Section.AddPageBreak();

        //    title = e.Section.AddText();
        //    font = new Font("Verdana", 14);
        //    title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
        //    title.Style.Font.Bold = true;
        //    title.AddContent(ChartCaption.Text);

        //    img = UltraGridExporter.GetImageFromChart(UltraChart);
        //    e.Section.AddImage(img);

        //    e.Section.AddPageBreak();
        //    title = e.Section.AddText();
        //    font = new Font("Verdana", 14);
        //    title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
        //    title.Style.Font.Bold = true;
        //    title.AddContent(ChartCaption1.Text);

        //    img = UltraGridExporter.GetImageFromChart(UltraChart1);
        //    e.Section.AddImage(img);

        //    e.Section.AddPageBreak();


        //    title = e.Section.AddText();
        //    font = new Font("Verdana", 14);
        //    title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
        //    title.Style.Font.Bold = true;
        //    title.AddContent(ChartCaption3.Text);

        //    img = UltraGridExporter.GetImageFromChart(UltraChart3);
        //    e.Section.AddImage(img);
        //}

        #endregion  
        
    }
}