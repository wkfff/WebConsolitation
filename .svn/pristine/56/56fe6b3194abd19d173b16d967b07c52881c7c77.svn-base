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

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class ValuationControl : Control
    {
        private const string StoreId = "dsValuationVars";
        private const string GridId = "gpValuationVars";

        public Store Store { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            Store = CreateStore();
            page.Controls.Add(Store);
            page.Controls.Add(CreateParentScenStore());

            GridPanel gp = new GridPanel
            {
                Collapsible = true,
                ID = GridId,
                StoreID = StoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                ////Width = 175,
                ////Height = 600,
                AutoWidth = true,
                AutoHeight = true,
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;"
            };

            /* GroupingView groupingView = new GroupingView
             {
                 ID = "GroupingView1",
                 HideGroupedColumn = true,
                 EnableNoGroups = true,
                 GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Items' : 'Item']})"
             };

             gp.View.Add(groupingView);*/

            ColumnModel cm = gp.ColumnModel;
            ////cm.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.NotNull);

            CommandColumn cmdColumn = new CommandColumn { Width = 25, Hideable = false };
            var command = new GridCommand { CommandName = "edit", Icon = Icon.ApplicationFormEdit };
            command.ToolTip.Text = "Редактировать";
            cmdColumn.Commands.Add(command);
            cm.Columns.Add(cmdColumn);

            gp.Listeners.Command.AddAfter(@"var tab = parent.MdiTab.getComponent('valuation_'+record.id);
if (!tab) 
{
    parent.MdiTab.addTab({   
        id: 'valuation_'+record.id,
        title: 'Вариант прогнозирования', 
        url: '/ValuationParam/ShowExist/'+record.id,
        passParentSize: false        
    });
}
else
{
    parent.MdiTab.setActiveTab(tab);
}");

            cm.AddColumn("Name", "Name", "Наименование", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(350);
            cm.AddColumn("Year", "Year", "Год", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(75);
            ////cm.AddColumn("Parent", "Parent", "Родительский сценарий", DataAttributeTypes.dtString, Mandatory.NotNull);
            ////cm.AddColumn("PercOfComplete", "PercOfComplete", "Процент заполнения", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(75);
            /////cm.AddColumn("Status", "Status", "Статус", DataAttributeTypes.dtString, Mandatory.NotNull);
            cm.AddColumn("User", "User", "Пользователь", DataAttributeTypes.dtString, Mandatory.NotNull);

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            gp.Listeners.CellClick.AddAfter("if ({0}.getSelectionModel().hasSelection()) {{ btnCalc.setDisabled(false); }} else {{ btnCalc.setDisabled(true); }} ".FormatWith(GridId));

            return new List<Component> { gp, AddNewWindow() };
        }

        public Component ToolBar()
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
                ID = "btnCreateNew",
                Text = "Новый вариант",
                Icon = Icon.New,
                ToolTip = "Создать новый варинат на основе рассчитанного сценария"
            };

            btnCreateNew.Listeners.Click.AddAfter("wndAddNewValuation.show();");

            toolbar.Add(btnCreateNew);

            Button btnCalc = new Button
            {
                ID = "btnCalc",
                Icon = Icon.Calculator,
                Text = "Расчет",
                ToolTip = "Расчет заполненного варианта"
            };

            btnCalc.Disabled = true;

            btnCalc.DirectEvents.Click.Url = "/Valuation/Calc";
            btnCalc.DirectEvents.Click.CleanRequest = true;
            btnCalc.DirectEvents.Click.EventMask.ShowMask = true;
            btnCalc.DirectEvents.Click.ExtraParams.Add(new Parameter("varid", "{0}.getSelectionModel().hasSelection() ? {0}.getSelectionModel().getSelected().id : -1".FormatWith(GridId), ParameterMode.Raw));
            btnCalc.DirectEvents.Click.Success = "{0}.reload();".FormatWith(StoreId);

            toolbar.Add(btnCalc);

            /*Button btnSetReadyToCalc = new Button
            {
                ID = "btnSetReadyToCalc",
                Icon = Icon.Accept,
                Text = "Готов к расчету",
                ToolTip = "Пометить готовым к расчету"
            };

            btnSetReadyToCalc.Disabled = true;

            btnSetReadyToCalc.DirectEvents.Click.Url = "/Scenario/SetReadyToCalc";
            btnSetReadyToCalc.DirectEvents.Click.CleanRequest = true;
            btnSetReadyToCalc.DirectEvents.Click.EventMask.ShowMask = true;
            btnSetReadyToCalc.DirectEvents.Click.ExtraParams.Add(new Parameter("varid", "{0}.getSelectionModel().hasSelection() ? {0}.getSelectionModel().getSelected().id : -1".FormatWith(GridId), ParameterMode.Raw));
            btnSetReadyToCalc.DirectEvents.Click.Success = "{0}.reload();".FormatWith(StoreId);

            toolbar.Add(btnSetReadyToCalc);*/

            return toolbarPanel;
        }

        private static Component AddNewWindow()
        {
            Window wndAddNew = new Window
            {
                ID = "wndAddNewValuation",
                Hidden = true,
                Width = 400,
                Height = 250,
                Layout = "FitLayout",
                Modal = true,
                Title = "Добавление нового варинта",
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
                FieldLabel = "Наименование нового варианта расчета",
                Width = 300
            };

            singlePanel.Items.Add(txtName);

            ComboBox cboxParent = new ComboBox
            {
                ID = "cboxParent",
                Width = 300,
                FieldLabel = "Родительский сценарий",
                StoreID = "dsParentScen",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                DisplayField = "Text",
                ValueField = "Value",
                PageSize = 10
            };

            singlePanel.Items.Add(cboxParent);

            Button btnCancel = new Button
            {
                ID = "btnCancel",
                Text = "Отмена"
            };

            btnCancel.Listeners.Click.Handler = "wndAddNewValuation.hide();";

            Button btnOk = new Button
            {
                ID = "btnOk",
                Text = "Создать"
            };

            btnOk.DirectEvents.Click.Url = "/Valuation/NewValuation";
            btnOk.DirectEvents.Click.CleanRequest = true;
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("parentId", "cboxParent.getSelectedItem().value", ParameterMode.Raw));
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("name", "txtName.getValue()", ParameterMode.Raw));
            
            btnOk.DirectEvents.Click.Success = "wndAddNewValuation.hide(); {0}.reload();".FormatWith(StoreId);

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

        private Store CreateStore()
        {
            Store store = new Store { ID = StoreId, /*GroupField = "Groups",*/ AutoLoad = true };

            store.Sort("Year", SortDirection.ASC);

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Name");
            ////reader.Fields.Add("Parent");
            ////reader.Fields.Add("PercOfComplete");
            ////reader.Fields.Add("Status");
            reader.Fields.Add("Year");
            reader.Fields.Add("User");

            store.Reader.Add(reader);

            ////store.BaseParams.Add(new Parameter("sourceId", "cboxDataSource.getSelectedItem().value", ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Valuation/Load",
                Method = HttpMethod.POST
            });

            return store;
        }

        private Store CreateParentScenStore()
        {
            Store store = new Store { ID = "dsParentScen", /*GroupField = "Groups",*/ AutoLoad = true };

            ////store.Sort("Year", SortDirection.ASC);

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("Text");
            reader.Fields.Add("Value");
            
            store.Reader.Add(reader);

            ////store.BaseParams.Add(new Parameter("sourceId", "cboxDataSource.getSelectedItem().value", ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Valuation/LoadParentScen",
                Method = HttpMethod.POST
            });

            return store;
        }
    }
}
