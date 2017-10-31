using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.UltraWebGrid;
using System.Drawing;
using Microsoft.AnalysisServices.AdomdClient;
using Infragistics.UltraChart.Core;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;

using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;

using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

using Infragistics.Documents.Excel; 
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font; 
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Resources.Appearance;
namespace Krista.FM.Server.Dashboards.STAT_0028_0017_Novosib
{ 
    public partial class _default : CustomReportPage
	{
        private string page_title = "Мониторинг демографической ситуации ({0})";
        private string page_sub_title = "Данные мониторинга демографической ситуации по выбранной территории";
        private string chart2_caprion = "Сравнительная динамика показателя «{0}»";
        private string chart1_caprion = "Динамика численности населения с учётом расчётного прироста (убыли) населения ({0})";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam chart2_region { get { return (UserParams.CustomParam("chart2_region")); } }
        private CustomParam selectedPeriod { get { return (UserParams.CustomParam("selectedPeriod")); } }
        private CustomParam selectedRegion { get { return (UserParams.CustomParam("selectedRegion")); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok")); } }
        private CustomParam groupType { get { return (UserParams.CustomParam("groupType")); } }
        private CustomParam prevPeriod { get { return (UserParams.CustomParam("prevPeriod")); } }
        private CustomParam nextPeriod { get { return (UserParams.CustomParam("nextPeriod")); } }
        private CustomParam selectedPokRF { get { return (UserParams.CustomParam("selectedPokRF")); } }

        private GridHeaderLayout headerLayout;
        private string style = "";
        private bool IsSmallResolution
        { 
            get { return CRHelper.GetScreenWidth < 1200; }
        }
        private int minScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            ComboRegion.Width = 350;
            Year.Width = 200;
            Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            DBRFHelper = new DataBaseHelper(DataProvidersFactory.SecondaryMASDataProvider);
            Grid.Height = Unit.Empty;
            if (IsSmallResolution)
            {
                Chart2.Width = minScreenWidth - 5;
                Chart1.Width = minScreenWidth - 5;
            }
            else
            {
                Chart2.Width = (int)(minScreenWidth - 20);
                Chart1.Width = (int)(minScreenWidth - 20);
            }
            Chart2.Height = 450;
            Chart2.ChartType = ChartType.AreaChart;
            Chart2.Border.Thickness = 0;
            Chart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            Chart2.Axis.X.Extent = 60;
            Chart2.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 8);
            Chart2.Axis.X.Labels.FontColor = Color.Black;
            Chart2.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            Chart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";            
            Chart2.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            Chart2.Axis.Y.Labels.FontColor = Color.Black;
            Chart2.Axis.Y.Extent = 40;

            Chart2.Data.EmptyStyle.Text = " ";
            Chart2.EmptyChartText = " ";

            Chart2.AreaChart.NullHandling = NullHandling.DontPlot;

            Chart2.AreaChart.LineAppearances.Clear();

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 5;
            Chart2.AreaChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            Chart2.AreaChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.None;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 5;
            lineAppearance.LineStyle.MidPointAnchors = false;
            Chart2.AreaChart.LineAppearances.Add(lineAppearance);

            Chart2.Legend.Visible = true;
            Chart2.Legend.Location = LegendLocation.Top;
            Chart2.Legend.SpanPercentage = 18;
            Chart2.Legend.Font = new System.Drawing.Font("Verdana", 10);


            Chart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
            Chart2.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                PaintElementType peType = PaintElementType.Gradient;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Transparent;
                            stopColor = Color.Gray;
                            peType = PaintElementType.Hatch;
                            pe.Hatch = FillHatchStyle.ForwardDiagonal;
                            break;
                        }
                    case 2:
                        {
                            color = Color.MediumSeaGreen;
                            stopColor = Color.Green;
                            peType = PaintElementType.Gradient;
                            

                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = peType;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = (byte)150;
                pe.FillStopOpacity = (byte)150;
                Chart2.ColorModel.Skin.PEs.Add(pe);
                
            }

            Grid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

        }

