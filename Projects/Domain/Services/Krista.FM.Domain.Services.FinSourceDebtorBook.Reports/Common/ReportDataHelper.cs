using System;
using System.Collections.Generic;
using System.Data;

namespace Krista.FM.Domain.Services.FinSourceDebtorBook.Reports
{
    /// <summary>
    /// Процедуры заполнения данных
    /// </summary>
    public partial class ReportsDataService
    {
        /// <summary>
        /// тип колонки отчета
        /// </summary>
        public enum ReportColumnType
        {
            /// <summary>
            /// строковые данные
            /// </summary>
            ctDataStr,

            /// <summary>
            /// числовые данные
            /// </summary>
            ctDataNum,

            /// <summary>
            /// колонка заполяемая по шаблону
            /// </summary>
            ctCalc,

            /// <summary>
            /// вычисляемая формула
            /// </summary>
            ctForm,

            /// <summary>
            /// счетчик строк
            /// </summary>
            ctCounter,

            /// <summary>
            /// сумма по данным детали
            /// </summary>
            ctDetailSum,

            /// <summary>
            /// строковые данные детали
            /// </summary>
            ctDetailStr,
        }

        private string FillDataValues(DataRow dr, KeyValuePair<string, ReportColumnType> fieldRecord)
        {
            var value = dr[fieldRecord.Key].ToString();
            if (fieldRecord.Value == ReportColumnType.ctDataNum)
            {
                var curFieldName = String.Format("{0}Cy", fieldRecord.Key);
                if (dr.Table.Columns.Contains("RefOkv") &&
                    dr["RefOkv"].ToString() != "-1" &&
                    dr.Table.Columns.Contains(curFieldName))
                {
                    const string TextKPTemplate = "; KP: {0}";
                    var textkpValue = String.Empty;

                    if (fieldRecord.Key == "Attract")
                    {
                        textkpValue = String.Format(TextKPTemplate, dr["DiffrncRatesDbt"]);
                    }

                    if (fieldRecord.Key == "PlanService")
                    {
                        textkpValue = String.Format(TextKPTemplate, dr["DiffrncRatesInterest"]);
                    }

                    value = String.Format(
                        "USD: {1}; RUB: {0}{2}", GetNumber(dr[fieldRecord.Key]), GetNumber(dr[curFieldName]), textkpValue);
                }
            }

            if (!value.Contains(","))
            {
                DateTime testDate;
                if (DateTime.TryParse(value, out testDate))
                {
                    value = testDate.ToShortDateString();
                }
            }

            return value;
        }

