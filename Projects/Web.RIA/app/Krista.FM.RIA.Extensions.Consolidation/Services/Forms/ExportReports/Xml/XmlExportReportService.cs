using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ExportReports.Xml
{
    public class XmlExportReportService : IXmlExportReportService
    {
        private readonly IReportSectionDataService sectionDataService;
        private readonly IReportRequisiteDataService reportRequisiteDataService;
        private readonly ISectionRequisiteDataService sectionRequisiteDataService;

        public XmlExportReportService(IReportSectionDataService sectionDataService, IReportRequisiteDataService reportRequisiteDataService, ISectionRequisiteDataService sectionRequisiteDataService)
        {
            this.sectionDataService = sectionDataService;
            this.reportRequisiteDataService = reportRequisiteDataService;
            this.sectionRequisiteDataService = sectionRequisiteDataService;
        }

        public Stream Export(D_CD_Report report)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                Stream stream = new MemoryStream();
                XmlWriter w = new XmlTextWriter(stream, Encoding.UTF8);
                w.WriteStartDocument();

                w.WriteStartElement("ReportDescriptor");

                WriteTaskDescriptor(w, report.RefTask);

                var visitor = new Visitor(w, sectionDataService, reportRequisiteDataService, sectionRequisiteDataService);
                visitor.Visit(report);

                w.WriteEndElement();

                w.WriteEndDocument();
                w.Flush();
                stream.Position = 0;
                return stream;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        private void WriteTaskDescriptor(XmlWriter writer, D_CD_Task task)
        {
            writer.WriteStartElement("TaskDescriptor");

            writer.WriteStartElement("Subject");
            writer.WriteString(task.RefSubject.Name);
            writer.WriteEndElement();
            
            writer.WriteStartElement("SubjectLevel");
            writer.WriteString(task.RefSubject.RefLevel.Code);
            writer.WriteEndElement();

            writer.WriteStartElement("SubjectRole");
            writer.WriteString(task.RefSubject.RefRole.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("BeginDate");
            writer.WriteString(task.BeginDate.ToString("s"));
            writer.WriteEndElement();

            writer.WriteStartElement("EndDate");
            writer.WriteString(task.EndDate.ToString("s"));
            writer.WriteEndElement();

            writer.WriteStartElement("Form");
            writer.WriteString(task.RefTemplate.InternalName);
            writer.WriteEndElement();

            writer.WriteStartElement("FormVersion");
            writer.WriteString(Convert.ToString(task.RefTemplate.FormVersion));
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}
