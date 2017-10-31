using System.Collections.Generic;
using System.Data;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public interface IReportRequisiteDataService
    {
        /// <summary>
        /// Выбирает строку реквизитов отчета.
        /// </summary>
        /// <param name="report">Конкретный отчет.</param>
        /// <param name="requisites">Коллекция реквизитов.</param>
        /// <param name="requisiteKind">Вид реквизитов.</param>
        DataTable Get(D_CD_Report report, IEnumerable<D_Form_Requisites> requisites, RequisiteKinds requisiteKind);

        void Save(D_CD_Report report, IList<D_Form_Requisites> requisites, RequisiteKinds requisiteKind, Ext.Net.JsonObject json);

        void Save(D_CD_Report report, IList<D_Form_Requisites> requisites, RequisiteKinds requisiteKind, DataRow row);
    }
}