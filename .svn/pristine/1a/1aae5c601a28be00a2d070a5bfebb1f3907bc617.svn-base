using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class EO_0001_0002 : CustomReportPage
    {
        private DateTime currentDate;
        private DataTable dt;
        private DataTable violationDt;

        private CustomParam currentPeriod;

        private Dictionary<string, string> violationDictionary;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            currentPeriod = UserParams.CustomParam("current_period");

            currentDate = CubeInfo.GetLastDate(DataProvidersFactory.PrimaryMASDataProvider, "EO_0001_0002_lastDate");
            currentPeriod.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", currentDate, 4);

            CommonGridBrick.BrowserSizeAdapting = false;
            CommonGridBrick.Height = Unit.Empty;
            CommonGridBrick.Width = Unit.Empty;
            CommonGridBrick.RedNegativeColoring = false;
            CommonGridBrick.HeaderVisible = false;
            CommonGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(CommonGrid_InitializeLayout);
            CommonGridBrick.Grid.InitializeRow += new InitializeRowEventHandler(CommonGrid_InitializeRow);

            string shortCustomerName = CustomParam.CustomParamFactory("current_builder_customer").Value.Replace("(заказчик Адресной инвестиционной программы)", String.Empty);
            shortCustomerName = shortCustomerName.Replace("(государственный заказчик объектов Адресной инвестиционной программы)", String.Empty);

            CommonGridCaption.Text = shortCustomerName;
            CustomerCaption.Text = String.Format("на {0:dd.MM.yyyy}", currentDate.AddMonths(1));

            ViolationDataBind();
            CommonGridDataBind();
        }

        #region Обработчики главного грида

        private void CommonGridDataBind()
        {
            string query = DataProvider.GetQueryText("EO_0001_0002_commonGrid");
            dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            if (dt.Rows.Count > 0)
            {
                if (dt.Columns.Count > 0)
                {
                    dt.Columns.RemoveAt(0);
                }
                DataTable newDt = dt.Clone();
                foreach (DataRow row in dt.Rows)
                {
                    newDt.ImportRow(row);
                    newDt.AcceptChanges();

                    string objectCode = GetStringDTValue(row, "Код");
                    if (objectCode != String.Empty && violationDictionary.ContainsKey(objectCode))
                    {
                        DataRow newRow = newDt.NewRow();
                        newRow[0] = row[0];
                        newRow[1] = violationDictionary[objectCode];
                        newRow["Тип строки"] = "-2";

                        newDt.Rows.Add(newRow);
                    }
                }
                newDt.AcceptChanges();

                CommonGridBrick.DataTable = newDt;
            }
        }

        private void CommonGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.White;
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 13;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 11;

            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].Width = 220;
            e.Layout.Bands[0].Columns[1].Width = 220;
            e.Layout.Bands[0].Columns[2].Width = 140;
            e.Layout.Bands[0].Columns[3].Width = 180;

            e.Layout.Bands[0].Columns[4].Hidden = true;
            e.Layout.Bands[0].Columns[5].Hidden = true;
        }

        protected void CommonGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count - 1;
            int rowType = -1;
            if (e.Row.Cells[cellCount].Value != null && e.Row.Cells[cellCount].Value.ToString() != String.Empty)
            {
                rowType = Convert.ToInt32(e.Row.Cells[cellCount].Value);
            }

            if (rowType == 0 || rowType == 4)
            {
                e.Row.Style.BorderDetails.WidthTop = 3;
            }
            else if (rowType == 3)
            {
                e.Row.Style.BorderDetails.WidthBottom = 3;
            }

            for (int i = 0; i < e.Row.Cells.Count - 2; i++)
            {
                bool nameColumn = i == 0;
                bool valueColumn = e.Row.Band.Columns[i].Header.Caption.Contains("Отчет");
                bool indicatorColumn = e.Row.Band.Columns[i].Header.Caption.Contains("Колонка для индикации");
                
                if (nameColumn)
                {
                    string[] valueParts = e.Row.Cells[i].Value.ToString().Split(';');
                    if (valueParts.Length > 1)
                    {
                        string name = valueParts[0];
                        string code = valueParts[1];
                        e.Row.Cells[i].Value = String.Format("<a href='http://monitoring.yanao.ru/site/reports/EO_0001_0003/Index.aspx?paramlist=object_code={1}'>{0}</a>", name, code);
                    }
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }
                if (valueColumn)
                {
                    switch (rowType)
                    {
                        case 0:
                            {
                                string[] valueParts = e.Row.Cells[i].Value.ToString().Split(',');
                                if (valueParts.Length > 1)
                                {
                                    double value1 = GetDoubleValue(valueParts[0]);
                                    double value2 = GetDoubleValue(valueParts[1]);

                                    e.Row.Cells[i].Value = String.Format("{0} \\ {1}",
                                        value1 == 0 ? "-" : value1.ToString("0"),
                                        value2 == 0 ? "-" : value2.ToString("0"));
                                }
                                break;
                            }
                        case 1:
                        case 2:
                        case 3:
                            {
                                e.Row.Cells[i].Value = GetStringValue(e.Row.Cells[i].Value, "N2");
                                break;
                            }
                        case 4:
                            {
                                e.Row.Cells[i].Value = GetStringValue(e.Row.Cells[i].Value, "N0");
                                break;
                            }
                        case -2:
                            {
                                e.Row.Cells[1].ColSpan = 3;
                                if (e.Row.PrevRow != null)
                                {
                                    e.Row.PrevRow.Style.BorderDetails.WidthBottom = 1;
                                }
                                break;
                            }
                    }
                }
                else if (indicatorColumn)
                {
                    switch (rowType)
                    {
                        case 2:
                            {
                                double value = GetDoubleValue(e.Row.Cells[i].Value);
                                e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                                e.Row.Cells[i].Value = GetGaugeTable(value, "от стоимости строительства");
                                break;
                            }
                        case 3:
                            {
                                string[] valueParts = e.Row.Cells[i].Value.ToString().Split(';');
                                if (valueParts.Length > 1)
                                {
                                    double value1 = GetDoubleValue(valueParts[0]);
                                    double value2 = GetDoubleValue(valueParts[1]);

                                    e.Row.Cells[i].Value = String.Format("{0}<br/>{1}", GetGaugeTable(value1, "от финансирования"), GetGaugeTable(value2, "от стоимости строительства"));
                                }
                                break;
                            }
                    }
                }
            }
        }

        private string GetGaugeTable(double value, string caption)
        {
            if (value != Double.MinValue)
            {
                return String.Format("<span style=\"font-size: 12px;color: white;\"><b>{0}</b><br/>{1:P2}&nbsp;{2}</span>", GetGaugeUrl(100 * value), value, caption);
            }
            return String.Empty;
        }

        protected string GetGaugeUrl(object oValue)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue);
            if (value > 100)
                value = 100;
            string path = "Prog_0001_0001_gauge" + Convert.ToInt32(value) + ".png";
            string returnPath = String.Format("<img style=\"FLOAT: center;\" src=\"../../../TemporaryImages/{0}\"/>", path);
            string serverPath = String.Format("~/TemporaryImages/{0}", path);

            if (File.Exists(Server.MapPath(serverPath)))
                return returnPath;
            LinearGaugeScale scale = ((LinearGauge)Gauge.Gauges[0]).Scales[0];
            scale.Markers[0].Value = value;
            MultiStopLinearGradientBrushElement BrushElement = (MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (value > 90)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else
            {
                if (value < 50)
                {

                    BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                    BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                    BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(199, 0, 0), 1);
                }
                else
                {
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 255, 128), 0);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 128, 0), 1);
                }
            }

            Size size = new Size(100, 40);
            Gauge.SaveTo(Server.MapPath(serverPath), GaugeImageType.Png, size);
            return returnPath;
        }

        private string GetStringValue(object strValue, string format)
        {
            double value = GetDoubleValue(strValue);
            if (value != Double.MinValue)
            {
                return value.ToString(format);
            }

            return String.Empty;
        }

        private Double GetDoubleValue(object strValue)
        {
            if (strValue != null)
            {

                decimal value;
                if (Decimal.TryParse(strValue.ToString(), out value))
                {
                    return Convert.ToDouble(value);
                }
            }

            return Double.MinValue;
        }

        #endregion

        #region Нарушения

        private void ViolationDataBind()
        {
            string query = DataProvider.GetQueryText("EO_0001_0003_violation");
            violationDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", violationDt);

            violationDictionary = new Dictionary<string, string>();

            foreach (DataRow row in violationDt.Rows)
            {
                string objectCode = GetStringDTValue(row, "Код объекта");
                string subjectBudget = GetBudgetCompete(row, "Бюджет субъкта", "Бюджет субъекта РФ");
                string moBudget = GetBudgetCompete(row, "бюджет МО", "Бюджет муниципального образования");
                string othersBudget = GetBudgetCompete(row, "прочие", "Прочие источники");

                if (!violationDictionary.ContainsKey(objectCode))
                {
                    violationDictionary.Add(objectCode, String.Format("{0}{1}{2}", subjectBudget, moBudget, othersBudget));
                }
            }
        }

        private string GetBudgetCompete(DataRow row, string budgetName, string fullBugdetName)
        {
            double masteredVolume = GetDoubleDTValue(row, "Освоено " + budgetName);
            double fundedVolume = GetDoubleDTValue(row, "Финансирование " + budgetName);
            double masteredPercent = GetDoubleDTValue(row, "Процент освоения " + budgetName);
            double buildYear = GetDoubleDTValue(row, "Начало строительства");
            double debtVolume = GetDoubleDTValue(row, "Дебиторская задолженность " + budgetName);
            string indicatorImage = String.Format("<img style=\"FLOAT: center;\" src=\"../../../Images/{0}.png\"/>", masteredPercent < 0.3 ? "ballRedBB" : "ballYellowBB");

            if (masteredVolume != Double.MinValue && fundedVolume != Double.MinValue && masteredPercent != Double.MinValue && buildYear != Double.MinValue && debtVolume != Double.MinValue && masteredPercent < 0.5)
            {
                return String.Format("<span style='font-size:14px;font-family: Arial;'>{0}&nbsp;{1}&nbsp;с начала строительства ({2}&nbsp;год):<br/>выделено&nbsp;{3}, освоено&nbsp;{4} ({5}), дебиторская задолженность&nbsp;{6}</span>", indicatorImage, GetTextSpan(fullBugdetName), GetDigitSpan(buildYear, "", ""),
                    GetDigitSpan(fundedVolume, "N2", "млн.руб."), GetDigitSpan(masteredVolume, "N2", "млн.руб."), GetDigitSpan(masteredPercent, "P2", ""), GetDigitSpan(debtVolume, "N2", "млн.руб."));
            }
            return String.Empty;
        }

        private string GetDigitSpan(double value, string format, string unit)
        {
            return String.Format("<span style='font-size:14px;font-family: Arial;color: white;'>{0}</span>{1}", value.ToString(format), unit == String.Empty ? unit : "&nbsp;" + unit);
        }

        private string GetTextSpan(string text)
        {
            return String.Format("<span style='font-size:14px;font-family: Arial;color: white;'>{0}</span>", text);
        }

        private static Double GetDoubleDTValue(DataRow row, string columnName)
        {
            return GetDoubleDTValue(row, columnName, Double.MinValue);
        }

        private static Double GetDoubleDTValue(DataRow row, string columnName, double defaultValue)
        {
            if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(row[columnName].ToString());
            }
            return defaultValue;
        }

        private static string GetStringDTValue(DataRow row, string columnName)
        {
            if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
            {
                return row[columnName].ToString();
            }
            return String.Empty;
        }

        #endregion
    }
}