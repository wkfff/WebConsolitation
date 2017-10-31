using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class SvodMOValuesGridControl : Control
    {
        private const string StoreId = "dsSvodMOValues";
        private const string GridId = "gpSvodMOValues";

        private int varid;
        private int year;

        public SvodMOValuesGridControl(int varid, int year)
        {
            this.varid = varid;
            this.year = year;
        }

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateStore());
            
            GridPanel gp = new GridPanel
            {
                ID = GridId,
                StoreID = StoreId,
                ////MonitorResize = true,
                /*AutoHeight = true,*/
                /*AutoWidth = true,*/
                Height = 600,
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                ////AutoExpandColumn = "ParamName",
                AutoScroll = true,
                Border = false
            };

            gp.ColumnModel.AddColumn("ParamName", "ParamName", "Параметр", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(300);
            
            gp.ColumnModel.AddColumn("Units", "Units", "Ед.измер.", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(150);
            ////gp.ColumnModel.AddColumn("RefForecastType", "RefForecastType", "RefForecastType", DataAttributeTypes.dtInteger, Mandatory.NotNull);
            
            var cb = gp.ColumnModel.AddColumn("R1", "R1", String.Format("Отчет {0}", year - 2), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = gp.ColumnModel.AddColumn("R2", "R2", String.Format("Отчет {0}", year - 1), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = gp.ColumnModel.AddColumn("Est", "Est", String.Format("Оценка {0}", year), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = gp.ColumnModel.AddColumn("Y1v1", "Y1v1", String.Format("Прогноз {0} <br/> 1 вар.", year + 1), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = gp.ColumnModel.AddColumn("Y1v2", "Y1v2", String.Format("Прогноз {0} <br/> 2 вар.", year + 1), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = gp.ColumnModel.AddColumn("Y2v1", "Y2v1", String.Format("Прогноз {0} <br/> 1 вар.", year + 2), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = gp.ColumnModel.AddColumn("Y2v2", "Y2v2", String.Format("Прогноз {0} <br/> 2 вар.", year + 2), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = gp.ColumnModel.AddColumn("Y3v1", "Y3v1", String.Format("Прогноз {0} <br/> 1 вар.", year + 3), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = gp.ColumnModel.AddColumn("Y3v2", "Y3v2", String.Format("Прогноз {0} <br/> 2 вар.", year + 3), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            gp.AddColumnsWrapStylesToPage(page);
            
            ////выбор строками
            gp.SelectionModel.Add(new RowSelectionModel());

            return new List<Component> { gp };
        }

        public Store CreateStore()
        {
            Store store = new Store { ID = StoreId, AutoLoad = true };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID", RecordFieldType.Int);
            reader.Fields.Add("ParamName", RecordFieldType.String);
            reader.Fields.Add("Units", RecordFieldType.String);
            reader.Fields.Add("R1", RecordFieldType.Auto);
            reader.Fields.Add("R2", RecordFieldType.Auto);
            reader.Fields.Add("Est", RecordFieldType.Auto);
            reader.Fields.Add("Y1v1", RecordFieldType.Auto);
            reader.Fields.Add("Y1v2", RecordFieldType.Auto);
            reader.Fields.Add("Y2v1", RecordFieldType.Auto);
            reader.Fields.Add("Y2v2", RecordFieldType.Auto);
            reader.Fields.Add("Y3v1", RecordFieldType.Auto);
            reader.Fields.Add("Y3v2", RecordFieldType.Auto);
            reader.Fields.Add("Code", RecordFieldType.Auto);
            
            store.Reader.Add(reader);

            store.Sort("Code", SortDirection.ASC);

            store.BaseParams.Add(new Parameter("varid", varid.ToString(), ParameterMode.Value));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/SvodMOValues/Load",
                Method = HttpMethod.POST
            });

            store.WriteBaseParams.Add(new Parameter("savedData", String.Format("{0}.getChangedData()", StoreId), ParameterMode.Raw));
            
            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/SvodMOValues/Save",
                Method = HttpMethod.POST,
                Timeout = 50000
            });

            return store;
        }
    }
}
