using System;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers.HMAO
{
    public class FO41HMAORequestsListController : SchemeBoundController
    {
        private readonly IAppPrivilegeService appRepository;
        private readonly IRepository<FX_FX_TypeTax> typeTaxRepository;
        private readonly IFO41Extension extension;
        private readonly ICategoryTaxpayerService categoryRepository;
        private readonly IRepository<FX_State_ApplicOrg> stateApplicOrgRepository;
        private readonly ILinqRepository<D_Org_Privilege> orgRepository;

        /// <summary>
        /// Репозиторий с заявками от налогоплательщиков
        /// </summary>
        private readonly IAppPrivilegeService requestsRepository;

        public FO41HMAORequestsListController(
            IFO41Extension extension,
            IAppPrivilegeService appRepository,
            IRepository<FX_FX_TypeTax> typeTaxRepository,
            IAppPrivilegeService requestsRepository,
            ICategoryTaxpayerService categoryRepository,
            IRepository<FX_State_ApplicOrg> stateApplicOrgRepository,
            ILinqRepository<D_Org_Privilege> orgRepository)
        {
            this.appRepository = appRepository;
            this.typeTaxRepository = typeTaxRepository;
            this.extension = extension;
            this.requestsRepository = requestsRepository;
            this.categoryRepository = categoryRepository;
            this.stateApplicOrgRepository = stateApplicOrgRepository;
            this.orgRepository = orgRepository;
        }

        /// <summary>
        /// Чтение списка заявок по налогоплательщику
        /// </summary>
        /// <param name="taxPayerId">Идентификатор налогоплательщика</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Список заявок</returns>
        [Transaction]
        public RestResult Read(int taxPayerId, int periodId)
        {
            try
            {
                var data = appRepository.GetAppForTaxPayer(taxPayerId, periodId);
                return new RestResult { Success = true, Data = data };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Чтение списка заявок по категории, периоду
        /// </summary>
        /// <param name="categoryId">Идентификатор налогоплательщика</param>
        /// <param name="periodId">Идентификатор периода</param>
        /// <returns>Список заявок</returns>
        [Transaction]
        public RestResult ReadForOGV(int categoryId, int periodId)
        {
            try
            {
                var data = appRepository.GetAppForOGV(categoryId, periodId);
                return new RestResult { Success = true, Data = data };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Обработчик на открытие формы для выбора вида налога
        /// </summary>
        /// <returns>Представление с формой</returns>
        public ActionResult HMAOChooseTaxView()
        {
            var taxTypeForm = new HMAOChooseTaxView(extension);
            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", taxTypeForm);
        }

/*        /// <summary>
        /// Обработчик на открытие формы для выбора периода (список заявок для налогоплательщиков)
        /// </summary>
        /// <returns>Представление с формой</returns>
        public ActionResult HMAOChoosePeriodReqListView()
        {
            var periodChooseForm = new HMAOChoosePeriodView(extension, -1);
            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", periodChooseForm);
        }*/

        /// <summary>
        /// Обработчик на открытие формы создания новой заявки
        /// </summary>
        /// <returns>Представление с формой</returns>
        public ActionResult HMAORequestView(int applicationId, int taxTypeId, bool? wasNew, int? categoryId, int? periodId, bool? isPrevRVisible)
        {
            var requestView = (extension.UserGroup == FO41Extension.GroupTaxpayer)
                ? new HMAORequestView(
                    extension, 
                    requestsRepository, 
                    categoryRepository, 
                    extension.ResponsOrg.ID, 
                    taxTypeId, 
                    applicationId, 
                    "Реквизиты", 
                    applicationId < 1 || wasNew == true, 
                    categoryId)
                      {
                          IsPrevRequestsVisisble = isPrevRVisible != false
                      }

                : new HMAORequestView(
                    extension, 
                    requestsRepository, 
                    categoryRepository, 
                    -1, 
                    taxTypeId, 
                    applicationId, 
                    "Реквизиты", 
                    applicationId < 1 || wasNew == true,
                    categoryId)
                      {
                          IsPrevRequestsVisisble = isPrevRVisible != false
                      };
            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", requestView);
        }

        /// <summary>
        /// Возвращает список всех категорий
        /// </summary>
        /// <returns>Список категорий</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LookupTaxType()
        {
            var data = from t in typeTaxRepository.GetAll().Where(x => x.ID == 4 || x.ID == 9 || x.ID == 11)
                        select new
                        {
                            t.ID,
                            t.Name
                        };

            return new AjaxStoreResult(data);
        }

        [Transaction]
        public AjaxStoreResult ChangeState(int categoryId, int periodId, int newState)
        {
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            var requestsList = requestsRepository.FindAll().Where(x =>
                                              x.RefOrgCategory.ID == categoryId &&
                                              x.RefYearDayUNV.ID == periodId &&
                                              (x.RefStateOrg.ID != 1)).ToList();

            if (requestsList.Count() < 1)
            {
                result.SaveResponse.Success = true;
                result.SaveResponse.Message = newState == 2
                                       ? "Перевести отдельную заявку из состояния 'Создана' в состояние 'На рассмотрении у ОГВ' можно открыв заявку. Перевести все заявки в состояние 'На рассмотрении у ОГВ' можно только из состояния 'На оценке'."
                                       : "Заявки отсутствуют или находятся в состоянии 'Создана'";
                return result;
            }

            var firstRequestInList = requestsList.FirstOrDefault();
            var oldState = firstRequestInList == null ? 1 : firstRequestInList.RefStateOrg.ID;
            var canChange = (newState == 2 && oldState <= 3) ||
                            (newState == 3 && (oldState == 2 || oldState == 4 || oldState == 5)) ||
                            ((newState == 4 || newState == 5) && oldState == 3);

            var state = stateApplicOrgRepository.Get(newState); 
            if (!canChange)
            {
                var oldStateName = stateApplicOrgRepository.Get(oldState).Name; 
                result.SaveResponse.Success = true;
                result.SaveResponse.Message = "Невозможно перевести заявки из состояния '{0}' в состояние '{1}'"
                    .FormatWith(oldStateName, state.Name);
                return result;
            }

            foreach (var request in requestsList)
            {
                request.RefStateOrg = state;
                requestsRepository.Save(request);
            }

            result.SaveResponse.Success = true;
            result.SaveResponse.Message = "Состояние заявок изменено";

            return result;
        }

        [Transaction]
        public AjaxStoreResult Save(int applicationId, int stateId)
        {
            var request = requestsRepository.Get(applicationId);
            if (request != null)
            {
                request.RefStateOrg = stateApplicOrgRepository.Get(stateId);
                requestsRepository.Save(request);
            }

            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            result.SaveResponse.Message = "Состояние изменено";
            result.SaveResponse.Success = true;

            return result;
        }
        
        public AjaxStoreResult CheckCanCreateRequestHMAO(int periodId, int categoryId)
        {
            var existReqWhithStateNotCreated = requestsRepository.FindAll().Any(x =>
                                                               x.RefYearDayUNV.ID >= periodId &&
                                                               (x.RefOrgCategory.ID == categoryId || categoryId < 1) &&
                                                               x.RefYearDayUNV.ID < ((periodId / 10000) + 1) * 10000 &&
                                                               x.RefStateOrg.ID >= 3);
            var canAddRequest = periodId == extension.GetPrevPeriod() && !existReqWhithStateNotCreated;
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            result.SaveResponse.Success = canAddRequest;
            result.SaveResponse.Message = canAddRequest ? string.Empty : "Срок подачи заявок истек";

            if (!canAddRequest)
            {
                return result;
            }

            // если категория больше нуля и пользователь ЭО, то проверяем по организациям
            // проверить, остались ли в предыдущем периоде организации, по которым заявка еще не подана
            if (categoryId > 0 && extension.UserGroup == FO41Extension.GroupOGV)
            {
                var orgToDecludeList = from x in requestsRepository.FindAll()
                .Where(x => x.RefOrgCategory.ID == categoryId && x.RefYearDayUNV.ID == extension.GetPrevPeriod())
                                       select new { x.RefOrgPrivilege.ID };

                var orgToDeclude = orgToDecludeList.Select(x => x.ID).ToList();

                var data = (from r in orgRepository.FindAll()
                            where !orgToDeclude.Contains(r.ID)
                            select new
                            {
                                r.ID
                            }).ToList();

                result.SaveResponse.Success = data.Count > 0;
                result.SaveResponse.Message = data.Count > 0 ? string.Empty : "По категории '{0}' для всех организаций заявки созданы"
                    .FormatWith(categoryRepository.GetQueryOne(categoryId).ShortName);
            }
            else
                if (extension.UserGroup == FO41Extension.GroupTaxpayer && extension.ResponsOrg != null)
                {
                    // проверить, остались ли в предыдущем периоде категории, по которым заявка еще не подана
                    var categoryToDecludeList = from x in requestsRepository.FindAll()
                        .Where(x => x.RefOrgPrivilege.ID == extension.ResponsOrg.ID && x.RefYearDayUNV.ID == extension.GetPrevPeriod())
                                                select new { x.RefOrgPrivilege.ID };

                    var categoryToDeclude = categoryToDecludeList.Select(x => x.ID).ToList();

                    var data = (from r in orgRepository.FindAll()
                                where !categoryToDeclude.Contains(r.ID)
                                select new
                                {
                                    r.ID
                                }).ToList();

                    result.SaveResponse.Success = data.Count > 0;
                    result.SaveResponse.Message = data.Count > 0 ? string.Empty : "В выбранном периоде заявки по всем категориям поданы"
                        .FormatWith(categoryRepository.GetQueryOne(categoryId).ShortName);
                }

            return result;
        }

        public AjaxStoreResult GetCntOrgs(int categoryId, int periodId)
        {
            var cntOrgs = requestsRepository.FindAll().Count(x => 
                x.RefYearDayUNV.ID == periodId && 
                x.RefStateOrg.ID != 1 && 
                x.RefOrgCategory.ID == categoryId);

            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            result.SaveResponse.Success = true;
            result.SaveResponse.Message = cntOrgs.ToString();

            return result;
        }
    }
}
