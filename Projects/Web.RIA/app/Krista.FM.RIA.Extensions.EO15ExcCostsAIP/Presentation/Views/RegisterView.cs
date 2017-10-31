using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls;
using Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Properties;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Views
{
    public class RegisterView : View
    {
        public const string Scope = "EO15AIP.View.Register.Grid";
        
        private readonly IEO15ExcCostsAIPExtension extension;
        private readonly ILinqRepository<D_ExcCosts_Status> statusRepository;
        private readonly string storeId = "RegisterStore";
        private readonly bool canEdit;
        private readonly string mainPanelId = "registerMainPanel";
        private readonly List<Icon> statusIcons;

        public RegisterView(IEO15ExcCostsAIPExtension extension, ILinqRepository<D_ExcCosts_Status> statusRepository)
        {
            this.extension = extension;
            this.statusRepository = statusRepository;
            this.statusRepository = statusRepository;
            canEdit = User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.Coordinator) || User.IsInRole(AIPRoles.GovClient);

            statusIcons = new List<Icon>
                              {
                                  Icon.UserEdit,
                                  Icon.UserMagnify,
                                  Icon.Accept,
                                  Icon.StarGoldHalfGrey,
                                  Icon.StarGrey,
                                  Icon.StarGold
                              };
        }

        public override List<Component> Build(ViewPage page)
        {
            var isInAvailableRoles = User.IsInRole(AIPRoles.MOClient) || 
                User.IsInRole(AIPRoles.GovClient) ||
                User.IsInRole(AIPRoles.Coordinator) || 
                User.IsInRole(AIPRoles.User);

            if (!isInAvailableRoles)
            {
                return new List<Component>
                           {
                               new DisplayField
                                   {
                                       Text = @"Данный пользователь не входит в группы 'МО', 'Заказчики', 'Координаторы' или 'Пользователи'. Представление недоступно."
                                   }
                           };
            }

            return new List<Component>
            {
                new Viewport
                {
                    ID = "viewportRegister",
                    AutoScroll = true,
                    Items =
                    {
                        new BorderLayout
                        {
                            North =
                            {
                                Items =
                                {
                                    new Panel
                                    {
                                        ID = mainPanelId,
                                        Collapsible = true,
                                        Height = 110,
                                        BodyCssClass = "x-window-mc",
                                        CssClass = "x-window-mc",
                                        StyleSpec = "margin-top: 5px",
                                        Border = true,
                                        Items = { CreateFilterPanel(page) }
                                    }
                                }
                            },

                            // по центру - таблица с объектами строительства
                            Center = { Items = { CreateGrid(page) } }
                        }
                    }
                }
            };
        }

        private Store CreateStore()
        {
            var ds = new Store
            {
                ID = storeId,
                Restful = false,
                AutoSave = true,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные данные. Вы уверены, что хотите обновить?"
            };

            ds.SetHttpProxy("/EO15AIPRegister/Read")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name")
                .AddField("ClientName")
                .AddField("ProgramName")
                .AddField("RegionName")
                .AddField("RegionID")
                .AddField("StateName")
                .AddField("StateId");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/EO15AIPRegister/Save",
                Method = HttpMethod.POST
            });

            if (User.IsInRole(AIPRoles.MOClient) || User.IsInRole(AIPRoles.GovClient))
            {
                ds.BaseParams.Add(new Parameter("clientId", extension.Client == null ? "-1" : extension.Client.ID.ToString(), ParameterMode.Raw));
            }

            ds.BaseParams.Add(new Parameter("filter", Scope + ".getStateFilter()", ParameterMode.Raw));

            return ds;
        }

        private IEnumerable<Component> CreateGrid(ViewPage page)
        {
            var store = CreateStore();
            page.Controls.Add(store);

            var gp = new GridPanel
                         {
                             ID = "RegisterGrid",
                             LoadMask = { ShowMask = true, Msg = "Загрузка" },
                             SaveMask = { ShowMask = true, Msg = "Сохранение" },
                             Icon = Icon.Table,
                             Closable = false,
                             Collapsible = false,
                             Frame = true,
                             StoreID = store.ID,
                             AutoExpandColumn = "Name",
                             ColumnLines = true
                         };

            var command = new GridCommand { Icon = Icon.ApplicationGo, CommandName = "OpenCObject" };
            command.ToolTip.Text = "Открыть карточку объекта";
            var column = new CommandColumn { Header = string.Empty, Width = 30, ButtonAlign = Alignment.Center };
            column.Commands.Add(command);
            gp.ColumnModel.Columns.Add(column);

            var commandFinance = new GridCommand { Icon = Icon.Money, CommandName = "OpenFinance" };
            commandFinance.ToolTip.Text = "Открыть финансирование объекта";
            var columnFinance = new CommandColumn { Header = string.Empty, Width = 30, ButtonAlign = Alignment.Center };
            columnFinance.Commands.Add(commandFinance);
            gp.ColumnModel.Columns.Add(columnFinance);

            var commandInfo = new GridCommand { Icon = Icon.Information, CommandName = "OpenInfo" };
            commandInfo.ToolTip.Text = "Открыть справку о вводе объекта в эксплуатацию";
            var columnInfo = new CommandColumn { Header = string.Empty, Width = 30, ButtonAlign = Alignment.Center };
            columnInfo.Commands.Add(commandInfo);
            gp.ColumnModel.Columns.Add(columnInfo);

            gp.ColumnModel.AddColumn("Name", "Перечень объектов", DataAttributeTypes.dtString).SetWidth(300);
            gp.ColumnModel.AddColumn("ClientName", "Заказчик", DataAttributeTypes.dtString).SetWidth(100);
            gp.ColumnModel.AddColumn("ProgramName", "Целевая программа", DataAttributeTypes.dtString).SetWidth(200);
            gp.ColumnModel.AddColumn("RegionName", "МО", DataAttributeTypes.dtString).SetWidth(100);

            var stateColumn = gp.ColumnModel.AddColumn("StateId", "Статус", DataAttributeTypes.dtString);
            var storeState = FilterControl.CreateFilterStore("statusStore", "/EO15AIPRegister/LookupStatus");
            page.Controls.Add(storeState);
            stateColumn.Editor.Add(FilterControl.CreateEditorCombo(storeState.ID, "cbStatus"));
            stateColumn.SetWidth(150);
            stateColumn.Renderer.Fn = Scope + ".rendererFn";

            // на открытие существующей заявки
            gp.Listeners.Command.Handler = Scope + @".registerOpenTab(command, record);";

            gp.AddRefreshButton();
            if (canEdit)
            {
                gp.AddNewRecordButton().Listeners.Click.Handler = Scope + @".openNewObjectTab();";

                // удаление объекта строительства
                var resourceManager = ResourceManager.GetInstance(page);
                resourceManager.RegisterClientScriptBlock("EO15AIPRegister", Resources.EO15AIPRegister);

                /*gp.AddRemoveRecordButton().Listeners.Click.Handler =
                    Scope + ".deleteRecordInGrid('{0}', '{1}')".FormatWith(mainPanelId, gp.ID); */
            }

            CreateStateFilterButtons(gp);

            gp.AddColumnsHeaderAlignStylesToPage(page);
            gp.AddColumnsWrapStylesToPage(page);

            return new List<Component>
                       {
                           new Hidden { ID = "UrlIconStatus1", Value = ResourceManager.GetInstance().GetIconUrl(statusIcons[0]) },
                           new Hidden { ID = "UrlIconStatus2", Value = ResourceManager.GetInstance().GetIconUrl(statusIcons[1]) },
                           new Hidden { ID = "UrlIconStatus3", Value = ResourceManager.GetInstance().GetIconUrl(statusIcons[2]) },
                           new Hidden { ID = "UrlIconStatus4", Value = ResourceManager.GetInstance().GetIconUrl(statusIcons[3]) },
                           new Hidden { ID = "UrlIconStatus5", Value = ResourceManager.GetInstance().GetIconUrl(statusIcons[4]) },
                           gp
                       };
        }

        private void CreateStateFilterButtons(GridPanel gp)
        {
            var statuses = statusRepository.FindAll().Where(x => x.ID > 0);

            gp.Toolbar().Add(new ToolbarSeparator());
            gp.Toolbar().Add(new DisplayField { Text = @"Фильтры: " });
            foreach (var status in statuses)
            {
                gp.Toolbar().Add(new Button
                {
                    ID = "statusFilter{0}".FormatWith(status.ID),
                    Icon = statusIcons[status.ID - 1],
                    ToolTip = status.Name,
                    EnableToggle = true,
                    Pressed = true,
                    ToggleHandler = Scope + ".toggleFilter"
                });    
            }

            gp.Toolbar().Add(new ToolbarSeparator());
        }

        private FieldSet CreateFilterPanel(ViewPage page)
        {
            var fields = new FieldSet
            {
                ID = "registerFields",
                Collapsible = false,
                Collapsed = false,
                Layout = "form",
                Border = false,
                LabelSeparator = String.Empty,
                LabelAlign = LabelAlign.Left,
                LabelWidth = 150,
                StyleSpec = "margin-top: 5px",
                DefaultAnchor = "0",
                Height = 110
            };

            // для гос. заказчиков и МО не нужен выбор заказчика
            if (!User.IsInRole(AIPRoles.MOClient) && !User.IsInRole(AIPRoles.GovClient))
            {
                var comboClient = FilterControl.GetFilterClient(page);
                FilterControl.AddSelectListener(comboClient, storeId, "clientId");
                fields.Add(comboClient);
            }

            ComboBox comboProgram = FilterControl.GetFilterProgram(page);
            FilterControl.AddSelectListener(comboProgram, storeId, "programId");
            fields.Add(comboProgram);

            ComboBox comboRegion = FilterControl.GetFilterRegion(page);
            FilterControl.AddSelectListener(comboRegion, storeId, "regionId");
            fields.Add(comboRegion);

            return fields;
        }
    }
}
