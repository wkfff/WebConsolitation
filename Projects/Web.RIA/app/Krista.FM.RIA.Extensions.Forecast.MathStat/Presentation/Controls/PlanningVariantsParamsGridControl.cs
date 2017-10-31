using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;
using GridView = Ext.Net.GridView;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public enum PlanVarSource
    {
        /// <summary>
        /// Сообщает гриду что он будет использоваться при добавлении строк к прогнозу
        /// </summary>
        AddRow = 0,

        /// <summary>
        /// Сообщает гриду что он будет использоваться при создании нового прогноза
        /// </summary>
        AddForecast = 1
    }
    
    public class PlanningVariantsParamsGridControl : Control
    {
        private const string StoreParamsId = "dsVariantParams";
        private const string GridParamsId = "gpVariantParams";

        private const string StoreRegsId = "dsVariantRegs";
        private const string GridRegsId = "gpVariantRegs";

        private readonly string Key;

        private readonly PlanVarSource source;
        
        public PlanningVariantsParamsGridControl(PlanVarSource source)
        {
            this.source = source;
        }

        public PlanningVariantsParamsGridControl(PlanVarSource source, string key)
        {
            this.source = source;
            this.Key = key;
        }

        ////public Store Store { get; private set; }

        public GridPanel ParamsGridPanel { get; private set; }

        public GridPanel RegsGridPanel { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateParamStore());

            if (source == PlanVarSource.AddRow)
            {
                page.Controls.Add(CreateRegsStore());
            }

            Panel panel = new Panel
            {
                ID = "panelParent",
                Width = 420,
                Height = 190,
                Border = false
            };

            if (source == PlanVarSource.AddRow)
            {
                panel.BodyCssClass = "x-window-mc";
                panel.CssClass = "x-window-mc";
            }
            
            ParamsGridPanel = ParamsGrid(page);
            
            if (source == PlanVarSource.AddRow)
            {
                RegsGridPanel = RegsGrid(page);
                
                panel.Layout = "accordion";

                Panel panelParams = new Panel
                {
                    ID = "panelParams",
                    Border = false,
                    Title = "Параметры"
                };

                panelParams.Items.Add(ParamsGridPanel);

                Panel panelRegs = new Panel
                {
                    ID = "panelRegs",
                    Border = false,
                    Title = "Регуляторы"
                };

                panelRegs.Items.Add(RegsGridPanel);

                panel.Items.Add(panelParams);
                panel.Items.Add(panelRegs);
            }
            else
            {
                panel.Items.Add(ParamsGridPanel);
            }

            return new List<Component> { panel };
        }

        public Store CreateParamStore()
        {
            Store store = new Store { ID = StoreParamsId, GroupField = "grp" };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Name");
            reader.Fields.Add("grp");
            
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningParams/LoadListParams",
                Method = HttpMethod.POST
            });

            return store;
        }

        public Store CreateRegsStore()
        {
            Store store = new Store { ID = StoreRegsId /*, GroupField = "grp" */ };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Name");

            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningRegulators/LoadListRegs",
                Method = HttpMethod.POST
            });

            return store;
        }

        private GridPanel ParamsGrid(ViewPage page)
        {
            GridPanel gp = new GridPanel
            {
                ID = GridParamsId,
                StoreID = StoreParamsId,
                MonitorResize = true,
                AutoScroll = true,
                Height = 170,
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                Collapsible = false,
                Width = 410
            };

            ColumnBase cb = gp.ColumnModel.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.NotNull);
            cb.SetWidth(50);
            cb.Hidden = true;

            gp.ColumnModel.AddColumn("Name", "Name", "Наименование", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(400);
            gp.ColumnModel.AddColumn("grp", "grp", "Группа", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(50);

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            GroupingView groupingView = new GroupingView
            {
                ID = "gvPlanVarParam",
                HideGroupedColumn = true,
                GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Items' : 'Item']})"
            };

            gp.View.Add(groupingView);
            return gp;
        }

        private GridPanel RegsGrid(ViewPage page)
        {
            GridPanel gp = new GridPanel
            {
                ID = GridRegsId,
                StoreID = StoreRegsId,
                MonitorResize = true,
                AutoScroll = true,
                Height = 170,
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                Collapsible = false,
                Width = 410
            };

            ColumnBase cb = gp.ColumnModel.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.NotNull);
            cb.SetWidth(50);
            cb.Hidden = true;

            gp.ColumnModel.AddColumn("Name", "Name", "Наименование", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(400);
            ////gp.ColumnModel.AddColumn("grp", "grp", "Группа", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(50);

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            /*GroupingView groupingView = new GroupingView
            {
                ID = "gvPlanVarParam",
                HideGroupedColumn = true,
                GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Items' : 'Item']})"
            };

            gp.View.Add(groupingView);*/
            return gp;
        }
    }
}
