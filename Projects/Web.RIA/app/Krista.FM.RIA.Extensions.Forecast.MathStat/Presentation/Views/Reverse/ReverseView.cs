using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class ReverseView : View
    {
        private readonly IForecastExtension extension;
        private readonly IForecastVariantsRepository variantsRepository;
        private readonly IForecastReverseRepository reverseVarRepository;
        private readonly IRepository<FX_Date_YearDayUNV> yearRepository;
        private readonly IForecastParamsRepository paramsRepository;
        private readonly IForecastReverseValuesRepository valuesRepository;

        private string key;

        private string name = String.Empty;
        private int dataFrom;
        private int dataTo;
        private int progFrom;
        private int progTo;
        private int status = -1;
        private int group = -1;
        private int method = -1;
        private int paramId;
        
        public ReverseView(
                IForecastExtension extension,
                IForecastVariantsRepository variantsRepository,
                IRepository<FX_Date_YearDayUNV> yearRepository,
                IForecastReverseRepository reverseVarRepository,
                IForecastParamsRepository paramsRepository,
                IForecastReverseValuesRepository valuesRepository)
        {
            this.extension = extension;
            this.variantsRepository = variantsRepository;
            this.yearRepository = yearRepository;
            this.reverseVarRepository = reverseVarRepository;
            this.paramsRepository = paramsRepository;
            this.valuesRepository = valuesRepository;
        }

        public int VarId { get; private set; }
        
        public void Initialize(int varid, int parentId)
        {
            VarId = varid;
            key = String.Format("reverseForm_{0}", VarId);

            UserFormsControls ufc = extension.Forms[key];

            ufc.AddObject("VarId", varid);

            if (VarId != -1)
            {
                var variant = variantsRepository.FindOne(parentId);

                name = variant.Name;
                status = variant.Status;

                XDocument xDoc = XDocument.Parse(variant.XMLString);

                progFrom = Convert.ToInt32(xDoc.Root.Attribute("from").Value);
                progTo = Convert.ToInt32(xDoc.Root.Attribute("to").Value);

                var usedDatas = xDoc.Root.Element("UsedDatas");

                paramId = Convert.ToInt32(usedDatas.Attribute("fparamid").Value);

                method = Convert.ToInt32(usedDatas.Attribute("method").Value);
                group = Convert.ToInt32(usedDatas.Attribute("group").Value);

                var usedYears = xDoc.Root.Element("UsedYears");

                dataFrom = Convert.ToInt32(usedYears.Attribute("from").Value);
                dataTo = Convert.ToInt32(usedYears.Attribute("to").Value);
            }
            else
            {
            }

            ufc.ParamId = paramId;

            DataTable resultTable = new DataTable();
            
            resultTable.Columns.Add("id", typeof(int));
            resultTable.Columns.Add("Param", typeof(string));

            ufc.AddObject("ResultTable", resultTable);
        }

        public void LoadSavedData(int varId)
        {
            var ufc = this.extension.Forms[key];
            DataTable datY = ufc.DataService.GetProgData();
            DataTable datFactors = ufc.DataService.GetStaticData();
            DataTable resultTable = null;

            if (ufc.Contains("ResultTable"))
            {
                resultTable = ufc.GetObject("ResultTable") as DataTable;
                resultTable.PrimaryKey = new DataColumn[] { resultTable.Columns["id"] };
            }

            foreach (DataRow row in datY.Rows)
            {
                int paramid = Convert.ToInt32(row["id"]);

                foreach (DataColumn column in datY.Columns)
                {
                    if (column.ColumnName.Contains("year_"))
                    {
                        var colName = column.ColumnName;
                        var year = Convert.ToInt32(colName.Replace("year_", String.Empty));
                        var yearRef = yearRepository.Get((year * 10000) + 1);
                        var varRef = reverseVarRepository.FindOne(varId);
                        var paramRef = paramsRepository.FindOne(paramid);
                        
                        var existValue = from t in valuesRepository.FindAll()
                                         where (t.RefRev == varRef) && (t.RefDate == yearRef) && (t.RefParam == paramRef)
                                         select t.Value;
                        var lst = existValue.ToList();
                        if (lst.Count() > 0)
                        {
                            row[colName] = existValue.First();
                        }
                    }
                }
            }
            
            foreach (DataRow row in datFactors.Rows)
            {
                int paramid = Convert.ToInt32(row["id"]);

                foreach (DataColumn column in datY.Columns)
                {
                    if (column.ColumnName.Contains("year_"))
                    {
                        var colName = column.ColumnName;
                        var year = Convert.ToInt32(colName.Replace("year_", String.Empty));

                        var yearRef = yearRepository.Get((year * 10000) + 1);
                        var varRef = reverseVarRepository.FindOne(varId);
                        var paramRef = paramsRepository.FindOne(paramid);

                        var existValue = from t in valuesRepository.FindAll()
                                         where (t.RefRev == varRef) && (t.RefDate == yearRef) && (t.RefParam == paramRef)
                                         select t.Value;

                        if (existValue.ToList().Count() > 0)
                        {
                            if (year <= dataTo)
                            {
                                if (!datFactors.Columns.Contains(colName))
                                {
                                    datFactors.Columns.Add(colName, typeof(double));
                                }

                                row[colName] = existValue.First();
                            }
                            else
                            {
                                if (resultTable != null)
                                {
                                    if (!resultTable.Columns.Contains(colName))
                                    {
                                        resultTable.Columns.Add(colName, typeof(double));
                                    }

                                    DataRow resultRow = resultTable.Rows.Find(row["id"]);

                                    if (resultRow == null)
                                    {
                                        resultRow = resultTable.NewRow();
                                        resultRow["id"] = row["id"];
                                        resultRow["Param"] = row["Param"];

                                        resultTable.Rows.Add(resultRow);
                                    }

                                    resultRow[colName] = existValue.First();
                                }
                            }
                        }
                    }
                }
            }
        }

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateYStore());
            page.Controls.Add(CreateFactorsStore());
            page.Controls.Add(CreateRegsStore());

            DataTable datY = this.extension.Forms[key].DataService.GetProgData();
            DataTable datFactors = this.extension.Forms[key].DataService.GetStaticData();
            DataTable datRegs = this.extension.Forms[key].DataService.GetRegulatorData();

            int cntFactorsRow = datFactors.Rows.Count > 5 ? 5 : datFactors.Rows.Count;
            int cntYRow = datY.Rows.Count + 1 > 5 ? 5 : datY.Rows.Count + 1;
            int cntRegsRow = datRegs.Rows.Count + 1 > 5 ? 5 : datRegs.Rows.Count + 1;
            
            BorderLayout layout = new BorderLayout
            {
                ID = "layoutMain"
            };

            Panel centerPanel = new Panel
            {
                Border = false,
                AutoScroll = true
            };

            layout.North.Items.Add(CreateToolBar());
            layout.Center.Items.Add(centerPanel);

            Panel panelY = new Panel
            {
                ID = "panelY",
                Collapsible = true,
                Height = (50 + 15) + (cntYRow * 45) + 45,
                Title = "Значения зависимой величины"
            };

            panelY.Items.Add(YGrid(page));
            
            Panel panelFactors = new Panel
            {
                ID = "panelFactors",
                Collapsible = true,
                Height = (50 + 15) + (cntFactorsRow * 45) + 45,
                Title = "Прогнозируемые значения факторов"
            };

            panelFactors.Items.Add(FactorsGrid(page));
            
            centerPanel.Items.Add(new List<Component> { panelY, panelFactors });

            if (cntRegsRow > 1)
            {
                Panel panelRegs = new Panel
                {
                    ID = "panelRegs",
                    Collapsible = true,
                    Height = (50 + 15) + (cntRegsRow * 45) + 45,
                    Title = "Нерегулируемые регуляторы"
                };

                panelRegs.Items.Add(RegsGrid(page));

                centerPanel.Items.Add(panelRegs);
            }

            layout.South.Items.Add(ChartPanel(page));
            layout.South.Collapsible = true;
            
            Viewport viewport = new Viewport { ID = "viewportMain", Layout = "center" };
            viewport.Items.Add(layout);

            return new List<Component> { viewport };
        }

        public Component ChartPanel(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            Store storeChart = CreateChartStore();
            page.Controls.Add(storeChart);
            
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
        title: 'Параметры обратного прогнозирования',
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
                yField: 'Xp',
                displayName: 'Прогноз',                                
                showInLegend: true,
                style: {
                    color:0xFF8888
                }
            },
            {
                type: 'line',                
                yField: 'Xs',   
                displayName: 'Статистика',                
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
            
            panel.Listeners.AfterRender.AddAfter("panelLoaded(); "); ////stChartData.load();

            panel.Listeners.BeforeCollapse.AddAfter("chart1.setVisible(false);");
            panel.Listeners.Expand.AddAfter("chart1.setVisible(true)");

            return panel;
        }

        private Component CreateToolBar()
        {
            FormPanel toolbarPanel = new FormPanel
            {
                Border = false,
                ////AutoHeight = true,
                Height = 0,
                ////Width = 400,
                Collapsible = false,
                LabelWidth = 125,
                LabelAlign = LabelAlign.Right,
                Padding = 5,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Layout = "form"
            };

            Toolbar toolbar = new Toolbar
            {
                ID = "toolbar"
            };

            Button btnSave = new Button
            {
                ID = "btnSave",
                ToolTip = "Сохранить расчет",
                Icon = Icon.TableSave
            };

            btnSave.DirectEvents.Click.Url = "/ReverseData/Save";
            btnSave.DirectEvents.Click.CleanRequest = true;
            btnSave.DirectEvents.Click.EventMask.ShowMask = true;
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("key", key, ParameterMode.Value));
            btnSave.DirectEvents.Click.ExtraParams.Add(new Parameter("year", dataTo.ToString(), ParameterMode.Value));

            toolbar.Items.Add(btnSave);

            Button btnCalc = new Button
            {
                ID = "btnCalc",
                ToolTip = "Расчет обратной задачи",
                Icon = Icon.Calculator
            };

            btnCalc.DirectEvents.Click.Url = "/ReverseData/Calc";
            btnCalc.DirectEvents.Click.CleanRequest = true;
            btnCalc.DirectEvents.Click.EventMask.ShowMask = true;
            btnCalc.DirectEvents.Click.ExtraParams.Add(new Parameter("key", key, ParameterMode.Value));
            btnCalc.DirectEvents.Click.ExtraParams.Add(new Parameter("year", dataTo.ToString(), ParameterMode.Value));
            btnCalc.DirectEvents.Click.Success = "dsFactors.reload(); if (chart1.isVisible() && grpFactors.getSelectionModel().hasSelection()) { stChartData.reload(); }";

            toolbar.Items.Add(btnCalc);

            toolbarPanel.TopBar.Add(toolbar);

            return toolbarPanel;
        }

        private Component YGrid(ViewPage page)
        {
            DataTable datY = this.extension.Forms[key].DataService.GetProgData();

            int cntYRow = datY.Rows.Count + 1 > 5 ? 5 : datY.Rows.Count + 1;

            GridPanel grpY = new GridPanel
            {
                ID = "grpY",
                StoreID = "dsY",
                MonitorResize = true,
                Border = false,
                Height = (50 + 15) + (cntYRow * 45) + 45,
                AutoScroll = true,
                ColumnLines = true
            };

            ColumnModel cm = grpY.ColumnModel;

            cm.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.Nullable).SetVisible(false);
            cm.AddColumn("Param", "Param", "Показатель", DataAttributeTypes.dtInteger, Mandatory.Nullable).SetWidth(150);
            
            List<string> list = new List<string>();
            foreach (DataColumn col in datY.Columns)
            {
                string colName = col.ColumnName;
                if (colName.Contains("year_"))
                {
                    list.Add(colName);
                }
            }

            list.Sort();

            foreach (string colName in list)
            {
                string headName = colName.Replace("year_", String.Empty);
                int colYear = Convert.ToInt32(headName);
                ColumnBase col = cm.AddColumn(colName, colName, headName, DataAttributeTypes.dtInteger, Mandatory.Nullable).SetWidth(80);

                if (colYear > dataTo)
                {
                    col.SetEditableDouble(6);
                }
            }

            string editFn = String.Format(
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
    Ext.net.DirectEvent.confirmRequest(
    {{
	    cleanRequest: true,
	    isUpload: false,
	    url: '/ReverseData/ChangeData?rowid='+e.record.data.id+'&col='+this.getColumnModel().getDataIndex(e.column)+'&newVal='+e.value+'&key={0}',
        control: this,
        userSuccess: function(response, result, el, type, action, extraParams){{  }}
    }});",
key);
            grpY.Listeners.AfterEdit.AddAfter(editFn);

            grpY.AddColumnsWrapStylesToPage(page);

            return grpY;
        }

        private Component FactorsGrid(ViewPage page)
        {
            DataTable datFactors = this.extension.Forms[key].DataService.GetStaticData();
            DataTable datY = this.extension.Forms[key].DataService.GetProgData();

            int cntFactorsRow = datFactors.Rows.Count > 5 ? 5 : datFactors.Rows.Count;
            
            GridPanel gpanelFactors = new GridPanel
            {
                ID = "grpFactors",
                StoreID = "dsFactors",
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                Height = (50 + 15) + (cntFactorsRow * 45) + 45,
                ColumnLines = true
            };

            ColumnModel cm = gpanelFactors.ColumnModel;

            cm.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.Nullable).SetVisible(false);
            cm.AddColumn("Param", "Param", "Показатель", DataAttributeTypes.dtInteger, Mandatory.Nullable).SetWidth(150);

            List<string> list = new List<string>();
            /*foreach (DataColumn col in datFactors.Columns)
            {
                string colName = col.ColumnName;
                if (colName.Contains("year_"))
                {
                    list.Add(colName);
                }
            }*/

            foreach (DataColumn col in datY.Columns)
            {
                string colName = col.ColumnName;
                if (colName.Contains("year_"))
                {
                    ////if (!list.Contains(colName))
                    {
                        list.Add(colName);
                    }
                }
            }

            list.Sort();

            foreach (string colName in list)
            {
                string headName = colName.Replace("year_", String.Empty);
                ColumnBase col = cm.AddColumn(colName, colName, headName, DataAttributeTypes.dtInteger, Mandatory.Nullable).SetWidth(80);
            }

            gpanelFactors.AddColumnsWrapStylesToPage(page);

            ////выбор строками
            gpanelFactors.SelectionModel.Add(new RowSelectionModel());

            gpanelFactors.Listeners.CellClick.AddAfter("if (chart1.isVisible()) { stChartData.reload(); }");

            return gpanelFactors;
        }

        private Component RegsGrid(ViewPage page)
        {
            DataTable datRegs = this.extension.Forms[key].DataService.GetRegulatorData();
            
            int cntRegsRow = datRegs.Rows.Count > 5 ? 5 : datRegs.Rows.Count;

            GridPanel gpanelRegs = new GridPanel
            {
                ID = "grpRegs",
                StoreID = "dsRegs",
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                Height = (50 + 15) + (cntRegsRow * 45) + 45,
                ColumnLines = true
            };

            ColumnModel cm = gpanelRegs.ColumnModel;

            cm.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.Nullable).SetVisible(false);
            cm.AddColumn("Param", "Param", "Показатель", DataAttributeTypes.dtInteger, Mandatory.Nullable).SetWidth(150);

            List<string> list = new List<string>();
            
            foreach (DataColumn col in datRegs.Columns)
            {
                string colName = col.ColumnName;
                if (colName.Contains("year_"))
                {
                    list.Add(colName);
                }
            }

            list.Sort();

            foreach (string colName in list)
            {
                string headName = colName.Replace("year_", String.Empty);
                ColumnBase col = cm.AddColumn(colName, colName, headName, DataAttributeTypes.dtInteger, Mandatory.Nullable).SetWidth(80);
            }

            gpanelRegs.AddColumnsWrapStylesToPage(page);

            ////выбор строками
            gpanelRegs.SelectionModel.Add(new RowSelectionModel());
            
            return gpanelRegs;
        }

        private Store CreateYStore()
        {
            DataTable datY = this.extension.Forms[key].DataService.GetProgData();

            Store store = new Store
            {
                AutoLoad = true,
                ID = "dsY"
            };

            store.DirectEventConfig.EventMask.ShowMask = false;
            
            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            
            foreach (DataColumn c in datY.Columns)
            {
                string colName = c.ColumnName;
                RecordFieldType recType;
                switch (c.DataType.FullName)
                {
                    case "System.String":
                        recType = RecordFieldType.String;
                        break;
                    case "System.Double":
                        recType = RecordFieldType.Float;
                        break;
                    default: recType = RecordFieldType.Auto;
                        break;
                }

                reader.Fields.Add(colName, recType);
            }

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("key", key, ParameterMode.Value));

            HttpProxy httpProxy = new HttpProxy
            {
                Url = "/ReverseData/LoadY",
                Method = HttpMethod.POST
            };

            store.Proxy.Add(httpProxy);

            return store;
        }

        private Store CreateFactorsStore()
        {
            ////DataTable datFactors = this.extension.Forms[key].DataService.GetStaticData();
            DataTable datY = this.extension.Forms[key].DataService.GetProgData();

            Store store = new Store
            {
                AutoLoad = true,
                ID = "dsFactors"
            };

            store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            
            foreach (DataColumn c in datY.Columns)
            {
                string colName = c.ColumnName;
                RecordFieldType recType;
                switch (c.DataType.FullName)
                {
                    case "System.String":
                        recType = RecordFieldType.String;
                        break;
                    case "System.Double":
                        recType = RecordFieldType.Float;
                        break;
                    default: recType = RecordFieldType.Auto;
                        break;
                }

                reader.Fields.Add(colName, recType);
            }

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("key", key, ParameterMode.Value));

            HttpProxy httpProxy = new HttpProxy
            {
                Url = "/ReverseData/LoadFactors",
                Method = HttpMethod.POST
            };

            store.Proxy.Add(httpProxy);

            return store;
        }

        private Store CreateChartStore()
        {
            Store store = new Store { ID = "stChartData", AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data" };
            store.Reader.Add(reader);
            reader.Fields.Add("Year");
            reader.Fields.Add("Xp");
            reader.Fields.Add("Xs");

            ////store.BaseParams.Add(new Parameter("pyears", "parent.sfProgToYear.getValue() - parent.sfProgFromYear.getValue()", ParameterMode.Raw));
            ////store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));

            store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("paramId", "grpFactors.getSelectionModel().getSelected().data.id", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("expYear", dataTo.ToString(), ParameterMode.Value));
            
            store.Proxy.Add(new HttpProxy
            {
                Url = "/ReverseData/LoadChart",  ////progchartdata
                Method = HttpMethod.POST
            });

            store.Listeners.Load.AddAfter("eval(store.reader.jsonData.extraParams);");

            return store;
        }

        private Store CreateRegsStore()
        {
            DataTable datRegs = this.extension.Forms[key].DataService.GetRegulatorData();

            Store store = new Store
            {
                AutoLoad = true,
                ID = "dsRegs"
            };

            store.DirectEventConfig.EventMask.ShowMask = false;

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };

            foreach (DataColumn c in datRegs.Columns)
            {
                string colName = c.ColumnName;
                RecordFieldType recType;
                switch (c.DataType.FullName)
                {
                    case "System.String":
                        recType = RecordFieldType.String;
                        break;
                    case "System.Double":
                        recType = RecordFieldType.Float;
                        break;
                    default: recType = RecordFieldType.Auto;
                        break;
                }

                reader.Fields.Add(colName, recType);
            }

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("key", key, ParameterMode.Value));

            HttpProxy httpProxy = new HttpProxy
            {
                Url = "/ReverseData/LoadRegs",
                Method = HttpMethod.POST
            };

            store.Proxy.Add(httpProxy);

            return store;
        }
    }
}