		protected override void Page_Load(object sender, EventArgs e)
		{
            base.Page_Load(sender, e);
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            if (!Page.IsPostBack)
            {
                ComboRegion.Title = "Территория";
                ComboRegion.FillDictionaryValues(RegionsLoad("Regions"));
                Year.Title = "Период";
                Year.FillDictionaryValues(YearsLoad("Years"));
                Year.SelectLastNode();
                ComboRegion.ParentSelect = true;
                ComboRegion.SetСheckedState("Новосибирская область", true);
                
            }
            PageTitle.Text =String.Format(page_title,ComboRegion.SelectedValue);
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = page_sub_title;
            selectedPeriod.Value = Year.SelectedValue.Split(' ')[0];
            if (ComboRegion.SelectedNode.Level==0)
            {
                selectedRegion.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Сибирский федеральный округ].[Новосибирская область].datamember";
                chart2_region.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].datamember";
                groupType.Value = "Без группировки";
            }
            else 
            { 
                selectedRegion.Value="[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Сибирский федеральный округ].[Новосибирская область].["+ComboRegion.SelectedValue+"]";
                chart2_region.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Сибирский федеральный округ].[Новосибирская область].datamember";
                groupType.Value = "По городам и районам";
            }
            if (Year.SelectedIndex != 0)
            {
                prevPeriod.Value = Year.SelectedNode.PrevNode.Text.Split(' ')[0];
            }
            else
            {
                prevPeriod.Value = (int.Parse(Year.SelectedNode.Text.Split(' ')[0])-1).ToString();
            }
            if (Year.GetLastNode(0).Text == Year.SelectedNode.Text)
            {
                nextPeriod.Value = (int.Parse(Year.SelectedNode.Text.Split(' ')[0]) + 1).ToString();
            }
            else
            {
                nextPeriod.Value = Year.SelectedNode.NextNode.Text.Split(' ')[0];
            }
            headerLayout = new GridHeaderLayout(Grid);
            Grid.DataBind();
            FormDynamicText();
            SetSfereparam();
            selectedPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[Коэффициент рождаемости]";
            selectedPokRF.Value = "Число родившихся на 1000 человек населения";
          /*  string buf = selectedRegion.Value;
            if (ComboRegion.SelectedIndex != 0)
            {
                selectedRegion.Value += ",[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Сибирский федеральный округ].[Новосибирская область]";
            }*/
            Chart2.DataBind();
           // selectedRegion.Value = buf;
            Chart1.DataBind();
            Label1.Text = String.Format(chart1_caprion, ComboRegion.SelectedValue);
            Label2.Text = String.Format(chart2_caprion, "Коэффициент рождаемости");
            Chart2.Tooltips.FormatString = "<SERIES_LABEL>,<br><ITEM_LABEL><br><b><DATA_VALUE:# ##0.##></b>";
		}

        Dictionary<string, int> YearsLoad(string sql)
        {
            CellSet cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 0; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption + " год", 0);
            }
            return d;
        }

        Dictionary<string, int> RegionsLoad(string sql)
        {
            CellSet cs;
            if (RegionSettings.Instance.Id == "HMAO")
            {
                cs = DataProvidersFactory.SpareMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            }
            else
            {
                cs = DataProvidersFactory.PrimaryMASDataProvider.GetCellset(DataProvider.GetQueryText(sql));
            }
            Dictionary<string, int> d = new Dictionary<string, int>();

            d.Add(cs.Axes[1].Positions[0].Members[0].Caption, 0);
            for (int i = 1; i <= cs.Axes[1].Positions.Count - 1; i++)
            {
                d.Add(cs.Axes[1].Positions[i].Members[0].Caption, 1);
            }
            return d;
        }

        protected double GetNumber(string s)
        {
            try
            {
                if (!String.IsNullOrEmpty(s))
                {
                    return double.Parse(s);
                }
                else
                {
                    return 0;
                }
            }
            catch { return 0; }
        }

        protected void FormDynamicText()
        {
            DynamicText.Text = "";
            string s1 = "{0} <b>{1}</b> года – <b>{2}</b> {3}<br>";
            string s2 = "{0} в <b>{1}</b> году – <b>{2}</b> {3}<br>";
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("dynamicText"), "Показатель", dt);
            if (dt.Rows.Count < 1)
            {
                DynamicText.Text = "Нет данных";
            }
            else
            {
                TextTable.Visible = true;
                if (GetNumber(dt.Rows[0][1].ToString()) != 0)
                {
                    DynamicText.Text += String.Format(s1, dt.Rows[0][0].ToString(), Year.SelectedValue.Split(' ')[0], String.Format("{0:### ### ##0.##}", GetNumber(dt.Rows[0][1].ToString())), dt.Rows[0][2].ToString().ToLower());
                }
                if (GetNumber(dt.Rows[1][1].ToString()) != 0)
                {
                    DynamicText.Text += String.Format(s2, dt.Rows[1][0].ToString(), prevPeriod.Value, String.Format("{0:### ### ##0.##}", GetNumber(dt.Rows[1][1].ToString())), dt.Rows[1][2].ToString());
                }
                if (GetNumber(dt.Rows[2][1].ToString()) != 0)
                {
                    DynamicText.Text += String.Format(s2, dt.Rows[2][0].ToString(), prevPeriod.Value, String.Format("{0:### ### ##0.##}", GetNumber(dt.Rows[2][1].ToString())), dt.Rows[2][2].ToString());
                }
                if (GetNumber(dt.Rows[3][1].ToString()) != 0)
                {
                    DynamicText.Text += String.Format(s1, dt.Rows[3][0].ToString(), Year.SelectedValue.Split(' ')[0], String.Format("{0:### ### ##0.##}", GetNumber(dt.Rows[3][1].ToString())), String.Empty);
                }
                if (GetNumber(dt.Rows[4][1].ToString()) != 0)
                {
                    DynamicText.Text += String.Format(s1, dt.Rows[4][0].ToString(), Year.SelectedValue.Split(' ')[0], String.Format("{0:### ### ##0.##}", GetNumber(dt.Rows[4][1].ToString())), String.Empty);
                }
                if (DynamicText.Text == "")
                {
                    TextTable.Visible = false;
                }
            }
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            Grid.Columns.Clear();
            Grid.Bands.Clear();
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Grid"), "Показатель", dt);
            if (dt.Rows.Count < 1)
            {
                Grid.DataSource = null;
            }
            else
            {
                Grid.DataSource = dt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double colWidth = 0;
            if (IsSmallResolution)
            {
                colWidth = 0.13;
            }
            else
            {
                colWidth = 0.1;
            }
            headerLayout.childCells.Clear();
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            e.Layout.HeaderStyleDefault.Wrap = true;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.25);
            e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * colWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
            //    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                headerLayout.AddCell("январь - "+e.Layout.Bands[0].Columns[i].Key.ToLower()+" "+Year.SelectedValue.Split(' ')[0]+ " года");
            }

            headerLayout.ApplyHeaderInfo();
        }

        #region Добавление checkbox
        static int ra = 1;
        void SetSfereparam()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            string[] buttonNames = new string[4];
            buttonNames[0] = "Коэффициент рождаемости";
            buttonNames[1] = "Коэффициент смертности";
            buttonNames[2] = "Коэффициент естественного прироста (убыли)";
            buttonNames[3] = "Коэффициент миграционного прироста (убыли)";
            if (PlaceHolder1.Controls.Count != 0)
            {
                RadioButton r1 = (RadioButton)(PlaceHolder1.Controls[0]);
                if (r1.Text != buttonNames[0])
                {
                    PlaceHolder1.Controls.Clear();
                    for (int i = 0; i < buttonNames.Length; i++)
                    {
                        Random r = new Random();
                        ra = ra++; 
                        RadioButton rb = new RadioButton();
                        rb.Style.Add("font-size", "10pt");
                        rb.ID = "s" + ra.ToString() + "a" + i.ToString();//CRHelper.GetRandomColor().A.ToString() + CRHelper.GetRandomColor().B.ToString();
                        rb.Style.Add("font-family", "Verdana");
                        PlaceHolder1.Controls.Add(rb);
                        Label l = new Label();
                        l.Text = "<br>";
                        PlaceHolder1.Controls.Add(l);
                        rb.Text = buttonNames[i];
                        rb.GroupName = "sfere" + ra.ToString();
                        rb.ValidationGroup = rb.GroupName;
                        rb.CheckedChanged += new EventHandler(RadioButton1_CheckedChanged);
                        rb.AutoPostBack = 1 == 1;
                        rb.Checked = 1 == 2;
                    }
                    ((RadioButton)(PlaceHolder1.Controls[0])).Checked = true;
                }
            }
            else
            {
                PlaceHolder1.Controls.Clear();
                for (int i = 0; i < buttonNames.Length; i++)
                {
                    Random r = new Random();
                    ra = ra++;
                    RadioButton rb = new RadioButton();
                    rb.Style.Add("font-size", "10pt");

                    rb.ID = "s" + ra.ToString() + "a" + i.ToString();//CRHelper.GetRandomColor().A.ToString() + CRHelper.GetRandomColor().B.ToString();
                    rb.Style.Add("font-family", "Verdana");
                    PlaceHolder1.Controls.Add(rb);
                    Label l = new Label();
                    l.Text = "<br>";
                    PlaceHolder1.Controls.Add(l);
                    rb.Text = buttonNames[i];
                    rb.GroupName = "sfere" + ra.ToString();
                    rb.ValidationGroup = rb.GroupName;
                    rb.CheckedChanged += new EventHandler(RadioButton1_CheckedChanged);
                    rb.AutoPostBack = 1 == 1;
                    rb.Checked = 1 == 2;
                }
                ((RadioButton)(PlaceHolder1.Controls[0])).Checked = true;
            }
        }

        protected void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)(sender);
            rb.Checked = 1 == 1;
            selectedPok.Value = "[Население__Естественное движение].[Население__Естественное движение].[Все показатели].[" + rb.Text + "]";
            if (rb.Text.EndsWith("рождаемости"))
            {
                selectedPokRF.Value = "Число родившихся на 1000 человек населения";
            }
            if (rb.Text.EndsWith("смертности"))
            {
                selectedPokRF.Value = "Число умерших на 1000 человек населения";
            }
            if (rb.Text.EndsWith("естественного прироста (убыли)"))
            {
                selectedPokRF.Value = "Коэффициент естественного прироста населения за год, (на 1000 человек населения)";
            }
            if (rb.Text.EndsWith("миграционного прироста (убыли)"))
            {
                selectedPokRF.Value = "Коэффициент миграционного прироста (на 10000 человек населения)";
            }
            string buf = selectedRegion.Value;
            if (ComboRegion.SelectedNode.Level == 0)
            {
                selectedRegion.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Сибирский федеральный округ].[Новосибирская область].datamember";
                chart2_region.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].datamember";
                groupType.Value = "Без группировки";
            }
            else
            {
                selectedRegion.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Сибирский федеральный округ].[Новосибирская область].[" + ComboRegion.SelectedValue + "]";
                chart2_region.Value = "[Территории__Сопоставимый].[Территории__Сопоставимый].[Все территории].[Российская Федерация].[Сибирский федеральный округ].[Новосибирская область].datamember";
                groupType.Value = "По городам и районам";
            }
            
            Chart2.DataBind();
            selectedRegion.Value = buf;
            Label2.Text = String.Format(chart2_caprion, rb.Text);
        }
        #endregion

        DataBaseHelper DBRFHelper;

        class DataBaseHelper
        {
            public DataProvider ActiveDP;

            public DataTable ExecQueryByID(string QueryId)
            {
                return ExecQueryByID(QueryId, QueryId);
            }

            public DataTable ExecQueryByID(string QueryId, string FirstColName)
            {
                string QueryText = DataProvider.GetQueryText(QueryId);
                DataTable Table = ExecQuery(QueryText, FirstColName);
                return Table;
            }

            public DataTable ExecQuery(string QueryText, string FirstColName)
            {
                DataTable Table = new DataTable();
                ActiveDP.GetDataTableForChart(QueryText, FirstColName, Table);
                return Table;
            }

            public DataBaseHelper(DataProvider ActiveDataProvaider)
            {
                this.ActiveDP = ActiveDataProvaider;
            }

        }


        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            if (ComboRegion.SelectedNode.Level == 0)
            {
                DataTable dt = new DataTable();
                DataTable resDt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart2"), "Показатель", dt);
                DataTable TableRFValue = DBRFHelper.ExecQueryByID("ChartRFLine");

                if (dt.Columns.Count <= 1)
                {
                    Chart2.DataSource = null;
                }
                else
                {
                    if (dt.Columns.Count > TableRFValue.Columns.Count)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            resDt.Columns.Add(dt.Columns[i].ColumnName, dt.Columns[i].DataType);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < TableRFValue.Columns.Count; i++)
                        {
                            resDt.Columns.Add(TableRFValue.Columns[i].ColumnName, TableRFValue.Columns[i].DataType);
                        }
                    }
                    object[] o = new object[resDt.Columns.Count];
                    o[0] = "Новосибирская область";

                    for (int i = 1; i < dt.Columns.Count; i++)
                    {
                        o[i] = dt.Rows[1][i];
                    }
                    resDt.Rows.Add(o);
                    o = new object[resDt.Columns.Count];
                    o[0] = "Российская Федерация";
                    for (int i = 1; i < TableRFValue.Columns.Count; i++)
                    {
                        o[i] = TableRFValue.Rows[0][i];
                    }
                    resDt.Rows.Add(o);
                    for (int i = 1; i < resDt.Columns.Count; i++)
                    {
                        resDt.Columns[i].ColumnName = "январь-" + resDt.Columns[i].ColumnName.ToLower() + " " + selectedPeriod.Value + " года";
                    }
                    Chart2.DataSource = resDt;
                    /*
                        foreach (DataRow row in dt.Rows)
                        {
                            row[0] = row[0].ToString().Remove(row[0].ToString().LastIndexOf(';'));
                            row[0] = row[0].ToString().Replace(" ДАННЫЕ)", String.Empty).Replace("(", String.Empty);
                        }
                    try
                    {
                        for (int i = 1; i < dt.Columns.Count; i++)
                        {
                            dt.Rows[0][i] = TableRFValue.Rows[0][i];
                        }
                    }
                    catch { }
                    
                
                
                    Chart2.DataSource = dt;*/
                }
            }
            else
            {
                DataTable dt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart2"), "Показатель", dt);
                foreach (DataRow row in dt.Rows)
                {
                    row[0] = row[0].ToString().Remove(row[0].ToString().LastIndexOf(';'));
                    row[0] = row[0].ToString().Replace(" ДАННЫЕ)", String.Empty).Replace("(", String.Empty);
                }
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    dt.Columns[i].ColumnName = "январь-" + dt.Columns[i].ColumnName.ToLower() + " " + selectedPeriod.Value + " года";
                }
                Chart2.DataSource = dt;
            }
        }
        DataTable Chart1Dt;
        protected void Chart1_DataBinding(object sender, EventArgs e) 
        {
            Chart1Dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart1"), "Показатель", Chart1Dt);
            int n = 1;
            if (Chart1Dt.Columns[1].ColumnName == "Численность")
            {
                double max = 0;
                for (int i = 0; i < Chart1Dt.Rows.Count; i++)
                {
                    if (max < Convert.ToDouble(Chart1Dt.Rows[i][1]))
                    {
                        max = Convert.ToDouble(Chart1Dt.Rows[i][1]);
                    }
                }
                Chart1.Axis.Y.RangeType = AxisRangeType.Custom;
                Chart1.Axis.Y.RangeMax =Math.Round(max * 1.1,0);
                Chart1.Axis.Y.RangeMin =Math.Round(max -max /3,0);
                    /*  for (int i = 0; i < Chart1Dt.Rows.Count; i += 4)
                      {
                          for (int j = 3; j < Chart1Dt.Columns.Count; j++)
                          {
                              Chart1Dt.Rows[i + n][j] = Chart1Dt.Rows[i][j];
                              Chart1Dt.Rows[i][j] = DBNull.Value;
                              n += 1;
                          }
                          n = 1;
                      }*/

                    for (int i = 1; i < Chart1Dt.Columns.Count; i++)
                    {
                        NumericSeries series = CRHelper.GetNumericSeries(i, Chart1Dt);
                        series.Label = Chart1Dt.Columns[i].ColumnName;
                        Chart1.Series.Add(series);
                    }
            }
            else
            {
                Chart1.DataSource = null;
            }
        }

        protected void Chart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (Chart1Dt.Columns[1].ColumnName == "Численность")
            {
                int col = 0;
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive p = e.SceneGraph[i];
                    if (p is Infragistics.UltraChart.Core.Primitives.Box)
                    {
                        Box box = (Box)e.SceneGraph[i];
                        if (p.DataPoint != null)
                        {
                            if (p.DataPoint.Label == "Численность")
                            {
                                box.PE.Fill = Color.Red;
                                box.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                                box.PE.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.SolidFill;
                                box.PE.FillOpacity = 255;
                                string month = Chart1Dt.Rows[box.Row][box.Column].ToString().Split(';')[0];
                                string date="";
                                if (CRHelper.MonthNum(month) < 10)
                                {
                                    date = "01.0" + CRHelper.MonthNum(month).ToString() + "." + Year.SelectedValue.Split(' ')[0];
                                }
                                else
                                {
                                    date = "01." + CRHelper.MonthNum(month).ToString() + "." + Year.SelectedValue.Split(' ')[0];
                                }

                                if (box.Row != 0 && month == "Январь")
                                {
                                    box.DataPoint.Label = "Численность населения<br>на 01.01." + nextPeriod.Value + "<br><b>" + Convert.ToDouble(box.Value) + "</b>, чел";
                                }
                                else
                                {
                                    box.DataPoint.Label = "Численность населения<br>на " + date + "<br><b>" + String.Format("{0:### ### ##0.##}",Convert.ToDouble(box.Value)) + "</b>, чел";
                                }
                            }
                            if (p.DataPoint.Label == "Прирост (убыль) населения")
                            {
                                box.PE.Fill = Color.Black;
                                box.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                                box.PE.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.Hatch;
                                box.PE.Hatch = Infragistics.UltraChart.Shared.Styles.FillHatchStyle.DarkDownwardDiagonal;
                                box.PE.FillStopColor = Color.Red;
                                box.PE.FillOpacity = 255;
                                string month = Chart1Dt.Rows[box.Row][0].ToString().Split(';')[0];
                                string date = "1." + CRHelper.MonthNum(month).ToString() + "." + Year.SelectedValue.Split(' ')[0];
                                if (Convert.ToDouble(box.Value) > 0)
                                {
                                    box.DataPoint.Label = "Прирост населения<br> с 01.01." + Year.SelectedValue.Split(' ')[0] + " по " + date + "<br><b>" + String.Format("{0:### ### ##0.##}", Convert.ToDouble(box.Value)) + "</b>, чел";
                                }
                                else
                                {
                                    box.DataPoint.Label = "Убыль населения<br> с 01.01." + Year.SelectedValue.Split(' ')[0] + " по " + date + "<br><b>" + String.Format("{0:### ### ##0.##}", Convert.ToDouble(box.Value)) + "</b>, чел";
                                }
                            }
                            if (p.DataPoint.Label == "Естественный прирост (убыль)")
                            {
                                box.PE.Fill = Color.Green;
                                box.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                                box.PE.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.SolidFill;
                                box.PE.FillOpacity = 255;
                                string month = Chart1Dt.Rows[box.Row][0].ToString().Split(';')[0];
                                string date = "1." + CRHelper.MonthNum(month).ToString() + "." + Year.SelectedValue.Split(' ')[0];
                                if (Convert.ToDouble(box.Value) > 0)
                                {
                                    box.DataPoint.Label = "Естественный прирост населения<br> с 01.01." + Year.SelectedValue.Split(' ')[0] + " по " + date + "<br><b>" + String.Format("{0:### ### ##0.##}", Convert.ToDouble(box.Value)) + "</b>, чел";
                                }
                                else
                                {
                                    box.DataPoint.Label = "Естественная убыль населения<br> с 01.01." + Year.SelectedValue.Split(' ')[0] + " по " + date + "<br><b>" + String.Format("{0:### ### ##0.##}", Convert.ToDouble(box.Value)) + "</b>, чел";
                                }
                            }
                            if (p.DataPoint.Label == "Миграционный прирост (убыль)")
                            {
                                box.PE.Fill = Color.Blue;
                                box.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                                box.PE.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.SolidFill;
                                box.PE.FillOpacity = 255;
                                string month = Chart1Dt.Rows[box.Row][0].ToString().Split(';')[0];
                                string date = "1." + CRHelper.MonthNum(month).ToString() + "." + Year.SelectedValue.Split(' ')[0];
                                if (Convert.ToDouble(box.Value) > 0)
                                {
                                    box.DataPoint.Label = "Миграционный прирост населения<br> с 01.01." + Year.SelectedValue.Split(' ')[0] + " по " + date + "<br><b>" + String.Format("{0:### ### ##0.##}", Convert.ToDouble(box.Value)) + "</b>, чел";
                                }
                                else
                                {
                                    box.DataPoint.Label = "Миграционная убыль населения<br> с 01.01." + Year.SelectedValue.Split(' ')[0] + " по " + date + "<br><b>" + String.Format("{0:### ### ##0.##}", Convert.ToDouble(box.Value)) + "</b>, чел";
                                }
                            }

                        }
                        else
                        {
                            if (box.Column == 0)
                            {
                                box.PE.Fill = Color.Red;
                                box.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                                box.PE.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.SolidFill;
                                box.PE.FillOpacity = 255;
                            }
                            if (box.Column == 1)
                            {
                                box.PE.Fill = Color.Black;
                                box.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                                box.PE.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.Hatch;
                                box.PE.Hatch = Infragistics.UltraChart.Shared.Styles.FillHatchStyle.DarkDownwardDiagonal;
                                box.PE.FillStopColor = Color.Red;
                                box.PE.FillOpacity = 255;
                            }
                            if (box.Column == 2)
                            {
                                box.PE.Fill = Color.Green;
                                box.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                                box.PE.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.SolidFill;
                                box.PE.FillOpacity = 255;
                            }
                            if (box.Column == 3)
                            {
                                box.PE.Fill = Color.Blue;
                                box.PE.FillGradientStyle = Infragistics.UltraChart.Shared.Styles.GradientStyle.None;
                                box.PE.ElementType = Infragistics.UltraChart.Shared.Styles.PaintElementType.SolidFill;
                                box.PE.FillOpacity = 255;
                            }

                        }
                    }
                    if (p is Infragistics.UltraChart.Core.Primitives.Text)
                    {
                        Text text = (Text)e.SceneGraph[i];
                        if (text.GetTextString().EndsWith("1") || text.GetTextString().EndsWith("2") || text.GetTextString().EndsWith("3") || text.GetTextString().EndsWith("Нарастающий итог"))
                        {
                            if (text.GetTextString().EndsWith("Нарастающий итог"))
                            {
                              //  text.SetTextString(String.Empty);

                                if (text.GetTextString().StartsWith("Январь"))
                                {
                                    col += 1;
                                }
                                if (col == 1)
                                {
                                    if (CRHelper.MonthNum(text.GetTextString().Split(';')[0]) < 10)
                                    {
                                        text.SetTextString("01.0" + CRHelper.MonthNum(text.GetTextString().Split(';')[0]).ToString() + "." + Year.SelectedValue.Split(' ')[0]);
                                    }
                                    else
                                    {
                                        text.SetTextString("01." + CRHelper.MonthNum(text.GetTextString().Split(';')[0]).ToString() + "." + Year.SelectedValue.Split(' ')[0]);
                                    }
                                }
                                else
                                {
                                    if (CRHelper.MonthNum(text.GetTextString().Split(';')[0]) < 10)
                                    {
                                        text.SetTextString("01.0" + CRHelper.MonthNum(text.GetTextString().Split(';')[0]).ToString() + "." + (Convert.ToInt32(Year.SelectedValue.Split(' ')[0]) + 1).ToString());
                                    }
                                    else
                                    {
                                        text.SetTextString("01." + CRHelper.MonthNum(text.GetTextString().Split(';')[0]).ToString() + "." + (Convert.ToInt32(Year.SelectedValue.Split(' ')[0]) + 1).ToString());
                                    }
                                }
                                //text.bounds.Width += 20;
                            }
                            else
                            {
                                
                            }

                        } 
                    }

                }
            }
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
           if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position: 15px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 30px"; }

            if ((e.Row.Index + 1) % 3 == 0)
            {

                Grid.Rows[e.Row.Index - 2].Cells[0].Text = Grid.Rows[e.Row.Index - 2].Cells[0].Text.Split(';')[0] + ", чел.";
                Grid.Rows[e.Row.Index - 1].Cells[0].Text = "";
                e.Row.Cells[0].Text = "";
                Grid.Rows[e.Row.Index - 2].Cells[0].RowSpan = 3;

                Grid.Rows[e.Row.Index - 2].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BackColor = Color.White;
                Grid.Rows[e.Row.Index - 1].Cells[0].Style.BorderDetails.WidthBottom = 0;
                Grid.Rows[e.Row.Index].Cells[0].Style.BorderDetails.WidthBottom = 0;
                e.Row.Cells[0].Style.BackColor = Color.White;
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BorderDetails.WidthBottom = 0;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BorderDetails.WidthBottom = 0;
                    Grid.Rows[e.Row.Index - 2].Cells[i].Style.BackColor = Color.White;
                    Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackColor = Color.White;
                    e.Row.Cells[i].Style.BackColor = Color.White;
                    int n = 0;
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                    {
                        n = CRHelper.MonthNum(Grid.Columns[i].Key);
                        e.Row.Cells[i].Title = "Снижение к январю-" + CRHelper.RusMonthDat(n) + " 2010 года";
                     //   e.Row.Cells[i].Style.CustomRules = style;
                    }
                    if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        n = CRHelper.MonthNum(Grid.Columns[i].Key);
                        e.Row.Cells[i].Title = "Прирост к январю-" + CRHelper.RusMonthDat(n) + " 2010 года";
                    //   e.Row.Cells[i].Style.CustomRules = style;
                    }

                    if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) < 0)
                    {
                        n = CRHelper.MonthNum(Grid.Columns[i].Key);
                        Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Темп снижения к январю-" + CRHelper.RusMonthDat(n) + " 2010 года";
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;
                        if (Grid.Rows[e.Row.Index - 2].Cells[0].Text.EndsWith("Число умерших, чел.") || Grid.Rows[e.Row.Index - 2].Cells[0].Text.EndsWith("Число выбывших, чел."))
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage ="~/images/arrowGreenDownBB.png";
                        }
                        else
                        {
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            
                        }
                    }
                    if (Convert.ToDouble(Grid.Rows[e.Row.Index - 1].Cells[i].Value) > 0)
                    {
                        n = CRHelper.MonthNum(Grid.Columns[i].Key);
                        Grid.Rows[e.Row.Index - 1].Cells[i].Title = "Темп прироста к январю-" + CRHelper.RusMonthDat(n) + " 2010 года";
                        Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = style;
                        if (Grid.Rows[e.Row.Index - 2].Cells[0].Text.EndsWith("Число умерших, чел.") || Grid.Rows[e.Row.Index - 2].Cells[0].Text.EndsWith("Число выбывших, чел."))
                        {

                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                        }
                        else
                        {
                            
                            Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        }
                    }
                    Grid.Rows[e.Row.Index - 1].Cells[i].Value = String.Format("{0:# ##0.00%}", Grid.Rows[e.Row.Index - 1].Cells[i].Value);
                    if (!Grid.Rows[e.Row.Index - 2].Cells[0].Text.Contains("прирост") && !Grid.Rows[e.Row.Index - 2].Cells[0].Text.Contains("Прирост"))
                    {
                        if (e.Row.Cells[i].Value != null)
                        {
                            e.Row.Cells[i].Value = String.Format("{0:N0}", Convert.ToDouble(e.Row.Cells[i].Value));
                        }
                        if (Grid.Rows[e.Row.Index - 2].Cells[i].Value!=null)
                        {
                            Grid.Rows[e.Row.Index - 2].Cells[i].Value = String.Format("{0:N0}", Convert.ToDouble(Grid.Rows[e.Row.Index - 2].Cells[i].Value));
                        }
                    }
                    else
                    {
                        if (e.Row.Cells[i].Value != null)
                        {
                            e.Row.Cells[i].Value = String.Format("{0:# ##0.00}", Convert.ToDouble(e.Row.Cells[i].Value) );
                        }
                        if (Grid.Rows[e.Row.Index - 2].Cells[i].Value != null)
                        {
                         
                            Grid.Rows[e.Row.Index - 2].Cells[i].Value = String.Format("{0:# ##0.00}", Convert.ToDouble(Grid.Rows[e.Row.Index - 2].Cells[i].Value));
                        }
                    }
                }
            }
        }


        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();
            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Infragistics.Documents.Excel.Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма1");
            Infragistics.Documents.Excel.Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма2");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;

            ReportExcelExporter1.TitleStartRow = 0;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.Export(headerLayout, sheet1, 4);

            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            Chart1.Width = 800;
            Chart2.Width = 800;
            ReportExcelExporter1.Export(Chart1, Label1.Text, sheet2, 1);
            ReportExcelExporter1.Export(Chart2, Label2.Text, sheet3, 1);
            
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
           /* if (!emptyReport)
            {
                e.Workbook.Worksheets["Диаграмма"].MergedCellsRegions.Clear();
                e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            }*/
        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text + "\n" + DynamicText.Text.Replace("<br>", "\n") + "\n";
            ReportPDFExporter1.HeaderCellHeight = 30;
            Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();

            Infragistics.Documents.Reports.Report.Section.ISection section1 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section2 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section3 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 60;

            ReportPDFExporter1.Export(headerLayout, section1);
            Chart1.Width = 1000;
            Chart2.Width = 1000;
            ReportPDFExporter1.Export(Chart1, Label1.Text,section2);
            ReportPDFExporter1.Export(Chart2, Label2.Text, section3);

        }
        #endregion

        protected void Chart1_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.Text = "В настоящий момент данные отсутствуют";
            e.LabelStyle.FontColor = System.Drawing.Color.LightGray;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.Font = new System.Drawing.Font("Verdana", 30);
        }

        protected void Chart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive p = e.SceneGraph[i];
                if (p is Infragistics.UltraChart.Core.Primitives.Text)
                {
                    Text text = (Text)e.SceneGraph[i];
                    if (text.GetTextString().EndsWith("года"))
                    {
                        text.SetTextString(text.GetTextString().Insert(text.GetTextString().IndexOf(' '), "\n"));
                    }
                }
            }
        }

	}
}
