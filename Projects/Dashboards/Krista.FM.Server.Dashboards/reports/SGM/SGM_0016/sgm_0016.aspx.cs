using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0016
{
    public partial class sgm_0016 : CustomReportPage
    {
        private readonly DataTable dtGrid = new DataTable();
        private readonly DataTable dtData = new DataTable();

        private string subjectCodes = string.Empty;
        private int year;
        private const int yearCount = 5;
        private Color backColor = Color.Tan;

        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();
        private readonly SGMSQLTexts sqlTextClass = new SGMSQLTexts();

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            base.Page_Load(sender, e);
            dataRotator.formNumber = 2;
            dataObject.InitObject();
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);
            dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, false);

            if (!Page.IsPostBack)
            {
                dataRotator.FillYearList(ComboYear);
                ComboYear.SetСheckedState(SGMDataRotator.startYearInjections, true);
            }
            year = Convert.ToInt32(ComboYear.SelectedValue);
            subjectCodes = dataRotator.regionSubstrSubjectIDs[dataRotator.mapList[0]];

            FillData();

            Page.Title = string.Format("Показатели охвата и своевременности иммунизации населения на территории РФ в {0}-{1} гг. (в %)",
                year - 4, year);
            LabelTitle.Text = string.Format("{0}", Page.Title);
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int dirtyWidth = Convert.ToInt32(CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30).Value);

            if (dirtyWidth != 0)
            {
                LabelTitle.Width = dirtyWidth - 50;
            }
            grid.Width = dirtyWidth;
            grid.Height = Unit.Empty;
            
            SetExportHandlers();
        }

        protected virtual double GetSum(DataRow[] drs)
        {
            double result = 0;
            for (int i = 0; i < drs.Length; i++)
            {
                result += Convert.ToDouble(drs[i][0]);
            }
            return result;
        }

        protected virtual void CalcRowValues(string deseaseCode, string peopleGroup,
            string injectionKind, string deseaseCodePlan)
        {
            DataRow drAdd = dtGrid.Rows.Add();

            const string templateSelect = "yr = {0} and priv in ({2}) and vozr in ({3}) and inf = {1}";

            int startYear = year - yearCount + 1;
            for (int i = startYear; i < year + 1; i++)
            {
                DataRow[] dtr1 = dtData.Select(string.Format(templateSelect, i, deseaseCode, injectionKind, peopleGroup));
                DataRow[] dtr2 = dtData.Select(string.Format(templateSelect, i, "99", "99", peopleGroup));

                if (dtr1.Length > 0 && dtr2.Length > 0)
                {
                    drAdd[i - startYear + 2] = Math.Round(100 * GetSum(dtr1) / GetSum(dtr2), 2);
                }
            }
        }

        protected virtual void CorrectRowValues(string deseaseCode, string peopleGroup,
            string injectionKind)
        {
            DataRow drAdd = dtGrid.Rows[dtGrid.Rows.Count - 1];

            const string templateSelect = "yr = {0} and priv in ({2}) and vozr in ({3}) and inf = {1}";

            int startYear = year - yearCount + 1;
            for (int i = startYear; i < 2006; i++)
            {
                DataRow[] dtr1 = dtData.Select(string.Format(templateSelect, i, deseaseCode, injectionKind, peopleGroup));
                DataRow[] dtr2 = dtData.Select(string.Format(templateSelect, i, "99", "99", peopleGroup));

                if (dtr1.Length > 0 && dtr2.Length > 0)
                {
                    drAdd[i - startYear + 2] = Math.Round(100 * GetSum(dtr1) / GetSum(dtr2), 2);
                }
            }
        }

        protected virtual void CalcRowValues(string deseaseCode, string peopleGroup, 
            string injectionKind)
        {
            CalcRowValues(deseaseCode, peopleGroup, injectionKind, "99");
        }

        protected virtual void FillDataTable()
        {
            DataColumn dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");

            int startYear = year - yearCount + 1;
            for (int i = 0; i < 5; i++)
            {
                dataColumn = dtGrid.Columns.Add();
                dataColumn.DataType = Type.GetType("System.Double");
                dataColumn.ColumnName = Convert.ToString(startYear + i);
            }

            dataRotator.ExecQuery(dtData, sqlTextClass.GetDeseaseSQLText_SGM0016(subjectCodes, year));

            // Дифтерия
            CalcRowValues("11", "2", "1,22");
            CalcRowValues("11", "2", "80");
            CalcRowValues("11", "3", "22");
            CalcRowValues("11", "3", "82");
            CalcRowValues("11", "8", "24");
            CalcRowValues("11", "16", "26");
            CalcRowValues("11", "37,38,39", "1,22,24,26,30");
            CorrectRowValues("11", "30", "1,22,24,26,30");
            CalcRowValues("11", "37,38,39", "22,24,26,30");
            CorrectRowValues("11", "30", "22,24,26,30");

            dtGrid.Rows[00][0] = "Дифтерия";
            dtGrid.Rows[00][1] = "1 год";
            dtGrid.Rows[01][1] = "12 мес.";
            dtGrid.Rows[02][1] = "2 года";
            dtGrid.Rows[03][1] = "24 мес.";
            dtGrid.Rows[04][1] = "7 лет";
            dtGrid.Rows[05][1] = "14 лет";
            dtGrid.Rows[06][1] = "с 18 лет вакцинация";
            dtGrid.Rows[07][1] = "с 18 лет ревакцинация";

            // Коклюш
            CalcRowValues("10", "2", "1,20");
            CalcRowValues("10", "2", "80");
            CalcRowValues("10", "3", "20");
            CalcRowValues("10", "3", "82");

            dtGrid.Rows[08][0] = "Коклюш";
            dtGrid.Rows[08][1] = "1 год";
            dtGrid.Rows[09][1] = "12 мес.";
            dtGrid.Rows[10][1] = "2 года";
            dtGrid.Rows[11][1] = "24 мес.";

            // Полиомиелит
            CalcRowValues("4", "2", "1,22,24");
            CalcRowValues("4", "2", "80");
            CalcRowValues("4", "3", "24");
            CalcRowValues("4", "3", "84");
            CalcRowValues("4", "16", "26");

            dtGrid.Rows[12][0] = "Полиомиелит";
            dtGrid.Rows[12][1] = "1 год";
            dtGrid.Rows[13][1] = "12 мес.";
            dtGrid.Rows[14][1] = "2 года";
            dtGrid.Rows[15][1] = "24 мес.";
            dtGrid.Rows[16][1] = "14 лет";

            // Вирусный гепатит В
            CalcRowValues("18", "2", "1");
            CalcRowValues("18", "2", "80");
            CalcRowValues("18", "15", "1");

            dtGrid.Rows[17][0] = "Вирусный гепатит В";
            dtGrid.Rows[17][1] = "1 год";
            dtGrid.Rows[18][1] = "12 мес.";
            dtGrid.Rows[19][1] = "13 лет";

            // Корь
            CalcRowValues("5", "3", "1,44");
            CalcRowValues("5", "3", "80");
            CalcRowValues("5", "7", "20,44");

            dtGrid.Rows[20][0] = "Корь";
            dtGrid.Rows[20][1] = "2 года";
            dtGrid.Rows[21][1] = "24 мес.";
            dtGrid.Rows[22][1] = "6 лет";

            // Эпидемический паротит
            CalcRowValues("6", "3", "1,44");
            CalcRowValues("6", "3", "80");
            CalcRowValues("6", "7", "20,44");

            dtGrid.Rows[23][0] = "Эпидемический паротит";
            dtGrid.Rows[23][1] = "2 года";
            dtGrid.Rows[24][1] = "24 мес.";
            dtGrid.Rows[25][1] = "6 лет";

            // Краснуха
            CalcRowValues("30", "3", "1,44");
            CalcRowValues("30", "3", "80");
            CalcRowValues("30", "7", "20,44");
            CalcRowValues("30", "15", "20,44");

            dtGrid.Rows[26][0] = "Краснуха";
            dtGrid.Rows[26][1] = "2 года";
            dtGrid.Rows[27][1] = "24 мес.";
            dtGrid.Rows[28][1] = "6 лет";
            dtGrid.Rows[29][1] = "13 лет";

            // Туберкулез
            CalcRowValues("1", "32", "80");
            CalcRowValues("1", "8", "22");

            dtGrid.Rows[30][0] = "Туберкулез";
            dtGrid.Rows[30][1] = "30 дней";
            dtGrid.Rows[31][1] = "7 лет";

            grid.Bands.Clear();
            grid.DataSource = dtGrid;
            grid.DataBind();
            grid.Width = 390 + yearCount * 105;
        }

        protected virtual void FillData()
        {
            FillDataTable();
        }

        protected void grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].Header.Caption = "Заболевание";
            e.Layout.Bands[0].Columns[0].Width = 180;
            e.Layout.Bands[0].Columns[1].Header.Caption = "Возрастная группа";
            e.Layout.Bands[0].Columns[1].Width = 150;

            for (int i = 2; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = 100;
                CRHelper.FormatNumberColumn(grid.Columns[i], "N2");
                grid.Columns[i].Header.Caption = dtGrid.Columns[i].ColumnName;
            }
        }

        protected void grid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            for (int i = 0; i < yearCount - 1; i++)
            {
                supportClass.SetInjectionsCellImage(e.Row, 2 + i, 3 + i, 3 + i);
            }

            e.Row.Cells[0].Style.Font.Bold = true;
            e.Row.Cells[1].Style.Font.Bold = true;

            if (supportClass.CheckValue(e.Row.Cells[0].Value))
            {
                backColor = backColor == Color.Tan ? Color.LightBlue : Color.Tan;

            }
            e.Row.Style.BackColor = backColor;
        }


        #region PDFExport

        private void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.WordExportButton.Visible = true;
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            exportClass.ExportCaptionText(e, LabelTitle.Text);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = String.Format("0016.pdf");
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        #endregion
    }
}
