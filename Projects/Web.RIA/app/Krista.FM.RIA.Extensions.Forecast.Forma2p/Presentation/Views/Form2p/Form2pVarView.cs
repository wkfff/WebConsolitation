using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class Form2pVarView : View
    {
        private readonly IForecastForma2pVarRepository variantRepository;

        public Form2pVarView(IForecastForma2pVarRepository variantRepository)
        {
            this.variantRepository = variantRepository;
        }

        public override List<Component> Build(ViewPage page)
        {
            BorderLayout layout = new BorderLayout { ID = "borderLayoutMain" };
            
            Form2pVarGridControl variantsGrid = new Form2pVarGridControl(variantRepository);
            
            layout.North.Items.Add(CreateToolBar());

            Panel panel = new Panel
            {
                AutoScroll = true,
                Border = false,
                ////AutoWidth = true,
                Width = layout.Width,
                Height = layout.Height
                ////AutoHeight = true
            };

            panel.Items.Add(variantsGrid.Build(page));

            layout.Center.Items.Add(panel);
            
            ////((RowSelectionModel)paramsGrid.GridPanel.SelectionModel[0]).Listeners.RowSelect.Handler = "dsUsers.reload()";

            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "center" };
            viewport.Items.Add(layout);

            ////paramsGrid.Store.DataBinding += StoreDataBinding;

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

            Button createNew = new Button
            {
                ID = "btnCreateNew",
                Icon = Icon.New,
                ToolTip = "Создать новую Форму-2п"
            };

            createNew.Listeners.Click.Handler = "wndAddForm2p.show();";

            toolbar.Items.Add(createNew);

            Button btnExportForm2p = new Button
            {
                ID = "btnExportForm2p",
                Icon = Icon.PageExcel,
                ToolTip = "Экспорт в Excel",
                Enabled = false
            };

            toolbar.AddScript("btnExportForm2p.setDisabled(true);");

            btnExportForm2p.Listeners.Click.Handler = @"
    wndExportForm2p.show(); 
    dsVarForm2p.load(); 
";

            ////
            
            /*btnExportForm2p.DirectEvents.Click.Url = "/Form2pVar/ExcelExport";
            btnExportForm2p.DirectEvents.Click.CleanRequest = true;
            ////btnExportForm2p.DirectEvents.Click.EventMask.ShowMask = true;
            btnExportForm2p.DirectEvents.Click.IsUpload = true;
            btnExportForm2p.DirectEvents.Click.ExtraParams.Add(new Parameter("v1", "64", ParameterMode.Value));
            btnExportForm2p.DirectEvents.Click.ExtraParams.Add(new Parameter("v2", "84", ParameterMode.Value));
            btnExportForm2p.DirectEvents.Click.ExtraParams.Add(new Parameter("year", "2008", ParameterMode.Value));*/

            toolbar.Items.Add(btnExportForm2p);
            
            return new List<Component> { toolbarPanel };
        }
    }
}
