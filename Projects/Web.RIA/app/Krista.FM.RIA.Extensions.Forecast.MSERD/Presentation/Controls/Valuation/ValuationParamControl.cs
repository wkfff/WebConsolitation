using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class ValuationParamControl : Control
    {
        private readonly string AdjStoreId = "adjStore";
        private readonly string IndStoreId = "indStore";

        private IForecastExtension extension;

        private DataTable adjustersTable;
        private DataTable indicatorsTable;

        public ValuationParamControl(IForecastExtension extension)
        {
            this.extension = extension;
        }

        public int ScenId { get; set; }
        
        public int BaseYear { get; set; }

        public int RefScenId { get; set; }

        public string Key { get; set; }

        public void Initialize(string key)
        {
            var ufc = this.extension.Forms[key];

            DataTable adjTable = new DataTable();

            adjTable.Columns.Add("ID", typeof(int));
            adjTable.Columns.Add("ParamName", typeof(string));
            adjTable.Columns.Add("ValueEst", typeof(decimal));
            adjTable.Columns.Add("ValueEstB", typeof(decimal));
            adjTable.Columns.Add("ValueY1", typeof(decimal));
            adjTable.Columns.Add("ValueY1B", typeof(decimal));
            adjTable.Columns.Add("ValueY2", typeof(decimal));
            adjTable.Columns.Add("ValueY2B", typeof(decimal));
            adjTable.Columns.Add("ValueY3", typeof(decimal));
            adjTable.Columns.Add("ValueY3B", typeof(decimal));
            adjTable.Columns.Add("ValueY4", typeof(decimal));
            adjTable.Columns.Add("ValueY4B", typeof(decimal));
            adjTable.Columns.Add("ValueY5", typeof(decimal));
            adjTable.Columns.Add("ValueY5B", typeof(decimal));
            adjTable.Columns.Add("MaxBound", typeof(decimal));
            adjTable.Columns.Add("MinBound", typeof(decimal));
            adjTable.Columns.Add("Units", typeof(string));
            adjTable.Columns.Add("Code", typeof(string));

            adjustersTable = adjTable;
            
            DataTable indTable = new DataTable();

            indTable.Columns.Add("id", typeof(int));
            indTable.Columns.Add("ParamName", typeof(string));
            indTable.Columns.Add("ValueEst", typeof(decimal));
            indTable.Columns.Add("ValueEstB", typeof(decimal));
            indTable.Columns.Add("ValueY1", typeof(decimal));
            indTable.Columns.Add("ValueY1B", typeof(decimal));
            indTable.Columns.Add("ValueY2", typeof(decimal));
            indTable.Columns.Add("ValueY2B", typeof(decimal));
            indTable.Columns.Add("ValueY3", typeof(decimal));
            indTable.Columns.Add("ValueY3B", typeof(decimal));
            indTable.Columns.Add("ValueY4", typeof(decimal));
            indTable.Columns.Add("ValueY4B", typeof(decimal));
            indTable.Columns.Add("ValueY5", typeof(decimal));
            indTable.Columns.Add("ValueY5B", typeof(decimal));
            indTable.Columns.Add("MaxBound", typeof(decimal));
            indTable.Columns.Add("MinBound", typeof(decimal));
            indTable.Columns.Add("Units", typeof(string));
            indTable.Columns.Add("Code", typeof(string));

            indicatorsTable = indTable;

            ufc.AddObject("AdjsTable", adjTable);
            ufc.AddObject("IndsTable", indTable);

            Key = key;

            LoadData();
        }
        
        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateAdjsStore());
            page.Controls.Add(CreateIndsStore());

            TabPanel tabPanel = new TabPanel
            {
                ID = "valuationTabPanel",
                Region = Region.Center,
                ForceLayout = true,
                AutoRender = true
            };
            
            Panel adjPanel = new Panel
            {
                ID = "adjPanel",
                Title = "Регуляторы",
                Layout = "Accordion",
                ForceLayout = true,
                AutoRender = true
            };
            
            Panel indPanel = new Panel
            {
                ID = "indPanel",
                Title = "Индикаторы",
                Layout = "Accordion",
                ForceLayout = true,
                AutoRender = true
            };

            /*Panel statPanel = new Panel
            {
                ID = "statPanel",
                Title = "Статистические параметры"
            };

            Panel unregPanel = new Panel
            {
                ID = "unregPanel",
                Title = "Нерегулируемые параметры"
            };*/
            
            adjPanel.Items.Add(AdjustersPanel(page));
            indPanel.Items.Add(IndicatorsPanel(page));
            
            /*layoutAdj.Center.Items.Add(adjPanel);
            layoutInd.Center.Items.Add(indPanel);*/
            
            tabPanel.Items.Add(adjPanel);
            tabPanel.Items.Add(indPanel);
            
            indPanel.Listeners.Activate.AddAfter("{0}.reload(); ".FormatWith(IndStoreId));
            adjPanel.Listeners.Activate.AddAfter("{0}.reload(); ".FormatWith(AdjStoreId));
            
            return new List<Component> { tabPanel };
        }

        public Component CreateToolBar()
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

            toolbarPanel.TopBar.Add(toolbar);

            Button btnSaveChange = new Button
            {
                ID = "btnSaveChange",
                Icon = Icon.TableSave,
                ToolTip = "Сохранить изменения"
            };
            
            btnSaveChange.DirectEvents.Click.Url = "/ValuationParam/SaveChange";
            btnSaveChange.DirectEvents.Click.IsUpload = false;
            btnSaveChange.DirectEvents.Click.CleanRequest = true;
            btnSaveChange.DirectEvents.Click.EventMask.ShowMask = true;
            btnSaveChange.DirectEvents.Click.ExtraParams.Add(new Parameter("key", Key, ParameterMode.Value));

            btnSaveChange.DirectEvents.Click.Success = "btnCalc.setDisabled(false); {0}.commitChanges(); ".FormatWith(AdjStoreId);

            toolbar.Items.Add(btnSaveChange);

            Button btnCalc = new Button
            {
                ID = "btnCalc",
                Icon = Icon.Calculator,
                ToolTip = "Расчет варината"
            };

            btnCalc.DirectEvents.Click.Url = "/ValuationParam/Calc";
            btnCalc.DirectEvents.Click.IsUpload = false;
            btnCalc.DirectEvents.Click.CleanRequest = true;
            btnCalc.DirectEvents.Click.EventMask.ShowMask = true;
            btnCalc.DirectEvents.Click.ExtraParams.Add(new Parameter("varid", ScenId.ToString(), ParameterMode.Value));

            toolbar.Items.Add(btnCalc);

            return toolbarPanel;
        }

        private List<Component> AdjustersPanel(ViewPage page)
        {
            var items = new List<Component> { };
            
            string query = "select v.groupid, v.name from dv.v_forecast_scengroups v where v.groupid like '2%'";

            var queryResult = NHibernateSession.Current.CreateSQLQuery(query).List();

            Dictionary<string, string> list = new Dictionary<string, string>();

            foreach (object[] res in queryResult)
            {
                list.Add(Convert.ToString(res[0]), Convert.ToString(res[1]));
            }

            foreach (KeyValuePair<string, string> keyValuePair in list)
            {
                /*Panel panel = new Panel
                {
                    ID = "adjPanel_{0}".FormatWith(keyValuePair.Key),
                    Title = keyValuePair.Value
                };*/

                GridPanel adjGridPanel = new GridPanel
                {
                    ID = "adjGrid_{0}".FormatWith(keyValuePair.Key),
                    StoreID = AdjStoreId,
                    MonitorResize = true,
                    Border = false,
                    AutoScroll = true,
                    ////AutoHeight = true,
                    Height = 280,
                    ////Layout = "fit",
                    ////StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                    ColumnLines = true,
                    Title = keyValuePair.Value,
                    Layout = "Fit",
                    ForceLayout = true,
                    AutoRender = true
                };

                ColumnModel cm = adjGridPanel.ColumnModel;
                cm.AddColumn("ParamName", "ParamName", "Показатель", DataAttributeTypes.dtString, Mandatory.Nullable).SetWidth(150);
                cm.AddColumn("Units", "Units", "Ед. измерения", DataAttributeTypes.dtString, Mandatory.Nullable);

                string renderFunction = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if (value != null)
    {
        return String.format(""<span style='color:{0};'>{1}</span>"", ((value >= record.data.MinBound) && (value <= record.data.MaxBound))? 'green' : 'red', value);
    }
    else
    {
        return '';
    }
}";

                var cb = cm.AddColumn("ValueEst", "ValueEst", "Оценочный год {0} (сценарий)".FormatWith(BaseYear + 1), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(90);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueEstB", "ValueEstB", "Оценочный год {0} (вариант)".FormatWith(BaseYear + 1), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.SetEditableDouble(4);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);
                (cb.Editor[0] as NumberField).DecimalSeparator = ".";

                cb = cm.AddColumn("ValueY1", "ValueY1", "Прогнозный год {0} (сценарий)".FormatWith(BaseYear + 2), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(80);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueY1B", "ValueY1B", "Прогнозный год {0} (вариант)".FormatWith(BaseYear + 2), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.SetEditableDouble(4);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);
                (cb.Editor[0] as NumberField).DecimalSeparator = ".";

                cb = cm.AddColumn("ValueY2", "ValueY2", "Прогнозный год {0} (сценарий)".FormatWith(BaseYear + 3), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(80);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueY2B", "ValueY2B", "Прогнозный год {0} (вариант)".FormatWith(BaseYear + 3), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.SetEditableDouble(4);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);
                (cb.Editor[0] as NumberField).DecimalSeparator = ".";

                cb = cm.AddColumn("ValueY3", "ValueY3", "Прогнозный год {0} (сценарий)".FormatWith(BaseYear + 4), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(80);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueY3B", "ValueY3B", "Прогнозный год {0} (вариант)".FormatWith(BaseYear + 4), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.SetEditableDouble(4);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);
                (cb.Editor[0] as NumberField).DecimalSeparator = ".";

                cb = cm.AddColumn("ValueY4", "ValueY4", "Прогнозный год {0} (сценарий)".FormatWith(BaseYear + 5), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(80);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueY4B", "ValueY4B", "Прогнозный год {0} (вариант)".FormatWith(BaseYear + 5), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.SetEditableDouble(4);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);
                (cb.Editor[0] as NumberField).DecimalSeparator = ".";

                cb = cm.AddColumn("ValueY5", "ValueY5", "Прогнозный год {0} (сценарий)".FormatWith(BaseYear + 6), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(80);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueY5B", "ValueY5B", "Прогнозный год {0} (вариант)".FormatWith(BaseYear + 6), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.SetEditableDouble(4);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);
                (cb.Editor[0] as NumberField).DecimalSeparator = ".";

                cb = cm.AddColumn("MaxBound", "MaxBound", "Максимальная граница", DataAttributeTypes.dtDouble, Mandatory.Nullable);
                (cb as NumberColumn).Format = "0.0000";
                cb = cm.AddColumn("MinBound", "MinBound", "Минимальная граница", DataAttributeTypes.dtDouble, Mandatory.Nullable);
                (cb as NumberColumn).Format = "0.0000";
                /*cm.AddColumn("UserName", "UserName", "Пользователь", DataAttributeTypes.dtString, Mandatory.Nullable);
                cm.AddColumn("Finished", "Finished", "Finished", DataAttributeTypes.dtBoolean, Mandatory.Nullable);
                cm.AddColumn("GroupName", "GroupName", "Группа", DataAttributeTypes.dtString, Mandatory.Nullable);*/

                adjGridPanel.AddColumnsWrapStylesToPage(page);

                adjGridPanel.SelectionModel.Add(new RowSelectionModel());

                adjGridPanel.Listeners.Expand.AddAfter("{0}.reload(); ".FormatWith(AdjStoreId));
                adjGridPanel.Listeners.Collapse.AddBefore("{0}.commitChanges(); ".FormatWith(AdjStoreId));

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
	    url: '/ValuationParam/ChangeAdjData?rowid='+e.record.id+'&col='+this.getColumnModel().getDataIndex(e.column)+'&newVal='+e.value+'&key={0}',
        control: this        
    }});
    btnCalc.setDisabled(true);",
         Key);
                adjGridPanel.Listeners.AfterEdit.AddAfter(editFn);

                items.Add(adjGridPanel);

                ////items.Add(panel);
            }

            return items;
        }

        private List<Component> IndicatorsPanel(ViewPage page)
        {
            var items = new List<Component> { };

            string query = "select v.groupid, v.name from dv.v_forecast_scengroups v where v.groupid like '1%'";

            var queryResult = NHibernateSession.Current.CreateSQLQuery(query).List();

            Dictionary<string, string> list = new Dictionary<string, string>();

            foreach (object[] res in queryResult)
            {
                list.Add(Convert.ToString(res[0]), Convert.ToString(res[1]));
            }

            foreach (KeyValuePair<string, string> keyValuePair in list)
            {
                GridPanel indGridPanel = new GridPanel
                {
                    ID = "indGrid_{0}".FormatWith(keyValuePair.Key),
                    StoreID = IndStoreId,
                    MonitorResize = true,
                    Border = false,
                    AutoScroll = true,
                    ////AutoHeight = true,
                    Height = 280,
                    ////Layout = "fit",
                    ////StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                    ColumnLines = true,
                    Title = keyValuePair.Value,
                    Layout = "Fit",
                    ForceLayout = true,
                    AutoRender = true
                };

                ColumnModel cm = indGridPanel.ColumnModel;
                cm.AddColumn("ParamName", "ParamName", "Показатель", DataAttributeTypes.dtString, Mandatory.Nullable).SetWidth(150);
                cm.AddColumn("Units", "Units", "Ед. измерения", DataAttributeTypes.dtString, Mandatory.Nullable);

                string renderFunction = @"function(value, metadata, record, rowIndex, colIndex, store)
{
    if (value != null)
    {
        return String.format(""<span style='color:{0};'>{1}</span>"", ((value >= record.data.MinBound) && (value <= record.data.MaxBound))? 'green' : 'red', value);
    }
    else
    {
        return '';
    }
}";

                var cb = cm.AddColumn("ValueEst", "ValueEst", "Оценочный год {0} (расчет)".FormatWith(BaseYear + 1), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(90);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueEstB", "ValueEstB", "Оценочный год {0} (вариант)".FormatWith(BaseYear + 1), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);

                cb = cm.AddColumn("ValueY1", "ValueY1", "Прогнозный год {0} (сценарий)".FormatWith(BaseYear + 2), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(90);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueY1B", "ValueY1B", "Прогнозный год {0} (вариант)".FormatWith(BaseYear + 2), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);

                cb = cm.AddColumn("ValueY2", "ValueY2", "Прогнозный год {0} (сценарий)".FormatWith(BaseYear + 3), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(90);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueY2B", "ValueY2B", "Прогнозный год {0} (вариант)".FormatWith(BaseYear + 3), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);

                cb = cm.AddColumn("ValueY3", "ValueY3", "Прогнозный год {0} (сценарий)".FormatWith(BaseYear + 4), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(90);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueY3B", "ValueY3B", "Прогнозный год {0} (вариант)".FormatWith(BaseYear + 4), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);

                cb = cm.AddColumn("ValueY4", "ValueY4", "Прогнозный год {0} (сценарий)".FormatWith(BaseYear + 5), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(90);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueY4B", "ValueY4B", "Прогнозный год {0} (вариант)".FormatWith(BaseYear + 5), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);

                cb = cm.AddColumn("ValueY5", "ValueY5", "Прогнозный год {0} (сценарий)".FormatWith(BaseYear + 6), DataAttributeTypes.dtDouble, Mandatory.Nullable).SetWidth(90);
                (cb as NumberColumn).Format = "0.0000";
                cb.Css = "background-color: #eaeaea;";
                cb = cm.AddColumn("ValueY5B", "ValueY5B", "Прогнозный год {0} (вариант)".FormatWith(BaseYear + 6), DataAttributeTypes.dtUnknown, Mandatory.Nullable);
                cb.Renderer.Fn = renderFunction;
                cb.SetWidth(90);
                
                cb = cm.AddColumn("MaxBound", "MaxBound", "Максимальная граница", DataAttributeTypes.dtDouble, Mandatory.Nullable);
                (cb as NumberColumn).Format = "0.0000";
                cb = cm.AddColumn("MinBound", "MinBound", "Минимальная граница", DataAttributeTypes.dtDouble, Mandatory.Nullable);
                (cb as NumberColumn).Format = "0.0000";
                ////cm.AddColumn("GroupName", "GroupName", "Группа", DataAttributeTypes.dtString, Mandatory.Nullable);

                indGridPanel.AddColumnsWrapStylesToPage(page);

                indGridPanel.SelectionModel.Add(new RowSelectionModel());

                indGridPanel.Listeners.Expand.AddAfter("{0}.reload(); ".FormatWith(IndStoreId));

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
	    url: '/ValuationParam/ChangeIndData?rowid='+e.record.id+'&col='+this.getColumnModel().getDataIndex(e.column)+'&newVal='+e.value+'&key={0}',
        control: this        
    }});",
         Key);
                indGridPanel.Listeners.AfterEdit.AddAfter(editFn);

                items.Add(indGridPanel);

                ////items.Add(panel);
            }

            return items;
        }

        private void LoadData()
        {
            /*var resourceManager = ResourceManager.GetInstance(page);

            string script = @"function refreshDataTable()
{{
    Ext.net.DirectEvent.confirmRequest(
    {{
        cleanRequest: true,
        isUpload: false,
        url: '/ValuationParam/IndsLoadTable?scenId={0}&key={1}',
        control: this
                   ////userSuccess: function(response, result, el, type, action, extraParams){  }
    }});
}}".FormatWith(ScenId, Key);
            
            resourceManager.RegisterClientScriptBlock("refreshDataTable", script);
            resourceManager.AddScript("refreshDataTable();");*/

            string selectSQL;
            
            ////string filter = " p.code like '{0}%' ".FormatWith(activeTab.Split('_').Last());)

            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation, p.code
from dv.v_forecast_val_indicators a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substr(a.groupcode, 1, 3) = grp.groupid
where a.refscenario = {0} and a.basescenario = {1}".FormatWith(ScenId, RefScenId);
            }
            else
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation, p.code
from dv.v_forecast_val_indicators a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substring(CONVERT(VARCHAR(6),a.groupcode), 1, 3) = grp.groupid
where a.refscenario = {0} and a.basescenario = {1}".FormatWith(ScenId, RefScenId);
            }

            var queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

            foreach (object[] row in queryResult)
            {
                var datarow = indicatorsTable.NewRow();

                datarow["ID"] = Convert.ToInt32(row[0]);
                datarow["ParamName"] = Convert.ToString(row[1]);
                
                if (row[2] != null)
                {
                    datarow["ValueEst"] = Convert.ToDecimal(row[2]);
                }

                if (row[3] != null)
                {
                    datarow["ValueEstB"] = Convert.ToDecimal(row[3]);
                }

                if (row[4] != null)
                {
                    datarow["ValueY1"] = Convert.ToDecimal(row[4]);
                }

                if (row[5] != null)
                {
                    datarow["ValueY1B"] = Convert.ToDecimal(row[5]);
                }

                if (row[6] != null)
                {
                    datarow["ValueY2"] = Convert.ToDecimal(row[6]);
                }

                if (row[7] != null)
                {
                    datarow["ValueY2B"] = Convert.ToDecimal(row[7]);
                }

                if (row[8] != null)
                {
                    datarow["ValueY3"] = Convert.ToDecimal(row[8]);
                }

                if (row[9] != null)
                {
                    datarow["ValueY3B"] = Convert.ToDecimal(row[9]);
                }

                if (row[10] != null)
                {
                    datarow["ValueY4"] = Convert.ToDecimal(row[10]);
                }

                if (row[11] != null)
                {
                    datarow["ValueY4B"] = Convert.ToDecimal(row[11]);
                }

                if (row[12] != null)
                {
                    datarow["ValueY5"] = Convert.ToDecimal(row[12]);
                }

                if (row[13] != null)
                {
                    datarow["ValueY5B"] = Convert.ToDecimal(row[13]);
                }
                
                datarow["MinBound"] = Convert.ToDecimal(row[14]);
                datarow["MaxBound"] = Convert.ToDecimal(row[15]);
                datarow["Units"] = Convert.ToString(row[16]);
                datarow["Code"] = Convert.ToString(row[17]);

                indicatorsTable.Rows.Add(datarow);
            }
            
            if (NHibernateSession.Current.Connection.GetType().Name == "OracleConnection")
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation, p.code
from dv.v_forecast_val_adjusters a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substr(a.groupcode, 1, 3) = grp.groupid
where a.refscenario = {0} and a.basescenario = {1}".FormatWith(ScenId, RefScenId);
            }
            else
            {
                selectSQL = @"select a.id, p.name, a.valueestimate, a.v_est_b, a.valuey1, a.v_y1_b, a.valuey2, a.v_y2_b, a.valuey3, a.v_y3_b, a.valuey4, a.v_y4_b, a.valuey5, a.v_y5_b, a.minbound, a.maxbound, a.designation, p.code
from dv.v_forecast_val_adjusters a
left join d_forecast_parametrs p on p.id = a.refparams
left join v_forecast_scengroups grp on substring(CONVERT(VARCHAR(6),a.groupcode), 1, 3) = grp.groupid
where a.refscenario = {0} and a.basescenario = {1}".FormatWith(ScenId, RefScenId);
            }

            queryResult = NHibernateSession.Current.CreateSQLQuery(selectSQL).List();

            foreach (object[] row in queryResult)
            {
                var datarow = adjustersTable.NewRow();

                datarow["ID"] = Convert.ToInt32(row[0]);
                datarow["ParamName"] = Convert.ToString(row[1]);

                if (row[2] != null)
                {
                    datarow["ValueEst"] = Convert.ToDecimal(row[2]);
                }

                if (row[3] != null)
                {
                    datarow["ValueEstB"] = Convert.ToDecimal(row[3]);
                }

                if (row[4] != null)
                {
                    datarow["ValueY1"] = Convert.ToDecimal(row[4]);
                }

                if (row[5] != null)
                {
                    datarow["ValueY1B"] = Convert.ToDecimal(row[5]);
                }

                if (row[6] != null)
                {
                    datarow["ValueY2"] = Convert.ToDecimal(row[6]);
                }

                if (row[7] != null)
                {
                    datarow["ValueY2B"] = Convert.ToDecimal(row[7]);
                }

                if (row[8] != null)
                {
                    datarow["ValueY3"] = Convert.ToDecimal(row[8]);
                }

                if (row[9] != null)
                {
                    datarow["ValueY3B"] = Convert.ToDecimal(row[9]);
                }

                if (row[10] != null)
                {
                    datarow["ValueY4"] = Convert.ToDecimal(row[10]);
                }

                if (row[11] != null)
                {
                    datarow["ValueY4B"] = Convert.ToDecimal(row[11]);
                }

                if (row[12] != null)
                {
                    datarow["ValueY5"] = Convert.ToDecimal(row[12]);
                }

                if (row[13] != null)
                {
                    datarow["ValueY5B"] = Convert.ToDecimal(row[13]);
                }

                datarow["MinBound"] = Convert.ToDecimal(row[14]);
                datarow["MaxBound"] = Convert.ToDecimal(row[15]);
                datarow["Units"] = Convert.ToString(row[16]);
                datarow["Code"] = Convert.ToString(row[17]);

                adjustersTable.Rows.Add(datarow);
            }

            indicatorsTable.AcceptChanges();
            adjustersTable.AcceptChanges();
        }

        private Store CreateAdjsStore()
        {
            Store store = new Store { ID = AdjStoreId, /*GroupField = "GroupName",*/ AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("ParamName");
            reader.Fields.Add("ValueEst", RecordFieldType.Auto);
            reader.Fields.Add("ValueEstB", RecordFieldType.Auto);
            reader.Fields.Add("ValueY1", RecordFieldType.Auto);
            reader.Fields.Add("ValueY1B", RecordFieldType.Auto);
            reader.Fields.Add("ValueY2", RecordFieldType.Auto);
            reader.Fields.Add("ValueY2B", RecordFieldType.Auto);
            reader.Fields.Add("ValueY3", RecordFieldType.Auto);
            reader.Fields.Add("ValueY3B", RecordFieldType.Auto);
            reader.Fields.Add("ValueY4", RecordFieldType.Auto);
            reader.Fields.Add("ValueY4B", RecordFieldType.Auto);
            reader.Fields.Add("ValueY5", RecordFieldType.Auto);
            reader.Fields.Add("ValueY5B", RecordFieldType.Auto);
            reader.Fields.Add("MaxBound");
            reader.Fields.Add("MinBound");
            reader.Fields.Add("Units");
            reader.Fields.Add("Code");

            store.Reader.Add(reader);

            ////store.BaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("key", Key.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("activeTab", "adjPanel.layout.activeItem.getId()", ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/ValuationParam/AdjsLoadDT",
                Method = HttpMethod.POST
            });

            /*store.WriteBaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("savedData", String.Format("{0}.getChangedData()", AdjStoreId), ParameterMode.Raw));

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/ScenarioParam/AdjsSave",
                Method = HttpMethod.POST,
                Timeout = 50000
            });*/

            return store;
        }

        private Store CreateIndsStore()
        {
            Store store = new Store { ID = IndStoreId, /*GroupField = "GroupName",*/ AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("ParamName");
            reader.Fields.Add("ValueEst", RecordFieldType.Auto);
            reader.Fields.Add("ValueEstB", RecordFieldType.Auto);
            reader.Fields.Add("ValueY1", RecordFieldType.Auto);
            reader.Fields.Add("ValueY1B", RecordFieldType.Auto);
            reader.Fields.Add("ValueY2", RecordFieldType.Auto);
            reader.Fields.Add("ValueY2B", RecordFieldType.Auto);
            reader.Fields.Add("ValueY3", RecordFieldType.Auto);
            reader.Fields.Add("ValueY3B", RecordFieldType.Auto);
            reader.Fields.Add("ValueY4", RecordFieldType.Auto);
            reader.Fields.Add("ValueY4B", RecordFieldType.Auto);
            reader.Fields.Add("ValueY5", RecordFieldType.Auto);
            reader.Fields.Add("ValueY5B", RecordFieldType.Auto);
            reader.Fields.Add("MaxBound", RecordFieldType.Auto);
            reader.Fields.Add("MinBound", RecordFieldType.Auto);
            reader.Fields.Add("Units");

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("key", Key.ToString(), ParameterMode.Value));
            store.BaseParams.Add(new Parameter("activeTab", "indPanel.layout.activeItem.getId()", ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/ValuationParam/IndsLoadDT",
                Method = HttpMethod.POST
            });

            /*store.WriteBaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("savedData", String.Format("{0}.getChangedData()", AdjStoreId), ParameterMode.Raw));

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/ScenarioParam/AdjsSave",
                Method = HttpMethod.POST,
                Timeout = 50000
            });*/

            return store;
        }
    }
}
