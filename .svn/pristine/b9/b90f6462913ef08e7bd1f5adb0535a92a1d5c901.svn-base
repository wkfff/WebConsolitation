using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningProgGridControl : Control
    {
        private const string GridId = "grProgData";
        private const string StoreDataId = "stProgData";

        private readonly string Key;

        private IForecastExtension extension;

        public PlanningProgGridControl(IForecastExtension extension, string key)
        {
            this.extension = extension;
            Key = key;
        }
        
        public Store StoreData { get; private set; }
        
        public static Store CreateDataStore(string storeId, string key, DataTable datProg)
        {
            Store store = new Store { ID = storeId, AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data" };
            
            foreach (DataColumn c in datProg.Columns)
            {
                string colName = c.ColumnName;
                if (colName != "id")
                {
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
            }

            reader.Fields.Add("image", RecordFieldType.String);

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));
            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningProg/LoadProg",  ////progdata
                Method = HttpMethod.POST
            });

            return store;
        }
        
        public override List<Component> Build(ViewPage page)
        {
            var ufc = this.extension.Forms[Key];
            DataTable datProg = ufc.DataService.GetProgData(); ////.GetObject("dtProg") as DataTable;
            
            StoreData = CreateDataStore(StoreDataId, Key, datProg);
            page.Controls.Add(StoreData);
            
            var rowCount = datProg.Rows.Count > 5 ? 5 : datProg.Rows.Count + 1;

            GridPanel gridPanel = new GridPanel
            {
                ID = GridId,
                StoreID = StoreDataId,
                Border = false,
                ////AutoHeight = true,
                ////MinHeight = 80,
                ////Height = 150,
                Height = 50 + (rowCount * 45) + 15,
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                AutoScroll = true,
                AutoWidth = true,
                Collapsible = false,
                /*StripeRows = true,
                Split = true,*/
                ColumnLines = true
                ////Title = "Прогнозируемые показатели"
            };

            ColumnModel cm = gridPanel.ColumnModel;

            List<string> list = new List<string>();
            foreach (DataColumn col in datProg.Columns)
            {
                string colName = col.ColumnName;
                if (colName.Contains("year_"))
                {
                    list.Add(colName);
                }
            }

            list.Sort();

            cm.AddColumn("Param", "Param", "Показатель", DataAttributeTypes.dtInteger, Mandatory.Nullable);

            var column = cm.AddColumn("image", "image", String.Empty, DataAttributeTypes.dtString, Mandatory.Nullable);
            column.Width = 71;
            
            foreach (string colName in list)
            {
                string headName = colName.Replace("year_", String.Empty);
                ColumnBase col = cm.AddColumn(colName, colName, headName, DataAttributeTypes.dtInteger, Mandatory.Nullable);
                col.SetEditableDouble(6);
                col.Fixed = true;
                col.SetLocked();
                col.Editor.Editor.ReadOnly = true;
            }
            
            gridPanel.AddColumnsWrapStylesToPage(page);

            gridPanel.SelectionModel.Add(new RowSelectionModel());
            
            gridPanel.Listeners.AfterRender.AddAfter("stProgData.load();");

            //// stCritData.load(); stFormulaData.load();
            
            return new List<Component> { gridPanel };
        }
    }
}
