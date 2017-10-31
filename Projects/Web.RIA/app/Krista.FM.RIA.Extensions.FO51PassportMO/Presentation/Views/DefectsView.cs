using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO51PassportMO.Presentation.Views
{
    public class DefectsView : View
    {
        private readonly IFO51Extension extension;
        private readonly int periodId;
        private readonly D_Regions_Analysis region;

        /// <summary>
        /// Идентификатор грида
        /// </summary>
        private const string GridId = "defectsGrid";

        /// <summary>
        /// Функция: как отображать поле с показателем
        /// </summary>
        private const string RendererFn =
        @"function (v, p, r) {{
                    p.css = 'gray-cell';
                    var f = Ext.util.Format.numberRenderer('0.000,00/i');
                    return f(v);
                }}";

        public DefectsView(IFO51Extension extension, int periodId, D_Regions_Analysis region)
        {
            this.extension = extension;
            this.periodId = periodId;
            this.region = region; 
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

            var style = @"
            .gray-cell
            {
                background-color: #DCDCDC !important; 
                border-right-color: #FFFFFF; !important;
            }";
            ResourceManager.GetInstance(page).RegisterClientStyleBlock("CustomStyle", style);
            ResourceManager.GetInstance(page).RegisterScript("Hack", "/Content/js/Ext.util.Format.number.Hack.js");

            return new List<Component>
                {
                    new Viewport
                    {
                        ID = "viewportDefects",
                        Items = 
                        { 
                            new BorderLayout
                                      {
                                          Center = { Items = { CreateDefectsForm(page) } },
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
        private static ColumnBase AddColumn(GridPanelBase gp, string columnId, string header)
        {
            var column = gp.ColumnModel
                .AddColumn(columnId, header, DataAttributeTypes.dtString);
            column.Align = Alignment.Right;
            column.Sortable = false;
            column.Hideable = false;
            column.Width = 100;

            return column;
        }
        
        private IEnumerable<Component> CreateDefectsForm(ViewPage page)
        {
           var panel = new Panel
            {
                ID = "DefectsPanel",
                Border = false,
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                Layout = "RowLayout",
                AutoHeight = true
            };

            panel.Add(new DisplayField
            {
                Text = @"Сверка данных от МО с данными месячной отчетности",
                StyleSpec = "font-size: 14px; padding-bottom: 5px; padding-top: 5px; padding-left: 5px; font-weight: bold; text-align: left;"
            });

            var storeDefects = CreateDefectsStore();
            page.Controls.Add(storeDefects);

            panel.Add(new DisplayField
            {
                Text = @"Наименование МО: {0}".FormatWith(region.Name),
                StyleSpec = "margin: 0px 0px 5px 5px; font-size: 12px;"
            });

            var panelMain = new FormPanel
            {
                ID = "DefectsForm",
                Border = false,
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                Items =
                    {
                        new BorderLayout
                            {
                                North =
                                    {
                                        Items = { panel }, 
                                        Split = true
                                    },
                                Center =
                                    {
                                        Items = { CreatePlanGrid(storeDefects.ID, page) }
                                    }
                            }
                    }
            };

            return new List<Component>
                       {
                            panelMain
                       };
        }

        private Store CreateDefectsStore()
        {
            var ds = new Store
            {
                ID = "defectsStore",
                AutoSave = true
            };

            ds.SetHttpProxy("/FO51FormSbor/Defects?periodId={0}&regionId={1}".FormatWith(periodId, region.ID))
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name")
                .AddField("FactPeriodMO")
                .AddField("FactPeriodMonthReport")
                .AddField("FactPeriodDefect")
                .AddField("PlanPeriodMO")
                .AddField("PlanPriodMonthReport")
                .AddField("PlanPeriodDefect");

            ds.Listeners.Load.AddAfter(@"
                var msg = store.reader.jsonData.extraParams;
                if (msg != null && msg != '') 
                    Ext.net.Notification.show({
                                    iconCls    : 'icon-information', 
                                    html       : msg, 
                                    title      : 'Внимание', 
                                    hideDelay  : 10000
                                });
                ");

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
                AutoExpandColumn = "Name",
                AutoExpandMin = 150,
                StyleSpec = "margin: 5px 0px 0px 5px; padding-right: 30px; padding-bottom: 5px;",
                AutoScroll = true
            };

            var groupRow = new HeaderGroupRow();
            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Исполнено за отчетный месяц", ColSpan = 3 });
            groupRow.Columns.Add(new HeaderGroupColumn
                                     {
                                         Header = "Уточненный план на год (по месячному отчету)", 
                                         ColSpan = 3
                                     });

            gp.View.Add(new Ext.Net.GridView { HeaderGroupRows = { groupRow } });

            AddColumn(gp, "Name", "Наименование показателя").Align = Alignment.Left;
            
            AddColumn(gp, "FactPeriodMO", "Данные от МО").RendererFn(RendererFn).SetWrap(false);
            AddColumn(gp, "FactPeriodMonthReport", "Данные по месячной отчетности").RendererFn(RendererFn).SetWrap(false);
            AddColumn(gp, "FactPeriodDefect", "Отклонение").RendererFn(RendererFn).SetWrap(false);

            AddColumn(gp, "PlanPeriodMO", "Данные от МО").RendererFn(RendererFn).SetWrap(false);
            AddColumn(gp, "PlanPriodMonthReport", "Данные по месячной отчетности").RendererFn(RendererFn).SetWrap(false);
            AddColumn(gp, "PlanPeriodDefect", "Отклонение").RendererFn(RendererFn).SetWrap(false);

            gp.AddColumnsWrapStylesToPage(page);
            gp.AddColumnsHeaderAlignStylesToPage(page);

            gp.AddRefreshButton();

            return gp;
        }
    }
}
