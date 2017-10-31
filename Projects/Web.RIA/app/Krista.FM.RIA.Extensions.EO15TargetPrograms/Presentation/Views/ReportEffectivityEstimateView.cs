using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.ExtNet.Extensions.ExcelLikeSelectionModel;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Helpers;
using Krista.FM.RIA.Extensions.EO15TargetPrograms.Services;
using GridView = Ext.Net.GridView;
using Icon = Ext.Net.Icon;

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Views
{
    public class ReportEffectivityEstimateView : View
    {
        internal const string DatasourceID = "dsEstimate";
        internal const string GridpanelID = "gpEstimate";

        private IProgramService programService;

        public ReportEffectivityEstimateView(IProgramService programService)
        {
            this.programService = programService;
        }

        public int ProgramId
        {
            get { return String.IsNullOrEmpty(Params["programId"]) ? 0 : Convert.ToInt32(Params["programId"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
            if (!ExtNet.IsAjaxRequest)
            {
                var resourceManager = ResourceManager.GetInstance(page);
                resourceManager.RegisterEmbeddedScript<ReportEffectivityEstimateView>("ReportEffectivityEstimateView.js");
            }
            
            var view = new Viewport
            {
                ID = "viewportMain",
                AutoScroll = false,
                Layout = LayoutType.Fit.ToString(),
            };

            // Права на редактирование - только у владельца программы
            var program = programService.GetProgram(ProgramId);
            var ownerPermissions = new PermissionSettings(User, program);
            bool editable = ownerPermissions.CanEditDetail;

            view.Items.Add(CreateReportGrid(editable, page));
        
            return new List<Component> { view };
        }

        private GridPanel CreateReportGrid(bool editable, ViewPage page)
        {
            var criteriaSelector = new ComboBox
                               {
                                   TriggerAction = TriggerAction.All, 
                                   Store = { GetCriteriasLookupStore() },
                                   DataIndex = "Point",
                                   ForceSelection = true,
                                   AutoShow = true,
                                   ListWidth = 400, 
                                   Resizable = true, 
                                   Editable = false, 
                                   AllowBlank = false, 
                                   ValueField = "Point", 
                                   DisplayField = "Name", 
                                   Listeners = { Select = { Fn = "OnSelectCriterias" } },
                               };

            var gp = new GridPanel
            {
                ID = GridpanelID,
                Icon = Icon.DatabaseTable,
                Store = { GetReportStore() },
                TopBar = { GetToolbar(editable) },
                AutoScroll = true,
                Border = false,
                ColumnLines = true,
                TrackMouseOver = true,
                LoadMask = { ShowMask = true },
                ColumnModel =
                {
                    Columns = 
                    {
                        new Column { Hidden = true, DataIndex = "ID" },
                        new Column 
                        {
                            ColumnID = "CritName",
                            Header = "Наименование критерия/подкритерия", 
                            DataIndex = "CritName", 
                            Locked = true,  
                            Width = 300, 
                            Editable = false, 
                            Wrap = true,
                            Renderer = 
                            {
                                Handler = @"
var indent = record.get('Level')*20;
return '<div style=""display:inline-block; padding-left:'+indent+'px"">' + value + '</span>'
"
                            }
                        },
                        new Column { ColumnID = "Weight", Header = "Вес", DataIndex = "Weight", Locked = true,  Width = 50, Editable = false, Wrap = true },
                        new Column
                            {
                                ColumnID = "Point", Header = "Балл", DataIndex = "Point", Width = 50, 
                                Editable = editable,
                                Editor = { criteriaSelector },
                                Wrap = true
                            },
                        new Column
                            {
                                ColumnID = "Estimate", Header = "Оценка по критерию/подкритерию", DataIndex = "Estimate", Width = 150, Editable = false,
                            },
                        new Column
                            {
                                ColumnID = "Comment", Header = "Комментарий", DataIndex = "Comment", Width = 250, Editable = editable,
                                Editor = { new TextField() }
                            }
                    },
                },
                SelectionModel = { new ExcelLikeSelectionModel() },
                View = { new GridView() },
                Listeners =
                    {
                        BeforeEdit =
                            {
                                Handler = @"
if (e.record.get('Level') != 1) { 
  return false; 
}
e.grid.getColumnModel().getCellEditor(e.column, e.row).field.store.load(); 
return true;
"
                            }
                    }
            };

            ////gp.AddColumnsWrapStylesToPage(page);
            var style = new StringBuilder();
            foreach (ColumnBase column in gp.ColumnModel.Columns)
            {
                if (column.Wrap)
                {
                    style.Append(".x-grid3-col-")
                        .Append(column.ColumnID)
                        .AppendLine("{white-space: normal;}");
                }
            }

            ResourceManager.GetInstance(page).RegisterClientStyleBlock("RefColumnStyles", style.ToString());

            return gp;
        }

        private Toolbar GetToolbar(bool editable)
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(DatasourceID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить",
                Hidden = !editable,
                Listeners = { Click = { Handler = "{0}.save();".FormatWith(DatasourceID) } }
            });

            toolbar.Add(new ToolbarSpacer(25));

            toolbar.Add(new ComboBox
            {
                ID = "cmbPeriod",
                FieldLabel = "Период",
                LabelWidth = 50,
                Store = { GetYearLookupStore() },
                AutoShow = true,
                Editable = false,
                AllowBlank = false,
                TriggerAction = TriggerAction.All,
                Mode = DataLoadMode.Local,
                ValueField = "ID",
                DisplayField = "Value",
                Width = 250,
                Listeners = { Select = { Handler = "{0}.reload();".FormatWith(DatasourceID) } }
            });

            toolbar.Add(new ComboBox
            {
                ID = "cmbStage",
                FieldLabel = "Стадия",
                LabelWidth = 50,
                Editable = false,
                AllowBlank = false,
                TriggerAction = TriggerAction.All,
                Mode = DataLoadMode.Local,
                Items =
                    {
                        new ListItem("Разработка концептуальных предложений", ((int)ProgramStage.Concept).ToString()),
                        new ListItem("Разработка проекта", ((int)ProgramStage.Design).ToString()),
                        new ListItem("Реализация", ((int)ProgramStage.Realization).ToString())
                    },
                Value = (int)ProgramStage.Concept,
                Width = 250,
                Listeners = { Select = { Handler = "{0}.reload();".FormatWith(DatasourceID) } }
            });

            return toolbar;
        }

        private Store GetReportStore()
        {
            var store = new Store
            {
                ID = DatasourceID,
                AutoLoad = false,
                WarningOnDirty = true,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                ShowWarningOnFailure = false,
                RefreshAfterSaving = RefreshAfterSavingMode.Always
            };

            store.Proxy.Add(new HttpProxy { Url = "/ReportEffectivityEstimate/GetReportTable", Method = HttpMethod.GET });
            store.BaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("year", "cmbPeriod.getValue()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("stage", "cmbStage.getValue()", ParameterMode.Raw));

            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/ReportEffectivityEstimate/SaveReportTable", Method = HttpMethod.POST, Timeout = 500000 });
            store.WriteBaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("year", "cmbPeriod.getValue()", ParameterMode.Raw));
            store.WriteBaseParams.Add(new Parameter("stage", "cmbStage.getValue()", ParameterMode.Raw));

            var reader = new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                {
                    new RecordField("ID"),
                    new RecordField("Level"),
                    new RecordField("CritName"),
                    new RecordField("Weight"),
                    new RecordField("Point"),
                    new RecordField("Estimate"),
                    new RecordField("Comment"),
                    new RecordField("SelectedId")
                }
            };

            store.Reader.Add(reader);
            store.Listeners.Save.Handler = "Ext.net.Notification.show({iconCls: 'icon-information', html: 'Изменения сохранены.', title: 'Уведомление', hideDelay: 2500});";
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке отчета', response.responseText);";
            store.Listeners.SaveException.Handler = @"
if (response.extraParams != undefined && response.extraParams.responseText != undefined) {
    Ext.Msg.alert('Ошибка', response.extraParams.responseText);
} else {
    var responseParams = Ext.decode(response.responseText);
    if (responseParams != undefined && responseParams.extraParams != undefined && responseParams.extraParams.responseText != undefined) {
        Ext.Msg.alert('Ошибка', responseParams.extraParams.responseText);
    } else {
        Ext.Msg.alert('Ошибка', 'Server failed');
    }
};
";
            return store;
        }

        private Store GetYearLookupStore()
        {
            var store = new Store { ID = "dsRefYear", AutoLoad = true };
            store.SetHttpProxy("/ReportEffectivityEstimate/GetYearList");
            store.BaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));
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
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка лет', response.responseText);";
            store.Listeners.DataChanged.Handler = "cmbPeriod.setValue(dsRefYear.data.items[0].data.ID);{0}.load();".FormatWith(DatasourceID);
            return store;
        }

        private Store GetCriteriasLookupStore()
        {
            var store = new Store { ID = "dsCriterias", AutoLoad = false };
            store.SetHttpProxy("/ReportEffectivityEstimate/GetCriteriasListForLookup");
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name"),
                    new RecordField("Point")
                }
            });
            store.BaseParams.Add(new Parameter("criteriaId", "{0}.getSelectionModel().getSelections()[0].get('ID')".FormatWith(GridpanelID), ParameterMode.Raw));
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка критериев', response.responseText);";

            ////store.Listeners.Load.Handler = "this.loaded = true;";
            ////store.Listeners.Load.Delay = 100;
            return store;
        }
    }
}
