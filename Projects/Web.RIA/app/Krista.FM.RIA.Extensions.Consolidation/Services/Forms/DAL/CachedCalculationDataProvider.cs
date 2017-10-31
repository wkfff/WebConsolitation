using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Krista.FM.Common.Consolidation.Calculations;
using Krista.FM.Common.Consolidation.Data;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.Consolidation.Data;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.DAL
{
    public class CachedCalculationDataProvider : IDataProvider
    {
        private readonly IReportSectionDataService dataService;
        private readonly ICollectingTaskRepository collectTaskRepository;

        private readonly Dictionary<string, IList<IRecord>> dataCach = new Dictionary<string, IList<IRecord>>();
        
        private ICollection<D_CD_Report> subReportsCach; 

        private D_CD_Report report;

        private IList<D_Form_TableColumn> slaveMetaColumns;

        private string sectionCode;
        private string formCode;
        private bool isSlave;

        public CachedCalculationDataProvider(IReportSectionDataService dataService, ICollectingTaskRepository collectTaskRepository)
        {
            this.dataService = dataService;
            this.collectTaskRepository = collectTaskRepository;
        }

        public void SetReport(D_CD_Report reportContext)
        {
            report = reportContext;
        }

        public void SetContext(string sectionName, string formName, bool slave)
        {
            sectionCode = sectionName;
            formCode = formName;
            isSlave = slave;
        }

        public IList<IRecord> GetSectionRows()
        {
            var key = GetKey(sectionCode, formCode, isSlave);
            IList<IRecord> cachItem;
            if (dataCach.TryGetValue(key, out cachItem))
            {
                return cachItem;
            }

            IList<IRecord> data = null;
            if (isSlave)
            {
                var childSubjects = GetCollectSubjects(report.RefTask.RefSubject);
                if (subReportsCach == null)
                {
                    subReportsCach = collectTaskRepository.GetReports(report.RefTask.RefCollectTask);
                }
                
                var reports = subReportsCach;
                foreach (var subReport in reports.Where(x => childSubjects.Contains(x.RefTask.RefSubject) && x.RefForm.Code.ToUpper() == formCode.ToUpper()))
                {
                    if (slaveMetaColumns == null)
                    {
                        slaveMetaColumns = subReport.RefForm.Parts.First(x => x.Code.ToUpper() == sectionCode).Columns;
                    }

                    if (data == null)
                    {
                        data = dataService.GetAll(subReport, sectionCode);
                    }
                    else
                    {
                        var appendData = dataService.GetAll(subReport, sectionCode);
                        foreach (var record in appendData)
                        {
                            data.Add(record);
                        }
                    }
                }
            }
            else
            {
                var contextReport = report;

                data = dataService.GetAll(contextReport, sectionCode);
            }

            if (data == null)
            {
                return new List<IRecord>();
            }

            dataCach.Add(key, data);

            return data;
        }

        public IList<IRecord> GetSectionRows(string sqlFilter)
        {
            throw new NotImplementedException();
        }

        public IList<D_Form_TableColumn> GetMetaColumns()
        {
            if (isSlave)
            {
                return slaveMetaColumns;
            }
            
            return report.RefForm.Parts.First(x => x.Code.ToUpper() == sectionCode.ToUpper()).Columns;
        }

        private static string GetKey(string sectionName, string formName, bool slave)
        {
            return new StringBuilder()
                .Append(formName)
                .Append('.')
                .Append(sectionName)
                .Append('.')
                .Append(slave)
                .ToString();
        }

        /// <summary>
        /// Получает все субъекты для дерективы ПОДОТЧЕТНЫЙ[].
        /// </summary>
        /// <param name="subject">Субъект отчета.</param>
        private IEnumerable<D_CD_Subjects> GetCollectSubjects(D_CD_Subjects subject)
        {
            List<D_CD_Subjects> result = new List<D_CD_Subjects> { subject };
            result.AddRange(subject.Childs);
            return result;
        }
    }
}
