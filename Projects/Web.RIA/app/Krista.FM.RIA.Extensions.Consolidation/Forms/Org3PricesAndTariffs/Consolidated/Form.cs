using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers;
using Microsoft.Practices.Unity;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs.Consolidated
{
    public class Form : IReportForm
    {
        /// <summary>
        /// Идентификатор формы.
        /// </summary>
        public string ID
        {
            get { return "PricesAndTariffsConsolidated"; }
        }

        public static void Initialize(IUnityContainer container)
        {
            container.RegisterType<IExportService, ExportService>();
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
