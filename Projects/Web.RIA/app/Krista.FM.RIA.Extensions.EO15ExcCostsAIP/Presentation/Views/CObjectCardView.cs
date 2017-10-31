using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Properties;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Services;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views
{
    public class CObjectCardView : View
    {
        public const string CommonScope = "EO15AIP.View.Register.Grid";
        public const string Scope = "EO15AIP.View.CObjectCard.Data";

        private readonly int constrObjectId;
        private readonly IEO15ExcCostsAIPExtension extension;
        private readonly D_ExcCosts_CObject constrObject;
        private readonly D_ExcCosts_Status statusEdit;
        private readonly bool canEdit;
        private readonly IConstructionService constrRepository;
        private readonly IList<D_ExcCosts_CharObj> constrMarks;

        public CObjectCardView(
            IEO15ExcCostsAIPExtension extension,
            IConstructionService constrRepository,
            IRepository<D_ExcCosts_Status> statusRepository,
            IRepository<D_ExcCosts_StatusD> statusDRepository,
            ILinqRepository<D_ExcCosts_CharObj> buildDirRepository,
            int id)
        {
            constrObjectId = id;
            this.extension = extension;
            this.constrRepository = constrRepository;
            constrObject = constrRepository.GetOne(id) ?? new D_ExcCosts_CObject();
            if (constrObject.RefStatusD == null)
            {
                constrObject.RefStatusD = statusDRepository.Get(User.IsInRole(AIPRoles.Coordinator)
                                                                    ? (int)AIPStatusD.Review
                                                                    : (int)AIPStatusD.Edit);
            }

            constrMarks = buildDirRepository.FindAll().Where(x => x.RefCObject.ID == id).ToList();
            statusEdit = statusRepository.Get((int)AIPStatus.UnderConstruction);
            var status = constrObject.RefStatusD.ID;
            canEdit = ((User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient)) && (status == (int)AIPStatusD.Edit)) ||
                (User.IsInRole(AIPRoles.Coordinator) && (status == (int)AIPStatusD.Review));
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("EO15AIPRegister", Resources.EO15AIPRegister);
            resourceManager.RegisterClientScriptBlock("EO15AIPCObjectCard", Resources.EO15AIPCObjectCard);

            resourceManager.AddScript(@"
            var idTab = parent.MdiTab.hashCode('/EO15AIPRegister/ShowCObjectCard?objId=' + {0});
            var tab = parent.MdiTab.getComponent(idTab);    
            if (typeof(tab.events.beforeclose) == 'object'){{
                tab.events.beforeclose.clearListeners();
            }}
            tab.addListener('beforeclose', {1});".FormatWith(constrObject.ID, constrObject.ID > 0 ? "beforeCloseCard" : "beforeCloseCardNew"));

            return new List<Component>
            {
                new Viewport
                    {
                        ID = "viewportCObjectCard_{0}".FormatWith(constrObjectId),
                        AutoScroll = true,
                        Items =
                            {
                                new BorderLayout
                                    {
                                        North = { Items = { CreateFields(page) } },
                                        Center = { Items = { CreateDetails(page) } }
                                    }
                            }
                    }
            };
        }

        private IEnumerable<Component> CreateFields(ViewPage page)
        {
            var formPanel = new FormPanel
            {
                ID = "CObjectCardForm",
                Border = true,
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                StyleSpec = "padding-right: 20px",
                Layout = "RowLayout",
                Collapsible = true,
                Height = 560,
                Url = "/EO15AIPCObjectCard/Save?objId={0}".FormatWith(constrObjectId)
            };

            if (canEdit)
            {
                var isCoord = User.IsInRole(AIPRoles.Coordinator);
                var isClient = User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient);
                formPanel.Buttons.Add(new Button
                {
                    ID = "toEdit",
                    ToolTip = @"Отправить объект на редактирование",
                    Icon = Icon.PageEdit,
                    Hidden = !(isCoord && (constrObject.RefStatusD.ID != (int)AIPStatusD.Edit)),
                    Listeners = { Click = { Handler = Scope + @".toEdit({0}, {1});".FormatWith(isCoord ? "true" : "false", isClient ? "true" : "false") } }
                });

                formPanel.Buttons.Add(new Button
                {
                    ID = "toReview",
                    ToolTip = @"Отправить объект на рассмотрение",
                    Hidden = !(isClient && (constrObject.RefStatusD.ID == (int)AIPStatusD.Edit)),
                    Icon = isCoord ? Icon.PageBack : Icon.PageForward,
                    Listeners = { Click = { Handler = Scope + @".toReview({0}, {1});".FormatWith(isCoord ? "true" : "false", isClient ? "true" : "false") } }
                });

                formPanel.Buttons.Add(new Button
                {
                    ID = "toAccept",
                    ToolTip = @"Утвердить объект",
                    Hidden = !(isCoord && (constrObject.RefStatusD.ID == (int)AIPStatusD.Review)),
                    Icon = Icon.Tick,
                    Listeners = { Click = { Handler = Scope + @".toAccept({0}, {1});".FormatWith(isCoord ? "true" : "false", isClient ? "true" : "false") } }
                });

                var buttonSave = new Button
                                     {
                                         ID = "btnOk",
                                         Text = @"Сохранить",
                                         Icon = Icon.Accept,
                                         Listeners =
                                             {
                                                 Click =
                                                     {
                                                         Handler = CommonScope + ".cardSaveAndCloseHandler({0}, {1})".FormatWith(constrObjectId, "false")
                                                     }
                                             }
                                     };
                formPanel.Buttons.Add(buttonSave);
            }

            var fields = CreateProps(page);
            formPanel.Add(fields);
            
            return new List<Component> { formPanel };
        }

        private Component CreateDetails(ViewPage page)
        {
            if (constrObjectId < 1)
            {
                return new DisplayField
                           {
                               Text = @"Детализация будет доступна после сохранения объекта",
                               StyleSpec = "margin-left: 10px; color: red; font-size: 12px; font-weight: bold;"
                           };
            }

            var tabPanel = new TabPanel
            {
                EnableTabScroll = true,
                StyleSpec = "padding-right: 20px",
                MinHeight = 480,
                Header = true
            };

            tabPanel.Add(new DetailExpertiseView(extension, constrRepository, constrObjectId).Build(page));
            tabPanel.Add(new DetailContractView(extension, constrRepository, constrObjectId).Build(page));
            tabPanel.Add(new DetailLimitView(extension, constrRepository, constrObjectId).Build(page));
            tabPanel.Add(new DetailPlanView(extension, constrRepository, constrObjectId).Build(page));
            tabPanel.Add(new DetailDocView(extension, constrRepository, constrObjectId).Build(page));
            tabPanel.Add(new DetailReviewView(extension, constrRepository, constrObjectId).Build(page));
            tabPanel.Add(new DetailAdditObjectInfoView(extension, constrRepository, constrObjectId).Build(page));
            
            return tabPanel;
        }

        private FieldSet CreateProps(ViewPage page)
        {
            var fields = new FieldSet
                             {
                                 ID = "objCardFields",
                                 Layout = "form",
                                 Border = false,
                                 LabelSeparator = String.Empty,
                                 LabelAlign = LabelAlign.Left,
                                 LabelWidth = 470,
                                 Height = 560,
                                 DefaultAnchor = "0"
                             };
            
            fields.Add(new DisplayField
                            {
                                ID = "StatusDName",
                                Disabled = !canEdit,
                                FieldLabel = @"Статус данных",
                                Text = constrObject.RefStatusD.Name
                            });
            fields.Add(new TextField
            {
                ID = "StatusDId",
                Hidden = true,
                Disabled = !canEdit,
                FieldLabel = @"Статус данных",
                Text = constrObject.RefStatusD.ID.ToString()
            });

            // Изменять заказчика нельзя, если тек. пользователь -  заказчик или объект уже сохранен. (Определяется 1 раз).
            var comboClient = FilterControl.GetFilterClient(page);
            comboClient.AllowBlank = false;
            comboClient.Disabled = !canEdit;
            comboClient.FieldLabel = @"Заказчик*";
            fields.Add(comboClient);
            if (constrObject.RefClients != null)
            {
                comboClient.SelectedItem.Value = constrObject.RefClients.ID.ToString();
                comboClient.SelectedItem.Text = constrObject.RefClients.Name;
            }

            if (User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient))
            {
                comboClient.Hidden = true;
                comboClient.AllowBlank = true;
                fields.Add(new DisplayField
                               {
                                   ID = "Client",
                                   Disabled = !canEdit,
                                   FieldLabel = @"Заказчик*",
                                   Text = extension.Client == null ? string.Empty : extension.Client.Name
                               });
            }
            else
            {
                if (constrObject.RefClients != null)
                {
                    comboClient.Hidden = true;
                    fields.Add(new DisplayField
                                   {
                                       ID = "Client",
                                       Disabled = !canEdit,
                                       FieldLabel = @"Заказчик",
                                       Text = constrObject.RefClients.Name
                                   });
                }
            }

            var comboProgram = FilterControl.GetFilterProgram(page);
            comboProgram.AllowBlank = false;
            comboProgram.Disabled = !canEdit;
            comboProgram.FieldLabel = @"Целевая программа*";
            if (constrObject.RefListPrg != null)
            {
                if (constrObject.RefListPrg.ID > 0)
                {
                    comboProgram.SelectedItem.Value = constrObject.RefListPrg.ID.ToString();
                    comboProgram.SelectedItem.Text = constrObject.RefListPrg.Name;
                }
            }

            fields.Add(comboProgram);
            var comboRegion = FilterControl.GetFilterRegion(page);
            comboRegion.FieldLabel = @"Территория*";
            comboRegion.AllowBlank = false;
            comboRegion.Disabled = !canEdit;
            if (constrObject.RefTerritory != null)
            {
                if (constrObject.RefTerritory.ID > 0)
                {
                    comboRegion.SelectedItem.Value = constrObject.RefTerritory.ID.ToString();
                    comboRegion.SelectedItem.Text = constrObject.RefTerritory.Name;
                }
            }

            fields.Add(comboRegion);

            fields.Add(new TextField
            {
                ID = "Name",
                FieldLabel = @"Объект",
                AllowBlank = false,
                Disabled = !canEdit,
                Text = constrObject.Name
            });

            fields.Add(new TextField
                           {
                               ID = "Unit",
                               FieldLabel = @"Единица измерения",
                               Disabled = !canEdit,
                               Text = constrObject.Unit
                           });
            fields.Add(new TextField
                           {
                               ID = "Power",
                               FieldLabel = @"Мощность по проекту",
                               Disabled = !canEdit,
                               Text = constrObject.Power
                           });
            fields.Add(new NumberField
                           {
                               ID = "StartConstr",
                               FieldLabel = @"Начало строительства*",
                               Disabled = !canEdit,
                               AllowBlank = false,
                               MaxValue = 9999,
                               MinValue = 1900,
                               Text = constrObject.StartConstruction == null
                                          ? String.Empty
                                          : ((DateTime)constrObject.StartConstruction).Year.ToString(),
                               Value = constrObject.StartConstruction == null 
                                          ? null
                                          : constrObject.StartConstruction.Value.Year.ToString()
                           });
            fields.Add(new NumberField
                           {
                               ID = "EndConstr",
                               FieldLabel = @"Завершение строительства*",
                               Disabled = !canEdit,
                               AllowBlank = false,
                               MaxValue = 9999,
                               MinValue = 1900,
                               Text = constrObject.EndConstruction == null
                                          ? String.Empty
                                          : ((DateTime)constrObject.EndConstruction).Year.ToString(),
                               Value = constrObject.EndConstruction == null
                                          ? null
                                          : constrObject.EndConstruction.Value.Year.ToString()
                           });
            fields.Add(new TextField
                           {
                               ID = "Permit",
                               FieldLabel = @"Разрешение на строительство",
                               Disabled = !canEdit,
                               Text = constrObject.Permit
                           });
            fields.Add(new TextField 
                           {
                               ID = "Act",
                               FieldLabel = @"Акт выбора земельного участка",
                               Disabled = !canEdit,
                               Text = constrObject.Act 
                           });
            fields.Add(new TextField
                           {
                               ID = "DesignEstimates",
                               Disabled = !canEdit,
                               FieldLabel = @"Утверждение проектно-сметной документации/задания на проектирование",
                               Text = constrObject.DesignEstimates
                           });
            fields.Add(new TextField 
                           {
                               ID = "NormativeDate",
                               Disabled = !canEdit,
                               FieldLabel = @"Нормативный срок строительства",
                               Text = constrObject.NormativeDate
                           });
            fields.Add(new TextField 
                           {
                               ID = "PlanDate",
                               Disabled = !canEdit,
                               FieldLabel = @"Планируемый заказчиком срок ввода объекта в эксплуатацию",
                               Text = constrObject.PlanDate
                           });

            AddObjectMark(fields, AIPMarks.MarkBuildDirection, "BuildDirection", @"Направление строительства");
            AddObjectMark(fields, AIPMarks.MarkObjectType, "ObjectType", @"Тип объекта");
            AddObjectMark(fields, AIPMarks.MarkObjectKind, "ObjectKind", @"Вид объекта");
            AddObjectMark(fields, AIPMarks.MarkReasonsNonProgram, "ReasonsNonProgram", @"Основания для финансирования непрограммного объекта");

            var comboStatus = FilterControl.GetFilterStatus(page);
            comboStatus.AllowBlank = false;
            comboStatus.Disabled = !canEdit;
            comboStatus.FieldLabel = @"Состояние объекта";
            if (constrObject.RefStat != null)
            {
                comboStatus.SelectedItem.Value = constrObject.RefStat.ID.ToString();
                comboStatus.SelectedItem.Text = constrObject.RefStat.Name;
            }
            else
            {
                comboStatus.SelectedItem.Value = statusEdit.ID.ToString();
                comboStatus.SelectedItem.Text = statusEdit.Name;
            }

            fields.Add(comboStatus);

            fields.Add(new DisplayField { Text = @"* - обязательное поле для заполнения" });
            return fields;
        }

        private void AddObjectMark(FieldSet fields, int markCode, string fieldId, string markName)
        {
            var markBuildDir = constrMarks.FirstOrDefault(x => x.RefAIPMark.Code == markCode); 
            fields.Add(new TextField
                           {
                               ID = fieldId,
                               Disabled = !canEdit,
                               FieldLabel = markName,
                               Text = markBuildDir == null ? String.Empty : markBuildDir.Value
                           });
        }
    }
}