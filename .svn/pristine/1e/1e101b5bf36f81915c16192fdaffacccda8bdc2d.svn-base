using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;
using Button = Ext.Net.Button;
using Label = Ext.Net.Label;
using ListItem = Ext.Net.ListItem;
using Panel = Ext.Net.Panel;
using Parameter = Ext.Net.Parameter;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningVariantsGridControl : Control
    {
        private const string StoreId = "dsVariants";
        private const string GridId = "gpVariants";

        private IForecastExtension extension;

        public PlanningVariantsGridControl(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public Store Store { get; private set; }

        public GridPanel GridPanel { get; private set; }
        
        public override List<Component> Build(ViewPage page)
        {
            Store = CreateStore(StoreId);
            page.Controls.Add(Store);

            GridPanel = CreateGridPanel(GridId, StoreId, page);
            return new List<Component> { GridPanel, InsertWindow(page) };
        }

        public GridPanel CreateGridPanel(string gridId, string storeId, ViewPage page)
        {
            GridPanel gp = new GridPanel
            {
                ID = gridId,
                StoreID = storeId,
                MonitorResize = true,
                AutoScroll = true,
                Width = 500,
                AutoHeight = true,
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true
            };

            ////ColumnBase cb = gp.ColumnModel.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.NotNull);
            ////cb.SetWidth(50);

            CommandColumn cmdColumn = new CommandColumn { Width = 25, Hideable = false };
            var command = new GridCommand { CommandName = "edit", Icon = Icon.ApplicationFormEdit };
            command.ToolTip.Text = "Редактировать";
            cmdColumn.Commands.Add(command);
            gp.ColumnModel.Columns.Add(cmdColumn);

            // Not visible
            gp.ColumnModel.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(200).SetVisible(false);
            
            gp.ColumnModel.AddColumn("Name", "Name", "Наименование", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(200);
            gp.ColumnModel.AddColumn("refParamName", "refParamName", "Параметр", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(200);
            gp.ColumnModel.AddColumn("luMethod", "luMethod", "Метод", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(200);
            gp.ColumnModel.AddColumn("luRefDate", "luRefDate", "Период", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(80);
            gp.ColumnModel.AddColumn("luStatus", "luStatus", "Статус", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(150);
            gp.ColumnModel.AddColumn("luUser", "luUser", "Пользователь", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(150);
            ////gp.ColumnModel.AddColumn("XMLString", "XMLString", "XML String", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(100);
            ////gp.ColumnModel.AddColumn("refParam", "refParam", "Параметр", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(50);

            GroupingView groupingView = new GroupingView
            {
                ID = "gvPlanVar",
                HideGroupedColumn = true,
                EnableNoGroups = true,
                GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Items' : 'Item']})"
            };

            gp.View.Add(groupingView);

            ////String url = "/PlanningView/Show/";
            gp.Listeners.Command.AddAfter(@"var tab = parent.MdiTab.getComponent('planningForm_'+record.id);
if (!tab) 
{
    parent.MdiTab.addTab({   
        id: 'planningForm_'+record.id,
        title: 'Прогнозирование показателей', 
        url: '/Planning/ShowExist/'+record.id,
        passParentSize: false        
    });

}
else
{
    parent.MdiTab.setActiveTab(tab);
}");
            ////       
            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            return gp;
        }

        public Store CreateStore(string storeId)
        {
            Store store = new Store { ID = storeId, GroupField = "refParamName" };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Name");
            reader.Fields.Add("refParam");
            reader.Fields.Add("refParamName");
            reader.Fields.Add("luMethod");
            reader.Fields.Add("luStatus");
            reader.Fields.Add("luUser");
            reader.Fields.Add("luRefDate");
            ////reader.Fields.Add("refParam");

            store.Reader.Add(reader);
            
            ////store.BaseParams.Add(new Parameter("id", "gpParams.getSelectionModel().hasSelection() ? gpParams.getSelectionModel().getSelected().id : -1", ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningVariants/Load",
                Method = HttpMethod.POST
            });
            
            return store;
        }

        private Component InsertWindow(ViewPage page)
        {
            Window wndAddForecast = new Window
            {
                ID = "wndAddForecast",
                Hidden = true,
                Width = 440,
                Height = 400,
                Layout = "FitLayout",
                Modal = true,
                Title = "Добавление нового прогноза"
            };

            wndAddForecast.Listeners.Show.AddBefore("paramPanel.layout.setActiveItem(0); if (gpVariantParams.getSelectionModel().hasSelection() && (cboxMathGroup.getSelectedIndex() != -1)) { btnInsertOk.setDisabled(false);} else { btnInsertOk.setDisabled(true);}");

            FormPanel formPanel = new FormPanel
            {
                ButtonAlign = Alignment.Right,
                ////Border = false,
                MonitorPoll = 500,
                MonitorValid = true,
                Padding = 10,
                LabelPad = -5,
                Width = 440,
                Border = false,
                LabelAlign = LabelAlign.Top
            };

            wndAddForecast.Items.Add(formPanel);

            Panel paramPanel = new Panel
            {
                ID = "paramPanel",
                Height = 300,
                Width = 420,
                Border = false,
                Layout = "card",
                ActiveIndex = 0
            };
            
            formPanel.Items.Add(paramPanel);

            Panel firstTabPanel = new Panel
            {
                ID = "firstTab",
                Border = false,
                Header = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            Panel secondTabPanel = new Panel
            {
                ID = "secondTab",
                Border = false,
                Header = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            paramPanel.Items.Add(firstTabPanel);
            paramPanel.Items.Add(secondTabPanel);
            
            PlanningVariantsParamsGridControl paramsGridControl = new PlanningVariantsParamsGridControl(PlanVarSource.AddForecast);

            ComboBox cboxMathGroup = new ComboBox
            {
                FieldLabel = "Группа прогнозных методов",
                ID = "cboxMathGroup",
                Name = "cboxMathGroup",
                Width = 350,
                ////StoreID = "dsPlanVar",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                EmptyText = "Выберите группу..."
                ////DisplayField = "Text",
                ////ValueField = "Value",
            };

            foreach (var mathGroup in extension.LoadedMathGroups)
            {
                cboxMathGroup.Items.Add(new ListItem { Text = mathGroup.TextName, Value = mathGroup.Code.ToString() });
            }

            cboxMathGroup.Listeners.Select.AddAfter("if (gpVariantParams.getSelectionModel().hasSelection() && (cboxMathGroup.getSelectedIndex() != -1)) { btnInsertOk.setDisabled(false);} else { btnInsertOk.setDisabled(true);}");

            firstTabPanel.Items.Add(cboxMathGroup);

            firstTabPanel.Items.Add(new Label { Text = "Прогнозируемый параметр:" });
            firstTabPanel.Items.Add(paramsGridControl.Build(page));

            paramsGridControl.ParamsGridPanel.Listeners.CellClick.AddAfter("if (gpVariantParams.getSelectionModel().hasSelection() && (cboxMathGroup.getSelectedIndex() != -1)) { btnInsertOk.setDisabled(false);} else { btnInsertOk.setDisabled(true);}");

            ComboBox cboxMathMethod = new ComboBox
            {
                FieldLabel = "Предустановленная методика",
                ID = "cboxMathMethod",
                Name = "cboxMathMethod",
                Width = 350,
                ////StoreID = "dsPlanVar",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                EmptyText = "Выберите методику..."
                ////DisplayField = "Text",
                ////ValueField = "Value",
            };

            TextArea txtArea = new TextArea
            {
                ID = "textArea",
                Text = String.Empty,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                BorderWidth = 0,
                FieldLabel = "Описание",
                Width = 350,
                Height = 100,
                Value = " "
            };
            
            var group = extension.LoadedMathGroups.GetGroupByCode(FixedMathGroups.ComplexEquation);
            if (group.HasValue)
            {
                MathMethods methods = group.Value.Methods;

                foreach (var mathMethods in methods)
                {
                    cboxMathMethod.Items.Add(new ListItem { Text = mathMethods.TextName, Value = mathMethods.Code.ToString() });
                }

                cboxMathMethod.SelectedIndex = 0;
            }

            cboxMathMethod.DirectEvents.Select.Url = "/PlanningVariants/GetDescription";
            cboxMathMethod.DirectEvents.Select.CleanRequest = true;
            cboxMathMethod.DirectEvents.Select.EventMask.ShowMask = true;
            cboxMathMethod.DirectEvents.Select.ExtraParams.Add(new Parameter("method", "cboxMathMethod.getSelectedItem().value", ParameterMode.Raw));
            
            secondTabPanel.Items.Add(cboxMathMethod);
            secondTabPanel.Items.Add(txtArea);

            ////secondTabPanel.AddScript("textArea.setHeight(100);");

            Button btnInsertCancel = new Button
            {
                ID = "btnInsertCancel",
                Text = "Отмена"
            };

            btnInsertCancel.Listeners.Click.Handler = "wndAddForecast.hide();";

            Button btnInsertOk = new Button
            {
                ID = "btnInsertOk",
                Text = "Вставить",
                Enabled = false
            };

            /*
             Ext.net.DirectEvent.confirmRequest(
            {
                    cleanRequest: true,
                    isUpload: false,
                    url: '/Planning/SetGroup?group='+cboxMathGroup.getSelectedIndex(),
                    control: this
                    ////userSuccess: function(response, result, el, type, action, extraParams){  }
            });

             */
            btnInsertOk.Listeners.Click.AddAfter(@"
if ((cboxMathGroup.getSelectedItem().value == {0}) && (paramPanel.layout.activeItem.getId() == ""firstTab""))
{{
    paramPanel.layout.setActiveItem(1);
}}
else
{{
    parent.MdiTab.addTab({{ 
        title: 'Прогнозирование показателей', 
        url: '/Planning/ShowNew/'+gpVariantParams.getSelectionModel().getSelected().id+'?group='+cboxMathGroup.getSelectedItem().value+'&method='+cboxMathMethod.getSelectedItem().value,
        passParentSize: false  
    }});
    wndAddForecast.hide();
}};".FormatWith(FixedMathGroups.ComplexEquation));
            
            /////url: '/Planning/ShowNew?id='+gpVariantParams.getSelectionModel().getSelected().id+'&group='+cboxMathGroup.getSelectedIndex(),
            formPanel.Buttons.Add(new List<ButtonBase> { btnInsertCancel, btnInsertOk });

            return wndAddForecast;
        }
    }
}
