using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;
using GridView = Ext.Net.GridView;
using Icon = Ext.Net.Icon;
using Label = Ext.Net.Label;
using Parameter = Ext.Net.Parameter;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningStatGridControl : Control
    {
        private const string GridId = "grStaticData";
        private const string StoreId = "stStaticData";
        private readonly string Key;

        ////private DataTable datStatic = new DataTable();

        private IForecastExtension extension;

        public PlanningStatGridControl(IForecastExtension extension, string key)
        {
            this.extension = extension;
            Key = key;
        }

        public Store Store { get; private set; }

        public static Component AddRowWindow(ViewPage page, string key)
        {
            Window wndAddRow = new Window
            {
                ID = "wndAddRow",
                Hidden = true,
                Width = 440,
                Height = 300,
                Layout = "FitLayout",
                Modal = true,
                Title = "Добавление статистического параметра"
            };

            FormPanel formPanel = new FormPanel
            {
                ButtonAlign = Alignment.Right,
                ////Border = false,
                MonitorPoll = 500,
                MonitorValid = true,
                Padding = 5,
                LabelPad = -5,
                Width = 440,
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };

            wndAddRow.Items.Add(formPanel);

            Panel paramPanel = new Panel
            {
                Height = 200,
                Width = 420,
                Border = false,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
            };
            
            formPanel.Items.Add(paramPanel);
            
            PlanningVariantsParamsGridControl paramsGridControl = new PlanningVariantsParamsGridControl(PlanVarSource.AddRow, key);

            paramPanel.Items.Add(paramsGridControl.Build(page));
            
            Button btnInsertCancel = new Button
            {
                ID = "btnInsertCancel",
                Text = "Отмена"
            };

            btnInsertCancel.Listeners.Click.Handler = "wndExportForm2p.hide();";

            Button btnInsertOk = new Button
            {
                ID = "btnInsertOk",
                Text = "Вставить"
            };

            btnInsertOk.Listeners.Click.AddAfter(String.Format(
@"if (panelParent.layout.activeItem.getId() == ""panelParams"")
{{
    Ext.net.DirectEvent.confirmRequest(
    {{
	        cleanRequest: true,
	        isUpload: false,
	        url: '/PlanningStat/AddParamRow?paramId='+gpVariantParams.getSelectionModel().getSelected().id+'&key={0}',
            control: this,
            userSuccess: function(response, result, el, type, action, extraParams){{ panelStat.getBody().stStaticData.reload(); }}
    }})
}}
else
{{
    Ext.net.DirectEvent.confirmRequest(
    {{
	        cleanRequest: true,
	        isUpload: false,
	        url: '/PlanningStat/AddRegRow?regId='+gpVariantRegs.getSelectionModel().getSelected().id+'&key={0}',
            control: this,
            userSuccess: function(response, result, el, type, action, extraParams){{ stRegData.reload(); }}
    }})
}}",
    key));
            formPanel.Buttons.Add(new List<ButtonBase> { btnInsertCancel, btnInsertOk });

            return wndAddRow;
        }
        
        public override List<Component> Build(ViewPage page)
        {
            /////DataTable datStatic;//// = new DataTable();

            DataTable datStatic = this.extension.Forms[Key].DataService.GetStaticData(); ////.GetObject("dtStatic") as DataTable;
            SortedList<int, bool> arrYear = this.extension.Forms[Key].DataService.GetArrYears();

            Store = CreateStore(StoreId);
            page.Controls.Add(Store);
            
            var rowCount = datStatic.Rows.Count > 5 ? 5 : datStatic.Rows.Count;
            
            GridPanel topGridPanel = new GridPanel
            {
                ID = GridId,
                StoreID = StoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                ////AutoHeight = true,
                Height = 50 + (rowCount * 45) + 15,  //// 24 .... 34
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true
            };

            ////topGridContainer.Items.Add(topGridPanel);
            
            ColumnModel cm = topGridPanel.ColumnModel;
           
            CommandColumn cmdColumn = new CommandColumn { Width = 25, Hideable = false };
            var command = new GridCommand { CommandName = "delete", Icon = Icon.ApplicationFormDelete };
            command.ToolTip.Text = "Удалить показатель";
            cmdColumn.Commands.Add(command);
            cm.Columns.Add(cmdColumn);

            string s = String.Format(
@"Ext.net.DirectEvent.confirmRequest(
{{
	    cleanRequest: true,
	    isUpload: false,
	    url: '/PlanningStat/DeleteRow?paramId='+record.id+'&key={0}',
        control: this,
        userSuccess: function(response, result, el, type, action, extraParams){{ stStaticData.reload(); }}
}});",
    Key);

            topGridPanel.Listeners.Command.AddAfter(s);

            cm.AddColumn("Param", "Param", "Показатель", DataAttributeTypes.dtInteger, Mandatory.Nullable).SetWidth(150);

            List<string> list = new List<string>();
            foreach (DataColumn col in datStatic.Columns)
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
                col.SetEditableDouble(6);
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
	    url: '/PlanningStat/ChangeStatData?rowid='+e.record.id+'&col='+this.getColumnModel().getDataIndex(e.column)+'&newVal='+e.value+'&key={0}',
        control: this,
        userSuccess: function(response, result, el, type, action, extraParams){{ if (parent.chart1.isVisible()) {{ parent.stChartData.reload(); }} }}
    }});", 
         Key);
            topGridPanel.Listeners.AfterEdit.AddAfter(editFn);
           
            topGridPanel.AddColumnsWrapStylesToPage(page);
            topGridPanel.SelectionModel.Add(new RowSelectionModel());
            
            GridView gv = new GridView
            {
                ID = "gridView",
                StandardHeaderRow = true,
            };

            topGridPanel.View.Add(gv);
            HeaderRow hr = new HeaderRow { Cls = "x-small-editor" };
            topGridPanel.View[0].HeaderRows.Add(hr);
            
            ////topGridContainer.Items.Add(topGridPanel);
            
            HeaderColumn headerColumn;

            headerColumn = new HeaderColumn { Cls = "x-small-editor" };

            Button button = new Button
            {
                ID = "btnAddRow",
                Icon = Icon.TableRowInsert,
                ToolTip = "Добавить исходный показатель"
            };

            button.Listeners.Click.AddAfter(String.Format(
@"Ext.net.DirectEvent.confirmRequest(
{{
	    cleanRequest: true,
	    isUpload: false,
	    url: '/PlanningStat/ShowAddRowWnd?key={0}',
        control: this,
        userSuccess: function(response, result, el, type, action, extraParams){{ stStaticData.reload(); }}
}});",
    Key));

            ////parent.wndAddRow.show();

            headerColumn.Component.Add(button);
            hr.Columns.Add(headerColumn);

            headerColumn = new HeaderColumn { Cls = "x-small-editor" };
            headerColumn.Component.Add(new Label { Text = "Использовать в прогнозе" });
            hr.Columns.Add(headerColumn);
            
            foreach (string colName in list)
            {
                /*if (colName == "Param")
                {
                    headerColumn = new HeaderColumn { Cls = "x-small-editor" };
                    headerColumn.Component.Add(new Label { Text = "Использовать в прогнозе" });
                    hr.Columns.Add(headerColumn);
                }*/

                headerColumn = new HeaderColumn { Cls = "x-small-editor" };

                int year = Convert.ToInt32(colName.Replace("year_", String.Empty));

                string cmbName = String.Format("cbUseYear_{0}", year);
                Checkbox cb = new Checkbox
                {
                    ID = cmbName,
                    Checked = arrYear[year]
                };

                cb.DirectEvents.Check.Url = "/PlanningStat/CheckYear";

                cb.DirectEvents.Check.CleanRequest = true;
                cb.DirectEvents.Check.EventMask.ShowMask = true;

                cb.DirectEvents.Check.ExtraParams.Add(new Parameter("year", String.Format("'{0}'", year), ParameterMode.Raw));
                cb.DirectEvents.Check.ExtraParams.Add(new Parameter("status", String.Format("{0}.checked", cmbName), ParameterMode.Raw));
                cb.DirectEvents.Check.ExtraParams.Add(new Parameter("key", String.Format("'{0}'", Key), ParameterMode.Raw));

                headerColumn.Component.Add(cb);
                hr.Columns.Add(headerColumn);
            }

////            var s1 = gv.GetGeneratedScripts();
            
            ////gv.HeaderRows.Add(hr);

            topGridPanel.Listeners.AfterRender.AddAfter("stStaticData.load();");
            
            return new List<Component> { topGridPanel };
        }
        
        public Store CreateStore(string storeId)
        {
            Store store = new Store { ID = storeId, AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data" };

            DataTable datStatic = this.extension.Forms[Key].DataService.GetStaticData(); 

            foreach (DataColumn c in datStatic.Columns)
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

            /*reader.Fields.Add("Param", RecordFieldType.String);
            reader.Fields.Add("year_2000", RecordFieldType.Float);
            reader.Fields.Add("year_2001", RecordFieldType.Float);
            reader.Fields.Add("year_2002", RecordFieldType.Float);
            reader.Fields.Add("year_2003", RecordFieldType.Float);
            reader.Fields.Add("year_2004", RecordFieldType.Float);*/

            store.Reader.Add(reader);
            
            ////store.RemoveField(store.Reader[0].Fields.Get("year_2005"));

            ////var s = store.GetGeneratedScripts();

            store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", Key), ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningStat/StaticLoad",
                Method = HttpMethod.POST
            });
            
            return store;
        }
    }
}
