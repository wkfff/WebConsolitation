using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public class FormRepository : IFormRepository
    {
        private readonly ILinqRepository<F_Marks_MOFOTrihedralAgr> factRepository;
        private readonly ILinqRepository<FX_OrgType_TrihedrAlagr> orgRepository;
        private readonly ILinqRepository<FX_Marks_TrihedrAlagr> markRepository;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly ILinqRepository<DataSources> datasourceRepository;
        private readonly IRepository<D_CD_Task> taskRepository;
        private readonly ILinqRepository<D_Report_TrihedrAgr> reportRepository;

        public FormRepository(
            ILinqRepository<F_Marks_MOFOTrihedralAgr> factRepository,
            ILinqRepository<FX_OrgType_TrihedrAlagr> orgRepository,
            ILinqRepository<FX_Marks_TrihedrAlagr> markRepository,
            IRepository<FX_Date_YearDayUNV> periodRepository,
            ILinqRepository<DataSources> datasourceRepository,
            IRepository<D_CD_Task> taskRepository,
            ILinqRepository<D_Report_TrihedrAgr> reportRepository)
        {
            this.factRepository = factRepository;
            this.orgRepository = orgRepository;
            this.markRepository = markRepository;
            this.periodRepository = periodRepository;
            this.datasourceRepository = datasourceRepository;
            this.taskRepository = taskRepository;
            this.reportRepository = reportRepository;
        }

        public IList<FormModel> GetFormData(int taskId)
        {
            var report = reportRepository.FindAll()
                .Where(x => x.RefTask.ID == taskId)
                .FirstOrDefault();

            // select ...
            // from f_Marks_MOFOTrihedralAgr f 
            // join fx_OrgType_TrihedrAlagr o on (f.RefOrgType = o.ID)
            var data1 = from f in factRepository.FindAll()
                        join o in orgRepository.FindAll() on f.RefOrgType equals o
                        where f.RefReport == report
                        select new { f.RefMarks, OrgType = f.RefOrgType.ID, f.Value, f.ID };

            // right outer join fx_Marks_TrihedrAlagr m on (f.RefMarks = m.ID)
            var data2 = from m in markRepository.FindAll().ToList()
                        join f in data1 on m equals f.RefMarks into fm
                        from f in fm.DefaultIfEmpty()
                        select new
                        {
                            m.Code,
                            m.Name,
                            OrgType = f == null ? null : (int?)f.OrgType,
                            Value = f == null ? null : f.Value,
                            Id = f == null ? null : (int?)f.ID,
                        };

            // group by Code, Name
            // order by Code
            var data3 = from f in data2
                        group f by new { f.Code, f.Name } into g
                        orderby g.Key.Code
                        select new FormModel
                        {
                            Code = g.Key.Code,
                            CodeStr = "{0}.{1}".FormatWith(g.Key.Code / 10, g.Key.Code % 10),
                            Name = g.Key.Name,
                            OrgType1 = g.SumWithNull(x => x.OrgType == 1 ? x.Value : null),
                            OrgType2 = g.SumWithNull(x => x.OrgType == 2 ? x.Value : null),
                            OrgType3 = g.SumWithNull(x => x.OrgType == 3 ? x.Value : null),
                            IsEditable = g.Key.Code % 10 > 0
                        };

            return data3.ToList();
        }

        public void Save(
            AjaxStoreSaveDomainModel<FormModel> data,
            int taskId,
            D_Regions_Analysis region)
        {
            if (data.Updated == null)
            {
                return;
            }

            var task = taskRepository.Get(taskId);
            var report = reportRepository.FindAll()
                .Where(x => x.RefTask.ID == taskId)
                .FirstOrDefault();
            if (report == null)
            {
                report = new D_Report_TrihedrAgr { RefTask = task };
                reportRepository.Save(report);
            }

            // TODO: Требуемого источника может не быть, нужно либо создавать либо ругаться
            int sourceId = datasourceRepository.FindAll()
                .Where(x => x.SupplierCode == "ФО" && x.DataCode == 6 && x.Year == Convert.ToString(task.BeginDate.Year))
                .First().ID;

            foreach (var model in data.Updated)
            {
                if (model.Code > 50)
                {
                    SaveFact(model.Code, 1, sourceId, task, report, region, model.OrgType1);
                }
                else if (model.Code < 50)
                {
                    SaveFact(model.Code, 2, sourceId, task, report, region, model.OrgType2);
                    SaveFact(model.Code, 3, sourceId, task, report, region, model.OrgType3);
                }
            }
        }

        private void SaveFact(int x, int y, int sourceId, D_CD_Task task, D_Report_TrihedrAgr report, D_Regions_Analysis region, decimal? value)
        {
            var fact = (from f in factRepository.FindAll()
                        where f.RefMarks.Code == x && f.RefOrgType.ID == y && f.RefReport.ID == report.ID 
                        select f).FirstOrDefault();

            if (fact != null)
            {
                fact.Value = value;
            }
            else
            {
                fact = new F_Marks_MOFOTrihedralAgr
                {
                    SourceID = sourceId,
                    Value = value,
                    RefReport = report,
                    RefRegions = region,
                    RefOrgType = orgRepository.FindOne(y),
                    RefMarks = markRepository.FindAll().Single(m => m.Code == x),
                    RefYearDayUNV = periodRepository.Get((task.BeginDate.Year * 10000) + 9991 + Convert.ToInt32(Math.Round(task.BeginDate.Month / (float)4, MidpointRounding.AwayFromZero)))
                };
            }

            factRepository.Save(fact);
        }
    }
}
