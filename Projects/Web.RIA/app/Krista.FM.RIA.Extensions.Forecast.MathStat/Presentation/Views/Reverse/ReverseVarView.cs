using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ReverseVarView : View
    {
        public ReverseVarView()
        {
        }
        
        public override List<Component> Build(ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };

            Panel panel = new Panel
            {
                AutoScroll = true,
                Border = false,
                Width = layout.Width,
                Height = layout.Height
            };

            ReverseVarGridControl reverseGrid = new ReverseVarGridControl();

            panel.Items.Add(reverseGrid.Build(page));

            layout.North.Items.Add(CreateToolBar());

            layout.Center.Items.Add(panel);
            
            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "center" };
            viewport.Items.Add(layout);
            
            return new List<Component> { viewport };
        }

        private Component CreateToolBar()
        {
            FormPanel toolbarPanel = new FormPanel
            {
                Border = false,
                ////AutoHeight = true,
                Height = 0,
                ////Width = 400,
                Collapsible = false,
                LabelWidth = 125,
                LabelAlign = LabelAlign.Right,
                Padding = 5,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Layout = "form"
            };

            Toolbar toolbar = new Toolbar
            {
                ID = "toolbar"
            };

            toolbarPanel.TopBar.Add(toolbar);

            Button btnAddNew = new Button
            {
                ID = "btnAddNew",
                Text = "Добавить обратный прогноз",
                Icon = Icon.Add
            };

            btnAddNew.Listeners.Click.Handler = "wndAddReverse.show();";

            toolbar.Items.Add(btnAddNew);

            return toolbarPanel;
        }
    }
}
