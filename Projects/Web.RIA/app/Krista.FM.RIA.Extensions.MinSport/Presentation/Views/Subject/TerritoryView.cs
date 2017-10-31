using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.MinSport.MinSportUtils;
using Krista.FM.RIA.Extensions.MinSport.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Views.Subject
{
    public class TerritoryView : View
    {
        private const string StoreName = "dsTerritory";
        private const string GridName = "gpTerritory";
        private const string WindowName = "wndTerritory";
        private const string FormName = "formPanel1";
        private const string PopulationStoreName = "dsPopulation";
        private const string PopulationGridName = "gpPopulation";
        private readonly Dictionary<string, string> baseParamsList;
        
        private ViewPage page;

        public TerritoryView()
        {
            baseParamsList = new Dictionary<string, string>();
        }

        public override List<Component> Build(ViewPage viewPage)
        {
            page = viewPage;

            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptInclude("TerritoryView", Resource.GeneralFunctionJS);
            }

            var storeTerritoryCb = SportUtils.CreateStoreForComboBox("Combo", "LoadTerritorySubject", "dsTerritoryCB");
            page.Controls.Add(storeTerritoryCb);
            var storeTypeTerritoryCb = SportUtils.CreateStoreForComboBox("Combo", "LoadTerritorialPartitionType", "dsTypeTerritoryCB");
            page.Controls.Add(storeTypeTerritoryCb);

            var viewport = new Viewport
            {
                ID = "viewport",
                Items =
                {
                    new BorderLayout { Center = { Items = { CreateCenterPanel() } } }
                }
            };

            return new List<Component> { viewport, CreateTerritoryWindow() };
        }

        private Panel CreateCenterPanel()
        {
            var selectSubjectControl = new SelectSubjectControl
            {
                ScriptReloadComponents = "{0}.reload(); {1}.reload();".FormatWith(StoreName, PopulationStoreName)
            };
            var mainCenterPanel = new Panel { Border = false, Layout = "form" };

            mainCenterPanel.Items.Add(selectSubjectControl.Build(page));

            baseParamsList.Add("territoryId", "subjectsComboBox.getSelectedItem().value");

            var gridTerritory = new BaseGridControl
            {
                GridName = GridName,
                StoreName = StoreName,
                WindowName = WindowName,
                FormName = FormName,
                JsonReaderFields = { "ID", "Name", "ParentID", "ShortName", "TypeID", "TypeName" },
                Columns =
                {
                    new Column { DataIndex = "Name", Header = "Территория", Width = 300 }
                },
                BaseParams = baseParamsList,
                ControllerName = "Territory"
            };

            mainCenterPanel.Items.Add(gridTerritory.Build(page));
            return mainCenterPanel;
        }

        private Component CreateTerritoryWindow()
        {
            var windowTerritory = new BaseWindowControl
            {
                GridName = GridName,
                WindowName = WindowName,
                FormName = FormName,
                Title = "Карточка муниципального образования",
                Width = 600,
                Height = 400,
                FormPanelLabelWidth = 150,
                ControllerName = "Territory",
                FormPanelComponents =
                {
                    new ComboBox 
                    { 
                        ID = "cbSubjects", StoreID = "dsTerritoryCB", FieldLabel = "Субъект РФ", 
                        AnchorHorizontal = "100%", ValueField = "Value", DisplayField = "Text", DataIndex = "ParentID", 
                        Mode = DataLoadMode.Local, AllowBlank = false 
                    },
                    new TextField { ID = "tfName", FieldLabel = "Название", AnchorHorizontal = "100%", DataIndex = "Name", AllowBlank = false },
                    new TextField { ID = "tfShortName", FieldLabel = "Краткое название", AnchorHorizontal = "100%", DataIndex = "ShortName" },
                    new ComboBox 
                    { 
                        ID = "cbTypeTerritory", StoreID = "dsTypeTerritoryCB", FieldLabel = "Вид территории", 
                        AnchorHorizontal = "100%", ValueField = "Value", DisplayField = "Text", DataIndex = "TypeID", 
                        Mode = DataLoadMode.Local, AllowBlank = false 
                    },
                    CreatePeoplePopulationGrid()
                }
            };
            return windowTerritory.Build(page)[0];
        }

        private Component CreatePeoplePopulationGrid()
        {
            var peoplePopulationControl = new MinSportGridControl
            {
                GridName = PopulationGridName,
                StoreName = PopulationStoreName,
                JsonReaderFields = { "ID", "YearDayUNV", "Val", "Territory" },
                HideEditColumn = true,
                Title = "Численность населения",
                Columns =
                {
                    new Column { DataIndex = "YearDayUNV", Header = "Год", Width = 150 },
                    new Column { DataIndex = "Val", Header = "Численность населения", Width = 200, Editable = true }
                },
                BaseParams = baseParamsList,
                ControllerName = "PeoplePopulation"
            };

            return peoplePopulationControl.Build(page)[0];
        }
    }
}
