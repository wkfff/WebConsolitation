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
    public class Form2pParamsGridControl : Control
    {
        private const string StoreId = "dsForm2pParams";
        private const string GridId = "gpForm2Params";
        
        private Store comboStore;

        public Store Store { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            Store = CreateStore();
            page.Controls.Add(Store);

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

            GroupingView groupingView = new GroupingView
            {
                ID = "GroupingView1",
                HideGroupedColumn = true,
                EnableNoGroups = true,
                GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Items' : 'Item']})"
            };

            gp.View.Add(groupingView);
            
            ColumnModel cm = gp.ColumnModel;
            cm.AddColumn("Code", "Code", "Код", DataAttributeTypes.dtInteger, Mandatory.NotNull);
            cm.AddColumn("Name", "Name", "Параметр", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(400);
            cm.AddColumn("Note", "Note", "Примечание", DataAttributeTypes.dtString, Mandatory.NotNull);
            cm.AddColumn("RefOKEI", "RefOKEI", "Ед.измерения", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(100);
            cm.AddColumn("Signat", "Signat", "Сигнатура", DataAttributeTypes.dtString, Mandatory.NotNull);
            cm.AddColumn("Groups", "Groups", "Группа", DataAttributeTypes.dtString, Mandatory.Nullable);

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            return new List<Component> { gp };
        }

        public List<Component> TopPanel(ViewPage page)
        {
            comboStore = CreateComboStore();
            page.Controls.Add(comboStore);

            Panel panel = new Panel
            {
                Height = 50,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false,
                Padding = 10
            };

            ComboBox cboxDataSource = new ComboBox
            {
                ID = "cboxDataSource",
                StoreID = "comboStore",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                DisplayField = "Text",
                ValueField = "Value",
                FieldLabel = "Источник",
                Width = 300
            };

            cboxDataSource.SelectedIndex = 0;
            
            cboxDataSource.Listeners.Select.AddAfter(String.Format("{0}.reload();", StoreId));

            ////cboxDataSource.Listeners.AfterRender.AddAfter(String.Format("cboxDataSource.selectByIndex(0);{0}.reload();", StoreId));
            
            panel.Items.Add(cboxDataSource);
            
            return new List<Component> { panel };
        }

        public Store CreateStore()
        {
            Store store = new Store { ID = StoreId, GroupField = "Groups", AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Code" };
            reader.Fields.Add("Code");
            reader.Fields.Add("Name");
            reader.Fields.Add("Note");
            reader.Fields.Add("RefOKEI");
            reader.Fields.Add("Signat");
            reader.Fields.Add("Groups");

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("sourceId", "cboxDataSource.getSelectedItem().value", ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pParams/Load",
                Method = HttpMethod.POST
            });

            return store;
        }

        public Store CreateComboStore()
        {
            Store store = new Store { ID = "comboStore" };
            
            store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };
            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pParams/ComboStoreLoad",
                Method = HttpMethod.POST
            });

            store.Listeners.DataChanged.AddAfter("cboxDataSource.selectByIndex(0);{0}.reload();".FormatWith(StoreId));

            return store;
        }
    }
}