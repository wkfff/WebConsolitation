using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.MDXExpert
{
    public class MapSynchronization
    {
        private string boundTo;
        private bool objectsInRows = true;
        private MapReportElement mapElement;

        /// <summary>
        /// Уникальный ключ таблицы, к которой привязана карта
        /// </summary>
        public string BoundTo
        {
            get { return boundTo; }
            set
            {
                SetBoundTo(value);
                //boundTo = value;
            }
        }

        /// <summary>
        /// Брать в качестве объектов карты элементы из строк таблицы (если false - то из столбцов)
        /// </summary>
        public bool ObjectsInRows
        {
            get { return objectsInRows; }
            set { objectsInRows = value; }
        }

        public MapReportElement MapElement
        {
            get { return mapElement; }
            set { mapElement = value; }
        }

        public MapSynchronization(MapReportElement mapElement)
        {
            this.mapElement = mapElement;
        }

        private void SetBoundTo(string key)
        {
            if (key == this.BoundTo)
                return;

            //удаляем у таблицы ссылку на карту
            if (!String.IsNullOrEmpty(this.BoundTo))
            {
                TableReportElement tableElement = this.MapElement.MainForm.FindTableReportElement(this.BoundTo);
                if (tableElement != null)
                    tableElement.AnchoredElements.Remove(this.MapElement.UniqueName);
            }

            //добавляем для таблицы ссылку на карту
            if (!String.IsNullOrEmpty(key))
            {
                TableReportElement tableElement = this.MapElement.MainForm.FindTableReportElement(key);
                if (tableElement != null)
                {
                    tableElement.AnchoredElements.Add(this.MapElement.UniqueName);
                }
            }

            this.boundTo = key;
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(this.BoundTo))
            {
                return this.MapElement.MainForm.GetReportElementText(this.BoundTo);
            }
            else
            {
                return "";
            }
        }
    }

}
