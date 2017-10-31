using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.MinSport.MinSportUtils;
using Krista.FM.RIA.Extensions.MinSport.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Views.Oiv
{
    public class OivView : View
    {
        private const string WindowName = "wndOIV";
        private const string GridName = "gpOIV";
        private const string StoreName = "dsOIV";
        private const string FormName = "formPanel1";

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock("OivView", Resource.GeneralFunctionJS);
            } 

            var storeLocalityType = SportUtils.CreateStoreForComboBox("Combo", "LoadLocalityType", "dsLocalityType");
            page.Controls.Add(storeLocalityType);

            var viewport = new Viewport
            {
                ID = "viewport",
                Items =
                {
                    new BorderLayout { Center = { Items = { CreateCenterPanel(page) } } }
                }
            };

            return new List<Component> { viewport, CreateOivWindow(page) };
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
                JsonReaderFields =
                    {
                        "ID", "AuthorityName", "territoryID", "territoryName", "LeaderSurname", "LeaderFirstName",
                        "LeaderPatronymic", "LeaderJobPosition", "LeaderInformation", "OtherName", "Telephone",
                        "Email", "Website", "PostalCode", "Locality", "House", "BuildingK", "BuildingS",
                        "LocalityTypeID", "LocalityTypeName"
                    },
                Columns =
                    {
                        new Column { Header = "Территория", DataIndex = "territoryName", Width = 400 },
                        new Column { Header = "ОИВ", DataIndex = "AuthorityName", Width = 300 }
                    },
                BaseParams = baseParamsList,
                ControllerName = "Oiv"
            };
            mainCenterPanel.Items.Add(gridTerritory.Build(page));

            return mainCenterPanel;
        }

        private Component CreateOivWindow(ViewPage page)
        {
            var windowInternet = new BaseWindowControl
            {
                GridName = GridName,
                WindowName = WindowName,
                FormName = FormName,
                Title = "Карточка ОИВ",
                Width = 700,
                Height = 500,
                ControllerName = "Oiv",
                FormPanelComponents =
                    {
                        new TextField
                        {
                            ID = "tfAuthorityName",
                            FieldLabel = "Название ОИВ",
                            AnchorHorizontal = "95%",
                            DataIndex = "AuthorityName",
                            AllowBlank = false
                        },
                        new TextField
                        {
                            ID = "tfOtherName",
                            FieldLabel = "Краткое название",
                            AnchorHorizontal = "95%",
                            DataIndex = "OtherName"
                        },
                        CreateFieldSetCoordinates(),
                        CreateFieldSetManager(),
                        CreateFieldSetStructureOiv()
                    }
            };
            return windowInternet.Build(page)[0];
        }

        private FieldSet CreateFieldSetCoordinates()
        {
            var fieldSetCoordinates = new FieldSet
            {
                ID = "fieldSet1",
                Title = "Координаты",
                Collapsible = true,
                LabelAlign = LabelAlign.Top,
                Layout = "form",
                Items =
                    {
                        new CompositeField
                        {
                            AnchorHorizontal = "95%",
                            Items =
                                {
                                    new TextField
                                    {
                                        ID = "tfPostalCode",
                                        Note = "индекс",
                                        NoteAlign = NoteAlign.Top,
                                        Width = 100,
                                        DataIndex = "PostalCode"
                                    },
                                    new ComboBox
                                    {
                                        ID = "cbLocalityType",
                                        StoreID = "dsLocalityType",
                                        Width = 150,
                                        DataIndex = "LocalityTypeID",
                                        ValueField = "Value",
                                        DisplayField = "Text",
                                        Note = "населенный пункт",
                                        NoteAlign = NoteAlign.Top,
                                    },
                                    new TextField
                                    {
                                        ID = "tfLocality",
                                        Note = "&nbsp",
                                        NoteAlign = NoteAlign.Top,
                                        Flex = 1,
                                        DataIndex = "Locality"
                                    }
                                }
                        },
                        new CompositeField
                        {
                            AnchorHorizontal = "95%",
                            NoteAlign = NoteAlign.Top,
                            Items =
                                {
                                    new TextField
                                    {
                                        ID = "tfStreet",
                                        Note = "улица",
                                        NoteAlign = NoteAlign.Top,
                                        Flex = 1,
                                        DataIndex = "Street"
                                    },
                                    new NumberField
                                    {
                                        ID = "nfHouse",
                                        Note = "дом",
                                        NoteAlign = NoteAlign.Top,
                                        Width = 100,
                                        DataIndex = "House"
                                    },
                                    new TextField
                                    {
                                        ID = "tfBuildingK",
                                        Note = "корпус",
                                        NoteAlign = NoteAlign.Top,
                                        Width = 100,
                                        DataIndex = "BuildingK"
                                    },
                                    new TextField
                                    {
                                        ID = "tfBuildingS",
                                        Note = "строение",
                                        NoteAlign = NoteAlign.Top,
                                        Width = 100,
                                        DataIndex = "BuildingS"
                                    }
                                }
                        },
                        new CompositeField
                        {
                            AnchorHorizontal = "95%",
                            NoteAlign = NoteAlign.Top,
                            Items =
                                {
                                    new TextField
                                    {
                                        ID = "tfTelephone",
                                        Note = "телефон",
                                        NoteAlign = NoteAlign.Top,
                                        Width = 100,
                                        DataIndex = "Telephone"
                                    },
                                    new TextField
                                    {
                                        ID = "tfEmail",
                                        Note = "E-mail",
                                        NoteAlign = NoteAlign.Top,
                                        Width = 100,
                                        DataIndex = "Email"
                                    },
                                    new TextField
                                    {
                                        ID = "tfWebsite",
                                        Note = "web-сайт",
                                        NoteAlign = NoteAlign.Top,
                                        Flex = 1,
                                        DataIndex = "Website"
                                    }
                                }
                        }
                    }
            };

            return fieldSetCoordinates;
        }

        private FieldSet CreateFieldSetManager()
        {
            var fieldSetManager = new FieldSet
            {
                ID = "fieldSet2",
                Collapsible = true,
                Collapsed = true,
                AnchorHorizontal = "100%",
                Layout = "form",
                Title = "Руководитель",
                Items =
                    {
                        new TextField
                        {
                            ID = "tfLeaderSurname",
                            FieldLabel = "Фамилия",
                            AnchorHorizontal = "100%",
                            LabelWidth = 200,
                            DataIndex = "LeaderSurname",
                            AllowBlank = false
                        },
                        new TextField
                        {
                            ID = "tfLeaderFirstName",
                            FieldLabel = "Имя",
                            AnchorHorizontal = "100%",
                            LabelWidth = 200,
                            DataIndex = "LeaderFirstName",
                            AllowBlank = false
                        },
                        new TextField
                        {
                            ID = "tfLeaderPatronymic",
                            FieldLabel = "Отчество",
                            AnchorHorizontal = "100%",
                            LabelWidth = 200,
                            DataIndex = "LeaderPatronymic"
                        },
                        new TextField
                        {
                            ID = "tfLeaderJobPosition",
                            FieldLabel = "Должность",
                            AnchorHorizontal = "100%",
                            LabelWidth = 200,
                            DataIndex = "LeaderJobPosition"
                        },
                        new TextArea
                        {
                            ID = "tfLeaderInformation",
                            FieldLabel = "Информация",
                            AnchorHorizontal = "100%",
                            Height = 100,
                            AutoScroll = true,
                            LabelWidth = 200,
                            DataIndex = "LeaderInformation"
                        }
                    }
            };
            return fieldSetManager;
        }

        private FieldSet CreateFieldSetStructureOiv()
        {
            var fieldSetStructureOiv = new FieldSet
            {
                ID = "fieldSet3",
                Collapsible = true,
                Collapsed = true,
                AnchorHorizontal = "100%",
                Title = "Структура ОИВ (для регионального ОИВ)",
            };

            return fieldSetStructureOiv;
        }
    }
}
