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
namespace Krista.FM.Server.Dashboards.reports.EO.EO_0011
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Демография";
        private string page_sub_title = "Отчет предназначен для формирование аналитической записки о социально – экономическом положении";
        private string chart_caption = "Естественный и миграционный прирост";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok")); } }
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
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            ReportExcelExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            Text.Visible = true;
            TabChart.Visible = false;
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
            DemChartCaption.Text = chart_caption;
        }


        protected void FormText()
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("DynamicText"), "Text", dt);
            string s1 = "&nbsp;&nbsp;&nbsp;&nbsp;На начало {0} года в муниципальном образовании зарегистрировано {1} чел.<br>";
            string s2 = "&nbsp;&nbsp;&nbsp;&nbsp;С начала года в городе зарегистрировано {0}  новорожденных, это на {1}  младенцев {2} ({3} {4}), чем в аналогичном периоде прошлого года. В расчете на 1000 человек населения коэффициент рождаемости составил {5}.<br>";
            string s3 = "&nbsp;&nbsp;&nbsp;&nbsp;Число умерших по сравнению с отчетным периодом {0} года {1} на {2} человек и составило {3}  человек. В расчете на 1000 человек населения общий коэффициент смертности составил {4} (в аналогичном периоде прошлого года  – {5}).<br>";
            string s4 = "&nbsp;&nbsp;&nbsp;&nbsp;Естественный прирост населения с начала года составил {0} человека против {1} человек в {2} году.<br>";
            string s5 = "&nbsp;&nbsp;&nbsp;&nbsp;Другим фактором, влияющим на формирование численности населения города, является миграция. В {0} году число {1},";
            string s6 = "в результате чего миграционный прирост населения составил {0} человек, что {1} уровня {2} года на {3} человек.<br>";
            string s7 = "&nbsp;&nbsp;&nbsp;&nbsp;Органами ЗАГС по состоянию с начала года зарегистрировано {0} браков и {1} разводов. В расчете на 1000 человек населения приходится {2} браков и {3} развода.<br>";
            string s8 = "&nbsp;&nbsp;&nbsp;&nbsp;В результате {0} уровня миграции,";
            string s9 = "{0},";
            string s10 = "численность постоянного населения города {0} на {1} человек и по состоянию на {2} год составила {3} человек.";
            bool flag = true;

            for (int i = 0; i < dt.Rows.Count - 1; i++)
            {
                if (dt.Rows[i][0].ToString() != dt.Rows[dt.Rows.Count - 1][0].ToString())
                {
                    dt.Rows.Remove(dt.Rows[i]);
                }
                else
                {
                    if (int.Parse(dt.Rows[i][1].ToString()) != (int.Parse(dt.Rows[dt.Rows.Count - 1][1].ToString()) - 1))
                    {
                        dt.Rows.Remove(dt.Rows[i]);
                    }
                } 
            }
            DynamicText.Text = ""; 

            DynamicText.Text=String.Format(s1,dt.Rows[1][1], String.Format("{0:0.##}",Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность населения на начало периода")])));
            double calc1=Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность родившихся")])-Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Численность родившихся")]);
            double calc2=Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность родившихся")])/Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Численность родившихся")])-1;
            if (calc1>0)
            {
                DynamicText.Text += String.Format(s2, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность родившихся")])), String.Format("{0:0.##}", calc1), "больше", "рост", String.Format("{0:0.##%}", calc2), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Коэффициент рождаемости")])));
            }
            else
            {
                DynamicText.Text += String.Format(s2, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность родившихся")])), String.Format("{0:0.##}", -1 * calc1), "меньше", "снижение", String.Format("{0:0.##%}", calc2), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Коэффициент рождаемости")])));
            }

            double calc3=Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность умерших")])-Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Численность умерших")]);
            if (calc3>0)
            {
                DynamicText.Text += String.Format(s3, dt.Rows[0][1].ToString(), "увеличилось", String.Format("{0:0.##}", calc3), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность умерших")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Коэффициент смертности")])),String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Коэффициент смертности")])));
            }
            else
            {
                DynamicText.Text += String.Format(s3, dt.Rows[0][1].ToString(), "уменьшилось", String.Format("{0:0.##}", -1 * calc3), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность умерших")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Коэффициент смертности")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Коэффициент смертности")])));
            }

            DynamicText.Text += String.Format(s4, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Естественный прирост")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Естественный прирост")])),dt.Rows[0][1].ToString());
            double calc4=Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность убывших")])-Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Миграционный прирост")]);
            if (Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Миграционный прирост")]) > 0)
            {
                DynamicText.Text += String.Format(s5, dt.Rows[1][1].ToString(), "прибывших граждан превысило число убывших");
            }
            else
            {
                DynamicText.Text += String.Format(s5, dt.Rows[1][1].ToString(), "убывших граждан превысило число прибывших");
            }

            double calc5 = Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Миграционный прирост")]) - Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Миграционный прирост")]);
            if (calc5 > 0)
            {
                DynamicText.Text += String.Format(s6, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Миграционный прирост")])), "выше", dt.Rows[0][1].ToString(), String.Format("{0:0.##}", calc5));
            }
            else
            {
                DynamicText.Text += String.Format(s6, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Миграционный прирост")])), "ниже", dt.Rows[0][1].ToString(), String.Format("{0:0.##}", -1 * calc5));
            }

            DynamicText.Text += String.Format(s7, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Количество заключенных браков")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Количество расторгнутых браков")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Коэффициент брачности")])), String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Коэффициент разводов")])));
            if (Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Миграционный прирост")]) < 0)
            {
                DynamicText.Text += String.Format(s8,"отрицательного");
            }
            else
            {
                DynamicText.Text += String.Format(s8, "положительного");
            }
            
            double calc6=Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность родившихся")]) - Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность умерших")]);;
            if (Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Естественный прирост")]) > 0)
            {
                DynamicText.Text += String.Format(s9,"превышения рождаемости над смертностью");
            }
            else
            {
                DynamicText.Text += String.Format(s9, "превышения смертности над рождаемостью");
            }

            double calc7 = Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность населения на конец периода")]) - Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность населения на начало периода")]); ;
            if (calc7 > 0)
            {
                DynamicText.Text += String.Format(s10, "увеличилась", String.Format("{0:0.##}", calc7), dt.Rows[1][1].ToString(), Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность населения на конец периода")]));
            }
            else
            {
                DynamicText.Text += String.Format(s10, "уменьшилась", String.Format("{0:0.##}", calc7), dt.Rows[1][1].ToString(), Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Численность населения на конец периода")]));
            }

        }

        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "Chart", dt);
            foreach (DataRow row in dt.Rows)
            {
                row[0] = row[row.ItemArray.Length - 1].ToString() + ", " + row[0].ToString();
            }
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
            Chart.DataSource = dt;
        }

        protected void LifeLevelChart_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("LifeLevelChart"), "Chart", dt);
            foreach (DataRow row in dt.Rows)
            {
                row[0] = row[row.ItemArray.Length - 1].ToString() + ", " + row[0].ToString();
            }
            dt.Columns.RemoveAt(dt.Columns.Count - 1);
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
                Text.Visible = true;
                TabChart.Visible = true;
            }
            if (rb.Text == "показать только текст")
            {
                Text.Visible = true;
                TabChart.Visible = false;
            }
            if (rb.Text == "показать только диаграммы")
            {
                Text.Visible = false;
                TabChart.Visible = true;
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
            Worksheet sheet1 = workbook.Worksheets.Add("Диаграмма");

            Chart.Width = 800;
            ReportExcelExporter1.Export(Chart, DemChartCaption.Text, sheet1, 4);
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

            
        }
        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            Report report = new Report();

            ISection section1 = report.AddSection();

            DynamicText.Text = DynamicText.Text.Replace("<br>", "\n").Replace("&nbsp;", " ");
            IText title = section1.AddText();

            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text );

            title = section1.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = false;
            title.AddContent(PageSubTitle.Text + "\n\n"+DynamicText.Text+"\n\n");

            ReportPDFExporter1.Export(Chart, DemChartCaption.Text, section1);
        }
        #endregion
    }
}