using System;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Shared.Events;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0012
{
    public partial class sgm_0012 : CustomReportPage
    {
        private DataTable tblFullDataDes = new DataTable();
        private DataTable tblFullDataMap = new DataTable();
        private DataRow[] rowsChart;
        private DataTable dtMainChart = new DataTable();
        
        private string mapName = String.Empty;
        private string mapNameReport = String.Empty;
        private int year;
        private string deseasesCodes = String.Empty;
        private string month = String.Empty;
        private string foName = String.Empty;
        private string rfName = String.Empty;

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
            dataObject.InitObject();
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);
            regionNamer.FillFMtoSGMRegions();
            regionNamer.FillSGMRegionsDictionary1();
            regionNamer.FillSGMRegionsDictionary2();
            regionNamer.FillSGMDeseasesDictionary1();
            regionNamer.FillSGMDeseasesDictionary2();

            dataRotator.FillDeseasesList(null, 0);
            if (!Page.IsPostBack)
            {
                dataRotator.FillSGMMapList(ComboMap, dataObject.dtAreaShort, true);
                ComboMap.ParentSelect = true;
                dataRotator.FillYearList(ComboYear);
                dataRotator.FillMonthListEx(ComboMonth, ComboYear.SelectedValue);
            }
            else
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, true);
            }

            if (Page.IsPostBack
                || !regionNamer.sgmDictionary1.ContainsKey(UserParams.CustomParam("SGM0012MapName").Value) 
                || UserParams.CustomParam("SGM0012MapName").Value == String.Empty)
            {
                UserParams.CustomParam("SGM0012MapName").Value = ComboMap.SelectedValue;
                UserParams.CustomParam("SGM0012Year").Value = ComboYear.SelectedValue;
                UserParams.CustomParam("SGM0012Month").Value = dataRotator.GetMonthParamString(ComboMonth, ComboYear.SelectedValue);
            }     

            year = Convert.ToInt32(UserParams.CustomParam("SGM0012Year").Value);
            month = Convert.ToString(UserParams.CustomParam("SGM0012Month").Value);
            mapName = Convert.ToString(UserParams.CustomParam("SGM0012MapName").Value);
            mapNameReport = regionNamer.sgmDictionary1[mapName];

            dataRotator.CheckFormNumber(year, ref month);

            FillMainData();

            string monthText = supportClass.GetMonthLabelFull(month, 0);
            Page.Title = "Справка о состоянии инфекционной заболеваемости";
            LabelTitle.Text = String.Format("{0}: {3} за {2} {1} год{4}{5}",
                regionNamer.GetFMName(mapName), year, monthText, Page.Title, dataRotator.GetYearAppendix(), dataRotator.GetFormHeader());
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int dirtyWidth = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);

            LabelTitle.Width = dirtyWidth - 100;            
            chart3.Width = dirtyWidth;
            dirtyWidth = dirtyWidth / 2;

            LabelPart1Text.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            chart1.Width = dirtyWidth;
            chart2.Width = dirtyWidth;
            chart1.Height = 240;
            chart2.Height = 240;

            SetExportHandlers();
        }

        protected virtual string GetNotFoundDeseases()
        {
            var strBuilder = new StringBuilder();
            string columnAbsCount = tblFullDataDes.Columns[1].ColumnName;

            DataRow[] rowsEmpty = tblFullDataDes.Select(String.Format("{0} = 0", columnAbsCount));

            foreach (DataRow dataRow in rowsEmpty)
            {
                string diesName = Convert.ToString(dataRow[0]);
                diesName = dataObject.GetDeseaseFullName(diesName);
                diesName = CRHelper.ToLowerFirstSymbol(regionNamer.sgmDeseaseDictionary1[diesName]);
                strBuilder.Append(String.Format(" {0},", diesName));
            }

            return strBuilder.ToString().Trim().Trim(',');
        }

        protected virtual string GetMaxDeseases(double totalCount)
        {
            string columnDeseaseRel = tblFullDataDes.Columns[2].ColumnName;
            string columnDeseaseName = tblFullDataDes.Columns[0].ColumnName;

            DataRow[] rowsDeseases = tblFullDataDes.Select(String.Empty, columnDeseaseRel + " desc");

            if (rowsDeseases.Length > 0)
            {
                DataRow drGripp = supportClass.FindDataRowEx(tblFullDataDes, "Грипп", columnDeseaseName);
                double valueGripp = 0;
                string stringGripp = String.Empty;
                if (drGripp != null)
                {
                    stringGripp = "(включая грипп)";
                    valueGripp = Convert.ToDouble(drGripp[1]);
                }

                double percent = 100 * (valueGripp + Convert.ToDouble(rowsDeseases[0][1])) / totalCount;

                return String.Format("{3}В структуре инфекционных и паразитарных болезней преобладали {0}{2}, доля которых составила {1:N1}%.",
                    rowsDeseases[0][0], percent, stringGripp, supportClass.GetNewParagraphSyms(true));
            }
            return String.Empty;
        }

        protected virtual string GetOverlimitedDeseases(bool compareCountry)
        {
            var strBuilder = new StringBuilder();
            string columnSubRel = tblFullDataDes.Columns[2].ColumnName;

            int columnCompareIndex = 8;
            string compareName = foName;
            if (compareCountry)
            {
                columnCompareIndex = 14;
                compareName = rfName;
            }

            string columnRgnRel = tblFullDataDes.Columns[columnCompareIndex].ColumnName;

            DataRow[] rowsDeseases = tblFullDataDes.Select(
                String.Format("{0} > {1}", columnSubRel, columnRgnRel),
                columnSubRel + " desc");

            foreach (DataRow rowDesease in rowsDeseases)
            {
                string diesName = Convert.ToString(rowDesease[0]);
                diesName = dataObject.GetDeseaseFullName(diesName);
                diesName = CRHelper.ToLowerFirstSymbol(diesName);
                strBuilder.Append(String.Format(" {0} ({1:N3} на 100 тысяч населения в сравнении с {2:N3} по {3}),",
                    diesName, rowDesease[columnSubRel], rowDesease[columnRgnRel], compareName));                
            }

            return strBuilder.ToString().Trim().Trim(',');
        }

        protected virtual int GetDecreaseDeseases()
        {
            string columnSubjectCur = tblFullDataDes.Columns[2].ColumnName;
            string columnSubjectPrv = tblFullDataDes.Columns[4].ColumnName;

            DataRow[] rowsDeseases = tblFullDataDes.Select(
                String.Format("{0} < {1}", columnSubjectCur, columnSubjectPrv));

            return rowsDeseases.Length;
        }

        protected virtual string GetIncreaseDeseases()
        {
            var strBuilder = new StringBuilder();
            string columnSubjectCur = tblFullDataDes.Columns[1].ColumnName;
            string columnSubjectPrv = tblFullDataDes.Columns[3].ColumnName;
            string columnSubjectRelCur = tblFullDataDes.Columns[2].ColumnName;
            string columnSubjectRelPrv = tblFullDataDes.Columns[4].ColumnName;

            DataRow[] rowsDeseases = tblFullDataDes.Select(
                String.Format("{0} > {1}", columnSubjectCur, columnSubjectPrv));

            foreach (DataRow rowIncrease in rowsDeseases)
            {
                string diesCode = Convert.ToString(rowIncrease[tblFullDataDes.Columns.Count - 1]); 
                double valueAbs1 = Convert.ToDouble(rowIncrease[columnSubjectCur]);
                double valueAbs2 = Convert.ToDouble(rowIncrease[columnSubjectPrv]);

                double valueRel1 = Convert.ToDouble(rowIncrease[columnSubjectRelCur]);
                double valueRel2 = Convert.ToDouble(rowIncrease[columnSubjectRelPrv]);

                string diesName = Convert.ToString(rowIncrease[0]);
                diesName = dataObject.GetDeseaseFullName(diesName);
                diesName = CRHelper.ToLowerFirstSymbol(regionNamer.sgmDeseaseDictionary1[diesName]);

                if (supportClass.CheckDeseasePeriod(diesCode, year, year - 1))
                {
                    strBuilder.Append(String.Format(" {0} заболеваемости {1},",
                                                    supportClass.GetDifferenceTextEx(
                                                        valueAbs1, 
                                                        valueAbs2, 
                                                        valueRel1,
                                                        valueRel2, 
                                                        true, 
                                                        true),
                                                    diesName));
                }
            }

            return strBuilder.ToString().Trim().Trim(',');
        }

        protected DataTable GetChartDataset1(double totalCount, bool needFirst)
        {
            const int absCountColumnIndex = 1;
            var tblResult = new DataTable();
            tblResult.Columns.Add("ColumnName", typeof(string));
            tblResult.Columns.Add("ColumnPerc", typeof(Double));
            DataRow[] rowsDies = tblFullDataDes.Select(
                String.Empty, 
                tblFullDataDes.Columns[absCountColumnIndex].ColumnName + " desc");

            if (rowsDies.Length > 0)
            {
                double maxDiesPct = Convert.ToDouble(rowsDies[0][absCountColumnIndex]);
                DataRow rowFind = supportClass.FindDataRowEx(tblFullDataDes, "Грипп", tblFullDataDes.Columns[0].ColumnName);

                if (rowFind != null)
                {
                    maxDiesPct += Convert.ToDouble(rowFind[absCountColumnIndex]);
                }

                maxDiesPct = Math.Round(100 * maxDiesPct / totalCount, 2);

                if (!needFirst)
                {
                    double totalPercent = 0;

                    for (int i = 1; i < 10; i++)
                    {
                        string diesName = Convert.ToString(rowsDies[i][0]).Trim();
                        if (diesName != "Грипп")
                        {
                            double percent = 100 * Convert.ToDouble(rowsDies[i][absCountColumnIndex]) / totalCount;
                            totalPercent += percent;
                            DataRow dr = tblResult.Rows.Add();
                            dr[0] = diesName;
                            dr[1] = percent;
                        }
                    }

                    DataRow rowOther = tblResult.Rows.Add();
                    rowOther[0] = "Прочие";
                    rowOther[1] = 100 - totalPercent - maxDiesPct;
                }
                else
                {
                    DataRow rowMain = tblResult.Rows.Add();
                    rowMain[0] = String.Format("{0} + Грипп", rowsDies[0][0].ToString().Trim());
                    rowMain[1] = maxDiesPct;

                    DataRow rowOther = tblResult.Rows.Add();
                    rowOther[0] = "Остальные";
                    rowOther[1] = 100 - maxDiesPct;
                }
            }
            else
            {
                tblResult.Rows.Add();
            }
            return tblResult;
        }

        protected DataTable GetChartDataset2()
        {
            string caption1 = rfName;
            string caption2 = mapName.Trim();

            if (mapName == dataRotator.mapList[0])
            {
                caption1 = String.Format("{1} ({0})", year - 1, rfName);
                caption2 = String.Format("{1} ({0})", year - 0, rfName);
            }
            else
            {
                caption2 = supportClass.GetFOShortName(caption2);
            }

            var tblResult = new DataTable();
            tblResult.Columns.Add("ColumnName", typeof(string));
            tblResult.Columns.Add(caption1, typeof(double));

            if (foName.Length > 0)
            {
                tblResult.Columns.Add(foName, typeof(double));
            }

            tblResult.Columns.Add(caption2, typeof(double));

            var tblTemp = new DataTable();
            tblTemp.Columns.Add("ColumnName", typeof(string));
            for (int i = 0; i < 5; i++)
            {
                tblTemp.Columns.Add(String.Format("ColumnData{0}", i), typeof(double));
            }


            for (int i = 0; i < tblFullDataDes.Rows.Count; i++)
            {
                DataRow rowDesease = tblTemp.Rows.Add();
                DataRow rowSource = tblFullDataDes.Rows[i];

                double valueSubject = Convert.ToDouble(rowSource[2]);
                double valueRF = Convert.ToDouble(rowSource[14]);
                double valueFO = Convert.ToDouble(rowSource[8]);
                rowDesease[0] = rowSource[0];

                if (mapName == dataRotator.mapList[0])
                {
                    valueRF = Convert.ToDouble(rowSource[4]);
                }

                if (valueRF > 0)
                {
                    rowDesease[1] = valueSubject / valueRF;
                }
                else
                {
                    rowDesease[1] = 0;
                }


                rowDesease[2] = valueSubject;
                rowDesease[3] = valueRF;

                if (foName.Length > 0)
                {
                    if (valueRF > 0)
                    {
                        rowDesease[4] = valueFO / valueRF;
                    }
                    else
                    {
                        rowDesease[4] = 0;
                    }

                    rowDesease[5] = valueFO;
                }
            }

            string rel1Column = tblTemp.Columns[1].ColumnName;
            string rel2Column = tblTemp.Columns[4].ColumnName;

            rowsChart = tblTemp.Select(
                String.Format("{0} > 1 or ({0} > {1} and {1} <> 0)", rel1Column, rel2Column),
                rel1Column + " asc");

            int subjectCellIndex = 2;
            if (foName.Length > 0)
            {
                subjectCellIndex = 3;
            }

            for (int i = 0; i < rowsChart.Length; i++)
            {
                DataRow rowSource = rowsChart[i];
                DataRow rowData = tblResult.Rows.Add();
                rowData[0] = rowSource[0];
                rowData[1] = 1;

                if (foName.Length > 0)
                {
                    rowData[2] = rowSource[4];
                }

                rowData[subjectCellIndex] = Convert.ToDouble(rowSource[1]);
            }

            return tblResult;
        }

        protected void ConfigureChart(UltraChart chart, DataTable dtChart)
        {
            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Right;
            chart.Data.ZeroAligned = true;
            chart.Axis.X.Visible = false;
            chart.Axis.X.Extent = 0;
            chart.Axis.Y.Extent = 180;
            chart.Border.Thickness = 0;
            chart.Axis.Y.Labels.Visible = false;
            chart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            chart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 15, FontStyle.Regular);
            chart.DataSource = dtChart;
            const int legendWidth = 150;
            
            if (dtChart.Rows.Count < 1)
            {
                chart.Visible = false;
            }
            else
            {
                int rowCount = 3;
                if (foName.Length > 0)
                {
                    rowCount = 4;
                }

                double widthTest = 20 * (dtChart.Rows.Count * rowCount - 1);
                chart.Height = (Unit)widthTest;
            }
            
            chart.Legend.SpanPercentage = Convert.ToInt32(100 * legendWidth / chart.Width.Value);
            chart.Legend.Margins.Bottom = Convert.ToInt32(chart.Height.Value - 120);

            chart.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            chart.Tooltips.FormatString = "<SERIES_LABEL>\n <ITEM_LABEL>\n <DATA_VALUE:N3> на 100 тыс.чел.";
            chart.DataBind();
            dtMainChart = dtChart;
        }

        private void ConfigureChart1(UltraChart chart, int totalCount, string caption, bool needFirst)
        {
            chart.Border.Thickness = 0;
            chart.Legend.Visible = true;
            chart.DataSource = GetChartDataset1(totalCount, needFirst);
            chart.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:N2>%";
            chart.DataBind();
            chart.TitleTop.Font = new Font("Verdana", 10, FontStyle.Regular);
            chart.TitleTop.HorizontalAlign = StringAlignment.Center;
            chart.TitleTop.Visible = true;
            chart.TitleTop.Text = caption;
            chart.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            chart.Legend.Font = new Font("Verdana", 7, FontStyle.Regular);
        }

        protected void FillMainData()
        {
            foName = dataObject.GetFOName(dataObject.GetMapCode(mapName), false);
            rfName = dataRotator.mapList[0];
            var strBuilder = new StringBuilder();
            deseasesCodes = dataRotator.deseasesCodes[0];
            if (year > 2005 && year < 2010)
            {
                deseasesCodes = deseasesCodes.Replace("117,", String.Empty);
            }

            const string groupName = "0";
            string year1Str = Convert.ToString(year - 0);
            string year2Str = Convert.ToString(year - 1);
            dataObject.InitObject();
            // Структура данных по болезням
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");
            // Заболевание по субъекту текущий год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                 year1Str, month, String.Empty, groupName, deseasesCodes);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                 "2");
            // Заболевание по субъекту прошлый год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                 year2Str, month, String.Empty, groupName, deseasesCodes);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                 "4");
            // Рост \ снижение текстовка
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText,
                                 "2", "4", "3", "5");
            // отношение показателей
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
                                 "2", "4");
            tblFullDataMap = dataObject.FillData();

            dataObject.InitObject();
            // Структура данных по болезням
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctDeseaseName;
            dataObject.mainColumnRange = deseasesCodes;
            var mapArray = new string[] { mapName, foName.Length > 0 ? foName : mapName, rfName };
            const int columnCount = 6;

            for (int i = 0; i < mapArray.Length; i++)
            {
                int baseOffset = columnCount * i;
                // Заболевание по субъекту текущий год
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                     year1Str, month, mapArray[i], groupName, String.Empty);
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                     Convert.ToString(baseOffset + 1));
                // Заболевание по субъекту прошлый год
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                     year2Str, month, mapArray[i], groupName, String.Empty);
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                     Convert.ToString(baseOffset + 3));
                // Рост \ снижение текстовка
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText,
                                     Convert.ToString(baseOffset + 1), Convert.ToString(baseOffset + 3),
                                     Convert.ToString(baseOffset + 2), Convert.ToString(baseOffset + 4));
                // отношение показателей
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
                                     Convert.ToString(baseOffset + 2), Convert.ToString(baseOffset + 4));
            }

            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctCodeDesease,
                                 String.Empty);

            tblFullDataDes = dataObject.FillData();

            rfName = supportClass.GetFOShortName(rfName);
            foName = supportClass.GetFOShortName(foName);

            string monthAppendix1 = supportClass.GetMonthLabelFull(month, 0) + " ";
            string monthAppendix2 = supportClass.GetMonthLabelFull(month, 1) + " ";
            string monthAppendix3 = supportClass.GetMonthLabelFull(month, 2) + " ";
            string yearAppendix1 = "года";
            string yearAppendix2 = "года";
            string yearAppendix3 = "года";

            if (month.Split(',').Length == 12 || dataRotator.formNumber == 2)
            {
                yearAppendix1 = "год";
                yearAppendix2 = "году";
                yearAppendix3 = "годом";
                monthAppendix1 = String.Empty;
                monthAppendix2 = String.Empty;
                monthAppendix3 = String.Empty;
            }

            DataRow rowSubject = supportClass.FindDataRow(
                tblFullDataMap,
                supportClass.GetFOShortName(mapName), 
                tblFullDataMap.Columns[0].ColumnName);

            double percent = Convert.ToDouble(rowSubject[7]);
            int totalCountCur = Convert.ToInt32(rowSubject[2]);
            int totalCountPrev = Convert.ToInt32(rowSubject[4]);

            string appendix = "<font color='#00a86b'>меньше</font>";
            if (percent > 0)
            {
                appendix = "<font color='#911e42'>больше</font>";
            }
            percent = Math.Abs(100 * (Math.Abs(percent) - 1));

            strBuilder.Append(
                String.Format(
                    "{7}За {9}{0} {10} {1} зарегистрировано {2:N0} {11} инфекционных и паразитарных болезней (без педикулеза), что на <nobr>{3:N1} {4} </nobr>, чем в предыдущем году ({9}{5}г. - {6:N0}).",
                    year, mapNameReport, totalCountCur, rowSubject[6], appendix, year - 1, totalCountPrev,
                    supportClass.GetNewParagraphSyms(false, true), String.Empty, monthAppendix1, yearAppendix1,
                    GetTimesPresentation(totalCountCur)));

            strBuilder.Append(GetMaxDeseases(totalCountCur));
            LabelPart1Text.Text = strBuilder.ToString();

            ConfigureChart1(chart1, totalCountCur, "Заболевания с максимальной долей среди общего числа заболевших", true);
            ConfigureChart1(chart2, totalCountCur, "Остальные, в том числе", false);

            strBuilder.Remove(0, strBuilder.ToString().Length);
            appendix = GetNotFoundDeseases();
            if (appendix.Length > 0)
            {
                strBuilder.Append(String.Format(@"{3}В {4}{0} {5} {1} не зарегистрировано заболеваний {2}.",
                                                year, mapNameReport, appendix,
                                                supportClass.GetNewParagraphSyms(false, true), monthAppendix2,
                                                yearAppendix2));
            }

            appendix = GetOverlimitedDeseases(true);
            if (appendix.Length > 0)
            {
                strBuilder.Append(
                    String.Format(
                        @"{2}Актуальными инфекциями {0}, уровни которых превышали показатели по стране, являлись: {1}.",
                        mapNameReport, appendix, supportClass.GetNewParagraphSyms(true)));
            }
            else
            {
                if (mapName != dataRotator.mapList[0])
                {
                    strBuilder.Append(
                        String.Format(@"{0}{1} ни по одной инфекции не наблюдается превышения показателей по {2}.",
                                      supportClass.GetNewParagraphSyms(true), CRHelper.ToUpperFirstSymbol(mapNameReport),
                                      supportClass.GetRootMapName(dataObject.dtAreaShort)));
                }
            }

            appendix = GetOverlimitedDeseases(false);
            if (appendix.Length > 0)
            {
                strBuilder.Append(
                    String.Format(
                        @"{2}Актуальными инфекциями {0}, уровни которых превышали показатели по {3}, являлись: {1}.",
                        mapNameReport, appendix, supportClass.GetNewParagraphSyms(true), supportClass.GetFOShortName(foName)));
            }
            else
            {
                if (foName.Length > 0)
                {
                    strBuilder.Append(
                        String.Format(@"{0}{1} ни по одной инфекции не наблюдается превышения показателей по ФО.",
                                      supportClass.GetNewParagraphSyms(true), CRHelper.ToUpperFirstSymbol(mapNameReport)));
                }
            }

            ConfigureChart(chart3, GetChartDataset2());
            LabelPart2Text.Text = strBuilder.ToString();
            strBuilder.Remove(0, strBuilder.ToString().Length);
            int decreaseCount = GetDecreaseDeseases();
            strBuilder.Append(
                String.Format(
                    @"{2}По сравнению с предыдущим годом, в соответствии с формой {3}, {0} отмечалось снижение заболеваемости по {1}.",
                    mapNameReport, GetCountPresentation(decreaseCount), supportClass.GetNewParagraphSyms(false, true),
                    dataRotator.formNumber));
            appendix = GetIncreaseDeseases();
            if (appendix.Length > 0)
            {
                strBuilder.Append(
                    String.Format(@"{3}Вместе с тем, {0} по сравнению с {4}{1} {5} зарегистрирован рост {2}.",
                                  mapNameReport, Convert.ToString(year - 1), appendix,
                                  supportClass.GetNewParagraphSyms(true), monthAppendix3, yearAppendix3));
            }

            LabelPart3Text.Text = strBuilder.ToString();
        }

        public string GetGrowthPresentation1(double percent)
        {
            if (percent < 0)
            {
                return "<font color='#00a86b'>снизилась</font>";
            }

            return "<font color='#911e42'>увеличилась</font>";
        }

        public string GetGrowthPresentation2(double percent)
        {
            if (percent < 0)
            {
                return "<font color='#00a86b'>меньше</font>";
            }

            return "<font color='#911e42'>больше</font>";
        }

        public string GetGrowthPresentation3(double percent)
        {
            if (percent < 0)
            {
                return "<font color='#00a86b'>меньше, чем</font>";
            }

            return "<font color='#911e42'>превышает</font>";
        }

        public string GetTimesPresentation(double count)
        {

            if (Convert.ToInt32(count) % 10 == 1)
            {
                return "случай";
            }

            if (Convert.ToInt32(count) % 10 < 5)
            {
                return "случая";
            }

            return "случаев";
        }

        public string GetNewPresentation(double count)
        {

            if (Convert.ToInt32(count) % 10 == 1)
            {
                return "новый";
            }

            return "новых";
        }

        protected string GetStatePresentation(string subjectName)
        {
            if (supportClass.FindInStr(subjectName, "обл"))
            {
                return "области";
            }

            return "республике";
        }

        public string GetCountPresentation(int v1)
        {
            if (v1 == 0)
            {
                return String.Format("{0} нозологических форм", v1);
            }

            if (v1 % 10 == 1 && v1 != 11)
            {
                return String.Format("{0} нозологической форме", v1);
            }

            return String.Format("{0} нозологическим формам", v1);
        }

        protected void chart3_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    var box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        int cellIndex = 2;

                        var text = new Text {PE = {Fill = Color.Black}};

                        if (box.DataPoint.Label == rfName || 
                            box.DataPoint.Label == String.Format("{1} ({0})", year - 1, rfName))
                        {
                            cellIndex = 3;
                            box.PE.FillStopColor = Color.SteelBlue;
                            box.PE.Fill = Color.DodgerBlue;
                        }
                        else if (box.DataPoint.Label == supportClass.GetFOShortName(foName))
                        {
                            cellIndex = 5;
                            box.PE.Fill = Color.LightGray;
                            box.PE.FillStopColor = Color.DarkGray;
                        }
                        else
                        {
                            box.PE.Fill = Color.LightGreen;
                            box.PE.FillStopColor = Color.DarkGreen;
                        }
                        text.SetTextString(string.Format("{0:N3}", rowsChart[box.Row][cellIndex]));
                        text.labelStyle.Orientation = TextOrientation.Horizontal;
                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.HorizontalAlign = StringAlignment.Center;

                        text.bounds = box.rect.Width > 40 ? 
                            new Rectangle(box.rect.X, box.rect.Y, box.rect.Width, box.rect.Height) : 
                            new Rectangle(box.rect.Right + 2, box.rect.Y, text.GetTextString().Length * 7, box.rect.Height);

                        e.SceneGraph.Add(text);
                    }
                    else
                    {
                        if (i != 0 && box.Path.Contains("Legend"))
                        {
                            var primitive1 = e.SceneGraph[i - 0];
                            var primitive2 = e.SceneGraph[i + 1];
                            if (primitive2 is Text && primitive1 is Box)
                            {
                                var text = (Text)primitive2;
                                if (text.GetTextString() == rfName ||
                                    text.GetTextString() == String.Format("{1} ({0})", year - 1, rfName))
                                {
                                    box.PE.FillStopColor = Color.SteelBlue;
                                    box.PE.Fill = Color.DodgerBlue;
                                }
                                else if (text.GetTextString() == supportClass.GetFOShortName(foName))
                                {
                                    box.PE.Fill = Color.LightGray;
                                    box.PE.FillStopColor = Color.DarkGray;
                                }
                                else
                                {
                                    box.PE.Fill = Color.LightGreen;
                                    box.PE.FillStopColor = Color.DarkGreen;
                                }
                            }
                        }
                    }
                }
            }
            var subjectIndex = 2;
            if (foName.Length > 0)
            {
                subjectIndex = 3;
            }

            for (int i = 0; i < dtMainChart.Rows.Count; i++)
            {
                DataRow srcRow = rowsChart[i];
                DataRow dstRow = dtMainChart.Rows[i];

                dstRow[1] = srcRow[3];
                dstRow[subjectIndex] = srcRow[2];
                if (foName.Length > 0)
                {
                    dstRow[2] = srcRow[5];
                }
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
            InitializeExportLayout(e);
            exportClass.ExportCaptionText(e, LabelTitle.Text);
            exportClass.ExportMainText(e, LabelPart1Text.Text);
            exportClass.ExportChart(e, chart1);
            exportClass.ExportChart(e, chart2);
            exportClass.ExportMainText(e, LabelPart2Text.Text);
            exportClass.ExportChart(e, chart3);
            exportClass.ExportMainText(e, LabelPart3Text.Text);

        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = String.Format("0012.pdf");
            UltraGridExporter1.PdfExporter.Export(new UltraWebGrid());
        }

        protected virtual void InitializeExportLayout(Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {            
            chart3.Width = (Unit)(chart3.Width.Value - 200);
        }

        #endregion
    }
}
