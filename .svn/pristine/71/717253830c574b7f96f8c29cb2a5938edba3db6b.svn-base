using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using Dundas.Maps.WinControl;
using Infragistics.UltraGauge.Resources;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Controls;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Цветовой интервал для индикатора
    /// </summary>
    public class GaugeColorRange
    {
        private double _startValue;
        private double _endValue;
        private Color _color;
        private string _text;

        public double StartValue
        {
            get { return this._startValue; }
            set { this._startValue = value; }
        }

        public double EndValue
        {
            get { return this._endValue; }
            set { this._endValue = value; }
        }

        public Color Color
        {
            get { return this._color; }
            set { this._color = value; }
        }

        public string Text
        {
            get { return this._text; }
            set { this._text = value; }
        }

        public GaugeColorRange(double startValue, double endValue, Color color, string text)
        {
            this.StartValue = startValue;
            this.EndValue = endValue;
            this.Color = color;
            this.Text = text;
        }


        /// <summary>
        /// Загрузка свойств интервала
        /// </summary>
        /// <param name="nodePropertys"></param>
        public void Load(XmlNode nodePropertys)
        {
            if (nodePropertys == null)
                return;
            this.StartValue = XmlHelper.GetFloatAttrValue(nodePropertys, Consts.startValue, 0);
            this.EndValue = XmlHelper.GetFloatAttrValue(nodePropertys, Consts.endValue, 0);
            this.Text = XmlHelper.GetStringAttrValue(nodePropertys, Consts.intervalName, String.Empty);

            ColorConverter colorConvertor = new ColorConverter();
            string color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.intervalColor, string.Empty);
            if (color != string.Empty)
            {
                this.Color = (Color)colorConvertor.ConvertFromString(color);
            }
        }

        /// <summary>
        /// Сохранение свойств интервала
        /// </summary>
        /// <param name="propertysNode"></param>
        public void Save(XmlNode propertysNode)
        {
            if (propertysNode == null)
                return;
            XmlHelper.SetAttribute(propertysNode, Consts.startValue, this.StartValue.ToString());
            XmlHelper.SetAttribute(propertysNode, Consts.endValue, this.EndValue.ToString());
            XmlHelper.SetAttribute(propertysNode, Consts.intervalName, this.Text);
            ColorConverter colorConvertor = new ColorConverter();
            XmlHelper.SetAttribute(propertysNode, Consts.intervalColor, colorConvertor.ConvertToString(this.Color));
        }

    }


    public class GaugeColorRangeCollection : List<GaugeColorRange>
    {
        public GaugeColorRange this[int index]
        {
            get
            {
                return (GaugeColorRange)base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        public GaugeColorRangeCollection()
        {

        }

        /// <summary>
        /// Инициализация по интервалам, отображаемым на идикаторе. Нужно для поддержки старых версий, 
        /// в которых настройки интервалов хранились в самом индикаторе
        /// </summary>
        /// <param name="visibleRanges"></param>
        public void InitByVisibleRanges(List<GaugeRange> visibleRanges)
        {
            this.Clear();
            foreach(GaugeRange range in visibleRanges)
            {
                GaugeColorRange colorRange = new GaugeColorRange(0, 0, Color.Empty, String.Empty);
                colorRange.StartValue = (double)range.StartValue;
                colorRange.EndValue = (double)range.EndValue;
                colorRange.Text = range.Key;

                if ((range.BrushElement != null) && (range.BrushElement is SolidFillBrushElement))
                {
                    colorRange.Color = ((SolidFillBrushElement)range.BrushElement).Color;
                }

                this.Add(colorRange);
            }
        }


        public void Save(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;

            foreach (GaugeColorRange range in this)
            {
                range.Save(XmlHelper.AddChildNode(collectionNode, Consts.colorRange));
            }
        }

        public void Load(XmlNode collectionNode)
        {
            if (collectionNode == null)
                return;

            this.Clear();
            XmlNodeList rangeNodes = collectionNode.SelectNodes(Consts.colorRange);

            foreach (XmlNode rangeNode in rangeNodes)
            {
                GaugeColorRange range = new GaugeColorRange(0, 0, Color.Empty, String.Empty);
                range.Load(rangeNode);
                this.Add(range);
            }

        }



    }

}
