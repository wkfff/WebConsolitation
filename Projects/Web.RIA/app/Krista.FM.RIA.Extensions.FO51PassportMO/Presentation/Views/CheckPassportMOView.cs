using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.PeriodField;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Views
{
    public class CheckPassportMOView : View
    {
        private readonly IFO51Extension extension;

        /// <summary>
        /// Скрипт - диалоговое окно с вопросом о изменении состояния заявки
        /// </summary>
        private const string AskChangeState =
            @"
            Ext.Msg.show({{
                title: 'Анализ и планирование',
                msg: '{0}',
                buttons: Ext.MessageBox.YESNO,
                multiline: false,
                animEl: 'ApplicPanel',
                icon: Ext.MessageBox.WARNING,
                fn: function (btn) {{
                    if (btn == 'yes') {{
                        {1}
                    }}
                }}
            }});";

        /// <summary>
        /// Функция: как отображать статус
        /// </summary>
        private const string RendererStatusFn =
            @"function (v, p, r, rowIndex, colIndex, ds) { 
                if (r.data.ID <= 0) {
                    return '';  
                }
                var tpl = '<img title=\'{1}\' src=\'{0}\' width=\'16\' height=\'16\' />';
                return String.format(tpl, Ext.getCmp(String.format('UrlIconStatus{0}', r.data.StatusID)).getValue(), r.data.StatusName);
            }";

        /// <summary>
        /// Идентификатор грида
        /// </summary>
        private const string GridId = "CheckMOsGrid";

        public CheckPassportMOView(IFO51Extension extension)
        {
            this.extension = extension;
        }

        public override List<Component> Build(ViewPage page)
        {
            if (extension.UserGroup == FO51Extension.GroupOther)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен МО или ОГВ.").
                        ToScript());

                return new List<Component>();
            }

            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("Fo51ToolBar", Resources.Resource.Fo51ToolBar);

            return CreateForm(page);
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

            column.Sortable = false;
            column.Hideable = false;

            return column;
        }

        private GridPanel CreateGrid(string storeId, ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = GridId,
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true },
                StoreID = storeId,
                EnableColumnMove = false,
                ColumnLines = true,
                AutoExpandColumn = "MoName",
                StyleSpec = "margin: 5px 0px 5px 5px; padding-right: 30px;",
                AutoScroll = true
            };

            var columnStatus = AddColumn(gp, "StatusName", String.Empty).SetWidth(30);
            columnStatus.Renderer.Fn = RendererStatusFn;
            columnStatus.Align = Alignment.Center;

            var command = new GridCommand { Icon = Icon.ApplicationGo, CommandName = "OpenForm" };
            command.ToolTip.Text = "Открыть форму ввода";
            var column = new CommandColumn { Header = String.Empty, Width = 30, ButtonAlign = Alignment.Center };
            column.Commands.Add(command);
            gp.ColumnModel.Columns.Add(column);

            AddColumn(gp, "MoName", "Наименование МО");

            gp.AddColumnsWrapStylesToPage(page);

            var isOGV = extension.UserGroup == FO51Extension.GroupOGV ? "OGV" : "MO";
            gp.Listeners.RowClick.Handler = @"
            var stateId = this.store.getAt(rowIndex).get('StatusID');
            updateButtons{0}(stateId);
            ".FormatWith(isOGV);

            gp.Listeners.KeyPress.AddAfter(@"
            var k = e.charCode || e.keyCode;
            var rowIndex = this.selModel.selection.cell[0];
            if (k == e.PAGE_UP && rowIndex > 0)
                rowIndex--;
            if (k == e.PAGE_DOWN && rowIndex < this.store.data.items.length - 1)
                rowIndex++;
            {0}
            ".FormatWith(gp.Listeners.RowClick.Handler));

            // на скрипт на открытие формы сбора
            gp.Listeners.Command.Handler = @"
            if (command == 'OpenForm' && record != null) {{
                    /*if (record.data.ID < 1) {{
                        Ext.net.Notification.show({{
                            iconCls    : 'icon-information', 
                            html       : 'Сводный отчет будет доступен после утверждения данных по всем территориям.', 
                            title      : 'Внимание', 
                            hideDelay  : 10000
                        }});
                    }}
                    else*/ {{
                        parent.MdiTab.addTab({{ 
                            title: 'Cбор c ' + record.data.MoName, 
                            url: '/CheckPassportMO/ShowRequest?periodId=' + {0}.store.cbPeriod.getValue() + '&markId={1}&regionId=' + record.data.ID, 
                            icon: 'icon-report'}});
                    }}
            }}".FormatWith(GridId, -1);

            return gp;
        }

        private Store CreateStore()
        {
            var ds = new Store
            {
                ID = "checkMOsStore",
                AutoLoad = true,
                ShowWarningOnFailure = false,
                AutoSave = true
            };

            ds.AddScript("{0}.store.cbPeriod = cbPeriodMonth; ".FormatWith(GridId));

            ds.Listeners.BeforeLoad.AddAfter(@"
                var period = cbPeriodMonth.getValue() || {0}; 
                Ext.apply({1}.store.baseParams, {{ periodId: period }}); "
                .FormatWith(((DateTime.Today.Year * 10000) + (DateTime.Today.Month * 100)).ToString(), GridId));

            ds.Listeners.Load.AddAfter(@"
                var msg = store.reader.jsonData.extraParams;
                if (msg) {
                    InfoMsg.setValue('');
                }
                else {
                    InfoMsg.setValue('Сводные отчеты по МР и ГО будут доступны после утверждения данных по всем территориям.');
                }
                ");

            ds.SetHttpProxy("/CheckPassportMO/Read")
                .SetJsonReader()
                .AddField("ID")
                .AddField("MoName")
                .AddField("StatusID")
                .AddField("StatusName")
                .AddField("HasDefects");

            return ds;
        }

        private List<Component> CreateForm(ViewPage page)
        {
            const string StyleAll = " padding-left: 5px; padding-top: 5px; padding-bottom: 5px; font-size: 12px;";
            
            var storePlan = CreateStore();
            page.Controls.Add(storePlan);

            if (extension.ResponsOIV == null)
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert("Ошибка", "Текущему пользователю не сопоставлен ОГВ.").
                        ToScript());

                return new List<Component>();
            }

            var periodToday = (DateTime.Today.Year * 10000) + ((DateTime.Today.Month - 1) * 100);
            
            var isOGV = extension.UserGroup == FO51Extension.GroupOGV ? "true" : "false";
            var changeState = AskChangeState.FormatWith(
                "Вы действительно хотите отправить форму на доработку?",
                "changeStateHandler({0}, 1, 'Отправка на доработку МО', 'Форма отправлена на доработку', 'На редактировании у МО', {1});".FormatWith(GridId, isOGV));

            var acceptHandler = AskChangeState.FormatWith(
                @"Вы действительно хотите утвердить форму?",
                "changeStateHandler({0}, 3, 'Утверждение', 'Форма утверждена', 'Утверждена', {1});".FormatWith(GridId, isOGV));

            var toOGVHandler = AskChangeState.FormatWith(
                @"Вы действительно отправить форму на рассмотрение ОГВ?",
                "changeStateHandler({0}, 2, 'На рассмотрение ОГВ', 'Форма на рассмотрении у ОГВ', 'На рассмотрении у ОГВ', {1});".FormatWith(GridId, isOGV));

            var dateOn = "{0}.{1}.{2}".FormatWith("01", DateTime.Today.Month, DateTime.Today.Year);

            var selectPeriodHandler = @"
                            var month = (cbPeriodMonth.getValue() / 100) % 100 + 1;
                            var year = (cbPeriodMonth.getValue() - cbPeriodMonth.getValue() % 10000) / 10000;
                            if (month > 12) {{
                                month = 1;
                                year = year + 1;
                            }}    
                            var monthText = '' + month;
                            if (month < 10)
                                monthText = '0' + month;
                            DateOn.setValue('01.' + monthText + '.' + year); 
                            DateOn.show(); 
                            {0}.store.reload();"
                .FormatWith(GridId);

            var config = new PeriodFieldConfig
                             {
                                 AfterSelectHandler = selectPeriodHandler,
                                 DaySelectable = false, 
                                 MonthSelectable = true,
                                 QuarterSelectable = false,
                                 YearSelectable = false,
                                 ShowDay = false,
                                 ShowMonth = true,
                                 ShowQuarter = false,
                                 ShowYear = true,
                                 MinDate = new DateTime(2011, 01, 01),
                                 SortASC = false
                             };
            var today = DateTime.Today;
            var month = today.Month > 1 ? today.Month - 1 : 12;
            var year = today.Month > 1 ? today.Year : today.Year - 1;
            var periodDateDefault = new DateTime(year, month, 1);

            var periodCombo = new PeriodField(config)
                                  {
                                      StyleSpec = "pagging-left: 5px",
                                      ID = "cbPeriodMonth",
                                      AllowBlank = false,
                                      Width = 400,
                                      Disabled = false,
                                      FieldLabel = @"Отчетный период",
                                      LabelWidth = 120,
                                      Value = periodToday,
                                      Text = periodDateDefault.ToString("MMMM yyyy года", CultureInfo.CreateSpecificCulture("ru-RU"))
                                  };

            var reportUrl = extension.GetSvodReportUrl();
            
            var topToolbar = new Toolbar
                {
                    Height = 30,
                    Items =
                        {
                            new Button
                                {
                                    ID = "RefreshBtn",
                                    ToolTip = @"Обновить",
                                    Icon = Icon.TableRefresh,
                                    Listeners = { Click = { Handler = "{0}.reload();".FormatWith(GridId) } }
                                },
                            new Button
                                {
                                    ID = "SendEditBtn",
                                    Text = @"Отправить на доработку МО",
                                    Icon = Icon.PageBack,
                                    Hidden = true,
                                    Listeners = { Click = { Handler = changeState } }
                                },
                            new Button
                                {
                                    ID = "SendDFBtn",
                                    Text = @"Отправить на рассмотрение ОГВ",
                                    Icon = Icon.Tick,
                                    Hidden = true,
                                    Listeners = { Click = { Handler = toOGVHandler } }
                                },
                            new Button
                                {
                                    ID = "AcceptBtn",
                                    Text = @"Утвердить",
                                    Icon = Icon.Accept,
                                    Hidden = true,
                                    Listeners = { Click = { Handler = acceptHandler } }
                                },
                                new HyperLink 
                                {
                                    ID = "LinkToSiteReport",
                                    Text = @"Сводный отчет",
                                    NavigateUrl = reportUrl,
                                    Hidden = reportUrl == null,
                                    Target = "_blank"
                                }
                        }
                };

            var mainPanel = new Panel
            {
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                StyleSpec = StyleAll,
                Border = false,
                Height = 150,
                Items =
                    {
                        topToolbar,
                        new DisplayField
                            {
                                StyleSpec = "padding-top: 10px;  padding-bottom: 10px; font-size: 14px; font-weight: bold;",
                                Text = @"Показатели для формирования антикризисного паспорта МО"
                            },
                        periodCombo,
                        new DisplayField
                            {
                                ID = "DateOn",
                                StyleSpec = StyleAll,
                                LabelWidth = 120, 
                                FieldLabel = @"Данные на",
                                Text = "{0}".FormatWith(dateOn)
                            },
                        new DisplayField
                        {
                            ID = "InfoMsg",
                            StyleSpec = StyleAll + " color: red;",
                            LabelWidth = 120, 
                            FieldLabel = String.Empty,
                            Text = String.Empty
                        },
                        new Hidden { ID = "UrlIconStatus1", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserEdit) },
                        new Hidden { ID = "UrlIconStatus2", Value = ResourceManager.GetInstance().GetIconUrl(Icon.UserTick) },
                        new Hidden { ID = "UrlIconStatus3", Value = ResourceManager.GetInstance().GetIconUrl(Icon.Accept) }
                    }
            };

            return new List<Component>
                       {
                           new Viewport
                               {
                                   ID = "viewportMeasures",
                                   CssClass = "x-window-mc",
                                   StyleSpec = " padding-top: 5px;",
                                   Items =
                                       {
                                           new BorderLayout
                                               {
                                                   Center = { Items = { CreateGrid(storePlan.ID, page) } },
                                                   North = { Items = { mainPanel } },
                                               }
                                       }
                               }
                       };
        }
    }
}
