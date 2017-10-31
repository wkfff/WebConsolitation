﻿using System;
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
namespace Krista.FM.Server.Dashboards.reports.EO.EO_0011_4
{
    public partial class _default : CustomReportPage
    {
        private string page_title = "Промышленное производство";
        private string page_sub_title = "Отчет предназначен для формирование аналитической записки о социально – экономическом положении ";
        private CustomParam baseRegion { get { return (UserParams.CustomParam("baseRegion")); } }
        private CustomParam selectedPok { get { return (UserParams.CustomParam("selectedPok")); } }

        private string chart1Caption = "Структура промышленного производства";
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
            Chart.Tooltips.FormatString = "<ITEM_LABEL>, <b><DATA_VALUE:### ##0.##></b> млн.руб.";
           
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
            ChartCaption.Text = chart1Caption;
            
        }


        protected void FormText()
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("DynamicText"), "Text", dt);
            string s1 = "&nbsp;&nbsp;&nbsp;&nbsp;С начала года объём отгруженных товаров промышленного производства по городу составил {0} млн.рублей, на {1} {2} показателя соответствующего периода предыдущего года, в том числе:<br>";
            string s2 = "&nbsp;&nbsp;&nbsp;&nbsp;•	«добыча полезных ископаемых и предоставление услуг в этой области» - {0} млн.рублей, или {1}  общего объема промышленного производства;<br>";
            string s3 = "&nbsp;&nbsp;&nbsp;&nbsp;•	«обрабатывающее производство» – {0} млн.рублей, или {1};<br>";
            string s4 = "&nbsp;&nbsp;&nbsp;&nbsp;•	«производство и распределение электроэнергии, газа и воды» – {0} млн. рублей, или {1}.<br>";
            string s5 = "&nbsp;&nbsp;&nbsp;&nbsp;Средний размер назначенных месячных пенсий с начала года {0} на {1} по сравнению с аналогичным периодом предыдущего года и составил {2} рубля.<br>";
            string s6 = "&nbsp;&nbsp;&nbsp;&nbsp;Одним из показателей, позволяющих оценить уровень жизни населения, является величина прожиточного минимума, которая  в расчете на душу населения с начала года составила {0} руб. и {1} по сравнению с аналогичным периодом прошлого года на {2}. В разрезе социально-демографических групп населения самый высокий показатель прожиточного минимума у граждан трудоспособного возраста – {3} руб., для пенсионеров он составил – {4} руб., для детей – {5} руб. <br>";
            DynamicText.Text = "";

            double calc1 = (Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Объем промышленного производства")]) / Convert.ToDouble(dt.Rows[0][dt.Columns.IndexOf("Объем промышленного производства")]) - 1);

            if (calc1 > 0)
            {
                DynamicText.Text += String.Format(s1, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Объем промышленного производства")])), String.Format("{0:0.##%}", calc1), "выше");
            }
            else
            {
                DynamicText.Text += String.Format(s1, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Объем промышленного производства")])), String.Format("{0:0.##%", -1 * calc1), "ниже");
            }

            double calc2 = (Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Добыча топливно-энергетических полезных ископаемых")]) / Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Объем промышленного производства")]));
            DynamicText.Text += String.Format(s2, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Добыча топливно-энергетических полезных ископаемых")])), String.Format("{0:0.##%}", calc2));

            double calc3 = (Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Обрабатывающие производства")]) / Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Объем промышленного производства")]));
            DynamicText.Text += String.Format(s3, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Обрабатывающие производства")])), String.Format("{0:P2}", calc3));

            double calc4 = (Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Производство и распределение электроэнергии, газа и воды")]) / Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Объем промышленного производства")]));
            DynamicText.Text += String.Format(s4, String.Format("{0:0.##}", Convert.ToDouble(dt.Rows[1][dt.Columns.IndexOf("Производство и распределение электроэнергии, газа и воды")])), String.Format("{0:P2}", calc4));
            
        }

        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("Chart"), "Chart", dt);
            foreach (DataRow row in dt.Rows)
            {
                row[0] = "     " + row[row.ItemArray.Length - 1].ToString() + ",\n " + row[0].ToString();
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
            ReportExcelExporter1.Export(Chart, ChartCaption.Text, sheet1, 4);
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
            title.AddContent(PageTitle.Text);

            title = section1.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = false;
            title.AddContent(PageSubTitle.Text + "\n\n" + DynamicText.Text + "\n");

            ReportPDFExporter1.Export(Chart, ChartCaption.Text, section1);

            
        }
        #endregion

       


    }
}