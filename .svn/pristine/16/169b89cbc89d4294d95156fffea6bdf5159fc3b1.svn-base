using System.Data;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public interface ISectionRequisiteDataService
    {
        /// <summary>
        /// Выбирает строку реквизитов отчета.
        /// </summary>
        /// <param name="report">Конкретный отчет.</param>
        /// <param name="section">Раздел отчета.</param>
        /// <param name="requisiteKind">Вид реквизитов.</param>
        DataTable Get(D_CD_Report report, D_Report_Section section, RequisiteKinds requisiteKind);

        void Save(D_CD_Report report, D_Report_Section section, RequisiteKinds requisiteKind, Ext.Net.JsonObject json);

        void Save(D_CD_Report report, D_Report_Section section, RequisiteKinds requisiteKind, DataRow row);
    }
}