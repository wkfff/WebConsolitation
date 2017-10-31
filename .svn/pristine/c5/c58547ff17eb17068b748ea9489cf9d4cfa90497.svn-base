using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using Dundas.Maps.WinControl;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Правило для значения серии(заливка или значки)
    /// </summary>
    public class SerieRule
    {
        #region Поля

        private MapSerie serie;
        private string valueName;
        private List<Symbol> symbols;
        private RuleBase ruleAppearance;
        private bool isFillMap;

        private SymbolRule symbolRule;
        private ShapeRule shapeRule;

        private StringCollection symbolTextValues = new StringCollection();
        private StringCollection shapeTextValues = new StringCollection();

        private Color pieChartColor = Color.Empty; 

        private int digitCount = 2;
        private List<double> rangeLimits;
        private DataGroupingExt dataGrouping = DataGroupingExt.EqualInterval;
        private int rangeCount = 5; 

        #endregion 

        #region Свойства

        public string Name
        {
            get { return valueName; }
        }

        //отображать значки или нет
        public bool SymbolsVisible
        {
            set
            {
                if (this.Symbols == null)
                    return;

                foreach (Symbol symbol in this.Symbols)
                {
                    symbol.Visible = value;
                }
            }
        }

        // true - если заливка
        public bool IsFillMap
        {
            get { return isFillMap; }
            set
            {
                if (isFillMap != value)
                {
                    SetRule(value);
                    SymbolsVisible = !value;
                    isFillMap = value;
                }
            }
        }

        /// <summary>
        /// установка свойства IsFillMap в нужное значение
        /// </summary>
        public void SetIsFillMap(bool value)
        {
            this.isFillMap = value;
        }

        // правило отображения данных на карте (заливка или значки)
        public RuleBase RuleAppearance
        {
            get { return ruleAppearance; }
            set { ruleAppearance = value; }
        }

        //список значений для выпадающего списка свойства "Подписи" для значков
        public StringCollection SymbolTextValues
        {
            get { return symbolTextValues; }
            set { symbolTextValues = value; }
        }

        //список значений для выпадающего списка свойства "Подписи" для заливки
        public StringCollection ShapeTextValues
        {
            get { return shapeTextValues; }
            set { shapeTextValues = value; }
        }

        public Color PieChartColor
        {
            get { return pieChartColor; }
            set
            {
                pieChartColor = value;
                Legend legend = (Legend) this.serie.Series.Element.Map.Legends.GetByName(serie.Name);
                if (legend != null)
                {
                    LegendItem legendItem = (LegendItem)legend.Items.GetByName(this.Name);
                    if (legendItem != null)
                        legendItem.Color = value;
                }
                //this.serie.RefreshPieCharts();
                this.serie.Series.Element.RefreshMapAppearance();
            }
        }

        public MapSerie Serie
        {
            get { return this.serie; }
        }

        public List<Symbol> Symbols
        {
            get { return this.symbols; }
            set { this.symbols = value; }
        }

        //настройка для редактора интервалов. кол-во отображаемых десятичных знаков после запятой
        public int DigitCount
        {
            get
            {
                return digitCount;
            }
            set
            {
                if(this.symbolRule != null)
                {
                    if (this.symbolRule.Tag != null)
                    {
                        ((SerieRuleData)this.symbolRule.Tag).DigitCount = value;
                    }
                    else
                    {
                        this.symbolRule.Tag = new SerieRuleData(value, this.RangeLimits, this.DataGrouping);
                    }
                }

                if (this.shapeRule != null)
                {
                    if (this.shapeRule.Tag != null)
                    {
                        ((SerieRuleData)this.shapeRule.Tag).DigitCount = value;
                    }
                    else
                    {
                        this.shapeRule.Tag = new SerieRuleData(value, this.RangeLimits, this.DataGrouping);
                    }
                }
                digitCount = value;
            }
        }

        //границы пользовательских интервалов значений
        public List<double> RangeLimits
        {
            get { return rangeLimits; }
            set
            {
                rangeLimits = value;
                if (this.symbolRule != null)
                {
                    if (this.symbolRule.Tag != null)
                    {
                        ((SerieRuleData)this.symbolRule.Tag).RangeLimits = value;
                    }
                    else
                    {
                        this.symbolRule.Tag = new SerieRuleData(this.DigitCount, value, this.DataGrouping);
                    }
                }

            }
        }

        //тип разбиения интервалов
        public DataGroupingExt DataGrouping
        {
            get { return dataGrouping; }
            set
            {
                dataGrouping = value;
                if (this.symbolRule != null)
                {
                    if (this.symbolRule.Tag != null)
                    {
                        ((SerieRuleData)this.symbolRule.Tag).DataGrouping = value;
                    }
                    else
                    {
                        this.symbolRule.Tag = new SerieRuleData(this.DigitCount, this.RangeLimits, value);
                    }
                }

            }
        }

        //количество интервалов
        public int RangeCount
        {
            get { return this.rangeCount; }
            set { this.rangeCount = value; }
        }


        #endregion 

        public SerieRule(MapSerie serie, List<Symbol> symbols, string valueName)
        {
            this.serie = serie;
            this.valueName = valueName;
            this.symbols = symbols;
            this.pieChartColor = MapHelper.GetRandomColor();

            InitRule();
            symbolTextValues.Add("");
            shapeTextValues.Add("");
        }

        public SerieRule(MapSerie serie, string valueName, Color pieChartColor)
        {
            this.serie = serie;
            this.valueName = valueName;
            this.pieChartColor = pieChartColor;
            symbolTextValues.Add("");
            shapeTextValues.Add("");
        }

        /// <summary>
        /// Инициализация свойств при создании правила
        /// </summary>
        public void InitRule()
        {
            
            if (this.serie.ShowCharts)
            {
                return;
            }
            
            if (shapeRule == null)
                shapeRule = GetShapeRule();
            if(symbolRule == null)
                symbolRule = GetSymbolRule();

            if (shapeRule != null)
            {
                RuleAppearance = shapeRule;
                isFillMap = true;

                GetShapeRuleRangeSettings();
            }
            else
            {
                if (this.serie.ShowCharts)
                {
                    return;
                }

                RuleAppearance = symbolRule;
                isFillMap = false;

                GetSymbolRuleRangeSettings();
            }

            if ((this.symbolRule != null)&&(this.symbolRule.Tag == null))
                this.symbolRule.Tag = new SerieRuleData(this.digitCount, this.rangeLimits, this.dataGrouping);

        }

        private DataGroupingExt GetShapeRuleDataGrouping(ShapeRule rule)
        {
            if (rule.CustomColors.Count > 0)
            {
                if (rule.CustomColors[0].FromValue != "")
                {
                    return DataGroupingExt.Custom;
                }
            }

            switch (rule.DataGrouping)
            {
                case Dundas.Maps.WinControl.DataGrouping.EqualDistribution:
                    return DataGroupingExt.EqualDistribution;
                case Dundas.Maps.WinControl.DataGrouping.EqualInterval:
                    return DataGroupingExt.EqualInterval;
                case Dundas.Maps.WinControl.DataGrouping.Optimal:
                    return DataGroupingExt.Optimal;
            }

            return DataGroupingExt.EqualInterval;
        }

        private List<double> GetShapeRuleRangeLimits(ShapeRule rule)
        {
            List<double> rangeLimits = new List<double>();

            double rangeLimit;
            for (int i = 0; i < rule.CustomColors.Count; i++)
            {
                Double.TryParse(rule.CustomColors[i].FromValue, out rangeLimit);
                rangeLimits.Add(rangeLimit);
            }
            Double.TryParse(rule.CustomColors[rule.CustomColors.Count - 1].ToValue,
                         out rangeLimit);
            rangeLimits.Add(rangeLimit);


            return rangeLimits;

        }

        private DataGroupingExt GetSymbolRuleDataGrouping(SymbolRule rule)
        {
            if (rule.PredefinedSymbols.Count > 0)
            {
                if (rule.PredefinedSymbols[0].FromValue != "")
                {
                    return DataGroupingExt.Custom;
                }
            }

            switch (rule.DataGrouping)
            {
                case Dundas.Maps.WinControl.DataGrouping.EqualDistribution:
                    return DataGroupingExt.EqualDistribution;
                case Dundas.Maps.WinControl.DataGrouping.EqualInterval:
                    return DataGroupingExt.EqualInterval;
                case Dundas.Maps.WinControl.DataGrouping.Optimal:
                    return DataGroupingExt.Optimal;
            }

            return DataGroupingExt.EqualInterval;
        }

        private List<double> GetSymbolRuleRangeLimits(SymbolRule rule)
        {
            List<double> rangeLimits = new List<double>();

            double rangeLimit;
            for (int i = 0; i < rule.PredefinedSymbols.Count; i++)
            {
                Double.TryParse(rule.PredefinedSymbols[i].FromValue, out rangeLimit);
                rangeLimits.Add(rangeLimit);
            }
            Double.TryParse(rule.PredefinedSymbols[rule.PredefinedSymbols.Count - 1].ToValue,
                         out rangeLimit);
            rangeLimits.Add(rangeLimit);

            return rangeLimits;
        }


        /// <summary>
        /// добавление правила для значков
        /// </summary>
        private void AddSymbolRule()
        {

            //создаем правило для символов
            symbolRule = serie.Series.GetSymbolRule(serie.Name, valueName);
            foreach (SymbolRule rule in serie.Series.Element.Map.SymbolRules)
            {
                if ((rule.SymbolField == MapHelper.CorrectFieldName(valueName)) && (rule.Category == serie.Name))
                {
                    serie.Series.Element.Map.SymbolRules.Remove(rule);
                    break;
                }
            }
            symbolRule.Name = "";
            serie.Series.Element.Map.SymbolRules.Add(symbolRule);
            RuleAppearance = symbolRule;

            //мочим правило для заливки
            if (this.shapeRule == null)
                this.shapeRule = GetShapeRule();

            GetShapeRuleRangeSettings();
            CopyRangesToSymbols();
            ShapeRule shRule = GetShapeRule();
            if (shRule != null)
                serie.Series.Element.Map.ShapeRules.Remove(shRule);

        }

        /// <summary>
        /// добавление правила для заливки
        /// </summary>
        private void AddShapeRule()
        {

            //создаем правило для заливки объектов
            shapeRule = serie.Series.GetShapeRule(serie.Name, valueName);
            serie.Series.Element.Map.ShapeRules.Clear();
            serie.Series.Element.Map.ShapeRules.Add(shapeRule);
            RuleAppearance = shapeRule;

            //мочим правило для значков
            if (this.symbolRule == null)
                this.symbolRule = GetSymbolRule();

            GetSymbolRuleRangeSettings();
            CopyRangesToShapes();

            SymbolRule symRule = GetSymbolRule();
            if (symRule != null)
                serie.Series.Element.Map.SymbolRules.Remove(GetSymbolRule());

            //ShapeRuleBrowseAdapter srba = new ShapeRuleBrowseAdapter(shapeRule, this);
            if (this.Serie.Series.Element.Map.Legends.GetByName("Заливка") != null)
            {
                Legend legend = (Legend) this.Serie.Series.Element.Map.Legends.GetByName("Заливка");
                legend.Title = (shapeRule.Category == "Серия") ? "" : shapeRule.Category;

                if (legend.CellColumns.GetByName("name") != null)
                    legend.CellColumns["name"].HeaderText = shapeRule.ShapeField;

            }

        }

        /// <summary>
        /// Удаление других правил заливки
        /// </summary>
        private void RemoveOldFillMap()
        {
            foreach(MapSerie mapSerie in serie.Series.Items)
            {
                foreach(SerieRule serieRule in mapSerie.SerieRules)
                {
                    if (serieRule.IsFillMap)
                    {
                        serieRule.IsFillMap = false;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Установка правила для значков/заливки в зависимости от параметра
        /// </summary>
        /// <param name="isFillMap">true - если создаем правило для заливки, false - правило для значков</param>
        private void SetRule(bool isFillMap)
        {
            foreach(Shape sh in serie.Series.Element.Map.Shapes)
            {
                sh.ParentGroup = sh.Layer;
            }
            if (isFillMap)
            {
                RemoveOldFillMap();
                AddShapeRule();
            }
            else
            {
                AddSymbolRule();
            }

        }

        private bool GetSymbolRuleRangeSettings()
        {
            if (this.Serie.Series.IsProportionalSymbolSize)
                return false;

            if (symbolRule != null)
            {
                this.RangeCount = this.symbolRule.PredefinedSymbols.Count;
                this.DataGrouping = GetSymbolRuleDataGrouping(this.symbolRule);
                if (this.DataGrouping == DataGroupingExt.Custom)
                    this.RangeLimits = GetSymbolRuleRangeLimits(this.symbolRule);
                if (symbolRule.Tag != null)
                {
                    this.digitCount = ((SerieRuleData)symbolRule.Tag).DigitCount;
                }

                return true;
            }
            return false;
        }

        private bool GetShapeRuleRangeSettings()
        {
            if (this.Serie.Series.IsProportionalSymbolSize)
                return false;

            if (shapeRule != null)
            {
                this.RangeCount = this.shapeRule.CustomColors.Count;
                this.DataGrouping = GetShapeRuleDataGrouping(this.shapeRule);
                if (this.DataGrouping == DataGroupingExt.Custom)
                    this.RangeLimits = GetShapeRuleRangeLimits(this.shapeRule);
                if (shapeRule.Tag != null)
                {
                    this.digitCount = ((SerieRuleData)shapeRule.Tag).DigitCount;
                }
                return true;
            }
            return false;
        }

        private void CopyRangesToShapes()
        {
            if (this.symbolRule == null)
                return;

            if (this.DataGrouping == DataGroupingExt.Custom)
            {
                if (this.symbolRule.PredefinedSymbols.Count == 0)
                {
                    return;
                }

                this.shapeRule.ColorCount = this.symbolRule.PredefinedSymbols.Count;
                this.SetShapeIntervalCount(this.symbolRule.PredefinedSymbols.Count);

                for (int i = 0; i < this.symbolRule.PredefinedSymbols.Count; i++)
                {
                    if (i < this.shapeRule.CustomColors.Count)
                    {
                        this.shapeRule.CustomColors[i].FromValue = this.symbolRule.PredefinedSymbols[i].FromValue;
                        this.shapeRule.CustomColors[i].ToValue = this.symbolRule.PredefinedSymbols[i].ToValue;
                    }
                }
            }

            this.Serie.Series.Element.ColorIntervalNames.Clear();
            for (int i = 0; i < this.symbolRule.PredefinedSymbols.Count; i++)
            {
                this.Serie.Series.Element.ColorIntervalNames.Add(this.symbolRule.PredefinedSymbols[i].LegendText);
            }

        }
        
        private void CopyRangesToSymbols()
        {
            if (this.shapeRule == null)
                return;

            if (this.DataGrouping == DataGroupingExt.Custom)
            {

                if (this.shapeRule.CustomColors.Count == 0)
                {
                    return;
                }

                SetSymbolIntervalCount(this.shapeRule.CustomColors.Count);

                for (int i = 0; i < this.shapeRule.CustomColors.Count; i++)
                {
                    if (i < this.symbolRule.PredefinedSymbols.Count)
                    {
                        this.symbolRule.PredefinedSymbols[i].FromValue = this.shapeRule.CustomColors[i].FromValue;
                        this.symbolRule.PredefinedSymbols[i].ToValue = this.shapeRule.CustomColors[i].ToValue;
                    }
                }
            }

            for (int i = 0; i < this.shapeRule.CustomColors.Count; i++)
            {
                if (i < this.symbolRule.PredefinedSymbols.Count)
                {
                    if (i < this.Serie.Series.Element.ColorIntervalNames.Count)
                        this.symbolRule.PredefinedSymbols[i].LegendText =
                            this.Serie.Series.Element.ColorIntervalNames[i];
                }
            }

        }


        private void SetSymbolIntervalCount(int value)
        {
            if (value == symbolRule.PredefinedSymbols.Count)
            {
                return;
            }

            while (symbolRule.PredefinedSymbols.Count >= value)
            {
                symbolRule.PredefinedSymbols.RemoveAt(symbolRule.PredefinedSymbols.Count - 1);
            }

            while (symbolRule.PredefinedSymbols.Count < value)
            {
                PredefinedSymbol symbol = new PredefinedSymbol();
                symbolRule.PredefinedSymbols.Add(symbol);
            }
        }

        private void SetShapeIntervalCount(int value)
        {
            if (value == shapeRule.CustomColors.Count)
            {
                return;
            }

            while (shapeRule.CustomColors.Count >= value)
            {
                shapeRule.CustomColors.RemoveAt(shapeRule.CustomColors.Count - 1);
            }

            while (shapeRule.CustomColors.Count < value)
            {
                CustomColor customColor = new CustomColor();
                shapeRule.CustomColors.Add(customColor);
            }
        }

        /// <summary>
        /// установка пропорционального размера значков
        /// </summary>
        /// <param name="minValue">минимальное значение границы интервала</param>
        /// <param name="maxValue">максимальное значение границы интервала</param>
        public void SetProportionalSizeSymbols(bool value, double minValue, double maxValue)
        {
            if (this.symbolRule == null)
                return;
            List<double> rangeLimits;
            if (value)
            {
                //сохраняем настройки правила
                //this.symbolRule.Tag = new SerieRuleData(this.digitCount, this.rangeLimits, this.dataGrouping);

                //рассчитываем и задаем новые границы интервалов для пропорционального отображения значков
                rangeLimits = MapHelper.GetRangeLimits(this.symbolRule.PredefinedSymbols.Count, minValue,
                                                                    maxValue, this.DigitCount);
                for (int i = 0; i < rangeLimits.Count - 1; i++)
                {
                    this.symbolRule.PredefinedSymbols[i].FromValue = rangeLimits[i].ToString();
                    this.symbolRule.PredefinedSymbols[i].ToValue = rangeLimits[i + 1].ToString();
                }
            }
            else
            {
                //возвращаем старые сохраненные настройки интервалов
                RestoreSymbolDataGrouping(ref this.symbolRule);
            }

        }

        private void RestoreSymbolDataGrouping(ref SymbolRule rule)
        {
            ClearIntervals(ref rule);

            if (rule.Tag != null)
            {
                SerieRuleData ruleData = (SerieRuleData) this.symbolRule.Tag;

                switch (ruleData.DataGrouping)
                {
                    case DataGroupingExt.EqualDistribution:
                        rule.DataGrouping = Dundas.Maps.WinControl.DataGrouping.EqualDistribution;
                        break;
                    case DataGroupingExt.EqualInterval:
                        rule.DataGrouping = Dundas.Maps.WinControl.DataGrouping.EqualInterval;
                        break;
                    case DataGroupingExt.Optimal:
                        rule.DataGrouping = Dundas.Maps.WinControl.DataGrouping.Optimal;
                        break;
                    case DataGroupingExt.Custom:
                        rule.DataGrouping = Dundas.Maps.WinControl.DataGrouping.EqualInterval;
                        List<double> rangeLimits = ruleData.RangeLimits;
                        if (rangeLimits == null)
                            rangeLimits = MapHelper.GetRangeLimits(rule.PredefinedSymbols.Count, this.Serie.Table,
                                                                        this.Name, ruleData.DigitCount);

                        for (int i = 0; i < rule.PredefinedSymbols.Count; i++)
                        {
                            if (i < rangeLimits.Count - 1)
                            {
                                this.symbolRule.PredefinedSymbols[i].FromValue = rangeLimits[i].ToString();
                                this.symbolRule.PredefinedSymbols[i].ToValue = rangeLimits[i + 1].ToString();
                            }
                            else
                            {
                                this.symbolRule.PredefinedSymbols[i].FromValue = rangeLimits[rangeLimits.Count - 1].ToString();
                                this.symbolRule.PredefinedSymbols[i].ToValue = rangeLimits[rangeLimits.Count - 1].ToString();
                            }
                        }
                        break;
                }

            }
            else
            {
                rule.DataGrouping = Dundas.Maps.WinControl.DataGrouping.EqualInterval;
            }
        }


        private void ClearIntervals(ref SymbolRule symbolRule)
        {
            foreach (PredefinedSymbol symbol in symbolRule.PredefinedSymbols)
            {
                symbol.FromValue = "";
                symbol.ToValue = "";
            }

        }

        /// <summary>
        /// Получение существующего правила для значков
        /// </summary>
        /// <returns>Если правило есть, то возвращает его, иначе - null</returns>
        private SymbolRule GetSymbolRule()
        {
            foreach (SymbolRule rule in serie.Series.Element.Map.SymbolRules)
            {
                if ((rule.SymbolField == MapHelper.CorrectFieldName(valueName)) && (rule.Category == serie.Name))
                {
                    return rule;
                }
            }
            return null;
        }

        /// <summary>
        /// Получение существующего правила для заливки
        /// </summary>
        /// <returns>Если правило есть, то возвращает его, иначе - null</returns>
        private ShapeRule GetShapeRule()
        {
            foreach (ShapeRule rule in serie.Series.Element.Map.ShapeRules)
            {
                if ((rule.ShapeField == MapHelper.CorrectFieldName(valueName)) && (rule.Category == serie.Name))
                {
                    return rule;
                }
            }
            return null;
        }

        /// <summary>
        /// Сохранение настроек правил заливки и значков
        /// </summary>
        /// <param name="map"></param>
        public void SaveRuleAppearances(MapControl map)
        {
            if (this.symbolRule != null)
            {
                symbolRule.Name = "";
                map.SymbolRules.Add(this.symbolRule);
            }
            if (this.shapeRule != null)
            {
                map.ShapeRules.Clear();
                map.ShapeRules.Add(this.shapeRule);
                //map.ColorSwatchPanel.Title = this.Name;
            }

        }


        public override string ToString()
        {
            return this.valueName;
        }


    }


    public class SerieRuleData
    {
        private int digitCount = 2;
        private List<double> rangeLimits;
        private DataGroupingExt dataGrouping = DataGroupingExt.EqualInterval;


        public int DigitCount
        {
            get { return digitCount; }
            set { digitCount = value; }
        }

        public List<double> RangeLimits
        {
            get { return rangeLimits; }
            set { rangeLimits = value; }
        }

        public DataGroupingExt DataGrouping
        {
            get { return dataGrouping; }
            set { dataGrouping = value; }
        }

        public SerieRuleData(int digitCount, List<double> rangeLimits, DataGroupingExt dataGrouping)
        {
            this.digitCount = digitCount;
            this.rangeLimits = rangeLimits;
            this.dataGrouping = dataGrouping;
        }

    }
}
