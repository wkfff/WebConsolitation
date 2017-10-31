using System;
using System.Collections.Generic;
using System.Linq;

using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public class CalculationPrimaryDataProvider : IPrimaryDataProvider
    {
        private readonly IReportSectionDataService dataService;

        private D_CD_Report report;

        private string sectionCode;

        private IList<IRecord> data;

        public CalculationPrimaryDataProvider(IReportSectionDataService dataService)
        {
            this.dataService = dataService;
        }

        public void SetLeftContext(D_CD_Report reportContext, string sectionCodeContext)
        {
            report = reportContext;
            sectionCode = sectionCodeContext;
        }

        public IList<IRecord> GetSectionRows()
        {
            if (data == null)
            {
                data = dataService.GetAll(report, sectionCode);
            }

            return data;
        }

        public List<IRecord> GetSectionRows(string sqlFilter)
        {
            throw new NotImplementedException();
        }

        public IRecord CreateRecord(IRecord template)
        {
            var record = dataService.CreateRecord(report, sectionCode);
            record.AssignRecord(template.Value);
            return record;
        }

        public void AppendRecord(IRecord record)
        {
            data.Add(record);
        }

        public IList<IRecord> GetMultipliesRowsTemplates()
        {
            var section = report.Sections.First(x => x.RefFormSection.Code == sectionCode);
            return dataService.GetMultipliesRowsTemplates(section);
        }

        public void Save()
        {
            if (data == null)
            {
                return;
            }

            var formSection = report.RefForm.Parts.First(x => x.Code.ToUpper() == sectionCode.ToUpper());

            dataService.Save(report, formSection, data);
        }
    }
}
