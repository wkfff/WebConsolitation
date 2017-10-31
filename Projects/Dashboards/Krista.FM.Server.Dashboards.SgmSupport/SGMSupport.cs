using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.SGM
{
    public class SGMSupport
    {
        public string GetFOShortName(string fullName)
        {
            string upperName = fullName.ToUpper();

            if (upperName.Contains("ЦЕНТРАЛ")) return "ЦФО";
            if (upperName.Contains("ЮЖНЫЙ")) return "ЮФО";
            if (upperName.Contains("УРАЛЬС")) return "УФО";
            if (upperName.Contains("ДАЛЬНЕВ")) return "ДФО";
            if (upperName.Contains("СЕВЕРО-З")) return "СЗФО";
            if (upperName.Contains("СЕВЕРО-К")) return "СКФО";
            if (upperName.Contains("ПРИВОЛЖ")) return "ПФО";
            if (upperName.StartsWith("СИБИРС")) return "СФО";
            if (upperName.Contains("РОССИЙСК")) return "РФ";

            if (upperName.Contains("КАБАРДИНО-БАЛК. РЕСП")) return "КБР";
            if (upperName.Contains("ПРОХЛАДНЕНСКОЕ ТО")) return "Прохл.ТО";
            if (upperName.Contains("БАКСАНСКОЕ ТО")) return "Бакс.ТО";
            if (upperName.Contains("УРВАНСКОЕ ТО")) return "Урван.ТО";
            return fullName;
        }

        /// <summary>
        /// Заполнение параметра-списка вида отображения данных на карте
        /// </summary>
        public void FillPeopleGroupList(CustomMultiCombo ComboList)
        {
            var tempList = new Collection<string>();
            tempList.Clear();
            tempList.Add("Всего");
            tempList.Add("Взрослые");
            tempList.Add("Дети");
            tempList.Add("Подростки");
            ComboList.FillValues(tempList);
            ComboList.SetСheckedState(tempList[0], true);
        }

        /// <summary>
        /// Заполнение параметра-списка для формы 2
        /// </summary>
        public void FillPeopleGroupList2(CustomMultiCombo ComboList)
        {
            var tempList = new Collection<string>();
            tempList.Clear();
            tempList.Add("до 1 года");
            tempList.Add("1-2 года");
            tempList.Add("3-6 лет");
            tempList.Add("до 14 лет");
            tempList.Add("сельские жители до 14 лет");
            tempList.Add("15-17 лет");
            tempList.Add("до 17 лет");
            tempList.Add("сельские жители до 17 лет");
            ComboList.FillValues(tempList);
            ComboList.SetСheckedState(tempList[6], true);
        }

        public void FillMeasure(CustomMultiCombo ComboList)
        {
            var tempList = new Collection<string>();
            tempList.Clear();
            tempList.Add("Удельный вес");
            tempList.Add("Относительный показатель на 100 тыс. населения");
            tempList.Add("Абсолютный показатель");
            ComboList.FillValues(tempList);
            ComboList.SetСheckedState(tempList[0], true);
        }

        public void CalculateLinearParams(DataRow data, Collection<string> usedYears, Collection<string> allYears, int startIndex, ref double a, ref double b)
        {
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;
            double diffValue = double.MaxValue;
            double avgDiff = 0;
            int len = allYears.Count + startIndex;
            int yearCount = usedYears.Count;


            for (int i = startIndex; i < len; i++)
            {
                if (usedYears.IndexOf(allYears[i - startIndex]) != -1)
                {
                    if ((i > startIndex) && (i < len - 1))
                    {
                        if (CheckValue(data.ItemArray[i + 1]) && CheckValue(data.ItemArray[i + 0]))
                        {
                            avgDiff = avgDiff + Convert.ToDouble(data.ItemArray[i + 1])
                                - Convert.ToDouble(data.ItemArray[i]);
                        }
                        else
                        {
                            yearCount--;
                        }
                    }
                }
            }

            a = avgDiff / yearCount;

            for (int i = startIndex; i < len; i++)
            {
                if (usedYears.IndexOf(allYears[i - startIndex]) != -1)
                {
                    if (data.ItemArray[i].ToString() != string.Empty)
                    {
                        double value = Convert.ToDouble(data.ItemArray[i]) - a * Convert.ToDouble(allYears[i - startIndex]);
                        minValue = Math.Round(Math.Min(minValue, value), 4);
                        maxValue = Math.Round(Math.Max(maxValue, value), 4);
                    }
                }
            }

            if (minValue == maxValue && Math.Abs(minValue) > 0.0001)
            {
                b = minValue;
            }
            else
            {
                for (double i = minValue; i < maxValue; i += 0.05)
                {
                    double currentDiff = 0;
                    for (int k = startIndex; k < len; k++)
                    {
                        if (usedYears.IndexOf(allYears[k - startIndex]) != -1)
                        {
                            if (data[k].ToString() != string.Empty)
                            {
                                currentDiff += Math.Abs(Convert.ToDouble(data[k]) -
                                    Math.Abs((a * Convert.ToDouble(allYears[k - startIndex]) + i)));
                            }
                        }
                    }
                    if (diffValue > currentDiff)
                    {
                        b = i;
                        diffValue = currentDiff;
                    }
                }
            }
        }

        public string GetMonthLabel(CustomMultiCombo comboMonth, Collection<string> usedMonth)
        {
            string labelText = string.Empty;
            if (usedMonth.Count == 1) return usedMonth[0];
            if (usedMonth.Count == 0 || usedMonth.Count == 12) return string.Empty;

            bool isBreaked = false;
            int lastNum = 0;
            for (int i = 0; i < usedMonth.Count; i++)
            {
                if (i > 0) labelText = string.Format("{0}, ", labelText);
                labelText = string.Format("{0} {1}", labelText, usedMonth[i]);
                int curNum = CRHelper.MonthNum(usedMonth[i]);
                    isBreaked = isBreaked || (Math.Abs(lastNum - curNum) > 1);
                    lastNum = curNum;

            }
            if (isBreaked) return labelText;
            if (usedMonth.Count == 12) return string.Format(String.Empty);

            return String.Format("{0}-{1}", usedMonth[0], usedMonth[usedMonth.Count - 1]);
        }

        public DataRow GetTableRowValue(DataTable dt, string value, int columnIndex)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][columnIndex].ToString() == value)
                {
                    return dt.Rows[i];
                }
            }
            return null;
        }

        public DataRow GetLastRow(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[dt.Rows.Count - 1];
            }

            return null;
        }

        public string GetMonthShortName(string monthNum)
        {
            string result = string.Empty;
            switch (monthNum.Trim())
            {
                case "1":
                    {
                        result = "янв";
                        break;
                    }
                case "2":
                    {
                        result = "фев";
                        break;
                    }
                case "3":
                    {
                        result = "мар";
                        break;
                    }
                case "4":
                    {
                        result = "апр";
                        break;
                    }
                case "5":
                    {
                        result = "май";
                        break;
                    }
                case "6":
                    {
                        result = "июн";
                        break;
                    }
                case "7":
                    {
                        result = "июл";
                        break;
                    }
                case "8":
                    {
                        result = "авг";
                        break;
                    }
                case "9":
                    {
                        result = "сен";
                        break;
                    }
                case "10":
                    {
                        result = "окт";
                        break;
                    }
                case "11":
                    {
                        result = "ноя";
                        break;
                    }
                case "12":
                    {
                        result = "дек";
                        break;
                    }
            }

            return result;
        }

        public string GetMonthRusName(string monthNum, int outType)
        {
            string result = string.Empty;
            
            if (monthNum == string.Empty) return result;

            if (outType == 0)
            {
                result = CRHelper.RusMonth(Convert.ToInt32(monthNum));
            }
            else if (outType == 1)
            {
                if (monthNum == "1") result = "январе";
                if (monthNum == "2") result = "феврале";
                if (monthNum == "3") result = "марте";
                if (monthNum == "4") result = "апреле";
                if (monthNum == "5") result = "мае";
                if (monthNum == "6") result = "июне";
                if (monthNum == "7") result = "июле";
                if (monthNum == "8") result = "августе";
                if (monthNum == "9") result = "сентябре";
                if (monthNum == "10") result = "октябре";
                if (monthNum == "11") result = "ноябре";
                if (monthNum == "12") result = "декабре";
            }
            else if (outType == 2)
            {
                if (monthNum == "1") result = "январем";
                if (monthNum == "2") result = "февралем";
                if (monthNum == "3") result = "мартом";
                if (monthNum == "4") result = "апрелем";
                if (monthNum == "5") result = "маем";
                if (monthNum == "6") result = "июнем";
                if (monthNum == "7") result = "июлем";
                if (monthNum == "8") result = "августом";
                if (monthNum == "9") result = "сентябрем";
                if (monthNum == "10") result = "октябрем";
                if (monthNum == "11") result = "ноябрем";
                if (monthNum == "12") result = "декабрем";
            }
            else if (outType == 3)
            {
                if (monthNum == "1") result = "января";
                if (monthNum == "2") result = "февраля";
                if (monthNum == "3") result = "марта";
                if (monthNum == "4") result = "апреля";
                if (monthNum == "5") result = "мая";
                if (monthNum == "6") result = "июня";
                if (monthNum == "7") result = "июля";
                if (monthNum == "8") result = "августа";
                if (monthNum == "9") result = "сентября";
                if (monthNum == "10") result = "октября";
                if (monthNum == "11") result = "ноября";
                if (monthNum == "12") result = "декабря";
            }
            return result;
        }

        public string GetMonthLabelShort(string month)
        {
            string result = string.Empty;
            string[] monthNums = month.Split(',');
            if (monthNums.Length > 0) result = GetMonthShortName(monthNums[0]);
            if (monthNums.Length > 1)
            {
                result = string.Format("{0}-{1}", result, GetMonthShortName(monthNums[monthNums.Length - 1]));
            }

            return result;
        }

        public string GetMonthLabelFull(string month, int outType)
        {
            string result = string.Empty;
            string[] monthNums = month.Split(',');
            if (monthNums.Length > 0)
            {
                if (monthNums[0] == "0")
                {
                    return result;
                }

                result = GetMonthRusName(monthNums[0], outType);
            }
            if (monthNums.Length > 1)
            {
                result = String.Format("{0}-{1}", result, GetMonthRusName(monthNums[monthNums.Length - 1], outType));
            }

            return result;
        }

        public DataRow FindDataRow(DataTable dt, string pattern, string fieldName)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][fieldName].ToString() == pattern)
                {
                    return dt.Rows[i];
                }
            }
            return null;
        }

        public DataRow FindDataRowEx(DataTable dt, string pattern, string fieldName)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][fieldName].ToString().Trim().ToUpper() == pattern.Trim().ToUpper())
                {
                    return dt.Rows[i];
                }
            }

            return null;
        }

        public DataRow FindRowInSelection(DataRow[] dt, string pattern, string fieldName)
        {
            for (int i = 0; i < dt.Length; i++)
            {
                if (dt[i][fieldName].ToString() == pattern)
                {
                    return dt[i];
                }
            }
            return null;
        }

        public void SetColumnWidthAndCaption(UltraWebGrid grid, int index, string caption, int width, HorizontalAlign align, string tooltip)
        {
            grid.Columns[index].Width = width;
            grid.Columns[index].Header.Caption = caption;
            grid.Columns[index].Header.Style.Wrap = true;
            grid.Columns[index].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            grid.Columns[index].CellStyle.HorizontalAlign = align;
            grid.Columns[index].Header.Title = tooltip;
        }

        public void SetConditionalStats(DataTable dt, double population, int columnIndex)
        {
            if (population != 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][columnIndex] = 100000 * Convert.ToDouble(dt.Rows[i][columnIndex]) / population;
                }
            }
        }

        public string GetResultStringCountEx(double v1, bool absNumbers, bool useBeforeSyms)
        {
            string appendix = string.Empty;
            if (useBeforeSyms) appendix = "на ";

            if (Convert.ToInt32(v1) == 0) return "-";
            if (absNumbers) v1 = Math.Abs(v1);
            return string.Format("{1}{0} сл. ", v1, appendix);
        }

        public string GetResultStringTimesEx(double v1, bool absNumbers, bool useBeforeSyms)
        {
            string appendix = string.Empty;
            if (useBeforeSyms) appendix = "в ";
            if (absNumbers) v1 = Math.Abs(v1);
            if (v1 < 5 || v1 * 10 % 10 > 0) return string.Format("{1}{0:N1} раза ", v1, appendix);
            return string.Format("{1}{0:N1} раз ", v1, appendix);
        }

        public bool CheckDeseasePeriod(string deseaseCodes, int yr1, int yr2)
        {
            var diesPeriods = new Dictionary<int, Dictionary<int, int>>();
            var dies2011Year = new Dictionary<int, int> { { 2011, DateTime.MaxValue.Year } };
            diesPeriods.Add(052, dies2011Year);
            diesPeriods.Add(130, dies2011Year);
            diesPeriods.Add(131, dies2011Year);
            diesPeriods.Add(135, dies2011Year);

            string[] codes = deseaseCodes.Trim().Split(',');

            bool hasPeriod = false;
            foreach (string code in codes)
            {
                int diesCode = Convert.ToInt32(code);

                if (diesPeriods.ContainsKey(diesCode))
                {
                    foreach (KeyValuePair<int, int> period in diesPeriods[diesCode])
                    {
                        hasPeriod = hasPeriod || period.Key <= yr1 && period.Value >= yr1 && period.Key <= yr2 && period.Value >= yr2;
                    }
                }
                else
                {
                    hasPeriod = true;
                }
            }

            return hasPeriod;
        }

        public string GetDifferenceTextEx(double v1, double v2, double relCount1, double relCount2, bool absNumbers, bool useBeforeSyms)
        {
            return GetDifferenceTextEx(
                String.Empty, 
                DateTime.Now.Year,
                DateTime.Now.Year, 
                v1, 
                v2, 
                relCount1, 
                relCount2, 
                absNumbers, 
                useBeforeSyms);
        }

        public string GetDifferenceTextEx(string deseaseCode, int year1, int year2, 
            double v1, double v2, double relCount1, double relCount2, bool absNumbers, bool useBeforeSyms)
        {
            if (deseaseCode.Length > 0)
            {
                if (!CheckDeseasePeriod(deseaseCode, year1, year2))
                {
                    return "-";
                }
                // проверим что по заболеванию что-то было в один из годков
            }

            int difValueAbs = Convert.ToInt32(Math.Abs(v1 - v2));
            int difValue = Convert.ToInt32(v1 - v2);

            if (Convert.ToInt32(v1) == 0 || Convert.ToInt32(v2) == 0 || difValueAbs == 0)
                return GetResultStringCountEx(difValue, absNumbers, useBeforeSyms);

            if (difValueAbs > 0 && difValueAbs < 10)
            {
                return GetResultStringCountEx(difValue, absNumbers, useBeforeSyms);
            }
            double divValue = relCount1 / relCount2;

            if (divValue >= 1.5)
            {
                return GetResultStringTimesEx(divValue, absNumbers, useBeforeSyms);
            }

            if (divValue < 0.5)
            {
                return GetResultStringTimesEx(-relCount2 / relCount1, absNumbers, useBeforeSyms);
            }

            if (Math.Abs(divValue) < 0.00000000001)
            {
                return String.Format(" ");
            }

            string appendix = String.Empty;
            divValue = (divValue - 1) * 100;
            
            if (useBeforeSyms)
            {
                appendix = "на ";
            }
           
            if (absNumbers)
            {
                divValue = Math.Abs(divValue);
            }

            return String.Format("{1}{0:N1}%", divValue, appendix);
        }

        public int SetCellImageEx(UltraGridRow row, int index1, int index2, int indexResult)
        {
            return SetCellImageEx(row, null, index1, index2, indexResult);
        }

        public int SetCellImageEx(UltraGridRow row, DataTable dt, int index1, int index2, int indexResult)
        {
            int result = 0;
            double v1, v2;
            if (dt == null)
            {
                if (row.Cells[index1].Value == null) return result;
                if (row.Cells[index2].Value == null) return result;

                v1 = Convert.ToDouble(row.Cells[index1].Value);
                v2 = Convert.ToDouble(row.Cells[index2].Value);
            }
            else
            {
                v1 = 0; v2 = 0;
                DataRow dr = FindDataRowEx(dt, row.Cells[0].Value.ToString(), dt.Columns[0].ColumnName);
                if (dr != null)
                {
                    if (dr[index1].ToString() == string.Empty) return result;
                    if (dr[index2].ToString() == string.Empty) return result;

                    v1 = Convert.ToDouble(dr[index1]);
                    v2 = Convert.ToDouble(dr[index2]);
                }
            }

            if (row.Cells[indexResult].Value != null)
            {
                if (Convert.ToString(row.Cells[indexResult].Value) == "-")
                {
                    return result;
                }
            }

            if (v1 < v2)
            {
                result = 1;
                row.Cells[indexResult].Title = "Рост заболеваемости";
                row.Cells[indexResult].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
            }
            else
            {
                if (Math.Abs(v1 - v2) > 0.00000001)
                {
                    result = 2;
                    row.Cells[indexResult].Title = "Снижение заболеваемости";
                    row.Cells[indexResult].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                }
            }
            row.Cells[indexResult].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            return result;
        }

        public int SetCellImageStar(UltraGridRow row, double[] maxValues, double[] minValues, 
            int maxIndex, int cellIndex, int resultValue)
        {
            return SetCellImageStar(row, maxValues, minValues, maxIndex, cellIndex, resultValue, false);
        }

        public int SetCellImageStar(
            UltraGridRow row, 
            double[] maxValues,
            double[] minValues,
            int maxIndex, 
            int cellIndex, 
            int resultValue, 
            bool needInvert)
        {
            int result = 0;
            double cellValue = Convert.ToDouble(row.Cells[cellIndex].Value);
            UltraGridCell cellResult = row.Cells[resultValue];

            const string image1 = "~/images/starGrayBB.png";
            const string image2 = "~/images/starYellowBB.png";
            const string caption1 = "Максимальный уровень заболеваемости на 100 тыс. населения";
            const string caption2 = "Минимальный уровень заболеваемости на 100 тыс. населения";

            string captionMin = caption2;
            string captionMax = caption1;
            string imageMin = image2;
            string imageMax = image1;

            if (needInvert)
            {
                captionMin = caption1;
                captionMax = caption2;
                imageMin = image1;
                imageMax = image2;
            }

            if ((maxValues[maxIndex] <= cellValue) && (maxValues[maxIndex] > 0))
            {
                result = 1;
                cellResult.Style.BackgroundImage = imageMax;
                cellResult.Title = captionMax;
            }
            if ((minValues[maxIndex] >= cellValue) && (minValues[maxIndex] > 0))
            {
                if (cellResult.Value != null)
                {
                    result = 2;
                    cellResult.Style.BackgroundImage = imageMin;
                    cellResult.Title = captionMin;
                }
            }
            cellResult.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            return result;
        }

        public int SetInjectionsCellImage(UltraGridRow row, int index1, int index2, int indexResult)
        {
            int result = 0;

            if (row.Cells[index1].Value == null) return result;
            if (row.Cells[index2].Value == null) return result;

            var cellResult = row.Cells[indexResult];

            double v1 = Convert.ToDouble(row.Cells[index1].Value);
            double v2 = Convert.ToDouble(row.Cells[index2].Value);

            if (v1 < v2)
            {
                result = 1;
                cellResult.Title = string.Format("Рост показателя охвата вакцинацией на {0:N2}%", Convert.ToDouble(row.Cells[index2].Value) - Convert.ToDouble(row.Cells[index1].Value));
                cellResult.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
            }
            else
            {
                if (v1 != v2)
                {
                    result = 2;
                    cellResult.Title = string.Format("Снижение показателя охвата вакцинацией на {0:N2}%", Convert.ToDouble(row.Cells[index1].Value) - Convert.ToDouble(row.Cells[index2].Value));
                    cellResult.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                }
            }

            cellResult.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
            return result;
        }

        public void CalculateGridColumnsWidth(UltraWebGrid grid, int startIndex)
        {
            double totalWidth = grid.Width.Value;
            double fixedWidth = 0;

            for (int i = 0; i < startIndex; i++)
            {
                fixedWidth += grid.Columns[i].Width.Value;
            }

            double usableWidth = totalWidth - fixedWidth - 70 - (grid.Columns.Count - startIndex) * 5;
            double possibleColumnWidth = Math.Round(usableWidth / (grid.Columns.Count - startIndex));

            for (int i = startIndex; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = (Unit)(possibleColumnWidth);
            }
        }

        public string GetNewParagraphSyms(bool needBr)
        {
            string appendix = needBr ? "<br>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" : "&nbsp";
            return appendix;
        }

        public string GetNewParagraphSyms(bool needBr, bool needOffset)
        {
            string appendix = string.Empty;
            if (needBr)
            {
                appendix = "<br>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp";
            }
            if (needOffset)
            {
                appendix = string.Format("{0}{1}", appendix, "&nbsp&nbsp&nbsp&nbsp&nbsp");
            }
            appendix = string.Format("{0}{1}", appendix, "&nbsp");
            return string.Format("{0}", appendix);
        }

        public bool FindInStr(string str, string substr)
        {
            int pos = str.IndexOf(substr, StringComparison.CurrentCultureIgnoreCase);
            if (pos >= 0)
            {
                if ((pos == 0) || (str[pos - 1] == ' ') || (str[pos - 1] == '.'))
                    return true;
            }

            return false;
        }

        public bool CheckValue(object valueObject)
        {
            if (valueObject == null || valueObject.ToString() == string.Empty) return false;
            return true;
        }

        public string ConvertEncode1(string value)
        {
            return ConvertEncoding(value, Encoding.GetEncoding(866), Encoding.GetEncoding(1251));
        }

        public string ConvertEncode2(string value)
        {
            return value;
        }

        private string ConvertEncoding(string value, Encoding src, Encoding trg)
        {
            Decoder dec = src.GetDecoder();
            byte[] ba = trg.GetBytes(value);
            int len = dec.GetCharCount(ba, 0, ba.Length);
            var ca = new char[len];
            dec.GetChars(ba, 0, ba.Length, ca, 0);
            return new string(ca);
        }

        public string GetRootMapName(DataTable dtAreaShort)
        {
            DataRow[] drsSelect = dtAreaShort.Select("cod = 999");
            if (drsSelect.Length > 0)
            {
                return GetFOShortName(drsSelect[0]["Name"].ToString());
            }
            return string.Empty;
        }
    }
}
