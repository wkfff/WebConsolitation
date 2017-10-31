using System;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.FO41.Presentation.Views.HMAO;
using Krista.FM.RIA.Extensions.FO41.Services;

namespace Krista.FM.RIA.Extensions.FO41.Presentation.Controllers.HMAO
{
    public class FO41HMAORequestsController : SchemeBoundController
    {
        private readonly AppPrivilegeService requestsRepository;
        private readonly CategoryTaxpayerService categoryRepository;
        private readonly IRepository<D_Org_Privilege> orgRepository;
        private readonly IRepository<FX_Date_YearDayUNV> periodRepository;
        private readonly IRepository<FX_State_ApplicOrg> stateApplicOrgRepository;
        private readonly AppFromOGVService appFromOGVRepository;
        private readonly IFO41Extension extension;
        private readonly IRepository<FX_FX_TypeTax> typeTaxRepository;
        private readonly IRepository<B_Regions_Bridge> regionsBridgeRepository;

        public FO41HMAORequestsController(
            IFO41Extension extension,
            AppPrivilegeService requestsRepository,
            CategoryTaxpayerService categoryRepository,
            IRepository<D_Org_Privilege> orgRepository,
            IRepository<FX_Date_YearDayUNV> periodRepository,
            IRepository<FX_State_ApplicOrg> stateApplicOrgRepository,
            AppFromOGVService appFromOGVRepository,
            IRepository<T_Doc_ApplicationOrg> filesRepository,
            IRepository<FX_FX_TypeTax> typeTaxRepository,
            IRepository<B_Regions_Bridge> regionsBridgeRepository)
        {
            this.extension = extension;
            this.requestsRepository = requestsRepository;
            this.categoryRepository = categoryRepository;
            this.orgRepository = orgRepository;
            this.periodRepository = periodRepository;
            this.stateApplicOrgRepository = stateApplicOrgRepository;
            this.appFromOGVRepository = appFromOGVRepository;
            this.typeTaxRepository = typeTaxRepository;
            this.regionsBridgeRepository = regionsBridgeRepository;
        }

        [HttpPost]
        [Transaction]
        public AjaxStoreResult Save(int applicaionId, int? taxPayerId, int? taxTypeId, FormCollection values, int? state)
        {
            var result = new AjaxStoreResult();

            if (state == null || state == 0)
            {
                state = 1;
            }

            try
            {
                // проверить данные на корректность  (реквизиты)
                // если заявка существующая - обновляем реквизиты, иначе - создаем новую
                D_Application_Privilege application = requestsRepository.Get(applicaionId);
                if (application != null)
                {
                    application.Executor = values["requestPersonsValue"];
                    application.RefOrgCategory = categoryRepository.Get(Convert.ToInt32(values["requestCategoryValue_Value"]));
                    application.RefStateOrg = stateApplicOrgRepository.Get((int)state);
                    taxPayerId = application.RefOrgPrivilege.ID;
                    }
                else
                {
                    if (taxTypeId == null)
                    {
                        result.ResponseFormat = StoreResponseFormat.Save;
                        result.SaveResponse.Success = false;
                        result.SaveResponse.Message = "Заявка не сохранена. Налогоплательщик не определен.";
                        return result;
                    }

                    application = new D_Application_Privilege
                    {
                        Executor = values["requestPersonsValue"],
                        RefOrgCategory =
                            categoryRepository.Get(
                                Convert.ToInt32(values["requestCategoryValue_Value"])),
                        RequestDate = DateTime.Parse(values["requestDateValue"]),
                        RowType = 0,
                        RefApplicOGV = appFromOGVRepository.Get(-1),

                        // указать период, основываясь на текущем годе
                        // создавать заявку можно только за предыдущий период
                        RefYearDayUNV = periodRepository.Get(extension.GetPrevPeriod()),
                        RefStateOrg = stateApplicOrgRepository.Get((int)state),
                        RefTypeTax = typeTaxRepository.Get((int)taxTypeId)
                    };

                    if (extension.UserGroup == FO41Extension.GroupTaxpayer)
                    {
                        if (taxPayerId != null)
                        {
                            application.RefOrgPrivilege = orgRepository.Get((int)taxPayerId);
                        }
                    }
                    else
                    {
                        taxPayerId = Convert.ToInt32(values["requestOrgValue_Value"]);
                        application.RefOrgPrivilege = orgRepository.Get((int)taxPayerId);
                    }
                }

                // сохраняем заявку
                requestsRepository.Save(application);

                // заполняем реквизиты налогоплательщика
                if (taxPayerId != null)
                {
                    var org = orgRepository.Get((int)taxPayerId);
                    org.RefBridgeRegions = regionsBridgeRepository
                        .Get(Convert.ToInt32(values["requestRegionValue_Value"]));
                    org.LegalAddress = values["requestLegalAddressTextValue"];
                    org.Address = values["requestAddressTextValue"];
                    var unitStr = values["requestUnitTextValue"];
                    if (unitStr.Equals(string.Empty))
                    {
                        unitStr = "0";
                    }

                    org.Unit = Convert.ToInt32(unitStr);
                    var okatoStr = values["requestOKATOTextValue"];
                    if (okatoStr.Equals(string.Empty))
                    {
                        okatoStr = "0";
                    }

                    org.OKATO = long.Parse(okatoStr);

                    // сохраняем налогоплательщика
                    orgRepository.Save(org);
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

        public ActionResult ShowTaxView(int taxId, int periodID)
        {
            var taxView = new HMAOTaxView(extension, categoryRepository)
            {
                TaxId = taxId,
                PeriodId = periodID
            };

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", taxView);
        }

        public ActionResult ShowPeriodView(int taxId)
        {
            var periodView = new HMAOChoosePeriodView(extension, taxId);
            
            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", periodView);
        }

        public ActionResult ShowReqListForTaxPayerView(int periodId)
        {
            var taxView = new HMAOReqListForTaxPayerView(extension, requestsRepository, periodId);

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", taxView);
        }
    }
}
