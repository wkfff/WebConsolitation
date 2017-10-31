using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Data;
using Dundas.Maps.WinControl;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    public class MapSerie
    {
        private MapSerieCollection series;
        //список правил для каждого значения серии
        private List<SerieRule> serieRules;
        //private DataTable table = null;
        private string name;

        //значки-болванки для секторных диаграмм
        private List<MapPieChartSymbol> pieChartSymbols = new List<MapPieChartSymbol>();

        private List<Symbol> oldPieChartSymbols = new List<Symbol>();
        //цвета секторов диаграмм
        private List<Color> pieChartColors = new List<Color>();

        private bool showCharts;
        private int pieChartOffset;
        private int pieChartSize = 30;
        private int minPieChartSize = 20;
        private int maxPieChartSize = 50;

        private PieChartBase pieChartBaseType;
        private double maxChartBaseSum;
        private double minChartBaseSum;
        private double maxChartBaseMaximum;
        private double minChartBaseMaximum;
        private double customBase = 1000;

        private bool isBindOffsetWithSize = true;

        public List<SerieRule> SerieRules
        {
            get { return this.serieRules; }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public DataTable Table
        {
            get
            {
                return (this.Name != "") ? this.Series.Element.SourceDS.Tables[this.Name] : null;
            }
        }

        /*
        public string Name
        {
            get
            {
                return this.Table != null ? this.Table.TableName : "";
            }
        }

        public DataTable Table
        {
            get { return this.table; }
            set { this.table = value; }
        }
        */
        public List<MapPieChartSymbol> PieChartSymbols
        {
            get { return pieChartSymbols; }
            set { pieChartSymbols = value; }
        }


        public MapSerieCollection Series
        {
            get { return series; }
            set { series = value; }
        }

        /// <summary>
        /// Отображать данные серии в виде секторных диаграмм
        /// </summary>
        public bool ShowCharts
        {
            get { return this.showCharts; }
            set
            {
                this.showCharts = value;
                RestoreRuleAppearances(value);
                Series.Element.RefreshMapAppearance();
            }
        }

        public int PieChartOffset
        {
            get { return pieChartOffset; }
            set { pieChartOffset = value; }
        }

        public int PieChartSize
        {
            get { return pieChartSize; }
            set
            {
                pieChartSize = value;
                SetChartSize(value);

                RefreshPieCharts();
                Series.Element.RefreshMapAppearance();
            }
        }

        /// <summary>
        /// тип основания диаграмм
        /// </summary>
        public PieChartBase PieChartBaseType
        {
            get { return pieChartBaseType; }
            set
            {
                pieChartBaseType = value;

                RefreshPieCharts();
                Series.Element.RefreshMapAppearance();
            }
        }

        /// <summary>
        /// Максимальное значение основания диаграммы по сумме значений
        /// </summary>
        private double MaxChartBaseSum
        {
            get { return maxChartBaseSum; }
            set
            {
                maxChartBaseSum = value;
            }
        }

        /// <summary>
        /// Минимальное значение основания диаграммы по сумме значений
        /// </summary>
        private double MinChartBaseSum
        {
            get { return minChartBaseSum; }
            set
            {
                minChartBaseSum = value;
            }
        }

        /// <summary>
        /// Максимальное значение основания диаграммы по максимальному значению
        /// </summary>
        private double MaxChartBaseMaximum
        {
            get { return maxChartBaseMaximum; }
            set { maxChartBaseMaximum = value; }
        }

        /// <summary>
        /// Минимальное значение основания диаграммы по максимальному значению
        /// </summary>
        private double MinChartBaseMaximum
        {
            get { return minChartBaseMaximum; }
            set { minChartBaseMaximum = value; }
        }

        /// <summary>
        /// Пользовательское основание
        /// </summary>
        public double CustomBase
        {
            get { return customBase; }
            set
            {
                customBase = value;

                RefreshPieCharts();
                Series.Element.RefreshMapAppearance(); 
            }
        }

        /// <summary>
        /// Минимальный размер диаграммы
        /// </summary>
        public int MinPieChartSize
        {
            get { return minPieChartSize; }
            set
            {
                minPieChartSize = value;
                RefreshPieChartSize();

                RefreshPieCharts();
                Series.Element.RefreshMapAppearance();
            }
        }

        /// <summary>
        /// Максимальный размер диаграммы
        /// </summary>
        public int MaxPieChartSize
        {
            get { return maxPieChartSize; }
            set
            {
                maxPieChartSize = value;
                RefreshPieChartSize();

                RefreshPieCharts();
                Series.Element.RefreshMapAppearance();
            }
        }

        public List<Color> PieChartColors
        {
            get { return pieChartColors; }
        }

        //привязывать смещение значка к его размеру или нет
        public bool IsBindOffsetWithSize
        {
            get { return isBindOffsetWithSize; }
            set { isBindOffsetWithSize = value; }
        }

        /*
        public MapSerie(MapSerieCollection series, DataTable table)
        {
            this.series = series;
            this.table = table;
            this.serieRules = new List<SerieRule>();
        }
        */
        public MapSerie(MapSerieCollection series, string name)
        {
            this.series = series;
            this.name = name;
            this.serieRules = new List<SerieRule>();
        }
        
        /// <summary>
        /// запоминаем настройки секторной диаграммы
        /// </summary>
        public void SavePieChartAppearance()
        {
            MapSerie pieChartSerie = this.Series.GetPieChartSerie(this.Name);
            if (pieChartSerie != null)
            {
                pieChartSerie.pieChartBaseType = this.PieChartBaseType;
                pieChartSerie.pieChartSize = this.PieChartSize;
                pieChartSerie.minPieChartSize = this.MinPieChartSize;
                pieChartSerie.maxPieChartSize = this.MaxPieChartSize;
                pieChartSerie.customBase = this.CustomBase;
                pieChartSerie.pieChartColors = this.PieChartColors;
                pieChartSerie.pieChartSymbols = this.pieChartSymbols;
            }
        }

        /// <summary>
        /// восстанавливаем настройки секторной диаграммы
        /// </summary>
        private void RestorePieChartAppearance()
        {
            MapSerie pieChartSerie = Series.GetPieChartSerie(this.Name);
            if (pieChartSerie != null)
            {
                this.pieChartBaseType = pieChartSerie.PieChartBaseType;
                this.pieChartSize = pieChartSerie.PieChartSize;
                this.minPieChartSize = pieChartSerie.MinPieChartSize;
                this.maxPieChartSize = pieChartSerie.MaxPieChartSize;
                this.customBase = pieChartSerie.CustomBase;
            }
        }

        /// <summary>
        /// восстановить внешний вид правил
        /// </summary>
        private void RestoreRuleAppearances(bool showCharts)
        {
            MapSerie pieChartSerie = Series.GetPieChartSerie(this.Name);

            if (pieChartSerie != null)
            {
                pieChartSerie.SetShowCharts(showCharts);
                if (!showCharts)
                {
                    foreach (SerieRule rule in pieChartSerie.SerieRules)
                    {
                        if (rule.RuleAppearance is SymbolRule)
                        {
                            SymbolRule symbolRule = (SymbolRule)rule.RuleAppearance;
                            symbolRule.Name = GetNameForSymbolRule(this.Series.Element.Map.SymbolRules);
                            this.Series.Element.Map.SymbolRules.Add(symbolRule);
                        }
                        else if (rule.RuleAppearance is ShapeRule)
                        {
                            this.Series.Element.Map.ShapeRules.Clear();
                            this.Series.Element.Map.ShapeRules.Add((ShapeRule)rule.RuleAppearance);
                            //this.Series.Element.Map.ColorSwatchPanel.Title = rule.Name;
                        }
                        //rule.SaveRuleAppearances(this.Series.Element.Map);
                    }
                }
                else
                {
                    foreach (SerieRule rule in pieChartSerie.SerieRules)
                    {
                        SerieRule serieRule = this.GetSerieRule(rule.Name);
                        if (serieRule != null)
                        {
                            if ((serieRule.IsFillMap) && (serieRule.RuleAppearance is ShapeRule))
                            {
                                this.Series.Element.Map.ShapeRules.Clear();
                                this.Series.Element.Map.ShapeRules.Add((ShapeRule)serieRule.RuleAppearance);
                            }
                            rule.RuleAppearance = serieRule.RuleAppearance;
                            rule.Symbols = serieRule.Symbols;
                        }
                    }
                }
            }

            if (showCharts)
            {
                if (pieChartSerie == null)
                {
                    Series.PieChartSeries.Add(this);
                }
            }

        }

        private string GetNameForSymbolRule(SymbolRuleCollection symbolRules)
        {
            int i = 0;
            string ruleName = "";
            do
            {
                i++;
                ruleName = String.Format("symbolRule{0}", i);
            } while (symbolRules.GetByName(ruleName) != null);
            return ruleName;
        }

        private void SetChartSize(int value)
        {
            foreach (MapPieChartSymbol symbol in pieChartSymbols)
            {
                symbol.PieChartSymbol.Height = value;
                symbol.PieChartSymbol.Width = value;
                if(this.IsBindOffsetWithSize)
                    symbol.PieChartSymbol.Offset.Y = value / 2 + 10;
            }
        }

        /// <summary>
        /// Получить существующее правило по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SerieRule GetSerieRule(string name)
        {
            foreach (SerieRule serieRule in this.SerieRules)
            {
                if (serieRule.Name == name)
                {
                    return serieRule;
                }
            }
            return null;
        }

        public void ClearSymbols()
        {
            serieRules.Clear();
        }

        public void RefreshPieCharts()
        {
            if(!this.Series.Element.CanRefreshMapAppearance)
            {
                return;
            }

            ClearPieChartSymbols();
            CalcChartBaseLimits();
            RefreshPieChartColors();

            foreach (Shape sh in Series.Element.Map.Shapes)
            {
                if (sh["NAME"] == DBNull.Value)
                    continue;

                if (!Series.Element.IsNullShape((string)sh["NAME"]))
                {
                    Symbol pieChartDummy = new Symbol();
                    //dummySymbols.Add(pieChartDummy);
                    Series.Element.Map.Symbols.Add(pieChartDummy);

                    pieChartDummy.ParentShape = sh.Name;
                    pieChartDummy.Category = this.Name;
                    pieChartDummy.Layer = sh.Layer;
                    pieChartDummy.Width = GetPieChartSize(sh);
                    pieChartDummy.Height = pieChartDummy.Width;
                    Offset symbolOffset = GetPieChartOffset(pieChartDummy.ParentShape, pieChartDummy.Category);
                    if (symbolOffset != null)
                    {
                        pieChartDummy.Offset = symbolOffset;
                    }
                    else
                    {
                        if(this.IsBindOffsetWithSize)
                            pieChartDummy.Offset.Y = pieChartDummy.Height/2 + 10;
                        pieChartDummy.Offset.X = this.PieChartOffset;
                    }
                    pieChartDummy.Color = Color.Empty;
                    pieChartDummy.Tag = this.Name;

                    MapPieChartSymbol symbol = GetPieChartSymbolWithSettings(ref pieChartDummy);
                    pieChartSymbols.Add(symbol);
                }
            }
            //RefreshPieChartColors();
            SavePieChartAppearance();
            AddPieChartLegend();
        }

        private Offset GetPieChartOffset(string parentShape, string category)
        {
            if (this.oldPieChartSymbols == null)
            {
                return null;
            }

            foreach (Symbol symbol in this.oldPieChartSymbols)
            {
                if((symbol.ParentShape == parentShape)&&(symbol.Category == category))
                {
                    return symbol.Offset;
                }
            }
            return null;
        }

        private MapPieChartSymbol GetPieChartSymbolWithSettings(ref Symbol symbol)
        {
            List<double> valueList = new List<double>();

            Shape sh = (Shape)this.Series.Element.Map.Shapes.GetByName(symbol.ParentShape);
            string objName = (sh != null) ? (string)sh["NAME"] : "";

            DataRow row = MapHelper.GetTableRow(this.Table, objName);
            Color baseColor = Color.Gray;

            if (row == null)
            {
                return new MapPieChartSymbol(symbol, 0, valueList, baseColor);
            }

            symbol.ToolTip = "";
            //Shape sh = this.Series.Element.GetShapeByName(symbol.ParentShape);// (Shape)this.Series.Element.Map.Shapes.GetByName(symbol.ParentShape);
            if(sh != null)
            {
                symbol.ToolTip = sh.Name;
            }

            //Флаг - является ли диаграмма пустой(все значения пустые)
            bool isEmptyChart = true;

            for (int i = 3; i < this.Table.Columns.Count; i++)
            {
                SerieRule serieRule = this.GetSerieRule(this.Table.Columns[i].ColumnName);
                if (serieRule.IsFillMap)
                    continue;

                if (row[i] == DBNull.Value)
                {
                    valueList.Add(0);
                }
                else
                {
                    isEmptyChart = false;
                    valueList.Add((Double)row[i]);
                    symbol[MapHelper.CorrectFieldName(this.Table.Columns[i].ColumnName)] = (Double)row[i];
                }

                if (symbol.ToolTip != "")
                {
                    symbol.ToolTip += "\n";
                }
                symbol.ToolTip += this.Table.Columns[i].ColumnName + ": #" + MapHelper.CorrectFieldName(this.Table.Columns[i].ColumnName).ToUpper();
            }

            if (isEmptyChart)
                return new MapPieChartSymbol(symbol, 0, valueList, baseColor, true);

            double baseValue = 0;

            switch (this.PieChartBaseType)
            {
                case PieChartBase.Sum:
                case PieChartBase.DynamicSum:
                    foreach (double value in valueList)
                    {
                        if (value > 0)
                            baseValue += value;
                    }
                    break;
                case PieChartBase.Max:
                case PieChartBase.DynamicMax:
                    foreach (double value in valueList)
                    {
                        if (value > 0)
                            baseValue = Math.Max(baseValue, value);
                    }
                    for (int i = 0; i < valueList.Count; i++)
                    {
                        if (valueList[i] == baseValue)
                        {
                            valueList[i] = 0;
                            if (this.PieChartColors.Count > i)
                            {
                                baseColor = this.PieChartColors[i];
                            }
                            break;
                        }
                    }

                    break;
                case PieChartBase.Custom:
                    baseValue = this.CustomBase;
                    break;
            }

            return new MapPieChartSymbol(symbol, baseValue, valueList, baseColor);
        }

        public void RefreshPieChartColors()
        {
            pieChartColors.Clear();
            for (int i = 3; i < this.Table.Columns.Count; i++)
            {
                SerieRule serieRule = this.GetSerieRule(this.Table.Columns[i].ColumnName);
                if (serieRule.IsFillMap)
                    continue;

                pieChartColors.Add(GetPieChartColor(this.Table.Columns[i].ColumnName));
            }
        }

        private int GetPieChartSize(Shape sh)
        {
            if ((this.PieChartBaseType == PieChartBase.Max) || 
                (this.PieChartBaseType == PieChartBase.Sum)||
                (this.PieChartBaseType == PieChartBase.Custom))
            {
                return this.PieChartSize;
            }

            double pieChartBaseSum = 0;
            double pieChartBaseMax = 0;


            string objectCode = (sh != null) ? (string)sh["CODE"] : "";
            //DataRow row = MapHelper.GetTableRow(this.Table, objectName);
            DataRow row = MapHelper.GetTableRowByObjCode(this.Table, objectCode);

            if (row != null)
            {
                for (int i = 3; i < this.Table.Columns.Count; i++)
                {
                    SerieRule serieRule = this.GetSerieRule(this.Table.Columns[i].ColumnName);
                    if (serieRule.IsFillMap)
                        continue;

                    if (row[i] != DBNull.Value)
                    {
                        if ((Double) row[i] > 0)
                        {
                            pieChartBaseSum += (Double) row[i];
                            pieChartBaseMax = Math.Max(pieChartBaseMax, (Double) row[i]);
                        }
                    }
                }
            }
            double pieChartSize = 0;

            

            if (this.PieChartBaseType == PieChartBase.DynamicSum)
            {
                pieChartSize = Math.Round(((this.MaxPieChartSize - this.MinPieChartSize) /
                                           (this.MaxChartBaseSum - this.MinChartBaseSum))*
                                          (pieChartBaseSum - this.MinChartBaseSum));
            }

            if (this.PieChartBaseType == PieChartBase.DynamicMax)
            {
                pieChartSize = Math.Round(((this.MaxPieChartSize - this.MinPieChartSize) /
                                           (this.MaxChartBaseMaximum - this.MinChartBaseMaximum)) *
                                          (pieChartBaseMax - this.MinChartBaseMaximum));
            }

            return (int)pieChartSize + this.MinPieChartSize;
        }

        private void RefreshPieChartSize()
        {
            CalcChartBaseLimits();
            foreach (MapPieChartSymbol symbol in pieChartSymbols)
            {
                Shape parentShape = (Shape)this.Series.Element.Map.Shapes.GetByName(symbol.PieChartSymbol.ParentShape);
                symbol.PieChartSymbol.Height = GetPieChartSize(parentShape);
                symbol.PieChartSymbol.Width = symbol.PieChartSymbol.Height;
                if (IsBindOffsetWithSize)
                    symbol.PieChartSymbol.Offset.Y = symbol.PieChartSymbol.Height / 2 + 10;
            }

        }

        public void ClearPieChartSymbols()
        {
            pieChartSymbols.Clear();
        }

        public void SetShowCharts(bool value)
        {
            this.showCharts = value;
        }

        /// <summary>
        /// Добавить показатель в секторную диаграмму
        /// </summary>
        /// <param name="valueName">имя показателя</param>
        public void AddPieChartPart(string valueName)
        {
            this.showCharts = true;
            SerieRule serieRule = null;

            MapSerie pieChartSerie = Series.GetPieChartSerie(this.Name);
            if (pieChartSerie != null)
            {
                RestorePieChartAppearance();
                serieRule = pieChartSerie.GetSerieRule(valueName);
            }

            if (serieRule == null)
            {
                serieRule = new SerieRule(this, valueName, MapHelper.GetRandomColor());
                if (pieChartSerie != null)
                {
                    pieChartSerie.SerieRules.Add(serieRule);
                }
            }
            serieRule.SetIsFillMap(false);
            this.serieRules.Add(serieRule);
        }
        
        public void AddPieChartLegend()
        {
            Legend pieChartLegend = (Legend)Series.Element.Map.Legends.GetByName(this.Name);
            if (pieChartLegend == null)
            {
                pieChartLegend = Series.Element.Map.Legends.Add(this.Name);
                pieChartLegend.Title = (this.Name == "Серия") ? "" : this.Name;
            }
            else
            {
                pieChartLegend.Items.Clear();
            }

            foreach(SerieRule rule in this.serieRules)
            {
                if (rule.IsFillMap)
                    continue;

                LegendItem legItem = pieChartLegend.Items.Add(rule.Name);
                legItem.Color = rule.PieChartColor;
                legItem.Text = rule.Name;
            }
        }

        public void AddSymbolSerieRule(List<Symbol> symbols, string valueName)
        {
            SerieRule serieRule = new SerieRule(this, symbols, valueName);
            serieRule.SetIsFillMap(false);
            this.serieRules.Add(serieRule);
        }

        public void AddShapeSerieRule(List<Symbol> symbols, string valueName)
        {
            SerieRule serieRule = new SerieRule(this, symbols, valueName);
            serieRule.SymbolsVisible = false;
            serieRule.SetIsFillMap(true);
            if (this.Series.Element.Map.ShapeRules.Count > 0)
                serieRule.RuleAppearance = this.Series.Element.Map.ShapeRules[0];
            this.serieRules.Add(serieRule);
        }

        public Color GetPieChartColor(string valueName)
        {
            foreach(SerieRule rule in this.SerieRules)
            {
                if (rule.Name == valueName)
                {
                    return rule.PieChartColor;
                }
            }
            return Color.Empty;
        }

        public void SetOldPieChartSymbols(SymbolCollection symbols)
        {
            this.oldPieChartSymbols.Clear();
            foreach(Symbol symbol in symbols)
            {
                this.oldPieChartSymbols.Add(symbol);
            }
        }

        public void RefreshDataView(List<Symbol> symbols)
        {
            if (this.ShowCharts)
            {
                MapSerie pieChartSerie = this.Series.GetPieChartSerie(this.Name);
                if (pieChartSerie != null)
                {
                    this.oldPieChartSymbols.Clear();
                    /*
                    foreach(MapPieChartSymbol pieChartSymbol in pieChartSerie.pieChartSymbols)
                    {
                        this.oldPieChartSymbols.Add(pieChartSymbol.PieChartSymbol);
                    }*/
                    foreach(Symbol s in symbols)
                    {
                        if (this.Name == s.Category)
                            this.oldPieChartSymbols.Add(s);
                    }
                }
                //this.oldPieChartSymbols = symbols;
                RefreshPieCharts();
            }
            else
            {
                ClearPieChartSymbols();
            }
        }

        /// <summary>
        /// Вычислить пределы (минимум и максимум) для оснований секторных диаграмм серии
        /// </summary>
        private void CalcChartBaseLimits()
        {
            if(!this.Series.Element.CanRefreshMapAppearance)
            {
                return;
            }
            this.MaxChartBaseSum = 0;
            this.MinChartBaseSum = 0;
            this.MaxChartBaseMaximum = 0;
            this.MinChartBaseMaximum = 0;

            foreach (DataRow row in this.Table.Rows)
            {
                double curChartBaseSum = 0;
                double curChartBaseMax = 0;
                if (row[Consts.objectsColumn] == DBNull.Value)
                    continue;

                string shapeName = (string)row[Consts.objectsColumn];

                if (!this.Series.Element.ShapeExists(shapeName))
                {
                    continue;
                }

                for (int i = 3; i < this.Table.Columns.Count; i++)
                {
                    SerieRule serieRule = this.GetSerieRule(this.Table.Columns[i].ColumnName);
                    if (serieRule.IsFillMap)
                        continue;

                    if (row[i] != DBNull.Value)
                    {
                        if ((Double) row[i] > 0)
                        {
                            curChartBaseSum += (Double)row[i];
                            curChartBaseMax = Math.Max(curChartBaseMax, (Double)row[i]);
                        }
                    }
                }
                this.MaxChartBaseSum = Math.Max(this.MaxChartBaseSum, curChartBaseSum);
                this.MinChartBaseSum = Math.Min(this.MinChartBaseSum, curChartBaseSum);
                this.MaxChartBaseMaximum = Math.Max(this.MaxChartBaseMaximum, curChartBaseMax);
                this.MinChartBaseMaximum = Math.Min(this.MinChartBaseMaximum, curChartBaseMax);
            }
        }


        public MapPieChartSymbol GetPieChartSymbol(string shapeName, string category)
        {
            foreach (MapPieChartSymbol symbol in this.pieChartSymbols)
            {
                if((symbol.PieChartSymbol.ParentShape == shapeName)&&(symbol.PieChartSymbol.Category == category))
                {
                    return symbol;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return this.Name;
        }

    }

    /// <summary>
    /// Тип основания секторной диаграммы
    /// </summary>
    public enum PieChartBase
    {
        [Description("Сумма")]
        Sum,
        [Description("Динамическая сумма")]
        DynamicSum,
        [Description("Максимум")]
        Max,
        [Description("Динамический максимум")]
        DynamicMax,
        [Description("Пользовательское")]
        Custom
    }
}
