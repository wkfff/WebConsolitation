using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using View = Krista.FM.RIA.Core.Gui.View;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Views
{
    public class ProjectDetailView : View
    {
        public int? ProjId
        {
            get { return Params["projId"] == "null" ? null : (int?)Convert.ToInt32(Params["projId"]); }
        }

        public int RefPartId
        {
            get { return Convert.ToInt32(Params["refPart"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            if (!ExtNet.IsAjaxRequest)
            {
                resourceManager.RegisterClientScriptBlock("ProjectDetailView", Resource.ProjectDetailView);
            }
            
            // Регистрируем событие на закрытие вкладки и на Переключение вкладок
            resourceManager.AddScript(@"
var tab = parent.ProjectsTabPanel.getComponent('ProjectDetail_{0}');
if (typeof(tab.events.beforeclose) == 'object'){{
    tab.events.beforeclose.clearListeners();
}}
tab.addListener('beforeclose', onTabCloseEventHandler);

var detailTab = Ext.getCmp('ProjectDetailTabPanel');
if (detailTab != undefined){{
  detailTab.addListener('beforetabchange', onTabChangeEventHandler);
}}
".FormatWith(ProjId == null ? "null" : ProjId.ToString()));
            
            page.Controls.Add(GetProjectAttributeStore());

            if (RefPartId == (int)InvProjPart.Part1)
            {
                page.Controls.Add(GetSumInvestPlanStore());
            }

            var view = new Viewport
            {
                ID = "viewportMain",
                AutoScroll = false,
                Layout = LayoutType.Row.ToString(),
                Items = 
                {
                    new Panel { ID = "ToolBarPanel", TopBar = { CreateToolbar() }, Height = 27, Border = false },
                    CreateProjectAttributesPanel()
                }
            };

            if (RefPartId == (int)InvProjPart.Part1)
            {
                view.Items.Add(new Button { ID = "btSaveNewProject", Text = "Сохранить и перейти к детализации", Height = 20, Icon = Icon.BookmarkGo, MaxWidth = 250, Visible = !(ProjId > 0), Listeners = { Click = { Handler = "SaveProjectAttributes();" } } });
                view.Items.Add(new Panel { ID = "ProjectPanel", RowHeight = 0.3, Border = false, Items = { CreateProjectDetailTabPanel() } });

                if (ProjId != null)
                {
                    view.AddScript("InvestTab.reload();");
                }
            }

            return new List<Component> { view, GetTerritoryLookupWindow(), GetOKVEDLookupWindow() };
        }
        
        private Toolbar CreateToolbar()
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "dsProjectDetail.reload();{0}".FormatWith(RefPartId == (int)InvProjPart.Part1 ? "dsSumInvestPlan.reload();" : String.Empty) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить",
                Listeners = { Click = { Handler = "SaveProjectAttributes();" } }
            });

            return toolbar;
        }

        private List<Component> CreateProjectAttributesPanel()
        {
            var form = new FormPanel
                           {
                               ID = "ProjectAttrForm",
                               Border = false,
                               Url = "/ProjectDetail/Save",
                               RowHeight = 0.7,
                               AutoScroll = true,
                               ////AutoHeight = true,
                               LabelWidth = 250,
                               LabelSeparator = String.Empty,
                               LabelPad = 10,
                               Padding = 6,
                               DefaultAnchor = "95%",
                               MonitorValid = true,
                           };
            form.Listeners.ClientValidation.Handler = @"
btnSave.setDisabled(!valid);
btnSave.setTooltip(valid ? 'Сохранить' : 'В карточке не все поля заполнены, либо имеют некорректные значения!');
var btnSaveNew = Ext.getCmp('btSaveNewProject');
if (btnSaveNew !=undefined) {
   btSaveNewProject.setDisabled(!valid);
}";
            
            int maxWidth = 500;

            form.Items.Add(new TextField { ID = "ID", DataIndex = "ID", Hidden = true });

            form.Items.Add(new TextField { ID = "Name", FieldLabel = "Наименование проекта", DataIndex = "Name", MaxWidth = maxWidth, AllowBlank = false });
            form.Items.Add(new TextField { ID = "Code", FieldLabel = "Идентификационный номер", DataIndex = "Code", MaxWidth = maxWidth, AllowBlank = RefPartId != (int)InvProjPart.Part1 });

            var comboboxRefPart = new ComboBox
            {
                ID = "comboRefPart",
                FieldLabel = "Включен в:",
                Items =
                    {
                      new ListItem { Value = "1", Text = "Раздел 1" },
                      new ListItem { Value = "2", Text = "Раздел 2" },
                    },
                Editable = false,
                ReadOnly = true,
                HideMode = HideMode.Visibility,
                TriggerAction = TriggerAction.All,
                Mode = DataLoadMode.Local,
                Resizable = false,
                SelectOnFocus = false,
                DataIndex = "RefPartId",
                MaxWidth = maxWidth,
                AllowBlank = false
            };
            form.Items.Add(comboboxRefPart);

            var lookupRefTerritory = new TriggerField
            {
                ID = "lfRefTerritoryName",
                MaxWidth = maxWidth,
                Editable = false,
                FieldLabel = RefPartId == (int)InvProjPart.Part1 ? "Место реализации" : "Предполагаемое место реализации",
                DataIndex = "RefTerritoryName",
                AllowBlank = false,
                TriggerIcon = TriggerIcon.Ellipsis,
                Listeners = { TriggerClick = { Handler = "wTerritoryLookup.show('lfRefTerritoryName');" } }
            };
            form.Items.Add(lookupRefTerritory);
            form.Items.Add(new TextField { ID = "RefTerritoryId", Hidden = true, DataIndex = "RefTerritoryId" });

            if (RefPartId != (int)InvProjPart.Part1)
            {
                form.Items.Add(new TextField { ID = "Goal", FieldLabel = "Цель проекта", DataIndex = "Goal", MaxWidth = maxWidth, AllowBlank = false });
            }

            form.Items.Add(new TextField { ID = "InvestorName", FieldLabel = "Инвестор:", DataIndex = "InvestorName", MaxWidth = maxWidth, AllowBlank = false });
            
            var group1 = new FieldSet
            {
                Title = "Контакты инвестора",
                Collapsible = true,
                Collapsed = false,
                AutoHeight = true,
                Border = false,
                StyleSpec = "margin-bottom: 10px; border-left: 0px; border-right: 0px; border-bottom: 0px;",
                Layout = LayoutType.Form.ToString(),
                FormGroup = true,
                LabelWidth = 250,
                LabelSeparator = String.Empty,
                LabelPad = 10,
                Padding = 6,
                DefaultAnchor = "0"
            };
            form.Items.Add(group1);

            group1.Items.Add(new TextField { ID = "LegalAddress", FieldLabel = "юридический адрес", DataIndex = "LegalAddress", MaxWidth = maxWidth, AllowBlank = false });
            group1.Items.Add(new TextField { ID = "MailingAddress", FieldLabel = "почтовый адрес", DataIndex = "MailingAddress", MaxWidth = maxWidth });
            group1.Items.Add(new TextField { ID = "Email", FieldLabel = "электронная почта", DataIndex = "Email", MaxWidth = maxWidth, AllowBlank = false });
            group1.Items.Add(new TextField { ID = "Phone", FieldLabel = "телефон", DataIndex = "Phone", MaxWidth = maxWidth, AllowBlank = false });
            group1.Items.Add(new TextField { ID = "Contact", FieldLabel = "контактное лицо", DataIndex = "Contact", MaxWidth = maxWidth, AllowBlank = false });

            if (RefPartId == (int)InvProjPart.Part2)
            {
                form.Items.Add(new TextField { ID = "Study", FieldLabel = "Степень проработки проекта", DataIndex = "Study", MaxWidth = maxWidth, AllowBlank = true });
                form.Items.Add(new TextField { ID = "Effect", FieldLabel = "Предполагаемый полезный социально - экономический эффект от реализации проекта", DataIndex = "Effect", MaxWidth = maxWidth, AllowBlank = true });
            }

            if (RefPartId == (int)InvProjPart.Part1)
            {
                form.Items.Add(new TextField { ID = "Goal", FieldLabel = "Цель проекта", DataIndex = "Goal", MaxWidth = maxWidth, AllowBlank = false });

                form.Items.Add(new SpinnerField { ID = "RefBeginDateVal", FieldLabel = "Год начала реализации", DataIndex = "RefBeginDateVal", MaxWidth = maxWidth, AllowBlank = false });
                form.Items.Add(new SpinnerField { ID = "RefEndDateVal", FieldLabel = "Год окончания реализации", DataIndex = "RefEndDateVal", MaxWidth = maxWidth, AllowBlank = false });
            }
                
            var group2 = new FieldSet
            {
                Title = "Технико - экономические показатели:",
                Collapsible = true,
                Collapsed = false,
                AutoHeight = true,
                Border = false,
                StyleSpec = "margin-bottom: 10px; border-left: 0px; border-right: 0px; border-bottom: 0px;",
                Layout = LayoutType.Form.ToString(),
                FormGroup = true,
                LabelWidth = 250,
                LabelSeparator = String.Empty,
                LabelPad = 10,
                Padding = 6,
                DefaultAnchor = "0"
            };
            form.Items.Add(group2);

            group2.Items.Add(new TextField { ID = "ExpectedOutput", FieldLabel = "планируемые результаты", DataIndex = "ExpectedOutput", MaxWidth = maxWidth });
            group2.Items.Add(new TextField { ID = "Production", FieldLabel = "виды конечной продукции", DataIndex = "Production", MaxWidth = maxWidth });
            group2.Items.Add(new TextField { ID = "PaybackPeriod", FieldLabel = "срок окупаемости", DataIndex = "PaybackPeriod", MaxWidth = maxWidth });

            if (RefPartId == (int)InvProjPart.Part1)
            {
                form.Items.Add(new TextField { ID = "SumInvestPlan", FieldLabel = "Плановый объем инвестиций всего, млн.руб.", ReadOnly = true, MaxWidth = maxWidth });

                form.Items.Add(new TextField { ID = "DocBase", FieldLabel = "Основание для заключения инвестиционного соглашения", DataIndex = "DocBase", MaxWidth = maxWidth });
                form.Items.Add(new TextField { ID = "InvestAgreement", FieldLabel = "Инвестиционное соглашение", DataIndex = "InvestAgreement", MaxWidth = maxWidth });
                form.Items.Add(new TextField { ID = "AddMech", FieldLabel = "Использование дополнительных механизмов реализации", DataIndex = "AddMech", MaxWidth = maxWidth });
                form.Items.Add(new TextField { ID = "ExpertOpinion", FieldLabel = "Наличие экспертного заключения", DataIndex = "ExpertOpinion", MaxWidth = maxWidth });
            }

            var lookupRefOKVEDName = new TriggerField
            {
                ID = "lfRefOKVEDName",
                MaxWidth = maxWidth,
                Editable = false,
                FieldLabel = "Вид деятельности",
                DataIndex = "RefOKVEDName",
                AllowBlank = false,
                TriggerIcon = TriggerIcon.Ellipsis,
                Listeners = { TriggerClick = { Handler = "wOKVEDLookup.show('lfRefOKVEDName');" } }
            };
            form.Items.Add(lookupRefOKVEDName);
            form.Items.Add(new TextField { ID = "RefOKVEDId", Hidden = true, DataIndex = "RefOKVEDId" });

            var comboboxRefStatus = new ComboBox
            {
                ID = "comboRefStatus",
                FieldLabel = "Состояние",
                Items =
                    {
                      new ListItem { Value = "1", Text = "На редактировании" },
                      new ListItem { Value = "2", Text = "Исполняется" },
                      new ListItem { Value = "3", Text = "Исключен" },
                    },
                Editable = false,
                HideMode = HideMode.Visibility,
                TriggerAction = TriggerAction.All,
                Mode = DataLoadMode.Local,
                Resizable = false,
                SelectOnFocus = false,
                DataIndex = "RefStatusId",
                StyleSpec = "font-weight:bold;",
                MaxWidth = maxWidth,
                AllowBlank = false
            };
            comboboxRefStatus.Listeners.Select.Handler = @"
Exception.setVisible(this.value == 3);
Exception.allowBlank = (this.value != 3);
Exception.validate(); 
SetFieldsEditableOption(this.value == 1);
";

            form.Items.Add(comboboxRefStatus);

            form.Items.Add(new DateField
                                    {
                                        ID = "IncomingDate",
                                        FieldLabel = "Дата поступления сведений",
                                        DataIndex = "IncomingDate",
                                        MaxWidth = maxWidth,
                                        Format = "d.m.Y",
                                        AllowBlank = false
                                    });
            form.Items.Add(new DateField
                                    {
                                        ID = "RosterDate",
                                        FieldLabel = "Дата включения в реестр приоритетных инвестиционных проектов",
                                        DataIndex = "RosterDate",
                                        MaxWidth = maxWidth,
                                        Format = "d.m.Y"
                                    });

            form.Items.Add(new TextField { ID = "Exception", FieldLabel = "Причина исключения", DataIndex = "Exception", MaxWidth = maxWidth, Hidden = true });

            return new List<Component> { form };
        }

        private List<Component> CreateProjectDetailTabPanel()
        {
            var tabPanel = new TabPanel
            {
                ID = "ProjectDetailTabPanel",
                IDMode = IDMode.Explicit,
                Border = false,
                Disabled = !(ProjId > 0),
                ActiveTabIndex = 0,
                EnableTabScroll = true,
                MonitorResize = true,
                ////AutoHeight = true,
                LayoutOnTabChange = true,
                Closable = true
            };

            tabPanel.Items.Add(new Panel
            {
                ID = "InvestTab",
                Title = "Плановый объем инвестиций",
                AutoRender = false,
                AutoShow = false,
                AutoLoad =
                {
                    Url = "/View/InvProjInvestPlan",
                    Params =
                        {
                            new Parameter("projId", ProjId.ToString(), ParameterMode.Value),
                            new Parameter("projectInvestType", ((int)InvProjInvestType.Investment).ToString(), ParameterMode.Value)
                        },
                    Mode = LoadMode.IFrame,
                    ManuallyTriggered = true,
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });

            tabPanel.Items.Add(new Panel
            {
                ID = "GosTab",
                Title = "Плановые формы и объемы господдержки",
                AutoLoad =
                {
                    Url = "/View/InvProjInvestPlan",
                    Params =
                        {
                            new Parameter("projId", ProjId.ToString(), ParameterMode.Value),
                            new Parameter("projectInvestType", ((int)InvProjInvestType.GosSupport).ToString(), ParameterMode.Value)
                        },
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });

            tabPanel.Items.Add(new Panel
            {
                ID = "TargetRatingsTab",
                Title = "Целевые показатели",
                AutoLoad =
                {
                    Url = "/View/InvProjTargetRatings",
                    Params =
                        {
                            new Parameter("projId", ProjId.ToString(), ParameterMode.Value),
                        },
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });

            tabPanel.Items.Add(new Panel
            {
                ID = "VisualizationTab",
                Title = "Визуализация проекта",
                AutoLoad =
                {
                    Url = "/View/InvProjVisualization",
                    Params =
                        {
                            new Parameter("projId", ProjId.ToString(), ParameterMode.Value),
                        },
                    Mode = LoadMode.IFrame,
                    TriggerEvent = "show",
                    ShowMask = true,
                    MaskMsg = "Загрузка..."
                }
            });
            return new List<Component> { new FitLayout { Items = { tabPanel } } };
        }

        private Store GetProjectAttributeStore()
        {
            var store = new Store { ID = "dsProjectDetail", AutoLoad = true };
            store.SetHttpProxy("/ProjectDetail/Load");
            store.BaseParams.Add(new Parameter("projId", ProjId.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("refPartId", RefPartId.ToString(), ParameterMode.Value));
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Code"),
                    new RecordField("Name"),
                    new RecordField("InvestorName"),
                    new RecordField("LegalAddress"),
                    new RecordField("MailingAddress"),
                    new RecordField("Email"),
                    new RecordField("Phone"),
                    new RecordField("Contact"),
                    new RecordField("Goal"),
                    new RecordField("ExpectedOutput"),
                    new RecordField("Production"),
                    new RecordField("PaybackPeriod"),
                    new RecordField("DocBase"),
                    new RecordField("InvestAgreement"),
                    new RecordField("AddMech"),
                    new RecordField("ExpertOpinion"),
                    new RecordField("Study"),
                    new RecordField("Effect"),
                    new RecordField("RefStatus"),
                    new RecordField("IncomingDate", RecordFieldType.Date),
                    new RecordField("RosterDate", RecordFieldType.Date),
                    new RecordField("Exception"),
                    new RecordField("RefBeginDateVal"),
                    new RecordField("RefEndDateVal"),
                    new RecordField("RefTerritoryId"),
                    new RecordField("RefTerritoryName"),
                    new RecordField("RefStatusId"),
                    new RecordField("RefPartId"),
                    new RecordField("RefOKVEDId"),
                    new RecordField("RefOKVEDName"),
                }
            });
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка проектов', response.responseText);";
            store.Listeners.DataChanged.Handler = @"
var record = this.getAt(0) || {};
ProjectAttrForm.getForm().loadRecord(record);
ProjectAttrForm.validate();
comboRefStatus.fireEvent('select');
SetFieldsEditableOption(comboRefStatus.getValue()==1);
ResetDirtyAttributeOnFormItems(ProjectAttrForm);
";
            return store;
        }

        private Store GetSumInvestPlanStore()
        {
            var store = new Store { ID = "dsSumInvestPlan", AutoLoad = true };
            store.SetHttpProxy("/ProjectDetail/LoadSumInvestPlan");
            store.BaseParams.Add(new Parameter("projId", ProjId.ToString(), ParameterMode.Value));
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Value"),
                }
            });
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке планового объема инвестиций', response.responseText);";
            store.Listeners.DataChanged.Handler = @"
var record = this.getAt(0) || {};
if (record != undefined && record.data != undefined){
    SumInvestPlan.setValue(Ext.util.Format.number(record.data.Value,'0,0.00'));
    SumInvestPlan.originalValue = SumInvestPlan.getValue();
}
";
            return store;
        }

        private Window GetTerritoryLookupWindow()
        {
            var w = new Window();

            w.ID = "wTerritoryLookup";
            w.Title = "Выбор варианта";
            w.Width = 600;
            w.Height = 400;
            w.Hidden = true;

            w.AutoLoad.TriggerEvent = "show";
            w.AutoLoad.Url = "/Entity/Book?objectKey={0}".FormatWith(D_Territory_RF.Key);
            w.AutoLoad.Mode = LoadMode.IFrame;
            var btn = new Button { ID = "btTerritorySelect", Text = "Выбрать", Disabled = true };
            btn.Listeners.Click.Handler = @"
 var record = wTerritoryLookup.getBody().Extension.entityBook.selectedRecord;
 lfRefTerritoryName.setValue(record.data.NAME);
 RefTerritoryId.setValue(record.data.ID);
 #{wTerritoryLookup}.hide();";
            w.Buttons.Add(btn);

            // Установка обработчика выбора варианта
            w.Listeners.Update.Handler = @"
function rowSelected(record) { 
    wTerritoryLookup.getBody().Extension.entityBook.selectedRecord = record;
    btTerritorySelect.setDisabled(false); 
}
wTerritoryLookup.getBody().Extension.entityBook.onRowSelect = rowSelected;
";

            w.AddAfterClientInitScript("wTerritoryLookup.MetaData = {};");
            return w;
        }
        
        private Window GetOKVEDLookupWindow()
        {
            var w = new Window();

            w.ID = "wOKVEDLookup";
            w.Title = "Выбор варианта";
            w.Width = 600;
            w.Height = 400;
            w.Hidden = true;

            w.AutoLoad.TriggerEvent = "show";
            w.AutoLoad.Url = "/Entity/Book?objectKey={0}".FormatWith(D_OK_OKVED.Key);
            w.AutoLoad.Mode = LoadMode.IFrame;
            var btn = new Button { ID = "btOKVEDSelect", Text = "Выбрать", Disabled = true };
            btn.Listeners.Click.Handler = @"
 var record = wOKVEDLookup.getBody().Extension.entityBook.selectedRecord;
 lfRefOKVEDName.setValue(record.data.NAME);
 RefOKVEDId.setValue(record.data.ID);
 #{wOKVEDLookup}.hide();";
            w.Buttons.Add(btn);

            // Установка обработчика выбора варианта
            w.Listeners.Update.Handler = @"
function rowSelected(record) { 
    wOKVEDLookup.getBody().Extension.entityBook.selectedRecord = record;
    btOKVEDSelect.setDisabled(false); 
}
wOKVEDLookup.getBody().Extension.entityBook.onRowSelect = rowSelected;
";

            w.AddAfterClientInitScript("wOKVEDLookup.MetaData = {};");
            return w;
        }
    }
}
