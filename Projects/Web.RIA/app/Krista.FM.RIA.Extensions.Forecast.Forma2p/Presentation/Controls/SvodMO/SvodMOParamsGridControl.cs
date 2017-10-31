using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Forecast.Forma2p
{
    public class SvodMOParamsGridControl : Control
    {
        private const string StoreId = "dsSvodMOParams";
        private const string GridId = "gpSvodMOParams";

        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateStore());

            GridPanel gp = new GridPanel
            {
                Collapsible = false,
                ID = GridId,
                StoreID = StoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                ////Width = 175,
                ////Height = 600,
                AutoWidth = true,
                AutoHeight = true,
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
            cm.AddColumn("Name", "Name", "Параметр", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(500);
            cm.AddColumn("Units", "Units", "Ед.измерения", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(200);
            cm.AddColumn("Group", "Group", "Группа", DataAttributeTypes.dtString, Mandatory.Nullable);

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            return new List<Component> { gp };
        }

        public Store CreateStore()
        {
            Store store = new Store { ID = StoreId, GroupField = "Group", AutoLoad = true };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Name");
            reader.Fields.Add("Units");
            reader.Fields.Add("Group");

            store.Reader.Add(reader);
            
            store.Proxy.Add(new HttpProxy
            {
                Url = "/SvodMOParams/Load",
                Method = HttpMethod.POST
            });

            return store;
        }
    }
}
