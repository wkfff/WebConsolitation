using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Xml;
using Dundas.Maps.WinControl;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert
{
    public class MapSerieCollection : IEnumerable
    {
        private List<MapSerie> items;
        private MapReportElement element;
        private List<MapSerie> pieChartSeries;
        private bool isProportionalSymbolSize;
        private List<string> notProportionalMeasures = new List<string>();

        private string currSerie;
        private ShapeCollection shapes;
        private List<SymbolRule> symbolRules = new List<SymbolRule>();
        private List<ShapeRule> shapeRules = new List<ShapeRule>();

        public MapReportElement Element
        {
            get { return element; }
            set { element = value; }
        }

        public List<ShapeRule> ShapeRules
        {
            get { return shapeRules; }
            set { shapeRules = value; }
        }

        public List<SymbolRule> SymbolRules
        {
            get { return symbolRules; }
            set { symbolRules = value; }
        }

        public MapSerie this[int index]
        {
            get
            {
                return this.items[index];
            }
        }

        public MapSerie this[string name]
        {
            get
            {
                this.currSerie = name;
                return this.items.Find(SerieExist);
            }
        }

        public List<MapSerie> Items
        {
            get
            {
                return this.items;
            }
        }

        //серии, отображаемые в виде секторных диаграмм
        public List<MapSerie> PieChartSeries
        {
            get { return pieChartSeries; }
            set { pieChartSeries = value; }
        }

        public bool IsProportionalSymbolSize
        {
            get { return this.isProportionalSymbolSize; }
            set
            {
                SetProportionalSymbolSize(value);
                this.isProportionalSymbolSize = value;
                this.Element.RefreshMapAppearance();
            }
        }

        public List<string> NotProportionalMeasures
        {
            get { return notProportionalMeasures; }
            set { notProportionalMeasures = value; }
        }

        public MapSerieCollection(ShapeCollection shapes)
        {
            this.items = new List<MapSerie>();
            this.pieChartSeries = new List<MapSerie>();
            this.shapes = shapes;
        }

        public bool PieChartSeriesContains(string serieName)
        {
            MapSerie mapSerie = GetPieChartSerie(serieName);
            if (mapSerie == null)
            {
                return false;
            }
            
            return mapSerie.ShowCharts;
        }

        public MapSerie GetPieChartSerie(string serieName)
        {
            foreach(MapSerie pieChartSerie in this.PieChartSeries)
            {
                if (pieChartSerie.Name == serieName)
                {
                    return pieChartSerie;
                }
            }
            return null;
        }

        public void SavePieChartsAppearance()
        {
            foreach(MapSerie mapSerie in this.Items)
            {
                mapSerie.SavePieChartAppearance();
            }
        }

        public ShapeRule GetShapeRule(string serieName, string valueName)
        {
            return SetShapeRule(serieName, valueName, true);
            
        }

        public ShapeRule SetShapeRule(string serieName, string valueName, bool needRefresh)
        {
            
            ShapeRule shapeRule = null;
            foreach(ShapeRule rule in this.ShapeRules)
            {
                if ((rule.Category == serieName) && (rule.ShapeField == MapHelper.CorrectFieldName(valueName)))
                {
                    shapeRule = rule;
                }
            }

            
            if (shapeRule == null)
            {
                
                shapeRule = new ShapeRule();
                this.ShapeRules.Add(shapeRule);
                shapeRule.Category = serieName;
                shapeRule.ShapeField = MapHelper.CorrectFieldName(valueName);
                shapeRule.ColoringMode = ColoringMode.ColorRange;
                shapeRule.DataGrouping = DataGrouping.EqualInterval;
                shapeRule.ShowInLegend = serieName + " " + valueName;
                shapeRule.ShowInColorSwatch = true;

                string formatStr = Element.GetTotalFormatString(valueName);

                shapeRule.Text = "#" + Element.GetShapeFieldCaption() + "\n#" + MapHelper.CorrectFieldName(valueName).ToUpper() + "{" + formatStr + "}";
                shapeRule.ToolTip = "#NAME\n#" + MapHelper.CorrectFieldName(valueName).ToUpper() + "{" + formatStr + "}";
                shapeRule.LegendText = "#FROMVALUE{" + formatStr + "} - #TOVALUE{" + formatStr + "}";

                this.Element.Map.ColorSwatchPanel.Title = valueName;
            }

            if (needRefresh)
            {
                RefreshShapeValues(serieName, shapes, GetTable(serieName), valueName);
            }

            SetShapeCategory(serieName);

            ClearNullShapesFormat(valueName, GetTable(serieName));
            return shapeRule;
        }

        private void SetShapeCategory(string categoryName)
        {
            foreach(Shape shape in this.Element.Map.Shapes)
            {
                shape.Category = categoryName;
            }
        }

        public void SetShapeRule(ShapeRule rule, string serieName, string valueName)
        {
            this.AddShapeRule(rule);
            RefreshShapeValues(serieName, shapes, GetTable(serieName), valueName);
            ClearNullShapesFormat(valueName, GetTable(serieName));
        }

        public SymbolRule GetSymbolRule(string category, string fieldName)
        {
            foreach (SymbolRule rule in this.SymbolRules)
            {
                if ((rule.Category == category) && (rule.SymbolField == MapHelper.CorrectFieldName(fieldName)))
                {
                    MapHelper.SetSizePredefinedSymbols(rule);
                    return rule;
                }
            }

            SymbolRule symbRule = MapHelper.NewSymbolRule(category, fieldName);
            string formatStr = this.Element.GetTotalFormatString(fieldName);
            foreach (PredefinedSymbol symbol in symbRule.PredefinedSymbols)
            {
                symbol.Text = "#" + MapHelper.CorrectFieldName(fieldName).ToUpper() + "{" + formatStr + "}";
                symbol.ToolTip = "#" + MapHelper.CorrectFieldName(fieldName).ToUpper() + "{" + formatStr + "}";
            }
            symbRule.LegendText = "#FROMVALUE{" + formatStr + "} - #TOVALUE{" + formatStr + "}";

            return symbRule;
        }

        public void AddShapeRule(ShapeRule shapeRule)
        {
            foreach (ShapeRule rule in this.shapeRules)
            {
                if ((rule.Category == shapeRule.Category) && (rule.ShapeField == shapeRule.ShapeField))
                {
                    this.shapeRules.Remove(rule);
                    break;
                }
            }

            this.shapeRules.Add(shapeRule);
        }

        public void AddSymbolRule(SymbolRule symbolRule)
        {
            foreach (SymbolRule rule in this.symbolRules)
            {
                if ((rule.Category == symbolRule.Category) && (rule.SymbolField == symbolRule.SymbolField))
                {
                    this.symbolRules.Remove(rule);
                    break;
                }
            }

            this.symbolRules.Add(symbolRule);
        }

        public bool ShapeRuleExist(string serieName, string valueName)
        {
            foreach (ShapeRule rule in this.shapeRules)
            {
                if ((rule.Category == serieName) && (rule.ShapeField == MapHelper.CorrectFieldName(valueName)))
                {
                    return true;
                }
            }
            return false;
        }

        public bool SymbolRuleExist(string serieName, string valueName)
        {
            foreach (SymbolRule rule in this.symbolRules)
            {
                if ((rule.Category == serieName) && (rule.SymbolField == MapHelper.CorrectFieldName(valueName)))
                {
                    return true;
                }
            }
            return false;
        }

        private DataTable GetTable(string serieName)
        {
            return Element.SourceDS.Tables[serieName];
        }

        /// <summary>
        /// Перезагрузить объекты (чтобы очистить поля)
        /// </summary>
        private void ReloadShapes()
        {
            using (StringWriter strWriter = new StringWriter())
            {
                this.Element.Map.Serializer.NonSerializableContent = "";
                this.Element.Map.Serializer.SerializableContent = "Shape.*,Offset.*,Symbol.*";
                this.Element.Map.Serializer.Save(strWriter);
                this.Element.Map.Shapes.Clear();
                using (StringReader strReader = new StringReader(strWriter.ToString()))
                {
                    this.Element.Map.SuspendLayout();
                    this.Element.Map.Serializer.ResetWhenLoading = false;
                    this.Element.Map.Serializer.Load(strReader);
                    this.Element.Map.ResumeLayout();
                }
            }
        }

        public void RefreshShapeValues(string serieName, ShapeCollection shapes, DataTable dt, string valueName)
        {
            Type valueType = dt.Columns[valueName].DataType;
            string shapeFieldName = MapHelper.CorrectFieldName(valueName);

            Field field = (Field)this.Element.Map.ShapeFields.GetByName(shapeFieldName);
            if (field != null)
            {
                this.Element.Map.ShapeFields.Remove(field);
            }

            ReloadShapes();

            field = new Field();
            field.Name = shapeFieldName;
            field.Type = typeof(Double);
            this.Element.Map.ShapeFields.Add(field);


            foreach (DataRow row in dt.Rows)
            {
                if (row[Consts.objectsColumn] == DBNull.Value)
                    continue;

                if (!ShapeExists((string)row[Consts.objectsColumn]))
                {
                    continue;
                }

                Shape shape = this.Element.GetShapeByName((string)row[Consts.objectsColumn]);


                if (row[valueName] != DBNull.Value)
                {
                    shape[shapeFieldName] = row[valueName];
                    shape.Color = Color.Empty;
                    shape.ParentGroup = "";
                }

            }

            ClearNullShapes(valueName, dt);
        }

        private bool ShapeExists(string shapeName)
        {
            foreach(Shape sh in this.shapes)
            {
                
                if (sh["NAME"] == DBNull.Value)
                    continue;

                if ((string)sh["NAME"] == shapeName)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsNullShape(string shapeName, string valueName, DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[Consts.objectsColumn] == DBNull.Value)
                    continue;

                if (shapeName == (string)row[Consts.objectsColumn])
                {
                    if (row[valueName] != DBNull.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void ClearNullShapes(string valueName, DataTable dt)
        {
            foreach (Shape sh in this.shapes)
            {
                if (sh["NAME"] == DBNull.Value)
                    continue;

                if (IsNullShape((string)sh["NAME"], valueName, dt))
                {
                    sh.ParentGroup = sh.Layer;
                }
            }
        }

        private void ClearNullShapesFormat(string valueName, DataTable dt)
        {
            string currShapeCaption = Element.GetShapeFieldCaption();
            foreach (Shape sh in this.shapes)
            {
                if (sh[currShapeCaption] == null)
                    continue;


                if (IsNullShape(sh.Name, valueName, dt))
                {
                    sh.Text = GetDisplayEmptyShapeName(sh) ? "#" + currShapeCaption + " " : "";
                    sh.ToolTip = "#NAME ";
                }
                else
                {
                    sh.Text = "#" + currShapeCaption;
                    sh.ToolTip = "";
                }
            }
            this.Element.SetFillShapesByDefault();
        }

        private bool GetDisplayEmptyShapeName(Shape shape)
        {
            Layer layer = (Layer)this.Element.Map.Layers.GetByName(shape.Layer);
            if((layer != null)&&(layer.Tag != null))
            {
                return (bool) layer.Tag;
            }
            return true;
        }

        public void SetSymbolRule(SymbolRule rule)
        {
            this.SymbolRules.Add(rule);
        }

        public bool IsCalculateTotal(string totalName)
        {
            foreach (PivotTotal total in Element.PivotData.TotalAxis.Totals)
            {
                if (total.Caption == totalName)
                {
                    return total.IsCalculate;
                }
            }
            return false;
        }

        public void Add(MapSerie serie)
        {
            this.items.Add(serie);
        }

        public void AddRange(List<MapSerie> serieCollection)
        {
            this.items.AddRange(serieCollection);
        }

        public void Clear()
        {
            this.items.Clear();
        }

        private bool SerieExist(MapSerie serie)
        {
            return serie.Name == this.currSerie ? true : false;
        }

        public void InitRules()
        {
            foreach(MapSerie mapSerie in this.Items)
            {
                foreach (SerieRule serieRule in mapSerie.SerieRules)
                {
                    serieRule.InitRule();
                }
            }
        }

        /// <summary>
        /// Если нужно, устанавливаем пропорциональный размер значков
        /// </summary>
        public void SetProportionalSymbolSize()
        {
            this.SetProportionalSymbolSize(this.isProportionalSymbolSize);
        }

        /// <summary>
        /// Получение правила для заливки, если есть
        /// </summary>
        /// <returns></returns>
        private SerieRule GetFillSerieRule()
        {
            foreach (MapSerie mapSerie in this.Items)
            {
                foreach (SerieRule serieRule in mapSerie.SerieRules)
                {
                    if (serieRule.IsFillMap)
                    {
                        return serieRule;
                    }
                }
            }
            return null;
        }

        private void SetProportionalSymbolSize(bool value)
        {
            if ((!this.isProportionalSymbolSize)&&(!value))
                return;
            SerieRule fillSerieRule = GetFillSerieRule();

            double maxValue = MapHelper.GetMaximumRangeLimit(this.Element.SourceDS, this.NotProportionalMeasures, fillSerieRule);
            double minValue = MapHelper.GetMinimumRangeLimit(this.Element.SourceDS, this.NotProportionalMeasures, fillSerieRule);


            foreach (MapSerie mapSerie in this.Items)
            {
                if(mapSerie.ShowCharts)
                    continue;

                foreach(SerieRule serieRule in mapSerie.SerieRules)
                {
                    if ((!serieRule.IsFillMap) && (!this.NotProportionalMeasures.Contains(serieRule.Name)))
                    {
                        serieRule.SetProportionalSizeSymbols(value, minValue, maxValue);
                    }
                }
            }
        }

        public void AddNotPropMeasuresNode(XmlNode root)
        {
            XmlNode measuresNode = XmlHelper.AddChildNode(root, Consts.notProportionalMeasures, "", null);
            foreach (string measure in this.NotProportionalMeasures)
            {
                XmlHelper.AddChildNode(measuresNode, "measure", new string[2] { "name", measure });
            }
        }

        public void LoadNotPropMeasuresNode(XmlNode value)
        {
            if (value == null)
            {
                return;
            }
            XmlNodeList measureNodeList = value.SelectNodes("measure");
            if (measureNodeList == null)
            {
                return;
            }

            foreach (XmlNode measureNode in measureNodeList)
            {
                this.NotProportionalMeasures.Add(XmlHelper.GetStringAttrValue(measureNode, "name", ""));
            }

        }

        public void AddPieChartsNode(XmlNode root)
        {
            ColorConverter colorConverter = new ColorConverter();

            XmlNode pieChartsNode = XmlHelper.AddChildNode(root, Consts.mapPieCharts, "", null);
            foreach (MapSerie mapSerie in this.items)
            {
                if (!mapSerie.ShowCharts)
                {
                    continue;
                }
                XmlNode pieChartNode = XmlHelper.AddChildNode(pieChartsNode, "pieChart", 
                                                            new string[2] { "name", mapSerie.Name },
                                                            new string[2] { "baseType", mapSerie.PieChartBaseType.ToString() },
                                                            new string[2] { "size", mapSerie.PieChartSize.ToString() },
                                                            new string[2] { "maxSize", mapSerie.MaxPieChartSize.ToString() },
                                                            new string[2] { "minSize", mapSerie.MinPieChartSize.ToString() },
                                                            new string[2] { "customBase", mapSerie.CustomBase.ToString() }
                                                            );
                foreach(SerieRule serieRule in mapSerie.SerieRules)
                {
                    XmlHelper.AddChildNode(pieChartNode, "rule",
                                                                new string[2] { "name", serieRule.Name },
                                                                new string[2] { "color", colorConverter.ConvertToString(serieRule.PieChartColor) }
                                                                );

                }
            }
        }

        public void LoadPieChartsXml(XmlNode value)
        {
            if (value == null)
            {
                return;
            }
            XmlNodeList pieChartNodeList = value.SelectNodes("pieChart");

            if (pieChartNodeList == null)
            {
                return;
            }

            foreach (XmlNode pieChartNode in pieChartNodeList)
            {
                string serieName = XmlHelper.GetStringAttrValue(pieChartNode, "name", "");
                /*MapSerie pieChartSerie = this[serieName];
                /
                if (pieChartSerie == null)
                {
                    continue;
                }*/

                MapSerie pieChartSerie = new MapSerie(this, serieName);

                pieChartSerie.IsBindOffsetWithSize = false;
                pieChartSerie.SetOldPieChartSymbols(this.Element.Map.Symbols);

                pieChartSerie.PieChartBaseType = (PieChartBase)Enum.Parse(typeof(PieChartBase),
                                        XmlHelper.GetStringAttrValue(pieChartNode, "baseType", "Sum"));
                pieChartSerie.PieChartSize = XmlHelper.GetIntAttrValue(pieChartNode, "size", 30);
                pieChartSerie.MaxPieChartSize = XmlHelper.GetIntAttrValue(pieChartNode, "maxSize", 50);
                pieChartSerie.MinPieChartSize = XmlHelper.GetIntAttrValue(pieChartNode, "minSize", 20);
                pieChartSerie.CustomBase = XmlHelper.GetFloatAttrValue(pieChartNode, "customBase", 1000);


                XmlNodeList ruleNodeList = pieChartNode.SelectNodes("rule");
                if (ruleNodeList == null)
                {
                    continue;
                }
                foreach(XmlNode ruleNode in ruleNodeList)
                {
                    string ruleName = XmlHelper.GetStringAttrValue(ruleNode, "name", "");
                    /*
                    SerieRule serieRule = pieChartSerie.GetSerieRule(ruleName);
                    if (serieRule == null)
                    {
                        continue;
                    }
                    */

                    SerieRule serieRule = new SerieRule(pieChartSerie, ruleName, MapHelper.GetRandomColor());
                    pieChartSerie.SerieRules.Add(serieRule);

                    if (serieRule.Symbols != null)
                    {
                        serieRule.Symbols.Clear();
                    }

                    ColorConverter colorConverter = new ColorConverter();
                    string pieColor = XmlHelper.GetStringAttrValue(ruleNode, "color", string.Empty);
                    if (pieColor != string.Empty)
                    {
                        serieRule.PieChartColor = (Color)colorConverter.ConvertFromString(pieColor);
                    }
                    
                }
                pieChartSerie.ShowCharts = true;
                pieChartSerie.IsBindOffsetWithSize = true;

                //PieChartSeries.Add(pieChartSerie);
                //pieChartSerie.ShowCharts = true;
                
            }
        }

        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public override string ToString()
        {
            return "";
        }
    }

}
