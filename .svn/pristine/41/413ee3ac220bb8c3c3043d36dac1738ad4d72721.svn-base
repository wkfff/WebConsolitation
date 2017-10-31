using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls
{
    public class ResponsOivGridControl : Control
    {
        public GridPanel GridPanel { get; private set; }
        
        public Store Store { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            Store = CreateStore("dsOIV");
            page.Controls.Add(Store);

            GridPanel = CreateGridPanel("gpOIV", "dsOIV", page);
            return new List<Component> { GridPanel };
        }

        public GridPanel CreateGridPanel(string gridId, string storeId, ViewPage page)
        {
            GridPanel gp = new GridPanel
            {
                ID = gridId,
                StoreID = storeId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                Height = 600,
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;"
            };

            gp.ColumnModel.AddColumn("Code", "Code", "Код", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(50);
            gp.ColumnModel.AddColumn("Name", "Name", "Наименование", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(700);

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            return gp;
        }
        
        public Store CreateStore(string storeId)
        {
            Store store = new Store { ID = storeId };

            JsonReader reader = new JsonReader { IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Code");
            reader.Fields.Add("Name");
            store.Reader.Add(reader);

            return store;
        }
    }
}
