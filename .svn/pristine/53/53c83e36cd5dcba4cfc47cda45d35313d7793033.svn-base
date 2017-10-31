using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls
{
    public class MarksOmsuGridControl : Control
    {
        private const string StoreId = "dsMarks";
        private const string GridId = "gpMarks";

        private readonly IMarksOmsuExtension extension;

        public MarksOmsuGridControl(IMarksOmsuExtension extension)
        {
            this.extension = extension;
        }

        public string StoreController { get; set; }

        public Store Store { get; private set; }

        public GridPanel GridPanel { get; private set; }

        public override List<Component> Build(ViewPage page)
        {
            Store = CreateStore(StoreId);
            page.Controls.Add(Store);

            return new List<Component> { CreateGridPanel(GridId, StoreId, page) };
        }

        public GridPanel CreateGridPanel(string gridId, string storeId, ViewPage page)
        {
            if (!ExtNet.IsAjaxRequest)
            {
                RegisterResources(page);
            }

            GridPanel gp = new GridPanel
            {
                ID = gridId,
                StoreID = storeId,
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
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Предыдущий", ColSpan = 1, Align = Alignment.Center });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Отчет", ColSpan = 2, Align = Alignment.Center });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = "Прогноз", ColSpan = 3, Align = Alignment.Center });
            groupRow.Columns.Add(new HeaderGroupColumn { Header = String.Empty, ColSpan = 1 });

            view.HeaderGroupRows.Add(groupRow);

            gp.ColumnModel.Columns.Add(new Column { ColumnID = "ID", DataIndex = "ID", Header = "Id", Hidden = true });

            var stateColumn = new Column { ColumnID = "State", DataIndex = "RefStatusData", Width = 30, Groupable = false };
            stateColumn.Renderer.Fn = "columnRenderStatus";

            gp.ColumnModel.Columns.Add(stateColumn);

            gp.ColumnModel.Columns.Add(new Column { ColumnID = "CompRep", DataIndex = "CompRep", Header = "Раздел", Wrap = true, Groupable = true });
            gp.ColumnModel.Columns.Add(new Column { ColumnID = "MarkName", DataIndex = "MarkName", Header = "Показатель", Width = 300, Wrap = true, Groupable = false });
            gp.ColumnModel.Columns.Add(new Column { ColumnID = "OKEI", DataIndex = "OKEI", Header = "Единицы измерения", Width = 70, Groupable = false, Wrap = true });
            gp.ColumnModel.Columns.Add(new Column { ColumnID = "CodeRepDouble", DataIndex = "CodeRepDouble", Header = "Номер в докладе", Width = 55, Groupable = false, Wrap = true });

            var year = this.extension.CurrentYear;

            var cl = gp.ColumnModel.AddColumn("PriorYearCurrentValue", "PriorYearCurrentValue", (year - 1).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
              .SetWidth(90)
              .SetGroupable(false)
              .SetEditable(false)
              .SetVisible(false);
            cl.Renderer.Fn = "columnRenderCurrent()";
            cl.Align = Alignment.Center;

            cl = gp.ColumnModel.AddColumn("PriorValue", "PriorValue", (year - 1).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Отчет").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderCurrent()";
            
            cl = gp.ColumnModel.AddColumn("CurrentValue", "CurrentValue", year.ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Отчет").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderCurrent()";

            cl = gp.ColumnModel.AddColumn("Prognoz1", "Prognoz1", (year + 1).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Прогноз").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderPrognoz()";

            cl = gp.ColumnModel.AddColumn("Prognoz2", "Prognoz2", (year + 2).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Прогноз").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderPrognoz()";

            cl = gp.ColumnModel.AddColumn("Prognoz3", "Prognoz3", (year + 3).ToString(), DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(90).SetGroup("Прогноз").SetGroupable(false).SetEditableDouble(4);
            cl.Renderer.Fn = "columnRenderPrognoz()";

            gp.ColumnModel.AddColumn("Note", "Note", "Примечание", DataAttributeTypes.dtString, Mandatory.Nullable)
                .SetWidth(200).SetGroupable(false).SetEditableString();

            gp.Listeners.BeforeEdit.Fn = "beforeEditCell";

            // Устанавливаем для полей стиль переноса по словам
            gp.AddColumnsWrapStylesToPage(page);

            gp.Plugins.Add(CreateRowExpander(gp.ID));

            gp.SelectionModel.Add(new RowSelectionModel());

            GridPanel = gp;

            return gp;
        }

        public Store CreateStore(string storeId)
        {
            Store store = new Store { ID = storeId, GroupField = "CompRep" };

            JsonReader reader = new JsonReader { IDProperty = "ID", Root = "data", TotalProperty = "total" };
            reader.Fields.Add("ID");
            reader.Fields.Add("CompRep");
            reader.Fields.Add("MarkName");
            reader.Fields.Add("OKEI");
            reader.Fields.Add("CodeRepDouble");
            reader.Fields.Add("Formula");
            reader.Fields.Add("Grouping");
            reader.Fields.Add("PriorYearCurrentValue");
            reader.Fields.Add("PriorValue");
            reader.Fields.Add("CurrentValue");
            reader.Fields.Add("Prognoz1");
            reader.Fields.Add("Prognoz2");
            reader.Fields.Add("Prognoz3");
            reader.Fields.Add("Note");
            reader.Fields.Add("Status");
            reader.Fields.Add("RefMarksOMSU");
            reader.Fields.Add("RefYearDayUNV");
            reader.Fields.Add("RefRegions");
            reader.Fields.Add("RefStatusData");
            reader.Fields.Add("MO");
            reader.Fields.Add("Capacity");
            reader.Fields.Add("ReadonlyCurrent");
            reader.Fields.Add("ReadonlyPrognoz");
            store.Reader.Add(reader);

            store.Proxy.Add(new HttpProxy
            {
                Url = "/{0}/Load".FormatWith(StoreController),
                Method = HttpMethod.POST,
            });

            store.UpdateProxy.Add(new HttpWriteProxy
            {
                Url = "/{0}/Save".FormatWith(StoreController),
                Method = HttpMethod.POST,
                Timeout = 500000
            });

            return store;
        }

        private static void RegisterResources(ViewPage page)
        {
            var resourceManager = ResourceManager.GetInstance(page);

            resourceManager.RegisterIcon(Icon.UserEdit);
            resourceManager.RegisterIcon(Icon.UserMagnify);
            resourceManager.RegisterIcon(Icon.UserTick);
            resourceManager.RegisterIcon(Icon.Accept);

            resourceManager.RegisterScript("Hack", "/Content/js/Ext.util.Format.number.Hack.js");

            resourceManager.RegisterClientStyleBlock(
                "CustomStyle",
                ".disable-cell{background-color: #EDEDED !important;}");

            resourceManager.RegisterClientScriptBlock(
                "MarksOmsuGridControl",
                Resources.Resource.MarksOmsuGridControl);
        }

        private RowExpander CreateRowExpander(string gridId)
        {
            RowExpander rowExpander = new RowExpander { ID = "gridRowExpander", ExpandOnDblClick = false };
            rowExpander.Template.Html = @"<div id=""row-{ID}"" style=""background-color:White;""></div>";

            rowExpander.DirectEvents.BeforeExpand.Url = "/{0}/Expand".FormatWith(StoreController);
            rowExpander.DirectEvents.BeforeExpand.CleanRequest = true;
            rowExpander.DirectEvents.BeforeExpand.IsUpload = false;
            rowExpander.DirectEvents.BeforeExpand.Before = "return !body.rendered;";
            if (gridId == GridId)
            {
                rowExpander.DirectEvents.BeforeExpand.Success = "body.rendered=true;";
            }
            else
            {
                rowExpander.DirectEvents.BeforeExpand.Success = String.Format(
                    "body.rendered=true; var gp = Ext.getCmp('{0}'); if (gp) {{ gp.setHeight(gp.getView().mainHd.getHeight() + gp.getView().mainBody.getHeight()); }}",
                    gridId);
            }

            rowExpander.DirectEvents.BeforeExpand.EventMask.ShowMask = true;
            rowExpander.DirectEvents.BeforeExpand.EventMask.Target = MaskTarget.CustomTarget;
            rowExpander.DirectEvents.BeforeExpand.EventMask.CustomTarget = GridId;
            rowExpander.DirectEvents.BeforeExpand.ExtraParams.Add(new Parameter("id", "record.id", ParameterMode.Raw));
            rowExpander.DirectEvents.BeforeExpand.ExtraParams.Add(new Parameter("markId", "record.get('RefMarksOMSU')", ParameterMode.Raw));
            rowExpander.DirectEvents.BeforeExpand.ExtraParams.Add(new Parameter("regionId", "record.get('RefRegions')", ParameterMode.Raw));
            rowExpander.DirectEvents.BeforeExpand.ExtraParams.Add(new Parameter("showHierarhy", "showHierarhy.pressed", ParameterMode.Raw));
            rowExpander.DirectEvents.BeforeExpand.ExtraParams.Add(new Parameter("calculatePreviosResult", "calculatePreviousResultFilter.pressed", ParameterMode.Raw));

            return rowExpander;
        }
    }
}
