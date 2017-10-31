using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Extensions.MarksOMSU.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controllers
{
    public class IneffExpensesController : SchemeBoundController
    {
        private readonly IIneffExpencesService ineffExpencesService;
        private readonly IMarksOmsuRepository factsRepository;
        private readonly IMarksOmsuExtension marksOmsuExtension;
        private readonly IMarksCalculator marksCalculator;
        private readonly IRepository<FX_OMSU_StatusData> statusRepository;
        private readonly IExportService exportService;

        public IneffExpensesController(
            IIneffExpencesService ineffExpencesService,
            IMarksOmsuRepository factsRepository,
            IMarksOmsuExtension marksOmsuExtension,
            IMarksCalculator marksCalculator,
            IRepository<FX_OMSU_StatusData> statusRepository,
            IExportService exportService)
        {
            this.ineffExpencesService = ineffExpencesService;
            this.factsRepository = factsRepository;
            this.marksOmsuExtension = marksOmsuExtension;
            this.marksCalculator = marksCalculator;
            this.statusRepository = statusRepository;
            this.exportService = exportService;
        }

        #region Target marks

        public ActionResult GetTargetMarkGkh()
        {
            return GetTargetMark(ineffExpencesService.GetMarkIneffGkh());
        }

        public ActionResult GetTargetMarkOmu()
        {
            return GetTargetMark(ineffExpencesService.GetMarkIneffOmu());
        }

        public ActionResult GetTargetMarkEducation()
        {
            return GetTargetMark(ineffExpencesService.GetMarkIneffEducation());
        }

        public ActionResult GetTargetMarkMedicine()
        {
            return GetTargetMark(ineffExpencesService.GetMarkIneffMedicine());
        }
        
        #endregion

        public ActionResult GetTargetFacts(int targetMarkId)
        {
            var viewModel = ineffExpencesService.GetTargetFactsViewModel(targetMarkId, false);
            return new AjaxStoreResult(viewModel, viewModel.Rows.Count);
        }

        public ActionResult GetSourceFacts(int targetMarkId, int regionId)
        {
            var viewModel = new DataTable();
            viewModel.Columns.Add(new DataColumn("ID", typeof(int)));
            viewModel.Columns.Add(new DataColumn("Symbol", typeof(string)));
            viewModel.Columns.Add(new DataColumn("Formula", typeof(string)));
            viewModel.Columns.Add(new DataColumn("Prior", typeof(decimal)));
            viewModel.Columns.Add(new DataColumn("Current", typeof(decimal)));
            viewModel.Columns.Add(new DataColumn("Name", typeof(string)));
            viewModel.Columns.Add(new DataColumn("Level", typeof(int)));
            viewModel.Columns.Add(new DataColumn("MeasureUnit", typeof(string)));
            viewModel.Columns.Add(new DataColumn("Precision", typeof(int)));

            foreach (var factWithLevel in 
                ineffExpencesService.GetSourceFactsWithHierarchy(marksCalculator.GetMarkCalculationPlan(targetMarkId), regionId))
            {
                var fact = factWithLevel.Key;

                var currentRow = viewModel.NewRow();
                currentRow.BeginEdit();

                currentRow.SetField("ID", fact.ID);
                currentRow.SetField("Symbol", fact.RefMarksOMSU.Symbol);
                currentRow.SetField("Formula", (fact.RefMarksOMSU.Formula.IsNullOrEmpty() || fact.RefMarksOMSU.RefTypeMark.ID == (int)TypeMark.Gather) ? "&nbsp;" : HttpUtility.HtmlEncode(fact.RefMarksOMSU.CalcMark));
                currentRow.SetField("Prior", fact.PriorValue ?? 0);
                currentRow.SetField("Current", fact.CurrentValue ?? 0);
                currentRow.SetField("Name", fact.RefMarksOMSU.Name);
                currentRow.SetField("Level", factWithLevel.Value); 
                currentRow.SetField("MeasureUnit", fact.RefMarksOMSU.RefOKEI.Name);
                currentRow.SetField("Precision", fact.RefMarksOMSU.Capacity);

                currentRow.EndEdit();
                viewModel.Rows.Add(currentRow);
            }

            return new AjaxStoreResult(viewModel, viewModel.Rows.Count);
        }

        [Transaction]
        public ActionResult SaveTargetFacts(object data)
        {
            if (!((BasePrincipal)System.Web.HttpContext.Current.User).IsInRole(MarksOMSUConstants.IneffGkhCalculateRole))
            {
                throw new PermissionException(
                    ((BasePrincipal)System.Web.HttpContext.Current.User).Identity.Name,
                    "IneffExpencesController",
                    "Save",
                    "Пользователь не имеет полномочий изменять данные");
            }

            var ss = JavaScriptDomainConverter<F_OMSU_Reg16>
                .Deserialize(Convert.ToString(((string[])data)[0]));

            foreach (F_OMSU_Reg16 factUpdated in ss.Updated)
            {
                // Берем факт из БД, так как у него изменится только статус и расчетные поля.
                var targetFact = factsRepository.Get(factUpdated.ID);
                var wasEditable =
                    targetFact.RefStatusData.ID == (int)OMSUStatus.Undefined
                    || targetFact.RefStatusData.ID == (int)OMSUStatus.OnEdit
                    || targetFact.RefStatusData.ID == (int)OMSUStatus.OnReview;
                var nowProtected =
                    factUpdated.RefStatusData.ID == (int)OMSUStatus.Accepted
                    || factUpdated.RefStatusData.ID == (int)OMSUStatus.Approved;

                if (wasEditable && nowProtected)
                {
                    // Здесь текущее состояние всегда допускает расчет.
                    marksCalculator.Calc(new List<F_OMSU_Reg16> { targetFact }, targetFact.RefRegions.ID, false);
                }
                
                targetFact.RefStatusData = statusRepository.Get(factUpdated.RefStatusData.ID);
                
                /* Обеспечиваем оперативность расчетов,
                   иначе запишется все только на CommitChanges,
                   и некотоыре итерации могут рассчитаться неправильно */
                factsRepository.Save(targetFact);
            }

            factsRepository.DbContext.CommitChanges();

            return new AjaxStoreResult(StoreResponseFormat.Save);
        }

        public ActionResult ExportTargetFactsToXls(int targetMarkId, string itfCaption)
        {
            var stream = exportService.ExportIneffExpenceFacts(targetMarkId, itfCaption);
            return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith(itfCaption));
        }

        private ActionResult GetTargetMark(D_OMSU_MarksOMSU targetMark)
        {
            var viewModel = from mark in new List<D_OMSU_MarksOMSU> { targetMark } 
                            select
                            new
                                {
                                    mark.ID, 
                                    Description = mark.Description.IsNullOrEmpty() ? "(не задано)" : mark.Description,
                                    mark.CalcMark,
                                    Year = marksOmsuExtension.CurrentYear,
                                    MeasureUnit = mark.RefOKEI.Name,
                                    Precision = mark.Capacity,
                                };
            return new AjaxStoreResult(viewModel, viewModel.Count());
        }
    }
}
