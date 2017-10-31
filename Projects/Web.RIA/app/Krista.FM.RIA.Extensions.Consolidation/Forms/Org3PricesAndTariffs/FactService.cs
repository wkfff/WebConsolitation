using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.Org3PricesAndTariffs
{
    public class FactService : IFactService
    {
        private readonly ILinqRepository<T_Org_CPrice> factRepository;
        private readonly ILinqRepository<D_Org_RegistrOrg> organizationRepository;
        private readonly ILinqRepository<FX_Org_ProdGroup> prodGroupRepository;
        private readonly ILinqRepository<D_Org_Good> goodRepository;
        private readonly ILinqRepository<D_Org_Report> reportRepository;
        private readonly ILinqRepository<D_CD_Task> taskRepository;
        private readonly ITaskService taskService;

        private readonly ILinqRepository<D_OK_OKOPF> okokopfRepository;
        private readonly ILinqRepository<D_OK_OKFS> okokfsRepository;
        private readonly ILinqRepository<D_Org_TypeOrg> orgtypeorgRepository;
        private readonly ILinqRepository<B_Org_OrgBridge> orgbridgeRepository;

        public FactService(
                           ILinqRepository<T_Org_CPrice> factRepository, 
                           ILinqRepository<D_Org_RegistrOrg> organizationRepository, 
                           ILinqRepository<FX_Org_ProdGroup> prodGroupRepository,
                           ILinqRepository<D_Org_Good> goodRepository,
                           ILinqRepository<D_Org_Report> reportRepository,
                           ILinqRepository<D_CD_Task> taskRepository,
                           ITaskService taskService, 
                           ILinqRepository<D_OK_OKOPF> okokopfRepository, 
                           ILinqRepository<D_OK_OKFS> okokfsRepository, 
                           ILinqRepository<D_Org_TypeOrg> orgtypeorgRepository, 
                           ILinqRepository<B_Org_OrgBridge> orgbridgeRepository)
        {
            this.factRepository = factRepository;
            this.organizationRepository = organizationRepository;
            this.prodGroupRepository = prodGroupRepository;
            this.goodRepository = goodRepository;
            this.reportRepository = reportRepository;
            this.taskService = taskService;
            this.okokopfRepository = okokopfRepository;
            this.okokfsRepository = okokfsRepository;
            this.orgtypeorgRepository = orgtypeorgRepository;
            this.orgbridgeRepository = orgbridgeRepository;
            this.taskRepository = taskRepository;
        }

        public IList<FormModel> LoadFactData(int taskId)
        {
            var data = factRepository.FindAll()
                                     .Where(f => f.RefReport.RefTask.ID == taskId)
                                     .Select(f => new FormModel
                                                    {
                                                        ID = f.ID,
                                                        Ordering = f.RefGood.Code,
                                                        NameOrg = f.RefRegistrOrg.NameOrg,
                                                        RefRegistrOrg = f.RefRegistrOrg.ID,
                                                        NameGood = f.RefGood.Name,
                                                        RefGood = f.RefGood.ID,
                                                        Price = f.Price
                                                    })
                                     .ToList();
            return data;
        }

        public IList<FormModel> GetInitialData(int taskId, GoodType goodType)
        {
            var task = taskService.GetTaskViewModel(taskId);

            if (task.RefStatus != (int)TaskViewModel.TaskStatus.Edit)
            {
                return new List<FormModel>();
            }

            if (task.Region == null || task.Region.RefBridgeRegions == null)
            {
                throw new Exception("У задачи не корректно задан Район, либо данный Район не сопоставлен");
            }

            var goods = GetGoods(goodType).ToList();

            var org = GetOrganizations(task.Region.RefBridgeRegions, goodType).ToList();
            
            var data = (from g in goods
                        from o in org
                        select new FormModel
                                   {
                                       Ordering = g.Code,
                                       NameOrg = o.NameOrg,
                                       RefRegistrOrg = o.ID,
                                       NameGood = g.Name,
                                       RefGood = g.ID,
                                       Price = null
                                   })
                        .ToList();

            for (int i = 0; i < data.Count; i++)
            {
                data[i].ID = -(i + 1);
            }
            
            return data;
        }

        public void CreateData(int taskId, FormModel[] rows)
        {
            var report = GetReport(taskId);

            foreach (FormModel row in rows)
            {
                var fact = new T_Org_CPrice
                               {
                                   Price = row.Price ?? 0,
                                   RefReport = report,
                                   RefRegistrOrg = organizationRepository.FindOne(row.RefRegistrOrg),
                                   RefGood = goodRepository.FindOne(row.RefGood)
                               };
                factRepository.Save(fact);
                factRepository.DbContext.CommitChanges();

                ProtectionFromDuplicatesInFactData(fact);
            }
        }

        public void UpdateData(int taskId, FormModel[] rows)
        {
            var report = GetReport(taskId);
            
            foreach (FormModel row in rows)
            {
                var fact = factRepository.FindAll()
                             .FirstOrDefault(x => x.RefReport == report
                                                  && x.RefRegistrOrg.ID == row.RefRegistrOrg
                                                  && x.RefGood.ID == row.RefGood);
                if (fact == null)
                {
                    continue;
                }

                fact.Price = row.Price ?? 0;

                factRepository.Save(fact);
                factRepository.DbContext.CommitChanges();

                ProtectionFromDuplicatesInFactData(fact);
            }
        }

        public IList GetOrganizations(int taskId, string filter, GoodType goodType)
        {
            var task = taskService.GetTaskViewModel(taskId);

            IList orgs;
            
            if (filter == null)
            {
                orgs = GetOrganizations(task.Region.RefBridgeRegions, goodType)
                            .OrderBy(t => t.Code)
                            .Select(x => new { x.ID, Name = x.NameOrg, IsMarketGrid = x.SignCN ?? false }).ToList();
            }
            else
            {
                orgs = GetOrganizations(task.Region.RefBridgeRegions, goodType)
                            .Where(x => x.NameOrg.ToUpper().Contains(filter.ToUpper()))
                            .OrderBy(t => t.Code)
                            .Select(x => new { x.ID, Name = x.NameOrg, IsMarketGrid = x.SignCN ?? false }).ToList();
            }

            return orgs;
        }

        public void IncludeOrganization(int taskId, int organizationId)
        {
            var report = GetReport(taskId);

            var org = organizationRepository.FindOne(organizationId);

            if (org == null)
            {
                throw new Exception("Организация не найдена");
            }

            var existingFacts = factRepository.FindAll()
                .Where(x => x.RefReport == report
                            && x.RefRegistrOrg == org);
            if (existingFacts.Any())
            {
                throw new Exception("Данная организация уже была добавлена ранее");
            }

            IncludeOrganization(report, org);
        }

        public void CreateAndIncludeOrganization(int taskId, string orgName, bool orgIsMarketGrid, GoodType goodType)
        {
            var report = GetReport(taskId);
            var refRegion = report.RefTask.RefSubject.RefRegion;

            CheckUniqueOrganization(orgName, refRegion, goodType);

            var org = CreateOrganization(orgName, orgIsMarketGrid, refRegion, goodType);

            IncludeOrganization(report, org);
        }

        public void ExcludeOrganization(int taskId, int organizationId)
        {
            var report = GetReport(taskId);

            var org = organizationRepository.FindOne(organizationId);

            if (org == null)
            {
                throw new Exception("Организация не найдена");
            }

            var facts = factRepository.FindAll()
                                      .Where(x => x.RefReport == report
                                               && x.RefRegistrOrg == org)
                                      .ToList();
            foreach (var row in facts)
            {
                factRepository.Delete(row);
            }

            factRepository.DbContext.CommitChanges();
        }

        public IList GetOldTaskDates(int taskId)
        {
            var currentTask = GetTask(taskId);

            var data = taskRepository.FindAll().Where(x => x.RefTemplate == currentTask.RefTemplate
                                                          && x.BeginDate < currentTask.BeginDate
                                                          && x.RefSubject == currentTask.RefSubject)
                                               .OrderByDescending(x => x.BeginDate)
                                               .Select(x => new { x.ID, ReportDate = x.BeginDate.ToString("dd.MM.yyyy") })
                                               .ToList();
            return data;
        }

        public void CopyFromTask(int taskId, int sourceTaskId)
        {
            var currentReport = GetReport(taskId);
            var sourceReport = GetReport(sourceTaskId);

            if (currentReport.RefTask.RefSubject.RefRegion.RefBridgeRegions != sourceReport.RefTask.RefSubject.RefRegion.RefBridgeRegions)
            {
                throw new Exception("Район у пользователя копируемого отчета отличается от текущего.");
            }

            // Находим данные для копирования
            var sourceData = factRepository.FindAll().Where(x => x.RefReport == sourceReport).ToList();

            if (sourceData.Count == 0)
            {
                throw new Exception("Отсутствуют данные для копирования");
            }

            // Удаляем текущие данные
            var facts = factRepository.FindAll()
                                      .Where(x => x.RefReport == currentReport)
                                      .ToList();
            foreach (var row in facts)
            {
                factRepository.Delete(row);
            }

            // Вставляем новые
            foreach (var row in sourceData)
            {
                var fact = new T_Org_CPrice
                {
                    Price = row.Price,
                    RefReport = currentReport,
                    RefRegistrOrg = row.RefRegistrOrg,
                    RefGood = row.RefGood
                };
                factRepository.Save(fact);
                factRepository.DbContext.CommitChanges();

                ProtectionFromDuplicatesInFactData(fact);
            }

            factRepository.DbContext.CommitChanges();
        }

        /// <summary>
        /// Создает запись в таблице зарегистрированных организаций
        /// </summary>
        /// <param name="orgName">Наименование организации</param>
        /// <param name="orgIsMarketGrid">Признак торговой сети </param>
        /// <param name="region">Район, к которому будет привязана данная организация</param>
        /// <param name="goodType">Тип организации(чем торгует)</param>
        private D_Org_RegistrOrg CreateOrganization(string orgName, bool orgIsMarketGrid, D_Regions_Analysis region, GoodType goodType)
        {
            var newCode = organizationRepository.FindAll().Max(x => x.Code);

            var org = new D_Org_RegistrOrg
                          {
                              RefRegionAn = region,
                              NameOrg = orgName,
                              SignCN = orgIsMarketGrid,
                              RefProdGroup = prodGroupRepository.FindOne((int)goodType),
                              Code = newCode + 1,

                              RefOK = okokopfRepository.FindOne(-1),
                              RefOKOKFS = okokfsRepository.FindOne(-1),
                              RefOrg = orgtypeorgRepository.FindOne(-1),
                              RefOrgBridge = orgbridgeRepository.FindOne(-1) 
                          };
            organizationRepository.Save(org);
            organizationRepository.DbContext.CommitChanges();
            return org;
        }

        /// <summary>
        /// Вносит записи с данной организацией для всех товаров в таблицу фактов
        /// </summary>
        private void IncludeOrganization(D_Org_Report report, D_Org_RegistrOrg org)
        {
            var goods = GetGoods((GoodType)org.RefProdGroup.ID).ToList();

            foreach (var good in goods)
            {
                var fact = new T_Org_CPrice
                {
                    Price = 0,
                    RefReport = report,
                    RefRegistrOrg = org,
                    RefGood = good
                };

                factRepository.Save(fact);
                factRepository.DbContext.CommitChanges();

                ProtectionFromDuplicatesInFactData(fact);
            }
        }

        /// <summary>
        /// Находит отчет по id
        /// </summary>
        private D_Org_Report GetReport(int taskId)
        {
            var report = reportRepository.FindAll().FirstOrDefault(x => x.RefTask.ID == taskId);

            if (report == null)
            {
                throw new Exception("Не найден отчет по задаче");
            }

            return report;
        }

        /// <summary>
        /// Находит отчет по id
        /// </summary>
        private D_CD_Task GetTask(int taskId)
        {
            var task = taskRepository.FindOne(taskId);

            if (task == null)
            {
                throw new Exception("Задача не найдена");
            }

            return task;
        }

        /// <summary>
        /// Находит список организаций для данного района по сопоставимому классификатору
        /// </summary>
        private IQueryable<D_Org_RegistrOrg> GetOrganizations(B_Regions_Bridge regionBridge, GoodType goodType)
        {
            var orgs = organizationRepository.FindAll().Where(x => x.RefRegionAn.RefBridgeRegions == regionBridge && x.RefProdGroup.ID == (int)goodType);
            return orgs;
        }

        /// <summary>
        /// Проверка на уникальность - существование организации с такими параметрами
        /// </summary>
        private void CheckUniqueOrganization(string orgName, D_Regions_Analysis refRegionAn, GoodType goodType)
        {
            var existingOrgs = organizationRepository.FindAll().Where(x => x.NameOrg == orgName 
                                                                           && x.RefRegionAn == refRegionAn
                                                                           && x.RefProdGroup.ID == (int)goodType);
            if (existingOrgs.Any())
            {
                throw new Exception("Организация с такими параметрами уже существует");
            }
        }
        
        /// <summary>
        /// Находит список товаров данной категории
        /// </summary>
        private IQueryable<D_Org_Good> GetGoods(GoodType goodType)
        {
            return goodRepository.FindAll().Where(x => x.RefProdGroup.ID == (int)goodType);
        }

        /// <summary>
        /// Проверка на дубликаты (актуально при многопользовательском режиме и отсутствии уникальных ключей в БД)
        /// </summary>
        private void ProtectionFromDuplicatesInFactData(T_Org_CPrice entity)
        {
            var dummy = from p in factRepository.FindAll()
                        where p.RefReport == entity.RefReport
                              && p.RefRegistrOrg == entity.RefRegistrOrg
                              && p.RefGood == entity.RefGood
                        select new { p.ID };
            int i = dummy.Count();
            if (dummy.Count() > 1)
            {
                factRepository.Delete(entity);
                factRepository.DbContext.CommitChanges();
                throw new Exception("Создаваемые данные по организации/товару уже были занесены.");
            }
        }
    }
}
