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
    public class NavigationTree : Navigation
    {
        private readonly IParametersService parametersService;

        public NavigationTree(IParametersService parametersService)
        {
            this.parametersService = parametersService;
            Items = new List<NavigationItem>();
        }

        public override List<Component> Build(ViewPage page)
        {
            var panel = CreateNavigationTreePanel(this);

            BorderLayout layout = new BorderLayout { ID = "{0}BorderLayout".FormatWith(panel.ID) };
            layout.Center.Items.Add(panel);

            // Туллбар с командами
            BuildCommands(panel);

            // Нижний тулбар
            foreach (var component in ButtomBar)
            {
                layout.South.Items.Add(component);    
            }

            Panel contentPanel = new Panel 
            { 
                ID = Id,
                Title = Title,
                Icon = Icon,
                Border = false
            };
            contentPanel.Items.Add(layout);

            return new List<Component> { contentPanel };
        }

        private void FillNavigationTree(TreeNode root, IEnumerable<NavigationItem> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (NavigationItem item in items.OrderBy(x => x.OrderPosition))
            {
                var node = new TreeNode(item.Title) { NodeID = item.ID, Icon = item.Icon };
                foreach (NavigationItemParameter param in item.Params)
                {
                    node.CustomAttributes.Add(
                        new ConfigItem(
                            param.Name, 
                            String.Format("'{0}'", new Expression(parametersService).Eval(param.Value))));
                }

                FillNavigationTree(node, item.Items);
                root.Nodes.Add(node);
            }
        }

        private PanelBase CreateNavigationTreePanel(Navigation navigation)
        {
            var treePanel = new TreePanel
            {
                ID = "{0}TreePanel".FormatWith(navigation.Id),
                Border = false,
                RootVisible = false
            };

            treePanel.Listeners.Click.Handler = @"
if (node.childNodes.length > 0){
    node.toggle();
    return;
}
var url = node.attributes.url;
MdiTab.addTab({ title: node.text, url: url, icon: node.attributes.iconCls, passParentSize: false});";

            var root = new TreeNode("Root");
            treePanel.Root.Add(root);

            FillNavigationTree(root, navigation.Items);

            return treePanel;
        }
    }
}
