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
using Krista.FM.RIA.Extensions.OrgGKH.Presentation.Models;
using Krista.FM.RIA.Extensions.OrgGKH.Services;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Controllers
{
    public class MonthlyController : SchemeBoundController
    {
        /// <summary>
        /// Репозиторий «Организации.ОРГАНИЗАЦИИ_Сбор по ЖКХ (f.Org.GKH)»
        /// </summary>
        private readonly ILinqRepository<F_Org_GKH> planRepository;

        /// <summary>
        /// Репозиторий Организации.Показатели ЖКХ
        /// </summary>
        private readonly IRepository<D_Org_MarksGKH> marksGkhRepository;

        /// <summary>
        /// Репозиторий периодов
        /// </summary>
        private readonly ILinqRepository<FX_Date_YearDayUNV> periodRepository;

        /// <summary>
        /// Репозиторий статусов
        /// </summary>
        private readonly ILinqRepository<FX_Org_StatusD> statusDRepository;

        /// <summary>
        /// Репозиторий организаций
        /// </summary>
        private readonly ILinqRepository<D_Org_RegistrOrg> orgRepository;

        private readonly IList<D_Org_MarksGKH> marksGkh;
        private readonly int sourceId;

        // список планов для текущей организации, на выбранный период и источник
        private List<F_Org_GKH> orgPlans; 

        public MonthlyController(
            IOrgGkhExtension extension,
            IRepository<D_Org_MarksGKH> marksGkhRepository,
            ILinqRepository<F_Org_GKH> planRepository,
            ILinqRepository<FX_Org_StatusD> statusDRepository,
            ILinqRepository<FX_Date_YearDayUNV> periodRepository,
            ILinqRepository<D_Org_RegistrOrg> orgRepository)
        {
            this.marksGkhRepository = marksGkhRepository;
            this.planRepository = planRepository;
            this.statusDRepository = statusDRepository;
            this.periodRepository = periodRepository;
            this.orgRepository = orgRepository;
            marksGkh = marksGkhRepository.GetAll()
                .OrderBy(x => ((x.Code % 100 != 0 ? 1 : 2) * 10) + (((x.Code / 100) % 100 != 0 ? 1 : 2) * 100))
                .ToList();
            sourceId = (extension.DataSource == null) ? -1 : sourceId = extension.DataSource.ID;
        }

        /// <summary>
        /// Чтение данных по периоду
        /// </summary>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="orgId">Идентификатор организации</param>
        /// <returns>Список фактов</returns>
        [Transaction]
        public ActionResult Read(int periodId, int orgId)
        {
            try
            {
                if (sourceId == -1)
                {
                    return new AjaxStoreResult(new List<int>(), 0);
                }

                var data = new List<MonthlyDataModel>();
                var tempId = -2;

                orgPlans = planRepository.FindAll()
                    .Where(x => x.RefRegistrOrg.ID == orgId && x.SourceID == sourceId)
                    .ToList();

                // статус значения по показателю
                var forStatusPlan = orgPlans.FirstOrDefault(x => 
                    x.RefYearDayUNV.ID == periodId && 
                    x.RefMarksGKH.Code != 50000 && 
                    x.RefMarksGKH.Code != 60000);

                var statusId = forStatusPlan == null ? OrgGKHConsts.StateEdited : forStatusPlan.RefStatusD.ID;
                var status = statusDRepository.FindOne(statusId);
                var org = orgRepository.FindOne(orgId);
                var period = periodRepository.FindOne(periodId);
                
                foreach (var mark in marksGkh)
                {
                    var fact = new MonthlyDataModel
                                   {
                                       MarkId = mark.ID,
                                       MarkName = mark.Name,
                                       PrPlanY = mark.PrPlanY,
                                       PrPlanO = mark.PrPlanO,
                                       PrAssigned = mark.PrAssigned,
                                       PrPerformed = mark.PrPerformed,
                                       Code = "{0}.{1}.{2}"
                                        .FormatWith(mark.Code / 10000, (mark.Code / 100) % 100, mark.Code % 100),
                                       PrPlanS = mark.PrPlanS,
                                       PrPlanOOP = mark.PrPlanOOP,
                                       PrAssignedOP = mark.PrAssignedOP,
                                       PrPerformedOP = mark.PrPerformedOP,
                                       PrPlanSOP = mark.PrPlanSOP
                                   };
                    var planData = orgPlans.FirstOrDefault(x => 
                        x.RefMarksGKH.ID == mark.ID && 
                        x.RefYearDayUNV.ID == periodId);

                    fact.Value = FactValueService.GetPlanY(periodId, mark, planData, marksGkh, orgPlans);

                    fact.PlanO = FactValueService.GetPlanO(periodId, mark, planData, marksGkh, orgPlans);
                    fact.Assigned = FactValueService.GetAssigned(periodId, mark, planData, marksGkh, orgPlans);
                    fact.PlanS = FactValueService.GetPlanS(periodId, mark, planData, marksGkh, orgPlans);
                    fact.Performed = FactValueService.GetPerformed(periodId, mark, planData, marksGkh, orgPlans);

                    fact.PlanOOP = FactValueService.GetPlanOOP(mark, planData, periodId, marksGkh, orgPlans);
                    fact.AssignedOP = FactValueService.GetAssignedOP(mark, planData, periodId, marksGkh, orgPlans);
                    fact.PlanSOP = FactValueService.GetPlanSOP(periodId, mark, planData, marksGkh, orgPlans);
                    fact.PerformedOP = FactValueService.GetPerformedOP(periodId, mark, planData, marksGkh, orgPlans);
                    if (planData == null)
                    {
                        fact.ID = tempId;
                        fact.Status = statusId;
                        tempId--;
                        planData = new F_Org_GKH
                                       {
                                           RefStatusD = status,
                                           RefMarksGKH = mark,
                                           RefRegistrOrg = org,
                                           RefYearDayUNV = period,
                                           SourceID = sourceId
                                       };
                    }

                    planData.Value = fact.Value;
                    planData.PlanO = fact.PlanO;
                    planData.PlanS = fact.PlanS;
                    planData.Assigned = fact.Assigned;
                    planData.Performed = fact.Performed;
                    planData.PlanOOP = fact.PlanOOP;
                    planData.PlanSOP = fact.PlanSOP;
                    planData.AssignedOP = fact.AssignedOP;
                    planData.PerformedOP = fact.PerformedOP;

                    planRepository.Save(planData);

                    var index = orgPlans.FindIndex(x => x.ID == planData.ID);
                    if (index < 0)
                    {
                        orgPlans.Add(planData);
                    }
                    else
                    {
                        orgPlans[index] = planData;
                    }

                    fact.ID = planData.ID;
                    fact.Status = planData.RefStatusD.ID;
                    data.Add(fact);
                }

                data = data.OrderBy(x => x.Code).ToList();

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [Transaction]
        public ActionResult ReadWeek(int periodId, int orgId)
        {
            try
            {
                if (sourceId == -1)
                {
                    return new AjaxStoreResult(new List<int>(), 0);
                }

                var minPeriodValue = (periodId / 10000) * 10000;
                var maxPeriodValue = (periodId / 100) * 100;

                var data = new List<WeeklyDataModel>();
                var marks = marksGkh.Where(x => x.PrPerformedOP != "X");
                orgPlans = planRepository.FindAll()
                    .Where(x => x.RefRegistrOrg.ID == orgId && x.SourceID == sourceId)
                    .ToList();

                // статус значения по показателю
                var forStatusPlan = orgPlans.FirstOrDefault(x => 
                    x.RefRegistrOrg.ID == orgId && 
                    x.RefYearDayUNV.ID == periodId && 
                    x.SourceID == sourceId);

                var status = forStatusPlan == null ? OrgGKHConsts.StateEdited : forStatusPlan.RefStatusD.ID;
                var yearPlansForAllMarks = orgPlans.Where(x =>
                              x.RefYearDayUNV.ID > minPeriodValue &&
                              x.RefYearDayUNV.ID < maxPeriodValue &&
                              x.RefYearDayUNV.ID % 100 == 0);

                foreach (var mark in marks)
                {
                    var plan = new WeeklyDataModel
                    {
                        MarkName = mark.Name,
                        MarkId = mark.ID,
                        Code = "{0}.{1}.{2}".FormatWith(mark.Code / 10000, (mark.Code / 100) % 100, mark.Code % 100),
                        PrPerformedOP = mark.PrPerformedOP,
                        Status = status
                    };

                    var planData = orgPlans.FirstOrDefault(x => 
                                                  x.RefYearDayUNV.ID == periodId &&
                                                  x.RefMarksGKH.ID == mark.ID);

                    if (planData == null)
                    {
                        planData = new F_Org_GKH
                        {
                            RefStatusD = statusDRepository.FindOne(status),
                            RefMarksGKH = mark,
                            RefRegistrOrg = orgRepository.FindOne(orgId),
                            RefYearDayUNV = periodRepository.FindOne(periodId),
                            SourceID = sourceId
                        };
                    }

                    plan.PerformedOP = mark.PrPerformedOP.Equals("AS")
                                           ? FactValueService.GetASValue(marksGkh, orgPlans, mark.Code, fact => fact.PerformedOP, periodId)
                                           : planData.PerformedOP;

                    planData.PerformedOP = plan.PerformedOP;
                    planRepository.Save(planData);
                    plan.ID = planData.ID;

                    /*В ячейки столбца выводится значение поля «План на год (Value)» 
                     * из таблицы фактов «Организации.ОРГАНИЗАЦИИ_Сбор по ЖКХ (f.Org.GKH)». 
                     * Значения прочих атрибутов должны быть такими:
                     * RefYearDayUNV – последний имеющийся период текущего года;
                     * RefRegistrOrg – выбранная в форме ввода организация;
                     * RefMarksGKH –  соответствующий показатель в строке;
                     * RefStatusD – «Заблокирован» (ID=2).
                     * Источник - ОРГАНИЗАЦИИ\0012 Сбор по ЖКХ*/

                    var yearPlanFact = yearPlansForAllMarks.Where(x => x.RefMarksGKH.ID == mark.ID)
                        .OrderBy(x => x.RefYearDayUNV.ID)
                        .LastOrDefault();

                    plan.YearPlan = yearPlanFact == null ? null : yearPlanFact.Value;

                    data.Add(plan);
                }

                data = data.OrderBy(x => x.Code).ToList();

                return new AjaxStoreResult(data, data.Count);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Сохранение данных по ежемесячному сбору
        /// </summary>
        /// <param name="periodId">Идентификатор периода</param>
        /// <param name="orgId">Идентификатор организации</param>
        /// <param name="data">Параметры записи</param>
        /// <param name="isMonth">Признак - на период по месяцу сохранение или по неделе</param>
        /// <param name="statusId">Идентификатор статуса</param>
        /// <returns>Результат сохранения</returns>
        [HttpPost]
        [Transaction]
        public RestResult Save(int periodId, int orgId, string data, bool isMonth, int? statusId)
        {
            try
            {
                if (sourceId == -1)
                {
                    return new RestResult
                    {
                        Success = false,
                        Message = "Сохранено",
                        Data = new List<int>()
                    };
                }

                if (statusId != null)
                {
                    var status = (int)statusId;

                    // изменилось состояние, поэтому 
                    // у всех записей в таблице «Организации.ОРГАНИЗАЦИИ_Сбор по ЖКХ (f.Org.GKH)» 
                    // по выбранному периоду и выбранным организациям должна произойти смена состояния
                    var plans = planRepository.FindAll().Where(x =>
                        x.RefRegistrOrg.ID == orgId && 
                        x.RefYearDayUNV.ID == periodId && 
                        x.SourceID == sourceId);

                    foreach (var plan in plans)
                    {
                        plan.RefStatusD = statusDRepository.FindOne(status);
                        planRepository.Save(plan);
                    }

                    return new RestResult
                    {
                        Success = true,
                        Message = "Сохранено",
                        Data = new List<int>()
                    };
                }

                var returnResult = new List<Dictionary<string, object>>();

                var dataHandler = new StoreDataHandler(data);
                var dataSet = dataHandler.ObjectData<Dictionary<string, object>>();

                var yearDayUNV = periodRepository.FindOne(periodId);
                var statusOnEdit = statusDRepository.FindOne(OrgGKHConsts.StateEdited);
                foreach (var updated in dataSet.Updated)
                {
                    var id = Convert.ToInt32(updated["ID"]);
                    var markId = Convert.ToInt32(updated["MarkId"]);
                    var markGKH = marksGkhRepository.Get(markId);

                    var orgGkh = (id > 0)
                                     ? planRepository.FindOne(id)
                                     : new F_Org_GKH
                                           {
                                               RefMarksGKH = markGKH,
                                               RefStatusD = statusOnEdit,
                                               RefRegistrOrg = orgRepository.FindOne(orgId),
                                               RefYearDayUNV = yearDayUNV,
                                               SourceID = sourceId
                                           };

                    orgGkh.Value = GetMarkValue(updated["Value"]);

                    orgGkh.PlanO = GetMarkValue(updated["PlanO"]);
                    orgGkh.PlanS = GetMarkValue(updated["PlanS"]);
                    orgGkh.Assigned = GetMarkValue(updated["Assigned"]);
                    orgGkh.Performed = GetMarkValue(updated["Performed"]);

                    orgGkh.PlanOOP = GetMarkValue(updated["PlanOOP"]);
                    orgGkh.PlanSOP = GetMarkValue(updated["PlanSOP"]);
                    orgGkh.AssignedOP = GetMarkValue(updated["AssignedOP"]);
                    orgGkh.PerformedOP = GetMarkValue(updated["PerformedOP"]);

                    if (markGKH != null)
                    {
                        planRepository.Save(orgGkh);
                        updated["ID"] = orgGkh.ID.ToString();
                        returnResult.Add(updated);
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

        [HttpPost]
        [Transaction]
        public RestResult SaveWeek(int periodId, int orgId, string data, bool isMonth)
        {
            try
            {
                if (sourceId == -1)
                {
                    return new RestResult
                    {
                        Success = false,
                        Message = "Сохранено",
                        Data = new List<int>()
                    };
                }

                var returnResult = new List<Dictionary<string, object>>();

                var dataHandler = new StoreDataHandler(data);
                var dataSet = dataHandler.ObjectData<Dictionary<string, object>>();

                var yearDayUNV = periodRepository.FindOne(periodId);
                var statusOnEdit = statusDRepository.FindOne(OrgGKHConsts.StateEdited);
                foreach (var updated in dataSet.Updated)
                {
                    var id = Convert.ToInt32(updated["ID"]);
                    var markId = Convert.ToInt32(updated["MarkId"]);
                    var markGKH = marksGkhRepository.Get(markId);

                    var orgGkh = (id > 0)
                                     ? planRepository.FindOne(id)
                                     : new F_Org_GKH
                                     {
                                         RefMarksGKH = markGKH,
                                         RefStatusD = statusOnEdit,
                                         RefRegistrOrg = orgRepository.FindOne(orgId),
                                         RefYearDayUNV = yearDayUNV,
                                         SourceID = sourceId
                                     };

                    orgGkh.PerformedOP = GetMarkValue(updated["PerformedOP"]);

                    if (markGKH != null)
                    {
                        planRepository.Save(orgGkh);
                        updated["ID"] = orgGkh.ID.ToString();
                        returnResult.Add(updated);
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

        public AjaxStoreResult CheckEditable(int orgId, int periodId)
        {
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };

            var planForOrgFirst = planRepository.FindAll().FirstOrDefault(x => 
                    x.RefRegistrOrg.ID == orgId && 
                    x.RefYearDayUNV.ID == periodId && 
                    x.RefMarksGKH.Code != 50000 && 
                    x.RefMarksGKH.Code != 60000);
            var status = (planForOrgFirst == null) ? OrgGKHConsts.StateEdited : planForOrgFirst.RefStatusD.ID;
            var editable = status == OrgGKHConsts.StateEdited;
            result.SaveResponse.Success = editable;
            result.SaveResponse.Message = string.Empty;
            return result;
        }

        private static decimal? GetMarkValue(object uiValue)
        {
            if (uiValue == null)
            {
                return null;
            }

            if (!uiValue.Equals('X'))
            {
                try
                {
                    return Convert.ToDecimal(uiValue);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }
   }
}