        private string CalcDataValues(DataRow dr, KeyValuePair<string, ReportColumnType> fieldRecord)
        {
            var result = String.Empty;

            if (fieldRecord.Key == "CreditInfo")
            {
                result = String.Format(
                    "{0} {1} {2}",
                    GetBookValue(scheme, DomainObjectsKeys.d_S_TypeContract, dr["RefTypeContract"].ToString()),
                    dr["Num"],
                    GetDateValue(dr["ContractDate"]));
            }

            if (fieldRecord.Key == "GarantInfo")
            {
                result = String.Format("{0} {1}", dr["PrincipalNum"], GetDateValue(dr["PrincipalStartDate"]));
            }

            if (fieldRecord.Key == "GarantNum")
            {
                result = String.Format("{0} {1}", dr["Num"], GetDateValue(dr["ChargeDate"]));
            }

            if (fieldRecord.Key == "CapitalType")
            {
                result = GetBookValue(scheme, DomainObjectsKeys.d_S_TypeContract, dr["RefSCap"].ToString());
            }

            if (fieldRecord.Key == "CapitalCurrencySum")
            {
                result = Convert.ToString(GetNumber(dr["CurrencySum"]) * GetNumber(dr["Nominal"]));
            }

            if (fieldRecord.Key == "CalcRegionName")
            {
                result = GetBookValue(scheme, DomainObjectsKeys.d_Regions_Analysis, dr["RefRegion"].ToString());
            }

            if (fieldRecord.Key.StartsWith("Zero"))
            {
                result = "0";
            }

            if (fieldRecord.Key.StartsWith("Empty"))
            {
                result = String.Empty;
            }

            if (fieldRecord.Key == "CreditNumContractDate")
            {
                result = FillContractInfo(dr, "ContractDate", "Num");
            }

            if (fieldRecord.Key == "CreditRenewalInfo")
            {
                result = FillContractInfo(dr, "RenewalDate", "RenewalNum");
            }

            if (fieldRecord.Key == "GarantNumStartDate")
            {
                result = FillContractInfo(dr, "StartDate", "Num");
            }

            if (fieldRecord.Key == "CreditRegRenewalInfo")
            {
                result = FillContractInfo(dr, "RenewalDate", "RegNum");
            }

            if (fieldRecord.Key == "CreditPercentN3")
            {
                result = GetPercentText(dr["CreditPercent"]);
            }

            if (fieldRecord.Key == "CreditActPercentN3")
            {
                if (!dr.IsNull("ActualCredPercent"))
                {
                    result = Convert.ToString(dr["ActualCredPercent"]);
                }
            }

            if (fieldRecord.Key == "CalcDetailRegionName")
            {
                const string RegionEntityKey = DomainObjectsKeys.d_Regions_Analysis;
                const string RgnField = "RefRegion";
                const string RefField = "ParentId";
                const string TerField = "RefTerr";
                var regionId = Convert.ToString(dr[RgnField]);
                var terrCode = Convert.ToInt32(GetBookValue(scheme, RegionEntityKey, regionId, TerField));
                var terrName = GetBookValue(scheme, RegionEntityKey, regionId);

                if (terrCode == 5 || terrCode == 6)
                {
                    var parentItem = GetBookValue(scheme, RegionEntityKey, regionId, RefField);
                    var parentRegion = GetBookValue(scheme, RegionEntityKey, parentItem, RefField);
                    var regionName = GetBookValue(scheme, RegionEntityKey, parentRegion);

                    result = String.Format("{0} ({1})", terrName, regionName);
                }
                else
                {
                    result = terrName;
                }
            }

            return result;
        }

        private string FillContractInfo(DataRow rowData, string dateFieldName, string numFieldName)
        {
            var formatStr = "{0} от {1}";

            if (rowData.IsNull(dateFieldName))
            {
                formatStr = "{0}";
            }

            return String.Format(formatStr, rowData[numFieldName], GetDateValue(rowData[dateFieldName]));
        }

        private string GetPercentText(object cellValue)
        {
            if (cellValue != DBNull.Value)
            {
                var percentValue = GetNumber(cellValue);
                if (percentValue > 0)
                {
                    return String.Format("{0:N3}%", percentValue);
                }
            }

            return String.Empty;
        }

        private string CalcCellValue(object sourceValue)
        {
            var value = sourceValue.ToString();

            DateTime testDate;
            if (DateTime.TryParse(value, out testDate))
            {
                value = GetDateValue(value);
            }

            return value;
        }

        private double ParseCurrencyStr(object strValue)
        {
            var result = GetNumber(strValue);
            var allParts = strValue.ToString().Split(';');

            if (allParts.Length > 1)
            {
                var rubParts = allParts[1].Split(' ');

                if (rubParts.Length > 1)
                {
                    result = GetNumber(rubParts[2]);
                }

                if (allParts.Length > 2)
                {
                    var difParts = allParts[2].Split(' ');
                    result += GetNumber(difParts[2]);
                }
            }

            return result;
        }

        private string CalcFormula(DataRow dr, KeyValuePair<string, ReportColumnType> fieldRecord, bool isCurrency)
        {
            var operands = fieldRecord.Key.Split(';');
            double sum = 0;

            foreach (var t in operands)
            {
                var sign = t.Substring(0, 1);
                var columnIndex = Convert.ToInt32(t.Remove(0, 1));
                var multiplier = 1;

                if (sign == "-")
                {
                    multiplier = -1;
                }

                var sumOperand = GetNumber(dr[columnIndex]);

                if (isCurrency)
                {
                    sumOperand = ParseCurrencyStr(dr[columnIndex]);
                }

                sum += multiplier * sumOperand;
            }

            return sum.ToString();
        }

        private object CalcDetailValue(DataTable tblData, DataRow dr, KeyValuePair<string, ReportColumnType> fieldRecord)
        {
            var result = String.Empty;

