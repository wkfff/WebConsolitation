using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.ExtensionModule;
using Krista.FM.RIA.Core.Extensions;

namespace Krista.FM.RIA.Core.Gui
{
    public class NavigationList : Navigation
    {
        private readonly IParametersService parametersService;

        public NavigationList(IParametersService parametersService)
        {
            this.parametersService = parametersService;
        }

        public override List<Component> Build(ViewPage page)
        {
            return new List<Component> { CreateNavigationMenuPanel(this) };
        }

        private PanelBase CreateNavigationMenuPanel(Navigation navigation)
        {
            var panel = new MenuPanel
            {
                ID = navigation.Id,
                Title = navigation.Title,
                Icon = navigation.Icon,
                SaveSelection = false,
                Border = false
            };

            foreach (NavigationItem item in navigation.Items.OrderBy(x => x.OrderPosition))
            {
                var mi = new MenuItem(item.Title) { ID = item.ID, Icon = item.Icon };
                if (item.Link.IsNotNullOrEmpty())
                {
                    mi.CustomConfig.Add(new ConfigItem("url", item.Link, ParameterMode.Value));
                }

                foreach (NavigationItemParameter param in item.Params)
                {
                    mi.CustomConfig.Add(
                        new ConfigItem(
                            param.Name,
                            String.Format("'{0}'", new Expression(parametersService).Eval(param.Value))));
                }

                panel.Menu.Items.Add(mi);
            }

            panel.Menu.Listeners.Click.Handler = Action ?? DefaultAction;
            return panel;
        }
    }
}
