using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.Month.Fillers
{
    public class CommonMonthReportFiller
    {
        protected  virtual void FillIndexRow(HSSFSheet sheet, int rowIndex, int startColumn, int startValue)
        {
            var row = sheet.GetRow(rowIndex);

            if (row == null)
            {
                return;
            }

            var counter = startValue;

            for (var i = startColumn; i < row.LastCellNum; i++)
            {
                NPOIHelper.SetCellValue(sheet, rowIndex, i, counter++);
            }
        }

        protected virtual void FillCaptionParams(
            HSSFWorkbook wb,
            HSSFSheet sheet,
            DataTable tblCaptions,
            Dictionary<string, int> paramList)
        {
            if (tblCaptions.Rows.Count <= 0)
            {
                return;
            }

            var rowCaptions = tblCaptions.Rows[0];

            for (var i = 0; i < sheet.LastRowNum; i++)
            {
                for (var j = 0; j < 256; j++)
                {
                    foreach (var param in paramList)
                    {
                        var cellValue = NPOIHelper.GetCellStringValue(sheet, i, j);

                        if (!cellValue.Contains(param.Key))
                        {
                            continue;
                        }

                        var newVal = cellValue.Replace(param.Key, Convert.ToString(rowCaptions[param.Value]));
                        NPOIHelper.SetCellValue(sheet, i, j, newVal);
                    }
                }
            }

            var sheetIndex = wb.GetSheetIndex(sheet);
            var oldSheetName = wb.GetSheetName(sheetIndex);
            var newSheetName = oldSheetName;

            foreach (var param in paramList)
            {
                var paramValue = Convert.ToString(rowCaptions[param.Value]);
                newSheetName = newSheetName.Replace(param.Key, paramValue);
            }

            wb.SetSheetName(sheetIndex, newSheetName);
        }

        protected virtual int[] GetHeaderColumns()
        {
            return new[] { 0, 1, 2 };
        }

        protected virtual Dictionary<int, List<int>> GetHeaderPatterns()
        {
            var patterns = new Dictionary<int, List<int>>
                               {
                                   {0, new List<int>() {0}},
                                   {1, new List<int>() {1}},
                                   {2, new List<int>() {2}}
                               };

            return patterns;
        }

        protected virtual void CreateRegionHeaders(
            HSSFWorkbook wb,
            HSSFSheet sheet,
            DataTable tblData,
            int startRow,
            int columnCount,
            bool showSettles)
        {
            var rowIndex = startRow;
            var rowNum = 1;

            var rangeCol = GetHeaderColumns();
            var rangeAll = new int[columnCount];

            for (var i = 0; i < columnCount; i++)
            {
                rangeAll[i] = i;
            }

            var styleList = new List<KeyValuePair<int, int>>();
            var styleRowN = new List<int>();
            var styleRows = new List<Collection<int>>();

            foreach (DataRow row in tblData.Rows)
            {
                var lvl = Convert.ToInt32(row[ReportMonthMethods.RegionLvlIndex]);
                var ter = Convert.ToInt32(row[ReportMonthMethods.RegionTypIndex]);

                if (lvl == 3 && ter == 4)
                {
                    ter = 4;
                }
                else
                {
                    ter = -1;
                }
               
                var alreadyDefined = false;

                var itemNum = 0;
                foreach (var styleInfo in styleList)
                {
                    var isEqual = styleInfo.Key == lvl && styleInfo.Value == ter;

                    if (!alreadyDefined && isEqual)
                    {
                        styleRows[itemNum].Add(rowIndex);
                    }

                    alreadyDefined = alreadyDefined || isEqual;
                    itemNum++;
                }

                if (!alreadyDefined)
                {
                    styleList.Add(new KeyValuePair<int, int>(lvl, ter));
                    styleRowN.Add(rowIndex);
                    styleRows.Add(new Collection<int>());
                }

                if (rowNum != tblData.Rows.Count)
                {
                    if (lvl == 3)
                    {
                        NPOIHelper.SetCellValue(sheet, rowIndex, 0, row[0]);
                    }

                    for (var h = 1; h < rangeCol.Length; h++)
                    {
                        var patternFields = GetHeaderPatterns()[h];
                        var patternValues = new string[patternFields.Count];
                        var patternIndex = 0;

                        foreach (var patternField in patternFields)
                        {
                            patternValues[patternIndex++] = Convert.ToString(row[patternField]);
                        }

                        NPOIHelper.SetCellValue(sheet, rowIndex, h, String.Join(" ", patternValues).Trim());
                    }
                }

                rowIndex++;
                rowNum++;
            }

            rowNum = 0;
            foreach (var styleRow in styleList)
            {
                rowIndex = styleRowN[rowNum];
                var oldRow = sheet.GetRow(rowIndex);

                for (var i = 0; i < columnCount; i++)
                {
                    var oldCell = oldRow.GetCell(i);
                    var newCellStyle = wb.CreateCellStyle();
                    newCellStyle.CloneStyleFrom(oldCell.CellStyle);
                    oldCell.CellStyle = newCellStyle;
                }
                
                rowNum++;
            }

            rowNum = 0;

            var rangeParams = new NPOIHelper.RangeParams
                                  {
                                      wb = wb, 
                                      sheet = sheet,
                                      colsList = rangeAll
                                  };

            foreach (var styleRow in styleList)
            {
                var lvl = styleRow.Key;
                var ter = styleRow.Value;
                rangeParams.rowsList = new[] { styleRowN[rowNum] };

                rangeParams.colsList = rangeAll;
                rangeParams.action = NPOIHelper.FormatAction.FontNormal;
                NPOIHelper.SetRangeAction(rangeParams);
                rangeParams.action = NPOIHelper.FormatAction.FontNonItalic;
                NPOIHelper.SetRangeAction(rangeParams);

                if (lvl == 2)
                {
                    rangeParams.colsList = rangeCol;
                    rangeParams.action = NPOIHelper.FormatAction.FontBold;
                    NPOIHelper.SetRangeAction(rangeParams);                    
                }

                if (lvl < 5)
                {
                    rangeParams.colsList = rangeCol;
                    rangeParams.action = NPOIHelper.FormatAction.FontItalic;
                    NPOIHelper.SetRangeAction(rangeParams);
                }

                if (lvl == 3 && ter == 4)
                {
                    rangeParams.colsList = rangeAll;

                    if (showSettles)
                    {
                        rangeParams.action = NPOIHelper.FormatAction.BlueBG;
                        NPOIHelper.SetRangeAction(rangeParams);  
                    }

                    rangeParams.action = NPOIHelper.FormatAction.FontBold;
                    NPOIHelper.SetRangeAction(rangeParams);
                    rangeParams.action = NPOIHelper.FormatAction.FontItalic;
                    NPOIHelper.SetRangeAction(rangeParams); 
                }

                foreach (var cloneRow in styleRows[rowNum])
                {
                    rowIndex = styleRowN[rowNum];
                    var oldRow = sheet.GetRow(rowIndex);
                    var newRow = sheet.GetRow(cloneRow);

                    for (var i = 0; i < columnCount; i++)
                    {
                        var oldCell = oldRow.GetCell(i);
                        var newCell = newRow.GetCell(i);
                        newCell.CellStyle = oldCell.CellStyle;
                    }                
                }

                rowNum++;
            }
        }

        protected virtual Dictionary<string, int> GetParamDictionary()
        {
            return new Dictionary<string, int> 
            {
                {"YEAR", 0}, 
                {"KD", 1}, 
                {"VARIANT", 2}, 
                {"MEASURE", 4}, 
                {"MONTH", 5}
            };
        }

        protected virtual void SetRangeDataFormat(
            HSSFWorkbook wb,
            HSSFSheet sheet,
            string formatStr,
            IEnumerable<int> rowsList,
            IEnumerable<int> colsList)
        {
            if (rowsList == null || colsList == null)
            {
                return;
            }

            var format = HSSFDataFormat.GetBuiltinFormat(formatStr);
            if (format < 0)
            {
                format = wb.CreateDataFormat().GetFormat(formatStr);
            }

            foreach (var i in rowsList)
            {
                foreach (var j in colsList)
                {
                    var cell = NPOIHelper.GetCellByXY(sheet, i, j);

                    if (cell == null)
                    {
                        continue;
                    }

                   cell.CellStyle = NPOIHelper.GetCellStyle(wb, cell.CellStyle, CellStyleProperty.DataFormat, format);
                }
            }
        }

        protected virtual string GetPrecisionFormat(object precision)
        {
            switch (Convert.ToString(precision))
            {
                case "0":
                    return "#,##0";
                case "1":
                    return "#,##0.0";
                default:
                    return "#,##0.00";
            }
        }

        public virtual void FillMonthReport(HSSFWorkbook wb, DataTable[] tableList)
        {
        }
    }
}
