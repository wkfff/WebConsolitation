using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.reports.SGM;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SGM_0001_0001_v : CustomReportPage
    {
        protected DataTable dtFullData = new DataTable();
        protected DataTable tblFullDataDes = new DataTable();
        protected DataTable tblFullDataMap = new DataTable();

        protected string month;
        protected string year1, year2;
        protected string groupName1, groupName2;
        protected string mapName, foName, rfName, mapCode;
        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMRegionNamer regionNamer = new SGMRegionNamer();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMSQLTexts sqlTexts = new SGMSQLTexts();

        private string deseasesCodes;
        private int year = 2010;
        private string mapNameReport;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            grid.Width = Unit.Empty;
            grid.Height = Unit.Empty;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (HttpContext.Current.Session["CurrentSgmID"] == null)
            {
                CustomParams.MakeRegionParams("83", "id");
            }

            dataObject.InitObject();
            dataRotator.formNumber = 1;
            dataRotator.FillDeseasesList(null, 0);
            dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, false);
            dataRotator.FillRelationValues();
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);

            regionNamer.FillFMtoSGMRegions();
            regionNamer.FillSGMRegionsDictionary1();
            regionNamer.FillSGMRegionsDictionary2();
            regionNamer.FillSGMDeseasesDictionary1();
            regionNamer.FillSGMDeseasesDictionary2();

            dataRotator.formNumber = 1;
            year = dataRotator.GetLastYear();
            month = dataRotator.GetMonthParamIphone();
            year1 = year.ToString();
            year2 = (year - 1).ToString();
            mapName = regionNamer.GetSGMName(RegionsNamingHelper.FullName(UserParams.ShortStateArea.Value));
            mapCode = dataObject.GetMapCode(mapName);
            foName = dataObject.GetFOName(mapCode, false);
            rfName = dataRotator.mapList[0];
            deseasesCodes = dataRotator.deseasesCodes[0];

            groupName1 = "0";
            groupName2 = "4";

            FillData();
            FillComponentData();
            BindFirstLabel();
            FillDataTable();
        }

        protected virtual void FillComponentData()
        {
            grid.DataSource = dtFullData;
            grid.DataBind();
        }

        protected virtual void FillData()
        {
            // Структура данных по болезням
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctDeseaseName;
            dataObject.useLongNames = true;
            dataObject.mainColumnRange = dataRotator.GetDeseaseCodes(0).Replace("62,42,", String.Empty);
            // Заболевание по субъекту текущий год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, mapName, groupName1, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "1");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, mapName, groupName2, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "3");
            // Заболевание по субъекту прошлый год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year2, month, mapName, groupName1, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "5");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year2, month, mapName, groupName2, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "7");
            // Рост \ снижение текстовка
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText,
                "1", "5", "2", "6");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText,
                "3", "7", "4", "8");
            // ФО
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, foName, groupName1, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "11");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, foName, groupName2, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "13");
            // РФ
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, rfName, groupName1, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "15");
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, rfName, groupName2, String.Empty);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "17");
            dtFullData = dataObject.FillData(0, false);
            // Подчистим ненужные колонки с отн. показателями(они чтобы посчитать рост\снижение)
            dtFullData.Columns.RemoveAt(17);
            dtFullData.Columns.RemoveAt(15);
            dtFullData.Columns.RemoveAt(13);
            dtFullData.Columns.RemoveAt(11);
            dtFullData.Columns.RemoveAt(8);
            dtFullData.Columns.RemoveAt(7);
            dtFullData.Columns.RemoveAt(6);
            dtFullData.Columns.RemoveAt(5);
            dtFullData.Columns.RemoveAt(3);
            dtFullData.Columns.RemoveAt(1);

            dataObject.InitObject();
            // Структура данных по болезням
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctDeseaseName;
            dataObject.mainColumnRange = deseasesCodes;
            var mapArray = new[] { mapName, foName.Length > 0 ? foName : mapName, rfName };
            const int columnCount = 6;

            for (int i = 0; i < mapArray.Length; i++)
            {
                int baseOffset = columnCount * i;
                // Заболевание по субъекту текущий год
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                     year1, month, mapArray[i], groupName1, String.Empty);
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                     Convert.ToString(baseOffset + 1));
                // Заболевание по субъекту прошлый год
                dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                     year2, month, mapArray[i], groupName1, String.Empty);
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

            dataObject.InitObject();
            // Структура данных по болезням
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");
            // Заболевание по субъекту текущий год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                 year1, month, String.Empty, groupName1, deseasesCodes);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                 "2");
            // Заболевание по субъекту прошлый год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                                 year2, month, String.Empty, groupName1, deseasesCodes);
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                                 "4");
            // Рост \ снижение текстовка
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText,
                                 "2", "4", "3", "5");
            // отношение показателей
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
                                 "2", "4");
            tblFullDataMap = dataObject.FillData();
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Grid.Width = Unit.Empty;

            e.Layout.Bands[0].Columns[0].Header.Caption = "Заболевание";
            e.Layout.Bands[0].Columns[0].Width = 212;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Header.Caption = "всего";
                if (i % 2 == 0) grid.Columns[i].Header.Caption = "дети";
                grid.Columns[i].Width = 67;
                grid.Columns[i].Header.Title = "Заболеваемость на 100 тысяч населения";
                CRHelper.FormatNumberColumn(grid.Columns[i], "N2");
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                var rlcInfo = e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo;
                if (i == 0)
                {
                    rlcInfo.OriginY = 0;
                    rlcInfo.SpanY = 2;
                }
                else
                {
                    rlcInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;
            var captions = new string[4];

            string monthAppendix1 = supportClass.GetMonthLabelFull(month, 0) + " ";

            if (month.Split(',').Length == 12 || dataRotator.formNumber == 2)
            {
                monthAppendix1 = string.Empty;
            }
            string caption = string.Format("<br/>{0} {1}г", monthAppendix1, year1);
            captions[0] = string.Format("{0} {1}", UserParams.ShortStateArea.Value, caption);
            captions[1] = "Рост/снижение к прошлому году";
            captions[2] = string.Format("{0} {1}", supportClass.GetFOShortName(foName), caption);
            captions[3] = string.Format("{0} {1}", supportClass.GetFOShortName(rfName), caption);

            bool isFirst = e.Layout.Bands[0].HeaderLayout.Count < 10;
            for (int i = 0; i < 4; i++)
            {
                if (isFirst)
                {
                    var ch = new ColumnHeader(true) {RowLayoutColumnInfo = {OriginY = 0, OriginX = multiHeaderPos}};
                    multiHeaderPos += 2;
                    ch.RowLayoutColumnInfo.SpanX = 2;
                    ch.Style.Wrap = true;
                    ch.Style.HorizontalAlign = HorizontalAlign.Center;
                    ch.Caption = captions[i];
                    e.Layout.Bands[0].HeaderLayout.Add(ch);
                }
                else
                {
                    HeaderBase hb = e.Layout.Bands[0].HeaderLayout[9 + i];
                    hb.Caption = captions[i];
                }
            }
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            UltraGridCell cell = e.Row.Cells[3];
            if (cell.Value.ToString().Length > 1)
            {
                cell.Style.BackgroundImage = !cell.Value.ToString().Contains("-") ? 
                    "~/images/arrowRedUpBB.png" : 
                    "~/images/arrowGreenDownBB.png";
            }
            else if (cell.Value.ToString() == "-")
            {
                cell.Style.Padding.Right = 20;
                cell.Value = "-  ";
            }


            cell = e.Row.Cells[4];
            if (cell.Value.ToString().Length > 1)
            {
                cell.Style.BackgroundImage = !cell.Value.ToString().Contains("-") ? 
                    "~/images/arrowRedUpBB.png" : 
                    "~/images/arrowGreenDownBB.png";
            }
            else if (cell.Value.ToString() == "-")
            {
                cell.Style.Padding.Right = 20;
                cell.Value = "-  ";
            }

            cell = e.Row.Cells[5];
            if (supportClass.CheckValue(cell.Value) &&
                supportClass.CheckValue(e.Row.Cells[1].Value))
            {
                cell.Style.BackgroundImage = Convert.ToDouble(cell.Value) < 
                                             Convert.ToDouble(e.Row.Cells[1].Value) ? 
                                             "~/images/cornerRed.gif" : "~/images/cornerGreen.gif";

                if (Convert.ToDouble(cell.Value) > 999)
                {
                    cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N0");
                }
            }

            cell = e.Row.Cells[7];
            if (supportClass.CheckValue(cell.Value) &&
                supportClass.CheckValue(e.Row.Cells[1].Value))
            {
                cell.Style.BackgroundImage = Convert.ToDouble(cell.Value) <
                                             Convert.ToDouble(e.Row.Cells[1].Value) ? 
                                             "~/images/cornerRed.gif" : "~/images/cornerGreen.gif";

                if (Convert.ToDouble(e.Row.Cells[1].Value) > 999)
                {
                    e.Row.Cells[1].Value = Convert.ToDouble(e.Row.Cells[1].Value).ToString("N0");
                }
                if (Convert.ToDouble(cell.Value) > 999)
                {
                    cell.Value = Convert.ToDouble(cell.Value).ToString("N0");
                }
            }

            if (supportClass.CheckValue(e.Row.Cells[6].Value) &&
                supportClass.CheckValue(e.Row.Cells[2].Value))
            {
                e.Row.Cells[6].Style.BackgroundImage = Convert.ToDouble(e.Row.Cells[6].Value) <
                                                       Convert.ToDouble(e.Row.Cells[2].Value) ? 
                                                       "~/images/cornerRed.gif" : "~/images/cornerGreen.gif";
                if (Convert.ToDouble(e.Row.Cells[6].Value) > 999)
                {
                    e.Row.Cells[6].Value = Convert.ToDouble(e.Row.Cells[6].Value).ToString("N0");
                }
            }

            if (supportClass.CheckValue(e.Row.Cells[8].Value) &&
                supportClass.CheckValue(e.Row.Cells[2].Value))
            {
                double value;
                if (double.TryParse(e.Row.Cells[8].Value.ToString(), out value) &&
                    double.TryParse(e.Row.Cells[2].Value.ToString(), out value))
                {
                    e.Row.Cells[8].Style.BackgroundImage = Convert.ToDouble(e.Row.Cells[8].Value) <
                                                           Convert.ToDouble(e.Row.Cells[2].Value) ? 
                                                           "~/images/cornerRed.gif" : "~/images/cornerGreen.gif";
                }
                if (Convert.ToDouble(e.Row.Cells[2].Value) > 999)
                {
                    e.Row.Cells[2].Value = Convert.ToDouble(e.Row.Cells[2].Value).ToString("N0");
                }
                if (Convert.ToDouble(e.Row.Cells[8].Value) > 999)
                {
                    e.Row.Cells[8].Value = Convert.ToDouble(e.Row.Cells[8].Value).ToString("N0");
                }
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-left: 2px; padding-right: 3px;  padding-top: 3px";
            }

            e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-left: 2px; padding-right: 1px;  padding-top: 3px";
            e.Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-left: 2px; padding-right: 1px;  padding-top: 3px";

            e.Row.Cells[0].Style.ForeColor = Color.FromArgb(0xC0C0C0);
            e.Row.Cells[2].Style.ForeColor = Color.FromArgb(0xC0C0C0);
            e.Row.Cells[4].Style.ForeColor = Color.FromArgb(0xC0C0C0);
            e.Row.Cells[6].Style.ForeColor = Color.FromArgb(0xC0C0C0);
            e.Row.Cells[8].Style.ForeColor = Color.FromArgb(0xC0C0C0);

            e.Row.Cells[3].Value = e.Row.Cells[3].Value.ToString().Replace(" раза", "<br/>раза");
            e.Row.Cells[4].Value = e.Row.Cells[4].Value.ToString().Replace(" раза", "<br/>раза");

            string code = GetCode(e.Row.Cells[0].Value.ToString());

            if (e.Row.Cells[0].Value != null)
            {
                e.Row.Cells[0].Value = supportClass.ConvertEncode1(e.Row.Cells[0].Value.ToString());
            }

            if (dataRotator.deseasesLinksRelation.ContainsKey(code))
            {
                if (dataRotator.deseasesLinksRelation[code].Length > 0)
                {
                    string[] links = dataRotator.deseasesLinksRelation[code].Split(';');
                    e.Row.Cells[0].Value = String.Format("<br/><a href=\"{0}\">{1}</a>", links[0], e.Row.Cells[0].Value);
                }
            }
            else if (dataRotator.deseasesRelation.ContainsKey(e.Row.Cells[0].Value.ToString()))
            {
                code = dataRotator.deseasesRelation[e.Row.Cells[0].Value.ToString()];
                if (dataRotator.deseasesLinksRelation[code] != "")
                {
                    string[] links = dataRotator.deseasesLinksRelation[code].Split(';');
                    e.Row.Cells[0].Value = String.Format("<br/><a href=\"{0}\">{1}</a>", links[0], e.Row.Cells[0].Value);
                }
            }
            else if (e.Row.Cells[0].Value.ToString().Contains("сальмонелл"))
            {
                code = "4";
                if (dataRotator.deseasesLinksRelation[code] != "")
                {
                    string[] links = dataRotator.deseasesLinksRelation[code].Split(';');
                    e.Row.Cells[0].Value = String.Format("<br/><a href=\"{0}\">{1}</a>", links[0], e.Row.Cells[0].Value);
                }
            }
        }


        #region 1
        private void BindFirstLabel()
        {
            var strBuilder = new StringBuilder();
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

            string appendix = "меньше";
            string imageName = "arrowGreenDownBB";
            if (percent > 0)
            {
                appendix = "больше";
                imageName = "arrowRedUpBB";
            }

            percent = Math.Abs(percent);

            mapNameReport = regionNamer.sgmDictionary1[mapName];

            strBuilder.Append(String.Format("{7}За&nbsp;<span style=\"color: white\"><b>{9}{0}</b></span>&nbsp;{10} {1} зарегистрировано&nbsp;<span style=\"color: white\"><b>{2:N0}</b></span>&nbsp;{11} инфекционных и паразитарных болезней (без педикулеза), что на <nobr><span style=\"color: white\"><b>{3:N1}%</b></span>&nbsp;{4}&nbsp;<img  src=\"../../../images/{8}.png\"></nobr>, чем в предыдущем году ({9}{5}г. -&nbsp;<span style=\"color: white\"><b>{6:N0}</b></span>).",
                                            year, mapNameReport, totalCountCur, percent, appendix, year - 1, totalCountPrev,
                                            supportClass.GetNewParagraphSyms(false, true), imageName, monthAppendix1, yearAppendix1,
                                            GetTimesPresentation(totalCountCur)));

            strBuilder.Remove(0, strBuilder.ToString().Length);
            appendix = GetNotFoundDeseases();
            if (appendix.Length > 0)
            {
                strBuilder.Append(String.Format(@"{3}В {4}{0} {5} {1} не зарегистрировано заболеваний {2}.",
                    year, mapNameReport, appendix, supportClass.GetNewParagraphSyms(false, true), monthAppendix2, yearAppendix2));
            }

            appendix = GetOverlimitedDeseases(true);
            if (appendix.Length > 0)
            {
                strBuilder.Append(String.Format("{2}Актуальными инфекциями {0}, уровни которых превышали&nbsp;<img  src=\"../../../images/ballRedBB.png\">&nbsp;показатели по стране, являлись: {1}.",
                    mapNameReport, appendix, supportClass.GetNewParagraphSyms(true)));
            }
            else
            {
                if (mapName != dataRotator.mapList[0])
                {
                    strBuilder.Append(String.Format("{0}{1} ни по одной инфекции не наблюдается превышения&nbsp;<img  src=\"../../../images/ballGreenBB.png\">&nbsp;показателей по стране.",
                        supportClass.GetNewParagraphSyms(true), CRHelper.ToUpperFirstSymbol(mapNameReport)));
                }
            }

            appendix = GetOverlimitedDeseases(false);
            if (appendix.Length  > 0)
            {
                strBuilder.Append(String.Format("{2}Актуальными инфекциями {0}, уровни которых превышали&nbsp;<img  src=\"../../../images/ballRedBB.png\">&nbsp;показатели по {3}, являлись: {1}.",
                    mapNameReport, appendix, supportClass.GetNewParagraphSyms(true), supportClass.GetFOShortName(foName)));
            }
            else
            {
                if (foName.Length  > 0)
                {
                    strBuilder.Append(String.Format("{0}{1} ни по одной инфекции не наблюдается превышения&nbsp;<img  src=\"../../../images/ballGreenBB.png\">&nbsp;показателей по ФО.",
                        supportClass.GetNewParagraphSyms(true), CRHelper.ToUpperFirstSymbol(mapNameReport)));
                }
            }

            LabelPart2Text.Text = strBuilder.ToString();
            strBuilder.Remove(0, strBuilder.ToString().Length);

            int decreaseCount = GetDecreaseDeseases();
            strBuilder.Append(String.Format("{2}По сравнению с предыдущим годом, в соответствии с формой {3}, {0} отмечалось снижение&nbsp;<img  src=\"../../../images/arrowGreenDownBB.png\">&nbsp;заболеваемости по {1}.",
                mapNameReport, GetCountPresentation(decreaseCount), supportClass.GetNewParagraphSyms(false, true), dataRotator.formNumber));

            appendix = GetIncreaseDeseases();
            if (appendix.Length > 0)
            {
                strBuilder.Append(String.Format("<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Вместе с тем, {0} по сравнению с {4}{1} {5} зарегистрирован рост&nbsp;<img  src=\"../../../images/arrowRedUpBB.png\">&nbsp;{2}.",
                    mapNameReport, Convert.ToString(year - 1), appendix, String.Empty, monthAppendix3, yearAppendix3));
            }

            LabelPart3Text.Text = strBuilder.ToString();
        }

        public static string GetTimesPresentation(double count)
        {

            if (Convert.ToInt32(count) % 10 == 1) return "случай";
            if (Convert.ToInt32(count) % 10 < 5) return "случая";
            return "случаев";
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

        protected virtual string GetNotFoundDeseases()
        {
            var strBuilder = new StringBuilder();
            string columnAbsCount = tblFullDataDes.Columns[1].ColumnName;

            DataRow[] rowsEmpty = tblFullDataDes.Select(String.Format("{0} = 0", columnAbsCount));

            foreach (DataRow dataRow in rowsEmpty)
            {
                string diesName = Convert.ToString(dataRow[0]);
                diesName = dataObject.GetDeseaseFullName(diesName);
                diesName = CRHelper.ToLowerFirstSymbol(regionNamer.sgmDeseaseDictionary1[diesName]).Trim();
                strBuilder.Append(String.Format(" {0},", diesName));
            }

            return strBuilder.ToString().Trim().Trim(',');
        }

        protected virtual string GetOverlimitedDeseases(bool compareCountry)
        {
            var strBuilder = new StringBuilder();
            string columnSubRel = tblFullDataDes.Columns[2].ColumnName;

            int columnCompareIndex = 8;
            string compareName = supportClass.GetFOShortName(foName);
            if (compareCountry)
            {
                columnCompareIndex = 14;
                compareName = supportClass.GetFOShortName(rfName);
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

        public static string GetCountPresentation(int v1)
        {
            if (v1 == 0) return string.Format("{0} нозологических форм", v1);
            if (v1 % 10 == 1 && v1 != 11) return String.Format("{0} нозологической форме", v1);
            return string.Format("{0} нозологическим формам", v1);
        }
        #endregion

        #region 4

        private readonly DataTable dtGrid = new DataTable();
        private readonly DataTable dtBadSubjects = new DataTable();
        private readonly DataTable[] dtData = new DataTable[5];
        private string subjectCodes = string.Empty;
        string deseaseAppendix;
        private const int yearCount = 1;

        protected virtual void FillDataTable()
        {
            DataColumn dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");

            const int startYear = 2009;
            {
                dataColumn = dtGrid.Columns.Add();
                dataColumn.DataType = Type.GetType("System.Double");
                dataColumn.ColumnName = Convert.ToString(startYear);
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

            subjectCodes = dataRotator.regionSubstrSubjectIDs[mapName];

            dataRotator.ExecQuery(dtData[0], sqlTexts.GetDeseaseSQLText_SGM0018(subjectCodes, startYear));

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

            var dtInjection = new DataTable();
            dataColumn = dtInjection.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Возраст";

            dataColumn = dtInjection.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Дифтерия";

            dataColumn = dtInjection.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Коклюш";

            dataColumn = dtInjection.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Полио- миелит";

            dataColumn = dtInjection.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Гепатит В";

            dataColumn = dtInjection.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Корь";

            dataColumn = dtInjection.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Паротит";

            dataColumn = dtInjection.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn.ColumnName = "Краснуха";

            DataRow row = dtInjection.NewRow();
            dtInjection.Rows.Add(row);

            row = dtInjection.NewRow();
            dtInjection.Rows.Add(row);

            row = dtInjection.NewRow();
            dtInjection.Rows.Add(row);

            row = dtInjection.NewRow();
            dtInjection.Rows.Add(row);

            row = dtInjection.NewRow();
            dtInjection.Rows.Add(row);

            row = dtInjection.NewRow();
            dtInjection.Rows.Add(row);

            row = dtInjection.NewRow();
            dtInjection.Rows.Add(row);

            dtInjection.Rows[0][0] = "12 мес.";
            dtInjection.Rows[1][0] = "24 мес.";
            dtInjection.Rows[2][0] = "6 лет";
            dtInjection.Rows[3][0] = "7 лет";
            dtInjection.Rows[4][0] = "13 лет";
            dtInjection.Rows[5][0] = "14 лет";
            dtInjection.Rows[6][0] = "18 лет";

            dtInjection.Rows[0]["Дифтерия"] = "V";
            dtInjection.Rows[0]["Коклюш"] = "V";
            dtInjection.Rows[0]["Полио- миелит"] = "V";
            dtInjection.Rows[0]["Гепатит В"] = "V";

            dtInjection.Rows[1]["Дифтерия"] = "RV1";
            dtInjection.Rows[1]["Коклюш"] = "RV1";
            dtInjection.Rows[1]["Полио- миелит"] = "RV2";
            dtInjection.Rows[1]["Гепатит В"] = "V";
            dtInjection.Rows[1]["Корь"] = "V";
            dtInjection.Rows[1]["Паротит"] = "V";
            dtInjection.Rows[1]["Краснуха"] = "V";

            dtInjection.Rows[2]["Корь"] = "RV";
            dtInjection.Rows[2]["Паротит"] = "RV";
            dtInjection.Rows[2]["Краснуха"] = "RV";

            dtInjection.Rows[3]["Дифтерия"] = "RV";

            dtInjection.Rows[4]["Гепатит В"] = "V";

            dtInjection.Rows[5]["Дифтерия"] = "RV3";
            dtInjection.Rows[5]["Полио- миелит"] = "RV3";

            dtInjection.Rows[6]["Дифтерия"] = "V, RV";
            dtInjection.Rows[6]["Коклюш"] = "";
            dtInjection.Rows[6]["Полио- миелит"] = "";
            dtInjection.Rows[6]["Гепатит В"] = "";
            dtInjection.Rows[6]["Корь"] = "";
            dtInjection.Rows[6]["Паротит"] = "";
            dtInjection.Rows[6]["Краснуха"] = "";

            UltraWebGrid1.InitializeLayout += grid1_InitializeLayout;
            UltraWebGrid1.InitializeRow += grid1_InitializeRow;

            UltraWebGrid1.DataSource = dtInjection;
            UltraWebGrid1.DataBind();
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

            int i = year;
            if (i > 2005 && peopleGroup == "30")
            {
                peopleGroup = "37,38,39";
            }
            DataRow[] dtr1 =
                dtData[year - i].Select(string.Format(templateSelect, deseaseCode, injectionKind, peopleGroup));
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
                            percent = sum1/sum2;
                            if (percent > 0.9999)
                            {
                            }
                            else
                            {
                                if (i == year) AddToBad(dtr2[j]["area"].ToString(), percent);
                                drAdd[2] = percent;
                            }
                        }
                    }
                    else
                    {
                        DataRow drFind2 = dtr2[j];
                        DataRow drFind1 = supportClass.FindRowInSelection(dtr1, dtr2[j]["area"].ToString(), "area");
                        if (supportClass.CheckValue(drFind1) && supportClass.CheckValue(drFind2))
                        {
                            percent = Convert.ToDouble(drFind1[0])/Convert.ToDouble(drFind2[0]);
                            if (percent > 0.9999)
                            {
                            }
                            else
                            {
                                if (i == year) AddToBad(dtr2[j]["area"].ToString(), percent);
                                drAdd[2] = percent;
                            }
                        }
                    }
                }
            }

            drAdd[yearCount + 2] = GetBadSubjects();
            dtBadSubjects.Clear();
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

        protected virtual string GetAreaNameByCode(string code)
        {
            string result = string.Empty;
            DataRow[] dtrArea = dataObject.dtAreaFull.Select(string.Format("kod = {0}", code));
            if (dtrArea.Length > 0)
            {
                result = dtrArea[0][0].ToString();
            }
            return result.Trim(' ');
        }

        protected virtual string GetBadSubjects()
        {
            string result = string.Empty;
            DataRow[] drs = dtBadSubjects.Select(string.Empty, "percent desc");
            for (int i = 0; i < drs.Length; i++)
            {
                result = string.Format("{0}, {1} ({2:N2})", result, drs[i][0], drs[i][1]);
            }

            return result.TrimStart(',').TrimStart();
        }

        protected void grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].Width = 118;
            e.Layout.Bands[0].Columns[1].Width = 90;
            e.Layout.Bands[0].Columns[2].Width = 90;
            e.Layout.Bands[0].Columns[3].Width = 90;
            e.Layout.Bands[0].Columns[4].Width = 90;
            e.Layout.Bands[0].Columns[5].Width = 90;
            e.Layout.Bands[0].Columns[6].Width = 90;
            e.Layout.Bands[0].Columns[7].Width = 90;
        }

        private int injectionNumber;

        protected void grid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Value != null &&
                    e.Row.Cells[i].Value.ToString() != String.Empty)
                {
                    double value;
                    if (Double.TryParse(dtGrid.Rows[injectionNumber][2].ToString(), out value))
                    {
                        e.Row.Cells[i].Style.BackgroundImage = value < 0.95 ?
                            "~/images/ballRedBB.png" : 
                            "~/images/ballGreenBB.png";
                    }
                    e.Row.Cells[i].Value = 
                        String.Format("{0}<br/>{1:P2}", e.Row.Cells[i].Value, value);
                    injectionNumber++;
                }
                e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px 20px; padding-left: 2px";
            }

        }
        #endregion

        private string GetCode(string name)
        {
            DataRow[] row = dataObject.dtDeseasesFull.Select(String.Format("diesname = '{0}'", name));
            if (row.Length == 1)
            {
                return row[0][1].ToString();
            }
            return String.Empty;
        }
    }
}
