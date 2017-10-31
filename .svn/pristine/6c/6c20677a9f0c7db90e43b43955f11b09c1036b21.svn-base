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
using Dundas.Maps.WebControl;

using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Image = Infragistics.Documents.Reports.Graphics.Image;

namespace Krista.FM.Server.Dashboards.reports.SEP_0004_ComplexSahalin
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Интегральная оценка уровня и динамики развития муниципальных образований по сферам социально-экономического развития";
        private string page_sub_title = "Интегральные показатели уровня и динамики развития социальной, экономической и финансовой сфер в разрезе муниципальных образований";
        private string chart_caption = "{0}, {1}";
        private string chart_caption2 = "Интегральные показатели уровня и динамики развития сфер, {0}, {1}";
        private string map_caption = "{0}, {1}";
        private CustomParam weightKoef { get { return (UserParams.CustomParam("weightKoef")); } }
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedPeriod { get { return (UserParams.CustomParam("selectedPeriod")); } }
        private CustomParam selectedRegion { get { return (UserParams.CustomParam("selectedRegion")); } }
        private CustomParam selectedMeasure { get { return (UserParams.CustomParam("selectedMeasure")); } }
        private CustomParam selectedArea { get { return (UserParams.CustomParam("selectedArea")); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok")); } }
        private string legendTitle = "";
        private string mapFolderName = "";
        private bool flag=false;
        private GridHeaderLayout headerLayout;
        int[] rankColumns;
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.Towns.ToString();
        }
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
            Year.Width = 250;
            ComboRegion.Width = 400;
            G.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            Chart1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            DundasMap.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15 - 5);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight-30); 
            G.Height = Unit.Empty;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        } 

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                Year.FillDictionaryValues(YearsLoad("years"));
                Year.Title = "Период";
                Year.ParentSelect = true;
                Year.SelectLastNode();

                ComboRegion.FillDictionaryValues(RegionLoad("regions"));
                ComboRegion.Title = "Территория";
            }
            mapFolderName = "Субъекты/Сахалин";
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;
            PageTitle.Text = page_title;
            Page.Title = page_title;
            
            if (Year.SelectedNode.Level == 0)
            {
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Text.Split(' ')[0] + "].[Полугодие 2].[Квартал 4]";
               // prevPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + (int.Parse(Year.SelectedNode.Text.Split(' ')[0]) - 1).ToString() + "].[Полугодие 2].[Квартал 4]";
                weightKoef.Value = "Годовой";
                PageSubTitle.Text = page_sub_title + ", " + Year.SelectedValue.ToLower();
                Label2.Text = String.Format(map_caption, "Интегральный показатель уровня развития, социальная сфера", Year.SelectedValue.ToLower());
            }
            else
            {
                int half_num = CRHelper.HalfYearNumByQuarterNum(int.Parse(Year.SelectedValue.Split(' ')[1]));
                selectedPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + Year.SelectedNode.Parent.Text.Split(' ')[0] + "].[Полугодие " + half_num.ToString() + "].[Квартал " + Year.SelectedValue.Split(' ')[1] + "]";
                weightKoef.Value = "Квартальный";
                PageSubTitle.Text = page_sub_title + ", " + Year.SelectedValue.ToLower().Split(' ')[1] + " квартал " + Year.SelectedNode.Parent.Text.Split(' ')[0] + " года";
                Label2.Text = String.Format(map_caption, "Интегральный показатель уровня развития, социальная сфера", Year.SelectedValue.ToLower().Split(' ')[1] + " квартал " + Year.SelectedNode.Parent.Text.Split(' ')[0] + " года");
               // prevPeriod.Value = "[Период__Период].[Период__Период].[Данные всех периодов].[" + (int.Parse(Year.SelectedNode.Parent.Text.Split(' ')[0]) - 1).ToString() + "].[Полугодие " + half_num.ToString() + "].[Квартал " + Year.SelectedValue.Split(' ')[1] + "]";
            }

            if (ComboRegion.SelectedIndex == 0)
            {
                selectedRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Дальневосточный федеральный округ].[Сахалинская область].[Сахалинская обл.].children";
            } 
            else
            {
                selectedRegion.Value = "[Территории__РФ].[Территории__РФ].[Все территории].[Российская Федерация].[Дальневосточный федеральный округ].[Сахалинская область].[Сахалинская обл.].["+ComboRegion.SelectedValue+"]";
                PageSubTitle.Text += " (" + ComboRegion.SelectedValue + ")";
            }
            headerLayout = new GridHeaderLayout(G);
            G.DataBind(); 
               
            for (int i = 0; i < rankColumns.Length; i++)
            {
                calculateRank(G, rankColumns[i] - 1, rankColumns[i]);
            } 
            SetSfereparam();
           /* selectedArea.Value = "Социальная сфера";
            selectedMeasure.Value = "Индикатор уровня";*/
            Chart1.Tooltips.FormatString = "Интегральный показатель уровня развития, социальная сфера<br> составляет <DATA_VALUE:## ##0.000>";
            Chart1.DataBind();
              
            SetMapSettings();
            
           
        } 

        Dictionary<string, int> YearsLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "years", dt);

            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add(dt.Rows[0][2].ToString() + " год", 0);
            d.Add(dt.Rows[0][0].ToString() + " " + dt.Rows[0][2].ToString() + " года", 1);
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][2].ToString() != dt.Rows[i - 1][2].ToString())
                {
                    d.Add(dt.Rows[i][2].ToString() + " год", 0);
                }
                d.Add(dt.Rows[i][0].ToString() + " " + dt.Rows[i][2].ToString() + " года", 1);
            }
            return d;
        }

        Dictionary<string, int> RegionLoad(string sql)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText(sql), "Region", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("Все территории", 0);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;
        }
        DataTable dtGrid;
        protected void G_DataBinding(object sender, EventArgs e)
        {
            int col = 0;
            G.Columns.Clear();
            G.Bands.Clear();
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("grid"), "Территория", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                bool flag = true;
                for (int i = 1; i < dtGrid.Columns.Count; i+=2)
                {
                    flag = true;
                    for (int j = 0; j < dtGrid.Rows.Count; j++)
                    {
                        if (dtGrid.Rows[j][i] != DBNull.Value)
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        dtGrid.Columns.Remove(dtGrid.Columns[i]);
                       // i -= 1;
                        dtGrid.Columns.Remove(dtGrid.Columns[i]);
                        i -= 2;
                        
                    }
                }
                for (int i = 1; i < dtGrid.Columns.Count; i++)
                {
                    if (dtGrid.Columns[i].ColumnName.Contains("Ранг"))
                    {
                        col += 1;
                    }
                }
                rankColumns = new int[col];
                col = 0;
                for (int i = 1; i < dtGrid.Columns.Count; i++)
                {
                    if (dtGrid.Columns[i].ColumnName.Contains("Ранг"))
                    {
                        rankColumns[col] = i;
                        col += 1;
                    }
                }
                G.DataSource = dtGrid;
            }
            else
            {
                G.DataSource = null;
            } 
        }

        protected void calculateRank(UltraWebGrid Grid, int colNumber, int colRankNumber)
        {
            string style = "";
            if (IsSmallResolution)
            { style = "background-repeat: no-repeat;background-position: 15px"; }
            else
            { style = "background-repeat: no-repeat;background-position: 40px"; }
            int m = 0;
            for (int i = 0; i < Grid.Rows.Count; i++)
            {
                if (MathHelper.IsDouble(Grid.Rows[i].Cells[colNumber].Value) == true)
                {
                    m += 1;

                }
                Grid.Rows[i].Cells[colRankNumber].Text = "0";
            }

            if (m != 0)
            {
                double[] rank = new double[m];
                m = 0;
                for (int i = 0; i < Grid.Rows.Count; i++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[i].Cells[colNumber].Value) == true)
                    {
                        rank[m] = Convert.ToDouble(Grid.Rows[i].Cells[colNumber].Value);
                        m += 1;
                        Grid.Rows[i].Cells[colRankNumber].Text = "0";
                    }
                    else
                    {
                        Grid.Rows[i].Cells[colRankNumber].Text = String.Empty;
                    }

                }
                Array.Sort(rank);

                m = 1;

                for (int i = rank.Length - 1; i >= 0; i--)
                {

                    for (int j = 0; j < Grid.Rows.Count; j++)
                    {
                        if (rank[i] == GetNumber(Grid.Rows[j].Cells[colNumber].Text))
                        {
                            if (Grid.Rows[j].Cells[colRankNumber].Text == "0")
                            {
                                Grid.Rows[j].Cells[colRankNumber].Text = String.Format("{0:0}", m);
                                if ((m) == 1)
                                {

                                    Grid.Rows[j].Cells[colRankNumber].Style.BackgroundImage = "~/images/starYellowBB.png";
                                    Grid.Rows[j].Cells[colRankNumber].Title = "Самое высокое значение";
                                    Grid.Rows[j].Cells[colRankNumber].Style.CustomRules = style;
                                }
                                if (m == rank.Length)
                                {

                                    Grid.Rows[j].Cells[colRankNumber].Style.BackgroundImage = "~/images/starGrayBB.png";
                                    Grid.Rows[j].Cells[colRankNumber].Title = "Самое низкое значение";
                                    Grid.Rows[j].Cells[colRankNumber].Style.CustomRules = style;
                                }
                            }
                        }
                    }
                    if (i != 0)
                    {
                        if (rank[i] != rank[i - 1])
                        {
                            m += 1;
                        }
                    }
                    else
                    { m += 1; }

                }
                double max = GetNumber(Grid.Rows[0].Cells[colRankNumber].Text);
                for (int j = 0; j < Grid.Rows.Count; j++)
                {
                    if (MathHelper.IsDouble(Grid.Rows[j].Cells[colNumber].Value) == true && Grid.Rows[j].Cells[colNumber].Text != String.Empty)
                    {
                        if (GetNumber(Grid.Rows[j].Cells[colRankNumber].Text) > max)
                        {
                            max = GetNumber(Grid.Rows[j].Cells[colRankNumber].Text);
                        }
                    }
                }
                for (int j = 0; j < Grid.Rows.Count; j++)
                {

                    if (GetNumber(Grid.Rows[j].Cells[colRankNumber].Text) == max)
                    {
                        Grid.Rows[j].Cells[colRankNumber].Style.BackgroundImage = "~/images/starGrayBB.png";
                        Grid.Rows[j].Cells[colRankNumber].Title = "Самое низкое значение";
                        Grid.Rows[j].Cells[colRankNumber].Style.CustomRules = style;
                    }

                }
            }

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

        protected void G_InitializeLayout(object sender, LayoutEventArgs e)
        {
            double width = 0;
            if (IsSmallResolution)
            {
                width = 0.1;
            }
            else
            {
                width = 0.07;
            }
            if (dtGrid.Rows.Count > 1)
            {
                headerLayout.childCells.Clear();
                e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
                e.Layout.HeaderStyleDefault.Wrap = true; headerLayout.childCells.Clear();
                e.Layout.AllowSortingDefault = AllowSorting.No;
                e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
                e.Layout.HeaderStyleDefault.Wrap = true;
                e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(minScreenWidth * 0.15);
                e.Layout.Bands[0].Columns[0].Header.Style.Wrap = true;
                e.Layout.Bands[0].Columns[0].Header.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Wrap = 1 == 1;
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(minScreenWidth * width);
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[i].CellStyle.CustomRules = "padding-right:5px";
                    e.Layout.Bands[0].Columns[i].CellStyle.Wrap = 1 == 1;
                    //    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                }

                if (dtGrid.Rows.Count > 1)
                {
                    headerLayout.AddCell("Территория");
                    GridHeaderCell headerCell = null;
                    GridHeaderCell headerCell1 = null;
                    headerCell = headerLayout.AddCell("Интегральный показатель уровня развития сферы");
                    headerCell1 = headerCell.AddCell(e.Layout.Bands[0].Columns[1].Key.Split(';')[1]);
                    headerCell1.AddCell("Значение");
                    headerCell1.AddCell("Ранг");
                    for (int i = 3; i < e.Layout.Bands[0].Columns.Count; i+=2)
                    { 
                        if (e.Layout.Bands[0].Columns[i - 1].Key.Split(';')[0] != e.Layout.Bands[0].Columns[i].Key.Split(';')[0])
                        {
                            headerCell = headerLayout.AddCell("Интегральный показатель динамики развития сферы");
                        }
                        headerCell1 = headerCell.AddCell(e.Layout.Bands[0].Columns[i].Key.Split(';')[1]);
                        headerCell1.AddCell("Значение");
                        headerCell1.AddCell("Ранг");
                    }
                }
                headerLayout.ApplyHeaderInfo();
            }
        }

        protected void G_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Style.BackColor = Color.White;
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BackColor = Color.White;
                if (G.Columns[i].Key.EndsWith("Ранг"))
                {
                    if (e.Row.Cells[i].Value != null)
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N0}", GetNumber(e.Row.Cells[i].Text));
                    }
                }
                else
                {
                    if (e.Row.Cells[i].Value != null)
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}", Convert.ToDouble(e.Row.Cells[i].Value));
                    }
                }

            }
            if (e.Row.Cells[0].Text == ComboRegion.SelectedValue)
            {
                e.Row.Cells[0].Style.ForeColor = Color.Red;
            }
        }

        protected void Chart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            
            if (ComboRegion.SelectedIndex == 0)
            {
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart2"), "Территории", dt);
                foreach (DataRow row in dt.Rows)
                {
                    row[0] = row[0].ToString().Replace("\"","\'");
                }
                Chart1.DataSource = dt;
                if (selectedMeasure.Value=="Индикатор уровня")
                {
                    if (Year.SelectedNode.Level == 0)
                    {
                        Label1.Text = "Интегральный показатель уровня развития, " + selectedArea.Value + ", " + Year.SelectedValue.ToLower();
                    }
                    else
                    {
                        Label1.Text = "Интегральный показатель уровня развития, " + selectedArea.Value + ", " + Year.SelectedValue.ToLower().Split(' ')[1] + " квартал " + Year.SelectedNode.Parent.Text.Split(' ')[0] + " года";
                    }
                }
                else
                {
                    if (Year.SelectedNode.Level == 0)
                    {
                        Label1.Text = "Интегральный показатель динамики развития, " + selectedArea.Value + ", " + Year.SelectedValue.ToLower();
                    }
                    else
                    {
                        Label1.Text = "Интегральный показатель динамики развития, " + selectedArea.Value + ", " + Year.SelectedValue.ToLower().Split(' ')[1] + " квартал " + Year.SelectedNode.Parent.Text.Split(' ')[0] + " года";
                    }
                }
            }
            else
            {
                //DataTable resDt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("chart1"), "Территории", dt);
                Chart1.DataSource = dt;
                Chart1.Tooltips.FormatString = "<ITEM_LABEL><br>составляет <DATA_VALUE:##0.##> ";
                if (Year.SelectedNode.Level == 0)
                {
                    Label1.Text = String.Format(chart_caption2, ComboRegion.SelectedValue, Year.SelectedValue.ToLower());
                }
                else
                {
                    Label1.Text = String.Format(chart_caption2, ComboRegion.SelectedValue, Year.SelectedValue.ToLower().Split(' ')[1] + " квартал " + Year.SelectedNode.Parent.Text.Split(' ')[0] + " года");
                }
            }
           
            
        }

        #region Добавление checksbox
        static int ra = 1;
        void SetSfereparam()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            //DataTable dt = new DataTable();
            int col = G.Columns.Count / 2;
            string[] buttonNames = new string[col];
            col=0;
            for (int i = 1; i < G.Columns.Count; i+=2)
            {
                if (G.Columns[i].Key.Split(';')[0] == "Индикатор уровня")
                {
                    buttonNames[col] = "Интегральный показатель уровня развития, " + G.Columns[i].Key.Split(';')[1].ToLower();
                }
                else 
                {
                    buttonNames[col] = "Интегральный показатель динамики развития, " + G.Columns[i].Key.Split(';')[1].ToLower();
                }
                
                col += 1;
            }
           /*     buttonNames[0] = "Интегральный показатель уровня развития, социальная сфера";
            buttonNames[1] = "Интегральный показатель уровня развития, экономическая сфера";
            buttonNames[2] = "Интегральный показатель уровня развития, финансовая сфера";
            buttonNames[3] = "Интегральный показатель динамики развития, социальная сфера";
            buttonNames[4] = "Интегральный показатель динамики развития, экономическая сфера";
            buttonNames[5] = "Интегральный показатель динамики развития, финансовая сфера";*/
            if (buttonNames[0].Contains("уровня"))
            {
                selectedMeasure.Value = "Индикатор уровня";
            }
            else
            {
                selectedMeasure.Value = "Индикатор динамики";
            }
            selectedArea.Value = buttonNames[0].Split(',')[1].Remove(0, 2);
            legendTitle = buttonNames[0];
            //   DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("AreasChart"), "dfdf", dt);
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
            if (rb.Text.Contains("уровня"))
            {
                selectedMeasure.Value = "Индикатор уровня";
            }
            else
            {
                selectedMeasure.Value = "Индикатор динамики";
            }

            selectedArea.Value = rb.Text.Split(',')[1].Remove(0, 2);
            Chart1.Tooltips.FormatString = rb.Text + "<br> составляет <DATA_VALUE:## ##0.##>";
            Chart1.DataBind();
            
            legendTitle = rb.Text;
            SetMapSettings();
            Label2.Text = String.Format(map_caption, rb.Text, Year.SelectedValue.ToLower());
        }
        #endregion 

        #region Обработчики карты
        DataTable dtMap;
        bool smallLegend = false;
        bool revPok = false;
        double maxMap = 0;
        double minMap = 0;
        double step = 0;
        public void SetMapSettings()
        {


            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeRules.Clear();
            DundasMap.Shapes.Clear();
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.ViewCenter.X += 8;
            DundasMap.Viewport.EnablePanning = true;
            // DundasMap.Viewport.Zoom = 150;
            DundasMap.ColorSwatchPanel.Visible = false;

            DundasMap.Legends.Clear();
            // добавляем легенду
            Legend legend = new Legend("CostLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.Dock = PanelDockStyle.Right;
            legend.DockAlignment = DockAlignment.Far;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = legendTitle;
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);
            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();

            dtMap = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Map"), "Карта", dtMap);

            double[] values = new double[dtMap.Rows.Count];
            maxMap = Convert.ToDouble(dtMap.Rows[0].ItemArray[1]);
            minMap = Convert.ToDouble(dtMap.Rows[0].ItemArray[1]);
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                values[i] = Convert.ToDouble(dtMap.Rows[i].ItemArray[1]);
                if (maxMap < Convert.ToDouble(dtMap.Rows[i].ItemArray[1]))
                {
                    maxMap = Convert.ToDouble(dtMap.Rows[i].ItemArray[1]);
                }
                if (minMap > Convert.ToDouble(dtMap.Rows[i].ItemArray[1]))
                {
                    minMap = Convert.ToDouble(dtMap.Rows[i].ItemArray[1]);
                }
            }
            // Array.Sort(values);


            step = (maxMap - minMap) / 3;
            LegendItem item = new LegendItem();
            item.Text = String.Format(String.Format("{0:##0.00}", minMap) + " - " + String.Format("{0:##0.00}", (minMap + step)));
            item.Color = Color.Red;
            DundasMap.Legends["CostLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = String.Format(String.Format("{0:##0.00}", (minMap + step)) + " - " + String.Format("{0:##0.00}", (minMap + 2 * step)));
            item.Color = Color.Yellow;
            DundasMap.Legends["CostLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = String.Format(String.Format("{0:##0.00}", (minMap + 2 * step)) + " - " + String.Format("{0:##0.00}", maxMap));
            item.Color = Color.Green;
            DundasMap.Legends["CostLegend"].Items.Add(item);
            // добавляем поля
            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Cost");
            DundasMap.ShapeFields["Cost"].Type = typeof(double);
            DundasMap.ShapeFields["Cost"].UniqueIdentifier = false;

            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.CalloutTowns);
            FillMapData();

        }

        public static ArrayList FindMapShape(MapControl map, string patternValue, out bool hasCallout)
        {
            hasCallout = false;
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                //CRHelper.SaveToErrorLog(GetNormalName(GetShapeName(shape).ToLower()) + "|" + GetNormalName(patternValue.ToLower()));
                if (GetNormalName(GetShapeName(shape).ToLower()) == GetNormalName(patternValue.ToLower()))
                {
                    shapeList.Add(shape);
                    if (IsCalloutTownShape(shape))
                    {
                        hasCallout = true;
                    }
                }
            }

            return shapeList;
        }

        private static string GetNormalName(string p)
        {
            return p.Replace(" район", "").Replace(" городской округ", "").Replace("поселок ", "").Replace("\"", "").Replace("городской округ ", "").Replace("г.", "").Replace("г. ", "").Replace(" го", "").Replace(" мр", "").Replace(" ", "");
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Clear();
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        public static string GetShapeName(Shape shape)
        {
            string shapeName = shape.Name;
            if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
            {
                shapeName = shape.Name.Split('_')[0];
            }

            return shapeName;
        }

        void SetShapeTextOffset(Shape Shape)
        {
            string[] Offsets = new string[4] {
                "Углегорский","Холмский","Анивский","Невельский"
            };

            string ShapeName = Shape.Name;
            foreach (string s in Offsets)
            {
                if (ShapeName.Contains(s))
                {
                    Shape.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;

                    if (ShapeName.Contains("Анивский"))
                    {
                        Shape.Text += "        `";
                    }

                    return;
                }
            }
            Shape.TextAlignment = System.Drawing.ContentAlignment.MiddleRight;
        }

        public void FillMapData()
        {
            bool hasCallout;

            string shapeHint = "";


            shapeHint = "{0} "+legendTitle +" {1}";


            Dictionary<string, int> colTown = new Dictionary<string, int>();

            for (int j = 0; j < dtMap.Rows.Count; j++) 
            {

                string subject = dtMap.Rows[j].ItemArray[0].ToString().Replace("Город", "г.");
                string name = dtMap.Rows[j].ItemArray[2].ToString().Replace("р-н",String.Empty);
                double value = Convert.ToDouble(dtMap.Rows[j].ItemArray[1].ToString());
                ArrayList shapeList = FindMapShape(DundasMap, subject, out hasCallout);
                foreach (Shape shape in shapeList)
                {
                    shape.Visible = true;
                    shape["Name"] = subject;

                    shape["Cost"] = Convert.ToDouble(dtMap.Rows[j].ItemArray[1]);
                    shape.ToolTip = String.Format(shapeHint, GetNormalName(subject), String.Format("{0:##0.000}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1])));

                    shape.TextAlignment = System.Drawing.ContentAlignment.TopCenter;

                    shape.Text = GetNormalName(subject) + "\n" + String.Format("{0:##0.000}", Convert.ToDouble(dtMap.Rows[j].ItemArray[1]));
                    shape.Name = GetNormalName(subject);
                    shape.TextVisibility = TextVisibility.Shown;
                    SetShapeTextOffset(shape);
                    if (value <= (minMap + step))
                    {
                        shape.Color = Color.Red;
                        
                    }
                    else
                    {
                        if (value > (minMap + step) && value <= (minMap + 2 * step))
                        {
                            shape.Color = Color.Yellow;
                        }
                        else
                        {
                            if (value > (minMap + 2 * step) && value <= maxMap)
                            {
                                shape.Color = Color.Green;
                            }
                        }
                    }
                }
            }
        }


        #endregion
         
        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Infragistics.Documents.Excel.Workbook workbook = new Infragistics.Documents.Excel.Workbook();

            Infragistics.Documents.Excel.Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Infragistics.Documents.Excel.Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Infragistics.Documents.Excel.Worksheet sheet3 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.HeaderCellFont = new System.Drawing.Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new System.Drawing.Font("Verdana", 11, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new System.Drawing.Font("Verdana", 9);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;

            ReportExcelExporter1.TitleStartRow = 0;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            Chart1.Width = 1000;
            Chart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 7);
            DundasMap.Width = 1000;
            //ReportExcelExporter1.Export(Chart1, Label1.Text, sheet2, 3);

            ReportExcelExporter1.Export(DundasMap, Label2.Text, sheet3, 3);
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
            if (!flag)
            {
                System.Drawing.Image img = UltraGridExporter.GetExcelImage(Chart1);
                WorksheetImage excelImage = new WorksheetImage(img);
                excelImage.TopLeftCornerCell = e.Workbook.Worksheets["Диаграмма"].Rows[3].Cells[0];
                excelImage.BottomRightCornerCell = e.Workbook.Worksheets["Диаграмма"].Rows[3].Cells[0];
                e.Workbook.Worksheets["Диаграмма"].Rows[3].Cells[0].Worksheet.Shapes.Add(excelImage);

                excelImage.SetBoundsInTwips(e.Workbook.Worksheets["Диаграмма"].Rows[3].Cells[0].Worksheet,
                                            new Rectangle(
                                                excelImage.TopLeftCornerCell.GetBoundsInTwips().Left,
                                                excelImage.TopLeftCornerCell.GetBoundsInTwips().Top,
                                                 (int)(15 * Chart1.Width.Value), (int)(15 * Chart1.Height.Value)));
                flag = true;
            }
            e.Workbook.Worksheets["Диаграмма"].Rows[0].Cells[0].Value = PageTitle.Text;
            e.Workbook.Worksheets["Диаграмма"].Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.Font.Name = "Verdana";
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Font.Name = "Verdana";

            e.Workbook.Worksheets["Диаграмма"].Rows[0].Cells[0].CellFormat.Font.Height = 220;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].CellFormat.Font.Height = 180;
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Font.Height = 180;

            e.Workbook.Worksheets["Диаграмма"].Rows[0].Cells[0].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].Value = PageSubTitle.Text;
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].Value = Label1.Text;

            e.Workbook.Worksheets["Диаграмма"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Карта"].MergedCellsRegions.Clear();
            e.Workbook.Worksheets["Карта"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.Workbook.Worksheets["Таблица"].Rows[1].Height = 500;
            e.Workbook.Worksheets["Таблица"].Rows[0].Height = 700;
        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 30;
            Infragistics.Documents.Reports.Report.Report report = new Infragistics.Documents.Reports.Report.Report();

            Infragistics.Documents.Reports.Report.Section.ISection section1 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section2 = report.AddSection();
            Infragistics.Documents.Reports.Report.Section.ISection section3 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 60;
            Chart1.Width = 1000;
            Chart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 7);
            DundasMap.Width = 1100;
            DundasMap.Height = 700;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(Chart1, Label1.Text, section2);
            ReportPDFExporter1.Export(DundasMap, Label2.Text, section3);

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

        protected void Chart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (ComboRegion.SelectedIndex != 0)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                    if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                    {

                        Infragistics.UltraChart.Core.Primitives.Text text = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                        if (text.GetTextString().EndsWith("сфера"))
                        {

                            string pokName = text.GetTextString();
                            if (pokName.Length > 20) 
                            {
                                pokName = pokName.Insert(pokName.IndexOf(' ', 15) + 1, "\n");
                                if (pokName.Length > 30)
                                {
                                    pokName = pokName.Insert(pokName.IndexOf(' ', 30) + 1, "\n");
                                    if (pokName.Length > 40)
                                    {
                                        pokName = pokName.Insert(pokName.IndexOf(' ', 40) + 1, "\n");

                                    }
                                } 
                            }
                            text.SetTextString(pokName);
                            text.labelStyle.Orientation = Infragistics.UltraChart.Shared.Styles.TextOrientation.Horizontal;
                            text.bounds.Width = 156;
                            text.bounds.Height = 70;
                            text.labelStyle.HorizontalAlign = StringAlignment.Center;
                            text.labelStyle.VerticalAlign = StringAlignment.Near;
                            text.bounds.X -= 90;
                        }
                    }
                }
            }
            else
            { 
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                    if (primitive is Infragistics.UltraChart.Core.Primitives.Text)
                    {
                        Infragistics.UltraChart.Core.Primitives.Text text = (Infragistics.UltraChart.Core.Primitives.Text)primitive;
                        if (text.GetTextString() == "Городской округ \'Александровск-Сахалинский район\'")
                        {
                            text.SetTextString("Городской округ\n\'Александровск-Сахалинский район\'");
                            text.bounds.X -= 15;
                            text.bounds.Width += 20;
                        }
                    }
                }
            }
        }
    }
}