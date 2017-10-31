using System;
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
    public partial class RG_0003_0001 : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            GridCaption.Text = "Информация по основным социально-экономическим показателям региона";
            GridBrick.BrowserSizeAdapting = false;
            GridBrick.Height = Unit.Empty;
            GridBrick.Width = Unit.Empty;
            GridBrick.RedNegativeColoring = false;
            GridBrick.HeaderVisible = false;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(CommonGrid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(CommonGrid_InitializeRow);

            CommonGridDataBind();
        }

        #region Обработчики главного грида

        private void CommonGridDataBind()
        {
            string query = DataProvider.GetQueryText("RG_0003_0001_grid");
            dt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            if (dt.Rows.Count > 0)
            {
                if (dt.Columns.Count > 0)
                {
                    dt.Columns.RemoveAt(0);
                }
                
                GridBrick.DataTable = dt;
            }
        }

        private void CommonGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.White;
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 13;

            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 11;

            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[2].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 11;

            e.Layout.Bands[0].Columns[0].Width = 360;
            e.Layout.Bands[0].Columns[1].Width = 150;
            e.Layout.Bands[0].Columns[2].Width = 250;

            e.Layout.Bands[0].Columns[3].Hidden = true;
            e.Layout.Bands[0].Columns[4].Hidden = true;
            e.Layout.Bands[0].Columns[5].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;
            e.Layout.Bands[0].Columns[7].Hidden = true;
        }

        protected void CommonGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int rowType = Convert.ToInt32(GetDoubleGridValue(e.Row, "Тип строки", -1));
            if (rowType == 0)
            {
                e.Row.Style.BorderDetails.WidthTop = 3;
                e.Row.Cells[0].ColSpan = 3;
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
                e.Row.Cells[0].Style.VerticalAlign = VerticalAlign.Middle;
                e.Row.Cells[0].Style.Font.Size = 16;
                string name = e.Row.Cells[0].Value.ToString();
                name = name.Replace("Доходы и расходы населения", "Доходы и расходы");
                name = name.Replace("Рыболовство и рыбоводство", "Рыболовство");
                name = name.Replace("Финансовые результаты", "Финрезультаты");
                name = name.Replace("Потребительские цены", "ПотребЦены");

                e.Row.Cells[0].Value = String.Format("{0}&nbsp;<img height=\"40px\" style=\"margin-bottom:-10px;\" src=\"../../../Images/SEPImages/{0}.png\"/>", name);
            }
            else
            {
                DateTime currDate = CRHelper.PeriodDayFoDate(GetStringGridValue(e.Row, "Последний период "));
                DateTime prevDate = CRHelper.PeriodDayFoDate(GetStringGridValue(e.Row, "Предыдущий период "));
                string indicatorType = GetStringGridValue(e.Row, "Тип показателя");
                bool isDirectIndication = GetStringGridValue(e.Row, "Направление показателя") == "прямой показатель";

                DateTime nextDate = currDate;
                switch (indicatorType)
                {
                    case "Ежеквартальный":
                        {
                            nextDate = nextDate.AddMonths(3);
                            break;
                        }
                    case "Ежемесячный":
                        {
                            nextDate = nextDate.AddMonths(1);
                            break;
                        }
                }

                e.Row.Cells[0].Value = String.Format("{0}<br/><div style='font-size:14px;color:lightgray; text-align:right;width:100%'>{2} на {1:dd.MM.yyyy}</div>", e.Row.Cells[0].Value, nextDate, indicatorType);

                string[] valueParts = e.Row.Cells[1].Value.ToString().Split(';');
                if (valueParts.Length > 1)
                {
                    bool percentValue = valueParts[1] == "%";
                    string value1 = percentValue ? GetDoubleValue(valueParts[0]).ToString("N2") + "%" : GetDoubleValue(valueParts[0]).ToString("N2");
                    string value2 = percentValue ? String.Empty : "<br/>" + valueParts[1];

                    e.Row.Cells[1].Value = String.Format("<span style='font-size:22px;color:white'>{0}</span>{1}", value1, value2);
                }

                switch (rowType)
                {
                    case 1:
                        {
                            double value = GetDoubleValue(e.Row.Cells[2].Value);
                            string deviationStr = value > 100 ? "Рост показателя" : "Снижение показателя";
                            string deviationImg = (isDirectIndication && value > 100) || (!isDirectIndication && value < 100) ? "greenRect.gif" : "redrect.gif";
                            e.Row.Cells[2].Value = String.Format("<img style=\"FLOAT: center;\" src=\"../../../Images/{0}\"/><br/>{1}", deviationImg, deviationStr);
                            break;
                        }
                    case 2:
                        {
                            if (e.Row.Cells[2].Value != null)
                            {
                                valueParts = e.Row.Cells[2].Value.ToString().Split(';');
                                if (valueParts.Length > 1)
                                {
                                    double value1 = GetDoubleValue(valueParts[0]);
                                    double value2 = GetDoubleValue(valueParts[1]);

                                    string deviationStr = value1 > 1 ? "Рост" : "Снижение";
                                    string arrowColor = (isDirectIndication && value1 > 1) || (!isDirectIndication && value1 < 1)
                                                            ? "green"
                                                            : "red";
                                    string deviationImgName = String.Format("Arrow{0}{1}.png", value1 > 1 ? "Up" : "Down", arrowColor);
                                    string deviationImageTag = String.Format("<img style=\"margin-bottom:-5px;\" src=\"../../../Images/{0}\"/>", deviationImgName);

                                    switch (indicatorType)
                                    {
                                        case "Ежемесячный":
                                            {
                                                e.Row.Cells[2].Value = String.Format("{0}&nbsp;{1}<br/>{2} к {3} {4} года<br/>Прирост за {5} {6} года составил {7}",
                                                    deviationImageTag, GetDigitSpan(value1, "P2", "", 18), deviationStr, CRHelper.RusMonthDat(currDate.Month), currDate.Year - 1, CRHelper.RusMonth(currDate.Month), currDate.Year, GetDigitSpan(value2, "N2", "млн.руб.", 16));
                                                break;
                                            }
                                        case "Ежеквартальный":
                                            {
                                                e.Row.Cells[2].Value = String.Format("{0}&nbsp;{1}<br/>{2} к {3} кварталу {4} года<br/>Прирост за {3} квартал {5} года составил {6}",
                                                    deviationImageTag, GetDigitSpan(value1, "P2", "", 18), deviationStr, CRHelper.QuarterNumByMonthNum(currDate.Month), currDate.Year - 1, currDate.Year, GetDigitSpan(value2, "N2", "млн.руб.", 16));
                                                break;
                                            }
                                        case "Ежегодный":
                                            {
                                                e.Row.Cells[2].Value = String.Format("{0}&nbsp;{1:P2}<br/>{2} к {3} году", deviationImageTag, value1, deviationStr, currDate.Year - 1);
                                                break;
                                            }
                                    }
                                }
                            }

                            break;
                        }
                }
            }
        }

        private string GetDigitSpan(double value, string format, string unit, int fontHeight)
        {
            return String.Format("<span style='font-size:{2}px;font-family: Arial;color: white; font-weight:bold'>{0}</span>{1}", value.ToString(format), unit == String.Empty ? unit : "&nbsp;" + unit, fontHeight);
        }

        private string GetTextSpan(string text, int fontHeight)
        {
            return String.Format("<span style='font-size:{1}px;font-family: Arial;color: white;'>{0}</span>", text, fontHeight);
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

        private static Double GetDoubleGridValue(UltraGridRow row, string columnName)
        {
            return GetDoubleGridValue(row, columnName, 0);
        }

        private static Double GetDoubleGridValue(UltraGridRow row, string columnName, double defaultValue)
        {
            foreach (UltraGridCell cell in row.Cells)
            {
                if (cell.Column.Header.Caption == columnName)
                {
                    if (cell.Value != null && cell.Value.ToString() != String.Empty)
                    {
                        return Convert.ToDouble(cell.Value);
                    }
                }
            }
            
            return defaultValue;
        }

        private static string GetStringGridValue(UltraGridRow row, string columnName)
        {
            foreach (UltraGridCell cell in row.Cells)
            {
                if (cell.Column.Header.Caption == columnName)
                {
                    if (cell.Value != null)
                    {
                        return cell.Value.ToString();
                    }
                }
            }

            return String.Empty;
        }

        #endregion
    }
}