using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert.Common
{
    public static class MapHelper
    {
        private static Random random = new Random();

        public static double minRangeValue = -9e23; 
        public static double maxRangeValue = 9e23;

        public static string CorrectFieldName(string fieldName)
        {
            return fieldName.Replace(' ', '_');
        }

        public static SymbolRule GetSymbolRule(List<SymbolRule> rules, string category, string value)
        {
            foreach (SymbolRule rule in rules)
            {
                if ((rule.Category == category) && (rule.SymbolField == CorrectFieldName(value)))
                {
                    return rule;
                }
            }
            return null;
        }

        public static SymbolRule NewSymbolRule(string category, string valueName)
        {
            SymbolRule symRule = new SymbolRule();

            AddPredefinedSymbols(symRule, valueName);
            symRule.Category = category;
            symRule.DataGrouping = DataGrouping.EqualInterval;
            symRule.SymbolField = CorrectFieldName(valueName);
            symRule.ShowInLegend = category + " " + valueName;

            return symRule;
        }

        public static Color GetRandomColor()
        {
            return Color.FromArgb(random.Next(256),
                             random.Next(256),
                             random.Next(256));
        }

        public static void AddPredefinedSymbols(SymbolRule symRule, string valueName)
        {
            Color newColor = GetRandomColor();

            for (int i = 0; i < 5; i++)
            {
                PredefinedSymbol symbol = new PredefinedSymbol();
                symRule.PredefinedSymbols.Add(symbol);
                {
                    symbol.Color = newColor;
                    symbol.Text = "#" + CorrectFieldName(valueName).ToUpper();
                    symbol.ToolTip = "#" + CorrectFieldName(valueName).ToUpper();
                }
            }
            SetSizePredefinedSymbols(symRule);
        }

        public static void SetSizePredefinedSymbols(SymbolRule symRule)
        {
            double interval = (SymbolSize.SymbolMaxSize - SymbolSize.SymbolMinSize) / symRule.PredefinedSymbols.Count;
            int i = 0;
            foreach (PredefinedSymbol symbol in symRule.PredefinedSymbols)
            {
                symbol.Height = SymbolSize.SymbolMinSize + i * (int)interval;
                symbol.Width = SymbolSize.SymbolMinSize + i * (int)interval;
                i++;
            }
        }

        //получение строки таблицы с данными конкретного объекта
        public static DataRow GetTableRow(DataTable dt, string objectName)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[Consts.objectsColumn] == DBNull.Value)
                    continue;

                if ((string)row[Consts.objectsColumn] == objectName)
                {
                    return row;
                }
            }
            return null;
        }

        //получение строки таблицы с данными конкретного объекта
        public static DataRow GetTableRowByObjCode(DataTable dt, string objectCode)
        {
            foreach (DataRow row in dt.Rows)
            {
                if ((row[Consts.objCodeColumn] != DBNull.Value) && 
                    ((string)row[Consts.objCodeColumn] == objectCode))
                {
                    return row;
                }
            }
            return null;
        }


        //получение строки таблицы с данными конкретного объекта
        public static DataRow GetTableRow(DataTable dt, string objectCode, string objectName)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (((row[Consts.objCodeColumn] == DBNull.Value) || 
                    ((string)row[Consts.objCodeColumn] == objectCode)) && 
                    ((string)row[Consts.objectsColumn] == objectName))
                {
                    return row;
                }
            }
            return null;
        }

        public static double GetMinimumRangeLimit(DataTable table, string valueName)
        {
            double minValue = maxRangeValue;
            foreach (DataRow row in table.Rows)
            {
                if (row[valueName] != DBNull.Value)
                    minValue = Math.Min(minValue, (double)row[valueName]);
            }
            return minValue;

        }

        public static double GetMaximumRangeLimit(DataTable table, string valueName)
        {
            double maxValue = minRangeValue;

            foreach (DataRow row in table.Rows)
            {
                if (row[valueName] != DBNull.Value)
                    maxValue = Math.Max(maxValue, (double)row[valueName]);
            }
            return maxValue;
        }

        public static double GetMinimumRangeLimit(DataSet ds, List<string> excludedMeasures, string serieName, string serieRuleName)
        {
            double minValue = maxRangeValue;
            if (ds == null)
            {
                return minValue;
            }

            foreach (DataTable table in ds.Tables)
            {
                for (int i = 3; i < table.Columns.Count; i++)
                {
                    if (excludedMeasures.Contains(table.Columns[i].ColumnName))
                        continue;

                    if ((serieName == table.TableName) &&
                       (serieRuleName == table.Columns[i].ColumnName))
                    {    
                        continue;
                    }

                    double minRL = GetMinimumRangeLimit(table, table.Columns[i].ColumnName);

                    if (minValue > minRL)
                    {
                        minValue = minRL;
                    }
                }
            }
            return minValue;
        }

        public static double GetMaximumRangeLimit(DataSet ds, List<string> excludedMeasures, string serieName, string serieRuleName)
        {
            double maxValue = minRangeValue;
            if (ds == null)
            {
                return maxValue;
            }
            foreach (DataTable table in ds.Tables)
            {
                for (int i = 3; i < table.Columns.Count; i++)
                {
                    if (excludedMeasures.Contains(table.Columns[i].ColumnName))
                        continue;

                    if((serieName == table.TableName)&&(serieRuleName == table.Columns[i].ColumnName))
                    {
                        continue;
                    }

                    double maxRL = GetMaximumRangeLimit(table, table.Columns[i].ColumnName);

                    if (maxValue < maxRL)
                    {
                        maxValue = maxRL;
                    }
                }
            }
            return maxValue;
        }


        public static List<double> GetRangeLimits(int limitCount, DataTable table, string valueName, int digitCount)
        {
            List<double> rangeLimits = new List<double>();

            if (limitCount == 0)
            {
                return rangeLimits;
            }
            double max = GetMaximumRangeLimit(table, valueName);
            double min = GetMinimumRangeLimit(table, valueName);
            if (min > max)
            {
                min = 0;
                max = 100;
            }

            double rangeLength = (max - min) / limitCount;
            rangeLimits.Add(min);

            for (int i = 1; i < limitCount; i++)
            {
                rangeLimits.Add(CommonUtils.SetNumberPrecision(rangeLimits[i - 1] + rangeLength, digitCount));
            }
            if (limitCount > 1)
            {
                rangeLimits.Add(max);
            }

            return rangeLimits;
        }


        public static List<double> GetRangeLimits(int limitCount, double minValue, double maxValue, int digitCount)
        {
            List<double> rangeLimits = new List<double>();

            if (limitCount == 0)
            {
                return rangeLimits;
            }

            if (minValue > maxValue)
            {
                minValue = 0;
                maxValue = 100;
            }

            double rangeLength = (maxValue - minValue) / limitCount;
            rangeLimits.Add(minValue);

            for (int i = 1; i < limitCount; i++)
            {
                rangeLimits.Add(CommonUtils.SetNumberPrecision(rangeLimits[i - 1] + rangeLength, digitCount));
            }
            if (limitCount > 1)
            {
                rangeLimits.Add(maxValue);
            }

            return rangeLimits;
        }


    }
    public static class SymbolSize
    {
        private static int symbolMaxSize = 50;
        private static int symbolMinSize = 2;

        public static int SymbolMaxSize
        {
            get { return symbolMaxSize; }
            set { symbolMaxSize = value; }
        }

        public static int SymbolMinSize
        {
            get { return symbolMinSize; }
            set { symbolMinSize = value; }
        }
    }

}
