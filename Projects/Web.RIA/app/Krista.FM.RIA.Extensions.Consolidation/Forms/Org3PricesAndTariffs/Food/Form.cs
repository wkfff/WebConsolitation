using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs.Food
{
    public class Form : IReportForm
    {
        public const string FormClassName = "PricesAndTariffsFood";

        private readonly ILinqRepository<D_Org_Report> reportRepository;

        public Form(ILinqRepository<D_Org_Report> reportRepository)
        {
            this.reportRepository = reportRepository;
        }

        /// <summary>
        /// Идентификатор формы.
        /// </summary>
        public string ID
        {
            get { return FormClassName; }
        }

        public static void Initialize(IUnityContainer container)
        {
            container.RegisterType<IFactService, FactService>();
            container.RegisterType<IExportService, ExportService>("{0}ExportService".FormatWith(GoodType.Food));
        }

        /// <summary>
        /// Метод валидации формы.
        /// </summary>
        /// <param name="taskId">ID задачи связанной с отчетом.</param>
        /// <returns>true - ошибок нет, false - есть ошибки.</returns>
        public bool Validate(int taskId)
        {
            return true;
        }

        public void CreateReport(D_CD_Task task)
        {
            var report = new D_Org_Report { RefTask = task };
            reportRepository.Save(report);
        }

        public bool HasPampers(D_CD_Task task)
        {
            return true;
        }

        public void Pump(D_CD_Task task, PamperActionsEnum actions)
        {
            var report = reportRepository.FindAll().FirstOrDefault(x => x.RefTask == task);
            var pumper = Core.Resolver.Get<Pumper>();
            pumper.Pump(report, actions);
        }
    }
}
