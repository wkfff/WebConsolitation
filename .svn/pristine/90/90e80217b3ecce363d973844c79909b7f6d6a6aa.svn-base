using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class Form2pVarGridControl : Control
    {
        private const string StoreId = "dsForm2pVar";
        private const string GridId = "gpForm2pVar";

        private readonly IForecastForma2pVarRepository variantRepository;
        ////private readonly IForecastExtension extension;

        private UserFormsControls ufc;
        
        private int year;
      
        public Form2pVarGridControl(IForecastForma2pVarRepository variantRepository)
        {
            this.variantRepository = variantRepository;
            ////this.extension = extension;

        /*    ufc = extension.Forms[key];

            var obj = ufc.GetObject("year");
            if (obj != null)
            {
                year = Convert.ToInt32(obj);
            }*/
        }
        
        public Store Store { get; private set; }        

        public override List<Component> Build(ViewPage page)
        {
            Store = CreateStore();
            page.Controls.Add(Store);

            Store regionStore = CreateRegionStore();
            page.Controls.Add(regionStore);

            Store variantStore = CreateVariantStore();
            page.Controls.Add(variantStore);

            Store varForm2pStore = CreateVarForm2pStore();
            page.Controls.Add(varForm2pStore);

            GridPanel gp = new GridPanel
            {
                ID = GridId,
                StoreID = StoreId,
                MonitorResize = true,
                AutoScroll = true,
                ////Width = 500,
                AutoHeight = true,
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true
            };

            CommandColumn cmdColumn = new CommandColumn { Width = 25, Hideable = false };
            var command = new GridCommand { CommandName = "edit", Icon = Icon.ApplicationFormEdit };
            command.ToolTip.Text = "Редактировать";
            cmdColumn.Commands.Add(command);
            gp.ColumnModel.Columns.Add(cmdColumn);

            gp.Listeners.Command.AddAfter(@"var tab = parent.MdiTab.getComponent('form2pForm_'+record.id);
if (!tab) 
{
    parent.MdiTab.addTab({   
        id: 'from2pForm_'+record.id,
        title: 'Форма-2п', 
        url: '/Form2pValues/ShowExist/'+record.id,
        passParentSize: false        
    });
}
else
{
    parent.MdiTab.setActiveTab(tab);
}");

            gp.Listeners.CellClick.AddAfter(String.Format("if ({0}.getSelectionModel().hasSelection()) {{ btnExportForm2p.setDisabled(false); }} else {{  btnExportForm2p.setDisabled(true); }}", GridId));
                
            ////ColumnBase cb = gp.ColumnModel.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.NotNull);
            gp.ColumnModel.AddColumn("Name", "Name", "Наименование", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(400);
            gp.ColumnModel.AddColumn("Year", "Year", "Год", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(50);
            gp.ColumnModel.AddColumn("Forecast", "Forecast", "Вид прогноза", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(100);
            gp.ColumnModel.AddColumn("Territory", "Territory", "Территория", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(300);

            gp.SelectionModel.Add(new RowSelectionModel());
            
            return new List<Component> { gp, AddNewForm2pWindow(), ExportForm2pWindow() };
        }

        public Store CreateStore()
        {
            Store store = new Store { ID = StoreId, AutoLoad = true };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Name");
            reader.Fields.Add("Year");
            reader.Fields.Add("Forecast");
            reader.Fields.Add("Territory");

            ////reader.Fields.Add("refParam");

            store.Reader.Add(reader);
            
            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pVar/Load",
                Method = HttpMethod.POST
            });

            return store;
        }

        private static Component AddNewForm2pWindow()
        {
            Window wndAddForm2p = new Window
            {
                ID = "wndAddForm2p",
                Hidden = true,
                Width = 550,
                Height = 250,
                Layout = "FitLayout",
                Modal = true,
                Title = "Добавление новой Формы-2п",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            FormPanel formPanel = new FormPanel
            {
                ButtonAlign = Alignment.Right,
                Layout = "Column",
                ////Border = false,
                MonitorPoll = 500,
                MonitorValid = true,
                Padding = 10,
                LabelPad = -5
            };

            Panel leftPanel = new Panel 
            {
                Border = false,
                Header = false,
                ColumnWidth = 0.6,
                Layout = "form",
                LabelAlign = LabelAlign.Top,
                ////BodyCssClass = "x-window-mc",
                ////CssClass = "x-window-mc"
            };

            Panel rightPanel = new Panel
            {
                Border = false,
                Header = false,
                ColumnWidth = 0.4,
                Layout = "form",
                LabelAlign = LabelAlign.Top,
                ////BodyCssClass = "x-window-mc",
                ////CssClass = "x-window-mc",
            };

            formPanel.Items.Add(leftPanel);
            formPanel.Items.Add(rightPanel);

            TextField txtfName = new TextField
            {
                ID = "tfName",
                Name = "tfName",
                FieldLabel = "Имя Формы-2п",
                Width = 300
            };

            leftPanel.Items.Add(txtfName);

            ComboBox cboxRegion = new ComboBox
            {
                ID = "cbRegion",
                Name = "cbRegion",
                FieldLabel = "Регион",
                Width = 200,
                StoreID = "dsRegions",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                DisplayField = "Text",
                ValueField = "Value"
            };

            leftPanel.Items.Add(cboxRegion);

            SpinnerField spfYear = new SpinnerField
            {
                ID = "sfYear",
                Name = "sfYear",
                FieldLabel = "Год",
                Width = 100,
                MaxValue = 2015,
                MinValue = 2005
            };

            rightPanel.Items.Add(spfYear);

            ComboBox cboxVariant = new ComboBox
            {
                ID = "cbVariant",
                Name = "cbVariant",
                FieldLabel = "Вариант",
                Width = 200,
                StoreID = "dsVariants",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                DisplayField = "Text",
                ValueField = "Value"
            };

            rightPanel.Items.Add(cboxVariant);

            leftPanel.Defaults.Add(new Parameter("AllowBlank", "false", ParameterMode.Raw));
            leftPanel.Defaults.Add(new Parameter("MsgTarget", "false"));

            rightPanel.Defaults.Add(new Parameter("AllowBlank", "false", ParameterMode.Raw));
            rightPanel.Defaults.Add(new Parameter("MsgTarget", "false"));
            
            Button btnCancel = new Button
            {
                ID = "btnCancel",
                Text = "Отмена"
            };

            btnCancel.Listeners.Click.Handler = "wndAddForm2p.hide();";

            Button btnOk = new Button 
            {
                ID = "btnOk",
                Text = "Создать"
            };
            
            btnOk.DirectEvents.Click.Url = "/Form2pVar/NewForm2p";
            btnOk.DirectEvents.Click.CleanRequest = true;
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("name", "tfName.getValue()", ParameterMode.Raw));
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("estYear", "sfYear.getValue()", ParameterMode.Raw));
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("scenid", "cbVariant.getSelectedItem().value", ParameterMode.Raw));
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("terrid", "cbRegion.getSelectedItem().value", ParameterMode.Raw));

            btnOk.DirectEvents.Click.Success = "wndAddForm2p.hide(); {0}.reload();".FormatWith(StoreId);

            /////btnRefresh.DirectEvents.Click.Success = "panelStat.reload(); panelProg.reload();stChartData.reload();"; ////"stProgData.load();stStaticData.load();";
            
            formPanel.Buttons.Add(btnCancel);
            formPanel.Buttons.Add(btnOk);

            formPanel.BottomBar.Add(new StatusBar { Height = 25 });

            formPanel.Listeners.ClientValidation.Handler = @"
if (!valid)
{
    this.getBottomToolbar().setStatus({text : 'Не все поля заполнены!', iconCls : 'icon-exclamation'});
    btnOk.setDisabled(true)
}
else
{
    this.getBottomToolbar().setStatus({text : '', iconCls : 'icon-accept'});
    btnOk.setDisabled(false)
}
";
            ////this.getBottomToolbar().setStatus({text : valid ? '' : 'Не все поля заполнены!', iconCls: valid ? 'icon-accept' : 'icon-exclamation'});";
            
            wndAddForm2p.Items.Add(formPanel);

            return wndAddForm2p;
        }

        private Component ExportForm2pWindow()
        {
            Window wndExportForm2p = new Window
            {
                ID = "wndExportForm2p",
                Hidden = true,
                Width = 450,
                Height = 150,
                Layout = "FitLayout",
                Modal = true,
                Title = "Экспорт Формы-2п в Excel",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            FormPanel formPanel = new FormPanel
            {
                ButtonAlign = Alignment.Right,
                ////Layout = "Column",
                ////Border = false,
                MonitorPoll = 500,
                MonitorValid = true,
                Padding = 10,
                LabelPad = -5,
                LabelWidth = 200
            };

            ComboBox cboxVariant1 = new ComboBox
            {
                ID = "cboxVariant1",
                Name = "cboxVariant1",
                FieldLabel = "Форма 2п для \"Вариант 1\"",
                Width = 200,
                StoreID = "dsVarForm2p",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                DisplayField = "Text",
                ValueField = "Value"
            };

            ////cboxVariant1.Listeners. .AddBefore();
            
            ComboBox cboxVariant2 = new ComboBox
            {
                ID = "cboxVariant2",
                Name = "cboxVariant2",
                FieldLabel = "Форма 2п для \"Вариант 2\"",
                Width = 200,
                StoreID = "dsVarForm2p",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                DisplayField = "Text",
                ValueField = "Value"
            };

            formPanel.Items.Add(cboxVariant1);
            formPanel.Items.Add(cboxVariant2);
            
            Button btnExpCancel = new Button
            {
                ID = "btnExpCancel",
                Text = "Отмена"
            };

            btnExpCancel.Listeners.Click.Handler = "wndExportForm2p.hide();";

            Button btnExpOk = new Button
            {
                ID = "btnExpOk",
                Text = "Создать"
            };

            btnExpOk.DirectEvents.Click.Url = "/Form2pVar/ExcelExport";
            btnExpOk.DirectEvents.Click.CleanRequest = true;
            ////btnExportForm2p.DirectEvents.Click.EventMask.ShowMask = true;
            btnExpOk.DirectEvents.Click.IsUpload = true;
            btnExpOk.DirectEvents.Click.ExtraParams.Add(new Parameter("v1", "cboxVariant1.getSelectedItem().value", ParameterMode.Raw));
            btnExpOk.DirectEvents.Click.ExtraParams.Add(new Parameter("v2", "cboxVariant2.getSelectedItem().value", ParameterMode.Raw));
            btnExpOk.DirectEvents.Click.ExtraParams.Add(new Parameter("year", "gpForm2pVar.getSelectionModel().getSelected().data.Year", ParameterMode.Raw));

            btnExpOk.Listeners.Click.AddAfter("wndExportForm2p.hide();");

            /////btnRefresh.DirectEvents.Click.Success = "panelStat.reload(); panelProg.reload();stChartData.reload();"; ////"stProgData.load();stStaticData.load();";

            formPanel.Buttons.Add(btnExpCancel);
            formPanel.Buttons.Add(btnExpOk);

            wndExportForm2p.Items.Add(formPanel);

            return wndExportForm2p;
        }

        private Store CreateVariantStore()
        {
            Store store = new Store { ID = "dsVariants" };

            store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };
            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pVar/ComboVariantsLoad",
                Method = HttpMethod.POST
            });

            return store;
        }

        private Store CreateRegionStore()
        {
            Store store = new Store { ID = "dsRegions" };

            store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };
            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pVar/ComboRegionsLoad",
                Method = HttpMethod.POST
            });

            return store;
        }

        private Store CreateVarForm2pStore()
        {
            Store store = new Store { ID = "dsVarForm2p", AutoLoad = false };

            ////store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };
            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("selectedId", String.Format("{0}.getSelectionModel().hasSelection() ? {0}.getSelectionModel().getSelected().id : -1", GridId), ParameterMode.Raw));
            
            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pVar/ComboVarForm2pLoad",
                Method = HttpMethod.POST
            });

            store.Listeners.DataChanged.AddBefore(@"
    var v = gpForm2pVar.getSelectionModel().hasSelection() ? gpForm2pVar.getSelectionModel().getSelected().id : -1;
    if (v != -1)
    {
        var r = cboxVariant1.findRecord(cboxVariant1.valueField, v); 
        if(r)
        {
            var i = cboxVariant1.store.indexOf(r)
            cboxVariant1.selectByIndex(i);
        }
    }");
            
            return store;
        }
    }
}
