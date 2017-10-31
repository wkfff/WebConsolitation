using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Models;
using Krista.FM.RIA.Extensions.EO12InvestProjects.Services;

namespace Krista.FM.RIA.Extensions.EO12InvestProjects.Presentation.Views
{
    public class InvestPlanView : View
    {
        private const string DatasourceInvestListID = "dsInvests";
        private const string GridpanelInvestListID = "gpInvests";
        private readonly IInvestPlanService investService;
        private readonly IProjectService projectService;

        public InvestPlanView(IInvestPlanService investService, IProjectService projectService)
        {
            this.investService = investService;
            this.projectService = projectService;
        }

        public int ProjId
        {
            get { return Params["projId"] == "null" ? 0 : Convert.ToInt32(Params["projId"]); }
        }

        public int ProjectInvestType
        {
            get { return Convert.ToInt32(Params["projectInvestType"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
            var yearsColumns = investService.GetYearsColumns(ProjId);
            var projectStatus = projectService.GetProjectStatus(ProjId);
            
            var view = new Viewport
            {
                ID = "viewportMain",
                Items = 
                                {
                                    new BorderLayout
                                    {   
                                        North = { Items = { CreateTopPanel(projectStatus == InvProjStatus.Edit) }, Margins = { Bottom = 5 } },
                                        Center = { Items = { CreateInvestListPanel(yearsColumns, projectStatus == InvProjStatus.Edit) }, Margins = { Bottom = 3 } }
                                    }
                                }
            };

            return new List<Component> { view };
        }

        private Panel CreateTopPanel(bool editable)
        {
            var toolbar = new Toolbar();
            
            toolbar.Add(new Button
            {
                ID = "btnRefresh",
                Icon = Icon.ArrowRefresh,
                ToolTip = "Обновить",
                Listeners = { Click = { Handler = "{0}.reload();".FormatWith(DatasourceInvestListID) } }
            });
            
            toolbar.Add(new Button
            {
                ID = "btnAdd",
                Icon = Icon.Add,
                ToolTip = "Добавить",
                Hidden = !editable,
                Listeners = { Click = { Handler = "{0}.insertRecord();{0}.getSelectionModel().selectFirstRow();{0}.startEditing(0,1);".FormatWith(GridpanelInvestListID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnDelete",
                Icon = Icon.Delete,
                Disabled = true,
                Hidden = !editable,
                ToolTip = "Удалить",
                Listeners = { Click = { Handler = "{0}.deleteSelected();".FormatWith(GridpanelInvestListID) } }
            });

            toolbar.Add(new Button
            {
                ID = "btnSave",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить",
                Hidden = !editable,
                Listeners =
                    {
                        Click =
                            {
                                Handler = "{0}.save();{1}".FormatWith(
                                                                      DatasourceInvestListID, 
                                                                      (ProjectInvestType == (int)InvProjInvestType.Investment ? "parent.dsSumInvestPlan.reload();" : String.Empty))
                            }
                    }
            });

            return new Panel
            {
                ID = "topPanel",
                Height = 27,
                Border = false,
                TopBar = { toolbar }
            };
        }

        private List<Component> CreateInvestListPanel(IList<int> yearsColumns, bool editable)
        {
            int firstYear = yearsColumns[0];
            int lastYear = yearsColumns[yearsColumns.Count - 1];

            GridPanel table = new GridPanel
            {
                ID = GridpanelInvestListID,
                Store = { GetInvestListStore(yearsColumns) },
                Border = false,
                TrackMouseOver = true,
                Icon = Icon.DatabaseTable,
                AutoScroll = true,
                Layout = LayoutType.Fit.ToString(),
                LoadMask = { ShowMask = true, Msg = "Загрузка..." },
                ClearEditorFilter = false, 
                SelectionModel =
                {
                    new RowSelectionModel
                        {
                            ID = "RowSelectionModel1", 
                            SingleSelect = true,
                            Listeners =
                                {
                                    RowSelect = { Handler = "if(Ext.getCmp('btnDelete')!=undefined){btnDelete.enable();}" },
                                    RowDeselect = { Handler = "if (!gpInvests.hasSelection() && Ext.getCmp('btnDelete')!=undefined){btnDelete.disable();}" }
                                }
                        }
                }
            };

            table.ColumnModel.Columns.Add(new Column { ColumnID = "ID", Hidden = true, DataIndex = "ID" });
            table.ColumnModel.Columns.Add(new Column { ColumnID = "IndicatorId", Hidden = true, DataIndex = "IndicatorId" });
            
            table.ColumnModel.Columns.Add(new Column
                                              {
                                                  ColumnID = "Indicator",
                                                  Header = "Показатель",
                                                  DataIndex = "Indicator",
                                                  Width = 180,
                                                  Align = Alignment.Left,
                                                  Editor =
                                                      {
                                                          new ComboBox
                                                              {
                                                                  TriggerAction = TriggerAction.All,
                                                                  Store = { GetIndicatorLookupStore() },
                                                                  AutoShow = true,
                                                                  MaxHeight = 60,
                                                                  Editable = false,
                                                                  AllowBlank = false,
                                                                  ValueField = "Name",
                                                                  DisplayField = "Name",
                                                                  Listeners =
                                                                      {
                                                                          Select =
                                                                              {
                                                                                  Handler = @"
var row = {0}.getSelectionModel().getSelected();
row.set('IndicatorId', record.get('ID'));
row.set('Unit', record.get('Unit'));
".FormatWith(GridpanelInvestListID)
                                                                              }
                                                                      }
                                                              }
                                                      }
                                              });

            ////Запрещаем редактировать колонку у существующих строк! Инициализируем store
            table.Listeners.BeforeEdit.Handler = @"
if (e.field == 'Indicator' && !e.record.phantom){{
   return false;
}}
else if (e.field == 'Indicator' && dsRefIndicator.loaded === undefined){{
   {0}.getColumnModel().getCellEditor(e.column, e.row).field.store.load();
}}
return true;
".FormatWith(GridpanelInvestListID);

            ////Фильтруем допустимые значения
            table.Listeners.AfterEdit.Handler = "if (e.field == 'Indicator'){ filterIndicators(e); }";

            table.ColumnModel.Columns.Add(new Column
                                              {
                                                  ColumnID = "Unit",
                                                  Header = "Ед.изм.",
                                                  DataIndex = "Unit",
                                                  Width = 70,
                                                  Align = Alignment.Left,
                                                  Editable = false
                                              });

            // Динамический набор колонок))
            if (yearsColumns.Count == 1)
            {
                table.ColumnModel.Columns.Add(new NumberColumn
                                                  {
                                                      Header = "Год - {0}".FormatWith(firstYear), 
                                                      DataIndex = "Year{0}".FormatWith(firstYear), 
                                                      Width = 150,
                                                      Format = "0,0.00",
                                                      Align = Alignment.Center,
                                                      Editable = editable,
                                                      Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                                                  });
            }
            else
            {
                table.ColumnModel.Columns.Add(new NumberColumn
                                                  {
                                                      Header = "Год начала реализации - {0}".FormatWith(firstYear), 
                                                      DataIndex = "Year{0}".FormatWith(firstYear), 
                                                      Width = 180, 
                                                      Format = "0,0.00",
                                                      Align = Alignment.Center,
                                                      Editable = editable,
                                                      Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                                                  });
                for (int i = 0 + 1; i < yearsColumns.Count - 1; i++)
                {
                    table.ColumnModel.Columns.Add(new NumberColumn
                                                      {
                                                          Header = "Год - {0}".FormatWith(yearsColumns[i]), 
                                                          DataIndex = "Year{0}".FormatWith(yearsColumns[i]), 
                                                          Width = 150,
                                                          Format = "0,0.00",
                                                          Align = Alignment.Center,
                                                          Editable = editable,
                                                          Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                                                      });
                }

                table.ColumnModel.Columns.Add(new NumberColumn
                                                  {
                                                      Header = "Год окончания реализации - {0}".FormatWith(lastYear), 
                                                      DataIndex = "Year{0}".FormatWith(lastYear), 
                                                      Width = 190,
                                                      Format = "0,0.00",
                                                      Align = Alignment.Center,
                                                      Editable = editable,
                                                      Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                                                  });
            }

            return new List<Component> { table };
        }

        private Store GetInvestListStore(IList<int> yearsColumns)
        {
            var store = new Store
            {
                ID = DatasourceInvestListID,
                WarningOnDirty = true,
                DirtyWarningTitle = "Несохраненные изменения",
                DirtyWarningText = "Есть несохраненные изменения. Перезагрузить данные?",
                ShowWarningOnFailure = false,
                RefreshAfterSaving = RefreshAfterSavingMode.None
            };

            store.Proxy.Add(new HttpProxy { Url = "/InvestPlan/GetInvestTable", Method = HttpMethod.GET });
            store.BaseParams.Add(new Parameter("refProjId", ProjId.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("projInvestType", ProjectInvestType.ToString(), ParameterMode.Value));

            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/InvestPlan/SaveInvestTable", Method = HttpMethod.POST, Timeout = 500000 });
            store.WriteBaseParams.Add(new Parameter("refProjId", ProjId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("storeChangedData", "#{{{0}}}.store.getChangedData()".FormatWith(GridpanelInvestListID), ParameterMode.Raw));

            var reader = new JsonReader
                             {
                                 IDProperty = "ID",
                                 Root = "data",
                                 Fields =
                                     {
                                         new RecordField("ID"),
                                         new RecordField("Indicator"),
                                         new RecordField("IndicatorId"),
                                         new RecordField("Unit"),
                                     }
                             };
            foreach (int year in yearsColumns)
            {
                reader.Fields.Add(new RecordField("Year{0}".FormatWith(year)));
            }

            store.Reader.Add(reader);
            store.Listeners.Save.Handler = "Ext.net.Notification.show({iconCls: 'icon-information', html: 'Изменения сохранены.', title: 'Уведомление', hideDelay: 2500});";
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка инвестиций', e.message || response.statusText);";
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

        private Store GetIndicatorLookupStore()
        {
            var store = new Store { ID = "dsRefIndicator", AutoLoad = false };
            store.SetHttpProxy("/InvestPlan/GetIndicatorList");
            store.Reader.Add(new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                { 
                    new RecordField("ID"),
                    new RecordField("Name"),
                    new RecordField("Unit"),
                }
            });
            store.BaseParams.Add(new Parameter("refTypeI", ProjectInvestType.ToString(), ParameterMode.Value));
            store.Listeners.LoadException.Handler = "Ext.Msg.alert('Ошибка при загрузке списка показателей', response.responseText);";

            store.Listeners.Load.Handler = "this.loaded = true; filterIndicators({{field:'Indicator', grid: {0}, column: 2, row:0}});".FormatWith(GridpanelInvestListID);
            store.Listeners.Load.Delay = 100;
            store.AddScript(@"
function filterIndicators(o){
   var editor = o.grid.getColumnModel().getCellEditor(o.column, o.row);
   editor.field.store.filterBy(function(record){
                                    var value = record.data.ID;
                                    var store = o.grid.store;
                                    ind = store.findExact('IndicatorId', value);
                                    return ind === -1;
                               });
   editor.field.lastQuery = '';
};
");

            return store;
        }
    }
}
