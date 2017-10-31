using System;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Calculations.Expressions;
using Krista.FM.Common.Consolidation.Calculations.Visitors;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.Consolidation.Data;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;

namespace Krista.FM.RIA.Extensions.Consolidation.Presentation.Controllers
{
    public class ConsRelationsController : SchemeBoundController
    {
        private readonly IRepository<D_CD_Report> reportRepository;
        private readonly ILinqRepository<D_Form_Relations> relationRepository;
        private readonly IReportSectionDataService dataService;
        private readonly ICollectingTaskRepository collectTaskRepository;
        private readonly IProgressManager progressManager;

        public ConsRelationsController(
            IRepository<D_CD_Report> reportRepository, 
            ILinqRepository<D_Form_Relations> relationRepository, 
            IReportSectionDataService dataService,
            ICollectingTaskRepository collectTaskRepository,
            IProgressManager progressManager)
        {
            this.reportRepository = reportRepository;
            this.relationRepository = relationRepository;
            this.dataService = dataService;
            this.collectTaskRepository = collectTaskRepository;
            this.progressManager = progressManager;
        }

        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult CalcReport(int reportId)
        {
            var report = reportRepository.Get(reportId);
            try
            {
                using (new ServerContext())
                {
                    var formParts = report.RefForm.Parts.OrderBy(x => x.Ord);
                    int partCount = formParts.Count();
                    var partIndx = 0.0;
                    foreach (var formPart in formParts)
                    {
                        var relations = formPart.Relations.OrderBy(x => x.Ord).ToList();
                        int relationCount = relations.Count();
                        var relationIndx = 1.0;

                        var primaryDataProvider = new CalculationPrimaryDataProvider(dataService);
                        var dataProvider = new CachedCalculationDataProvider(dataService, collectTaskRepository);
                        foreach (var relation in relations.OrderBy(x => x.Ord))
                        {
                            ConsRelationExpression exprTree;
                            try
                            {
                                exprTree = Expression.Compile(relation);
                            }
                            catch (EvaluationException e)
                            {
                                var msg = "Синтаксическая ошибка \"{0}\" в выражении \"{1}\" раздела \"{2}\".".FormatWith(
                                    e.Message,
                                    relation.LeftPart + relation.RalationType + relation.RightPart,
                                    formPart.Code);
                                throw new EvaluationException(msg, e);
                            }

                            var visitor = new EvaluationVisitor(primaryDataProvider, dataProvider);
                            visitor.SetContext(formPart.Code, report.RefForm.Code, formPart.Columns);
                            primaryDataProvider.SetLeftContext(report, formPart.Code);
                            dataProvider.SetReport(report);
                            dataProvider.SetContext(formPart.Code, report.RefForm.Code, false);
                            
                            try
                            {
                                exprTree.Accept(visitor);
                            }
                            catch (Exception e)
                            {
                                var msg = "Ошибка при вычислении выражения \"{0}\" для раздела \"{1}\".".FormatWith(
                                    relation.LeftPart + relation.RalationType + relation.RightPart,
                                    formPart.Code);
                                throw new EvaluationException(msg, e);
                            }

                            progressManager.SetCompleted((partIndx / partCount) + (relationIndx++ / relationCount / partCount));
                        }

                        primaryDataProvider.Save();

                        partIndx++;
                    }
                }

                reportRepository.DbContext.CommitChanges();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("error", e);
                var msg = new ErrorWindow
                {
                    Title = "Критическая ошибка",
                    Text = e.ExpandException()
                };

                return new AjaxResult(msg.Build(new ViewPage())[0].ToScript());
            }

            return new AjaxResult();
        }

        [Transaction]
        public ActionResult CalcSection(int reportId, string sectionInternalName)
        {
            var report = reportRepository.Get(reportId);
            var formSection = report.RefForm.Parts.FirstOrDefault(x => x.InternalName == sectionInternalName);
            if (formSection == null)
            {
                throw new ReportDataAccessException("Внутреннее имя раздела \"{0}\" не найдено.".FormatWith(sectionInternalName));
            }

            try
            {
                var relations = relationRepository.FindAll().Where(x => x.RefPart == formSection);
                var primaryDataProvider = new CalculationPrimaryDataProvider(dataService);
                var dataProvider = new CalculationDataProvider(dataService, collectTaskRepository);
                foreach (var relation in relations)
                {
                    var exprTree = Expression.Compile(relation);
                    var visitor = new EvaluationVisitor(primaryDataProvider, dataProvider);
                    exprTree.Accept(visitor);
                }

                primaryDataProvider.Save();
            }
            catch (Exception e)
            {
                var msg = new ErrorWindow
                {
                    Title = "Критическая ошибка",
                    Text = e.ExpandException()
                };

                return new AjaxResult(msg.Build(new ViewPage())[0].ToScript());
            }

            return new AjaxResult();
        }
    }
}
