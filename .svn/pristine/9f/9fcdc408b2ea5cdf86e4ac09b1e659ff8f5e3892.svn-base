using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ExportReports.Xml
{
    public class Visitor
    {
        private readonly XmlWriter writer;
        private readonly IReportSectionDataService sectionDataService;
        private readonly IReportRequisiteDataService reportRequisiteDataService;
        private readonly ISectionRequisiteDataService sectionRequisiteDataService;

        private D_CD_Report currentReport;
        private D_Report_Section currentSection;

        public Visitor(XmlWriter xmlWriter, IReportSectionDataService sectionDataService, IReportRequisiteDataService reportRequisiteDataService, ISectionRequisiteDataService sectionRequisiteDataService)
        {
            writer = xmlWriter;
            this.sectionDataService = sectionDataService;
            this.reportRequisiteDataService = reportRequisiteDataService;
            this.sectionRequisiteDataService = sectionRequisiteDataService;
        }

        public void Visit(D_CD_Report report)
        {
            currentReport = report;

            writer.WriteStartElement("Report");

            var requisites = report.RefForm.Requisites.ToList();

            if (requisites.Any(x => x.IsHeader))
            {
                Visit(reportRequisiteDataService.Get(report, requisites.Where(x => x.IsHeader), RequisiteKinds.Header).SetName("HeaderReportRequisites"));
            }

            Visit(report.Sections.OrderBy(x => x.RefFormSection.Ord).ToList());

            if (requisites.Any(x => !x.IsHeader))
            {
                Visit(reportRequisiteDataService.Get(report, requisites.Where(x => !x.IsHeader), RequisiteKinds.Footer).SetName("FooterReportRequisites"));
            }

            writer.WriteEndElement();
        }

        public void Visit(IList<D_Report_Section> sections)
        {
            writer.WriteStartElement("Sections");
            writer.WriteAttributeString("size", Convert.ToString(sections.Count()));
            foreach (var section in sections)
            {
                Visit(section);
            }

            writer.WriteEndElement();
        }

        public void Visit(D_Report_Section section)
        {
            currentSection = section;

            writer.WriteStartElement("Section");
            writer.WriteAttributeString("internalName", section.RefFormSection.InternalName);

            if (section.RefFormSection.Requisites.Any(x => x.IsHeader))
            {
                Visit(sectionRequisiteDataService.Get(currentReport, section, RequisiteKinds.Header).SetName("HeaderSectionRequisites"));
            }

            var rows = sectionDataService.GetAll(section.RefReport, section.RefFormSection.Code);
            Visit(rows);

            if (section.RefFormSection.Requisites.Any(x => !x.IsHeader))
            {
                Visit(sectionRequisiteDataService.Get(currentReport, section, RequisiteKinds.Footer).SetName("FooterSectionRequisites"));
            }

            writer.WriteEndElement();
        }

        public void Visit(IList<IRecord> rows)
        {
            writer.WriteStartElement("Rows");
            writer.WriteAttributeString("size", Convert.ToString(rows.Count()));
            foreach (var row in rows)
            {
                Visit(row);
            }

            writer.WriteEndElement();
        }

        public void Visit(IRecord row)
        {
            writer.WriteStartElement("Row");
            writer.WriteAttributeString("code", ((D_Report_Row)row.Value).RefFormRow.Name);
            if (((D_Report_Row)row.Value).RefFormRow.Multiplicity)
            {
                writer.WriteAttributeString("mult", "true");
            }

            foreach (var column in currentSection.RefFormSection.Columns.OrderBy(x => x.Ord))
            {
                writer.WriteStartElement(column.InternalName);
                writer.WriteValue(row.Get(column.InternalName) ?? String.Empty);
                writer.WriteEndElement();
            }
            
            writer.WriteEndElement();
        }

        public void Visit(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
            {
                return;
            }

            writer.WriteStartElement(dataTable.TableName);

            var row = dataTable.Rows[0];
            foreach (DataColumn column in dataTable.Columns.OfType<DataColumn>().OrderBy(x => x.ColumnName))
            {
                if (column.ColumnName == "ID")
                {
                    continue;
                }

                writer.WriteStartElement(column.ColumnName);
                var value = row[column] is DBNull ? String.Empty : row[column];
                writer.WriteValue(value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
