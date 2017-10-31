using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using System.Drawing;

namespace Krista.FM.Server.Dashboards.reports.OIV_0001_0001
{
    public partial class Default : CustomReportPage
    {
        private GridHeaderLayout headerLayout;
        DateTime currentDate;

        private bool haveSavePermission
        {
            get
            {
                return Session["IsOivAdministrator"] != null &&
                       (bool)Session["IsOivAdministrator"];
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            UltraWebGrid1.Height = haveSavePermission ? CRHelper.GetGridHeight(600) : CRHelper.GetGridHeight(650);
            UltraWebGrid1.Width = CRHelper.GetGridWidth(1200);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            PopupInformer1.Visible = haveSavePermission;
            SaveRejectButton1.Visible = haveSavePermission;
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value != null &&
                e.Row.Cells[0].Value.ToString().Contains("Итого"))
            {
                e.Row.Style.Font.Bold = true;
            }
                 
        }

        private DataSet LoadData()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/OIV_0001_0001/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            return ds;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!IsPostBack ||
                Request.Form["__EVENTTARGET"] == "rejectButton")
            {
                DataSet ds = LoadData();
                DataTable dtDate = ds.Tables["date"];
                DataTable dtGrbs = ds.Tables["grbsTable"];

                DataTable dtSource = new DataTable();
                dtSource.Columns.Add(new DataColumn("1", typeof(string)));
                dtSource.Columns.Add(new DataColumn("2", typeof(Int32)));
                dtSource.Columns.Add(new DataColumn("3", typeof(Int32)));

                int startYearSumm = 0;
                int summ = 0;

                foreach (DataRow row in dtGrbs.Rows)
                {
                    DataRow newRow = dtSource.NewRow();
                    newRow[0] = row[1];
                    int value;
                    if (Int32.TryParse(row[2].ToString(), out value))
                    {
                        newRow[1] = value;
                        startYearSumm += value;
                    }
                    if (Int32.TryParse(row[3].ToString(), out value))
                    {
                        newRow[2] = value;
                        summ += value;
                    }
                    dtSource.Rows.Add(newRow);
                }
                DataRow totalRow = dtSource.NewRow();
                totalRow[0] = "Итого";
                totalRow[1] = startYearSumm;
                totalRow[2] = summ;
                dtSource.Rows.Add(totalRow);

                currentDate = Convert.ToDateTime(dtDate.Rows[0]["curretnDate"]);
                dtSource.TableName = "  ";
                UltraWebGrid1.DataSource = dtSource;
                UltraWebGrid1.DataBind();
                Page.Title = "Структура органов государственой власти и государственных органов Новосибирской области";
                PageTitle.Text = Page.Title;                
                PageSubTitle.Text = String.Format("по состоянию на {0:dd.MM.yyyy}", currentDate);
            }
            else
            {
                //foreach (string key in Request.Form.AllKeys)
                //{
                //    Response.Write(key + " " + Request.Form[key] + "<br/>");
                //}

                if (Request.Form["__EVENTTARGET"] == "saveButton")
                {
                    SaveData();
                }
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            UltraWebGrid1.DisplayLayout.AddNewBox.Hidden = !haveSavePermission;
            UltraWebGrid1.DisplayLayout.AddNewBox.View = AddNewBoxView.Compact;
            UltraWebGrid1.DisplayLayout.AllowAddNewDefault = AllowAddNew.Yes;
            UltraWebGrid1.DisplayLayout.AllowDeleteDefault = AllowDelete.Yes;
            UltraWebGrid1.DisplayLayout.AddNewBox.Prompt = "";
            UltraWebGrid1.DisplayLayout.AddNewBox.ButtonStyle.BackgroundImage = "~/images/addButton.png";
            UltraWebGrid1.DisplayLayout.AddNewBox.ButtonStyle.CustomRules = "background-position: left top; background-repeat: no-repeat";
            UltraWebGrid1.DisplayLayout.AddNewBox.ButtonStyle.BorderColor = Color.White;
            UltraWebGrid1.DisplayLayout.AddNewBox.ButtonStyle.ForeColor = Color.Transparent;
            UltraWebGrid1.DisplayLayout.AddNewBox.ButtonStyle.BackColor = Color.Transparent;

            UltraWebGrid1.DisplayLayout.AddNewBox.ButtonStyle.Width = 81;
            UltraWebGrid1.DisplayLayout.AddNewBox.ButtonStyle.Height = 26;
            UltraWebGrid1.DisplayLayout.AddNewBox.BoxStyle.CustomRules = "padding-left: 20px";

            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(750);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(150);

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            headerLayout.AddCell("Наименование областного исполнительного органа государственной власти", "");
            headerLayout.AddCell(String.Format("Предельная штатная численность на 01.01.{0:yyyy} года, чел.", currentDate), "Предельная штатная численность органов исполнительной власти, чел.");
            headerLayout.AddCell(String.Format("Предельная штатная численность на {0:dd.MM.yyyy} года, чел.", currentDate), "Предельная штатная численность органов исполнительной власти, чел.");
            headerLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 20;
            e.Layout.Bands[0].Columns[2].CellStyle.Padding.Right = 20;
        }

        #region экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            headerLayout.AddCell("Наименование областного исполнительного органа государственной власти", "");
            headerLayout.AddCell(String.Format("Предельная штатная численность на 01.01.{0:yyyy} года, чел.", currentDate), "Предельная штатная численность органов исполнительной власти, чел.");
            headerLayout.AddCell(String.Format("Предельная штатная численность на {0:dd.MM.yyyy} года, чел.", currentDate), "Предельная штатная численность органов исполнительной власти, чел.");
            headerLayout.ApplyHeaderInfo();

            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, 3);
        }

        #endregion

        #region Экспорт в Pdf


        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            headerLayout.AddCell("Наименование областного исполнительного органа государственной власти", "");
            headerLayout.AddCell(String.Format("Предельная штатная численность на 01.01.{0:yyyy} года, чел.", currentDate), "Предельная штатная численность органов исполнительной власти, чел.");
            headerLayout.AddCell(String.Format("Предельная штатная численность на {0:dd.MM.yyyy} года, чел.", currentDate), "Предельная штатная численность органов исполнительной власти, чел.");
            headerLayout.ApplyHeaderInfo();

            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion

        private void SaveData()
        {
            string xml = String.Format("<table><date><curretnDate>{0:yyyy-MM-dd}</curretnDate></date>", DateTime.Now);
            foreach (UltraGridRow row in UltraWebGrid1.Rows)
            {
                Int32 value;
                if (row.Cells[0].ToString() != "Итого" &&(
                    Int32.TryParse(row.Cells[1].ToString(), out value) || 
                    row.Cells[1].ToString() == String.Empty ||
                    Int32.TryParse(row.Cells[2].ToString(), out value) ||
                    row.Cells[2].ToString() == String.Empty))
                {
                    xml += String.Format("<grbsTable><id>{0}</id><grbsName>{1}</grbsName><staffCountStartYear>{2}</staffCountStartYear><staffCountLimit>{3}</staffCountLimit></grbsTable>",
                                        row.Index + 1, row.Cells[0], row.Cells[1], row.Cells[2]);
                }
            }
            xml += "</table>";

            string filePath = HttpContext.Current.Server.MapPath("~/reports/OIV_0001_0001/Default.Settings.xml");
            File.WriteAllText(filePath, xml, System.Text.Encoding.UTF8);
            PageSubTitle.Text = String.Format("по состоянию на {0:dd.MM.yyyy}", DateTime.Now);
        }
    }
}
