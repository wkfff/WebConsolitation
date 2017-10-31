using System.Collections.Generic;
using System.Data;
using Krista.FM.Common.Consolidation.Forms;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public interface IReportRequisiteDataService
    {
        /// <summary>
        /// �������� ������ ���������� ������.
        /// </summary>
        /// <param name="report">���������� �����.</param>
        /// <param name="requisites">��������� ����������.</param>
        /// <param name="requisiteKind">��� ����������.</param>
        DataTable Get(D_CD_Report report, IEnumerable<D_Form_Requisites> requisites, RequisiteKinds requisiteKind);

        void Save(D_CD_Report report, IList<D_Form_Requisites> requisites, RequisiteKinds requisiteKind, Ext.Net.JsonObject json);

        void Save(D_CD_Report report, IList<D_Form_Requisites> requisites, RequisiteKinds requisiteKind, DataRow row);
    }
}