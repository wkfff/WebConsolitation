using System.IO;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.Metadata
{
    public interface IFormExportService
    {
        Stream Export(D_CD_Templates form);
    }
}