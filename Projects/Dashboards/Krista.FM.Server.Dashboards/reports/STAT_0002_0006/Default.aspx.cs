using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using System.Web;
using System.IO;

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0006
{
    public partial  class Default: CustomReportPage
    {
        #region ����

        private DataTable dtGrid;
        private DataTable dtChart1;
        private DataTable dtChart2;
        private DataTable dtData;
        private DataTable dt;
        private DataTable dt1;
        private int kol;

        #endregion

        private int GetScreenWidth
        {
            get
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                    {
                        HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                        int value = Int32.Parse(cookie.Value);
                        return value;
                    }
                }
                return (int)Session["width_size"];
            }
        }

        private bool IsSmallResolution
        {
            get { return GetScreenWidth < 1000; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 1500: CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        private UltraChart UltraChart2;
        private Label chart2ElementCaption;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraWebGrid1.Width = IsSmallResolution ? 720 :CRHelper.GetGridWidth(MinScreenWidth - 12) ;
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "��� ������";
            UltraChart1.Width = CRHelper.GetChartWidth(MinScreenWidth / 2 - 30);
            UltraChart1.Height = CRHelper.GetChartHeight(MinScreenHeight / 1.6);

            UltraChart2 = new UltraChart();
            chart2ElementCaption = new Label();
            chart2ElementCaption.CssClass = "ElementTitle";

            UltraChart2.Width = CRHelper.GetChartWidth(MinScreenWidth / 2 - 30);
            UltraChart2.Height = CRHelper.GetChartHeight(MinScreenHeight / 1.6);

            #region ��������� ��������

            UltraChart1.ChartType = ChartType.PieChart; // �������� ���������
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Extent = 130;
            UltraChart1.Axis.Y.Extent = 130;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0>  �������";
            
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 10);
            System.Drawing.Font font1 = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Legend.Font = IsSmallResolution ? font: font1;
            UltraChart1.PieChart.Labels.Font = IsSmallResolution ? font : font1;
        
            UltraChart1.Legend.SpanPercentage = IsSmallResolution ? 18 : 15; 
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);

            UltraChart2.ChartType = ChartType.StackColumnChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.X.Extent = 40;
            UltraChart2.Axis.Y.Extent = 40;
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart2.Axis.X.Labels.Visible = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.Data.SwapRowsAndColumns = false;
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Legend.Font = IsSmallResolution ? font : font1;
            UltraChart2.Legend.SpanPercentage = IsSmallResolution ? 18 : 15;
            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Text = "�������";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Far;
            CRHelper.CopyCustomColorModel(UltraChart1, UltraChart2);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChartFF_FillSceneGraph);
            UltraChart2.DataBinding+=new EventHandler(UltraChart2_DataBinding);
            UltraChart2.DeploymentScenario.FilePath = "../../TemporaryImages/";
            UltraChart2.DeploymentScenario.ImageURL = "../../TemporaryImages/Chart_stat_01_07#SEQNUM(100).png";
            UltraChart2.Axis.Y.LineThickness = 1;

            GradientEffect gradientEffect = new GradientEffect();
            gradientEffect.Style = GradientStyle.ForwardDiagonal;
            gradientEffect.Coloring = GradientColoringStyle.Darken;
            UltraChart2.Effects.Effects.Add(gradientEffect);

            if ( ComboKinds.SelectedIndex == 1 || ComboKinds.SelectedIndex == 2 )
            {
                 UltraChart1.Height = CRHelper.GetChartHeight(MinScreenHeight / 2);
                 UltraChart2.Height = CRHelper.GetChartHeight(MinScreenHeight / 2);

                UltraChart1.Legend.SpanPercentage = IsSmallResolution ? 20 : 12;
                UltraChart2.Legend.SpanPercentage = IsSmallResolution ? 20 : 12;
            }

            if (ComboKinds.SelectedIndex == 4)
            {
                UltraChart1.Height = CRHelper.GetChartHeight(MinScreenHeight / 2);
                UltraChart2.Height = CRHelper.GetChartHeight(MinScreenHeight /2);

                UltraChart1.Legend.SpanPercentage = IsSmallResolution ? 20 : 16;
                UltraChart2.Legend.SpanPercentage = IsSmallResolution ? 20 : 17;
            }

            if (ComboKinds.SelectedIndex == 3)
            {
                UltraChart1.Height = CRHelper.GetChartHeight(MinScreenHeight / 2);
                UltraChart2.Height = CRHelper.GetChartHeight(MinScreenHeight / 1.92);

                UltraChart1.Legend.SpanPercentage = IsSmallResolution ? 20 : 16;
                UltraChart2.Legend.SpanPercentage = IsSmallResolution ? 20 : 16;
            }
            #endregion

            UltraGridExporter1.Visible = true;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
            //UltraGridExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);
            
            if (IsSmallResolution)
            {
                HorizontalTD.Visible = false;
                VerticalTD.Visible = true;
                VerticalChartTD.Controls.Add(chart2ElementCaption);
                VerticalChartTD.Controls.Add(UltraChart2);
            }
            else
            {
                VerticalTD.Visible = false;
                HorizontalTD.Visible = true;
                HorizontalChartTD.Controls.Add(chart2ElementCaption);
                HorizontalChartTD.Controls.Add(UltraChart2);
            }
        }
        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //������������� ������ ������ ������� 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart1.Legend.Location == LegendLocation.Top) || (UltraChart1.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart1.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = ((int)UltraChart1.Legend.SpanPercentage * (int)UltraChart1.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart1.Legend.Margins.Left + UltraChart1.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }

       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                FillComboRegions();
                ComboRegion.Title = "����������";
                ComboRegion.Width = IsSmallResolution ? 310 : 400;
                ComboRegion.Set�heckedState("��������� ����������� �����", true);
                ComboRegion.ParentSelect = true;

                ComboKinds.Title = "�����������";
                ComboKinds.Width = IsSmallResolution ? 200 : 350;
                ComboKinds.Visible = true;
                ComboKinds.FillDictionaryValues(GetKindsDictionary());
                ComboKinds.Set�heckedState("�� ��������",true);
            }
            Page.Title = string.Format("����������� ����������� �������, ��������� �� ��������������� ����� {0}, {1}", ComboKinds.SelectedValue, RegionsNamingHelper.ShortName(ComboRegion.SelectedValue));
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = " ������ �������� � ������������ ����������� ����������� ����������� �������, ��������� �� ��������������� ����� �� ��������, ����, ���� ���������, ����������������� ������ ������ � �� ������� ����� ������ � ��������� ���������� ���������, �������� � ��������� ����������� �����";
            //Label1.Font.Bold = true; 
            //Label1.Text = string.Format("����������� ����������� �������, ��������� �� ��������������� ����� {0}, {1}", ComboKinds.SelectedValue, RegionsNamingHelper.ShortName(ComboRegion.SelectedValue));
            
            UserParams.SubjectFO.Value = ComboRegion.SelectedValue == "��������� ����������� �����"
                                            ? String.Empty
                                            : String.Format(".[{0}]", ComboRegion.SelectedValue);

            dtData = new DataTable();
            string queryName = "STAT_0002_0006_Data";
            queryName = String.Format("{0}_{1}",queryName,QueryPostfix);
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query,dtData);
            string quoter = string.Empty;
            string year = string.Empty;
            chart1ElementCaption.Text = string.Empty;
            chart2ElementCaption.Text = string.Empty;

           if (dtData.Rows.Count > 0)
            {
                quoter = dtData.Rows[0][2].ToString().Replace("������� ", string.Empty);
                year = dtData.Rows[0][0].ToString();
                chart1ElementCaption.Text = string.Format(" ��������� ����������� ����������� �������, ��������� �� ��������������� ����� {2} �� {0} ������� {1} ����", quoter, year,ComboKinds.SelectedValue);
            }

           if (ComboKinds.SelectedIndex == 3)
            {
                chart2ElementCaption.Text =
                    string.Format(
                        "�������� � ��������� ����������� ����������� �������, ��������� �� ��������������� ����� {0}",
                        ComboKinds.SelectedValue);
            }
            else
            {
                chart2ElementCaption.Text =
                    string.Format(
                        "�������� � ��������� ����������� ����������� �������, ��������� �� ��������������� ����� {0}",
                        ComboKinds.SelectedValue);
            }
            
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }

        private void FillComboRegions()
        {
            DataTable dtRegions = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0002_0006_Regions");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�����", dtRegions);

            Dictionary<string, int> regions = new Dictionary<string, int>();
            foreach (DataRow row in dtRegions.Rows)
            {
                regions.Add(row[0].ToString(), 0);
            }
            regions.Add("��������� ����������� �����", 0);
            ComboRegion.FillDictionaryValues(regions);
        }

      private Dictionary<string, int> GetKindsDictionary()
        {
            Dictionary <string, int> kinds = new Dictionary<string, int>();
            kinds.Add("�� ��������",0);
            kinds.Add("�� ����", 0);
            kinds.Add("�� ���� ���������", 0);
            kinds.Add("�� ����������������� ������ ������", 0);
            kinds.Add("�� ����� ������", 0);
            return kinds;
        }
        private string QueryPostfix
        {
            get
            {
                switch (ComboKinds.SelectedIndex)
                {
                    case (0):
                        {
                            return "age";
                        }
                    case (1):
                        {
                            return "sex";
                        }
                    case (2):
                        {
                            return "place";
                        }
                    case (3):
                        {
                            return "search";
                        }
                    case (4):
                        {
                            return "experience";
                        }
                    default:
                        {
                            return "age";
                        }
                }
            }
        }
        // � ���� ����!!!
       #region ���������� �����

        protected void UltraWebGrid_DataBinding(Object sender,EventArgs e)
        {
            string query = DataProvider.GetQueryText(string.Format("STAT_0002_0006_grid_{0}",QueryPostfix));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"�������",dtGrid);
           
           if (ComboKinds.SelectedIndex == 1)
            {
                dtGrid.Rows[0][0] = "�������";
                dtGrid.Rows[1][0] = "�������";
            }
            if (ComboKinds.SelectedIndex == 2)
            {
                dtGrid.Rows[0][0] = "��������� ���������";
                dtGrid.Rows[1][0] = "�������� ���������";
            }
            if (ComboKinds.SelectedIndex == 3)
            {
                dtGrid.Rows[0][0] = "�� 1 ������";
                dtGrid.Rows[1][0] = "�� 1 �� 4 �������";
                dtGrid.Rows[2][0] = "�� 4 �� 8 �������";
                dtGrid.Rows[3][0] = "�� 8 ������� �� ����";
                dtGrid.Rows[4][0] = "����� ����";
            }
            if (ComboKinds.SelectedIndex == 4)
            {
                dtGrid.Rows[0][0] = "����� �� ����������, ������ ������ �������";
                dtGrid.Rows[1][0] = "����� ����������";
            }
            dtGrid.AcceptChanges();
            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid1.DataSource = dtGrid;
            }
        }
        protected  void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
           
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

           for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i==0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }
             e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
             e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(138);

             for (int i = 1; i < UltraWebGrid1.Columns.Count; i++)
             {
                 CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                 e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(68);
             }

             if (ComboKinds.SelectedIndex == 0)
             {
                 CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 0, "�� ��������", "");
             }
            if (ComboKinds.SelectedIndex==1) // ����������� �� ����
          {
              CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 0, "�� ����", "");
          }

         if (ComboKinds.SelectedIndex == 2) //����������� �� ���� ���������
         {
             CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 0, "�� ���� ���������", "");
         }

          if (ComboKinds.SelectedIndex ==3 ) // ����������� �� ����������������� ������ ������
          {
              CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 0, "�� ����������������� ������", "");
          }

          if (ComboKinds.SelectedIndex == 4) //����������� �� ����� ������
          {
              CRHelper.SetHeaderCaption(UltraWebGrid1, 0, 0, "�� ����� ������ ", "");
          }

           DataTable dt = new DataTable();
           string query = DataProvider.GetQueryText(string.Format("STAT_0002_0006_chart2_{0}", QueryPostfix));
           DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);
            int k = 0, k1 = 0, k2=0;
            for (int i=0; i<dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString()=="2008")
                {
                    k++;
                }
                if (dt.Rows[i][0].ToString() == "2009")
                {
                    k1++;
                }
                if (dt.Rows[i][0].ToString() == "2010")
                {
                    k2++;
                }
            }
            
            if (k!=0)
                {
                    CRHelper.AddHierarchyHeader(UltraWebGrid1, 0, "2008", 1, 0, k, 1);
                }
            if (k1!=0)
            {
                CRHelper.AddHierarchyHeader(UltraWebGrid1, 0, "2009", 1 + k, 0, k1, 1);
            }
            if (k2!=0)
            {
                CRHelper.AddHierarchyHeader(UltraWebGrid1, 0, "2010", 1 + k + k1, 0, k2, 1);
            }

        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
        }
        #endregion

        #region ���������� ��������
          
        protected void UltraChart1_DataBinding( Object sender, EventArgs e)
         {
                string query = DataProvider.GetQueryText(string.Format("STAT_0002_0006_chart1_{0}", QueryPostfix));
                dtChart1 = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���������1", dtChart1);

                UltraChart1.Series.Clear();
                for (int i = 1; i < dtChart1.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart1);
                    series.Label = dtChart1.Columns[i].ColumnName;
                    UltraChart1.Series.Add(series);
                }
              
         }

        protected  void UltraChart2_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(string.Format("STAT_0002_0006_chart2_{0}",QueryPostfix));
            dtChart2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "���������2", dtChart2);
            
            foreach (DataColumn column in dtChart2.Columns)
            {
                column.ColumnName = column.ColumnName.TrimEnd('_');
            }

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText(string.Format("STAT_0002_0006_chart2_{0}", QueryPostfix));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            List<string> yearList = new List<string>();

            for (int j = 0; j < dtChart2.Rows.Count; j++)
            {
               DataRow row = dtChart2.Rows[j];
               

               if (row[0] != DBNull.Value && dt.Rows[j][0] != DBNull.Value)
                {
                if (row[0].ToString() == "������� 4")
                {
                    string quarter = string.Format("{0}", row[0]);

                  /*  if (quarter == "������� 1")
                    {
                        row[0] = string.Format("1 �� {0}", dt.Rows[j][0], row[0]);
                    }

                    if (quarter == "������� 2")
                    {
                        row[0] = string.Format("2 �� {0}", dt.Rows[j][0], row[0]);
                    }

                    if (quarter == "������� 3")
                    {
                        row[0] = string.Format("3 �� {0}", dt.Rows[j][0], row[0]);
                    }
                    */
                    if (quarter == "������� 4")
                    {
                        row[0] = string.Format("4 �� {0}", dt.Rows[j][0], row[0]);
                    }   
                    yearList.Add(dt.Rows[j][0].ToString());
                }

                if (row[0].ToString() == "������� 3")
                {
                    string quarter = string.Format("{0}", row[0]);

                 /*   if (quarter == "������� 1")
                    {
                        row[0] = string.Format("1 �� {0}", dt.Rows[j][0], row[0]);
                    }

                    if (quarter == "������� 2")
                    {
                        row[0] = string.Format("2 �� {0}", dt.Rows[j][0], row[0]);
                    }
                    */
                    if (quarter == "������� 3")
                    {
                        row[0] = string.Format("3 �� {0}", dt.Rows[j][0], row[0]);
                    }
             /*
                    if (quarter == "������� 4")
                    {
                        row[0] = string.Format("4 �� {0}", dt.Rows[j][0], row[0]);
                    }
               */
                   yearList.Add(dt.Rows[j][0].ToString());
                }

                if (row[0].ToString() == "������� 2" ||
                     (j != dtChart2.Rows.Count-1 && dtChart2.Rows[j + 1][0].ToString() == "������� 3"))
                {
                    string quarter = string.Format("{0}", row[0]);

                  /*  if (quarter == "������� 1")
                    {
                        row[0] = string.Format("1 �� {0}", dt.Rows[j][0], row[0]);
                    }
                    */
                    if (quarter == "������� 2")
                    {
                        row[0] = string.Format("2 �� {0}", dt.Rows[j][0], row[0]);
                    }

                   /* if (quarter == "������� 3")
                    {
                        row[0] = string.Format("3 �� {0}", dt.Rows[j][0], row[0]);
                    }

                    if (quarter == "������� 4")
                    {
                        row[0] = string.Format("4 �� {0}", dt.Rows[j][0], row[0]);
                    }*/
                    yearList.Add(dt.Rows[j][0].ToString());

                }

                // ���� ������ ������, ���� � ������ ������ ������� 1, ���� ���������� ������� 4
                if (j == 0 ||
                     row[0].ToString() == "������� 1" ||
                     (j != 0 && dtChart2.Rows[j - 1][0].ToString() == "������� 4"))
                {
                    string quarter = string.Format("{0}", row[0]);

                    if (quarter == "������� 1")
                    {
                        row[0] = string.Format("1 �� {0}", dt.Rows[j][0], row[0]);
                    }

                  /*  if (quarter == "������� 2")
                    {
                        row[0] = string.Format("2 �� {0}", dt.Rows[j][0], row[0]);
                    }

                    if (quarter == "������� 3")
                    {
                        row[0] = string.Format("3 �� {0}", dt.Rows[j][0], row[0]);
                    }

                    if (quarter == "������� 4")
                    {
                        row[0] = string.Format("4 �� {0}", dt.Rows[j][0], row[0]);
                    }*/
                    yearList.Add(dt.Rows[j][0].ToString());
                }

            }
        }

            if (ComboKinds.SelectedIndex == 0)
            {
                dtChart2.Columns["16-17; �� ������"].ColumnName = "16-17";
                dtChart2.Columns["18-19; �� ������"].ColumnName = "18-19";
                dtChart2.Columns["20-24; �� ������"].ColumnName = "20-24";
                dtChart2.Columns["25-29; �� ������"].ColumnName = "25-29";
                dtChart2.Columns["��������������� �������� (�� 2 ���� �� ����������� ������); �� ������"].ColumnName =
                    "��������������� �������� (�� 2 ���� �� ����������� ������)";
                dtChart2.Columns["������ ���������; �� ������"].ColumnName = "������ ���������";
            }
            if (ComboKinds.SelectedIndex==1)
            {
                dtChart2.Columns["�������; �� ������"].ColumnName = "�������";
                dtChart2.Columns["�������; �� ������"].ColumnName = "�������";
            }
            if (ComboKinds.SelectedIndex==2)
            {
                dtChart2.Columns["��������� ���������; �� ������"].ColumnName = "��������� ���������";
                dtChart2.Columns["�������� ���������; �� ������"].ColumnName = "�������� ���������";
            }
            if (ComboKinds.SelectedIndex==3)
            {
                dtChart2.Columns["�� 1 ������; �� ������"].ColumnName = "�� 1 ������";
                dtChart2.Columns["�� 1 �� 4 �������; �� ������"].ColumnName = "�� 1 �� 4 �������";
                dtChart2.Columns["�� 4 �� 8 �������; �� ������"].ColumnName = "�� 4 �� 8 �������";
                dtChart2.Columns["�� 8 ������� �� ����; �� ������"].ColumnName = "�� 8 ������� �� ����";
                dtChart2.Columns["����� ����; �� ������"].ColumnName = "����� ����";
            }

            if (ComboKinds.SelectedIndex==4)
            {
                dtChart2.Columns["����� �� ����������, ������ ������ �������; �� ������"].ColumnName =
                    "����� �� ����������, ������ ������ �������";
                dtChart2.Columns["����� ����������; �� ������"].ColumnName = "����� ����������";

            }
           
            dtChart2.AcceptChanges();

            UltraChart2.Data.SwapRowsAndColumns = true;
            UltraChart2.Series.Clear();
            for (int i = 1; i < dtChart2.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart2);
                series.Label = dtChart2.Columns[i].ColumnName;
                UltraChart2.Series.Add(series);
            }

         }

        protected void UltraChartFF_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
          for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.DataPoint.Label != null)
                        {
                            int columnIndex = box.Column+1;
                            int rowIndex = box.Row;
                            int sum = 0;
                            for (int j = 1; j < dtChart2.Columns.Count;j++ )
                            {
                              if (dtChart2 != null && dtChart2.Rows.Count != 0 &&
                                       dtChart2.Rows[rowIndex][j] != DBNull.Value)
                              {sum += Convert.ToInt32(dtChart2.Rows[rowIndex][j]);}
                            }
                            double percent=0;
                            if (dtChart2 != null && dtChart2.Rows.Count != 0 &&
                                      dtChart2.Rows[rowIndex][columnIndex] != DBNull.Value)
                            {
                               percent = (Convert.ToDouble(dtChart2.Rows[rowIndex][columnIndex])/Convert.ToDouble(sum));
                            }
                            if (dtChart2 != null && dtChart2.Rows.Count != 0 &&
                                        dtChart2.Rows[rowIndex][columnIndex] != DBNull.Value)
                                {
                                    /*double percent;
                                       Double.TryParse((Convert.ToDouble(Convert.ToInt32(dtChart2.Rows[rowIndex][columnIndex]) / sum)).ToString(), out percent);
                                   */
                                    box.DataPoint.Label =
                                              string.Format("{0}\n {1:N0} �������, {2:P2}",
                                              box.DataPoint.Label, dtChart2.Rows[rowIndex][columnIndex],percent);
                                }
                        }
                    }
                }
            }
        }

        #endregion

        #region �������

            #region ������� � Excel
              private void ExcelExporter_BeginExport(Object sender, BeginExportEventArgs e)
              {
                  e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
                  e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
              }
             private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
             {
               e.HeaderText = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Caption;
             }

              private  void ExcelExporter_EndExport (Object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
              {
                  e.CurrentWorksheet.Columns[0].Width = 150*37;
                  e.CurrentWorksheet.Rows[3].Height = 17 * 37;
                  e.CurrentWorksheet.Rows[4].Height = 17*37;
                  for(int i=1 ; i<UltraWebGrid1.Columns.Count; i++)
                  {
                      e.CurrentWorksheet.Columns[i].Width = 100*37;
                      e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0";
                  }

                  for (int i = 2; i < UltraWebGrid1.Rows.Count+4; i++)
                  {
                      e.CurrentWorksheet.Rows[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                      e.CurrentWorksheet.Rows[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                      e.CurrentWorksheet.Rows[i].Height = 17*37;
                  }
              }

              private void ExcelExportButton_Click(Object sender, EventArgs e)
              {
                  UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
                  UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
                  UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1);
              }

        #endregion

            #region ������� � Pdf
            private void PdfExportButton_Click(object sender, EventArgs e)
            {
                UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
                
                UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
            }      
            
            private void PdfExporter_BeginExport (Object sender, DocumentExportEventArgs e)
             {
                 IText title = e.Section.AddText();
                 System.Drawing.Font font  = new System.Drawing.Font("Verdana",16);
                 title.Style.Font = new Font(font);
                 title.Style.Font.Bold = true;
                 title.AddContent(PageTitle.Text);

                 title = e.Section.AddText();
                 title.Style.Font = new Font(font);
                 title.Style.Font.Bold = false;
                 title.AddContent(PageSubTitle.Text);
                 
                 title = e.Section.AddText();
                 title.AddContent("                 ");

                 ITable table = e.Section.AddTable();
                 ITableRow row = table.AddRow();
                 ITableCell cell = row.AddCell();

                 cell.Width = new FixedWidth((float) UltraChart1.Width.Value + 10);
                 cell.AddText().AddContent(chart1ElementCaption.Text);

                 cell = row.AddCell();
                 cell.AddText().AddContent(chart2ElementCaption.Text);

                 row = table.AddRow();
                 cell = row.AddCell();
                 cell.Width = new FixedWidth((float)UltraChart1.Width.Value + 100);
                 cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart1));

                 cell = row.AddCell();
                 cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart2));
                  
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                 {
                     e.Layout.Bands[0].Columns[i].Width = 170;
                 }
             }
        #endregion
      #endregion

      

    }
}