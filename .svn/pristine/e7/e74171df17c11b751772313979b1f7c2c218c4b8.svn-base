using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Presentation.Views;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers
{
    public class FO41EstimateController : SchemeBoundController
    {
        private readonly AppPrivilegeService requestsTaxpayerRepository;
        private readonly AppFromOGVService requestsRepository;
        private readonly CategoryTaxpayerService categoryRepository;
        private readonly IRepository<FX_State_ApplicOGV> stateOGVRepository;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly ILinqRepository<FX_State_ApplicOrg> stateOrgRepository;
        private readonly IFO41Extension extension;
        private readonly IRepository<D_Marks_NormPrivilege> normRepository;

        public FO41EstimateController(
                IFO41Extension extension,
                AppFromOGVService requestsRepository, 
                AppPrivilegeService requestsTaxpayerRepository,
                IRepository<D_OMSU_ResponsOIV> ogvRepository,
                CategoryTaxpayerService categoryRepository, 
                IRepository<FX_State_ApplicOGV> stateOGVRepository,
                IRepository<FX_Date_YearDayUNV> periodRepository,
                IRepository<D_Marks_NormPrivilege> normRepository,
                ILinqRepository<FX_State_ApplicOrg> stateOrgRepository)
        {
            this.extension = extension;
            this.requestsRepository = requestsRepository;
            this.requestsTaxpayerRepository = requestsTaxpayerRepository;
            this.categoryRepository = categoryRepository;
            this.stateOGVRepository = stateOGVRepository;
            this.periodRepository = periodRepository;
            this.normRepository = normRepository;
            this.stateOrgRepository = stateOrgRepository;
        }

        public ActionResult ShowRequest(int appFromOGVId, int categoryId, int periodId, string activeTab) 
        {
            var categoryShortName = requestsRepository.Get(appFromOGVId).RefOrgCategory.ShortName;
            var requstView = new EstimateRequestView(
                extension, 
                appFromOGVId, 
                categoryId,
                periodId,
                extension.ResponsOIV, 
                requestsRepository, 
                normRepository, 
                categoryShortName,
                activeTab)
                                 {
                                     Executor = User.Identity.Name
                                 };
            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", requstView);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public RestResult Create(string data)
        {
            return new RestResult { Success = false, Message = string.Empty };
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult Read(int? categoryId, int? ogvId, bool[] filter, int periodId)
        {
            try
            {
                var filters = new List<int>();
                for (int index = 0; index < 5; index++)
                {
                    if (!filter[index])
                    {
                        filters.Add(index + 1);
                    }
                }

                return new RestResult
                           {
                                Success = true, 
                                Data = extension.ResponsOIV == null 
                                    ? null
                                    : (extension.ResponsOIV.Role.Equals("ДФ") 
                                        ? requestsRepository.GetQueryByOGV(null, filters, periodId)
                                        : requestsRepository.GetQueryByOGV(extension.ResponsOIV.ID, filters, periodId))
                           };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [Transaction]
        public RestResult Destroy(int id)
        {
            try
            {
                var reglament = requestsRepository.FindOne(id);
                requestsRepository.Delete(reglament);
                requestsRepository.DbContext.CommitChanges();

                return new RestResult { Success = true, Message = "Заявка удалена" };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        [Transaction]
        public AjaxStoreResult Save(int applicaionId, int categoryId, int stateOGV, string date)
        {
            var result = new AjaxStoreResult();

            try
            {
                // проверить данные на корректность  (реквизиты)
                // сохранить заявку
                D_Application_FromOGV application;
                if (applicaionId > 0)
                {
                    application = requestsRepository.Get(applicaionId);
                    application.Executor = User.Identity.Name;
                    application.RefOrgCategory = categoryRepository.Get(categoryId);
                    application.RefStateOGV = stateOGVRepository.Get(stateOGV);
                }
                else
                {
                    application = new D_Application_FromOGV
                                      {
                                          Executor = User.Identity.Name,
                                          RowType = 0,
                                          RefOGV = extension.ResponsOIV,
                                          RefOrgCategory = categoryRepository.Get(categoryId),
                                          RefStateOGV = stateOGVRepository.Get(stateOGV),
                                          RequestDate = DateTime.Parse(date),

                                          // указать период, основываясь на текщем годе
                                          RefYearDayUNV = periodRepository.Get(extension.GetPrevPeriod()),
                                      };
                }

                requestsRepository.Save(application);

                // Изменить состояние заявок от налогопл в соотв с состоянием заявки от ОГВ, в которую они включены
                var stateOrg = (stateOGV == 2) 
                    ? stateOrgRepository.FindOne(3) 
                    : (stateOGV == 1 || stateOGV == 3
                        ? stateOrgRepository.FindOne(2) 
                        : stateOrgRepository.FindOne(stateOGV));

                // Список заявок от налогопл.по данной категории в текущем периоде кроме со статусом -  создана
                var reqs = requestsTaxpayerRepository.FindAll()
                    .Where(x => 
                        x.RefOrgCategory.ID == application.RefOrgCategory.ID && 
                        x.RefYearDayUNV.ID == application.RefYearDayUNV.ID &&
                        x.RefStateOrg.ID != 1);

                foreach (var requestTaxpayer in reqs)
                {
                    // изменяем состояние, сохраняем
                    requestTaxpayer.RefStateOrg = stateOrg;
                    requestsTaxpayerRepository.Save(requestTaxpayer);
                }

                result.ResponseFormat = StoreResponseFormat.Save;
                result.SaveResponse.Success = true;
                result.SaveResponse.Message = application.ID.ToString();

                result.Data = application;

                return result;
            }
            catch (Exception e)
            {
                result.ResponseFormat = StoreResponseFormat.Save;
                result.SaveResponse.Success = false;
                result.SaveResponse.Message = e.Message;

                return result;
            }
        }

        [Transaction]
        public RestResult Update(int id, string data)
        {
            try
            {
                var dataSet = JSON.Deserialize<JsonObject>(data);
                var application = requestsRepository.Get(id);
                var stateOGV = Convert.ToInt32(dataSet["RefStateID"]);
                application.RefStateOGV = stateOGVRepository.Get(stateOGV);
                requestsRepository.Save(application);
                
                // если заявка на оценке, все соотв заявки от налогопл. - в состояние на оценке, 
                // иначе - на рассмотрении у ОГВ
                var stateOrg = (stateOGV == 2)
                    ? stateOrgRepository.FindOne(3)
                    : (stateOGV == 1 || stateOGV == 3
                        ? stateOrgRepository.FindOne(2)
                        : stateOrgRepository.FindOne(stateOGV));

                // Список заявок от налогопл.по данной категории в текущем периоде кроме со статусом -  создана
                var reqs = requestsTaxpayerRepository.FindAll()
                    .Where(x =>
                        x.RefOrgCategory.ID == application.RefOrgCategory.ID &&
                        x.RefYearDayUNV.ID == application.RefYearDayUNV.ID &&
                        x.RefStateOrg.ID != 1);
                
                foreach (var requestTaxpayer in reqs)
                {
                    // изменяем состояние, сохраняем
                    requestTaxpayer.RefStateOrg = stateOrg;
                    requestsTaxpayerRepository.Save(requestTaxpayer);
                }

                dataSet["RefState"] = application.RefStateOGV.Name;
                return new RestResult { Success = true, Message = "Заявка обновлена", Data = dataSet };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }
    }
}
