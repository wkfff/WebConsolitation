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

namespace Krista.FM.RIA.Extensions.Forecast.MathStat
{
    public class PlanningRegulatorsControl : Control
    {
        public readonly string GridId = "gpRegulators";
        public readonly string StoreId = "dsRegulators";

        private IForecastRegulatorsValueRepository regulatorsValueRepository;
        private SortedList<int, string> lstOfYear = new SortedList<int, string>();

        public PlanningRegulatorsControl(IForecastRegulatorsValueRepository regulatorsValueRepository)
        {
            this.regulatorsValueRepository = regulatorsValueRepository;
            
            var list = from f in regulatorsValueRepository.GetAllValues()
                       orderby f.RefDate.ID
                       select new
                       {
                           Year = f.RefDate.ID / 10000,
                       };

            foreach (var item in list)
            {
                if (!lstOfYear.ContainsKey(item.Year))
                {
                    lstOfYear.Add(item.Year, "year_{0}".FormatWith(item.Year));
                }
            }
        }
        
        public override List<Component> Build(ViewPage page)
        {
            page.Controls.Add(CreateStore());
            page.Controls.Add(CreateValuesStore());

            Panel panel = new Panel
            {
                Height = 300
            };

            GridPanel gp = new GridPanel
            {
                Collapsible = false,
                ID = GridId,
                StoreID = StoreId,
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                Height = 275,
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;"
            };
            
            ColumnModel cm = gp.ColumnModel;
            ////cm.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(50);
            gp.ColumnModel.AddColumn("Name", "Name", "Наименование регулятора", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(400);
            gp.ColumnModel.AddColumn("Descr", "Descr", "Описание", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(200);
            gp.ColumnModel.AddColumn("Untis", "Units", "Единицы измерения", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(100);

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            gp.Listeners.CellClick.AddAfter("dsRegulatorsValue.reload();");

            panel.Items.Add(gp);

            return new List<Component> { panel };
        }

        public Component ValuesGrid(ViewPage page)
        {
            Panel panel = new Panel
            {
                Layout = "fit"
            };

            GridPanel gp = new GridPanel
            {
                Collapsible = false,
                ID = "gpRegulatorsValue",
                StoreID = "dsRegulatorsValue",
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                Layout = "fit",
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;"
            };
            
            ColumnModel cm = gp.ColumnModel;
            ////cm.AddColumn("ID", "ID", "ID", DataAttributeTypes.dtInteger, Mandatory.NotNull).SetWidth(50);
            gp.ColumnModel.AddColumn("scenCondName", "scenCondName", "Вариант", DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(100);
            
            foreach (var item in lstOfYear)
            {
                var cb = gp.ColumnModel.AddColumn(item.Value, item.Value, item.Key.ToString(), DataAttributeTypes.dtString, Mandatory.NotNull).SetWidth(100);
                cb.SetVisible(false);
            }

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            ////gp.SelectionModel.Add(new RowSelectionModel());

            panel.Items.Add(gp);

            return panel;
        }

        public Store CreateStore()
        {
            Store store = new Store { ID = StoreId };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Name");
            reader.Fields.Add("Descr");
            reader.Fields.Add("Units");
            
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningRegulators/Load",
                Method = HttpMethod.POST
            });

            return store;
        }

        public Store CreateValuesStore()
        {
            Store store = new Store { ID = "dsRegulatorsValue", AutoLoad = false };
            
            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("scenCondName");
            foreach (var item in lstOfYear)
            {
                reader.Fields.Add(item.Value);
            }
            
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("regId", "{0}.getSelectionModel().hasSelection() ? {0}.getSelectionModel().getSelected().id : -1".FormatWith(GridId), ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningRegulators/ValuesLoad",
                Method = HttpMethod.POST
            });

            store.Listeners.Load.AddAfter(@"
var colcount = gpRegulatorsValue.getColumnModel().getColumnCount();

for (i=0; i<colcount; i++){
    gpRegulatorsValue.getColumnModel().setHidden(i,true);
}

var data = store.reader.jsonData.data[0];
for (x in data){
    var index = gpRegulatorsValue.getColumnModel().getIndexById(x);
    if (index >= 0)    {
        gpRegulatorsValue.getColumnModel().setHidden(index,false);
    }
}");

            return store;
        }
    }
}
