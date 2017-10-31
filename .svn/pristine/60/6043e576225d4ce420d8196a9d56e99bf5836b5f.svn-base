using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.MinSport.MinSportUtils;
using Krista.FM.RIA.Extensions.MinSport.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Views.Subject
{
    public class PersonCuratorView : View
    {
        private const string StoreName = "dsPersonCurator";
        private const string GridName = "gpPersonCurator";
        private const string WindowName = "wndPersonCurator";
        private const string FormName = "formPanel1";

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock("PersonCuratorView", Resource.GeneralFunctionJS);
            }

            Store storeTerritoryCb = SportUtils.CreateStoreForComboBox("Combo", "LoadTerritorySubject", "dsTerritoryCB");
            page.Controls.Add(storeTerritoryCb);

            var viewport = new Viewport
            {
                ID = "viewport",
                Items =
                {
                    new BorderLayout { Center = { Items = { CreateCenterPanel(page) } } }
                }
            };

            return new List<Component> { viewport, CreatePersonCuratorWindow(page) };
        }

        private Panel CreateCenterPanel(ViewPage page)
        {
            var selectSubjectControl = new SelectSubjectControl();
            selectSubjectControl.ScriptReloadComponents = "{0}.reload(); ".FormatWith(StoreName);
            var mainCenterPanel = new Panel { Border = false, Layout = "form" };

            mainCenterPanel.Items.Add(selectSubjectControl.Build(page));

            var baseParamsList = new Dictionary<string, string>();
            baseParamsList.Add("territoryId", "subjectsComboBox.getSelectedItem().value");

            var gridTerritory = new BaseGridControl
            {
                GridName = GridName,
                StoreName = StoreName,
                WindowName = WindowName,
                FormName = FormName,
                JsonReaderFields = { "ID", "FirstName", "Surname", "Patronimyc", "JobPosition", "Telephone", "Email", "territoryID", "territoryName" },
                Columns =
                {
                    new Column { DataIndex = "territoryName", Header = "Субъект РФ", Width = 300 },
                    new Column { DataIndex = "Surname", Header = "Фамилия", Width = 200 },
                    new Column { DataIndex = "FirstName", Header = "Имя", Width = 200 },
                    new Column { DataIndex = "Patronimyc", Header = "Отчество", Width = 200 },
                    new Column { DataIndex = "JobPosition", Header = "Должность", Width = 200 }
                },
                BaseParams = baseParamsList,
                ControllerName = "PersonCurator"
            };

            mainCenterPanel.Items.Add(gridTerritory.Build(page));
            return mainCenterPanel;
        }

        private Component CreatePersonCuratorWindow(ViewPage page)
        {
            var baseWindow = new BaseWindowControl
            {
                GridName = GridName,
                WindowName = WindowName,
                FormName = FormName,
                Title = "Карточка куратора",
                Width = 500,
                Height = 350,
                FormPanelLabelWidth = 150,
                ControllerName = "PersonCurator",
                FormPanelComponents =
                {
                    new ComboBox
                    {
                        ID = "cbTerritory", StoreID = "dsTerritoryCB", FieldLabel = "Субъект РФ",
                        DisplayField = "Text", ValueField = "Value", AnchorHorizontal = "100%",
                        DataIndex = "territoryID", Mode = DataLoadMode.Local, AllowBlank = false
                    },
                    new TextField
                    {
                        ID = "tfSurname", FieldLabel = "Фамилия", AnchorHorizontal = "100%",
                        DataIndex = "Surname", AllowBlank = false
                    },
                    new TextField
                    {
                        ID = "tfFirstname", FieldLabel = "Имя", AnchorHorizontal = "100%",
                        DataIndex = "FirstName"
                    },
                    new TextField
                    {
                        ID = "tfPatronimyc", FieldLabel = "Отчество", AnchorHorizontal = "100%",
                        DataIndex = "Patronimyc"
                    },
                    new TextField
                    {
                        ID = "tfJobPosition", FieldLabel = "Должность", AnchorHorizontal = "100%", 
                        DataIndex = "JobPosition"
                    },
                    new TextField
                    {
                        ID = "tfTelephone", FieldLabel = "Телефон", AnchorHorizontal = "100%",
                        DataIndex = "Telephone"
                    },
                    new TextField
                    {
                        ID = "tfEmail", FieldLabel = "E-mail", AnchorHorizontal = "100%",
                        DataIndex = "Email"
                    },
                    new TextArea
                    { 
                        ID = "taInfo", FieldLabel = "Информация", AnchorHorizontal = "100%"
                    }
                }
            };
            return baseWindow.Build(page)[0];
        } 
    }
}