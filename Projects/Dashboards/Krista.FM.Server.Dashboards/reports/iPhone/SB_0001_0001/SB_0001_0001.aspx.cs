using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Xml;
using Krista.FM.Server.Dashboards.Components.Components;
using Infragistics.UltraGauge.Resources;
using System.Web.UI.HtmlControls;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SB_0001_0001 : CustomReportPage
    {
        #region Поля

        private DataTable gridDt;
        private DataTable bankTerritory;
        private DataTable sberbankFacility;
        private DataTable activityEvaluation;
        private DataTable facilityEvaluation;

        private List<string> bankList;
        private List<string> facilityList;

        private Dictionary<string, int> bankDicitionary;
        private Dictionary<string, string> regionBankDicitionary;
        private Dictionary<string, string> facilityEvaluaitonDictionary;
        private Dictionary<string, string> facilityDescriptionDictionary;
        private Dictionary<string, bool> possibleEvaluationRegions;
        private Dictionary<string, double> avgFacilityDictionary = new Dictionary<string, double>();
        private Dictionary<string, int> regionFacilityDictionary = new Dictionary<string, int>();
        private Dictionary<string, bool> regionAgreements;
        private Dictionary<string, bool> regionProgressStrategies;
        private Dictionary<string, string> regionAgreementCharacteristics;
        private Dictionary<string, string> regionProgressStrategyCharacteristics;

        private string sberbankFacilityColumn = "sberbank-facility";
        private string evaluationColumn = "evaluation";
        private string descriptionColumn = "description";
        private string communicativeEvaluationColumn = "communicative-evaluation";
        private string federalSubjectColumn = "federal-subject";
        private string bankColumn = "bank";
        private string agreementColumn = "agreement";
        private string progressStrategyColumn = "progress-strategy";
        private string agreementCharacteristicColumn = "agreement-characteristic";
        private string progressStrategyCharacteristicColumn = "progress-strategy-characteristic";
        private string indicatorNameColumn = "IndicatorName";
        private string regionFacilityCountColumn = "RegionFacilityCount";
        private string gaugeColumn = "Gauge";

        private int regionAgreementCount;
        private int regionProgressStrategyCount;

        #endregion

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            LoadXmlData();

            GenerateDictionaries();

            GaugeDataBind();
            GridDataBind();
        }

        #region Обработчики гейджа

        private void GaugeDataBind()
        {
            double currentBankAvg = 0;

            RankCalculator rankCalculator = new RankCalculator(RankDirection.Asc);
            AvgCalculator avgCalculator = new AvgCalculator();

            foreach (string bank in bankList)
            {
                bool isCurrentBank = (bank == UserParams.Terbank.Value);

                string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, bank);
                DataRow[] terrRows = activityEvaluation.Select(sqlFilter);

                foreach (DataRow terrRow in terrRows)
                {
                    if (terrRow[communicativeEvaluationColumn] != DBNull.Value &&
                        terrRow[communicativeEvaluationColumn].ToString() != String.Empty)
                    {
                        double value;
                        if (Double.TryParse(terrRow[communicativeEvaluationColumn].ToString(), out value))
                        {
                            avgCalculator.AddValue(value);
                        }
                    }
                }
                rankCalculator.AddItem(bank, avgCalculator.GetAverage());

                if (isCurrentBank)
                {
                    currentBankAvg = avgCalculator.GetAverage();
                }
                avgCalculator.Reset();
            }
            
            RadialGauge gauge = (RadialGauge)UltraGauge2.Gauges[0];
            RadialGaugeScale scale = (RadialGaugeScale)gauge.Scales[0];

            scale.Axis.SetTickmarkInterval(1);
            scale.MinorTickmarks.Frequency = 1;
            scale.Labels.Frequency = 1;
            scale.Axis.SetStartValue(0);
            scale.Axis.SetEndValue(5);

            scale.Markers[0].Precision = 0.1;

            scale.Markers[0].Value = currentBankAvg;       

            ActivityValueLabel.Text = String.Format("Коммуникативная активность:&nbsp;<span class='DigitsValueXLarge'>{0:N1}</span>", currentBankAvg);
            ActivityRankLabel.Text = String.Format("ранг активности:&nbsp;<span class='DigitsValueXLarge'>{0:N0}</span>&nbsp;{1}",
                  rankCalculator.GetRank(currentBankAvg), GetBankStar(rankCalculator.GetRank(currentBankAvg), rankCalculator.GetWorseRank()));
        }

        #endregion

        #region Обработчики грида

        private void GridDataBind()
        {
            gridDt = new DataTable();
            DataColumn column = new DataColumn(indicatorNameColumn, typeof(String));
            gridDt.Columns.Add(column);
            column = new DataColumn(regionFacilityCountColumn, typeof(String));
            gridDt.Columns.Add(column);

            foreach (string region in possibleEvaluationRegions.Keys)
            {
                column = new DataColumn(region, typeof(String));
                gridDt.Columns.Add(column);
            }

            column = new DataColumn(gaugeColumn, typeof(String));
            gridDt.Columns.Add(column);

            // добавляем строку с гербами
            DataRow row = gridDt.NewRow();
            row[indicatorNameColumn] = String.Empty;
            row[regionFacilityCountColumn] = String.Empty;
            foreach (string subject in possibleEvaluationRegions.Keys)
            {
                //double opacity = possibleEvaluationRegions[subject] ? 1 : 0.4;
                row[subject] = String.Format("{0}<br/>{1}", GetHeraldImage(subject, 1), GetHeraldTitle(subject, 1));
            }
            gridDt.Rows.Add(row);

            // добавляем строку с заключенными соглашениями
            row = gridDt.NewRow();
            row[indicatorNameColumn] = "Заключены соглашения с субъектами РФ";
            row[regionFacilityCountColumn] = String.Format("<span class='DigitsValue'>{0}</span>&nbsp;из&nbsp;<span class='DigitsValue'>{1}</span>", regionAgreementCount, possibleEvaluationRegions.Count); ;
            foreach (string subject in possibleEvaluationRegions.Keys)
            {
                row[subject] = GetBallImage(regionAgreements[subject], regionAgreementCharacteristics[subject]);
            }
            gridDt.Rows.Add(row);

            // добавляем строку с утвержденными стратегиями работы
            row = gridDt.NewRow();
            row[indicatorNameColumn] = "Утверждены стратегии работы с субъектами РФ";
            row[regionFacilityCountColumn] = String.Format("<span class='DigitsValue'>{0}</span>&nbsp;из&nbsp;<span class='DigitsValue'>{1}</span>", regionProgressStrategyCount, possibleEvaluationRegions.Count); ;
            foreach (string subject in possibleEvaluationRegions.Keys)
            {
                row[subject] = GetBallImage(regionProgressStrategies[subject], regionProgressStrategyCharacteristics[subject]);
            }
            //row[gaugeColumn] = String.Empty;
            gridDt.Rows.Add(row);

            // забиваем остальные строки
            foreach (string facility in avgFacilityDictionary.Keys)
            {
                row = gridDt.NewRow();
                row[indicatorNameColumn] = facility;

                row[regionFacilityCountColumn] = String.Format("<span class='DigitsValue'>{0}</span>&nbsp;из&nbsp;<span class='DigitsValue'>{1}</span>", regionFacilityDictionary[facility], possibleEvaluationRegions.Count);

                foreach (string subject in possibleEvaluationRegions.Keys)
                {
                    string key = String.Format("{0};{1}", subject, facility);
                    string tooltip = String.Empty;
                    if (facilityDescriptionDictionary.ContainsKey(key))
                    {
                        tooltip = facilityDescriptionDictionary[key];
                    }
                    row[subject] = GetBallImage(facilityEvaluaitonDictionary.ContainsKey(key), tooltip);
                }

                row[gaugeColumn] = avgFacilityDictionary[facility];

                gridDt.Rows.Add(row);
            }

            GridTd.Controls.Add(GenerateHtmlTable(gridDt));
        }

        private static string GetBallImage(bool performed, string hint)
        {
            return String.Format("<img src='../../../images/{0}.png'>;{1}", performed ? "ballGreenBB" : "ballGrayBB", hint);
        }

        private static string GetHeraldImage(string subject, double opacity)
        {
            return String.Format("<img height='65px' src='{0}' style='opacity:{1};filter:alpha(opacity={2})' title='{3}'>", GetRegionImg(subject, "../../../"),
                opacity.ToString().Replace(",", "."), 100 * opacity, subject);
        }

        private static string GetBankStar(int rank, int worseRank)
        {
            if (rank == 1)
            {
                return "<img height='20px' src='../../../images/StarYellow.png' title='Лучшая активность среди банков' >";
            }
            else if (rank == worseRank)
            {
                return "<img height='20px' src='../../../images/StarGray.png' title='Худшая активность среди банков' >";
            }
            return String.Empty;
        }

        private static string GetHeraldTitle(string subject, double opacity)
        {
            return String.Format("<span style='opacity:{1};filter:alpha(opacity={2})' class='SberbankHeraldTitle'>{0}</span>", RegionsNamingHelper.ShortName(GetRegionName(subject)),
                opacity.ToString().Replace(",", "."), 100 * opacity);
        }

        private Table GenerateHtmlTable(DataTable dt)
        {
            Table table = new Table();
            table.CssClass = "HtmlTableSb";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool isHeraldRow = (i == 0);
                DataRow dtRow = dt.Rows[i];

                TableRow htmlRow = new TableRow();
                htmlRow.Height = Unit.Pixel(50);
                int colNum = 0;
                foreach (DataColumn dtColumn in dt.Columns)
                {
                    if (dtColumn.ColumnName == gaugeColumn ||
                        dtColumn.ColumnName == regionFacilityCountColumn)
                    {
                    }
                    else
                    {
                        TableCell htmlCell = new TableCell();

                        if (dtRow[dtColumn.ColumnName] != DBNull.Value)
                        {
                            double value;
                            //if (dtColumn.ColumnName == gaugeColumn && Double.TryParse(dtRow[dtColumn.ColumnName].ToString(), NumberStyles.Any, NumberFormatInfo.CurrentInfo, out value))
                            if (dtColumn.ColumnName == indicatorNameColumn)
                            {
                                if (dtRow[gaugeColumn] != DBNull.Value &&
                                Double.TryParse(dtRow[gaugeColumn].ToString(), NumberStyles.Any, NumberFormatInfo.CurrentInfo, out value) &&
                                    !isHeraldRow)
                                {
                                    htmlCell.Text = String.Format("<div style='float: left; width: 100%'>{0}<div><div style='float: left;' class='InformationText'><nobr>суб-ты&nbsp;{2};&nbsp;оценка&nbsp;<span class='DigitsValue'>{1:N1}</span></nobr><div>", dtRow[dtColumn.ColumnName], value, dtRow[regionFacilityCountColumn]);
                                }
                                else if (!isHeraldRow)
                                {
                                    htmlCell.Text = String.Format("<div style='float: left; width: 100%'>{0}<div><div style='float: left;' class='InformationText'><nobr>суб-ты&nbsp;{1}</nobr><div>", dtRow[dtColumn.ColumnName], dtRow[regionFacilityCountColumn]);
                                }
                                else
                                {
                                    htmlCell.Text = dtRow[dtColumn.ColumnName].ToString();
                                }
                            }
                            else if (isHeraldRow)
                            {
                                HtmlGenericControl div = new HtmlGenericControl("div");
                                div.InnerHtml = dtRow[dtColumn.ColumnName].ToString();
                                htmlCell.Controls.Add(div);
                            }
                            else
                            {
                                HtmlGenericControl div = new HtmlGenericControl("div");
                                div.ID = String.Format("TagCloud_tag{0}_{1}", i, colNum);
                                div.InnerHtml =  dtRow[dtColumn.ColumnName].ToString().Split(';')[0];
                                TooltipHelper.AddToolTip(div, String.Format("<span style=\"font-family: Arial; font-size: 14pt\">{0}</span>", iPadBricks.iPadBricks.iPadBricksHelper.GetWarpedHint(dtRow[dtColumn.ColumnName].ToString().Split(';')[1])), this.Page);
                                htmlCell.Controls.Add(div);
                            }
                            htmlCell.HorizontalAlign = dtColumn.ColumnName == indicatorNameColumn ? HorizontalAlign.Left : HorizontalAlign.Center;

                            htmlCell.CssClass = (isHeraldRow) ? "HtmlTableContourTd" : "HtmlTableTd";
                            htmlCell.Width = GetColumnWidth(dtColumn.ColumnName);
                            colNum++;
                        }

                        htmlRow.Cells.Add(htmlCell);
                    }
                }
                table.Rows.Add(htmlRow);
            }
            return table;
        }

        private Unit GetColumnWidth(string columnName)
        {
            if (columnName == indicatorNameColumn)
            {
                return Unit.Pixel(700);
            }
            if (columnName == regionFacilityCountColumn)
            {
                return Unit.Pixel(50);
            }
            return Unit.Pixel(80);
        }

        private static string GetRegionImg(string subjectName, string prefixPath)
        {

            string subjectId = CustomParams.GetSubjectIdByName(GetRegionName(subjectName));
            return String.Format("{1}/images/Heralds/{0}.png", subjectId, prefixPath);
        }

        private static string GetRegionName(string subjectName)
        {
            subjectName = subjectName.Replace("Республика Татарстан", "Республика Татарстан (Татарстан)");
            subjectName = subjectName.Replace("Ханты-Мансийский автономный округ - Югра", "Ханты-Мансийский автономный округ");
            subjectName = subjectName.Replace("Удмуртская Республика", "Удмуртская республика");
            subjectName = subjectName.Replace("Астраханская область", "Астраханская область");
            subjectName = subjectName.Replace("Республика Северная Осетия -Алания", "Республика Северная Осетия-Алания");
            subjectName = subjectName.Replace("Чеченская Республика", "Чеченская республика");
            subjectName = subjectName.Replace("Республика Адыгея", "Республика Адыгея (Адыгея)");
            subjectName = subjectName.Replace("Чувашская Республика", "Чувашская Республика-Чувашия");

            return subjectName;
        }

        //private System.Web.UI.Control NewLinearGauge(string bankName, double value, int index)
        //{
        //    LinearGaugeIndicator gauge = (LinearGaugeIndicator)Page.LoadControl("../../../Components/Gauges/LinearGaugeIndicator.ascx");

        //    gauge.Width = 200;
        //    gauge.Height = 60;
        //    gauge.SetRange(0, 5, 1);
        //    gauge.MarkerPrecision = 0.01;
        //    gauge.IndicatorValue = value;
        //    gauge.TitleText = bankName;
        //    gauge.SetImageUrl(index);
        //    gauge.GaugeContainer.Width = "200px";
        //    gauge.GaugeContainer.Height = "60px";
        //    gauge.Tooltip = String.Format("Средняя оценка: {0:N1}", value);
        //    gauge.SetMarkerAnnotation(value);

        //    return gauge;
        //}

        #endregion

        #region Эагрузка из XML

        public void LoadXmlData()
        {
            string dataPath = Server.MapPath("../../../data/Sberbank/");
            string xmlFileName = String.Format("{0}/dataset.xml", dataPath);
            string xsdFileName = String.Format("{0}/dataset.xsd", dataPath);

            DataSet ds = new DataSet();
            ds.ReadXmlSchema(xsdFileName);

            XmlDataDocument xmlDataDocument = new XmlDataDocument(ds);
            xmlDataDocument.Load(xmlFileName);

            bankTerritory = xmlDataDocument.DataSet.Tables["bank-territory"];
            sberbankFacility = xmlDataDocument.DataSet.Tables["sberbank-facility"];
            activityEvaluation = GetActivityEvaluation(xmlDataDocument.DataSet.Tables["activity-evaluation"]);
            facilityEvaluation = GetFacilityEvaluation(xmlDataDocument.DataSet.Tables["facility-evaluation"]);

            // заполняем список банков
            bankList = new List<string>();
            foreach (DataRow row in bankTerritory.Rows)
            {
                string bank = row[bankColumn].ToString().TrimEnd(' ');

                if (!bankList.Contains(bank))
                {
                    bankList.Add(bank);
                }
            }
        }

        private void GenerateDictionaries()
        {
            // заполняем возможность оценки регионов банка и словари заключения договоров
            possibleEvaluationRegions = new Dictionary<string, bool>();
            regionAgreements = new Dictionary<string, bool>();
            regionProgressStrategies = new Dictionary<string, bool>();
            regionAgreementCharacteristics = new Dictionary<string, string>();
            regionProgressStrategyCharacteristics = new Dictionary<string, string>();

            string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, UserParams.Terbank.Value);
            DataRow[] activityRows = activityEvaluation.Select(sqlFilter);

            regionAgreementCount = 0;
            regionProgressStrategyCount = 0;
            foreach (DataRow row in activityRows)
            {
                string subject = row[federalSubjectColumn].ToString().TrimEnd(' ');
                string evaluation = row[communicativeEvaluationColumn].ToString();

                decimal value;
                bool isPossible = Decimal.TryParse(evaluation, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out value);

                possibleEvaluationRegions.Add(subject, isPossible);

                if (row[agreementColumn] != DBNull.Value && row[agreementColumn].ToString() != "Не заключено")
                {
                    regionAgreementCount++;
                    regionAgreements.Add(subject, true);
                }
                else
                {
                    regionAgreements.Add(subject, false);
                }

                regionAgreementCharacteristics.Add(subject, GetStringRowValue(row, agreementCharacteristicColumn));
                regionProgressStrategyCharacteristics.Add(subject, GetStringRowValue(row, progressStrategyCharacteristicColumn));

                if (row[progressStrategyColumn] != DBNull.Value && row[progressStrategyColumn].ToString() != "Не существует")
                {
                    regionProgressStrategyCount++;
                    regionProgressStrategies.Add(subject, true);
                }
                else
                {
                    regionProgressStrategies.Add(subject, false);
                }
            }

            // заполняем список механизмов
            facilityList = new List<string>();
            foreach (DataRow row in sberbankFacility.Rows)
            {
                string facility = row["name"].ToString().TrimEnd(' ');

                if (!facilityList.Contains(facility))
                {
                    facilityList.Add(facility);
                }
            }

            // заполняем словарь оценок и описаний механизмов
            facilityEvaluaitonDictionary = new Dictionary<string, string>();
            facilityDescriptionDictionary = new Dictionary<string, string>();
            foreach (DataRow row in facilityEvaluation.Rows)
            {
                string facility = row[sberbankFacilityColumn].ToString();
                string subject = row[federalSubjectColumn].ToString().TrimEnd(' ');
                string evaluation = row[evaluationColumn].ToString();
                string description = row[descriptionColumn].ToString();

                string key = String.Format("{0};{1}", subject, facility);

                if (!facilityEvaluaitonDictionary.ContainsKey(key))
                {
                    facilityEvaluaitonDictionary.Add(key, evaluation);
                }

                if (!facilityDescriptionDictionary.ContainsKey(key))
                {
                    facilityDescriptionDictionary.Add(key, description);
                }
            }

            // считаем среднее по регионам банка
            AvgCalculator avgCalculator = new AvgCalculator();
            foreach (string facility in facilityList)
            {
                avgCalculator.Reset();
                int regionCount = 0;
                foreach (string region in possibleEvaluationRegions.Keys)
                {
                    string subject = region;
                    string key = String.Format("{0};{1}", subject, facility);
                    if (facilityEvaluaitonDictionary.ContainsKey(key))
                    {
                        double value;
                        if (Double.TryParse(facilityEvaluaitonDictionary[key], out value))
                        {
                            avgCalculator.AddValue(value);
                            regionCount++;
                        }
                    }
                }
                avgFacilityDictionary.Add(facility, avgCalculator.GetAverage());
                regionFacilityDictionary.Add(facility, regionCount);
            }
        }

        private DataTable GetActivityEvaluation(DataTable xmlDt)
        {
            DataTable dt = xmlDt.Copy();

            DataColumn newBankColumn = new DataColumn(bankColumn, typeof(String));
            dt.Columns.Add(newBankColumn);

            regionBankDicitionary = new Dictionary<string, string>();
            foreach (DataRow row in bankTerritory.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                string bank = row[bankColumn].ToString();
                if (!regionBankDicitionary.ContainsKey(subject))
                {
                    regionBankDicitionary.Add(subject, bank);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                if (regionBankDicitionary.ContainsKey(subject))
                {
                    row[bankColumn] = regionBankDicitionary[subject];
                }
            }

            return dt;
        }

        private DataTable GetFacilityEvaluation(DataTable xmlDt)
        {
            DataTable dt = xmlDt.Copy();

            // дописываем колонку с банками
            DataColumn newBankColumn = new DataColumn(bankColumn, typeof(String));
            dt.Columns.Add(newBankColumn);

            regionBankDicitionary = new Dictionary<string, string>();
            foreach (DataRow row in bankTerritory.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                string bank = row[bankColumn].ToString();
                if (!regionBankDicitionary.ContainsKey(subject))
                {
                    regionBankDicitionary.Add(subject, bank);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                if (regionBankDicitionary.ContainsKey(subject))
                {
                    row[bankColumn] = regionBankDicitionary[subject];
                }
            }

            return dt;
        }

        private void FillComboBank()
        {
            bankDicitionary = new Dictionary<string, int>();
            foreach (string bank in bankList)
            {
                if (!bankDicitionary.ContainsKey(bank))
                {
                    bankDicitionary.Add(bank, 0);
                }
            }
        }

        #endregion

        #region Получение данных из dataTable

        private static string GetStringRowValue(DataRow row, string columnName, string defaultValue)
        {
            if (row != null && row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
            {
                return row[columnName].ToString();
            }
            return defaultValue;
        }

        private static string GetStringRowValue(DataRow row, string columnName)
        {
            return GetStringRowValue(row, columnName, String.Empty);
        }

        #endregion
    }

    public class AvgCalculator
    {
        private double summary = 0;
        private int count = 0;

        public void AddValue(double value)
        {
            summary += value;
            if (value != 0)
            {
                count++;
            }
        }

        public double GetAverage()
        {
            if (count != 0)
                return summary / count;
            return 0;
        }

        public void Reset()
        {
            summary = 0;
            count = 0;
        }
    }
}