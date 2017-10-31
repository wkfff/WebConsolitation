using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls
{
    public class MarksOivGridControl : Control
    {
        private const string StoreId = "dsMarks";
        private const string GridId = "gpMarks";
        
        public Store Store { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            if (!ExtNet.IsAjaxRequest)
            {
                RegisterResources(page);
            }

            Store = CreateStore(StoreId);
            page.Controls.Add(Store);

            return new List<Component> { CreateGridPanel(GridId, StoreId, page) };
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
                StyleSpec = "margin-top: 5px; margin-bottom: 5px;",
                ColumnLines = true,
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true }
            };

            var view = new GroupingView { ForceFit = false, HideGroupedColumn = true };
            gp.View.Add(view);

            var groupRow = new HeaderGroupRow();
            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 3 });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Предыдущий", ColSpan = 1, Align = Alignment.Center });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Отчет", ColSpan = 2, Align = Alignment.Center });

            view.HeaderGroupRows.Add(groupRow);

            gp.ColumnModel.Columns.Add(new Column { ColumnID = "ID", DataIndex = "ID", Header = "Id", Hidden = true });

            var stateColumn = new Column { ColumnID = "State", DataIndex = "RefStatusData", Width = 30, Groupable = false };
            stateColumn.Renderer.Fn = "columnRenderStatus";
            gp.ColumnModel.Columns.Add(stateColumn);

            gp.ColumnModel.AddColumn("Region", "Region", "Наименование МО", DataAttributeTypes.dtString, Mandatory.NotNull)
                .SetWidth(300).SetGroupable(false);

            var cl = gp.ColumnModel.AddColumn("PriorYearCurrentValue", "PriorYearCurrentValue", "2008", DataAttributeTypes.dtString, Mandatory.Nullable)
                          .SetWidth(90) 
                          .SetGroupable(false)
                          .SetEditable(false)
                          .SetVisible(false);
            cl.Renderer.Fn = "columnRenderValue()";
            cl.Align = Alignment.Center;

            cl = gp.ColumnModel.AddColumn("PriorValue", "PriorValue", "2009", DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Отчет").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderValue()";

            cl = gp.ColumnModel.AddColumn("CurrentValue", "CurrentValue", "2010", DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Отчет").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderValue()";

            gp.Listeners.BeforeEdit.AddAfter(@"
if (e.record.get('RefStatusData') != 1 || e.record.get('Readonly')) {
    return false;
}
return true;");

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.SelectionModel.Add(new RowSelectionModel());

            return gp;
        }

        public Store CreateStore(string storeId)
        {
            Store store = new Store { ID = storeId };

            JsonReader reader = new JsonReader { Root = "data", IDProperty = "ID" };
            reader.Fields.Add("ID");
            reader.Fields.Add("Region");
            reader.Fields.Add("RegionCodeLine");
            reader.Fields.Add("PriorYearCurrentValue");
            reader.Fields.Add("PriorValue");
            reader.Fields.Add("CurrentValue");
            reader.Fields.Add("Prognoz1");
            reader.Fields.Add("Prognoz2");
            reader.Fields.Add("Prognoz3");
            reader.Fields.Add("Note");
            reader.Fields.Add("RefMarksOMSU");
            reader.Fields.Add("RefRegions");
            reader.Fields.Add("RefStatusData");
            reader.Fields.Add("RefYearDayUNV");
            reader.Fields.Add("OKEI");
            reader.Fields.Add("Capacity");
            reader.Fields.Add("Readonly");
            store.Reader.Add(reader);

            store.BaseParams.Add(new Parameter("markId", "marksCombo.getSelectedItem().value", ParameterMode.Raw));

            store.Proxy.Add(new HttpProxy
            {
                Url = "/MarksOiv/Load",
                Method = HttpMethod.POST
            });

            var httpWriteProxy = new HttpWriteProxy
            {
                Url = "/MarksOiv/Save",
                Method = HttpMethod.POST,
                Timeout = 500000
            };
            store.UpdateProxy.Add(httpWriteProxy);

            store.SortInfo.Field = "RegionCodeLine";
            store.SortInfo.Direction = SortDirection.ASC;

            return store;
        }

        private static void RegisterResources(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            resourceManager.RegisterClientStyleBlock(
                "CustomStyle",
                ".disable-cell{background-color: #EDEDED !important;}");

            resourceManager.RegisterClientScriptBlock(
                "MarksOivGridControl", 
                Resources.Resource.MarksOivGridControl);

            resourceManager.RegisterIcon(Icon.UserEdit);
            resourceManager.RegisterIcon(Icon.UserMagnify);
            resourceManager.RegisterIcon(Icon.UserTick);
            resourceManager.RegisterIcon(Icon.Accept);

            resourceManager.RegisterScript("Hack", "/Content/js/Ext.util.Format.number.Hack.js");
        }
    }
}
