using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Mrot
{
    public class ExportRepository : IExportRepository
    {
        private readonly ILinqRepository<D_CD_Task> taskRepository;
        private readonly ILinqRepository<F_Marks_MOFOTrihedralAgr> factRepository;

        public ExportRepository(
            ILinqRepository<D_CD_Task> taskRepository, 
            ILinqRepository<F_Marks_MOFOTrihedralAgr> factRepository)
        {
            this.taskRepository = taskRepository;
            this.factRepository = factRepository;
        }

        public IList<D_CD_Subjects> GetIncompleteRegions(int taskId)
        {
            return GetChildTasks(taskId)
                .Where(f => f.RefStatus.ID == 1)
                .Select(task => task.RefSubject)
                .OrderBy(f => f.RefRegion.CodeLine)
                .ToList();
        }

        public IList<D_CD_Subjects> GetCompleteRegions(int taskId)
        {
            return GetChildTasks(taskId)
                .Where(f => f.RefStatus.ID == 2 || f.RefStatus.ID == 3)
                .Select(task => task.RefSubject)
                .OrderBy(f => f.RefRegion.CodeLine)
                .ToList();
        }

        public D_CD_Task GetTaskInfo(int taskId)
        {
            return taskRepository.FindOne(taskId);
        }

        public IList<D_CD_Task> GetChildTasks(int taskId)
        {
            var taskInfo = GetTaskInfo(taskId);

            if (String.Compare(taskInfo.RefTemplate.Class, "ConsFormMrotSummary", true) == 0)
            {
                return taskRepository.FindAll()
                    .Where(
                        f => f.RefTemplate.Class == "ConsFormMrot" &&
                             f.EndDate == taskInfo.EndDate)
                    .ToList();

                // f.Name == rootTask.Name &&
            }

            return new List<D_CD_Task> { taskInfo };
        }

        public IList<SubjectTrihedrDataModel> GetSubjectTrihedrData(int taskId)
        {
            var childTasks = GetChildTasks(taskId);

            var factList = from f in factRepository.FindAll()
                           where childTasks.Contains(f.RefReport.RefTask) && f.RefReport.RefTask.RefStatus.ID != 1
                           orderby f.RefRegions.CodeLine, f.RefOrgType
                           group f by new { f.RefRegions.CodeLine, f.RefRegions.Name, f.RefRegions.ID, f.RefRegions.RefTerr.FullName, f.RefOrgType }
                               into g
                               select new SubjectTrihedrDataModel
                               {
                                   RegionName = g.Key.Name,
                                   RegionId = g.Key.ID,
                                   RegionCode = g.Key.CodeLine,
                                   RegionTypeName = g.Key.FullName,
                                   OrgName = g.Key.RefOrgType.Name,
                                   OrgType = g.Key.RefOrgType.ID,
                                   PrincipalCountOffBudget = g.SumWithNull(x => x.RefMarks.ID == 2 ? x.Value : null),
                                   WorkerCountOffBudget = g.SumWithNull(x => x.RefMarks.ID == 3 ? x.Value : null),
                                   PrincipalCountJoined = g.SumWithNull(x => x.RefMarks.ID == 5 ? x.Value : null),
                                   WorkerCountJoined = g.SumWithNull(x => x.RefMarks.ID == 6 ? x.Value : null),
                                   PrincipalCountMinSalary = g.SumWithNull(x => x.RefMarks.ID == 8 ? x.Value : null),
                                   WorkerCountMinSalary = g.SumWithNull(x => x.RefMarks.ID == 9 ? x.Value : null),
                                   PrincipalCountAvgSalary = g.SumWithNull(x => x.RefMarks.ID == 11 ? x.Value : null),
                                   WorkerCountAvgSalary = g.SumWithNull(x => x.RefMarks.ID == 12 ? x.Value : null),
                                   MinSalary = g.SumWithNull(x => x.RefMarks.ID == 14 ? x.Value : null),
                                   AvgSalary = g.SumWithNull(x => x.RefMarks.ID == 15 ? x.Value : null)
                               };

            var orgTypes = from f in factList.ToList()
                             group f by new { f.OrgType, f.OrgName }
                                 into g
                                 select g;

            var emptyTasks = from f in childTasks
                           where f.RefStatus.ID == 1
                           select f;

            var emptyRegionsList = new List<SubjectTrihedrDataModel>();

            foreach (var orgType in orgTypes)
            {
                var type = orgType;
                var emptyList = from f in emptyTasks 
                                select new SubjectTrihedrDataModel
                                           {
                                               RegionName = f.RefSubject.RefRegion.Name,
                                               RegionId = f.RefSubject.RefRegion.ID,
                                               RegionCode = f.RefSubject.RefRegion.CodeLine,
                                               RegionTypeName = f.RefSubject.RefRegion.RefTerr.FullName,
                                               OrgName = type.Key.OrgName,
                                               OrgType = type.Key.OrgType,
                                               PrincipalCountOffBudget = 0,
                                               WorkerCountOffBudget = 0,
                                               PrincipalCountJoined = 0,
                                               WorkerCountJoined = 0,
                                               PrincipalCountMinSalary = 0,
                                               WorkerCountMinSalary = 0,
                                               PrincipalCountAvgSalary = 0,
                                               WorkerCountAvgSalary = 0,
                                               MinSalary = 0,
                                               AvgSalary = 0
                                           };
                emptyRegionsList.AddRange(emptyList);
            }

            var lstResult = factList.ToList();
            lstResult.AddRange(emptyRegionsList);

            var regionList = from f in lstResult group f by new { f.RegionId } into g select g;

            emptyRegionsList.Clear();

            foreach (var regionId in regionList.ToList())
            {
                var dataList = from f in lstResult where f.RegionId == regionId.Key.RegionId select f;

                if (dataList.Count() == orgTypes.Count())
                {
                    continue;
                }

                var dataRow = dataList.FirstOrDefault();

                foreach (var orgType in orgTypes)
                {
                    var type = orgType;
                    var existList = 
                        from f in lstResult
                        where f.RegionId == regionId.Key.RegionId && f.OrgType == type.Key.OrgType
                        select f;

                    if (existList.Count() > 0)
                    {
                        continue;
                    }

                    var newData = new SubjectTrihedrDataModel
                                      {
                                          RegionName = dataRow.RegionName,
                                          RegionId = dataRow.RegionId,
                                          RegionCode = dataRow.RegionCode,
                                          RegionTypeName = dataRow.RegionTypeName,
                                          OrgName = type.Key.OrgName,
                                          OrgType = type.Key.OrgType,
                                          PrincipalCountOffBudget = 0,
                                          WorkerCountOffBudget = 0,
                                          PrincipalCountJoined = 0,
                                          WorkerCountJoined = 0,
                                          PrincipalCountMinSalary = 0,
                                          WorkerCountMinSalary = 0,
                                          PrincipalCountAvgSalary = 0,
                                          WorkerCountAvgSalary = 0,
                                          MinSalary = 0,
                                          AvgSalary = 0
                                      };

                    emptyRegionsList.Add(newData);
                }
            }

            lstResult.AddRange(emptyRegionsList);
            return lstResult;
        }
    }
}
