using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.OLAPStructures
{
    /// <summary>
    /// Класс для инициализации контекстного меню объекта
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class MenuActionAttribute : Attribute
    {
        private string caption = string.Empty;
        private int imageIndex;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="caption"></param>
        public MenuActionAttribute(string caption)
        {
            this.caption = caption;
        }

        public int ImageIndex
        {
            get { return imageIndex; }
            set { imageIndex = value; }
        }

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
    }
}
