using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class Form2pValuesView : View
    {
        private string key;

        private int year;
        private int varId;

        private IForecastExtension extension;

        public Form2pValuesView(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public void Initialize(string key)
        {
            this.key = key;

            var ufc = this.extension.Forms[key];

            var obj = ufc.GetObject("year");
            if (obj != null)
            {
                year = Convert.ToInt32(obj);
            }

            obj = ufc.GetObject("varId");
            if (obj != null)
            {
                varId = Convert.ToInt32(obj);
            }
        }

        public override List<Component> Build(ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };

            Form2pValuesGridControl valuesGrid = new Form2pValuesGridControl(key, extension);

            layout.North.Items.Add(CreateToolBar());
            
            Panel panel = new Panel
            {
                AutoScroll = true,
                Border = false
            };

            panel.Items.Add(valuesGrid.Build(page));

            layout.Center.Items.Add(panel);

            layout.South.Items.Add(valuesGrid.ChartPanel(page));
            layout.South.Collapsible = true;
            
            ////layout.li
            
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

            Button btnCreateNew = new Button
            {
                ID = "btnSaveChanged",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить изменения",
                Enabled = false
            };

            btnCreateNew.Listeners.Click.Handler = "dsForm2pValues.save();";
            toolbar.Items.Add(btnCreateNew);
            
            Button btnLoadFromRegs = new Button
            {
                ID = "btnInsertRegs",
                Icon = Icon.TableRowInsert,
                ToolTip = "Заполнить регуляторы",
                Enabled = true
            };

            btnLoadFromRegs.DirectEvents.Click.Url = "/Form2pValues/InsertRegs";
            btnLoadFromRegs.DirectEvents.Click.ExtraParams.Add(new Parameter("varid", varId.ToString()));
            btnLoadFromRegs.DirectEvents.Click.ExtraParams.Add(new Parameter("baseYear", year.ToString()));
            btnLoadFromRegs.DirectEvents.Click.IsUpload = false;
            
            toolbar.Items.Add(btnLoadFromRegs);

            Button btnFillFromScen = new Button
            {
                ID = "btnFillFromScen",
                Icon = Icon.CalculatorLink,
                ToolTip = "Вставить из сценария",
                Enabled = true
            };

            btnFillFromScen.Listeners.Click.Handler = "wndFillFromScen.show();";

            toolbar.Items.Add(btnFillFromScen);

            /*btnFillFromScen.DirectEvents.Click.Url = "/Form2pValues/InsertFromScen";
            btnFillFromScen.DirectEvents.Click.ExtraParams.Add(new Parameter("varid", varId.ToString(), ParameterMode.Value));
            btnLoadFromRegs.DirectEvents.Click.IsUpload = false;*/

            return new List<Component> { toolbarPanel };
        }
    }
}
