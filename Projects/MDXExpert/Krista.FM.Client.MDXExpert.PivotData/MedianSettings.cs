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
    /// Настройки для расчета медианы
    /// </summary>
    public class MedianSettings
    {
        private bool _isMedianCalculate;
        
        private bool _isLowerHigherSeparate;

        private string _medianMemberName;
        private Color _lowerMedianColor = Color.Green;
        private Color _higherMedianColor = Color.Red;

        /// <summary>
        /// Расчитывать/нет медиану
        /// </summary>
        public bool IsMedianCalculate
        {
            get { return _isMedianCalculate; }
            set { _isMedianCalculate = value; }
        }

        /// <summary>
        /// Уникальное имя элемента для медианы
        /// </summary>
        public string MedianMemberName
        {
            get { return _medianMemberName; }
            set { _medianMemberName = value; }
        }

        /// <summary>
        /// Цвет значений меньше медианы
        /// </summary>
        public Color LowerMedianColor
        {
            get { return _lowerMedianColor; }
            set { _lowerMedianColor = value; }
        }

        /// <summary>
        /// Цвет значений больше медианы
        /// </summary>
        public Color HigherMedianColor
        {
            get { return _higherMedianColor; }
            set { _higherMedianColor = value; }
        }

        /// <summary>
        /// Разделять значения на выше и ниже медианы
        /// </summary>
        public bool IsLowerHigherSeparate
        {
            get { return _isLowerHigherSeparate; }
            set { _isLowerHigherSeparate = value; }
        }


        public MedianSettings()
        {
            this._isMedianCalculate = false;
            this._medianMemberName = Guid.NewGuid().ToString();
        }



        /// <summary>
        /// Загрузка свойств
        /// </summary>
        /// <param name="nodePropertys"></param>
        public void Load(XmlNode nodePropertys)
        {
            if (nodePropertys == null)
                return;

            this.IsMedianCalculate = XmlHelper.GetBoolAttrValue(nodePropertys, Consts.isMedianCalculate, false);
            this.IsLowerHigherSeparate = XmlHelper.GetBoolAttrValue(nodePropertys, Consts.isLowerHigherSeparate, false);

            ColorConverter colorConvertor = new ColorConverter();
            string color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.lowerMedianColor, string.Empty);
            if (color != string.Empty)
            {
                this.LowerMedianColor = (Color)colorConvertor.ConvertFromString(color);
            }

            color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.higherMedianColor, string.Empty);
            if (color != string.Empty)
            {
                this.HigherMedianColor = (Color)colorConvertor.ConvertFromString(color);
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

            XmlHelper.SetAttribute(propertysNode, Consts.isMedianCalculate, this.IsMedianCalculate.ToString());
            XmlHelper.SetAttribute(propertysNode, Consts.isLowerHigherSeparate, this.IsLowerHigherSeparate.ToString());

            ColorConverter colorConvertor = new ColorConverter();

            XmlHelper.SetAttribute(propertysNode, Consts.lowerMedianColor, colorConvertor.ConvertToString(this.LowerMedianColor));
            XmlHelper.SetAttribute(propertysNode, Consts.higherMedianColor, colorConvertor.ConvertToString(this.HigherMedianColor));
        }
    }
}
