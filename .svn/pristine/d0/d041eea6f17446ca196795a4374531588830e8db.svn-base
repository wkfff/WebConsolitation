using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers;
using Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers.FO47Dkz;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DomainGenerator;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm
{
    public class ReportForm : IReportForm
    {
        private readonly ILinqRepository<D_CD_Report> reportRepository;
        private readonly IReportDataRepository dataRepository;
        private readonly IDomainTypesResolver typeResolver;

        public ReportForm(ILinqRepository<D_CD_Report> reportRepository, IReportDataRepository dataRepository, IDomainTypesResolver typeResolver)
        {
            this.reportRepository = reportRepository;
            this.dataRepository = dataRepository;
            this.typeResolver = typeResolver;
        }

        /// <summary>
        /// Идентификатор формы.
        /// </summary>
        public string ID
        {
            get { return "ConsForm"; }
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
            var report = new D_CD_Report { RefTask = task, RefForm = task.RefTemplate };

            foreach (var formSection in task.RefTemplate.Parts)
            {
                var section = new D_Report_Section { RefReport = report, RefFormSection = formSection };
                report.Sections.Add(section);

                var cells = formSection.Cells.ToList();
                var recType = typeResolver.GetRecordType(formSection);
                foreach (var formSectionRow in formSection.Rows)
                {
                    InsertRow(section, formSectionRow, recType, cells.Where(x => x.RefRow == formSectionRow).ToList());
                }
            }

            reportRepository.Save(report);

            CreateReportRequisites(report, null, RequisiteKinds.Header);
            CreateReportRequisites(report, null, RequisiteKinds.Footer);
            CreateReportSections(report);
        }

        /// <summary>
        /// Возвращает true, если для отчета есть процедура переноса данных в таблицы фактов.
        /// </summary>
        public bool HasPampers(D_CD_Task task)
        {
            return GetPumper(GetReport(task)) != null;
        }

        public void Pump(D_CD_Task task, PamperActionsEnum actions)
        {
            var report = GetReport(task);
            var pumper = GetPumper(report);

            if (pumper != null)
            {
                pumper.Pump(report, actions);
            }
        }

        private Pumper GetPumper(D_CD_Report report)
        {
            Pumper pumper = null;
            switch (report.RefForm.InternalName)
            {
                case "DKz_GRBS01":
                case "DKz_GRBS02":
                case "DKz_GRBS03":
                case "DKz_GRBS04":
                case "DKz_GRBS05":
                case "DKz_GRBS06":
                case "DKz_GRBS07":
                case "DKz_GRBS08":
                case "DKz_GRBS09":
                case "DKz_GRBS10":
                case "DKz_GRBS11":
                case "DKz_GRBS12":
                case "DKz_GRBS13":
                case "DKz_GRBS14":
                    pumper = Core.Resolver.Get<FO47DkzPumper>();
                    break;
            }

            return pumper;
        }

        private void InsertRow(D_Report_Section section, D_Form_TableRow formSectionRow, Type recType, List<D_Form_TableCell> cells)
        {
            var rec = dataRepository.Create(recType);

            foreach (var column in section.RefFormSection.Columns)
            {
                var cell = cells.FirstOrDefault(x => x.RefColumn == column);
                if (cell != null && cell.DefaultValue != null)
                {
                    rec.Set(column.InternalName, cell.DefaultValue);
                }
            }

            ((D_Report_Row)rec.Value).RefSection = section;
            ((D_Report_Row)rec.Value).RefFormRow = formSectionRow;
            section.Rows.Add((D_Report_Row)rec.Value);
        }

        private void CreateReportRequisites(D_CD_Report report, D_Report_Section section, RequisiteKinds requisiteKind)
        {
            IList<D_Form_Requisites> requisiteses = section == null 
                ? report.RefForm.Requisites.Where(x => x.IsHeader == (requisiteKind == RequisiteKinds.Header)).ToList()
                : section.RefFormSection.Requisites.Where(x => x.IsHeader == (requisiteKind == RequisiteKinds.Header)).ToList();

            if (requisiteses.Count == 0)
            {
                return;
            }

            Type recType = typeResolver.GetRequisiteType(
                report.RefForm, 
                section == null ? null : section.RefFormSection, 
                requisiteKind);

            var rec = dataRepository.Create(recType);
            rec.Set("ID", report.ID);
            foreach (var requisite in requisiteses)
            {
                if (requisite.InternalName == "Subject")
                {
                    rec.Set(requisite.InternalName, report.RefTask.RefSubject.Name);
                }
                else if (requisite.InternalName == "ParentSubj")
                {
                    var parentSubj = report.RefTask.RefSubject.ParentID == null
                        ? "Нет родителя"
                        : report.RefTask.RefSubject.ParentID.Name;
                    rec.Set(requisite.InternalName, parentSubj);
                }
                else if (requisite.InternalName == "BeginDate")
                {
                    rec.Set(requisite.InternalName, report.RefTask.BeginDate);
                }
                else if (requisite.InternalName == "Year")
                {
                    rec.Set(requisite.InternalName, report.RefTask.RefYear.ID);
                }
                else if (requisite.InternalName == "EndDate")
                {
                    rec.Set(requisite.InternalName, report.RefTask.EndDate);
                }
                else if (requisite.InternalName == "Month")
                {
                    rec.Set(requisite.InternalName, report.RefTask.EndDate.AddDays(-1).ToString("MMMM", new CultureInfo("ru-RU")));
                }
                else if (requisite.InternalName == "Quarter")
                {
                    rec.Set(requisite.InternalName, report.RefTask.EndDate.AddDays(-1).Quarter());
                }
                else if (requisite.InternalName == "QuarterName")
                {
                    rec.Set(requisite.InternalName, "Квартал " + report.RefTask.EndDate.AddDays(-1).Quarter());
                }
                else
                {
                    rec.Set(requisite.InternalName, null);
                }
            }

            dataRepository.Save(rec);
        }

        private void CreateReportSections(D_CD_Report report)
        {
            foreach (var section in report.Sections)
            {
                CreateReportRequisites(report, section, RequisiteKinds.Header);
                CreateReportRequisites(report, section, RequisiteKinds.Footer);
            }
        }

        private D_CD_Report GetReport(D_CD_Task task)
        {
            var report = reportRepository.FindAll().FirstOrDefault(x => x.RefTask == task);
            return report;
        }
    }
}
