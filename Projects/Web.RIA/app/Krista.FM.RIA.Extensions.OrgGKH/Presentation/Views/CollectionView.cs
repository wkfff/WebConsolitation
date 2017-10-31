using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.OrgGKH.Params;
using Krista.FM.ServerLibrary;
using Icon = Ext.Net.Icon;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Views
{
    public class CollectionView : View
    {
        /// <summary>
        /// Глобальные параметры
        /// </summary>
        protected readonly IOrgGkhExtension Extension;

        /// <summary>
        /// Репозиторий фактов
        /// </summary>
        private readonly ILinqRepository<F_Org_GKH> planRepository;

        /// <summary>
        /// Репозиторий организаций
        /// </summary>
        private readonly ILinqRepository<D_Org_RegistrOrg> orgRepository;

        /// <summary>
        /// Наименование организации
        /// </summary>
        private readonly string orgName;

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        private readonly int orgId;

        /// <summary>
        /// Наименование района
        /// </summary>
        private readonly string regionName;
 
        /// <summary>
        /// Идентификатор формы
        /// </summary>
        private string formID;

        /// <summary>
        /// Форма в состоянии - не редактируема?
        /// </summary>
        private bool stateNotEdited;

        /// <summary>
        /// Initializes a new instance of the CollectionView class (интерфейса "Ежемесячный сбор")
        /// </summary>
        /// <param name="extension">глобальные параметры</param>
        /// <param name="planRepository">Репозиторий планов</param>
        /// <param name="orgRepository">Репозиторий организаций</param>
        /// <param name="orgId">идентификатор организации</param>
        /// <param name="orgName">наименование организации</param>
        /// <param name="periodId">идентификатор периода</param>
        /// <param name="regionName">Наименование района</param>
        public CollectionView(
            IOrgGkhExtension extension,
            ILinqRepository<F_Org_GKH> planRepository, 
            ILinqRepository<D_Org_RegistrOrg> orgRepository,
            int orgId, 
            string orgName, 
            int periodId, 
            string regionName)
        {
            Extension = extension;
            this.orgId = orgId;
            this.orgName = orgName;
            PeriodId = periodId;
            this.regionName = regionName;
            this.planRepository = planRepository;
            this.orgRepository = orgRepository;
        }

        /// <summary>
        /// Initializes a new instance of the CollectionView class (интерфейса "Ежемесячный сбор")
        /// </summary>
        /// <param name="extension">глобальные параметры</param>
        /// <param name="planRepository">Репозиторий фактов</param>
        /// <param name="orgRepository">Репозиторий организаций</param>
        public CollectionView(
            IOrgGkhExtension extension, 
            ILinqRepository<F_Org_GKH> planRepository, 
            ILinqRepository<D_Org_RegistrOrg> orgRepository)
        {
            Extension = extension;

            this.orgRepository = orgRepository;
            var org = orgRepository.FindAll()
                .FirstOrDefault(x => x.Login != null && x.Login.Equals(extension.User.Name));
            
            orgId = (org == null) ? -1 : org.ID;
            orgName = (org == null) ? string.Empty : org.NameOrg;

            regionName = extension.Region.Name;
            this.planRepository = planRepository;
            PeriodId = (DateTime.Today.Year * 10000) + (DateTime.Today.Month * 100) - 100;
        }

        /// <summary>
        /// Наименования полей
        /// </summary>
        protected List<string> ColumnNames { get; set; }

        /// <summary>
        /// Идентификатор грида
        /// </summary>
        protected string GridId { get; set; }

        /// <summary>
        /// Функция: как отображать значения показателей
        /// </summary>
        protected string RendererFn { get; set; }

        /// <summary>
        /// URL для чтения периодов
        /// </summary>
        protected string ReadPeriodUrl { get; set; }

        /// <summary>
        /// скрипт - проверка, можно ли редактировать ячейку таблицы
        /// </summary>
        protected string CheckEdit { get; set; }

        /// <summary>
        /// URL для чтения показателей
        /// </summary>
        protected string ReadPlanUrl { get; set; }

        /// <summary>
        /// URL для чтения показателей
        /// </summary>
        protected string SavePlanUrl { get; set; }

        /// <summary>
        /// Признак, форма сбора ежемесячная или еженедельная
        /// </summary>
        protected bool IsMonth { get; set; }

        protected string FormTitle { get; set; }

        /// <summary>
        /// Идентификатор периода
        /// </summary>
        protected int PeriodId { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            var styles = @".gray-cell{background-color: #DCDCDC !important; border-right-color: #FFFFFF; !important;}";
            ResourceManager.GetInstance(page).RegisterClientStyleBlock("CustomStyle", styles);

            return new List<Component>
                {
                    new Viewport
                    {
                        ID = "viewportMonthlyCollection",
                        CssClass = "x-window-mc",
                        Items = 
                        { 
                            new BorderLayout
                                      {
                                          Center = { Items = { CreateForm(page) } },
                                          North = { Items = { CreateToolBar() } }
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
        /// <param name="editableDouble">Признак, редактируемо ли поле (дробное, 2 знака после запятой)</param>
        protected static ColumnBase AddColumn(GridPanel gp, string columnId, string header, bool editableDouble)
        {
            var column = gp.ColumnModel.AddColumn(columnId, header, DataAttributeTypes.dtString);

            column.Sortable = false;
            column.Hideable = false;
            column.Align = Alignment.Left;
            if (editableDouble)
            {
                column.SetEditableDouble(2);
            }

            return column;
        }

        protected virtual void CreateColumns(GridPanel gp)
        {
        }

        protected void DetermineState()
        {
            var planForOrgFirst = planRepository.FindAll()
                .FirstOrDefault(x => x.RefRegistrOrg.ID == orgId && x.RefYearDayUNV.ID == PeriodId);
            var status = (planForOrgFirst == null) ? OrgGKHConsts.StateEdited : planForOrgFirst.RefStatusD.ID;
            stateNotEdited = status != OrgGKHConsts.StateEdited;
        }

        private IEnumerable<Component> CreateToolBar()
        {
            var acceptButtonHandler = @"
                Ext.Ajax.request({{
                    url: '/PlanProceeds/CheckCanAccept?orgId={1}&periodId=' + cbPeriodMonth.value,
                    success: function (response, options) {{
                        if (response.responseText.indexOf('success:true') > -1) {{
                                Ext.Ajax.request({{
                                    url: '/PlanProceeds/Accept?orgId={1}&periodId=' + cbPeriodMonth.value + '&statusId=2',
                                    success: function (response, options) {{
                                        if (response.responseText.indexOf('success:true') > -1) {{
                                            Ext.net.Notification.show({{
                                                iconCls    : 'icon-information', 
                                                html       : 'Форма отправлена на утверждение', 
                                                title      : 'Внимание', 
                                                hideDelay  : 2500
                                            }});
                                            EditMsg.setValue('Заблокировано'); EditMsg.show();
                                        }}
                                        else {{
                                            var fi = response.responseText.indexOf('message:') + 9;
                                            var li = response.responseText.lastIndexOf('{1}')
                                            var msg = response.responseText.substring(li, fi);
                                            Ext.net.Notification.show({{
                                                iconCls    : 'icon-information', 
                                                html       : 'Не все показатели заполнены', 
                                                title      : 'Внимание', 
                                                hideDelay  : 2500
                                            }});
                                        }}
                                    }},
                                    failure: function (response, options) {{
                                    }}
                                }});
                                save.hide();
                                accept.hide();
                                {0}.store.load();
                                                                            
                        }}
                        else {{
                            var fi = response.responseText.indexOf('message:') + 9;
                            var li = response.responseText.lastIndexOf('{1}')
                            var msg = response.responseText.substring(li, fi);
                            Ext.net.Notification.show({{
                                iconCls    : 'icon-information', 
                                html       : 'Не все показатели заполнены', 
                                title      : 'Внимание', 
                                hideDelay  : 2500
                            }});
                        }}
                    }},
                    failure: function (response, options) {{
                    }}
                }})"
                .FormatWith(GridId, orgId);

            var refreshButton = new Button
            {
                ID = "refresh",
                ToolTip = @"Обновить",
                Icon = Icon.TableRefresh,
                Listeners = { Click = { Handler = @"{0}.store.load();".FormatWith(GridId) } } 
            };

            var saveButton = new Button
            {
                ID = "save",
                ToolTip = @"Сохранить",
                Icon = Icon.TableSave,
                Hidden = (!User.IsInRole(OrgGKHConsts.GroupMOName) && 
                          Extension.UserGroup != OrgGKHExtension.GroupOrg) || stateNotEdited,
                Listeners = 
                {
                    Click =
                    {
                        Handler = @"{0}.store.save(); {0}.store.commitChanges(); {0}.store.load();"
                            .FormatWith(GridId)
                    }
                }
            };

            var acceptButton = new Button
                                 {
                                     ID = "accept",
                                     ToolTip = @"Отправить на утвержение",
                                     Icon = Icon.Accept,
                                     Hidden = (Extension.UserGroup != OrgGKHExtension.GroupOrg) || stateNotEdited,
                                     Listeners = { Click = { Handler = acceptButtonHandler } }
                                 };

            var terr = Extension.UserGroup == OrgGKHExtension.GroupMo || Extension.UserGroup == OrgGKHExtension.GroupAudit
                           ? regionName
                           : null;

            var excelButton = new Button
            {
                ID = "exportReportButton",
                Icon = Icon.PageExcel,
                ToolTip = @"Выгрузка в Excel",
                Listeners =
                {
                    Click =
                    {
                        Handler = @"
                            Ext.net.DirectMethod.request({{
                                url: '/OrgGKHExport/Report',
                                isUpload: true,
                                formProxyArg: 'MonthlyForm',
                                cleanRequest: true,
                                params: {{
                                    orgId: {1}, periodId: cbPeriodMonth.value, sourceId : {3}, name: '{0}', terr: '{4}'
                                }},
                                success:function (response, options) {{
                                                }},
                                                failure: function (response, options) {{
                                                }}
                            }});".FormatWith(orgName, orgId, PeriodId, Extension.DataSource == null ? -1 : Extension.DataSource.ID, terr)
                    }
                }
            };

            var tb = new Toolbar
                         {
                             ID = "planToolbar",
                             Height = 30,
                             Items =
                                 {
                                     refreshButton,
                                     saveButton,
                                     acceptButton,
                                     excelButton
                                 }
                         };

            return new List<Component> { tb };
        }

        /// <summary>
        /// Создает Store для периодов по месяцам
        /// </summary>
        /// <returns>Store для периодов</returns>
        private Store CreatePeriodsMonthStore()
        {
            var ds = new Store
            {
                ID = "periodsMonthStore",
                AutoLoad = true,
            };

            ds.SetHttpProxy(ReadPeriodUrl)
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            ds.SaveAllFields = true;

            return ds;
        }

        private GridPanel CreateMonthlyGrid(string storeId, ViewPage page)
        {
            var gp = new GridPanel
            {
                ID = GridId,
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true },
                StoreID = storeId,
                EnableColumnMove = false,
                ColumnLines = true,
                SelectionModel = { new ExcelLikeSelectionModel() },
                StyleSpec = "margin: 0px 0px 0px 5px; padding-right: 30px;",
                AutoScroll = true
            };

            CreateColumns(gp);

            gp.Listeners.BeforeEdit.AddAfter(CheckEdit);

            gp.AddColumnsWrapStylesToPage(page);
            gp.AddColumnsHeaderAlignStylesToPage(page);

            return gp;
        }

        private Store CreateMonthlyCollectStore()
        {
            var ds = new Store
            {
                ID = "monthlyStore",
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные данные. Вы уверены, что хотите обновить?"
            };

            ds.Listeners.BeforeLoad.AddAfter(@"
                var period = cbPeriodMonth.value || {0}; 
                Ext.apply({1}.store.baseParams, {{ periodId: period, orgId: {2} }}); "
                .FormatWith(
                ((DateTime.Today.Year * 10000) + (DateTime.Today.Month * 100)).ToString(), 
                GridId, 
                orgId));

            // TODO отменить данные, которые реально в БД еще не сохранены как modified
            ds.Listeners.BeforeSave.AddAfter(@"
                var period = cbPeriodMonth.value || {0}; 
                {1}.store.updateProxy.conn.url = '{3}?periodId=' + period + 
                    '&orgId=' + {2} + 
                    '&isMonth={4}&statusId=' + {1}.store.statusId;"
                .FormatWith(
                    ((DateTime.Today.Year * 10000) + 1).ToString(),
                    GridId, 
                    orgId, 
                    SavePlanUrl, 
                    IsMonth ? "true" : "false"));

            ds.SetHttpProxy(ReadPlanUrl)
                .SetJsonReader();

            foreach (var columnName in ColumnNames)
            {
                ds.AddField(columnName);
            }

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = SavePlanUrl,
                Method = HttpMethod.POST
            });

            return ds;
        }

        private IEnumerable<Component> CreateForm(ViewPage page)
        {
            formID = "MonthlyForm";

            var panel = new Panel
            {
                ID = "MonthlyPanel",
                Border = false,
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                StyleSpec = "margin: 0px 5px 0px 5px;",
                Layout = "RowLayout",
                AutoHeight = true
            };

            panel.Add(new DisplayField
            {
                Text = FormTitle,
                StyleSpec = "font-size: 14px; padding-bottom: 5px; padding-top: 5px; font-weight: bold; text-align: left;"
            });

            panel.Add(new DisplayField
            {
                ID = "EditMsg",
                Text = stateNotEdited ? @"Заблокировано" : @"На редактировании",
                StyleSpec = "margin: 0px 0px 5px 0px; font-size: 12px;",
            });

            var storeMonth = CreatePeriodsMonthStore();
            page.Controls.Add(storeMonth);

            var storePlan = CreateMonthlyCollectStore();
            page.Controls.Add(storePlan);

            var titleMoText = Extension.UserGroup == OrgGKHExtension.GroupOrg
                                  ? "Муниципальное образование"
                                  : "Территория";

            panel.Add(new DisplayField
            {
                Text = @"{0}: {1}".FormatWith(titleMoText, regionName),
                StyleSpec = "margin: 0px 0px 5px 0px; font-size: 12px;"
            });

            var org = orgRepository.FindOne(orgId);

            if (Extension.UserGroup != OrgGKHExtension.GroupOrg)
            {
                panel.Add(new DisplayField
                {
                    Text = @"Муниципальное образование: {0}".FormatWith(org.RefRegionAn.Name),
                    StyleSpec = "margin: 0px 0px 5px 0px; font-size: 12px;"
                });
            }

            panel.Add(new DisplayField
            {
                Text = @"Организация: {0}".FormatWith(orgName),
                StyleSpec = "margin: 0px 0px 5px 0px; font-size: 12px;"
            });

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
                Value = PeriodId,
                Listeners =
                {
                    Select =
                    {
                        Handler = @"
                         Ext.Ajax.request({{
                            url: '/Monthly/CheckEditable?orgId=' + {1} + '&periodId='+cbPeriodMonth.value,
                            success: function (response, options) {{
                                if (response.responseText.indexOf('success:true') > -1) {{
                                    save.show();
                                    accept.show();
                                    EditMsg.setValue('На редактировании'); EditMsg.show();
                                }}
                                else {{
                                    save.hide();
                                    accept.hide();
                                    EditMsg.setValue('Заблокировано'); EditMsg.show();
                                }}                          
                            }},
                            failure: function (response, options) {{
                                    save.show();
                                    accept.show();
                                    EditMsg.setValue('На редактировании'); EditMsg.show();
                            }}
                        }});

                        {0}.store.load();
                        ".FormatWith(GridId, orgId)
                    }
                }
            };

            panel.Add(comboPeriodMonth);

            var panelMain = new FormPanel
                                {
                                    ID = formID,
                                    Border = false,
                                    CssClass = "x-window-mc",
                                    BodyCssClass = "x-window-mc",
                                    StyleSpec = "margin: 0px 5px 0px 5px;",
                                    AutoScroll = false,
                                    Items =
                                        {
                                            new BorderLayout
                                                {
                                                    North = { Items = { panel }, Split = true },
                                                    Center = { Items = { CreateMonthlyGrid(storePlan.ID, page) } }
                                                }
                                        }
                                };

            return new List<Component> { panelMain };
        }
    }
}
