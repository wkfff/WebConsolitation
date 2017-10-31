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

namespace Krista.FM.RIA.Extensions.EO15TargetPrograms.Presentation.Views
{
    public class ReportProgramExecutingView : View
    {
        internal const string DatasourceID = "dsExecuting";
        internal const string GridpanelID = "gpExecuting";

        private IProgramService programService;

        public ReportProgramExecutingView(IProgramService programService)
        {
            this.programService = programService;
        }

        public int ProgramId
        {
            get { return String.IsNullOrEmpty(Params["programId"]) ? 0 : Convert.ToInt32(Params["programId"]); }
        }

        public override List<Component> Build(ViewPage page)
        {
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
            const string DigitFormat = "0,0.00";

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
                        new Column { ColumnID = "RateName", Header = "Наименование показателя", DataIndex = "RateName", Locked = true,  Width = 150, Editable = false, Wrap = true },
                        new Column { ColumnID = "RateUnits", Header = "Ед.изм.", DataIndex = "RateUnits", Locked = true,  Width = 70, Editable = false, Wrap = true },
                        new Column { ColumnID = "RateTypeName", Header = "Тип показателя", DataIndex = "RateTypeName", Locked = true,  Width = 150, Editable = false, Wrap = true },
                        new NumberColumn
                            {
                                ColumnID = "RateBaseValue", Header = "Базовый показатель", DataIndex = "RateBaseValue", Width = 150, Wrap = true, 
                                Editable = false, Format = DigitFormat, Css = "font-weight: bold;"
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month1", Header = "Январь", DataIndex = "Month1", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month2", Header = "Февраль", DataIndex = "Month2", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month3", Header = "Март", DataIndex = "Month3", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Quarter1", Header = "1 Квартал", DataIndex = "Quarter1", Width = 70, Editable = false, Format = DigitFormat, Css = "font-weight: bold;"
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month4", Header = "Апрель", DataIndex = "Month4", Width = 60, Editable = editable, Format = DigitFormat, 
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month5", Header = "Май", DataIndex = "Month5", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month6", Header = "Июнь", DataIndex = "Month6", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Quarter2", Header = "2 Квартал", DataIndex = "Quarter2", Width = 70, Editable = false, Format = DigitFormat, Css = "font-weight: bold;"
                            },
                        new NumberColumn
                            {
                                ColumnID = "HalfYear1", Header = "1 Полугодие", DataIndex = "HalfYear1", Width = 70, Editable = false, Format = DigitFormat, Css = "font-weight: bold;"
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month7", Header = "Июль", DataIndex = "Month7", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month8", Header = "Август", DataIndex = "Month8", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month9", Header = "Сентябрь", DataIndex = "Month9", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Quarter3", Header = "3 Квартал", DataIndex = "Quarter3", Width = 70, Editable = false, Format = DigitFormat, Css = "font-weight: bold;"
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month10", Header = "Октябрь", DataIndex = "Month10", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month11", Header = "Ноябрь", DataIndex = "Month11", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Month12", Header = "Декабрь", DataIndex = "Month12", Width = 60, Editable = editable, Format = DigitFormat,
                                Editor = { new NumberField { AllowDecimals = true, DecimalPrecision = 2, DecimalSeparator = "." } }
                            },
                        new NumberColumn
                            {
                                ColumnID = "Quarter4", Header = "4 Квартал", DataIndex = "Quarter4", Width = 70, Editable = false, Format = DigitFormat, Css = "font-weight: bold;"
                            },
                        new NumberColumn
                            {
                                ColumnID = "HalfYear2", Header = "2 Полугодие", DataIndex = "HalfYear2", Width = 70, Editable = false, Format = DigitFormat, Css = "font-weight: bold;"
                            },
                        new NumberColumn
                            {
                                ColumnID = "FactYear", Header = "Фактическое значение", DataIndex = "FactYear", Width = 70, Editable = false, Format = DigitFormat, Css = "font-weight: bold;"
                            },
                        new NumberColumn
                            {
                                ColumnID = "PlanYear", Header = "Плановое значение", DataIndex = "PlanYear", Width = 70, Editable = false, Format = DigitFormat, Css = "font-weight: bold;"
                            },
                        new Column
                            {
                                ColumnID = "RateNote", Header = "Примечание", DataIndex = "RateNote", Width = 200, Editable = editable, Wrap = true, Tooltip = "Причины недостижения показателя",
                                Editor = { new TextField() }
                            }
                    },
                },
                SelectionModel = { new ExcelLikeSelectionModel() },
                View = { new GridView() }
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

            store.Proxy.Add(new HttpProxy { Url = "/ReportProgramExecuting/GetReportTable", Method = HttpMethod.GET });
            store.BaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("year", "cmbPeriod.getValue()", ParameterMode.Raw));

            store.UpdateProxy.Add(new HttpWriteProxy { Url = "/ReportProgramExecuting/SaveReportTable", Method = HttpMethod.POST, Timeout = 500000 });
            store.WriteBaseParams.Add(new Parameter("programId", ProgramId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("year", "cmbPeriod.getValue()", ParameterMode.Raw));
            store.WriteBaseParams.Add(new Parameter("storeChangedData", "{0}.store.getChangedData()".FormatWith(GridpanelID), ParameterMode.Raw));

            var reader = new JsonReader
            {
                IDProperty = "ID",
                Root = "data",
                Fields =
                {
                    new RecordField("ID"),
                    new RecordField("RateName"),
                    new RecordField("RateUnits"),
                    new RecordField("RateTypeName"),

                    new RecordField("RateBaseValue"),
                    new RecordField("Month1"),
                    new RecordField("Month2"),
                    new RecordField("Month3"),
                    new RecordField("Month4"),
                    new RecordField("Month5"),
                    new RecordField("Month6"),
                    new RecordField("Month7"),
                    new RecordField("Month8"),
                    new RecordField("Month9"),
                    new RecordField("Month10"),
                    new RecordField("Month11"),
                    new RecordField("Month12"),
                    new RecordField("Quarter1"),
                    new RecordField("Quarter2"),
                    new RecordField("Quarter3"),
                    new RecordField("Quarter4"),
                    new RecordField("HalfYear1"),
                    new RecordField("HalfYear2"),
                    new RecordField("FactYear"),
                    new RecordField("PlanYear"),
                    
                    new RecordField("RateNote")
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
            store.SetHttpProxy("/ReportProgramExecuting/GetYearList");
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
    }
}
