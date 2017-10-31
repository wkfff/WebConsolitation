using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.EO10MissivePRF.Presentation.Views
{
    public class MeasuresView : View
    {
        /// <summary>
        /// Глобальные параметры
        /// </summary>
        private readonly IEO10Extension extension;

        /// <summary>
        /// Репозиторий источников
        /// </summary>
        private readonly IRepository<DataSources> sourceRepository;

        public MeasuresView(IScheme scheme, IEO10Extension extension, IRepository<DataSources> sourceRepository)
        {
            scheme.RootPackage.FindEntityByName("4fbd9e15-38ff-45f9-9428-8cf1e8c1b6dd");
            this.extension = extension;
            this.sourceRepository = sourceRepository;
        }
      
        public override List<Component> Build(ViewPage page)
        {
            const string Styles = @".x-grid3-row-expander{background-image: url('/Content/images/row-expand-sprite.gif');}
.ux-exp-panel{background-color:White;padding-left:30px}
.ux-exp-panel > p {color:graytext;padding-bottom:10px;}
.ux-exp-panel > p > span {font-weight:bold}";
            ResourceManager.GetInstance(page).RegisterClientStyleBlock("RowExpanderStyle", Styles);

            if (extension.UserGroup.Equals(string.Empty))
            {
                ResourceManager.GetInstance(page).RegisterOnReadyScript(
                    ExtNet.Msg.Alert(
                        "Ошибка",
                        "Текущему пользователю не сопоставлен исполнитель мероприятий.")
                    .ToScript());

                return new List<Component>();
            }

            // Store для мероприятий
            var store = CreateMeasuresStore();
            page.Controls.Add(store);

            // Store для годов
            var yearsStore = CreateYearsStore();
            page.Controls.Add(yearsStore);

            var gridMeasures = CreateMeasuresGrid(page, store.ID, yearsStore.ID);

            // Верхняя часть интерфейса с реквизитами
            var titlePanel = new Panel
                                 {
                                     ID = "TitlePanel",

                                     Title = @"Отчитывающаяся организация {0}".FormatWith(extension.Executer.Name),
                                     CssClass = "x-window-mc",
                                     BodyCssClass = "x-window-mc",
                                     Height = 65,
                                     Layout = "RowLayout",
                                     Items =
                                         {
                                             new DisplayField
                                                 {
                                                     ID = "Details", 
                                                     Text = @"Реквизиты: {0}".FormatWith(extension.Executer.Details), 
                                                     StyleSpec = "margin: 0px 0px 5px 10px;"
                                                 },
                                             new DisplayField
                                                 {
                                                     ID = "Curator", 
                                                     Text = @"Ответственный: {0}".FormatWith(extension.Executer.Curator), 
                                                     StyleSpec = "margin: 0px 0px 5px 10px;"
                                                 },
                                         }
                                 };

            return new List<Component>
                       {
                           new Viewport
                               {
                                   ID = "viewportMeasures",
                                   AutoScroll = true,
                                   Items =
                                       {
                                           new BorderLayout
                                               {
                                                   // по центру - таблица с мероприятиями
                                                   Center = 
                                                   { 
                                                       Items =
                                                           {
                                                               gridMeasures
                                                           } 
                                                   },

                                                   // вверху - реквизиты исполнителя
                                                   North = { Items = { titlePanel } }
                                               }
                                       }
                               }
                       };
        }

        /// <summary>
        /// Формирование Store для мероприятий
        /// </summary>
        /// <returns>Store для мероприятий</returns>
        private static Store CreateMeasuresStore()
        {
            var ds = new Store
            {
                ID = "MeasuresStore",
                Restful = false,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                GroupField = "TaskId"
            };

            ds.SetHttpProxy("/Measures/Read")
                .SetJsonReader()
                .AddField("MeasureId")
                .AddField("MeasureName")
                .AddField("TaskId")
                .AddField("TaskName")
                .AddField("SpaceDisch")
                .AddField("RelationshipsId")
                .AddField("RelationshipsReport")
                .AddField("RelationshipsDate")
                .AddField("RelationshipsPeriodId");

            ds.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/Measures/Save",
                Method = HttpMethod.POST
            });

            ds.Listeners.BeforeLoad.AddAfter(@"
                Ext.apply(MeasuresStore.baseParams, { sourceId: (YearBox.value || -1) }); ");

            return ds;
        }

        /// <summary>
        /// Формирование Store для годов, на которые есть классификатор Послание ПРФ. Задачи Мероприятия
        /// </summary>
        /// <returns>Store для годов</returns>
        private static Store CreateYearsStore()
        {
            var ds = new Store
            {
                ID = "YearsStore",
                Restful = false,
                AutoLoad = true,
                ShowWarningOnFailure = false,
                SkipIdForNewRecords = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные данные. Вы уверены, что хотите обновить?"
            };

            ds.SetHttpProxy("/Measures/Years")
                .SetJsonReader()
                .AddField("SourceID")
                .AddField("Year");

            return ds;
        }

        private static RowExpander CreateRowExpander(string gridId)
        {
            var rowExpander = new RowExpander { ExpandOnDblClick = false };
            rowExpander.Template.Html = @"
            <div id='row-{MeasureId}' class='ux-exp-panel'></div>";
            rowExpander.Template.ID = gridId + "_TPL";
            rowExpander.DirectEvents.BeforeExpand.Url = "/Measures/Expand";
            rowExpander.DirectEvents.BeforeExpand.CleanRequest = true;
            rowExpander.DirectEvents.BeforeExpand.IsUpload = false;
            rowExpander.DirectEvents.BeforeExpand.Before = "return !body.rendered;";
            rowExpander.DirectEvents.BeforeExpand.Success = "body.rendered=true;";
            rowExpander.DirectEvents.BeforeExpand.EventMask.ShowMask = true;
            rowExpander.DirectEvents.BeforeExpand.EventMask.Target = MaskTarget.CustomTarget;
            rowExpander.DirectEvents.BeforeExpand.EventMask.CustomTarget = gridId;
            rowExpander.DirectEvents.BeforeExpand.ExtraParams.Add(new Parameter("measureId", "record.data.MeasureId", ParameterMode.Raw));

            return rowExpander;
        }

        /// <summary>
        /// Формирование таблицы с мероприятиями
        /// </summary>
        /// <param name="page">Страница, на которой размещается таблица</param>
        /// <param name="storeId">Идентификатор Store для мероприятий</param>
        /// <param name="yearsStoreId">Идентификатор Store для годов</param>
        /// <returns>Таблицу с мероприятиями</returns>
        private GridPanel CreateMeasuresGrid(ViewPage page, string storeId, string yearsStoreId)
        {
            var gp = new GridPanel
                         {
                             ID = "MeasuresGrid",
                             Icon = Icon.Table,
                             Closable = false,
                             Collapsible = false,
                             Frame = true,
                             StoreID = storeId,
                             AutoExpandColumn = "MeasureName",
                             ColumnLines = true,
                             View =
                                 {
                                     new GroupingView
                                         {
                                             HideGroupedColumn = false,
                                             ForceFit = true,
                                             GroupTextTpl =
                                                 "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'мероприятий' : 'мероприятие']})",
                                             EnableRowBody = true,
                                             EnableNoGroups = true,
                                             EnableGrouping = true,
                                             EmptyGroupText = @"Пусто"
                                         }
                                 }
                         };
            
            gp.AddRefreshButton();

            gp.Toolbar()
                .AddIconButton(
                    "exportReportButton",
                    Icon.PageExcel,
                    "Выгрузка в Excel",
                    string.Empty);

            var sources =
                  sourceRepository.GetAll().Where(s => s.SupplierCode == "ЭО" && s.DataCode == 1).OrderBy(s => s.Year).
                      LastOrDefault();
            var sourceDefaultValue = sources == null ? 0 : sources.ID;
            
            gp.Toolbar().Add(new ComboBox
            {
                ID = "YearBox",
                FieldLabel = @"Источник",
                AllowBlank = false,
                Width = 150,
                LabelWidth = 50,
                TriggerAction = TriggerAction.All,
                StoreID = yearsStoreId,
                ValueField = "SourceID",
                DisplayField = "Year",
                Value = sourceDefaultValue,
                Listeners =
                {
                    Select =
                    {
                        Handler =
                            @"{0}.load();".
                            FormatWith(storeId)
                    }
                }
            });

            gp.ColumnModel.AddColumn("MeasureName", "Мероприятие", DataAttributeTypes.dtString).SetWidth(300).Groupable = false;
            gp.ColumnModel.AddColumn("SpaceDisch", "Срок выполнения", DataAttributeTypes.dtString).SetWidth(150).Groupable = false;
            gp.AddColumn(new Column
            {
                ColumnID = "TaskId",
                Hidden = true,
                DataIndex = "TaskId",
                Header = "Задача",

                GroupRenderer =
                {
                    Handler = @" return '<span style=""visibility: visible;"">' + record.data.TaskName + '</span>';"
                }
            });

            gp.AddScript(@"
            var loadLevel = function(expander, record, body, row){
                if (body.rendered){
                    return;
                }
            
                var recId = record.id,
                    gridId = expander.grid.id,
                    level = 2;
            
                mLevels.BuildLevel(level+1, recId, gridId, {
                    eventMask: {
                        showMask: true,
                        tartget: 'customtarget',
                        customTarget: expander.grid.body
                    },
                
                    success: function(){
                        body.rendered = true;
                    }
                });
            };");
            gp.Plugins.Add(CreateRowExpander(gp.ID));
            gp.AddColumnsWrapStylesToPage(page);
            return gp;
        }
    }
}

