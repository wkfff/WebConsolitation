using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningRegGridControl : Control
    {
        private readonly IForecastExtension extension;
        private readonly string Key;
        private const string StoreId = "stRegData";
        private const string GridId = "grRegData";
        
        public PlanningRegGridControl(IForecastExtension extension, string key)
        {
            this.extension = extension;
            this.Key = key;
        }

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateStore());
            page.Controls.Add(CreateFVarStore());

            DataTable datReg = this.extension.Forms[Key].DataService.GetRegulatorData();
            int rowCount = datReg.Rows.Count;

            GridPanel regGridPanel = new GridPanel
            {
                ID = GridId,
                StoreID = StoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                ////AutoHeight = true,
                Height = 50 + (rowCount * 45) + 15,  //// 24 .... 34
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true
            };

            List<string> list = new List<string>();
            foreach (DataColumn col in datReg.Columns)
            {
                string colName = col.ColumnName;
                if (colName.Contains("year_"))
                {
                    list.Add(colName);
                }
            }

            list.Sort();

            ColumnModel cm = regGridPanel.ColumnModel;
            cm.AddColumn("Param", "Param", "Показатель", DataAttributeTypes.dtInteger, Mandatory.Nullable).SetWidth(150);

            Column column = new Column
            {
                ColumnID = "fvarcode",
                DataIndex = "fvarcode",
                Header = "Вариант",
                Wrap = true
            };
            
            ComboBox cbox = new ComboBox
            {
                ID = "cboxFVar",
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                StoreID = "stFVar",
                ValueField = "Value",
                DisplayField = "Text",
                Editable = false
            };

            cbox.Listeners.Render.AddAfter("stFVar.reload();");
            cbox.Listeners.Expand.AddAfter("stFVar.reload();");
            cbox.DirectEvents.Select.Url = "/PlanningReg/ChangeFVar";
            cbox.DirectEvents.Select.IsUpload = false;
            cbox.DirectEvents.Select.ExtraParams.Add(new Parameter("key", Key, ParameterMode.Value));
            cbox.DirectEvents.Select.ExtraParams.Add(new Parameter("paramId", "{0}.getSelectionModel().getSelected().id".FormatWith(GridId), ParameterMode.Raw));
            cbox.DirectEvents.Select.ExtraParams.Add(new Parameter("newVal", "cboxFVar.getSelectedItem().value", ParameterMode.Raw));
            cbox.DirectEvents.Select.Success = "{0}.reload();".FormatWith(StoreId);

            column.Editor.Add(cbox);

            cm.Columns.Add(column);
            
            foreach (string colName in list)
            {
                string headName = colName.Replace("year_", String.Empty);
                ColumnBase col = cm.AddColumn(colName, colName, headName, DataAttributeTypes.dtInteger, Mandatory.Nullable);
            }

            regGridPanel.AddColumnsWrapStylesToPage(page);

            regGridPanel.SelectionModel.Add(new RowSelectionModel());

            return new List<Component> { regGridPanel };
        }

        private Store CreateStore()
        {
            Store store = new Store { ID = StoreId, AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data" };

            DataTable datReg = this.extension.Forms[Key].DataService.GetRegulatorData();

            foreach (DataColumn c in datReg.Columns)
            {
                string colName = c.ColumnName;
                RecordFieldType recType;
                switch (c.DataType.FullName)
                {
                    case "System.String":
                        recType = RecordFieldType.String;
                        break;
                    case "System.Double":
                        recType = RecordFieldType.Float;
                        break;
                    default: recType = RecordFieldType.Auto;
                        break;
                }

                reader.Fields.Add(colName, recType);
            }
            
            store.Reader.Add(reader);
            
            store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", Key), ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningReg/LoadRegs",
                Method = HttpMethod.POST
            });

            return store;
        }

        private Store CreateFVarStore()
        {
            Store store = new Store { ID = "stFVar", AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };

            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("regId", "{0}.getSelectionModel().hasSelection() ? {0}.getSelectionModel().getSelected().id : -1".FormatWith(GridId), ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningReg/LoadFVar",
                Method = HttpMethod.POST
            });

            return store;
        }
    }
}
