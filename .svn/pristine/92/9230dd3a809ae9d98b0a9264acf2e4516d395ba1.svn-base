using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class Form2pValuesGridControl : Control
    {
        private const string StoreId = "dsForm2pValues";
        private const string GridId = "gpForm2pValues";

        private readonly int year;
        private readonly int varId;
        
        private string key;
        private UserFormsControls ufc;
        private IForecastExtension extension;

        public Form2pValuesGridControl(string key, IForecastExtension extension)
        {
            this.extension = extension;
            this.key = key;
            ufc = this.extension.Forms[key];

            var obj = ufc.GetObject("year");
            if (obj != null)
            {
                year = Convert.ToInt32(obj);
            }

            obj = ufc.GetObject("varId");
            if (obj != null)
            {
                varId = Convert.ToInt32(obj);
            }
        }

        public Store Store { get; private set; }
        
        public override List<Component> Build(ViewPage page)
        {
            Store = CreateStore();
            page.Controls.Add(Store);

            page.Controls.Add(CreatePlanVarStore());
            page.Controls.Add(CreateScenVarStore());
            
            GridPanel gp = new GridPanel
            {
                ID = GridId,
                StoreID = StoreId,
                ////MonitorResize = true,
                AutoHeight = true,
                AutoWidth = true,
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                AutoExpandColumn = "ParamName"
            };
            
            string editFn =
@"
    /*
    Properties of 'e' include:
        e.grid - This grid
        e.record - The record being edited
        e.field - The field name being edited
        e.value - The value being set
        e.originalValue - The original value for the field, before the edit.
        e.row - The grid row index
        e.column - The grid column index
    */    
    if (dsForm2pValues.getChangedData().length >0)
    {{
        btnSaveChanged.setDisabled(false);
    }}
    else
    {{
        btnSaveChanged.setDisabled(true);
    }}

    Ext.net.DirectEvent.confirmRequest(
    {{
        cleanRequest: true,
        isUpload: false,
        url: '/Form2pValues/CalcInGridParam?baseYear={0}&varId={1}&sig='+e.record.data.Signat+'&column='+this.getColumnModel().getDataIndex(e.column)+'&changedData='+{2}.getChangedData(),
        control: this
        ////userSuccess: function(response, result, el, type, action, extraParams){{  }}
    }});
".FormatWith(year, varId.ToString(), StoreId);

            gp.Listeners.AfterEdit.AddAfter(editFn);
            /*gp.DirectEvents.AfterEdit.Url = "/Form2pValues/CalcInGridParam";
            gp.DirectEvents.AfterEdit.ExtraParams.Add(new Parameter("year", year.ToString(), ParameterMode.Value));
            gp.DirectEvents.AfterEdit.ExtraParams.Add(new Parameter("varid", varId.ToString(), ParameterMode.Value));*/
            
            GroupingView groupingView = new GroupingView
            {
                ID = "GroupingView1",
                HideGroupedColumn = true,
                EnableNoGroups = true,
                GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Items' : 'Item']})",
                StartCollapsed = true
            };

            gp.View.Add(groupingView);

            ////gp.ColumnModel.AddColumn("ID", "ID", "id", DataAttributeTypes.dtInteger, Mandatory.NotNull);
            ColumnBase cb;

            /*cb = gp.ColumnModel.AddColumn("ParamName", "ParamName", "Параметр", DataAttributeTypes.dtString, Mandatory.NotNull);
            cb.SetWidth(300);*/

            Column column = new Column
            {
                ColumnID = "ParamName",
                DataIndex = "ParamName",
                Header = "Параметр",
                Width = 300,
                Wrap = true
            };
            
            column.Commands.Add(new ImageCommand
            {
                Icon = Icon.TableRowInsert,
                CommandName = "insert"
            });

            gp.Listeners.Command.AddAfter("if (command == 'insert') { gpForm2pValues.getSelectionModel().selectById(record.id); dsPlanVar.reload(); clearInsertForm(); insertWindow.show(); }");

            gp.Listeners.CellClick.AddAfter("if (chart1.isVisible()) { stChartData.reload(); }");
            
            /*GridCommand gridCommand = new GridCommand
            {
                Icon = Icon.DateAdd
            };

            Menu insertMenu = new Menu
            {
                EnableScrolling = false
            };

            MenuItem menuCommand = new MenuItem
                                          {
                                              Text = "Вставить из прогноза",
                                              Icon = Icon.TableRowInsert,
                                              CommandName = "insertPlan"
                                          };

            insertMenu.Items.Add(menuCommand);

            column.Commands.Add(gridCommand);*/
            
            gp.ColumnModel.Columns.Add(column);
            
            gp.ColumnModel.AddColumn("Units", "Units", "Ед.измер.", DataAttributeTypes.dtString, Mandatory.NotNull);
            ////gp.ColumnModel.AddColumn("RefForecastType", "RefForecastType", "RefForecastType", DataAttributeTypes.dtInteger, Mandatory.NotNull);
            
            cb = gp.ColumnModel.AddColumn("R1", "R1", String.Format("Отчет {0}", year - 2), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    var a = record.data.Signat.split('_I');
    if ((a.length == 2) && ((a[1][0] & 0x02) != 0))
    {
        metadata.style += 'background-color: #d0ffd0;';
    }
    return value;
}"; 

            cb = gp.ColumnModel.AddColumn("R2", "R2", String.Format("Отчет {0}", year - 1), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            cb.Renderer.Fn = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    var a = record.data.Signat.split('_I');
    if ((a.length == 2) && ((a[1][0] & 0x01) != 0))
    {
        metadata.style += 'background-color: #d0ffd0;';
    }
    return value;
}"; 

            cb = gp.ColumnModel.AddColumn("Est", "Est", String.Format("Оценка {0}", year), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(6);

            cb = gp.ColumnModel.AddColumn("Y1", "Y1", String.Format("Прогноз {0}", year + 1), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(6);

            cb = gp.ColumnModel.AddColumn("Y2", "Y2", String.Format("Прогноз {0}", year + 2), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(6);

            cb = gp.ColumnModel.AddColumn("Y3", "Y3", String.Format("Прогноз {0}", year + 3), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(6);
            ////gp.ColumnModel.AddColumn("YearOf", "YearOf", "Год", DataAttributeTypes.dtInteger, Mandatory.NotNull);
            ////gp.ColumnModel.AddColumn("Signat", "Signat", "Сигнатура", DataAttributeTypes.dtString, Mandatory.NotNull);
            gp.ColumnModel.AddColumn("GroupName", "GroupName", "Группа", DataAttributeTypes.dtString, Mandatory.NotNull);

            ////Перенос по словам
            gp.AddColumnsWrapStylesToPage(page);
            
            ////выбор строками
            gp.SelectionModel.Add(new RowSelectionModel());

            return new List<Component> { gp, InsertWindow(page), FillFromScenWindow(page) };
        }

        public Component ChartPanel(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            Store storeChart = CreateChartStore();
            page.Controls.Add(storeChart);

            page.Controls.Add(CreateForm2pVar2());

            Panel panel = new Panel
            {
                ID = "mainPanel",
                Height = 250,
                Border = false,
                Collapsible = false,
                LabelWidth = 150,
                LabelAlign = LabelAlign.Right,
                Padding = 10,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Layout = "form"
            };
            
            Toolbar toolbar = new Toolbar
            {
                ID = "chartToolbar"
            };

            panel.TopBar.Add(toolbar);

            ComboBox cboxVar2 = new ComboBox
            {
                ID = "cboxVar2",
                Width = 400,
                FieldLabel = "Форма-2п для \"Вариант 2\"",
                StoreID = "dsForm2pVar2",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                DisplayField = "Text",
                ValueField = "Value",
                PageSize = 10
            };

            cboxVar2.Items.Add(new ListItem("(пусто)", "-1"));
            cboxVar2.SelectedIndex = 0;

            cboxVar2.Listeners.Select.AddAfter("if (chart1.isVisible()) { stChartData.reload(); }");

            toolbar.Items.Add(cboxVar2);
            
            Panel chartPanel = new Panel
            {
                ID = "chartPanel",
                Layout = "fit",
                Height = 200,
                ////utoHeight = true,
                ////ColumnWidth = 0.7
            };

            panel.Items.Add(chartPanel);

            string script = @"
function panelLoaded(){
    chartPanel.add(new Ext.chart.LineChart({
        xtype: 'linechart',    
        id : 'chart1',
        title: 'Параметры Формы-2п',
        url: '/Content/charts.swf',        
        store: stChartData,        
        xField: 'Year',
        layout: 'fit',
        xAxis: new Ext.chart.CategoryAxis({
                    title: 'Год',
                    orientation: 'horizontal'                
            }),    
        yAxis: new Ext.chart.NumericAxis({
                    title: 'Параметр',
                    orientation: 'vertical'
            }),    
        series: [{
                type: 'line',                    
                yField: 'Xv1',
                displayName: 'Вариант 1',                                
                showInLegend: true,
                style: {
                    color:0xFF8888
                }
            },
            {
                type: 'line',                
                yField: 'Xv2',   
                displayName: 'Вариант 2',                
                showInLegend: true,
                style: {
                    color:0x99BBE8
                }
            }
            ],
        extraStyle: {
                yAxis:  {
                            titleRotation: -90
                        },
                legend: {   
                            display: 'right'   
                        }
            }
    }));
 
    chartPanel.doLayout();
}";

            ////Ext.chart.
            resourceManager.RegisterClientScriptBlock("panelLoaded", script);
            /*chartContainer.AddScript(script);*/

            panel.Listeners.AfterRender.AddAfter("panelLoaded(); "); ////stChartData.load();

            panel.Listeners.BeforeCollapse.AddAfter("chart1.setVisible(false);");
            panel.Listeners.Expand.AddAfter("chart1.setVisible(true)");

            return panel;
        }
        
        public Component InsertWindow(ViewPage page)
        {
            Window insertWindow = new Window
            {
                ID = "insertWindow",
                Width = 600,
                Height = 195,
                Hidden = true,
                Modal = true,
                Title = "Вставка значений ранее спрогнозированных показателей",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            FormPanel formPanel = new FormPanel
            {
                ButtonAlign = Alignment.Right,
                ////Border = false,
                MonitorPoll = 500,
                MonitorValid = true,
                Padding = 10,
                LabelPad = -5,
                Width = 586
            };

            ComboBox cboxPlanVar = new ComboBox
            {
                ID = "cbPlanVar",
                Name = "cbPlanVar",
                FieldLabel = "Вариант прогноза:",
                Width = 450,
                StoreID = "dsPlanVar",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                DisplayField = "Text",
                ValueField = "Value",
                PageSize = 10
            };

            cboxPlanVar.Template.Html = @"
<tpl for=""."">
    <tpl if=""[xindex] == 1""><table><tr><th><b>Показатель</b></th><th><b>Наименование варианта</b></th><th><b>Период</b></th></tr></tpl>
	
    <tr class = ""list-item"">
		<td style=""padding:3px 0px;"">{Group}</td>		
        <td>{Text}</td>
        <td>{Year}</td>
	</tr>

	<tpl if=""[xcount] == [xindex]""></table></tpl>
</tpl>";
            cboxPlanVar.ItemSelector = "tr.list-item";

            cboxPlanVar.DirectEvents.Select.Url = "/Form2pValues/InsertFromPlan";
            cboxPlanVar.DirectEvents.Select.CleanRequest = true;
            cboxPlanVar.DirectEvents.Select.ExtraParams.Add(new Parameter("varid", "cbPlanVar.getSelectedItem().value", ParameterMode.Raw));
            cboxPlanVar.DirectEvents.Select.ExtraParams.Add(new Parameter("estYear", year.ToString(), ParameterMode.Value));

            TableLayout layout = new TableLayout
            {
                Columns = 6,
                LabelAlign = LabelAlign.Top,
                ColumnWidth = (double)1 / 6,
                Width = 565
            };

            TextField txtfR1 = new TextField 
            {
                Name = "txtfR1",
                ID = "txtfR1",
                FieldLabel = "Отчет {0}".FormatWith(year - 2), 
                Width = 70, 
                ReadOnly = true 
            };
            Checkbox cboxR1 = new Checkbox 
            {
                Name = "cboxR1",
                ID = "cboxR1",
                Checked = false, 
                Width = 95 
            };

            TextField txtfR2 = new TextField 
            {
                Name = "txtfR2",
                ID = "txtfR2",
                FieldLabel = "Отчет {0}".FormatWith(year - 1), 
                Width = 70, 
                ReadOnly = true 
            };
            Checkbox cboxR2 = new Checkbox 
            {
                Name = "cboxR2",
                ID = "cboxR2",
                Checked = false, 
                Width = 95 
            };

            TextField txtfEst = new TextField 
            {
                Name = "txtfEst",
                ID = "txtfEst",
                FieldLabel = "Оценка {0}".FormatWith(year), 
                Width = 70, 
                ReadOnly = true 
            };
            Checkbox cboxEst = new Checkbox 
            {
                Name = "cboxEst",
                ID = "cboxEst",
                Checked = false, 
                Width = 95 
            };
            
            TextField txtfY1 = new TextField 
            {
                Name = "txtfY1",
                ID = "txtfY1",
                FieldLabel = "Прогноз {0}".FormatWith(year + 1), 
                Width = 70, 
                ReadOnly = true 
            };

            Checkbox cboxY1 = new Checkbox 
            {
                Name = "cboxY1",
                ID = "cboxY1",
                Checked = false, 
                Width = 95 
            };
            
            TextField txtfY2 = new TextField 
            {
                Name = "txtfY2",
                ID = "txtfY2",
                FieldLabel = "Прогноз {0}".FormatWith(year + 2), 
                Width = 70, 
                ReadOnly = true 
            };

            Checkbox cboxY2 = new Checkbox 
            {
                Name = "cboxY2",
                ID = "cboxY2",
                Checked = false, 
                Width = 95 
            };

            TextField txtfY3 = new TextField 
            {
                Name = "txtfY3",
                ID = "txtfY3",
                FieldLabel = "Прогноз {0}".FormatWith(year + 3), 
                Width = 70, 
                ReadOnly = true 
            };

            Checkbox cboxY3 = new Checkbox 
            { 
                Name = "cboxY3",
                ID = "cboxY3",
                Checked = false, 
                Width = 95 
            };

            Cell varName = new Cell { ColSpan = 6, Items = { cboxPlanVar } };

            Cell cellCbR1 = new Cell { Items = { cboxR1 } };
            Cell cellCbR2 = new Cell { Items = { cboxR2 } };
            Cell cellCbEst = new Cell { Items = { cboxEst } };
            Cell cellCbY1 = new Cell { Items = { cboxY1 } };
            Cell cellCbY2 = new Cell { Items = { cboxY2 } };
            Cell cellCbY3 = new Cell { Items = { cboxY3 } };

            Cell cellTfR1 = new Cell { Items = { txtfR1 } };
            Cell cellTfR2 = new Cell { Items = { txtfR2 } };
            Cell cellTfEst = new Cell { Items = { txtfEst } };
            Cell cellTfY1 = new Cell { Items = { txtfY1 } };
            Cell cellTfY2 = new Cell { Items = { txtfY2 } };
            Cell cellTfY3 = new Cell { Items = { txtfY3 } };
            
            layout.Cells.Add(varName);
            
            layout.Cells.Add(cellTfR1);
            layout.Cells.Add(cellTfR2);
            layout.Cells.Add(cellTfEst);
            layout.Cells.Add(cellTfY1);
            layout.Cells.Add(cellTfY2);
            layout.Cells.Add(cellTfY3);

            layout.Cells.Add(cellCbR1);
            layout.Cells.Add(cellCbR2);
            layout.Cells.Add(cellCbEst);
            layout.Cells.Add(cellCbY1);
            layout.Cells.Add(cellCbY2);
            layout.Cells.Add(cellCbY3);
            
            ////formPanel.Items.Add(cboxPlanVar);

            formPanel.Items.Add(layout);

            Button btnInsertCancel = new Button
            {
                ID = "btnInsertCancel",
                Text = "Отмена"
            };

            btnInsertCancel.Listeners.Click.Handler = "insertWindow.hide();";

            Button btnInsertOk = new Button
            {
                ID = "btnInsertOk",
                Text = "Вставить"
            };

            btnInsertOk.DirectEvents.Click.Url = "/Form2pValues/InsertFromVar";
            btnInsertOk.DirectEvents.Click.CleanRequest = true;
            btnInsertOk.DirectEvents.Click.EventMask.ShowMask = true;
            btnInsertOk.DirectEvents.Click.ExtraParams.Add(new Parameter("paramid", "{0}.getSelectionModel().hasSelection() ? {0}.getSelectionModel().getSelected().id : -1".FormatWith(GridId), ParameterMode.Raw));
            btnInsertOk.DirectEvents.Click.ExtraParams.Add(new Parameter("r1", "cboxR1.getValue() ? txtfR1.getValue() : \"\"", ParameterMode.Raw));
            btnInsertOk.DirectEvents.Click.ExtraParams.Add(new Parameter("r2", "cboxR2.getValue() ? txtfR2.getValue() : \"\"", ParameterMode.Raw));
            btnInsertOk.DirectEvents.Click.ExtraParams.Add(new Parameter("est", "cboxEst.getValue() ? txtfEst.getValue() : \"\"", ParameterMode.Raw));
            btnInsertOk.DirectEvents.Click.ExtraParams.Add(new Parameter("y1", "cboxY1.getValue() ? txtfY1.getValue() : \"\"", ParameterMode.Raw));
            btnInsertOk.DirectEvents.Click.ExtraParams.Add(new Parameter("y2", "cboxY2.getValue() ? txtfY2.getValue() : \"\"", ParameterMode.Raw));
            btnInsertOk.DirectEvents.Click.ExtraParams.Add(new Parameter("y3", "cboxY3.getValue() ? txtfY3.getValue() : \"\"", ParameterMode.Raw));
            
            ////btnInsertOk.DirectEvents.Click.ExtraParams.Add(new Parameter("year", "2008", ParameterMode.Value));

            /////btnRefresh.DirectEvents.Click.Success = "panelStat.reload(); panelProg.reload();stChartData.reload();"; ////"stProgData.load();stStaticData.load();";

            formPanel.Buttons.Add(btnInsertCancel);
            formPanel.Buttons.Add(btnInsertOk);
            
            insertWindow.Items.Add(formPanel);

            string clearFunction = @"
function clearInsertForm()
{
    cbPlanVar.clear();
    txtfR1.clear();
    txtfR2.clear();
    txtfEst.clear();
    txtfY1.clear();
    txtfY2.clear();
    txtfY3.clear();        
    ////cboxR1.
}";

            var resourceManager = ResourceManager.GetInstance(page);
            resourceManager.RegisterClientScriptBlock("clearInsertForm", clearFunction);
            ////insertWindow.AddScript(clearFunction);

            return insertWindow;
        }

        public Component FillFromScenWindow(ViewPage page)
        {
            Window fillFromScenWindow = new Window
            {
                ID = "wndFillFromScen",
                Width = 600,
                Height = 150,
                Layout = "FitLayout",
                Hidden = true,
                Modal = true,
                Title = "Вставка значений из ранее спрогнозированного сценария",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Border = false
            };

            fillFromScenWindow.Listeners.Show.AddBefore("dsScenVar.reload();");

            FormPanel formPanel = new FormPanel
            {
                ButtonAlign = Alignment.Right,
                Layout = "fit",
                ////Border = false,
                MonitorPoll = 500,
                MonitorValid = true,
                Padding = 10,
                LabelPad = -5,
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            Panel singlePanel = new Panel
            {
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            formPanel.Items.Add(singlePanel);
            
            singlePanel.Defaults.Add(new Parameter("AllowBlank", "false", ParameterMode.Raw));
            singlePanel.Defaults.Add(new Parameter("MsgTarget", "false"));

            ComboBox cboxScenVar = new ComboBox
            {
                ID = "cboxScenVar",
                Name = "cboxScenVar",
                FieldLabel = "Вариант сценария:",
                Width = 450,
                StoreID = "dsScenVar",
                ForceSelection = true,
                TypeAhead = true,
                SelectOnFocus = true,
                Mode = DataLoadMode.Local,
                TriggerAction = TriggerAction.All,
                Editable = false,
                DisplayField = "Text",
                ValueField = "Value",
                PageSize = 10
            };

            singlePanel.Items.Add(cboxScenVar);

            fillFromScenWindow.Items.Add(formPanel);

            Button btnFillCancel = new Button
             {
                 ID = "btnFillCancel",
                 Text = "Отмена"
             };

            btnFillCancel.Listeners.Click.Handler = "wndFillFromScen.hide();";

            Button btnFillOk = new Button
            {
                ID = "btnFillOk",
                Text = "Вставить"
            };

            btnFillOk.DirectEvents.Click.Url = "/Form2pValues/FillFromScen";
            btnFillOk.DirectEvents.Click.CleanRequest = true;
            btnFillOk.DirectEvents.Click.EventMask.ShowMask = true;
            btnFillOk.DirectEvents.Click.ExtraParams.Add(new Parameter("scenid", "cboxScenVar.getSelectedItem().value", ParameterMode.Raw));
            btnFillOk.DirectEvents.Click.ExtraParams.Add(new Parameter("id2p", varId.ToString(), ParameterMode.Value));
            btnFillOk.DirectEvents.Click.Success = "wndFillFromScen.hide();{0}.reload();".FormatWith(StoreId);
            
            formPanel.Buttons.Add(btnFillCancel);
            formPanel.Buttons.Add(btnFillOk);

            formPanel.BottomBar.Add(new StatusBar { Height = 25 });

            formPanel.Listeners.ClientValidation.Handler = @"
if (!valid)
{
    this.getBottomToolbar().setStatus({text : 'Не все поля заполнены!', iconCls : 'icon-exclamation'});
    btnFillOk.setDisabled(true);
}
else
{
    this.getBottomToolbar().setStatus({text : '', iconCls : 'icon-accept'});
    btnFillOk.setDisabled(false);
}
";

            return fillFromScenWindow;
        }

        public Store CreateStore()
        {
            Store store = new Store { ID = StoreId, AutoLoad = true, GroupField = "GroupName" };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID", RecordFieldType.Int);
            reader.Fields.Add("RefParametrs", RecordFieldType.Int);
            reader.Fields.Add("Units", RecordFieldType.String);
            ////reader.Fields.Add("RefForecastType", RecordFieldType.Int);
            reader.Fields.Add("R1", RecordFieldType.Auto);
            reader.Fields.Add("R2", RecordFieldType.Auto);
            reader.Fields.Add("Est", RecordFieldType.Auto);
            reader.Fields.Add("Y1", RecordFieldType.Auto);
            reader.Fields.Add("Y2", RecordFieldType.Auto);
            reader.Fields.Add("Y3", RecordFieldType.Auto);
            ////reader.Fields.Add("YearOf", RecordFieldType.Int);
            reader.Fields.Add("Signat", RecordFieldType.String);
            reader.Fields.Add("GroupName", RecordFieldType.String);
            reader.Fields.Add("ParamName", RecordFieldType.String);
            reader.Fields.Add("Code", RecordFieldType.Int);

            ////reader.Fields.Add("refParam");

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pValues/Load",
                Method = HttpMethod.POST
            });

            store.Sort("Code", SortDirection.ASC);

            store.WriteBaseParams.Add(new Parameter("savedData", String.Format("{0}.getChangedData()", StoreId), ParameterMode.Raw));
            store.WriteBaseParams.Add(new Parameter("estYear", year.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("varId", varId.ToString(), ParameterMode.Value));

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/Form2pValues/Save",
                Method = HttpMethod.POST,
                Timeout = 50000
            });
            
            return store;
        }

        private Store CreatePlanVarStore()
        {
            Store store = new Store { ID = "dsPlanVar", AutoLoad = false };

            ////store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };
            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);
            reader.Fields.Add("Group", RecordFieldType.String);
            reader.Fields.Add("Year", RecordFieldType.String);
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("id", String.Format("{0}.getSelectionModel().hasSelection() ? {0}.getSelectionModel().getSelected().id : -1", GridId), ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("estYear", year.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("signat", String.Format("{0}.getSelectionModel().hasSelection() ? {0}.getSelectionModel().getSelected().data.Signat : ''", GridId), ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pValues/LoadComboPlanVar",
                Method = HttpMethod.POST
            });

            return store;
        }

        private Store CreateScenVarStore()
        {
            Store store = new Store { ID = "dsScenVar", AutoLoad = false };

            ////store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };
            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("estYear", year.ToString(), ParameterMode.Value));
            
            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pValues/LoadComboScenVar",
                Method = HttpMethod.POST
            });

            return store;
        }

        private Store CreateChartStore()
        {
            Store store = new Store { ID = "stChartData", AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data" };
            store.Reader.Add(reader);
            reader.Fields.Add("Year");
            reader.Fields.Add("Xv1");
            reader.Fields.Add("Xv2");

            ////store.BaseParams.Add(new Parameter("pyears", "parent.sfProgToYear.getValue() - parent.sfProgFromYear.getValue()", ParameterMode.Raw));
            ////store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));

            store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("paramId", "gpForm2pValues.getSelectionModel().getSelected().data.RefParametrs", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("varId2", "cboxVar2.getSelectedItem().value", ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pValues/LoadChart",  ////progchartdata
                Method = HttpMethod.POST
            });

            store.Listeners.Load.AddAfter("eval(store.reader.jsonData.extraParams);");

            return store;
        }

        private Store CreateForm2pVar2()
        {
            Store store = new Store { ID = "dsForm2pVar2", AutoLoad = true };

            ////store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "Value" };
            reader.Fields.Add("Text", RecordFieldType.String);
            reader.Fields.Add("Value", RecordFieldType.String);
            
            store.Reader.Add(reader);

            ////store.BaseParams.Add(new Parameter("id", , ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("estYear", year.ToString(), ParameterMode.Value));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/Form2pValues/LoadComboForm2pVar2",
                Method = HttpMethod.POST
            });
            
            return store;
        }
    }
}
