using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;
using Krista.FM.RIA.Extensions.MarksOMSU.Services.Exceptions;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controllers
{
    public class MarksOmsuReportController : SchemeBoundController
    {
        private readonly IMarksOmsuExtension extension;
        private readonly IMarksOmsuRepository repository;
        private readonly IRegionsRepository regionRepository;
        private readonly IMarksRepository marksRepository;
        private readonly IRepository<FX_OMSU_StatusData> statusRepository;
        private readonly IReportService reportService;
        private readonly MarksOmsuGridControl gridControl;

        public MarksOmsuReportController(
            IMarksOmsuExtension extension, 
            IMarksOmsuRepository repository,
            IRegionsRepository regionRepository,
            IMarksRepository marksRepository,
            IRepository<FX_OMSU_StatusData> statusRepository,
            IReportService reportService,
            MarksOmsuGridControl gridControl)
        {
            this.extension = extension;
            this.repository = repository;
            this.regionRepository = regionRepository;
            this.marksRepository = marksRepository;
            this.statusRepository = statusRepository;
            this.reportService = reportService;
            this.gridControl = gridControl;

            gridControl.StoreController = "MarksOmsuReport";
        }

        public ActionResult GetRegionList()
        {
            if (extension.DataSourceRegion == null)
            {
                var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                {
                    Title = "Ошибка инициализации.",
                    Html = "Не найден источник данных у Регионы.Анализ для выбранного года!",
                    HideDelay = 2000
                });
                return new AjaxStoreExtraResult(new object(), 0, message.ToScript());
            }

            var data = regionRepository.FindAll()
                                       .Where(x => (x.RefTerr.ID == (int)TerrytoryType.MR || x.RefTerr.ID == (int)TerrytoryType.GO))
                                       .Select(x => new
                                                        {
                                                            x.ID, 
                                                            x.Name,
                                                            RegionCodeLine = x.CodeLine
                                                        })
                                       .ToList();
            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult Load(int? regionId, int? markId, bool[] filter, bool showHierarhy, bool? calculatePreviosResult)
        {
            if (regionId == null)
            {
                return new AjaxStoreResult(new object(), 0);
            }

            int parentMarkId = markId == null ? -1 : (int)markId;

            List<int> filters = new List<int>();
            for (int index = 0; index < 4; index++)
            {
                if (!filter[index])
                {
                    filters.Add(index + 1);
                }
            }

            var data = showHierarhy
                           ? repository.GetForMO(regionRepository.FindOne((int)regionId), parentMarkId)
                           : repository.GetAllMarksForMO(regionRepository.FindOne((int)regionId));
            
            IList<F_OMSU_Reg16> previousFacts = new List<F_OMSU_Reg16>();
            if (calculatePreviosResult == true)
            {
                previousFacts = showHierarhy
                           ? repository.GetForMOPrevious(regionRepository.FindOne((int)regionId), parentMarkId)
                           : repository.GetAllMarksForMOPrevious(regionRepository.FindOne((int)regionId));
            }

            var view = from f in data
                       where !filters.Contains(f.RefStatusData.ID)
                       select new
                       {
                           f.ID,
                           CompRep = f.RefMarksOMSU.RefCompRep.Name,
                           MarkName = f.RefMarksOMSU.Name,
                           OKEI = f.RefMarksOMSU.RefOKEI.Symbol,
                           f.RefMarksOMSU.CodeRepDouble,
                           f.RefMarksOMSU.Formula,
                           f.RefMarksOMSU.Grouping,
                           Status = f.RefStatusData.Name,
                           f.RefMarksOMSU.MO,
                           PriorYearCurrentValue = calculatePreviosResult == true ?
                                                   (from p in previousFacts
                                                    where p.RefMarksOMSU.RefMarksB == f.RefMarksOMSU.RefMarksB
                                                          && p.RefRegions.RefBridgeRegions == f.RefRegions.RefBridgeRegions
                                                    select p.CurrentValue)
                                                    .FirstOrDefault()
                                                   : null,
                           f.PriorValue,
                           f.CurrentValue,
                           f.Prognoz1,
                           f.Prognoz2,
                           f.Prognoz3,
                           f.Note,
                           RefMarksOMSU = f.RefMarksOMSU.ID,
                           RefRegions = f.RefRegions.ID,
                           RefStatusData = f.RefStatusData.ID,
                           RefYearDayUNV = f.RefYearDayUNV.ID,
                           f.RefMarksOMSU.Capacity,
                           ReadonlyCurrent = !(String.IsNullOrEmpty(f.RefMarksOMSU.Formula) && f.RefMarksOMSU.MO && !f.RefMarksOMSU.Grouping),
                           ReadonlyPrognoz = !(f.RefMarksOMSU.PrognMO && !f.RefMarksOMSU.Grouping)
                       };
            
            return new AjaxStoreResult(view, view.Count());
        }

        [Transaction]
        public ActionResult Save(object data)
        {
            var ss = JavaScriptDomainConverter<F_OMSU_Reg16>
                .Deserialize(Convert.ToString(((string[])data)[0]));

            foreach (F_OMSU_Reg16 omsuReg16 in ss.Updated)
            {
                omsuReg16.SourceID = extension.DataSourceOmsu.ID;
                omsuReg16.RefMarksOMSU = marksRepository.FindOne(omsuReg16.RefMarksOMSU.ID);
                omsuReg16.RefRegions = regionRepository.FindOne(omsuReg16.RefRegions.ID); 
                omsuReg16.RefStatusData = statusRepository.Get(omsuReg16.RefStatusData.ID);
                omsuReg16.RefYearDayUNV = extension.CurrentYearUNV;

                repository.Save(omsuReg16);
            }

            repository.DbContext.CommitChanges();

            return new AjaxStoreResult(StoreResponseFormat.Save);
        }

        [Transaction]
        public ActionResult AcceptReport(int regionId)
        {
            try
            {
                reportService.Accept(regionId);
            }
            catch (CannotAcceptReportException e)
            {
                return new AjaxResult { ErrorMessage = e.Message };
            }

            return new AjaxResult();
        }

        [Transaction]
        public ActionResult RejectReport(int regionId)
        {
            reportService.Reject(regionId);

            return new AjaxResult();
        }

        public ActionResult Expand(int id, int markId, int regionId, bool showHierarhy)
        {
            var result = new AjaxResult();

            var facts = repository.GetForMO(regionRepository.FindOne(regionId), markId);

            result.Script = facts.Count > 0 && showHierarhy
                ? CreateGridScript(id, regionId, markId)
                : CreateDescriptionPanelScript(markId, id);

            return result;
        }

        private string CreateDescriptionPanelScript(int markId, int id)
        {
            var mark = marksRepository.FindOne(markId);

            Label label = new Label();
            label.Html = String.Format(
                @"<div class=""x-window-mc"">
<p><b>Что показывает:</b> {0} </p>
<p><b>Расчет:</b> {1} </p>
<p><b>Информационная обеспеченность:</b> {2} </p>
<p><b>Ответственный (в том числе по согласованию):</b> {3} </p>
<p><b>Обозначение:</b> {4} </p>
<p><b>Формула:</b> {5} </p>
</div>",
                mark.Description,
                mark.CalcMark,
                mark.InfoSource,
                mark.RefResponsOIV.ID == -1 ? "ОМСУ" : mark.RefResponsOIV.Name,
                mark.Symbol,
                mark.Formula);

            return label.ToScript(RenderMode.RenderTo, "row-" + id);
        }

        private string CreateGridScript(int id, int regionId, int markId)
        {
            StringBuilder sb = new StringBuilder();

            var gridId = "GridPanelRow_" + id;

            Store store = gridControl.CreateStore("StoreRow_" + id);
            store.GroupField = String.Empty;
            store.BaseParams.Add(new Parameter("regionId", Convert.ToString(regionId)));
            store.BaseParams.Add(new Parameter("markId", Convert.ToString(markId)));
            store.BaseParams.Add(new Parameter("filter", "getStateFilter()", ParameterMode.Raw));
            store.BaseParams.Add(new Parameter("showHierarhy", "showHierarhy.pressed", ParameterMode.Raw));

            store.Listeners.Load.Handler = String.Format(
                "var gp = Ext.getCmp('{0}'); if (gp) {{ gp.setHeight(gp.getView().mainHd.getHeight() + gp.getView().mainBody.getHeight()); }}",
                gridId);

            sb.AppendLine(store.ToScript(RenderMode.Auto, "row-" + id));

            GridPanel grid = gridControl.CreateGridPanel(gridId, "{raw}StoreRow_" + id, null);
            grid.Height = 200;

            grid.ColumnModel.ID = "GridPanelRowCM_" + id;

            grid.View[0].ID = "GridPanelRowView_" + id;
            grid.View[0].ForceFit = true;
            grid.View[0].HeaderGroupRows.Clear();

            ((RowExpander)grid.Plugins[0]).Listeners.Expand.Handler = String.Format(
                "var gp = Ext.getCmp('{0}'); if (gp) {{ gp.setHeight(gp.getView().mainHd.getHeight() + gp.getView().mainBody.getHeight()); }}",
                gridId);
            ((RowExpander)grid.Plugins[0]).Listeners.Collapse.Handler = String.Format(
                "var gp = Ext.getCmp('{0}'); if (gp) {{ gp.setHeight(gp.getView().mainHd.getHeight() + gp.getView().mainBody.getHeight()); }}",
                gridId);

            // important
            X.Get("row-" + id).SwallowEvent(new string[] { "click", "mousedown", "mouseup", "dblclick" }, true);

            sb.AppendLine(grid.ToScript(RenderMode.RenderTo, "row-" + id));

            var instanceScript = (StringBuilder)System.Web.HttpContext.Current.Items["Ext.Net.ResourceMgr.InstanceScript"];
            sb.AppendLine(instanceScript.ToString());
            instanceScript.Remove(0, instanceScript.Length);

            return sb.ToString();
        }
    }
}
