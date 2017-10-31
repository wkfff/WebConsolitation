using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.Consolidation.Models;
using Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.ExcelGridView;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ExcelReportTableControl : Control
    {
        public string Title { get; set; }

        public int ReportId { get; set; }
        
        public D_Form_Part Section { get; set; }

        public FormGridViewModel LayoutMarkup { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            var store = CreateStore();

            page.Controls.Add(store);

            var grid = new GridPanel
            {
                ID = "grid" + Section.InternalName, 
                StoreID = store.ID, 
                HideHeaders = true, 
                Border = false,
                Collapsible = true,
                CollapseMode = CollapseMode.Mini,
                Title = "Раздел " + Section.Name
            };

            grid.LoadMask.ShowMask = true;
            grid.LoadMask.Msg = "Загрузка данных...";

            grid.SaveMask.ShowMask = true;
            grid.SaveMask.Msg = "Сохранение данных...";

            grid.ColumnModel.Columns.AddRange(LayoutMarkup.Columns);

            var view = new ExcelGridView { LayoutMarkup = this.LayoutMarkup.Layout, HeadersDisabled = true };

            var handler = @"
                function onNewRow(sourceRecord, insertTo)
                {{       
                    var record = sourceRecord.copy();
                    record.newRecord = true;
                    record.phantom = true;
                    record.id = Ext.data.Record.id(record);
                    record.set('ID', -1);
                    {0}.insert(insertTo, record);
                    
                    var h = {1}.getView().mainBody.getHeight();
                    {1}.setHeight(h + 55);
                }};
                function onDeleteRow(record, index)
                {{    
                    var metaId = record.get('RefFormRow');
                    var multipliesRows = {0}.queryBy(function(r){{ return r.get('RefFormRow') == metaId; }});
                    if (multipliesRows.getCount() > 1) {{
                        {0}.removeAt(index);

                        var h = {1}.getView().mainBody.getHeight();
                        {1}.setHeight(h + 55);
                    }}
                }};
                {1}.getView().on('newrow', onNewRow);
                {1}.getView().on('deleterow', onDeleteRow);".FormatWith(store.ID, grid.ID);
            grid.View.Add(view);
            grid.AddScript(handler);

            grid.SelectionModel.Add(new CellSelectionModel());

            return new List<Component> { grid };
        }

        private Store CreateStore()
        {
            var store = new Store { ID = "store" + Section.InternalName };
            store.SortInfo.Field = "MetaRowOrd";
            store.SortInfo.Direction = SortDirection.ASC;
            store.SetJsonReader()
                .SetHttpProxy("/ConsReport/GetSectionData")
                .AddField("ID")
                .AddField("RefFormRow")
                .AddField("MetaRowOrd");

            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/ConsReport/SaveSectionData" });

            foreach (var tableColumn in Section.Columns)
            {
                store.AddField(tableColumn.InternalName);
            }

            store.BaseParams.Add(new Parameter("reportId", Convert.ToString(ReportId)));
            store.BaseParams.Add(new Parameter("sectionCode", Section.Code));
            store.WriteBaseParams.Add(new Parameter("reportId", Convert.ToString(ReportId)));
            store.WriteBaseParams.Add(new Parameter("sectionCode", Section.Code));

            return store;
        }
    }
}
