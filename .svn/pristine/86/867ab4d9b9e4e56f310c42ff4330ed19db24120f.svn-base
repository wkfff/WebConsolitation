using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Views
{
    public class EstimateInputDataView : View
    {
        private const string StyleAll = "margin: 0px 0px 5px 10px; font-size: 12px;";

        /// <summary>
        /// Идентификатор категории
        /// </summary>
        private readonly int categoryId;

        private readonly int periodId;

        public EstimateInputDataView(int categoryId, int periodId)
        {
            this.categoryId = categoryId;
            this.periodId = periodId;
        }

        /// <summary>
        /// Признак, можно ли редактировать
        /// </summary>
        public bool Editable { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateRequestsStore());

            var panelHeader = new Panel
            {
                StyleSpec = StyleAll,
                CssClass = "x-window-mc",
                BodyCssClass = "x-window-mc",
                Border = false,
                Height = 85
            };

            panelHeader.Add(new DisplayField
            {
                ID = "requestCntAll",
                StyleSpec = "margin: 10px 0px 5px 10px;",
                LabelAlign = LabelAlign.Left,
                FieldLabel = @"Общее количество заявок от налогоплательщиков в данной категории",
                LabelWidth = 600,
                Value = string.Empty,
                ReadOnly = true,
                Enabled = false
            });

            panelHeader.Add(new DisplayField
            {
                ID = "requestCntEstimate",
                StyleSpec = "margin: 10px 0px 5px 10px;",
                LabelAlign = LabelAlign.Left,
                LabelWidth = 600,
                FieldLabel = @"Количество заявок от налогоплательщиков в данной категории, отправленных на оценку",
                Value = string.Empty,
                ReadOnly = true,
                Enabled = false
            });

            panelHeader.Add(new DisplayField
            {
                ID = "requestMsg",
                StyleSpec = "margin: 10px 0px 5px 0px; font-size: 12px; color: red;",
                LabelAlign = LabelAlign.Left,
                LabelWidth = 600,
                FieldLabel = string.Empty,
                Value = string.Empty,
                ReadOnly = true,
                Enabled = false
            });

            var tabDetails = new Panel
            {
                ID = "EstimateInputTab",
                Title = @"Исходные данные (Заявки от налогплательщиков)",
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Closable = false,
                Items =
                        {
                            new BorderLayout
                                {
                                    North = { Items = { panelHeader } },
                                    Center = { Items = { CreateRequestsTable(page) } }
                                }
                        }
            };

            return new List<Component> { tabDetails };
        }

        private GridPanel CreateRequestsTable(ViewPage page)
        {
            var gp = new GridPanel
                         {
                             ID = "RequstsEstimateGrid{0}".FormatWith(categoryId),
                             StoreID = "RequstsEstimateStore{0}".FormatWith(categoryId),
                             AutoScroll = true,
                             AutoExpandColumn = "RefOrgName"
                         };

            var command = new GridCommand { Icon = Icon.ApplicationGo, CommandName = "OpenRequest" };
            command.ToolTip.Text = "Открыть заявку";
            var columnOpen = new CommandColumn { Header = string.Empty, Width = 30, ButtonAlign = Alignment.Center };
            columnOpen.Commands.Add(command);
            gp.ColumnModel.Columns.Add(columnOpen);

            gp.ColumnModel.AddColumn("ID", "Номер заявки", DataAttributeTypes.dtString).SetWidth(60);
            gp.ColumnModel.AddColumn("RefOrgName", "Наименование налогоплательщика", DataAttributeTypes.dtString);
            gp.ColumnModel.AddColumn("RequestDate", "Дата создания заявки", DataAttributeTypes.dtString);
            gp.ColumnModel.AddColumn("CopiesCnt", "Число копий", DataAttributeTypes.dtString);
            var column  = gp.ColumnModel.AddColumn("Included", "Включена", DataAttributeTypes.dtBoolean);
            if (Editable)
            {
                column.SetEditableBoolean();
                gp.AddRefreshButton();

                var handlerSelectAll = @"
                    for (var iRequest = 0; iRequest < {0}.data.items.length; iRequest++) {{
                        {0}.data.items[iRequest].set('Included', true);
                    }};
                    requestMsg.setValue('Для пересчета итоговых данных в соответствии с внесенными изменениями необходимо сохранить заявку');
                    ".FormatWith(gp.StoreID, gp.ID);
                gp.Toolbar()
                .AddIconButton(
                    "{0}SelectAllBtn".FormatWith(gp.ID),
                    Icon.TableAdd,
                    "Включить все заявки",
                    handlerSelectAll);

                var handlerDeselectAll = @"
                    for (var iRequest = 0; iRequest < {0}.data.items.length; iRequest++) {{
                        {0}.data.items[iRequest].set('Included', false);
                    }};
                    requestMsg.setValue('Для пересчета итоговых данных в соответствии с внесенными изменениями необходимо сохранить заявку');
                    ".FormatWith(gp.StoreID, gp.ID);
                gp.Toolbar()
                .AddIconButton(
                    "{0}DeselectAllBtn".FormatWith(gp.ID),
                    Icon.TableDelete,
                    "Исключить все заявки",
                    handlerDeselectAll);
            }

            var excelHandler =
                @" var s = RequstsEstimateGrid{0}.getSelectionModel().selection;
                        if (s != null && s != undefined) {{
                            var orgName = s.record.get('RefOrgName');
                            Ext.net.DirectMethod.request({{
                                url: '/FO41Export/ReportTaxpayer',
                                isUpload: true,
                                formProxyArg: 'RequestsEstimateFrom',
                                cleanRequest: true,
                                params: {{
                                    id: s.record.get('ID'), name: orgName
                                }},
                                success:function (response, options) {{}},
                                failure: function (response, options) {{}}
                            }});
                        }};".FormatWith(categoryId);

            gp.Toolbar().AddIconButton("exportReportButton", Icon.PageExcel, "Выгрузка в Excel", excelHandler);

            gp.Listeners.AfterEdit.Handler = @"
            requestCntEstimate.setValue(requestsEstimate()); 
            requestMsg.setValue('Для пересчета итоговых данных в соответствии с внесенными изменениями необходимо сохранить заявку');";

            gp.Listeners.Command.Handler = @"
            if (command == 'OpenRequest') {
                parent.MdiTab.addTab({ id: 'req_'  + record.id, title: 'Просмотр заявки', url: '/FO41Requests/ShowRequest?applicationId=' + record.id, icon: 'icon-report'});
            }";

            // ToolTip для строки (заявки)
            gp.ToolTips.Add(new ToolTip
            {
                TargetControl = gp,
                Delegate = ".x-grid3-row",
                TrackMouse = true,
                Listeners =
                {
                    Show =
                    {
                        Handler = @"var rowIndex = #{{{0}}}.view.findRowIndex(this.triggerElement);
                                    if (#{{{0}}}.store.getAt(rowIndex).get('Included') == true)
                                        this.body.dom.innerHTML = 'Информация по данной заявке будет включена в итоговую заявку по соответствующей категории';
                                    else
                                        this.body.dom.innerHTML = 'Заявка не включена';"
                            .FormatWith(gp.ID)
                    }
                }
            });

            gp.AddColumnsWrapStylesToPage(page);
            return gp;
        }

        private Store CreateRequestsStore()
        {
            var ds = new Store 
            { 
                ID = "RequstsEstimateStore{0}".FormatWith(categoryId), 
                AutoLoad = true,
                DirtyWarningTitle = @"Несохраненные изменения",
                DirtyWarningText = @"Есть несохраненные изменения. Перезагрузить данные?"
            };

            ds.BaseParams.Add(new Parameter("categoryId", categoryId.ToString(), ParameterMode.Raw));
            ds.BaseParams.Add(new Parameter("periodId", periodId.ToString(), ParameterMode.Raw));
            
            ds.SetHttpProxy("/FO41Requests/ReadByCategory")
                .SetJsonReader()
                .AddField("ID")
                .AddField("RefOrgName")
                .AddField("RequestDate")
                .AddField("CopiesCnt")
                .AddField("Included");

            ds.Listeners.Load.AddAfter(
                @"
                    requestCntAll.setValue(RequstsEstimateStore{0}.data.length); 
                    requestCntEstimate.setValue(requestsEstimate());".FormatWith(categoryId));

            ds.AddScript(@"
                var saveIncluded = function(appId) {{
                    var toInclude = [];
                    Ext.each(RequstsEstimateStore{0}.getModifiedRecords(), function (record) {{
                            toInclude.push(record.data);
                    }});
                    Ext.Ajax.request({{
                        url: '/FO41Requests/UpdateIncluded?appFromOGVId='+appId,
                        params: {{ data: '{{{1}Included{1}:' + Ext.encode(toInclude) + '}}'}},
                        success: function (response, options) {{
                            RequstsEstimateStore{0}.commitChanges();;
                        }},
                        failure: function (response, options) {{
                            Ext.MessageBox.alert('Message', response.statusText);
                        }}
                    }});
                }}
                var requestsEstimate = function() {{
                    var cnt = 0;
                    Ext.each(RequstsEstimateStore{0}.data.items, function (record) {{
                        // если заявка влючена на оценку
                        if (record.data.Included == true) {{
                            // увеличиваем счетчик
                            cnt++;
                        }}
                    }});
                    var cntAll = RequstsEstimateStore{0}.data.items.length;
                    if (cntAll == 0) {{
                        return '' + cnt + ' (0%)';
                    }}
                    return '' + cnt + ' (' + cnt/cntAll*100 + '%)';
                }}
            ".FormatWith(categoryId, '"'));
            return ds;
        }
    }
}
