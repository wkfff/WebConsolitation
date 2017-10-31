using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert.Grid
{
    /// <summary>
    /// Масштаб таблицы
    /// </summary>
    public class GridScale
    {
        private const float _minValue = 0.3f;
        private const float _maxValue = 3;
        private float _value = 1;
        private ExpertGrid _grid;

        /// <summary>
        /// Значение коеффициента увеличения
        /// </summary>
        public float Value
        {
            get { return _value; }
            set
            {
                if (value < _minValue)
                    value = _minValue;
                if (value > _maxValue)
                    value = _maxValue;
                
                if (value != this._value)
                {
                    this._value = value;
                    this._grid.RecalculateGrid();
                    this.DoScaleChange();
                }
            }
        }

        public int PercentValue
        {
            get { return (int) Math.Round(this.Value*100); }
            set { this.Value = (float)value/100; }
        }

        public GridScale(ExpertGrid grid)
        {
            this._grid = grid;
        }

        /// <summary>
        /// Получить новое значение с учетом коеф. увеличения
        /// </summary>
        /// <param name="initialValue">исходное значение</param>
        /// <returns></returns>
        public int GetScaledValue(int initialValue)
        {
            return (int) Math.Round(initialValue*this.Value);
        }

        /// <summary>
        /// Получить новое значение с учетом коеф. увеличения
        /// </summary>
        /// <param name="initialValue">исходное значение</param>
        /// <returns></returns>
        public float GetScaledValue(float initialValue)
        {
            return (float)Math.Round(initialValue * this.Value);
        }

        /// <summary>
        /// Получить исходное значение без учета коеф. увеличения
        /// </summary>
        /// <param name="scaledValue">увеличенное значение</param>
        /// <returns></returns>
        public int GetNonScaledValue(int scaledValue)
        {
            return (int)Math.Round(scaledValue / this.Value);
        }

        /// <summary>
        /// Получить исходное значение без учета коеф. увеличения
        /// </summary>
        /// <param name="scaledValue">увеличенное значение</param>
        /// <returns></returns>
        public float GetNonScaledValue(float scaledValue)
        {
            return (float)Math.Round(scaledValue / this.Value);
        }

        public void Save(XmlNode scaleNode)
        {
            XmlHelper.SetAttribute(scaleNode, GridConsts.scaleValue, this.Value.ToString());
        }

        /// <summary>
        /// вызвать событие изменения масштаба
        /// </summary>
        public void DoScaleChange()
        {
            if (this._grid != null)
            {
                this._grid.OnScaleChanged();
            }
        }


        public void Load(XmlNode scaleNode)
        {
            if (scaleNode == null)
                return;
            this.Value = XmlHelper.GetFloatAttrValue(scaleNode, GridConsts.scaleValue, 1);
        }
    }
    
}
