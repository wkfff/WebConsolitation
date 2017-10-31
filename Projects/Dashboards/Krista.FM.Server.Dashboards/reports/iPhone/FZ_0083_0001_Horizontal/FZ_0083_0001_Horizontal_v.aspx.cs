using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web;
using System.Drawing;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FZ_0083_0001_Horizontal_v : CustomReportPage
    {
        private GridHeaderLayout headerLayout;
        private GridHeaderLayout headerLayout1;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            //PageTitle.Text = "Сведения о сроках переходного периода по 83ФЗ";
            //Page.Title = PageTitle.Text;
            //PageSubTitle.Text = "Сроки окончания действия переходных положений по введению в действие Федерального закона от 8 мая 2010г. N 83-ФЗ «О внесении изменений в отдельные законодательные акты Российской Федерации в связи с совершенствованием правового положения государственных (муниципальных) учреждений»";


            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);

            UltraWebGrid1.Bands.Clear();

            DataTable dtGrid = LoadIndicators();
            DataTable dtGrid2 = GetSecondGridDataSource(dtGrid);

            UltraWebGrid1.DataSource = dtGrid2;
            UltraWebGrid1.DataBind();

            //lbGrid1Caption.Text = "Сроки окончания действия переходных положений по 83ФЗ";

        }

        #region Обработчики грида

        private DataTable LoadIndicators()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/FZ_0083_0001/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds.Tables["table"];
        }

        private DataTable GetSecondGridDataSource(DataTable dtGrid)
        {
            DataTable dtGrid2 = new DataTable();

            DataColumn col = new DataColumn("Территория");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Финансирование бюджетных учреждений по бюджетной смете");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями доходов от сдачи в аренду имущества Казенные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями доходов от сдачи в аренду имущества Бюджетные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями средств от приносящей доход деятельности Казенные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Использование учреждениями средств от приносящей доход деятельности Бюджетные");
            dtGrid2.Columns.Add(col);

            col = new DataColumn("Основание");
            dtGrid2.Columns.Add(col);

            foreach (DataRow row in dtGrid.Rows)
            {
                DataRow newRow = dtGrid2.NewRow();
                newRow["Территория"] = row["territory"];
                newRow["Основание"] = row["npa"] != DBNull.Value && row["npa"].ToString() != String.Empty ?
                            String.Format("<a href='{0}'>{1}</a>", row["npa"], row["hint"]) :
                            row["hint"];

                for (int i = 1; i < dtGrid.Columns.Count - 1; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        string value = row[i].ToString();
                        if (value.Contains("A"))
                        {
                            SetNewRowValue(newRow, i, 1);
                        }
                        if (value.Contains("B1"))
                        {
                            SetNewRowValue(newRow, i, 2);
                        }
                        if (value.Contains("B2"))
                        {
                            SetNewRowValue(newRow, i, 3);
                        }
                        if (value.Contains("C1"))
                        {
                            SetNewRowValue(newRow, i, 4);
                        }
                        if (value.Contains("C2"))
                        {
                            SetNewRowValue(newRow, i, 5);
                        }
                    }
                }
                dtGrid2.Rows.Add(newRow);
            }
            return dtGrid2;
        }

        private static void SetNewRowValue(DataRow newRow, int i, int cellIndex)
        {
            if (i == 1)
            {
                newRow[cellIndex] = "01.01.2011";
            }
            if (i == 3)
            {
                newRow[cellIndex] = "01.04.2011";
            }
            if (i == 5)
            {
                newRow[cellIndex] = "01.10.2011";
            }
            if (i == 7)
            {
                newRow[cellIndex] = "01.01.2012";
            }
            if (i == 9)
            {
                newRow[cellIndex] = "01.07.2012";
            }
        }

       
        private int GetTodayColumnIndex()
        {
            if (new DateTime(2012, 1, 1) < DateTime.Now)
            {
                return 8;
            }
            if (new DateTime(2011, 10, 1) < DateTime.Now)
            {
                return 6;
            }
            if (new DateTime(2011, 4, 1) < DateTime.Now)
            {
                return 4;
            }
            if (new DateTime(2011, 1, 1) < DateTime.Now)
            {
                return 2;
            }
            return 9;
        }


        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90);
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(160);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(360);
            e.Layout.Bands[0].Columns[6].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[6].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[4].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            headerLayout1.AddCell("Территория");

            GridHeaderCell cell = headerLayout1.AddCell("Срок окончания действия переходных положений");
            cell.AddCell("Перевод всех бюджетных учреждений на субсидии");
            GridHeaderCell childCell = cell.AddCell("Окончание действия временного порядка использования средств от приносящей доход деятельности");
            childCell.AddCell("Казенн. учрежд.");
            childCell.AddCell("Бюджетн. учрежд.");
            childCell = cell.AddCell("Окончание действия временного порядка использования доходов от аренды");
            childCell.AddCell("Казенн. учрежд.");
            childCell.AddCell("Бюджетн. учрежд.");
            headerLayout1.AddCell("Основание");

            headerLayout1.ApplyHeaderInfo();
        }

        private Dictionary<string, int> GetKindsDictionary()
        {
            Dictionary<string, int> kinds = new Dictionary<string, int>();
            kinds.Add("Все", 0);
            kinds.Add("Финансирование бюджетных учреждений по бюджетной смете", 0);
            kinds.Add("Использование учреждениями доходов от сдачи в аренду имущества", 0);
            kinds.Add("Использование учреждениями средств от приносящей доход деятельности", 0);
            return kinds;
        }

        #endregion

        /*    #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion*/
    }
}