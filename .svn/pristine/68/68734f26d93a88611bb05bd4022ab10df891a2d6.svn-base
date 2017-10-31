using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Data
{
    /// <summary>
    /// Настройки для расчета k-первых
    /// </summary>
    public class TopCountSettings
    {
        private bool _isTopCountCalculate;

        private int _topCount;
        private string _medianMemberName;
        private Color _topCountColor = Color.Orange;

        /// <summary>
        /// Расчитывать/нет к-первых элементов
        /// </summary>
        public bool IsTopCountCalculate
        {
            get { return _isTopCountCalculate; }
            set { _isTopCountCalculate = value; }
        }

        /// <summary>
        /// Цвет значений
        /// </summary>
        public Color TopCountColor
        {
            get { return _topCountColor; }
            set { _topCountColor = value; }
        }

        /// <summary>
        /// Количество первых элементов
        /// </summary>
        public int TopCount
        {
            get { return _topCount; }
            set { _topCount = value; }
        }


        public TopCountSettings()
        {
            this._isTopCountCalculate = false;
            this._topCount = 5;
        }

        /// <summary>
        /// Получение имени для меры, определяющей k-первых элементов
        /// </summary>
        /// <param name="measureSource">мера, по которой будет считаться k-первых</param>
        /// <returns></returns>
        public string GetTopCountMeasureName(string measureSource)
        {
            string sourceName = PivotData.GetNameFromUniqueName(measureSource);
            return String.Format("[Measures].[TopCount({0})]", sourceName);
        }
        

        /// <summary>
        /// Загрузка свойств
        /// </summary>
        /// <param name="nodePropertys"></param>
        public void Load(XmlNode nodePropertys)
        {
            if (nodePropertys == null)
                return;

            this.IsTopCountCalculate = XmlHelper.GetBoolAttrValue(nodePropertys, Consts.isTopCountCalculate, false);
            this.TopCount = XmlHelper.GetIntAttrValue(nodePropertys, Consts.topCount, 0);

            ColorConverter colorConvertor = new ColorConverter();
            string color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.topCountColor, string.Empty);
            if (color != string.Empty)
            {
                this.TopCountColor = (Color)colorConvertor.ConvertFromString(color);
            }

        }

        /// <summary>
        /// Сохранение свойств
        /// </summary>
        /// <param name="propertysNode"></param>
        public void Save(XmlNode propertysNode)
        {
            if (propertysNode == null)
                return;

            XmlHelper.SetAttribute(propertysNode, Consts.isTopCountCalculate, this.IsTopCountCalculate.ToString());
            XmlHelper.SetAttribute(propertysNode, Consts.topCount, this.TopCount.ToString());

            ColorConverter colorConvertor = new ColorConverter();
            XmlHelper.SetAttribute(propertysNode, Consts.topCountColor, colorConvertor.ConvertToString(this.TopCountColor));
        }
         
    }
}
