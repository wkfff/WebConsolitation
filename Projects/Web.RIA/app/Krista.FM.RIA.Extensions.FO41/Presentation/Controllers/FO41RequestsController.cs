using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Presentation.Views;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers
{
    public class FO41RequestsController : SchemeBoundController
    {
        private readonly IAppPrivilegeService requestsRepository;
        private readonly CategoryTaxpayerService categoryRepository;
        private readonly ILinqRepository<D_Org_Privilege> orgRepository;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly IRepository<FX_State_ApplicOrg> stateApplicOrgRepository;
        private readonly AppFromOGVService appFromOGVRepository;
        private readonly ILinqRepository<T_Doc_ApplicationOrg> filesRepository;
        private readonly IRepository<FX_FX_TypeTax> typeTaxRepository;
        private readonly IFO41Extension extension;

        public FO41RequestsController(
            IFO41Extension extension,
            IAppPrivilegeService requestsRepository,
            CategoryTaxpayerService categoryRepository,
            ILinqRepository<D_Org_Privilege> orgRepository,
            IRepository<FX_Date_YearDayUNV> periodRepository,
            IRepository<FX_State_ApplicOrg> stateApplicOrgRepository,
            AppFromOGVService appFromOGVRepository,
            ILinqRepository<T_Doc_ApplicationOrg> filesRepository,
            IRepository<FX_FX_TypeTax> typeTaxRepository)
        {
            this.extension = extension;
            this.requestsRepository = requestsRepository;
            this.categoryRepository = categoryRepository;
            this.orgRepository = orgRepository;
            this.periodRepository = periodRepository;
            this.stateApplicOrgRepository = stateApplicOrgRepository;
            this.appFromOGVRepository = appFromOGVRepository;
            this.typeTaxRepository = typeTaxRepository;
            this.filesRepository = filesRepository;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public RestResult Create(string data)
        {
            return new RestResult { Success = false, Message = string.Empty };
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult Read(int? categoryId, bool[] filter, int? periodId)
        {
            try
            {
                var filters = new List<int>();
                for (var index = 0; index < 5; index++)
                {
                    if (!filter[index])
                    {
                        filters.Add(index + 1);
                    }
                }
                
                return new RestResult
                           {
                               Success = true, 
                               Data = extension.UserGroup == FO41Extension.GroupTaxpayer
                                ? requestsRepository.GetQueryByTaxPayer(
                                    extension.ResponsOrg.ID,
                                    periodId,
                                    filters)
                                : requestsRepository.GetQueryByCategory(
                                    categoryId, 
                                    extension.ResponsOIV.ID,
                                    periodId,
                                    filters)
                           };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult ReadByCategory(int categoryId, int periodId)
        {
            try
            {
                return new RestResult { Success = true, Data = requestsRepository.GetRequestToInputData(categoryId, periodId) };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult ReadPrevPeriods(int? orgId, int periodId)
        {
            if (orgId == null)
            {
                return new RestResult { Success = true, Data = requestsRepository.GetReqPrevPeriods(-1, periodId) };
            }

            try
            {
                return new RestResult { Success = true, Data = requestsRepository.GetReqPrevPeriods((int)orgId, periodId) };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public RestResult Update(int id, string data)
        {
            try
            {
                var dataSet = JSON.Deserialize<JsonObject>(data);
                var application = requestsRepository.Get(id);
                application.RefStateOrg = stateApplicOrgRepository.Get(Convert.ToInt32(dataSet["RefStateID"]));
                requestsRepository.Save(application);
                dataSet["RefState"] = application.RefStateOrg.Name;
                return new RestResult { Success = true, Message = "Заявка обновлена", Data = dataSet };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Удаление заявки
        /// </summary>
        /// <param name="requestId">Идентификатор заявки</param>
        /// <returns>Результат удаления</returns>
        [Transaction]
        public AjaxStoreResult RemoveRequest(int requestId)
        {
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            result.SaveResponse.Success = true;
            result.SaveResponse.Message = requestsRepository.RemoveRequest(requestId);
            return result;
        }

        public AjaxStoreResult CheckCanCreateRequest(int categoryId)
        {
            // проверить, есть ли заявка от ОГВ по соотв категории, 
            // которая находится в состоянии на оценке или дальше. За предыдущий период период
            var reqs = appFromOGVRepository.FindAll()
                .Where(r =>
                       (r.RefStateOGV.ID > 1) &&
                       r.RefYearDayUNV.ID == extension.GetPrevPeriod() &&
                       r.RefOrgCategory.ID == categoryId).ToList();

            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            result.SaveResponse.Success = reqs.Count() <= 0;

            var category = categoryRepository.Get(categoryId);
            var categoryName = category == null ? string.Empty : category.ShortName;
            result.SaveResponse.Message = category == null 
                ? "Срок подачи заявки на предоставление льгот истек" 
                : "Срок подачи заявки на предоставление льгот по категории '{0}' истек"
                .FormatWith(categoryName);

            if (result.SaveResponse.Success)
            {
                // проверить, остались ли в предыдущем периоде организации, по которым заявка еще не подана
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
                result.SaveResponse.Message = "По категории '{0}' для всех организаций заявки созданы"
                    .FormatWith(categoryName);
            }

            return result;
        }
        
        public ActionResult ShowRequest(
            int applicationId, 
            int? categoryId, 
            string categoryName, 
            bool? isCopy, 
            bool? wasNew, 
            string activeTab, 
            bool? isPrevRVisible)
        {
            var requstView = new RequestView(
                extension, 
                requestsRepository, 
                appFromOGVRepository, 
                categoryRepository, 
                applicationId, 
                categoryId, 
                isCopy, 
                wasNew, 
                activeTab);

            if (isPrevRVisible != null)
            {
                requstView.IsPrevRequestsVisisble = (bool)isPrevRVisible;
            }

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", requstView);
        }

        public ActionResult ShowRequestsListView(int periodId)
        {
            var requestsList = new RequestsListView(categoryRepository, requestsRepository, extension, periodId);
            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", requestsList);
        }

        public ActionResult ShowReqToEstimateListView(int periodId)
        {
            var requestsList = new EstimateRequestsListView(
                extension,
                new NHibernateLinqRepository<D_OMSU_ResponsOIV>(), 
                periodId);
            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", requestsList);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult ReadOne(int id)
        {
            try
            {
                return new RestResult { Success = true, Data = requestsRepository.GetQueryOne(id) };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [HttpPost]
        [Transaction]
        public AjaxStoreResult Save(int applicaionId, int? copyApplicId, FormCollection values, int state)
        {
            var result = new AjaxStoreResult();

            if (state == 0)
            {
                state = 1;
            }

            try
            {
                // проверить данные на корректность  (реквизиты)
                // если заявка существующая - обновляем реквизиты, иначе - создаем новую
                D_Application_Privilege application;
                if (applicaionId > -1)
                {
                    application = requestsRepository.Get(applicaionId);
                    application.Executor = values["requestPersonsValue"];
                    application.RefOrgCategory =
                        categoryRepository.Get(Convert.ToInt32(values["requestCategoryValue_Value"]));
                    application.RefStateOrg = stateApplicOrgRepository.Get(state);

                    // если в состоянии "Создана", не должно быть ссылки на заявку от ОГВ 
                    // (не должна быть включена в заявку)
                    if (state == 1)
                    {
                        application.RefApplicOGV = appFromOGVRepository.Get(-1);
                    }
                }
                else
                {
                    application = new D_Application_Privilege
                                      {
                                          Executor = values["requestPersonsValue"],
                                          RefOrgCategory =
                                              categoryRepository.Get(
                                                  Convert.ToInt32(values["requestCategoryValue_Value"])),
                                          RequestDate = DateTime.Parse(values["requestDateValue"]),
                                          RowType = 0,
                                          RefOrgPrivilege = orgRepository.FindOne(Convert.ToInt32(values["requestOrgPrivilege_Value"])),
                                          RefApplicOGV = appFromOGVRepository.Get(-1),

                                          // указать период, основываясь на годе из даты заявки
                                          // новая заяка создается только на предыдущий период
                                          RefYearDayUNV = periodRepository.Get(extension.GetPrevPeriod()),
                                          RefStateOrg = stateApplicOrgRepository.Get(state),
                                      };
                }

                application.RefTypeTax = typeTaxRepository.Get(-1);

                // сохраняем заявку
                requestsRepository.Save(application);

                if (copyApplicId != null)
                {
                    if (copyApplicId > -1)
                    {
                        // это копия заявки, надо скопировать документы
                        var files = from f in filesRepository.FindAll().Where(x => x.RefApplicOrg.ID == copyApplicId) 
                                    select new T_Doc_ApplicationOrg
                                      {
                                          Name = f.Name,
                                          Note = f.Note,
                                          Doc = f.Doc,
                                          DateDoc = f.DateDoc,
                                          Executor = f.Executor,
                                          RefApplicOrg = application
                                      };

                        foreach (var file in files)
                        {
                            var newFile = new T_Doc_ApplicationOrg
                                              {
                                                  Name = file.Name,
                                                  Note = file.Note,
                                                  Doc = new byte[1],
                                                  DateDoc = file.DateDoc,
                                                  Executor = file.Executor,
                                                  RefApplicOrg = requestsRepository.Get(application.ID)
                                              };
                            filesRepository.Save(newFile);
                        }
                    }
                }

                result.ResponseFormat = StoreResponseFormat.Save;
                result.SaveResponse.Success = true;
                result.SaveResponse.Message = application.ID.ToString();
                values.Set("requestNumberValue", application.ID.ToString());
                result.Data = values;

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

        [HttpPost]
        [Transaction]
        public AjaxStoreResult UpdateIncluded(int appFromOGVId)
        {
            var dataHandler = new StoreDataHandler(HttpContext.Request["data"]);
            var dataSet = JsonDataSetParser.Parse(dataHandler.JsonData);

            if (dataSet.ContainsKey("Included"))
            {
                var appFromOGVState = appFromOGVRepository.Get(appFromOGVId).RefStateOGV.ID;
                var stateNotIncluded = stateApplicOrgRepository.Get(2);
                var stateIncluded = appFromOGVState == 2 ? stateApplicOrgRepository.Get(3) : stateNotIncluded;
                var table = dataSet["Included"];
                foreach (var item in table)
                {
                    if (item.ContainsKey("ID") && item.ContainsKey("Included"))
                    {
                        var application = requestsRepository.Get(Convert.ToInt32(item["ID"].ToString()));
                        var included = Convert.ToBoolean(item["Included"].ToString());
                        
                        application.RefApplicOGV = included
                            ? application.RefApplicOGV = appFromOGVRepository.Get(appFromOGVId)
                            : null;

                        application.RefStateOrg = included ? stateIncluded : stateNotIncluded;

                        requestsRepository.Save(application);
                    }
                }
            }

            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
            result.SaveResponse.Success = true;
            result.SaveResponse.Message = "Included";

            return result;
        }

        public ActionResult Expand(int applicationId)
        {
            var result = new AjaxResult();

            var orignRequest = requestsRepository.Get(applicationId);

            var requests = requestsRepository.FindAll().Where(f =>
                f.ID != applicationId &&
                f.RefOrgPrivilege.ID == orignRequest.RefOrgPrivilege.ID &&
                f.RefYearDayUNV.ID == orignRequest.RefYearDayUNV.ID);

            if (requests.Count() <= 0)
            {
                return result;
            }

            var label = new Label
                            {
                                Html = String.Format(
                                    "<div class=\"x-window-mc\">\r\n<p><b>Копии по категориям:</b></p>")
                            };

            foreach (var request in requests)
            {
                var onClickHandler = @"
                            parent.MdiTab.addTab({{ 
                                title: {1}Редактирование заявки{1}, 
                                url: {1}/FO41Requests/ShowRequest?applicationId={1} + {0}, 
                                icon: {1}icon-report{1} 
                            }});".FormatWith(request.ID, '"');
                label.Html += @"
                    <p><b>{0}</b> {1} 
                        <button 
                            type=button 
                            class='x-btn-text icon-applicationgo' 
                            style='border-style:none;'  
                            onclick='{2}'>&nbsp;&nbsp;&nbsp;
                        </button>
                    </p>".FormatWith(request.RefOrgCategory.ShortName + ":", "заявка № " + request.ID, onClickHandler);
            }

            label.Html += @"</div>";

            result.Script = label.ToScript(RenderMode.RenderTo, "row-" + applicationId);

            return result;
        }

        public ActionResult ShowPeriodView()
        {
            var periodView = new ChoosePeriodView(extension);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", periodView);
        }
    }
}
