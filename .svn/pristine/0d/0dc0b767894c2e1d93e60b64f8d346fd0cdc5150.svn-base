using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Common.Consolidation.Forms.Layout;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.Helpers;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Core.TemplateViewEngine;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ExportReports;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Metadata;

using Newtonsoft.Json;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ConsReportController : SchemeBoundController
    {
        private readonly IReportSectionDataService dataService;
        private readonly IReportRequisiteDataService requisiteDataService;
        private readonly ISectionRequisiteDataService sectionRequisiteDataService;
        private readonly ILinqRepository<D_CD_Report> reportRepository;
        private readonly IExportReportService reportService;
        private readonly FormLayoutMarkupService formLayoutMarkupService;

        public ConsReportController(
            IReportSectionDataService dataService, 
            IReportRequisiteDataService requisiteDataService, 
            ISectionRequisiteDataService sectionRequisiteDataService, 
            ILinqRepository<D_CD_Report> reportRepository,
            IExportReportService reportService,
            FormLayoutMarkupService formLayoutMarkupService)
        {
            this.dataService = dataService;
            this.requisiteDataService = requisiteDataService;
            this.sectionRequisiteDataService = sectionRequisiteDataService;
            this.reportRepository = reportRepository;
            this.reportService = reportService;
            this.formLayoutMarkupService = formLayoutMarkupService;
        }

        public ActionResult ConsForm(int taskId)
        {
            var report = reportRepository.FindAll().First(x => x.RefTask.ID == taskId);

            ViewData.Add("reportId", report.ID);
            return new TemplateViewResult("/App_Resource/Krista.FM.RIA.Extensions.Consolidation.dll/Krista.FM.RIA.Extensions.Consolidation.Presentation.Views.ConsReport.View.html");
        }

        public ActionResult ConsFormMrot(int taskId)
        {
            return Redirect("/View/ConsFormMrot?taskId=" + taskId);
        }

        public ActionResult ConsFormMrotSummary(int taskId)
        {
            return Redirect("/View/ConsFormMrotSummary?taskId=" + taskId);
        }

        public ActionResult PricesAndTariffsGasoline(int taskId)
        {
            return Redirect("/View/PricesAndTariffsGasoline?taskId=" + taskId);
        }

        public ActionResult PricesAndTariffsFood(int taskId)
        {
            return Redirect("/View/PricesAndTariffsFood?taskId=" + taskId);
        }

        public ActionResult PricesAndTariffsConsolidated(int taskId)
        {
            return Redirect("/View/PricesAndTariffsConsolidated?taskId=" + taskId);
        }

        /// <summary>
        /// Возвращает метаданные формы.
        /// { 
        ///     Form: - структура формы,
        ///     Mapping: - мапинг структуры на шаблон формы,
        ///     FormLayout: - разметка шаблона формы
        /// }
        /// </summary>
        /// <param name="reportId">Id отчета.</param>
        public ActionResult GetReportMetadata(int reportId)
        {
            var report = reportRepository.FindOne(reportId);
            var form = report.RefForm;
            var mapping = report.RefForm.GetFormMarkupMappings();

            // Форма
            StringWriter stringWriter = new StringWriter();
            stringWriter.Write("{Form: ");
            var serializer = new JsonSerializer
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new NHibernateJsonContractResolver()
            };
            serializer.Serialize(new JsonTextWriter(stringWriter), form);

            // Маппинг формы
            stringWriter.Write(", Mapping: ");
            var ser = new DataContractJsonSerializer(mapping.GetType(), null, Int32.MaxValue, true, new HibernateDataContractSurrogate(), false);
            var metadataStream = new MemoryStream();
            ser.WriteObject(metadataStream, mapping);
            metadataStream.Position = 0;
            var sr = new StreamReader(metadataStream);
            stringWriter.Write(sr.ReadToEnd());
            metadataStream.Close();
            sr.Close();

            // Разметка формы
            stringWriter.Write(", FormLayout: [");
            foreach (var sheet in mapping.Sheets)
            {
                stringWriter.Write("{Region:'");
                stringWriter.Write(sheet.Region);
                stringWriter.Write("',Code:'");
                stringWriter.Write(sheet.Code);
                stringWriter.Write("',Name:'");
                stringWriter.Write(sheet.Name);
                stringWriter.Write("'");

                // Реквизиты формы
                if (sheet.HeaderRequisites != null && sheet.HeaderRequisites.Requisites.Count > 0)
                {
                    var rm = formLayoutMarkupService.GetFormRequisite(form, sheet, RequisiteKinds.Header);
                    stringWriter.Write(",HeaderReq:");
                    stringWriter.Write(rm.Layout.ToScript(null));
                }

                // Разделы формы
                if (sheet.Sections.Count > 0)
                {
                    stringWriter.Write(",Sections:[");
                    foreach (var section in sheet.Sections)
                    {
                        stringWriter.Write("{");

                        // Реквизиты раздела заголовочные
                        if (section.HeaderRequisites != null && section.HeaderRequisites.Requisites.Count > 0)
                        {
                            var rm = formLayoutMarkupService.GetSectionRequisite(form, sheet, section.Code, RequisiteKinds.Header);
                            stringWriter.Write("HeaderReq:");
                            stringWriter.Write(rm.Layout.ToScript(null));
                            stringWriter.Write(",");
                        }

                        // Таблица раздела
                        var tm = formLayoutMarkupService.GetSectionTable(form, sheet, section.Code);
                        stringWriter.Write("Section:");
                        stringWriter.Write(tm.Layout.ToScript(null));

                        // Реквизиты раздела заключительныеы
                        if (section.FooterRequisites != null && section.FooterRequisites.Requisites.Count > 0)
                        {
                            var rm = formLayoutMarkupService.GetSectionRequisite(form, sheet, section.Code, RequisiteKinds.Footer);
                            stringWriter.Write(",FooterReq:");
                            stringWriter.Write(rm.Layout.ToScript(null));
                        }

                        stringWriter.Write("},");
                    }

                    stringWriter.Write("]");
                }

                // Реквизиты формы
                if (sheet.FooterRequisites != null && sheet.FooterRequisites.Requisites.Count > 0)
                {
                    var rm = formLayoutMarkupService.GetFormRequisite(form, sheet, RequisiteKinds.Footer);
                    stringWriter.Write(",FooterReq:");
                    stringWriter.Write(rm.Layout.ToScript(null));
                }

                stringWriter.Write("},");
            }

            stringWriter.Write("]");

            stringWriter.Write("}");

            HttpResponseBase response = ControllerContext.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(stringWriter.ToString());

            return new EmptyResult();
        }

        public ActionResult Report(int reportId)
        {
            using (new ServerContext())
            {
                var report = reportRepository.FindOne(reportId);
                Stream stream = reportService.Export(report);
                return File(stream, "application/vnd.ms-excel", "{0}.xls".FormatWith("report"));
            }
        }

        public ActionResult GetHeaderRequisites(int reportId)
        {
            var report = reportRepository.FindOne(reportId);
            var data = requisiteDataService.Get(report, report.RefForm.Requisites.Where(x => x.IsHeader), RequisiteKinds.Header);
            return new AjaxStoreResult(data, data.Rows.Count);
        }

        public ActionResult GetFooterRequisites(int reportId)
        {
            var report = reportRepository.FindOne(reportId);
            var data = requisiteDataService.Get(report, report.RefForm.Requisites.Where(x => !x.IsHeader), RequisiteKinds.Footer);
            return new AjaxStoreResult(data, data.Rows.Count);
        }

        public ActionResult GetSectionHeaderRequisites(int reportId, string sectionCode)
        {
            var report = reportRepository.FindOne(reportId);
            var section = report.Sections.First(x => x.RefFormSection.Code == sectionCode);
            var data = sectionRequisiteDataService.Get(report, section, RequisiteKinds.Header);

            return new AjaxStoreResult(data, data.Rows.Count);
        }

        public ActionResult GetSectionFooterRequisites(int reportId, string sectionCode)
        {
            var report = reportRepository.FindOne(reportId);
            var section = report.Sections.First(x => x.RefFormSection.Code == sectionCode);
            var data = sectionRequisiteDataService.Get(report, section, RequisiteKinds.Footer);

            return new AjaxStoreResult(data, data.Rows.Count);
        }

        public ActionResult GetSectionData(int reportId, string sectionCode)
        {
            var report = reportRepository.FindOne(reportId);
            var data = from d in dataService.GetAll(report, sectionCode)
                       select d.Value;

            // TODO: Вынести следующий код в специальный репозиторий
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            foreach (var item in data)
            {
                var properties = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (var propertyInfo in properties)
                {
                    // Пропускаем свойство ID родотельского класса
                    // (для версии .net 3.5.30729.5420 (SP1 + fix) это условие не делать, т.к. 
                    // метод GetProperties не возвращает перекрытые в дочерних классах свойства)
                    if (propertyInfo.DeclaringType == typeof(DomainObject))
                    {
                        continue;
                    }

                    object propValue = propertyInfo.GetValue(item, BindingFlags.GetProperty, null, null, null);
                    if (propValue is DomainObject)
                    {
                        propValue = ((DomainObject)propValue).ID;
                    }

                    row.Add(propertyInfo.Name, propValue);
                }

                row.Add("MetaRowMult", ((D_Report_Row)item).RefFormRow.Multiplicity);
                row.Add("MetaRowOrd", ((D_Report_Row)item).RefFormRow.Ord);

                result.Add(row);
            }

            return new AjaxStoreResult(result, result.Count());
        }

        [Transaction(RollbackOnModelStateError = true)]
        public ActionResult SaveSectionData(int reportId, string sectionCode, string data)
        {
            var report = reportRepository.FindOne(reportId);
            var formSection = report.RefForm.Parts.FirstOrDefault(x => x.Code == sectionCode);
            if (formSection == null)
            {
                throw new ReportDataAccessException("Код раздела \"{0}\" не найден.".FormatWith(sectionCode));
            }

            try
            {
                dataService.Save(report, formSection, JSON.Deserialize<JsonObject>(data));
            }
            catch (Exception e)
            {
                var errorWnd = new ErrorWindow
                {
                    Title = "Ошибка во время сохранения",
                    Text = e.ExpandException(new HtmlExceptionFormatter())
                };
                ModelState.AddModelError("key", e);
                return new AjaxResult(errorWnd.Build(new ViewPage())[0].ToScript());
            }
                
            return new AjaxStoreResult(StoreResponseFormat.Save);
        }

        [Transaction]
        public ActionResult SaveHeaderRequisites(int reportId, string data)
        {
            var report = reportRepository.FindOne(reportId);
            requisiteDataService.Save(report, report.RefForm.Requisites.Where(x => x.IsHeader).ToList(), RequisiteKinds.Header, JSON.Deserialize<JsonObject>(data));
            return new AjaxStoreResult(StoreResponseFormat.Save);
        }

        [Transaction]
        public ActionResult SaveFooterRequisites(int reportId, string data)
        {
            var report = reportRepository.FindOne(reportId);
            requisiteDataService.Save(report, report.RefForm.Requisites.Where(x => !x.IsHeader).ToList(), RequisiteKinds.Footer, JSON.Deserialize<JsonObject>(data));
            return new AjaxStoreResult(StoreResponseFormat.Save);
        }

        [Transaction]
        public ActionResult SaveSectionHeaderRequisites(int reportId, string sectionCode, string data)
        {
            var report = reportRepository.FindOne(reportId);
            var section = report.Sections.FirstOrDefault(x => x.RefFormSection.Code == sectionCode);
            if (section == null)
            {
                throw new ReportDataAccessException("Код раздела \"{0}\" не найден.".FormatWith(sectionCode));
            }

            sectionRequisiteDataService.Save(report, section, RequisiteKinds.Header, JSON.Deserialize<JsonObject>(data));
            return new AjaxStoreResult(StoreResponseFormat.Save);
        }

        [Transaction]
        public ActionResult SaveSectionFooterRequisites(int reportId, string sectionCode, string data)
        {
            var report = reportRepository.FindOne(reportId);
            var section = report.Sections.FirstOrDefault(x => x.RefFormSection.Code == sectionCode);
            if (section == null)
            {
                throw new ReportDataAccessException("Код раздела \"{0}\" не найден.".FormatWith(sectionCode));
            }

            sectionRequisiteDataService.Save(report, section, RequisiteKinds.Footer, JSON.Deserialize<JsonObject>(data));
            return new AjaxStoreResult(StoreResponseFormat.Save);
        }
    }
}
