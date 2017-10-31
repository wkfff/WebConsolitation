using System.Collections.Generic;
using Ext.Net;

namespace Krista.FM.RIA.Core.ExtensionModule
{
    /// <summary>
    /// Дескриптор дочернего элемента навигации модуля расширения.
    /// </summary>
    public class NavigationItem
    {
        public NavigationItem()
        {
            Params = new List<NavigationItemParameter>();
        }

        public string ID { get; set; }

        public string Title { get; set; }
        
        public Icon Icon { get; set; }
        
        public string ToolTip { get; set; }
        
        public string Link { get; set; }

        /// <summary>
        /// Параменты элемента.
        /// </summary>
        public List<NavigationItemParameter> Params { get; set; }

        /// <summary>
        /// Дочерние навигационные элементы.
        /// </summary>
        public List<NavigationItem> Items { get; set; }

        /// <summary>
        /// Позиция элемента.
        /// Чем меньше значение, тем выше и левее, чем больше, тем ниже и правее.
        /// </summary>
        public int OrderPosition { get; set; }
    }
}
