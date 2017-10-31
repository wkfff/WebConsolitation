using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Presentation.Controls
{
    public class MarksTableControl : Control
    {
        private const string StoreId = "dsMarks";
        private const string GridId = "gpMarks";

        private readonly string controllerName;
        private readonly int year;

        public MarksTableControl(string controllerName, int year)
        {
            this.controllerName = controllerName;
            this.year = year;
        }

        public override List<Component> Build(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);
            RegisterResources(resourceManager);

            return new List<Component> { CreateGridPanel(page) };
        }

        private GridPanel CreateGridPanel(ViewPage page)
        {
            GridPanel gp = new GridPanel
            {
                ID = GridId,
                Store = { CreateStore(StoreId) },
                MonitorResize = true,
                Border = false,
                AutoScroll = true,
                Height = 600,
                AutoExpandColumn = "CompRep",
                ColumnLines = true,
                LoadMask = { ShowMask = true },
                SaveMask = { ShowMask = true }
            };

            var view = new GroupingView
            {
                ForceFit = false,
                HideGroupedColumn = true,
                EnableGrouping = true,
                StartCollapsed = true,
                EnableNoGroups = false
            };
            gp.View.Add(view);

            var groupRow = new HeaderGroupRow();
            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 7 });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Отчет", ColSpan = 1, Align = Alignment.Center });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Прогноз", ColSpan = 3, Align = Alignment.Center });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });

            view.HeaderGroupRows.Add(groupRow);

            gp.ColumnModel.Columns.Add(new Column { ColumnID = "ID", DataIndex = "ID", Header = "Id", Hidden = true });
            gp.ColumnModel.Columns.Add(new Column { ColumnID = "State", DataIndex = "RefStatusData", Width = 30, Groupable = false, Renderer = { Fn = "columnRenderStatus" } });
            gp.ColumnModel.Columns.Add(new Column { ColumnID = "CompRep", DataIndex = "CompRep", Header = "Раздел", Wrap = true, Groupable = true });
            gp.ColumnModel.Columns.Add(new Column { ColumnID = "MarkName", DataIndex = "MarkName", Header = "Показатель", Width = 300, Wrap = true, Groupable = false });
            gp.ColumnModel.Columns.Add(new Column { ColumnID = "OKEI", DataIndex = "OKEI", Header = "Единицы измерения", Width = 70, Groupable = false, Wrap = true });
            gp.ColumnModel.Columns.Add(new Column { ColumnID = "CodeRep", DataIndex = "CodeRep", Header = "Номер в докладе", Width = 55, Groupable = false, Wrap = true });

            var cl = gp.ColumnModel.AddColumn("Fact", "Fact", year.ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Отчет").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderMO()";

            cl = gp.ColumnModel.AddColumn("Forecast", "Forecast", (year + 1).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Прогноз").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderPrognoz()";

            cl = gp.ColumnModel.AddColumn("Forecast2", "Forecast2", (year + 2).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Прогноз").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderPrognoz()";

            cl = gp.ColumnModel.AddColumn("Forecast3", "Forecast3", (year + 3).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Прогноз").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderPrognoz()";

            gp.ColumnModel.AddColumn("Note", "Note", "Примечание", DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(200).SetGroupable(false).SetEditableString();

            gp.Listeners.BeforeEdit.Fn = "beforeEditCell";

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.Plugins.Add(CreateRowExpander());

            gp.SelectionModel.Add(new RowSelectionModel());

            return gp;
        }

        private RowExpander CreateRowExpander()
        {
            RowExpander rowExpander = new RowExpander { ID = "gridRowExpander", ExpandOnDblClick = false };
            rowExpander.Template.Html = @"<div id=""row-{ID}"" style=""background-color:White;""></div>";

            rowExpander.DirectEvents.BeforeExpand.Url = String.Format("/{0}/Expand", controllerName);
            rowExpander.DirectEvents.BeforeExpand.CleanRequest = true;
            rowExpander.DirectEvents.BeforeExpand.IsUpload = false;
            rowExpander.DirectEvents.BeforeExpand.Before = "return !body.rendered;";
            rowExpander.DirectEvents.BeforeExpand.Success = "body.rendered=true;";

            rowExpander.DirectEvents.BeforeExpand.EventMask.ShowMask = true;
            rowExpander.DirectEvents.BeforeExpand.EventMask.Target = MaskTarget.CustomTarget;
            rowExpander.DirectEvents.BeforeExpand.EventMask.CustomTarget = GridId;
            rowExpander.DirectEvents.BeforeExpand.ExtraParams.Add(new Parameter("id", "record.id", ParameterMode.Raw));
            rowExpander.DirectEvents.BeforeExpand.ExtraParams.Add(new Parameter("markId", "record.get('RefOIV')", ParameterMode.Raw));

            return rowExpander;
        }

        private Store CreateStore(string storeId)
        {
            Store store = new Store { ID = storeId, GroupField = "CompRep" };

            JsonReader reader = new JsonReader { IDProperty = "ID", Root = "data", TotalProperty = "total" };
            reader.Fields.Add("ID");
            reader.Fields.Add("RefStatusData");
            reader.Fields.Add("Status");
            reader.Fields.Add("CompRep");
            reader.Fields.Add("MarkName");
            reader.Fields.Add("OKEI");
            reader.Fields.Add("CodeRep");
            reader.Fields.Add("Formula");
            reader.Fields.Add("MO");
            reader.Fields.Add("Fact");
            reader.Fields.Add("Forecast");
            reader.Fields.Add("Forecast2");
            reader.Fields.Add("Forecast3");
            reader.Fields.Add("Note");
            reader.Fields.Add("RefOIV");
            reader.Fields.Add("RefRegions");
            reader.Fields.Add("RefYear");
            reader.Fields.Add("Capacity");
            reader.Fields.Add("Readonly");
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = String.Format("/{0}/Load", controllerName),
                Method = HttpMethod.POST,
            });

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = String.Format("/{0}/Save", controllerName),
                Method = HttpMethod.POST,
                Timeout = 500000
            });

            store.BaseParams.Add(new Parameter("filter", "getStateFilter()", ParameterMode.Raw));

            store.Listeners.DataChanged.Handler = @"
var year = parseInt(parent.cmbOivYearChooser.getValue());
gpMarks.getColumnModel().setColumnHeader(7, year);
gpMarks.getColumnModel().setColumnHeader(8, year+1);
gpMarks.getColumnModel().setColumnHeader(9, year+2);
gpMarks.getColumnModel().setColumnHeader(10, year+3);
labelYear.update('<b>Период</b>: '+ year +' год');
";

            return store;
        }

        private void RegisterResources(ResourceManager resourceManager)
        {
            resourceManager.RegisterIcon(Icon.UserEdit);
            resourceManager.RegisterIcon(Icon.UserMagnify);
            resourceManager.RegisterIcon(Icon.UserTick);
            resourceManager.RegisterIcon(Icon.Accept);

            resourceManager.RegisterScript("Hack", "/Content/js/Ext.util.Format.number.Hack.js");

            resourceManager.RegisterClientStyleBlock(
                "CustomStyle",
                ".disable-cell{background-color: #EDEDED !important;}");

            resourceManager.RegisterClientScriptBlock("MarksTableControl", Resources.Resource.MarksTableControl);
        }
    }
}
