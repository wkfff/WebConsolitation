using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.OrgGKH.Params;
using Krista.FM.RIA.Extensions.OrgGKH.Presentation.Views;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для интерфейса «План по выручке»
    /// </summary>
    public class PlanProceedsController : SchemeBoundController
    {
        /// <summary>
        /// Репозиторий «Организации.ОРГАНИЗАЦИИ_Сбор по ЖКХ (f.Org.GKH)»
        /// </summary>
        private readonly ILinqRepository<F_Org_GKH> planRepository;

        /// <summary>
        /// Репозиторий глобальных параметров
        /// </summary>
        private readonly IOrgGkhExtension extension;

        /// <summary>
        /// Репозиторий Организации.Показатели ЖКХ
        /// </summary>
        private readonly ILinqRepository<D_Org_MarksGKH> marksGkhRepository;

        private readonly ILinqRepository<FX_Org_StatusD> statusDRepository;

        private readonly ILinqRepository<D_Org_RegistrOrg> orgRepository;

        private readonly ILinqRepository<D_Regions_Analysis> regionRepository;

        private int sourceId;

        public PlanProceedsController(
            ILinqRepository<F_Org_GKH> planRepository, 
            IOrgGkhExtension extension,
            ILinqRepository<FX_Org_StatusD> statusDRepository,
            ILinqRepository<D_Org_RegistrOrg> orgRepository,
            ILinqRepository<D_Regions_Analysis> regionRepository,
            ILinqRepository<D_Org_MarksGKH> marksGkhRepository)
        {
            this.planRepository = planRepository;
            this.extension = extension;
            this.statusDRepository = statusDRepository;
            this.orgRepository = orgRepository;
            this.regionRepository = regionRepository;
            this.marksGkhRepository = marksGkhRepository;
            sourceId = (extension.DataSource == null) ? -1 : extension.DataSource.ID;
        }

        /// <summary>
        /// Чтение данных по периоду
        /// </summary>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="regionId">Идентификатор района</param>
        /// <returns>Список фактов</returns>
        public ActionResult Read(int periodId, int regionId)
        {
            try
            {
                var resultData = new List<object>();
                if (regionId > 0)
                {
                    var orgs = orgRepository.FindAll()
                        .Where(x => x.RefRegionAn.ID == regionId || x.RefRegionAn.ParentID == regionId)
                        .OrderBy(x => x.Code);

                    var planAll = planRepository.FindAll();
                    var marks = marksGkhRepository.FindAll();
                    foreach (var org in orgs)
                    {
                        var plans = planAll.Where(x =>
                                                  x.RefRegistrOrg.ID == org.ID &&
                                                  x.RefYearDayUNV.ID == periodId &&
                                                  x.SourceID == sourceId);

                        var statusId = plans.Count(x =>
                                                   x.RefStatusD.ID == OrgGKHConsts.StateLocked &&
                                                   x.RefMarksGKH.Code != 50000 &&
                                                   x.RefMarksGKH.Code != 60000) > 0
                                           ? OrgGKHConsts.StateLocked
                                           : OrgGKHConsts.StateEdited;
                        var statusD = statusDRepository.FindOne(statusId);
                        int markId = 0;
                        var allPlansFilled = sourceId == -1
                                                 ? false
                                                 : CheckIfAllPlansFilled(marks, org.ID, periodId, out markId);

                        resultData.Add(new
                                           {
                                               org.ID,
                                               org.NameOrg,
                                               OrgId = org.ID,
                                               org.INN,
                                               Region = org.RefRegionAn.Name,
                                               HasData = plans.Count() > 0,
                                               StatusId = statusD.ID,
                                               StatusName = statusD.Name,
                                               MayAccept = allPlansFilled,
                                               EmptyMarks = markId,
                                               org.Login
                                           });
                    }
                }

                return new AjaxStoreResult(resultData.ToList(), resultData.ToList().Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Проверка, нет ли в БД данных записанных на организацию с указанным идентификатором
        /// </summary>
        /// <param name="orgId">Идентификатор организации</param>
        /// <returns>Ответ с признаком и сообщением, можно ли удалять организацию</returns>
        public AjaxStoreResult CheckCanDeleteOrg(int orgId)
        {
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            var can = planRepository.FindAll().Count(x => x.RefRegistrOrg.ID == orgId) <= 0;
            result.SaveResponse.Success = can;
            result.SaveResponse.Message = can 
                ? string.Empty 
                : "Удаление невозможно, так как в базе данных присутствуют данные записанные на выбранную организацию";
            return result;
        }

        /// <summary>
        /// Добавление новой записи в план по выручке
        /// </summary>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="data">Параметры записи</param>
        /// <returns>Результат сохранения</returns>
        [HttpPost]
        [Transaction]
        public RestResult Save(int periodId, string data)
        {
            try
            {
                if (sourceId == -1)
                {
                    return new RestResult
                    {
                        Success = false,
                        Message = "Источник не найден",
                        Data = new List<int>()
                    };   
                }

                var dataHandler = new StoreDataHandler(data);
                var dataSet = dataHandler.ObjectData<Dictionary<string, string>>();

                foreach (var deleted in dataSet.Deleted)
                {
                    // удаляем все данные на организацию
                    var id = Convert.ToInt32(deleted["OrgId"]);
                    if (id > 0)
                    {
                        var plansForOrg = planRepository.FindAll().Where(x => x.RefRegistrOrg.ID == id);
                        foreach (var plan in plansForOrg)
                        {
                            planRepository.Delete(plan);    
                        }
                    }

                    var orgId = Convert.ToInt32(deleted["OrgId"]);

                    // удаляем организацию
                    if (orgId > 0)
                    {
                        if (planRepository.FindAll().Where(x => x.RefRegistrOrg.ID == orgId).Count() <= 0)
                        {
                            orgRepository.Delete(orgRepository.FindOne(orgId));
                        }
                    }
                }

                var returnResult = new List<Dictionary<string, string>>();
                foreach (var updated in dataSet.Updated)
                {
                    // изменилось состояние, поэтому 
                    // у всех записей в таблице «Организации.ОРГАНИЗАЦИИ_Сбор по ЖКХ (f.Org.GKH)» 
                    // по выбранному периоду и выбранным организациям должна произойти смена состояния
                    var plans = planRepository.FindAll().Where(x =>
                        x.RefRegistrOrg.ID == Convert.ToInt32(updated["OrgId"]) && 
                        x.RefYearDayUNV.ID == periodId && 
                        x.SourceID == sourceId);

                    foreach (var plan in plans)
                    {
                        plan.RefStatusD = statusDRepository.FindOne(Convert.ToInt32(updated["StatusId"]));
                        planRepository.Save(plan);
                    }
                }

                return new RestResult
                {
                    Success = true,
                    Message = "Сохранено",
                    Data = returnResult
                }; 
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [Transaction]
        public RestResult Accept(int orgId, int periodId, int statusId)
        {
            try 
            {
                   // изменилось состояние, поэтому 
                    // у всех записей в таблице «Организации.ОРГАНИЗАЦИИ_Сбор по ЖКХ (f.Org.GKH)» 
                    // по выбранному периоду и выбранным организациям должна произойти смена состояния
                var plans = planRepository.FindAll().Where(x =>
                        x.RefRegistrOrg.ID == orgId && 
                        x.RefYearDayUNV.ID == periodId && 
                        x.SourceID == sourceId);

                    foreach (var plan in plans)
                    {
                        plan.RefStatusD = statusDRepository.FindOne(statusId);
                        planRepository.Save(plan);
                    }

                return new RestResult
                {
                    Success = true,
                    Message = "Сохранено",
                    Data = new List<int>()
                }; 
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public ActionResult ShowRequest(int periodId, int orgId, int regionId, bool isMonth)
        {
            if (isMonth)
            {
                var view = new MonthlyCollectionView(
                    extension, 
                    planRepository,
                    orgRepository,
                    orgId, 
                    orgRepository.FindOne(orgId).NameOrg, 
                    periodId,
                    regionRepository.FindOne(regionId).Name);
                return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", view);
            }

            var viewWeek = new WeeklyCollectionView(
                extension, 
                planRepository, 
                orgRepository,
                orgId,
                orgRepository.FindOne(orgId).NameOrg, 
                periodId, 
                regionRepository.FindOne(regionId).Name);
            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", viewWeek);
        }

        /// <summary>
        /// Чтение районов
        /// </summary>
        /// <returns>список районов</returns>
        public ActionResult LookupRegions()
        {
            try
            {
                var data = from x in regionRepository.FindAll()
                   .Where(x => (x.RefTerr.ID == 4 || x.RefTerr.ID == 7) && x.SourceID == extension.RegionSource.ID)
                   .OrderBy(x => x.RefTerr.ID)
                   .OrderBy(x => x.Name)
                        select new
                            {
                                x.ID,
                                x.Name
                            };

                return new AjaxStoreResult(data.ToList(), data.ToList().Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        public AjaxStoreResult CheckCanAccept(int orgId, int periodId)
        {
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            int markId;
            var marks = marksGkhRepository.FindAll();
            var can = CheckIfAllPlansFilled(marks, orgId, periodId, out markId);
            result.SaveResponse.Success = can;
            result.SaveResponse.Message = can
                ? string.Empty
                : "Утверждение невозможно, так как не все показатели заполнены, например {0}".FormatWith(markId);
            return result;
        }

        private bool CheckIfAllPlansFilled(
            IEnumerable<D_Org_MarksGKH> marks, 
            int orgId, 
            int periodId, 
            out int markId)
        {
            markId = 0;

            foreach (var mark in marks)
            {
                var planData = planRepository.FindAll().FirstOrDefault(x =>
                                            x.RefRegistrOrg.ID == orgId &&
                                            x.RefYearDayUNV.ID == periodId &&
                                            x.SourceID == sourceId &&
                                            x.RefMarksGKH.ID == mark.ID);

                if (periodId % 100 == 0)
                {
                    if (planData == null)
                    {
                        markId = mark.ID;
                        return false;
                    }

                    // проверить все показатели, чтобы кроме показателей с крестами, значения были заполнены
                    if (!(mark.PrPlanY.Equals("W") || mark.PrPlanY.Equals("X") || planData.Value != null))
                    {
                        markId = mark.ID;
                        return false;
                    }

                    if (!(mark.PrPlanO.Equals("W") || mark.PrPlanO.Equals("X") || planData.PlanO != null))
                    {
                        markId = mark.ID;
                        return false;
                    }

                    if (!(mark.PrAssigned.Equals("W") || mark.PrAssigned.Equals("X") || planData.Assigned != null))
                    {
                        markId = mark.ID;
                        return false;
                    }

                    if (!(mark.PrPlanS.Equals("W") || mark.PrPlanS.Equals("X") || planData.PlanS != null))
                    {
                        markId = mark.ID;
                        return false;
                    }

                    if (!(mark.PrPerformed.Equals("W") || mark.PrPerformed.Equals("X") || planData.Performed != null))
                    {
                        markId = mark.ID;
                        return false;
                    }

                    if (!(mark.PrPlanOOP.Equals("W") || mark.PrPlanOOP.Equals("X") || planData.PlanOOP != null))
                    {
                        markId = mark.ID;
                        return false;
                    }

                    if (!(mark.PrAssignedOP.Equals("W") || mark.PrAssignedOP.Equals("X") || planData.AssignedOP != null))
                    {
                        markId = mark.ID;
                        return false;
                    }

                    if (!(mark.PrPlanSOP.Equals("W") || mark.PrPlanSOP.Equals("X") || planData.PlanSOP != null))
                    {
                        markId = mark.ID;
                        return false;
                    }

                    if (!(mark.PrPerformedOP.Equals("X") || planData.PerformedOP != null))
                    {
                        markId = mark.ID;
                        return false;
                    }
                }
                else
                {
                    if (planData == null && !(mark.PrPerformedOP.Equals("M") || mark.PrPerformedOP.Equals("X")))
                    {
                        markId = mark.ID;
                        return false;
                    }

                    if (!(mark.PrPerformedOP.Equals("M") || mark.PrPerformedOP.Equals("X") || planData.PerformedOP != null))
                    {
                        markId = mark.ID;
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
