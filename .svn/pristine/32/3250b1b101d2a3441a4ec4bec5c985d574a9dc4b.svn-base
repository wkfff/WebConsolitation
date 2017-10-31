using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningChartControl : Control
    {
        private const string StoreChartId = "stChartData";

        private IForecastExtension extension;
        private string key;
        
        public PlanningChartControl(IForecastExtension extension, string key)
        {
            this.extension = extension;
            this.key = key;
        }

        public Store StoreChart { get; private set; }

        public static void ChartRange(DataRow rowStatic, DataRow rowProg, out double? min, out double? max, out double? delta)
        {
            min = null;
            max = null;
            delta = null;

            foreach (DataColumn dataColumn in rowProg.Table.Columns)
            {
                var columnName = dataColumn.ColumnName;
                if (columnName.Contains("year_"))
                {
                    double value;

                    if (rowProg[columnName] != DBNull.Value)
                    {
                        value = Convert.ToDouble(rowProg[columnName]);

                        if (!min.HasValue)
                        {
                            min = value;
                        }

                        if (!max.HasValue)
                        {
                            max = value;
                        }

                        if (value < min.Value)
                        {
                            min = value;
                        }

                        if (value > max.Value)
                        {
                            max = value;
                        }
                    }
                }
            }

            if (!min.HasValue || ((min == 0) && (max == 0)))
            {
                foreach (DataColumn dataColumn in rowStatic.Table.Columns)
                {
                    var columnName = dataColumn.ColumnName;
                    if (columnName.Contains("year_"))
                    {
                        double value;

                        if (rowStatic[columnName] != DBNull.Value)
                        {
                            value = Convert.ToDouble(rowStatic[columnName]);

                            if (!min.HasValue)
                            {
                                min = value;
                            }

                            if (!max.HasValue)
                            {
                                max = value;
                            }

                            if (value < min.Value)
                            {
                                min = value;
                            }

                            if (value > max.Value)
                            {
                                max = value;
                            }
                        }
                    }
                }
            }

            if (min.HasValue && max.HasValue)
            {
                delta = (max - min) / 10;

                min -= delta / 2;
                max += delta / 2;

                int dig;
                if (delta > 1)
                {
                    var tmp = Convert.ToInt32(Math.Round(delta.Value)).ToString();
                    dig = tmp.Length;
                    if (dig > 4)
                    {
                        min = Math.Round(min.Value);
                        max = Math.Round(max.Value);
                        delta = Math.Round(delta.Value);
                    }
                    else
                    {
                        min = Math.Round(min.Value, 5 - dig);
                        max = Math.Round(max.Value, 5 - dig);
                        delta = Math.Round(delta.Value, 5 - dig);
                    }
                }
                else
                {
                    dig = 5;
                    min = Math.Round(min.Value, dig);
                    max = Math.Round(max.Value, dig);
                    delta = Math.Round(delta.Value, dig);
                }
            }
        }
        
        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            string scriptRange = String.Empty;

            StoreChart = CreateChartStore(StoreChartId);
            page.Controls.Add(StoreChart);

            var ufc = this.extension.Forms[key];
            DataTable datProg = ufc.DataService.GetProgData(); ////.GetObject("dtProg") as DataTable;
            DataTable datStatic = ufc.DataService.GetStaticData();

            DataRow rowStatic = (from row in datStatic.AsEnumerable()
                                 where Convert.ToInt32(row["id"]) == ufc.ParamId
                                 select row).First();
            DataRow rowProg = (from row in datProg.AsEnumerable()
                               where Convert.ToInt32(row["id"]) == ufc.ParamId
                               select row).First();

            double? min, max, delta;

            ChartRange(rowStatic, rowProg, out min, out max, out delta);

            string paramName = Convert.ToString(rowStatic["Param"]);

            if (delta.HasValue)
            {
                scriptRange = String.Format(
                        "minimum: {0}, maximum: {1}, majorUnit: {2},",
                        min.Value.ToString().Replace(",", "."),
                        max.Value.ToString().Replace(",", "."),
                        delta.ToString().Replace(",", "."));
            }

            Panel chartPanel = new Panel
            {
                ID = "chartPanel",
                Layout = "fit",
                Height = 350,
                ////utoHeight = true,
                ////ColumnWidth = 0.7
            };

            string script = String.Format(
@" function panelLoaded(){{
    chartPanel.add(new Ext.chart.LineChart({{
        xtype: 'linechart',    
        id : 'chart1',
        title: 'Test',
        url: '/Content/charts.swf',        
        store: stChartData,        
        xField: 'Year',
        layout: 'fit',
        xAxis: new Ext.chart.CategoryAxis({{
                    title: 'Год',
                    orientation: 'horizontal'                
            }}),    
        yAxis: new Ext.chart.NumericAxis({{
                    title: '{0}',
                    {1}
                    orientation: 'vertical'
            }}),    
        series: [{{
                type: 'line',                    
                yField: 'Xp',
                displayName: 'Прогноз',                                
                showInLegend: true,
                style: {{ 
                    color:0xFF8888,                    
                }}
            }},
            {{
                type: 'line',                
                yField: 'Xs',   
                displayName: 'Статистика',                
                showInLegend: true,
                style: {{                    
                    color:0x99BBE8,                    
                }}
            }}
            ],
        extraStyle: {{
                yAxis:  {{
                            titleRotation: -90
                        }},
                legend: {{   
                            display: 'bottom'   
                        }}
            }}
    }}));
 
    chartPanel.doLayout();
}}",
   paramName,
   scriptRange);

            ////Ext.chart.
            resourceManager.RegisterClientScriptBlock("panelLoaded", script);
            /*chartContainer.AddScript(script);*/

            chartPanel.Listeners.AfterRender.AddAfter("panelLoaded();stChartData.load();");
            
            return new List<Component> { chartPanel };
        }

        public Store CreateChartStore(string storeId)
        {
            Store store = new Store { ID = storeId, AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data" };
            store.Reader.Add(reader);
            reader.Fields.Add("Year");
            reader.Fields.Add("Xp");
            reader.Fields.Add("Xs");

            ////store.BaseParams.Add(new Parameter("pyears", "parent.sfProgToYear.getValue() - parent.sfProgFromYear.getValue()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", key), ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningChart/LoadProgChart",  ////progchartdata
                Method = HttpMethod.POST
            });

            return store;
        }
    }
}
