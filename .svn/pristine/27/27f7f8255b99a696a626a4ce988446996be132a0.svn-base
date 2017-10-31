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
    /// Настройки для расчета среднего значения
    /// </summary>
    public class AverageSettings
    {
        private AverageType _averageType;
        private bool _isLowerHigherAverageSeparate;
        private Color _lowerAverageColor = Color.LightGreen;
        private Color _higherAverageColor = Color.Pink;
        private bool _isStandartDeviationCalculate;
        private Color _lowerDeviationColor = Color.Green;
        private Color _higherDeviationColor = Color.Red;
        private string _averageMemberName;
        private string _standartDeviationName;
        private bool _isAverageDeviationCalculate;

        /// <summary>
        /// Тип среднего значения
        /// </summary>
        public AverageType AverageType
        {
            get
            {
                return _averageType;
            }
            set { _averageType = value; }
        }

        /// <summary>
        /// Разделять на "выше" и "ниже" среднего значения (показывать цветом)
        /// </summary>
        public bool IsLowerHigherAverageSeparate
        {
            get { return _isLowerHigherAverageSeparate; }
            set { _isLowerHigherAverageSeparate = value; }
        }

        /// <summary>
        /// Рассчитывать или нет обычное отклонение от среднего (в таблице будет выводится отдельная мера со значениями)
        /// </summary>
        public bool IsAverageDeviationCalculate
        {
            get { return this._isAverageDeviationCalculate; }
            set { this._isAverageDeviationCalculate = value; }
        }

        /// <summary>
        /// Цвет, соответствующий значению ниже среднего значения
        /// </summary>
        public Color LowerAverageColor
        {
            get { return _lowerAverageColor; }
            set { _lowerAverageColor = value; }
        }

        /// <summary>
        /// Цвет, соответствующий значению выше среднего значения
        /// </summary>
        public Color HigherAverageColor
        {
            get { return _higherAverageColor; }
            set { _higherAverageColor = value; }
        }

        /// <summary>
        /// Расчитывать стандартное отклонение
        /// </summary>
        public bool IsStandartDeviationCalculate
        {
            get { return _isStandartDeviationCalculate; }
            set { _isStandartDeviationCalculate = value; }
        }

        /// <summary>
        /// Цвет, соответствующий значению ниже границы стандартного отклонения
        /// </summary>
        public Color LowerDeviationColor
        {
            get { return _lowerDeviationColor; }
            set { _lowerDeviationColor = value; }
        }

        /// <summary>
        /// Цвет, соответствующий значению выше границы стандартного отклонения
        /// </summary>
        public Color HigherDeviationColor
        {
            get { return _higherDeviationColor; }
            set { _higherDeviationColor = value; }
        }

        /// <summary>
        /// Уникальное имя для элемента среднее значение
        /// </summary>
        public string AverageMemberName
        {
            get { return _averageMemberName; }
        }

        /// <summary>
        /// Уникальное имя для элемента стандартное отклонение
        /// </summary>
        public string StandartDeviationName
        {
            get { return _standartDeviationName; }
        }

        public AverageSettings()
        {
            this._averageType = AverageType.None;
            this._averageMemberName = Guid.NewGuid().ToString();
            this._standartDeviationName = Guid.NewGuid().ToString();
        }


        /// <summary>
        /// Загрузка свойств
        /// </summary>
        /// <param name="nodePropertys"></param>
        public void Load(XmlNode nodePropertys)
        {
            if (nodePropertys == null)
                return;
            this.AverageType = (AverageType)Enum.Parse(typeof(AverageType),
                XmlHelper.GetStringAttrValue(nodePropertys, Consts.averageType, "None"));

            this.IsLowerHigherAverageSeparate = XmlHelper.GetBoolAttrValue(nodePropertys, Consts.isAverageSeparate, false);
            this.IsAverageDeviationCalculate = XmlHelper.GetBoolAttrValue(nodePropertys, Consts.isAverageDeviationCalculate, false);

            ColorConverter colorConvertor = new ColorConverter();
            string color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.lowerAverageColor, string.Empty);
            if (color != string.Empty)
            {
                this.LowerAverageColor = (Color)colorConvertor.ConvertFromString(color);
            }

            color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.higherAverageColor, string.Empty);
            if (color != string.Empty)
            {
                this.HigherAverageColor = (Color)colorConvertor.ConvertFromString(color);
            }

            this.IsStandartDeviationCalculate = XmlHelper.GetBoolAttrValue(nodePropertys, Consts.isStdDevCalculate, false);
            
            color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.lowerDeviationColor, string.Empty);
            if (color != string.Empty)
            {
                this.LowerDeviationColor = (Color)colorConvertor.ConvertFromString(color);
            }

            color = XmlHelper.GetStringAttrValue(nodePropertys, Consts.higherDeviationColor, string.Empty);
            if (color != string.Empty)
            {
                this.HigherDeviationColor = (Color)colorConvertor.ConvertFromString(color);
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

            XmlHelper.SetAttribute(propertysNode, Consts.averageType, this.AverageType.ToString());

            XmlHelper.SetAttribute(propertysNode, Consts.isAverageSeparate, this.IsLowerHigherAverageSeparate.ToString());
            XmlHelper.SetAttribute(propertysNode, Consts.isAverageDeviationCalculate, this.IsAverageDeviationCalculate.ToString());

            ColorConverter colorConvertor = new ColorConverter();

            XmlHelper.SetAttribute(propertysNode, Consts.lowerAverageColor, colorConvertor.ConvertToString(this.LowerAverageColor));
            XmlHelper.SetAttribute(propertysNode, Consts.higherAverageColor, colorConvertor.ConvertToString(this.HigherAverageColor));

            XmlHelper.SetAttribute(propertysNode, Consts.isStdDevCalculate, this.IsStandartDeviationCalculate.ToString());

            XmlHelper.SetAttribute(propertysNode, Consts.lowerDeviationColor, colorConvertor.ConvertToString(this.LowerDeviationColor));
            XmlHelper.SetAttribute(propertysNode, Consts.higherDeviationColor, colorConvertor.ConvertToString(this.HigherDeviationColor));
        }
    }
}
