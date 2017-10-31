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
    public class PlanningCritsControl : Control
    {
        private const string StoreCriteriasId = "stCritData";

        private readonly string Key;
        private readonly IForecastExtension extension;

        public PlanningCritsControl(IForecastExtension extension, string key)
        {
            this.extension = extension;
            Key = key;
        }

        public Store StoreCrits { get; private set; }
        
        public override List<Component> Build(ViewPage page)
        {
            StoreCrits = CreateCritStore(StoreCriteriasId);
            page.Controls.Add(StoreCrits);

            GridPanel gridCrits = new GridPanel
            {
                ID = "gpCrits",
                StoreID = StoreCriteriasId,
                AutoHeight = true
            };

            ColumnModel columnModel = gridCrits.ColumnModel;
            columnModel.AddColumn("Name", "Name", "Параметр", DataAttributeTypes.dtString, Mandatory.NotNull);
            columnModel.AddColumn("Value", "Value", "Значение", DataAttributeTypes.dtDouble, Mandatory.NotNull);
            var column = columnModel.AddColumn("Text", "Text", "Описание", DataAttributeTypes.dtString, Mandatory.NotNull);
            string script = @"gpCrits.colModel.setColumnWidth(2,(gpCrits.lastSize.width-gpCrits.colModel.getColumnWidth(0)-gpCrits.colModel.getColumnWidth(1)-10),false)";

            gridCrits.AddScript(script);

            ////column.Width = 350;
            
            /*Panel panStatist = new Panel
            {
                ID = "pStatist",
                Title = "Статистика"
            };

            panStatist.Items.Add(gridCrits);*/

            gridCrits.Listeners.AfterRender.AddAfter(String.Format("{0}.load();", StoreCriteriasId));

            return new List<Component> { gridCrits };
        }

        public Store CreateCritStore(string storeId)
        {
            Store store = new Store { ID = storeId, AutoLoad = false };

            JsonReader reader = new JsonReader { Root = "data" };
            store.Reader.Add(reader);
            reader.Fields.Add("Name");
            reader.Fields.Add("Value");
            reader.Fields.Add("Text");

            store.BaseParams.Add(new Parameter("key", String.Format("'{0}'", Key), ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/PlanningCrits/LoadCrits",  ////progchartdata
                Method = HttpMethod.POST
            });

            return store;
        }
    }
}
