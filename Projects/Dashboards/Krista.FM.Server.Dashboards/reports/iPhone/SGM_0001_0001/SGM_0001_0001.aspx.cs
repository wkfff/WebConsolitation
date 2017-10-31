using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.reports.SGM;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SGM_0001_0001 : CustomReportPage
    {
        protected DataTable tblDiesData = new DataTable();
        protected DataTable tblRegionData = new DataTable();

        protected DataTable dtChart = new DataTable();
        protected string year1, year2;
        private int year;
        private string foName, rfName;
        private string mapName, mapCode, mapNameReport;
        private string month;

        const string groupName = "0";

        protected int chartRowCount = 8;
        protected int chartFontSize = 11;

        private readonly DataTable dtGrid = new DataTable();

        private readonly DataTable dtBadSubjects = new DataTable();

        private readonly DataTable[] dtData = new DataTable[5];

        private string subjectCodes = String.Empty;
        private const int yearCount = 1;
        string deseaseAppendix;

        string shortFoName = String.Empty;

        private SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMRegionNamer regionNamer = new SGMRegionNamer();
        private readonly SGMSQLTexts sqlTexts = new SGMSQLTexts();
        private readonly SGMSupport supportClass = new SGMSupport();

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (HttpContext.Current.Session["CurrentSgmID"] == null)
            {
                CustomParams.MakeRegionParams("13", "id");
            }

            var href = HttpContext.Current.Session["CurrentSgmID"].ToString().Length == 2
                              ? String.Format("http://{0}.rospotrebnadzor.ru/",
                                              HttpContext.Current.Session["CurrentSgmID"])
                              : HttpContext.Current.Session["CurrentSgmID"].ToString();

            HeraldImageContainer.InnerHtml = String.Format("<a href ='{0}' <img style='height: 65px' src=\"../../../images/Heralds/{1}.png\"></a>", href, HttpContext.Current.Session["CurrentSubjectID"]);

            dataObject.InitObject();
            regionNamer.FillSGMRegionsDictionary1();
            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);
            dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, false);
            dataRotator.FillDeseasesList(null, -1);
            dataRotator.formNumber = 1;
            year = dataRotator.GetLastYear();
            year1 = Convert.ToString(year - 0);
            year2 = Convert.ToString(year - 1);
            month = dataRotator.GetMonthParamIphone();
            mapName = regionNamer.GetSGMName(RegionsNamingHelper.FullName(UserParams.ShortStateArea.Value));
            mapCode = dataObject.GetMapCode(mapName);
            shortFoName = UserParams.ShortRegion.Value;
            subjectCodes = dataRotator.regionSubstrSubjectIDs[dataRotator.mapList[0]];
            foName = dataObject.GetFOName(mapCode, false);
            rfName = dataRotator.mapList[0];
            mapNameReport = regionNamer.sgmDictionary1[mapName];

            FillData();
            BindFirstLabel();
            BindOverflowTagCloud();
            grid.InitializeLayout += grid_InitializeLayout;
            grid.InitializeRow += grid_InitializeRow;
            LabelPart1Text.Width = 680;
            FillDataTable();
            grid.Height = Unit.Empty;
        }

        #region Прививки
        protected virtual void FillDataTable()
        {
            var dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");

            const int startYear = 2010;

            dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn.ColumnName = Convert.ToString(startYear);

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

            grid.DataSource = dtInjection;
            grid.DataBind();
        }

        protected virtual void CalcRowValues(string deseaseCode, string peopleGroup,
          string injectionKind)
        {
            var drAdd = dtGrid.Rows.Add();
            var needMultiCount = (peopleGroup.Split(',').Length > 1 || injectionKind.Split(',').Length > 1);

            const string templateSelect = "priv in ({1}) and vozr in ({2}) and inf = {0}";

            if (deseaseCode == "11") drAdd[1] = "Дифтерия";
            if (deseaseCode == "10") drAdd[1] = "Коклюш";
            if (deseaseCode == "4") drAdd[1] = "Полиомиелит";
            if (deseaseCode == "18") drAdd[1] = "Гепатит В";
            if (deseaseCode == "6") drAdd[1] = "Паротит";
            if (deseaseCode == "5") drAdd[1] = "Корь";
            if (deseaseCode == "30") drAdd[1] = "Краснуха";

            drAdd[1] = String.Format("{0}{1}", drAdd[1], deseaseAppendix);
            var i = year;

            if (i > 2005 && peopleGroup == "30")
            {
                peopleGroup = "37,38,39";
            }

            var dtr1 = dtData[year - i].Select(String.Format(templateSelect, deseaseCode, injectionKind, peopleGroup));
            var dtr2 = dtData[year - i].Select(String.Format(templateSelect, "99", "99", peopleGroup));

            if (dtr1.Length > 0 && dtr2.Length > 0)
            {
                for (var j = 0; j < dtr2.Length; j++)
                {
                    double percent;
                    if (needMultiCount)
                    {
                        var sum1 = GetSum(dtr1, dtr2[j]["area"].ToString());
                        var sum2 = GetSum(dtr2, dtr2[j]["area"].ToString());
                        
                        if (sum2 > 0)
                        {
                            percent = sum1/sum2;
                            if (percent < 0.9495 && i == year)
                            {
                                AddToBad(dtr2[j]["area"].ToString(), percent);
                            }

                            drAdd[2] = percent;
                        }
                    }
                    else
                    {
                        var drFind2 = dtr2[j];
                        var drFind1 = supportClass.FindRowInSelection(dtr1, dtr2[j]["area"].ToString(), "area");

                        if (supportClass.CheckValue(drFind1) && supportClass.CheckValue(drFind2))
                        {
                            percent = Convert.ToDouble(drFind1[0])/Convert.ToDouble(drFind2[0]);
                            
                            if (percent < 0.9495)
                            {
                                if (i == year) AddToBad(dtr2[j]["area"].ToString(), percent);
                            }

                            drAdd[2] = percent;
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

            for (var i = 0; i < drs.Length; i++)
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
                var drBad = dtBadSubjects.Rows.Add();
                drBad[0] = GetAreaNameByCode(subjectName);
                drBad[1] = 100 * measureValue;
            }
        }

        protected virtual string GetAreaNameByCode(string code)
        {
            var result = String.Empty;
            var dtrArea = dataObject.dtAreaFull.Select(String.Format("kod = {0}", code));

            if (dtrArea.Length > 0)
            {
                result = dtrArea[0][0].ToString();
            }

            return result.Trim();
        }

        protected virtual string GetBadSubjects()
        {
            var result = String.Empty;
            var drs = dtBadSubjects.Select(String.Empty, "percent desc");

            for (var i = 0; i < drs.Length; i++)
            {
                result = String.Format("{0}, {1} ({2:N2})", result, drs[i][0], drs[i][1]);
            }

            return result.TrimStart(',').TrimStart(' ');
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            grid.Width = Unit.Empty;

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

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                var cell = e.Row.Cells[i];

                if (cell.Value != null && cell.Value.ToString().Length > 0)
                {
                    double value;
                    if (Double.TryParse(dtGrid.Rows[injectionNumber][2].ToString(), out value))
                    {
                        cell.Style.BackgroundImage = value < 0.95 ? "~/images/ballRedBB.png" : "~/images/ballGreenBB.png";
                    }
                    
                    injectionNumber++;
                }

                cell.Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px; padding-top: 1px; padding-bottom: 1px";
            }
            e.Row.Cells[0].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px; padding-top: 1px; padding-bottom: 1px";
        }
        #endregion

        #region 1

        private DataRow GetRegionRow()
        {
            return supportClass.FindDataRow(tblRegionData, mapName, tblRegionData.Columns[0].ColumnName);            
        }

        private string[] GetRankText(int peopleGroup)
        {
            var result = new string[2];
            var rowSubject = GetRegionRow();
            
            if (rowSubject != null)
            {
                int subjRelIndex;
                switch (peopleGroup)
                {
                    case 1 :
                        subjRelIndex = 8;
                        break;
                    case 3:
                        subjRelIndex = 16;
                        break;
                    default:
                        subjRelIndex = 2;
                        break;
                }

                var rowsFO = tblRegionData.Select(
                    String.Format("{0} = '{1}'", tblRegionData.Columns[6].ColumnName, rowSubject[6]),
                    tblRegionData.Columns[subjRelIndex].ColumnName + " desc");

                var maxFoRank = rowsFO.Length;
                var maxRfRank = tblRegionData.Select(String.Format("{0}<>''", tblRegionData.Columns[6].ColumnName)).Length;

                var curRfRank = Convert.ToInt32(rowSubject[subjRelIndex + 1]);
                var curFoRank = Array.IndexOf(rowsFO, rowSubject) + 1;

                result[0] = Convert.ToString(curFoRank);
                result[1] = Convert.ToString(curRfRank);

                var imgTemplate = "{0} <img src=\"../../../images/{1}.png\">";

                if (curRfRank == maxRfRank)
                {
                    result[1] = String.Format(imgTemplate, result[1], "starYellow");
                }
                else if (curRfRank == 1)
                {
                    result[1] = String.Format(imgTemplate, result[1], "starGray");
                }
                if (curFoRank == maxFoRank)
                {
                    result[0] = String.Format(imgTemplate, result[0], "starYellow");
                }
                else if (curFoRank == 1)
                {
                    result[0] = String.Format(imgTemplate, result[0], "starGray");
                }
            }

            return result;
        }

        private DataTable AddGroupGridRow(DataTable tblGrid, DataRow subjRow, int peopleGroup, string[] rankText)
        {
            var subjRelIndex = 23;
            var subjAbsIndex = 1;
            var groupCaption = "Все";

            switch (peopleGroup)
            {
                case 1:
                    groupCaption = "Дети";
                    subjRelIndex = 13;
                    subjAbsIndex = 7;
                    break;
                case 3:
                    groupCaption = "Взрослые";
                    subjRelIndex = 21;
                    subjAbsIndex = 15;
                    break;
            }

            var grownDescr = Convert.ToDouble(subjRow[subjRelIndex]) <= 1 ? "снижение" : "рост";
            var clue = "на";

            if (Convert.ToDouble(subjRow[subjRelIndex]) > 1.5)
            {
                clue = "в";
            }

            var row = tblGrid.NewRow();
            row[0] = groupCaption;
            row[1] = String.Format("{0:N0}", subjRow[subjAbsIndex]);
            row[2] = String.Format("{0:N0}", subjRow[subjAbsIndex + 1]);
            row[3] = String.Format("ранг РФ {0} ранг {1} {2}", rankText[1], shortFoName, rankText[0]);
            row[4] = String.Format("{0} {2} {1}", grownDescr, subjRow[subjRelIndex + 1], clue).Replace("-", String.Empty);

            tblGrid.Rows.Add(row);
            return tblGrid;
        }

        private void BindFirstLabel()
        {
            var subjRow = GetRegionRow();
            var totalCountCur = Convert.ToInt32(subjRow[1]);
            var monthAppendix1 = supportClass.GetMonthLabelFull(month, 0) + " ";
            const string yearAppendix1 = "г.";
            BindStructureTagCloud(totalCountCur);

            var dtInfectionGrid = new DataTable();
            dtInfectionGrid.Columns.Add(new DataColumn(" ", typeof (string)));
            dtInfectionGrid.Columns.Add(new DataColumn("Заболевших", typeof (string)));
            dtInfectionGrid.Columns.Add(new DataColumn("  ", typeof (string)));
            dtInfectionGrid.Columns.Add(new DataColumn("На 100 тысяч населения", typeof (string)));
            dtInfectionGrid.Columns.Add(new DataColumn("К прошлому году", typeof (string)));

            var matureRankText = GetRankText(3);
            var childRankText = GetRankText(1);
            var allRankText = GetRankText(0);

            dtInfectionGrid = AddGroupGridRow(dtInfectionGrid, subjRow, 3, matureRankText);
            dtInfectionGrid = AddGroupGridRow(dtInfectionGrid, subjRow, 1, childRankText);
            dtInfectionGrid = AddGroupGridRow(dtInfectionGrid, subjRow, 0, allRankText);

            UltraWebGrid1.Style.Add("margin-left", "-10px");
            UltraWebGrid1.DataSource = dtInfectionGrid;
            UltraWebGrid1.InitializeLayout += UltraWebGrid1_InitializeLayout;
            UltraWebGrid1.InitializeRow += UltraWebGrid1_InitializeRow;
            UltraWebGrid1.DataBind();

            var percent = Convert.ToDouble(subjRow[23]);
            var appendix = "меньше";
            var imageName = "arrowGreenDownBB";

            if (percent > 1)
            {
                appendix = "больше";
                imageName = "arrowRedUpBB";
            }

            var strBuilder = new StringBuilder();
            strBuilder.Append(String.Format(
                "{5}За&nbsp;<span style=\"color: white\"><b>{7}{0}{8}</b></span>&nbsp;{1} {10}&nbsp;<span style=\"color: white\"><b>{2:N0}</b></span>&nbsp;{9} инфекционных и&nbsp;паразитарных болезней (без педикулеза), что на&nbsp;<span style=\"color: white\"><b>{3:N2}</b></span>&nbsp;<img width='13' height='18' src=\"../../../images/{6}.png\">&nbsp;{4}, чем в предыдущем году.",
                year, 
                mapNameReport, 
                Convert.ToDouble(subjRow[1]),
                Convert.ToString(subjRow[24]).Replace("-", String.Empty), 
                appendix,
                String.Empty, 
                imageName, 
                monthAppendix1, 
                yearAppendix1,
                GetTimesPresentation(Convert.ToDouble(subjRow[1])),
                "зарегистрировано"));

            LabelPart1Text.Text = strBuilder.ToString();
        }

        static void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            var cell = e.Row.Cells[4];

            if (cell.Value.ToString().Contains("рост"))
            {
                cell.Style.BackgroundImage = "~/images/arrowRedUpBB.png";
            }
            else if (cell.Value.ToString().Contains("снижение"))
            {
                cell.Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
            }

            for (var i = 0; i < e.Row.Cells.Count; i++)
            {
                cell.Style.CustomRules =
                    "background-repeat: no-repeat; background-position: 10px center; padding-left: 20px; height 36px; padding-bottom: 2px";
            }

            e.Row.Cells[2].Style.BorderDetails.ColorRight = Color.Black;
            e.Row.Cells[3].Style.BorderDetails.ColorLeft = Color.Black;
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.RowHeightDefault = 36;
            e.Layout.Bands[0].Columns[0].Width = 120;
            e.Layout.Bands[0].Columns[1].Width = 110;
            e.Layout.Bands[0].Columns[2].Width = 65;
            e.Layout.Bands[0].Columns[3].Width = 253;
            e.Layout.Bands[0].Columns[4].Width = 200;

            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.VerticalAlign = VerticalAlign.Bottom;
            e.Layout.Bands[0].Columns[1].CellStyle.VerticalAlign = VerticalAlign.Bottom;
            e.Layout.Bands[0].Columns[2].CellStyle.VerticalAlign = VerticalAlign.Bottom;
            e.Layout.Bands[0].Columns[3].CellStyle.VerticalAlign = VerticalAlign.Bottom;
            e.Layout.Bands[0].Columns[4].CellStyle.VerticalAlign = VerticalAlign.Bottom;

            e.Layout.Bands[0].Columns[2].Header.Style.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[3].Header.Style.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Grid.DisplayLayout.HeaderStyleDefault.Height = 30;
        }
        #endregion

        #region Структура инфекционной заболеваемости
        
        private void BindStructureTagCloud(double totalCount)
        {
            const int outCount = 10;
            var color1 = Color.Honeydew;
            var color10 = Color.Silver;

            TagCloud2.ForeColors = new Collection<Color>();
            
            var infections = new Dictionary<string, Tag>();

            var dtResult = new DataTable();

            var dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            var tblDies = dataObject.SortDataSet(tblDiesData, 1, true);
            double pctTotal = 0;

            TagCloud2.ForeColors.Add(color1);

            for (var i = 0; i < outCount; i++)
            {
                var rowDesease = tblDies.Rows[i];
                var pctValue = 100 * Convert.ToDouble(rowDesease[1]) / totalCount;
                double absValue = Convert.ToInt32(rowDesease[1]);
                var relValue = Convert.ToDouble(rowDesease[2]);
                var tag = new Tag
                              {
                                  key = String.Format("{0} ({1:N2}%)", rowDesease[0], pctValue),
                                  weight = (int) (pctValue),
                                  toolTip =
                                      String.Format(
                                          "{0}<br/>{1:N2}%<br/>заболевших: {2:N0}<br/>на 100 тыс. населения: {3:N2}",
                                          rowDesease[0], pctValue, absValue, relValue)
                              };
                infections.Add(tag.key, tag);
                pctTotal += pctValue;
                var deseaseCode = Convert.ToString(rowDesease[12]);

                if (i > 0)
                {
                    TagCloud2.ForeColors.Add(dataRotator.deseasesColorRelation[deseaseCode]);
                }
            }

            var tagOther = new Tag
            {
                key = String.Format("{0} ({1:N2}%)", "Прочие", 100 - pctTotal),
                weight = (int)(100 - pctTotal),
                toolTip = String.Format("{0}<br/>{1:N2}%", "Прочие", 100 - pctTotal)
            };

            infections.Add(tagOther.key, tagOther);
            TagCloud2.ForeColors.Add(color10);
            TagCloud2.Render(infections);
        }

        private static string GetTimesPresentation(double count)
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

        #endregion

        #region 3
        private void BindOverflowTagCloud()
        {
            Label3.Text = String.Format("Актуальные инфекции (превышение уровня {0})", shortFoName);
            BindData();
        }

        private void FillData()
        {
            // Структура данных по болезням
            var diesCodes = dataRotator.GetDeseaseCodes(0);
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctDeseaseName;
            dataObject.mainColumnRange = diesCodes;
            // 01 Заболевание по субъекту текущий год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, mapName, groupName, String.Empty);
            // 02
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "1");
            // 03 Заболевание по субъекту прошлый год
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year2, month, mapName, groupName, String.Empty);
            // 04
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "3");
            // 05 ФО
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, foName, groupName, String.Empty);
            // 06
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "5");
            // 07 РФ
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctAbs,
                year1, month, rfName, groupName, String.Empty);
            // 08
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation,
                "7");
            // 09 Рост
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
                "2", "8");
            // 10
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText, "1", "3", "2", "4");
            // 11
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
               "2", "6");
            // 12
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctCodeDesease,"0");

            tblDiesData = dataObject.FillData(9);

            // Структура данных по территориям

            dataObject.InitObject();
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            // 01 абс
            dataObject.AddColumn(
                SGMDataObject.DependentColumnType.dctAbs,
                year1, month, String.Empty, groupName, diesCodes);
            // 02 отн
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "1");
            // 03 ранг
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRank, "2");
            // 04 абс
            dataObject.AddColumn(
                SGMDataObject.DependentColumnType.dctAbs,
                year2, month, String.Empty, groupName, diesCodes);
            // 05 отн
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "4");
            // 06 ФО
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");

            // Дети
            // 07 абс
            dataObject.AddColumn(
                SGMDataObject.DependentColumnType.dctAbs,
                year1, month, String.Empty, "1", diesCodes);
            // 08 отн
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "7");
            // 09
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRank, "8");
            // 10
            dataObject.AddColumn(
                SGMDataObject.DependentColumnType.dctAbs,
                year2, month, String.Empty, "1", diesCodes);
            // 11 отн
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "10");
            // 12
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRank, "11");
            // 13 Рост
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
                "7", "10");
            // 14
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText, "7", "10", "8", "11");

            // Взрослые

            // 15 абс
            dataObject.AddColumn(
                SGMDataObject.DependentColumnType.dctAbs,
                year1, month, String.Empty, "3", diesCodes);
            // 16 отн
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "15");
            // 17
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRank, "16");
            // 18
            dataObject.AddColumn(
                SGMDataObject.DependentColumnType.dctAbs,
                year2, month, String.Empty, "3", diesCodes);
            // 19 отн
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRelation, "18");
            // 20
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctRank, "19");
            // 21 Рост
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
                "15", "18");
            // 22
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText, "15", "18", "16", "19");

            // 23 Рост
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercent,
                "1", "4");
            // 24
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctUpDownPercentText, "1", "4", "2", "5");

            tblRegionData = dataObject.FillData();
        }

        protected virtual void BindData()
        {
            var dataColumn = dtChart.Columns.Add();
            dataColumn.DataType = typeof (string);
            dataColumn = dtChart.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");
            dataColumn = dtChart.Columns.Add();
            dataColumn.DataType = Type.GetType("System.Double");

            var filterStr = String.Format("{0} > 1", tblDiesData.Columns[11].ColumnName);
            var tblDies = dataObject.FilterDataSet(tblDiesData, filterStr);
            tblDies = dataObject.SortDataSet(tblDies, 11, true);

            var infections = new Dictionary<string, Tag>();

            foreach (DataRow rowDies in tblDies.Rows)
            {
                var tag = new Tag();
                var name = rowDies[0].ToString();
                tag.key = name;
                var grown = Convert.ToDouble(rowDies[11]) - 1;
                tag.weight = (int) (grown * 100);
                var grownDescr = String.Format("на {0:P2}", grown);

                if (grown > 1)
                {
                    grownDescr = String.Format("в {0:N1} раза", grown + 1);
                }

                tag.key = String.Format("{0} ({1})", tag.key, grownDescr);
                tag.toolTip =
                    String.Format(
                        "{0}<br/>заболевших: {1}<br/>на 100 тыс. населения: {2:N2}<br/>превышение уровня {3} {4}<br/>на 100 тыс. населения {3}: {5:N2}<br/>на 100 тыс. населения РФ: {6:N2}",
                        name,
                        rowDies[1],
                        rowDies[2],
                        shortFoName,
                        grownDescr,
                        rowDies[6],
                        rowDies[8]);

                infections.Add(tag.key, tag);
            }

            if (infections.Count > 14)
            {
                TagCloud1.startFontSize = 10;
            }

            TagCloud1.Render(infections);
        }

        #endregion
    }
}
