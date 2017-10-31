using System.Collections.Generic;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.ViewModel;

namespace Krista.FM.RIA.Core.ExtensionModule
{
    /// <summary>
    /// Дескриптор главгоно элемента навигации модуля расширения.
    /// </summary>
    public abstract class Navigation : Control
    {
        public readonly string DefaultAction =
            "MdiTab.addTab({ title: menuItem.text, url: menuItem.url, icon: menuItem.iconCls, passParentSize: menuItem.passParentSize});";

        protected Navigation()
        {
            ButtomBar = new List<Component>();
        }

        public string Title { get; set; }

        public Icon Icon { get; set; }
        
        public string DashboardIcon { get; set; }
        
        public NavigationType NavigationType { get; set; }
        
        public string DefaultItemId { get; set; }

        public string Action { get; set; }
        
        /// <summary>
        /// Имя группы.
        /// </summary>
        public string Group { get; set; }
        
        /// <summary>
        /// Дочерние навигационные элементы.
        /// </summary>
        public List<NavigationItem> Items { get; set; }

        /// <summary>
        /// Команды навигационной области.
        /// </summary>
        public List<ActionDescriptor> Commands { get; set; }

        /// <summary>
        /// Элементы нижней панели навигационной области.
        /// </summary>
        public List<Component> ButtomBar { get; set; }

        /// <summary>
        /// Позиция элемента.
        /// Чем меньше значение, тем выше и левее, чем больше, тем ниже и правее.
        /// </summary>
        public int OrderPosition { get; set; }

        protected void BuildCommands(PanelBase panel)
        {
            if (Commands != null && Commands.Count > 0)
            {
                var tb = new Toolbar { ID = "tb{0}".FormatWith(Id) };

                // Команды
                foreach (var command in Commands)
                {
                    var btn = new Button
                    {
                        ID = command.Id,
                        Icon = command.Icon,
                        Handler = command.Handler
                    };
                    if (!command.Title.IsNullOrEmpty())
                    {
                        btn.ToolTips.Add(new ToolTip { Html = command.Title });
                    }

                    tb.Items.Add(btn);
                }

                panel.TopBar.Add(tb);
            }
        }

        protected void BuildButtomBar(PanelBase panel)
        {
            if (ButtomBar != null)
            {
                var toolbar = new Toolbar { ID = "tbBottom{0}".FormatWith(Id) };
                foreach (var component in ButtomBar)
                {
                    toolbar.Items.Add(component);
                }

                panel.BottomBar.Add(toolbar);
            }
        }
    }
}
