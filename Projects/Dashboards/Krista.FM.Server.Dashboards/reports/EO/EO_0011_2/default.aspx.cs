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
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
namespace Krista.FM.Server.Dashboards.reports.EO.EO_0011_2
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Уровень жизни населения";
        private string page_sub_title = "Отчет предназначен для формирование аналитической записки о социально – экономическом положении ";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok")); } }
        private string chart1Caption = "Среднемесячный доход на одного работающего по отраслям, рублей";
        private string chart2Caption = "Величина прожиточного минимума в расчете на душу населения, рублей";
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

            Chart.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            Chart.Height = 300;

            Chart2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            Chart2.Height = 450;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            
            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            Text1.Visible = true;
            Text2.Visible = true;
            TabChart1.Visible = false;
            TabChart2.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            PageTitle.Text = page_title;
            PageSubTitle.Text = page_sub_title;
            Page.Title = page_title;
            baseRegion.Value = RegionSettingsHelper.Instance.RegionBaseDimension;

            SetSfereparam();
            FormText();
            Chart.DataBind();
            ChartCaption.Text = chart1Caption;
            Chart_2Caption.Text = chart2Caption;
            Chart2.DataBind();
        }


        protected void FormText() 
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("DynamicText"), "Text", dt);
            for (int i = 1; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.Split(';')[0];
            }
            string s1 = "&nbsp;&nbsp;&nbsp;&nbsp;При существующих условиях жизни основным источником доходов населения по-прежнему остается заработная плата, которая составила {0} руб.<br>";
            string s2 = "&nbsp;&nbsp;&nbsp;&nbsp;Размер среднемесячной номинальной начисленной заработной платы на одного работника по кругу отчитывающихся крупных и средних организаций города  составил {0} руб., при этом величина прожиточного минимума трудоспособного населения составила {1} руб. <br>";
            string s3 = "&nbsp;&nbsp;&nbsp;&nbsp;Среднемесячный доход на одного работающего составил в отчетном периоде {0} руб., что {1}  аналогичного периода предыдущего года на {2}.<br>";
            string s4 = "&nbsp;&nbsp;&nbsp;&nbsp;Просроченная задолженность по заработной плате по состоянию с начала года  составила {0} тыс.руб., {1} по сравнению с аналогичным периодом прошлого года на  {2}  %. <br>";
            string s5 = "&nbsp;&nbsp;&nbsp;&nbsp;Средний размер назначенных месячных пенсий с начала года {0} на {1} по сравнению с аналогичным периодом предыдущего года и составил {2} рубля.<br>";
            string s6 = "&nbsp;&nbsp;&nbsp;&nbsp;Одним из показателей, позволяющих оценить уровень жизни населения, является величина прожиточного минимума, которая  в расчете на душу населения с начала года составила {0} руб. и {1} по сравнению с аналогичным периодом прошлого года на {2}. В разрезе социально-демографических групп населения самый высокий показатель прожиточного минимума у граждан трудоспособного возраста – {3} руб., для пенсионеров он составил – {4} руб., для детей – {5} руб. <br>";
            DynamicText.Text = "";
            DynamicText2.Text = "";
            DynamicText.Text = String.Format(s1,  String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Среднемесячная заработная плата на 1 работающего")])));
            DynamicText.Text += String.Format(s2, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Среднемесячная заработная плата на 1 работающего крупные предприятия")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Трудоспособное население")])));
            double calc1 = Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Среднемесячный доход на 1 работающего")]) / Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Среднемесячный доход на 1 работающего")])-1;
            
            if (calc1 > 0)
            {
                DynamicText.Text += String.Format(s3, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Среднемесячный доход на 1 работающего")])), "выше",  String.Format("{0:0.##%}", calc1));
            }
            else
            {
                DynamicText.Text += String.Format(s3, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Среднемесячный доход на 1 работающего")])), "ниже", String.Format("{0:0.##%", -1 * calc1));
            }

            double calc2 =( Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Просроченная задолженность по оплате труда")]) / Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Просроченная задолженность по оплате труда")]) )- 1;
            if (calc2 > 0)
            {
                DynamicText2.Text += String.Format(s4, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Просроченная задолженность по оплате труда")])), "увеличилась", String.Format("{0:0.##}", calc2));
            }
            else
            {
                DynamicText2.Text += String.Format(s4, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Просроченная задолженность по оплате труда")])), "снизилась", String.Format("{0:0.##}", -1 * calc2));
            }


            double calc3 = Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Средний размер назначенных месячных пенсий")]) / Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Средний размер назначенных месячных пенсий")])-1;
            if (calc3 > 0)
            {
                DynamicText2.Text += String.Format(s5, "вырос", String.Format("{0:P2}", calc3), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Средний размер назначенных месячных пенсий")])));
            }
            else
            {
                DynamicText2.Text += String.Format(s5,  "снизился", String.Format("{0:P2}", -1 * calc3), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Средний размер назначенных месячных пенсий")])));
            }
             

            double calc4 = Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Все население")]) / Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Все население")])-1;
            if (calc4 > 0)
            {
                DynamicText2.Text += String.Format(s6, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Все население")])), "выросла", String.Format("{0:P2}", calc4), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Трудоспособное население")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Пенсионеры")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Дети")])));
            }
            else
            {
                DynamicText2.Text += String.Format(s6, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Все население")])), "снизилась", String.Format("{0:P2}", calc4), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Трудоспособное население")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Пенсионеры")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Дети")])));
            }
        }

        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "Chart", dt);
            Chart.DataSource = dt;
        }

        #region Добавление checkbox
        static int ra = 1;
        void SetSfereparam()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            string[] buttonNames = new string[3];
            
            buttonNames[0] = "показать только текст";
            buttonNames[1] = "показать только диаграммы";
            buttonNames[2] = "показать текст и диаграммы";
            //buttonNames[4] = "Перинатальная смертность";
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
            if (rb.Text == "показать текст и диаграммы")
            {
                Text1.Visible = true;
                Text2.Visible = true;
                TabChart1.Visible = true;
                TabChart2.Visible = true;
            }
            if (rb.Text == "показать только текст")
            {
                Text1.Visible = true;
                Text2.Visible = true;
                TabChart1.Visible = false;
                TabChart2.Visible = false;
            }
            if (rb.Text == "показать только диаграммы")
            {
                Text1.Visible = false;
                Text2.Visible = false;
                TabChart1.Visible = true;
                TabChart2.Visible = true;
            }
        }
        #endregion

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.SheetColumnCount = 15;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Диаграмма1");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма2");

            Chart.Width = 800;
            Chart2.Width = 800;
            ReportExcelExporter1.Export(Chart, ChartCaption.Text, sheet1, 4);
            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;
            ReportExcelExporter1.Export(Chart2, Chart_2Caption.Text, sheet2, 3);
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

          //  e.Workbook.Worksheets["Диаграмма"].MergedCellsRegions.Clear();
          //  e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            DynamicText.Text = DynamicText.Text.Replace("<br>", "\n").Replace("&nbsp;", " ");
            DynamicText2.Text = DynamicText2.Text.Replace("<br>", "\n").Replace("&nbsp;", " ");
            IText title = section1.AddText();

            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = section1.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = false;
            title.AddContent(PageSubTitle.Text+"\n\n"+DynamicText.Text+"\n");


            title = section2.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(DynamicText2.Text+"\n");

            ReportPDFExporter1.Export(Chart, ChartCaption.Text, section1);
            ReportPDFExporter1.Export(Chart2, Chart_2Caption.Text, section2);


        }
        #endregion

        protected void Chart2_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart2"), "Chart", dt);
            foreach (DataRow row in dt.Rows)
            {
                row[0] = row[row.ItemArray.Length - 1].ToString() + ", " + row[0].ToString();
            }
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            Chart2.DataSource = dt;
        }

        
    }
}