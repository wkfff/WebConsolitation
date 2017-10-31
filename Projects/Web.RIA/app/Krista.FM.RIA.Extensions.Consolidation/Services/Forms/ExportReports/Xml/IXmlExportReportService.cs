using System.IO;

using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ExportReports.Xml
{
    public interface IXmlExportReportService
    {
        Stream Export(D_CD_Report report);
    }
}