using System.Collections.Generic;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controls
{
    /// <summary>
    /// Панель параметров документа(заголовок)
    /// </summary>
    public sealed class ParamDocPanelControl : Control
    {
        private readonly string paramDocFormId;

        public ParamDocPanelControl(int docId, Toolbar toolBar = null)
        {
            DocId = docId.ToString();
            ParamDocToolBar = toolBar;

            paramDocFormId = "frmParamDoc" + DocId;

            string paramDocStoreId = "dsParamDoc" + DocId;
            ParamDocStore = StoreExtensions.StoreCreateDefault(paramDocStoreId, true, UiBuilders.GetControllerID<ParameterDocController>(), "Read", "Update");
            ParamDocStore.SetBaseParams("itemId", DocId, ParameterMode.Value);
            ParamDocStore.AddFieldsByClass(new ParameterDocViewModel());
            ParamDocStore.Listeners.Load.Handler = @"{0}.getForm().loadRecord( {1}.getAt(0) );".FormatWith(paramDocFormId, paramDocStoreId);
        }
        
        public string DocId { get; set; }

        public Toolbar ParamDocToolBar { get; set; }

        public Store ParamDocStore { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(ParamDocStore);
            Component paramDocPanel = GetParameterDocPanel(ParamDocStore, paramDocFormId, ParamDocToolBar);

            return new List<Component> { paramDocPanel };
        }

        public Component BuildComponent(ViewPage page)
        {
            return Build(page)[0];
        }

        private Component GetParameterDocPanel(
           Store store,
           string formId,
           Toolbar toolBar = null)
        {
            var form =
                new FormPanel
                {
                    Title = @"Параметры документа",
                    ID = formId,
                    Border = false,
                    Height = 300,
                    Collapsible = true,
                    LabelWidth = 180,
                    LabelSeparator = string.Empty,
                    LabelPad = 10,
                    Padding = 6,
                    BodyCssClass = "x-window-mc",
                    CssClass = "x-window-mc",
                    DefaultAnchor = "99%"
                };

            if (toolBar != null)
            {
                if (toolBar.ID == "StateToolBar")
                {
                    store.Listeners.Load.AddAfter("ChangeState(records[0].data.RefSostID, records[0].data.CloseDate);");
                }

                form.TopBar.Add(toolBar);
            }

            form.Items.Add(
                new NumberField
                {
                    ID = "ID",
                    DataIndex = "ID",
                    FieldLabel = @"ШапкаДокумента/Ссылка",
                    ReadOnly = true,
                    Hidden = true
                });

            form.Items.Add(
                new TextField
                {
                    ID = "Note",
                    DataIndex = "Note",
                    FieldLabel = @"Комментарий",
                    ReadOnly = true,
                    Hidden = true
                });

            form.Items.Add(
                new Checkbox
                {
                    ID = "PlanThreeYear",
                    DataIndex = "PlanThreeYear",
                    FieldLabel = @"ПланНаТриГода",
                    ReadOnly = true,
                    Hidden = true
                });

            form.Items.Add(
                new NumberField
                {
                    ID = "RefPartDocID",
                    DataIndex = "RefPartDocID",
                    FieldLabel = @"ТипДокумента/Ссылка",
                    ReadOnly = true,
                    Hidden = true
                });

            form.Items.Add(
                new TextField
                {
                    ID = "RefUchrID_RefOrgPPOID_Name",
                    DataIndex = "RefUchrID_RefOrgPPOID_Name",
                    FieldLabel = @"ППО, создавшее учреждение:",
                    ReadOnly = true
                });

            form.Items.Add(
                new TextField
                {
                    ID = "RefUchrID_RefOrgGRBSID_Name",
                    DataIndex = "RefUchrID_RefOrgGRBSID_Name",
                    FieldLabel = @"ГРБС:",
                    ReadOnly = true
                });

            form.Items.Add(
                new TextField
                {
                    ID = "RefUchrID_Name",
                    DataIndex = "RefUchrID_Name",
                    FieldLabel = @"Наименование учреждения:",
                    ReadOnly = true
                });

            form.Items.Add(
                new TextField
                {
                    ID = "INN",
                    DataIndex = "INN",
                    FieldLabel = @"ИНН:",
                    ReadOnly = true
                });

            form.Items.Add(
                new TextField
                {
                    ID = "RefUchrID_RefTypYcID_Name",
                    DataIndex = "RefUchrID_RefTypYcID_Name",
                    FieldLabel = @"Тип учреждения:",
                    ReadOnly = true
                });

            form.Items.Add(
                new NumberField
                {
                    ID = "RefYearFormID",
                    DataIndex = "RefYearFormID",
                    FieldLabel = @"Год формирования:",
                    ReadOnly = true
                });

            form.Items.Add(
                new TextField
                {
                    ID = "PlanThreeYear_Name",
                    DataIndex = "PlanThreeYear_Name",
                    FieldLabel = @"Период планирования:",
                    ReadOnly = true
                });

            form.Items.Add(
                new Hidden
                {
                    ID = "RefSostID",
                    DataIndex = "RefSostID"
                });

            form.Items.Add(
                new TextField
                {
                    ID = "OpeningDate",
                    DataIndex = "OpeningDate",
                    FieldLabel = @"Дата открытия:",
                    ReadOnly = true
                });

            form.Items.Add(
                new TextField
                {
                    ID = "CloseDate",
                    DataIndex = "CloseDate",
                    FieldLabel = @"Дата закрытия:",
                    ReadOnly = true
                });

            form.Items.Add(
                new TextField
                {
                    ID = "RefSostID_Name",
                    DataIndex = "RefSostID_Name",
                    FieldLabel = @"Состояние документа:",
                    ReadOnly = true,
                    AutoWidth = true
                });

            return form;
        }
    }
}
