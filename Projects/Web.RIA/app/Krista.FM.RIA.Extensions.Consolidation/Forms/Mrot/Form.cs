using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public class Form : IReportForm
    {
        private readonly ILinqRepository<D_Report_TrihedrAgr> reportRepository;

        public Form(ILinqRepository<D_Report_TrihedrAgr> reportRepository)
        {
            this.reportRepository = reportRepository;
        }

        /// <summary>
        /// Идентификатор формы.
        /// </summary>
        public string ID
        {
            get { return "ConsFormMrot"; }
        }

        /// <summary>
        /// Метод валидации формы.
        /// </summary>
        /// <param name="taskId">ID задачи связанной с отчетом.</param>
        /// <returns>true - ошибок нет, false - есть ошибки.</returns>
        public bool Validate(int taskId)
        {
            var report = reportRepository.FindAll()
                .Where(x => x.RefTask.ID == taskId).FirstOrDefault();

            return report != null;
        }

        public void CreateReport(D_CD_Task task)
        {
        }

        public bool HasPampers(D_CD_Task task)
        {
            return false;
        }

        public void Pump(D_CD_Task task, PamperActionsEnum actions)
        {
        }
    }
}
