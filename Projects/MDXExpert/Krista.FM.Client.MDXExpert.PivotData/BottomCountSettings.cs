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
    /// Настройки для расчета k-последних
    /// </summary>
    public class BottomCountSettings
    {
        private bool _isBottomCountCalculate;

        private int _bottomCount;
        private Color _bottomCountColor = Color.Yellow;

        /// <summary>
        /// Расчитывать/нет к-последних элементов
        /// </summary>
        public bool IsBottomCountCalculate
        {
            get { return _isBottomCountCalculate; }
            set { _isBottomCountCalculate = value; }
        }

        /// <summary>
        /// Цвет значений
        /// </summary>
        public Color BottomCountColor
        {
            get { return _bottomCountColor; }
            set { _bottomCountColor = value; }
        }

        /// <summary>
        /// Количество последних элементов
        /// </summary>
        public int BottomCount
        {
            get { return _bottomCount; }
            set { _bottomCount = value; }
        }


        public BottomCountSettings()
        {
            this._isBottomCountCalculate = false;
            this._bottomCount = 5;
        }

        /// <summary>
        /// Получение имени для меры, определяющей k-последних элементов
        /// </summary>
        /// <param name="measureSource">мера, по которой будет считаться k-последних</param>
        /// <returns></returns>
        public string GetBottomCountMeasureName(string measureSource)
        {
            string sourceName = PivotData.GetNameFromUniqueName(measureSource);
            return String.Format("[Measures].[BottomCount({0})]", sourceName);
        }
        

        /// <summary>
        /// Загрузка свойств
        /// </summary>
        /// <param name="nodePropertys"></param>
        public void Load(XmlNode nodePropertys)
        {
            if (nodePropertys == null)
                return;

            this.IsBottomCountCalculate = XmlHelper.GetBoolAttrValue(nodePropertys, Consts.isBottomCountCalculate, false);
            this.BottomCount = XmlHelper.GetIntAttrValue(nodePropertys, Consts.bottomCount, 0);

            ColorConverter colorConvertor = new ColorConverter();
            string color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.bottomCountColor, string.Empty);
            if (color != string.Empty)
            {
                this.BottomCountColor = (Color)colorConvertor.ConvertFromString(color);
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

            XmlHelper.SetAttribute(propertysNode, Consts.isBottomCountCalculate, this.IsBottomCountCalculate.ToString());
            XmlHelper.SetAttribute(propertysNode, Consts.bottomCount, this.BottomCount.ToString());

            ColorConverter colorConvertor = new ColorConverter();
            XmlHelper.SetAttribute(propertysNode, Consts.bottomCountColor, colorConvertor.ConvertToString(this.BottomCountColor));
        }
         
    }
}