            // оставляем только детализации текущего договора
            var tblDetail = FilterDataSet(tblData, String.Format("ParentID = {0}", dr["ID"]));

            // сортируем по ключу - пока не сделал привязку к определенному полю даты
            tblDetail = SortDataSet(tblDetail, "ID");
            double sum = 0;
            var isSumColumn = fieldRecord.Value == ReportColumnType.ctDetailSum;

            for (int i = 0; i < tblDetail.Rows.Count; i++)
            {
                var cellValue = CalcCellValue(tblDetail.Rows[i][fieldRecord.Key]);

                if (isSumColumn)
                {
                    sum += Convert.ToDouble(cellValue);
                }
                else
                {
                    result = CombineStrings(result, cellValue);
                }
            }

            // Если нужна была сумма
            if (isSumColumn)
            {
                return sum;
            }

            // Было нужно строковые значение
            return result;
        }

        private DataTable FillTableData(DataTable tblData, Dictionary<string, ReportColumnType> fieldCollection)
        {
            var tblResult = CreateReportCaptionTable(fieldCollection.Count);
            var tblMaster = FilterDataSet(tblData, String.Format("ParentID is null"));
            var rowCounter = 1;
            var summary = new double[fieldCollection.Count];

            var columnCounter = 0;

            // настраиваем колонки
            foreach (var fieldRecord in fieldCollection)
            {
                if (fieldRecord.Value == ReportColumnType.ctDataStr && !tblResult.Columns.Contains(fieldRecord.Key))
                {
                    tblResult.Columns[columnCounter].ColumnName = fieldRecord.Key;
                }

                columnCounter++;
            }

            foreach (DataRow dr in tblMaster.Rows)
            {
                var isCurrency = false;
                if (dr.Table.Columns.Contains("RefOkv"))
                {
                    isCurrency = dr["RefOkv"].ToString() != "-1";
                }

                var rowResult = tblResult.Rows.Add();
                rowResult[0] = rowCounter++;
                columnCounter = 0;

                // сначала заполняем данные
                foreach (var fieldRecord in fieldCollection)
                {
                    if (fieldRecord.Value == ReportColumnType.ctDataStr || fieldRecord.Value == ReportColumnType.ctDataNum)
                    {
                        rowResult[columnCounter] = FillDataValues(dr, fieldRecord);
                    }

                    if (fieldRecord.Value == ReportColumnType.ctCalc)
                    {
                        rowResult[columnCounter] = CalcDataValues(dr, fieldRecord);
                    }

                    if (fieldRecord.Value == ReportColumnType.ctDetailStr || fieldRecord.Value == ReportColumnType.ctDetailSum)
                    {
                        rowResult[columnCounter] = CalcDetailValue(tblData, dr, fieldRecord);
                    }

                    columnCounter++;
                }

                // потом по данным считаем формулы
                columnCounter = 0;
                foreach (var fieldRecord in fieldCollection)
                {
                    if (fieldRecord.Value == ReportColumnType.ctForm)
                    {
                        rowResult[columnCounter] = CalcFormula(rowResult, fieldRecord, isCurrency);
                    }

                    columnCounter++;
                }

                for (var i = 1; i < fieldCollection.Count; i++)
                {
                    var addValue = GetNumber(rowResult[i]);
                    if (isCurrency)
                    {
                        addValue = ParseCurrencyStr(rowResult[i]);
                    }

                    summary[i] += addValue;
                }
            }

            var rowSummary = tblResult.Rows.Add();
            rowSummary[0] = "Итого";
            var columnIndex = 0;
            foreach (var fieldRecord in fieldCollection)
            {
                if (fieldRecord.Value == ReportColumnType.ctDataNum || fieldRecord.Value == ReportColumnType.ctForm
                    || fieldRecord.Value == ReportColumnType.ctDetailSum
                    || (fieldRecord.Value == ReportColumnType.ctCalc && fieldRecord.Key == "CapitalCurrencySum"))
                {
                    rowSummary[columnIndex] = summary[columnIndex];
                }

                columnIndex++;
            }

            return tblResult;
        }
    }
}
