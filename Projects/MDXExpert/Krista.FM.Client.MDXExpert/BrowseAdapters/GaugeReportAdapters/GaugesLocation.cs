using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Controls;
using Krista.FM.Common.Xml;

namespace Krista.FM.Client.MDXExpert
{
    public delegate void LocationChangedEventHandler(bool isColumnsChanged);

    public class GaugesLocation
    {
        private bool _isAutoLocation;
        private int _maxCount;
        private int _columns;
        private int _rows;
        private MultipleGaugeReportElement _element;
        private event LocationChangedEventHandler _changed;

        public bool IsAutoLocation
        {
            get { return _isAutoLocation; }
            set
            {
                SetAutoLocation(value);
                DoChanged(true);
            }
        }

        public int MaxCount
        {
            get { return this._maxCount; }
            set
            {
                this._maxCount = value;
                this._element.SourceDT = this._element.SourceDT;
            }
        }

        public int Columns
        {
            get { return this._columns; }
            set
            {
                SetColumns(value);
                DoChanged(true);
            }
        }

        public int Rows
        {
            get { return this._rows; }
            set
            {
                SetRows(value);
                DoChanged(false);
            }
        }

        public event LocationChangedEventHandler Changed
        {
            add { this._changed += value; }
            remove { this._changed = value; }
        }

        public GaugesLocation(MultipleGaugeReportElement element)
        {
            this._element = element;
            this._isAutoLocation = true;
            this._maxCount = 50;
            this._columns = 1;
            this._rows = 1;
        }

        private void DoChanged(bool isColumnsChanged)
        {
            if(this._changed != null)
            {
                this._changed(isColumnsChanged);
            }
        }

        /// <summary>
        /// Обновляем значения колонок и строк
        /// </summary>
        public void Refresh()
        {
            if (this.IsAutoLocation)
            {
                SetAutoLocation(true);
            }
        }

       

        private void SetColumns(int value)
        {
            if (this.IsAutoLocation)
                return;

            if (value <= 0)
            {
                value = 1;
            }

            if (this._element.Gauges.Count == 0)
                return;

            int gaugesCount = Math.Min(this.MaxCount, this._element.Gauges.Count);

            if (value > gaugesCount)
            {
                value = gaugesCount;
            }

            this._columns = value;
            this._rows = (gaugesCount / value);
            this._rows += (gaugesCount % value) > 0 ? 1 : 0;
        }


        private void SetRows(int value)
        {
            if (this.IsAutoLocation)
                return;

            if (value <= 0)
            {
                value = 1;
            }

            if (this._element.Gauges.Count == 0)
                return;


            int gaugesCount = Math.Min(this.MaxCount, this._element.Gauges.Count);


            if (value > gaugesCount)
            {
                value = gaugesCount;
            }

            this._rows = value;
            this._columns = gaugesCount / value;
            this._columns += (gaugesCount % value) > 0 ? 1 : 0;
        }


        private void SetAutoLocation(bool value)
        {
            if (!value)
            {
                this._isAutoLocation = value;
                return;
            }
            int count = Math.Min(this.MaxCount, this._element.Gauges.Count);
            if (count == 0)
                return;

            this._isAutoLocation = false;
            int cols = (int)Math.Round(Math.Pow(count, 0.5));
            SetColumns(cols);
            this._isAutoLocation = true;

        }


        /// <summary>
        /// Загрузка свойств
        /// </summary>
        /// <param name="nodePropertys"></param>
        public void Load(XmlNode nodePropertys)
        {
            if (nodePropertys == null)
                return;
            
            this.IsAutoLocation = XmlHelper.GetBoolAttrValue(nodePropertys, "autoLocation", true);
            this._maxCount = XmlHelper.GetIntAttrValue(nodePropertys, "maxCount", 50);
            if (!this.IsAutoLocation)
            {
                this.Columns = XmlHelper.GetIntAttrValue(nodePropertys, "columns", 2);
                this.Rows = XmlHelper.GetIntAttrValue(nodePropertys, "rows", 2);
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
            XmlHelper.SetAttribute(propertysNode, "autoLocation", this.IsAutoLocation.ToString());
            XmlHelper.SetAttribute(propertysNode, "maxCount", this.MaxCount.ToString());
            if (!this.IsAutoLocation)
            {
                XmlHelper.SetAttribute(propertysNode, "columns", this.Columns.ToString());
                XmlHelper.SetAttribute(propertysNode, "rows", this.Rows.ToString());
            }
        }


    }
}
