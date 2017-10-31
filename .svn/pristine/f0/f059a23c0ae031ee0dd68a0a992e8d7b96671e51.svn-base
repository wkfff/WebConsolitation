using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0018
{
    public partial class sgm_0018 : CustomReportPage
    {
        private readonly DataTable dtGrid = new DataTable();
        private readonly DataTable dtBadSubjects = new DataTable();
        private readonly DataTable[] dtData = new DataTable[5];

        private string subjectCodes = string.Empty;
        private int year;
        private const int yearCount = 5;
        private Color backColor = Color.Tan;
        string deseaseAppendix;

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

            FillDataTable();

            Page.Title = string.Format("Анализ доли территорий РФ, охваченных вакцинацией в {0}-{1} гг. (в %)",
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

        protected virtual string GetAreaNameByCode(string code)
        {
            string result = string.Empty;
            DataRow[] dtrArea = dataObject.dtAreaFull.Select(string.Format("kod = {0}", code));
            if (dtrArea.Length > 0)
            {
                result = dtrArea[0]["name"].ToString();
            }
            return result.Trim(' ');
        }

        protected virtual double GetSum(DataRow[] drs, string subjectCode)
        {
            double result = 0;
            for (int i = 0; i < drs.Length; i++)
            {
                if (drs[i]["area"].ToString() == subjectCode)
                {
                    result += Convert.ToDouble(drs[i][0]);
                }
            }
            return result;
        }

        protected virtual void AddToBad(string subjectName, double measureValue)
        {
            if (supportClass.FindDataRow(dtBadSubjects, GetAreaNameByCode(subjectName), "name") == null)
            {
            DataRow drBad = dtBadSubjects.Rows.Add();
            drBad[0] = GetAreaNameByCode(subjectName);
            drBad[1] = 100 * measureValue;
            }
        }

        protected virtual string GetBadSubjects()
        {
            string result = string.Empty;
            DataRow[] drs = dtBadSubjects.Select(string.Empty, "percent desc");
            for (int i = 0; i < drs.Length; i++)
            {
                result = string.Format("{0}, {1} ({2:N2})", result, drs[i][0], drs[i][1]);
            }

            return result.TrimStart(',').TrimStart(' ');
        }

        protected virtual void CalcRowValues(string deseaseCode, string peopleGroup,
            string injectionKind)
        {
            DataRow drAdd = dtGrid.Rows.Add();
            bool needMultiCount = (peopleGroup.Split(',').Length > 1 || injectionKind.Split(',').Length > 1);

            const string templateSelect = "priv in ({1}) and vozr in ({2}) and inf = {0}";

            if (deseaseCode == "11") drAdd[1] = "Дифтерия";
            if (deseaseCode == "10") drAdd[1] = "Коклюш";
            if (deseaseCode == "4") drAdd[1] = "Полиомиелит";
            if (deseaseCode == "18") drAdd[1] = "Гепатит В";
            if (deseaseCode == "6") drAdd[1] = "Паротит";
            if (deseaseCode == "5") drAdd[1] = "Корь";
            if (deseaseCode == "30") drAdd[1] = "Краснуха";

            drAdd[1] = string.Format("{0}{1}", drAdd[1], deseaseAppendix);

            int startYear = year - yearCount + 1;
            for (int i = startYear; i < year + 1; i++)
            {
                int subjectCount = 0;
                if (i > 2005 && peopleGroup == "30") peopleGroup = "37,38,39";
                DataRow[] dtr1 = dtData[year - i].Select(string.Format(templateSelect, deseaseCode, injectionKind, peopleGroup));
                DataRow[] dtr2 = dtData[year - i].Select(string.Format(templateSelect, "99", "99", peopleGroup));

                if (dtr1.Length > 0 && dtr2.Length > 0)
                {
                    for (int j = 0; j < dtr2.Length; j++)
                    {
                        double percent;
                        if (needMultiCount)
                        {
                            double sum1 = GetSum(dtr1, dtr2[j]["area"].ToString());
                            double sum2 = GetSum(dtr2, dtr2[j]["area"].ToString());
                            if (sum2 > 0)
                            {
                                percent = sum1 / sum2;
                                if (percent > 0.9495)
                                {
                                    subjectCount++;
                                }
                                else
                                {
                                    if (i == year) AddToBad(dtr2[j]["area"].ToString(), percent);
                                }
                            }
                        }
                        else
                        {
                            DataRow drFind2 = dtr2[j];
                            DataRow drFind1 = supportClass.FindRowInSelection(dtr1, dtr2[j]["area"].ToString(), "area");
                            if (supportClass.CheckValue(drFind1) && supportClass.CheckValue(drFind2))
                            {
                                percent = Convert.ToDouble(drFind1[0]) / Convert.ToDouble(drFind2[0]);
                                if (percent > 0.9495)
                                {
                                    subjectCount++;
                                }
                                else
                                {
                                    if (i == year) AddToBad(dtr2[j]["area"].ToString(), percent);
                                }
                            }
                        }
                    }
                }
                if (dtr1.Length > 0) drAdd[i - startYear + 2] = 100 * Convert.ToDouble(subjectCount) / Convert.ToDouble(dtr2.Length);
            }
            drAdd[yearCount + 2] = GetBadSubjects();
            dtBadSubjects.Clear();
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
            dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.Caption = "Территории с показателем охвата вакцинацией менее 95%";

            dataColumn = dtBadSubjects.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "name";
            dataColumn = dtBadSubjects.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = "percent";

            dtData[0] = new DataTable();
            dtData[1] = new DataTable();
            dtData[2] = new DataTable();
            dtData[3] = new DataTable();
            dtData[4] = new DataTable();

            dataRotator.ExecQuery(dtData[0], sqlTextClass.GetDeseaseSQLText_SGM0018(subjectCodes, year - 0));
            dataRotator.ExecQuery(dtData[1], sqlTextClass.GetDeseaseSQLText_SGM0018(subjectCodes, year - 1));
            dataRotator.ExecQuery(dtData[2], sqlTextClass.GetDeseaseSQLText_SGM0018(subjectCodes, year - 2));
            dataRotator.ExecQuery(dtData[3], sqlTextClass.GetDeseaseSQLText_SGM0018(subjectCodes, year - 3));
            dataRotator.ExecQuery(dtData[4], sqlTextClass.GetDeseaseSQLText_SGM0018(subjectCodes, year - 4));

            // 12 мес
            deseaseAppendix = " V";
            CalcRowValues("11", "2", "80");
            dtGrid.Rows[dtGrid.Rows.Count - 1][0] = "12 месяцев";
            CalcRowValues("10", "2", "80");
            CalcRowValues("4", "2", "80");
            CalcRowValues("18", "2", "80");

            // 24 мес
            deseaseAppendix = " RV1";
            CalcRowValues("11", "3", "82");
            dtGrid.Rows[dtGrid.Rows.Count - 1][0] = "24 месяца";
            CalcRowValues("10", "3", "82");
            deseaseAppendix = " RV2";
            CalcRowValues("4", "3", "84");
            deseaseAppendix = " V";
            CalcRowValues("5", "3", "80");
            CalcRowValues("6", "3", "80");
            CalcRowValues("30", "3", "80");

            // 6 лет
            deseaseAppendix = " RV";
            CalcRowValues("5", "7", "20,44");
            dtGrid.Rows[dtGrid.Rows.Count - 1][0] = "6 лет";
            CalcRowValues("6", "7", "20,44");
            CalcRowValues("30", "7", "20,44");

            // 7 лет
            deseaseAppendix = " RV";
            CalcRowValues("11", "8", "24");
            dtGrid.Rows[dtGrid.Rows.Count - 1][0] = "7 лет";

            //13 лет
            deseaseAppendix = " V";
            CalcRowValues("18", "15", "1");
            dtGrid.Rows[dtGrid.Rows.Count - 1][0] = "13 лет";

            // 14 лет
            deseaseAppendix = " RV3";
            CalcRowValues("11", "16", "26");
            dtGrid.Rows[dtGrid.Rows.Count - 1][0] = "14 лет";
            CalcRowValues("4", "16", "26");

            // С 18 лет вакинация
            deseaseAppendix = " V";
            CalcRowValues("11", "30", "1,22,24,26,30");
            dtGrid.Rows[dtGrid.Rows.Count - 1][0] = "18 лет";
            deseaseAppendix = " RV";       
            CalcRowValues("11", "30", "22,24,26,30");

            grid.Bands.Clear();
            grid.DataSource = dtGrid;
            grid.DataBind();
        }

        protected void grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].Header.Caption = "Возрастная группа";
            e.Layout.Bands[0].Columns[0].Width = 140;
            e.Layout.Bands[0].Columns[1].Header.Caption = "Заболевание";
            e.Layout.Bands[0].Columns[1].Width = 130;

            for (int i = 2; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = 60;
                CRHelper.FormatNumberColumn(grid.Columns[i], "N2");
                grid.Columns[i].Header.Caption = dtGrid.Columns[i].ColumnName;
            }
            grid.Columns[grid.Columns.Count - 1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            grid.Columns[grid.Columns.Count - 1].Width = Convert.ToInt32(grid.Width.Value) - yearCount * 60 - 380;
            grid.Columns[grid.Columns.Count - 1].Header.Caption = "Территории с показателем охвата вакцинацией менее 95%";
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

            for (int i = 2; i < yearCount + 2; i++)
            {
                if (supportClass.CheckValue(e.Row.Cells[i].Value))
                {
                    if (e.Row.Cells[i].Value.ToString() == "100.00") e.Row.Cells[i].Style.Font.Bold = true;
                }
            }

                e.Row.Style.BackColor = backColor;
            e.Row.Style.Wrap = true;
        }

        #region PDFExport

        private void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.WordExportButton.Visible = true;
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            exportClass.ExportCaptionText(e, LabelTitle.Text);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = String.Format("0018.pdf");
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        #endregion
    }
}
