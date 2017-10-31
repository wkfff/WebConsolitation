using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class SvodMOVarGridControl : Control
    {
        private const string StoreId = "dsSvodMOVar";
        private const string GridId = "gpSvodMOVar";

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateStore());
            page.Controls.Add(CreateTerritoryStore());

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
        title: 'Варианты прогнозов показателей МО', 
        url: '/SvodMOValues/ShowExist/'+record.id,
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
            gp.ColumnModel.AddColumn("Territory", "Territory", "Территория", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(300);
            gp.ColumnModel.AddColumn("Year", "Year", "Год", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(50);
            
            gp.SelectionModel.Add(new RowSelectionModel());

            return new List<Component> { gp, AddNewVarWindow() };
        }

        public Component AddNewVarWindow()
        {
            Window wndAddNew = new Window
            {
                ID = "wndAddNewVar",
                Hidden = true,
                Width = 400,
                Height = 250,
                Layout = "FitLayout",
                Modal = true,
                Title = "Создание нового варианта",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            FormPanel formPanel = new FormPanel
            {
                ButtonAlign = Alignment.Right,
                Layout = "fit",
                ////Border = false,
                MonitorPoll = 500,
                MonitorValid = true,
                Padding = 10,
                LabelPad = -5
            };

            wndAddNew.Items.Add(formPanel);

            Panel singlePanel = new Panel
            {
                Border = false,
                Header = false,
                ColumnWidth = 0.6,
                Layout = "form",
                LabelAlign = LabelAlign.Top,
                ////BodyCssClass = "x-window-mc",
                ////CssClass = "x-window-mc"
            };

            singlePanel.Defaults.Add(new Parameter("AllowBlank", "false", ParameterMode.Raw));
            singlePanel.Defaults.Add(new Parameter("MsgTarget", "false"));

            formPanel.Items.Add(singlePanel);

            TextField txtName = new TextField
            {
                ID = "txtName",
                Name = "txtName",
                FieldLabel = "Наименование варианта",
                Width = 300
            };

            singlePanel.Items.Add(txtName);

            SpinnerField spfYear = new SpinnerField
            {
                ID = "sfYear",
                Name = "sfYear",
                FieldLabel = "Год",
                Width = 100,
                MaxValue = 2015,
                MinValue = 2005
            };

            singlePanel.Items.Add(spfYear);

            ComboBox cboxTerritory = new ComboBox
            {
                ID = "cboxTerritory",
                Width = 300,
                FieldLabel = "Выбирете территорию",
                StoreID = "dsTerritory",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                DisplayField = "Text",
                ValueField = "Value",
                ////PageSize = 10
            };

            singlePanel.Items.Add(cboxTerritory);

            Button btnCancel = new Button
            {
                ID = "btnCancel",
                Text = "Отмена"
            };

            btnCancel.Listeners.Click.Handler = "wndAddNewVar.hide();";

            Button btnOk = new Button
            {
                ID = "btnOk",
                Text = "Создать"
            };

            btnOk.DirectEvents.Click.Url = "/SvodMOVar/NewVar";
            btnOk.DirectEvents.Click.CleanRequest = true;
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("name", "txtName.getValue()", ParameterMode.Raw));
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("year", "sfYear.getValue()", ParameterMode.Raw));
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("territory", "cboxTerritory.getSelectedItem().value", ParameterMode.Raw));

            btnOk.DirectEvents.Click.Success = "wndAddNewVar.hide(); {0}.reload();".FormatWith(StoreId);

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

            return wndAddNew;
        }

        public Store CreateStore()
        {
            Store store = new Store { ID = StoreId, AutoLoad = true };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Name");
            reader.Fields.Add("Territory");
            reader.Fields.Add("Year");

            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/SvodMOVar/Load",
                Method = HttpMethod.POST
            });

            return store;
        }

        public Store CreateTerritoryStore()
        {
            Store store = new Store { ID = "dsTerritory", AutoLoad = true };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Values" };
            reader.Fields.Add("Text");
            reader.Fields.Add("Value");

            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/SvodMOVar/LoadTerritory",
                Method = HttpMethod.POST
            });

            return store;
        }
    }
}