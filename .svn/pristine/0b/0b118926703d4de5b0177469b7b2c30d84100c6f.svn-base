using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningParamsGridControl : Control
    {
        private const string StoreId = "dsParams";
        private const string GridId = "gpParams";
        
        public Store Store { get; private set; }

        public GridPanel GridPanel { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            Store = CreateStore(StoreId);
            page.Controls.Add(Store);

            GridPanel = CreateGridPanel(GridId, StoreId, page);
            return new List<Component> { GridPanel };
        }
        
        public GridPanel CreateGridPanel(string gridId, string storeId, ViewPage page)
        {
            GridPanel gp = new GridPanel
            {
                Collapsible = true,
                ID = gridId,
                StoreID = storeId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                Width = 175,
                Height = 600,
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;"
            };

            GroupingView groupingView = new GroupingView
            {
                ID = "GroupingView1",
                HideGroupedColumn = true,
                EnableNoGroups = true,
                GroupTextTpl = "{text} ({[values.rs.length]} {[values.rs.length > 1 ? 'Items' : 'Item']})"
            };
            
            gp.View.Add(groupingView);
            ColumnModel cm = gp.ColumnModel;
            ////cm.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(50);
            gp.ColumnModel.AddColumn("Name", "Name", "Наименование", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(400);
            ////gp.ColumnModel.AddColumn("XMLString", "XMLString", "XML настройка", DataAttributeTypes.dtString, Mandatory.NotNull).SetVisible(false);
            gp.ColumnModel.AddColumn("refOKEI", "refOKEI", "Единицы измерения", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(200);
            ////gp.ColumnModel.AddColumn("ParentID", "ParentID", "ParentID", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(50);
            gp.ColumnModel.AddColumn("grp", "grp", "Группа", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(50);
            
            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            return gp;
        }

        public Store CreateStore(string storeId)
        {
            Store store = new Store { ID = storeId, GroupField = "grp" };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Name");
            reader.Fields.Add("XMLString");
            reader.Fields.Add("refOKEI");
            reader.Fields.Add("Prognoseable");
            reader.Fields.Add("ParamForm2P");
            reader.Fields.Add("ParentID");
            reader.Fields.Add("grp");
            
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningParams/Load",
                Method = HttpMethod.POST
            });

            return store;
        }
    }
}
