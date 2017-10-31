using System.Collections.Generic;
using Ext.Net;

using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public interface IReportSectionDataService
    {
        /// <summary>
        /// �������� ��� ������ ������� ������.
        /// </summary>
        /// <param name="report">���������� �����.</param>
        /// <param name="sectionCode">���������� ��� ������� ������.</param>
        IList<IRecord> GetAll(D_CD_Report report, string sectionCode);

        IRecord CreateRecord(D_CD_Report report, string sectionCode);

        void Save(D_CD_Report report, D_Form_Part formSection, IEnumerable<IRecord> records);

        void Save(D_CD_Report report, D_Form_Part formSection, JsonObject json);

        /// <summary>
        /// ��������� �������� �������� ����� ��� ���������� �������.
        /// </summary>
        /// <param name="section">������ ������.</param>
        IList<IRecord> GetMultipliesRowsTemplates(D_Report_Section section);
    }
}