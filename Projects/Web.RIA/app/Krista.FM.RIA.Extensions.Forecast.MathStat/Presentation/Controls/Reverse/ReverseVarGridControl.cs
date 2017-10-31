using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ReverseVarGridControl : Control
    {
        private const string GridId = "gpReverseVar";
        private const string StoreId = "dsReverseVar";

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateStore());
            page.Controls.Add(CreateComboVarsStore());

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

            gp.Listeners.Command.AddAfter(@"var tab = parent.MdiTab.getComponent('reverseForm_'+record.id);
if (!tab) 
{
    parent.MdiTab.addTab({   
        id: 'reverseForm_'+record.id,
        title: 'Обратное прогнозирование', 
        url: '/Reverse/ShowExist/'+record.id+'?parentId='+record.data.RefID,
        passParentSize: false        
    });
}
else
{
    parent.MdiTab.setActiveTab(tab);
}");
            
            ColumnBase cb = gp.ColumnModel.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.NotNull);
            gp.ColumnModel.AddColumn("Name", "Name", "Наименование", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(400);
            gp.ColumnModel.AddColumn("RefID", "RefID", "RefID", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(50);
            
            gp.SelectionModel.Add(new RowSelectionModel());

            return new List<Component> { gp, InsertWindow(page) };
        }

        private Component InsertWindow(ViewPage page)
        {
            Window wndAddReverse = new Window
            {
                ID = "wndAddReverse",
                Hidden = true,
                Width = 440,
                Height = 200,
                Layout = "FitLayout",
                Modal = true,
                Title = "Добавление обратного прогноза"
            };

            wndAddReverse.Listeners.Show.AddAfter("dsBaseVar.reload();");
            
            FormPanel formPanel = new FormPanel
            {
                ButtonAlign = Alignment.Right,
                ////Border = false,
                MonitorPoll = 500,
                MonitorValid = true,
                Padding = 10,
                Height = 200,
                LabelPad = -5,
                Width = 440,
                Border = false,
                LabelAlign = LabelAlign.Top
            };

            wndAddReverse.Items.Add(formPanel);

            TextField tfieldName = new TextField
            {
                ID = "tfieldName",
                FieldLabel = "Наименование прогноза",
                Width = 300
            };

            ComboBox cboxBase = new ComboBox 
            {
                ID = "cboxBase",
                FieldLabel = "Базовый вариант расчета",
                StoreID = "dsBaseVar",
                ValueField = "Value",
                DisplayField = "Text",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                Width = 400,
                PageSize = 10
            };

            cboxBase.Template.Html = @"
<tpl for=""."">
    <tpl if=""[xindex] == 1""><table><tr><th><b>Показатель</b></th><th><b>Наименование варианта</b></th><th><b>Период</b></th></tr></tpl>
	
    <tr class = ""list-item"">
		<td style=""padding:3px 0px;"">{Group}</td>		
        <td>{Text}</td>
        <td>{Year}</td>
	</tr>

	<tpl if=""[xcount] == [xindex]""></table></tpl>
</tpl>";
            cboxBase.ItemSelector = "tr.list-item";

            formPanel.Items.Add(new List<Component> { tfieldName, cboxBase });

            Button btnInsertCancel = new Button
            {
                ID = "btnInsertCancel",
                Text = "Отмена"
            };

            btnInsertCancel.Listeners.Click.Handler = "wndAddReverse.hide();";

            Button btnInsertOk = new Button
            {
                ID = "btnInsertOk",
                Text = "Вставить",
                Enabled = false
            };

            btnInsertOk.DirectEvents.Click.Url = "/ReverseVar/InsertNewReverse";
            btnInsertOk.DirectEvents.Click.CleanRequest = true;
            btnInsertOk.DirectEvents.Click.EventMask.ShowMask = true;
            btnInsertOk.DirectEvents.Click.ExtraParams.Add(new Parameter("name", "tfieldName.getValue()", ParameterMode.Raw));
            btnInsertOk.DirectEvents.Click.ExtraParams.Add(new Parameter("parentId", "cboxBase.getSelectedItem().value", ParameterMode.Raw));
            btnInsertOk.DirectEvents.Click.Success = "wndAddForecast.hide();";
            /*btnInsertOk.DirectEvents.Click.Failure = "";*/
            
            formPanel.Buttons.Add(new List<ButtonBase> { btnInsertCancel, btnInsertOk });

            return wndAddReverse;
        }

        private Store CreateStore()
        {
            Store store = new Store { ID = StoreId, AutoLoad = true };

            store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID", RecordFieldType.Int);
            reader.Fields.Add("Name", RecordFieldType.String);
            reader.Fields.Add("RefID", RecordFieldType.Int);
            
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/ReverseVar/Load",
                Method = HttpMethod.POST
            });

            return store;
        }

        private Store CreateComboVarsStore()
        {
            Store store = new Store { ID = "dsBaseVar", AutoLoad = true };

            store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };
            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);
            reader.Fields.Add("Group", RecordFieldType.String);
            reader.Fields.Add("Year", RecordFieldType.String);
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/ReverseVar/ComboVarsLoad",
                Method = HttpMethod.POST
            });

            return store;
        }
    }
}
