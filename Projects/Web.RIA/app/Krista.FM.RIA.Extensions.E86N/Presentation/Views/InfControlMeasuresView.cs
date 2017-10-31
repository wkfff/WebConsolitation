using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.InfControlMeasures;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.InfControlMeasures;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    /// <summary>
    /// Контрольные мероприятия 
    /// </summary>
    public class InfControlMeasuresView : View
    {
        #region Setting

        private const string Scope = "E86n.View.InfControlMeasures";

        private const string InfControlMeasuresStoreID = "InfControlMeasuresStore";

        private const string InfControlMeasuresID = "InfControlMeasures";

        private const string ExtHeaderStoreID = "ExtHeaderStore";

        private const string ExtHeaderID = "NotInspectionActivity";

        private readonly ExtHeaderModel extHeaderModel = new ExtHeaderModel();

        #endregion

        private readonly IAuthService auth;
        
        public InfControlMeasuresView()
        {
            auth = Resolver.Get<IAuthService>();
        }

        private int? DocId
        {
            get { return Params["docId"] == "null" ? -1 : Convert.ToInt32(Params["docId"]); }
        }

        private ViewPage Page { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            Page = page;

            ResourceManager.GetInstance(page).RegisterClientScriptBlock("InfControlMeasuresView", Resource.InfControlMeasuresView);

            RestActions restActions = ResourceManager.GetInstance(page).RestAPI;
            restActions.Create = HttpMethod.POST;
            restActions.Read = HttpMethod.GET;
            restActions.Update = HttpMethod.POST;
            restActions.Destroy = HttpMethod.DELETE;

            GetExtHeaderStore();
            
            Toolbar tb = new NewStateToolBarControl(Convert.ToInt32(DocId.ToString())).BuildComponent(page);
            tb.Add(new VersioningControl(Convert.ToInt32(DocId.ToString()), UiBuilders.GetControllerID<InfControlMeasuresController>(), Scope + ".SetReadOnlyInfControlMeasures").Build(page));

            if (auth.IsAdmin())
            {
                var export = new UpLoadFileBtnControl
                {
                    Id = "btnExport",
                    Name = "Экспорт в XML",
                    Icon = Icon.DiskDownload,
                    Upload = false,
                    UploadController = "/InfControlMeasures/ExportToXml",
                    Params = { { "recId", DocId.ToString() } }
                };

                tb.Add(export.Build(Page));

                tb.Add(new ToolbarSeparator());

                tb.Add(new SetDocStateBtn(Convert.ToInt32(DocId.ToString())).Build(Page));
            }
            
            var view = new Viewport
                {
                    Items =
                        {
                            new BorderLayout
                                {
                                    North = { Items = { new ParamDocPanelControl(DocId ?? -1, tb).BuildComponent(page) } },
                                    Center =
                                        {
                                            Items =
                                                    { 
                                                        UiBuilders.GetTabbedDetails(new List<Component>
                                                                {
                                                                    GetInfControlMeasures(),
                                                                    new DocsDetailControl(DocId ?? -1).BuildComponent(page)
                                                                })
                                                    }
                                        }
                                }
                        }
                };

            return new List<Component> { view };
        }

        private void GetExtHeaderStore()
        {
            var store = StoreExtensions.StoreCreateDefault(
                 ExtHeaderStoreID,
                 true,
                 typeof(InfControlMeasuresController),
                 "ExtHeaderRead");

            store.Listeners.DataChanged.Handler = Scope + ".DataChanged('{0}', store)".FormatWith(ExtHeaderID);

            store.AddFieldsByClass(extHeaderModel);
            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Raw);
            Page.Controls.Add(store);
        }

        private Store GetInfControlMeasuresStore()
        {
            Store store = StoreExtensions.StoreCreateDefault(
                InfControlMeasuresStoreID,
                true,
                typeof(InfControlMeasuresController),
                "{0}Read".FormatWith(InfControlMeasuresID),
                "{0}Save".FormatWith(InfControlMeasuresID),
                "{0}Save".FormatWith(InfControlMeasuresID),
                "{0}Delete".FormatWith(InfControlMeasuresID));
            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Raw);
            InfControlMeasuresHelpers.InfControlMeasuresExportMetadataTo((JsonReader)store.Reader.Reader);
            Page.Controls.Add(store);
            return store;
        }

        private GridPanel GetInfControlMeasures()
        {
            GridPanel gp = UiBuilders.CreateGridPanel(InfControlMeasuresID, GetInfControlMeasuresStore());
            gp.Title = @"Сведения о проведенных контрольных мероприятиях и их результатах";

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.Toolbar().Add(new ToolbarSeparator());

            var cb = new Checkbox
                {
                    ID = ExtHeaderID,
                    FieldLabel = extHeaderModel.DescriptionOf(() => extHeaderModel.NotInspectionActivity),
                    LabelPad = 70
                };

            cb.Listeners.Check.Handler = Scope + ".Check(item, checked, '{0}', {1})".FormatWith(
                                                         UiBuilders.GetUrl<InfControlMeasuresController>(
                                                             "SetNotInspectionActivity",
                                                             new Dictionary<string, object>
                                                                 {
                                                                     { "docId", DocId }
                                                                 }),
                                                             DocId);

            gp.Toolbar().Add(cb);

            gp.Toolbar().Add(new ToolbarSeparator());

            gp.ColumnModel.AddColumn(
                InfControlMeasures.Supervisor.ToString(),
                InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.Supervisor),
                DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);

            gp.ColumnModel.AddColumn(
                InfControlMeasures.Topic.ToString(),
                InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.Topic),
                DataAttributeTypes.dtString).SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);

            gp.ColumnModel.AddColumn(
                InfControlMeasures.EventBegin.ToString(),
                InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.EventBegin),
                DataAttributeTypes.dtDate).SetEditableDate().SetWidth(100);

            gp.ColumnModel.AddColumn(
                InfControlMeasures.EventEnd.ToString(),
                InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.EventEnd),
                DataAttributeTypes.dtDate).SetEditableDate().SetWidth(100);

            gp.ColumnModel.AddColumn(
                InfControlMeasures.Violation.ToString(),
                InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.Violation),
                DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(300).SetMaxLengthEdior(2000);

            gp.ColumnModel.AddColumn(
                InfControlMeasures.ResultActivity.ToString(),
                InfControlMeasuresHelpers.InfControlMeasuresNameMapping(InfControlMeasures.ResultActivity),
                DataAttributeTypes.dtString).SetNullable().SetEditableString().SetWidth(400).SetMaxLengthEdior(2000);

            var gridFilters = new GridFilters
                {
                    Local = true
                };

            gridFilters.Filters.Add(new NumericFilter { DataIndex = InfControlMeasures.ID.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = InfControlMeasures.Supervisor.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = InfControlMeasures.Topic.ToString() });
            gridFilters.Filters.Add(new DateFilter { DataIndex = InfControlMeasures.EventBegin.ToString() });
            gridFilters.Filters.Add(new DateFilter { DataIndex = InfControlMeasures.EventEnd.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = InfControlMeasures.Violation.ToString() });
            gridFilters.Filters.Add(
                new StringFilter
                    {
                        DataIndex = InfControlMeasures.ResultActivity.ToString()
                    });

            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }
    }
}