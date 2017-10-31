using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class SvodMOValuesView : View
    {
        private int varid;
        private int year;
        
        public void Initialize(int varid, int year)
        {
            this.varid = varid;
            this.year = year;
        }

        public override List<Component> Build(ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };

            SvodMOValuesGridControl valuesGrid = new SvodMOValuesGridControl(varid, year);

            layout.North.Items.Add(CreateToolBar());

            Panel panel = new Panel
            {
                AutoScroll = true,
                Border = false
            };

            panel.Items.Add(valuesGrid.Build(page));

            layout.Center.Items.Add(panel);

            /*layout.South.Items.Add(valuesGrid.ChartPanel(page));
            layout.South.Collapsible = true;*/

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "center" };
            viewport.Items.Add(layout);

            return new List<Component> { viewport };
        }

        private List<Component> CreateToolBar()
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

            Button btnSave = new Button
            {
                ID = "btnSave",
                Icon = Icon.TableSave,
                ToolTip = "Сохранит изменения"
            };

            btnSave.Listeners.Click.Handler = "dsSvodMOValues.save();";

            toolbar.Items.Add(btnSave);

            return new List<Component> { toolbarPanel };
        }
    }
}
