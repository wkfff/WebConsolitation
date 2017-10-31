using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.MinSport.MinSportUtils;
using Krista.FM.RIA.Extensions.MinSport.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Views.Propaganda
{
    public class PrintsView : View
    {
        private const string StoreName = "dsPrints";
        private const string GridName = "gpPrints";
        private const string WindowName = "wndPrints";
        private const string FormName = "formPanel1";
        private const string ViewName = "PrintsView";

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock(ViewName, Resource.GeneralFunctionJS);
            }

            var listParamsForTerritory = new Dictionary<string, string>();
            listParamsForTerritory.Add("parentId", "subjectsComboBox.getSelectedItem().value");

            var parametersListForKind = new Dictionary<string, string>();
            parametersListForKind.Add("beginCode", "100");
            parametersListForKind.Add("endCode", "199");

            var storeTerritoryCb = SportUtils.CreateStoreForComboBox("Combo", "LoadTerritorySubjectAndSettlement", "dsTerritoryCB", false, listParamsForTerritory);
            page.Controls.Add(storeTerritoryCb);

            var storeKindCb = SportUtils.CreateStoreForComboBox("Combo", "LoadMassMediaKind", "dsKindCB", true, parametersListForKind);
            page.Controls.Add(storeKindCb);

            var storeTargetGroupCb = SportUtils.CreateStoreForComboBox("Combo", "LoadTargetGroup", "dsTargetGroupCB");
            page.Controls.Add(storeTargetGroupCb);

            var viewport = new Viewport
            {
                ID = "viewport",
                Items =
                {
                    new BorderLayout { Center = { Items = { CreateCenterPanel(page) } } }
                }
            };

            return new List<Component> { viewport, CreatePrintsWindow(page) };
        }

        private Panel CreateCenterPanel(ViewPage page)
        {
            var selectSubjectControl = new SelectSubjectControl();
            selectSubjectControl.ScriptReloadComponents = "{0}.reload(); dsTerritoryCB.reload(); ".FormatWith(StoreName);
            var mainCenterPanel = new Panel { Border = false, Layout = "form" };

            mainCenterPanel.Items.Add(selectSubjectControl.Build(page));

            var baseParamsList = new Dictionary<string, string>();
            baseParamsList.Add("territoryId", "subjectsComboBox.getSelectedItem().value");
            baseParamsList.Add("beginCode", "100");
            baseParamsList.Add("endCode", "199");

            var gridTerritory = new BaseGridControl
            {
                GridName = GridName,
                StoreName = StoreName,
                WindowName = WindowName,
                FormName = FormName,
                JsonReaderFields =
                    {
                        "ID",
                        "KindName",
                        "TerritoryName",
                        "MediaName",
                        "TerritoryID",
                        "KindID",
                        "TargetGroupID",
                        "OtherName"
                    },
                Columns =
                    {
                        new Column { Header = "Субъект РФ", DataIndex = "TerritoryName", Width = 300 },
                        new Column { Header = "Вид издания", DataIndex = "KindName", Width = 200 },
                        new Column { Header = "Название", DataIndex = "MediaName", Width = 200 }
                    },
                BaseParams = baseParamsList,
                ControllerName = "Prints"
            };

            mainCenterPanel.Items.Add(gridTerritory.Build(page));
            
            return mainCenterPanel;
        }

        private Component CreatePrintsWindow(ViewPage page)
        {
            var windowPrints = new BaseWindowControl
            {
                GridName = GridName,
                WindowName = WindowName,
                FormName = FormName,
                Title = "Карточка периодического печатного издания",
                Width = 500,
                Height = 500,
                ControllerName = "Prints",
                FormPanelComponents =
                {
                    new ComboBox 
                    { 
                        ID = "cbTerritoryID", StoreID = "dsTerritoryCB", FieldLabel = "Территория", 
                        DisplayField = "Text", ValueField = "Value", AnchorHorizontal = "100%", DataIndex = "TerritoryID",
                        Mode = DataLoadMode.Local, AllowBlank = false, LabelWidth = 300 
                    },
                    new TextField 
                    { 
                        ID = "tfMediaName", FieldLabel = "Название", AnchorHorizontal = "100%", 
                        DataIndex = "MediaName", AllowBlank = false, LabelWidth = 300 
                    },
                    new TextField 
                    { 
                        ID = "tfOtherName", FieldLabel = "Краткое название", AnchorHorizontal = "100%",
                        DataIndex = "OtherName", LabelWidth = 300 
                    },
                    new ComboBox 
                    { 
                        ID = "cbTargetGroupID", StoreID = "dsTargetGroupCB", FieldLabel = "Целевая аудитория",
                        DisplayField = "Text", ValueField = "Value", AnchorHorizontal = "100%", DataIndex = "TargetGroupID",
                        Mode = DataLoadMode.Local, AllowBlank = false, LabelWidth = 300 
                    },
                    new ComboBox 
                    { 
                        ID = "cbKindID", StoreID = "dsKindCB", FieldLabel = "Вид издания", DisplayField = "Text",
                        ValueField = "Value", AnchorHorizontal = "100%", DataIndex = "KindID", Mode = DataLoadMode.Local, 
                        AllowBlank = false, LabelWidth = 300 
                    }
                }
            };
            return windowPrints.Build(page)[0];
        }
    }
}