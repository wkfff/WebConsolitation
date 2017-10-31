using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class IdicPlanParamControl : Control
    {
        private const string AdjStoreId = "adjStore";
        private const string IndStoreId = "indStore";
        
        public IdicPlanParamControl()
        {
        }

        public int VarId { get; set; }

        public int ParentId { get; set; }

        public string Key { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateAdjsStore());
            page.Controls.Add(CreateIndsStore());
            page.Controls.Add(CreateAdjGroupStore());
            page.Controls.Add(CreateIndGroupStore());
            page.Controls.Add(CreateAddAdjsStore());
            page.Controls.Add(CreateAddIndsStore());

            Panel mainPanel = new Panel
            {
            };

            Panel adjPanel = new Panel
            {
                ID = "adjPanel",
                Collapsible = true,
                Height = 300,
                Title = "Регуляторы",
                AutoScroll = true
            };

            adjPanel.Items.Add(AdjusterPanel(page));

            Panel indPanel = new Panel
            {
                ID = "indPanel",
                Collapsible = true,
                Height = 300,
                Title = "Индикаторы",
                AutoScroll = true
            };

            indPanel.Items.Add(IndicatorPanel(page));

            mainPanel.Items.Add(adjPanel);
            mainPanel.Items.Add(indPanel);

            Toolbar toolbarAdj = new Toolbar
            {
                ID = "toolbarAdj"
            };

            adjPanel.TopBar.Add(toolbarAdj);

            Button btnAddNewAdj = new Button
            {
                ID = "btnAddNewAdj",
                Icon = Icon.Add
            };

            btnAddNewAdj.Listeners.Click.AddAfter("wndAddAdj.show();");
            
            toolbarAdj.Items.Add(btnAddNewAdj);

            Toolbar toolbarInd = new Toolbar
            {
                ID = "toolbarInd"
            };

            indPanel.TopBar.Add(toolbarInd);

            Button btnAddNewInd = new Button
            {
                ID = "btnAddNewInd",
                Icon = Icon.Add
            };

            btnAddNewInd.Listeners.Click.AddAfter("wndAddInd.show();");

            toolbarInd.Items.Add(btnAddNewInd);

            mainPanel.Items.Add(AddAdjWindow(page));
            mainPanel.Items.Add(AddIndWindow(page));

            mainPanel.AddScript("{0}.reload(); {1}.reload();".FormatWith(AdjStoreId, IndStoreId));
            
            return new List<Component> { mainPanel };
        }

        public Component CreateToolBar()
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

            /*Button btnSaveChange = new Button
            {
                ID = "btnSaveChange",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить изменения"
            };

            btnSaveChange.DirectEvents.Click.Url = "/ValuationParam/SaveChange";
            btnSaveChange.DirectEvents.Click.IsUpload = false;
            btnSaveChange.DirectEvents.Click.CleanRequest = true;
            btnSaveChange.DirectEvents.Click.EventMask.ShowMask = true;
            ////btnSaveChange.DirectEvents.Click.ExtraParams.Add(new Parameter("key", Key, ParameterMode.Value));

            ////btnSaveChange.DirectEvents.Click.Success = "btnCalc.setDisabled(false); {0}.commitChanges(); ".FormatWith(AdjStoreId);

            toolbar.Items.Add(btnSaveChange);*/

            Button btnIdicPlan = new Button
            {
                ID = "btnIdicPlan",
                Icon = Icon.Calculator,
                ToolTip = "Идикативное планирование"
            };

            btnIdicPlan.DirectEvents.Click.Url = "/IdicPlanParam/IdicPlan";
            btnIdicPlan.DirectEvents.Click.IsUpload = false;
            btnIdicPlan.DirectEvents.Click.CleanRequest = true;
            btnIdicPlan.DirectEvents.Click.EventMask.ShowMask = true;
            btnIdicPlan.DirectEvents.Click.ExtraParams.Add(new Parameter("parentId", ParentId.ToString(), ParameterMode.Value));
            btnIdicPlan.DirectEvents.Click.ExtraParams.Add(new Parameter("key", Key, ParameterMode.Value));

            toolbar.Items.Add(btnIdicPlan);

            return toolbarPanel;
        }

        private Component AdjusterPanel(ViewPage page)
        {
            GridPanel adjGridPanel = new GridPanel
            {
                ID = "adjGrid",
                StoreID = AdjStoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                ////AutoHeight = true,
                Height = 280,
                ////Layout = "fit",
                ////StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                ////Title = keyValuePair.Value,
                Layout = "Fit",
                ForceLayout = true,
                AutoRender = true
            };

            ColumnModel cm = adjGridPanel.ColumnModel;
            
            CommandColumn cmdColumn = new CommandColumn { Width = 25, Hideable = false };
            var command = new GridCommand { CommandName = "delete", Icon = Icon.Delete };
            command.ToolTip.Text = "Убрать регулятор из плана";
            cmdColumn.Commands.Add(command);
            cm.Columns.Add(cmdColumn);

            cm.AddColumn("ParamName", "ParamName", "Показатель", DataAttributeTypes.dtString, Mandatory.Nullable).SetWidth(150);
            cm.AddColumn("Units", "Units", "Ед. измерения", DataAttributeTypes.dtString, Mandatory.Nullable);

            var cb = cm.AddColumn("ValueEstimate", "ValueEstimate", "Оценочное значение", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x20) != 0)
    {
        metadata.style += 'background-color: pink;';
    }
    return value;
}";

            cb = cm.AddColumn("ValueY1", "ValueY1", "Y1", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x10) != 0)
    {
        metadata.style += 'background-color: pink;';
    }
    return value;
}";

            cb = cm.AddColumn("ValueY2", "ValueY2", "Y2", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x08) != 0)
    {
        metadata.style += 'background-color: pink;';
    }
    return value;
}";
            cb = cm.AddColumn("ValueY3", "ValueY3", "Y3", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x04) != 0)
    {
        metadata.style += 'background-color: pink;';
    }
    return value;
}";
            cb = cm.AddColumn("ValueY4", "ValueY4", "Y4", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x02) != 0)
    {
        metadata.style += 'background-color: pink;';
    }
    return value;
}";
            cb = cm.AddColumn("ValueY5", "ValueY5", "Y5", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x01) != 0)
    {
        metadata.style += 'background-color: pink;';
    }
    return value;
}";
            cm.AddColumn("MinBound", "MinBound", "MinBound", DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cm.AddColumn("MaxBound", "MaxBound", "MaxBound", DataAttributeTypes.dtDouble, Mandatory.Nullable);

            adjGridPanel.AddColumnsWrapStylesToPage(page);

            adjGridPanel.SelectionModel.Add(new RowSelectionModel());

            /*adjGridPanel.DirectEvents.DblClick.Url = "/IdicPlanParam/SelectYear";
            adjGridPanel.DirectEvents.DblClick.IsUpload = false;
            adjGridPanel.DirectEvents.DblClick.CleanRequest = true;*/

            adjGridPanel.Listeners.CellDblClick.Handler = @"
Ext.net.DirectEvent.confirmRequest(
{{
    cleanRequest: true,
    isUpload: false,
    url: '/IdicPlanParam/SelectAdjYear?paramId='+item.getRowsValues()[rowIndex].ID+'&columnName='+item.getColumnModel().columns[columnIndex].id+'&key={0}',
    control: this,
    userSuccess: function(response, result, el, type, action, extraParams){{ {1}.reload(); }}
}});".FormatWith(Key, AdjStoreId);

            adjGridPanel.Listeners.Command.AddAfter(
                String.Format(
@"Ext.net.DirectEvent.confirmRequest(
{{
	    cleanRequest: true,
	    isUpload: false,
	    url: '/IdicPlanParam/DeleteAdj?paramid='+record.id+'&key={0}',
        control: this,
        userSuccess: function(response, result, el, type, action, extraParams){{ {1}.reload(); }}
}});",
     Key,
     AdjStoreId));

            return adjGridPanel;
        }

        private Component IndicatorPanel(ViewPage page)
        {
            GridPanel indGridPanel = new GridPanel
            {
                ID = "indGrid",
                StoreID = IndStoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                ////AutoHeight = true,
                Height = 280,
                ////Layout = "fit",
                ////StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                ////Title = keyValuePair.Value,
                Layout = "Fit",
                ForceLayout = true,
                AutoRender = true
            };

            /*string fn = @"function(value,metadata,record,rowIndex,colIndex,store)
{
    return String.format(""<span style='color:{0};'>{1}</span>"", ((value >= record.data.MinBound) && (value <= record.data.MaxBound))? 'green' : 'red', value);
}";*/

            ColumnBase cb;
            ColumnModel cm = indGridPanel.ColumnModel;
            
            CommandColumn cmdColumn = new CommandColumn { Width = 25, Hideable = false };
            var command = new GridCommand { CommandName = "delete", Icon = Icon.Delete };
            command.ToolTip.Text = "Убрать индикатор из плана";
            cmdColumn.Commands.Add(command);
            cm.Columns.Add(cmdColumn);

            cm.AddColumn("ParamName", "ParamName", "Показатель", DataAttributeTypes.dtString, Mandatory.Nullable).SetWidth(150);
            cm.AddColumn("Units", "Units", "Ед. измерения", DataAttributeTypes.dtString, Mandatory.Nullable);

            cb = cm.AddColumn("ValueEstimate", "ValueEstimate", "Оценочное значение", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x20) != 0)
    {
        metadata.style += 'background-color: pink;';
    }

    return String.format(""<span style='color:{0};'>{1}</span>"", ((value >= record.data.MinBound) && (value <= record.data.MaxBound))? 'green' : 'red', value);
}";

            cb = cm.AddColumn("ValueY1", "ValueY1", "Y1", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x10) != 0)
    {
        metadata.style += 'background-color: pink;';
    }

    return String.format(""<span style='color:{0};'>{1}</span>"", ((value >= record.data.MinBound) && (value <= record.data.MaxBound))? 'green' : 'red', value);
}";

            cb = cm.AddColumn("ValueY2", "ValueY2", "Y2", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x08) != 0)
    {
        metadata.style += 'background-color: pink;';
    }

    return String.format(""<span style='color:{0};'>{1}</span>"", ((value >= record.data.MinBound) && (value <= record.data.MaxBound))? 'green' : 'red', value);
}"; 

            cb = cm.AddColumn("ValueY3", "ValueY3", "Y3", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x04) != 0)
    {
        metadata.style += 'background-color: pink;';
    }

    return String.format(""<span style='color:{0};'>{1}</span>"", ((value >= record.data.MinBound) && (value <= record.data.MaxBound))? 'green' : 'red', value);
}"; 

            cb = cm.AddColumn("ValueY4", "ValueY4", "Y4", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x02) != 0)
    {
        metadata.style += 'background-color: pink;';
    }

    return String.format(""<span style='color:{0};'>{1}</span>"", ((value >= record.data.MinBound) && (value <= record.data.MaxBound))? 'green' : 'red', value);
}"; 

            cb = cm.AddColumn("ValueY5", "ValueY5", "Y5", DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if ((record.data.Mask & 0x01) != 0)
    {
        metadata.style += 'background-color: pink;';
    }

    return String.format(""<span style='color:{0};'>{1}</span>"", ((value >= record.data.MinBound) && (value <= record.data.MaxBound))? 'green' : 'red', value);
}"; 

            cm.AddColumn("MinBound", "MinBound", "MinBound", DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cm.AddColumn("MaxBound", "MaxBound", "MaxBound", DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cm.AddColumn("LeftPenaltyCoef", "LeftPenaltyCoef", "LeftPenaltyCoef", DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cm.AddColumn("RightPenaltyCoef", "RightPenaltyCoef", "RightPenaltyCoef", DataAttributeTypes.dtDouble, Mandatory.Nullable);
            
            indGridPanel.AddColumnsWrapStylesToPage(page);

            indGridPanel.SelectionModel.Add(new RowSelectionModel());

            indGridPanel.Listeners.CellDblClick.Handler = @"
Ext.net.DirectEvent.confirmRequest(
{{
    cleanRequest: true,
    isUpload: false,
    url: '/IdicPlanParam/SelectIndYear?paramId='+item.getRowsValues()[rowIndex].ID+'&columnName='+item.getColumnModel().columns[columnIndex].id+'&key={0}',
    control: this,
    userSuccess: function(response, result, el, type, action, extraParams){{ {1}.reload(); }}
}});".FormatWith(Key, IndStoreId);

            indGridPanel.Listeners.Command.AddAfter(
                String.Format(
@"Ext.net.DirectEvent.confirmRequest(
{{
	    cleanRequest: true,
	    isUpload: false,
	    url: '/IdicPlanParam/DeleteInd?paramid='+record.id+'&key={0}',
        control: this,
        userSuccess: function(response, result, el, type, action, extraParams){{ {1}.reload(); }}
}});", 
     Key, 
     IndStoreId));

            return indGridPanel;
        }
        
        private Component AddAdjWindow(ViewPage page)
        {
            Window wndAddAdjNew = new Window
            {
                ID = "wndAddAdj",
                Hidden = true,
                Width = 400,
                Height = 320,
                Layout = "FitLayout",
                Modal = true,
                Title = "Добавить регулятор",
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
                LabelPad = -5,
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            Panel panel = new Panel
            {
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            formPanel.Items.Add(panel);

            ComboBox cboxAdjGroup = new ComboBox
            {
                ID = "cboxAdjGroup",
                Width = 350,
                FieldLabel = "Группа",
                StoreID = "dsAdjGroup",
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

            cboxAdjGroup.Listeners.Select.AddAfter("addAdjGrid.reload();");

            panel.Items.Add(cboxAdjGroup);

            GridPanel grid = new GridPanel
            {
                ID = "addAdjGrid",
                StoreID = "addAdjStore",
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                AutoWidth = true,
                Height = 200,
                ////Width = 350,
                ////Layout = "fit",
                ////StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                Title = "Параметр",
                Layout = "Fit",
                ForceLayout = true,
                AutoRender = true
            };

            ColumnModel cm = grid.ColumnModel;
            cm.AddColumn("ParamName", "ParamName", "Показатель", DataAttributeTypes.dtString, Mandatory.Nullable).SetWidth(250);
            cm.AddColumn("Units", "Units", "Ед. измерения", DataAttributeTypes.dtString, Mandatory.Nullable);

            grid.AddColumnsWrapStylesToPage(page);

            grid.SelectionModel.Add(new RowSelectionModel());

            panel.Items.Add(grid);

            Button btnCancel = new Button
            {
                ID = "btnAdjCancel",
                Text = "Отмена"
            };

            btnCancel.Listeners.Click.Handler = "wndAddAdj.hide();";

            Button btnOk = new Button
            {
                ID = "btnAdjOk",
                Text = "Добавить"
            };

            btnOk.DirectEvents.Click.Url = "/IdicPlanParam/AddAdjuster";
            btnOk.DirectEvents.Click.CleanRequest = true;
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("varId", "{0}".FormatWith(VarId), ParameterMode.Value));
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("adjId", "addAdjGrid.getSelectionModel().getSelected().id", ParameterMode.Raw));

            btnOk.DirectEvents.Click.Success = "wndAddAdj.hide(); {0}.reload();".FormatWith(AdjStoreId);

            formPanel.Buttons.Add(btnCancel);
            formPanel.Buttons.Add(btnOk);

            wndAddAdjNew.Items.Add(formPanel);
            
            return wndAddAdjNew;
        }

        private Component AddIndWindow(ViewPage page)
        {
            Window wndAddIndNew = new Window
            {
                ID = "wndAddInd",
                Hidden = true,
                Width = 400,
                Height = 320,
                Layout = "FitLayout",
                Modal = true,
                Title = "Добавить индикатор",
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
                LabelPad = -5,
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            Panel panel = new Panel
            {
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            formPanel.Items.Add(panel);

            ComboBox cboxIndGroup = new ComboBox
            {
                ID = "cboxIndGroup",
                Width = 350,
                FieldLabel = "Группа",
                StoreID = "dsIndGroup",
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

            cboxIndGroup.Listeners.Select.AddAfter("addIndGrid.reload();");

            panel.Items.Add(cboxIndGroup);

            GridPanel grid = new GridPanel
            {
                ID = "addIndGrid",
                StoreID = "addIndStore",
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                AutoWidth = true,
                Height = 200,
                ////Width = 350,
                ////Layout = "fit",
                ////StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                Title = "Параметр",
                Layout = "Fit",
                ForceLayout = true,
                AutoRender = true
            };

            ColumnModel cm = grid.ColumnModel;
            cm.AddColumn("ParamName", "ParamName", "Показатель", DataAttributeTypes.dtString, Mandatory.Nullable).SetWidth(250);
            cm.AddColumn("Units", "Units", "Ед. измерения", DataAttributeTypes.dtString, Mandatory.Nullable);

            grid.AddColumnsWrapStylesToPage(page);

            grid.SelectionModel.Add(new RowSelectionModel());

            panel.Items.Add(grid);

            Button btnCancel = new Button
            {
                ID = "btnIndCancel",
                Text = "Отмена"
            };

            btnCancel.Listeners.Click.Handler = "wndAddInd.hide();";

            Button btnOk = new Button
            {
                ID = "btnIndOk",
                Text = "Добавить"
            };

            btnOk.DirectEvents.Click.Url = "/IdicPlanParam/AddIndicator";
            btnOk.DirectEvents.Click.CleanRequest = true;
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("varId", "{0}".FormatWith(VarId), ParameterMode.Value));
            btnOk.DirectEvents.Click.ExtraParams.Add(new Parameter("indId", "addIndGrid.getSelectionModel().getSelected().id", ParameterMode.Raw));

            btnOk.DirectEvents.Click.Success = "wndAddInd.hide(); {0}.reload();".FormatWith(IndStoreId);

            formPanel.Buttons.Add(btnCancel);
            formPanel.Buttons.Add(btnOk);

            wndAddIndNew.Items.Add(formPanel);

            return wndAddIndNew;
        }

        private Store CreateAdjsStore()
        {
            Store store = new Store { ID = AdjStoreId, /*GroupField = "GroupName",*/ AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("ParamName");
            reader.Fields.Add("ValueEstimate");
            reader.Fields.Add("ValueY1");
            reader.Fields.Add("ValueY2");
            reader.Fields.Add("ValueY3");
            reader.Fields.Add("ValueY4");
            reader.Fields.Add("ValueY5");
            reader.Fields.Add("MinBound");
            reader.Fields.Add("MaxBound");
            reader.Fields.Add("Units");
            reader.Fields.Add("Mask");
            
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("varId", VarId.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("key", Key.ToString(), ParameterMode.Value));
            
            store.Proxy.Add(new HttpProxy
            {
                Url = "/IdicPlanParam/AdjsLoad",
                Method = HttpMethod.POST
            });

            /*store.WriteBaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("savedData", String.Format("{0}.getChangedData()", AdjStoreId), ParameterMode.Raw));

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/ScenarioParam/AdjsSave",
                Method = HttpMethod.POST,
                Timeout = 50000
            });*/

            return store;
        }

        private Store CreateIndsStore()
        {
            Store store = new Store { ID = IndStoreId, /*GroupField = "GroupName",*/ AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("ParamName");
            reader.Fields.Add("ValueEstimate");
            reader.Fields.Add("ValueY1");
            reader.Fields.Add("ValueY2");
            reader.Fields.Add("ValueY3");
            reader.Fields.Add("ValueY4");
            reader.Fields.Add("ValueY5");
            reader.Fields.Add("MinBound");
            reader.Fields.Add("MaxBound");
            reader.Fields.Add("LeftPenaltyCoef");
            reader.Fields.Add("RightPenaltyCoef");
            reader.Fields.Add("Units");
            reader.Fields.Add("Mask");
            
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("varId", VarId.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("key", Key.ToString(), ParameterMode.Value));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/IdicPlanParam/IndsLoad",
                Method = HttpMethod.POST
            });

            /*store.WriteBaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("savedData", String.Format("{0}.getChangedData()", AdjStoreId), ParameterMode.Raw));

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/ScenarioParam/AdjsSave",
                Method = HttpMethod.POST,
                Timeout = 50000
            });*/

            return store;
        }

        private Store CreateAdjGroupStore()
        {
            Store store = new Store { ID = "dsAdjGroup", /*GroupField = "GroupName",*/ AutoLoad = true };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("Text");
            reader.Fields.Add("Value");
            
            store.Reader.Add(reader);
            
            store.Proxy.Add(new HttpProxy
            {
                Url = "/IdicPlanParam/LoadAdjGroup",
                Method = HttpMethod.POST
            });
            
            return store;
        }

        private Store CreateIndGroupStore()
        {
            Store store = new Store { ID = "dsIndGroup", /*GroupField = "GroupName",*/ AutoLoad = true };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("Text");
            reader.Fields.Add("Value");

            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/IdicPlanParam/LoadIndGroup",
                Method = HttpMethod.POST
            });

            return store;
        }

        private Store CreateAddAdjsStore()
        {
            Store store = new Store { ID = "addAdjStore", /*GroupField = "GroupName",*/ AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("ParamName");
            reader.Fields.Add("Units");

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("groupId", "cboxAdjGroup.getSelectedItem().value", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("parentId", "{0}".FormatWith(ParentId), ParameterMode.Value));
            
            store.Proxy.Add(new HttpProxy
            {
                Url = "/IdicPlanParam/AddAdjsLoad",
                Method = HttpMethod.POST
            });
            
            return store;
        }

        private Store CreateAddIndsStore()
        {
            Store store = new Store { ID = "addIndStore", /*GroupField = "GroupName",*/ AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("ParamName");
            reader.Fields.Add("Units");

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("groupId", "cboxIndGroup.getSelectedItem().value", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("parentId", "{0}".FormatWith(ParentId), ParameterMode.Value));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/IdicPlanParam/AddIndsLoad",
                Method = HttpMethod.POST
            });

            return store;
        }
    }
}
