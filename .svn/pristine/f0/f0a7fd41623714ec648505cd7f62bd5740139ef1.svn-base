using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.Consolidation.Models;
using Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.ExcelRequisitesGridView;
using Krista.FM.RIA.Extensions.Consolidation.Presentation.Controls.RequisitesSelectionModel;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ExcelReportRequisitesControl : Control
    {
        public string Title { get; set; }

        public int ReportId { get; set; }

        public string SectionCode { get; set; }

        public IList<D_Form_Requisites> Requisites { get; set; }
        
        public FormGridViewModel LayoutMarkup { get; set; }

        public RequisiteKinds RequisiteKind { get; set; }

        public RequisiteClass RequisiteClass { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            var store = CreateStore();

            page.Controls.Add(store);

            var grid = new GridPanel
            {
                ID = "grid" + Id,
                StoreID = store.ID, 
                HideHeaders = true, 
                Border = false,
                Collapsible = true,
                CollapseMode = CollapseMode.Mini,
                Title = this.Title
            };

            grid.LoadMask.ShowMask = true;
            grid.LoadMask.Msg = "Загрузка данных...";

            grid.SaveMask.ShowMask = true;
            grid.SaveMask.Msg = "Сохранение данных...";

            grid.ColumnModel.Columns.AddRange(LayoutMarkup.Columns);

            grid.View.Add(new ExcelRequisitesGridView { LayoutMarkup = this.LayoutMarkup.Layout, HeadersDisabled = true });
            grid.SelectionModel.Add(new RequisitesSelectionModel());

            return new List<Component> { grid };
        }

        private Store CreateStore()
        {
            StringBuilder action = new StringBuilder();
            if (RequisiteClass == RequisiteClass.Section)
            {
                action.Append("Section");
            }

            action.Append(RequisiteKind == RequisiteKinds.Header ? "HeaderRequisites" : "FooterRequisites");

            var store = new Store { ID = "store" + Id };
            store.SetJsonReader()
                .SetHttpProxy("/ConsReport/Get" + action)
                .AddField("ID");

            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/ConsReport/Save" + action });

            foreach (var requisite in Requisites)
            {
                store.AddField(requisite.InternalName.ToUpper());
            }

            store.BaseParams.Add(new Parameter("reportId", Convert.ToString(ReportId)));
            store.WriteBaseParams.Add(new Parameter("reportId", Convert.ToString(ReportId)));

            if (SectionCode.IsNotNullOrEmpty())
            {
                store.BaseParams.Add(new Parameter("sectionCode", SectionCode));
                store.WriteBaseParams.Add(new Parameter("sectionCode", SectionCode));
            }

            return store;
        }
    }
}
