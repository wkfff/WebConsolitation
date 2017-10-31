using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.OrgGKH.Params;
using Krista.FM.ServerLibrary;
using Icon = Ext.Net.Icon;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Views
{
    /// <summary>
    /// Интерфейс "План по выручке"
    /// </summary>
    public class PlanProceedsView : View
    {
        /// <summary>
        /// Репозиторий глобальных параметров
        /// </summary>
        private readonly IOrgGkhExtension extension;

        /// <summary>
        /// Функция: как отображать статус
        /// </summary>
        private const string RendererStatusFn =
            @"function (v, p, r, rowIndex, colIndex, ds) { 
                var tpl = '<img title=\'{1}\' src=\'{0}\' width=\'16\' height=\'16\' />';
                return String.format(
                    tpl, 
                    Ext.getCmp(String.format('UrlIconStatus{0}', r.data.StatusId)).getValue(), r.data.StatusName);
            }";

        /// <summary>
        /// Функция: как отображать поле с "Ввод данных"
        /// </summary>
        private const string RendererHasDataFn =
            "function (v, p, r, rowIndex, colIndex, ds) { if (r.data.MayAccept) { var s = \"<img title='Форма полностью заполнена' src='/icons/accept-png/ext.axd' width='16' height='16'>\"; return s; } else { var s = \"<img title='Форма полностью не заполнена' src='/icons/cross-png/ext.axd' width='16' height='16' >\"; return s; } }";

        /// <summary>
        /// Идентификатор грида
        /// </summary>
        private const string GridId = "planGrid";

        public PlanProceedsView(IOrgGkhExtension extension)
        {
            this.extension = extension;
        }

        public override List<Component> Build(ViewPage page)
        {
            if (extension.Region == null && !User.IsInRole(OrgGKHConsts.GroupAuditName))
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен МО.").ToScript());

                return new List<Component>();
            }

            return new List<Component>
                {
                    new Viewport
                    {
                        ID = "viewportPlanProceeds",
                        Items = 
                        { 
                            new BorderLayout
                                      {
                                          Center = { Items = { CreatePlanForm(page) } },
                                          North = { Items = { CreatePlanToolBar() } }
                                      } 
                        }
                    }
                };  
        }

        /// <summary>
        /// Добавляет столбец в грид
        /// </summary>
        /// <param name="gp">Грид, в который дорбавляется столбец</param>
        /// <param name="columnId">Идентификатор столбца (имя в Store)</param>
        /// <param name="header">Заголовок столбца</param>
        private static ColumnBase AddColumn(GridPanel gp, string columnId, string header)
        {
            var column = gp.ColumnModel
                .AddColumn(columnId, header, DataAttributeTypes.dtString);

            column.Sortable = true;
            column.Hideable = false;

            return column;
        }

        /// <summary>
        /// Создает Store для периодов по месяцам
        /// </summary>
        /// <returns>Store для периодов</returns>
        private static Store CreatePeriodsMonthStore()
        {
            var ds = new Store
            {
                ID = "periodsMonthStore",
                AutoLoad = true,
            };

            ds.SetHttpProxy("/Periods/LookupMonthPeriod")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            return ds;
        }

        /// <summary>
        /// Создает Store для периодов по неделям
        /// </summary>
        /// <returns>Store для периодов</returns>
        private static Store CreatePeriodsWeekStore()
        {
            var ds = new Store
            {
                ID = "periodsWeekStore",
                AutoLoad = true,
            };

            ds.SetHttpProxy("/Periods/LookupWeekPeriod")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            return ds;
        }

        /// <summary>
        /// Создание модального окна для формы с новой организацией
        /// </summary>
        /// <param name="gridId">Идентификатор грида, который нужно обновить после добавление организации</param>
        /// <param name="title">Заголовок окна</param>
        /// <returns>Модальное окно</returns>
        private static Window GetBookWindow(string gridId, string title)
        {
            var win = new Window
            {
                ID = "{0}BookWindow".FormatWith(gridId),
                Width = 600,
                Height = 340,
                Title = title,
                Icon = Icon.ApplicationFormEdit,
                Hidden = false,
                Modal = true,
                Constrain = true
            };
            win.AutoLoad.Url = "/";
            win.AutoLoad.Mode = LoadMode.IFrame;
            win.AutoLoad.TriggerEvent = "show";
            win.AutoLoad.ReloadOnEvent = true;
            win.AutoLoad.ShowMask = true;
            win.AutoLoad.MaskMsg = @"Открытие формы с реквизитами организации...";
            win.AutoLoad.Params.Add(new Parameter("id", String.Empty, ParameterMode.Value));

            var buttonSave = new Button
            {
                ID = "btnOk",
                Text = @"Сохранить",
                Icon = Icon.Accept,
                Listeners =
                    {
                        Click =
                            {
                                Handler = @"
            if (btnOk.ownerCt.ownerCt.iframe.dom.contentWindow.OrgForm.isValid()) {{
                    btnOk.ownerCt.ownerCt.iframe.dom.contentWindow.OrgForm.form.submit({{
                                    waitMsg:'Сохранение...', 
                                    success: function(form, action) {{ {0}.hide(); {1}.store.reload(); }}, 
                                    failure: function(form, action) {{ 
                                        var fi = action.response.responseText.indexOf('message:') + 9;
                                        var li = action.response.responseText.lastIndexOf('{2}}}')
                                        var msg = action.response.responseText.substring(li, fi);
                                        Ext.net.Notification.show({{
                                            iconCls    : 'icon-information', 
                                            html       : msg, 
                                            title      : 'Внимание', 
                                            hideDelay  : 2500
                                        }}); 
                                    }}
                                }});
                }}
                else {{
                Ext.net.Notification.show({{
                    iconCls    : 'icon-information', 
                    html       : 'Сохранение невозможно. Необходимо заполнить все обязательные поля.', 
                    title      : 'Внимание', 
                    hideDelay  : 2500
                }});
            }}".FormatWith(win.ID, gridId, '"')
                            }
                    }
            };

            var buttonCancel = new Button
            {
                ID = "btnCancel",
                Text = @"Отмена",
                Icon = Icon.Cancel,
                Listeners =
                    {
                        Click =
                            {
                                Handler = @"{0}.hide()".FormatWith(win.ID)
                            }
                    }
            };

            win.Buttons.Add(buttonCancel);
            win.Buttons.Add(buttonSave);

            return win;
        }

        private static int GetTodayWeek()
        {
            var today = DateTime.Today;
            var days = (today.DayOfWeek > DayOfWeek.Friday) 
                ? (7 - (DayOfWeek.Friday - today.DayOfWeek)) 
                : today.DayOfWeek - DayOfWeek.Friday;
            today = today.AddDays(-days);
            return (today.Year * 10000) + (today.Month * 100) + today.Day;
        }

        /// <summary>
        /// Создает Store для районов
        /// </summary>
        /// <returns>Store для периодов</returns>
        private Store CreateRegionsStore()
        {
            var ds = new Store
            {
                ID = "regionsStore",
                AutoLoad = true,
            };

            ds.SetHttpProxy("/PlanProceeds/LookupRegions")
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            return ds;
        }

        private GridPanel CreatePlanGrid(string storeId, ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = GridId,
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true },
                StoreID = storeId,
                EnableColumnMove = false,
                ColumnLines = true,
                AutoExpandColumn = "NameOrg",
                SelectionModel = { new ExcelLikeSelectionModel() },
                StyleSpec = "margin: 5px 0px 5px 5px; padding-right: 30px;",
                AutoScroll = true
            };

            var columnStatus = AddColumn(gp, "StatusName", string.Empty).SetWidth(30);
            columnStatus.Renderer.Fn = RendererStatusFn;
            columnStatus.Align = Alignment.Center;
            columnStatus.Sortable = true;

            AddColumn(gp, "NameOrg", "Организация");
            AddColumn(gp, "INN", "ИНН");
            AddColumn(gp, "Region", "Муниципальное образование").Width = 200;

            var columnHasData = AddColumn(gp, "MayAccept", "Ввод данных");
            columnHasData.Renderer.Fn = RendererHasDataFn;
            columnHasData.Align = Alignment.Center;
            columnHasData.Sortable = true;

            var command = new GridCommand { Icon = Icon.ApplicationGo, CommandName = "OpenForm" };
            command.ToolTip.Text = "Открыть форму ввода";
            var column = new CommandColumn { Header = string.Empty, Width = 30, ButtonAlign = Alignment.Center };
            column.Commands.Add(command);
            column.Sortable = false;
            gp.ColumnModel.Columns.Add(column);

            gp.AddColumnsWrapStylesToPage(page);

            if (User.IsInRole(OrgGKHConsts.GroupMOName))
            {
                gp.Listeners.RowClick.Handler =
                    @"
                if (this.store.getAt(rowIndex).get('StatusId') == 1) 
                    accept.show();
                else accept.hide();";
            }

            // на скрипт на открытие формы сбора
            var regionIdScriptOrValue = User.IsInRole(OrgGKHConsts.GroupMOName)
                ? extension.Region.ID.ToString() 
                : "cbRegions.value || -1";
            gp.Listeners.Command.Handler = @"
            if (command == 'OpenForm') {{
                if (cbPeriodWeek.disabled)
                    parent.MdiTab.addTab(
                        {{ 
                            title: 'Ежемесячный сбор', 
                            url: '/PlanProceeds/ShowRequest?periodId=' + {0}.store.cbPeriod.value + 
                                    '&orgId=' + record.data.OrgId + '&isMonth=true&regionId=' + {1}, 
                            icon: 'icon-report'
                        }});
                else
                    parent.MdiTab.addTab(
                        {{ 
                            title: 'Еженедельный сбор', 
                            url: '/PlanProceeds/ShowRequest?periodId=' + {0}.store.cbPeriod.value + 
                                '&orgId=' + record.data.OrgId + '&isMonth=false&regionId=' + {1}, 
                            icon: 'icon-report'
                        }});
            }}".FormatWith(GridId, regionIdScriptOrValue);

            gp.AddColumnsHeaderAlignStylesToPage(page);

            return gp;
        }

        private Store CreatePlanStore()
        {
            var ds = new Store
            {
                ID = "planStore",
                AutoSave = true
            };

            ds.AddScript("{0}.store.cbPeriod = cbPeriodMonth; ".FormatWith(GridId));

            var region = User.IsInRole(OrgGKHConsts.GroupMOName)
                ? extension.Region.ID.ToString() 
                : "cbRegions.value || -1";

            ds.Listeners.BeforeLoad.AddAfter(@"
                var period = {1}.store.cbPeriod.value || {0}; 
                Ext.apply({1}.store.baseParams, {{ periodId: period, regionId: {2} }}); "
                .FormatWith(
                    ((DateTime.Today.Year * 10000) + (DateTime.Today.Month * 100)).ToString(), 
                    GridId, 
                    region));

            ds.Listeners.BeforeSave.AddAfter(@"
                var period = {1}.store.cbPeriod.value || {0}; 
                {1}.store.updateProxy.conn.url = '/PlanProceeds/Save?periodId=' + period"
                .FormatWith(((DateTime.Today.Year * 10000) + (DateTime.Today.Year * 100)).ToString(), GridId));

            ds.SetHttpProxy("/PlanProceeds/Read")
                .SetJsonReader()
                .AddField("ID")
                .AddField("NameOrg")
                .AddField("INN")
                .AddField("Region")
                .AddField("OrgId")
                .AddField("StatusId")
                .AddField("StatusName")
                .AddField("StatusId")
                .AddField("HasData")
                .AddField("Login")
                .AddField("MayAccept");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/PlanProceeds/Save",
                Method = HttpMethod.POST
            });

            return ds;
        }

        private IEnumerable<Component> CreatePlanToolBar()
        {
            var bookWindow = GetBookWindow(GridId, @"Добавить новую организацию");
            string addOrgHandler;

            if (User.IsInRole(OrgGKHConsts.GroupMOName))
            {
                addOrgHandler = @"
                {0}
                {1}.autoLoad.url = '/Organization/Book?';
                {1}.show();"
                    .FormatWith(bookWindow.ToScript(), bookWindow.ID);
            }
            else
            {
                addOrgHandler = @"
                var regionId = cbRegions.value;
                if (regionId != null && regionId !=undefined) {{
                    {0}
                    {1}.autoLoad.url = '/Organization/Book?regionId=' + regionId;
                    {1}.show();
                }}".FormatWith(bookWindow.ToScript(), bookWindow.ID);
            }

            var bookWindowEdit = GetBookWindow(GridId, @"Редактирование реквизитов организации");

            var editOrgHandler = @"
                var regionId = cbRegions.value;
                if ({2}.selModel.selection != null) {{
                    {0}
                    {1}.autoLoad.url = '/Organization/Book?orgId=' + {2}.selModel.selection.record.data.OrgId + '&regionId=' + regionId;
                    {1}.show();
                }}"
                .FormatWith(bookWindowEdit.ToScript(), bookWindowEdit.ID, GridId);
            
            var deleteButton = new Button
            {
                ID = "delRecord",
                ToolTip = @"Удалить активную строку",
                Icon = Icon.Delete,
                Hidden = !User.IsInRole(OrgGKHConsts.GroupAuditName),
                Listeners =
                {
                    Click =
                    {
                        Handler = @"
            if ({0}.selModel.selection != null) {{
                Ext.Ajax.request({{
                    url: '/PlanProceeds/CheckCanDeleteOrg?orgId=' + {0}.selModel.selection.record.data.OrgId,
                    success: function (response, options) {{
                        if (response.responseText.indexOf('success:true') > -1)
                            Ext.Msg.show({{
                                title: 'Анализ и планирование',
                                msg: 'Удалить организацию?',
                                buttons: Ext.MessageBox.YESNO,
                                multiline: false,
                                animEl: 'ApplicPanel',
                                icon: Ext.MessageBox.WARNING,
                                fn: function (btn) {{
                                    if (btn == 'yes') {{
                                        var recToRmv = {0}.selModel.selection.record;
                                        Ext.Ajax.request({{
                                            url: '/Organization/DeleteUser?login=' + recToRmv.data.Login,
                                            success: function (response, options) {{ }},
                                            failure: function (response, options) {{ }}
                                        }});
                                        {0}.store.removed.push(recToRmv);
                                        {0}.store.remove(recToRmv);    
                                                
                                        }}
                                }}
                            }});
                        else {{
                            var fi = response.responseText.indexOf('message:') + 9;
                            var li = response.responseText.lastIndexOf('{1}')
                            var msg = response.responseText.substring(li, fi);
                            Ext.net.Notification.show({{
                                iconCls    : 'icon-information', 
                                html       : msg, 
                                title      : 'Анализ и планирование', 
                                hideDelay  : 2500
                            }});
                        }}
                    }},
                    failure: function (response, options) {{ }}
                }});
            }}".FormatWith(GridId, '"')
                    }
                }
            };

            var tb = new Toolbar
                         {
                             ID = "planToolbar",
                             Height = 30,
                             Items =
                                 {
                                     new Button
                                         {
                                             ID = "addOrg",
                                             ToolTip = @"Добавить новую организацию",
                                             Icon = Icon.Add,
                                             Hidden = !(User.IsInRole(OrgGKHConsts.GroupMOName) || 
                                                        User.IsInRole(OrgGKHConsts.GroupAuditName)),
                                             Listeners = { Click = { Handler = addOrgHandler } }
                                         },
                                        new Button
                                         {
                                             ID = "editOrg",
                                             ToolTip = @"Редактировать реквизиты организации",
                                             Icon = Icon.TableEdit,
                                             Hidden = !User.IsInRole(OrgGKHConsts.GroupAuditName),
                                             Listeners = { Click = { Handler = editOrgHandler } }
                                         },

                                     deleteButton,
                                     new Button
                                         {
                                             ID = "refresh",
                                             ToolTip = @"Обновить",
                                             Hidden = !(User.IsInRole(OrgGKHConsts.GroupMOName) || 
                                                        User.IsInRole(OrgGKHConsts.GroupAuditName)),
                                             Icon = Icon.TableRefresh,
                                             Listeners =
                                                 {
                                                     Click =
                                                         {
                                                             Handler = @"{0}.store.load();".FormatWith(GridId)
                                                         }
                                                 }
                                         },
                                     new Button
                                         {
                                             ID = "accept",
                                             ToolTip = @"Утвердить",
                                             Icon = Icon.Accept,
                                             Hidden = !User.IsInRole(OrgGKHConsts.GroupMOName),
                                             Listeners =
                                                 {
                                                     Click =
                                                         {
                                                             Handler = @"
                                                            if ({0}.selModel.selection != null) {{
                                                                if ({0}.selModel.selection.record.data.MayAccept) {{
                                                                    {0}.selModel.selection.record.set('StatusId', 2);
                                                                    {0}.selModel.selection.record.set('StatusName', 'Заблокирован');
                                                                    accept.hide();
                                                                }}
                                                                else {{
                                                                    Ext.net.Notification.show({{
                                                                        iconCls    : 'icon-information', 
                                                                        html       : 'Не может быть утверждена, поскольку не все показатели формы заполнены', 
                                                                        title      : 'Внимание', 
                                                                        hideDelay  : 2500
                                                                    }});
                                                                }}
                                                            }}
                                                            ".FormatWith(GridId)
                                                         }
                                                 }
                                         },
                                     new Button
                                         {
                                             ID = "toEdit",
                                             ToolTip = @"Отправить на редактирование",
                                             Icon = Icon.PageBack,
                                             Hidden = !User.IsInRole(OrgGKHConsts.GroupAuditName),
                                             Listeners = 
                                             { 
                                                 Click = 
                                                 { 
                                                     Handler = @"
                                            if ({0}.selModel.selection != null) {{
                                                {0}.selModel.selection.record.set('StatusId', 1);
                                            }}
                                            ".FormatWith(GridId) 
                                                 } 
                                             }
                                         }
                                 }
                         };
            return new List<Component> { tb };
        }

        private IEnumerable<Component> CreatePlanForm(ViewPage page)
        {
            var panelMain = new FormPanel
            {
                ID = "PlanForm",
                Border = false,
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc"
            };

            var panel = new Panel
            {
                ID = "PlanPanel",
                Border = false,
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                StyleSpec = "margin: 5px 5px 5px 5px;",
                Layout = "RowLayout",
                AutoHeight = true
            };

            panel.Add(new DisplayField
            {
                Text = @"Форма ввода плановых значений по выручке организаций ЖКХ",
                StyleSpec = "font-size: 14px; padding-bottom: 5px; padding-top: 5px; font-weight: bold; text-align: left;"
            });

            var storeMonth = CreatePeriodsMonthStore();
            page.Controls.Add(storeMonth);

            var storeWeek = CreatePeriodsWeekStore();
            page.Controls.Add(storeWeek);

            var storePlan = CreatePlanStore();
            page.Controls.Add(storePlan);

            var storeRegions = CreateRegionsStore();
            page.Controls.Add(storeRegions);

            if (User.IsInRole(OrgGKHConsts.GroupMOName))
            {
                panel.Add(new DisplayField
                              {
                                  Text = @"Территория: {0}".FormatWith(extension.Region.Name),
                                  StyleSpec = "margin: 0px 0px 5px 0px; font-size: 12px;"
                              });
            }

            var periodId = DateTime.Today.Month > 1
                ? (DateTime.Today.Year * 10000) + (DateTime.Today.Month * 100) - 100
                : ((DateTime.Today.Year - 1) * 10000) + 1200;

            var comboPeriodMonth = new ComboBox
                                  {
                                      EmptyText = @"период",
                                      StoreID = storeMonth.ID,
                                      ID = "cbPeriodMonth",
                                      AllowBlank = false,
                                      Editable = false,
                                      TriggerAction = TriggerAction.All,
                                      ValueField = "ID",
                                      Width = 300,
                                      Disabled = false,
                                      DisplayField = "Name",
                                      Value = periodId,
                                      Listeners =
                                          {
                                              Select =
                                                  {
                                                      Handler = @"{0}.store.load();".FormatWith(GridId)
                                                  }
                                          }
                                  };

            var comboPeriodWeek = new ComboBox
                                   {
                                       EmptyText = @"период",
                                       StoreID = storeWeek.ID,
                                       ID = "cbPeriodWeek",
                                       Editable = false,
                                       AllowBlank = false,
                                       TriggerAction = TriggerAction.All,
                                       ValueField = "ID",
                                       Width = 300,
                                       DisplayField = "Name",
                                       Value = GetTodayWeek(),
                                       Disabled = true,
                                       Listeners =
                                           {
                                               Select =
                                                   {
                                                       Handler = @"{0}.store.load();".FormatWith(GridId)
                                                   }
                                           }
                                   };

            var comboPanel = new Panel
            {
                ID = "comboPanel",
                Border = false,
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                Height = 70,
                Width = 350,
                Items =
                    {
                        new RowLayout { Items = { comboPeriodMonth, comboPeriodWeek } }
                    }
            };

            var radioMonth = new Radio
                                 {
                                     ID = "rMonth",
                                     BoxLabel = "Ежемесячный сбор",
                                     Split = true,
                                     Listeners =
                                         {
                                             Check = 
                                                 {
                                                     Handler = @" 
                                                            if (checked) {{
                                                                cbPeriodMonth.setDisabled(false); 
                                                                cbPeriodWeek.setDisabled(true); 
                                                                {0}.store.cbPeriod = cbPeriodMonth; 
                                                                {0}.store.load();
                                                            }}".FormatWith(GridId)
                                                 }
                                         }
                                 };

            var radioWeek = new Radio
                                {
                                    ID = "rWeek",
                                    BoxLabel = "Еженедельный сбор",
                                    Split = true,
                                    Listeners =
                                        {
                                            Check = 
                                                {
                                                    Handler = @"
                                                            if (checked) {{
                                                                cbPeriodMonth.setDisabled(true); 
                                                                cbPeriodWeek.setDisabled(false); 
                                                                {0}.store.cbPeriod = cbPeriodWeek; 
                                                                {0}.store.load();        
                                                            }}".FormatWith(GridId)
                                                }
                                        }
                                };

            var radiogroup = new RadioGroup
                                 {
                                     BorderWidth = 2,
                                     Vertical = true,
                                     ColumnsNumber = 1,
                                     Width = 300,
                                     Items = { radioMonth, radioWeek }
                                 };

            radioMonth.SetValue(true);

            var regionsCombo = new ComboBox
                               {
                                   ID = "cbRegions",
                                   EmptyText = @"Выберите территорию",
                                   StoreID = storeRegions.ID,
                                   Editable = false,
                                   FieldLabel = @"Территория",
                                   AllowBlank = false,
                                   MinWidth = 200,
                                   MaxWidth = 600,
                                   Width = 400,
                                   TriggerAction = TriggerAction.All,
                                   ColumnWidth = 1,
                                   ValueField = "ID",
                                   DisplayField = "Name",
                                   Hidden = User.IsInRole(OrgGKHConsts.GroupMOName),
                                   Listeners =
                                       {
                                           Select =
                                               {
                                                   Handler =
                                                       @"{0}.store.load();".
                                                       FormatWith(GridId)
                                               }
                                       }
                               };

            var periodPanel = new Panel
                                  {
                                      ID = "periodPanel",
                                      Border = false,
                                      CssClass = "x-window-mc",
                                      BodyCssClass = "x-window-mc",
                                      Split = true,
                                      Height = 70,
                                      Items =
                                          {
                                              new BorderLayout
                                                  {
                                                      West = { Items = { radiogroup } },
                                                      Center = { Items = { comboPanel } },
                                                      North =
                                                          {
                                                              MaxWidth = 400,
                                                              Split = true,
                                                              Items = { regionsCombo }
                                                          }
                                                  }
                                          }
                                  };

            panel.Add(periodPanel);

            panelMain.Items.Add(new BorderLayout
            {
                North = { Items = { panel }, Split = true },
                Center = { Items = { CreatePlanGrid(storePlan.ID, page) } }
            });

            return new List<Component>
                       {
                            new Hidden
                                {
                                    ID = "UrlIconStatus1", 
                                    Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit)
                                },
                            new Hidden
                                {
                                    ID = "UrlIconStatus2", 
                                    Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserTick)
                                },
                            panelMain
                       };
        }
    }
}
