using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Krista.FM.Common.Templates;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.Domain.Services.FinSourceDebtorBook.Reports;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.RIA.Extensions.DebtBook.Presentation.Controllers
{
    public class BebtBookReportsController : SchemeBoundController
    {
        private readonly IDebtBookExtension extension;
        private const string ShowParamsView = "~/App_Resource/Krista.FM.RIA.Extensions.DebtBook.dll/Krista.FM.RIA.Extensions.DebtBook/Presentation/Views/DebtBookReports/ShowParams.aspx";

        public BebtBookReportsController(IDebtBookExtension extension)
        {
            this.extension = extension;
        }

        public ActionResult ShowParams()
        {
            return View(ShowParamsView, extension);
        }

        public ActionResult Create(string reportType, string userId, string userRegion)
        {
            int userRegionId;
            
            // Обработка параметров отчета
            // Район
            userRegionId = userRegion.IsNotNullOrEmpty() ? Convert.ToInt32(userRegion) : extension.UserRegionId;

            // Подпись отчета
            var repository = new NHibernateLinqRepository<D_S_TitleReport>();
            D_S_TitleReport titleReport = userId.IsNotNullOrEmpty() 
                ? repository.Get(Convert.ToInt32(userId))
                : null;
            if (titleReport == null)
            {
                titleReport = repository.FindAll()
                    .Where(x => x.RefRegion.ID == userRegionId)
                    .FirstOrDefault();

                if (titleReport == null)
                {
                    titleReport = repository.FindAll().FirstOrDefault();
                }
            }

            // Создаем экземрляр класса отчета
            Type reportClassType = Type.GetType(reportType);
            Report report = (Report)Resolver.Get(reportClassType);

            // Получаем шаблон отчета и сохраняем на диск во временный коталог
            string templateDocumentName;
            string downloadFilename;
            using (new ServerContext())
            {
                report.ReportDate = extension.Variant.ReportDate;                
                DataTable dtblTemplates = Scheme.TemplatesService.Repository.GetTemplatesInfo(TemplateTypes.System);

                if (dtblTemplates.Select("Code = '{0}'".FormatWith(report.TemplateName)).Length == 0)
                {
                    throw new ApplicationException("Не найден шаблон отчета {0}".FormatWith(report.TemplateName));
                }

                DataRow templateRow = dtblTemplates.Select("Code = '{0}'".FormatWith(report.TemplateName))[0];
                string reportFullName = templateRow["DocumentFileName"].ToString();
                int templateId = Convert.ToInt32(templateRow["ID"]);

                TemplatesDocumentsHelper documentsHelper =
                    new TemplatesDocumentsHelper(Scheme.TemplatesService.Repository);
                templateDocumentName = documentsHelper.SaveDocument(
                    templateId, 
                    Convert.ToString(templateRow["Name"]), 
                    reportFullName);

                downloadFilename = FileHelper.GetDownloadableFileName(Convert.ToString(templateRow["Name"]) + ".xls");

                // Создаем отчет
                report.Create(
                    templateDocumentName,
                    extension.Variant.Id,
                    userRegionId,
                    extension.Variant.ReportDate,
                    titleReport);
            }

            return File(templateDocumentName, "application/vnd.ms-excel", downloadFilename);
        }

        public ActionResult MinfinExport(DateTime exportDate)
        {
            string fileName = String.Format("{0}690r0{1}.txt", Scheme.GlobalConstsManager.Consts["RegionMFRF"].Value.ToString().Substring(0, 2), ((int)exportDate.DayOfWeek));

            fileName = Path.Combine(TemplatesDocumentsHelper.GetDocsFolder(), fileName);

            MinfinUnloadingService minfinService = new MinfinUnloadingService(Scheme);
            
            using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                {
                    writer.Write(minfinService.GetMinfinData(exportDate));
                }
            }

            ////Arj32Helper.ArchiveFile(fileName);

            return File(fileName, "text/plain", FileHelper.GetDownloadableFileName(Path.GetFileName(fileName)));
        }
    }
}
