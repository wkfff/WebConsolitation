using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.WebUI.UltraWebGrid;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0015
{
    public partial class sgm_0015 : CustomReportPage
    {
        const int casesCount = 3;

        private DataTable tblFullData = new DataTable();

        private readonly DataTable dtGrid = new DataTable();
        private readonly DataTable dtChart1 = new DataTable();
        private readonly DataTable dtChart2 = new DataTable();
        private readonly DataTable dtChartAllData1 = new DataTable();
        private readonly DataTable dtChartAllData2 = new DataTable();

        private string mapName = String.Empty;
        private string mapNameReport = String.Empty;
        private int year, prevYear;
        private string deseasesCodes = String.Empty;
        private string month = String.Empty;

        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMRegionNamer regionNamer = new SGMRegionNamer();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            dataRotator.CheckSubjectReport();
            base.Page_Load(sender, e);
            dataRotator.formNumber = 1;
            regionNamer.FillFMtoSGMRegions();
            dataObject.InitObject();
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);
            regionNamer.FillSGMRegionsDictionary1();
            regionNamer.FillSGMRegionsDictionary2();
            regionNamer.FillSGMDeseasesDictionary1();

            dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, false);
            dataRotator.FillDeseasesList(null, 0);

            if (!Page.IsPostBack)
            {
                dataRotator.FillYearList(ComboYear);
                dataRotator.FillMonthListEx(ComboMonth, ComboYear.SelectedValue);
            }
            
            year = Convert.ToInt32(ComboYear.SelectedValue);
            prevYear = year - 1;
            mapName = dataRotator.mapList[0];
            mapNameReport = regionNamer.sgmDictionary1[mapName];
            month = dataRotator.GetMonthParamString(ComboMonth, year.ToString());
            dataRotator.CheckFormNumber(year, ref month);
            
            FillMainData();

            string monthText = supportClass.GetMonthLabelFull(month, 0);
            Page.Title = String.Format("Санитарно-эпидемиологическая обстановка {0} за {1} {2} год{3}",
                mapNameReport, monthText, year, dataRotator.GetYearAppendix());
            LabelTitle.Text = String.Format("{0}{1}", Page.Title, dataRotator.GetFormHeader());
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int dirtyWidth = Convert.ToInt32(CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30).Value);

            if (dirtyWidth > 0)
            {
                LabelTitle.Width = dirtyWidth - 50;
                grid1.Width = dirtyWidth;
                grid1.Height = Unit.Empty;
                LabelPart11Text.Width = dirtyWidth;
                LabelPart12Text.Width = dirtyWidth;
                LabelPart2Text.Width = dirtyWidth;
                chart1.Width = dirtyWidth;
                chart2.Width = dirtyWidth;
            }

            chart1.Height = 400;
            chart2.Height = chart1.Height;

            SetExportHandlers();
        }

        protected string GetSubjectsDeseaseExists(string deseaseCode)
        {
            string result = String.Empty;
            var groupName = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));
            var subjectDataObject = new SGMDataObject();
            subjectDataObject.InitObject();
            subjectDataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            subjectDataObject.useLongNames = false;
            subjectDataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");
            subjectDataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctAbs,
                    Convert.ToString(year),
                    month,
                    String.Empty,
                    groupName,
                    deseaseCode);
            DataTable tblDeseaseData = subjectDataObject.FillData();
            DataRow[] rowsSubjects = tblDeseaseData.Select(String.Format("{0} <> '' and {1} <> 0", 
                tblDeseaseData.Columns[1].ColumnName,
                tblDeseaseData.Columns[2].ColumnName));

            foreach (DataRow rowSubject in rowsSubjects)
            {
                result = String.Format("{0}, {1} - {2}", result, rowSubject[0], rowSubject[2]);
            }

            return result.Trim(',');
        }

        protected string GetSubjectsOverlimitedText(string deseaseCode, double relCount, int statType, bool showTitle, bool startSpace)
        {
            string result = String.Empty;
            var groupName = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));
            var subjectDataObject = new SGMDataObject();
            subjectDataObject.InitObject();
            subjectDataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            subjectDataObject.useLongNames = false;
            subjectDataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");
            subjectDataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctAbs,
                    Convert.ToString(year),
                    month,
                    String.Empty,
                    groupName,
                    deseaseCode);
            subjectDataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "2");
            DataTable tblDeseaseData = subjectDataObject.FillData();

            string selectStr = String.Format("{0} <> '' and ", tblDeseaseData.Columns[1].ColumnName);
            string columnName = tblDeseaseData.Columns[3].ColumnName;
            double limitValue = relCount * 2;
            if (statType == 0)
            {
                selectStr = String.Format("{0}{1} > {2}", selectStr, columnName, limitValue);
            }
            else
            {
                selectStr = String.Format("{0}{1} > {2} and {1} <= {3}", selectStr, columnName, relCount, limitValue);
            }

            DataRow[] rowsSubjects = tblDeseaseData.Select(
                selectStr.Replace(',', '.'),
                columnName + " desc");

            foreach (DataRow rowSubject in rowsSubjects)
            {
                result = String.Format("{0}, {1} ({2:N2})", result, rowSubject[0], rowSubject[3]);
            }

            result = result.Trim(',');
            if (result.Length > 0)
            {
                if (showTitle)
                {
                    if (statType == 0)
                    {
                        result = String.Format(@" Показатели заболеваемости, превышающие средний по {2} ({0:N2} на 100 тысяч населения) в 2 и более раз, отмечаются {1}.", 
                            relCount,
                            result,
                            supportClass.GetRootMapName(dataObject.dtAreaShort));
                    }
                    if (statType == 1)
                    {
                        result = String.Format(@" Высокие показатели заболеваемости регистрируются {0}.", result);
                    }
                }
            }
            
            return startSpace ? result : result.Trim();
        }

        private string GetUpDownText(string caption, string filter, string order)
        {
            string result = caption;

            string columnPctName = tblFullData.Columns[6].ColumnName;

            DataRow[] rowsData = tblFullData.Select(columnPctName + filter, columnPctName + order);
            string strDesData = String.Empty;

            foreach (DataRow rowData in rowsData)
            {
                if (Convert.ToInt32(rowData[1]) != 0 && Convert.ToInt32(rowData[3]) != 0)
                {
                    string percentText = Convert.ToString(rowData[5]).Replace("-", String.Empty).Trim();
                    if (percentText.Length > 0)
                    {
                        string diesName = Convert.ToString(rowData[0]);
                        diesName = CRHelper.ToLowerFirstSymbol(regionNamer.sgmDeseaseDictionary1[diesName]);

                        strDesData = String.Format("{0}, {1} - {2}",
                                                   strDesData,
                                                   diesName,
                                                   percentText);
                    }
                }
            }

            if (strDesData.Length > 0)
            {
                result = result + strDesData.Trim(',').Trim() + '.';
            }

            return result;            
        }

        protected string GetDecreaseDesease()
        {
            string result = String.Format(@"{0}За истекший период {1} года по сравнению с аналогичным периодом {2} г. в {3} отмечено снижение заболеваемости по следующим нозологическим формам: ",
                supportClass.GetNewParagraphSyms(true, false),
                year,
                prevYear,
                supportClass.GetRootMapName(dataObject.dtAreaShort));

            return GetUpDownText(result, " < 1", " asc");
        }

        protected string GetIncreaseDesease()
        {
            string result = String.Format(@"{0}За прошедший период выросла заболеваемость ",
                supportClass.GetNewParagraphSyms(true));

            return GetUpDownText(result, " > 1", " desc");
        }

        protected void GetChartDataset()
        {
            dtChart1.Columns.Add("ColumnName", typeof(string));
            dtChart1.Columns.Add(Convert.ToString(year), typeof(double));
            dtChart2.Columns.Add("ColumnName", typeof(string));
            dtChart2.Columns.Add(Convert.ToString(year), typeof(double));

            dtChartAllData2.Columns.Add(Convert.ToString(year), typeof(double));
            dtChartAllData2.Columns.Add(Convert.ToString(prevYear), typeof(double));
            dtChartAllData2.Columns.Add("DiesName", typeof(string));

            dtChartAllData1.Columns.Add(Convert.ToString(year), typeof(double));
            dtChartAllData1.Columns.Add(Convert.ToString(prevYear), typeof(double));
            dtChartAllData1.Columns.Add("DiesName", typeof(string));

            string columnRelName = tblFullData.Columns[6].ColumnName;
            string columnAbs1Name = tblFullData.Columns[1].ColumnName;
            string columnAbs2Name = tblFullData.Columns[3].ColumnName;

            // inc
            DataRow[] rowsIncrease = tblFullData.Select(
                String.Format("{0} > 1 and {1} > 0 and {2} > 0", columnRelName, columnAbs1Name, columnAbs2Name), 
                columnRelName + " desc");
            // dec
            DataRow[] rowsDecrease = tblFullData.Select(
                String.Format("{0} < 1 and {1} > 0 and {2} > 0", columnRelName, columnAbs1Name, columnAbs2Name), 
                columnRelName + " asc");

            for (int i = 0; i < rowsDecrease.Length; i++)
            {
                DataRow sourceRow = rowsDecrease[i];
                DataRow dr = dtChart1.Rows.Add();
                dr[0] = sourceRow[7];
                dr[1] = sourceRow[6];
                dtChartAllData1.Rows.Add(sourceRow[2], sourceRow[4], sourceRow[0]);
            }

            for (int i = 0; i < rowsIncrease.Length; i++)
            {
                DataRow dr = dtChart2.Rows.Add();
                DataRow sourceRow = rowsIncrease[i];
                dr[0] = sourceRow[7];
                dr[1] = sourceRow[6];
                dtChartAllData2.Rows.Add(sourceRow[2], sourceRow[4], sourceRow[0]);
            }
        }

        protected void ConfigureChart(Infragistics.WebUI.UltraWebChart.UltraChart chart, DataTable dtChart)
        {
            chart.Data.ZeroAligned = true;
            chart.Border.Thickness = 0;
            
            chart.Axis.Y.Extent = 0;
            chart.Axis.Y.Labels.Visible = false;
            chart.Axis.Y.Labels.SeriesLabels.Visible = false;

            chart.Axis.X.Extent = 200;
            chart.Axis.X.Visible = true;
            chart.Axis.X.Labels.Visible = false;
            chart.Axis.X.Labels.SeriesLabels.Visible = true;
            chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            chart.Axis.X.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Center;
            chart.Axis.X.Labels.SeriesLabels.VerticalAlign = StringAlignment.Center;
            chart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 15, FontStyle.Regular);

            chart.TitleTop.Visible = true;
            chart.TitleTop.Text = " ";
            chart.TitleTop.Extent = 50;

            chart.Tooltips.FormatString = "<ITEM_LABEL>";
            chart.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            double chartDirtyWidth = dtChart.Rows.Count * 50 + 100;

            if (chartDirtyWidth < chart.Width.Value)
            {
                chart.Width = (Unit)chartDirtyWidth;
            }
        }

        protected void FillMainData()
        {
            var strBuilder = new StringBuilder();
            deseasesCodes = dataRotator.deseasesCodes[0];
            // Лихорадка КУ не попадает чето никуда, добавим сами
            deseasesCodes = String.Format("{0},51,35,21,94,20,122,43,64,65", deseasesCodes);
            const string groupName = "0";
            string year1Str = Convert.ToString(year);
            string year2Str = Convert.ToString(prevYear);
            dataObject.InitObject();
            // Структура данных по болезням
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctDeseaseName;
            dataObject.useLongNames = true;
            dataObject.mainColumnRange = deseasesCodes;
            // Заболевание по субъекту текущий год
            // 1
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1Str, month, mapName, groupName, String.Empty);
            // 2
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "1");
            // Заболевание по субъекту прошлый год
            // 3
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year2Str, month, mapName, groupName, String.Empty);
            // 4
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, 
                "3");
            // Рост \ снижение текстовка
            // 5
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText, 
                "1", "3", "2", "4");
            // отношение показателей
            // 6
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent, 
                "2", "4");
            // 7
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctShortDeseaseName, String.Empty);
            tblFullData = dataObject.FillData();

            string monthAppendix1 = supportClass.GetMonthLabelFull(month, 0);
            string monthAppendix2 = supportClass.GetMonthLabelFull(month, 1);
            string yearAppendix1 = "года";
            string yearAppendix2 = "года";
            
            if (dataRotator.formNumber == 2)
            {
                yearAppendix1 = "год";
                yearAppendix2 = "году";
                monthAppendix1 = String.Empty;
                monthAppendix2 = String.Empty;
            }

            dtGrid.Columns.Add("Заболевание", typeof(string));
            dtGrid.Columns.Add(String.Format("{0}{1}", Convert.ToString(year), ", на 100 тыс."), typeof(double));
            dtGrid.Columns.Add(String.Format("{0}{1}", Convert.ToString(prevYear), ", на 100 тыс."), typeof(double));
            dtGrid.Columns.Add("Рост/снижение", typeof(string));
            DataColumn dataColumn = dtGrid.Columns.Add("Субъекты", typeof(string));

            string territoryName = "Субъекты";
            if (dataRotator.isSubjectReport)
            {
                territoryName = "Районы";
            }

            dataColumn.ColumnName = String.Format("{0}, показатель заболеваемости в которых в 2 или более раз превышает уровень {1} ({2} г., на 100 тыс.)",
                territoryName, supportClass.GetRootMapName(dataObject.dtAreaShort), year);
            dtGrid.Columns.Add(String.Format("{1} с высоким уровнем заболеваемости ({0} г., на 100 тыс.)", year, territoryName), typeof(string));

            var deseaseCodes = new Collection<string>();
            deseaseCodes.Add("33");
            deseaseCodes.Add("31");
            deseaseCodes.Add("25");
            deseaseCodes.Add("27");
            deseaseCodes.Add("32");
            deseaseCodes.Add("34");
            deseaseCodes.Add("35");
            deseaseCodes.Add("59");
            deseaseCodes.Add("58");
            deseaseCodes.Add("1");
            deseaseCodes.Add("8");
            deseaseCodes.Add("4");
            deseaseCodes.Add("18");
            deseaseCodes.Add("13");
            deseaseCodes.Add("121");
            deseaseCodes.Add("122");
            deseaseCodes.Add("19");
            deseaseCodes.Add("21");
            deseaseCodes.Add("94");
            deseaseCodes.Add("20");
            deseaseCodes.Add("42");
            deseaseCodes.Add("43");
            deseaseCodes.Add("48");
            deseaseCodes.Add("51");
            deseaseCodes.Add("40");
            deseaseCodes.Add("41");
            deseaseCodes.Add("37");
            deseaseCodes.Add("38");
            deseaseCodes.Add("36");
            deseaseCodes.Add("44");
            deseaseCodes.Add("63");
            deseaseCodes.Add("64");
            deseaseCodes.Add("65");
            deseaseCodes.Add("71");

            var deseaseOffsets = new Dictionary<string, bool>();
            for (int i = 0; i < deseaseCodes.Count; i++)
            {
                deseaseOffsets.Add(deseaseCodes[i],
                    (deseaseCodes[i] != "35") 
                    && (deseaseCodes[i] != "4")
                    && (deseaseCodes[i] != "122")
                    && (deseaseCodes[i] != "20")
                    && (deseaseCodes[i] != "21")
                    && (deseaseCodes[i] != "94")
                    && (deseaseCodes[i] != "43")
                    && (deseaseCodes[i] != "64")
                    && (deseaseCodes[i] != "65")
                    && (deseaseCodes[i] != "41")
                    && (deseaseCodes[i] != "36")
                    && (deseaseCodes[i] != "44")
                    && (deseaseCodes[i] != "31"));   
            }

            const string preheaderOKI = "В группе кишечных инфекций ";
            for (int i = 0; i < deseaseCodes.Count; i++)
            {
                bool isMainElement = false;
                string diesName = dataObject.GetDeseaseName(deseaseCodes[i], false);
                string deseaseNameCase1 = regionNamer.sgmDeseaseDictionary1[diesName];
                string deseaseNameCaseOrigin = deseaseNameCase1;
                deseaseNameCase1 = CRHelper.ToLowerFirstSymbol(deseaseNameCase1);
                if (deseaseOffsets[deseaseCodes[i]])
                {
                    isMainElement = true;
                    deseaseNameCase1 = string.Format("<b>{0}</b>", deseaseNameCase1);
                }

                DataRow rowDesease = supportClass.FindDataRow(tblFullData, diesName, tblFullData.Columns[0].ColumnName);
                DataRow drGrid = dtGrid.Rows.Add();
                drGrid[0] = CRHelper.ToUpperFirstSymbol(diesName);
                drGrid[1] = rowDesease[2];
                drGrid[2] = rowDesease[4];
                drGrid[3] = rowDesease[5];
                drGrid[4] = "-";
                drGrid[5] = "-";

                double relCount1 = Convert.ToDouble(rowDesease[2]);
                double relCount2 = Convert.ToDouble(rowDesease[4]);
                bool needOffset = deseaseOffsets[deseaseCodes[i]] || i == 0;
                double absCount1 = Convert.ToDouble(rowDesease[1]);
                double absCount2 = Convert.ToDouble(rowDesease[3]);

                if (absCount1 > 0)
                {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
                    drGrid[4] = GetSubjectsOverlimitedText(deseaseCodes[i], relCount1, 0, false, false);
                    drGrid[5] = GetSubjectsOverlimitedText(deseaseCodes[i], relCount1, 1, false, false);

                    if ((deseaseCodes[i] == "32") || (deseaseCodes[i] == "25") || (deseaseCodes[i] == "1")
                        || (deseaseCodes[i] == "121") || (deseaseCodes[i] == "42") || (deseaseCodes[i] == "40"))
                    {
                        string diffText = supportClass.GetDifferenceTextEx(absCount1, absCount2, relCount1, relCount2, true, true).Trim();
                        if (diffText == "-")
                        {
                            diffText = String.Empty;
                        }

                        strBuilder.Append(String.Format(@"{0}За истекший период {1}г. заболеваемость {2} {3} {4}. Зарегистрировано {5} (в {6} {7}г. - {8}).",
                            supportClass.GetNewParagraphSyms(needOffset), 
                            year, 
                            deseaseNameCase1,
                            GetGrowthPresentation1(absCount1, absCount2, isMainElement),
                            diffText,
                            supportClass.GetResultStringCountEx(absCount1, true, false),
                            monthAppendix2, 
                            prevYear,
                            supportClass.GetResultStringCountEx(absCount2, true, false).Trim()));
                    }
                    else
                    {
                        string addString = String.Empty;
                        bool zeroDifference = CheckDifference(relCount1, relCount2);
                        double value1 = relCount1;
                        double value2 = relCount2;
                        if (zeroDifference)
                        {
                            value1 = absCount1;
                            value2 = absCount2;
                        }
                        string writeForm1;
                        if (i % casesCount == 0)
                        {
                            addString = "Заболеваемость";
                            writeForm1 = "{0:N2} против {1:N2} на 100 тысяч населения";
                            
                            if (zeroDifference)
                            {
                                writeForm1 = "{0:N0} сл. против {1:N0} сл";
                            }
                            
                            if (deseaseCodes[i] == "8")
                            {
                                addString = String.Format("{0}{1}", 
                                    preheaderOKI, 
                                    CRHelper.ToLowerFirstSymbol(addString));
                            }

                            addString = String.Format(@"{7}{8} {0} {1} в {2} по сравнению с {3} годом {4} {5} и составила {6}.",
                                deseaseNameCase1, 
                                mapNameReport, 
                                year,
                                prevYear,
                                GetGrowthPresentation1(absCount1, absCount2, isMainElement),
                                supportClass.GetDifferenceTextEx(absCount1, absCount2, relCount1, relCount2, true, true),
                                writeForm1,
                                supportClass.GetNewParagraphSyms(needOffset), 
                                addString);

                            addString = String.Format(addString, value1, value2);
                        }
                        if (i % casesCount == 1)
                        {
                            writeForm1 = "{0:N2} на 100 тысяч населения";
                            string writeForm2 = "{1:N2}";
                            if (zeroDifference)
                            {
                                writeForm1 = "{0:N0} сл.";
                                writeForm2 = "{1:N0} сл.";
                            }
                            addString = "В";
                            if (deseaseCodes[i] == "8")
                            {
                                addString = String.Format("{0}{1}", preheaderOKI, CRHelper.ToLowerFirstSymbol(addString));
                            }

                            addString = String.Format(@"{0}{13} {1} {2} {3} {4} по сравнению с {5} годом заболеваемость {6} {7} {8} и составила {9} ({10} {5} {11} - {12}).",
                                supportClass.GetNewParagraphSyms(needOffset), 
                                monthAppendix2, 
                                year,
                                yearAppendix1, 
                                mapNameReport, 
                                prevYear,
                                deseaseNameCase1,
                                GetGrowthPresentation1(absCount1, absCount2, isMainElement),
                                supportClass.GetDifferenceTextEx(absCount1, absCount2, relCount1, relCount2, true, true),
                                writeForm1, 
                                monthAppendix1, 
                                yearAppendix2, 
                                writeForm2, 
                                addString);

                            addString = String.Format(addString, value1, value2);
                        }
                        if (i % casesCount == 2)
                        {
                            addString = "Заболеваемость";
                            writeForm1 = "{0:N2} на 100 тысяч населения против {1:N2}";
                            if (zeroDifference)
                            {
                                writeForm1 = "{0:N0} сл. против {1:N0} сл.";
                            }

                            if (deseaseCodes[i] == "8")
                            {
                                addString = String.Format("{0}{1}", preheaderOKI, CRHelper.ToLowerFirstSymbol(addString));
                            }

                            addString = String.Format(@"{0}{10} {1} {2} {3}, показатель заболеваемости {4} за {5} {6} {7} составил {8} за аналогичный период {9} года.",
                                supportClass.GetNewParagraphSyms(needOffset), 
                                deseaseNameCase1,
                                GetGrowthPresentation1(absCount1, absCount2, isMainElement),
                                supportClass.GetDifferenceTextEx(absCount1, absCount2, relCount1, relCount2, true, true).Trim(),
                                deseaseNameCaseOrigin, 
                                monthAppendix1, 
                                year, 
                                yearAppendix1, 
                                writeForm1, 
                                prevYear, 
                                addString);

                            addString = String.Format(addString, value1, value2);
                        }

                        strBuilder.Append(addString);
                    }
                    if (deseaseCodes[i] == "122" || deseaseCodes[i] == "20" || deseaseCodes[i] == "63")
                    {
                        strBuilder.Append(GetSubjectsOverlimitedText(deseaseCodes[i], relCount1, 0, true, true));
                        if (deseaseCodes[i] == "20")
                        {
                            strBuilder.Append(GetSubjectsOverlimitedText(deseaseCodes[i], relCount1, 1, true, true));
                        }
                    }
                    if (deseaseCodes[i] == "1" || deseaseCodes[i] == "25")
                    {
                        strBuilder.Append(GetSubjectsDeseaseExists(deseaseCodes[i]));
                    }
                }
                else
                {
                    if (month.Length > 0)
                    {
                        strBuilder.Append(string.Format("{0}Случаев заболевания {1} в {4} {2} {5} {3} не зарегистрировано.",
                            supportClass.GetNewParagraphSyms(needOffset), deseaseNameCase1,
                            year, mapNameReport, monthAppendix2, yearAppendix2));
                    }
                }
            }

            LabelPart11Text.Text = GetDecreaseDesease();
            LabelPart12Text.Text = GetIncreaseDesease();
            LabelPart2Text.Text = strBuilder.ToString().Replace(" .", ".");

            GetChartDataset();
            ConfigureChart(chart1, dtChart1);
            ConfigureChart(chart2, dtChart2);
            chart1.DataSource = dtChart1;
            chart2.DataSource = dtChart2;
            
            chart1.DataBind();
            chart2.DataBind();

            grid1.DataSource = dtGrid;
            grid1.DataBind();
        }

        protected bool CheckDifference(double relNum1, double relNum2)
        {

            return ((Math.Round(relNum1, 2) - Math.Round(relNum2, 2)) == 0)
                || (Math.Round(relNum1, 2) == 0) || (Math.Round(relNum1, 2) == 0);
        }

        public string GetGrowthPresentation1(double absCount1, double absCount2, bool needImage)
        {
            string fullText;

            if (absCount1 < absCount2)
            {
                fullText = "<font color='#00a86b'> снизилась</font>";
            }
            else if (absCount1 > absCount2)
            {
                fullText = "<font color='#911e42'> увеличилась</font>";
            }
            else
            {
                fullText = " не изменилась";
            }

            return fullText;
        }

        protected void grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].Width = 150;
            grid1.Columns[0].CellStyle.Wrap = true;
            grid1.Columns[0].CellStyle.VerticalAlign = VerticalAlign.Middle;
            grid1.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            for (int i = 1; i < grid1.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(grid1.Columns[i], "N3");
                grid1.Columns[i].Width = 60;
                
                grid1.Columns[i].CellStyle.VerticalAlign = VerticalAlign.Middle;
                grid1.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                grid1.Columns[i].CellStyle.Wrap = true;
            }
            
            grid1.Columns[3].Width = 100;
            supportClass.CalculateGridColumnsWidth(grid1, 4);
        }

        protected void grid1_InitializeRow(object sender, RowEventArgs e)
        {
            supportClass.SetCellImageEx(e.Row, 2, 1, 3);
        }

        protected void chart3_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            DataTable dtChart;
            DataTable dtChartAllData;
            string tooltipDirection;

            if (sender.Equals(chart1))
            {
                tooltipDirection = "снижение";
                dtChart = dtChart1;
                dtChartAllData = dtChartAllData1;
            }
            else
            {
                tooltipDirection = "рост";
                dtChart = dtChart2;
                dtChartAllData = dtChartAllData2;
            }

            var xAxis = (IAdvanceAxis)e.Grid["X"];
            var yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null) return;

            var xMin = (int)xAxis.MapMinimum;
            var xMax = (int)xAxis.MapMaximum;

            var percent100 = (int)yAxis.Map(100);
            var percent120 = (int)yAxis.Map(120);

            var line = new Line
                           {
                               lineStyle = {DrawStyle = LineDrawStyle.Dash},
                               PE =
                                   {
                                       Stroke = Color.DarkGray,
                                       StrokeWidth = 2
                                   },
                               p1 = new Point(xMin, percent100),
                               p2 = new Point(xMax, percent100)
                           };

            e.SceneGraph.Add(line);

            var text1 = new Text
                            {
                                PE = {Fill = Color.Black},
                                bounds = new Rectangle(xMin, percent120, xMin + 100, percent120)
                            };

            text1.SetTextString(String.Format("Уровень {0} года", prevYear));
            text1.labelStyle.Orientation = TextOrientation.Horizontal;
            text1.labelStyle.VerticalAlign = StringAlignment.Center;
            text1.labelStyle.HorizontalAlign = StringAlignment.Center;
            if (sender.Equals(chart1)) e.SceneGraph.Add(text1);

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                var primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    var box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        int cellIndex = 0;

                        var text = new Text
                                       {
                                           PE = {Fill = Color.Black},
                                           bounds = new Rectangle(box.rect.X, box.rect.Y - 50, box.rect.Width, 50)
                                       };

                        if (box.DataPoint.Label != year.ToString())
                        {
                            cellIndex = 1;
                            box.PE.FillStopColor = Color.SteelBlue;
                            box.PE.Fill = Color.DodgerBlue;
                        }
                        else
                        {
                            if (Convert.ToDouble(dtChartAllData.Rows[box.Row][0]) > Convert.ToDouble(dtChartAllData.Rows[box.Row][1]))
                            {
                                box.PE.Fill = Color.Red;
                                box.PE.FillStopColor = Color.Maroon;
                            }
                            else
                            {
                                box.PE.Fill = Color.LightGreen;
                                box.PE.FillStopColor = Color.DarkGreen;
                            }
                        }

                        string deseaseName = box.Series.Label;
                        if (deseaseName.Length > 120) deseaseName = box.Series.Label.Trim(); 
                        box.DataPoint.Label = String.Format(
                            "{6} \n{0} год - {1:N2} на 100 тыс. \n{2} год - {3:n2} на 100 тыс. \n{4} {5}",
                            year, 
                            dtChartAllData.Rows[box.Row][0], 
                            prevYear,
                            dtChartAllData.Rows[box.Row][1], 
                            tooltipDirection, 
                            dtChartAllData.Rows[box.Row][2], 
                            deseaseName);

                        text.SetTextString(String.Format("{0:N2}", dtChartAllData.Rows[box.Row][cellIndex]));
                        text.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.HorizontalAlign = StringAlignment.Center;

                        e.SceneGraph.Add(text);
                    }
                }
            }

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                dtChart.Rows[i][1] = dtChartAllData.Rows[i][0];
            }
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
            exportClass.ExportMainText(e, LabelPart11Text.Text);
            exportClass.ExportChart(e, chart1);
            exportClass.ExportMainText(e, LabelPart12Text.Text);
            exportClass.ExportChart(e, chart2);
            exportClass.ExportMainText(e, LabelPart2Text.Text);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = String.Format("0015.pdf");
            UltraGridExporter1.PdfExporter.Export(grid1);
        }

        #endregion
    }
}
