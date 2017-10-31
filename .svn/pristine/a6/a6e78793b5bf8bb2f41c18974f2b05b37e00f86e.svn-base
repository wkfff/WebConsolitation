using System;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using System.Data;
using Infragistics.UltraChart.Shared.Events;
using System.Drawing;


namespace Krista.FM.Server.Dashboards.reports.Default
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private GridHeaderLayout headerLayout;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int layout = 1;

            if (Request.Params["id"] != null)
            {
                DataRow[] rows;
                string id = Request.Params["id"];
                if (id.Contains(".aspx"))
                {
                    id = id.Replace(".aspx", String.Empty);
                    rows = this.AllowedReportsIPhone.Select(String.Format("CODE = '{0}'", id));
                }
                else
                {
                    rows = this.AllowedReports.Select(String.Format("CODE = '{0}'", id));
                }

                Page.Title = rows[0]["NAME"].ToString();
                PageTitle.Text = Page.Title;
                PageSubTitle.Text = rows[0][4].ToString();

                layout = PageSubTitle.Text.Length % 3;

            }

            switch (layout)
            {
                case 0: { SetLayout1(); break; }
                case 1: { SetLayout2(); break; }
                case 2: { SetLayout3(); break; }
            }


            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        private void SetLayout1()
        {
            layout1.Visible = true;
            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.5);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Недостаточно данных для формирования отчета";
        }

        private void SetLayout2()
        {
            layout2.Visible = true;
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Недостаточно данных для формирования отчета";
            UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 120);

            UltraWebGrid3.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid3.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 120);
            UltraWebGrid3.DisplayLayout.NoDataMessage = "Недостаточно данных для формирования отчета";

            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChartInvalidDataReceived);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 120);
            UltraChart.Border.Color = Color.White;
        }

        private void UltraChartInvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Недостаточно данных для формирования отчета";
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
        }

        private void SetLayout3()
        {
            layout3.Visible = true;

            UltraWebGridFF.DisplayLayout.NoDataMessage = "Недостаточно данных для формирования отчета";
            UltraWebGridFF.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 25);
            UltraChartFF1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.550 - 15);
            UltraChartFF2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.450 - 15);

            UltraWebGridFF.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 130);
            UltraChartFF1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 120);
            UltraChartFF2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 120);

            UltraChartFF1.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChartInvalidDataReceived);
            UltraChartFF2.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChartInvalidDataReceived);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            headerLayout = new GridHeaderLayout();
        }


        #region экспорт


        #region экспорт в Excel


        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в Pdf


        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);


        }

        #endregion

        #endregion
    }
}