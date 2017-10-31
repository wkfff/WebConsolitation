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

namespace Krista.FM.RIA.Extensions.Forecast.MSERD
{
    public class ScenarioParamControl : Control
    {
        private const string AdjStoreId = "dsAdjusters";
        private const string IndStoreId = "dsIndicators";
        private const string UnregStoreId = "dsUnreg";
        private const string StatStoreId = "dsStatic";

        private const string AdjGridId = "grAdjusters";
        private const string IndGridId = "grIndicators";
        private const string UnregGridId = "grUnreg";
        private const string StatGridId = "grStatic";

        public int ScenId { get; set; }

        public int BaseYear { get; set; }

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateAdjsStore());
            page.Controls.Add(CreateIndsStore());
            page.Controls.Add(CreateStaticStore());
            page.Controls.Add(CreateUnregStore());
            
            Panel panelAccord = new Panel
            {
                Layout = "Accordion",
                Height = 320,
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc"
                ////AutoWidth = true,
            };

            ////panelMain.Items.Add(panelAccord);

            /*Panel panelAdjs = ;
            Panel panelStat = ;
            Panel panelUnreg = ;*/

            panelAccord.Items.Add(GetPanelAdjs(page));
            panelAccord.Items.Add(GetPanelStat(page));
            panelAccord.Items.Add(GetPanelUnreg(page));

            panelAccord.AddScript("{0}.reload();".FormatWith(AdjStoreId));

            Panel indPanel = CreateIndicators(page);

            indPanel.AddScript("{0}.reload();".FormatWith(IndStoreId));
            
