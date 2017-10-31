using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;

using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ImportReports.Xml
{
    public class XmlImportReportService : IImportReportService
    {
        private readonly ILinqRepository<D_CD_Task> taskRepository;
        private readonly ILinqRepository<D_CD_Report> reportRepository;
        private readonly IReportSectionDataService sectionDataService;
        private readonly IReportRequisiteDataService reportRequisiteDataService;
        private readonly ISectionRequisiteDataService sectionRequisiteDataService;

        public XmlImportReportService(
            ILinqRepository<D_CD_Task> taskRepository, 
            ILinqRepository<D_CD_Report> reportRepository,
            IReportSectionDataService sectionDataService, 
            IReportRequisiteDataService reportRequisiteDataService, 
            ISectionRequisiteDataService sectionRequisiteDataService)
        {
            this.taskRepository = taskRepository;
            this.reportRepository = reportRepository;
            this.sectionDataService = sectionDataService;
            this.reportRequisiteDataService = reportRequisiteDataService;
            this.sectionRequisiteDataService = sectionRequisiteDataService;
        }

        public void Import(D_CD_CollectTask collectTask, Stream stream)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                var r = new XmlTextReader(stream);

                r.Read();
                r.ReadStartElement("ReportDescriptor");

                var taskDescriptor = ReadTaskDescriptor(r);

                var tasks = taskRepository.FindAll()
                    .Where(x => x.RefSubject.Name == taskDescriptor.Subject
                                && x.RefSubject.RefLevel.Code == taskDescriptor.SubjectLevel
                                && x.RefSubject.RefRole.Name == taskDescriptor.SubjectRole
                                && x.BeginDate == taskDescriptor.BeginDate
                                && x.EndDate == taskDescriptor.EndDate
                                && x.RefTemplate.InternalName == taskDescriptor.Form
                                && x.RefTemplate.FormVersion == taskDescriptor.FormVersion)
                    .ToList();

                if (tasks.Count > 1)
                {
                    throw new ApplicationException("По заданным критериям найдено более одной задачи.");
                }
            
                if (tasks.Count == 0)
                {
                    throw new ApplicationException("По заданным критериям не найдено ни одной задачи.");
                }

                var report = reportRepository.FindAll().FirstOrDefault(x => x.RefTask == tasks[0]);
                if (report == null)
                {
                    throw new ApplicationException("Связанный с задачей отчет не найден.");
                }

                LoadReport(report, r);

                r.ReadEndElement();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private void LoadReport(D_CD_Report report, XmlReader reader)
        {
            reader.ReadStartElement("Report");

            if (report.RefForm.Requisites.Any(x => x.IsHeader))
            {
                LoadReportRequisites(report, RequisiteKinds.Header, reader);
            }

            reader.MoveToAttribute("size");
            int size = Convert.ToInt32(reader.Value);
            reader.MoveToElement();
            reader.ReadStartElement("Sections");

            for (int i = 0; i < size; i++)
            {
                LoadReportSection(report, reader);
            }

            reader.ReadEndElement();

            if (report.RefForm.Requisites.Any(x => !x.IsHeader))
            {
                LoadReportRequisites(report, RequisiteKinds.Footer, reader);
            }

            reader.ReadEndElement();
        }

        private void LoadReportSection(D_CD_Report report, XmlReader reader)
        {
            reader.MoveToAttribute("internalName");
            var formName = reader.Value;
            reader.MoveToElement();
            reader.ReadStartElement("Section");

            var part = report.RefForm.Parts.FirstOrDefault(x => x.InternalName == formName);
            if (part == null)
            {
                throw new ApplicationException("Раздел отчета не найден в метаданных формы.");
            }

            var section = report.Sections.First(x => x.RefFormSection == part);

            if (section.RefFormSection.Requisites.Any(x => x.IsHeader))
            {
                LoadSectionRequisites(section, RequisiteKinds.Header, reader);
            }

            LoadReportSectionRows(report, section, reader);

            if (section.RefFormSection.Requisites.Any(x => !x.IsHeader))
            {
                LoadSectionRequisites(section, RequisiteKinds.Footer, reader);
            }

            reader.ReadEndElement();
        }

        private void LoadReportSectionRows(D_CD_Report report, D_Report_Section section, XmlReader reader)
        {
            var data = sectionDataService.GetAll(report, section.RefFormSection.Code);
            var multiplicityRowsTemplates = sectionDataService.GetMultipliesRowsTemplates(section);

            RowDescriptor emptyRow = new RowDescriptor();
            foreach (var column in section.RefFormSection.Columns.OrderBy(x => x.Ord))
            {
                emptyRow.Items.Add(column.InternalName, String.Empty);
            }

            reader.MoveToAttribute("size");
            int size = Convert.ToInt32(reader.Value);
            reader.MoveToElement();
            reader.ReadStartElement("Rows");

            for (int i = 0; i < size; i++)
            {
                var row = LoadReportSectionRow(emptyRow, reader);

                var dataRow = data.FirstOrDefault(x => ((D_Report_Row)x.Value).RefFormRow.Name == row.Code);
                if (dataRow == null)
                {
                    // TODO: Написать более детальное сообщение
                    throw new ApplicationException("Строка не найдена.");
                }

                if (!dataRow.IsMultiplicity())
                {
                    foreach (var col in row.Items)
                    {
                        // TODO: Convert to specific type
                        dataRow.Set(col.Key, col.Value);
                    }
                }
                else
                {
                    var template = multiplicityRowsTemplates
                        .FirstOrDefault(x => ((D_Report_Row)x.Value).RefFormRow.Name == row.Code);

                    if (template == null)
                    {
                        // TODO: Написать более детальное сообщение
                        throw new ApplicationException("Не найден шаблон для множимой строки.");
                    }

                    var newRow = sectionDataService.CreateRecord(report, section.RefFormSection.Code);
                    newRow.AssignRecord(template.Value);

                    foreach (var col in row.Items)
                    {
                        // TODO: Convert to specific type
                        newRow.Set(col.Key, col.Value);
                    }

                    data.Add(newRow);
                }
            }

            // Удаляем неизмененные множимые строки 
            foreach (var record in data)
            {
                if (record.IsMultiplicity() && record.State == ReportDataRecordState.Unchanged)
                {
                    record.Delete();
                }
            }

            sectionDataService.Save(report, section.RefFormSection, data);

            reader.ReadEndElement();
        }

        private RowDescriptor LoadReportSectionRow(RowDescriptor emptyRow, XmlReader reader)
        {
            var row = (RowDescriptor)emptyRow.Clone();

            reader.MoveToAttribute("code");
            row.Code = reader.Value;
            reader.MoveToElement();
            reader.ReadStartElement("Row");

            foreach (var columnKey in row.Items.Keys.ToList())
            {
                if (reader.IsEmptyElement)
                {
                    reader.ReadStartElement(columnKey);
                    continue;
                }

                reader.ReadStartElement(columnKey);
                row.Items[columnKey] = reader.ReadString();
                reader.ReadEndElement();
            }

            reader.ReadEndElement();

            return row;
        }

        private void LoadReportRequisites(D_CD_Report report, RequisiteKinds requisiteKind, XmlReader reader)
        {
            var requisites = report.RefForm.Requisites
                .Where(x => x.IsHeader == (requisiteKind == RequisiteKinds.Header))
                .OrderBy(x => x.InternalName)
                .ToList();

            var dt = reportRequisiteDataService.Get(report, requisites, requisiteKind);
            var row = dt.Rows[0];

            ReadRequisites(
                requisiteKind == RequisiteKinds.Header ? "HeaderReportRequisites" : "FooterReportRequisites", 
                requisites, 
                row, 
                reader);

            reportRequisiteDataService.Save(report, requisites, requisiteKind, row);
        }

        private void LoadSectionRequisites(D_Report_Section section, RequisiteKinds requisiteKind, XmlReader reader)
        {
            var requisites = section.RefFormSection.Requisites
                .Where(x => x.IsHeader == (requisiteKind == RequisiteKinds.Header))
                .OrderBy(x => x.InternalName)
                .ToList();

            var dt = sectionRequisiteDataService.Get(section.RefReport, section, requisiteKind);
            var row = dt.Rows[0];

            ReadRequisites(
                requisiteKind == RequisiteKinds.Header ? "HeaderSectionRequisites" : "FooterSectionRequisites",
                requisites,
                row,
                reader);

            sectionRequisiteDataService.Save(section.RefReport, section, requisiteKind, row);
        }

        private void ReadRequisites(string requisitesTagName, IEnumerable<D_Form_Requisites> requisites, DataRow row, XmlReader reader)
        {
            reader.ReadStartElement(requisitesTagName);
            foreach (var requisite in requisites)
            {
                if (reader.IsEmptyElement)
                {
                    reader.ReadStartElement(requisite.InternalName.ToUpper());
                    row[requisite.InternalName] = DBNull.Value;
                    continue;
                }

                reader.ReadStartElement(requisite.InternalName.ToUpper());
                if (requisite.DataType == "System.DateTime")
                {
                    var val = reader.ReadString();
                    if (String.IsNullOrEmpty(val))
                    {
                        row[requisite.InternalName] = DBNull.Value;
                    }
                    else
                    {
                        row[requisite.InternalName] = Convert.ToDateTime(val);
                    }
                }
                else if (requisite.DataType == "System.String")
                {
                    var val = reader.ReadString();
                    if (String.IsNullOrEmpty(val))
                    {
                        row[requisite.InternalName] = DBNull.Value;
                    }
                    else
                    {
                        row[requisite.InternalName] = val;
                    }
                }

                reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }

        private TaskDescriptor ReadTaskDescriptor(XmlReader reader)
        {
            var taskDescriptor = new TaskDescriptor();

            reader.ReadStartElement("TaskDescriptor");

            reader.ReadStartElement("Subject");
            taskDescriptor.Subject = reader.ReadString();
            reader.ReadEndElement();

            reader.ReadStartElement("SubjectLevel");
            taskDescriptor.SubjectLevel = reader.ReadString();
            reader.ReadEndElement();

            reader.ReadStartElement("SubjectRole");
            taskDescriptor.SubjectRole = reader.ReadString();
            reader.ReadEndElement();

            reader.ReadStartElement("BeginDate");
            taskDescriptor.BeginDate = Convert.ToDateTime(reader.ReadString());
            reader.ReadEndElement();

            reader.ReadStartElement("EndDate");
            taskDescriptor.EndDate = Convert.ToDateTime(reader.ReadString());
            reader.ReadEndElement();

            reader.ReadStartElement("Form");
            taskDescriptor.Form = reader.ReadString();
            reader.ReadEndElement();

            reader.ReadStartElement("FormVersion");
            taskDescriptor.FormVersion = Convert.ToInt32(reader.ReadString());
            reader.ReadEndElement();

            reader.ReadEndElement();

            return taskDescriptor;
        }

        private class TaskDescriptor
        {
            public string Subject { get; set; }

            public string SubjectLevel { get; set; }
            
            public string SubjectRole { get; set; }

            public DateTime BeginDate { get; set; }
            
            public DateTime EndDate { get; set; }

            public string Form { get; set; }

            public int FormVersion { get; set; }
        }

        private class RowDescriptor : ICloneable
        {
            public RowDescriptor()
            {
                Items = new Dictionary<string, string>();
            }

            public string Code { get; set; }

            public Dictionary<string, string> Items { get; private set; }

            public object Clone()
            {
                var clon = new RowDescriptor { Code = Code, Items = new Dictionary<string, string>() };
                foreach (var item in Items)
                {
                    clon.Items.Add(item.Key, item.Value);
                }

                return clon;
            }
        }
    }
}
