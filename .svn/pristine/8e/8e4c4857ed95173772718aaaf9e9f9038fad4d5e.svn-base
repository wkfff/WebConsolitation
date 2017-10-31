using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0017
{
    public partial class sgm_0017 : CustomReportPage
    {
        private readonly DataTable dtGrid = new DataTable();

        readonly DataTable dtRegion1 = new DataTable();
        readonly DataTable dtRegion2 = new DataTable();
        readonly DataTable dtRegion3 = new DataTable();
        readonly DataTable dtRegion4 = new DataTable();

        private string subjectCodes = string.Empty;
        private int year;
        private int groupCount;
        private string deseaseCode = string.Empty;

        private string[] privArray;
        private string[] vozrArray;
        private string[] captionsArray;

        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMRegionNamer regionNamer = new SGMRegionNamer();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();
        private readonly SGMSQLTexts sqlTextClass = new SGMSQLTexts();

        protected void FillInfectionsList(CustomMultiCombo ComboList)
        {
            var tempList = new Collection<string>();
            tempList.Clear();
            tempList.Add("Дифтерия");
            tempList.Add("Коклюш");
            tempList.Add("Острый паралитический полиомиелит");
            tempList.Add("Корь");
            tempList.Add("Паротит эпидемический");
            tempList.Add("Краснуха");
            tempList.Add("Острый гепатит В");
            tempList.Add("Туберкулез органов дыхания");
            ComboList.FillValues(tempList);
            ComboList.SetСheckedState(tempList[0], true);
        }

        protected void CalcGroupCount()
        {
            groupCount = 2;
            if (ComboDesease.SelectedIndex == 0) groupCount = 6;
            if (ComboDesease.SelectedIndex == 2) groupCount = 3;
            if (ComboDesease.SelectedIndex == 5) groupCount = 3;
        }

        protected void CalcDeseaseCode()
        {
            var codes = new string[8];
            codes[0] = "11";
            codes[1] = "10";
            codes[2] = "4";
            codes[3] = "5";
            codes[4] = "6";
            codes[5] = "30";
            codes[6] = "18";
            codes[7] = "1";
            deseaseCode = codes[ComboDesease.SelectedIndex];
        }

        protected void CalcArrayCodes()
        {
            int index = ComboDesease.SelectedIndex;

            privArray = new string[groupCount];
            vozrArray = new string[groupCount];
            captionsArray = new string[groupCount];

            //Дифтерия
            if (index == 0)
            {
                privArray[0] = "80";
                vozrArray[0] = "2";
                captionsArray[0] = "12 мес. вакцинация своевременно";

                privArray[1] = "82";
                vozrArray[1] = "3";
                captionsArray[1] = "24 мес. ревакцинация своевременно";
                
                privArray[2] = "24";
                vozrArray[2] = "8";
                captionsArray[2] = "7л .- 7л.11мес.29 дн. ревакцинация II";

                privArray[3] = "26";
                vozrArray[3] = "16";
                captionsArray[3] = "14 л. – 14 л.11 мес. 29 дн. ревакцинация III";

                privArray[4] = "1,22,24,26,30";
                vozrArray[4] = "37,38,39";
                captionsArray[4] = "С 18 лет вакцинация";

                privArray[5] = "22,24,26,30";
                vozrArray[5] = "37,38,39";
                captionsArray[5] = "С 18 лет ревакцинация";
            }
            // Коклюш
            if (index == 1)
            {
                privArray[0] = "80";
                vozrArray[0] = "2";
                captionsArray[0] = "12  мес. вакцинация своевременно";

                privArray[1] = "82";
                vozrArray[1] = "3";
                captionsArray[1] = "24  мес. ревакцинация своевременно";
            }
            // Полимилит
            if (index == 2)
            {
                privArray[0] = "80";
                vozrArray[0] = "2";
                captionsArray[0] = "12  мес. вакцинация своевременно";

                privArray[1] = "84";
                vozrArray[1] = "3";
                captionsArray[1] = "24  мес. ревакцинация II своевременно";

                privArray[2] = "26";
                vozrArray[2] = "16";
                captionsArray[2] = "14 л. –14 л.11 мес. 29 дн. Ревакцинация III";
            }
            // Корь
            if (index == 3)
            {
                privArray[0] = "80";
                vozrArray[0] = "3";
                captionsArray[0] = "24 мес. вакцинация своевременно";

                privArray[1] = "20,44";
                vozrArray[1] = "7";
                captionsArray[1] = "6 л. – 6 л. 11 мес. 29 дн. ревакцинация";
            }
            // Паротит
            if (index == 4)
            {
                privArray[0] = "80";
                vozrArray[0] = "3";
                captionsArray[0] = "24 мес. вакцинация своевременно";

                privArray[1] = "20,44";
                vozrArray[1] = "7";
                captionsArray[1] = "6 л. – 6 л. 1 1мес. 29 дн. ревакцинация";
            }
            // Краснуха
            if (index == 5)
            {
                privArray[0] = "80";
                vozrArray[0] = "3";
                captionsArray[0] = "24  мес. вакцинация своевременно";

                privArray[1] = "20,44";
                vozrArray[1] = "7";
                captionsArray[1] = "6 л. – 6 л. 11 мес. 29 дней ревакцинация";

                privArray[2] = "20,44";
                vozrArray[2] = "15";
                captionsArray[2] = "13 л. – 13 л. 11 мес. 29 дней ревакцинация (девочки)";
            }
            // Гепатит
            if (index == 6)
            {
                privArray[0] = "80";
                vozrArray[0] = "2";
                captionsArray[0] = "12  мес. вакцинация своевременно";

                privArray[1] = "1";
                vozrArray[1] = "15";
                captionsArray[1] = "13 л. –  13 л. 11  мес. 29 дн. вакцинация";
            }
            // Туберкулез
            if (index == 7)
            {
                privArray[0] = "80";
                vozrArray[0] = "32";
                captionsArray[0] = "новорожденные (0 – 30 дней) вакцинация своевременно";

                privArray[1] = "22";
                vozrArray[1] = "8";
                captionsArray[1] = "7 л. – 7 л. 11 мес. 29 дн. ревакцинация I";
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            base.Page_Load(sender, e);
            dataObject.InitObject();
            dataRotator.formNumber = 2;
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);
            regionNamer.FillSGMDeseasesDictionary2();
            regionNamer.FillFMtoSGMRegions();
            dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, false);

            if (!Page.IsPostBack)
            {
                dataRotator.FillYearList(ComboYear);
                FillInfectionsList(ComboDesease);

                ComboYear.SetСheckedState(SGMDataRotator.startYearInjections, true);
            }
            year = Convert.ToInt32(ComboYear.SelectedValue);
            
            CalcGroupCount();
            CalcDeseaseCode();
            CalcArrayCodes();

            FillDataTable();

            Page.Title = string.Format(@" Анализ охвата вакцинацией против {0} в {1}-{2} гг. (в %)",
                regionNamer.sgmDeseaseDictionary2[ComboDesease.SelectedValue], year - 1, year);
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

        protected virtual bool CheckSubjectCode(string subjectCode)
        {
            bool result = true;
            if (subjectCode == "57") result = false;
            if (subjectCode == "102") result = false;
            if (subjectCode == "76") result = false;
            if (subjectCode == "108") result = false;
            if (subjectCode == "105") result = false;
            if (subjectCode == "107") result = false;
            if (subjectCode == "106") result = false;
            if (subjectCode == "30") result = false;
            if (subjectCode == "110") result = false;
            return result;
        }

        protected virtual void FillRowRegion(string subjectCode, string rowName)
        {
            const string templateSelect = "priv in ({0}) and vozr in ({1}) and area in ({2})";
            if (CheckSubjectCode(subjectCode.Trim()))
            {
                DataRow drAdd = dtGrid.Rows.Add();

                for (int i = 0; i < privArray.Length; i++)
                {
                    DataRow[] dtr1 = dtRegion1.Select(string.Format(templateSelect, privArray[i], vozrArray[i], subjectCode));
                    DataRow[] dtr2 = dtRegion2.Select(string.Format(templateSelect, "99", vozrArray[i], subjectCode));

                    if (subjectCode == "113" && (year < 2009)) subjectCode = "113,76,108";
                    if (subjectCode.Trim() == "25" && (year < 2009)) subjectCode = "25,107";

                    DataRow[] dtr3 = dtRegion3.Select(string.Format(templateSelect, privArray[i], vozrArray[i], subjectCode));
                    DataRow[] dtr4 = dtRegion4.Select(string.Format(templateSelect, "99", vozrArray[i], subjectCode));

                    double sum1 = GetSum(dtr1);
                    double sum2 = GetSum(dtr2);
                    double sum3 = GetSum(dtr3);
                    double sum4 = GetSum(dtr4);

                    if (sum1 > 0)
                    {
                        if (sum4 > 0) drAdd[i * 2 + 1] = 100 * sum3 / sum4;
                        if (sum2 > 0) drAdd[i * 2 + 2] = 100 * sum1 / sum2;
                    }
                }
                drAdd[0] = rowName;
            }
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

        protected virtual void FillDataTable()
        {
            DataColumn dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.Caption = "Административные территории";

            for (int i = 0; i < groupCount * 2; i++)
            {
                dataColumn = dtGrid.Columns.Add();
                dataColumn.DataType = Type.GetType("System.Double");
                dataColumn.Caption = Convert.ToString(year - (1 - i % 2));
            }


            subjectCodes = dataRotator.regionSubstrSubjectIDs[dataRotator.mapList[0]];
            dataRotator.ExecQuery(dtRegion1, sqlTextClass.GetDeseaseSQLText_SGM0017(deseaseCode, year, subjectCodes));
            dataRotator.ExecQuery(dtRegion2, sqlTextClass.GetDeseaseSQLText_SGM0017("99", year, subjectCodes));
            dataRotator.ExecQuery(dtRegion3, sqlTextClass.GetDeseaseSQLText_SGM0017(deseaseCode, year - 1, subjectCodes));
            dataRotator.ExecQuery(dtRegion4, sqlTextClass.GetDeseaseSQLText_SGM0017("99", year - 1, subjectCodes));

            for (int i = 0; i < dataRotator.mapList.Count; i++)
            {
                subjectCodes = dataRotator.regionSubstrSubjectIDs[dataRotator.mapList[i]];
                FillRowRegion(subjectCodes, dataRotator.mapList[i]);
                if (i != 0)
                {                   
                    string[] codes = subjectCodes.Split(',');
                    for (int j = 0; j < codes.Length; j++)
                    {
                        FillRowRegion(codes[j], GetAreaNameByCode(codes[j]));
                    }
                }
            }

            grid.Bands.Clear();
            grid.DataSource = dtGrid;
            grid.DataBind();
            grid.Width = 300 + groupCount * 2 * 65;
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].Width = 280;

            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = 60;
                CRHelper.FormatNumberColumn(grid.Columns[i], "N2");
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            bool isFirstCaptions = true;
            for (int i = 0; i < groupCount; i++)
            {
                var ch = new ColumnHeader(true)
                             {
                                 Caption = string.Format(captionsArray[i]),
                                 RowLayoutColumnInfo =
                                     {
                                         OriginY = 0,
                                         OriginX = 1 + i*2,
                                         SpanX = 2,
                                         SpanY = 1
                                     }
                             };

                if (e.Layout.Bands[0].HeaderLayout.Count > (grid.Columns.Count + 5))
                {
                    if (e.Layout.Bands[0].HeaderLayout[0].Caption == "Административные территории")
                    {
                        e.Layout.Bands[0].HeaderLayout[i + grid.Columns.Count].Caption = captionsArray[i];
                    }
                    else
                    {
                        isFirstCaptions = false;
                        e.Layout.Bands[0].HeaderLayout[i].Caption = captionsArray[i];
                    }
                }
                else
                {
                    e.Layout.Bands[0].HeaderLayout.Add(ch);
                }
            }
            
            int startIndex = 1;
            if (!isFirstCaptions)  startIndex = 7;

            for (int i = 0; i < groupCount; i++)
            {
                e.Layout.Bands[0].HeaderLayout[startIndex + i * 2 + 0].Caption = Convert.ToString(year - 1);
                e.Layout.Bands[0].HeaderLayout[startIndex + i * 2 + 1].Caption = Convert.ToString(year - 0);
            }
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < groupCount; i++ )
            {
                supportClass.SetInjectionsCellImage(e.Row, 1 + i * 2, 2 + i * 2, 2 + i * 2);
            }

            if (dataRotator.mapList.IndexOf(e.Row.Cells[0].Value.ToString()) >= 0)
            {
                e.Row.Style.Font.Bold = true;
                e.Row.Cells[0].Value = regionNamer.GetFMName(e.Row.Cells[0].Value.ToString());
            }
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
            UltraGridExporter1.PdfExporter.DownloadName = string.Format("0017.pdf");
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        #endregion
    }
}