            return new List<Component> { panelAccord, indPanel };
        }

        public Panel CreateIndicators(ViewPage page)
        {
            Panel panelInds = GetPanelInds(page);
            return panelInds;
        }
        
        public Component CreateToolBar()
        {
            FormPanel toolbarPanel = new FormPanel
            {
                Border = false,
                ////AutoHeight = true,
                Height = 30,
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
                Icon = Icon.DatabaseSave,
                ToolTip = "Записать изменения"
            };

            btnSave.Listeners.Click.AddAfter("{0}.save(); {1}.save(); {2}.save();".FormatWith(AdjStoreId, UnregStoreId, StatStoreId));

            toolbar.Items.Add(btnSave);

            toolbarPanel.TopBar.Add(toolbar);
            
            return toolbarPanel;
        }

        public List<Component> InfoPanel()
        {
            FormPanel infoPanel = new FormPanel
            {
                Border = false,
                ////AutoHeight = true,
                Height = 30,
                ////Width = 400,
                Collapsible = false,
                Layout = "form",
                Title = "Параметры",
                BodyCssClass = "x-window-mc",
                CssClass = "x-window-mc",
                Padding = 10
            };

            return new List<Component> { infoPanel };
        }

        private Component GetPanelAdjs(ViewPage page)
        {
            /*Panel panelAdjs = new Panel
            {
                ID = "panelAdjs",
                Height = 320,
                Title = "Регуляторы"
            };*/

            GridPanel adjGridPanel = new GridPanel
            {
                ID = AdjGridId,
                StoreID = AdjStoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                ////AutoHeight = true,
                Height = 280,
                ////Layout = "fit",
                ////StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                Title = "Регуляторы"
            };

            GroupingView groupingView = new GroupingView
            {
                ID = "GroupingViewReg",
                HideGroupedColumn = true,
                EnableNoGroups = true,
                GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Записей' : 'Запись']})",
                StartCollapsed = true
            };

            adjGridPanel.View.Add(groupingView);
            
            adjGridPanel.Listeners.Expand.AddAfter("if ({0}.getCount() == 0) {{ {0}.reload(); }}".FormatWith(AdjStoreId));
            
            ColumnModel cm = adjGridPanel.ColumnModel;
            cm.AddColumn("ParamName", "ParamName", "Показатель", DataAttributeTypes.dtString, Mandatory.Nullable).SetWidth(150);
            cm.AddColumn("Units", "Units", "Ед. измерения", DataAttributeTypes.dtString, Mandatory.Nullable);
            
            ColumnBase cb = cm.AddColumn("ValueEstimate", "ValueEstimate", "Оценочный год<br/>{0} год".FormatWith(BaseYear + 1), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("ValueY1", "ValueY1", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 2), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("ValueY2", "ValueY2", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 3), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("ValueY3", "ValueY3", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 4), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("ValueY4", "ValueY4", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 5), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("ValueY5", "ValueY5", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 6), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("MaxBound", "MaxBound", "Верхняя граница", DataAttributeTypes.dtDouble, Mandatory.Nullable);
            (cb as NumberColumn).Format = "0.0000";
            cb = cm.AddColumn("MinBound", "MinBound", "Нижняя граница", DataAttributeTypes.dtDouble, Mandatory.Nullable);
            (cb as NumberColumn).Format = "0.0000";
            cm.AddColumn("UserName", "UserName", "Пользователь", DataAttributeTypes.dtString, Mandatory.Nullable);

            cb = cm.AddColumn("Finished", "Finished", "Заполнен", DataAttributeTypes.dtBoolean, Mandatory.Nullable);
            cb.SetEditableBoolean();

            cm.AddColumn("GroupName", "GroupName", "Группа", DataAttributeTypes.dtString, Mandatory.Nullable);

            adjGridPanel.AddColumnsWrapStylesToPage(page);

            adjGridPanel.SelectionModel.Add(new RowSelectionModel());

            ////panelAdjs.Items.Add(adjGridPanel);
            return adjGridPanel;
        }
        
        private Component GetPanelStat(ViewPage page)
        {
            /*Panel panelStat = new Panel
            {
                ID = "panelStatic",
                Height = 320,
                Title = "Статистические параметры"
            };*/

            GridPanel statGridPanel = new GridPanel
            {
                ID = StatGridId,
                StoreID = StatStoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                ////AutoHeight = true,
                Height = 280,
                ////Layout = "fit",
                ////StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                Title = "Статистические параметры"
            };
            
            statGridPanel.Listeners.Expand.AddAfter("if ({0}.getCount() == 0) {{ {0}.reload(); }}".FormatWith(StatStoreId));

            GroupingView groupingView = new GroupingView
            {
                ID = "GroupingViewStat",
                HideGroupedColumn = true,
                EnableNoGroups = true,
                GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Items' : 'Item']})",
                StartCollapsed = true
            };

            statGridPanel.View.Add(groupingView);
            
            ColumnModel cm = statGridPanel.ColumnModel;
            cm.AddColumn("ParamName", "ParamName", "Показатель", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(150);
            cm.AddColumn("Units", "Units", "Ед. измерения", DataAttributeTypes.dtString, Mandatory.Nullable);
            
            var cb = cm.AddColumn("ValueBase", "ValueBase", "Базовый год<br/>{0} год".FormatWith(BaseYear), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";
            
            cb = cm.AddColumn("ValueEstimate", "ValueEstimate", "Оценочный год<br/>{0} год".FormatWith(BaseYear + 1), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";
            
            cm.AddColumn("UserName", "UserName", "Пользователь", DataAttributeTypes.dtString, Mandatory.Nullable);
            
            cb = cm.AddColumn("Finished", "Finished", "Заполнен", DataAttributeTypes.dtBoolean, Mandatory.Nullable);
            cb.SetEditableBoolean();
            
            cm.AddColumn("GroupName", "GroupName", "Группа", DataAttributeTypes.dtString, Mandatory.Nullable);

            statGridPanel.AddColumnsWrapStylesToPage(page);

            statGridPanel.SelectionModel.Add(new RowSelectionModel());

            ////panelStat.Items.Add(statGridPanel);
            return statGridPanel;
        }

        private Component GetPanelUnreg(ViewPage page)
        {
            /*Panel panelUnreg = new Panel
            {
                ID = "panelUnreg",
                Height = 320,
                Title = "Нерегулируемые параметры"
            };*/

            GridPanel unregGridPanel = new GridPanel
            {
                ID = UnregGridId,
                StoreID = UnregStoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                ////AutoHeight = true,
                Height = 280,
                ////Layout = "fit",
                ////StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                Title = "Нерегулируемые параметры"
            };

            GroupingView groupingView = new GroupingView
            {
                ID = "GroupingViewUnreg",
                HideGroupedColumn = true,
                EnableNoGroups = true,
                GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Записей' : 'Запись']})",
                StartCollapsed = true
            };

            unregGridPanel.View.Add(groupingView);

            unregGridPanel.Listeners.Expand.AddAfter("if ({0}.getCount() == 0) {{ {0}.reload(); }}".FormatWith(UnregStoreId));

            ColumnModel cm = unregGridPanel.ColumnModel;
            cm.AddColumn("ParamName", "ParamName", "Показатель", DataAttributeTypes.dtString, Mandatory.Nullable).SetWidth(150);
            cm.AddColumn("Units", "Units", "Ед. измерения", DataAttributeTypes.dtString, Mandatory.Nullable);
            
            var cb = cm.AddColumn("ValueEstimate", "ValueEstimate", "Оценочный год<br/>{0} год".FormatWith(BaseYear + 1), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("ValueY1", "ValueY1", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 2), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("ValueY2", "ValueY2", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 3), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("ValueY3", "ValueY3", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 4), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("ValueY4", "ValueY4", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 5), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cb = cm.AddColumn("ValueY5", "ValueY5", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 6), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            cb.SetEditableDouble(4);
            (cb as NumberColumn).Format = "0.0000";
            (cb.Editor[0] as NumberField).DecimalSeparator = ".";

            cm.AddColumn("UserName", "UserName", "Пользователь", DataAttributeTypes.dtString, Mandatory.Nullable);
            
            cb = cm.AddColumn("Finished", "Finished", "Заполнен", DataAttributeTypes.dtBoolean, Mandatory.Nullable);
            cb.SetEditableBoolean();

            cm.AddColumn("GroupName", "GroupName", "Группа", DataAttributeTypes.dtString, Mandatory.Nullable);
            
            unregGridPanel.AddColumnsWrapStylesToPage(page);

            unregGridPanel.SelectionModel.Add(new RowSelectionModel());

            /////panelUnreg.Items.Add(unregGridPanel);
            return unregGridPanel;
        }

        private Panel GetPanelInds(ViewPage page)
        {
            Panel panelInds = new Panel
            {
                ID = "panelInds",
                Collapsible = true,
                Height = 300,
                ////Width = 1050,
                ////AutoHeight = true,
                AutoWidth = true,
                Title = "Индикаторы"
            };

            GridPanel indGridPanel = new GridPanel
            {
                ID = IndGridId,
                StoreID = IndStoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                ////AutoHeight = true,
                Height = 270,
                ////Width = 1000,
                Layout = "fit",
                ////StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true
            };

            GroupingView groupingView = new GroupingView
            {
                ID = "GroupingViewInd",
                HideGroupedColumn = true,
                EnableNoGroups = true,
                GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Записей' : 'Запись']})",
                StartCollapsed = true
            };

            indGridPanel.View.Add(groupingView);

            ColumnModel cm = indGridPanel.ColumnModel;
            cm.AddColumn("ParamName", "ParamName", "Показатель", DataAttributeTypes.dtString, Mandatory.Nullable).SetWidth(150);
            cm.AddColumn("Units", "Units", "Ед. измерения", DataAttributeTypes.dtString, Mandatory.Nullable);
            var cb = cm.AddColumn("ValueEstimate", "ValueEstimate", "Оценочный год<br/>{0} год".FormatWith(BaseYear + 1), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            (cb as NumberColumn).Format = "0.0000";
            cb = cm.AddColumn("ValueY1", "ValueY1", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 2), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            (cb as NumberColumn).Format = "0.0000";
            cb = cm.AddColumn("ValueY2", "ValueY2", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 3), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            (cb as NumberColumn).Format = "0.0000";
            cb = cm.AddColumn("ValueY3", "ValueY3", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 4), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            (cb as NumberColumn).Format = "0.0000";
            cb = cm.AddColumn("ValueY4", "ValueY4", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 5), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            (cb as NumberColumn).Format = "0.0000";
            cb = cm.AddColumn("ValueY5", "ValueY5", "Прогнозный год<br/>{0} год".FormatWith(BaseYear + 6), DataAttributeTypes.dtDouble, Mandatory.Nullable);
            (cb as NumberColumn).Format = "0.0000";
            cb = cm.AddColumn("MaxBound", "MaxBound", "Верхняя граница", DataAttributeTypes.dtDouble, Mandatory.Nullable);
            (cb as NumberColumn).Format = "0.0000";
            cb = cm.AddColumn("MinBound", "MinBound", "Нижняя граница", DataAttributeTypes.dtDouble, Mandatory.Nullable);
            (cb as NumberColumn).Format = "0.0000";
            cm.AddColumn("GroupName", "GroupName", "Группа", DataAttributeTypes.dtString, Mandatory.Nullable);

            indGridPanel.AddColumnsWrapStylesToPage(page);

            indGridPanel.SelectionModel.Add(new RowSelectionModel());

            panelInds.Items.Add(indGridPanel);
            return panelInds;
        }
        
        private Store CreateAdjsStore()
        {
            Store store = new Store { ID = AdjStoreId, GroupField = "GroupName", AutoLoad = false };
            
            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("ParamName");
            reader.Fields.Add("ValueEstimate");
            reader.Fields.Add("ValueY1");
            reader.Fields.Add("ValueY2");
            reader.Fields.Add("ValueY3");
            reader.Fields.Add("ValueY4");
            reader.Fields.Add("ValueY5");
            reader.Fields.Add("MaxBound");
            reader.Fields.Add("MinBound");
            reader.Fields.Add("IndexDef");
            reader.Fields.Add("UserName");
            reader.Fields.Add("Finished");
            reader.Fields.Add("GroupName");
            reader.Fields.Add("Units");

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/ScenarioParam/AdjsLoad",
                Method = HttpMethod.POST
            });

            store.WriteBaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("savedData", String.Format("{0}.getChangedData()", AdjStoreId), ParameterMode.Raw));

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/ScenarioParam/AdjsSave",
                Method = HttpMethod.POST,
                Timeout = 50000
            });

            return store;
        }
        
        private Store CreateIndsStore()
        {
            Store store = new Store { ID = IndStoreId, GroupField = "GroupName", AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("ParamName");
            reader.Fields.Add("ValueEstimate");
            reader.Fields.Add("ValueY1");
            reader.Fields.Add("ValueY2");
            reader.Fields.Add("ValueY3");
            reader.Fields.Add("ValueY4");
            reader.Fields.Add("ValueY5");
            reader.Fields.Add("MaxBound");
            reader.Fields.Add("MinBound");
            reader.Fields.Add("GroupName");
            reader.Fields.Add("Units");
            
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/ScenarioParam/IndsLoad",
                Method = HttpMethod.POST
            });

            return store;
        }

        private Store CreateStaticStore()
        {
            Store store = new Store { ID = StatStoreId, GroupField = "GroupName", AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("ParamName");
            reader.Fields.Add("ValueBase");
            reader.Fields.Add("ValueEstimate");
            reader.Fields.Add("UserName");
            reader.Fields.Add("Finished");
            reader.Fields.Add("GroupName");
            reader.Fields.Add("Units");

            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/ScenarioParam/StaticLoad",
                Method = HttpMethod.POST
            });

            store.WriteBaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("savedData", String.Format("{0}.getChangedData()", StatStoreId), ParameterMode.Raw));

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/ScenarioParam/StaticSave",
                Method = HttpMethod.POST,
                Timeout = 50000
            });

            return store;
        }

        private Store CreateUnregStore()
        {
            Store store = new Store { ID = UnregStoreId, GroupField = "GroupName", AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            
            reader.Fields.Add("ID");
            reader.Fields.Add("ParamName");
            reader.Fields.Add("ValueEstimate");
            reader.Fields.Add("ValueY1");
            reader.Fields.Add("ValueY2");
            reader.Fields.Add("ValueY3");
            reader.Fields.Add("ValueY4");
            reader.Fields.Add("ValueY5");
            reader.Fields.Add("UserName");
            reader.Fields.Add("Finished");
            reader.Fields.Add("GroupName");
            reader.Fields.Add("Units");
            
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/ScenarioParam/UnregLoad",
                Method = HttpMethod.POST
            });

            store.WriteBaseParams.Add(new Parameter("scenId", ScenId.ToString(), ParameterMode.Value));
            store.WriteBaseParams.Add(new Parameter("savedData", String.Format("{0}.getChangedData()", UnregStoreId), ParameterMode.Raw));

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/ScenarioParam/UnregSave",
                Method = HttpMethod.POST,
                Timeout = 50000
            });

            return store;
        }
    }
}
